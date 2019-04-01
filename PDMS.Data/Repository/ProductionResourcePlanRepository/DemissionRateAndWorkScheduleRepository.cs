using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IDemissionRateAndWorkScheduleRepository : IRepository<DemissionRateAndWorkSchedule>
    {
        List<DemissionRateAndWorkScheduleDTO> QueryTurnoverSchedulingInfo(DemissionRateAndWorkScheduleDTO searchModel, Page page, out int totalCount);

        DemissionRateAndWorkScheduleDTO GetDemissionInfoByID(int demissionID);

        List<DemissionRateAndWorkScheduleDTO> ExportDemissionRateList(DemissionRateAndWorkScheduleDTO searchModel);

        bool DeleteDemissionInfoByID(int demissionID);

        List<Enumeration> GetWorkScheduleList();
     
        /// <summary>
        /// 判断数据是否已经存在
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        bool IsExistSchedule(DemissionRateAndWorkScheduleDTO dto);
    }
    public class DemissionRateAndWorkScheduleRepository : RepositoryBase<DemissionRateAndWorkSchedule>, IDemissionRateAndWorkScheduleRepository
    {
        public DemissionRateAndWorkScheduleRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<DemissionRateAndWorkScheduleDTO> QueryTurnoverSchedulingInfo(DemissionRateAndWorkScheduleDTO searchModel, Page page, out int totalCount)
        {
            string startDate = string.Empty;
            string endDate = string.Empty;

            var sql = @"SELECT DemissionRateAndWorkSchedule_UID,Plant_Organization_UID,B.Organization_Name, BG_Organization_UID,C.Organization_Name AS OPType,
                Product_Phase, Product_Date,DemissionRate_NPI AS DemissionRate_NPI,DemissionRate_MP AS DemissionRate_MP,
                NPI_RecruitStaff_Qty,MP_RecruitStaff_Qty,WorkSchedule FROM dbo.DemissionRateAndWorkSchedule A
                JOIN dbo.System_Organization B ON A.Plant_Organization_UID = B.Organization_UID
                JOIN dbo.System_Organization C ON A.BG_Organization_UID = C.Organization_UID
                WHERE 1 = 1 ";

            if (searchModel.Plant_Organization_UID != 0)
            {
                sql = sql + string.Format("AND Plant_Organization_UID = {0} ", searchModel.Plant_Organization_UID);
            }

            if (searchModel.BG_Organization_UID != 0)
            {
                sql = sql + string.Format("AND BG_Organization_UID = {0}", searchModel.BG_Organization_UID);
            }

            if (searchModel.StartDate != null)
            {
                startDate = searchModel.StartDate.Value.ToString(FormatConstants.DateTimeFormatStringByDate);
                sql = sql + "AND CONVERT(VARCHAR(10),Product_Date,120) >= '" + startDate + "' ";
            }
            if (searchModel.EndDate != null)
            {
                endDate = searchModel.EndDate.Value.ToString(FormatConstants.DateTimeFormatStringByDate);
                sql = sql + "AND CONVERT(VARCHAR(10),Product_Date,120) <= '" + endDate + "' ";
            }

            totalCount = DataContext.Database.SqlQuery<DemissionRateAndWorkScheduleDTO>(sql).ToList().Count();
            var list = DataContext.Database.SqlQuery<DemissionRateAndWorkScheduleDTO>(sql).Skip(page.PageSize * page.PageNumber).Take(page.PageSize).ToList();
            return list;
        }

        public List<DemissionRateAndWorkScheduleDTO> ExportDemissionRateList(DemissionRateAndWorkScheduleDTO searchModel)
        {
            string startDate = string.Empty;
            string endDate = string.Empty;

            var sql = @"SELECT DemissionRateAndWorkSchedule_UID,Plant_Organization_UID,B.Organization_Name, BG_Organization_UID,C.Organization_Name AS OPType,
                Product_Phase, Product_Date,DemissionRate_NPI AS DemissionRate_NPI,DemissionRate_MP AS DemissionRate_MP,
                NPI_RecruitStaff_Qty,MP_RecruitStaff_Qty,WorkSchedule FROM dbo.DemissionRateAndWorkSchedule A
                JOIN dbo.System_Organization B ON A.Plant_Organization_UID = B.Organization_UID
                JOIN dbo.System_Organization C ON A.BG_Organization_UID = C.Organization_UID
                WHERE 1 = 1 ";

            if (searchModel.Plant_Organization_UID != 0)
            {
                sql = sql + string.Format("AND Plant_Organization_UID = {0} ", searchModel.Plant_Organization_UID);
            }

            if (searchModel.BG_Organization_UID != 0)
            {
                sql = sql + string.Format("AND BG_Organization_UID = {0}", searchModel.BG_Organization_UID);
            }

            if (searchModel.StartDate != null)
            {
                startDate = searchModel.StartDate.Value.ToString(FormatConstants.DateTimeFormatStringByDate);
                sql = sql + "AND CONVERT(VARCHAR(10),Product_Date,120) >= '" + startDate + "' ";
            }
            if (searchModel.EndDate != null)
            {
                endDate = searchModel.EndDate.Value.ToString(FormatConstants.DateTimeFormatStringByDate);
                sql = sql + "AND CONVERT(VARCHAR(10),Product_Date,120) <= '" + endDate + "' ";
            }

            return DataContext.Database.SqlQuery<DemissionRateAndWorkScheduleDTO>(sql).ToList();
        }


        public DemissionRateAndWorkScheduleDTO GetDemissionInfoByID(int demissionID)
        {
            var query = from dem in DataContext.DemissionRateAndWorkSchedule
                        select new DemissionRateAndWorkScheduleDTO
                        {
                            DemissionRateAndWorkSchedule_UID = dem.DemissionRateAndWorkSchedule_UID,
                            Plant_Organization_UID = dem.Plant_Organization_UID,
                            Organization_Name = dem.System_Organization.Organization_Name,
                            OPType = dem.System_Organization1.Organization_Name,
                            BG_Organization_UID = dem.BG_Organization_UID,
                            Product_Phase = dem.Product_Phase,
                            Product_Date = dem.Product_Date,
                            DemissionRate_NPI = dem.DemissionRate_NPI,
                            DemissionRate_MP = dem.DemissionRate_MP,
                            NPI_RecruitStaff_Qty = dem.NPI_RecruitStaff_Qty,
                            MP_RecruitStaff_Qty = dem.MP_RecruitStaff_Qty,
                            WorkSchedule = dem.WorkSchedule,
                            Created_UID = dem.Created_UID,
                            Created_Date = dem.Created_Date,
                            Modified_UID = dem.Modified_UID,
                            Modified_Date = dem.Modified_Date
                        };
            query = query.Where(p => p.DemissionRateAndWorkSchedule_UID == demissionID);

            return query.FirstOrDefault();
        }

        public bool IsExistSchedule(DemissionRateAndWorkScheduleDTO dto)
        {
            try
            {
                var query = from dem in DataContext.DemissionRateAndWorkSchedule
                            select new DemissionRateAndWorkScheduleDTO
                            {
                                DemissionRateAndWorkSchedule_UID = dem.DemissionRateAndWorkSchedule_UID,
                                Plant_Organization_UID = dem.Plant_Organization_UID,
                                Organization_Name = dem.System_Organization.Organization_Name,
                                OPType = dem.System_Organization1.Organization_Name,
                                BG_Organization_UID = dem.BG_Organization_UID,
                                Product_Phase = dem.Product_Phase,
                                Product_Date = dem.Product_Date,
                                DemissionRate_NPI = dem.DemissionRate_NPI,
                                DemissionRate_MP = dem.DemissionRate_MP,
                                NPI_RecruitStaff_Qty = dem.NPI_RecruitStaff_Qty,
                                MP_RecruitStaff_Qty = dem.MP_RecruitStaff_Qty,
                                WorkSchedule = dem.WorkSchedule,
                                Created_UID = dem.Created_UID,
                                Created_Date = dem.Created_Date,
                                Modified_UID = dem.Modified_UID,
                                Modified_Date = dem.Modified_Date
                            };
                var repeatList = query.Where(p => p.Plant_Organization_UID == dto.Plant_Organization_UID && p.BG_Organization_UID == dto.BG_Organization_UID && p.Product_Date == dto.Product_Date && p.Product_Phase == dto.Product_Phase);

                if (repeatList.Count() > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        public bool DeleteDemissionInfoByID(int demissionID)
        {
            var sql = $" DELETE from dbo.DemissionRateAndWorkSchedule  WHERE DemissionRateAndWorkSchedule_UID='{demissionID}'";
            var i = DataContext.Database.ExecuteSqlCommand(sql);
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Enumeration> GetWorkScheduleList()
        {
            List<Enumeration> enumerationItems = DataContext.Enumeration.Where(o => o.Enum_Type == "WorkSchedule").ToList();
            return enumerationItems.OrderBy(p => p.Enum_UID).ToList();
        }
    }

}
