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

    public interface IOEE_MachineDailyDownRecordRepository : IRepository<OEE_MachineDailyDownRecord>
    {
        List<OEE_MachineDailyDownRecordDTO> GetMachineDailyDownRecord(OEE_ReprortSearchModel searchModel);
        void UpdateList(List<OEE_MachineDailyDownRecord> downLists);
        IQueryable<OEE_MachineDailyDownRecordDTO> GetDownTimeDetialsDTO(OEE_ReprortSearchModel searchModel, Page page, out int totalcount);
        List<OEE_MachineDailyDownRecordDTO> GetDTTimeInfo(OEEFourQParamModel searchModel);
    }

    public class OEE_MachineDailyDownRecordRepository : RepositoryBase<OEE_MachineDailyDownRecord>, IOEE_MachineDailyDownRecordRepository
    {
        public OEE_MachineDailyDownRecordRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public void UpdateList(List<OEE_MachineDailyDownRecord> downLists)
        {
            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in downLists)
                    {
                        var sql = $"UPDATE dbo.OEE_MachineDailyDownRecord SET DownTime={item.DownTime} , EndTIme='{item.EndTIme}'  WHERE OEE_MachineDailyDownRecord_UID={item.OEE_MachineDailyDownRecord_UID}";
                        sb.AppendLine(sql.ToString());
                    }

                    DataContext.Database.ExecuteSqlCommand(sb.ToString());
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public List<OEE_MachineDailyDownRecordDTO> GetMachineDailyDownRecord(OEE_ReprortSearchModel searchModel)
        {
            var query = from downRecord in DataContext.OEE_MachineDailyDownRecord
                        where downRecord.BG_Organization_UID == searchModel.BG_Organization_UID
                        && downRecord.Plant_Organization_UID == searchModel.Plant_Organization_UID
                        && downRecord.StationID == searchModel.StationID
                        && downRecord.OEE_MachineInfo_UID == searchModel.EQP_Uid
                        && downRecord.OEE_DownTimeCode.Is_Enable == true
                        select new OEE_MachineDailyDownRecordDTO()
                        {
                            Plant_Organization_UID = downRecord.Plant_Organization_UID,
                            BG_Organization_UID = downRecord.BG_Organization_UID,
                            FunPlant_Organization_UID = downRecord.FunPlant_Organization_UID,
                            StationID = downRecord.StationID,
                            OEE_MachineInfo_UID = downRecord.OEE_MachineInfo_UID,
                            OEE_DownTimeCode_UID = downRecord.OEE_DownTimeCode_UID,
                            DownDate = downRecord.DownDate,
                            ShiftTimeID = downRecord.ShiftTimeID,
                            StartTime = downRecord.StartTime,
                            EndTIme = downRecord.EndTIme,
                            DownTime = downRecord.DownTime,
                            Is_Enable = downRecord.OEE_DownTimeCode.Is_Enable,
                            Type_Name = downRecord.OEE_DownTimeCode.OEE_DownTimeType.Type_Name,
                            OEE_DownTimeType_UID = downRecord.OEE_DownTimeCode.OEE_DownTimeType.OEE_DownTimeType_UID
                        };
            query = query.Where(p => p.DownDate >= searchModel.StartTime && p.DownDate <= searchModel.EndTime);
            //全天
            if (searchModel.ShiftTimeID != -1)
            {
                query = query.Where(p => p.ShiftTimeID == searchModel.ShiftTimeID);
            }
            return query.ToList();
        }


        public IQueryable<OEE_MachineDailyDownRecordDTO> GetDownTimeDetialsDTO(OEE_ReprortSearchModel searchModel, Page page, out int totalcount)
        {
            var query = from downRecord in DataContext.OEE_MachineDailyDownRecord
                        where downRecord.BG_Organization_UID == searchModel.BG_Organization_UID
                        && downRecord.Plant_Organization_UID == searchModel.Plant_Organization_UID
                        && downRecord.StationID == searchModel.StationID
                        && downRecord.OEE_MachineInfo_UID == searchModel.EQP_Uid
                        && downRecord.OEE_DownTimeCode.Is_Enable == true
                        select new OEE_MachineDailyDownRecordDTO()
                        {
                            Plant_Organization_UID = downRecord.Plant_Organization_UID,
                            BG_Organization_UID = downRecord.BG_Organization_UID,
                            FunPlant_Organization_UID = downRecord.FunPlant_Organization_UID,
                            StationID = downRecord.StationID,
                            OEE_MachineInfo_UID = downRecord.OEE_MachineInfo_UID,
                            OEE_DownTimeCode_UID = downRecord.OEE_DownTimeCode_UID,
                            DownDate = downRecord.DownDate,
                            ShiftTimeID = downRecord.ShiftTimeID,
                            StartTime = downRecord.StartTime,
                            EndTIme = downRecord.EndTIme,
                            DownTime = downRecord.DownTime,
                            Is_Enable = downRecord.OEE_DownTimeCode.Is_Enable,
                            Type_Name = downRecord.OEE_DownTimeCode.OEE_DownTimeType.Type_Name,
                            OEE_DownTimeType_UID = downRecord.OEE_DownTimeCode.OEE_DownTimeType.OEE_DownTimeType_UID,
                            Error_Code = downRecord.OEE_DownTimeCode.Error_Code,
                            Upload_Ways = downRecord.OEE_DownTimeCode.Upload_Ways,
                            Level_Details = downRecord.OEE_DownTimeCode.Level_Details,
                        };
            query = query.Where(p => p.OEE_DownTimeType_UID == searchModel.OEE_DownTimeType_UID);
            var DownDate = Convert.ToDateTime(searchModel.StartTime.ToString("yyyy-MM-dd"));
            query = query.Where(p => p.DownDate == DownDate);
            if (searchModel.StationID != -1)
            {
                query = query.Where(p => p.StationID == searchModel.StationID);
            }
            if (searchModel.ShiftTimeID != -1)
            {
                query = query.Where(p => p.ShiftTimeID == searchModel.ShiftTimeID);
            }

            totalcount = query.Count();
            var list = query.ToList();

            List<OEE_MachineDailyDownRecordDTO> modelList = new List<OEE_MachineDailyDownRecordDTO>();
            var rrrorCodeGroup = list.GroupBy(p => p.Error_Code);
            foreach (var item in rrrorCodeGroup)
            {
                OEE_MachineDailyDownRecordDTO model = new OEE_MachineDailyDownRecordDTO();
                model.Error_Code = item.Key;
                model.Level_Details = item.FirstOrDefault().Level_Details;
                model.DownTime = item.Sum(p => p.DownTime);
                modelList.Add(model);
            }

            query = modelList.OrderByDescending(p=>p.DownTime).AsQueryable().GetPage(page);
            return query;
        }

        public List<OEE_MachineDailyDownRecordDTO> GetDTTimeInfo(OEEFourQParamModel searchModel)
        {
            var query = from downRecord in DataContext.OEE_MachineDailyDownRecord
                        where downRecord.BG_Organization_UID == searchModel.BG_Organization_UID
                        && downRecord.Plant_Organization_UID == searchModel.Plant_Organization_UID
                        && downRecord.StationID == searchModel.StationID
                        && downRecord.OEE_MachineInfo_UID == searchModel.OEE_MachineInfo_UID
                        && downRecord.OEE_DownTimeCode.Is_Enable == true
                        select new OEE_MachineDailyDownRecordDTO()
                        {
                            Plant_Organization_UID = downRecord.Plant_Organization_UID,
                            BG_Organization_UID = downRecord.BG_Organization_UID,
                            FunPlant_Organization_UID = downRecord.FunPlant_Organization_UID,
                            StationID = downRecord.StationID,
                            OEE_MachineInfo_UID = downRecord.OEE_MachineInfo_UID,
                            OEE_DownTimeCode_UID = downRecord.OEE_DownTimeCode_UID,
                            DownDate = downRecord.DownDate,
                            ShiftTimeID = downRecord.ShiftTimeID,
                            StartTime = downRecord.StartTime,
                            EndTIme = downRecord.EndTIme,
                            DownTime = downRecord.DownTime,
                            Is_Enable = downRecord.OEE_DownTimeCode.Is_Enable,
                            Type_Name = downRecord.OEE_DownTimeCode.OEE_DownTimeType.Type_Name,
                            OEE_DownTimeType_UID = downRecord.OEE_DownTimeCode.OEE_DownTimeType.OEE_DownTimeType_UID
                        };
            query = query.Where(p => p.DownDate >= searchModel.StartTime && p.DownDate <= searchModel.EndTime);
            return query.ToList();
        }
    }
}
