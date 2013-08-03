using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
namespace iConnect.Presentation.Models
{
    public static class DirectoryLibrary
    {
        public static string RootDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings["KnowledgeBase_RootPath"];
            }
        }

        public static List<Node> GetDirectory()
        {
            List<Node> folderList = new List<Node>();

            folderList = GetDirectory(new DirectoryInfo(RootDirectory));

            var topLevelNodes = folderList.Where(x => x.ParentNodePath == null).ToList();

            foreach (var topLevelNode in topLevelNodes)
            {
                SetSubFolder(topLevelNode, folderList);
            }
            return topLevelNodes;

        }

        private static void SetSubFolder(Node model, List<Node> folderList)
        {
            var subFolders = folderList.
                            Where(x => x.ParentNodePath == model.NodePath).ToList();
            if (subFolders.Count > 0)
            {
                foreach (var subFolder in subFolders)
                {
                    SetSubFolder(subFolder, folderList);
                    model.Nodes.Add(subFolder);
                }
            }
        }

        private static List<Node> GetDirectory(DirectoryInfo directory)
        {
            List<Node> _AllFolders = new List<Node>();
            //Add Root
            Node node = new Node();
            node.Name = directory.Name;
            node.NodePath = directory.FullName;
            node.ParentNodePath = null;
            _AllFolders.Add(node);

            foreach (string subDirectory in Directory.GetDirectories(RootDirectory, "*", SearchOption.AllDirectories))
            {
                DirectoryInfo subfolder = new DirectoryInfo(subDirectory);
                node = new Node();
                node.Name = subfolder.Name;
                node.NodePath = subfolder.FullName;
                node.ParentNodePath = subfolder.Parent.FullName;
                _AllFolders.Add(node);
            }
            return _AllFolders;
        }
    }
}