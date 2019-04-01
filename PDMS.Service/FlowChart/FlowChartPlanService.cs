using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Repository;
using PDMS.Model;
using System.Transactions;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Model.ViewModels;

namespace PDMS.Service.FlowChart
{
    public interface IFlowChartPlanService
    {
        PagedListModel<FlPlanManagerVM> QueryProcessMGData(int masterUID, DateTime date);
        PagedListModel<IEPlanManagerVM> QueryProcessIEMGData(int masterUID, DateTime date);
        FlowChartPlanManagerVM QueryProcessMGDataSingle(int uid, DateTime date);
        IEPlanManagerVM QueryProcessIEMGDataSingle(int uid, DateTime date,int shiftTimeId);
        IEPlanManagerVM QueryProcessIEMGDataSingle1(int uid, DateTime date);
        int getTheLastVersionDetailUID(int uid);
        string FlowChartPlan(FlowChartPlanManagerVM ent);
        string FlowIEChartPlan(IEPlanManagerVM ent);
        List<FlowChartDetailAndMgData> QueryFLDetailList(int id, string week);
        PagedListModel<PrjectListVM> QueryProjectList(int user_account_uid, bool MHFlag_MulitProject);
        ProcessDataSearch QueryFlowChartDataByMasterUid(int flowChartMaster_uid);
        List<string> QueryPlantByUser(int userid);
        List<FlowChartMasterDTO> QueryProjectTypes(string project);
        List<string> QueryProcess(int flowchartmasterUid);
        List<FlowChartMgDataDTO> QueryFLMgDataList(List<int> flDetailUIDList);

    }

    public class FlowChartPlanService : IFlowChartPlanService
    {
        #region Private interfaces properties
        private readonly IUnitOfWork unitOfWork;
        private readonly IFlowChartMasterRepository flowChartMasterRepository;
        private readonly IFlowChartDetailRepository flowChartDetailRepository;
        private readonly IFlowChartMgDataRepository flowChartMgDataRepository;
        private readonly IFlowChartIEMgDataRepository flowChartIEMgDataRepository;
        private readonly ISystemBUDRepository systemBUDRepository;
        private readonly ISystemProjectRepository systemProjectRepository;
        private readonly ISystemFunctionPlantRepository systemFunctionPlantRepository;
        private readonly ISystemUserRepository systemUserRepository;
        private readonly IFlowChartPCMHRelationshipRepository flowChartPCMHRelationshipRepository;
        private readonly ISystemUserRoleRepository systemUserRoleRepository;
        private readonly ISystemRoleRepository systemRoleRepository;
        private readonly ISystemUserOrgRepository systemUserOrgRepository;
        private readonly IProjectUsersGroupRepository projectUsersGroupRepository;
        private readonly ISystemOrgRepository systemOrgRepository;
        private readonly ISystemOrgBomRepository systemOrgBomRepository;

        #endregion //Private interfaces properties

        #region Service constructor
        public FlowChartPlanService(
            IFlowChartMasterRepository flowChartMasterRepository,
            IFlowChartDetailRepository flowChartDetailRepository,
            IFlowChartMgDataRepository flowChartMgDataRepository,
            IFlowChartIEMgDataRepository flowChartIEMgDataRepository,
            ISystemBUDRepository systemBUDRepository,
            ISystemProjectRepository systemProjectRepository,
            ISystemFunctionPlantRepository systemFunctionPlantRepository,
            ISystemUserRepository systemUserRepository,
            ISystemUserRoleRepository systemUserRoleRepository,
            IFlowChartPCMHRelationshipRepository flowChartPCMHRelationshipRepository,
            ISystemRoleRepository systemRoleRepository,
            ISystemUserOrgRepository systemUserOrgRepository,
            IProjectUsersGroupRepository projectUsersGroupRepository,
            ISystemOrgRepository systemOrgRepository,
            ISystemOrgBomRepository systemOrgBomRepository,
        IUnitOfWork unitOfWork)
        {
            this.systemRoleRepository = systemRoleRepository;
            this.unitOfWork = unitOfWork;
            this.flowChartMasterRepository = flowChartMasterRepository;
            this.flowChartDetailRepository = flowChartDetailRepository;
            this.flowChartMgDataRepository = flowChartMgDataRepository;
            this.flowChartIEMgDataRepository = flowChartIEMgDataRepository;
            this.systemProjectRepository = systemProjectRepository;
            this.systemBUDRepository = systemBUDRepository;
            this.systemFunctionPlantRepository = systemFunctionPlantRepository;
            this.systemUserRepository = systemUserRepository;
            this.systemUserRoleRepository = systemUserRoleRepository;
            this.flowChartPCMHRelationshipRepository = flowChartPCMHRelationshipRepository;
            this.systemUserOrgRepository = systemUserOrgRepository;
            this.projectUsersGroupRepository = projectUsersGroupRepository;
            this.systemOrgRepository = systemOrgRepository;
            this.systemOrgBomRepository = systemOrgBomRepository;
        }
        #endregion //Service constructor

