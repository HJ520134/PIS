using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

using PDMS.Common;

using PDMS.Model.ViewModels.Settings;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using MoreLinq;
using PDMS.Service.Language;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels.Batch;

namespace PDMS.Service
{
    public interface ISettingsService
    {
        #region define System_User interface ---------------------Add by Tonny 2015/11/03

        PagedListModel<SystemUserInfo> QueryUsers(UserModelSearch search, Page page);
        IEnumerable<SystemUserDTO> DoExportUser(string uuids);
        System_Users QueryUsersSingle(int uuid);
        int QueryUsersNTID(string NTID);
        string AddUser(System_Users ent);
        void ModifyUser(System_Users ent);
        bool DeleteUser(System_Users ent);
        bool ForbidUser(System_Users ent);
        string ChangeUserPassword(UserPasswordInfo info);

        #endregion //define System_Users interface ---------------------Add by Tonny 2015/11/09

        #region define System_User interface ---------------------Add by Robert 2016/10/20
        bool EmployeeLogin(string userEmployee, string password);
        bool MH_FlagLogin(string userEmployee, string password);
        #endregion //define System_Users interface ---------------------Add by Robert 2016/10/20

        #region All of Rock's interface -----------Start-----------Add by Rock 2015/11/18

        #region define System_BU_M interface ---------------------Add by Rock 2015/11/09
        PagedListModel<BUModelGet> QueryBus(BUModelSearch search, Page page);
        bool CheckBuExistById(string BU_ID, string BU_Name);
        void AddBU(System_BU_M bu);
        System_BU_M QueryBUSingle(int BU_M_UID);
        void ModifyBU(System_BU_M item);
        bool DeleteBU(System_BU_M item);
        List<string> GetTypeClassfy(int userUID);
        IEnumerable<BUModelGet> DoExportBU(string uuids);
        #endregion //define System_BU_M interface ---------------------Add by Rock 2015/11/09

        #region define System_BU_D interface ---------------------Add by Rock 2015/11/12
        PagedListModel<BUDetailModelGet> QueryBUDs(BUDetailModelSearch search, Page page);
        bool CheckBuExistById_Two(string buid);
        SystemBUMDTO GetBUIDAndBUNameByBUID(string BU_ID);
        //bool CheckExistBU_D_ID(string BU_D_ID, int BU_D_UID, bool isEdit);
        void AddOrEditBUDetailInfo(SystemBUDDTO dto, string BU_ID, string BU_Name, bool isEdit);
        BUDetailModelGet QueryBUDSingle(int BU_D_UID);
        System_BU_D QueryBUDSingleByModule(int BU_D_UID);
        bool DeleteBUDSingle(System_BU_D budItem);
        IEnumerable<BUDetailModelGet> DoExportBUDetail(string BU_D_UIDS);
        string CheckBeginDateAndEndDate(string BU_ID, string BU_Name, string Begin_Date, string End_Date);
        SystemBUDDTO QueryBUDInfoByBU_D_ID(string BU_D_ID);
        #endregion //define System_BU_D interface ---------------------Add by Rock 2015/11/12

        #region define System_User_Business_Group ---------Start------------Add by Rock 2015/11/18
        PagedListModel<UserBUSettingGet> QueryUserBUs(UserBUSettingSearch search, Page page);
        List<BUAndBUDByUserNTID> QueryUserBU(int System_User_BU_UID);
        CustomBUItemAndBUD QueryBUAndBUDSByBUIDS(string BU_ID);
        string AddUserBU(UserBUAddOrSave addOrSave);
        string EditUserBU(UserBUAddOrSave addOrSave);
        IEnumerable<UserBUSettingGet> DoExportUserBU(string KeyIDS);
        #endregion define System_User_Business_Group ---------End------------Add by Rock 2015/11/18

        #endregion All of Rock's interface -----------End-----------Add by Rock 2015/11/18

        #region define System_Role interface ---------------------Add by Allen 2015/11/12
        PagedListModel<SystemRoleDTO> QueryRoles(RoleModelSearch search, Page page);
        IEnumerable<SystemRoleDTO> DoExportRole(string ruids);
        System_Role QueryRolesSingle(int uuid);
        bool IsExistRoleID(string role_ID);
        void AddRole(System_Role ent);
        void ModifyRole(System_Role ent);
        string DeleteRole(System_Role ent);
        bool CheckRoleExistById(string rid);
        #endregion //define System_Role interface ---------------------Add by Allen 2015/11/12

        #region define Plant interface ---------------------Add by Sidney 2015/11/13
        PagedListModel<SystemPlantDTO> QueryPlants(PlantModelSearch search, Page page);
        IEnumerable<SystemPlantDTO> DoExportPlant(string uuids);
        System_Plant QueryPlantSingle(int uuid);
        string AddPlant(System_Plant ent);
        string ModifyPlant(System_Plant ent);
        string DeletePlant(System_Plant ent);
        bool CheckPlantExistByUId(int uuid);
        bool CheckPlantExistByPlant(string plant);
        DateTime? GetMaxEnddate4Plant(int uid);
        #endregion //define Plant interface ---------------------Add by Sidney 2015/11/13

        #region define System Function Maintenance interface ------------------ Add by Tonny 2015/11/11
        PagedListModel<FunctionItem> QueryFunctions(FunctionModelSearch searchModel, Page page);
        string DeleteFunction(int uid);
        string DeleteSubFunction(int subfun_uid);
        string AddFunctionWithSubs(FunctionWithSubs vm);
        System_Function QueryFunctionWithSubs(int uid);
        System_Function GetFunctionByID(string functionId);
        System_Function GetFunction(int uid);
        string ModifyFunctionWithSubs(FunctionWithSubs vm);
        IEnumerable<FunctionItem> DoExportFunction(string uids);
        #endregion //end define System Function Maintenance interface

        #region define User Plant Setting interface ------------------ Add by Sidney 2015/11/18
        PagedListModel<UserPlantItem> QueryUserPlants(UserPlantModelSearch searchModel, Page page);
        System_Plant GetPlantInfo(string Plant);
        List<string> getFunPOrg(string Plant);
        string AddUserPlantWithSubs(UserPlantEditModel vm);
        IQueryable<UserPlantWithPlant> QueryUserPlantByAccountUID(int uid);
        string ModifyUserPlantWithSubs(UserPlantEditModel vm);
        IEnumerable<UserPlantItem> DoExportUserPlant(string uids);
        #endregion //end defineUser Plant Setting interface

        #region define Role Function Setting interface -------------- Add by Tonny 2015/11/18
        PagedListModel<RoleFunctionItem> QueryRoleFunctions(RoleFunctionSearchModel searchModel, Page page);
        RoleFunctionsWithSub QueryRoleFunction(int uid);
        IEnumerable<RoleFunctionItem> DoExportRoleFunctions(string uids);
        IEnumerable<SubFunction> QueryRoleSubFunctions(int roleUId, int functionUId);
        IEnumerable<SubFunction> QuerySubFunctionsByFunctionUID(int functionUId);
        void DeleteRoleFunction(int uid);
        string MaintainRoleFunctionWithSubs(RoleFunctionsWithSub vm, int AccountUId);
        #endregion

        #region define System_User_Role interface ---------------------Add by Allen 2015/11/16
        PagedListModel<UserRoleItem> QueryUserRoles(UserRoleSearchModel search, Page page);
        IEnumerable<UserRoleItem> DoExportUserRole(string uruids);
        System_User_Role QueryUserRolesSingle(int uruid);
        void AddUserRole(System_User_Role ent);
        System_Role GetRoleNameById(string roleid);
        bool DeleteUserRole(System_User_Role ent);
        System_User_Role GetSystemUserRole(int accountUId, int roleUId);
        #endregion //define System_Role interface

        #region define Project Maintanance interface----------Add by Sidney 2016/5/20

        PagedListModel<ProjectVM> QueryProjects(ProjectSearchModel search, Page page);
        List<string> GetCustomer();
        List<string> GetProductPhase();
        List<SystemOrgDTO> GetOpTypes(int plantuid, int oporguid);
        List<SystemOrgDTO> GetPlants(int PlantOrgUid);
        ProjectVM QueryProjectSingle(int uuid);
        string AddProject(ProjectVMDTO vm);
        string EditProject(ProjectVMDTO vm);
        string DeleteProject(int Project_UID);

        List<System_Project> GetProjectByOrgId(int org);
        #endregion

        #region org  by justin  interface
        PagedListModel<SystemOrgDTO> QueryOrgs(OrgModelSearch search, Page page);
        IEnumerable<SystemOrgDTO> DoExportOrg(string uids);
        System_Organization QueryOrgSingle(int uuid);
        void AddOrg(System_Organization ent);
        void ModifyOrg(System_Organization ent);
        bool DeleteOrg(int uid);
        bool CheckOrgExistByIdAndName(string id, string name);
        bool CheckOrgExistByIdAndNameWithUId(int uid, string id, string name);
        DateTime? GetMaxEnddate4Org(int orgUId);
        #endregion

        #region OrgBom  by justin  interface
        PagedListModel<SystemOrgAndBomDTO> QueryOrgBom(OrgBomModelSearch search, Page page);
        IEnumerable<SystemOrgAndBomDTO> DoExportOrgBom(string uuids);
        SystemOrgAndBomDTO QueryOrgBom(int uid);
        string AddOrgBom(System_OrganizationBOM ent);
        string ModifyOrgBom(SystemOrgBomDTO ent);
        bool CheckOrgBomExist(int uid, int parentUId, int childUId, int index);
        #endregion

        #region define User Organization Setting interface ------------------ Add by Justin
        PagedListModel<UserOrgItem> QueryUserOrgs(UserOrgModelSearch searchModel, Page page);
        IEnumerable<System_Organization> GetOrgInfo(string OrgID);
        string AddUserOrgWithSubs(UserOrgEditModel vm);
        IQueryable<UserOrgWithOrg> QueryUserOrgsByAccountUID(int uuid);
        string ModifyUserOrgWithSubs(UserOrgEditModel vm);
        IEnumerable<UserOrgItem> DoExportUserOrg(string uids);
        #endregion //end defineUser Plant Setting interface

        #region FuncPlant Maintanance  interface--------------------------------Add By Sidney
        string AddFuncPlant(System_Function_Plant ent);
        System_Plant GetPlantByPlant(string Plant);
        PagedListModel<FuncPlantMaintanance> QueryFuncPlants(FuncPlantSearchModel search, Page page);
        List<string> GetPlantSingle();
        FuncPlantMaintanance QueryFuncPlant(int uuid);
        string DeleteFuncPlant(int uuid);
        System_Function_Plant QueryFuncPlantSingle(int uuid);
        string ModifyFuncPlant(System_Function_Plant ent);
        IEnumerable<FuncPlantMaintanance> DoExportFuncPlant(string uuids);
        List<SystemFunctionPlantDTO> GetAllFuncPlants();
        #endregion
        #region User FuncPlant Setting interface-----------------------Add By Sidney
        PagedListModel<UserFuncPlantVM> QueryUserFuncPlants(UserFuncPlantSearch search, Page page);
        List<string> GetFuncPlant(string Plant, string OP_Types);
        string GetUserInfo(string User_NTID);
        string EditUserFuncPlantWithSubs(EditUserFuncPlant vm);
        List<UserPlantWithPlants> GetUserInfoByFuncPlant(string FuncPlant);
        #endregion

        #region Admin maintance function interface--------------------------Add By Destiny
        string AddEnumeration(Enumeration ent);
        bool DeleteEnumeration(int enum_uid);
        List<string> GetEnumNameForKeyProcess();
        PagedListModel<EnumerationDTO> GetEnumValueForKeyProcess(FlowChartModelSearch search, Page page);
        List<EnumerationDTO> GetEnumList(string enumType);
        string Edit_WIP(EditWIPDTO dto);
        string Edit_Product(EditProductDTO dto);
        string Edit_Flag(EditFlagDTO dto);
        string Edit_Enum(EditEnumDTO dto);
        string ExeSql(string SQLText);
        string Add_Enum(Enumeration dto);
        #endregion

        #region Binding MH and Process interface--------------------Add By Sidney
        SystemOrgAndProVM getOrgAndPro(int currentUser, int Org_OP_Id);
        List<string> getUserPlant(int currentUser, string plant);
        string AddOrgInfo(SystemUserInfo1 info);
        string AddProjectInfo(SystemUserInfo1 info);
        string AddRoleInfo(SystemUserInfo1 info);
        string ModifyOrgInfo(SystemUserInfo1 info);
        string ModifyProjectInfo(SystemUserInfo1 info);
        string ModifyRoleInfo(SystemUserInfo1 info);
        SystemUserInfo1 QueryOrgAndProjectByUid(int uuid);
        int GetUserAccountUid(string NTID);
        #endregion
        PagedListModel<SystemBUD_OrgDTO> QueryBU_Org(BUD_OrgSearch search, Page page);
        void AddBU_Org(System_BU_D_Org bu);
        System_BU_D_Org QueryBU_OrgSingle(int BU_M_UID);
        void ModifyBU_Org(System_BU_D_Org item);
        bool DeleteBU_Org(System_BU_D_Org item);

        #region Language Maintance interface -------------------------Add By Rock 2017/7/17
        List<SystemLanguageDTO> LanguagesInfo();
        string GetLocaleStringResource(int languageID, string resourceName);
        PagedListModel<SystemLocaleStringResourceDTO> LocaleStringResourceInfo(SystemLocaleStringResourceDTO searchModel, Page page);
        string SaveResourceInfo(SystemLocaleStringResourceDTO searchModel);
        List<SystemLocaleStringResourceDTO> LocaleStringResourceInfoByLanguageId(int System_Language_UID);
        string ImportLanguageExcel(List<SystemLocaleStringResourceDTO> list);
        #endregion Language Maintance interface -------------------------Add By Rock 2017/7/17

        #region 生产车间维护
        PagedListModel<WorkshopDTO> QueryWorkshops(WorkshopModelSearch search, Page page);
        Workshop QueryWorkshopSingle(int uid);
        string AddWorkshop(Workshop workshop);
        string EditWorkshop(Workshop workshop);
        string DeleteWorkshop(int uid);
        IEnumerable<WorkshopDTO> DoExportWorkshop(string uids);
        #endregion

        #region 制程维护
        PagedListModel<Process_InfoDTO> QueryProcess_Infos(Process_InfoModelSearch search, Page page);
        IList<Process_InfoDTO> QueryProcess_InfoList(Process_InfoModelSearch search);
        Process_Info QueryProcess_InfoSingle(int uid);
        string AddProcess_Info(Process_Info workshop);
        string EditProcess_Info(Process_Info workshop);
        string DeleteProcess_Info(int uid);
        IEnumerable<Process_InfoDTO> DoExportProcess_Info(string uids);
        #endregion

        #region 表格内容多语言
        PagedListModel<System_LocalizedPropertyDTO> QuerySystemLocalizedProperties(System_LocalizedPropertyModelSearch search, Page page);
        System_LocalizedPropertyDTO QuerySystemLocalizedPropertySingle(int uid);
        string AddSystemLocalizedProperty(System_LocalizedPropertyDTO dto);
        string EditSystemLocalizedProperty(System_LocalizedPropertyDTO dto);
        string DeleteSystemLocalizedProperty(int uid);
        #endregion

        #region Add By Rock 排程邮件设定 平安夜加班..... 2017/12/24
        PagedListModel<SystemModuleEmailVM> QueryBatchEmailSetting(SystemModuleEmailVM search, Page page);

        List<SystemModuleEmailFunctionVM> GetBatchEmailSettingFunction(bool isAdmin, int plantUID);

        SystemModuleEmailVM QueryBatchEmailSettingByEdit(int System_Email_Delivery_UID);

        SystemUserDTO ChangeNTID(string User_NTID);

        string SaveBatchEmailSetting(SystemModuleEmailVM item);

        string CheckEmailIsError(SystemModuleEmailVM item);
        #endregion 平安夜加班..... 2017/12/24



    }
    public class SettingsService : ISettingsService
    {
        #region Private interfaces properties

        private readonly ICommonService commonService;
        private readonly IUnitOfWork unitOfWork;
        private readonly ISystemFunctionRepository systemFunctionRepository;
        private readonly ISystemUserRepository systemUserRepository;
        private readonly ISystemRoleRepository systemRoleRepository;
        private readonly ISystemUserRoleRepository systemUserRoleRepository;
        private readonly ISystemBUMRepository systemBURepository;
        private readonly ISystemBUDRepository systemBUDRepository;
        private readonly ISystemUserBusinessGroupRepository systemUserBusinessGroupRepository;
        private readonly ISystemRoleFunctionRepository systemRoleFunctionRepository;
        private readonly ISystemPlantRepository systemPlantRepository;
        private readonly ISystemFunctionSubRepository systemFunctionSubRepository;
        private readonly ISystemRoleFunctionSubRepository systemRoleFunctionSubRepository;

        private readonly ISystemUserPlantRepository systemUserPlantRepository;
        private readonly IEnumerationRepository enumerationRepository;

        //org
        private readonly ISystemOrgRepository systemOrgRepository;
        //orgBom
        private readonly ISystemOrgBomRepository systemOrgBomRepository;

        private readonly ISystemUserOrgRepository systemUserOrgRepository;
        private readonly ISystemFunctionPlantRepository systemFunctionPlantRepository;
        private readonly ISystemUserFunPlantRepository systemUserFunPlantRepository;
        private readonly IProductInputRepository productInputRepository;
        private readonly ISystemProjectRepository systemProjectRepository;
        private readonly IProjectUsersGroupRepository projectUsersGroupRepository;
        private readonly IFlowChartMasterRepository flowChartMasterRepository;
        private readonly ISystemBUDOrgRepository systemBUDOrgRepository;
        private readonly ISystemLanguageRepository systemLanguageRepository;
        private readonly ISystemLocaleStringResourceRepository systemLocaleStringResourceRepository;
        private readonly IWorkshopRepository workshopRepository;
        private readonly IProcess_InfoRepository process_InfoRepository;
        private readonly ISystemEmailDeliveryRepository systemEmailDeliveryRepository;
        private readonly ISystemLocalizedPropertyRepository systemLocalizedPropertyRepository;
        #endregion //Private interfaces properties

