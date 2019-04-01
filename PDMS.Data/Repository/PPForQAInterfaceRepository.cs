using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Common.Helpers;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;


namespace PDMS.Data.Repository
{

    public class PPForQAInterfaceRepository : RepositoryBase<PPForQAInterface>, IPPForQAInterfaceRepository
    {
        private Logger log = new Logger("PPForQAInterfaceRepository");

        public PPForQAInterfaceRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<IPQCOper> GetIPQCOper(int Master_UID)
        {
            var strSql = @"SELECT DISTINCT BeforeSeq ,
                            AfterSeq,fcd2.IsQAProcess
                            FROM    ( SELECT    Process_Seq BeforeSeq ,
                                                LEAD(Process_Seq, 1) OVER ( ORDER BY Process_Seq ) AfterSeq
                                      FROM      ( SELECT DISTINCT
                                                            Process_Seq
                                                  FROM      dbo.FlowChart_Detail AS fcd
                                                            WHERE     FlowChart_Master_UID = {0}
                                                            AND IsQAProcess IN ( 'Polling_IPQC', 'Inspect_IPQC','Polling_OQC', 'Inspect_OQC' )
                                                ) Temp
                                    ) Temp1,dbo.FlowChart_Detail AS fcd2
                            WHERE   AfterSeq IS NOT NULL
                            AND AfterSeq=fcd2.Process_Seq
                            AND fcd2.FlowChart_Master_UID={0}
                            AND fcd2.IsQAProcess in ('Polling_IPQC','Polling_OQC')";
            strSql = string.Format(strSql, Master_UID);
            var IPQCOper = DataContext.Database.SqlQuery<IPQCOper>(strSql).ToList();
            return IPQCOper;
        }

        public List<PPForQAInterfaceDTO> GetPPForQAInterface(int BeforeSeq, int AfterSeq, string Product_Date, string Time_Interval, int Master_UID)
        {
            var result = new List<PPForQAInterfaceDTO>();
            var isPolling = from temp in DataContext.FlowChart_Detail
                            where
                                (temp.FlowChart_Master_UID == Master_UID && temp.Process_Seq == AfterSeq &&
                                 temp.IsQAProcess == "Polling")
                            select temp;
            if (isPolling.Any())
            {
                //获取当前是否有颜色区分，如果有则需要遍历颜色分别统计

                var strSqlColor = @"SELECT DISTINCT color from
                                    dbo.FlowChart_Detail AS fcd
                                    WHERE FlowChart_Master_UID={0}
                                    AND Process_Seq={1}";
                strSqlColor = string.Format(strSqlColor, Master_UID, AfterSeq);
                var color = DataContext.Database.SqlQuery<string>(strSqlColor).ToList();
                foreach (var item in color)
                {
                    var colorTemp = item.ToString();
                    //获取汇总的NGQTY
                    var strSql1 = @"
                                    select SUM(Abnormal_NG_QTY) Abnormal_NG_QTY,SUM(Normal_NG_QTY) Normal_NG_QTY from
                                    dbo.Product_Input AS pi
                                    WHERE pi.FlowChart_Master_UID={0}
                                    AND Product_Date='{1}'
                                    AND Time_Interval='{2}'
                                    AND Color=N'{3}'
                                    AND Process_Seq>{4}
                                    AND process_Seq<{5}";
                    strSql1 = string.Format(strSql1, Master_UID, Product_Date, Time_Interval, colorTemp, BeforeSeq, AfterSeq);
                    var IPQC_NG = DataContext.Database.SqlQuery<IPQC_NG>(strSql1).ToList().FirstOrDefault();
                    //获取当前的Picking_QTY
                    var strSql2 = @"select SUM(Normal_Good_QTY)INPUT_Normal,SUM(Abnormal_Good_QTY)Input_Abnormal from
                                   dbo.Product_Input AS pi
                                   WHERE Product_Date='{0}'
                                   AND Time_Interval='{1}'
                                   AND FlowChart_Master_UID={2}
                                   AND Process_Seq=({3}-1)
                                   AND Color=N'{4}'";
                    strSql2 = string.Format(strSql2, Product_Date, Time_Interval, Master_UID, AfterSeq, colorTemp);
                    var IPQC_Input = DataContext.Database.SqlQuery<IPQC_Input>(strSql2).ToList().FirstOrDefault();
                    //获取AfterSeq的Detail_UID
                    var strSql3 = @"select FlowChart_Detail_UID from
                                   dbo.Product_Input AS pi
                                   WHERE Product_Date='{0}'
                                   AND Time_Interval='{1}'
                                   AND FlowChart_Master_UID={2}
                                   AND Process_Seq={3}
                                   AND Color=N'{4}'";
                    strSql3 = string.Format(strSql3, Product_Date, Time_Interval, Master_UID, AfterSeq, colorTemp);
                    var DetailUid = DataContext.Database.SqlQuery<int>(strSql3).ToList().FirstOrDefault();
                    var PPForQA_Normal = new PPForQAInterfaceDTO();
                    PPForQA_Normal.FlowChart_Detail_UID = DetailUid;
                    PPForQA_Normal.MaterielType = "正常料";
                    PPForQA_Normal.Input_Qty = IPQC_Input.INPUT_Normal;
                    PPForQA_Normal.NG_Qty = IPQC_NG.Normal_NG_QTY;
                    PPForQA_Normal.Product_Date = DateTime.Parse(Product_Date) ;
                    PPForQA_Normal.Time_Interval = Time_Interval;
                    PPForQA_Normal.Color = colorTemp;
                    PPForQA_Normal.QAUsedFlag = false;
                    PPForQA_Normal.Create_Date = DateTime.Now;
                    PPForQA_Normal.Modified_Date = DateTime.Now;

                    var PPForQA_Abnormal = new PPForQAInterfaceDTO();
                    PPForQA_Abnormal.FlowChart_Detail_UID = DetailUid;
                    PPForQA_Abnormal.MaterielType = "非正常料";
                    PPForQA_Abnormal.Input_Qty = IPQC_Input.Input_Abnormal;
                    PPForQA_Abnormal.NG_Qty = IPQC_NG.Abnormal_NG_QTY;
                    PPForQA_Abnormal.Product_Date = DateTime.Parse(Product_Date);
                    PPForQA_Abnormal.Time_Interval = Time_Interval;
                    PPForQA_Abnormal.Color = colorTemp;
                    PPForQA_Abnormal.QAUsedFlag = false;
                    PPForQA_Abnormal.Create_Date = DateTime.Now;
                    PPForQA_Abnormal.Modified_Date = DateTime.Now;

                    result.Add(PPForQA_Normal);
                    result.Add(PPForQA_Abnormal);
                }
            }
            return result;
        }

