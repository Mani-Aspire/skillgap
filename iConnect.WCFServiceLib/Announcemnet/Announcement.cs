using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iConnect.WCFServiceLib
{
	public class Announcement : IAnnouncement
	{
		public Entities.AnnouncementList GetAnnouncements( int pageSize, int pageIndex )
		{
			throw new NotImplementedException();
		}

		public string GetSample()
		{
			throw new NotImplementedException();
		}

		public string CreateAnnouncement( Entities.AnnouncementEntity announcementEntity )
		{
			throw new NotImplementedException();
		}

		public Entities.Node GetDocumentByPath( string documentPath )
		{
			throw new NotImplementedException();
		}

		public bool CreateDocumentOrFolder( Entities.Node node )
		{
			throw new NotImplementedException();
		}

		public bool DeleteDocumentOrFolder( Entities.Node node )
		{
			throw new NotImplementedException();
		}

		public Entities.MeetingRoomEntity SearchMeetingRoom( string roomName, int pageSize, int pageIndex )
		{
			throw new NotImplementedException();
		}
	}
}
