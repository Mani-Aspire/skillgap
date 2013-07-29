using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace iConnect.Entities
{
    [DataContract]
    public class PhotoEntity
    {
        [DataMember]
        public Guid PhotoId { get; set; }
        [DataMember]
        public string PhotoName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime UploadDate { get; set; }
        [DataMember]
        public UserEntity UploadedBy { get; set; }
        [DataMember]
        public ImageEntity Image { get; set; }
    }
}
