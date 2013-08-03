using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using iConnect.Presentation.Models;
using System.IO;

namespace iConnect.Presentation.Controllers
{
    public class KnowledgeBaseController : Controller
    {
        
        public ActionResult Index()
        {
            //var jsSerializer = new JavaScriptSerializer();
            //string serializedData = string.Empty;
            List<Node> folderList = new List<Node>();
            TempData["RootUrl"] = DirectoryLibrary.RootDirectory;
            folderList = DirectoryLibrary.GetDirectory();
            // serializedData = jsSerializer.Serialize(folderList);
            //   return serializedData;
            return View(folderList);
        }

        public string AddDirectory(string selectedPath, string folderName)
        {
            var jsSerializer = new JavaScriptSerializer();
            string serializedData = string.Empty;
            try
            {
                string dirPath = string.Format("{0}{1}\\{2}", DirectoryLibrary.RootDirectory, selectedPath, folderName);
                DirectoryInfo dir = new DirectoryInfo(dirPath);

                if (!dir.Exists)
                {
                    dir.Create();
                    serializedData = jsSerializer.Serialize("Folder has been created successfullly!.");
                }
                else
                {
                    serializedData = jsSerializer.Serialize(string.Format("{0} folder already exists!", folderName));
                }
            }
            catch (Exception ex)
            {
                serializedData = jsSerializer.Serialize(ex.Message);
            }
            return serializedData;
        }

        [HttpPost]
        public JsonResult UploadTest()
        {
            bool result = false;
            try
            {
                string uploadDir = Request.Headers["X-Root-Path"];
                string fileName = Request.Headers["X-File-Name"];
                string fileType = Request.Headers["X-File-Type"];
                int fileSize = Convert.ToInt32(Request.Headers["X-File-Size"]);


                //File's content is available in Request.InputStream property
                System.IO.Stream fileContent = Request.InputStream;

                System.IO.FileStream fileStream = System.IO.File.Create(string.Format("{0}{1}\\{2}", DirectoryLibrary.RootDirectory, uploadDir, fileName));
                fileContent.Seek(0, System.IO.SeekOrigin.Begin);
                //Copying file's content to FileStream
                fileContent.CopyTo(fileStream);
                fileStream.Dispose();
                result = true;
            }
            catch
            {
                result = false;
            }
            return Json(result);
        }

        public string DeleteDirectory(string folderPath)
        {
            var jsSerializer = new JavaScriptSerializer();
            string serializedData = string.Empty;

            try
            {
                string DirPath = string.Format("{0}{1}", DirectoryLibrary.RootDirectory, folderPath);
                DirectoryInfo dir = new DirectoryInfo(DirPath);
                if (dir.Exists)
                {
                    dir.Delete(true);
                    serializedData = jsSerializer.Serialize("Folder has been deleted successfullly!.");
                }
                else
                {
                    serializedData = jsSerializer.Serialize("Folder not exists");
                }
            }
            catch (Exception ex)
            {
                serializedData = jsSerializer.Serialize(ex.Message);
            }
            return serializedData;

        }

    }
}
