using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Exception;
using PDMS.Model.ViewModels.Fixture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PDMS.Web.Controllers
{
    public class ExceptionController : WebControllerBase
    {


        #region 异常部门管理--------------------

        public ActionResult ExceptionDeptIndex()
        {
            ExceptionVM currentVM = new ExceptionVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
            }
            else
            {
                //获取
                var Plants = new List<PlantVM>();
                var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {

                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }

            }

            var apiDeptUrl = $"Exception/FetchExceptionDeptsAPI/?Plant_Organization_UID=0&BG_Organization_UID=0";
            var responSeptMessage = APIHelper.APIGetAsync(apiDeptUrl);
            var resultDept = responSeptMessage.Content.ReadAsStringAsync().Result;
            var Depts = JsonConvert.DeserializeObject<List<Depts>>(resultDept);
            currentVM.ExceDepts = Depts;
            ViewBag.PageTitle = "Department";
            return View(currentVM);
        }
        public  ActionResult QueryExceptionDept(ExceptionDTO search, Page page)
        {
            var apiUrl = string.Format("Exception/QueryExceptionDeptAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 部门添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddExceptionDept(ExceptionDTO dto)
        {
            dto.Exception_Dept_Name = dto.Exception_Dept_Name.ToUpper().Trim();
            dto.Created_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = "Exception/AddExceptionDeptAPI";
            var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
            //return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FetchExceptionDepts(int BG_Organization_UID, int Plant_Organization_UID)
        {
            //获取所有部门
            var apiDeptUrl = $"Exception/FetchExceptionDeptsAPI/?Plant_Organization_UID={Plant_Organization_UID}&BG_Organization_UID={BG_Organization_UID}";
            var responSeptMessage = APIHelper.APIGetAsync(apiDeptUrl);
            var resultDept = responSeptMessage.Content.ReadAsStringAsync().Result;
            var Depts = JsonConvert.DeserializeObject<List<Depts>>(resultDept);
            return Json(Depts, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FetchExceptionDept(int uid)
        {
            var apiUrl = $"Exception/FetchExceptionDeptAPI?uid={uid}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<ExceptionDTO>(result);
            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleExceptionDept(int uid)
        {
            var apiUrl = $"Exception/DeleExceptionDeptAPI?uid={uid}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            if (Convert.ToInt32(result) > 0)
            {
                return Json(new {Msg="Delete successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Delete failed" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult UpdateExceptionDept(ExceptionDTO dto)
        {

            dto.Modified_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = "Exception/UpdateExceptionDeptAPI";
            var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var obj = new
            {
                success = result,

            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 异常编码管理----------------------
        public ActionResult ExceptionInfoIndex()
        {
            ExceptionVM currentVM = new ExceptionVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
            }
            else
            {
                //获取
                var Plants = new List<PlantVM>();
                var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {

                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }

            }

            var apiDeptUrl = $"Exception/FetchExceptionDeptsAPI/?Plant_Organization_UID=0&BG_Organization_UID=0";
            var responSeptMessage = APIHelper.APIGetAsync(apiDeptUrl);
            var resultDept = responSeptMessage.Content.ReadAsStringAsync().Result;
            var Depts = JsonConvert.DeserializeObject<List<Depts>>(resultDept);
            currentVM.ExceDepts = Depts;
            ViewBag.PageTitle = "Issue Code";
            return View(currentVM);
        }
        public ActionResult QueryExceptionCode(ExceptionDTO search, Page page)
        {
            var apiUrl = string.Format("Exception/QueryExceptionCodeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 添加异常收件人
        /// </summary>
        public ActionResult AddExceptionCode(int deptUID, string exce_code, string exce_name)
        {
            ExceptionDTO dto = new ExceptionDTO();
            dto.Exception_Dept_UID = deptUID;
            dto.Exception_Nub = exce_code;
            dto.Exception_Name = exce_name;
            dto.Created_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = "Exception/AddExceptionCodeAPI";
            var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<ExceptionDTO>(result);

            //var obj = new
            //{
            //    success = entity,

            //};
            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleExceptionCode(string IDs)
        {
            var apiUrl = $"Exception/DeleExceptionCodeAPI?IDs={IDs}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            if (Convert.ToInt16(result) > 0)
            {
                return Json(new { success = 1, Msg = "Delete successfully !" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = 0, Msg = "Delete failed !" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult FetchExceptionCode(int uid)
        {
            var apiUrl = $"Exception/FetchExceptionCodeAPI?uid={uid}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<ExceptionDTO>(result);
            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateExceptionCode(ExceptionDTO dto)
        {
            dto.Modified_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = "Exception/UpdateExceptionCodeAPI";
            var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var obj = new
            {
                success = result,

            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据 部门ID获取ExceptionCode
        /// </summary>
        /// <param name="deptUID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FetchExceptionCodeBasedDept(int deptUID)
        {
            var apiUrl = $"Exception/FetchExceptionCodeBasedDeptAPI?deptUID={deptUID}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<List<ExceptionDTO>>(result);

            return Json(new { success=entity}, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 异常收件人以及时间段分配
        public ActionResult ExceptionTimeIndex()
        {
            ExceptionVM currentVM = new ExceptionVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
            }
            else
            {
                //获取
                var Plants = new List<PlantVM>();
                var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {

                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }

            }

            var apiDeptUrl = $"Exception/FetchExceptionDeptsAPI/?Plant_Organization_UID=0&BG_Organization_UID=0";
            var responSeptMessage = APIHelper.APIGetAsync(apiDeptUrl);
            var resultDept = responSeptMessage.Content.ReadAsStringAsync().Result;
            var Depts = JsonConvert.DeserializeObject<List<Depts>>(resultDept);
            currentVM.ExceDepts = Depts;
            ViewBag.PageTitle = "Trigger Circle Time";
            return View(currentVM);
        }
        public ActionResult ExceptionAllocateIndex()
        {
            ExceptionVM currentVM = new ExceptionVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
            }
            else
            {
                //获取
                var Plants = new List<PlantVM>();
                var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {

                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }

            }

            var apiDeptUrl = $"Exception/FetchExceptionDeptsAPI/?Plant_Organization_UID=0&BG_Organization_UID=0";
            var responSeptMessage = APIHelper.APIGetAsync(apiDeptUrl);
            var resultDept = responSeptMessage.Content.ReadAsStringAsync().Result;
            var Depts = JsonConvert.DeserializeObject<List<Depts>>(resultDept);
            currentVM.ExceDepts = Depts;
            ViewBag.PageTitle = "Email Setting";
            return View(currentVM);

        }
        public ActionResult QueryExceptionEmail(ExceptionDTO search, Page page)
        {
            var apiUrl = string.Format("Exception/QueryExceptionEmailAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryExceptionTime(ExceptionDTO search, Page page)
        {
            var apiUrl = string.Format("Exception/QueryExceptionTimeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddPeriodTime(string model)
        {
            IList<ExceptionDTO> ctm = new JavaScriptSerializer().Deserialize<IList<ExceptionDTO>>(model);
            int ret = 0;
            foreach(var dto in ctm)
            {
                dto.Origin_UID = dto.Origin_UID;
                dto.Exception_Time_At = dto.Exception_Time_At;

                dto.Created_UID = CurrentUser.GetUserInfo.Account_UID;
                dto.Created_Date = DateTime.Now;
                dto.Modified_UID = CurrentUser.GetUserInfo.Account_UID;
                dto.Modified_Date = dto.Created_Date;

                var apiUrl = $"Exception/AddPeriodTimeAPI";
                var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                ret += Convert.ToInt32(result);
            }
         

            return Json(new { success= ret },JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletPeriodTime(string timeID)
        {
            var apiUrl = $"Exception/DeletPeriodTimeAPI?timeID={timeID}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");

            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddExceptionEmail(string model)
        {
            IList<ExceptionDTO> ctm = new JavaScriptSerializer().Deserialize<IList<ExceptionDTO>>(model);
            int ret = 0;
            foreach(var dto in ctm)
            {
                dto.Created_UID = CurrentUser.GetUserInfo.Account_UID;
                dto.Created_Date = DateTime.Now;
                dto.Modified_UID = CurrentUser.GetUserInfo.Account_UID;
                dto.Modified_Date = DateTime.Now;
                var apiUrl = $"Exception/AddExceptionEmailAPI";
                var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var entity = JsonConvert.DeserializeObject<ExceptionDTO>(result);
                if (entity != null && entity.Repeat != 1)
                {
                    ret++;
                }
            }



            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FetchExceptionEmailInfo(int uid)
        {
            var apiUrl = $"Exception/FetchExceptionEmailInfoAPI?uid={uid}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<ExceptionDTO>(result);
            return Json(new { success = entity }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult  EditExceptionEmail(ExceptionDTO dto)
        {
            dto.Modified_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = $"Exception/EditExceptionEmailAPI";
            var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult  DeleExceptionEmail(int uid)
        {
            var apiUrl = $"Exception/DeleExceptionEmailAPI?uid={uid}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FetchEmail(int uid)
        {
            var sendUrl = $"Exception/FetchEmailAPI?uid={uid}";
            var responMessage = APIHelper.APIGetAsync(sendUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var sendEmail = result.Replace("[","").Replace("]","");

            var ccUrl = $"Exception/FetchEmailCCAPI?uid={uid}";
            var responCCMessage = APIHelper.APIGetAsync(ccUrl);
            var ccResults = responCCMessage.Content.ReadAsStringAsync().Result;
            var ccEmail = ccResults.Replace("[", "").Replace("]", ""); ;


            return Json(new { sendEmail = sendEmail, ccEmail= ccEmail }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FetchUserInfo(string NTID)
        {

            var url = $"Exception/FetchUserInfoAPI?User_NTID={NTID.Trim()}";
            var responMessage = APIHelper.APIGetAsync(url);
            var results = responMessage.Content.ReadAsStringAsync().Result;
            return Content(results, "application/json");
        }

        public ActionResult  FetchExceptionProject(int uid)
        {
            var apiDeptUrl = $"Exception/FetchExceptionProjectAPI/?uid={uid}";
            var responSeptMessage = APIHelper.APIGetAsync(apiDeptUrl);
            var resultDept = responSeptMessage.Content.ReadAsStringAsync().Result;
            var projects = JsonConvert.DeserializeObject<List<Projects>>(resultDept);
            return Json(projects, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult FetchPeriodTimeBasedDeptID(int deptID)
        //{
        //    var apiUrl = $"Exception/FetchPeriodTimeBasedDeptIDAPI?deptID={deptID}";
        //    var responMessage = APIHelper.APIGetAsync(apiUrl);
        //    var result = responMessage.Content.ReadAsStringAsync().Result;
        //    var entity = JsonConvert.DeserializeObject<List<ExceptionDTO>>(result);
        //    return Json(new { success = entity }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult FetchPeriodTimeBasedProjectID(int projectID)
        {
            var apiUrl = $"Exception/FetchPeriodTimeBasedProjectIDAPI?projectID={projectID}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<List<ExceptionDTO>>(result);
            return Json(new { success = entity }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateDeptTime(int deptID, int dealyDayNub, int dayPeriod, int sendMaxTime)
        {
            var apiUrl = $"Exception/UpdateDeptTimeAPI?deptID={deptID}&dealyDayNub={dealyDayNub}&dayPeriod={dayPeriod}&sendMaxTime={sendMaxTime}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");

            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateProjectTime(int projectID, int dealyDayNub, int dayPeriod, int sendMaxTime)
        {
            ExceptionProjectDTO dto = new ExceptionProjectDTO();
            dto.Project_UID = projectID;
            dto.DelayDayNub = dealyDayNub;
            dto.DayPeriod = dayPeriod;
            dto.SendMaxTime = sendMaxTime;
            dto.Created_UID= CurrentUser.GetUserInfo.Account_UID;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Modified_Date = dto.Created_Date;
            var apiUrl = $"Exception/UpdateProjectTimeAPI";
            var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");

            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FetchExceptionProjectCycleTime(int projectID)
        {
            var apiDeptUrl = $"Exception/FetchExceptionProjectCycleTimeAPI/?uid={projectID}";
            var responSeptMessage = APIHelper.APIGetAsync(apiDeptUrl);
            var result = responSeptMessage.Content.ReadAsStringAsync().Result;
            var ret= JsonConvert.DeserializeObject<ExceptionProjectDTO>(result);
            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 异常记录管理
        public ActionResult ExceptionRecordAdd(string WorkDate, int ShiftTimeID, int LintID , string codeID, int stationID, string note,int ProjectID,string ContactPerson, string ContactPhone, string SendIDs, string CCIDs )
        {
            // ExceptionDTO dto = new ExceptionDTO();
            ExceptionAddDTO dto = new ExceptionAddDTO();
            dto.WorkDate = Convert.ToDateTime(WorkDate);
            dto.ShiftTimeID = ShiftTimeID;
            dto.LineID = LintID;
            dto.Exception_Code_UIDs = codeID;
            dto.StationID = stationID;
            dto.Note = note;
            dto.Project_UID = ProjectID;
            dto.Contact_Person = ContactPerson;
            dto.Contact_Phone = ContactPhone;
            dto.SendIDs = SendIDs;
            dto.CCIDs = CCIDs;
            dto.Created_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = "Exception/ExceptionRecordAddAPI";
            var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return Json(new { success=result}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FetchStations(int LineID)
        {
            var apiUrl = $"Exception/FetchStationsAPI?LineID={LineID}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<List<Stations>>(result);
            
            return Json(new { success= entity }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取班次信息
        /// </summary>
        /// <param name="ShiftTimeID"></param>
        /// <returns></returns>
        public ActionResult FetchShifTime(int ShiftTimeID)
        {
            var apiUrl = $"Exception/FetchShifTimeAPI?ShiftTimeID={ShiftTimeID}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<ShiftTime>(result);

            return Json(new { success = entity }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExceptionRecordIndex()
        {
            ExceptionVM currentVM = new ExceptionVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
            }
            else
            {
                //获取
                var Plants = new List<PlantVM>();
                var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {

                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }

            }

            var apiDeptUrl = $"Exception/FetchExceptionDeptsAPI/?Plant_Organization_UID=0&BG_Organization_UID=0";
            var responSeptMessage = APIHelper.APIGetAsync(apiDeptUrl);
            var resultDept = responSeptMessage.Content.ReadAsStringAsync().Result;
            var Depts = JsonConvert.DeserializeObject<List<Depts>>(resultDept);
            currentVM.ExceDepts = Depts;



            var apiShiftUrl = $"Exception/FetchAllShifTimeAPI";
            var responsShiftMessage = APIHelper.APIGetAsync(apiShiftUrl);
            var resultShift = responsShiftMessage.Content.ReadAsStringAsync().Result;
            var Shifts = JsonConvert.DeserializeObject<List<ShiftTime>>(resultShift);
            currentVM.Shifts = Shifts;

            var emailUrl = string.Format("Exception/FethAllEmailAPI");
            HttpResponseMessage emailMessage = APIHelper.APIPostAsync(new EmailSendDTO(), emailUrl);
            var emails= emailMessage.Content.ReadAsStringAsync().Result;
            ViewBag.Email = JsonConvert.DeserializeObject<List<SystemUserDTO>>(emails);
            ViewBag.PageTitle = "Issue Management";
            return View(currentVM);

        }

        public ActionResult QueryExceptionRecord(ExceptionDTO search, Page page)
        {
            var apiUrl = string.Format("Exception/QueryExceptionRecordAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

       public ActionResult ExportExcptionRecord2Excel(ExceptionDTO search)
        {
            //get Export datas
            var apiUrl = "Exception/ExportExcptionRecord2ExcelAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<ExceptionDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("ExceptionRecord");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq","Project", "Line","Procedure", "Station", "Status", "Output Date", "Shift", "Exception Department", "Exception Code", "Exception Reason", "Owner","Contact","Create User", "Created Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("ExceptionRecord");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    //seq
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Project_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.LineName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Seq;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.StationName;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Status==0?"Open":"Close";
                    worksheet.Cells[index + 2, 7].Value = currentRecord.WorkDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Shift;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Exception_Dept_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Exception_Nub;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Exception_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Contact_Person;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Contact_Phone;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.User_Name;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult ExportSomeRecord2Excel(string uids)
        {
            var apiUrl =$@"Exception/ExportSomeRecord2Excel?uids={uids}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<ExceptionDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("ExceptionRecord");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "Project", "Line", "Procedure", "Station", "Status", "Output Date", "Shift", "Exception Department", "Exception Code", "Exception Reason", "Owner", "Contact", "Create User", "Created Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("ExceptionRecord");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    //seq
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Project_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.LineName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Seq;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.StationName;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Status == 0 ? "Open" : "Close";
                    worksheet.Cells[index + 2, 7].Value = currentRecord.WorkDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Shift;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Exception_Dept_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Exception_Nub;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Exception_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Contact_Person;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Contact_Phone;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.User_Name;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }
        /// <summary>
        /// 根据线体的ID获取所有的工站
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FetchStationsBasedLine(int uid)
        {
            var apiUrl = $"Exception/FetchStationsBasedLineAPI?uid={uid}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<List<Stations>>(result);
            return Json(new { success = entity }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CloseExceptionOrder(int uid)
        {
            ExceptionDTO dto = new ExceptionDTO();
            dto.Exception_Record_UID = uid;
            dto.Modified_UID= CurrentUser.GetUserInfo.Account_UID;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = $@"Exception/CloseExceptionOrderAPI";

            var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteExceptionOrder(int uid)
        {
            var apiUrl = $@"Exception/DeleteExceptionOrderAPI?uid={uid}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ExceptionReply(int uid,string content)
        {
            ReplyRecordDTO dto = new ReplyRecordDTO();
            dto.Exception_Record_UID = uid;
            dto.Exception_Content = content;
            dto.Reply_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Reply_Date = DateTime.Now;

            var apiUrl = $@"Exception/ExceptionReplyAPI";
            var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FetchLineBasedPlant(int plantuid,int bguid, int funuid)
        {

            var apiLineUrl = $"Exception/FetchLineBasedPlantAPI?plantuid={plantuid}&bguid={bguid}&funuid={funuid}";
            var responsLineMessage = APIHelper.APIGetAsync(apiLineUrl);
            var resultLine = responsLineMessage.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<Line>>(resultLine);
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FetchGL_LineWithGroupAPI(int plantuid, int bguid, int customerid)
        {

            ExceptionDTO dto = new ExceptionDTO();
            dto.Plant_Organization_UID = plantuid;
            dto.BG_Organization_UID = bguid;
            dto.CustomerID = customerid;
            var apiLineUrl = $"Exception/FetchGL_LineWithGroupAPI";
            var responsLineMessage = APIHelper.APIPostAsync(dto, apiLineUrl);
            var resultLine = responsLineMessage.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<Line>>(resultLine);
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult FetchLineBasedPlantBGCustomer(int plantuid, int bguid, int customerid)
        {
            ExceptionDTO dto = new ExceptionDTO();
            dto.Plant_Organization_UID = plantuid;
            dto.BG_Organization_UID = bguid;
            dto.CustomerID = customerid;
            var apiLineUrl = $"Exception/FetchLineBasedPlantBGCustomerAPI";
            var responsLineMessage = APIHelper.APIPostAsync(dto, apiLineUrl);
            var resultLine = responsLineMessage.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<Line>>(resultLine);
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewRecordReply(int uid)
        {
            var apiUrl = $"Exception/ViewRecordReplyAPI?uid={uid}";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<List<ReplyRecordDTO>>(result);
            ViewBag.Entity= entity;
            return View();
        }


        [HttpPost]
        public ActionResult SendEmailException(string exceIDs,string SendIDs,string CCIDs,string Subject,string ContentString)
        {

            EmailSendDTO dto = new EmailSendDTO();
            dto.Exception_Record_UIDs = exceIDs;
            dto.CCIDs = CCIDs;
            dto.SendIDs = SendIDs;
            dto.Subject = Subject;
            dto.Modified_UID = CurrentUser.GetUserInfo.Account_UID;
            dto.Modified_Date = DateTime.Now;
            dto.ContentString = ContentString;
            var apiUrl = $"Exception/SendEmailExceptionAPI";
            var responMessage = APIHelper.APIPostAsync(dto,apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result,"application/json");
        }





        #endregion

        #region Downtime Dashboard
        public ActionResult DowntimeIndex()
        {
            ExceptionVM currentVM = new ExceptionVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
            }
            else
            {
                //获取
                var Plants = new List<PlantVM>();
                var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {

                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }

            }

            var apiDeptUrl = $"Exception/FetchExceptionDeptsAPI/?Plant_Organization_UID=0&BG_Organization_UID=0";
            var responSeptMessage = APIHelper.APIGetAsync(apiDeptUrl);
            var resultDept = responSeptMessage.Content.ReadAsStringAsync().Result;
            var Depts = JsonConvert.DeserializeObject<List<Depts>>(resultDept);
            currentVM.ExceDepts = Depts;


            var emailUrl = string.Format("Exception/FethAllEmailAPI");
            HttpResponseMessage emailMessage = APIHelper.APIPostAsync(new EmailSendDTO(), emailUrl);
            var emails = emailMessage.Content.ReadAsStringAsync().Result;
            ViewBag.Email = JsonConvert.DeserializeObject<List<SystemUserDTO>>(emails);
            ViewBag.PageTitle = "Issue Elapsed Time Dashboard";
            return View(currentVM);
        }

        [HttpPost]
        public ActionResult FetchShiftTimeBasedBG(int plantuid, int bguid)
        {
            var apiShiftUrl = $"Exception/FetchShiftTimeBasedBGAPI?plantuid={plantuid}&&bguid={bguid}";
            var responsShiftMessage = APIHelper.APIGetAsync(apiShiftUrl);
            var resultShift = responsShiftMessage.Content.ReadAsStringAsync().Result;
            var ret = JsonConvert.DeserializeObject<List<ShiftTime>>(resultShift);
            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult FetchShiftTimeDetail(int uid)
        {
            var apiShiftUrl = $"Exception/FetchShiftTimeDetailAPI?uid={uid}";
            var responsShiftMessage = APIHelper.APIGetAsync(apiShiftUrl);
            var resultShift = responsShiftMessage.Content.ReadAsStringAsync().Result;
            var ret = JsonConvert.DeserializeObject<ShiftTime>(resultShift);
            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult QueryExceptionRecordDashboard(int plantID,int bgUid, int projectID, int lineID, string lineName, string workDate, int unproposeDays, string startH, string startM, int shiftID)
        {

            DashboardSearchDTO dto = new DashboardSearchDTO();
            dto.Plant_Organization_UID = plantID;
            dto.BG_Organization_UID = bgUid;
            dto.Project_UID = projectID;
            dto.LineID = lineID;
            dto.LineName = lineName;
            dto.WorkDate = workDate;
            dto.DelayDayNub = unproposeDays;
            dto.StartH = startH;
            dto.StartM = startM;
            dto.ShiftTimeID = shiftID;
            var apiUrl = string.Format("Exception/QueryExceptionRecordDashboardAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }
        [HttpPost]
        public ActionResult QueryDowntimeRecords(int plantID, int bgUid, int projectID, int lineID, string lineName, string workDate, int unproposeDays, string startH, string startM, int shiftID) {
            DashboardSearchDTO dto = new DashboardSearchDTO();
            dto.Plant_Organization_UID = plantID;
            dto.BG_Organization_UID = bgUid;
            dto.Project_UID = projectID;
            dto.LineID = lineID;
            dto.LineName = lineName;
            dto.WorkDate = workDate;
            dto.DelayDayNub = unproposeDays;
            dto.StartH = startH;
            dto.StartM = startM;
            dto.ShiftTimeID = shiftID;
            var apiUrl = string.Format("Exception/QueryDowntimeRecordsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        [HttpPost]
        public ActionResult FetchExceptionDetail(int uid)
        {
            var apiShiftUrl = $"Exception/FetchExceptionDetailAPI?uid={uid}";
            var responsShiftMessage = APIHelper.APIGetAsync(apiShiftUrl);
            var resultShift = responsShiftMessage.Content.ReadAsStringAsync().Result;
            // var ret = JsonConvert.DeserializeObject<ExceptionDTO>(resultShift);
            return Json(new { success = resultShift }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FetchLatestReply(int uid)
        {
            var apiShiftUrl = $"Exception/FetchLatestReplyAPI?uid={uid}";
            var responsShiftMessage = APIHelper.APIGetAsync(apiShiftUrl);
            var resultShift = responsShiftMessage.Content.ReadAsStringAsync().Result;
            // var ret = JsonConvert.DeserializeObject<ExceptionDTO>(resultShift);
            return Json(new { success = resultShift }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region others
        public ActionResult FetchDatetimeNow()
        {
            return Content(DateTime.Now.ToString("yyyy-MM-dd"));
        }
        public ActionResult FetchDatetimeNowTime()
        {
            return Content(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        #endregion

    }
}