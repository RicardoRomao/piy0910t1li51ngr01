using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MineSweeperFlags.Controllers;
using MineSweeperFlagsLib;

namespace MineSweeperFlags {

	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication {

		public static void RegisterRoutes(RouteCollection routes) {
		
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			
			//registar route handlers
			routes.MapRoute( null, "{controller}/{action}", new { controller = "Home", action = "Index" } );

		}

		//STARTUP
		protected void Application_Start() {

			//regista a controller factory a usar
			ControllerBuilder.Current.SetControllerFactory(new MSFControllerFactory());

			//regista routers
			RegisterRoutes(RouteTable.Routes);
			
		}
	}
}