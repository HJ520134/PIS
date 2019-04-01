using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Storage_Outbound_MRepository : IRepository<Fixture_Storage_Outbound_M>
    {
        string DeleteBound(int Fixture_Storage_Inbound_UID);
        List<FixturePartInOutBoundInfoDTO> GetByUId(int Storage_In_Out_Bound_UID);
    }
    public class Fixture_Storage_Outbound_MRepository : RepositoryBase<Fixture_Storage_Outbound_M>, IFixture_Storage_Outbound_MRepository
    {
        public Fixture_Storage_Outbound_MRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public string DeleteBound(int Fixture_Storage_Inbound_UID)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    // var sql = string.Format(@"DELETE FROM Fixture_Storage_Inbound WHERE Fixture_Storage_Inbound_UID={0}", Fixture_Storage_Inbound_UID);
                    List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                    FixturePartCreateboundStatuDTO fixtureStatus = fixtureStatuDTOs.FirstOrDefault(o => o.Status == "已删除");
                    var sql = string.Format(@"UPDATE [dbo].[Fixture_Storage_Outbound_M]  SET  [Status_UID] = {0}  WHERE Fixture_Storage_Outbound_M_UID={1}", fixtureStatus.Status_UID, Fixture_Storage_Inbound_UID);
                    DataContext.Database.ExecuteSqlCommand(sql);
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
        public List<FixturePartInOutBoundInfoDTO> GetByUId(int Storage_In_Out_Bound_UID)
        {

            var query1 = from M in DataContext.Fixture_Storage_Outbound_M
                         join bm in DataContext.Fixture_Storage_Outbound_D on
                         M.Fixture_Storage_Outbound_M_UID equals bm.Fixture_Storage_Outbound_M_UID
                         select new FixturePartInOutBoundInfoDTO
                         {
                             Storage_In_Out_Bound_UID = M.Fixture_Storage_Outbound_M_UID,
                             Storage_In_Out_Bound_D_UID = bm.Fixture_Storage_Outbound_D_UID,
                             Storage_In_Out_Bound_Type_UID = M.Fixture_Storage_Outbound_Type_UID,
                             InOut_Type = "出库单",
                             In_Out_Type = M.Enumeration.Enum_Value,
                             Fixture_Part_Order_M_UID = 0,
                             Fixture_Part_Order = "",
                             Issue_NO = "",
                             Fixture_Repair_M_UID = M.Fixture_Repair_M_UID,
                             Fixture_Repair_ID = M.Fixture_Repair_M.Repair_NO,
                             SentOut_Number = M.Fixture_Repair_M.SentOut_Number,
                             SentOut_Name = M.Fixture_Repair_M.SentOut_Name,
                             SentOut_Date = M.Fixture_Repair_M.SentOut_Date,
                             Storage_In_Out_Bound_ID = M.Fixture_Storage_Outbound_ID,
                             Status_UID = M.Status_UID,
                             Fixture_Part_UID = bm.Fixture_Part_UID,
                             Fixture_Warehouse_Storage_UID = bm.Fixture_Warehouse_Storage_UID,
                             In_Out_Bound_Qty = bm.Outbound_Qty,
                             Applicant_UID = M.Applicant_UID,
                             Applicant_Date = M.Applicant_Date,
                             Applicant = M.System_Users1.User_Name,
                             Approve_UID = M.Approve_UID,
                             Approve_Date = M.Approve_Date,
                             Approve = M.System_Users2.User_Name,
                             Outbound_Account_UID = M.Outbound_Account_UID,
                             Outbound_Account = M.System_Users.User_Name,
                             Plant_Organization_UID = bm.Fixture_Warehouse_Storage.Plant_Organization_UID,
                             Plant = bm.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                             BG_Organization_UID = bm.Fixture_Warehouse_Storage.BG_Organization_UID,
                             BG_Organization = bm.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                             FunPlant_Organization_UID = bm.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                             FunPlant_Organization = bm.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                             Storage_ID = bm.Fixture_Warehouse_Storage.Storage_ID,
                             Rack_ID = bm.Fixture_Warehouse_Storage.Rack_ID,
                             Fixture_Warehouse_ID = bm.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                             Fixture_Warehouse_Name = bm.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                             Part_ID = bm.Fixture_Part.Part_ID,
                             Part_Name = bm.Fixture_Part.Part_Name,
                             Part_Spec = bm.Fixture_Part.Part_Spec,
                             Status = M.Enumeration1.Enum_Value,
                             Remarks= M.Remarks
                         };

            query1 = from M in query1
                     group M by new
                     {
                         M.Storage_In_Out_Bound_UID,
                         M.Storage_In_Out_Bound_D_UID,
                         M.Storage_In_Out_Bound_Type_UID,
                         M.InOut_Type,
                         M.In_Out_Type,
                         M.Fixture_Part_Order_M_UID,
                         M.Fixture_Part_Order,
                         M.Issue_NO,
                         M.Fixture_Repair_M_UID,
                         M.Fixture_Repair_ID,
                         M.SentOut_Number,
                         M.SentOut_Name ,
                         M.SentOut_Date,
                         M.Storage_In_Out_Bound_ID,
                         M.Status_UID,
                         M.Applicant_UID,
                         M.Applicant_Date,
                         M.Applicant,
                         M.Approve_UID,
                         M.Approve_Date,
                         M.Approve,
                         M.Outbound_Account_UID,
                         M.Outbound_Account,
                         M.Status,
                         M.Plant_Organization_UID,
                         M.Plant,
                         M.BG_Organization_UID,
                         M.BG_Organization,
                         M.Remarks
                         //M.FunPlant_Organization_UID,
                         //M.FunPlant_Organization,
                     } into g
                     select new FixturePartInOutBoundInfoDTO
                     {
                         Storage_In_Out_Bound_UID = g.Key.Storage_In_Out_Bound_UID,
                         Storage_In_Out_Bound_D_UID = g.Key.Storage_In_Out_Bound_D_UID,
                         Storage_In_Out_Bound_Type_UID = g.Key.Storage_In_Out_Bound_Type_UID,
                         InOut_Type = g.Key.InOut_Type,
                         In_Out_Type = g.Key.In_Out_Type,
                         Fixture_Part_Order_M_UID = g.Key.Fixture_Part_Order_M_UID,
                         Fixture_Part_Order = g.Key.Fixture_Part_Order,
                         Issue_NO = g.Key.Issue_NO,
                         Fixture_Repair_M_UID = g.Key.Fixture_Repair_M_UID,
                         Fixture_Repair_ID = g.Key.Fixture_Repair_ID,
                         SentOut_Number = g.Key.SentOut_Number,
                         SentOut_Name = g.Key.SentOut_Name,
                         SentOut_Date = g.Key.SentOut_Date,
                         Storage_In_Out_Bound_ID = g.Key.Storage_In_Out_Bound_ID,
                         Status_UID = g.Key.Status_UID,
                         Fixture_Part_UID = 0,
                         Fixture_Warehouse_Storage_UID = 0,
                         In_Out_Bound_Qty = 0,
                         Applicant_UID = g.Key.Applicant_UID,
                         Applicant_Date = g.Key.Applicant_Date,
                         Applicant = g.Key.Applicant,
                         Approve_UID = g.Key.Approve_UID,
                         Approve_Date = g.Key.Approve_Date,
                         Approve = g.Key.Approve,
                         Outbound_Account_UID = g.Key.Outbound_Account_UID,
                         Outbound_Account = g.Key.Outbound_Account,
                         Plant_Organization_UID = g.Key.Plant_Organization_UID,
                         Plant = g.Key.Plant,
                         BG_Organization_UID = g.Key.BG_Organization_UID,
                         BG_Organization = g.Key.BG_Organization,
                         FunPlant_Organization_UID = 0,
                         FunPlant_Organization = "",
                         Storage_ID = "",
                         Rack_ID = "",
                         Fixture_Warehouse_ID = "",
                         Fixture_Warehouse_Name = "",
                         Part_ID = "",
                         Part_Name = "",
                         Part_Spec = "",
                         Status = g.Key.Status,
                         Remarks = g.Key.Remarks
                     };

            query1 = query1.Where(m => m.Storage_In_Out_Bound_UID == Storage_In_Out_Bound_UID);
            return query1.ToList();

        }
    }
}
