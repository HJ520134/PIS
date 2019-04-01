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
    public interface IEQP_TypeRepository : IRepository<EQP_Type>
    {
        IQueryable<EQPTypeDTO> QueryEQPTypes(EQPTypeDTO searchModel, Page page, out int totalcount);
        List<EQPTypeBaseDTO> GetEQPTypes();
        List<EQPTypeDTO> DoAllExportEQPTypeReprot(EQPTypeDTO searchModel);
        List<EQPTypeDTO> DoExportEQPTypeReprot(string uids);
        List<EQPTypeDTO> GetEQPTypeAlls();
    }
    public class EQP_TypeRepository: RepositoryBase<EQP_Type>, IEQP_TypeRepository
    {
        public EQP_TypeRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }


        public List<EQPTypeBaseDTO> GetEQPTypes()
        {
            var query = from c in DataContext.EQP_Type
                        select new EQPTypeBaseDTO
                        {
                            BG_Organization_UID=c.BG_Organization_UID,
                            FunPlant_Organization_UID=c.FunPlant_Organization_UID,
                            EQP_Type1=c.EQP_Type1
                        };

                return query.ToList();
          
        }

        public List<EQPTypeDTO> GetEQPTypeAlls()
        {
            var query = from c in DataContext.EQP_Type
                        join modifiedUser in DataContext.System_Users on c.Modified_UID equals modifiedUser.Account_UID
                        join org in DataContext.System_Organization on c.FunPlant_Organization_UID equals org.Organization_UID
                        join org1 in DataContext.System_Organization on c.BG_Organization_UID equals org1.Organization_UID
                        select new EQPTypeDTO
                        {
                            EQP_Type_UID = c.EQP_Type_UID,
                            BG_Organization_UID = c.BG_Organization_UID,
                            BG = org1.Organization_Name,
                            EQP_Type1 = c.EQP_Type1,
                            FunPlant_Organization_UID = c.FunPlant_Organization_UID,
                            FunPlant = org.Organization_Name,
                            Type_Desc = c.Type_Desc,
                            Modified_UserName = modifiedUser.User_Name,
                            Modified_UserNTID = modifiedUser.User_NTID,
                            Modified_Date = c.Modified_Date,
                            Modified_UID = c.Modified_UID,
                            Is_Enable = c.Is_Enable,
                        };
            return query.ToList();
        }
        public IQueryable<EQPTypeDTO> QueryEQPTypes(EQPTypeDTO searchModel, Page page, out int totalcount)
        {
            var query = from c in DataContext.EQP_Type
                        join modifiedUser in DataContext.System_Users on c.Modified_UID equals modifiedUser.Account_UID
                        join org in DataContext.System_Organization on c.FunPlant_Organization_UID equals org.Organization_UID
                        join org1 in DataContext.System_Organization on c.BG_Organization_UID equals org1.Organization_UID
                        select new EQPTypeDTO
                        {
                            EQP_Type_UID = c.EQP_Type_UID,
                            BG_Organization_UID = c.BG_Organization_UID,
                            BG= org1.Organization_Name,
                            EQP_Type1 = c.EQP_Type1,
                            FunPlant_Organization_UID=c.FunPlant_Organization_UID,
                            FunPlant=org.Organization_Name,
                            Type_Desc=c.Type_Desc,
                            Modified_UserName = modifiedUser.User_Name,
                            Modified_UserNTID = modifiedUser.User_NTID,
                            Modified_Date = c.Modified_Date,
                            Modified_UID = c.Modified_UID,
                            Is_Enable=c.Is_Enable,
                        };
            if (string.IsNullOrEmpty(searchModel.ExportUIds))
            {
                if (searchModel.EQP_Type_UID > 0)
                {
                    query = query.Where(p => p.EQP_Type_UID == searchModel.EQP_Type_UID);
                }
                if (searchModel.BG_Organization_UID > 0)
                {
                    query = query.Where(p => p.BG_Organization_UID == searchModel.BG_Organization_UID);
                }
                if (searchModel.FunPlant_Organization_UID > 0)
                {
                    query = query.Where(p => p.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
                }
                if (!string.IsNullOrEmpty(searchModel.FunPlant)&& searchModel.FunPlant!="Nothing")
                {
                    query = query.Where(p => p.FunPlant.Contains(searchModel.FunPlant));
                }
                if (!string.IsNullOrEmpty(searchModel.EQP_Type1))
                {
                    query = query.Where(p => p.EQP_Type1.IndexOf(searchModel.EQP_Type1)>=0);
                }
                if (!string.IsNullOrEmpty(searchModel.Type_Desc))
                {
                    query = query.Where(p => p.Type_Desc.IndexOf(searchModel.Type_Desc) >= 0);
                }
                if (searchModel.Modified_Date != null)
                {
                    query = query.Where(p => p.Modified_Date >= searchModel.Modified_Date);
                }
                if (!string.IsNullOrWhiteSpace(searchModel.Modified_UserNTID))
                {
                    query = query.Where(p => p.Modified_UserNTID == searchModel.Modified_UserNTID);
                }
                if (searchModel.Modified_Date_From != null)
                {
                    query = query.Where(p => p.Modified_Date >= searchModel.Modified_Date_From);
                }
                if (searchModel.Modified_Date_End != null)
                {
                    var endDate = ((DateTime)searchModel.Modified_Date_End).AddDays(1);
                    query = query.Where(p => p.Modified_Date < endDate);
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
                query = query.Where(p => array.Contains(p.EQP_Type_UID));

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
        public List<EQPTypeDTO> DoAllExportEQPTypeReprot(EQPTypeDTO searchModel)
        {
            var query = from c in DataContext.EQP_Type
                        join modifiedUser in DataContext.System_Users on c.Modified_UID equals modifiedUser.Account_UID
                        join org in DataContext.System_Organization on c.FunPlant_Organization_UID equals org.Organization_UID
                        join org1 in DataContext.System_Organization on c.BG_Organization_UID equals org1.Organization_UID
                        select new EQPTypeDTO
                        {
                            EQP_Type_UID = c.EQP_Type_UID,
                            BG_Organization_UID = c.BG_Organization_UID,
                            BG = org1.Organization_Name,
                            EQP_Type1 = c.EQP_Type1,
                            FunPlant_Organization_UID = c.FunPlant_Organization_UID,
                            FunPlant = org.Organization_Name,
                            Type_Desc = c.Type_Desc,
                            Modified_UserName = modifiedUser.User_Name,
                            Modified_UserNTID = modifiedUser.User_NTID,
                            Modified_Date = c.Modified_Date,
                            Modified_UID = c.Modified_UID,
                            Is_Enable = c.Is_Enable,
                        };
            if (string.IsNullOrEmpty(searchModel.ExportUIds))
            {
                if (searchModel.EQP_Type_UID > 0)
                {
                    query = query.Where(p => p.EQP_Type_UID == searchModel.EQP_Type_UID);
                }
                if (searchModel.BG_Organization_UID > 0)
                {
                    query = query.Where(p => p.BG_Organization_UID == searchModel.BG_Organization_UID);
                }
                if (searchModel.FunPlant_Organization_UID > 0)
                {
                    query = query.Where(p => p.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
                }
                if (!string.IsNullOrEmpty(searchModel.FunPlant) && searchModel.FunPlant != "Nothing")
                {
                    query = query.Where(p => p.FunPlant.Contains(searchModel.FunPlant));
                }
                if (!string.IsNullOrEmpty(searchModel.EQP_Type1))
                {
                    query = query.Where(p => p.EQP_Type1.IndexOf(searchModel.EQP_Type1) >= 0);
                }
                if (!string.IsNullOrEmpty(searchModel.Type_Desc))
                {
                    query = query.Where(p => p.Type_Desc.IndexOf(searchModel.Type_Desc) >= 0);
                }
                if (searchModel.Modified_Date != null)
                {
                    query = query.Where(p => p.Modified_Date >= searchModel.Modified_Date);
                }
                if (!string.IsNullOrWhiteSpace(searchModel.Modified_UserNTID))
                {
                    query = query.Where(p => p.Modified_UserNTID == searchModel.Modified_UserNTID);
                }
                if (searchModel.Modified_Date_From != null)
                {
                    query = query.Where(p => p.Modified_Date >= searchModel.Modified_Date_From);
                }
                if (searchModel.Modified_Date_End != null)
                {
                    var endDate = ((DateTime)searchModel.Modified_Date_End).AddDays(1);
                    query = query.Where(p => p.Modified_Date < endDate);
                }
                List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
                if (Plant_UIDs.Count > 0)
                {
                    query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
                }              
                return query.ToList();
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(searchModel.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.EQP_Type_UID));

                List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
                if (Plant_UIDs.Count > 0)
                {
                    query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
                }


                return query.ToList();
            }
        }
        public List<EQPTypeDTO> DoExportEQPTypeReprot(string uids)
        {
            uids = "," + uids + ",";
            var query = from c in DataContext.EQP_Type
                        join modifiedUser in DataContext.System_Users on c.Modified_UID equals modifiedUser.Account_UID
                        join org in DataContext.System_Organization on c.FunPlant_Organization_UID equals org.Organization_UID
                        join org1 in DataContext.System_Organization on c.BG_Organization_UID equals org1.Organization_UID
                        select new EQPTypeDTO
                        {
                            EQP_Type_UID = c.EQP_Type_UID,
                            BG_Organization_UID = c.BG_Organization_UID,
                            BG = org1.Organization_Name,
                            EQP_Type1 = c.EQP_Type1,
                            FunPlant_Organization_UID = c.FunPlant_Organization_UID,
                            FunPlant = org.Organization_Name,
                            Type_Desc = c.Type_Desc,
                            Modified_UserName = modifiedUser.User_Name,
                            Modified_UserNTID = modifiedUser.User_NTID,
                            Modified_Date = c.Modified_Date,
                            Modified_UID = c.Modified_UID,
                            Is_Enable = c.Is_Enable,
                        };
            query = query.Where(m => uids.Contains("," + m.EQP_Type_UID + ","));

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

       
    }
}
