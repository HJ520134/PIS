using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using System.Data.Entity.SqlServer;
using PDMS.Common.Constants;

namespace PDMS.Data.Repository
{
    public interface IWarehouseRepository: IRepository<Warehouse>
    {
        IQueryable<WarehouseDTO> GetInfo(WarehouseDTO searchModel, Page page, out int totalcount);
        List<WarehouseDTO> DoExportWarehouseReprot(string uids);
        List<WarehouseDTO> DoAllExportWarehouseReprot(WarehouseDTO searchModel);
        List<WarehouseDTO> GetWarehouseStorageALL(int Plant_UID);
        List<SystemOrgDTO> GetOpType(int plantorguid);
        List<EnumerationDTO> GetWarehouseType();
        List<WarehouseBaseDTO> GetWarehouse();
        string InsertWarehouseStorageItem(List<Warehouse_StorageDTO> dtolist);
        string InsertWarehouseStorage(List<WarehouseBaseDTO> dtolist);
        List<SystemOrgDTO> GetFunplantsByop(int opuid);

        List<WarehouseDTO> GetByUId(int Warehouse_UID);
        List<SystemOrgDTO> QueryWarOps(int PlantID);
        List<SystemOrgDTO> GetWarFunplantsByop(int opuid,int PartType_UID);
        List<WarehouseDTO> GetWarIdByFunction(int Functionuid,int PartType_UID);

