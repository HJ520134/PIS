using PDMS.Common.Constants;
using PDMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch.FixtureMaintenanceProcess
{
    public class FixtureMaintenanceDate
    {
        public void ExecToDoOne()
        {

            try
            {
                var DataContext = new SPPContext();
                List<Fixture_Maintenance_Record> recordList = new List<Fixture_Maintenance_Record>();
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    //1.先把7天以外治具保养表数据找到，然后插入到历史表中，然后再删除7天以外的治具保养表数据，
                    //2.先把7天以外的治具履历表数据找到，然后插入到历史表中，然后再删除7天以外的治具履历表数据。
                    var insertSql = string.Format(@"INSERT INTO   Fixture_Maintenance_Record_History(
                                        Fixture_Maintenance_Record_UID
                                      , Fixture_Maintenance_Profile_UID
                                      , Maintenance_Record_NO
                                      , Fixture_M_UID
                                      , Maintenance_Date
                                      , Maintenance_Status
                                      , Maintenance_Person_Number
                                      , Maintenance_Person_Name
                                      , Confirm_Date
                                      , Confirm_Status
                                      , Confirmor_UID
                                      , Created_UID
                                      , Created_Date
                                      , Modified_UID
                                      , Modified_Date)
                                      SELECT
                                        Fixture_Maintenance_Record_UID
                                      , Fixture_Maintenance_Profile_UID
                                      , Maintenance_Record_NO
                                      , Fixture_M_UID
                                      , Maintenance_Date
                                      , Maintenance_Status
                                      , Maintenance_Person_Number
                                      , Maintenance_Person_Name
                                      , Confirm_Date
                                      , Confirm_Status
                                      , Confirmor_UID
                                      , Created_UID
                                      , Created_Date
                                      , Modified_UID
                                      , Modified_Date FROM Fixture_Maintenance_Record  WHERE Confirm_Status = 1  AND Modified_Date <= N'{0}';
                                 DELETE FROM Fixture_Maintenance_Record  WHERE Confirm_Status = 1  AND Modified_Date <=  N'{0}';
                                 INSERT INTO  Fixture_Resume_History(
                                        Fixture_Resume_UID
                                      , Fixture_M_UID
                                      , Data_Source
                                      , Resume_Date
                                      , Source_UID
                                      , Source_NO
                                      , Resume_Notes
                                      , Modified_UID
                                      , Modified_Date)
                                      SELECT
                                      Fixture_Resume_UID
                                      , Fixture_M_UID
                                      , Data_Source
                                      , Resume_Date
                                      , Source_UID
                                      , Source_NO
                                      , Resume_Notes
                                      , Modified_UID
                                      , Modified_Date FROM Fixture_Resume  WHERE Modified_Date <= N'{0}';
                                 DELETE FROM Fixture_Resume  WHERE Modified_Date <= N'{0}'; ",
                                    DateTime.Now.AddDays(-7).ToString(FormatConstants.DateTimeFormatString)
                             );
                    sb.AppendLine(insertSql);
                    string sql = sb.ToString();
                    DataContext.Database.ExecuteSqlCommand(sb.ToString());
                    trans.Commit();

                }

            }
            catch (Exception e)
            {
                // return e.Message;
            }
        }
    }
}
