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
    public interface IOEE_DefectCodeDailyNumDTORepository : IRepository<OEE_DefectCodeDailyNum>
    {
        List<OEE_StationDefectCodeDTO> QueryOEE_DefectCodeDailyNum(OEE_StationDefectCodeDTO serchModel, Page page);
        bool DeleteDailyNum(int dailyNum_uid);
        void UpdateList(List<OEE_DefectCodeDailyNum> downLists);
        List<GL_Rest> getGlRestList(int ShiftTimeID);

        List<GL_Rest> getGlRestListByOP(int OPUID);
    }

    public class OEE_DefectCodeDailyNumDTORepository : RepositoryBase<OEE_DefectCodeDailyNum>, IOEE_DefectCodeDailyNumDTORepository
    {
        public OEE_DefectCodeDailyNumDTORepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

    
        public void UpdateList(List<OEE_DefectCodeDailyNum> downLists)
        {
            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in downLists)
                    {
                        var sql = $"UPDATE dbo.OEE_DefectCodeDailyNum SET DefectNum={item.DefectNum} , Modify_Date='{item.Modify_Date}'  WHERE OEE_DefectCodeDailyNum_UID={item.Modify_Date}";
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
        public bool DeleteDailyNum(int dailyNum_uid)
        {
            try
            {
                var sql = $"delete  from OEE_DefectCodeDailyNum where OEE_DefectCodeDailyNum_UID={dailyNum_uid}";
                var result = DataContext.Database.ExecuteSqlCommand(sql.ToString());
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取指定ShiftTimeID关联下的所有休息时间段数据
        /// </summary>
        /// <param name="ShiftTImeID"></param>
        /// <returns></returns>
        public List<GL_Rest> getGlRestList(int ShiftTimeID)
        {
            var query = from reset in DataContext.GL_Rest
                        where reset.ShiftTimeID == ShiftTimeID
                        orderby reset.SEQ  
                        select reset;
            return query.ToList();
        }

        public List<GL_Rest> getGlRestListByOP(int OPUID)
        {
            var query = from reset in DataContext.GL_Rest
                        where reset.GL_ShiftTime.System_Organization1.Organization_UID==OPUID
                        orderby reset.SEQ 
                        select reset;
            return query.ToList();
        }
        public List<OEE_StationDefectCodeDTO> QueryOEE_DefectCodeDailyNum(OEE_StationDefectCodeDTO serchModel, Page page)
        {
            var query = from Defect in DataContext.OEE_DefectCodeDailyNum
                        where Defect.Plant_Organization_UID == serchModel.Plant_Organization_UID &&
                        Defect.BG_Organization_UID == serchModel.BG_Organization_UID &&
                        Defect.OEE_MachineInfo_UID == serchModel.OEE_MachineInfo_UID &&
                        Defect.ProductDate == serchModel.currentDate &&
                        Defect.TimeInterval == serchModel.currentTimeInterval
                        select new OEE_StationDefectCodeDTO
                        {
                            OEE_DefectCodeDailyNum_UID = Defect.OEE_DefectCodeDailyNum_UID,
                            Plant_Organization_UID = Defect.Plant_Organization_UID,
                            BG_Organization_UID = Defect.BG_Organization_UID,
                            FunPlant_Organization_UID = Defect.FunPlant_Organization_UID,
                            OEE_StationDefectCode_UID = Defect.OEE_StationDefectCode_UID,
                            Defect_Code = Defect.OEE_StationDefectCode.Defect_Code,
                            DefectEnglishName = Defect.OEE_StationDefectCode.DefectEnglishName,
                            DefecChinesetName = Defect.OEE_StationDefectCode.DefecChinesetName,
                            DefectNum = Defect.DefectNum
                        };
            return query.OrderByDescending(p => p.DefectNum).ToList();
        }
    }

}
