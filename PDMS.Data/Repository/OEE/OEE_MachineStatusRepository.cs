using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IOEE_MachineStatusRepository : IRepository<OEE_MachineStatus>
    {
        List<OEE_MachineStatus> GetAllStationMachineStatusList(OEE_ReprortSearchModel serchModel);

    }

    public class OEE_MachineStatusRepository : RepositoryBase<OEE_MachineStatus>, IOEE_MachineStatusRepository
    {
        public OEE_MachineStatusRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }


        /// <summary>
        /// 获取该工站下面所有工站的数据
        /// </summary>
        /// <returns></returns>
        public List<OEE_MachineStatus> GetAllStationMachineStatusList(OEE_ReprortSearchModel serchModel)
        {
            var query = from machienStatus in DataContext.OEE_MachineStatus
                                select new OEE_MachineStatusDTO
                                {
                                    OEE_MachineStatus_UID = machienStatus.OEE_MachineInfo_UID,
                                    OEE_MachineInfo_UID = machienStatus.OEE_MachineInfo_UID,
                                    Product_Date = machienStatus.Product_Date,
                                    ShiftTimeID = machienStatus.ShiftTimeID,
                                    StatusID = machienStatus.StatusID,
                                    StatusDuration = machienStatus.StatusDuration,
                                    UpdateTime = machienStatus.UpdateTime,
                                };

            return null;
        }
    }
}
