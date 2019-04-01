using System;
using System.Collections.Generic;
using System.Linq;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;

namespace PDMS.Data.Repository
{
    public class SystemRoleRepository : RepositoryBase<System_Role>, ISystemRoleRepository
    {
        public SystemRoleRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        List<UserRoleJ> reselt = new List<UserRoleJ>();
        /// <summary>
        /// Query system users and order by account_id asc
        /// </summary>
        /// <param name="search">search model</param>
        /// <param name="page">page info</param>
        /// <param name="count">number of total records</param>
        /// <returns></returns>
        public IQueryable<System_Role> QueryRoles(RoleModelSearch search, Page page, out int count)
        {
            var query = from role in DataContext.System_Role.Include("System_Users")
                        select role;

            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                if (!string.IsNullOrWhiteSpace(search.Role_ID))
                {
                    query = query.Where(p => p.Role_ID == search.Role_ID);
                }
                if (!string.IsNullOrWhiteSpace(search.Role_Name))
                {
                    query = query.Where(p => p.Role_Name.Contains(search.Role_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Modified_By))
                {
                    query = query.Where(p => p.System_Users.User_Name == search.Modified_By);
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

                return query.OrderBy(o => o.Role_UID).GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Role_UID));

                count = 0;
                return query
                            .OrderBy(o => o.Role_UID)
                            .ThenBy(o => o.Role_ID);
            }

        }


        /// <summary>
        /// add by tonny 2015-11-24
        /// get role functions by role uid
        /// </summary>
        /// <param name="role_uid"></param>
        /// <returns></returns>
        public System_Role QueryRoleFunctionsByRoleUID(int role_uid)
        {
            var query = from role in DataContext.System_Role
                        join role_func in DataContext.System_Role_Function on role.Role_UID equals role_func.Role_UID
                        join func in DataContext.System_Function on role_func.Function_UID equals func.Function_UID
                        where role.Role_UID == role_uid
                        select role;

            return query.FirstOrDefault();
        }

        public List<UserRoleJ> GetUserRoleNow(int currentUser)
        {
            var IDS = new List<UserRoleJ>();
            var role = from temp in DataContext.System_Role
                       select new UserRoleJ()
                       {
                           RoleUID = temp.Role_UID,
                           UserRoleID = temp.Role_ID,
                           UserRoleName = temp.Role_Name
                       };
            var userRoleTemp = from temp in DataContext.System_User_Role
                               where (temp.Account_UID == currentUser)
                               select temp.System_Role.Role_ID;
            //var userRoleNow = userRoleTemp.FirstOrDefault().ToString();
            var userRoleNow = userRoleTemp.ToList();
            //再在集合IDS中查找所有父ID为集合元素的角色  result
            if (userRoleNow.Contains("SystemAdmin"))
            {
                IDS = role.Distinct().ToList();
            }
            else
            {
                foreach (var roleName in userRoleNow)
                {
                    IDS.AddRange(GetChildRole(roleName));
                }
                //IDS = GetChildRole(userRoleNow);
            }



            //if (userRoleNow == "PPAdmin")
            //{
            //    var ppRole =
            //    DataContext.Enumeration.Where(m => m.Enum_Type == "PP_CREATE_USER_ROLE")
            //        .Select(m =>new UserRole() {UserRoleID = m.Enum_Name,UserRoleName = m.Enum_Value})
            //        .Distinct();
            //    result.AddRange(ppRole);
            //}
            //else
            //{
            //    result = role.Distinct().ToList();
            //}
            return IDS.Distinct().ToList();
        }

        //通过当前用户的角色 查找所有角色的父ID为当前角色ID 集合IDS string[]
        private List<UserRoleJ> GetChildRole(string role_id)
        {
            List<UserRoleJ> IDS = new List<UserRoleJ>();
            var Roles = from temp in DataContext.System_Role
                        where temp.Father_Role_ID == role_id
                        select new UserRoleJ()
                        {
                            RoleUID = temp.Role_UID,
                            UserRoleID = temp.Role_ID,
                            UserRoleName = temp.Role_Name
                        };

            IDS = Roles.ToList();
            if (IDS.Count > 0)
            {
                reselt.AddRange(IDS);
                foreach (var item in IDS)
                {
                    GetChildRole(item.UserRoleID);
                }
            }

            return reselt;
        }

        public string GetRoleName(string RoleId)
        {
            var roleName =
                DataContext.System_Role.Where(m => m.Role_ID == RoleId).Select(m => m.Role_Name).FirstOrDefault();
            return roleName;
        }

        public void AddRole(System_Role dto)
        {
            var strSql = @"INSERT INTO dbo.System_Role
                        ( Role_UID ,
                          Role_ID ,
                          Role_Name ,
                          Modified_UID ,
                          Modified_Date
                        )
                VALUES  ( {0} , -- Role_UID - int
                          N'{1}' , -- Role_ID - nvarchar(20)
                          N'{2}' , -- Role_Name - nvarchar(50)
                          {3} , -- Modified_UID - int
                          GETDATE()  -- Modified_Date - datetime
                        )";
            strSql = string.Format(strSql, dto.Role_UID, dto.Role_ID, dto.Role_Name, dto.Modified_UID);
            var dbList = DataContext.Database.ExecuteSqlCommand(strSql);
        }

        public bool IsExistRoleID(string roleID)
        {
            var query = from role in DataContext.System_Role
                        select role;
            query = query.Where(p => p.Role_ID == roleID);
            if (query.Count() > 0)
            {
                return true;
            }
            return false;
        }
    }

    public interface ISystemRoleRepository : IRepository<System_Role>
    {
        IQueryable<System_Role> QueryRoles(RoleModelSearch search, Page page, out int count);
        System_Role QueryRoleFunctionsByRoleUID(int role_uid);
        List<UserRoleJ> GetUserRoleNow(int currentUser);
        void AddRole(System_Role dto);

        bool IsExistRoleID(string roleID);
    }
}
