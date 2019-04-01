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
    public interface IFixture_Storage_TransferRepository : IRepository<Fixture_Storage_Transfer>
    {
        IQueryable<Fixture_Storage_TransferDTO> QueryStorageTransfers(Fixture_Storage_TransferDTO searchModel, Page page, out int totalcount);
        Fixture_Storage_TransferDTO QueryStorageTransferByUid(int Fixture_Storage_Transfer_UID);
        List<Fixture_Storage_TransferDTO> DoExportStorageTransferReprot(string uids);
        List<Fixture_Storage_TransferDTO> DoAllExportStorageTransferReprot(Fixture_Storage_TransferDTO search);
        string InsertStorageTransfer(List<Fixture_Storage_TransferDTO> dtolist);
    }
    public class Fixture_Storage_TransferRepository : RepositoryBase<Fixture_Storage_Transfer>, IFixture_Storage_TransferRepository
    {
        public Fixture_Storage_TransferRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
        public IQueryable<Fixture_Storage_TransferDTO> QueryStorageTransfers(Fixture_Storage_TransferDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Fixture_Storage_Transfer
                        select new Fixture_Storage_TransferDTO
                        {
                            Fixture_Storage_Transfer_UID = M.Fixture_Storage_Transfer_UID,
                            Fixture_Storage_Transfer_ID = M.Fixture_Storage_Transfer_ID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Out_Fixture_Warehouse_Storage_UID = M.Out_Fixture_Warehouse_Storage_UID,
                            In_Fixture_Warehouse_Storage_UID = M.In_Fixture_Warehouse_Storage_UID,
                            Transfer_Qty = M.Transfer_Qty,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Applicant = M.System_Users.User_Name,
                            Status_UID = M.Status_UID,
                            Approver_UID = M.Approver_UID,
                            Approver = M.System_Users1.User_Name,
                            Approver_Date = M.Approver_Date,
                            Out_Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Out_Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Out_Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Out_Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            In_Storage_ID = M.Fixture_Warehouse_Storage1.Storage_ID,
                            In_Rack_ID = M.Fixture_Warehouse_Storage1.Rack_ID,
                            In_Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage1.Fixture_Warehouse.Fixture_Warehouse_ID,
                            In_Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage1.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration.Enum_Value,
                            Plant_Organization_UID = M.Fixture_Part.Plant_Organization_UID,
                            Plant = M.Fixture_Part.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Part.BG_Organization_UID,
                            BG_Organization = M.Fixture_Part.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Part.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Part.System_Organization2.Organization_Name,
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
                query = query.Where(m => m.Part_ID == searchModel.Part_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
                query = query.Where(m => m.Part_Name == searchModel.Part_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
                query = query.Where(m => m.Part_Spec == searchModel.Part_Spec);
            if (!string.IsNullOrWhiteSpace(searchModel.Approver))
                query = query.Where(m => m.Approver == searchModel.Approver);
            if (!string.IsNullOrWhiteSpace(searchModel.Applicant))
                query = query.Where(m => m.Applicant == searchModel.Applicant);
            if (searchModel.Applicant_Date.Year != 1)
            {
                DateTime nextday = searchModel.Applicant_Date.AddDays(1);
                query = query.Where(m => m.Applicant_Date >= searchModel.Applicant_Date & m.Applicant_Date < nextday);
            }
            if (searchModel.Approver_Date.Year != 1)
            {
                DateTime nextday = searchModel.Approver_Date.AddDays(1);
                query = query.Where(m => m.Approver_Date >= searchModel.Approver_Date & m.Applicant_Date < nextday);
            }
            if (searchModel.Out_Fixture_Warehouse_Storage_UID != 0)
                query = query.Where(m => m.Out_Fixture_Warehouse_Storage_UID == searchModel.Out_Fixture_Warehouse_Storage_UID);
            if (searchModel.In_Fixture_Warehouse_Storage_UID != 0)
                query = query.Where(m => m.In_Fixture_Warehouse_Storage_UID == searchModel.In_Fixture_Warehouse_Storage_UID);
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

        public Fixture_Storage_TransferDTO QueryStorageTransferByUid(int Fixture_Storage_Transfer_UID)
        {
            var query = from M in DataContext.Fixture_Storage_Transfer
                        select new Fixture_Storage_TransferDTO
                        {
                            Fixture_Storage_Transfer_UID = M.Fixture_Storage_Transfer_UID,
                            Fixture_Storage_Transfer_ID = M.Fixture_Storage_Transfer_ID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Out_Fixture_Warehouse_Storage_UID = M.Out_Fixture_Warehouse_Storage_UID,
                            In_Fixture_Warehouse_Storage_UID = M.In_Fixture_Warehouse_Storage_UID,
                            Transfer_Qty = M.Transfer_Qty,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Applicant = M.System_Users.User_Name,
                            Status_UID = M.Status_UID,
                            Approver_UID = M.Approver_UID,
                            Approver = M.System_Users1.User_Name,
                            Approver_Date = M.Approver_Date,
                            Out_Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Out_Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Out_Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Out_Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            In_Storage_ID = M.Fixture_Warehouse_Storage1.Storage_ID,
                            In_Rack_ID = M.Fixture_Warehouse_Storage1.Rack_ID,
                            In_Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage1.Fixture_Warehouse.Fixture_Warehouse_ID,
                            In_Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage1.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration.Enum_Value,
                            Plant_Organization_UID = M.Fixture_Part.Plant_Organization_UID,
                            Plant = M.Fixture_Part.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Part.BG_Organization_UID,
                            BG_Organization = M.Fixture_Part.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Part.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Part.System_Organization2.Organization_Name,
                        };
            if (Fixture_Storage_Transfer_UID != 0)
                query = query.Where(m => m.Fixture_Storage_Transfer_UID == Fixture_Storage_Transfer_UID);
            return SetInventory_Qty(query.FirstOrDefault());
        }

        private Fixture_Storage_TransferDTO SetInventory_Qty(Fixture_Storage_TransferDTO fixture_Storage_TransferDTO)
        {

          var fixture_Part_Inventory=   DataContext.Fixture_Part_Inventory.FirstOrDefault(o => o.Fixture_Part_UID == fixture_Storage_TransferDTO.Fixture_Part_UID && o.Fixture_Warehouse_Storage_UID == fixture_Storage_TransferDTO.Out_Fixture_Warehouse_Storage_UID);

            if(fixture_Part_Inventory!=null)
            {
                fixture_Storage_TransferDTO.Inventory_Qty = fixture_Part_Inventory.Inventory_Qty;
            }
            return fixture_Storage_TransferDTO;
        }

        public List<Fixture_Storage_TransferDTO> DoExportStorageTransferReprot(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Fixture_Storage_Transfer
                        select new Fixture_Storage_TransferDTO
                        {
                            Fixture_Storage_Transfer_UID = M.Fixture_Storage_Transfer_UID,
                            Fixture_Storage_Transfer_ID = M.Fixture_Storage_Transfer_ID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Out_Fixture_Warehouse_Storage_UID = M.Out_Fixture_Warehouse_Storage_UID,
                            In_Fixture_Warehouse_Storage_UID = M.In_Fixture_Warehouse_Storage_UID,
                            Transfer_Qty = M.Transfer_Qty,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Applicant = M.System_Users.User_Name,
                            Status_UID = M.Status_UID,
                            Approver_UID = M.Approver_UID,
                            Approver = M.System_Users1.User_Name,
                            Approver_Date = M.Approver_Date,
                            Out_Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Out_Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Out_Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Out_Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            In_Storage_ID = M.Fixture_Warehouse_Storage1.Storage_ID,
                            In_Rack_ID = M.Fixture_Warehouse_Storage1.Rack_ID,
                            In_Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage1.Fixture_Warehouse.Fixture_Warehouse_ID,
                            In_Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage1.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration.Enum_Value,
                            Plant_Organization_UID = M.Fixture_Part.Plant_Organization_UID,
                            Plant = M.Fixture_Part.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Part.BG_Organization_UID,
                            BG_Organization = M.Fixture_Part.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Part.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Part.System_Organization2.Organization_Name,
                        };

            query = query.Where(m => uids.Contains("," + m.Fixture_Storage_Transfer_UID + ","));
            return query.ToList();
        }
        public List<Fixture_Storage_TransferDTO> DoAllExportStorageTransferReprot(Fixture_Storage_TransferDTO searchModel)
        {

            var query = from M in DataContext.Fixture_Storage_Transfer
                        select new Fixture_Storage_TransferDTO
                        {
                            Fixture_Storage_Transfer_UID = M.Fixture_Storage_Transfer_UID,
                            Fixture_Storage_Transfer_ID = M.Fixture_Storage_Transfer_ID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Out_Fixture_Warehouse_Storage_UID = M.Out_Fixture_Warehouse_Storage_UID,
                            In_Fixture_Warehouse_Storage_UID = M.In_Fixture_Warehouse_Storage_UID,
                            Transfer_Qty = M.Transfer_Qty,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Applicant = M.System_Users.User_Name,
                            Status_UID = M.Status_UID,
                            Approver_UID = M.Approver_UID,
                            Approver = M.System_Users1.User_Name,
                            Approver_Date = M.Approver_Date,
                            Out_Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Out_Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Out_Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Out_Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            In_Storage_ID = M.Fixture_Warehouse_Storage1.Storage_ID,
                            In_Rack_ID = M.Fixture_Warehouse_Storage1.Rack_ID,
                            In_Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage1.Fixture_Warehouse.Fixture_Warehouse_ID,
                            In_Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage1.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration.Enum_Value,
                            Plant_Organization_UID = M.Fixture_Part.Plant_Organization_UID,
                            Plant = M.Fixture_Part.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Part.BG_Organization_UID,
                            BG_Organization = M.Fixture_Part.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Part.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Part.System_Organization2.Organization_Name,
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
                query = query.Where(m => m.Part_ID == searchModel.Part_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
                query = query.Where(m => m.Part_Name == searchModel.Part_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
                query = query.Where(m => m.Part_Spec == searchModel.Part_Spec);
            if (!string.IsNullOrWhiteSpace(searchModel.Approver))
                query = query.Where(m => m.Approver == searchModel.Approver);
            if (!string.IsNullOrWhiteSpace(searchModel.Applicant))
                query = query.Where(m => m.Applicant == searchModel.Applicant);
            if (searchModel.Applicant_Date.Year != 1)
            {
                DateTime nextday = searchModel.Applicant_Date.AddDays(1);
                query = query.Where(m => m.Applicant_Date >= searchModel.Applicant_Date & m.Applicant_Date < nextday);
            }
            if (searchModel.Approver_Date.Year != 1)
            {
                DateTime nextday = searchModel.Approver_Date.AddDays(1);
                query = query.Where(m => m.Approver_Date >= searchModel.Approver_Date & m.Applicant_Date < nextday);
            }
            if (searchModel.Out_Fixture_Warehouse_Storage_UID != 0)
                query = query.Where(m => m.Out_Fixture_Warehouse_Storage_UID == searchModel.Out_Fixture_Warehouse_Storage_UID);
            if (searchModel.In_Fixture_Warehouse_Storage_UID != 0)
                query = query.Where(m => m.In_Fixture_Warehouse_Storage_UID == searchModel.In_Fixture_Warehouse_Storage_UID);
            if (searchModel.Status_UID != 0)
                query = query.Where(m => m.Status_UID == searchModel.Status_UID);
            if (searchModel.Status_UID == 0)
            {
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                FixturePartCreateboundStatuDTO fixtureStatus = fixtureStatuDTOs.FirstOrDefault(o => o.Status == "已删除");
                query = query.Where(m => m.Status_UID != fixtureStatus.Status_UID);
            }
            return query.ToList();
        }

        public string InsertStorageTransfer(List<Fixture_Storage_TransferDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        var sql = string.Format(@"INSERT INTO Fixture_Storage_Transfer
                                                       (Fixture_Storage_Transfer_ID
                                                       ,Fixture_Part_UID
                                                       ,Out_Fixture_Warehouse_Storage_UID
                                                       ,In_Fixture_Warehouse_Storage_UID
                                                       ,Transfer_Qty
                                                       ,Applicant_UID
                                                       ,Applicant_Date
                                                       ,Status_UID
                                                       ,Approver_UID
                                                       ,Approver_Date)
                                                 VALUES
                                                           (   '{0}',
                                                                {1},
                                                                {2},
                                                                {3},
                                                                {4},
                                                                {5},
                                                                '{6}',
                                                                {7},
                                                                {8},
                                                                '{9}'
                                                            )",
                        dtolist[i].Fixture_Storage_Transfer_ID,
                        dtolist[i].Fixture_Part_UID,
                        dtolist[i].Out_Fixture_Warehouse_Storage_UID,
                        dtolist[i].In_Fixture_Warehouse_Storage_UID,
                        dtolist[i].Transfer_Qty,
                        dtolist[i].Applicant_UID,                   
                        DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                        dtolist[i].Status_UID,
                        dtolist[i].Approver_UID,
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
