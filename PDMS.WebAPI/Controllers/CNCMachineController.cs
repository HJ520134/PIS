using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    public class CNCMachineController : ApiControllerBase
    {


        ICNCMachineService cNCMachineService;
        public CNCMachineController(ICNCMachineService cNCMachineService)
        {
            this.cNCMachineService = cNCMachineService;
        }

        [HttpPost]
        public IHttpActionResult QueryCNCMachineDTOsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<CNCMachineDTO>(jsondata);
            var result = cNCMachineService.QueryCNCMachineDTOs(searchModel, page);
            return Ok(result);
        }

        public string AddOrEditCNCMachineAPI(CNCMachineDTO dto)
        {
            return cNCMachineService.AddOrEditCNCMachineAPI(dto);
        }
        [HttpGet]
        public IHttpActionResult QueryCNCMachineDTOByUidAPI(int CNCMachineUID)
        {
            var result = cNCMachineService.QueryCNCMachineDTOByUid(CNCMachineUID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetSystemProjectDTOAPI(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var dto = cNCMachineService.GetSystemProjectDTO(Plant_Organization_UID, BG_Organization_UID);
            return Ok(dto);
        }
        [HttpGet]
        public string DeleteCNCMachineAPI(int CNCMachineUID, int userid)
        {
            return cNCMachineService.DeleteCNCMachine(CNCMachineUID, userid);
        }

        public IHttpActionResult DoExportCNCMachineReprotAPI(string uids)
        {
            var dto = Ok(cNCMachineService.DoExportCNCMachineReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportCNCMachineReprotAPI(CNCMachineDTO search)
        {
            var result = cNCMachineService.DoAllExportCNCMachineReprot(search);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetAllEquipmentInfoDTOsAPI()
        {
            var result = cNCMachineService.GetAllEquipmentInfoDTOs();
            return Ok(result);
        }
        public IHttpActionResult GetAllCNCMachineDTOListAPI()
        {
            var result = cNCMachineService.GetAllCNCMachineDTOList();
            return Ok(result);
        }
        [HttpPost]
        public string ImportMachineAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<CNCMachineDTO>>(jsondata);
            return cNCMachineService.ImportMachine(list);
        }


        [HttpPost]
        public IHttpActionResult QueryReportCNCMachineDatasAPI(dynamic data)
        {

            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<CNCMachineDTO>(jsondata);
            var result = cNCMachineService.QueryReportCNCMachineDatas(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult DoAllExportMachineReportAPI(CNCMachineDTO search)
        {
            var result = cNCMachineService.DoAllExportMachineReport(search);
            return Ok(result);
        }
        [HttpGet]
        public bool UpdateColumnInfoAPI(int Account_UID, string Column_Name, bool isDisplay)
        {
            var result = cNCMachineService.UpdateColumnInfo(Account_UID, Column_Name, isDisplay);
            return result;
        }
        [HttpGet]
        public IHttpActionResult GetCNCMachineColumnTableDTOsAPI(int Account_UID)
        {
            var result = cNCMachineService.GetCNCMachineColumnTableDTOs(Account_UID);
            return Ok(result);
        }

        [HttpPost]
        public string InsertMachineColumnTableAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<CNCMachineColumnTableDTO>>(jsondata);
            return cNCMachineService.InsertMachineColumnTable(list);
        }
        [HttpGet]
        public IHttpActionResult GetCNCMachineListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var result = cNCMachineService.GetCNCMachineList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult DoHisExportMachineReportAPI(int Plant_Organization_UID, string Machine_Name, DateTime? Date_From ,DateTime? Date_To)
        {

            var result = cNCMachineService.DoHisExportMachineReport(Plant_Organization_UID, Machine_Name, Date_From, Date_To);
            return Ok(result);
        }
        
    }
}