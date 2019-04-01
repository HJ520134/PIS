using PDMS.Common.Constants;
using PDMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch
{
    public class ExceptionEmailManager
    {
        public void InsertLogInfo(string flag, bool sendFlag, string errorInfo)
        {
            using (var context = new SPPContext())
            {
                Batch_Log newLog = new Batch_Log();
                newLog.Batch_Date = DateTime.Now;
                if (sendFlag)
                {
                    SetSuccessItem(flag, newLog);
                }
                else
                {
                    SetFailedItem(flag, newLog, errorInfo);
                }
                context.Batch_Log.Add(newLog);
                context.SaveChanges();
            }
        }

        private void SetSuccessItem(string flag, Batch_Log newLog)
        {
            switch (flag)
            {
                case ConstConstants.FixtureMaintenanceBatch:
                    newLog.Batch_Name = ConstConstants.FixtureMaintenanceBatch;
                    newLog.Batch_Status = true;
                    newLog.Batch_Desc = ConstConstants.FixtureMaintenanceBatch_Success;
                    break;
                case ConstConstants.FixtureNotMaintenanceBatch: //邮件发送成功文本跟失败文本不一样
                    newLog.Batch_Name = ConstConstants.FixtureNotMaintenanceBatch;
                    newLog.Batch_Status = true;
                    newLog.Batch_Desc = ConstConstants.FixtureNotMaintenanceBatch_Success;
                    break;
                case ConstConstants.FixtureMaintenance: //邮件发送成功文本跟失败文本不一样
                    newLog.Batch_Name = ConstConstants.FixtureMaintenance;
                    newLog.Batch_Status = true;
                    newLog.Batch_Desc = ConstConstants.FixtureMaintenance_Success;
                    break;
            }
        }

        private void SetFailedItem(string flag, Batch_Log newLog, string errorInfo)
        {
            switch (flag)
            {
                case ConstConstants.FixtureMaintenanceBatch:
                    newLog.Batch_Name = ConstConstants.FixtureMaintenanceBatch;
                    newLog.Batch_Status = false;
                    newLog.Batch_Desc = errorInfo;
                    break;
                case ConstConstants.FixtureNotMaintenanceBatch: //邮件发送成功文本跟失败文本不一样
                    newLog.Batch_Name = ConstConstants.FixtureNotMaintenanceBatch;
                    newLog.Batch_Status = false;
                    newLog.Batch_Desc = errorInfo;
                    break;
                case ConstConstants.FixtureMaintenance: //邮件发送成功文本跟失败文本不一样
                    newLog.Batch_Name = ConstConstants.FixtureMaintenance;
                    newLog.Batch_Status = true;
                    newLog.Batch_Desc = errorInfo;
                    break;
            }
        }
    }
}
