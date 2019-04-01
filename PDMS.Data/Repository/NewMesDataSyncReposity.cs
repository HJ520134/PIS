using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{

    public interface INewMesDataSyncReposity : IRepository<MES_StationDataRecord>
    {
        bool AddMesDataInfo();

        List<MES_StationDataRecordDTO> GteMesDataInfoByDate(string date, string timeInterval);
    }

    public class NewMesDataSyncReposity : RepositoryBase<MES_StationDataRecord>, INewMesDataSyncReposity
    {
        public NewMesDataSyncReposity(IDatabaseFactory databaseFactory)
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
        public List<MES_StationDataRecordDTO> GteMesDataInfoByDate(string date, string timeInterval)
        {
            //var query = from MES in DataContext.MES_StationDataRecord
            //            select MES;

            //query = query.Where(p => p.Date == date && p.TimeInterVal == timeInterval);
            //query.GroupBy(p=>p.PIS_ProcessID)

                var sql = @"SELECT
                                MAX([MES_StationDataRecord_UID]) AS [MES_StationDataRecord_UID]
                              , MAX([Date]) AS [Date]
                              , MAX([TimeInterVal]) AS [TimeInterVal]
                              , MAX([StartTimeInterval]) AS [StartTimeInterval]
                              , MAX([EndTimeInterval]) AS [EndTimeInterval]
                              , MAX([PIS_ProcessID]) AS [PIS_ProcessID]
                              , MAX([PIS_ProcessName]) AS [PIS_ProcessName]
                              , MAX([MES_ProcessID]) AS [MES_ProcessID]
                              , MAX([MES_ProcessName]) AS [MES_ProcessName]
                              , Sum([ProductQuantity]) AS [ProductQuantity]
                              , MAX([ProjectName]) AS [ProjectName]
                              , MAX([ProcessType]) AS [ProcessType]
	                          FROM [MES_StationDataRecord]";

            var sqlParam=$"WHERE[Date] = N'{date}' and[TimeInterVal] = N'{timeInterval}' GROUP BY [ProcessType],PIS_ProcessID";
            var result=  DataContext.Database.SqlQuery<MES_StationDataRecordDTO>(sql+sqlParam).ToList();
            return result;
        }
    }
}
