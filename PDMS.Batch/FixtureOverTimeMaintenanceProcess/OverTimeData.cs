using PDMS.Common.Constants;
using PDMS.Data;
using PDMS.Model.ViewModels.Batch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch.FixtureOverTimeMaintenanceProcess
{
    public class OverTimeData 
    {
        public void ExecToDoOne()
        {
            //治具逾期提醒By'P'
            using (var context = new SPPContext())
            {
                var sql = GetBatchSql();
                //获取Maintenance_Type = 'P'的超期未保养治具列表
                var list = context.Database.SqlQuery<FixtureBatchVM>(sql).ToList();

                List<int> idList = new List<int>();
                foreach (var item in list)
                {
                    var dateStr = item.Last_Execution_Date.Value.ToString("yyyyMMdd");
                    var uidList = context.Fixture_Maintenance_Record.Where(m => m.Fixture_Maintenance_Profile_UID == item.Fixture_Maintenance_Profile_UID
                    && m.Maintenance_Record_NO.Contains(dateStr) && m.Maintenance_Person_Number == null).Select(m => m.Fixture_M_UID).ToList();
                    idList.AddRange(uidList);
                }

                List<Fixture_Resume> ResumeList = new List<Fixture_Resume>();
                var Fixture_M_List = context.Fixture_M.Where(m => idList.Contains(m.Fixture_M_UID)).ToList();

                int i = 1;
                foreach (var item in Fixture_M_List)
                {
                    item.Status = StructConstants.FixtureStatus.StatusSix;
                    item.Modified_UID = ConstConstants.AdminUID;
                    item.Modified_Date = DateTime.Now;

                    var seq = i.ToString().PadLeft(4, '0');
                    var seqNo = string.Format("OP_{0}_{1}", DateTime.Now.ToString("yyyyMMdd"), seq);

                    ResumeList.Add(new Fixture_Resume {
                        Fixture_M_UID = item.Fixture_M_UID,
                        Data_Source = "6",
                         Resume_Date = item.Modified_Date,
                         Source_UID = item.Fixture_M_UID,
                         Source_NO = seqNo,
                         Resume_Notes = "週期保養逾時",
                         Modified_UID = ConstConstants.AdminUID,
                         Modified_Date = item.Modified_Date
                    });
                    i++;
                }

                context.Fixture_Resume.AddRange(ResumeList);
                context.SaveChanges();

            }

            //治具逾期保养By'D'
            using (var context = new SPPContext())
            {
                var sql = GetBatchSqlByD();
                //获取Maintenance_Type = 'D'的超期未保养治具列表
                var list = context.Database.SqlQuery<FixtureBatchVM>(sql).ToList();

                List<int> idList = new List<int>();
                foreach (var item in list)
                {
                    var dateStr = item.Last_Execution_Date.Value.ToString("yyyyMMdd");
                    var uidList = context.Fixture_Maintenance_Record.Where(m => m.Fixture_Maintenance_Profile_UID == item.Fixture_Maintenance_Profile_UID
                    && m.Maintenance_Record_NO.Contains(dateStr) && m.Maintenance_Person_Number == null).Select(m => m.Fixture_M_UID).ToList();
                    idList.AddRange(uidList);
                }

                List<Fixture_Resume> ResumeList = new List<Fixture_Resume>();
                var Fixture_M_List = context.Fixture_M.Where(m => idList.Contains(m.Fixture_M_UID)).ToList();

                int i = 1;
                foreach (var item in Fixture_M_List)
                {
                    item.Status = StructConstants.FixtureStatus.StatusSix;
                    item.Modified_UID = ConstConstants.AdminUID;
                    item.Modified_Date = DateTime.Now;

                    var seq = i.ToString().PadLeft(4, '0');
                    var seqNo = string.Format("OD_{0}_{1}", DateTime.Now.ToString("yyyyMMdd"), seq);

                    ResumeList.Add(new Fixture_Resume
                    {
                        Fixture_M_UID = item.Fixture_M_UID,
                        Data_Source = "6",
                        Resume_Date = item.Modified_Date,
                        Source_UID = item.Fixture_M_UID,
                        Source_NO = seqNo,
                        Resume_Notes = "週期保養逾時",
                        Modified_UID = ConstConstants.AdminUID,
                        Modified_Date = item.Modified_Date
                    });
                    i++;
                }

                context.Fixture_Resume.AddRange(ResumeList);
                context.SaveChanges();
            }
        }

        private string GetBatchSql()
        {
            string newSql = @"
                            ;WITH 
                            one AS 
                            (
                            SELECT B.Fixture_Maintenance_Profile_UID,A.Last_Execution_Date,A.Tolerance_Time,A.Maintenance_Plan_UID FROM dbo.Maintenance_Plan A
                            JOIN dbo.Fixture_Maintenance_Profile B
                            ON B.Maintenance_Plan_UID = A.Maintenance_Plan_UID
                            WHERE A.Maintenance_Type = 'P' 
                            AND A.Is_Enable = 1 AND B.Is_Enable = 1 
                            --A.Tolerance_Time + 1代表超过保养时间
                            AND CONVERT(DATE,DATEADD(dd,A.Tolerance_Time + 1,A.Last_Execution_Date),120) = CONVERT(CHAR(10),GETDATE(),120)
                            )
                            SELECT * FROM one";

            return newSql;
        }

        private string GetBatchSqlByD()
        {
            string newSql = @"
                            ;WITH 
                            one AS 
                            (
                            SELECT B.Fixture_Maintenance_Profile_UID,A.Last_Execution_Date,A.Tolerance_Time,A.Maintenance_Plan_UID FROM dbo.Maintenance_Plan A
                            JOIN dbo.Fixture_Maintenance_Profile B
                            ON B.Maintenance_Plan_UID = A.Maintenance_Plan_UID
                            WHERE A.Maintenance_Type = 'D' 
                            AND A.Is_Enable = 1 AND B.Is_Enable = 1 
                            --A.Tolerance_Time + 1代表超过保养时间
                            AND CONVERT(DATE,DATEADD(dd,A.Tolerance_Time + 1,A.Last_Execution_Date),120) = CONVERT(CHAR(10),GETDATE(),120)
                            )
                            SELECT * FROM one";

            return newSql;

        }
    }
}
