using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DotNetNuke.Collections;
using DotNetNuke.Security;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SF.CMS.Widgets.MapJobDetails.Models;

namespace SF.CMS.Widgets.MapJobDetails.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class MapJobDetailsSettingsController : DnnController
    {

        [HttpGet]
        public ActionResult Settings()
        {
            //Settings get route.
            var settings = new Settings();

            settings.Height = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MapJobDetails_MapHeight", "400");
            settings.Width = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MapJobDetails_MapWidth", "400");

            settings.JobPushpinHeight = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("JobPushpinHeight", 30);
            settings.JobPushpinWidth = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("JobPushpinWidth", 30);

            settings.UserPushpinHeight = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("UserPushpinHeight", 30);
            settings.UserPushpinWidth = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("UserPushpinWidth", 30);

            try
            {
                string[] jobfiles = GetFilesFromPath("PATH TO Job_Image DIRECTORY");
                if (jobfiles.Length > 0)
                {
                    settings.JobPushpinImageSrc = GetBytesOfImage(jobfiles[0]);
                    EventLogController.Instance.AddLog("Job Src: ", settings.JobPushpinImageSrc, EventLogController.EventLogType.ADMIN_ALERT);
                }

                string[] userfiles = GetFilesFromPath("PATH TO User_Image DIRECTORY");
                if (userfiles.Length > 0)
                {
                    settings.UserPushpinImageSrc = GetBytesOfImage(userfiles[0]);
                    EventLogController.Instance.AddLog("User Src: ", settings.UserPushpinImageSrc, EventLogController.EventLogType.ADMIN_ALERT);
                }
            }
            catch (DirectoryNotFoundException e)
            {
                EventLogController.Instance.AddLog("Files Error: ", e.ToString(), EventLogController.EventLogType.ADMIN_ALERT);
            }

            return View(settings);
        }

        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(Settings settings)
        {
            //Settings update route.

            ModuleContext.Configuration.ModuleSettings["JobPushpinHeight"] = settings.JobPushpinHeight;
            ModuleContext.Configuration.ModuleSettings["JobPushpinWidth"] = settings.JobPushpinWidth;

            ModuleContext.Configuration.ModuleSettings["UserPushpinHeight"] = settings.UserPushpinHeight;
            ModuleContext.Configuration.ModuleSettings["UserPushpinWidth"] = settings.UserPushpinWidth;

            ModuleContext.Configuration.ModuleSettings["MapJobDetails_MapHeight"] = settings.Height;
            ModuleContext.Configuration.ModuleSettings["MapJobDetails_MapWidth"] = settings.Width;

            return RedirectToDefaultRoute();
        }

        private string[] GetFilesFromPath(string path)
        {
            try
            {
                return Directory.GetFiles(Server.MapPath(path));
            }
            catch (DirectoryNotFoundException dnfe)
            {
                //swallow, first time install
                return new string[0];
            }
        }

        [HttpGet]
        public void ClearJobImage()
        {
            EventLogController.Instance.AddLog("Clearing Image: ", "True", EventLogController.EventLogType.ADMIN_ALERT);
            try
            {
                string[] files = GetFilesFromPath("PATH TO Job_Image DIRECTORY");
                if (files.Length > 0)
                {
                    foreach (string s in files)
                    {
                        System.IO.File.Delete(s);
                    }
                }

            }
            catch (DirectoryNotFoundException e)
            {
                EventLogController.Instance.AddLog("Files Error: ", e.ToString(), EventLogController.EventLogType.ADMIN_ALERT);
            }
        }

        [HttpGet]
        public void ClearUserImage()
        {
            EventLogController.Instance.AddLog("Clearing Image: ", "True", EventLogController.EventLogType.ADMIN_ALERT);
            try
            {
                string[] files = GetFilesFromPath("PATH TO User_Image DIRECTORY");
                if (files.Length > 0)
                {
                    foreach (string s in files)
                    {
                        System.IO.File.Delete(s);
                    }
                }

            }
            catch (DirectoryNotFoundException e)
            {
                EventLogController.Instance.AddLog("Files Error: ", e.ToString(), EventLogController.EventLogType.ADMIN_ALERT);
            }
        }

        [HttpPost]
        public string UploadJobImage()
        {
            //Delete last upload.
            ClearJobImage();

            string imgSrc = null;
            string imgPath = Server.MapPath("PATH TO Job_Image DIRECTORY");
            if (!Directory.Exists(imgPath))
            {
                Directory.CreateDirectory(imgPath);
            }

            foreach (string s in Request.Files)
            {
                var file = Request.Files[s];
                if (file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(imgPath, fileName);

                    ModuleContext.Configuration.ModuleSettings["JobPushpinFilePath"] = path;

                    file.SaveAs(path);
                    imgSrc = GetBytesOfImage(path);
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(new
            {
                src = imgSrc
            });
        }

        [HttpPost]
        public string UploadUserImage()
        {
            //Delete last upload.
            ClearUserImage();

            string imgSrc = null;
            string imgPath = Server.MapPath("PATH TO User_Image DIRECTORY");
            if (!Directory.Exists(imgPath))
            {
                Directory.CreateDirectory(imgPath);
            }

            foreach (string s in Request.Files)
            {
                var file = Request.Files[s];
                if (file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(imgPath, fileName);

                    ModuleContext.Configuration.ModuleSettings["UserPushpinFilePath"] = path;

                    file.SaveAs(path);
                    imgSrc = GetBytesOfImage(path);
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(new
            {
                src = imgSrc
            });
        }

        private string GetBytesOfImage(string path)
        {
            //Get file extension for img src.
            string extension = Path.GetExtension(path);
            extension = extension.Substring(1);
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] byteData = new byte[fs.Length];
            fs.Read(byteData, 0, byteData.Length);
            var base64 = Convert.ToBase64String(byteData);
            var imgSrc = String.Format("data:image/{0};base64,{1}", extension, base64);
            fs.Close();
            return imgSrc;
        }

    }
}