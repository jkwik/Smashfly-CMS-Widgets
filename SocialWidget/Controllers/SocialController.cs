using System;
using System.Web.Mvc;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SF.CMS.Widgets.SocialWidget.Models;
using DotNetNuke.Collections;
using DotNetNuke.Services.Log.EventLog;

namespace SF.CMS.Widgets.SocialWidget.Controllers
{
    public class SocialController : DnnController
    {

        public ActionResult Index()
        {
            var social = new Social
            {
                Allignment =
                    ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Allignment", "Left"),
                Dimensions =
                    ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Dimensions", "32x32"),
                SelectedServices = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Services",
                    "preferred_1,preferred_2,preferred_3,preferred_4").Split(','),
                CompactButton =
                    ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Compact", false),
                CounterButton = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Counter", false)
            };


            social.Orientation = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Orientation", "Horizontal");

            return View(social);
        }
    }
}