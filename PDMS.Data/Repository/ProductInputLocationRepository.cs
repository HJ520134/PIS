using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model;

namespace PDMS.Data.Repository
{
    public interface IProductInputLocationRepository : IRepository<Product_Input_Location>
    {
        List<ProductLocationItem> QueryProductInputLocation(ProductInputLocationSearch search, Page page);

        /// <summary>
        /// 通过制程序号获取
        /// </summary>
        List<ProductLocationItem> GetPDInputLocationByProSeqAPI(PDByProSeqSearch searchModel, Page page);

    }

    public class ProductInputLocationRepository : RepositoryBase<Product_Input_Location>, IProductInputLocationRepository
    {
        public ProductInputLocationRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }


        /// <summary>
        /// 通过制程序号获取
        /// </summary>
        public List<ProductLocationItem> GetPDInputLocationByProSeqAPI(PDByProSeqSearch search, Page page)
        {
            List<ProductLocationItem> resultList = new List<ProductLocationItem>();

            using (var context = new SPPContext())
            {
                if (search.Time_Interval == "全天" || search.Time_Interval == "ALL")
                {
                    var strSql = @"        
                                  SELECT
	                                    p.Good_QTY,
	                                    p.Picking_QTY,
	                                    p.WH_Picking_QTY,
	                                    p.NG_QTY,
	                                    p.WH_QTY,
	                                    p.WIP_QTY,
	                                    p.Place,
                                        p.Color,
	                                    p.Time_Interval
                                    FROM
	                                    Product_Input_Location p
                                    JOIN dbo.FlowChart_Detail f ON f.FlowChart_Detail_UID = p.FlowChart_Detail_UID
                                   
                                  ";
                    var paramWhere =
                    $" WHERE  Is_Comfirm = 1 AND p.Project = N'{search.project}' AND p.Product_Phase = N'{search.Product_Phase}'  AND p.Customer = N'{search.customer}'   AND p.Part_Types = N'{search.part_types}' AND p.Process = N'{search.Process_Seq}' AND p.Product_Date = '{search.input_date}'";
                    strSql = strSql + paramWhere;
                    var result = DataContext.Database.SqlQuery<ProductLocationItem>(strSql).ToList();
                    var timeInterValModelList = GetMaxTimeInterVal(search.optype);
                    //1 按楼栋分组
                    var PlaceDic = result.GroupBy(p => p.Place).ToDictionary(p => p.Key, q => q);
                    foreach (var pdItems in PlaceDic)
                    {
                        //2 按颜色分组
                        var colorDic = pdItems.Value.GroupBy(p => p.Color).ToDictionary(p => p.Key, q => q);
                      
                        int? WIP_QTY = 0;
                        foreach (var coloritem in colorDic)
                        {
                            var timeIntervalDic = new Dictionary<int, int?>();
                            foreach (var itemVal in coloritem.Value)
                            {
                                var timeInterModel =
                                timeInterValModelList.FirstOrDefault(p => p.Enum_Value == itemVal.Time_Interval);
                                if (timeInterModel != null)
                                    timeIntervalDic.Add(int.Parse(timeInterModel.Enum_Name), itemVal.WIP_QTY);
                            }
                            WIP_QTY += timeIntervalDic.OrderByDescending(p => p.Key).FirstOrDefault().Value;
                        }

                        var resultModel = new ProductLocationItem()
                        {
                            Good_QTY = pdItems.Value.Sum(p => p.Good_QTY),
                            Picking_QTY = pdItems.Value.Sum(p => p.Picking_QTY),
                            WH_Picking_QTY = pdItems.Value.Sum(p => p.WH_Picking_QTY),
                            NG_QTY = pdItems.Value.Sum(p => p.NG_QTY),
                            WH_QTY = pdItems.Value.Sum(p => p.WH_QTY),
                            Place = pdItems.Key,
                            WIP_QTY = WIP_QTY,
                        };

                        resultList.Add(resultModel);
                    }
                }
                else
                {
                    var strSql = @"        
                                    SELECT
	                                SUM ([Good_QTY]) AS Good_QTY,
	                                SUM ([Picking_QTY]) AS Picking_QTY,
	                                SUM ([WH_Picking_QTY]) AS WH_Picking_QTY,
	                                SUM ([NG_QTY]) AS NG_QTY,
	                                SUM ([WH_QTY]) AS WH_QTY,
	                                sum (p.WIP_QTY) AS WIP_QTY,
	                                p.Place
                                FROM
	                                Product_Input_Location p
                                JOIN dbo.FlowChart_Detail f ON f.FlowChart_Detail_UID = p.FlowChart_Detail_UID
                                WHERE
	                                Is_Comfirm = 1
                                AND p.Project =N'{0}'
                                AND p.Product_Phase = N'{1}'
                                AND p.Customer = N'{2}'
                                AND p.Part_Types = N'{3}'
                                AND p.Process = N'{4}'
                                AND p.Product_Date = '{5}'
                                AND p.Time_Interval = '{6}'
                                GROUP BY
	                                p.Place
                                  ";
                    strSql = string.Format(strSql, search.project, search.Product_Phase, search.customer,
                        search.part_types, search.Process_Seq, search.input_date, search.Time_Interval);
                    var query = DataContext.Database.SqlQuery<ProductLocationItem>(strSql).ToList();

                    if (page != null)
                    {
                        query = query.Skip(page.Skip).Take(page.PageSize).ToList();
                    }
                    resultList = query;
                }

                return resultList;
            }
        }