        /// <summary>
        /// 根据flowchart masteruid 查询计划数据
        /// </summary>
        /// <param name="masterUID"></param>
        /// <returns></returns>
        public PagedListModel<FlPlanManagerVM> QueryProcessMGData(int masterUID, DateTime date)
        {
            var totalCount = 0;

            var flChartList = flowChartMasterRepository.QueryFlowMGData(masterUID, date, out totalCount);
            var result = new List<FlPlanManagerVM>();
            foreach (var item in flChartList)
            {
                var returnItem = new FlPlanManagerVM();
                returnItem.Detail_UID = item.Detail_UID;
                returnItem.Process_seq = item.Process_seq;
                returnItem.Process = item.Process;
                returnItem.Place = item.Place;
                returnItem.date = item.date;
                returnItem.Color = item.Color;
                returnItem.MondayProduct_Plan = item.MondayProduct_Plan;
                if (item.MondayTarget_Yield != null)
                {
                    returnItem.MondayTarget_Yield = item.MondayTarget_Yield * 100 + "%";
                }
                returnItem.MondayProper_WIP = item.MondayProper_WIP;
                returnItem.TuesdayProduct_Plan = item.TuesdayProduct_Plan;
                if (item.TuesdayTarget_Yield != null)
                {
                    returnItem.TuesdayTarget_Yield = item.TuesdayTarget_Yield * 100 + "%";
                }
                returnItem.TuesdayProper_WIP = item.TuesdayProper_WIP;
                returnItem.WednesdayProduct_Plan = item.WednesdayProduct_Plan;
                if (item.WednesdayTarget_Yield != null)
                {
                    returnItem.WednesdayTarget_Yield = item.WednesdayTarget_Yield * 100 + "%";
                }
                returnItem.WednesdayProper_WIP = item.WednesdayProper_WIP;
                returnItem.ThursdayProduct_Plan = item.ThursdayProduct_Plan;
                if (item.ThursdayTarget_Yield != null)
                {
                    returnItem.ThursdayTarget_Yield = item.ThursdayTarget_Yield * 100 + "%";
                }
                returnItem.ThursdayProper_WIP = item.ThursdayProper_WIP;
                returnItem.FridayProduct_Plan = item.FridayProduct_Plan;
                if (item.FridayTarget_Yield != null)
                {
                    returnItem.FridayTarget_Yield = item.FridayTarget_Yield * 100 + "%";
                }
                returnItem.FridayProper_WIP = item.FridayProper_WIP;
                returnItem.SaterdayProduct_Plan = item.SaterdayProduct_Plan;
                if (item.SaterdayTarget_Yield != null)
                {
                    returnItem.SaterdayTarget_Yield = item.SaterdayTarget_Yield * 100 + "%";
                }
                returnItem.SaterdayProper_WIP = item.SaterdayProper_WIP;

                returnItem.SundayProduct_Plan = item.SundayProduct_Plan;
                if (item.SundayTarget_Yield != null)
                {
                    returnItem.SundayTarget_Yield = item.SundayTarget_Yield * 100 + "%";
                }
                returnItem.SundayProper_WIP = item.SundayProper_WIP;
                result.Add(returnItem);
            }

            return new PagedListModel<FlPlanManagerVM>(totalCount, result);
        }

