using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IOEE_ImprovementPlanDRepository : IRepository<OEE_ImprovementPlanD>
    {
        PagedListModel<OEE_MetricInfoDTO> QueryMetricInfoInfo(OEE_MetricInfoDTO serchModel, Page page);
    }

    public class OEE_ImprovementPlanDRepository : RepositoryBase<OEE_ImprovementPlanD>, IOEE_ImprovementPlanDRepository
    {
        public OEE_ImprovementPlanDRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public PagedListModel<OEE_MetricInfoDTO> QueryMetricInfoInfo(OEE_MetricInfoDTO serchModel, Page page)
        {
            var query = from MetricInfo in DataContext.OEE_MetricInfo
                        select new OEE_MetricInfoDTO
                        {
                            Metric_UID = MetricInfo.Metric_UID,
                            Plant_Organization_UID = MetricInfo.Plant_Organization_UID,
                            Plant_Organization_Name = MetricInfo.System_Organization2.Organization_Name,
                            BG_Organization_UID = MetricInfo.BG_Organization_UID,
                            BG_Organization_Name = MetricInfo.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = MetricInfo.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = MetricInfo.System_Organization.Organization_Name,
                            Metric_ID = MetricInfo.Metric_ID,
                            Metric_Name = MetricInfo.Metric_Name,
                            Modified_UID = MetricInfo.Modified_UID,
                            Modified_Date = MetricInfo.Modified_Date,
                        };

            //厂区
            if (serchModel.Plant_Organization_UID != 0)
            {
                query = query.Where(p => p.Plant_Organization_UID == serchModel.Plant_Organization_UID);
            }

            //OP
            if (serchModel.BG_Organization_UID != 0)
            {
                query = query.Where(p => p.BG_Organization_UID == serchModel.BG_Organization_UID);
            }

            //功能厂
            if (serchModel.FunPlant_Organization_UID != 0 && serchModel.FunPlant_Organization_UID != null)
            {
                query = query.Where(p => p.FunPlant_Organization_UID == serchModel.FunPlant_Organization_UID);
            }

            //会议ID
            if (!string.IsNullOrEmpty(serchModel.Metric_ID))
            {
                query = query.Where(p => p.Metric_ID == serchModel.Metric_ID);
            }

            //会议名称
            if (!string.IsNullOrEmpty(serchModel.Metric_Name))
            {
                query = query.Where(p => p.Metric_Name == serchModel.Metric_Name);
            }

            var totalCount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return new PagedListModel<OEE_MetricInfoDTO>(totalCount, query.ToList());
        }

    }
}
