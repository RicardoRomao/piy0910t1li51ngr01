using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MineSweeperFlags.Models;
using MineSweeperFlagsLib;

namespace MineSweeperFlags.Controllers {

	[HandleError]
	public class GameController : MSFBaseController {

		//O Construtor recebe o repositório
		public GameController(IMSFRepository repository) : base(repository) {}


		//aux:executar validação e obtenção do id de jogo
		private void ValidateGame(GameData game) {
			if(!game.GameId.HasValue) ModelState.AddModelError("GameId", "null or empty gameid");
			else if (game.GameId.Value < 1) ModelState.AddModelError("GameId", "invalid gameid");
			else {
				game.SetGame(m_Repository.GetGame(game.GameId.Value));
				if (game.Game() == null) ModelState.AddModelError("GameId", "unknown gameid");
			}
		}



		//------------------
		//DETALHE DE UM JOGO
		//------------------
		//Output: HTML (parcial)
		//
		public ActionResult GameDetail(GameData args) {
			ValidateUser(args);
			if (ModelState.IsValid) ValidateGame(args);
			args.SetError(!ModelState.IsValid);
			return View("GameDetail", args);
		}



		//-----------------------------------
		//CRIAR NOVO JOGO (inclui um desafio)
		//-----------------------------------
		//Output: HTML (parcial)
		//
		public ActionResult NewGame(GameData args) {
			ValidateUser(args);
			if (ModelState.IsValid) {
				User challanged = null;
				if(!String.IsNullOrEmpty(args.RefUser)) {
					challanged = m_Repository.GetUser(args.RefUser);
					if (challanged == null) ModelState.AddModelError("UserId", ("unknown userid " + args.RefUser));
				}
				if (ModelState.IsValid) {
					try {
						Game newGame = new Game(args.User());
						if (challanged != null) newGame.AddChallange(challanged);
						m_Repository.AddGame(newGame);
						args.SetGame(newGame);
					} catch (ApplicationException e) { ModelState.AddModelError("GameId", e.Message); }
				}
			}
			args.SetError(!ModelState.IsValid);
			return View("GameDetail", args);
		}



		//-------------------------
		//DESAFIAR OUTRSO JOGADORES
		//-------------------------
		//Output: HTML (parcial)
		//
		public ActionResult InviteOthers(GameData args) {
			ValidateUser(args);
			if (ModelState.IsValid) ValidateGame(args);
			if (ModelState.IsValid) {
				args.SetCandidates(
					 m_Repository.Users().Where(u => !args.Game().ContainsUser(u))
				);
			}
			args.SetError(!ModelState.IsValid);
			return View("GameDetail", args);
		}



		//-----------------
		//ADICIONAR CONVITE
		//-----------------
		//Output: HTML (parcial)
		//
		public ActionResult AddChallange(GameData args) {
			ValidateUser(args);
			if (ModelState.IsValid) ValidateGame(args);
			if (ModelState.IsValid) {
				User challanged = null;
				if (String.IsNullOrEmpty(args.RefUser)) ModelState.AddModelError("UserId", "missing challanged userid.");
				else {
					challanged = m_Repository.GetUser(args.RefUser);
					if (challanged == null) ModelState.AddModelError("UserId", ("unknown userid " + args.RefUser));
				}
				if (ModelState.IsValid) {
					try { args.Game().AddChallange(challanged); }
					catch (ApplicationException e) { ModelState.AddModelError("GameId", e.Message); }
				}
			}
			args.SetError(!ModelState.IsValid);
			return View("GameDetail", args);
		}



		//---------------
		//ACEITAR CONVITE
		//---------------
		//Output: HTML (parcial)
		//
		public ActionResult AcceptChallange(GameData args) {
			ValidateUser(args);
			if(ModelState.IsValid) ValidateGame(args);
			if(ModelState.IsValid) {
				try { args.Game().ReplyOnChallange(args.User(),true); }
				catch (Exception e) { ModelState.AddModelError("GameId", e.Message); }
			}
			args.SetError(!ModelState.IsValid);
			return View("GameDetail", args);
		}


		//----------------
		//REJEITAR CONVITE
		//----------------
		//Output: HTML (parcial)
		//
		public ActionResult RefuseChallange(GameData args) {
			ValidateUser(args);
			if (ModelState.IsValid) ValidateGame(args);
			if (ModelState.IsValid) {
				try { args.Game().ReplyOnChallange(args.User(), false); } catch (ApplicationException e) { ModelState.AddModelError("GameId", e.Message); }
			}
			args.SetError(!ModelState.IsValid);
			return View("GameDetail", args);
		}


		//------------
		//INICIAR JOGO
		//------------
		//Output: Redirect
		//
		public ActionResult Start(GameData args) {
			ValidateUser(args);
			if (ModelState.IsValid) ValidateGame(args);
			if (ModelState.IsValid) {
				try { args.Game().Start(args.User()); } catch (ApplicationException e) { ModelState.AddModelError("GameId", e.Message); }
			}
			args.SetError(!ModelState.IsValid);
			return RedirectToAction("Continue", args);
		}


		//--------------
		//CONTINUAR JOGO
		//--------------
		//Output: HTML (parcial)
		//
		public ActionResult Continue(GameData args) {
			ValidateUser(args);
			if (ModelState.IsValid) ValidateGame(args);
			if (ModelState.IsValid && args.Game().State != Game.GameState.ACTIVE)
				ModelState.AddModelError("GameId", "Game is not active...");
			if(ModelState.IsValid) {
				OnlineUsers.UserGames(args.User()).AddGame(args.Game());
			}
			args.SetError(!ModelState.IsValid);
			return View("GameConsole", args);
		}



