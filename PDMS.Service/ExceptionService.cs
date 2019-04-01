using PDMS.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;
using AutoMapper;
using PDMS.Data;
using PDMS.Model;
using System.Diagnostics;

namespace PDMS.Service
{
    public interface IExceptionService
    {
        ExceptionDTO AddExceptionDept(ExceptionDTO dto);
        ExceptionDTO AddExceptionCode(ExceptionDTO dto);
        List<Depts> FetchExceptionDepts(int Plant_Organization_UID, int BG_Organization_UID);
        int DeleExceptionCode(string IDs);
        PagedListModel<ExceptionDTO> QueryExceptionCode(ExceptionDTO searchModel, Page page);
        PagedListModel<ExceptionDTO> QueryExceptionDept(ExceptionDTO searchModel, Page page);
        ExceptionDTO FetchExceptionDept(int uid);
        int DeleExceptionDept(int uid);
        ExceptionDTO FetchExceptionCode(int uid);
        int UpdateExceptionDept(ExceptionDTO dto);
        int UpdateExceptionCodeAPI(ExceptionDTO dto);
        List<ExceptionDTO> FetchExceptionCodeBasedDept(int deptUID);
        int ExceptionRecordAdd(ExceptionAddDTO dto);
        List<Stations> FetchStations(int lineID);
        ShiftTime FetchShifTime(int ShiftTimeID);
        int AddPeriodTime(ExceptionDTO dto);
        List<ExceptionDTO> FetchPeriodTimeBasedDeptID(int deptID);
        int DeletPeriodTime(string timeID);
        int UpdateDeptTime(int deptID, int dealyDayNub, int dayPeriod, int sendMaxTime);

        ExceptionDTO AddExceptionEmail(ExceptionDTO dto);
        PagedListModel<ExceptionDTO> QueryExceptionEmail(ExceptionDTO searchModel, dynamic page);
        ExceptionDTO FetchExceptionEmailInfo(int uid);
        int EditExceptionEmail(ExceptionDTO dto);
        int DeleExceptionEmail(int uid);
        PagedListModel<ExceptionDTO> QueryExceptionRecord(ExceptionDTO searchModel, Page page);
        List<Line> FetchGL_Line(int plantuid, int bguid, int funuid);
        List<Line> FetchGL_LineWithGroup(ExceptionDTO dto);
        List<Stations> FetchStationsBasedLine(int uid);
        List<ShiftTime> FetchAllShifTime();
        int CloseExceptionOrder(ExceptionDTO dto);
        int DeleteExceptionOrder(int uid);
        int ExceptionReply(ReplyRecordDTO dto);
        List<string> FetchEmail(int uid);
        List<string> FetchEmailCC(int uid);
        void ExceptionShedule();
        List<ExceptionDTO> ExportFixtrueReturn2Excel(ExceptionDTO dto);
        PagedListModel<ExceptionDTO> QueryExceptionTime(ExceptionDTO searchModel, dynamic page);
        ExceptionDTO FetchUserInfo(string nTID);
        List<Line> FetchLineBasedPlantBGCustomer(ExceptionDTO dto);
        List<ReplyRecordDTO> ViewRecordReplyAPI(int uid);
        List<SystemUserDTO> FethAllEmail();
        int SendEmailException(EmailSendDTO dto);
        List<Projects> FetchExceptionProject(int uid);
        List<ExceptionDTO> FetchPeriodTimeBasedProjectID(int projectID);
        int UpdateProjectTime(ExceptionProjectDTO dto);
        ExceptionProjectDTO FetchExceptionProjectCycleTime(int uid);
        List<ExceptionDTO> ExportSomeRecord2Excel(string uids);
        ShiftTime FetchShiftTimeDetail(int uid);
        List<ShiftTime> FetchShiftTimeBasedBG(int plantuid, int bguid);
        ChartsModel QueryExceptionRecordDashboard(DashboardSearchDTO dto);
        List<ExceptionDTO> QueryDowntimeRecords(DashboardSearchDTO dto);
        ExceptionDTO FetchExceptionDetail(int uid);
        ReplyRecordDTO FetchLatestReply(int uid);
    }
    public class ExceptionService : IExceptionService
    {
        #region 变量和构造函数
        /// <summary>
        /// 异常部门
        /// </summary>
        private readonly IException_DeptRepository exception_DeptRepository;
        /// <summary>
        /// 异常编码
        /// </summary>
        private readonly IException_CodeRepository exception_CodeRepository;
        /// <summary>
        /// 异常记录
        /// </summary>
        private readonly IException_RecordRepository exception_RecordRepository;
        /// <summary>
        /// 异常时间段
        /// </summary>
        private readonly IException_TimeRepository exception_TimeRepository;
        /// <summary>
        /// 异常收件人
        /// </summary>
        private readonly IException_EmailRepository exception_EmailRepository;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExceptionService(IException_DeptRepository exception_DeptRepository, IException_CodeRepository exception_CodeRepository, IException_RecordRepository exception_RecordRepository, IException_TimeRepository exception_TimeRepository, IException_EmailRepository exception_EmailRepository)
        {
            this.exception_DeptRepository = exception_DeptRepository;
            this.exception_CodeRepository = exception_CodeRepository;
            this.exception_RecordRepository = exception_RecordRepository;
            this.exception_TimeRepository = exception_TimeRepository;
            this.exception_EmailRepository = exception_EmailRepository;
        }
        #endregion

