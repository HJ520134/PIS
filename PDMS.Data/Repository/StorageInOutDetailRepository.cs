using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using PDMS.Common.Constants;

namespace PDMS.Data.Repository
{
    public interface  IStorageInOutDetailRepository: IRepository<Storage_InOut_Detail>
    { 
        IQueryable<StorageDetailDTO> GetInfo(StorageSearchMod searchModel, Page page, out int totalcount);
        IQueryable<StorageReportDTO> GetStorageReportInfo(StorageReportDTO searchModel, Page page, out int totalcount);
        List<StorageReportDTO> DoSRExportFunction(int plant, int bg, int funplant, string material, DateTime start_date, DateTime end_date);
        List<StorageDetailDTO> DoExportStorageDetailReprot(string uids);
        List<StorageDetailDTO> DoAllExportStorageDetailReprot(StorageSearchMod searchModel);


    }
    public class StorageInOutDetailRepository : RepositoryBase<Storage_InOut_Detail>, IStorageInOutDetailRepository
    {
        public StorageInOutDetailRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public List<StorageDetailDTO> DoExportStorageDetailReprot(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Storage_InOut_Detail
                        join inbound in DataContext.Storage_Inbound on
                        M.Storage_InOut_UID equals inbound.Storage_Inbound_UID
                        join mat in DataContext.Material_Info on
                        inbound.Material_UID equals mat.Material_Uid
                        join warst in DataContext.Warehouse_Storage on
                        inbound.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        join war in DataContext.Warehouse on
                        warst.Warehouse_UID equals war.Warehouse_UID
                        join bgorg in DataContext.System_Organization on
                        war.BG_Organization_UID equals bgorg.Organization_UID
                        join funorg in DataContext.System_Organization on
                        war.FunPlant_Organization_UID equals funorg.Organization_UID
                        join typeenum in DataContext.Enumeration on
                        inbound.PartType_UID equals typeenum.Enum_UID
                        join inouttype in DataContext.Enumeration on
                        inbound.Storage_Inbound_Type_UID equals inouttype.Enum_UID
                        join users in DataContext.System_Users on
                        M.Modified_UID equals users.Account_UID
                        select new StorageDetailDTO
                        {
                            Storage_InOut_Detaill_UID = M.Storage_InOut_Detail_UID,
                            BG_Name = bgorg.Organization_Name,
                            Funplant = funorg.Organization_Name,
                            Storage_Bound_ID = inbound.Storage_Inbound_ID,
                            Inout_Type = inouttype.Enum_Value,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Warehouse_ID = war.Warehouse_ID,
                            Rack_ID = warst.Rack_ID,
                            Storage_ID = warst.Storage_ID,
                            InOut_Date = M.InOut_Date,
                            InOut_QTY = M.InOut_Qty,
                            Balance_Qty = M.Balance_Qty,
                            Bound_Type = typeenum.Enum_Value,
                            ModifiedUser = users.User_Name,
                            Modified_Date = M.Modified_Date,
                            BG_Organization_UID = war.BG_Organization_UID,
                            FunPlant_Organization_UID = war.FunPlant_Organization_UID,
                            InOut_Type_UID = M.InOut_Type_UID,
                            Warehouse_Storage_UID = warst.Warehouse_Storage_UID
                        };
            List<int> inlist = new List<int>();
            //fky2017/11/13
            //inlist.Add(379);
            inlist.Add(411);
            //fky2017/11/13
            //inlist.Add(402);
            inlist.Add(427);
            //fky2017/11/13
            //inlist.Add(386);
            inlist.Add(415);
            //fky2017/11/13
            //inlist.Add(403);
            inlist.Add(428);
            //fky2017/11/13
            //inlist.Add(399);
            inlist.Add(424);
            query = query.Where(m => inlist.Contains(m.InOut_Type_UID));
            var query1 = from M in DataContext.Storage_InOut_Detail
                         join outboundd in DataContext.Storage_Outbound_D on
                         M.Storage_InOut_UID equals outboundd.Storage_Outbound_D_UID
                         join outboundm in DataContext.Storage_Outbound_M on
                         outboundd.Storage_Outbound_M_UID equals outboundm.Storage_Outbound_M_UID
                         join mat in DataContext.Material_Info on
                         M.Material_UID equals mat.Material_Uid
                         join warst in DataContext.Warehouse_Storage on
                         M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                         join war in DataContext.Warehouse on
                         warst.Warehouse_UID equals war.Warehouse_UID
                         join bgorg in DataContext.System_Organization on
                         war.BG_Organization_UID equals bgorg.Organization_UID
                         join funorg in DataContext.System_Organization on
                         war.FunPlant_Organization_UID equals funorg.Organization_UID
                         join typeenum in DataContext.Enumeration on
                         outboundd.PartType_UID equals typeenum.Enum_UID
                         join inouttype in DataContext.Enumeration on
                         M.InOut_Type_UID equals inouttype.Enum_UID
                         join users in DataContext.System_Users on
                         M.Modified_UID equals users.Account_UID
                         select new StorageDetailDTO
                         {
                             Storage_InOut_Detaill_UID=M.Storage_InOut_Detail_UID,
                             BG_Name = bgorg.Organization_Name,
                             Funplant = funorg.Organization_Name,
                             Storage_Bound_ID = outboundm.Storage_Outbound_ID,
                             Inout_Type = inouttype.Enum_Value,
                             Material_Id = mat.Material_Id,
                             Material_Name = mat.Material_Name,
                             Material_Types = mat.Material_Types,
                             Warehouse_ID = war.Warehouse_ID,
                             Rack_ID = warst.Rack_ID,
                             Storage_ID = warst.Storage_ID,
                             InOut_Date = M.InOut_Date,
                             InOut_QTY = 0 - M.InOut_Qty,
                             Balance_Qty = M.Balance_Qty,
                             Bound_Type = typeenum.Enum_Value,
                             ModifiedUser = users.User_Name,
                             Modified_Date = M.Modified_Date,
                             BG_Organization_UID = war.BG_Organization_UID,
                             FunPlant_Organization_UID = war.FunPlant_Organization_UID,
                             InOut_Type_UID = M.InOut_Type_UID,
                             Warehouse_Storage_UID = warst.Warehouse_Storage_UID
                         };
            query1 = query1.Where(m => inlist.Contains(m.InOut_Type_UID) == false);

            query = query.Union(query1);

            query = query.Where(m => uids.Contains("," + m.Storage_InOut_Detaill_UID + ","));


            return query.ToList();
        }
        public List<StorageDetailDTO> DoAllExportStorageDetailReprot(StorageSearchMod searchModel)
        {
            var query = from M in DataContext.Storage_InOut_Detail
                        join inbound in DataContext.Storage_Inbound on
                        M.Storage_InOut_UID equals inbound.Storage_Inbound_UID
                        join mat in DataContext.Material_Info on
                        inbound.Material_UID equals mat.Material_Uid
                        join warst in DataContext.Warehouse_Storage on
                        inbound.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        join war in DataContext.Warehouse on
                        warst.Warehouse_UID equals war.Warehouse_UID
                        join bgorg in DataContext.System_Organization on
                        war.BG_Organization_UID equals bgorg.Organization_UID
                        join funorg in DataContext.System_Organization on
                        war.FunPlant_Organization_UID equals funorg.Organization_UID
                        join typeenum in DataContext.Enumeration on
                        inbound.PartType_UID equals typeenum.Enum_UID
                        join inouttype in DataContext.Enumeration on
                        inbound.Storage_Inbound_Type_UID equals inouttype.Enum_UID
                        join users in DataContext.System_Users on
                        M.Modified_UID equals users.Account_UID
                        select new StorageDetailDTO
                        {
                            Storage_InOut_Detaill_UID = M.Storage_InOut_Detail_UID,
                            BG_Name = bgorg.Organization_Name,
                            Funplant = funorg.Organization_Name,
                            Storage_Bound_ID = inbound.Storage_Inbound_ID,
                            Inout_Type = inouttype.Enum_Value,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Warehouse_ID = war.Warehouse_ID,
                            Rack_ID = warst.Rack_ID,
                            Storage_ID = warst.Storage_ID,
                            InOut_Date = M.InOut_Date,
                            InOut_QTY = M.InOut_Qty,
                            Balance_Qty = M.Balance_Qty,
                            Bound_Type = typeenum.Enum_Value,
                            ModifiedUser = users.User_Name,
                            Modified_Date = M.Modified_Date,
                            BG_Organization_UID = war.BG_Organization_UID,
                            FunPlant_Organization_UID = war.FunPlant_Organization_UID,
                            InOut_Type_UID = M.InOut_Type_UID,
                            Warehouse_Storage_UID = warst.Warehouse_Storage_UID
                        };
            List<int> inlist = new List<int>();
            //fky2017/11/13
            //inlist.Add(379);
            inlist.Add(411);
            //fky2017/11/13
            //inlist.Add(402);
            inlist.Add(427);
            //fky2017/11/13
            //inlist.Add(386);
            inlist.Add(415);
            //fky2017/11/13
            //inlist.Add(403);
            inlist.Add(428);
            //fky2017/11/13
            //inlist.Add(399);
            inlist.Add(424);
            query = query.Where(m => inlist.Contains(m.InOut_Type_UID));
            var query1 = from M in DataContext.Storage_InOut_Detail
                         join outboundd in DataContext.Storage_Outbound_D on
                         M.Storage_InOut_UID equals outboundd.Storage_Outbound_D_UID
                         join outboundm in DataContext.Storage_Outbound_M on
                         outboundd.Storage_Outbound_M_UID equals outboundm.Storage_Outbound_M_UID
                         join mat in DataContext.Material_Info on
                         M.Material_UID equals mat.Material_Uid
                         join warst in DataContext.Warehouse_Storage on
                         M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                         join war in DataContext.Warehouse on
                         warst.Warehouse_UID equals war.Warehouse_UID
                         join bgorg in DataContext.System_Organization on
                         war.BG_Organization_UID equals bgorg.Organization_UID
                         join funorg in DataContext.System_Organization on
                         war.FunPlant_Organization_UID equals funorg.Organization_UID
                         join typeenum in DataContext.Enumeration on
                         outboundd.PartType_UID equals typeenum.Enum_UID
                         join inouttype in DataContext.Enumeration on
                         M.InOut_Type_UID equals inouttype.Enum_UID
                         join users in DataContext.System_Users on
                         M.Modified_UID equals users.Account_UID
                         select new StorageDetailDTO
                         {
                             Storage_InOut_Detaill_UID = M.Storage_InOut_Detail_UID,
                             BG_Name = bgorg.Organization_Name,
                             Funplant = funorg.Organization_Name,
                             Storage_Bound_ID = outboundm.Storage_Outbound_ID,
                             Inout_Type = inouttype.Enum_Value,
                             Material_Id = mat.Material_Id,
                             Material_Name = mat.Material_Name,
                             Material_Types = mat.Material_Types,
                             Warehouse_ID = war.Warehouse_ID,
                             Rack_ID = warst.Rack_ID,
                             Storage_ID = warst.Storage_ID,
                             InOut_Date = M.InOut_Date,
                             InOut_QTY = 0 - M.InOut_Qty,
                             Balance_Qty = M.Balance_Qty,
                             Bound_Type = typeenum.Enum_Value,
                             ModifiedUser = users.User_Name,
                             Modified_Date = M.Modified_Date,
                             BG_Organization_UID = war.BG_Organization_UID,
                             FunPlant_Organization_UID = war.FunPlant_Organization_UID,
                             InOut_Type_UID = M.InOut_Type_UID,
                             Warehouse_Storage_UID = warst.Warehouse_Storage_UID
                         };
            query1 = query1.Where(m => inlist.Contains(m.InOut_Type_UID) == false);
            query = query.Union(query1);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.InOut_Type_UID != 0)
                query = query.Where(m => m.InOut_Type_UID == searchModel.InOut_Type_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_Bound_ID))
                query = query.Where(m => m.Storage_Bound_ID.Contains(searchModel.Storage_Bound_ID));
            //if (searchModel.InOut_Date.Year != 1)
            //{

