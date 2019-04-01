using PDMS.Data.Infrastructure;
using PDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface ICurrentStaffRepository : IRepository<CurrentStaffDTO>
    {
        List<CurrentStaffDTO> QueryCurrentStaffInfo(CurrentStaffDTO dto, Page page, out int totalCount);
    }

    public class CurrentStaffRepository : RepositoryBase<CurrentStaffDTO>, ICurrentStaffRepository
    {
        public CurrentStaffRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<CurrentStaffDTO> QueryCurrentStaffInfo(CurrentStaffDTO dto, Page page, out int totalCount)
        {
            var strSql = @"SELECT A.Current_Staff_UID,A.Plant_Organization_UID,A.ProductDate,B.Organization_Name AS PlantName,
                        A.BG_Organization_UID,C.Organization_Name AS Optype, 
                        A.Product_Phase,A.OP_Qty,A.Monitor_Staff_Qty,A.Technical_Staff_Qty,A.Material_Keeper_Qty,A.Others_Qty,
                        A.Created_Date
                        FROM dbo.Current_Staff A
                        LEFT JOIN dbo.System_Organization B
                        ON A.Plant_Organization_UID = B.Organization_UID
                        LEFT JOIN dbo.System_Organization C
                        ON A.BG_Organization_UID = C.Organization_UID
                        WHERE 1= 1 ";

            if (dto.Plant_Organization_UID != 0)
            {
                strSql = strSql + string.Format("AND A.Plant_Organization_UID = {0} ", dto.Plant_Organization_UID);
            }

            if (dto.BG_Organization_UID != 0)
            {
                strSql = strSql + string.Format("AND A.BG_Organization_UID = {0} ", dto.BG_Organization_UID);
            }

            if (dto.Product_Phase != "All")
            {
                strSql = strSql + string.Format("AND A.Product_Phase='{0}'", dto.Product_Phase);
            }


            totalCount = DataContext.Database.SqlQuery<CurrentStaffDTO>(strSql).ToList().Count();
            var list = DataContext.Database.SqlQuery<CurrentStaffDTO>(strSql).Skip(page.PageSize * page.PageNumber).Take(page.PageSize).ToList();
            return list;
        }
    }
}
