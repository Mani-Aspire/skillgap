using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace iConnect.WCFServiceLib
{
	public class LoginService : ILogin
	{
		public bool Login()
		{

			DataAccess.MeetingRoomDataAccess access = new DataAccess.MeetingRoomDataAccess();
			access.SearchMeetingRoom( "room", 1, 2 );
			return true;
		}
	}
}