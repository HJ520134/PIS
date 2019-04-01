using Newtonsoft.Json;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace PDMS.Web.Controllers.PlayBoard
{
    public class PlayBoardController : WebControllerBase
    {
        public int GetPlantOrgUid()
        {
            int plantorguid = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                plantorguid = (int)CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
            return plantorguid;
        }

        /// <summary>
        /// 播放画面
        /// </summary>
        /// <param name="playUserUID"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index(int playUserUID)
        {
            try
            {
                //自动登录
                if (CurrentUser.GetUserInfo == null)
                {
                    var resultModel = SignInWithoutPassword(playUserUID);
                    if (!resultModel.isSuccess)
                    {
                        //playUserUID 登录失败(账号不存在或不存在播放角色)，则跳转到主画面
                        return RedirectToAction("Index", "Home");
                    }
                }
                //this.Response.Clear();//这里是关键，清除在返回前已经设置好的标头信息，这样后面的跳转才不会报错
                //var cooike = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.PlayBoard);
                //if (cooike != null)
                //{
                //    cooike["playUserUID"] = playUserUID.ToString();
                //}
                //else
                //{
                //    var userCookie = new HttpCookie("PlayBoard");
                //    userCookie["playUserUID"] = playUserUID.ToString();
                //    userCookie.Expires.AddDays(365);
                //    //HttpContext.Response.Cookies.Add(userCookie);
                //    HttpContext.Response.SetCookie(userCookie);
                //}

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }

            FixtureVM currentVM = new FixtureVM();
            var userUID = CurrentUser.AccountUId;
            //if (playUserUID.HasValue)
            //{
            //    userUID = playUserUID.Value;
            //}
            //通过ViewBag 把播放账号UID 传到前端
            ViewBag.PlayUserUID = playUserUID;

            return View("Index", currentVM);
        }

        /// <summary>
        /// 唯一的用途测试程式是否OK，如果在发布肯定会超时
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult IsHealth()
        {
            var resultModel = new ApiResultModel() { isSuccess = true };
            return Content(JsonConvert.SerializeObject(resultModel), "application/json"); ;
        }

        private ApiResultModel SignInWithoutPassword(int uid)
        {
            //LoginUserMoel loginUser = new LoginUserMoel() { UserName = userName };
            HttpResponseMessage responMessage;
            var apiPath = ConfigurationManager.AppSettings["WebApiPath"].ToString();
            //var content = new StringContent(JsonConvert.SerializeObject(loginUser));
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //var contentString = JsonConvert.SerializeObject(new {uid=uid });
            //var content = new StringContent(contentString);

            //var content = new StringContent(JsonConvert.SerializeObject(new { uid = uid }));
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //responMessage = client.PostAsync(apiPath + "Login/LoginInWithoutPassword", content).Result;

            var json = JsonConvert.SerializeObject(new { uid = uid });
            var apiInsertMachineDUrl = string.Format("Login/LoginInWithoutPassword?uid={0}&roleID={1}", uid, "PlayBoardPlayUser");
            responMessage = APIHelper.APIGetAsync(apiInsertMachineDUrl);

            var jsonResult = responMessage.Content.ReadAsStringAsync().Result;
            
            var resultModel = JsonConvert.DeserializeObject<ApiResultModel>(jsonResult);
            
            if (resultModel.isSuccess)
            {
                var user = JsonConvert.DeserializeObject<AuthorizedLoginUser>(JsonConvert.SerializeObject(resultModel.data));
                //var user = JsonConvert.DeserializeObject<AuthorizedLoginUser>(responMessage.Content.ReadAsStringAsync().Result);

                if (Request.Cookies[SessionConstants.LoginTicket] != null)
                {
                    CookiesHelper.RemoveCookiesByCookieskey(Request, Response, SessionConstants.LoginTicket);
                    CookiesHelper.RemoveCookiesByCookieskey(Request, Response, SessionConstants.CurrentAccountUID);
                    CookiesHelper.RemoveCookiesByCookieskey(Request, Response, SessionConstants.CurrentUserName);
                }
                else
                {
                    CookiesHelper.AddCookies(Response, SessionConstants.LoginTicket, user.Token, 1);
                    CookiesHelper.AddCookies(Response, SessionConstants.CurrentAccountUID, user.Account_UID.ToString(), 1);
                    CookiesHelper.AddCookies(Response, SessionConstants.CurrentUserName, user.User_Name, 1);
                }


                if (Request.Cookies["APIPath"] == null)
                {
                    CookiesHelper.AddCookies(Response, "APIPath", apiPath, 1);
                }

                FormsAuthentication.SetAuthCookie(user.User_Name, false);

                SetLogon(user);
            }
            return resultModel;
        }

        private void SetLogon(AuthorizedLoginUser user)
        {
            Session[SessionConstants.LoginTicket] = user.Token;
            Session[SessionConstants.CurrentAccountUID] = user.Account_UID.ToString();
            Session[SessionConstants.CurrentUserName] = user.User_Name;
            Session[SessionConstants.CurrentLanguageId] = user.System_Language_UID.ToString();

            // Session[SessionConstants.CurrentUserInfo] = AAresult;
            if (user.USER_Ntid.ToUpper() != "EQPUSER")
            {
                var userApiUrl = string.Format("Common/GetUserInfo/?uid={0}", user.Account_UID);
                var AAresponMessage = APIHelper.APIGetAsync(userApiUrl);
                var AAresult = JsonConvert.DeserializeObject<CustomUserInfoVM>(AAresponMessage.Content.ReadAsStringAsync().Result);

                Session[SessionConstants.CurrentUserInfo] = AAresult;
                string apiUrl = string.Format("Common/GetLanguagesAPI?Language_UID={0}", user.System_Language_UID);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var languageVM = JsonConvert.DeserializeObject<LanguageVM>(result);

                string userUrl = string.Format("Common/GetUserInfo/?uid={0}", user.Account_UID);
                var userResponMessage = APIHelper.APIGetAsync(userUrl);
                var userResult = userResponMessage.Content.ReadAsStringAsync().Result;

                var userDTO = JsonConvert.DeserializeObject<CustomUserInfoVM>(userResult);

                PISSessionContext.Current.CurrentWorkingLanguage = languageVM.CurrentLanguage;
                PISSessionContext.Current.CurrentUser = userDTO;
                PISSessionContext.Current.Languages = languageVM.Languages;
                var userCookie = new HttpCookie(SessionConstants.UserSelect);
                userCookie[SessionConstants.CurrentAccountUID] = PISSessionContext.Current.CurrentUser.Account_UID.ToString();
                userCookie[SessionConstants.CurrentLanguageId] = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID.ToString();
                userCookie.Expires.AddDays(365);
                HttpContext.Response.SetCookie(userCookie);

                //用于传值给前台JS
                Session[SessionConstants.CurrentLanguageId] = languageVM.CurrentLanguage.System_Language_UID;

            }
            else
            {
                var customUserInfoVM = new CustomUserInfoVM()
                {
                    Account_UID = user.Account_UID,
                    Language_UID = 2,
                    User_NTID = user.USER_Ntid,
                    User_Name = user.User_Name,
                    MH_Flag = user.MH_Flag,
                    LoginToken = user.Token

                };
                Session[SessionConstants.CurrentUserInfo] = customUserInfoVM;
                string apiUrl = string.Format("Common/GetLanguagesAPI?Language_UID={0}", 2);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var languageVM = JsonConvert.DeserializeObject<LanguageVM>(result);
                PISSessionContext.Current.CurrentWorkingLanguage = languageVM.CurrentLanguage;
                PISSessionContext.Current.Languages = languageVM.Languages;
                //用于传值给前台JS
                Session[SessionConstants.CurrentLanguageId] = 2;

                // JsonConvert.DeserializeObject<CustomUserInfoVM>(customUserInfoVM)
            }
        }

        /// <summary>
        /// 通过Cookie 自动登录
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexWithCookie()
        {
            //先判断是否有cookie，有cookie直接转到播放页面，无cookie，直接转换到index页面
            var cooike = System.Web.HttpContext.Current.Request.Cookies.Get("PlayBoard");
            if (cooike != null)
            {
                return RedirectToAction("Index", "PlayBoard", new { playUserUID = cooike["playUserUID"] });
            }
            else
                return RedirectToAction("Index");
        }

        /// <summary>
        /// 设置画面
        /// </summary>
        /// <returns></returns>
        public ActionResult Setting()
        {
            WorkStationVM currentVM = new WorkStationVM();
            int optypeID = 0;
            int funPlantID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                if (CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null)
                {
                    optypeID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }

                if (CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null)
                {
                    funPlantID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                }
                currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
            }
            else
            {
                //获取
                var Plants = new List<PlantVM>();
                var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {
                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            return View("Setting", currentVM);
        }

        /// <summary>
        /// 保存播放设置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public string SaveAddPlayBoardSetting(PlayBoard_SettingDTO dto)
        {
            dto.Created_UID = CurrentUser.AccountUId;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = dto.Created_UID;
            dto.Modified_Date = dto.Created_Date;
            var apiUrl = string.Format("PlayBoard/SaveAddPlayBoardSettingAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostDynamicAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            JsonResult res = new JsonResult();
            return result;
        }

        /// <summary>
        /// 编辑播放设置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public string SaveEditPlayBoardSetting(PlayBoard_SettingDTO dto)
        {
            dto.Created_UID = CurrentUser.AccountUId;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = dto.Created_UID;
            dto.Modified_Date = dto.Created_Date;
            var apiUrl = string.Format("PlayBoard/SaveEditPlayBoardSettingAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostDynamicAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            JsonResult res = new JsonResult();
            return result;
        }


        public ActionResult QueryPlayBoardSettings(PlayBoard_SettingModelSearch search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && organization_UID != 0)
            {
                search.BG_Organization_UID = organization_UID;
            }
            var apiUrl = string.Format("PlayBoard/QueryPlayBoardSettingsAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult QueryPlayBoardSetting(int id)
        {
            var apiUrl = string.Format("PlayBoard/QueryPlayBoardSettingAPI?id={0}", id);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 获取轮流播放画面
        /// </summary>
        /// <param name="playUserUID"></param>
        /// <param name="currentSettingID"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult GetInTurnPartialView(int playUserUID, int currentSettingID, int step)
        {
            //根据播放账号获取播放画面
            var apiUrl = string.Format("PlayBoard/GetEnabledInTurnPlayBoardSettingsByPlayUIDAPI?playUserUID={0}", playUserUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dtoList = JsonConvert.DeserializeObject<List<PlayBoard_SettingDTO>>(result);

            PlayBoard_SettingDTO settingDto = null;
            //应该检查子画面的cshtml 文件是否存在，否则可能根本不存在对应的PartialView
            if (dtoList.Count() > 0)
            {
                //排序
                var orderedDtoList = dtoList.OrderBy(x => x.PlaySeq).ThenBy(x => x.PlayBoard_Setting_ID).ToList();

                //数据表中的PlayBoard_SettingID 是>0的,如果PlayBoard_SettingID<1 或者不存在currentSettingID则播放第一条
                if (currentSettingID < 1 || !orderedDtoList.Exists(x => x.PlayBoard_Setting_ID == currentSettingID))
                {
                    settingDto = orderedDtoList.FirstOrDefault();
                    settingDto.LastViewTitle = orderedDtoList[(orderedDtoList.Count - 1) % orderedDtoList.Count].Title;
                    settingDto.NextViewTiitle = orderedDtoList[1 % orderedDtoList.Count].Title;
                }
                else
                {
                    for (int i = 0; i < orderedDtoList.Count; i++)
                    {
                        if (orderedDtoList[i].PlayBoard_Setting_ID == currentSettingID)
                        {
                            var index = i + step;
                            if (index < 0)
                            {
                                index += orderedDtoList.Count;
                            }
                            settingDto = orderedDtoList[index % orderedDtoList.Count];

                            //带出上一个播放画面title, 下一个播放画面title
                            var lastIndex = index - 1;
                            if (lastIndex<0)
                            {
                                lastIndex += orderedDtoList.Count;
                            }
                            var nextIndex = index + 1;
                            settingDto.LastViewTitle = orderedDtoList[lastIndex % orderedDtoList.Count].Title;
                            settingDto.NextViewTiitle = orderedDtoList[nextIndex % orderedDtoList.Count].Title;
                        }
                    }
                }
            }

            return Content(JsonConvert.SerializeObject(settingDto), "application/json");
        }

        /// <summary>
        /// 获取定时播放画面，让前端轮询确实是否有定时播放的子画面
        /// </summary>
        /// <param name="playUserUID"></param>
        /// <param name="currentSettingID"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult GetTimingPartialView(int playUserUID)
        {
            //自动登录
            if (CurrentUser.GetUserInfo == null)
            {
                var resultModel = SignInWithoutPassword(playUserUID);
                if (!resultModel.isSuccess)
                {
                    //playUserUID 登录失败(账号不存在或不存在播放角色)，则跳转到主画面
                    return RedirectToAction("Index", "Home");
                }
            }

            //根据播放账号获取播放画面
            var apiUrl = string.Format("PlayBoard/GetEnabledTimmingPlayBoardSettingsByPlayUIDAPI?playUserUID={0}", playUserUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 根据[PlayBoard_Setting].[PlayBoard_Setting_ID] 返回子画面
        /// </summary>
        /// <param name="settingID">[PlayBoard_Setting].[PlayBoard_Setting_ID]</param>
        /// <returns></returns>
        [AllowAnonymous]
        public PartialViewResult ShowPartialView(int settingID)
        {
            //string HostName = string.Empty;
            //string ip = string.Empty;
            //string ipv4 = String.Empty;

            //if (null == System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"])
            //{
            //    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}
            //else
            //{
            //    ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //}

            //foreach (IPAddress ipAddr in Dns.GetHostEntry(ip).AddressList)
            //{
            //    if (ipAddr.AddressFamily.ToString() == "InterNetwork")
            //    {
            //        ipv4 = ipAddr.ToString();
            //    }
            //}
            //HostName = "主机名: " + Dns.GetHostEntry(ip).HostName + " IP: " + ipv4;
            

            //通过currentSettingID 找到要播放的画面的ActionName
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);
            //用ViewBage 传递参数
            ViewBag.SettingID = settingID;
            ViewBag.JsonParameter = dto.JsonParameter;
            ViewBag.Play_UID = dto.Play_UID;
            string actionName = dto.ActionName;
            return PartialView(actionName, settingID);
        }

        /// <summary>
        /// 播放看板跳转到子画面
        /// </summary>
        /// <param name="playUserUID"></param>
        /// <param name="currentView"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        //[ChildActionOnly]
        [AllowAnonymous]
        public PartialViewResult ChildAction(int playUserUID, string currentView, int step)
        {
            //根据播放账号获取播放画面
            var apiUrl = string.Format("PlayBoard/GetEnabledPlayBoardSettingsByPlayUIDAPI?playUserUID={0}", playUserUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dtoList = JsonConvert.DeserializeObject<List<PlayBoard_SettingDTO>>(result);
            var orderedDtoList = dtoList.OrderBy(x => x.PlaySeq).ToList();
            string viewName = null;
            //应该检查子画面的cshtml 文件是否存在，否则可能根本不存在对应的PartialView
            if (orderedDtoList.Count() > 0)
            {
                if (string.IsNullOrWhiteSpace(currentView))
                {
                    var dto = orderedDtoList.FirstOrDefault();
                    viewName = dto.ActionName;
                }
                else
                {
                    for (int i = 0; i < orderedDtoList.Count; i++)
                    {
                        if (orderedDtoList[i].ActionName == currentView)
                        {
                            viewName = orderedDtoList[i + step].ActionName;
                        }
                    }
                }
            }
            return PartialView(viewName);
        }

        public ActionResult DeletePlayBoardSetting(int settingID)
        {
            var apiUrl = string.Format("PlayBoard/DeletePlayBoardSettingAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #region Picture
        /// <summary>
        /// Picture 播放画面
        /// </summary>
        /// <param name="settingID"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult PictureShow(int settingID)
        {
            return View();
        }

        /// <summary>
        /// Picture 配置画面
        /// </summary>
        /// <param name="playUserUID"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult PictureSetting(int settingID)
        {
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            var dirPath = Server.MapPath("~/Upload/PlayBoard/Picture/" + dto.Play_UID);
            //若无文件夹则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var dir = new DirectoryInfo(dirPath);
            var imgVMs = dir.EnumerateFiles("*.*").Where(s => s.Name.ToLower().EndsWith(".jpg") || s.Name.ToLower().EndsWith(".jpeg") || s.Name.ToLower().EndsWith(".png")).Select(f => new PictureVM() { Name = f.Name, FullPath = f.FullName, LastWriteTime = f.LastWriteTime });
            var imgList = imgVMs.OrderBy(x => x.Name).Select(x => x.Name);
            ViewBag.ImgList = imgList;
            ViewBag.SettingID = dto.PlayBoard_Setting_ID;
            ViewBag.Play_UID = dto.Play_UID;
            ViewBag.Play_UserName = dto.Play_UserName;
            ViewBag.Play_UserNTID = dto.Play_UserNTID;
            ViewBag.JsonParameter = dto.JsonParameter;
            return View();
        }

        public ActionResult ImportPictureImg(int settingID, HttpPostedFileBase uploadName)
        {
            var resultModel = new ApiResultModel();
            resultModel.isSuccess = false;

            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            string errorInfo = string.Empty;
            string name = uploadName.FileName;
            string extName = name.Split('.')[1].ToLower();
            var imgFileExtList = new List<string>() { "jpg", "jpeg", "png" };
            if (!imgFileExtList.Contains(extName))
            {
                resultModel.message = "必须是jpg、jpeg、png格式文件，请返回后重新上传！";
            }
            else if (uploadName == null)
            {
                resultModel.message = "没有文件！";
            }
            else
            {
                var dir = Server.MapPath("~/Upload/PlayBoard/Picture/" + dto.Play_UID);
                //若无文件夹则创建
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                //var severURL = dir + Path.GetFileName(uploadName.FileName);//severURL
                var fileName = Path.Combine(dir, Path.GetFileName(uploadName.FileName));//Path.GetFileName(uploadName.FileName)
                try
                {
                    if (System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Delete(fileName);
                    }
                    uploadName.SaveAs(fileName);
                }
                catch (Exception ex)
                {
                    resultModel.message = "上传异常:" + ex.Message;
                }
            }

            resultModel.isSuccess = true;
            return Content(JsonConvert.SerializeObject(resultModel), "application/json");
        }

        public ActionResult QueryPictureImg(int settingID, Page page)
        {
            //存放图片的文件夹以用户ID命名
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            var dirPath = Server.MapPath("~/Upload/PlayBoard/Picture/" + dto.Play_UID);
            //若无文件夹则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var dir = new DirectoryInfo(dirPath);
            var imgVMs = dir.EnumerateFiles("*.*").Where(s => s.Name.ToLower().EndsWith(".jpg") || s.Name.ToLower().EndsWith(".jpeg") || s.Name.ToLower().EndsWith(".png")).Select(f => new PictureVM() { Name = f.Name, FullPath = f.FullName, LastWriteTime = f.LastWriteTime });
            var orderedImgVMs = imgVMs.OrderByDescending(f => f.LastWriteTime).Skip(page.Skip).Take(page.PageSize);
            var ORTImagList = new List<PictureVM>();
            foreach (var item in orderedImgVMs)
            {
                ORTImagList.Add(new PictureVM() { Name = item.Name, FullPath = item.FullPath, LastWriteTime = item.LastWriteTime });
            }
            var pagedImgVMs = new PagedListModel<PictureVM>(imgVMs.Count(), ORTImagList);
            var serializeStr = JsonConvert.SerializeObject(pagedImgVMs);
            return Content(serializeStr, "application/json");
        }
        public string DeletePictureImg(int settingID, string name)
        {
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            var dirPath = Server.MapPath("~/Upload/PlayBoard/Picture/" + dto.Play_UID);
            //若无文件夹则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var fullName = Path.Combine(dirPath, name);
            if (System.IO.File.Exists(fullName))
            {
                System.IO.File.Delete(fullName);
                return "";
            }
            return "不存在此文件";
        }

        public ActionResult SavePictureSetting(int settingID, string fileName)
        {
            var apiUrl = string.Format("PlayBoard/SavePictureSettingAPI?settingID={0}&fileName={1}&modifiedUID={2}", settingID, fileName, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region PPT
        /// <summary>
        /// Picture 播放画面
        /// </summary>
        /// <param name="settingID"></param>
        /// <returns></returns>
        public ActionResult PPTShow(int settingID)
        {
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            ViewBag.SettingID = dto.PlayBoard_Setting_ID;
            ViewBag.Play_UID = dto.Play_UID;

            //使用匿名对象
            var parameterType = new { fileName = "", sheetPlayTime = 0 };
            var parameter = JsonConvert.DeserializeAnonymousType(dto.JsonParameter, parameterType);
            var jsonParameterObject = JsonConvert.DeserializeObject(dto.JsonParameter);
            ViewBag.Play_PPT = parameter.fileName;
            ViewBag.SheetPlayTime = parameter.sheetPlayTime;

            return View();
        }

        /// <summary>
        /// Picture 配置画面
        /// </summary>
        /// <param name="playUserUID"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult PPTSetting(int settingID)
        {
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            var dirPath = Server.MapPath("~/Upload/PlayBoard/PPT/" + dto.Play_UID);
            //若无文件夹则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var dir = new DirectoryInfo(dirPath);
            var htmlFileNameList = dir.EnumerateFiles("*.*").Where(s => s.Name.ToLower().EndsWith(".html")).Select(f => Path.GetFileNameWithoutExtension(f.Name)).ToList();
            var fileVMs = dir.EnumerateFiles("*.*").Where(s => s.Name.ToLower().EndsWith(".pptx")).Select(f => new PPTVM() { Name = f.Name, FullPath = f.FullName, LastWriteTime = f.LastWriteTime });
            var pptFiles = new List<PPTVM>();
            foreach (var item in fileVMs)
            {
                if (htmlFileNameList.Contains(Path.GetFileNameWithoutExtension(item.Name)))
                {
                    pptFiles.Add(item);
                }
            }
            var fileList = pptFiles.OrderBy(x => x.Name).Select(x => x.Name);
            ViewBag.FileList = fileList;
            ViewBag.SettingID = dto.PlayBoard_Setting_ID;
            ViewBag.Play_UID = dto.Play_UID;
            ViewBag.Play_UserName = dto.Play_UserName;
            ViewBag.Play_UserNTID = dto.Play_UserNTID;
            ViewBag.JsonParameter = dto.JsonParameter;

            //使用匿名对象
            if (dto.JsonParameter != null)
            {
                var parameterType = new { fileName = "", sheetPlayTime = 0 };
                var parameter = JsonConvert.DeserializeAnonymousType(dto.JsonParameter, parameterType);
                var jsonParameterObject = JsonConvert.DeserializeObject(dto.JsonParameter);
                ViewBag.Play_PPT = parameter.fileName;
                ViewBag.SheetPlayTime = parameter.sheetPlayTime;
            }
            else
            {
                ViewBag.Play_PPT = "";
                ViewBag.SheetPlayTime = 0;
            }

            return View();
        }

        public string ImportPPTFile(int playUserUID, HttpPostedFileBase uploadName)
        {
            if (uploadName == null)
            {
                return "上传文件为空，请选择要上传的文件!";
            }
            string errorInfo = string.Empty;
            string name = uploadName.FileName;
            string extName = name.Split('.')[1].ToLower();
            var imgFileExtList = new List<string>() { "pptx" };
            if (!imgFileExtList.Contains(extName))
            {
                return "必须是*.pptx格式文件，请返回后重新上传！";
            }
            if (uploadName == null)
            {
                return "没有文件！";
            }

            var dir = Server.MapPath("~/Upload/PlayBoard/PPT/" + playUserUID);
            //若无文件夹则创建
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            //var severURL = dir + Path.GetFileName(uploadName.FileName);//severURL
            var fileName = Path.Combine(dir, Path.GetFileName(uploadName.FileName));//Path.GetFileName(uploadName.FileName)
            try
            {
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }
                uploadName.SaveAs(fileName);
                //转换成同名的html 文件
                PPTHelper.PptToHtml(fileName);
            }
            catch (Exception ex)
            {
                return "上传异常 ！ " + "   " + ex;
            }
            //上传成功后将数据写入数据表
            return "";
        }

        public ActionResult QueryPPTFile(int settingID, Page page)
        {
            //存放图片的文件夹以用户ID命名
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            var dirPath = Server.MapPath("~/Upload/PlayBoard/PPT/" + dto.Play_UID);
            //若无文件夹则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var dir = new DirectoryInfo(dirPath);
            var imgVMs = dir.EnumerateFiles("*.*").Where(s => s.Name.ToLower().EndsWith(".pptx")).Select(f => new PictureVM() { Name = f.Name, FullPath = f.FullName, LastWriteTime = f.LastWriteTime });
            var orderedImgVMs = imgVMs.OrderByDescending(f => f.LastWriteTime).Skip(page.Skip).Take(page.PageSize);
            var ORTImagList = new List<PPTVM>();
            foreach (var item in orderedImgVMs)
            {
                //显示转换成功的html 文件
                string convertedHtmlFileName = null;
                var htmlFileName = Path.GetFileNameWithoutExtension(item.Name) + ".html";
                var htmlFile = dir.EnumerateFiles(htmlFileName).FirstOrDefault();
                if (htmlFile != null)
                {
                    convertedHtmlFileName = htmlFileName;
                }
                ORTImagList.Add(new PPTVM() { Name = item.Name, FullPath = item.FullPath, LastWriteTime = item.LastWriteTime, HtmlName = convertedHtmlFileName });
            }
            var pagedImgVMs = new PagedListModel<PPTVM>(imgVMs.Count(), ORTImagList);
            var serializeStr = JsonConvert.SerializeObject(pagedImgVMs);
            return Content(serializeStr, "application/json");
        }
        public string DeletePPTFile(int settingID, string name)
        {
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            var dirPath = Server.MapPath("~/Upload/PlayBoard/PPT/" + dto.Play_UID);
            //若无文件夹则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var fullName = Path.Combine(dirPath, name);
            var htmlFullName = Path.Combine(dirPath, Path.GetFileNameWithoutExtension(name) + ".html");
            if (System.IO.File.Exists(fullName))
            {
                System.IO.File.Delete(fullName);
                if (System.IO.File.Exists(htmlFullName))
                {
                    System.IO.File.Delete(htmlFullName);
                }
                return "";
            }
            return "不存在此文件";
        }

        public ActionResult SavePPTSetting(int settingID, string fileName, int sheetPlayTime)
        {
            var apiUrl = string.Format("PlayBoard/SavePPTSettingAPI?settingID={0}&fileName={1}&sheetPlayTime={2}&modifiedUID={3}", settingID, fileName, sheetPlayTime, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetPlayUsersByOPType(int opTypeUID, int? funcOrgID)
        {
            var apiUrl = string.Format("PlayBoard/GetPlayUsersByOPTypeAPI?opTypeUID={0}&funcOrgID={1}", opTypeUID, funcOrgID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetPlayBoardViewList()
        {
            var apiUrl = string.Format("PlayBoard/GetPlayBoardViewListAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region EBoardShow
        [AllowAnonymous]
        public ActionResult EBoardShowSetting(int settingID)
        {
            #region 保留旧版电子看板代码
            string OpType = "OP2";
            List<OrganiztionVM> userinfo = CurrentUser.GetUserInfo.OrgInfo;
            foreach (OrganiztionVM item in userinfo)
            {
                ViewBag.plant = item.Plant;
                OpType = item.OPType;
            }
            ViewBag.Opty = OpType;
            var apiUrl = string.Format("EventReportManager/GetProjectByOpAPI/?Op_Type={0}", OpType);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<string> Projects = js.Deserialize<List<string>>(result);
            ViewBag.Projects = Projects;

            apiUrl = string.Format("Equipmentmaintenance/GetEBoardLocationAPI/?optype={0}", OpType);
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            List<string> loctions = js.Deserialize<List<string>>(result);
            ViewBag.loctions = loctions;

            #endregion

            var apiUrl_PlayBoard = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage_PlayBoard = APIHelper.APIGetAsync(apiUrl_PlayBoard);
            var result_PlayBoard = responMessage_PlayBoard.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result_PlayBoard);

            ViewBag.SettingID = dto.PlayBoard_Setting_ID;
            ViewBag.Play_UID = dto.Play_UID;
            ViewBag.Play_UserName = dto.Play_UserName;
            ViewBag.Play_UserNTID = dto.Play_UserNTID;

            return View();
        }


        //电子看板选择Part
        [AllowAnonymous]
        public ActionResult EBoardShowSettingSelectPart(string selectProjects, string opType, string location, string plant, int settingID)
        {
            ViewBag.selectProjects = selectProjects;
            ViewBag.opType = opType;
            ViewBag.location = location;
            ViewBag.plant = plant;
            //获取所有功能厂
            var apiUrl = string.Format("Equipmentmaintenance/GetFunPlantsAPI/?selectProjects={0}&opType={1}", selectProjects, opType);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<string> funplants = js.Deserialize<List<string>>(result);
            ViewBag.funplants = funplants;
            ViewBag.SettingID = settingID;
            return View();
        }

        public string SaveSettingParameter(int settingID, string parameter)
        {
            var dynamicModel = new { settingID = settingID, parameter = parameter, modifiedUID = CurrentUser.AccountUId };
            var apiUrl = string.Format("PlayBoard/SaveSettingParameterAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostDynamicAsync(dynamicModel, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            JsonResult res = new JsonResult();
            return result;
        }

        [AllowAnonymous]
        public ActionResult EBoardShow(int settingID)
        {
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            ViewBag.SettingID = dto.PlayBoard_Setting_ID;
            ViewBag.EBoardShowUrl = dto.JsonParameter;

            return View();
        }

        public ActionResult VideoShow(int settingID)
        {
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            ViewBag.SettingID = dto.PlayBoard_Setting_ID;
            ViewBag.Play_UID = dto.Play_UID;

            //使用匿名对象
            //var parameterType = new { fileName = "", sheetPlayTime = 0 };
            //var parameter = JsonConvert.DeserializeAnonymousType(dto.JsonParameter, parameterType);
            //var fileNameList = JsonConvert.DeserializeObject<string[]>(dto.JsonParameter).ToList();
            //ViewBag.Play_Video = parameter.fileName;
            //ViewBag.SheetPlayTime = parameter.sheetPlayTime;
            //ViewBag.FileNameList = fileNameList;
            return View();
        }

        public ActionResult GetVideos(int settingID)
        {
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            //ViewBag.SettingID = dto.PlayBoard_Setting_ID;
            //ViewBag.Play_UID = dto.Play_UID;

            //使用匿名对象
            //var parameterType = new { fileName = "", sheetPlayTime = 0 };
            //var parameter = JsonConvert.DeserializeAnonymousType(dto.JsonParameter, parameterType);
            //var fileNameList = JsonConvert.DeserializeObject<string[]>(dto.JsonParameter).ToList();
            //ViewBag.Play_Video = parameter.fileName;
            //ViewBag.SheetPlayTime = parameter.sheetPlayTime;
            //ViewBag.FileNameList = fileNameList;
            return Content(dto.JsonParameter, "application/json"); ;
        }

        /// <summary>
        /// Picture 配置画面
        /// </summary>
        /// <param name="playUserUID"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult VideoSetting(int settingID)
        {
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            var dirPath = Server.MapPath("~/Upload/PlayBoard/Video/" + dto.Play_UID);
            //若无文件夹则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var dir = new DirectoryInfo(dirPath);
            //沿用PictureVM
            var imgVMs = dir.EnumerateFiles("*.*").Where(s => s.Name.ToLower().EndsWith(".mp4")).Select(f => new PictureVM() { Name = f.Name, FullPath = f.FullName, LastWriteTime = f.LastWriteTime });
            var imgList = imgVMs.OrderBy(x => x.Name).Select(x => x.Name);
            ViewBag.ImgList = imgList;
            ViewBag.SettingID = dto.PlayBoard_Setting_ID;
            ViewBag.Play_UID = dto.Play_UID;
            ViewBag.Play_UserName = dto.Play_UserName;
            ViewBag.Play_UserNTID = dto.Play_UserNTID;
            if (dto.JsonParameter != null)
            {
                ViewBag.FileNameList = JsonConvert.DeserializeObject<string[]>(dto.JsonParameter).ToList();
            }
            return View();
        }

        public ActionResult ImportVideoFile(int settingID, HttpPostedFileBase uploadName)
        {
            var resultModel = new ApiResultModel();
            resultModel.isSuccess = false;

            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            string errorInfo = string.Empty;
            string name = uploadName.FileName;
            string extName = name.Split('.')[1].ToLower();
            var imgFileExtList = new List<string>() { "mp4"};
            if (!imgFileExtList.Contains(extName))
            {
                resultModel.message = "必须是mp4格式文件，请返回后重新上传！";
            }
            else if (uploadName == null)
            {
                resultModel.message = "没有文件！";
            }
            else
            {
                var dir = Server.MapPath("~/Upload/PlayBoard/Video/" + dto.Play_UID);
                //若无文件夹则创建
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                //var severURL = dir + Path.GetFileName(uploadName.FileName);//severURL
                var fileName = Path.Combine(dir, Path.GetFileName(uploadName.FileName));//Path.GetFileName(uploadName.FileName)
                try
                {
                    if (System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Delete(fileName);
                    }
                    uploadName.SaveAs(fileName);
                }
                catch (Exception ex)
                {
                    resultModel.message = "上传异常:" + ex.Message;
                }
            }

            resultModel.isSuccess = true;
            return Content(JsonConvert.SerializeObject(resultModel), "application/json");
        }

        public ActionResult QueryVideoFile(int settingID, Page page)
        {
            //存放图片的文件夹以用户ID命名
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            var dirPath = Server.MapPath("~/Upload/PlayBoard/Video/" + dto.Play_UID);
            //若无文件夹则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var dir = new DirectoryInfo(dirPath);
            var imgVMs = dir.EnumerateFiles("*.*").Where(s => s.Name.ToLower().EndsWith(".mp4")).Select(f => new PictureVM() { Name = f.Name, FullPath = f.FullName, LastWriteTime = f.LastWriteTime, SizeMB = ((int) Math.Ceiling(f.Length/1024.0/1024.0)).ToString()+" MB" });
            var orderedImgVMs = imgVMs.OrderByDescending(f => f.LastWriteTime).Skip(page.Skip).Take(page.PageSize);
            var ORTImagList = new List<PictureVM>();
            foreach (var item in orderedImgVMs)
            {
                ORTImagList.Add(new PictureVM() { Name = item.Name, FullPath = item.FullPath, LastWriteTime = item.LastWriteTime, SizeMB = item.SizeMB });
            }
            var pagedImgVMs = new PagedListModel<PictureVM>(imgVMs.Count(), ORTImagList.OrderBy(x => x.Name));
            var serializeStr = JsonConvert.SerializeObject(pagedImgVMs);
            return Content(serializeStr, "application/json");
        }
        public string DeleteVideoFile(int settingID, string name)
        {
            var apiUrl = string.Format("PlayBoard/GetPlayBoardSettingByIDAPI?settingID={0}", settingID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(result);

            var dirPath = Server.MapPath("~/Upload/PlayBoard/Video/" + dto.Play_UID);
            //若无文件夹则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var fullName = Path.Combine(dirPath, name);
            if (System.IO.File.Exists(fullName))
            {
                System.IO.File.Delete(fullName);
                if (System.IO.File.Exists(fullName))
                {
                    System.IO.File.Delete(fullName);
                }
                return "";
            }
            return "不存在此文件";
        }
        
        public ActionResult SaveVideoSetting(int settingID, string[] fileName)
        {
            var fileNameJson = JsonConvert.SerializeObject(fileName);
            var apiUrl = string.Format("PlayBoard/SaveVideoSettingAPI?settingID={0}&fileName={1}&modifiedUID={2}", settingID, fileNameJson, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion
    }
}