        List<SystemOrgDTO> GetPlants(int plantorguid);
        List<SystemOrgDTO> GetBGByPlant(int plantuid);
    }
    public class WarehouseRepository: RepositoryBase<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public string InsertWarehouseStorage(List<WarehouseBaseDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {

                        var sql = string.Format(@"INSERT dbo.Warehouse
                                            ( BG_Organization_UID ,
                                              FunPlant_Organization_UID ,
                                              Warehouse_ID ,
                                              Is_Enable ,
                                              Warehouse_Type_UID ,
                                              Name_ZH ,
                                              Name_EN ,
                                              [Desc] ,
                                              Modified_UID ,
                                              Modified_Date
                                            )
                                    VALUES  (   {0}, -- BG_Organization_UID - int
                                              {1} , -- FunPlant_Organization_UID - int
                                              N'{2}' , -- Warehouse_ID - nvarchar(10)
                                              {3} , -- Is_Enable - bit
                                              {4}, -- Warehouse_Type_UID - int
                                              N'{5}' , -- Name_ZH - nvarchar(50)
                                              N'{6}' , -- Name_EN - nvarchar(50)
                                              N'{7}' , -- Desc - nvarchar(50)
                                              {8}, -- Modified_UID - int
                                              GETDATE()  -- Modified_Date - datetime
                                            )
                                                                            ",
                                         dtolist[i].BG_Organization_UID,
                                           dtolist[i].FunPlant_Organization_UID,
                                            dtolist[i].Warehouse_ID,
                                            dtolist[i].Is_Enable ? 1 : 0,
                                           dtolist[i].Warehouse_Type_UID,
                                            dtolist[i].Name_ZH,
                                            dtolist[i].Name_EN,
                                               dtolist[i].Desc,
                                            dtolist[i].Modified_UID,
                                            DateTime.Now.ToString(FormatConstants.DateTimeFormatString));

                        DataContext.Database.ExecuteSqlCommand(sql);
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
        public string InsertWarehouseStorageItem(List<Warehouse_StorageDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {

                        var sql = string.Format(@"INSERT dbo.Warehouse_Storage
                                            ( Warehouse_UID ,
                                              Rack_ID ,
                                              Storage_ID ,
                                              Is_Enable ,
                                              [Desc] ,
                                              Modified_UID ,
                                              Modified_Date
                                            )
                                    VALUES  (  {0} , -- Warehouse_UID - int
                                              N'{1}' , -- Rack_ID - nvarchar(20)
                                              N'{2}' , -- Storage_ID - nvarchar(20)
                                              {3} , -- Is_Enable - bit
                                              N'{4}' , -- Desc - nvarchar(50)
                                              {5}, -- Modified_UID - int
                                              GETDATE()  -- Modified_Date - datetime
                                            )
                                        ",

                                            dtolist[i].Warehouse_UID,
                                           dtolist[i].Rack_ID,
                                            dtolist[i].Storage_ID,
                                            dtolist[i].Is_Enable ? 1 : 0,
                                               dtolist[i].Desc,
                                            dtolist[i].Modified_UID,
                                            DateTime.Now.ToString(FormatConstants.DateTimeFormatString));

                        DataContext.Database.ExecuteSqlCommand(sql);
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
        public IQueryable<WarehouseDTO> GetInfo(WarehouseDTO searchModel, Page page, out int totalcount)
        {
            var query = from D in DataContext.Warehouse_Storage
                        join M in DataContext.Warehouse 
                        on D.Warehouse_UID equals M.Warehouse_UID 
                        join user in DataContext.System_Users
                        on D.Modified_UID equals user.Account_UID into temp2
                        from bb in temp2.DefaultIfEmpty()
                        join enums in DataContext.Enumeration on
                        M.Warehouse_Type_UID equals enums.Enum_UID
                        join bgorg in DataContext.System_Organization on 
                        M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization on
                        M.FunPlant_Organization_UID equals funplantorg.Organization_UID
                        select new WarehouseDTO
                        {
                            Warehouse_Storage_UID = D.Warehouse_Storage_UID,
                            Warehouse_UID = M.Warehouse_UID,
                            Warehouse_ID = M.Warehouse_ID,
                            Name_ZH = M.Name_ZH,
                            Name_EN = M.Name_EN,
                            Rack_ID = D.Rack_ID,
                            Storage_ID = D.Storage_ID,
                            Warehouse_Type_UID = M.Warehouse_Type_UID,
                            Desc = M.Desc,
                            WarehouseStorageDesc=D.Desc,
                            Modifier = bb.User_Name,
                            Modified_Date = D.Modified_Date,
                            Warehouse_Type = enums.Enum_Value,
                            BG_Organization=bgorg.Organization_Name,
                            FunPlant_Organization=funplantorg.Organization_Name,
                            BG_Organization_UID=M.BG_Organization_UID,
                            FunPlant_Organization_UID=M.FunPlant_Organization_UID
                        };
            var query1 = from M in DataContext.Warehouse
                         join user in DataContext.System_Users on
                         M.Modified_UID equals user.Account_UID
                         join enums in DataContext.Enumeration on
                         M.Warehouse_Type_UID equals enums.Enum_UID
                         join bgorg in DataContext.System_Organization on
                         M.BG_Organization_UID equals bgorg.Organization_UID
                         join funplantorg in DataContext.System_Organization on
                         M.FunPlant_Organization_UID equals funplantorg.Organization_UID
                         select new WarehouseDTO
                         {
                             Warehouse_Storage_UID = 0,
                             Warehouse_UID = M.Warehouse_UID,
                             Warehouse_ID = M.Warehouse_ID,
                             Name_ZH = M.Name_ZH,
                             Name_EN = M.Name_EN,
                             Rack_ID = null,
                             Storage_ID = null,
                             Warehouse_Type_UID = M.Warehouse_Type_UID,
                             Desc = M.Desc,
                             WarehouseStorageDesc = null,
                             Modifier = user.User_Name,
                             Modified_Date = M.Modified_Date,
                             Warehouse_Type = enums.Enum_Value,
                             BG_Organization = bgorg.Organization_Name,
                             FunPlant_Organization = funplantorg.Organization_Name,
                             BG_Organization_UID = M.BG_Organization_UID,
                             FunPlant_Organization_UID = M.FunPlant_Organization_UID
                         };
            query = query.Union(query1);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Name_ZH))
                query = query.Where(m => m.Name_ZH == searchModel.Name_ZH);
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
                query = query.Where(m => m.Rack_ID==searchModel.Rack_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
                query = query.Where(m => m.Storage_ID == searchModel.Storage_ID);
            if (searchModel.Warehouse_Type_UID != 0)
                query = query.Where(m => m.Warehouse_Type_UID == searchModel.Warehouse_Type_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Warehouse_ID))
                query = query.Where(m => m.Warehouse_ID == searchModel.Warehouse_ID);
            query = query.Where(m => m.Rack_ID == ""|| m.Rack_ID==null);
            List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
            }
            query = SetWarehouseDTO(query.ToList());
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;
        }
        public IQueryable<WarehouseDTO> SetWarehouseDTO(List<WarehouseDTO> warehouses)
        {     
            List<SystemOrgDTO> system_Organizations = GetPant();
        
         
            foreach (var item in warehouses)
            {
                // 设置厂区                            
                var systemOrgDTO = system_Organizations.Where(o => o.ChildOrg_UID == item.BG_Organization_UID).FirstOrDefault();
                if(systemOrgDTO!=null)
                {
                    item.Plant_Organization_UID = systemOrgDTO.Organization_UID;
                    item.Plant = systemOrgDTO.Organization_Name;
                }
      
            }
            return warehouses.AsQueryable();
        }

        public List<WarehouseDTO> DoExportWarehouseReprot(string uids)
        {
            uids = "," + uids + ",";
            var query = from D in DataContext.Warehouse_Storage
                        join M in DataContext.Warehouse
                        on D.Warehouse_UID equals M.Warehouse_UID
                        join user in DataContext.System_Users
                        on D.Modified_UID equals user.Account_UID into temp2
                        from bb in temp2.DefaultIfEmpty()
                        join enums in DataContext.Enumeration on
                        M.Warehouse_Type_UID equals enums.Enum_UID
                        join bgorg in DataContext.System_Organization on
                        M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization on
                        M.FunPlant_Organization_UID equals funplantorg.Organization_UID
                        select new WarehouseDTO
                        {
                            Warehouse_Storage_UID = D.Warehouse_Storage_UID,
                            Warehouse_UID = M.Warehouse_UID,
                            Warehouse_ID = M.Warehouse_ID,
                            Name_ZH = M.Name_ZH,
                            Name_EN = M.Name_EN,
                            Rack_ID = D.Rack_ID,
                            Storage_ID = D.Storage_ID,
                            Warehouse_Type_UID = M.Warehouse_Type_UID,
                            Desc = M.Desc,
                            WarehouseStorageDesc = D.Desc,
                            Modifier = bb.User_Name,
                            Modified_Date = D.Modified_Date,
                            Warehouse_Type = enums.Enum_Value,
                            BG_Organization = bgorg.Organization_Name,
                            FunPlant_Organization = funplantorg.Organization_Name,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID
                        };
            var query1 = from M in DataContext.Warehouse
                         join user in DataContext.System_Users on
                         M.Modified_UID equals user.Account_UID
                         join enums in DataContext.Enumeration on
                         M.Warehouse_Type_UID equals enums.Enum_UID
                         join bgorg in DataContext.System_Organization on
                         M.BG_Organization_UID equals bgorg.Organization_UID
                         join funplantorg in DataContext.System_Organization on
                         M.FunPlant_Organization_UID equals funplantorg.Organization_UID
                         select new WarehouseDTO
                         {
                             Warehouse_Storage_UID = 0,
                             Warehouse_UID = M.Warehouse_UID,
                             Warehouse_ID = M.Warehouse_ID,
                             Name_ZH = M.Name_ZH,
                             Name_EN = M.Name_EN,
                             Rack_ID = null,
                             Storage_ID = null,
                             Warehouse_Type_UID = M.Warehouse_Type_UID,
                             Desc = M.Desc,
                             WarehouseStorageDesc = null,
                             Modifier = user.User_Name,
                             Modified_Date = M.Modified_Date,
                             Warehouse_Type = enums.Enum_Value,
                             BG_Organization = bgorg.Organization_Name,
                             FunPlant_Organization = funplantorg.Organization_Name,
                             BG_Organization_UID = M.BG_Organization_UID,
                             FunPlant_Organization_UID = M.FunPlant_Organization_UID
                         };
            query = query.Union(query1);

            query = query.Where(m => uids.Contains("," + m.Warehouse_UID + ","));
            return SetListWarehouseDTO(query.ToList());
            //return query.ToList();

        }
        public List<WarehouseDTO> DoAllExportWarehouseReprot(WarehouseDTO searchModel)
        {
            var query = from D in DataContext.Warehouse_Storage
                        join M in DataContext.Warehouse
                        on D.Warehouse_UID equals M.Warehouse_UID
                        join user in DataContext.System_Users
                        on D.Modified_UID equals user.Account_UID into temp2
                        from bb in temp2.DefaultIfEmpty()
                        join enums in DataContext.Enumeration on
                        M.Warehouse_Type_UID equals enums.Enum_UID
                        join bgorg in DataContext.System_Organization on
                        M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization on
                        M.FunPlant_Organization_UID equals funplantorg.Organization_UID
                        select new WarehouseDTO
                        {
                            Warehouse_Storage_UID = D.Warehouse_Storage_UID,
                            Warehouse_UID = M.Warehouse_UID,
                            Warehouse_ID = M.Warehouse_ID,
                            Name_ZH = M.Name_ZH,
                            Name_EN = M.Name_EN,
                            Rack_ID = D.Rack_ID,
                            Storage_ID = D.Storage_ID,
                            Warehouse_Type_UID = M.Warehouse_Type_UID,
                            Desc = M.Desc,
                            WarehouseStorageDesc = D.Desc,
                            Modifier = bb.User_Name,
                            Modified_Date = D.Modified_Date,
                            Warehouse_Type = enums.Enum_Value,
                            BG_Organization = bgorg.Organization_Name,
                            FunPlant_Organization = funplantorg.Organization_Name,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID
                        };
            var query1 = from M in DataContext.Warehouse
                         join user in DataContext.System_Users on
                         M.Modified_UID equals user.Account_UID
                         join enums in DataContext.Enumeration on
                         M.Warehouse_Type_UID equals enums.Enum_UID
                         join bgorg in DataContext.System_Organization on
                         M.BG_Organization_UID equals bgorg.Organization_UID
                         join funplantorg in DataContext.System_Organization on
                         M.FunPlant_Organization_UID equals funplantorg.Organization_UID
                         select new WarehouseDTO
                         {
                             Warehouse_Storage_UID = 0,
                             Warehouse_UID = M.Warehouse_UID,
                             Warehouse_ID = M.Warehouse_ID,
                             Name_ZH = M.Name_ZH,
                             Name_EN = M.Name_EN,
                             Rack_ID = null,
                             Storage_ID = null,
                             Warehouse_Type_UID = M.Warehouse_Type_UID,
                             Desc = M.Desc,
                             WarehouseStorageDesc = null,
                             Modifier = user.User_Name,
                             Modified_Date = M.Modified_Date,
                             Warehouse_Type = enums.Enum_Value,
                             BG_Organization = bgorg.Organization_Name,
                             FunPlant_Organization = funplantorg.Organization_Name,
                             BG_Organization_UID = M.BG_Organization_UID,
                             FunPlant_Organization_UID = M.FunPlant_Organization_UID
                         };
            query = query.Union(query1);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Name_ZH))
                query = query.Where(m => m.Name_ZH == searchModel.Name_ZH);
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
                query = query.Where(m => m.Rack_ID == searchModel.Rack_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
                query = query.Where(m => m.Storage_ID == searchModel.Storage_ID);
            if (searchModel.Warehouse_Type_UID != 0)
                query = query.Where(m => m.Warehouse_Type_UID == searchModel.Warehouse_Type_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Warehouse_ID))
                query = query.Where(m => m.Warehouse_ID == searchModel.Warehouse_ID);

