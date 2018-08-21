using System.Collections.Generic;

namespace SF.CMS.Widgets.SocialWidget.Models
{
    public class Settings
    {
        public string Allignment { get; set; }

        public string Dimensions { get; set; }

        public string[] SelectedServices { get; set; }

        public bool CompactButton { get; set; }

        public bool CounterButton { get; set; }

        public string Orientation { get; set; }

        public IEnumerable<Service> ServicesCollection{ get; set; }

        public IEnumerable<AllignmentSelection> AllignmentOptions =
            new List<AllignmentSelection>
            {
                new AllignmentSelection { Allignment = "left" },
                new AllignmentSelection { Allignment = "center"},
                new AllignmentSelection { Allignment = "right"},
                new AllignmentSelection { Allignment = "justify"}
            };

        public IEnumerable<DimensionsSelection> DimensionsOptions =
            new List<DimensionsSelection>
            {
                new DimensionsSelection { Dimensions = "16x16" },
                new DimensionsSelection { Dimensions = "20x20" },
                new DimensionsSelection { Dimensions = "32x32" }
            };

        public IEnumerable<OrientationSelection> OrientationOptions =
            new List<OrientationSelection>
            {
                new OrientationSelection { Orientation = "Horizontal" },
                new OrientationSelection { Orientation = "Vertical" }
            };

    }

    public class AllignmentSelection
    {
        public string Allignment { get; set; }

        public override string ToString()
        {
            return Allignment;
        }
    }

    public class DimensionsSelection
    {
        public string Dimensions { get; set; }

        public override string ToString()
        {
            return Dimensions;
        }
    }

    public class OrientationSelection
    {

        public string Orientation { get; set; }

        public override string ToString()
        {
            return Orientation;
        }

    }
}