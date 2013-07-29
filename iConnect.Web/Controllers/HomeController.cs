using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.ObjectModel;

namespace iConnect.Presentation.Controllers
{
    public class HomeController : Controller
    {
		//
        // GET: /Home/

        public ActionResult Index()
        {
			MeetingRoomService.MeetingRoomClient meetingRommClient = new MeetingRoomService.MeetingRoomClient();
			iConnect.Presentation.MeetingRoomService.MeetingRoomEntity[] rooms = meetingRommClient.SearchMeetingRoom( "capella" );
			this.ViewData[ "MeetingRoomDetails" ] = rooms;
			return View();
		}
    }
}
