using System;
using System.Collections.Generic;
using System.Text;

namespace MineSweeperFlagsLib {
	
	/// <summary>
	/// Classe fornecedora de entidades master.
	/// Fornece objectos via padrão SINGLETON
	/// </summary>
	public class MSFEntityContainer {

		//Fornecer o repositório de utilizadores
		public static IMSFRepository resolveMSFRepository() { return MSFRepositoryInRam.SINGLETON; }
		
	}

}
