using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;

namespace PDMS.Data.Repository
{
    public interface IMaterialInfoRepository : IRepository<Material_Info>
    {
        IQueryable<MaterialInfoDTO> GetInfo(MaterialInfoDTO searchModel, Page page, out int totalcount);
        List<MaterialInfoDTO> DoExportMaterialReprot(string uids);
        List<MaterialInfoDTO> DoAllExportMaterialReprot(MaterialInfoDTO searchModel);
        List<MaterialInfoDTO> GetByUId(int EQPUser_Uid);
        string DeleteMaterial(int Material_Uid);
        string InsertItem(List<MaterialInfoDTO> dtolist);
        List<string> GetDepart(int Userid);
        List<string> GetUnitMat();
        MaterialInfoDTO GetMaterialByMaterialId(string id);
        List<MaterialInfoDTO> GetMaterialByMaterialId();
    }
    public class MaterialInfoRepository : RepositoryBase<Material_Info>, IMaterialInfoRepository
    {
        public MaterialInfoRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public IQueryable<MaterialInfoDTO> GetInfo(MaterialInfoDTO searchModel, Page page, out int totalcount)
        {
            var query = from material in DataContext.Material_Info
                        join warst in DataContext.Warehouse_Storage
                        on material.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        into temp
                        from aa in temp.DefaultIfEmpty()
                        join war in DataContext.Warehouse
                        on aa.Warehouse_UID equals war.Warehouse_UID
                        into temp2
                        from bb in temp2.DefaultIfEmpty()
                        select new MaterialInfoDTO
                        {
                            Material_Uid = material.Material_Uid,
                            Material_Id = material.Material_Id,
                            Material_Name = material.Material_Name,
                            Material_Types = material.Material_Types,
                            Classification = material.Classification,
                            Delivery_Date = material.Delivery_Date,
                            Unit_Price = material.Unit_Price,
                            Last_Qty = material.Last_Qty,
                            Modified_Date = material.Modified_Date,
                            Warehouse_ID = bb.Warehouse_ID,
                            Rack_ID = aa.Rack_ID,
                            Storage_ID = aa.Storage_ID,
                            Cost_Center = material.Cost_Center,
                            Maintenance_Cycle = (int)material.Maintenance_Cycle,
                            Material_Life = (int)material.Material_Life,
                            Requisitions_Cycle = (int)material.Requisitions_Cycle,
                            Sign_days = (int)material.Sign_days,
                            Daily_Consumption = (decimal)material.Daily_Consumption,
                            Monthly_Consumption = (int)material.Monthly_Consumption,
                            IsRework = material.IsRework,
                            Is_Enable = material.Is_Enable,
                            PlantId = (int)material.Organization_UID
                        };
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
            if (!string.IsNullOrWhiteSpace(searchModel.Classification))
                query = query.Where(m => m.Classification.Contains(searchModel.Classification));
            if (searchModel.Delivery_Date != null)
                query = query.Where(m => m.Delivery_Date == searchModel.Delivery_Date);
            if (searchModel.Unit_Price != null)
                query = query.Where(m => m.Unit_Price == searchModel.Unit_Price);
            if (searchModel.PlantId != null && searchModel.PlantId != 0)
                query = query.Where(m => m.PlantId == searchModel.PlantId);
            if (searchModel.Plant_OrganizationUID != null && searchModel.Plant_OrganizationUID != 0)
                query = query.Where(m => m.PlantId == searchModel.Plant_OrganizationUID);

            if (searchModel.IsCheck == "Y")
            {
                query = from eqptype in DataContext.EQP_Type
                        join eqpmat in DataContext.EQP_Material
                        on eqptype.EQP_Type_UID equals eqpmat.EQP_Type_UID
                        join material in DataContext.Material_Info
                        on eqpmat.Material_UID equals material.Material_Uid
                        join warst in DataContext.Warehouse_Storage
                        on material.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        into temp
                        from aa in temp.DefaultIfEmpty()
                        join war in DataContext.Warehouse
                        on aa.Warehouse_UID equals war.Warehouse_UID
                        into temp2
                        from bb in temp2.DefaultIfEmpty()
                        select new MaterialInfoDTO
                        {
                            Material_Uid = material.Material_Uid,
                            Material_Id = material.Material_Id,
                            Material_Name = material.Material_Name,
                            Material_Types = material.Material_Types,
                            Classification = material.Classification,
                            Delivery_Date = material.Delivery_Date,
                            Unit_Price = material.Unit_Price,
                            Last_Qty = material.Last_Qty,
                            Modified_Date = material.Modified_Date,
                            Warehouse_ID = bb.Warehouse_ID,
                            Rack_ID = aa.Rack_ID,
                            Storage_ID = aa.Storage_ID,
                            Cost_Center = material.Cost_Center,
                            Maintenance_Cycle = (int)material.Maintenance_Cycle,
                            Material_Life = (int)material.Material_Life,
                            Requisitions_Cycle = (int)material.Requisitions_Cycle,
                            Sign_days = (int)material.Sign_days,
                            Daily_Consumption = (decimal)material.Daily_Consumption,
                            Monthly_Consumption = (int)material.Monthly_Consumption,
                            IsRework = material.IsRework,
                            Is_Enable = material.Is_Enable,
                            BG_Organization_UID = eqptype.BG_Organization_UID,
                            FunPlant_Organization_UID = eqptype.FunPlant_Organization_UID,
                            PlantId = (int)material.Organization_UID
                        };
                query = query.Where(m => m.Material_Life == null || m.Material_Life == 0 || m.Maintenance_Cycle == null || m.Maintenance_Cycle == 0
                                || m.Requisitions_Cycle == null || m.Requisitions_Cycle == 0 || m.Sign_days == null || m.Sign_days == 0 || m.Daily_Consumption == null
                                || m.Daily_Consumption == 0);
                if (searchModel.BG_Organization_UID != null && searchModel.BG_Organization_UID != 0)
                    query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
                if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                    query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                    query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                    query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                    query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
                if (!string.IsNullOrWhiteSpace(searchModel.Classification))
                    query = query.Where(m => m.Classification.Contains(searchModel.Classification));
                if (searchModel.Delivery_Date != null)
                    query = query.Where(m => m.Delivery_Date == searchModel.Delivery_Date);
                if (searchModel.Unit_Price != null)
                    query = query.Where(m => m.Unit_Price == searchModel.Unit_Price);
                if (searchModel.PlantId != null && searchModel.PlantId != 0)
                    query = query.Where(m => m.PlantId == searchModel.PlantId);
                if (searchModel.Plant_OrganizationUID != null && searchModel.Plant_OrganizationUID != 0)
                    query = query.Where(m => m.PlantId == searchModel.Plant_OrganizationUID);
            }
            query = SetMaterialInfoPlantName(query.ToList());
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Material_Id).GetPage(page);
            return query;
        }

