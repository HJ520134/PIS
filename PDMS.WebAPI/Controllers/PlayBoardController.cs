using Newtonsoft.Json;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace PDMS.WebAPI.Controllers
{
    public class PlayBoardController : ApiController
    {
        IPlayBoard_ViewService viewService;
        IPlayBoard_SettingService settingService;

        public PlayBoardController(IPlayBoard_ViewService viewService, IPlayBoard_SettingService settingService)
        {
            this.viewService = viewService;
            this.settingService = settingService;
        }

        /// <summary>
        /// 保存新增播放设置
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SaveAddPlayBoardSettingAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var model = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(jsondata);
            var result = settingService.SaveAddPlayBoardSetting(model);
            return result;
        }

        /// <summary>
        /// 保存编辑播放设置
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SaveEditPlayBoardSettingAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var model = JsonConvert.DeserializeObject<PlayBoard_SettingDTO>(jsondata);
            var result = settingService.SaveEditPlayBoardSetting(model);
            return result;
        }


        [HttpPost]
        public IHttpActionResult QueryPlayBoardSettingsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<PlayBoard_SettingModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = settingService.QueryPlayBoard_Settings(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public ApiResultModel QueryPlayBoardSettingAPI(int id)
        {
            var model = new ApiResultModel();
            var searchModel = new QueryModel<PlayBoard_SettingModelSearch>();
            try
            {
                searchModel.Equal = new PlayBoard_SettingModelSearch() { PlayBoard_Setting_ID = id };
                var entity = settingService.Query(searchModel).FirstOrDefault();
                if (entity != null)
                {
                    var dto = AutoMapper.Mapper.Map<PlayBoard_SettingDTO>(entity);
                    if (entity.PlayBoard_PlayTime.Count() > 0)
                    {
                        var playTime = new StringBuilder();
                        foreach (var item in entity.PlayBoard_PlayTime)
                        {
                            playTime.Append(string.Format("{0}~{1};", item.StartTime, item.EndTime));
                        }
                        dto.PlayTime = playTime.ToString().Trim(';');
                    }
                    dto.PlantName = entity.System_Organization.Organization_Name;
                    dto.BGName = entity.System_Organization1.Organization_Name;
                    if (entity.System_Organization2 != null)
                    {
                        dto.FunPlantName = entity.System_Organization2.Organization_Name;
                    }
                    dto.Play_UserName = string.Format("{0} {1}", entity.System_Users.User_NTID, entity.System_Users.User_Name);

                    model.data = dto;
                    model.isSuccess = true;
                }
                else
                {
                    model.isSuccess = false;
                    model.message = "未查到数据";
                }

            }
            catch (Exception ex)
            {
                model.isSuccess = false;
                model.message = ex.Message;
            }

            return model;
        }

        public PlayBoard_SettingDTO GetPlayBoardSettingByIDAPI(int settingID)
        {
            return settingService.GetPlayBoardSettingByID(settingID);
        }

        [HttpGet]
        public IList<PlayBoard_SettingDTO> GetEnabledInTurnPlayBoardSettingsByPlayUIDAPI(int playUserUID)
        {
            var searchModel = new QueryModel<PlayBoard_SettingModelSearch>() { Equal = new PlayBoard_SettingModelSearch() { Play_UID = playUserUID, IsEnabled = true, IsTiming = false } };
            var result = settingService.Query(searchModel);
            var dotList = new List<PlayBoard_SettingDTO>();
            foreach (var item in result)
            {
                var dto = AutoMapper.Mapper.Map<PlayBoard_SettingDTO>(item);
                dto.ActionName = item.PlayBoard_View.ActionName;
                dto.ViewName = item.PlayBoard_View.Name;
                dotList.Add(dto);
            }
            return dotList;
        }

        [HttpGet]
        public PlayBoard_SettingDTO GetEnabledTimmingPlayBoardSettingsByPlayUIDAPI(int playUserUID)
        {
            var searchModel = new QueryModel<PlayBoard_SettingModelSearch>() { Equal = new PlayBoard_SettingModelSearch() { Play_UID = playUserUID, IsEnabled = true, IsTiming = true } };
            //IsJsonParameterNeed 为true, 并且JsonParameter 为空的将不可播放，因为参数不对不可正常播放
            var result = settingService.Query(searchModel).Where(x => !x.PlayBoard_View.IsJsonParameterNeed || (x.PlayBoard_View.IsJsonParameterNeed && x.JsonParameter != null && x.JsonParameter != ""));

            //返回对象
            PlayBoard_SettingDTO resultDto = null;

            //先找有定时的Setting，PlaySeq 大的优先
            var timingList = result.Where(x => x.PlayBoard_PlayTime.Count > 0).OrderByDescending(x => x.PlaySeq);
            foreach (var item in timingList)
            {
                var timingDto = AutoMapper.Mapper.Map<PlayBoard_SettingDTO>(item);
                timingDto.ActionName = item.PlayBoard_View.ActionName;
                timingDto.ViewName = item.PlayBoard_View.Name;

                //如果设置了播放时间，则判断是否当前播放
                if (item.PlayBoard_PlayTime.Count > 0)
                {
                    //获取定时
                    timingDto.PlayBoard_PlayTimeDTOList = AutoMapper.Mapper.Map<List<PlayBoard_PlayTimeDTO>>(item.PlayBoard_PlayTime);
                    if (timingDto.PlayBoard_PlayTimeDTOList.Count > 0)
                    {
                        timingDto.PlayTime = "";
                        foreach (var playTime in timingDto.PlayBoard_PlayTimeDTOList)
                        {
                            var playTimeDuring = string.Format("{0}~{1}", playTime.StartTime, playTime.EndTime);
                            timingDto.PlayTime += playTimeDuring + ";";

                            //获取当前播放时间，在播放画面显示
                            try
                            {
                                var now = DateTime.Now;
                                var startHourMinute = playTime.StartTime.Split(':');
                                var endtHourMinute = playTime.EndTime.Split(':');
                                var startTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(startHourMinute[0]), int.Parse(startHourMinute[1]), 0);
                                var endTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(endtHourMinute[0]), int.Parse(endtHourMinute[1]), 0);
                                if (now >= startTime && now < endTime)
                                {
                                    //在当前时间段，则设置CurrentPlayTime
                                    if (string.IsNullOrEmpty(timingDto.CurrentPlayTime))
                                    {
                                        timingDto.CurrentPlayTime = "[" + playTimeDuring + "]";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(timingDto.CurrentPlayTime))
                {
                    //CurrentPlayTime不为空，表明需要播放此画面
                    //找到了这个对象立即赋给返回对象,并跳出循环
                    resultDto = timingDto;
                    break;
                }
            }

            //有播放时间的没找到，在找没有播放时间的
            if (resultDto == null)
            {
                var notTiming = result.Where(x => x.PlayBoard_PlayTime.Count == 0).OrderByDescending(x => x.PlaySeq).FirstOrDefault();
                if (notTiming != null)
                {
                    resultDto = AutoMapper.Mapper.Map<PlayBoard_SettingDTO>(notTiming);
                    resultDto.ActionName = notTiming.PlayBoard_View.ActionName;
                    resultDto.ViewName = notTiming.PlayBoard_View.Name;
                    //若未设置播放时间，则全天播放
                    resultDto.CurrentPlayTime = "[全天]";
                }
            }
            return resultDto;
        }

        [HttpGet]
        public ApiResultModel DeletePlayBoardSettingAPI(int settingID)
        {
            var result = settingService.DeletePlayBoardSetting(settingID);
            return result;
        }

        #region Picture
        [HttpGet]
        public ApiResultModel SavePictureSettingAPI(int settingID, string fileName, int modifiedUID)
        {
            var result = settingService.SavePictureSetting(settingID, fileName, modifiedUID);
            return result;
        }
        #endregion
        #region PPT
        [HttpGet]
        public ApiResultModel SavePPTSettingAPI(int settingID, string fileName, int sheetPlayTime, int modifiedUID)
        {
            var result = settingService.SavePPTSetting(settingID, fileName, sheetPlayTime, modifiedUID);
            return result;
        }

        [HttpGet]
        public IHttpActionResult GetPlayUsersByOPTypeAPI(int opTypeUID, int? funcOrgID)
        {
            var result = settingService.GetPlayUsersByOPType(opTypeUID, funcOrgID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetPlayBoardViewListAPI()
        {
            var result = settingService.GetPlayBoardViewList();
            return Ok(result);
        }
        #endregion

        #region EBoardShow
        [HttpPost]
        public ApiResultModel SaveSettingParameterAPI(dynamic data)
        {
            var dynamicModel = new { settingID = 0, parameter = "", modifiedUID = 0 };
            var jsondata = data.ToString();
            var model = JsonConvert.DeserializeAnonymousType(jsondata, dynamicModel);
            var result = settingService.SaveSettingParameter(model.settingID, model.parameter, model.modifiedUID);
            return result;
        }
        #endregion

        #region Video
        [HttpGet]
        public ApiResultModel SaveVideoSettingAPI(int settingID, string fileName, int modifiedUID)
        {
            var result = settingService.SaveVideoSetting(settingID, fileName, modifiedUID);
            return result;
        }
        #endregion
    }
}