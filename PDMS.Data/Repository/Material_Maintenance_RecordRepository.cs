using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{

    public interface IMaterial_Maintenance_RecordRepository : IRepository<Material_Maintenance_Record>
    {
        IQueryable<Material_Maintenance_RecordDTO> QueryMaterial_Maintenance_Records(Material_Maintenance_RecordDTO searchModel, Page page, out int totalcount);
        Material_Maintenance_RecordDTO QueryMaterial_Maintenance_RecordByUid(int Material_Maintenance_Record_UID);
        string DeleteMaterial_Maintenance_Record(int Material_Maintenance_Record_UID);
        List<Material_Maintenance_RecordDTO> DoAllExportMaterial_Maintenance_Record(Material_Maintenance_RecordDTO search);
        List<Material_Maintenance_RecordDTO> DoExportMaterial_Maintenance_Record(string Material_Maintenance_Record_UIDs);
    }
    public class Material_Maintenance_RecordRepository : RepositoryBase<Material_Maintenance_Record>, IMaterial_Maintenance_RecordRepository
    {
        public Material_Maintenance_RecordRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

        public IQueryable<Material_Maintenance_RecordDTO> QueryMaterial_Maintenance_Records(Material_Maintenance_RecordDTO searchModel, Page page, out int totalcount)
        {
            var query = from material in DataContext.Material_Maintenance_Record
                        select new Material_Maintenance_RecordDTO
                        {

                            Material_Maintenance_Record_UID = material.Material_Maintenance_Record_UID,
                            Plant_Organization_UID = material.Plant_Organization_UID,
                            BG_Organization_UID = material.BG_Organization_UID,
                            FunPlant_Organization_UID = material.FunPlant_Organization_UID,
                            Material_UID = material.Material_UID,
                            Material_Seq = material.Material_Seq,
                            Is_Warranty = material.Is_Warranty,
                            Submit_UID = material.Submit_UID,
                            Submit_Date = material.Submit_Date,
                            Judge_UID = material.Judge_UID,
                            Judge_Date = material.Judge_Date,
                            Abnormal = material.Abnormal,
                            Delivery_Date = material.Delivery_Date,
                            Expected_Return_Date = material.Expected_Return_Date,
                            Acceptance_Staff_UID = material.Acceptance_Staff_UID,
                            Acceptance_Date = material.Acceptance_Date,
                            Acceptance_Results = material.Acceptance_Results,
                            Vendor = material.Vendor,
                            Maintenance_Items = material.Maintenance_Items,
                            Maintenance_Fees = material.Maintenance_Fees,
                            Maintenance_Days = material.Maintenance_Days,
                            Notes = material.Notes,
                            Status_UID = material.Status_UID,
                            Return_Date = material.Return_Date,
                            Return_UID = material.Return_UID,
                            Return_Time = material.Return_Time,
                            Modified_UID = material.Modified_UID,
                            Modified_Date = material.Modified_Date,
                            Plant_Organization_Name = material.System_Organization.Organization_Name,
                            BG_Organization_Name = material.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = material.System_Organization2.Organization_Name,
                            Submiter = material.System_Users.User_Name,
                            Judger = material.System_Users1.User_Name,
                            Acceptancer = material.System_Users2.User_Name,
                            Returner = material.System_Users3.User_Name,
                            Modifieder = material.System_Users4.User_Name,
                            //Status = material.System_Organization2.Organization_Name,
                            Material_ID = material.Material_Info.Material_Id,
                            Expected_Return_Days = SqlFunctions.DateDiff("DAY", material.Delivery_Date, material.Expected_Return_Date) == null ? 0 : (int)SqlFunctions.DateDiff("DAY", material.Delivery_Date, material.Expected_Return_Date),

                        };
            if (searchModel.Plant_Organization_UID != 0)
            {
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            }
            if (searchModel.BG_Organization_UID != 0)
            {
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            }
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
            {
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            }
            if (searchModel.Status_UID != 0)
            {
                query = query.Where(m => m.Status_UID == searchModel.Status_UID);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Material_ID))
            {
                query = query.Where(m => m.Material_ID.ToLower().Contains(searchModel.Material_ID.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Seq))
            {
                query = query.Where(m => m.Material_Seq.ToLower().Contains(searchModel.Material_Seq.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Abnormal))
            {
                query = query.Where(m => m.Abnormal.ToLower().Contains(searchModel.Abnormal.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Vendor))
            {
                query = query.Where(m => m.Vendor.ToLower().Contains(searchModel.Vendor.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Maintenance_Items))
            {
                query = query.Where(m => m.Maintenance_Items.ToLower().Contains(searchModel.Maintenance_Items.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Notes))
            {
                query = query.Where(m => m.Notes.ToLower().Contains(searchModel.Notes.ToLower()));
            }

            if (searchModel.Maintenance_Days != null && searchModel.Maintenance_Days != 0)
            {
                // Maintenance_Days = material.Maintenance_Days,
                query = query.Where(m => m.Maintenance_Days >= searchModel.Maintenance_Days);
            }
            if (searchModel.Maintenance_Fees != null)
            {
                //Maintenance_Fees = material.Maintenance_Fees,   
                query = query.Where(m => m.Maintenance_Fees >= searchModel.Maintenance_Fees);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Submiter))
            {
                query = query.Where(m => m.Submiter.Contains(searchModel.Submiter));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Judger))
            {
                query = query.Where(m => m.Judger.Contains(searchModel.Judger));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Acceptancer))
            {
                query = query.Where(m => m.Acceptancer.Contains(searchModel.Acceptancer));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Returner))
            {
                query = query.Where(m => m.Submiter.Contains(searchModel.Returner));
            }
            //   开始维修天数
            //StartMaintenance_Days 
            if (searchModel.StartMaintenance_Days != null && searchModel.StartMaintenance_Days != 0)
            {
                query = query.Where(m => m.Maintenance_Days >= searchModel.StartMaintenance_Days);
            }
            //结束维修天数
            //EndMaintenance_Days
            if (searchModel.EndMaintenance_Days != null && searchModel.EndMaintenance_Days != 0)
            {
                query = query.Where(m => m.Maintenance_Days <= searchModel.EndMaintenance_Days);
            }

            //   开始维修天数
            //StartMaintenance_Days 
            if (searchModel.StartExpected_Return_Days != null && searchModel.StartExpected_Return_Days != 0)
            {
                query = query.Where(m => m.Expected_Return_Days >= searchModel.StartExpected_Return_Days);
            }
            //结束维修天数
            //EndMaintenance_Days
            if (searchModel.EndExpected_Return_Days != null && searchModel.EndExpected_Return_Days != 0)
            {
                query = query.Where(m => m.Expected_Return_Days <= searchModel.EndExpected_Return_Days);
            }

            //预计天数
            //Expected_Return_Days 
            //预计开始维修天数     
            //StartExpected_Return_Days
            //预计结束始维修天数
            //EndExpected_Return_Days     
            if (searchModel.StartDelivery_Date != null)
                query = query.Where(m => m.Delivery_Date >= searchModel.StartDelivery_Date);
            if (searchModel.EndDelivery_Date != null)
            {
                DateTime nextTime = searchModel.EndDelivery_Date.Value.Date.AddDays(1);
                query = query.Where(m => m.Delivery_Date < nextTime);
            }
            //送修开始时间
            //StartDelivery_Date     
            //送修结束时间       
            //EndDelivery_Date
            if (searchModel.StartReturn_Date != null)
                query = query.Where(m => m.Return_Date >= searchModel.StartReturn_Date);
            if (searchModel.EndReturn_Date != null)
            {
                DateTime nextTime = searchModel.EndReturn_Date.Value.Date.AddDays(1);
                query = query.Where(m => m.Return_Date < nextTime);
            }
            //送修回厂时间
            //StartReturn_Date
            //送修回厂时间
            //EndReturn_Date
            if (searchModel.StartExpected_Return_Date != null)
                query = query.Where(m => m.Expected_Return_Date >= searchModel.StartExpected_Return_Date);
            if (searchModel.EndExpected_Return_Date != null)
            {
                DateTime nextTime = searchModel.EndExpected_Return_Date.Value.Date.AddDays(1);
                query = query.Where(m => m.Expected_Return_Date < nextTime);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Notes))
            {
                query = query.Where(m => m.Notes.Contains(searchModel.Notes));
            }
            //预计开始维修时间
            //StartExpected_Return_Date 
            //预计结束维修时间      
            //EndExpected_Return_Date         
            if (searchModel.Is_Warranty != null)
            {
                query = query.Where(m => m.Is_Warranty == searchModel.Is_Warranty);
            }
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Material_Maintenance_Record_UID).GetPage(page);
            return query;
        }

        public Material_Maintenance_RecordDTO QueryMaterial_Maintenance_RecordByUid(int Material_Maintenance_Record_UID)
        {
            var query = from material in DataContext.Material_Maintenance_Record
                        select new Material_Maintenance_RecordDTO
                        {

                            Material_Maintenance_Record_UID = material.Material_Maintenance_Record_UID,
                            Plant_Organization_UID = material.Plant_Organization_UID,
                            BG_Organization_UID = material.BG_Organization_UID,
                            FunPlant_Organization_UID = material.FunPlant_Organization_UID,
                            Material_UID = material.Material_UID,
                            Material_Seq = material.Material_Seq,
                            Is_Warranty = material.Is_Warranty,
                            Submit_UID = material.Submit_UID,
                            Submit_Date = material.Submit_Date,
                            Judge_UID = material.Judge_UID,
                            Judge_Date = material.Judge_Date,
                            Abnormal = material.Abnormal,
                            Delivery_Date = material.Delivery_Date,
                            Expected_Return_Date = material.Expected_Return_Date,
                            Acceptance_Staff_UID = material.Acceptance_Staff_UID,
                            Acceptance_Date = material.Acceptance_Date,
                            Acceptance_Results = material.Acceptance_Results,
                            Vendor = material.Vendor,
                            Maintenance_Items = material.Maintenance_Items,
                            Maintenance_Fees = material.Maintenance_Fees,
                            Maintenance_Days = material.Maintenance_Days,
                            Notes = material.Notes,
                            Status_UID = material.Status_UID,
                            Return_Date = material.Return_Date,
                            Return_UID = material.Return_UID,
                            Return_Time = material.Return_Time,
                            Modified_UID = material.Modified_UID,
                            Modified_Date = material.Modified_Date,
                            Plant_Organization_Name = material.System_Organization.Organization_Name,
                            BG_Organization_Name = material.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = material.System_Organization2.Organization_Name,
                            Submiter = material.System_Users.User_Name,
                            Judger = material.System_Users1.User_Name,
                            Acceptancer = material.System_Users2.User_Name,
                            Returner = material.System_Users3.User_Name,
                            Modifieder = material.System_Users4.User_Name,
                            Status = material.System_Organization2.Organization_Name,
                            Material_ID = material.Material_Info.Material_Id
                        };
            query = query.Where(m => m.Material_Maintenance_Record_UID == Material_Maintenance_Record_UID);
            return query.FirstOrDefault();
        }

        public string DeleteMaterial_Maintenance_Record(int Material_Maintenance_Record_UID)
        {

            try
            {
                string sql = "delete Material_Maintenance_Record where Material_Maintenance_Record_UID={0}";
                sql = string.Format(sql, Material_Maintenance_Record_UID);
                int result = DataContext.Database.ExecuteSqlCommand(sql);
                if (result == 1)
                    return "";
                else
                    return "删除设备备品记录失败！";
            }
            catch (Exception e)
            {
                return "删除设备备品记录失败！";
            }

        }

        public List<Material_Maintenance_RecordDTO> DoAllExportMaterial_Maintenance_Record(Material_Maintenance_RecordDTO searchModel)
        {

            var query = from material in DataContext.Material_Maintenance_Record
                        select new Material_Maintenance_RecordDTO
                        {

                            Material_Maintenance_Record_UID = material.Material_Maintenance_Record_UID,
                            Plant_Organization_UID = material.Plant_Organization_UID,
                            BG_Organization_UID = material.BG_Organization_UID,
                            FunPlant_Organization_UID = material.FunPlant_Organization_UID,
                            Material_UID = material.Material_UID,
                            Material_Seq = material.Material_Seq,
                            Is_Warranty = material.Is_Warranty,
                            Submit_UID = material.Submit_UID,
                            Submit_Date = material.Submit_Date,
                            Judge_UID = material.Judge_UID,
                            Judge_Date = material.Judge_Date,
                            Abnormal = material.Abnormal,
                            Delivery_Date = material.Delivery_Date,
                            Expected_Return_Date = material.Expected_Return_Date,
                            Acceptance_Staff_UID = material.Acceptance_Staff_UID,
                            Acceptance_Date = material.Acceptance_Date,
                            Acceptance_Results = material.Acceptance_Results,
                            Vendor = material.Vendor,
                            Maintenance_Items = material.Maintenance_Items,
                            Maintenance_Fees = material.Maintenance_Fees,
                            Maintenance_Days = material.Maintenance_Days,
                            Notes = material.Notes,
                            Status_UID = material.Status_UID,
                            Return_Date = material.Return_Date,
                            Return_UID = material.Return_UID,
                            Return_Time = material.Return_Time,
                            Modified_UID = material.Modified_UID,
                            Modified_Date = material.Modified_Date,
                            Plant_Organization_Name = material.System_Organization.Organization_Name,
                            BG_Organization_Name = material.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = material.System_Organization2.Organization_Name,
                            Submiter = material.System_Users.User_Name,
                            Judger = material.System_Users1.User_Name,
                            Acceptancer = material.System_Users2.User_Name,
                            Returner = material.System_Users3.User_Name,
                            Modifieder = material.System_Users4.User_Name,
                            //Status = material.System_Organization2.Organization_Name,
                            Material_ID = material.Material_Info.Material_Id,
                            Expected_Return_Days = SqlFunctions.DateDiff("DAY", material.Delivery_Date, material.Expected_Return_Date) == null ? 0 : (int)SqlFunctions.DateDiff("DAY", material.Delivery_Date, material.Expected_Return_Date),

                        };
            if (searchModel.Plant_Organization_UID != 0)
            {
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            }
            if (searchModel.BG_Organization_UID != 0)
            {
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            }
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
            {
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            }
            if (searchModel.Status_UID != 0)
            {
                query = query.Where(m => m.Status_UID == searchModel.Status_UID);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Material_ID))
            {
                query = query.Where(m => m.Material_ID.ToLower().Contains(searchModel.Material_ID.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Seq))
            {
                query = query.Where(m => m.Material_Seq.ToLower().Contains(searchModel.Material_Seq.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Abnormal))
            {
                query = query.Where(m => m.Abnormal.ToLower().Contains(searchModel.Abnormal.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Vendor))
            {
                query = query.Where(m => m.Vendor.ToLower().Contains(searchModel.Vendor.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Maintenance_Items))
            {
                query = query.Where(m => m.Maintenance_Items.ToLower().Contains(searchModel.Maintenance_Items.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Notes))
            {
                query = query.Where(m => m.Notes.ToLower().Contains(searchModel.Notes.ToLower()));
            }

            if (searchModel.Maintenance_Days != null && searchModel.Maintenance_Days != 0)
            {
                // Maintenance_Days = material.Maintenance_Days,
                query = query.Where(m => m.Maintenance_Days >= searchModel.Maintenance_Days);
            }
            if (searchModel.Maintenance_Fees != null)
            {
                //Maintenance_Fees = material.Maintenance_Fees,   
                query = query.Where(m => m.Maintenance_Fees >= searchModel.Maintenance_Fees);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Submiter))
            {
                query = query.Where(m => m.Submiter.Contains(searchModel.Submiter));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Judger))
            {
                query = query.Where(m => m.Judger.Contains(searchModel.Judger));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Acceptancer))
            {
                query = query.Where(m => m.Acceptancer.Contains(searchModel.Acceptancer));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Returner))
            {
                query = query.Where(m => m.Submiter.Contains(searchModel.Returner));
            }
            //   开始维修天数
            //StartMaintenance_Days 
            if (searchModel.StartMaintenance_Days != null && searchModel.StartMaintenance_Days != 0)
            {
                query = query.Where(m => m.Maintenance_Days >= searchModel.StartMaintenance_Days);
            }
            //结束维修天数
            //EndMaintenance_Days
            if (searchModel.EndMaintenance_Days != null && searchModel.EndMaintenance_Days != 0)
            {
                query = query.Where(m => m.Maintenance_Days <= searchModel.EndMaintenance_Days);
            }

            //   开始维修天数
            //StartMaintenance_Days 
            if (searchModel.StartExpected_Return_Days != null && searchModel.StartExpected_Return_Days != 0)
            {
                query = query.Where(m => m.Expected_Return_Days >= searchModel.StartExpected_Return_Days);
            }
            //结束维修天数
            //EndMaintenance_Days
            if (searchModel.EndExpected_Return_Days != null && searchModel.EndExpected_Return_Days != 0)
            {
                query = query.Where(m => m.Expected_Return_Days <= searchModel.EndExpected_Return_Days);
            }

            //预计天数
            //Expected_Return_Days 
            //预计开始维修天数     
            //StartExpected_Return_Days
            //预计结束始维修天数
            //EndExpected_Return_Days     
            if (searchModel.StartDelivery_Date != null)
                query = query.Where(m => m.Delivery_Date >= searchModel.StartDelivery_Date);
            if (searchModel.EndDelivery_Date != null)
            {
                DateTime nextTime = searchModel.EndDelivery_Date.Value.Date.AddDays(1);
                query = query.Where(m => m.Delivery_Date < nextTime);
            }
            //送修开始时间
            //StartDelivery_Date     
            //送修结束时间       
            //EndDelivery_Date
            if (searchModel.StartReturn_Date != null)
                query = query.Where(m => m.Return_Date >= searchModel.StartReturn_Date);
            if (searchModel.EndReturn_Date != null)
            {
                DateTime nextTime = searchModel.EndReturn_Date.Value.Date.AddDays(1);
                query = query.Where(m => m.Return_Date < nextTime);
            }
            //送修回厂时间
            //StartReturn_Date
            //送修回厂时间
            //EndReturn_Date
            if (searchModel.StartExpected_Return_Date != null)
                query = query.Where(m => m.Expected_Return_Date >= searchModel.StartExpected_Return_Date);
            if (searchModel.EndExpected_Return_Date != null)
            {
                DateTime nextTime = searchModel.EndExpected_Return_Date.Value.Date.AddDays(1);
                query = query.Where(m => m.Expected_Return_Date < nextTime);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Notes))
            {
                query = query.Where(m => m.Notes.Contains(searchModel.Notes));
            }
            //预计开始维修时间
            //StartExpected_Return_Date 
            //预计结束维修时间      
            //EndExpected_Return_Date         
            if (searchModel.Is_Warranty != null)
            {
                query = query.Where(m => m.Is_Warranty == searchModel.Is_Warranty);
            }

            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Material_Maintenance_Record_UID);
            return query.ToList();


        }
        public List<Material_Maintenance_RecordDTO> DoExportMaterial_Maintenance_Record(string Material_Maintenance_Record_UIDs)
        {

            Material_Maintenance_Record_UIDs = "," + Material_Maintenance_Record_UIDs + ",";
            var query = from material in DataContext.Material_Maintenance_Record
                        select new Material_Maintenance_RecordDTO
                        {

                            Material_Maintenance_Record_UID = material.Material_Maintenance_Record_UID,
                            Plant_Organization_UID = material.Plant_Organization_UID,
                            BG_Organization_UID = material.BG_Organization_UID,
                            FunPlant_Organization_UID = material.FunPlant_Organization_UID,
                            Material_UID = material.Material_UID,
                            Material_Seq = material.Material_Seq,
                            Is_Warranty = material.Is_Warranty,
                            Submit_UID = material.Submit_UID,
                            Submit_Date = material.Submit_Date,
                            Judge_UID = material.Judge_UID,
                            Judge_Date = material.Judge_Date,
                            Abnormal = material.Abnormal,
                            Delivery_Date = material.Delivery_Date,
                            Expected_Return_Date = material.Expected_Return_Date,
                            Acceptance_Staff_UID = material.Acceptance_Staff_UID,
                            Acceptance_Date = material.Acceptance_Date,
                            Acceptance_Results = material.Acceptance_Results,
                            Vendor = material.Vendor,
                            Maintenance_Items = material.Maintenance_Items,
                            Maintenance_Fees = material.Maintenance_Fees,
                            Maintenance_Days = material.Maintenance_Days,
                            Notes = material.Notes,
                            Status_UID = material.Status_UID,
                            Return_Date = material.Return_Date,
                            Return_UID = material.Return_UID,
                            Return_Time = material.Return_Time,
                            Modified_UID = material.Modified_UID,
                            Modified_Date = material.Modified_Date,
                            Plant_Organization_Name = material.System_Organization.Organization_Name,
                            BG_Organization_Name = material.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = material.System_Organization2.Organization_Name,
                            Submiter = material.System_Users.User_Name,
                            Judger = material.System_Users1.User_Name,
                            Acceptancer = material.System_Users2.User_Name,
                            Returner = material.System_Users3.User_Name,
                            Modifieder = material.System_Users4.User_Name,
                            //Status = material.System_Organization2.Organization_Name,
                            Material_ID = material.Material_Info.Material_Id,
                            Expected_Return_Days = SqlFunctions.DateDiff("DAY", material.Delivery_Date, material.Expected_Return_Date) == null ? 0 : (int)SqlFunctions.DateDiff("DAY", material.Delivery_Date, material.Expected_Return_Date),


                        };


            query = query.Where(m => Material_Maintenance_Record_UIDs.Contains("," + m.Material_Maintenance_Record_UID + ","));

            return query.ToList();

        }
    }

}
