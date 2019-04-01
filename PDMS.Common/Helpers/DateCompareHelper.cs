using System;
using PDMS.Common.Constants;

namespace PDMS.Common.Helpers
{
    public class DateCompareHelper
    {
        #region 公用方法对比时间
        public static string CompareInterval(DateCompareModel head, DateCompareModel sub)
        {
            var headBeginDate = head.BeginDate.ToString(FormatConstants.DateTimeFormatStringByDate);
            var subBeginDate = sub.BeginDate.ToString(FormatConstants.DateTimeFormatStringByDate);

            //比较begin date
            //一级 begindate after 二级begindate, fail
            if (head.BeginDate > sub.BeginDate)
            {
                return string.Format("{0} Begin Date [{1}] should after {2} Begin Date [{3}]", sub.Name, subBeginDate, head.Name, headBeginDate);
            }

            //一级 enddate is null，不需要比较二级enddate, always pass
            //一级 enddate is not null, 二级enddate is null, fail
            //一级 enddate before 二级enddate, fail
            if (head.EndDate != null)
            {
                var headEndDate = ((DateTime)head.EndDate).ToString(FormatConstants.DateTimeFormatStringByDate);
                if (sub.EndDate == null)
                {
                    return string.Format("{0} End Date have set, please enter correct End Date", head.Name);
                }
                else
                {
                    var subEndDate = ((DateTime)sub.EndDate).ToString(FormatConstants.DateTimeFormatStringByDate);
                    if (head.EndDate < sub.EndDate)
                    {
                        return string.Format("{0} End Date [{1}] should before {2} End Date [{3}]", sub.Name, subEndDate, head.Name, headEndDate);
                    }
                }
            }
            return "PASS";
        }
        #region 获取当前时段
        public static string GetNowTimeInterval()
        {
            DateTime dt = DateTime.Now;
            var h = dt.Hour;
            var m = dt.Minute;
            string NowTimeInterval = null;
            if ((h == 0 & m > 30) || h == 1 || (h == 2 & m <= 30))
            {
                NowTimeInterval = "00:30-02:30";
            }
            if ((h == 2 & m > 30) || h == 3 || (h == 4 & m <= 30))
            {
                NowTimeInterval = "02:30-04:30";
            }
            if ((h == 4 & m > 30) || h == 5 || (h == 6 & m <= 30))
            {
                NowTimeInterval = "04:30-06:30";
            }
            if ((h == 6 & m > 30) || h == 7 || (h == 8 & m <= 30))
            {
                NowTimeInterval = "06:30-08:30";
            }
            if ((h == 8 & m > 30) || h == 9 || (h == 10 & m <= 30))
            {
                NowTimeInterval = "08:30-10:30";
            }
            if ((h == 10 & m > 30) || h == 11 || (h == 12 & m <= 30))
            {
                NowTimeInterval = "10:30-12:30";
            }
            if ((h == 12 & m > 30) || h == 13 || (h == 14 & m <= 30))
            {
                NowTimeInterval = "12:30-14:30";
            }
            if ((h == 14 & m > 30) || h == 15 || (h == 16 & m <= 30))
            {
                NowTimeInterval = "14:30-16:30";
            }
            if ((h == 16 & m > 30) || h == 17 || (h == 18 & m <= 30))
            {
                NowTimeInterval = "16:30-18:30";
            }
            if ((h == 18 & m > 30) || h == 19 || (h == 20 & m <= 30))
            {
                NowTimeInterval = "18:30-20:30";
            }
            if ((h == 20 & m > 30) || h == 21 || (h == 22 & m <= 30))
            {
                NowTimeInterval = "20:30-22:30";
            }
            if ((h == 22 & m > 30) || h == 23 || (h == 0 & m <= 30))
            {
                NowTimeInterval = "22:30-00:30";
            }
            return NowTimeInterval;

        }
        public static int GetNowTimeIntervalNo()
        {
            DateTime dt = DateTime.Now;
            var h = dt.Hour;
            var m = dt.Minute;
            int NowTimeIntervalNo = 0;
            if ((h == 0 & m > 30) || h == 1 || (h == 2 & m <= 30))
            {
                NowTimeIntervalNo = 9;
            }
            if ((h == 2 & m > 30) || h == 3 || (h == 4 & m <= 30))
            {
                NowTimeIntervalNo = 10;
            }
            if ((h == 4 & m > 30) || h == 5 || (h == 6 & m <= 30))
            {
                NowTimeIntervalNo = 11;
            }
            if ((h == 6 & m > 30) || h == 7 || (h == 8 & m <= 30))
            {
                NowTimeIntervalNo = 12;
            }
            if ((h == 8 & m > 30) || h == 9 || (h == 10 & m <= 30))
            {
                NowTimeIntervalNo = 1;
            }
            if ((h == 10 & m > 30) || h == 11 || (h == 12 & m <= 30))
            {
                NowTimeIntervalNo = 2;
            }
            if ((h == 12 & m > 30) || h == 13 || (h == 14 & m <= 30))
            {
                NowTimeIntervalNo = 3;
            }
            if ((h == 14 & m > 30) || h == 15 || (h == 16 & m <= 30))
            {
                NowTimeIntervalNo = 4;
            }
            if ((h == 16 & m > 30) || h == 17 || (h == 18 & m <= 30))
            {
                NowTimeIntervalNo = 5;
            }
            if ((h == 18 & m > 30) || h == 19 || (h == 20 & m <= 30))
            {
                NowTimeIntervalNo = 6;
            }
            if ((h == 20 & m > 30) || h == 21 || (h == 22 & m <= 30))
            {
                NowTimeIntervalNo = 7;
            }
            if ((h == 22 & m > 30) || h == 23 || (h == 0 & m <= 30))
            {
                NowTimeIntervalNo = 8;
            }
            return NowTimeIntervalNo;

        }
        #endregion

        #endregion

        /// <summary>
        /// 判断当前日期是在这个月的第几周
        /// </summary>
        /// <param name="day"></param>
        /// <param name="WeekStart"></param>
        /// <returns></returns>
        public static int WeekOfMonth(DateTime day, int WeekStart)
         {
             //WeekStart
             //1表示 周一至周日 为一周
            //2表示 周日至周六 为一周
             DateTime FirstofMonth;
             FirstofMonth = Convert.ToDateTime(day.Date.Year + "-" + day.Date.Month + "-" + 1);
             
            int i = (int)FirstofMonth.Date.DayOfWeek;
             if (i == 0)
             {
                 i = 7;
             }
 
             if (WeekStart == 1)
             {
                 return (day.Date.Day + i - 2) / 7 + 1;
             }
             if (WeekStart == 2)
             {
                 return (day.Date.Day + i - 1) / 7 ;
 
             }
             return 0;
             //错误返回值0
         }
    }

    public class DateCompareModel
    {
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
