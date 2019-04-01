using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using PDMS.Data;
using PDMS.Model.ViewModels.Common;
using PDMS.Model.ViewModels;
using AutoMapper.Internal;
using System.Configuration;
using System.Collections;
using static PDMS.Common.Constants.StructConstants;

namespace PDMS.Service
{
    public interface ICommonService
    {
        #region User Part
        SystemUserDTO GetSystemUserByUId(int uid);
        SystemUserDTO GetSystemUserByNTId(string ntid,int islogin);
        #endregion //User Part
        bool CheckPISMUser(string EmailAddr);
        int GetProcessPlan(int projectUID, string Process, DateTime date);
        IEnumerable<SystemRoleDTO> GetAllRoles();
        IEnumerable<SystemPlantDTO> GetValidPlantsByUserUId(int accountUId);
        IEnumerable<SystemBUMDTO> GetValidBUMsByUserUId(int accountUId);
        IEnumerable<SystemBUDDTO> GetValidBUDsByUserUId(int accountUId);
        IEnumerable<SystemOrgDTO> GetValidOrgsByUserUId(int accountUId);
        //数据权限------Sidney 2016-4-26
        CurrentUserDataPermission GetPermissonsByNTId(int accountUId);
        List<string> GetOptysByNTId(int accountUId);
        //不同的专案/OP类型查看到不同数据--------------Sidney 2016-5-9
        List<string> GetFiltProject(List<string> CurrentOP, List<int> CurrentProject);
        //UserOrgInfo GetUserOrgInfo(int accountUid);
        int GetUserUIDByEmail(string Email);
        CustomUserInfoVM GetUserInfo(int uid);
        string GetProjectSite(int uid);
        List<string> GetUserBu(int OrgUID);
        //获取用户组织架构
        List<OrganiztionVM> GetOrgInfoByUserUId(int Account_UId);
        void ChangeLanguageInfo(int accountId, int languid);
        int GetSelectLanguageId(int accountId);
    }

    public class CommonService : ICommonService
    {
        #region Private interfaces properties
        private readonly IUnitOfWork unitOfWork;
        private readonly ISystemUserRepository systemUserRepository;
        private readonly ISystemRoleRepository systemRoleRepository;
        private readonly ISystemPlantRepository systemPlantRepository;
        private readonly ISystemOrgRepository systemOrgRepository;
        private readonly ISystemBUMRepository systemBUMRepository;
        private readonly ISystemBUDRepository systemBUDRepository;
        private readonly IFlowChartPCMHRelationshipRepository flowChartPCMHRelationshipRepository;
        private readonly ISystemOrgBomRepository systemOrgBomRepository;
        private readonly IProjectUsersGroupRepository projectUsersGroupRepository;
        private readonly ISystemUserOrgRepository systemUserOrgRepository;
        private readonly ISystemUserBusinessGroupRepository systemUserBusinessGroupRepository;
        private readonly ISystemProjectRepository systemProjectRepository;
        private readonly IFlowChartMasterRepository flowChartMasterRepository;
        private readonly IFlowChartMgDataRepository flowChartMgDataRepository;
        private readonly ISystemLocaleStringResourceRepository systemLocaleStringResourceRepository;
        #endregion //Private interfaces properties

        #region Service constructor
        public CommonService(
            ISystemUserRepository systemUserRepository,
            ISystemRoleRepository systemRoleRepository,
            ISystemPlantRepository systemPlantRepository,
            ISystemOrgRepository systemOrgRepository,
            ISystemBUMRepository systemBUMRepository,
            ISystemBUDRepository systemBUDRepository,
            IFlowChartPCMHRelationshipRepository flowChartPCMHRelationshipRepository,
            ISystemOrgBomRepository systemOrgBomRepository,
            IProjectUsersGroupRepository projectUsersGroupRepository,
            ISystemUserOrgRepository systemUserOrgRepository,
            ISystemUserBusinessGroupRepository systemUserBusinessGroupRepository,
            ISystemProjectRepository systemProjectRepository,
            IFlowChartMasterRepository flowChartMasterRepository,
            IFlowChartMgDataRepository flowChartMgDataRepository,
        ISystemLocaleStringResourceRepository systemLocaleStringResourceRepository,
            IUnitOfWork unitOfWork)
        {
            this.systemUserRepository = systemUserRepository;
            this.systemRoleRepository = systemRoleRepository;
            this.systemPlantRepository = systemPlantRepository;
            this.systemOrgRepository = systemOrgRepository;
            this.systemBUMRepository = systemBUMRepository;
            this.systemBUDRepository = systemBUDRepository;
            this.flowChartPCMHRelationshipRepository = flowChartPCMHRelationshipRepository;
            this.systemOrgBomRepository = systemOrgBomRepository;
            this.projectUsersGroupRepository = projectUsersGroupRepository;
            this.systemUserOrgRepository = systemUserOrgRepository;
            this.systemUserBusinessGroupRepository = systemUserBusinessGroupRepository;
            this.systemProjectRepository = systemProjectRepository;
            this.flowChartMasterRepository = flowChartMasterRepository;
            this.systemLocaleStringResourceRepository = systemLocaleStringResourceRepository;
            this.flowChartMgDataRepository = flowChartMgDataRepository;
            this.unitOfWork = unitOfWork;
        }
        #endregion //Service constructor

