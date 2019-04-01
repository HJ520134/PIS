using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PDMS.Service
{
    public interface IMesDataSyncService
    {
        /// <summary>
        /// 添加MES制程信息
        /// </summary>
        /// <returns></returns>
        bool AddListMesDataInfo(List<MES_StationDataRecordDTO> MesProcessInfo);

        //通过日期时段获取MES信息
        List<MES_StationDataRecordDTO> GetMesDataInfoByDate(string date, string timeInterval);

        bool AddMesData(MES_StationDataRecord MesInfo);

        string SynchronizeMesInfo(MesSyncParam syncParam);
        PagedListModel<MES_StationDataRecordDTO> GetMESSyncReport(MES_StationDataRecordDTO MesInfo, Page page);

        /// <summary>
        /// 导出两小时的所有的数据
        /// </summary>
        /// <param name="MesInfo"></param>
        /// <returns></returns>
        List<MES_StationDataRecordDTO> ExportAllTwoHourReport(MES_StationDataRecordDTO MesInfo);
    }

    public class MesDataSyncService : IMesDataSyncService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEnumerationRepository EnumerationRepository;
        private readonly IMesDataSyncRepository MesDataSyncRepository;
        private readonly IProductInputRepository ProductInputRepository;
        private readonly IProcessIDTRSConfigRepository ProcessIDTRSConfigRepository;

        public MesDataSyncService(IUnitOfWork unitOfWork,
            IEnumerationRepository EnumerationRepository,
            IMesDataSyncRepository MesDataSyncRepository,
            IProductInputRepository ProductInputRepository,
            IProcessIDTRSConfigRepository ProcessIDTRSConfigRepository
            )
        {
            this.unitOfWork = unitOfWork;
            this.EnumerationRepository = EnumerationRepository;
            this.MesDataSyncRepository = MesDataSyncRepository;
            this.ProductInputRepository = ProductInputRepository;
            this.ProcessIDTRSConfigRepository = ProcessIDTRSConfigRepository;
        }
        public bool AddListMesDataInfo(List<MES_StationDataRecordDTO> MesProcessInfo)
        {
            List<MES_StationDataRecord> dtoList = AutoMapper.Mapper.Map<List<MES_StationDataRecord>>(MesProcessInfo);
            MesDataSyncRepository.AddList(dtoList);
            unitOfWork.Commit();
            return true;
        }

        public bool AddMesData(MES_StationDataRecord MesInfo)
        {
            try
            {
                //插入数据前判断是否重复--
                //var result = MesDataSyncRepository.GetAll().Where(p => p.Date == MesInfo.Date && p.TimeInterVal == MesInfo.TimeInterVal && p.StartTimeInterval == MesInfo.StartTimeInterval && p.EndTimeInterval == MesInfo.EndTimeInterval && p.PIS_ProcessID == MesInfo.PIS_ProcessID);
                var result = MesDataSyncRepository.Is_MES_Exist(MesInfo);
                //更新
                if (result)
                {
                    //result.FirstOrDefault().ProductQuantity = MesInfo.ProductQuantity;
                    //MesDataSyncRepository.Update(MesInfo);
                    return MesDataSyncRepository.UpData_MESByID(MesInfo);
                    //unitOfWork.Commit();
                }
                else//增加
                {
                    //MES_StationDataRecord dto = AutoMapper.Mapper.Map<MES_StationDataRecord>(MesInfo);
                    MesDataSyncRepository.Add(MesInfo);
                    unitOfWork.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public List<MES_StationDataRecordDTO> GetMesDataInfoByDate(string date, string timeInterval)
        {
            var result = MesDataSyncRepository.GetAll();
            result = result.Where(p => p.Date.ToString() == date && p.TimeInterVal == timeInterval);
            List<MES_StationDataRecordDTO> dtoList = AutoMapper.Mapper.Map<List<MES_StationDataRecordDTO>>(result);
            return dtoList.OrderByDescending(p => p.EndTimeInterval).ToList();
        }

        public MES_StationDataRecord GetLastSyncDate(string ProjectID)
        {
            var result = MesDataSyncRepository.GetLastTimeInterval(ProjectID);
            if (result != null)
            {
                //MES_StationDataRecord dto = AutoMapper.Mapper.Map<MES_StationDataRecord>(result);
                return result;
            }
            else
            {
                return null;
            }
        }

        public string SynchronizeMesInfo(MesSyncParam syncParam)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ////通过日期时段
                    ////1 获取Mes临时表数据
                    ////2 匹配ProductInput表的数据
                    ////3 更新字段
                    //var mesList = MesDataSyncRepository.GteMesDataInfoByDate(syncParam.currentDate, syncParam.currentInterval);
                    //var sumMesProcessList = mesList.GroupBy(p => p.PIS_ProcessID).ToDictionary(m => m.Key, n => n.ToList());
                    //List<MES_StationDataRecord> MesModelList = new List<MES_StationDataRecord>();
                    //foreach (var item in sumMesProcessList)
                    //{
                    //    MES_StationDataRecord model = new MES_StationDataRecord();
                    //    model.PIS_ProcessID = item.Key;
                    //    model.TimeInterVal = item.Value.FirstOrDefault().TimeInterVal;
                    //    model.Date = item.Value.FirstOrDefault().Date;
                    //    model.ProductQuantity = item.Value.Sum(p => p.ProductQuantity);
                    //    MesModelList.Add(model);
                    //}

                    //ProductInputRepository.updateMesSynsData(MesModelList);
                    return "同步成功";
                }
            }
            catch (Exception ex)
            {
                return "同步失败";
            }
        }

        public PagedListModel<MES_StationDataRecordDTO> GetMESSyncReport(MES_StationDataRecordDTO MesInfo, Page page)
        {
            List<MES_StationDataRecordDTO> mesResultList = new List<MES_StationDataRecordDTO>();
            var result = MesDataSyncRepository.GetMESSyncReport(MesInfo);
            var sumdataList = result.GroupBy(p => new { p.PIS_ProcessID, p.MES_ProcessID, p.TimeInterVal });
            var mesAllConfig = ProcessIDTRSConfigRepository.GetAll().ToList();
            foreach (var item in sumdataList)
            {
                MES_StationDataRecordDTO model = new MES_StationDataRecordDTO();
                model.PIS_ProcessID = item.Key.PIS_ProcessID;
                model.PIS_ProcessName = item.FirstOrDefault().PIS_ProcessName;
                model.MES_ProcessID = item.Key.MES_ProcessID;
                model.MES_ProcessName = item.FirstOrDefault().MES_ProcessName;
                model.ProcessType = item.FirstOrDefault().ProcessType;
                model.ProductQuantity = item.Sum(p => p.ProductQuantity);
                model.TimeInterVal = item.Key.TimeInterVal;
                var colorModel = mesAllConfig.Where(p => p.PIS_ProcessID == item.Key.PIS_ProcessID).FirstOrDefault();
                if (colorModel != null)
                {
                    model.Color = colorModel.Color;
                    mesResultList.Add(model);
                }
            }

            mesResultList = mesResultList.OrderBy(p => p.PIS_ProcessID).ThenBy(m => m.TimeInterVal).ToList();
            var totalCount = mesResultList.Count();
            var resultList = mesResultList.Skip(page.PageSize * page.PageNumber).Take(page.PageSize).ToList();
            return new PagedListModel<MES_StationDataRecordDTO>(totalCount, resultList);
        }

        /// <summary>
        /// 导出两小时的所有的数据
        /// </summary>
        /// <param name="MesInfo"></param>
        /// <returns></returns>
        public List<MES_StationDataRecordDTO> ExportAllTwoHourReport(MES_StationDataRecordDTO MesInfo)
        {
            List<MES_StationDataRecordDTO> mesResultList = new List<MES_StationDataRecordDTO>();
            var result = MesDataSyncRepository.GetMESSyncReport(MesInfo);
            var sumdataList = result.GroupBy(p => new { p.PIS_ProcessID, p.MES_ProcessID, p.TimeInterVal });
            var mesAllConfig = ProcessIDTRSConfigRepository.GetAll().ToList();
            foreach (var item in sumdataList)
            {
                MES_StationDataRecordDTO model = new MES_StationDataRecordDTO();
                model.PIS_ProcessID = item.Key.PIS_ProcessID;
                model.PIS_ProcessName = item.FirstOrDefault().PIS_ProcessName;
                model.MES_ProcessID = item.Key.MES_ProcessID;
                model.MES_ProcessName = item.FirstOrDefault().MES_ProcessName;
                model.ProcessType = item.FirstOrDefault().ProcessType;
                model.ProductQuantity = item.Sum(p => p.ProductQuantity);
                model.TimeInterVal = item.Key.TimeInterVal;
                var colorModel = mesAllConfig.Where(p => p.PIS_ProcessID == item.Key.PIS_ProcessID).FirstOrDefault();
                if (colorModel != null)
                {
                    model.Color = colorModel.Color;
                    mesResultList.Add(model);
                }
            }

            mesResultList = mesResultList.OrderBy(p => p.PIS_ProcessID).ThenBy(m => m.TimeInterVal).ToList();

            return mesResultList;
        }
    }
}
