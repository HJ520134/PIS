using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Storage_Outbound_DRepository : IRepository<Fixture_Storage_Outbound_D>
    {
        List<Fixture_Storage_Outbound_DDTO> QueryOutBouddetails(int Storage_In_Out_Bound_UID);
    }
    public class Fixture_Storage_Outbound_DRepository : RepositoryBase<Fixture_Storage_Outbound_D>, IFixture_Storage_Outbound_DRepository
    {
        public Fixture_Storage_Outbound_DRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {



        }
        public List<Fixture_Storage_Outbound_DDTO> QueryOutBouddetails(int Storage_In_Out_Bound_UID)
        {

            var query = from M in DataContext.Fixture_Storage_Outbound_D
                        select new Fixture_Storage_Outbound_DDTO
                        {

                            Fixture_Storage_Outbound_D_UID = M.Fixture_Storage_Outbound_D_UID,
                            Fixture_Storage_Outbound_M_UID = M.Fixture_Storage_Outbound_M_UID,
                            Fixture_Part_UID = M.Fixture_Part_UID,
                            Fixture_Warehouse_Storage_UID = M.Fixture_Warehouse_Storage_UID,
                            In_Out_Bound_Qty = M.Outbound_Qty,
                            Remarks = M.Remarks,
                            Outbound_Qty = 0,
                            Be_Out_Qty = 0,
                            Storage_ID = M.Fixture_Warehouse_Storage.Storage_ID,
                            Rack_ID = M.Fixture_Warehouse_Storage.Rack_ID,
                            Fixture_Warehouse_ID = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_ID,
                            Fixture_Warehouse_Name = M.Fixture_Warehouse_Storage.Fixture_Warehouse.Fixture_Warehouse_Name,
                            Part_ID = M.Fixture_Part.Part_ID,
                            Part_Name = M.Fixture_Part.Part_Name,
                            Part_Spec = M.Fixture_Part.Part_Spec,

                        };
            query = query.Where(m => m.Fixture_Storage_Outbound_M_UID == Storage_In_Out_Bound_UID);
            return SetInventory_Qty(query.ToList());
        }

        public List<Fixture_Storage_Outbound_DDTO> SetInventory_Qty(List<Fixture_Storage_Outbound_DDTO> Fixture_Storage_Outbound_DDTOs)
        {
            List<Fixture_Part_Inventory> Fixture_Part_Inventorys = DataContext.Fixture_Part_Inventory.ToList();


            foreach (var item in Fixture_Storage_Outbound_DDTOs)
            {
                Fixture_Part_Inventory fixture_Part_Inventory = Fixture_Part_Inventorys.FirstOrDefault(o => o.Fixture_Warehouse_Storage_UID == item.Fixture_Warehouse_Storage_UID && o.Fixture_Part_UID == item.Fixture_Part_UID);
                if (fixture_Part_Inventory != null)
                {
                    item.Outbound_Qty = fixture_Part_Inventory.Inventory_Qty;
                }
            }

            return Fixture_Storage_Outbound_DDTOs;
        }
    }
}
