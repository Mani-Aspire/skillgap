using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace iConnect.Entities
{
    [DataContract]
    public class AnnouncementEntity
    {
        [DataMember]
        public Guid AnnouncementId { get; set; }

        [DataMember]
        public UserEntity User { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Body { get; set; }

        [DataMember]
        public bool Status { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public ImageEntity Image { get; set; }



    }
    [DataContract]
    public class AnnouncementList
    {
        [DataMember]
        public Collection<AnnouncementEntity> Announcements { get; set; }
        
    }
}
