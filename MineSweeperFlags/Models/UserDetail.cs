using System;
using System.Collections.Generic;
using System.Web;
using System.ComponentModel;
using MineSweeperFlagsLib;

namespace MineSweeperFlags.Models {


	/// <summary>
	/// Detalhe de utilizador
	/// Extende UserRef para acrescentar DetailId 
	/// </summary>
	public class UserDetail : UserRef {

		//propriedades
		public String DetailId { get; set; }

		//out: o user para o qual se deseja o detalhe
		//expõe-se na forma de métodos para não "confundir" o controller
		private User m_detailUser;
		public void SetDetailUser(User user) { m_detailUser = user; }
		public User DetailUser() { return m_detailUser; }
		
		//out: a lista de jogos desse utilizador
		//expõe-se na forma de métodos para não "confundir" o controller
		private IEnumerable<Game> m_Games;
		public void SetGameList(IEnumerable<Game> list) { m_Games = list; }
		public IEnumerable<Game> GameList() { return m_Games; }

	}
}
