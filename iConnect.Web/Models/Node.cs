using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iConnect.Presentation.Models
{
    public class Node
    {
        public string Name { set; get; }
        public string NodePath { set; get; }
        public string ParentNodePath { set; get; }

        public IList<Node> Nodes { set; get; }

        public Node()
        {
            Nodes = new List<Node>();
        }

    }
}