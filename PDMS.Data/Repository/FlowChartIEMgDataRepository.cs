using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFlowChartIEMgDataRepository : IRepository<FlowChart_IEData>
    {
        int? GetProcessPlan(int projectUID, string Process, DateTime date);
    }
    public class FlowChartIEMgDataRepository : RepositoryBase<FlowChart_IEData>, IFlowChartIEMgDataRepository
    {
        public FlowChartIEMgDataRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public int? GetProcessPlan(int projectUID, string Process, DateTime date)
        {
            var query = from mg in DataContext.FlowChart_IEData
                        where mg.FlowChart_Detail.FlowChart_Master_UID == projectUID
                        && mg.IE_TargetDate == date && mg.FlowChart_Detail.Process == Process
                        orderby mg.FlowChart_Detail_UID descending
                        select mg.IE_TargetEfficacy;
           
                return query.FirstOrDefault();
            
      

        }
    }
}
