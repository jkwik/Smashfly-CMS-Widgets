﻿using System.Collections.Generic;

namespace SF.CMS.Widgets.MapWidget.Models
{
    public class Settings
    {
        public string Height { get; set; }

        public string Width { get; set; }

        public string PushpinImageSrc { get; set; }

        public int PushpinWidth { get; set; }

        public int PushpinHeight { get; set; }

        public int ClusterRadius { get; set; }

        public string ClusterColor { get; set; }

        public string ClusterTextColor { get; set; }

        public string DistanceUnit { get; set; }

        public bool HoursUnit { get; set; }

        public bool MinutesUnit { get; set; }

        public string PolygonFillColor { get; set; }

        public string PolygonStrokeColor { get; set; }

        public bool RadiusSearch { get; set; }

        public bool TravelTimeSearch { get; set; }

        public bool PolygonSearch { get; set; }

    }

}