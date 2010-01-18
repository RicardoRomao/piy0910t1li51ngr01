using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MineSweeperFlagsLib {

	/// <summary>
	/// Implementação fundamental de user (utilizador)
	/// </summary>
	public class User : IEquatable<User> {
	
	

		//propriedades
		public String   Id       { get; set; }
		public String   Name     { get; set; }
		public String   Email    { get; set; }
		public String   Avatar   { get; set; }
		// todo later... public DateTime LastLog  { get; set; }

		
		//construtores
		public User() {}
		protected User(User u) {
			 Id = u.Id; 
			 Name = u.Name;
			 Email = u.Email;
			 Avatar = u.Avatar;
		}
		
		//representação serializada (parcial) JSON
		public override string ToString() {
			StringBuilder res = new StringBuilder();
			res.Append("id:\"" + (Id != null ? Id.Replace("\"","\\\"") : "") + "\"");
			res.Append(",name:\"" + (Name != null ? Name.Replace("\"","\\\"") : "") + "\"");
			res.Append(",email:\"" + (Email != null ? Email.Replace("\"","\\\"") : "") + "\"");
			res.Append(",avatar:\"" + (Avatar != null ? Avatar.Replace("\"","\\\"") : "") + "\"");
			return res.ToString();
		}


		#region Igualdade

		public override bool Equals(object o) {
			if (o == null) return false;
			if (o is User) return Equals(o as User);
			else return false;
		}

		public override int GetHashCode() { return Id.GetHashCode(); }

		public bool Equals(User other) { return (this.Id.ToLower() == other.Id.ToLower()); }

		


		#endregion
	}	
}