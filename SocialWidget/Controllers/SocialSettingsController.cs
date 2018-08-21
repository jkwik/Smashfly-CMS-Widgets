using System.Web.Mvc;
using DotNetNuke.Collections;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SF.CMS.Widgets.SocialWidget.Models;

namespace SF.CMS.Widgets.SocialWidget.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SocialSettingsController : DnnController
    {

        private ServiceRepository serviceRepository;

        [HttpGet]
        public ActionResult Settings()
        {
            var settings = new Settings();
            serviceRepository = new ServiceRepository();

            settings.Allignment = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Allignment", "Left");
            settings.Dimensions = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Dimensions", "32x32");
            settings.ServicesCollection = serviceRepository.GetServices().Data;
            settings.SelectedServices = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Services",
                "")?.Split(',');
            settings.CompactButton = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Compact", false);
            settings.CounterButton = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Counter", false);
            settings.Orientation = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Social_DNN_Orientation", "Horizontal");

            return View(settings);
        }

        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(Settings settings)
        {
            //Storing Allignment, Dimensions, and Services settings into database.
            ModuleContext.Configuration.ModuleSettings["Social_DNN_Allignment"] = settings.Allignment;
            ModuleContext.Configuration.ModuleSettings["Social_DNN_Dimensions"] = settings.Dimensions;
            
            ModuleContext.Configuration.ModuleSettings["Social_DNN_Services"] = settings.SelectedServices != null
                ? string.Join(",", settings.SelectedServices)
                : null;

            ModuleContext.Configuration.ModuleSettings["Social_DNN_Compact"] = settings.CompactButton;
            ModuleContext.Configuration.ModuleSettings["Social_DNN_Counter"] = settings.CounterButton;
            ModuleContext.Configuration.ModuleSettings["Social_DNN_Orientation"] = settings.Orientation;

            return RedirectToDefaultRoute();
        }

    }
}