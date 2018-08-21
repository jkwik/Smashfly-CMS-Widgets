namespace SF.CMS.Widgets.SocialWidget.Models
{
    public class Service
    {
        public string name { get; set; }

        public string code { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}