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
using System.Web.Helpers;
using System.Web;
using System.Linq;
using System.Text;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.Ajax.Utilities;
using PDMS.Model.EntityDTO;
using PDMS.Core.Authentication;
using PDMS.Model.ViewModels.ProductionPlanning;
using PDMS.Web.Business.Flowchart;
using System.Net;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Collections;
using System.Reflection;

namespace PDMS.Web.Controllers
{
    public class MachineController : WebControllerBase
    {

        /// <summary>
        /// 获取当前登陆人的厂区ID
        /// </summary>
        /// <returns></returns>
        public int GetPlantOrgUid()
        {
            int plantorguid = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                plantorguid = (int)CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
            return plantorguid;
        }

        public ActionResult MachineYieldReport()
        {
            Machine_YieldReportVM currentVM = new Machine_YieldReportVM();
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
                var errorinfo = JsonConvert.DeserializeObject<IEnumerable<EnumerationDTO>>(result);
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {

                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }

            return View("MachineYieldReport", currentVM);

        }

        public ActionResult QueryMachine_Yields(Machine_YieldDTO search, Page page)
        {
            search.Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = string.Format("MachineYieldReport/QueryMachine_YieldsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public FileResult DoExportMachineReprot(int plantId, int optypeId, int funplantId, string customer, string station, string machine_ID)
        {
            var apiUrl = string.Format("MachineYieldReport/ExportMachineReprotAPI?plantId={0}&optypeId={1}&funplantId={2}&customer={3}&station={4}&machine_ID={5}", plantId, optypeId, funplantId, customer, station, machine_ID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Machine_YieldDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("MachineReprot");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "Customer", "站点", "机台号", "投入数", "良品数", "良率", "不良率" };

            using (var excelPackage = new ExcelPackage(stream))
            {

                var worksheet = excelPackage.Workbook.Worksheets.Add("Machine Reprot");

                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];

                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PIS_Customer_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.PIS_Station_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Machine_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.InPut_Qty;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Yield_Qty;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Yield;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.NO_Yield;
                }


                for (int i = 1; i <= 8; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                worksheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult MachineDetail()
        {
            MachineDetailVM currentVM = new MachineDetailVM();
            var apiUrl = string.Format("FixturePart/QueryOpTypesByUserAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.Orgs = optypes;
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
                apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                responMessage = APIHelper.APIGetAsync(apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
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

            var apiUrlMachine = string.Format("Enumeration/GetMachineDataSourceAPI?enum_Type={0}", "MachineDataSource");
            var responMessageMachine = APIHelper.APIGetAsync(apiUrlMachine);
            var resultMachine = responMessageMachine.Content.ReadAsStringAsync().Result;
            var MachineDataSources = JsonConvert.DeserializeObject<List<string>>(resultMachine);
            currentVM.MachineDataSources = MachineDataSources;
            return View("MachineDetail", currentVM);
        }
        public ActionResult QueryMachineDetails(string Customer, string Machine, DateTime StartTime, DateTime EndTime, string Station, int Input_Qty)
        {
            Machine = Machine.Replace("#", "%23");
            var apiUrl = string.Format("pis/pdcaNgdetbyMachine?Customer={0}&Station={1}&StartTime={2}&EndTime={3}&Machine={4}", Customer, Station, StartTime.ToString("yyyy-MM-dd HH:mm"), EndTime.ToString("yyyy-MM-dd HH:mm"), Machine);
            var result = HttpGet(WebAPIPath + apiUrl);
            JObject josnData = (JObject)JsonConvert.DeserializeObject(result);
            var Machinelist = JsonConvert.DeserializeObject<List<MachineDetailDTO>>(josnData["data"].ToString());
            foreach (var item in Machinelist)
            {
                if (Input_Qty == 0)
                {
                    item.NO_Yield = 1;
                }
                else
                {
                    item.NO_Yield = item.NG_Point / Input_Qty;
                }

            }
            if (Machinelist != null && Machinelist.Count > 0)
            {
                result = DataTableConvertToJson.DataTable2Json(ToDataTable(Machinelist));
            }
            else
            {
                result = "";
            }


            return Content(result, "application/json");
        }
        public FileResult ExportMachineNoYieldDetails(string Customer, string Machine, string StartTime, string EndTime, string Station)
        {
            Machine = Machine.Replace("#", "%23");
            //  var apiUrl = string.Format("pis/pdcaNgDetail?Customer={0}&Station={1}&StartTime={2}&EndTime={3}&Machine={4}", Customer, Station, StartTime.ToString("yyyy-MM-dd HH:mm"), EndTime.ToString("yyyy-MM-dd HH:mm"), Machine);

            var apiUrl = string.Format("pis/pdcaNgDetail?Customer={0}&Station={1}&StartTime={2}&EndTime={3}&Machine={4}", Customer, Station, StartTime, EndTime, Machine);
            var result = HttpGet(WebAPIPath + apiUrl);
            JObject josnData = (JObject)JsonConvert.DeserializeObject(result);
            var list = JsonConvert.DeserializeObject<List<MachineNoYieldDetailDTO>>(josnData["data"].ToString());

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("MachineNoYieldDetails");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "Customer", "站点", "机台号", "SN", "时间", "不良项", "治具号" };

            using (var excelPackage = new ExcelPackage(stream))
            {

                var worksheet = excelPackage.Workbook.Worksheets.Add("MachineNoYieldDetails");

                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];

                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Customer;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Station;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Machine;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.SN;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.LastUpdateDate;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.DefectCode;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Fixture;
                }


                for (int i = 1; i <= 8; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                worksheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            dt.TableName = "Machinelist";
            return dt;
        }
        /// <summary>
        /// 获取配置文件路径
        /// </summary>
        private static string WebAPIPath
        {
            get { return ConfigurationManager.AppSettings["WebMachinePath"].ToString(); }
        }
        /// <summary>
        /// 请求数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }


        /// <summary>
        /// 获取初始化列表数据
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryMachine_CustomerInfo(Machine_CustomerDTO search, Page page)
        {

            search.Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = string.Format("MachineYieldReport/QueryMachine_CustomerInfoAPI");
            var responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult QueryMachine_CustomerByuid(int Machine_Customer_UID)
        {
            var apiUrl = string.Format("MachineYieldReport/QueryMachine_CustomerByuidAPI?Machine_Customer_UID={0}", Machine_Customer_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddOrEditCustomerAndStationInfo(string jsonStorages)
        {
            var result = string.Empty;
            var apiUrl = string.Format("MachineYieldReport/AddOrEditCustomerAndStationInfoAPI");
            var entity = JsonConvert.DeserializeObject<Machine_CustomerAndStationDTO>(jsonStorages);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            entity.Created_UID = this.CurrentUser.AccountUId;
       
            if (entity.DataSourceType == "BadDetail")
            {
                //新增
                if (string.IsNullOrEmpty(entity.PIS_Customer_Name))
                {
                    int FlowChartMaster_UID = 0;
                    if (!string.IsNullOrEmpty(entity.Customer) && !string.IsNullOrEmpty(entity.Project) && !string.IsNullOrEmpty(entity.Part_Types) && !string.IsNullOrEmpty(entity.Product_Phase))
                    {
                        //获取对应的FlowCharMaster
                        var flowChartMasterIDAPI = string.Format("FlowChart/GetFlowChartMasterID?BU_D_Name={0}&Project_Name={1}&Part_Types={2}&Product_Phase={3}", entity.Customer, entity.Project, entity.Part_Types, entity.Product_Phase);
                        HttpResponseMessage plantsmessage = APIHelper.APIGetAsync(flowChartMasterIDAPI);
                        var flowChartMasterID = plantsmessage.Content.ReadAsStringAsync().Result;
                        FlowChartMaster_UID = int.Parse(flowChartMasterID);
                        if (int.Parse(flowChartMasterID) == 0)
                        {
                            result = string.Format("未找到对应:客户:{0},专案:{1},部件类型:{2},生产阶段:{3}", entity.Customer, entity.Project, entity.Part_Types, entity.Product_Phase);
                            return Content(result, "application/json");
                        }
                    }
                    entity.PIS_Customer_Name = FlowChartMaster_UID.ToString();
                }

                //检查绑定序号是否存在
                var apiUrlCheckFLDetialID = string.Format("FlowChart/CheckFlowChart_DetailByID");
                HttpResponseMessage responMessageCheckFLDetialID = APIHelper.APIPostAsync(entity, apiUrlCheckFLDetialID);
                var checkResult = responMessageCheckFLDetialID.Content.ReadAsStringAsync().Result;
                if (checkResult.Contains("SUCCESS"))
                {
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
                    result = responMessage.Content.ReadAsStringAsync().Result;
                    return Content(result, "application/json");
                }
                else
                {
                    //var message = string.Format("不存在绑定序号：{0} ", checkResult);
                    return Content(checkResult, "application/json");
                }
            }
            else
            {
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
                return Content(result, "application/json");
            }
        }

        /// <summary>
        /// 获取产线
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        public ActionResult GetCustomerList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("MachineYieldReport/GetCustomerListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 根据专案获取站点
        /// </summary>
        /// <param name="Machine_Customer_UID"></param>
        /// <returns></returns>
        public ActionResult GetStationList(int Machine_Customer_UID)
        {
            var apiUrl = string.Format("MachineYieldReport/GetStationListAPI?Machine_Customer_UID={0}", Machine_Customer_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteStation(int Machine_Station_UID)
        {
            var apiUrl = string.Format("MachineYieldReport/DeleteStationAPI?Machine_Station_UID={0}", Machine_Station_UID);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DeleteCustomer(int Machine_Customer_UID)
        {
            var apiUrl = string.Format("MachineYieldReport/DeleteCustomerAPI?Machine_Customer_UID={0}", Machine_Customer_UID);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }



    }
}