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
    public interface IFixturePartDemandDService : IBaseSevice<Fixture_Part_Demand_D, Fixture_Part_Demand_DDTO, Fixture_Part_Demand_DModelSearch>
    {
        bool UpdateFixturePartDemandDList(List<Fixture_Part_Demand_DDTO> fixturePartDemandDList);
    }
    public class FixturePartDemandDService : BaseSevice<Fixture_Part_Demand_D, Fixture_Part_Demand_DDTO, Fixture_Part_Demand_DModelSearch>, IFixturePartDemandDService
    {
        private readonly IFixture_Part_Demand_DRepository fixturePartDemandDRepository;
        private readonly IUnitOfWork unitOfWork;
        public FixturePartDemandDService(IFixture_Part_Demand_DRepository fixturePartDemandDRepository, IUnitOfWork unitOfWork)
            : base(fixturePartDemandDRepository)
        {
            this.fixturePartDemandDRepository = fixturePartDemandDRepository;
            this.unitOfWork = unitOfWork;
        }
        public bool UpdateFixturePartDemandDList(List<Fixture_Part_Demand_DDTO> fixturePartDemandDList)
        {
            try
            {
                foreach (var item in fixturePartDemandDList)
                {
                    var fixturePartDeamndD = fixturePartDemandDRepository.GetFirstOrDefault(i => i.Fixture_Part_Demand_D_UID == item.Fixture_Part_Demand_D_UID);
                    if (item.User_Adjustments_Qty != fixturePartDeamndD.User_Adjustments_Qty || item.Is_Deleted != fixturePartDeamndD.Is_Deleted)
                    {
                        fixturePartDeamndD.User_Adjustments_Qty = item.User_Adjustments_Qty;
                        fixturePartDeamndD.Is_Deleted = item.Is_Deleted;
                        fixturePartDemandDRepository.Update(fixturePartDeamndD);
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