        #region 添加异常部门
        public PagedListModel<ExceptionDTO> QueryExceptionDept(ExceptionDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = exception_DeptRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<ExceptionDTO>(totalcount, result);
        }
        public ExceptionDTO AddExceptionDept(ExceptionDTO dto)
        {
            var entity = exception_DeptRepository.AddExceptionDept(dto);
            return entity;
        }
        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <returns></returns>
        public List<Depts> FetchExceptionDepts(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var entitys = exception_DeptRepository.FetchExceptionDepts(Plant_Organization_UID, BG_Organization_UID);

            return entitys;
        }

        /// <summary>
        /// 获取部门
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ExceptionDTO FetchExceptionDept(int uid)
        {
            var entity = exception_DeptRepository.FetchExceptionDept(uid);

            return entity;
        }

        public int DeleExceptionDept(int uid)
        {
            var ret = exception_DeptRepository.DeleExceptionDept(uid);

            return ret;
        }
        public int UpdateExceptionDept(ExceptionDTO dto)
        {
            var ret = exception_DeptRepository.UpdateExceptionDept(dto);

            return ret;
        }
        #endregion

        #region 添加异常编码
        public ExceptionDTO AddExceptionCode(ExceptionDTO dto)
        {
            var entity = exception_CodeRepository.AddExceptionCode(dto);

            return entity;
        }
        //删除
        public int DeleExceptionCode(string IDs)
        {
            var ret = exception_CodeRepository.DeleExceptionCode(IDs);

            return ret;
        }

        public PagedListModel<ExceptionDTO> QueryExceptionCode(ExceptionDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = exception_CodeRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<ExceptionDTO>(totalcount, result);
        }

        public ExceptionDTO FetchExceptionCode(int uid)
        {
            var entity = exception_CodeRepository.FetchExceptionCode(uid);

            return entity;

        }

        public int UpdateExceptionCodeAPI(ExceptionDTO dto)
        {
            var ret = exception_CodeRepository.UpdateExceptionCode(dto);

            return ret;
        }

        public List<ExceptionDTO> FetchExceptionCodeBasedDept(int deptUID)
        {
            var ret = exception_CodeRepository.FetchExceptionCodeBasedDept(deptUID);

            return ret;
        }

        #endregion

        #region 异常记录管理
        public int ExceptionRecordAdd(ExceptionAddDTO dto)
        {
            var ret = exception_RecordRepository.ExceptionRecordAdd(dto);
            return ret;
        }

        public List<Stations> FetchStations(int lineID)
        {
            var ret = exception_RecordRepository.FetchStations(lineID);
            return ret;
        }

        public ShiftTime FetchShifTime(int ShiftTimeID)
        {
            var ret = exception_RecordRepository.FetchShifTime(ShiftTimeID);
            return ret;
        }
        public PagedListModel<ExceptionDTO> QueryExceptionRecord(ExceptionDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = exception_RecordRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<ExceptionDTO>(totalcount, result);
        }

        public List<Line> FetchGL_Line(int plantuid, int bguid, int funuid)
        {
            var ret = exception_RecordRepository.FetchGL_Line(plantuid, bguid, funuid);
            return ret;
        }

        public List<Line> FetchGL_LineWithGroup(ExceptionDTO dto)
        {
            var ret = exception_RecordRepository.FetchGL_LineWithGroup(dto);
            return ret;
        }

        
        public List<Stations> FetchStationsBasedLine(int uid)
        {
            var ret = exception_RecordRepository.FetchStationsBasedLine(uid);
            return ret;
        }

        public List<ShiftTime> FetchAllShifTime()
        {
            var ret = exception_RecordRepository.FetchAllShifTime();
            return ret;
        }

        public int CloseExceptionOrder(ExceptionDTO dto)
        {
            var ret = exception_RecordRepository.CloseExceptionOrder(dto);
            return ret;
        }
        public int DeleteExceptionOrder(int uid)
        {
            var ret = exception_RecordRepository.DeleteExceptionOrder(uid);
            return ret;
        }
        public int ExceptionReply(ReplyRecordDTO dto)
        {
            var ret = exception_RecordRepository.ExceptionReply(dto);
            return ret;
        }
        public void ExceptionShedule()
        {
            exception_RecordRepository.ExceptionShedule();
        }
        public List<ExceptionDTO> ExportFixtrueReturn2Excel(ExceptionDTO dto)
        {
            var ret = exception_RecordRepository.ExportFixtrueReturn2Excel(dto);
            return ret;
        }
        public List<ExceptionDTO> ExportSomeRecord2Excel(string uids)
        {
            var ret = exception_RecordRepository.ExportSomeRecord2Excel(uids);
            return ret;
        }

