using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Storage_InboundRepository : IRepository<Fixture_Storage_Inbound>
    {
        #region 开账
        IQueryable<FixturePartStorageInboundDTO> GetCreateInfo(FixturePartStorageInboundDTO searchModel, Page page, out int totalcount);
        List<FixturePartCreateboundStatuDTO> GetFixtureStatuDTO();
        List<FixturePartCreateboundStatuDTO> GetFixtureStatuDTO(string Enum_Type);
        string InsertCreateBoundItem(List<FixturePartStorageInboundDTO> dtolist);
        FixturePartStorageInboundDTO QueryInboundByUid(int Fixture_Storage_Inbound_UID);
        string DeleteCreateBound(int Fixture_Storage_Inbound_UID);
        List<FixturePartStorageInboundDTO> DoExportCreateBoundReprot(string uids);
        List<FixturePartStorageInboundDTO> DoAllExportCreateBoundReprot(FixturePartStorageInboundDTO search);
        List<FixturePartStorageInboundDTO> QueryAllStorageInbound(int Plant_Organization_UID);
        #endregion
        #region  出入库维护作业

        IQueryable<FixturePartInOutBoundInfoDTO> GetDetailInfo(FixturePartInOutBoundInfoDTO searchModel, Page page, out int totalcount);
        List<FixturePartInOutBoundInfoDTO> ExportAllOutboundInfo(FixturePartInOutBoundInfoDTO searchModel);
        List<FixturePartInOutBoundInfoDTO> ExportPartOutboundInfo(string uids);
        string InsertInItem(List<FixturePartStorageInboundDTO> dtolist);
        List<FixturePartInOutBoundInfoDTO> GetByUId(int Storage_In_Out_Bound_UID);
        IQueryable<FixtureInOutStorageModel> GetInOutDetialReport(FixtureInOutStorageModel searchModel, Page page, out int totalcount);
        List<FixtureInOutStorageModel> ExportAllInOutDetialReport(FixtureInOutStorageModel searchModel);
        List<FixtureInOutStorageModel> ExportSelectedInOutDetialReport(string uids);

        #endregion
        #region  库存报表查询
        IQueryable<FixturePartStorageReportDTO> GetStorageReportInfo(FixturePartStorageReportDTO searchModel, Page page, out int totalcount);
        List<FixturePartStorageReportDTO> DoSRExportFunction(int plant, int bg, int funplant, string Part_ID, string Part_Name, string Part_Spec, DateTime start_date, DateTime end_date);
        #endregion
    }
    public class Fixture_Storage_InboundRepository : RepositoryBase<Fixture_Storage_Inbound>, IFixture_Storage_InboundRepository
    {
        public Fixture_Storage_InboundRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {



        }
        #region 开账
        public IQueryable<FixturePartStorageInboundDTO> GetCreateInfo(FixturePartStorageInboundDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Fixture_Storage_Inbound
                        select new FixturePartStorageInboundDTO
                        {
                            Fixture_Storage_Inbound_UID = M.Fixture_Storage_Inbound_UID,
                            Fixture_Storage_Inbound_ID = M.Fixture_Storage_Inbound_ID,
                            Fixture_Storage_Inbound_Type_UID = M.Fixture_Storage_Inbound_Type_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_Order_M_UID = M.Fixture_Part_Order_M_UID,
                            Inbound_Qty = M.Inbound_Qty,
                            Inbound_Price = M.Inbound_Price,
                            Issue_NO = M.Issue_NO,
                            Remarks = M.Remarks,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Status_UID = M.Status_UID,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration1.Enum_Value
                        };


            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_ID))
                query = query.Where(m => m.Fixture_Warehouse_ID == searchModel.Fixture_Warehouse_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_Name))
                query = query.Where(m => m.Fixture_Warehouse_Name == searchModel.Fixture_Warehouse_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
                query = query.Where(m => m.Rack_ID == searchModel.Rack_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
                query = query.Where(m => m.Storage_ID == searchModel.Storage_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
                query = query.Where(m => m.Part_ID == searchModel.Part_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
                query = query.Where(m => m.Part_Name == searchModel.Part_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
                query = query.Where(m => m.Part_Spec == searchModel.Part_Spec);
            if (searchModel.Inbound_Qty != 0)
                query = query.Where(m => m.Inbound_Qty == searchModel.Inbound_Qty);

            List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");
            int Status_UID = inbound_Types.Where(o => o.Status == "开账").FirstOrDefault().Status_UID;
            query = query.Where(m => m.Fixture_Storage_Inbound_Type_UID == Status_UID);

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
        public List<FixturePartCreateboundStatuDTO> GetFixtureStatuDTO(string Enum_Type)
        {

            List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = new List<FixturePartCreateboundStatuDTO>();
            List<Enumeration> enumerationItems = DataContext.Enumeration.Where(o => o.Enum_Type == Enum_Type).ToList();
            foreach (var item in enumerationItems)
            {
                FixturePartCreateboundStatuDTO fixtureStatuDTO = new FixturePartCreateboundStatuDTO();
                fixtureStatuDTO.Status_UID = item.Enum_UID;
                fixtureStatuDTO.Status = item.Enum_Value;
                fixtureStatuDTOs.Add(fixtureStatuDTO);
            }
            return fixtureStatuDTOs;
        }
        public string InsertCreateBoundItem(List<FixturePartStorageInboundDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        var sql = string.Format(@"INSERT INTO Fixture_Storage_Inbound
                                                   (
                                                    Fixture_Storage_Inbound_Type_UID
                                                   ,Fixture_Storage_Inbound_ID
                                                   ,Fixture_Part_UID
                                                   ,Fixture_Warehouse_Storage_UID  
                                                   ,Inbound_Qty
                                                   ,Inbound_Price
                                                   ,Issue_NO
                                                   ,Remarks
                                                   ,Applicant_UID
                                                   ,Applicant_Date
                                                   ,Status_UID
                                                   ,Approve_UID
                                                   ,Approve_Date)
                                             VALUES
                                                   (
                                                    {0}
                                                   ,N'{1}'
                                                   ,{2}
                                                   ,{3}         
                                                   ,{4}
                                                   ,{5}
                                                   ,N'{6}'
                                                   ,N'{7}'
                                                   ,{8}
                                                   ,'{9}'
                                                   ,{10}
                                                   ,{11}
                                                   ,'{12}')",
                        dtolist[i].Fixture_Storage_Inbound_Type_UID,
                        dtolist[i].Fixture_Storage_Inbound_ID,
                        dtolist[i].Fixture_Part_UID,
                        dtolist[i].Fixture_Warehouse_Storage_UID,
                        dtolist[i].Inbound_Qty,
                        dtolist[i].Inbound_Price,
                        dtolist[i].Issue_NO,
                        dtolist[i].Remarks,
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
        public FixturePartStorageInboundDTO QueryInboundByUid(int Fixture_Storage_Inbound_UID)
        {
            var query = from M in DataContext.Fixture_Storage_Inbound
                        select new FixturePartStorageInboundDTO
                        {
                            Fixture_Storage_Inbound_UID = M.Fixture_Storage_Inbound_UID,
                            Fixture_Storage_Inbound_ID = M.Fixture_Storage_Inbound_ID,
                            Fixture_Storage_Inbound_Type_UID = M.Fixture_Storage_Inbound_Type_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_Order_M_UID = M.Fixture_Part_Order_M_UID,
                            Inbound_Qty = M.Inbound_Qty,
                            Inbound_Price = M.Inbound_Price,
                            Issue_NO = M.Issue_NO,
                            Remarks = M.Remarks,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Status_UID = M.Status_UID,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration1.Enum_Value
                        };

            if (Fixture_Storage_Inbound_UID != 0)
                query = query.Where(m => m.Fixture_Storage_Inbound_UID == Fixture_Storage_Inbound_UID);
            //List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartIn_out_Type");
            //int Status_UID = inbound_Types.Where(o => o.Status == "开账").FirstOrDefault().Status_UID;
            //            query = query.Where(m => m.Fixture_Storage_Inbound_Type_UID == Status_UID);

            return query.FirstOrDefault();
        }
        public string DeleteCreateBound(int Fixture_Storage_Inbound_UID)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    // var sql = string.Format(@"DELETE FROM Fixture_Storage_Inbound WHERE Fixture_Storage_Inbound_UID={0}", Fixture_Storage_Inbound_UID);
                    List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                    FixturePartCreateboundStatuDTO fixtureStatus = fixtureStatuDTOs.FirstOrDefault(o => o.Status == "已删除");
                    var sql = string.Format(@"UPDATE [dbo].[Fixture_Storage_Inbound]  SET  [Status_UID] = {0}  WHERE Fixture_Storage_Inbound_UID={1}", fixtureStatus.Status_UID, Fixture_Storage_Inbound_UID);
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
        public List<FixturePartStorageInboundDTO> DoExportCreateBoundReprot(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Fixture_Storage_Inbound
                        select new FixturePartStorageInboundDTO
                        {
                            Fixture_Storage_Inbound_UID = M.Fixture_Storage_Inbound_UID,
                            Fixture_Storage_Inbound_ID = M.Fixture_Storage_Inbound_ID,
                            Fixture_Storage_Inbound_Type_UID = M.Fixture_Storage_Inbound_Type_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_Order_M_UID = M.Fixture_Part_Order_M_UID,
                            Inbound_Qty = M.Inbound_Qty,
                            Inbound_Price = M.Inbound_Price,
                            Issue_NO = M.Issue_NO,
                            Remarks = M.Remarks,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Status_UID = M.Status_UID,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration1.Enum_Value
                        };
            query = query.Where(m => uids.Contains("," + m.Fixture_Storage_Inbound_UID + ","));

            return query.ToList();
        }
        public List<FixturePartStorageInboundDTO> DoAllExportCreateBoundReprot(FixturePartStorageInboundDTO search)
        {
            var query = from M in DataContext.Fixture_Storage_Inbound
                        select new FixturePartStorageInboundDTO
                        {
                            Fixture_Storage_Inbound_UID = M.Fixture_Storage_Inbound_UID,
                            Fixture_Storage_Inbound_ID = M.Fixture_Storage_Inbound_ID,
                            Fixture_Storage_Inbound_Type_UID = M.Fixture_Storage_Inbound_Type_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_Order_M_UID = M.Fixture_Part_Order_M_UID,
                            Inbound_Qty = M.Inbound_Qty,
                            Inbound_Price = M.Inbound_Price,
                            Issue_NO = M.Issue_NO,
                            Remarks = M.Remarks,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Status_UID = M.Status_UID,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration1.Enum_Value
                        };


            if (search.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == search.Plant_Organization_UID);
            if (search.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == search.BG_Organization_UID);
            //if (search.FunPlant_Organization_UID != 0 && search.FunPlant_Organization_UID != null)
            //    query = query.Where(m => m.FunPlant_Organization_UID == search.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(search.Fixture_Warehouse_ID))
                query = query.Where(m => m.Fixture_Warehouse_ID == search.Fixture_Warehouse_ID);
            if (!string.IsNullOrWhiteSpace(search.Fixture_Warehouse_Name))
                query = query.Where(m => m.Fixture_Warehouse_Name == search.Fixture_Warehouse_Name);
            if (!string.IsNullOrWhiteSpace(search.Rack_ID))
                query = query.Where(m => m.Rack_ID == search.Rack_ID);
            if (!string.IsNullOrWhiteSpace(search.Storage_ID))
                query = query.Where(m => m.Storage_ID == search.Storage_ID);
            if (!string.IsNullOrWhiteSpace(search.Part_ID))
                query = query.Where(m => m.Part_ID == search.Part_ID);
            if (!string.IsNullOrWhiteSpace(search.Part_Name))
                query = query.Where(m => m.Part_Name == search.Part_Name);
            if (!string.IsNullOrWhiteSpace(search.Part_Spec))
                query = query.Where(m => m.Part_Spec == search.Part_Spec);
            if (search.Inbound_Qty != 0)
                query = query.Where(m => m.Inbound_Qty == search.Inbound_Qty);
            if (search.Status_UID != 0)
                query = query.Where(m => m.Status_UID == search.Status_UID);
            List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");
            int Status_UID = inbound_Types.Where(o => o.Status == "开账").FirstOrDefault().Status_UID;
            query = query.Where(m => m.Fixture_Storage_Inbound_Type_UID == Status_UID);

            if (search.Status_UID == 0)
            {
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                FixturePartCreateboundStatuDTO fixtureStatus = fixtureStatuDTOs.FirstOrDefault(o => o.Status == "已删除");
                query = query.Where(m => m.Status_UID != fixtureStatus.Status_UID);
            }
            return query.ToList();

        }
        public List<FixturePartStorageInboundDTO> QueryAllStorageInbound(int Plant_Organization_UID)
        {

            var query = from M in DataContext.Fixture_Storage_Inbound
                        select new FixturePartStorageInboundDTO
                        {
                            Fixture_Storage_Inbound_UID = M.Fixture_Storage_Inbound_UID,
                            Fixture_Storage_Inbound_ID = M.Fixture_Storage_Inbound_ID,
                            Fixture_Storage_Inbound_Type_UID = M.Fixture_Storage_Inbound_Type_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_Order_M_UID = M.Fixture_Part_Order_M_UID,
                            Inbound_Qty = M.Inbound_Qty,
                            Inbound_Price = M.Inbound_Price,
                            Issue_NO = M.Issue_NO,
                            Remarks = M.Remarks,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Status_UID = M.Status_UID,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration1.Enum_Value
                        };


            //List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
            //FixturePartCreateboundStatuDTO fixtureStatus = fixtureStatuDTOs.FirstOrDefault(o => o.Status == "已删除");
            //query = query.Where(m => m.Status_UID != fixtureStatus.Status_UID);

            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");
            int Status_UID = inbound_Types.Where(o => o.Status == "开账").FirstOrDefault().Status_UID;
            query = query.Where(m => m.Fixture_Storage_Inbound_Type_UID == Status_UID);
            return query.ToList();

        }
        #endregion

        #region  出入库维护作业
        public IQueryable<FixturePartInOutBoundInfoDTO> GetDetailInfo(FixturePartInOutBoundInfoDTO searchModel, Page page, out int totalcount)
        {

            var query = from M in DataContext.Fixture_Storage_Inbound
                        select new FixturePartInOutBoundInfoDTO
                        {
                            Storage_In_Out_Bound_UID = M.Fixture_Storage_Inbound_UID,
                            Storage_In_Out_Bound_D_UID = 0,
                            Storage_In_Out_Bound_Type_UID = M.Fixture_Storage_Inbound_Type_UID,
                            InOut_Type = "入库单",
                            In_Out_Type = M.Enumeration.Enum_Value,
                            Fixture_Part_Order_M_UID = M.Fixture_Part_Order_M_UID,
                            Fixture_Part_Order = "",
                            Issue_NO = M.Issue_NO,
                            Fixture_Repair_M_UID = 0,
                            Fixture_Repair_ID = "",
                            Storage_In_Out_Bound_ID = M.Fixture_Storage_Inbound_ID,
                            Status_UID = M.Status_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            In_Out_Bound_Qty = M.Inbound_Qty,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Applicant = M.System_Users.User_Name,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Approve = M.System_Users1.User_Name,
                            Outbound_Account_UID = 0,
                            Outbound_Account = "",
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration1.Enum_Value
                        };
            List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");
            int Status_UID = inbound_Types.Where(o => o.Status == "入库单").FirstOrDefault().Status_UID;
            query = query.Where(m => m.Storage_In_Out_Bound_Type_UID == Status_UID);

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
                             Fixture_Repair_ID = "",
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
                             Status = M.Enumeration1.Enum_Value
                         };

            query1 = from M in query1
                     group M by new
                     {
                         M.Storage_In_Out_Bound_UID,
                         //M.Storage_In_Out_Bound_D_UID,
                         M.Storage_In_Out_Bound_Type_UID,
                         M.InOut_Type,
                         M.In_Out_Type,
                         M.Fixture_Part_Order_M_UID,
                         M.Fixture_Part_Order,
                         M.Issue_NO,
                         M.Fixture_Repair_M_UID,
                         M.Fixture_Repair_ID,
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
                         M.BG_Organization
                         //,
                         //M.FunPlant_Organization_UID,
                         //M.FunPlant_Organization,
                     } into g
                     select new FixturePartInOutBoundInfoDTO
                     {
                         Storage_In_Out_Bound_UID = g.Key.Storage_In_Out_Bound_UID,
                         Storage_In_Out_Bound_D_UID = 0,
                         Storage_In_Out_Bound_Type_UID = g.Key.Storage_In_Out_Bound_Type_UID,
                         InOut_Type = g.Key.InOut_Type,
                         In_Out_Type = g.Key.In_Out_Type,
                         Fixture_Part_Order_M_UID = g.Key.Fixture_Part_Order_M_UID,
                         Fixture_Part_Order = g.Key.Fixture_Part_Order,
                         Issue_NO = g.Key.Issue_NO,
                         Fixture_Repair_M_UID = g.Key.Fixture_Repair_M_UID,
                         Fixture_Repair_ID = g.Key.Fixture_Repair_ID,
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
                         Status = g.Key.Status
                     };
            query = query.Union(query1);


            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Storage_In_Out_Bound_Type_UID != 0)
                query = query.Where(m => m.Storage_In_Out_Bound_Type_UID == searchModel.Storage_In_Out_Bound_Type_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_In_Out_Bound_ID))
                query = query.Where(m => m.Storage_In_Out_Bound_ID.Contains(searchModel.Storage_In_Out_Bound_ID));
            if (searchModel.Status_UID != 0)
                query = query.Where(m => m.Status_UID == searchModel.Status_UID);
            if (searchModel.Status_UID == 0)
            {
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                FixturePartCreateboundStatuDTO fixtureStatus = fixtureStatuDTOs.FirstOrDefault(o => o.Status == "已删除");
                query = query.Where(m => m.Status_UID != fixtureStatus.Status_UID);
            }
            if (searchModel.Applicant_Date != null)
                query = query.Where(m => m.Applicant_Date >= searchModel.Applicant_Date);
            if (searchModel.Applicant_Date != null)
                query = query.Where(m => m.Approve_Date >= searchModel.Approve_Date);
            if (!string.IsNullOrWhiteSpace(searchModel.Outbound_Account))
                query = query.Where(m => m.Outbound_Account == searchModel.Outbound_Account);
            if (!string.IsNullOrWhiteSpace(searchModel.Applicant))
                query = query.Where(m => m.Applicant == searchModel.Applicant);
            if (!string.IsNullOrWhiteSpace(searchModel.Approve))
                query = query.Where(m => m.Approve == searchModel.Approve);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Part_Order))
                query = query.Where(m => m.Fixture_Part_Order.Contains(searchModel.Fixture_Part_Order));
            if (!string.IsNullOrWhiteSpace(searchModel.Issue_NO))
                query = query.Where(m => m.Issue_NO.Contains(searchModel.Issue_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Repair_ID))
                query = query.Where(m => m.Fixture_Repair_ID.Contains(searchModel.Fixture_Repair_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_ID))
                query = query.Where(m => m.Fixture_Warehouse_ID.Contains(searchModel.Fixture_Warehouse_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_Name))
                query = query.Where(m => m.Fixture_Warehouse_Name.Contains(searchModel.Fixture_Warehouse_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
                query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
                query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
                query = query.Where(m => m.Part_ID.Contains(searchModel.Part_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
                query = query.Where(m => m.Part_Name.Contains(searchModel.Part_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
                query = query.Where(m => m.Part_Spec.Contains(searchModel.Part_Spec));
            if (searchModel.In_Out_Bound_Qty != 0)
            {
                query = query.Where(m => m.In_Out_Bound_Qty == searchModel.In_Out_Bound_Qty);
            }
            query = query.Where(p => p.In_Out_Type != "盘点入库");
            query = query.Where(p => p.In_Out_Type != "盘点出库");
            query = query.Where(p => p.In_Out_Type != "料品移转入库单");
            query = query.Where(p => p.In_Out_Type != "料品移转出库单");
            query = SetSetFixture_Part_Order_ID(query.ToList());
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Applicant_Date).GetPage(page);
            return query;
        }
        public IQueryable<FixturePartInOutBoundInfoDTO> SetSetFixture_Part_Order_ID(List<FixturePartInOutBoundInfoDTO> FixturePartInOutBoundInfoDTOs)
        {

            List<Fixture_Part_Order_M> Fixture_Part_Order_Ms = DataContext.Fixture_Part_Order_M.ToList();
            foreach (var item in FixturePartInOutBoundInfoDTOs)
            {
                if (item.Fixture_Part_Order_M_UID != null && item.Fixture_Part_Order_M_UID !=0)
                {
                    item.Fixture_Part_Order = Fixture_Part_Order_Ms.FirstOrDefault(o => o.Fixture_Part_Order_M_UID == item.Fixture_Part_Order_M_UID).Order_ID;
                }
            }
            return FixturePartInOutBoundInfoDTOs.AsQueryable();
        }

        public List<FixturePartInOutBoundInfoDTO> ExportAllOutboundInfo(FixturePartInOutBoundInfoDTO searchModel)
        {
            var query = from M in DataContext.Fixture_Storage_Inbound
                        select new FixturePartInOutBoundInfoDTO
                        {
                            Storage_In_Out_Bound_UID = M.Fixture_Storage_Inbound_UID,
                            Storage_In_Out_Bound_D_UID = 0,
                            Storage_In_Out_Bound_Type_UID = M.Fixture_Storage_Inbound_Type_UID,
                            InOut_Type = "入库单",
                            In_Out_Type = M.Enumeration.Enum_Value,
                            Fixture_Part_Order_M_UID = M.Fixture_Part_Order_M_UID,
                            Fixture_Part_Order = "",
                            Issue_NO = M.Issue_NO,
                            Fixture_Repair_M_UID = 0,
                            Fixture_Repair_ID = "",
                            Storage_In_Out_Bound_ID = M.Fixture_Storage_Inbound_ID,
                            Status_UID = M.Status_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            In_Out_Bound_Qty = M.Inbound_Qty,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Applicant = M.System_Users.User_Name,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Approve = M.System_Users1.User_Name,
                            Outbound_Account_UID = 0,
                            Outbound_Account = "",
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration1.Enum_Value
                        };
            List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");
            int Status_UID = inbound_Types.Where(o => o.Status == "入库单").FirstOrDefault().Status_UID;
            query = query.Where(m => m.Storage_In_Out_Bound_Type_UID == Status_UID);

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
                             Fixture_Repair_ID = "",
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
                             Status = M.Enumeration1.Enum_Value
                         };

            query1 = from M in query1
                     group M by new
                     {
                         M.Storage_In_Out_Bound_UID,
                         //M.Storage_In_Out_Bound_D_UID,
                         M.Storage_In_Out_Bound_Type_UID,
                         M.InOut_Type,
                         M.In_Out_Type,
                         M.Fixture_Part_Order_M_UID,
                         M.Fixture_Part_Order,
                         M.Issue_NO,
                         M.Fixture_Repair_M_UID,
                         M.Fixture_Repair_ID,
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
                         M.BG_Organization
                         //,
                         //M.FunPlant_Organization_UID,
                         //M.FunPlant_Organization,
                     } into g
                     select new FixturePartInOutBoundInfoDTO
                     {
                         Storage_In_Out_Bound_UID = g.Key.Storage_In_Out_Bound_UID,
                         Storage_In_Out_Bound_D_UID = 0,
                         Storage_In_Out_Bound_Type_UID = g.Key.Storage_In_Out_Bound_Type_UID,
                         InOut_Type = g.Key.InOut_Type,
                         In_Out_Type = g.Key.In_Out_Type,
                         Fixture_Part_Order_M_UID = g.Key.Fixture_Part_Order_M_UID,
                         Fixture_Part_Order = g.Key.Fixture_Part_Order,
                         Issue_NO = g.Key.Issue_NO,
                         Fixture_Repair_M_UID = g.Key.Fixture_Repair_M_UID,
                         Fixture_Repair_ID = g.Key.Fixture_Repair_ID,
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
                         Status = g.Key.Status
                     };
            query = query.Union(query1);


            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Storage_In_Out_Bound_Type_UID != 0)
                query = query.Where(m => m.Storage_In_Out_Bound_Type_UID == searchModel.Storage_In_Out_Bound_Type_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_In_Out_Bound_ID))
                query = query.Where(m => m.Storage_In_Out_Bound_ID.Contains(searchModel.Storage_In_Out_Bound_ID));
            if (searchModel.Status_UID != 0)
                query = query.Where(m => m.Status_UID == searchModel.Status_UID);
            if (searchModel.Status_UID == 0)
            {
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                FixturePartCreateboundStatuDTO fixtureStatus = fixtureStatuDTOs.FirstOrDefault(o => o.Status == "已删除");
                query = query.Where(m => m.Status_UID != fixtureStatus.Status_UID);
            }
            if (searchModel.Applicant_Date != null)
                query = query.Where(m => m.Applicant_Date >= searchModel.Applicant_Date);
            if (searchModel.Applicant_Date != null)
                query = query.Where(m => m.Approve_Date >= searchModel.Approve_Date);
            if (!string.IsNullOrWhiteSpace(searchModel.Outbound_Account))
                query = query.Where(m => m.Outbound_Account == searchModel.Outbound_Account);
            if (!string.IsNullOrWhiteSpace(searchModel.Applicant))
                query = query.Where(m => m.Applicant == searchModel.Applicant);
            if (!string.IsNullOrWhiteSpace(searchModel.Approve))
                query = query.Where(m => m.Approve == searchModel.Approve);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Part_Order))
                query = query.Where(m => m.Fixture_Part_Order.Contains(searchModel.Fixture_Part_Order));
            if (!string.IsNullOrWhiteSpace(searchModel.Issue_NO))
                query = query.Where(m => m.Issue_NO.Contains(searchModel.Issue_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Repair_ID))
                query = query.Where(m => m.Fixture_Repair_ID.Contains(searchModel.Fixture_Repair_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_ID))
                query = query.Where(m => m.Fixture_Warehouse_ID.Contains(searchModel.Fixture_Warehouse_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_Name))
                query = query.Where(m => m.Fixture_Warehouse_Name.Contains(searchModel.Fixture_Warehouse_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
                query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
                query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
                query = query.Where(m => m.Part_ID.Contains(searchModel.Part_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
                query = query.Where(m => m.Part_Name.Contains(searchModel.Part_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
                query = query.Where(m => m.Part_Spec.Contains(searchModel.Part_Spec));
            if (searchModel.In_Out_Bound_Qty != 0)
                query = query.Where(m => m.In_Out_Bound_Qty == searchModel.In_Out_Bound_Qty);
            query = query.Where(p => p.In_Out_Type != "盘点入库");
            query = query.Where(p => p.In_Out_Type != "盘点出库");
            query = query.Where(p => p.In_Out_Type != "料品移转入库单");
            query = query.Where(p => p.In_Out_Type != "料品移转出库单");

            return SetFixture_Part_Order_ID(query.OrderByDescending(p => p.Applicant_Date).ToList());
        }

        public List<FixturePartInOutBoundInfoDTO> ExportPartOutboundInfo(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Fixture_Storage_Inbound
                        select new FixturePartInOutBoundInfoDTO
                        {
                            Storage_In_Out_Bound_UID = M.Fixture_Storage_Inbound_UID,
                            Storage_In_Out_Bound_D_UID = 0,
                            Storage_In_Out_Bound_Type_UID = M.Fixture_Storage_Inbound_Type_UID,
                            InOut_Type = "入库单",
                            In_Out_Type = M.Enumeration.Enum_Value,
                            Fixture_Part_Order_M_UID = M.Fixture_Part_Order_M_UID,
                            Fixture_Part_Order = "",
                            Issue_NO = M.Issue_NO,
                            Fixture_Repair_M_UID = 0,
                            Fixture_Repair_ID = "",
                            Storage_In_Out_Bound_ID = M.Fixture_Storage_Inbound_ID,
                            Status_UID = M.Status_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            In_Out_Bound_Qty = M.Inbound_Qty,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Applicant = M.System_Users.User_Name,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Approve = M.System_Users1.User_Name,
                            Outbound_Account_UID = 0,
                            Outbound_Account = "",
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration1.Enum_Value
                        };
            List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");
            int Status_UID = inbound_Types.Where(o => o.Status == "入库单").FirstOrDefault().Status_UID;
            query = query.Where(m => m.Storage_In_Out_Bound_Type_UID == Status_UID);

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
                             Fixture_Repair_ID = "",
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
                             Status = M.Enumeration1.Enum_Value
                         };

            query1 = from M in query1
                     group M by new
                     {
                         M.Storage_In_Out_Bound_UID,
                         //M.Storage_In_Out_Bound_D_UID,
                         M.Storage_In_Out_Bound_Type_UID,
                         M.InOut_Type,
                         M.In_Out_Type,
                         M.Fixture_Part_Order_M_UID,
                         M.Fixture_Part_Order,
                         M.Issue_NO,
                         M.Fixture_Repair_M_UID,
                         M.Fixture_Repair_ID,
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
                         M.BG_Organization
                         //,
                         //M.FunPlant_Organization_UID,
                         //M.FunPlant_Organization,
                     } into g
                     select new FixturePartInOutBoundInfoDTO
                     {
                         Storage_In_Out_Bound_UID = g.Key.Storage_In_Out_Bound_UID,
                         Storage_In_Out_Bound_D_UID = 0,
                         Storage_In_Out_Bound_Type_UID = g.Key.Storage_In_Out_Bound_Type_UID,
                         InOut_Type = g.Key.InOut_Type,
                         In_Out_Type = g.Key.In_Out_Type,
                         Fixture_Part_Order_M_UID = g.Key.Fixture_Part_Order_M_UID,
                         Fixture_Part_Order = g.Key.Fixture_Part_Order,
                         Issue_NO = g.Key.Issue_NO,
                         Fixture_Repair_M_UID = g.Key.Fixture_Repair_M_UID,
                         Fixture_Repair_ID = g.Key.Fixture_Repair_ID,
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
                         Status = g.Key.Status
                     };
            query = query.Union(query1);
            query = query.Where(m => uids.Contains("," + m.Storage_In_Out_Bound_UID + ","));
            return  SetFixture_Part_Order_ID(query.OrderByDescending(p => p.Applicant_Date).ToList());
        }

        public List<FixturePartInOutBoundInfoDTO> SetFixture_Part_Order_ID(List<FixturePartInOutBoundInfoDTO> FixturePartInOutBoundInfoDTOs)
        {

            List<Fixture_Part_Order_M> Fixture_Part_Order_Ms = DataContext.Fixture_Part_Order_M.ToList();
            foreach (var item in FixturePartInOutBoundInfoDTOs)
            {
                if (item.Fixture_Part_Order_M_UID != null && item.Fixture_Part_Order_M_UID != 0)
                {
                    item.Fixture_Part_Order = Fixture_Part_Order_Ms.FirstOrDefault(o => o.Fixture_Part_Order_M_UID == item.Fixture_Part_Order_M_UID).Order_ID;
                }
            }
            return FixturePartInOutBoundInfoDTOs;
        }


        public string InsertInItem(List<FixturePartStorageInboundDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        var sql = string.Format(@"INSERT INTO Fixture_Storage_Inbound
                                                   (
                                                    Fixture_Storage_Inbound_Type_UID
                                                   ,Fixture_Storage_Inbound_ID
                                                   ,Fixture_Part_UID
                                                   ,Fixture_Warehouse_Storage_UID  
                                                   ,Inbound_Qty
                                                   ,Inbound_Price
                                                   ,Issue_NO
                                                   ,Remarks
                                                   ,Applicant_UID
                                                   ,Applicant_Date
                                                   ,Status_UID
                                                   ,Approve_UID
                                                   ,Approve_Date
                                                   ,Fixture_Part_Order_M_UID)
                                             VALUES
                                                   (
                                                    {0}
                                                   ,N'{1}'
                                                   ,{2}
                                                   ,{3}         
                                                   ,{4}
                                                   ,{5}
                                                   ,N'{6}'
                                                   ,N'{7}'
                                                   ,{8}
                                                   ,'{9}'
                                                   ,{10}
                                                   ,{11}
                                                   ,'{12}'
                                                   ,{13})",
                        dtolist[i].Fixture_Storage_Inbound_Type_UID,
                        dtolist[i].Fixture_Storage_Inbound_ID,
                        dtolist[i].Fixture_Part_UID,
                        dtolist[i].Fixture_Warehouse_Storage_UID,
                        dtolist[i].Inbound_Qty,
                        dtolist[i].Inbound_Price,
                        dtolist[i].Issue_NO,
                        dtolist[i].Remarks,
                        dtolist[i].Applicant_UID,
                        DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                        dtolist[i].Status_UID,
                        dtolist[i].Approve_UID,
                        DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                        dtolist[i].Fixture_Part_Order_M_UID);
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
        public List<FixturePartInOutBoundInfoDTO> GetByUId(int Storage_In_Out_Bound_UID)
        {

            var query = from M in DataContext.Fixture_Storage_Inbound
                        select new FixturePartInOutBoundInfoDTO
                        {
                            Storage_In_Out_Bound_UID = M.Fixture_Storage_Inbound_UID,
                            Storage_In_Out_Bound_D_UID = 0,
                            Storage_In_Out_Bound_Type_UID = M.Fixture_Storage_Inbound_Type_UID,
                            InOut_Type = "入库单",
                            In_Out_Type = M.Enumeration.Enum_Value,
                            Fixture_Part_Order_M_UID = M.Fixture_Part_Order_M_UID,
                            Fixture_Part_Order = "",
                            Issue_NO = M.Issue_NO,
                            Fixture_Repair_M_UID = 0,
                            Fixture_Repair_ID = "",
                            Storage_In_Out_Bound_ID = M.Fixture_Storage_Inbound_ID,
                            Status_UID = M.Status_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            In_Out_Bound_Qty = M.Inbound_Qty,
                            Applicant_UID = M.Applicant_UID,
                            Applicant_Date = M.Applicant_Date,
                            Applicant = M.System_Users.User_Name,
                            Approve_UID = M.Approve_UID,
                            Approve_Date = M.Approve_Date,
                            Approve = M.System_Users1.User_Name,
                            Outbound_Account_UID = 0,
                            Outbound_Account = "",
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            Status = M.Enumeration1.Enum_Value,
                            Price = 0,
                            Qty = 0

                            //Fixture_Part_Order_D_UID= M.p
                        };
            List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");
            int Status_UID = inbound_Types.Where(o => o.Status == "入库单").FirstOrDefault().Status_UID;
            query = query.Where(m => m.Storage_In_Out_Bound_Type_UID == Status_UID);
            query = query.Where(m => m.Storage_In_Out_Bound_UID == Storage_In_Out_Bound_UID);
            // query = query.Where(m => m.InOut_Type == Storage_In_Out_Bound_UID);
            return SetFixture_Part_Order(query.ToList());
        }

        public List<FixturePartInOutBoundInfoDTO> SetFixture_Part_Order(List<FixturePartInOutBoundInfoDTO> FixturePartInOutBoundInfoDTOs)
        {

            List<Fixture_Part_Order_M> Fixture_Part_Order_Ms = DataContext.Fixture_Part_Order_M.ToList();
            List<Fixture_Part_Order_D> Fixture_Part_Order_Ds = DataContext.Fixture_Part_Order_D.ToList();
            foreach (var item in FixturePartInOutBoundInfoDTOs)
            {
                item.Fixture_Part_Order = Fixture_Part_Order_Ms.FirstOrDefault(o => o.Fixture_Part_Order_M_UID == item.Fixture_Part_Order_M_UID).Order_ID;
                item.Price = Fixture_Part_Order_Ds.FirstOrDefault(o => o.Fixture_Part_Order_M_UID == item.Fixture_Part_Order_M_UID && o.Fixture_Part_UID == item.Fixture_Part_UID).Price;
                item.Qty = Fixture_Part_Order_Ds.FirstOrDefault(o => o.Fixture_Part_Order_M_UID == item.Fixture_Part_Order_M_UID && o.Fixture_Part_UID == item.Fixture_Part_UID).Qty;
            }

            return FixturePartInOutBoundInfoDTOs;
        }

        /// <summary>
        /// 获取出入库报表明细信息的数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public IQueryable<FixtureInOutStorageModel> GetInOutDetialReport(FixtureInOutStorageModel searchModel, Page page,
            out int totalcount)
        {
            //入库单
            var query1 = from M in DataContext.Fixture_Storage_InOut_Detail
                         where M.Enumeration.Enum_Type == "FixturePartIn_out_Type"
                         && (M.Enumeration.Enum_Value == "入库单"
                         || M.Enumeration.Enum_Value == "开账"
                         || M.Enumeration.Enum_Value == "盘点入库"
                         || M.Enumeration.Enum_Value == "平盘入库单"
                         || M.Enumeration.Enum_Value == "料品移转入库单")
                         join Inbound in DataContext.Fixture_Storage_Inbound
                         on M.Fixture_Storage_InOut_UID equals Inbound.Fixture_Storage_Inbound_UID
                         select new FixtureInOutStorageModel
                         {
                             Fixture_Storage_InOut_Detail_UID = M.Fixture_Storage_InOut_Detail_UID,
                             Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                             Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                             BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                             BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                             FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                             FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                             Storage_In_Out_Bound_ID = Inbound.Fixture_Storage_Inbound_ID,
                             Storage_In_Out_Bound_Type_UID = M.Enumeration.Enum_UID,
                             InOut_Type = M.Enumeration.Enum_Value,
                             Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                             Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                             Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                             Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                             In_Out_StorageTime = Inbound.Applicant_Date,
                             Remaining_Bound_Qty = M.Balance_Qty,
                             Out_Bound_Qty = 0,
                             In_Bound_Qty = M.InOut_Qty,
                             In_Out_StorageType = "",
                             Part_ID = M.Fixture_Part.Part_ID,
                             Part_Name = M.Fixture_Part.Part_Name,
                             Part_Spec = M.Fixture_Part.Part_Spec,
                             Modified_UID = M.System_Users.Modified_UID,
                             Modified_User_Name = M.System_Users.User_Name,
                             Modified_Date = M.Modified_Date,
                         };

            //出库单
            var query2 = from M in DataContext.Fixture_Storage_InOut_Detail
                         where M.Enumeration.Enum_Type == "FixturePartIn_out_Type"
                               && (M.Enumeration.Enum_Value == "领料单"
                                   || M.Enumeration.Enum_Value == "不良品出库单"
                                   || M.Enumeration.Enum_Value == "盘点出库"
                                   || M.Enumeration.Enum_Value == "领料单"
                                   || M.Enumeration.Enum_Value == "料品移转出库单"
                                   )
                         join Outbound_M in DataContext.Fixture_Storage_Outbound_M
                             on M.Fixture_Storage_InOut_UID equals Outbound_M.Fixture_Storage_Outbound_M_UID
                         join Outbound_D in DataContext.Fixture_Storage_Outbound_D
                         on Outbound_M.Fixture_Storage_Outbound_M_UID equals Outbound_D.Fixture_Storage_Outbound_M_UID
                         select new FixtureInOutStorageModel
                         {
                             Fixture_Storage_InOut_Detail_UID = M.Fixture_Storage_InOut_Detail_UID,
                             Plant_Organization_UID = Outbound_D.Fixture_Warehouse_Storage.Plant_Organization_UID,
                             Plant = Outbound_D.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                             BG_Organization_UID = Outbound_D.Fixture_Warehouse_Storage.BG_Organization_UID,
                             BG_Organization = Outbound_D.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                             FunPlant_Organization_UID = Outbound_D.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                             FunPlant_Organization = Outbound_D.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                             Storage_In_Out_Bound_ID = Outbound_M.Fixture_Storage_Outbound_ID,
                             Storage_In_Out_Bound_Type_UID = M.Enumeration.Enum_UID,
                             InOut_Type = M.Enumeration.Enum_Value,
                             Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                             Rack_ID = Outbound_D.Fixture_Warehouse_Storage.Rack_ID,
                             Fixture_Warehouse_ID = Outbound_D.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                             Fixture_Warehouse_Name = Outbound_D.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                             In_Out_StorageTime = Outbound_M.Applicant_Date,
                             Remaining_Bound_Qty = M.Balance_Qty,
                             Out_Bound_Qty = M.InOut_Qty,
                             In_Bound_Qty = 0,
                             In_Out_StorageType = "",
                             Part_ID = Outbound_D.Fixture_Part.Part_ID,
                             Part_Name = Outbound_D.Fixture_Part.Part_Name,
                             Part_Spec = Outbound_D.Fixture_Part.Part_Spec,

                             Modified_UID = M.System_Users.Modified_UID,
                             Modified_User_Name = M.System_Users.User_Name,
                             Modified_Date = M.Modified_Date,
                         };
            var query = query1.Union(query2);
            //厂区
            if (searchModel.Plant_Organization_UID != 0)
            {
                query = query.Where(p => p.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            }
            //op
            if (searchModel.BG_Organization_UID != 0)
            {
                query = query.Where(p => p.BG_Organization_UID == searchModel.BG_Organization_UID);
            }

            //功能厂
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
            {
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            }
            //出入库类别
            if (searchModel.Storage_In_Out_Bound_Type_UID != 0)
            {
                query = query.Where(m => m.Storage_In_Out_Bound_Type_UID == searchModel.Storage_In_Out_Bound_Type_UID);
            }
            //出入库日期
            if (searchModel.Start_Date != null)
            {
                DateTime Start_Date = new DateTime();
                Start_Date =Convert.ToDateTime(searchModel.Start_Date) ;
                var stratTime = Start_Date.ToString("yyyy-MM-dd 00:00:00.000");
                Start_Date = Convert.ToDateTime(stratTime);
                query = query.Where(m => m.In_Out_StorageTime >= Start_Date);
            }

            if (searchModel.End_Date != null)
            {
                DateTime End_Date = new DateTime();
                End_Date = Convert.ToDateTime(searchModel.End_Date) ;
                var endTime = End_Date.ToString("yyyy-MM-dd 23:59:59.999");
                End_Date = Convert.ToDateTime(endTime);
                query = query.Where(m => m.In_Out_StorageTime <= End_Date);
            }

            //出入库单编号
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_In_Out_Bound_ID))
            {
                query = query.Where(m => m.Storage_In_Out_Bound_ID.Contains(searchModel.Storage_In_Out_Bound_ID));
            }

            //料号
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
            {
                query = query.Where(m => m.Part_ID.Contains(searchModel.Part_ID));
            }
            //品名
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
            {
                query = query.Where(m => m.Part_Name.Contains(searchModel.Part_Name));
            }
            //型号
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
            {
                query = query.Where(m => m.Part_Spec.Contains(searchModel.Part_Spec));
            }

            //储位
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
            {
                query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));
            }

            //料架
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
            {
                query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
            }

            //仓库编码
            if (!string.IsNullOrEmpty(searchModel.Fixture_Warehouse_ID))
            {
                query = query.Where(m => m.Fixture_Warehouse_ID.Contains(searchModel.Fixture_Warehouse_ID));
            }

            //仓库名字
            if (!string.IsNullOrEmpty(searchModel.Fixture_Warehouse_Name))
            {
                query = query.Where(m => m.Fixture_Warehouse_Name.Contains(searchModel.Fixture_Warehouse_Name));
            }

            totalcount = query.Count();
            return query.OrderByDescending(p => p.Modified_Date).GetPage(page);
        }


        /// <summary>
        /// 导出All出入库报表明细信息的数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<FixtureInOutStorageModel> ExportAllInOutDetialReport(FixtureInOutStorageModel searchModel
            )
        {
            //入库单
            var query1 = from M in DataContext.Fixture_Storage_InOut_Detail
                         where M.Enumeration.Enum_Type == "FixturePartIn_out_Type"
                         && (M.Enumeration.Enum_Value == "入库单"
                         || M.Enumeration.Enum_Value == "开账"
                         || M.Enumeration.Enum_Value == "盘点入库"
                         || M.Enumeration.Enum_Value == "平盘入库单"
                         || M.Enumeration.Enum_Value == "料品移转入库单")
                         join Inbound in DataContext.Fixture_Storage_Inbound
                         on M.Fixture_Storage_InOut_UID equals Inbound.Fixture_Storage_Inbound_UID
                         select new FixtureInOutStorageModel
                         {
                             Fixture_Storage_InOut_Detail_UID = M.Fixture_Storage_InOut_Detail_UID,
                             Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                             Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                             BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                             BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                             FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                             FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                             Storage_In_Out_Bound_ID = Inbound.Fixture_Storage_Inbound_ID,
                             Storage_In_Out_Bound_Type_UID = M.Enumeration.Enum_UID,
                             InOut_Type = M.Enumeration.Enum_Value,
                             Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                             Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                             Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                             Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                             In_Out_StorageTime = Inbound.Applicant_Date,
                             Remaining_Bound_Qty = M.Balance_Qty,
                             Out_Bound_Qty = 0,
                             In_Bound_Qty = M.InOut_Qty,
                             In_Out_StorageType = "",
                             Part_ID = M.Fixture_Part.Part_ID,
                             Part_Name = M.Fixture_Part.Part_Name,
                             Part_Spec = M.Fixture_Part.Part_Spec,
                             Modified_UID = M.System_Users.Modified_UID,
                             Modified_User_Name = M.System_Users.User_Name,
                             Modified_Date = M.Modified_Date,
                         };

            //出库单
            var query2 = from M in DataContext.Fixture_Storage_InOut_Detail
                         where M.Enumeration.Enum_Type == "FixturePartIn_out_Type"
                               && (M.Enumeration.Enum_Value == "领料单"
                                   || M.Enumeration.Enum_Value == "不良品出库单"
                                   || M.Enumeration.Enum_Value == "盘点出库"
                                   || M.Enumeration.Enum_Value == "领料单"
                                   || M.Enumeration.Enum_Value == "料品移转出库单"
                                   )
                         join Outbound_M in DataContext.Fixture_Storage_Outbound_M
                             on M.Fixture_Storage_InOut_UID equals Outbound_M.Fixture_Storage_Outbound_M_UID
                         join Outbound_D in DataContext.Fixture_Storage_Outbound_D
                         on Outbound_M.Fixture_Storage_Outbound_M_UID equals Outbound_D.Fixture_Storage_Outbound_M_UID
                         select new FixtureInOutStorageModel
                         {
                             Fixture_Storage_InOut_Detail_UID = M.Fixture_Storage_InOut_Detail_UID,
                             Plant_Organization_UID = Outbound_D.Fixture_Warehouse_Storage.Plant_Organization_UID,
                             Plant = Outbound_D.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                             BG_Organization_UID = Outbound_D.Fixture_Warehouse_Storage.BG_Organization_UID,
                             BG_Organization = Outbound_D.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                             FunPlant_Organization_UID = Outbound_D.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                             FunPlant_Organization = Outbound_D.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                             Storage_In_Out_Bound_ID = Outbound_M.Fixture_Storage_Outbound_ID,
                             Storage_In_Out_Bound_Type_UID = M.Enumeration.Enum_UID,
                             InOut_Type = M.Enumeration.Enum_Value,
                             Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                             Rack_ID = Outbound_D.Fixture_Warehouse_Storage.Rack_ID,
                             Fixture_Warehouse_ID = Outbound_D.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                             Fixture_Warehouse_Name = Outbound_D.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                             In_Out_StorageTime = Outbound_M.Applicant_Date,
                             Remaining_Bound_Qty = M.Balance_Qty,
                             Out_Bound_Qty = M.InOut_Qty,
                             In_Bound_Qty = 0,
                             In_Out_StorageType = "",
                             Part_ID = Outbound_D.Fixture_Part.Part_ID,
                             Part_Name = Outbound_D.Fixture_Part.Part_Name,
                             Part_Spec = Outbound_D.Fixture_Part.Part_Spec,
                             Modified_UID = M.System_Users.Modified_UID,
                             Modified_User_Name = M.System_Users.User_Name,
                             Modified_Date = M.Modified_Date,
                         };
            var query = query1.Union(query2);
            //厂区
            if (searchModel.Plant_Organization_UID != 0)
            {
                query = query.Where(p => p.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            }
            //op
            if (searchModel.BG_Organization_UID != 0)
            {
                query = query.Where(p => p.BG_Organization_UID == searchModel.BG_Organization_UID);
            }

            //功能厂
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
            {
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            }
            //出入库类别
            if (searchModel.Storage_In_Out_Bound_Type_UID != 0)
            {
                query = query.Where(m => m.Storage_In_Out_Bound_Type_UID == searchModel.Storage_In_Out_Bound_Type_UID);
            }
            //出入库日期
            if (searchModel.Start_Date != null)
                query = query.Where(m => m.In_Out_StorageTime >= searchModel.Start_Date);
            if (searchModel.End_Date != null)
                query = query.Where(m => m.In_Out_StorageTime <= searchModel.End_Date);
            //出入库单编号
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_In_Out_Bound_ID))
            {
                query = query.Where(m => m.Storage_In_Out_Bound_ID.Contains(searchModel.Storage_In_Out_Bound_ID));
            }

            //料号
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
            {
                query = query.Where(m => m.Part_ID.Contains(searchModel.Part_ID));
            }
            //品名
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
            {
                query = query.Where(m => m.Part_Name.Contains(searchModel.Part_Name));
            }
            //型号
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
            {
                query = query.Where(m => m.Part_Spec.Contains(searchModel.Part_Spec));
            }

            //储位
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
            {
                query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));
            }

            //料架
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
            {
                query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
            }

            //仓库编码
            if (!string.IsNullOrEmpty(searchModel.Fixture_Warehouse_ID))
            {
                query = query.Where(m => m.Fixture_Warehouse_ID.Contains(searchModel.Fixture_Warehouse_ID));
            }

            //仓库名字
            if (!string.IsNullOrEmpty(searchModel.Fixture_Warehouse_Name))
            {
                query = query.Where(m => m.Fixture_Warehouse_Name.Contains(searchModel.Fixture_Warehouse_Name));
            }

            return query.OrderByDescending(p => p.Modified_Date).ToList();
        }

        /// <summary>
        /// 导出勾选的出入库报表明细信息的数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<FixtureInOutStorageModel> ExportSelectedInOutDetialReport(string uids
            )
        {
            uids = "," + uids + ",";
            //入库单
            var query1 = from M in DataContext.Fixture_Storage_InOut_Detail
                         where M.Enumeration.Enum_Type == "FixturePartIn_out_Type"
                         && (M.Enumeration.Enum_Value == "入库单"
                         || M.Enumeration.Enum_Value == "开账"
                         || M.Enumeration.Enum_Value == "盘点入库"
                         || M.Enumeration.Enum_Value == "平盘入库单"
                         || M.Enumeration.Enum_Value == "料品移转入库单")
                         join Inbound in DataContext.Fixture_Storage_Inbound
                         on M.Fixture_Storage_InOut_UID equals Inbound.Fixture_Storage_Inbound_UID
                         select new FixtureInOutStorageModel
                         {
                             Fixture_Storage_InOut_Detail_UID = M.Fixture_Storage_InOut_Detail_UID,
                             Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                             Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                             BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                             BG_Organization = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                             FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                             FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                             Storage_In_Out_Bound_ID = Inbound.Fixture_Storage_Inbound_ID,
                             Storage_In_Out_Bound_Type_UID = M.Enumeration.Enum_UID,
                             InOut_Type = M.Enumeration.Enum_Value,
                             Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                             Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                             Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                             Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                             In_Out_StorageTime = Inbound.Applicant_Date,
                             Remaining_Bound_Qty = M.Balance_Qty,
                             Out_Bound_Qty = 0,
                             In_Bound_Qty = M.InOut_Qty,
                             In_Out_StorageType = "",
                             Part_ID = M.Fixture_Part.Part_ID,
                             Part_Name = M.Fixture_Part.Part_Name,
                             Part_Spec = M.Fixture_Part.Part_Spec,
                             Modified_UID = M.System_Users.Modified_UID,
                             Modified_User_Name = M.System_Users.User_Name,
                             Modified_Date = M.Modified_Date,
                         };

            //出库单
            var query2 = from M in DataContext.Fixture_Storage_InOut_Detail
                         where M.Enumeration.Enum_Type == "FixturePartIn_out_Type"
                               && (M.Enumeration.Enum_Value == "领料单"
                                   || M.Enumeration.Enum_Value == "不良品出库单"
                                   || M.Enumeration.Enum_Value == "盘点出库"
                                   || M.Enumeration.Enum_Value == "领料单"
                                   || M.Enumeration.Enum_Value == "料品移转出库单"
                                   )
                         join Outbound_M in DataContext.Fixture_Storage_Outbound_M
                             on M.Fixture_Storage_InOut_UID equals Outbound_M.Fixture_Storage_Outbound_M_UID
                         join Outbound_D in DataContext.Fixture_Storage_Outbound_D
                         on Outbound_M.Fixture_Storage_Outbound_M_UID equals Outbound_D.Fixture_Storage_Outbound_M_UID
                         select new FixtureInOutStorageModel
                         {
                             Fixture_Storage_InOut_Detail_UID = M.Fixture_Storage_InOut_Detail_UID,
                             Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                             Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                             BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                             BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                             FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                             FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                             Storage_In_Out_Bound_ID = Outbound_M.Fixture_Storage_Outbound_ID,
                             Storage_In_Out_Bound_Type_UID = M.Enumeration.Enum_UID,
                             InOut_Type = M.Enumeration.Enum_Value,
                             Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                             Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                             Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                             Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                             In_Out_StorageTime = Outbound_M.Applicant_Date,
                             Remaining_Bound_Qty = M.Balance_Qty,
                             Out_Bound_Qty = M.InOut_Qty,
                             In_Bound_Qty = 0,
                             In_Out_StorageType = "",
                             Part_ID = M.Fixture_Part.Part_ID,
                             Part_Name = M.Fixture_Part.Part_Name,
                             Part_Spec = M.Fixture_Part.Part_Spec,
                             Modified_UID = M.System_Users.Modified_UID,
                             Modified_User_Name = M.System_Users.User_Name,
                             Modified_Date = M.Modified_Date,
                         };
            var query = query1.Union(query2);
            query = query.Where(m => uids.Contains("," + m.Fixture_Storage_InOut_Detail_UID + ","));
            return query.OrderByDescending(p => p.Modified_Date).ToList();
        }

        #endregion

        #region  库存报表查询
        public IQueryable<FixturePartStorageReportDTO> GetStorageReportInfo(FixturePartStorageReportDTO searchModel, Page page, out int totalcount)
        {
            //获取厂区的所有所选时间段的出入库数据。
            var query = from M in DataContext.Fixture_Storage_InOut_Detail
                        select new Fixture_Storage_InOut_DetailDTO
                        {
                            Fixture_Storage_InOut_Detail_UID = M.Fixture_Storage_InOut_Detail_UID,
                            Fixture_Storage_InOut_UID = M.Fixture_Storage_InOut_UID,
                            InOut_Type_UID = M.InOut_Type_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            InOut_Date = M.InOut_Date,
                            InOut_Qty = M.InOut_Qty,
                            Balance_Qty = M.Balance_Qty,
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
            //厂区
            if (searchModel.Plant_Organization_UID != 0)
            {
                query = query.Where(p => p.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            }
            //op
            if (searchModel.BG_Organization_UID != 0)
            {
                query = query.Where(p => p.BG_Organization_UID == searchModel.BG_Organization_UID);
            }

            //功能厂
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
            {
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            }
            //if (startDate.Value.ToString() == "0001-01-01")
            //{
            //    startDate.Value = "2000-01-01";
            //}
            //if (endDate.Value.ToString() == "0001-01-01")
            //{
            //    endDate.Value = DateTime.Now.ToString(FormatConstants.DateTimeFormatStringByDate);
            //}
            //出入库日期
            if (searchModel.Start_Date != null && searchModel.Start_Date.Year.ToString() != "1")
                query = query.Where(m => m.InOut_Date >= searchModel.Start_Date);
            if (searchModel.End_Date != null && searchModel.End_Date.Year.ToString() != "1")
                query = query.Where(m => m.InOut_Date <= searchModel.End_Date);

            //料号
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
            {
                query = query.Where(m => m.Part_ID.Contains(searchModel.Part_ID));
            }
            //品名
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
            {
                query = query.Where(m => m.Part_Name.Contains(searchModel.Part_Name));
            }
            //型号
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
            {
                query = query.Where(m => m.Part_Spec.Contains(searchModel.Part_Spec));
            }
            //获取时间段的库存报表
            List<Fixture_Storage_InOut_DetailDTO> Fixture_Storage_InOut_DetailDTOs = query.ToList();
            List<int> OutEnums = DataContext.Enumeration.Where(M => M.Enum_Type == "FixturePartIn_out_Type"
                            && (M.Enum_Value == "领料单"
                            || M.Enum_Value == "不良品出库单"
                            || M.Enum_Value == "盘点出库"
                            || M.Enum_Value == "领料单"
                            || M.Enum_Value == "料品移转出库单")).Select(o => o.Enum_UID).ToList();
            List<int> intEnums = DataContext.Enumeration.Where(M => M.Enum_Type == "FixturePartIn_out_Type"
                         && (M.Enum_Value == "入库单"
                         || M.Enum_Value == "开账"
                         || M.Enum_Value == "盘点入库"
                         || M.Enum_Value == "平盘入库单"
                         || M.Enum_Value == "料品移转入库单")).Select(o => o.Enum_UID).ToList();
            //出库明细
            List<Fixture_Storage_InOut_DetailDTO> outFixture_Storage_InOut_DetailDTOs = Fixture_Storage_InOut_DetailDTOs.Where(o => OutEnums.Contains(o.InOut_Type_UID)).ToList();
            //入库明细
            List<Fixture_Storage_InOut_DetailDTO> inFixture_Storage_InOut_DetailDTOs = Fixture_Storage_InOut_DetailDTOs.Where(o => intEnums.Contains(o.InOut_Type_UID)).ToList();
            //功能厂报表

            var fixture_Storage_InOut_Details = DataContext.Fixture_Storage_InOut_Detail.Where(o => o.InOut_Date <= searchModel.Start_Date).OrderByDescending(p => p.InOut_Date).ToList();

            //   var fixture_Storage_InOut_Details = Fixture_Storage_InOut_DetailDTOs.OrderBy(p => p.InOut_Date).ToList();


            List<FixturePartStorageReportDTO> fixturePartStorageReports = new List<Model.EntityDTO.FixturePartStorageReportDTO>();
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
            {
                var InOut_DetailDTOs = Fixture_Storage_InOut_DetailDTOs.Where((x, i) => Fixture_Storage_InOut_DetailDTOs.FindIndex(z => z.Fixture_Part_UID == x.Fixture_Part_UID && z.FunPlant_Organization_UID == x.FunPlant_Organization_UID) == i).ToList();

                foreach (var item in InOut_DetailDTOs)
                {
                    FixturePartStorageReportDTO StorageReport = new FixturePartStorageReportDTO();
                    StorageReport.Plant_Organization_UID = item.Plant_Organization_UID;
                    StorageReport.Plant = item.Plant;
                    StorageReport.BG_Organization_UID = item.BG_Organization_UID;
                    StorageReport.BG_Organization = item.BG_Organization;
                    StorageReport.FunPlant_Organization_UID = item.FunPlant_Organization_UID;
                    StorageReport.FunPlant_Organization = item.FunPlant_Organization;
                    StorageReport.Part_ID = item.Part_ID;
                    StorageReport.Part_Name = item.Part_Name;
                    StorageReport.Part_Spec = item.Part_Spec;
                    if (fixture_Storage_InOut_Details != null && fixture_Storage_InOut_Details.FirstOrDefault(o => o.Fixture_Part_UID == item.Fixture_Part_UID && o.Fixture_Part.FunPlant_Organization_UID == item.FunPlant_Organization_UID) != null)
                    {

                        StorageReport.Balance_Qty = fixture_Storage_InOut_Details.FirstOrDefault(o => o.Fixture_Part_UID == item.Fixture_Part_UID && o.Fixture_Part.FunPlant_Organization_UID == item.FunPlant_Organization_UID).Balance_Qty;
                    }
                    else
                    {
                        StorageReport.Balance_Qty = 0;
                    }
                    StorageReport.In_Qty = inFixture_Storage_InOut_DetailDTOs.Where(o => o.Fixture_Part_UID == item.Fixture_Part_UID && o.FunPlant_Organization_UID == item.FunPlant_Organization_UID).Select(o => o.InOut_Qty).Sum();
                    StorageReport.Out_Qty = outFixture_Storage_InOut_DetailDTOs.Where(o => o.Fixture_Part_UID == item.Fixture_Part_UID && o.FunPlant_Organization_UID == item.FunPlant_Organization_UID).Select(o => o.InOut_Qty).Sum();
                    StorageReport.Last_Qty = StorageReport.Balance_Qty + StorageReport.In_Qty - StorageReport.Out_Qty;
                    //StorageReport.Last_Qty = StorageReport.Balance_Qty  - StorageReport.Out_Qty;
                    fixturePartStorageReports.Add(StorageReport);
                }

            }
            else
            {
                //OP报表
                var InOut_DetailDTOs = Fixture_Storage_InOut_DetailDTOs.Where((x, i) => Fixture_Storage_InOut_DetailDTOs.FindIndex(z => z.Fixture_Part_UID == x.Fixture_Part_UID) == i).ToList();

                foreach (var item in InOut_DetailDTOs)
                {
                    FixturePartStorageReportDTO StorageReport = new FixturePartStorageReportDTO();
                    StorageReport.Plant_Organization_UID = item.Plant_Organization_UID;
                    StorageReport.Plant = item.Plant;
                    StorageReport.BG_Organization_UID = item.BG_Organization_UID;
                    StorageReport.BG_Organization = item.BG_Organization;
                    StorageReport.FunPlant_Organization_UID = item.FunPlant_Organization_UID;
                    StorageReport.FunPlant_Organization = item.FunPlant_Organization;
                    StorageReport.Part_ID = item.Part_ID;
                    StorageReport.Part_Name = item.Part_Name;
                    StorageReport.Part_Spec = item.Part_Spec;
                    if (fixture_Storage_InOut_Details != null && fixture_Storage_InOut_Details.FirstOrDefault(o => o.Fixture_Part_UID == item.Fixture_Part_UID) != null)
                    {
                        StorageReport.Balance_Qty = fixture_Storage_InOut_Details.FirstOrDefault(o => o.Fixture_Part_UID == item.Fixture_Part_UID).Balance_Qty;
                    }
                    else
                    {
                        StorageReport.Balance_Qty = 0;
                    }
                    StorageReport.In_Qty = inFixture_Storage_InOut_DetailDTOs.Where(o => o.Fixture_Part_UID == item.Fixture_Part_UID).Select(o => o.InOut_Qty).Sum();
                    StorageReport.Out_Qty = outFixture_Storage_InOut_DetailDTOs.Where(o => o.Fixture_Part_UID == item.Fixture_Part_UID).Select(o => o.InOut_Qty).Sum();
                    StorageReport.Last_Qty = StorageReport.Balance_Qty + StorageReport.In_Qty - StorageReport.Out_Qty;
                    //StorageReport.Last_Qty = StorageReport.Balance_Qty - StorageReport.Out_Qty;
                    fixturePartStorageReports.Add(StorageReport);
                }

            }
            var query1 = fixturePartStorageReports.AsQueryable();
            //if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
            //{
            //    query1 = query1.Where(m => m.Part_ID.Contains(searchModel.Part_ID));
            //}
            totalcount = query1.Count();
            return query1.GetPage(page);
        }
        public List<FixturePartStorageReportDTO> DoSRExportFunction(int plant, int bg, int funplant, string Part_ID, string Part_Name, string Part_Spec, DateTime start_date, DateTime end_date)
        {
            //获取厂区的所有所选时间段的出入库数据。
            var query = from M in DataContext.Fixture_Storage_InOut_Detail
                        select new Fixture_Storage_InOut_DetailDTO
                        {
                            Fixture_Storage_InOut_Detail_UID = M.Fixture_Storage_InOut_Detail_UID,
                            Fixture_Storage_InOut_UID = M.Fixture_Storage_InOut_UID,
                            InOut_Type_UID = M.InOut_Type_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            InOut_Date = M.InOut_Date,
                            InOut_Qty = M.InOut_Qty,
                            Balance_Qty = M.Balance_Qty,
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
            //厂区
            if (plant != 0)
            {
                query = query.Where(p => p.Plant_Organization_UID == plant);
            }
            //op
            if (bg != 0)
            {
                query = query.Where(p => p.BG_Organization_UID == bg);
            }

            //功能厂
            if (funplant != 0)
            {
                query = query.Where(m => m.FunPlant_Organization_UID == funplant);
            }

            //出入库日期
            if (start_date != null && start_date.Year.ToString() != "1")
                query = query.Where(m => m.InOut_Date >= start_date);
            if (end_date != null && end_date.Year.ToString() != "1")
                query = query.Where(m => m.InOut_Date <= end_date);

            //料号
            if (!string.IsNullOrWhiteSpace(Part_ID))
            {
                query = query.Where(m => m.Part_ID.Contains(Part_ID));
            }
            //品名
            if (!string.IsNullOrWhiteSpace(Part_Name))
            {
                query = query.Where(m => m.Part_Name.Contains(Part_Name));
            }
            //型号
            if (!string.IsNullOrWhiteSpace(Part_Spec))
            {
                query = query.Where(m => m.Part_Spec.Contains(Part_Spec));
            }
            //获取时间段的库存报表
            List<Fixture_Storage_InOut_DetailDTO> Fixture_Storage_InOut_DetailDTOs = query.ToList();
            List<int> OutEnums = DataContext.Enumeration.Where(M => M.Enum_Type == "FixturePartIn_out_Type"
                            && (M.Enum_Value == "领料单"
                            || M.Enum_Value == "不良品出库单"
                            || M.Enum_Value == "盘点出库"
                            || M.Enum_Value == "领料单"
                            || M.Enum_Value == "料品移转出库单")).Select(o => o.Enum_UID).ToList();
            List<int> intEnums = DataContext.Enumeration.Where(M => M.Enum_Type == "FixturePartIn_out_Type"
                         && (M.Enum_Value == "入库单"
                         || M.Enum_Value == "开账"
                         || M.Enum_Value == "盘点入库"
                         || M.Enum_Value == "平盘入库单"
                         || M.Enum_Value == "料品移转入库单")).Select(o => o.Enum_UID).ToList();
            //出库明细
            List<Fixture_Storage_InOut_DetailDTO> outFixture_Storage_InOut_DetailDTOs = Fixture_Storage_InOut_DetailDTOs.Where(o => OutEnums.Contains(o.InOut_Type_UID)).ToList();
            //入库明细
            List<Fixture_Storage_InOut_DetailDTO> inFixture_Storage_InOut_DetailDTOs = Fixture_Storage_InOut_DetailDTOs.Where(o => intEnums.Contains(o.InOut_Type_UID)).ToList();
            //功能厂报表

            var fixture_Storage_InOut_Details = DataContext.Fixture_Storage_InOut_Detail.Where(o => o.InOut_Date <= start_date).OrderByDescending(p => p.InOut_Date).ToList();

            List<FixturePartStorageReportDTO> fixturePartStorageReports = new List<Model.EntityDTO.FixturePartStorageReportDTO>();
            if (funplant != 0)
            {
                var InOut_DetailDTOs = Fixture_Storage_InOut_DetailDTOs.Where((x, i) => Fixture_Storage_InOut_DetailDTOs.FindIndex(z => z.Fixture_Part_UID == x.Fixture_Part_UID && z.FunPlant_Organization_UID == x.FunPlant_Organization_UID) == i).ToList();

                foreach (var item in InOut_DetailDTOs)
                {
                    FixturePartStorageReportDTO StorageReport = new FixturePartStorageReportDTO();
                    StorageReport.Plant_Organization_UID = item.Plant_Organization_UID;
                    StorageReport.Plant = item.Plant;
                    StorageReport.BG_Organization_UID = item.BG_Organization_UID;
                    StorageReport.BG_Organization = item.BG_Organization;
                    StorageReport.FunPlant_Organization_UID = item.FunPlant_Organization_UID;
                    StorageReport.FunPlant_Organization = item.FunPlant_Organization;
                    StorageReport.Part_ID = item.Part_ID;
                    StorageReport.Part_Name = item.Part_Name;
                    StorageReport.Part_Spec = item.Part_Spec;
                    if (fixture_Storage_InOut_Details != null && fixture_Storage_InOut_Details.FirstOrDefault(o => o.Fixture_Part_UID == item.Fixture_Part_UID && o.Fixture_Part.FunPlant_Organization_UID == item.FunPlant_Organization_UID) != null)
                    {

                        StorageReport.Balance_Qty = fixture_Storage_InOut_Details.FirstOrDefault(o => o.Fixture_Part_UID == item.Fixture_Part_UID && o.Fixture_Part.FunPlant_Organization_UID == item.FunPlant_Organization_UID).Balance_Qty;
                    }
                    else
                    {
                        StorageReport.Balance_Qty = 0;
                    }
                    StorageReport.In_Qty = inFixture_Storage_InOut_DetailDTOs.Where(o => o.Fixture_Part_UID == item.Fixture_Part_UID && o.FunPlant_Organization_UID == item.FunPlant_Organization_UID).Select(o => o.InOut_Qty).Sum();
                    StorageReport.Out_Qty = outFixture_Storage_InOut_DetailDTOs.Where(o => o.Fixture_Part_UID == item.Fixture_Part_UID && o.FunPlant_Organization_UID == item.FunPlant_Organization_UID).Select(o => o.InOut_Qty).Sum();
                    StorageReport.Last_Qty = StorageReport.Balance_Qty + StorageReport.In_Qty - StorageReport.Out_Qty;
                    fixturePartStorageReports.Add(StorageReport);
                }

            }
            else
            {
                //OP报表
                var InOut_DetailDTOs = Fixture_Storage_InOut_DetailDTOs.Where((x, i) => Fixture_Storage_InOut_DetailDTOs.FindIndex(z => z.Fixture_Part_UID == x.Fixture_Part_UID) == i).ToList();

                foreach (var item in InOut_DetailDTOs)
                {
                    FixturePartStorageReportDTO StorageReport = new FixturePartStorageReportDTO();
                    StorageReport.Plant_Organization_UID = item.Plant_Organization_UID;
                    StorageReport.Plant = item.Plant;
                    StorageReport.BG_Organization_UID = item.BG_Organization_UID;
                    StorageReport.BG_Organization = item.BG_Organization;
                    StorageReport.FunPlant_Organization_UID = item.FunPlant_Organization_UID;
                    StorageReport.FunPlant_Organization = item.FunPlant_Organization;
                    StorageReport.Part_ID = item.Part_ID;
                    StorageReport.Part_Name = item.Part_Name;
                    StorageReport.Part_Spec = item.Part_Spec;
                    if (fixture_Storage_InOut_Details != null && fixture_Storage_InOut_Details.FirstOrDefault(o => o.Fixture_Part_UID == item.Fixture_Part_UID) != null)
                    {
                        StorageReport.Balance_Qty = fixture_Storage_InOut_Details.FirstOrDefault(o => o.Fixture_Part_UID == item.Fixture_Part_UID).Balance_Qty;
                    }
                    else
                    {
                        StorageReport.Balance_Qty = 0;
                    }
                    StorageReport.In_Qty = inFixture_Storage_InOut_DetailDTOs.Where(o => o.Fixture_Part_UID == item.Fixture_Part_UID).Select(o => o.InOut_Qty).Sum();
                    StorageReport.Out_Qty = outFixture_Storage_InOut_DetailDTOs.Where(o => o.Fixture_Part_UID == item.Fixture_Part_UID).Select(o => o.InOut_Qty).Sum();
                    StorageReport.Last_Qty = StorageReport.Balance_Qty + StorageReport.In_Qty - StorageReport.Out_Qty;
                    fixturePartStorageReports.Add(StorageReport);
                }

            }
            return fixturePartStorageReports;

        }
        #endregion
    }
}
