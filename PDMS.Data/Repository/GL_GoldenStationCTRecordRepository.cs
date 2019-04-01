using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_GoldenStationCTRecordRepository : IRepository<GL_GoldenStationCTRecord>
    {
        List<GoldenLineCTRecordDTO> GetGoldenLineCTRecordDTO(int stationID, int shiftTimeID, string cycleTimeDate);
        string InsertANDUpdateGoldenLineCTRecord(List<GoldenLineCTRecordDTO> GoldenLineCTRecordDTOs);
        List<GL_StationDTO> GetStationDTO();
        List<GL_ShiftTimeDTO> GetShiftTimeDTO(int Plant_Organization_UID, int BG_Organization_UID);

        List<GL_LineDTO> GetLineDTO();
        List<StationStdActCTInfoDTO> GetStationStdActCT(int customerId, int lineId, string outputDate, int shiftTimeID);
        List<SystemProjectDTO> GetCustomerDTOs(int BG_Organization_UID);
        List<GL_StationDTO> GetStationDTOs(int LineId);
        List<GL_StationDTO> GetONOMESStationDTOs(int LineId);
        
        List<GL_StationDTO> GetStationDTOsByLineID(int LineID);
        List<GL_LineDTO> GetLineDTOs(int CustomerID);

        List<GL_LineDTO> GetReportLineDTOs(int CustomerID);
        List<GL_LineDTO> GetAllLineDTOs(int CustomerID);
        List<GL_LineDTO> GetOEELineDTO(int CustomerID);
        List<GL_LineDTO> GetIPQCLineDTO(int CustomerID);
        List<WipGroupLineItem> GetGroupLineDTOs(int CustomerID);
        LatestLineStationInfoDTO GetLatestLineStationInfo(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID);
        List<LineShiftPlanActInfoDTO> GetLineShiftPlanActInfo(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID);
        ActualAndPlanDTO GetActualAndPlanDTOs(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID);
        GL_ShiftTimeDTO GetShiftTimeDTO(int ShiftTimeID);
        GL_LineDTO GetOneLineDTO(int LineId);
        string ImportBuildPlans(List<GLBuildPlanDTO> GLBuildPlanDTOs);
        List<GLBuildPlanDTO> GetBuildPlans(int LineId, DateTime StartDate, DateTime EndDate);
        string ImportBuildHCPlans(List<GLBuildPlanDTO> GLBuildPlanDTOs);
        //   PagedListModel<GLBuildPlanVM> QueryPlanData(int LineID, string date);

        string ImportBuildHCActua(List<GLBuildPlanDTO> GLBuildPlanDTOs);

        string SyncGoldenLineWeekPlan(List<GLBuildPlanDTO> GLBuildPlanDTOs);

        List<GLBuildPlanDTO> GetWeekPlan(List<string> thisDates);
        string InserOrUpdateStations(List<GL_StationDTO> listStations);
        List<GL_StationDTO> GetStationDTO(int CustomerID);
    }
    public class GL_GoldenStationCTRecordRepository : RepositoryBase<GL_GoldenStationCTRecord>, IGL_GoldenStationCTRecordRepository
    {
        public GL_GoldenStationCTRecordRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }


        public List<GoldenLineCTRecordDTO> GetGoldenLineCTRecordDTO(int stationID, int shiftTimeID, string cycleTimeDate)
        {

            var query = from c in DataContext.GL_GoldenStationCTRecord
                        select new GoldenLineCTRecordDTO
                        {
                            GoldenLineCTRecord_UID = c.GoldenLineCTRecord_UID,
                            StationID = c.StationID,
                            ShiftTimeID = c.ShiftTimeID,
                            CycleTime = c.CycleTime,
                            CycleTimeDate = c.CycleTimeDate,
                            StartTime = c.StartTime,
                            EndTime = c.EndTime,
                            UpdateTime = c.UpdateTime
                        };


            if (stationID != 0)
            {
                query = query.Where(o => o.StationID == stationID);
            }
            if (shiftTimeID != 0)
            {
                query = query.Where(o => o.ShiftTimeID == shiftTimeID);
            }
            if (cycleTimeDate != "")
            {
                query = query.Where(o => o.CycleTimeDate.Contains(cycleTimeDate));
            }
            return query.ToList();
        }
        public string InsertANDUpdateGoldenLineCTRecord(List<GoldenLineCTRecordDTO> GoldenLineCTRecordDTOs)
        {

            try
            {

                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();

                    var query = from c in DataContext.GL_GoldenStationCTRecord
                                where c.GL_Station.IsGoldenLine==true&&c.GL_Station.IsEnabled==true
                                select new GoldenLineCTRecordDTO
                                {
                                    GoldenLineCTRecord_UID = c.GoldenLineCTRecord_UID,
                                    StationID = c.StationID,
                                    ShiftTimeID = c.ShiftTimeID,
                                    CycleTime = c.CycleTime,
                                    CycleTimeDate = c.CycleTimeDate,
                                    StartTime = c.StartTime,
                                    EndTime = c.EndTime,
                                    UpdateTime = c.UpdateTime
                                };
                    //var CycleTimeDate = GoldenLineCTRecordDTOs.FirstOrDefault().CycleTimeDate;
                    //query = query.Where(o => o.CycleTimeDate == CycleTimeDate);
                    var CTRecordDTOs = query.ToList();

                    if (CTRecordDTOs != null)
                    {
                        foreach (var item in GoldenLineCTRecordDTOs)
                        {

                            var GoldenLineCTRecordDTO = CTRecordDTOs.FirstOrDefault(o => o.StationID == item.StationID && o.ShiftTimeID == item.ShiftTimeID && o.CycleTimeDate == item.CycleTimeDate);
                            if (GoldenLineCTRecordDTO == null)
                            {
                                //构造机台良率插入SQL数据
                                var insertSql = string.Format(@"INSERT INTO GL_GoldenStationCTRecord
                                                       (StationID
                                                       ,ShiftTimeID
                                                       ,CycleTime
                                                       ,CycleTimeDate
                                                       ,StartTime
                                                       ,EndTime
                                                       ,UpdateTime)
                                                       VALUES
                                                       ({0}
                                                       ,{1}
                                                       ,{2}
                                                       ,'{3}'
                                                       ,'{4}'
                                                       ,'{5}'
                                                       ,'{6}');", item.StationID, item.ShiftTimeID, item.CycleTime, item.CycleTimeDate, item.StartTime, item.EndTime, item.UpdateTime);

                                sb.AppendLine(insertSql);
                            }
                            else
                            {

                                var updateSql = string.Format(@"  
                                                  UPDATE GL_GoldenStationCTRecord
                                                  SET 
                                                  CycleTime = {0}       
                                                  ,EndTime= '{1}'
                                                  ,UpdateTime ='{2}'
                                                  WHERE  GoldenLineCTRecord_UID={3} ;", item.CycleTime, item.EndTime, item.UpdateTime, GoldenLineCTRecordDTO.GoldenLineCTRecord_UID);

                                sb.AppendLine(updateSql);

                            }

                        }
                    }
                    else
                    {
                        foreach (var item in GoldenLineCTRecordDTOs)
                        {


                            //构造机台良率插入SQL数据
                            var insertSql = string.Format(@"INSERT INTO GL_GoldenStationCTRecord
                                                       (StationID
                                                       ,ShiftTimeID
                                                       ,CycleTime
                                                       ,CycleTimeDate
                                                       ,StartTime
                                                       ,EndTime
                                                       ,UpdateTime)
                                                       VALUES
                                                       ({0}
                                                       ,{1}
                                                       ,{2}
                                                       ,'{3}'
                                                       ,'{4}'
                                                       ,'{5}'
                                                       ,'{6}');", item.StationID, item.ShiftTimeID, item.CycleTime, item.CycleTimeDate, item.StartTime, item.EndTime, item.UpdateTime);

                            sb.AppendLine(insertSql);
                        }

                    }

                    string sql = sb.ToString();
                    DataContext.Database.ExecuteSqlCommand(sb.ToString());
                    trans.Commit();
                }

                return "SUCCESS";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
        public List<GL_StationDTO> GetStationDTO()
        {
            var query = from w in DataContext.GL_Station
                        select new GL_StationDTO
                        {

                            StationID = w.StationID,
                            StationName = w.StationName,
                            LineID = w.LineID,
                            IsBirth = w.IsBirth,
                            IsOutput = w.IsOutput,
                            IsTest = w.IsTest,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            CustomerID = w.GL_Line.CustomerID,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            LineName = w.GL_Line.LineName,
                            LineIsEnabled = w.GL_Line.IsEnabled,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            LineCycleTime = w.GL_Line.CycleTime,
                            CycleTime = w.CycleTime,
                            MESStationName = w.MESStationName,
                            MESLineName = w.GL_Line.MESLineName,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name,
                            Binding_Seq = w.Binding_Seq,
                            IsGoldenLine = w.IsGoldenLine,
                            IsOEE = w.IsOEE,
                            IsOne = w.IsOne,
                            IsTwo = w.IsTwo,
                            IsThree = w.IsThree,
                            IsFour = w.IsFour,
                            IsFive = w.IsFive
                        };
            // query = query.Where(o => o.IsGoldenLine == true);
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.LineIsEnabled == true);
            return query.ToList();

        }
        public List<GL_ShiftTimeDTO> GetShiftTimeDTO(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var query = from w in DataContext.GL_ShiftTime
                        select new GL_ShiftTimeDTO
                        {
                            ShiftTimeID = w.ShiftTimeID,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            Shift = w.Shift,
                            StartTime = w.StartTime,
                            End_Time = w.End_Time,
                            IsEnabled = w.IsEnabled,
                            Break_Time=w.Break_Time,
                            Sequence=w.Sequence
                        };

            if (Plant_Organization_UID > 0)
            {
                query = query.Where(w => w.Plant_Organization_UID == Plant_Organization_UID);
            }
            if (BG_Organization_UID > 0)
            {
                query = query.Where(w => w.BG_Organization_UID == BG_Organization_UID);
            }
            query = query.Where(o => o.IsEnabled == true);
            return query.ToList();
        }
        public List<GL_LineDTO> GetLineDTO()
        {

            var query = from w in DataContext.GL_Line
                        where w.IsEnabled == true
                        select new GL_LineDTO
                        {

                            LineID = w.LineID,
                            LineName = w.LineName,
                            CustomerID = w.CustomerID,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            Modified_UID = w.Modified_UID,
                            Modified_Date = w.Modified_Date,
                            CycleTime = w.CycleTime,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID
                        };

            query = query.Where(o => o.IsEnabled == true);
            return query.ToList();

        }
        public GL_LineDTO GetOneLineDTO(int LineId)
        {
            var query = from w in DataContext.GL_Line
                        select new GL_LineDTO
                        {

                            LineID = w.LineID,
                            LineName = w.LineName,
                            CustomerID = w.CustomerID,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            Modified_UID = w.Modified_UID,
                            Modified_Date = w.Modified_Date,
                            CycleTime = w.CycleTime,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            Plant_Organization = w.System_Organization.Organization_Name,
                            BG_Organization = w.System_Organization1.Organization_Name,
                            ProjectName = w.System_Project.Project_Name,
                            MESProjectName = w.System_Project.MESProject_Name
                        };

            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.LineID == LineId);
            return query.FirstOrDefault();
        }
        public List<StationStdActCTInfoDTO> GetStationStdActCT(int customerId, int lineId, string outputDate, int shiftTimeID)
        {

            var query = from c in DataContext.GL_GoldenStationCTRecord
                        where c.GL_Station.IsGoldenLine == true && c.GL_Station.IsOne != true
                        select new StationStdActCTInfoDTO
                        {
                            StationName = c.GL_Station.StationName,
                            StdRouteCT = c.CycleTime,
                            CycleTime = c.GL_Station.CycleTime,
                            IsEnabled = c.GL_Station.IsEnabled,
                            LineIsEnabled = c.GL_Station.GL_Line.IsEnabled,
                            CustomerID = c.GL_Station.GL_Line.CustomerID,
                            LineID = c.GL_Station.LineID,
                            StationID = c.StationID,
                            ShiftTimeID = c.ShiftTimeID,
                            OutputDate = c.CycleTimeDate.ToString()
                        };

            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.LineIsEnabled == true);
            //  query = query.Where(o => o.CustomerIsEnabled == true);
            query = query.Where(o => o.CustomerID == customerId);
            query = query.Where(o => o.LineID == lineId);
            query = query.Where(o => o.ShiftTimeID == shiftTimeID);
            query = query.Where(o => o.OutputDate.Contains(outputDate));

            return query.ToList();

        }
        public List<SystemProjectDTO> GetCustomerDTOs(int BG_Organization_UID)
        {
            var query = from w in DataContext.System_Project
                        select new SystemProjectDTO
                        {
                            Project_UID = w.Project_UID,
                            Project_Code = w.Project_Code,
                            BU_D_UID = w.BU_D_UID,
                            Project_Name = w.Project_Name,
                            MESProject_Name = w.MESProject_Name,
                            Product_Phase = w.Product_Phase,
                            Start_Date = w.Start_Date,
                            Closed_Date = w.Closed_Date,
                            OP_TYPES = w.OP_TYPES,
                            Organization_UID = w.Organization_UID,
                            Project_Type = w.Project_Type,
                            Modified_UID = w.Modified_UID,
                            Modified_Date = w.Modified_Date
                        };
            if (BG_Organization_UID != 0)
            {
                query = query.Where(o => o.Organization_UID == BG_Organization_UID);

            }
            query = query.Where(o => o.MESProject_Name != "" && o.MESProject_Name != null);
            return query.ToList();
        }
        public List<GL_StationDTO> GetStationDTOs(int LineID)
        {
            var query = from w in DataContext.GL_Station
                        select new GL_StationDTO
                        {

                            StationID = w.StationID,
                            StationName = w.StationName,
                            LineID = w.LineID,
                            IsBirth = w.IsBirth,
                            IsOutput = w.IsOutput,
                            IsTest = w.IsTest,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            CustomerID = w.GL_Line.CustomerID,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            LineName = w.GL_Line.LineName,
                            LineIsEnabled = w.GL_Line.IsEnabled,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            LineCycleTime = w.GL_Line.CycleTime,
                            CycleTime = w.CycleTime,
                            MESStationName = w.MESStationName,
                            MESLineName = w.GL_Line.MESLineName,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name,
                            Binding_Seq = w.Binding_Seq,
                            IsGoldenLine = w.IsGoldenLine,
                            IsOEE = w.IsOEE,
                            IsOne = w.IsOne,
                            IsTwo = w.IsTwo,
                            IsThree = w.IsThree,
                            IsFour = w.IsFour,
                            IsFive = w.IsFive,
                            DashboardTarget = w.DashboardTarget
                        };

            query = query.Where(o => o.IsGoldenLine == true);
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.LineIsEnabled == true);
            query = query.Where(o => o.LineID == LineID);
            return query.ToList();

        }
        public List<GL_StationDTO> GetONOMESStationDTOs(int LineID)
        {
            var query = from w in DataContext.GL_Station
                        select new GL_StationDTO
                        {

                            StationID = w.StationID,
                            StationName = w.StationName,
                            LineID = w.LineID,
                            IsBirth = w.IsBirth,
                            IsOutput = w.IsOutput,
                            IsTest = w.IsTest,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            CustomerID = w.GL_Line.CustomerID,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            LineName = w.GL_Line.LineName,
                            LineIsEnabled = w.GL_Line.IsEnabled,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            LineCycleTime = w.GL_Line.CycleTime,
                            CycleTime = w.CycleTime,
                            MESStationName = w.MESStationName,
                            MESLineName = w.GL_Line.MESLineName,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name,
                            Binding_Seq = w.Binding_Seq,
                            IsGoldenLine = w.IsGoldenLine,
                            IsOEE = w.IsOEE,
                            IsOne = w.IsOne,
                            IsTwo = w.IsTwo,
                            IsThree = w.IsThree,
                            IsFour = w.IsFour,
                            IsFive = w.IsFive,
                            DashboardTarget = w.DashboardTarget
                        };

            query = query.Where(o => o.IsGoldenLine == true);
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.LineIsEnabled == true);
            query = query.Where(o => o.IsOne != true);
            query = query.Where(o => o.LineID == LineID);
            return query.ToList();

        }
        

        public List<GL_StationDTO> GetStationDTOsByLineID(int LineID)
        {
            var query = from w in DataContext.GL_Station
                        select new GL_StationDTO
                        {

                            StationID = w.StationID,
                            StationName = w.StationName,
                            LineID = w.LineID,
                            IsBirth = w.IsBirth,
                            IsOutput = w.IsOutput,
                            IsTest = w.IsTest,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            CustomerID = w.GL_Line.CustomerID,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            LineName = w.GL_Line.LineName,
                            LineIsEnabled = w.GL_Line.IsEnabled,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            LineCycleTime = w.GL_Line.CycleTime,
                            CycleTime = w.CycleTime,
                            MESStationName = w.MESStationName,
                            MESLineName = w.GL_Line.MESLineName,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name,
                            Binding_Seq = w.Binding_Seq,
                            IsGoldenLine = w.IsGoldenLine,
                            IsOEE = w.IsOEE,
                            IsOne = w.IsOne,
                            IsTwo = w.IsTwo,
                            IsThree = w.IsThree,
                            IsFour = w.IsFour,
                            IsFive = w.IsFive,
                            DashboardTarget=w.DashboardTarget
                        };

            //query = query.Where(o => o.IsGoldenLine == true);
            query = query.Where(o => o.LineID == LineID);
            return query.ToList();

        }
        public List<GL_LineDTO> GetLineDTOs(int CustomerID)
        {


            //var query = from w in DataContext.GL_Line
            //            select new GL_LineDTO
            //            {
            //                LineID = w.LineID,
            //                LineName = w.LineName,
            //                CustomerID = w.CustomerID,
            //                Seq = w.Seq,
            //                IsEnabled = w.IsEnabled,
            //                Modified_UID = w.Modified_UID,
            //                Modified_Date = w.Modified_Date,
            //                CycleTime = w.CycleTime,
            //                Plant_Organization_UID = w.Plant_Organization_UID,
            //                BG_Organization_UID = w.BG_Organization_UID,
            //                FunPlant_Organization_UID = w.FunPlant_Organization_UID,
            //                Plant_Organization = w.System_Organization.Organization_Name,
            //                BG_Organization = w.System_Organization1.Organization_Name,
            //                ProjectName = w.System_Project.Project_Name,
            //                MESProjectName = w.System_Project.MESProject_Name
            //            };

            //query = query.Where(o => o.IsEnabled == true);
            //query = query.Where(o => o.CustomerID == CustomerID);
            //return query.ToList();
            var query = from w in DataContext.GL_Station
                        where w.IsEnabled == true && w.IsGoldenLine == true
                        select new GL_LineDTO
                        {
                            LineID = w.LineID,
                            LineName = w.GL_Line.LineName,
                            CustomerID = w.GL_Line.CustomerID,
                            Seq = w.GL_Line.Seq,
                            IsEnabled = w.GL_Line.IsEnabled,
                            Modified_UID = w.GL_Line.Modified_UID,
                            Modified_Date = w.GL_Line.Modified_Date,
                            CycleTime = w.GL_Line.CycleTime,
                            Plant_Organization_UID = w.GL_Line.Plant_Organization_UID,
                            BG_Organization_UID = w.GL_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = w.GL_Line.FunPlant_Organization_UID,
                            Plant_Organization = w.GL_Line.System_Organization.Organization_Name,
                            BG_Organization = w.GL_Line.System_Organization1.Organization_Name,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name
                        };
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.CustomerID == CustomerID);
            return query.Distinct().ToList();
        }

        public List<GL_LineDTO> GetOEELineDTO(int CustomerID) {

            var query = from w in DataContext.GL_Station
                        where w.IsEnabled == true && w.IsOEE == true
                        select new GL_LineDTO
                        {
                            LineID = w.LineID,
                            LineName = w.GL_Line.LineName,
                            CustomerID = w.GL_Line.CustomerID,
                            Seq = w.GL_Line.Seq,
                            IsEnabled = w.GL_Line.IsEnabled,
                            Modified_UID = w.GL_Line.Modified_UID,
                            Modified_Date = w.GL_Line.Modified_Date,
                            CycleTime = w.GL_Line.CycleTime,
                            Plant_Organization_UID = w.GL_Line.Plant_Organization_UID,
                            BG_Organization_UID = w.GL_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = w.GL_Line.FunPlant_Organization_UID,
                            Plant_Organization = w.GL_Line.System_Organization.Organization_Name,
                            BG_Organization = w.GL_Line.System_Organization1.Organization_Name,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name
                        };
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.CustomerID == CustomerID);
            return query.Distinct().ToList();



        }


        
        /// <summary>
        /// IPQC 的Line
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        public List<GL_LineDTO> GetIPQCLineDTO(int CustomerID)
        {
            var query = from w in DataContext.GL_Station
                        where w.IsEnabled == true && w.IsTest == true
                        select new GL_LineDTO
                        {
                            LineID = w.LineID,
                            LineName = w.GL_Line.LineName,
                            CustomerID = w.GL_Line.CustomerID,
                            Seq = w.GL_Line.Seq,
                            IsEnabled = w.GL_Line.IsEnabled,
                            Modified_UID = w.GL_Line.Modified_UID,
                            Modified_Date = w.GL_Line.Modified_Date,
                            CycleTime = w.GL_Line.CycleTime,
                            Plant_Organization_UID = w.GL_Line.Plant_Organization_UID,
                            BG_Organization_UID = w.GL_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = w.GL_Line.FunPlant_Organization_UID,
                            Plant_Organization = w.GL_Line.System_Organization.Organization_Name,
                            BG_Organization = w.GL_Line.System_Organization1.Organization_Name,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name
                        };
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.CustomerID == CustomerID);
            return query.Distinct().ToList();
        }

        public List<GL_LineDTO> GetReportLineDTOs(int CustomerID)
        {

            var query = from w in DataContext.GL_Station
                        where w.IsEnabled == true && w.IsGoldenLine == true && w.IsOne != true
                        select new GL_LineDTO
                        {
                            LineID = w.LineID,
                            LineName = w.GL_Line.LineName,
                            CustomerID = w.GL_Line.CustomerID,
                            Seq = w.GL_Line.Seq,
                            IsEnabled = w.GL_Line.IsEnabled,
                            Modified_UID = w.GL_Line.Modified_UID,
                            Modified_Date = w.GL_Line.Modified_Date,
                            CycleTime = w.GL_Line.CycleTime,
                            Plant_Organization_UID = w.GL_Line.Plant_Organization_UID,
                            BG_Organization_UID = w.GL_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = w.GL_Line.FunPlant_Organization_UID,
                            Plant_Organization = w.GL_Line.System_Organization.Organization_Name,
                            BG_Organization = w.GL_Line.System_Organization1.Organization_Name,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name
                        };
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.CustomerID == CustomerID);
            return query.Distinct().ToList();

        }

        public List<GL_LineDTO> GetAllLineDTOs(int CustomerID)
        {

            var query = from w in DataContext.GL_Station
                        where w.IsEnabled == true && w.IsGoldenLine == true
                        select new GL_LineDTO
                        {
                            LineID = w.LineID,
                            LineName = w.GL_Line.LineName,
                            CustomerID = w.GL_Line.CustomerID,
                            Seq = w.GL_Line.Seq,
                            IsEnabled = w.GL_Line.IsEnabled,
                            Modified_UID = w.GL_Line.Modified_UID,
                            Modified_Date = w.GL_Line.Modified_Date,
                            CycleTime = w.GL_Line.CycleTime,
                            Plant_Organization_UID = w.GL_Line.Plant_Organization_UID,
                            BG_Organization_UID = w.GL_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = w.GL_Line.FunPlant_Organization_UID,
                            Plant_Organization = w.GL_Line.System_Organization.Organization_Name,
                            BG_Organization = w.GL_Line.System_Organization1.Organization_Name,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name
                        };
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.CustomerID == CustomerID);
            return query.Distinct().ToList();

        }

        public List<WipGroupLineItem> GetGroupLineDTOs(int CustomerID)
        {

            var query = from w in DataContext.GL_Line
                        join s1 in DataContext.GL_LineGroup on w.LineID equals s1.LineID into t1
                        from s1 in t1.DefaultIfEmpty()
                        join s2 in DataContext.GL_LineGroup on s1.LineParent_ID equals s2.LineGroup_UID into t2
                        from s2 in t2.DefaultIfEmpty()
                        where w.IsEnabled == true && w.CustomerID== CustomerID
                        orderby s1.LineParent_ID, w.LineID
                        select new WipGroupLineItem
                        {
                            LineID = w.LineID,
                            LineName = w.LineName,
                            CustomerID = w.CustomerID,
                            LineParent_ID = s1.LineParent_ID,
                            LineGroup_UID = s1.LineGroup_UID,
                            GroupLineName = s2.LineName
                        };
            
            return query.ToList();

        }

        public LatestLineStationInfoDTO GetLatestLineStationInfo(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID)
        {

            var query = from w in DataContext.GL_Station
                        where w.IsGoldenLine == true
                        select new LatestLineStationInfoDTO
                        {

                            StationID = w.StationID,
                            StationName = w.StationName,
                            MESStationName = w.MESStationName,
                            LineID = w.LineID,
                            IsBirth = w.IsBirth,
                            IsOutput = w.IsOutput,
                            IsTest = w.IsTest,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            CustomerID = w.GL_Line.CustomerID,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name,
                            LineName = w.GL_Line.LineName,
                            MESLineName = w.GL_Line.MESLineName,
                            LineIsEnabled = w.GL_Line.IsEnabled,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            LineCycleTime = w.GL_Line.CycleTime,
                            CycleTime = w.CycleTime
                            //Planqty=w.GL_Line.GL_BuildPlan.
                            //UpdateTime=w.GL_GoldenStationCTRecord
                            //OutputDate =
                            //ShiftTimeID =
                        };

            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.LineIsEnabled == true);
            //query = query.Where(o => o.CustomerIsEnabled == true);
            if (customerId != 0)
            {
                query = query.Where(o => o.CustomerID == customerId);
            }
            if (lineId != 0)
            {
                query = query.Where(o => o.LineID == lineId);
            }
            if (stationId != 0)
            {
                query = query.Where(o => o.StationID == stationId);
            }


            return SetPlanqty(query.FirstOrDefault(), customerId, lineId, outputDate, shiftTimeID);

        }
        public LatestLineStationInfoDTO SetPlanqty(LatestLineStationInfoDTO LatestLineStationInfos, int customerId, int lineId, string outputDate, int shiftTimeID)
        {
            if (LatestLineStationInfos != null)
            {
                var GL_BuildPlans = DataContext.GL_BuildPlan.ToList();

                int planqty = 0;
                if (GL_BuildPlans != null)
                {
                    var BuildPlan = GL_BuildPlans.FirstOrDefault(o => o.CustomerID == customerId && o.LineID == lineId && o.OutputDate == outputDate && o.ShiftTimeID == shiftTimeID && o.GL_Line.IsEnabled == true);
                    if (BuildPlan != null)
                    {
                        planqty = BuildPlan.PlanOutput;
                    }

                }
                LatestLineStationInfos.Planqty = planqty;
            }
            return LatestLineStationInfos;
        }
        /// <summary>
        /// 获取第二个报表 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="lineId"></param>
        /// <param name="stationId"></param>
        /// <param name="outputDate"></param>
        /// <param name="shiftTimeID"></param>
        /// <returns></returns>
        public List<LineShiftPlanActInfoDTO> GetLineShiftPlanActInfo(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID)
        {
            var query = from w in DataContext.GL_WIPHourOutput
                        where w.GL_Station.IsGoldenLine == true && w.GL_Line.IsEnabled == true && w.GL_Station.IsEnabled == true
                        select new LineShiftPlanActInfoDTO
                        {

                            StationID = w.StationID,
                            StationName = w.GL_Station.StationName,
                            IsEnabled = w.GL_Station.IsEnabled,
                            LineID = w.LineID,
                            LineName = w.GL_Line.LineName,
                            LineIsEnabled = w.GL_Line.IsEnabled,
                            CustomerID = w.CustomerID,
                            ProjectName = w.System_Project.Project_Name,
                            MESProjectName = w.System_Project.MESProject_Name,
                            Plant_Organization_UID = w.GL_Station.Plant_Organization_UID,
                            BG_Organization_UID = w.GL_Station.BG_Organization_UID,
                            FunPlant_Organization_UID = w.GL_Station.FunPlant_Organization_UID,
                            ActualOutput = w.ActualOutput,
                            OutputDate = w.OutputDate,
                            StrOutputDate = w.ShiftDate,
                            ShiftTimeID = w.ShiftTimeID,
                            HourIndex = w.HourIndex,
                            StartDate = w.GL_ShiftTime.StartTime,
                            EndDate = w.GL_ShiftTime.End_Time
                            // HourQuantum = w.HourQuantum,
                        };

            if (customerId != 0)
            {
                query = query.Where(o => o.CustomerID == customerId);
            }
            if (lineId != 0)
            {
                query = query.Where(o => o.LineID == lineId);
            }
            if (stationId != 0)
            {
                query = query.Where(o => o.StationID == stationId);
            }
            if (shiftTimeID != 0)
            {
                query = query.Where(o => o.ShiftTimeID == shiftTimeID);
            }
            if (outputDate != "")
            {
                query = query.Where(o => o.StrOutputDate.Contains(outputDate));
            }
            return SetHourQuantum(query.ToList(), stationId);

        }
        public ActualAndPlanDTO GetActualAndPlanDTOs(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID)
        {

            ActualAndPlanDTO actualAndPlanDTO = new ActualAndPlanDTO();
            //计划标准CT
            decimal SumTime = 0;
            decimal allSumTime = 0;
            int PlanOutPut = 0;
            decimal PlanLB = 0;
            decimal PlanSMH = 0;
            int PlanHC = 0;
            decimal PlanUPPH = 0;
            decimal PlanVAOLE = 0;

            // int ActualOutPut = 0;
            decimal ActualLB = 0;
            decimal ActualSMH = 0;
            int ActualHC = 0;
            decimal ActualUPPH = 0;
            decimal ActualVAOLE = 0;

            string StationName = "";
            string LineName = "";
            string CustomerName = "";
            DateTime StartdDateTime = Convert.ToDateTime(outputDate);
            DateTime EndDateTime = Convert.ToDateTime(outputDate);
            //这里要获取人力 得到PlanHC和ActualHC
            var BuildPlanDTO = GetGL_BuildPlanDTO(customerId, lineId, outputDate, shiftTimeID);
            if (BuildPlanDTO != null)
            {
                PlanHC = BuildPlanDTO.PlanHC == null ? 0 : BuildPlanDTO.PlanHC.Value;
                ActualHC = BuildPlanDTO.ActualHC == null ? 0 : BuildPlanDTO.ActualHC.Value;
            }
            //根据班次ID获取班次
            var ShiftTimeDTO = GetShiftTimeDTO(shiftTimeID);
            if (ShiftTimeDTO != null)
            {
                StartdDateTime = Convert.ToDateTime((outputDate + " " + ShiftTimeDTO.StartTime));
                EndDateTime = Convert.ToDateTime((outputDate + " " + ShiftTimeDTO.End_Time));
                if (StartdDateTime >= EndDateTime)
                {
                    EndDateTime = EndDateTime.AddDays(1);
                }
                var timenum = EndDateTime - StartdDateTime;
                //得到整个班次的总秒数
                SumTime = Convert.ToInt32(timenum.TotalSeconds);

                if (outputDate == DateTime.Now.ToString("yyyy-MM-dd") && DateTime.Now >= StartdDateTime && DateTime.Now <= EndDateTime)
                {
                    allSumTime = Convert.ToInt32((DateTime.Now - StartdDateTime).TotalSeconds);
                }
                else
                {
                    allSumTime = SumTime;
                }
                //获取计划的标准CT
                var PlanStationCT = DataContext.GL_Station.FirstOrDefault(o => o.LineID == lineId && o.StationID == stationId && o.IsEnabled == true && o.IsGoldenLine == true && o.GL_Line.IsEnabled == true);
                decimal PlanCT = 0;
                if (PlanStationCT != null)
                {
                    PlanCT = PlanStationCT.CycleTime;
                    //StationName = PlanStationCT.StationName;
                    //LineName = PlanStationCT.GL_Line.LineName;
                    //CustomerName = PlanStationCT.GL_Line.GL_Customer.CustomerName;

                }
                if (PlanCT != 0)
                {
                    PlanOutPut = Convert.ToInt32((SumTime - 7200) / PlanCT);
                }
                var stationDTOs = GetStationDTOs(lineId);
                decimal sumStationCT = 0;
                decimal maxStation = 0;
                int countStation = 0;
                if (stationDTOs != null)
                {
                    sumStationCT = stationDTOs.Sum(o => o.CycleTime);
                    maxStation = stationDTOs.Max(o => o.CycleTime);
                    countStation = stationDTOs.Count;

                }
                if (maxStation != 0 && countStation != 0)
                {
                    PlanLB = sumStationCT / (maxStation * countStation);
                }
                // ??
                PlanSMH = sumStationCT / 3600;
                if (PlanCT != 0 && PlanHC != 0)
                {
                    PlanUPPH = 3600 / (PlanCT * PlanHC);
                }
                //获取实际站点的CT
                var ActualStationCT = DataContext.GL_GoldenStationCTRecord.FirstOrDefault(o => o.StationID == stationId && o.ShiftTimeID == shiftTimeID && o.CycleTimeDate == outputDate && o.GL_Station.IsEnabled == true);
                decimal ActualCT = 0;
                if (ActualStationCT != null)
                {
                    ActualCT = ActualStationCT.CycleTime;
                }
                //if (ActualOutPut != 0)
                //{
                //    ActualOutPut = Convert.ToInt32(SumTime / ActualCT);  (Output * actualAndPlanDTO.ActualSMH) / (actualAndPlanDTO.ActualHC * actualAndPlanDTO.SumTime);
                //}
                var ActualstationCTs = GetGoldenLineCTRecords(customerId, lineId, outputDate, shiftTimeID);
                decimal ActualsumStationCT = 0;
                decimal ActualmaxStation = 0;
                int ActualcountStation = 0;
                if (ActualstationCTs != null && ActualstationCTs.Count > 0)
                {
                    ActualsumStationCT = ActualstationCTs.Sum(o => o.CycleTime);
                    ActualmaxStation = ActualstationCTs.Max(o => o.CycleTime);
                    ActualcountStation = ActualstationCTs.Count;
                }
                if (ActualmaxStation != 0 && ActualcountStation != 0)
                {
                    ActualLB = ActualsumStationCT / (ActualmaxStation * ActualcountStation);
                }
                ActualSMH = ActualsumStationCT / 3600;
                if (ActualCT != 0 && ActualHC != 0)
                {
                    ActualUPPH = 3600 / (ActualCT * ActualHC);
                }

                if (actualAndPlanDTO.PlanHC != 0 && actualAndPlanDTO.SumTime != 0)
                {
                    PlanVAOLE = (actualAndPlanDTO.PlanOutPut * PlanSMH) / (actualAndPlanDTO.PlanHC * actualAndPlanDTO.SumTime);
                }
            }
            //设置基础数据
            actualAndPlanDTO.CustomerID = customerId;
            actualAndPlanDTO.LineID = lineId;
            actualAndPlanDTO.StationID = stationId;
            actualAndPlanDTO.OutputDate = outputDate;
            actualAndPlanDTO.ShiftTimeID = shiftTimeID;
            //设置计划的数据
            actualAndPlanDTO.PlanOutPut = PlanOutPut;
            actualAndPlanDTO.PlanLB = PlanLB;
            actualAndPlanDTO.PlanSMH = PlanSMH;


            actualAndPlanDTO.PlanHC = PlanHC;
            actualAndPlanDTO.PlanUPPH = PlanUPPH;
            actualAndPlanDTO.PlanVAOLE = PlanVAOLE;
            //设置实际数据 
            //double ActualVAOLE = 0.0;
            // actualAndPlanDTO.ActualOutPut = ActualOutPut;
            actualAndPlanDTO.ActualLB = ActualLB;
            actualAndPlanDTO.ActualSMH = ActualSMH;
            actualAndPlanDTO.ActualHC = ActualHC;
            actualAndPlanDTO.ActualUPPH = ActualUPPH;
            actualAndPlanDTO.SumTime = allSumTime;
            actualAndPlanDTO.ActualVAOLE = ActualVAOLE;

            var CustomerTable = DataContext.System_Project.FirstOrDefault(o => o.Project_UID == customerId);
            if (CustomerTable != null)
            {
                //actualAndPlanDTO.StationName = StationName;
                actualAndPlanDTO.CustomerName = CustomerTable.Project_Name;
                actualAndPlanDTO.MESCustomerName = CustomerTable.MESProject_Name;   //Jay 20180621
            }
            var LineTable = DataContext.GL_Line.FirstOrDefault(o => o.LineID == lineId && o.IsEnabled == true);
            if (LineTable != null)
            {
                // actualAndPlanDTO.LineName = LineName;
                actualAndPlanDTO.LineName = LineTable.LineName;
                actualAndPlanDTO.MESLineName = LineTable.MESLineName;
            }

            var StationTable = DataContext.GL_Station.FirstOrDefault(o => o.StationID == stationId && o.IsEnabled == true && o.IsGoldenLine == true);
            if (StationTable != null)
            {
                // actualAndPlanDTO.CustomerName = CustomerName;
                actualAndPlanDTO.StationName = StationTable.StationName;
                actualAndPlanDTO.MESStationName = StationTable.MESStationName;
                actualAndPlanDTO.Plant_Organization_UID = StationTable.Plant_Organization_UID;
            }

            actualAndPlanDTO.StartDateTime = StartdDateTime;
            actualAndPlanDTO.EndDateTime = EndDateTime;
            return actualAndPlanDTO;

        }
        public List<LineShiftPlanActInfoDTO> SetHourQuantum(List<LineShiftPlanActInfoDTO> lineShiftPlanActInfos, int stationId)
        {
            var stations = DataContext.GL_Station.FirstOrDefault(o => o.StationID == stationId && o.IsGoldenLine == true && o.IsEnabled == true);
            decimal stationSMH = 0;
            if (stations != null)
            {
                stationSMH = 3600 / stations.CycleTime;
            }
            foreach (var item in lineShiftPlanActInfos)
            {

                DateTime StartdDateTime = Convert.ToDateTime((item.StrOutputDate + " " + item.StartDate));
                item.HourQuantum = StartdDateTime.AddHours(item.HourIndex - 1).ToString("HH:mm") + "~" + StartdDateTime.AddHours(item.HourIndex).ToString("HH:mm");
                item.StationSMH = stationSMH;
            }

            return lineShiftPlanActInfos;
        }
        public GL_ShiftTimeDTO GetShiftTimeDTO(int ShiftTimeID)
        {
            var query = from w in DataContext.GL_ShiftTime
                        select new GL_ShiftTimeDTO
                        {
                            ShiftTimeID = w.ShiftTimeID,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            Shift = w.Shift,
                            StartTime = w.StartTime,
                            End_Time = w.End_Time,
                            IsEnabled = w.IsEnabled,
                            Break_Time=w.Break_Time,
                            Sequence=w.Sequence
                        };

            if (ShiftTimeID > 0)
            {
                query = query.Where(w => w.ShiftTimeID == ShiftTimeID);
            }

            return query.FirstOrDefault();
        }

        public GL_BuildPlanDTO GetGL_BuildPlanDTO(int customerId, int lineId, string outputDate, int shiftTimeID)
        {

            var query = from w in DataContext.GL_BuildPlan
                        where w.GL_Line.IsEnabled == true
                        select new GL_BuildPlanDTO
                        {
                            BuildPlanID = w.BuildPlanID,
                            CustomerID = w.CustomerID,
                            LineID = w.LineID,
                            StartTime = w.StartTime,
                            EndTime = w.EndTime,
                            PlanOutput = w.PlanOutput,
                            OutputDate = w.OutputDate,
                            ShiftTimeID = w.ShiftTimeID,
                            PlanHC = w.PlanHC.Value,
                            ActualHC = w.ActualHC.Value
                        };

            if (customerId > 0)
            {
                query = query.Where(w => w.CustomerID == customerId);
            }
            if (lineId > 0)
            {
                query = query.Where(w => w.LineID == lineId);
            }
            if (shiftTimeID > 0)
            {
                query = query.Where(w => w.ShiftTimeID == shiftTimeID);
            }
            if (!string.IsNullOrWhiteSpace(outputDate))
            {
                query = query.Where(w => w.OutputDate == outputDate);
            }

            return query.FirstOrDefault();

        }

        public List<GoldenLineCTRecordDTO> GetGoldenLineCTRecords(int customerId, int lineId, string outputDate, int shiftTimeID)
        {
            var query = from w in DataContext.GL_GoldenStationCTRecord
                        where w.GL_Station.IsGoldenLine == true && w.GL_Station.IsEnabled == true
                        select new GoldenLineCTRecordDTO
                        {
                            GoldenLineCTRecord_UID = w.GoldenLineCTRecord_UID,
                            StationID = w.StationID,
                            ShiftTimeID = w.ShiftTimeID,
                            CycleTime = w.CycleTime,
                            CycleTimeDate = w.CycleTimeDate,
                            StartTime = w.StartTime,
                            EndTime = w.EndTime,
                            UpdateTime = w.UpdateTime,
                            LineID = w.GL_Station.LineID,
                            CustomerID = w.GL_Station.GL_Line.CustomerID
                        };

            if (customerId > 0)
            {
                query = query.Where(w => w.CustomerID == customerId);
            }
            if (lineId > 0)
            {
                query = query.Where(w => w.LineID == lineId);
            }
            if (shiftTimeID > 0)
            {
                query = query.Where(w => w.ShiftTimeID == shiftTimeID);
            }
            if (!string.IsNullOrWhiteSpace(outputDate))
            {
                query = query.Where(w => w.CycleTimeDate.Contains(outputDate));
            }

            return query.ToList();
        }

        public string ImportBuildPlans(List<GLBuildPlanDTO> GLBuildPlanDTOs)
        {
            try
            {

                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    if (GLBuildPlanDTOs != null && GLBuildPlanDTOs.Count > 0)
                    {
                        var LineDTOs = GetLineDTO();
                        var BuildPlanDTOs = DataContext.GL_BuildPlan.ToList();
                        foreach (var item in GLBuildPlanDTOs)
                        {

                            var LineDTO = LineDTOs.FirstOrDefault(o => o.LineID == item.LineID);

                            var BuildPlanDTO = BuildPlanDTOs.FirstOrDefault(o => o.LineID == item.LineID && o.ShiftTimeID == item.ShiftTimeID && o.OutputDate == item.OutputDate && o.GL_Line.IsEnabled == true);

                            if (LineDTO != null)
                            {
                                if (BuildPlanDTO == null)
                                {
                                    //构造插入SQL数据
                                    var insertSql = string.Format(@"INSERT INTO GL_BuildPlan
                                                       (CustomerID
                                                       ,LineID          
                                                       ,PlanOutput
                                                       ,OutputDate
                                                       ,ShiftTimeID
                                                       ,Created_UID
                                                       ,Created_Date
                                                       ,Modified_UID
                                                       ,Modified_Date)
                                                      VALUES
                                                       ({0}
                                                       ,{1}
                                                       ,{2}
                                                       ,'{3}'
                                                       ,{4}
                                                       ,{5}
                                                       ,'{6}'
                                                       ,{7}
                                                       ,'{6}');", LineDTO.CustomerID, item.LineID, item.PlanOutput, item.OutputDate, item.ShiftTimeID, item.Created_UID, item.Created_Date, item.Modified_UID, item.Modified_Date);
                                    sb.AppendLine(insertSql);
                                }
                                else
                                {

                                    var updateSql = string.Format(@" UPDATE GL_BuildPlan SET
                                                                   PlanOutput = {0}   
                                                                  ,Modified_UID ={1}
                                                                  ,Modified_Date = '{2}'
                                                                   WHERE   BuildPlanID ={3};", item.PlanOutput, item.Modified_UID, item.Modified_Date, BuildPlanDTO.BuildPlanID);

                                    sb.AppendLine(updateSql);

                                }

                            }

                        }
                        string sql = sb.ToString();
                        DataContext.Database.ExecuteSqlCommand(sb.ToString());
                        trans.Commit();
                    }
                }

                return "SUCCESS";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string ImportBuildHCPlans(List<GLBuildPlanDTO> GLBuildPlanDTOs)
        {

            try
            {

                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    if (GLBuildPlanDTOs != null && GLBuildPlanDTOs.Count > 0)
                    {
                        var LineDTOs = GetLineDTO();
                        var BuildPlanDTOs = DataContext.GL_BuildPlan.ToList();
                        foreach (var item in GLBuildPlanDTOs)
                        {

                            var LineDTO = LineDTOs.FirstOrDefault(o => o.LineID == item.LineID);

                            var BuildPlanDTO = BuildPlanDTOs.FirstOrDefault(o => o.LineID == item.LineID && o.ShiftTimeID == item.ShiftTimeID && o.OutputDate == item.OutputDate && o.GL_Line.IsEnabled == true);

                            if (LineDTO != null)
                            {
                                if (BuildPlanDTO == null)
                                {
                                    //构造插入SQL数据
                                    var insertSql = string.Format(@"INSERT INTO GL_BuildPlan
                                                       (CustomerID
                                                       ,LineID                                                          
                                                       ,PlanHC
                                                       ,PlanOutput
                                                       ,OutputDate
                                                       ,ShiftTimeID
                                                       ,Created_UID
                                                       ,Created_Date
                                                       ,Modified_UID
                                                       ,Modified_Date)
                                                      VALUES
                                                       ({0}
                                                       ,{1}
                                                       ,{2}
                                                       ,{3}
                                                       ,'{4}'
                                                       ,{5}
                                                       ,{6}
                                                       ,'{7}'
                                                       ,{8}
                                                      ,'{7}');", LineDTO.CustomerID, item.LineID, item.PlanHC, 0, item.OutputDate, item.ShiftTimeID, item.Created_UID, item.Created_Date, item.Modified_UID, item.Modified_Date);
                                    sb.AppendLine(insertSql);
                                }
                                else
                                {

                                    var updateSql = string.Format(@" UPDATE GL_BuildPlan SET
                                                                   PlanHC = {0}   
                                                                  ,Modified_UID ={1}
                                                                  ,Modified_Date = '{2}'
                                                                   WHERE   BuildPlanID ={3};", item.PlanHC, item.Modified_UID, item.Modified_Date, BuildPlanDTO.BuildPlanID);

                                    sb.AppendLine(updateSql);

                                }

                            }

                        }
                        string sql = sb.ToString();
                        DataContext.Database.ExecuteSqlCommand(sb.ToString());
                        trans.Commit();
                    }
                }

                return "SUCCESS";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
        public string ImportBuildHCActua(List<GLBuildPlanDTO> GLBuildPlanDTOs)
        {
            try
            {

                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    if (GLBuildPlanDTOs != null && GLBuildPlanDTOs.Count > 0)
                    {
                        var LineDTOs = GetLineDTO();
                        var BuildPlanDTOs = DataContext.GL_BuildPlan.ToList();
                        foreach (var item in GLBuildPlanDTOs)
                        {

                            var LineDTO = LineDTOs.FirstOrDefault(o => o.LineID == item.LineID);

                            var BuildPlanDTO = BuildPlanDTOs.FirstOrDefault(o => o.LineID == item.LineID && o.ShiftTimeID == item.ShiftTimeID && o.OutputDate == item.OutputDate && o.GL_Line.IsEnabled == true);

                            if (LineDTO != null)
                            {
                                if (BuildPlanDTO == null)
                                {
                                    //构造插入SQL数据
                                    var insertSql = string.Format(@"INSERT INTO GL_BuildPlan
                                                       (CustomerID
                                                       ,LineID          
                                                       ,ActualHC
                                                       ,PlanOutput
                                                       ,OutputDate
                                                       ,ShiftTimeID
                                                       ,Created_UID
                                                       ,Created_Date
                                                       ,Modified_UID
                                                       ,Modified_Date)
                                                       VALUES
                                                       ({0}
                                                       ,{1}
                                                       ,{2}
                                                       ,{3}
                                                       ,'{4}'
                                                       ,{5}
                                                       ,{6}
                                                       ,'{7}'
                                                       ,{8}
                                                      ,'{7}');", LineDTO.CustomerID, item.LineID, item.ActualHC, 0, item.OutputDate, item.ShiftTimeID, item.Created_UID, item.Created_Date, item.Modified_UID, item.Modified_Date);
                                    sb.AppendLine(insertSql);
                                }
                                else
                                {

                                    var updateSql = string.Format(@" UPDATE GL_BuildPlan SET
                                                                   ActualHC = {0}   
                                                                  ,Modified_UID ={1}
                                                                  ,Modified_Date = '{2}'
                                                                   WHERE   BuildPlanID ={3};", item.ActualHC, item.Modified_UID, item.Modified_Date, BuildPlanDTO.BuildPlanID);

                                    sb.AppendLine(updateSql);

                                }

                            }

                        }
                        string sql = sb.ToString();
                        DataContext.Database.ExecuteSqlCommand(sb.ToString());
                        trans.Commit();
                    }
                }

                return "SUCCESS";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
        public List<GLBuildPlanDTO> GetBuildPlans(int LineId, DateTime StartDate, DateTime EndDate)
        {
            var query = from w in DataContext.GL_BuildPlan
                        where w.GL_Line.IsEnabled == true
                        select new GLBuildPlanDTO
                        {
                            BuildPlanID = w.BuildPlanID,
                            CustomerID = w.CustomerID,
                            LineID = w.LineID,
                            PlanOutput = w.PlanOutput,
                            OutputDate = w.OutputDate,
                            ShiftTimeID = w.ShiftTimeID,
                            Created_UID = w.Created_UID,
                            Created_Date = w.Created_Date,
                            PlanHC = w.PlanHC,
                            ActualHC = w.ActualHC,
                            Plant_Organization_UID = w.GL_Line.Plant_Organization_UID,
                            BG_Organization_UID = w.GL_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = w.GL_Line.FunPlant_Organization_UID,
                            ProjectName = w.System_Project.Project_Name,
                            MESProjectName = w.System_Project.MESProject_Name,
                            LineName = w.GL_Line.LineName,
                            Plant_Organization = w.GL_Line.System_Organization.Organization_Name,
                            BG_Organization = w.GL_Line.System_Organization1.Organization_Name,
                            FunPlant_Organization = w.GL_Line.System_Organization2.Organization_Name,
                            ShiftTime = w.GL_ShiftTime.Shift
                        };


            if (LineId > 0)
            {
                query = query.Where(w => w.LineID == LineId);
            }
            //query = query.Where(w => Convert.ToDateTime(w.OutputDate) >= StartDate);
            //query = query.Where(w => Convert.ToDateTime(w.OutputDate) <= EndDate);
            //return query.ToList();

            var BuildPlans = query.ToList();
            if (BuildPlans != null)
            {

                BuildPlans = BuildPlans.Where(w => Convert.ToDateTime(w.OutputDate) >= StartDate && Convert.ToDateTime(w.OutputDate) < EndDate).ToList();

            }
            return BuildPlans;
        }

        public string SyncGoldenLineWeekPlan(List<GLBuildPlanDTO> GLBuildPlanDTOs)
        {
            try
            {

                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    if (GLBuildPlanDTOs != null && GLBuildPlanDTOs.Count > 0)
                    {
                        var LineDTOs = GetLineDTO();
                        var BuildPlanDTOs = DataContext.GL_BuildPlan.ToList();
                        foreach (var item in GLBuildPlanDTOs)
                        {

                            var LineDTO = LineDTOs.FirstOrDefault(o => o.LineID == item.LineID && o.IsEnabled == true);
                            var BuildPlanDTO = BuildPlanDTOs.FirstOrDefault(o => o.LineID == item.LineID && o.ShiftTimeID == item.ShiftTimeID && o.OutputDate == item.OutputDate && o.GL_Line.IsEnabled == true);
                            if (LineDTO != null)
                            {
                                if (BuildPlanDTO == null)
                                {
                                    string insertSql = "";

                                    #region  第二种方式
                                    insertSql = string.Format(@"INSERT INTO GL_BuildPlan
                                                           (CustomerID
                                                           ,LineID        
                                                           ,PlanOutput        
                                                           ,OutputDate
                                                           ,ShiftTimeID
                                                           ,Created_UID
                                                           ,Created_Date
                                                           ,Modified_UID
                                                           ,Modified_Date
                                                           ,PlanHC)
                                                          VALUES
                                                           (
		                                                   {0}
                                                          ,{1}
                                                          ,{2}
                                                         ,'{3}'
                                                          ,{4}
                                                          ,{5}
                                                         ,'{6}'
                                                          ,{7}
                                                         ,'{8}'
                                                          ,{9}         
		                                );", LineDTO.CustomerID, item.LineID, item.PlanOutput, item.OutputDate, item.ShiftTimeID, item.Created_UID, item.Created_Date, item.Modified_UID, item.Modified_Date, item.PlanHC != null ? item.PlanHC : -99);
                                    insertSql = insertSql.Replace("-99", "NULL");
                                    #endregion
                                    sb.AppendLine(insertSql);
                                }
                                else
                                {
                                    string updateSql = "";

                                    #region  第二种方式
                                    updateSql = string.Format(@" UPDATE GL_BuildPlan
                                                                   SET 
                                                                      PlanOutput = {0}
                                                                      ,PlanHC = {1}                                                                     
                                                                      ,Modified_UID = {2}
                                                                      ,Modified_Date = '{3}'
                                                                      WHERE   BuildPlanID ={4};", item.PlanOutput, item.PlanHC != null ? item.PlanHC : -99, item.Modified_UID, item.Modified_Date, BuildPlanDTO.BuildPlanID);
                                    updateSql = updateSql.Replace("-99", "NULL");
                                    #endregion

                                    sb.AppendLine(updateSql);
                                }
                            }
                        }
                        string sql = sb.ToString();
                        DataContext.Database.ExecuteSqlCommand(sb.ToString());
                        trans.Commit();
                    }
                }

                return "SUCCESS";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public List<GLBuildPlanDTO> GetWeekPlan(List<string> thisDates)
        {


            var query = from w in DataContext.GL_BuildPlan
                        where w.GL_Line.IsEnabled == true && w.GL_ShiftTime.IsEnabled == true
                        select new GLBuildPlanDTO
                        {
                            BuildPlanID = w.BuildPlanID,
                            CustomerID = w.CustomerID,
                            LineID = w.LineID,
                            PlanOutput = w.PlanOutput,
                            OutputDate = w.OutputDate,
                            ShiftTimeID = w.ShiftTimeID,
                            Created_UID = w.Created_UID,
                            Created_Date = w.Created_Date,
                            PlanHC = w.PlanHC,
                            ActualHC = w.ActualHC,
                            Modified_UID = w.Modified_UID,
                            Modified_Date = w.Modified_Date,
                            Plant_Organization_UID = w.GL_Line.Plant_Organization_UID,
                            BG_Organization_UID = w.GL_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = w.GL_Line.FunPlant_Organization_UID,
                            ProjectName = w.System_Project.Project_Name,
                            MESProjectName = w.System_Project.MESProject_Name,
                            LineName = w.GL_Line.LineName,
                            Plant_Organization = w.GL_Line.System_Organization.Organization_Name,
                            BG_Organization = w.GL_Line.System_Organization1.Organization_Name,
                            FunPlant_Organization = w.GL_Line.System_Organization2.Organization_Name,
                            ShiftTime = w.GL_ShiftTime.Shift
                        };

            query = query.Where(w => thisDates.Contains(w.OutputDate));
            return query.ToList();

        }

        public string InserOrUpdateStations(List<GL_StationDTO> listStations)
        {
            try
            {

                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    if (listStations != null && listStations.Count > 0)
                    {
                        //var LineDTOs = GetLineDTO();
                        //var BuildPlanDTOs = DataContext.GL_BuildPlan.ToList();
                        foreach (var item in listStations)
                        {

                            if (item.StationID == 0)
                            {
                                //构造插入SQL数据
                                var insertSql = string.Format(@"INSERT INTO GL_Station
                                                               (Plant_Organization_UID
                                                               ,BG_Organization_UID
                                                               ,FunPlant_Organization_UID
                                                               ,StationName
                                                               ,LineID
                                                               ,IsBirth
                                                               ,IsOutput
                                                               ,IsTest
                                                               ,Seq
                                                               ,IsEnabled
                                                               ,Modified_UID
                                                               ,Modified_Date
                                                               ,CycleTime
                                                               ,MESStationName
                                                               ,Binding_Seq
                                                               ,IsGoldenLine
                                                               ,IsOEE,
                                                                IsOne,
                                                               DashboardTarget)
                                                         VALUES
                                                               ({0}
                                                               ,{1}
                                                               ,{2}
                                                               ,N'{3}'
                                                               ,{4}
                                                               ,{5}
                                                               ,{6}
                                                               ,{7}
                                                               ,{8}
                                                               ,{9}
                                                               ,{10}
                                                               ,'{11}'
                                                               ,{12}
                                                               ,N'{13}'
                                                               ,{14}
                                                               ,{15}
                                                               ,{16}
                                                 ,{17},'{18}'  );",
                                                               item.Plant_Organization_UID, item.BG_Organization_UID, item.FunPlant_Organization_UID == null ? -99 : item.FunPlant_Organization_UID, item.StationName, item.LineID,
                                                               item.IsBirth ? 1 : 0, item.IsOutput ? 1 : 0, item.IsTest ? 1 : 0, item.Seq, item.IsEnabled ? 1 : 0, item.Modified_UID, item.Modified_Date, item.CycleTime, item.MESStationName, item.Binding_Seq, item.IsGoldenLine ? 1 : 0, item.IsOEE ? 1 : 0, item.IsOne ? 1 : 0 ,item.DashboardTarget);
                                insertSql = insertSql.Replace("-99", "NULL");
                                sb.AppendLine(insertSql);
                            }
                            else
                            {

                                var updateSql = string.Format(@" UPDATE GL_Station
                                                                   SET 
                                                                       LineID = {0}
                                                                      ,IsBirth = {1}
                                                                      ,IsOutput = {2}
                                                                      ,IsTest ={3}
                                                                      ,Seq ={4}
                                                                      ,IsEnabled = {5}
                                                                      ,Modified_UID = {6}
                                                                      ,Modified_Date = '{7}'
                                                                      ,CycleTime ={8}
                                                                      ,MESStationName = N'{9}'
                                                                      ,Binding_Seq={11}
                                                                      ,IsGoldenLine={12}
                                                                      ,IsOEE={13}
                                                                      ,IsOne={14}
                                                                      ,StationName=N'{15} ',
                                                                       DashboardTarget='{16}'
                                              WHERE   StationID ={10};", item.LineID, item.IsBirth ? 1 : 0, item.IsOutput ? 1 : 0, item.IsTest ? 1 : 0, item.Seq, item.IsEnabled ? 1 : 0,
                                          item.Modified_UID, item.Modified_Date, item.CycleTime, item.MESStationName, item.StationID, item.Binding_Seq, item.IsGoldenLine ? 1 : 0, item.IsOEE ? 1 : 0, item.IsOne ? 1 : 0,item.StationName,item.DashboardTarget);
                                sb.AppendLine(updateSql);

                            }



                        }
                        string sql = sb.ToString();
                        DataContext.Database.ExecuteSqlCommand(sb.ToString());
                        trans.Commit();
                    }
                }

                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public List<GL_StationDTO> GetStationDTO(int CustomerID)
        {
            var query = from w in DataContext.GL_Station
                        select new GL_StationDTO
                        {

                            StationID = w.StationID,
                            StationName = w.StationName,
                            LineID = w.LineID,
                            IsBirth = w.IsBirth,
                            IsOutput = w.IsOutput,
                            IsTest = w.IsTest,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            CustomerID = w.GL_Line.CustomerID,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            LineName = w.GL_Line.LineName,
                            LineIsEnabled = w.GL_Line.IsEnabled,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            LineCycleTime = w.GL_Line.CycleTime,
                            CycleTime = w.CycleTime,
                            MESStationName = w.MESStationName,
                            MESLineName = w.GL_Line.MESLineName,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name,
                            Binding_Seq = w.Binding_Seq,
                            IsGoldenLine = w.IsGoldenLine,
                            IsOEE = w.IsOEE,
                            IsOne = w.IsOne,
                            IsTwo = w.IsTwo,
                            IsThree = w.IsThree,
                            IsFour = w.IsFour,
                            IsFive = w.IsFive,
                            Plant_Organization=w.System_Organization.Organization_Name,
                            BG_Organization = w.System_Organization1.Organization_Name,
                            FunPlant_Organization = w.System_Organization2.Organization_Name,
                        };
            query = query.Where(o => o.IsOEE == true);
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.LineIsEnabled == true);
            if (CustomerID != 0)
            {
                query = query.Where(o => o.CustomerID == CustomerID);
            }
            return query.ToList();

        }

    }
}
