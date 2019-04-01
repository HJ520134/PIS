using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Mvc;
using PDMS.Model.ViewModels.Settings;
using System.Web.Helpers;
using System.Net;
using System.Web.Script.Serialization;
using PDMS.Core.Authentication;
using PDMS.Core.Filters;
using System.Web;
using System.Web.SessionState;
using PDMS.Model.EntityDTO;
using System.Drawing;
using System.Diagnostics;
using System.Linq;

namespace PDMS.Web.Controllers
{
    public class BoardController : WebControllerBase
    {
        // GET: Board

        public ActionResult Index()
        {

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

            return View();
        }

        public ActionResult IndexWithCookie()
        {
            //先判断是否有cookie，有cookie直接转到播放页面，无cookie，直接转换到index页面
            var cooike = System.Web.HttpContext.Current.Request.Cookies.Get("UserSelect");
            if (cooike != null)
            {
                return RedirectToAction("EBoardShow", "Board", new { opType = cooike["opType"], selectProjects = cooike["selectProjects"], selectFunplants = cooike["selectFunplants"], location = cooike["location"] });
            }
            else
                return RedirectToAction("Index");
        }
        public string ImportMovie(HttpPostedFileBase uploadName, DateTime Start_Time, DateTime End_Time, string Scope, int? RepeatTime)
        {
            string errorInfo = string.Empty;
            string name = uploadName.FileName;
            string extName = name.Split('.')[1].ToLower();
            if (extName != "mp4")
            {
                return "必须是mp4格式文件，请返回后重传！";
            }
            if (uploadName == null)
            {
                return "没有文件！";
            }

            var severURL = "~/Movie/" + Path.GetFileName(uploadName.FileName);//severURL
            var fileName = Path.Combine(Request.MapPath("~/Movie"), Path.GetFileName(uploadName.FileName));
            try
            {

                uploadName.SaveAs(fileName);

            }
            catch (Exception ex)
            {
                return "上传异常 ！ " + "   " + ex;
            }

            //上传成功后将数据写入数据表
            var apiUrl = "Chart/AddNoticeAPI";
            var entity = new NoticeDTO();
            entity.Start_Time = Start_Time;
            entity.End_Time = End_Time;
            entity.Creator_UID = CurrentUser.AccountUId;
            entity.Creat_Time = DateTime.Now;
            if (DateTime.Compare(entity.Creat_Time, entity.Start_Time) < 0)
            {
                entity.State = "未开始";
            }

            else if (DateTime.Compare(entity.Creat_Time, entity.Start_Time) >= 0 && DateTime.Compare(entity.Creat_Time, entity.End_Time) <= 0)
            {
                entity.State = "进行中";
            }
            else if (DateTime.Compare(entity.Creat_Time, entity.End_Time) > 0)
            {
                entity.State = "已过时";
            }
            entity.Color = "视频";
            entity.Notice_Content = "../Movie/" + Path.GetFileName(uploadName.FileName) + "$" + fileName;
            entity.Scope = Scope;

            entity.RepeatTime = RepeatTime;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return "上传成功";
        }

        public ActionResult SelectPart(string selectProjects, string opType, string location, string plant)
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
            return View();
        }
        public ActionResult Notice()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult EBoardShow(string opType, string selectProjects, string selectFunplants, string location)
        {
            var cooike = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.UserSelect);
            if (cooike != null)
            {
                cooike["opType"] = opType;
                cooike["selectProjects"] = selectProjects;
                cooike["selectFunplants"] = selectFunplants;
                cooike["location"] = location;
                cooike["userName"] = CurrentUser.UserName;
            }
            else
            {
                var userCookie = new HttpCookie("UserSelect");
                userCookie["opType"] = opType;
                userCookie["selectProjects"] = selectProjects;
                userCookie["selectFunplants"] = selectFunplants;
                userCookie["location"] = location;
                userCookie["userName"] = CurrentUser.UserName;
                userCookie[SessionConstants.CurrentAccountUID] = CurrentUser.AccountUId.ToString();
                userCookie[SessionConstants.CurrentLanguageId] = CurrentUser.Language_UID.ToString();
                userCookie.Expires.AddDays(365);
                HttpContext.Response.SetCookie(userCookie);

            }

