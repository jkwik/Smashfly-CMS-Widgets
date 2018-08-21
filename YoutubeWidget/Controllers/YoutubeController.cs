using System.Web.Mvc;
using DotNetNuke.Collections;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SF.CMS.Widgets.YoutubeWidget.Models;

namespace SF.CMS.Widgets.YoutubeWidget.Controllers
{
    public class YoutubeController : DnnController
    {
        public ActionResult Index()
        {

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

            string LinkExtension = settings.Link.Substring(settings.Link.LastIndexOf('=') + 1);
            string PrivacyEnhancedMode = "";

            int SettingsAdded = 0;

            if (settings.EnablePrivacyEnhancedMode)
            {
                PrivacyEnhancedMode = "-nocookie";
            }

            string YoutubeUrl = "https://www.youtube" + PrivacyEnhancedMode + ".com/embed/" + LinkExtension;

            if (!settings.ShowSuggested)
            {
                if (SettingsAdded > 0)
                {
                    YoutubeUrl += "&amp;rel=0";
                }
                else
                {
                    YoutubeUrl += "?rel=0";
                }
                SettingsAdded++;
            }

            if (!settings.ShowPlayerControls)
            {
                if (SettingsAdded > 0)
                {
                    YoutubeUrl += "&amp;controls=0";
                }
                else
                {
                    YoutubeUrl += "?controls=0";
                }
                SettingsAdded++;
            }

            if (!settings.ShowVideoTitlePlayerActions)
            {
                if (SettingsAdded > 0)
                {
                    YoutubeUrl += "&amp;showinfo=0";
                }
                else
                {
                    YoutubeUrl += "?showinfo=0";
                }
                SettingsAdded++;
            }

            if (settings.StartAt != 0)
            {
                if (SettingsAdded > 0)
                {
                    YoutubeUrl += "&amp;start=" + settings.StartAt;
                }
                else
                {
                    YoutubeUrl += "?start=" + settings.StartAt;
                }
            }

            settings.EmbedUrl = YoutubeUrl;

            return View(settings);

        }
    }
}