        public List<MaterialInfoDTO> DoExportMaterialReprot(string uids)
        {
            uids = "," + uids + ",";
            var query = from material in DataContext.Material_Info
                        join warst in DataContext.Warehouse_Storage
                        on material.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        into temp
                        from aa in temp.DefaultIfEmpty()
                        join war in DataContext.Warehouse
                        on aa.Warehouse_UID equals war.Warehouse_UID
                        into temp2
                        from bb in temp2.DefaultIfEmpty()
                        select new MaterialInfoDTO
                        {
                            Material_Uid = material.Material_Uid,
                            Material_Id = material.Material_Id,
                            Material_Name = material.Material_Name,
                            Material_Types = material.Material_Types,
                            Classification = material.Classification,
                            Delivery_Date = material.Delivery_Date,
                            Unit_Price = material.Unit_Price,
                            Modified_Date = material.Modified_Date,
                            Warehouse_ID = bb.Warehouse_ID,
                            Rack_ID = aa.Rack_ID,
                            Storage_ID = aa.Storage_ID,
                            Cost_Center = material.Cost_Center,
                            Maintenance_Cycle = (int)material.Maintenance_Cycle,
                            Material_Life = (int)material.Material_Life,
                            Requisitions_Cycle = (int)material.Requisitions_Cycle,
                            Sign_days = (int)material.Sign_days,
                            Daily_Consumption = (decimal)material.Daily_Consumption,
                            Monthly_Consumption = (int)material.Monthly_Consumption,
                            IsRework = material.IsRework,
                            Is_Enable = material.Is_Enable,
                            PlantId = (int)material.Organization_UID,
                            Last_Qty = material.Last_Qty
                        };
            query = query.Where(m => uids.Contains("," + m.Material_Uid + ","));
            query = SetMaterialInfoPlantName(query.ToList());
            return query.ToList();
        }
        public List<MaterialInfoDTO> DoAllExportMaterialReprot(MaterialInfoDTO searchModel)
        {
            var query = from material in DataContext.Material_Info
                        join warst in DataContext.Warehouse_Storage
                        on material.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        into temp
                        from aa in temp.DefaultIfEmpty()
                        join war in DataContext.Warehouse
                        on aa.Warehouse_UID equals war.Warehouse_UID
                        into temp2
                        from bb in temp2.DefaultIfEmpty()
                        select new MaterialInfoDTO
                        {
                            Material_Uid = material.Material_Uid,
                            Material_Id = material.Material_Id,
                            Material_Name = material.Material_Name,
                            Material_Types = material.Material_Types,
                            Classification = material.Classification,
                            Delivery_Date = material.Delivery_Date,
                            Unit_Price = material.Unit_Price,
                            Modified_Date = material.Modified_Date,
                            Warehouse_ID = bb.Warehouse_ID,
                            Rack_ID = aa.Rack_ID,
                            Storage_ID = aa.Storage_ID,
                            Cost_Center = material.Cost_Center,
                            Maintenance_Cycle = (int)material.Maintenance_Cycle,
                            Material_Life = (int)material.Material_Life,
                            Requisitions_Cycle = (int)material.Requisitions_Cycle,
                            Sign_days = (int)material.Sign_days,
                            Daily_Consumption = (decimal)material.Daily_Consumption,
                            Monthly_Consumption = (int)material.Monthly_Consumption,
                            IsRework = material.IsRework,
                            Is_Enable = material.Is_Enable,
                            PlantId = (int)material.Organization_UID,
                            Last_Qty = material.Last_Qty
                        };
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
            if (!string.IsNullOrWhiteSpace(searchModel.Classification))
                query = query.Where(m => m.Classification.Contains(searchModel.Classification));
            if (searchModel.Delivery_Date != null)
                query = query.Where(m => m.Delivery_Date == searchModel.Delivery_Date);
            if (searchModel.Unit_Price != null)
                query = query.Where(m => m.Unit_Price == searchModel.Unit_Price);
            if (searchModel.PlantId != null && searchModel.PlantId != 0)
                query = query.Where(m => m.PlantId == searchModel.PlantId);
            if (searchModel.Plant_OrganizationUID != null && searchModel.Plant_OrganizationUID != 0)
                query = query.Where(m => m.PlantId == searchModel.Plant_OrganizationUID);

            if (searchModel.IsCheck == "Y")
            {
                query = from eqptype in DataContext.EQP_Type
                        join eqpmat in DataContext.EQP_Material
                        on eqptype.EQP_Type_UID equals eqpmat.EQP_Type_UID
                        join material in DataContext.Material_Info
                        on eqpmat.Material_UID equals material.Material_Uid
                        join warst in DataContext.Warehouse_Storage
                        on material.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
                        into temp
                        from aa in temp.DefaultIfEmpty()
                        join war in DataContext.Warehouse
                        on aa.Warehouse_UID equals war.Warehouse_UID
                        into temp2
                        from bb in temp2.DefaultIfEmpty()
                        select new MaterialInfoDTO
                        {
                            Material_Uid = material.Material_Uid,
                            Material_Id = material.Material_Id,
                            Material_Name = material.Material_Name,
                            Material_Types = material.Material_Types,
                            Classification = material.Classification,
                            Delivery_Date = material.Delivery_Date,
                            Unit_Price = material.Unit_Price,
                            Modified_Date = material.Modified_Date,
                            Warehouse_ID = bb.Warehouse_ID,
                            Rack_ID = aa.Rack_ID,
                            Storage_ID = aa.Storage_ID,
                            Cost_Center = material.Cost_Center,
                            Maintenance_Cycle = (int)material.Maintenance_Cycle,
                            Material_Life = (int)material.Material_Life,
                            Requisitions_Cycle = (int)material.Requisitions_Cycle,
                            Sign_days = (int)material.Sign_days,
                            Daily_Consumption = (decimal)material.Daily_Consumption,
                            Monthly_Consumption = (int)material.Monthly_Consumption,
                            IsRework = material.IsRework,
                            Is_Enable = material.Is_Enable,
                            BG_Organization_UID = eqptype.BG_Organization_UID,
                            FunPlant_Organization_UID = eqptype.FunPlant_Organization_UID,
                            PlantId = (int)material.Organization_UID
                        };
                query = query.Where(m => m.Material_Life == null || m.Material_Life == 0 || m.Maintenance_Cycle == null || m.Maintenance_Cycle == 0
                                || m.Requisitions_Cycle == null || m.Requisitions_Cycle == 0 || m.Sign_days == null || m.Sign_days == 0 || m.Daily_Consumption == null
                                || m.Daily_Consumption == 0);
                if (searchModel.BG_Organization_UID != null && searchModel.BG_Organization_UID != 0)
                    query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
                if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                    query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                    query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                    query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
                if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                    query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
                if (!string.IsNullOrWhiteSpace(searchModel.Classification))
                    query = query.Where(m => m.Classification.Contains(searchModel.Classification));
                if (searchModel.Delivery_Date != null)
                    query = query.Where(m => m.Delivery_Date == searchModel.Delivery_Date);
                if (searchModel.Unit_Price != null)
                    query = query.Where(m => m.Unit_Price == searchModel.Unit_Price);
                if (searchModel.PlantId != null && searchModel.PlantId != 0)
                    query = query.Where(m => m.PlantId == searchModel.PlantId);
                if (searchModel.Plant_OrganizationUID != null && searchModel.Plant_OrganizationUID != 0)
                    query = query.Where(m => m.PlantId == searchModel.Plant_OrganizationUID);
            }
            query = SetMaterialInfoPlantName(query.ToList());
  
            return query.ToList();
        }

