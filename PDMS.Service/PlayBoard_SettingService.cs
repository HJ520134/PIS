using Newtonsoft.Json;
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
    public interface IPlayBoard_SettingService : IBaseSevice<PlayBoard_Setting, PlayBoard_SettingDTO, PlayBoard_SettingModelSearch>
    {
        PagedListModel<PlayBoard_SettingDTO> QueryPlayBoard_Settings(PlayBoard_SettingModelSearch search, Page page);
        PlayBoard_SettingDTO GetPlayBoardSettingByID(int settingID);
        ApiResultModel SavePictureSetting(int settingID, string fileName, int modifiedUID);
        ApiResultModel SavePPTSetting(int settingID, string fileName, int sheetPlayTime, int modifiedUID);
        ApiResultModel DeletePlayBoardSetting(int settingID);
        List<PlayBoard_PlayUsetDTO> GetPlayUsersByOPType(int opTypeUID, int? funcOrgID);
        List<PlayBoard_ViewDTO> GetPlayBoardViewList();
        ApiResultModel SaveSettingParameter(int settingID, string jsonParameter, int modifiedUID);
        ApiResultModel SaveAddPlayBoardSetting(PlayBoard_SettingDTO dto);
        ApiResultModel SaveEditPlayBoardSetting(PlayBoard_SettingDTO dto);
        ApiResultModel SaveVideoSetting(int settingID, string fileName, int modifiedUID);
    }
    public class PlayBoard_SettingService : BaseSevice<PlayBoard_Setting, PlayBoard_SettingDTO, PlayBoard_SettingModelSearch>, IPlayBoard_SettingService
    {
        private readonly IPlayBoard_SettingRepository settingRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ISystemUserOrgRepository systemUserOrgRepository;
        private readonly ISystemUserRepository systemUserRepository;
        private readonly IPlayBoard_ViewRepository playBoardViewRepository;
        private readonly IPlayBoard_PlayTimeRepository playBoardPlayTimeRepository;
        public PlayBoard_SettingService(IUnitOfWork unitOfWork, IPlayBoard_SettingRepository settingRepository
            , ISystemUserOrgRepository systemUserOrgRepository, ISystemUserRepository systemUserRepository
            , IPlayBoard_ViewRepository playBoardViewRepository, IPlayBoard_PlayTimeRepository playBoardPlayTimeRepository) : base(settingRepository)
        {
            this.unitOfWork = unitOfWork;
            this.settingRepository = settingRepository;
            this.systemUserOrgRepository = systemUserOrgRepository;
            this.systemUserRepository = systemUserRepository;
            this.playBoardViewRepository = playBoardViewRepository;
            this.playBoardPlayTimeRepository = playBoardPlayTimeRepository;
        }
        public PagedListModel<PlayBoard_SettingDTO> QueryPlayBoard_Settings(PlayBoard_SettingModelSearch search, Page page)
        {
            var totalCount = 0;
            var searchModel = new QueryModel<PlayBoard_SettingModelSearch>() { Equal = search };
            var queryResult = Query(searchModel);
            totalCount = queryResult.Count();

            //分页
            var pagedResult = queryResult.Where(x=>x.System_Users.Enable_Flag == true).OrderBy(x => x.Play_UID).ThenByDescending(x => x.IsTiming).ThenByDescending(x => x.PlaySeq).Skip(page.Skip).Take(page.PageSize);
            var playBoardSettingDtoList = new List<PlayBoard_SettingDTO>(); //AutoMapper.Mapper.Map<List<PlayBoard_SettingDTO>>(demandDList);
            foreach (var item in pagedResult)
            {
                var dto = AutoMapper.Mapper.Map<PlayBoard_SettingDTO>(item);
                dto.PlantName = item.System_Organization.Organization_Name;
                dto.BGName = item.System_Organization1.Organization_Name;
                if (item.System_Organization2 != null)
                {
                    dto.FunPlantName = item.System_Organization2.Organization_Name;
                }
                dto.ViewName = item.PlayBoard_View.Name;
                dto.ActionName = item.PlayBoard_View.ActionName;
                dto.SettingActionName = item.PlayBoard_View.SettingActionName;
                dto.Play_UserNTID = item.System_Users.User_NTID;
                dto.Play_UserName = item.System_Users.User_Name;
                dto.Created_UserName = item.System_Users1.User_Name;
                dto.Modified_UserName = item.System_Users2.User_Name;

                dto.PlayBoard_PlayTimeDTOList = AutoMapper.Mapper.Map<List<PlayBoard_PlayTimeDTO>>(item.PlayBoard_PlayTime);
                if (dto.PlayBoard_PlayTimeDTOList.Count > 0)
                {
                    dto.PlayTime = "";
                    foreach (var playTime in dto.PlayBoard_PlayTimeDTOList)
                    {
                        dto.PlayTime += string.Format("{0}~{1};", playTime.StartTime, playTime.EndTime);
                    }
                }
                else
                {
                    if (dto.IsTiming)
                    {
                        //未设置播放时间的定时播放，认为是全天播放
                        dto.PlayTime = "[全天]";
                    }
                }

                playBoardSettingDtoList.Add(dto);
            }
            //按播放序号排序
            return new PagedListModel<PlayBoard_SettingDTO>(totalCount, playBoardSettingDtoList);
        }

        public PlayBoard_SettingDTO GetPlayBoardSettingByID(int settingID)
        {
            var result = settingRepository.GetFirstOrDefault(x => x.PlayBoard_Setting_ID == settingID);
            PlayBoard_SettingDTO dto = null;
            if (result != null)
            {
                dto = AutoMapper.Mapper.Map<PlayBoard_SettingDTO>(result);
                dto.ActionName = result.PlayBoard_View.ActionName;
                dto.SettingActionName = result.PlayBoard_View.SettingActionName;
                dto.Play_UserName = result.System_Users.User_Name;
                dto.Play_UserNTID = result.System_Users.User_NTID;
            }
            return dto;
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="settingID"></param>
        /// <param name="fileName"></param>
        /// <param name="modifiedUID"></param>
        /// <returns></returns>
        public ApiResultModel SavePictureSetting(int settingID, string fileName, int modifiedUID)
        {
            var resultModel = new ApiResultModel();
            resultModel.isSuccess = false;
            try
            {
                var entity = settingRepository.GetFirstOrDefault(x => x.PlayBoard_Setting_ID == settingID);
                if (entity != null)
                {
                    entity.JsonParameter = fileName;
                    entity.Modified_Date = DateTime.Now;
                    entity.Modified_UID = modifiedUID;
                    settingRepository.Update(entity);
                    unitOfWork.Commit();
                    resultModel.isSuccess = true;
                }
                else
                {
                    resultModel.message = "没有要更新的对象";
                }
            }
            catch (Exception ex)
            {
                resultModel.message = ex.Message;
            }
            return resultModel;
        }
        public ApiResultModel SavePPTSetting(int settingID, string fileName, int sheetPlayTime, int modifiedUID)
        {
            var resultModel = new ApiResultModel();
            try
            {
                var entity = settingRepository.GetFirstOrDefault(x => x.PlayBoard_Setting_ID == settingID);
                if (entity != null)
                {
                    entity.JsonParameter = JsonConvert.SerializeObject(new { fileName = fileName, sheetPlayTime = sheetPlayTime });
                    entity.Modified_Date = DateTime.Now;
                    entity.Modified_UID = modifiedUID;
                    settingRepository.Update(entity);
                    unitOfWork.Commit();
                    resultModel.isSuccess = true;
                }
                else
                {
                    resultModel.isSuccess = false;
                    resultModel.message = "没有要更新的对象";
                }
            }
            catch (Exception ex)
            {
                resultModel.isSuccess = false;
                resultModel.message = ex.Message;
            }
            return resultModel;
        }

        public ApiResultModel DeletePlayBoardSetting(int settingID)
        {
            var resultModel = new ApiResultModel();
            var setting = settingRepository.GetFirstOrDefault(x => x.PlayBoard_Setting_ID == settingID);
            if (setting !=null)
            {
                try
                {
                    settingRepository.Delete(setting);
                    unitOfWork.Commit();
                    resultModel.isSuccess = true;
                }
                catch (Exception ex)
                {
                    resultModel.isSuccess = false;
                    resultModel.message = ex.Message;
                }
            }
            else
            {
                resultModel.isSuccess = false;
                resultModel.message = "没找到要删除的数据";
            }
            return resultModel;
        }

        public List<PlayBoard_PlayUsetDTO> GetPlayUsersByOPType(int opTypeUID, int? funcOrgID)
        {
            var query = systemUserOrgRepository.GetMany(x => x.OPType_OrganizationUID == opTypeUID && x.System_Users.Enable_Flag == true);
            if (funcOrgID.HasValue)
            {
                query = query.Where(x => x.Funplant_OrganizationUID == funcOrgID);
            }
            var accountUIDList = query.Select(x => x.Account_UID).ToList();
            var users = systemUserRepository.GetMany(x => accountUIDList.Contains(x.Account_UID));
            var dtoList = new List<PlayBoard_PlayUsetDTO>();
            foreach (var item in users)
            {
                //播放看板固定角色PlayBoardPlayUser 播放看板播放账号
                var role = item.System_User_Role.FirstOrDefault(x=>x.System_Role.Role_ID == "PlayBoardPlayUser");
                if (role!=null)
                {
                    var dto = AutoMapper.Mapper.Map<PlayBoard_PlayUsetDTO>(item);
                    dto.Role_ID = role.System_Role.Role_ID;
                    dto.Role_Name = role.System_Role.Role_Name;
                    dtoList.Add(dto);
                }
            }
            return dtoList;
        }

        public List<PlayBoard_ViewDTO> GetPlayBoardViewList()
        {
            var viewQuery = playBoardViewRepository.GetAll().ToList();
            var dtoList = AutoMapper.Mapper.Map<List<PlayBoard_ViewDTO>>(viewQuery);
            return dtoList;
        }

        public ApiResultModel SaveSettingParameter(int settingID, string jsonParameter, int modifiedUID)
        {
            var resultModel = new ApiResultModel();
            try
            {
                var entity = settingRepository.GetFirstOrDefault(x => x.PlayBoard_Setting_ID == settingID);
                if (entity != null)
                {
                    entity.JsonParameter = jsonParameter;
                    entity.Modified_Date = DateTime.Now;
                    entity.Modified_UID = modifiedUID;
                    settingRepository.Update(entity);
                    unitOfWork.Commit();
                    resultModel.isSuccess = true;
                }
                else
                {
                    resultModel.isSuccess = false;
                    resultModel.message = "没有要更新的对象";
                }
            }
            catch (Exception ex)
            {
                resultModel.isSuccess = false;
                resultModel.message = ex.Message;
            }
            return resultModel;
        }

        public ApiResultModel SaveAddPlayBoardSetting(PlayBoard_SettingDTO dto) {
            var resultModel = new ApiResultModel();
            try
            {
                var entity = AutoMapper.Mapper.Map<PlayBoard_Setting>(dto);
                var entityDb= settingRepository.Add(entity);
                if (entityDb != null)
                {
                    if (!string.IsNullOrWhiteSpace(dto.PlayTime))
                    {
                        var playTime = dto.PlayTime.Trim().Trim(';');
                        var playTimeList = playTime.Split(';');
                        foreach (var item in playTimeList)
                        {
                            var timePair = item.Split('~');
                            var timeStart = timePair[0];
                            var timeEnd = timePair[1];
                            playBoardPlayTimeRepository.Add(new PlayBoard_PlayTime() { PlayBoard_Setting_ID = entityDb.PlayBoard_Setting_ID, StartTime = timeStart, EndTime = timeEnd, Modified_UID = entityDb.Modified_UID, Modified_Date = entityDb.Modified_Date });
                        }
                    }
                    unitOfWork.Commit();
                    resultModel.isSuccess = true;
                }
                else
                {
                    resultModel.isSuccess = false;
                    resultModel.message = "新增失败";
                }
            }
            catch (Exception ex)
            {
                resultModel.isSuccess = false;
                resultModel.message = ex.Message;
            }
            return resultModel;
        }
        public ApiResultModel SaveEditPlayBoardSetting(PlayBoard_SettingDTO dto) {
            var resultModel = new ApiResultModel();
            try
            {
                var entityDb= settingRepository.GetFirstOrDefault(x=>x.PlayBoard_Setting_ID == dto.PlayBoard_Setting_ID);
                if (entityDb != null)
                {
                    var oldPlayTimeList = entityDb.PlayBoard_PlayTime.ToList();
                    if (oldPlayTimeList.Count > 0)
                    {
                        playBoardPlayTimeRepository.DeleteList(oldPlayTimeList);
                    }
                    
                    if (!string.IsNullOrWhiteSpace(dto.PlayTime))
                    {
                        var playTime = dto.PlayTime.Trim().Trim(';');
                        var playTimeList = playTime.Split(';');
                        foreach (var item in playTimeList)
                        {
                            var timePair = item.Split('~');
                            var timeStart = timePair[0];
                            var timeEnd = timePair[1];
                            playBoardPlayTimeRepository.Add(new PlayBoard_PlayTime() { PlayBoard_Setting_ID = entityDb.PlayBoard_Setting_ID, StartTime = timeStart, EndTime = timeEnd, Modified_UID = entityDb.Modified_UID, Modified_Date = entityDb.Modified_Date });
                        }
                    }
                    entityDb.PlayBoard_View_ID = dto.PlayBoard_View_ID;
                    entityDb.PlaySeq = dto.PlaySeq;
                    entityDb.Title = dto.Title;
                    entityDb.IsTiming = dto.IsTiming;
                    entityDb.IsEnabled = dto.IsEnabled;
                    entityDb.Remark = dto.Remark;
                    entityDb.Modified_UID = dto.Modified_UID;
                    entityDb.Modified_Date = dto.Modified_Date;

                    unitOfWork.Commit();
                    resultModel.isSuccess = true;
                }
                else
                {
                    resultModel.isSuccess = false;
                    resultModel.message = "没找到要编辑的数据";
                }
            }
            catch (Exception ex)
            {
                resultModel.isSuccess = false;
                resultModel.message = ex.Message;
            }
            return resultModel;
        }

        public ApiResultModel SaveVideoSetting(int settingID, string fileName, int modifiedUID)
        {
            var resultModel = new ApiResultModel();
            resultModel.isSuccess = false;
            try
            {
                var entity = settingRepository.GetFirstOrDefault(x => x.PlayBoard_Setting_ID == settingID);
                if (entity != null)
                {
                    entity.JsonParameter = fileName;
                    entity.Modified_Date = DateTime.Now;
                    entity.Modified_UID = modifiedUID;
                    settingRepository.Update(entity);
                    unitOfWork.Commit();
                    resultModel.isSuccess = true;
                }
                else
                {
                    resultModel.message = "没有要更新的对象";
                }
            }
            catch (Exception ex)
            {
                resultModel.message = ex.Message;
            }
            return resultModel;
        }
    }
}
