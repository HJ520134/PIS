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
    public interface IFixturePartDemandMService : IBaseSevice<Fixture_Part_Demand_M, Fixture_Part_Demand_MDTO, Fixture_Part_Demand_MModelSearch>
    {
        PagedListModel<Fixture_Part_Demand_MDTO> QueryFixturePartDemandMs(Fixture_Part_Demand_MModelSearch search, Page page);
        bool CalculateFixturePartDemand(int Plant_Organization_UID, int BG_Organization_UID, int? FunPlant_Organization_UID, DateTime Demand_Date,int Applicant_UID);
        Fixture_Part_Demand_M QueryFixturePartDemandMByUID(int uid);
        bool UpdateStatus(int uid, int statusUID);
        /// <summary>
        /// 审核取消，删除需求汇总数据
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool ApproveCancelFixturPartDemand(int uid);
    }
    public class FixturePartDemandMService : BaseSevice<Fixture_Part_Demand_M, Fixture_Part_Demand_MDTO, Fixture_Part_Demand_MModelSearch>, IFixturePartDemandMService
    {
        private readonly IFixture_Part_Demand_MRepository fixturePartDemandMRepository;
        private readonly IFixture_Part_Demand_DRepository fixturePartDemandDRepository;
        private readonly IFixture_Part_Demand_Summary_MRepository fixturePartDemandSummaryMRepository;
        private readonly IFixture_Part_Demand_Summary_DRepository fixturePartDemandSummaryDRepository;
        private readonly IUnitOfWork unitOfWork;
        public FixturePartDemandMService(IFixture_Part_Demand_MRepository fixturePartDemandMRepository, IFixture_Part_Demand_DRepository fixturePartDemandDRepository, IFixture_Part_Demand_Summary_MRepository fixturePartDemandSummaryMRepository, IFixture_Part_Demand_Summary_DRepository fixturePartDemandSummaryDRepository, IUnitOfWork unitOfWork)
            :base(fixturePartDemandMRepository)
        {
            this.fixturePartDemandMRepository = fixturePartDemandMRepository;
            this.fixturePartDemandDRepository = fixturePartDemandDRepository;
            this.fixturePartDemandSummaryMRepository = fixturePartDemandSummaryMRepository;
            this.fixturePartDemandSummaryDRepository = fixturePartDemandSummaryDRepository;
            this.unitOfWork = unitOfWork;
        }

        public PagedListModel<Fixture_Part_Demand_MDTO> QueryFixturePartDemandMs(Fixture_Part_Demand_MModelSearch search, Page page)
        {
            var totalCount = 0;
            var fixtureDemandMs = fixturePartDemandMRepository.QueryFixturePartDemandMs(search, page, out totalCount);

            var fixture_MachineDTOList = new List<Fixture_Part_Demand_MDTO>();

            foreach (var demandM in fixtureDemandMs)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_Part_Demand_MDTO>(demandM);
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

            return new PagedListModel<Fixture_Part_Demand_MDTO>(totalCount, fixture_MachineDTOList);
        }

        public bool CalculateFixturePartDemand(int Plant_Organization_UID, int BG_Organization_UID, int? FunPlant_Organization_UID, DateTime Demand_Date, int Applicant_UID)
        {
            var culculateDate = DateTime.Now;
            try
            {
                fixturePartDemandMRepository.CalculateFixturePartDemand(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, Demand_Date, culculateDate, Applicant_UID);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Fixture_Part_Demand_M QueryFixturePartDemandMByUID(int uid)
        {
            var demandM = fixturePartDemandMRepository.GetFirstOrDefault(i => i.Fixture_Part_Demand_M_UID == uid);
            return demandM;
        }

        public bool UpdateStatus(int uid, int statusUID) {
            try
            {
                var demandM = fixturePartDemandMRepository.GetFirstOrDefault(i => i.Fixture_Part_Demand_M_UID == uid);
                demandM.Status_UID = statusUID;
                fixturePartDemandMRepository.Update(demandM);
                unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 取消审核，删除汇总数据
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool ApproveCancelFixturPartDemand(int uid)
        {
            try
            {
                var demandM = fixturePartDemandMRepository.GetFirstOrDefault(i => i.Fixture_Part_Demand_M_UID == uid);
                var demandSummaryM = fixturePartDemandSummaryMRepository.GetFirstOrDefault(i => i.Plant_Organization_UID == demandM.Plant_Organization_UID && i.BG_Organization_UID == demandM.BG_Organization_UID && i.Plant_Organization_UID == demandM.Plant_Organization_UID && i.Demand_Date == demandM.Demand_Date);
                if (demandSummaryM.Enumeration.Enum_Value == "已审核" || demandSummaryM.Enumeration.Enum_Value == "已采购")
                {
                    fixturePartDemandSummaryDRepository.DeleteList(demandSummaryM.Fixture_Part_Demand_Summary_D.ToList());
                    fixturePartDemandSummaryMRepository.Delete(demandSummaryM);
                    unitOfWork.Commit();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public Fixture_Part_Demand_MDTO QueryFixturePartDemandMByUID(int uid) {
        //    var demandM=fixturePartDemandMRepository.GetFirstOrDefault(i => i.Fixture_Part_Demand_M_UID == uid);
        //    var demandMDto = AutoMapper.Mapper.Map<Fixture_Part_Demand_MDTO>(demandM);
        //    var demandDDtoList = AutoMapper.Mapper.Map<ICollection<Fixture_Part_Demand_DDTO>>(demandM.Fixture_Part_Demand_D);
        //    demandMDto.Fixture_Part_Demand_DDTOList = demandDDtoList.ToList();
        //    return demandMDto;
        //}
    }
}
