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
    public interface IFixture_Part_UseTimesService : IBaseSevice<Fixture_Part_UseTimes, Fixture_Part_UseTimesDTO, Fixture_Part_UseTimesModelSearch>
    {
        string ClearFixturePartUseTimes(List<int> uidList, int modifiedUid, DateTime modifiedDate);
    }
    public class Fixture_Part_UseTimesService : BaseSevice<Fixture_Part_UseTimes, Fixture_Part_UseTimesDTO, Fixture_Part_UseTimesModelSearch>, IFixture_Part_UseTimesService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IFixture_Part_UseTimesRepository fixturePartUseTimesRepository;
        public Fixture_Part_UseTimesService(IFixture_Part_UseTimesRepository fixturePartUseTimesRepository, IUnitOfWork unitOfWork) : base(fixturePartUseTimesRepository)
        {
            this.fixturePartUseTimesRepository = fixturePartUseTimesRepository;
            this.unitOfWork = unitOfWork;
        }

        public string ClearFixturePartUseTimes(List<int> uidList, int modifiedUid, DateTime modifiedDate)
        {
            var result = "";
            if (uidList != null && uidList.Count>0)
            {
                var usetimesList = fixturePartUseTimesRepository.GetMany(x => uidList.Contains(x.Fixture_Part_UseTimes_UID)).ToList();
                if (usetimesList.Count > 0)
                {
                    foreach (var item in usetimesList)
                    {
                        item.Fixture_Part_UseTimesCount = 0;
                        item.Modified_UID = modifiedUid;
                        item.Modified_Date = modifiedDate;
                    }
                    try
                    {
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        result = string.Format("更换配件失败，错误信息:{0}", ex.Message);
                    }
                }
            }
            
            return result;
        }
    }
}
