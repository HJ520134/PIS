using PDMS.Data.Infrastructure;
using PDMS.Model;
using System.Linq;
using PDMS.Common.Helpers;
using System;
using System.Data.Entity;
using PDMS.Data;
using PDMS.Model.ViewModels;

using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace PDMS.Data.Repository
{
    public class ProductReworkInfoRepository : RepositoryBase<Product_Rework_Info>, IProductReworkInfoRepository
    {
        public ProductReworkInfoRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
        /// <summary>
        /// 获取所有返工站点的相关信息   这里需要修改 1  Product_UID修改为  Flowchart_DetailUID \ product_Data   TimeInterval
        ///                                       2 将原来获取所有返工站点修改为获取制定的返修制程对应的返工站点（所有返工站点的[RelatedRepairUID]为自己的制程）
        ///                                       
        /// </summary>
        /// <param name="Detail_UID"></param>
        /// <param name="Product_UID"></param>
        /// <returns></returns>
        public List<string> GetRepairOper(int Detail_UID, int Product_UID, string selectDate, string selectTime)
        {
            #region 注释
            //            var strSql = @"SELECT  Process ,
            //        Color ,
            //        CASE WHEN FlowChart_Detail_UID IS NULL THEN ''
            //             ELSE CAST (FlowChart_Detail_UID AS VARCHAR(20))
            //        END Detail_UID , 
            //        CASE WHEN Rework_UID IS NULL THEN ''
            //             ELSE CAST (Rework_UID AS VARCHAR(20))
            //        END Rework_UID ,
            //        CASE WHEN Opposite_QTY IS NULL THEN ''
            //             ELSE CAST (Opposite_QTY AS VARCHAR(20))
            //        END Rework_QTY,
            //        CASE WHEN Is_Match IS NULL THEN ''
            //             ELSE CAST (Is_Match AS VARCHAR(20))
            //        END Is_Match,
            //        CASE WHEN Rework_Type IS NULL THEN ''
            //             ELSE CAST (Rework_Type AS VARCHAR(20))
            //        END Rework_Type,
            //        Rework_Flag
            //FROM    ( SELECT    FlowChart_Detail_UID ,
            //                    Process_Seq ,
            //                    Process ,
            //                    Color
            //          FROM      dbo.FlowChart_Detail AS fcd
            //          WHERE     FlowChart_Master_UID = ( SELECT FlowChart_Master_UID
            //                                             FROM   dbo.FlowChart_Detail
            //                                             WHERE  FlowChart_Detail_UID = {0}
            //                                           )
            //                    AND fcd.Rework_Flag = 'Rework'
            //                    AND fcd.Color=(select Color from dbo.FlowChart_Detail WHERE FlowChart_Detail_UID = {0})
            //                    AND fcd.FlowChart_Version = ( SELECT    f.FlowChart_Version
            //                                                  FROM      dbo.FlowChart_Master f ,
            //                                                            dbo.FlowChart_Detail
            //                                                            AS fcd
            //                                                  WHERE     f.FlowChart_Master_UID = fcd.FlowChart_Master_UID
            //                                                            AND f.FlowChart_Version = fcd.FlowChart_Version
            //                                                            AND fcd.FlowChart_Detail_UID = {0}
            //)
            //        ) flowChart
            //        LEFT JOIN ( select Rework_UID,Opposite_Detail_UID,Opposite_QTY,Is_Match,Rework_Type,Rework_Flag FROM
            //                  dbo.Product_Rework_Info AS pri
            //                  WHERE product_UID={1}
            //                  ) rework ON flowChart.FlowChart_Detail_UID = rework.Opposite_Detail_UID";
            #endregion

            var strSql = @"
                            DECLARE @UID INT
                            DECLARE @Version INT

                            IF OBJECT_ID('tempdb..#T') is not null
                            drop table #T

                            IF OBJECT_ID('tempdb..#TT') is not null
                            drop table #TT

                            CREATE TABLE #T
                            (
                            FlowChart_Detail_UID INT,
                            Process_Seq INT,
                            Place NVARCHAR(50),
                            Process NVARCHAR(50),
                            Color NVARCHAR(50),
                            Rework_Flag NVARCHAR(20),
                            Location_Flag BIT,
                            RelatedRepairUID INT 
                            )

                            CREATE TABLE #TT
                            (
                            Rework_UID INT,
                            FlowChart_Detail_UID INT,
                            Opposite_Detail_UID INT,
                            Opposite_QTY INT,
                            Product_Date DATE,
                            Time_Interval NVARCHAR(20),
                            Is_Match BIT,
                            Rework_Type NVARCHAR(20),
                            Rework_Flag NVARCHAR(20),
                            Process NVARCHAR(50),
                            Color NVARCHAR(50)
                            )

                            SELECT @UID = FlowChart_Master_UID,@Version = FlowChart_Version FROM dbo.FlowChart_Detail WHERE FlowChart_Detail_UID = {0} 
                            PRINT @UID
                            PRINT @Version

                            ;WITH 
                            one AS 
                            (
                            SELECT A.FlowChart_Detail_UID,A.Process_Seq,A.Place,A.Process,A.Color,A.Rework_Flag,A.Location_Flag,A.RelatedRepairUID 
                            FROM dbo.FlowChart_Detail A 
                            WHERE A.FlowChart_Master_UID = @UID AND A.FlowChart_Version = @Version
                            AND A.Rework_Flag = 'Rework' AND A.RelatedRepairUID = {0}
                            )
                            INSERT INTO #T
                            SELECT * FROM one

                            --SELECT * FROM #T

                            DECLARE @DetailUID INT
                            DECLARE @ReleatedRepairUID INT 
                            DECLARE @Process NVARCHAR(50)
                            DECLARE @Color NVARCHAR(50)
                            DECLARE T_Cursor CURSOR FOR 
                            SELECT FlowChart_Detail_UID,RelatedRepairUID, Process,Color FROM #T
                            OPEN T_Cursor
                            FETCH NEXT FROM T_Cursor INTO @DetailUID, @ReleatedRepairUID, @Process, @Color
                            WHILE (@@FETCH_STATUS=0)
                            BEGIN
                            PRINT @DetailUID
                            PRINT @ReleatedRepairUID
                            PRINT @Process
                            PRINT @Color
                            IF EXISTS 
                            (
                            SELECT TOP 1 * FROM dbo.Product_Rework_Info WHERE FlowChart_Detail_UID = @ReleatedRepairUID  AND Opposite_Detail_UID = @DetailUID
                            AND Product_Date = '{1}' AND Time_Interval = '{2}'AND Rework_Flag = 'Repair'
                            )
	                            BEGIN
		                            INSERT INTO #TT
		                            SELECT Rework_UID,FlowChart_Detail_UID,Opposite_Detail_UID,Opposite_QTY,Product_Date,Time_Interval, Is_Match, Rework_Type, Rework_Flag, 
		                            @Process AS 'Process', @Color AS 'Color' FROM dbo.Product_Rework_Info WHERE FlowChart_Detail_UID = @ReleatedRepairUID AND Opposite_Detail_UID = @DetailUID
		                            AND Product_Date = '{1}' AND Time_Interval = '{2}'AND Rework_Flag = 'Repair'
	                            END 
                            ELSE
	                            BEGIN
		                            INSERT INTO #TT
		                                    ( Rework_UID ,
		                                      FlowChart_Detail_UID ,
		                                      Opposite_Detail_UID ,
		                                      Opposite_QTY ,
		                                      Product_Date ,
		                                      Time_Interval ,
		                                      Is_Match ,
		                                      Rework_Type ,
		                                      Rework_Flag ,
		                                      Process ,
		                                      Color
		                                    )
		                            VALUES  ( 0 , -- Rework_UID - int
		                                      @ReleatedRepairUID, -- FlowChart_Detail_UID - int
		                                      @DetailUID, -- Opposite_Detail_UID - int
		                                      0 , -- Opposite_QTY - int
		                                      GETDATE() , -- Product_Date - date
		                                      N'' , -- Time_Interval - nvarchar(20)
		                                      NULL , -- Is_Match - bit
		                                      N'' , -- Rework_Type - nvarchar(20)
		                                      N'' , -- Rework_Flag - nvarchar(20)
		                                      @Process , -- Process - nvarchar(50)
		                                      @Color  -- Color - nvarchar(50)
		                                    )
	                            END  

                            --取下一行
                            FETCH NEXT FROM T_Cursor INTO @DetailUID, @ReleatedRepairUID, @Process, @Color
                            END 

                            CLOSE T_Cursor
                            DEALLOCATE T_Cursor

                            --SELECT * FROM #TT
                            SELECT 
                            CAST(FlowChart_Detail_UID AS NVARCHAR(50)) AS Detail_UID,  
                            CAST(Opposite_Detail_UID AS NVARCHAR(50)) AS Opposite_UID,  
                            CAST(Rework_UID AS NVARCHAR(50)) AS Rework_UID,
                            Process,Color, 
                            CAST(Opposite_QTY AS NVARCHAR(50)) AS Rework_QTY,
                            CAST(Is_Match AS NVARCHAR(10)) AS Is_Match,
                            Rework_Type,
                            Rework_Flag
                            FROM #TT

                            ";
            strSql = string.Format(strSql, Detail_UID, selectDate, selectTime);
            //strSql = string.Format(strSql, Detail_UID, Product_UID);
            var dbList = DataContext.Database.SqlQuery<GetReworkOper>(strSql).ToList();

            var resultString = (from item in dbList
                                let temp = ""
                                select item.Detail_UID + "_" + item.Opposite_UID + "_" + item.Rework_UID + "_" + item.Process + "_" + item.Color + "_" + item.Rework_QTY + "_" + item.Is_Match + "_" + item.Rework_Type + "_" + item.Rework_Flag).ToList();

            return resultString;
        }

        public List<Product_Rework_Info> GetReworkMatch(List<int> FlowChart_Detail_UIDList, int FlowChart_Detail_UID, DateTime Product_Date, string Time_Interval)
        {
            //string strSql = @"WITH 
            //            one AS
            //            (
            //            SELECT Product_UID FROM dbo.Product_Input
            //            WHERE FlowChart_Master_UID={0} AND FlowChart_Version={1} AND FlowChart_Detail_UID IN ('{2}') AND Product_Date='{3}' AND Time_Interval='{4}'
            //            ),
            //            two AS
            //            (
            //            SELECT * FROM dbo.Product_Rework_Info WHERE Product_UID IN 
            //            (SELECT one.Product_UID FROM one) AND Product_Date='{3}' AND Time_Interval='{4}'
            //            )
            //            SELECT * FROM two";
            string strSql = @"SELECT * FROM dbo.Product_Rework_Info WHERE FlowChart_Detail_UID IN ('{0}') AND Opposite_Detail_UID = '{1}' AND Product_Date='{2}' AND Time_Interval='{3}'";
            var uidjoin = string.Join("','", FlowChart_Detail_UIDList);
            strSql = string.Format(strSql, uidjoin, FlowChart_Detail_UID, Product_Date, Time_Interval);
            var list = DataContext.Database.SqlQuery<Product_Rework_Info>(strSql).ToList();
            return list;
        }

    }

    public interface IProductReworkInfoRepository : IRepository<Product_Rework_Info>
    {
        List<string> GetRepairOper(int Detail_UID, int Product_UID, string selectDate, string selectTime);
        List<Product_Rework_Info> GetReworkMatch(List<int> FlowChart_Detail_UIDList, int FlowChart_Detail_UID,  DateTime Product_Date, string Time_Interval);
    }
}
