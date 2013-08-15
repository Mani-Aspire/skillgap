using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iConnect.Presentation.Controllers
{
    public class IConnectController : Controller
	{
		protected SessionWrapper session;

		protected override void Initialize( System.Web.Routing.RequestContext requestContext )
		{
			base.Initialize( requestContext );
			session = new SessionWrapper( requestContext.HttpContext );
		}

		protected override void OnException( ExceptionContext filterContext )
		{
			filterContext.Result = new ViewResult
			{
				ViewName = "~/Views/Shared/Error.cshtml"
			};
			filterContext.ExceptionHandled = true;

			base.OnException( filterContext );
		}

		/// <summary>
		/// Overrides OnAuthorization of Mvc controller and checks the access
		/// </summary>
		/// <param name="filterContext">Filercontext</param>
		protected override void OnAuthorization( System.Web.Mvc.AuthorizationContext filterContext )
		{

			//string securityCheck = System.Configuration.ConfigurationManager.AppSettings[ "SecurityCheck" ];
			//if( string.IsNullOrEmpty( securityCheck ) )
			//{
			//    return;
			//}

			//bool isSecurityCheck = Convert.ToBoolean( securityCheck );
			//if( !isSecurityCheck )
			//{
			//    return;
			//}

			string url = filterContext.HttpContext.Request.RawUrl;

			bool hasAccess = true;

			if( !hasAccess )
			{
				throw new InvalidOperationException();
			}

			base.OnAuthorization( filterContext );
		}
	}
}
