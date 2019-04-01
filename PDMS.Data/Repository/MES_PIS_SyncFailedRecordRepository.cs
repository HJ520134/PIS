using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;

namespace PDMS.Data.Repository
{
    public interface IMES_PIS_SyncFailedRecordRepository : IRepository<MES_PIS_SyncFailedRecord>
    {
        IQueryable<MES_PIS_SyncFailedRecordDTO> QuerySyncFailedRecord(MES_PIS_SyncFailedRecordDTO searchModel, Page page, out int totalcount);
    }

    public class MES_PIS_SyncFailedRecordRepository : RepositoryBase<MES_PIS_SyncFailedRecord>, IMES_PIS_SyncFailedRecordRepository
    {
        public MES_PIS_SyncFailedRecordRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public IQueryable<MES_PIS_SyncFailedRecordDTO> QuerySyncFailedRecord(MES_PIS_SyncFailedRecordDTO searchModel, Page page, out int totalcount)
        {
            var query = from SyncFailed in DataContext.MES_PIS_SyncFailedRecord
                        select new MES_PIS_SyncFailedRecordDTO
                        {
                            MES_PIS_SyncFailedRecord_UID = SyncFailed.MES_PIS_SyncFailedRecord_UID,
                            SyncType = SyncFailed.SyncType,
                            SyncName = SyncFailed.SyncName,
                            SyncTime = SyncFailed.SyncTime,
                            SyncRequest = SyncFailed.SyncRequest,
                            SyncResult = SyncFailed.SyncResult,
                            FailedNumber = SyncFailed.FailedNumber,
                            OperateID = SyncFailed.MES_PIS_SyncFailedRecord_UID,
                            OperateTime = SyncFailed.OperateTime
                        };
            query = query.Where(p => p.Is_ManuallySuccess == false);
            totalcount = query.Count();
            return query.GetPage(page);
        }
    }

}
