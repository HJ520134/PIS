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
    public interface IProductionResourcePlanRepository: IRepository<RP_Flowchart_Master>
    {
        IQueryable<DemissionRateAndWorkSchedule> GetDSPlanList(DRAWS_QueryParam searchModel, Page page, out int totalcount);
    }
}
