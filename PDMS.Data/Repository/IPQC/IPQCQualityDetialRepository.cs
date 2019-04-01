using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IIPQCQualityDetialRepository : IRepository<IPQCQualityDetial>
    {
        List<IPQCQualityDetialDto> GetIPQCDetialReport(IPQCQualityReportVM serchModel);
        List<GL_IPQCQualityDetialDTO> GetAllGL_IPQCQualityDetialDTO(int StationID, int ShiftID, int TimeIntervalIndex, DateTime ProductDate, string TimeInterval);
    }

    public class IPQCQualityDetialRepository : RepositoryBase<IPQCQualityDetial>, IIPQCQualityDetialRepository
    {
        public IPQCQualityDetialRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public List<IPQCQualityDetialDto> GetIPQCDetialReport(IPQCQualityReportVM serchModel)
        {
            var query = from ipqc in DataContext.IPQCQualityDetial
                        where
                         ipqc.GL_Station.StationID == serchModel.StationID &&
                         ipqc.ProductDate == serchModel.ProductDate &&
                          ipqc.TimeInterval == serchModel.TimeInterval &&
                         ipqc.GL_Station.IsTest == true
                        select new IPQCQualityDetialDto
                        {
                            IPQCQualityDetial_UID = ipqc.IPQCQualityDetial_UID
                             ,
                            StationID = ipqc.StationID
                             ,
                            ShiftID = ipqc.ShiftID
                             ,
                            ProductDate = ipqc.ProductDate
                             ,
                            TimeInterval = ipqc.TimeInterval
                             ,
                            TimeIntervalIndex = ipqc.TimeIntervalIndex
                             ,
                            NGName = ipqc.NGName
                             ,
                            NGNumber = ipqc.NGNumber
                             ,
                            NGType = ipqc.NGType
                             ,
                            ModifyTime = ipqc.ModifyTime
                        };

            return query.ToList();
        }



        public List<GL_IPQCQualityDetialDTO> GetAllGL_IPQCQualityDetialDTO(int StationID, int ShiftID, int TimeIntervalIndex, DateTime ProductDate, string TimeInterval)
        {

            var query = from ipqc in DataContext.IPQCQualityDetial
                        where
                         ipqc.GL_Station.StationID == StationID &&
                         ipqc.ProductDate == ProductDate &&
                         ipqc.TimeInterval ==TimeInterval &&
                         ipqc.ShiftID == ShiftID &&
                         ipqc.TimeIntervalIndex == TimeIntervalIndex
                        select new GL_IPQCQualityDetialDTO
                        {
                            IPQCQualityDetial_UID = ipqc.IPQCQualityDetial_UID,
                            StationID = ipqc.StationID,
                            ShiftID = ipqc.ShiftID,
                            ProductDate = ipqc.ProductDate,
                            TimeInterval = ipqc.TimeInterval ,
                            TimeIntervalIndex = ipqc.TimeIntervalIndex  ,
                            NGName = ipqc.NGName,
                            NGNumber = ipqc.NGNumber,
                            NGType = ipqc.NGType ,
                            ModifyTime = ipqc.ModifyTime
                        };

            return query.ToList();


        }
    }
}
