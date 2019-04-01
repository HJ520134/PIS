using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Fixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace PDMS.Service
{
    public interface IMachineYieldReportService
    {
        PagedListModel<Machine_YieldDTO> QueryMachine_Yields(Machine_YieldDTO searchModel, Page page);
        List<Machine_YieldDTO> ExportMachineReprot(Machine_YieldDTO searchModel);
        List<Machine_Schedule_ConfigDTO> GetMachine_Schedule_ConfigDTO();
        List<Machine_Schedule_ConfigDTO> GetNOMachine_Schedule_ConfigDTO();
        string InsertMachine_YieldAndMachineConfig(List<Machine_YieldDTO> Machine_YieldDTOs, List<Machine_Schedule_ConfigDTO> Machine_Schedule_ConfigDTOs);

        PagedListModel<Machine_CustomerDTO> QueryMachine_CustomerInfo(Machine_CustomerDTO searchModel, Page page);

        List<Machine_CustomerDTO> QueryMachine_Customer(int Machine_Customer_UID);
        List<Machine_StationDTO> QueryStations(int Machine_Customer_UID);

        string AddOrEditCustomerAndStationInfo(Machine_CustomerAndStationDTO dto);
        List<Machine_CustomerDTO> GetCustomerList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<Machine_StationDTO> GetStationList(int Machine_Customer_UID);
        string DeleteStation(int Machine_Station_UID);
        string DeleteCustomer(int Machine_Customer_UID);
        Machine_StationDTO GetStationDTOList(string machine_Customer, string station_Name);

        Machine_Station_CustomerDTO GetStationList(int flowChartMaster, int flowChartDetialID);

        MES_NGAPIParam GetMesNGParam(QADetailSearch model, Machine_Station_CustomerDTO stationInfo);
    }
    public class MachineYieldReportService : IMachineYieldReportService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMachine_YieldRepository machine_YieldRepository;
        private readonly IMachine_Schedule_ConfigRepository machine_Schedule_ConfigRepository;
        private readonly IMachine_CustomerRepository machine_CustomerRepository;
        private readonly IMachine_StationRepository machine_StationRepository;
        public MachineYieldReportService(IMachine_YieldRepository machine_YieldRepository, IUnitOfWork unitOfWork, IMachine_Schedule_ConfigRepository machine_Schedule_ConfigRepository, IMachine_StationRepository machine_StationRepository, IMachine_CustomerRepository machine_CustomerRepository)
        {
            this.unitOfWork = unitOfWork;
            this.machine_YieldRepository = machine_YieldRepository;
            this.machine_Schedule_ConfigRepository = machine_Schedule_ConfigRepository;
            this.machine_CustomerRepository = machine_CustomerRepository;
            this.machine_StationRepository = machine_StationRepository;

        }

        public PagedListModel<Machine_YieldDTO> QueryMachine_Yields(Machine_YieldDTO searchModel, Page page)
        {
            int totalcount;
            var result = machine_YieldRepository.QueryMachine_Yields(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<Machine_YieldDTO>(totalcount, result);
            return bd;
        }
        public List<Machine_YieldDTO> ExportMachineReprot(Machine_YieldDTO searchModel)
        {
            var result = machine_YieldRepository.ExportMachineReprot(searchModel);
            return result.ToList();
        }
        public List<Machine_Schedule_ConfigDTO> GetMachine_Schedule_ConfigDTO()
        {
            var result = machine_Schedule_ConfigRepository.GetMachine_Schedule_ConfigDTO();
            return result.ToList();
        }

        public List<Machine_Schedule_ConfigDTO> GetNOMachine_Schedule_ConfigDTO()
        {
            var result = machine_Schedule_ConfigRepository.GetNOMachine_Schedule_ConfigDTO();
            return result.ToList();
        }
        public string InsertMachine_YieldAndMachineConfig(List<Machine_YieldDTO> Machine_YieldDTOs, List<Machine_Schedule_ConfigDTO> Machine_Schedule_ConfigDTOs)
        {
            return machine_YieldRepository.InsertMachine_YieldAndMachineConfig(Machine_YieldDTOs, Machine_Schedule_ConfigDTOs);

        }
        public PagedListModel<Machine_CustomerDTO> QueryMachine_CustomerInfo(Machine_CustomerDTO searchModel, Page page)
        {
            int totalcount;
            var result = machine_YieldRepository.QueryMachine_CustomerInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<Machine_CustomerDTO>(totalcount, result);
            return bd;
        }
        public List<Machine_CustomerDTO> GetCustomerList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var bud = machine_YieldRepository.GetCustomerList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID).ToList();
            return bud;
        }
        public List<Machine_StationDTO> GetStationList(int Machine_Customer_UID)
        {
            var bud = machine_YieldRepository.GetStationList(Machine_Customer_UID).ToList();
            return bud;
        }
        public List<Machine_CustomerDTO> QueryMachine_Customer(int Machine_Customer_UID)
        {

            var bud = machine_YieldRepository.QueryMachine_Customer(Machine_Customer_UID).ToList();
            return bud;
        }
        public List<Machine_StationDTO> QueryStations(int Machine_Customer_UID)
        {
            var bud = machine_YieldRepository.QueryStations(Machine_Customer_UID).ToList();
            return bud;
        }
        public Machine_StationDTO GetStationDTOList(string machine_Customer, string station_Name)
        {
            var bud = machine_YieldRepository.GetStationDTOList(machine_Customer, station_Name);
            return bud;
        }
        public string AddOrEditCustomerAndStationInfo(Machine_CustomerAndStationDTO dto)
        {
            try
            {
                var warehouse = machine_CustomerRepository.GetById(dto.Machine_Customer_UID);
                if (warehouse != null)
                {
                    var ware = machine_CustomerRepository.GetById(dto.Machine_Customer_UID);
                    ware.Plant_Organization_UID = dto.Plant_Organization_UID;
                    ware.BG_Organization_UID = dto.BG_Organization_UID;
                    ware.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    ware.PIS_Customer_Name = dto.PIS_Customer_Name;
                    ware.MES_Customer_Name = dto.MES_Customer_Name;

                    ware.Is_Enable = dto.Is_Enable.Value;
                    //ware.Created_Date = DateTime.Now;
                    //ware.Created_UID = dto.Created_UID;
                    ware.Modified_UID = dto.Modified_UID;
                    ware.Modified_Date = DateTime.Now;
                    ware.DataSourceType = dto.DataSourceType;
                    machine_CustomerRepository.Update(ware);
                    unitOfWork.Commit();
                }
                else
                {
                    Machine_Customer ware = new Machine_Customer();
                    ware.Plant_Organization_UID = dto.Plant_Organization_UID;
                    ware.BG_Organization_UID = dto.BG_Organization_UID;
                    ware.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    ware.PIS_Customer_Name = dto.PIS_Customer_Name;
                    ware.MES_Customer_Name = dto.MES_Customer_Name;
                    ware.Is_Enable = dto.Is_Enable.Value;
                    ware.Created_Date = DateTime.Now;
                    ware.Created_UID = dto.Created_UID;
                    ware.Modified_UID = dto.Modified_UID;
                    ware.Modified_Date = DateTime.Now;
                    ware.DataSourceType = dto.DataSourceType;
                    machine_CustomerRepository.Add(ware);
                    unitOfWork.Commit();
                }
                foreach (var item in dto.Storages)
                {
                    if (dto.Machine_Customer_UID == 0)
                    {
                        var war = machine_CustomerRepository.GetFirstOrDefault(m => m.BG_Organization_UID == dto.BG_Organization_UID &&
                                            m.Plant_Organization_UID == dto.Plant_Organization_UID && m.MES_Customer_Name == dto.MES_Customer_Name && m.PIS_Customer_Name == dto.PIS_Customer_Name);
                        dto.Machine_Customer_UID = war.Machine_Customer_UID;
                    }
                    if (item.Machine_Station_UID == 0)
                    {
                        Machine_Station ws = new Machine_Station();
                        ws.Plant_Organization_UID = dto.Plant_Organization_UID;
                        ws.BG_Organization_UID = dto.BG_Organization_UID;
                        ws.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                        ws.Machine_Customer_UID = dto.Machine_Customer_UID;
                        ws.PIS_Station_Name = item.PIS_Station_Name;
                        ws.MES_Station_Name = item.MES_Station_Name;
                        ws.Is_Enable = item.Is_Enable;
                        ws.Created_Date = DateTime.Now;
                        ws.Created_UID = dto.Created_UID;
                        ws.Modified_UID = dto.Modified_UID;
                        ws.Modified_Date = DateTime.Now;

                        var hasdata = machine_StationRepository.GetFirstOrDefault(m => m.Machine_Customer_UID == ws.Machine_Customer_UID
                                                    && m.PIS_Station_Name == ws.PIS_Station_Name && m.MES_Station_Name == ws.MES_Station_Name);
                        if (hasdata != null)
                            return "更新站点信息失败:已经存在,不可重复添加";
                        machine_StationRepository.Add(ws);
                        unitOfWork.Commit();
                    }
                    else
                    {
                        var warehousestorate = machine_StationRepository.GetById(item.Machine_Station_UID);
                        warehousestorate.Plant_Organization_UID = dto.Plant_Organization_UID;
                        warehousestorate.BG_Organization_UID = dto.BG_Organization_UID;
                        warehousestorate.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                        warehousestorate.Machine_Customer_UID = dto.Machine_Customer_UID;
                        warehousestorate.PIS_Station_Name = item.PIS_Station_Name;
                        warehousestorate.MES_Station_Name = item.MES_Station_Name;
                        warehousestorate.Is_Enable = item.Is_Enable;
                        //warehousestorate.Created_Date = DateTime.Now;
                        //warehousestorate.Created_UID = dto.Created_UID;
                        warehousestorate.Modified_UID = dto.Modified_UID;
                        warehousestorate.Modified_Date = DateTime.Now;
                        machine_StationRepository.Update(warehousestorate);
                        unitOfWork.Commit();
                    }
                }

                return "SUCCESS";
            }
            catch (Exception e)
            {
                return "更新专案信息失败:" + e.Message;
            }
        }
        public string DeleteStation(int Machine_Station_UID)
        {
            string result = "";
            var entity = machine_StationRepository.GetFirstOrDefault(p => p.Machine_Station_UID == Machine_Station_UID);
            if (entity == null)
            {
                result = "此站点已经删除";
            }
            else
            {
                try
                {
                    machine_StationRepository.Delete(entity);
                    unitOfWork.Commit();
                    result = "SUCCESS";
                }
                catch
                {
                    result = "此站点已经被使用,不能删除";
                }
            }
            return result;
        }

        public string DeleteCustomer(int Machine_Customer_UID)
        {
            string result = "";
            var entity = machine_CustomerRepository.GetFirstOrDefault(p => p.Machine_Customer_UID == Machine_Customer_UID);
            if (entity == null)
            {
                result = "此专案已经删除";
            }
            else
            {
                try
                {
                    machine_CustomerRepository.Delete(entity);
                    unitOfWork.Commit();
                    result = "";
                }
                catch
                {
                    result = "此专案已经被使用,不能删除";
                }
            }
            return result;
        }


        public Machine_Station_CustomerDTO GetStationList(int flowChartMaster, int flowChartDetialID)
        {
            return machine_StationRepository.GetMachine_Station_Customer(flowChartMaster, flowChartDetialID);
        }

        public MES_NGAPIParam GetMesNGParam(QADetailSearch model, Machine_Station_CustomerDTO stationInfo)
        {
            var timeInterval = model.Time_interval.Split('-');
            var startTime = timeInterval[0];
            var endTime = timeInterval[1];
            MES_NGAPIParam requestParam = new MES_NGAPIParam();
            requestParam.startTime = Convert.ToDateTime(model.ProductDate + " " + startTime).AddHours(-0.5).ToString("yyyy-MM-dd HH:mm");
            requestParam.endTime = Convert.ToDateTime(model.ProductDate + " " + endTime).AddHours(-0.5).ToString("yyyy-MM-dd HH:mm");
            requestParam.PIS_Customer_Name = stationInfo.PIS_Customer_Name;
            requestParam.MES_Customer_Name = stationInfo.MES_Customer_Name;
            return requestParam;
        }
    }
}
