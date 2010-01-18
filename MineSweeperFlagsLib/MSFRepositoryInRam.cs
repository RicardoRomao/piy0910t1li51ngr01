using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;

namespace MineSweeperFlagsLib {

	/// <summary>
	/// Implementação de arquivo de utilizadores em memória.
	/// </summary>
	public class MSFRepositoryInRam :IMSFRepository {

		//SINGLETON
		public static readonly MSFRepositoryInRam SINGLETON = new MSFRepositoryInRam();

		//campos
		Dictionary<string, User>   _userDB;
		Dictionary<int, Game>      _gameDB;

		//construtor que instancia a base de dados
		private MSFRepositoryInRam() {
		
			//instanciar repositórios
			_userDB = new Dictionary<string, User>(100);
			_gameDB = new Dictionary<int, Game>(100);


			
			/******************************
			 * TESTE : CRIAR 4 UTILIZADORES
			 ******************************/
			_userDB.Add("Alpha", new User { Id = "Alpha", Name = "User Alpha", Email = "ua@mail.com", Avatar = "/Images/Avatars/AvatarA.png" });
			_userDB.Add("Beta", new User { Id = "Beta", Name = "User Beta", Email = "ub@mail.com", Avatar = "/Images/Avatars/AvatarB.png" });
			_userDB.Add("Gamma", new User { Id = "Gamma", Name = "User Gamma", Email = "uc@mail.com", Avatar = "/Images/Avatars/AvatarC.png" });
			_userDB.Add("Hepta", new User { Id = "Hepta", Name = "User Hepta", Email = "ud@mail.com", Avatar = "/Images/Avatars/AvatarD.png" });



			/****************************************
			 * TESTE : CRIAR 5 JOGOS ...
			 ****************************************/
			 
			//Jogo Owner A (na realidade este tipo de jogo não existe só c/1)
			Game g = new Game(_userDB["Alpha"]);
			_gameDB.Add(g.Id, g);

			//Jogo Owner A a desafiar o B e o G
			g = new Game(_userDB["Alpha"]);
			g.AddChallange(_userDB["Beta"]);
			g.AddChallange(_userDB["Gamma"]);
			_gameDB.Add(g.Id, g);

			//Jogo Owner B a desafiar o A e o H
			g = new Game(_userDB["Beta"]);
			g.AddChallange(_userDB["Alpha"]);
			g.AddChallange(_userDB["Hepta"]);
			_gameDB.Add(g.Id, g);

			//Jogo Owner G c/ desafio aceite pelo A
			g = new Game(_userDB["Gamma"]);
			g.AddChallange(_userDB["Alpha"]);
			g.ReplyOnChallange(_userDB["Alpha"], true);
			_gameDB.Add(g.Id, g);

			//Jogo c/todos, pronto para jogar entre o A, B e G (h recusou)
			g = new Game(_userDB["Alpha"]);
			g.AddChallange(_userDB["Beta"]);
			g.AddChallange(_userDB["Gamma"]);
			g.AddChallange(_userDB["Hepta"]);
			g.ReplyOnChallange(_userDB["Beta"], true);
			g.ReplyOnChallange(_userDB["Gamma"], true);
			g.ReplyOnChallange(_userDB["Hepta"], false);
			_gameDB.Add(g.Id, g);




		}


		#region Métodos de USER

		//obter utilizador
		public User GetUser(String id) {
			if(id == null) return null;
			if (!_userDB.ContainsKey(id)) return null;
			return _userDB[id];
		}

		//adicionar utilizador
		public void AddUser(User u) { _userDB.Add(u.Id, u); }

		//remover utilizador
		public void RemoveUser(User u) { _userDB.Remove(u.Id); }

		//actualizar dados de utilizador
		public void UpdateUser(User u) {
			if (!_userDB.ContainsKey(u.Id)) return;
			_userDB[u.Id] = u;
		}

		//obter enumerável de jogadores
		public IEnumerable<User> Users() { return _userDB.Values; }

		//contar utilizadores
		public int TotalUsers() { return _userDB.Count; }

		#endregion


		#region Métodos de GAME

		//obter jogo
		public Game GetGame(int id) {
			if (!_gameDB.ContainsKey(id)) return null;
			return _gameDB[id];
		}

		//adicionar jogo
		public void AddGame(Game g) { _gameDB.Add(g.Id, g); }

		//remover jogo
		public void RemoveGame(Game g) { _gameDB.Remove(g.Id); }

		//actualizar jogo (persistência)
		public void UpdateGame(Game g) {
			if (!_gameDB.ContainsKey(g.Id)) return;
			_gameDB[g.Id] = g;
		}

		//obter enumerável de jogos
		public IEnumerable<Game> Games() { return _gameDB.Values; }

		//contar jogos
		public int TotalGames() { return _gameDB.Count; }

		#endregion


		#region Métodos compostos
		
		//Jogos partilhados entre dois jogadores
		//desafios não aceites não contam  
		public void CountSharedGames(User u1, User u2) {
			
		}

		#endregion

	}
	

}