        public List<Line> FetchLineBasedPlantBGCustomer(ExceptionDTO dto)
        {
            var ret = exception_RecordRepository.FetchLineBasedPlantBGCustomer(dto);
            return ret;
        }
        public List<ReplyRecordDTO> ViewRecordReplyAPI(int uid)
        {
            var ret = exception_RecordRepository.ViewRecordReplyAPI(uid);
            return ret;
        }
        public List<SystemUserDTO> FethAllEmail()
        {
            var ret = exception_RecordRepository.FethAllEmail();
            return ret;
        }
        public int SendEmailException(EmailSendDTO dto)
        {
            var ret = exception_RecordRepository.SendEmailException(dto);
            return ret;
        }

        public ShiftTime FetchShiftTimeDetail(int uid)
        {
            var ret = exception_RecordRepository.FetchShiftTimeDetail(uid);
            return ret;
        }
        public List<ShiftTime> FetchShiftTimeBasedBG(int plantuid, int bguid)
        {
            var ret = exception_RecordRepository.FetchShiftTimeBasedBG(plantuid, bguid);
            return ret;
        }
        public ChartsModel QueryExceptionRecordDashboard(DashboardSearchDTO dto)
        {
            var ret = exception_RecordRepository.QueryExceptionRecordDashboard(dto);
            return ret;
        }
        public List<ExceptionDTO> QueryDowntimeRecords(DashboardSearchDTO dto)
        {
            var ret = exception_RecordRepository.QueryDowntimeRecords(dto);
            return ret;
        }
        public ExceptionDTO FetchExceptionDetail(int uid)
        {
            var ret = exception_RecordRepository.FetchExceptionDetail(uid);
            return ret;
        }
        public ReplyRecordDTO FetchLatestReply(int uid)
        {
            var ret = exception_RecordRepository.FetchLatestReply(uid);
            return ret;
        }
        #endregion

        #region 异常时间点添加

        public PagedListModel<ExceptionDTO> QueryExceptionTime(ExceptionDTO searchModel, dynamic page)
        {
            var totalcount = 0;
            var result = exception_TimeRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<ExceptionDTO>(totalcount, result);
        }

        public int AddPeriodTime(ExceptionDTO dto)
        {
            var ret = exception_TimeRepository.AddPeriodTime(dto);
            return ret;
        }

        public List<ExceptionDTO> FetchPeriodTimeBasedDeptID(int deptID)
        {
            var ret = exception_TimeRepository.FetchPeriodTimeBasedDeptID(deptID);
            return ret;
        }

        public List<ExceptionDTO> FetchPeriodTimeBasedProjectID(int projectID)
        {
            var ret = exception_TimeRepository.FetchPeriodTimeBasedProjectID(projectID);
            return ret;
        }

        public int DeletPeriodTime(string timeID)
        {
            var ret = exception_TimeRepository.DeletPeriodTime(timeID);
            return ret;
        }

        public int UpdateDeptTime(int deptID, int dealyDayNub, int dayPeriod, int sendMaxTime)
        {
            var ret = exception_TimeRepository.UpdateDeptTime(deptID, dealyDayNub, dayPeriod, sendMaxTime);
            return ret;
        }
        public int UpdateProjectTime(ExceptionProjectDTO dto)
        {
            var ret = exception_TimeRepository.UpdateProjectTime(dto);
            return ret;
        }

        public List<Projects> FetchExceptionProject(int uid)
        {
            var ret = exception_TimeRepository.FetchExceptionProject(uid);
            return ret;
        }

        public ExceptionProjectDTO FetchExceptionProjectCycleTime(int uid)
        {
            var ret = exception_TimeRepository.FetchExceptionProjectCycleTime(uid);
            return ret;
        }


        #endregion

        #region 异常收件人管理
        public ExceptionDTO AddExceptionEmail(ExceptionDTO dto)
        {
            var ret = exception_EmailRepository.AddExceptionEmail(dto);
            return ret;
        }

        public PagedListModel<ExceptionDTO> QueryExceptionEmail(ExceptionDTO searchModel, dynamic page)
        {
            var totalcount = 0;
            var result = exception_EmailRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<ExceptionDTO>(totalcount, result);
        }

        public ExceptionDTO FetchExceptionEmailInfo(int uid)
        {
            var ret = exception_EmailRepository.FetchExceptionEmailInfo(uid);
            return ret;
        }

        public int EditExceptionEmail(ExceptionDTO dto)
        {
            var ret = exception_EmailRepository.EditExceptionEmail(dto);
            return ret;
        }

        public int DeleExceptionEmail(int uid)
        {
            var ret = exception_EmailRepository.DeleExceptionEmail(uid);
            return ret;
        }

        public List<string> FetchEmail(int uid)
        {
            var ret = exception_EmailRepository.FetchEmail(uid);
            return ret;
        }

        public List<string> FetchEmailCC(int uid)
        {
            var ret = exception_EmailRepository.FetchEmailCC(uid);
            return ret;
        }
        public ExceptionDTO FetchUserInfo(string nTID)
        {
            var ret = exception_EmailRepository.FetchUserInfo(nTID);
            return ret;
        }









        #endregion



    }
}
