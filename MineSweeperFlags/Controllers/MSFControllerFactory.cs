using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MineSweeperFlagsLib;

namespace MineSweeperFlags.Controllers {

	/// <summary>
	/// Redefinição do método GetControllerInstance
	/// para instanciar controllers com repositórios
	/// activos no servidor.
	/// </summary>
	public class MSFControllerFactory : DefaultControllerFactory {

		//controllers instanciados
		protected Dictionary<Type, IController> m_Controllers;

		// ----------------------------------------------
		// Instanciar repositórios em contexto de modelo.
		// Em função do tipo do controller, instanciar e
		// Devolver o controller associado. 
		protected override IController GetControllerInstance(Type controllerType) {
		
			//controller type pode ser nulo...
			if(controllerType == null) return null;
			
			//verifica se o controller já foi instanciado
			if(m_Controllers == null) m_Controllers = new Dictionary<Type, IController>();
			
			//mutex...
			lock( m_Controllers ) {
			
				if(m_Controllers.ContainsKey(controllerType)) return m_Controllers[controllerType];
				IController newController = null; 
			
				//controllers que recebem um repositório
				if (controllerType.GetConstructor(new Type[1]{typeof(IMSFRepository)}) != null) {
					newController = (IController) Activator.CreateInstance(
						controllerType,
						new object[1] {MSFEntityContainer.resolveMSFRepository()}
					);
					
				//controllerssem argumentos
				} else {
					newController = (IController)Activator.CreateInstance( controllerType );
				
				}
			
				//adicionar o controller e devolver
				m_Controllers.Add(controllerType, newController);
				return newController;
				
			}
	
		}


		// -------------------------------------------
		// Garantir libertação correcta do Controller.
		public override void ReleaseController(IController controller) {
		
			m_Controllers.Remove(controller.GetType());
			if (controller is IDisposable) (controller as IDisposable).Dispose(); 
			else controller = null;

		}
	
	}
}
