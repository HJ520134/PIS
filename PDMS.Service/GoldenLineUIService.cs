using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public enum GoldenLine_BuildPlanUpdateType
    {
        None,
        PlanOutput,
        PlanHC,
        ActualHC
    }
    public interface IGoldenLineUIService
    {
        // Shift Time
        IList<SystemProjectDTO> GetCustomers(int BG_Organization_UID);
        IList<GL_ShiftTimeDTO> GetShiftTimes(int Plant_Organization_UID, int? FunPlant_Organization_UID, int BG_Organization_UID);
        IList<GL_ShiftTimeDTO> GetAllShiftTimes(); 
        IList<GL_RestTimeDTO> GetAllRestTimesAPI();
        PagedListModel<GL_ShiftTimeDTO> GetShiftTimesPaged(GoldenLineNormalQueryViewModel vm, Page page);
        GL_ShiftTimeDTO GetShiftTimeByID(int ShiftTimeID);
        GL_RestTimeDTO GetRestTimeByID(int ShiftTimeID);
        PagedListModel<GL_RestTimeDTO> GetRestTimeList(RestTimeQueryViewModel query, Page page);
        string AddOrUpdateShiftTime(GL_ShiftTimeDTO dto, out string errorMessage);
        bool RemoveShiftTimeByID(int ShiftTimeID);

        IList<GL_LineDTO> GetLines(int Plant_Organization_UID, int? FunPlant_Organization_UID, int BG_Organization_UID,bool? IsEnabled);
        PagedListModel<GL_LineDTO> GetLinePaged(GoldenLineNormalQueryViewModel queryVM, Page page);
        GL_LineDTO GetLineByID(int LineID);
        GL_LineDTO AddOrUpdateLine(GL_LineDTO dto, out string errorMessage);
        bool RemoveLineByID(int LineID);

        PagedListModel<GL_StationDTO> GetStationPaged(GoldenLineNormalQueryViewModel queryVM, Page page);
        GL_StationDTO GetStationByID(int StationID);
        GL_StationDTO AddOrUpdateStation(GL_StationDTO dto, out string errorMessage);
        bool RemoveStationByID(int StationID);
        bool RemoveStationByLineID(int LineID);
        bool RemoveRestTimeByID(int RestUID);
        PagedListModel<GL_BuildPlanDTO> GetBuildPlanPaged(GoldenLineNormalQueryViewModel queryVM, Page page);
        GL_BuildPlanDTO AddOrUpdateBuildPlan(GL_BuildPlanDTO dto, GoldenLine_BuildPlanUpdateType updateType, out string errorMessage);
        string AddOrUpdateGLStations(GL_StationDTO dto, bool isEdit);
        string AddOrUpdateRestTime(GL_RestTimeDTO dto, out string errorMessage);
        string UpdateRestTime(GL_RestTimeDTO dto, out string errorMessage);
    }

    public class GoldenLineUIService : IGoldenLineUIService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISystemProjectRepository _projectRepository;
        private readonly ISystemFunctionPlantRepository _funPlantRepository;

        private readonly IGL_ShiftTimeRepository _shiftTimeRepository;
        private readonly IGL_RestRepository GL_RestRepository;
        private readonly IGL_LineRepository _lineRepository;
        private readonly IGL_StationRepository _stationRepository;
        private readonly IGL_BuildPlanRepository _buildPlanRepository;
        private readonly IGL_LineShiftResposibleUserRepository _lineShiftResposibleUserRepository;

        public GoldenLineUIService(
            IUnitOfWork unitOfWork,
            ISystemProjectRepository projectRepository,
            ISystemFunctionPlantRepository functionPlantRepository,
            IGL_ShiftTimeRepository shitTimeRepository,
            IGL_LineRepository lineRepository,
            IGL_StationRepository stationRepository,
            IGL_BuildPlanRepository buildPlanRepository,
            IGL_RestRepository gL_RestRepository,
            IGL_LineShiftResposibleUserRepository lineShiftResposibleUserRepository)
        {
            _unitOfWork = unitOfWork;
            _projectRepository = projectRepository;
            _funPlantRepository = functionPlantRepository;
            _shiftTimeRepository = shitTimeRepository;
            _lineRepository = lineRepository;
            GL_RestRepository = gL_RestRepository;
            _stationRepository = stationRepository;
            _buildPlanRepository = buildPlanRepository;
            _lineShiftResposibleUserRepository = lineShiftResposibleUserRepository;
        }
        // Get Customers from BUD
        public PagedListModel<GL_RestTimeDTO> GetRestTimeList(RestTimeQueryViewModel query, Page page)
        {
            int totalCount = 0;
            var PPlist = GL_RestRepository.GetRestTimeList(query, out totalCount);
            return new PagedListModel<GL_RestTimeDTO>(0, PPlist);
        }


        public IList<SystemProjectDTO> GetCustomers(int BG_Organization_UID)
        {
            var result = _projectRepository.GetMany(x =>x.Organization_UID == BG_Organization_UID);
            // entity mapper
            var dtos = AutoMapper.Mapper.Map<IList<SystemProjectDTO>>(result);
            return dtos;
        }
        // Get Projects from system
        public List<string> GetAllProjectsByOrgID(int orgID)
        {
            var result = _projectRepository.GetMany(x => x.Organization_UID == orgID).Select(x => x.Project_Name).Distinct();
            return result.ToList();
        }
        // Get Shift Time by User Organization
        public IList<GL_ShiftTimeDTO> GetShiftTimes(int Plant_Organization_UID, int? FunPlant_Organization_UID, int BG_Organization_UID)
        {
            IQueryable<GL_ShiftTime> result = null;
            if (Plant_Organization_UID == 0)
            {
                result = _shiftTimeRepository.GetAll();
               // result = _shiftTimeRepository.GetMany(x => x.IsEnabled == true);
            }
                
            else
            {

                if (BG_Organization_UID != 0)
                {
                    result = _shiftTimeRepository.GetMany(
                                          x =>
                                          x.Plant_Organization_UID == Plant_Organization_UID &&
                                          x.BG_Organization_UID == BG_Organization_UID );

                    //result = _shiftTimeRepository.GetMany(
                    //  x =>
                    //  x.Plant_Organization_UID == Plant_Organization_UID &&
                    //  x.FunPlant_Organization_UID == FunPlant_Organization_UID &&
                    //  x.BG_Organization_UID == BG_Organization_UID &&
                    //  x.IsEnabled == true);
                }
                else
                {
                    result = _shiftTimeRepository.GetMany(
                            x =>  x.Plant_Organization_UID == Plant_Organization_UID );
                }


            }

            // entity mapper
            var dtos = AutoMapper.Mapper.Map<IList<GL_ShiftTimeDTO>>(result);
            return dtos;
        }

        public IList<GL_ShiftTimeDTO> GetAllShiftTimes()
        {
            IQueryable<GL_ShiftTime> result = _shiftTimeRepository.GetAll();
            var dtos = AutoMapper.Mapper.Map<IList<GL_ShiftTimeDTO>>(result);
            return dtos;
        }

        public IList<GL_RestTimeDTO> GetAllRestTimesAPI()
        {
            IQueryable<GL_Rest> result = GL_RestRepository.GetAll();
            var dtos = AutoMapper.Mapper.Map<IList<GL_RestTimeDTO>>(result);
            return dtos;
        }
        // Get Shift Time by Id
        public GL_ShiftTimeDTO GetShiftTimeByID(int ShiftTimeID)
        {
            IQueryable<GL_ShiftTime> result = _shiftTimeRepository.GetMany(
                x => x.ShiftTimeID == ShiftTimeID);
            if (result.Count() > 0)
            {
                var dto = AutoMapper.Mapper.Map<GL_ShiftTimeDTO>(result.First());
                return dto;
            }
            else
                return null;
        }
        // Remove Shift Time by Id
        public bool RemoveShiftTimeByID(int ShiftTimeID)
        {


            try
            {
                IQueryable<GL_ShiftTime> result = _shiftTimeRepository.GetMany(
                    x => x.ShiftTimeID == ShiftTimeID);
                if (result.Count() > 0)
                {
                    //GL_ShiftTime entity = result.First();
                    //entity.IsEnabled = false;
                    //_shiftTimeRepository.Update(entity);
                    GL_ShiftTime entity = result.First();
                    _shiftTimeRepository.Delete(entity);
                    _unitOfWork.Commit();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public GL_RestTimeDTO GetRestTimeByID(int ShiftTimeID)
        {
            return  GL_RestRepository.GetRestById(ShiftTimeID);

        }

        public bool RemoveRestTimeByID(int RestUID)
        {
            var item = GL_RestRepository.GetById(RestUID);
            try
            {
                GL_RestRepository.Delete(item);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        // Get Paged Shift Time
        public PagedListModel<GL_ShiftTimeDTO> GetShiftTimesPaged(GoldenLineNormalQueryViewModel queryVM, Page page)
        {
            var dtos = GetShiftTimes(queryVM.Plant_Organization_UID, queryVM.FunPlant_Organization_UID, queryVM.BG_Organization_UID);
            return new PagedListModel<GL_ShiftTimeDTO>(dtos.Count, dtos.Skip(page.PageNumber * page.PageSize).Take(page.PageSize));
        }
        // Add or update shift time entity
        public string AddOrUpdateShiftTime(GL_ShiftTimeDTO dto, out string errorMessage)
        {
            errorMessage = string.Empty;
            GL_ShiftTime entity = AutoMapper.Mapper.Map<GL_ShiftTime>(dto);
            try
            {
                // get exists shift time
                IList<GL_ShiftTimeDTO> existsDtos = GetShiftTimes(dto.Plant_Organization_UID, dto.FunPlant_Organization_UID, dto.BG_Organization_UID);
                TimeSpan ts1 = TimeSpan.Parse(dto.StartTime);
                TimeSpan ts2 = TimeSpan.Parse(dto.End_Time);

                if (ts1 > ts2)
                    ts2 = ts2.Add(new TimeSpan(1, 0, 0, 0));

                // check the overlap shift time
                bool isIntersection = false;
                GL_ShiftTimeDTO overlapShiftTime = null;
                foreach (GL_ShiftTimeDTO eDto in existsDtos)
                {
                    TimeSpan ts1a = TimeSpan.Parse(eDto.StartTime);
                    TimeSpan ts2a = TimeSpan.Parse(eDto.End_Time);

                    if (ts1a > ts2a)
                        ts2a = ts2a.Add(new TimeSpan(1, 0, 0, 0));

                    TimeSpan ts = getIntersectionTimeSpan(ts1a, ts2a, ts1, ts2);
                    if (ts.Ticks > 0)
                    {
                        overlapShiftTime = eDto;
                        isIntersection = true;
                        break;
                    }
                }
                if (isIntersection)
                    errorMessage = string.Format("Shift time overlap - {0}, {1}", overlapShiftTime.StartTime, overlapShiftTime.End_Time);

                GL_ShiftTime entityContext;
                if (dto.ShiftTimeID == 0)
                {
                    entityContext = _shiftTimeRepository.Add(entity);
                    _unitOfWork.Commit();
                }
                else
                {
                    entityContext = _shiftTimeRepository.GetById(entity.ShiftTimeID);
                    entityContext.Plant_Organization_UID = entity.Plant_Organization_UID;
                    entityContext.BG_Organization_UID = entity.BG_Organization_UID;
                    entityContext.FunPlant_Organization_UID = entity.FunPlant_Organization_UID;
                    entityContext.Shift = entity.Shift;
                    entityContext.StartTime = entity.StartTime;
                    entityContext.End_Time = entity.End_Time;
                    entityContext.Modified_UID = entity.Modified_UID;
                    entityContext.Modified_Date = entity.Modified_Date;
                    entityContext.Break_Time = entity.Break_Time;
                    entityContext.Sequence = entity.Sequence;
                    entityContext.IsEnabled= entity.IsEnabled;
                    _shiftTimeRepository.Update(entityContext);
                    _unitOfWork.Commit();
                }

              //  var returnDto = AutoMapper.Mapper.Map<GL_ShiftTimeDTO>(entityContext);
                return "";
            }
            catch (Exception ex)
            {
              //  errorMessage = ex.Message;
                return ex.Message;;
            }
        }

        public string AddOrUpdateRestTime(GL_RestTimeDTO dto, out string errorMessage)
        {
            errorMessage = string.Empty;
            GL_Rest entity = AutoMapper.Mapper.Map<GL_Rest>(dto);
            try
            {

                GL_Rest entityContext;
              
                    entityContext =GL_RestRepository.Add(entity);
                    _unitOfWork.Commit();
          
                
                return "";
            }
            catch (Exception ex)
            {
                //  errorMessage = ex.Message;
                return ex.Message; ;
            }
        }

        public string UpdateRestTime(GL_RestTimeDTO dto, out string errorMessage)
        {
            errorMessage = string.Empty;
            GL_Rest entity = AutoMapper.Mapper.Map<GL_Rest>(dto);
            try
            {

             GL_RestRepository.Update(entity);
                _unitOfWork.Commit();
                return "";
            }
            catch (Exception ex)
            {
                //  errorMessage = ex.Message;
                return ex.Message; ;
            }
        }

        // Get Line by User Organization
        public IList<GL_LineDTO> GetLines(int Plant_Organization_UID, int? FunPlant_Organization_UID, int BG_Organization_UID, bool? IsEnabled)
        {
            IQueryable<GL_Line> result = null;
            if (IsEnabled != null)
            {


                if (Plant_Organization_UID == 0)
                {
                    result = _lineRepository.GetMany(o => o.IsEnabled == IsEnabled).OrderByDescending(o => o.Modified_Date);
                }
                else
                {

                    if (BG_Organization_UID != 0)
                    {
                        if (FunPlant_Organization_UID != null && FunPlant_Organization_UID != 0)
                        {
                            result = _lineRepository.GetMany(
                                            x =>
                                            x.Plant_Organization_UID == Plant_Organization_UID &&
                                            x.FunPlant_Organization_UID == FunPlant_Organization_UID &&
                                            x.BG_Organization_UID == BG_Organization_UID
                                            && x.IsEnabled == IsEnabled
                                            ).OrderByDescending(o => o.Modified_Date);
                        }
                        else
                        {
                            result = _lineRepository.GetMany(
                    x =>
                    x.Plant_Organization_UID == Plant_Organization_UID &&
                    x.BG_Organization_UID == BG_Organization_UID
                    && x.IsEnabled == IsEnabled
                    ).OrderByDescending(o => o.Modified_Date);
                        }

                    }
                    else
                    {
                        result = _lineRepository.GetMany(x =>
                       x.Plant_Organization_UID == Plant_Organization_UID && x.IsEnabled == IsEnabled).OrderByDescending(o => o.Modified_Date);
                    }
                }
            }
            else
            {
                if (Plant_Organization_UID == 0)
                {
                    result = _lineRepository.GetAll();
                }
                else
                {

                    if (BG_Organization_UID != 0)
                    {
                        if (FunPlant_Organization_UID != null && FunPlant_Organization_UID != 0)
                        {
                            result = _lineRepository.GetMany(
                                            x =>
                                            x.Plant_Organization_UID == Plant_Organization_UID &&
                                            x.FunPlant_Organization_UID == FunPlant_Organization_UID &&
                                            x.BG_Organization_UID == BG_Organization_UID
                                            //&& x.IsEnabled == IsEnabled
                                            ).OrderByDescending(o => o.Modified_Date);
                        }
                        else
                        {
                            result = _lineRepository.GetMany(
                    x =>
                    x.Plant_Organization_UID == Plant_Organization_UID &&
                    x.BG_Organization_UID == BG_Organization_UID
                    //&& x.IsEnabled == IsEnabled
                    ).OrderByDescending(o => o.Modified_Date);
                        }

                    }
                    else
                    {
                        result = _lineRepository.GetMany(x =>
                       x.Plant_Organization_UID == Plant_Organization_UID
                       //&& x.IsEnabled == IsEnabled
                       ).OrderByDescending(o => o.Modified_Date);
                    }
                }
            }
            // entity mapper
            var dtos = new List<GL_LineDTO>();
            //var dtos = AutoMapper.Mapper.Map<IList<GL_LineDTO>>(result);
            foreach (var item in result)
            {
                //构造负责人数据
                var responsibleList = new List<GL_LineShiftResposibleUserDTO>();
                //过滤可用的班次
                var enabledResponsibleUser = item.GL_LineShiftResposibleUser.Where(x => x.GL_ShiftTime.IsEnabled);
                foreach (var responible in enabledResponsibleUser)
                {
                    var responsibleDto = new GL_LineShiftResposibleUserDTO()
                    {
                        ResponsibleUser = AutoMapper.Mapper.Map<SystemUserDTO>(responible.System_Users),
                        GL_Line = AutoMapper.Mapper.Map<GL_LineDTO>(responible.GL_Line),
                        GL_ShiftTime = AutoMapper.Mapper.Map<GL_ShiftTimeDTO>(responible.GL_ShiftTime)
                    };
                    responsibleList.Add(responsibleDto);
                }

                var dto = AutoMapper.Mapper.Map<GL_LineDTO>(item);
                dto.GL_LineShiftResposibleUserList = responsibleList;
                dtos.Add(dto);
            }
            return dtos;
        }
        // Get Paged Line
        public PagedListModel<GL_LineDTO> GetLinePaged(GoldenLineNormalQueryViewModel queryVM, Page page)
        {
            var dtos = GetLines(queryVM.Plant_Organization_UID, queryVM.FunPlant_Organization_UID, queryVM.BG_Organization_UID, queryVM.IsEnabled);
            return new PagedListModel<GL_LineDTO>(dtos.Count, dtos.Skip(page.PageNumber * page.PageSize).Take(page.PageSize));
        }
        // Get Line by Id
        public GL_LineDTO GetLineByID(int LineID)
        {
            var result = _lineRepository.GetFirstOrDefault(x => x.LineID == LineID);
            if (result != null)
            {
                var dto = AutoMapper.Mapper.Map<GL_LineDTO>(result);
                if (result.GL_LineShiftResposibleUser.Count > 0)
                {

                    var responsibleUserList = new List<GL_LineShiftResposibleUserDTO>(); //AutoMapper.Mapper.Map<List<GL_LineShiftResposibleUserDTO>>(result.GL_LineShiftResposibleUser);
                    //过滤可用的班次
                    var enabledResponsibleUsers = result.GL_LineShiftResposibleUser.Where(x => x.GL_ShiftTime.IsEnabled);
                    foreach (var item in enabledResponsibleUsers)
                    {
                        var responsibleUser = AutoMapper.Mapper.Map<GL_LineShiftResposibleUserDTO>(item);
                        responsibleUser.ResponsibleUser = AutoMapper.Mapper.Map<SystemUserDTO>(item.System_Users);
                        responsibleUserList.Add(responsibleUser);
                    }
                    dto.GL_LineShiftResposibleUserList = responsibleUserList;
                }
                return dto;
            }
            else
                return null;
        }
        // Remove Shift Time by Id
        public bool RemoveLineByID(int LineID)
        {


            IQueryable<GL_Line> result = _lineRepository.GetMany(
                x => x.LineID == LineID);
            if (result.Count() > 0)
            {
                //GL_Line entity = result.First();
                //entity.IsEnabled = false;
                //_lineRepository.Update(entity);
                //_unitOfWork.Commit();
                //return true;
                GL_Line entity = result.First();
                _lineRepository.Delete(entity);
                _unitOfWork.Commit();
                return true;
            }
            else
                return false;
        }
        // Add or update line entity
        public GL_LineDTO AddOrUpdateLine(GL_LineDTO dto, out string errorMessage)
        {
            errorMessage = string.Empty;
            GL_Line entity = AutoMapper.Mapper.Map<GL_Line>(dto);
            try
            {
                GL_Line entityContext;
                if (dto.LineID == 0)
                {
                    //新增
                    //新增Line
                    entityContext = _lineRepository.Add(entity);
                    //新增负责人
                    if (dto.GL_LineShiftResposibleUserList != null)
                    {
                        //User_UID 不为0才是有效的责任人
                        var responsibleUserDtoList = dto.GL_LineShiftResposibleUserList.Where(x => x.User_UID != 0).ToList();
                        if (responsibleUserDtoList.Count>0)
                        {
                            var responsibleUserList = AutoMapper.Mapper.Map<List<GL_LineShiftResposibleUser>>(responsibleUserDtoList);
                            foreach (var item in responsibleUserList)
                            {
                                item.LineID = entityContext.LineID;
                            }
                            _lineShiftResposibleUserRepository.AddList(responsibleUserList);
                        }
                        
                    }

                    _unitOfWork.Commit();
                }
                else
                {
                    //编辑
                    entityContext = _lineRepository.GetById(entity.LineID);
                    entityContext.CustomerID = entity.CustomerID;
                    entityContext.CycleTime = entity.CycleTime;
                    //entityContext.ProjectName = entity.ProjectName;
                    //entityContext.MESProjectName = entity.MESProjectName ?? "";
                    entityContext.Plant_Organization_UID = entity.Plant_Organization_UID;
                    entityContext.BG_Organization_UID = entity.BG_Organization_UID;
                    entityContext.FunPlant_Organization_UID = entity.FunPlant_Organization_UID;
                    entityContext.Phase = entity.Phase;
                    entityContext.LineName = entity.LineName;
                    entityContext.MESLineName = entity.MESLineName ?? "";
                    entityContext.Modified_UID = entity.Modified_UID;
                    entityContext.Modified_Date = entity.Modified_Date;
                    entityContext.Seq = entity.Seq;
                    entityContext.IsEnabled = entity.IsEnabled;
                    _lineRepository.Update(entityContext);
                    
                    //更新负责人
                    var responsibleUserList_new = new List<GL_LineShiftResposibleUser>();
                    if (dto.GL_LineShiftResposibleUserList != null)
                    {
                        responsibleUserList_new = AutoMapper.Mapper.Map<List<GL_LineShiftResposibleUser>>(dto.GL_LineShiftResposibleUserList);
                        foreach (var item in responsibleUserList_new)
                        {
                            item.LineID = entityContext.LineID;
                            //从数据库找到对应的数据
                            var responsibleOld = _lineShiftResposibleUserRepository.GetFirstOrDefault(x => x.LineID == item.LineID && x.ShiftTimeID == item.ShiftTimeID);
                            if (responsibleOld == null)
                            {
                                if (item.User_UID != 0)
                                {
                                    //没有的，新增
                                    var responsibleNew = AutoMapper.Mapper.Map<GL_LineShiftResposibleUser>(item);
                                    _lineShiftResposibleUserRepository.Add(responsibleNew);
                                }
                            }
                            else
                            {
                                //删除
                                if (item.User_UID == 0)
                                {
                                    _lineShiftResposibleUserRepository.Delete(responsibleOld);
                                }
                                else
                                {
                                    if (item.User_UID != responsibleOld.User_UID)
                                    {
                                        responsibleOld.User_UID = item.User_UID;
                                        _lineShiftResposibleUserRepository.Update(responsibleOld);
                                    }
                                }
                            }
                        }
                    }
                    //var responsibleUserList_old = entityContext.GL_LineShiftResposibleUser.ToList();
                    ////删除负责人
                    //responsibleUserList_old.Where(x=>)

                    _unitOfWork.Commit();
                }

                var returnDto = AutoMapper.Mapper.Map<GL_LineDTO>(entityContext);
                return returnDto;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }
        // Get Station by User Organization
        public IList<GL_StationDTO> GetStation(int LineID)
        {
            IQueryable<GL_Station> result = _stationRepository.GetMany(
                x =>
                x.LineID == LineID 
                //&&
                //x.IsEnabled == true
                );
            // entity mapper
            var dtos = AutoMapper.Mapper.Map<IList<GL_StationDTO>>(result);
            return dtos;
        }
        // Get Station by Id
        public GL_StationDTO GetStationByID(int StationID)
        {
            IQueryable<GL_Station> result = _stationRepository.GetMany(
                x => x.StationID == StationID);
            if (result.Count() > 0)
            {
                var dto = AutoMapper.Mapper.Map<GL_StationDTO>(result.First());
                return dto;
            }
            else
                return null;
        }
        // Get Paged Station
        public PagedListModel<GL_StationDTO> GetStationPaged(GoldenLineNormalQueryViewModel queryVM, Page page)
        {
            var dtos = GetStation(queryVM.LineID);
            return new PagedListModel<GL_StationDTO>(dtos.Count, dtos.Skip(page.PageNumber * page.PageSize).Take(page.PageSize));
        }
        // Remove Shift Time by Id
        public bool RemoveStationByID(int StationID)
        {


            IQueryable<GL_Station> result = _stationRepository.GetMany(
                x => x.StationID == StationID);
            if (result.Count() > 0)
            {
                //GL_Station entity = result.First();
                //entity.IsEnabled = false;
                //_stationRepository.Update(entity);
                //_unitOfWork.Commit();
                //return true;
                GL_Station entity = result.First();
                _stationRepository.Delete(entity);
                _unitOfWork.Commit();
                return true;
            }
            else
                return false;
        }
        public bool RemoveStationByLineID(int LineID)
        {
            IQueryable<GL_Station> result = _stationRepository.GetMany(x => x.LineID == LineID);
            foreach (var r in result)
            {
                r.IsEnabled = false;
            }
            _unitOfWork.Commit();
            return true;
        }
        // Add or update station entity
        public GL_StationDTO AddOrUpdateStation(GL_StationDTO dto, out string errorMessage)
        {
            errorMessage = string.Empty;
            GL_Station entity = AutoMapper.Mapper.Map<GL_Station>(dto);
            if (entity.IsBirth == true || entity.IsOutput == true)
            {
                var entities = _stationRepository.GetMany(x => x.LineID == entity.LineID);
                if (entity.IsBirth == true)
                {
                    foreach (var e in entities)
                        e.IsBirth = false;
                }
                if (entity.IsOutput == true)
                {
                    foreach (var e in entities)
                        e.IsOutput = false;
                }
            }
            try
            {
                GL_Station entityContext;
                if (dto.StationID == 0)
                {
                    entity.MESStationName = entity.MESStationName ?? "";
                    entityContext = _stationRepository.Add(entity);
                    _unitOfWork.Commit();
                }
                else
                {
                    entityContext = _stationRepository.GetById(entity.StationID);
                    entityContext.Seq = entity.Seq;
                    entityContext.StationName = entity.StationName;
                    entityContext.MESStationName = entity.MESStationName ?? "";
                    entityContext.CycleTime = entity.CycleTime;
                    entityContext.IsBirth = entity.IsBirth;
                    entityContext.IsOutput = entity.IsOutput;
                    entityContext.IsTest = entity.IsTest;
                    entityContext.Modified_UID = entity.Modified_UID;
                    entityContext.Modified_Date = entity.Modified_Date;
                    entityContext.IsEnabled = entity.IsEnabled;
                    entityContext.DashboardTarget = entity.DashboardTarget;
                    _stationRepository.Update(entityContext);
                    _unitOfWork.Commit();
                }

                var returnDto = AutoMapper.Mapper.Map<GL_StationDTO>(entityContext);
                return returnDto;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }
        // Get build plan by User Organization
        public IList<GL_BuildPlanDTO> GetBuildPlan(int LineID)
        {
            IQueryable<GL_BuildPlan> result = _buildPlanRepository.GetMany(
                x => x.LineID == LineID);
            // entity mapper
            var dtos = AutoMapper.Mapper.Map<IList<GL_BuildPlanDTO>>(result);
            return dtos;
        }
        // Get Paged build plan
        public PagedListModel<GL_BuildPlanDTO> GetBuildPlanPaged(GoldenLineNormalQueryViewModel queryVM, Page page)
        {
            var dtos = GetBuildPlan(queryVM.LineID);
            return new PagedListModel<GL_BuildPlanDTO>(dtos.Count, dtos);
        }
        // Add or update build plan entity
        public GL_BuildPlanDTO AddOrUpdateBuildPlan(GL_BuildPlanDTO dto, GoldenLine_BuildPlanUpdateType updateType, out string errorMessage)
        {
            errorMessage = string.Empty;
            GL_BuildPlan entity = AutoMapper.Mapper.Map<GL_BuildPlan>(dto);
            // Query
            GL_BuildPlan foundBuildPlan = _buildPlanRepository.GetMany(x => x.LineID == entity.LineID && x.StartTime == entity.StartTime).FirstOrDefault();
            try
            {
                GL_BuildPlan entityContext;
                if (foundBuildPlan == null)
                    entityContext = _buildPlanRepository.Add(entity);
                else
                {
                    entityContext = foundBuildPlan;
                    switch (updateType)
                    {
                        case GoldenLine_BuildPlanUpdateType.PlanOutput:
                            entityContext.PlanOutput = entity.PlanOutput;
                            break;
                        case GoldenLine_BuildPlanUpdateType.PlanHC:
                            entityContext.PlanHC = entity.PlanHC;
                            break;
                        case GoldenLine_BuildPlanUpdateType.ActualHC:
                            entityContext.ActualHC = entity.ActualHC;
                            break;
                    }
                }
                var returnDto = AutoMapper.Mapper.Map<GL_BuildPlanDTO>(entityContext);
                return returnDto;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }
        // Get Function Plant Organization by uid
        public int GetFunPlantOrganizationUID(string plantName, string opTypeName)
        {
            List<System_Function_Plant> funcPlants = _funPlantRepository.GetAll().ToList();
            System_Function_Plant entity = funcPlants.Where(x => x.FunPlant == plantName && x.OP_Types == opTypeName).FirstOrDefault();
            if (entity == null)
                return 0;
            else
                return entity.FunPlant_OrganizationUID ?? 0;
        }
        // Get intersection time span
        private TimeSpan getIntersectionTimeSpan(TimeSpan s1, TimeSpan e1, TimeSpan s2, TimeSpan e2)
        {
            TimeSpan value = new TimeSpan();
            if (s1 <= s2 && e1 >= e2)
            {
                TimeSpan ts = e2 - s2;
                if (ts.Ticks > 0)
                    value += ts;
            }
            if (s1 <= s2 && e1 < e2)
            {
                TimeSpan ts = e1 - s2;
                if (ts.Ticks > 0)
                    value += ts;
            }
            if (s1 > s2 && e1 >= e2)
            {
                TimeSpan ts = e2 - s1;
                if (ts.Ticks > 0)
                    if (ts.Ticks > 0)
                        value += ts;
            }
            if (s1 > s2 && e1 < e2)
            {
                TimeSpan ts = e1 - s1;
                if (ts.Ticks > 0)
                    if (ts.Ticks > 0)
                        value += ts;
            }
            return value;
        }

        public string AddOrUpdateGLStations(GL_StationDTO dto, bool isEdit)
        {
            string errorMessage = string.Empty;
            GL_Station entity = AutoMapper.Mapper.Map<GL_Station>(dto);
            if (entity.IsBirth == true || entity.IsOutput == true)
            {
                var entities = _stationRepository.GetMany(x => x.LineID == entity.LineID);
                if (entity.IsBirth == true)
                {
                    foreach (var e in entities)
                        e.IsBirth = false;
                }
                if (entity.IsOutput == true)
                {
                    foreach (var e in entities)
                        e.IsOutput = false;
                   
                }
            }
          
            try
            {
                GL_Station entityContext;
                if (dto.StationID == 0)
                {
                    var entities = _stationRepository.GetMany(x => x.LineID == entity.LineID&&x.Binding_Seq==dto.Binding_Seq).ToList();
                    if(entities!=null&& entities.Count>0)
                    {
                        return string.Format("此线下已经有绑定序号{0}", dto.Binding_Seq);

                    }
                    entity.MESStationName = entity.MESStationName ?? "";
                    entityContext = _stationRepository.Add(entity);
                    _unitOfWork.Commit();
                }
                else
                {
                    entityContext = _stationRepository.GetById(entity.StationID);
                    entityContext.Seq = entity.Seq;
                    entityContext.StationName = entity.StationName;
                    entityContext.MESStationName = entity.MESStationName ?? "";
                    entityContext.CycleTime = entity.CycleTime;
                    entityContext.IsBirth = entity.IsBirth;
                    entityContext.IsOutput = entity.IsOutput;
                    entityContext.IsTest = entity.IsTest;
                    entityContext.DashboardTarget = entity.DashboardTarget;
                    entityContext.Binding_Seq = entity.Binding_Seq;
                    entityContext.IsGoldenLine = entity.IsGoldenLine;
                    entityContext.IsOEE = entity.IsOEE;
                    entityContext.IsOne = entity.IsOne;
                    entityContext.IsTwo = entity.IsTwo;
                    entityContext.IsThree = entity.IsThree;
                    entityContext.IsFour = entity.IsFour;
                    entityContext.IsFive = entity.IsFive;
                    entityContext.Modified_UID = entity.Modified_UID;
                    entityContext.Modified_Date = entity.Modified_Date;
                    entityContext.IsEnabled = dto.IsEnabled;
                    _stationRepository.Update(entityContext);
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

    }
}