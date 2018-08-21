using System.Web.Mvc;
using DotNetNuke.Collections;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SF.CMS.Widgets.YoutubeWidget.Models;

namespace SF.CMS.Widgets.YoutubeWidget.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class YoutubeSettingsController : DnnController
    {

        [HttpGet]
        public ActionResult Settings()
        {
            //Settings get route.
            var settings = new Settings
            {
                Link = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("YoutubeWidget_Link", "https://www.youtube.com/watch?v=WxZRcOLJ-6k"),
                Width = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("YoutubeWidget_Width", 560),
                Height = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("YoutubeWidget_Height", 315),
                ShowSuggested = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("YoutubeWidget_ShowSuggested", true),
                ShowPlayerControls = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("YoutubeWidget_ShowPlayerControls", true),
                ShowVideoTitlePlayerActions = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("YoutubeWidget_ShowTitlePlayerActions", true),
                EnablePrivacyEnhancedMode = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("YoutubeWidget_EnablePrivacy", false),
                StartAt = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("YoutubeWidget_StartAt", 0)
            };

            return View(settings);
        }

        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(Settings settings)
        {
            //Settings update route.
            ModuleContext.Configuration.ModuleSettings["YoutubeWidget_Link"] = settings.Link;
            ModuleContext.Configuration.ModuleSettings["YoutubeWidget_Width"] = settings.Width;
            ModuleContext.Configuration.ModuleSettings["YoutubeWidget_Height"] = settings.Height;
            ModuleContext.Configuration.ModuleSettings["YoutubeWidget_ShowSuggested"] = settings.ShowSuggested;
            ModuleContext.Configuration.ModuleSettings["YoutubeWidget_ShowPlayerControls"] = settings.ShowPlayerControls;
            ModuleContext.Configuration.ModuleSettings["YoutubeWidget_ShowTitlePlayerActions"] = settings.ShowVideoTitlePlayerActions;
            ModuleContext.Configuration.ModuleSettings["YoutubeWidget_EnablePrivacy"] = settings.EnablePrivacyEnhancedMode;
            ModuleContext.Configuration.ModuleSettings["YoutubeWidget_StartAt"] = settings.StartAt;

            return RedirectToDefaultRoute();
        }

    }
}