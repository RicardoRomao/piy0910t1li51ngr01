using System;
using System.Web;
using System.ComponentModel;
using MineSweeperFlagsLib;

namespace MineSweeperFlags.Models {

	/// <summary>
	/// Referência para um Utilizador.
	/// É utilizado para as chamadas de update
	/// aos dados apresentados ao cliente.
	/// </summary>
	public class UserRef {

		//proriedades
		public String UserId { get; set; }

		//o user do modelo (se fez login)
		//expõe-se na forma de métodos para não ser tratado no Model-Binder
		protected User m_user;
		public void SetUser(User user) { m_user = user; }
		public User User() { return m_user; }

		//erros na validação do modelo, dá jeito para testar na View
		//expõe-se em forma mista para não ser tratado no Model-Binder
		private bool m_error;
		public bool HasError { get { return m_error; } }
		public void SetError(bool e) { m_error = e; }



	}

}
