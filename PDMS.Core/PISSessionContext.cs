using PDMS.Common.Constants;
using PDMS.Model;
using PDMS.Model.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PDMS.Core
{
    public class PISSessionContext
    {
        private HttpContext _context = HttpContext.Current;

        #region 设定上下文
        /// <summary>
        /// 设定上下文
        /// </summary>
        public static PISSessionContext Current
        {
            get
            {
                // 当HttpContext为Null时直接返回Null说明过期
                if (HttpContext.Current == null)
                    return null;

                // 当设定的为null时，添加对象到HttpContext
                if (HttpContext.Current.Items[SessionConstants.PISSessionContext] == null)
                {
                    // 创建对象并且添加到HttpContext

                    PISSessionContext context = new PISSessionContext();
                    HttpContext.Current.Items.Add(SessionConstants.PISSessionContext, context);
                    return context;
                }

                // 返回已经设定的对象
                return (PISSessionContext)HttpContext.Current.Items[SessionConstants.PISSessionContext];
            }
        }
        #endregion

        public string CurrentLoginToken
        {
            get
            {
                return _context.Session[SessionConstants.LoginTicket] as string;
            }
            set
            {
                _context.Session[SessionConstants.LoginTicket] = value;
            }
        }

        public string CurrentAccountUID
        {
            get
            {
                return _context.Session[SessionConstants.CurrentAccountUID] as string;
            }
            set
            {
                _context.Session[SessionConstants.CurrentAccountUID] = value;
            }
        }

        public string CurrentUserName
        {
            get
            {
                return _context.Session[SessionConstants.CurrentUserName] as string;
            }
            set
            {
                _context.Session[SessionConstants.CurrentUserName] = value;
            }
        }

        public CustomUserInfoVM CurrentUser
        {
            get
            {
                return _context.Session[SessionConstants.CurrentUserInfo] as CustomUserInfoVM;
            }
            set
            {
                _context.Session[SessionConstants.CurrentUserInfo] = value;
            }
        }

        public SystemLanguageDTO CurrentWorkingLanguage
        {
            get
            {
                return _context.Session[SessionConstants.CurrentWorkingLanguage] as SystemLanguageDTO;
            }
            set
            {
                _context.Session[SessionConstants.CurrentWorkingLanguage] = value;
            }
        }

        public List<SystemLanguageDTO> Languages
        {
            get
            {
                return _context.Session[SessionConstants.Languages] as List<SystemLanguageDTO>;
            }
            set
            {
                _context.Session[SessionConstants.Languages] = value;
            }
        }

        public string CurrentDB
        {
            get
            {
                return _context.Session[SessionConstants.CurrentDB] as string;
            }
            set
            {
                _context.Session[SessionConstants.CurrentDB] = value;
            }
        }

        public void RemoveAllSession()
        {
            _context.Session.RemoveAll();
        }



        #region 重写索引器
        /// <summary>
        /// 重写索引器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                if (this._context == null)
                {
                    return null;
                }

                if (this._context.Items[key] != null)
                {
                    return this._context.Items[key];
                }
                return null;
            }
            set
            {
                if (this._context != null)
                {
                    this._context.Items.Remove(key);
                    this._context.Items.Add(key, value);

                }
            }
        }
        #endregion
    }
}
