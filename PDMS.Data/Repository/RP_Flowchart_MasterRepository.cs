using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PDMS.Common.Constants;
using System.Data.Entity.SqlServer;
using System.Text;
using System.Data.Entity.Infrastructure;
using PDMS.Model.ViewModels.ProductionPlanning;
using System.Data;
using PDMS.Common.Helpers;
using System.Transactions;

namespace PDMS.Data.Repository
{
    public interface IRP_Flowchart_MasterRepository : IRepository<RP_Flowchart_Master>
    {
        /// <summary>
        /// 匯入ME資料
        /// </summary>
        /// <param name="all_vm">資料集合</param>
        void ImportFlowchartME(RP_All_VM all_vm);
        /// <summary>
        /// 取得ME清單
        /// </summary>
        /// <param name="search">查詢條件集合</param>
        /// <param name="page">分頁參數</param>
        /// <param name="count">筆數</param>
        /// <returns></returns>
        List<RP_ME_VM> QueryMEs(RP_MESearch search, Page page, out int count);
        /// <summary>
        /// 取得ME_D資料清單by ME主檔流水號
        /// </summary>
        /// <param name="rP_Flowchart_Master_UID">ME主檔流水號</param>
        /// <returns></returns>
        List<RP_ME_D> GetME_Ds(int rP_Flowchart_Master_UID);
        /// <summary>
        /// 取得ME主檔 Change History
        /// </summary>
        /// <param name="plant_Organization_UID">plant_Organization_UID</param>
        /// <param name="bG_Organization_UID">bG_Organization_UID</param>
        /// <param name="project_UID">project_UID</param>
        /// <returns></returns>
        List<RP_M> GetME_ChangeHistory(int plant_Organization_UID, int bG_Organization_UID, int project_UID);

        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment
        /// </summary>
        /// <param name="search">搜尋條件集合</param>
        /// <param name="page">分頁參數</param>
        /// <returns></returns>
        List<RP_ME_D_Equipment> GetME_D_Equipments(ME_EquipmentSearchVM search, Page page, out int count);
        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment (單筆)
        /// </summary>
        /// <param name="rP_Flowchart_Detail_ME_Equipment_UID">設備明細檔流水號</param>
        /// <returns></returns>
        RP_ME_D_Equipment GetME_D_Equipment(int rP_Flowchart_Detail_ME_Equipment_UID);
    }
    public class RP_Flowchart_MasterRepository : RepositoryBase<RP_Flowchart_Master>, IRP_Flowchart_MasterRepository
    {
        public RP_Flowchart_MasterRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        /// <summary>
        /// 取得ME清單
        /// </summary>
        /// <param name="search">查詢條件集合</param>
        /// <param name="page">分頁參數</param>
        /// <param name="count">筆數</param>
        /// <returns></returns>
        public List<RP_ME_VM> QueryMEs(RP_MESearch search, Page page, out int count)
        {
            List<RP_ME_VM> query = new List<RP_ME_VM>();
            var Plant_Organization_UID = new SqlParameter("@Plant_Organization_UID", search.Plant_Organization_UID);
            var BG_Organization_UID = new SqlParameter("@BG_Organization_UID", search.BG_Organization_UID);
            var Project_Name = new SqlParameter("@Project_Name", search.Project_Name == null ? string.Empty : search.Project_Name.Trim());
            var BU = new SqlParameter("@BU", search.BU == null ? string.Empty : search.BU.Trim());
            var Part_Types = new SqlParameter("@Part_Types", search.Part_Types == null ? string.Empty : search.Part_Types.Trim());
            var Product_Phase = new SqlParameter("@Product_Phase", search.Product_Phase == null ? string.Empty : search.Product_Phase.Trim());
            var Is_Closed = new SqlParameter("@Is_Closed", search.Is_Closed == null ? false : search.Is_Closed);
            var Start_Date = new SqlParameter("@Start_Date", search.Start_Date == null ? string.Empty : search.Start_Date);
            var End_Date = new SqlParameter("@End_Date", search.End_Date == null ? string.Empty : search.End_Date);

            object[] parameter = new object[] { Plant_Organization_UID, BG_Organization_UID, Project_Name, BU, Part_Types, Product_Phase, Is_Closed, Start_Date, End_Date };

            using (var context = new SPPContext())
            {
                var sql_str = string.Empty;
                sql_str += @"
--DECLARE @Plant_Organization_UID int = 1,
--		@BG_Organization_UID int = 3,
--		@Project_Name nvarchar(50) = '',
--		@BU nvarchar(50) = '',
--		@Part_Types nvarchar(50) = '',
--		@Product_Phase nvarchar(50) = '',	
--		@Is_Closed bit = null,
--		@Start_Date nvarchar(50) = '',	
--		@End_Date nvarchar(50) = ''
--;
SELECT a.[RP_Flowchart_Master_UID],
	   h.BU_Name + '-' +e.BU_D_Name as BU,
	   d.[Project_UID],
	   d.[Project_Name],
	   a.[Part_Types],
	   a.[Product_Phase],
	   a.[Plant_Organization_UID],
	   b.[Organization_Name] as Plant_Organization_Name,
	   a.[BG_Organization_UID],
	   c.[Organization_Name] as [BG_Organization_Name],
	   a.[Daily_Targetoutput],
	   a.[FPY],
	   a.[FlowChart_Version],
	   a.[FlowChart_Version_Comment],
	   a.[Created_UID],
	   f.[User_Name] as [Created_UserName],
	   a.[Created_Date],
	   a.[Modified_UID],
	   g.[User_Name] as [Modified_UserName],
	   a.[Modified_Date]
FROM [dbo].[RP_Flowchart_Master] a
LEFT JOIN [dbo].[System_Organization] b ON a.[Plant_Organization_UID] = b.[Organization_UID]
LEFT JOIN [dbo].[System_Organization] c ON a.[BG_Organization_UID] = c.[Organization_UID]
INNER JOIN [dbo].[System_Project] d ON a.[Project_UID] = d.[Project_UID] 
INNER JOIN [dbo].[System_BU_D] e ON d.[BU_D_UID] = e.[BU_D_UID]
INNER JOIN [dbo].[System_Users] f ON a.[Created_UID] = f.[Account_UID]
INNER JOIN [dbo].[System_Users] g ON a.[Modified_UID] = g.[Account_UID]
INNER JOIN [dbo].[System_BU_M] h ON e.[BU_M_UID] = h.[BU_M_UID]
WHERE [Is_Latest] = 1 AND [Is_Closed] = 0
AND (a.[Plant_Organization_UID] = @Plant_Organization_UID OR @Plant_Organization_UID = 0)
AND (a.[BG_Organization_UID] = @BG_Organization_UID OR @BG_Organization_UID = 0)
AND (d.[Project_Name] like '%'+ @Project_Name +'%' OR ISNULL(@Project_Name,'') = '')
AND (h.BU_Name like N'%'+ @BU +'%' OR e.BU_D_Name like N'%'+ @BU +'%' OR ISNULL(@BU,'') = '')
AND (a.[Part_Types] like '%'+ @Part_Types +'%' OR ISNULL(@Part_Types,'') = '')
AND (a.[Product_Phase] like N'%'+ @Product_Phase +'%' OR ISNULL(@Product_Phase,'') = '')
AND (a.[Is_Closed] = @Is_Closed OR ISNULL(@Is_Closed,'') = '')
AND ((@Start_Date <= CONVERT(char(10),a.[Modified_Date],126) OR ISNULL(@Start_Date,'') = '') and
	(@End_Date >= CONVERT(char(10),a.[Modified_Date],126) OR ISNULL(@End_Date,'') = ''))
";
                query = context.Database.SqlQuery<RP_ME_VM>(sql_str, parameter).ToList();
            }

            count = query.Count();
            return query;
        }

