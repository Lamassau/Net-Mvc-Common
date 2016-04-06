using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TeknoMobi.Common.Mvc
{
    public class teknoControllerFactory : DefaultControllerFactory
    {
        IContainer teknoContainer = IOCCommonConfig.GetContainer();

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            try
            {
                if ((requestContext == null) || (controllerType == null))
                    return null;

                return (Controller)teknoContainer.GetInstance(controllerType) as teknoController;
            }
            catch (StructureMapException)
            {

                //System.Diagnostics.Debug.WriteLine(teknoContainer.WhatDoIHave());
                throw new Exception(teknoContainer.WhatDoIHave());
            }

        }
    }
}