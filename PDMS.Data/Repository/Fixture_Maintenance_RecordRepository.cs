using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PDMS.Common.Helpers;
using PDMS.Model.EntityDTO;
using System.Text;
using PDMS.Common.Constants;

namespace PDMS.Data.Repository
{
    public interface IFixture_Maintenance_RecordRepository : IRepository<Fixture_Maintenance_Record>
    {
        IQueryable<Fixture_Maintenance_RecordDTO> QueryFixtureMaintenance(Fixture_Maintenance_RecordDTO searchModel, Page page, out int totalcount);
        Fixture_Maintenance_RecordDTO QueryFixtureMaintenanceByUid(int Fixture_Maintenance_Record_UID);
        List<Fixture_Maintenance_RecordDTO> DoExportFixtureMaintenanceReprot(string Fixture_Maintenance_Record_UIDs);
        List<Fixture_Maintenance_RecordDTO> DoAllExportFixtureMaintenanceReprot(Fixture_Maintenance_RecordDTO search);
        IEnumerable<Fixture_Maintenance_RecordDTO> GetFixtureMaintenance(string Fixture_Maintenance_Record_UIDs, int straus);
        List<Fixture_Maintenance_RecordDTO> GetFixtureMaintenanceList(Fixture_Maintenance_RecordDTO dto, int straus);
        List<Fixture_Maintenance_Record> GetUpdateFixture_Maintenance_Record(string fixture_Maintenance_Record_UIDs, int straus);
        List<FixtureMaintenance_PlanDTO> GetFixtureMaintenance_Plan(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string Maintenance_Type);
        List<Fixture_Maintenance_RecordDTO> GetFixture_Maintenance_RecordDTO(Fixture_Maintenance_RecordDTO dto);
        int GetFixtureCount(Fixture_Maintenance_RecordDTO searchModel);
        string CreateFixture_Maintenance_Record(List<Fixture_Maintenance_RecordDTO> recordDTOs, Fixture_Maintenance_RecordDTO dto, string maintenance_Record_NO);
        string UpdateFixture_Maintenance_Record(List<Fixture_Maintenance_Record> Fixture_Maintenance_Records, int NTID, string personNumber, string personName, DateTime date, int straus, int CurrentUserID);

    }