        /// <summary>
        /// 取得ME_D資料清單by ME主檔流水號
        /// </summary>
        /// <param name="rP_Flowchart_Master_UID">ME主檔流水號</param>
        /// <returns></returns>
        public List<RP_ME_D> GetME_Ds(int rP_Flowchart_Master_UID)
        {
            List<RP_ME_D> query = new List<RP_ME_D>();
            var RP_Flowchart_Master_UID = new SqlParameter("@RP_Flowchart_Master_UID", rP_Flowchart_Master_UID);

            object[] parameter = new object[] { RP_Flowchart_Master_UID };

            using (var context = new SPPContext())
            {
                var sql_str = string.Empty;
                sql_str += @"
--DECLARE @RP_Flowchart_Master_UID int = 33
--;
SELECT f.BU_Name + '-' +e.BU_D_Name as BU,
	   d.[Project_Name],
	   b.[Part_Types],
	   b.[Product_Phase],
	   a.[Process_Station],
	   c.Organization_Name as FunPlant_Organization_Name,
	   a.[Process],
       a.[Process_Desc],
	   a.[Processing_Equipment],
	   a.[Automation_Equipment],
	   a.[Processing_Fixtures],
	   a.[Auxiliary_Equipment],
	   a.[Equipment_CT],
	   a.[Setup_Time],
	   a.[Total_Cycletime],
	   a.[ME_Estimate_Yield],
	   a.[Manpower_Ratio],
	   a.[Capacity_ByHour],
	   a.[Capacity_ByDay],
	   a.[Equipment_RequstQty],
	   a.[Manpower_2Shift]
FROM [dbo].[RP_Flowchart_Detail_ME] a
INNER JOIN [dbo].[RP_Flowchart_Master] b ON a.[RP_Flowchart_Master_UID] = b.[RP_Flowchart_Master_UID]
INNER JOIN [dbo].[System_Organization] c ON a.[FunPlant_Organization_UID] = c.Organization_UID
INNER JOIN [dbo].[System_Project] d ON b.[Project_UID] = d.[Project_UID] 
INNER JOIN [dbo].[System_BU_D] e ON d.[BU_D_UID] = e.[BU_D_UID]
INNER JOIN [dbo].[System_BU_M] f ON f.[BU_M_UID] = e.[BU_M_UID]
WHERE b.[RP_Flowchart_Master_UID] = @RP_Flowchart_Master_UID
";
                query = context.Database.SqlQuery<RP_ME_D>(sql_str, parameter).ToList();
            }
            return query;
        }

