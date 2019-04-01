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
    public interface IStorageOutboundDRepository : IRepository<Storage_Outbound_D>
    {
        List<OutBoundDetail> GetByMUid(int Storage_Outbound_M_UID);
        List<OutBoundInfoDTO> GetOutBoundInfoDTOALL();
        List<StorageOutboundDDTO> GetThreeMonthOut(int Material_Uid);
        List<RepairOutboundDTO> GetRepairOutboundQty(int Repair_Uid, int Storage_Outbound_M_UID);
    }
    public class StorageOutboundDRepository : RepositoryBase<Storage_Outbound_D>, IStorageOutboundDRepository
    {
        public StorageOutboundDRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public List<OutBoundDetail> GetByMUid(int Storage_Outbound_M_UID)
        {
            string sql = string.Format(@"with one as (
                                    select t1.Material_Uid,Material_Id+'_'+Material_Name+'_'+Material_Types mateial,
                                    Warehouse_ID+'_'+Rack_ID+'_'+Storage_ID WarehouseStorage,T1.Warehouse_Storage_UID,Outbound_Qty,
                                    isnull(T5.Inventory_Qty,0) Inventory_Qty,t1.Storage_Outbound_M_UID,PartType_UID,
                                    t6.Enum_Value PartType from Storage_Outbound_D t1 inner join Material_Info t2
                                    on t1.Material_Uid=t2.Material_Uid inner join Warehouse_Storage T3
                                    ON T1.Warehouse_Storage_UID=T3.Warehouse_Storage_UID INNER JOIN Warehouse T4
                                    ON T3.Warehouse_UID=T4.Warehouse_UID left join Material_Inventory t5
                                    ON T1.Material_Uid=T5.Material_Uid AND T1.Warehouse_Storage_UID=
                                    T5.Warehouse_Storage_UID INNER JOIN Enumeration T6 ON T1.PartType_UID=T6.Enum_UID),
                                    two as (
                                    select t2.Material_Uid,t2.Warehouse_Storage_UID,sum(t2.Outbound_Qty) Be_Out_Qty1 from
                                    Storage_Outbound_M t1 inner join Storage_Outbound_D t2
                                    on t1.Storage_Outbound_M_UID=t2.Storage_Outbound_M_UID WHERE Status_UID=407
                                    group by Material_Uid,Warehouse_Storage_UID ) 
                                    select *,ISNULL(Be_Out_Qty1,0) Be_Out_Qty from one left join two on 
                                    one.Material_Uid=two.Material_Uid and one.Warehouse_Storage_UID=two.Warehouse_Storage_UID
                                    where storage_Outbound_M_UID={0}", Storage_Outbound_M_UID);
            //fky2017/11/13
            //on t1.Storage_Outbound_M_UID = t2.Storage_Outbound_M_UID WHERE Status_UID = 374
            var dblist = DataContext.Database.SqlQuery<OutBoundDetail>(sql).ToList();
            return dblist;
        }

        public  List<OutBoundInfoDTO> GetOutBoundInfoDTOALL()
        {

            string sql = string.Format(@"with one as (select t1.Material_Uid,
                                        Material_Id,
                                        Material_Name,
                                        Material_Types,
                                        Warehouse_ID,
                                        Rack_ID,
                                        Storage_ID,
                                        T1.Warehouse_Storage_UID,
                                        Outbound_Qty,
                                        isnull(T5.Inventory_Qty, 0) Inventory_Qty,
                                        t1.Storage_Outbound_M_UID,
                                        t6.Enum_Value PartType
                                        FROM Storage_Outbound_D t1 inner join Material_Info t2
                                    on t1.Material_Uid = t2.Material_Uid inner join Warehouse_Storage T3
                                    ON T1.Warehouse_Storage_UID = T3.Warehouse_Storage_UID INNER JOIN Warehouse T4
                                    ON T3.Warehouse_UID = T4.Warehouse_UID left join Material_Inventory t5
                                    ON T1.Material_Uid = T5.Material_Uid AND T1.Warehouse_Storage_UID =
                                      T5.Warehouse_Storage_UID INNER JOIN Enumeration T6 ON T1.PartType_UID = T6.Enum_UID),
                                    two as (
                                    select t2.Material_Uid,t2.Warehouse_Storage_UID,sum(t2.Outbound_Qty) Be_Out_Qty1 from
                                    Storage_Outbound_M t1 inner join Storage_Outbound_D t2
                                    on t1.Storage_Outbound_M_UID = t2.Storage_Outbound_M_UID WHERE Status_UID = 407
                                    group by Material_Uid,Warehouse_Storage_UID ) 
                                    select *,ISNULL(Be_Out_Qty1, 0) Be_Out_Qty from one left join two on
                                    one.Material_Uid = two.Material_Uid and one.Warehouse_Storage_UID = two.Warehouse_Storage_UID 
");
            var dblist = DataContext.Database.SqlQuery<OutBoundInfoDTO>(sql).ToList();
            return dblist;
        }
        public List<StorageOutboundDDTO> GetThreeMonthOut(int Material_Uid)
        {
            var sql = @"select t1.* from storage_outbound_D t1 inner join Storage_Outbound_M t2
                                    on t1.Storage_Outbound_M_UID=t2.Storage_Outbound_M_UID where
                                    Status_UID=408 AND Material_Uid={0} and Approver_Date BETWEEN 
                                    convert(datetime,Convert(nvarchar(10),DATEADD (MONTH,-3,GETDATE()),111))
                                    AND Convert(datetime,convert(nvarchar(10),GETDATE(),111))";
            //fky2017/11/13
            //  Status_UID=376 AND Material_Uid={0} and Approver_Date BETWEEN 
            sql = string.Format(sql, Material_Uid);
            var dblist = DataContext.Database.SqlQuery<StorageOutboundDDTO>(sql).ToList();
            return dblist;
        }

        public List<RepairOutboundDTO> GetRepairOutboundQty(int Repair_Uid, int Storage_Outbound_M_UID)
        {
            var sql = @"select t1.Repair_Uid,t2.Material_Uid,t2.Update_No,t3.Material_Id,isnull(t7.Sum_Outbound_Qty,0) Sum_Outbound_Qty
                        from EQPRepair_Info t1
                        join Meterial_UpdateInfo t2 on t1.Repair_Uid = t2.Repair_Uid 
                        join Material_Info t3 on t2.Material_Uid = t3.Material_Uid
                        left join (select t5.Repair_Uid,t6.Material_UID,sum(t6.Outbound_Qty) Sum_Outbound_Qty
		                           from Storage_Outbound_M t5 
		                           left join Storage_Outbound_D t6 on t5.Storage_Outbound_M_UID=t6.Storage_Outbound_M_UID
		                           where t5.Status_UID != 420 and t5.Storage_Outbound_M_UID != {1}
		                           group by t5.Repair_Uid,t6.Material_UID) t7 on t1.Repair_Uid = t7.Repair_Uid and t2.Material_Uid = t7.Material_UID
                        where t1.Repair_Uid = {0}";
            sql = string.Format(sql, Repair_Uid, Storage_Outbound_M_UID);
            var dblist = DataContext.Database.SqlQuery<RepairOutboundDTO>(sql).ToList();
            return dblist;
        }
    }
}
