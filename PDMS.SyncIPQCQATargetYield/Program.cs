using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using PDMS.Service;
using PDMS.Data.Repository;
using PDMS.Model.EntityDTO;
using PDMS.Common.Constants;

namespace PDMS.SyncIPQCQATargetYield
{
    class Program
    {
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();
        static void Main(string[] args)
        {

            SyncIPQCQATargetYield();

        }
        public static string SyncIPQCQATargetYield()
        {
            string result = "";
            IPQCQualityService iPQCQualityService = new IPQCQualityService(
            new UnitOfWork(_DatabaseFactory),
            new IPQCQUalityReportRepository(_DatabaseFactory),
            new GL_QADetectionPointRepository(_DatabaseFactory),
            new GL_ShiftTimeRepository(_DatabaseFactory),
            new GL_StationRepository(_DatabaseFactory),
            new GL_QATargetYieldRepository(_DatabaseFactory),
            new IPQCQualityDetialRepository(_DatabaseFactory),
            new GL_GoldenStationCTRecordRepository(_DatabaseFactory),
            new GL_WIPHourOutputRepository(_DatabaseFactory)
           );
            //2018 - 11 - 22 10:01:57.000
            // 获取当前时间          
            DateTime dateNow = DateTime.Now;
            //DateTime dateNow = Convert.ToDateTime("2018-11-23 15:01:57.000");
            //获取所的IPQC站点
            var stationDTOs = iPQCQualityService.GetIPQCAllStationDTOs();
            //获取所有的班次
            var shiftTimeDTOs = iPQCQualityService.GetShiftTimeDTO(0, 0);
            //获取所有的IPQC检测点
            var gL_QADetectionPointDTOs = iPQCQualityService.GetAllGLQADetectionPointDTO();
            //要插入的IPQC良率报表数据对象
            List<GL_IPQCQualityReportDTO> iPQCQualityReportDTOs = new List<GL_IPQCQualityReportDTO>();
            //要插入的IPQC良率报表一次不良/二次不良数据对象
            List<GL_IPQCQualityDetialDTO> gL_IPQCQualityDetialDTOs = new List<GL_IPQCQualityDetialDTO>();

            if (stationDTOs != null && stationDTOs.Count > 0)
            {
                string connectionString = GetConnectionStrings();
                foreach (var item in stationDTOs)
                {
                    //获取当前站点的检测点
                    var gL_QADetectionPointDTO = gL_QADetectionPointDTOs.FirstOrDefault(o => o.StationID == item.StationID);
                    if (gL_QADetectionPointDTO != null)
                    {
                        //获取时间段 及其班次
                        var timeIntervalDTO = GetTimeIntervalDTO(shiftTimeDTOs, item, dateNow);

                        //解决处理跨时间点的5分钟内的数据问题
                        if ((timeIntervalDTO.NowDateTime - timeIntervalDTO.SerchStartTime).TotalMinutes < 5)
                        {
                            //要插入的IPQC良率报表数据对象
                            List<GL_IPQCQualityReportDTO> iPQCQualityReportDTOs1 = new List<GL_IPQCQualityReportDTO>();
                            //要插入的IPQC良率报表一次不良/二次不良数据对象
                            List<GL_IPQCQualityDetialDTO> gL_IPQCQualityDetialDTOs1 = new List<GL_IPQCQualityDetialDTO>();
                            //同步之前时间段的数据
                            var timeIntervalDTO1 = GetTimeIntervalDTO(shiftTimeDTOs, item, timeIntervalDTO.SerchStartTime);
                            // ScanIN  扫码IN
                            var ScanINs1 = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTO.ScanIN, timeIntervalDTO1.SerchStartTime, timeIntervalDTO1.SerchEndTime, connectionString);
                            // ScanBACK 扫码返修
                            var ScanBACKs1 = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTO.ScanBACK, timeIntervalDTO1.SerchStartTime, timeIntervalDTO1.SerchEndTime, connectionString);
                            // ScanNG 扫码NG
                            var ScanNGs1 = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTO.ScanNG, timeIntervalDTO1.SerchStartTime, timeIntervalDTO1.SerchEndTime, connectionString);
                            // ScanOUT 扫码 OUT
                            var ScanOUTs1 = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTO.ScanOUT, timeIntervalDTO1.SerchStartTime, timeIntervalDTO1.SerchEndTime, connectionString);

                            iPQCQualityReportDTOs1 = GetGL_IPQCQualityReportDTOs(item, iPQCQualityReportDTOs1, timeIntervalDTO1, ScanINs1, ScanBACKs1, ScanNGs1, ScanOUTs1, gL_QADetectionPointDTO);

                            iPQCQualityReportDTOs.AddRange(iPQCQualityReportDTOs1);
                            gL_IPQCQualityDetialDTOs1 = GetGL_IPQCQualityDetialDTOs(item, gL_IPQCQualityDetialDTOs1, timeIntervalDTO1, ScanBACKs1, ScanNGs1);
                            gL_IPQCQualityDetialDTOs.AddRange(gL_IPQCQualityDetialDTOs1);
                            //先插入WIP数据
                            if (iPQCQualityReportDTOs1 != null)
                            {
                                foreach (var itemWIP in iPQCQualityReportDTOs1)
                                {
                                    iPQCQualityService.InserOrUpdateIPQCWIP(itemWIP.StationID, itemWIP.WIP);

                                }
                            }

                            //同步之后时间段的数据
                            //需要重新获取监测点的WIP
                            var gL_QADetectionPointDTOnext = iPQCQualityService.GetStationsDetectionPointByID(item.StationID);
                            //统计数量
                            // ScanIN  扫码IN
                            var ScanINs = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTOnext.ScanIN, timeIntervalDTO.SerchStartTime, timeIntervalDTO.SerchEndTime, connectionString);
                            // ScanBACK 扫码返修
                            var ScanBACKs = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTOnext.ScanBACK, timeIntervalDTO.SerchStartTime, timeIntervalDTO.SerchEndTime, connectionString);
                            // ScanNG 扫码NG
                            var ScanNGs = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTOnext.ScanNG, timeIntervalDTO.SerchStartTime, timeIntervalDTO.SerchEndTime, connectionString);
                            // ScanOUT 扫码 OUT
                            var ScanOUTs = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTOnext.ScanOUT, timeIntervalDTO.SerchStartTime, timeIntervalDTO.SerchEndTime, connectionString);


                            // iPQCQualityReportDTOs.AddRange(GetGL_IPQCQualityReportDTOs(item, iPQCQualityReportDTOs, timeIntervalDTO, ScanINs, ScanBACKs, ScanNGs, ScanOUTs, gL_QADetectionPointDTOnext));
                            //gL_IPQCQualityDetialDTOs.AddRange(GetGL_IPQCQualityDetialDTOs(item, gL_IPQCQualityDetialDTOs, timeIntervalDTO, ScanBACKs, ScanNGs));
                            iPQCQualityReportDTOs = GetGL_IPQCQualityReportDTOs(item, iPQCQualityReportDTOs, timeIntervalDTO, ScanINs, ScanBACKs, ScanNGs, ScanOUTs, gL_QADetectionPointDTOnext);
                            gL_IPQCQualityDetialDTOs = GetGL_IPQCQualityDetialDTOs(item, gL_IPQCQualityDetialDTOs, timeIntervalDTO, ScanBACKs, ScanNGs);

                        }
                        else
                        {
                            //统计数量
                            // ScanIN  扫码IN
                            var ScanINs = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTO.ScanIN, timeIntervalDTO.SerchStartTime, timeIntervalDTO.SerchEndTime, connectionString);
                            // ScanBACK 扫码返修
                            var ScanBACKs = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTO.ScanBACK, timeIntervalDTO.SerchStartTime, timeIntervalDTO.SerchEndTime, connectionString);
                            // ScanNG 扫码NG
                            var ScanNGs = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTO.ScanNG, timeIntervalDTO.SerchStartTime, timeIntervalDTO.SerchEndTime, connectionString);
                            // ScanOUT 扫码 OUT
                            var ScanOUTs = GetWP_IPQC(item.MESProjectName, item.MESLineName, gL_QADetectionPointDTO.ScanOUT, timeIntervalDTO.SerchStartTime, timeIntervalDTO.SerchEndTime, connectionString);

                            //iPQCQualityReportDTOs.AddRange(GetGL_IPQCQualityReportDTOs(item, iPQCQualityReportDTOs, timeIntervalDTO, ScanINs, ScanBACKs, ScanNGs, ScanOUTs, gL_QADetectionPointDTO));
                            //gL_IPQCQualityDetialDTOs.AddRange(GetGL_IPQCQualityDetialDTOs(item, gL_IPQCQualityDetialDTOs, timeIntervalDTO, ScanBACKs, ScanNGs));

                            iPQCQualityReportDTOs = GetGL_IPQCQualityReportDTOs(item, iPQCQualityReportDTOs, timeIntervalDTO, ScanINs, ScanBACKs, ScanNGs, ScanOUTs, gL_QADetectionPointDTO);
                            gL_IPQCQualityDetialDTOs = GetGL_IPQCQualityDetialDTOs(item, gL_IPQCQualityDetialDTOs, timeIntervalDTO, ScanBACKs, ScanNGs);

                        }



                    }

                }
            }

            //把数据插入到数据库
            result = iPQCQualityService.InserOrUpdateIPQCReports(gL_IPQCQualityDetialDTOs, iPQCQualityReportDTOs);
            return result;
        }
        public static List<GL_IPQCQualityDetialDTO> GetGL_IPQCQualityDetialDTOs(GL_StationDTO item, List<GL_IPQCQualityDetialDTO> gL_IPQCQualityDetialDTOs, TimeIntervalDTO timeIntervalDTO, List<WP_IPQCDTO> ScanBACKs, List<WP_IPQCDTO> ScanNGs)
        {
            IPQCQualityService iPQCQualityService = new IPQCQualityService(
                                        new UnitOfWork(_DatabaseFactory),
                                        new IPQCQUalityReportRepository(_DatabaseFactory),
                                        new GL_QADetectionPointRepository(_DatabaseFactory),
                                        new GL_ShiftTimeRepository(_DatabaseFactory),
                                        new GL_StationRepository(_DatabaseFactory),
                                        new GL_QATargetYieldRepository(_DatabaseFactory),
                                        new IPQCQualityDetialRepository(_DatabaseFactory),
                                        new GL_GoldenStationCTRecordRepository(_DatabaseFactory),
                                        new GL_WIPHourOutputRepository(_DatabaseFactory)
                                        );
            #region IPQC不良明细数据组装
            //或取指定时间段的明细数据
            var allGLIPQCQualityDetialDTOs = iPQCQualityService.GetAllGL_IPQCQualityDetialDTO(item.StationID, timeIntervalDTO.ShiftTimeID, timeIntervalDTO.TimeIntervalIndex, timeIntervalDTO.ProductDate, timeIntervalDTO.TimeInterval);
            if (ScanBACKs != null)
            {
                #region 返修不良明细

                var groupScanBACKs = ScanBACKs.GroupBy(a => a.DefectName).Select(g => (new { name = g.Key, count = g.Count() })).ToList();
                foreach (var itemBACK in groupScanBACKs)
                {
                    GL_IPQCQualityDetialDTO gL_IPQCQualityDetialDTO = new GL_IPQCQualityDetialDTO();
                    gL_IPQCQualityDetialDTO.StationID = item.StationID;
                    gL_IPQCQualityDetialDTO.ShiftID = timeIntervalDTO.ShiftTimeID;
                    gL_IPQCQualityDetialDTO.TimeInterval = timeIntervalDTO.TimeInterval;
                    gL_IPQCQualityDetialDTO.TimeIntervalIndex = timeIntervalDTO.TimeIntervalIndex;
                    gL_IPQCQualityDetialDTO.ProductDate = timeIntervalDTO.ProductDate;
                    gL_IPQCQualityDetialDTO.NGName = itemBACK.name;
                    gL_IPQCQualityDetialDTO.NGNumber = itemBACK.count;
                    gL_IPQCQualityDetialDTO.NGType = "0";
                    gL_IPQCQualityDetialDTO.ModifyTime = DateTime.Now;

                    //  需要查看数据是否存在， 如果存在 则要把IPQCQualityDetial_UID赋值
                    if (allGLIPQCQualityDetialDTOs != null && allGLIPQCQualityDetialDTOs.Count > 0)
                    {
                        var iPQCQualityDetialDTO = allGLIPQCQualityDetialDTOs.FirstOrDefault(o => o.NGName == itemBACK.name && o.NGType == "0");
                        if (iPQCQualityDetialDTO != null)
                        {
                            gL_IPQCQualityDetialDTO.IPQCQualityDetial_UID = iPQCQualityDetialDTO.IPQCQualityDetial_UID;
                        }
                    }

                    gL_IPQCQualityDetialDTOs.Add(gL_IPQCQualityDetialDTO);
                }
                #endregion
            }
            if (ScanNGs != null)
            {
                #region NG不良明细
                var groupScanNGs = ScanNGs.GroupBy(a => a.DefectName).Select(g => (new { name = g.Key, count = g.Count() })).ToList();
                foreach (var itemNG in groupScanNGs)
                {
                    GL_IPQCQualityDetialDTO gL_IPQCQualityDetialDTO = new GL_IPQCQualityDetialDTO();
                    gL_IPQCQualityDetialDTO.StationID = item.StationID;
                    gL_IPQCQualityDetialDTO.ShiftID = timeIntervalDTO.ShiftTimeID;
                    gL_IPQCQualityDetialDTO.TimeInterval = timeIntervalDTO.TimeInterval;
                    gL_IPQCQualityDetialDTO.TimeIntervalIndex = timeIntervalDTO.TimeIntervalIndex;
                    gL_IPQCQualityDetialDTO.ProductDate = timeIntervalDTO.ProductDate;
                    gL_IPQCQualityDetialDTO.NGName = itemNG.name;
                    gL_IPQCQualityDetialDTO.NGNumber = itemNG.count;
                    gL_IPQCQualityDetialDTO.NGType = "1";
                    gL_IPQCQualityDetialDTO.ModifyTime = DateTime.Now;

                    //  需要查看数据是否存在， 如果存在 则要把IPQCQualityDetial_UID赋值
                    if (allGLIPQCQualityDetialDTOs != null && allGLIPQCQualityDetialDTOs.Count > 0)
                    {
                        var iPQCQualityDetialDTO = allGLIPQCQualityDetialDTOs.FirstOrDefault(o => o.NGName == itemNG.name && o.NGType == "1");
                        if (iPQCQualityDetialDTO != null)
                        {
                            gL_IPQCQualityDetialDTO.IPQCQualityDetial_UID = iPQCQualityDetialDTO.IPQCQualityDetial_UID;
                        }
                    }
                    gL_IPQCQualityDetialDTOs.Add(gL_IPQCQualityDetialDTO);
                }
                #endregion
            }
            #endregion

            return gL_IPQCQualityDetialDTOs;
        }
        public static List<GL_IPQCQualityReportDTO> GetGL_IPQCQualityReportDTOs(GL_StationDTO item, List<GL_IPQCQualityReportDTO> iPQCQualityReportDTOs, TimeIntervalDTO timeIntervalDTO,
            List<WP_IPQCDTO> ScanINs, List<WP_IPQCDTO> ScanBACKs, List<WP_IPQCDTO> ScanNGs, List<WP_IPQCDTO> ScanOUTs, GL_QADetectionPointDTO gL_QADetectionPointDTO)
        {

            IPQCQualityService iPQCQualityService = new IPQCQualityService(
                                                    new UnitOfWork(_DatabaseFactory),
                                                    new IPQCQUalityReportRepository(_DatabaseFactory),
                                                    new GL_QADetectionPointRepository(_DatabaseFactory),
                                                    new GL_ShiftTimeRepository(_DatabaseFactory),
                                                    new GL_StationRepository(_DatabaseFactory),
                                                    new GL_QATargetYieldRepository(_DatabaseFactory),
                                                    new IPQCQualityDetialRepository(_DatabaseFactory),
                                                    new GL_GoldenStationCTRecordRepository(_DatabaseFactory),
                                                    new GL_WIPHourOutputRepository(_DatabaseFactory)
                                                    );
            GL_IPQCQualityReportDTO gL_IPQCQualityReportDTO = new GL_IPQCQualityReportDTO();

            #region IPQC时段良率数据组装
            gL_IPQCQualityReportDTO.StationID = item.StationID;
            gL_IPQCQualityReportDTO.ShiftID = timeIntervalDTO.ShiftTimeID;
            gL_IPQCQualityReportDTO.TimeInterval = timeIntervalDTO.TimeInterval;
            gL_IPQCQualityReportDTO.TimeIntervalIndex = timeIntervalDTO.TimeIntervalIndex;
            gL_IPQCQualityReportDTO.ProductDate = timeIntervalDTO.ProductDate;
            //进料数
            gL_IPQCQualityReportDTO.InputNumber = GetInputNumber(timeIntervalDTO, item.StationID);
            //扫码 IN        
            gL_IPQCQualityReportDTO.TestNumber = ScanINs != null ? ScanINs.Count : 0;
            //返修
            gL_IPQCQualityReportDTO.RepairNumber = ScanBACKs != null ? ScanBACKs.Count : 0;
            //NG
            gL_IPQCQualityReportDTO.NGNumber = ScanNGs != null ? ScanNGs.Count : 0;
            //二次OK数 OUT数
            gL_IPQCQualityReportDTO.SecondPassNumber = ScanOUTs != null ? ScanOUTs.Count : 0;
            //一次OK数 (MES-IPQC扫码IN)-(MES-IPQC返修)-(MES-IPQC-NG)
            gL_IPQCQualityReportDTO.FirstPassNumber = gL_IPQCQualityReportDTO.TestNumber - gL_IPQCQualityReportDTO.RepairNumber - gL_IPQCQualityReportDTO.NGNumber;

            #region 设置一次二次良率
            //一次良率  一次OK数/一次检验数
            if (gL_IPQCQualityReportDTO.FirstPassNumber != 0 && gL_IPQCQualityReportDTO.TestNumber != 0)
            {
                gL_IPQCQualityReportDTO.FirstYield = (gL_IPQCQualityReportDTO.FirstPassNumber * 1.0) / gL_IPQCQualityReportDTO.TestNumber;
            }
            else
            {
                gL_IPQCQualityReportDTO.FirstYield = 0.0;
            }
            //二次良率 二次OK数/（二次OK数+NG数）
            if (gL_IPQCQualityReportDTO.SecondPassNumber != 0)
            {
                gL_IPQCQualityReportDTO.SecondYield = (gL_IPQCQualityReportDTO.SecondPassNumber * 1.0) / (gL_IPQCQualityReportDTO.SecondPassNumber + gL_IPQCQualityReportDTO.NGNumber);

            }
            else
            {
                gL_IPQCQualityReportDTO.SecondYield = 0.0;
            }

            #endregion

            #region 设置一次二次目标良率
            var gLQATargetYieldDTOs = iPQCQualityService.GetGLQATargetYieldDTOList(item.StationID, timeIntervalDTO.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate));
            if (gLQATargetYieldDTOs != null && gLQATargetYieldDTOs.Count > 0)
            {
                var oneTargetYield = gLQATargetYieldDTOs.FirstOrDefault(o => o.Tag == 1);
                if (oneTargetYield != null)
                {
                    //一次目标良率
                    gL_IPQCQualityReportDTO.FirstTargetYield = Convert.ToDouble(oneTargetYield.TargetYield);
                }
                else
                {
                    //一次目标良率
                    gL_IPQCQualityReportDTO.FirstTargetYield = 0;
                }
                var twoTargetYield = gLQATargetYieldDTOs.FirstOrDefault(o => o.Tag == 2);

                if (twoTargetYield != null)
                {
                    //二次目标良率
                    gL_IPQCQualityReportDTO.SecondTargetYield = Convert.ToDouble(twoTargetYield.TargetYield);
                }
                else
                {
                    //二次目标良率
                    gL_IPQCQualityReportDTO.SecondTargetYield = 0;
                }

            }
            else
            {
                //一次目标良率
                gL_IPQCQualityReportDTO.FirstTargetYield = 0;
                //二次目标良率
                gL_IPQCQualityReportDTO.SecondTargetYield = 0;
            }
            #endregion

            //WIP
            gL_IPQCQualityReportDTO.WIP = gL_QADetectionPointDTO.WIP + gL_IPQCQualityReportDTO.InputNumber - gL_IPQCQualityReportDTO.NGNumber - gL_IPQCQualityReportDTO.SecondPassNumber;// - gL_IPQCQualityReportDTO.FirstPassNumber;

            gL_IPQCQualityReportDTO.ModifyTime = DateTime.Now;
            gL_IPQCQualityReportDTO.NowDateTime = timeIntervalDTO.NowDateTime;
            gL_IPQCQualityReportDTO.SerchEndTime = timeIntervalDTO.SerchEndTime;
            gL_IPQCQualityReportDTO.SerchStartTime = timeIntervalDTO.SerchStartTime;

            //  需要查看数据是否存在 如果存在 则要把IPQCQualityReport_UID赋值
            var iPQCQualityReportDTO = iPQCQualityService.GetGLIPQCQualityReportDTO(item.StationID, timeIntervalDTO.ShiftTimeID, timeIntervalDTO.TimeIntervalIndex, timeIntervalDTO.ProductDate, timeIntervalDTO.TimeInterval);
            if (iPQCQualityReportDTO != null)
            {
                gL_IPQCQualityReportDTO.IPQCQualityReport_UID = iPQCQualityReportDTO.IPQCQualityReport_UID;
            }

            iPQCQualityReportDTOs.Add(gL_IPQCQualityReportDTO);
            #endregion

            return iPQCQualityReportDTOs;
        }
        public static int GetInputNumber(TimeIntervalDTO timeIntervalDTO, int StationID)
        {
            IPQCQualityService iPQCQualityService = new IPQCQualityService(
            new UnitOfWork(_DatabaseFactory),
            new IPQCQUalityReportRepository(_DatabaseFactory),
            new GL_QADetectionPointRepository(_DatabaseFactory),
            new GL_ShiftTimeRepository(_DatabaseFactory),
            new GL_StationRepository(_DatabaseFactory),
            new GL_QATargetYieldRepository(_DatabaseFactory),
            new IPQCQualityDetialRepository(_DatabaseFactory),
            new GL_GoldenStationCTRecordRepository(_DatabaseFactory),
            new GL_WIPHourOutputRepository(_DatabaseFactory)
           );
            int InputNumber = 0;
            //获取两小时的数据索引
            List<int> HourIndexs = new List<int>();
            HourIndexs.Add(timeIntervalDTO.TimeIntervalIndex * 2);
            HourIndexs.Add(timeIntervalDTO.TimeIntervalIndex * 2 - 1);

            var wIPHourOutputDTOs = iPQCQualityService.GetGL_WIPHourOutputDTOs(StationID, timeIntervalDTO.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate), HourIndexs, timeIntervalDTO.ShiftTimeID);
            if (wIPHourOutputDTOs != null && wIPHourOutputDTOs.Count > 0)
            {
                InputNumber = wIPHourOutputDTOs.Sum(x => x.ActualOutput);
            }
            return InputNumber;

        }
        public static TimeIntervalDTO GetTimeIntervalDTO(List<GL_ShiftTimeDTO> shiftTimeDTOs, GL_StationDTO item, DateTime dateNow)
        {

            TimeIntervalDTO timeIntervalDTO = new TimeIntervalDTO();
            #region IPQC获取班次相关

            //获取当前站点所在厂区，OP的所有班次
            var shiftTimes = shiftTimeDTOs.Where(o => o.Plant_Organization_UID == item.Plant_Organization_UID && o.BG_Organization_UID == item.BG_Organization_UID);
            //得到当前dateNow时间所属的班次
            var shiftTimeDTO = GetGL_ShiftTimeDTO(shiftTimes.ToList(), dateNow);
            if (shiftTimeDTO != null)
            {

                //得到所属时间dateNow 所在班次的开始时间和技术时间
                DateTime StartTime = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + shiftTimeDTO.StartTime));
                DateTime EndTime = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + shiftTimeDTO.End_Time));
                //同步当前班次的数据
                if (StartTime > EndTime)
                {
                    StartTime = StartTime.AddDays(-1);
                }
                timeIntervalDTO.ShiftTimeID = shiftTimeDTO.ShiftTimeID;
                timeIntervalDTO.StartTime = StartTime;
                timeIntervalDTO.EndTime = EndTime;
                timeIntervalDTO.NowDateTime = dateNow;
                timeIntervalDTO.ProductDate = Convert.ToDateTime(StartTime.ToString(FormatConstants.DateTimeFormatStringByDate));
                //设置第几个时间段
                timeIntervalDTO.TimeIntervalIndex = GetTimeIntervalIndex(timeIntervalDTO);
                //设置时间段字符串
                timeIntervalDTO = GetTimeInterval(timeIntervalDTO);

            }
            #endregion

            return timeIntervalDTO;

        }
        /// <summary>
        /// 设置时间段字符串
        /// </summary>
        /// <param name="timeIntervalDTO"></param>
        /// <returns></returns>
        public static TimeIntervalDTO GetTimeInterval(TimeIntervalDTO timeIntervalDTO)
        {
            timeIntervalDTO.SerchStartTime = timeIntervalDTO.StartTime.AddHours(2 * (timeIntervalDTO.TimeIntervalIndex - 1));
            timeIntervalDTO.SerchEndTime = timeIntervalDTO.StartTime.AddHours(2 * timeIntervalDTO.TimeIntervalIndex);
            timeIntervalDTO.TimeInterval = timeIntervalDTO.SerchStartTime.ToString(FormatConstants.DateTimeFormatStringByMin).Substring(11) + "-" + timeIntervalDTO.SerchEndTime.ToString(FormatConstants.DateTimeFormatStringByMin).Substring(11);
            return timeIntervalDTO;
        }
        /// <summary>
        /// 设置时间段
        /// </summary>
        /// <param name="timeIntervalDTO"></param>
        /// <returns></returns>
        public static int GetTimeIntervalIndex(TimeIntervalDTO timeIntervalDTO)
        {
            int timeIntervalIndex = 1;
            int hourIndex = Math.Abs(timeIntervalDTO.StartTime.Hour - timeIntervalDTO.EndTime.Hour);
            hourIndex = hourIndex / 2 + 1;
            for (int k = 1; k <= hourIndex; k++)
            {
                if (timeIntervalDTO.StartTime.AddHours(2 * (k - 1)) < timeIntervalDTO.NowDateTime && timeIntervalDTO.NowDateTime <= timeIntervalDTO.StartTime.AddHours(2 * k))
                {
                    timeIntervalIndex = k;
                }
            }
            return timeIntervalIndex;
        }
        /// <summary>
        /// 获取并判断时间点所在的班次范围
        /// </summary>
        /// <param name="GL_ShiftTimeDTOs">班次的所有列表</param>
        /// <param name="dateNow">时间点</param>
        /// <returns></returns>
        public static GL_ShiftTimeDTO GetGL_ShiftTimeDTO(List<GL_ShiftTimeDTO> GL_ShiftTimeDTOs, DateTime dateNow)
        {
            GL_ShiftTimeDTO GL_ShiftTimeDTO = new GL_ShiftTimeDTO();
            string strdateNow = dateNow.ToString("yyyy-MM-dd");
            foreach (var item in GL_ShiftTimeDTOs)
            {
                DateTime StartdDateTime = Convert.ToDateTime((strdateNow + " " + item.StartTime));
                DateTime EndDateTime = Convert.ToDateTime((strdateNow + " " + item.End_Time));
                if (StartdDateTime <= EndDateTime)
                {
                    if ((StartdDateTime < dateNow && dateNow <= EndDateTime) || (StartdDateTime.AddDays(-1) < dateNow && dateNow <= EndDateTime.AddDays(-1)) || (StartdDateTime.AddDays(1) < dateNow && dateNow <= EndDateTime.AddDays(1)))
                    {
                        GL_ShiftTimeDTO = item;
                    }
                }
                else
                {
                    EndDateTime = EndDateTime.AddDays(1);
                    if ((StartdDateTime < dateNow && dateNow <= EndDateTime) || (StartdDateTime.AddDays(-1) < dateNow && dateNow <= EndDateTime.AddDays(-1)) || (StartdDateTime.AddDays(1) < dateNow && dateNow <= EndDateTime.AddDays(1)))
                    {
                        GL_ShiftTimeDTO = item;
                    }
                }

            }

            return GL_ShiftTimeDTO;
        }
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionStrings()
        {
            return ConfigurationManager.ConnectionStrings["SyncIPQCQATargetYield"].ConnectionString;
        }
        /// <summary>
        /// 获取IPQC 数据
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="LineName"></param>
        /// <param name="RouteStep"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<WP_IPQCDTO> GetWP_IPQC(string CustomerName, string LineName, string RouteStep, DateTime StartTime, DateTime EndTime, string connectionString)
        {

            List<WP_IPQCDTO> WipeventDTOs = new List<WP_IPQCDTO>();
            WP_IPQCDTO model = null;
            //connectionString = @"Server=CNWXIg0lsqlv01b\inst1;DataBase=MESLDB;uid=PISUser;pwd=PISuser123";
            string cmdText = @"SELECT * FROM WP_IPQC where Customer=@CustomerName and LineName=@LineName and RouteStep=@RouteStep and LastUpdated>=@StartTime AND  LastUpdated<=@EndTime;";
            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@CustomerName",CustomerName),
                new SqlParameter("@LineName",LineName),
                new SqlParameter("@RouteStep",RouteStep),
                new SqlParameter("@StartTime",StartTime),
                new SqlParameter("@EndTime",EndTime)
            };
            CommandType commandType = CommandType.Text;
            //using (SqlDataReader dr = SqlHelper.ExecuteReader(GetConnectionStrings(), commandType, cmdText, paras))
            using (SqlDataReader dr = SqlHelper.ExecuteReader(connectionString, commandType, cmdText, paras))
            {
                while (dr.Read())
                {
                    model = new WP_IPQCDTO
                    {

                        ID = Convert.ToInt32(dr["ID"]),
                        Customer = Convert.ToString(dr["Customer"]),
                        LineName = Convert.ToString(dr["LineName"]),
                        RouteStep = Convert.ToString(dr["RouteStep"]),
                        SN = Convert.ToString(dr["SN"]),
                        DefectName = Convert.ToString(dr["DefectName"]),
                        NTID = Convert.ToString(dr["NTID"]),
                        LastUpdated = Convert.ToDateTime(dr["LastUpdated"]),

                    };
                    WipeventDTOs.Add(model);
                };
            }




            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    using (SqlCommand cmd = conn.CreateCommand())
            //    {

            //        string cmdText = @"SELECT * FROM WP_IPQC where Customer=@customerName and LineName=@lineName and RouteStep=@routeStep and LastUpdated>=@startTime AND  LastUpdated<=@endTime;";

            //        cmd.Parameters.Add(new SqlParameter("@customerName", CustomerName));
            //        cmd.Parameters.Add(new SqlParameter("@lineName", LineName));
            //        cmd.Parameters.Add(new SqlParameter("@routeStep", RouteStep));
            //        cmd.Parameters.Add(new SqlParameter("@startTime", StartTime));
            //        cmd.Parameters.Add(new SqlParameter("@endTime", EndTime));

            //        cmd.CommandText = cmdText;

            //        try
            //        {
            //            conn.Open();
            //            using (var dr = cmd.ExecuteReader())
            //            {
            //                while (dr.Read())//读取每行数据
            //                {
            //                    model = new WP_IPQCDTO
            //                    {

            //                        ID = Convert.ToInt32(dr["ID"]),
            //                        Customer = Convert.ToString(dr["Customer"]),
            //                        LineName = Convert.ToString(dr["LineName"]),
            //                        RouteStep = Convert.ToString(dr["RouteStep"]),
            //                        SN = Convert.ToString(dr["SN"]),
            //                        DefectName = Convert.ToString(dr["DefectName"]),
            //                        NTID = Convert.ToString(dr["NTID"]),
            //                        LastUpdated = Convert.ToDateTime(dr["LastUpdated"]),

            //                    };
            //                    WipeventDTOs.Add(model);
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.ToString());

            //        }
            //        finally
            //        {
            //            cmd.Dispose();
            //            conn.Close();
            //        }
            //    }
            //}
            return WipeventDTOs;
        }

    }
    public class WP_IPQCDTO
    {
        public int ID { get; set; }
        public string Customer { get; set; }
        public string RouteStep { get; set; }
        public string LineName { get; set; }
        public string SN { get; set; }
        public string DefectName { get; set; }
        public string NTID { get; set; }
        public DateTime LastUpdated { get; set; }
    }
    public class TimeIntervalDTO
    {
        /// <summary>
        /// 时间段字符串
        /// </summary>
        public string TimeInterval { get; set; }
        /// <summary>
        /// 第几个两小时
        /// </summary>
        public int TimeIntervalIndex { get; set; }
        /// <summary>
        /// 当前要同步的时间
        /// </summary>
        public DateTime NowDateTime { get; set; }
        /// <summary>
        /// 班次开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 班次结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 班次ID
        /// </summary>
        public int ShiftTimeID { get; set; }
        /// <summary>
        /// 计算数据所属日期
        /// </summary>
        public DateTime ProductDate { get; set; }

        /// <summary>
        /// 班次开始时间
        /// </summary>
        public DateTime SerchStartTime { get; set; }
        /// <summary>
        /// 班次结束时间
        /// </summary>
        public DateTime SerchEndTime { get; set; }

    }
}
