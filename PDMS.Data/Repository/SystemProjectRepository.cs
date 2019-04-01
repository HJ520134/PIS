using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PDMS.Common.Helpers;

namespace PDMS.Data.Repository
{

    public class SystemProjectRepository : RepositoryBase<System_Project>, ISystemProjectRepository
    {
        private Logger log = new Logger("SystemProjectRepository ");

        public SystemProjectRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
            
        }
        public IQueryable<string> QueryDistinctProject(string customer,List<int> orgs)
        {
            if (orgs != null && orgs.Count > 0)
            {
                var query = from bud in DataContext.System_BU_D
                            join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                            where (bud.BU_D_Name == customer) & orgs.Contains(project.Organization_UID)
                            select (project.Project_Name);
                return query.Distinct();
            }
            else
            {
                var query = from bud in DataContext.System_BU_D
                            join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                            where (bud.BU_D_Name == customer)
                            select (project.Project_Name);
                return query.Distinct();
            }
        }
        public IQueryable<string> QueryDistinctProjectAPP(string customer, List<int> orgs)
        {
            if (orgs != null && orgs.Count > 0)
            {
                var query = from bud in DataContext.System_BU_D
                            join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                            join M in DataContext.FlowChart_Master on project.Project_UID equals M.Project_UID
                            where (bud.BU_D_Name == customer) & orgs.Contains(project.Organization_UID) & M.Is_Closed == false
                            select (project.Project_Name);
                return query.Distinct();
            }
            else
            {
                var query = from bud in DataContext.System_BU_D
                            join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                            join M in DataContext.FlowChart_Master on project.Project_UID equals M.Project_UID
                            where (bud.BU_D_Name == customer) & M.Is_Closed == false
                            select (project.Project_Name);
                return query.Distinct();
            }
        }
        public IQueryable<string> QueryDistinctProductPhase(string customername, string projectname)
        {
            var query = from bud in DataContext.System_BU_D
                        join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                        where (bud.BU_D_Name == customername && project.Project_Name == projectname)
                        select (project.Product_Phase);
            return query.Distinct();
        }

        public IQueryable<string> GetProjectPhaseSource(int Plant, int projectname)
        {
            var query = from  project in DataContext.System_Project 
                        join orgbom in DataContext.System_OrganizationBOM on project.Organization_UID equals orgbom.ChildOrg_UID
                        join org in DataContext.System_Organization on orgbom.ParentOrg_UID equals org.Organization_UID
                        where (org.Organization_UID == Plant && project.Project_UID == projectname)
                        select (project.Product_Phase);
            return query.Distinct();
        }
        public string GetSelctOP(string customername, string projectname)
        {
            var query = from bud in DataContext.System_BU_D
                        join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                        where bud.BU_D_Name==customername && project.Project_Name==projectname
                        select (project.OP_TYPES);
            return query.FirstOrDefault();
        }

        /// <summary>
        /// APP只查询最新版本的
        /// </summary>
        /// <param name="projectname"></param>
        /// <returns></returns>
        public IQueryable<string> QueryDistinctProductPhaseAPP( string projectname)
        {
            var query = from project in DataContext.System_Project
                        join master in DataContext.FlowChart_Master on project.Project_UID equals master.Project_UID
                        where (project.Project_Name == projectname&& master.Is_Latest==true)
                        select (project.Product_Phase);
                        
            return query.Distinct();
        }

