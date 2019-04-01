using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IMachine_YieldRepository : IRepository<Machine_Yield>
    {
        IQueryable<Machine_YieldDTO> QueryMachine_Yields(Machine_YieldDTO searchModel, Page page, out int totalcount);
        List<Machine_YieldDTO> ExportMachineReprot(Machine_YieldDTO searchModel);
        string InsertMachine_YieldAndMachineConfig(List<Machine_YieldDTO> Machine_YieldDTOs, List<Machine_Schedule_ConfigDTO> Machine_Schedule_ConfigDTOs);
        IQueryable<Machine_CustomerDTO> QueryMachine_CustomerInfo(Machine_CustomerDTO searchModel, Page page, out int totalcount);
        List<Machine_CustomerDTO> QueryMachine_Customer(int Machine_Customer_UID);

        List<Machine_StationDTO> QueryStations(int Machine_Customer_UID);
        List<Machine_CustomerDTO> GetCustomerList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<Machine_StationDTO> GetStationList(int Machine_Customer_UID);

        Machine_StationDTO GetStationDTOList(string machine_Customer, string station_Name);

    }
    public class Machine_YieldRepository : RepositoryBase<Machine_Yield>, IMachine_YieldRepository
    {

        public Machine_YieldRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
        public IQueryable<Machine_YieldDTO> QueryMachine_Yields(Machine_YieldDTO searchModel, Page page, out int totalcount)
        {
            var query = from machine_Yield in DataContext.Machine_Yield
                        select new Machine_YieldDTO
                        {
                            Machine_Yield_UID = machine_Yield.Machine_Yield_UID,
                            Machine_Station_UID = machine_Yield.Machine_Station_UID,
                            Machine_ID = machine_Yield.Machine_ID,
                            InPut_Qty = machine_Yield.InPut_Qty,
                            NG_Qty = machine_Yield.NG_Qty,
                            NG_Point_Qty = machine_Yield.NG_Point_Qty,
                            StarTime = machine_Yield.StarTime,
                            EndTime = machine_Yield.EndTime,
                            Yield_Qty = machine_Yield.Yield_Qty,
                            Yield = machine_Yield.Yield,
                            NO_Yield = machine_Yield.NO_Yield,
                            Created_Date = machine_Yield.Created_Date,
                            MES_Customer_Name = machine_Yield.Machine_Station.Machine_Customer.MES_Customer_Name,
                            PIS_Customer_Name = machine_Yield.Machine_Station.Machine_Customer.PIS_Customer_Name,
                            MES_Station_Name = machine_Yield.Machine_Station.MES_Station_Name,
                            PIS_Station_Name = machine_Yield.Machine_Station.PIS_Station_Name,
                            Machine_Customer_UID = machine_Yield.Machine_Station.Machine_Customer.Machine_Customer_UID,
                            Plant_Organization_UID = machine_Yield.Machine_Station.Plant_Organization_UID,
                            BG_Organization_UID = machine_Yield.Machine_Station.BG_Organization_UID,
                            FunPlant_Organization_UID = machine_Yield.Machine_Station.FunPlant_Organization_UID,
                            Plant_Organization = machine_Yield.Machine_Station.System_Organization.Organization_Name,
                            BG_Organization = machine_Yield.Machine_Station.System_Organization1.Organization_Name,
                            FunPlant_Organization = machine_Yield.Machine_Station.System_Organization2.Organization_Name
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Machine_Customer_UID != 0)
                query = query.Where(m => m.Machine_Customer_UID == searchModel.Machine_Customer_UID);
            if (searchModel.Machine_Station_UID != 0)
                query = query.Where(m => m.Machine_Station_UID == searchModel.Machine_Station_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.MES_Customer_Name))
                query = query.Where(m => m.MES_Customer_Name == searchModel.MES_Customer_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.PIS_Customer_Name))
                query = query.Where(m => m.PIS_Customer_Name == searchModel.PIS_Customer_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.MES_Station_Name))
                query = query.Where(m => m.MES_Station_Name == searchModel.MES_Station_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.PIS_Station_Name))
                query = query.Where(m => m.PIS_Station_Name == searchModel.PIS_Station_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.Machine_ID))
                query = query.Where(m => m.Machine_ID == searchModel.Machine_ID);
            if (searchModel.StarTime != null)
                query = query.Where(m => m.StarTime >= searchModel.StarTime);
            if (searchModel.EndTime != null)
            {
                // DateTime nextTime = searchModel.EndTime.Value.Date.AddDays(1);
                query = query.Where(m => m.EndTime <= searchModel.EndTime);
            }
            query = query.Where(m => m.Machine_ID != "" && m.Machine_ID != null);
            //totalcount = query.Count();
            //query = query.OrderByDescending(m => m.Created_Date).GetPage(page);
            //return query;

            //分组求和；           
            //var querysum = from item in query
            var querysum = from item in query.ToList()
                           group item by new { weno = item.Machine_Station_UID, wename = item.Machine_ID } into g
                           select new Machine_YieldDTO()
                           {
                               Machine_Station_UID = g.Select(o => o.Machine_Station_UID).First(),
                               Machine_ID = g.Select(o => o.Machine_ID).First(),
                               InPut_Qty = g.Sum(o => o.InPut_Qty),
                               NG_Qty = g.Sum(o => o.NG_Qty),
                               NG_Point_Qty = g.Sum(o => o.NG_Point_Qty),
                               MES_Customer_Name = g.Select(o => o.MES_Customer_Name).First(),
                               PIS_Customer_Name = g.Select(o => o.PIS_Customer_Name).First(),
                               MES_Station_Name = g.Select(o => o.MES_Station_Name).First(),
                               PIS_Station_Name = g.Select(o => o.PIS_Station_Name).First(),
                               Machine_Customer_UID = g.Select(o => o.Machine_Customer_UID).First(),
                               Plant_Organization_UID = g.Select(o => o.Plant_Organization_UID).First(),
                               BG_Organization_UID = g.Select(o => o.BG_Organization_UID).First(),
                               FunPlant_Organization_UID = g.Select(o => o.FunPlant_Organization_UID).First(),
                               Plant_Organization = g.Select(o => o.Plant_Organization).First(),
                               BG_Organization = g.Select(o => o.BG_Organization).First(),
                               FunPlant_Organization = g.Select(o => o.FunPlant_Organization).First()
                           };

            //求和的结果查询出来，之后再求算良率，不良率，良品数等。

            var queryreturn = SetMachine_YieldDTO(querysum.ToList());
            totalcount = queryreturn.Count();
            queryreturn = queryreturn.OrderByDescending(m => m.Machine_Station_UID).GetPage(page);
            return queryreturn;
        }

        public IQueryable<Machine_YieldDTO> SetMachine_YieldDTO(List<Machine_YieldDTO> Machine_YieldDTOs)
        {

            foreach (var item in Machine_YieldDTOs)
            {

                item.Yield_Qty = item.InPut_Qty - item.NG_Qty;
                if (item.InPut_Qty == 0)
                {
                    item.Yield = 1;
                    item.NO_Yield = 1;
                }
                else
                {
                    int Yield_QTY = item.InPut_Qty.Value - item.NG_Qty.Value;
                    item.Yield = (decimal)Yield_QTY / (decimal)item.InPut_Qty.Value;
                    item.NO_Yield = (decimal)item.NG_Point_Qty.Value / (decimal)item.InPut_Qty;
                }

            }
            return Machine_YieldDTOs.AsQueryable();
        }

        public List<Machine_YieldDTO> ExportMachineReprot(Machine_YieldDTO searchModel)
        {

            var query = from machine_Yield in DataContext.Machine_Yield
                        select new Machine_YieldDTO
                        {
                            Machine_Yield_UID = machine_Yield.Machine_Yield_UID,
                            Machine_Station_UID = machine_Yield.Machine_Station_UID,
                            Machine_ID = machine_Yield.Machine_ID,
                            InPut_Qty = machine_Yield.InPut_Qty,
                            NG_Qty = machine_Yield.NG_Qty,
                            NG_Point_Qty = machine_Yield.NG_Point_Qty,
                            StarTime = machine_Yield.StarTime,
                            EndTime = machine_Yield.EndTime,
                            Yield_Qty = machine_Yield.Yield_Qty,
                            Yield = machine_Yield.Yield,
                            NO_Yield = machine_Yield.NO_Yield,
                            Created_Date = machine_Yield.Created_Date,
                            MES_Customer_Name = machine_Yield.Machine_Station.Machine_Customer.MES_Customer_Name,
                            PIS_Customer_Name = machine_Yield.Machine_Station.Machine_Customer.PIS_Customer_Name,
                            MES_Station_Name = machine_Yield.Machine_Station.MES_Station_Name,
                            PIS_Station_Name = machine_Yield.Machine_Station.PIS_Station_Name,
                            Machine_Customer_UID = machine_Yield.Machine_Station.Machine_Customer.Machine_Customer_UID,
                            Plant_Organization_UID = machine_Yield.Machine_Station.Plant_Organization_UID,
                            BG_Organization_UID = machine_Yield.Machine_Station.BG_Organization_UID,
                            FunPlant_Organization_UID = machine_Yield.Machine_Station.FunPlant_Organization_UID
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Machine_Customer_UID != 0)
                query = query.Where(m => m.Machine_Customer_UID == searchModel.Machine_Customer_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.MES_Customer_Name))
                query = query.Where(m => m.MES_Customer_Name == searchModel.MES_Customer_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.PIS_Customer_Name))
                query = query.Where(m => m.PIS_Customer_Name == searchModel.PIS_Customer_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.MES_Station_Name))
                query = query.Where(m => m.MES_Station_Name == searchModel.MES_Station_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.PIS_Station_Name))
                query = query.Where(m => m.PIS_Station_Name == searchModel.PIS_Station_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.Machine_ID))
                query = query.Where(m => m.Machine_ID == searchModel.Machine_ID);
            if (searchModel.StarTime != null)
                query = query.Where(m => m.StarTime >= searchModel.StarTime);
            if (searchModel.EndTime != null)
            {
                DateTime nextTime = searchModel.EndTime.Value.Date.AddDays(1);
                query = query.Where(m => m.EndTime < nextTime);
            }
            return query.ToList();
        }


        public string InsertMachine_YieldAndMachineConfig(List<Machine_YieldDTO> Machine_YieldDTOs, List<Machine_Schedule_ConfigDTO> Machine_Schedule_ConfigDTOs)
        {

            try
            {

                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in Machine_YieldDTOs)
                    {

                        //构造机台良率插入SQL数据

                        var insertSql = string.Format(@"INSERT INTO Machine_Yield
                                                       (Machine_Station_UID
                                                       ,Machine_ID
                                                       ,InPut_Qty
                                                       ,NG_Qty
                                                       ,NG_Point_Qty
                                                       ,StarTime
                                                       ,EndTime             
                                                       ,Created_Date)
                                                       VALUES
                                                       ({0},
                                                      N'{1}',
                                                        {2},
                                                        {3},
                                                        {4},
                                                       '{5}',
                                                       '{6}',        
                                                       '{7}');", item.Machine_Station_UID, item.Machine_ID, item.InPut_Qty, item.NG_Qty, item.NG_Point_Qty, item.StarTime, item.EndTime, item.Created_Date);
                        sb.AppendLine(insertSql);

                    }

                    foreach (var item in Machine_Schedule_ConfigDTOs)
                    {
                        //构造机台同步配置插入SQL数据
                        var insertSql = string.Format(@"INSERT INTO Machine_Schedule_Config
                                                       (Machine_Station_UID
                                                       ,Created_Date
                                                       ,StarTime
                                                       ,EndTime)
                                                       VALUES
                                                       ({0}
                                                       ,'{1}'
                                                       ,'{2}'
                                                       ,'{3}');", item.Machine_Station_UID, item.Created_Date, item.StarTime, item.EndTime);

                        sb.AppendLine(insertSql);
                    }

                    string sql = sb.ToString();
                    DataContext.Database.ExecuteSqlCommand(sb.ToString());
                    trans.Commit();
                }

                return "SUCCESS";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public IQueryable<Machine_CustomerDTO> QueryMachine_CustomerInfo(Machine_CustomerDTO searchModel, Page page, out int totalcount)
        {
            var query = from machine_Customer in DataContext.Machine_Customer
                        select new Machine_CustomerDTO
                        {
                            Machine_Customer_UID = machine_Customer.Machine_Customer_UID,
                            MES_Customer_Name = machine_Customer.MES_Customer_Name,
                            PIS_Customer_Name = machine_Customer.PIS_Customer_Name,
                            Created_UID = machine_Customer.Created_UID,
                            Created_Date = machine_Customer.Created_Date,
                            Modified_UID = machine_Customer.Modified_UID,
                            Modified_Date = machine_Customer.Modified_Date,
                            Is_Enable = machine_Customer.Is_Enable,
                            Plant_Organization_UID = machine_Customer.Plant_Organization_UID,
                            BG_Organization_UID = machine_Customer.BG_Organization_UID,
                            FunPlant_Organization_UID = machine_Customer.FunPlant_Organization_UID,
                            Plant_Organization = machine_Customer.System_Organization.Organization_Name,
                            BG_Organization = machine_Customer.System_Organization1.Organization_Name,
                            FunPlant_Organization = machine_Customer.System_Organization2.Organization_Name,
                            Createder = machine_Customer.System_Users.User_Name,
                            Modifieder = machine_Customer.System_Users1.User_Name,
                            DataSourceType = machine_Customer.DataSourceType
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.MES_Customer_Name))
                query = query.Where(m => m.MES_Customer_Name == searchModel.MES_Customer_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.PIS_Customer_Name))
                query = query.Where(m => m.PIS_Customer_Name == searchModel.PIS_Customer_Name);
            if (searchModel.Is_Enable != null)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);
            if (searchModel.DataSourceType != null)
                query = query.Where(m => m.DataSourceType == searchModel.DataSourceType);

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;
        }
        public List<Machine_CustomerDTO> QueryMachine_Customer(int Machine_Customer_UID)
        {
            var query = from machine_Customer in DataContext.Machine_Customer
                        select new Machine_CustomerDTO
                        {
                            Machine_Customer_UID = machine_Customer.Machine_Customer_UID,
                            MES_Customer_Name = machine_Customer.MES_Customer_Name,
                            PIS_Customer_Name = machine_Customer.PIS_Customer_Name,
                            Created_UID = machine_Customer.Created_UID,
                            Created_Date = machine_Customer.Created_Date,
                            Modified_UID = machine_Customer.Modified_UID,
                            Modified_Date = machine_Customer.Modified_Date,
                            Is_Enable = machine_Customer.Is_Enable,
                            Plant_Organization_UID = machine_Customer.Plant_Organization_UID,
                            BG_Organization_UID = machine_Customer.BG_Organization_UID,
                            FunPlant_Organization_UID = machine_Customer.FunPlant_Organization_UID,
                            Plant_Organization = machine_Customer.System_Organization.Organization_Name,
                            BG_Organization = machine_Customer.System_Organization1.Organization_Name,
                            FunPlant_Organization = machine_Customer.System_Organization2.Organization_Name,
                            Createder = machine_Customer.System_Users.User_Name,
                            Modifieder = machine_Customer.System_Users1.User_Name,
                            DataSourceType= machine_Customer.DataSourceType
                        };
            if (Machine_Customer_UID != 0)
                query = query.Where(m => m.Machine_Customer_UID == Machine_Customer_UID);
            return query.ToList();
        }
        public List<Machine_StationDTO> QueryStations(int Machine_Customer_UID)
        {

            var query = from machine_Station in DataContext.Machine_Station
                        select new Machine_StationDTO
                        {
                            Machine_Station_UID = machine_Station.Machine_Station_UID,
                            Machine_Customer_UID = machine_Station.Machine_Customer_UID,
                            MES_Station_Name = machine_Station.MES_Station_Name,
                            PIS_Station_Name = machine_Station.PIS_Station_Name,
                            Created_UID = machine_Station.Created_UID,
                            Created_Date = machine_Station.Created_Date,
                            Is_Enable = machine_Station.Is_Enable,
                            Plant_Organization_UID = machine_Station.Plant_Organization_UID,
                            BG_Organization_UID = machine_Station.BG_Organization_UID,
                            FunPlant_Organization_UID = machine_Station.FunPlant_Organization_UID,
                            MES_Customer_Name = machine_Station.Machine_Customer.MES_Customer_Name,
                            PIS_Customer_Name = machine_Station.Machine_Customer.PIS_Customer_Name,
                            Plant_Organization = machine_Station.System_Organization.Organization_Name,
                            BG_Organization = machine_Station.System_Organization1.Organization_Name,
                            FunPlant_Organization = machine_Station.System_Organization2.Organization_Name,
                            Createder = machine_Station.System_Users.User_Name,
                            Modifieder = machine_Station.System_Users1.User_Name,
                        };
            if (Machine_Customer_UID != 0)
                query = query.Where(o => o.Machine_Customer_UID == Machine_Customer_UID);
            return query.ToList();

        }

        public List<Machine_CustomerDTO> GetCustomerList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from machine_Customer in DataContext.Machine_Customer
                        select new Machine_CustomerDTO
                        {
                            Machine_Customer_UID = machine_Customer.Machine_Customer_UID,
                            MES_Customer_Name = machine_Customer.MES_Customer_Name,
                            PIS_Customer_Name = machine_Customer.PIS_Customer_Name,
                            Created_UID = machine_Customer.Created_UID,
                            Created_Date = machine_Customer.Created_Date,
                            Modified_UID = machine_Customer.Modified_UID,
                            Modified_Date = machine_Customer.Modified_Date,
                            Is_Enable = machine_Customer.Is_Enable,
                            Plant_Organization_UID = machine_Customer.Plant_Organization_UID,
                            BG_Organization_UID = machine_Customer.BG_Organization_UID,
                            FunPlant_Organization_UID = machine_Customer.FunPlant_Organization_UID,
                            Plant_Organization = machine_Customer.System_Organization.Organization_Name,
                            BG_Organization = machine_Customer.System_Organization1.Organization_Name,
                            FunPlant_Organization = machine_Customer.System_Organization2.Organization_Name,
                            Createder = machine_Customer.System_Users.User_Name,
                            Modifieder = machine_Customer.System_Users1.User_Name

                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();

        }
        public List<Machine_StationDTO> GetStationList(int Machine_Customer_UID)
        {
            var query = from machine_Station in DataContext.Machine_Station
                        select new Machine_StationDTO
                        {
                            Machine_Station_UID = machine_Station.Machine_Station_UID,
                            Machine_Customer_UID = machine_Station.Machine_Customer_UID,
                            MES_Station_Name = machine_Station.MES_Station_Name,
                            PIS_Station_Name = machine_Station.PIS_Station_Name,
                            Created_UID = machine_Station.Created_UID,
                            Created_Date = machine_Station.Created_Date,
                            Is_Enable = machine_Station.Is_Enable,
                            Plant_Organization_UID = machine_Station.Plant_Organization_UID,
                            BG_Organization_UID = machine_Station.BG_Organization_UID,
                            FunPlant_Organization_UID = machine_Station.FunPlant_Organization_UID,
                            MES_Customer_Name = machine_Station.Machine_Customer.MES_Customer_Name,
                            PIS_Customer_Name = machine_Station.Machine_Customer.PIS_Customer_Name,
                            Plant_Organization = machine_Station.System_Organization.Organization_Name,
                            BG_Organization = machine_Station.System_Organization1.Organization_Name,
                            FunPlant_Organization = machine_Station.System_Organization2.Organization_Name,
                            Createder = machine_Station.System_Users.User_Name,
                            Modifieder = machine_Station.System_Users1.User_Name,
                        };
            if (Machine_Customer_UID != 0)
                query = query.Where(o => o.Machine_Customer_UID == Machine_Customer_UID);
            return query.ToList();
        }

        public Machine_StationDTO GetStationDTOList(string machine_Customer, string station_Name)
        {

            var query = from machine_Station in DataContext.Machine_Station
                        select new Machine_StationDTO
                        {
                            Machine_Station_UID = machine_Station.Machine_Station_UID,
                            Machine_Customer_UID = machine_Station.Machine_Customer_UID,
                            MES_Station_Name = machine_Station.MES_Station_Name,
                            PIS_Station_Name = machine_Station.PIS_Station_Name,
                            Created_UID = machine_Station.Created_UID,
                            Created_Date = machine_Station.Created_Date,
                            Is_Enable = machine_Station.Is_Enable,
                            Plant_Organization_UID = machine_Station.Plant_Organization_UID,
                            BG_Organization_UID = machine_Station.BG_Organization_UID,
                            FunPlant_Organization_UID = machine_Station.FunPlant_Organization_UID,
                            MES_Customer_Name = machine_Station.Machine_Customer.MES_Customer_Name,
                            PIS_Customer_Name = machine_Station.Machine_Customer.PIS_Customer_Name,
                            Plant_Organization = machine_Station.System_Organization.Organization_Name,
                            BG_Organization = machine_Station.System_Organization1.Organization_Name,
                            FunPlant_Organization = machine_Station.System_Organization2.Organization_Name,
                            Createder = machine_Station.System_Users.User_Name,
                            Modifieder = machine_Station.System_Users1.User_Name,
                        };
            if (!string.IsNullOrWhiteSpace(machine_Customer))
                query = query.Where(m => m.MES_Customer_Name == machine_Customer);
            if (!string.IsNullOrWhiteSpace(station_Name))
                query = query.Where(m => m.MES_Station_Name == station_Name);
            return query.FirstOrDefault();
        }

    }
}
