using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Common;

namespace PDMS.Web.Business.Flowchart
{
    public static class FlowchartImportCommon
    {
        public static string[] GetHeadColumn()
        {
            var propertiesHead = new[]
            {
                "客户",
                "专案名称",
                "部件",
                "阶段"
            };
            return propertiesHead;
        }

        public static string[] GetContentColumn()
        {
            var propertiesContent = new[]
                {
                    "绑定序号",
                    "制程序号",
                    "DRI",
                    "场地",
                    "是否分楼栋",
                    "工站名稱",
                    "厂别",
                    "阶层",
                    "颜色",
                    "工站說明",
                    "检测设定",
                    "返工设定",
                    "对应修复站点",
                    "WIP",
                    "不可用WIP",
                    "当前库存数",
                    "数据来源",
                    "是否同步"
                };
            return propertiesContent;
        }

        public static string[] GetEtransferContentColumn()
        {
            var propertiesContent = new[]
                {
                    "绑定序号",
                    "制程序号",
                    "DRI",
                    "场地",
                    "工站名稱",
                    "厂别",
                    "阶层",
                    "颜色",
                    "Edition",
                    "工站說明",
                    "返工设定",
                    "检测设定",
                    "FromWHS",
                    "ToWHSOK",
                    "ToWHSNG"
                };
            return propertiesContent;
        }

        public static string[] GetMEContentColumn()
        {
            var propertiesContent = new[]
                {
                    "制程序号",
                    "工站",
                    "厂别",
                    "工序",
                    "工序说明",
                    "颜色",
                    "加工设备",
                    "自动化设备",
                    "加工治具",
                    "辅助设备",
                    "设备 Cycle Time(s)",
                    "装夹时间",
                    "Cycle Time(s)",
                    "预估良率(%)",
                    "人力配比",
                    "产能(/1H)",
                    "产能",
                    "设备需求",
                    "人力*2班"
                };
            return propertiesContent;
        }

        public static string[] GetMEEquipmentColumn()
        {
            var propertiesContent = new[]
                {
                    "制程序号",
                    "制程",
                    "工站",
                    "设备名称",
                    "设备规格",
                    "Cycle Time(s)",
                    "产能",
                    "设备需求",
                    "配比",
                    "需求数量",
                    "设备变动数量",
                    "NPI阶段现有数量",
                    "MP阶段现有数量",
                    "备注",
                };
            return propertiesContent;
        }

        public static string[] GetMEAutoEquipmentColumn()
        {
            var propertiesContent = new[]
                {
                    "",
                    "制程序号",
                    "制程",
                    "工站",
                    "設備名稱",
                    "主設備需求",
                    "备用",
                    "需求數量",
                };
            return propertiesContent;
        }

        public static string[] GetNPIHeadColumn()
        {
            var propertiesHead = new[]
            {
                "专案名称",
                "版本",
                "阶段",
                "日期",
                "投入数",
                "FlowChart_Master_UID"
            };
            return propertiesHead;
        }

        public static string[] GetMPHeadColumn()
        {
            var propertiesHead = new[]
            {
                //"制程序号",
                //"制程名称",
                //"战情制程名称",
                //"颜色",
                //"Flowchart_Detail_UID",
                "专案名称",
                "版本",
                "阶段",
                "计划类型",
                "日期",
                "投入数"
                //"良率"
            };
            return propertiesHead;
        }

        public static string[] GetManPowerHeadColumn()
        {
            var propertiesHead = new[]
            {
                "制程序号",
                "功能厂",
                "工序（制程）",
                "Flowchart_Detail_UID",
                "Flowchart_Detail_ME_UID",
                "FatherUID",
                "ChildUID",
                "阶段",
                "生产日期",
                "OP人力",
                "班长",
                "技术员",
                "物料员",
                "其他人力"
            };
            return propertiesHead;
        }


        public static string[] GetEquipHeadColumn()
        {
            var propertiesHead = new[]
            {
                "制程序号",
                "功能厂",
                "工序（制程）",
                "设备名称",
                "FlowChart_Detail_UID",
                "Flowchart_Detail_ME_UID",
                "Flowchart_Detail_ME_Equipment_UID",
                "FatherUID",
                "ChildUID",
                "阶段",
                "生产日期",
                "设备数量",
            };
            return propertiesHead;
        }

