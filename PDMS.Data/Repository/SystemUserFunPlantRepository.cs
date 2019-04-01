using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System.Linq;
using System.Data.Entity.SqlServer;
using System;
using PDMS.Common.Constants;
using System.Collections.Generic;


namespace PDMS.Data.Repository
{

    public interface ISystemUserFunPlantRepository : IRepository<System_User_FunPlant>
    {
        int checkFuncPlantIsExit(int uuid);
        IQueryable<UserFuncPlantVM> QueryUserFuncPlants(UserFuncPlantSearch search, Page page, out int count);
        IQueryable<System_User_FunPlant> CheckUserFuncPlantIsExit(string FuncPlant, string User_NTID);
    }

    public class SystemUserFunPlantRepository : RepositoryBase<System_User_FunPlant>, ISystemUserFunPlantRepository
    {
        public SystemUserFunPlantRepository(IDatabaseFactory databaseFactory)
        : base(databaseFactory)
        {

        }

        public int checkFuncPlantIsExit(int uuid)
        {
            var result = DataContext.System_User_FunPlant.Count(p => p.System_FunPlant_UID == uuid);
            return result;
        }

        public IQueryable<UserFuncPlantVM> QueryUserFuncPlants(UserFuncPlantSearch search, Page page, out int count)
        {
            var query = from item in DataContext.System_User_FunPlant.Include("System_Users")
                        join modify in DataContext.System_Users on item.Modified_UID equals modify.Account_UID
                        join func in DataContext.System_Function_Plant on item.System_FunPlant_UID equals
                            func.System_FunPlant_UID
                        select new UserFuncPlantVM
                        {
                            System_User_FunPlant_UID = item.System_User_FunPlant_UID,
                            User_NTID = item.System_Users.User_NTID,
                            User_Name = item.System_Users.User_Name,
                            Plant = func.System_Plant.Plant,
                            OP_Types = func.OP_Types,
                            FuncPlant = func.FunPlant,
                            Modified_UserName = modify.User_Name,
                            Modified_By_NTID = modify.User_NTID,
                            Modified_Date = item.Modified_Date
                        };
            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                if (!string.IsNullOrWhiteSpace(search.FuncPlant))
                    query = query.Where(p => p.FuncPlant == search.FuncPlant);
                if (!string.IsNullOrWhiteSpace(search.User_NTID))
                    query = query.Where(p => p.User_NTID == search.User_NTID);
                if (!string.IsNullOrWhiteSpace(search.User_Name))
                    query = query.Where(p => p.User_Name == search.User_Name);
                if (!string.IsNullOrWhiteSpace(search.Modified_By_NTID))
                    query = query.Where(p => p.Modified_By_NTID == search.Modified_By_NTID);
                if (search.Modified_Date_From != null)
                {
                    query = query.Where(m => m.Modified_Date >= search.Modified_Date_From);
                }
                if (search.Modified_Date_End != null)
                {
                    var endDate = ((DateTime)search.Modified_Date_End).AddDays(1);
                    query = query.Where(m => m.Modified_Date < endDate);
                }
                count = query.Count();
                return query.OrderBy(o => o.System_User_FunPlant_UID).GetPage(page);
            }
            else
            {

                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.System_User_FunPlant_UID));

                count = 0;
                return query.OrderBy(o => o.System_User_FunPlant_UID);
            }

        }

        public IQueryable<System_User_FunPlant> CheckUserFuncPlantIsExit(string FuncPlant, string User_NTID)
        {
            var temp = from item in DataContext.System_User_FunPlant.Include("System_Uses")
                       select item;
            if (User_NTID != "")
                temp = temp.Where(p => p.System_Users.User_NTID == User_NTID);
            temp = temp.Where(p => p.System_Function_Plant.FunPlant == FuncPlant);
            return temp;
        }
    }
}