        //IE
        public PagedListModel<IEPlanManagerVM> QueryProcessIEMGData(int masterUID, DateTime date)
        {
            var totalCount = 0;

            var flChartList = flowChartMasterRepository.QueryFlowIEMGData(masterUID, date, out totalCount);
            var result = new List<IEPlanManagerVM>();
            foreach (var item in flChartList)
            {
                var returnItem = new IEPlanManagerVM();
                returnItem.Detail_UID = item.Detail_UID;
                returnItem.Process_seq = item.Process_seq;
                returnItem.Process = item.Process;
                returnItem.Place = item.Place;
                returnItem.date = item.date;
                returnItem.Color = item.Color;
                returnItem.ShiftTimeID = item.ShiftTimeID;
                returnItem.MondayIE_TargetEfficacy = item.MondayIE_TargetEfficacy;
                returnItem.MondayIE_DeptHuman = item.MondayIE_DeptHuman;
                
                returnItem.ThursdayIE_TargetEfficacy = item.ThursdayIE_TargetEfficacy;
                returnItem.ThursdayIE_DeptHuman = item.ThursdayIE_DeptHuman;
                returnItem.WednesdayIE_TargetEfficacy = item.WednesdayIE_TargetEfficacy;
                returnItem.WednesdayIE_DeptHuman = item.WednesdayIE_DeptHuman;
                returnItem.TuesdayIE_TargetEfficacy = item.TuesdayIE_TargetEfficacy;
                returnItem.TuesdayIE_DeptHuman = item.TuesdayIE_DeptHuman;
                returnItem.FridayIE_TargetEfficacy = item.FridayIE_TargetEfficacy;
                returnItem.FridayIE_DeptHuman = item.FridayIE_DeptHuman;
                returnItem.SaterdayIE_TargetEfficacy = item.SaterdayIE_TargetEfficacy;
                returnItem.SaterdayIE_DeptHuman = item.SaterdayIE_DeptHuman;
                returnItem.SundayIE_TargetEfficacy = item.SundayIE_TargetEfficacy;
                returnItem.SundayIE_DeptHuman = item.SundayIE_DeptHuman;

               
                result.Add(returnItem);
            }

            return new PagedListModel<IEPlanManagerVM>(totalCount, result);
        }

        public int getTheLastVersionDetailUID(int uid)
        {
            return flowChartMasterRepository.getTheLastVersionDetailUID(uid);
        }

        public FlowChartPlanManagerVM QueryProcessMGDataSingle(int uid, DateTime date)
        {
            return flowChartMasterRepository.QueryFlowMGDataSingle(uid, date);
        }


        //public IEPlanManagerVM1 QueryProcessIEMGDataSingle(int uid, DateTime date,int shiftTimeId)
        //{
        //    return flowChartMasterRepository.QueryFlowIEMGDataSingle(uid, date, shiftTimeId);
        //}

        public IEPlanManagerVM QueryProcessIEMGDataSingle1(int uid, DateTime date)
        {
            return flowChartMasterRepository.QueryFlowIEMGDataSingle1(uid, date);
        }


        public string FlowChartPlan(FlowChartPlanManagerVM ent)
        {
            var items = flowChartMasterRepository.UpdatePlan(ent.Detail_UID, ent.date);
            int i = 0;

            foreach (var item in items)
            {
                i++;
                if (Week(item.Product_Date) == "星期一")
                {
                    item.Product_Plan = int.Parse(ent.MondayProduct_Plan.ToString());
                    if (ent.MondayProper_WIP != null)
                        item.Proper_WIP= int.Parse(ent.MondayProper_WIP.ToString());
                    item.Target_Yield = double.Parse(ent.MondayTarget_Yield.ToString());
                    flowChartMgDataRepository.Update(item);
                }
                if (Week(item.Product_Date) == "星期二")
                {
                    item.Product_Plan = int.Parse(ent.TuesdayProduct_Plan.ToString());
                    if (ent.TuesdayProper_WIP != null)
                        item.Proper_WIP = int.Parse(ent.TuesdayProper_WIP.ToString());
                    item.Target_Yield = double.Parse(ent.TuesdayTarget_Yield.ToString());
                    flowChartMgDataRepository.Update(item);
                }
                if (Week(item.Product_Date) == "星期三")
                {
                    item.Product_Plan = int.Parse(ent.WednesdayProduct_Plan.ToString());
                    if (ent.WednesdayProper_WIP != null)
                        item.Proper_WIP = int.Parse(ent.WednesdayProper_WIP.ToString());
                    item.Target_Yield = double.Parse(ent.WednesdayTarget_Yield.ToString());
                    flowChartMgDataRepository.Update(item);
                }
                if (Week(item.Product_Date) == "星期四")
                {
                    item.Product_Plan = int.Parse(ent.ThursdayProduct_Plan.ToString());
                    if (ent.ThursdayProper_WIP != null)
                        item.Proper_WIP = int.Parse(ent.ThursdayProper_WIP.ToString());
                    item.Target_Yield = double.Parse(ent.ThursdayTarget_Yield.ToString());
                    flowChartMgDataRepository.Update(item);
                }
                if (Week(item.Product_Date) == "星期五")
                {
                    item.Product_Plan = int.Parse(ent.FridayProduct_Plan.ToString());
                    if (ent.FridayProper_WIP != null)
                        item.Proper_WIP = int.Parse(ent.FridayProper_WIP.ToString());
                    item.Target_Yield = double.Parse(ent.FridayTarget_Yield.ToString());
                    flowChartMgDataRepository.Update(item);
                }
                if (Week(item.Product_Date) == "星期六")
                {
                    item.Product_Plan = int.Parse(ent.SaterdayProduct_Plan.ToString());
                    if(ent.SaterdayProper_WIP!=null)
                    item.Proper_WIP = int.Parse(ent.SaterdayProper_WIP.ToString());
                    item.Target_Yield = double.Parse(ent.SaterdayTarget_Yield.ToString());
                    flowChartMgDataRepository.Update(item);
                }
                if (Week(item.Product_Date) == "星期日")
                {
                    item.Product_Plan = int.Parse(ent.SundayProduct_Plan.ToString());
                    if (ent.SundayProper_WIP != null)
                        item.Proper_WIP = int.Parse(ent.SundayProper_WIP.ToString());
                    item.Target_Yield = double.Parse(ent.SundayTarget_Yield.ToString());
                    flowChartMgDataRepository.Update(item);
                }

            }
            unitOfWork.Commit();
            return "SUCCESS";
        }


