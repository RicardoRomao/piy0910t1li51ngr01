using System;
using System.Web;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace MineSweeperFlags.Models {

	/// <summary>
	/// Estrutura de dados para recepção de 
	/// um form com os dados de registo ou
	/// de actualização de perfil de utilizador.
	/// </summary>
	public class UserRegistration : IDataErrorInfo {


		//proriedades
		public String Id        { get; set; }
		public String Name      { get; set; }
		public String Email     { get; set; }
		public String EmailConf { get; set; }
		public String Avatar    { get; set; }
		
		//novo ou update?
		public bool? IsUpdate   { get; set; }

		//erros na validação do modelo, dá jeito para testar na View
		//expõe-se em forma mista para não ser tratado no Model-Binder
		private bool m_error = true;
		public bool HasError { get { return m_error; } }
		public void SetError(bool e) { m_error = e; }


		#region controlo IDataErrorInfo

		public string Error { get { return null; } }
		public string this[string propName] {
			get {
				if ((propName == "Id") && String.IsNullOrEmpty(Id))
					return "Please enter your nick (userid)";
				if ((propName == "Name") && String.IsNullOrEmpty(Name))
					return "Please enter your name";
				if ((propName == "Email") && !Regex.IsMatch(Email, ".+\\@.+\\..+"))
					return "Please enter a valid email address";
				if ((propName == "EmailConf") && (EmailConf != Email))
					return "Confirmation email differs from first entry";
				return null;
			}
		}

		#endregion

	}
}
