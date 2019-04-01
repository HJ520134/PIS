using Newtonsoft.Json;
using SPP.Common.Constants;
using SPP.Core;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace SPP.Model
{
    public class AccountModel
    {
        /// <summary>
        /// Create user ticket
        /// </summary>
        /// <param name="strUserName"></param>
        public string CreateLoginUserTicket(int accountId, string strUserName, string strPassword)
        {
            //build ticket
            FormsAuthenticationTicket ticket = 
                new FormsAuthenticationTicket(1, strUserName, DateTime.Now, DateTime.Now.AddMinutes(240),
                    true, string.Format("{0}:{1}:{2}",accountId, strUserName, strPassword), FormsAuthentication.FormsCookiePath);

            string ticString = FormsAuthentication.Encrypt(ticket);

            //put ticket into Cookie and Session
            //SetAuthCookie make Identity true
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, ticString));
            FormsAuthentication.SetAuthCookie(strUserName, true);
            HttpContext.Current.Session[SessionConstants.LoginTicket] = ticString;
            
            //rewrite user info in HttpContext
            //string[] roles = ticket.UserData.Split(',');
            IIdentity identity = new FormsIdentity(ticket);
            IPrincipal principal = new GenericPrincipal(identity, null);
            HttpContext.Current.User = principal;

            return ticString;
        }

        /// <summary>
        /// sign out
        /// </summary>
        internal void Logout()
        {
            FormsAuthentication.SignOut();
        }
    }
}