using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.SqlServer;

namespace PDMS.Data.Repository
{
    public class SystemUserRepository : RepositoryBase<System_Users>, ISystemUserRepository
    {
        public SystemUserRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        /// <summary>
        /// Query system users and order by account_id asc
        /// </summary>
        /// <param name="search">search model</param>
        /// <param name="page">page info</param>
        /// <param name="count">number of total records</param>
        /// <returns></returns>
        public IQueryable<SystemUserDTO> QueryUsersJ(UserModelSearch search, Page page, out int count ,List<UserRoleJ> roles)
        {
            List<int> roleIDs = new List<int>();
            bool flag = false;
            foreach(var item in roles)
            {
                if (item.UserRoleID == "SystemAdmin")
                    flag = true;
                roleIDs.Add(item.RoleUID);
            }

            //如果是管理员登录，可以看到所有数据
     
            if (flag)
            {
                var query = from user in DataContext.System_Users
                             join modifiedUser in DataContext.System_Users on user.Modified_UID equals modifiedUser.Account_UID
                             select new SystemUserDTO
                             {
                                 Account_UID = user.Account_UID,
                                 User_NTID = user.User_NTID,
                                 User_Name = user.User_Name,
                                 Email = user.Email,
                                 Enable_Flag = user.Enable_Flag,
                                 Tel = user.Tel,
                                 Modified_UserName = modifiedUser.User_Name,
                                 Modified_UserNTID = modifiedUser.User_NTID,
                                 Modified_Date = user.Modified_Date,
                                 Modified_UID = user.Modified_UID,
                                 EmployeeNumber=user.EmployeeNumber,
                                 EmployeePassword=user.EmployeePassword,
                                 EnableEmpIdLogin= (user.EnableEmpIdLogin ==null? 0 : user.EnableEmpIdLogin.Value),
                                 Building =user.Building
                };
                if (string.IsNullOrEmpty(search.ExportUIds))
                {
                    if (search.Account_UID > 0)
                    {
                        query = query.Where(p => p.Account_UID == search.Account_UID);
                    }
                    if (!string.IsNullOrWhiteSpace(search.User_NTID))
                    {
                        query = query.Where(p => p.User_NTID == search.User_NTID);
                    }
                    if (!string.IsNullOrWhiteSpace(search.User_Name))
                    {
                        query = query.Where(p => p.User_Name.Contains(search.User_Name));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Tel))
                    {
                        query = query.Where(p => p.Tel.Contains(search.Tel));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Email))
                    {
                        query = query.Where(p => p.Email.Contains(search.Email));
                    }
                    if (search.Enable_Flag != null)
                    {
                        query = query.Where(p => p.Enable_Flag == search.Enable_Flag);
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

                    return query.OrderByDescending(o => o.Modified_Date).GetPage(page);
                }
                else
                {
                    //for export data
                    var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                    query = query.Where(p => array.Contains(p.Account_UID));
                    query = query.Distinct();
                    count = 0;
                    return query.OrderByDescending(o => o.Modified_Date);
                }
            }
            else
            {
                
                var query = from user in DataContext.System_Users
                            join modifiedUser in DataContext.System_Users on user.Modified_UID equals modifiedUser.Account_UID
                            join UR in DataContext.System_User_Role on user.Account_UID equals UR.Account_UID
                            join org in DataContext.System_UserOrg on user.Account_UID equals org.Account_UID
                            where user.MH_Flag == false && roleIDs.Contains(UR.Role_UID)
                            select  new SystemUserDTO 
                            {
                                Account_UID = user.Account_UID,
                                User_NTID = user.User_NTID,
                                User_Name = user.User_Name,
                                Email = user.Email,
                                Enable_Flag = user.Enable_Flag,
                                Tel = user.Tel,
                                Modified_UserName = modifiedUser.User_Name,
                                Modified_UserNTID = modifiedUser.User_NTID,
                                Modified_Date = user.Modified_Date,
                                Modified_UID = user.Modified_UID,
                                EmployeeNumber = user.EmployeeNumber,
                                EmployeePassword = user.EmployeePassword,
                                EnableEmpIdLogin = user.EnableEmpIdLogin.Value,
                                Building = user.Building
                            };
                query = query.Distinct();
                if (search.Orgnizations != null && search.Orgnizations.Count > 0)
                {
                    List<SystemUserDTO> list = new List<SystemUserDTO>();
                    foreach (OrganiztionVM vm in search.Orgnizations)
                    {
                        if (vm.OPType == "Support team")
                        {
                            var query1 = from q in query
                                         join plant in DataContext.System_UserOrg on q.Account_UID equals plant.Account_UID
                                         where plant.Plant_OrganizationUID == vm.Plant_OrganizationUID
                                         select q;
                            list.AddRange(query1.ToList());
                        }
                        //没有设置BG的就可以看到所有的用户
                        if (vm.OPType == null)
                        {
                            var query1 = from q in query
                                         join plant in DataContext.System_UserOrg on q.Account_UID equals plant.Account_UID
                                         where plant.Plant_OrganizationUID == vm.Plant_OrganizationUID
                                         select q;
                            list.AddRange(query1.ToList());
                        }
                        else
                        {
                            var query1 = from q in query
                                         join plant in DataContext.System_UserOrg on q.Account_UID equals plant.Account_UID
                                         where plant.OPType_OrganizationUID == vm.OPType_OrganizationUID
                                         select q;
                            list.AddRange(query1.ToList());
                        }
                    }
                    query = list.AsQueryable();
                }
                
                if (string.IsNullOrEmpty(search.ExportUIds))
                {
                    if (search.Account_UID > 0)
                    {
                        query = query.Where(p => p.Account_UID == search.Account_UID);
                    }
                    if (!string.IsNullOrWhiteSpace(search.User_NTID))
                    {
                        query = query.Where(p => p.User_NTID == search.User_NTID);
                    }
                    if (!string.IsNullOrWhiteSpace(search.User_Name))
                    {
                        query = query.Where(p => p.User_Name.Contains(search.User_Name));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Tel))
                    {
                        query = query.Where(p => p.Tel.Contains(search.Tel));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Email))
                    {
                        query = query.Where(p => p.Email.Contains(search.Email));
                    }
                    if (search.Enable_Flag != null)
                    {
                        query = query.Where(p => p.Enable_Flag == search.Enable_Flag);
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

                    return query.OrderByDescending(m=>m.Modified_Date).GetPage(page);
                }
                else
                {
                    //for export data
                    var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                    query = query.Where(p => array.Contains(p.Account_UID));

                    count = 0;
                    return query.OrderByDescending(o => o.Modified_Date);
                }
            }
          
          
        }

        public int GetUserUIDByEmail(string Email)
        {
            var query = from user in DataContext.System_Users
                        where user.Email == Email
                        select user.Account_UID;
            return query.FirstOrDefault();
        }
        public IQueryable<SystemUserDTO> QueryUsers(UserModelSearch search, Page page, out int count)
        {
            
            var query = from user in DataContext.System_Users
                        join modifiedUser in DataContext.System_Users on user.Modified_UID equals modifiedUser.Account_UID
                        where user.MH_Flag == false 
                        select new SystemUserDTO
                        {
                            Account_UID = user.Account_UID,
                            User_NTID = user.User_NTID,
                            User_Name = user.User_Name,
                            Email = user.Email,
                            Enable_Flag = user.Enable_Flag,
                            Tel = user.Tel,
                            Modified_UserName = modifiedUser.User_Name,
                            Modified_UserNTID = modifiedUser.User_NTID,
                            Modified_Date = user.Modified_Date,
                            Modified_UID = user.Modified_UID,
                            EmployeeNumber = user.EmployeeNumber,
                            EmployeePassword = user.EmployeePassword,
                            EnableEmpIdLogin = user.EnableEmpIdLogin.Value,
                            Building = user.Building
                        };

            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                if (search.Account_UID > 0)
                {
                    query = query.Where(p => p.Account_UID == search.Account_UID);
                }
                if (!string.IsNullOrWhiteSpace(search.User_NTID))
                {
                    query = query.Where(p => p.User_NTID == search.User_NTID);
                }
                if (!string.IsNullOrWhiteSpace(search.User_Name))
                {
                    query = query.Where(p => p.User_Name.Contains(search.User_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Tel))
                {
                    query = query.Where(p => p.Tel.Contains(search.Tel));
                }
                if (!string.IsNullOrWhiteSpace(search.Email))
                {
                    query = query.Where(p => p.Email.Contains(search.Email));
                }
                if (search.Enable_Flag != null)
                {
                    query = query.Where(p => p.Enable_Flag == search.Enable_Flag);
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

                return query.OrderBy(o => o.Account_UID).GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Account_UID));

                count = 0;
                return query.OrderBy(o => o.Account_UID);
            }
        }

        public int QueryUserUIDByNTID(string User_NTID)
        {
            var query = from user in DataContext.System_Users
                        where user.User_NTID == User_NTID
                        orderby user.Modified_Date descending
                        select user.Account_UID ;
            return query.FirstOrDefault();
        }
        public IQueryable<SystemUserDTO> GetUserInfoByNTID(string User_NTID)
        {
            var ntid = from user in DataContext.System_Users
                       join modifiedUser in DataContext.System_Users on user.Modified_UID equals modifiedUser.Account_UID
                       where (user.User_NTID==User_NTID)
                       select new SystemUserDTO
                       {
                           Account_UID = user.Account_UID,
                           User_NTID = user.User_NTID,
                           User_Name = user.User_Name,
                           Email = user.Email,
                           Enable_Flag = user.Enable_Flag,
                           Tel = user.Tel,
                           Modified_UserName = modifiedUser.User_Name,
                           Modified_UserNTID = modifiedUser.User_NTID,
                           Modified_Date = user.Modified_Date,
                           Modified_UID = user.Modified_UID,
                           EmployeeNumber = user.EmployeeNumber,
                           EmployeePassword = user.EmployeePassword,
                           EnableEmpIdLogin = user.EnableEmpIdLogin.Value,
                           Building = user.Building
                       }; 
            return ntid;
        }

        public void DeleteMHUsers(List<int> userList)
        {
            using (var result = DataContext.Database.BeginTransaction())
            {
                try
                {
                    //删除UserRole
                    var userRole = DataContext.System_User_Role.Where(m => userList.Contains(m.Account_UID));
                    DataContext.System_User_Role.RemoveRange(userRole);
                    //删除UserOrg
                    var userOrg = DataContext.System_UserOrg.Where(m => userList.Contains(m.Account_UID));
                    DataContext.System_UserOrg.RemoveRange(userOrg);
                    //删除User_Project
                    var userPro= DataContext.Project_Users_Group.Where(m => userList.Contains(m.Account_UID));
                    DataContext.Project_Users_Group.RemoveRange(userPro);
                    //删除User
                    var user = DataContext.System_Users.Where(m => userList.Contains(m.Account_UID));
                    DataContext.System_Users.RemoveRange(user);
                    DataContext.SaveChanges();
                    result.Commit();
                }
                catch (Exception e)
                {
                    result.Rollback();
                }
            }
        }

         public   bool CheckPISMUser(string EmailAddr)
        {
            bool result = false;
            var query = from U in DataContext.System_Users
                        where U.Email == EmailAddr
                        select U.Account_UID;
            if(query.Count()>0)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 用户使用工号登录判断(robert)
        /// </summary>
        /// <param name="userEmployee"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool EmployeeLogin(string userEmployee, string password)
        {
            var f = (from user in DataContext.System_Users
                     where user.EmployeeNumber == userEmployee && user.EmployeePassword == password
                     select user).FirstOrDefault();
            if (f != null && f.Enable_Flag == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public interface ISystemUserRepository : IRepository<System_Users>
    {
        /// <summary>
        /// Query system users and order by account_id asc
        /// </summary>
        /// <param name="search">search model</param>
        /// <param name="page">page info</param>
        /// <param name="count">number of total records</param>
        /// <returns>paged records</returns>
        IQueryable<SystemUserDTO> QueryUsers(UserModelSearch search, Page page, out int count);
        IQueryable<SystemUserDTO> QueryUsersJ(UserModelSearch search, Page page, out int count, List<UserRoleJ> roles);
        IQueryable<SystemUserDTO> GetUserInfoByNTID(string User_NTID);
        int QueryUserUIDByNTID(string User_NTID);
        void DeleteMHUsers(List<int> userList);
        bool EmployeeLogin(string userEmployee, string password);
        bool CheckPISMUser(string EmailAddr);
        int GetUserUIDByEmail(string Email);
    }
}
