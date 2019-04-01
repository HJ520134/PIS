using PDMS.Common.Enums;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SyncWuxiOEEData
{
    class Program
    {
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();
        static void Main(string[] args)
        {
            OEE_Sync();
        }

        /// <summary>
        /// 同步OEE班次产能
        /// </summary>
        public static void OEE_Sync()
        {


            OEE_SyncService OEE_SyncService = new OEE_SyncService
              (
                  new OEE_EveryDayMachineRepository(_DatabaseFactory),
                  new GL_WIPHourOutputRepository(_DatabaseFactory),
                  new OEE_MachineDailyDownRecordRepository(_DatabaseFactory),
                  new OEE_DefectCodeDailyNumDTORepository(_DatabaseFactory),
                  new OEE_EveryDayDFcodeMissingRepository(_DatabaseFactory),
                  new OEE_EveryDayMachineDTCodeRepository(_DatabaseFactory),
                  new OEE_MachineStatusRepository(_DatabaseFactory),
                  new UnitOfWork(_DatabaseFactory)
             );
           OEE_SyncService.ExcuteOEE_Output(DateTime.Now);
            //for (int j = 0; j < 5; j++)
            //{
            //    for (int i = 0; i < 20; i = i + 15)
            //    {

            //        OEE_SyncService.ExcuteOEE_Output(DateTime.Now.AddDays(-j).AddHours(i));
            //    }
            //}
        }
    
    }
}