        #region Service constructor
        public SettingsService(
            ISystemFunctionRepository systemFunctionRepository,
            ISystemUserRepository systemUserRepository,
            ISystemUserRoleRepository systemUserRoleRepository,
            ISystemBUMRepository systemBURepository,
            ISystemBUDRepository systemBUDRepository,
            ISystemUserBusinessGroupRepository systemUserBusinessGroupRepository,
            ISystemRoleRepository systemRoleRepository,
            ISystemRoleFunctionRepository systemRoleFunctionRepository,
            ISystemPlantRepository systemPlantRepository,
            ISystemFunctionSubRepository systemFunctionSubRepository,
            ISystemRoleFunctionSubRepository systemRoleFunctionSubRepository,
            ISystemBUDOrgRepository systemBUDOrgRepository,
            ISystemUserPlantRepository systemUserPlantRepository,

             ISystemOrgRepository systemOrgRepository,
            ISystemOrgBomRepository systemOrgBomRepository,

            ISystemUserOrgRepository systemUserOrgRepository,
            ISystemFunctionPlantRepository systemFunctionPlantRepository,
            ISystemUserFunPlantRepository systemUserFunPlantRepository,
            IEnumerationRepository enumerationRepository,
            IProductInputRepository productInputRepository,
            ISystemProjectRepository systemProjectRepository,
            IProjectUsersGroupRepository projectUsersGroupRepository,
            IFlowChartMasterRepository flowChartMasterRepository,
            ISystemLanguageRepository systemLanguageRepository,
            ISystemLocaleStringResourceRepository systemLocaleStringResourceRepository,
            ICommonService commonService,
        IUnitOfWork unitOfWork,
        IWorkshopRepository workshopRepository,
        IProcess_InfoRepository process_InfoRepository,
        ISystemEmailDeliveryRepository systemEmailDeliveryRepository,
        ISystemLocalizedPropertyRepository systemLocalizedProperty)
        {
            this.commonService = commonService;
            this.unitOfWork = unitOfWork;
            this.systemFunctionRepository = systemFunctionRepository;
            this.systemUserRepository = systemUserRepository;
            this.systemUserRoleRepository = systemUserRoleRepository;
            this.systemBURepository = systemBURepository;

            this.systemBUDRepository = systemBUDRepository;

            this.systemUserBusinessGroupRepository = systemUserBusinessGroupRepository;

            this.systemRoleRepository = systemRoleRepository;
            this.systemRoleFunctionRepository = systemRoleFunctionRepository;
            this.systemPlantRepository = systemPlantRepository;
            this.systemFunctionSubRepository = systemFunctionSubRepository;
            this.systemRoleFunctionSubRepository = systemRoleFunctionSubRepository;

            this.systemUserPlantRepository = systemUserPlantRepository;

            this.systemOrgRepository = systemOrgRepository;
            this.systemOrgBomRepository = systemOrgBomRepository;
            this.systemBUDOrgRepository = systemBUDOrgRepository;
            this.systemUserOrgRepository = systemUserOrgRepository;
            this.systemFunctionPlantRepository = systemFunctionPlantRepository;
            this.enumerationRepository = enumerationRepository;
            this.systemUserFunPlantRepository = systemUserFunPlantRepository;
            this.productInputRepository = productInputRepository;
            this.systemProjectRepository = systemProjectRepository;
            this.projectUsersGroupRepository = projectUsersGroupRepository;
            this.flowChartMasterRepository = flowChartMasterRepository;
            this.systemLanguageRepository = systemLanguageRepository;
            this.systemLocaleStringResourceRepository = systemLocaleStringResourceRepository;
            this.workshopRepository = workshopRepository;
            this.process_InfoRepository = process_InfoRepository;
            this.systemEmailDeliveryRepository = systemEmailDeliveryRepository;
            this.systemLocalizedPropertyRepository = systemLocalizedProperty;
        }
        #endregion //Service constructor

        #region System_Users implement interface---------------------------Add by Robert 2016/10/20
        public bool EmployeeLogin(string userEmployee, string password)
        {
            return systemUserRepository.EmployeeLogin(userEmployee, password);
        }

        public bool MH_FlagLogin(string User_NTID, string password)
        {
            var hasUser = systemUserRepository.GetMany(m => m.User_NTID == User_NTID && m.EmployeePassword == password).FirstOrDefault();
            return hasUser == null ? false : true;
        }
        #endregion //System_Users implement interface---------------------------Add by Robert 2016/10/20


        #region System_Users implement interface---------------------------Add by Tonny 2015/11/14 

        public PagedListModel<SystemUserInfo> QueryUsers(UserModelSearch searchModel, Page page)
        {

            var roles = systemRoleRepository.GetUserRoleNow((int)searchModel.currntUID);
            List<int> roleIDs = new List<int>();
            bool flag = false;
            foreach (var item in roles)
            {
                if (item.UserRoleID == "SystemAdmin")
                    flag = true;
                roleIDs.Add(item.RoleUID);
            }
            var totalCount = 0;
            var users = systemUserRepository.QueryUsersJ(searchModel, page, out totalCount, roles).Distinct();
            var userOrgAndProList = new List<SystemUserInfo>();

            foreach (var item in users)
            {
                var userOrgAndPro = new SystemUserInfo();
                //添加SystemUserDTO的值
                userOrgAndPro.Account_UID = item.Account_UID;
                userOrgAndPro.Email = item.Email;
                userOrgAndPro.Enable_Flag = item.Enable_Flag;
                userOrgAndPro.ExportUIds = item.ExportUIds;
                userOrgAndPro.LoginToken = item.LoginToken;
                userOrgAndPro.ModifiedUser = item.ModifiedUser;
                userOrgAndPro.Modified_Date = item.Modified_Date;
                userOrgAndPro.Modified_UID = item.Modified_UID;
                userOrgAndPro.Modified_UserNTID = item.Modified_UserNTID;
                userOrgAndPro.Modified_UserName = item.Modified_UserName;
                userOrgAndPro.Tel = item.Tel;
                userOrgAndPro.User_NTID = item.User_NTID;
                userOrgAndPro.User_Name = item.User_Name;
                userOrgAndPro.Building = item.Building;
                userOrgAndPro.EmployeeNumber = item.EmployeeNumber;
                userOrgAndPro.EmployeePassword = item.EmployeePassword;
                userOrgAndPro.EnableEmpIdLogin = item.EnableEmpIdLogin;
                //添加Project
                var userProject = projectUsersGroupRepository.GetMany(m => m.Account_UID == item.Account_UID).Select(m => m.System_Project.Project_Name);
                userOrgAndPro.Project = string.Join(",", userProject.ToArray());
                //添加User_Role
                var userRole =
                    systemUserRoleRepository.GetMany(m => m.Account_UID == item.Account_UID).Select(m => m.System_Role.Role_Name);

                userOrgAndPro.User_Role = string.Join(",", userRole.ToArray());
                //添加Org字符串
                var userOrg1 = systemOrgRepository.QueryOrganzitionInfoByAccountID(item.Account_UID);
                List<string> useOrgList = new List<string>();
                if (userOrg1 != null && userOrg1.Count > 0)
                {
                    if (!string.IsNullOrEmpty(userOrg1[0].Plant))
                        useOrgList.Add(userOrg1[0].Plant);
                    userOrgAndPro.Org_Plant = userOrg1[0].Plant;
                    if (!string.IsNullOrEmpty(userOrg1[0].OPType))
                        useOrgList.Add(userOrg1[0].OPType);
                    userOrgAndPro.Org_OP = userOrg1[0].OPType;
                    if (!string.IsNullOrEmpty(userOrg1[0].Department))
                        useOrgList.Add(userOrg1[0].Department);
                    if (!string.IsNullOrEmpty(userOrg1[0].Funplant))
                        useOrgList.Add(userOrg1[0].Funplant);
                    var userOrgString = string.Join(">", useOrgList);
                    userOrgAndPro.Org_CTU = userOrgString;
                }
                userOrgAndProList.Add(userOrgAndPro);
            }
            userOrgAndProList = userOrgAndProList.GroupBy(p => new { p.Account_UID }).Select(g => g.First()).ToList();
            //if (!flag)
            //{
            //    if (searchModel.Orgnizations != null && searchModel.Orgnizations.Count > 0 && searchModel.Orgnizations[0].OPType != null)
            //    {
            //        if (searchModel.Orgnizations[0].OPType=="Support team")
            //        {
            //            userOrgAndProList = userOrgAndProList.Where(p => p.Org_Plant == searchModel.Orgnizations[0].Plant).ToList();
            //        }
            //        else
            //        {
            //            userOrgAndProList = userOrgAndProList.Where(p => p.Org_OP == searchModel.Orgnizations[0].OPType).ToList();
            //        }
            //    }
            //    else
            //    {
            //        userOrgAndProList = null;
            //        totalCount = 0;
            //    }
            //}
            return new PagedListModel<SystemUserInfo>(totalCount, userOrgAndProList);
        }

        public IEnumerable<SystemUserDTO> DoExportUser(string uuids)
        {
            var totalCount = 0;
            return systemUserRepository
                        .QueryUsers(new UserModelSearch { ExportUIds = uuids }, null, out totalCount)
                        .AsEnumerable();
        }
        public List<string> getUserPlant(int currentUser, string plant)
        {

            var result = systemPlantRepository.GetPlantsByUserUId(currentUser, plant);
            return result;

        }
        public System_Users QueryUsersSingle(int uuid)
        {
            return systemUserRepository.GetById(uuid);
        }

        public int QueryUsersNTID(string NTID)
        {
            return systemUserRepository.QueryUserUIDByNTID(NTID);
        }

