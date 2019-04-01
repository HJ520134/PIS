using PDMS.Data;
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
    public interface IFixturePartSettingMService : IBaseSevice<Fixture_Part_Setting_M, Fixture_Part_Setting_MDTO, Fixture_Part_Setting_MModelSearch>
    {
    }
    public class FixturePartSettingMService: BaseSevice<Fixture_Part_Setting_M, Fixture_Part_Setting_MDTO, Fixture_Part_Setting_MModelSearch>, IFixturePartSettingMService
    {

        //private readonly IFixture_Part_Setting_MRepository fixturePartSettingMRepository;
        public FixturePartSettingMService(IFixture_Part_Setting_MRepository fixturePartSettingMRepository)
            :base(fixturePartSettingMRepository)
        {
            //this.fixturePartSettingMRepository = fixturePartSettingMRepository;
        }
    }
}
