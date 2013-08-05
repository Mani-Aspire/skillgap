using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections.ObjectModel;
using iConnect.Service.Extensions;

namespace iConnect.WCFServiceLib
{
    [ErrorHandlingBehavior]
	public class MeetingRoomService : IMeetingRoom
	{
		public Collection<iConnect.Entities.MeetingRoomEntity> SearchMeetingRoom( string searchText )
		{
			iConnect.DataAccess.MeetingRoomDataAccess m = new iConnect.DataAccess.MeetingRoomDataAccess();
			Collection<iConnect.Entities.MeetingRoomEntity> obj = m.SearchMeetingRoom( searchText , 1, 1 );
			return obj;
		}
	}
}