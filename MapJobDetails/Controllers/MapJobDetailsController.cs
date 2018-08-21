using System;
using System.IO;
using System.Web.Mvc;
using DotNetNuke.Collections;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SF.CMS.Widgets.MapJobDetails.Models;
using SF.CMS.Widgets.Shared;

namespace SF.CMS.Widgets.MapJobDetails.Controllers
{
    public class MapJobDetailsController : DnnController
    {

        private readonly ICMSServiceClient _cacheableClient = new CacheableCmsServiceClient();

        public ActionResult Index()
        {
            var MapViewModel = new MapViewModel();
            MapViewModel.MapHeight = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MapJobDetails_MapHeight", "400") + "px";
            MapViewModel.MapWidth = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MapJobDetails_MapWidth", "400") + "px";

            MapViewModel.JobPushpinHeight = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("JobPushpinHeight", 30);
            MapViewModel.JobPushpinWidth = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("JobPushpinWidth", 30);

            MapViewModel.UserPushpinHeight = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("UserPushpinHeight", 30);
            MapViewModel.UserPushpinWidth = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("UserPushpinWidth", 30);

            try
            {
                string[] jobfiles = GetFilesFromPath("PATH TO Job_Image DIRECTORY");
                if (jobfiles.Length > 0)
                {
                    MapViewModel.JobPushpinImageSrc = GetBytesOfImage(jobfiles[0]);
                    EventLogController.Instance.AddLog("Job Src: ", MapViewModel.JobPushpinImageSrc, EventLogController.EventLogType.ADMIN_ALERT);
                }
                else
                {
                    MapViewModel.JobPushpinImageSrc = "empty";
                }

                string[] userfiles = GetFilesFromPath("PATH TO User_Image DIRECTORY");
                if (userfiles.Length > 0)
                {
                    MapViewModel.UserPushpinImageSrc = GetBytesOfImage(userfiles[0]);
                    EventLogController.Instance.AddLog("User Src: ", MapViewModel.UserPushpinImageSrc, EventLogController.EventLogType.ADMIN_ALERT);
                }
                else
                {
                    MapViewModel.UserPushpinImageSrc = "empty";
                }
            }
            catch (DirectoryNotFoundException e)
            {
                EventLogController.Instance.AddLog("Files Error: ", e.ToString(), EventLogController.EventLogType.ADMIN_ALERT);
            }

            int jobId = 0;
            if (Int32.TryParse(Request.QueryString["JobId"], out jobId))
            {
                var jobDetailsModel = _cacheableClient.GetJobDetails(PortalController.GetPortalSettingAsInteger("ClientId", PortalSettings.PortalId, 1234), jobId);
                if (jobDetailsModel != null)
                {
                    MapViewModel.JobLatitude = jobDetailsModel.Latitude;
                    MapViewModel.JobLongitude = jobDetailsModel.Longitude;

                    EventLogController.Instance.AddLog("Latitude, Longitude", ""+MapViewModel.JobLatitude+", "+MapViewModel.JobLongitude+"", EventLogController.EventLogType.ADMIN_ALERT);

                    return View(MapViewModel);
                }
            }

            MapViewModel.JobLatitude = 0.0;
            MapViewModel.JobLongitude = 0.0;
            return View(MapViewModel);
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