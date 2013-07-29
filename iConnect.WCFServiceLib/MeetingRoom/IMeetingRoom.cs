using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using iConnect.Entities;
using System.Collections.ObjectModel;


namespace iConnect.WCFServiceLib
{
	[ServiceContract]
	public interface IMeetingRoom
	{
		[OperationContract]
		Collection<iConnect.Entities.MeetingRoomEntity> SearchMeetingRoom(string searchText);
	}
}
