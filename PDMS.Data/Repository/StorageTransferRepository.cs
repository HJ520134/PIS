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
    public interface IStorageTransferRepository : IRepository<Storage_Transfer>
    {
        IQueryable<StorageTransferDTO> GetInfo(StorageTransferDTO searchModel, Page page, out int totalcount);
        List<StorageTransferDTO> GetByUid(int Storage_Transfer_UID);
        List<WarehouseDTO> GetWarehouseSITE(int planID);
        List<StorageTransferDTO> DoExportStorageTransferReprot(string uids);
        List<StorageTransferDTO> DoAllExportStorageTransferReprot(StorageTransferDTO search);
    }
    public class StorageTransferRepository : RepositoryBase<Storage_Transfer>, IStorageTransferRepository
    {
        public StorageTransferRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IQueryable<StorageTransferDTO> GetInfo(StorageTransferDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Storage_Transfer
                                    join statusenum in DataContext.Enumeration on
                                    M.Status_UID equals statusenum.Enum_UID
                                    join mat in DataContext.Material_Info on
                                    M.Material_UID equals mat.Material_Uid
                                    join outwarst in DataContext.Warehouse_Storage on
                                    M.Out_Warehouse_Storage_UID equals outwarst.Warehouse_Storage_UID
                                    join outwar in DataContext.Warehouse on
                                    outwarst.Warehouse_UID equals outwar.Warehouse_UID
                                    join inwarst in DataContext.Warehouse_Storage on
                                    M.In_Warehouse_Storage_UID equals inwarst.Warehouse_Storage_UID
                                    join inwar in DataContext.Warehouse on
                                    inwarst.Warehouse_UID equals inwar.Warehouse_UID
                                    join typeenum in DataContext.Enumeration on
                                    M.PartType_UID equals typeenum.Enum_UID
                                    join appuser in DataContext.System_Users on
                                    M.Applicant_UID equals appuser.Account_UID
                                    join apruser in DataContext.System_Users on
                                    M.Approver_UID equals apruser.Account_UID
                        select new StorageTransferDTO
                        {
                            Storage_Transfer_UID = M.Storage_Transfer_UID,
                            Storage_Transfer_ID = M.Storage_Transfer_ID,
                            Status = statusenum.Enum_Value,
                            Material_Id =mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types=mat.Material_Types,
                            Out_Warehouse_ID =outwar.Warehouse_ID,
                            Out_Rack_ID = outwarst.Rack_ID,
                            Out_Storage_ID = outwarst.Storage_ID,
                            Out_BG_Organization_UID= outwar.BG_Organization_UID,
                            Out_BG_Organization=outwar.System_Organization.Organization_Name,
                            Out_FunPlant_Organization_UID = outwar.FunPlant_Organization_UID,
                            Out_FunPlant_Organization = outwar.System_Organization1.Organization_Name,
                            In_Warehouse_ID = inwar.Warehouse_ID,
                            In_Rack_ID = inwarst.Rack_ID,
                            In_Storage_ID = inwarst.Storage_ID,
                            In_BG_Organization_UID = inwar.BG_Organization_UID,
                            In_BG_Organization = inwar.System_Organization.Organization_Name,
                            In_FunPlant_Organization_UID = inwar.FunPlant_Organization_UID,
                            In_FunPlant_Organization = inwar.System_Organization1.Organization_Name,
                            Transfer_Qty = M.Transfer_Qty,
                            Transfer_Type =typeenum.Enum_Value,
                            ApplicantUser =appuser.User_Name,
                            Applicant_Date = M.Applicant_Date,
                            ApproverUser =apruser.User_Name,
                            Approver_Date = M.Approver_Date,
                            Status_UID = M.Status_UID,
                            Out_Warehouse_Storage_UID = M.Out_Warehouse_Storage_UID,
                            In_Warehouse_Storage_UID=M.In_Warehouse_Storage_UID,
                            BG_Organization_UID=inwar.BG_Organization_UID

                        };
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_Transfer_ID))
                query = query.Where(m => m.Storage_Transfer_ID.Contains(searchModel.Storage_Transfer_ID));
            if (searchModel.Status_UID != 0)
                query = query.Where(m => m.Status_UID == searchModel.Status_UID);
            else
                //fky2017/11/13
                //query = query.Where(m => m.Status_UID !=392);
                query = query.Where(m => m.Status_UID != 420);
            if (!string.IsNullOrWhiteSpace(searchModel.ApplicantUser))
                query = query.Where(m => m.ApplicantUser.Contains(searchModel.ApplicantUser));
            if (searchModel.Applicant_Date.Year != 1)
            {
                DateTime nextday = searchModel.Applicant_Date.AddDays(1);
                query = query.Where(m => m.Applicant_Date >= searchModel.Applicant_Date & m.Applicant_Date < nextday);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.ApproverUser))
                query = query.Where(m => m.ApproverUser.Contains(searchModel.ApproverUser));
            if (searchModel.Approver_Date.Year != 1)
            {
                DateTime nextday = searchModel.Approver_Date.AddDays(1);
                query = query.Where(m => m.Approver_Date >= searchModel.Approver_Date & m.Applicant_Date < nextday);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));

            if (!string.IsNullOrWhiteSpace(searchModel.Out_Warehouse_ID))
                query = query.Where(m => m.Out_Warehouse_ID.Contains(searchModel.Out_Warehouse_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Out_Rack_ID))
                query = query.Where(m => m.Out_Rack_ID.Contains(searchModel.Out_Rack_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Out_Storage_ID))
                query = query.Where(m => m.Out_Storage_ID.Contains(searchModel.Out_Storage_ID));

            if (!string.IsNullOrWhiteSpace(searchModel.In_Warehouse_ID))
                query = query.Where(m => m.In_Warehouse_ID.Contains(searchModel.In_Warehouse_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.In_Rack_ID))
                query = query.Where(m => m.In_Rack_ID.Contains(searchModel.In_Rack_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.In_Storage_ID))
                query = query.Where(m => m.In_Storage_ID.Contains(searchModel.In_Storage_ID));

            if (searchModel.Out_Warehouse_Storage_UID != 0)
                query = query.Where(m => m.Out_Warehouse_Storage_UID == searchModel.Out_Warehouse_Storage_UID);
            if (searchModel.In_Warehouse_Storage_UID != 0)
                query = query.Where(m => m.In_Warehouse_Storage_UID == searchModel.In_Warehouse_Storage_UID);
            List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
            }

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Applicant_Date).GetPage(page);
            return query;
        }
         public  List<StorageTransferDTO> DoExportStorageTransferReprot(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Storage_Transfer
                        join statusenum in DataContext.Enumeration on
                        M.Status_UID equals statusenum.Enum_UID
                        join mat in DataContext.Material_Info on
                        M.Material_UID equals mat.Material_Uid
                        join outwarst in DataContext.Warehouse_Storage on
                        M.Out_Warehouse_Storage_UID equals outwarst.Warehouse_Storage_UID
                        join outwar in DataContext.Warehouse on
                        outwarst.Warehouse_UID equals outwar.Warehouse_UID
                        join inwarst in DataContext.Warehouse_Storage on
                        M.In_Warehouse_Storage_UID equals inwarst.Warehouse_Storage_UID
                        join inwar in DataContext.Warehouse on
                        inwarst.Warehouse_UID equals inwar.Warehouse_UID
                        join typeenum in DataContext.Enumeration on
                        M.PartType_UID equals typeenum.Enum_UID
                        join appuser in DataContext.System_Users on
                        M.Applicant_UID equals appuser.Account_UID
                        join apruser in DataContext.System_Users on
                        M.Approver_UID equals apruser.Account_UID
                        select new StorageTransferDTO
                        {
                            Storage_Transfer_UID = M.Storage_Transfer_UID,
                            Storage_Transfer_ID = M.Storage_Transfer_ID,
                            Status = statusenum.Enum_Value,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Out_Warehouse_ID = outwar.Warehouse_ID,
                            Out_Rack_ID = outwarst.Rack_ID,
                            Out_Storage_ID = outwarst.Storage_ID,
                            Out_BG_Organization_UID = outwar.BG_Organization_UID,
                            Out_BG_Organization = outwar.System_Organization.Organization_Name,
                            Out_FunPlant_Organization_UID = outwar.FunPlant_Organization_UID,
                            Out_FunPlant_Organization = outwar.System_Organization1.Organization_Name,
                            In_Warehouse_ID = inwar.Warehouse_ID,
                            In_Rack_ID = inwarst.Rack_ID,
                            In_Storage_ID = inwarst.Storage_ID,
                            In_BG_Organization_UID = inwar.BG_Organization_UID,
                            In_BG_Organization = inwar.System_Organization.Organization_Name,
                            In_FunPlant_Organization_UID = inwar.FunPlant_Organization_UID,
                            In_FunPlant_Organization = inwar.System_Organization1.Organization_Name,
                            Transfer_Qty = M.Transfer_Qty,
                            Transfer_Type = typeenum.Enum_Value,
                            ApplicantUser = appuser.User_Name,
                            Applicant_Date = M.Applicant_Date,
                            ApproverUser = apruser.User_Name,
                            Approver_Date = M.Approver_Date,
                            Status_UID = M.Status_UID,
                            Out_Warehouse_Storage_UID = M.Out_Warehouse_Storage_UID,
                            In_Warehouse_Storage_UID = M.In_Warehouse_Storage_UID,
                            BG_Organization_UID = inwar.BG_Organization_UID

                        };
            query = query.Where(m => uids.Contains("," + m.Storage_Transfer_UID + ","));

            return query.ToList();
        }
        public List<StorageTransferDTO> DoAllExportStorageTransferReprot(StorageTransferDTO searchModel)
        {
            var query = from M in DataContext.Storage_Transfer
                        join statusenum in DataContext.Enumeration on
                        M.Status_UID equals statusenum.Enum_UID
                        join mat in DataContext.Material_Info on
                        M.Material_UID equals mat.Material_Uid
                        join outwarst in DataContext.Warehouse_Storage on
                        M.Out_Warehouse_Storage_UID equals outwarst.Warehouse_Storage_UID
                        join outwar in DataContext.Warehouse on
                        outwarst.Warehouse_UID equals outwar.Warehouse_UID
                        join inwarst in DataContext.Warehouse_Storage on
                        M.In_Warehouse_Storage_UID equals inwarst.Warehouse_Storage_UID
                        join inwar in DataContext.Warehouse on
                        inwarst.Warehouse_UID equals inwar.Warehouse_UID
                        join typeenum in DataContext.Enumeration on
                        M.PartType_UID equals typeenum.Enum_UID
                        join appuser in DataContext.System_Users on
                        M.Applicant_UID equals appuser.Account_UID
                        join apruser in DataContext.System_Users on
                        M.Approver_UID equals apruser.Account_UID
                        select new StorageTransferDTO
                        {
                            Storage_Transfer_UID = M.Storage_Transfer_UID,
                            Storage_Transfer_ID = M.Storage_Transfer_ID,
                            Status = statusenum.Enum_Value,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Out_Warehouse_ID = outwar.Warehouse_ID,
                            Out_Rack_ID = outwarst.Rack_ID,
                            Out_Storage_ID = outwarst.Storage_ID,
                            Out_BG_Organization_UID = outwar.BG_Organization_UID,
                            Out_BG_Organization = outwar.System_Organization.Organization_Name,
                            Out_FunPlant_Organization_UID = outwar.FunPlant_Organization_UID,
                            Out_FunPlant_Organization = outwar.System_Organization1.Organization_Name,
                            In_Warehouse_ID = inwar.Warehouse_ID,
                            In_Rack_ID = inwarst.Rack_ID,
                            In_Storage_ID = inwarst.Storage_ID,
                            In_BG_Organization_UID = inwar.BG_Organization_UID,
                            In_BG_Organization = inwar.System_Organization.Organization_Name,
                            In_FunPlant_Organization_UID = inwar.FunPlant_Organization_UID,
                            In_FunPlant_Organization = inwar.System_Organization1.Organization_Name,
                            Transfer_Qty = M.Transfer_Qty,
                            Transfer_Type = typeenum.Enum_Value,
                            ApplicantUser = appuser.User_Name,
                            Applicant_Date = M.Applicant_Date,
                            ApproverUser = apruser.User_Name,
                            Approver_Date = M.Approver_Date,
                            Status_UID = M.Status_UID,
                            Out_Warehouse_Storage_UID = M.Out_Warehouse_Storage_UID,
                            In_Warehouse_Storage_UID = M.In_Warehouse_Storage_UID,
                            BG_Organization_UID = inwar.BG_Organization_UID

                        };
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_Transfer_ID))
                query = query.Where(m => m.Storage_Transfer_ID.Contains(searchModel.Storage_Transfer_ID));
            if (searchModel.Status_UID != 0)
                query = query.Where(m => m.Status_UID == searchModel.Status_UID);
            else
                query = query.Where(m => m.Status_UID != 420);
            if (!string.IsNullOrWhiteSpace(searchModel.ApplicantUser))
                query = query.Where(m => m.ApplicantUser.Contains(searchModel.ApplicantUser));
            if (searchModel.Applicant_Date.Year != 1)
            {
                DateTime nextday = searchModel.Applicant_Date.AddDays(1);
                query = query.Where(m => m.Applicant_Date >= searchModel.Applicant_Date & m.Applicant_Date < nextday);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.ApproverUser))
                query = query.Where(m => m.ApproverUser.Contains(searchModel.ApproverUser));
            if (searchModel.Approver_Date.Year != 1)
            {
                DateTime nextday = searchModel.Approver_Date.AddDays(1);
                query = query.Where(m => m.Approver_Date >= searchModel.Approver_Date & m.Applicant_Date < nextday);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
            if (searchModel.Out_Warehouse_Storage_UID != 0)
                query = query.Where(m => m.Out_Warehouse_Storage_UID == searchModel.Out_Warehouse_Storage_UID);
            if (searchModel.In_Warehouse_Storage_UID != 0)
                query = query.Where(m => m.In_Warehouse_Storage_UID == searchModel.In_Warehouse_Storage_UID);
            List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
            }

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

        public List<StorageTransferDTO> GetByUid(int Storage_Transfer_UID)
        {
            string sql = @"select t1.*,Material_Id,Material_Name,Material_Types,outwar.Warehouse_ID Out_Warehouse_ID,
                        t3.Rack_ID Out_Rack_ID,t3.Storage_ID Out_Storage_ID,inwar.Warehouse_ID In_Warehouse_ID,
                        t4.Rack_ID In_Rack_ID,t4.Storage_ID In_Storage_ID,t5.Enum_Value Status,t6.Enum_Value PartType
                        from Storage_Transfer t1 inner join Material_Info t2
                        on t1.material_uid = t2.material_uid inner join Warehouse_Storage t3
                        on t1.Out_Warehouse_Storage_UID = t3.Warehouse_Storage_UID inner join Warehouse outwar
                        on t3.Warehouse_UID=outwar.Warehouse_UID inner join Warehouse_Storage t4
                        on t1.In_Warehouse_Storage_UID=t4.Warehouse_Storage_UID inner join Warehouse inwar 
                        on t4.Warehouse_UID=inwar.Warehouse_UID inner join Enumeration t5
                        on t1.Status_UID=T5.Enum_UID inner join Enumeration t6
						on t1.PartType_UID=t6.Enum_UID  where Storage_Transfer_UID={0}";
            sql = string.Format(sql, Storage_Transfer_UID);
            var dblist = DataContext.Database.SqlQuery<StorageTransferDTO>(sql).ToList();
            return dblist;
        }
        public List<WarehouseDTO> GetWarehouseSITE(int planID)
        {
            string sql = @"SELECT t1.*,t2.Rack_ID,t2.Storage_ID,t2.[Desc] WarehouseStorageDesc,t2.Warehouse_Storage_UID
                                    ,t3.Organization_Name BG_Organization, t4.Organization_Name FunPlant_Organization ,t5.Enum_Value  FROM 
                                    Warehouse t1 inner join Warehouse_Storage t2 on t1.Warehouse_UID=t2.Warehouse_UID
                                    inner join System_Organization t3 on t1.BG_Organization_UID=t3.Organization_UID
                                    inner join System_Organization t4 on t1.FunPlant_Organization_UID=t4.Organization_UID
                                    left join Enumeration t5 on t1.Warehouse_Type_UID=t5.Enum_UID";
            sql = string.Format(sql);
            var dblist = DataContext.Database.SqlQuery<WarehouseDTO>(sql).ToList();
            List<int> Plant_UIDs = GetOpType(planID).Select(o => o.Organization_UID).ToList();
            return dblist.Where(o=> Plant_UIDs.Contains(o.BG_Organization_UID)).ToList();
        }
    }
}
