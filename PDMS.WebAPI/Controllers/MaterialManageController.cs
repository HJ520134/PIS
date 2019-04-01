using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Data;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Web.Http;
using PDMS.Common.Constants;
using System.Linq;
using PDMS.Core.Authentication;
using PDMS.Model.EntityDTO;

namespace PDMS.WebAPI.Controllers
{
    public class MaterialManageController : ApiController
    {
        IMaterialManageService materialManageService;

        #region 备品备件申请
        public MaterialManageController(IMaterialManageService materialManageService)
        {
            this.materialManageService = materialManageService;
        }

        //[HttpPost]
        //public IHttpActionResult QueryMaterialApplyAPI(dynamic data)
        //{
        //    var jsonData = data.ToString();
        //    var searchModel = JsonConvert.DeserializeObject<MaterialManageDTO>(jsonData);
        //    var page = JsonConvert.DeserializeObject<Page>(jsonData);
        //    var bus = materialManageService.QueryMaterialApplyInfo(searchModel, page);
        //    return Ok(bus);
        //}

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetAllMatAPI()
        {
            var mats = materialManageService.GetAllMat();
            return Ok(mats);
        }

        [HttpGet]
        public IHttpActionResult QueryMatinfoAPI(int Material_Uid)
        {
            if (Material_Uid == 0)
                return Ok("");
            var dto = materialManageService.QueryMatSingle(Material_Uid);
            return Ok(dto);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetAllUserAPI()
        {
            var users = materialManageService.GetAllUser();
            return Ok(users);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetAllLocationAPI(int Plant_Organization_UID)
        {
            var users = materialManageService.GetAllLocation( Plant_Organization_UID);
            return Ok(users);
        }

        //public string AddOrEditMaterialApplyAPI(MaterialManageDTO dto, bool isEdit)
        //{
        //    var result = materialManageService.AddOrEditMaterialApply(dto, isEdit);
        //    return result;
        //}

        //[HttpGet]
        //public IHttpActionResult QueryMaterialApplyByUidAPI(int Material_Apply_Uid)
        //{
        //    var dto = materialManageService.QueryMaterialApplySingle(Material_Apply_Uid);
        //    return Ok(dto);
        //}

        [HttpGet]
        public IHttpActionResult QueryDepartByUserAPI(int Userid)
        {
            if (Userid == 0)
                return Ok("");
            var dto = materialManageService.QueryDepartSingle(Userid);
            return Ok(dto);
        }

        //[HttpGet]
        //public IHttpActionResult DoExportFunctionAPI(string materialid, string materialname, string materialtypes,
        //    string classification)
        //{
        //    var dto = materialManageService.DoExportFunction(materialid, materialname, materialtypes, classification);
        //    return Ok(dto);
        //}
        #endregion
        #region 仓库管理
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetAllMatLocationAPI()
        {
            var users = materialManageService.GetAllMatLocation();
            return Ok(users);
        }
        #endregion
    }
}