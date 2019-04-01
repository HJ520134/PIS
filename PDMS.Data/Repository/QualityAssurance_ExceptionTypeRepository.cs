using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Common.Helpers;

namespace PDMS.Data.Repository
{
    public class QualityAssurance_ExceptionTypeRepository : RepositoryBase<QualityAssurance_ExceptionType>, IQualityAssurance_ExceptionTypeRepository
    {
        private Logger log = new Logger("QualityAssurance_ExceptionTypeRepository");

        public QualityAssurance_ExceptionTypeRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public IQueryable<ExceptionTypeVM> QueryBadTypes(BadTypeSearch search, Page page, out int count)
        {
            string sql = "";
            try
            {
                sql = string.Format(@"
                                            SELECT  A.ExceptionType_UID ,
                                                    A.TypeName ,
                                                    A.ShortName ,
                                                    A.TypeClassify ,
                                                    A.Project,
	                                                A.BadTypeCode,
													A.BadTypeEnglishCode,
                                                    CASE WHEN A.Org_TypeCode LIKE '%000000' THEN 1
                                                         WHEN A.Org_TypeCode LIKE '%000' THEN 2
                                                         ELSE 3
                                                    END AS TypeLevel ,
                                                    CASE WHEN A.Org_TypeCode LIKE '%000000' THEN ''
                                                         WHEN A.Org_TypeCode LIKE '%000'
                                                         THEN ( SELECT  TypeName
                                                                FROM    dbo.QualityAssurance_ExceptionType
                                                                WHERE   Org_TypeCode = SUBSTRING(A.Org_TypeCode, 1, 3)
                                                                        + '000000' and Flowchart_Master_UID=A.Flowchart_Master_UID
                                                              )
                                                         ELSE ( SELECT  TypeName
                                                                FROM    dbo.QualityAssurance_ExceptionType
                                                                WHERE   Org_TypeCode = SUBSTRING(A.Org_TypeCode, 1, 6)
                                                                        + '000' and Flowchart_Master_UID=A.Flowchart_Master_UID
                                                              )
                                                    END AS Second_Type 
                                            FROM    dbo.QualityAssurance_ExceptionType A WITH ( NOLOCK )
                                            WHERE   A.EnableFlag = 1 and A.Flowchart_Master_UID={0}", search.FlowChart_Master_UID);

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            var query = DataContext.Database.SqlQuery<ExceptionTypeVM>(sql).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.TypeName))
            {
                query = query.Where(p => p.TypeName.Contains(search.TypeName));
            }
            if (!string.IsNullOrWhiteSpace(search.ShortName))
            {
                query = query.Where(p => p.ShortName.Contains(search.ShortName));
            }
            if (!string.IsNullOrWhiteSpace(search.Org_TypeCode))
            {
                query = query.Where(p => p.Org_TypeCode == search.Org_TypeCode);
            }

            if (!string.IsNullOrWhiteSpace(search.Father_TypeCode))
            {
                query = query.Where(p => p.Org_TypeCode.StartsWith(search.Father_TypeCode));
            }
            //if (!string.IsNullOrWhiteSpace(search.Project))
            //{
            //    query = query.Where(p => p.Project == search.Project);
            //}
            count = query.Count();
            var result = query.OrderBy(o => o.ExceptionType_UID).GetPage(page);

            return result;
        }

        public string CheckBadTypeByName(string typeName)
        {
            var query = from PData in DataContext.QualityAssurance_ExceptionType
                        where PData.TypeName == typeName 
                        select PData;
            int count = query.Count();
            if (count > 0)
            {
                return "OK";
            }
            else
            {
                return "KO";
            }
        }

        public string CheckBadTypeByCode(string code)
        {
            var query = from PData in DataContext.QualityAssurance_ExceptionType
                        where PData.Org_TypeCode == code
                        select PData;
            int count = query.Count();
            if (count > 0)
            {
                return "OK";
            }
            else
            {
                return "KO";
            }
        }

