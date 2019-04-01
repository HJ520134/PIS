using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PDMS.Web.Controllers
{
    public class PISSyncFailedRecordController : WebControllerBase
    {
        // GET: PISSyncFailedRecord
        public ActionResult PISSyncFailedRecord()
        {
            return View("PISSyncFailedRecord");
        }


        //获取所有同步失败的数据
        public ActionResult GetSyncFailedRecordData(Page page)
        {
            var apiUrl = string.Format("ProcessIDTRSConfig/QuerySyncFailedRecordAPI");
            MES_PIS_SyncFailedRecordDTO model = new MES_PIS_SyncFailedRecordDTO();
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(model, page, apiUrl);
            //HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        /// <summary>
        /// 机台同步方法
        /// </summary>
        /// <param name="MES_PIS_SyncFailedRecord_UID"></param>
        /// <returns></returns>
        public string MachineYieldSync(int  MES_PIS_SyncFailedRecord_UID)
        {

            //获取错误信息
            var apiUrl = string.Format("ProcessIDTRSConfig/GetMES_PIS_SyncFailedRecordDTOByIDAPI?MES_PIS_SyncFailedRecord_UID={0}", MES_PIS_SyncFailedRecord_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var mES_PIS_SyncFailedRecordDTO = JsonConvert.DeserializeObject<MES_PIS_SyncFailedRecordDTO>(result);

            //获取解析配置数据，根据URL
            List<Machine_Schedule_ConfigDTO> machine_Schedule_ConfigDTOs = new List<Machine_Schedule_ConfigDTO>();
            var machine_Schedule_ConfigDTO = GetMachine_Schedule_ConfigDTOs(mES_PIS_SyncFailedRecordDTO.SyncRequest);

            machine_Schedule_ConfigDTOs.Add(machine_Schedule_ConfigDTO);

            //获取机台数据
            List<Machine_YieldDTO> machine_YieldDTOs = new List<Machine_YieldDTO>();
            var resultMachine = HttpGet(mES_PIS_SyncFailedRecordDTO.SyncRequest);
            JObject josnData = (JObject)JsonConvert.DeserializeObject(resultMachine);
            var list = JsonConvert.DeserializeObject<List<MachineYield>>(josnData["data"].ToString());
            machine_YieldDTOs = SetMachine_YieldDTOs(list, machine_YieldDTOs, machine_Schedule_ConfigDTO.Machine_Station_UID, machine_Schedule_ConfigDTO.StarTime.Value, machine_Schedule_ConfigDTO.EndTime.Value);

            //组装请求的数据
            var machineYieldAndMachineConfigDTO = new MachineYieldAndMachineConfigDTO();
            machineYieldAndMachineConfigDTO.Machine_YieldDTOs = machine_YieldDTOs;
            machineYieldAndMachineConfigDTO.Machine_Schedule_ConfigDTOs = machine_Schedule_ConfigDTOs;
            //插入数据结果
            var apiUrlMachineSave = string.Format("MachineYieldReport/InsertMachine_YieldAndMachineConfigAPI");                      
            HttpResponseMessage responMessageMachineSave = APIHelper.APIPostAsync(machineYieldAndMachineConfigDTO, apiUrlMachineSave);
            var resultMachineSave = responMessageMachineSave.Content.ReadAsStringAsync().Result;

            mES_PIS_SyncFailedRecordDTO.OperateID=this.CurrentUser.AccountUId;
            if (resultMachineSave.Contains("SUCCESS"))
            {
                //执行成功
                mES_PIS_SyncFailedRecordDTO.Is_ManuallySuccess = true;
    
            }else
            {
                //执行失败
                mES_PIS_SyncFailedRecordDTO.FailedNumber += 1;         
            }
            //更新配置表
            var apiUrlSynce = string.Format("ProcessIDTRSConfig/UpdateSyncFailedLogAPI");
            HttpResponseMessage responMessageSynce = APIHelper.APIPostAsync(mES_PIS_SyncFailedRecordDTO, apiUrlSynce);
            var resultSynce = responMessageSynce.Content.ReadAsStringAsync().Result;
            return "SUCCESS";

        }


        public static List<Machine_YieldDTO> SetMachine_YieldDTOs(List<MachineYield> MachineYields, List<Machine_YieldDTO> machine_YieldDTOs, int Machine_Station_UID, DateTime startTime, DateTime endTime)
        {

            foreach (var item in MachineYields)
            {
                Machine_YieldDTO tempMachine_YieldDTO = new Machine_YieldDTO();
                tempMachine_YieldDTO.Machine_Station_UID = Machine_Station_UID;
                tempMachine_YieldDTO.Machine_ID = item.Machine;
                tempMachine_YieldDTO.InPut_Qty = item.Input;
                tempMachine_YieldDTO.NG_Qty = item.NG;
                tempMachine_YieldDTO.NG_Point_Qty = item.NGP;
                tempMachine_YieldDTO.StarTime = startTime;
                tempMachine_YieldDTO.EndTime = endTime;
                tempMachine_YieldDTO.Created_Date = DateTime.Now;
                machine_YieldDTOs.Add(tempMachine_YieldDTO);
            }
            return machine_YieldDTOs;
        }
        /// <summary>
        /// 解析url 生成Machine_Schedule_ConfigDTO；
        /// </summary>
        /// <param name="apiurl"></param>
        /// <returns></returns>
        public Machine_Schedule_ConfigDTO GetMachine_Schedule_ConfigDTOs(string apiurl)
        {
            // "http://CNCTUG0WEBOA01:9800/pis/pdcaNgSummary?Customer=Milan-CTU-Housing&Station=Milan-Laser(Band)&StartTime=2018-05-18 07:30&EndTime=2018-05-18 08:00";
            Machine_Schedule_ConfigDTO machine_Schedule_ConfigDTO = new Machine_Schedule_ConfigDTO();
            var strMachine_Schedule_ConfigDTO = apiurl.Substring(apiurl.LastIndexOf("?") + 1);
            var  machine_Schedule_ConfigDTOs = strMachine_Schedule_ConfigDTO.Split('&');
            string Customer = machine_Schedule_ConfigDTOs[0].Substring(machine_Schedule_ConfigDTOs[0].LastIndexOf("=") + 1);
            string Station = machine_Schedule_ConfigDTOs[1].Substring(machine_Schedule_ConfigDTOs[1].LastIndexOf("=") + 1);
            string StartTime = machine_Schedule_ConfigDTOs[2].Substring(machine_Schedule_ConfigDTOs[2].LastIndexOf("=") + 1);
            string EndTime = machine_Schedule_ConfigDTOs[3].Substring(machine_Schedule_ConfigDTOs[3].LastIndexOf("=") + 1);
            machine_Schedule_ConfigDTO.StarTime = Convert.ToDateTime(StartTime);
            machine_Schedule_ConfigDTO.EndTime = Convert.ToDateTime(EndTime);
            machine_Schedule_ConfigDTO.MES_Customer_Name = Customer;
            machine_Schedule_ConfigDTO.MES_Station_Name = Station;
            
            machine_Schedule_ConfigDTO.Machine_Station_UID = GetMachine_Station_UID(Customer, Station);
            machine_Schedule_ConfigDTO.Created_Date = DateTime.Now;


            return machine_Schedule_ConfigDTO;
        }


        public  int GetMachine_Station_UID(string MES_Customer_Name,string MES_Station_Name)
        {

            var apiUrl = string.Format("MachineYieldReport/GetStationDTOListAPI?machine_Customer={0}&station_Name={1}", MES_Customer_Name, MES_Station_Name);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var machine_StationDTO = JsonConvert.DeserializeObject<Machine_StationDTO>(result);
            return   machine_StationDTO.Machine_Station_UID;
        }
           


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
    }
    public class MachineYield
    {
        public string Customer { get; set; }
        public string Station { get; set; }
        public string Machine { get; set; }
        public int Input { get; set; }
        public int NG { get; set; }
        public int NGP { get; set; }
    }
}