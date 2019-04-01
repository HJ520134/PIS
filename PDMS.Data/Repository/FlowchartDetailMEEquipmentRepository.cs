using PDMS.Common.Helpers;
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
    //public interface IFlowchartDetailMEEquipmentRepository : IRepository<Flowchart_Detail_ME_Equipment>
    //{
    //    PagedListModel<EquipmentGet> QueryFLMEEquipmentDetail(int id, int Version, int Param);
    //}

    //public class FlowchartDetailMEEquipmentRepository : RepositoryBase<Flowchart_Detail_ME_Equipment>, IFlowchartDetailMEEquipmentRepository
    //{
    //    private Logger log = new Logger("FlowchartDetailMEEquipmentRepository");
    //    public FlowchartDetailMEEquipmentRepository(IDatabaseFactory databaseFactory)
    //        : base(databaseFactory)
    //    {

    //    }

    //    public PagedListModel<EquipmentGet> QueryFLMEEquipmentDetail(int id, int Version, int Param)
    //    {
    //        string sql = @"SELECT B.Flowchart_Detail_ME_UID,B.Binding_Seq,B.Process_Seq,B.Process,B.Capacity_ByDay,B.Capacity_ByHour,B.Process_Station,B.System_FunPlant_UID,C.FunPlant, B.Equipment_RequstQty,
    //                        A.Flowchart_Detail_ME_Equipment_UID,A.Equipment_Name,A.Equipment_Spec,A.Plan_CT,A.Current_CT,A.EquipmentQty,
    //                        A.Ratio,A.RequestQty,A.EquipmentType,A.Notes,A.NPI_CurrentQty,A.MP_CurrentQty,A.EquipmentQty, A.RequestQty
    //                        FROM dbo.Flowchart_Detail_ME_Equipment A
    //                        JOIN dbo.Flowchart_Detail_ME B ON A.Flowchart_Detail_ME_UID = B.Flowchart_Detail_ME_UID
    //                        JOIN dbo.System_Function_Plant C ON B.System_FunPlant_UID = C.System_FunPlant_UID
    //                        WHERE B.FlowChart_Master_UID={0} AND B.FlowChart_Version={1}";
    //        if (Param == 1)
    //        {
    //            sql = sql + " AND EquipmentType=N'主加工设备'";
    //        }
    //        else
    //        {
    //            sql = sql + " AND EquipmentType=N'自动化设备'";
    //        }

    //        sql = string.Format(sql, id, Version);
    //        var list = DataContext.Database.SqlQuery<EquipmentGet>(sql).ToList();
    //        return new PagedListModel<EquipmentGet>(0, list);
    //    }

    //}
}
