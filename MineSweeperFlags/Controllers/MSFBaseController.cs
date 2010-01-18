using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MineSweeperFlags.Models;
using MineSweeperFlagsLib;


namespace MineSweeperFlags.Controllers {


	/// <summary>
	/// Comportamento genérico
	/// </summary>
    public class MSFBaseController : Controller {


		#region Online User Pool

		/// <summary>
		/// Utilizador Online
		/// </summary>
		internal class OnlinePool : User {
			
			//Pool de células modificadas por jogo
			//(permite jogar mais do que um jogo em clientes distintos)
			private Dictionary<Game, LinkedList<Cell>> _activeGames;
			
			//construtor
			internal OnlinePool(User user) : base(user) {
				_activeGames = new Dictionary<Game, LinkedList<Cell>>(3);
			}
			
			//verificar se existe jogo
			internal bool ContainsGame(Game g) { return _activeGames.ContainsKey(g); }
			
			//adicionar jogo
			internal void AddGame(Game g) {
				if(g == null) return;
				lock(_activeGames) {
					if(!ContainsGame(g)) _activeGames.Add(g, new LinkedList<Cell>());
					else _activeGames[g] = new LinkedList<Cell>();
					if(g.Engine() != null) {
						foreach (Cell c in g.Engine().GetUncoveredCells()) {
							_activeGames[g].AddLast(c);
						}
					}
				}
			}

			//adicionar celulas alteradas
			internal void AddChanges(Game g, Cell[] changes) {
				if(g == null) return;
				lock(_activeGames) {
					if(!ContainsGame(g)) return;
					foreach (Cell c in changes) {
						_activeGames[g].AddLast(c);
					}
				}
			}

			//obter celulas modificadas e remover da lista
			internal Cell[] ChangedCells(Game g) {
				Cell[] cc = new Cell[0];
				if (g == null) return cc;
				lock (_activeGames) {
					if (ContainsGame(g)) {
						cc = _activeGames[g].ToArray();
						_activeGames[g].Clear();
					}
				}
				return cc;
			}
		
		}

		/// <summary>
		/// Pool de utilizadores online
		/// </summary>
		internal class OnlineUserPool {

			//pool de utilizadores
			private readonly Dictionary<User, DateTime> _onlineUsers;

			//construtor
			internal OnlineUserPool() {
				_onlineUsers = new Dictionary<User, DateTime>(100);
			}

			//Referência para a pool de Users
			internal Dictionary<User, DateTime> UserPool() { return _onlineUsers; }

			//Referência para a pool de Jogos de um User
			internal OnlinePool UserGames(User u) {
				if (u == null) return null;
				foreach (OnlinePool ou in _onlineUsers.Keys) {
					if (ou.Equals(u))
						return ou;
				}
				return null;
			}
			
			//verificar se um utilizador está Online
			internal bool Online(User u) {
				if (u == null) return false;
				return _onlineUsers.ContainsKey(u);
			}

			//remover uilizador da lista Online
			internal void Offline(User u) {
				if (u == null) return;
				_onlineUsers.Remove(u);
			}

			//actualizar um "hit" (se o utilizador nã existe, adiciona).
			internal void Hit(User u) {
				if (u == null) return;
				if (_onlineUsers.ContainsKey(u)) _onlineUsers[u] = DateTime.Now;
				else _onlineUsers.Add(new OnlinePool(u), DateTime.Now);
			}

			//obter ultima actualização
			internal DateTime LastHit(User u) {
				if (!Online(u)) return new DateTime();
				return _onlineUsers[u];
			}

			
			//Actualizar buufer de células alteradas 
			//dos jogadores que possuem o jogo online
			internal void AddCellsToGamePools(Game g, Cell[] cells, User notForUser) {
				foreach (OnlinePool op in _onlineUsers.Keys.Where(u => u != notForUser)) {
					if (op.ContainsGame(g)) op.AddChanges(g, cells);
				}
			}

		}

		//referências
		private static readonly int LOGIN_TIMEOUT_MS = 300000;	//5 minutos
		private static readonly int LOGIN_CHECKTO_MS = 30000;	//30 segundos

		//pool de jogadores
		internal static readonly OnlineUserPool OnlineUsers;

		//THREAD:
		//limpeza de users online sem hits
		private static void ClearOnlines() {
			DateTime refTS;
			TimeSpan difTS;
			while (true) {
				try {
					if (OnlineUsers != null) {
						lock (OnlineUsers) {
							refTS = DateTime.Now;
							foreach (KeyValuePair<User, DateTime> ou in OnlineUsers.UserPool()) {
								difTS = refTS.Subtract(ou.Value);
								if (difTS.TotalMilliseconds > LOGIN_TIMEOUT_MS)
									OnlineUsers.Offline(ou.Key);
							}
						}
					}
				} catch (Exception e) {
					Console.WriteLine(e.StackTrace);
				} finally {
					Thread.Sleep(LOGIN_CHECKTO_MS);
				}
			}
		}
		
		
		//O contrutor estático incia
		//thread de limpeza de jogadores
		static MSFBaseController() {
			OnlineUsers = new OnlineUserPool();
			Thread otimeout = new Thread(ClearOnlines);
			otimeout.Start();
		}

		#endregion


		//o repositório de dados
		protected IMSFRepository m_Repository;


		//O Construtor recebe o repositório
		//Só pode ser invocado por extensões da classe.
		protected MSFBaseController(IMSFRepository repository) { m_Repository = repository; }


		//aux:executar validação do utilizador
		//a substituir pelo "session id" no futuro
		protected void ValidateUser(UserRef user) {
			if (String.IsNullOrEmpty(user.UserId)) ModelState.AddModelError("UserId", "null or empty userid");
			user.SetUser(m_Repository.GetUser(user.UserId));
			if (user.User() == null) ModelState.AddModelError("UserId", "unknown userid");
			if(ModelState.IsValid) OnlineUsers.Hit(user.User());
		}


		//aux: resolver localização dos avatars no user
		//note-se que este campo não pode ser gravado...
		protected User ResolveAvatar(User u) {
			if (u.Avatar == null) u.Avatar = MSFControllerConst.AVATAR_DEFAULT;
			if (!u.Avatar.StartsWith("/")) u.Avatar = Url.Content(MSFControllerConst.AVATAR_REL_LOCATION + u.Avatar);
			return u;
		}


		//aux: produzir um atraso aleatório p/testes
		//de estabilidade do cliente...
		protected void randomDelay() {
			DateTime dt = DateTime.Now;
			Random random = new Random(dt.Millisecond * dt.Second);
			Thread.Sleep(random.Next(10)*1000); 
		}


		//aux: formatar os erros do modelo em JSON
		//a partir do ModelState da instância....
		protected String ModelStateErrorsAsJSON() {
			StringBuilder errors = new StringBuilder();
			bool first = true;
			errors.Append("[");
			foreach (ModelState ms in ModelState.Values) {
				foreach(ModelError me in ms.Errors) {
					if (first) first = false;
					else errors.Append(",");
					errors.Append("\"");
					errors.Append(me.ErrorMessage.Replace("\"","\\\""));
					errors.Append("\"");
				}
			}
			errors.Append("]");
			return errors.ToString();
		}

    }
}