            List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
            }
            return SetListWarehouseDTO(query.ToList());
           // return query.ToList();
        }
        public List<WarehouseDTO> GetWarehouseStorageALL(int Plant_UID)
        {
            var query = from D in DataContext.Warehouse_Storage
                        join M in DataContext.Warehouse
                        on D.Warehouse_UID equals M.Warehouse_UID
                        join user in DataContext.System_Users
                        on D.Modified_UID equals user.Account_UID into temp2
                        from bb in temp2.DefaultIfEmpty()
                        join enums in DataContext.Enumeration on
                        M.Warehouse_Type_UID equals enums.Enum_UID
                        join bgorg in DataContext.System_Organization on
                        M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization on
                        M.FunPlant_Organization_UID equals funplantorg.Organization_UID
                        select new WarehouseDTO
                        {
                            Warehouse_Storage_UID = D.Warehouse_Storage_UID,
                            Warehouse_UID = M.Warehouse_UID,
                            Warehouse_ID = M.Warehouse_ID,
                            Name_ZH = M.Name_ZH,
                            Name_EN = M.Name_EN,
                            Rack_ID = D.Rack_ID,
                            Storage_ID = D.Storage_ID,
                            Warehouse_Type_UID = M.Warehouse_Type_UID,
                            Desc = M.Desc,
                            WarehouseStorageDesc = D.Desc,
                            Modifier = bb.User_Name,
                            Modified_Date = D.Modified_Date,
                            Warehouse_Type = enums.Enum_Value,
                            BG_Organization = bgorg.Organization_Name,
                            FunPlant_Organization = funplantorg.Organization_Name,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID
                        };
            List<int> Plant_UIDs = GetOpType(Plant_UID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
            }
            return SetListWarehouseDTO(query.ToList());
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
        public List<SystemOrgDTO> GetOpType(int plantorguid)
        {
            var sqlStr = @"SELECT * FROM dbo.System_Organization t1 INNER JOIN dbo.System_OrganizationBOM t2
                                        ON t1.Organization_UID=t2.ChildOrg_UID WHERE  t1.Organization_ID LIKE'%2000%' ";
            if (plantorguid!=0)
            {
                sqlStr = @"SELECT * FROM dbo.System_Organization t1 INNER JOIN dbo.System_OrganizationBOM t2
                                        ON t1.Organization_UID=t2.ChildOrg_UID WHERE  t1.Organization_ID LIKE'%2000%'  and  t2.ParentOrg_UID={0}";
                sqlStr = string.Format(sqlStr, plantorguid);
            }

            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList();
            return dbList;
        }
        public List<SystemOrgDTO> GetPant()
        {
            var sqlStr = @"SELECT* FROM dbo.System_Organization t1 INNER JOIN dbo.System_OrganizationBOM t2
                                        ON t1.Organization_UID=t2.ParentOrg_UID WHERE  t1.Organization_ID LIKE'%1000%'";
            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList();
            return dbList;
        }


        public List<EnumerationDTO> GetWarehouseType()
        {
            var sqlStr = @"select * from Enumeration where Enum_Type='StorageManage' and Enum_Name='WarehouseType'";
            var dbList = DataContext.Database.SqlQuery<EnumerationDTO>(sqlStr).ToList();
            return dbList;
        }

        public List<WarehouseBaseDTO> GetWarehouse()
        {
            var query = from W in DataContext.Warehouse
                        select
                            new WarehouseBaseDTO
                            {
                            Warehouse_Type_UID = W.Warehouse_Type_UID,
                            Warehouse_ID = W.Warehouse_ID,
                            Name_EN = W.Name_EN,
                            Name_ZH = W.Name_ZH,
                            BG_Organization_UID = W.BG_Organization_UID,
                            FunPlant_Organization_UID = W.FunPlant_Organization_UID,
                            Desc = W.Desc,
                            Warehouse_UID = W.Warehouse_UID,
                            Is_Enable = W.Is_Enable
                            };

            return query.Distinct().ToList();
        }

        public List<SystemOrgDTO> GetFunplantsByop(int opuid)
        {
            var sqlStr = @"WITH one AS (
                                SELECT * FROM dbo.System_OrganizationBOM WHERE ParentOrg_UID={0}),
                                two AS (SELECT t1.* FROM dbo.System_OrganizationBOM t1 INNER JOIN one
                                ON t1.ParentOrg_UID=one.ChildOrg_UID ),
                                three AS (SELECT t1.* FROM dbo.System_Organization t1 INNER JOIN two
                                ON t1.Organization_UID=two.ChildOrg_UID)
                                SELECT DISTINCT * FROM three";
            sqlStr = string.Format(sqlStr, opuid);
            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList();
            return dbList;
        }

        public List<WarehouseDTO> GetByUId(int Warehouse_UID)
        {
            string sql = @"SELECT t1.*,t3.Organization_Name BG_Organization,
                                    t4.Organization_Name FunPlant_Organization ,t5.Enum_Value Warehouse_Type  FROM 
                                    Warehouse t1  inner join System_Organization t3 on t1.BG_Organization_UID=t3.Organization_UID
                                    inner join System_Organization t4 on t1.FunPlant_Organization_UID=t4.Organization_UID
                                    left join Enumeration t5 on t1.Warehouse_Type_UID=t5.Enum_UID 
                                    where t1.Warehouse_UID={0}";
            sql = string.Format(sql, Warehouse_UID);
            var dblist = DataContext.Database.SqlQuery<WarehouseDTO>(sql).ToList();

            return SetListWarehouseDTO(dblist);
           // return dblist;
        }

        public List<SystemOrgDTO> QueryWarOps(int PlantID=0)
        {
            string sql = @"select distinct t2.Organization_UID,t2.Organization_Name from Warehouse t1 inner join 
                                System_Organization t2 on t1.BG_Organization_UID=t2.Organization_UID";
            sql = string.Format(sql);
            var dblist = DataContext.Database.SqlQuery<SystemOrgDTO>(sql).ToList();
            List<int> Plant_UIDs = GetOpType(PlantID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                dblist = dblist.Where(m => Plant_UIDs.Contains(m.Organization_UID)).ToList();
            }
            return dblist;
        }

        public List<SystemOrgDTO> GetWarFunplantsByop(int opuid,int PartType_UID)
        {     //fky2017/11/13
              //if (PartType_UID == 407)
            if (PartType_UID == 432)
            {
                //fky2017/11/13
                // PartType_UID = 371;
                  PartType_UID = 405;
            }
            //fky2017/11/13
            //else if (PartType_UID == 408)
            else if (PartType_UID == 433)
                //fky2017/11/13
                // PartType_UID = 398;
                PartType_UID = 418;
            var sqlStr = @"select distinct t2.Organization_UID,t2.Organization_Name from Warehouse t1 inner join 
                                System_Organization t2 on t1.FunPlant_Organization_UID=t2.Organization_UID
                                where BG_Organization_UID={0} ";
            if (PartType_UID != 0)
            {
                sqlStr += " and Warehouse_Type_UID={1}";
                sqlStr = string.Format(sqlStr, opuid, PartType_UID);
            }
            else
            {
                sqlStr = string.Format(sqlStr, opuid);
            }

            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList();
            return dbList;
        }

        public List<WarehouseDTO> GetWarIdByFunction(int Functionuid,int PartType_UID)
        {   //fky2017/11/13
            //if (PartType_UID == 407)
            if (PartType_UID == 432)
            {
                //fky2017/11/13
                //PartType_UID = 371;
                PartType_UID = 405;
            }
            //fky2017/11/13
           // else if (PartType_UID == 408)
            else if (PartType_UID == 433)
                //fky2017/11/13
                // PartType_UID = 398;
                PartType_UID = 418;
                var sqlStr = @"select * from Warehouse where FunPlant_Organization_UID={0}";
            if (PartType_UID != 0)
            {
                sqlStr += " and Warehouse_Type_UID={1}";
                sqlStr = string.Format(sqlStr, Functionuid, PartType_UID);
            }
            else
            {
                sqlStr = string.Format(sqlStr, Functionuid);
            }
            var dbList = DataContext.Database.SqlQuery<WarehouseDTO>(sqlStr).ToList();
            return dbList;
        }

        public List<SystemOrgDTO> GetPlants(int plantorguid)
        {

            var sqlStr = @"select * from System_Organization where Organization_ID like '1%'";
            if (plantorguid != 0)
                sqlStr = @"select * from System_Organization where Organization_UID =" + plantorguid.ToString();
            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList();
            return dbList;
        }

        public List<SystemOrgDTO> GetBGByPlant(int plantuid)
        {
            var sqlStr = @"select t2.* from System_OrganizationBOM t1 inner join System_Organization
                                    t2 on t1.ChildOrg_UID=t2.Organization_UID where ParentOrg_UID={0}";
            sqlStr = string.Format(sqlStr, plantuid);
            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList();
            return dbList;
        }
    }
}
