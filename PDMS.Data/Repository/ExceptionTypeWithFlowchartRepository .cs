using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Common.Helpers;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace PDMS.Data.Repository
{

    public class ExceptionTypeWithFlowchartRepository : RepositoryBase<ExceptionTypeWithFlowchart>, IExceptionTypeWithFlowchartRepository
    {
        private Logger log = new Logger("ExceptionTypeWithFlowchartRepository");

        public ExceptionTypeWithFlowchartRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public string AddTExceptionTypesoFlowChart(ExceptionTypesAddToFlowChartVM data)
        {
            string result = "Success";
            try
            {
                if (data.ExcetionTypes.Count == 0)
                {
                    return result;
                }

                using (var trans = DataContext.Database.BeginTransaction())
                {
                    try
                    {
                        foreach(ExceptionTypeWithFlowchartVM tempData in data.ExcetionTypes)
                        {
                            string sql = string.Format(@"
                                                        IF NOT EXISTS ( SELECT TOP 1
                                                                                    1
                                                                          FROM      dbo.ExceptionTypeWithFlowchart
                                                                          WHERE     FlowChart_Master_UID = {0}
                                                                                    AND Process_Seq = {5}
                                                                                    AND ExceptionType_UID = {1}
                                                                                    AND FunPlant = N'{2}'
                                                                                    AND TypeClassify = N'{3}' )
                                                            BEGIN
                                                                INSERT  INTO dbo.ExceptionTypeWithFlowchart
                                                                        ( FlowChart_Master_UID ,
                                                                          Process_Seq ,
                                                                          ExceptionType_UID ,
                                                                          FunPlant ,
                                                                          Creator_UID ,
                                                                          Creator_Date ,
                                                                          TypeClassify,
                                                                          FlowChart_Detail_UID
                                                                        )
                                                                VALUES  ( {0} , -- FlowChart_Master_UID - int
                                                                          {5} ,
                                                                          {1} , -- ExceptionType_UID - int
                                                                          N'{2}' , -- FunPlant - nvarchar(50)
                                                                          {4} , -- Creator_UID - int
                                                                          GETDATE() , -- Creator_Date - datetime
                                                                          N'{3}',  -- TypeClassify - nvarchar(50)
                                                                          {6}
                                                                        )
                                                            END
                                                        ", tempData.FlowChart_Master_UID, tempData.ExceptionType_UID, tempData.FunPlant,
                                                        tempData.TypeClassify, tempData.Creator_UID,tempData.Process_Seq,tempData.FlowChart_Detail_UID);

                            DataContext.Database.ExecuteSqlCommand(sql);
                        }

                        trans.Commit();
                    }
                    catch(Exception ex)
                    {
                        trans.Rollback();
                        result = "Error";
                        log.Error(ex);
                    }
                
                }
            }
            catch (Exception ex)
            {
                
                log.Error(ex);
            }
            return result;
        }

        public IQueryable<ExceptionTypeListVM> QueryExcepProcS(ExcepTypeFlowChartSearch search, Page page, out int count)
        {
            var query = from exceProcess in DataContext.ExceptionTypeWithFlowchart
                        join exceType in DataContext.QualityAssurance_ExceptionType on exceProcess.ExceptionType_UID equals exceType.ExceptionType_UID
                        join detail in DataContext.FlowChart_Detail on exceProcess.FlowChart_Detail_UID equals detail.FlowChart_Detail_UID
                        join master in DataContext.FlowChart_Master on detail.FlowChart_Master_UID equals master.FlowChart_Master_UID
                        join project in DataContext.System_Project on master.Project_UID equals project.Project_UID
                        join U in DataContext.System_Users on exceProcess.Creator_UID equals U.Account_UID
                        where detail.FlowChart_Version == master.FlowChart_Version && detail.FlowChart_Detail_UID==search.FlowChart_Detail_UID
                        select new ExceptionTypeListVM
                        {
                            ExceptionTypeWithFlowchart_UID = exceProcess.ExceptionTypeWithFlowchart_UID,
                            Project = project.Project_Name,
                            Process = detail.Process,
                            ExceptionType_Name = exceType.TypeName,
                            FunPlant = exceProcess.FunPlant,
                            TypeClassify = exceProcess.TypeClassify,
                            Creator_User = U.User_Name,
                            Process_Seq = exceProcess.Process_Seq,
                            FlowChart_Detail_UID=detail.FlowChart_Detail_UID,
                            FlowchartMaster_UID = exceProcess.FlowChart_Master_UID
                        };
            query = query.Distinct();

            if (!string.IsNullOrWhiteSpace(search.TypeName))
            {
                query = query.Where(p => p.ExceptionType_Name.Contains(search.TypeName));
            }
            if (!string.IsNullOrWhiteSpace(search.FunPlant))
            {
                query = query.Where(p => p.FunPlant.Contains(search.FunPlant));
            }
            if (!string.IsNullOrWhiteSpace(search.TypeClassify))
            {
                query = query.Where(p => p.TypeClassify == search.TypeClassify);
            }
            count = query.Count();

            return query.OrderBy(o => o.ExceptionType_Name).GetPage(page);
        }

        public string DeleteAllExceptionProcess(int FlowChart_Detail_UID)
        {
            string result = "OK";
            try
            {
                string sql = string.Format(@"DELETE FROM dbo.ExceptionTypeWithFlowchart WHERE FlowChart_Detail_UID={0}", FlowChart_Detail_UID);
                DataContext.Database.ExecuteSqlCommand(sql);
            }
            catch(Exception ex)
            {
                result = "Error happens when delete data from ExceptionTypeWithFlowchart /r/n" + ex.ToString();
                log.Error(ex);
            }
            return result;
        }
    }

    public interface IExceptionTypeWithFlowchartRepository : IRepository<ExceptionTypeWithFlowchart>
    {
        string AddTExceptionTypesoFlowChart(ExceptionTypesAddToFlowChartVM data);
        string DeleteAllExceptionProcess(int FlowChart_Detail_UID);
        IQueryable<ExceptionTypeListVM> QueryExcepProcS(ExcepTypeFlowChartSearch search, Page page, out int count);
    }

}

