using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Model.EntityDTO;

namespace PDMS.Data.Repository
{
    public interface IGL_LineGroupRepository : IRepository<GL_LineGroup>
    {
        #region  define GroupLine interface Add By Roy 2018/12/18
        /// <summary>
        /// System GroupLine, get grid data
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IQueryable<GroupLineItem> QueryGroupLines(GL_LineModelSearch search, Page page, out int count);

        /// <summary>
        /// Query GroupLine by uid
        /// </summary>
        /// <param name="uid">key</param>
        /// <returns></returns>
        GL_LineGroupDTO QueryGroupLine(int uid);
        List<GL_LineGroupDTO> GetGroupLine(int? Optype, int? Optypes, int? opFunPlant,int? customerId);
        List<SubLineItem> GetSubLine(int Oporgid, int Optype);
        #endregion //End GroupLine
    }

    public class GL_LineGroupRepository : RepositoryBase<GL_LineGroup>, IGL_LineGroupRepository
    {
        public GL_LineGroupRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public IQueryable<GroupLineItem> QueryGroupLines(GL_LineModelSearch search, Page page, out int count)
        {
            var query = from c in DataContext.GL_LineGroup
                        join s1 in DataContext.System_Organization on c.Plant_Organization_UID equals s1.Organization_UID
                        join s2 in DataContext.System_Organization on c.BG_Organization_UID equals s2.Organization_UID
                        join s3 in DataContext.System_Organization on c.FunPlant_Organization_UID equals s3.Organization_UID into t3
                        from s3 in t3.DefaultIfEmpty()
                        join s4 in DataContext.GL_LineGroup on c.LineParent_ID equals s4.LineGroup_UID into t4
                        from s4 in t4.DefaultIfEmpty()
                        join u in DataContext.System_Users on c.Modified_UID equals u.Account_UID
                        join s5 in DataContext.System_Project on c.CustomerID equals s5.Project_UID into t5
                        from s5 in t5.DefaultIfEmpty()
                        
                        select new GroupLineItem
                        {
                            LineGroup_UID = c.LineGroup_UID,
                            Plant_Organization_UID = c.Plant_Organization_UID,
                            BG_Organization_UID = c.BG_Organization_UID,
                            FunPlant_Organization_UID = c.FunPlant_Organization_UID,
                            LineID = c.LineID,
                            LineParent_ID = c.LineParent_ID,
                            LineParent_Name =s4.LineName,
                            LineName = c.LineName,
                            factory = s1.Organization_Name,
                            op_Type = s2.Organization_Name,
                            funPlant = s3.Organization_Name,
                            Modified_UserName = u.User_Name,
                            Modified_Date = c.Modified_Date,
                            CustomerID = c.CustomerID,
                            Project_Name = s5.Project_Name,
                            MESProject_Name = s5.MESProject_Name
                        };

            if (search.Plant_Organization_UID != 0 && search.Plant_Organization_UID != null)
            {
                query = query.Where(p => p.Plant_Organization_UID == search.Plant_Organization_UID);
            }

            if (search.BG_Organization_UID != 0 && search.BG_Organization_UID != null)
            {
                query = query.Where(p => p.BG_Organization_UID == search.BG_Organization_UID);
            }

            if (search.FunPlant_Organization_UID != 0 && search.FunPlant_Organization_UID != null)
            {
                query = query.Where(p => p.FunPlant_Organization_UID == search.FunPlant_Organization_UID);
            }

            if (search.LineID != 0 && search.LineID!= null)
            {
                query = query.Where(p => p.LineGroup_UID == search.LineID || p.LineParent_ID == search.LineID);
            }

            if (search.CustomerID != 0 && search.CustomerID != null)
            {
                query = query.Where(p => p.CustomerID == search.CustomerID);
            }

            query = query.Where(o => o.MESProject_Name != "" && o.MESProject_Name != null);

            count = query.Count();
            return query
                        .OrderBy(o => o.Plant_Organization_UID)
                        .ThenBy(o => o.BG_Organization_UID)
                        .ThenBy(o => o.FunPlant_Organization_UID)
                        .ThenBy(o => o.LineGroup_UID)
                        .GetPage(page);
        }

        public List <GL_LineGroupDTO> GetGroupLine(int? oporgid, int? Optypes, int? opFunPlant,int? customerId)
        {
            var query = from c in DataContext.GL_LineGroup
                        where c.LineID == null 
                        orderby c.LineGroup_UID
                        select new GL_LineGroupDTO
                        {
                            LineGroup_UID = c.LineGroup_UID,
                            Plant_Organization_UID = c.Plant_Organization_UID,
                            BG_Organization_UID = c.BG_Organization_UID,
                            FunPlant_Organization_UID = c.FunPlant_Organization_UID,
                            LineID = c.LineID,
                            LineParent_ID = c.LineParent_ID,
                            LineName = c.LineName,
                            _Modified_UID = c.Modified_UID,
                            _Modified_Date = c.Modified_Date,
                            CustomerID = c.CustomerID
                        };
            if (oporgid != 0 && oporgid != null)
                query = query.Where(m => m.Plant_Organization_UID == oporgid);
            if (Optypes != 0 && Optypes != null)
                query = query.Where(m => m.BG_Organization_UID == Optypes);
            if (opFunPlant != 0 && opFunPlant != null)
                query = query.Where(m => m.FunPlant_Organization_UID == opFunPlant);
            if (customerId != 0 && customerId != null)
                query = query.Where(m => m.CustomerID == customerId);

            return query.ToList();
        }

        public List<SubLineItem> GetSubLine(int Oporgid, int Optype)
        {
            var query = from c in DataContext.GL_Line
                        where c.IsEnabled == true && c.Plant_Organization_UID== Oporgid && c.BG_Organization_UID== Optype
                        orderby c.LineID
                        select new SubLineItem
                        {
                            Line_ID = c.LineID,
                            LineName = c.LineName
                        };

            return query.ToList();
        }

        /// <summary>
        /// Query GroupLine by uid
        /// </summary>
        /// <param name="uid">key</param>
        /// <returns></returns>
        public GL_LineGroupDTO QueryGroupLine(int uid)
        {
            var query = from c in DataContext.GL_LineGroup
                        join s4 in DataContext.GL_LineGroup on c.LineParent_ID equals s4.LineGroup_UID into t4
                        from s4 in t4.DefaultIfEmpty()
                        join s5 in DataContext.System_Project on c.CustomerID equals s5.Project_UID into t5
                        from s5 in t5.DefaultIfEmpty()
                        where c.LineGroup_UID == uid
                        select new GL_LineGroupDTO
                        {
                            LineGroup_UID =c.LineGroup_UID,
                            Plant_Organization_UID = c.Plant_Organization_UID,
                            BG_Organization_UID = c.BG_Organization_UID,
                            FunPlant_Organization_UID = c.FunPlant_Organization_UID,
                            LineID = c.LineID,
                            LineParent_ID = c.LineParent_ID,
                            LineParent_Name = s4.LineName,
                            LineName = c.LineName,
                            _Modified_UID = c.Modified_UID,
                            _Modified_Date = c.Modified_Date,
                            CustomerID = c.CustomerID,
                            Project_Name = s5.Project_Name,
                            MESProject_Name = s5.MESProject_Name
                        };
            query = query.Where(o => o.MESProject_Name != "" && o.MESProject_Name != null);
            return query.FirstOrDefault();
        }

    }
}
