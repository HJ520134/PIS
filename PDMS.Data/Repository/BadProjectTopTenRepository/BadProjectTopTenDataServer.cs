using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public class BadProjectTopTenDataServer : RepositoryBase<QEboardSumModel>, IBadProjectTopTenDataServer
    {
        public BadProjectTopTenDataServer(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<QEboardSumModel> GteQEboardSumDetailData(string Projects)
        {
            string sql = @" SELECT
	                        [QEboadSum_UID],
	                        [FlowChartMaster_UID],
	                        [Project],
	                        [Part_Types],
	                        [Product_Date],
	                        [Time_Interval],
	                        [Process_Seq],
	                        [Process],
	                        [OneCheck_QTY],
	                        [OneCheck_OK],
	                        [NGReuse],
	                        [NGReject],
	                        [OneTargetYield],
	                        [OneYield],
	                        [RepairOK],
	                        [SecondTargetYield],
	                        [SecondYield]
                            FROM
	                        [dbo].[QEboardSum]";
            sql += $" where Project={Projects}";
            var dblist = DataContext.Database.SqlQuery<QEboardSumModel>(sql).ToList();
            return dblist;
        }
    }
}
