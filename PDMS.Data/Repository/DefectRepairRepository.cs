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
    public interface IDefectRepairRepository : IRepository<DefectCode_RepairSolution>
    {

        IQueryable<DefectCode_RepairSolution> QueryDefectRepairs(DefectRepairSearch search, Page page, out int count);
        List<Fixture_RepairSolution> GetRepairSoulutions(DefectRepairSearch search);
        IQueryable<DefectCode_RepairSolution> QueryDefectCodeReapairSolution(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        int getFixture_DefectUID(string Fixture_DefectName);
        int getFixture_RepairSolution_UID(string Repair_SoulutionName);

    }
    public class DefectRepairRepository : RepositoryBase<DefectCode_RepairSolution>, IDefectRepairRepository
    {
        public DefectRepairRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
        public IQueryable<DefectCode_RepairSolution> QueryDefectCodeReapairSolution(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID) {
            var query = from d in DataContext.DefectCode_RepairSolution select d;
            if (Plant_Organization_UID > 0)
            {
                query = query.Where(w => w.Plant_Organization_UID == Plant_Organization_UID);
            }
            if (BG_Organization_UID > 0)
            {
                query = query.Where(w => w.BG_Organization_UID == BG_Organization_UID);
            }
            if (FunPlant_Organization_UID > 0)
            {
                query = query.Where(w => w.FunPlant_Organization_UID == FunPlant_Organization_UID);
            }
            return query;
        }
        public List<Fixture_RepairSolution> GetRepairSoulutions(DefectRepairSearch search)
        {
            var query = from FD in DataContext.Fixture_RepairSolution
                        select FD;

            if (search.Plant_Organization_UID > 0)
            {
                query = query.Where(w => w.Plant_Organization_UID == search.Plant_Organization_UID);
            }
            if (search.BG_Organization_UID > 0)
            {
                query = query.Where(w => w.BG_Organization_UID == search.BG_Organization_UID);
            }
            if (search.FunPlant_Organization_UID > 0)
            {
                query = query.Where(w => w.FunPlant_Organization_UID == search.FunPlant_Organization_UID);
            }
            return query.ToList();

        }

        public int getFixture_DefectUID(string Fixture_DefectName)
        {
            var query = from FD in DataContext.Fixture_DefectCode
                        where FD.DefectCode_Name == Fixture_DefectName
                        select FD.Fixture_Defect_UID;
            return query.FirstOrDefault();
        }

        public int getFixture_RepairSolution_UID(string Repair_SoulutionName)
        {
            var query = from FD in DataContext.Fixture_RepairSolution
                        where FD.RepairSolution_Name == Repair_SoulutionName
                        select FD.Fixture_RepairSolution_UID;
            return query.FirstOrDefault();
        }


        public IQueryable<DefectCode_RepairSolution> QueryDefectRepairs(DefectRepairSearch search, Page page, out int count)
        {
            var query = from w in DataContext.DefectCode_RepairSolution select w;

            if (string.IsNullOrWhiteSpace(search.ExportUIds))
            {
                if (search != null)
                {
                    if (search.Plant_Organization_UID > 0)
                    {
                        query = query.Where(w => w.Plant_Organization_UID == search.Plant_Organization_UID);
                    }
                    if (search.BG_Organization_UID > 0)
                    {
                        query = query.Where(w => w.BG_Organization_UID == search.BG_Organization_UID);
                    }
                    if (search.FunPlant_Organization_UID > 0)
                    {
                        query = query.Where(w => w.FunPlant_Organization_UID == search.FunPlant_Organization_UID);
                    }
                    if(search.Fixtrue_Defect_UID!=null&&search.Fixtrue_Defect_UID!=0)
                    {
                        query = query.Where(w => w.Fixtrue_Defect_UID == search.Fixtrue_Defect_UID);
                    }
                    if (!string.IsNullOrWhiteSpace(search.Fixture_DefectName))
                    {
                        //获取Fixture_DefectName对应的UID
                        int Fixture_Defect_UID = getFixture_DefectUID(search.Fixture_DefectName);

                        query = query.Where(w => w.Fixtrue_Defect_UID == Fixture_Defect_UID);
                    }
                    if (search.Repair_Solution_UID != null && search.Repair_Solution_UID != 0)
                    {                       
                           query = query.Where(w => w.Repair_Solution_UID == search.Repair_Solution_UID);
                    }
                    if (!string.IsNullOrWhiteSpace(search.Repair_SoulutionName))
                    {
                        int Fixture_RepairSolution_UID = getFixture_RepairSolution_UID(search.Repair_SoulutionName);
                        query = query.Where(w => w.Repair_Solution_UID == Fixture_RepairSolution_UID);
                    }

                    if (search.Is_Enable.HasValue)
                    {
                        query = query.Where(w => w.Is_Enable == search.Is_Enable);
                    }
                }
                count = query.Count();
                return query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Defect_RepairSolution_UID).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Defect_RepairSolution_UID)).OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Defect_RepairSolution_UID);
                count = 0;
                return query;
            }
        }
    }
}
