using System;
using System.Runtime.Serialization;

namespace iConnect.Entities
{
    [DataContract]
    public class UserEntity
    {
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public bool Status { get; set; }
        [DataMember]
        public RoleEntity Role { get; set; }
    }
}
