using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System.Linq;
using System.Data.Entity.SqlServer;
using System;
using PDMS.Common.Constants;
using System.Collections.Generic;

namespace PDMS.Data.Repository
{
    public interface ISystemFunctionPlantRepository : IRepository<System_Function_Plant>
    {
        IQueryable<FuncPlantMaintanance> QueryFuncPlants(FuncPlantSearchModel search, Page page, out int count);
        FuncPlantMaintanance QueryFuncPlant(int uuid);
        List<string> GetFuncPlantByPlantAndOPType(string Plant, string OP_Types);
        List<System_Function_Plant> GetAllPlant(string optype);

        List<FunPlantVM> GetFunPlantByQAMasterUID(int QualityAssurance_InputMaster_UID);
    }

    public class SystemFunctionPlantRepository : RepositoryBase<System_Function_Plant>, ISystemFunctionPlantRepository
    {
        public SystemFunctionPlantRepository(IDatabaseFactory databaseFactory)
        : base(databaseFactory)
        {

        }
        public IQueryable<FuncPlantMaintanance> QueryFuncPlants(FuncPlantSearchModel search, Page page, out int count)
        {
            var query = from plant in DataContext.System_Function_Plant.Include("System_Users")
                        select new FuncPlantMaintanance
                        {
                            System_FuncPlant_UID = plant.System_FunPlant_UID,
                            FunPlant = plant.FunPlant,
                            Plant = plant.System_Plant.Plant,
                            OPType = plant.OP_Types,
                            Plant_Manager = plant.FunPlant_Manager,
                            FuncPlant_Context = plant.FunPlant_Contact,
                            Modified_UserName = plant.System_Users.User_Name,
                            Modified_UserNTID = plant.System_Users.User_NTID,
                            Modified_Date = DateTime.Now,
                            Organization_ID=plant.FunPlant_OrganizationUID.ToString()
                        };
            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                #region Modified_Date
                if (search.Modified_Date_From != null)
                {
                    query = query.Where(m => m.Modified_Date >= search.Modified_Date_From);
                }
                if (search.Modified_Date_End != null)
                {
                    var endDate = ((DateTime)search.Modified_Date_End).AddDays(1);
                    query = query.Where(m => m.Modified_Date < endDate);
                }
                #endregion

                #region 查询Modified_NTID
                if (!string.IsNullOrWhiteSpace(search.Modified_By_NTID))
                {
                    query = query.Where(q => q.Modified_UserNTID == search.Modified_By_NTID);
                }
                #endregion


                if (!string.IsNullOrWhiteSpace(search.FunPlant))
                {
                    query = query.Where(p => p.FunPlant == search.FunPlant);

                }
                if (!string.IsNullOrWhiteSpace(search.FuncPlant_Manager))
                {
                    query = query.Where(p => p.Plant_Manager == search.FuncPlant_Manager);
                }

                if (!string.IsNullOrWhiteSpace(search.OPType))
                {
                    query = query.Where(p => p.OPType == search.OPType);
                }

                if (!string.IsNullOrWhiteSpace(search.Plant))
                {
                    query = query.Where(p => p.Plant == search.Plant);
                }

                count = query.Count();
                return query.OrderBy(o => o.System_FuncPlant_UID).GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.System_FuncPlant_UID));

