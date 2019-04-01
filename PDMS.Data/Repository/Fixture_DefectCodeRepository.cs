using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_DefectCodeRepository:IRepository<Fixture_DefectCode>
    {

        IQueryable<Fixture_DefectCode> QueryFixture_DefectCodes(Fixture_DefectCodeModelSearch search, Page page, out int count);
        Fixture_DefectCodeDTO GetFixture_DefectCode(string DefectCode_ID, int Plant_Organization_UID, int BG_Organization_UID);
        List<Fixture_DefectCodeDTO> GetDefectCodes(List<int> IDs);
        List<Fixture_DefectCodeDTO> GetDefectCodesByPlant(int Plant_Organization_UID);
    }

    public class Fixture_DefectCodeRepository : RepositoryBase<Fixture_DefectCode>, IFixture_DefectCodeRepository
    {
        public Fixture_DefectCodeRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public IQueryable<Fixture_DefectCode> QueryFixture_DefectCodes(Fixture_DefectCodeModelSearch search, Page page, out int count)
        {
            var query = from m in DataContext.Fixture_DefectCode select m;
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
                    if (!string.IsNullOrWhiteSpace(search.DefectCode_ID))
                    {
                        query = query.Where(m => m.DefectCode_ID.Contains(search.DefectCode_ID));
                    }
                    if (!string.IsNullOrWhiteSpace(search.DefectCode_Name))
                    {
                        query = query.Where(m => m.DefectCode_Name.Contains(search.DefectCode_Name));
                    }
                    if (search.Is_Enable.HasValue)
                    {
                        query = query.Where(m => m.Is_Enable == search.Is_Enable.Value);
                    }
                }
                count = query.Count();
                return query.OrderBy(m => m.Plant_Organization_UID).ThenBy(m => m.BG_Organization_UID).ThenBy(m => m.Fixture_Defect_UID).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Fixture_Defect_UID)).OrderBy(m => m.Plant_Organization_UID).ThenBy(m => m.BG_Organization_UID).ThenBy(m => m.Fixture_Defect_UID);
                count = 0;
                return query;
            }
        }
        public Fixture_DefectCodeDTO GetFixture_DefectCode(string DefectCode_ID, int Plant_Organization_UID, int BG_Organization_UID)
        {

            var query = from defectCode in DataContext.Fixture_DefectCode
                        select new Fixture_DefectCodeDTO
                        {
                            Fixture_Defect_UID = defectCode.Fixture_Defect_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            Is_Enable = defectCode.Is_Enable,
                            Created_UID = defectCode.Created_UID,
                            Created_Date = defectCode.Created_Date,
                            Modified_UID = defectCode.Modified_UID,
                            Modified_Date = defectCode.Modified_Date,
                            DefectCode_ID = defectCode.DefectCode_ID,
                            DefectCode_Name = defectCode.DefectCode_Name

                        };
            if (Plant_Organization_UID > 0)
            {
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            }
            if (BG_Organization_UID > 0)
            {
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            }
            if (!string.IsNullOrWhiteSpace(DefectCode_ID))
            {
                query = query.Where(m => m.DefectCode_ID==DefectCode_ID);
            }
            query = query.Where(m => m.Is_Enable == true);
            return query.FirstOrDefault();
        }

        public List<Fixture_DefectCodeDTO> GetDefectCodes(List<int> IDs)
        {
            var query = from defectCode in DataContext.Fixture_DefectCode
                        select new Fixture_DefectCodeDTO
                        {
                            Fixture_Defect_UID = defectCode.Fixture_Defect_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            Is_Enable = defectCode.Is_Enable,
                            Created_UID = defectCode.Created_UID,
                            Created_Date = defectCode.Created_Date,
                            Modified_UID = defectCode.Modified_UID,
                            Modified_Date = defectCode.Modified_Date,
                            DefectCode_ID = defectCode.DefectCode_ID,
                            DefectCode_Name = defectCode.DefectCode_Name

                        };
            query = query.Where(o => o.Is_Enable == true);
            query = query.Where(o => IDs.Contains(o.Fixture_Defect_UID));
            return query.ToList();

        }


     public   List<Fixture_DefectCodeDTO> GetDefectCodesByPlant(int Plant_Organization_UID)
        {
            var query = from defectCode in DataContext.Fixture_DefectCode
                        select new Fixture_DefectCodeDTO
                        {
                            Fixture_Defect_UID = defectCode.Fixture_Defect_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            Is_Enable = defectCode.Is_Enable,
                            Created_UID = defectCode.Created_UID,
                            Created_Date = defectCode.Created_Date,
                            Modified_UID = defectCode.Modified_UID,
                            Modified_Date = defectCode.Modified_Date,
                            DefectCode_ID = defectCode.DefectCode_ID,
                            DefectCode_Name = defectCode.DefectCode_Name

                        };
            query = query.Where(o => o.Is_Enable == true);
            query = query.Where(o => o.Plant_Organization_UID== Plant_Organization_UID);
            return query.ToList();


        }
    }
}
