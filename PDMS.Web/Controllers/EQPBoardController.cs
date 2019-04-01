using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Mvc;
using PDMS.Model.ViewModels.Settings;
using System.Web.Helpers;
using System.Net;
using System.Web.Script.Serialization;
using PDMS.Core.Authentication;
using PDMS.Core.Filters;
using System.Web;
using System.Web.SessionState;
using PDMS.Model.EntityDTO;

namespace PDMS.Web.Controllers
{
    public class EQPBoardController : WebControllerBase
    {
        public int GetPlantOrgUid()
        {
            int plantorguid = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                plantorguid = (int)CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
            return plantorguid;
        }
        // GET: EQPBoard
        public ActionResult Index()
        {

            int Plant_Organization_UID= GetPlantOrgUid();
            EQPRepairVM currentVM = new EQPRepairVM();
            var apiUrl =string.Format(@"Equipmentmaintenance/GetEQPLocationAPI?Plant_Organization_UID={0}", Plant_Organization_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var eqplocation = JsonConvert.DeserializeObject<List<string>>(result);
            currentVM.eqplocation = eqplocation;
            return View(currentVM);
        }

        public ActionResult EQPBoardShow(string location)
        {
            ViewBag.location = location;
            return View();
        }

        public ActionResult GetEQPBoard(EQPRepairInfoDTO search, Page page)
        {
            var apiUrl = string.Format("Equipmentmaintenance/GetEQPBoardAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
    }
} 