                count = 0;
                return query.OrderBy(o => o.FunPlant);
            }
        }

        public FuncPlantMaintanance QueryFuncPlant(int uuid)
        {
            var query = from plant in DataContext.System_Function_Plant.Include("System_Users")
                        where (plant.System_FunPlant_UID==uuid)
                        select new FuncPlantMaintanance
                        {
                            System_FuncPlant_UID = plant.System_FunPlant_UID,
                            FunPlant = plant.FunPlant,
                            Plant = plant.System_Plant.Plant,
                            OPType = plant.OP_Types,
                            Plant_Manager = plant.FunPlant_Manager,
                            FuncPlant_Context = plant.FunPlant_Contact,
                            Modified_UserName = plant.System_Users.User_Name,
                            Modified_UserNTID = plant.System_Users.User_NTID,
                            Modified_Date = DateTime.Now,
                            Organization_ID= plant.FunPlant_OrganizationUID.ToString()
                        };
            return query.FirstOrDefault();
        }

        public List<string> GetFuncPlantByPlantAndOPType(string Plant, string OP_Types)
        {
            var query = from item in DataContext.System_Function_Plant
                join plant in DataContext.System_Plant on item.System_Plant_UID equals plant.System_Plant_UID
                where (plant.Plant == Plant && item.OP_Types == OP_Types)
                orderby item.FunPlant
                select item.FunPlant;
            return query.Distinct().ToList();
        }


        public List<System_Function_Plant> GetAllPlant(string optype)
        {
            var sqlStr = @"SELECT * FROM dbo.System_Function_Plant where OP_Types='{0}'";
            sqlStr = string.Format(sqlStr, optype);
            var dbList = DataContext.Database.SqlQuery<System_Function_Plant>(sqlStr).ToList();
            return dbList;
        }

        public List<FunPlantVM> GetFunPlantByQAMasterUID(int QualityAssurance_InputMaster_UID)
        {
            List<FunPlantVM> result = new List<FunPlantVM>();
            string sql = string.Format(@"
                    IF EXISTS ( SELECT TOP 1
                                        1
                                FROM    QualityAssurance_InputMaster
                                WHERE   QualityAssurance_InputMaster_UID = {0} )
                        BEGIN
                            SELECT DISTINCT
                                    SFP.FunPlant as FunPlant,
                                    SFP.System_FunPlant_UID as System_FunPlant_UID
                            FROM    dbo.QualityAssurance_InputMaster QA
                                    INNER JOIN dbo.FlowChart_Detail Fd ON Fd.FlowChart_Detail_UID = QA.FlowChart_Detail_UID
                                    INNER JOIN dbo.FlowChart_Master FM ON FM.FlowChart_Master_UID = Fd.FlowChart_Master_UID AND FM.FlowChart_Version = Fd.FlowChart_Version
                                    INNER JOIN dbo.FlowChart_Detail FD2 ON FD2.FlowChart_Master_UID = FM.FlowChart_Master_UID AND FD2.FlowChart_Version = FM.FlowChart_Version
                                    INNER JOIN dbo.System_Function_Plant SFP ON SFP.System_FunPlant_UID = FD2.System_FunPlant_UID
                            WHERE	QA.QualityAssurance_InputMaster_UID={0}
                        END
                    ELSE
                        BEGIN
                            SELECT DISTINCT
                                    SFP.FunPlant as FunPlant,
                                    SFP.System_FunPlant_UID as System_FunPlant_UID
                            FROM    dbo.QualityAssurance_InputMaster_History QA
                                    INNER JOIN dbo.FlowChart_Detail Fd ON Fd.FlowChart_Detail_UID = QA.FlowChart_Detail_UID
                                    INNER JOIN dbo.FlowChart_Master FM ON FM.FlowChart_Master_UID = Fd.FlowChart_Master_UID AND FM.FlowChart_Version = Fd.FlowChart_Version
                                    INNER JOIN dbo.FlowChart_Detail FD2 ON FD2.FlowChart_Master_UID = FM.FlowChart_Master_UID AND FD2.FlowChart_Version = FM.FlowChart_Version
                                    INNER JOIN dbo.System_Function_Plant SFP ON SFP.System_FunPlant_UID = FD2.System_FunPlant_UID
                            WHERE	QA.QualityAssurance_InputMaster_UID={0}
	                    END
                    ", QualityAssurance_InputMaster_UID);

            result = DataContext.Database.SqlQuery<FunPlantVM>(sql).ToList();
            return result;
        }
    }
}
