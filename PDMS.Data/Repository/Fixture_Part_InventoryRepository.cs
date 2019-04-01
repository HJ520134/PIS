using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDMS.Model;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Part_InventoryRepository : IRepository<Fixture_Part_Inventory>
    {
        List<Fixture_Part_InventoryDTO> GetWarehouseStorageByFixture_Part_UID(int Fixture_Part_UID);

        List<Fixture_Part_InventoryDTO> GetMatinventory(int Fixture_Part_UID, int Fixture_Warehouse_Storage_UID);

        List<Fixture_Part_InventoryDTO> Fixture_Part_InventoryDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);

        IQueryable<Fixture_Part_InventoryDTO> GetStorageDetialReportInfo(Fixture_Part_InventoryDTO searchModel, Page page, out int totalcount);

        IQueryable<Fixture_Part_InventoryDTO> GetFixtureinventory(int Fixture_Part_UID,
            int Fixture_Warehouse_Storage_UID, Page page);

        List<Fixture_Part_InventoryDTO> ExportAllFixtureInventoryDetialReport(Fixture_Part_InventoryDTO searchModel);
        List<Fixture_Part_InventoryDTO> ExportSelectedFixtureInventoryDetialReport(string uids);

    }
    public class Fixture_Part_InventoryRepository : RepositoryBase<Fixture_Part_Inventory>, IFixture_Part_InventoryRepository
    {
        public Fixture_Part_InventoryRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<Fixture_Part_InventoryDTO> GetWarehouseStorageByFixture_Part_UID(int Fixture_Part_UID)
        {

            var query = from M in DataContext.Fixture_Part_Inventory
                        select new Fixture_Part_InventoryDTO
                        {
                            Fixture_Part_Inventory_UID = M.Fixture_Part_Inventory_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Inventory_Qty = M.Inventory_Qty,
                            Remarks = M.Remarks,
                            Modified_UID = M.Modified_UID,
                            Modified_Date = M.Modified_Date,
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                            User_Name = M.System_Users.User_Name,
                        };
            if (Fixture_Part_UID != 0)
                query = query.Where(m => m.Fixture_Part_UID == Fixture_Part_UID);
            return query.ToList();

        }

        public List<Fixture_Part_InventoryDTO> GetMatinventory(int Fixture_Part_UID, int Fixture_Warehouse_Storage_UID)
        {
            var query = from M in DataContext.Fixture_Part_Inventory
                        select new Fixture_Part_InventoryDTO
                        {
                            Fixture_Part_Inventory_UID = M.Fixture_Part_Inventory_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Inventory_Qty = M.Inventory_Qty,
                            Remarks = M.Remarks,
                            Modified_UID = M.Modified_UID,
                            Modified_Date = M.Modified_Date,
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                        };
            if (Fixture_Part_UID != 0)
                query = query.Where(m => m.Fixture_Part_UID == Fixture_Part_UID);
            if (Fixture_Warehouse_Storage_UID != 0)
                query = query.Where(m => m.Fixture_Warehouse_Storage_UID == Fixture_Warehouse_Storage_UID);
            return query.ToList();
        }

        public List<Fixture_Part_InventoryDTO> Fixture_Part_InventoryDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from M in DataContext.Fixture_Part_Inventory
                        select new Fixture_Part_InventoryDTO
                        {
                            Fixture_Part_Inventory_UID = M.Fixture_Part_Inventory_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Inventory_Qty = M.Inventory_Qty,
                            Remarks = M.Remarks,
                            Modified_UID = M.Modified_UID,
                            Modified_Date = M.Modified_Date,
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,
                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }

        /// <summary>
        /// 获取治具的库存明细数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public IQueryable<Fixture_Part_InventoryDTO> GetStorageDetialReportInfo(Fixture_Part_InventoryDTO searchModel, Page page, out int totalcount)
        {
            var query = from FIXP in DataContext.Fixture_Part_Inventory
                        select new Fixture_Part_InventoryDTO
                        {
                            Fixture_Part_Inventory_UID = FIXP.Fixture_Part_Inventory_UID,
                            Fixture_Warehouse_Storage_UID = FIXP.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_UID = FIXP.Fixture_Part_UID,
                            Inventory_Qty = FIXP.Inventory_Qty,
                            Remarks = FIXP.Remarks,
                            Modified_UID = FIXP.Modified_UID,
                            User_Name = FIXP.System_Users.User_Name,
                            Modified_Date = FIXP.Modified_Date,
                            Plant_Organization_UID = FIXP.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = FIXP.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = FIXP.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = FIXP.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = FIXP.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = FIXP.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = FIXP.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = FIXP.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = FIXP.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = FIXP.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = FIXP.Fixture_Part.Part_ID,
                            Part_Name = FIXP.Fixture_Part.Part_Name,
                            Part_Spec = FIXP.Fixture_Part.Part_Spec
                        };

            //厂区
            if (searchModel.Plant_Organization_UID != 0)
            {
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            }
            
            //BG
            if (searchModel.BG_Organization_UID != 0)
            {
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            }

            //功能厂
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
            {
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            }

            //料号
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
            {
                query = query.Where(m => m.Part_ID.Contains(searchModel.Part_ID));
            }
            //品名
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
            {
                query = query.Where(m => m.Part_Name.Contains(searchModel.Part_Name));
            }
            //型号
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
            {
                query = query.Where(m => m.Part_Spec.Contains(searchModel.Part_Spec));
            }

            //储位
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
            {
                query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));
            }

            //料架
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
            {
                query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
            }

            //仓库编码
            if (!string.IsNullOrEmpty(searchModel.Fixture_Warehouse_ID))
            {
                query = query.Where(m => m.Fixture_Warehouse_ID.Contains(searchModel.Fixture_Warehouse_ID));
            }

            //仓库名字
            if (!string.IsNullOrEmpty(searchModel.Fixture_Warehouse_Name))
            {
                query = query.Where(m => m.Fixture_Warehouse_Name.Contains(searchModel.Fixture_Warehouse_Name));
            }

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;
        }

        public IQueryable<Fixture_Part_InventoryDTO> GetFixtureinventory(int Fixture_Part_UID,
            int Fixture_Warehouse_Storage_UID, Page page)
        {
            var query = from M in DataContext.Fixture_Part_Inventory
                        select new Fixture_Part_InventoryDTO
                        {
                            Fixture_Part_Inventory_UID = M.Fixture_Part_Inventory_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Inventory_Qty = M.Inventory_Qty,
                            Remarks = M.Remarks,
                            Modified_UID = M.Modified_UID,
                            Modified_Date = M.Modified_Date,
                            User_Name = M.System_Users.User_Name,
                            Plant_Organization_UID = M.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = M.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = M.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = M.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = M.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = M.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,//储位
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,//料架
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,//仓库编码
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,//仓库名字
                            Part_ID = M.Fixture_Part.Part_ID,//料号
                            Part_Name = M.Fixture_Part.Part_Name,//品名
                            Part_Spec = M.Fixture_Part.Part_Spec,//型号
                        };
            if (Fixture_Part_UID != 0)
                query = query.Where(m => m.Fixture_Part_UID == Fixture_Part_UID);
            if (Fixture_Warehouse_Storage_UID != 0)
                query = query.Where(m => m.Fixture_Warehouse_Storage_UID == Fixture_Warehouse_Storage_UID);

            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;
        }

        /// <summary>
        /// 导出全部的库存信息
        /// </summary>
        public List<Fixture_Part_InventoryDTO> ExportAllFixtureInventoryDetialReport(Fixture_Part_InventoryDTO searchModel)
        {
            var query = from FIXP in DataContext.Fixture_Part_Inventory
                        select new Fixture_Part_InventoryDTO
                        {
                            Fixture_Part_Inventory_UID = FIXP.Fixture_Part_Inventory_UID,
                            Fixture_Warehouse_Storage_UID = FIXP.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_UID = FIXP.Fixture_Part_UID,
                            Inventory_Qty = FIXP.Inventory_Qty,
                            Remarks = FIXP.Remarks,
                            Modified_UID = FIXP.Modified_UID,
                            User_Name = FIXP.System_Users.User_Name,
                            Modified_Date = FIXP.Modified_Date,
                            Plant_Organization_UID = FIXP.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = FIXP.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = FIXP.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = FIXP.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = FIXP.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = FIXP.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = FIXP.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = FIXP.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = FIXP.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = FIXP.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = FIXP.Fixture_Part.Part_ID,
                            Part_Name = FIXP.Fixture_Part.Part_Name,
                            Part_Spec = FIXP.Fixture_Part.Part_Spec
                        };

            //厂区
            if (searchModel.Plant_Organization_UID != 0)
            {
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            }

            //BG
            if (searchModel.BG_Organization_UID != 0)
            {
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            }

            //功能厂
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
            {
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            }

            //料号
            if (!string.IsNullOrWhiteSpace(searchModel.Part_ID))
            {
                query = query.Where(m => m.Part_ID.Contains(searchModel.Part_ID));
            }
            //品名
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Name))
            {
                query = query.Where(m => m.Part_Name.Contains(searchModel.Part_Name));
            }
            //型号
            if (!string.IsNullOrWhiteSpace(searchModel.Part_Spec))
            {
                query = query.Where(m => m.Part_Spec.Contains(searchModel.Part_Spec));
            }

            //储位
            if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
            {
                query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));
            }

            //料架
            if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
            {
                query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
            }

            //仓库编码
            if (!string.IsNullOrEmpty(searchModel.Fixture_Warehouse_ID))
            {
                query = query.Where(m => m.Fixture_Warehouse_ID.Contains(searchModel.Fixture_Warehouse_ID));
            }

            //仓库名字
            if (!string.IsNullOrEmpty(searchModel.Fixture_Warehouse_Name))
            {
                query = query.Where(m => m.Fixture_Warehouse_Name.Contains(searchModel.Fixture_Warehouse_Name));
            }

            return query.OrderByDescending(m => m.Modified_Date).ToList();
        }

        /// <summary>
        /// 导出勾选不的库存信息
        /// </summary>
        public List<Fixture_Part_InventoryDTO> ExportSelectedFixtureInventoryDetialReport(string uids)
        {
            uids = "," + uids + ",";
            var query = from FIXP in DataContext.Fixture_Part_Inventory
                        select new Fixture_Part_InventoryDTO
                        {
                            Fixture_Part_Inventory_UID = FIXP.Fixture_Part_Inventory_UID,
                            Fixture_Warehouse_Storage_UID = FIXP.Fixture_Warehouse_Storage_UID,
                            Fixture_Part_UID = FIXP.Fixture_Part_UID,
                            Inventory_Qty = FIXP.Inventory_Qty,
                            Remarks = FIXP.Remarks,
                            Modified_UID = FIXP.Modified_UID,
                            User_Name = FIXP.System_Users.User_Name,
                            Modified_Date = FIXP.Modified_Date,
                            Plant_Organization_UID = FIXP.Fixture_Warehouse_Storage.Plant_Organization_UID,
                            Plant = FIXP.Fixture_Warehouse_Storage.System_Organization.Organization_Name,
                            BG_Organization_UID = FIXP.Fixture_Warehouse_Storage.BG_Organization_UID,
                            BG_Organization = FIXP.Fixture_Warehouse_Storage.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = FIXP.Fixture_Warehouse_Storage.FunPlant_Organization_UID,
                            FunPlant_Organization = FIXP.Fixture_Warehouse_Storage.System_Organization2.Organization_Name,
                            Storage_ID = FIXP.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = FIXP.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = FIXP.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = FIXP.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = FIXP.Fixture_Part.Part_ID,
                            Part_Name = FIXP.Fixture_Part.Part_Name,
                            Part_Spec = FIXP.Fixture_Part.Part_Spec
                        };

            query = query.Where(m => uids.Contains("," + m.Fixture_Part_Inventory_UID + ","));
            return query.OrderByDescending(p => p.Modified_Date).ToList();
        }
    }
}
