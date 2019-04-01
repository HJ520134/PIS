using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixturePartOrderDRepository : IRepository<Fixture_Part_Order_D>
    {
        List<Fixture_Part_Order_DDTO> GetFixture_Part_Order_Ds(int Fixture_Part_Order_M_UID);
        Fixture_Part_Order_DDTO GetFixture_Part_Order_D(int Fixture_Part_Order_D_UID);
    }

    public class FixturePartOrderDRepository : RepositoryBase<Fixture_Part_Order_D>, IFixturePartOrderDRepository
    {
        public FixturePartOrderDRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<Fixture_Part_Order_DDTO> GetFixture_Part_Order_Ds(int Fixture_Part_Order_M_UID)
        {
            var query = from record in DataContext.Fixture_Part_Order_D
                        select new Fixture_Part_Order_DDTO
                        {
                            Fixture_Part_Order_D_UID = record.Fixture_Part_Order_D_UID,
                            Fixture_Part_Order_M_UID = record.Fixture_Part_Order_M_UID,
                            Vendor_Info_UID = record.Vendor_Info_UID,
                            Fixture_Part_UID = record.Fixture_Part_UID,
                            Price = record.Price,
                            Qty = record.Qty,
                            Is_Complated = record.Is_Complated,
                            Del_Flag = record.Del_Flag,
                            Part_ID = record.Fixture_Part.Part_ID,
                            Part_Name = record.Fixture_Part.Part_Name,
                            Part_Spec = record.Fixture_Part.Part_Spec
                        };
            if (Fixture_Part_Order_M_UID != 0)
                query = query.Where(m => m.Fixture_Part_Order_M_UID == Fixture_Part_Order_M_UID);
            return query.ToList();

        }

        public Fixture_Part_Order_DDTO GetFixture_Part_Order_D(int Fixture_Part_Order_D_UID)
        {

            var query = from record in DataContext.Fixture_Part_Order_D
                        select new Fixture_Part_Order_DDTO
                        {
                            Fixture_Part_Order_D_UID = record.Fixture_Part_Order_D_UID,
                            Fixture_Part_Order_M_UID = record.Fixture_Part_Order_M_UID,
                            Vendor_Info_UID = record.Vendor_Info_UID,
                            Fixture_Part_UID = record.Fixture_Part_UID,
                            Price = record.Price,
                            Qty = record.Qty,
                            Is_Complated = record.Is_Complated,
                            Del_Flag = record.Del_Flag,
                            Part_ID = record.Fixture_Part.Part_ID,
                            Part_Name = record.Fixture_Part.Part_Name,
                            Part_Spec = record.Fixture_Part.Part_Spec,
                            Plant_Organization_UID = record.Fixture_Part.Plant_Organization_UID,
                            //Plant 
                            BG_Organization_UID = record.Fixture_Part.BG_Organization_UID,
                            //   BG_Organization 
                            FunPlant_Organization_UID = record.Fixture_Part.FunPlant_Organization_UID,
                            //  FunPlant_Organization 
                        };

            query = query.Where(m => m.Fixture_Part_Order_D_UID == Fixture_Part_Order_D_UID);
            return SetActual_Delivery_Qty(query.FirstOrDefault());

        }


        public Fixture_Part_Order_DDTO SetActual_Delivery_Qty(Fixture_Part_Order_DDTO fixture_Part_Order_D)
        {
            //根据配件主键和采购主键获取单个配件的明细
            List<int> fixturePartOrderD_UIDs = DataContext.Fixture_Part_Order_D.Where(o => o.Fixture_Part_UID == fixture_Part_Order_D.Fixture_Part_UID && o.Fixture_Part_Order_M_UID == fixture_Part_Order_D.Fixture_Part_Order_M_UID).Select(o => o.Fixture_Part_Order_D_UID).ToList();
            List<Fixture_Part_Order_Schedule> fixture_Part_Order_Schedules = DataContext.Fixture_Part_Order_Schedule.Where(o => fixturePartOrderD_UIDs.Contains(o.Fixture_Part_Order_D_UID)).ToList();
            decimal actual_Delivery_Qty = 0;
            if (fixture_Part_Order_Schedules != null && fixture_Part_Order_Schedules.Count > 0)
            {
                foreach (var item in fixture_Part_Order_Schedules)
                {
                    if (item.Actual_Delivery_Qty != null)
                    {
                        actual_Delivery_Qty += item.Actual_Delivery_Qty.Value;
                    }
                }
            }
            fixture_Part_Order_D.Actual_Delivery_Qty = actual_Delivery_Qty;
            //已入库总数量
            decimal inbound_Qty = 0;
            List<Fixture_Storage_Inbound> fixture_Storage_Inbounds = DataContext.Fixture_Storage_Inbound.Where(o => o.Fixture_Part_UID == fixture_Part_Order_D.Fixture_Part_UID && o.Fixture_Part_Order_M_UID == fixture_Part_Order_D.Fixture_Part_Order_M_UID).ToList();

            if (fixture_Storage_Inbounds != null && fixture_Storage_Inbounds.Count > 0)
            {
                foreach (var item in fixture_Storage_Inbounds)
                {
                    inbound_Qty += item.Inbound_Qty;
                }
            }
            fixture_Part_Order_D.Inbound_Qty = inbound_Qty;
            return fixture_Part_Order_D;
        }
    }
}