        public static string[] GetLanguageHeadColumn()
        {
            var propertiesHead = new[]
            {
                "资源名称",
                "语言名称",
                "System_LocaleStringResource_UID",
                "System_Language_UID",
            };
            return propertiesHead;
        }

        public static string[] GetFlowchartDetailIEColumn()
        {
            var propertiesHead = new[]
            {
                "工序",
                "功能厂",
                "制程",
                "总CT时间(/秒)",
                "人机配比",
                "目标良率",
                "1小时产能(/台)",
                "1天产能(/台)",
                "设备需求",
                "设备需求(变动数)",
                "固定OP",
                "OP变动数",
                "班长(配比规则)",
                "班长(配比比例)",
                "班长(变动数)",
                "技术员(配比)",
                "技术员(变动数)",
                "物料员",
                "其他",
                "备注"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureResumeHeadColumn()
        {
            var propertiesHead = new[]
            {
                "序号",
                "表单编号",
                "厂区",
                "OP类型",
                "功能厂",
                "制程",
                "工站",
                "线别",
                "设备编号",
                "治具编号",
                "治具短码",
                "备注",
                "更新者",
                "更新日期"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureNotMaintainedHeadColumn()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "制程",
                "工站",
                "线别",
                "治具编号",
                "治具短码",
                "保养类别",
                "日期"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureRepairReportHeadColumnOne()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "治具编号",
                "工站",
                "异常原因代码",
                "异常原因名称",
                "维修数量统计"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureRepairReportHeadColumnTwo()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "治具编号",
                "日期",
                "工站",
                "维修数量统计"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureRepairReportHeadByPersonColumn()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "工站",
                "异常原因代码",
                "异常原因名称",
                "维修人",
                "维修数量统计"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureHeadByPageColumn()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "治具图号",
                "版本号",
                "维修数量统计"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureRepairReportByDetailColumnOne()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "工站",
                "治具唯一编号",
                "维修人",
                "维修数量统计"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureRepairReportByDetailColumnTwo()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "工站",
                "治具唯一编号",
                "异常原因代码",
                "异常原因名称",
                "维修数量统计"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureRepairReportByAnalisisColumn()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "维修间",
                "工站",
                "治具编号",
                "小于30分钟数量",
                "小于2小时数量",
                "小于4小时数量",
                "其他数量"
            };
            return propertiesHead;

        }

        public static string[] GetFixtureHeadByStatusColumn()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "工站",
                "治具编号",
                "治具状态",
                "数量统计"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureHeadByStatusAnalisisColumn()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "状态",
                "治具总数",
                "使用中",
                "保养逾期",
                "未使用",
                "维修中",
                "返供应商RTV",
                "报废",
                "已保养",
                "未保养"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureHeadByFMTColumn()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "制程",
                "当前治具总数",
                "当日新增",
                "当日报废",
                "当日送修数",
                "当日领用数",
                "待修数",
                "应保养数",
                "已保养数",
                "未保养数"
            };
            return propertiesHead;
        }

        public static string[] GetFixtureHeadByFMTDetailColumn()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "制程",
                "工站",
                "当前治具总数",
                "当日新增",
                "当日报废",
                "当日送修数",
                "当日领用数",
                "待修数",
                "应保养数",
                "已保养数",
                "未保养数"
            };
            return propertiesHead;
        }



        public static bool ValidIsInt(string result, bool isEdit)
        {
            var validResult = false;
            //if (isEdit)
            //{
            int validInt = 0;
            var splitList = result.Split('_').ToList();
            var isInt =  int.TryParse(splitList.First(), out validInt);
            if (isInt)
            {
                validResult = true;
            }
            //}
            //else
            //{
            //    int validInt = 0;
            //    var isInt = int.TryParse(result, out validInt);
            //    if (isInt)
            //    {
            //        validResult = true;
            //    }
            //}
            return validResult;
        }

    }
}