        /// <summary>
        /// 取得ME主檔 Change History
        /// </summary>
        /// <param name="plant_Organization_UID">plant_Organization_UID</param>
        /// <param name="bG_Organization_UID">bG_Organization_UID</param>
        /// <param name="project_UID">project_UID</param>
        /// <returns></returns>
        public List<RP_M> GetME_ChangeHistory(int plant_Organization_UID, int bG_Organization_UID, int project_UID)
        {
            List<RP_M> query = new List<RP_M>();
            var Plant_Organization_UID = new SqlParameter("@plant_Organization_UID", plant_Organization_UID);
            var BG_Organization_UID = new SqlParameter("@BG_Organization_UID", bG_Organization_UID);
            var Project_UID = new SqlParameter("@Project_UID", project_UID);

            object[] parameter = new object[] { Plant_Organization_UID, BG_Organization_UID, Project_UID };

            using (var context = new SPPContext())
            {
                var sql_str = string.Empty;
                sql_str += @"
--DECLARE @Plant_Organization_UID nvarchar(50) = 1,
--		@BG_Organization_UID nvarchar(50) = 3,
--		@Project_UID nvarchar(50) = 66
--;
SELECT  a.RP_Flowchart_Master_UID,
        d.BU_Name + '-' + c.BU_D_Name as BU,
		b.[Project_Name],
		a.[Part_Types],
		a.[Product_Phase],
		a.[FlowChart_Version],
		a.[FlowChart_Version_Comment],
		a.[Daily_Targetoutput],
		a.[FPY],
		a.[Is_Closed],
		a.[Created_UID],
		e.[User_Name] + ' (' + e.[User_NTID]　+ ')' as [Created_User_Name],
		a.[Created_Date],
		a.[Modified_UID],
		f.[User_Name] + ' (' + f.[User_NTID]　+ ')' as [Modified_User_Name],
		a.[Modified_Date]
FROM [dbo].[RP_Flowchart_Master] a
INNER JOIN [dbo].[System_Project] b ON ａ.[Project_UID] = b.[Project_UID] 
INNER JOIN [dbo].[System_BU_D] c ON b.[BU_D_UID] = c.[BU_D_UID]
INNER JOIN [dbo].[System_BU_M] d ON c.[BU_M_UID] = d.[BU_M_UID]
INNER JOIN [dbo].[System_Users] e ON a.[Created_UID] = e.[Account_UID]
INNER JOIN [dbo].[System_Users] f ON a.[Modified_UID] = f.[Account_UID]
WHERE [Plant_Organization_UID] = @Plant_Organization_UID 
AND [BG_Organization_UID] = @BG_Organization_UID
AND a.[Project_UID] = @Project_UID
ORDER BY [FlowChart_Version]
";
                query = context.Database.SqlQuery<RP_M>(sql_str, parameter).ToList();
            }
            return query;
        }

        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment
        /// </summary>
        /// <param name="search">搜尋條件集合</param>
        /// <param name="page">分頁參數</param>
        /// <returns></returns>
        public List<RP_ME_D_Equipment> GetME_D_Equipments(ME_EquipmentSearchVM search, Page page, out int count)
        {
            List<RP_ME_D_Equipment> query = new List<RP_ME_D_Equipment>();
            var RP_Flowchart_Master_UID = new SqlParameter("@RP_Flowchart_Master_UID", search.RP_Flowchart_Master_UID);
            var Equipment_Type = new SqlParameter("@Equipment_Type", search.Equipment_Type);

            object[] parameter = new object[] { RP_Flowchart_Master_UID, Equipment_Type };

            using (var context = new SPPContext())
            {
                var sql_str = string.Empty;
                sql_str += @"
--DECLARE @RP_Flowchart_Master_UID int = 33,
--		@Equipment_Type nvarchar(50) = N''
--;
SELECT b.RP_Flowchart_Detail_ME_Equipment_UID,
       a.[Process_Seq],
	   a.[Process_Station],
	   c.Organization_Name as FunPlant_Organization_Name,
	   a.[Process],
	   b.[Equipment_Name],
	   b.Equipment_Spec,
	   b.Equipment_Qty,
	   b.Ratio,
	   b.Request_Qty,
	   b.EQP_Variable_Qty,
	   b.NPI_Current_Qty,
	   b.MP_Current_Qty,
	   a.[Capacity_ByDay],
	   a.[Capacity_ByHour],
	   a.ME_Estimate_Yield,
	   b.Plan_CT
FROM [dbo].[RP_Flowchart_Detail_ME] a
INNER JOIN [dbo].[RP_Flowchart_Detail_ME_Equipment] b ON a.[RP_Flowchart_Detail_ME_UID] = b.[RP_Flowchart_Detail_ME_UID]
INNER JOIN [dbo].[System_Organization] c ON a.[FunPlant_Organization_UID] = c.Organization_UID
WHERE a.[RP_Flowchart_Master_UID] = @RP_Flowchart_Master_UID AND ([Equipment_Type] = @Equipment_Type OR ISNULL(@Equipment_Type,'') = '')

";
                query = context.Database.SqlQuery<RP_ME_D_Equipment>(sql_str, parameter).ToList();
            }
            count = query.Count();
            return query.Skip(page.Skip).Take(page.PageSize).ToList();
        }

        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment
        /// </summary>
        /// <param name="rP_Flowchart_Detail_ME_Equipment_UID">設備明細檔流水號</param>
        /// <returns></returns>
        public RP_ME_D_Equipment GetME_D_Equipment(int rP_Flowchart_Detail_ME_Equipment_UID)
        {
            RP_ME_D_Equipment query = new RP_ME_D_Equipment();
            var RP_Flowchart_Detail_ME_Equipment_UID = new SqlParameter("@RP_Flowchart_Detail_ME_Equipment_UID", rP_Flowchart_Detail_ME_Equipment_UID);

            object[] parameter = new object[] { RP_Flowchart_Detail_ME_Equipment_UID };

            using (var context = new SPPContext())
            {
                var sql_str = string.Empty;
                sql_str += @"
SELECT b.[Process_Seq],
       b.[Process_Station],
	   c.Organization_Name as FunPlant_Organization_Name,
	   b.Process,
       b.[Capacity_ByHour],
	   b.[Capacity_ByDay],
	   a.*  
FROM [dbo].[RP_Flowchart_Detail_ME_Equipment] a
INNER JOIN [dbo].[RP_Flowchart_Detail_ME] b ON a.[RP_Flowchart_Detail_ME_UID] = b.[RP_Flowchart_Detail_ME_UID]
INNER JOIN [dbo].[System_Organization] c ON b.[FunPlant_Organization_UID] = c.Organization_UID
WHERE a.[RP_Flowchart_Detail_ME_Equipment_UID] = @RP_Flowchart_Detail_ME_Equipment_UID
";
                query = context.Database.SqlQuery<RP_ME_D_Equipment>(sql_str, parameter).SingleOrDefault();
            }

            return query;
        }

