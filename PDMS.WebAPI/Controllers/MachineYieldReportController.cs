using Newtonsoft.Json;
using PDMS.Core.Authentication;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Fixture;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    public class MachineYieldReportController : ApiController
    {
        IMachineYieldReportService machineYieldReportService;
  
        public MachineYieldReportController(IMachineYieldReportService machineYieldReportService)
        {
            this.machineYieldReportService = machineYieldReportService;
     
        }
     
        [HttpPost]
        public IHttpActionResult QueryMachine_YieldsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<Machine_YieldDTO>(jsondata);
            var result = machineYieldReportService.QueryMachine_Yields(searchModel, page);
            return Ok(result);
        }

        

        [HttpGet]
        public IHttpActionResult ExportMachineReprotAPI(int plantId, int optypeId, int funplantId, string customer, string station, string machine_ID)
        {
            var searchModel = new Machine_YieldDTO();
            searchModel.Plant_Organization_UID = plantId;
            searchModel.BG_Organization_UID = optypeId;            
            searchModel.FunPlant_Organization_UID = funplantId;
            searchModel.PIS_Customer_Name = customer;
            searchModel.PIS_Station_Name = station;
            searchModel.Machine_ID = machine_ID;
            var bus = machineYieldReportService.ExportMachineReprot(searchModel);
            return Ok(bus);
        }
        [HttpPost]
        public IHttpActionResult QueryMachine_CustomerInfoAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<Machine_CustomerDTO>(jsondata);
            var result = machineYieldReportService.QueryMachine_CustomerInfo(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryMachine_CustomerByuidAPI(int Machine_Customer_UID)
        {
            //获取仓库
            var war = machineYieldReportService.QueryMachine_Customer(Machine_Customer_UID)[0];
            //获取储位
            var storages = machineYieldReportService.QueryStations(Machine_Customer_UID);

            var result = new Machine_CustomerAndStationDTO
            {
                Machine_Customer_UID = war.Machine_Customer_UID,
                Plant_Organization_UID = war.Plant_Organization_UID,
                Plant_Organization = war.Plant_Organization,
                BG_Organization_UID = war.BG_Organization_UID,
                BG_Organization = war.BG_Organization,
                FunPlant_Organization_UID = war.FunPlant_Organization_UID,
                FunPlant_Organization = war.FunPlant_Organization,
                MES_Customer_Name = war.MES_Customer_Name,
                PIS_Customer_Name = war.PIS_Customer_Name,            
                Is_Enable = war.Is_Enable,
                Createder = war.Createder,
                Created_UID = war.Created_UID,
                Created_Date = war.Created_Date,
                Modifieder = war.Modifieder,
                Modified_UID = war.Modified_UID,
                Modified_Date = war.Modified_Date,
                DataSourceType = war.DataSourceType,
                Storages = storages
            };
            return Ok(result);
        }

        public string AddOrEditCustomerAndStationInfoAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<Machine_CustomerAndStationDTO>(data.ToString());
            return machineYieldReportService.AddOrEditCustomerAndStationInfo(entity);
        }

        [HttpGet]
        public IHttpActionResult GetCustomerListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = machineYieldReportService.GetCustomerList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetStationListAPI(int Machine_Customer_UID)
        {
            var result = machineYieldReportService.GetStationList(Machine_Customer_UID);
            return Ok(result);
        }

        [HttpGet]
        public string DeleteStationAPI(int Machine_Station_UID)
        {
            return machineYieldReportService.DeleteStation(Machine_Station_UID);
        }
        [HttpGet]
        public string DeleteCustomerAPI(int Machine_Customer_UID)
        {
            return machineYieldReportService.DeleteCustomer(Machine_Customer_UID);
        }
        [HttpPost]
        public string InsertMachine_YieldAndMachineConfigAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<MachineYieldAndMachineConfigDTO>(data.ToString());
            return machineYieldReportService.InsertMachine_YieldAndMachineConfig(entity.Machine_YieldDTOs, entity.Machine_Schedule_ConfigDTOs);

        }
        [HttpGet]
        public IHttpActionResult GetStationDTOListAPI(string machine_Customer, string station_Name)
        {
            var result = machineYieldReportService.GetStationDTOList(machine_Customer, station_Name);
            return Ok(result);         
        }
        
    }
}