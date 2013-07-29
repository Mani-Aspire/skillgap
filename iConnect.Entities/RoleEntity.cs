using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace iConnect.Entities
{
    [DataContract]
    public class RoleEntity
    {
        [DataMember]
        public Guid RoleId { get; set; }
        [DataMember]
        public string RoleCode { get; set; }
        [DataMember]
        public Collection<PrivilegeEntity> Privileges { get; set; }
    }
}
