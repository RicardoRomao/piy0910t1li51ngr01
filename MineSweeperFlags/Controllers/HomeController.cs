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
	public class HomeController : MSFBaseController {

		//O Construtor recebe o repositório
		public HomeController(IMSFRepository repository) : base(repository) {}


		//-----
		//LOGIN
		//-----

		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult Index() { return View("Index", new UserRef()); }

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Index(UserRef user) {
			ValidateUser(user);
			if (ModelState.IsValid) {
				User u = user.User();
				OnlineUsers.Hit(u);
				ResolveAvatar(u);
			}
			user.SetError(!ModelState.IsValid);
			return View("Index", user);
		}



		//-------------------------------------
		//ACTUALIZAÇÃO DA LISTA DE UTILIZADORES
		//-------------------------------------
		//Devolve objecto JSON
		//
		public ActionResult UpdateUsers(UserRef args) {
			ValidateUser(args);
			if (!ModelState.IsValid) return new ContentResult() { Content = MSFControllerConst.JSON_NO_DATA };
			int sharedGames;
			bool first = true;
			StringBuilder res = new StringBuilder();
			res.Append("[");
			foreach (User u in m_Repository.Users().Where(u => u.Id != args.UserId)) {
				if (first) first = false;
				else res.Append(",");
				res.Append("{");
				res.Append(u.ToString());
				res.Append(",icon:\"");
				if (OnlineUsers.Online(u)) {
					res.Append(Url.Content("~/Images/userStatusOn.png\""));
					res.Append(",online:true");
				} else {
					res.Append(Url.Content("~/Images/userStatusOff.png\""));
				}
				sharedGames = 0;
				foreach (Game g in m_Repository.Games()
					.Where(g => (g.ContainsUser(u) && g.ContainsUser(args.User())) ))
						++sharedGames;
				if (sharedGames > 0) {
					res.Append(",games:");
					res.Append(sharedGames);
				}
				res.Append("}");
			}
			res.Append("]");
			return new ContentResult() { Content = res.ToString() };
		}




		//--------------------------------------------
		//ACTUALIZAÇÃO DA LISTA DE JOGOS DE UM JOGADOR
		//--------------------------------------------
		//Devolve objecto JSON
		//
		public ActionResult UpdateGames(UserRef args) {
			ValidateUser(args);
			if (!ModelState.IsValid) return new ContentResult() { Content = MSFControllerConst.JSON_NO_DATA };
			bool first = true;
			User u = args.User();
			StringBuilder res = new StringBuilder();
			res.Append("[");
			foreach (Game g in m_Repository.Games().Where(g => g.ContainsUser(u))) {
				String whosNext = null;
				if (first) first = false;
				else res.Append(",");
				res.Append("{id:");
				res.Append(g.Id);
				res.Append(",owner:\"");
				res.Append(g.Owner().Id);
				res.Append("\",state:\"");
				res.Append(g.State.ToString());
				res.Append("\",icon:\"");
				switch(g.State) {
					case Game.GameState.CHALLANGE:
						if (g.Challange(u) == Game.ChallangeState.NEW)
							res.Append(Url.Content("~/Images/gameState_CHALLANGE.png"));
						else if (g.Challange(u) == Game.ChallangeState.ACCEPTED)
							res.Append(Url.Content("~/Images/gameState_WAIT.png"));
						else
							res.Append(Url.Content("~/Images/gameState_REFUSED.png"));
						break;
						
					case Game.GameState.ACTIVE:
						whosNext = g.Engine().NextPlayer().Id;
						if (g.Engine().NextPlayer().Equals(u))
							res.Append(Url.Content("~/Images/gameState_ACTIVEYOU.png"));
						else
							res.Append(Url.Content("~/Images/gameState_ACTIVE.png"));
						break;
						
					case Game.GameState.ENDED:
						res.Append(Url.Content("~/Images/gameState_ENDED.png"));
						break;
				}
				res.Append("\"");
				if (whosNext != null) {
					res.Append(",next:\"");
					res.Append(whosNext);
					res.Append("\"");
					if (whosNext == u.Id) res.Append(",you_next:true");
				}
				if (g.Challange(u) == Game.ChallangeState.NEW) res.Append(",you_ans:true");
				if (g.Owner().Equals(u)) res.Append(",you_own:true");
				res.Append("}");
			}
			res.Append("]");
			return new ContentResult() { Content = res.ToString() };
		}



	}
}
