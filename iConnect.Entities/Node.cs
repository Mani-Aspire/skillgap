using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace iConnect.Entities
{
    [DataContract]
    public class Node
    {
        [DataMember]
        public Collection<Node> Nodes { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public byte[] Content { get; set; }
        [DataMember]
        public NodeType NodeType { get; set; }
        [DataMember]
        public string NodePath { get; set; }
        [DataMember]
        public DateTime CreatedOn { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }

    }
    public enum NodeType : int
    {
        None = 0,
        Folder = 1,
        File = 2
    }

}
