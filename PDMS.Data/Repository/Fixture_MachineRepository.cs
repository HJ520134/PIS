using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_MachineRepository: IRepository<Fixture_Machine>
    {
        IQueryable<Fixture_Machine> QueryFixture_Machines(Fixture_MachineModelSearch search, Page page, out int count);
        
    }
    public class Fixture_MachineRepository: RepositoryBase<Fixture_Machine>,IFixture_MachineRepository
    {
        public Fixture_MachineRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public IQueryable<Fixture_Machine> QueryFixture_Machines(Fixture_MachineModelSearch search, Page page, out int count)
        {
            var query = from m in DataContext.Fixture_Machine select m;
            if (string.IsNullOrWhiteSpace(search.ExportUIds))
            {
                if (search != null)
                {
                    if (search.Plant_Organization_UID > 0)
                    {
                        query = query.Where(m => m.Plant_Organization_UID == search.Plant_Organization_UID);
                    }
                    if (search.BG_Organization_UID > 0)
                    {
                        query = query.Where(m => m.BG_Organization_UID == search.BG_Organization_UID);
                    }
                    if (search.FunPlant_Organization_UID.HasValue)
                    {
                        query = query.Where(m => m.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                    }
                    if (search.Production_Line_UID.HasValue)
                    {
                        query = query.Where(m => m.Production_Line_UID == search.Production_Line_UID.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(search.Machine_ID))
                    {
                        query = query.Where(m => m.Machine_ID.Contains(search.Machine_ID));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Equipment_No))
                    {
                        query = query.Where(m => m.Equipment_No.Contains(search.Equipment_No));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Machine_Name))
                    {
                        query = query.Where(m => m.Machine_Name.Contains(search.Machine_Name));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Machine_Desc))
                    {
                        query = query.Where(m => m.Machine_Desc.Contains(search.Machine_Desc));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Machine_Type))
                    {
                        query = query.Where(m => m.Machine_Type.Contains(search.Machine_Type));
                    }
                    if (search.Is_Enable.HasValue)
                    {
                        query = query.Where(m => m.Is_Enable == search.Is_Enable.Value);
                    }

                    //TODO 2018/09/18 steven EQP_Uids
                    if (search.EQP_Uid.HasValue)
                    {
                        query = query.Where(m => m.EQP_Uid == search.EQP_Uid.Value);
                    }
                }
                count = query.Count();
                return query.OrderBy(m => m.Plant_Organization_UID).ThenBy(m => m.BG_Organization_UID).ThenBy(m => m.Fixture_Machine_UID).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Fixture_Machine_UID)).OrderBy(m => m.Plant_Organization_UID).ThenBy(m => m.BG_Organization_UID).ThenBy(m => m.Fixture_Machine_UID);
                count = 0;
                return query;
            }
        }
    }
}