        public string AddUser(System_Users ent)
        {
            try
            {
                var maxUid = systemUserRepository.GetAll().OrderByDescending(m => m.Account_UID).Select(m => m.Account_UID).FirstOrDefault() + 1;
                ent.Account_UID = maxUid;
                ent.Enable_Flag = true;
                ent.MH_Flag = false;
                ent.System_Language_UID = 2;
                systemUserRepository.Add(ent);
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {
                return "Error";
            }

        }

        public string ChangeUserPassword(UserPasswordInfo info)
        {
            try
            {
                var user = systemUserRepository.GetById(info.UserId);
                if (user.MH_Flag)
                {
                    info.NowPassword = JGP.Common.PasswordUtil.EncryptionHelper.Encrypt(info.NowPassword, "");

                    if (user.EmployeePassword != null && user.EmployeePassword == info.NowPassword)
                    {
                        user.EmployeePassword = JGP.Common.PasswordUtil.EncryptionHelper.Encrypt(info.NewPassword, "");
                        systemUserRepository.Update(user);
                        unitOfWork.Commit();
                        return "密码修改成功,请退出并重新登录";
                    }
                    else
                    {
                        return "用户现密码输入不正确";
                    }
                }
                else
                {
                    return "该用户不是物料员账户";
                }
            }
            catch (Exception e)
            {
                return "Error";
            }
        }

        public void ModifyUser(System_Users ent)
        {
            //if (ent.Enable_Flag == false)
            //{
            //    systemUserRoleRepository.Delete(q => q.Account_UID == ent.Account_UID);
            //}
            systemUserRepository.Update(ent);
            unitOfWork.Commit();
        }

        //public bool DeleteUser(System_Users ent)
        //{
        //    var deleteRole = systemUserRoleRepository.GetMany(m => m.Account_UID == ent.Account_UID);
        //    if (deleteRole.Any())
        //    {
        //        foreach (var item in deleteRole)
        //        {
        //            systemUserRoleRepository.Delete(item);
        //        }
        //        try
        //        {
        //            unitOfWork.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            return false;
        //        }
        //    }
        //    var deleteOrg = systemUserOrgRepository.GetMany(m => m.Account_UID == ent.Account_UID);
        //    if (deleteOrg.Any())
        //    {
        //        foreach (var item in deleteOrg)
        //        {
        //            systemUserOrgRepository.Delete(item);
        //        }
        //        try
        //        {
        //            unitOfWork.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            return false;
        //        }

        //    }
        //    var deleteProject = projectUsersGroupRepository.GetMany(m => m.Account_UID == ent.Account_UID);
        //    if (deleteProject.Any())
        //    {

        //        foreach (var item in deleteProject)
        //        {
        //            projectUsersGroupRepository.Delete(item);
        //        }
        //        try
        //        {
        //            unitOfWork.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            return false;
        //        }
        //    }
        //    if (!systemUserRoleRepository.GetMany(r => r.Account_UID == ent.Account_UID).Any())
        //    {
        //        systemUserRepository.Delete(ent);
        //        unitOfWork.Commit();
        //        return true;
        //    }
        //    return false;
        //}

        public bool ForbidUser(System_Users ent)
        {
            ent.Enable_Flag = false;
            systemUserRepository.Update(ent);
            try
            {
                unitOfWork.Commit();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteUser(System_Users ent)
        {
            systemUserRepository.Delete(ent);
            var userrole = systemUserRoleRepository.GetMany(m => m.Account_UID == ent.Account_UID);
            var userorg = systemUserOrgRepository.GetMany(m => m.Account_UID == ent.Account_UID);
            var usergroup = projectUsersGroupRepository.GetMany(m => m.Account_UID == ent.Account_UID);
            foreach (System_User_Role role in userrole)
            {
                systemUserRoleRepository.Delete(role);
            }
            foreach (System_UserOrg org in userorg)
            {
                systemUserOrgRepository.Delete(org);
            }
            foreach (Project_Users_Group group in usergroup)
            {
                projectUsersGroupRepository.Delete(group);
            }
            try
            {
                unitOfWork.Commit();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion //System_Users implement interface---------------------------Add by Tonny 2015/11/14 

        #region  System_Role implement interface-------------------Add by Allen 2015/11/06

        public PagedListModel<SystemRoleDTO> QueryRoles(RoleModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var roles = systemRoleRepository.QueryRoles(searchModel, page, out totalCount);

            IList<SystemRoleDTO> rolesDTO = new List<SystemRoleDTO>();

            foreach (var role in roles)
            {
                var dto = AutoMapper.Mapper.Map<SystemRoleDTO>(role);
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(role.System_Users);
                rolesDTO.Add(dto);
            }
            return new PagedListModel<SystemRoleDTO>(totalCount, rolesDTO);
        }

        //1.查詢 2.將EF Modal轉換成DTO 3.返回
        public IEnumerable<SystemRoleDTO> DoExportRole(string ruids)
        {
            var totalCount = 0;
            //查詢QueryRoles，已ruids參數去查詢

            var roles = systemRoleRepository
                        .QueryRoles(new RoleModelSearch { ExportUIds = ruids }, null, out totalCount).ToList();
            List<SystemRoleDTO> rolesDTO = new List<SystemRoleDTO>();
            //利用Mapper把EF一個一個轉換成DTO
            foreach (var role in roles)
            {
                var dto = AutoMapper.Mapper.Map<SystemRoleDTO>(role);
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(role.System_Users);
                rolesDTO.Add(dto);
            }
            //返回DTO的值
            return rolesDTO;
        }

        public System_Role QueryRolesSingle(int uuid)
        {
            return systemRoleRepository.GetFirstOrDefault(r => r.Role_UID == uuid);
        }

        public bool IsExistRoleID(string role_ID)
        {
            return systemRoleRepository.IsExistRoleID(role_ID);
        }

        public void AddRole(System_Role ent)
        {
            var maxRoleUid1 = systemRoleRepository.GetAll().Select(m => m.Role_UID).Max();
            ent.Role_UID = maxRoleUid1 + 1;
            systemRoleRepository.AddRole(ent);
        }

        public void ModifyRole(System_Role ent)
        {
            systemRoleRepository.Update(ent);
            unitOfWork.Commit();
        }

        public string DeleteRole(System_Role ent)
        {
            if (systemRoleFunctionRepository.GetMany(r => r.Role_UID == ent.Role_UID).Count() != 0)
            {
                string error = "1";
                return error;
            }

            if (systemUserRoleRepository.GetMany(u => u.Role_UID == ent.Role_UID).Count() != 0)
            {

                string error = "2";
                return error;
            }
            else
            {
                systemRoleRepository.Delete(ent);
                unitOfWork.Commit();
                return "success";
            }
        }

        public bool CheckRoleExistById(string rid)
        {
            return systemRoleRepository.GetMany(p => p.Role_ID == rid).Count() == 0;
        }

        #endregion //System_Role implement interface-------------------Add by Allen 2015/11/06

        #region All of Rock's implement interface-------------------Start----------------Add by Rock 2015/11/18

        #region System_BU_M Module-------------------Add by Rock 2015/11/09
        /// <summary>
        /// 页面初始化加载
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PagedListModel<BUModelGet> QueryBus(BUModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var bus = systemBURepository.QueryBUs(searchModel, page, out totalCount);
            IList<BUModelGet> busDTO = new List<BUModelGet>();

            foreach (var bu in bus)
            {
                busDTO.Add(new BUModelGet
                {
                    SystemBUMDTO = AutoMapper.Mapper.Map<SystemBUMDTO>(bu),
                    SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(bu.System_Users)
                });
            }

            return new PagedListModel<BUModelGet>(totalCount, busDTO);
        }

        /// <summary>
        /// 根据buid检查是否存在
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public bool CheckBuExistById(string buid = null, string buname = null)
        {
            var bus = systemBURepository.GetInfoByBU_ID(buid, buname);
            if (bus != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 新增BU信息
        /// </summary>
        /// <param name="bu"></param>
        public void AddBU(System_BU_M bu)
        {
            systemBURepository.Add(bu);
            unitOfWork.Commit();
        }

        /// <summary>
        /// 获取单笔信息
        /// </summary>
        /// <param name="BU_M_UID"></param>
        /// <returns></returns>
        public System_BU_M QueryBUSingle(int BU_M_UID)
        {
            return systemBURepository.GetById(BU_M_UID);
        }
        public void ModifyBU(System_BU_M item)
        {
            systemBURepository.Update(item);
            unitOfWork.Commit();
        }

        public bool DeleteBU(System_BU_M item)
        {
            var count1 = systemBUDRepository.GetMany(m => m.BU_M_UID == item.BU_M_UID).Count();
            var count2 = systemUserBusinessGroupRepository.GetMany(m => m.BU_M_UID == item.BU_M_UID).Count();
            if (count1 > 0 || count2 > 0)
            {
                return false;
            }
            else
            {
                systemBURepository.Delete(item);
                unitOfWork.Commit();
                return true;
            }
        }
        public PagedListModel<SystemBUD_OrgDTO> QueryBU_Org(BUD_OrgSearch search, Page page)
        {
            var totalCount = 0;
            var bus = systemBUDOrgRepository.QueryBU_Org(search, page, out totalCount);
            IList<SystemBUD_OrgDTO> rolesDTO = new List<SystemBUD_OrgDTO>();

            foreach (var role in bus)
            {
                var dto = new SystemBUD_OrgDTO();
                dto.BU_D_UID = role.BU_D_UID;
                dto.Organization_UID = role.Organization_UID;
                dto.System_BU_D_Org_UID = role.System_BU_D_Org_UID;

                rolesDTO.Add(dto);
            }

            return new PagedListModel<SystemBUD_OrgDTO>(totalCount, rolesDTO);
        }
        public void AddBU_Org(System_BU_D_Org bu)
        {
            systemBUDOrgRepository.Add(bu);
            unitOfWork.Commit();
        }
        public System_BU_D_Org QueryBU_OrgSingle(int BU_M_UID)
        {
            return systemBUDOrgRepository.GetById(BU_M_UID);
        }

        public void ModifyBU_Org(System_BU_D_Org item)
        {
            systemBUDOrgRepository.Update(item);
            unitOfWork.Commit();
        }

        public bool DeleteBU_Org(System_BU_D_Org item)
        {

            systemBUDOrgRepository.Delete(item);
            unitOfWork.Commit();
            return true;

        }

        public IEnumerable<BUModelGet> DoExportBU(string uuids)
        {
            var totalCount = 0;
            var bus = systemBURepository.QueryBUs(new BUModelSearch { ExportUIds = uuids }, null, out totalCount);
            IList<BUModelGet> busDTO = new List<BUModelGet>();

            foreach (var bu in bus)
            {
                busDTO.Add(new BUModelGet
                {
                    SystemBUMDTO = AutoMapper.Mapper.Map<SystemBUMDTO>(bu),
                    SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(bu.System_Users)
                });
            }

            return busDTO;
        }
        #endregion //System_BU_M implement interface-------------------Add by Rock 2015/11/09

        #region System_BU_D implement interface-------------------Add by Rock 2015/11/12

        /// <summary>
        /// 页面初始化加载
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PagedListModel<BUDetailModelGet> QueryBUDs(BUDetailModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var roles = systemRoleRepository.GetUserRoleNow((int)searchModel.CurrentUser);
            List<int> roleIDs = new List<int>();
            bool flag = false;
            foreach (var item in roles)
            {
                if (item.UserRoleID == "SystemAdmin")
                    flag = true;
                roleIDs.Add(item.RoleUID);
            }
            IList<BUDetailModelGet> busDTO = new List<BUDetailModelGet>();
            if (flag)
            {
                var bus = systemBUDRepository.QueryBUDetails(searchModel, page, out totalCount);
                foreach (var bu in bus)
                {
                    busDTO.Add(SetAutoMapBUDetail(bu));
                }
            }
            else
            {
                var bus = systemBUDRepository.QueryBUDetailsByUser(searchModel, page, out totalCount);
                foreach (var bu in bus)
                {
                    busDTO.Add(SetAutoMapBUDetail(bu));
                }
            }
            //if (!flag)
            //{

            //    if (searchModel.Organization_UID != 0)   //只显示该OP绑定的 映射注释
            //    {
            //        //通过当前人的Organization_UID获取对应的BU_Org_ID
            //        string Org_ID = systemBUDRepository.GetBu_Org_By_Org(searchModel.Organization_UID);
            //        busDTO = busDTO.Where(p => p.SystemBUDDTO.BU_Org_ID == Org_ID).ToList();
            //        // busDTO = busDTO.Where(p => p.SystemBUDDTO.BU_Org_ID == searchModel.Organization_UID).ToList();
            //        totalCount = busDTO.Count();
            //    }
            //    else
            //    {
            //        busDTO = null;
            //        totalCount = 0;
            //    }
            //}
            return new PagedListModel<BUDetailModelGet>(totalCount, busDTO);
        }

        private BUDetailModelGet SetAutoMapBUDetail(System_BU_D item)
        {
            BUDetailModelGet model = new BUDetailModelGet();
            model.SystemBUMDTO = AutoMapper.Mapper.Map<SystemBUMDTO>(item.System_BU_M);
            model.SystemBUDDTO = AutoMapper.Mapper.Map<SystemBUDDTO>(item);
            model.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(item.System_Users);
            return model;
        }

        /// <summary>
        /// 根据BU_D_ID或BUD信息
        /// </summary>
        /// <param name="BU_D_ID"></param>
        /// <returns></returns>
        public SystemBUDDTO QueryBUDInfoByBU_D_ID(string BU_D_ID)
        {
            //返回到前台的方法，所以如果为空必须手动new
            SystemBUDDTO dtoItem = new SystemBUDDTO();
            var item = systemBUDRepository.GetFirstOrDefault(q => q.BU_D_ID == BU_D_ID);
            if (item != null)
            {
                dtoItem = AutoMapper.Mapper.Map<SystemBUDDTO>(item);
            }
            return dtoItem;
        }

        /// <summary>
        /// 检查buid是否存在，如果存在则说明正确，不存在则不正确
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public bool CheckBuExistById_Two(string buid)
        {
            var bus = systemBURepository.GetInfoByBU_ID(buid, null);
            if (bus != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据BU_ID获取BU_Name，将数据加载到画面上
        /// </summary>
        /// <param name="BU_ID"></param>
        /// <returns></returns>
        public SystemBUMDTO GetBUIDAndBUNameByBUID(string BU_ID)
        {
            var bum = systemBURepository.GetInfoByBU_ID(BU_ID, null);
            var bumDTO = AutoMapper.Mapper.Map<SystemBUMDTO>(bum);
            return bumDTO;
        }

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="BU_ID"></param>
        /// <param name="BU_Name"></param>
        /// <param name="isEdit"></param>
        public void AddOrEditBUDetailInfo(SystemBUDDTO dto, string BU_ID, string BU_Name, bool isEdit)
        {
            var item = new System_BU_D();
            dto.Modified_Date = DateTime.Now;
            //检查输入的主档是否存在
            var bum = systemBURepository.GetInfoByBU_ID(BU_ID, null);
            if (bum != null)
            {
                if (isEdit)
                {
                    item = systemBUDRepository.GetById(dto.BU_D_UID);
                    item.BU_D_ID = dto.BU_D_ID;
                    item.BU_D_Name = dto.BU_D_Name;
                    item.Begin_Date = dto.Begin_Date;
                    item.End_Date = dto.End_Date;
                    item.Modified_UID = dto.Modified_UID;
                    item.Modified_Date = dto.Modified_Date;
                    //item.BU_Org_ID = dto.BU_Org_ID;
                    //item.Organization_UID = dto.BU_Org_ID; 映射注释
                    systemBUDRepository.Update(item);
                }
                else
                {
                    item = AutoMapper.Mapper.Map<System_BU_D>(dto);
                    item.BU_M_UID = bum.BU_M_UID;
                    //item.BU_Org_ID = dto.BU_Org_ID;
                    //item.Organization_UID = dto.Organization_UID; 映射注释
                    systemBUDRepository.Add(item);
                }
                unitOfWork.Commit();
            }
        }

        public BUDetailModelGet QueryBUDSingle(int BU_D_UID)
        {
            BUDetailModelGet item = new BUDetailModelGet();
            var bud = systemBUDRepository.GetById(BU_D_UID);
            item.SystemBUDDTO = AutoMapper.Mapper.Map<SystemBUDDTO>(bud);
            item.SystemBUMDTO = AutoMapper.Mapper.Map<SystemBUMDTO>(bud.System_BU_M);
            return item;
        }

        public System_BU_D QueryBUDSingleByModule(int BU_D_UID)
        {
            var bud = systemBUDRepository.GetById(BU_D_UID);
            return bud;
        }

        public bool DeleteBUDSingle(System_BU_D budItem)
        {
            var count = systemUserBusinessGroupRepository.GetMany(m => m.BU_D_UID == budItem.BU_D_UID).Count();
            if (count == 0)
            {
                systemBUDRepository.Delete(budItem);
                unitOfWork.Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<BUDetailModelGet> DoExportBUDetail(string BU_D_UIDS)
        {
            var totalCount = 0;
            var bus = systemBUDRepository.QueryBUDetails(new BUDetailModelSearch { ExportUIds = BU_D_UIDS }, null, out totalCount);
            IList<BUDetailModelGet> busDTO = new List<BUDetailModelGet>();

            foreach (var bu in bus)
            {
                busDTO.Add(SetAutoMapBUDetail(bu));
            }
            return busDTO;
        }

        public string CheckBeginDateAndEndDate(string BU_ID, string BU_Name, string Begin_Date, string End_Date)
        {
            var result = string.Empty;
            var buItem = systemBURepository.GetInfoByBU_ID(BU_ID, BU_Name);
            if (buItem != null)
            {
                if (buItem.Begin_Date > Convert.ToDateTime(Begin_Date))
                {
                    result = "BU Master Begin Date cann't be greater than BU Customer Begin Date";
                    return result;
                }
                if (buItem.End_Date != null)
                {
                    if (string.IsNullOrEmpty(End_Date))
                    {
                        result = "BU Customer End Date is not empty because BU Master End Date is not empty";
                        return result;
                    }
                    if (Convert.ToDateTime(End_Date) > buItem.End_Date)
                    {
                        result = "BU Customer End Date cann't be greater than BU Master End Date";
                        return result;
                    }
                }
            }
            return result;
        }

        #endregion //System_BU_D implement interface-------------------Add by Rock 2015/11/12

        #region System_User_Business_Group implement interface--------------------Start-----------Add by Rock 2015/11/18
        public PagedListModel<UserBUSettingGet> QueryUserBUs(UserBUSettingSearch search, Page page)
        {
            var totalCount = 0;
            var roles = systemRoleRepository.GetUserRoleNow((int)search.CurrentUser);
            List<int> roleIDs = new List<int>();
            bool flag = false;
            foreach (var item in roles)
            {
                if (item.UserRoleID == "SystemAdmin")
                    flag = true;
                roleIDs.Add(item.RoleUID);
            }
            var list = systemUserBusinessGroupRepository.QueryUserBUSettings(search, page, out totalCount);
            IList<UserBUSettingGet> listDTO = new List<UserBUSettingGet>();

            foreach (var item in list)
            {
                listDTO.Add(SetAutoMapUserBUSetting(item));
            }
            if (!flag)
            {
                if (search.OrgID != 0)
                {
                    listDTO = listDTO.Where(p => p.SystemBUDDTO.Organization_UID == search.OrgID).ToList();
                    totalCount = listDTO.Count();
                }
                else
                {
                    listDTO = null;
                    totalCount = 0;
                }
            }
            return new PagedListModel<UserBUSettingGet>(totalCount, listDTO);
        }

        /// <summary>
        /// 对象转换
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private UserBUSettingGet SetAutoMapUserBUSetting(System_User_Business_Group item)
        {
            UserBUSettingGet itemDTO = new UserBUSettingGet();
            itemDTO.SystemUserBusinessGroupDTO = AutoMapper.Mapper.Map<SystemUserBusinessGroupDTO>(item);
            itemDTO.SystemBUMDTO = AutoMapper.Mapper.Map<SystemBUMDTO>(item.System_BU_M);
            itemDTO.SystemBUDDTO = AutoMapper.Mapper.Map<CustomBUDDTO>(item.System_BU_D);
            itemDTO.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(item.System_Users);
            itemDTO.SystemUserDTO1 = AutoMapper.Mapper.Map<SystemUserDTO>(item.System_Users1);
            return itemDTO;
        }

        public IEnumerable<UserBUSettingGet> DoExportUserBU(string KeyIDS)
        {
            var totalCount = 0;
            var list = systemUserBusinessGroupRepository.QueryUserBUSettings(new UserBUSettingSearch { ExportUIds = KeyIDS }, null, out totalCount);
            IList<UserBUSettingGet> listDTO = new List<UserBUSettingGet>();

            foreach (var item in list)
            {
                listDTO.Add(SetAutoMapUserBUSetting(item));
            }
            return listDTO;
        }

        public List<BUAndBUDByUserNTID> QueryUserBU(int System_User_BU_UID)
        {
            var list = systemUserBusinessGroupRepository.QueryBUAndBUDByUserNTIDS(System_User_BU_UID);
            return list;
        }

        public CustomBUItemAndBUD QueryBUAndBUDSByBUIDS(string BU_ID)
        {
            var bubudItem = systemUserBusinessGroupRepository.QueryBUAndBUDSByBUIDS(BU_ID);
            CustomBUItemAndBUD item = new CustomBUItemAndBUD();
            if (bubudItem != null)
            {
                item.BU_M_UID = bubudItem.BU_M_UID;
                item.BU_ID = bubudItem.BU_ID;
                item.BU_Name = bubudItem.BU_Name;

                List<SystemBUDDTO> dtoList = new List<SystemBUDDTO>();
                foreach (var budItem in bubudItem.System_BU_D)
                {
                    dtoList.Add(AutoMapper.Mapper.Map<SystemBUDDTO>(budItem));
                }
                if (dtoList.Count() > 0)
                {
                    item.SystemBUDDTOList = dtoList;
                }
            }
            return item;
        }

        public string AddUserBU(UserBUAddOrSave addOrSave)
        {
            string errorInfo = string.Empty;
            UserBUAddOrSave newItem = SetUserBUItemValue(addOrSave);
            errorInfo = ValidUserBUDate(newItem);

            if (!string.IsNullOrEmpty(errorInfo))
            {
                return errorInfo;
            }

            //-------------第一步,根据用户名获取原先表里面对应的所有数据
            var userItem = systemUserRepository.GetFirstOrDefault(q => q.User_NTID == addOrSave.User_NTID);
            var userBUGroupList = systemUserBusinessGroupRepository.GetMany(q => q.Account_UID == userItem.Account_UID).AsEnumerable();

            List<System_User_Business_Group> UserBUList = new List<System_User_Business_Group>();
            foreach (var item in newItem.FunctionSubs)
            {
                System_User_Business_Group groupItem = new System_User_Business_Group();
                groupItem.Account_UID = userItem.Account_UID;
                groupItem.BU_M_UID = item.BU_M_UID;
                groupItem.BU_D_UID = item.BU_D_UID;
                groupItem.Begin_Date = item.Begin_Date;
                groupItem.End_Date = item.End_Date;
                groupItem.Modified_UID = newItem.Modified_UID;
                groupItem.Modified_Date = newItem.Modified_Date;
                UserBUList.Add(groupItem);
            }

            //第三步，对UserBUList这些数据进行涮选，去掉重复的
            var compare1 = ComparisonHelper<System_User_Business_Group>.CreateComparer(m => Tuple.Create(m.BU_M_UID, m.BU_D_UID));
            UserBUList = UserBUList.Distinct(compare1).ToList();


            //第四步，需要将原先DB表中的数据先排除掉
            foreach (var userBUGroupItem in userBUGroupList)
            {
                var hasItem = UserBUList.Where(m => m.BU_M_UID == userBUGroupItem.BU_M_UID && m.BU_D_UID == userBUGroupItem.BU_D_UID).FirstOrDefault();
                if (hasItem != null)
                {
                    UserBUList.Remove(hasItem);
                }
            }

            if (UserBUList.Count() == 0)
            {
                errorInfo = "This data has exist can't add this data";
                return errorInfo;
            }

            //第五步，插入数据
            foreach (var item in UserBUList)
            {
                systemUserBusinessGroupRepository.Add(item);
            }
            unitOfWork.Commit();
            return "Success";
        }

        public string EditUserBU(UserBUAddOrSave addOrSave)
        {
            string errorInfo = string.Empty;
            UserBUAddOrSave newItem = SetUserBUItemValue(addOrSave);
            errorInfo = ValidUserBUDate(newItem);

            if (!string.IsNullOrEmpty(errorInfo))
            {
                return errorInfo;
            }

            List<int> keyIdList = new List<int>();
            foreach (var item in newItem.FunctionSubs)
            {
                var userBUItem = systemUserBusinessGroupRepository.GetById(item.System_User_BU_UID);
                userBUItem.End_Date = item.End_Date;
                userBUItem.Modified_Date = DateTime.Now;
                userBUItem.Modified_UID = addOrSave.Modified_UID;

                systemUserBusinessGroupRepository.Update(userBUItem);
            }
            unitOfWork.Commit();
            return "Success";
        }

        private UserBUAddOrSave SetUserBUItemValue(UserBUAddOrSave addOrSave)
        {
            UserBUAddOrSave newItem = new UserBUAddOrSave();

            newItem.User_NTID = addOrSave.User_NTID;
            newItem.User_Name = addOrSave.User_Name;
            newItem.Modified_UID = addOrSave.Modified_UID;
            newItem.Modified_Date = DateTime.Now;
            //将画面上的数据插入到Model中
            var buidList = addOrSave.FunctionSubs.Select(m => m.BU_ID).Distinct().ToList();
            //从DB表中获取画面上所有BUID的信息
            var buInfoList = systemBURepository.GetMany(m => buidList.Contains(m.BU_ID)).ToList();

            //从DB表中获取画面上所有BUDID的信息
            var targetBUMUIds = buInfoList.Select(q => q.BU_M_UID);
            var budInfoList = systemBUDRepository.GetMany(m => targetBUMUIds.Contains(m.BU_M_UID)).ToList();
            List<CustomBUAndBUD> subList = new List<CustomBUAndBUD>();
            int i = 1;
            foreach (var item in addOrSave.FunctionSubs)
            {
                CustomBUAndBUD subItem = new CustomBUAndBUD();
                subItem.System_User_BU_UID = item.System_User_BU_UID;
                subItem.Begin_Date = item.Begin_Date;
                subItem.End_Date = item.End_Date;

                var buItem = buInfoList.Where(m => m.BU_ID == item.BU_ID).First();
                subItem.BU_M_UID = buItem.BU_M_UID;
                subItem.BU_BeginDate = buItem.Begin_Date;
                subItem.BU_EndDate = buItem.End_Date;
                subItem.RowNum = i++;
                if (item.BU_D_ID == ConstConstants.SelectAll)
                {
                    //获取所有匹配的BUD列表
                    var budList = budInfoList.Where(m => m.BU_M_UID == buItem.BU_M_UID).ToList();

                    foreach (var budItem in budList)
                    {
                        subItem.BU_D_UID = budItem.BU_D_UID;
                        subItem.BUD_BeginDate = budItem.Begin_Date;
                        subItem.BUD_EndDate = budItem.End_Date;
                        subList.Add(subItem);
                    }
                }
                else
                {
                    //如果BUD里面没有对应项目说明还没有建立BUD明细项
                    var budItem = budInfoList.Where(m => m.BU_D_ID == item.BU_D_ID).FirstOrDefault();
                    if (budItem != null)
                    {
                        subItem.BU_D_UID = budItem.BU_D_UID;
                    }
                    subList.Add(subItem);
                }
            }
            newItem.FunctionSubs = subList;
            return newItem;
        }

        private string ValidUserBUDate(UserBUAddOrSave addOrSave)
        {
            string errorInfo = string.Empty;
            foreach (var item in addOrSave.FunctionSubs)
            {
                if (item.BUD_BeginDate != null)
                {
                    if (item.Begin_Date < item.BUD_BeginDate)
                    {
                        errorInfo = string.Format("Incomplete context at row #{0}, Begin Date Can't less than BU Detail Begin Date.", item.RowNum);
                        break;
                    }
                }
                else
                {
                    if (item.Begin_Date < item.BU_BeginDate)
                    {
                        errorInfo = string.Format("Incomplete context at row #{0}, Begin Date Can't less than BU Master Begin Date.", item.RowNum);
                        break;

                    }
                }

                if (item.BUD_EndDate != null)
                {
                    if (item.End_Date == null)
                    {
                        errorInfo = string.Format("Incomplete context at row #{0}, End Date is not empty.", item.RowNum);
                        break;
                    }
                    if (item.End_Date > item.BUD_EndDate)
                    {
                        errorInfo = string.Format("Incomplete context at row #{0}, End Date Can't more than BU Detail End Date.", item.RowNum);
                        break;
                    }
                }
                else
                {
                    if (item.BU_EndDate != null)
                    {
                        if (item.End_Date == null)
                        {
                            errorInfo = string.Format("Incomplete context at row #{0}, End Date is not empty.", item.RowNum);
                            break;
                        }
                        if (item.End_Date > item.BU_EndDate)
                        {
                            errorInfo = string.Format("Incomplete context at row #{0}, End Date Can't less than BU Master End Date.", item.RowNum);
                            break;
                        }
                    }
                }
            }
            return errorInfo;
        }

        #endregion System_User_Business_Group implement interface--------------------End-----------Add by Rock 2015/11/18

        #endregion All of Rock's implement interface-------------------End----------------Add by Rock 2015/11/18

        #region System_Plant implement interface-------------------Add by Sidney 2015/11/12

        public PagedListModel<SystemPlantDTO> QueryPlants(PlantModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var plants = systemPlantRepository.QueryPlants(searchModel, page, out totalCount);

            IList<SystemPlantDTO> plantsDTO = new List<SystemPlantDTO>();

            foreach (var plant in plants)
            {
                var dto = AutoMapper.Mapper.Map<SystemPlantDTO>(plant);
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(plant.System_Users);
                plantsDTO.Add(dto);
            }

            return new PagedListModel<SystemPlantDTO>(totalCount, plantsDTO);
        }
        public System_Plant QueryPlantSingle(int uuid)
        {
            return systemPlantRepository.GetById(uuid);
        }
        public string ModifyPlant(System_Plant ent)
        {
            systemPlantRepository.Update(ent);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string AddPlant(System_Plant ent)
        {
            systemPlantRepository.Add(ent);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string DeletePlant(System_Plant ent)
        {
            if (systemUserPlantRepository.GetMany(p => p.System_Plant_UID == ent.System_Plant_UID).Count() != 0)
            { return "FAIL"; }
            else
            {
                systemPlantRepository.Delete(ent);
                unitOfWork.Commit();
                return "SUCCESS";
            }
        }

        public IEnumerable<SystemPlantDTO> DoExportPlant(string uuids)
        {
            var totalCount = 0;
            var plants = systemPlantRepository.QueryPlants(new PlantModelSearch { ExportUIds = uuids }, null, out totalCount);

            IList<SystemPlantDTO> plantsDTO = new List<SystemPlantDTO>();

            foreach (var plant in plants)
            {
                var dto = AutoMapper.Mapper.Map<SystemPlantDTO>(plant);
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(plant.System_Users);
                plantsDTO.Add(dto);
            }
            return plantsDTO;
        }
        public bool CheckPlantExistByUId(int uuid)
        {
            return systemPlantRepository.GetById(uuid) == null;
        }
        public bool CheckPlantExistByPlant(string plant)
        {
            return systemPlantRepository.GetMany(p => p.Plant == plant).Count() == 0;
        }

        public DateTime? GetMaxEnddate4Plant(int uid)
        {
            return systemUserPlantRepository.GetMaxEnddate4Plant(uid);
        }
        #endregion

        #region System Function Maintenance Module -------------- Add by Tonny 2015/11/12

        public PagedListModel<FunctionItem> QueryFunctions(FunctionModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var functions = systemFunctionRepository.QueryFunctions(searchModel, page, out totalCount).AsEnumerable();

            return new PagedListModel<FunctionItem>(totalCount, functions);
        }

        public string DeleteFunction(int uid)
        {
            if (systemFunctionRepository.GetById(uid) == null)
            {
                return "Record aleady deleted";
            }
            if (systemFunctionRepository.GetMany(r => r.Parent_Function_UID == uid).Count() > 0)
            {
                return "Can't delete since it is in used by other functions";
            }
            if (systemRoleFunctionRepository.GetMany(r => r.Function_UID == uid).Count() > 0)
            {
                return "Function aleady role assigned";
            }

            var subs = systemFunctionSubRepository.GetMany(f => f.Function_UID == uid);
            foreach (var item in subs)
            {
                var result = FunctionSubsCanDelete(item.System_FunctionSub_UID);
                if (result != "OK")
                {
                    return result;
                }
            }
            systemFunctionRepository.Delete(f => f.Function_UID == uid);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public System_Function QueryFunctionWithSubs(int uid)
        {
            return systemFunctionRepository.QueryFunctionWithSubs(uid);
        }

        public System_Function GetFunctionByID(string functionId)
        {
            var func = systemFunctionRepository.GetFirstOrDefault(p => p.Function_ID == functionId);
            return func;
        }

        public System_Function GetFunction(int uid)
        {
            var func = systemFunctionRepository.GetFirstOrDefault(p => p.Function_UID == uid);
            return func;
        }

        public string AddFunctionWithSubs(FunctionWithSubs vm)
        {
            var funcEntity = AutoMapper.Mapper.Map<System_Function>(vm);
            int? parentUId = null;

            //check parent_funtion_id and function_id if exist
            if (!string.IsNullOrEmpty(vm.Parent_Function_ID))
            {
                parentUId = systemFunctionRepository.GetFirstOrDefault(p => p.Function_ID == vm.Parent_Function_ID).Function_UID;
            }

            if (systemFunctionRepository.GetMany(p => p.Parent_Function_UID == parentUId && p.Function_ID == vm.Function_ID).Count() > 0)
            {
                return string.Format("item with same parent function Id [{0}] and function Id [{1}] is exist!", parentUId == null ? "NULL" : parentUId.ToString(), vm.Function_ID);
            }

            var now = DateTime.Now;
            funcEntity.Parent_Function_UID = parentUId;
            funcEntity.Modified_Date = now;
            funcEntity.URL = funcEntity.URL.ToUpper();

            foreach (var item in vm.FunctionSubs)
            {
                if (funcEntity.System_FunctionSub.Where(p => p.Sub_Fun == item.Sub_Fun).Count() > 0)
                {
                    return string.Format("Can't to add same Sub_Funs, The sub_fun is {0}", item.Sub_Fun);
                }

                var funcSub = AutoMapper.Mapper.Map<System_FunctionSub>(item);
                funcSub.Modified_UID = funcEntity.Modified_UID;
                funcSub.Modified_Date = now;
                funcSub.Sub_Fun_URL = funcSub.Sub_Fun_URL.ToUpper();
                funcEntity.System_FunctionSub.Add(funcSub);
            }

            systemFunctionRepository.Add(funcEntity);
            unitOfWork.Commit();

            return "SUCCESS";
        }

        public string ModifyFunctionWithSubs(FunctionWithSubs vm)
        {
            int? parentUId = null;
            //check parent_funtion_id and function_id if exist
            if (!string.IsNullOrEmpty(vm.Parent_Function_ID))
            {
                parentUId = systemFunctionRepository.GetFirstOrDefault(p => p.Function_ID == vm.Parent_Function_ID).Function_UID;
            }

            if (systemFunctionRepository.GetMany(p => p.Parent_Function_UID == parentUId && p.Function_ID == vm.Function_ID && p.Function_UID != vm.Function_UID).Count() > 0)
            {
                return string.Format("item with same parent function Id [{0}] and function Id [{1}] is exist!", parentUId == null ? "NULL" : parentUId.ToString(), vm.Function_ID);
            }

            var now = DateTime.Now;
            var funcEntity = systemFunctionRepository.GetFirstOrDefault(k => k.Function_UID == vm.Function_UID);
            if (funcEntity != null)
            {
                funcEntity.Function_Name = vm.Function_Name;
                funcEntity.Function_Desc = vm.Function_Desc;
                funcEntity.Function_ID = vm.Function_ID;
                funcEntity.Icon_ClassName = vm.Icon_ClassName;
                funcEntity.Is_Show = vm.Is_Show;
                funcEntity.Order_Index = vm.Order_Index;
                funcEntity.Parent_Function_UID = parentUId;
                funcEntity.URL = vm.URL.ToUpper();
                funcEntity.Mobile_URL = vm.Mobile_URL;
                funcEntity.Modified_UID = vm.Modified_UID;
                funcEntity.Modified_Date = now;
            }

            foreach (var item in vm.FunctionSubs)
            {
                var funcSub = AutoMapper.Mapper.Map<System_FunctionSub>(item);
                funcSub.Modified_UID = funcEntity.Modified_UID;
                funcSub.Modified_Date = now;

                var sub = funcEntity.System_FunctionSub.FirstOrDefault(p => p.Sub_Fun == item.Sub_Fun);

                if (sub == null)
                {
                    funcEntity.System_FunctionSub.Add(funcSub);
                }
                else
                {
                    sub.Function_UID = funcEntity.Function_UID;
                    sub.Sub_Fun_Name = item.Sub_Fun_Name;
                    sub.Sub_Fun_URL = item.Sub_Fun_URL.ToUpper();
                    sub.Modified_UID = funcEntity.Modified_UID;
                    sub.Modified_Date = now;
                }
            }

            systemFunctionRepository.Update(funcEntity);
            unitOfWork.Commit();

            return "SUCCESS";
        }

        private string FunctionSubsCanDelete(int function_sub_uid)
        {
            var entity = systemFunctionSubRepository.GetFirstOrDefault(p => p.System_FunctionSub_UID == function_sub_uid);
            if (entity == null)
            {
                return "Record aleady deleted";
            }
            if (systemRoleFunctionSubRepository.GetMany(p => p.System_FunctionSub_UID == function_sub_uid).Count() > 0)
            {
                return "Function aleady role assigned";
            }

            systemFunctionSubRepository.Delete(entity);
            return "OK";
        }

        public string DeleteSubFunction(int function_sub_uid)
        {
            var result = FunctionSubsCanDelete(function_sub_uid);
            if (result == "OK")
            {
                unitOfWork.Commit();
                return "SUCCESS";
            }
            else
            {
                return result;
            }

        }

        public IEnumerable<FunctionItem> DoExportFunction(string uids)
        {
            var totalCount = 0;
            return systemFunctionRepository
                    .QueryFunctions(new FunctionModelSearch { ExportUIds = uids }, null, out totalCount)
                    .AsEnumerable();
        }

        #endregion //End System Function Maintenance

        #region User Plant -------------- Add by Sidney 2015/11/17

        public System_Plant GetPlantInfo(string Plant)
        {
            var plantInfo = systemPlantRepository.GetFirstOrDefault(p => p.Plant == Plant);
            return plantInfo;
        }


        public List<string> getFunPOrg(string Plant)
        {
            var plantInfo = systemOrgRepository.getFunPOrg(Plant);
            return plantInfo;
        }
        public PagedListModel<UserPlantItem> QueryUserPlants(UserPlantModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var functions = systemUserPlantRepository.QueryUserPlants(searchModel, page, out totalCount).AsEnumerable();

            return new PagedListModel<UserPlantItem>(totalCount, functions);
        }

        public IQueryable<UserPlantWithPlant> QueryUserPlantByAccountUID(int uuid)
        {
            return systemUserPlantRepository.QueryUserPlantByAccountUID(uuid);
        }

        public string AddUserPlantWithSubs(UserPlantEditModel vm)
        {
            var userplants = vm.UserPlantWithPlants;

            var distinctKeys = userplants.DistinctBy(k => k.System_Plant_UID);

            foreach (var userorg in distinctKeys)
            {
                var compareUserPlants = userplants.Where(k => k.System_Plant_UID == userorg.System_Plant_UID);
                var targetPlant = systemPlantRepository.GetById(userorg.System_Plant_UID);

                //取出新增数据的最小时间
                var minBeginDate = compareUserPlants.Min(q => q.User_Plant_Begin_Date);

                var notSetEndDateCount = compareUserPlants.Count(q => q.User_Plant_End_Date == null);
                if (notSetEndDateCount > 1)
                {
                    return string.Format("some plant which id is {0} with more than one EndDate is not set!", userorg.Plant);
                }
                foreach (var newUserPlant in compareUserPlants)
                {
                    var compareResult = DateCompareHelper.CompareInterval(
                            head: new DateCompareModel { Name = "Organization", BeginDate = targetPlant.Begin_Date, EndDate = targetPlant.End_Date }
                            , sub: new DateCompareModel { Name = "User Organization", BeginDate = newUserPlant.User_Plant_Begin_Date, EndDate = newUserPlant.User_Plant_End_Date });

                    if (compareResult != "PASS")
                    {
                        return compareResult;
                    }

                    var self = new UserPlantWithPlant[] { newUserPlant };

                    var invalidInputRecords =
                        compareUserPlants.Except(self).Where(q =>
                            (q.User_Plant_Begin_Date >= newUserPlant.User_Plant_Begin_Date && q.User_Plant_End_Date <= newUserPlant.User_Plant_End_Date)
                            || (q.User_Plant_Begin_Date <= newUserPlant.User_Plant_Begin_Date && q.User_Plant_End_Date >= newUserPlant.User_Plant_Begin_Date)
                            || (q.User_Plant_Begin_Date <= newUserPlant.User_Plant_End_Date && q.User_Plant_End_Date >= newUserPlant.User_Plant_End_Date)
                            || (q.User_Plant_End_Date == null && newUserPlant.User_Plant_End_Date > q.User_Plant_Begin_Date));

                    if (invalidInputRecords.Count() > 0)
                    {
                        return string.Format("Valid date of the pending insert has coincide zone, plant is {0}, data zone are [{1} ~ {2}] and [{3} ~ {4}]",
                            newUserPlant.Plant,
                            newUserPlant.User_Plant_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                            newUserPlant.User_Plant_End_Date == null ? "Endless" : ((DateTime)newUserPlant.User_Plant_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate),
                            invalidInputRecords.First().User_Plant_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                            invalidInputRecords.First().User_Plant_End_Date == null ? "Endless" : ((DateTime)invalidInputRecords.First().User_Plant_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate));
                    }
                }

                //根据最小时间查处可能有交集的所有时间
                var dbUserPlants =
                systemUserPlantRepository.GetMany(q => q.System_Plant_UID == userorg.System_Plant_UID
                                                && q.Account_UID == vm.Account_UID
                                                && (q.End_Date >= minBeginDate || q.End_Date == null))
                                        .Select(r => new { BeginDate = r.Begin_Date, EndDate = r.End_Date });

                //如果已有设置的记录
                if (dbUserPlants.Count() > 0)
                {
                    //如果设置记录未设置Enddate，则此记录无效
                    if (dbUserPlants.Where(q => q.EndDate == null).Count() > 0 && notSetEndDateCount > 0)
                    {
                        return "some org with more than one EndDate is not set!";
                    }
                    else
                    {
                        foreach (var newUserOrg in compareUserPlants)
                        {
                            //compare date with db
                            var invalidRecords =
                                dbUserPlants.Where(q =>
                                    (q.BeginDate >= newUserOrg.User_Plant_Begin_Date && q.EndDate <= newUserOrg.User_Plant_End_Date)
                                    || (q.BeginDate <= newUserOrg.User_Plant_Begin_Date && q.EndDate >= newUserOrg.User_Plant_Begin_Date)
                                    || (q.BeginDate <= newUserOrg.User_Plant_End_Date && q.EndDate >= newUserOrg.User_Plant_End_Date)
                                    || (q.EndDate == null && newUserOrg.User_Plant_End_Date > q.BeginDate));

                            if (invalidRecords.Count() > 0)
                            {
                                return string.Format("Valid date of the pending insert has coincide zone with exist data, plant is {0}, data zone are [{1} ~ {2}] and [{3} ~ {4}]",
                                    newUserOrg.Plant,
                                    newUserOrg.User_Plant_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                                    newUserOrg.User_Plant_End_Date == null ? "Endless" : ((DateTime)newUserOrg.User_Plant_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate),
                                    invalidRecords.First().BeginDate.ToString(FormatConstants.DateTimeFormatStringByDate),
                                    invalidRecords.First().EndDate == null ? "Endless" : ((DateTime)invalidRecords.First().EndDate).ToString(FormatConstants.DateTimeFormatStringByDate));
                            }

                            //valid if exist
                            if (systemUserPlantRepository
                                    .GetMany(q => q.Account_UID == vm.Account_UID
                                            && q.System_Plant_UID == newUserOrg.System_Plant_UID
                                            && q.Begin_Date == newUserOrg.User_Plant_Begin_Date).Count() > 0)
                            {
                                return string.Format("Same data with org id [{0}], user [{1}] and Begin Date[{2}] already exist in system",
                                    newUserOrg.Plant, vm.User_Name, newUserOrg.User_Plant_Begin_Date);
                            }
                        }
                    }
                }
            }

            var now = DateTime.Now;
            foreach (var item in userplants)
            {
                var entity = new System_User_Plant();
                entity.Account_UID = vm.Account_UID;
                entity.Begin_Date = item.User_Plant_Begin_Date;
                entity.End_Date = item.User_Plant_End_Date;
                entity.System_Plant_UID = item.System_Plant_UID;
                entity.Modified_Date = now;
                entity.Modified_UID = vm.Modified_UID;
                systemUserPlantRepository.Add(entity);
            }

            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string ModifyUserPlantWithSubs(UserPlantEditModel vm)
        {
            var userplants = vm.UserPlantWithPlants;

            var distinctKeys = userplants.DistinctBy(k => k.System_Plant_UID);

            foreach (var userorg in distinctKeys)
            {
                var compareUserPlants = userplants.Where(k => k.System_Plant_UID == userorg.System_Plant_UID);
                var modifyUserPlantsIds = userplants.Where(q => q.System_User_Plant_UID > 0).Select(r => r.System_User_Plant_UID);
                var targetPlant = systemPlantRepository.GetById(userorg.System_Plant_UID);

                //取出新增数据的最小时间
                var minBeginDate = compareUserPlants.Min(q => q.User_Plant_Begin_Date);

                var notSetEndDateCount = compareUserPlants.Count(q => q.User_Plant_End_Date == null);
                if (notSetEndDateCount > 1)
                {
                    return string.Format("some plant which id is {0} with more than one EndDate is not set!", userorg.Plant);
                }
                foreach (var newUserPlant in compareUserPlants)
                {
                    var compareResult = DateCompareHelper.CompareInterval(
                            head: new DateCompareModel { Name = "Organization", BeginDate = targetPlant.Begin_Date, EndDate = targetPlant.End_Date }
                            , sub: new DateCompareModel { Name = "User Organization", BeginDate = newUserPlant.User_Plant_Begin_Date, EndDate = newUserPlant.User_Plant_End_Date });

                    if (compareResult != "PASS")
                    {
                        return compareResult;
                    }

                    var self = new UserPlantWithPlant[] { newUserPlant };

                    var invalidInputRecords =
                        compareUserPlants.Except(self).Where(q =>
                            (q.User_Plant_Begin_Date >= newUserPlant.User_Plant_Begin_Date && q.User_Plant_End_Date <= newUserPlant.User_Plant_End_Date)
                            || (q.User_Plant_Begin_Date <= newUserPlant.User_Plant_Begin_Date && q.User_Plant_End_Date >= newUserPlant.User_Plant_Begin_Date)
                            || (q.User_Plant_Begin_Date <= newUserPlant.User_Plant_End_Date && q.User_Plant_End_Date >= newUserPlant.User_Plant_End_Date)
                            || (q.User_Plant_End_Date == null && newUserPlant.User_Plant_End_Date > q.User_Plant_Begin_Date));

                    if (invalidInputRecords.Count() > 0)
                    {
                        return string.Format("Valid date of the pending insert has coincide zone, plant is {0}, data zone are [{1} ~ {2}] and [{3} ~ {4}]",
                            newUserPlant.Plant,
                            newUserPlant.User_Plant_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                            newUserPlant.User_Plant_End_Date == null ? "Endless" : ((DateTime)newUserPlant.User_Plant_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate),
                            invalidInputRecords.First().User_Plant_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                            invalidInputRecords.First().User_Plant_End_Date == null ? "Endless" : ((DateTime)invalidInputRecords.First().User_Plant_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate));
                    }
                }

                //根据最小时间查处可能有交集的所有时间
                var dbUserPlants =
                systemUserPlantRepository.GetMany(q => q.System_Plant_UID == userorg.System_Plant_UID
                                                && q.Account_UID == vm.Account_UID
                                                && (q.End_Date >= minBeginDate || q.End_Date == null)).AsEnumerable();

                dbUserPlants = dbUserPlants.Where(q => !modifyUserPlantsIds.Contains(q.System_User_Plant_UID));

                //如果已有设置的记录
                if (dbUserPlants.Count() > 0)
                {
                    //如果设置记录未设置Enddate，则此记录无效
                    if (dbUserPlants.Where(q => q.End_Date == null).Count() > 0 && notSetEndDateCount > 0)
                    {
                        return "some org with more than one EndDate is not set!";
                    }
                    else
                    {
                        foreach (var newUserOrg in compareUserPlants)
                        {
                            //compare date with db
                            var invalidRecords =
                                dbUserPlants.Where(q =>
                                    (q.Begin_Date >= newUserOrg.User_Plant_Begin_Date && q.End_Date <= newUserOrg.User_Plant_End_Date)
                                    || (q.Begin_Date <= newUserOrg.User_Plant_Begin_Date && q.End_Date >= newUserOrg.User_Plant_Begin_Date)
                                    || (q.Begin_Date <= newUserOrg.User_Plant_End_Date && q.End_Date >= newUserOrg.User_Plant_End_Date)
                                    || (q.End_Date == null && newUserOrg.User_Plant_End_Date > q.Begin_Date));

                            if (invalidRecords.Count() > 0)
                            {
                                return string.Format("Valid date of the pending insert has coincide zone with exist data, plant is {0}, data zone are [{1} ~ {2}] and [{3} ~ {4}]",
                                    newUserOrg.Plant,
                                    newUserOrg.User_Plant_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                                    newUserOrg.User_Plant_End_Date == null ? "Endless" : ((DateTime)newUserOrg.User_Plant_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate),
                                    invalidRecords.First().Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                                    invalidRecords.First().End_Date == null ? "Endless" : ((DateTime)invalidRecords.First().End_Date).ToString(FormatConstants.DateTimeFormatStringByDate));
                            }

                            //valid if exist
                            if (systemUserPlantRepository
                                    .GetMany(q => q.Account_UID == vm.Account_UID
                                            && q.System_Plant_UID == newUserOrg.System_Plant_UID
                                            && q.Begin_Date == newUserOrg.User_Plant_Begin_Date).Count() > 0)
                            {
                                return string.Format("Same data with org id [{0}], user [{1}] and Begin Date[{2}] already exist in system",
                                    newUserOrg.Plant, vm.User_Name, newUserOrg.User_Plant_Begin_Date);
                            }
                        }
                    }
                }
            }

            var now = DateTime.Now;
            foreach (var item in userplants)
            {
                if (item.System_User_Plant_UID == 0)
                {
                    var entity = new System_User_Plant();
                    entity.Account_UID = vm.Account_UID;
                    entity.Begin_Date = item.User_Plant_Begin_Date;
                    entity.End_Date = item.User_Plant_End_Date;
                    entity.System_Plant_UID = item.System_Plant_UID;
                    entity.Modified_Date = now;
                    entity.Modified_UID = vm.Modified_UID;
                    systemUserPlantRepository.Add(entity);
                }
                else
                {
                    var entity = systemUserPlantRepository.GetById(item.System_User_Plant_UID);
                    entity.End_Date = item.User_Plant_End_Date;
                    entity.Modified_Date = now;
                    entity.Modified_UID = vm.Modified_UID;
                    systemUserPlantRepository.Update(entity);
                }
            }

            unitOfWork.Commit();
            return "SUCCESS";
        }

        public IEnumerable<UserPlantItem> DoExportUserPlant(string uids)
        {
            var totalCount = 0;
            return systemUserPlantRepository.QueryUserPlants(new UserPlantModelSearch { ExportUIds = uids }, null, out totalCount)
                    .AsEnumerable();
        }
        #endregion

        #region Role Function Setting Module -------------- Add by Tonny 2015/11/18

        public PagedListModel<RoleFunctionItem> QueryRoleFunctions(RoleFunctionSearchModel searchModel, Page page)
        {
            var totalCount = 0;
            var roleFunctions = systemRoleFunctionRepository.QueryRoleFunctions(searchModel, page, out totalCount).AsEnumerable();

            return new PagedListModel<RoleFunctionItem>(totalCount, roleFunctions);
        }

        public RoleFunctionsWithSub QueryRoleFunction(int uid)
        {
            var result = new RoleFunctionsWithSub();
            var role = systemRoleRepository.QueryRoleFunctionsByRoleUID(uid);

            result.Role_UID = role.Role_UID;
            result.Role_ID = role.Role_ID;
            result.Role_Name = role.Role_Name;

            foreach (var item in role.System_Role_Function)
            {
                result.HeadFunctions.Add(new HeadFunction
                {
                    System_Role_Function_UID = item.System_Role_Function_UID,
                    Function_UID = item.System_Function.Function_UID,
                    Function_ID = item.System_Function.Function_ID,
                    Function_Name = item.System_Function.Function_Name,
                    Is_Show = item.System_Function.Is_Show,
                    Order_Index = item.System_Function.Order_Index,
                    URL = item.System_Function.URL,
                    Is_Show_Role = item.Is_Show
                });
            }

            //fill subFun for first head function
            if (result.HeadFunctions.Count() > 0)
            {
                var firstFunction = result.HeadFunctions.First();
                var firstSub = systemRoleFunctionSubRepository.QueryRoleSubFunctionsByRoleUIDAndFunctionUID(uid, firstFunction.Function_UID);
                firstFunction.SubFun = firstSub.ToList();
            }
            return result;
        }

        public IEnumerable<SubFunction> QueryRoleSubFunctions(int roleUId, int functionUId)
        {
            var query = systemRoleFunctionSubRepository.QueryRoleSubFunctionsByRoleUIDAndFunctionUID(roleUId, functionUId);
            return query;
        }

        public IEnumerable<SubFunction> QuerySubFunctionsByFunctionUID(int functionUId)
        {
            return AutoMapper.Mapper.Map<IEnumerable<SubFunction>>(systemFunctionSubRepository.GetMany(q => q.Function_UID == functionUId));
        }

        public void DeleteRoleFunction(int uid)
        {
            systemRoleFunctionSubRepository.Delete(p => p.System_Role_Function_UID == uid);
            systemRoleFunctionRepository.Delete(p => p.System_Role_Function_UID == uid);
            unitOfWork.Commit();
        }

        public string MaintainRoleFunctionWithSubs(RoleFunctionsWithSub vm, int AccountUId)
        {
            var roleUId = vm.Role_UID;
            var now = DateTime.Now;
            if (vm.Modified_UID == 0)
                vm.Modified_UID = AccountUId;

            foreach (var func in vm.HeadFunctions)
            {
                var queryFunc = systemRoleFunctionRepository.GetFirstOrDefault(p => p.Function_UID == func.Function_UID && p.Role_UID == roleUId);
                if (queryFunc != null)
                {
                    #region 更新已存在项
                    queryFunc.Is_Show = func.Is_Show_Role;
                    queryFunc.Modified_UID = vm.Modified_UID;
                    queryFunc.Modified_Date = now;

                    systemRoleFunctionRepository.Update(queryFunc);

                    foreach (var sub in func.SubFun)
                    {
                        if (sub.System_Role_FunctionSub_UID > 0)
                        {
                            var entity = systemRoleFunctionSubRepository.GetById(sub.System_Role_FunctionSub_UID);
                            entity.Sub_Flag = sub.Grant;
                            entity.Modified_UID = vm.Modified_UID;
                            entity.Modified_Date = now;

                            systemRoleFunctionSubRepository.Update(entity);
                        }
                        else
                        {
                            systemRoleFunctionSubRepository.Add(new System_Role_FunctionSub
                            {
                                System_Role_Function_UID = queryFunc.System_Role_Function_UID,
                                Sub_Flag = sub.Grant,
                                System_FunctionSub_UID = sub.System_FunctionSub_UID,
                                Modified_UID = vm.Modified_UID,
                                Modified_Date = now
                            });
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 新增主项
                    var newFunc = new System_Role_Function
                    {
                        Role_UID = roleUId,
                        Function_UID = func.Function_UID,
                        Is_Show = func.Is_Show_Role,
                        Modified_UID = vm.Modified_UID,
                        Modified_Date = now
                    };


                    foreach (var sub in func.SubFun)
                    {
                        newFunc.System_Role_FunctionSub.Add(new System_Role_FunctionSub
                        {
                            Sub_Flag = sub.Grant,
                            System_FunctionSub_UID = sub.System_FunctionSub_UID,
                            Modified_UID = vm.Modified_UID,
                            Modified_Date = now
                        });
                    }

                    systemRoleFunctionRepository.Add(newFunc);
                    #endregion
                }

            }
            try
            {
                unitOfWork.Commit();
            }
            catch (Exception e)
            {

            }
            return "SUCCESS";
        }

        public IEnumerable<RoleFunctionItem> DoExportRoleFunctions(string uids)
        {
            var totalCount = 0;
            return systemRoleFunctionRepository
                    .QueryRoleFunctions(new RoleFunctionSearchModel { ExportUIds = uids }, null, out totalCount)
                    .AsEnumerable();
        }

        #endregion
        #region System_User_Role Setting -------------------Add by Allen 2015/11/16
        public PagedListModel<UserRoleItem> QueryUserRoles(UserRoleSearchModel searchModel, Page page)
        {
            var totalCount = 0;
            var userroles = systemUserRoleRepository.QueryUserRoles(searchModel, page, out totalCount);
            return new PagedListModel<UserRoleItem>(totalCount, userroles);
        }

        public IEnumerable<UserRoleItem> DoExportUserRole(string uruids)
        {
            var totalCount = 0;
            return systemUserRoleRepository
                    .QueryUserRoles(new UserRoleSearchModel { ExportUIds = uruids }, null, out totalCount)
                    .AsEnumerable();
        }

        public System_User_Role QueryUserRolesSingle(int uruid)
        {
            return systemUserRoleRepository.GetFirstOrDefault(r => r.System_User_Role_UID == uruid);
        }

        public void AddUserRole(System_User_Role ent)
        {
            systemUserRoleRepository.Add(ent);
            unitOfWork.Commit();
        }
        /// <summary>
        /// ?
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public bool DeleteUserRole(System_User_Role ent)
        {
            if (systemUserRoleRepository.GetMany(r => r.System_User_Role_UID == ent.System_User_Role_UID).Count() != 0)
            {
                systemUserRoleRepository.Delete(ent);
                unitOfWork.Commit();
                return true;
            }
            return false;
        }

        public System_User_Role GetSystemUserRole(int accountUId, int roleUId)
        {
            return systemUserRoleRepository.GetFirstOrDefault(q => q.Account_UID == accountUId && q.Role_UID == roleUId);
        }

        /// <summary>
        ///檢查User NTID
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public System_Role GetRoleNameById(string roleid)
        {
            var userroleid = systemRoleRepository.GetFirstOrDefault(p => p.Role_ID == roleid);
            return userroleid;
        }
        #endregion //System_User_Role Module

        #region OrgBom by justin
        public PagedListModel<SystemOrgAndBomDTO> QueryOrgBom(OrgBomModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var OrgBoms = systemOrgBomRepository.QueryOrgBoms(searchModel, page, out totalCount);

            return new PagedListModel<SystemOrgAndBomDTO>(totalCount, OrgBoms);
        }

        public IEnumerable<SystemOrgAndBomDTO> DoExportOrgBom(string uuids)
        {
            var totalCount = 0;
            var OrgBoms = systemOrgBomRepository.QueryOrgBoms(new OrgBomModelSearch { ExportUIds = uuids }, null, out totalCount);

            return OrgBoms;
        }

        public SystemOrgAndBomDTO QueryOrgBom(int uid)
        {
            return systemOrgBomRepository.QueryOrgBom(uid);
        }

        public string AddOrgBom(System_OrganizationBOM ent)
        {
            ///保存时候验证bom关系的有效时间在child ORG的时间中， child org id 输入时候，需要先现在子ORG有效时间是否在父ORG时间中
            var childOrg = systemOrgRepository.GetById(ent.ChildOrg_UID);

            var compareResult = DateCompareHelper.CompareInterval(
               head: new DateCompareModel { Name = "Child Organization", BeginDate = childOrg.Begin_Date, EndDate = childOrg.End_Date }
               , sub: new DateCompareModel { Name = "Organization BOM", BeginDate = ent.Begin_Date, EndDate = ent.End_Date });

            if (compareResult != "PASS")
            {
                return compareResult;
            }

            if (ent.ParentOrg_UID != null)
            {
                var parentOrg = systemOrgRepository.GetById((int)ent.ParentOrg_UID);

                compareResult = DateCompareHelper.CompareInterval(
                    head: new DateCompareModel { Name = "Parent Organization", BeginDate = parentOrg.Begin_Date, EndDate = parentOrg.End_Date }
                    , sub: new DateCompareModel { Name = "Organization BOM", BeginDate = ent.Begin_Date, EndDate = ent.End_Date });

                if (compareResult != "PASS")
                {
                    return compareResult;
                }
            }

            systemOrgBomRepository.Add(ent);

            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string ModifyOrgBom(SystemOrgBomDTO dto)
        {
            var ent = systemOrgBomRepository.GetById(dto.OrganizationBOM_UID);
            ent.ParentOrg_UID = dto.ParentOrg_UID;
            ent.ChildOrg_UID = dto.ChildOrg_UID;
            if (ent.End_Date == null && dto.End_Date != null) { ent.End_Date = dto.End_Date; };

            var childOrg = systemOrgRepository.GetById(ent.ChildOrg_UID);

            var compareResult = DateCompareHelper.CompareInterval(
               head: new DateCompareModel { Name = "Child Organization", BeginDate = childOrg.Begin_Date, EndDate = childOrg.End_Date }
               , sub: new DateCompareModel { Name = "Organization BOM", BeginDate = ent.Begin_Date, EndDate = ent.End_Date });

            if (compareResult != "PASS")
            {
                return compareResult;
            }

            if (ent.ParentOrg_UID != null)
            {
                var parentOrg = systemOrgRepository.GetById((int)ent.ParentOrg_UID);

                compareResult = DateCompareHelper.CompareInterval(
                    head: new DateCompareModel { Name = "Parent Organization", BeginDate = parentOrg.Begin_Date, EndDate = parentOrg.End_Date }
                    , sub: new DateCompareModel { Name = "Organization BOM", BeginDate = ent.Begin_Date, EndDate = ent.End_Date });

                if (compareResult != "PASS")
                {
                    return compareResult;
                }
            }

            ent.Modified_Date = DateTime.Now;
            ent.Modified_UID = dto.Modified_UID;
            ent.Order_Index = dto.Order_Index;

            systemOrgBomRepository.Update(ent);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public bool CheckOrgBomExist(int uid, int parentUId, int childUId, int index)
        {
            int? pUid = null;
            if (parentUId > 0) { pUid = parentUId; }

            if (uid == 0)
            {
                return systemOrgBomRepository
                            .GetMany(
                                q => q.ChildOrg_UID == childUId
                                && q.Order_Index == index
                                && q.ParentOrg_UID == pUid)
                            .Count() > 0;
            }
            else
            {
                return systemOrgBomRepository
                            .GetMany(
                                q => q.OrganizationBOM_UID != uid
                                && q.ChildOrg_UID == childUId
                                && q.Order_Index == index
                                && q.ParentOrg_UID == pUid)
                            .Count() > 0;
            }
        }

        #endregion

        #region Org by Justin
        public PagedListModel<SystemOrgDTO> QueryOrgs(OrgModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var Orgs = systemOrgRepository.QueryOrgs(searchModel, page, out totalCount);

            return new PagedListModel<SystemOrgDTO>(totalCount, Orgs);
        }

        public IEnumerable<SystemOrgDTO> DoExportOrg(string uids)
        {
            var totalCount = 0;
            var Orgs = systemOrgRepository.QueryOrgs(new OrgModelSearch { ExportUIds = uids }, null, out totalCount);
            return Orgs;
        }

        public System_Organization QueryOrgSingle(int uuid)
        {
            return systemOrgRepository.GetById(uuid);
        }

        public void AddOrg(System_Organization ent)
        {
            systemOrgRepository.Add(ent);
            unitOfWork.Commit();
        }

        public void ModifyOrg(System_Organization ent)
        {
            systemOrgRepository.Update(ent);
            unitOfWork.Commit();
        }

        public bool DeleteOrg(int uid)
        {
            if (systemOrgBomRepository.GetMany(q => q.ChildOrg_UID == uid || q.ParentOrg_UID == uid).Count() > 0)
            {
                return false;
            }

            if (systemUserOrgRepository.GetMany(q => q.Organization_UID == uid).Count() > 0)
            {
                return false;
            }

            systemOrgRepository.Delete(k => k.Organization_UID == uid);
            unitOfWork.Commit();
            return true;
        }

        public bool CheckOrgExistByIdAndName(string id, string name)
        {
            return systemOrgRepository.GetMany(p => p.Organization_ID == id && p.Organization_Name == name).Count() > 0;
        }

        public bool CheckOrgExistByIdAndNameWithUId(int uid, string id, string name)
        {
            return systemOrgRepository.GetMany(p => p.Organization_ID == id && p.Organization_Name == name && p.Organization_UID != uid).Count() > 0;
        }

        public DateTime? GetMaxEnddate4Org(int orgUId)
        {
            return systemOrgBomRepository.GetMaxEnddate4Org(orgUId);
        }
        #endregion

        #region User Organization by Justin
        public IEnumerable<System_Organization> GetOrgInfo(string OrgID)
        {
            var orglist = systemOrgRepository.GetMany(p => p.Organization_ID == OrgID);
            return orglist;
        }

        public PagedListModel<UserOrgItem> QueryUserOrgs(UserOrgModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var functions = systemUserOrgRepository.QueryUserOrgs(searchModel, page, out totalCount).AsEnumerable();

            return new PagedListModel<UserOrgItem>(totalCount, functions);
        }

        public IQueryable<UserOrgWithOrg> QueryUserOrgsByAccountUID(int uuid)
        {
            return systemUserOrgRepository.QueryUserOrgsByAccountUID(uuid);
        }

        public string AddUserOrgWithSubs(UserOrgEditModel vm)
        {
            var userorgs = vm.UserOrgWithOrgs;

            var distinctKeys = userorgs.DistinctBy(k => k.Organization_UID);

            foreach (var userorg in distinctKeys)
            {
                var compareUserOrgs = userorgs.Where(k => k.Organization_UID == userorg.Organization_UID);
                var targetOrg = systemOrgRepository.GetById(userorg.Organization_UID);

                //取出新增数据的最小时间
                var minBeginDate = compareUserOrgs.Min(q => q.UserOrg_Begin_Date);

                var notSetEndDateCount = compareUserOrgs.Count(q => q.UserOrg_End_Date == null);
                if (notSetEndDateCount > 1)
                {
                    return string.Format("some orginaztion which id is {0} with more than one EndDate is not set!", userorg.Organization_ID);
                }
                foreach (var newUserOrg in compareUserOrgs)
                {
                    var compareResult = DateCompareHelper.CompareInterval(
                            head: new DateCompareModel { Name = "Organization", BeginDate = targetOrg.Begin_Date, EndDate = targetOrg.End_Date }
                            , sub: new DateCompareModel { Name = "User Organization", BeginDate = newUserOrg.UserOrg_Begin_Date, EndDate = newUserOrg.UserOrg_End_Date });

                    if (compareResult != "PASS")
                    {
                        return compareResult;
                    }

                    var self = new UserOrgWithOrg[] { newUserOrg };

                    var invalidInputRecords =
                        compareUserOrgs.Except(self).Where(q =>
                            (q.UserOrg_Begin_Date >= newUserOrg.UserOrg_Begin_Date && q.UserOrg_End_Date <= newUserOrg.UserOrg_End_Date)
                            || (q.UserOrg_Begin_Date <= newUserOrg.UserOrg_Begin_Date && q.UserOrg_End_Date >= newUserOrg.UserOrg_Begin_Date)
                            || (q.UserOrg_Begin_Date <= newUserOrg.UserOrg_End_Date && q.UserOrg_End_Date >= newUserOrg.UserOrg_End_Date)
                            || (q.UserOrg_End_Date == null && newUserOrg.UserOrg_End_Date > q.UserOrg_Begin_Date));

                    if (invalidInputRecords.Count() > 0)
                    {
                        return string.Format("Valid date of the pending insert has coincide zone, org id is {0}, data zone are [{1} ~ {2}] and [{3} ~ {4}]",
                            newUserOrg.Organization_ID,
                            newUserOrg.UserOrg_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                            newUserOrg.UserOrg_End_Date == null ? "Endless" : ((DateTime)newUserOrg.UserOrg_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate),
                            invalidInputRecords.First().UserOrg_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                            invalidInputRecords.First().UserOrg_End_Date == null ? "Endless" : ((DateTime)invalidInputRecords.First().UserOrg_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate));
                    }
                }

                //根据最小时间查处可能有交集的所有时间
                var dbUserOrgs =
                systemUserOrgRepository.GetMany(q => q.Organization_UID == userorg.Organization_UID
                                                && q.Account_UID == vm.Account_UID
                                                && (q.End_Date >= minBeginDate || q.End_Date == null))
                                        .Select(r => new { BeginDate = r.Begin_Date, EndDate = r.End_Date });

                //如果已有设置的记录
                if (dbUserOrgs.Count() > 0)
                {
                    //如果设置记录未设置Enddate，则此记录无效
                    if (dbUserOrgs.Where(q => q.EndDate == null).Count() > 0 && notSetEndDateCount > 0)
                    {
                        return "some org with more than one EndDate is not set!";
                    }
                    else
                    {
                        foreach (var newUserOrg in compareUserOrgs)
                        {
                            //compare date with db
                            var invalidRecords =
                                dbUserOrgs.Where(q =>
                                    (q.BeginDate >= newUserOrg.UserOrg_Begin_Date && q.EndDate <= newUserOrg.UserOrg_End_Date)
                                    || (q.BeginDate <= newUserOrg.UserOrg_Begin_Date && q.EndDate >= newUserOrg.UserOrg_Begin_Date)
                                    || (q.BeginDate <= newUserOrg.UserOrg_End_Date && q.EndDate >= newUserOrg.UserOrg_End_Date)
                                    || (q.EndDate == null && newUserOrg.UserOrg_End_Date > q.BeginDate));

                            if (invalidRecords.Count() > 0)
                            {
                                return string.Format("Valid date of the pending insert has coincide zone with exist data, org id is {0}, data zone are [{1} ~ {2}] and [{3} ~ {4}]",
                                    newUserOrg.Organization_ID,
                                    newUserOrg.UserOrg_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                                    newUserOrg.UserOrg_End_Date == null ? "Endless" : ((DateTime)newUserOrg.UserOrg_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate),
                                    invalidRecords.First().BeginDate.ToString(FormatConstants.DateTimeFormatStringByDate),
                                    invalidRecords.First().EndDate == null ? "Endless" : ((DateTime)invalidRecords.First().EndDate).ToString(FormatConstants.DateTimeFormatStringByDate));
                            }

                            //valid if exist
                            if (systemUserOrgRepository
                                    .GetMany(q => q.Account_UID == vm.Account_UID
                                            && q.Organization_UID == newUserOrg.Organization_UID
                                            && q.Begin_Date == newUserOrg.UserOrg_Begin_Date).Count() > 0)
                            {
                                return string.Format("Same data with org id [{0}], user [{1}] and Begin Date[{2}] already exist in system",
                                    newUserOrg.Organization_ID, vm.User_Name, newUserOrg.UserOrg_Begin_Date);
                            }
                        }
                    }
                }
            }

            var now = DateTime.Now;

            foreach (var item in userorgs)
            {
                var newUserOrg = new System_UserOrg();

                newUserOrg.Account_UID = vm.Account_UID;
                newUserOrg.Begin_Date = item.UserOrg_Begin_Date;
                newUserOrg.End_Date = item.UserOrg_End_Date;
                newUserOrg.Organization_UID = item.Organization_UID;
                newUserOrg.Modified_Date = now;
                newUserOrg.Modified_UID = vm.Modified_UID;
                systemUserOrgRepository.Add(newUserOrg);
            }

            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string ModifyUserOrgWithSubs(UserOrgEditModel vm)
        {
            var userorgs = vm.UserOrgWithOrgs;

            var distinctKeys = userorgs.DistinctBy(k => k.Organization_UID);
            var modifyUserOrgsIds = userorgs.Where(q => q.System_UserOrgUID > 0).Select(r => r.System_UserOrgUID);

            foreach (var userorg in distinctKeys)
            {
                var compareUserOrgs = userorgs.Where(k => k.Organization_UID == userorg.Organization_UID);
                var targetOrg = systemOrgRepository.GetById(userorg.Organization_UID);

                //取出新增数据的最小时间
                var minBeginDate = compareUserOrgs.Min(q => q.UserOrg_Begin_Date);

                var notSetEndDateCount = compareUserOrgs.Count(q => q.UserOrg_End_Date == null);
                if (notSetEndDateCount > 1)
                {
                    return string.Format("some orginaztion witch id is {0} with more than one EndDate is not set!", userorg.Organization_ID);
                }
                foreach (var newUserOrg in compareUserOrgs)
                {
                    var compareResult = DateCompareHelper.CompareInterval(
                        head: new DateCompareModel { Name = "Organization", BeginDate = targetOrg.Begin_Date, EndDate = targetOrg.End_Date }
                        , sub: new DateCompareModel { Name = "User Organization", BeginDate = newUserOrg.UserOrg_Begin_Date, EndDate = newUserOrg.UserOrg_End_Date });

                    if (compareResult != "PASS")
                    {
                        return compareResult;
                    }

                    var self = new UserOrgWithOrg[] { newUserOrg };

                    var invalidInputRecords =
                        compareUserOrgs.Except(self).Where(q =>
                            (q.UserOrg_Begin_Date >= newUserOrg.UserOrg_Begin_Date && q.UserOrg_End_Date <= newUserOrg.UserOrg_End_Date)
                            || (q.UserOrg_Begin_Date <= newUserOrg.UserOrg_Begin_Date && q.UserOrg_End_Date >= newUserOrg.UserOrg_Begin_Date)
                            || (q.UserOrg_Begin_Date <= newUserOrg.UserOrg_End_Date && q.UserOrg_End_Date >= newUserOrg.UserOrg_End_Date)
                            || (q.UserOrg_End_Date == null && newUserOrg.UserOrg_End_Date > q.UserOrg_Begin_Date));

                    if (invalidInputRecords.Count() > 0)
                    {
                        return string.Format("Valid date of the pending insert has coincide zone, org id is {0}, data zone are [{1} ~ {2}] and [{3} ~ {4}]",
                            newUserOrg.Organization_ID,
                            newUserOrg.UserOrg_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                            newUserOrg.UserOrg_End_Date == null ? "Endless" : ((DateTime)newUserOrg.UserOrg_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate),
                            invalidInputRecords.First().UserOrg_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                            invalidInputRecords.First().UserOrg_End_Date == null ? "Endless" : ((DateTime)invalidInputRecords.First().UserOrg_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate));
                    }
                }

                //根据最小时间查处可能有交集的所有时间
                var dbUserOrgs =
                systemUserOrgRepository.GetMany(q => q.Organization_UID == userorg.Organization_UID
                                                && q.Account_UID == vm.Account_UID
                                                && (q.End_Date >= minBeginDate || q.End_Date == null)).AsEnumerable();

                dbUserOrgs = dbUserOrgs.Where(q => !modifyUserOrgsIds.Contains(q.System_UserOrgUID));

                //如果已有设置的记录
                if (dbUserOrgs.Count() > 0)
                {
                    //如果设置记录未设置Enddate，则此记录无效
                    if (dbUserOrgs.Where(q => q.End_Date == null).Count() > 0 && notSetEndDateCount > 0)
                    {
                        return "some org with more than one EndDate is not set!";
                    }
                    else
                    {
                        foreach (var newUserOrg in compareUserOrgs)
                        {
                            //compare date with db
                            var invalidRecords =
                                dbUserOrgs.Where(q =>
                                    (q.Begin_Date >= newUserOrg.UserOrg_Begin_Date && q.End_Date <= newUserOrg.UserOrg_End_Date)
                                    || (q.Begin_Date <= newUserOrg.UserOrg_Begin_Date && q.End_Date >= newUserOrg.UserOrg_Begin_Date)
                                    || (q.Begin_Date <= newUserOrg.UserOrg_End_Date && q.End_Date >= newUserOrg.UserOrg_End_Date)
                                    || (q.End_Date == null && newUserOrg.UserOrg_End_Date > q.Begin_Date));

                            if (invalidRecords.Count() > 0)
                            {
                                return string.Format("Valid date of the pending insert has coincide zone with exist data, org id is {0}, data zone are [{1} ~ {2}] and [{3} ~ {4}]",
                                    newUserOrg.Organization_ID,
                                    newUserOrg.UserOrg_Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                                    newUserOrg.UserOrg_End_Date == null ? "Endless" : ((DateTime)newUserOrg.UserOrg_End_Date).ToString(FormatConstants.DateTimeFormatStringByDate),
                                    invalidRecords.First().Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate),
                                    invalidRecords.First().End_Date == null ? "Endless" : ((DateTime)invalidRecords.First().End_Date).ToString(FormatConstants.DateTimeFormatStringByDate));
                            }

                            //valid if exist
                            if (newUserOrg.System_UserOrgUID == 0)
                            {
                                if (systemUserOrgRepository
                                    .GetMany(q => q.Account_UID == vm.Account_UID
                                            && q.Organization_UID == newUserOrg.Organization_UID
                                            && q.Begin_Date == newUserOrg.UserOrg_Begin_Date).Count() > 0)
                                {
                                    return string.Format("Same data with org id [{0}], user [{1}] and Begin Date[{2}] already exist in system",
                                        newUserOrg.Organization_ID, vm.User_Name, newUserOrg.UserOrg_Begin_Date);
                                }
                            }
                        }
                    }
                }
            }

            var now = DateTime.Now;

            foreach (var item in userorgs)
            {
                if (item.System_UserOrgUID > 0)
                {
                    var entity = systemUserOrgRepository.GetById(item.System_UserOrgUID);
                    entity.End_Date = item.UserOrg_End_Date;
                    entity.Modified_Date = now;
                    entity.Modified_UID = vm.Modified_UID;
                    systemUserOrgRepository.Update(entity);
                }
                else
                {
                    var newUserOrg = new System_UserOrg();

                    newUserOrg.Account_UID = vm.Account_UID;
                    newUserOrg.Begin_Date = item.UserOrg_Begin_Date;
                    newUserOrg.End_Date = item.UserOrg_End_Date;
                    newUserOrg.Organization_UID = item.Organization_UID;
                    newUserOrg.Modified_Date = now;
                    newUserOrg.Modified_UID = vm.Modified_UID;
                    systemUserOrgRepository.Add(newUserOrg);
                }
            }

            unitOfWork.Commit();
            return "SUCCESS";
        }

        public IEnumerable<UserOrgItem> DoExportUserOrg(string uids)
        {
            var totalCount = 0;
            return systemUserOrgRepository.QueryUserOrgs(new UserOrgModelSearch { ExportUIds = uids }, null, out totalCount)
                    .AsEnumerable();
        }
        #endregion

        #region FuncPlant Maintanance--------------------------------Add By Sidney
        public string AddFuncPlant(System_Function_Plant ent)
        {
            systemFunctionPlantRepository.Add(ent);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public System_Plant GetPlantByPlant(string Plant)
        {
            return systemPlantRepository.GetPlantByPlant(Plant);
        }

        //Add by Rock----------------------------------------- 2016-06-06------start
        public List<SystemFunctionPlantDTO> GetAllFuncPlants()
        {
            var list = systemFunctionPlantRepository.GetAll().ToList();
            var dto = AutoMapper.Mapper.Map<List<SystemFunctionPlantDTO>>(list);
            return dto;
        }

        //Add by Rock----------------------------------------- 2016-06-06------end



        public PagedListModel<FuncPlantMaintanance> QueryFuncPlants(FuncPlantSearchModel search, Page page)
        {
            var totalCount = 0;
            var plants = systemFunctionPlantRepository.QueryFuncPlants(search, page, out totalCount);

            IList<FuncPlantMaintanance> plantsDTO = new List<FuncPlantMaintanance>();

            foreach (var plant in plants)
            {
                var dto = AutoMapper.Mapper.Map<FuncPlantMaintanance>(plant);
                plantsDTO.Add(dto);
            }

            return new PagedListModel<FuncPlantMaintanance>(totalCount, plantsDTO);
        }

        public List<string> GetPlantSingle()
        {
            var allentity = systemPlantRepository.GetAll();
            var entity = allentity.Where(p => p.Name_0 == "Metal Chengdu").Select(p => p.Plant).Distinct().ToList();
            return entity;
        }

        public FuncPlantMaintanance QueryFuncPlant(int uuid)
        {
            var result = systemFunctionPlantRepository.QueryFuncPlant(uuid);
            return result;
        }
        public string DeleteFuncPlant(int uuid)
        {
            try
            {
                if (systemUserFunPlantRepository.checkFuncPlantIsExit(uuid) > 0)
                    return "FuncPlant_Is_Exit";
                else
                {
                    var result = systemFunctionPlantRepository.GetById(uuid);
                    systemFunctionPlantRepository.Delete(result);
                    unitOfWork.Commit();
                    return "SUCCESS";
                }
            }
            catch (Exception)
            {

                return "FAIL";
            }

        }

        public System_Function_Plant QueryFuncPlantSingle(int uuid)
        {
            return systemFunctionPlantRepository.GetById(uuid);
        }

        public string ModifyFuncPlant(System_Function_Plant ent)
        {
            try
            {
                systemFunctionPlantRepository.Update(ent);
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {
                return e.ToString();

            }

        }

        public IEnumerable<FuncPlantMaintanance> DoExportFuncPlant(string uuids)
        {
            var totalCount = 0;
            var plants = systemFunctionPlantRepository.QueryFuncPlants(new FuncPlantSearchModel() { ExportUIds = uuids }, null, out totalCount);
            return plants;

        }

        #endregion

        #region User FuncPlant Setting-----------------------Add By Sidney
        public PagedListModel<UserFuncPlantVM> QueryUserFuncPlants(UserFuncPlantSearch search, Page page)
        {
            var totalCount = 0;
            var plants = systemUserFunPlantRepository.QueryUserFuncPlants(search, page, out totalCount);

            IList<UserFuncPlantVM> plantsDTO = new List<UserFuncPlantVM>();

            foreach (var plant in plants)
            {
                var dto = AutoMapper.Mapper.Map<UserFuncPlantVM>(plant);
                plantsDTO.Add(dto);
            }

            return new PagedListModel<UserFuncPlantVM>(totalCount, plantsDTO);
        }

        public List<string> GetFuncPlant(string Plant, string OP_Types)
        {
            var funcPlant = systemFunctionPlantRepository.GetFuncPlantByPlantAndOPType(Plant, OP_Types);
            return funcPlant;
        }

        public string GetUserInfo(string User_NTID)
        {
            var UserInfo = systemUserRepository.GetUserInfoByNTID(User_NTID);
            var User = UserInfo.FirstOrDefault();
            if (User != null)
            {
                var User_Name = User.User_Name;
                return User_Name;
            }
            else
            {
                return "";
            }
        }

        public string EditUserFuncPlantWithSubs(EditUserFuncPlant vm)
        {
            var userplants = vm.UserPlantWithPlants;

            var distinctKeys = userplants.DistinctBy(k => k.User_NTID);
            //Delete掉删除的User
            var DB_NTID = systemUserFunPlantRepository.CheckUserFuncPlantIsExit(vm.FuncPlant, "").Select(p => p.System_Users.User_NTID);
            var Page_NTID = userplants.Select(p => p.User_NTID);
            var result_NTID = DB_NTID.Except(Page_NTID);
            foreach (var a in result_NTID)
            {
                var funcplant_uid = systemFunctionPlantRepository.GetMany(p => p.FunPlant == vm.FuncPlant).Select(p => p.System_FunPlant_UID).FirstOrDefault();
                var account_uid = systemUserRepository.GetMany(p => p.User_NTID == a).Select(p => p.Account_UID).FirstOrDefault();
                var userplant =
                    systemUserFunPlantRepository.GetMany(
                        p => p.Account_UID == account_uid && p.System_FunPlant_UID == funcplant_uid).FirstOrDefault();
                systemUserFunPlantRepository.Delete(userplant);
            }

            foreach (var userorg in distinctKeys)
            {
                var result = systemUserFunPlantRepository.CheckUserFuncPlantIsExit(vm.FuncPlant, userorg.User_NTID);
                if (!result.Any())
                {
                    System_User_FunPlant ufunplant = new System_User_FunPlant();
                    var funcplant_uid = systemFunctionPlantRepository.GetAll().Where(p => p.FunPlant == vm.FuncPlant).Select(p => p.System_FunPlant_UID).FirstOrDefault();
                    var account_uid = systemUserRepository.GetAll().Where(p => p.User_NTID == userorg.User_NTID).Select(p => p.Account_UID).FirstOrDefault();
                    ufunplant.Account_UID = account_uid;
                    ufunplant.System_FunPlant_UID = funcplant_uid;
                    ufunplant.Modified_Date = DateTime.Now;
                    ufunplant.Modified_UID = vm.Modified_UID;
                    systemUserFunPlantRepository.Add(ufunplant);
                }

            }
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public List<UserPlantWithPlants> GetUserInfoByFuncPlant(string FuncPlant)
        {
            var temp =
                systemUserFunPlantRepository.GetMany(p => p.System_Function_Plant.FunPlant == FuncPlant)
                    .Select(p => p.System_Users).Distinct();
            var result = from t in temp
                         select new UserPlantWithPlants
                         {
                             User_NTID = t.User_NTID,
                             User_Name = t.User_Name
                         };
            return result.ToList();
        }

        #endregion

        #region Admin maintance function--------------------------Add By Destiny

        public bool DeleteEnumeration(int enum_uid)
        {
            enumerationRepository.Delete(k => k.Enum_UID == enum_uid);
            unitOfWork.Commit();
            return true;
        }


        public string AddEnumeration(Enumeration ent)
        {
            enumerationRepository.Add(ent);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public List<string> GetEnumNameForKeyProcess()
        {
            return enumerationRepository.GetEnumNamebyType("Report_Key_Process");
        }

        public PagedListModel<EnumerationDTO> GetEnumValueForKeyProcess(FlowChartModelSearch search, Page page)
        {
            var enumerations = enumerationRepository.GetEnumValueForKeyProcess(search.Project_Name, search.Part_Types);
            var totalCount = 0;
            return new PagedListModel<EnumerationDTO>(totalCount, enumerations);
        }
        public List<EnumerationDTO> GetEnumList(string enumType)
        {

         return   enumerationRepository.GetEnumList(enumType);
        }

        public string Edit_WIP(EditWIPDTO dto)
        {
            return productInputRepository.Edit_WIP(dto);
        }

        public string Edit_Product(EditProductDTO dto)
        {
            try
            {
                var result = productInputRepository.GetById(dto.Product_UID);
                result.Picking_QTY = dto.Picking_QTY;
                result.Good_QTY = dto.Good_QTY;
                result.WH_Picking_QTY = dto.WH_Picking_QTY;
                result.WH_QTY = dto.WH_QTY;
                result.NG_QTY = dto.NG_QTY;
                result.WIP_QTY = dto.WIP_QTY;
                result.Adjust_QTY = dto.Adjust_QTY;
                result.Is_Comfirm = dto.Is_Comfirm != 0;
                result.IsLast = dto.Is_Last != 0;
                productInputRepository.Update(result);
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {

                return e.Message;
            }


        }
        public string Edit_Flag(EditFlagDTO dto)
        {
            try
            {
                var time = DateTime.Parse(dto.Product_Date);
                var result = productInputRepository.GetMany(m => m.Product_Date == time && m.FlowChart_Master_UID == dto.Master_UID && m.Time_Interval == dto.Interval_Time && m.FunPlant == dto.Func_Plant).ToList();
                foreach (var item in result)
                {
                    item.Is_Comfirm = dto.Is_Comfirmed != 0;
                    item.IsLast = dto.Is_Lasted != 0;
                    productInputRepository.Update(item);
                }
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string ExeSql(string ExeSql)
        {
            return enumerationRepository.ExeSql(ExeSql);
        }
        public string Edit_Enum(EditEnumDTO dto)
        {
            try
            {

                Enumeration result = enumerationRepository.GetById(dto.Enum_UID);
                result.Enum_Type = dto.Enum_Type;
                result.Enum_Name = dto.Enum_Name;
                result.Enum_Value = dto.Enum_Value;
                result.Decription = dto.Decription;
                enumerationRepository.Update(result);
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {

                return e.Message;
            }
        }
        public string Add_Enum(Enumeration dto)
        {
            try
            {
                enumerationRepository.Add(dto);
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {

                return e.Message;
            }
        }



        public List<string> GetTypeClassfy(int userUID)
        {
            return enumerationRepository.GetTypeClassfy(userUID);
        }
        #endregion

        #region Binding MH and Process--------------------Add By Sidney

        //public SystemOrgAndProVM getOrgAndPro(int currentUser)
        //{
        //    var result = new SystemOrgAndProVM
        //    {
        //        Project = systemProjectRepository.GetProjectList(),
        //        SystemOrg = systemOrgRepository.GetOrgList(),
        //        UserRole = systemRoleRepository.GetUserRoleNow(currentUser)
        //    };
        //    return result;

        //}

        public SystemOrgAndProVM getOrgAndPro(int currentUser, int Org_OP_Id)
        {
            var result = new SystemOrgAndProVM
            {
                SystemOrg = systemOrgRepository.GetOrgList(),
                UserRole = systemRoleRepository.GetUserRoleNow(currentUser)
            };

            //获取当前用户所在的OP
            var roles = systemRoleRepository.GetUserRoleNow(currentUser);
            //string OP = systemProjectRepository.GetCurrentOPType(currentUser);
            result.Project = systemProjectRepository.GetProjectList(Org_OP_Id, roles);
            return result;
        }

        private int getOrgUid(string OrgName)
        {
            var org_uid =
                    systemOrgRepository.GetMany(m => m.Organization_Name == OrgName)
                        .Select(m => m.Organization_UID)
                        .FirstOrDefault();
            return org_uid;
        }

        public int GetPPOrgUid(string OPName, string PPName)
        {
            var org_uid =
                systemOrgBomRepository.GetMany(
                    m =>
                        m.System_Organization.Organization_Name == OPName &&
                        m.System_Organization1.Organization_Name == PPName).Select(m => m.ChildOrg_UID).FirstOrDefault();
            return org_uid;
        }

        public int GetOrgUidByID(string OrgID)
        {
            var org_uid =
                    systemOrgRepository.GetMany(m => m.Organization_ID == OrgID)
                        .Select(m => m.Organization_UID)
                        .FirstOrDefault();
            return org_uid;

        }

        public string AddOrgInfo(SystemUserInfo1 info)
        {
            try
            {

                if (info.Org_FuncPlant != null)
                {
                    foreach (var func in info.Org_FuncPlant)
                    {
                        //var userOrg = new System_UserOrg
                        //{
                        //    Account_UID = info.Account_UID,
                        //    Modified_Date = DateTime.Now,
                        //    Begin_Date = DateTime.Now,
                        //    Modified_UID = info.Modified_UID,
                        //    Organization_UID = int.Parse(func),
                        //    Plant_OrganizationUID = systemUserOrgRepository.GetOrgUidByName(info.Org_CTU),
                        //    OPType_OrganizationUID = int.Parse(info.Org_OP),
                        //    Department_OrganizationUID = int.Parse(info.Org_PP),
                        //    Funplant_OrganizationUID = int.Parse(func)
                        //};
                        int result = 0;
                        int intfunc = 0;
                        int intpp = 0;
                        try
                        {
                            result = int.Parse(func);
                            intpp = int.Parse(info.Org_PP);
                            intfunc = int.Parse(func);
                        }
                        catch
                        {
                            try
                            {
                                result = int.Parse(info.Org_PP);
                                intpp = int.Parse(info.Org_PP);
                            }
                            catch
                            {
                                result = int.Parse(info.Org_OP);
                            }
                        }
                        var insertOrg = new System_UserOrg
                        {
                            Account_UID = info.Account_UID,
                            Organization_UID = result,
                            Modified_Date = DateTime.Now,
                            Begin_Date = DateTime.Now,
                            Modified_UID = info.Modified_UID,
                            Plant_OrganizationUID = systemUserOrgRepository.GetOrgUidByName(info.Org_CTU),
                            OPType_OrganizationUID = int.Parse(info.Org_OP)
                        };
                        if (intpp != 0)
                        {
                            insertOrg.Department_OrganizationUID = intpp;
                        }
                        if (intfunc != 0)
                        {
                            insertOrg.Funplant_OrganizationUID = intfunc;
                        }


                        systemUserOrgRepository.Add(insertOrg);
                        //添加数据到System_User_FuncPlant表

                        //var updateEntity =
                        //    systemUserFunPlantRepository.GetMany(
                        //        m => m.Account_UID == info.Account_UID && m.System_Function_Plant.Organization_ID == func).FirstOrDefault();

                        //var insertEntity = new System_User_FunPlant();
                        //if (updateEntity != null)
                        //{
                        //    updateEntity.Modified_Date = DateTime.Now;
                        //    updateEntity.Modified_UID = info.Modified_UID;
                        //}
                        //else
                        //{
                        //    var funcUid =
                        //        systemFunctionPlantRepository.GetMany(m => m.Organization_ID == func)
                        //            .Select(m => m.System_FunPlant_UID)
                        //            .FirstOrDefault();
                        //    insertEntity.Account_UID = info.Account_UID;
                        //    insertEntity.System_FunPlant_UID = funcUid;
                        //    insertEntity.Modified_Date = DateTime.Now;
                        //    insertEntity.Modified_UID = info.Modified_UID;
                        //}
                        //systemUserFunPlantRepository.AddOrUpdate(insertEntity, updateEntity, updateEntity == null);
                    }
                }
                else
                {
                    var userOrg = new System_UserOrg
                    {
                        Account_UID = info.Account_UID,
                        Modified_Date = DateTime.Now,
                        End_Date = DateTime.Now,
                        Modified_UID = info.Modified_UID
                    };
                    if (!(info.Org_PP == null || info.Org_PP == "Nothing"))
                    {
                        userOrg.Organization_UID = int.Parse(info.Org_PP);
                        userOrg.Department_OrganizationUID = int.Parse(info.Org_PP);
                    }
                    if (!(info.Org_OP == null || info.Org_OP == "Nothing"))
                    {
                        userOrg.Organization_UID = int.Parse(info.Org_OP);
                        userOrg.OPType_OrganizationUID = int.Parse(info.Org_OP);
                    }
                    if (info.Org_CTU != null && info.Org_CTU != "Nothing")
                    {
                        userOrg.Organization_UID = systemUserOrgRepository.GetOrgUidByName(info.Org_CTU);
                        userOrg.Plant_OrganizationUID = systemUserOrgRepository.GetOrgUidByName(info.Org_CTU);
                    }
                    else
                    {
                        return "SUCCESS";
                    }
                    systemUserOrgRepository.Add(userOrg);
                }
                unitOfWork.Commit();
                //if (info.Org_CTU == "WUXI_M")
                //{
                //    systemUserOrgRepository.InsertUserDataToMiddleTable(info.Account_UID);
                //}
                return "SUCCESS";



            }
            catch (Exception e)
            {
                return "False" + e.ToString();
            }
        }

        public string AddProjectInfo(SystemUserInfo1 info)
        {
            try
            {
                bool flag = false;
                if (info.Project != null)
                {
                    foreach (var item in info.Project)
                    {
                        var projectType =
                            systemProjectRepository.GetById(item).Project_Type;
                        var project = new Project_Users_Group
                        {
                            Project_UID = item,
                            Account_UID = info.Account_UID,
                            Modified_UID = info.Modified_UID,
                            Modified_Date = DateTime.Now
                        };
                        projectUsersGroupRepository.Add(project);
                        if (info.Org_CTU.Contains("WUXI_M") && projectType.Contains("PIS&eTransfer"))
                        {
                            flag = true;
                        }
                    }
                }
                //else
                //{
                //    var userProject = projectUsersGroupRepository.GetMany(m => m.Account_UID == info.Account_UID);
                //    foreach (var item in userProject)
                //    {
                //        projectUsersGroupRepository.Delete(item); 
                //    }
                //}
                unitOfWork.Commit();
                if (flag)
                {
                    systemUserOrgRepository.InsertUserDataToMiddleTable(info.Account_UID);
                }
                return "SUCCESS";
            }
            catch (Exception e)
            {
                return "False" + e.ToString();
            }
        }

        public string AddRoleInfo(SystemUserInfo1 info)
        {
            try
            {
                if (info.Role != null)
                {
                    info.Role = info.Role.Distinct().ToList();
                    foreach (var item in info.Role)
                    {
                        var projectUid =
                            systemRoleRepository.GetMany(m => m.Role_ID == item).Select(m => m.Role_UID).FirstOrDefault();
                        var userRole = systemUserRoleRepository.GetMany(m => m.Account_UID == info.Account_UID && m.Role_UID == projectUid).FirstOrDefault();
                        if (userRole == null)
                        {
                            var project = new System_User_Role
                            {
                                Role_UID = projectUid,
                                Account_UID = info.Account_UID,
                                Modified_UID = info.Modified_UID,
                                Modified_Date = DateTime.Now
                            };
                            systemUserRoleRepository.Add(project);
                        }
                    }
                }
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {
                return "False" + e.ToString();
            }
        }

        public int GetUserAccountUid(string NTID)
        {
            return systemUserRepository.GetMany(m => m.User_NTID == NTID).Select(m => m.Account_UID).FirstOrDefault();
        }


        public string ModifyOrgInfo(SystemUserInfo1 info)
        {
            return systemUserOrgRepository.UpdateUserOrgInfo(info);
        }

        public string ModifyProjectInfo(SystemUserInfo1 info)
        {
            if (info.Project != null)
            {
                try
                {
                    bool flag = false;
                    //先删除掉所有的Project，然后再添加
                    var temp = projectUsersGroupRepository.GetMany(m => m.Account_UID == info.Account_UID);
                    if (temp != null)
                    {
                        var items = temp.ToList();
                        for (int i = 0; i < items.Count; i++)
                        {
                            var item = items[i];
                            projectUsersGroupRepository.Delete(item);
                        }

                        unitOfWork.Commit();
                    }
                    foreach (var item in info.Project)
                    {
                        //var pro = item;
                        //var projectUid = systemProjectRepository.GetMany(m => m.Project_Name == pro).Select(m => m.Project_UID).FirstOrDefault();
                        //var userProject = projectUsersGroupRepository.GetMany(m => m.Account_UID == info.Account_UID && m.Project_UID == projectUid).FirstOrDefault();
                        //if (userProject == null)
                        //{
                        var projectType =
                            systemProjectRepository.GetById(item).Project_Type;
                        var tempProject = new Project_Users_Group
                        {
                            Account_UID = info.Account_UID,
                            Project_UID = item,
                            Modified_Date = DateTime.Now,
                            Modified_UID = info.Modified_UID
                        };
                        projectUsersGroupRepository.Add(tempProject);
                        if (info.Org_CTU.Contains("WUXI_M") && projectType.Contains("PIS&eTransfer"))
                        {
                            flag = true;
                        }
                        //}
                        //else
                        //{
                        //    userProject.Project_UID = projectUid;
                        //    userProject.Modified_UID = info.Modified_UID;
                        //    userProject.Modified_Date = DateTime.Now;
                        //    projectUsersGroupRepository.Update(userProject);
                        //}

                    }
                    unitOfWork.Commit();
                    if (flag)
                    {
                        systemUserOrgRepository.InsertUserDataToMiddleTable(info.Account_UID);
                    }
                    return "SUCCESS";
                }
                catch (Exception e)
                {
                    return "False" + e.ToString();
                }

            }
            else
            {
                var userProject =
                    projectUsersGroupRepository.GetMany(m => m.Account_UID == info.Account_UID).Distinct();
                try
                {

                    var lists = userProject.ToList();
                    for (int i = 0; i < lists.Count; i++)

                    {

                        var item = lists[i];
                        projectUsersGroupRepository.Delete(item);

                    }

                    unitOfWork.Commit();

                    return "SUCCESS";
                }
                catch (Exception e)
                {

                    return "False" + e.ToString();
                }
            }
        }

        public string ModifyRoleInfo(SystemUserInfo1 info)
        {
            if (info.Role != null)
            {
                try
                {
                    //先获取当前管理者用户可以管理的角色，将属于当前管理者的角色信息删除。
                    var currentRoles = systemRoleRepository.GetUserRoleNow(info.CurrentUID);
                    List<int> RoleUIDs = new List<int>();
                    int roleUID = 0;
                    foreach (var item in currentRoles)
                    {
                        roleUID = item.RoleUID;
                        RoleUIDs.Add(roleUID);
                    }
                    //先删除掉可以管理的Role，然后再添加
                    var temp = systemUserRoleRepository.GetMany(m => m.Account_UID == info.Account_UID);
                    if (temp != null)
                    {
                        var items = temp.ToList();
                        for (int i = 0; i < items.Count; i++)
                        {

                            var item = items[i];
                            if (RoleUIDs.Contains(item.Role_UID))
                            {
                                systemUserRoleRepository.Delete(item);
                            }

                        }

                        unitOfWork.Commit();


                    }
                    foreach (var item in info.Role)
                    {
                        var pro = item;
                        var roleUid = systemRoleRepository.GetMany(m => m.Role_ID == pro).Select(m => m.Role_UID).FirstOrDefault();
                        var userRole = systemUserRoleRepository.GetMany(m => m.Account_UID == info.Account_UID && m.Role_UID == roleUid).FirstOrDefault();
                        if (userRole == null)
                        {
                            var tempProject = new System_User_Role
                            {
                                Account_UID = info.Account_UID,
                                Role_UID = roleUid,
                                Modified_Date = DateTime.Now,
                                Modified_UID = info.Modified_UID
                            };
                            systemUserRoleRepository.Add(tempProject);
                        }
                        else
                        {
                            userRole.Role_UID = roleUid;
                            userRole.Account_UID = info.Account_UID;
                            userRole.Modified_Date = DateTime.Now;
                            userRole.Modified_UID = info.Modified_UID;
                            systemUserRoleRepository.Update(userRole);
                        }
                        unitOfWork.Commit();
                    }
                    return "SUCCESS";
                }
                catch (Exception e)
                {
                    return "False" + e.ToString();
                }

            }
            else
            {
                var userRole =
                    systemUserRoleRepository.GetMany(m => m.Account_UID == info.Account_UID).Distinct();
                try
                {
                    var items = userRole.ToList();
                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        systemUserRoleRepository.Delete(item);
                    }

                    unitOfWork.Commit();
                    return "SUCCESS";
                }
                catch (Exception e)
                {

                    return "False" + e.ToString();
                }
            }
        }

        public SystemUserInfo1 QueryOrgAndProjectByUid(int uuid)
        {
            var result = new SystemUserInfo1();
            var role =
                systemUserRoleRepository.GetMany(m => m.Account_UID == uuid)
                    .Select(m => m.System_Role.Role_ID).ToList();
            result.Role = role;
            var project =
                projectUsersGroupRepository.GetMany(m => m.Account_UID == uuid)
                    .Select(m => m.Project_UID).ToList();
            result.Project = project;
            var org = systemOrgRepository.QueryOrganzitionInfoByAccountID(uuid);
            if (org != null && org.Count > 0)
            {
                result.Org_CTU = org[0].Plant;
                result.Org_OP = org[0].OPType;
                result.Org_OP_ID = org[0].OPType_OrganizationUID.ToString();
                result.Org_PP = org[0].Department;
                result.Org_PP_ID = org[0].Department_OrganizationUID.ToString();
                result.Org_FunP = org[0].Funplant;
                result.Org_FunP_ID = org[0].Funplant_OrganizationUID.ToString();
            }
            return result;
        }

        public string GetFatherOrg(string ChildOrg)
        {
            var childOrgUid =
                systemOrgRepository.GetMany(m => m.Organization_Name == ChildOrg)
                    .Select(m => m.Organization_UID)
                    .FirstOrDefault();
            var FatherOrg =
                systemOrgBomRepository.GetMany(m => m.ChildOrg_UID == childOrgUid).Select(m => m.System_Organization.Organization_Name).FirstOrDefault();
            return FatherOrg;
        }

        #endregion

        #region Project Maintanance--------------Add By Sidney
        public PagedListModel<ProjectVM> QueryProjects(ProjectSearchModel search, Page page)
        {
            var totalCount = 0;
            var roles = systemRoleRepository.GetUserRoleNow((int)search.CurrentUser);
            List<int> roleIDs = new List<int>();
            bool flag = false;
            foreach (var item in roles)
            {
                if (item.UserRoleID == "SystemAdmin")
                    flag = true;
                roleIDs.Add(item.RoleUID);
            }
            var plantsDTO = systemProjectRepository.QueryProjects(search, page, flag, out totalCount);

            return new PagedListModel<ProjectVM>(totalCount, plantsDTO);
        }

        public List<string> GetCustomer()
        {
            var result = systemBUDRepository.GetAll().Select(m => m.BU_D_Name).Distinct().ToList();
            return result;
        }

        public List<string> GetProductPhase()
        {
            var result =
                enumerationRepository.GetMany(m => m.Enum_Type == "Product_Phase")
                    .Select(m => m.Enum_Name)
                    .Distinct()
                    .ToList();
            return result;
        }

        public List<SystemOrgDTO> GetOpTypes(int plantuid, int oporguid)
        {
            var result = systemOrgRepository.GetOpTypeByPlant(plantuid, oporguid);
            return result;
        }

        public List<SystemOrgDTO> GetPlants(int PlantOrgUid)
        {
            var result = systemOrgRepository.GetPlants(PlantOrgUid);
            return result;
        }

        public ProjectVM QueryProjectSingle(int uuid)
        {
            var result = systemProjectRepository.GetMany(m => m.Project_UID == uuid).FirstOrDefault();
            var org = systemOrgRepository.GetPlant(result.Organization_UID);
            var dto = new ProjectVM();
            dto.Organization_UID = result.Organization_UID;
            dto.Organization_Name = org[0];
            dto.Project = result.Project_Name;
            dto.MESProject = result.MESProject_Name;
            dto.Project_UID = result.Project_UID;
            dto.Product_Phase = result.Product_Phase;
            dto.OP_TYPES = result.OP_TYPES;
            dto.Modified_User = result.System_Users.User_Name;
            dto.Modified_Date = result.Modified_Date;
            dto.Customer = result.System_BU_D.BU_D_Name;
            dto.Project_Type = result.Project_Type;
            return dto;
        }

        public string AddProject(ProjectVMDTO vm)
        {
            var buduid = systemBUDRepository.GetMany(m => m.BU_D_Name == vm.Customer).Select(m => m.BU_D_UID).FirstOrDefault();
            //check Unique
            var unique =
                systemProjectRepository.GetMany(
                    m => m.Project_Name == vm.Project && m.Product_Phase == vm.Product_Phase && m.BU_D_UID == buduid);
            if (unique.Any())
            {
                return "客户/专案/生产阶段组合已存在！";
            }
            else
            {
                var project = new System_Project();
                project.Project_Name = vm.Project;
                project.MESProject_Name = vm.MESProject;
                project.Product_Phase = vm.Product_Phase;
                project.BU_D_UID = buduid;
                project.Project_Code = vm.Project;

                project.Modified_Date = DateTime.Now;
                project.Modified_UID = vm.Modified_UID;
                project.OP_TYPES = vm.OP_TYPES;
                project.Organization_UID = vm.Organization_UID;
                project.Project_Type = vm.Project_Type;
                systemProjectRepository.Add(project);
                try
                {
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return "SUCCESS";
            }
        }

        public string EditProject(ProjectVMDTO vm)
        {
            var buduid = systemBUDRepository.GetMany(m => m.BU_D_Name == vm.Customer).Select(m => m.BU_D_UID).FirstOrDefault();
            //check Unique
            var unique =
                systemProjectRepository.GetMany(
                    m => m.Project_Name == vm.Project && m.Product_Phase == vm.Product_Phase && m.BU_D_UID == buduid && m.Project_UID != vm.Project_UID);
            if (unique.Any())
            {
                return "客户/专案/生产阶段组合已存在！";
            }
            else
            {
                var project = systemProjectRepository.GetMany(m => m.Project_UID == vm.Project_UID).FirstOrDefault();
                if (project != null)
                {
                    try
                    {
                        project.Project_Name = vm.Project;
                        project.MESProject_Name = vm.MESProject;
                        project.Product_Phase = vm.Product_Phase;
                        project.BU_D_UID = buduid;
                        project.Project_Code = vm.Project;
                        project.Modified_Date = DateTime.Now;
                        project.Modified_UID = vm.Modified_UID;
                        project.OP_TYPES = vm.OP_TYPES;
                        project.Project_Type = vm.Project_Type;
                        systemProjectRepository.Update(project);
                        unitOfWork.Commit();
                        return "SUCCESS";
                    }
                    catch (Exception ex)
                    {
                        return "Fail";
                    }
                }
                else
                {
                    return "隐藏的Project_UID不存在！";
                }

            }
        }

        public string DeleteProject(int Project_UID)
        {
            var result = systemProjectRepository.GetById(Project_UID);
            var flowProject = flowChartMasterRepository.GetMany(m => m.Project_UID == Project_UID);
            var userProject = projectUsersGroupRepository.GetMany(m => m.Project_UID == Project_UID);
            if (flowProject.Any() || userProject.Any())
            {
                return "Project already in used";
            }
            else
            {
                systemProjectRepository.Delete(result);
                unitOfWork.Commit();
                return "SUCCESS";
            }

        }

        #endregion

        public string GetOrganizationId(string OrgName)
        {
            var result = systemOrgRepository.GetMany(m => m.Organization_Name == OrgName).Select(m => m.Organization_ID).FirstOrDefault();
            return result;
        }

        public List<System_Project> GetProjectByOrgId(int org)
        {
            return systemProjectRepository.GetProjects(org);
        }

        #region Language Maintance  -------------------------Add By Rock 2017/7/17
        public List<SystemLanguageDTO> LanguagesInfo()
        {
            var result = systemLanguageRepository.GetAll().OrderBy(m => m.DisplayOrder).ToList();
            var dtoList = AutoMapper.Mapper.Map<List<SystemLanguageDTO>>(result);
            return dtoList;
        }

        public string GetLocaleStringResource(int languageID, string resourceName)
        {
            if (CacheHelper.Get(resourceName + languageID) != null)
            {
                return CacheHelper.Get(resourceName + languageID).ToString();
            }
            else
            {
                string result = string.Empty;
                var item = systemLocaleStringResourceRepository.GetMany(m => m.ResourceName == resourceName && m.System_Language_UID == languageID).FirstOrDefault();
                if (item == null)
                {
                    result = resourceName;
                }
                else
                {
                    result = item.ResourceValue;
                    CacheHelper.Set(resourceName + languageID, result);
                }
                return result;
            }
        }

        public PagedListModel<SystemLocaleStringResourceDTO> LocaleStringResourceInfo(SystemLocaleStringResourceDTO searchModel, Page page)
        {
            var totalCount = 0;
            var list = systemLocaleStringResourceRepository.LocaleStringResourceInfo(searchModel, page, out totalCount);
            var result = new PagedListModel<SystemLocaleStringResourceDTO>(totalCount, list);
            return result;
        }

        public string SaveResourceInfo(SystemLocaleStringResourceDTO searchModel)
        {
            if (Convert.ToInt32(searchModel.System_LocaleStringResource_UID) == 0) //新增
            {
                var existItem = systemLocaleStringResourceRepository.GetMany(m => m.System_Language_UID == searchModel.System_Language_UID
                && m.ResourceName.ToLower() == searchModel.ResourceName.Trim().ToLower()).FirstOrDefault();
                if (existItem != null)
                {
                    var errorInfo = GetLocaleStringResource(searchModel.CurrentWorkingLanguage, "System.Language.ExistResourceName");
                    return errorInfo;
                }
                else
                {
                    System_LocaleStringResource resourceItem = new System_LocaleStringResource();
                    resourceItem.System_Language_UID = searchModel.System_Language_UID;
                    resourceItem.ResourceName = searchModel.ResourceName.Trim();
                    resourceItem.ResourceValue = searchModel.ResourceValue.Trim();
                    resourceItem.Modified_UID = searchModel.Modified_UID;
                    resourceItem.Modified_Date = DateTime.Now;
                    systemLocaleStringResourceRepository.Add(resourceItem);
                    unitOfWork.Commit();
                }
            }
            else //编辑
            {
                var item = systemLocaleStringResourceRepository.GetById(Convert.ToInt32(searchModel.System_LocaleStringResource_UID));
                var otherItem = systemLocaleStringResourceRepository.GetMany(m => m.System_LocaleStringResource_UID != searchModel.System_LocaleStringResource_UID &&
                m.System_Language_UID == searchModel.System_Language_UID && m.ResourceName.ToLower() == searchModel.ResourceName.Trim().ToLower()).FirstOrDefault();
                if (otherItem != null)
                {
                    var errorInfo = GetLocaleStringResource(searchModel.CurrentWorkingLanguage, "System.Language.ExistResourceName");
                    return errorInfo;

                }
                else
                {
                    item.ResourceName = searchModel.ResourceName;
                    item.ResourceValue = searchModel.ResourceValue;
                    item.Modified_UID = searchModel.Modified_UID;
                    item.Modified_Date = DateTime.Now;
                    unitOfWork.Commit();
                }
            }

            return string.Empty;
        }

        public List<SystemLocaleStringResourceDTO> LocaleStringResourceInfoByLanguageId(int System_Language_UID)
        {
            var list = systemLocaleStringResourceRepository.GetMany(m => m.System_Language_UID == System_Language_UID).ToList();
            var dtoList = AutoMapper.Mapper.Map<List<SystemLocaleStringResourceDTO>>(list);
            return dtoList;
        }

        public string ImportLanguageExcel(List<SystemLocaleStringResourceDTO> dtoList)
        {
            var list = AutoMapper.Mapper.Map<List<System_LocaleStringResource>>(dtoList);
            systemLocaleStringResourceRepository.AddList(list);
            unitOfWork.Commit();
            return string.Empty;
        }

        #endregion Language Maintance  -------------------------Add By Rock 2017/7/17
        #region 生产车间维护
        public PagedListModel<WorkshopDTO> QueryWorkshops(WorkshopModelSearch search, Page page)
        {
            var totalCount = 0;
            var workshops = workshopRepository.QueryWorkshops(search, page, out totalCount);

            IList<WorkshopDTO> workshopsDTO = new List<WorkshopDTO>();

            foreach (var workshop in workshops)
            {
                var dto = AutoMapper.Mapper.Map<WorkshopDTO>(workshop);
                if (workshop.System_Organization != null)
                {
                    dto.PlantName = workshop.System_Organization.Organization_Name;
                }
                if (workshop.System_Organization1 != null)
                {
                    dto.BGName = workshop.System_Organization1.Organization_Name;
                }
                if (workshop.System_Organization2 != null)
                {
                    dto.FunPlantName = workshop.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(workshop.System_Users1);
                workshopsDTO.Add(dto);
            }

            return new PagedListModel<WorkshopDTO>(totalCount, workshopsDTO);
        }

        public Workshop QueryWorkshopSingle(int uid)
        {
            return workshopRepository.GetById(uid);
        }

        public string AddWorkshop(Workshop workshop)
        {
            workshopRepository.Add(workshop);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string EditWorkshop(Workshop workshop)
        {
            workshopRepository.Update(workshop);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string DeleteWorkshop(int uid)
        {
            var workshop = workshopRepository.GetFirstOrDefault(w => w.Workshop_UID == uid);
            if (workshop != null)
            {
                //如果有关联的数据不可删
                var relativeFixTureUserWorkshopCount = workshop.Fixture_User_Workshop.Count();
                var productLineCount = workshop.Production_Line.Count();
                if (relativeFixTureUserWorkshopCount > 0 || productLineCount > 0)
                {
                    return "HAVEREFERENCE";
                }
                try
                {
                    workshopRepository.Delete(workshop);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "SUCCESS";
        }

        public IEnumerable<WorkshopDTO> DoExportWorkshop(string uids)
        {
            var totalCount = 0;
            var workshops = workshopRepository.QueryWorkshops(new WorkshopModelSearch { ExportUIds = uids }, null, out totalCount);

            IList<WorkshopDTO> WorkshopDTO = new List<WorkshopDTO>();

            foreach (var workshop in workshops)
            {
                var dto = AutoMapper.Mapper.Map<WorkshopDTO>(workshop);
                if (workshop.System_Organization != null)
                {
                    dto.PlantName = workshop.System_Organization.Organization_Name;
                }
                if (workshop.System_Organization1 != null)
                {
                    dto.BGName = workshop.System_Organization1.Organization_Name;
                }
                if (workshop.System_Organization2 != null)
                {
                    dto.FunPlantName = workshop.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(workshop.System_Users1);
                WorkshopDTO.Add(dto);
            }
            return WorkshopDTO;
        }
        #endregion

        #region 制程维护
        public PagedListModel<Process_InfoDTO> QueryProcess_Infos(Process_InfoModelSearch search, Page page)
        {
            var totalCount = 0;
            var process_Infos = process_InfoRepository.QueryProcess_Infos(search, page, out totalCount);

            IList<Process_InfoDTO> process_InfoDTOs = new List<Process_InfoDTO>();

            foreach (var process_Info in process_Infos)
            {
                var dto = AutoMapper.Mapper.Map<Process_InfoDTO>(process_Info);
                if (process_Info.System_Organization != null)
                {
                    dto.PlantName = process_Info.System_Organization.Organization_Name;
                }
                if (process_Info.System_Organization1 != null)
                {
                    dto.BGName = process_Info.System_Organization1.Organization_Name;
                }
                if (process_Info.System_Organization2 != null)
                {
                    dto.FunPlantName = process_Info.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(process_Info.System_Users1);
                process_InfoDTOs.Add(dto);
            }

            return new PagedListModel<Process_InfoDTO>(totalCount, process_InfoDTOs);
        }

        public IList<Process_InfoDTO> QueryProcess_InfoList(Process_InfoModelSearch search)
        {
            var process_Infos = process_InfoRepository.QueryProcess_InfoList(search);
            IList<Process_InfoDTO> process_InfoDTOs = new List<Process_InfoDTO>();
            foreach (var process_Info in process_Infos)
            {
                var dto = AutoMapper.Mapper.Map<Process_InfoDTO>(process_Info);
                dto.PlantName = process_Info.System_Organization.Organization_Name;
                dto.BGName = process_Info.System_Organization1.Organization_Name;
                if (process_Info.System_Organization2 != null)
                {
                    dto.FunPlantName = process_Info.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(process_Info.System_Users1);
                process_InfoDTOs.Add(dto);
            }
            return process_InfoDTOs;
        }

        public Process_Info QueryProcess_InfoSingle(int uid)
        {
            return process_InfoRepository.GetById(uid);
        }

        public string AddProcess_Info(Process_Info process_Info)
        {
            process_InfoRepository.Add(process_Info);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string EditProcess_Info(Process_Info process_Info)
        {
            process_InfoRepository.Update(process_Info);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string DeleteProcess_Info(int uid)
        {
            var process_Info = process_InfoRepository.GetFirstOrDefault(w => w.Process_Info_UID == uid);
            if (process_Info != null)
            {
                //如果有关联的数据则不可删
                var productLineCount = process_Info.Production_Line.Count();
                if (productLineCount > 0)
                {
                    return "HAVEREFERENCE";
                }
                try
                {
                    process_InfoRepository.Delete(process_Info);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "SUCCESS";
        }

        public IEnumerable<Process_InfoDTO> DoExportProcess_Info(string uids)
        {
            var totalCount = 0;
            var Process_Infos = process_InfoRepository.QueryProcess_Infos(new Process_InfoModelSearch { ExportUIds = uids }, null, out totalCount);

            IList<Process_InfoDTO> process_InfoDTOs = new List<Process_InfoDTO>();

            foreach (var Process_Info in Process_Infos)
            {
                var dto = AutoMapper.Mapper.Map<Process_InfoDTO>(Process_Info);
                if (Process_Info.System_Organization != null)
                {
                    dto.PlantName = Process_Info.System_Organization.Organization_Name;
                }
                if (Process_Info.System_Organization1 != null)
                {
                    dto.BGName = Process_Info.System_Organization1.Organization_Name;
                }
                if (Process_Info.System_Organization2 != null)
                {
                    dto.FunPlantName = Process_Info.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(Process_Info.System_Users1);
                process_InfoDTOs.Add(dto);
            }
            return process_InfoDTOs;
        }
        #endregion

        #region 表格内容多语言维护
        public PagedListModel<System_LocalizedPropertyDTO> QuerySystemLocalizedProperties(System_LocalizedPropertyModelSearch search, Page page)
        {
            var totalCount = 0;
            var systemLocalizedPropertyQuery = systemLocalizedPropertyRepository.QuerySystemLocalizedProperties(search, page, out totalCount);

            var systemLocalizedPropertyDTOs = new List<System_LocalizedPropertyDTO>();

            foreach (var item in systemLocalizedPropertyQuery)
            {
                var dto = AutoMapper.Mapper.Map<System_LocalizedPropertyDTO>(item);
                if (item.System_Language != null)
                {
                    dto.LanguageName = item.System_Language.Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(item.System_Users);
                systemLocalizedPropertyDTOs.Add(dto);
            }

            return new PagedListModel<System_LocalizedPropertyDTO>(totalCount, systemLocalizedPropertyDTOs);
        }

        public System_LocalizedPropertyDTO QuerySystemLocalizedPropertySingle(int uid)
        {
            var result = systemLocalizedPropertyRepository.GetById(uid);
            var dto = AutoMapper.Mapper.Map<System_LocalizedPropertyDTO>(result);
            return dto;
        }

        public string AddSystemLocalizedProperty(System_LocalizedPropertyDTO dto)
        {
            var systemLocalizedProperty = AutoMapper.Mapper.Map<System_LocalizedProperty>(dto);
            systemLocalizedPropertyRepository.Add(systemLocalizedProperty);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string EditSystemLocalizedProperty(System_LocalizedPropertyDTO dto)
        {
            //var systemLocalizedProperty = AutoMapper.Mapper.Map<System_LocalizedProperty>(dto);
            var editEntity = systemLocalizedPropertyRepository.GetFirstOrDefault(i => i.System_LocalizedProperty_UID == dto.System_LocalizedProperty_UID);
            editEntity.System_Language_UID = dto.System_Language_UID;
            editEntity.Table_Name = dto.Table_Name;
            editEntity.TablePK_UID = dto.TablePK_UID;
            editEntity.Table_ColumnName = dto.Table_ColumnName;
            editEntity.ResourceValue = dto.ResourceValue;
            editEntity.Modified_UID = dto.Modified_UID;
            editEntity.Modified_Date = DateTime.Now;
            systemLocalizedPropertyRepository.Update(editEntity);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string DeleteSystemLocalizedProperty(int uid)
        {
            var process_Info = systemLocalizedPropertyRepository.GetFirstOrDefault(w => w.System_LocalizedProperty_UID == uid);
            try
            {
                systemLocalizedPropertyRepository.Delete(process_Info);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "SUCCESS";
        }
        #endregion

        #region Add By Rock 排程邮件设定 平安夜加班..... 2017/12/24
        public List<SystemModuleEmailFunctionVM> GetBatchEmailSettingFunction(bool isAdmin, int plantUID)
        {
            var list = systemEmailDeliveryRepository.GetBatchEmailSettingFunction(isAdmin, plantUID);
            return list;
        }

        public PagedListModel<SystemModuleEmailVM> QueryBatchEmailSetting(SystemModuleEmailVM search, Page page)
        {
            var totalcount = 0;
            var result = systemEmailDeliveryRepository.QueryBatchEmailSetting(search, page, out totalcount);
            return new PagedListModel<SystemModuleEmailVM>(totalcount, result);

        }

        public SystemModuleEmailVM QueryBatchEmailSettingByEdit(int System_Email_Delivery_UID)
        {
            var item = systemEmailDeliveryRepository.QueryBatchEmailSettingByEdit(System_Email_Delivery_UID);
            return item;
        }

        public SystemUserDTO ChangeNTID(string User_NTID)
        {
            var item = systemUserRepository.GetMany(m => m.User_NTID == User_NTID).FirstOrDefault();
            if (item != null)
            {
                SystemUserDTO dto = new SystemUserDTO();
                dto.Account_UID = item.Account_UID;
                dto.User_Name = item.User_Name;
                dto.Email = item.Email;
                return dto;
            }
            else
            {
                return null;
            }
        }

        public string SaveBatchEmailSetting(SystemModuleEmailVM item)
        {
            var result = systemEmailDeliveryRepository.SaveBatchEmailSetting(item);
            return result;
        }

        public string CheckEmailIsError(SystemModuleEmailVM item)
        {
            var result = systemEmailDeliveryRepository.CheckEmailIsError(item);
            return result;
        }

        #endregion 平安夜加班..... 2017/12/24

    }
}
