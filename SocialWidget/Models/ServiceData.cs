using Newtonsoft.Json;
using System.Collections.Generic;

namespace SF.CMS.Widgets.SocialWidget.Models
{
    public class ServiceData
    {
        [JsonProperty("data")]
        public List<Service> Data { get; set; }
    }
}