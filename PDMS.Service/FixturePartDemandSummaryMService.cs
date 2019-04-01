using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IFixturePartDemandSummaryMService : IBaseSevice<Fixture_Part_Demand_Summary_M, Fixture_Part_Demand_Summary_MDTO, Fixture_Part_Demand_Summary_MModelSearch>
    {
        PagedListModel<Fixture_Part_Demand_Summary_MDTO> QueryFixturePartDemandSummaryMs(Fixture_Part_Demand_Summary_MModelSearch search, Page page);
        bool CalculateFixturePartDemandSummary(int Fixture_Part_Demand_M_UID, int Applicant_UID);
        Fixture_Part_Demand_Summary_M QueryFixturePartDemandSummaryMByUID(int uid);
        bool UpdateStatus(int uid, int statusUID);
        void FixturePartDemandSubmitOrder(int demandSummaryMUID, int modifiedUID);
    }
    public class FixturePartDemandSummaryMService : BaseSevice<Fixture_Part_Demand_Summary_M, Fixture_Part_Demand_Summary_MDTO, Fixture_Part_Demand_Summary_MModelSearch>, IFixturePartDemandSummaryMService
    {
        private readonly IFixture_Part_Demand_Summary_MRepository fixturePartDemandSummaryMRepository;
        private readonly IUnitOfWork unitOfWork;
        public FixturePartDemandSummaryMService(IFixture_Part_Demand_Summary_MRepository fixturePartDemandSummaryMRepository, IUnitOfWork unitOfWork)
            :base(fixturePartDemandSummaryMRepository)
        {
            this.fixturePartDemandSummaryMRepository = fixturePartDemandSummaryMRepository;
            this.unitOfWork = unitOfWork;
        }

        public PagedListModel<Fixture_Part_Demand_Summary_MDTO> QueryFixturePartDemandSummaryMs(Fixture_Part_Demand_Summary_MModelSearch search, Page page)
        {
            var totalCount = 0;
            var fixtureDemandMs = fixturePartDemandSummaryMRepository.QueryFixturePartDemandSummaryMs(search, page, out totalCount);

            var fixture_MachineDTOList = new List<Fixture_Part_Demand_Summary_MDTO>();

            foreach (var demandM in fixtureDemandMs)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_Part_Demand_Summary_MDTO>(demandM);
                if (demandM.System_Organization != null)
                {
                    dto.Plant_Organization_Name = demandM.System_Organization.Organization_Name;
                }
                if (demandM.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = demandM.System_Organization1.Organization_Name;
                }
                if (demandM.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = demandM.System_Organization2.Organization_Name;
                }
                if (demandM.System_Users != null)
                {
                    dto.Applicant_Name = demandM.System_Users.User_Name;
                }
                if (demandM.System_Users1 != null)
                {
                    dto.Approver_Name = demandM.System_Users1.User_Name;
                }
                if (demandM.Enumeration!=null)
                {
                    dto.StatusName = demandM.Enumeration.Enum_Value;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(demandM.System_Users1);
                fixture_MachineDTOList.Add(dto);
            }

            return new PagedListModel<Fixture_Part_Demand_Summary_MDTO>(totalCount, fixture_MachineDTOList.OrderByDescending(i=>i.Demand_Date));
        }

        public bool CalculateFixturePartDemandSummary(int Fixture_Part_Demand_M_UID, int Applicant_UID)
        {
            var culculateDate = DateTime.Now;
            try
            {
                fixturePartDemandSummaryMRepository.CalculateFixturePartDemandSummary(Fixture_Part_Demand_M_UID, Applicant_UID);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Fixture_Part_Demand_Summary_M QueryFixturePartDemandSummaryMByUID(int uid)
        {
            var demandM = fixturePartDemandSummaryMRepository.GetFirstOrDefault(i => i.Fixture_Part_Demand_Summary_M_UID == uid);
            return demandM;
        }

        public bool UpdateStatus(int uid, int statusUID) {
            try
            {
                var demandM = fixturePartDemandSummaryMRepository.GetFirstOrDefault(i => i.Fixture_Part_Demand_Summary_M_UID == uid);
                demandM.Status_UID = statusUID;
                fixturePartDemandSummaryMRepository.Update(demandM);
                unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void FixturePartDemandSubmitOrder(int demandSummaryMUID, int modifiedUID)
        {
            try
            {
                fixturePartDemandSummaryMRepository.FixturePartDemandSubmitOrder(demandSummaryMUID, modifiedUID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public Fixture_Part_Demand_Summary_MDTO QueryFixturePartDemandMByUID(int uid) {
        //    var demandM=fixturePartDemandMRepository.GetFirstOrDefault(i => i.Fixture_Part_Demand_Summary_M_UID == uid);
        //    var demandMDto = AutoMapper.Mapper.Map<Fixture_Part_Demand_Summary_MDTO>(demandM);
        //    var demandDDtoList = AutoMapper.Mapper.Map<ICollection<Fixture_Part_Demand_DDTO>>(demandM.Fixture_Part_Demand_D);
        //    demandMDto.Fixture_Part_Demand_DDTOList = demandDDtoList.ToList();
        //    return demandMDto;
        //}
    }
}
