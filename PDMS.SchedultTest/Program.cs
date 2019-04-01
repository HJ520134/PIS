using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model;
using System.Configuration;
using PDMS.Common.Constants;
using PDMS.Schedule;
using PDMS.Model.EntityDTO;

namespace PDMS.SchedultTest
{
    class Program
    {
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();

        static void Main(string[] args)
        {
            var startTime = "2019-01-02T11:49:17.1696195+08:00";
            var curDate=System.Convert.ToDateTime(startTime);
            Console.WriteLine(curDate.ToString("yyyy-MM-dd HH:mm:sss"));
            Console.ReadKey();

            //BatchMailService batchMailService = new BatchMailService(
            //    new SystemEmailMRepository(_DatabaseFactory),
            //    new SystemScheduleRepository(_DatabaseFactory),
            //    new UnitOfWork(_DatabaseFactory)
            //    );


            //OEE_Service OeeService = new OEE_Service(
            //  new UnitOfWork(_DatabaseFactory),
            //  new OEE_MachineInfoRepository(_DatabaseFactory),
            //  new OEE_DownTimeCodeRepository(_DatabaseFactory),
            //    new OEE_UserStationRepository(_DatabaseFactory),
            //      new OEE_StationDefectCodeRepository(_DatabaseFactory),
            //        new GL_ShiftTimeRepository(_DatabaseFactory),
            //          new OEE_DefectCodeDailyNumDTORepository(_DatabaseFactory),
            //           new EnumerationRepository(_DatabaseFactory),
            //             new OEE_DefectCodeDailySumRepository(_DatabaseFactory),
            //            new OEE_EveryDayMachineRepository(_DatabaseFactory),
            //              new OEE_MachineDailyDownRecordRepository(_DatabaseFactory),
            //                new OEE_DownTypeRepository(_DatabaseFactory),
            //                  new EquipmentInfoRepository(_DatabaseFactory)
            //  );




            //var currentDate = Convert.ToDateTime("2017-10-10 00:00");
            //for (int i = 0; i < 48; i++)
            //{
            //    OEE_MachineInfoDTO model = new OEE_MachineInfoDTO();
            //    model.Plant_Organization_UID = 1;
            //    model.BG_Organization_UID = 3;
            //    model.Modify_Date = currentDate.AddHours(i);
            //    var resultModel = OeeService.GetTimeModel(model);
            //    Console.WriteLine("Num"+i+"    "+"  Date：" + currentDate.AddHours(i));
            //    System.Console.WriteLine("Date： " + resultModel.currentDate + "----->Shift： " + resultModel.currentShiftID + "---->Interval： " + resultModel.currentTimeInterval);
            //    Console.WriteLine(" ");
            //}

            //Console.ReadKey();

            QualityTraceService qService = new QualityTraceService
            (
               new EnumerationRepository(_DatabaseFactory),
            new QEboadSumRepository(_DatabaseFactory),
             new TopTenQeboardRepository(_DatabaseFactory),
             new QTrace_SumRepository(_DatabaseFactory),
            new QTrace_TopTen_SumRepository(_DatabaseFactory),
             new UnitOfWork(_DatabaseFactory)
                );

            //FixtureService fixtureService = new FixtureService(
            //    new SystemProjectRepository(_DatabaseFactory),
            //    new EnumerationRepository(_DatabaseFactory),
            //    new FixtureRepository(_DatabaseFactory),
            //    new DefectRepairRepository(_DatabaseFactory),
            //    new UnitOfWork(_DatabaseFactory),
            //    new WorkStationRepository(_DatabaseFactory),
            //    new Process_InfoRepository(_DatabaseFactory),
            //    new VendorInfoRepository(_DatabaseFactory),
            //    new SystemOrgRepository(_DatabaseFactory),
            //    new MaintenancePlanRepository(_DatabaseFactory),
            //    new FixtureMaintenanceProfileRepository(_DatabaseFactory),
            //    new FixtureUserWorkshopRepository(_DatabaseFactory),
            //    new RepairLocationRepository(_DatabaseFactory),
            //    new Production_LineRepository(_DatabaseFactory),
            //     new Fixture_MachineRepository(_DatabaseFactory),
            //     new Fixture_DefectCodeRepository(_DatabaseFactory),
            //     new Fixture_Maintenance_RecordRepository(_DatabaseFactory),
            //     new Fixture_RepairSolutionRepository(_DatabaseFactory),
            //     new SystemUserRepository(_DatabaseFactory),
            //     new WorkshopRepository(_DatabaseFactory),
            //     new FixtureRepairRepository(_DatabaseFactory),
            //     new Fixture_ResumeRepository(_DatabaseFactory),
            //     new DefectCode_GroupRepository(_DatabaseFactory),
            //     new FixtureDefectCode_SettingRepository(_DatabaseFactory),
            //     new Fixture_Totake_MRepository(_DatabaseFactory),
            //     new Fixture_Totake_DRepository(_DatabaseFactory),
            //     new Fixture_Repair_DRepository(_DatabaseFactory),
            //     new Fixture_Repair_D_DefectRepository(_DatabaseFactory),
            //     new Fixture_PartRepository(_DatabaseFactory)
            //);


            //batchMailService.ExecBatch();
            //batchMailService.ExecSendEmail();
            //var pdmsService = new PDMS_Service();
            //pdmsService.InitialTask();
            //pdmsService.(StructConstants.BatchModuleName.FMTDashboard_Module);

            //var TimerInterval = Convert.ToInt32(ConfigurationManager.AppSettings["Timer_Email"]);
            //batchMailService.ExecBatch();
            //batchMailService.ExecSendEmail();
            qService.getTraceData();

            //qService.getTraceSumData();

        }
    }
}
