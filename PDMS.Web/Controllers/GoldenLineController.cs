using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace PDMS.Web.Controllers
{
    public class GoldenLineController : WebControllerBase
    {
        public int GetPlantOrgUid()
        {
            int plantorguid = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                plantorguid = (int)CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
            return plantorguid;
        }
        public ActionResult Customer()
        {
            Fixture_PartVM currentVM = new Fixture_PartVM();
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


            var emailUrl = string.Format("Exception/FethAllEmailAPI");
            HttpResponseMessage emailMessage = APIHelper.APIPostAsync(new EmailSendDTO(), emailUrl);
            var emails = emailMessage.Content.ReadAsStringAsync().Result;
            ViewBag.Email = JsonConvert.DeserializeObject<List<SystemUserDTO>>(emails);


            return View("Customer", currentVM);
        }

        public ActionResult GetCustomers(int oporgid)
        {
            var queryModel = new QueryModel<System_ProjectModelSearch>();
            queryModel.Equal = new System_ProjectModelSearch() { Organization_UID = oporgid };

            var apiUrl = string.Format("GoldenLine/QueryCustomersAPI?oporgid={0}", oporgid);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(queryModel, apiUrl);

            //var apiUrl = string.Format("GoldenLine/QueryCustomersAPI?opUID={0}", oporgid);
            //HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string GetCustomerActualVSRealTimePlan(int customerID, int shiftTimeID, string outputDate)
        {
            var apiUrl = string.Format("GoldenLine/GetCustomerActualVSRealTimePlanAPI?customerID={0}&shiftTimeID={1}&outputDate={2}", customerID, shiftTimeID, outputDate);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string GetCustomerVAOLE(int customerID, int shiftTimeID, string outputDate)
        {
            var apiUrl = string.Format("GoldenLine/GetCustomerVAOLEAPI?customerID={0}&shiftTimeID={1}&outputDate={2}", customerID, shiftTimeID, outputDate);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public ActionResult GetShiftTimes(int oporgid, int? funcgid = null)
        {
            var queryModel = new QueryModel<GL_ShiftTimeModelSearch>();
            queryModel.Equal = new GL_ShiftTimeModelSearch() { BG_Organization_UID = oporgid, FunPlant_Organization_UID = funcgid, IsEnabled = true };

            var apiUrl = string.Format("GoldenLine/QueryGetShiftTimesAPI?oporgid={0}", oporgid);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(queryModel, apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult Station()
        {

            return View();
        }

        public ActionResult GetCustomerLinePerf(Page page,int customerId, string outputDate, int shiftTimeId)
        {
            //var apiUrl = string.Format("GoldenLine/GetCustomerLinePerfAPI?customerId={0}&outputDate={1}&shiftId={2}", customerId, outputDate, shiftId);
            //var responMessage = APIHelper.APIGetAsync(apiUrl);
            //var result = responMessage.Content.ReadAsStringAsync().Result;
            //return Content(result, "application/json");

            var queryModel = new QueryModel<GL_LineShiftPerfModelSearch>();
            queryModel.Equal = new GL_LineShiftPerfModelSearch() { CustomerID = customerId, OutputDate = outputDate, ShiftTimeID = shiftTimeId };

            var apiUrl = string.Format("GoldenLine/GetCustomerLinePerfAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(queryModel, page, apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

            ////模拟的数据
            //var result = "{\"TotalItemCount\":3,\"Items\":[{\"CustomerId\":1,\"CustomerName\":\"IBM\",\"OutputDate\":null,\"ShiftIndex\":null,\"LineId\":1,\"LineName\":\"IBMSER_01\",\"StationId\":2,\"StationName\":\"ASSEMBLE MA_2\",\"AssemblyId\":null,\"AssemblyNumber\":null,\"PlanOutput\":0,\"StandOutput\":0,\"ActualOutput\":0,\"UPH\":0.00,\"PlanDownTime\":0,\"UPDownTime\":0,\"RunTime\":0,\"ActualOutputVSPlan\":0.0000,\"ActualOutputVSRealTimePlan\":0.0000,\"ActualOutputVSStdOutput\":0.0000,\"LineUtil\":0.0000,\"CapacityLoading\":0.0000,\"VAOLE\":null,\"LineStatus\":null},{\"CustomerId\":1,\"CustomerName\":\"IBM\",\"OutputDate\":\"2018 - 05 - 25T00: 00:00\",\"ShiftIndex\":0,\"LineId\":2,\"LineName\":\"IBMSER_02\",\"StationId\":7,\"StationName\":\"ASSEMBLE MA_2\",\"AssemblyId\":2,\"AssemblyNumber\":\"IBM01KU418\",\"PlanOutput\":0,\"StandOutput\":86,\"ActualOutput\":58,\"UPH\":11.61,\"PlanDownTime\":4589,\"UPDownTime\":4268,\"RunTime\":22443,\"ActualOutputVSPlan\":0.0000,\"ActualOutputVSRealTimePlan\":0.0000,\"ActualOutputVSStdOutput\":0.6740,\"LineUtil\":0.7170,\"CapacityLoading\":0.1961,\"VAOLE\":0.5938,\"LineStatus\":0},{\"CustomerId\":1,\"CustomerName\":\"IBM\",\"OutputDate\":null,\"ShiftIndex\":null,\"LineId\":3,\"LineName\":\"IBMSER_03\",\"StationId\":12,\"StationName\":\"ASSEMBLE MA_2\",\"AssemblyId\":null,\"AssemblyNumber\":null,\"PlanOutput\":0,\"StandOutput\":0,\"ActualOutput\":0,\"UPH\":0.00,\"PlanDownTime\":0,\"UPDownTime\":0,\"RunTime\":0,\"ActualOutputVSPlan\":0.0000,\"ActualOutputVSRealTimePlan\":0.0000,\"ActualOutputVSStdOutput\":0.0000,\"LineUtil\":0.0000,\"CapacityLoading\":0.0000,\"VAOLE\":null,\"LineStatus\":null}]}";
            //return Content(result, "application/json");
        }

        public string GetStartTime(string customerName, string stationName,int Plant_Organization_UID, DateTime startTime, DateTime? endTime)
        {
            var apiUrl = string.Format("GoldenLine/GetStartTimeAPI?customerName={0}&stationName={1}&Plant_Organization_UID={2}&startTime={3}&endTime={4}", customerName, stationName, Plant_Organization_UID, startTime, endTime);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
            //return Content(result, "application/json");
        }
        [HttpGet]
        public string GetRunTime(string customerName, string lineName,int Plant_Organization_UID, int STDCT, DateTime startTime, DateTime? endTime)
        {
            var apiUrl = string.Format("GoldenLine/GetRunTimeAPI?customerName={0}&lineName={1}&Plant_Organization_UID={2}&STDCT={3}&startTime={4}&endTime={5}", customerName, lineName, Plant_Organization_UID, STDCT, startTime, endTime);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        [HttpGet]
        public string GetUnPlanDownTime(string customerName, string lineName, int Plant_Organization_UID, int STDCT, DateTime startTime, DateTime? endTime)
        {
            var apiUrl = string.Format("GoldenLine/GetUnPlanDownTimeAPI?customerName={0}&lineName={1}&Plant_Organization_UID={2}&STDCT={3}&startTime={4}&endTime={5}", customerName, lineName, Plant_Organization_UID, STDCT, startTime, endTime);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        [HttpGet]
        public string GetLastUpdateTime(string customerName, string lineName, int Plant_Organization_UID, DateTime flagTime)
        {
            var apiUrl = string.Format("GoldenLine/GetLastUpdateTimeAPI?customerName={0}&lineName={1}&Plant_Organization_UID={2}&flagTime={3}", customerName, lineName, Plant_Organization_UID, flagTime);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        #region    线的图表
        public ActionResult Show(int? customerId, int? lineId, int? stationId, DateTime? dt, int? shiftIndex, int Plant_Organization_UID = 0, int BG_Organization_UID = 0, int FunPlant_Organization_UID = 0)
        {

            Fixture_PartVM currentVM = new Fixture_PartVM();
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
            ViewBag.Plant_UID = Plant_Organization_UID != 0 ? Plant_Organization_UID : 35;
            ViewBag.BG_UID = BG_Organization_UID != 0 ? BG_Organization_UID : optypeID;
            ViewBag.FunPlant_UID = FunPlant_Organization_UID != 0 ? FunPlant_Organization_UID : funPlantID;
            if (lineId != null && lineId != 0)
            {
                var apiUrl = string.Format("GoldenLine/GetStationDTOAPI?LineId={0}", lineId);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var GL_StationDTOs = JsonConvert.DeserializeObject<List<GL_StationDTO>>(result);

                if (GL_StationDTOs != null && GL_StationDTOs.Count > 0)
                {
                    var StationDTO = GL_StationDTOs.FirstOrDefault(o => o.IsOutput == true);

                    if (StationDTO != null)
                    {
                        stationId = StationDTO.StationID;
                    }
                }
            }


            ViewBag.RETRIVE_CID = customerId ?? 3;
            ViewBag.RETRIVE_LID = lineId ?? 2;
            ViewBag.RETRIVE_SID = stationId ?? 2;
            ViewBag.RETRIVE_DT = dt ?? DateTime.Now;

            ViewBag.RETRIVE_SI = shiftIndex ?? 4;
            return View("Show", currentVM);
        }
        public string GetStationStdActCT(int customerId, int lineId, DateTime outputDate, int shiftTimeID)
        {

            var apiUrl = string.Format("GoldenLine/GetStationStdActCTAPI?customerId={0}&lineId={1}&outputDate={2}&shiftTimeID={3}", customerId, lineId, outputDate, shiftTimeID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public ActionResult GetCustomerDTOs(int BG_Organization_UID)
        {
            var apiUrl = string.Format("GoldenLine/GetCustomerDTOAPI?BG_Organization_UID={0}", BG_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetShiftTimeDTOs(int Plant_Organization_UID, int BG_Organization_UID)
        {

            var apiUrl = string.Format("GoldenLine/GetShiftTimeDTOAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}", Plant_Organization_UID, BG_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetStationDTOs(int LineId)
        {

            var apiUrl = string.Format("GoldenLine/GetStationDTOAPI?LineId={0}", LineId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetONOMESStationDTOs(int LineId)
        {

            var apiUrl = string.Format("GoldenLine/GetONOMESStationDTOsAPI?LineId={0}", LineId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetLineDTOs(int CustomerID)
        {

            var apiUrl = string.Format("GoldenLine/GetReportLineDTOAPI?CustomerID={0}", CustomerID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetAllLineDTOs(int CustomerID)
        {

            var apiUrl = string.Format("GoldenLine/GetAllLineDTOsAPI?CustomerID={0}", CustomerID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        
        public ActionResult GetIPQCLineDTOs(int CustomerID)
        {
            var apiUrl = string.Format("GoldenLine/GetIPQCLineDTOAPI?CustomerID={0}", CustomerID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        
        public ActionResult GetGroupLineDTOs(int CustomerID)
        {

            var apiUrl = string.Format("GoldenLine/GetGroupLineDTOAPI?CustomerID={0}", CustomerID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string GetLatestLineStationInfo(int customerId, int lineId, int stationId, DateTime outputDate, int shiftTimeID)
        {

            var apiUrl = string.Format("GoldenLine/GetLatestLineStationInfoAPI?customerId={0}&lineId={1}&stationId={2}&outputDate={3}&shiftTimeID={4}", customerId, lineId, stationId, outputDate, shiftTimeID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }
        public string GetLineShiftPlanActInfo(int customerId, int lineId, int stationId, DateTime outputDate, int shiftTimeID)
        {
            var apiUrl = string.Format("GoldenLine/GetActualAndPlanDTOAPI?customerId={0}&lineId={1}&stationId={2}&outputDate={3}&shiftTimeID={4}", customerId, lineId, stationId, outputDate, shiftTimeID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;

        }

        public string GetShiftHourOutput(int customerId, int lineId, int stationId, DateTime outputDate, int shiftTimeID)
        {
            var apiUrl = string.Format("GoldenLine/GetShiftHourOutputAPI?customerId={0}&lineId={1}&stationId={2}&outputDate={3}&shiftTimeID={4}", customerId, lineId, stationId, outputDate, shiftTimeID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        #endregion

        public string GetShiftTime(int shiftIndex)
        {
            //ShiftTime model = ShiftTimeService.GetShiftTime(shiftIndex);
            //int hours = Math.Abs(Convert.ToDateTime(model.EndTime).Hour - Convert.ToDateTime(model.StartTime).Hour);
            //DateTime startTime = Convert.ToDateTime(model.StartTime);
            //List<string> shiftHour = new List<string>();
            //for (int i = 0; i < hours; i++)
            //{
            //    shiftHour.Add(startTime.ToString("HH:mm") + '~' + startTime.AddHours(1).ToString("HH:mm"));
            //    startTime = startTime.AddHours(1);
            //}
            //return JsonConvert.SerializeObject(shiftHour);



            //  ShiftTime model = ShiftTimeService.GetShiftTime(shiftIndex);
            int hours = Math.Abs(Convert.ToDateTime(DateTime.Now).Hour - Convert.ToDateTime(DateTime.Now).Hour);
            DateTime startTime = Convert.ToDateTime(DateTime.Now);
            List<string> shiftHour = new List<string>();
            for (int i = 0; i < hours; i++)
            {
                shiftHour.Add(startTime.ToString("HH:mm") + '~' + startTime.AddHours(1).ToString("HH:mm"));
                startTime = startTime.AddHours(1);
            }
            return JsonConvert.SerializeObject(shiftHour);
        }
        public string UpdateActualHC(int id, int actHC, decimal actUPPH)
        {
            //int r = WipShiftOutputService.Update(id, actHC, actUPPH);
            //string str = JsonConvert.SerializeObject(r);
            //return str;


            string str = JsonConvert.SerializeObject(1);
            return str;
        }
        public string GetAllNormalLineByCustomerId(int customerId)
        {
            //List<Line> lstRes = LineService.GetAllNormalByCustomerId(customerId);
            //string json = JsonConvert.SerializeObject(lstRes);
            //json = json.Replace("Bay ", "Bay");
            //return json;

            // List<Line> lstRes = LineService.GetAllNormalByCustomerId(customerId);
            string json = JsonConvert.SerializeObject("");
            json = json.Replace("Bay ", "Bay");
            return json;
        }

        public string GetAllNormalStationByCustomerId(int lineId)
        {

            //List<Station> lstRes = StationService.GetAllNormalByLineId(lineId);
            //return JsonConvert.SerializeObject(lstRes);

            //List<Station> lstRes = StationService.GetAllNormalByLineId(lineId);
            return JsonConvert.SerializeObject("");
        }

        #region GoldLine每小时产能
        public ActionResult WIPHourOutput(int? customerId, int? lineId, int? stationId, DateTime? dt, int? shiftIndex)
        {
            Fixture_PartVM currentVM = new Fixture_PartVM();
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

            ViewBag.RETRIVE_CID = customerId ?? 1;
            ViewBag.RETRIVE_LID = lineId ?? 1;
            ViewBag.RETRIVE_SID = stationId ?? 1;
            ViewBag.RETRIVE_DT = dt ?? DateTime.Now;
            ViewBag.RETRIVE_SI = shiftIndex ?? 1;
            return View("WIPHourOutput", currentVM);
        }

        public ActionResult queryHourOutPut(GL_WIPHourOutputDTO searchParm, Page page)
        {
            var apiUrl = string.Format("GoldenLine/QueryWIPHourOutputListAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParm, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParm"></param>
        /// <returns></returns>
        public ActionResult ExportHourOutPut(GL_WIPHourOutputDTO searchParmm)
        {
            var apiUrl = string.Format("GoldenLine/ExportHourOutPutAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("WIPHourOutPutDetial");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            //var stringParamHeads = new string[] { "Site", "BG", "Project", "Line", "Date", "料号", "Shift" };
            var stringHeads = new string[] { "NO.", "CustomerName", "LinaName", "StationName", "TimeInterval", "SerialNumber", "StartTime" };
            var list = JsonConvert.DeserializeObject<List<WipEventModel>>(result).ToList();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("WIPHourOutPutDetial");
                //for (int colIndex = 0; colIndex < stringParamHeads.Length; colIndex++)
                //{
                //    worksheet.Cells[1, colIndex + 1].Value = stringParamHeads[colIndex];
                //}
                //worksheet.Cells[2, 1].Value = "1";
                //worksheet.Cells[2, 2].Value = "2";
                //worksheet.Cells[2, 3].Value = "3";
                //worksheet.Cells[2, 4].Value = "4";
                //worksheet.Cells[2, 5].Value = "5";
                //worksheet.Cells[2, 6].Value = "6";
                //worksheet.Cells[2, 7].Value = "7";

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.CustomerName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.LineName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.StationName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.StartTime.ToString("HH:00") + "-" + currentRecord.StartTime.AddHours(1).ToString("HH:00");
                    worksheet.Cells[index + 2, 6].Value = currentRecord.SerialNumber;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.StartTime.ToString("yyyy-MM-dd HH:mm");
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

            //return Content(result, "application/json");
        }

        #endregion
        public string GetShiftTimeHead(int ShiftTimeID)
        {
            var apiUrl = string.Format("GoldenLine/GetShiftTimeAPI?ShiftTimeID={0}", ShiftTimeID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<GL_ShiftTimeDTO>(result);
            var currentDate = DateTime.Now;
            var startTime = Convert.ToDateTime(currentDate.ToString("yyyy-MM-dd") + " " + model.StartTime);
            var endTime = Convert.ToDateTime(currentDate.ToString("yyyy-MM-dd") + " " + model.End_Time);
            if (startTime > endTime)
            {
                endTime = endTime.AddDays(1);
            }

            int hours = Math.Abs(endTime.Hour - startTime.Hour);
            List<string> shiftHour = new List<string>();
            for (int i = 0; i < hours; i++)
            {
                shiftHour.Add(startTime.ToString("HH:mm") + '~' + startTime.AddHours(1).ToString("HH:mm"));
                startTime = startTime.AddHours(1);
            }

            return JsonConvert.SerializeObject(shiftHour);
        }

        public ActionResult ShiftTime()
        {
            // a copy...
            Fixture_PartVM currentVM = new Fixture_PartVM();
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

            return View(currentVM);
        }

        public ActionResult RestTime(int ShiftID)
        {
            // a copy...
            Fixture_PartVM currentVM = new Fixture_PartVM();
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
            ViewBag.ShiftID = ShiftID;
            return View(currentVM);
        }

     
        public ActionResult GetRestTimeList(int ShiftID, Page page)
        {
            string apiUrl = "GoldenLine/GetRestTimeList";

            RestTimeQueryViewModel vm = new RestTimeQueryViewModel();
            vm.ShiftID = ShiftID;
            HttpResponseMessage response = APIHelper.APIPostAsync(vm, page, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetShiftTimeList(Page page, GoldenLineNormalQueryViewModel vm)
        {
            string apiUrl = "GoldenLine/GetShiftTime";

            if(vm.Plant_Organization_UID==0)
            {
                vm.Plant_Organization_UID = GetPlantOrgUid();
            }
         
            HttpResponseMessage response = APIHelper.APIPostAsync(vm, page, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        [HttpPost]
        public ActionResult GetShiftTimeByID(int id)
        {
            string apiUrl = "GoldenLine/GetShiftTimeByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", id), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        [HttpPost]
        public ActionResult RemoveShiftTimeByID(int id)
        {
            string apiUrl = "GoldenLine/RemoveShiftTimeByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", id), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        [HttpPost]
        public string AddOrUpdateGLShiftTime(GL_ShiftTimeDTO dto, bool isEdit)
        {
            string strresult = "";
            //获取所有版次
            string apiShiftUrl = "GoldenLine/GetAllShiftTimesAPI";
            var responMessage = APIHelper.APIGetAsync(apiShiftUrl);
            var resultShift = responMessage.Content.ReadAsStringAsync().Result;
            var shiftDTOs = JsonConvert.DeserializeObject<List<GL_ShiftTimeDTO>>(resultShift);
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            DateTime dateNow = DateTime.Now;
            try
            {
                Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + dto.StartTime));
            }
            catch (Exception ex)
            {
                return strresult = "班次开始时间错误，请重新填写.小时数小于24，分钟数小于60.";
            }
            try
            {
                Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + dto.End_Time));
            }
            catch (Exception ex)
            {
                return strresult = "班次结束时间错误，请重新填写.小时数小于24，分钟数小于60.";
            }

            if (isEdit)
            {
                var shiftallDTOs = shiftDTOs.Where(o => o.Plant_Organization_UID == dto.Plant_Organization_UID && o.BG_Organization_UID == dto.BG_Organization_UID && o.IsEnabled == dto.IsEnabled && o.ShiftTimeID != dto.ShiftTimeID).ToList();
                bool isRepeat = GetIsRepeat(shiftallDTOs, dto);
                if (isRepeat == true)
                {
                    return strresult = "班次错误，添加的时间段和现有班次时间段有交集。";
                }
                // post & submit to api
                var shiftDTO = shiftDTOs.Where(o => o.ShiftTimeID == dto.ShiftTimeID).ToList();
                var shiftDTO1 = shiftDTOs.Where(o => o.Plant_Organization_UID == dto.Plant_Organization_UID && o.BG_Organization_UID == dto.BG_Organization_UID && o.Shift == dto.Shift).ToList();
                if (shiftDTO != null && shiftDTO.Count > 0)
                {
                    if (  shiftDTO1 != null && shiftDTO1.Count > 0)
                    {
                        if (shiftDTO.Count == shiftDTO1.Count)
                            if (shiftDTO[0].ShiftTimeID == shiftDTO1[0].ShiftTimeID)
                            {
                                string apiUrl = "GoldenLine/AddOrUpdateShiftTime";
                                HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                                var result = response.Content.ReadAsStringAsync().Result;
                            }
                            else
                            {
                                return strresult = "班次重复，已经有相同名称的班次。";
                            }
                    }else
                    {
                        string apiUrl = "GoldenLine/AddOrUpdateShiftTime";
                        HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                        var result = response.Content.ReadAsStringAsync().Result;
                    }
               
                }

            }
            else
            {
                var shiftallDTOs = shiftDTOs.Where(o => o.Plant_Organization_UID == dto.Plant_Organization_UID && o.BG_Organization_UID == dto.BG_Organization_UID && o.IsEnabled == dto.IsEnabled).ToList();
                bool isRepeat = GetIsRepeat(shiftallDTOs, dto);

                if (isRepeat == true)
                {
                    return strresult = "班次错误，添加的时间段和现有班次时间段有交集。";
                }
                var shiftDTO = shiftDTOs.Where(o => o.Plant_Organization_UID == dto.Plant_Organization_UID && o.BG_Organization_UID == dto.BG_Organization_UID && o.Shift == dto.Shift).ToList();
                if (shiftDTO != null && shiftDTO.Count > 0)
                {
                    return strresult = "班次添加重复，已经有相同名称的班次。";
                }
                //验证时间段数据
                dto.ShiftTimeID = 0;
                // post & submit to api
                string apiUrl = "GoldenLine/AddOrUpdateShiftTime";
                HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                strresult = response.Content.ReadAsStringAsync().Result;
            }
            return strresult;
        }



        /// <summary>
        /// 判断时间和班次是否有交集
        /// </summary>
        /// <param name="shiftallDTOs"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool GetRestShiftIsRepeat(GL_ShiftTimeDTO  ST, GL_RestTimeDTO dto)
        {
            bool isRepeat = false;
            DateTime dateNow = DateTime.Now;
            DateTime StartTime = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + dto.StartTime));
            DateTime EndTime = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + dto.EndTime));
            if (StartTime > EndTime)
            {
                EndTime = EndTime.AddDays(1);
            }
          
              //获取班次的开始与结束时间
                DateTime StartItem = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + ST.StartTime));
                DateTime EndItem = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + ST.End_Time));
                if (StartItem > EndItem)
                {
                    EndItem = EndItem.AddDays(1);
                }

                //如果休息时间的开始时间早于班次的开始时间 或者 休息时间的结束时间 晚与班次的结束时间说明有异常
                if (StartTime < StartItem  || EndTime > EndItem)
                {
                    return isRepeat = true;
                }

        
            return isRepeat;

        }

        /// <summary>
        /// 判断时间是否有交集
        /// </summary>
        /// <param name="shiftallDTOs"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool GetRestIsRepeat(List<GL_RestTimeDTO> shiftallDTOs, GL_RestTimeDTO dto )
        {
            bool isRepeat = false;
            DateTime dateNow = DateTime.Now;
            DateTime StartTime = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + dto.StartTime));
            DateTime EndTime = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + dto.EndTime));
            if (StartTime > EndTime)
            {
                EndTime = EndTime.AddDays(1);
            }
            foreach (var item in shiftallDTOs)
            {
                if(item.RestID==dto.RestID)
                {
                    continue;
                }
                DateTime StartItem = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + item.StartTime));
                DateTime EndItem = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + item.EndTime));

                if (StartItem > EndItem)
                {
                    EndItem = EndItem.AddDays(1);
                }

                if (StartTime > StartItem && StartTime <= EndItem)
                {
                    return isRepeat = true;
                }

                if (EndTime > StartItem && EndTime <= EndItem)
                {
                    return isRepeat = true;
                }
                if (StartTime <= StartItem && EndTime >= EndItem)
                {
                    return isRepeat = true;
                }

            }

            return isRepeat;

        }


        public bool GetIsRepeat(List<GL_ShiftTimeDTO> shiftallDTOs, GL_ShiftTimeDTO dto)
        {
            bool isRepeat = false;
            DateTime dateNow = DateTime.Now;
            DateTime StartTime = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + dto.StartTime));
            DateTime EndTime = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + dto.End_Time));
            if (StartTime > EndTime)
            {
                EndTime = EndTime.AddDays(1);
            }
            foreach (var item in shiftallDTOs)
            {
                DateTime StartItem = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + item.StartTime));
                DateTime EndItem = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + item.End_Time));

                if (StartItem > EndItem)
                {
                    EndItem = EndItem.AddDays(1);
                }

                if (StartTime > StartItem && StartTime < EndItem)
                {
                    return isRepeat = true;
                }

                if (EndTime > StartItem && EndTime < EndItem)
                {
                    return isRepeat = true;
                }
                if (StartTime <= StartItem && EndTime >= EndItem)
                {
                    return isRepeat = true;
                }    

            }

            return isRepeat;

        }

        #region GL 4Q
        /// <summary>
        /// GL 4Q Report 报表
        /// </summary>
        /// <param name="searchParmm"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GLFourQReport()
        {
            Fixture_PartVM currentVM = new Fixture_PartVM();
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

            //ViewBag.Plant_UID = Plant_UID;
            //ViewBag.BG_UID = BG_UID;
            //ViewBag.customerId = customerId;
            //ViewBag.lineName = lineName;
            //ViewBag.stationName = stationName;
            //ViewBag.MachineName = MachineName;
            //ViewBag.ShiftID = ShiftID;
            //ViewBag.startTime = startTime;
            //ViewBag.endTime = endTime;
            //ViewBag.OrderByRule = OrderByRule;
            //ViewBag.isFromPhotoReport = isFromPhotoReport;

            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            return View("GLFourQReport", currentVM);
        }

        public ActionResult GetDownTimeRecord(GLFourQParamModel searchParmm)
        {
            var apiUrl = string.Format("GoldenLine/GetDownTimeRecordAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFourQDTTypeDetail(GLFourQParamModel searchParmm)
        {
            var apiUrl = string.Format("GoldenLine/GetFourQDTTypeDetailAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetPaynterChartDetial(GLFourQParamModel searchParmm)
        {
            var apiUrl = string.Format("GoldenLine/GetPaynterChartDetialAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFourQActionInfo(GLFourQParamModel searchParmm, Page page)
        {
            var apiUrl = string.Format("GoldenLine/GetFourQActionInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion

        #region  会议信息meetingtypeinfo
        public ActionResult GL_MeetingTypeInfo()
        {
            Fixture_DefectCodeVM currentVM = new Fixture_DefectCodeVM();
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
            //ViewBag.PageTitle = "异常原因维护";
            return View("GL_MeetingTypeInfo", currentVM);
        }

        public ActionResult QueryGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }

            var apiUrl = string.Format("GoldenLine/QueryGL_MeetingTypeInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        ///   
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public ActionResult AddGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO serchModel)
        {
            serchModel.Modified_UID = this.CurrentUser.AccountUId;
            serchModel.Modified_Date = DateTime.Now;
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("GoldenLine/AddGL_MeetingTypeInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult UpdateGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO serchModel)
        {
            serchModel.Modified_UID = this.CurrentUser.AccountUId;
            serchModel.Modified_Date = DateTime.Now;
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("GoldenLine/UpdateGL_MeetingTypeInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteGL_MeetingTypeInfo(string meetingTypeInfo_UID)
        {
            var apiUrl = string.Format("GoldenLine/DeleteGL_MeetingTypeInfoAPI?meetingTypeInfo_UID={0}", meetingTypeInfo_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetGL_MeetingTypeInfoById(string uid)
        {
            var apiUrl = string.Format("GoldenLine/GetGL_MeetingTypeInfoByIdAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetMeetingTypeName(string plantUid, string bgUid, string funplantUid)
        {
            var apiUrl = string.Format("GoldenLine/GetMeetingTypeNameAPI?plantUid={0}&bgUid={1}&funplantUid={2}", int.Parse(plantUid), int.Parse(bgUid), string.IsNullOrEmpty(funplantUid) ? 0 : int.Parse(funplantUid));
            HttpResponseMessage response = APIHelper.APIGetAsync(apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region ActionTask
        public ActionResult ActionTask()
        {
            Fixture_DefectCodeVM currentVM = new Fixture_DefectCodeVM();
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
            return View("ActionTask", currentVM);
        }

        public ActionResult Get_GL_ActionTasker(GL_ActionTaskDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }

            var apiUrl = string.Format("GoldenLine/Get_GL_ActionTaskerAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string Save_GL_ActionTasker(GL_ActionTaskDTO vm)
        {
            vm.Created_UID = this.CurrentUser.AccountUId;
            vm.Modified_UID = this.CurrentUser.AccountUId;
            vm.Modified_Date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            vm.Created_Date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            var url = string.Format("GoldenLine/Add_GL_ActionTaskerAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string Update_GL_ActionTasker(GL_ActionTaskDTO vm)
        {
            vm.Created_UID = this.CurrentUser.AccountUId;
            vm.Modified_UID = this.CurrentUser.AccountUId;
            vm.Modified_Date = DateTime.Now;
            vm.Created_Date = DateTime.Now;
            var url = string.Format("GoldenLine/Update_GL_ActionTaskerAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public ActionResult Get_GL_ActionTaskerById(int ActionTasker_ID)
        {
            var apiUrl = string.Format("GoldenLine/Get_GL_ActionTaskerByIdAPI?ActionTasker_ID={0}", ActionTasker_ID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult Delete_GL_ActionTaskerById(int ActionTasker_ID)
        {
            var apiUrl = string.Format("GoldenLine/Delete_GL_ActionTaskerByIdAPI?ActionTasker_ID={0}", ActionTasker_ID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        
        public ActionResult GetMetricName(string plantUid, string bgUid, string funplantUid)
        {
            var apiUrl = string.Format("GoldenLine/GetMetricNameAPI?plantUid={0}&bgUid={1}&funplantUid={2}", int.Parse(plantUid), int.Parse(bgUid), string.IsNullOrEmpty(funplantUid) ? 0 : int.Parse(funplantUid));
            HttpResponseMessage response = APIHelper.APIGetAsync(apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion

        #region  MetricInfo
        public ActionResult GL_MetricInfo()
        {
            Fixture_DefectCodeVM currentVM = new Fixture_DefectCodeVM();
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
            return View("GL_MetricInfo", currentVM);
        }

        public ActionResult QueryMetricInfo(GL_MetricInfoDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }

            var apiUrl = string.Format("GoldenLine/QueryMetricInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        ///   增加Metrice 的基本信息
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public ActionResult AddMetricInfoInfo(GL_MetricInfoDTO serchModel)
        {
            serchModel.Modified_UID = this.CurrentUser.AccountUId;
            serchModel.Modified_Date = DateTime.Now;
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("GoldenLine/AddMetricInfoInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult UpdateMetricInfo(GL_MetricInfoDTO serchModel)
        {
            serchModel.Modified_UID = this.CurrentUser.AccountUId;
            serchModel.Modified_Date = DateTime.Now;
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("GoldenLine/UpdateMetricInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteMetricInfo(string metricInfo_Uid)
        {
            var apiUrl = string.Format("GoldenLine/DeleteMetricInfoAPI?metricInfo_Uid={0}", metricInfo_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetMetricInfoByIdAPI(string uid)
        {
            var apiUrl = string.Format("GoldenLine/GetMetricInfoByIdAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddOrEditMetricInfo(GL_MetricInfoDTO dto, bool isEdit)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = string.Format("GoldenLine/AddOrEditMetricInfoAPI?isEdit={0}", isEdit);
            HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }

        #endregion

        #region LineGroup Add By Roy 2018/12/24
        public ActionResult LineGroup()
        {
            return View();
        }
        
        

        /// <summary>
        /// get paged records of GroupLines by query conditions
        /// </summary>
        /// <param name="search">query conditions</param>
        /// <param name="page">page info, auto fill by front-end</param>
        /// <returns>json of paged records</returns>
        public ActionResult QueryGroupLines(GL_LineModelSearch search, Page page)
        {
            var apiUrl = "GoldenLine/QueryGroupLinesAPI";

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult QueryGroupLine(int uid)
        {
            var apiUrl = string.Format("GoldenLine/QueryGroupLineAPI?uid={0}", uid);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult GetPlantsUseSettingsAPI()
        {
            int PlantOrgUid = 1;
            var orgs = CurrentUser.GetUserInfo.RoleList;
            if (CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count == 0)
            {
                PlantOrgUid = 0;
            }

            foreach (SystemRoleDTO item in orgs)
            {
                if (item.Role_ID == "SystemAdmin")
                {
                    PlantOrgUid = 0;
                }
            }
            if (PlantOrgUid == 1)
                PlantOrgUid = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID == null ? 0 : (int)CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
            var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", PlantOrgUid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 获取当前用户的功能厂
        /// </summary>
        /// <param name="plant_OrganizationUID"></param>
        /// <returns></returns>
        public ActionResult GetOPTypeUseFixtureAPI(int plant_OrganizationUID)
        {
            int organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }

            var apiUrl = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", plant_OrganizationUID, organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFunPlantUseFixtureAPI(int Optype, string Optypes = "")
        {
            var apiUrl = string.Format("Fixture/GetFunPlantByOPTypeAPI?Optype={0}&Optypes={1}", Optype, Optypes);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetGroupLine(int? oporgid, int? Optypes,int? opFunPlant,int? customerId)
        {
            var apiUrl = string.Format("GoldenLine/GetGroupLineAPI?oporgid={0}&Optypes={1}&opFunPlant={2}&customerId={3}", oporgid, Optypes, opFunPlant, customerId);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetSubLine(int oporgid, int optype)
        {
            var apiUrl = string.Format("GoldenLine/GetSubLineAPI?Oporgid={0}&Optype={1}", oporgid, optype);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddGroupLine(string jsonGroupLine)
        {

            var apiUrl = "GoldenLine/AddGroupLineAPI";
            var entity = JsonConvert.DeserializeObject<GL_LineGroupDTO>(jsonGroupLine);
            entity._Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult ModifyGroupLine(string jsonGroupLine)
        {
            var apiUrl = "GoldenLine/ModifyGroupLineAPI";
            var entity = JsonConvert.DeserializeObject<GL_LineGroupDTO>(jsonGroupLine);
            entity._Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult RemoveGroupLine(int uid)
        {
            var apiUrl = string.Format("GoldenLine/RemoveGroupLineAPI?uid={0}", uid);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult AddSubToGroup(string jsonGroupLine)
        {
            var apiUrl = "GoldenLine/AddSubToGroupAPI";
            var entity = JsonConvert.DeserializeObject<GL_LineGroupDTO>(jsonGroupLine);
            entity._Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        #endregion LineGroup

        public ActionResult LineSetup()
        {
            // a copy...
            Fixture_PartVM currentVM = new Fixture_PartVM();
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

            return View(currentVM);
        }

        [HttpPost]
        public ActionResult GetCustomerList()
        {
            string apiUrl = "GoldenLine/GetCustomer";
            GoldenLineNormalQueryViewModel vm = new GoldenLineNormalQueryViewModel();
            vm.Plant_Organization_UID = 35;
            vm.FunPlant_Organization_UID = null;
            vm.BG_Organization_UID = 35;
            HttpResponseMessage response = APIHelper.APIPostAsync(vm, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetLine(Page page, GoldenLineNormalQueryViewModel vm)
        {
            string apiUrl = "GoldenLine/GetLine";
            //if(vm.LineID==0)
            //{
            //    vm.IsEnabled = true;
            //}
            if(vm.Plant_Organization_UID==0)
            {
                vm.Plant_Organization_UID = GetPlantOrgUid();
            }

            HttpResponseMessage response = APIHelper.APIPostAsync(vm, page, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        [HttpPost]
        public ActionResult GetLineByID(int id)
        {
            string apiUrl = "GoldenLine/GetLineByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", id), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        [HttpPost]
        public string AddOrUpdateLine(GL_LineDTO dto, bool isEdit)
        {
            //获取所有线别
            string apiShiftUrl = "GoldenLine/GetLinesAllAPI";
            var responMessage = APIHelper.APIGetAsync(apiShiftUrl);
            var resultShift = responMessage.Content.ReadAsStringAsync().Result;
            var gL_LineDTOs = JsonConvert.DeserializeObject<List<GL_LineDTO>>(resultShift);
            string strresult = "";
            //dto.IsEnabled = true;
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            if(dto.CustomerID==0)
            {
                return strresult = "请选择专案。";
            }

            //负责人
            if (dto.GL_LineShiftResposibleUserList != null)
            {
                foreach (var item in dto.GL_LineShiftResposibleUserList)
                {
                    item.Modified_UID = CurrentUser.AccountUId;
                    item.Modified_Date = DateTime.Now;
                }
            }

            if (isEdit)
            {
                var gL_LineDTO = gL_LineDTOs.Where(o => o.LineID == dto.LineID).ToList();
                var gL_LineDTO1 = gL_LineDTOs.Where(o => o.ProjectName == dto.ProjectName && o.LineName == dto.LineName).ToList();

                if (gL_LineDTO != null && gL_LineDTO.Count > 0)
                {
                    if (gL_LineDTO1 != null && gL_LineDTO1.Count > 0)
                    {
                        if (gL_LineDTO1.Count == gL_LineDTO.Count)
                        {
                            if (gL_LineDTO[0].LineID == gL_LineDTO1[0].LineID)
                            {
                                dto.Phase = dto.Phase ?? "";
                                dto.MESLineName = dto.MESLineName ?? "";
                                //dto.Seq = 1;
                                // post & submit to api
                                string apiUrl = "GoldenLine/AddOrUpdateLine";
                                HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                                var result = response.Content.ReadAsStringAsync().Result;
                            }
                            else
                            {
                                return strresult = "此专案下线别重复，已经有相同名称的线别。";
                            }
                        }
                        else
                        {
                            return strresult = "此专案下线别重复，已经有相同名称的线别。";
                        }

                    }
                    else
                    {
                        dto.Phase = dto.Phase ?? "";
                        dto.MESLineName = dto.MESLineName ?? "";
                        //dto.Seq = 1;
                        // post & submit to api
                        string apiUrl = "GoldenLine/AddOrUpdateLine";
                        HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                        var result = response.Content.ReadAsStringAsync().Result;

                    }

                }
            }
            else
            {
                var newgL_LineDTOs = gL_LineDTOs.Where(o => o.ProjectName == dto.ProjectName && o.LineName == dto.LineName).ToList();
                if (newgL_LineDTOs != null && newgL_LineDTOs.Count > 0)
                {
                    strresult = "此专案下面已经有相同名称的线别，不能重复添加。";
                }
                else
                {

                    dto.LineID = 0;
                    dto.Phase = dto.Phase ?? "";
                    dto.MESLineName = dto.MESLineName ?? "";
                    //dto.Seq = 1;
                    // post & submit to api
                    string apiUrl = "GoldenLine/AddOrUpdateLine";
                    HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                    var result = response.Content.ReadAsStringAsync().Result;
                }
            }
            return strresult;
        }

        [HttpPost]
        public ActionResult RemoveLineByID(int id)
        {


            string apiUrl = "GoldenLine/RemoveLineByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", id), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult FlowChart(int LineID)
        {
            string apiUrl = "GoldenLine/GetLineByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", LineID), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            GL_LineDTO dto = JsonConvert.DeserializeObject<GL_LineDTO>(result);

            return View(dto);
        }
        [HttpPost]
        public ActionResult AddOrUpdateGLStation(GL_StationDTO dto, bool isEdit)
        {
            dto.IsEnabled = true;
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;

            if (isEdit)
            {
                // post & submit to api
                string apiUrl = "GoldenLine/AddOrUpdateStation";
                HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                var result = response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                // post & submit to api
                string apiUrl = "GoldenLine/AddOrUpdateStation";
                HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                var result = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("FlowChart", new { LineID = dto.LineID });
        }
        public ActionResult UploadFlowChartFile(HttpPostedFileBase uploadfile, int LineID, int Plant_Organization_UID, int BG_Organization_UID, int? FunPlant_Organization_UID, int CustomerID)
        {
            //string errorInfo = string.Empty;
            try
            {
                List<GL_StationDTO> newStationDTOs = new List<GL_StationDTO>();
                var apiUrl = string.Format("GoldenLine/GetStationDTOAPI?LineId={0}", LineID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var StationDTOresult = responMessage.Content.ReadAsStringAsync().Result;
                //得到导入之前的GL_Station
                List<GL_StationDTO> OldStationDTOs = JsonConvert.DeserializeObject<List<GL_StationDTO>>(StationDTOresult);

                using (var xlsPackage = new OfficeOpenXml.ExcelPackage(uploadfile.InputStream))
                {
                    bool IsBirth = false;
                    bool IsOutput = false;
                    bool IsOne = false;
                    var ws = xlsPackage.Workbook.Worksheets.FirstOrDefault();
                    int totalRows = ws.Dimension.End.Row;
                    for (int rowIdx = 2; rowIdx <= totalRows; rowIdx++)
                    {
                        // site name
                        string SeqStr = (ws.Cells[rowIdx, 1].Value ?? "").ToString();
                        string StationNameStr = (ws.Cells[rowIdx, 2].Value ?? "").ToString();
                        string MESStationNameStr = (ws.Cells[rowIdx, 3].Value ?? "").ToString();
                        string CycleTimeStr = (ws.Cells[rowIdx, 4].Value ?? "").ToString();
                        string IsBirthStr = (ws.Cells[rowIdx, 5].Value ?? "").ToString();
                        string IsOutputStr = (ws.Cells[rowIdx, 6].Value ?? "").ToString();
                        string IsTestStr = (ws.Cells[rowIdx, 7].Value ?? "").ToString();
                        string IsOneStr = (ws.Cells[rowIdx, 8].Value ?? "").ToString();
                        string DashboardTarget = (ws.Cells[rowIdx, 9].Value ?? "").ToString();
                        GL_StationDTO entity = new GL_StationDTO();
                        int value00 = 0;
                        if (int.TryParse(SeqStr, out value00))
                            entity.Seq = value00;
                        entity.StationName = StationNameStr.Trim();
                        entity.MESStationName = MESStationNameStr.Trim();
                        decimal value02 = 0;
                        if (decimal.TryParse(CycleTimeStr, out value02))
                            entity.CycleTime = Math.Round(value02, 2);
                        if (IsBirth == false)
                        {
                            entity.IsBirth = IsBirthStr.ToUpper().IndexOf('Y') >= 0;
                            if (entity.IsBirth == true)
                            {
                                IsBirth = true;
                            }

                        }
                        else
                        {
                            entity.IsBirth = false;
                        }

                        if (IsOutput == false)
                        {
                            entity.IsOutput = IsOutputStr.ToUpper().IndexOf('Y') >= 0;
                            if (entity.IsOutput == true)
                            {
                                IsOutput = true;
                            }

                        }
                        else
                        {
                            entity.IsOutput = false;
                        }
                        entity.IsTest = IsTestStr.ToUpper().IndexOf('Y') >= 0;
                        if (IsOne == false)
                        {
                            entity.IsOne = IsOneStr.ToUpper().IndexOf('Y') >= 0;
                            if (entity.IsOne == true)
                            {
                                IsOne = true;
                            }

                        }
                        else
                        {
                            entity.IsOne = false;
                        }
                        entity.IsEnabled = true;
                        entity.LineID = LineID;
                        entity.Plant_Organization_UID = Plant_Organization_UID;
                        entity.BG_Organization_UID = BG_Organization_UID;
                        entity.FunPlant_Organization_UID = FunPlant_Organization_UID;

                        entity.Modified_UID = CurrentUser.AccountUId;
                        entity.Modified_Date = DateTime.Now;
                        entity.DashboardTarget = DashboardTarget;
                        var StationDTO = OldStationDTOs.FirstOrDefault(o => o.Plant_Organization_UID == Plant_Organization_UID && o.BG_Organization_UID == BG_Organization_UID && o.LineID == LineID && o.StationName == StationNameStr);

                        if (StationDTO != null)
                        {
                            entity.StationID = StationDTO.StationID;
                        }
                        else
                        {
                            entity.StationID = 0;
                        }

                        newStationDTOs.Add(entity);

                    }

                    //防单 处理
                    if (IsOutput == true)
                    {

                        var OldStationDTO = OldStationDTOs.FirstOrDefault(o => o.IsOutput == true);
                        var newStationDTO = newStationDTOs.FirstOrDefault(o => o.IsOutput == true);
                        if (OldStationDTO != null)
                        {
                            OldStationDTO.IsOutput = false;
                            OldStationDTO.Modified_UID = CurrentUser.AccountUId;
                            OldStationDTO.Modified_Date = DateTime.Now;
                            //还要做判断
                            if (OldStationDTO.StationID != newStationDTO.StationID)
                            {
                                newStationDTOs.Add(OldStationDTO);
                            }

                        }

                    }

                    if (IsBirth == true)
                    {
                        var OldStationDTO = OldStationDTOs.FirstOrDefault(o => o.IsBirth == true);
                        var newStationDTO = newStationDTOs.FirstOrDefault(o => o.IsBirth == true);
                        if (OldStationDTO != null)
                        {
                            OldStationDTO.IsBirth = false;
                            OldStationDTO.Modified_UID = CurrentUser.AccountUId;
                            OldStationDTO.Modified_Date = DateTime.Now;
                            //还要做判断
                            if (OldStationDTO.StationID != newStationDTO.StationID)
                            {
                                newStationDTOs.Add(OldStationDTO);
                            }
                        }
                    }

                    //插入表
                    var json = JsonConvert.SerializeObject(newStationDTOs);
                    var apiInsertVendorInfoUrl = string.Format("GoldenLine/InserOrUpdateStationsAPI");
                    HttpResponseMessage responMessageStation = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    var errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }

            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("FlowChart", new { LineID = LineID });
            //   return errorInfo;

        }
        public ActionResult BuildPlan(int LineID)
        {
            string apiUrl = "GoldenLine/GetLineByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", LineID), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            GL_LineDTO dto = JsonConvert.DeserializeObject<GL_LineDTO>(result);
            ViewBag.LineID = LineID;
            return View(dto);
        }
        public ActionResult GetDateTime(bool IsThisWork)
        {
            string startDate = string.Empty;
            string endDate = string.Empty;
           // DateTime dateNow = new DateTime(2018, 11, 11, 7, 59, 59);
            DateTime dateNow = DateTime.Now;
            DayOfWeek weekDay = dateNow.DayOfWeek;
            //DayOfWeek weekDay = DateTime.Now.DayOfWeek;

            if (IsThisWork)
            {

                if (weekDay.ToString() == "Sunday")
                {
                    endDate = dateNow.AddDays(DayOfWeek.Sunday - weekDay).ToShortDateString();
                    startDate = dateNow.AddDays(DayOfWeek.Sunday - weekDay - 6).ToShortDateString();

                }
                else
                {
                    startDate = dateNow.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                    endDate = dateNow.AddDays(DayOfWeek.Sunday - weekDay + 7).ToShortDateString();
                }

                //startDate = dateNow.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                //endDate = dateNow.AddDays(DayOfWeek.Sunday - weekDay + 7).ToShortDateString();
                //startDate = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                //endDate = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 7).ToShortDateString();
            }
            else
            {

                if (weekDay.ToString() == "Sunday")
                {
                    int changeDay = DayOfWeek.Sunday - weekDay + 1;
                    startDate = dateNow.AddDays(changeDay).ToShortDateString();

                    changeDay = DayOfWeek.Sunday - weekDay + 7;
                    endDate = dateNow.AddDays(changeDay).ToShortDateString();

                }
                else
                {
                    int changeDay = DayOfWeek.Sunday - weekDay + 8;
                    startDate = dateNow.AddDays(changeDay).ToShortDateString();

                    changeDay = DayOfWeek.Sunday - weekDay + 14;
                    endDate = dateNow.AddDays(changeDay).ToShortDateString();
                }

   
                //int changeDay = DayOfWeek.Sunday - weekDay + 8;
                //startDate = DateTime.Now.AddDays(changeDay).ToShortDateString();

                //changeDay = DayOfWeek.Sunday - weekDay + 15;
                //endDate = DateTime.Now.AddDays(changeDay).ToShortDateString();
            }

            string result = "From" + startDate + "To" + endDate;
            return Content(result);
        }


        #region    生产计划

        public FileResult DownloadPlanExcelAll(int id, string clintName)
        {
            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            switch (clintName)
            {
                case "js_btn_download_fl":
                    propertiesHead = GetNextWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAllAPI?LineID={0}&week=next", id);
                    break;
                case "js_btn_download_currentWK":
                    propertiesHead = GetCurrentWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAllAPI?LineID={0}&week=current", id);
                    break;
            }
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<GLBuildPlanDTO>>(result);

            if (list.Count() > 0)
            {

                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("生产计划");

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("PlanOutPut");
                    SetExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization;
                        worksheet.Cells[iRow, 3].Value = item.ProjectName;
                        worksheet.Cells[iRow, 4].Value = item.LineName;
                        worksheet.Cells[iRow, 5].Value = item.ShiftTime;
                        worksheet.Cells[iRow, 13].Value = item.LineID;
                        worksheet.Cells[iRow, 14].Value = item.ShiftTimeID;
                        iRow++;
                    }

                    //Excel最后一行行号
                    int maxRow = iRow - 1;
                    //设置列隐藏
                    worksheet.Column(13).Hidden = true;
                    worksheet.Column(14).Hidden = true;
                    //设置灰色背景
                    var colorRange = string.Format("A2:E{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                    //设置边框
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelPackage.Save();
                }
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
            else
            {
                return null;
            }

        }

        public FileResult DownloadPlanExcel(int id, string clintName)
        {
            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            switch (clintName)
            {
                case "js_btn_download_fl":
                    propertiesHead = GetNextWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAPI?LineID={0}&week=next", id);
                    break;
                case "js_btn_download_currentWK":
                    propertiesHead = GetCurrentWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAPI?LineID={0}&week=current", id);
                    break;
            }
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<GLBuildPlanDTO>>(result);

            if (list.Count() > 0)
            {

                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("生产计划");

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("PlanOutPut");
                    SetExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization;
                        worksheet.Cells[iRow, 3].Value = item.ProjectName;
                        worksheet.Cells[iRow, 4].Value = item.LineName;
                        worksheet.Cells[iRow, 5].Value = item.ShiftTime;
                        worksheet.Cells[iRow, 13].Value = id;
                        worksheet.Cells[iRow, 14].Value = item.ShiftTimeID;
                        iRow++;
                    }

                    //Excel最后一行行号
                    int maxRow = iRow - 1;
                    //设置列隐藏
                    worksheet.Column(13).Hidden = true;
                    worksheet.Column(14).Hidden = true;
                    //设置灰色背景
                    var colorRange = string.Format("A2:E{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                    //设置边框
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelPackage.Save();
                }
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
            else
            {
                return null;
            }

        }
        private void SetExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 5].Value = propertiesHead[4];
            worksheet.Cells[1, 6].Value = propertiesHead[5];
            worksheet.Cells[1, 7].Value = propertiesHead[6];
            worksheet.Cells[1, 8].Value = propertiesHead[7];
            worksheet.Cells[1, 9].Value = propertiesHead[8];
            worksheet.Cells[1, 10].Value = propertiesHead[9];
            worksheet.Cells[1, 11].Value = propertiesHead[10];
            worksheet.Cells[1, 12].Value = propertiesHead[11];
            worksheet.Cells[1, 13].Value = propertiesHead[12];
            worksheet.Cells[1, 14].Value = propertiesHead[13];
            //设置列宽

            for (int i = 1; i <= 14; i++)
            {

                if (i <= 5)
                {
                    worksheet.Column(i).Width = 17;
                }
                else
                {
                    worksheet.Column(i).Width = 23;
                }

            }

            worksheet.Cells["A1:L1"].Style.Font.Bold = true;
            worksheet.Cells["A1:L1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:L1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }
        private string[] GetNextWeekPlanHeadColumn()
        {
            var nextWeek = GetNextWeek(DateTime.Now);
            var propertiesHead = new[]
            {
                "厂区",
                "OP类型",
                "专案",
                "线别",
                "班别",
                string.Format("星期一({0})", nextWeek.Monday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期二({0})", nextWeek.Tuesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期三({0})", nextWeek.Wednesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期四({0})", nextWeek.Thursday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期五({0})", nextWeek.Friday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期六({0})", nextWeek.Saturday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期日({0})", nextWeek.Sunday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                "LineID",
                "ShiftTimeID"

            };
            return propertiesHead;
        }
        private string[] GetCurrentWeekPlanHeadColumn()
        {
            var currentWeek = GetCurrentWeek(DateTime.Now);
            var propertiesHead = new[]
            {
                "厂区",
                "OP类型",
                "专案",
                "线别",
                "班别",
                string.Format("星期一({0})", currentWeek.Monday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期二({0})", currentWeek.Tuesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期三({0})", currentWeek.Wednesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期四({0})", currentWeek.Thursday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期五({0})", currentWeek.Friday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期六({0})", currentWeek.Saturday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期日({0})", currentWeek.Sunday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                "LineID",
                "ShiftTimeID"

            };
            return propertiesHead;
        }
        private Week GetNextWeek(DateTime dt)
        {
            var strDT = dt.DayOfWeek.ToString();
            Week nextWeek = new Week();
            //获取下周一的日期
            switch (strDT)
            {
                case "Monday":
                    nextWeek.Monday = dt.AddDays(7);
                    break;
                case "Tuesday":
                    nextWeek.Monday = dt.AddDays(6);
                    break;
                case "Wednesday":
                    nextWeek.Monday = dt.AddDays(5);
                    break;
                case "Thursday":
                    nextWeek.Monday = dt.AddDays(4);
                    break;
                case "Friday":
                    nextWeek.Monday = dt.AddDays(3);
                    break;
                case "Saturday":
                    nextWeek.Monday = dt.AddDays(2);
                    break;
                case "Sunday":
                    nextWeek.Monday = dt.AddDays(1);
                    break;
            }
            nextWeek.Tuesday = nextWeek.Monday.AddDays(1);
            nextWeek.Wednesday = nextWeek.Monday.AddDays(2);
            nextWeek.Thursday = nextWeek.Monday.AddDays(3);
            nextWeek.Friday = nextWeek.Monday.AddDays(4);
            nextWeek.Saturday = nextWeek.Monday.AddDays(5);
            nextWeek.Sunday = nextWeek.Monday.AddDays(6);

            return nextWeek;
        }
        private Week GetCurrentWeek(DateTime dt)
        {
            var strDT = dt.DayOfWeek.ToString();
            Week currentWeek = new Week();
            switch (strDT)
            {
                case "Monday":
                    currentWeek.Monday = dt;
                    break;
                case "Tuesday":
                    currentWeek.Monday = dt.AddDays(-1);
                    break;
                case "Wednesday":
                    currentWeek.Monday = dt.AddDays(-2);
                    break;
                case "Thursday":
                    currentWeek.Monday = dt.AddDays(-3);
                    break;
                case "Friday":
                    currentWeek.Monday = dt.AddDays(-4);
                    break;
                case "Saturday":
                    currentWeek.Monday = dt.AddDays(-5);
                    break;
                case "Sunday":
                    currentWeek.Monday = dt.AddDays(-6);
                    break;
            }
            currentWeek.Tuesday = currentWeek.Monday.AddDays(1);
            currentWeek.Wednesday = currentWeek.Monday.AddDays(2);
            currentWeek.Thursday = currentWeek.Monday.AddDays(3);
            currentWeek.Friday = currentWeek.Monday.AddDays(4);
            currentWeek.Saturday = currentWeek.Monday.AddDays(5);
            currentWeek.Sunday = currentWeek.Monday.AddDays(6);

            return currentWeek;
        }
        public string ImportPlanExcel(HttpPostedFileBase upload_excel, int LineID, string hid_currentOrNextWeek)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {
                    errorInfo = "没有worksheet内容";
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] { };
                Week week = new Week();
                switch (hid_currentOrNextWeek)
                {
                    case "nextWeek":
                        propertiesHead = GetNextWeekPlanHeadColumn();
                        week = GetNextWeek(DateTime.Now);
                        break;
                    case "currentWeek":
                        propertiesHead = GetCurrentWeekPlanHeadColumn();
                        week = GetCurrentWeek(DateTime.Now);
                        break;
                }

                bool allColumnsAreError = false;

                //验证Excel的表头是否正确
                allColumnsAreError = validExcelTitleIsErrorTwo(worksheet, propertiesHead, 1, totalColumns);
                if (allColumnsAreError)
                {
                    errorInfo = "Excel格式不正确";
                    return errorInfo;
                }

                List<GLBuildPlanDTO> mgDataList = new List<GLBuildPlanDTO>();
                //Excel行号
                int iRow = 2;
                //Excel列号
                int iColumn;
                int j = 6;
                //如果Excel导入的计划是本周的计划，则导入的数据从当天的日期开始到往后导入到数据库中
                if (hid_currentOrNextWeek == "currentWeek")
                {
                    var strDT = DateTime.Now.DayOfWeek.ToString();
                    switch (strDT)
                    {
                        case "Monday":
                            j = 6;
                            break;
                        case "Tuesday":
                            j = 7;
                            break;
                        case "Wednesday":
                            j = 8;
                            break;
                        case "Thursday":
                            j = 9;
                            break;
                        case "Friday":
                            j = 10;
                            break;
                        case "Saturday":
                            j = 11;
                            break;
                        case "Sunday":
                            j = 12;
                            break;
                    }
                }

                for (iRow = 2; iRow <= totalRows; iRow++)
                {
                    if (allColumnsAreError)
                    {
                        break;
                    }
                    for (iColumn = j; iColumn <= 12; iColumn++)
                    {
                        GLBuildPlanDTO newMGDataItem = new GLBuildPlanDTO();

                        var lineID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 13].Value);
                        if (string.IsNullOrWhiteSpace(lineID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的线", iRow);
                            break;
                        }
                        var ShiftTimeID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 14].Value);
                        if (string.IsNullOrWhiteSpace(ShiftTimeID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的班别", iRow);
                            break;
                        }

                        var Product_Plan = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value);
                        //如果生产计划为空则默认为0
                        if (string.IsNullOrWhiteSpace(Product_Plan))
                        {
                            Product_Plan = "0";
                        }

                        if (!ValidIsDouble(Product_Plan))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行生产计划输入不正确", iRow);
                            break;
                        }

                        newMGDataItem.LineID = Convert.ToInt32(LineID);
                        newMGDataItem.ShiftTimeID = Convert.ToInt32(ShiftTimeID);
                        switch (iColumn)
                        {
                            case 6:
                                newMGDataItem.OutputDate = week.Monday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 7:
                                newMGDataItem.OutputDate = week.Tuesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 8:
                                newMGDataItem.OutputDate = week.Wednesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 9:
                                newMGDataItem.OutputDate = week.Thursday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 10:
                                newMGDataItem.OutputDate = week.Friday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 11:
                                newMGDataItem.OutputDate = week.Saturday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 12:
                                newMGDataItem.OutputDate = week.Sunday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                        }
                        newMGDataItem.PlanOutput = Convert.ToInt32(Product_Plan);
                        newMGDataItem.Created_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Created_Date = DateTime.Now;
                        newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Modified_Date = DateTime.Now;
                        mgDataList.Add(newMGDataItem);
                    }

                }

                if (allColumnsAreError)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("GoldenLine/ImportBuildPlansAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);

            }
            return errorInfo;
        }
        private bool ValidIsDouble(string result)
        {
            var validResult = false;
            int validDouble = 0;
            var isDouble = int.TryParse(result, out validDouble);
            if (isDouble)
            {
                validResult = true;
            }
            return validResult;
        }
        private bool validExcelTitleIsErrorTwo(ExcelWorksheet worksheet, string[] propertiesHead, int iRow, int totalColumns)
        {
            bool allColumnsAreError = false;
            for (var i = 1; i <= totalColumns; i++)
            {
                if (allColumnsAreError)
                {
                    break;
                }
                switch (i)
                {
                    case 1:
                        if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, i].Value) != "厂区")
                        {
                            allColumnsAreError = true;
                        }
                        break;
                    case 2:
                        if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, i].Value) != "OP类型")
                        {
                            allColumnsAreError = true;
                        }
                        break;
                    case 3:
                        if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, i].Value) != "专案")
                        {
                            allColumnsAreError = true;
                        }
                        break;
                    case 4:
                        if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, i].Value) != "线别")
                        {
                            allColumnsAreError = true;
                        }
                        break;
                    case 5:
                        if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, i].Value) != "班别")
                        {
                            allColumnsAreError = true;
                        }
                        break;
                    case 6: //星期一
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 5);
                        break;
                    case 7: //星期二
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 6);
                        break;
                    case 8: //星期三
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 7);
                        break;
                    case 9: //星期四
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 8);
                        break;
                    case 10: //星期五
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 9);
                        break;
                    case 11: //星期六
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 10);
                        break;
                    case 12: //星期日
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 11);
                        break;
                    case 13:
                        if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, i].Value) != "LineID")
                        {
                            allColumnsAreError = true;
                        }
                        break;
                    case 14:
                        if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, i].Value) != "ShiftTimeID")
                        {
                            allColumnsAreError = true;
                        }
                        break;
                    default:
                        continue;
                }
            }
            return allColumnsAreError;
        }
        private bool validExcelTitleIsError(ExcelWorksheet worksheet, string[] propertiesHead, int iRow, int iColumn, int column)
        {
            bool isError = false;
            if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value) != propertiesHead[column])
            {
                isError = true;
            }
            return isError;
        }
        public ActionResult QueryPlanData(int LineID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;
            if (IsThisWork)
            {
                if (weekDay.ToString() == "Sunday")
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay - 6).ToShortDateString();
                   // date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay).ToShortDateString();

                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                }
            }
            else
            {
                if (weekDay.ToString() == "Sunday")
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 1).ToShortDateString();
                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
                }

            }
            //if (IsThisWork)
            //{
            //    date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            //}
            //else
            //{
            //    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
            //}
            string api = "GoldenLine/QueryPlanDataAPI?LineID=" + LineID + "&date=" + date;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ImportPlanExcelAll(HttpPostedFileBase upload_excel, int LineID, string hid_currentOrNextWeek)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {
                    errorInfo = "没有worksheet内容";
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] { };
                Week week = new Week();
                switch (hid_currentOrNextWeek)
                {
                    case "nextWeek":
                        propertiesHead = GetNextWeekPlanHeadColumn();
                        week = GetNextWeek(DateTime.Now);
                        break;
                    case "currentWeek":
                        propertiesHead = GetCurrentWeekPlanHeadColumn();
                        week = GetCurrentWeek(DateTime.Now);
                        break;
                }

                bool allColumnsAreError = false;

                //验证Excel的表头是否正确
                allColumnsAreError = validExcelTitleIsErrorTwo(worksheet, propertiesHead, 1, totalColumns);
                if (allColumnsAreError)
                {
                    errorInfo = "Excel格式不正确";
                    return errorInfo;
                }

                List<GLBuildPlanDTO> mgDataList = new List<GLBuildPlanDTO>();
                //Excel行号
                int iRow = 2;
                //Excel列号
                int iColumn;
                int j = 6;
                //如果Excel导入的计划是本周的计划，则导入的数据从当天的日期开始到往后导入到数据库中
                if (hid_currentOrNextWeek == "currentWeek")
                {
                    var strDT = DateTime.Now.DayOfWeek.ToString();
                    switch (strDT)
                    {
                        case "Monday":
                            j = 6;
                            break;
                        case "Tuesday":
                            j = 7;
                            break;
                        case "Wednesday":
                            j = 8;
                            break;
                        case "Thursday":
                            j = 9;
                            break;
                        case "Friday":
                            j = 10;
                            break;
                        case "Saturday":
                            j = 11;
                            break;
                        case "Sunday":
                            j = 12;
                            break;
                    }
                }

                for (iRow = 2; iRow <= totalRows; iRow++)
                {
                    if (allColumnsAreError)
                    {
                        break;
                    }
                    for (iColumn = j; iColumn <= 12; iColumn++)
                    {
                        GLBuildPlanDTO newMGDataItem = new GLBuildPlanDTO();

                        var lineID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 13].Value);
                        if (string.IsNullOrWhiteSpace(lineID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的线", iRow);
                            break;
                        }
                        var ShiftTimeID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 14].Value);
                        if (string.IsNullOrWhiteSpace(ShiftTimeID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的班别", iRow);
                            break;
                        }

                        var Product_Plan = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value);
                        //如果生产计划为空则默认为0
                        if (string.IsNullOrWhiteSpace(Product_Plan))
                        {
                            Product_Plan = "0";
                        }

                        if (!ValidIsDouble(Product_Plan))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行生产计划输入不正确", iRow);
                            break;
                        }

                        newMGDataItem.LineID = Convert.ToInt32(lineID);
                        newMGDataItem.ShiftTimeID = Convert.ToInt32(ShiftTimeID);
                        switch (iColumn)
                        {
                            case 6:
                                newMGDataItem.OutputDate = week.Monday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 7:
                                newMGDataItem.OutputDate = week.Tuesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 8:
                                newMGDataItem.OutputDate = week.Wednesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 9:
                                newMGDataItem.OutputDate = week.Thursday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 10:
                                newMGDataItem.OutputDate = week.Friday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 11:
                                newMGDataItem.OutputDate = week.Saturday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 12:
                                newMGDataItem.OutputDate = week.Sunday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                        }
                        newMGDataItem.PlanOutput = Convert.ToInt32(Product_Plan);
                        newMGDataItem.Created_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Created_Date = DateTime.Now;
                        newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Modified_Date = DateTime.Now;
                        mgDataList.Add(newMGDataItem);
                    }

                }

                if (allColumnsAreError)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("GoldenLine/ImportBuildPlansAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);

            }
            return errorInfo;
        }
        public ActionResult SearchPlanDataByLineID(int LineID, int ShiftTimeID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;
            if (IsThisWork)
            {
                if (weekDay.ToString() == "Sunday")
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay - 6).ToShortDateString();
                    //date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay).ToShortDateString();

                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                }
            }
            else
            {
                if (weekDay.ToString() == "Sunday")
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 1).ToShortDateString();
                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
                }

            }
            //if (IsThisWork)
            //{
            //    date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            //}
            //else
            //{
            //    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
            //}
            string api = "GoldenLine/SearchPlanDataByLineIDAPI?LineID=" + LineID + "&ShiftTimeID=" + ShiftTimeID + "&date=" + date + "&WeekDay=" + weekDay.ToString();
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult EditBuildPlanInfo(string jsonStorages)
        {
            var entity = JsonConvert.DeserializeObject<GLHCActuaWM>(jsonStorages);
            Week week = new Week();
            if (!entity.IsThisWork)
            {
                week = GetNextWeek(DateTime.Now);
            }
            else
            {
                week = GetCurrentWeek(DateTime.Now);

            }

            List<GLBuildPlanDTO> mgDataList = new List<GLBuildPlanDTO>();
            for (int i = 0; i < 7; i++)
            {
                GLBuildPlanDTO newMGDataItem = new GLBuildPlanDTO();
                newMGDataItem.LineID = Convert.ToInt32(entity.LineID);
                newMGDataItem.ShiftTimeID = Convert.ToInt32(entity.ShiftTimeID);
                newMGDataItem.Created_UID = this.CurrentUser.AccountUId;
                newMGDataItem.Created_Date = DateTime.Now;
                newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                newMGDataItem.Modified_Date = DateTime.Now;
                switch (i)
                {
                    case 0:
                        newMGDataItem.PlanOutput = Convert.ToInt32(entity.MondayHCActua);
                        newMGDataItem.OutputDate = week.Monday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 1:
                        newMGDataItem.PlanOutput = Convert.ToInt32(entity.TuesdayHCActua);
                        newMGDataItem.OutputDate = week.Tuesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 2:
                        newMGDataItem.PlanOutput = Convert.ToInt32(entity.WednesdayHCActua);
                        newMGDataItem.OutputDate = week.Wednesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 3:
                        newMGDataItem.PlanOutput = Convert.ToInt32(entity.ThursdayHCActua);
                        newMGDataItem.OutputDate = week.Thursday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 4:
                        newMGDataItem.PlanOutput = Convert.ToInt32(entity.FridayHCActua);
                        newMGDataItem.OutputDate = week.Friday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 5:
                        newMGDataItem.PlanOutput = Convert.ToInt32(entity.SaterdayHCActua);
                        newMGDataItem.OutputDate = week.Saturday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 6:
                        newMGDataItem.PlanOutput = Convert.ToInt32(entity.SundayHCActua);
                        newMGDataItem.OutputDate = week.Sunday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                }

                mgDataList.Add(newMGDataItem);
            }
            var json = JsonConvert.SerializeObject(mgDataList);
            string api = string.Format("GoldenLine/ImportBuildPlansAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion

        #region 计划人力
        public ActionResult HCPlan(int LineID)
        {
            string apiUrl = "GoldenLine/GetLineByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", LineID), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            GL_LineDTO dto = JsonConvert.DeserializeObject<GL_LineDTO>(result);
            ViewBag.LineID = LineID;
            return View(dto);
        }
        public ActionResult QueryHCPlanData(int LineID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;
            if (IsThisWork)
            {
                if (weekDay.ToString() == "Sunday")
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay - 6).ToShortDateString();
                    //date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay).ToShortDateString();

                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                }
            }
            else
            {
                if (weekDay.ToString() == "Sunday")
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 1).ToShortDateString();
                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
                }

            }
            //if (IsThisWork)
            //{
            //    date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            //}
            //else
            //{
            //    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
            //}
            string api = "GoldenLine/QueryHCPlanDataAPI?LineID=" + LineID + "&date=" + date;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public FileResult DownloadHCPlanExcelAll(int id, string clintName)

        {
            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            switch (clintName)
            {
                case "js_btn_download_fl":
                    propertiesHead = GetNextWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAllAPI?LineID={0}&week=next", id);
                    break;
                case "js_btn_download_currentWK":
                    propertiesHead = GetCurrentWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAllAPI?LineID={0}&week=current", id);
                    break;
            }
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<GLBuildPlanDTO>>(result);

            if (list.Count() > 0)
            {

                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("人力计划");

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("PlanHC");
                    SetExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization;
                        worksheet.Cells[iRow, 3].Value = item.ProjectName;
                        worksheet.Cells[iRow, 4].Value = item.LineName;
                        worksheet.Cells[iRow, 5].Value = item.ShiftTime;
                        worksheet.Cells[iRow, 13].Value = item.LineID;
                        worksheet.Cells[iRow, 14].Value = item.ShiftTimeID;
                        iRow++;
                    }

                    //Excel最后一行行号
                    int maxRow = iRow - 1;
                    //设置列隐藏
                    worksheet.Column(13).Hidden = true;
                    worksheet.Column(14).Hidden = true;
                    //设置灰色背景
                    var colorRange = string.Format("A2:E{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                    //设置边框
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelPackage.Save();
                }
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
            else
            {
                return null;
            }

        }
        public FileResult DownloadHCPlanExcel(int id, string clintName)

        {
            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            switch (clintName)
            {
                case "js_btn_download_fl":
                    propertiesHead = GetNextWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAPI?LineID={0}&week=next", id);
                    break;
                case "js_btn_download_currentWK":
                    propertiesHead = GetCurrentWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAPI?LineID={0}&week=current", id);
                    break;
            }
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<GLBuildPlanDTO>>(result);

            if (list.Count() > 0)
            {

                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("人力计划");

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("PlanHC");
                    SetExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization;
                        worksheet.Cells[iRow, 3].Value = item.ProjectName;
                        worksheet.Cells[iRow, 4].Value = item.LineName;
                        worksheet.Cells[iRow, 5].Value = item.ShiftTime;
                        worksheet.Cells[iRow, 13].Value = id;
                        worksheet.Cells[iRow, 14].Value = item.ShiftTimeID;
                        iRow++;
                    }

                    //Excel最后一行行号
                    int maxRow = iRow - 1;
                    //设置列隐藏
                    worksheet.Column(13).Hidden = true;
                    worksheet.Column(14).Hidden = true;
                    //设置灰色背景
                    var colorRange = string.Format("A2:E{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                    //设置边框
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelPackage.Save();
                }
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
            else
            {
                return null;
            }

        }
        public string ImportHCPlanExcel(HttpPostedFileBase upload_excel, int LineID, string hid_currentOrNextWeek)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {
                    errorInfo = "没有worksheet内容";
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] { };
                Week week = new Week();
                switch (hid_currentOrNextWeek)
                {
                    case "nextWeek":
                        propertiesHead = GetNextWeekPlanHeadColumn();
                        week = GetNextWeek(DateTime.Now);
                        break;
                    case "currentWeek":
                        propertiesHead = GetCurrentWeekPlanHeadColumn();
                        week = GetCurrentWeek(DateTime.Now);
                        break;
                }

                bool allColumnsAreError = false;

                //验证Excel的表头是否正确
                allColumnsAreError = validExcelTitleIsErrorTwo(worksheet, propertiesHead, 1, totalColumns);
                if (allColumnsAreError)
                {
                    errorInfo = "Excel格式不正确";
                    return errorInfo;
                }

                List<GLBuildPlanDTO> mgDataList = new List<GLBuildPlanDTO>();
                //Excel行号
                int iRow = 2;
                //Excel列号
                int iColumn;
                int j = 6;
                //如果Excel导入的计划是本周的计划，则导入的数据从当天的日期开始到往后导入到数据库中
                if (hid_currentOrNextWeek == "currentWeek")
                {
                    var strDT = DateTime.Now.DayOfWeek.ToString();
                    switch (strDT)
                    {
                        case "Monday":
                            j = 6;
                            break;
                        case "Tuesday":
                            j = 7;
                            break;
                        case "Wednesday":
                            j = 8;
                            break;
                        case "Thursday":
                            j = 9;
                            break;
                        case "Friday":
                            j = 10;
                            break;
                        case "Saturday":
                            j = 11;
                            break;
                        case "Sunday":
                            j = 12;
                            break;
                    }
                }

                for (iRow = 2; iRow <= totalRows; iRow++)
                {
                    if (allColumnsAreError)
                    {
                        break;
                    }
                    for (iColumn = j; iColumn <= 12; iColumn++)
                    {
                        GLBuildPlanDTO newMGDataItem = new GLBuildPlanDTO();

                        var lineID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 13].Value);
                        if (string.IsNullOrWhiteSpace(lineID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的线", iRow);
                            break;
                        }
                        var ShiftTimeID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 14].Value);
                        if (string.IsNullOrWhiteSpace(ShiftTimeID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的班别", iRow);
                            break;
                        }

                        var Product_HCPlan = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value);
                        //如果生产计划为空则默认为0
                        if (string.IsNullOrWhiteSpace(Product_HCPlan))
                        {
                            Product_HCPlan = "0";
                        }

                        if (!ValidIsDouble(Product_HCPlan))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行生产计划输入不正确", iRow);
                            break;
                        }

                        newMGDataItem.LineID = Convert.ToInt32(LineID);
                        newMGDataItem.ShiftTimeID = Convert.ToInt32(ShiftTimeID);
                        switch (iColumn)
                        {
                            case 6:
                                newMGDataItem.OutputDate = week.Monday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 7:
                                newMGDataItem.OutputDate = week.Tuesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 8:
                                newMGDataItem.OutputDate = week.Wednesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 9:
                                newMGDataItem.OutputDate = week.Thursday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 10:
                                newMGDataItem.OutputDate = week.Friday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 11:
                                newMGDataItem.OutputDate = week.Saturday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 12:
                                newMGDataItem.OutputDate = week.Sunday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                        }
                        newMGDataItem.PlanHC = Convert.ToInt32(Product_HCPlan);
                        newMGDataItem.Created_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Created_Date = DateTime.Now;
                        newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Modified_Date = DateTime.Now;
                        mgDataList.Add(newMGDataItem);
                    }

                }

                if (allColumnsAreError)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("GoldenLine/ImportBuildHCPlansAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);

            }
            return errorInfo;
        }
        public string ImportHCPlanExcelAll(HttpPostedFileBase upload_excel, int LineID, string hid_currentOrNextWeek)
        {

            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {
                    errorInfo = "没有worksheet内容";
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] { };
                Week week = new Week();
                switch (hid_currentOrNextWeek)
                {
                    case "nextWeek":
                        propertiesHead = GetNextWeekPlanHeadColumn();
                        week = GetNextWeek(DateTime.Now);
                        break;
                    case "currentWeek":
                        propertiesHead = GetCurrentWeekPlanHeadColumn();
                        week = GetCurrentWeek(DateTime.Now);
                        break;
                }

                bool allColumnsAreError = false;

                //验证Excel的表头是否正确
                allColumnsAreError = validExcelTitleIsErrorTwo(worksheet, propertiesHead, 1, totalColumns);
                if (allColumnsAreError)
                {
                    errorInfo = "Excel格式不正确";
                    return errorInfo;
                }

                List<GLBuildPlanDTO> mgDataList = new List<GLBuildPlanDTO>();
                //Excel行号
                int iRow = 2;
                //Excel列号
                int iColumn;
                int j = 6;
                //如果Excel导入的计划是本周的计划，则导入的数据从当天的日期开始到往后导入到数据库中
                if (hid_currentOrNextWeek == "currentWeek")
                {
                    var strDT = DateTime.Now.DayOfWeek.ToString();
                    switch (strDT)
                    {
                        case "Monday":
                            j = 6;
                            break;
                        case "Tuesday":
                            j = 7;
                            break;
                        case "Wednesday":
                            j = 8;
                            break;
                        case "Thursday":
                            j = 9;
                            break;
                        case "Friday":
                            j = 10;
                            break;
                        case "Saturday":
                            j = 11;
                            break;
                        case "Sunday":
                            j = 12;
                            break;
                    }
                }

                for (iRow = 2; iRow <= totalRows; iRow++)
                {
                    if (allColumnsAreError)
                    {
                        break;
                    }
                    for (iColumn = j; iColumn <= 12; iColumn++)
                    {
                        GLBuildPlanDTO newMGDataItem = new GLBuildPlanDTO();

                        var lineID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 13].Value);
                        if (string.IsNullOrWhiteSpace(lineID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的线", iRow);
                            break;
                        }
                        var ShiftTimeID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 14].Value);
                        if (string.IsNullOrWhiteSpace(ShiftTimeID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的班别", iRow);
                            break;
                        }

                        var Product_HCPlan = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value);
                        //如果生产计划为空则默认为0
                        if (string.IsNullOrWhiteSpace(Product_HCPlan))
                        {
                            Product_HCPlan = "0";
                        }

                        if (!ValidIsDouble(Product_HCPlan))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行生产计划输入不正确", iRow);
                            break;
                        }

                        newMGDataItem.LineID = Convert.ToInt32(lineID);
                        newMGDataItem.ShiftTimeID = Convert.ToInt32(ShiftTimeID);
                        switch (iColumn)
                        {
                            case 6:
                                newMGDataItem.OutputDate = week.Monday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 7:
                                newMGDataItem.OutputDate = week.Tuesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 8:
                                newMGDataItem.OutputDate = week.Wednesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 9:
                                newMGDataItem.OutputDate = week.Thursday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 10:
                                newMGDataItem.OutputDate = week.Friday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 11:
                                newMGDataItem.OutputDate = week.Saturday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 12:
                                newMGDataItem.OutputDate = week.Sunday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                        }
                        newMGDataItem.PlanHC = Convert.ToInt32(Product_HCPlan);
                        newMGDataItem.Created_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Created_Date = DateTime.Now;
                        newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Modified_Date = DateTime.Now;
                        mgDataList.Add(newMGDataItem);
                    }

                }

                if (allColumnsAreError)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("GoldenLine/ImportBuildHCPlansAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);

            }
            return errorInfo;
        }

        public ActionResult SearchPlanHCDataByLineID(int LineID, int ShiftTimeID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;
            if (IsThisWork)
            {
                if (weekDay.ToString() == "Sunday")
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay - 6).ToShortDateString();
                    //date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay).ToShortDateString();

                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                }
            }
            else
            {
                if (weekDay.ToString() == "Sunday")
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 1).ToShortDateString();
                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
                }

            }
            //if (IsThisWork)
            //{
            //    date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            //}
            //else
            //{
            //    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
            //}
            string api = "GoldenLine/SearchPlanHCDataByLineIDAPI?LineID=" + LineID + "&ShiftTimeID=" + ShiftTimeID + "&date=" + date + "&WeekDay=" + weekDay.ToString();
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult EditHCPlanInfo(string jsonStorages)
        {
            var entity = JsonConvert.DeserializeObject<GLHCActuaWM>(jsonStorages);
            Week week = new Week();
            if (!entity.IsThisWork)
            {
                week = GetNextWeek(DateTime.Now);
            }
            else
            {
                week = GetCurrentWeek(DateTime.Now);

            }

            List<GLBuildPlanDTO> mgDataList = new List<GLBuildPlanDTO>();
            for (int i = 0; i < 7; i++)
            {
                GLBuildPlanDTO newMGDataItem = new GLBuildPlanDTO();
                newMGDataItem.LineID = Convert.ToInt32(entity.LineID);
                newMGDataItem.ShiftTimeID = Convert.ToInt32(entity.ShiftTimeID);
                newMGDataItem.Created_UID = this.CurrentUser.AccountUId;
                newMGDataItem.Created_Date = DateTime.Now;
                newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                newMGDataItem.Modified_Date = DateTime.Now;
                switch (i)
                {
                    case 0:
                        newMGDataItem.PlanHC = Convert.ToInt32(entity.MondayHCActua);
                        newMGDataItem.OutputDate = week.Monday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 1:
                        newMGDataItem.PlanHC = Convert.ToInt32(entity.TuesdayHCActua);
                        newMGDataItem.OutputDate = week.Tuesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 2:
                        newMGDataItem.PlanHC = Convert.ToInt32(entity.WednesdayHCActua);
                        newMGDataItem.OutputDate = week.Wednesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 3:
                        newMGDataItem.PlanHC = Convert.ToInt32(entity.ThursdayHCActua);
                        newMGDataItem.OutputDate = week.Thursday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 4:
                        newMGDataItem.PlanHC = Convert.ToInt32(entity.FridayHCActua);
                        newMGDataItem.OutputDate = week.Friday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 5:
                        newMGDataItem.PlanHC = Convert.ToInt32(entity.SaterdayHCActua);
                        newMGDataItem.OutputDate = week.Saturday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 6:
                        newMGDataItem.PlanHC = Convert.ToInt32(entity.SundayHCActua);
                        newMGDataItem.OutputDate = week.Sunday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                }

                mgDataList.Add(newMGDataItem);
            }
            var json = JsonConvert.SerializeObject(mgDataList);
            string api = string.Format("GoldenLine/ImportBuildHCPlansAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region 实际人力
        public ActionResult HCActual(int LineID)
        {
            string apiUrl = "GoldenLine/GetLineByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", LineID), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            GL_LineDTO dto = JsonConvert.DeserializeObject<GL_LineDTO>(result);
            ViewBag.LineID = LineID;
            return View(dto);
        }
        public FileResult DownloadHCActuaExcelAll(int id, string clintName)
        {
            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            switch (clintName)
            {
                case "js_btn_download_fl":
                    propertiesHead = GetNextWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAllAPI?LineID={0}&week=next", id);
                    break;
                case "js_btn_download_currentWK":
                    propertiesHead = GetCurrentWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAllAPI?LineID={0}&week=current", id);
                    break;
            }
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<GLBuildPlanDTO>>(result);

            if (list.Count() > 0)
            {

                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("实际人力");

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("ActuaHC");
                    SetExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization;
                        worksheet.Cells[iRow, 3].Value = item.ProjectName;
                        worksheet.Cells[iRow, 4].Value = item.LineName;
                        worksheet.Cells[iRow, 5].Value = item.ShiftTime;
                        worksheet.Cells[iRow, 13].Value = item.LineID;
                        worksheet.Cells[iRow, 14].Value = item.ShiftTimeID;
                        iRow++;
                    }

                    //Excel最后一行行号
                    int maxRow = iRow - 1;
                    //设置列隐藏
                    worksheet.Column(13).Hidden = true;
                    worksheet.Column(14).Hidden = true;
                    //设置灰色背景
                    var colorRange = string.Format("A2:E{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                    //设置边框
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelPackage.Save();
                }
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
            else
            {
                return null;
            }

        }
        public FileResult DownloadHCActuaExcel(int id, string clintName)

        {
            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            switch (clintName)
            {
                case "js_btn_download_fl":
                    propertiesHead = GetNextWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAPI?LineID={0}&week=next", id);
                    break;
                case "js_btn_download_currentWK":
                    propertiesHead = GetCurrentWeekPlanHeadColumn();
                    apiUrl = string.Format("GoldenLine/GetBuildPlanDTOListAPI?LineID={0}&week=current", id);
                    break;
            }
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<GLBuildPlanDTO>>(result);

            if (list.Count() > 0)
            {

                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("实际人力");

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("ActuaHC");
                    SetExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization;
                        worksheet.Cells[iRow, 3].Value = item.ProjectName;
                        worksheet.Cells[iRow, 4].Value = item.LineName;
                        worksheet.Cells[iRow, 5].Value = item.ShiftTime;
                        worksheet.Cells[iRow, 13].Value = id;
                        worksheet.Cells[iRow, 14].Value = item.ShiftTimeID;
                        iRow++;
                    }

                    //Excel最后一行行号
                    int maxRow = iRow - 1;
                    //设置列隐藏
                    worksheet.Column(13).Hidden = true;
                    worksheet.Column(14).Hidden = true;
                    //设置灰色背景
                    var colorRange = string.Format("A2:E{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                    //设置边框
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelPackage.Save();
                }
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
            else
            {
                return null;
            }

        }
        public string ImportHCActuaExcel(HttpPostedFileBase upload_excel, int LineID, string hid_currentOrNextWeek)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {
                    errorInfo = "没有worksheet内容";
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] { };
                Week week = new Week();
                switch (hid_currentOrNextWeek)
                {
                    case "nextWeek":
                        propertiesHead = GetNextWeekPlanHeadColumn();
                        week = GetNextWeek(DateTime.Now);
                        break;
                    case "currentWeek":
                        propertiesHead = GetCurrentWeekPlanHeadColumn();
                        week = GetCurrentWeek(DateTime.Now);
                        break;
                }

                bool allColumnsAreError = false;

                //验证Excel的表头是否正确
                allColumnsAreError = validExcelTitleIsErrorTwo(worksheet, propertiesHead, 1, totalColumns);
                if (allColumnsAreError)
                {
                    errorInfo = "Excel格式不正确";
                    return errorInfo;
                }

                List<GLBuildPlanDTO> mgDataList = new List<GLBuildPlanDTO>();
                //Excel行号
                int iRow = 2;
                //Excel列号
                int iColumn;
                int j = 6;
                //如果Excel导入的计划是本周的计划，则导入的数据从当天的日期开始到往后导入到数据库中
                if (hid_currentOrNextWeek == "currentWeek")
                {
                    var strDT = DateTime.Now.DayOfWeek.ToString();
                    switch (strDT)
                    {
                        case "Monday":
                            j = 6;
                            break;
                        case "Tuesday":
                            j = 7;
                            break;
                        case "Wednesday":
                            j = 8;
                            break;
                        case "Thursday":
                            j = 9;
                            break;
                        case "Friday":
                            j = 10;
                            break;
                        case "Saturday":
                            j = 11;
                            break;
                        case "Sunday":
                            j = 12;
                            break;
                    }
                }

                for (iRow = 2; iRow <= totalRows; iRow++)
                {
                    if (allColumnsAreError)
                    {
                        break;
                    }
                    for (iColumn = j; iColumn <= 12; iColumn++)
                    {
                        GLBuildPlanDTO newMGDataItem = new GLBuildPlanDTO();

                        var lineID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 13].Value);
                        if (string.IsNullOrWhiteSpace(lineID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的线", iRow);
                            break;
                        }
                        var ShiftTimeID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 14].Value);
                        if (string.IsNullOrWhiteSpace(ShiftTimeID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的班别", iRow);
                            break;
                        }

                        var Product_HCActua = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value);
                        //如果生产计划为空则默认为0
                        if (string.IsNullOrWhiteSpace(Product_HCActua))
                        {
                            Product_HCActua = "0";
                        }

                        if (!ValidIsDouble(Product_HCActua))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行生产计划输入不正确", iRow);
                            break;
                        }

                        newMGDataItem.LineID = Convert.ToInt32(lineID);
                        newMGDataItem.ShiftTimeID = Convert.ToInt32(ShiftTimeID);
                        switch (iColumn)
                        {
                            case 6:
                                newMGDataItem.OutputDate = week.Monday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 7:
                                newMGDataItem.OutputDate = week.Tuesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 8:
                                newMGDataItem.OutputDate = week.Wednesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 9:
                                newMGDataItem.OutputDate = week.Thursday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 10:
                                newMGDataItem.OutputDate = week.Friday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 11:
                                newMGDataItem.OutputDate = week.Saturday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 12:
                                newMGDataItem.OutputDate = week.Sunday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                        }
                        newMGDataItem.ActualHC = Convert.ToInt32(Product_HCActua);
                        newMGDataItem.Created_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Created_Date = DateTime.Now;
                        newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Modified_Date = DateTime.Now;
                        mgDataList.Add(newMGDataItem);
                    }

                }

                if (allColumnsAreError)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("GoldenLine/ImportBuildHCActuaAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);

            }
            return errorInfo;
        }
        public string ImportHCActuaExcelAll(HttpPostedFileBase upload_excel, int LineID, string hid_currentOrNextWeek)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {
                    errorInfo = "没有worksheet内容";
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] { };
                Week week = new Week();
                switch (hid_currentOrNextWeek)
                {
                    case "nextWeek":
                        propertiesHead = GetNextWeekPlanHeadColumn();
                        week = GetNextWeek(DateTime.Now);
                        break;
                    case "currentWeek":
                        propertiesHead = GetCurrentWeekPlanHeadColumn();
                        week = GetCurrentWeek(DateTime.Now);
                        break;
                }

                bool allColumnsAreError = false;

                //验证Excel的表头是否正确
                allColumnsAreError = validExcelTitleIsErrorTwo(worksheet, propertiesHead, 1, totalColumns);
                if (allColumnsAreError)
                {
                    errorInfo = "Excel格式不正确";
                    return errorInfo;
                }

                List<GLBuildPlanDTO> mgDataList = new List<GLBuildPlanDTO>();
                //Excel行号
                int iRow = 2;
                //Excel列号
                int iColumn;
                int j = 6;
                //如果Excel导入的计划是本周的计划，则导入的数据从当天的日期开始到往后导入到数据库中
                if (hid_currentOrNextWeek == "currentWeek")
                {
                    var strDT = DateTime.Now.DayOfWeek.ToString();
                    switch (strDT)
                    {
                        case "Monday":
                            j = 6;
                            break;
                        case "Tuesday":
                            j = 7;
                            break;
                        case "Wednesday":
                            j = 8;
                            break;
                        case "Thursday":
                            j = 9;
                            break;
                        case "Friday":
                            j = 10;
                            break;
                        case "Saturday":
                            j = 11;
                            break;
                        case "Sunday":
                            j = 12;
                            break;
                    }
                }

                for (iRow = 2; iRow <= totalRows; iRow++)
                {
                    if (allColumnsAreError)
                    {
                        break;
                    }
                    for (iColumn = j; iColumn <= 12; iColumn++)
                    {
                        GLBuildPlanDTO newMGDataItem = new GLBuildPlanDTO();

                        var lineID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 13].Value);
                        if (string.IsNullOrWhiteSpace(lineID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的线", iRow);
                            break;
                        }
                        var ShiftTimeID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 14].Value);
                        if (string.IsNullOrWhiteSpace(ShiftTimeID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行没有找到对应的班别", iRow);
                            break;
                        }

                        var Product_HCActua = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value);
                        //如果生产计划为空则默认为0
                        if (string.IsNullOrWhiteSpace(Product_HCActua))
                        {
                            Product_HCActua = "0";
                        }

                        if (!ValidIsDouble(Product_HCActua))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行生产计划输入不正确", iRow);
                            break;
                        }

                        newMGDataItem.LineID = Convert.ToInt32(lineID);
                        newMGDataItem.ShiftTimeID = Convert.ToInt32(ShiftTimeID);
                        switch (iColumn)
                        {
                            case 6:
                                newMGDataItem.OutputDate = week.Monday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 7:
                                newMGDataItem.OutputDate = week.Tuesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 8:
                                newMGDataItem.OutputDate = week.Wednesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 9:
                                newMGDataItem.OutputDate = week.Thursday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 10:
                                newMGDataItem.OutputDate = week.Friday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 11:
                                newMGDataItem.OutputDate = week.Saturday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                            case 12:
                                newMGDataItem.OutputDate = week.Sunday.ToString(FormatConstants.DateTimeFormatStringByDate);
                                break;
                        }
                        newMGDataItem.ActualHC = Convert.ToInt32(Product_HCActua);
                        newMGDataItem.Created_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Created_Date = DateTime.Now;
                        newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Modified_Date = DateTime.Now;
                        mgDataList.Add(newMGDataItem);
                    }

                }

                if (allColumnsAreError)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("GoldenLine/ImportBuildHCActuaAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);

            }
            return errorInfo;

        }
        public ActionResult QueryHCActuaData(int LineID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;

            if (IsThisWork)
            {
                if (weekDay.ToString() == "Sunday")
                {
               
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay-6).ToShortDateString();

                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                }
            }
            else
            {
                if (weekDay.ToString() == "Sunday")
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 1).ToShortDateString();
                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
                }
               
            }
            string api = "GoldenLine/QueryHCActuaDataAPI?LineID=" + LineID + "&date=" + date;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryHCActuaDataByLineID(int LineID, int ShiftTimeID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;
            if (IsThisWork)
            {
                if (weekDay.ToString() == "Sunday")
                {

                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay - 6).ToShortDateString();

                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                }
            }
            else
            {
                if (weekDay.ToString() == "Sunday")
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 1).ToShortDateString();
                }
                else
                {
                    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
                }

            }
            //if (IsThisWork)
            //{
            //    date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            //}
            //else
            //{
            //    date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
            //}
            string api = "GoldenLine/QueryHCActuaDataByLineIDAPI?LineID=" + LineID + "&ShiftTimeID=" + ShiftTimeID + "&date=" + date + "&WeekDay=" + weekDay.ToString();
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult EditHCActuaInfo(string jsonStorages)
        {
            //   var apiUrl = string.Format("GoldenLine/EditHCActuaInfoAPI");
            var entity = JsonConvert.DeserializeObject<GLHCActuaWM>(jsonStorages);
            Week week = new Week();
            if (!entity.IsThisWork)
            {
                week = GetNextWeek(DateTime.Now);
            }
            else
            {
                week = GetCurrentWeek(DateTime.Now);

            }

            List<GLBuildPlanDTO> mgDataList = new List<GLBuildPlanDTO>();
            for (int i = 0; i < 7; i++)
            {
                GLBuildPlanDTO newMGDataItem = new GLBuildPlanDTO();
                newMGDataItem.LineID = Convert.ToInt32(entity.LineID);
                newMGDataItem.ShiftTimeID = Convert.ToInt32(entity.ShiftTimeID);
                newMGDataItem.Created_UID = this.CurrentUser.AccountUId;
                newMGDataItem.Created_Date = DateTime.Now;
                newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                newMGDataItem.Modified_Date = DateTime.Now;
                switch (i)
                {
                    case 0:
                        newMGDataItem.ActualHC = Convert.ToInt32(entity.MondayHCActua);
                        newMGDataItem.OutputDate = week.Monday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 1:
                        newMGDataItem.ActualHC = Convert.ToInt32(entity.TuesdayHCActua);
                        newMGDataItem.OutputDate = week.Tuesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 2:
                        newMGDataItem.ActualHC = Convert.ToInt32(entity.WednesdayHCActua);
                        newMGDataItem.OutputDate = week.Wednesday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 3:
                        newMGDataItem.ActualHC = Convert.ToInt32(entity.ThursdayHCActua);
                        newMGDataItem.OutputDate = week.Thursday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 4:
                        newMGDataItem.ActualHC = Convert.ToInt32(entity.FridayHCActua);
                        newMGDataItem.OutputDate = week.Friday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 5:
                        newMGDataItem.ActualHC = Convert.ToInt32(entity.SaterdayHCActua);
                        newMGDataItem.OutputDate = week.Saturday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                    case 6:
                        newMGDataItem.ActualHC = Convert.ToInt32(entity.SundayHCActua);
                        newMGDataItem.OutputDate = week.Sunday.ToString(FormatConstants.DateTimeFormatStringByDate);
                        break;
                }

                mgDataList.Add(newMGDataItem);
            }
            var json = JsonConvert.SerializeObject(mgDataList);
            string api = string.Format("GoldenLine/ImportBuildHCActuaAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region FlowChartStation
        public string AddOrUpdateGLStations(GL_StationDTO dto, bool isEdit)
        {
            string strresult = "";
            //获取线别所有站点
            string apiShiftUrl = string.Format("GoldenLine/GetStationDTOsByLineIDAPI?LineID={0}", dto.LineID);
            var responMessage = APIHelper.APIGetAsync(apiShiftUrl);
            var resultShift = responMessage.Content.ReadAsStringAsync().Result;
            var gL_StationDTOs = JsonConvert.DeserializeObject<List<GL_StationDTO>>(resultShift);
            //dto.IsEnabled = true;
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            if (dto.StationID != 0)
            {
                //编辑
                var gL_StationDTO = gL_StationDTOs.Where(o => o.StationID == dto.StationID).ToList();
                var gL_StationDTO1 = gL_StationDTOs.Where(o => o.StationName == dto.StationName).ToList();
                if (gL_StationDTO != null && gL_StationDTO.Count > 0)
                {
                    if (gL_StationDTO1 != null && gL_StationDTO1.Count > 0)
                    {
                        if (gL_StationDTO1.Count == gL_StationDTO.Count)
                        {
                            if (gL_StationDTO[0].StationID == gL_StationDTO1[0].StationID)
                            {
                                var apiUrl = string.Format("GoldenLine/AddOrUpdateGLStationsAPI?isEdit={0}", isEdit);
                                HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                                var result = response.Content.ReadAsStringAsync().Result;
                                return result;

                            }
                            else
                            {
                                return "此线别下站点重复，已经有相同名称的站点。";
                            }
                        }
                        else
                        {
                            return "此线别下站点重复，已经有相同名称的站点。";
                        }

                    }
                    else
                    {
                        var apiUrl = string.Format("GoldenLine/AddOrUpdateGLStationsAPI?isEdit={0}", isEdit);
                        HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                        var result = response.Content.ReadAsStringAsync().Result;
                        return result;

                    }

                }

            }
            else
            {
                //新增
                var gL_StationDTO = gL_StationDTOs.Where(o => o.StationName == dto.StationName).ToList();

                if (gL_StationDTO != null && gL_StationDTO.Count > 0)
                {
                    return "此线别下已有重名的站点，不能新增。";
                }
                else
                {
                    var apiUrl = string.Format("GoldenLine/AddOrUpdateGLStationsAPI?isEdit={0}", isEdit);
                    HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                    var result = response.Content.ReadAsStringAsync().Result;
                    return result;

                }
            }

            return strresult;
        }
        public ActionResult FlowChartStation(int LineID)
        {
            string apiUrl = "GoldenLine/GetLineByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", LineID), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            GL_LineDTO dto = JsonConvert.DeserializeObject<GL_LineDTO>(result);

            return View(dto);
        }
        public ActionResult GetStation(Page page, int LineID)
        {
            string apiUrl = "GoldenLine/GetStation";
            GoldenLineNormalQueryViewModel vm = new GoldenLineNormalQueryViewModel();
            vm.LineID = LineID;
            HttpResponseMessage response = APIHelper.APIPostAsync(vm, page, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        [HttpPost]
        public ActionResult GetStationByID(int id)
        {
            string apiUrl = "GoldenLine/GetStationByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", id), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        [HttpPost]
        public ActionResult RemoveStationByID(int id)
        {
            string apiUrl = "GoldenLine/RemoveStationByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", id), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public FileResult DownloadStationExcel(int id)
        {
            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            propertiesHead = GetFlowChartHeadColumn();
            apiUrl = string.Format("GoldenLine/GetStationDTOsByLineIDAPI?LineID={0}", id);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<GL_StationDTO>>(result);

            //if (list.Count() > 0)
            //{

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FlowChart");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("FlowChart");
                SetFlowChartExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[iRow, 1].Value = item.Seq;
                    worksheet.Cells[iRow, 2].Value = item.StationName;
                    worksheet.Cells[iRow, 3].Value = item.MESStationName;
                    worksheet.Cells[iRow, 4].Value = item.Binding_Seq;
                    worksheet.Cells[iRow, 5].Value = item.CycleTime;
                    worksheet.Cells[iRow, 6].Value = item.IsBirth ? "Y" : "N";
                    worksheet.Cells[iRow, 7].Value = item.IsOutput ? "Y" : "N";
                    worksheet.Cells[iRow, 8].Value = item.IsTest ? "Y" : "N";
                    worksheet.Cells[iRow, 9].Value = item.IsEnabled ? "Y" : "N";
                    worksheet.Cells[iRow, 10].Value = item.IsGoldenLine ? "Y" : "N";
                    worksheet.Cells[iRow, 11].Value = item.IsOEE ? "Y" : "N";
                    worksheet.Cells[iRow, 12].Value = item.IsOne ? "Y" : "N";
                    worksheet.Cells[iRow, 13].Value = item.DashboardTarget ;
                    iRow++;
                }

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
            //}
            //else
            //{
            //    return null;
            //}

        }
        private string[] GetFlowChartHeadColumn()
        {

            var propertiesHead = new[]
            {
                "Seq",
                "Station_Name",
                "MES_Station_Name",
                "Binding_Seq",
                "IE_Cycle_Time",
                "IsBirth",
                "IsOutPut",
                "IsTest",
                "IsEnabled",
                "IsGoldenLine",
                "IsOEE",
                "IsMES",
                 "OEE Target"
            };
            return propertiesHead;
        }
        private void SetFlowChartExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 5].Value = propertiesHead[4];
            worksheet.Cells[1, 6].Value = propertiesHead[5];
            worksheet.Cells[1, 7].Value = propertiesHead[6];
            worksheet.Cells[1, 8].Value = propertiesHead[7];
            worksheet.Cells[1, 9].Value = propertiesHead[8];
            worksheet.Cells[1, 10].Value = propertiesHead[9];
            worksheet.Cells[1, 11].Value = propertiesHead[10];
            worksheet.Cells[1, 12].Value = propertiesHead[11];
            worksheet.Cells[1, 13].Value = propertiesHead[12];
            //设置列宽
            for (int i = 1; i <= 13; i++)
            {
                worksheet.Column(i).Width = 17;
            }

            worksheet.Cells["A1:M1"].Style.Font.Bold = true;
            worksheet.Cells["A1:M1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:M1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }
        public string UploadFlowChartNewFile(HttpPostedFileBase uploadfile, int LineID, int Plant_Organization_UID, int BG_Organization_UID, int? FunPlant_Organization_UID, int CustomerID)
        {

            string errorInfo = string.Empty;
            try
            {
                List<GL_StationDTO> newStationDTOs = new List<GL_StationDTO>();
                var apiUrl = string.Format("GoldenLine/GetStationDTOsByLineIDAPI?LineId={0}", LineID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var StationDTOresult = responMessage.Content.ReadAsStringAsync().Result;
                //得到导入之前的 GL_Station
                List<GL_StationDTO> OldStationDTOs = JsonConvert.DeserializeObject<List<GL_StationDTO>>(StationDTOresult);

                using (var xlsPackage = new OfficeOpenXml.ExcelPackage(uploadfile.InputStream))
                {
                    bool IsBirth = false;
                    bool IsOutput = false; 
                    bool IsOne = false;
                    var ws = xlsPackage.Workbook.Worksheets.FirstOrDefault();
                    int totalRows = ws.Dimension.End.Row;
                    string[] TargetList;
                    int Target1;
                    int Target2=0;
                    for (int rowIdx = 2; rowIdx <= totalRows; rowIdx++)
                    {
                        // site name
                        string SeqStr = (ws.Cells[rowIdx, 1].Value ?? "").ToString();
                        string StationNameStr = (ws.Cells[rowIdx, 2].Value ?? "").ToString();
                        string MESStationNameStr = (ws.Cells[rowIdx, 3].Value ?? "").ToString();
                        string BindingSeqStr = (ws.Cells[rowIdx, 4].Value ?? "").ToString();
                        string CycleTimeStr = (ws.Cells[rowIdx, 5].Value ?? "").ToString();
                        string IsBirthStr = (ws.Cells[rowIdx, 6].Value ?? "").ToString();
                        string IsOutputStr = (ws.Cells[rowIdx, 7].Value ?? "").ToString();
                        string IsTestStr = (ws.Cells[rowIdx, 8].Value ?? "").ToString();
                        string IsEnabledStr = (ws.Cells[rowIdx, 9].Value ?? "").ToString();
                        string IsGoldenLineStr = (ws.Cells[rowIdx, 10].Value ?? "").ToString();
                        string IsOEEStr = (ws.Cells[rowIdx, 11].Value ?? "").ToString();

                        string IsOneStr = (ws.Cells[rowIdx, 12].Value ?? "").ToString();
                        string DashboardTarget = (ws.Cells[rowIdx, 13].Value ?? "").ToString();
                        //OEE Target 验证
                        if(DashboardTarget !="") //为空的时候 放他一马
                        {
                            TargetList = DashboardTarget.Split(',');

                            if(TargetList.Length!=2)  // 正确的形式为两个
                            {
                                return string.Format("第{0}行OEE Target数据格式错误", rowIdx);
                            }
                            else
                            {
                      
                                    int.TryParse(TargetList[0], out Target1);
                                    int.TryParse(TargetList[1], out Target2);
                                    if (Target1<=0 ||Target1>=100 || Target2 <=0 || Target2 >100)
                                    {
                                        return string.Format("第{0}行OEE Target数据格式错误,目标必须在0~100之间", rowIdx);
                                    }
                            }
                        }



                        GL_StationDTO entity = new GL_StationDTO();
                        int value00 = 0;
                        if (int.TryParse(SeqStr, out value00))
                            entity.Seq = value00;
                        entity.StationName = StationNameStr;
                        entity.MESStationName = MESStationNameStr;
                        int BindingSeq = 0;
                        if (int.TryParse(BindingSeqStr, out BindingSeq))
                            entity.Binding_Seq = BindingSeq;

                        decimal value02 = 0;
                        if (decimal.TryParse(CycleTimeStr, out value02))
                            entity.CycleTime = Math.Round(value02, 2);
                        if (IsBirth == false)
                        {
                            entity.IsBirth = IsBirthStr.ToUpper().IndexOf('Y') >= 0;
                            if (entity.IsBirth == true)
                            {
                                IsBirth = true;
                            }

                        }
                        else
                        {
                            entity.IsBirth = false;
                        }

                        if (IsOutput == false)
                        {
                            entity.IsOutput = IsOutputStr.ToUpper().IndexOf('Y') >= 0;
                            if (entity.IsOutput == true)
                            {
                                IsOutput = true;
                            }

                        }
                        else
                        {
                            entity.IsOutput = false;
                        }
                        entity.IsTest = IsTestStr.ToUpper().IndexOf('Y') >= 0;
                        entity.IsGoldenLine = IsGoldenLineStr.ToUpper().IndexOf('Y') >= 0;
                        entity.IsOEE = IsOEEStr.ToUpper().IndexOf('Y') >= 0;
                        entity.IsOne = IsOneStr.ToUpper().IndexOf('Y') >= 0;
                        entity.IsEnabled = IsEnabledStr.ToUpper().IndexOf('Y') >= 0;
                        entity.LineID = LineID;
                        entity.Plant_Organization_UID = Plant_Organization_UID;
                        entity.BG_Organization_UID = BG_Organization_UID;
                        entity.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        entity.DashboardTarget = DashboardTarget;
                        entity.Modified_UID = CurrentUser.AccountUId;
                        entity.Modified_Date = DateTime.Now;

                        var StationDTO = OldStationDTOs.FirstOrDefault(o => o.LineID == LineID && o.Binding_Seq == BindingSeq);

                        if (StationDTO != null)
                        {
                            entity.StationID = StationDTO.StationID;
                            // entity.IsEnabled = StationDTO.IsEnabled;
                        }
                        else
                        {
                            entity.StationID = 0;
                        }

                        newStationDTOs.Add(entity);
                        //做重复判断
                        var cfStationDTOs = newStationDTOs.Where(o => o.LineID == LineID && o.Binding_Seq == BindingSeq).ToList();
                        if (cfStationDTOs.Count > 1)
                        {
                            return string.Format("第{0}行数据重复", rowIdx);
                        }

                    }

                    //防单 处理
                    if (IsOutput == true)
                    {

                        var OldStationDTO = OldStationDTOs.FirstOrDefault(o => o.IsOutput == true);
                        var newStationDTO = newStationDTOs.FirstOrDefault(o => o.IsOutput == true);
                        if (OldStationDTO != null)
                        {
                            OldStationDTO.IsOutput = false;
                            OldStationDTO.Modified_UID = CurrentUser.AccountUId;
                            OldStationDTO.Modified_Date = DateTime.Now;
                            //还要做判断
                            if (OldStationDTO.StationID != newStationDTO.StationID)
                            {
                                newStationDTOs.Add(OldStationDTO);
                            }else
                            {
                              //  newStationDTOs.Add(OldStationDTO);
                            }

                        }

                    }

                    if (IsBirth == true)
                    {
                        var OldStationDTO = OldStationDTOs.FirstOrDefault(o => o.IsBirth == true);
                        var newStationDTO = newStationDTOs.FirstOrDefault(o => o.IsBirth == true);
                        if (OldStationDTO != null)
                        {
                            OldStationDTO.IsBirth = false;
                            OldStationDTO.Modified_UID = CurrentUser.AccountUId;
                            OldStationDTO.Modified_Date = DateTime.Now;
                            //还要做判断
                            if (OldStationDTO.StationID != newStationDTO.StationID)
                            {
                                newStationDTOs.Add(OldStationDTO);

                            }else
                            {

                            }
                        }
                    }

             
                    //插入表
                    var json = JsonConvert.SerializeObject(newStationDTOs);
                    var apiInsertVendorInfoUrl = string.Format("GoldenLine/InserOrUpdateStationsAPI");
                    HttpResponseMessage responMessageStation = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessageStation.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }

            }
            catch (Exception ex)
            {
                errorInfo = ex.Message;
            }

            return errorInfo;

        }

        #endregion

        #region OperatorList  OEE
        public ActionResult OperatorList(int LineID)
        {
            string apiUrl = "GoldenLine/GetLineByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", LineID), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            GL_LineDTO dto = JsonConvert.DeserializeObject<GL_LineDTO>(result);
            ViewBag.LineID = LineID;
            return View(dto);
        }
        public FileResult DownloadOperatorListExcel(int id)
        {
            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            propertiesHead = GetOperatorListHeadColumn();
            apiUrl = string.Format("GoldenLine/DownloadOperatorListExcelAPI?LineID={0}", id);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<OEE_UserStationDTO>>(result);

            if (list.Count() > 0)
            {

                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("NGAccount");

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("NGAccount");
                    SetExcelStyleUser(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization;
                        worksheet.Cells[iRow, 3].Value = item.FunPlant_Organization;
                        worksheet.Cells[iRow, 4].Value = item.Project_Name;
                        worksheet.Cells[iRow, 5].Value = item.Line_Name;
                        worksheet.Cells[iRow, 6].Value = item.Station_Name;
                        worksheet.Cells[iRow, 7].Value = item.User_NTID;
                       //worksheet.Cells[iRow, 8].Value = item.OEE_UserStation_UID;

                        iRow++;
                    }
                    //Excel最后一行行号
                    int maxRow = iRow - 1;
                    //设置列隐藏
                    //worksheet.Column(8).Hidden = true;
                    //设置灰色背景
                    var colorRange = string.Format("A2:G{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    //设置边框
                    worksheet.Cells[string.Format("A1:G{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:G{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:G{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:G{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelPackage.Save();
                }
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
            else
            {
                return null;
            }

        }

        private void SetExcelStyleUser(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 5].Value = propertiesHead[4];
            worksheet.Cells[1, 6].Value = propertiesHead[5];
            worksheet.Cells[1, 7].Value = propertiesHead[6];


            for (int i = 1; i <= 7; i++)
            {


                worksheet.Column(i).Width = 23;


            }

            worksheet.Cells["A1:G1"].Style.Font.Bold = true;
            worksheet.Cells["A1:G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:G1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

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
        public string ImportOperatorList(HttpPostedFileBase upload_excel, int lineID, int plant_Organization_UID, int bG_Organization_UID, int? funPlant_Organization_UID, int customerID)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {
                    errorInfo = "没有worksheet内容";
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] { };
                Week week = new Week();

                propertiesHead = GetOperatorListHeadColumn();


                bool isExcelError = false;

                //验证Excel的表头是否正确

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

                List<OEE_UserStationDTO> mgDataList = new List<OEE_UserStationDTO>();
                //获得所有ORGBOMLIST
                var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                //获取专案
                var apiUrlProjectDTO = string.Format("CNCMachine/GetSystemProjectDTOAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}", 0, 0);
                var responMessageProjectDTO = APIHelper.APIGetAsync(apiUrlProjectDTO);
                var resultProjectDTO = responMessageProjectDTO.Content.ReadAsStringAsync().Result;
                var projectDTOs = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(resultProjectDTO);

                //获取线
                var apiUrlLineDTO = string.Format("GoldenLine/GetOEELineDTOAPI?CustomerID={0}", customerID);
                var responMessageLineDTO = APIHelper.APIGetAsync(apiUrlLineDTO);
                var resultLineDTO = responMessageLineDTO.Content.ReadAsStringAsync().Result;
                var LineDTOs = JsonConvert.DeserializeObject<List<GL_LineDTO>>(resultLineDTO);

                //获取站
                var apiUrlStation = string.Format("GoldenLine/GetOEEStationDTOAPI?CustomerID={0}", customerID);
                var responMessageStation = APIHelper.APIGetAsync(apiUrlStation);
                var resultStation = responMessageStation.Content.ReadAsStringAsync().Result;
                var Stations = JsonConvert.DeserializeObject<List<GL_StationDTO>>(resultStation);


                //获取已有的用户列表

                var apiUrlOperatorList = string.Format("GoldenLine/GetOperatorListAPI?CustomerID={0}", customerID);
                var responMessageOperatorList = APIHelper.APIGetAsync(apiUrlOperatorList);
                var resultOperatorList = responMessageOperatorList.Content.ReadAsStringAsync().Result;
                var OperatorLists = JsonConvert.DeserializeObject<List<OEE_UserStationDTO>>(resultOperatorList);

                //获取所有用户列表

                var apiUrlUserByDTOs = string.Format("GoldenLine/GetAllUserByDTOsAPI?Plant_Organization_UID={0}", plant_Organization_UID);
                var responMessageUserByDTOs = APIHelper.APIGetAsync(apiUrlUserByDTOs);
                var resultUserByDTOs = responMessageUserByDTOs.Content.ReadAsStringAsync().Result;
                var UserByDTOs = JsonConvert.DeserializeObject<List<SystemUserOEEDTO>>(resultUserByDTOs);

                for (int i = 2; i <= totalRows; i++)
                {
                    var oEE_UserStationDTO = new OEE_UserStationDTO();
                    int Plant_Organization_UID = 0;
                    int BG_Organization_UID = 0;
                    int? FunPlant_Organization_UID = null;
                    int Project_UID = 0;

                    string Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value).Trim();
                    string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                    string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                    string Project_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "专案")].Value);

                    if (string.IsNullOrWhiteSpace(Plant_Organization))
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行厂区代号没有值", i);
                        return errorInfo;
                    }
                    else
                    {
                        var hasplant = orgboms.Where(m => m.Plant == Plant_Organization).FirstOrDefault();

                        if (hasplant != null)
                        {
                            Plant_Organization_UID = hasplant.Plant_Organization_UID;
                        }
                        else
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行厂区代号的值没有找到", i);
                            return errorInfo;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(BG_Organization))
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行op代号没有值", i);
                        return errorInfo;
                    }
                    else
                    {

                        var hasbg = orgboms.Where(m => m.BG == BG_Organization).FirstOrDefault();
                        if (hasbg != null)
                        {
                            BG_Organization_UID = hasbg.BG_Organization_UID;
                        }
                        else
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行OP代号的值没有找到", i);
                            return errorInfo;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                    {
                        FunPlant_Organization_UID = null;
                    }
                    else
                    {

                        var hasfunplant = orgboms.Where(m => m.Plant == Plant_Organization
                        & m.BG == BG_Organization & m.Funplant == FunPlant_Organization).FirstOrDefault();
                        if (hasfunplant != null)
                        {
                            FunPlant_Organization_UID = hasfunplant.Funplant_UID;
                        }
                        else
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                            return errorInfo;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(Project_Name))
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行专案没有值", i);
                        return errorInfo;
                    }
                    else
                    {
                        var hasProject = projectDTOs.Where(m => m.Project_Name == Project_Name&&m.Organization_UID==BG_Organization_UID).FirstOrDefault();
                        if (hasProject != null)
                        {
                            Project_UID = hasProject.Project_UID;
                        }
                        else
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行专案没有找到", i);
                            return errorInfo;
                        }

                    }

                    if (plant_Organization_UID != Plant_Organization_UID)
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行厂区不正确，请检查！", i);
                        return errorInfo;
                    }

                    if (bG_Organization_UID != BG_Organization_UID)
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行OP类型不正确，请检查！", i);
                        return errorInfo;
                    }

                    if (customerID != Project_UID)
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行专案不正确，请检查！", i);
                        return errorInfo;
                    }

                    string Line_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "线别")].Value);
                    var lineDTO = LineDTOs.FirstOrDefault(o => o.CustomerID == Project_UID && o.LineName == Line_Name);
                    int LineID = 0;
                    if (lineDTO != null)
                    {
                        LineID = lineDTO.LineID;
                    }
                    else
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行线别没有找到，请检查！", i);
                        return errorInfo;
                    }

                    string Station_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "工站")].Value);
                    var stationDTO = Stations.FirstOrDefault(o => o.CustomerID == Project_UID && o.LineID == LineID && o.StationName == Station_Name);
                    int StationID = 0;
                    if (stationDTO != null)
                    {
                        StationID = stationDTO.StationID;
                    }
                    else
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行工站没有找到，请检查！", i);
                        return errorInfo;
                    }

                    string NGAccount = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "NGAccount")].Value);
                    if (string.IsNullOrWhiteSpace(NGAccount))
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行NTID没有值", i);
                        return errorInfo;
                    }
                    else if (NGAccount.Length > 20)
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行NTID长度超过最大限定[20]", i);
                        return errorInfo;
                    }
                    NGAccount = NGAccount.Trim();

                    int Account_UID = 0;
                    var userDTO = UserByDTOs.FirstOrDefault(o => o.User_NTID.Contains(NGAccount));
                    if (userDTO != null)
                    {
                        Account_UID = userDTO.Account_UID;
                    }
                    else
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行没有找到此用户的NTID", i);
                        return errorInfo;
                    }

                    //数据库判重,
                    var isDbRepeated = mgDataList.Exists(m => m.StationID == StationID && m.KeyInNG_User_UID == Account_UID && m.BG_Organization_UID == BG_Organization_UID);
                    if (isDbRepeated)
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行用户[{1}]已经存在,不可重复导入", i, NGAccount);
                        return errorInfo;
                    }

                    var OperatorList = OperatorLists.FirstOrDefault(o => o.Plant_Organization_UID == Plant_Organization_UID  && o.BG_Organization_UID == BG_Organization_UID && o.StationID == StationID);
                    if (OperatorList != null)
                    {
                        oEE_UserStationDTO.OEE_UserStation_UID = OperatorList.OEE_UserStation_UID;
                    }

                    oEE_UserStationDTO.Plant_Organization_UID = Plant_Organization_UID;
                    oEE_UserStationDTO.BG_Organization_UID = BG_Organization_UID;
                    oEE_UserStationDTO.FunPlant_Organization_UID = FunPlant_Organization_UID;
                    oEE_UserStationDTO.KeyInNG_User_UID = Account_UID;
                    oEE_UserStationDTO.Line_ID = LineID;
                    oEE_UserStationDTO.StationID = StationID;
                    oEE_UserStationDTO.Project_UID = Project_UID;
                    oEE_UserStationDTO.Modified_UID = CurrentUser.AccountUId;
                    oEE_UserStationDTO.Modified_Date = DateTime.Now;

                    mgDataList.Add(oEE_UserStationDTO);
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("GoldenLine/ImportOperatorListAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);

            }
            return errorInfo;

        }
        private string[] GetOperatorListHeadColumn()
        {
            var currentWeek = GetCurrentWeek(DateTime.Now);
            var propertiesHead = new[]
            {
                "厂区",
                "OP类型",
                "功能厂",
                "专案",
                "线别",
                "工站",
                "NGAccount"
            };
            return propertiesHead;
        }
        public ActionResult QueryOperatorListDTOs(int LineID)
        {
            string api = "GoldenLine/QueryOperatorListAPI?LineID=" + LineID;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region  休息时间
        [HttpPost]
        public ActionResult GetRestTimeByID(int id)
        {
            string apiUrl = "GoldenLine/GetRestTimeByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", id), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        [HttpPost]
        public ActionResult RemoveRestTimeByID(int id)
        {
            string apiUrl = "GoldenLine/RemoveRestTimeByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", id), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        [HttpPost]
        public string AddOrUpdateGLRestTime(GL_RestTimeDTO dto, bool isEdit, int shiftID)
        {
            string strresult = "";
            //获取所有休息时间
            string apiShiftUrl = "GoldenLine/GetAllRestTimesAPI";
            var responMessage = APIHelper.APIGetAsync(apiShiftUrl);
            var resultShift = responMessage.Content.ReadAsStringAsync().Result;
            var shiftDTOs = JsonConvert.DeserializeObject<List<GL_RestTimeDTO>>(resultShift);
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Time = DateTime.Now;
            DateTime dateNow = DateTime.Now;
            try
            {
                Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + dto.StartTime));
            }
            catch (Exception ex)
            {
                return strresult = "休息开始时间错误，请重新填写.小时数小于24，分钟数小于60.";
            }
            try
            {
                Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + dto.EndTime));
            }
            catch (Exception ex)
            {
                return strresult = "休息结束时间错误，请重新填写.小时数小于24，分钟数小于60.";
            }

            string apiUrl1 = "GoldenLine/GetShiftTimeByID";
            HttpResponseMessage response1 = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", shiftID), apiUrl1);
            var ShiftTimeResult = response1.Content.ReadAsStringAsync().Result;
            var ShifTimeDTO = JsonConvert.DeserializeObject<GL_ShiftTimeDTO>(ShiftTimeResult);

            bool isShiftRepeat = GetRestShiftIsRepeat(ShifTimeDTO, dto);
            if (isShiftRepeat == true)
            {
                return strresult = "休息时间错误，添加的时间段不在班次范围内。";
            }

            var shiftallDTOs = shiftDTOs.Where(o => o.ShiftTimeID == dto.ShiftTimeID).ToList();
            bool isRepeat = GetRestIsRepeat(shiftallDTOs, dto);
            if (isRepeat == true)
            {
                return strresult = "休息时间错误，添加的时间段和现有休息时间段有交集。";
            }
           if(isEdit)
            {
                string apiUrl = "GoldenLine/UpdateGLRestTime";
                HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                strresult = response.Content.ReadAsStringAsync().Result;
            }
           else
            {
                string apiUrl = "GoldenLine/AddOrUpdateGLRestTime";
                HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
                strresult = response.Content.ReadAsStringAsync().Result;
            }
                // post & submit to api
            
     
            return strresult;
        }

        #endregion

    }
}