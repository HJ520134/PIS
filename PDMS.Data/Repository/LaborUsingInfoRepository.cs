using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;

namespace PDMS.Data.Repository
{
    public interface ILaborUsingInfoRepository : IRepository<Labor_UsingInfo>
    {
        string DeleteByRepairUid(int Repair_Uid);
        List<laborlist> GetByRepairUid(int Repair_Uid);
    }
    public class LaborUsingInfoRepository : RepositoryBase<Labor_UsingInfo>, ILaborUsingInfoRepository
    {
        public LaborUsingInfoRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public string DeleteByRepairUid(int Repair_Uid)
        {
            try
            {
                string sql = @"delete  dbo.Labor_UsingInfo where Repair_Uid={0} ";
                sql = string.Format(sql, Repair_Uid);
                DataContext.Database.ExecuteSqlCommand(sql);
                return "";
            }
            catch (Exception e)
            {
                return "更新维修人员失败" + e.Message;
            }

        }

        public List<laborlist> GetByRepairUid (int Repair_Uid)
        {
            string sql = @"SELECT t1.Labor_Using_Uid,t1.Repair_Uid,t1.EQPUser_Uid,t2.User_Id,t2.User_Name 
FROM dbo.Labor_UsingInfo t1 inner join EQP_UserTable t2 on t1.EQPUser_Uid=t2.EQPUser_Uid WHERE Repair_Uid={0}"; 
            sql = string.Format(sql, Repair_Uid);
            var dbList = DataContext.Database.SqlQuery<laborlist>(sql).ToList();
            return dbList;
        }
    }
}
