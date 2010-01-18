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
	public class UserController : MSFBaseController {

		//O Construtor recebe o repositório
		public UserController(IMSFRepository repository) : base(repository) {}


		//------
		//LOGOFF
		//------
		//Output: Redirect to Index
		//
		public ActionResult Logoff(UserRef user) {
			if (!String.IsNullOrEmpty(user.UserId))
				OnlineUsers.Offline(m_Repository.GetUser(user.UserId));
			return RedirectToAction("/../Home/Index", new UserRef());
		}


		//-------------------------
		//REGISTO (NOVO UTILIZADOR)
		//-------------------------
		//Output: HTML (página)
		//
		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult Registration(String UserId) {
			if (String.IsNullOrEmpty(UserId))
				return View("UserRegistration", new UserRegistration());
			else {
				User user = m_Repository.GetUser(UserId);
				if (user == null) {
					ModelState.AddModelError("UserId", "unknown userid");
					return RedirectToAction("/../Home/Index");
				}
				UserRegistration data = new UserRegistration 
				{ Id = user.Id, Name = user.Name, Email = user.Email, Avatar = user.Avatar, EmailConf = user.Email };
				data.IsUpdate = true;
				return View("UserRegistration", data);
			}
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Registration(UserRegistration form) {
			if (ModelState.IsValid) {
				User user = new User { Id = form.Id, Name = form.Name, Email = form.Email };
				if (form.IsUpdate.HasValue && form.IsUpdate.Value == true)
					m_Repository.UpdateUser(user);
				else {
					if (m_Repository.GetUser(form.Id) != null)
						ModelState.AddModelError("Id", "UserId " + form.Id + " already in use.");
					else
						m_Repository.AddUser(user);
				}
			}
			form.SetError(!ModelState.IsValid);
			return View("UserRegistration", form);
		}


		//---------------------
		//DETALHE DE UTILIZADOR
		//---------------------
		//Output: HTML (parcial)
		//
		public ActionResult UserDetail(UserDetail args) {
			if (String.IsNullOrEmpty(args.UserId)) ModelState.AddModelError("GameId", "null or empty userid");
			if (String.IsNullOrEmpty(args.DetailId)) ModelState.AddModelError("GameId", "null or empty detailid");
			if(args.UserId == args.DetailId) return Redirect("Registration");
			args.SetUser(ResolveAvatar(m_Repository.GetUser(args.UserId)));
			args.SetDetailUser(ResolveAvatar(m_Repository.GetUser(args.DetailId)));
			args.SetGameList(
				m_Repository.Games().Where(
					g => (g.ContainsUser(args.DetailUser()) && g.ContainsUser(args.User()))
				)
			);
			return View("UserDetail", args);
		}




    }
}
