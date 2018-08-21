using System.Collections.Generic;

namespace SF.CMS.Widgets.YoutubeWidget.Models
{
    public class Settings
    {

        public string Link { get; set; }
        
        public int Width { get; set; }

        public int Height { get; set; }

        public int StartAt{ get; set; }

        public bool ShowSuggested { get; set; }

        public bool ShowPlayerControls{ get; set; }

        public bool ShowVideoTitlePlayerActions{ get; set; }

        public bool EnablePrivacyEnhancedMode { get; set; }

        public string EmbedUrl { get; set; }

    }

}