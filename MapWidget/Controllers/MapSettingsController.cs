using System.Web.Mvc;
using DotNetNuke.Collections;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SF.CMS.Widgets.MapWidget.Models;
using DotNetNuke.Services.Log.EventLog;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace SF.CMS.Widgets.MapWidget.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class MapSettingsController : DnnController
    {

        [HttpGet]
        public ActionResult Settings()
        {
            //Settings get route.
            var settings = new Settings();

            settings.Height = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MapWidget_Height", "700");
            settings.Width = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MapWidget_Width", "900");

            settings.PushpinHeight = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("PushpinHeight", 30);
            settings.PushpinWidth = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("PushpinWidth", 30);

            settings.ClusterRadius = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("ClusterRadius", 15);
            settings.ClusterColor = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("ClusterColor", "#113676");
            settings.ClusterTextColor = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("ClusterTextColor", "#ffffff");

            settings.DistanceUnit = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("DistanceUnit", "miles");
            settings.HoursUnit = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("HoursUnit", false);
            settings.MinutesUnit = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MinutesUnit", true);

            settings.PolygonFillColor = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("PolygonFillColor", "#85EA75");
            settings.PolygonStrokeColor = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("PolygonStrokeColor", "#85EA75");

            settings.RadiusSearch = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("RadiusSearch", true);
            settings.TravelTimeSearch = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("TravelTimeSearch", true);
            settings.PolygonSearch = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("PolygonSearch", true);

            //Get current pushpin image to display as a thumbnail.
            //Should either be length of 0 or 1 because of the upload handler.
            try
            {
                string[] files = GetFilesFromPath("PATH TO PUSHPIN_IMAGES FOLDER, WILL CREATE ONE IF DOESN'T EXIST.");
                if(files.Length > 0)
                {
                    settings.PushpinImageSrc = GetBytesOfImage(files[0]);
                }
            } catch (DirectoryNotFoundException e)
            {
                EventLogController.Instance.AddLog("Files Error: ", e.ToString(), EventLogController.EventLogType.ADMIN_ALERT);
            }

            return View(settings);
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

        [HttpPost]
        public string Upload()
        {
            //Delete last upload.
            ClearIcon();

            string imgSrc = null;
            string imgPath = Server.MapPath("PATH TO PUSHPIN_IMAGES FOLDER, WILL CREATE ONE IF DOESN'T EXIST.");
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

                    ModuleContext.Configuration.ModuleSettings["PushpinFilePath"] = path;

                    file.SaveAs(path);
                    imgSrc = GetBytesOfImage(path);
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(new {
                src = imgSrc
            });
        }

        [HttpGet]
        public void ClearIcon()
        {
            EventLogController.Instance.AddLog("Clearing Image: ", "True", EventLogController.EventLogType.ADMIN_ALERT);
            try
            {
                string[] files = GetFilesFromPath("PATH TO PUSHPIN_IMAGES FOLDER, WILL CREATE ONE IF DOESN'T EXIST.");
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
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(Settings settings)
        {
            //Settings update route.

            ModuleContext.Configuration.ModuleSettings["MapWidget_Height"] = settings.Height;
            ModuleContext.Configuration.ModuleSettings["MapWidget_Width"] = settings.Width;
            ModuleContext.Configuration.ModuleSettings["PushpinHeight"] = settings.PushpinHeight;
            ModuleContext.Configuration.ModuleSettings["PushpinWidth"] = settings.PushpinWidth;

            ModuleContext.Configuration.ModuleSettings["ClusterRadius"] = settings.ClusterRadius;
            ModuleContext.Configuration.ModuleSettings["ClusterColor"] = settings.ClusterColor;
            ModuleContext.Configuration.ModuleSettings["ClusterTextColor"] = settings.ClusterTextColor;
            ModuleContext.Configuration.ModuleSettings["DistanceUnit"] = settings.DistanceUnit;
            ModuleContext.Configuration.ModuleSettings["HoursUnit"] = settings.HoursUnit;
            ModuleContext.Configuration.ModuleSettings["MinutesUnit"] = settings.MinutesUnit;

            ModuleContext.Configuration.ModuleSettings["PolygonFillColor"] = settings.PolygonFillColor;
            ModuleContext.Configuration.ModuleSettings["PolygonStrokeColor"] = settings.PolygonStrokeColor;

            ModuleContext.Configuration.ModuleSettings["RadiusSearch"] = settings.RadiusSearch;
            ModuleContext.Configuration.ModuleSettings["TravelTimeSearch"] = settings.TravelTimeSearch;
            ModuleContext.Configuration.ModuleSettings["PolygonSearch"] = settings.PolygonSearch;

            return RedirectToDefaultRoute();
        }

    }
}