using System;
using System.Collections.Generic;
using System.Text;

namespace MineSweeperFlagsLib {

	#region internal class GameEngine (Motor de jogo)

	/// <summary>
	/// Implementação do motor de Jogo.
	/// Implementação abstracta de motor de Jogo.
	/// </summary>
	public class GameEngine {

		//Estados possíveis do motor de jogo
		public enum EngineState : byte { NEW, ACTIVE, ENDED }

		//campos
		private LinkedList<IPlayer> _players;
		private LinkedListNode<IPlayer> _nextPlayer;
		private GameBoard Board { get; set; }
		public  EngineState State { get; private set; }

		//construtor que recebe enumeravel de jogadores 
		internal GameEngine(int rows, int cols, int mines, IEnumerable<IPlayer> players) {
			Board = new GameBoard(rows, cols, mines);
			_players = new LinkedList<IPlayer>();
			foreach (IPlayer p in players) {
				_players.AddLast(p);
				_players.Last.Value.Next = false;
			}
			_players.First.Value.Next = true;
			_nextPlayer = _players.First;
			State = EngineState.NEW;
		}

		//proxy:obter as células já descobertas
		public Cell[] GetUncoveredCells() { return Board.GetUncoveredCells(); }

		//proxy:obter o tabuleiro de jogo
		public Cell[] GetFullBoard() { return Board.GetFullBoard(); }

		//obter lista de jogadores
		internal IEnumerable<IPlayer> Players() { return _players; }

		//obter próximo jogador
		public IPlayer NextPlayer() { return _nextPlayer.Value; }

		//obter a pontuação de um jogador
		public int Score(IPlayer p) {
			return _players.Find(p).Value.Score; 
		}

		//proxy:obter número de minas por descobrir
		public int RemainigMines() { return Board.Mines; }

		//Executar jogada, devolve celulas destapadas
		internal Cell[] PlayAt(int row, int col) {
			IPlayer player = _nextPlayer.Value;

			//celula já destapada...
			if (!Board.CellHidden(row, col)) throw new ApplicationException("Cell already played");

			//executa jogada, comuta jogador se necessário
			Cell[] uncovered = Board.PlayAt(row, col, _nextPlayer.Value);
			if (uncovered[0].IsMine) {
				_nextPlayer.Value.Score++;
				CheckEndOfGame();
			} else {
				_nextPlayer.Value.Next = false;
				if (_nextPlayer.Next == null) _nextPlayer = _players.First;
				else _nextPlayer = _nextPlayer.Next;
				_nextPlayer.Value.Next = true;
			}
			return uncovered;
		}

		//Remover Jogador do Jogo
		internal void RemovePlayer(IPlayer player) {
			if (_players.Find(player) == null) return;
			if (player == _nextPlayer.Value) {
				if (_nextPlayer.Next == null) _nextPlayer = _players.First;
				else _nextPlayer = _nextPlayer.Next;
				_nextPlayer.Value.Next = true;
			}
			_players.Remove(player);
			if (_players.Count < 2) State = EngineState.ENDED;
		}
		
		//cancelar/invalidar o motor
		internal void Cancel() {
			State = EngineState.ENDED;
			_nextPlayer = null;
		}

		//validar terminação do jogo
		private void CheckEndOfGame() {
			int bestScore = 0, sndBestScore = 0, diff = 0;
			IPlayer bestPlayer = _players.First.Value;
			LinkedList<int> scores = new LinkedList<int>();
			foreach (IPlayer player in _players) {
				if (player.Score > bestPlayer.Score) bestPlayer = player;
				scores.AddLast(player.Score);
			}
			bestScore = bestPlayer.Score;
			scores.Remove(bestScore);
			foreach (int i in scores) if (i > sndBestScore) sndBestScore = i;
			diff = bestScore - sndBestScore;
			if (diff <= Board.Mines) return;
			State = EngineState.ENDED;
		}
	}

	#endregion

	#region private class GameBoard (Tabuleiro de jogo)

	class GameBoard {

		private Cell[,] _board;
		private Queue<Cell> _playResult;
		private int Rows   { get; set; }
		private int Cols   { get; set; }
		internal int Mines { get; set; }

		private delegate void CellAction(int row, int col);

