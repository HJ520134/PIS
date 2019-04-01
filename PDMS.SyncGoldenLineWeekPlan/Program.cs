using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.SyncGoldenLineWeekPlan
{
    class Program
    {
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();
        static void Main(string[] args)
        {


            SyncGoldenLineWeekPlan();

        }
        public static string SyncGoldenLineWeekPlan()
        {
            string result = "";
            GL_GoldenStationCTRecordService gL_GoldenStationCTRecordService = new GL_GoldenStationCTRecordService(new GL_GoldenStationCTRecordRepository(_DatabaseFactory), new UnitOfWork(_DatabaseFactory));
            //获取本周计划
            var ThisWeekPlans = gL_GoldenStationCTRecordService.GetWeekPlan(GetWeekDates(GetCurrentWeek(DateTime.Now)));
            //获取下周计划
            var NextWeekPlans = gL_GoldenStationCTRecordService.GetWeekPlan(GetWeekDates(GetNextWeek(DateTime.Now)));
            //根据本周计划和下周计划组装下周计划
            List<GLBuildPlanDTO> GLBuildPlanDTOs = new List<GLBuildPlanDTO>();

            foreach (var item in ThisWeekPlans)
            {
                GLBuildPlanDTO gLBuildPlanDTO = new GLBuildPlanDTO();
                string nextOutputDate = Convert.ToDateTime(item.OutputDate).AddDays(7).ToString(FormatConstants.DateTimeFormatStringByDate);
                if (NextWeekPlans != null && NextWeekPlans.Count > 0)
                {

                    var NextWeekPlan = NextWeekPlans.FirstOrDefault(o => o.LineID == item.LineID && o.ShiftTimeID == item.ShiftTimeID && o.OutputDate == nextOutputDate);
                    if (NextWeekPlan == null)
                    {
                        //新增
                        gLBuildPlanDTO.OutputDate = nextOutputDate;
                        // BuildPlanID
                        gLBuildPlanDTO.CustomerID = item.CustomerID;
                        gLBuildPlanDTO.LineID = item.LineID;
                        gLBuildPlanDTO.PlanOutput = item.PlanOutput;
                        gLBuildPlanDTO.ShiftTimeID = item.ShiftTimeID;
                        gLBuildPlanDTO.PlanHC = item.PlanHC;
                        gLBuildPlanDTO.ActualHC = item.ActualHC;
                        gLBuildPlanDTO.Created_UID = item.Created_UID;
                        gLBuildPlanDTO.Created_Date = item.Created_Date;
                        gLBuildPlanDTO.Modified_UID = item.Modified_UID;
                        gLBuildPlanDTO.Modified_Date = item.Modified_Date;

                    }
                    else
                    {
                        //编辑
                        gLBuildPlanDTO.BuildPlanID = NextWeekPlan.BuildPlanID;
                        gLBuildPlanDTO.OutputDate = nextOutputDate;
                        gLBuildPlanDTO.CustomerID = NextWeekPlan.CustomerID;
                        gLBuildPlanDTO.LineID = NextWeekPlan.LineID;
                        if (NextWeekPlan.PlanOutput == 0)
                        {
                            gLBuildPlanDTO.PlanOutput = item.PlanOutput;
                        }
                        gLBuildPlanDTO.ShiftTimeID = item.ShiftTimeID;
                        if (NextWeekPlan.PlanHC == null)
                        {
                            gLBuildPlanDTO.PlanHC = item.PlanHC;
                        }

                        if (NextWeekPlan.PlanHC == null)
                        {
                            gLBuildPlanDTO.PlanHC = item.PlanHC;
                        }
                        gLBuildPlanDTO.Created_UID = NextWeekPlan.Created_UID;
                        gLBuildPlanDTO.Created_Date = NextWeekPlan.Created_Date;
                        gLBuildPlanDTO.Modified_UID = item.Modified_UID;
                        gLBuildPlanDTO.Modified_Date = item.Modified_Date;

                    }

                }
                else
                {
                    //新增
                    gLBuildPlanDTO.OutputDate = nextOutputDate;
                    // BuildPlanID
                    gLBuildPlanDTO.CustomerID = item.CustomerID;
                    gLBuildPlanDTO.LineID = item.LineID;
                    gLBuildPlanDTO.PlanOutput = item.PlanOutput;
                    gLBuildPlanDTO.ShiftTimeID = item.ShiftTimeID;
                    gLBuildPlanDTO.PlanHC = item.PlanHC;
                    gLBuildPlanDTO.ActualHC = item.ActualHC;
                    gLBuildPlanDTO.Created_UID = item.Created_UID;
                    gLBuildPlanDTO.Created_Date = item.Created_Date;
                    gLBuildPlanDTO.Modified_UID = item.Modified_UID;
                    gLBuildPlanDTO.Modified_Date = item.Modified_Date;

                }
                GLBuildPlanDTOs.Add(gLBuildPlanDTO);

            }

            result = gL_GoldenStationCTRecordService.SyncGoldenLineWeekPlan(GLBuildPlanDTOs);
            return result;
        }

        /// <summary>
        /// 获取下周数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Week GetNextWeek(DateTime dt)
        {
            var strDT = dt.DayOfWeek.ToString();
            Week nextWeek = new Week();
            //获取下周一的日期
            switch (strDT)
            {
                case "Monday":
                    nextWeek.Monday = dt.AddDays(7);
                    break;
                case "Tuesday":
                    nextWeek.Monday = dt.AddDays(6);
                    break;
                case "Wednesday":
                    nextWeek.Monday = dt.AddDays(5);
                    break;
                case "Thursday":
                    nextWeek.Monday = dt.AddDays(4);
                    break;
                case "Friday":
                    nextWeek.Monday = dt.AddDays(3);
                    break;
                case "Saturday":
                    nextWeek.Monday = dt.AddDays(2);
                    break;
                case "Sunday":
                    nextWeek.Monday = dt.AddDays(1);
                    break;
            }
            nextWeek.Tuesday = nextWeek.Monday.AddDays(1);
            nextWeek.Wednesday = nextWeek.Monday.AddDays(2);
            nextWeek.Thursday = nextWeek.Monday.AddDays(3);
            nextWeek.Friday = nextWeek.Monday.AddDays(4);
            nextWeek.Saturday = nextWeek.Monday.AddDays(5);
            nextWeek.Sunday = nextWeek.Monday.AddDays(6);
            return nextWeek;
        }
        /// <summary>
        /// 根据周获取周的日期字符串列表
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        public static List<string> GetWeekDates(Week week)
        {

            List<string> weekDates = new List<string>();
            weekDates.Add(week.Monday.ToString(FormatConstants.DateTimeFormatStringByDate));
            weekDates.Add(week.Tuesday.ToString(FormatConstants.DateTimeFormatStringByDate));
            weekDates.Add(week.Wednesday.ToString(FormatConstants.DateTimeFormatStringByDate));
            weekDates.Add(week.Thursday.ToString(FormatConstants.DateTimeFormatStringByDate));
            weekDates.Add(week.Friday.ToString(FormatConstants.DateTimeFormatStringByDate));
            weekDates.Add(week.Saturday.ToString(FormatConstants.DateTimeFormatStringByDate));
            weekDates.Add(week.Sunday.ToString(FormatConstants.DateTimeFormatStringByDate));
            return weekDates;
        }
        /// <summary>
        /// 获取本周的数据
        /// </summary>
        /// <param name="dt">当前日期</param>
        /// <returns></returns>
        public static Week GetCurrentWeek(DateTime dt)
        {
            var strDT = dt.DayOfWeek.ToString();
            Week currentWeek = new Week();
            switch (strDT)
            {
                case "Monday":
                    currentWeek.Monday = dt;
                    break;
                case "Tuesday":
                    currentWeek.Monday = dt.AddDays(-1);
                    break;
                case "Wednesday":
                    currentWeek.Monday = dt.AddDays(-2);
                    break;
                case "Thursday":
                    currentWeek.Monday = dt.AddDays(-3);
                    break;
                case "Friday":
                    currentWeek.Monday = dt.AddDays(-4);
                    break;
                case "Saturday":
                    currentWeek.Monday = dt.AddDays(-5);
                    break;
                case "Sunday":
                    currentWeek.Monday = dt.AddDays(-6);
                    break;
            }
            currentWeek.Tuesday = currentWeek.Monday.AddDays(1);
            currentWeek.Wednesday = currentWeek.Monday.AddDays(2);
            currentWeek.Thursday = currentWeek.Monday.AddDays(3);
            currentWeek.Friday = currentWeek.Monday.AddDays(4);
            currentWeek.Saturday = currentWeek.Monday.AddDays(5);
            currentWeek.Sunday = currentWeek.Monday.AddDays(6);

            return currentWeek;
        }

    }
}