		//--------------
		//ABANDONAR JOGO
		//--------------
		//Output: HTML (parcial)
		//
		public ActionResult Abandon(GameData args) {
			ValidateUser(args);
			if (ModelState.IsValid) ValidateGame(args);
			if (ModelState.IsValid) {
				try { args.Game().RemoveUser(args.User()); }
				catch (ApplicationException e) { ModelState.AddModelError("GameId", e.Message); }
			}
			args.SetError(!ModelState.IsValid);
			return View("GameDetail", args);
		}


		//---------------
		//EXECUTAR JOGADA
		//---------------
		//Output: JSON
		//
		public ActionResult Play(GameData args) {
			String json;
			Cell[] playRes = null; 
			ValidateUser(args);
			if (ModelState.IsValid) ValidateGame(args);
			if (ModelState.IsValid) {
				if (args.PlayAt.HasValue) {
					int row = args.PlayAt.Value / Game.GAME_COLS;
					int col = args.PlayAt.Value % Game.GAME_COLS;
					try {
						playRes = args.Game().play(args.User(), row, col);
						OnlineUsers.AddCellsToGamePools(args.Game(), playRes, args.User());
					} catch (ApplicationException e) {
						ModelState.AddModelError("GameId", e.Message); 
					}
				} else ModelState.AddModelError("GameId", "missing cell index to play."); 
			}
			if (ModelState.IsValid) {
				Cell[] changedCells = OnlineUsers.UserGames(args.User()).ChangedCells(args.Game());
				json = UpdateAsJSON("play", args.Game(), playRes, changedCells, null);
			} else json = UpdateAsJSON("play", args.Game(), null, null, ModelStateErrorsAsJSON());
			return new ContentResult() { Content = json.ToString() };
		}


		//--------------------
		//ACTUALIZAÇÃO DE JOGO
		//--------------------
		//Output: JSON
		//
		public ActionResult GetUpdate(GameData args) {
			String json;
			ValidateUser(args);
			if (ModelState.IsValid) ValidateGame(args);
			if (ModelState.IsValid) {
				Cell[] changedCells = OnlineUsers.UserGames(args.User()).ChangedCells(args.Game());
				json = UpdateAsJSON("update", args.Game(), null, changedCells, null);
			} else json = UpdateAsJSON("update", args.Game(), null, null, ModelStateErrorsAsJSON());
			return new ContentResult() { Content = json.ToString() };			
		}



		//AUXILIAR:
		//Serializar update uo resultado de jogada em formato JSON
		private String UpdateAsJSON(String replyTo, Game g, Cell[] playedCells, Cell[] changedCells, String error) {

			StringBuilder res = new StringBuilder(128);

			res.Append("{reply:\"");
			res.Append(replyTo);
			res.Append("\"");
			
			//se tem erro, preenche e sai
			//nota: erro tem que vir já formatado em json
			if (!String.IsNullOrEmpty(error)) {
				res.Append(",invalid:");
				res.Append(error);
				res.Append("}");
				return res.ToString();
			}

			//jogadores registados (todos)
			bool first = true;
			res.Append(",players:[");
			foreach (User u in g.Users()) {
				if (first) first = false;
				else res.Append(",");
				res.Append("{");
				res.Append(u.ToString());
				res.Append(",score:");
				res.Append(g.Engine().Score(new Player(u)));
				if(u == g.Engine().NextPlayer()) res.Append(",next:true");
				res.Append("}");
			}
			res.Append("]");

			//jogo...
			res.Append(",game:{");

			//estado do jogo
			res.Append("status:\"");
			res.Append(g.Engine().State.ToString());
			res.Append("\"");

			//minas que falta destapar
			res.Append(",mines:");
			res.Append(g.Engine().RemainigMines());

			//próximo jogador
			res.Append(",nextplayer:\"");
			res.Append(g.Engine().NextPlayer().Id);
			res.Append("\"");

			//células modificadas
			if (playedCells != null || changedCells != null) {
				first = true;
				res.Append(",cells:[");
				if(playedCells != null) {
					foreach (Cell c in playedCells) {
						if (c != null) {
							if (first) first = false;
							else res.Append(",");
							res.Append("{index:");
							res.Append(c.Index);
							res.Append(",content:");
							res.Append(c.IsHidden ? 0 : (c.IsMine ? 9 : c.SurrMines));
							if(c.FoundBy != null) {
								res.Append(",foundBy:\"");
								res.Append(c.FoundBy.Id);
								res.Append("\"");
							}
							res.Append("}");
						}
					}
				}
				if(changedCells != null) {
					foreach (Cell c in changedCells) {
						if (c != null) {
							if (first) first = false;
							else res.Append(",");
							res.Append("{index:");
							res.Append(c.Index);
							res.Append(",content:");
							res.Append(c.IsHidden ? 0 : (c.IsMine ? 9 : c.SurrMines));
							if (c.FoundBy != null) {
								res.Append(",foundBy:\"");
								res.Append(c.FoundBy.Id);
								res.Append("\"");
							}
							res.Append("}");
						}
					}
				}
				res.Append("]");
			}

			//end of game data
			res.Append("}");

			//end of object
			res.Append("}");
			return res.ToString();
		}
    }
}
