using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;

namespace PDMS.Data.Repository
{
    public interface IOEE_DefectCodeDailySumRepository : IRepository<OEE_DefectCodeDailySum>
    {
        List<OEE_DefectCodeDailySumDTO> GetDefectCodeDailySum(OEE_ReprortSearchModel serchModel);
    }

    public class OEE_DefectCodeDailySumRepository : RepositoryBase<OEE_DefectCodeDailySum>, IOEE_DefectCodeDailySumRepository
    {
        public OEE_DefectCodeDailySumRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public List<OEE_DefectCodeDailySumDTO> GetDefectCodeDailySum(OEE_ReprortSearchModel serchModel)
        {
            var startProductDate = Convert.ToDateTime(serchModel.StartTime.ToShortDateString());
            var endProductDate = Convert.ToDateTime(serchModel.EndTime.ToShortDateString());
            var query = from dayOutput in DataContext.OEE_DefectCodeDailySum
                        where dayOutput.Plant_Organization_UID == serchModel.Plant_Organization_UID
                        && dayOutput.BG_Organization_UID == serchModel.BG_Organization_UID
                        && dayOutput.OEE_MachineInfo_UID == serchModel.EQP_Uid
                        && dayOutput.OEE_StationDefectCode.Is_Enable == true
                        select new OEE_DefectCodeDailySumDTO
                        {
                            OEE_DefectCodeDailySum_UID = dayOutput.OEE_DefectCodeDailySum_UID,
                            Plant_Organization_UID = dayOutput.Plant_Organization_UID,
                            BG_Organization_UID = dayOutput.BG_Organization_UID,
                            FunPlant_Organization_UID = dayOutput.FunPlant_Organization_UID,
                            OEE_MachineInfo_UID = dayOutput.OEE_MachineInfo_UID,
                            OEE_StationDefectCode_UID = dayOutput.OEE_StationDefectCode_UID,
                            DefectNum = dayOutput.DefectNum,
                            ProductDate = dayOutput.ProductDate,
                            ShiftTimeID = dayOutput.ShiftTimeID,
                            DefectName = dayOutput.OEE_StationDefectCode.DefectEnglishName,
                            DefectChineseName= dayOutput.OEE_StationDefectCode.DefecChinesetName
                        };

            query = query.Where(p => p.ProductDate >= startProductDate && p.ProductDate <= endProductDate);

            if (serchModel.ShiftTimeID == -1)
            {
                //query = query.Where(p => p.ProductDate == serchModel.StartTime);
            }
            else
            {
                query = query.Where(p => p.ShiftTimeID == serchModel.ShiftTimeID);
            }

            return query.ToList();
        }
    }
}
