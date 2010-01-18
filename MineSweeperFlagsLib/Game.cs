using System;
using System.Collections.Generic;
using System.Text;

namespace MineSweeperFlagsLib {

	/// <summary>
	/// Implementação de jogo.
	/// O jogo é composto por um motor de jogo
	/// e um conjunto de jogadores.
	/// </summary>
	public class Game {
	
		//controlador de unicidade de id de jogo
		private static int gameId = 0;

		//estado do desafio de jogadores
		//NEW    - Desafiado, sem resposta
		//ACCEPT - Desafio aceite
		//DENY   - Desafio negado
		public enum ChallangeState : byte { NEW, ACCEPTED, REFUSED }

		//estados do jogo
		//CHALLANGE - Em preparação (escolher jogadores)
		//ACTIVE    - Jogo activo
		//ENDED     - Jogo terminado
		public enum GameState : byte { CHALLANGE, ACTIVE, ENDED }


		//retirar daqui se for para dinamizar um dia...
		public static readonly int GAME_ROWS = 16;
		public static readonly int GAME_COLS = 16;
		public static readonly int GAME_MINES = 51;


		//campos
		private User _owner;
		private Dictionary<User, ChallangeState> _players;
		private GameEngine _engine;
		
		
		//propriedades
		public int Id          { get; private set; }
		public GameState State { get; private set; }


		//Construir novo Jogo
		public Game(User owner) {
			_owner = owner;
			_players = new Dictionary<User, ChallangeState>(10);
			_players.Add(owner, ChallangeState.ACCEPTED);
			State = GameState.CHALLANGE;
			
			//Este método não é seguro...
			Id = ++Game.gameId;
		}
		
		//para saber quem lançou o desafio do jogo
		public User Owner() { return _owner; }

		//obter referência para os jogadores
		public IEnumerable<User> Users() { return _players.Keys; }

		//desafiar um utilizador
		public void AddChallange(User user) {
			if (State != GameState.CHALLANGE) throw new ApplicationException("Invalid state.");
			if (!_players.ContainsKey(user)) _players.Add(user, ChallangeState.NEW);
		}

		//aceitar um desafio
		public void ReplyOnChallange(User user, bool answer) {
			if (State != GameState.CHALLANGE) throw new ApplicationException("Invalid state.");
			if (_players.ContainsKey(user)) _players[user] = (answer ? ChallangeState.ACCEPTED : ChallangeState.REFUSED);
		}

		//verificar a resposta ao desafio
		public ChallangeState Challange(User user) {
			if (user == null) throw new ApplicationException("User cannot be null.");
			if (!_players.ContainsKey(user)) throw new ApplicationException("User not challanged.");
			return _players[user];
		}

		//remover um jogador
		public void RemoveUser(User user) {
			if (State == GameState.ENDED) throw new ApplicationException("Invalid state.");
			if (State == GameState.CHALLANGE && user.Equals(_owner)) throw new ApplicationException("Cannot remove challanger.");
			if (_players.ContainsKey(user)) {
				_players.Remove(user);
				if (State == GameState.ACTIVE) {
					_engine.RemovePlayer(new Player(user));
					if (_engine.State == GameEngine.EngineState.ENDED) State = GameState.ENDED;
				}
			}
		}

		//verificar se um utilizador está registado no jogo
		public bool ContainsUser(User user) {
			if (State == GameState.ENDED) return false;
			return _players.ContainsKey(user);
		}

		//verificar se jogo pronto (pelo menos 2 jogadores aceitaram)
		public bool Ready() {
			if(State == GameState.ENDED) return false;
			if(State == GameState.ACTIVE) return true;
			int ac = 0;
			foreach(ChallangeState s in _players.Values) {
				if(s == ChallangeState.ACCEPTED) ++ac;
				if(ac > 1) return true;
			}
			return false;
		}
		
		//iniciar o novo Jogo
		public void Start(User starter) {
			if (State != GameState.CHALLANGE) throw new ApplicationException("Invalid state.");
			if (starter != _owner) throw new ApplicationException("Game must be started by owner.");
			if (!Ready()) throw new ApplicationException("Not enough players to start the game.");
			LinkedList<IPlayer> group = new LinkedList<IPlayer>();
			Dictionary<User, ChallangeState> players = new Dictionary<User, ChallangeState>(_players.Count);
			lock(_players) {
				Dictionary<User, ChallangeState>.Enumerator e = _players.GetEnumerator();
				while(e.MoveNext()) {
					if(e.Current.Value == ChallangeState.ACCEPTED) {
						group.AddLast(new Player(e.Current.Key));
						players.Add(e.Current.Key, e.Current.Value);
					}
				}
			}
			_players = players;
			_engine = new GameEngine(GAME_ROWS, GAME_COLS, GAME_MINES, group);
			State = GameState.ACTIVE;
		}

		//cancelar um jogo
		public void Cancel() {
			if (State == GameState.ENDED) throw new ApplicationException("Invalid state.");
			if(_engine != null) _engine.Cancel();
			State = GameState.ENDED;
		}

		//executar uma jogada (método "proxy")
		//valida próximo jogador
		//actualiza estado do jogo
		public Cell[] play(User user, int row, int col) {
			if (State != GameState.ACTIVE) return null;
			if(user.Id != _engine.NextPlayer().Id) throw new ApplicationException("User is not next..."); 
			Cell[] res;
			try { res = _engine.PlayAt(row, col); }
			catch (ApplicationException e) { throw e; } 
			if(_engine.State == GameEngine.EngineState.ENDED) State = GameState.ENDED;
			return res;
		}

		//obter referência para o motor de jogo
		//delega no motor de jogo os metodos de inquérito
		public GameEngine Engine() { return _engine; }

	}

}
