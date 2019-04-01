using PDMS.Common.Constants;
using PDMS.Data;
using PDMS.Model.ViewModels.Batch;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch.FixtureProcess
{
    public class FixtureData
    {
        public void ExecToDoOne()
        {

            #region 注释，因数据量太多，采用存储过程
            //using (var context = new SPPContext())
            //{
            //    var sql = GetBatchSql();
            //    var list = context.Database.SqlQuery<FixtureBatchVM>(sql).ToList();

            //    List<Fixture_Maintenance_Record> recordList = new List<Fixture_Maintenance_Record>();


            //    var groupList = list.GroupBy(m => new { m.Plant_Organization_UID, m.BG_Organization_UID, m.FunPlant_Organization_UID,
            //    m.Cycle_ID, m.Cycle_Interval, m.Cycle_Unit });

            //    int i = 1;
            //    foreach (var groupItem in groupList)
            //    {
            //        var seq = i.ToString().PadLeft(3, '0');
            //        var seqNo = string.Format("MP{0}_{1}_{2}", DateTime.Now.ToString("yyyyMMdd"), seq, groupItem.Key.Cycle_ID + groupItem.Key.Cycle_Interval + groupItem.Key.Cycle_Unit);
            //        var groupItemList = groupItem.ToList();
            //        foreach (var item in groupItemList)
            //        {
            //            Fixture_Maintenance_Record recordItem = new Fixture_Maintenance_Record();
            //            recordItem.Fixture_Maintenance_Profile_UID = item.Fixture_Maintenance_Profile_UID;
            //            recordItem.Maintenance_Record_NO = seqNo;
            //            recordItem.Fixture_M_UID = item.Fixture_M_UID;
            //            recordItem.Maintenance_Status = 0;
            //            recordItem.Created_UID = ConstConstants.AdminUID;
            //            recordItem.Created_Date = DateTime.Now;
            //            recordItem.Modified_UID = ConstConstants.AdminUID;
            //            recordItem.Modified_Date = DateTime.Now;
            //            recordList.Add(recordItem);
            //        }
            //        i++;
            //    }

            //    context.Fixture_Maintenance_Record.AddRange(recordList);

            //    //更新Maintenance_Plan表信息
            //    var planUID = list.Select(m => m.Maintenance_Plan_UID).Distinct().ToList();
            //    var planList = context.Maintenance_Plan.Where(m => planUID.Contains(m.Maintenance_Plan_UID)).ToList();
            //    var nextDate = DateTime.Now;
            //    foreach (var planItem in planList)
            //    {
            //        switch (planItem.Cycle_Unit)
            //        {
            //            case "W": //planItem.Next_Execution_Date.Value这个日期是本次执行的日期，后面要改写
            //                nextDate = planItem.Next_Execution_Date.Value.AddDays(7 * planItem.Cycle_Interval);
            //                break;
            //            case "M": //planItem.Next_Execution_Date.Value这个日期是本次执行的日期，后面要改写
            //                nextDate = planItem.Next_Execution_Date.Value.AddMonths(planItem.Cycle_Interval);
            //                break;
            //            case "H": //planItem.Next_Execution_Date.Value这个日期是本次执行的日期，后面要改写
            //                nextDate = planItem.Next_Execution_Date.Value.AddHours(planItem.Cycle_Interval);
            //                break;
            //        }
            //        planItem.Last_Execution_Date = DateTime.Now;
            //        planItem.Next_Execution_Date = nextDate;
            //        planItem.Modified_UID = ConstConstants.AdminUID;
            //        planItem.Modified_Date = DateTime.Now;
            //    }
            //    context.SaveChanges();
            //}
            #endregion
            using (var context = new SPPContext())
            {
                context.Database.ExecuteSqlCommand("Fixture_Batch_Record");
            }
        }

        private string GetBatchSql()
        {
            string sql = @"
                        ;WITH 
                        one AS
                        (
                        SELECT A.Maintenance_Plan_UID,B.Fixture_Maintenance_Profile_UID,C.Fixture_M_UID,A.Plant_Organization_UID,A.BG_Organization_UID,A.FunPlant_Organization_UID,
                        A.Maintenance_Type,A.Cycle_ID,A.Cycle_Interval,A.Cycle_Unit,A.Lead_Time,A.Start_Date,A.Tolerance_Time,A.Last_Execution_Date,
                        A.Next_Execution_Date,A.Is_Enable,B.Fixture_NO,C.Version,
                        CASE A.Cycle_Unit 
                        WHEN 'W' 
                        THEN DATEADD(DD,A.Cycle_Interval * 7, ISNULL(DATEADD(DD,A.Lead_Time,A.Last_Execution_Date), A.Start_Date))
                        WHEN 'M'
                        THEN DATEADD(MM,A.Cycle_Interval, ISNULL(DATEADD(DD,A.Lead_Time,A.Last_Execution_Date), A.Start_Date))
                        WHEN 'H'
                        THEN DATEADD(hh,A.Cycle_Interval, ISNULL(DATEADD(DD,A.Lead_Time,A.Last_Execution_Date), A.Start_Date))
                        ELSE A.Start_Date END AS EndDate 
                        FROM dbo.Maintenance_Plan A
                        JOIN dbo.Fixture_Maintenance_Profile B
                        ON A.Maintenance_Plan_UID = B.Maintenance_Plan_UID
                        JOIN dbo.Fixture_M C
                        ON B.Fixture_NO = C.Fixture_NO
                        WHERE A.Maintenance_Type = 'P' 
                        AND A.Is_Enable = 1 AND B.Is_Enable = 1
                        AND C.Status = 550
                        --AND CONVERT(CHAR(10),A.Next_Execution_Date,120) = CONVERT(CHAR(10),GETDATE(),120)
                        AND CONVERT(CHAR(10),A.Next_Execution_Date,120) = CONVERT(CHAR(10),'2017-11-20',120)
                        )
                        SELECT * FROM one";
            return sql;
        }
    }
}
