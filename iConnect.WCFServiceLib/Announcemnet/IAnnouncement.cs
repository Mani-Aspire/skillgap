using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using iConnect.Entities;

namespace iConnect.WCFServiceLib
{
    [ServiceContract]
    public interface IAnnouncement
    {
        [OperationContract]
        AnnouncementList GetAnnouncements(int pageSize, int pageIndex);

		[OperationContract]
		string GetSample();

        [OperationContract]
        string CreateAnnouncement(AnnouncementEntity announcementEntity);

        [OperationContract]
        Node GetDocumentByPath(string documentPath);

        [OperationContract]
        bool CreateDocumentOrFolder(Node node);

        [OperationContract]
        bool DeleteDocumentOrFolder(Node node);

        [OperationContract]
        MeetingRoomEntity SearchMeetingRoom(string roomName, int pageSize, int pageIndex);

    }

}
