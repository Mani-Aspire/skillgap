using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace iConnect.Entities
{
    [DataContract]
    public class ImageEntity
    {
        [DataMember]
        public Guid ImageId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Content { get; set; }

    }
}
