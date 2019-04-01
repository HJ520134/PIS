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
    public interface IMaterialInventoryRepository : IRepository<Material_Inventory>
    {
        List<MaterialInventoryDTO> GetAllInfo();
        List<MaterialInventoryDTO> GetSingleMatinventory(int Material_Uid, int Warehouse_Storage_UID);
        List<MaterialInventoryDTO> GetMatByOutType(int Out_Type_Uid, int FunplantUid);
        List<MaterialInventoryDTO> GetWarStByMat(int Material_Uid);
        List<MaterialInventoryDTO> GetWarStByMatCheck(int Material_Uid);
        List<MaterialInventoryDTO> GetMatinventoryByMat(int Material_Uid);
        IQueryable<MaterialInventoryDTO> GetInfo(MaterialInventoryDTO searchModel, Page page, out int totalcount);
        IQueryable<MaterialInventoryDTO> GetDetailInfo(MaterialInventoryDTO searchModel, Page page, out int totalcount);

        List<MaterialInventoryDTO> DoExportMaterialInventoryReprot(string uids);
        List<MaterialInventoryDTO> DoAllExportMaterialInventoryReprot(MaterialInventoryDTO searchModel);

    }
    public class MaterialInventoryRepository: RepositoryBase<Material_Inventory>, IMaterialInventoryRepository
    {
        public MaterialInventoryRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<MaterialInventoryDTO> GetAllInfo()
        {
            string sql = @"SELECT distinct t2.Material_Uid,t2.Material_Id,T2.Material_Name,T2.Material_Types 
                                        FROM MATERIAL_INVENTORY T1 INNER JOIN Material_Info T2
                                        ON T1.MATERIAL_UID=T2.MATERIAL_UID";
            var dblist = DataContext.Database.SqlQuery<MaterialInventoryDTO>(sql).ToList();
            return dblist;
        }

        public List<MaterialInventoryDTO> GetSingleMatinventory(int Material_Uid, int Warehouse_Storage_UID)
        {
            string sql = @"with one as(
                                SELECT t1.*,t2.Material_Id,Material_Name,Material_Types,Warehouse_ID,Rack_ID,Storage_ID
                                FROM Material_Inventory t1
                                inner join Material_Info t2 on t1.Material_Uid=t2.Material_Uid
                                inner join Warehouse_Storage t3 on t1.Warehouse_Storage_UID=t3.Warehouse_Storage_UID
                                inner join Warehouse t4 on t3.Warehouse_UID=t4.Warehouse_UID),
                                two as (
                                select T2.Material_Uid,t2.Warehouse_Storage_UID,sum(t2.Outbound_Qty) Be_Out_Qty1 from 
                                Storage_Outbound_M t1 inner join Storage_Outbound_D T2
                                ON T1.Storage_Outbound_M_UID=T2.Storage_Outbound_M_UID INNER JOIN Enumeration T3
                                ON T1.Status_UID=t3.Enum_UID WHERE T3.Enum_Value=N'未审核' group by Material_Uid,
                                Warehouse_Storage_UID)
                                select one.*,ISNULL(Be_Out_Qty1,0) Be_Out_Qty from one left join two on one.Material_Uid=two.Material_Uid
                                and one.Warehouse_Storage_UID=two.Warehouse_Storage_UID
                                WHERE one.Material_Uid={0} AND one.Warehouse_Storage_UID={1}";
            sql = string.Format(sql, Material_Uid, Warehouse_Storage_UID);
            var dblist = DataContext.Database.SqlQuery<MaterialInventoryDTO>(sql).ToList();
            return dblist;
        }
        public List<MaterialInventoryDTO> GetMatByOutType(int Out_Type_Uid, int FunplantUid)
        {
            string sql = @"select distinct t1.Material_Uid,Material_Id,Material_Name,Material_Types,t1.Warehouse_Storage_UID,t1.Inventory_Qty from
                        Material_Inventory t1 inner join Warehouse_Storage t2
                        on t1.Warehouse_Storage_UID = t2.Warehouse_Storage_UID inner join Warehouse
                          t3 on t2.Warehouse_UID = t3.Warehouse_UID inner join Material_Info t4
                        on t1.Material_Uid = t4.Material_Uid  inner join System_OrganizationBOM
                        t5 on t3.BG_Organization_UID=t5.ChildOrg_UID where 1=1";
            //fky2017/11/13
            // if (Out_Type_Uid == 388 || Out_Type_Uid == 408)  //不良品时只能选择MRB仓
            if (Out_Type_Uid == 417 || Out_Type_Uid == 433)  //不良品时只能选择MRB仓
            {
                //fky2017/11/13
                //sql += " and Warehouse_Type_UID=398 ";
                sql += " and Warehouse_Type_UID=418 ";
            }
            else
            {
                sql += " and Warehouse_Type_UID=405 ";
            }
            if (FunplantUid != 0)
                sql += " and ParentOrg_UID=" + FunplantUid;
            var dblist = DataContext.Database.SqlQuery<MaterialInventoryDTO>(sql).ToList();
            return dblist;
        }

        public List<MaterialInventoryDTO> GetMatinventoryByMat(int Material_Uid)
        {
            string sql = @"SELECT t1.Material_UID,t1.Inventory_Qty,t1.Warehouse_Storage_UID
	                              ,t2.Material_Id,t2.Material_Name,t2.Material_Types
	                              ,t4.Warehouse_Type_UID,t4.Warehouse_ID,t3.Rack_ID,t3.Storage_ID
	                              ,isnull(Be_Out_Qty,0) Be_Out_Qty
                            FROM Material_Inventory t1
                            inner join Material_Info t2 on t1.Material_Uid=t2.Material_Uid
                            inner join Warehouse_Storage t3 on t1.Warehouse_Storage_UID=t3.Warehouse_Storage_UID
                            inner join Warehouse t4 on t3.Warehouse_UID=t4.Warehouse_UID
                            left join (select t6.Material_UID,t6.Warehouse_Storage_UID,sum(t6.Outbound_Qty) Be_Out_Qty
		                               from Storage_Outbound_M t5 
		                               left join Storage_Outbound_D t6 on t5.Storage_Outbound_M_UID=t6.Storage_Outbound_M_UID
		                               where t5.Status_UID = 407
		                               group by t6.Material_UID,t6.Warehouse_Storage_UID
		                              ) t7 on t1.Material_UID = t7.Material_UID and t1.Warehouse_Storage_UID=t7.Warehouse_Storage_UID
                            WHERE t1.Inventory_Qty<>0 and t1.Material_Uid={0};";
            sql = string.Format(sql, Material_Uid);
            var dblist = DataContext.Database.SqlQuery<MaterialInventoryDTO>(sql).ToList();
            return dblist;
        }
        public List<MaterialInventoryDTO> GetWarStByMatCheck(int Material_Uid)
        {
            string sql = @"select * from Material_Inventory t1 inner join Warehouse_Storage t2
                                on t1.Warehouse_Storage_UID=t2.Warehouse_Storage_UID inner join Warehouse
                                t3 on t2.Warehouse_UID=t3.Warehouse_UID ";

            //string sql = @"select * from Material_Inventory t1 inner join Warehouse_Storage t2
            //                    on t1.Warehouse_Storage_UID=t2.Warehouse_Storage_UID inner join Warehouse
            //                    t3 on t2.Warehouse_UID=t3.Warehouse_UID WHERE t1.Inventory_Qty<>0 ";
            if (Material_Uid != 0)
            {
                sql = sql + " WHERE   Material_Uid ={0} ";
                sql = string.Format(sql, Material_Uid);
            }
            var dblist = DataContext.Database.SqlQuery<MaterialInventoryDTO>(sql).ToList();
            return SetBGandFunplant(dblist);
        }
        public List<MaterialInventoryDTO> GetWarStByMat(int Material_Uid)
        {
            //string sql = @"select * from Material_Inventory t1 inner join Warehouse_Storage t2
            //                    on t1.Warehouse_Storage_UID=t2.Warehouse_Storage_UID inner join Warehouse
            //                    t3 on t2.Warehouse_UID=t3.Warehouse_UID WHERE Material_Uid={0}";

            string sql = @"select * from Material_Inventory t1 inner join Warehouse_Storage t2
                                on t1.Warehouse_Storage_UID=t2.Warehouse_Storage_UID inner join Warehouse
                                t3 on t2.Warehouse_UID=t3.Warehouse_UID WHERE t1.Inventory_Qty<>0 ";
            if (Material_Uid != 0)
            {
                sql = sql + " AND   Material_Uid ={0} ";
                sql = string.Format(sql, Material_Uid);
            }        
            var dblist = DataContext.Database.SqlQuery<MaterialInventoryDTO>(sql).ToList();
            return SetBGandFunplant(dblist);
        }

        public List<MaterialInventoryDTO> SetBGandFunplant(List<MaterialInventoryDTO> MaterialInventoryDTOs)
        {
            var System_Organizations = DataContext.System_Organization.ToList();
            foreach (var item in MaterialInventoryDTOs)
            {
                item.BG = System_Organizations.FirstOrDefault(o => o.Organization_UID == item.BG_Organization_UID).Organization_Name;
                item.Funplant = System_Organizations.FirstOrDefault(o => o.Organization_UID == item.FunPlant_Organization_UID).Organization_Name;
            }
            return MaterialInventoryDTOs;
        }


        public IQueryable<MaterialInventoryDTO> GetInfo(MaterialInventoryDTO searchModel, Page page, out int totalcount)
        {
            if (searchModel.Warehouse_Storage_UID == 0)
            {
                var query1 = from M in DataContext.Material_Inventory
                             join warst in DataContext.Warehouse_Storage
                             on M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                             join war in DataContext.Warehouse
                             on warst.Warehouse_UID equals war.Warehouse_UID
                             group M by new
                             {
                                 M.Material_UID,
                                 war.BG_Organization_UID,
                                 war.FunPlant_Organization_UID,
                                 war.Warehouse_Type_UID
                             } into temp
                             select new
                             {
                                 matuid = temp.Key.Material_UID,
                                 bguid = temp.Key.BG_Organization_UID,
                                 funplantuid = temp.Key.FunPlant_Organization_UID,
                                 typeuid = temp.Key.Warehouse_Type_UID,
                                 totalqty = temp.Sum(m => m.Inventory_Qty),
                                 maxuid = temp.Max(m => m.Material_Inventory_UID)
                             };
                var query = from M in query1
                            join mat in DataContext.Material_Info on
                            M.matuid equals mat.Material_Uid
                            join bgorg in DataContext.System_Organization
                            on M.bguid equals bgorg.Organization_UID
                            join funplantorg in DataContext.System_Organization
                            on M.funplantuid equals funplantorg.Organization_UID
                            join typeenum in DataContext.Enumeration
                            on M.typeuid equals typeenum.Enum_UID
                            join newm in DataContext.Material_Inventory
                            on M.maxuid equals newm.Material_Inventory_UID
                            join users in DataContext.System_Users
                            on newm.Modified_UID equals users.Account_UID
                            select new MaterialInventoryDTO
                            {
                                Material_Inventory_UID = M.maxuid,
                                BG = bgorg.Organization_Name,
                                Funplant = funplantorg.Organization_Name,
                                Warehouse_Type = typeenum.Enum_Value,
                                Material_Id = mat.Material_Id,
                                Material_Name = mat.Material_Name,
                                Material_Types = mat.Material_Types,
                                Total_Qty = M.totalqty,
                                ModifyUser = users.User_Name,
                                Modified_Date = newm.Modified_Date,
                                Warehouse_Type_UID = M.typeuid,
                                BG_Organization_UID = M.bguid,
                                FunPlant_OrganizationUID = M.funplantuid,
                                Warehouse_Storage_UID = newm.Warehouse_Storage_UID
                            };
                if (searchModel.BG_Organization_UID != 0)
                    query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
                if (searchModel.FunPlant_Organization_UID != 0)
                    query = query.Where(m => m.FunPlant_OrganizationUID == searchModel.FunPlant_Organization_UID);
                if (searchModel.Warehouse_Type_UID != 0)
                    query = query.Where(m => m.Warehouse_Type_UID == searchModel.Warehouse_Type_UID);
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                    query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                    query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                    query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
                if (searchModel.Warehouse_Storage_UID != 0)
                    query = query.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);
                List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
                if (Plant_UIDs.Count > 0)
                {
                    query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
                }
                totalcount = query.Count();
                query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
                return query;
            }
            else
            {
                var query1 = from M in DataContext.Material_Inventory
                             join warst in DataContext.Warehouse_Storage
                             on M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                             join war in DataContext.Warehouse
                             on warst.Warehouse_UID equals war.Warehouse_UID
                             where warst.Warehouse_Storage_UID==searchModel.Warehouse_Storage_UID
                             group M by new
                             {
                                 M.Material_UID,
                                 war.BG_Organization_UID,
                                 war.FunPlant_Organization_UID,
                                 war.Warehouse_Type_UID
                             } into temp
                             select new
                             {
                                 matuid = temp.Key.Material_UID,
                                 bguid = temp.Key.BG_Organization_UID,
                                 funplantuid = temp.Key.FunPlant_Organization_UID,
                                 typeuid = temp.Key.Warehouse_Type_UID,
                                 totalqty = temp.Sum(m => m.Inventory_Qty),
                                 maxuid = temp.Max(m => m.Material_Inventory_UID)
                             };
                var query = from M in query1
                            join mat in DataContext.Material_Info on
                            M.matuid equals mat.Material_Uid
                            join bgorg in DataContext.System_Organization
                            on M.bguid equals bgorg.Organization_UID
                            join funplantorg in DataContext.System_Organization
                            on M.funplantuid equals funplantorg.Organization_UID
                            join typeenum in DataContext.Enumeration
                            on M.typeuid equals typeenum.Enum_UID
                            join newm in DataContext.Material_Inventory
                            on M.maxuid equals newm.Material_Inventory_UID
                            join users in DataContext.System_Users
                            on newm.Modified_UID equals users.Account_UID
                            select new MaterialInventoryDTO
                            {
                                Material_Inventory_UID = M.maxuid,
                                BG = bgorg.Organization_Name,
                                Funplant = funplantorg.Organization_Name,
                                Warehouse_Type = typeenum.Enum_Value,
                                Material_Id = mat.Material_Id,
                                Material_Name = mat.Material_Name,
                                Material_Types = mat.Material_Types,
                                Total_Qty = M.totalqty,
                                ModifyUser = users.User_Name,
                                Modified_Date = newm.Modified_Date,
                                Warehouse_Type_UID = M.typeuid,
                                BG_Organization_UID = M.bguid,
                                FunPlant_OrganizationUID = M.funplantuid,
                                Warehouse_Storage_UID = newm.Warehouse_Storage_UID
                            };
                if (searchModel.BG_Organization_UID != 0)
                    query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
                if (searchModel.FunPlant_Organization_UID != 0)
                    query = query.Where(m => m.FunPlant_OrganizationUID == searchModel.FunPlant_Organization_UID);
                if (searchModel.Warehouse_Type_UID != 0)
                    query = query.Where(m => m.Warehouse_Type_UID == searchModel.Warehouse_Type_UID);
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                    query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                    query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                    query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
                if (searchModel.Warehouse_Storage_UID != 0)
                    query = query.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);

                List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
                if (Plant_UIDs.Count > 0)
                {
                    query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
                }

                totalcount = query.Count();
                query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
                return query;
            }

        }

        public List<MaterialInventoryDTO> DoExportMaterialInventoryReprot(string uids)
        {
            uids = "," + uids + ",";
            var query1 = from M in DataContext.Material_Inventory
                         join warst in DataContext.Warehouse_Storage
                         on M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                         join war in DataContext.Warehouse
                         on warst.Warehouse_UID equals war.Warehouse_UID
                         group M by new
                         {
                             M.Material_UID,
                             war.BG_Organization_UID,
                             war.FunPlant_Organization_UID,
                             war.Warehouse_Type_UID
                         } into temp
                         select new
                         {
                             matuid = temp.Key.Material_UID,
                             bguid = temp.Key.BG_Organization_UID,
                             funplantuid = temp.Key.FunPlant_Organization_UID,
                             typeuid = temp.Key.Warehouse_Type_UID,
                             totalqty = temp.Sum(m => m.Inventory_Qty),
                             maxuid = temp.Max(m => m.Material_Inventory_UID)
                         };
            var query = from M in query1
                        join mat in DataContext.Material_Info on
                        M.matuid equals mat.Material_Uid
                        join bgorg in DataContext.System_Organization
                        on M.bguid equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on M.funplantuid equals funplantorg.Organization_UID
                        join typeenum in DataContext.Enumeration
                        on M.typeuid equals typeenum.Enum_UID
                        join newm in DataContext.Material_Inventory
                        on M.maxuid equals newm.Material_Inventory_UID
                        join users in DataContext.System_Users
                        on newm.Modified_UID equals users.Account_UID
                        select new MaterialInventoryDTO
                        {
                            Material_Inventory_UID = M.maxuid,
                            BG = bgorg.Organization_Name,
                            Funplant = funplantorg.Organization_Name,
                            Warehouse_Type = typeenum.Enum_Value,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Total_Qty = M.totalqty,
                            ModifyUser = users.User_Name,
                            Modified_Date = newm.Modified_Date,
                            Warehouse_Type_UID = M.typeuid,
                            BG_Organization_UID = M.bguid,
                            FunPlant_OrganizationUID = M.funplantuid,
                            Warehouse_Storage_UID = newm.Warehouse_Storage_UID,
                            Material_Uid = mat.Material_Uid
                        };

            query = query.Where(m => uids.Contains("," + m.Material_Inventory_UID + ","));

            return SetMaterialInventoryDetails(query.ToList());

        }

        public List<MaterialInventoryDTO> DoAllExportMaterialInventoryReprot(MaterialInventoryDTO searchModel)
        {
            if (searchModel.Warehouse_Storage_UID == 0)
            {
                var query1 = from M in DataContext.Material_Inventory
                             join warst in DataContext.Warehouse_Storage
                             on M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                             join war in DataContext.Warehouse
                             on warst.Warehouse_UID equals war.Warehouse_UID
                             group M by new
                             {
                                 M.Material_UID,
                                 war.BG_Organization_UID,
                                 war.FunPlant_Organization_UID,
                                 war.Warehouse_Type_UID
                             } into temp
                             select new
                             {
                                 matuid = temp.Key.Material_UID,
                                 bguid = temp.Key.BG_Organization_UID,
                                 funplantuid = temp.Key.FunPlant_Organization_UID,
                                 typeuid = temp.Key.Warehouse_Type_UID,
                                 totalqty = temp.Sum(m => m.Inventory_Qty),
                                 maxuid = temp.Max(m => m.Material_Inventory_UID)
                             };
                var query = from M in query1
                            join mat in DataContext.Material_Info on
                            M.matuid equals mat.Material_Uid
                            join bgorg in DataContext.System_Organization
                            on M.bguid equals bgorg.Organization_UID
                            join funplantorg in DataContext.System_Organization
                            on M.funplantuid equals funplantorg.Organization_UID
                            join typeenum in DataContext.Enumeration
                            on M.typeuid equals typeenum.Enum_UID
                            join newm in DataContext.Material_Inventory
                            on M.maxuid equals newm.Material_Inventory_UID
                            join users in DataContext.System_Users
                            on newm.Modified_UID equals users.Account_UID
                            select new MaterialInventoryDTO
                            {
                                Material_Inventory_UID = M.maxuid,
                                BG = bgorg.Organization_Name,
                                Funplant = funplantorg.Organization_Name,
                                Warehouse_Type = typeenum.Enum_Value,
                                Material_Id = mat.Material_Id,
                                Material_Name = mat.Material_Name,
                                Material_Types = mat.Material_Types,
                                Total_Qty = M.totalqty,
                                ModifyUser = users.User_Name,
                                Modified_Date = newm.Modified_Date,
                                Warehouse_Type_UID = M.typeuid,
                                BG_Organization_UID = M.bguid,
                                FunPlant_OrganizationUID = M.funplantuid,
                                Warehouse_Storage_UID = newm.Warehouse_Storage_UID,
                                Material_Uid = mat.Material_Uid
                            };
                if (searchModel.BG_Organization_UID != 0)
                    query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
                if (searchModel.FunPlant_OrganizationUID != 0)
                    query = query.Where(m => m.FunPlant_OrganizationUID == searchModel.FunPlant_OrganizationUID);
                if (searchModel.Warehouse_Type_UID != 0)
                    query = query.Where(m => m.Warehouse_Type_UID == searchModel.Warehouse_Type_UID);
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                    query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                    query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                    query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
                if (searchModel.Warehouse_Storage_UID != 0)
                    query = query.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);
                List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
                if (Plant_UIDs.Count > 0)
                {
                    query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
                }

                return SetMaterialInventoryDetails(query.ToList());
            }
            else
            {
                var query1 = from M in DataContext.Material_Inventory
                             join warst in DataContext.Warehouse_Storage
                             on M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                             join war in DataContext.Warehouse
                             on warst.Warehouse_UID equals war.Warehouse_UID
                             where warst.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID
                             group M by new
                             {
                                 M.Material_UID,
                                 war.BG_Organization_UID,
                                 war.FunPlant_Organization_UID,
                                 war.Warehouse_Type_UID
                             } into temp
                             select new
                             {
                                 matuid = temp.Key.Material_UID,
                                 bguid = temp.Key.BG_Organization_UID,
                                 funplantuid = temp.Key.FunPlant_Organization_UID,
                                 typeuid = temp.Key.Warehouse_Type_UID,
                                 totalqty = temp.Sum(m => m.Inventory_Qty),
                                 maxuid = temp.Max(m => m.Material_Inventory_UID)
                             };
                var query = from M in query1
                            join mat in DataContext.Material_Info on
                            M.matuid equals mat.Material_Uid
                            join bgorg in DataContext.System_Organization
                            on M.bguid equals bgorg.Organization_UID
                            join funplantorg in DataContext.System_Organization
                            on M.funplantuid equals funplantorg.Organization_UID
                            join typeenum in DataContext.Enumeration
                            on M.typeuid equals typeenum.Enum_UID
                            join newm in DataContext.Material_Inventory
                            on M.maxuid equals newm.Material_Inventory_UID
                            join users in DataContext.System_Users
                            on newm.Modified_UID equals users.Account_UID
                            select new MaterialInventoryDTO
                            {
                                Material_Inventory_UID = M.maxuid,
                                BG = bgorg.Organization_Name,
                                Funplant = funplantorg.Organization_Name,
                                Warehouse_Type = typeenum.Enum_Value,
                                Material_Id = mat.Material_Id,
                                Material_Name = mat.Material_Name,
                                Material_Types = mat.Material_Types,
                                Total_Qty = M.totalqty,
                                ModifyUser = users.User_Name,
                                Modified_Date = newm.Modified_Date,
                                Warehouse_Type_UID = M.typeuid,
                                BG_Organization_UID = M.bguid,
                                FunPlant_OrganizationUID = M.funplantuid,
                                Warehouse_Storage_UID = newm.Warehouse_Storage_UID,
                                Material_Uid= mat.Material_Uid
                            };
                if (searchModel.BG_Organization_UID != 0)
                    query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
                if (searchModel.FunPlant_OrganizationUID != 0)
                    query = query.Where(m => m.FunPlant_OrganizationUID == searchModel.FunPlant_OrganizationUID);
                if (searchModel.Warehouse_Type_UID != 0)
                    query = query.Where(m => m.Warehouse_Type_UID == searchModel.Warehouse_Type_UID);
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                    query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                    query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                    query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
                if (searchModel.Warehouse_Storage_UID != 0)
                    query = query.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);

                List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
                if (Plant_UIDs.Count > 0)
                {
                    query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
                }
                return SetMaterialInventoryDetails(query.ToList());
            }
        }

        public List<MaterialInventoryDTO> SetMaterialInventoryDetails(List<MaterialInventoryDTO> materialInventoryDTOs)
        {
            var query = from M in DataContext.Material_Inventory
            join mat in DataContext.Material_Info on
            M.Material_UID equals mat.Material_Uid
            join warst in DataContext.Warehouse_Storage
            on M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
            join war in DataContext.Warehouse
            on warst.Warehouse_UID equals war.Warehouse_UID
            join bgorg in DataContext.System_Organization
            on war.BG_Organization_UID equals bgorg.Organization_UID
            join funplantorg in DataContext.System_Organization
            on war.FunPlant_Organization_UID equals funplantorg.Organization_UID
            join typeenum in DataContext.Enumeration
            on war.Warehouse_Type_UID equals typeenum.Enum_UID
            join users in DataContext.System_Users
            on M.Modified_UID equals users.Account_UID
            select new MaterialInventoryDetailDTO
            {
                Material_Inventory_UID = M.Material_Inventory_UID,
                BG = bgorg.Organization_Name,
                Funplant = funplantorg.Organization_Name,
                Warehouse_Type = typeenum.Enum_Value,
                Material_Id = mat.Material_Id,
                Material_Name = mat.Material_Name,
                Material_Types = mat.Material_Types,
                Warehouse_ID = war.Warehouse_ID,
                Rack_ID = warst.Rack_ID,
                Storage_ID = warst.Storage_ID,
                Inventory_Qty = M.Inventory_Qty,
                ModifyUser = users.User_Name,
                Modified_Date = M.Modified_Date,
                BG_Organization_UID = war.BG_Organization_UID,
                FunPlant_OrganizationUID = war.FunPlant_Organization_UID,
                Material_Uid = mat.Material_Uid,
                Warehouse_Type_UID = war.Warehouse_Type_UID,
                Desc = M.Desc
            };
            List<MaterialInventoryDetailDTO> materialInventoryDetailDTOs = query.ToList();
            foreach (var item in materialInventoryDTOs)
            {
                item.MaterialInventoryDetails = materialInventoryDetailDTOs.Where(o => o.Material_Uid == item.Material_Uid&&o.FunPlant_OrganizationUID == item.FunPlant_OrganizationUID).ToList();
            }
            return materialInventoryDTOs;
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

        public IQueryable<MaterialInventoryDTO> GetDetailInfo(MaterialInventoryDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Material_Inventory
                        join mat in DataContext.Material_Info on
                        M.Material_UID equals mat.Material_Uid
                        join warst in DataContext.Warehouse_Storage
                        on M.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        join war in DataContext.Warehouse
                        on warst.Warehouse_UID equals war.Warehouse_UID
                        join bgorg in DataContext.System_Organization
                        on war.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on war.FunPlant_Organization_UID equals funplantorg.Organization_UID
                        join typeenum in DataContext.Enumeration
                        on war.Warehouse_Type_UID equals typeenum.Enum_UID
                        join users in DataContext.System_Users
                        on M.Modified_UID equals users.Account_UID
                        select new MaterialInventoryDTO
                        {
                            Material_Inventory_UID = M.Material_Inventory_UID,
                            BG = bgorg.Organization_Name,
                            Funplant = funplantorg.Organization_Name,
                            Warehouse_Type = typeenum.Enum_Value,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Warehouse_ID = war.Warehouse_ID,
                            Rack_ID = warst.Rack_ID,
                            Storage_ID = warst.Storage_ID,
                            Inventory_Qty = M.Inventory_Qty,
                            ModifyUser = users.User_Name,
                            Modified_Date = M.Modified_Date,
                            BG_Organization_UID = war.BG_Organization_UID,
                            FunPlant_OrganizationUID = war.FunPlant_Organization_UID,
                            Material_Uid = mat.Material_Uid,
                            Warehouse_Type_UID = war.Warehouse_Type_UID,
                            Desc = M.Desc
                        };
            query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID & m.FunPlant_OrganizationUID == searchModel.FunPlant_OrganizationUID
                                                    & m.Material_Uid == searchModel.Material_Uid & m.Warehouse_Type_UID == searchModel.Warehouse_Type_UID);
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;
        }

    }
}
