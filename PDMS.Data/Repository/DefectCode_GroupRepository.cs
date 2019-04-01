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
    public interface IDefectCode_GroupRepository : IRepository<DefectCode_Group>
    {
        IQueryable<DefectCode_GroupDTO> QueryDefectCode_Group(DefectCode_GroupDTO searchModel, Page page, out int totalcount);
        List<DefectCode_GroupDTO> DoExportDefectCode_GroupReprot(string DefectCode_Group_UIDs);

        List<DefectCode_GroupDTO> DoAllExportDefectCode_GroupReprot(DefectCode_GroupDTO searchModel);
        List<DefectCode_GroupDTO> DefectCode_GroupList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);

        List<DefectCode_GroupDTO> DefectCode_GroupList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID,string DefectCode_Group_ID);
        List<DefectCode_GroupDTO> DefectCode_GroupList(int Plant_Organization_UID);
        DefectCode_GroupDTO GetDefectCode_GroupByUID(int DefectCode_Group_UID);
    }
    public class DefectCode_GroupRepository : RepositoryBase<DefectCode_Group>, IDefectCode_GroupRepository
    {
        public DefectCode_GroupRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

        /// <summary>
        /// 获取治具异常群组列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public IQueryable<DefectCode_GroupDTO> QueryDefectCode_Group(DefectCode_GroupDTO searchModel, Page page, out int totalcount)
        {
            var query = from defectCode in DataContext.DefectCode_Group
                        select new DefectCode_GroupDTO
                        {
                            DefectCode_Group_UID = defectCode.DefectCode_Group_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            DefectCode_Group_ID = defectCode.DefectCode_Group_ID,
                            DefectCode_Group_Name = defectCode.DefectCode_Group_Name,
                            Fixtrue_Defect_UID = defectCode.Fixtrue_Defect_UID,
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
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_Group_ID))
                query = query.Where(m => m.DefectCode_Group_ID.Contains(searchModel.DefectCode_Group_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_Group_Name))
                query = query.Where(m => m.DefectCode_Group_Name.Contains(searchModel.DefectCode_Group_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_ID))
                query = query.Where(m => m.DefectCode_ID.Contains(searchModel.DefectCode_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_Name))
                query = query.Where(m => m.DefectCode_Name.Contains(searchModel.DefectCode_Name));
            if (searchModel.Is_Enable!=null)
                query = query.Where(m => m.Is_Enable== searchModel.Is_Enable);
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
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.DefectCode_Group_UID).GetPage(page);
            query = SetFixtureDTO(query.ToList());
            return query;
        }
        public IQueryable<DefectCode_GroupDTO> SetFixtureDTO(List<DefectCode_GroupDTO> Fixtures)
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
        public List<DefectCode_GroupDTO> DoExportDefectCode_GroupReprot(string DefectCode_Group_UIDs)
        {


            DefectCode_Group_UIDs = "," + DefectCode_Group_UIDs + ",";
            var query = from defectCode in DataContext.DefectCode_Group
                        select new DefectCode_GroupDTO
                        {

                            DefectCode_Group_UID = defectCode.DefectCode_Group_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            DefectCode_Group_ID = defectCode.DefectCode_Group_ID,
                            DefectCode_Group_Name = defectCode.DefectCode_Group_Name,
                            Fixtrue_Defect_UID = defectCode.Fixtrue_Defect_UID,
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
            query = query.Where(m => DefectCode_Group_UIDs.Contains("," + m.DefectCode_Group_UID + ","));
            var fixtures = SetListFixtureDTO(query.ToList());
            return fixtures;


        }
        /// <summary>
        /// 设置加载厂区，OP，功能厂，厂商
        /// </summary>
        /// <param name="Fixtures"></param>
        /// <returns></returns>
        public List<DefectCode_GroupDTO> SetListFixtureDTO(List<DefectCode_GroupDTO> Fixtures)
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

        public List<DefectCode_GroupDTO> DefectCode_GroupList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from defectCode in DataContext.DefectCode_Group
                        select new DefectCode_GroupDTO
                        {
                            DefectCode_Group_UID = defectCode.DefectCode_Group_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            DefectCode_Group_ID = defectCode.DefectCode_Group_ID,
                            DefectCode_Group_Name = defectCode.DefectCode_Group_Name,
                            Fixtrue_Defect_UID = defectCode.Fixtrue_Defect_UID,
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

            return query.GroupBy(x => new { x.Plant_Organization_UID, x.BG_Organization_UID, x.FunPlant_Organization_UID, x.DefectCode_Group_Name }).Select(r => r.FirstOrDefault()).ToList();

        }

        public List<DefectCode_GroupDTO> DefectCode_GroupList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string DefectCode_Group_ID)
        {

            var query = from defectCode in DataContext.DefectCode_Group
                        select new DefectCode_GroupDTO
                        {
                            DefectCode_Group_UID = defectCode.DefectCode_Group_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            DefectCode_Group_ID = defectCode.DefectCode_Group_ID,
                            DefectCode_Group_Name = defectCode.DefectCode_Group_Name,
                            Fixtrue_Defect_UID = defectCode.Fixtrue_Defect_UID,
                            Is_Enable = defectCode.Is_Enable,
                            Created_UID = defectCode.Created_UID,
                            Created_Date = defectCode.Created_Date,
                            Modified_UID = defectCode.Modified_UID,
                            Modified_Date = defectCode.Modified_Date,
                            DefectCode_ID = defectCode.Fixture_DefectCode.DefectCode_ID,
                            DefectCode_Name = defectCode.Fixture_DefectCode.DefectCode_Name,
                            Createder= defectCode.System_Users.User_Name,
                            Modifieder=defectCode.System_Users1.User_Name
                        };

            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            query = query.Where(m => m.DefectCode_Group_ID == DefectCode_Group_ID);
            return query.ToList();


        }

        public List<DefectCode_GroupDTO> DefectCode_GroupList(int Plant_Organization_UID)
        {
            var query = from defectCode in DataContext.DefectCode_Group
                        select new DefectCode_GroupDTO
                        {
                            DefectCode_Group_UID = defectCode.DefectCode_Group_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            DefectCode_Group_ID = defectCode.DefectCode_Group_ID,
                            DefectCode_Group_Name = defectCode.DefectCode_Group_Name,
                            Fixtrue_Defect_UID = defectCode.Fixtrue_Defect_UID,
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
        public DefectCode_GroupDTO GetDefectCode_GroupByUID(int DefectCode_Group_UID)
        {
            var query = from defectCode in DataContext.DefectCode_Group
                        select new DefectCode_GroupDTO
                        {
                            DefectCode_Group_UID = defectCode.DefectCode_Group_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            DefectCode_Group_ID = defectCode.DefectCode_Group_ID,
                            DefectCode_Group_Name = defectCode.DefectCode_Group_Name,
                            Fixtrue_Defect_UID = defectCode.Fixtrue_Defect_UID,
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

            query = query.Where(m => m.DefectCode_Group_UID == DefectCode_Group_UID);
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

        public List<DefectCode_GroupDTO> DoAllExportDefectCode_GroupReprot(DefectCode_GroupDTO searchModel)
        {

            var query = from defectCode in DataContext.DefectCode_Group
                        select new DefectCode_GroupDTO
                        {
                            DefectCode_Group_UID = defectCode.DefectCode_Group_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            DefectCode_Group_ID = defectCode.DefectCode_Group_ID,
                            DefectCode_Group_Name = defectCode.DefectCode_Group_Name,
                            Fixtrue_Defect_UID = defectCode.Fixtrue_Defect_UID,
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
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_Group_ID))
                query = query.Where(m => m.DefectCode_Group_ID.Contains(searchModel.DefectCode_Group_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_Group_Name))
                query = query.Where(m => m.DefectCode_Group_Name.Contains(searchModel.DefectCode_Group_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_ID))
                query = query.Where(m => m.DefectCode_ID.Contains(searchModel.DefectCode_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.DefectCode_Name))
                query = query.Where(m => m.DefectCode_Name.Contains(searchModel.DefectCode_Name));
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

                  
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.DefectCode_Group_UID);
            query = SetFixtureDTO(query.ToList());
            return query.ToList();
        }
    }
}
