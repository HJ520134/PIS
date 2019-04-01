using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IMachine_Schedule_ConfigRepository : IRepository<Machine_Schedule_Config>
    {
        List<Machine_Schedule_ConfigDTO> GetMachine_Schedule_ConfigDTO();
        List<Machine_Schedule_ConfigDTO> GetNOMachine_Schedule_ConfigDTO();
    }
    public class Machine_Schedule_ConfigRepository : RepositoryBase<Machine_Schedule_Config>, IMachine_Schedule_ConfigRepository
    {
        public Machine_Schedule_ConfigRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
        public List<Machine_Schedule_ConfigDTO> GetMachine_Schedule_ConfigDTO()
        {
            var query = from machine_Schedule_Config in DataContext.Machine_Schedule_Config
                        select new Machine_Schedule_ConfigDTO
                        {
                            Machine_Schedule_Config_UID = machine_Schedule_Config.Machine_Schedule_Config_UID,
                            Machine_Station_UID = machine_Schedule_Config.Machine_Station_UID,
                            MES_Customer_Name = machine_Schedule_Config.Machine_Station.Machine_Customer.MES_Customer_Name,
                            MES_Station_Name = machine_Schedule_Config.Machine_Station.MES_Station_Name,
                            Created_Date = machine_Schedule_Config.Created_Date,
                            StarTime = machine_Schedule_Config.StarTime,
                            EndTime = machine_Schedule_Config.EndTime,
                            Customer_Is_Enable = machine_Schedule_Config.Machine_Station.Machine_Customer.Is_Enable,
                            Station_Is_Enable = machine_Schedule_Config.Machine_Station.Is_Enable,
                            DataSourceType = machine_Schedule_Config.Machine_Station.Machine_Customer.DataSourceType
                        };


            query = query.Where(o => o.Customer_Is_Enable == true);
            query = query.Where(o => o.Station_Is_Enable == true);
            query = query.Where(o => o.DataSourceType == "MachineProject");
            List<Machine_Schedule_ConfigDTO> machine_Schedule_ConfigDTOs = query.OrderByDescending(o => o.EndTime).ToList();
             return machine_Schedule_ConfigDTOs.Where((x, i) => machine_Schedule_ConfigDTOs.FindIndex(z => z.Machine_Station_UID == x.Machine_Station_UID) == i).ToList();
            // return machineScheduleConfigDTOs.ToList();
            // return query.OrderByDescending(o => o.Machine_Schedule_Config_UID).ToList();
            // return query.ToList();
        }

        public List<Machine_Schedule_ConfigDTO> GetNOMachine_Schedule_ConfigDTO()
        {

            var query = from machine_Station in DataContext.Machine_Station
                        select new Machine_Schedule_ConfigDTO
                        {
                        
                            Machine_Station_UID = machine_Station.Machine_Station_UID,
                            MES_Customer_Name = machine_Station.Machine_Customer.MES_Customer_Name,
                            MES_Station_Name = machine_Station.MES_Station_Name,                    
                            Customer_Is_Enable = machine_Station.Machine_Customer.Is_Enable,
                            Station_Is_Enable = machine_Station.Is_Enable,
                            DataSourceType= machine_Station.Machine_Customer.DataSourceType
                        };
            query = query.Where(o => o.DataSourceType == "MachineProject");
            query = query.Where(o => o.Customer_Is_Enable == true);
            query = query.Where(o => o.Station_Is_Enable == true);
            return query.ToList();
        }
    }
}
