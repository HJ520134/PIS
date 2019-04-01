using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Model;
using PDMS.Service;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;

namespace PDMS.WebAPI.Controllers
{
    public class LoginController : ApiControllerBase
    {

        ICommonService commonService;
        ISystemService systemService;
        ISettingsService settingService;

        public LoginController(ICommonService commonService,ISystemService systemService,ISettingsService settingService)
        {
            this.commonService = commonService;
            this.systemService = systemService;
            this.settingService = settingService;
        }
     
        [AllowAnonymous]
        public HttpResponseMessage LoginIn(LoginUserMoel loginUser)
        {
            var systemUser = commonService.GetSystemUserByNTId(loginUser.UserName,1);

            if (systemUser == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "ACCOUNT NOT EXIST");
            }
            if (systemUser.Enable_Flag==false)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "ACCOUNT NOT ENABLED");
            }

            //var userInfo = commonService.GetUserInfo(systemUser.Account_UID);

            var LDAPswitch = ConfigurationManager.AppSettings["LDAPAuthentication"].ToString();

            //判断WebConfig里面是否开启了需要输入密码登录
            if (!string.IsNullOrWhiteSpace(LDAPswitch) && LDAPswitch.Equals("ON", StringComparison.CurrentCultureIgnoreCase))
            {
                ValidateUser validateUser = new ValidateUser(settingService);
                //如果是物料员登录，则要判断是成都还是无锡的专案，成都的不需要输入密码，无锡的需要输入密码
                if (systemUser.MH_Flag)
                {
                    var projectSite = commonService.GetProjectSite(systemUser.Account_UID);
                    switch (projectSite)
                    {
                        case "CTU":
                            break;
                        case "WUXI_M":
                            if (string.IsNullOrEmpty(loginUser.Password) ||
    !validateUser.LDAPValidateByMHFlag(loginUser.UserName, loginUser.Password, loginUser.IsEmployee))
                            {
                                return Request.CreateResponse(HttpStatusCode.Unauthorized, "WRONG PASSWORD");
                            }
                            break;
                    }
                }
                else if (systemUser.RoleList != null && systemUser.RoleList.Exists(x => x.Role_ID == "PlayBoardPlayUser"))
                {
                    //硬编码的角色Role_ID,这个角色免密码登录，直接显示播放看板
                    //PlayBoardPlayUser 播放看板播放账号

                }
                else if (systemUser.User_Name.Contains("电子看板"))
                {
                }
                else
                {
                    if (string.IsNullOrEmpty(loginUser.Password) ||
                        !validateUser.LDAPValidate(loginUser.UserName, loginUser.Password, loginUser.IsEmployee))
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, "WRONG PASSWORD");
                    }

                    //loginUser.Password = "TopSecreat";
                }
                //如果不是物料员帐号登录则需要密码

                //if (!systemUser.MH_Flag && !systemUser.User_Name.Contains("电子看板"))
                //{
                //    //LDAP Authentication

                //    ValidateUser validateUser = new ValidateUser(settingService);
                //    if (string.IsNullOrEmpty(loginUser.Password) ||
                //        !validateUser.LDAPValidate(loginUser.UserName, loginUser.Password, loginUser.IsEmployee))
                //    {
                //        return Request.CreateResponse(HttpStatusCode.Unauthorized, "WRONG PASSWORD");
                //    }
                //}
                //else
                //    loginUser.Password = "TopSecreat";
            }
            else
            {
                loginUser.Password = string.Empty;
            }

            //登录后，更新登录时间

            systemService.updateLastLoginDate(systemUser.Account_UID);
            //从db获取token数据并解密
            var userlogintoken = string.Empty;
            bool refresh = systemUser.LoginToken == null;
            FormsAuthenticationTicket ticket = null;

            if (!refresh)
            {
                userlogintoken = systemUser.LoginToken;

                try
                {
                    ticket = FormsAuthentication.Decrypt(userlogintoken);
                }
                catch
                {
                    refresh = true;
                }
            }

            if (refresh || loginUser.Password != ticket.UserData || loginUser.UserName != ticket.Name)
            {
                //userlogintoken = ReFreshToken(systemUser.Account_UID, loginUser.Password);
            }

            return Request.CreateResponse(new AuthorizedLoginUser {
                Account_UID = systemUser.Account_UID,
                User_Name = systemUser.User_Name,
                System_Language_UID = systemUser.System_Language_UID,
                Token = userlogintoken,
                MH_Flag = systemUser.MH_Flag,
                IsMulitProject = systemUser.IsMulitProject,
                flowChartMaster_Uid = systemUser.flowChartMaster_Uid,
                USER_Ntid = systemUser.User_NTID,
                RoleList = systemUser.RoleList
            });
        }

        /// <summary>
        /// 无密码登录并设置设置用户相关Session, Copy 的LoginIn
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ApiResultModel LoginInWithoutPassword(int uid, string roleID)
        {
            ApiResultModel resultModel = new ApiResultModel() { isSuccess = true};
            //var systemUser = commonService.GetSystemUserByNTId(loginUser.UserName, 1);
            var systemUser = commonService.GetSystemUserByUId(uid);

            if (systemUser == null)
            {
                resultModel.isSuccess = false;
                resultModel.message = "账号不存在";
            }
            else if (systemUser.Enable_Flag == false)
            {
                resultModel.isSuccess = false;
                resultModel.message = "账号被禁用";
            }
            else if (systemUser.RoleList == null || systemUser.RoleList.Count == 0)
            {
                resultModel.isSuccess = false;
                resultModel.message = "账号没有任何角色";
            }
            else if (systemUser.RoleList != null && systemUser.RoleList.Count > 0 && !systemUser.RoleList.Exists(x => x.Role_ID == roleID))
            {
                resultModel.isSuccess = false;
                resultModel.message = "账号没有播放角色";
            }

            //不成功返回resultModel
            if (!resultModel.isSuccess)
            {
                return resultModel;
            }

            //无密码登录
            //loginUser.Password = string.Empty;

            //登录后，更新登录时间

            systemService.updateLastLoginDate(systemUser.Account_UID);
            //从db获取token数据并解密
            var userlogintoken = string.Empty;
            bool refresh = systemUser.LoginToken == null;
            FormsAuthenticationTicket ticket = null;

            if (!refresh)
            {
                userlogintoken = systemUser.LoginToken;

                try
                {
                    ticket = FormsAuthentication.Decrypt(userlogintoken);
                }
                catch
                {
                    refresh = true;
                }
            }

            //if (refresh || loginUser.Password != ticket.UserData || loginUser.UserName != ticket.Name)
            //{
            //userlogintoken = ReFreshToken(systemUser.Account_UID, loginUser.Password);
            //}
            resultModel.data =new AuthorizedLoginUser
            {
                Account_UID = systemUser.Account_UID,
                User_Name = systemUser.User_Name,
                System_Language_UID = systemUser.System_Language_UID,
                Token = userlogintoken,
                MH_Flag = systemUser.MH_Flag,
                IsMulitProject = systemUser.IsMulitProject,
                flowChartMaster_Uid = systemUser.flowChartMaster_Uid,
                USER_Ntid = systemUser.User_NTID,
                RoleList = systemUser.RoleList
            };
            return resultModel;
        }


        private string ReFreshToken(int accountID,string password)
        {
            //refresh token
            FormsAuthenticationTicket ticket =
                    new FormsAuthenticationTicket(1, accountID.ToString(), DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(3), true, password);

            var userlogintoken = FormsAuthentication.Encrypt(ticket);

            //update token in db
            var updateUser = systemService.GetSystemUserByUId(accountID);
            updateUser.LoginToken = userlogintoken;
            settingService.ModifyUser(updateUser);

            return userlogintoken;
        }

        [HttpPost]
        public HttpResponseMessage GetUserProfile(Newtonsoft.Json.Linq.JObject jsonData)
        {
            string ntid = jsonData.Value<string>("ntid");
            var _user = commonService.GetSystemUserByNTId(ntid, 1);
            // check user is exists
            if (_user == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            // check user is disabled
            if (_user.Enable_Flag == false)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            // update last login time
            systemService.updateLastLoginDate(_user.Account_UID);
            // response user
            return Request.CreateResponse(new AuthorizedLoginUser
            {
                Account_UID = _user.Account_UID,
                User_Name = _user.User_Name,
                System_Language_UID = _user.System_Language_UID,
                Token = string.Empty,
                MH_Flag = _user.MH_Flag,
                IsMulitProject = _user.IsMulitProject,
                flowChartMaster_Uid = _user.flowChartMaster_Uid,
                USER_Ntid = _user.User_NTID
            });
        }
    }
}
