using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PDMS.Common.Helpers;
using PDMS.Model.EntityDTO;
using System.Text;
using PDMS.Model.ViewModels.ProductionPlanning;
using System.Data;

namespace PDMS.Data.Repository
{
//    public interface IFlowChartDetail_IE_Repository : IRepository<Flowchart_Detail_IE>
//    {
//        List<FLowchart_Detail_IE_VM> QueryFlowChartForIE(int Flowchart_Master_UID);
//        List<Flowchart_Detail_IE_VM> QueryFlowChartByRangeNumber(int Flowchart_Detail_ME_UID, int RangeNumber);

//        Flowchart_Detail_ProductionPlanning QueryFlowChartByFlowchart_Detail_ME_UID(int Flowchart_Detail_ME_UID);

//        string SaveIEFlowchartData(Flowchart_Detail_ProductionPlanningList data);
//        List<HumanResourcesSummaryDTO> GetHumanResourcesSummary(string strWhere);
//        List<HumanResources> GetHumanResourcesByProject(int flowchatMasterId);
//        List<HumanResources> GetHumanResourcesByFunplant(int flowchatMasterId);
//        List<List<PlanDataView>> GetPlanDataByProject(int site, int op, int ProjectID, int PartTypeUID, string begin);
//        List<List<PlanDataView>> GetInputDataByProject(int ProjectID, int OrgId, int flowchatMasterId, string begin);
//        List<HumanInfo> GetNowHumanByBG(int bgOrgID, DateTime begindate);
//        List<HumanInfo> GetDemissionRateByBG(int bgOrgID, DateTime begindate);
//        List<InputDataForSelectDTO> GetInputDataForSelect(int projectId);

//        string ImportFlowchartIE(List<FLowchart_Detail_IE_VM> ieList);

//        DataTable QueryManPowerRequestRPT(ProductionPlanningReportVM vm);
//        List<string> GetAllManPowerProject(ProductionPlanningReportVM vm);
//        DataTable QueryManPowerRequestByProjectOne(ProductionPlanningReportVM vm);
//        DataTable QueryManPowerRequestByProjectOneByTwo(ProductionPlanningReportVM vm);
//        DataTable QueryManPowerRequestByProjectOneByThree(ProductionPlanningReportVM vm);
//        DataTable QueryManPowerRequestByFuncOne(ProductionPlanningReportVM vm);
//    }
//    public class FlowChartDetail_IE_Repository : RepositoryBase<Flowchart_Detail_IE>, IFlowChartDetail_IE_Repository
//    {

//        private Logger log = new Logger("FlowChartDetail_IE_Repository");
//        public FlowChartDetail_IE_Repository(IDatabaseFactory databaseFactory)
//            : base(databaseFactory)
//        {

//        }

//        public List<FLowchart_Detail_IE_VM> QueryFlowChartForIE(int Flowchart_Master_UID)
//        {
//            List<FLowchart_Detail_IE_VM> result = new List<FLowchart_Detail_IE_VM>();
//            try
//            {
//                string sql = string.Empty;

//                #region 注释
//                /*
//                #region --Flowchart_Details_ME Columns

//                string selected_Columns_ME = string.Format(@"
//                                    FM.FlowChart_Master_UID ,
//                                    ME.Flowchart_Detail_ME_UID ,
//                                    ME.Process ,
//                                    ME.Process_Seq ,
//                                    ME.Process_Station ,
//                                    SFP.FunPlant ,
//                                    ME.System_FunPlant_UID,
//                                    ME.Process_Desc ,
//                                    mE.Processing_Equipment ,
//                                    ME.Automation_Equipment ,
//                                    me.Equipment_CT ,
//                                    me.Setup_Time ,
//                                    me.Total_Cycletime ,
//                                    me.Manpower_Ratio ,
//                                    me.Estimate_Yield ,
//                                    me.Capacity_ByHour ,
//                                    me.Capacity_ByDay ,
//                                    me.Equipment_RequstQty");
//                #endregion

//                #region ---- IE Columns

//                string selected_Columns_IE = string.Format(@"
//                                    FD_IE.Flowchart_Detail_IE_UID,
//                                    FD_IE.VariationEquipment_RequstQty ,
//                                    FD_IE.VariationOP_Qty,
//                                    FD_IE.RegularOP_Qty,
//                                    FD_IE.MaterialKeeper_Qty,
//                                    FD_IE.Others_Qty,
//                                    FD_IE.Notes");


//                string selected_maxColumns = string.Format(@"                                    
//                                    MAX(CASE WHEN IE_Staff.Match_Type = 1 
//                                              THEN IE_Staff.Match_Rule
//                                        END) AS 'Match_Rule',
//                                    MAX(CASE WHEN IE_Staff.Match_Type = 1
//                                             THEN IE_Staff.Match_Raito
//                                        END) AS 'Monitor_Match_Raito',
//                                    MAX(CASE WHEN IE_Staff.Match_Type = 1
//                                             THEN IE_Staff.Variable_Qty
//                                        END) AS 'VariableMonitor__Qty',
//                                    MAX(CASE WHEN IE_Staff.Match_Type = 2
//                                             THEN IE_Staff.Match_Raito
//                                        END) AS 'Technician_Match_Raito',
//                                    MAX(CASE WHEN IE_Staff.Match_Type = 2
//                                             THEN IE_Staff.Variable_Qty
//                                        END) AS 'VariableTechnician__Qty'");
//                #endregion

//                #region Organiza Sql

 
              
           
//                    sql = string.Format(@"
//        IF EXISTS ( SELECT TOP 1 1
//            FROM    dbo.FlowChart_Master FM WITH ( NOLOCK )
//                    INNER JOIN dbo.FlowChart_Detail_ME ME WITH ( NOLOCK ) ON ME.FlowChart_Master_UID = FM.FlowChart_Master_UID
//                                                              AND ME.FlowChart_Version = FM.FlowChart_Version
//                    INNER JOIN dbo.Flowchart_Detail_IE FD_IE WITH ( NOLOCK ) ON FD_IE.Flowchart_Detail_ME_UID = ME.Flowchart_Detail_ME_UID
//                    INNER JOIN dbo.System_Function_Plant SFP WITH ( NOLOCK ) ON SFP.System_FunPlant_UID = ME.System_FunPlant_UID
//                    LEFT JOIN dbo.IE_MonitorStaff_Mapping IMap WITH ( NOLOCK ) ON IMap.Flowchart_Detail_IE_UID = FD_IE.Flowchart_Detail_IE_UID
//                    LEFT JOIN dbo.Flowchart_Detail_IE_MonitorStaff IE_Staff
//                    WITH ( NOLOCK ) ON IE_Staff.Flowchart_Detail_IE_MonitorStaff_UID = IMap.Flowchart_Detail_IE_MonitorStaff_UID
//            WHERE   FM.FlowChart_Master_UID = {0} )
//    BEGIN

//        SELECT  {1},{2},{3}
//        FROM    dbo.FlowChart_Master FM WITH ( NOLOCK )
//                INNER JOIN dbo.FlowChart_Detail_ME ME WITH ( NOLOCK ) ON ME.FlowChart_Master_UID = FM.FlowChart_Master_UID
//                                                              AND ME.FlowChart_Version = FM.FlowChart_Version
//                left JOIN dbo.Flowchart_Detail_IE FD_IE WITH ( NOLOCK ) ON FD_IE.Flowchart_Detail_ME_UID = ME.Flowchart_Detail_ME_UID
//                INNER JOIN dbo.System_Function_Plant SFP WITH ( NOLOCK ) ON SFP.System_FunPlant_UID = ME.System_FunPlant_UID
//                LEFT JOIN dbo.IE_MonitorStaff_Mapping IMap WITH ( NOLOCK ) ON IMap.Flowchart_Detail_IE_UID = FD_IE.Flowchart_Detail_IE_UID
//                LEFT JOIN dbo.Flowchart_Detail_IE_MonitorStaff IE_Staff WITH ( NOLOCK ) ON IE_Staff.Flowchart_Detail_IE_MonitorStaff_UID = IMap.Flowchart_Detail_IE_MonitorStaff_UID
//        WHERE   FM.FlowChart_Master_UID = {0}
//        GROUP BY {1},{2}
//        order by ME.Process_Seq
//    END
//ELSE
//    BEGIN
//        SELECT  {1}
//        FROM    dbo.FlowChart_Master FM WITH ( NOLOCK )
//                INNER JOIN dbo.FlowChart_Detail_ME ME WITH ( NOLOCK ) ON ME.FlowChart_Master_UID = FM.FlowChart_Master_UID
//                                                              AND ME.FlowChart_Version = FM.FlowChart_Version
//                INNER JOIN dbo.System_Function_Plant SFP WITH ( NOLOCK ) ON SFP.System_FunPlant_UID = ME.System_FunPlant_UID
//                WHERE   FM.FlowChart_Master_UID = {0} order by ME.Process_Seq
//    END
//", Flowchart_Master_UID, selected_Columns_ME, selected_Columns_IE, selected_maxColumns);

           
                
//                #endregion
//                */
//                #endregion

//                sql = @"IF EXISTS ( SELECT TOP 1 1
//                        FROM    dbo.FlowChart_Master FM 
//                                INNER JOIN dbo.FlowChart_Detail_ME ME ON ME.FlowChart_Master_UID = FM.FlowChart_Master_UID
//                                AND ME.FlowChart_Version = FM.FlowChart_Version
//                                INNER JOIN dbo.Flowchart_Detail_IE FD_IE ON FD_IE.Flowchart_Detail_ME_UID = ME.Flowchart_Detail_ME_UID
//                                INNER JOIN dbo.System_Function_Plant SFP ON SFP.System_FunPlant_UID = ME.System_FunPlant_UID
//                        WHERE   FM.FlowChart_Master_UID = {0} )
//	                        BEGIN
//				                        SELECT  
//							                        FM.FlowChart_Master_UID ,
//							                        ME.Flowchart_Detail_ME_UID ,
//							                        ME.Process ,
//							                        ME.Process_Seq ,
//							                        ME.Process_Station ,
//							                        SFP.FunPlant ,
//							                        ME.System_FunPlant_UID,
//							                        ME.Process_Desc ,
//							                        mE.Processing_Equipment ,
//							                        ME.Automation_Equipment ,
//							                        me.Equipment_CT ,
//							                        me.Setup_Time ,
//							                        me.Total_Cycletime ,
//							                        me.Manpower_Ratio ,
//							                        me.Estimate_Yield ,
//							                        me.Capacity_ByHour ,
//							                        me.Capacity_ByDay ,
//							                        me.Equipment_RequstQty,
//							                        FD_IE.Flowchart_Detail_IE_UID,
//                                                    FD_IE.Match_Rule,
//							                        FD_IE.VariationEquipment_RequstQty ,
//							                        FD_IE.VariationOP_Qty,
//							                        FD_IE.RegularOP_Qty,
//							                        FD_IE.MaterialKeeper_Qty,
//							                        FD_IE.Others_Qty,
//							                        FD_IE.Notes,
//							                        SquadLeader_Raito,
//							                        SquadLeader_Variable_Qty,
//							                        Technician_Raito,
//							                        Technician_Variable_Qty,
//							                        SquadLeader_Qty,
//							                        Technician_Qty,
//							                        'Edit' AS IEFlag                                    
//		                        FROM    dbo.FlowChart_Master FM 
//		                        INNER JOIN dbo.FlowChart_Detail_ME ME ON ME.FlowChart_Master_UID = FM.FlowChart_Master_UID
//		                        AND ME.FlowChart_Version = FM.FlowChart_Version
//		                        LEFT JOIN dbo.Flowchart_Detail_IE FD_IE ON FD_IE.Flowchart_Detail_ME_UID = ME.Flowchart_Detail_ME_UID
//		                        INNER JOIN dbo.System_Function_Plant SFP ON SFP.System_FunPlant_UID = ME.System_FunPlant_UID
//		                        WHERE   FM.FlowChart_Master_UID = {0}
//	                        END 
//                        ELSE
//	                        BEGIN
//	                                SELECT  
//                                        FM.FlowChart_Master_UID ,
//                                        ME.Flowchart_Detail_ME_UID ,
//                                        ME.Process ,
//                                        ME.Process_Seq ,
//                                        ME.Process_Station ,
//                                        SFP.FunPlant ,
//                                        ME.System_FunPlant_UID,
//                                        ME.Process_Desc ,
//                                        mE.Processing_Equipment ,
//                                        ME.Automation_Equipment ,
//                                        me.Equipment_CT ,
//                                        me.Setup_Time ,
//                                        me.Total_Cycletime ,
//                                        me.Manpower_Ratio ,
//                                        me.Estimate_Yield ,
//                                        me.Capacity_ByHour ,
//                                        me.Capacity_ByDay ,
//                                        me.Equipment_RequstQty,
//				                        'Add' AS IEFlag
//				                        FROM    dbo.FlowChart_Master FM 
//                                        INNER JOIN dbo.FlowChart_Detail_ME ME ON ME.FlowChart_Master_UID = FM.FlowChart_Master_UID
//                                        AND ME.FlowChart_Version = FM.FlowChart_Version
//                                        INNER JOIN dbo.System_Function_Plant SFP ON SFP.System_FunPlant_UID = ME.System_FunPlant_UID
//                                        WHERE   FM.FlowChart_Master_UID = {0} ORDER BY ME.Process_Seq

//	                        END ";


//                sql = string.Format(sql, Flowchart_Master_UID);

//                result = DataContext.Database.SqlQuery<FLowchart_Detail_IE_VM>(sql).ToList();
//            }
//            catch (Exception ex)
//            {
//                log.Error(ex);
//            }
//            return result;
//        }

//        public List<Flowchart_Detail_IE_VM> QueryFlowChartByRangeNumber(int Flowchart_Detail_ME_UID, int RangeNumber)
//        {
//            List<Flowchart_Detail_IE_VM> result = new List<Flowchart_Detail_IE_VM>();
//            try
//            {
      
//                #region ---Sql new

//                string sql = string.Format(@"
                            
//                            DECLARE @RangeNumber INT ,
//                                @Process_Seq INT,
//                                @Flowchart_Detail_ME_UID INT,
//                                @version int

//                            SET @RangeNumber = {0}
//                            SET @Flowchart_Detail_ME_UID = {1}

//                            SELECT @Process_Seq=Process_Seq FROM  dbo.Flowchart_Detail_ME
//                            WHERE Flowchart_Detail_ME_UID=@Flowchart_Detail_ME_UID
                            
//                            select @version=[FlowChart_Version] from Flowchart_Detail_ME
//                            where Flowchart_Detail_ME_UID=@Flowchart_Detail_ME_UID

//                            DECLARE @Result TABLE
//                                (
//                                  Flowchart_Detail_ME_UID INT ,
//                                  Flowchart_Detail_ME_UID_Banding INT ,
//                                  Process NVARCHAR(50) ,
//                                  Process_Seq INT ,
//                                  Monitor_Staff_UID INT ,
//                                  MonitorFlag BIT ,
//                                  Technician_Staff_UID INT ,
//                                  TechnicianFlag BIT
//                                )

//                            IF EXISTS ( SELECT  1
//                                        FROM    dbo.Flowchart_Detail_ME MED WITH ( NOLOCK )
//                                                INNER JOIN dbo.IE_MonitorStaff_Mapping_Banding IE WITH ( NOLOCK ) ON IE.Flowchart_Detail_ME_UID = MED.Flowchart_Detail_ME_UID
//                                        WHERE   IE.Flowchart_Detail_ME_UID = @Flowchart_Detail_ME_UID )
//                                BEGIN
//                                    INSERT  @Result
//                                            ( Flowchart_Detail_ME_UID ,
//                                              Flowchart_Detail_ME_UID_Banding ,
//                                              Process ,
//                                              Process_Seq ,
//                                              Monitor_Staff_UID ,
//                                              MonitorFlag ,
//                                              Technician_Staff_UID ,
//                                              TechnicianFlag
//			                                )
//                                            SELECT  IE.Flowchart_Detail_ME_UID ,
//                                                    IE.Flowchart_Detail_ME_UID_Banding ,
//                                                    MED.Process ,
//                                                    MED.Process_Seq ,
//                                                    MAX(CASE WHEN IE.MappingType = 1
//                                                             THEN IE.IE_MonitorStaff_Mapping_Banding_UID
//                                                        END) AS 'Monitor_Staff_UID' ,
//                                                    MAX(CASE WHEN IE.MappingType = 1 THEN 1
//                                                             ELSE 0
//                                                        END) AS 'MonitorFlag' ,
//                                                    MAX(CASE WHEN IE.MappingType = 2
//                                                             THEN IE.IE_MonitorStaff_Mapping_Banding_UID
//                                                        END) AS 'Technician_Staff_UID' ,
//                                                    MAX(CASE WHEN IE.MappingType = 2 THEN 1
//                                                             ELSE 0
//                                                        END) AS 'TechnicianFlag'
//                                            FROM    dbo.Flowchart_Detail_ME MED WITH ( NOLOCK )
//                                                    INNER JOIN dbo.IE_MonitorStaff_Mapping_Banding IE WITH ( NOLOCK ) ON IE.[Flowchart_Detail_ME_UID_Banding] = MED.Flowchart_Detail_ME_UID
//                                            WHERE   IE.Flowchart_Detail_ME_UID = @Flowchart_Detail_ME_UID
//                                                    OR IE.IE_MonitorStaff_Mapping_Banding_UID = @Flowchart_Detail_ME_UID
//                                            GROUP BY IE.Flowchart_Detail_ME_UID ,
//                                                    IE.Flowchart_Detail_ME_UID_Banding ,
//                                                    MED.Process ,
//                                                    MED.Process_Seq
//							end

                                
                              
//                            IF EXISTS ( SELECT  1
//                                        FROM    dbo.Flowchart_Detail_ME MED WITH ( NOLOCK )
//                                                INNER JOIN dbo.IE_MonitorStaff_Mapping_Banding IE WITH ( NOLOCK ) ON IE.Flowchart_Detail_ME_UID = MED.Flowchart_Detail_ME_UID
//                                        WHERE   IE.Flowchart_Detail_ME_UID_Banding = @Flowchart_Detail_ME_UID )
//                                BEGIN
//                                    INSERT  @Result
//                                            ( Flowchart_Detail_ME_UID ,
//                                              Flowchart_Detail_ME_UID_Banding ,
//                                              Process ,
//                                              Process_Seq ,
//                                              Monitor_Staff_UID ,
//                                              MonitorFlag ,
//                                              Technician_Staff_UID ,
//                                              TechnicianFlag
//			                                )
//                                            SELECT  IE.Flowchart_Detail_ME_UID ,
//                                                    IE.Flowchart_Detail_ME_UID_Banding ,
//                                                    MED.Process ,
//                                                    MED.Process_Seq ,
//                                                    MAX(CASE WHEN IE.MappingType = 1
//                                                             THEN IE.IE_MonitorStaff_Mapping_Banding_UID
//                                                        END) AS 'Monitor_Staff_UID' ,
//                                                    MAX(CASE WHEN IE.MappingType = 1 THEN 1
//                                                             ELSE 0
//                                                        END) AS 'MonitorFlag' ,
//                                                    MAX(CASE WHEN IE.MappingType = 2
//                                                             THEN IE.IE_MonitorStaff_Mapping_Banding_UID
//                                                        END) AS 'Technician_Staff_UID' ,
//                                                    MAX(CASE WHEN IE.MappingType = 2 THEN 1
//                                                             ELSE 0
//                                                        END) AS 'TechnicianFlag'
//                                            FROM    dbo.Flowchart_Detail_ME MED WITH ( NOLOCK )
//                                                    INNER JOIN dbo.IE_MonitorStaff_Mapping_Banding IE WITH ( NOLOCK ) ON IE.Flowchart_Detail_ME_UID= MED.Flowchart_Detail_ME_UID
//                                            WHERE   IE.Flowchart_Detail_ME_UID_Banding = @Flowchart_Detail_ME_UID
//                                            GROUP BY IE.Flowchart_Detail_ME_UID ,
//                                                    IE.Flowchart_Detail_ME_UID_Banding ,
//                                                    MED.Process ,
//                                                    MED.Process_Seq
//								end

//      INSERT  INTO @Result
//                                    ( Flowchart_Detail_ME_UID ,
//                                      Process ,
//                                      Process_Seq ,
//                                      MonitorFlag ,
//                                      TechnicianFlag,
//                                      Flowchart_Detail_ME_UID_Banding
//                                    )
//                                    SELECT  MED2.Flowchart_Detail_ME_UID ,
//                                            MED2.Process ,
//                                            MED2.Process_Seq ,
//                                            0 ,
//                                            0,
//                                            0
//                                    FROM    dbo.Flowchart_Detail_ME MED WITH ( NOLOCK )
//				                            INNER JOIN dbo.Flowchart_Detail_ME MED2 WITH ( NOLOCK ) ON MED.FlowChart_Master_UID=MED2.FlowChart_Master_UID
//                                    WHERE   MED.Flowchart_Detail_ME_UID = @Flowchart_Detail_ME_UID
//                                            AND MED2.Process_Seq <= ( @Process_Seq + @RangeNumber )
//                                            AND MED2.Process_Seq >= ( @Process_Seq - @RangeNumber )
//                                            AND MED2.Flowchart_Detail_ME_UID<>@Flowchart_Detail_ME_UID
//                                            AND MED2.[FlowChart_Version]=@version
//                                            AND MED2.Process NOT IN (
//                                            SELECT  Process
//                                            FROM    @Result )    
                                 

//                            SELECT  Flowchart_Detail_ME_UID ,
//                                    Flowchart_Detail_ME_UID_Banding ,
//                                    Process ,
//                                    Process_Seq ,
//                                    Monitor_Staff_UID ,
//                                    MonitorFlag ,
//                                    Technician_Staff_UID,
//                                    TechnicianFlag
//                            FROM    @Result", RangeNumber, Flowchart_Detail_ME_UID);

//                #endregion

//                result = DataContext.Database.SqlQuery<Flowchart_Detail_IE_VM>(sql).ToList();
//            }
//            catch (Exception ex)
//            {
//                log.Error(ex);
//            }
//            return result;

//        }

//        public string SaveIEFlowchartData(Flowchart_Detail_ProductionPlanningList data)
//        {
//            string result = "Success";
//            try
//            {
                
//                foreach(Flowchart_Detail_ProductionPlanning temp in data.DataList)
//                {
//                    #region --- sql

//                    string sql = string.Format(@"
//                            DECLARE @Flowchart_Detail_IE_UID INT ,
//                                    @Flowchart_Detail_IE_MonitorStaff INT 

//                            INSERT  INTO dbo.Flowchart_Detail_IE
//                                    ( Flowchart_Detail_ME_UID ,
//                                      VariationOP_Qty ,
//                                      RegularOP_Qty ,
//                                      MaterialKeeper_Qty ,
//                                      Others_Qty ,
//                                      Notes ,
//                                      Created_Date ,
//                                      Created_UID ,
//                                      Modified_Date ,
//                                      Modified_UID ,
//                                      VariationEquipment_RequstQty,
//                                      SquadLeader_Qty,
//                                      Technician_Qty
//                                    )
//                            VALUES  ( {0} , -- Flowchart_Detail_ME_UID - int
//                                      {1} , -- VariationOP_Qty - int
//                                      {2} , -- RegularOP_Qty - int
//                                      {3} , -- MaterialKeeper_Qty - int
//                                      {4} , -- Others_Qty - int
//                                      N'{5}' , -- Notes - nvarchar(200)
//                                      GETDATE() , -- Created_Date - datetime
//                                      {6} , -- Created_UID - int
//                                      GETDATE() , -- Modified_Date - datetime
//                                      {6} , -- Modified_UID - int
//                                      {7}, -- VariationEquipment_RequstQty - int
//                                      {14} , -- SquadLeader_Qty - int
//                                      {15} -- Technician_Qty - int  
//                                    )
//                            SELECT  @Flowchart_Detail_IE_UID = SCOPE_IDENTITY()    

//                            IF(ISNULL({10},0)<>0 or ISNULL('{11}',0)<>0 )
//                            BEGIN
//	                            INSERT  INTO dbo.Flowchart_Detail_IE_MonitorStaff
//			                            ( Match_Type ,
//			                              Match_Rule ,
//			                              Match_Raito ,
//			                              Variable_Qty ,
//			                              Created_Date ,
//			                              Created_UID ,
//			                              Modified_Date ,
//			                              Modified_UID
//			                            )
//	                            VALUES  ( 1 , -- Match_Type - int
//			                              {9} , -- Match_Rule - int
//			                              {10} , -- Match_Raito - decimal
//			                              {11} , -- Variable_Qty - int
//			                              GETDATE() , -- Created_Date - datetime
//			                              {6} , -- Created_UID - int
//			                              GETDATE() , -- Modified_Date - datetime
//			                              {6} -- Modified_UID - int
//			                            )
//	                            SELECT  @Flowchart_Detail_IE_MonitorStaff = SCOPE_IDENTITY()   
	
//	                            INSERT  INTO dbo.IE_MonitorStaff_Mapping
//		                            ( Flowchart_Detail_IE_MonitorStaff_UID ,
//		                              Flowchart_Detail_IE_UID ,
//		                              Created_Date ,
//		                              Created_UID ,
//		                              Modified_Date ,
//		                              Modified_UID
//		                            )
//	                            VALUES  ( @Flowchart_Detail_IE_MonitorStaff , -- Flowchart_Detail_IE_MonitorStaff_UID - int
//		                              @Flowchart_Detail_IE_UID , -- Flowchart_Detail_IE_UID - int
//		                              GETDATE() , -- Created_Date - datetime
//		                              {6} , -- Created_UID - int
//		                              GETDATE() , -- Modified_Date - datetime
//		                              {6}  -- Modified_UID - int
//		                            )
//                            END

//                           IF(ISNULL({12},0)<>0 or ISNULL('{13}',0)<>0 )
//                            BEGIN
//	                            INSERT  INTO dbo.Flowchart_Detail_IE_MonitorStaff
//			                            ( Match_Type ,
//			                              Match_Rule ,
//			                              Match_Raito ,
//			                              Variable_Qty ,
//			                              Created_Date ,
//			                              Created_UID ,
//			                              Modified_Date ,
//			                              Modified_UID
//			                            )
//	                            VALUES  ( 2, -- Match_Type - int
//			                              2, -- Match_Rule - int
//			                              {12} , -- Match_Raito - decimal
//			                              {13} , -- Variable_Qty - int
//			                              GETDATE() , -- Created_Date - datetime
//			                              {6} , -- Created_UID - int
//			                              GETDATE() , -- Modified_Date - datetime
//			                              {6} -- Modified_UID - int
//			                            )
//	                            SELECT  @Flowchart_Detail_IE_MonitorStaff = SCOPE_IDENTITY()   
	
//	                            INSERT  INTO dbo.IE_MonitorStaff_Mapping
//		                            ( Flowchart_Detail_IE_MonitorStaff_UID ,
//		                              Flowchart_Detail_IE_UID ,
//		                              Created_Date ,
//		                              Created_UID ,
//		                              Modified_Date ,
//		                              Modified_UID
//		                            )
//	                            VALUES  ( @Flowchart_Detail_IE_MonitorStaff , -- Flowchart_Detail_IE_MonitorStaff_UID - int
//		                              @Flowchart_Detail_IE_UID , -- Flowchart_Detail_IE_UID - int
//		                              GETDATE() , -- Created_Date - datetime
//		                              {6} , -- Created_UID - int
//		                              GETDATE() , -- Modified_Date - datetime
//		                              {6}  -- Modified_UID - int
//		                            )
//                            END", temp.Flowchart_Detail_ME_UID, temp.VariationOP_Qty, temp.RegularOP_Qty, temp.MaterialKeeper_Qty, temp.Others_Qty,
//                                 temp.Notes, temp.Created_UID, temp.VariationEquipment_RequstQty, 
//                                 string.IsNullOrEmpty(temp.Match_Type.ToString()) ? 0 : temp.Match_Type, 
//                                 string.IsNullOrEmpty(temp.Match_Rule.ToString())?0:temp.Match_Rule, 
//                                 string.IsNullOrEmpty(temp.Monitor_Match_Raito.ToString())?0:temp.Monitor_Match_Raito,
//                                 string.IsNullOrEmpty(temp.VariableMonitor__Qty.ToString()) ? 0 : temp.VariableMonitor__Qty,
//                                 string.IsNullOrEmpty(temp.Technician_Match_Raito.ToString()) ? 0 : temp.Technician_Match_Raito,
//                                 string.IsNullOrEmpty(temp.VariableTechnician__Qty.ToString()) ? 0 : temp.VariableTechnician__Qty,
//                                 temp.SquadLeader_Qty,temp.Technician_Qty);
//                    #endregion
//                    DataContext.Database.ExecuteSqlCommand(sql);
//                }                
//            }
//            catch (Exception ex)
//            {
//                result = ex.Message;
//                log.Error(ex);
//            }
//            return result;
//        }

//        public string EditIEFlowchartData(Flowchart_Detail_ProductionPlanningList data)
//        {
//            string result = "Success";
//            try
//            {

//                foreach (Flowchart_Detail_ProductionPlanning temp in data.DataList)
//                {
//                    #region --- sql

//                    string sql = string.Format(@"
//                            DECLARE @Flowchart_Detail_IE_UID INT ,
//                                    @Flowchart_Detail_IE_MonitorStaff INT 

//                            UPDATE dbo.Flowchart_Detail_IE set
//                                    Flowchart_Detail_ME_UID= {0},
//                                      VariationOP_Qty={1} ,
//                                      RegularOP_Qty ={2},
//                                      MaterialKeeper_Qty= {3},
//                                      Others_Qty= {4},
//                                      Notes= N'{5}' ,
//                                      Created_Date=GETDATE() ,
//                                      Created_UID={6} ,
//                                      Modified_Date=GETDATE() ,
//                                      Modified_UID={6} ,
//                                      VariationEquipment_RequstQty={7}                            
//                            SELECT  @Flowchart_Detail_IE_UID = SCOPE_IDENTITY()    

//                            IF(ISNULL({10},0)<>0 or ISNULL('{11}',0)<>0 )
//                            BEGIN
//	                            INSERT  INTO dbo.Flowchart_Detail_IE_MonitorStaff
//			                            ( Match_Type ,
//			                              Match_Rule ,
//			                              Match_Raito ,
//			                              Variable_Qty ,
//			                              Created_Date ,
//			                              Created_UID ,
//			                              Modified_Date ,
//			                              Modified_UID
//			                            )
//	                            VALUES  ( {8} , -- Match_Type - int
//			                              {9} , -- Match_Rule - int
//			                              {10} , -- Match_Raito - decimal
//			                              {11} , -- Variable_Qty - int
//			                              GETDATE() , -- Created_Date - datetime
//			                              {6} , -- Created_UID - int
//			                              GETDATE() , -- Modified_Date - datetime
//			                              {6} -- Modified_UID - int
//			                            )
//	                            SELECT  @Flowchart_Detail_IE_MonitorStaff = SCOPE_IDENTITY()   
	
//	                            INSERT  INTO dbo.IE_MonitorStaff_Mapping
//		                            ( Flowchart_Detail_IE_MonitorStaff_UID ,
//		                              Flowchart_Detail_IE_UID ,
//		                              Created_Date ,
//		                              Created_UID ,
//		                              Modified_Date ,
//		                              Modified_UID
//		                            )
//	                            VALUES  ( @Flowchart_Detail_IE_MonitorStaff , -- Flowchart_Detail_IE_MonitorStaff_UID - int
//		                              @Flowchart_Detail_IE_UID , -- Flowchart_Detail_IE_UID - int
//		                              GETDATE() , -- Created_Date - datetime
//		                              {6} , -- Created_UID - int
//		                              GETDATE() , -- Modified_Date - datetime
//		                              {6}  -- Modified_UID - int
//		                            )
//                            END

//                           IF(ISNULL({12},0)<>0 or ISNULL('{13}',0)<>0 )
//                            BEGIN
//	                            INSERT  INTO dbo.Flowchart_Detail_IE_MonitorStaff
//			                            ( Match_Type ,
//			                              Match_Rule ,
//			                              Match_Raito ,
//			                              Variable_Qty ,
//			                              Created_Date ,
//			                              Created_UID ,
//			                              Modified_Date ,
//			                              Modified_UID
//			                            )
//	                            VALUES  ( 2, -- Match_Type - int
//			                              2, -- Match_Rule - int
//			                              {12} , -- Match_Raito - decimal
//			                              {13} , -- Variable_Qty - int
//			                              GETDATE() , -- Created_Date - datetime
//			                              {6} , -- Created_UID - int
//			                              GETDATE() , -- Modified_Date - datetime
//			                              {6} -- Modified_UID - int
//			                            )
//	                            SELECT  @Flowchart_Detail_IE_MonitorStaff = SCOPE_IDENTITY()   
	
//	                            INSERT  INTO dbo.IE_MonitorStaff_Mapping
//		                            ( Flowchart_Detail_IE_MonitorStaff_UID ,
//		                              Flowchart_Detail_IE_UID ,
//		                              Created_Date ,
//		                              Created_UID ,
//		                              Modified_Date ,
//		                              Modified_UID
//		                            )
//	                            VALUES  ( @Flowchart_Detail_IE_MonitorStaff , -- Flowchart_Detail_IE_MonitorStaff_UID - int
//		                              @Flowchart_Detail_IE_UID , -- Flowchart_Detail_IE_UID - int
//		                              GETDATE() , -- Created_Date - datetime
//		                              {6} , -- Created_UID - int
//		                              GETDATE() , -- Modified_Date - datetime
//		                              {6}  -- Modified_UID - int
//		                            )
//                            END", temp.Flowchart_Detail_ME_UID, temp.VariationOP_Qty, temp.RegularOP_Qty, temp.MaterialKeeper_Qty, temp.Others_Qty,
//                                 temp.Notes, temp.Created_UID, temp.VariationEquipment_RequstQty,
//                                 string.IsNullOrEmpty(temp.Match_Type.ToString()) ? 0 : temp.Match_Type,
//                                 string.IsNullOrEmpty(temp.Match_Rule.ToString()) ? 0 : temp.Match_Rule,
//                                 string.IsNullOrEmpty(temp.Monitor_Match_Raito.ToString()) ? 0 : temp.Monitor_Match_Raito,
//                                 string.IsNullOrEmpty(temp.VariableMonitor__Qty.ToString()) ? 0 : temp.VariableMonitor__Qty,
//                                 string.IsNullOrEmpty(temp.Technician_Match_Raito.ToString()) ? 0 : temp.Technician_Match_Raito,
//                                 string.IsNullOrEmpty(temp.VariableTechnician__Qty.ToString()) ? 0 : temp.VariableTechnician__Qty);
//                    #endregion
//                    DataContext.Database.ExecuteSqlCommand(sql);
//                }
//            }
//            catch (Exception ex)
//            {
//                result = ex.Message;
//                log.Error(ex);
//            }
//            return result;
//        }


//        public Flowchart_Detail_ProductionPlanning QueryFlowChartByFlowchart_Detail_ME_UID(int Flowchart_Detail_ME_UID)
//        {
//            string sql = @"SELECT  
//							FM.FlowChart_Master_UID ,
//							ME.Flowchart_Detail_ME_UID ,
//							ME.Process ,
//							ME.Process_Seq ,
//							ME.Process_Station ,
//							SFP.FunPlant ,
//							ME.System_FunPlant_UID,
//							ME.Process_Desc ,
//							mE.Processing_Equipment ,
//							ME.Automation_Equipment ,
//							me.Equipment_CT ,
//							me.Setup_Time ,
//							me.Total_Cycletime ,
//							me.Manpower_Ratio ,
//							me.Estimate_Yield ,
//							me.Capacity_ByHour ,
//							me.Capacity_ByDay ,
//							me.Equipment_RequstQty,
//							FD_IE.Flowchart_Detail_IE_UID,
//							FD_IE.VariationEquipment_RequstQty ,
//							FD_IE.VariationOP_Qty,
//							FD_IE.RegularOP_Qty,
//							FD_IE.MaterialKeeper_Qty,
//							FD_IE.Others_Qty,
//							FD_IE.Notes,
//							SquadLeader_Raito,
//							SquadLeader_Variable_Qty,
//							Technician_Raito,
//							Technician_Variable_Qty,
//							SquadLeader_Qty,
//							Technician_Qty,
//							'Edit' AS IEFlag                                    
//		            FROM    dbo.FlowChart_Master FM 
//		            INNER JOIN dbo.FlowChart_Detail_ME ME ON ME.FlowChart_Master_UID = FM.FlowChart_Master_UID
//		            AND ME.FlowChart_Version = FM.FlowChart_Version
//		            LEFT JOIN dbo.Flowchart_Detail_IE FD_IE ON FD_IE.Flowchart_Detail_ME_UID = ME.Flowchart_Detail_ME_UID
//		            INNER JOIN dbo.System_Function_Plant SFP ON SFP.System_FunPlant_UID = ME.System_FunPlant_UID
//		            WHERE   ME.Flowchart_Detail_ME_UID = {0}";
//            sql = string.Format(sql, Flowchart_Detail_ME_UID);
//            var item = DataContext.Database.SqlQuery<Flowchart_Detail_ProductionPlanning>(sql).FirstOrDefault();
//            return item;
//        }

//        public List<HumanResourcesSummaryDTO> GetHumanResourcesSummary(string strWhere)
//        {
//            string sql = string.Format(@"select e.Project_Name,b.Estimate_Yield from [dbo].[Flowchart_Detail_IE] as a 
//	  inner join [dbo].[Flowchart_Detail_ME] as b on a.[Flowchart_Detail_ME_UID]=b.[Flowchart_Detail_ME_UID]
//	  inner join dbo.System_Function_Plant as c on b.[System_FunPlant_UID]=c.[System_FunPlant_UID]
//	  inner join dbo.FlowChart_Master as d on b.[FlowChart_Master_UID]=d.[FlowChart_Master_UID] 
//	  inner join dbo.System_Project as e on d.Project_UID=e.Project_UID {0}", strWhere);
//            var ff = DataContext.Database.SqlQuery<HumanProjectInfo>(sql).ToList();
//            decimal num = 100.00M;
//            string projectName = string.Empty;
//            foreach (var t in ff)
//            {
//                num = num * t.Estimate_Yield;
//                projectName = t.Project_Name;
//            }
//            sql = string.Format(@"select c.FunPlant as FunPlant,isnull(2*(sum(a.[VariationOP_Qty])+sum(a.[RegularOP_Qty])+sum(a.[Others_Qty])),0) as OPNow,isnull(2*sum(a.[MaterialKeeper_Qty]),0) as MaterialKeeperNow,
//	  isnull(2*sum(a.[SquadLeader_Qty]),0) as SquadLeaderNow,isnull(2*sum(a.[Technician_Qty]),0) as TechnicianNow from [dbo].[Flowchart_Detail_IE] as a 
//	  inner join [dbo].[Flowchart_Detail_ME] as b on a.[Flowchart_Detail_ME_UID]=b.[Flowchart_Detail_ME_UID]
//	  inner join dbo.System_Function_Plant as c on b.[System_FunPlant_UID]=c.[System_FunPlant_UID]
//	  inner join dbo.FlowChart_Master as d on b.[FlowChart_Master_UID]=d.[FlowChart_Master_UID] {0} group by c.[System_FunPlant_UID],c.FunPlant", strWhere);
//            var f = DataContext.Database.SqlQuery<HumanResourcesSummaryDTO>(sql).ToList();
//            foreach(var t in f)
//            {
//                float i = t.MaterialKeeperNow * 7 / 6;
//                t.MaterialKeeperRound =(int)Math.Ceiling(i);
//                i = t.OPNow * 7 / 6;
//                t.OPRound= (int)Math.Ceiling(i);
//                i = t.SquadLeaderNow * 7 / 6;
//                t.SquadLeaderRound= (int)Math.Ceiling(i);
//                i=t.TechnicianNow * 7 / 6;
//                t.TechnicianRound= (int)Math.Ceiling(i);
//                t.TotalNow = t.MaterialKeeperNow + t.OPNow + t.SquadLeaderNow + t.TechnicianNow;
//                i = t.TotalNow * 7 / 6;
//                t.TotalRound= (int)Math.Ceiling(i);
//                t.OnePass = num;
//                t.FlowerName = projectName;
//            }
//            return f;
//        }


//        public List<List<PlanDataView>> GetPlanDataByProject(int site, int op, int ProjectID, int PartTypeUID, string begin)
//        {
//            string sql = GetIESumSql(site, op, ProjectID, PartTypeUID, begin);
//            var f = DataContext.Database.SqlQuery<ProjectPlanDTO>(sql).ToList();
//            //f = f.Where(a => a.Product_Date_MP >= DateTime.Parse(begin)||a.Product_Date_MP==null).ToList();
//            //f = f.OrderBy(c => c.Flowchart_Detail_ME_UID).ThenByDescending(c => c.Product_Date_MP).Take(200).ToList();
//            List<List<PlanDataView>> vmlist = new List<List<PlanDataView>>();
//            int tmpNum = 0;
//            foreach (ProjectPlanDTO d in f)
//            {
                
//                //d.Pink = decimal.ToInt32(decimal.Floor(d.Capacity_ByDay / d.Estimate_Yield));
//                //if (d.Product_Date_MP != null)
//                //{
//                    d.DateInWeek = d.Product_Date_MP.Value.DayOfWeek;
//                    d.WeekOfMonth = DateCompareHelper.WeekOfMonth(d.Product_Date_MP.Value, 1);
//                //}
//                //else
//                //{

//                //    DateTime time = DateTime.Parse(begin).AddDays(tmpNum);
//                //    d.Product_Date_MP = time;
//                //    d.DateInWeek = time.DayOfWeek;
//                //    d.WeekOfMonth = DateCompareHelper.WeekOfMonth(time, 1);
//                //    tmpNum++;
//                //}
//            }
//            var t = f.GroupBy(a => a.WeekOfMonth);
            
//            foreach (var tmp in t)
//            {
//                List<PlanDataView> list = new List<PlanDataView>();
//                foreach (var m in tmp)
//                {
//                    PlanDataView view;
//                    bool flag = false;
//                    if (list.FirstOrDefault(a => a.Process == m.Project_Name) == null)
//                    {
//                        view = new PlanDataView();
//                        flag = true;
//                    }
//                    else
//                    {
//                        view = list.FirstOrDefault(a => a.Process == m.Project_Name);
//                    }
//                    view.Pink = m.Pink;
//                    view.Process = m.Project_Name;
//                    view.WeekOfMonth = m.WeekOfMonth;
//                    view.Product_Date_MP = m.Product_Date_MP.Value;
//                    switch (m.DateInWeek)
//                    {
//                        case DayOfWeek.Monday:
//                            view.Monday = m.Pink;
//                            break;
//                        case DayOfWeek.Tuesday:
//                            view.Tuesday = m.Pink;
//                            break;
//                        case DayOfWeek.Wednesday:
//                            view.Wednesday = m.Pink;
//                            break;
//                        case DayOfWeek.Thursday:
//                            view.Thursday = m.Pink;
//                            break;
//                        case DayOfWeek.Friday:
//                            view.Friday = m.Pink;
//                            break;
//                        case DayOfWeek.Saturday:
//                            view.Saturday = m.Pink;
//                            break;
//                        case DayOfWeek.Sunday:
//                            view.Sunday = m.Pink;
//                            break;

//                    }
//                    if (flag)
//                    {
//                        list.Add(view);
//                    }
//                }
//                vmlist.Add(list);
//            }
            
//            return vmlist;
//        }

//        private string GetIESumSql(int site, int op, int ProjectID, int PartTypeUID, string begin)
//        {
//            string sql = string.Empty;
//            if (op == 0) //Business Group没有选择直接取整个site的数据
//            {
//                sql = @";WITH 
//                one_Q AS 
//                (
//	                SELECT Organization_UID,Organization_Name FROM dbo.System_Organization
//                    WHERE Organization_UID IN 
//	                (SELECT ChildOrg_UID FROM dbo.System_OrganizationBOM WHERE ParentOrg_UID={0})
//                ),
//                two_Q AS
//                (
//	                SELECT A.*,one_Q.Organization_Name FROM dbo.FlowChart_Master A 
//	                JOIN one_Q 
//	                ON A.Organization_UID = one_Q.Organization_UID
//                ),";
//                sql = string.Format(sql, site);
//            }
//            else
//            {
//                if (ProjectID == 0) //专案没有选择
//                {
//                    sql = @";WITH 
//                        two_Q AS 
//                        (
//                            SELECT A.* FROM dbo.FlowChart_Master A WHERE A.Organization_UID = {0}
//                        ),";
//                    sql = string.Format(sql, op);
//                } //四个下拉框都有值
//                else
//                {
//                    sql = @";WITH 
//                            two_Q AS 
//                            (
//                             SELECT A.* FROM dbo.FlowChart_Master A WHERE A.FlowChart_Master_UID = {0}
//                            ),";
//                    sql = string.Format(sql, PartTypeUID);
//                }
//            }

//            sql = sql + string.Format(@"
//                            two_Q AS
//                            (
//                             SELECT A.FlowChart_Master_UID,A.FlowChart_Version,B.Flowchart_Detail_ME_UID,
//                             B.System_FunPlant_UID,A.Project_Name,A.Product_Phase,B.Process,B.Capacity_ByDay,B.Estimate_Yield, C.Flowchart_Detail_ME_Equipment_UID, C.Equipment_Name,C.Ratio,
//                             E.Product_Date AS Product_Date_NPI,    --NPI日期
//                             ISNULL(E.Input,0) AS Input_Qty_NPI,    --NPI产能
//                             ISNULL(CEILING(E.Input * B.Estimate_Yield/B.Capacity_ByHour/20*C.Ratio),0) AS Request_NPI --NPI需求数量
//                             FROM one_Q A
//                             JOIN dbo.Flowchart_Detail_ME B
//                             ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
//                             AND A.FlowChart_Version = B.FlowChart_Version
//                             JOIN dbo.Flowchart_Detail_ME_Equipment C
//                             ON B.Flowchart_Detail_ME_UID = C.Flowchart_Detail_ME_UID
//                             LEFT JOIN dbo.Production_Schedul_NPI E
//                             ON A.FlowChart_Master_UID = E.FlowChart_Master_UID
//                             --WHERE 
//                             --CONVERT(CHAR(7),Product_Date,120) = @time

//                            ),
//                            three_Q AS 
//                            (
//                             SELECT A.FlowChart_Master_UID,A.FlowChart_Version,B.Flowchart_Detail_ME_UID,
//                             B.System_FunPlant_UID,A.Project_Name,A.Product_Phase,B.Process,B.Capacity_ByDay,B.Estimate_Yield, C.Flowchart_Detail_ME_Equipment_UID, C.Equipment_Name,C.Ratio,
//                             G.Product_Date AS Product_Date_MP,     --MP日期
//                             ISNULL(G.Input_Qty,0) AS Input_Qty_MP, --MP产能
//                             ISNULL(CEILING(G.Input_Qty / B.Estimate_Yield), 0) AS Pink,
//                             ISNULL(CEILING(G.Input_Qty * B.Estimate_Yield/B.Capacity_ByHour/20*C.Ratio),0) AS Request_MP --MP需求数量
//                             FROM one_Q A
//                             JOIN dbo.Flowchart_Detail_ME B
//                             ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
//                             AND A.FlowChart_Version = B.FlowChart_Version
//                             JOIN dbo.Flowchart_Detail_ME_Equipment C
//                             ON B.Flowchart_Detail_ME_UID = C.Flowchart_Detail_ME_UID
//                             LEFT JOIN dbo.Flowchart_Mapping F
//                             ON B.Flowchart_Detail_ME_UID = F.Flowchart_Detail_ME_UID
//                             LEFT JOIN dbo.Production_Schedul G
//                             ON A.FlowChart_Master_UID = G.FlowChart_Master_UID
//                             WHERE G.Product_Date >= CONVERT(CHAR(10),'{0}',120)
//                             --WHERE G.PlanType = @QueryMode
//                             --AND G.Product_Date >= @StartDate AND G.Product_Date <= @EndDate
//                            ),
//                            four_Q AS 
//                            (
//                             --SELECT three_Q.FlowChart_Master_UID,three_Q.FlowChart_Version,three_Q.Organization_UID,three_Q.Organization_Name AS Organization_Name ,three_Q.Flowchart_Detail_ME_UID,
//                             --three_Q.System_FunPlant_UID,three_Q.Capacity_ByHour,three_Q.Estimate_Yield,three_Q.FunPlant, three_Q.Flowchart_Detail_ME_Equipment_UID, three_Q.Equipment_Name,three_Q.Ratio,
//                             --three_Q.Product_Date_NPI AS Product_Date,three_Q.Input_Qty_NPI AS Input_Qty, three_Q.Request_NPI AS Request_MPANDNPI -- 关键是这一句把上面两段不同的字段整合在一起
//                             --FROM three_Q
//                             --UNION 
//                             SELECT * FROM three_Q
//                            )
//                            --INSERT INTO #BaseRequestQtyTable
//                            SELECT * FROM three_Q", begin);

//            return sql;
//        }


//        public List<List<PlanDataView>> GetInputDataByProject(int ProjectID, int OrgId, int flowchatMasterId, string begin)
//        {
//            string sql = string.Empty;
//            if (ProjectID > 0)
//            {
//                sql = string.Format(@";WITH 
//                            one_Q AS 
//                            (
//                             SELECT A.FlowChart_Master_UID,A.FlowChart_Version,B.Project_Name,A.Product_Phase FROM dbo.FlowChart_Master A 
//                             JOIN dbo.System_Project B
//                             ON A.Project_UID = B.Project_UID
//                             WHERE A.Organization_UID ={0} AND A.Project_UID = {1}
//                             AND ({2} = 0 OR A.FlowChart_Master_UID = {3})
//                            ),
//                            two_Q AS
//                            (
//                             SELECT A.FlowChart_Master_UID,A.FlowChart_Version,B.Flowchart_Detail_ME_UID,
//                             B.System_FunPlant_UID,A.Project_Name,A.Product_Phase,B.Process,B.Capacity_ByDay,B.Estimate_Yield, C.Flowchart_Detail_ME_Equipment_UID, C.Equipment_Name,C.Ratio,
//                             E.Product_Date AS Product_Date,    --NPI日期
//                             ISNULL(E.Input,0) AS Input_Qty,    --NPI产能
//                             ISNULL(CEILING(E.Input * B.Estimate_Yield/B.Capacity_ByHour/20*C.Ratio),0) AS Request_NPI --NPI需求数量
//                             FROM one_Q A
//                             JOIN dbo.Flowchart_Detail_ME B
//                             ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
//                             AND A.FlowChart_Version = B.FlowChart_Version
//                             JOIN dbo.Flowchart_Detail_ME_Equipment C
//                             ON B.Flowchart_Detail_ME_UID = C.Flowchart_Detail_ME_UID
//                             LEFT JOIN dbo.Production_Schedul_NPI E
//                             ON A.FlowChart_Master_UID = E.FlowChart_Master_UID
//                             where a.Product_Phase='NPI'
//                             --WHERE 
//                             --CONVERT(CHAR(7),Product_Date,120) = @time

//                            ),
//                            three_Q AS 
//                            (
//                             SELECT A.FlowChart_Master_UID,A.FlowChart_Version,B.Flowchart_Detail_ME_UID,
//                             B.System_FunPlant_UID,A.Project_Name,A.Product_Phase,B.Process,B.Capacity_ByDay,B.Estimate_Yield, C.Flowchart_Detail_ME_Equipment_UID, C.Equipment_Name,C.Ratio,
//                             G.Product_Date AS Product_Date,     --MP日期
//                             ISNULL(G.Input_Qty,0) AS Input_Qty, --MP产能
//                             ISNULL(CEILING(G.Input_Qty * B.Estimate_Yield/B.Capacity_ByHour/20*C.Ratio),0) AS Request_MP --MP需求数量
//                             FROM one_Q A
//                             JOIN dbo.Flowchart_Detail_ME B
//                             ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
//                             AND A.FlowChart_Version = B.FlowChart_Version
//                             JOIN dbo.Flowchart_Detail_ME_Equipment C
//                             ON B.Flowchart_Detail_ME_UID = C.Flowchart_Detail_ME_UID
//                             LEFT JOIN dbo.Flowchart_Mapping F
//                             ON B.Flowchart_Detail_ME_UID = F.Flowchart_Detail_ME_UID
//                             LEFT JOIN dbo.Production_Schedul G
//                             ON A.FlowChart_Master_UID = G.FlowChart_Master_UID
//                             where a.Product_Phase='MP'
//                             --WHERE G.PlanType = @QueryMode
//                             --AND G.Product_Date >= @StartDate AND G.Product_Date <= @EndDate
//                            ),
//                            four_Q AS 
//                            (
//                             SELECT * FROM three_Q union select * from two_Q
//                            )
//                            --INSERT INTO #BaseRequestQtyTable
//                            SELECT * FROM four_Q", OrgId, ProjectID, flowchatMasterId, flowchatMasterId);
//            }
//            else
//            {
//                sql = string.Format(@";WITH 
//                            one_Q AS 
//                            (
//                             SELECT A.FlowChart_Master_UID,A.FlowChart_Version,B.Project_Name,A.Product_Phase FROM dbo.FlowChart_Master A 
//                             JOIN dbo.System_Project B
//                             ON A.Project_UID = B.Project_UID
//                             WHERE A.Organization_UID ={0}
//                            ),
//                            two_Q AS
//                            (
//                             SELECT A.FlowChart_Master_UID,A.FlowChart_Version,B.Flowchart_Detail_ME_UID,
//                             B.System_FunPlant_UID,A.Project_Name,A.Product_Phase,B.Process,B.Capacity_ByDay,B.Estimate_Yield, C.Flowchart_Detail_ME_Equipment_UID, C.Equipment_Name,C.Ratio,
//                             E.Product_Date AS Product_Date,    --NPI日期
//                             ISNULL(E.Input,0) AS Input_Qty,    --NPI产能
//                             ISNULL(CEILING(E.Input * B.Estimate_Yield/B.Capacity_ByHour/20*C.Ratio),0) AS Request_NPI --NPI需求数量
//                             FROM one_Q A
//                             JOIN dbo.Flowchart_Detail_ME B
//                             ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
//                             AND A.FlowChart_Version = B.FlowChart_Version
//                             JOIN dbo.Flowchart_Detail_ME_Equipment C
//                             ON B.Flowchart_Detail_ME_UID = C.Flowchart_Detail_ME_UID
//                             LEFT JOIN dbo.Production_Schedul_NPI E
//                             ON A.FlowChart_Master_UID = E.FlowChart_Master_UID
//                            where a.Product_Phase='NPI'
//                             --WHERE 
//                             --CONVERT(CHAR(7),Product_Date,120) = @time

//                            ),
//                            three_Q AS 
//                            (
//                             SELECT A.FlowChart_Master_UID,A.FlowChart_Version,B.Flowchart_Detail_ME_UID,
//                             B.System_FunPlant_UID,A.Project_Name,A.Product_Phase,B.Process,B.Capacity_ByDay,B.Estimate_Yield, C.Flowchart_Detail_ME_Equipment_UID, C.Equipment_Name,C.Ratio,
//                             G.Product_Date AS Product_Date,     --MP日期
//                             ISNULL(G.Input_Qty,0) AS Input_Qty, --MP产能
//                             ISNULL(CEILING(G.Input_Qty * B.Estimate_Yield/B.Capacity_ByHour/20*C.Ratio),0) AS Request_MP --MP需求数量
//                             FROM one_Q A
//                             JOIN dbo.Flowchart_Detail_ME B
//                             ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
//                             AND A.FlowChart_Version = B.FlowChart_Version
//                             JOIN dbo.Flowchart_Detail_ME_Equipment C
//                             ON B.Flowchart_Detail_ME_UID = C.Flowchart_Detail_ME_UID
//                             LEFT JOIN dbo.Flowchart_Mapping F
//                             ON B.Flowchart_Detail_ME_UID = F.Flowchart_Detail_ME_UID
//                             LEFT JOIN dbo.Production_Schedul G
//                             ON A.FlowChart_Master_UID = G.FlowChart_Master_UID
//                            where a.Product_Phase='MP'
//                             --WHERE G.PlanType = @QueryMode
//                             --AND G.Product_Date >= @StartDate AND G.Product_Date <= @EndDate
//                            ),
//                            four_Q AS 
//                            (
//                             SELECT * FROM three_Q union select * from two_Q
//                            )
//                            --INSERT INTO #BaseRequestQtyTable
//                            SELECT * FROM four_Q", OrgId, ProjectID, flowchatMasterId, flowchatMasterId);
//            }
//            var f = DataContext.Database.SqlQuery<ProjectPlanDTO>(sql).ToList();
//            f = f.Where(a => a.Product_Date >= DateTime.Parse(begin)).ToList();
//            var tp = f.GroupBy(p => p.Flowchart_Detail_ME_UID)
//                      .Select(s => s.OrderBy(o => o.Flowchart_Detail_ME_UID).FirstOrDefault()).ToList();
            
//            List<List<PlanDataView>> vmlist = new List<List<PlanDataView>>();
//            foreach (ProjectPlanDTO d in tp)
//            {
//                d.Pink = decimal.ToInt32(decimal.Floor(d.Capacity_ByDay / d.Estimate_Yield));
//                d.DateInWeek = d.Product_Date_MP.Value.DayOfWeek;
//                d.WeekOfMonth = DateCompareHelper.WeekOfMonth(d.Product_Date_MP.Value, 1);
//            }
//            var t = f.GroupBy(a => a.WeekOfMonth);
//            foreach (var tmp in t)
//            {
//                List<PlanDataView> list = new List<PlanDataView>();
//                //foreach (var m in tmp)
//                //{
//                //    PlanDataView view;
//                //    bool flag = false;
//                //    if (list.FirstOrDefault(a => a.Process == m.Process) == null)
//                //    {
//                //        view = new PlanDataView();
//                //        flag = true;
//                //    }
//                //    else
//                //    {
//                //        view = list.FirstOrDefault(a => a.Process == m.Process);
//                //    }
//                //    view.Pink = m.Pink;
//                //    view.Process = m.Process;
//                //    view.WeekOfMonth = m.WeekOfMonth;
//                //    view.Product_Date_MP = m.Product_Date_MP.Value;
//                //    switch (m.DateInWeek)
//                //    {
//                //        case DayOfWeek.Monday:
//                //            view.Monday = m.Pink;
//                //            break;
//                //        case DayOfWeek.Tuesday:
//                //            view.Tuesday = m.Pink;
//                //            break;
//                //        case DayOfWeek.Wednesday:
//                //            view.Wednesday = m.Pink;
//                //            break;
//                //        case DayOfWeek.Thursday:
//                //            view.Thursday = m.Pink;
//                //            break;
//                //        case DayOfWeek.Friday:
//                //            view.Friday = m.Pink;
//                //            break;
//                //        case DayOfWeek.Saturday:
//                //            view.Saturday = m.Pink;
//                //            break;
//                //        case DayOfWeek.Sunday:
//                //            view.Sunday = m.Pink;
//                //            break;

//                //    }
//                //    if (flag)
//                //    {
//                //        list.Add(view);
//                //    }
//                //}
//                vmlist.Add(list);
//            }
//            return vmlist;
//        }

//        public List<HumanResources> GetHumanResourcesByFunplant(int flowchatMasterId)
//        {
//            string strWhere = "where b.FlowChart_Master_UID=" + flowchatMasterId;
//            if (flowchatMasterId == 0)
//            {
//                strWhere = "";
//            }
//            string sql = string.Format(@"select c.FunPlant as FunPlant,p.Product_Phase,2*(isnull(sum(a.[VariationOP_Qty]),0)+isnull(sum(a.[RegularOP_Qty]),0)+isnull(sum(a.[Others_Qty]),0)
//+isnull(sum(a.[MaterialKeeper_Qty]),0)+isnull(sum(a.[SquadLeader_Qty]),0)+isnull(sum(a.[Technician_Qty]),0)) as Total from [dbo].[Flowchart_Detail_IE] as a 
//	  inner join [dbo].[Flowchart_Detail_ME] as b on a.[Flowchart_Detail_ME_UID]=b.[Flowchart_Detail_ME_UID]
//	  inner join dbo.System_Function_Plant as c on b.[System_FunPlant_UID]=c.[System_FunPlant_UID]
//	  inner join dbo.FlowChart_Master as d on b.[FlowChart_Master_UID]=d.[FlowChart_Master_UID] 
//        inner join dbo.System_Project as p on d.Project_UID=p.Project_UID {0} group by c.[System_FunPlant_UID],c.FunPlant,p.Product_Phase", strWhere);
//            var ff = DataContext.Database.SqlQuery<HumanResourcesDTO>(sql).ToList();
//            List<HumanResources> list = new List<HumanResources>();
//            int mp = 0;
//            int npi = 0;
//            int all = 0;
//            foreach (var t in ff)
//            {
//                if (t.Product_Phase == "MP")
//                {
//                    mp += t.Total;
//                }
//                if (t.Product_Phase == "NPI")
//                {
//                    npi += t.Total;
//                }
//                all += t.Total;
//                HumanResources hr = new HumanResources();
//                hr.FunPlant = t.FunPlant;
//                hr.Monday = t.Total;
//                hr.Tuesday = t.Total;
//                hr.Wednesday = t.Total;
//                hr.Thursday = t.Total;
//                hr.Friday = t.Total;
//                hr.Saturday = t.Total;
//                hr.Sunday = t.Total;
//                list.Add(hr);
//            }
//            HumanResources hrMP = new HumanResources();
//            hrMP.FunPlant = "MP人力需求";
//            hrMP.Monday = mp;
//            hrMP.Tuesday = mp;
//            hrMP.Wednesday = mp;
//            hrMP.Thursday = mp;
//            hrMP.Friday = mp;
//            hrMP.Saturday = mp;
//            hrMP.Sunday = mp;
//            list.Add(hrMP);
//            HumanResources hrNPI = new HumanResources();
//            hrNPI.FunPlant = "NPI人力需求";
//            hrNPI.Monday = npi;
//            hrNPI.Tuesday = npi;
//            hrNPI.Wednesday = npi;
//            hrNPI.Thursday = npi;
//            hrNPI.Friday = npi;
//            hrNPI.Saturday = npi;
//            hrNPI.Sunday = npi;
//            list.Add(hrNPI);
//            HumanResources hrAll = new HumanResources();
//            hrAll.FunPlant = "需求总人力";
//            hrAll.Monday = all;
//            hrAll.Tuesday = all;
//            hrAll.Wednesday = all;
//            hrAll.Thursday = all;
//            hrAll.Friday = all;
//            hrAll.Saturday = all;
//            hrAll.Sunday = all;
//            list.Add(hrAll);
//            return list;
//        }

//        public List<HumanResources> GetHumanResourcesByProject(int flowchatMasterId)
//        {
//            string strWhere = "where b.FlowChart_Master_UID=" + flowchatMasterId;
//            if (flowchatMasterId == 0)
//            {
//                strWhere = "";
//            }
//            string sql = string.Format(@"  select p.Project_Name+'_'+d.Part_Types as Project,p.Product_Phase,2*(isnull(sum(a.[VariationOP_Qty]),0)+isnull(sum(a.[RegularOP_Qty]),0)+isnull(sum(a.[Others_Qty]),0)
//+isnull(sum(a.[MaterialKeeper_Qty]),0)+isnull(sum(a.[SquadLeader_Qty]),0)+isnull(sum(a.[Technician_Qty]),0)) as Total from [dbo].[Flowchart_Detail_IE] as a 
//	  inner join [dbo].[Flowchart_Detail_ME] as b on a.[Flowchart_Detail_ME_UID]=b.[Flowchart_Detail_ME_UID]
//	  inner join dbo.System_Function_Plant as c on b.[System_FunPlant_UID]=c.[System_FunPlant_UID]
//	  left join dbo.FlowChart_Master as d on b.[FlowChart_Master_UID]=d.[FlowChart_Master_UID] and b.[FlowChart_Version]=d.[FlowChart_Version]
//	  inner join dbo.System_Project as p on d.Project_UID=p.Project_UID {0}  group by p.Project_Name,d.Part_Types,p.Product_Phase", strWhere);
//            var ff = DataContext.Database.SqlQuery<HumanResourcesDTO>(sql).ToList();
//            List<HumanResources> list = new List<HumanResources>();
//            int mp = 0;
//            int npi = 0;
//            int all = 0;
//            foreach (var t in ff)
//            {
//                if (t.Product_Phase == "MP")
//                {
//                    mp += t.Total;
//                }
//                if (t.Product_Phase == "NPI")
//                {
//                    npi += t.Total;
//                }
//                all += t.Total;
//                HumanResources hr = new HumanResources();
//                hr.Project = t.Project;
//                hr.Monday = t.Total;
//                hr.Tuesday = t.Total;
//                hr.Wednesday = t.Total;
//                hr.Thursday = t.Total;
//                hr.Friday = t.Total;
//                hr.Saturday = t.Total;
//                hr.Sunday = t.Total;
//                list.Add(hr);
//            }
//            HumanResources hrMP = new HumanResources();
//            hrMP.Project = "MP人力需求";
//            hrMP.Monday = mp;
//            hrMP.Tuesday = mp;
//            hrMP.Wednesday = mp;
//            hrMP.Thursday = mp;
//            hrMP.Friday = mp;
//            hrMP.Saturday = mp;
//            hrMP.Sunday = mp;
//            list.Add(hrMP);
//            HumanResources hrNPI = new HumanResources();
//            hrNPI.Project = "NPI人力需求";
//            hrNPI.Monday = npi;
//            hrNPI.Tuesday = npi;
//            hrNPI.Wednesday = npi;
//            hrNPI.Thursday = npi;
//            hrNPI.Friday = npi;
//            hrNPI.Saturday = npi;
//            hrNPI.Sunday = npi;
//            list.Add(hrNPI);
//            HumanResources hrAll = new HumanResources();
//            hrAll.Project = "需求总人力";
//            hrAll.Monday = all;
//            hrAll.Tuesday = all;
//            hrAll.Wednesday = all;
//            hrAll.Thursday = all;
//            hrAll.Friday = all;
//            hrAll.Saturday = all;
//            hrAll.Sunday = all;
//            list.Add(hrAll);
//            return list;
//        }

//        public List<HumanInfo> GetNowHumanByBG(int bgOrgID,DateTime begindate)
//        {
//            int diff = 28;
//            List<DateTime> timeList = new List<DateTime>();
//            for (int j = 0; j < diff; j++)
//            {
//                timeList.Add(begindate.AddDays(j));
//            }
//            string sql = string.Format(@"select 2*(isnull([OP_Qty],0)+isnull([Monitor_Staff_Qty],0)+isnull([Technical_Staff_Qty],0)
//  +isnull([Material_Keeper_Qty],0)+isnull([Others_Qty],0)) 
//  as TotalNum, [Product_Phase],[ProductDate] from [dbo].[Current_Staff] where [BG_Organization_UID]={0} and [ProductDate]>'{1}' and [ProductDate]<'{2}'", 
//  bgOrgID, begindate, begindate.AddDays(28));
//            var ff = DataContext.Database.SqlQuery<NowHumanDTO>(sql).ToList();
//            foreach (DateTime time in timeList)
//            {
//                var tp = ff.Find(a => a.ProductDate == time);
//                if (tp== null)
//                {
//                    NowHumanDTO nh = new NowHumanDTO();
//                    nh.TotalNum = 0;
//                    //nh.Product_Phase = "MP";
//                    nh.ProductDate = time;
//                    ff.Add(nh);
//                }
                
//            }
//            foreach (var t in ff)
//            {
//                t.DateInWeek = t.ProductDate.DayOfWeek;
//                t.WeekOfMonth = DateCompareHelper.WeekOfMonth(t.ProductDate, 1);
//            }
//            var f = ff.GroupBy(a => a.WeekOfMonth);
//            List<HumanInfo> list = new List<HumanInfo>();
//            foreach (var tmp in f)
//            {
//                HumanInfo hi = new HumanInfo();
//                List<NowHuman> list1 = new List<NowHuman>();
//                List<NowHuman> list2 = new List<NowHuman>();
//                NowHuman MPlist = new NowHuman();
//                NowHuman NPIlist = new NowHuman();
                
//                NowHuman view = new NowHuman();
//                foreach (var m in tmp)
//                {
//                    switch (m.DateInWeek)
//                    {
//                        case DayOfWeek.Monday:
//                            view.Monday = m.TotalNum;
//                            view.ProductDate = m.ProductDate;
//                            //view.Product_Phase = m.Product_Phase;
//                            break;
//                        case DayOfWeek.Tuesday:
//                            view.Tuesday = m.TotalNum;
//                            view.ProductDate = m.ProductDate;
//                            //view.Product_Phase = m.Product_Phase;
//                            break;
//                        case DayOfWeek.Wednesday:
//                            view.Wednesday = m.TotalNum;
//                            view.ProductDate = m.ProductDate;
//                            //view.Product_Phase = m.Product_Phase;
//                            break;
//                        case DayOfWeek.Thursday:
//                            view.Thursday = m.TotalNum;
//                            view.ProductDate = m.ProductDate;
//                            //view.Product_Phase = m.Product_Phase;
//                            break;
//                        case DayOfWeek.Friday:
//                            view.Friday = m.TotalNum;
//                            view.ProductDate = m.ProductDate;
//                            //view.Product_Phase = m.Product_Phase;
//                            break;
//                        case DayOfWeek.Saturday:
//                            view.Saturday = m.TotalNum;
//                            view.ProductDate = m.ProductDate;
//                            //view.Product_Phase = m.Product_Phase;
//                            break;
//                        case DayOfWeek.Sunday:
//                            view.Sunday = m.TotalNum;
//                            view.ProductDate = m.ProductDate;
//                            //view.Product_Phase = m.Product_Phase;
//                            break;

//                    }
//                    //if (m.Product_Phase == "MP")
//                    //{
//                    //    MPlist=view;
//                    //}
//                    //else
//                    //{
//                    //    NPIlist =view;
//                    //}
//                    MPlist.Title = "现有总人力－MP";
//                    NPIlist.Title = "现有总人力－NPI";
//                }
//                list1.Add(MPlist);
//                list2.Add(NPIlist);
//                hi.MPHuman = list1;
//                hi.NPIHuman = list2;
//                list.Add(hi);
//            }

//            return list;
//        }

//        public List<HumanInfo> GetDemissionRateByBG(int bgOrgID, DateTime begindate)
//        {
//            int diff = 28;
//            List<DateTime> timeList = new List<DateTime>();
//            for (int j = 0; j < diff; j++)
//            {
//                timeList.Add(begindate.AddDays(j));
//            }
//            string sql = string.Format(@"  select isnull([DemissionRate_NPI],0) as DRN,isnull([DemissionRate_MP],0) as DRM,(isnull([NPI_RecruitStaff_Qty],0)
//  +isnull([MP_RecruitStaff_Qty],0)) 
//  as Recruit, [Product_Date] from [dbo].[DemissionRateAndWorkSchedule]  where [BG_Organization_UID]={0} and [Product_Date]>'{1}' and [Product_Date]<'{2}'",
//  bgOrgID, begindate, begindate.AddDays(28));
//            var ff = DataContext.Database.SqlQuery<NowHumanDTO>(sql).ToList();
//            foreach (DateTime time in timeList)
//            {
//                var tp = ff.Find(a => a.Product_Date == time);
//                if (tp == null)
//                {
//                    NowHumanDTO nh = new NowHumanDTO();
//                    nh.DRN = 0;
//                    nh.DRM = 0;
//                    nh.Recruit = 0;
//                    //nh.Product_Phase = "MP";
//                    nh.Product_Date = time;
//                    ff.Add(nh);
//                }

//            }
//            foreach (var t in ff)
//            {
//                t.DateInWeek = t.Product_Date.DayOfWeek;
//                t.WeekOfMonth = DateCompareHelper.WeekOfMonth(t.Product_Date, 1);
//            }
//            var f = ff.GroupBy(a => a.WeekOfMonth);
//            List<HumanInfo> list = new List<HumanInfo>();
//            foreach (var tmp in f)
//            {
//                HumanInfo hi = new HumanInfo();
//                List<NowDemissionRate> list1 = new List<NowDemissionRate>();
//                List<NowDemissionRate> list2 = new List<NowDemissionRate>();
//                List<NowHuman> list3 = new List<NowHuman>();
//                NowDemissionRate MPlist = new NowDemissionRate();
//                NowDemissionRate NPIlist = new NowDemissionRate();
//                NowHuman Recruitlist = new NowHuman();
//                NowHuman view = new NowHuman();
//                foreach (var m in tmp)
//                {
//                    //if (m.Product_Phase == "MP")
//                    //{
//                        switch (m.DateInWeek)
//                        {
//                            case DayOfWeek.Monday:
//                                MPlist.Monday =(m.DRM*100).ToString()+'%';
//                                NPIlist.Monday = (m.DRN * 100).ToString() + '%';
//                                Recruitlist.Monday = m.Recruit;
//                                MPlist.ProductDate = m.Product_Date;
//                                //MPlist.Product_Phase = m.Product_Phase;
//                                break;
//                            case DayOfWeek.Tuesday:
//                                MPlist.Tuesday = (m.DRM * 100).ToString() + '%';
//                            NPIlist.Tuesday = (m.DRN * 100).ToString() + '%';
//                            Recruitlist.Tuesday = m.Recruit;
//                                MPlist.ProductDate = m.Product_Date;
//                                //MPlist.Product_Phase = m.Product_Phase;
//                                break;
//                            case DayOfWeek.Wednesday:
//                                MPlist.Wednesday = (m.DRM * 100).ToString() + '%';
//                            NPIlist.Wednesday = (m.DRN * 100).ToString() + '%';
//                            Recruitlist.Wednesday = m.Recruit;
//                                MPlist.ProductDate = m.Product_Date;
//                                //MPlist.Product_Phase = m.Product_Phase;
//                                break;
//                            case DayOfWeek.Thursday:
//                                MPlist.Thursday = (m.DRM * 100).ToString() + '%';
//                            NPIlist.Thursday = (m.DRN * 100).ToString() + '%';
//                            Recruitlist.Thursday = m.Recruit;
//                                MPlist.ProductDate = m.Product_Date;
//                                //MPlist.Product_Phase = m.Product_Phase;
//                                break;
//                            case DayOfWeek.Friday:
//                                MPlist.Friday = (m.DRM * 100).ToString() + '%';
//                            NPIlist.Friday = (m.DRN * 100).ToString() + '%';
//                            Recruitlist.Friday = m.Recruit;
//                                MPlist.ProductDate = m.Product_Date;
//                                //MPlist.Product_Phase = m.Product_Phase;
//                                break;
//                            case DayOfWeek.Saturday:
//                                MPlist.Saturday = (m.DRM * 100).ToString() + '%';
//                            NPIlist.Saturday = (m.DRN * 100).ToString() + '%';
//                            Recruitlist.Saturday = m.Recruit;
//                                MPlist.ProductDate = m.Product_Date;
//                                //MPlist.Product_Phase = m.Product_Phase;
//                                break;
//                            case DayOfWeek.Sunday:
//                                MPlist.Sunday = (m.DRM * 100).ToString() + '%';
//                            NPIlist.Sunday = (m.DRN * 100).ToString() + '%';
//                            Recruitlist.Sunday = m.Recruit;
//                                MPlist.ProductDate = m.Product_Date;
//                                //MPlist.Product_Phase = m.Product_Phase;
//                                break;

//                        }
//                    //}
//                    //else
//                    //{
//                    //    switch (m.DateInWeek)
//                    //    {
//                    //        case DayOfWeek.Monday:
//                    //            NPIlist.Monday = (m.DRN * 100).ToString() + '%';
//                    //            Recruitlist.Monday = m.Recruit;
//                    //            NPIlist.ProductDate = m.ProductDate;
//                    //            NPIlist.Product_Phase = m.Product_Phase;
//                    //            break;
//                    //        case DayOfWeek.Tuesday:
//                    //            NPIlist.Tuesday = (m.DRN * 100).ToString() + '%';
//                    //            Recruitlist.Tuesday = m.Recruit;
//                    //            NPIlist.ProductDate = m.ProductDate;
//                    //            NPIlist.Product_Phase = m.Product_Phase;
//                    //            break;
//                    //        case DayOfWeek.Wednesday:
//                    //            NPIlist.Wednesday = (m.DRN * 100).ToString() + '%';
//                    //            Recruitlist.Wednesday = m.Recruit;
//                    //            NPIlist.ProductDate = m.ProductDate;
//                    //            NPIlist.Product_Phase = m.Product_Phase;
//                    //            break;
//                    //        case DayOfWeek.Thursday:
//                    //            NPIlist.Thursday = (m.DRN * 100).ToString() + '%';
//                    //            Recruitlist.Thursday = m.Recruit;
//                    //            NPIlist.ProductDate = m.ProductDate;
//                    //            NPIlist.Product_Phase = m.Product_Phase;
//                    //            break;
//                    //        case DayOfWeek.Friday:
//                    //            NPIlist.Friday = (m.DRN * 100).ToString() + '%';
//                    //            Recruitlist.Friday = m.Recruit;
//                    //            NPIlist.ProductDate = m.ProductDate;
//                    //            NPIlist.Product_Phase = m.Product_Phase;
//                    //            break;
//                    //        case DayOfWeek.Saturday:
//                    //            NPIlist.Saturday = (m.DRN * 100).ToString() + '%';
//                    //            Recruitlist.Saturday = m.Recruit;
//                    //            NPIlist.ProductDate = m.ProductDate;
//                    //            NPIlist.Product_Phase = m.Product_Phase;
//                    //            break;
//                    //        case DayOfWeek.Sunday:
//                    //            NPIlist.Sunday = (m.DRN * 100).ToString() + '%';
//                    //            Recruitlist.Sunday = m.Recruit;
//                    //            NPIlist.ProductDate = m.ProductDate;
//                    //            NPIlist.Product_Phase = m.Product_Phase;
//                    //            break;

//                    //    }
//                    //}
//                    MPlist.Title = "MP预计离职率";
//                    NPIlist.Title = "NPI预计离职率";
//                    Recruitlist.Title = "预计招募人力";
//                }
//                list1.Add(MPlist);
//                list2.Add(NPIlist);
//                list3.Add(Recruitlist);
//                hi.MPDemissionRate = list1;
//                hi.NPIDemissionRate = list2;
//                hi.RecruitStaff = list3;
//                list.Add(hi);
//            }

//            return list;
//        }

//        public List<InputDataForSelectDTO> GetInputDataForSelect(int projectId)
//        {
//            string sql = string.Format(@"select Top 5 [Capacity_ByDay],[FlowChart_Version] from [dbo].[Flowchart_Detail_ME] where [FlowChart_Master_UID] in 
//(select [FlowChart_Master_UID] from [dbo].[FlowChart_Master] where [Project_UID]={0}) group by [Capacity_ByDay],[FlowChart_Version]
//order by [Capacity_ByDay] desc",projectId);
//            var ff = DataContext.Database.SqlQuery<InputDataForSelectDTO>(sql).ToList();
//            return ff;
//        }

//        public string ImportFlowchartIE(List<FLowchart_Detail_IE_VM> ieList)
//        {
//            using (var trans = DataContext.Database.BeginTransaction())
//            {
//                var addOrUpdate = ieList.First().IEFlag;
//                var addOrUpdateSql = InsertOrUpdateIESql(ieList, addOrUpdate);
//                DataContext.Database.ExecuteSqlCommand(addOrUpdateSql);
//                trans.Commit();
//            }
//            return "Success";
//        }

//        private string InsertOrUpdateIESql(List<FLowchart_Detail_IE_VM> ieList, string Flag)
//        {
//            StringBuilder sb = new StringBuilder();
//            switch (Flag)
//            {
//                case "Add":
//                    foreach (var ieItem in ieList)
//                    {
//                        var sql = @"INSERT INTO dbo.Flowchart_Detail_IE
//	                                ( Flowchart_Detail_ME_UID ,
//                                      VariationOP_Qty ,
//                                      RegularOP_Qty ,
//                                      MaterialKeeper_Qty ,
//                                      Match_Rule ,
//                                      VariationEquipment_RequstQty ,
//                                      SquadLeader_Raito ,
//                                      SquadLeader_Variable_Qty ,
//                                      Technician_Raito ,
//                                      Technician_Variable_Qty ,
//                                      SquadLeader_Qty ,
//                                      Technician_Qty ,
//                                      Others_Qty ,
//                                      Notes ,
//                                      Created_Date ,
//                                      Created_UID ,
//                                      Modified_Date ,
//                                      Modified_UID	                                
//                                    )
//	                        VALUES  ( {0} , -- Flowchart_Detail_ME_UID - int
//                                      {1} , -- VariationOP_Qty - int
//                                      {2} , -- RegularOP_Qty - int
//                                      {3} , -- MaterialKeeper_Qty - int
//                                      {4} , -- Match_Rule - int
//                                      {5} , -- VariationEquipment_RequstQty - int
//                                      {6} , -- SquadLeader_Raito - decimal
//                                      {7} , -- SquadLeader_Variable_Qty - int
//                                      {8} , -- Technician_Raito - decimal
//                                      {9} , -- Technician_Variable_Qty - int
//                                      {10} , -- SquadLeader_Qty - int
//                                      {11} , -- Technician_Qty - int
//                                      {12} , -- Others_Qty - int
//                                      N'{13}' , -- Notes - nvarchar(200)
//                                      GETDATE() , -- Created_Date - datetime
//                                      {14} , -- Created_UID - int
//                                      GETDATE() , -- Modified_Date - datetime
//                                      {14}  -- Modified_UID - int
//	                                );";
//                        sql = string.Format(sql,ieItem.Flowchart_Detail_ME_UID,
//                            ieItem.VariationOP_Qty,
//                            ieItem.RegularOP_Qty,
//                            ieItem.MaterialKeeper_Qty,
//                            ieItem.Match_Rule,
//                            ieItem.VariationEquipment_RequstQty,
//                            ieItem.SquadLeader_Raito,
//                            ieItem.SquadLeader_Variable_Qty,
//                            ieItem.Technician_Raito,
//                            ieItem.Technician_Variable_Qty,
//                            ieItem.SquadLeader_Qty,
//                            ieItem.Technician_Qty,
//                            ieItem.Others_Qty,
//                            ieItem.Notes,
//                            ieItem.Modified_UID);
//                        sb.AppendLine(sql);
//                    }
//                    break;
//                case "Edit":
//                    foreach (var ieItem in ieList)
//                    {
//                        var sql = @"UPDATE dbo.Flowchart_Detail_IE
//                                    SET 
//                                    VariationOP_Qty = {0},
//                                    RegularOP_Qty = {1},
//                                    MaterialKeeper_Qty = {2},
//                                    Match_Rule = {3},
//                                    VariationEquipment_RequstQty = {4},
//                                    SquadLeader_Raito = {5},
//                                    SquadLeader_Variable_Qty = {6},
//                                    Technician_Raito = {7},
//                                    Technician_Variable_Qty = {8},
//                                    SquadLeader_Qty = {9},
//                                    Technician_Qty = {10},
//                                    Others_Qty = {11},
//                                    Notes = N'{12}',
//                                    Modified_Date = GETDATE(),
//                                    Modified_UID = {13}
//                                    WHERE Flowchart_Detail_IE_UID = {14};";
//                        sql = string.Format(sql,ieItem.VariationOP_Qty,
//                            ieItem.RegularOP_Qty,
//                            ieItem.MaterialKeeper_Qty,
//                            ieItem.Match_Rule,
//                            ieItem.VariationEquipment_RequstQty,
//                            ieItem.SquadLeader_Raito,
//                            ieItem.SquadLeader_Variable_Qty,
//                            ieItem.Technician_Raito,
//                            ieItem.Technician_Variable_Qty,
//                            ieItem.SquadLeader_Qty,
//                            ieItem.Technician_Qty,
//                            ieItem.Others_Qty,
//                            ieItem.Notes,
//                            ieItem.Modified_UID,
//                            ieItem.Flowchart_Detail_IE_UID);
//                        sb.AppendLine(sql);
//                    }
//                    break;
//            }
//            return sb.ToString();
//        }

//        #region 人力汇总报表
//        public List<string> GetAllManPowerProject(ProductionPlanningReportVM vm)
//        {
//            string sql = string.Empty;
//            if (vm.OpTypeUID == 0)
//            {
//                sql = @";WITH 
//                        one_Q AS 
//                        (
//	                        SELECT Organization_UID,Organization_Name FROM dbo.System_Organization
//	                        WHERE Organization_UID IN 
//	                        (SELECT ChildOrg_UID FROM dbo.System_OrganizationBOM WHERE ParentOrg_UID= {0})
//                        ),
//                        two_Q AS
//                        (
//	                        SELECT B.Project_Name + '(' + A.Part_Types + ')' AS PName FROM dbo.FlowChart_Master A 
//	                        JOIN one_Q 
//	                        ON A.Organization_UID = one_Q.Organization_UID
//	                        JOIN System_Project B
//	                        ON A.Project_UID = B.Project_UID
//                        )
//                        SELECT * FROM two_Q";

//                sql = string.Format(sql, vm.PlantUID);
//            }
//            else
//            {
//                if (vm.ProjectUID == 0)
//                {
//                    sql = @";WITH 
//                            two_Q AS 
//                            (
//                                SELECT B.Project_Name + '(' + A.Part_Types + ')' AS PName FROM dbo.FlowChart_Master A
//							    JOIN System_Project B ON A.Project_UID = B.Project_UID
//							    WHERE A.Organization_UID = {0}
//                            )
//                            SELECT * FROM two_Q";
//                    sql = string.Format(sql, vm.OpTypeUID);
//                }
//            }

//            var list = DataContext.Database.SqlQuery<string>(sql).ToList();
//            return list;
//        }


//        public DataTable QueryManPowerRequestRPT(ProductionPlanningReportVM vm)
//        {
//            DataTable dt = new DataTable();
//            var conStr = MethodExtension.GetConnectionStr();
//            using (SqlConnection con = new SqlConnection(conStr))
//            {
//                con.Open();
//                SqlCommand cmd = null;
//                cmd = new SqlCommand("RPT_DlSummary", con);
//                cmd.CommandType = System.Data.CommandType.StoredProcedure;
//                cmd.Parameters.Add(new SqlParameter("@PlantUID", vm.PlantUID));
//                cmd.Parameters.Add(new SqlParameter("@OpTypeUID", vm.OpTypeUID));
//                cmd.Parameters.Add(new SqlParameter("@ProjectUID", vm.ProjectUID));
//                cmd.Parameters.Add(new SqlParameter("@PartTypeUID", vm.PartTypeUID));
//                cmd.Parameters.Add(new SqlParameter("@StartDate", vm.StartDate));
//                cmd.Parameters.Add(new SqlParameter("@EndDate", vm.EndDate));

//                using (SqlDataReader sdr = cmd.ExecuteReader())
//                {
//                    dt.Load(sdr);
//                    sdr.Close();
//                }
//            }
//            return dt;
//        }

//        //获取按专案-MP人力需求，NPI人力需求等
//        public DataTable QueryManPowerRequestByProjectOne(ProductionPlanningReportVM vm)
//        {
//            DataTable dt = new DataTable();
//            var conStr = MethodExtension.GetConnectionStr();
//            using (SqlConnection con = new SqlConnection(conStr))
//            {
//                con.Open();
//                SqlCommand cmd = null;
//                cmd = new SqlCommand("RPT_DlSummaryByProjectOne", con);
//                cmd.CommandType = System.Data.CommandType.StoredProcedure;
//                cmd.Parameters.Add(new SqlParameter("@PlantUID", vm.PlantUID));
//                cmd.Parameters.Add(new SqlParameter("@OpTypeUID", vm.OpTypeUID));
//                cmd.Parameters.Add(new SqlParameter("@ProjectUID", vm.ProjectUID));
//                cmd.Parameters.Add(new SqlParameter("@PartTypeUID", vm.PartTypeUID));
//                cmd.Parameters.Add(new SqlParameter("@StartDate", vm.StartDate));
//                cmd.Parameters.Add(new SqlParameter("@EndDate", vm.EndDate));

//                using (SqlDataReader sdr = cmd.ExecuteReader())
//                {
//                    dt.Load(sdr);
//                    sdr.Close();
//                }
//            }
//            return dt;
//        }

//        //获取按专案-现有总人力MP,现有总人力NPI
//        public DataTable QueryManPowerRequestByProjectOneByTwo(ProductionPlanningReportVM vm)
//        {
//            DataTable dt = new DataTable();
//            var conStr = MethodExtension.GetConnectionStr();
//            string sql = string.Empty;
//            sql = @"SELECT 2*(ISNULL(SUM([OP_Qty]),0) + ISNULL(SUM([Monitor_Staff_Qty]),0) + ISNULL(SUM([Technical_Staff_Qty]),0)
//                    + ISNULL(SUM([Material_Keeper_Qty]),0) + ISNULL(SUM([Others_Qty]),0)) 
//                    AS TotalNum, [Product_Phase],[ProductDate] 
//                    FROM [dbo].[Current_Staff] 
//                    WHERE [ProductDate]>='{0}' and [ProductDate]<='{1}' ";

//            sql = string.Format(sql, vm.StartDate, vm.EndDate);

//            if (vm.OpTypeUID != 0)
//            {
//                sql = sql + " AND [BG_Organization_UID]={0} ";
//                sql = string.Format(sql,vm.OpTypeUID);
//            }

//            sql = sql + "GROUP BY ProductDate,Product_Phase";


//            using (SqlConnection con = new SqlConnection(conStr))
//            {
//                con.Open();
//                SqlCommand cmd = null;
//                cmd = new SqlCommand(sql, con);

//                using (SqlDataReader sdr = cmd.ExecuteReader())
//                {
//                    dt.Load(sdr);
//                    sdr.Close();
//                }
//            }
//            return dt;
//        }

//        //获取按专案-MP预计离职率,NPI预计离职率，预计招募人力
//        public DataTable QueryManPowerRequestByProjectOneByThree(ProductionPlanningReportVM vm)
//        {
//            DataTable dt = new DataTable();
//            var conStr = MethodExtension.GetConnectionStr();
//            string sql = string.Empty;
//            sql = @"SELECT 
//                    ISNULL(AVG([DemissionRate_NPI]),0) as DRN,
//                    ISNULL(AVG([DemissionRate_MP]),0) AS  DRM,
//                    (ISNULL(SUM([NPI_RecruitStaff_Qty]),0) + ISNULL(SUM([MP_RecruitStaff_Qty]),0)) AS Recruit, 
//                    [Product_Date] 
//                    FROM [dbo].[DemissionRateAndWorkSchedule]  
//                    WHERE [Product_Date]>='{0}' and [Product_Date]<='{1}' ";


//            sql = string.Format(sql, vm.StartDate, vm.EndDate);

//            if (vm.OpTypeUID != 0)
//            {
//                sql = sql + " AND [BG_Organization_UID]={0} ";
//                sql = string.Format(sql, vm.OpTypeUID);
//            }

//            sql = sql + "GROUP BY Product_Date";

//            using (SqlConnection con = new SqlConnection(conStr))
//            {
//                con.Open();
//                SqlCommand cmd = null;
//                cmd = new SqlCommand(sql, con);

//                using (SqlDataReader sdr = cmd.ExecuteReader())
//                {
//                    dt.Load(sdr);
//                    sdr.Close();
//                }
//            }
//            return dt;
//        }


//        //获取按功能厂分组
//        public DataTable QueryManPowerRequestByFuncOne(ProductionPlanningReportVM vm)
//        {
//            DataTable dt = new DataTable();
//            var conStr = MethodExtension.GetConnectionStr();
//            string sql = string.Empty;
//            if (vm.OpTypeUID == 0) //整个厂区搜索
//            {
//                sql = @";WITH 
//                            one_Q AS 
//                            (
//	                            SELECT Organization_UID,Organization_Name FROM dbo.System_Organization
//	                            WHERE Organization_UID IN 
//	                            (SELECT ChildOrg_UID FROM dbo.System_OrganizationBOM WHERE ParentOrg_UID= {0})
//                            ),
//                            two_Q AS
//                            (
//	                            SELECT A.FlowChart_Master_UID,A.Product_Phase FROM dbo.FlowChart_Master A 
//	                            JOIN one_Q 
//	                            ON A.Organization_UID = one_Q.Organization_UID
//	                            JOIN System_Project B
//	                            ON A.Project_UID = B.Project_UID
//                            ),
//                            three_Q AS 
//                            (
//                            select C.FunPlant as FunPlant,D.Product_Phase,2*(isnull(sum(a.[VariationOP_Qty]),0)+isnull(sum(a.[RegularOP_Qty]),0)+isnull(sum(a.[Others_Qty]),0)
//                            +isnull(sum(a.[MaterialKeeper_Qty]),0)+isnull(sum(a.[SquadLeader_Qty]),0)+isnull(sum(a.[Technician_Qty]),0)) as Total 
//                            FROM [dbo].[Flowchart_Detail_IE] AS A
//                            inner join [dbo].[Flowchart_Detail_ME] AS B on B.[Flowchart_Detail_ME_UID] = A.[Flowchart_Detail_ME_UID]
//                            inner join dbo.System_Function_Plant AS C on C.[System_FunPlant_UID] = B.[System_FunPlant_UID]
//                            inner join two_Q as D on D.[FlowChart_Master_UID] = B.[FlowChart_Master_UID] 
//                            GROUP BY C.FunPlant,D.Product_Phase
//                            )
//                            SELECT * FROM three_Q";
//                sql = string.Format(sql, vm.PlantUID);

//            }
//            else
//            {
//                if (vm.ProjectUID == 0) //单个OP搜索
//                {
//                    sql = @";WITH 
//                                two_Q AS 
//                                (
//	                                SELECT A.FlowChart_Master_UID,A.Product_Phase FROM dbo.FlowChart_Master A
//	                                JOIN System_Project B ON A.Project_UID = B.Project_UID
//	                                WHERE A.Organization_UID = {0}
//                                ),
//                                three_Q AS 
//                                (
//                                select C.FunPlant as FunPlant,D.Product_Phase,2*(isnull(sum(a.[VariationOP_Qty]),0)+isnull(sum(a.[RegularOP_Qty]),0)+isnull(sum(a.[Others_Qty]),0)
//                                +isnull(sum(a.[MaterialKeeper_Qty]),0)+isnull(sum(a.[SquadLeader_Qty]),0)+isnull(sum(a.[Technician_Qty]),0)) as Total 
//                                FROM [dbo].[Flowchart_Detail_IE] AS A
//                                inner join [dbo].[Flowchart_Detail_ME] AS B on B.[Flowchart_Detail_ME_UID] = A.[Flowchart_Detail_ME_UID]
//                                inner join dbo.System_Function_Plant AS C on C.[System_FunPlant_UID] = B.[System_FunPlant_UID]
//                                inner join two_Q as D on D.[FlowChart_Master_UID] = B.[FlowChart_Master_UID] 
//                                GROUP BY C.FunPlant,D.Product_Phase
//                                )
//                                SELECT * FROM three_Q";
//                    sql = string.Format(sql, vm.OpTypeUID);
//                }
//            }
//            using (SqlConnection con = new SqlConnection(conStr))
//            {
//                con.Open();
//                SqlCommand cmd = null;
//                cmd = new SqlCommand(sql, con);
//                using (SqlDataReader sdr = cmd.ExecuteReader())
//                {
//                    dt.Load(sdr);
//                    sdr.Close();
//                }
//            }
//            return dt;
//        }
//        #endregion
//    }



}
