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
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace PDMS.Data.Repository
{
    public class QualityAssuranceInputDetailRepository : RepositoryBase<QualityAssurance_InputDetail>, IQualityAssuranceInputDetailRepository
    {
        private Logger log = new Logger("QualityAssuranceInputDetailRepository");
        public QualityAssuranceInputDetailRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<QAInputDetailVM> QueryQAInputDetail(QADetailSearch searchData)
        {
            List<QAInputDetailVM> result = new List<QAInputDetailVM>();
            try
            {
                if(searchData.QAMaster_UID==0&&("白班小计,夜班小计,全天".Contains(searchData.Time_interval)))
                {
                    return QueryInputDetailForDaySum(searchData);
                }
                bool searchHistoryFlag = false;
                TimeSpan t = DateTime.Now - Convert.ToDateTime(searchData.ProductDate);
                if(t.Days>=7)
                {
                    searchHistoryFlag = true;
                }
                if (!string.IsNullOrEmpty(searchData.TypeCode)||!string.IsNullOrEmpty(searchData.TypeName)||!string.IsNullOrEmpty(searchData.ShortName))
                {
                   return QueryQAInputDetailVM(searchData, searchHistoryFlag);
                }
                string sql = string.Empty;

                #region

                sql = string.Format(@"
                              SELECT  ExType.TypeName AS ExceptionTypeName ,
                                      ExType.BadTypeCode as BadTypeCode,
                                      ExType.BadTypeEnglishCode as BadTypeEnglishCode,
                                      detail.QualityAssurance_InputDetail_UID ,
                                      detail.QualityAssurance_InputMaster_UID ,
                                      detail.ExceptionType_UID ,
                                      detail.RepairNG_Qty ,
                                      detail.NG_Qty ,
                                      detail.SepcialAccept_Qty ,
                                      detail.Repair_Qty,
                                      detail.Displace_Qty                    
                              INTO    #temp
                              FROM    dbo.{3} detail WITH ( NOLOCK )
                                      INNER JOIN dbo.QualityAssurance_ExceptionType Extype WITH ( NOLOCK ) ON Extype.ExceptionType_UID = detail.ExceptionType_UID
                                      INNER JOIN dbo.{4} qaMa WITH ( NOLOCK ) ON qaMa.QualityAssurance_InputMaster_UID = detail.QualityAssurance_InputMaster_UID
                              WHERE   qaMa.QualityAssurance_InputMaster_UID = {0}
                                      AND IsDeleted = 0


                              IF EXISTS (SELECT TOP 1 1 FROM #temp where Repair_Qty IS not NULL AND SepcialAccept_Qty IS not NULL AND 
                                        RepairNG_Qty IS not NULL AND NG_Qty IS not NULL )
                              BEGIN
                                      SELECT  ExceptionTypeName ,
									      BadTypeCode,
									      BadTypeEnglishCode,
                                          QualityAssurance_InputDetail_UID ,
                                          QualityAssurance_InputMaster_UID ,
                                          ExceptionType_UID ,
                                          RepairNG_Qty ,
                                          NG_Qty ,
                                          SepcialAccept_Qty ,
                                          Repair_Qty,
                                          Displace_Qty
                                      FROM    #temp
                                      ORDER BY ExceptionType_UID
                              END  
                              ElSE
                              BEGIN
                                  SELECT  ExceptionTypeName ,
								          BadTypeCode,
									      BadTypeEnglishCode,
                                          QualityAssurance_InputDetail_UID ,
                                          QualityAssurance_InputMaster_UID ,
                                          ExceptionType_UID ,
                                          RepairNG_Qty ,
                                          NG_Qty ,
                                          SepcialAccept_Qty ,
                                          Repair_Qty,
                                          Displace_Qty
                                      FROM    #temp
                                  UNION

                                  SELECT  EType.TypeName AS ExceptionTypeName ,
								          EType.BadTypeCode as BadTypeCode,
			                              EType.BadTypeEnglishCode as BadTypeEnglishCode,
                                          0 AS QualityAssurance_InputDetail_UID ,
                                          {0} AS QualityAssurance_InputMaster_UID ,
                                          EType.ExceptionType_UID ,
                                          null AS RepairNG_Qty ,
                                          null AS NG_Qty ,
                                          null AS SepcialAccept_Qty ,
                                          null AS Repair_Qty,
                                          null AS Displace_Qty
                                  FROM    dbo.ExceptionTypeWithFlowchart EWF WITH ( NOLOCK )
                                          INNER JOIN dbo.QualityAssurance_ExceptionType EType WITH ( NOLOCK ) ON EType.ExceptionType_UID = EWF.ExceptionType_UID
                                  WHERE   EWF.FlowChart_Detail_UID = {1}
                                          AND EWF.FlowChart_Master_UID = {2}
                                          AND EType.FlowChart_Master_UID={2}
                                          AND ewf.ExceptionType_UID NOT IN ( SELECT   ExceptionType_UID
                                                                            FROM  #temp )
                              END
                              DROP TABLE #temp", searchData.QAMaster_UID, searchData.FlowChart_Detail_UID, searchData.FlowChart_Master_UID, searchHistoryFlag ? "QualityAssurance_InputDetail_History" : "QualityAssurance_InputDetail",
                              searchHistoryFlag ? "QualityAssurance_InputMaster_History" : "QualityAssurance_InputMaster");

 
                #endregion

                var query = DataContext.Database.SqlQuery<QAInputDetailVM>(sql).ToList();

                result = query.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public List<QAInputDetailVM> QueryInputDetailForDaySum(QADetailSearch searchData)
        {
            List<QAInputDetailVM> result = new List<QAInputDetailVM>();
            try
            {
                string time_interval = string.Empty;
                if (searchData.Time_interval.Contains("白班小计"))
                {
                    time_interval = "Daily_Sum";
                }
                else if (searchData.Time_interval.Contains("夜班小计"))
                {
                    time_interval = "Night_Sum";
                }
                else if (searchData.Time_interval.Contains("全天"))
                {
                    time_interval = "ALL";
                }

                string sqlWhere = "";
                if (!string.IsNullOrEmpty(searchData.ShortName))
                {
                    sqlWhere = sqlWhere + string.Format(@" AND QAType.ShortName=N'{0}'", searchData.ShortName.Replace("'", "''"));
                }
                if (!string.IsNullOrEmpty(searchData.TypeName))
                {
                    sqlWhere = sqlWhere + string.Format(@" AND QAType.TypeName=N'{0}'", searchData.TypeName.Replace("'", "''"));
                }
                if (!string.IsNullOrEmpty(searchData.TypeCode))
                {
                    sqlWhere = sqlWhere + string.Format(@" AND QAType.Org_TypeCode='{0}'", searchData.TypeCode.Replace("'", "''"));
                }
                if (!string.IsNullOrEmpty(searchData.Project))
                {
                    sqlWhere = sqlWhere + string.Format(@" AND QAType.FlowChart_Master_UID={0}", searchData.FlowChart_Master_UID);
                }

                bool searchHistoryFlag = false;
                TimeSpan t = DateTime.Now - Convert.ToDateTime(searchData.ProductDate);
                if (t.Days >= 7)
                {
                    searchHistoryFlag = true;
                }

                string sql = string.Format(@"DECLARE @ProductDate Date,
	                                        
	                                        @FlowChart_Detail_UID INT,
	                                        @SumType NVARCHAR(50),
	                                        @MaterialType NVARCHAR(50),
	                                        @Color NVARCHAR(50) 	
	
                                           SET @ProductDate = N'{0}'
                                           SET @FlowChart_Detail_UID = {1}
                                           SET @SumType = N'{2}'
                                           SET @MaterialType = N'{3}'
                                           SET @Color = N'{4}' 
	
	                                        DECLARE  @TempQAMasterSum TABLE(
		                                        QualityAssurance_InputMaster_UID INT,
		                                        FlowChart_Detail_UID int,
		                                        Process NVARCHAR(50),
		                                        color NVARCHAR(50),
		                                        Time_Interval NVARCHAR(20),
		                                        Product_Date DATE,
		                                        Input INT,
		                                        FirstCheck_Qty INT,
		                                        FirstOK_Qty INT,
		                                        FirstRejectionRate DECIMAL,
		                                        NG_Qty INT,
		                                        SurfaceSA_Qty INT,
		                                        SizeSA_Qty INT,
		                                        RepairCheck_Qty INT,
		                                        RepairOK_Qty INT,
		                                        Shipment_Qty INT,
		                                        WIPForCheck_Qty INT,
		                                        NGFlag BIT,
		                                        FirstCheckFlag BIT,
		                                        Displace_Qty INT,
                                                DisplaceFlag BIT)
		
	                                        INSERT INTO @TempQAMasterSum(
		                                        QualityAssurance_InputMaster_UID, 
		                                        FlowChart_Detail_UID,
		                                        Process ,
		                                        color,
		                                        Time_Interval,
		                                        Product_Date ,
		                                        Input ,
		                                        FirstCheck_Qty ,
		                                        FirstOK_Qty ,
		                                        FirstRejectionRate ,
		                                        NG_Qty ,
		                                        SurfaceSA_Qty ,
		                                        SizeSA_Qty ,
		                                        RepairCheck_Qty ,
		                                        RepairOK_Qty ,
		                                        Shipment_Qty ,
		                                        WIPForCheck_Qty ,
		                                        NGFlag,
		                                        FirstCheckFlag,
		                                        Displace_Qty,
                                                DisplaceFlag
		                                        )
	                                        EXEC usp_GetQAMasterDaySum @ProductDate,@FlowChart_Detail_UID,@SumType,@MaterialType,@Color

	                                        SELECT QAType.TypeName as ExceptionTypeName, QAType.BadTypeCode as BadTypeCode, QAType.BadTypeEnglishCode as BadTypeEnglishCode,QAType.ExceptionType_UID,0 as QualityAssurance_InputDetail_UID,0 AS QualityAssurance_InputMaster_UID ,SUM(QADetail.NG_Qty) AS NG_Qty,SUM(QADetail.RepairNG_Qty) AS RepairNG_Qty,SUM(QADetail.Repair_Qty) AS Repair_Qty,
	                                        SUM(QADetail.SepcialAccept_Qty) AS SepcialAccept_Qty,SUM(QADetail.Displace_Qty) AS Displace_Qty FROM @TempQAMasterSum QAMaster
	                                        INNER JOIN dbo.{5} QADetail ON QADetail.QualityAssurance_InputMaster_UID = QAMaster.QualityAssurance_InputMaster_UID 
	                                        INNER JOIN dbo.QualityAssurance_ExceptionType QAType ON QAType.ExceptionType_UID = QADetail.ExceptionType_UID
	                                        WHERE QADetail.IsDeleted=0 {6}
                                            GROUP BY QAType.TypeName,QAType.ExceptionType_UID , QAType.BadTypeCode,QAType.BadTypeEnglishCode
	                                        ", searchData.ProductDate,searchData.FlowChart_Detail_UID, time_interval,searchData.MaterialType,searchData.Color,searchHistoryFlag ? "QualityAssurance_InputDetail_History" : "QualityAssurance_InputDetail",sqlWhere);

                var query = DataContext.Database.SqlQuery<QAInputDetailVM>(sql).ToList();

                result = query.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }


        public string ModifyQAInputDetail(QAInputDetailVM data)
        {
            string result = "";
            try
            {
                string sql = string.Format(@"
                                            IF EXISTS ( SELECT TOP 1
                                                                1
                                                        FROM    dbo.QualityAssurance_InputDetail
                                                        WHERE   QualityAssurance_InputDetail_UID = {7}
                                                                AND Repair_Qty IS NOT NULL
                                                                AND SepcialAccept_Qty IS NOT NULL
                                                                AND RepairNG_Qty IS NOT NULL
                                                                AND NG_Qty IS NOT NULL )
                                            BEGIN
                                                INSERT INTO dbo.QualityAssurance_DataChangeLog
                                                    ( QualityAssurance_InputDetail_UID ,
                                                      QualityAssurance_InputMaster_UID ,
                                                      IsDeleted ,
                                                      LastRepair_Qty ,
                                                      LastSepcialAccept_Qty ,
                                                      LastNG_Qty ,
                                                      LastRepairNG_Qty ,
                                                      NewRepair_Qty ,
                                                      NewSepcialAccept_Qty ,
                                                      NewNG_Qty ,
                                                      NewRepairNG_Qty ,
                                                      Modified_UID ,
                                                      Modified_Date ,
                                                      ModifiedReason,
                                                      LastDisplace_Qty,
                                                      NewDisplace_Qty
                                                    )
                                            SELECT QualityAssurance_InputDetail_UID,
	                                               QualityAssurance_InputMaster_UID,
	                                               {0},
	                                               Repair_Qty,
	                                               SepcialAccept_Qty,
	                                               NG_Qty,
	                                               RepairNG_Qty,
	                                               {1},
	                                               {2},
	                                               {3},
	                                               {4},
	                                               {5},
	                                               GETDATE(),
	                                               N'{6}',
                                                   {8},
                                                   Displace_Qty
	                                               FROM dbo.QualityAssurance_InputDetail
	                                               WHERE QualityAssurance_InputDetail_UID={7}

                                            IF {0}=1
											BEGIN
											    UPDATE dbo.QualityAssurance_InputDetail SET IsDeleted={0} WHERE QualityAssurance_InputDetail_UID={7}
											END
											ELSE
											BEGIN
										        UPDATE dbo.QualityAssurance_InputDetail SET
                                                Repair_Qty={1},SepcialAccept_Qty={2},RepairNG_Qty={3},NG_Qty={4},
                                                Modified_UID={5},Modified_Date=GETDATE(), Displace_Qty={8}
                                                WHERE QualityAssurance_InputDetail_UID={7}
											END
   
                                            END
                                            ELSE
                                            BEGIN
	                                            DELETE FROM dbo.QualityAssurance_InputDetail WHERE QualityAssurance_InputDetail_UID = {7}
                                            END", data.IsDeleted ? 1 : 0, data.Repair_Qty == null ? 0 : data.Repair_Qty, data.SepcialAccept_Qty == null ? 0 : data.SepcialAccept_Qty,
                                            data.RepairNG_Qty == null ? 0 : data.RepairNG_Qty, data.NG_Qty == null ? 0 : data.NG_Qty, data.Modified_UID, data.ModifyReason.Replace("'", "''"), data.QualityAssurance_InputDetail_UID,data.Displace_Qty == null ? 0 : data.Displace_Qty);

                DataContext.Database.ExecuteSqlCommand(sql);

            }
            catch (Exception ex)
            {
                result = "Error";
                log.Error(ex);
            }
            result = "Success";
            return result;
        }

        public List<QAInputDetailVM> QueryQAInputDetailVM(QADetailSearch searchData,bool searchHistoryFlag)
        {
            List<QAInputDetailVM> result = new List<QAInputDetailVM>();
            try
            {
                string masterSql = "";
                if (searchData.QAMaster_UID != 0)
                {
                    masterSql = string.Format(" QaDetail.QualityAssurance_InputMaster_UID={0}", searchData.QAMaster_UID);
                }
                else
                {
                    masterSql = string.Format(" QaMaster.FlowChart_Detail_UID={0}", searchData.FlowChart_Detail_UID);
                }


                #region --- where sql

                string sqlWhere = "";
                if (!string.IsNullOrEmpty(searchData.ShortName))
                {
                    sqlWhere = sqlWhere + string.Format(@" AND ExType.ShortName=N'{0}'", searchData.ShortName.Replace("'", "''"));
                }
                if (!string.IsNullOrEmpty(searchData.TypeName))
                {
                    sqlWhere = sqlWhere + string.Format(@" AND ExType.TypeName=N'{0}'", searchData.TypeName.Replace("'", "''"));
                }
                if (!string.IsNullOrEmpty(searchData.TypeCode))
                {
                    sqlWhere = sqlWhere + string.Format(@" AND ExType.Org_TypeCode='{0}'", searchData.TypeCode.Replace("'", "''"));
                }
                if (!string.IsNullOrEmpty(searchData.Project))
                {
                    sqlWhere = sqlWhere + string.Format(@" AND ExType.FlowChart_Master_UID={0}", searchData.FlowChart_Master_UID);
                }

                if (sqlWhere.StartsWith(" AND"))
                {
                    sqlWhere = sqlWhere.Remove(0, 4);
                }
                
                #endregion
                string sql = @"
                                            DECLARE @TypeCode NVARCHAR(50)
                                            SELECT @TypeCode=Org_TypeCode FROM dbo.QualityAssurance_ExceptionType ExType WITH(NOLOCK)
                                            WHERE {1}

                                            IF {2}
                                            BEGIN 
                                                DECLARE @levelOne NVARCHAR(3),
		                                                @levelTwo NVARCHAR(3),
		                                                @levelThree NVARCHAR(3)
                                                IF ISNULL(@TypeCode,'')=''
                                                BEGIN
	                                                SET @TypeCode=''
                                                END
                                                ELSE
                                                BEGIN
 	                                                SET @levelOne=SUBSTRING(@TypeCode,1,3)
 	                                                SET @levelTwo=SUBSTRING(@TypeCode,4,3)
 	                                                SET @levelThree=SUBSTRING(@TypeCode,7,3)
 	                                                IF @levelThree<>'000'
					                                BEGIN
						                                SET @TypeCode = @levelOne + @levelTwo + @levelThree
					                                END
                                                    ELSE IF @levelTwo <> '000'
                                                    BEGIN
                                                        SET @TypeCode = @levelOne + @levelTwo + '%'			
                                                    END
                                                    ELSE
                                                    BEGIN
                                                        SET @TypeCode = @levelOne + '%'
                                                    END 
                                                END
                                            END
                                            SELECT ExType.TypeName AS ExceptionTypeName,
                                                ExType.BadTypeCode as BadTypeCode,
                                                ExType.BadTypeEnglishCode as BadTypeEnglishCode,
                                                QaDetail.QualityAssurance_InputDetail_UID,
                                                {4} AS QualityAssurance_InputMaster_UID,
                                                QaDetail.ExceptionType_UID,
                                                QaDetail.RepairNG_Qty,
                                                QaDetail.NG_Qty,
                                                QaDetail.SepcialAccept_Qty,
                                                QaDetail.Repair_Qty,
                                                QaDetail.IsDeleted,
                                                QaDetail.Create_Date,
                                                QaDetail.Creator_UID,
                                                QaDetail.Modified_Date,
                                                QaDetail.Modified_UID,
                                                QaDetail.Displace_Qty FROM {0} QaDetail WITH(NOLOCK) 
                                            INNER JOIN dbo.QualityAssurance_ExceptionType ExType WITH(NOLOCK)
                                            ON ExType.ExceptionType_UID = QaDetail.ExceptionType_UID
                                            INNER JOIN {5} QaMaster WITH(NOLOCK)
                                            ON QaMaster.QualityAssurance_InputMaster_UID = QaDetail.QualityAssurance_InputMaster_UID
                                            WHERE {3} and ExType.Org_TypeCode LIKE @TypeCode
                                            ";
                string sql1 = string.Format(sql, searchHistoryFlag? "dbo.QualityAssurance_InputDetail_History" : "dbo.QualityAssurance_InputDetail", sqlWhere, searchData.IsContainsChild ? "1=1" : "1<>1", masterSql, searchData.QAMaster_UID,searchHistoryFlag ? "dbo.QualityAssurance_InputMaster_History" : "dbo.QualityAssurance_InputMaster");
                var query = DataContext.Database.SqlQuery<QAInputDetailVM>(sql1).ToList();

                result = query.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public string DeleteRepeatData()
        {
            string result = "Success";
            try
            {
                string sql = string.Format(@"DELETE FROM dbo.QualityAssurance_InputDetail WHERE QualityAssurance_InputDetail_UID IN 
                                            (
                                            SELECT A.QualityAssurance_InputDetail_UID FROM dbo.QualityAssurance_InputDetail A
                                            INNER JOIN dbo.QualityAssurance_InputDetail B ON A.QualityAssurance_InputMaster_UID = B.QualityAssurance_InputMaster_UID
                                            WHERE    A.ExceptionType_UID = B.ExceptionType_UID AND A.QualityAssurance_InputDetail_UID > B.QualityAssurance_InputDetail_UID
                                            AND A.Repair_Qty IS NULL AND A.SepcialAccept_Qty IS NULL AND a.RepairNG_Qty IS NULL AND A.NG_Qty IS NULL AND B.IsDeleted=0
                                            )");

                DataContext.Database.ExecuteSqlCommand(sql);

            }
            catch (Exception ex)
            {
                result = "Error";
                log.Error(ex);
            }
            return result;
        }

        public string DeleteNullData()
        {
            string result = "Success";
            try
            {
                string sql = string.Format(@"DELETE FROM dbo.QualityAssurance_InputDetail
                                             WHERE Repair_Qty IS NULL AND SepcialAccept_Qty IS NULL AND RepairNG_Qty IS NULL AND NG_Qty IS NULL");

                DataContext.Database.ExecuteSqlCommand(sql);

            }
            catch (Exception ex)
            {
                result = "Error";
                log.Error(ex);
            }
            return result;

        }

        public string CalculateQAReportSumData(int QaMasterUID)
        {
            string result = "";

            try
            {
                var masterUID = new SqlParameter("QAMasterUID ", QaMasterUID);

                IEnumerable<SPReturnMessage> Tempresult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<SPReturnMessage>("usp_CalculateQAReportSumData  @QAMasterUID", masterUID).ToArray();

                result = Tempresult.ToList()[0].Message;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;

        }

        public QAInputDetailVM QuerySingleQAInputDetailInfoAPI(int QAInputDetailUID)
        {
            QAInputDetailVM result = new QAInputDetailVM();
            try
            {
                var query = (from QADetails in DataContext.QualityAssurance_InputDetail
                             join ExcetptionTYpe in DataContext.QualityAssurance_ExceptionType on QADetails.ExceptionType_UID equals ExcetptionTYpe.ExceptionType_UID
                             where QADetails.QualityAssurance_InputDetail_UID == QAInputDetailUID
                             select new QAInputDetailVM
                             {
                                 RepairNG_Qty = QADetails.RepairNG_Qty,
                                 Repair_Qty = QADetails.Repair_Qty,
                                 QualityAssurance_InputDetail_UID = QADetails.QualityAssurance_InputDetail_UID,
                                 SepcialAccept_Qty = QADetails.SepcialAccept_Qty,
                                 NG_Qty = QADetails.NG_Qty,
                                 ExceptionType_UID = QADetails.ExceptionType_UID,
                                 ExceptionTypeName = ExcetptionTYpe.TypeName,
                                 QualityAssurance_InputMaster_UID = QADetails.QualityAssurance_InputMaster_UID,
                                 CreateDate = QADetails.Create_Date,
                                 Displace_Qty=QADetails.Displace_Qty
                             }).Union(
                                from QADetails in DataContext.QualityAssurance_InputDetail_History
                                join ExcetptionTYpe in DataContext.QualityAssurance_ExceptionType on QADetails.ExceptionType_UID equals ExcetptionTYpe.ExceptionType_UID
                                where QADetails.QualityAssurance_InputDetail_UID == QAInputDetailUID
                                select new QAInputDetailVM
                                {
                                    RepairNG_Qty = QADetails.RepairNG_Qty,
                                    Repair_Qty = QADetails.Repair_Qty,
                                    QualityAssurance_InputDetail_UID = QADetails.QualityAssurance_InputDetail_UID,
                                    SepcialAccept_Qty = QADetails.SepcialAccept_Qty,
                                    NG_Qty = QADetails.NG_Qty,
                                    ExceptionType_UID = QADetails.ExceptionType_UID,
                                    ExceptionTypeName = ExcetptionTYpe.TypeName,
                                    QualityAssurance_InputMaster_UID = QADetails.QualityAssurance_InputMaster_UID,
                                    CreateDate = QADetails.Create_Date,
                                    Displace_Qty = QADetails.Displace_Qty
                                });


                if (query.Count() != 0)
                {
                    result = query.Distinct().ToList()[0];
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }
         
    }

    public interface IQualityAssuranceInputDetailRepository : IRepository<QualityAssurance_InputDetail>
    {
        string ModifyQAInputDetail(QAInputDetailVM data);

        List<QAInputDetailVM> QueryQAInputDetail(QADetailSearch searchData);

        string DeleteRepeatData();

        string DeleteNullData();

        string CalculateQAReportSumData(int QaMasterUID);

        QAInputDetailVM QuerySingleQAInputDetailInfoAPI(int QAInputDetailUID);
    }
}