        public List<QualityAssurance_ExceptionType> QueryExceptionTypeForAddAPI(int typeLevel, int Flowchart_Master_UID, string parentCode = null)
        {
            List<QualityAssurance_ExceptionType> result = new List<QualityAssurance_ExceptionType>();
            try
            {
                string typeCode = "Org_TypeCode LIKE '%{0}%'";
                switch (typeLevel)
                {
                    case 1:
                        {
                            typeCode = "Org_TypeCode LIKE '%000000'";
                        }
                        break;
                    case 2:
                        {
                            string tempCode = parentCode.Substring(0, 3) + "%000";
                            typeCode = string.Format(@"Org_TypeCode LIKE '{0}' and Org_TypeCode <> '{1}'", tempCode, parentCode.Substring(0, 3) + "000000");
                        }
                        break;
                    case 3:
                        {
                            string tempCode = parentCode.Substring(0, 6) + "%";
                            typeCode = string.Format(@"Org_TypeCode LIKE '{0}' and Org_TypeCode <> '{1}'", tempCode, parentCode.Substring(0, 6) + "000");
                        }
                        break;
                }
                string sql = string.Format(@"SELECT *  FROM dbo.QualityAssurance_ExceptionType WHERE EnableFlag=1 and Flowchart_Master_UID={1} AND {0}", typeCode, Flowchart_Master_UID);
                var query = DataContext.Database.SqlQuery<QualityAssurance_ExceptionType>(sql).ToList();
                result = query;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public List<QualityAssurance_ExceptionType> QueryExceptionTypeForSearchAPI(int typeLevel, int QAMasterUID, DateTime ProductDate,int Flowchart_Master_UID, string parentCode = null)
        {
            List<QualityAssurance_ExceptionType> result = new List<QualityAssurance_ExceptionType>();
            try
            {
                TimeSpan t = DateTime.Now - ProductDate;
                string DetailTableName = "QualityAssurance_InputDetail";
                string MasterTableName = "QualityAssurance_InputMaster";
                if (t.Days >= 7)
                {
                    DetailTableName = "QualityAssurance_InputDetail_History";
                    MasterTableName = "QualityAssurance_InputMaster_History";
                }

                string sql = "";
                switch (typeLevel)
                {
                    case 1:
                        {
                            if(QAMasterUID==0)
                            {
                                sql = string.Format(@"SELECT  *
                                                    FROM    dbo.QualityAssurance_ExceptionType QAExType
                                                    WHERE   QAExType.Org_TypeCode IN (
                                                            SELECT  SUBSTRING(QAExType.Org_TypeCode, 1, 3) + '000000'
                                                            FROM    dbo.QualityAssurance_ExceptionType QAExType WITH ( NOLOCK )
                                                                    INNER JOIN dbo.{0} QADetail WITH ( NOLOCK ) ON QADetail.ExceptionType_UID = QAExType.ExceptionType_UID
                                                                    INNER JOIN dbo.{1} qaHistory WITH(NOLOCK) ON qaHistory.QualityAssurance_InputMaster_UID = QADetail.QualityAssurance_InputMaster_UID
                                                                    INNER JOIN dbo.FlowChart_Detail FD WITH(NOLOCK) ON FD.FlowChart_Detail_UID = qaHistory.FlowChart_Detail_UID
                                                            WHERE   EnableFlag = 1 AND FD.FlowChart_Master_UID={2} AND Product_Date='{3}'
                                                                    AND IsDeleted = 0)              
                                                            AND EnableFlag = 1 and Flowchart_Master_UID={2}", DetailTableName,MasterTableName, Flowchart_Master_UID, ProductDate.ToShortDateString());
                            }
                            else
                            {
                                sql = string.Format(@"  SELECT  *
                                                        FROM    dbo.QualityAssurance_ExceptionType QAExType
                                                        WHERE   QAExType.Org_TypeCode IN (
                                                                SELECT  SUBSTRING(QAExType.Org_TypeCode, 1, 3) + '000000'
                                                                FROM    dbo.QualityAssurance_ExceptionType QAExType WITH ( NOLOCK )
                                                                        INNER JOIN dbo.{0} QADetail WITH ( NOLOCK ) ON QADetail.ExceptionType_UID = QAExType.ExceptionType_UID
                                                                WHERE   EnableFlag = 1
                                                                        AND IsDeleted = 0
                                                                        AND QADetail.QualityAssurance_InputMaster_UID = {1})              
                                                                AND EnableFlag = 1 and Flowchart_Master_UID={2}", DetailTableName, QAMasterUID, Flowchart_Master_UID);
                            }
                        }
                        break;
                    case 2:
                        {
                            string tempCode = parentCode.Substring(0, 3) + "%000";
                            var typeCode = string.Format(@"Org_TypeCode LIKE '{0}' and Org_TypeCode <> '{1}'", tempCode, parentCode.Substring(0, 3) + "000000");

                            if (QAMasterUID == 0)
                            {
                                sql = string.Format(@"SELECT  *
                                                    FROM    dbo.QualityAssurance_ExceptionType QAExType
                                                    WHERE   QAExType.Org_TypeCode IN (
                                                            SELECT  SUBSTRING(QAExType.Org_TypeCode, 1, 6) + '000'
                                                            FROM    dbo.QualityAssurance_ExceptionType QAExType WITH ( NOLOCK )
                                                                    INNER JOIN dbo.{0} QADetail WITH ( NOLOCK ) ON QADetail.ExceptionType_UID = QAExType.ExceptionType_UID
                                                                    INNER JOIN dbo.{1} qaHistory WITH(NOLOCK) ON qaHistory.QualityAssurance_InputMaster_UID = QADetail.QualityAssurance_InputMaster_UID
                                                                    INNER JOIN dbo.FlowChart_Detail FD WITH(NOLOCK) ON FD.FlowChart_Detail_UID = qaHistory.FlowChart_Detail_UID
                                                            WHERE   EnableFlag = 1 AND FD.FlowChart_Master_UID={2} AND Product_Date='{3}'
                                                                    AND IsDeleted = 0)              
                                                            AND EnableFlag = 1 and Flowchart_Master_UID={2} and {4}", DetailTableName, MasterTableName, Flowchart_Master_UID, ProductDate.ToShortDateString(), typeCode);
                            }
                            else
                            {
                                sql = string.Format(@"    
                                                    SELECT  *
                                                    FROM    dbo.QualityAssurance_ExceptionType QAExType
                                                    WHERE   QAExType.Org_TypeCode IN (
                                                            SELECT  SUBSTRING(QAExType.Org_TypeCode, 1, 6) + '000'
                                                            FROM    dbo.QualityAssurance_ExceptionType QAExType WITH ( NOLOCK )
                                                                    INNER JOIN dbo.{2} QADetail WITH ( NOLOCK ) ON QADetail.ExceptionType_UID = QAExType.ExceptionType_UID
                                                            WHERE   EnableFlag = 1
                                                                    AND IsDeleted = 0
                                                                    AND QADetail.QualityAssurance_InputMaster_UID = {1} )
                                                            AND EnableFlag = 1 and Flowchart_Master_UID={3} AND {0}
                                                    ", typeCode, QAMasterUID, DetailTableName, Flowchart_Master_UID);
                            }
                        }
                        break;
                    case 3:
                        {
                            string tempCode = parentCode.Substring(0, 6) + "%";
                            var typeCode = string.Format(@"Org_TypeCode LIKE '{0}' and Org_TypeCode <> '{1}'", tempCode, parentCode.Substring(0, 6) + "000");

                            if (QAMasterUID == 0)
                            {
                                sql = string.Format(@"
                                                            SELECT  QAExType.*
                                                            FROM    dbo.QualityAssurance_ExceptionType QAExType WITH ( NOLOCK )
                                                                    INNER JOIN dbo.{0} QADetail WITH ( NOLOCK ) ON QADetail.ExceptionType_UID = QAExType.ExceptionType_UID
                                                                    INNER JOIN dbo.{1} qaHistory WITH(NOLOCK) ON qaHistory.QualityAssurance_InputMaster_UID = QADetail.QualityAssurance_InputMaster_UID
                                                                    INNER JOIN dbo.FlowChart_Detail FD WITH(NOLOCK) ON FD.FlowChart_Detail_UID = qaHistory.FlowChart_Detail_UID
                                                            WHERE   EnableFlag = 1 AND FD.FlowChart_Master_UID={2} AND Product_Date='{3}'
                                                                    AND IsDeleted = 0             
                                                                    AND {4}", DetailTableName, MasterTableName, Flowchart_Master_UID, ProductDate.ToShortDateString(), typeCode);
                            }
                            else
                            {
                                sql = string.Format(@" 
                                                            SELECT  QAExType.*
                                                            FROM    dbo.QualityAssurance_ExceptionType QAExType WITH ( NOLOCK )
                                                                    INNER JOIN dbo.{2} QADetail WITH ( NOLOCK ) ON QADetail.ExceptionType_UID = QAExType.ExceptionType_UID
                                                            WHERE   EnableFlag = 1
                                                                    AND IsDeleted = 0
                                                                    AND QADetail.QualityAssurance_InputMaster_UID = {1} 
                                                                    AND {0}
                                                    ", typeCode, QAMasterUID, DetailTableName, Flowchart_Master_UID);
                            }

                        }
                        break;
                }

                var query = DataContext.Database.SqlQuery<QualityAssurance_ExceptionType>(sql).ToList();
                result = query;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public List<QualityAssurance_ExceptionType> QueryQAExceptionTypeAPI(QADetailSearch searchData)
        {
            List<QualityAssurance_ExceptionType> result = new List<QualityAssurance_ExceptionType>();

            try
            {
                string sqlWhere = "";

                if (!string.IsNullOrEmpty(searchData.ShortName))
                {
                    sqlWhere = string.Format(@" ExType.ShortName=N'{0}' ", searchData.ShortName.Replace("'", "''"));
                }
                if (!string.IsNullOrEmpty(searchData.TypeName))
                {
                    sqlWhere = sqlWhere + string.Format(@"AND ExType.TypeName=N'{0}' ", searchData.TypeName.Replace("'", "''"));
                }
                if (!string.IsNullOrEmpty(searchData.TypeCode))
                {
                    sqlWhere = sqlWhere + string.Format(@"AND ExType.Org_TypeCode='{0}' ", searchData.TypeCode.Replace("'", "''"));
                }
                if (searchData.FlowChart_Master_UID!=0)
                {
                    sqlWhere = sqlWhere + string.Format(@"AND ExType.FlowChart_Master_UID={0} ", searchData.FlowChart_Master_UID);
                }
                if (sqlWhere.StartsWith("AND"))
                {
                    sqlWhere = sqlWhere.Remove(0, 3);
                }

                string sql = string.Format(@"
                                            DECLARE @TypeCode NVARCHAR(50)
                                            SELECT @TypeCode=Org_TypeCode FROM dbo.QualityAssurance_ExceptionType ExType WITH(NOLOCK)
                                            WHERE  {0} 

                                            IF {1}
                                            BEGIN 
                                                IF ISNULL(@TypeCode,'')=''
                                                BEGIN
                                                    SET @TypeCode=''
                                                END
                                                ELSE
                                                BEGIN
		                                            IF @TypeCode LIKE '%000000'
		                                            BEGIN
			                                            SELECT * FROM dbo.QualityAssurance_ExceptionType ExType WITH(NOLOCK)
			                                            WHERE ExType.EnableFlag=1 AND ExType.FlowChart_Master_UID={2} and ExType.Org_TypeCode LIKE SUBSTRING(@TypeCode,1,3)+'%000' 
                                                        AND ( (TypeClassify=N'外观不良' AND Org_TypeCode NOT LIKE '%000000' AND Org_TypeCode NOT LIKE '%000')
				                                        OR (TypeClassify=N'尺寸不良' AND Org_TypeCode NOT LIKE '%000000'))
		                                            END
		                                            ELSE IF @TypeCode LIKE '%000'
		                                            BEGIN 
			                                            SELECT * FROM dbo.QualityAssurance_ExceptionType ExType WITH(NOLOCK)
			                                            WHERE ExType.EnableFlag=1 AND ExType.FlowChart_Master_UID={2} and ExType.Org_TypeCode LIKE SUBSTRING(@TypeCode,1,6)+'%' 
                                                        AND ( (TypeClassify=N'外观不良' AND Org_TypeCode NOT LIKE '%000000' AND Org_TypeCode NOT LIKE '%000')
				                                        OR (TypeClassify=N'尺寸不良' AND Org_TypeCode NOT LIKE '%000000'))
		                                            END
                                                    ELSE
		                                            BEGIN
                                                        SELECT * FROM dbo.QualityAssurance_ExceptionType ExType WITH(NOLOCK)
                                                        WHERE ExType.EnableFlag=1 and {0} 
		                                            END
                                                END
                                            END", sqlWhere, searchData.IsContainsChild ? "1=1" : "1<>1", searchData.FlowChart_Master_UID);

                var query = DataContext.Database.SqlQuery<QualityAssurance_ExceptionType>(sql).ToList();
                result = query;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public IQueryable<QAInputModifyDTO> GetQAInputModify(QAInputModifySearch search, Page page, out int count)
        {
            var query = from log in DataContext.QualityAssurance_DataChangeLog.Include("System_Users")
                        join master in DataContext.QualityAssurance_InputMaster on log.QualityAssurance_InputMaster_UID equals master.QualityAssurance_InputMaster_UID
                        join detail in DataContext.QualityAssurance_InputDetail on log.QualityAssurance_InputDetail_UID equals detail.QualityAssurance_InputDetail_UID
                        join qualityType in DataContext.QualityAssurance_ExceptionType on detail.ExceptionType_UID equals qualityType.ExceptionType_UID
                        join flowchart in DataContext.FlowChart_Detail on master.FlowChart_Detail_UID equals flowchart.FlowChart_Detail_UID
                        //join  flowMaster in DataContext.FlowChart_Master on flowchart.FlowChart_Master_UID equals flowMaster.FlowChart_Master_UID
                        //join PJ in DataContext.System_Project on flowMaster.Project_UID equals PJ.Project_UID
                        select new QAInputModifyDTO
                        {
                               Log_UID = log.Log_UID,
                               QualityAssurance_InputDetail_UID=detail.QualityAssurance_InputDetail_UID,
                               QualityAssurance_InputMaster_UID =master.QualityAssurance_InputMaster_UID,
                               LastRepair_Qty =log.LastNG_Qty,
                               LastSepcialAccept_Qty=log.LastSepcialAccept_Qty,
                               LastNG_Qty=log.LastNG_Qty,
                               LastRepairNG_Qty=log.LastRepairNG_Qty,
                               NewRepair_Qty=log.NewNG_Qty,
                               NewSepcialAccept_Qty=log.NewSepcialAccept_Qty,
                               NewNG_Qty=log.NewNG_Qty,
                               NewRepairNG_Qty=log.NewRepairNG_Qty,
                               Modified_UID=log.Modified_UID,
                               Modified_Date =log.Modified_Date,
                               ModifiedReason =log.ModifiedReason,
                               Project=qualityType.Project,
                               CheckPoint=flowchart.Process,
                               Color =flowchart.Color,
                               MaterielType =master.MaterielType,
                               Product_Date=master.Product_Date,
                               Time_Interval =master.Time_Interval,
                               TypeName= qualityType.TypeName,
                               Modified_UserName = log.System_Users.User_Name,
                               IsDeleted=log.IsDeleted,
                               LastDisplace_Qty=log.LastDisplace_Qty,
                               NewDisplace_Qty=log.NewDisplace_Qty
                        };
            var query1 = from log in DataContext.QualityAssurance_DataChangeLog.Include("System_Users")
                         join master in DataContext.QualityAssurance_InputMaster_History on log.QualityAssurance_InputMaster_UID equals master.QualityAssurance_InputMaster_UID
                         join detail in DataContext.QualityAssurance_InputDetail_History on log.QualityAssurance_InputDetail_UID equals detail.QualityAssurance_InputDetail_UID
                         join qualityType in DataContext.QualityAssurance_ExceptionType on detail.ExceptionType_UID equals qualityType.ExceptionType_UID
                         join flowchart in DataContext.FlowChart_Detail on master.FlowChart_Detail_UID equals flowchart.FlowChart_Detail_UID
                         //join  flowMaster in DataContext.FlowChart_Master on flowchart.FlowChart_Master_UID equals flowMaster.FlowChart_Master_UID
                         //join PJ in DataContext.System_Project on flowMaster.Project_UID equals PJ.Project_UID

                         select new QAInputModifyDTO
                         {
                             Log_UID = log.Log_UID,
                             QualityAssurance_InputDetail_UID = detail.QualityAssurance_InputDetail_UID,
                             QualityAssurance_InputMaster_UID = master.QualityAssurance_InputMaster_UID,
                             LastRepair_Qty = log.LastNG_Qty,
                             LastSepcialAccept_Qty = log.LastSepcialAccept_Qty,
                             LastNG_Qty = log.LastNG_Qty,
                             LastRepairNG_Qty = log.LastRepairNG_Qty,
                             NewRepair_Qty = log.NewNG_Qty,
                             NewSepcialAccept_Qty = log.NewSepcialAccept_Qty,
                             NewNG_Qty = log.NewNG_Qty,
                             NewRepairNG_Qty = log.NewRepairNG_Qty,
                             Modified_UID = log.Modified_UID,
                             Modified_Date = log.Modified_Date,
                             ModifiedReason = log.ModifiedReason,
                             Project = qualityType.Project,
                             CheckPoint = flowchart.Process,
                             Color = flowchart.Color,
                             MaterielType = master.MaterielType,
                             Product_Date = master.Product_Date,
                             Time_Interval = master.Time_Interval,
                             TypeName = qualityType.TypeName,
                             Modified_UserName = log.System_Users.User_Name,
                             IsDeleted = log.IsDeleted,
                             LastDisplace_Qty = log.LastDisplace_Qty,
                             NewDisplace_Qty = log.NewDisplace_Qty
                         };
            query = query.Union(query1).Distinct();

            if (!string.IsNullOrWhiteSpace(search.Project))
            {
                query = query.Where(p => p.Project==search.Project);
            }
            if (!string.IsNullOrWhiteSpace(search.CheckPoint))
            {
                query = query.Where(p => p.CheckPoint==search.CheckPoint);
            }
            if (!string.IsNullOrWhiteSpace(search.Color))
            {
                query = query.Where(p => p.Color == search.Color);
            }

            if (!string.IsNullOrWhiteSpace(search.MaterielType))
            {
                query = query.Where(p => p.MaterielType==search.MaterielType);
            }
            if (search.Product_Date.ToOADate()!=0)
            {
                query = query.Where(p => p.Product_Date == search.Product_Date);
            }
            if (!string.IsNullOrWhiteSpace(search.Time_Interval))
            {
                query = query.Where(p => p.Time_Interval == search.Time_Interval);
            }
           
            count = query.Count();
            return query.OrderBy(o => o.Log_UID).GetPage(page); ;
        }

        public string AddExceptionType(ExceptionTypeVM newType)
        {
            string result = "";
            try
            {
                string fatherNode = "";
                if(!string.IsNullOrEmpty(newType.Second_Type))
                {
                    fatherNode = newType.Second_Type;
                }
                else if(!string.IsNullOrEmpty(newType.First_Type))
                {
                    fatherNode = newType.First_Type;
                }

                string sql = string.Format(@"
                            IF EXISTS (
                            SELECT TOP 1 1 FROM dbo.QualityAssurance_ExceptionType
                            WHERE TypeName=N'{0}' AND Flowchart_Master_UID = N'{6}')
                            BEGIN
                                SELECT N'已存在同名的类型，请检查。' AS Message
                            END
                            ELSE
                            BEGIN
                            DECLARE @NewOrgCode NVARCHAR(50),
		                            @fatherCode NVARCHAR(50),
		                            @NowMaxCode INT
			
                            SET @fatherCode='{1}'
                            IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.QualityAssurance_ExceptionType WHERE Project=N'{5}'  AND EnableFlag=1 )
                            BEGIN
                            SET @NewOrgCode='001000000'
                            END
                            ELSE
                            IF ISNULL(@fatherCode,'')=''
                            BEGIN
                                SELECT @NowMaxCode =MAX(CONVERT(INT,Org_TypeCode)) 
							    FROM dbo.QualityAssurance_ExceptionType	
							    WHERE  Project=N'{5}' and  Org_TypeCode LIKE '%000000'
                                
                                SET @NewOrgCode=CONVERT(NVARCHAR(50),@NowMaxCode+1000000)
                            END	
                            ElSE IF @fatherCode LIKE '%000000'
                            BEGIN
	                            SELECT @NowMaxCode=MAX(CONVERT(INT,Org_TypeCode)) FROM dbo.QualityAssurance_ExceptionType
	                            WHERE Project=N'{5}' and   Org_TypeCode LIKE SUBSTRING(@fatherCode,1,3)+'%000'
		
	                            SET @NewOrgCode=CONVERT(NVARCHAR(50),@NowMaxCode+1000)
                            END
                            ELSE IF @fatherCode LIKE '%000'
                            BEGIN
	                            SELECT @NowMaxCode=MAX(CONVERT(INT,Org_TypeCode)) FROM dbo.QualityAssurance_ExceptionType
	                            WHERE  Project=N'{5}' and  Org_TypeCode LIKE SUBSTRING(@fatherCode,1,6)+'%'
		
	                            SET @NewOrgCode=CONVERT(NVARCHAR(50),@NowMaxCode+1)
                            END

                            IF LEN(@NewOrgCode)=7
                            BEGIN
	                            SET @NewOrgCode='00'+@NewOrgCode
                            END  
                            ELSE IF LEN(@NewOrgCode)=8
                            BEGIN
	                            SET @NewOrgCode='0'+@NewOrgCode
                            END 
	
                            INSERT INTO dbo.QualityAssurance_ExceptionType
	                                (   TypeName ,
	                                    ShortName ,
	                                    EnableFlag ,
	                                    Creator_UID ,
	                                    Create_Date ,
	                                    Modified_UID ,
	                                    Modified_Date ,
	                                    Org_TypeCode ,
	                                    TypeClassify,
                                        Project,
                                        Flowchart_Master_UID
	                                )
                            VALUES  (   N'{0}' , -- TypeName - nvarchar(50)
	                                    N'{2}' , -- ShortName - nvarchar(20)
	                                    1 , -- EnableFlag - bit
	                                    {3} , -- Creator_UID - int
	                                    GETDATE() , -- Create_Date - datetime
	                                    {3} , -- Modified_UID - int
	                                    GETDATE() , -- Modified_Date - datetime
	                                    @NewOrgCode , -- Org_TypeCode - nvarchar(50)
	                                    N'{4}' , -- TypeClassify - nvarchar(50),
                                        N'{5}',
                                        {6}
	                                )
	                            SELECT 'Success' AS Message
                            END", newType.TypeName.Trim(), fatherNode, newType.ShortName, newType.Creator_UID, newType.TypeClassify, newType.Project,newType.Flowchart_Master_UID);

                result = DataContext.Database.SqlQuery<string>(sql).ToArray()[0];
            }
            catch(Exception ex)
            {
                result = "Error";
                log.Error(ex);
            }

            return result;
        }

        public string DeleteExceptionTypeByUID(int uid)
        {
            string result = "";
            try
            {
                string sql = string.Format(@"
                                            DECLARE @orgCode NVARCHAR(50),
		                                            @Message NVARCHAR(200),
                                                    @Project NVARCHAR(50)
                                            SELECT @orgCode=Org_TypeCode,@Project=Project FROM dbo.QualityAssurance_ExceptionType
                                            WHERE ExceptionType_UID={0}  AND EnableFlag=1 

                                            SET @Message='';

                                            IF @orgCode LIKE '%000000'
                                            BEGIN
	                                            IF EXISTS (SELECT TOP 1 1 FROM dbo.QualityAssurance_ExceptionType
	                                            WHERE Project=@Project and Org_TypeCode LIKE SUBSTRING(@orgCode,1,3)+'%000' AND Org_TypeCode<>@orgCode)
	                                            BEGIN
		                                            SET @Message=N'有子类型，不允许删除。';
	                                            END
                                            END
                                            ELSE IF @orgCode LIKE '%000'
                                            BEGIN
	                                            IF EXISTS (SELECT TOP 1 1 FROM dbo.QualityAssurance_ExceptionType
	                                            WHERE Project=@Project and Org_TypeCode LIKE SUBSTRING(@orgCode,1,6)+'%' AND Org_TypeCode<>@orgCode)
	                                            BEGIN
		                                            SET @Message=N'有子类型，不允许删除。';
	                                            END
                                            END

                                            IF ISNULL(@Message,'')=''
                                            BEGIN
	                                            UPDATE dbo.QualityAssurance_ExceptionType SET EnableFlag=0 WHERE ExceptionType_UID={0}
	                                            SET @Message='Success';
                                            END

                                            SELECT @Message AS Message", uid);

                result = DataContext.Database.SqlQuery<string>(sql).ToArray()[0];
            }
            catch(Exception ex)
            {
                result = "Error";
                log.Error(ex);
            }
            return result;
        }

        public int getExcUID(string name, int Flowchart_Master_UID)
        {
            var query = from  exceType in DataContext.QualityAssurance_ExceptionType
                        where exceType.TypeName == name && exceType.Flowchart_Master_UID == Flowchart_Master_UID
                        select exceType.ExceptionType_UID;
            int count = query.Count();
            if (count == 0)
                return 0;
            return query.FirstOrDefault();
    }
    }

    public interface IQualityAssurance_ExceptionTypeRepository : IRepository<QualityAssurance_ExceptionType>
    {
        IQueryable<ExceptionTypeVM> QueryBadTypes(BadTypeSearch search, Page page, out int count);
        string CheckBadTypeByName(string typeName);
        string CheckBadTypeByCode(string code);

        List<QualityAssurance_ExceptionType> QueryExceptionTypeForAddAPI(int typeLevel, int Flowchart_Master_UID, string parentCode = null);

        List<QualityAssurance_ExceptionType> QueryExceptionTypeForSearchAPI(int typeLevel, int QAMasterUID, DateTime ProductDate, int Flowchart_Master_UID, string parentCode = null);

        List<QualityAssurance_ExceptionType> QueryQAExceptionTypeAPI(QADetailSearch data);
        IQueryable<QAInputModifyDTO> GetQAInputModify(QAInputModifySearch search, Page page, out int count);
        string AddExceptionType(ExceptionTypeVM newType);
        string DeleteExceptionTypeByUID(int uid);
        int getExcUID(string name, int Flowchart_Master_UID);
    }

}
