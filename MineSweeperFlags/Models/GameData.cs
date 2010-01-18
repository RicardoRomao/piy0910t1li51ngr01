using System;
using System.Collections.Generic;
using System.Web;
using System.ComponentModel;
using MineSweeperFlagsLib;

namespace MineSweeperFlags.Models {

	/// <summary>
	/// Referência para um Jogo
	/// </summary>
	public class GameData : UserRef {

		//propriedades
		public int?   GameId  { get; set; }
		public String RefUser { get; set; }
		public int?   PlayAt  { get; set; }

		//o jogo do modelo (se existe)
		//expõe-se em forma de métodos para não ser tratado no Model-Binder
		private Game m_game;
		public Game Game() { return m_game; }
		public void SetGame(Game game) { m_game = game; }

		//uma lista de potenciais jogadores (a convidar)
		//expõe-se em forma de métodos para não ser tratado no Model-Binder
		private IEnumerable<User> m_candidates;
		public IEnumerable<User> Candidates() { return m_candidates; }
		public void SetCandidates(IEnumerable<User> candidates) { m_candidates = candidates; }

	}
}
