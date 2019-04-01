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
    public interface INewMesDataSyncService
    {
        /// <summary>
        /// 添加MES制程信息
        /// </summary>
        /// <returns></returns>
        bool AddMesDataInfo(List<MES_StationDataRecordDTO> MesProcessInfo);

        //通过日期时段获取MES信息
        List<MES_StationDataRecordDTO> GetMesDataInfoByDate(string date, string timeInterval);

        string SynchronizeMesInfo(MesSyncParam syncParam);
    }

    public class NewMesDataSyncService : INewMesDataSyncService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEnumerationRepository EnumerationRepository;
        private readonly INewMesDataSyncReposity NewMesDataSyncReposity;
        private readonly IProductInputRepository ProductInputRepository;

        public NewMesDataSyncService(IUnitOfWork unitOfWork,
            IEnumerationRepository EnumerationRepository,
            INewMesDataSyncReposity NewMesDataSyncReposity,
            IProductInputRepository ProductInputRepository
            )
        {
            this.unitOfWork = unitOfWork;
            this.EnumerationRepository = EnumerationRepository;
            this.NewMesDataSyncReposity = NewMesDataSyncReposity;
            this.ProductInputRepository = ProductInputRepository;
        }

        public bool AddMesDataInfo(List<MES_StationDataRecordDTO> MesProcessInfo)
        {
            List<MES_StationDataRecord> dtoList = AutoMapper.Mapper.Map<List<MES_StationDataRecord>>(MesProcessInfo);
            NewMesDataSyncReposity.AddList(dtoList);
            unitOfWork.Commit();
            return true;
        }

        public List<MES_StationDataRecordDTO> GetMesDataInfoByDate(string date, string timeInterval)
        {
            var result = NewMesDataSyncReposity.GetAll();
            result = result.Where(p => p.Date.ToString() == date && p.TimeInterVal == timeInterval);
            List<MES_StationDataRecordDTO> dtoList = AutoMapper.Mapper.Map<List<MES_StationDataRecordDTO>>(result);
            return dtoList.OrderByDescending(p => p.EndTimeInterval).ToList();
        }

        public string SynchronizeMesInfo(MesSyncParam syncParam)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //通过日期时段
                    //1 获取Mes临时表数据
                    //2 匹配ProductInput表的数据
                    //3 更新字段
                    var mesList = NewMesDataSyncReposity.GteMesDataInfoByDate(syncParam.currentDate, syncParam.currentInterval);
                    var sumMesProcessList = mesList.GroupBy(p => p.PIS_ProcessID).ToDictionary(m => m.Key, n => n.ToList());
                    List<MES_StationDataRecord> MesModelList = new List<MES_StationDataRecord>();
                    foreach (var item in sumMesProcessList)
                    {
                        //MES_PISParamDTO model = new MES_PISParamDTO();
                        //model.PIS_ProcessID = item.Key;
                        //model.TimeInterVal = item.Value.FirstOrDefault().TimeInterVal;
                        //model.Date = item.Value.FirstOrDefault().Date;
                        //model.PIS_Pick_Number = item.Value.Where(p => p.ProcessType == "PIS_PICK").FirstOrDefault().ProductQuantity;
                        //model.PIS_NG_Number = item.Value.Where(p => p.ProcessType == "PIS_NG").FirstOrDefault().ProductQuantity;
                        //model.PIS_Rework_Number = item.Value.Where(p => p.ProcessType == "PIS_Rework").FirstOrDefault().ProductQuantity;
                        //MesModelList.Add(model);
                        //ProductInputRepository.updateMesSynsData(model);
                    }

                    return "同步成功";
                }
            }
            catch (Exception ex)
            {
                return "同步失败";
            }
        }
    }
}
