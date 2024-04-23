using System;

namespace Hotels.Utilities
{
    public class AppAction
    {
        public AppAction(int position, string display, AppUriAction appUriAction, string description = "") 
        {
            this.Position = position;
            this.Display = display;
            this.AppUriAction = appUriAction;
            this.Description = description;
        }
        public int Position { get; set; }
        public string Display { get; set; }
        public AppUriAction AppUriAction { get; set; }
        public string Description { get; set; }
        
    }

    public class AppUriAction
    {
        public AppUriAction(string method, string controller, object otherUriParameters=null)
        {
            this.Method = method;
            this.Controller = controller;
            this.OtherUriParameters = otherUriParameters;
        }
        public string Method { get; set; }
        public string Controller { get; set; }
        public object OtherUriParameters { get; set; }
    }
}
