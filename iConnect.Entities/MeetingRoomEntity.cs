using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace iConnect.Entities
{
    [DataContract]
    public class MeetingRoomEntity
    {
        [DataMember]
        public string RoomId { get; set; }
        [DataMember]
        public string RoomName { get; set; }
        [DataMember]
        public string Location { get; set; }
        [DataMember]
        public string Extension { get; set; }
    }
}
