using PDMS.Common.Enums;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using System;
using System.Data;
using System.Linq;

namespace PDMS.Data.Repository
{
    public class SystemUserPlantRepository : RepositoryBase<System_User_Plant>, ISystemUserPlantRepository
    {
        public SystemUserPlantRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public IQueryable<UserPlantItem> QueryUserPlants(UserPlantModelSearch search, Page page, out int count)
        {
            var query = from userplant in DataContext.System_User_Plant
                        join users in DataContext.System_Users on userplant.Account_UID equals users.Account_UID
                        join modifyusers in DataContext.System_Users on userplant.Modified_UID equals modifyusers.Account_UID
                        join plant in DataContext.System_Plant on userplant.System_Plant_UID equals plant.System_Plant_UID
                        select new UserPlantItem
                        {     
                            Account_UID = userplant.Account_UID,                       
                            User_Plant_UID = userplant.System_User_Plant_UID,
                            User_NTID = users.User_NTID,
                            User_Name = users.User_Name,
                            Plant = plant.Plant,
                            Location = plant.Location,
                            Type = plant.Type,
                            Plant_Code = plant.Name_0,
                            Begin_Date = userplant.Begin_Date,
                            End_Date = userplant.End_Date,
                            Modified_UID = userplant.Modified_UID,
                            Modified_Date = userplant.Modified_Date,
                            Modified_UserName = modifyusers.User_Name,
                            Modified_UserNTID = modifyusers.User_NTID
                        };

            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                #region Query_Types

                if (search.query_types != null && search.Reference_Date != null)
                {
                    EnumValidity queryType = (EnumValidity)Enum.ToObject(typeof(EnumValidity), search.query_types);

                    switch (queryType)
                    {
                        case EnumValidity.Valid:
                            query = query.Where(p => p.Begin_Date <= search.Reference_Date && (p.End_Date >= search.Reference_Date || p.End_Date == null));
                            break;
                        case EnumValidity.Invalid:
                            query = query.Where(p => p.Begin_Date > search.Reference_Date || (p.End_Date < search.Reference_Date && p.End_Date != null));
                            break;
                        default:
                            break;
                    }

                }
                #endregion
                #region Modified_Date
                if (search.Modified_Date_From != null)
                {
                    query = query.Where(m => m.Modified_Date >= search.Modified_Date_From);
                }
                if (search.Modified_Date_End != null)
                {
                    var endDate = ((DateTime)search.Modified_Date_End).AddDays(1);
                    query = query.Where(m => m.Modified_Date < endDate);
                }   
                #endregion

                if (!string.IsNullOrWhiteSpace(search.User_NTID))
                {
                    query = query.Where(p => p.User_NTID == search.User_NTID);

                }
                if (!string.IsNullOrWhiteSpace(search.User_Name))
                {
                    query = query.Where(p => p.User_Name.Contains(search.User_Name));

                }
                if (!string.IsNullOrWhiteSpace(search.Plant))
                {
                    query = query.Where(p => p.Plant == search.Plant);

                }
                if (!string.IsNullOrWhiteSpace(search.Location))
                {
                    query = query.Where(p => p.Location.Contains(search.Location));
                }
                if (!string.IsNullOrWhiteSpace(search.Type))
                {
                    query = query.Where(p => p.Type.Contains(search.Type));
                }
                if (!string.IsNullOrWhiteSpace(search.Plant_Code))
                {
                    query = query.Where(p => p.Plant_Code.Contains(search.Plant_Code));
                }

                if (!string.IsNullOrWhiteSpace(search.Modified_By_NTID))
                {
                    query = query.Where(p => p.Modified_UserNTID == search.Modified_By_NTID);
                }

                count = query.Count();
                return query.OrderBy(o => o.User_Plant_UID).GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.User_Plant_UID));

                count = 0;
                return query.OrderByDescending(o => o.Modified_Date);
            }
        }

        public IQueryable<UserPlantWithPlant> QueryUserPlantByAccountUID(int uuid)
        {
            var query = from userplant in DataContext.System_User_Plant
                        join plant in DataContext.System_Plant on userplant.System_Plant_UID equals plant.System_Plant_UID
                        where userplant.Account_UID == uuid
                        select new UserPlantWithPlant
                        {
                            System_Plant_UID = uuid,
                            Plant = plant.Plant,
                            Location = plant.Location,
                            Type = plant.Type,
                            Plant_Code = plant.Name_0,
                            Plant_Begin_Date = plant.Begin_Date,
                            Plant_End_Date = plant.End_Date,
                            System_User_Plant_UID = userplant.System_User_Plant_UID,
                            User_Plant_Begin_Date = userplant.Begin_Date,
                            User_Plant_End_Date = userplant.End_Date
                        };
            return query;
        }

        public DateTime? GetMaxEnddate4Plant(int uid)
        {
            var query = from userPlant in DataContext.System_User_Plant
                        where userPlant.System_Plant_UID ==uid
                        orderby userPlant.End_Date descending
                        select userPlant.End_Date;

            return query.FirstOrDefault();
        }
    }
    public interface ISystemUserPlantRepository : IRepository<System_User_Plant>
    {

        IQueryable<UserPlantItem> QueryUserPlants(UserPlantModelSearch search, Page page, out int count);
        IQueryable<UserPlantWithPlant> QueryUserPlantByAccountUID(int uuid);
        DateTime? GetMaxEnddate4Plant(int uid);
    }
}
