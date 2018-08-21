using System.Web.Mvc;
using DotNetNuke.Collections;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SF.CMS.Widgets.MapWidget.Models;
using System.Configuration;
using SF.CMS.Widgets.Shared;
using System.IO;
using System;
using DotNetNuke.Services.Log.EventLog;

namespace SF.CMS.Widgets.MapWidget.Controllers
{
    public class MapController : BaseController
    {
        public ActionResult Index()
        {
            var mapviewmodel = new MapViewModel();
            mapviewmodel.JobsApiUrl = "BASE URL TO JOBS API";
            mapviewmodel.Height = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MapWidget_Height", "700") + "px";
            mapviewmodel.Width = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MapWidget_Width", "900") + "px";
            mapviewmodel.PushpinWidth = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("PushpinWidth", 30);
            mapviewmodel.PushpinHeight = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("PushpinHeight", 30);

            mapviewmodel.ClusterRadius = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("ClusterRadius", 15);
            mapviewmodel.ClusterColor = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("ClusterColor", "#113676");
            mapviewmodel.ClusterTextColor = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("ClusterTextColor", "#ffffff");

            mapviewmodel.DistanceUnit = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("DistanceUnit", "miles");

            mapviewmodel.MinutesUnit = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MinutesUnit", true);
            mapviewmodel.HoursUnit = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("HoursUnit", false);

            mapviewmodel.PolygonFillColor = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("PolygonFillColor", "#85EA75");
            mapviewmodel.PolygonStrokeColor = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("PolygonStrokeColor", "#a495b2");

            mapviewmodel.RadiusSearch = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("RadiusSearch", true);
            mapviewmodel.TravelTimeSearch = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("TravelTimeSearch", true);
            mapviewmodel.PolygonSearch = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("PolygonSearch", true);

            string[] files = GetFilesFromPath("PATH TO PUSHPIN_IMAGES FOLDER, WILL CREATE ONE AT THIS LOCATION IF DOESN'T EXIST.");
            if (files.Length > 0)
            {
                mapviewmodel.PushpinBase64 = GetBytesOfImage(files[0]);
            }
            else
            {
                mapviewmodel.PushpinBase64 = "empty";
            }

            return View(mapviewmodel);
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
    }
}