            var apiUrl = "Chart/GetPageSizeAPI";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.ToString();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string Size = js.Deserialize<string>(result);
            if (string.IsNullOrWhiteSpace(Size))
            {
                ViewBag.PageSize = 13;
            }
            else
                ViewBag.PageSize = int.Parse(Size);
            ViewBag.Projects = selectProjects;
            ViewBag.Parts = selectFunplants;
            ViewBag.opType = opType;
            //if(location=="/")
            //{
            //    apiUrl = string.Format("Equipmentmaintenance/GetEBoardLocationAPI/?optype={0}", opType);
            //    responMessage = APIHelper.APIGetAsync(apiUrl);
            //    result = responMessage.Content.ReadAsStringAsync().Result;
            //    List<string> loctions = js.Deserialize<List<string>>(result);
            //    string s = string.Empty;
            //    foreach (var item in loctions)
            //    {
            //        s += item+',';
            //    }
            //    ViewBag.location = s;
            //}
            //else
            ViewBag.location = location;
            return View();
        }

        [SPPWebAuthorize]
        [UnauthorizedError]
        public CurrentUserDataPermission getDP()
        {
            if (Session[SessionConstants.DataPermissions] == null)
            {
                var cookie = System.Web.HttpContext.Current.Request.Cookies[SessionConstants.DataPermissions];

                if (cookie != null)
                {
                    Session[SessionConstants.DataPermissions] = cookie.Value;
                }
                else
                {
                    var apiUrl = string.Format("Common/GetPermissonsByNTId/?uid={0}", CurrentUser.AccountUId);
                    var responMessage = APIHelper.APIGetAsync(apiUrl);
                    var result = JsonConvert.DeserializeObject<CurrentUserDataPermission>(responMessage.Content.ReadAsStringAsync().Result);

                    Session[SessionConstants.DataPermissions] = result;
                }
            }
            return Session[SessionConstants.DataPermissions] as CurrentUserDataPermission;
        }



        public ActionResult getPartTypes(string project)
        {

            var apiUrl = string.Format("Chart/getPartTypesAPI?project={0}", project);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult getNoticeContent(string optype)
        {
            var apiUrl = string.Format("Chart/getNoticeContentAPI?optype={0}", optype);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.ToString();
            return Content(result, "application/json");
        }

        public ActionResult getMovieUrl(string optype, int CurrentLocation)
        {
            var apiUrl = string.Format("Chart/getMovieUrl?optype={0}&CurrentLocation={1}", optype, CurrentLocation);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.ToString();
            return Content(result, "application/json");
        }


        public ActionResult GetOptypes()
        {

            List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;
            //   string result =  Orgnizations[0].OPType;
            List<string> dto = new List<string>();
            foreach (var item in Orgnizations)
            {
                dto.Add(item.OPType);
            }
            string result = JsonConvert.SerializeObject(dto);
            //var apiUrl = string.Format("Common/GetOptysByNTId/?uid={0}", CurrentUser.AccountUId);
            //var responMessage = APIHelper.APIGetAsync(apiUrl);
            //var result = responMessage.Content.ReadAsStringAsync().Result.ToString();
            return Content(result, "application/json");
        }
        public ActionResult QueryNotice(NoticeSearch search, Page page)
        {
            var apiUrl = "Chart/QueryNoticeAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        [SPPWebAuthorize]
        [UnauthorizedError]
        public ActionResult AddNotice(string jsonAddNotice)
        {

            var apiUrl = "Chart/AddNoticeAPI";
            var entity = JsonConvert.DeserializeObject<NoticeDTO>(jsonAddNotice);
            if (DateTime.Compare(entity.Creat_Time, entity.Start_Time) < 0)
            {
                entity.State = "未开始";
            }

            else if (DateTime.Compare(entity.Creat_Time, entity.Start_Time) >= 0 && DateTime.Compare(entity.Creat_Time, entity.End_Time) <= 0)
            {
                entity.State = "进行中";
            }
            else if (DateTime.Compare(entity.Creat_Time, entity.End_Time) > 0)
            {
                entity.State = "已过时";
            }
            entity.Color = "通知";
            entity.Creator_UID = CurrentUser.AccountUId;
            entity.Creat_Time = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }


        public ActionResult QueryOptype()
        {
            var dPer = getDP();
            var Optype = dPer.Op_Types.ToString();

            return Content(Optype, "application/json");
        }

        public ActionResult DeleteNotice(int uid)
        {

            var apiUrl = string.Format("Chart/DeleteNoticeAPI?uuid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult getShowContent(EboardSearchModel search, Page page)
        {

            var apiUrl = "Chart/getShowContentAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFinnalYield(string Projects)
        {

            var apiUrl = string.Format("Chart/GetFinnalYieldAPI?Projects={0}", Projects);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.ToString();
            return Content(result, "application/json");
        }
        public ActionResult GetPageSize()
        {
            var apiUrl = "Chart/GetPageSizeAPI";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.ToString();
            return Content(result, "application/json");
        }

        public ActionResult GetEQPBoard(EQPRepairInfoDTO search, Page page)
        {
            var apiUrl = string.Format("Chart/GetEQPBoardAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 获取电子看板
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetTopTenQeboardInfo(EboardSearchModel search, Page page)
        {
            page.PageSize = 10;
            var selectProjectsArray = search.selectProjects.Split('_');
            var apiUrl = string.Format("Chart/GetTopTenQeboardAPI?Projects={0}&PageNumber={1}&PageSize={2}", selectProjectsArray[0], page.PageNumber, page.PageSize);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetTopTenQeboardTotalData(string selectProjects, Page page)
        {
            page.PageSize = 10;
            var selectProjectsArray = selectProjects.Split('_');
            var apiUrl = string.Format("Chart/GetNotReachRateInfoData?Projects={0}&PageNumber={1}&PageSize={2}", selectProjectsArray[0], page.PageNumber, page.PageSize);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<TopTenQeboardModel>(result);
            return Content(result, "application/json");
        }

        public ActionResult GetQEboardSumDetailData(string selectProjects, Page page)
        {
          
            var selectProjectsArray = selectProjects.Split('_');
            var apiUrl = string.Format("Chart/GetQEboardSumTotalData?Projects={0}&PageNumber={1}&PageSize={2}", selectProjectsArray[0], page.PageNumber, page.PageSize);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<QEboardSumModel>(result);
            return Content(result, "application/json");
        }

        public ActionResult StaticQEBordAndQETopTen()
        {
            return View();
        }

        /// <summary>
        /// 获取品质报表汇总数据
        /// </summary>
        public ActionResult GetStaticQESumData(string ProjectName, string dataTime)
        {
            dataTime = Convert.ToDateTime(dataTime).ToString("d");
            var apiUrl = string.Format("Chart/GetStaticQESumData?projectName={0}&dataTime={1}", ProjectName, dataTime);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<QEboardSumModel>(result);
            return Content(result, "application/json");
        }

        /// <summary>
        /// 获取品质报表汇总数据
        /// </summary>
        public ActionResult GetStaticQETopTenData(string ProjectName, string dataTime)
        {
            dataTime= Convert.ToDateTime(dataTime).ToString("d");
            var apiUrl = string.Format("Chart/GetStaticQETopTenData?projectName={0}&dataTime={1}", ProjectName, dataTime);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<TopTenQeboardModel>(result);
            return Content(result, "application/json");
        }
        public ActionResult ORTImg(string buildingName)
        {
            var dirPath = Server.MapPath("~/Upload/ORTImg");
            var dir = new DirectoryInfo(dirPath);
            FileInfo img = null;
            var imgs = dir.EnumerateFiles("*.*").Where(s => s.Name.StartsWith(buildingName + "_") && (s.Name.ToLower().EndsWith(".jpg") || s.Name.ToLower().EndsWith(".jpeg") || s.Name.ToLower().EndsWith(".png")));//*.jpg|*.jpeg|*.png
            if (imgs.Count()==0)
            {
                img=dir.EnumerateFiles("*.*").Where(s => s.Name== buildingName+".jpg" || s.Name== buildingName + ".jpeg"|| s.Name == buildingName + ".png").FirstOrDefault();//*.jpg|*.jpeg|*.png
            }
            else
            {
                img = imgs.OrderByDescending(f => f.CreationTime).FirstOrDefault();
            }
            if (img!=null)
            {
                var imgType = "image/jpeg";
                if (img.Extension==".png")
                {
                    imgType = "image/png";
                }
                return File(img.FullName, imgType);
            }
            else
            {
                var defImg = Path.Combine(dirPath, "default.png");
                return File(defImg, "image/png");
            }
        }

        /// <summary>
        /// 根据楼栋名称获取图片文件名
        /// </summary>
        /// <param name="buildingName"></param>
        /// <returns></returns>
        public string GetORTImgByBuildingName(string buildingName)
        {
            var dirPath = Server.MapPath("~/Upload/ORTImg");
            //若无文件夹则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var dir = new DirectoryInfo(dirPath);
            FileInfo img = null;
            var imgs = dir.EnumerateFiles("*.*").Where(f=>f.Name== buildingName+".png" || f.Name == buildingName + ".PNG" || f.Name == buildingName + ".jpg" || f.Name == buildingName + ".JPG" || f.Name == buildingName + ".jpeg" || f.Name == buildingName + ".JPEG");
            img = imgs.OrderByDescending(f => f.CreationTime).FirstOrDefault();
            //var imgs = dir.EnumerateFiles("*.*").Where(s => s.Name.StartsWith(buildingName + "_") && (s.Name.ToLower().EndsWith(".jpg") || s.Name.ToLower().EndsWith(".jpeg") || s.Name.ToLower().EndsWith(".png")));//*.jpg|*.jpeg|*.png
            //if (imgs.Count() == 0)
            //{
            //    img = dir.EnumerateFiles("*.*").Where(s => s.Name.ToLower() == buildingName + ".jpg" || s.Name.ToLower() == buildingName + ".jpeg" || s.Name.ToLower() == buildingName + ".png").FirstOrDefault();//*.jpg|*.jpeg|*.png
            //}
            //else
            //{
            //    img = imgs.OrderByDescending(f => f.CreationTime).FirstOrDefault();
            //}
            if (img != null)
            {
                return img.Name;
            }
            else
            {
                return "";
            }
        }

        public ActionResult ORTImgSetting()
        {
            return View();
        }
        
        public string ImportORTImg(HttpPostedFileBase uploadName)
        {
            string errorInfo = string.Empty;
            string name = uploadName.FileName;
            string extName = name.Split('.')[1].ToLower();
            var imgFileExtList = new List<string>() { "jpg", "jpeg", "png" };
            if (!imgFileExtList.Contains( extName))
            {
                return "必须是jpg、jpeg、png格式文件，请返回后重传！";
            }
            if (uploadName == null)
            {
                return "没有文件！";
            }

            var dir = Server.MapPath("~/Upload/ORTImg/");
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
                return "上传异常 ！ " + "   " + ex;
            }
            //上传成功后将数据写入数据表
            return "";
        }

        public ActionResult QueryORTImg(NoticeSearch search, Page page)
        {
            var dirPath = Server.MapPath("~/Upload/ORTImg");
            //若无文件夹则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var dir = new DirectoryInfo(dirPath);
            var imgVMs = dir.EnumerateFiles("*.*").Where(s => s.Name.ToLower().EndsWith(".jpg") || s.Name.ToLower().EndsWith(".jpeg") || s.Name.ToLower().EndsWith(".png")).Select(f => new ORTImgVM() { Name=f.Name, FullPath=f.FullName, LastWriteTime=f.LastWriteTime });
            var orderedImgVMs = imgVMs.OrderByDescending(f => f.LastWriteTime).Skip(page.Skip).Take(page.PageSize);
            var ORTImagList = new List<ORTImgVM>();
            foreach (var item in orderedImgVMs)
            {
                var buildingName = item.Name.Split('.')[0];

                if (buildingName.Contains("_"))
                {
                    buildingName = buildingName.Split('_')[0];
                }
                ORTImagList.Add(new ORTImgVM() { Name = item.Name, FullPath = item.FullPath, BuildingName = buildingName, LastWriteTime = item.LastWriteTime });
            }
            //var serializeStr = JsonConvert.SerializeObject(orderedImgVMs);
            var pagedImgVMs = new PagedListModel<ORTImgVM>(imgVMs.Count(), ORTImagList);
            var serializeStr= JsonConvert.SerializeObject(pagedImgVMs);
            return Content(serializeStr, "application/json");
        }
        public string DeleteORTImg(string name)
        {
            var dirPath = Server.MapPath("~/Upload/ORTImg");
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

        public ActionResult QueryCapacity()
        {
            var apiUrl = string.Format("Fixture/QueryFixtureStatusMoniterAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult GetCapacity()
        {
            var apiUrl = string.Format("ElectricalBoard/GetCapacityAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetTotalCapacity()
        {
            var apiUrl = string.Format("ElectricalBoard/GetTotalCapacityAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #region ModelLineHRMaintenance
        public ActionResult ModelLineHRMaintenance()
        {
            return View();
        }
        
        public ActionResult QueryModelLineHRs()//Fixture_PartModelSearch search, Page page)
        {
            var apiUrl = "ElectricalBoard/QueryModelLineHRsAPI";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryModelLineHR(int uid)
        {
            var apiUrl = string.Format("ElectricalBoard/QueryModelLineHRAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddModelLineHR(ModelLineHRDTO dto)
        {
            dto.Created_Date = DateTime.Now;
            dto.Modified_Date = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, "ElectricalBoard/AddModelLineHRAPI");
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult EditModelLineHR(ModelLineHRDTO dto)
        {
            dto.Modified_Date = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, "ElectricalBoard/EditModelLineHRAPI");
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteModelLineHR(int uid)
        {
            var apiUrl = string.Format("ElectricalBoard/DeleteModelLineHRAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadName"></param>
        /// <returns></returns>
        public string ImportModelLineHR(HttpPostedFileBase uploadName)
        {
            string errorInfo = string.Empty;
            try
            {
                using (var xlPackage = new ExcelPackage(uploadName.InputStream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                    int totalRows = worksheet.Dimension.End.Row;
                    int totalColumns = worksheet.Dimension.End.Column;
                    if (worksheet == null)
                    {
                        errorInfo = "没有worksheet内容";
                        return errorInfo;
                    }
                    //头样式设置
                    var propertiesHead = new[]
                    {
                        "工站",
                        "总人力",
                        "应到",
                        "实到",
                        "休息",
                        "事假",
                        "病假",
                        "旷工"
                    };
                    bool isExcelError = false;
                    for (int i = 1; i <= totalColumns; i++)
                    {
                        if (worksheet.Cells[1, i].Value != null && !string.IsNullOrWhiteSpace(worksheet.Cells[1, i].Value.ToString()))
                        {
                            var result = worksheet.Cells[1, i].Value.ToString();
                            var hasItem = propertiesHead.FirstOrDefault(m => m.Contains(result));
                            if (hasItem == null)
                            {
                                isExcelError = true;
                                break;
                            }
                        }
                        else
                        {
                            isExcelError = true;
                            break;
                        }
                    }
                    if (isExcelError)
                    {
                        errorInfo = "Excel格式不正确";
                        return errorInfo;
                    }

                    var modelLineList = new List<ModelLineHRDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var workStationValue = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "工站")].Value);
                        var totalValue = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "总人力")].Value);
                        int? total = null;
                        var shouldComeValue = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "应到")].Value);
                        int? shouldCome = null;
                        var actualComeValue = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "实到")].Value);
                        int? actualCome = null;
                        var vacationLeaveValue = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "休息")].Value);
                        int? vacationLeave = null;
                        var personalLeaveValue = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "事假")].Value);
                        int? personalLeave = null;
                        var sickLeaveValue = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "病假")].Value);
                        int? sickLeave = null;
                        var absentLeaveValue = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "旷工")].Value);
                        int? absentLeave = null;
                        if (string.IsNullOrWhiteSpace(workStationValue))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行工站没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            workStationValue = workStationValue.Trim();
                            if (workStationValue == "总计" || workStationValue == "合计")
                            {
                                continue;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(totalValue))
                        {
                            totalValue = totalValue.Trim();
                            try
                            {
                                total = int.Parse(totalValue);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(shouldComeValue))
                        {
                            shouldComeValue = shouldComeValue.Trim();
                            try
                            {
                                shouldCome = int.Parse(shouldComeValue);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(actualComeValue))
                        {
                            actualComeValue = actualComeValue.Trim();
                            try
                            {
                                actualCome = int.Parse(actualComeValue);
                            }
                            catch (Exception)
                            {
                                
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(vacationLeaveValue))
                        {
                            vacationLeaveValue = vacationLeaveValue.Trim();
                            try
                            {
                                vacationLeave = int.Parse(vacationLeaveValue);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(personalLeaveValue))
                        {
                            personalLeaveValue = vacationLeaveValue.Trim();
                            try
                            {
                                personalLeave = int.Parse(personalLeaveValue);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(sickLeaveValue))
                        {
                            sickLeaveValue = sickLeaveValue.Trim();
                            try
                            {
                                sickLeave = int.Parse(sickLeaveValue);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(absentLeaveValue))
                        {
                            absentLeaveValue = absentLeaveValue.Trim();
                            try
                            {
                                absentLeave = int.Parse(absentLeaveValue);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }

                        var modelLineHR = new ModelLineHRDTO()
                        {
                            Station = workStationValue,
                            Total = total,
                            ShouldCome = shouldCome,
                            ActualCome = actualCome,
                            VacationLeave = vacationLeave,
                            PersonalLeave = personalLeave,
                            SickLeave = sickLeave,
                            AbsentLeave = absentLeave,
                            Created_Date = DateTime.Now,
                            Modified_Date = DateTime.Now
                        };

                        modelLineList.Add(modelLineHR);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(modelLineList);
                    var apiInsertVendorInfoUrl = string.Format("ElectricalBoard/CoverModelLineHRAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入Model Line 人力数据失败：" + e.ToString();
            }
            return errorInfo;
        }

        private int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    //Excel开始的index为1
                    return i + 1;
                }

            }
            return 0;
        }
    }
}