        public IQueryable<string> QueryOpenProject(List<int> orgs)
        {
            if (orgs != null && orgs.Count > 0)
            {
                var query = from
                            project in DataContext.System_Project
                            join M in DataContext.FlowChart_Master on project.Project_UID equals M.Project_UID
                            where orgs.Contains(project.Organization_UID) & M.Is_Closed == false
                            select project.Project_Name;
                return query.Distinct();
            }
            else
            {
                var query = from project in DataContext.System_Project
                            join M in DataContext.FlowChart_Master on project.Project_UID equals M.Project_UID
                            where M.Is_Closed == false
                            select project.Project_Name;
                return query.Distinct();
            }
        }
        public List<SystemPro> GetProjectList(int opid, List<UserRoleJ> roles)
        {
            //var sqlStr = @"select DISTINCT op_types OPTypes,Project_Name Project from
            //                dbo.System_Project AS sp
            //                WHERE OP_Types IN (SELECT DISTINCT  Organization_Name from
            //                dbo.System_Organization AS so

            //                WHERE Organization_ID ='" + opid + "%'AND Organization_ID LIKE '2%')";

            List<int> roleIDs = new List<int>();
            bool flag = false;
            foreach (var item in roles)
            {
                if (item.UserRoleID == "SystemAdmin")
                    flag = true;
                roleIDs.Add(item.RoleUID);
            }
            if (flag||opid==0) {
                var dbList = from p in DataContext.System_Project
                             join org in DataContext.System_Organization on p.Organization_UID equals org.Organization_UID
                             join bom in DataContext.System_OrganizationBOM on p.Organization_UID equals bom.ChildOrg_UID
                             join org2 in DataContext.System_Organization on bom.ParentOrg_UID equals org2.Organization_UID
                             select new SystemPro
                             {
                                 OPTypes = org.Organization_UID.ToString(),
                                 Project = p.Project_Name,
                                 ProjectID=p.Project_UID,
                                 plant = org2.Organization_Name
                             };
                return dbList.ToList();
            }
            else
            {
                var dbList = from p in DataContext.System_Project
                             join org in DataContext.System_Organization on p.Organization_UID equals org.Organization_UID
                             join bom in DataContext.System_OrganizationBOM on p.Organization_UID equals bom.ChildOrg_UID
                             join org2 in DataContext.System_Organization on bom.ParentOrg_UID equals org2.Organization_UID
                             where p.Organization_UID == opid
                             select new SystemPro
                             {
                                 OPTypes = org.Organization_UID.ToString(),
                                 Project = p.Project_Name,
                                 ProjectID = p.Project_UID,
                                 plant = org2.Organization_Name
                             };
                return dbList.ToList();
            }
        }
        public List<string> GetProjectByOp(string Optype)
        {
            var query = from P in DataContext.System_Project
                        join M in DataContext.FlowChart_Master on P.Project_UID equals M.Project_UID
                        where P.OP_TYPES == Optype && M.Is_Closed != true
                        select P.Project_Name + "_" + M.Part_Types + "_" + P.Product_Phase;
            return query.Distinct().ToList();
        }

        public IQueryable<ProjectVM> QueryProjects(ProjectSearchModel search, Page page,bool flag,out int totalCount)
        {
            var query = from project in DataContext.System_Project
                join bu in DataContext.System_BU_D on project.BU_D_UID equals bu.BU_D_UID
                join bom in DataContext.System_OrganizationBOM on project.Organization_UID equals bom.ChildOrg_UID
                join org in DataContext.System_Organization on bom.ParentOrg_UID equals org.Organization_UID
                select new ProjectVM()
                {
                    Project_UID = project.Project_UID,
                    Project = project.Project_Name,
                    MESProject = project.MESProject_Name,
                    Customer = bu.BU_D_Name,
                    Product_Phase = project.Product_Phase,
                    OP_TYPES = project.OP_TYPES,
                    Modified_Date = project.Modified_Date,
                    Modified_User = project.System_Users.User_Name,
                    OrgID=project.Organization_UID,
                    Organization_Name=bom.ParentOrg_UID,
                    Plant=org.Organization_Name
                };
            
            if (!string.IsNullOrWhiteSpace(search.Project))
            {
                query = query.Where(m => m.Project.Contains(search.Project));
            }
            if (!string.IsNullOrWhiteSpace(search.MESProject))
            {
                query = query.Where(m => m.MESProject.Contains(search.MESProject));
            }
            if (!string.IsNullOrWhiteSpace(search.Customer))
            {
                query = query.Where(m => m.Customer == search.Customer);
            }
            if (!string.IsNullOrWhiteSpace(search.Product_Phase))
            {
                query = query.Where(m => m.Product_Phase == search.Product_Phase);
            }
            if (search.Organization_UID != 0)
                query = query.Where(m => m.OrgID == search.Organization_UID);
            if (search.Organization_Name != 0)
                query = query.Where(m => m.Organization_Name == search.Organization_Name);
            if (!flag&& search.OrgID > 0)
            {
              
                    query = query.Where(p => p.OrgID == search.OrgID);
                
            }
            totalCount = query.Count();
            return query.OrderByDescending(o => o.Modified_Date).GetPage(page);
        }


