using PDMS.Data;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IFixtureRepairDService : IBaseSevice<Fixture_Repair_D, Fixture_Repair_DDTO, Fixture_Repair_DModelSearch>
    {
    }
    public class FixtureRepairDService : BaseSevice<Fixture_Repair_D, Fixture_Repair_DDTO, Fixture_Repair_DModelSearch>, IFixtureRepairDService
    {
        public FixtureRepairDService(IFixture_Repair_DRepository fixtureRepairDRepository) : base(fixtureRepairDRepository)
        {
        }
    }
}
