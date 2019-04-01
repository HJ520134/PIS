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

    public class QualityAssurance_ExceptionType_TempRepository : RepositoryBase<QualityAssurance_ExceptionType_Temp>, IQualityAssurance_ExceptionType_TempRepository
    {
        private Logger log = new Logger("QualityAssurance_ExceptionType_TempRepository");

        public QualityAssurance_ExceptionType_TempRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public string ExecSPImportData()
        {
            string result = "";

            try
            {

                IEnumerable<SPReturnMessage> topTenRate = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<SPReturnMessage>(@"usp_ImportExceptionType").ToArray();

                if (topTenRate.Count() != 0)
                {
                    return topTenRate.ToList()[0].Message;
                }
            }
            catch(Exception ex)
            {
                result = "Error";
                log.Error(ex);
            }

            return result;
        }

        public string getProjectNamebyMasterUid(int uid)
        {
            var query = from fm in DataContext.FlowChart_Master
                        join sp in DataContext.System_Project
                        on fm.Project_UID equals sp.Project_UID
                        where fm.FlowChart_Master_UID == uid
                        select sp.Project_Name;
            return query.FirstOrDefault();
        }
    }

   

    public interface IQualityAssurance_ExceptionType_TempRepository : IRepository<QualityAssurance_ExceptionType_Temp>
    {
        string ExecSPImportData();
        string getProjectNamebyMasterUid(int uid);
    }



}