        public List<System_Project> GetProjects(int oporgid)
        {           
            var sqlStr = @"SELECT * FROM dbo.System_Project WHERE Organization_UID={0}";
            sqlStr = string.Format(sqlStr, oporgid);
            var dbList = DataContext.Database.SqlQuery<System_Project>(sqlStr).ToList();
            return dbList;
        }
        
        public List<System_Function_Plant> GetFunplants(int oporgid,string optypes)
        {
          
            var sqlStr = @"select count(*) from (SELECT  distinct [System_Plant_UID],[OP_Types],[FunPlant]FROM dbo.System_Function_Plant WHERE OPType_OrganizationUID={0}) as a";
            if (optypes != ""&& optypes!=null)
            {
                sqlStr = @"select count(*) from (SELECT  distinct [System_Plant_UID],[OP_Types],[FunPlant]FROM dbo.System_Function_Plant WHERE OPType_OrganizationUID={0} and OP_Types='{1}') as a";
            }
            sqlStr = string.Format(sqlStr, oporgid,optypes);
            var num = DataContext.Database.SqlQuery<int>(sqlStr).FirstOrDefault();
            sqlStr = @"SELECT Top {1} * FROM dbo.System_Function_Plant WHERE OPType_OrganizationUID={0}";
           if (optypes != "" && optypes != null)
            {
                sqlStr = @"SELECT Top {1} * FROM dbo.System_Function_Plant WHERE OPType_OrganizationUID={0} and OP_Types='{2}'";
            }
            sqlStr = string.Format(sqlStr, oporgid,num,optypes);


            var dbList = DataContext.Database.SqlQuery<System_Function_Plant>(sqlStr).ToList();
            return dbList;
        }

        public List<SystemProjectDTO> GetAllProjects(int optype)
        {
            
            var sqlStr = @"SELECT * FROM dbo.System_Project";
            if (optype!=0)
            {
                sqlStr += " where Organization_UID={0}";
                sqlStr = string.Format(sqlStr, optype);
            }
            var dbList = DataContext.Database.SqlQuery<SystemProjectDTO>(sqlStr).ToList();
            return dbList;
        }


