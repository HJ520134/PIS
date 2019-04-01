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
    public interface  IMeterialUpdateInfoRepository : IRepository<Meterial_UpdateInfo>
    {
        List<MeterialUpdateInfoDTO> GetByUId(int Material_Uid);
        List<matlist> GetByRepairUid(int Repair_Uid);
        string DeleteByRepairUid(int Repair_Uid);
        List<MeterialUpdateInfoWithWarehouseDTO> GetMeterialUpdateInfoWithWarehouse(int Repair_Uid);
    }
    public class MeterialUpdateInfoRepository: RepositoryBase<Meterial_UpdateInfo>, IMeterialUpdateInfoRepository
    {
        public MeterialUpdateInfoRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public List<MeterialUpdateInfoDTO> GetByUId(int Material_Uid)
        {
            string sql = "SELECT Material_Uid,Material_Name, Assembly_Name,Unit_Price FROM dbo.Material_Info where Material_Uid={0}";
            sql = string.Format(sql, Material_Uid);
            var dblist = DataContext.Database.SqlQuery<MeterialUpdateInfoDTO>(sql).ToList();
            return dblist;
        }

        public List<matlist> GetByRepairUid(int Repair_Uid)
        {
            string sql = @"SELECT PartsUpdate_Uid,Repair_Uid,t1.Material_Uid,Material_Id,Material_Name,Update_No,
                            Update_Cost,Assembly_Name,Unit_Price,Reason_Analysis FROM dbo.Meterial_UpdateInfo t1
                            INNER JOIN dbo.Material_Info t2 ON t1.Material_Uid=t2.Material_Uid WHERE t1.Repair_Uid={0}";
            sql = string.Format(sql, Repair_Uid);
            var dbList = DataContext.Database.SqlQuery<matlist>(sql).ToList();
            return dbList;
        }

        public string DeleteByRepairUid(int Repair_Uid)
        {
            try
            {
                string sql = @"delete  dbo.Meterial_UpdateInfo where Repair_Uid={0} ";
                sql = string.Format(sql, Repair_Uid);
                DataContext.Database.ExecuteSqlCommand(sql);
                return "";
            }
            catch (Exception e)
            {
                return "更新更换材料失败" + e.Message;
            }

        }

        public List<MeterialUpdateInfoWithWarehouseDTO> GetMeterialUpdateInfoWithWarehouse(int Repair_Uid)
        {
            var query = from u in DataContext.Meterial_UpdateInfo
                        where u.Repair_Uid == Repair_Uid
                        select new MeterialUpdateInfoWithWarehouseDTO
                        {
                            //PartsUpdate_Uid = u.PartsUpdate_Uid,
                            Material_Uid = u.Material_Uid,
                            //Update_DateTime = u.Update_DateTime,
                            Update_No = u.Update_No,
                            //EQP_Uid = u.EQP_Uid,
                            //Update_Cost = u.Update_Cost,
                            Repair_Uid = u.Repair_Uid,
                            //Modified_EQPUser_Uid = u.Modified_EQPUser_Uid,
                            //Reason_Analysis = u.Reason_Analysis,
                            //料号信息
                            Material_Id = u.Material_Info.Material_Id,
                            Material_Name = u.Material_Info.Material_Name,
                            Material_Types = u.Material_Info.Material_Types
                            //儲位信息
                            //Warehouse_Storage_UID = u.Material_Info.Warehouse_Storage_UID,
                            //Warehouse_Type_UID = u.Material_Info.Warehouse_Storage.Warehouse.Warehouse_Type_UID
                        };

            return query.ToList();
        }
    }
}
