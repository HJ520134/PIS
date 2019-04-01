using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Data;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Web.Http;
using PDMS.Model.ViewModels.Settings;
using PDMS.Common.Constants;
using System.Linq;
using PDMS.Core.Authentication;
using PDMS.Model.ViewModels.Batch;

namespace PDMS.WebAPI.Controllers
{
    public class SettingsController : ApiControllerBase
    {
        ISettingsService settingsService;
        ICommonService commonService;

        public SettingsController(ISettingsService settingsService, ICommonService commonService)
        {
            this.settingsService = settingsService;
            this.commonService = commonService;
        }

        #region User Maintenance API-------------------Add By Tonny 2015/11/02
        /// <summary>
        /// get paged records of users by query conditions
        /// </summary>
        /// <param name="data">post search modal and page modal</param>
        /// <returns>json of paged records</returns>
        [HttpPost]
        public IHttpActionResult QueryUsersAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<UserModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var users = settingsService.QueryUsers(searchModel, page);
            return Ok(users);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryUserAPI(dynamic data)
        {
            try {
                var jsonData = data.ToString();
                UserModelSearch searchModel = JsonConvert.DeserializeObject<UserModelSearch>(jsonData);
                var dto = new SystemUserDTO();
                int uuid = searchModel.currntUID.Value;
                dto = AutoMapper.Mapper.Map<SystemUserDTO>(settingsService.QueryUsersSingle(uuid));
                uuid = settingsService.GetUserAccountUid(dto.User_NTID);
                var result = settingsService.QueryOrgAndProjectByUid(uuid);
                result.Account_UID = dto.Account_UID;
                result.Email = dto.Email;
                result.Enable_Flag = dto.Enable_Flag;
                result.LoginToken = dto.LoginToken;
                result.ModifiedUser = dto.ModifiedUser;
                result.Modified_Date = dto.Modified_Date;
                result.Modified_UID = dto.Modified_UID;
                result.Modified_UserNTID = dto.Modified_UserNTID;
                result.Modified_UserName = dto.Modified_UserName;
                result.Tel = dto.Tel;
                result.User_NTID = dto.User_NTID;
                result.User_Name = dto.User_Name;
                result.EmployeeNumber = dto.EmployeeNumber;
                result.EmployeePassword = dto.EmployeePassword == null ? null : eTransfer.PasswordUtil.EncryptionHelper.Decrypt(dto.EmployeePassword);
                result.EnableEmpIdLogin = dto.EnableEmpIdLogin;
                result.Building = dto.Building;
                result.System_Language_UID = dto.System_Language_UID;

                return Ok(result);
            }catch(Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public IHttpActionResult queryUserByNTIDAPI(string NTID)
        {
            int uuid = settingsService.QueryUsersNTID(NTID);
            var dto = new SystemUserDTO();
            dto = AutoMapper.Mapper.Map<SystemUserDTO>(settingsService.QueryUsersSingle(uuid));
            uuid = settingsService.GetUserAccountUid(dto.User_NTID);
            var result = settingsService.QueryOrgAndProjectByUid(uuid);
            result.Account_UID = dto.Account_UID;
            result.Email = dto.Email;
            result.Enable_Flag = dto.Enable_Flag;
            result.LoginToken = dto.LoginToken;
            result.ModifiedUser = dto.ModifiedUser;
            result.Modified_Date = dto.Modified_Date;
            result.Modified_UID = dto.Modified_UID;
            result.Modified_UserNTID = dto.Modified_UserNTID;
            result.Modified_UserName = dto.Modified_UserName;
            result.Tel = dto.Tel;
            result.User_NTID = dto.User_NTID;
            result.User_Name = dto.User_Name;
            result.EmployeeNumber = dto.EmployeeNumber;
            result.EmployeePassword = dto.EmployeePassword == null ? null : eTransfer.PasswordUtil.EncryptionHelper.Decrypt(dto.EmployeePassword);
            result.EnableEmpIdLogin = dto.EnableEmpIdLogin;
            result.Building = dto.Building;
            result.System_Language_UID = dto.System_Language_UID;
            return Ok(result);
        }
        public string ModifyUserAPI(SystemUserInfo1 dto)
        {
            try
            {
                dto.Account_UID = settingsService.GetUserAccountUid(dto.User_NTID);
                var ent = settingsService.QueryUsersSingle(dto.Account_UID);
                ent.User_NTID = dto.User_NTID;
                ent.User_Name = dto.User_Name;
                ent.Tel = dto.Tel;
                ent.Enable_Flag = dto.Enable_Flag;
                ent.Email = dto.Email;
                ent.Modified_UID = dto.Modified_UID;
                ent.Modified_Date = System.DateTime.Now;
                ent.EmployeePassword = dto.EmployeePassword == null ? null : eTransfer.PasswordUtil.EncryptionHelper.Encrypt(dto.EmployeePassword);
                ent.EmployeeNumber = dto.EmployeeNumber;
                ent.EnableEmpIdLogin = dto.EnableEmpIdLogin;
                ent.Building = dto.Building;
                ent.System_Language_UID = dto.System_Language_UID;
                settingsService.ModifyUser(ent);
                settingsService.ModifyOrgInfo(dto);
                settingsService.ModifyRoleInfo(dto);
                settingsService.ModifyProjectInfo(dto);
                return "SUCCESS";
            }
            catch (Exception e)
            {
                return "False" + e.ToString();
            }            
        }

        public void AddUserAPI(SystemUserInfo1 dto)
        {

            var result = new System_Users
            {
                User_NTID = dto.User_NTID,
                User_Name = dto.User_Name,
                Email = dto.Email,
                Tel = dto.Tel,
                LoginToken = dto.LoginToken,
                Modified_UID = dto.Modified_UID,
                Modified_Date = DateTime.Now,
                EnableEmpIdLogin = dto.EnableEmpIdLogin,
                EmployeeNumber = dto.EmployeeNumber,
                Building = dto.Building,
                EmployeePassword = dto.EmployeePassword == null ? null : eTransfer.PasswordUtil.EncryptionHelper.Encrypt(dto.EmployeePassword),
                System_Language_UID=dto.System_Language_UID
            };
            var userError=settingsService.AddUser(result);
            if (userError == "SUCCESS")
            {
                dto.Account_UID = settingsService.GetUserAccountUid(dto.User_NTID);
                //添加人员与组织架构关系
                settingsService.AddOrgInfo(dto);
                //添加人员与Role关系
                settingsService.AddRoleInfo(dto);
                //添加人员与Project关系
                settingsService.AddProjectInfo(dto);
            }
        }

        [HttpGet]
        public string ForbidUserAPI(int Account_UID)
        {
            var user = settingsService.QueryUsersSingle(Account_UID);
            return settingsService.ForbidUser(user) ? "SUCCESS" : "FAIL";
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeleteUserAPI(SystemUserDTO dto)
        {
            var ent = settingsService.QueryUsersSingle(dto.Account_UID);
            var result = settingsService.DeleteUser(ent) ? "SUCCESS" : "FAIL";
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportUserAPI(string uuids)
        {
            var users = settingsService.DoExportUser(uuids);
            return Ok(users);
        }
        #endregion //#region User Maintenance API

        #region PlantAPI-------------------Add By Sidney 2015/11/12
        /// <summary>
        /// get paged records of users by query conditions
        /// </summary>
        /// <param name="data">post search modal and page modal</param>
        /// <returns>json of paged records</returns>
        [HttpPost]
        public IHttpActionResult QueryPlantsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<PlantModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var Plants = settingsService.QueryPlants(searchModel, page);
            return Ok(Plants);
        }
        [HttpGet]
        public IHttpActionResult QueryPlantAPI(int uid)
        {
            var dto = new SystemPlantDTO();
            dto = AutoMapper.Mapper.Map<SystemPlantDTO>(settingsService.QueryPlantSingle(uid));
            return Ok(dto);
        }

        public string ModifyPlantAPI(SystemPlantDTO dto)
        {
            var ent = settingsService.QueryPlantSingle(dto.System_Plant_UID);
            ent.Address_EN = dto.Address_EN;
            ent.Address_ZH = dto.Address_ZH;
            ent.CCODE = dto.CCODE;
            ent.Legal_Entity_EN = dto.Legal_Entity_EN;
            ent.Legal_Entity_ZH = dto.Legal_Entity_ZH;
            ent.Location = dto.Location;
            ent.Name_0 = dto.Name_0;
            ent.Name_1 = dto.Name_1;
            ent.Name_2 = dto.Name_2;
            ent.Plant = dto.Plant;
            ent.Type = dto.Type;
            ent.PlantManager_Name = dto.PlantManager_Name;
            ent.PlantManager_Tel = dto.PlantManager_Tel;
            ent.PlantManager_Email = dto.PlantManager_Email;
            if (ent.End_Date == null && dto.End_Date != null)
            {
                ent.End_Date = dto.End_Date;
            }

            ent.Coordinate = dto.Coordinate;
            ent.Modified_UID = dto.Modified_UID;
            ent.Modified_Date = DateTime.Now;

            var plantstring = settingsService.ModifyPlant(ent);
            return plantstring;
        }

        public string AddPlantAPI(SystemPlantDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<System_Plant>(dto);
            ent.Modified_Date = DateTime.Now;
            var plantstring = settingsService.AddPlant(ent);
            if (plantstring != "SUCCESS")
                return plantstring;
            else
                return "SUCCESS";
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeletePlantAPI(SystemPlantDTO dto)
        {
            var ent = settingsService.QueryPlantSingle(dto.System_Plant_UID);
            var result = settingsService.DeletePlant(ent);
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportPlantAPI(string uuids)
        {
            var listEnt = settingsService.DoExportPlant(uuids);
            return Ok(listEnt);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public bool CheckPlantExistByUId(int uuid)
        {
            return settingsService.CheckPlantExistByUId(uuid);
        }
        [HttpGet]
        [IgnoreDBAuthorize]
        public bool CheckPlantExistByPlant(string Plant)
        {
            return settingsService.CheckPlantExistByPlant(Plant);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetMaxEnddate4Plant(int uid)
        {
            var enddate = settingsService.GetMaxEnddate4Plant(uid);
            if (enddate != null)
            {
                return Ok(new { Enddate = ((DateTime)enddate).ToString(FormatConstants.DateTimeFormatStringByDate) });
            }
            else
            {
                return Ok(new { Enddate = enddate });
            }

        }
        #endregion

        #region All of Rock's API ------------------Start--------Add by Rock 2015/11/18


        #region BU Master Maintenance API -------------------------Add by Rock 2015/11/09
        /// <summary>
        /// get paged records of users by query conditions
        /// </summary>
        /// <param name="data">post search modal and page modal</param>
        /// <returns>json of paged records</returns>
        [HttpPost]
        public IHttpActionResult QueryBUsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<BUModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var bus = settingsService.QueryBus(searchModel, page);
            return Ok(bus);
        }
        [HttpPost]
        public IHttpActionResult QueryBUD_OrgsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<BUD_OrgSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var bus = settingsService.QueryBU_Org(searchModel, page);
            return Ok(bus);
        }
        [HttpGet]
        [IgnoreDBAuthorize]
        public bool CheckBuExistById(string buid)
        {
            return settingsService.CheckBuExistById(buid, null);
        }

        //[HttpGet]
        //public bool CheckBuExistByName(string buname)
        //{
        //    return settingsService.CheckBuExistById(null, buname);
        //}

        [HttpGet]
        public IHttpActionResult QueryBUAPI(int BU_M_UID)
        {
            var dto = new SystemBUMDTO();
            dto = AutoMapper.Mapper.Map<SystemBUMDTO>(settingsService.QueryBUSingle(BU_M_UID));
            return Ok(dto);
        }

        public void AddBUAPI(SystemBUMDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<System_BU_M>(dto);
            ent.Modified_Date = DateTime.Now;
            settingsService.AddBU(ent);
        }

        public string ModifyBUAPI(SystemBUMDTO dto)
        {
            var ent = settingsService.QueryBUSingle(dto.BU_M_UID);
            ent.BU_Name = dto.BU_Name;
            ent.Begin_Date = dto.Begin_Date;
            ent.End_Date = dto.End_Date;
            ent.BUManager_Name = dto.BUManager_Name;
            ent.BUManager_Tel = dto.BUManager_Tel;
            ent.BUManager_Email = dto.BUManager_Email;
            ent.Modified_UID = dto.Modified_UID;
            ent.Modified_Date = DateTime.Now;
            settingsService.ModifyBU(ent);
            return "SUCCESS";
        }

        [AcceptVerbs("Post")]
        public string DeleteBUAPI(SystemBUMDTO dto)
        {
            var ent = settingsService.QueryBUSingle(dto.BU_M_UID);
            var result = settingsService.DeleteBU(ent) ? "SUCCESS" : "FAIL";
            return result;
        }


        [HttpGet]
        public IHttpActionResult DoExportBUAPI(string uuids)
        {
            var listDto = settingsService.DoExportBU(uuids);
            return Ok(listDto);
        }


        [HttpGet]
        public IHttpActionResult QueryBUD_OrgAPI(int System_BU_D_Org_UID)
        {
            var dto = new SystemBUD_OrgDTO();
            dto = AutoMapper.Mapper.Map<SystemBUD_OrgDTO>(settingsService.QueryBU_OrgSingle(System_BU_D_Org_UID));
            return Ok(dto);
        }

        public void AddBUD_OrgAPI(SystemBUD_OrgDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<System_BU_D_Org>(dto);
           
            settingsService.AddBU_Org(ent);
        }

        public string ModifyBUD_OrgAPI(SystemBUD_OrgDTO dto)
        {
            var ent = settingsService.QueryBU_OrgSingle(dto.System_BU_D_Org_UID);
            ent.BU_D_UID = dto.BU_D_UID;
            ent.Organization_UID = dto.Organization_UID;
            
            settingsService.ModifyBU_Org(ent);
            return "SUCCESS";
        }

        [AcceptVerbs("Post")]
        public string DeleteBUD_OrgAPI(SystemBUD_OrgDTO dto)
        {
            var ent = settingsService.QueryBU_OrgSingle(dto.System_BU_D_Org_UID);
            var result = settingsService.DeleteBU_Org(ent) ? "SUCCESS" : "FAIL";
            return result;
        }

        [HttpGet]
        public IHttpActionResult QueryBU_OrgAPI(int System_BU_D_Org_UID)
        {
            var dto = new SystemBUD_OrgDTO();
            dto = AutoMapper.Mapper.Map<SystemBUD_OrgDTO>(settingsService.QueryBU_OrgSingle(System_BU_D_Org_UID));
            return Ok(dto);
        }

        #endregion

        #region BU Detail Maintenance --------------------------- Add by Rock 2015/11/13
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryBUDetailsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<BUDetailModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var bus = settingsService.QueryBUDs(searchModel, page);
            return Ok(bus);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public bool CheckBuExistById_TwoAPI(string buid)
        {
            return settingsService.CheckBuExistById_Two(buid);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public string CheckBeginDateAndEndDateAPI(string BU_ID, string BU_Name, string Begin_Date, string End_Date)
        {
            return settingsService.CheckBeginDateAndEndDate(BU_ID, BU_Name, Begin_Date, End_Date);
        }



        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetBUIDAndBUNameByBUIDAPI(string BU_ID)
        {
            var dto = settingsService.GetBUIDAndBUNameByBUID(BU_ID);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryBUDInfoByBU_D_IDAPI(string BU_D_ID)
        {
            var dto = settingsService.QueryBUDInfoByBU_D_ID(BU_D_ID);
            return Ok(dto);
        }

        public void EditBUDetailInfoAPI(SystemBUDDTO dto, string BU_ID, string BU_Name, bool isEdit)
        {
            settingsService.AddOrEditBUDetailInfo(dto, BU_ID, BU_Name, isEdit);
        }

        public void AddBUDetailInfoAPI(SystemBUDDTO dto, string BU_ID, string BU_Name, bool isEdit)
        {
            settingsService.AddOrEditBUDetailInfo(dto, BU_ID, BU_Name, isEdit);
        }

        [HttpGet]
        public IHttpActionResult QueryBUDetailAPI(int BU_D_UID)
        {
            var dto = settingsService.QueryBUDSingle(BU_D_UID);
            return Ok(dto);
        }

        [HttpGet]
        public string DeleteBUDDetailAPI(int BU_D_UID)
        {
            var ent = settingsService.QueryBUDSingleByModule(BU_D_UID);
            var result = settingsService.DeleteBUDSingle(ent) ? "SUCCESS" : "FAIL";
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportBUDetailAPI(string BU_D_UIDS)
        {
            var list = settingsService.DoExportBUDetail(BU_D_UIDS);
            return Ok(list);
        }

        #endregion

        #region User BU Setting API-----------------Start-----------Add by Rock 2015/11/13
        [HttpPost]
        public IHttpActionResult QueryUserBUsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<UserBUSettingSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var bus = settingsService.QueryUserBUs(searchModel, page);
            return Ok(bus);
        }

        [HttpGet]
        public IHttpActionResult QueryUserBUAPI(int System_User_BU_UID)
        {
            var item = settingsService.QueryUserBU(System_User_BU_UID);
            return Ok(item);
        }

        [HttpGet]
        public IHttpActionResult QueryBUAndBUDSByBUIDAPI(string BU_ID)
        {
            var list = settingsService.QueryBUAndBUDSByBUIDS(BU_ID);
            return Ok(list);
        }

        public string AddUserBUAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<UserBUAddOrSave>(data.ToString());
            return settingsService.AddUserBU(entity);
        }

        public string EditUserBUAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<UserBUAddOrSave>(data.ToString());
            return settingsService.EditUserBU(entity);
        }

        [HttpGet]
        public IHttpActionResult DoExportUserBUAPI(string KeyIDS)
        {
            var list = settingsService.DoExportUserBU(KeyIDS);
            return Ok(list);

        }
        #endregion User BU Setting API-----------------End-----------Add by Rock 2015/11/13


        #endregion All of Rock's API ------------------End--------Add by Rock 2015/11/18

        //------------------------------------------------------------------

        #region Role Maintenance API------------------Add By Allen 2015/11/9

        /// <summary>
        /// get paged records of users by query conditions
        /// </summary>
        /// <param name="data">post search modal and page modal</param>
        /// <returns>json of paged records</returns>
        [HttpPost]
        public IHttpActionResult QueryRolesAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<RoleModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var role = settingsService.QueryRoles(searchModel, page);
            return Ok(role);

        }
        [HttpGet]
        public IHttpActionResult QueryRoleAPI(int uuid)
        {
            var dto = new SystemRoleDTO();
            dto = AutoMapper.Mapper.Map<SystemRoleDTO>(settingsService.QueryRolesSingle(uuid));
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult IsExistRoleIDAPI(string role_ID)
        {
            var dto = settingsService.IsExistRoleID(role_ID);
            return Ok(dto);
        }

        public void AddRoleAPI(SystemRoleDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<System_Role>(dto);
            ent.Modified_Date = DateTime.Now;
            settingsService.AddRole(ent);
        }
        public void ModifyRoleAPI(SystemRoleDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<System_Role>(dto);
            ent.Modified_Date = DateTime.Now;
            settingsService.ModifyRole(ent);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeleteRoleAPI(SystemRoleDTO dto)
        {
            var ent = settingsService.QueryRolesSingle(dto.Role_UID);
            var result = settingsService.DeleteRole(ent);
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportRoleAPI(string ruids)
        {
            return Ok(settingsService.DoExportRole(ruids));
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public bool CheckRoleExistById(string rid)
        {
            return settingsService.CheckRoleExistById(rid);
        }
        #endregion //Role Maintenance API

        #region System Function Maintenance module Add by Tonny 2015/11/12

        [AcceptVerbs("Post")]
        public IHttpActionResult QueryFunctionsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<FunctionModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var functions = settingsService.QueryFunctions(searchModel, page);
            return Ok(functions);
        }

        /// <summary>
        /// Get specific function and its subfunctions by key
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult QueryFunctionWithSubsAPI(int uid)
        {
            var entity = settingsService.QueryFunctionWithSubs(uid);
            System_Function parentFunction = null;

            if (entity.Parent_Function_UID != null)
            {
                parentFunction = settingsService.GetFunction((int)entity.Parent_Function_UID);
            }

            var result = new FunctionWithSubs
            {
                Function_UID = entity.Function_UID,
                Parent_Function_ID = parentFunction == null ? string.Empty : parentFunction.Function_ID,
                Parent_Function_Name = parentFunction == null ? string.Empty : parentFunction.Function_Name,
                Function_Name = entity.Function_Name,
                Function_ID = entity.Function_ID,
                Function_Desc = entity.Function_Desc,
                Is_Show = entity.Is_Show,
                Order_Index = entity.Order_Index,
                URL = entity.URL,
                Icon_ClassName = entity.Icon_ClassName,
                Mobile_URL = entity.Mobile_URL,
                FunctionSubs = AutoMapper.Mapper.Map<List<SystemFunctionSubDTO>>(entity.System_FunctionSub)
            };
            return Ok(result);
        }

        /// <summary>
        /// Add function and its sub funtions
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string AddFunctionWithSubsAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<FunctionWithSubs>(data.ToString());
            return settingsService.AddFunctionWithSubs(entity);
        }

        /// <summary>
        /// Modify function and its sub functions
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ModifyFunctionWithSubsAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<FunctionWithSubs>(data.ToString());
            return settingsService.ModifyFunctionWithSubs(entity);
        }

        /// <summary>
        /// Get specific function by function_id
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFunctionByIDAPI(string functionId)
        {
            var entity = settingsService.GetFunctionByID(functionId);
            var dto = entity == null ? new SystemFunctionDTO() : AutoMapper.Mapper.Map<SystemFunctionDTO>(entity);
            return Ok(dto);
        }

        /// <summary>
        /// Delete function
        /// </summary>
        /// <param name="uid">key</param>
        /// <returns>operate result</returns>
        [HttpGet]
        public string DeleteFunctionAPI(int uid)
        {
            return settingsService.DeleteFunction(uid);
        }

        [HttpGet]
        public string DeleteSubFunctionAPI(int subfunction_UId)
        {
            return settingsService.DeleteSubFunction(subfunction_UId);
        }

        [HttpGet]
        public IHttpActionResult DoExportFunctionAPI(string uids)
        {
            return Ok(settingsService.DoExportFunction(uids));
        }

        #endregion //end System Function Maintenance module

        #region User Plant Setting Add by Sidney 2015/11/17
        [HttpGet]
        [IgnoreDBAuthorize]
        public SystemPlantDTO GetPlantInfoAPI(string Plant)
        {
            var result = settingsService.GetPlantInfo(Plant);
            var result1 = AutoMapper.Mapper.Map<SystemPlantDTO>(result);
            return result1;
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public List<string> getFunPOrgIdAPI(string Plant)
        {
            var result = settingsService.getFunPOrg(Plant);
            //var result1 = AutoMapper.Mapper.Map<SystemPlantDTO>(result);
            return result;
        }

        [AcceptVerbs("Post")]
        public IHttpActionResult QueryUserPlantsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<UserPlantModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var userPlants = settingsService.QueryUserPlants(searchModel, page);
            return Ok(userPlants);
        }
        [HttpGet]
        public IHttpActionResult QueryUserPlantByAccountUID(int uuid)
        {
            var Users = commonService.GetSystemUserByUId(uuid);
            var userPlantList = settingsService.QueryUserPlantByAccountUID(uuid);

            var result = new UserPlantEditModel
            {
                Account_UID = Users.Account_UID,
                User_NTID = Users.User_NTID,
                User_Name = Users.User_Name,
                UserPlantWithPlants = userPlantList,
            };

            return Ok(result);
        }

        public string AddUserPlantAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<UserPlantEditModel>(data.ToString());
            return settingsService.AddUserPlantWithSubs(entity);
        }

        public string ModifyUserPlantAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<UserPlantEditModel>(data.ToString());
            return settingsService.ModifyUserPlantWithSubs(entity);
        }


        [HttpGet]
        public IHttpActionResult DoExportUserPlantAPI(string uids)
        {
            return Ok(settingsService.DoExportUserPlant(uids));
        }
        #endregion

        #region Role Function Setting Module -------------- Add by Tonny 2015/11/18

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryRoleFunctionsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<RoleFunctionSearchModel>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var roleFunctions = settingsService.QueryRoleFunctions(searchModel, page);

            return Ok(roleFunctions);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryRoleFunctionAPI(int uid)
        {
            var roleFunction = settingsService.QueryRoleFunction(uid);
            return Ok(roleFunction);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryRoleSubFunctionsAPI([FromUri] RoleSubFunctionSearchModel searchModel)
        {
            var roleSubFunctions = settingsService.QueryRoleSubFunctions(searchModel.Role_UID, searchModel.Function_UID);
            return Ok(roleSubFunctions);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult QuerySubFunctionsByFunctionUIDAPI(int uid)
        {
            var result = settingsService.QuerySubFunctionsByFunctionUID(uid);
            return Ok(result);
        }
        [IgnoreDBAuthorize]
        public string ModifyRoleFunctionWithSubsAPI(dynamic data,int AccountUId)
        {
            var entity = JsonConvert.DeserializeObject<RoleFunctionsWithSub>(data.ToString());
            return settingsService.MaintainRoleFunctionWithSubs(entity, AccountUId);
        }
        [IgnoreDBAuthorize]
        public string AddRoleFunctionWithSubsAPI(dynamic data,int AccountUId)
        {
            var entity = JsonConvert.DeserializeObject<RoleFunctionsWithSub>(data.ToString());
            return settingsService.MaintainRoleFunctionWithSubs(entity, AccountUId);
        }

        [HttpGet]
        public IHttpActionResult DeleteRoleFunctionAPI(int uid)
        {
            settingsService.DeleteRoleFunction(uid);
            return Ok("SUCCESS");
        }

        [HttpGet]
        public IHttpActionResult DoExportRoleFunctionsAPI(string uids)
        {
            return Ok(settingsService.DoExportRoleFunctions(uids));
        }
        #endregion

        #region User Role Setting API------------------Add By Allen 2015/11/16
        /// <summary>
        /// get paged records of users by query conditions
        /// </summary>
        /// <param name="data">post search modal and page modal</param>
        /// <returns>json of paged records</returns>
        [HttpPost]
        public IHttpActionResult QueryUserRolesAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<UserRoleSearchModel>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var userroles = settingsService.QueryUserRoles(searchModel, page);
            return Ok(userroles);

        }
        [HttpGet]
        public IHttpActionResult QueryUserRoleAPI(int uruid)
        {
            var dto = new UserRoleItem();
            dto = AutoMapper.Mapper.Map<UserRoleItem>(settingsService.QueryUserRolesSingle(uruid));
            return Ok(dto);
        }

        public string AddUserRoleAPI(SystemUserRoleDTO dto)
        {
            //检查是否已有相同的数据存在
            var ent = settingsService.GetSystemUserRole(dto.Account_UID, dto.Role_UID);
            if (ent == null)
            {
                var newEnt = AutoMapper.Mapper.Map<System_User_Role>(dto);
                newEnt.Modified_Date = DateTime.Now;
                settingsService.AddUserRole(newEnt);

                return "SUCCESS";
            }

            return "DATAEXIST";
        }

        /// <summary>
        /// Delete User Role
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeleteUserRoleAPI(SystemUserRoleDTO dto)
        {
            var ent = settingsService.QueryUserRolesSingle(dto.System_User_Role_UID);
            var result = settingsService.DeleteUserRole(ent) ? "SUCCESS" : "FAIL";
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportUserRoleAPI(string uruids)
        {
            return Ok(settingsService.DoExportUserRole(uruids));
        }

        [IgnoreDBAuthorize]
        public SystemRoleDTO GetRoleNameByIdAPI(string roleid)
        {
            return AutoMapper.Mapper.Map<SystemRoleDTO>(settingsService.GetRoleNameById(roleid));
        }

        #endregion //Role Maintenance API

        #region OrgAPI   by  justin

        [HttpPost]
        public IHttpActionResult QueryOrgsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OrgModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var Orgs = settingsService.QueryOrgs(searchModel, page);
            return Ok(Orgs);
        }
        [HttpGet]
        public IHttpActionResult QueryOrgAPI(int uuid)
        {
            var dto = new SystemOrgDTO();
            dto = AutoMapper.Mapper.Map<SystemOrgDTO>(settingsService.QueryOrgSingle(uuid));
            return Ok(dto);
        }

        public string ModifyOrgAPI(SystemOrgDTO dto)
        {
            var ent = settingsService.QueryOrgSingle(dto.Organization_UID);
            ent.Organization_ID = dto.Organization_ID;
            ent.Organization_Name = dto.Organization_Name;
            ent.Organization_Desc = dto.Organization_Desc;
            ent.Cost_Center = dto.Cost_Center;
            ent.OrgManager_Name = dto.OrgManager_Name;
            ent.OrgManager_Tel = dto.OrgManager_Tel;
            ent.OrgManager_Email = dto.OrgManager_Email;
            ent.Modified_UID = dto.Modified_UID;
            ent.Modified_Date = DateTime.Now;

            if (ent.End_Date == null)
            {
                ent.End_Date = dto.End_Date;
            }

            settingsService.ModifyOrg(ent);
            return "SUCCESS";
        }

        public string AddOrgAPI(SystemOrgDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<System_Organization>(dto);
            ent.Modified_Date = DateTime.Now;
            settingsService.AddOrg(ent);
            return "SUCCESS";
        }

        [HttpGet]
        public string DeleteOrgAPI(int uid)
        {
            var result = settingsService.DeleteOrg(uid) ? "SUCCESS" : "FAIL";
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportOrgAPI(string uids)
        {
            var listDto = settingsService.DoExportOrg(uids);
            return Ok(listDto);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public bool CheckOrgExistByIdAndName(string id, string name)
        {
            return settingsService.CheckOrgExistByIdAndName(id, name);
        }
        [HttpGet]
        [IgnoreDBAuthorize]
        public bool CheckOrgExistByIdAndNameWithUId(int uid, string id, string name)
        {
            return settingsService.CheckOrgExistByIdAndNameWithUId(uid, id, name);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetMaxEnddate4Org(int uid)
        {
            var enddate = settingsService.GetMaxEnddate4Org(uid);
            if (enddate != null)
            {
                return Ok(new { Enddate = ((DateTime)enddate).ToString(FormatConstants.DateTimeFormatStringByDate) });
            }
            else
            {
                return Ok(new { Enddate = enddate });
            }

        }
        #endregion

        #region OrgBomAPI   by  justin

        [HttpPost]
        public IHttpActionResult QueryOrgBomsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OrgBomModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var OrgBom = settingsService.QueryOrgBom(searchModel, page);
            return Ok(OrgBom);
        }

        [HttpGet]
        public IHttpActionResult QueryOrgBomAPI(int uid)
        {
            var dto = settingsService.QueryOrgBom(uid);
            return Ok(dto);
        }

        public string ModifyOrgBomAPI(SystemOrgBomDTO dto)
        {
            settingsService.ModifyOrgBom(dto);
            return "SUCCESS";
        }

        public string AddOrgBomAPI(SystemOrgBomDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<System_OrganizationBOM>(dto);
            ent.Modified_Date = DateTime.Now;
            var plantstring = settingsService.AddOrgBom(ent);
            if (plantstring != "SUCCESS")
                return plantstring;
            else
                return "SUCCESS";
        }

        [HttpGet]
        public IHttpActionResult DoExportOrgBomAPI(string uids)
        {
            var listDto = settingsService.DoExportOrgBom(uids);
            return Ok(listDto);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public bool CheckOrgBomExistAPI(int uid, int parentUId, int childUId, int index)
        {
            return settingsService.CheckOrgBomExist(uid, parentUId, childUId, index);
        }
        #endregion

        #region User Organization by Justin

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetOrgInfoAPI(string OrgID)
        {
            var orglist = settingsService.GetOrgInfo(OrgID);

            var result = AutoMapper.Mapper.Map<IEnumerable<SystemOrgDTO>>(orglist);
            return Ok(result);
        }

        [AcceptVerbs("Post")]
        public IHttpActionResult QueryUserOrgsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<UserOrgModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var userOrgs = settingsService.QueryUserOrgs(searchModel, page);
            return Ok(userOrgs);
        }

        [HttpGet]
        public IHttpActionResult QueryUserOrgsByAccountUIDAPI(int uuid)
        {
            var orgList = settingsService.QueryUserOrgsByAccountUID(uuid);
            var Users = commonService.GetSystemUserByUId(uuid);

            var result = new UserOrgEditModel
            {
                Account_UID = Users.Account_UID,
                User_NTID = Users.User_NTID,
                User_Name = Users.User_Name,
                UserOrgWithOrgs = orgList.AsEnumerable()
            };
            return Ok(result);
        }

        public string AddUserOrgAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<UserOrgEditModel>(data.ToString());
            return settingsService.AddUserOrgWithSubs(entity);
        }

        public string ModifyUserOrgAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<UserOrgEditModel>(data.ToString());
            return settingsService.ModifyUserOrgWithSubs(entity);
        }

        [HttpGet]
        public IHttpActionResult DoExportUserOrgAPI(string uids)
        {
            return Ok(settingsService.DoExportUserOrg(uids));
        }
        #endregion

        #region FuncPlant Maintenance---------------Add By Sidney 2016/02/23
        //[HttpPost]、[HttpGet]只是使用规范的问题，其在WEB层调用时需要与此匹配。同时IHttpActionResult表示返回HTTP的结果
        public string AddFuncPlantAPI(FuncPlantMaintanance fpm)
        {
            //获取Plant对应的Plant_UID
            var Plant_UID=settingsService.GetPlantByPlant(fpm.Plant).System_Plant_UID;
            SystemFunctionPlantDTO dto =new SystemFunctionPlantDTO();
            dto.System_FunPlant_UID = 0;
            dto.System_Plant_UID = Plant_UID;
            dto.OP_Types = fpm.OPType;
            dto.FunPlant = fpm.FunPlant;
            dto.FunPlant_Manager = fpm.Plant_Manager;
            dto.FunPlant_Contact = fpm.FuncPlant_Context;
            dto.Modified_UID = fpm.Modified_UID;
            dto.Modified_Date= DateTime.Now;
            var ent = AutoMapper.Mapper.Map<System_Function_Plant>(dto);
            var plantstring = settingsService.AddFuncPlant(ent);
            if (plantstring != "SUCCESS")
                return plantstring;
            else
                return "SUCCESS";
        }

        public System_Plant GetPlantByPlant(string Plant)
        {
            return settingsService.GetPlantByPlant(Plant);
        }
        [IgnoreDBAuthorize]
        [HttpPost]
        public IHttpActionResult QueryFuncPlantsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<FuncPlantSearchModel>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var FuncPlants = settingsService.QueryFuncPlants(searchModel, page);
            return Ok(FuncPlants);
        }
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetPlantSingleAPI()
        {
            var EnumEntity = settingsService.GetPlantSingle();
            return Ok(EnumEntity);
        }
        [HttpGet]
        public IHttpActionResult QueryFuncPlantAPI(int uuid)
        {
            var dto = new FuncPlantMaintanance();
            dto = settingsService.QueryFuncPlant(uuid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult DeleteFuncPlantAPI(int uuid)
        {
            var dto = settingsService.DeleteFuncPlant(uuid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult GetAllFuncPlantsAPI()
        {
            var dto = settingsService.GetAllFuncPlants();
            return Ok(dto);
        }

        public string ModifyFuncPlantAPI(FuncPlantMaintanance dto)
        {
            var ent = settingsService.QueryFuncPlantSingle(dto.System_FuncPlant_UID);
            ent.System_FunPlant_UID = dto.System_FuncPlant_UID;
            ent.OP_Types = dto.OPType;
            ent.FunPlant = dto.FunPlant;
            ent.FunPlant_Manager = dto.Plant_Manager;
            ent.FunPlant_Contact = dto.FuncPlant_Context;
            ent.Modified_UID = dto.Modified_UID;
            ent.Modified_Date = DateTime.Now;
            ent.FunPlant_OrganizationUID = int.Parse(dto.Organization_ID);
            var plantstring = settingsService.ModifyFuncPlant(ent);
            return plantstring;
        }

        [HttpGet]
        public IHttpActionResult DoExportFuncPlantAPI(string uuids)
        {
            var listEnt = settingsService.DoExportFuncPlant(uuids);
            return Ok(listEnt);
        }


        #endregion

        #region User FuncPlant Setting-----------------------------Add By Sidney 2016/03/01

        [HttpPost]
        public IHttpActionResult QueryUserFuncPlantsAPI(dynamic data)
        {
            var jsonData=data.ToString();
            var searchModel=JsonConvert.DeserializeObject<UserFuncPlantSearch>(jsonData);
            var page= JsonConvert.DeserializeObject<Page>(jsonData);
            var userFuncPlant= settingsService.QueryUserFuncPlants(searchModel, page);
            return Ok(userFuncPlant);
        }

        public IHttpActionResult GetFuncPlantAPI(string Plant, string OP_Types)
        {
            var userFuncPlant = settingsService.GetFuncPlant(Plant, OP_Types);
            return Ok(userFuncPlant);
        }
        public IHttpActionResult GetUserInfoAPI(string User_NTID)
        {
            var userFuncPlant = settingsService.GetUserInfo(User_NTID);
            return Ok(userFuncPlant);
        }

        public string EditUserFuncPlantAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<EditUserFuncPlant>(data.ToString());
            return settingsService.EditUserFuncPlantWithSubs(entity);
        }
        [HttpGet]
        public IHttpActionResult GetUserInfoByFuncPlantAPI(string FuncPlant)
        {
            var userPlantList = settingsService.GetUserInfoByFuncPlant(FuncPlant);
            var result = new EditUserFuncPlant
            {
                UserPlantWithPlants=userPlantList
            };

            return Ok(result);
        }
        #endregion
        #region 
        public string AddEnumeration(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<Enumeration>(data.ToString());
            return settingsService.AddEnumeration(entity);
        
        }

        public IHttpActionResult GetEnumNameForKeyProcess()
        {
            return Ok(settingsService.GetEnumNameForKeyProcess());
        }

        [AcceptVerbs("Post")]
        public IHttpActionResult GetEnumValueForKeyProcess(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<FlowChartModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var enumerations = settingsService.GetEnumValueForKeyProcess(searchModel, page);
            return Ok(enumerations);
        }
        [HttpGet]
        public IHttpActionResult GetEnumListAPI(string enumType)
        {
            return Ok(settingsService.GetEnumList(enumType));
        }
        [AcceptVerbs("Post")]
        public IHttpActionResult DeleteKeyProcessAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var entity = JsonConvert.DeserializeObject<Enumeration>(jsonData);
            var enumerations = settingsService.DeleteEnumeration(entity.Enum_UID);
            return Ok(enumerations);
        }

        #endregion

        #region
        [HttpPost]
        public IHttpActionResult Edit_WIPAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EditWIPDTO>(jsonData);
            var users = settingsService.Edit_WIP(searchModel);
            return Ok(users);
        }
        [HttpPost]
        public IHttpActionResult Edit_ProductAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EditProductDTO>(jsonData);
            var users = settingsService.Edit_Product(searchModel);
            return Ok(users);
        }
        [HttpPost]
        public IHttpActionResult Edit_FlagAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EditFlagDTO>(jsonData);
            var users = settingsService.Edit_Flag(searchModel);
            return Ok(users);
        }

        [HttpPost]
        public IHttpActionResult Edit_EnumAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EditEnumDTO>(jsonData);
            var users = settingsService.Edit_Enum(searchModel);
            return Ok(users);
        }

        [HttpPost]
        public IHttpActionResult ExeSqlAPI(dynamic data)
        {
            string jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<SQLDTO>(jsonData);
            var users = settingsService.ExeSql(searchModel.SQLText);
            return Ok(users);
        }

        [HttpPost]
        public string Add_EnumAPI(EditEnumDTO dto)
        {
          
            var ent = AutoMapper.Mapper.Map<Enumeration>(dto);
          
            var plantstring = settingsService.Add_Enum(ent);
            if (plantstring != "SUCCESS")
                return plantstring;
            else
                return "SUCCESS";
        }
        #endregion
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult getUserPlantAPI(int currentUser,string plant)
        {
            return Ok(settingsService.getUserPlant(currentUser,plant));
        }
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult getOrgAndProAPI(int currentUser,int opid)
        {
            return Ok(settingsService.getOrgAndPro(currentUser,opid));
        }

        #region Project Maintanance----------------Add By Sidney 2016/05/20
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryProjectsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ProjectSearchModel>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var Projects = settingsService.QueryProjects(searchModel, page);
            return Ok(Projects);
        }
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetCustomerAPI()
        {
            var EnumEntity = settingsService.GetCustomer();
            return Ok(EnumEntity);
        }
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProductPhaseAPI()
        {
            var EnumEntity = settingsService.GetProductPhase();
            return Ok(EnumEntity);
        }
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetOpTypesAPI(int plantuid,int oporguid)
        {
            var EnumEntity = settingsService.GetOpTypes(plantuid, oporguid);
            return Ok(EnumEntity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetPlantsAPI(int PlantOrgUid)
        {
            var EnumEntity = settingsService.GetPlants(PlantOrgUid);
            return Ok(EnumEntity);
        }

        [HttpGet]
        public IHttpActionResult QueryProjectAPI(int uuid)
        {
            var dto = new ProjectVM();
            dto = settingsService.QueryProjectSingle(uuid);
            return Ok(dto);
        }

        [HttpPost]
        public IHttpActionResult AddProjectAPI(dynamic jsonData)
        {
            var dto = jsonData.ToString();
            var projectDto = JsonConvert.DeserializeObject<ProjectVMDTO>(dto);
            var result = settingsService.AddProject(projectDto);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult ModifyProjectAPI(dynamic jsonData)
        {
            var dto = jsonData.ToString();
            var projectDto = JsonConvert.DeserializeObject<ProjectVMDTO>(dto);
            var result = settingsService.EditProject(projectDto);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DeleteProjectAPI(int project_Uid)
        {
            var result = settingsService.DeleteProject(project_Uid);
            return Ok(result);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetTypeClassfyAPI(int userUID)
        {
            var result = settingsService.GetTypeClassfy(userUID);
            return Ok(result);
        }

        #endregion

        #region Modify UserPassword--------------Add By Robert 2017/02/16
        public IHttpActionResult ChangePassWordAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<UserPasswordInfo>(jsonData);

            var users = settingsService.ChangeUserPassword(searchModel);
            return Ok(users);
        }
        [IgnoreDBAuthorize]
        public IHttpActionResult CheckAcountAPI(dynamic data)
        {
            var jsonData = data.ToString();
            UserPasswordInfo searchModel = JsonConvert.DeserializeObject<UserPasswordInfo>(jsonData);

            var users = settingsService.QueryUsersSingle(searchModel.UserId);
            return Ok(users.MH_Flag);
        }
        #endregion
        [IgnoreDBAuthorize]
        [AcceptVerbs("Get")]
        public IHttpActionResult GetProjectByOrgIdAPI(int org)
        {
            var f= AutoMapper.Mapper.Map<List<SystemProjectDTO>>(settingsService.GetProjectByOrgId(org));
            return Ok(f);
        }

        #region Language Maintenance --------------------------- Add By Rock 2017/07/18
        [HttpPost]
        public IHttpActionResult LanguagesInfoAPI()
        {
            var result = settingsService.LanguagesInfo();
            return Ok(result);
        }

        public IHttpActionResult LocaleStringResourceInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<SystemLocaleStringResourceDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = settingsService.LocaleStringResourceInfo(searchModel, page);
            return Ok(result);
        }

        public string SaveResourceInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<SystemLocaleStringResourceDTO>(jsonData);
            var result = settingsService.SaveResourceInfo(searchModel);
            return result;
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult LocaleStringResourceInfoByLanguageIdAPI(int System_Language_UID)
        {
            var result = settingsService.LocaleStringResourceInfoByLanguageId(System_Language_UID);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public string ImportLanguageExcelAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var list = JsonConvert.DeserializeObject<List<SystemLocaleStringResourceDTO>>(jsonData);
            var result = settingsService.ImportLanguageExcel(list);
            return result;
        }

        #endregion --------------------------- Add By Rock 2017/07/18

        #region 生产车间维护
        [HttpPost]
        public IHttpActionResult QueryWorkshopsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<WorkshopModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var workshops = settingsService.QueryWorkshops(searchModel, page);
            return Ok(workshops);
        }
        [HttpGet]
        public IHttpActionResult QueryWorkshopAPI(int uid)
        {
            var dto = new WorkshopDTO();
            dto = AutoMapper.Mapper.Map<WorkshopDTO>(settingsService.QueryWorkshopSingle(uid));
            return Ok(dto);
        }

        public string EditWorkshopAPI(WorkshopDTO dto)
        {
            var workshop = settingsService.QueryWorkshopSingle(dto.Workshop_UID);
            workshop.Plant_Organization_UID = dto.Plant_Organization_UID;
            workshop.BG_Organization_UID = dto.BG_Organization_UID;
            workshop.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
            workshop.Workshop_ID = dto.Workshop_ID;
            workshop.Workshop_Name = dto.Workshop_Name;
            workshop.Building_Name = dto.Building_Name;
            workshop.Floor_Name = dto.Floor_Name;
            workshop.Is_Enable = dto.Is_Enable;
            workshop.Modified_UID = dto.Modified_UID;
            workshop.Modified_Date = DateTime.Now;

            var Workshopstring = settingsService.EditWorkshop(workshop);
            return Workshopstring;
        }

        public string AddWorkshopAPI(WorkshopDTO dto)
        {
            var workshop = AutoMapper.Mapper.Map<Workshop>(dto);
            workshop.Created_UID = dto.Modified_UID;
            workshop.Created_Date = DateTime.Now;
            workshop.Modified_UID = dto.Modified_UID;
            workshop.Modified_Date = DateTime.Now;
            var workshopstring = settingsService.AddWorkshop(workshop);
            if (workshopstring != "SUCCESS")
                return workshopstring;
            else
                return "SUCCESS";
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeleteWorkshopAPI(WorkshopDTO dto)
        {
            
            var result = settingsService.DeleteWorkshop(dto.Workshop_UID);
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportWorkshopAPI(string uids)
        {
            var list = settingsService.DoExportWorkshop(uids);
            return Ok(list);
        }
        #endregion

        #region 制程维护
        [HttpPost]
        public IHttpActionResult QueryProcessInfosAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Process_InfoModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var workshops = settingsService.QueryProcess_Infos(searchModel, page);
            return Ok(workshops);
        }

        [HttpPost]
        public IHttpActionResult QueryProcessInfoListAPI(Process_InfoModelSearch search)
        {
            var processList = settingsService.QueryProcess_InfoList(search);
            return Ok(processList);
        }
        [HttpGet]
        public IHttpActionResult QueryProcessInfoAPI(int uid)
        {
            var dto = new Process_InfoDTO();
            dto = AutoMapper.Mapper.Map<Process_InfoDTO>(settingsService.QueryProcess_InfoSingle(uid));
            return Ok(dto);
        }

        public string EditProcessInfoAPI(Process_InfoDTO dto)
        {
            var process = settingsService.QueryProcess_InfoSingle(dto.Process_Info_UID);
            process.Plant_Organization_UID = dto.Plant_Organization_UID;
            process.BG_Organization_UID = dto.BG_Organization_UID;
            process.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
            string process_ID = null;
            if(dto.Process_ID!=null)
            {
                process_ID = dto.Process_ID.Trim();
            }
            string process_Name = null;
            if(dto.Process_Name!=null)
            {
                process_Name = dto.Process_Name.Trim();
            }
            string process_Desc = null;
            if (dto.Process_Desc != null)
            {
                process_Desc = dto.Process_Desc.Trim();
            }
            process.Process_ID = process_ID;
            process.Process_Name = process_Name;
            process.Process_Desc = process_Desc;
            process.Is_Enable = dto.Is_Enable;
            process.Modified_UID = dto.Modified_UID;
            process.Modified_Date = DateTime.Now;

            var Process_Infostring = settingsService.EditProcess_Info(process);
            return Process_Infostring;
        }

        public string AddProcessInfoAPI(Process_InfoDTO dto)
        {
            var process = AutoMapper.Mapper.Map<Process_Info>(dto);
            process.Created_UID = dto.Modified_UID;
            process.Created_Date = DateTime.Now;
            process.Modified_UID = dto.Modified_UID;
            process.Modified_Date = DateTime.Now;
            if(process.Process_ID!=null)
            {
                process.Process_ID = process.Process_ID.Trim();
            }
            if (process.Process_Name != null)
            {
                process.Process_Name = process.Process_Name.Trim();
            }
            if (process.Process_Desc != null)
            {
                process.Process_Desc = process.Process_Desc.Trim();
            }
            var workshopstring = settingsService.AddProcess_Info(process);
            if (workshopstring != "SUCCESS")
                return workshopstring;
            else
                return "SUCCESS";
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeleteProcessInfoAPI(Process_InfoDTO dto)
        {

            var result = settingsService.DeleteProcess_Info(dto.Process_Info_UID);
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportProcessInfoAPI(string uids)
        {
            var list = settingsService.DoExportProcess_Info(uids);
            return Ok(list); 
        }
        #endregion

        #region 表格内容多语言
        [HttpPost]
        public IHttpActionResult QuerySystemLocalizedPropertiesAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<System_LocalizedPropertyModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var workshops = settingsService.QuerySystemLocalizedProperties(searchModel, page);
            return Ok(workshops);
        }

        //[HttpPost]
        //public IHttpActionResult QuerySystemLocalizedPropertyListAPI(Process_InfoModelSearch search)
        //{
        //    var processList = settingsService.QueryProcess_InfoList(search);
        //    return Ok(processList);
        //}
        [HttpGet]
        public IHttpActionResult QuerySystemLocalizedPropertyAPI(int uid)
        {
            var dto = new System_LocalizedPropertyDTO();
            dto = AutoMapper.Mapper.Map<System_LocalizedPropertyDTO>(settingsService.QuerySystemLocalizedPropertySingle(uid));
            return Ok(dto);
        }

        public string AddSystemLocalizedPropertyAPI(System_LocalizedPropertyDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Table_Name) )
            {
                dto.Table_Name = dto.Table_Name.Trim();
            }
            if (!string.IsNullOrWhiteSpace(dto.Table_ColumnName))
            {
                dto.Table_ColumnName = dto.Table_ColumnName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(dto.ResourceValue))
            {
                dto.ResourceValue = dto.ResourceValue.Trim();
            }
            dto.Modified_UID = dto.Modified_UID;
            dto.Modified_Date = DateTime.Now;
            var workshopstring = settingsService.AddSystemLocalizedProperty(dto);
            if (workshopstring != "SUCCESS")
                return workshopstring;
            else
                return "SUCCESS";
        }

        public string EditSystemLocalizedPropertyAPI(System_LocalizedPropertyDTO dto)
        {
            var Process_Infostring = settingsService.EditSystemLocalizedProperty(dto);
            return Process_Infostring;
        }

        [HttpGet]
        public string DeleteSystemLocalizedPropertyAPI(int uid)
        {
            var result = settingsService.DeleteSystemLocalizedProperty(uid);
            return result;
        }
        #endregion

        #region Add By Rock 排程邮件设定 平安夜加班..... 2017/12/24
        public IHttpActionResult QueryBatchEmailSettingAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<SystemModuleEmailVM>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = settingsService.QueryBatchEmailSetting(searchModel, page);
            return Ok(result);

        }

        [HttpGet]
        public IHttpActionResult GetBatchEmailSettingFunctionAPI(bool isAdmin, int plantUID)
        {
            var result = settingsService.GetBatchEmailSettingFunction(isAdmin, plantUID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryBatchEmailSettingByEditAPI(int System_Email_Delivery_UID)
        {
            var item = settingsService.QueryBatchEmailSettingByEdit(System_Email_Delivery_UID);
            return Ok(item);

        }

        [HttpGet]
        public IHttpActionResult ChangeNTIDAPI(string User_NTID)
        {
            var result = settingsService.ChangeNTID(User_NTID);
            return Ok(result);
        }

        public string SaveBatchEmailSettingAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<SystemModuleEmailVM>(jsonData);
            var result = settingsService.SaveBatchEmailSetting(searchModel);
            return result;
        }

        public string CheckEmailIsErrorAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<SystemModuleEmailVM>(jsonData);
            var result = settingsService.CheckEmailIsError(searchModel);
            return result;

        }

        #endregion 平安夜加班..... 2017/12/24
    }
}