            //    DateTime nextday = searchModel.InOut_Date.AddDays(1);
            //    query = query.Where(m => m.InOut_Date > searchModel.InOut_Date & m.InOut_Date < nextday);
            //}
            if (searchModel.Start_Date.Year != 1 && searchModel.End_Date.Year != 1)
            {
                DateTime nextday = searchModel.End_Date.AddDays(1);
                query = query.Where(m => m.InOut_Date >= searchModel.Start_Date & m.InOut_Date < nextday);
            }

            if (searchModel.Warehouse_Storage_UID != 0)
                query = query.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
            List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
            }

            return query.ToList();
        }


        public IQueryable<StorageDetailDTO> GetInfo(StorageSearchMod searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Storage_InOut_Detail
                        join inbound in DataContext.Storage_Inbound on
                        M.Storage_InOut_UID equals inbound.Storage_Inbound_UID
                        join mat in DataContext.Material_Info on
                        inbound.Material_UID equals mat.Material_Uid
                        join warst in DataContext.Warehouse_Storage on
                        inbound.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        join war in DataContext.Warehouse on
                        warst.Warehouse_UID equals war.Warehouse_UID
                        join bgorg in DataContext.System_Organization on
                        war.BG_Organization_UID equals bgorg.Organization_UID
                        join funorg in DataContext.System_Organization on
                        war.FunPlant_Organization_UID equals funorg.Organization_UID
                        join typeenum in DataContext.Enumeration on
                        inbound.PartType_UID equals typeenum.Enum_UID
                        join inouttype in DataContext.Enumeration on
                        inbound.Storage_Inbound_Type_UID equals inouttype.Enum_UID
                        join users in DataContext.System_Users on
                        M.Modified_UID equals users.Account_UID
                        select new StorageDetailDTO
                        {
                            Storage_InOut_Detaill_UID = M.Storage_InOut_Detail_UID,
                            BG_Name = bgorg.Organization_Name,
                            Funplant = funorg.Organization_Name,
                            Storage_Bound_ID = inbound.Storage_Inbound_ID,
                            Inout_Type = inouttype.Enum_Value,
                            Material_Id =mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Warehouse_ID = war.Warehouse_ID,
                            Rack_ID = warst.Rack_ID,
                            Storage_ID = warst.Storage_ID,
                            InOut_Date = M.InOut_Date,
                            InOut_QTY = M.InOut_Qty,
                            Balance_Qty = M.Balance_Qty,
                            Bound_Type = typeenum.Enum_Value,
                            ModifiedUser = users.User_Name,
                            Modified_Date=M.Modified_Date,
                            BG_Organization_UID = war.BG_Organization_UID,
                            FunPlant_Organization_UID = war.FunPlant_Organization_UID,
                            InOut_Type_UID = M.InOut_Type_UID,
                            Warehouse_Storage_UID = warst.Warehouse_Storage_UID
                        };
            List<int> inlist = new List<int>();
            //fky2017/11/13
            //inlist.Add(379);
            inlist.Add(411);
            //fky2017/11/13
            //inlist.Add(402);
            inlist.Add(427);
            //fky2017/11/13
            //inlist.Add(386);
            inlist.Add(415);
            //fky2017/11/13
            //inlist.Add(403);
            inlist.Add(428);
            //fky2017/11/13
            //inlist.Add(399);
            inlist.Add(424);
            query = query.Where(m => inlist.Contains(m.InOut_Type_UID));
            var query1 = from M in DataContext.Storage_InOut_Detail
                         join outboundd in DataContext.Storage_Outbound_D on
                         M.Storage_InOut_UID equals outboundd.Storage_Outbound_D_UID
                         join outboundm in DataContext.Storage_Outbound_M on
                         outboundd.Storage_Outbound_M_UID equals outboundm.Storage_Outbound_M_UID
                         join mat in DataContext.Material_Info on
                         M.Material_UID equals mat.Material_Uid
                         join warst in DataContext.Warehouse_Storage on
                         M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                         join war in DataContext.Warehouse on
                         warst.Warehouse_UID equals war.Warehouse_UID
                         join bgorg in DataContext.System_Organization on
                         war.BG_Organization_UID equals bgorg.Organization_UID
                         join funorg in DataContext.System_Organization on
                         war.FunPlant_Organization_UID equals funorg.Organization_UID
                         join typeenum in DataContext.Enumeration on
                         outboundd.PartType_UID equals typeenum.Enum_UID
                         join inouttype in DataContext.Enumeration on
                         M.InOut_Type_UID equals inouttype.Enum_UID
                         join users in DataContext.System_Users on
                         M.Modified_UID equals users.Account_UID
                         select new StorageDetailDTO
                         {
                             Storage_InOut_Detaill_UID = M.Storage_InOut_Detail_UID,
                             BG_Name = bgorg.Organization_Name,
                             Funplant = funorg.Organization_Name,
                             Storage_Bound_ID = outboundm.Storage_Outbound_ID,
                             Inout_Type = inouttype.Enum_Value,
                             Material_Id = mat.Material_Id,
                             Material_Name = mat.Material_Name,
                             Material_Types = mat.Material_Types,
                             Warehouse_ID = war.Warehouse_ID,
                             Rack_ID = warst.Rack_ID,
                             Storage_ID = warst.Storage_ID,
                             InOut_Date = M.InOut_Date,
                             InOut_QTY = 0 - M.InOut_Qty,
                             Balance_Qty = M.Balance_Qty,
                             Bound_Type = typeenum.Enum_Value,
                             ModifiedUser = users.User_Name,
                             Modified_Date = M.Modified_Date,
                             BG_Organization_UID = war.BG_Organization_UID,
                             FunPlant_Organization_UID = war.FunPlant_Organization_UID,
                             InOut_Type_UID = M.InOut_Type_UID,
                             Warehouse_Storage_UID = warst.Warehouse_Storage_UID
                         };
            query1 = query1.Where(m => inlist.Contains(m.InOut_Type_UID) == false);
            query = query.Union(query1);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.InOut_Type_UID != 0)
                query = query.Where(m => m.InOut_Type_UID == searchModel.InOut_Type_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_Bound_ID))
                query = query.Where(m => m.Storage_Bound_ID.Contains(searchModel.Storage_Bound_ID));
     
            if (searchModel.Start_Date.Year!= 1)
            {             
                query = query.Where(m => m.InOut_Date >= searchModel.Start_Date);                
            }

            if(searchModel.End_Date.Year != 1)
            {        
                query = query.Where(m => m.InOut_Date <= searchModel.End_Date);
            }
         
            if (searchModel.Warehouse_Storage_UID != 0)
                query = query.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);

            if (!string.IsNullOrWhiteSpace(searchModel.Warehouse_ID))
                query = query.Where(m => m.Warehouse_ID.Contains(searchModel.Warehouse_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
                query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
                query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));

            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
            List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
            }
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;
        }
        public List<SystemOrgDTO> GetOpType(int plantorguid)
        {
            var sqlStr = @"SELECT * FROM dbo.System_Organization t1 INNER JOIN dbo.System_OrganizationBOM t2
                                        ON t1.Organization_UID=t2.ChildOrg_UID WHERE  t1.Organization_ID LIKE'%2000%' ";
            if (plantorguid != 0)
            {
                sqlStr = @"SELECT * FROM dbo.System_Organization t1 INNER JOIN dbo.System_OrganizationBOM t2
                                        ON t1.Organization_UID=t2.ChildOrg_UID WHERE  t1.Organization_ID LIKE'%2000%'  and  t2.ParentOrg_UID={0}";
                sqlStr = string.Format(sqlStr, plantorguid);
            }

            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList();
            return dbList;
        }

        public IQueryable<StorageReportDTO> GetStorageReportInfo(StorageReportDTO searchModel, Page page, out int totalcount)
        {
            SqlParameter startDate = null;
            SqlParameter endDate = null;
            var plant = new SqlParameter("plant", searchModel.Plant_Organization_UID);
            var bg = new SqlParameter("bg", searchModel.BG_Organization_UID);
            var funplant = new SqlParameter("funplant", searchModel.FunPlant_Organization_UID);
            startDate = new SqlParameter("start_date", ((DateTime)searchModel.Start_Date).ToString(FormatConstants.DateTimeFormatStringByDate).Substring(0, 10));
            endDate = new SqlParameter("end_date", ((DateTime)searchModel.End_Date).ToString(FormatConstants.DateTimeFormatStringByDate).Substring(0, 10));
            if(startDate.Value.ToString()=="0001-01-01")
            {
                startDate.Value = "2000-01-01";
            }
            if (endDate.Value.ToString() == "0001-01-01")
            {
                endDate.Value = DateTime.Now.ToString(FormatConstants.DateTimeFormatStringByDate);
            }
            IEnumerable<StorageReportDTO> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<StorageReportDTO>("usp_StorageReport  @plant,@bg,@funplant,@start_date,@end_date",
                plant,bg,funplant,startDate,endDate).ToArray();
            var query = result.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
            {
                query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
            }
            if(searchModel.Warehouse_Type_UID!=0)
            {
                query = query.Where(m => m.Warehouse_Type_UID==searchModel.Warehouse_Type_UID);
            }
            totalcount = query.Count();
            return query.GetPage(page);
        }

        public List<StorageReportDTO> DoSRExportFunction(int plant, int bg, int funplant, string material, DateTime start_date, DateTime end_date)
        {
            SqlParameter startDate = null;
            SqlParameter endDate = null;
            var sqlplant = new SqlParameter("plant", plant);
            var sqlbg = new SqlParameter("bg", bg);
            var sqlfunplant = new SqlParameter("funplant",funplant);
            startDate = new SqlParameter("start_date", (start_date).ToString(FormatConstants.DateTimeFormatStringByDate).Substring(0, 10));
            endDate = new SqlParameter("end_date", (end_date).ToString(FormatConstants.DateTimeFormatStringByDate).Substring(0, 10));
            if (startDate.Value.ToString() == "0001-01-01")
            {
                startDate.Value = "2000-01-01";
            }
            if (endDate.Value.ToString() == "0001-01-01")
            {
                endDate.Value = DateTime.Now.ToString(FormatConstants.DateTimeFormatStringByDate);
            }
            IEnumerable<StorageReportDTO> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<StorageReportDTO>("usp_StorageReport  @plant,@bg,@funplant,@start_date,@end_date",
                sqlplant, sqlbg, sqlfunplant, startDate, endDate).ToArray();
            var query = result.AsQueryable();
            if (!string.IsNullOrWhiteSpace(material))
                query = query.Where(m => m.Material_Id.Contains(material));
            return query.ToList();
        }
    }
}
