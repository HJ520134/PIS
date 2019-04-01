using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;

namespace PDMS.Data.Repository
{
    public interface IFixture_Totake_DRepository : IRepository<Fixture_Totake_D>
    {
        string DeleteByMuid(int Fixture_Totake_M_UID);
        IQueryable<Fixture_Totake_DDTO> GetInfo(Fixture_Totake_DDTO searchModel, Page page, out int totalcount);
    }
    public class Fixture_Totake_DRepository : RepositoryBase<Fixture_Totake_D>, IFixture_Totake_DRepository
    {
        public Fixture_Totake_DRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public string DeleteByMuid(int Fixture_Totake_M_UID)
        {
            try
            {
                string sql = @"DELETE dbo.Fixture_Totake_D WHERE Fixture_Totake_M_UID={0}";
                sql = string.Format(sql, Fixture_Totake_M_UID);
                DataContext.Database.ExecuteSqlCommand(sql);
                return "";
            }
            catch (Exception e)
            {
                return "更新治具失败" + e.Message;
            }

        }
        public IQueryable<Fixture_Totake_DDTO> GetInfo(Fixture_Totake_DDTO searchModel, Page page, out int totalcount)
        {
            var query = from D in DataContext.Fixture_Totake_D
                         join M in DataContext.Fixture_M
                         on D.Fixture_M_UID equals M.Fixture_M_UID
                         join Fixture in DataContext.Fixture_Totake_M
                         on D.Fixture_Totake_M_UID equals Fixture.Fixture_Totake_M_UID
                         join project in DataContext.System_Project
                         on M.Project_UID equals project.Project_UID into temp
                         from aa in temp.DefaultIfEmpty()
                         join vendor in DataContext.Vendor_Info
                         on M.Vendor_Info_UID equals vendor.Vendor_Info_UID into temp2
                         from bb in temp2.DefaultIfEmpty()
                         join productline in DataContext.Production_Line
                         on M.Production_Line_UID equals productline.Production_Line_UID into temp3
                         from cc in temp3.DefaultIfEmpty()
                         join process in DataContext.Process_Info
                         on cc.Process_Info_UID equals process.Process_Info_UID into temp4
                         from dd in temp4.DefaultIfEmpty()
                         join workstation in DataContext.WorkStation
                         on cc.Workstation_UID equals workstation.WorkStation_UID into temp5
                         from ee in temp5.DefaultIfEmpty()
                         join fixturemachine in DataContext.Fixture_Machine
                         on M.Fixture_Machine_UID equals fixturemachine.Fixture_Machine_UID into temp6
                         from ff in temp6.DefaultIfEmpty()
                         join plantorg in DataContext.System_Organization
                         on Fixture.Plant_Organization_UID equals plantorg.Organization_UID
                         join bgorg in DataContext.System_Organization
                         on Fixture.BG_Organization_UID equals bgorg.Organization_UID
                         join funplantorg in DataContext.System_Organization
                         on Fixture.FunPlant_Organization_UID equals funplantorg.Organization_UID into temp7
                         from gg in temp7.DefaultIfEmpty()
                         join user1 in DataContext.System_Users 
                         on D.Modified_UID equals user1.Account_UID
                         join user2 in DataContext.System_Users
                         on D.Created_UID equals user2.Account_UID
                         select new Fixture_Totake_DDTO
                         {
                             Fixture_Totake_M_UID = D.Fixture_Totake_M_UID,
                             Plant_Organization_UID=M.Plant_Organization_UID,
                             BG_Organization_UID=M.BG_Organization_UID,
                             FunPlant_Organization_UID=M.FunPlant_Organization_UID,
                             Plant=plantorg.Organization_Name,
                             BG=bgorg.Organization_Name,
                             FunPlant=gg.Organization_Name,
                             Process_ID = dd.Process_ID,
                             WorkStation_ID = ee.WorkStation_ID,
                             Line_ID =cc.Line_ID,
                             Machine_ID=ff.Machine_ID,
                             Fixture_Unique_ID=M.Fixture_Unique_ID,
                             ShortCode = M.ShortCode,
                             Modifier=user1.User_Name,
                             Modified_Date=D.Modified_Date,
                             Fixture_Name=M.Fixture_Name,
                             Creator=user2.User_Name,
                             Created_Date=D.Created_Date
                         };
            query = query.Where(m => m.Fixture_Totake_M_UID == searchModel.Fixture_Totake_M_UID);
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Process_ID))
                query = query.Where(m => m.Process_ID.Contains(searchModel.Process_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.WorkStation_ID))
                query = query.Where(m => m.WorkStation_ID.Contains(searchModel.WorkStation_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Machine_ID))
                query = query.Where(m => m.Machine_ID.Contains(searchModel.Machine_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Unique_ID))
                query = query.Where(m => m.Fixture_Unique_ID.Contains(searchModel.Fixture_Unique_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Vendor_ID))
                query = query.Where(m => m.Vendor_ID.Contains(searchModel.Vendor_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Line_ID))
                query = query.Where(m => m.Line_ID.Contains(searchModel.Line_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                query = query.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                query = query.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Modifier))
                query = query.Where(m => m.Modifier.Contains(searchModel.Modifier));
            if (searchModel.Modified_Date_from.Year != 1)
                query = query.Where(m => m.Modified_Date >= searchModel.Modified_Date_from);
            if (searchModel.Modified_Date_to.Year != 1)
            {
                DateTime nextday = searchModel.Modified_Date_to.AddDays(1);
                query = query.Where(m => m.Modified_Date < nextday);
            }
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;
        }
    }
}
