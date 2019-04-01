using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data;

namespace PDMS.Service
{

    public interface IMES_PIS_SyncFailedRecordService
    {
        //PagedListModel<MES_PIS_SyncFailedRecordDTO> QuerySyncFailedRecord(MES_PIS_SyncFailedRecordDTO searchModel, Page page);
        PagedListModel<MES_PIS_SyncFailedRecordDTO> QuerySyncFailedRecord(MES_PIS_SyncFailedRecordDTO paramModel, Page page);
        bool AddMESSyncFailedLog(MES_PIS_SyncFailedRecord logModel);

        //bool updateSyncFailedLog(MES_PIS_SyncFailedRecord logModel);

        MES_PIS_SyncFailedRecordDTO GetSyncFailedRecordByRequest(int uid);

        string updateSyncFailedLog(MES_PIS_SyncFailedRecordDTO logModel);
    }

    public class MES_PIS_SyncFailedRecordService : IMES_PIS_SyncFailedRecordService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMES_PIS_SyncFailedRecordRepository mES_PIS_SyncFailedRecordRepository;
        public MES_PIS_SyncFailedRecordService(
              IMES_PIS_SyncFailedRecordRepository mES_PIS_SyncFailedRecordRepository,
              IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.mES_PIS_SyncFailedRecordRepository = mES_PIS_SyncFailedRecordRepository;
        }

        public PagedListModel<MES_PIS_SyncFailedRecordDTO> QuerySyncFailedRecord(MES_PIS_SyncFailedRecordDTO paramModel, Page page)
        {
            var totalCount = 0;
            var resultList = mES_PIS_SyncFailedRecordRepository.QuerySyncFailedRecord(paramModel, page, out totalCount);
            totalCount = resultList.Count();
            return new PagedListModel<MES_PIS_SyncFailedRecordDTO>(totalCount, resultList);
        }

        public bool AddMESSyncFailedLog(MES_PIS_SyncFailedRecord logModel)
        {
            try
            {
                var result = mES_PIS_SyncFailedRecordRepository.Add(logModel);
                unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public bool updateSyncFailedLog(MES_PIS_SyncFailedRecord logModel)
        //{
        //    var isExist = MES_PIS_SyncFailedRecordReposity.GetMany(p => p.SyncType == logModel.SyncType && p.SyncName == logModel.SyncName && p.SyncRequest == logModel.SyncRequest).FirstOrDefault();
        //    isExist.SyncTime = logModel.SyncTime;
        //    isExist.OperateTime = logModel.OperateTime;
        //    isExist.Is_ManuallySuccess = logModel.Is_ManuallySuccess;
        //    if (logModel.Is_ManuallySuccess)
        //    {
        //    }
        //    else
        //    {
        //        isExist.FailedNumber += 1;
        //    }
        //    unitOfWork.Commit();
        //    return true;
        //}

        public string updateSyncFailedLog(MES_PIS_SyncFailedRecordDTO logModel)
        {
            try
            {
                var mES_PIS_SyncFailedRecord = mES_PIS_SyncFailedRecordRepository.GetById(logModel.MES_PIS_SyncFailedRecord_UID);
                mES_PIS_SyncFailedRecord.Is_ManuallySuccess = logModel.Is_ManuallySuccess;
                if (logModel.Is_ManuallySuccess)
                {
                }
                else
                {
                    mES_PIS_SyncFailedRecord.FailedNumber += 1;
                }
                mES_PIS_SyncFailedRecord.OperateID = logModel.OperateID;
                mES_PIS_SyncFailedRecordRepository.Update(mES_PIS_SyncFailedRecord);
                unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        //通过请求参数获取失败记录信息
        public MES_PIS_SyncFailedRecordDTO GetSyncFailedRecordByRequest(int uid)
        {
            var result = mES_PIS_SyncFailedRecordRepository.GetMany(p => p.MES_PIS_SyncFailedRecord_UID == uid && p.Is_ManuallySuccess == false).FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            MES_PIS_SyncFailedRecordDTO model = new MES_PIS_SyncFailedRecordDTO()
            {
                MES_PIS_SyncFailedRecord_UID = result.MES_PIS_SyncFailedRecord_UID,
                SyncType = result.SyncType,
                SyncName = result.SyncName,
                SyncTime = result.SyncTime,
                SyncRequest = result.SyncRequest,
                SyncResult = result.SyncResult,
                Is_ManuallySuccess = result.Is_ManuallySuccess,
                FailedNumber = result.FailedNumber,
                OperateID = result.OperateID,
                OperateTime = result.OperateTime
            };
            return model;
        }
    }
}
