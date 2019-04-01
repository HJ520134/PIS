using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    public class ExceptionController : ApiControllerBase
    {
        IExceptionService exceptionService;
        public ExceptionController(IExceptionService exceptionService)
        {
            this.exceptionService = exceptionService;
        }

        #region 异常部门管理
        [HttpPost]
        public IHttpActionResult QueryExceptionDeptAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ExceptionDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = exceptionService.QueryExceptionDept(searchModel, page);
            return Ok(result);
        }
        [HttpPost]
        public IHttpActionResult AddExceptionDeptAPI(ExceptionDTO dto)
        {
            var result = exceptionService.AddExceptionDept(dto);

            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult FetchExceptionDeptsAPI(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var result = exceptionService.FetchExceptionDepts(Plant_Organization_UID, BG_Organization_UID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult FetchExceptionDeptAPI(int uid)
        {
            var result = exceptionService.FetchExceptionDept(uid);

            return Ok(result);
        }
        [HttpGet]
        public int DeleExceptionDeptAPI(int uid)
        {
            var result = exceptionService.DeleExceptionDept(uid);

            return result;
        }
        [HttpPost]
        public int UpdateExceptionDeptAPI(ExceptionDTO dto)
        {
            var result = exceptionService.UpdateExceptionDept(dto);

            return result;
        }
        #endregion

        #region 异常代码管理
        [HttpPost]
        public IHttpActionResult QueryExceptionCodeAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ExceptionDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = exceptionService.QueryExceptionCode(searchModel, page);
            return Ok(result);
        }
        [HttpPost]
        public IHttpActionResult AddExceptionCodeAPI(ExceptionDTO dto)
        {
            var result = exceptionService.AddExceptionCode(dto);

            return Ok(result);
        }
        /// <summary>
        /// 删除异常代码
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        [HttpGet]
        public int DeleExceptionCodeAPI(string IDs)
        {
            var result = exceptionService.DeleExceptionCode(IDs);

            return result;
        }
        [HttpGet]
        public IHttpActionResult FetchExceptionCodeAPI(int uid)
        {
            var result = exceptionService.FetchExceptionCode(uid);

            return Ok(result);
        }
        public int UpdateExceptionCodeAPI(ExceptionDTO dto)
        {
            var result = exceptionService.UpdateExceptionCodeAPI(dto);

            return result;
        }

        /// <summary>
        /// 根据部门ID获取所有异常编码
        /// </summary>
        /// <param name="deptUID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult FetchExceptionCodeBasedDeptAPI(int deptUID)
        {
            var result = exceptionService.FetchExceptionCodeBasedDept(deptUID);

            return Ok(result);
        }




        #endregion

        #region 异常记录管理
        public int ExceptionRecordAddAPI(ExceptionAddDTO dto)
        {
            var result = exceptionService.ExceptionRecordAdd(dto);

            return result;
        }
        [HttpGet]
        public List<Stations> FetchStationsAPI(int LineID)
        {
            var result = exceptionService.FetchStations(LineID);

            return result;
        }
        /// <summary>
        /// 获取班次
        /// </summary>
        /// <param name="ShiftTimeID"></param>
        /// <returns></returns>
        [HttpGet]
        public ShiftTime FetchShifTimeAPI(int ShiftTimeID)
        {
            var result = exceptionService.FetchShifTime(ShiftTimeID);

            return result;
        }
        [HttpGet]
        public List<ShiftTime> FetchAllShifTimeAPI()
        {
            var result = exceptionService.FetchAllShifTime();

            return result;
        }

        [HttpPost]
        public IHttpActionResult QueryExceptionRecordAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ExceptionDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = exceptionService.QueryExceptionRecord(searchModel, page);
            return Ok(result);
        }
        [HttpGet]
        public List<Line> FetchLineBasedPlantAPI(int plantuid, int bguid, int funuid)
        {
            var result = exceptionService.FetchGL_Line(plantuid, bguid, funuid);

            return result;
        }
        [HttpPost]
        public IHttpActionResult FetchGL_LineWithGroupAPI(ExceptionDTO dto)
        {
            var result = exceptionService.FetchGL_LineWithGroup(dto);

            return Ok(result);
        }
        [HttpGet]
        public List<Stations> FetchStationsBasedLineAPI(int uid)
        {
            var result = exceptionService.FetchStationsBasedLine(uid);

            return result;
        }
        [HttpPost]
        public IHttpActionResult FetchLineBasedPlantBGCustomerAPI(ExceptionDTO dto)
        {
            var result = exceptionService.FetchLineBasedPlantBGCustomer(dto);
            return Ok(result);
        }
        [HttpPost]
        public int CloseExceptionOrderAPI(ExceptionDTO dto)
        {
            var result = exceptionService.CloseExceptionOrder(dto);

            return result;
        }
        [HttpGet]
        public int DeleteExceptionOrderAPI(int uid)
        {
            var result = exceptionService.DeleteExceptionOrder(uid);
            return result;
        }
        [HttpPost]
        public int ExceptionReplyAPI(ReplyRecordDTO dto)
        {
            var result = exceptionService.ExceptionReply(dto);
            return result;
        }
        [HttpGet]
        public IHttpActionResult ViewRecordReplyAPI(int uid)
        {
            var result = exceptionService.ViewRecordReplyAPI(uid);
            return Ok(result);
        }
        [HttpPost]
        public IHttpActionResult FethAllEmailAPI(EmailSendDTO dto)
        {
            var result = exceptionService.FethAllEmail();
            return Ok(result);
        }

        [HttpPost]
        public int SendEmailExceptionAPI(EmailSendDTO dto)
        {
            var result = exceptionService.SendEmailException(dto);
            return result;
        }
        [HttpGet]
        public IHttpActionResult FetchShiftTimeDetailAPI(int uid)
        {
            var result = exceptionService.FetchShiftTimeDetail(uid);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult FetchShiftTimeBasedBGAPI(int plantuid, int bguid)
        {
            var result = exceptionService.FetchShiftTimeBasedBG(plantuid, bguid);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult QueryExceptionRecordDashboardAPI(DashboardSearchDTO dto)
        {
            var result = exceptionService.QueryExceptionRecordDashboard(dto);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult QueryDowntimeRecordsAPI(DashboardSearchDTO dto)
        {
            var result = exceptionService.QueryDowntimeRecords(dto);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult FetchExceptionDetailAPI(int uid)
        {
            var result = exceptionService.FetchExceptionDetail(uid);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult FetchLatestReplyAPI(int uid)
        {
            var result = exceptionService.FetchLatestReply(uid);
            return Ok(result);
        }
        
        
        #endregion

        #region 异常时间段管理
        [HttpPost]
        public int AddPeriodTimeAPI(ExceptionDTO dto)
        {
            var result = exceptionService.AddPeriodTime(dto);

            return result;
        }

        [HttpGet]
        public int DeletPeriodTimeAPI(string timeID)
        {
            var result = exceptionService.DeletPeriodTime(timeID);

            return result;
        }

        [HttpPost]
        public ExceptionDTO AddExceptionEmailAPI(ExceptionDTO dto)
        {
            var result = exceptionService.AddExceptionEmail(dto);

            return result;
        }
        [HttpPost]
        public IHttpActionResult QueryExceptionEmailAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ExceptionDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = exceptionService.QueryExceptionEmail(searchModel, page);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult FetchExceptionEmailInfoAPI(int uid)
        {
            var result = exceptionService.FetchExceptionEmailInfo(uid);
            return Ok(result);
        }
        [HttpPost]
        public IHttpActionResult QueryExceptionTimeAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ExceptionDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = exceptionService.QueryExceptionTime(searchModel, page);
            return Ok(result);
        }
        [HttpPost]
        public int EditExceptionEmailAPI(ExceptionDTO dto)
        {
            var result = exceptionService.EditExceptionEmail(dto);

            return result;
        }
        [HttpGet]
        public int DeleExceptionEmailAPI(int uid)
        {
            var result = exceptionService.DeleExceptionEmail(uid);

            return result;
        }
        [HttpGet]
        public List<string> FetchEmailAPI(int uid)
        {
            var result = exceptionService.FetchEmail(uid);

            return result;
        }
        [HttpGet]
        public List<string> FetchEmailCCAPI(int uid)
        {
            var result = exceptionService.FetchEmailCC(uid);

            return result;
        }
        [HttpPost]
        public IHttpActionResult ExportExcptionRecord2ExcelAPI(ExceptionDTO dto)
        {
            var result = exceptionService.ExportFixtrueReturn2Excel(dto);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult ExportSomeRecord2Excel(string uids)
        {
            var result = exceptionService.ExportSomeRecord2Excel(uids);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult FetchUserInfoAPI(string User_NTID)
        {
            var result = exceptionService.FetchUserInfo(User_NTID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult FetchExceptionProjectAPI(int uid)
        {
            var result = exceptionService.FetchExceptionProject(uid);
            return Ok(result);
        }
        [HttpGet]
        public List<ExceptionDTO> FetchPeriodTimeBasedDeptIDAPI(int deptID)
        {
            var result = exceptionService.FetchPeriodTimeBasedDeptID(deptID);

            return result;
        }
        [HttpGet]
        public List<ExceptionDTO> FetchPeriodTimeBasedProjectIDAPI(int projectID)
        {
            var result = exceptionService.FetchPeriodTimeBasedProjectID(projectID);

            return result;
        }
        [HttpGet]
        public int UpdateDeptTimeAPI(int deptID, int dealyDayNub, int dayPeriod, int sendMaxTime)
        {
            var result = exceptionService.UpdateDeptTime(deptID, dealyDayNub, dayPeriod, sendMaxTime);

            return result;
        }
        [HttpPost]
        public int UpdateProjectTimeAPI(ExceptionProjectDTO dto)
        {
            var result = exceptionService.UpdateProjectTime(dto);

            return result;
        }
        [HttpGet]
        public IHttpActionResult FetchExceptionProjectCycleTimeAPI(int uid)
        {
            var result = exceptionService.FetchExceptionProjectCycleTime(uid);

            return Ok(result);
        }
        #endregion


    }
}