        #region User Part
        public SystemUserDTO GetSystemUserByUId(int uid)
        {
            var query = systemUserRepository.GetById(uid);
            SystemUserDTO returnUser = (query == null ? null : AutoMapper.Mapper.Map<SystemUserDTO>(query));
            if (returnUser != null && query.System_User_Role.Count > 0)
            {
                var roleList = query.System_User_Role.Select(x => x.System_Role).ToList();
                returnUser.RoleList = AutoMapper.Mapper.Map<List<SystemRoleDTO>>(roleList);
            }
            return returnUser;
        }
        public int GetUserUIDByEmail(string Email)
        {
            //通过Email获取
            return systemUserRepository.GetUserUIDByEmail(Email);
        }
        public bool CheckPISMUser(string EmailAddr)
        {
            return systemUserRepository.CheckPISMUser(EmailAddr);
        }

        public SystemUserDTO GetSystemUserByNTId(string ntid,int islogin)
        {
            var query = systemUserRepository.GetFirstOrDefault(q => q.User_NTID == ntid);
            SystemUserDTO returnUser = (query == null ? null : AutoMapper.Mapper.Map<SystemUserDTO>(query));
            //获取相关参数IsMulitProject、flowChartMaster_Uid
            if (returnUser != null)
            {
                //获取物料员的专案信息
                var flIdAndVersionList = flowChartPCMHRelationshipRepository.GetMany(m => m.MH_UID == returnUser.Account_UID && !m.FlowChart_Detail.FlowChart_Master.Is_Closed)
                    .Select(m => new { m.FlowChart_Detail.FlowChart_Master_UID, m.FlowChart_Detail.FlowChart_Version }).Distinct().ToList();

                //获取所有专案信息
                var flMasterList = flowChartMasterRepository.GetAll().ToList();

                //将两边数据进行匹配，判断该用户是否有多个专案
                int count = 0;
                foreach (var item in flIdAndVersionList)
                {
                    //只有最大版本的数据才是正确的
                    var hasExist = flMasterList.Exists(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == item.FlowChart_Version);
                    if (hasExist)
                    {
                        //跳转到ProductInput会用到
                        returnUser.flowChartMaster_Uid = item.FlowChart_Master_UID;
                        count++;
                    }
                }
                if (count > 1)
                {
                    returnUser.IsMulitProject = true;
                }
                else
                {
                    returnUser.IsMulitProject = false;
                }

                if (query.System_User_Role.Count > 0)
                {
                    var roleList = query.System_User_Role.Select(x => x.System_Role).ToList();
                    returnUser.RoleList = AutoMapper.Mapper.Map<List<SystemRoleDTO>>(roleList);
                }
            }
            else
            {
                if (islogin == 1)
                {
                    try
                    {
                        int test = Convert.ToInt32(ntid);
                        var eqpuserlist = flowChartMasterRepository.GetByUserId(ntid);
                        if (eqpuserlist.Count > 0)
                        {
                            returnUser = new SystemUserDTO();
                            returnUser.Account_UID = eqpuserlist[0].EQPUser_Uid;
                            returnUser.User_Name = eqpuserlist[0].User_Name;
                            returnUser.Enable_Flag = true;
                            returnUser.MH_Flag = true;
                            returnUser.User_NTID = "EQPUser";
                        }
                    }
                    catch
                    {

                    }
                }
            }




            return returnUser;
        }
        #endregion //User Part
        public int GetProcessPlan( int projectUID,string Process, DateTime date)
        {
            return flowChartMgDataRepository.GetProcessPlan(projectUID,Process, date);
        }
        public IEnumerable<SystemRoleDTO> GetAllRoles()
        {
            var roles = systemRoleRepository.GetAll().AsEnumerable();
            return AutoMapper.Mapper.Map<IEnumerable<SystemRoleDTO>>(roles);
        }

