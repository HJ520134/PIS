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
    public interface IFlowChartDetailService
    {
        PagedListModel<FlowChartDetailGet> QueryFLDetailList(int id, int Version);
        PagedListModel<FlowChartDetailGet> QueryFLDetailWUXI_MList(int id, int Version);

        FlowChartDetailGet QueryFLDetailByID(int id);
        string SaveFLDetailInfo(FlowChartDetailDTO dto, int AccountID);
        void SaveAllDetailInfo(List<FlowChartDetailAndMGDataInputDTO> dto, int AccountID);

        List<FlowChartBomGet> QueryFLDetailByUIDAndVersion(int MasterUID, int Version, List<int> idList);

        List<FlowChartDetailDTO> GetMaxDetailInfoAPI(int UID);
        List<FlowChartDetailDTO> QueryDetailList(int id, int Version);

        int GetFlowChart_DetailByID(int flowChartMasterID, int Binding_Seq,string color);

        int GetFlowChart_DetailByID(int flowChartMasterID, int Binding_Seq);
    }

    public class FlowChartDetailService : IFlowChartDetailService
    {
        #region Private interfaces properties
        private readonly IUnitOfWork unitOfWork;
        private readonly IFlowChartMasterRepository flowChartMasterRepository;
        private readonly IFlowChartDetailRepository flowChartDetailRepository;
        private readonly IFlowChartMgDataRepository flowChartMgDataRepository;
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

        private readonly IWIPChangeHistoryRepository _wipChangeHistoryRepository;
        #endregion //Private interfaces properties
        public delegate string CallSaveFL(FlowChartDetailDTO dto, int AccountID);



        #region Service constructor
        public FlowChartDetailService(
            IFlowChartMasterRepository flowChartMasterRepository,
            IFlowChartDetailRepository flowChartDetailRepository,
            IFlowChartMgDataRepository flowChartMgDataRepository,
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
            IWIPChangeHistoryRepository wipChangeHistoryRepository,
        IUnitOfWork unitOfWork)
        {
            this.systemRoleRepository = systemRoleRepository;
            this.unitOfWork = unitOfWork;
            this.flowChartMasterRepository = flowChartMasterRepository;
            this.flowChartDetailRepository = flowChartDetailRepository;
            this.flowChartMgDataRepository = flowChartMgDataRepository;
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
            this._wipChangeHistoryRepository = wipChangeHistoryRepository;
        }
        #endregion //Service constructor

        private FlowChartGet SetAutoMapFlChart(FlowChart_Master item)
        {
            FlowChartGet model = new FlowChartGet();
            model.FlowChartMasterDTO = AutoMapper.Mapper.Map<FlowChartMasterDTO>(item);
            model.SystemProjectDTO = AutoMapper.Mapper.Map<SystemProjectDTO>(item.System_Project);
            model.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(item.System_Users);
            model.BU_D_Name = item.System_Project.System_BU_D.BU_D_Name;
            return model;
        }

       public int GetFlowChart_DetailByID(int flowChartMasterID, int Binding_Seq, string color)
        {
            return flowChartDetailRepository.GetFlowChart_DetailByID(flowChartMasterID, Binding_Seq,color);
        }

        public int GetFlowChart_DetailByID(int flowChartMasterID, int Binding_Seq)
        {
            return flowChartDetailRepository.GetFlowChart_DetailByID(flowChartMasterID, Binding_Seq);
        }

        public PagedListModel<FlowChartDetailGet> QueryFLDetailList(int id, int Version)
        {
            var totalCount = 0;
            var masterItem = flowChartMasterRepository.GetById(id);
            if (masterItem != null)
            {
                IList<FlowChartDetailGet> importList = new List<FlowChartDetailGet>();

                var flChartList = flowChartMasterRepository.QueryFLDetailList(id, Version, out totalCount);
                //importItem.FlowChartMasterDTO = AutoMapper.Mapper.Map<FlowChartMasterDTO>(masterItem);
                foreach (var flChartItem in flChartList)
                {
                    FlowChartDetailGet importDetailItem = new FlowChartDetailGet();
                    importDetailItem.FlowChartDetailDTO = AutoMapper.Mapper.Map<FlowChartDetailDTO>(flChartItem);
                    if (flChartItem.FatherProcess_UID != null)
                    {
                        var process = flChartList.Where(m => m.FlowChart_Detail_UID == flChartItem.FatherProcess_UID).Select(m => m.Process).First();
                        importDetailItem.FlowChartDetailDTO.FatherProcess = process;
                    }

                    switch (flChartItem.IsQAProcess)
                    {
                        case StructConstants.IsQAProcessType.InspectKey: //IPQC全检
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.InspectText;
                            break;
                        case StructConstants.IsQAProcessType.PollingKey: //IPQC巡检
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.PollingText;
                            break;
                        case StructConstants.IsQAProcessType.InspectOQCKey: //OQC检测
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.InspectOQCText;
                            break;
                        case StructConstants.IsQAProcessType.InspectAssembleKey: //组装检测
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.InspectAssembleText;
                            break;
                        case StructConstants.IsQAProcessType.AssembleOQCKey: //组装&OQC检测
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.AssembleOQCText;
                            break;
                        default:
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = string.Empty;
                            break;
                    }

                    //importDetailItem.FlowChartMgDataDTO = AutoMapper.Mapper.Map<FlowChartMgDataDTO>(flChartItem.FlowChart_MgData.FirstOrDefault());
                    importDetailItem.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(flChartItem.System_Users);
                    importDetailItem.FunPlant = flChartItem.System_Function_Plant.FunPlant;
                    importList.Add(importDetailItem);
                }

                return new PagedListModel<FlowChartDetailGet>(0, importList);
            }
            else
            {
                return null;
            }
        }

        public PagedListModel<FlowChartDetailGet> QueryFLDetailWUXI_MList(int id, int Version)
        {
            var totalCount = 0;
            var masterItem = flowChartMasterRepository.GetById(id);
            if (masterItem != null)
            {
                IList<FlowChartDetailGet> importList = new List<FlowChartDetailGet>();

                var flChartList = flowChartMasterRepository.QueryFLDetailWUXI_MList(id, Version, out totalCount);
                //importItem.FlowChartMasterDTO = AutoMapper.Mapper.Map<FlowChartMasterDTO>(masterItem);
                foreach (var flChartItem in flChartList)
                {
                    FlowChartDetailGet importDetailItem = new FlowChartDetailGet();
                    importDetailItem.FlowChartDetailDTO = AutoMapper.Mapper.Map<FlowChartDetailDTO>(flChartItem);
                    //没合并前注释掉
                    //importDetailItem.FlowChartDetailDTO.Process_Seq = flChartItem.ItemNo;
                    if (flChartItem.FatherProcess_UID != null)
                    {
                        var process = flChartList.Where(m => m.FlowChart_Detail_UID == flChartItem.FatherProcess_UID).Select(m => m.Process).First();
                        importDetailItem.FlowChartDetailDTO.FatherProcess = process;
                    }

                    switch (flChartItem.IsQAProcess)
                    {
                        case StructConstants.IsQAProcessType.InspectKey: //IPQC全检
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.InspectText;
                            break;
                        case StructConstants.IsQAProcessType.PollingKey: //IPQC巡检
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.PollingText;
                            break;
                        case StructConstants.IsQAProcessType.InspectOQCKey: //OQC检测
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.InspectOQCText;
                            break;
                        case StructConstants.IsQAProcessType.InspectAssembleKey: //组装检测
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.InspectAssembleText;
                            break;
                        case StructConstants.IsQAProcessType.AssembleOQCKey: //组装&OQC检测
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.AssembleOQCText;
                            break;
                        default:
                            importDetailItem.FlowChartDetailDTO.IsQAProcessName = string.Empty;
                            break;
                    }

                    //importDetailItem.FlowChartMgDataDTO = AutoMapper.Mapper.Map<FlowChartMgDataDTO>(flChartItem.FlowChart_MgData.FirstOrDefault());
                    importDetailItem.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(flChartItem.System_Users);
                    importDetailItem.FunPlant = flChartItem.System_Function_Plant.FunPlant;
                    importList.Add(importDetailItem);
                }

                return new PagedListModel<FlowChartDetailGet>(0, importList);
            }
            else
            {
                return null;
            }
        }





        public FlowChartDetailGet QueryFLDetailByID(int id)
        {
            FlowChartDetailGet detailItem = new FlowChartDetailGet();
            var item = flowChartDetailRepository.GetById(id);
            detailItem.FlowChartDetailDTO = AutoMapper.Mapper.Map<FlowChartDetailDTO>(item);
            //获取父节点制程信息
            Dictionary<int, string> FatherProcessDict = new Dictionary<int, string>();
            //去掉重复的process后的制程信息
            Dictionary<int, string> DistinctFatherProcessDict = new Dictionary<int, string>();
            //var fatherProcessItem = flowChartDetailRepository.GetById(detailItem.FlowChartDetailDTO.FatherProcess);
            switch (item.Process_Seq)
            {
                case 1:
                    //取后面4个Seq
                    var seqListOne = new List<int> { 2, 3, 4, 5 };
                    var afterSeqList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == item.FlowChart_Version
                    && seqListOne.Contains(m.Process_Seq))
                        .Select(m => new { m.FlowChart_Detail_UID, m.Process }).ToList();
                    foreach (var afterSeqItem in afterSeqList)
                    {
                        FatherProcessDict.Add(afterSeqItem.FlowChart_Detail_UID, afterSeqItem.Process);
                    }
                    break;
                //取Seq=1和Seq=3，4
                case 2:
                    var seqListTwo = new List<int> { 1, 3, 4, 5 };
                    var afterSeqTwoList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == item.FlowChart_Version
                    && seqListTwo.Contains(m.Process_Seq))
                        .Select(m => new { m.FlowChart_Detail_UID, m.Process }).ToList();
                    foreach (var afterSeqItem in afterSeqTwoList)
                    {
                        FatherProcessDict.Add(afterSeqItem.FlowChart_Detail_UID, afterSeqItem.Process);
                    }
                    break;
                //取前两个Seq和后两个Seq
                default:
                    var frontOne = item.Process_Seq - 1;
                    var frontTwo = item.Process_Seq - 2;
                    var behindOne = item.Process_Seq + 1;
                    var behindTwo = item.Process_Seq + 2;
                    var seqListThree = new List<int> { frontOne, frontTwo, behindOne, behindTwo };
                    var seqThreeList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == item.FlowChart_Version
                    && seqListThree.Contains(m.Process_Seq))
                        .Select(m => new { m.FlowChart_Detail_UID, m.Process }).ToList();
                    foreach (var afterSeqItem in seqThreeList)
                    {
                        FatherProcessDict.Add(afterSeqItem.FlowChart_Detail_UID, afterSeqItem.Process);
                    }
                    break;
            }
            var groupByValues = FatherProcessDict.GroupBy(m => m.Value);
            foreach (var groupItem in groupByValues)
            {
                var hasItem = FatherProcessDict.Where(m => m.Value == groupItem.Key).FirstOrDefault();
                DistinctFatherProcessDict.Add(hasItem.Key, hasItem.Value);
            }
            detailItem.FatherProcess = DistinctFatherProcessDict;
            detailItem.FunPlant = item.System_Function_Plant.FunPlant;
            detailItem.Organization_UID = item.System_Function_Plant.System_FunPlant_UID;
            return detailItem;
        }


        public string SaveFLDetailInfo(FlowChartDetailDTO dto, int AccountID)
        {
            string errorInfo = string.Empty;
            //switch (site)
            //{
            //    case StructConstants.Site.CTU:
            //        errorInfo = CallSaveFLDetailInfo(SaveFLDetailCTUInfo, dto, AccountID);
            //        break;
            //    case StructConstants.Site.WUXI_M:
            //        errorInfo = CallSaveFLDetailInfo(SaveFLDetailWUXI_MInfo, dto, AccountID);
            //        break;
            //}

            //
            try
            {
                AddWIPChangeHistory(dto, AccountID);
            }
            catch (Exception)
            {
            }

            errorInfo = CallSaveFLDetailInfo(SaveFLDetailCTUInfo, dto, AccountID);
            return errorInfo;
        }

        /// <summary>
        /// 添加Wip的修改记录
        /// </summary>
        /// <returns></returns>
        private void AddWIPChangeHistory(FlowChartDetailDTO dto, int Modified_UID)
        {
            //查询
            var OriWIPValue = flowChartDetailRepository.GetWIPValueByFLID(dto.FlowChart_Detail_UID);
            WIP_Change_History WIPHistoryModel = new WIP_Change_History();
            WIPHistoryModel.Change_Type = "PPCheck";
            WIPHistoryModel.Comment = "FlowChart_Detial";
            WIPHistoryModel.FlowChart_Detail_UID = dto.FlowChart_Detail_UID;
            WIPHistoryModel.Modified_Date = DateTime.Now;
            WIPHistoryModel.Product_UID = 0;
            WIPHistoryModel.Modified_UID = Modified_UID;
            WIPHistoryModel.WIP_Old = OriWIPValue;
            WIPHistoryModel.WIP_Add = dto.WIP_QTY - OriWIPValue;
            if (WIPHistoryModel.WIP_Add != 0)
            {
                _wipChangeHistoryRepository.Add(WIPHistoryModel);
            }
            unitOfWork.Commit();
        }

        public string CallSaveFLDetailInfo(CallSaveFL call, FlowChartDetailDTO dto, int AccountID)
        {
            return call(dto, AccountID);
        }


        private string SaveFLDetailCTUInfo(FlowChartDetailDTO dto, int AccountID)
        {
            string errorInfo = "";
            List<FlowChart_Detail> detailLit = new List<FlowChart_Detail>();
            var item = flowChartDetailRepository.GetById(dto.FlowChart_Detail_UID);

            item.DRI = dto.DRI;
            item.Place = dto.Place;
            item.Location_Flag = dto.Location_Flag;
            item.System_FunPlant_UID = dto.System_FunPlant_UID;
            item.Process_Desc = dto.Process_Desc;
            item.Modified_UID = AccountID;
            item.Modified_Date = DateTime.Now;
            item.FatherProcess_UID = dto.FatherProcess_UID;
            item.WIP_QTY = dto.WIP_QTY;
            item.Current_WH_QTY = dto.Current_WH_QTY;
            item.NullWip = dto.NullWip;
            item.Data_Source = dto.Data_Source;
            item.Is_Synchronous = dto.Is_Synchronous;
            //获取相同制程的信息
            detailLit = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == item.FlowChart_Version
            && m.Process_Seq == item.Process_Seq).ToList();

            //设置QA站点--------------------Sidney
            if (dto.IsQAProcess == "NULL")
            {
                dto.IsQAProcess = "";
            }
            if (item.IsQAProcess != dto.IsQAProcess)
            {
                foreach (var isSameProcessItem in detailLit)
                {
                    isSameProcessItem.IsQAProcess = dto.IsQAProcess;
                }
            }

            //更新Rework或Repair
            if (dto.Rework_Flag == "NULL")
            {
                dto.Rework_Flag = null;
            }
            if (item.Rework_Flag != dto.Rework_Flag || (dto.Rework_Flag == "Rework" && item.RelatedRepairUID != dto.RelatedRepairUID))
            {
                if (dto.Rework_Flag == null)
                {
                    item.Rework_Flag = string.Empty;
                    item.RelatedRepairUID = null;
                }
                else
                {
                    switch (dto.Rework_Flag.ToLower())
                    {
                        case "rework":
                            //若RelatedRepairUID 没值，不保存
                            //Modified By Rock 2017-10-16 修改RelatedRepairUID为String类型
                            if (!string.IsNullOrEmpty(dto.RelatedRepairUID))
                            {
                                item.Rework_Flag = dto.Rework_Flag;
                                item.RelatedRepairUID = dto.RelatedRepairUID;
                            }

                            break;
                        case "repair":
                            item.Rework_Flag = "Repair";
                            item.RelatedRepairUID = null;
                            break;
                        default:
                            break;
                    }
                }
            }
            unitOfWork.Commit();
            return errorInfo;
        }

        private string SaveFLDetailWUXI_MInfo(FlowChartDetailDTO dto, int AccountID)
        {
            string errorInfo = "";
            List<FlowChart_Detail> detailLit = new List<FlowChart_Detail>();
            var item = flowChartDetailRepository.GetById(dto.FlowChart_Detail_UID);

            item.DRI = dto.DRI;
            item.Place = dto.Place;
            item.System_FunPlant_UID = dto.System_FunPlant_UID;
            item.Process_Desc = dto.Process_Desc;
            item.Modified_UID = AccountID;
            item.Modified_Date = DateTime.Now;
            item.FatherProcess_UID = dto.FatherProcess_UID;

            //获取相同制程的信息
            detailLit = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == item.FlowChart_Version
            //没合并前注释
            //&& m.ItemNo == item.ItemNo
            ).ToList();

            //设置QA站点--------------------Sidney
            if (dto.IsQAProcess == "NULL")
            {
                dto.IsQAProcess = "";
            }
            if (item.IsQAProcess != dto.IsQAProcess)
            {
                foreach (var isSameProcessItem in detailLit)
                {
                    isSameProcessItem.IsQAProcess = dto.IsQAProcess;
                }
            }

            //更新Rework或Repair
            if (dto.Rework_Flag == "NULL")
            {
                dto.Rework_Flag = null;
            }
            if (item.Rework_Flag != dto.Rework_Flag)
            {
                //检查是否已经存在Rework_Flag==Repair，是则不能更新

                var masterVersion = flowChartMasterRepository.GetById(item.FlowChart_Master_UID).FlowChart_Version;
                var isExsitRepair = flowChartDetailRepository.GetMany(m => m.Rework_Flag == "Repair" &&
                //没合并前注释
                //m.ItemNo != item.ItemNo && 
                m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == masterVersion).Count();

                if (isExsitRepair == 0 || dto.Rework_Flag != "Repair")
                {
                    foreach (var r in detailLit)
                    {
                        r.Rework_Flag = dto.Rework_Flag;
                    }
                }
                else
                {
                    errorInfo = "已经存在修复站点，不能多余1个修复站点";
                }
            }
            unitOfWork.Commit();
            return errorInfo;
        }

        public void SaveAllDetailInfo(List<FlowChartDetailAndMGDataInputDTO> list, int AccountID)
        {
            var idList = list.Select(m => m.FlowChart_Detail_UID).ToList();
            var mgDataList = flowChartMgDataRepository.GetMany(m => idList.Contains(m.FlowChart_Detail_UID)).ToList();

            foreach (var item in list)
            {
                var mgDataItem = mgDataList.Where(m => m.FlowChart_Detail_UID == item.FlowChart_Detail_UID).FirstOrDefault();
                if (mgDataItem != null)
                {
                    mgDataItem.Product_Plan = item.Product_Plan;
                    mgDataItem.Target_Yield = double.Parse(item.Target_Yield.Substring(0, item.Target_Yield.Length - 1)) / 100;
                    mgDataItem.Modified_Date = DateTime.Now;
                    mgDataItem.Modified_UID = AccountID;
                    flowChartMgDataRepository.Update(mgDataItem);
                }
            }
            unitOfWork.Commit();
        }



        public List<FlowChartBomGet> QueryFLDetailByUIDAndVersion(int MasterUID, int Version, List<int> idList)
        {
            //var list = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == MasterUID && m.FlowChart_Version == Version
            //&& idList.Contains(m.System_FunPlant_UID));
            //var dto = AutoMapper.Mapper.Map<List<FlowChartDetailDTO>>(list);
            //return dto;
            var list = flowChartDetailRepository.QueryFLDetailByUIDAndVersion(MasterUID, Version, idList);
            return list;
        }



        public List<FlowChartDetailDTO> GetMaxDetailInfoAPI(int UID)
        {
            var flowchartMaster = flowChartMasterRepository.GetById(UID);
            var detailList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == UID && m.FlowChart_Version == flowchartMaster.FlowChart_Version).ToList();
            var list = AutoMapper.Mapper.Map<List<FlowChartDetailDTO>>(detailList);
            return list;
        }

        public List<FlowChartDetailDTO> QueryDetailList(int id, int Version)
        {
            var detailList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == id && m.FlowChart_Version == Version).ToList();
            var list = AutoMapper.Mapper.Map<List<FlowChartDetailDTO>>(detailList);
            return list;
        }

    }


}