        public List<SystemProjectDTO> GetOpType(int oporguid,int accuntid =0)
        {

            var sqlStr = "";
            if (oporguid != 0)
            {
                sqlStr = string.Format(@"SELECT DISTINCT t2.OP_TYPES,t2.Organization_UID FROM 
                                    dbo.Equipment_Info t1 INNER JOIN dbo.System_Project t2
                                    ON t2.Project_UID = t1.Project_UID where Organization_UID={0}", oporguid);
            }
            else
            {
                if(accuntid == 0)
                {
                    sqlStr = @"SELECT DISTINCT t2.OP_TYPES,t2.Organization_UID FROM 
                                    dbo.Equipment_Info t1 INNER JOIN dbo.System_Project t2
                                    ON t2.Project_UID = t1.Project_UID ";
                }else
                {
                    sqlStr = string.Format(@"  SELECT DISTINCT t2.OP_TYPES,t2.Organization_UID FROM   dbo.Equipment_Info t1 INNER JOIN dbo.System_Project t2 ON t2.Project_UID = t1.Project_UID where Organization_UID IN(	select  BG_Organization_UID   FROM EQP_UserTable WHERE EQPUser_Uid={0})", accuntid);
                }
                
                // sqlStr = @"SELECT '' OP_TYPES,0 Organization_UID UNION SELECT DISTINCT t2.OP_TYPES,t2.Organization_UID FROM   dbo.System_Project t2 ";
            }
            var dbList = DataContext.Database.SqlQuery<SystemProjectDTO>(sqlStr).ToList();
            return dbList;
        }

        public List<SystemProjectDTO> GetCurrentOPType(int parentOrg_UID, int organization_UID)
        {

            var sqlStr = string.Format(@"SELECT DISTINCT t2.OP_TYPES,t2.Organization_UID FROM dbo.System_Project t2 WHERE t2.Organization_UID IN (SELECT A.Organization_UID FROM
                                        dbo.System_Organization A INNER  JOIN dbo.System_OrganizationBOM B ON a.Organization_UID=B.ChildOrg_UID WHERE A.Organization_ID LIKE'2000%' 
                                        ) ");

            if (parentOrg_UID > 0)
            {
                sqlStr = string.Format(@"SELECT DISTINCT t2.OP_TYPES,t2.Organization_UID FROM dbo.System_Project t2 WHERE t2.Organization_UID IN (SELECT A.Organization_UID FROM
                                        dbo.System_Organization A INNER  JOIN dbo.System_OrganizationBOM B ON a.Organization_UID=B.ChildOrg_UID WHERE A.Organization_ID LIKE'2000%' AND B.ParentOrg_UID={0}
                                        ) ", parentOrg_UID);
            }

            if (organization_UID != 0)
            {
                sqlStr += string.Format(@" AND t2.Organization_UID={0}", organization_UID);
            }

            var dbList = DataContext.Database.SqlQuery<SystemProjectDTO>(sqlStr).ToList();
            return dbList;

        }

        public List<SystemProjectDTO> GetCurrentFixtureOPType(int parentOrg_UID, int organization_UID)
        {

            var sqlStr = string.Format(@" SELECT A.Organization_UID,A.Organization_Name  AS OP_TYPES FROM
                                        dbo.System_Organization A INNER  JOIN dbo.System_OrganizationBOM B ON a.Organization_UID=B.ChildOrg_UID WHERE A.Organization_ID LIKE'20%'
                                       ");

            if (parentOrg_UID > 0)
            {
                sqlStr = string.Format(@" SELECT A.Organization_UID,A.Organization_Name  AS OP_TYPES FROM
                                        dbo.System_Organization A INNER  JOIN dbo.System_OrganizationBOM B ON a.Organization_UID=B.ChildOrg_UID WHERE A.Organization_ID LIKE'20%' AND B.ParentOrg_UID={0}
                                       ", parentOrg_UID);
            }

            if (organization_UID != 0)
            {
                sqlStr += string.Format(@" AND A.Organization_UID={0}", organization_UID);
            }

            var dbList = DataContext.Database.SqlQuery<SystemProjectDTO>(sqlStr).ToList();
            return dbList;

        }




        public string GetCurrentOPType(int currentUser)
        {
            var strSql = @";WITH one AS
                        (
	                        --递归获取所有相关父子节点信息
	                        SELECT * FROM System_OrganizationBOM WHERE ChildOrg_UID=(SELECT TOP 1 Organization_UID FROM dbo.System_UserOrg WHERE Account_UID='90004')
	                        UNION ALL
                            SELECT h.* FROM dbo.System_OrganizationBOM h JOIN one h1 ON h.ChildOrg_UID = h1.ParentOrg_UID
                        ),
                        two AS
                        (
	                        SELECT A.* FROM dbo.System_Organization A
	                        JOIN one
	                        ON one.ChildOrg_UID = A.Organization_UID
                        )
                        SELECT two.Organization_Name FROM two WHERE two.Organization_ID LIKE '2%'";
            strSql = string.Format(strSql, currentUser);
            var dbList = DataContext.Database.SqlQuery<string>(strSql).ToList();
            string result = string.Empty;
            if (dbList.Count > 0)
                result = dbList[0].ToString();
            return result;
        }

        public List<PartTypeVM> GetPartType(int Project_UID)
        {
            List<PartTypeVM> result = new List<PartTypeVM>();
            try
            {
                var queryProject = from flowMaster in DataContext.FlowChart_Master
                                   where flowMaster.Is_Closed == false && flowMaster.Project_UID == Project_UID
                                   select new PartTypeVM
                                   {
                                       FlowChart_Master_UID = flowMaster.FlowChart_Master_UID,
                                       Part_Type = flowMaster.Part_Types
                                   };
                result = queryProject.Distinct().ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }

        public string GetProjectSite(int uid)
        {
            var strSql = @"SELECT Organization_Name FROM dbo.System_Organization WHERE Organization_UID =
                            (
	                            SELECT ParentOrg_UID FROM dbo.System_OrganizationBOM WHERE ChildOrg_UID=
	                            (
		                            SELECT Organization_UID FROM dbo.System_Project WHERE Project_UID=
		                            (
			                            SELECT TOP 1 A.Project_UID FROM dbo.FlowChart_Master A
			                            JOIN dbo.FlowChart_Detail B
			                            ON A.FlowChart_Master_UID = B.FlowChart_Master_UID AND A.FlowChart_Version = B.FlowChart_Version
			                            WHERE B.FlowChart_Detail_UID = 
				                            (
					                            SELECT TOP 1 FlowChart_Detail_UID FROM dbo.FlowChart_PC_MH_Relationship WHERE MH_UID={0} ORDER BY Modified_Date DESC
				                            )
		                            )
	                            )
                            )";
            strSql = string.Format(strSql, uid);
            var value = DataContext.Database.SqlQuery<string>(strSql).FirstOrDefault();
            return value;
        }
        public List<string> GetUserBu(int OrgUID)
        {
            var query = from BO in DataContext.System_BU_D_Org
                        join A in DataContext.System_BU_D on BO.BU_D_UID equals A.BU_D_UID
                        join Org in DataContext.System_Organization on BO.Organization_UID equals Org.Organization_UID
                        where Org.Organization_UID == OrgUID
                        select A.BU_D_Name;
            return query.ToList();
        }
    }


  
    public interface ISystemProjectRepository : IRepository<System_Project>
    {
        IQueryable<string> QueryDistinctProject(string customer,List<int> orgs);
        IQueryable<string> QueryDistinctProductPhase(string customer, string project);
        IQueryable<string> GetProjectPhaseSource(int Plant, int projectname);
         IQueryable<string> QueryDistinctProductPhaseAPP( string project);
        string GetSelctOP(string customer, string project);
        List<SystemPro> GetProjectList(int opid, List<UserRoleJ> roles);
        IQueryable<ProjectVM> QueryProjects(ProjectSearchModel search, Page page, bool flag, out int totalCount);
        List<string> GetProjectByOp(string Optype);
        IQueryable<string> QueryOpenProject(List<int> orgs);
        IQueryable<string> QueryDistinctProjectAPP(string customer, List<int> orgs);
        List<System_Project> GetProjects(int oporgid);
        List<System_Function_Plant> GetFunplants (int oporgid, string optypes);
        List<SystemProjectDTO> GetAllProjects(int optype);
        List<SystemProjectDTO> GetOpType(int oporguid, int accuntid = 0);
        List<SystemProjectDTO> GetCurrentOPType(int parentOrg_UID, int organization_UID);
        List<SystemProjectDTO> GetCurrentFixtureOPType(int parentOrg_UID, int organization_UID);
        string GetCurrentOPType(int currentUser);
        List<PartTypeVM> GetPartType(int Project_UID);
        string GetProjectSite(int uid);
        List<string> GetUserBu(int uid);
    }
}
