using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Reflection;

namespace LkFinance
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            //загружаем базовые настройки smpl
            SMPL.Core.load(new SMPL.Models.LoadOptionsModel()
            {
                connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LkFinance"].ConnectionString,
                controllerNamespace = "LkFinance.Controllers",
                isUnitTest = false,
                parentAssembly = Assembly.GetExecutingAssembly(),
                security = new SMPL.Models.LoadOptionsModel.SecurityModel()
                {
                    isCookieCheckEnabled = true,
                    isCheckRoles = true,
                    listRouteWithoutCookie = new List<string>() { "/login", "/ext/", "/api/" }
                },
                listRouteException = new List<SMPL.Models.LoadOptionsModel.RouteExceptionModel>()
                {

                   new SMPL.Models.LoadOptionsModel.RouteExceptionModel(){ methodPath = "ext/", url = "ext/" }
                },
               
                json = new SMPL.Models.LoadOptionsModel.JsonModel() { dateTimeFormat = "yyyy-MM-dd" }
            });

        }


        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}