        //设置获取功能厂名称
        public IQueryable<MaterialInfoDTO> SetMaterialInfoPlantName(List<MaterialInfoDTO> MaterialInfos)
        {
            List<System_Organization> system_Organizations = DataContext.System_Organization.ToList();

            foreach (var item in MaterialInfos)
            {
                var system_Organization = system_Organizations.Where(o => o.Organization_UID == item.PlantId).FirstOrDefault();
                if (system_Organization != null)
                    item.PlantName = system_Organization.Organization_Name;
            }
            return MaterialInfos.AsQueryable();
        }




        public List<MaterialInfoDTO> GetByUId(int Material_Uid)
        {
            string sql = @"SELECT t1.*,isnull(IsRework,1) IsRework,isnull(t1.Is_Enable,1) Is_Enable,t2.Rack_ID,t2.Storage_ID,t3.Warehouse_ID,t3.BG_Organization_UID,t3.FunPlant_Organization_UID
                                 FROM dbo.Material_Info t1 left join Warehouse_Storage t2 
                                on t1.Warehouse_Storage_UID=t2.Warehouse_Storage_UID
                                left join Warehouse t3 on t2.Warehouse_UID=t3.Warehouse_UID where Material_Uid={0} ";
            sql = string.Format(sql, Material_Uid);
            var dblist = DataContext.Database.SqlQuery<MaterialInfoDTO>(sql).ToList();
            return SetMaterialInfoDTOPlantName(dblist);
        }

