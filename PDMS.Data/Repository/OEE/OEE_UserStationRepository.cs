using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;

namespace PDMS.Data.Repository
{
    public interface IOEE_UserStationRepository : IRepository<OEE_UserStation>
    {
        List<OEE_MachineInfoDTO> QueryOEE_UserStation(OEE_MachineInfoDTO serchModel, Page page);

        List<OEE_MachineInfoDTO> GetMachineByStationID(int stationID);

        UserModel GetMachineTimeInfo(OEE_MachineInfoDTO serchModel);
    }

    public class OEE_UserStationRepository : RepositoryBase<OEE_UserStation>, IOEE_UserStationRepository
    {
        public OEE_UserStationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<OEE_MachineInfoDTO> QueryOEE_UserStation(OEE_MachineInfoDTO serchModel, Page page)
        {
            var query = from machine in DataContext.OEE_MachineInfo
                        join station in DataContext.OEE_UserStation
                        on new
                        {
                            machine.StationID,
                            machine.Plant_Organization_UID,
                            machine.BG_Organization_UID,
                        }
                        equals
                        new
                        {
                            station.StationID,
                            station.Plant_Organization_UID,
                            station.BG_Organization_UID,
                        }
                        where station.KeyInNG_User_UID == serchModel.Modify_UID
                        select new OEE_MachineInfoDTO
                        {
                            OEE_MachineInfo_UID = machine.OEE_MachineInfo_UID,
                            Plant_Organization_UID = machine.Plant_Organization_UID,
                            Plant_Organization_Name = machine.System_Organization.Organization_Name,
                            BG_Organization_UID = machine.BG_Organization_UID,
                            BG_Organization_Name = machine.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = machine.Plant_Organization_UID,
                            FunPlant_Organization_Name = machine.System_Organization2.Organization_Name,
                            Project_UID = machine.Project_UID,
                            Project_Name = machine.System_Project.Project_Name,
                            LineID = machine.LineID,
                            Line_Name = machine.GL_Line.LineName,
                            StationID = machine.StationID,
                            Station_Name = machine.GL_Station.StationName,
                            EQP_Uid = machine.EQP_Uid,
                            EQP_EMTSerialNum = machine.Equipment_Info.Equipment,
                            MachineNo = machine.MachineNo,
                            Is_Enable = machine.Is_Enable,
                            Line_Is_Enable = machine.GL_Line.IsEnabled,
                            station_Is_Enable = machine.GL_Station.IsEnabled,
                        };

            query = query.Where(p => p.Is_Enable == true && p.Line_Is_Enable == true && p.station_Is_Enable == true);

            return query.ToList();
            //var List = new List<OEE_MachineInfoDTO>();
            //var currentDay = DateTime.Now.ToString("yyyy-MM-dd");
            //foreach (var item in query.ToList())
            //{
            //    var sql = @"SELECT TOP 1 a.Modify_UID as Modify_UID,(select COUNT(*) from dbo.OEE_DefectCodeDailyNum   where OEE_MachineInfo_UID={0} AND BG_Organization_UID={1} AND Plant_Organization_UID={2}  and ProductDate='{3}' and TimeInterval={4} ) as num, a.Modify_Date as Modify_Date, b.User_Name AS Modify_Name FROM dbo.OEE_DefectCodeDailyNum AS a INNER JOIN dbo.System_Users AS b ON a.Modify_UID=b.Account_UID
            //                WHERE OEE_MachineInfo_UID={0} AND BG_Organization_UID={1} AND Plant_Organization_UID={2} 
            //                ORDER BY Modify_Date DESC";
            //    sql = string.Format(sql, item.OEE_MachineInfo_UID, item.BG_Organization_UID, item.Plant_Organization_UID, currentDay,serchModel.TimeInterval
            //      );
            //    var model = DataContext.Database.SqlQuery<UserModel>(sql.ToString());
            //    if (model.FirstOrDefault() != null)
            //    {
            //        item.Modify_UID = model.FirstOrDefault().Modify_UID;
            //        item.Modify_Name = model.FirstOrDefault().Modify_Name;
            //        item.Modify_Date = model.FirstOrDefault().Modify_Date;
            //        item.TimeInterval = serchModel.TimeInterval;
            //        item.IsSubmit = model.FirstOrDefault().num > 0 ? true : false;
            //    }
            //    List.Add(item);
            //}

            //var totalCount = List.Count();
            //return new PagedListModel<OEE_MachineInfoDTO>(totalCount, List);
        }

        public UserModel GetMachineTimeInfo(OEE_MachineInfoDTO serchModel)
        {
            var List = new List<OEE_MachineInfoDTO>();
            var currentDay = DateTime.Now.ToString("yyyy-MM-dd");
            var sql = @"SELECT TOP 1 a.Modify_UID as Modify_UID,(select COUNT(*) from dbo.OEE_DefectCodeDailyNum   where OEE_MachineInfo_UID={0} AND BG_Organization_UID={1} AND Plant_Organization_UID={2}  and ProductDate='{3}' and TimeInterval='{4}' ) as num, a.Modify_Date as Modify_Date, b.User_Name AS Modify_Name FROM dbo.OEE_DefectCodeDailyNum AS a INNER JOIN dbo.System_Users AS b ON a.Modify_UID=b.Account_UID
                            WHERE OEE_MachineInfo_UID={0} AND BG_Organization_UID={1} AND Plant_Organization_UID={2} 
                            ORDER BY Modify_Date DESC";
            sql = string.Format(sql, serchModel.OEE_MachineInfo_UID, serchModel.BG_Organization_UID, serchModel.Plant_Organization_UID, currentDay, serchModel.TimeInterval
              );
            var model = DataContext.Database.SqlQuery<UserModel>(sql.ToString());
            return model.FirstOrDefault();
        }

        public List<OEE_MachineInfoDTO> GetMachineByStationID(int stationID)
        {
            var query = from machine in DataContext.OEE_MachineInfo
                        where machine.StationID == stationID
                        select new OEE_MachineInfoDTO
                        {
                            OEE_MachineInfo_UID = machine.OEE_MachineInfo_UID,
                            Plant_Organization_UID = machine.Plant_Organization_UID,
                            Plant_Organization_Name = machine.System_Organization.Organization_Name,
                            BG_Organization_UID = machine.BG_Organization_UID,
                            BG_Organization_Name = machine.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = machine.Plant_Organization_UID,
                            FunPlant_Organization_Name = machine.System_Organization2.Organization_Name,
                            Project_UID = machine.Project_UID,
                            Project_Name = machine.System_Project.Project_Name,
                            LineID = machine.LineID,
                            Line_Name = machine.GL_Line.LineName,
                            StationID = machine.StationID,
                            Station_Name = machine.GL_Station.StationName,
                            EQP_Uid = machine.EQP_Uid,
                            EQP_EMTSerialNum = machine.Equipment_Info.Equipment,
                            MachineNo = machine.MachineNo,
                            Is_Enable = machine.Is_Enable,
                            Modify_UID = machine.Modify_UID,
                            Modify_Name = machine.System_Users.User_Name,
                            Modify_Date = machine.Modify_Date
                        };
            return query.ToList();
        }
    }
}
