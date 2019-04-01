using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Service;
using System.Web.Http;
using PDMS.Model.ViewModels.Common;
using PDMS.Data;
using System.Linq;
using PDMS.Model;
using System.Collections.Generic;
using System;

namespace PDMS.WebAPI.Controllers
{
    public class CommonController : ApiControllerBase
    {
        #region Private interfaces properties
        private ICommonService commonService;
        private ISettingsService settingService;
        #endregion //Private interfaces properties

        #region Controller constructor
        public CommonController(ICommonService commonService, ISettingsService settingService)
        {
            this.commonService = commonService;
            this.settingService = settingService;
        }
        #endregion //Controller constructor

        #region System User
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetSystemUserByUId(int uid)
        {
            var targetUser = commonService.GetSystemUserByUId(uid);

            if (targetUser == null)
            {
                return NotFound();
            }

            return Ok(targetUser);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetSystemUserByNTId(string ntid,int islogin)
        {
            var targetUser = commonService.GetSystemUserByNTId(ntid, islogin);

            if (targetUser == null)
            {
                return NotFound();
            }

            return Ok(targetUser);
        }
        #endregion //System User

        /// <summary>
        /// 通过邮件地址判断用户是否在PIS系统中
        /// </summary>
        /// <param name="EmailAddr"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public bool CheckPISMUser(string EmailAddr)
        {
            return commonService.CheckPISMUser(EmailAddr);
        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetProcessPlanAPI(int projectUID, string process, DateTime date)
        {
            var result = commonService.GetProcessPlan(projectUID, process, date);
            return Ok(result);
        }


        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetAllRoles()
        {
            var roles = commonService.GetAllRoles();
            return Ok(roles);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetValidPlantsByUserUId(int uid)
        {
            var plants = commonService.GetValidPlantsByUserUId(uid);
            return Ok(plants);
        }
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetValidBUMsByUserUId(int uid)
        {
            var bums = commonService.GetValidBUMsByUserUId(uid);
            return Ok(bums);
        }
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetValidBUDsByUserUId(int uid)
        {
            var buds = commonService.GetValidBUDsByUserUId(uid);
            return Ok(buds);
        }
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetValidOrgsByUserUId(int uid)
        {
            var orgs = commonService.GetValidOrgsByUserUId(uid);
            return Ok(orgs);
        }
       
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetPermissonsByNTId(int uid)
        {
            var plants = commonService.GetPermissonsByNTId(uid);
            return Ok(plants);
        }
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetOptysByNTId(int uid)
        {
            var plants = commonService.GetOptysByNTId(uid);
            return Ok(plants);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetOrgInfoByUserUId(int Account_UId)
        {
            var plants = commonService.GetOrgInfoByUserUId(Account_UId);
            return Ok(plants);
        }

        //---App 
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetUserInfo(int uid)
        {
            var userInfo = commonService.GetUserInfo(uid);
            return Ok(userInfo);
        }

        /// <summary>
        /// 获取用户可以查询的客户
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetUserBU(int OrgUID)
        {
            var userInfo = commonService.GetUserBu(OrgUID);
            return Ok(userInfo);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetLanguagesAPI(int Language_UID)
        {
            using (var context = new SPPContext())
            {
                if (Language_UID == 0)
                    Language_UID = 2;
                var languages = context.System_Language.Where(m => m.Enable_Flag == true).ToList();
                LanguageVM vm = new LanguageVM();
                vm.Languages = AutoMapper.Mapper.Map<List<SystemLanguageDTO>>(languages);
                vm.CurrentLanguage = vm.Languages.Where(m => m.System_Language_UID.Equals(Language_UID)).First();
                return Ok(vm);
            }
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetLanguagesAllAPI()
        {
            using (var context = new SPPContext())
            {
                var languages = context.System_Language.Where(m => m.Enable_Flag == true).ToList();
                var dtos = AutoMapper.Mapper.Map<List<SystemLanguageDTO>>(languages);
                return Ok(dtos);
            }
        }
        [HttpGet]
        [IgnoreDBAuthorize]
        public string GetResourceValueByNameAPI(int languid, string resourceName)
        {
            var result = settingService.GetLocaleStringResource(languid,resourceName);
            return result;
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetUserUIDByEmail(string Email)
        {
            var userInfo = commonService.GetUserUIDByEmail(Email);
            return Ok(userInfo);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public int GetSelectLanguageId(int accountId)
        {
            var result = commonService.GetSelectLanguageId(accountId);
            return result;
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public void ChangeLanguageInfoAPI(int accountId, int languid)
        {
            commonService.ChangeLanguageInfo(accountId, languid);
        }
    }
}
