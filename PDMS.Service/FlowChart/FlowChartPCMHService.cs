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
    public interface IFlowChartPCMHService
    {
        FlowChartDetailGetByMasterInfo QueryBindBomByMasterId(int id);
        PagedListModel<FlowChartBomGet> QueryBomByFlowChartUID(int id, int Version, List<int> plants);
        FlowChartBomGet QueryBomEditByFlowChartUID(int PC_MH_UID);
        List<SystemUserDTO> QueryBomUserInfo();
        List<FlowChartPCMHRelationshipDTO> GetALlPCMH();
        int CheckBomUser(GetFuncPlantProcessSearch search);
        string InsertBomUserInfo(List<FlowChartPCMHRelationshipVM> list);
        string EditFLPCBomInfo(FlowChartBomGet bomItem, int CurrUser);
        void DeleteBomInfoByUIDList(List<int> IdList);
        List<FlowChartPCMHRelationshipDTO> QueryFLBomDataList(List<int> flDetailUIDList);

    }


    public class FlowChartPCMHService : IFlowChartPCMHService
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

        #endregion //Private interfaces properties


        #region Service constructor
        public FlowChartPCMHService(
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
        }
        #endregion //Service constructor


        public FlowChartDetailGetByMasterInfo QueryBindBomByMasterId(int id)
        {
            var masterItem = flowChartMasterRepository.GetById(id);

            FlowChartDetailGetByMasterInfo detailInfo = new FlowChartDetailGetByMasterInfo();
            detailInfo.BU_D_Name = masterItem.System_Project.System_BU_D.BU_D_Name;
            detailInfo.Project_Name = masterItem.System_Project.Project_Name;
            detailInfo.Part_Types = masterItem.Part_Types;
            detailInfo.Product_Phase = masterItem.System_Project.Product_Phase;

            return detailInfo;
        }

        public PagedListModel<FlowChartBomGet> QueryBomByFlowChartUID(int id, int version, List<int> plants)
        {
            List<FlowChartBomGet> list = new List<FlowChartBomGet>();
            var idList = systemFunctionPlantRepository.GetMany(m => plants.Contains(m.FunPlant_OrganizationUID.Value)).Select(m => m.System_FunPlant_UID).ToList();
            var detailList = flowChartDetailRepository.QueryBomByFlowChartUID(id, version, idList).ToList();

            return new PagedListModel<FlowChartBomGet>(0, detailList);
        }

        public FlowChartBomGet QueryBomEditByFlowChartUID(int PC_MH_UID)
        {

            var mhItem = flowChartPCMHRelationshipRepository.GetById(PC_MH_UID);
            var detailItem = flowChartDetailRepository.GetById(mhItem.FlowChart_Detail_UID);
            FlowChartBomGet bomItem = new FlowChartBomGet();
            bomItem.FlowChart_Detail_UID = detailItem.FlowChart_Detail_UID;
            bomItem.PC_MH_UID = PC_MH_UID;
            bomItem.Process_Seq = detailItem.Process_Seq;
            bomItem.Process = detailItem.Process;
            bomItem.Place = mhItem.Place;
            bomItem.Color = detailItem.Color;
            bomItem.User_NTID = mhItem.System_Users1.User_NTID;
            return bomItem;
        }

        public List<SystemUserDTO> QueryBomUserInfo()
        {
            //获取所有用户信息，还要判断用户是否是MHFLAG
            var list = systemUserRepository.GetAll().ToList();
            var dto = AutoMapper.Mapper.Map<List<SystemUserDTO>>(list);
            return dto;
        }

        public List<FlowChartPCMHRelationshipDTO> GetALlPCMH()
        {
            var list = flowChartPCMHRelationshipRepository.GetAll().ToList();
            var dtoList = AutoMapper.Mapper.Map<List<FlowChartPCMHRelationshipDTO>>(list);
            return dtoList;
        }

        public int CheckBomUser(GetFuncPlantProcessSearch search)
        {
            var count = flowChartDetailRepository.CheckBomUser(search);
            return count;
        }

        public string InsertBomUserInfo(List<FlowChartPCMHRelationshipVM> list)
        {
            if (list.Count() == 0)
            {
                return string.Empty;
            }
            //var MHRoleUid =
            //    systemRoleRepository.GetMany(m => m.Role_Name == "维护角色").Select(m => m.Role_UID).FirstOrDefault();
            var ErrorInfo = "";
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var languageUID = list.First().FlowchartPCDTOItem.System_Language_UID;
                    foreach (var item in list)
                    {
                        var flowchartPCItem = AutoMapper.Mapper.Map<FlowChart_PC_MH_Relationship>(item.FlowchartPCDTOItem);

                        var hasExistUserItem = systemUserRepository.GetMany(m => m.User_NTID.ToUpper().Equals(item.UserNTID.ToUpper())).FirstOrDefault();
                        if (hasExistUserItem == null)
                        {
                            int maxAccountID = flowChartPCMHRelationshipRepository.InsertBomUserInfo(list);

                            System_Users userItem = new System_Users();
                            userItem.Account_UID = maxAccountID + 1;
                            userItem.User_NTID = item.UserNTID;
                            userItem.User_Name = item.UserNTID;
                            userItem.Enable_Flag = true;
                            userItem.Email = "";
                            userItem.System_Language_UID = languageUID;
                            userItem.Modified_UID = item.FlowchartPCDTOItem.Modified_UID;
                            userItem.Modified_Date = item.FlowchartPCDTOItem.Modified_Date;
                            userItem.MH_Flag = true;
                            userItem.EmployeePassword = JGP.Common.PasswordUtil.EncryptionHelper.Encrypt("123456","");
                            systemUserRepository.Add(userItem);

                            userItem.FlowChart_PC_MH_Relationship1.Add(flowchartPCItem);

                            //SystemUserRoleDTO userRoleDTO = new SystemUserRoleDTO();
                            //userRoleDTO.Account_UID = userDTO.Account_UID;
                            //userRoleDTO.Modified_UID = this.CurrentUser.AccountUId;
                            //userRoleDTO.Modified_Date = userDTO.Modified_Date;

                        }
                        else
                        {
                            hasExistUserItem.FlowChart_PC_MH_Relationship1.Add(flowchartPCItem);
                        }
                        //数据提交后才能查找的到值，才能判断是否重复UserNTID
                        unitOfWork.Commit();

                    }

                    scope.Complete();
                }

            }
            catch (Exception e)
            {

                ErrorInfo = "插入数据失败！" + e.ToString();
            }
            return ErrorInfo;
        }

        public string EditFLPCBomInfo(FlowChartBomGet bomItem, int currUser)
        {
            string result = string.Empty;
            FlowChart_PC_MH_Relationship pcItem = new FlowChart_PC_MH_Relationship();
            pcItem.FlowChart_Detail_UID = bomItem.FlowChart_Detail_UID;
            pcItem.Place = bomItem.Place;
            pcItem.Modified_UID = currUser;
            pcItem.Modified_Date = DateTime.Now;

            //检查用户表中是否存在
            try
            {
                using (var trans = new TransactionScope())
                {
                    //删除以前的那条数据
                    var deleteItem = flowChartPCMHRelationshipRepository.GetById(bomItem.PC_MH_UID);
                    flowChartPCMHRelationshipRepository.Delete(deleteItem);

                    var hasUserItem = systemUserRepository.GetMany(m => m.User_NTID.ToUpper().Equals(bomItem.User_NTID.ToUpper())).FirstOrDefault();
                    if (hasUserItem != null)
                    {
                        if (!hasUserItem.MH_Flag)
                        {
                            result = "该账号已经存在，并且不是物料员所属的帐号";
                            return result;
                        }
                        else //用户表已经存在，直接新增该账户
                        {
                            pcItem.MH_UID = hasUserItem.Account_UID;
                            flowChartPCMHRelationshipRepository.Add(pcItem);
                        }
                    }
                    else //新增用户表和关系表
                    {
                        //获取maxAccountUID
                        int maxAccountID = flowChartPCMHRelationshipRepository.InsertBomUserInfo(null);

                        System_Users newUserItem = new System_Users();
                        newUserItem.Account_UID = maxAccountID + 1;
                        newUserItem.User_NTID = bomItem.User_NTID;
                        newUserItem.User_Name = bomItem.User_NTID;
                        newUserItem.System_Language_UID = StructConstants.SystemLanguageUID.Chinese_CN;
                        newUserItem.Enable_Flag = true;
                        newUserItem.Email = "";
                        newUserItem.Modified_UID = currUser;
                        newUserItem.Modified_Date = pcItem.Modified_Date;
                        newUserItem.MH_Flag = true;
                        newUserItem.EmployeePassword = JGP.Common.PasswordUtil.EncryptionHelper.Encrypt("123456", "");
                        systemUserRepository.Add(newUserItem);
                        //新增关系表
                        newUserItem.FlowChart_PC_MH_Relationship1.Add(pcItem);

                    }


                    unitOfWork.Commit();
                    trans.Complete();
                }
            }
            catch (Exception ex)
            {
                result = "数据修改错误";
            }

            return result;
        }

        public void DeleteBomInfoByUIDList(List<int> IdList)
        {
            var list = flowChartPCMHRelationshipRepository.GetMany(m => IdList.Contains(m.PC_MH_UID)).ToList();
            flowChartPCMHRelationshipRepository.DeleteList(list);
            unitOfWork.Commit();
        }

        public List<FlowChartPCMHRelationshipDTO> QueryFLBomDataList(List<int> flDetailUIDList)
        {
            var list = flowChartPCMHRelationshipRepository.GetMany(m => flDetailUIDList.Contains(m.FlowChart_Detail_UID)).ToList();
            var dto = AutoMapper.Mapper.Map<List<FlowChartPCMHRelationshipDTO>>(list);
            return dto;
        }



    }

}
