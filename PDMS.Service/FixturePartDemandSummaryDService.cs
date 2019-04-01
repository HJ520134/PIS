using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IFixturePartDemandSummaryDService : IBaseSevice<Fixture_Part_Demand_Summary_D, Fixture_Part_Demand_Summary_DDTO, Fixture_Part_Demand_Summary_DModelSearch>
    {
        bool UpdateFixturePartDemandSummaryDList(List<Fixture_Part_Demand_Summary_DDTO> fixturePartDemandDList);
    }
    public class FixturePartDemandSummaryDService : BaseSevice<Fixture_Part_Demand_Summary_D, Fixture_Part_Demand_Summary_DDTO, Fixture_Part_Demand_Summary_DModelSearch>, IFixturePartDemandSummaryDService
    {
        private readonly IFixture_Part_Demand_Summary_DRepository fixturePartDemandSummaryDRepository;
        private readonly IUnitOfWork unitOfWork;
        public FixturePartDemandSummaryDService(IFixture_Part_Demand_Summary_DRepository fixturePartDemandSummaryDRepository, IUnitOfWork unitOfWork)
            : base(fixturePartDemandSummaryDRepository)
        {
            this.fixturePartDemandSummaryDRepository = fixturePartDemandSummaryDRepository;
            this.unitOfWork = unitOfWork;
        }
        public bool UpdateFixturePartDemandSummaryDList(List<Fixture_Part_Demand_Summary_DDTO> fixturePartDemandDList)
        {
            try
            {
                foreach (var item in fixturePartDemandDList)
                {
                    var fixturePartDeamndD = fixturePartDemandSummaryDRepository.GetFirstOrDefault(i => i.Fixture_Part_Demand_Summary_D_UID == item.Fixture_Part_Demand_Summary_D_UID);
                    if (item.Is_Deleted != fixturePartDeamndD.Is_Deleted)
                    {
                        fixturePartDeamndD.Is_Deleted = item.Is_Deleted;
                        fixturePartDemandSummaryDRepository.Update(fixturePartDeamndD);
                    }
                }
                unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
