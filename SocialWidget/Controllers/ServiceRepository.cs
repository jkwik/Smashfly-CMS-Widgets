using System;
using System.Net.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Installer.Log;
using Newtonsoft.Json;
using SF.CMS.Widgets.SocialWidget.Models;

namespace SF.CMS.Widgets.SocialWidget.Controllers
{
    public class ServiceRepository
    {
    
        public string ServiceUri = "http://cache.addthiscdn.com/services/v1/sharing.en.json";

        public ServiceData GetServices()
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(ServiceUri)
            };

            try
            {
                var result = client.GetAsync("").Result;
                return JsonConvert.DeserializeObject<ServiceData>(result.Content.ReadAsStringAsync().Result);
            } catch (Exception e)
            {
                new Logger().AddFailure(e);
                return null;
            }
            

        }

    }
}