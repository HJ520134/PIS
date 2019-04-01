using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using System.Data.Entity.SqlServer;
namespace PDMS.Data.Repository
{
    public interface IStorageCheckRepository : IRepository<Storage_Check>
    {
        IQueryable<StorageCheckDTO> GetInfo(StorageCheckDTO searchModel, Page page, out int totalcount);
        List<StorageCheckDTO> GetByUid(int Storage_Check_UID);
        List<StorageCheckDTO> DoAllExportStorageCheckReprot(StorageCheckDTO searchModel);
        List<StorageCheckDTO> DoExportStorageCheckReprot(string uids);
        List<StorageCheckDTO> DownloadStorageCheck(int PartType_UID, string Material_Id, string Material_Name, string Material_Types, string Warehouse_ID, string Rack_ID, string Storage_ID, int Plant_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
    }
    public class StorageCheckRepository: RepositoryBase<Storage_Check>, IStorageCheckRepository
    {
        public StorageCheckRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public IQueryable<StorageCheckDTO> GetInfo(StorageCheckDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Storage_Check
                                    join status in DataContext.Enumeration on
                                    M.Status_UID equals status.Enum_UID
                                    join mat in DataContext.Material_Info on
                                    M.Material_UID equals mat.Material_Uid
                                    join warst in DataContext.Warehouse_Storage on
                                    M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                                    join war in DataContext.Warehouse on 
                                    warst.Warehouse_UID equals war.Warehouse_UID
                                    join appuser in DataContext.System_Users on
                                    M.Applicant_UID equals appuser.Account_UID
                                    join apruser in DataContext.System_Users on
                                    M.Applicant_UID equals apruser.Account_UID
                                    join checkenum in DataContext.Enumeration on 
                                    M.PartType_UID equals checkenum.Enum_UID
                        select new StorageCheckDTO
                        {
                            Storage_Check_UID = M.Storage_Check_UID,
                            Storage_Check_ID = M.Storage_Check_ID,
                            PartType = checkenum.Enum_Value,
                            Status = status.Enum_Value,
                            Material_Id = mat.Material_Id,
                            Material_Name=mat.Material_Name,
                            Material_Types=mat.Material_Types,
                            Warehouse_ID = war.Warehouse_ID,
                            Rack_ID = warst.Rack_ID,
                            Storage_ID = warst.Storage_ID,
                            Check_Qty =M.Check_Qty,
                            PartType_UID =M.PartType_UID,
                            ApplicantUser =appuser.User_Name,
                            Applicant_Date=M.Applicant_Date,
                            ApproverUser =apruser.User_Name,
                            Approver_Date=M.Approver_Date,
                            Status_UID =M.Status_UID,
                            BG_Organization_UID=war.BG_Organization_UID
                        };
            if (searchModel.PartType_UID != 0)
                query = query.Where(m => m.PartType_UID == searchModel.PartType_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
            if (!string.IsNullOrWhiteSpace(searchModel.Warehouse_ID))
                query = query.Where(m => m.Warehouse_ID.Contains(searchModel.Warehouse_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
                query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
                query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.ApplicantUser))
                query = query.Where(m => m.ApplicantUser.Contains(searchModel.ApplicantUser));
            if (!string.IsNullOrWhiteSpace(searchModel.ApproverUser))
                query = query.Where(m => m.ApproverUser.Contains(searchModel.ApproverUser));
            if (searchModel.Status_UID!=0)
                query = query.Where(m => m.Status_UID == searchModel.Status_UID);
            else
                //fky2017/11/13
                //query = query.Where(m => m.Status_UID != 392);
                  query = query.Where(m => m.Status_UID != 420);
            List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
            }
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Applicant_Date).GetPage(page);
            return query;
        }
        public List<StorageCheckDTO> DoAllExportStorageCheckReprot(StorageCheckDTO searchModel)
        {
            var query = from M in DataContext.Storage_Check
                        join status in DataContext.Enumeration on
                        M.Status_UID equals status.Enum_UID
                        join mat in DataContext.Material_Info on
                        M.Material_UID equals mat.Material_Uid
                        join warst in DataContext.Warehouse_Storage on
                        M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        join war in DataContext.Warehouse on
                        warst.Warehouse_UID equals war.Warehouse_UID
                        join appuser in DataContext.System_Users on
                        M.Applicant_UID equals appuser.Account_UID
                        join apruser in DataContext.System_Users on
                        M.Applicant_UID equals apruser.Account_UID
                        join checkenum in DataContext.Enumeration on
                        M.PartType_UID equals checkenum.Enum_UID
                        select new StorageCheckDTO
                        {
                            Storage_Check_UID = M.Storage_Check_UID,
                            Storage_Check_ID = M.Storage_Check_ID,
                            PartType = checkenum.Enum_Value,
                            Status = status.Enum_Value,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Warehouse_ID = war.Warehouse_ID,
                            Rack_ID = warst.Rack_ID,
                            Storage_ID = warst.Storage_ID,
                            Check_Qty = M.Check_Qty,
                            PartType_UID = M.PartType_UID,
                            ApplicantUser = appuser.User_Name,
                            Applicant_Date = M.Applicant_Date,
                            ApproverUser = apruser.User_Name,
                            Approver_Date = M.Approver_Date,
                            Status_UID = M.Status_UID,
                            BG_Organization_UID = war.BG_Organization_UID
                        };
            if (searchModel.PartType_UID != 0)
                query = query.Where(m => m.PartType_UID == searchModel.PartType_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
            if (!string.IsNullOrWhiteSpace(searchModel.Warehouse_ID))
                query = query.Where(m => m.Warehouse_ID.Contains(searchModel.Warehouse_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
                query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
                query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.ApplicantUser))
                query = query.Where(m => m.ApplicantUser.Contains(searchModel.ApplicantUser));
            if (!string.IsNullOrWhiteSpace(searchModel.ApproverUser))
                query = query.Where(m => m.ApproverUser.Contains(searchModel.ApproverUser));
            if (searchModel.Status_UID != 0)
                query = query.Where(m => m.Status_UID == searchModel.Status_UID);
            else
                //fky2017/11/13
                //query = query.Where(m => m.Status_UID != 392);
                query = query.Where(m => m.Status_UID != 420);
            List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
            }
            return query.ToList();
        }
        public List<StorageCheckDTO> DoExportStorageCheckReprot(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Storage_Check
                        join status in DataContext.Enumeration on
                        M.Status_UID equals status.Enum_UID
                        join mat in DataContext.Material_Info on
                        M.Material_UID equals mat.Material_Uid
                        join warst in DataContext.Warehouse_Storage on
                        M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        join war in DataContext.Warehouse on
                        warst.Warehouse_UID equals war.Warehouse_UID
                        join appuser in DataContext.System_Users on
                        M.Applicant_UID equals appuser.Account_UID
                        join apruser in DataContext.System_Users on
                        M.Applicant_UID equals apruser.Account_UID
                        join checkenum in DataContext.Enumeration on
                        M.PartType_UID equals checkenum.Enum_UID
                        select new StorageCheckDTO
                        {
                            Storage_Check_UID = M.Storage_Check_UID,
                            Storage_Check_ID = M.Storage_Check_ID,
                            PartType = checkenum.Enum_Value,
                            Status = status.Enum_Value,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Warehouse_ID = war.Warehouse_ID,
                            Rack_ID = warst.Rack_ID,
                            Storage_ID = warst.Storage_ID,
                            Check_Qty = M.Check_Qty,
                            PartType_UID = M.PartType_UID,
                            ApplicantUser = appuser.User_Name,
                            Applicant_Date = M.Applicant_Date,
                            ApproverUser = apruser.User_Name,
                            Approver_Date = M.Approver_Date,
                            Status_UID = M.Status_UID,
                            BG_Organization_UID = war.BG_Organization_UID
                        };

            query = query.Where(m => uids.Contains("," + m.Storage_Check_UID + ","));

            return query.ToList();
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
        public List<StorageCheckDTO> GetByUid(int Storage_Check_UID)
        {
            string sql = @"SELECT *,t5.Enum_Value PartType,t6.Enum_Value Status FROM Storage_Check t1 
                                    inner join Material_Info t2 on t1.Material_Uid=t2.Material_Uid inner join 
                                    Warehouse_Storage t3 on t1.Warehouse_Storage_UID=t3.Warehouse_Storage_UID inner join 
                                    Warehouse t4 on t3.Warehouse_UID=t4.Warehouse_UID inner join enumeration
                                    t5 on t1.PartType_UID=t5.Enum_UID inner join Enumeration t6
                                    on t1.Status_UID=t6.Enum_UID where t1.Storage_Check_UID={0}";
            sql = string.Format(sql, Storage_Check_UID);
            var dblist = DataContext.Database.SqlQuery<StorageCheckDTO>(sql).ToList();
            return dblist;
        }
        public List<StorageCheckDTO> DownloadStorageCheck(int PartType_UID, string Material_Id, string Material_Name, string Material_Types, string Warehouse_ID, string Rack_ID, string Storage_ID, int Plant_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from M in DataContext.Material_Inventory
                        join mat in DataContext.Material_Info on
                        M.Material_UID equals mat.Material_Uid
                        join warst in DataContext.Warehouse_Storage on
                        M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        join war in DataContext.Warehouse on
                        warst.Warehouse_UID equals war.Warehouse_UID
                        join checkenum in DataContext.Enumeration on
                         war.Warehouse_Type_UID equals checkenum.Enum_UID
                        select new StorageCheckDTO
                        {
                            Status_UID = war.Warehouse_Type_UID,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Warehouse_ID = war.Warehouse_ID,
                            Rack_ID = warst.Rack_ID,
                            Storage_ID = warst.Storage_ID,
                            Plant_UID= mat.Organization_UID.Value,
                            FunPlant_Organization_UID=war.FunPlant_Organization_UID,
                            BG_Organization_UID=war.BG_Organization_UID,


                        };

            int Status_UID = 0;
            if (PartType_UID == 433 || PartType_UID == 434 || PartType_UID == 417)
            {
                Status_UID = 418;
            }
            else
            {
                Status_UID = 405;
            }
            if (Status_UID != 0)
            {          
                    query = query.Where(m => m.Status_UID == Status_UID);
            }
            if (Plant_UID != 0)
            {
                query = query.Where(m => m.Plant_UID == Plant_UID);
            }
            if (BG_Organization_UID != 0)
            {
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            }
            if (FunPlant_Organization_UID != 0)
            {
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            }
            if (!string.IsNullOrWhiteSpace(Material_Id))
                query = query.Where(m => m.Material_Id.Contains(Material_Id));
            if (!string.IsNullOrWhiteSpace(Material_Name))
                query = query.Where(m => m.Material_Name.Contains(Material_Name));
            if (!string.IsNullOrWhiteSpace(Material_Types))
                query = query.Where(m => m.Material_Types.Contains(Material_Types));
            if (!string.IsNullOrWhiteSpace(Warehouse_ID))
                query = query.Where(m => m.Warehouse_ID.Contains(Warehouse_ID));
            if (!string.IsNullOrWhiteSpace(Rack_ID))
                query = query.Where(m => m.Rack_ID.Contains(Rack_ID));
            if (!string.IsNullOrWhiteSpace(Storage_ID))
                query = query.Where(m => m.Storage_ID.Contains(Storage_ID));
            return SetStorageCheckPartType(query.ToList());
        }
        private List<StorageCheckDTO> SetStorageCheckPartType(List<StorageCheckDTO> StorageCheckDTOs)
        {

            List<System_Organization> OrgDTOs = DataContext.System_Organization.ToList();
            foreach (var item in StorageCheckDTOs)
            {
                if(item.Status_UID==405)
                {
                    item.PartType = "正常料良品";
                }
                else
                {
                    item.PartType = "非正常料良品";
                }
                item.Plant = OrgDTOs.FirstOrDefault(o => o.Organization_UID == item.Plant_UID).Organization_Name;
                item.BG_Organization = OrgDTOs.FirstOrDefault(o => o.Organization_UID == item.BG_Organization_UID).Organization_Name;
                item.FunPlant_Organization = OrgDTOs.FirstOrDefault(o => o.Organization_UID == item.FunPlant_Organization_UID).Organization_Name;
            }
            return StorageCheckDTOs;

        }
    }
}
