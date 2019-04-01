using PDMS.Common.Helpers;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels.ProductionPlanning;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    //public interface IFlowchartDetailMERepository : IRepository<Flowchart_Detail_ME>
    //{
    //    DataTable QueryEquipRPT(ProductionPlanningReportVM item, out int totalCount);
    //    List<ProductionPlanningReportSearchVM> QueryEquipRPTByOPType(ProductionPlanningReportVM item);
    //    List<string> GetRPTColumnName(int PlantUID, int OpTypeUID);
    //    List<ReportByProject> QueryEquipRPTBySingleProject(int Organization_UID, int System_FunPlant_UID, string Equipment_Name, string time);
    //    List<DataTable> QueryEquipRPT_ByProject(ProductionPlanningReportVM item);
    //    List<string> GetColumn_ByProject(ProductionPlanningReportVM item);
    //    DataTable QueryEquipRPTByFunc(ReportByProject item);

    //    DataTable QueryPlanAndActualReportInfo(ProductionPlanningReportVM item);
    //}

    //public class FlowchartDetailMERepository : RepositoryBase<Flowchart_Detail_ME>, IFlowchartDetailMERepository
    //{
    //    private Logger log = new Logger("FlowChartDetailMERepository");
    //    public FlowchartDetailMERepository(IDatabaseFactory databaseFactory)
    //        : base(databaseFactory)
    //    {

    //    }

    //    public List<string> GetRPTColumnName(int PlantUID, int OpTypeUID)
    //    {
    //        List<string> columnName = new List<string>();
    //        columnName.Add("功能厂");
    //        columnName.Add("设备");

    //        //多个OPType的情况
    //        if (OpTypeUID == 0)
    //        {
    //            string sql = @"SELECT Organization_Name FROM dbo.System_Organization
    //                            WHERE Organization_UID IN (
    //                            SELECT ChildOrg_UID FROM dbo.System_OrganizationBOM WHERE ParentOrg_UID={0}
    //                            )";
    //            sql = string.Format(sql, PlantUID);
    //            var opList = DataContext.Database.SqlQuery<string>(sql).ToList();

    //            foreach (var opItem in opList)
    //            {
    //                columnName.Add(opItem + "现有数量");
    //            }
    //            columnName.Add("主设备汇总");
    //            foreach (var opItem in opList)
    //            {
    //                columnName.Add(opItem + "需求数量");
    //            }
    //            columnName.Add("需求汇总");
    //            foreach (var opItem in opList)
    //            {
    //                columnName.Add(opItem + "差异数量");
    //            }
    //            columnName.Add("差异汇总");
    //        }
    //        else
    //        {
    //            columnName.Add("MP设备");
    //            columnName.Add("NPI设备");
    //            columnName.Add("主设备汇总");
    //            columnName.Add("需求汇总");
    //            columnName.Add("差异");
    //        }
    //        return columnName;
    //    }

    //    public DataTable QueryEquipRPT(ProductionPlanningReportVM item, out int totalCount)
    //    {
    //        List<ProductionPlanningReportSearchVM> list = new List<ProductionPlanningReportSearchVM>();
    //        ProductionPlanningReportSearchVM vmItem = new ProductionPlanningReportSearchVM();
    //        list.Add(vmItem);

    //        var PlantUID = new SqlParameter("@PlantUID", item.PlantUID);
    //        var OpTypeUID = new SqlParameter("@OpTypeUID", item.OpTypeUID);
    //        var ProjectUID = new SqlParameter("@ProjectUID", item.ProjectUID);
    //        var PartTypeUID = new SqlParameter("@PartTypeUID", item.PartTypeUID);
    //        var ReportType = new SqlParameter("@ReportType", item.ReportType);
    //        var StartDate = new SqlParameter("@StartDate", item.StartDate);
    //        var EndDate = new SqlParameter("@EndDate", item.EndDate);
    //        //var DateFrom = new SqlParameter("@DateFrom", item.DateFrom);
    //        var QueryMode = new SqlParameter("@QueryMode", item.QueryMode);

    //        totalCount = 0;
    //        DataTable dt = new DataTable();
    //        var conStr = MethodExtension.GetConnectionStr();
    //        using (SqlConnection con = new SqlConnection(conStr))
    //        {
    //            con.Open();
    //            SqlCommand cmd = null;
    //            cmd = new SqlCommand("RPT_EquipRequest_By_AllOP", con);
    //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            cmd.Parameters.Add(new SqlParameter("@PlantUID", item.PlantUID));
    //            cmd.Parameters.Add(new SqlParameter("@StartDate", Convert.ToDateTime(item.StartDate)));
    //            cmd.Parameters.Add(new SqlParameter("@EndDate", Convert.ToDateTime(item.EndDate)));
    //            cmd.Parameters.Add(new SqlParameter("@QueryMode", item.QueryMode));

    //            using (SqlDataReader sdr = cmd.ExecuteReader())
    //            {
    //                dt.Load(sdr);
    //                sdr.Close();
    //            }
    //        }
    //        return dt;


            
    //        //if (item.OpTypeUID == 0) //统计整个厂区的情况
    //        //{
    //        //    var tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ProductionPlanningReportSearchVM>(
    //        //        "RPT_EquipRequest_By_AllOP @PlantUID,@OpTypeUID,@ProjectUID,@PartTypeUID,@ReportType,@StartDate,@EndDate,@QueryMode",
    //        //        PlantUID, OpTypeUID, ProjectUID, PartTypeUID, ReportType, StartDate, EndDate, QueryMode).ToArray();
    //        //    var result = tempResult.ToList();
    //        //    ////现有数量列表
    //        //    //var currentQtyList = result.Where(m => m.QtyMode == "RealQty").ToList();
    //        //    ////需求数量列表
    //        //    //var requestQtyList = result.Where(m => m.QtyMode == "RequestQty").ToList();

    //        //    //List<string> DimensionList = new List<string>() { "FunPlant", "Equipment_Name", "Product_Date", "QtyMode" };
    //        //    //string DynamicColumn = "Optype";
    //        //    //List<string> AllDynamicColumn = null;
    //        //    //List<dynamic> dynamicCurrentQtyList = DynamicLinq(currentQtyList, DimensionList, DynamicColumn, out AllDynamicColumn);
    //        //    //List<dynamic> dynamicRequestQtyList = DynamicLinq(requestQtyList, DimensionList, DynamicColumn, out AllDynamicColumn);

    //        //    return result;

    //        //}
    //        //else if (item.ProjectUID == 0) //统计单个OP的情况
    //        //{
    //        //    var tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ProductionPlanningReportSearchVM>(
    //        //        "RPT_EquipRequest_By_SingleOP_AllProject @PlantUID,@OpTypeUID,@ProjectUID,@PartTypeUID,@ReportType,@StartDate,@EndDate,@QueryMode",
    //        //        PlantUID, OpTypeUID, ProjectUID, PartTypeUID, ReportType, StartDate, EndDate, QueryMode).ToArray();
    //        //    var result = tempResult.ToList();
    //        //    return result;
    //        //}
    //        //else //统计单个专案的情况
    //        //{
    //        //    return null;
    //        //}
    //    }

    //    public List<ProductionPlanningReportSearchVM> QueryEquipRPTByOPType(ProductionPlanningReportVM item)
    //    {
    //        List<ProductionPlanningReportSearchVM> list = new List<ProductionPlanningReportSearchVM>();
    //        ProductionPlanningReportSearchVM vmItem = new ProductionPlanningReportSearchVM();
    //        list.Add(vmItem);

    //        var PlantUID = new SqlParameter("@PlantUID", item.PlantUID);
    //        var OpTypeUID = new SqlParameter("@OpTypeUID", item.OpTypeUID);
    //        var ProjectUID = new SqlParameter("@ProjectUID", item.ProjectUID);
    //        var PartTypeUID = new SqlParameter("@PartTypeUID", item.PartTypeUID);
    //        var ReportType = new SqlParameter("@ReportType", item.ReportType);
    //        var StartDate = new SqlParameter("@StartDate", item.StartDate);
    //        var EndDate = new SqlParameter("@EndDate", item.EndDate);
    //        //var DateFrom = new SqlParameter("@DateFrom", item.DateFrom);
    //        var QueryMode = new SqlParameter("@QueryMode", item.QueryMode);

    //        var tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ProductionPlanningReportSearchVM>(
    //            "RPT_EquipRequest_By_SingleOP_AllProject @PlantUID,@OpTypeUID,@ProjectUID,@PartTypeUID,@ReportType,@StartDate,@EndDate,@QueryMode",
    //            PlantUID, OpTypeUID, ProjectUID, PartTypeUID, ReportType, StartDate, EndDate, QueryMode).ToArray();
    //        var result = tempResult.ToList();
    //        return result;

    //    }


    //    public List<string> GetColumn_ByProject(ProductionPlanningReportVM item)
    //    {
    //        var PlantUID = new SqlParameter("@PlantUID", item.PlantUID);
    //        var OpTypeUID = new SqlParameter("@OpTypeUID", item.OpTypeUID);
    //        var ProjectUID = new SqlParameter("@ProjectUID", item.ProjectUID);
    //        var PartTypeUID = new SqlParameter("@PartTypeUID", item.PartTypeUID);
    //        var ReportType = new SqlParameter("@ReportType", item.ReportType);
    //        var StartDate = new SqlParameter("@StartDate", item.StartDate);
    //        var EndDate = new SqlParameter("@EndDate", item.EndDate);
    //        //var DateFrom = new SqlParameter("@DateFrom", item.DateFrom);
    //        var QueryMode = new SqlParameter("@QueryMode", item.QueryMode);
    //        var tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ReportBy_SingleProject>(
    //"RPT_EquipRequest_By_SingleOP_SingleProject @PlantUID,@OpTypeUID,@ProjectUID,@PartTypeUID,@ReportType,@StartDate,@EndDate,@QueryMode",
    //PlantUID, OpTypeUID, ProjectUID, PartTypeUID, ReportType, StartDate, EndDate, QueryMode).ToArray();
    //        var result = tempResult.ToList();
    //        return result.Select(m => m.Product_Date).Distinct().OrderBy(m => m).ToList();
    //    }

    //    public List<DataTable> QueryEquipRPT_ByProject(ProductionPlanningReportVM item)
    //    {
    //        var PlantUID = new SqlParameter("@PlantUID", item.PlantUID);
    //        var OpTypeUID = new SqlParameter("@OpTypeUID", item.OpTypeUID);
    //        var ProjectUID = new SqlParameter("@ProjectUID", item.ProjectUID);
    //        var PartTypeUID = new SqlParameter("@PartTypeUID", item.PartTypeUID);
    //        var ReportType = new SqlParameter("@ReportType", item.ReportType);
    //        var StartDate = new SqlParameter("@StartDate", item.StartDate);
    //        var EndDate = new SqlParameter("@EndDate", item.EndDate);
    //        //var DateFrom = new SqlParameter("@DateFrom", item.DateFrom);
    //        var QueryMode = new SqlParameter("@QueryMode", item.QueryMode);

    //        List<DataTable> dtList = new List<DataTable>();
    //        var conStr = MethodExtension.GetConnectionStr();
    //        using (SqlConnection con = new SqlConnection(conStr))
    //        {
    //            con.Open();
    //            SqlCommand cmd = null;
    //            for (int i = 0; i < 2; i++)
    //            {
    //                DataTable dt = new DataTable();
    //                switch (item.QueryMode)
    //                {
    //                    case 1:
    //                        if (i == 0)
    //                        {
    //                            cmd = new SqlCommand("RPT_EquipRequest_By_SingleOP_SingleProject_InputMonth", con);
    //                        }
    //                        else
    //                        {
    //                            cmd = new SqlCommand("RPT_EquipRequest_By_SingleOP_SingleProject_EquipMonth", con);
    //                        }
    //                        break;
    //                    case 2:
    //                        if (i == 0)
    //                        {
    //                            cmd = new SqlCommand("RPT_EquipRequest_By_SingleOP_SingleProject_InputWeek", con);
    //                        }
    //                        else
    //                        {
    //                            cmd = new SqlCommand("RPT_EquipRequest_By_SingleOP_SingleProject_EquipWeek", con);
    //                        }
    //                        break;
    //                }

    //                //SqlCommand cmd = new SqlCommand("RPT_EquipRequest_By_SingleOP_SingleProject_InputMonth", con);
    //                cmd.CommandType = System.Data.CommandType.StoredProcedure;
    //                cmd.Parameters.Add(new SqlParameter("@PlantUID", item.PlantUID));
    //                cmd.Parameters.Add(new SqlParameter("@OpTypeUID", item.OpTypeUID));
    //                cmd.Parameters.Add(new SqlParameter("@ProjectUID", item.ProjectUID));
    //                cmd.Parameters.Add(new SqlParameter("@PartTypeUID", item.PartTypeUID));
    //                cmd.Parameters.Add(new SqlParameter("@ReportType", item.ReportType));
    //                cmd.Parameters.Add(new SqlParameter("@StartDate", item.StartDate));
    //                cmd.Parameters.Add(new SqlParameter("@EndDate", item.EndDate));
    //                cmd.Parameters.Add(new SqlParameter("@QueryMode", item.QueryMode));

    //                using (SqlDataReader sdr = cmd.ExecuteReader())
    //                {
    //                    dt.Load(sdr);
    //                    sdr.Close();
    //                }
    //                dtList.Add(dt);
    //            }
    //        }

    //        return dtList;
    //    }

    //    public List<ReportByProject> QueryEquipRPTBySingleProject(int Organization_UID, int System_FunPlant_UID, string Equipment_Name, string time)
    //    {
    //        var OpTypeUID = new SqlParameter("@OpTypeUID", Organization_UID);
    //        var funPlantUID = new SqlParameter("@System_FunPlant_UID", System_FunPlant_UID);
    //        var equipName = new SqlParameter("@Equipment_Name", Equipment_Name);
    //        var date = new SqlParameter("@time", time);

    //        var tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ReportByProject>(
    //            "RPT_EquipRequest_By_SingleOP_AllProject_GroupProject @OpTypeUID,@System_FunPlant_UID,@Equipment_Name,@time",
    //            OpTypeUID, funPlantUID, equipName, date).ToArray();
    //        var result = tempResult.ToList();
    //        return result;

    //    }


    //    public DataTable QueryEquipRPTByFunc(ReportByProject item)
    //    {
    //        DataTable dt = new DataTable();
    //        var conStr = MethodExtension.GetConnectionStr();
    //        using (SqlConnection con = new SqlConnection(conStr))
    //        {
    //            con.Open();
    //            SqlCommand cmd = null;
    //            cmd = new SqlCommand("RPT_EquipRequest_By_SingleOP_SingleProject_Func_MonthAndWeek", con);
    //            //SqlCommand cmd = new SqlCommand("RPT_EquipRequest_By_SingleOP_SingleProject_InputMonth", con);
    //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            cmd.Parameters.Add(new SqlParameter("@OpTypeUID", item.Organization_UID));
    //            cmd.Parameters.Add(new SqlParameter("@PartTypeUID", item.FlowChart_Master_UID));
    //            cmd.Parameters.Add(new SqlParameter("@Equipment_Name", item.Equipment_Name));
    //            cmd.Parameters.Add(new SqlParameter("@StartDate", Convert.ToDateTime(item.time)));
    //            cmd.Parameters.Add(new SqlParameter("@EndDate", Convert.ToDateTime(item.time)));
    //            cmd.Parameters.Add(new SqlParameter("@QueryMode", item.QueryMode));

    //            using (SqlDataReader sdr = cmd.ExecuteReader())
    //            {
    //                dt.Load(sdr);
    //                sdr.Close();
    //            }
    //        }
    //        return dt;

    //    }


    //    #region 实际和预估对比报表
    //    public DataTable QueryPlanAndActualReportInfo(ProductionPlanningReportVM item)
    //    {
    //        DataTable dt = new DataTable();
    //        var conStr = MethodExtension.GetConnectionStr();
    //        using (SqlConnection con = new SqlConnection(conStr))
    //        {
    //            con.Open();
    //            SqlCommand cmd = null;
    //            cmd = new SqlCommand("RPT_PlanAndActual_ByEquip", con);
    //            //SqlCommand cmd = new SqlCommand("RPT_EquipRequest_By_SingleOP_SingleProject_InputMonth", con);
    //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            cmd.Parameters.Add(new SqlParameter("@PlantUID", item.PlantUID));
    //            cmd.Parameters.Add(new SqlParameter("@OpTypeUID", item.OpTypeUID));
    //            cmd.Parameters.Add(new SqlParameter("@ProjectUID", item.ProjectUID));
    //            cmd.Parameters.Add(new SqlParameter("@PartTypeUID", item.PartTypeUID));
    //            cmd.Parameters.Add(new SqlParameter("@DateFrom", Convert.ToDateTime(item.StartDate)));
    //            cmd.Parameters.Add(new SqlParameter("@DateEnd", Convert.ToDateTime(item.EndDate)));

    //            using (SqlDataReader sdr = cmd.ExecuteReader())
    //            {
    //                dt.Load(sdr);
    //                sdr.Close();
    //            }
    //        }
    //        return dt;
    //    }
    //    #endregion




    //    /// <summary>
    //    /// 动态Linq方式实现行转列
    //    /// </summary>
    //    /// <param name="list">数据</param>
    //    /// <param name="DimensionList">维度列</param>
    //    /// <param name="DynamicColumn">动态列</param>
    //    /// <returns>行转列后数据</returns>
    //    public static List<dynamic> DynamicLinq<T>(List<T> list, List<string> DimensionList, string DynamicColumn, out List<string> AllDynamicColumn) where T : class
    //    {
    //        //获取所有动态列
    //        var columnGroup = list.GroupBy(DynamicColumn, "new(it as Vm)") as IEnumerable<IGrouping<dynamic, dynamic>>;
    //        List<string> AllColumnList = new List<string>();
    //        foreach (var item in columnGroup)
    //        {
    //            if (!string.IsNullOrEmpty(item.Key))
    //            {
    //                AllColumnList.Add(item.Key);
    //            }
    //        }
    //        AllDynamicColumn = AllColumnList;
    //        var dictFunc = new Dictionary<string, Func<T, bool>>();
    //        foreach (var column in AllColumnList)
    //        {
    //            var func = DynamicExpression.ParseLambda<T, bool>(string.Format("{0}==\"{1}\"", DynamicColumn, column)).Compile();
    //            dictFunc[column] = func;
    //        }

    //        //获取实体所有属性
    //        Dictionary<string, PropertyInfo> PropertyInfoDict = new Dictionary<string, PropertyInfo>();
    //        Type type = typeof(T);
    //        var propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
    //        //数值列
    //        List<string> AllNumberField = new List<string>();
    //        foreach (var item in propertyInfos)
    //        {
    //            PropertyInfoDict[item.Name] = item;
    //            if (item.PropertyType == typeof(int) || item.PropertyType == typeof(double) || item.PropertyType == typeof(float))
    //            {
    //                AllNumberField.Add(item.Name);
    //            }
    //        }

    //        //分组
    //        var dataGroup = list.GroupBy(string.Format("new ({0})", string.Join(",", DimensionList)), "new(it as Vm)") as IEnumerable<IGrouping<dynamic, dynamic>>;
    //        List<dynamic> listResult = new List<dynamic>();
    //        IDictionary<string, object> itemObj = null;
    //        T vm2 = default(T);
    //        foreach (var group in dataGroup)
    //        {
    //            itemObj = new ExpandoObject();
    //            var listVm = group.Select(e => e.Vm as T).ToList();
    //            //维度列赋值
    //            vm2 = listVm.FirstOrDefault();
    //            foreach (var key in DimensionList)
    //            {
    //                itemObj[key] = PropertyInfoDict[key].GetValue(vm2);
    //            }

    //            foreach (var column in AllColumnList)
    //            {
    //                vm2 = listVm.FirstOrDefault(dictFunc[column]);
    //                if (vm2 != null)
    //                {
    //                    foreach (string name in AllNumberField)
    //                    {
    //                        itemObj[name + column] = PropertyInfoDict[name].GetValue(vm2);
    //                    }
    //                }
    //            }
    //            listResult.Add(itemObj);
    //        }
    //        return listResult;
    //    }

    //}
}
