using System;
using System.Collections;
using System.Collections.Generic;

namespace MineSweeperFlagsLib {

	/// <summary>
	/// Implementação de repositório de
	/// Utilizadores da aplicação
	/// </summary>
	public interface IMSFRepository {
	
		//user
		User GetUser(String id);
		void AddUser(User u);
		void RemoveUser(User u);
		void UpdateUser(User u);	//save persistent
		IEnumerable<User> Users();
		int TotalUsers();
		
		//game
		Game GetGame(int id);
		void AddGame(Game g);
		void RemoveGame(Game g);
		void UpdateGame(Game g);   //save persistent
		IEnumerable<Game> Games();
		int TotalGames();
		
	}

}
