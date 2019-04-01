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
    public interface IWarehouseStorageRepository : IRepository<Warehouse_Storage>
    {
        List<WarehouseDTO> GetByUid(int Warehouse_Storage_UID);
        List<WarehouseDTO> GetStinfo();

        List<WarehouseDTO> QueryWarStroge(int OPTypeID);
        List<WarehouseStorageDTO> GetAllInfo();
        List<WarehouseStorageDTO> GetAllWarehouseSt(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
    }
    public  class WarehouseStorageRepository: RepositoryBase<Warehouse_Storage>, IWarehouseStorageRepository
    {
        public WarehouseStorageRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public List<WarehouseDTO> GetByUid(int Warehouse_Storage_UID)
        {
            string sql = @"SELECT t1.*,t2.Rack_ID,t2.Storage_ID,t2.[Desc] WarehouseStorageDesc,t2.Warehouse_Storage_UID
                                    ,t3.Organization_Name BG_Organization, t4.Organization_Name FunPlant_Organization ,t5.Enum_Value Warehouse_Type  FROM 
                                    Warehouse t1 left join Warehouse_Storage t2 on t1.Warehouse_UID=t2.Warehouse_UID
                                    inner join System_Organization t3 on t1.BG_Organization_UID=t3.Organization_UID
                                    inner join System_Organization t4 on t1.FunPlant_Organization_UID=t4.Organization_UID
                                    left join Enumeration t5 on t1.Warehouse_Type_UID=t5.Enum_UID
                                    where t2.Warehouse_Storage_UID={0}";
            sql = string.Format(sql, Warehouse_Storage_UID);
            var dblist = DataContext.Database.SqlQuery<WarehouseDTO>(sql).ToList();


            return SetListWarehouseDTO(dblist);
        }
        public List<WarehouseDTO> SetListWarehouseDTO(List<WarehouseDTO> warehouses)
        {
            List<SystemOrgDTO> system_Organizations = GetPant();
            foreach (var item in warehouses)
            {
                // 设置厂区                            
                var systemOrgDTO = system_Organizations.Where(o => o.ChildOrg_UID == item.BG_Organization_UID).FirstOrDefault();
                if (systemOrgDTO != null)
                {
                    item.Plant_Organization_UID = systemOrgDTO.Organization_UID;
                    item.Plant = systemOrgDTO.Organization_Name;
                }
            }
            return warehouses;
        }
        public List<SystemOrgDTO> GetPant()
        {
            var sqlStr = @"SELECT* FROM dbo.System_Organization t1 INNER JOIN dbo.System_OrganizationBOM t2
                                        ON t1.Organization_UID=t2.ParentOrg_UID WHERE  t1.Organization_ID LIKE'%1000%'";
            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList();
            return dbList;
        }
        public List<WarehouseDTO> GetStinfo()
        {
            string sql = @"SELECT t1.*,t2.Rack_ID,t2.Storage_ID,t2.[Desc] WarehouseStorageDesc,t2.Warehouse_Storage_UID
                                    ,t3.Organization_Name BG_Organization, t4.Organization_Name FunPlant_Organization ,t5.Enum_Value  FROM 
                                    Warehouse t1 inner join Warehouse_Storage t2 on t1.Warehouse_UID=t2.Warehouse_UID
                                    inner join System_Organization t3 on t1.BG_Organization_UID=t3.Organization_UID
                                    inner join System_Organization t4 on t1.FunPlant_Organization_UID=t4.Organization_UID
                                    left join Enumeration t5 on t1.Warehouse_Type_UID=t5.Enum_UID";
            sql = string.Format(sql);
            var dblist = DataContext.Database.SqlQuery<WarehouseDTO>(sql).ToList();
            return dblist;
        }


        public List<WarehouseDTO> QueryWarStroge(int OPTypeID)
        {
            string sql = @"SELECT t1.*,t2.Rack_ID,t2.Storage_ID,t2.[Desc] WarehouseStorageDesc,t2.Warehouse_Storage_UID
                                    ,t3.Organization_Name BG_Organization, t4.Organization_Name FunPlant_Organization ,t5.Enum_Value  FROM 
                                    Warehouse t1 inner join Warehouse_Storage t2 on t1.Warehouse_UID=t2.Warehouse_UID
                                    inner join System_Organization t3 on t1.BG_Organization_UID=t3.Organization_UID
                                    inner join System_Organization t4 on t1.FunPlant_Organization_UID=t4.Organization_UID
                                    left join Enumeration t5 on t1.Warehouse_Type_UID=t5.Enum_UID where BG_Organization_UID={0}";
            sql = string.Format(sql, OPTypeID);
            var dblist = DataContext.Database.SqlQuery<WarehouseDTO>(sql).ToList();
            return dblist;
        }
        public List<WarehouseStorageDTO> GetAllInfo()
        {
            string sql = @"SELECT T1.*,T2.BG_Organization_UID, T2.FunPlant_Organization_UID,T2.Warehouse_ID,T2.Name_ZH,t2.Warehouse_Type_UID FROM Warehouse_Storage T1 INNER JOIN Warehouse T2 ON 
                                    T1.Warehouse_UID=T2.Warehouse_UID";
            sql = string.Format(sql);
            var dblist = DataContext.Database.SqlQuery<WarehouseStorageDTO>(sql).ToList();
            return dblist;
        }

        public List<WarehouseStorageDTO> GetAllWarehouseSt(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            string sql = @"SELECT T1.*,T2.BG_Organization_UID, T2.FunPlant_Organization_UID,T2.Warehouse_ID,T2.Name_ZH,t2.Warehouse_Type_UID FROM Warehouse_Storage T1 INNER JOIN Warehouse T2 ON 
                                    T1.Warehouse_UID=T2.Warehouse_UID  where 1=1 ";

            if (Plant_Organization_UID != 0)
            {
                sql += string.Format(@" AND T2.BG_Organization_UID IN (SELECT DISTINCT t2.Organization_UID FROM dbo.System_Project t2 WHERE t2.Organization_UID IN (SELECT A.Organization_UID FROM
                                        dbo.System_Organization A INNER JOIN dbo.System_OrganizationBOM B ON a.Organization_UID = B.ChildOrg_UID WHERE A.Organization_ID LIKE'2000%' AND B.ParentOrg_UID = {0}
                                        ))", Plant_Organization_UID);
            }
            if (BG_Organization_UID != 0)
            {
                sql += string.Format(@" AND T2.BG_Organization_UID ={0}", BG_Organization_UID);
            }

            if (FunPlant_Organization_UID != 0)
            {
                sql += string.Format(@" AND T2.FunPlant_Organization_UID ={0}", FunPlant_Organization_UID);
            }
            sql = string.Format(sql);
            var dblist = DataContext.Database.SqlQuery<WarehouseStorageDTO>(sql).ToList();
            return dblist;
        }
    }
}