        public string FlowIEChartPlan(IEPlanManagerVM ent)
        {
            var items = flowChartMasterRepository.UpdateIEPlan(ent.Detail_UID, ent.date,ent.ShiftTimeID);
            int i = 0;

            foreach (var item in items)
            {
                i++;
               // item.ShiftTimeID = int.Parse(ent.ShiftTimeID.ToString());

                if (Week(item.IE_TargetDate) == "星期一")
                {
                    item.IE_TargetEfficacy = int.Parse(ent.MondayIE_TargetEfficacy.ToString());
                   
                    item.IE_DeptHuman = int.Parse(ent.MondayIE_DeptHuman.ToString());
                   flowChartIEMgDataRepository.Update(item);
                }
                if (Week(item.IE_TargetDate) == "星期二")
                {
                    item.IE_TargetEfficacy = int.Parse(ent.TuesdayIE_TargetEfficacy.ToString());
                    
                    item.IE_DeptHuman = int.Parse(ent.TuesdayIE_DeptHuman.ToString());
                 flowChartIEMgDataRepository.Update(item);
                }
                if (Week(item.IE_TargetDate) == "星期三")
                {
                    item.IE_TargetEfficacy = int.Parse(ent.WednesdayIE_TargetEfficacy.ToString());

                    item.IE_DeptHuman = int.Parse(ent.WednesdayIE_DeptHuman.ToString());
                 flowChartIEMgDataRepository.Update(item);
                }
                if (Week(item.IE_TargetDate) == "星期四")
                {
                    item.IE_TargetEfficacy = int.Parse(ent.ThursdayIE_TargetEfficacy.ToString());

                    item.IE_DeptHuman = int.Parse(ent.ThursdayIE_DeptHuman.ToString());
                    flowChartIEMgDataRepository.Update(item);
                }
                if (Week(item.IE_TargetDate) == "星期五")
                {
                    item.IE_TargetEfficacy = int.Parse(ent.FridayIE_TargetEfficacy.ToString());

                    item.IE_DeptHuman = int.Parse(ent.FridayIE_DeptHuman.ToString());
                    flowChartIEMgDataRepository.Update(item);
                }
                if (Week(item.IE_TargetDate) == "星期六")
                {
                    item.IE_TargetEfficacy = int.Parse(ent.SaterdayIE_TargetEfficacy.ToString());

                    item.IE_DeptHuman = int.Parse(ent.SaterdayIE_DeptHuman.ToString());
                    flowChartIEMgDataRepository.Update(item);
                }
                if (Week(item.IE_TargetDate) == "星期日")
                {
                    item.IE_TargetEfficacy = int.Parse(ent.SundayIE_TargetEfficacy.ToString());

                    item.IE_DeptHuman = int.Parse(ent.SundayIE_DeptHuman.ToString());
                    flowChartIEMgDataRepository.Update(item);
                }

            }
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string Week(DateTime d)
        {
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weekdays[Convert.ToInt32(d.DayOfWeek)];

            return week;
        }

        public List<FlowChartDetailAndMgData> QueryFLDetailList(int id, string week)
        {
            Week getWeek = new Model.ViewModels.Week();
            List<FlowChartMgDataDTO> mgDataList = new List<FlowChartMgDataDTO>();
            List<int> flDetailUIDList = new List<int>();
            switch (week)
            {
                case "next":
                    getWeek = GetCurrentWeek(DateTime.Now.Date);
                    break;
                case "current":
                    getWeek = GetLastWeek(DateTime.Now.Date);
                    break;
            }
            var maxVersion = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == id).Max(m => m.FlowChart_Version);
            var list = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == id && m.FlowChart_Version == maxVersion).OrderBy(m => m.Process_Seq).ToList();
            var dto = AutoMapper.Mapper.Map<List<FlowChartDetailAndMgData>>(list);
            if (list.Count() > 0)
            {
                flDetailUIDList = list.Select(m => m.FlowChart_Detail_UID).ToList();
                var mgList = flowChartMgDataRepository.GetMany(m => flDetailUIDList.Contains(m.FlowChart_Detail_UID) && m.Product_Date >= getWeek.Monday && m.Product_Date <= getWeek.Sunday).ToList();
                mgDataList = AutoMapper.Mapper.Map<List<FlowChartMgDataDTO>>(mgList);

                foreach (var flDetailMgDataItem in dto)
                {
                    var currentMgList = mgDataList.Where(m => m.FlowChart_Detail_UID == flDetailMgDataItem.FlowChart_Detail_UID).ToList();
                    flDetailMgDataItem.MgDataList = currentMgList;
                }
            }
            return dto;
        }

