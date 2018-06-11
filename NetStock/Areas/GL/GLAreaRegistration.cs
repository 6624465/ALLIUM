using System.Web.Mvc;

namespace NetStock.Areas.GL
{
    public class GLAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "GL";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.MapMvcAttributeRoutes();
        }
    }
}