using PDMS.Common.Constants;
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
    public interface IFixture_Storage_CheckRepository : IRepository<Fixture_Storage_Check>
    {
        IQueryable<FixtureStorageCheckDTO> GetInfo(FixtureStorageCheckDTO searchModel, Page page, out int totalcount);
        FixtureStorageCheckDTO GetByUid(int Fixture_Storage_Check_UID);
        List<FixtureStorageCheckDTO> DoAllExportStorageCheckReprot(FixtureStorageCheckDTO searchModel);
        List<FixtureStorageCheckDTO> DoExportStorageCheckReprot(string uids);
        List<FixtureStorageCheckDTO> DownloadStorageCheck(string Part_ID, string Part_Name, string Part_Spec, string Fixture_Warehouse_ID, string Rack_ID, string Storage_ID);
        string InsertStorageCheck(List<FixtureStorageCheckDTO> dtolist);

    }
    public class Fixture_Storage_CheckRepository : RepositoryBase<Fixture_Storage_Check>, IFixture_Storage_CheckRepository
    {
        public Fixture_Storage_CheckRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public IQueryable<FixtureStorageCheckDTO> GetInfo(FixtureStorageCheckDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Fixture_Storage_Check
                        select new FixtureStorageCheckDTO
                        {

                            Fixture_Storage_Check_UID = M.Fixture_Storage_Check_UID,
                            Fixture_Storage_Check_ID = M.Fixture_Storage_Check_ID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Old_Inventory_Qty = M.Old_Inventory_Qty,
                            Check_Qty = M.Check_Qty,
                            Check_Status_UID = M.Check_Status_UID,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Status_UID = M.Status_UID,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Plant_Organization_UID = M.Fixture_Part.Plant_Organization_UID,
                            Plant = M.Fixture_Part.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Part.BG_Organization_UID,
                            BG_Organization = M.Fixture_Part.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Part.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Part.System_Organization2.Organization_Name,
                            Applicant = M.System_Users.User_Name,
                            Approve = M.System_Users1.User_Name,
                            Check_Status = M.Enumeration.Enum_Value,
                            Status = M.Enumeration1.Enum_Value
                        };
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
                query = query.Where(m => m.Part_ID.Contains(searchModel.Part_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
                query = query.Where(m => m.Part_Name.Contains(searchModel.Part_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
                query = query.Where(m => m.Part_Spec.Contains(searchModel.Part_Spec));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_ID))
                query = query.Where(m => m.Fixture_Warehouse_ID.Contains(searchModel.Fixture_Warehouse_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
                query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
                query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Applicant))
                query = query.Where(m => m.Applicant.Contains(searchModel.Applicant));
            if (!string.IsNullOrWhiteSpace(searchModel.Approve))
                query = query.Where(m => m.Approve.Contains(searchModel.Approve));
            if (searchModel.Status_UID != 0)
                query = query.Where(m => m.Status_UID == searchModel.Status_UID);
            if (searchModel.Status_UID == 0)
            {
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                FixturePartCreateboundStatuDTO fixtureStatus = fixtureStatuDTOs.FirstOrDefault(o => o.Status == "已删除");
                query = query.Where(m => m.Status_UID != fixtureStatus.Status_UID);
            }
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Applicant_Date).GetPage(page);
            return query;
        }
        public List<FixturePartCreateboundStatuDTO> GetFixtureStatuDTO()
        {
            List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = new List<FixturePartCreateboundStatuDTO>();
            List<Enumeration> enumerationItems = DataContext.Enumeration.Where(o => o.Enum_Type == "FixturePartCreateboundStatus").ToList();
            foreach (var item in enumerationItems)
            {
                FixturePartCreateboundStatuDTO fixtureStatuDTO = new FixturePartCreateboundStatuDTO();
                fixtureStatuDTO.Status_UID = item.Enum_UID;
                fixtureStatuDTO.Status = item.Enum_Value;
                fixtureStatuDTOs.Add(fixtureStatuDTO);
            }
            return fixtureStatuDTOs;
        }
        public FixtureStorageCheckDTO GetByUid(int Fixture_Storage_Check_UID)
        {
            var query = from M in DataContext.Fixture_Storage_Check
                        select new FixtureStorageCheckDTO
                        {
                            Fixture_Storage_Check_UID = M.Fixture_Storage_Check_UID,
                            Fixture_Storage_Check_ID = M.Fixture_Storage_Check_ID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Old_Inventory_Qty = M.Old_Inventory_Qty,
                            Check_Qty = M.Check_Qty,
                            Check_Status_UID = M.Check_Status_UID,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Status_UID = M.Status_UID,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Plant_Organization_UID = M.Fixture_Part.Plant_Organization_UID,
                            Plant = M.Fixture_Part.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Part.BG_Organization_UID,
                            BG_Organization = M.Fixture_Part.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Part.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Part.System_Organization2.Organization_Name,
                            Applicant = M.System_Users.User_Name,
                            Approve = M.System_Users1.User_Name,
                            Check_Status = M.Enumeration.Enum_Value,
                            Status = M.Enumeration1.Enum_Value
                        };

            if (Fixture_Storage_Check_UID != 0)
                query = query.Where(m => m.Fixture_Storage_Check_UID == Fixture_Storage_Check_UID);
            return query.FirstOrDefault();
        }
        public List<FixtureStorageCheckDTO> DoAllExportStorageCheckReprot(FixtureStorageCheckDTO searchModel)
        {
            var query = from M in DataContext.Fixture_Storage_Check
                        select new FixtureStorageCheckDTO
                        {

                            Fixture_Storage_Check_UID = M.Fixture_Storage_Check_UID,
                            Fixture_Storage_Check_ID = M.Fixture_Storage_Check_ID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Old_Inventory_Qty = M.Old_Inventory_Qty,
                            Check_Qty = M.Check_Qty,
                            Check_Status_UID = M.Check_Status_UID,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Status_UID = M.Status_UID,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Plant_Organization_UID = M.Fixture_Part.Plant_Organization_UID,
                            Plant = M.Fixture_Part.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Part.BG_Organization_UID,
                            BG_Organization = M.Fixture_Part.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Part.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Part.System_Organization2.Organization_Name,
                            Applicant = M.System_Users.User_Name,
                            Approve = M.System_Users1.User_Name,                        
                            Check_Status = M.Enumeration.Enum_Value,
                            Status = M.Enumeration1.Enum_Value
                        };
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
                query = query.Where(m => m.Part_ID.Contains(searchModel.Part_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
                query = query.Where(m => m.Part_Name.Contains(searchModel.Part_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
                query = query.Where(m => m.Part_Spec.Contains(searchModel.Part_Spec));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_ID))
                query = query.Where(m => m.Fixture_Warehouse_ID.Contains(searchModel.Fixture_Warehouse_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
                query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
                query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Applicant))
                query = query.Where(m => m.Applicant.Contains(searchModel.Applicant));
            if (!string.IsNullOrWhiteSpace(searchModel.Approve))
                query = query.Where(m => m.Approve.Contains(searchModel.Approve));
            if (searchModel.Status_UID == 0)
            {
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                FixturePartCreateboundStatuDTO fixtureStatus = fixtureStatuDTOs.FirstOrDefault(o => o.Status == "已删除");
                query = query.Where(m => m.Status_UID != fixtureStatus.Status_UID);
            }
            return query.ToList();
        }
        public List<FixtureStorageCheckDTO> DoExportStorageCheckReprot(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Fixture_Storage_Check
                        select new FixtureStorageCheckDTO
                        {

                            Fixture_Storage_Check_UID = M.Fixture_Storage_Check_UID,
                            Fixture_Storage_Check_ID = M.Fixture_Storage_Check_ID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Old_Inventory_Qty = M.Old_Inventory_Qty,
                            Check_Qty = M.Check_Qty,
                            Check_Status_UID = M.Check_Status_UID,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Status_UID = M.Status_UID,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Plant_Organization_UID = M.Fixture_Part.Plant_Organization_UID,
                            Plant = M.Fixture_Part.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Part.BG_Organization_UID,
                            BG_Organization = M.Fixture_Part.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Part.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Part.System_Organization2.Organization_Name,
                            Applicant = M.System_Users.User_Name,
                            Approve = M.System_Users1.User_Name,
                            Check_Status = M.Enumeration.Enum_Value,
                            Status = M.Enumeration1.Enum_Value
                        };

            query = query.Where(m => uids.Contains("," + m.Fixture_Storage_Check_UID + ","));

            return query.ToList();
        }
        public List<FixtureStorageCheckDTO> DownloadStorageCheck(string Part_ID, string Part_Name, string Part_Spec, string Fixture_Warehouse_ID, string Rack_ID, string Storage_ID)
        {

            var query = from M in DataContext.Fixture_Part_Inventory
                        select new FixtureStorageCheckDTO
                        {
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Plant_Organization_UID = M.Fixture_Part.Plant_Organization_UID,
                            Plant = M.Fixture_Part.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Part.BG_Organization_UID,
                            BG_Organization = M.Fixture_Part.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Part.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Part.System_Organization2.Organization_Name,
                        };

            if (!string.IsNullOrWhiteSpace(Part_ID))
                query = query.Where(m => m.Part_ID.Contains(Part_ID));
            if (!string.IsNullOrWhiteSpace(Part_Name))
                query = query.Where(m => m.Part_Name.Contains(Part_Name));
            if (!string.IsNullOrWhiteSpace(Part_Spec))
                query = query.Where(m => m.Part_Spec.Contains(Part_Spec));
            if (!string.IsNullOrWhiteSpace(Fixture_Warehouse_ID))
                query = query.Where(m => m.Fixture_Warehouse_ID.Contains(Fixture_Warehouse_ID));
            if (!string.IsNullOrWhiteSpace(Rack_ID))
                query = query.Where(m => m.Rack_ID.Contains(Rack_ID));
            if (!string.IsNullOrWhiteSpace(Storage_ID))
                query = query.Where(m => m.Storage_ID.Contains(Storage_ID));

            return query.ToList();
        }
        public string InsertStorageCheck(List<FixtureStorageCheckDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        var sql = string.Format(@"INSERT INTO Fixture_Storage_Check
                                                   (Fixture_Storage_Check_ID
                                                   ,Fixture_Part_UID
                                                   ,Fixture_Warehouse_Storage_UID
                                                   ,Old_Inventory_Qty
                                                   ,Check_Qty 
                                                   ,Check_Status_UID                                                  
                                                   ,Applicant_UID
                                                   ,Applicant_Date
                                                   ,Status_UID
                                                   ,Approve_UID
                                                   ,Approve_Date)
                                             VALUES
                                                   ('{0}'
                                                   ,{1}
                                                   ,{2}
                                                   ,{3}
                                                   ,{4}
                                                   ,{5}
                                                   ,{6}
                                                   ,'{7}'
                                                   ,{8}
                                                   ,{9}
                                                   ,'{10}'
                                                   )",
                        dtolist[i].Fixture_Storage_Check_ID,
                        dtolist[i].Fixture_Part_UID,
                        dtolist[i].Fixture_Warehouse_Storage_UID,
                        dtolist[i].Old_Inventory_Qty,
                        dtolist[i].Check_Qty,
                        dtolist[i].Check_Status_UID,
                        dtolist[i].Applicant_UID,
                        DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                        dtolist[i].Status_UID,
                        dtolist[i].Approve_UID,
                        DateTime.Now.ToString(FormatConstants.DateTimeFormatString));
                        DataContext.Database.ExecuteSqlCommand(sql);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    result = "Error:" + ex.Message;
                }
                return result;
            }


        }
    }
}
