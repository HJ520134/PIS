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
    public interface IOEE_EveryDayMachineDTCodeRepository : IRepository<OEE_EveryDayMachineDTCode>
    {

        List<OEE_EveryDayMachineDTCode> getDTMissingList(string ProductDate, int ShiftTimeID);
        List<OEE_AbnormalDFCode> GetDTcodeMissingList(OEE_ReprortSearchModel serchModel);
    }

    public class OEE_EveryDayMachineDTCodeRepository : RepositoryBase<OEE_EveryDayMachineDTCode>, IOEE_EveryDayMachineDTCodeRepository
    {
        public OEE_EveryDayMachineDTCodeRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        /// <summary>
        /// 获取Defect  Code  因为SA开的表名字有误，按照规范 只能用有误的名词命名。
        /// </summary>
        /// <param name="ProductDate"></param>
        /// <param name="ShiftTimeID"></param>
        /// <returns></returns>
        public List<OEE_EveryDayMachineDTCode> getDTMissingList(string ProductDate, int ShiftTimeID)
        {
            var Product_Date = DateTime.Parse(ProductDate);
            var query = from MissingList in DataContext.OEE_EveryDayMachineDTCode
                        where MissingList.Product_Date == Product_Date
                        && MissingList.ShiftTimeID == ShiftTimeID
                        select MissingList;
            return query.ToList();
        }

        public List<OEE_AbnormalDFCode> GetDTcodeMissingList(OEE_ReprortSearchModel serchModel)
        {
            var query = from MissingList in DataContext.OEE_EveryDayDFcodeMissing
                        where MissingList.OEE_MachineInfo.StationID == serchModel.StationID
                        && MissingList.Product_Date == serchModel.StartTime
                        select new OEE_AbnormalDFCode
                        {
                            Plant_Organization_Name = MissingList.OEE_MachineInfo.System_Organization.Organization_Name,
                            BG_Organization_Name = MissingList.OEE_MachineInfo.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = MissingList.OEE_MachineInfo.System_Organization2.Organization_Name,
                            ProjectName = MissingList.OEE_MachineInfo.System_Project.Project_Name,
                            LineName = MissingList.OEE_MachineInfo.GL_Line.LineName,
                            StationName = MissingList.OEE_MachineInfo.GL_Station.StationName,
                            MachineName = MissingList.OEE_MachineInfo.MachineNo,
                            Machine_UID= MissingList.OEE_MachineInfo_UID,
                            ShiftName = MissingList.GL_ShiftTime.Shift,
                            DownTimeCode = MissingList.DownTimeCode,
                            ProductDate = MissingList.Product_Date,
                            CreateTime = MissingList.Create_Date,
                            ShiftTimeID = MissingList.ShiftTimeID,
                        };

            if (serchModel.ShiftTimeID != -1)
            {
                query = query.Where(p => p.ShiftTimeID == serchModel.ShiftTimeID);
            }

            return query.ToList();
        }
    }

}
