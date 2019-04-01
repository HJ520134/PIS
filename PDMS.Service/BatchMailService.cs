using PDMS.Common.Common;
using PDMS.Common.Constants;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model.ViewModels.Batch;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IBatchMailService
    {
        void ExecSendEmail();
    }

    public class BatchMailService : IBatchMailService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ISystemEmailMRepository systemEmailMRepository;
        private readonly ISystemScheduleRepository systemScheduleRepository;

        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();

        FixtureService fixtureService = new FixtureService(
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

        public BatchMailService(ISystemEmailMRepository systemEmailMRepository,
            ISystemScheduleRepository systemScheduleRepository,
            IUnitOfWork unitOfWork
            )
        {
            this.systemEmailMRepository = systemEmailMRepository;
            this.systemScheduleRepository = systemScheduleRepository;
            this.unitOfWork = unitOfWork;
        }

        public void ExecSendEmail()
        {
            try
            {
                systemEmailMRepository.ExecSendEmail();
            }
            catch (Exception ex)
            {
                //插入错误数据
                EventLog.FilePath = new ServerInfoUtility().MapPath(StructConstants.Log_Path.BatchEmalLog);
                EventLog.Write(ex.ToString());
                throw;
            }
        }

        public void ExecBatch()
        {
            try
            {
                var list = systemScheduleRepository.ExecBatch();
                foreach (var item in list)
                {
                    switch (item.Function_Name)
                    {
                        case StructConstants.BatchModuleName.FMTDashboard_Module:
                        case StructConstants.BatchModuleName.FMTDashboard_Week_Module:
                        case StructConstants.BatchModuleName.FMTDashboard_Month_Module:
                        try
                        {
                            fixtureService.ExecFMTDashboard(StructConstants.BatchModuleName.FMTDashboard_Module, item.Plant_Organization_UID, item.System_Schedule_UID);
                        }
                        catch (Exception ex)
                        {
                            systemEmailMRepository.InsertExceptionBatchLog(item.System_Schedule_UID, ex.Message);
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.FilePath = new ServerInfoUtility().MapPath(StructConstants.Log_Path.BatchFMTDashBoardLog);
                EventLog.Write(ex.ToString());
                throw;
            }
        }





    }
}
