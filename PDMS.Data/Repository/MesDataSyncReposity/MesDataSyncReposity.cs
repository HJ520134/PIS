using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{

    public interface IMesDataSyncRepository : IRepository<MES_StationDataRecord>
    {
        bool AddMesDataInfo();

        List<MES_StationDataRecord> GteMesDataInfoByDate(string date, string timeInterval);


        MES_StationDataRecord GetLastTimeInterval(string mesProjectName);

        /// <summary>
        /// 判断该数据是否已经被存在
        /// </summary>
        /// <returns></returns>
        bool Is_MES_Exist(MES_StationDataRecord param);

        bool UpData_MESByID(MES_StationDataRecord param);

        List<MES_StationDataRecordDTO> GetMESSyncReport(MES_StationDataRecordDTO MesInfo);
    }

    public class MesDataSyncRepository : RepositoryBase<MES_StationDataRecord>, IMesDataSyncRepository
    {
        public MesDataSyncRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        { }

        public bool AddMesDataInfo()
        {
            return false;
        }

        /// <summary>
        ///获取当前时段的数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeInterval"></param>
        /// <returns></returns>
        public List<MES_StationDataRecord> GteMesDataInfoByDate(string date, string timeInterval)
        {
            var query = from MES in DataContext.MES_StationDataRecord
                        select MES;
            query = query.Where(p => p.Date == date && p.TimeInterVal == timeInterval);

            return query.ToList();
        }

        /// <summary>
        ///获取本专案的下面的最后一条数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeInterval"></param>
        /// <returns></returns>
        public MES_StationDataRecord GetLastTimeInterval(string mesProjectName)
        {
            var query = from MES in DataContext.MES_StationDataRecord
                        select MES;

            query = query.Where(p => p.ProjectName == mesProjectName);
            return query.OrderByDescending(p => p.EndTimeInterval).FirstOrDefault();
        }

        /// <summary>
        /// 判断该数据是否已经存在
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool Is_MES_Exist(MES_StationDataRecord MesInfo)
        {
            var query = from MES in DataContext.MES_StationDataRecord
                        select MES;
            var result = query.Where(p => p.Date == MesInfo.Date && p.TimeInterVal == MesInfo.TimeInterVal && p.StartTimeInterval == MesInfo.StartTimeInterval && p.EndTimeInterval == MesInfo.EndTimeInterval && p.PIS_ProcessID == MesInfo.PIS_ProcessID && p.MES_ProcessID == MesInfo.MES_ProcessID);

            if (result.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 更新MES数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool UpData_MESByID(MES_StationDataRecord param)
        {
            var sql = $" UPDATE [MES_StationDataRecord] SET  [ProductQuantity]={param.ProductQuantity} WHERE  [Date]=N'{param.Date}' AND  [TimeInterVal]=N'{param.TimeInterVal}' AND [StartTimeInterval]=N'{param.StartTimeInterval}' AND  [EndTimeInterval]=N'{param.EndTimeInterval}' And [PIS_ProcessID]={param.PIS_ProcessID} AND [MES_ProcessName]=N'{param.MES_ProcessName}'";
            var result = DataContext.Database.ExecuteSqlCommand(sql);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<MES_StationDataRecordDTO> GetMESSyncReport(MES_StationDataRecordDTO MesInfo)
        {
            //日期是必选
            var query = from MES in DataContext.MES_StationDataRecord
                        //join pisConfig in DataContext.ProcessIDTransformConfig on MES.PIS_ProcessID equals pisConfig.PIS_ProcessID
                        where MES.Date == MesInfo.Date
                        select new MES_StationDataRecordDTO
                        {
                            MES_StationDataRecord_UID = MES.MES_StationDataRecord_UID,
                            Date = MES.Date,
                            TimeInterVal = MES.TimeInterVal,
                            StartTimeInterval = MES.StartTimeInterval,
                            EndTimeInterval = MES.EndTimeInterval,
                            PIS_ProcessID = MES.PIS_ProcessID,
                            PIS_ProcessName = MES.PIS_ProcessName,
                            MES_ProcessID = MES.MES_ProcessID,
                            MES_ProcessName = MES.MES_ProcessName,
                            ProjectName = MES.ProjectName,
                            ProcessType = MES.ProcessType,
                            ProductQuantity = MES.ProductQuantity,
                            //Color = pisConfig.Color
                        };

            //时段
            if (!string.IsNullOrEmpty(MesInfo.TimeInterVal))
            {
                query = query.Where(p => p.TimeInterVal == MesInfo.TimeInterVal);
            }

            //PIS制程ID
            if (MesInfo.PIS_ProcessID != 0)
            {
                query = query.Where(p => p.PIS_ProcessID == MesInfo.PIS_ProcessID);
            }

            //PIS制程名称
            if (!string.IsNullOrEmpty(MesInfo.PIS_ProcessName))
            {
                query = query.Where(p => p.PIS_ProcessName == MesInfo.PIS_ProcessName);
            }

            //制程类型
            if (!string.IsNullOrEmpty(MesInfo.ProcessType))
            {
                query = query.Where(p => p.ProcessType == MesInfo.ProcessType);
            }

            //Mes制程名称
            if (!string.IsNullOrEmpty(MesInfo.MES_ProcessName))
            {
                query = query.Where(p => p.MES_ProcessName == MesInfo.MES_ProcessName);
            }

            return query.ToList();
        }

    }
}
