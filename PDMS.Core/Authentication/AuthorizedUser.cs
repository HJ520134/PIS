using PDMS.Common.Constants;
using PDMS.Model;

using System.Web;
using System.Web.Security;

namespace PDMS.Core
{
    public class AuthorizedUser
    {
        private AuthorizedUser()
        {
        }

        /// <summary>
        /// Unauthorized element in each authorized pages
        /// </summary>
        public PageUnauthorizedElement[] PageUnauthorizedElements
        {
            get
            {
                return HttpContext.Current.Session[SessionConstants.PageUnauthorizedElements] as PageUnauthorizedElement[];
            }
        }

        /// <summary>
        /// User ticket
        /// </summary>
        public string UserLoginTicket
        {
            get
            {
                if (HttpContext.Current.Session[SessionConstants.LoginTicket] != null)
                {
                    return HttpContext.Current.Session[SessionConstants.LoginTicket].ToString();
                }
                else
                {
                    var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                    if (cookie != null)
                    {
                        //session is invalid, write again with info from cookie
                        HttpContext.Current.Session[SessionConstants.LoginTicket] = cookie.Value;
                        return cookie.Value;
                    }
                    else
                    {
                        //cookie is invalid, relogin.
                        return string.Empty;
                    }
                }
            }
        }


        public static AuthorizedUser Current
        {
            get
            {
                //var user = (new AuthoirzedUserManager()).GetByUserName();
                return new AuthorizedUser();
            }
        }
    }
}
