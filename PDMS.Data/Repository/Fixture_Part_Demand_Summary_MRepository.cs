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
    public interface IFixture_Part_Demand_Summary_MRepository : IRepository<Fixture_Part_Demand_Summary_M>
    {
        IQueryable<Fixture_Part_Demand_Summary_M> QueryFixturePartDemandSummaryMs(Fixture_Part_Demand_Summary_MModelSearch search, Page page, out int count);
        bool CalculateFixturePartDemandSummary(int Fixture_Part_Demand_M_UID,int Applicant_UID);
        void FixturePartDemandSubmitOrder(int demandSummaryMUID, int modifiedUID);
    }
    public class Fixture_Part_Demand_Summary_MRepository : RepositoryBase<Fixture_Part_Demand_Summary_M>, IFixture_Part_Demand_Summary_MRepository
    {
        public Fixture_Part_Demand_Summary_MRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
        public IQueryable<Fixture_Part_Demand_Summary_M> QueryFixturePartDemandSummaryMs(Fixture_Part_Demand_Summary_MModelSearch search, Page page, out int count)
        {
            var query = from w in DataContext.Fixture_Part_Demand_Summary_M select w;
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
                    //审核日期
                    if (search.Approver_Date_From.HasValue)
                    {
                        query = query.Where(w => w.Approver_Date >= search.Approver_Date_From.Value);
                    }
                    if (search.Approver_Date_To.HasValue)
                    {
                        var approveDateTommorrow = search.Approver_Date_To.Value.AddDays(1);
                        query = query.Where(w => w.Approver_Date < approveDateTommorrow);
                    }
                    if (search.Status_UID.HasValue)
                    {
                        query = query.Where(w => w.Status_UID == search.Status_UID.Value);
                    }
                }
                count = query.Count();
                return query.OrderBy(w => w.Applicant_Date).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Fixture_Part_Demand_Summary_M_UID)).OrderBy(w => w.Applicant_Date);
                count = 0;
                return query;
            }
        }

        public bool CalculateFixturePartDemandSummary(int Fixture_Part_Demand_M_UID,int Applicant_UID)
        {
            try
            {
                var paraDemandMUID = new SqlParameter("@Fixture_Part_Demand_M_UID", Fixture_Part_Demand_M_UID);
                var paraAppliUID = new SqlParameter("@ApplicantUID", Applicant_UID);
                DataContext.Database.ExecuteSqlCommand("usp_CalculateFixturePartDemandSummary @Fixture_Part_Demand_M_UID,@ApplicantUID", paraDemandMUID, paraAppliUID);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void FixturePartDemandSubmitOrder(int demandSummaryMUID, int modifiedUID)
        {
            try
            {
                var paraDemandSummaryMUID = new SqlParameter("@Fixture_Part_Demand_Summary_M_UID", demandSummaryMUID);
                var partModifiedUID = new SqlParameter("@Modified_UID", modifiedUID);
                DataContext.Database.ExecuteSqlCommand("usp_FixturePartDemandSubmitOrder @Fixture_Part_Demand_Summary_M_UID,@Modified_UID", paraDemandSummaryMUID, partModifiedUID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
