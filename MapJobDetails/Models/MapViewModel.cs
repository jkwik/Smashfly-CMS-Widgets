using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SF.CMS.Widgets.MapJobDetails.Models
{
    public class MapViewModel
    {
        public string MapHeight { get; set; }

        public string MapWidth { get; set; }

        public double? JobLongitude { get; set; }

        public double? JobLatitude { get; set; }

        public string JobPushpinImageSrc { get; set; }

        public int JobPushpinWidth { get; set; }

        public int JobPushpinHeight { get; set; }

        public string UserPushpinImageSrc { get; set; }

        public int UserPushpinWidth { get; set; }

        public int UserPushpinHeight { get; set; }

    }
}