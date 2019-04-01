using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Service;
using PDMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using PDMS.Common.Common;

namespace PDMS.Schedule
{
    public partial class PDMS_Service : ServiceBase
    {
        public bool IsProcess_Email;
        public bool IsProcess_Fixture_Notification;

        // new DatabaseFactory
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();

        FixtureService fixtureService;
        BatchMailService batchMailService;


        public PDMS_Service()
        {
            InitializeComponent();
            InitialService();
        }

        private void InitialService()
        {
            this.batchMailService = new BatchMailService(new SystemEmailMRepository(_DatabaseFactory),
                new SystemScheduleRepository(_DatabaseFactory),
                new UnitOfWork(_DatabaseFactory));

            this.fixtureService = new FixtureService(
                new SystemProjectRepository(_DatabaseFactory),
                new EnumerationRepository(_DatabaseFactory),
                new FixtureRepository(_DatabaseFactory),
                new DefectRepairRepository(_DatabaseFactory),
                new UnitOfWork(_DatabaseFactory),
                new WorkStationRepository(_DatabaseFactory),
                new Process_InfoRepository(_DatabaseFactory),
                new VendorInfoRepository(_DatabaseFactory),
                new SystemOrgRepository(_DatabaseFactory),
                new MaintenancePlanRepository(_DatabaseFactory),
                new FixtureMaintenanceProfileRepository(_DatabaseFactory),
                new FixtureUserWorkshopRepository(_DatabaseFactory),
                new RepairLocationRepository(_DatabaseFactory),
                new Production_LineRepository(_DatabaseFactory),
                 new Fixture_MachineRepository(_DatabaseFactory),
                 new Fixture_DefectCodeRepository(_DatabaseFactory),
                 new Fixture_Maintenance_RecordRepository(_DatabaseFactory),
                 new Fixture_RepairSolutionRepository(_DatabaseFactory),
                 new SystemUserRepository(_DatabaseFactory),
                 new WorkshopRepository(_DatabaseFactory),
                 new FixtureRepairRepository(_DatabaseFactory),
                 new Fixture_ResumeRepository(_DatabaseFactory),
                 new DefectCode_GroupRepository(_DatabaseFactory),
                 new FixtureDefectCode_SettingRepository(_DatabaseFactory),
                 new Fixture_Totake_MRepository(_DatabaseFactory),
                 new Fixture_Totake_DRepository(_DatabaseFactory),
                 new Fixture_Repair_DRepository(_DatabaseFactory),
                 new Fixture_Repair_D_DefectRepository(_DatabaseFactory),
                 new Fixture_PartRepository(_DatabaseFactory),
                 new EquipmentInfoRepository(_DatabaseFactory),
                 new Fixture_Return_MRepository(_DatabaseFactory)
                );

        }

        protected override void OnStart(string[] args)
        {
            InitialTask();
        }

        protected override void OnStop()
        {
        }

        /// <summary>
        /// 多线程任务
        /// </summary>
        public void InitialTask()
        {
            Task.Factory.StartNew(() =>
            {
                RunTask(StructConstants.BatchModuleName.Email_Module);
            });

            Task.Factory.StartNew(() =>
            {
                RunTask(StructConstants.BatchModuleName.Fixture_Module);
            });
        }

        private void RunTask(string taskname)
        {
            switch (taskname)
            {
                case StructConstants.BatchModuleName.Email_Module:
                    var TimerInterval = Convert.ToInt32(ConfigurationManager.AppSettings["Timer_Email"]);

                    Timer Timer_Email = new Timer();
                    Timer_Email.Elapsed += new ElapsedEventHandler(Timer_Elapsed_Batch_Email);
                    Timer_Email.Interval = TimerInterval * 60 * 1000; //间隔1小时判断一次
                    //Timer_Email.Interval =  20 * 1000; //间隔1小时判断一次

                    Timer_Email.Start();
                    break;


                case StructConstants.BatchModuleName.Fixture_Module:
                    var Timer_BatchSetOne = Convert.ToInt32(ConfigurationManager.AppSettings["Timer_BatchSetOne"]);

                    Timer Timer_FMT = new Timer();
                    Timer_FMT.Elapsed += new ElapsedEventHandler(Timer_Elapsed_FMT_Notification);
                    Timer_FMT.Interval = Timer_BatchSetOne *  60 * 1000; //间隔1分钟判断一次
                    //Timer_FMT.Interval = 20 * 1000; //间隔1分钟判断一次

                    Timer_FMT.Start();
                    break;
            }
        }

        private void Timer_Elapsed_Batch_Email(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!IsProcess_Email)
                {
                    IsProcess_Email = true;
                    var TimerInterval = Convert.ToInt32(ConfigurationManager.AppSettings["Timer_Email"]);
                    batchMailService.ExecSendEmail();
                    IsProcess_Email = false;
                }
            }
            catch (Exception)
            {
                IsProcess_Email = false;
                throw;
            }
        }

        private void Timer_Elapsed_FMT_Notification(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!IsProcess_Fixture_Notification)
                {
                    IsProcess_Fixture_Notification = true;
                    //fixtureService.ExecFMTDashboard(); 
                    batchMailService.ExecBatch();
                    IsProcess_Fixture_Notification = false;
                }

            }
            catch (Exception)
            {
                IsProcess_Fixture_Notification = false;
                throw;

            }
        }
    }
}
