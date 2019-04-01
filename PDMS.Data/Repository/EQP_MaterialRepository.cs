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
    public interface IEQP_MaterialRepository : IRepository<EQP_Material>
    {
        IQueryable<EQPMaterialDTO> QueryEQPMaterials(EQPMaterialDTO searchModel, Page page, out int totalcount);
        string InsertItem(List<EQPTypeDTO> dtolist);
        List<EQPMaterialDTO> QueryEQPMaterialsByEqpID(int id);
        string InsertEQPMaterial(List<EQPMaterialDTO> dtolist);
        List<EQPMaterialDTO> QueryEQPMaterialsAll();

    }
    public class EQP_MaterialRepository:RepositoryBase<EQP_Material>, IEQP_MaterialRepository
    {
        public EQP_MaterialRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IQueryable<EQPMaterialDTO> QueryEQPMaterials(EQPMaterialDTO searchModel, Page page, out int totalcount)
        {
            string sql = string.Format(@"select c.EQP_Material_UID,c.Material_Uid,c.EQP_Type_UID,c.[Desc],modifiedUser.[User_Name] as Modified_UserName,modifiedUser.User_NTID as Modified_UserNTID,c.Modified_Date,c.Modified_UID,eqp.EQP_Type,org.Organization_Name as OPType,org.Organization_UID as BG_Organization_UID,org1.Organization_Name + '--- ' + org.Organization_Desc as Funplant,org1.Organization_UID as FunPlantUID,org2.Organization_Name as Plant from  EQP_Material as c
                                        inner join  System_Users as modifiedUser on c.Modified_UID = modifiedUser.Account_UID
                                        inner join  EQP_Type as eqp on c.EQP_Type_UID = eqp.EQP_Type_UID
                                        inner join  System_Organization as org on eqp.BG_Organization_UID = org.Organization_UID
                                        inner join  System_Organization as org1 on eqp.FunPlant_Organization_UID = org1.Organization_UID
                                        inner join  System_OrganizationBOM as bom on eqp.BG_Organization_UID = bom.ChildOrg_UID
                                        inner join  System_Organization as org2 on bom.ParentOrg_UID = org2.Organization_UID 
                                        where c.[EQP_Material_UID] in (select [EQP_Material_UID]
                                        from
                                        (
                                        select *,
        
                                               --先按typeid分组,在一组中按照datetime降序排列,来编号
                                               ROW_NUMBER() over(partition by [EQP_Type_UID] 
                                                                     order by [EQP_Material_UID] desc)  as rownum
                                        from [EQP_Material]
                                        )t
                                        where rownum<=1
                                        )");
            var query = DataContext.Database.SqlQuery<EQPMaterialDTO>(sql).AsQueryable();
            //var query =from c in DataContext.EQP_Material
            //        join modifiedUser in DataContext.System_Users on c.Modified_UID equals modifiedUser.Account_UID
            //        join eqp in DataContext.EQP_Type on c.EQP_Type_UID equals eqp.EQP_Type_UID
            //        join org in DataContext.System_Organization on eqp.BG_Organization_UID equals org.Organization_UID
            //        join org1 in DataContext.System_Organization on eqp.FunPlant_Organization_UID equals org1.Organization_UID
            //        join bom in DataContext.System_OrganizationBOM on eqp.BG_Organization_UID equals bom.ChildOrg_UID
            //        join org2 in DataContext.System_Organization on bom.ParentOrg_UID equals org2.Organization_UID
            //        select new EQPMaterialDTO
            //        {
            //            EQP_Material_UID = c.EQP_Material_UID,
            //            Material_Uid = c.Material_Uid,
            //            EQP_Type_UID=c.EQP_Type_UID,
            //            Desc = c.Desc,
            //            Modified_UserName = modifiedUser.User_Name,
            //            Modified_UserNTID = modifiedUser.User_NTID,
            //            Modified_Date = c.Modified_Date,
            //            Modified_UID = c.Modified_UID,
            //            EQP_Type1 = eqp.EQP_Type1,
            //            OPType = org.Organization_Name,
            //            BG_Organization_UID=org.Organization_UID,
            //            Funplant = org1.Organization_Name + "---" + org.Organization_Desc,
            //            FunPlantUID=org1.Organization_UID,
            //            Plant = org2.Organization_Name
            //        };
            if (string.IsNullOrEmpty(searchModel.ExportUIds))
            {
                if (searchModel.EQP_Material_UID > 0)
                {
                    query = query.Where(p => p.EQP_Material_UID == searchModel.EQP_Material_UID);
                }
                if (searchModel.Material_Uid > 0)
                {
                    query = query.Where(p => p.Material_Uid == searchModel.Material_Uid);
                }

                if (!string.IsNullOrEmpty(searchModel.Plant))
                {
                    query = query.Where(p => p.Plant.IndexOf(searchModel.Plant) >=0);
                }
                if (searchModel.BG_Organization_UID>0)
                {
                    query = query.Where(p =>p.BG_Organization_UID==searchModel.BG_Organization_UID);
                }
                if (searchModel.FunPlantUID>0)
                {
                    query = query.Where(p => p.FunPlantUID==searchModel.FunPlantUID);
                }

                if (!string.IsNullOrEmpty(searchModel.Desc))
                {
                    query = query.Where(p => p.Desc.Equals(searchModel.Desc));
                }
                if (searchModel.Modified_Date != null)
                {
                    query = query.Where(p => p.Modified_Date >= searchModel.Modified_Date);
                }
                if (!string.IsNullOrWhiteSpace(searchModel.Modified_UserNTID))
                {
                    query = query.Where(p => p.Modified_UserNTID == searchModel.Modified_UserNTID);
                }
                List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
                if (Plant_UIDs.Count > 0)
                {
                    query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
                }
          
                totalcount = query.Count();
                return query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(searchModel.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.EQP_Material_UID));
                List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
                if (Plant_UIDs.Count > 0)
                {
                    query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
                }
                query = query.Distinct();
                totalcount = 0;
                return query.OrderByDescending(o => o.Modified_Date);
            }
        }


        public string InsertItem(List<EQPTypeDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                     
                        var sql = string.Format(@"INSERT	dbo.EQP_Type
                                (
                                  BG_Organization_UID ,
                                  FunPlant_Organization_UID ,
                                  EQP_Type ,
                                  Type_Desc ,
                                  Is_Enable ,
                                  Modified_UID ,
                                  Modified_Date
                                )
                        VALUES  (   
                                  {0} , -- BG_Organization_UID - int
                                  {1} , -- FunPlant_Organization_UID - int
                                  N'{2}' , -- EQP_Type - nvarchar(20)
                                  N'{3}' , -- Type_Desc - nvarchar(50)
                                  {4} , -- Is_Enable - bit
                                  {5} , -- Modified_UID - int
                                 '{6}'  -- Modified_Date - datetime
                                )
                                        ",
                                          
                                            dtolist[i].BG_Organization_UID,
                                            dtolist[i].FunPlant_Organization_UID,
                                            dtolist[i].EQP_Type1,
                                            dtolist[i].Type_Desc,
                                            dtolist[i].Is_Enable ? 1 : 0,
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

     public   string InsertEQPMaterial(List<EQPMaterialDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {

                        var sql = string.Format(@"INSERT INTO [dbo].[EQP_Material]
                                                           ([EQP_Type_UID]
                                                           ,[Material_UID]
                                                           ,[BOM_Qty]
                                                           ,[Desc]
                                                           ,[Is_Enable]
                                                           ,[Modified_UID]
                                                           ,[Modified_Date])
                                                     VALUES
                                                           ({0}
                                                           ,{1}
                                                           ,{2}
                                                           ,N'{3}'
                                                           ,{4}
                                                           ,{5}
                                                           ,'{6}')   ",

                                            dtolist[i].EQP_Type_UID,
                                            dtolist[i].Material_Uid,
                                            dtolist[i].BOM_Qty,
                                            dtolist[i].Desc,
                                            dtolist[i].Is_Enable ? 1 : 0,
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

        public List<EQPMaterialDTO> QueryEQPMaterialsByEqpID(int id)
        {
            var query = from c in DataContext.EQP_Material
                        join modifiedUser in DataContext.System_Users on c.Modified_UID equals modifiedUser.Account_UID
                        join eqp in DataContext.EQP_Type on c.EQP_Type_UID equals eqp.EQP_Type_UID
                        join org in DataContext.System_Organization on eqp.BG_Organization_UID equals org.Organization_UID
                        join org1 in DataContext.System_Organization on eqp.FunPlant_Organization_UID equals org1.Organization_UID
                        join m in DataContext.Material_Info on c.Material_UID equals m.Material_Uid
                        join bom in DataContext.System_OrganizationBOM on eqp.BG_Organization_UID equals bom.ChildOrg_UID
                        join org2 in DataContext.System_Organization on bom.ParentOrg_UID equals org2.Organization_UID
                        where eqp.EQP_Type_UID==id
                        select new EQPMaterialDTO
                        {
                            EQP_Material_UID = c.EQP_Material_UID,
                            Material_Uid = c.Material_UID,
                            EQP_Type_UID = c.EQP_Type_UID,
                            Desc = c.Desc,
                            Modified_UserName = modifiedUser.User_Name,
                            Modified_UserNTID = modifiedUser.User_NTID,
                            Modified_Date = c.Modified_Date,
                            Modified_UID = c.Modified_UID,
                            EQP_Type = eqp.EQP_Type1,
                            OPType = org.Organization_Name,
                            Funplant = org1.Organization_Name + "---" + org.Organization_Desc,
                            Plant = org2.Organization_Name,
                            FunPlantUID = eqp.FunPlant_Organization_UID,
                            BG_Organization_UID=eqp.BG_Organization_UID,
                            Material_Id=m.Material_Id,
                            Material_Name=m.Material_Name,
                            Material_Types=m.Material_Types,
                            BOM_Qty=c.BOM_Qty,
                            Is_Enable=c.Is_Enable,
                            Org_CTU= org.Organization_Name
                        };
            return query.ToList();
        }
        public List<EQPMaterialDTO> QueryEQPMaterialsAll()
        {
            var query = from c in DataContext.EQP_Material
                        join modifiedUser in DataContext.System_Users on c.Modified_UID equals modifiedUser.Account_UID
                        join eqp in DataContext.EQP_Type on c.EQP_Type_UID equals eqp.EQP_Type_UID
                        join org in DataContext.System_Organization on eqp.BG_Organization_UID equals org.Organization_UID
                        join org1 in DataContext.System_Organization on eqp.FunPlant_Organization_UID equals org1.Organization_UID
                        join m in DataContext.Material_Info on c.Material_UID equals m.Material_Uid
                        join bom in DataContext.System_OrganizationBOM on eqp.BG_Organization_UID equals bom.ChildOrg_UID
                        join org2 in DataContext.System_Organization on bom.ParentOrg_UID equals org2.Organization_UID     
                        select new EQPMaterialDTO
                        {
                            EQP_Material_UID = c.EQP_Material_UID,
                            Material_Uid = c.Material_UID,
                            EQP_Type_UID = c.EQP_Type_UID,
                            Desc = c.Desc,
                            Modified_UserName = modifiedUser.User_Name,
                            Modified_UserNTID = modifiedUser.User_NTID,
                            Modified_Date = c.Modified_Date,
                            Modified_UID = c.Modified_UID,
                            EQP_Type = eqp.EQP_Type1,
                            OPType = org.Organization_Name,
                            Funplant = org1.Organization_Name + "---" + org.Organization_Desc,
                            Plant = org2.Organization_Name,
                            FunPlantUID = eqp.FunPlant_Organization_UID,
                            BG_Organization_UID = eqp.BG_Organization_UID,
                            Material_Id = m.Material_Id,
                            Material_Name = m.Material_Name,
                            Material_Types = m.Material_Types,
                            BOM_Qty = c.BOM_Qty,
                            Is_Enable = c.Is_Enable,
                            Org_CTU = org.Organization_Name
                        };
            return query.ToList();
        }

    }
}