        public List<PPForQAInterfaceDTO> GetQADataForInterface(string Product_Date, string Time_Interval,int Master_UID)
        {
            var strSql = @"
                select pi.FlowChart_Detail_UID,Normal_Good_QTY Input_Qty,Normal_NG_QTY NG_Qty,Product_Date,Time_Interval,pi.Color,N'正常料'MaterielType,CONVERT(BIT,0) QAUsedFlag,GETDATE()CREATE_Date,GETDATE()Modified_Date FROM
                dbo.Product_Input AS pi,(select * from
                dbo.FlowChart_Detail AS fcd
                WHERE fcd.FlowChart_Master_UID={0}
                AND IsQAProcess in ('Inspect_IPQC','Inspect_OQC'))flow
                WHERE 
                pi.FlowChart_Detail_UID=flow.FlowChart_Detail_UID
                AND Product_Date='{1}'
                AND Time_Interval='{2}'
                AND pi.FlowChart_Master_UID={0}

                UNION ALL

                select pi.FlowChart_Detail_UID,Abnormal_Good_QTY Input_Qty,Abnormal_NG_QTY NG_Qty,Product_Date,Time_Interval,pi.Color,N'非正常料'MaterielType,CONVERT(BIT,0) QAUsedFlag,GETDATE()CREATE_Date,GETDATE()Modified_Date FROM
                dbo.Product_Input AS pi,(select * from
                dbo.FlowChart_Detail AS fcd
                WHERE fcd.FlowChart_Master_UID={0}
                AND IsQAProcess in ('Inspect_IPQC','Inspect_OQC'))flow
                WHERE 
                pi.FlowChart_Detail_UID=flow.FlowChart_Detail_UID
                AND Product_Date='{1}'
                AND Time_Interval='{2}'
                AND pi.FlowChart_Master_UID={0}
                ";
            strSql = string.Format(strSql, Master_UID, Product_Date, Time_Interval);
            var result  = DataContext.Database.SqlQuery<PPForQAInterfaceDTO>(strSql).ToList();
            return result;
        }
        public PPForQAInterface getPrdInfo(int flowcharDetailUID, DateTime Date, string TimeInterval)
        {
            var query = from PI in DataContext.PPForQAInterface
                        where PI.FlowChart_Detail_UID == flowcharDetailUID & PI.Product_Date == Date & PI.Time_Interval == TimeInterval
                        select PI;
            return query.FirstOrDefault();
        }
        public QualityAssurance_InputMasterDTO QueryDataFromPP(CheckPointInputConditionModel searchModel)
        {
            QualityAssurance_InputMasterDTO result = new QualityAssurance_InputMasterDTO();
            try
            {
                var query = from QAinterface in DataContext.PPForQAInterface
                            join flowchartdetail in DataContext.FlowChart_Detail on QAinterface.FlowChart_Detail_UID equals
                                flowchartdetail.FlowChart_Detail_UID
                            where flowchartdetail.FlowChart_Master_UID == searchModel.Flowchart_Master_UID &&
                                  QAinterface.Color == searchModel.Color &&
                                  QAinterface.Product_Date == searchModel.ProductDate &&
                                  QAinterface.Time_Interval == searchModel.Time_interval && QAinterface.MaterielType == searchModel.MaterialType
                                  && QAinterface.QAUsedFlag == false && QAinterface.FlowChart_Detail_UID==searchModel.Flowchart_Detail_UID
                            select new
                            {
                                QAinterface.NG_Qty,
                                QAinterface.Input_Qty
                            };
                if (query.Count() > 0)
                {
                    result.NG_Qty = query.ToArray()[0].NG_Qty;
                    result.Input = query.ToArray()[0].Input_Qty;
                    result.Product_Date = searchModel.ProductDate;
                    result.Time_Interval = searchModel.Time_interval;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }
    }
    public interface IPPForQAInterfaceRepository : IRepository<PPForQAInterface>
    {
        //获取IPQC的数据
        List<PPForQAInterfaceDTO> GetPPForQAInterface(int BeforeSeq, int AfterSeq, string Product_Date, string Time_Interval, int Master_UID);
        //获取QA的数据
        List<PPForQAInterfaceDTO> GetQADataForInterface(string Product_Date, string Time_Interval, int Master_UID);
        //获取IPQC的站点
        List<IPQCOper> GetIPQCOper(int Master_UID);

        PPForQAInterface getPrdInfo(int flowcharDetailUID, DateTime Date, string TimeInterval);
        QualityAssurance_InputMasterDTO QueryDataFromPP(CheckPointInputConditionModel searchModel);
    }
}
