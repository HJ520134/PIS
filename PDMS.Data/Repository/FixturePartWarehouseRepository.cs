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
    public interface IFixturePartWarehouseRepository : IRepository<Fixture_Warehouse>
    {
        List<SystemOrgDTO> GetOpType(int plantorguid);
        IQueryable<FixturePartWarehouseDTO> GetInfo(FixturePartWarehouseDTO searchModel, Page page, out int totalcount);
        List<SystemOrgDTO> GetFunplantsByop(int opuid);
        List<FixturePartWarehouseDTO> GetByUId(int Warehouse_UID);
        List<System_Users> GetUserAll();
        List<FixturePartWarehouseDTO> GetAllInfo(int Plant_Organization_UID);
        List<FixturePartWarehouseDTO> GetFixtureWarehouseStorageALL(int Plant_Organization_UID);
        string InsertWarehouse(List<FixturePartWarehouseDTO> dtolist);
        string InsertWarehouseStorage(List<FixturePartWarehouseDTO> dtolist);
        List<FixturePartWarehouseDTO> DoExportWarehouseReprot(string uids);
        List<FixturePartWarehouseDTO> DoAllExportWarehouseReprot(FixturePartWarehouseDTO searchModel);
        List<FixturePartWarehouseDTO> GetFixtureWarehouseStorages(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);

        List<FixturePartWarehouseDTO> GetFixtureWarehouses(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
    }
    public class FixturePartWarehouseRepository : RepositoryBase<Fixture_Warehouse>, IFixturePartWarehouseRepository
    {
        public FixturePartWarehouseRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
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
        public IQueryable<FixturePartWarehouseDTO> GetInfo(FixturePartWarehouseDTO searchModel, Page page, out int totalcount)
        {
            //var query = from D in DataContext.Fixture_Warehouse_Storage
            //            join M in DataContext.Fixture_Warehouse
            //            on D.Fixture_Warehouse_UID equals M.Fixture_Warehouse_UID
            //            join cuser in DataContext.System_Users
            //            on D.Modified_UID equals cuser.Account_UID
            //            join user in DataContext.System_Users
            //            on D.Modified_UID equals user.Account_UID            
            //            join plantorg in DataContext.System_Organization on
            //            M.Plant_Organization_UID equals plantorg.Organization_UID
            //            join bgorg in DataContext.System_Organization on
            //            M.BG_Organization_UID equals bgorg.Organization_UID
            //            join funplantorg in DataContext.System_Organization on
            //            M.FunPlant_Organization_UID equals funplantorg.Organization_UID
            //            select new FixturePartWarehouseDTO
            //            {
            //                Fixture_Warehouse_Storage_UID = D.Fixture_Warehouse_Storage_UID,
            //                Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
            //                Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
            //                Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,                   
            //                Rack_ID = D.Rack_ID,
            //                Storage_ID = D.Storage_ID,
            //                Remarks = D.Remarks,
            //                Is_Enable=D.Is_Enable,
            //                Createder= cuser.User_Name,
            //                Created_Date=D.Created_Date,
            //                Created_UID=D.Created_UID,
            //                Modifier = user.User_Name,
            //                Modified_Date = D.Modified_Date,
            //                Modified_UID=D.Modified_UID,
            //                Plant=plantorg.Organization_Name,
            //                BG_Organization = bgorg.Organization_Name,
            //                FunPlant_Organization = funplantorg.Organization_Name,
            //                Plant_Organization_UID= D.Plant_Organization_UID,
            //                BG_Organization_UID = D.BG_Organization_UID,
            //                FunPlant_Organization_UID = D.FunPlant_Organization_UID
            //            };
            //var query = from M in DataContext.Fixture_Warehouse
            //             join cuser in DataContext.System_Users
            //             on M.Modified_UID equals cuser.Account_UID                       
            //             join user in DataContext.System_Users on
            //             M.Modified_UID equals user.Account_UID
            //             join plantorg in DataContext.System_Organization on
            //             M.Plant_Organization_UID equals plantorg.Organization_UID
            //             join bgorg in DataContext.System_Organization on
            //             M.BG_Organization_UID equals bgorg.Organization_UID
            //             //join funplantorg in DataContext.System_Organization on
            //             //M.FunPlant_Organization_UID equals funplantorg.Organization_UID
            //             select new FixturePartWarehouseDTO
            //             {
            //                 Fixture_Warehouse_Storage_UID = 0,
            //                 Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
            //                 Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
            //                 Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
            //                 Rack_ID = null,
            //                 Storage_ID =null,
            //                 Remarks = M.Remarks,
            //                 Is_Enable = M.Is_Enable,
            //                 Createder = cuser.User_Name,
            //                 Created_Date = M.Created_Date,
            //                 Created_UID = M.Created_UID,
            //                 Modifier = user.User_Name,
            //                 Modified_Date = M.Modified_Date,
            //                 Modified_UID = M.Modified_UID,
            //                 Plant = plantorg.Organization_Name,
            //                 BG_Organization = bgorg.Organization_Name,
            //                 //FunPlant_Organization = funplantorg.Organization_Name,
            //                 Plant_Organization_UID = M.Plant_Organization_UID,
            //                 BG_Organization_UID = M.BG_Organization_UID,
            //                 FunPlant_Organization_UID = M.FunPlant_Organization_UID
            //             };
            //query = query.Union(query1);


            var query = from M in DataContext.Fixture_Warehouse                   
                        select new FixturePartWarehouseDTO
                        {
                            Fixture_Warehouse_Storage_UID = 0,
                            Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
                            Rack_ID = null,
                            Storage_ID = null,
                            Remarks = M.Remarks,
                            Is_Enable = M.Is_Enable,
                            Createder = M.System_Users.User_Name,
                            Created_Date = M.Created_Date,
                            Created_UID = M.Created_UID,
                            Modifier = M.System_Users1.User_Name,
                            Modified_Date = M.Modified_Date,
                            Modified_UID = M.Modified_UID,
                            Plant = M.System_Organization.Organization_Name,
                            BG_Organization = M.System_Organization1.Organization_Name,
                            FunPlant_Organization = M.System_Organization2.Organization_Name,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID
                        };
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null&& searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_ID))
                query = query.Where(m => m.Fixture_Warehouse_ID == searchModel.Fixture_Warehouse_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_Name))
                query = query.Where(m => m.Fixture_Warehouse_Name == searchModel.Fixture_Warehouse_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.Remarks))
                query = query.Where(m => m.Fixture_Warehouse_Name == searchModel.Remarks);
            if (searchModel.Is_Enable != null)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);
            
            //if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
            //    query = query.Where(m => m.Rack_ID == searchModel.Rack_ID);
            //if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
            //    query = query.Where(m => m.Storage_ID == searchModel.Storage_ID);    
            //query = query.Where(m => m.Rack_ID == "" || m.Rack_ID == null);
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;
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

        public List<FixturePartWarehouseDTO> GetByUId(int Warehouse_UID)
        {

            //var query = from M in DataContext.Fixture_Warehouse
            //            join cuser in DataContext.System_Users
            //            on M.Modified_UID equals cuser.Account_UID
            //            join user in DataContext.System_Users on
            //            M.Modified_UID equals user.Account_UID
            //            join plantorg in DataContext.System_Organization on
            //            M.Plant_Organization_UID equals plantorg.Organization_UID
            //            join bgorg in DataContext.System_Organization on
            //            M.BG_Organization_UID equals bgorg.Organization_UID
            //            //join funplantorg in DataContext.System_Organization on
            //            //M.FunPlant_Organization_UID equals funplantorg.Organization_UID
            //            select new FixturePartWarehouseDTO
            //            {
            //                Fixture_Warehouse_Storage_UID = 0,
            //                Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
            //                Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
            //                Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
            //                Rack_ID = null,
            //                Storage_ID = null,
            //                Remarks = M.Remarks,
            //                Is_Enable = M.Is_Enable,
            //                Createder = cuser.User_Name,
            //                Created_Date = M.Created_Date,
            //                Created_UID = M.Created_UID,
            //                Modifier = user.User_Name,
            //                Modified_Date = M.Modified_Date,
            //                Modified_UID = M.Modified_UID,
            //                Plant = plantorg.Organization_Name,
            //                BG_Organization = bgorg.Organization_Name,
            //                //FunPlant_Organization = funplantorg.Organization_Name,
            //                Plant_Organization_UID = M.Plant_Organization_UID,
            //                BG_Organization_UID = M.BG_Organization_UID,
            //                FunPlant_Organization_UID = M.FunPlant_Organization_UID
            //            };


            var query = from M in DataContext.Fixture_Warehouse                
                        select new FixturePartWarehouseDTO
                        {
                            Fixture_Warehouse_Storage_UID = 0,
                            Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
                            Rack_ID = null,
                            Storage_ID = null,
                            Remarks = M.Remarks,
                            Is_Enable = M.Is_Enable,
                            Createder = M.System_Users.User_Name,
                            Created_Date = M.Created_Date,
                            Created_UID = M.Created_UID,
                            Modifier = M.System_Users1.User_Name,
                            Modified_Date = M.Modified_Date,
                            Modified_UID = M.Modified_UID,
                            Plant = M.System_Organization.Organization_Name,
                            BG_Organization = M.System_Organization1.Organization_Name,
                            FunPlant_Organization = M.System_Organization2.Organization_Name,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID
                        };
            if (Warehouse_UID != 0)
                query = query.Where(m => m.Fixture_Warehouse_UID == Warehouse_UID);
            return query.ToList();
            //return SetFunPlant_Organization(query.ToList());
        }

        public List<FixturePartWarehouseDTO> GetFixtureWarehouseStorageALL(int Plant_Organization_UID)
        {

            //var query = from D in DataContext.Fixture_Warehouse_Storage
            //            join M in DataContext.Fixture_Warehouse
            //            on D.Fixture_Warehouse_UID equals M.Fixture_Warehouse_UID
            //            join cuser in DataContext.System_Users
            //            on D.Modified_UID equals cuser.Account_UID
            //            join user in DataContext.System_Users
            //            on D.Modified_UID equals user.Account_UID
            //            join plantorg in DataContext.System_Organization on
            //            M.Plant_Organization_UID equals plantorg.Organization_UID
            //            join bgorg in DataContext.System_Organization on
            //            M.BG_Organization_UID equals bgorg.Organization_UID
            //            //join funplantorg in DataContext.System_Organization on
            //            //M.FunPlant_Organization_UID equals funplantorg.Organization_UID
            //            select new FixturePartWarehouseDTO
            //            {
            //                Fixture_Warehouse_Storage_UID = D.Fixture_Warehouse_Storage_UID,
            //                Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
            //                Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
            //                Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
            //                Rack_ID = D.Rack_ID,
            //                Storage_ID = D.Storage_ID,
            //                Remarks = D.Remarks,
            //                Is_Enable = D.Is_Enable,
            //                Createder = cuser.User_Name,
            //                Created_Date = D.Created_Date,
            //                Created_UID = D.Created_UID,
            //                Modifier = user.User_Name,
            //                Modified_Date = D.Modified_Date,
            //                Modified_UID = D.Modified_UID,
            //                Plant = plantorg.Organization_Name,
            //                BG_Organization = bgorg.Organization_Name,
            //                //FunPlant_Organization = funplantorg.Organization_Name,
            //                Plant_Organization_UID = D.Plant_Organization_UID,
            //                BG_Organization_UID = D.BG_Organization_UID,
            //                FunPlant_Organization_UID = D.FunPlant_Organization_UID
            //            };
            var query = from D in DataContext.Fixture_Warehouse_Storage
                        join M in DataContext.Fixture_Warehouse
                        on D.Fixture_Warehouse_UID equals M.Fixture_Warehouse_UID        
                        select new FixturePartWarehouseDTO
                        {
                            Fixture_Warehouse_Storage_UID = D.Fixture_Warehouse_Storage_UID,
                            Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
                            Rack_ID = D.Rack_ID,
                            Storage_ID = D.Storage_ID,
                            Remarks = D.Remarks,
                            Is_Enable = D.Is_Enable,
                            Createder = D.System_Users.User_Name,
                            Created_Date = D.Created_Date,
                            Created_UID = D.Created_UID,
                            Modifier = D.System_Users1.User_Name,
                            Modified_Date = D.Modified_Date,
                            Modified_UID = D.Modified_UID,
                            Plant = D.System_Organization.Organization_Name,
                            BG_Organization = D.System_Organization1.Organization_Name,
                            FunPlant_Organization = D.System_Organization2.Organization_Name,
                            Plant_Organization_UID = D.Plant_Organization_UID,
                            BG_Organization_UID = D.BG_Organization_UID,
                            FunPlant_Organization_UID = D.FunPlant_Organization_UID
                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            return query.ToList();
            //return SetFunPlant_Organization(query.ToList());
        }

        public List<System_Users> GetUserAll()
        {
            return DataContext.System_Users.ToList();

        }
        public List<FixturePartWarehouseDTO> GetAllInfo(int Plant_Organization_UID)
        {
            //var query = from M in DataContext.Fixture_Warehouse
            //            join cuser in DataContext.System_Users
            //            on M.Modified_UID equals cuser.Account_UID
            //            join user in DataContext.System_Users on
            //            M.Modified_UID equals user.Account_UID
            //            join plantorg in DataContext.System_Organization on
            //            M.Plant_Organization_UID equals plantorg.Organization_UID
            //            join bgorg in DataContext.System_Organization on
            //            M.BG_Organization_UID equals bgorg.Organization_UID
            //            //join funplantorg in DataContext.System_Organization on
            //            //M.FunPlant_Organization_UID equals funplantorg.Organization_UID
            //            select new FixturePartWarehouseDTO
            //            {
            //                Fixture_Warehouse_Storage_UID = 0,
            //                Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
            //                Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
            //                Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
            //                Rack_ID = null,
            //                Storage_ID = null,
            //                Remarks = M.Remarks,
            //                Is_Enable = M.Is_Enable,
            //                Createder = cuser.User_Name,
            //                Created_Date = M.Created_Date,
            //                Created_UID = M.Created_UID,
            //                Modifier = user.User_Name,
            //                Modified_Date = M.Modified_Date,
            //                Modified_UID = M.Modified_UID,
            //                Plant = plantorg.Organization_Name,
            //                BG_Organization = bgorg.Organization_Name,
            //                //FunPlant_Organization = funplantorg.Organization_Name,
            //                Plant_Organization_UID = M.Plant_Organization_UID,
            //                BG_Organization_UID = M.BG_Organization_UID,
            //                FunPlant_Organization_UID = M.FunPlant_Organization_UID
            //            };

            var query = from M in DataContext.Fixture_Warehouse                   
                        select new FixturePartWarehouseDTO
                        {
                            Fixture_Warehouse_Storage_UID = 0,
                            Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
                            Rack_ID = null,
                            Storage_ID = null,
                            Remarks = M.Remarks,
                            Is_Enable = M.Is_Enable,
                            Createder = M.System_Users.User_Name,
                            Created_Date = M.Created_Date,
                            Created_UID = M.Created_UID,
                            Modifier = M.System_Users1.User_Name,
                            Modified_Date = M.Modified_Date,
                            Modified_UID = M.Modified_UID,
                            Plant = M.System_Organization.Organization_Name,
                            BG_Organization = M.System_Organization1.Organization_Name,
                            FunPlant_Organization = M.System_Organization2.Organization_Name,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID
                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            return query.ToList();
            //return SetFunPlant_Organization(query.ToList());
        }


        //public List<FixturePartWarehouseDTO> SetFunPlant_Organization(List<FixturePartWarehouseDTO> FunPlant_Organizations)
        //{
        //    List<System_Organization> System_Organizations = DataContext.System_Organization.ToList();
        //    foreach (var item in FunPlant_Organizations)
        //    {
        //        if (item.FunPlant_Organization_UID != 0 || item.FunPlant_Organization_UID != null)
        //        {
        //            string FunPlant_Organization = System_Organizations.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).FirstOrDefault().Organization_Name;
        //            item.FunPlant_Organization = FunPlant_Organization;
        //        }
        //    }
        //    return FunPlant_Organizations;
        //}
        public string InsertWarehouse(List<FixturePartWarehouseDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        string strFunplantUid = "null";
                        if (dtolist[i].FunPlant_Organization_UID != null)
                            strFunplantUid = dtolist[i].FunPlant_Organization_UID.ToString();
                        var sql = string.Format(@"INSERT INTO Fixture_Warehouse
                                                       (Plant_Organization_UID
                                                       ,BG_Organization_UID
                                                       ,FunPlant_Organization_UID
                                                       ,Fixture_Warehouse_ID
                                                       ,Fixture_Warehouse_Name
                                                       ,Remarks
                                                       ,Is_Enable
                                                       ,Created_UID
                                                       ,Created_Date
                                                       ,Modified_UID
                                                       ,Modified_Date)
                                                 VALUES (
                                                          {0}, 
                                                          {1}, 
                                                          {2} ,
                                                          N'{3}' , 
                                                          N'{4}',  
                                                          N'{5}' , 
                                                          {6} ,                                             
                                                          {7}, 
                                                          '{8}',   
                                                          {9},  
                                                          '{10}'                                           
                                                         )
                                                           ",
                                            dtolist[i].Plant_Organization_UID,
                                            dtolist[i].BG_Organization_UID,
                                            strFunplantUid,
                                            dtolist[i].Fixture_Warehouse_ID,
                                            dtolist[i].Fixture_Warehouse_Name,
                                            dtolist[i].Remarks,
                                            dtolist[i].Is_Enable == true ? 1 : 0,
                                            dtolist[i].Created_UID,
                                            DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
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

        public   string InsertWarehouseStorage(List<FixturePartWarehouseDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        string strFunplantUid = "null";
                        if (dtolist[i].FunPlant_Organization_UID != null)
                            strFunplantUid = dtolist[i].FunPlant_Organization_UID.ToString();
                        var sql = string.Format(@"INSERT INTO Fixture_Warehouse_Storage
                                                       (Plant_Organization_UID
                                                       ,BG_Organization_UID
                                                       ,FunPlant_Organization_UID
                                                       ,Fixture_Warehouse_UID
                                                       ,Storage_ID
                                                       ,Rack_ID
                                                       ,Remarks
                                                       ,Is_Enable
                                                       ,Created_UID
                                                       ,Created_Date
                                                       ,Modified_UID
                                                       ,Modified_Date)
                                                 VALUES
                                                       ({0}
                                                       ,{1}
                                                       ,{2}
                                                       ,{3}
                                                       ,N'{4}'
                                                       ,N'{5}'
                                                       ,N'{6}'
                                                       ,{7}
                                                       ,{8}
                                                       ,'{9}'
                                                       ,{10}
                                                       ,'{11}')
                                              ",
                                        dtolist[i].Plant_Organization_UID,
                                        dtolist[i].BG_Organization_UID,
                                        strFunplantUid,
                                        dtolist[i].Fixture_Warehouse_UID,
                                        dtolist[i].Storage_ID,
                                        dtolist[i].Rack_ID,
                                        dtolist[i].Remarks,
                                        dtolist[i].Is_Enable == true ? 1 : 0,
                                        dtolist[i].Created_UID,
                                        DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
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


        public List<FixturePartWarehouseDTO> DoExportWarehouseReprot(string uids)
        {
            uids = "," + uids + ",";
            var query = from D in DataContext.Fixture_Warehouse_Storage
                        join M in DataContext.Fixture_Warehouse
                        on D.Fixture_Warehouse_UID equals M.Fixture_Warehouse_UID
                        select new FixturePartWarehouseDTO
                        {
                            Fixture_Warehouse_Storage_UID = D.Fixture_Warehouse_Storage_UID,
                            Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
                            Rack_ID = D.Rack_ID,
                            Storage_ID = D.Storage_ID,
                            Remarks = D.Remarks,
                            Is_Enable = D.Is_Enable,
                            Createder = D.System_Users.User_Name,
                            Created_Date = D.Created_Date,
                            Created_UID = D.Created_UID,
                            Modifier = D.System_Users1.User_Name,
                            Modified_Date = D.Modified_Date,
                            Modified_UID = D.Modified_UID,
                            Plant = D.System_Organization.Organization_Name,
                            BG_Organization = D.System_Organization1.Organization_Name,
                            FunPlant_Organization = D.System_Organization2.Organization_Name,
                            Plant_Organization_UID = D.Plant_Organization_UID,
                            BG_Organization_UID = D.BG_Organization_UID,
                            FunPlant_Organization_UID = D.FunPlant_Organization_UID
                        };
            var query1 = from M in DataContext.Fixture_Warehouse
                         select new FixturePartWarehouseDTO
                         {
                             Fixture_Warehouse_Storage_UID = 0,
                             Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
                             Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
                             Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
                             Rack_ID = null,
                             Storage_ID = null,
                             Remarks = M.Remarks,
                             Is_Enable = M.Is_Enable,
                             Createder = M.System_Users.User_Name,
                             Created_Date = M.Created_Date,
                             Created_UID = M.Created_UID,
                             Modifier = M.System_Users1.User_Name,
                             Modified_Date = M.Modified_Date,
                             Modified_UID = M.Modified_UID,
                             Plant = M.System_Organization.Organization_Name,
                             BG_Organization = M.System_Organization1.Organization_Name,
                             FunPlant_Organization = M.System_Organization2.Organization_Name,
                             Plant_Organization_UID = M.Plant_Organization_UID,
                             BG_Organization_UID = M.BG_Organization_UID,
                             FunPlant_Organization_UID = M.FunPlant_Organization_UID
                         };
            query = query.Union(query1);

            query = query.Where(m => uids.Contains("," + m.Fixture_Warehouse_UID + ","));
          
            return query.ToList();

        }
        public List<FixturePartWarehouseDTO> DoAllExportWarehouseReprot(FixturePartWarehouseDTO searchModel)
        {
            var query = from D in DataContext.Fixture_Warehouse_Storage
                        join M in DataContext.Fixture_Warehouse
                        on D.Fixture_Warehouse_UID equals M.Fixture_Warehouse_UID
                        select new FixturePartWarehouseDTO
                        {
                            Fixture_Warehouse_Storage_UID = D.Fixture_Warehouse_Storage_UID,
                            Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
                            Rack_ID = D.Rack_ID,
                            Storage_ID = D.Storage_ID,
                            Remarks = D.Remarks,
                            Is_Enable = D.Is_Enable,
                            Createder = D.System_Users.User_Name,
                            Created_Date = D.Created_Date,
                            Created_UID = D.Created_UID,
                            Modifier = D.System_Users1.User_Name,
                            Modified_Date = D.Modified_Date,
                            Modified_UID = D.Modified_UID,
                            Plant = D.System_Organization.Organization_Name,
                            BG_Organization = D.System_Organization1.Organization_Name,
                            FunPlant_Organization = D.System_Organization2.Organization_Name,
                            Plant_Organization_UID = D.Plant_Organization_UID,
                            BG_Organization_UID = D.BG_Organization_UID,
                            FunPlant_Organization_UID = D.FunPlant_Organization_UID
                        };
            var query1 = from M in DataContext.Fixture_Warehouse
                         select new FixturePartWarehouseDTO
                         {
                             Fixture_Warehouse_Storage_UID = 0,
                             Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
                             Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
                             Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
                             Rack_ID = null,
                             Storage_ID = null,
                             Remarks = M.Remarks,
                             Is_Enable = M.Is_Enable,
                             Createder = M.System_Users.User_Name,
                             Created_Date = M.Created_Date,
                             Created_UID = M.Created_UID,
                             Modifier = M.System_Users1.User_Name,
                             Modified_Date = M.Modified_Date,
                             Modified_UID = M.Modified_UID,
                             Plant = M.System_Organization.Organization_Name,
                             BG_Organization = M.System_Organization1.Organization_Name,
                             FunPlant_Organization = M.System_Organization2.Organization_Name,
                             Plant_Organization_UID = M.Plant_Organization_UID,
                             BG_Organization_UID = M.BG_Organization_UID,
                             FunPlant_Organization_UID = M.FunPlant_Organization_UID
                         };
            query = query.Union(query1);   
            if(searchModel.Plant_Organization_UID!=0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0&& searchModel.FunPlant_Organization_UID !=null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_ID))
                query = query.Where(m => m.Fixture_Warehouse_ID == searchModel.Fixture_Warehouse_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
                query = query.Where(m => m.Rack_ID == searchModel.Rack_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
                query = query.Where(m => m.Storage_ID == searchModel.Storage_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Warehouse_Name))
                query = query.Where(m => m.Fixture_Warehouse_Name == searchModel.Fixture_Warehouse_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.Remarks))
                query = query.Where(m => m.Remarks == searchModel.Remarks);
            //if (searchModel.Is_Enable !=null)
            //    query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);            
            return query.ToList();
        }
        public List<FixturePartWarehouseDTO> GetFixtureWarehouseStorages(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from D in DataContext.Fixture_Warehouse_Storage
                        join M in DataContext.Fixture_Warehouse
                        on D.Fixture_Warehouse_UID equals M.Fixture_Warehouse_UID
                        select new FixturePartWarehouseDTO
                        {
                            Fixture_Warehouse_Storage_UID = D.Fixture_Warehouse_Storage_UID,
                            Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
                            Rack_ID = D.Rack_ID,
                            Storage_ID = D.Storage_ID,
                            Remarks = D.Remarks,
                            Is_Enable = D.Is_Enable,
                            Createder = D.System_Users.User_Name,
                            Created_Date = D.Created_Date,
                            Created_UID = D.Created_UID,
                            Modifier = D.System_Users1.User_Name,
                            Modified_Date = D.Modified_Date,
                            Modified_UID = D.Modified_UID,
                            Plant = D.System_Organization.Organization_Name,
                            BG_Organization = D.System_Organization1.Organization_Name,
                            FunPlant_Organization = D.System_Organization2.Organization_Name,
                            Plant_Organization_UID = D.Plant_Organization_UID,
                            BG_Organization_UID = D.BG_Organization_UID,
                            FunPlant_Organization_UID = D.FunPlant_Organization_UID
                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }

        public List<FixturePartWarehouseDTO> GetFixtureWarehouses(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var query = from M in DataContext.Fixture_Warehouse
                        select new FixturePartWarehouseDTO
                        {
                            Fixture_Warehouse_Storage_UID = 0,
                            Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Name,
                            Rack_ID = null,
                            Storage_ID = null,
                            Remarks = M.Remarks,
                            Is_Enable = M.Is_Enable,
                            Createder = M.System_Users.User_Name,
                            Created_Date = M.Created_Date,
                            Created_UID = M.Created_UID,
                            Modifier = M.System_Users1.User_Name,
                            Modified_Date = M.Modified_Date,
                            Modified_UID = M.Modified_UID,
                            Plant = M.System_Organization.Organization_Name,
                            BG_Organization = M.System_Organization1.Organization_Name,
                            FunPlant_Organization = M.System_Organization2.Organization_Name,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID
                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }
    }
}
