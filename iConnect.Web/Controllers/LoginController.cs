using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iConnect.Presentation.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		public ActionResult Index( FormCollection form )
		{
			return RedirectToRoute(new System.Web.Routing.RouteValueDictionary(
				new { controller = "Home", action = "Index", id = UrlParameter.Optional }));
		}
    }
}