		internal GameBoard(int rows, int cols, int mines) {
			Rows = rows;
			Cols = cols;
			Mines = mines;
			_board = new Cell[rows, cols];
			for (int i = 0; i < Rows * Cols; i++) {
				int row = i / Rows;
				int col = i % Cols;
				_board[row, col] = new Cell(i);
			}
			GenerateRandomBoard();
		}

		//verificar se uma célula está tapada
		internal bool CellHidden(int row, int col) { return _board[row, col].IsHidden; }

		//executar uma jogada
		public Cell[] PlayAt(int row, int col, IPlayer player) {
			if (!_board[row, col].IsHidden) throw new ApplicationException("Cell already played");
			_playResult = new Queue<Cell>();
			Cell c = _board[row, col];
			if (c.SurrMines == 0 && !c.IsMine) {
				ShowEmptyCells(row, col);
			} else {
				c.IsHidden = false;
				c.FoundBy = player;
				_playResult.Enqueue(c);
				if (c.IsMine) Mines--;
			}
			return _playResult.ToArray();
		}

		//obter as células de jogo já descobertas
		//o array devolvido é dimensão do tabuleiro
		public Cell[] GetUncoveredCells() {
			int i = 0;
			Cell[] retCells = new Cell[Rows * Cols];
			foreach (Cell cell in _board) if (!cell.IsHidden) retCells[i++] = cell;
			return retCells;
		}

		//obter o tabuleiro de jogo
		internal Cell[] GetFullBoard() {
			int i = 0;
			Cell[] retCells = new Cell[Rows * Cols];
			foreach (Cell cell in _board) retCells[i++] = cell;
			return retCells;
		}

		//Geração aleatória do tabuleiro
		private void GenerateRandomBoard() {
			int mines = Mines;
			Random random = new Random();
			while (mines > 0) {
				int row = random.Next(Rows);
				int col = random.Next(Cols);
				if (_board[row, col].IsMine) continue;
				_board[row, col].IsMine = true;
				SetSurroundingCells(row, col, delegate(int r, int c) { _board[r, c].SurrMines++; });
				mines--;
			}
		}
		
		//descobrir células adjacentes a uma mina
		private void SetSurroundingCells(int row, int col, CellAction action) {
			int rMin = (row - 1) > 0 ? row - 1 : 0,
				rMax = (row + 1) < Rows - 1 ? row + 1 : Rows - 1,
				cMin = (col - 1) > 0 ? col - 1 : 0,
				cMax = (col + 1) < Cols - 1 ? col + 1 : Cols - 1;
			for (int r = rMin; r <= rMax; r++)
				for (int c = cMin; c <= cMax; c++)
					if (!(r == row && c == col)) action(r, c);
		}

		//método recursivo para destapar células vazias
		private void ShowEmptyCells(int row, int col) {
			if (!_board[row, col].IsHidden) return;
			Cell c = _board[row, col];
			_playResult.Enqueue(c);
			c.IsHidden = false;
			if (c.SurrMines == 0 && !c.IsMine) SetSurroundingCells(row, col, ShowEmptyCells);
		}
	}

	#endregion

	#region public class Cell (Celula de Jogo)

	/// <summary>
	/// Implementação de uma célula de jogo
	/// </summary>
	public class Cell {
		public int     SurrMines { get; internal set; }
		public bool    IsHidden  { get; internal set; }
		public bool    IsMine    { get; internal set; }
		public int     Index     { get; internal set; }
		public IPlayer FoundBy   { get; internal set; }

		public Cell(int index) {
			IsMine    = false;
			IsHidden  = true;
			SurrMines = 0;
			Index     = index;
			FoundBy   = null;
		}

		public override string ToString() {
			StringBuilder res = new StringBuilder();
			res.Append("{index:");
			res.Append(Index);
			res.Append(",hidden:");
			res.Append(IsHidden.ToString());
			res.Append(",mine:");
			res.Append(IsMine.ToString());
			res.Append(",nearby:");
			res.Append(SurrMines.ToString());
			res.Append("}");
			return res.ToString();
		}
	}

	#endregion

	#region interface IPlayer (Jogador)
	
	public interface IPlayer : IEquatable<IPlayer> {
		String  Id    { get; }
		int     Score { get; set; }
		bool    Next  { get; set; }
	}
	
	#endregion

}

