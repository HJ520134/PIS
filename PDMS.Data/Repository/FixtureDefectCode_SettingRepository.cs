using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{

    public interface IFixtureDefectCode_SettingRepository : IRepository<FixtureDefectCode_Setting>
    {
        IQueryable<FixtureDefectCode_SettingDTO> QueryDefectCode_Setting(FixtureDefectCode_SettingDTO searchModel, Page page, out int totalcount);
        List<FixtureDefectCode_SettingDTO> DoExportFixtureDefectCode_SettingReprot(string FixtureDefectCode_Setting_UIDs);
        List<FixtureDefectCode_SettingDTO> DoAllExportFixtureDefectCode_SettingReprot(FixtureDefectCode_SettingDTO searchModel);
        List<FixtureDefectCode_SettingDTO> GetFixtureDefectCode_SettingByPlant(int Plant_Organization_UID);
        FixtureDefectCode_SettingDTO GetFixtureDefectCode_SettingDTOByUID(int FixtureDefectCode_Setting_UID);
        List<FixtureDefectCode_SettingDTO> GetFixtureDefectCode_SettingList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, int Fixture_Defect_UID, string Fixture_NO);
    }
    public class FixtureDefectCode_SettingRepository : RepositoryBase<FixtureDefectCode_Setting>, IFixtureDefectCode_SettingRepository
    {
        public FixtureDefectCode_SettingRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

        /// <summary>
        /// 获取治具异常群组列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public IQueryable<FixtureDefectCode_SettingDTO> QueryDefectCode_Setting(FixtureDefectCode_SettingDTO searchModel, Page page, out int totalcount)
        {
            var query = from defectCode in DataContext.FixtureDefectCode_Setting
                        select new FixtureDefectCode_SettingDTO
                        {
                            FixtureDefectCode_Setting_UID = defectCode.FixtureDefectCode_Setting_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            Fixture_Defect_UID = defectCode.Fixture_Defect_UID,
                            Fixture_NO = defectCode.Fixture_NO,
                            Is_Enable = defectCode.Is_Enable,
                            Created_UID = defectCode.Created_UID,
                            Created_Date = defectCode.Created_Date,
                            Modified_UID = defectCode.Modified_UID,
                            Modified_Date = defectCode.Modified_Date,
                            DefectCode_ID = defectCode.Fixture_DefectCode.DefectCode_ID,
                            DefectCode_Name = defectCode.Fixture_DefectCode.DefectCode_Name,
                            Createder = defectCode.System_Users.User_Name,
                            Modifieder=defectCode.System_Users1.User_Name
                        };
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_ID))
                query = query.Where(m => m.DefectCode_ID.Contains(searchModel.DefectCode_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_Name))
                query = query.Where(m => m.DefectCode_Name.Contains(searchModel.DefectCode_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(searchModel.Fixture_NO));
            if (searchModel.Is_Enable != null)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);

            if (!string.IsNullOrWhiteSpace(searchModel.Modifieder))
                query = query.Where(m => m.Modifieder.Contains(searchModel.Modifieder));
            if (searchModel.Modified_UID != 0)
                query = query.Where(m => m.Modified_UID == searchModel.Modified_UID);
            if (searchModel.End_Date_From != null)
                query = query.Where(m => m.Created_Date >= searchModel.End_Date_From);
            if (searchModel.End_Date_To != null)
            {
                DateTime nextTime = searchModel.End_Date_To.Value.Date.AddDays(1);
                query = query.Where(m => m.Modified_Date < nextTime);
            }
         
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.FixtureDefectCode_Setting_UID).GetPage(page);
            query = SetFixtureDTO(query.ToList());
            return query;
        }
        public IQueryable<FixtureDefectCode_SettingDTO> SetFixtureDTO(List<FixtureDefectCode_SettingDTO> Fixtures)
        {
            List<System_Organization> system_Organizations = DataContext.System_Organization.ToList();
            foreach (var item in Fixtures)
            {
                // 设置厂区                            
                var system_OrganizationPlant = system_Organizations.Where(o => o.Organization_UID == item.Plant_Organization_UID).FirstOrDefault();
                if (system_OrganizationPlant != null)
                {
                    item.PlantName = system_OrganizationPlant.Organization_Name;
                }
                //设置OP
                var system_OrganizationOP = system_Organizations.Where(o => o.Organization_UID == item.BG_Organization_UID).FirstOrDefault();
                if (system_OrganizationOP != null)
                {
                    item.OPName = system_OrganizationOP.Organization_Name;
                }
                //设置功能厂
                if (item.FunPlant_Organization_UID != null && item.FunPlant_Organization_UID != 0)
                {
                    var system_OrganizationFunPlant = system_Organizations.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).FirstOrDefault();
                    if (system_OrganizationFunPlant != null)
                    {
                        item.FunPlantName = system_OrganizationFunPlant.Organization_Name;
                    }
                }

            }
            return Fixtures.AsQueryable();
        }
        /// <summary>
        /// 导出具异常群组
        /// </summary>
        /// <param name="DefectCode_Group_UIDs"></param>
        /// <returns></returns>
        public List<FixtureDefectCode_SettingDTO> DoExportFixtureDefectCode_SettingReprot(string FixtureDefectCode_Setting_UIDs)
        {


            FixtureDefectCode_Setting_UIDs = "," + FixtureDefectCode_Setting_UIDs + ",";
            var query = from defectCode in DataContext.FixtureDefectCode_Setting
                        select new FixtureDefectCode_SettingDTO
                        {
                            FixtureDefectCode_Setting_UID = defectCode.FixtureDefectCode_Setting_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            Fixture_Defect_UID = defectCode.Fixture_Defect_UID,
                            Fixture_NO = defectCode.Fixture_NO,
                            Is_Enable = defectCode.Is_Enable,
                            Created_UID = defectCode.Created_UID,
                            Created_Date = defectCode.Created_Date,
                            Modified_UID = defectCode.Modified_UID,
                            Modified_Date = defectCode.Modified_Date,
                            DefectCode_ID = defectCode.Fixture_DefectCode.DefectCode_ID,
                            DefectCode_Name = defectCode.Fixture_DefectCode.DefectCode_Name,
                            Createder = defectCode.System_Users.User_Name,
                            Modifieder = defectCode.System_Users1.User_Name

                        };
            query = query.Where(m => FixtureDefectCode_Setting_UIDs.Contains("," + m.FixtureDefectCode_Setting_UID + ","));
            var fixtures = SetListFixtureDTO(query.ToList());
            return fixtures;
        }

        public List<FixtureDefectCode_SettingDTO> DoAllExportFixtureDefectCode_SettingReprot(FixtureDefectCode_SettingDTO searchModel)
        {
            var query = from defectCode in DataContext.FixtureDefectCode_Setting
                        select new FixtureDefectCode_SettingDTO
                        {
                            FixtureDefectCode_Setting_UID = defectCode.FixtureDefectCode_Setting_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            Fixture_Defect_UID = defectCode.Fixture_Defect_UID,
                            Fixture_NO = defectCode.Fixture_NO,
                            Is_Enable = defectCode.Is_Enable,
                            Created_UID = defectCode.Created_UID,
                            Created_Date = defectCode.Created_Date,
                            Modified_UID = defectCode.Modified_UID,
                            Modified_Date = defectCode.Modified_Date,
                            DefectCode_ID = defectCode.Fixture_DefectCode.DefectCode_ID,
                            DefectCode_Name = defectCode.Fixture_DefectCode.DefectCode_Name,
                            Createder = defectCode.System_Users.User_Name,
                            Modifieder = defectCode.System_Users1.User_Name
                        };
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_ID))
                query = query.Where(m => m.DefectCode_ID.Contains(searchModel.DefectCode_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_Name))
                query = query.Where(m => m.DefectCode_Name.Contains(searchModel.DefectCode_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(searchModel.Fixture_NO));
            if (searchModel.Is_Enable != null)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);

            if (!string.IsNullOrWhiteSpace(searchModel.Modifieder))
                query = query.Where(m => m.Modifieder.Contains(searchModel.Modifieder));
            if (searchModel.Modified_UID != 0)
                query = query.Where(m => m.Modified_UID == searchModel.Modified_UID);
            if (searchModel.End_Date_From != null)
                query = query.Where(m => m.Created_Date >= searchModel.End_Date_From);
            if (searchModel.End_Date_To != null)
            {
                DateTime nextTime = searchModel.End_Date_To.Value.Date.AddDays(1);
                query = query.Where(m => m.Modified_Date < nextTime);
            }

           
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.FixtureDefectCode_Setting_UID);
            query = SetFixtureDTO(query.ToList());
            return query.ToList();
        }

        /// <summary>
        /// 设置加载厂区，OP，功能厂，厂商
        /// </summary>
        /// <param name="Fixtures"></param>
        /// <returns></returns>
        public List<FixtureDefectCode_SettingDTO> SetListFixtureDTO(List<FixtureDefectCode_SettingDTO> Fixtures)
        {
            List<System_Organization> system_Organizations = DataContext.System_Organization.ToList();
            foreach (var item in Fixtures)
            {
                // 设置厂区                            
                var system_OrganizationPlant = system_Organizations.Where(o => o.Organization_UID == item.Plant_Organization_UID).FirstOrDefault();
                if (system_OrganizationPlant != null)
                {
                    item.PlantName = system_OrganizationPlant.Organization_Name;
                }
                //设置OP
                var system_OrganizationOP = system_Organizations.Where(o => o.Organization_UID == item.BG_Organization_UID).FirstOrDefault();
                if (system_OrganizationOP != null)
                {
                    item.OPName = system_OrganizationOP.Organization_Name;
                }
                //设置功能厂
                if (item.FunPlant_Organization_UID != null && item.FunPlant_Organization_UID != 0)
                {
                    var system_OrganizationFunPlant = system_Organizations.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).FirstOrDefault();
                    if (system_OrganizationFunPlant != null)
                    {
                        item.FunPlantName = system_OrganizationFunPlant.Organization_Name;
                    }
                }

            }
            return Fixtures;
        }

        public List<FixtureDefectCode_SettingDTO> GetFixtureDefectCode_SettingByPlant(int Plant_Organization_UID)
        {
            var query = from defectCode in DataContext.FixtureDefectCode_Setting
                        select new FixtureDefectCode_SettingDTO
                        {
                            FixtureDefectCode_Setting_UID = defectCode.FixtureDefectCode_Setting_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            Fixture_Defect_UID = defectCode.Fixture_Defect_UID,
                            Fixture_NO = defectCode.Fixture_NO,
                            Is_Enable = defectCode.Is_Enable,
                            Created_UID = defectCode.Created_UID,
                            Created_Date = defectCode.Created_Date,
                            Modified_UID = defectCode.Modified_UID,
                            Modified_Date = defectCode.Modified_Date,
                            DefectCode_ID = defectCode.Fixture_DefectCode.DefectCode_ID,
                            DefectCode_Name = defectCode.Fixture_DefectCode.DefectCode_Name,
                            Createder = defectCode.System_Users.User_Name,
                            Modifieder = defectCode.System_Users1.User_Name

                        };

            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            return query.ToList();
        }

        public FixtureDefectCode_SettingDTO GetFixtureDefectCode_SettingDTOByUID(int FixtureDefectCode_Setting_UID)
        {
            var query = from defectCode in DataContext.FixtureDefectCode_Setting
                        select new FixtureDefectCode_SettingDTO
                        {
                            FixtureDefectCode_Setting_UID = defectCode.FixtureDefectCode_Setting_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            Fixture_Defect_UID = defectCode.Fixture_Defect_UID,
                            Fixture_NO = defectCode.Fixture_NO,
                            Is_Enable = defectCode.Is_Enable,
                            Created_UID = defectCode.Created_UID,
                            Created_Date = defectCode.Created_Date,
                            Modified_UID = defectCode.Modified_UID,
                            Modified_Date = defectCode.Modified_Date,
                            DefectCode_ID = defectCode.Fixture_DefectCode.DefectCode_ID,
                            DefectCode_Name = defectCode.Fixture_DefectCode.DefectCode_Name,
                            Createder = defectCode.System_Users.User_Name,
                            Modifieder = defectCode.System_Users1.User_Name

                        };
            query = query.Where(m => m.FixtureDefectCode_Setting_UID == FixtureDefectCode_Setting_UID);
            var fixtures = SetListFixtureDTO(query.ToList());
            if (fixtures.Count > 0)
            {
                return fixtures.FirstOrDefault();
            }
            else
            {
                return null;
            }

        }


        public List<FixtureDefectCode_SettingDTO> GetFixtureDefectCode_SettingList(int Plant_Organization_UID, int BG_Organization_UID,int FunPlant_Organization_UID,int Fixture_Defect_UID,string Fixture_NO)
        {
            var query = from defectCode in DataContext.FixtureDefectCode_Setting
                        select new FixtureDefectCode_SettingDTO
                        {
                            FixtureDefectCode_Setting_UID = defectCode.FixtureDefectCode_Setting_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            Fixture_Defect_UID = defectCode.Fixture_Defect_UID,
                            Fixture_NO = defectCode.Fixture_NO,
                            Is_Enable = defectCode.Is_Enable,
                            Created_UID = defectCode.Created_UID,
                            Created_Date = defectCode.Created_Date,
                            Modified_UID = defectCode.Modified_UID,
                            Modified_Date = defectCode.Modified_Date,
                            DefectCode_ID = defectCode.Fixture_DefectCode.DefectCode_ID,
                            DefectCode_Name = defectCode.Fixture_DefectCode.DefectCode_Name,
                            Createder = defectCode.System_Users.User_Name,
                            Modifieder = defectCode.System_Users1.User_Name

                        };

            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            if (Fixture_Defect_UID != 0)
                query = query.Where(m => m.Fixture_Defect_UID == Fixture_Defect_UID);
            if (!string.IsNullOrWhiteSpace(Fixture_NO))
                query = query.Where(m => m.Fixture_NO==Fixture_NO);
            return query.ToList();
        }

    }
}