        //获取这周和下周的生产计划数据
        public List<FlowChartMgDataDTO> QueryFLMgDataList(List<int> flDetailUIDList)
        {
            Week getWeek = new Week();
            //获取本周的日期
            getWeek = GetCurrentWeek(DateTime.Now.Date);
            //本周的日期再加7天就是下周的周日
            getWeek.Sunday = getWeek.Sunday.AddDays(7);
            //获取上个版本的flowchart的这周和下周的生产计划
            var mgList = flowChartMgDataRepository.GetMany(m => flDetailUIDList.Contains(m.FlowChart_Detail_UID) && m.Product_Date >= getWeek.Monday && m.Product_Date <= getWeek.Sunday).ToList();
            var mgDataList = AutoMapper.Mapper.Map<List<FlowChartMgDataDTO>>(mgList);
            return mgDataList;
        }

        private Week GetNextWeek(DateTime dt)
        {
            var strDT = dt.DayOfWeek.ToString();
            Week nextWeek = new Week();
            switch (strDT)
            {
                case "Monday":
                    nextWeek.Monday = dt.AddDays(7);
                    break;
                case "Tuesday":
                    nextWeek.Monday = dt.AddDays(6);
                    break;
                case "Wednesday":
                    nextWeek.Monday = dt.AddDays(5);
                    break;
                case "Thursday":
                    nextWeek.Monday = dt.AddDays(4);
                    break;
                case "Friday":
                    nextWeek.Monday = dt.AddDays(3);
                    break;
                case "Saturday":
                    nextWeek.Monday = dt.AddDays(2);
                    break;
                case "Sunday":
                    nextWeek.Monday = dt.AddDays(1);
                    break;
            }
            nextWeek.Tuesday = nextWeek.Monday.AddDays(1);
            nextWeek.Wednesday = nextWeek.Monday.AddDays(2);
            nextWeek.Thursday = nextWeek.Monday.AddDays(3);
            nextWeek.Friday = nextWeek.Monday.AddDays(4);
            nextWeek.Saturday = nextWeek.Monday.AddDays(5);
            nextWeek.Sunday = nextWeek.Monday.AddDays(6);

            return nextWeek;
        }