    public class Fixture_Maintenance_RecordRepository : RepositoryBase<Fixture_Maintenance_Record>, IFixture_Maintenance_RecordRepository
    {
        public Fixture_Maintenance_RecordRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
        /// <summary>
        /// 获取保养信息
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public IQueryable<Fixture_Maintenance_RecordDTO> QueryFixtureMaintenance(Fixture_Maintenance_RecordDTO searchModel, Page page, out int totalcount)
        {
            var query = from record in DataContext.Fixture_Maintenance_Record
                        select new Fixture_Maintenance_RecordDTO
                        {
                            Fixture_Maintenance_Record_UID = record.Fixture_Maintenance_Record_UID,
                            Fixture_Maintenance_Profile_UID = record.Fixture_Maintenance_Profile_UID,
                            Maintenance_Record_NO = record.Maintenance_Record_NO,
                            Fixture_M_UID = record.Fixture_M_UID,
                            Maintenance_Date = record.Maintenance_Date,
                            Maintenance_Status = record.Maintenance_Status,
                            Maintenance_Person_Number = record.Maintenance_Person_Number,
                            Maintenance_Person_Name = record.Maintenance_Person_Name,
                            Confirm_Date = record.Confirm_Date,
                            Confirm_Status = record.Confirm_Status,
                            Confirmor_UID = record.Confirmor_UID,
                            Confirmor = record.System_Users.User_Name,
                            Created_UID = record.Created_UID,
                            Created_Date = record.Created_Date,
                            Modified_UID = record.Modified_UID,
                            Modified_Date = record.Modified_Date,
                            Plant_Organization_UID = record.Fixture_Maintenance_Profile.Plant_Organization_UID,
                            BG_Organization_UID = record.Fixture_Maintenance_Profile.BG_Organization_UID,
                            FunPlant_Organization_UID = record.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                            Fixture_NO = record.Fixture_M.Fixture_NO,
                            Version = record.Fixture_M.Version,
                            Fixture_Seq = record.Fixture_M.Fixture_Seq,
                            Fixture_Unique_ID = record.Fixture_M.Fixture_Unique_ID,
                            Fixture_Name = record.Fixture_M.Fixture_Name,
                            Project_UID = record.Fixture_M.Project_UID,
                            Fixture_Machine_UID = record.Fixture_M.Fixture_Machine_UID,
                            Vendor_Info_UID = record.Fixture_M.Vendor_Info_UID,
                            Production_Line_UID = record.Fixture_M.Production_Line_UID,
                            Status = record.Fixture_M.Status,
                            ShortCode = record.Fixture_M.ShortCode,
                            TwoD_Barcode = record.Fixture_M.TwoD_Barcode,
                            Equipment_No = record.Fixture_M.Fixture_Machine.Equipment_No,
                            Machine_Type = record.Fixture_M.Fixture_Machine.Machine_Type,
                            Line_Name = record.Fixture_M.Production_Line.Line_Name,
                            Workshop = record.Fixture_M.Production_Line.Workshop.Workshop_Name,
                            Workshop_UID = record.Fixture_M.Production_Line.Workshop_UID,
                            Workstation = record.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                            Workstation_UID = record.Fixture_M.Production_Line.Workstation_UID,
                            Process_Info = record.Fixture_M.Production_Line.Process_Info.Process_Name,
                            Process_Info_UID = record.Fixture_M.Production_Line.Process_Info_UID,
                            Project = record.Fixture_M.System_Project.Project_Name,
                            WorkStation_ID = record.Fixture_M.Production_Line.WorkStation.WorkStation_ID,
                            Cycle_ALL = record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_ID + "_" + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Interval + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Unit,
                            Maintenance_Type = record.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                            //Createder = record.System_Users2.User_Name,
                            //Modifieder = record.System_Users3.User_Name
                        };


            if (searchModel.End_Date_From != null || searchModel.End_Date_To != null)
            {
                query = (from record in DataContext.Fixture_Maintenance_Record
                         select new Fixture_Maintenance_RecordDTO
                         {
                             Fixture_Maintenance_Record_UID = record.Fixture_Maintenance_Record_UID,
                             Fixture_Maintenance_Profile_UID = record.Fixture_Maintenance_Profile_UID,
                             Maintenance_Record_NO = record.Maintenance_Record_NO,
                             Fixture_M_UID = record.Fixture_M_UID,
                             Maintenance_Date = record.Maintenance_Date,
                             Maintenance_Status = record.Maintenance_Status,
                             Maintenance_Person_Number = record.Maintenance_Person_Number,
                             Maintenance_Person_Name = record.Maintenance_Person_Name,
                             Confirm_Date = record.Confirm_Date,
                             Confirm_Status = record.Confirm_Status,
                             Confirmor_UID = record.Confirmor_UID,
                             Confirmor = record.System_Users.User_Name,
                             Created_UID = record.Created_UID,
                             Created_Date = record.Created_Date,
                             Modified_UID = record.Modified_UID,
                             Modified_Date = record.Modified_Date,
                             Plant_Organization_UID = record.Fixture_Maintenance_Profile.Plant_Organization_UID,
                             BG_Organization_UID = record.Fixture_Maintenance_Profile.BG_Organization_UID,
                             FunPlant_Organization_UID = record.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                             Fixture_NO = record.Fixture_M.Fixture_NO,
                             Version = record.Fixture_M.Version,
                             Fixture_Seq = record.Fixture_M.Fixture_Seq,
                             Fixture_Unique_ID = record.Fixture_M.Fixture_Unique_ID,
                             Fixture_Name = record.Fixture_M.Fixture_Name,
                             Project_UID = record.Fixture_M.Project_UID,
                             Fixture_Machine_UID = record.Fixture_M.Fixture_Machine_UID,
                             Vendor_Info_UID = record.Fixture_M.Vendor_Info_UID,
                             Production_Line_UID = record.Fixture_M.Production_Line_UID,
                             Status = record.Fixture_M.Status,
                             ShortCode = record.Fixture_M.ShortCode,
                             TwoD_Barcode = record.Fixture_M.TwoD_Barcode,
                             Equipment_No = record.Fixture_M.Fixture_Machine.Equipment_No,
                             Machine_Type = record.Fixture_M.Fixture_Machine.Machine_Type,
                             Line_Name = record.Fixture_M.Production_Line.Line_Name,
                             Workshop = record.Fixture_M.Production_Line.Workshop.Workshop_Name,
                             Workshop_UID = record.Fixture_M.Production_Line.Workshop_UID,
                             Workstation = record.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                             Workstation_UID = record.Fixture_M.Production_Line.Workstation_UID,
                             Process_Info = record.Fixture_M.Production_Line.Process_Info.Process_Name,
                             Process_Info_UID = record.Fixture_M.Production_Line.Process_Info_UID,
                             Project = record.Fixture_M.System_Project.Project_Name,
                             WorkStation_ID = record.Fixture_M.Production_Line.WorkStation.WorkStation_ID,
                             Cycle_ALL = record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_ID + "_" + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Interval + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Unit,
                             Maintenance_Type = record.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                             //Createder = record.System_Users2.User_Name,
                             //Modifieder = record.System_Users3.User_Name
                         }).Union(from record__History in DataContext.Fixture_Maintenance_Record_History

                                  select new Fixture_Maintenance_RecordDTO
                                  {
                                      Fixture_Maintenance_Record_UID = record__History.Fixture_Maintenance_Record_UID,
                                      Fixture_Maintenance_Profile_UID = record__History.Fixture_Maintenance_Profile_UID,
                                      Maintenance_Record_NO = record__History.Maintenance_Record_NO,
                                      Fixture_M_UID = record__History.Fixture_M_UID,
                                      Maintenance_Date = record__History.Maintenance_Date,
                                      Maintenance_Status = record__History.Maintenance_Status,
                                      Maintenance_Person_Number = record__History.Maintenance_Person_Number,
                                      Maintenance_Person_Name = record__History.Maintenance_Person_Name,
                                      Confirm_Date = record__History.Confirm_Date,
                                      Confirm_Status = record__History.Confirm_Status,
                                      Confirmor_UID = record__History.Confirmor_UID,
                                      Confirmor = record__History.System_Users.User_Name,
                                      Created_UID = record__History.Created_UID,
                                      Created_Date = record__History.Created_Date,
                                      Modified_UID = record__History.Modified_UID,
                                      Modified_Date = record__History.Modified_Date,
                                      Plant_Organization_UID = record__History.Fixture_Maintenance_Profile.Plant_Organization_UID,
                                      BG_Organization_UID = record__History.Fixture_Maintenance_Profile.BG_Organization_UID,
                                      FunPlant_Organization_UID = record__History.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                                      Fixture_NO = record__History.Fixture_M.Fixture_NO,
                                      Version = record__History.Fixture_M.Version,
                                      Fixture_Seq = record__History.Fixture_M.Fixture_Seq,
                                      Fixture_Unique_ID = record__History.Fixture_M.Fixture_Unique_ID,
                                      Fixture_Name = record__History.Fixture_M.Fixture_Name,
                                      Project_UID = record__History.Fixture_M.Project_UID,
                                      Fixture_Machine_UID = record__History.Fixture_M.Fixture_Machine_UID,
                                      Vendor_Info_UID = record__History.Fixture_M.Vendor_Info_UID,
                                      Production_Line_UID = record__History.Fixture_M.Production_Line_UID,
                                      Status = record__History.Fixture_M.Status,
                                      ShortCode = record__History.Fixture_M.ShortCode,
                                      TwoD_Barcode = record__History.Fixture_M.TwoD_Barcode,
                                      Equipment_No = record__History.Fixture_M.Fixture_Machine.Equipment_No,
                                      Machine_Type = record__History.Fixture_M.Fixture_Machine.Machine_Type,
                                      Line_Name = record__History.Fixture_M.Production_Line.Line_Name,
                                      Workshop = record__History.Fixture_M.Production_Line.Workshop.Workshop_Name,
                                      Workshop_UID = record__History.Fixture_M.Production_Line.Workshop_UID,
                                      Workstation = record__History.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                                      Workstation_UID = record__History.Fixture_M.Production_Line.Workstation_UID,
                                      Process_Info = record__History.Fixture_M.Production_Line.Process_Info.Process_Name,
                                      Process_Info_UID = record__History.Fixture_M.Production_Line.Process_Info_UID,
                                      Project = record__History.Fixture_M.System_Project.Project_Name,
                                      WorkStation_ID = record__History.Fixture_M.Production_Line.WorkStation.WorkStation_ID,
                                      Cycle_ALL = record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_ID + "_" + record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Interval + record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Unit,
                                      Maintenance_Type = record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                                      //Createder = record.System_Users2.User_Name,
                                      //Modifieder = record.System_Users3.User_Name
                                  });
            }
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(searchModel.Fixture_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Version))
                query = query.Where(m => m.Version.Contains(searchModel.Version));
            if (!string.IsNullOrWhiteSpace(searchModel.Maintenance_Record_NO))
                query = query.Where(m => m.Maintenance_Record_NO.Contains(searchModel.Maintenance_Record_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Maintenance_Type))
                query = query.Where(m => m.Maintenance_Type == searchModel.Maintenance_Type);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                query = query.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));
            if (searchModel.Project_UID != null && searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.Workstation_UID != null && searchModel.Workstation_UID != 0)
                query = query.Where(m => m.Workstation_UID == searchModel.Workstation_UID);
            if (searchModel.Maintenance_Plan_UID != null && searchModel.Maintenance_Plan_UID != 0)
                query = query.Where(m => m.Maintenance_Plan_UID == searchModel.Maintenance_Plan_UID);
            if (searchModel.Workshop_UID != null && searchModel.Workshop_UID != 0)
                query = query.Where(m => m.Workshop_UID == searchModel.Workshop_UID);
            if (searchModel.Process_Info_UID != null && searchModel.Process_Info_UID != 0)
                query = query.Where(m => m.Process_Info_UID == searchModel.Process_Info_UID);
            if (searchModel.Fixture_Machine_UID != null && searchModel.Fixture_Machine_UID != 0)
                query = query.Where(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID);
            if (searchModel.Vendor_Info_UID != null && searchModel.Vendor_Info_UID != 0)
                query = query.Where(m => m.Vendor_Info_UID == searchModel.Vendor_Info_UID);
            if (searchModel.Production_Line_UID != null && searchModel.Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == searchModel.Production_Line_UID);
            if (searchModel.Status != 0)
                query = query.Where(m => m.Status == searchModel.Status);
            if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                query = query.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
            //if (!string.IsNullOrWhiteSpace(searchModel.TwoD_Barcode))
            //    query = query.Where(m => m.TwoD_Barcode.Contains(searchModel.TwoD_Barcode));
            if (searchModel.Maintenance_Status != null)
            {
                if (searchModel.Maintenance_Status != 0)
                {
                    query = query.Where(m => m.Maintenance_Status == searchModel.Maintenance_Status);
                }
                else
                {
                    query = query.Where(m => m.Maintenance_Status != 1 && m.Maintenance_Status != 2);
                }
            }
            if (searchModel.Confirm_Status != null)
            {
                if (searchModel.Confirm_Status != 0)
                {
                    query = query.Where(m => m.Confirm_Status == searchModel.Confirm_Status);
                }
                else
                {
                    query = query.Where(m => m.Confirm_Status != 1 && m.Confirm_Status != 2);
                }
            }
            //if (!string.IsNullOrWhiteSpace(searchModel.Modifieder))
            //    query = query.Where(m => m.Modifieder.Contains(searchModel.Modifieder));
            if (searchModel.Modified_UID != 0)
                query = query.Where(m => m.Modified_UID == searchModel.Modified_UID);
            if (searchModel.End_Date_From != null)
                query = query.Where(m => m.Created_Date >= searchModel.End_Date_From);
            if (searchModel.End_Date_To != null)
            {
                DateTime nextTime = searchModel.End_Date_To.Value.Date.AddDays(1);
                query = query.Where(m => m.Modified_Date < nextTime);
            }
            totalcount = query.Count();
            //jay(1286146 20181220)耗时太长,查询一次耗时2分钟，以下两行代码交换了位置
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Fixture_M_UID).GetPage(page);
            query = SetFixtureDTO(query.ToList());
            return query;
        }
        public IQueryable<Fixture_Maintenance_RecordDTO> SetFixtureDTO(List<Fixture_Maintenance_RecordDTO> Fixtures)
        {
            List<System_Organization> system_Organizations = DataContext.System_Organization.ToList();
            List<Vendor_Info> vendor_Infos = DataContext.Vendor_Info.ToList();
            // List<FixtureStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
            foreach (var item in Fixtures)
            {
                // 设置厂区                            
                var system_OrganizationPlant = system_Organizations.Where(o => o.Organization_UID == item.Plant_Organization_UID).FirstOrDefault();
                if (system_OrganizationPlant != null)
                {
                    item.PlantName = system_OrganizationPlant.Organization_Name;
                }
                //设置OP
                var system_OrganizationOP = system_Organizations.Where(o => o.Organization_UID == item.BG_Organization_UID).FirstOrDefault();
                if (system_OrganizationOP != null)
                {
                    item.OPName = system_OrganizationOP.Organization_Name;
                }
                //设置功能厂
                if (item.FunPlant_Organization_UID != null && item.FunPlant_Organization_UID != 0)
                {
                    var system_OrganizationFunPlant = system_Organizations.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).FirstOrDefault();
                    if (system_OrganizationFunPlant != null)
                    {
                        item.FunPlantName = system_OrganizationFunPlant.Organization_Name;
                    }
                }
                //设置厂商
                if (item.Vendor_Info_UID != null && item.Vendor_Info_UID != 0)
                {
                    var vendor_Info = vendor_Infos.Where(o => o.Vendor_Info_UID == item.Vendor_Info_UID).FirstOrDefault();
                    if (vendor_Info != null)
                    {
                        item.Vendor_Name = vendor_Info.Vendor_Name;
                    }
                }
                //var status = fixtureStatuDTOs.Where(o => o.Status == item.Status).FirstOrDefault();
                //if (status != null)
                //{
                //    item.StatuName = status.StatuName;
                //}

            }
            return Fixtures.AsQueryable();
        }
        public Fixture_Maintenance_RecordDTO QueryFixtureMaintenanceByUid(int Fixture_Maintenance_Record_UID)
        {

            //var query = from record in DataContext.Fixture_Maintenance_Record
            //            select new Fixture_Maintenance_RecordDTO
            //            {
            //                Fixture_Maintenance_Record_UID = record.Fixture_Maintenance_Record_UID,
            //                Fixture_Maintenance_Profile_UID = record.Fixture_Maintenance_Profile_UID,
            //                Maintenance_Record_NO = record.Maintenance_Record_NO,
            //                Fixture_M_UID = record.Fixture_M_UID,
            //                Maintenance_Date = record.Maintenance_Date,
            //                Maintenance_Status = record.Maintenance_Status,
            //                Maintenance_Person_Number = record.Maintenance_Person_Number,
            //                Maintenance_Person_Name = record.Maintenance_Person_Name,
            //                Confirm_Date = record.Confirm_Date,
            //                Confirm_Status = record.Confirm_Status,
            //                Confirmor_UID = record.Confirmor_UID,
            //                Created_UID = record.Created_UID,
            //                Created_Date = record.Created_Date,
            //                Modified_UID = record.Modified_UID,
            //                Modified_Date = record.Modified_Date,
            //                Plant_Organization_UID = record.Fixture_Maintenance_Profile.Plant_Organization_UID,
            //                BG_Organization_UID = record.Fixture_Maintenance_Profile.BG_Organization_UID,
            //                FunPlant_Organization_UID = record.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
            //                Fixture_NO = record.Fixture_M.Fixture_NO,
            //                Version = record.Fixture_M.Version,
            //                Fixture_Seq = record.Fixture_M.Fixture_Seq,
            //                Fixture_Unique_ID = record.Fixture_M.Fixture_Unique_ID,
            //                Fixture_Name = record.Fixture_M.Fixture_Name,
            //                Project_UID = record.Fixture_M.Project_UID,
            //                Fixture_Machine_UID = record.Fixture_M.Fixture_Machine_UID,
            //                Vendor_Info_UID = record.Fixture_M.Vendor_Info_UID,
            //                Production_Line_UID = record.Fixture_M.Production_Line_UID,
            //                Status = record.Fixture_M.Status,
            //                ShortCode = record.Fixture_M.ShortCode,
            //                TwoD_Barcode = record.Fixture_M.TwoD_Barcode,
            //                Equipment_No = record.Fixture_M.Fixture_Machine.Equipment_No,
            //                Machine_Type = record.Fixture_M.Fixture_Machine.Machine_Type,
            //                Line_Name = record.Fixture_M.Production_Line.Line_Name,
            //                Workshop = record.Fixture_M.Production_Line.Workshop.Workshop_Name,
            //                Workshop_UID = record.Fixture_M.Production_Line.Workshop_UID,
            //                Workstation = record.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
            //                Workstation_UID = record.Fixture_M.Production_Line.Workstation_UID,
            //                Process_Info = record.Fixture_M.Production_Line.Process_Info.Process_Name,
            //                Process_Info_UID = record.Fixture_M.Production_Line.Process_Info_UID,
            //                Confirmor = record.System_Users.User_Name,
            //                Project = record.Fixture_M.System_Project.Project_Name,
            //                WorkStation_ID = record.Fixture_M.Production_Line.WorkStation.WorkStation_ID,
            //            };


          var  query = (from record in DataContext.Fixture_Maintenance_Record
                     select new Fixture_Maintenance_RecordDTO
                     {
                         Fixture_Maintenance_Record_UID = record.Fixture_Maintenance_Record_UID,
                         Fixture_Maintenance_Profile_UID = record.Fixture_Maintenance_Profile_UID,
                         Maintenance_Record_NO = record.Maintenance_Record_NO,
                         Fixture_M_UID = record.Fixture_M_UID,
                         Maintenance_Date = record.Maintenance_Date,
                         Maintenance_Status = record.Maintenance_Status,
                         Maintenance_Person_Number = record.Maintenance_Person_Number,
                         Maintenance_Person_Name = record.Maintenance_Person_Name,
                         Confirm_Date = record.Confirm_Date,
                         Confirm_Status = record.Confirm_Status,
                         Confirmor_UID = record.Confirmor_UID,
                         Confirmor = record.System_Users.User_Name,
                         Created_UID = record.Created_UID,
                         Created_Date = record.Created_Date,
                         Modified_UID = record.Modified_UID,
                         Modified_Date = record.Modified_Date,
                         Plant_Organization_UID = record.Fixture_Maintenance_Profile.Plant_Organization_UID,
                         BG_Organization_UID = record.Fixture_Maintenance_Profile.BG_Organization_UID,
                         FunPlant_Organization_UID = record.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                         Fixture_NO = record.Fixture_M.Fixture_NO,
                         Version = record.Fixture_M.Version,
                         Fixture_Seq = record.Fixture_M.Fixture_Seq,
                         Fixture_Unique_ID = record.Fixture_M.Fixture_Unique_ID,
                         Fixture_Name = record.Fixture_M.Fixture_Name,
                         Project_UID = record.Fixture_M.Project_UID,
                         Fixture_Machine_UID = record.Fixture_M.Fixture_Machine_UID,
                         Vendor_Info_UID = record.Fixture_M.Vendor_Info_UID,
                         Production_Line_UID = record.Fixture_M.Production_Line_UID,
                         Status = record.Fixture_M.Status,
                         ShortCode = record.Fixture_M.ShortCode,
                         TwoD_Barcode = record.Fixture_M.TwoD_Barcode,
                         Equipment_No = record.Fixture_M.Fixture_Machine.Equipment_No,
                         Machine_Type = record.Fixture_M.Fixture_Machine.Machine_Type,
                         Line_Name = record.Fixture_M.Production_Line.Line_Name,
                         Workshop = record.Fixture_M.Production_Line.Workshop.Workshop_Name,
                         Workshop_UID = record.Fixture_M.Production_Line.Workshop_UID,
                         Workstation = record.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                         Workstation_UID = record.Fixture_M.Production_Line.Workstation_UID,
                         Process_Info = record.Fixture_M.Production_Line.Process_Info.Process_Name,
                         Process_Info_UID = record.Fixture_M.Production_Line.Process_Info_UID,
                         Project = record.Fixture_M.System_Project.Project_Name,
                         WorkStation_ID = record.Fixture_M.Production_Line.WorkStation.WorkStation_ID,
                         Cycle_ALL = record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_ID + "_" + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Interval + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Unit,
                         Maintenance_Type = record.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                         //Createder = record.System_Users2.User_Name,
                         //Modifieder = record.System_Users3.User_Name
                     }).Union(from record__History in DataContext.Fixture_Maintenance_Record_History

                              select new Fixture_Maintenance_RecordDTO
                              {
                                  Fixture_Maintenance_Record_UID = record__History.Fixture_Maintenance_Record_UID,
                                  Fixture_Maintenance_Profile_UID = record__History.Fixture_Maintenance_Profile_UID,
                                  Maintenance_Record_NO = record__History.Maintenance_Record_NO,
                                  Fixture_M_UID = record__History.Fixture_M_UID,
                                  Maintenance_Date = record__History.Maintenance_Date,
                                  Maintenance_Status = record__History.Maintenance_Status,
                                  Maintenance_Person_Number = record__History.Maintenance_Person_Number,
                                  Maintenance_Person_Name = record__History.Maintenance_Person_Name,
                                  Confirm_Date = record__History.Confirm_Date,
                                  Confirm_Status = record__History.Confirm_Status,
                                  Confirmor_UID = record__History.Confirmor_UID,
                                  Confirmor = record__History.System_Users.User_Name,
                                  Created_UID = record__History.Created_UID,
                                  Created_Date = record__History.Created_Date,
                                  Modified_UID = record__History.Modified_UID,
                                  Modified_Date = record__History.Modified_Date,
                                  Plant_Organization_UID = record__History.Fixture_Maintenance_Profile.Plant_Organization_UID,
                                  BG_Organization_UID = record__History.Fixture_Maintenance_Profile.BG_Organization_UID,
                                  FunPlant_Organization_UID = record__History.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                                  Fixture_NO = record__History.Fixture_M.Fixture_NO,
                                  Version = record__History.Fixture_M.Version,
                                  Fixture_Seq = record__History.Fixture_M.Fixture_Seq,
                                  Fixture_Unique_ID = record__History.Fixture_M.Fixture_Unique_ID,
                                  Fixture_Name = record__History.Fixture_M.Fixture_Name,
                                  Project_UID = record__History.Fixture_M.Project_UID,
                                  Fixture_Machine_UID = record__History.Fixture_M.Fixture_Machine_UID,
                                  Vendor_Info_UID = record__History.Fixture_M.Vendor_Info_UID,
                                  Production_Line_UID = record__History.Fixture_M.Production_Line_UID,
                                  Status = record__History.Fixture_M.Status,
                                  ShortCode = record__History.Fixture_M.ShortCode,
                                  TwoD_Barcode = record__History.Fixture_M.TwoD_Barcode,
                                  Equipment_No = record__History.Fixture_M.Fixture_Machine.Equipment_No,
                                  Machine_Type = record__History.Fixture_M.Fixture_Machine.Machine_Type,
                                  Line_Name = record__History.Fixture_M.Production_Line.Line_Name,
                                  Workshop = record__History.Fixture_M.Production_Line.Workshop.Workshop_Name,
                                  Workshop_UID = record__History.Fixture_M.Production_Line.Workshop_UID,
                                  Workstation = record__History.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                                  Workstation_UID = record__History.Fixture_M.Production_Line.Workstation_UID,
                                  Process_Info = record__History.Fixture_M.Production_Line.Process_Info.Process_Name,
                                  Process_Info_UID = record__History.Fixture_M.Production_Line.Process_Info_UID,
                                  Project = record__History.Fixture_M.System_Project.Project_Name,
                                  WorkStation_ID = record__History.Fixture_M.Production_Line.WorkStation.WorkStation_ID,
                                  Cycle_ALL = record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_ID + "_" + record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Interval + record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Unit,
                                  Maintenance_Type = record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                                  //Createder = record.System_Users2.User_Name,
                                  //Modifieder = record.System_Users3.User_Name
                              });
            query = query.Where(m => m.Fixture_Maintenance_Record_UID == Fixture_Maintenance_Record_UID);
            var fixtures = SetListFixtureDTO(query.ToList());
            if (fixtures.Count > 0)
            {
                return fixtures.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 设置加载厂区，OP，功能厂，厂商
        /// </summary>
        /// <param name="Fixtures"></param>
        /// <returns></returns>
        public List<Fixture_Maintenance_RecordDTO> SetListFixtureDTO(List<Fixture_Maintenance_RecordDTO> Fixtures)
        {
            List<System_Organization> system_Organizations = DataContext.System_Organization.ToList();
            List<Vendor_Info> vendor_Infos = DataContext.Vendor_Info.ToList();
            List<FixtureStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
            foreach (var item in Fixtures)
            {
                // 设置厂区                            
                var system_OrganizationPlant = system_Organizations.Where(o => o.Organization_UID == item.Plant_Organization_UID).FirstOrDefault();
                if (system_OrganizationPlant != null)
                {
                    item.PlantName = system_OrganizationPlant.Organization_Name;
                }
                //设置OP
                var system_OrganizationOP = system_Organizations.Where(o => o.Organization_UID == item.BG_Organization_UID).FirstOrDefault();
                if (system_OrganizationOP != null)
                {
                    item.OPName = system_OrganizationOP.Organization_Name;
                }
                //设置功能厂
                if (item.FunPlant_Organization_UID != null && item.FunPlant_Organization_UID != 0)
                {
                    var system_OrganizationFunPlant = system_Organizations.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).FirstOrDefault();
                    if (system_OrganizationFunPlant != null)
                    {
                        item.FunPlantName = system_OrganizationFunPlant.Organization_Name;
                    }
                }
                //设置厂商
                if (item.Vendor_Info_UID != null && item.Vendor_Info_UID != 0)
                {
                    var vendor_Info = vendor_Infos.Where(o => o.Vendor_Info_UID == item.Vendor_Info_UID).FirstOrDefault();
                    if (vendor_Info != null)
                    {
                        item.Vendor_Name = vendor_Info.Vendor_Name;
                    }
                }

                var status = fixtureStatuDTOs.Where(o => o.Status == item.Status).FirstOrDefault();
                if (status != null)
                {
                    item.StatuName = status.StatuName;
                }
            }
            return Fixtures;
        }
        public List<FixtureStatuDTO> GetFixtureStatuDTO()
        {
            ///治具狀態(1:使用中In - PRD; 2:未使用Non - PRD; 3.維修中In - Repair; 4.報廢Scrap; 5:返供應商維修RTV; 6:保養逾時Over - Due Maintenance)
            List<FixtureStatuDTO> fixtureStatuDTOs = new List<FixtureStatuDTO>();
            List<Enumeration> enumerationItems = DataContext.Enumeration.Where(o => o.Enum_Type == "Fixture_Status").ToList();
            foreach (var item in enumerationItems)
            {
                FixtureStatuDTO fixtureStatuDTO = new FixtureStatuDTO();
                fixtureStatuDTO.StatuName = item.Enum_Value;
                fixtureStatuDTO.Status = item.Enum_UID;
                fixtureStatuDTOs.Add(fixtureStatuDTO);
            }
            return fixtureStatuDTOs;
        }
        public List<Fixture_Maintenance_RecordDTO> DoAllExportFixtureMaintenanceReprot(Fixture_Maintenance_RecordDTO searchModel)
        {


            var query = from record in DataContext.Fixture_Maintenance_Record
                        select new Fixture_Maintenance_RecordDTO
                        {
                            Fixture_Maintenance_Record_UID = record.Fixture_Maintenance_Record_UID,
                            Fixture_Maintenance_Profile_UID = record.Fixture_Maintenance_Profile_UID,
                            Maintenance_Record_NO = record.Maintenance_Record_NO,
                            Fixture_M_UID = record.Fixture_M_UID,
                            Maintenance_Date = record.Maintenance_Date,
                            Maintenance_Status = record.Maintenance_Status,
                            Maintenance_Person_Number = record.Maintenance_Person_Number,
                            Maintenance_Person_Name = record.Maintenance_Person_Name,
                            Confirm_Date = record.Confirm_Date,
                            Confirm_Status = record.Confirm_Status,
                            Confirmor_UID = record.Confirmor_UID,
                            Confirmor = record.System_Users.User_Name,
                            Created_UID = record.Created_UID,
                            Created_Date = record.Created_Date,
                            Modified_UID = record.Modified_UID,
                            Modified_Date = record.Modified_Date,
                            Plant_Organization_UID = record.Fixture_Maintenance_Profile.Plant_Organization_UID,
                            BG_Organization_UID = record.Fixture_Maintenance_Profile.BG_Organization_UID,
                            FunPlant_Organization_UID = record.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                            Fixture_NO = record.Fixture_M.Fixture_NO,
                            Version = record.Fixture_M.Version,
                            Fixture_Seq = record.Fixture_M.Fixture_Seq,
                            Fixture_Unique_ID = record.Fixture_M.Fixture_Unique_ID,
                            Fixture_Name = record.Fixture_M.Fixture_Name,
                            Project_UID = record.Fixture_M.Project_UID,
                            Fixture_Machine_UID = record.Fixture_M.Fixture_Machine_UID,
                            Vendor_Info_UID = record.Fixture_M.Vendor_Info_UID,
                            Production_Line_UID = record.Fixture_M.Production_Line_UID,
                            Status = record.Fixture_M.Status,
                            ShortCode = record.Fixture_M.ShortCode,
                            TwoD_Barcode = record.Fixture_M.TwoD_Barcode,
                            Equipment_No = record.Fixture_M.Fixture_Machine.Equipment_No,
                            Machine_Type = record.Fixture_M.Fixture_Machine.Machine_Type,
                            Line_Name = record.Fixture_M.Production_Line.Line_Name,
                            Workshop = record.Fixture_M.Production_Line.Workshop.Workshop_Name,
                            Workshop_UID = record.Fixture_M.Production_Line.Workshop_UID,
                            Workstation = record.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                            Workstation_UID = record.Fixture_M.Production_Line.Workstation_UID,
                            Process_Info = record.Fixture_M.Production_Line.Process_Info.Process_Name,
                            Process_Info_UID = record.Fixture_M.Production_Line.Process_Info_UID,
                            Project = record.Fixture_M.System_Project.Project_Name,
                            WorkStation_ID = record.Fixture_M.Production_Line.WorkStation.WorkStation_ID,
                            Cycle_ALL = record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_ID + "_" + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Interval + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Unit,
                            Maintenance_Type = record.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                            //Createder = record.System_Users2.User_Name,
                            //Modifieder = record.System_Users3.User_Name
                        };
            if (searchModel.End_Date_From != null || searchModel.End_Date_To != null)
            {
                query = (from record in DataContext.Fixture_Maintenance_Record
                         select new Fixture_Maintenance_RecordDTO
                         {
                             Fixture_Maintenance_Record_UID = record.Fixture_Maintenance_Record_UID,
                             Fixture_Maintenance_Profile_UID = record.Fixture_Maintenance_Profile_UID,
                             Maintenance_Record_NO = record.Maintenance_Record_NO,
                             Fixture_M_UID = record.Fixture_M_UID,
                             Maintenance_Date = record.Maintenance_Date,
                             Maintenance_Status = record.Maintenance_Status,
                             Maintenance_Person_Number = record.Maintenance_Person_Number,
                             Maintenance_Person_Name = record.Maintenance_Person_Name,
                             Confirm_Date = record.Confirm_Date,
                             Confirm_Status = record.Confirm_Status,
                             Confirmor_UID = record.Confirmor_UID,
                             Confirmor = record.System_Users.User_Name,
                             Created_UID = record.Created_UID,
                             Created_Date = record.Created_Date,
                             Modified_UID = record.Modified_UID,
                             Modified_Date = record.Modified_Date,
                             Plant_Organization_UID = record.Fixture_Maintenance_Profile.Plant_Organization_UID,
                             BG_Organization_UID = record.Fixture_Maintenance_Profile.BG_Organization_UID,
                             FunPlant_Organization_UID = record.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                             Fixture_NO = record.Fixture_M.Fixture_NO,
                             Version = record.Fixture_M.Version,
                             Fixture_Seq = record.Fixture_M.Fixture_Seq,
                             Fixture_Unique_ID = record.Fixture_M.Fixture_Unique_ID,
                             Fixture_Name = record.Fixture_M.Fixture_Name,
                             Project_UID = record.Fixture_M.Project_UID,
                             Fixture_Machine_UID = record.Fixture_M.Fixture_Machine_UID,
                             Vendor_Info_UID = record.Fixture_M.Vendor_Info_UID,
                             Production_Line_UID = record.Fixture_M.Production_Line_UID,
                             Status = record.Fixture_M.Status,
                             ShortCode = record.Fixture_M.ShortCode,
                             TwoD_Barcode = record.Fixture_M.TwoD_Barcode,
                             Equipment_No = record.Fixture_M.Fixture_Machine.Equipment_No,
                             Machine_Type = record.Fixture_M.Fixture_Machine.Machine_Type,
                             Line_Name = record.Fixture_M.Production_Line.Line_Name,
                             Workshop = record.Fixture_M.Production_Line.Workshop.Workshop_Name,
                             Workshop_UID = record.Fixture_M.Production_Line.Workshop_UID,
                             Workstation = record.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                             Workstation_UID = record.Fixture_M.Production_Line.Workstation_UID,
                             Process_Info = record.Fixture_M.Production_Line.Process_Info.Process_Name,
                             Process_Info_UID = record.Fixture_M.Production_Line.Process_Info_UID,
                             Project = record.Fixture_M.System_Project.Project_Name,
                             WorkStation_ID = record.Fixture_M.Production_Line.WorkStation.WorkStation_ID,
                             Cycle_ALL = record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_ID + "_" + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Interval + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Unit,
                             Maintenance_Type = record.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                             //Createder = record.System_Users2.User_Name,
                             //Modifieder = record.System_Users3.User_Name
                         }).Union(from record__History in DataContext.Fixture_Maintenance_Record_History

                                  select new Fixture_Maintenance_RecordDTO
                                  {
                                      Fixture_Maintenance_Record_UID = record__History.Fixture_Maintenance_Record_UID,
                                      Fixture_Maintenance_Profile_UID = record__History.Fixture_Maintenance_Profile_UID,
                                      Maintenance_Record_NO = record__History.Maintenance_Record_NO,
                                      Fixture_M_UID = record__History.Fixture_M_UID,
                                      Maintenance_Date = record__History.Maintenance_Date,
                                      Maintenance_Status = record__History.Maintenance_Status,
                                      Maintenance_Person_Number = record__History.Maintenance_Person_Number,
                                      Maintenance_Person_Name = record__History.Maintenance_Person_Name,
                                      Confirm_Date = record__History.Confirm_Date,
                                      Confirm_Status = record__History.Confirm_Status,
                                      Confirmor_UID = record__History.Confirmor_UID,
                                      Confirmor = record__History.System_Users.User_Name,
                                      Created_UID = record__History.Created_UID,
                                      Created_Date = record__History.Created_Date,
                                      Modified_UID = record__History.Modified_UID,
                                      Modified_Date = record__History.Modified_Date,
                                      Plant_Organization_UID = record__History.Fixture_Maintenance_Profile.Plant_Organization_UID,
                                      BG_Organization_UID = record__History.Fixture_Maintenance_Profile.BG_Organization_UID,
                                      FunPlant_Organization_UID = record__History.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                                      Fixture_NO = record__History.Fixture_M.Fixture_NO,
                                      Version = record__History.Fixture_M.Version,
                                      Fixture_Seq = record__History.Fixture_M.Fixture_Seq,
                                      Fixture_Unique_ID = record__History.Fixture_M.Fixture_Unique_ID,
                                      Fixture_Name = record__History.Fixture_M.Fixture_Name,
                                      Project_UID = record__History.Fixture_M.Project_UID,
                                      Fixture_Machine_UID = record__History.Fixture_M.Fixture_Machine_UID,
                                      Vendor_Info_UID = record__History.Fixture_M.Vendor_Info_UID,
                                      Production_Line_UID = record__History.Fixture_M.Production_Line_UID,
                                      Status = record__History.Fixture_M.Status,
                                      ShortCode = record__History.Fixture_M.ShortCode,
                                      TwoD_Barcode = record__History.Fixture_M.TwoD_Barcode,
                                      Equipment_No = record__History.Fixture_M.Fixture_Machine.Equipment_No,
                                      Machine_Type = record__History.Fixture_M.Fixture_Machine.Machine_Type,
                                      Line_Name = record__History.Fixture_M.Production_Line.Line_Name,
                                      Workshop = record__History.Fixture_M.Production_Line.Workshop.Workshop_Name,
                                      Workshop_UID = record__History.Fixture_M.Production_Line.Workshop_UID,
                                      Workstation = record__History.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                                      Workstation_UID = record__History.Fixture_M.Production_Line.Workstation_UID,
                                      Process_Info = record__History.Fixture_M.Production_Line.Process_Info.Process_Name,
                                      Process_Info_UID = record__History.Fixture_M.Production_Line.Process_Info_UID,
                                      Project = record__History.Fixture_M.System_Project.Project_Name,
                                      WorkStation_ID = record__History.Fixture_M.Production_Line.WorkStation.WorkStation_ID,
                                      Cycle_ALL = record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_ID + "_" + record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Interval + record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Unit,
                                      Maintenance_Type = record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                                      //Createder = record.System_Users2.User_Name,
                                      //Modifieder = record.System_Users3.User_Name
                                  });
            }


            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(searchModel.Fixture_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Version))
                query = query.Where(m => m.Version.Contains(searchModel.Version));
            if (!string.IsNullOrWhiteSpace(searchModel.Maintenance_Record_NO))
                query = query.Where(m => m.Maintenance_Record_NO.Contains(searchModel.Maintenance_Record_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Maintenance_Type))
                query = query.Where(m => m.Maintenance_Type == searchModel.Maintenance_Type);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                query = query.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));
            if (searchModel.Project_UID != null && searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.Workstation_UID != null && searchModel.Workstation_UID != 0)
                query = query.Where(m => m.Workstation_UID == searchModel.Workstation_UID);
            if (searchModel.Maintenance_Plan_UID != null && searchModel.Maintenance_Plan_UID != 0)
                query = query.Where(m => m.Maintenance_Plan_UID == searchModel.Maintenance_Plan_UID);
            if (searchModel.Workshop_UID != null && searchModel.Workshop_UID != 0)
                query = query.Where(m => m.Workshop_UID == searchModel.Workshop_UID);
            if (searchModel.Process_Info_UID != null && searchModel.Process_Info_UID != 0)
                query = query.Where(m => m.Process_Info_UID == searchModel.Process_Info_UID);
            if (searchModel.Fixture_Machine_UID != null && searchModel.Fixture_Machine_UID != 0)
                query = query.Where(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID);
            if (searchModel.Vendor_Info_UID != null && searchModel.Vendor_Info_UID != 0)
                query = query.Where(m => m.Vendor_Info_UID == searchModel.Vendor_Info_UID);
            if (searchModel.Production_Line_UID != null && searchModel.Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == searchModel.Production_Line_UID);
            if (searchModel.Status != 0)
                query = query.Where(m => m.Status == searchModel.Status);
            if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                query = query.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
            //if (!string.IsNullOrWhiteSpace(searchModel.TwoD_Barcode))
            //    query = query.Where(m => m.TwoD_Barcode.Contains(searchModel.TwoD_Barcode));
            if (searchModel.Maintenance_Status != null)
            {
                if (searchModel.Maintenance_Status != 0)
                {
                    query = query.Where(m => m.Maintenance_Status == searchModel.Maintenance_Status);
                }
                else
                {
                    query = query.Where(m => m.Maintenance_Status != 1 && m.Maintenance_Status != 2);
                }

            }
            if (searchModel.Confirm_Status != null)
            {
                if (searchModel.Confirm_Status != 0)
                {
                    query = query.Where(m => m.Confirm_Status == searchModel.Confirm_Status);
                }
                else
                {
                    query = query.Where(m => m.Confirm_Status != 1 && m.Confirm_Status != 2);
                }

            }
            //if (!string.IsNullOrWhiteSpace(searchModel.Modifieder))
            //    query = query.Where(m => m.Modifieder.Contains(searchModel.Modifieder));
            if (searchModel.Modified_UID != 0)
                query = query.Where(m => m.Modified_UID == searchModel.Modified_UID);
            if (searchModel.End_Date_From != null)
                query = query.Where(m => m.Created_Date >= searchModel.End_Date_From);
            if (searchModel.End_Date_To != null)
            {
                DateTime nextTime = searchModel.End_Date_To.Value.Date.AddDays(1);
                query = query.Where(m => m.Modified_Date < nextTime);
            }
            query = query.Take(10000);                
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Fixture_M_UID);
            query = SetFixtureDTO(query.ToList());
            return query.ToList();

        }
        public List<Fixture_Maintenance_RecordDTO> DoExportFixtureMaintenanceReprot(string Fixture_Maintenance_Record_UIDs)
        {

            Fixture_Maintenance_Record_UIDs = "," + Fixture_Maintenance_Record_UIDs + ",";
            var query =  (from record in DataContext.Fixture_Maintenance_Record
                                 select new Fixture_Maintenance_RecordDTO
                                 {
                                     Fixture_Maintenance_Record_UID = record.Fixture_Maintenance_Record_UID,
                                     Fixture_Maintenance_Profile_UID = record.Fixture_Maintenance_Profile_UID,
                                     Maintenance_Record_NO = record.Maintenance_Record_NO,
                                     Fixture_M_UID = record.Fixture_M_UID,
                                     Maintenance_Date = record.Maintenance_Date,
                                     Maintenance_Status = record.Maintenance_Status,
                                     Maintenance_Person_Number = record.Maintenance_Person_Number,
                                     Maintenance_Person_Name = record.Maintenance_Person_Name,
                                     Confirm_Date = record.Confirm_Date,
                                     Confirm_Status = record.Confirm_Status,
                                     Confirmor_UID = record.Confirmor_UID,
                                     Confirmor = record.System_Users.User_Name,
                                     Created_UID = record.Created_UID,
                                     Created_Date = record.Created_Date,
                                     Modified_UID = record.Modified_UID,
                                     Modified_Date = record.Modified_Date,
                                     Plant_Organization_UID = record.Fixture_Maintenance_Profile.Plant_Organization_UID,
                                     BG_Organization_UID = record.Fixture_Maintenance_Profile.BG_Organization_UID,
                                     FunPlant_Organization_UID = record.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                                     Fixture_NO = record.Fixture_M.Fixture_NO,
                                     Version = record.Fixture_M.Version,
                                     Fixture_Seq = record.Fixture_M.Fixture_Seq,
                                     Fixture_Unique_ID = record.Fixture_M.Fixture_Unique_ID,
                                     Fixture_Name = record.Fixture_M.Fixture_Name,
                                     Project_UID = record.Fixture_M.Project_UID,
                                     Fixture_Machine_UID = record.Fixture_M.Fixture_Machine_UID,
                                     Vendor_Info_UID = record.Fixture_M.Vendor_Info_UID,
                                     Production_Line_UID = record.Fixture_M.Production_Line_UID,
                                     Status = record.Fixture_M.Status,
                                     ShortCode = record.Fixture_M.ShortCode,
                                     TwoD_Barcode = record.Fixture_M.TwoD_Barcode,
                                     Equipment_No = record.Fixture_M.Fixture_Machine.Equipment_No,
                                     Machine_Type = record.Fixture_M.Fixture_Machine.Machine_Type,
                                     Line_Name = record.Fixture_M.Production_Line.Line_Name,
                                     Workshop = record.Fixture_M.Production_Line.Workshop.Workshop_Name,
                                     Workshop_UID = record.Fixture_M.Production_Line.Workshop_UID,
                                     Workstation = record.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                                     Workstation_UID = record.Fixture_M.Production_Line.Workstation_UID,
                                     Process_Info = record.Fixture_M.Production_Line.Process_Info.Process_Name,
                                     Process_Info_UID = record.Fixture_M.Production_Line.Process_Info_UID,
                                     Project = record.Fixture_M.System_Project.Project_Name,
                                     WorkStation_ID = record.Fixture_M.Production_Line.WorkStation.WorkStation_ID,
                                     Cycle_ALL = record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_ID + "_" + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Interval + record.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Unit,
                                     Maintenance_Type = record.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                                     //Createder = record.System_Users2.User_Name,
                                     //Modifieder = record.System_Users3.User_Name
                                 }).Union(from record__History in DataContext.Fixture_Maintenance_Record_History

                                          select new Fixture_Maintenance_RecordDTO
                                          {
                                              Fixture_Maintenance_Record_UID = record__History.Fixture_Maintenance_Record_UID,
                                              Fixture_Maintenance_Profile_UID = record__History.Fixture_Maintenance_Profile_UID,
                                              Maintenance_Record_NO = record__History.Maintenance_Record_NO,
                                              Fixture_M_UID = record__History.Fixture_M_UID,
                                              Maintenance_Date = record__History.Maintenance_Date,
                                              Maintenance_Status = record__History.Maintenance_Status,
                                              Maintenance_Person_Number = record__History.Maintenance_Person_Number,
                                              Maintenance_Person_Name = record__History.Maintenance_Person_Name,
                                              Confirm_Date = record__History.Confirm_Date,
                                              Confirm_Status = record__History.Confirm_Status,
                                              Confirmor_UID = record__History.Confirmor_UID,
                                              Confirmor = record__History.System_Users.User_Name,
                                              Created_UID = record__History.Created_UID,
                                              Created_Date = record__History.Created_Date,
                                              Modified_UID = record__History.Modified_UID,
                                              Modified_Date = record__History.Modified_Date,
                                              Plant_Organization_UID = record__History.Fixture_Maintenance_Profile.Plant_Organization_UID,
                                              BG_Organization_UID = record__History.Fixture_Maintenance_Profile.BG_Organization_UID,
                                              FunPlant_Organization_UID = record__History.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                                              Fixture_NO = record__History.Fixture_M.Fixture_NO,
                                              Version = record__History.Fixture_M.Version,
                                              Fixture_Seq = record__History.Fixture_M.Fixture_Seq,
                                              Fixture_Unique_ID = record__History.Fixture_M.Fixture_Unique_ID,
                                              Fixture_Name = record__History.Fixture_M.Fixture_Name,
                                              Project_UID = record__History.Fixture_M.Project_UID,
                                              Fixture_Machine_UID = record__History.Fixture_M.Fixture_Machine_UID,
                                              Vendor_Info_UID = record__History.Fixture_M.Vendor_Info_UID,
                                              Production_Line_UID = record__History.Fixture_M.Production_Line_UID,
                                              Status = record__History.Fixture_M.Status,
                                              ShortCode = record__History.Fixture_M.ShortCode,
                                              TwoD_Barcode = record__History.Fixture_M.TwoD_Barcode,
                                              Equipment_No = record__History.Fixture_M.Fixture_Machine.Equipment_No,
                                              Machine_Type = record__History.Fixture_M.Fixture_Machine.Machine_Type,
                                              Line_Name = record__History.Fixture_M.Production_Line.Line_Name,
                                              Workshop = record__History.Fixture_M.Production_Line.Workshop.Workshop_Name,
                                              Workshop_UID = record__History.Fixture_M.Production_Line.Workshop_UID,
                                              Workstation = record__History.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                                              Workstation_UID = record__History.Fixture_M.Production_Line.Workstation_UID,
                                              Process_Info = record__History.Fixture_M.Production_Line.Process_Info.Process_Name,
                                              Process_Info_UID = record__History.Fixture_M.Production_Line.Process_Info_UID,
                                              Project = record__History.Fixture_M.System_Project.Project_Name,
                                              WorkStation_ID = record__History.Fixture_M.Production_Line.WorkStation.WorkStation_ID,
                                              Cycle_ALL = record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_ID + "_" + record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Interval + record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Cycle_Unit,
                                              Maintenance_Type = record__History.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                                              //Createder = record.System_Users2.User_Name,
                                              //Modifieder = record.System_Users3.User_Name
                                          });
            query = query.Where(m => Fixture_Maintenance_Record_UIDs.Contains("," + m.Fixture_Maintenance_Record_UID + ","));
            var fixtures = SetListFixtureDTO(query.ToList());
            return fixtures;

        }
        /// <summary>
        /// 根据选定主页面的保养数据弹出对话框时加载的数据列表
        /// </summary>
        /// <param name="Fixture_Maintenance_Record_UIDs"></param>
        /// <param name="straus">1.代表保养，2.代表确认，3.代表取消保养</param>
        /// <returns></returns>
        public IEnumerable<Fixture_Maintenance_RecordDTO> GetFixtureMaintenance(string Fixture_Maintenance_Record_UIDs, int straus)
        {


            Fixture_Maintenance_Record_UIDs = "," + Fixture_Maintenance_Record_UIDs + ",";
            var query = from record in DataContext.Fixture_Maintenance_Record
                        select new Fixture_Maintenance_RecordDTO
                        {
                            Fixture_Maintenance_Record_UID = record.Fixture_Maintenance_Record_UID,
                            Fixture_Maintenance_Profile_UID = record.Fixture_Maintenance_Profile_UID,
                            Maintenance_Record_NO = record.Maintenance_Record_NO,
                            Fixture_M_UID = record.Fixture_M_UID,
                            Maintenance_Date = record.Maintenance_Date,
                            Maintenance_Status = record.Maintenance_Status,
                            Maintenance_Person_Number = record.Maintenance_Person_Number,
                            Maintenance_Person_Name = record.Maintenance_Person_Name,
                            Confirm_Date = record.Confirm_Date,
                            Confirm_Status = record.Confirm_Status,
                            Confirmor_UID = record.Confirmor_UID,
                            Created_UID = record.Created_UID,
                            Created_Date = record.Created_Date,
                            Modified_UID = record.Modified_UID,
                            Modified_Date = record.Modified_Date,
                            Plant_Organization_UID = record.Fixture_Maintenance_Profile.Plant_Organization_UID,
                            BG_Organization_UID = record.Fixture_Maintenance_Profile.BG_Organization_UID,
                            FunPlant_Organization_UID = record.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                            Fixture_NO = record.Fixture_M.Fixture_NO,
                            Version = record.Fixture_M.Version,
                            Fixture_Seq = record.Fixture_M.Fixture_Seq,
                            Fixture_Unique_ID = record.Fixture_M.Fixture_Unique_ID,
                            Fixture_Name = record.Fixture_M.Fixture_Name,
                            //Project_UID = record.Fixture_M.Project_UID,
                            //Fixture_Machine_UID = record.Fixture_M.Fixture_Machine_UID,
                            //Vendor_Info_UID = record.Fixture_M.Vendor_Info_UID,
                            //Production_Line_UID = record.Fixture_M.Production_Line_UID,
                            Status = record.Fixture_M.Status,
                            ShortCode = record.Fixture_M.ShortCode,
                            //TwoD_Barcode = record.Fixture_M.TwoD_Barcode,
                            //Equipment_No = record.Fixture_M.Fixture_Machine.Equipment_No,
                            //Machine_Type = record.Fixture_M.Fixture_Machine.Machine_Type,
                            //Line_Name = record.Fixture_M.Production_Line.Line_Name,
                            //Workshop = record.Fixture_M.Production_Line.Workshop.Workshop_Name,
                            //Workshop_UID = record.Fixture_M.Production_Line.Workshop_UID,
                            //Workstation = record.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                            //Workstation_UID = record.Fixture_M.Production_Line.Workstation_UID,
                            //Process_Info = record.Fixture_M.Production_Line.Process_Info.Process_Name,
                            //Process_Info_UID = record.Fixture_M.Production_Line.Process_Info_UID,
                            //Project = record.Fixture_M.System_Project.Project_Name,
                            Confirmor = record.System_Users1.User_Name,
                            Maintenance_Type = record.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                            WorkStation_ID = record.Fixture_M.Production_Line.WorkStation.WorkStation_ID,

                        };
            query = query.Where(m => Fixture_Maintenance_Record_UIDs.Contains("," + m.Fixture_Maintenance_Record_UID + ","));
            //保养
            if (straus == 1)
            {
                query = query.Where(m => m.Maintenance_Person_Number == null && (m.Maintenance_Status == 0 || m.Maintenance_Status == null));

            }
            //确认
            else if (straus == 2)
            {
                query = query.Where(m => m.Maintenance_Person_Number != null && m.Maintenance_Status == 1 && m.Confirmor_UID == null);

            }
            //取消保养
            else if (straus == 3)
            {
                query = query.Where(m => m.Maintenance_Person_Number == null && (m.Maintenance_Status == 0 || m.Maintenance_Status == null));

            }
            //var fixtures = SetListFixtureDTO(query.ToList());
            var fixtures = query.ToList();
            return fixtures;

        }
        public List<Fixture_Maintenance_RecordDTO> GetFixtureMaintenanceList(Fixture_Maintenance_RecordDTO dto, int straus)
        {
            var query = from record in DataContext.Fixture_Maintenance_Record
                        select new Fixture_Maintenance_RecordDTO
                        {
                            Fixture_Maintenance_Record_UID = record.Fixture_Maintenance_Record_UID,
                            Fixture_Maintenance_Profile_UID = record.Fixture_Maintenance_Profile_UID,
                            Maintenance_Record_NO = record.Maintenance_Record_NO,
                            Fixture_M_UID = record.Fixture_M_UID,
                            Maintenance_Date = record.Maintenance_Date,
                            Maintenance_Status = record.Maintenance_Status,
                            Maintenance_Person_Number = record.Maintenance_Person_Number,
                            Maintenance_Person_Name = record.Maintenance_Person_Name,
                            Confirm_Date = record.Confirm_Date,
                            Confirm_Status = record.Confirm_Status,
                            Confirmor_UID = record.Confirmor_UID,
                            Created_UID = record.Created_UID,
                            Created_Date = record.Created_Date,
                            Modified_UID = record.Modified_UID,
                            Modified_Date = record.Modified_Date,
                            Plant_Organization_UID = record.Fixture_Maintenance_Profile.Plant_Organization_UID,
                            BG_Organization_UID = record.Fixture_Maintenance_Profile.BG_Organization_UID,
                            FunPlant_Organization_UID = record.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                            Fixture_NO = record.Fixture_M.Fixture_NO,
                            Version = record.Fixture_M.Version,
                            Fixture_Seq = record.Fixture_M.Fixture_Seq,
                            Fixture_Unique_ID = record.Fixture_M.Fixture_Unique_ID,
                            Fixture_Name = record.Fixture_M.Fixture_Name,
                            Project_UID = record.Fixture_M.Project_UID,
                            Fixture_Machine_UID = record.Fixture_M.Fixture_Machine_UID,
                            Vendor_Info_UID = record.Fixture_M.Vendor_Info_UID,
                            Production_Line_UID = record.Fixture_M.Production_Line_UID,
                            Status = record.Fixture_M.Status,
                            ShortCode = record.Fixture_M.ShortCode,
                            TwoD_Barcode = record.Fixture_M.TwoD_Barcode,
                            Equipment_No = record.Fixture_M.Fixture_Machine.Equipment_No,
                            Machine_Type = record.Fixture_M.Fixture_Machine.Machine_Type,
                            Line_Name = record.Fixture_M.Production_Line.Line_Name,
                            Workshop = record.Fixture_M.Production_Line.Workshop.Workshop_Name,
                            Workshop_UID = record.Fixture_M.Production_Line.Workshop_UID,
                            Workstation = record.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                            Workstation_UID = record.Fixture_M.Production_Line.Workstation_UID,
                            Process_Info = record.Fixture_M.Production_Line.Process_Info.Process_Name,
                            Process_Info_UID = record.Fixture_M.Production_Line.Process_Info_UID,
                            Project = record.Fixture_M.System_Project.Project_Name,
                            Confirmor = record.System_Users.User_Name,
                            Maintenance_Type = record.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type,
                            WorkStation_ID = record.Fixture_M.Production_Line.WorkStation.WorkStation_ID,

                        };
            if (dto.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == dto.Plant_Organization_UID);
            if (dto.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == dto.BG_Organization_UID);
            if (dto.FunPlant_Organization_UID != null && dto.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == dto.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(dto.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(dto.Fixture_NO));
            if (!string.IsNullOrWhiteSpace(dto.Version))
                query = query.Where(m => m.Version.Contains(dto.Version));
            //if (!string.IsNullOrWhiteSpace(dto.Fixture_Seq))
            //    query = query.Where(m => m.Fixture_Seq.Contains(dto.Fixture_Seq));
            //if (!string.IsNullOrWhiteSpace(dto.Fixture_Unique_ID))
            // query = query.Where(m => m.Fixture_Unique_ID == dto.Fixture_Unique_ID);
            if (!string.IsNullOrWhiteSpace(dto.Fixture_Name))
                query = query.Where(m => m.Fixture_Name.Contains(dto.Fixture_Name));
            //if (dto.Project_UID != null && dto.Project_UID != 0)
            //    query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (dto.Fixture_Machine_UID != null && dto.Fixture_Machine_UID != 0)
                query = query.Where(m => m.Fixture_Machine_UID == dto.Fixture_Machine_UID);
            if (dto.Vendor_Info_UID != null && dto.Vendor_Info_UID != 0)
                query = query.Where(m => m.Vendor_Info_UID == dto.Vendor_Info_UID);
            if (dto.Production_Line_UID != null && dto.Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == dto.Production_Line_UID);
            if (dto.Status != 0)
                query = query.Where(m => m.Status == dto.Status);
            if (!string.IsNullOrWhiteSpace(dto.ShortCode))
                query = query.Where(m => m.ShortCode.Contains(dto.ShortCode));
            //if (!string.IsNullOrWhiteSpace(dto.TwoD_Barcode))
            //    query = query.Where(m => m.TwoD_Barcode.Contains(dto.TwoD_Barcode));
            //if (dto.Created_Date != null)
            //    query = query.Where(m => m.Created_Date == dto.Created_Date);
            if (dto.Modified_UID != 0)
                query = query.Where(m => m.Modified_UID == dto.Modified_UID);
            if (dto.End_Date_From != null)
                query = query.Where(m => m.Created_Date >= dto.End_Date_From);
            if (dto.End_Date_To != null)
                query = query.Where(m => m.Created_Date <= dto.End_Date_To);

            //保养
            if (straus == 1)
            {
                query = query.Where(m => m.Maintenance_Person_Number == null && (m.Maintenance_Status == 0 || m.Maintenance_Status == null));

            }
            //确认
            else if (straus == 2)
            {
                query = query.Where(m => m.Maintenance_Person_Number != null && m.Maintenance_Status == 1 && m.Confirmor_UID == null);

            }
            //取消保养
            else if (straus == 3)
            {
                query = query.Where(m => m.Maintenance_Person_Number == null && (m.Maintenance_Status == 0 || m.Maintenance_Status == null));

            }
            var fixtures = SetListFixtureDTO(query.ToList());
            return fixtures;
        }
        public List<Fixture_Maintenance_Record> GetUpdateFixture_Maintenance_Record(string fixture_Maintenance_Record_UIDs, int straus)
        {
            fixture_Maintenance_Record_UIDs = "," + fixture_Maintenance_Record_UIDs + ",";
            var query = DataContext.Fixture_Maintenance_Record.Where(o => fixture_Maintenance_Record_UIDs.Contains("," + o.Fixture_Maintenance_Record_UID + ","));
            //保养
            if (straus == 1)
            {
                query = query.Where(m => m.Maintenance_Person_Number == null && (m.Maintenance_Status == 0));

            }
            //确认
            else if (straus == 2)
            {
                query = query.Where(m => m.Maintenance_Person_Number != null && m.Maintenance_Status == 1 && m.Confirmor_UID == null);

            }
            //取消保养
            else if (straus == 3)
            {
                query = query.Where(m => m.Maintenance_Person_Number == null && (m.Maintenance_Status == 0));

            }
            return query.ToList();
        }
        public string UpdateFixture_Maintenance_Record(List<Fixture_Maintenance_Record> Fixture_Maintenance_Records, int NTID, string personNumber, string personName, DateTime date, int straus, int CurrentUserID)
        {
            try
            {
                if (Fixture_Maintenance_Records != null && Fixture_Maintenance_Records.Count > 0)
                {
                    using (var trans = DataContext.Database.BeginTransaction())
                    {
                        //全插操作
                        StringBuilder sb = new StringBuilder();
                        foreach (var item in Fixture_Maintenance_Records)
                        {

                            if (straus == 1)
                            {

                                var updateSql = string.Format("UPDATE Fixture_Maintenance_Record SET  Maintenance_Status={0},Maintenance_Person_Number=N'{1}',Maintenance_Person_Name=N'{2}',Maintenance_Date='{3}' WHERE Fixture_Maintenance_Record_UID={4};",
                                                                1,
                                                                personNumber,
                                                                personName,
                                                                date.ToString(FormatConstants.DateTimeFormatString),
                                                                item.Fixture_Maintenance_Record_UID
                                                                );
                                sb.AppendLine(updateSql);

                            }
                            else if (straus == 2)
                            {
                                var updateSql = string.Format("UPDATE Fixture_Maintenance_Record SET  Confirm_Status={0},Confirmor_UID={1},Confirm_Date='{2}' WHERE Fixture_Maintenance_Record_UID={3};",
                                                     1,
                                                     NTID,
                                                     date.ToString(FormatConstants.DateTimeFormatString),
                                                     item.Fixture_Maintenance_Record_UID
                                                     );
                                sb.AppendLine(updateSql);


                                int data_Source = item.Fixture_Maintenance_Profile.Maintenance_Plan.Maintenance_Type.ToUpper() == "D" ? 2 : 3;
                                string Resume_Notes = "";
                                if (data_Source == 2)
                                {
                                    Resume_Notes = "日常保养确认";
                                }
                                else
                                {
                                    Resume_Notes = "周期保养确认";
                                }
                                var insertSql = string.Format("insert into Fixture_Resume (Fixture_M_UID, Data_Source, Resume_Date, Source_UID,Source_NO, Resume_Notes, Modified_UID,Modified_Date) values ({0},'{1}','{2}',{3},N'{4}',N'{5}',{6},'{7}');",
                                                               item.Fixture_M_UID,
                                                               data_Source,
                                                               date.ToString(FormatConstants.DateTimeFormatString),
                                                               item.Fixture_Maintenance_Record_UID,
                                                               item.Maintenance_Record_NO,
                                                               Resume_Notes,
                                                               CurrentUserID,
                                                               DateTime.Now.ToString(FormatConstants.DateTimeFormatString)
                                                               );
                                sb.AppendLine(insertSql);


                            }
                            else if (straus == 3)
                            {
                                var updateSql = string.Format("UPDATE Fixture_Maintenance_Record SET  Maintenance_Status={0},Maintenance_Person_Number='{1}',Maintenance_Person_Name='{2}',Maintenance_Date='{3}' WHERE Fixture_Maintenance_Record_UID={4};",
                                              2,
                                              personNumber,
                                              personName,
                                              date.ToString(FormatConstants.DateTimeFormatString),
                                              item.Fixture_Maintenance_Record_UID
                                              );
                                sb.AppendLine(updateSql);
                            }

                        }
                        string sql = sb.ToString();
                        DataContext.Database.ExecuteSqlCommand(sb.ToString());
                        trans.Commit();
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }


        public List<FixtureMaintenance_PlanDTO> GetFixtureMaintenance_Plan(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string Maintenance_Type)
        {
            var query = from maintenance_Plan in DataContext.Maintenance_Plan
                        select new FixtureMaintenance_PlanDTO
                        {

                            Plant_Organization_UID = maintenance_Plan.Plant_Organization_UID,
                            BG_Organization_UID = maintenance_Plan.BG_Organization_UID,
                            FunPlant_Organization_UID = maintenance_Plan.FunPlant_Organization_UID,
                            Maintenance_Plan_UID = maintenance_Plan.Maintenance_Plan_UID,
                            Maintenance_Type = maintenance_Plan.Maintenance_Type,
                            Cycle_ID = maintenance_Plan.Cycle_ID,
                            Cycle_Interval = maintenance_Plan.Cycle_Interval,
                            Cycle_Unit = maintenance_Plan.Cycle_Unit,
                            Lead_Time = maintenance_Plan.Lead_Time,
                            Start_Date = maintenance_Plan.Start_Date,
                            Tolerance_Time = maintenance_Plan.Tolerance_Time,
                            Last_Execution_Date = maintenance_Plan.Last_Execution_Date,
                            Next_Execution_Date = maintenance_Plan.Next_Execution_Date,
                            Is_Enable = maintenance_Plan.Is_Enable,

                            // Cycle_ALL = maintenance_Plan.Cycle_ID+"_" + maintenance_Plan.Cycle_Unit+"_"+ maintenance_Plan.Maintenance_Type,

                        };
            query = query.Where(m => m.Is_Enable == true);
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(Maintenance_Type))
                query = query.Where(m => m.Maintenance_Type == Maintenance_Type);
            return SetFixtureMaintenance_Plan(query.ToList());

        }

        public List<FixtureMaintenance_PlanDTO> SetFixtureMaintenance_Plan(List<FixtureMaintenance_PlanDTO> FixtureMaintenance_Plans)
        {

            foreach (var item in FixtureMaintenance_Plans)
            {
                if (item.Maintenance_Type.ToUpper() == "D")
                {
                    item.Cycle_ALL = item.Cycle_ID + "_" + item.Cycle_Interval + item.Cycle_Unit + "_" + "日常保养";

                }
                else
                {
                    item.Cycle_ALL = item.Cycle_ID + "_" + item.Cycle_Interval + item.Cycle_Unit + "_" + "周期保养";

                }

            }
            return FixtureMaintenance_Plans;
        }

        /// <summary>
        /// 获取治具报表和日常报表的关系
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public List<Fixture_Maintenance_RecordDTO> GetFixture_Maintenance_RecordDTO(Fixture_Maintenance_RecordDTO dto)
        {
            var query = from fixture_M in DataContext.Fixture_M
                        join profile in DataContext.Fixture_Maintenance_Profile
                          on new
                          {
                              fixture_NO = fixture_M.Fixture_NO,
                              plant_Organization_UID = fixture_M.Plant_Organization_UID,
                              bG_Organization_UID = fixture_M.BG_Organization_UID,
                              funPlant_Organization_UID = fixture_M.FunPlant_Organization_UID
                          }
                          equals new
                          {
                              fixture_NO = profile.Fixture_NO,
                              plant_Organization_UID = profile.Plant_Organization_UID,
                              bG_Organization_UID = profile.BG_Organization_UID,
                              funPlant_Organization_UID = profile.FunPlant_Organization_UID
                          }
                        select new Fixture_Maintenance_RecordDTO
                        {
                            Fixture_M_UID = fixture_M.Fixture_M_UID,
                            Fixture_Maintenance_Profile_UID = profile.Fixture_Maintenance_Profile_UID,
                            Plant_Organization_UID = fixture_M.Plant_Organization_UID,
                            BG_Organization_UID = fixture_M.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture_M.FunPlant_Organization_UID,
                            Fixture_NO = fixture_M.Fixture_NO,
                            Version = fixture_M.Version,
                            Project_UID = fixture_M.Project_UID,
                            Fixture_Machine_UID = fixture_M.Fixture_Machine_UID,
                            Vendor_Info_UID = fixture_M.Vendor_Info_UID,
                            Production_Line_UID = fixture_M.Production_Line_UID,
                            Workshop_UID = fixture_M.Production_Line.Workshop_UID,
                            Workstation_UID = fixture_M.Production_Line.Workstation_UID,
                            Process_Info_UID = fixture_M.Production_Line.Process_Info_UID,
                            Maintenance_Plan_UID = profile.Maintenance_Plan_UID,
                            Maintenance_Type = profile.Maintenance_Plan.Maintenance_Type,
                            Is_Enable = profile.Is_Enable,
                            Status= fixture_M.Status,
                        };

            if (dto.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == dto.Plant_Organization_UID);
            if (dto.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == dto.BG_Organization_UID);
            if (dto.FunPlant_Organization_UID != null && dto.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == dto.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(dto.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(dto.Fixture_NO));
            if (!string.IsNullOrWhiteSpace(dto.Version))
                query = query.Where(m => m.Version.Contains(dto.Version));
            if (dto.Project_UID != null && dto.Project_UID != 0)
                query = query.Where(m => m.Project_UID == dto.Project_UID);
            if (dto.Fixture_Machine_UID != null && dto.Fixture_Machine_UID != 0)
                query = query.Where(m => m.Fixture_Machine_UID == dto.Fixture_Machine_UID);
            if (dto.Vendor_Info_UID != null && dto.Vendor_Info_UID != 0)
                query = query.Where(m => m.Vendor_Info_UID == dto.Vendor_Info_UID);
            if (dto.Production_Line_UID != null && dto.Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == dto.Production_Line_UID);
            if (dto.Workshop_UID != null && dto.Workshop_UID != 0)
                query = query.Where(m => m.Workshop_UID == dto.Workshop_UID);
            if (dto.Workstation_UID != null && dto.Workstation_UID != 0)
                query = query.Where(m => m.Workstation_UID == dto.Workstation_UID);
            if (dto.Process_Info_UID != null && dto.Process_Info_UID != 0)
                query = query.Where(m => m.Process_Info_UID == dto.Process_Info_UID);
            if (dto.Maintenance_Plan_UID != null && dto.Maintenance_Plan_UID != 0)
                query = query.Where(m => m.Maintenance_Plan_UID == dto.Maintenance_Plan_UID);
            query = query.Where(m => m.Maintenance_Type.ToUpper() == "D");

            List<Enumeration> enumerations = DataContext.Enumeration.ToList();
            int Enum_UID = enumerations.FirstOrDefault(o => o.Enum_Type == "Fixture_Status" && o.Enum_Value == "使用中In-PRD").Enum_UID;
            query = query.Where(m => m.Status == Enum_UID);
            query = query.Where(m => m.Is_Enable == true);


            return query.ToList();
        }

        public int GetFixtureCount(Fixture_Maintenance_RecordDTO searchModel)
        {
            var query = from record in DataContext.Fixture_Maintenance_Record
                        select new Fixture_Maintenance_RecordDTO
                        {
                            Fixture_Maintenance_Record_UID = record.Fixture_Maintenance_Record_UID,
                            Fixture_Maintenance_Profile_UID = record.Fixture_Maintenance_Profile_UID,
                            Maintenance_Record_NO = record.Maintenance_Record_NO,
                            Fixture_M_UID = record.Fixture_M_UID,
                            Maintenance_Date = record.Maintenance_Date,
                            Maintenance_Status = record.Maintenance_Status,
                            Maintenance_Person_Number = record.Maintenance_Person_Number,
                            Maintenance_Person_Name = record.Maintenance_Person_Name,
                            Confirm_Date = record.Confirm_Date,
                            Confirm_Status = record.Confirm_Status,
                            Confirmor_UID = record.Confirmor_UID,
                            Created_UID = record.Created_UID,
                            Created_Date = record.Created_Date,
                            Modified_UID = record.Modified_UID,
                            Modified_Date = record.Modified_Date,
                            Plant_Organization_UID = record.Fixture_Maintenance_Profile.Plant_Organization_UID,
                            BG_Organization_UID = record.Fixture_Maintenance_Profile.BG_Organization_UID,
                            FunPlant_Organization_UID = record.Fixture_Maintenance_Profile.FunPlant_Organization_UID,
                            Fixture_NO = record.Fixture_M.Fixture_NO,
                            Version = record.Fixture_M.Version,
                            Fixture_Seq = record.Fixture_M.Fixture_Seq,
                            Fixture_Unique_ID = record.Fixture_M.Fixture_Unique_ID,
                            Fixture_Name = record.Fixture_M.Fixture_Name,
                            Project_UID = record.Fixture_M.Project_UID,
                            Fixture_Machine_UID = record.Fixture_M.Fixture_Machine_UID,
                            Vendor_Info_UID = record.Fixture_M.Vendor_Info_UID,
                            Production_Line_UID = record.Fixture_M.Production_Line_UID,
                            Confirmor = record.System_Users.User_Name,
                            WorkStation_ID = record.Fixture_M.Production_Line.WorkStation.WorkStation_ID,

                        };
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            DateTime nextdate = DateTime.Now.Date.AddDays(1);
            DateTime nowDate = DateTime.Now.Date;
            query = query.Where(m => m.Created_Date >= nowDate);
            query = query.Where(m => m.Created_Date <= nextdate);
            return query.ToList().Select(o => o.Maintenance_Record_NO).Distinct().Count();

        }
        public string CreateFixture_Maintenance_Record(List<Fixture_Maintenance_RecordDTO> recordDTOs, Fixture_Maintenance_RecordDTO dto, string maintenance_Record_NO)
        {
            try
            {
                int counti = 0;
                if (recordDTOs != null && recordDTOs.Count > 0)
                {
                    using (var trans = DataContext.Database.BeginTransaction())
                    {
                        //全插操作
                        StringBuilder sb = new StringBuilder();
                        foreach (var item in recordDTOs)
                        {
                            counti++;
                            var insertSql = string.Format("insert into Fixture_Maintenance_Record (Fixture_Maintenance_Profile_UID, Fixture_M_UID, Maintenance_Record_NO, Created_UID,Modified_UID, Created_Date, Modified_Date) values ({0},{1},'{2}',{3},{4},'{5}','{6}');",
                                item.Fixture_Maintenance_Profile_UID,
                                item.Fixture_M_UID,
                                maintenance_Record_NO.Trim(),
                                dto.Created_UID,
                                dto.Modified_UID,
                                DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                                DateTime.Now.ToString(FormatConstants.DateTimeFormatString)
                                );
                            sb.AppendLine(insertSql);

                        }
                        string sql = sb.ToString();
                        DataContext.Database.ExecuteSqlCommand(sb.ToString());
                        trans.Commit();
                    }
                }
                if (counti > 0)
                {
                    return string.Format(@"本次产生治具保养单编号为：{0}    共产生数据为：{1}条", maintenance_Record_NO, counti);
                }
                else
                {
                    return "本次没有数据产生";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
