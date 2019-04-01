using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;

namespace PDMS.Data.Repository
{

    public class WarningListRepository : RepositoryBase<Warning_List>, IWarningListRepository
    {
        public WarningListRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        /// <summary>
        /// Get warning list
        /// </summary>
        /// <param name="user_account_uid">user_account_uid</param>
        /// <param name="count">number of total records</param>
        /// <returns></returns>
        public IEnumerable<WarningListDTO> GetWarninglistDatas(int user_account_uid, out int count)
        {
            string[] showRoleList = { "SystemAdmin", "BasicAdmin", "PPAdmin" };
            //获取当前用户所在的功能厂
            //1.获取物料员所在功能厂
            var temp = from mh in DataContext.FlowChart_PC_MH_Relationship
                       join FlowChart in DataContext.FlowChart_Detail on mh.FlowChart_Detail_UID equals
                           FlowChart.FlowChart_Detail_UID
                       where (mh.MH_UID == user_account_uid)
                       select FlowChart.System_Function_Plant.FunPlant;
            var MH_Funcplant = temp.FirstOrDefault();
            //2.获取其他人员所在功能厂
            var temp1 = from func in DataContext.System_Organization
                        join UserOrg in DataContext.System_UserOrg on func.Organization_UID equals UserOrg.Organization_UID
                        where UserOrg.Account_UID == user_account_uid && func.Organization_ID.StartsWith("4")
                        select func.Organization_Name;
            var other_Funcplant = temp1.FirstOrDefault();

            //获取当前用户的角色
            var userRole = from user in DataContext.System_User_Role
                           where (user.Account_UID == user_account_uid)
                           select user.System_Role.Role_ID;
            var firstOrDefault = userRole.FirstOrDefault();
            var funcPlant = firstOrDefault != null && firstOrDefault.ToString() == "Maintain Function" ? MH_Funcplant : other_Funcplant;
            //查询出所有的WArning的功能厂信息
            var query = (from warninglist in DataContext.Warning_List
                         select new WarningListDTO
                         {
                             Warning_UID = warninglist.Warning_UID,
                             Part_Types = warninglist.Part_Types,
                             Product_Date = warninglist.Product_Date,
                             Product_Phase = warninglist.Product_Phase,
                             Project = warninglist.Project,
                             Customer = warninglist.Customer,
                             FncPlant_Effect = warninglist.System_Function_Plant.FunPlant,
                             Time_Interval = warninglist.Time_Interval
                         }).Distinct();
            //如果当前用户的账号为免检的账号，则不筛选
            var result = userRole.ToList().Any(item => showRoleList.Contains(item));
            if (!result)
            {
                query = query.Where(m => funcPlant.Contains(m.FncPlant_Effect));
            }
            count = query.Count();
            return query.OrderBy(o => o.Product_Date);

        }

        public IEnumerable<ProcessDataSearch> GetWarningDataByWarningUid(int WarningUId)
        {
            var query = from warninglist in DataContext.Warning_List
                        join FunPlant in DataContext.System_Function_Plant on warninglist.FncPlant_Now equals FunPlant.System_FunPlant_UID
                        where warninglist.Warning_UID == WarningUId
                        select new ProcessDataSearch
                        {
                            Part_Types = warninglist.Part_Types,
                            Product_Phase = warninglist.Product_Phase,
                            Project = warninglist.Project,
                            Customer = warninglist.Customer,
                            QuertFlag = "WarningList",
                            Date = warninglist.Product_Date,
                            Func_Plant = FunPlant.FunPlant,
                            Time = warninglist.Time_Interval
                        };
            return query;
        }

        public int GetMasterUidByWarningUid(int WarningUId)
        {
            var sql = @"select fcm.FlowChart_Master_UID from
dbo.Warning_List AS wl,dbo.FlowChart_Master AS fcm,dbo.System_Project AS sp,dbo.System_BU_D AS sbd
 WHERE Warning_UID={0}
 AND wl.Project=sp.Project_Name
 AND wl.Customer=sbd.BU_D_Name
 AND wl.Part_Types=fcm.Part_Types
 AND wl.Product_Phase=sp.Product_Phase";
            sql = string.Format(sql, WarningUId);
            var dbList = DataContext.Database.SqlQuery<int>(sql).ToList();
            return dbList.FirstOrDefault();
        }
    }

    public interface IWarningListRepository : IRepository<Warning_List>
    {
        /// <summary>
        /// Get warning list
        /// </summary>
        /// <param name="user_account_uid">user_account_uid</param>
        /// <param name="count">number of total records</param>
        /// <returns>paged records</returns>
        IEnumerable<WarningListDTO> GetWarninglistDatas(int user_account_uid, out int count);

        IEnumerable<ProcessDataSearch> GetWarningDataByWarningUid(int WarningUId);
        int GetMasterUidByWarningUid(int WarningUId);
    }
}
