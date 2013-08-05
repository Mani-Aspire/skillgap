using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace iConnect.Presentation.Controllers
{
    public class HomeController : Controller
    {
		//
        // GET: /Home/

        public ActionResult Index()
        {
            try
            {
                MeetingRoomService.MeetingRoomClient meetingRommClient = new MeetingRoomService.MeetingRoomClient();
                iConnect.Presentation.MeetingRoomService.MeetingRoomEntity[] rooms = meetingRommClient.SearchMeetingRoom("capella");
                this.ViewData["MeetingRoomDetails"] = rooms;
            }
            catch (FaultException ex)
            {
                string error = ex.Message;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return View();
		}
    }
}
