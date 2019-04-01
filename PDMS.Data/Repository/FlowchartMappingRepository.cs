using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels.ProductionPlanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    //public interface IFlowchartMappingRepository : IRepository<Flowchart_Mapping>
    //{
    //    PagedListModel<PPFlowchartDetailVM> FlowchartPPDetailList(int id, int Version);
    //}

    //public class FlowchartMappingRepository : RepositoryBase<Flowchart_Mapping>, IFlowchartMappingRepository
    //{
    //    public FlowchartMappingRepository(IDatabaseFactory databaseFactory)
    //        : base(databaseFactory)
    //    {

    //    }

    //    public PagedListModel<PPFlowchartDetailVM> FlowchartPPDetailList(int id, int Version)
    //    {
    //        string sql = @"SELECT A.*, B.Flowchart_Mapping_UID, A.Process_Station, A.System_FunPlant_UID, D.FunPlant, A.Process,
    //                        CASE WHEN C.FlowChart_Detail_UID IS NULL THEN  '' ELSE N'PP制程' END AS 'KeyProcess',
    //                        C.Process AS PP_Process,C.Rework_Flag,C.DRI,C.Place,
    //                        CASE C.IsQAProcess 
    //                        WHEN 'Inspect_IPQC' THEN N'IPQC全检'
    //                        WHEN 'Polling_IPQC' THEN N'IPQC巡检'
    //                        WHEN 'Inspect_OQC' THEN N'OQC检测'
    //                        WHEN 'Inspect_Assemble' THEN N'组装检测'
    //                        WHEN 'Inspect_Assemble,Inspect_OQC' THEN N'组装&OQC检测'
    //                        END AS IsQAProcess,
    //                        (SELECT top 1 
    //                        CASE WHEN CC.Process_Seq IS NULL THEN '' ELSE N'制程序号' + CONVERT(NVARCHAR(50),CC.Process_Seq)  END
    //                        FROM PP_Flowchart_Process_Mapping AA
    //                        JOIN Flowchart_Mapping BB
    //                        ON AA.Flowchart_Mapping_UID = BB.Flowchart_Mapping_UID
    //                        JOIN Flowchart_Detail_ME CC 
    //                        ON BB.Flowchart_Detail_ME_UID = CC.Flowchart_Detail_ME_UID
    //                        WHERE AA.Flowchart_Detail_ME_UID = A.Flowchart_Detail_ME_UID) AS 'KeyProcessSub'

    //                        FROM dbo.Flowchart_Detail_ME A
    //                        LEFT JOIN Flowchart_Mapping B
    //                        ON A.Flowchart_Detail_ME_UID = B.Flowchart_Detail_ME_UID
    //                        LEFT JOIN dbo.FlowChart_Detail C
    //                        ON B.FlowChart_Detail_UID = C.FlowChart_Detail_UID
    //                        JOIN dbo.System_Function_Plant D
    //                        ON A.System_FunPlant_UID = D.System_FunPlant_UID
    //                        WHERE A.FlowChart_Master_UID={0} AND A.FlowChart_Version={1}";
    //        sql = string.Format(sql, id, Version);
    //        var list = DataContext.Database.SqlQuery<PPFlowchartDetailVM>(sql).ToList();
    //        return new PagedListModel<PPFlowchartDetailVM>(0, list);
    //    }
    //}
}