        public IEnumerable<SystemPlantDTO> GetValidPlantsByUserUId(int accountUId)
        {
            var plants = systemPlantRepository.GetValidPlantsByUserUId(accountUId).AsEnumerable();
            return AutoMapper.Mapper.Map<IEnumerable<SystemPlantDTO>>(plants);
        }

        public IEnumerable<SystemBUMDTO> GetValidBUMsByUserUId(int accountUId)
        {
            var bums = systemBUMRepository.GetValidBUMsByUserUId(accountUId).AsEnumerable();
            return AutoMapper.Mapper.Map<IEnumerable<SystemBUMDTO>>(bums);
        }

        public IEnumerable<SystemBUDDTO> GetValidBUDsByUserUId(int accountUId)
        {
            var buds = systemBUDRepository.GetValidBUDsByUserUId(accountUId).AsEnumerable();
            return AutoMapper.Mapper.Map<IEnumerable<SystemBUDDTO>>(buds);
        }

        public IEnumerable<SystemOrgDTO> GetValidOrgsByUserUId(int accountUId)
        {
            var buds = systemOrgRepository.GetValidOrgsByUserUId(accountUId).AsEnumerable();
            return AutoMapper.Mapper.Map<IEnumerable<SystemOrgDTO>>(buds);
        }

        //获取当前用户所拥有的Flowchart专案列表
        public List<int> GetProjectUIDList(int uid)
        {
            var projectList =
projectUsersGroupRepository.GetMany(m => m.Account_UID == uid).Select(m => m.Project_UID).ToList();
            var list = projectList;
            return list;
        }

        //无锡Etransfer要用，必须要在用户设置画面勾选专案，不然就不属于Etransfer
        //---------20170429修改，通过Project_Type来判断--------------------------
        public List<string> GetProjectNameList(int uid)
        {
            var projectUIDList =
projectUsersGroupRepository.GetMany(m => m.Account_UID == uid).Select(m => m.Project_UID).ToList();
            //var projectNameList = systemProjectRepository.GetMany(m => projectUIDList.Contains(m.Project_UID)).Select(m => m.Project_Name.ToUpper()).ToList();
            //return projectNameList;
            var projectTypeList = systemProjectRepository.GetMany(m => projectUIDList.Contains(m.Project_UID)).Select(m => m.Project_Type).ToList();
            return projectTypeList;
        }

        public List<String> GetOptysByNTId(int accountUId)
        {
            //获取当前用户相关信息
            var opTypes = systemUserOrgRepository.GetUserOpTypes(accountUId);
           

            return opTypes;
        }

        //数据权限------Sidney 2016-4-26
        public CurrentUserDataPermission GetPermissonsByNTId(int accountUId)
        {
            //获取当前用户相关信息
            var opTypes = systemUserOrgRepository.GetUserOpTypes(accountUId);
            var project =
                projectUsersGroupRepository.GetMany(m => m.Account_UID == accountUId).Select(m => m.Project_UID).ToList();
            var process =
                flowChartPCMHRelationshipRepository.GetMany(m => m.MH_UID == accountUId)
                    .Select(m => new PDMS.Model.ProcessInfo()
                    {
                        FlowChart_Detail_UID = m.FlowChart_Detail_UID,
                        Place = m.Place,
                        Process = m.FlowChart_Detail.Process,
                        FlowChart_Master_UID = m.FlowChart_Detail.FlowChart_Master_UID
                    }).Distinct().ToList();
            var permission = new CurrentUserDataPermission
            {
                Op_Types = opTypes,
                Project_UID = project,
                ProcessInfo = process,
                UserOrgInfo = null
            };

            return permission;
        }
        #region-------------------------------------Get User Info By AccountUID Add by Rock 2016-05-31
        public CustomUserInfoVM GetUserInfo(int uid)
        {
            CustomUserInfoVM userVM = new CustomUserInfoVM();
            List<SystemRoleDTO> roleDTOList = new List<SystemRoleDTO>();
            var userInfo = systemUserRepository.GetById(uid);
            
                userVM = AutoMapper.Mapper.Map<CustomUserInfoVM>(userInfo);
            
                foreach (var item in userInfo.System_User_Role)
                {
                    roleDTOList.Add(AutoMapper.Mapper.Map<SystemRoleDTO>(item.System_Role));
                }
                userVM.RoleList = roleDTOList;
            

                //获取用户所在的厂区和所属的4级信息
                userVM.OrgInfo = GetOrgInfoByUserUId(userInfo.Account_UID);

                //排除掉超级管理员只设定Site其他都没设定的情况，不然会取到值为null的数据
                var opTypes = userVM.OrgInfo.Where(m => m.OPType != null).Select(m => m.OPType).ToList();
                var orgUIDList = userVM.OrgInfo.Where(m => m.OPType != null).Select(m => m.OPType_OrganizationUID.Value).ToList();
                var plantUIDList = userVM.OrgInfo.Select(m => m.Plant_OrganizationUID.Value).ToList();
                userVM.OpTypes = opTypes;
                userVM.OPType_OrganizationUIDList = orgUIDList;
                userVM.Plant_OrganizationUIDList = plantUIDList;
                //获取当前用户所拥有的Flowchart专案列表
                userVM.ProjectUIDList = GetProjectUIDList(uid);
                userVM.projectTypeList = GetProjectNameList(uid);
                
            
            return userVM;
        }
        #endregion----------------------------------Get User Info By AccountUID Add by Rock 2016-05-31