        private Week GetCurrentWeek(DateTime dt)
        {
            var strDT = dt.DayOfWeek.ToString();
            Week currentWeek = new Week();
            switch (strDT)
            {
                case "Monday":
                    currentWeek.Monday = dt;
                    break;
                case "Tuesday":
                    currentWeek.Monday = dt.AddDays(-1);
                    break;
                case "Wednesday":
                    currentWeek.Monday = dt.AddDays(-2);
                    break;
                case "Thursday":
                    currentWeek.Monday = dt.AddDays(-3);
                    break;
                case "Friday":
                    currentWeek.Monday = dt.AddDays(-4);
                    break;
                case "Saturday":
                    currentWeek.Monday = dt.AddDays(-5);
                    break;
                case "Sunday":
                    currentWeek.Monday = dt.AddDays(-6);
                    break;
            }
            currentWeek.Tuesday = currentWeek.Monday.AddDays(1);
            currentWeek.Wednesday = currentWeek.Monday.AddDays(2);
            currentWeek.Thursday = currentWeek.Monday.AddDays(3);
            currentWeek.Friday = currentWeek.Monday.AddDays(4);
            currentWeek.Saturday = currentWeek.Monday.AddDays(5);
            currentWeek.Sunday = currentWeek.Monday.AddDays(6);
            return currentWeek;
        }

        private Week GetLastWeek(DateTime dt)
        {
            var strDT = dt.DayOfWeek.ToString();
            Week nextWeek = new Week();
            //获取下周一的日期
            switch (strDT)
            {
                case "Monday":
                    nextWeek.Monday = dt.AddDays(-7);
                    break;
                case "Tuesday":
                    nextWeek.Monday = dt.AddDays(-8);
                    break;
                case "Wednesday":
                    nextWeek.Monday = dt.AddDays(-9);
                    break;
                case "Thursday":
                    nextWeek.Monday = dt.AddDays(-10);
                    break;
                case "Friday":
                    nextWeek.Monday = dt.AddDays(-11);
                    break;
                case "Saturday":
                    nextWeek.Monday = dt.AddDays(-12);
                    break;
                case "Sunday":
                    nextWeek.Monday = dt.AddDays(-13);
                    break;
            }
            nextWeek.Tuesday = nextWeek.Monday.AddDays(1);
            nextWeek.Wednesday = nextWeek.Monday.AddDays(2);
            nextWeek.Thursday = nextWeek.Monday.AddDays(3);
            nextWeek.Friday = nextWeek.Monday.AddDays(4);
            nextWeek.Saturday = nextWeek.Monday.AddDays(5);
            nextWeek.Sunday = nextWeek.Monday.AddDays(6);

            return nextWeek;
        }

        public PagedListModel<PrjectListVM> QueryProjectList(int Account_uid, bool MHFlag_MulitProject)
        {
            List<OrganiztionVM> tempResult = systemOrgRepository.QueryOrganzitionInfoByAccountID(Account_uid);
            List<int> OPType_OrganizationUID = new List<int>();
            foreach(OrganiztionVM temp in tempResult)
            {
                OPType_OrganizationUID.Add(int.Parse(temp.OPType_OrganizationUID.ToString()));
            }

            var ProjectListDatas = flowChartMasterRepository.QueryProjectList(Account_uid, OPType_OrganizationUID, MHFlag_MulitProject);
            return new PagedListModel<PrjectListVM>(ProjectListDatas.Count(), ProjectListDatas);
        }

        public ProcessDataSearch QueryFlowChartDataByMasterUid(int flowChartMaster_uid)
        {

            var tempDataList = flowChartMasterRepository.QueryFlowChartDataByMasterUid(flowChartMaster_uid);
            ProcessDataSearch result = new ProcessDataSearch();
            foreach (var item in tempDataList.Distinct())
            {
                result.Customer = item.Customer;
                result.Part_Types = item.Part_Types;
                result.Product_Phase = item.Product_Phase;
                result.Project = item.Project;
                result.Func_Plant = item.Func_Plant;
            }
            return result;
        }

        public List<string> QueryPlantByUser (int userid)
        {
            return flowChartDetailRepository.QueryPlantByUser(userid).ToList();
        }

        public List<FlowChartMasterDTO> QueryProjectTypes(string project)
        {
            var EnumEntity = flowChartMasterRepository.QueryProjectTypes(project);
            var result = AutoMapper.Mapper.Map<List<FlowChartMasterDTO>>(EnumEntity);
            return result;
        }

        public List<string> QueryProcess(int flowchartmasterUid)
        {
            return flowChartDetailRepository.QueryProcess(flowchartmasterUid).ToList();
        }

        IEPlanManagerVM IFlowChartPlanService.QueryProcessIEMGDataSingle(int uid, DateTime date, int shiftTimeId)
        {
            return flowChartMasterRepository.QueryFlowIEMGDataSingle(uid, date, shiftTimeId);
        }

        //public string FlowIEChartPlan(IEPlanManagerVM ent)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
