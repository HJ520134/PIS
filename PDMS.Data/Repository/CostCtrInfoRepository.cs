using PDMS.Data.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Data;
using PDMS.Model;

namespace PDMS.Data.Repository
{

    public interface ICostCtrInfoRepository : IRepository<CostCtr_info>
    {
        #region  define CostCenterMaintenace interface Add By Darren 2018/12/18
        /// <summary>
        /// System Cost Center Maintenace, get grid data
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IQueryable<CostCenterItem> QueryCostCenters(CostCenterModelSearch search, Page page, out int count);

        /// <summary>
        /// Query Cost Center Maintenace by uid
        /// </summary>
        /// <param name="uid">key</param>
        /// <returns></returns>
        CostCtr_infoDTO QueryCostCenter(int uid);

        #endregion //End CostCenterMaintenace

        /// <summary>
        /// get all Cost Center Data
        /// </summary>
        /// <returns></returns>
        List<CostCtrDTO> GetCostCtrs();
    }

    public class CostCtrInfoRepository : RepositoryBase<CostCtr_info>, ICostCtrInfoRepository
    {
        public CostCtrInfoRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        #region CostCenterMaintenace Module Add By Darren 2018/12/18
        /// <summary>
        /// get Cost Centers
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IQueryable<CostCenterItem> QueryCostCenters(CostCenterModelSearch search, Page page, out int count)
        {
            var query = from c in DataContext.CostCtr_info
                        join s1 in DataContext.System_Organization on c.Plant_Organization_UID equals s1.Organization_UID
                        join s2 in DataContext.System_Organization on c.BG_Organization_UID equals s2.Organization_UID
                        join s3 in DataContext.System_Organization on c.FunPlant_Organization_UID equals s3.Organization_UID into t3
                        from s3 in t3.DefaultIfEmpty()
                        join u in DataContext.System_Users on c.Modified_UID equals u.Account_UID
                        select new CostCenterItem
                        {
                            Plant_Organization_UID = c.Plant_Organization_UID,
                            BG_Organization_UID = c.BG_Organization_UID,
                            FunPlant_Organization_UID = c.FunPlant_Organization_UID,
                            CostCtr_ID = c.CostCtr_ID,
                            CostCtr_Description = c.CostCtr_Description,
                            CostCtr_UID = c.CostCtr_UID,
                            factory = s1.Organization_Name,
                            op_Type = s2.Organization_Name,
                            funPlant = s3.Organization_Name,
                            Modified_UserName = u.User_Name,
                            Modified_Date = c.Modified_Date
                        };

            if (search.Plant_Organization_UID != 0)
            {
                query = query.Where(p => p.Plant_Organization_UID == search.Plant_Organization_UID);
            }

            if (search.BG_Organization_UID != 0)
            {
                query = query.Where(p => p.BG_Organization_UID == search.BG_Organization_UID);
            }

            if (search.FunPlant_Organization_UID != 0)
            {
                query = query.Where(p => p.FunPlant_Organization_UID == search.FunPlant_Organization_UID);
            }

            if (!string.IsNullOrWhiteSpace(search.CostCtr_ID))
            {
                query = query.Where(p => p.CostCtr_ID.Contains(search.CostCtr_ID));
            }

            if (!string.IsNullOrWhiteSpace(search.CostCtr_Description))
            {
                query = query.Where(p => p.CostCtr_Description.Contains(search.CostCtr_Description));
            }

            if (!string.IsNullOrWhiteSpace(search.Modified_By))
            {
                query = query.Where(p => p.Modified_UserNTID == search.Modified_By);
            }
            if (search.Modified_Date_From != null)
            {
                query = query.Where(p => p.Modified_Date >= search.Modified_Date_From);
            }
            if (search.Modified_Date_End != null)
            {
                var endDate = ((DateTime)search.Modified_Date_End).AddDays(1);
                query = query.Where(p => p.Modified_Date < endDate);
            }

            count = query.Count();
            return query
                        .OrderBy(o => o.Plant_Organization_UID)
                        .ThenBy(o => o.BG_Organization_UID)
                        .ThenBy(o => o.CostCtr_ID)
                        .GetPage(page);
        }

        /// <summary>
        /// Query CostCenter by uid
        /// </summary>
        /// <param name="uid">key</param>
        /// <returns></returns>
        public CostCtr_infoDTO QueryCostCenter(int uid)
        {
            var query = from c in DataContext.CostCtr_info
                        where c.CostCtr_UID == uid
                        select new CostCtr_infoDTO
                        {
                            Plant_Organization_UID = c.Plant_Organization_UID,
                            BG_Organization_UID = c.BG_Organization_UID,
                            FunPlant_Organization_UID = c.FunPlant_Organization_UID,
                            CostCtr_ID = c.CostCtr_ID,
                            CostCtr_Description = c.CostCtr_Description,
                            CostCtr_UID = c.CostCtr_UID,
                            Modified_UID = c.Modified_UID,
                            Modified_Date = c.Modified_Date
                        };

            return query.FirstOrDefault();
        }

       /// <summary>
       /// get all Cost Center Data
       /// </summary>
       /// <returns></returns>
        public List<CostCtrDTO> GetCostCtrs()
        {
            var query = from c in DataContext.CostCtr_info
                        select new CostCtrDTO
                        {
                            CostCtr_UID = c.CostCtr_UID,
                            CostCtr = c.CostCtr_ID + "_" + c.CostCtr_Description
                        };

            return query.ToList();
        }
        #endregion //End CostCenterMaintenace
    }

}
