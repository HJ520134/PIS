using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Warehouse_StorageRepository : IRepository<Fixture_Warehouse_Storage>
    {
        List<Fixture_Warehouse_StorageDTO> GetFixturePartWarehouseStorages(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);


    }
    public class Fixture_Warehouse_StorageRepository : RepositoryBase<Fixture_Warehouse_Storage>, IFixture_Warehouse_StorageRepository
    {
        public Fixture_Warehouse_StorageRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<Fixture_Warehouse_StorageDTO> GetFixturePartWarehouseStorages(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from M in DataContext.Fixture_Warehouse_Storage
                        select new Fixture_Warehouse_StorageDTO
                        {
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Fixture_Warehouse_UID = M.Fixture_Warehouse_UID,
                            Storage_ID = M.Storage_ID,
                            Rack_ID = M.Rack_ID,
                            Remarks = M.Remarks,
                            Is_Enable = M.Is_Enable,
                            Created_UID = M.Created_UID,
                            Created_Date = M.Created_Date,
                            Modified_UID = M.Modified_UID,
                            Modified_Date = M.Modified_Date,
                            Plant = M.System_Organization.Organization_Name,
                            BG_Organization = M.System_Organization1.Organization_Name,
                            FunPlant_Organization = M.System_Organization2.Organization_Name,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse.Fixture_Warehouse_Name,
                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }

    }
}