        public List<MaterialInfoDTO> SetMaterialInfoDTOPlantName(List<MaterialInfoDTO> MaterialInfoDTOs)
        {

            List<System_Organization> system_Organizations = DataContext.System_Organization.ToList();

            foreach (var item in MaterialInfoDTOs)
            {
                item.PlantName = system_Organizations.Where(o => o.Organization_UID == item.Organization_UID).FirstOrDefault().Organization_Name;
                item.PlantId = item.Organization_UID;
                item.Plant_OrganizationUID = item.Organization_UID;
            }

            return MaterialInfoDTOs;
        }

        public string DeleteMaterial(int Material_Uid)
        {
            try
            {
                //删除之前先判断该料号是否已经绑定了机台，如果已经绑定了机台，需要需要提醒用户该料号已绑定机台，需要先删除机台料号关系后，才能删除料号



                string sql = "delete Material_Info where Material_Uid={0}";
                sql = string.Format(sql, Material_Uid);
                DataContext.Database.ExecuteSqlCommand(sql);
                return "";
            }
            catch (Exception e)
            {
                return "删除料号失败:" +"该料号已经绑定了机台，或已经存在维修材料更改履历中，不能删除";
            }
        }

        public string InsertItem(List<MaterialInfoDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        if (dtolist[i].Material_Uid != 0)
                        {
                            var sql = string.Format(@"UPDATE dbo.Material_Info SET Material_Name=N'{0}',Material_Types=N'{1}',Assembly_Name=N'{2}',
                            Classification = N'{3}', Unit_Price ={4},Delivery_Date ={5},Modified_UID ={6},Modified_Date = N'{7}',
                            Warehouse_Storage_UID={8},Cost_Center=N'{9}',Maintenance_Cycle={10},Material_Life={11},
                            Requisitions_Cycle={12},Sign_days={13},Daily_Consumption={14},Monthly_Consumption={15},
                            IsRework={16},Is_Enable={17},Organization_UID={19} WHERE Material_Uid = {18}",
                            dtolist[i].Material_Name,
                            dtolist[i].Material_Types,
                            dtolist[i].Assembly_Name,
                            dtolist[i].Classification,
                            dtolist[i].Unit_Price,
                            dtolist[i].Delivery_Date,
                            dtolist[i].Modified_UID,
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            dtolist[i].Warehouse_Storage_UID,
                            dtolist[i].Cost_Center,
                            dtolist[i].Maintenance_Cycle,
                            dtolist[i].Material_Life,
                            dtolist[i].Requisitions_Cycle,
                            dtolist[i].Sign_days,
                            dtolist[i].Daily_Consumption,
                            dtolist[i].Monthly_Consumption,
                            dtolist[i].IsRework == true ? 1 : 0,
                            dtolist[i].Is_Enable == true ? 1 : 0,
                            dtolist[i].Material_Uid,
                            dtolist[i].Organization_UID
                          
                            );
                            DataContext.Database.ExecuteSqlCommand(sql);
                            //var sql = string.Format(@"UPDATE dbo.Material_Info SET Material_Name=N'{0}',Material_Types=N'{1}',Assembly_Name=N'{2}',
                            //Classification = N'{3}', Unit_Price ={4},Delivery_Date ={5},Modified_UID ={6},
                            //Modified_Date = N'{7}' WHERE Material_Uid = {8}",
                            //    dtolist[i].Material_Name,
                            //    dtolist[i].Material_Types,
                            //    dtolist[i].Assembly_Name,
                            //    dtolist[i].Classification,
                            //    dtolist[i].Unit_Price,
                            //    dtolist[i].Delivery_Date,
                            //    dtolist[i].Modified_UID,
                            //    DateTime.Now.ToString(),
                            //    dtolist[i].Material_Uid);
                            //DataContext.Database.ExecuteSqlCommand(sql);

                        }
                        else
                        {
                            var sql = string.Format(@"insert into Material_Info values (N'{0}',N'{1}',N'{2}',N'{3}',N'{4}',{5},N'{6}',{7},{8},
                                               {9},{10},{11},{12},{13},{14},{15},{16},{17},N'{18}',{19},{20} )",
                            dtolist[i].Material_Id,
                            dtolist[i].Material_Name,
                            dtolist[i].Material_Types,
                            dtolist[i].Assembly_Name,
                            dtolist[i].Classification,
                            dtolist[i].Warehouse_Storage_UID,
                            dtolist[i].Cost_Center,
                            dtolist[i].Unit_Price,
                            dtolist[i].Delivery_Date,
                            dtolist[i].Maintenance_Cycle,
                            dtolist[i].Material_Life,
                            dtolist[i].Requisitions_Cycle,
                            dtolist[i].Sign_days,
                            dtolist[i].Daily_Consumption,
                            dtolist[i].Monthly_Consumption,
                            dtolist[i].IsRework == true ? 1 : 0,
                            dtolist[i].Is_Enable == true ? 1 : 0,
                            dtolist[i].Modified_UID,
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            dtolist[i].Organization_UID,
                            dtolist[i].Last_Qty
                            );
                            DataContext.Database.ExecuteSqlCommand(sql);

                            //var sql = string.Format("insert into Material_Info values (N'{0}',N'{1}',N'{2}',N'{3}',N'{4}',{5},N'{6}',{7},N'{8}')",
                            //        dtolist[i].Material_Id,
                            //        dtolist[i].Material_Name,
                            //        dtolist[i].Material_Types,
                            //        dtolist[i].Assembly_Name,
                            //        dtolist[i].Classification,
                            //        dtolist[i].Unit_Price,
                            //        dtolist[i].Delivery_Date,
                            //        dtolist[i].Modified_UID,
                            //        DateTime.Now.ToString());
                            //DataContext.Database.ExecuteSqlCommand(sql);
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    result = "Error" + ex;
                }
                return result;
            }
        }

        public List<string> GetDepart(int Userid)
        {
            string sql = @"SELECT t2.Organization_Name FROM  dbo.System_UserOrg t1 left JOIN dbo.System_Organization t2
                        ON t1.Organization_UID=t2.Organization_UID WHERE t1.Account_UID={0}";
            sql = string.Format(sql, Userid);
            var dblist = DataContext.Database.SqlQuery<string>(sql).ToList();
            return dblist;
        }

        public List<string> GetUnitMat()
        {
            string sql = @"SELECT DISTINCT Material_Name FROM dbo.Material_Info ";
            var dblist = DataContext.Database.SqlQuery<string>(sql).ToList();
            return dblist;
        }

        public List<MaterialInfoDTO> GetByUid(int Material_Uid)
        {
            string sql = @"SELECT * FROM dbo.Material_Info where Material_Uid={0} ";
            sql = string.Format(sql, Material_Uid);
            var dblist = DataContext.Database.SqlQuery<MaterialInfoDTO>(sql).ToList();
            return dblist;
        }

        public MaterialInfoDTO GetMaterialByMaterialId(string id)
        {
            string sql = @"SELECT * FROM dbo.Material_Info where Material_Id='{0}' ";
            sql = string.Format(sql, id);
            var dblist = DataContext.Database.SqlQuery<MaterialInfoDTO>(sql).FirstOrDefault();
            return dblist;
        }

        public List<MaterialInfoDTO> GetMaterialByMaterialId()
        {
            string sql = @"SELECT * FROM dbo.Material_Info ";
            sql = string.Format(sql);
            var dblist = DataContext.Database.SqlQuery<MaterialInfoDTO>(sql).ToList();
            return dblist;
        }
    }
}
