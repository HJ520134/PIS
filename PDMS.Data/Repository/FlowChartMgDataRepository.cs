using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;

namespace PDMS.Data.Repository
{
    public interface IFlowChartMgDataRepository : IRepository<FlowChart_MgData>
    {
        int GetProcessPlan(int projectUID,string Process, DateTime date);
    }

 

    public class FlowChartMgDataRepository : RepositoryBase<FlowChart_MgData>, IFlowChartMgDataRepository
    {
        public FlowChartMgDataRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public int GetProcessPlan(int projectUID,string Process, DateTime date)
        {
            var query = from mg in DataContext.FlowChart_MgData
                        where mg.FlowChart_Detail.FlowChart_Master_UID == projectUID
                        && mg.Product_Date == date && mg.FlowChart_Detail.Process == Process
                        orderby mg.FlowChart_MgData_UID descending
                        select mg.Product_Plan;
            return query.FirstOrDefault();
        }
    }
}
