using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using System.Data.Entity.SqlServer;

namespace PDMS.Data.Repository
{
    public interface IStorageOutboundMRepository : IRepository<Storage_Outbound_M>
    {
        List<StorageOutboundDTO> GetByUid(int Storage_Outbound_M_UID);
    }
    public class StorageOutboundMRepository : RepositoryBase<Storage_Outbound_M>, IStorageOutboundMRepository
    {
        public StorageOutboundMRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<StorageOutboundDTO> GetByUid(int Storage_Outbound_M_UID)
        {
            string sql = @"select M.*, t1.Repair_id,t1.Apply_Time,t1.Repair_Reason,t2.EQP_Location,t2.Equipment,t6.OP_Types,t6.FunPlant,
                                    t3.Enum_Value Storage_Outbound_Type,t4.User_Name Outbound_Account,t5.Enum_Value Status,
                                    storage_outbound_ID Storage_Bound_ID
                                    from Storage_Outbound_M M left join EQPRepair_Info t1
                                    on M.Repair_Uid=t1.Repair_Uid left join Equipment_Info t2
                                    on t1.EQP_Uid=t2.EQP_Uid inner join Enumeration t3 
                                    on M.Storage_Outbound_Type_UID=t3.Enum_Uid inner join EQP_UserTable t4
                                    on M.Outbound_Account_UID=t4.EQPUser_Uid inner join enumeration t5
                                    on M.Status_UID=t5.Enum_UID  
                                    left join System_Function_Plant t6 on t2.System_FunPlant_UID=t6.System_FunPlant_UID 
                                    where M.Storage_Outbound_M_UID={0}";
            sql = string.Format(sql, Storage_Outbound_M_UID);
            var dblist = DataContext.Database.SqlQuery<StorageOutboundDTO>(sql).ToList();
            return dblist;
        }
    }
}
