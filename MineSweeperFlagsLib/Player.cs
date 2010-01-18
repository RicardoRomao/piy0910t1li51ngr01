using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineSweeperFlagsLib {

	/// <summary>
	/// Junção de utilizador e Jogador
	/// </summary>
	public class Player : User, IPlayer {

		//contrutor a partir de User
		public Player(User user) : base(user) { }

		//implementação de IPlayer
		public int Score { get; set; }
		public bool Next { get; set; }
		public bool Equals(IPlayer other) { return (Id == other.Id); }

	}
}