        public List<ProductLocationItem> QueryProductInputLocation(ProductInputLocationSearch search, Page page)
        {
            List<ProductLocationItem> resultList = new List<ProductLocationItem>();
            using (var context = new SPPContext())
            {
                if (search.Time_Interval == "全天" || search.Time_Interval == "ALL")
                {
                    var strSql = @"        
                                  SELECT
	                                    p.Good_QTY,
	                                    p.Picking_QTY,
	                                    p.WH_Picking_QTY,
	                                    p.NG_QTY,
	                                    p.WH_QTY,
	                                    p.WIP_QTY,
	                                    p.Place,
	                                    p.Time_Interval,
                                        p.Unacommpolished_Reason

                                    FROM
	                                    Product_Input_Location p
                                    JOIN dbo.FlowChart_Detail f ON f.FlowChart_Detail_UID = p.FlowChart_Detail_UID
                                    
                                  ";

                    var date = Convert.ToDateTime(search.Product_Date);

                    var paramwhere =
                        $" WHERE Is_Comfirm = 1  AND p.FlowChart_Master_UID ={search.FlowChart_Master_UID}  AND p.Process = N'{search.Process}'   AND p.Color = N'{search.Color}' AND Product_Date = '{date}'";
                    strSql = strSql + paramwhere;

                    var result = DataContext.Database.SqlQuery<ProductLocationItem>(strSql).ToList();
                    var timeInterValModelList = GetMaxTimeInterVal(search.opType);
                    var PlaceDic = result.GroupBy(p => p.Place).ToDictionary(p => p.Key, q => q);

                    foreach (var pdItems in PlaceDic)
                    {
                        var timeIntervalDic = new Dictionary<int, int?>();
                        foreach (var item in pdItems.Value)
                        {
                            var timeInterModel = timeInterValModelList.Where(p => p.Enum_Value == item.Time_Interval).FirstOrDefault();
                            timeIntervalDic.Add(int.Parse(timeInterModel.Enum_Name), item.WIP_QTY);
                        }
                        var WIP_QTY = timeIntervalDic.OrderByDescending(p => p.Key).FirstOrDefault().Value;
                        var resultModel = new ProductLocationItem()
                        {
                            Good_QTY = pdItems.Value.Sum(p => p.Good_QTY),
                            Picking_QTY = pdItems.Value.Sum(p => p.Picking_QTY),
                            WH_Picking_QTY = pdItems.Value.Sum(p => p.WH_Picking_QTY),
                            NG_QTY = pdItems.Value.Sum(p => p.NG_QTY),
                            WH_QTY = pdItems.Value.Sum(p => p.WH_QTY),
                            Place = pdItems.Key,
                            WIP_QTY = WIP_QTY
                        };
                        resultList.Add(resultModel);
                    }
                }
                else
                {
                    var query = from l in context.Product_Input_Location
                                where l.FlowChart_Master_UID == search.FlowChart_Master_UID
                                && l.Color == search.Color && l.Process == search.Process
                                && l.Product_Date == search.Product_Date && l.Time_Interval == search.Time_Interval
                                select new ProductLocationItem
                                {
                                    Place = l.Place,
                                    Picking_QTY = l.Picking_QTY,
                                    Good_QTY = l.Good_QTY,
                                    NG_QTY = l.NG_QTY,
                                    WH_Picking_QTY = l.WH_Picking_QTY,
                                    WH_QTY = l.WH_QTY,
                                    Adjust_QTY = l.Adjust_QTY,
                                    WIP_QTY = l.WIP_QTY,
                                    Unacommpolished_Reason=l.Unacommpolished_Reason
                                };

                    if (page != null)
                    {
                        query = query.Skip(page.Skip).Take(page.PageSize);
                    }
                    resultList = query.ToList();

                }
                return resultList;
            }
        }

        /// <summary>
        /// 获取枚举表配置的时间段
        /// </summary>
        /// <param name="opType"></param>
        /// <returns></returns>
        public List<EnumTimeInterVal> GetMaxTimeInterVal(string opType)
        {
            string strSql = @"SELECT
	                            Enum_UID,
	                            Enum_Type,
	                            Enum_Name,
	                            Enum_Value,
	                            Decription
                            FROM
	                            Enumeration
                          ";
            var param = $"  WHERE Enum_Type = 'Time_InterVal_{opType}'";
            strSql = strSql + param;
            var dblist = DataContext.Database.SqlQuery<EnumTimeInterVal>(strSql).ToList();
            if (dblist.Count > 0)
            {
                return dblist;
            }
            return new List<EnumTimeInterVal>();
        }
    }
}