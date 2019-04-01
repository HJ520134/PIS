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
    public interface IOEE_EveryDayDFcodeRepository : IRepository<OEE_EveryDayMachineDTCode>
    {
        List<AbnormalDFCode> GetAbnormalDFCodeList(OEE_ReprortSearchModel serchModel);
    }

    public class OEE_EveryDayDFcodeRepository : RepositoryBase<OEE_EveryDayMachineDTCode>, IOEE_EveryDayDFcodeRepository
    {
        public OEE_EveryDayDFcodeRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<AbnormalDFCode> GetAbnormalDFCodeList(OEE_ReprortSearchModel serchModel)
        {
            var query = from MissingList in DataContext.OEE_EveryDayMachineDTCode
                        where MissingList.OEE_MachineInfo.StationID == serchModel.StationID
                        && MissingList.Product_Date == serchModel.StartTime
                        select new AbnormalDFCode
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
                            DFCode = MissingList.DFCode,
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
