using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Part_Demand_MRepository : IRepository<Fixture_Part_Demand_M>
    {
        IQueryable<Fixture_Part_Demand_M> QueryFixturePartDemandMs(Fixture_Part_Demand_MModelSearch search, Page page, out int count);
        bool CalculateFixturePartDemand(int Plant_Organization_UID, int BG_Organization_UID, int? FunPlant_Organization_UID, DateTime Demand_Date, DateTime Calculation_Date, int Applicant_UID);
    }
    public class Fixture_Part_Demand_MRepository : RepositoryBase<Fixture_Part_Demand_M>, IFixture_Part_Demand_MRepository
    {
        public Fixture_Part_Demand_MRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
        public IQueryable<Fixture_Part_Demand_M> QueryFixturePartDemandMs(Fixture_Part_Demand_MModelSearch search, Page page, out int count)
        {
            var query = from w in DataContext.Fixture_Part_Demand_M select w;
            if (string.IsNullOrWhiteSpace(search.ExportUIds))
            {
                if (search != null)
                {
                    if (search.Plant_Organization_UID > 0)
                    {
                        query = query.Where(w => w.Plant_Organization_UID == search.Plant_Organization_UID);
                    }
                    if (search.BG_Organization_UID > 0)
                    {
                        query = query.Where(w => w.BG_Organization_UID == search.BG_Organization_UID);
                    }
                    if (search.FunPlant_Organization_UID.HasValue)
                    {
                        query = query.Where(w => w.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                    }
                    //需求日期
                    if (search.Demand_Date_From.HasValue)
                    {
                        query = query.Where(w => w.Demand_Date >= search.Demand_Date_From.Value);
                    }
                    if (search.Demand_Date_To.HasValue)
                    {
                        var demandDateTommorrow = search.Demand_Date_To.Value.AddDays(1);
                        query = query.Where(w => w.Demand_Date < demandDateTommorrow);
                    }
                    //计算日期
                    if (search.Calculation_Date_From.HasValue)
                    {
                        query = query.Where(w => w.Calculation_Date >= search.Calculation_Date_From.Value);
                    }
                    if (search.Calculation_Date_To.HasValue)
                    {
                        var calculationDateTommorrow = search.Calculation_Date_To.Value.AddDays(1);
                        query = query.Where(w => w.Calculation_Date < calculationDateTommorrow);
                    }
                    if (search.Status_UID.HasValue)
                    {
                        query = query.Where(w => w.Status_UID == search.Status_UID.Value);
                    }
                }
                count = query.Count();
                return query.OrderByDescending(w => w.Applicant_Date).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Fixture_Part_Demand_M_UID)).OrderByDescending(w => w.Applicant_Date);
                count = 0;
                return query;
            }
        }

        public bool CalculateFixturePartDemand(int Plant_Organization_UID, int BG_Organization_UID, int? FunPlant_Organization_UID, DateTime Demand_Date, DateTime Calculation_Date, int Applicant_UID)
        {
            try
            {
                var paraPlant = new SqlParameter("@Plant_Organization_UID", Plant_Organization_UID);
                var paraOP = new SqlParameter("@BG_Organization_UID", BG_Organization_UID);
                var paraFunc = new SqlParameter("@FunPlant_Organization_UID", DBNull.Value);
                if (FunPlant_Organization_UID.HasValue)
                {
                    paraFunc = new SqlParameter("@FunPlant_Organization_UID", FunPlant_Organization_UID.Value);
                }
                var paraCalcuDate = new SqlParameter("@CalculateDate", Calculation_Date);
                var paraDemandDate = new SqlParameter("@DemandDate", Demand_Date);
                var paraAppliUID = new SqlParameter("@ApplicantUID", Applicant_UID);
                DataContext.Database.ExecuteSqlCommand("usp_CalculateFixturePartDemand @Plant_Organization_UID,@BG_Organization_UID,@FunPlant_Organization_UID,@CalculateDate,@DemandDate,@ApplicantUID"
                    , paraPlant, paraOP, paraFunc, paraCalcuDate, paraDemandDate, paraAppliUID);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
