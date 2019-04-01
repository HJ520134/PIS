using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Common.Constants
{
    public class StructConstants
    {
        public struct ReportStatus
        {
            public const int AllKey = 0;
            public const string AllValue = "All";
        }

        public struct IsClosedStatus
        {
            public const int AllKey = 0;
            public const string AllValue = "全部";

            public const int ClosedKey = 1;
            public const string ClosedValue = "关闭";

            public const int ProcessKey = 2;
            public const string ProcessValue = "进行中";

            public const int ApproveKey = 3;
            public const string ApproveValue = "未生效";
        }

        public struct IsLastestStatus
        {
            public const int AllKey = 0;
            public const string AllValue = "全部";

            public const int LastestKey = 1;
            public const string LastestValue = "最新版";

        }

        public struct ViewColumnMapping
        {
            public const string ProductReportDisplay = "productreportdisplay";
        }

        public struct ReworkFlag
        {
            public const string Rework = "Rework";
            public const string Repair = "Repair"; 
        }

        public struct ReworkType
        {
            public const string Input = "Input";
            public const string Output = "Output";
        }

        public struct IsQAProcessType
        {
            public const string InspectKey = "Inspect_IPQC";
            public const string PollingKey = "Polling_IPQC";
            public const string InspectOQCKey = "Inspect_OQC";
            public const string InspectAssembleKey = "Inspect_Assemble";
            public const string AssembleOQCKey = "Inspect_Assemble,Inspect_OQC";

            public const string InspectText = "IPQC全检";
            public const string PollingText = "IPQC巡检";
            public const string InspectOQCText = "OQC检测";
            public const string InspectAssembleText = "组装检测";
            public const string AssembleOQCText = "组装&OQC检测";
        }

        public struct Site
        {
            public const string CTU = "CTU";
            public const string WUXI_M = "WUXI_M";
        }

        public struct Phase
        {
            public const string MP = "MP";
            public const string NPI = "NPI";
        }

        public struct CurrentDepartent
        {
            public const string ME = "ME";
            public const string IE = "IE";
            public const string PP = "PP";

        }

        public struct PlanType
        {
            public const string LocPlan = "Lock计划";
            public const int LocPlanValue = 1;
            public const string WeekPlan = "四周生产计划";
            public const int WeekPlanValue = 2;
        }

        public struct DatePlan
        {
            public const string Month = "月计划";
            public const int MonthValue = 1;
            public const string Week = "周计划";
            public const int WeekValue = 2;
            public const string Day = "日计划";
            public const int DayValue = 3;
        }

        public struct BatchFixtureRequest
        {
            public const string fixtureToDoOne = "fixtureToDoOne";
            public const string fixtureToDoTwo = "fixtureToDoTwo";
        }

        public struct BatchFixtureOverTimeRequest
        {
            public const string OverTimeToDoOne = "OverTimeToDoOne";
            public const string OverTimeToDoTwo = "OverTimeToDoTwo";

        }

        public struct BatchRemindForStatusRequest
        {
            public const string StatusToDoOne = "StatusToDoOne";
            public const string StatusToDoTwo = "StatusToDoTwo";
        }

        public struct FixtureStatus
        {
            public const int StatusOne = 550;
            public const int StatusTwo = 551;
            public const int StatusThree = 552;
            public const int StatusFour = 553;
            public const int StatusFive = 554;
            public const int StatusSix = 555;

            public const string Fixture_Status = "Fixture_Status";
        }

        public struct SystemLanguageUID
        {
            public const int EnglishKey = 1;
            public const int Chinese_CN = 2;
            public const int Chinese_TW = 3;
        }

        public struct BatchFixtureMaintenanceRequest
        {
            public const string FixtureMaintenanceToDoOne = "FixtureMaintenanceToDoOne";
            public const string FixtureMaintenanceToDoTwo = "FixtureMaintenanceToDoTwo";

        }

        /// <summary>
        /// 每个模组的大模块，用于排程Task任务
        /// </summary>
        public struct BatchModuleName
        {
            //邮件发送模组
            public const string Email_Module = "Email_Module";

            //治具模组提醒
            public const string Fixture_Module = "Fixture_Module";

            //FMT排程
            public const string FMTDashboard_Module = "FMT Dashboard排程";

            //FMT排程-按周
            public const string FMTDashboard_Week_Module = "FMT Dashboard排程-按周";

            //FMT排程-按月
            public const string FMTDashboard_Month_Module = "FMT Dashboard排程-按月";

            //邮件发送模组FunctionName
            public const string EmailFunctionName = "Send Mail Batch";
        }

        public struct BatchExecList
        {
            public const string Task_FMTDashboard = "Task_FMTDashboard";
        }

        public struct BatchLog
        {
            public const string Email_Module_Success = "邮件排程模组执行成功";
            public const string Email_Module_Failed = "邮件排程模组执行失败";

        }

        public struct Log_Type
        {
            public const string PIS = "PIS";
            public const string SCHEDULE = "SCHEDULE";

        }

        //日志文件路径,注意排程之间不能用同一个日志不然同时出错往同一个日志写的时候另一个会出错
        public struct Log_Path
        {
            public const string BatchEmalLog = "PIS_log\\BatchEmalLog";
            public const string BatchFMTDashBoardLog = "PIS_log\\BatchFMTDashBoardLog";
        }

        public struct Email_From
        {
            public const string PIS_Email_From = "PIS_System,do-not-reply@jabil.com";
        }


    }
}