        /// <summary>
        /// 匯入ME資料
        /// </summary>
        /// <param name="all_vm">資料集合</param>
        public void ImportFlowchartME(RP_All_VM all_vm)
        {
            using (var trans = DataContext.Database.BeginTransaction())
            {
                decimal _RP_Flowchart_Master_UID = 0;
                foreach (var _RP in all_vm.RP_VM)
                {
                    var masterUidSql = "SELECT  SCOPE_IDENTITY();";

                    // 如果為編輯, 版號+1
                    if (all_vm.IsEdit)
                    {
                        var LatestMain = DataContext.RP_Flowchart_Master.Where(r => r.RP_Flowchart_Master_UID == _RP.RP_M.RP_Flowchart_Master_UID).Single();
                        _RP.RP_M.FlowChart_Version = LatestMain.FlowChart_Version + 1;
                    }
                    // 新增主檔資料(RP_Flowchart_Master)
                    string insertMasterSql = InsertMasterSql(_RP.RP_M);
                    DataContext.Database.ExecuteSqlCommand(insertMasterSql);
                    _RP_Flowchart_Master_UID = DataContext.Database.SqlQuery<decimal>(masterUidSql).First();

                    if(_RP_Flowchart_Master_UID > 0)
                    {
                        //新增 明細檔 & 設備明細檔 (RP_Flowchart_Detail_ME, RP_Flowchart_Detail_ME_Equipment)
                        foreach (var _RP_ME_D in _RP.RP_ME_D)
                        {
                            // 新增明細檔
                            string insertDetailMeSql = InsertDetailMeSql(_RP_ME_D, _RP_Flowchart_Master_UID);
                            DataContext.Database.ExecuteSqlCommand(insertDetailMeSql);
                            // 主鍵
                            var detailMeUID = DataContext.Database.SqlQuery<decimal>(masterUidSql).First();

                            // 主加工設備
                            var processingEquipList = all_vm.ProcessingEquipList.Where(m => m.Process_Seq == _RP_ME_D.Process_Seq).ToList();
                            foreach (var processingEquipItem in processingEquipList)
                            {
                                string insertProcessingEquipSql = InsertEquipSql(processingEquipItem, detailMeUID);
                                DataContext.Database.ExecuteSqlCommand(insertProcessingEquipSql);
                            }
                            // 自動化設備
                            var autoEquipList = all_vm.AutoEquipList.Where(m => m.Process_Seq == _RP_ME_D.Process_Seq).ToList();
                            foreach (var autoEquipItem in autoEquipList)
                            {
                                string insertAutoEquipSql = InsertEquipSql(autoEquipItem, detailMeUID);
                                DataContext.Database.ExecuteSqlCommand(insertAutoEquipSql);
                            }
                            // 輔助設備
                            var auxiliaryEquipList = all_vm.AuxiliaryEquipList.Where(m => m.Process_Seq == _RP_ME_D.Process_Seq).ToList();
                            foreach (var auxiliaryEquipItem in auxiliaryEquipList)
                            {
                                string insertAuxiliaryEquipSql = InsertEquipSql(auxiliaryEquipItem, detailMeUID);
                                DataContext.Database.ExecuteSqlCommand(insertAuxiliaryEquipSql);
                            }
                        }
                    }
                }
                trans.Commit();
            }

        }

