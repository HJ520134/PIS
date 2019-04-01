using PDMS.Data;
using PDMS.Data.Repository;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IGL_LineService : IBaseSevice<GL_Line, GL_LineDTO, GL_LineModelSearch>
    {
        
    }

    #region define LineGroup interface Add By Roy 2018/12/24
    public interface IGL_GroupLineService
    {
        PagedListModel<GroupLineItem> QueryGroupLines(GL_LineModelSearch searchModel, Page page);
        GL_LineGroupDTO QueryGroupLine(int uid);
        string AddGroupLine(GL_LineGroupDTO vm);
        string ModifyGroupLine(GL_LineGroupDTO vm);
        string RemoveGroupLine(int uid);
        string AddSubToGroup(GL_LineGroupDTO vm);
        List<GL_LineGroupDTO> GetGroupLine(int? Optype, int? Optypes, int? opFunPlant,int? customerId);
        List<SubLineItem> GetSubLine(int Oporgid,int Optype);
    }

    public class GL_GroupLineService : IGL_GroupLineService
    {
        private readonly IGL_LineGroupRepository lineGroupRepository;
        private readonly IUnitOfWork unitOfWork;
        public GL_GroupLineService(IGL_LineGroupRepository lineGroupRepository, IUnitOfWork unitOfWork)
        {
            this.lineGroupRepository = lineGroupRepository;
            this.unitOfWork = unitOfWork;
        }
        public PagedListModel<GroupLineItem> QueryGroupLines(GL_LineModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var groupLines = lineGroupRepository.QueryGroupLines(searchModel, page, out totalCount).AsEnumerable();

            return new PagedListModel<GroupLineItem>(totalCount, groupLines);
        }

        public GL_LineGroupDTO QueryGroupLine(int uid)
        {
            return lineGroupRepository.QueryGroupLine(uid);
        }

        public string AddGroupLine(GL_LineGroupDTO vm)
        {
            if (lineGroupRepository.GetMany(c => c.LineName == vm.LineName).Count() > 0)
            {
                return string.Format("This LineName [{0}] is already exist!", vm.LineName);
            }

            var now = DateTime.Now;
            var groupLineEntity = new GL_LineGroup();
            groupLineEntity.Plant_Organization_UID = vm.Plant_Organization_UID;
            groupLineEntity.BG_Organization_UID = vm.BG_Organization_UID;
            groupLineEntity.FunPlant_Organization_UID = vm.FunPlant_Organization_UID;
            groupLineEntity.LineName = vm.LineName;
            groupLineEntity.Modified_UID = vm._Modified_UID;
            groupLineEntity.Modified_Date = now;
            groupLineEntity.CustomerID = vm.CustomerID;
            lineGroupRepository.Add(groupLineEntity);
            unitOfWork.Commit();

            return "SUCCESS";
        }

        public string ModifyGroupLine(GL_LineGroupDTO vm)
        {
            if (lineGroupRepository.GetMany(c => c.LineName == vm.LineName).Count() > 0 && vm.LineID == null)
            {
                return string.Format("This LineName [{0}] is already exist!", vm.LineName);
            }

            if (lineGroupRepository.GetMany(c => c.LineName == vm.LineName && c.LineID == vm.LineID).Count() > 0)
            {
                return string.Format("This LineID [{0}] is already exist!", vm.LineName);
            }

            var now = DateTime.Now;
            var groupLineEntity = lineGroupRepository.GetFirstOrDefault(c => c.LineGroup_UID == vm.LineGroup_UID);
            if (groupLineEntity != null)
            { 
                groupLineEntity.LineGroup_UID = vm.LineGroup_UID;
                if (vm.LineID != null)
                    groupLineEntity.LineID = vm.LineID;
                groupLineEntity.LineName = vm.LineName;
                //groupLineEntity.Plant_Organization_UID = vm.Plant_Organization_UID;
                //groupLineEntity.BG_Organization_UID = vm.BG_Organization_UID;
                //groupLineEntity.FunPlant_Organization_UID = vm.FunPlant_Organization_UID;
                //groupLineEntity.LineID = vm.LineID;
                //groupLineEntity.LineParent_ID = vm.LineParent_ID;
                //groupLineEntity.LineName = vm.LineName;
                groupLineEntity.Modified_UID = vm._Modified_UID;
                groupLineEntity.Modified_Date = now;
            }
            lineGroupRepository.Update(groupLineEntity);
            unitOfWork.Commit();

            return "SUCCESS";
        }

        public string RemoveGroupLine(int uid)
        {
            if (lineGroupRepository.GetById(uid) == null)
            {
                return "Record aleady deleted";
            }

            if (lineGroupRepository.GetMany(c => c.LineParent_ID == uid).Count() > 0)
            {
                return "Please delete subline first.";
            }

            lineGroupRepository.Delete(c => c.LineGroup_UID == uid);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string AddSubToGroup(GL_LineGroupDTO vm)
        {
            if (lineGroupRepository.GetMany(c => c.LineID == vm.LineID && c.LineParent_ID == vm.LineParent_ID).Count() > 0)
            {
                return string.Format("This LineName [{0}] is already exist!", vm.LineName);
            }

            var now = DateTime.Now;
            var groupLineEntity = new GL_LineGroup();
            groupLineEntity.Plant_Organization_UID = vm.Plant_Organization_UID;
            groupLineEntity.BG_Organization_UID = vm.BG_Organization_UID;
            groupLineEntity.FunPlant_Organization_UID = vm.FunPlant_Organization_UID;
            groupLineEntity.LineParent_ID = vm.LineParent_ID;
            groupLineEntity.LineID = vm.LineID;
            groupLineEntity.LineName = vm.LineName;
            groupLineEntity.CustomerID = vm.CustomerID;
            groupLineEntity.Modified_UID = vm._Modified_UID;
            groupLineEntity.Modified_Date = now;
            lineGroupRepository.Add(groupLineEntity);
            unitOfWork.Commit();

            return "SUCCESS";
        }

        public List<GL_LineGroupDTO> GetGroupLine(int? oporgid, int? Optypes, int? opFunPlant,int? customerId)
        {
            var groupLines = lineGroupRepository.GetGroupLine(oporgid, Optypes, opFunPlant, customerId);

            return groupLines;
        }

        public List<SubLineItem> GetSubLine(int Oporgid, int Optype)
        {
            var subLines = lineGroupRepository.GetSubLine(Oporgid, Optype);

            return subLines;
        }
    }
    #endregion LineGroup

    #region IGL_Service
    public interface IGL_Service
    {
        string Add_GL_ActionTasker(GL_ActionTaskDTO dto);

        string Update_GL_ActionTasker(GL_ActionTaskDTO item);
        PagedListModel<GL_ActionTaskDTO> QueryActionTaskerInfo(GL_ActionTaskDTO serchModel, Page page);

        GL_ActionTaskDTO Get_GL_ActionTaskerById(int ActionTasker_ID);
        string Delete_GL_ActionTaskerById(int ActionTasker_ID);

        PagedListModel<GL_MetricInfoDTO> QueryMetricInfo(GL_MetricInfoDTO serchModel, Page page);

        string AddMetricInfoInfo(GL_MetricInfoDTO serchModel);
        string DeleteMetricInfo(int metricInfo_Uid);
        string UpdateMetricInfo(GL_MetricInfoDTO updateModel);
        GL_MetricInfoDTO GetMetricInfoById(int uid);

        string AddOrEditMetricInfo(GL_MetricInfoDTO dto, bool isEdit);
        List<GL_MetricInfoDTO> GetMetricName(int plantUid, int bgUid, int funplantUid);

        PagedListModel<GL_MeetingTypeInfoDTO> QueryGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO serchModel, Page page);

        string UpdateGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO updateModel);
        string AddGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO serchModel);

        string DeleteGL_MeetingTypeInfo(int meetingTypeInfo_UID);

        GL_MeetingTypeInfoDTO GetGL_MeetingTypeInfoById(int uid);

        List<GL_MeetingTypeInfoDTO> GetMeetingTypeName(int plantUid, int bgUid, int funplantUid);
        List<GLFourQDTModel> GetDownTimeRecord(GLFourQParamModel paramModel);
        List<GLFourQDTModel> GetFourQDTTypeDetail(GLFourQParamModel paramModel);
        List<GLFourQDTModel> GetPaynterChartDetial(GLFourQParamModel paramModel);
        PagedListModel<GL_ActionTaskDTO> QueryActionInfoByCreateDate(GLFourQParamModel paramModel, Page page);
    }

    public class GL_Service : IGL_Service
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGL_ActionTaskRepository actionTaskRepository;
        private readonly IGL_MeetingTypeInfoRepository GL_MeetingTypeInfoRepository;
        private readonly IGL_MetricInfoRepository GL_MetricInfoRepository;
        private readonly IGL_DownTimeRecordRepository GL_DownTimeRecordRepository;

        public GL_Service(
            IUnitOfWork unitOfWork,
            IGL_ActionTaskRepository actionTaskRepository,
            IGL_MeetingTypeInfoRepository GL_MeetingTypeInfoRepository,
            IGL_MetricInfoRepository GL_MetricInfoRepository,
            IGL_DownTimeRecordRepository GL_DownTimeRecordRepository
            )
        {
            this._unitOfWork = unitOfWork;
            this.actionTaskRepository = actionTaskRepository;
            this.GL_MeetingTypeInfoRepository = GL_MeetingTypeInfoRepository;
            this.GL_MetricInfoRepository = GL_MetricInfoRepository;
            this.GL_DownTimeRecordRepository = GL_DownTimeRecordRepository;
        }

        #region  FourQ Report
        /// <summary>
        /// 根据报表类型获取DT的时间
        /// </summary>
        /// <param name="paramModel"></param>
        /// <returns></returns>
        public List<GLFourQDTModel> GetDownTimeRecord(GLFourQParamModel paramModel)
        {
            var result = GL_DownTimeRecordRepository.GetDownTimeRecord(paramModel);
            var resultList = new List<GLFourQDTModel>();
            int totalTime = 0;
            foreach (var item in result)
            {
                totalTime += item.DTTime;
            }
            foreach (var item in result)
            {
                GLFourQDTModel model = new GLFourQDTModel();
                model.DTTime = item.DTTime;
                if (totalTime != 0)
                    model.DTTime_p = (item.DTTime / totalTime) * 100;
                else
                    model.DTTime_p = 0;
                if (paramModel.ReportType == "Month")
                    model.DTName = item.DTName + "M";
                else if (paramModel.ReportType == "Week")
                    model.DTName = item.DTName + "W";
                else
                    model.DTName = item.DTName;
                resultList.Add(model);
            }
            return resultList;
        }

        /// <summary>
        /// 抓取該周期最後一筆資料(依部門分類)
        /// </summary>
        /// <param name="paramModel"></param>
        /// <returns></returns>
        public List<GLFourQDTModel> GetFourQDTTypeDetail(GLFourQParamModel paramModel)
        {
            var result = GL_DownTimeRecordRepository.GetFourQDTTypeDetail(paramModel);
            var resultList = new List<GLFourQDTModel>();
            int totalTime = 0;
            foreach (var item in result)
            {
                totalTime += item.DTTime;
            }
            foreach (var item in result)
            {
                GLFourQDTModel model = new GLFourQDTModel();
                model.DTTime = item.DTTime;
                if (totalTime != 0)
                    model.DTTime_p = (item.DTTime / totalTime) * 100;
                else
                    model.DTTime_p = 0;
                model.DTName = item.DTName;
                resultList.Add(model);
            }
            return resultList;
        }

        public List<GLFourQDTModel> GetPaynterChartDetial(GLFourQParamModel paramModel)
        {
            var result = GL_DownTimeRecordRepository.GetPaynterChartDetial(paramModel);
            foreach (var Item in result)
            {
                var total = result.Where(x => x.DtMon == Item.DtMon).Sum(x => x.DTTime);
                if (total != 0)
                    Item.DTTime_p = Math.Round((double)((double)Item.DTTime / (double)total) * 100, 2);
                else
                    Item.DTTime_p = 0;
                if (paramModel.ReportType == "Month")
                    Item.DtMon = Item.DtMon + "M";
                else if (paramModel.ReportType == "Week")
                    Item.DtMon = Item.DtMon + "W";
                else
                    Item.DtMon = Item.DtMon;
            }
            return result;
        }

        public PagedListModel<GL_ActionTaskDTO> QueryActionInfoByCreateDate(GLFourQParamModel paramModel, Page page)
        {
            var dtResult = actionTaskRepository.QueryActionInfoByCreateDate(paramModel, page);
            return dtResult;
        }
        #endregion

        #region ActionTask
        public string Add_GL_ActionTasker(GL_ActionTaskDTO dto)
        {
            string errorMessage = string.Empty;
            try
            {
                var result = actionTaskRepository.Add_GL_ActionTasker(dto);
                return result;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return ex.Message;
            }
        }

        public string Update_GL_ActionTasker(GL_ActionTaskDTO dto)
        {
            string errorMessage = string.Empty;
            try
            {
                var result = actionTaskRepository.Update_GL_ActionTasker(dto);
                return result;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return ex.Message;
            }
        }

        public PagedListModel<GL_ActionTaskDTO> QueryActionTaskerInfo(GL_ActionTaskDTO serchModel, Page page)
        {
            var result = actionTaskRepository.QueryActionTaskerInfo(serchModel, page);
            return result;
        }

        public GL_ActionTaskDTO Get_GL_ActionTaskerById(int ActionTasker_ID)
        {
            var result = actionTaskRepository.Get_GL_ActionTaskerById(ActionTasker_ID);
            return result;
        }

        public string Delete_GL_ActionTaskerById(int ActionTasker_ID)
        {
            var result = actionTaskRepository.Delete_GL_ActionTaskerById(ActionTasker_ID);
            return result;
        }

        #endregion

        #region meetType
        public PagedListModel<GL_MeetingTypeInfoDTO> QueryGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO serchModel, Page page)
        {
            var result = GL_MeetingTypeInfoRepository.QueryGL_MeetingTypeInfo(serchModel, page);
            return result;
        }

        public string AddGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO serchModel)
        {
            //判断是否重复
            var isExist = GL_MeetingTypeInfoRepository.GetMany(
                   p => p.Plant_Organization_UID == serchModel.Plant_Organization_UID
                   && p.BG_Organization_UID == serchModel.BG_Organization_UID
                   && p.FunPlant_Organization_UID == serchModel.FunPlant_Organization_UID
                   && p.MeetingType_ID == serchModel.MeetingType_ID
                   && p.MeetingType_Name == serchModel.MeetingType_Name
               );

            if (isExist.Count() > 0)
            {
                return "添加的数据已经存在，请检查！";
            }
            else
            {
                var result = GL_MeetingTypeInfoRepository.AddGL_MeetingTypeInfo(serchModel);
                return result;
            }
        }

        public string DeleteGL_MeetingTypeInfo(int meetingTypeInfo_UID)
        {
            var result = GL_MeetingTypeInfoRepository.DeleteMeetTypeInfo(meetingTypeInfo_UID);
            return result;
        }

        public string UpdateGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO updateModel)
        {
            try
            {
                var entityContext = GL_MeetingTypeInfoRepository.GetById(updateModel.MeetingType_UID);
                entityContext.MeetingType_ID = updateModel.MeetingType_ID;
                entityContext.MeetingType_Name = updateModel.MeetingType_Name;
                GL_MeetingTypeInfoRepository.Update(entityContext);
                _unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return "修改失败，错误信息" + ex.Message.ToString();
            }
        }
        public GL_MeetingTypeInfoDTO GetGL_MeetingTypeInfoById(int uid)
        {
            var entityContext = GL_MeetingTypeInfoRepository.GetGL_MeetingTypeInfo(uid);
            return entityContext;
        }

        public List<GL_MeetingTypeInfoDTO> GetMeetingTypeName(int plantUid, int bgUid, int funplantUid)
        {
            var entityContext = GL_MeetingTypeInfoRepository.GetMeetingTypeName(plantUid, bgUid, funplantUid);
            return entityContext;
        }
        #endregion

        #region Metrices
        public PagedListModel<GL_MetricInfoDTO> QueryMetricInfo(GL_MetricInfoDTO serchModel, Page page)
        {
            var result = GL_MetricInfoRepository.QueryMetricInfoInfo(serchModel, page);
            return result;
        }

        public string AddMetricInfoInfo(GL_MetricInfoDTO serchModel)
        {
            var result = GL_MetricInfoRepository.AddMetricInfoInfo(serchModel);
            return result;
        }

        public string DeleteMetricInfo(int metricInfo_Uid)
        {
            var result = GL_MetricInfoRepository.DeleteMetricInfo(metricInfo_Uid);
            return result;
        }

        public string UpdateMetricInfo(GL_MetricInfoDTO updateModel)
        {
            try
            {
                var entityContext = GL_MetricInfoRepository.GetById(updateModel.Metric_UID);
                entityContext.Metric_ID = updateModel.Metric_ID;
                entityContext.Metric_Name = updateModel.Metric_Name;
                GL_MetricInfoRepository.Update(entityContext);
                _unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return "修改失败，错误信息" + ex.Message.ToString();
            }
        }

        public GL_MetricInfoDTO GetMetricInfoById(int uid)
        {
            var entityContext = GL_MetricInfoRepository.GetMetricInfoInfoById(uid);
            return entityContext;
        }

        public string AddOrEditMetricInfo(GL_MetricInfoDTO dto, bool isEdit)
        {
            string errorMessage = string.Empty;
            try
            {
                GL_MetricInfo entityContext;
                if (dto.Metric_UID == 0)
                {
                    //判断是否重复
                    var result = GL_MetricInfoRepository.GetMany(p => p.Plant_Organization_UID == dto.Plant_Organization_UID && p.BG_Organization_UID == dto.BG_Organization_UID && p.Metric_ID == dto.Metric_ID && p.Metric_Name == dto.Metric_Name);
                    if (result.Count() > 0)
                    {
                        return "填加的数据已经存在，请检查！";
                    }
                    else
                    {
                        entityContext = new GL_MetricInfo();
                        entityContext.Plant_Organization_UID = dto.Plant_Organization_UID;
                        entityContext.BG_Organization_UID = dto.BG_Organization_UID;
                        entityContext.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                        entityContext.Metric_ID = dto.Metric_ID;
                        entityContext.Metric_Name = dto.Metric_Name;
                        entityContext.Modified_UID = dto.Modified_UID;
                        entityContext.Modified_Date = dto.Modified_Date;
                        GL_MetricInfoRepository.Add(entityContext);
                        _unitOfWork.Commit();
                    }
                }
                else
                {
                    entityContext = GL_MetricInfoRepository.GetById(dto.Metric_UID);
                    entityContext.Metric_ID = dto.Metric_ID;
                    entityContext.Metric_Name = dto.Metric_Name;
                    GL_MetricInfoRepository.Update(entityContext);
                    _unitOfWork.Commit();
                }

                return "0";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return ex.Message;
            }


        }

        public List<GL_MetricInfoDTO> GetMetricName(int plantUid, int bgUid, int funplantUid)
        {
            var entityContext = GL_MetricInfoRepository.GetMetricName(plantUid, bgUid, funplantUid);
            return entityContext;
        }

        #endregion
    }
    #endregion

    public class GL_LineService : BaseSevice<GL_Line, GL_LineDTO, GL_LineModelSearch>, IGL_LineService
    {
        private readonly IGL_LineRepository lineRepository;
        public GL_LineService(IGL_LineRepository lineRepository) : base(lineRepository)
        {
            this.lineRepository = lineRepository;
        }
    }

}