        public List<OrganiztionVM> GetOrgInfoByUserUId(int Account_UId)
        {
            return systemOrgRepository.QueryOrganzitionInfoByAccountID(Account_UId);
        }

        //获取父节点组织ＩＤ
        public string  GetFatherOrgID(int? childUid = null)
        {

            return systemOrgBomRepository.GetFatherOrgID(childUid);
        }

        //获取父节点相关信息
        public KeyValuePair<int?, string> GetFatherOrgInfo(int? childUid=null)
        {
            var result = new System_OrganizationBOM();
            if (childUid != null)
            {
                result = systemOrgBomRepository.GetMany(m => m.ChildOrg_UID == childUid).FirstOrDefault();
            }
            var Dresult = new Dictionary<int?, string>();
            if (result != null)
                Dresult.Add(result.ParentOrg_UID,result.System_Organization.Organization_Name);
            return Dresult.FirstOrDefault();
        }
        public List<string> GetFiltProject(List<string> CurrentOP, List<int> CurrentProject)
        {
            //获取到OP对应的专案
            var OpList = new List<string>();
            //如果当前没有OP存在，则查询出所有OP下面的专案
            if (CurrentOP == null || !CurrentOP.Any())
            {
                var OPProject = systemProjectRepository.GetAll().Select(m => m.Project_Name).Distinct().ToList();
                //var OPProject = systemProjectRepository.GetMany(m=>m.Project_Name.ToUpper().Contains("BEIJING")==false).Select(m => m.Project_Name).Distinct().ToList();
                OpList.AddRange(OPProject);
            }
            else
            {
                //如果存在，则筛选出该OP下面的所有专案
                foreach (var item in CurrentOP)
                {
                    var OPProject = systemProjectRepository.GetMany(m => m.OP_TYPES == item).Select(m => m.Project_Name).Distinct().ToList();
                    //var OPProject = systemProjectRepository.GetMany(m => m.OP_TYPES == item&&m.Project_Name.ToUpper().Contains("BEIJING")==false).Select(m => m.Project_Name).Distinct().ToList();
                    OpList.AddRange(OPProject);
                }
            }
            //如果当前用户所在Project不存在，则只回传OP对应的专案
            if (CurrentProject == null || !CurrentProject.Any())
            {
                return OpList;
            }
            else
            {
                //如果当前用户Project存在，则取两者并集
                var projectList = new List<string>();
                foreach (var item in CurrentProject)
                {
                    var projectItem = systemProjectRepository.GetMany(m => m.Project_UID == item).Select(m => m.Project_Name).FirstOrDefault();
                    projectList.Add(projectItem);
                }
                var result = OpList.Intersect(projectList).Distinct().ToList();
                return result;
            }
        }

        /// <summary>
        /// 根据用户组织UID获取可以选择的BU
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<string> GetUserBu(int OrgUID)
        {
           return systemProjectRepository.GetUserBu(OrgUID);
        }
        public string GetProjectSite(int uid)
        {
            var projectSite = systemProjectRepository.GetProjectSite(uid);
            return projectSite;
        }

        public void ChangeLanguageInfo(int accountId, int languid)
        {
            var dto = systemUserRepository.GetById(accountId);
            dto.System_Language_UID = languid;
            unitOfWork.Commit();
        }

        public int GetSelectLanguageId(int accountId)
        {
            var dto = systemUserRepository.GetById(accountId);
            if (dto == null)
            {
                return 2;
            }
            else
            {
                return dto.System_Language_UID;
            }

        }

    }
}