        /// <summary>
        /// 新增主檔資料 (RP_Flowchart_Master)
        /// </summary>
        /// <param name="rP_M">主檔資料集</param>
        /// <returns></returns>
        private string InsertMasterSql(RP_M rP_M)
        {
            string insertMasterSql = @"
INSERT INTO [dbo].[RP_Flowchart_Master]
           ([Plant_Organization_UID]
           ,[BG_Organization_UID]
           ,[Project_UID]
           ,[Part_Types]
           ,[Product_Phase]
           ,[Color]
           ,[Daily_Targetoutput]
           ,[FPY]
           ,[FlowChart_Version]
           ,[FlowChart_Version_Comment]
           ,[Is_Latest]
           ,[Is_Closed]
           ,[Created_UID]
           ,[Created_Date]
           ,[Modified_UID]
           ,[Modified_Date])
     VALUES
           ({0}
           ,{1}
           ,{2}
           ,N'{3}'
           ,N'{4}'
           ,N'{5}'
           ,{6}
           ,{7}
           ,{8}
           ,N'{9}'
           ,{10}
           ,{11}
           ,{12}
           ,N'{13}'
           ,{14}
           ,N'{15}')";
            insertMasterSql = string.Format(insertMasterSql,
                rP_M.Plant_Organization_UID,
                rP_M.BG_Organization_UID,
                rP_M.Project_UID,
                rP_M.Part_Types,
                rP_M.Product_Phase,
                rP_M.Color,
                rP_M.Daily_Targetoutput,
                rP_M.FPY,
                rP_M.FlowChart_Version,
                rP_M.FlowChart_Version_Comment,
                rP_M.Is_Latest ? 1 : 0,
                rP_M.Is_Closed ? 1 : 0,
                rP_M.Created_UID,
                rP_M.Created_Date,
                rP_M.Modified_UID,
                rP_M.Modified_Date
                );
            return insertMasterSql;
        }
        /// <summary>
        /// 新增ME製程明細檔
        /// </summary>
        /// <param name="_RP_ME_D">ME製程明細檔資料集</param>
        /// <param name="rP_Flowchart_Master_UID"></param>
        /// <returns></returns>
        private string InsertDetailMeSql(RP_ME_D _RP_ME_D, decimal rP_Flowchart_Master_UID)
        {
            string insertDetailMeSql = @"INSERT INTO [dbo].[RP_Flowchart_Detail_ME]
           ([RP_Flowchart_Master_UID]
           ,[FunPlant_Organization_UID]
           ,[Process_Seq]
           ,[Process_Station]
           ,[Process]
           ,[Process_Desc]
           ,[Processing_Equipment]
           ,[Automation_Equipment]
           ,[Processing_Fixtures]
           ,[Auxiliary_Equipment]
           ,[Equipment_CT]
           ,[Setup_Time]
           ,[Total_Cycletime]
           ,[ME_Estimate_Yield]
           ,[Manpower_Ratio]
           ,[Capacity_ByHour]
           ,[Capacity_ByDay]
           ,[Equipment_RequstQty]
           ,[Manpower_2Shift]
           ,[Created_UID]
           ,[Created_Date]
           ,[Modified_UID]
           ,[Modified_Date])
     VALUES
           ({0}
           ,{1}
           ,{2}
           ,N'{3}'
           ,N'{4}'
           ,N'{5}'
           ,N'{6}'
           ,N'{7}'
           ,N'{8}'
           ,N'{9}'
           ,{10}
           ,{11}
           ,{12}
           ,{13}
           ,{14}
           ,{15}
           ,{16}
           ,{17}
           ,{18}
           ,{19}
           ,N'{20}'
           ,{21}
           ,N'{22}')";

            insertDetailMeSql = string.Format(insertDetailMeSql,
                    Convert.ToInt32(rP_Flowchart_Master_UID),
                    _RP_ME_D.FunPlant_Organization_UID,
                    _RP_ME_D.Process_Seq,
                    _RP_ME_D.Process_Station,
                    _RP_ME_D.Process,
                    _RP_ME_D.Process_Desc,
                    _RP_ME_D.Processing_Equipment.Replace("'", "''"), //防止excel里面的单引号导致不能插入问题
                    _RP_ME_D.Automation_Equipment.Replace("'", "''"),
                    _RP_ME_D.Processing_Fixtures.Replace("'", "''"),
                    _RP_ME_D.Auxiliary_Equipment.Replace("'", "''"),
                    _RP_ME_D.Equipment_CT ?? -1, //防止此数据为空而不能插入，下面会有替换
                    _RP_ME_D.Setup_Time ?? -1,
                    _RP_ME_D.Total_Cycletime ?? -1,
                    _RP_ME_D.ME_Estimate_Yield,
                    _RP_ME_D.Manpower_Ratio ?? -1,
                    _RP_ME_D.Capacity_ByHour,
                    _RP_ME_D.Capacity_ByDay,
                    _RP_ME_D.Equipment_RequstQty ?? -1,
                    _RP_ME_D.Manpower_2Shift ?? -1,
                    _RP_ME_D.Created_UID,
                    _RP_ME_D.Created_Date,                    
                    _RP_ME_D.Modified_UID,
                    _RP_ME_D.Modified_Date
            );

            insertDetailMeSql = insertDetailMeSql.Replace("-1", "NULL");

            return insertDetailMeSql;
        }
        /// <summary>
        /// 新增ME製程設備明细檔
        /// </summary>
        /// <param name="equipItem">ME製程設備明细檔資料集</param>
        /// <param name="detailMeUID">ME製程明细檔流水號</param>
        /// <returns></returns>
        private string InsertEquipSql(RP_ME_D_Equipment equipItem, decimal rP_Flowchart_Detail_ME_UID)
        {
            string insertSql = @"
INSERT INTO [dbo].[RP_Flowchart_Detail_ME_Equipment]
           ([RP_Flowchart_Detail_ME_UID]
           ,[Equipment_Name]
           ,[Equipment_Spec]
           ,[Equipment_Type]
           ,[Plan_CT]
           ,[Equipment_Qty]
           ,[Ratio]
           ,[Request_Qty]
           ,[EQP_Variable_Qty]
           ,[NPI_Current_Qty]
           ,[MP_Current_Qty]
           ,[Notes]
           ,[Created_Date]
           ,[Created_UID]
           ,[Modified_Date]
           ,[Modified_UID])
     VALUES
           ({0}
           ,N'{1}'
           ,N'{2}'
           ,N'{3}'
           ,{4}
           ,{5}
           ,{6}
           ,{7}
           ,{8}
           ,{9}
           ,{10}
           ,N'{11}'
           ,N'{12}'
           ,{13}
           ,N'{14}'
           ,{15})
";
            insertSql = string.Format(insertSql,
                rP_Flowchart_Detail_ME_UID,
                equipItem.Equipment_Name,
                equipItem.Equipment_Spec,
                equipItem.Equipment_Type,
                equipItem.Plan_CT,
                equipItem.Equipment_Qty,
                equipItem.Ratio,
                equipItem.Request_Qty,
                equipItem.EQP_Variable_Qty,
                equipItem.NPI_Current_Qty,
                equipItem.MP_Current_Qty,
                equipItem.Notes,
                equipItem.Created_Date,
                equipItem.Created_UID,
                equipItem.Modified_Date,
                equipItem.Modified_UID
                );
            return insertSql;
        }
    }
}

