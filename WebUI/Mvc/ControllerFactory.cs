using System;
using System.Web.Mvc;
using System.Web.Routing;
using ObjectFactory;

namespace AspNetDeploy.WebUI.Mvc
{
    public class ControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return (IController) typeof(Factory).GetMethod("GetInstance").MakeGenericMethod(controllerType).Invoke(null, new object[] { });
        }
    }
}