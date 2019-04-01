using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IIPQCQUalityReportRepository : IRepository<IPQCQualityReport>
    {
        List<GL_StationDTO> GetStationDTOs(int LineId);
        List<IPQCQualityReportDto> GetIPQCQualityReport(IPQCQualityReportVM serchModel, bool isMonth);
        GL_IPQCQualityReportDTO GetGLIPQCQualityReportDTO(int StationID, int ShiftID, int TimeIntervalIndex, DateTime ProductDate, string TimeInterval);
    }

    public class IPQCQUalityReportRepository : RepositoryBase<IPQCQualityReport>, IIPQCQUalityReportRepository
    {
        public IPQCQUalityReportRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
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
                            IsFive = w.IsFive
                        };
            //query = query.Where(o => o.IsGoldenLine == true);
            query = query.Where(o => o.IsTest == true);
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.LineIsEnabled == true);
            query = query.Where(o => o.LineID == LineID);
            return query.ToList();
        }

        public List<IPQCQualityReportDto> GetIPQCQualityReport(IPQCQualityReportVM serchModel, bool isMonth)
        {
            var query = from ipqc in DataContext.IPQCQualityReport
                        where ipqc.GL_Station.Plant_Organization_UID == serchModel.Plant_Organization_UID &&
                         ipqc.GL_Station.BG_Organization_UID == serchModel.BG_Organization_UID &&
                          //ipqc.GL_Station.FunPlant_Organization_UID == serchModel.FunPlant_Organization_UID &&
                          ipqc.GL_Station.GL_Line.CustomerID == serchModel.CustomerID &&
                         ipqc.GL_Station.LineID == serchModel.LineID &&
                         ipqc.GL_Station.StationID == serchModel.StationID &&
                         ipqc.GL_Station.IsTest == true
                        select new IPQCQualityReportDto
                        {
                            IPQCQualityReport_UID = ipqc.IPQCQualityReport_UID,
                            StationID = ipqc.StationID,
                            ShiftID = ipqc.ShiftID,
                            TimeInterval = ipqc.TimeInterval,
                            ProductDate = ipqc.ProductDate,
                            FirstYield = ipqc.FirstYield,
                            FirstTargetYield = ipqc.FirstTargetYield,
                            SecondYield = ipqc.SecondYield,
                            SecondTargetYield = ipqc.SecondTargetYield,
                            InputNumber = ipqc.InputNumber,
                            TestNumber = ipqc.TestNumber,
                            FirstPassNumber = ipqc.FirstPassNumber,
                            SecondPassNumber = ipqc.SecondPassNumber,
                            RepairNumber = ipqc.RepairNumber,
                            NGNumber = ipqc.NGNumber,
                            TimeIntervalIndex = ipqc.TimeIntervalIndex,
                            WIP = ipqc.WIP,
                            FunPlant_Organization_UID= ipqc.GL_Station.GL_Line.FunPlant_Organization_UID,
                        };
            if (serchModel.ShiftTimeID != 0)
            {
                query = query.Where(p => p.ShiftID == serchModel.ShiftTimeID);
            }

            if (serchModel.FunPlant_Organization_UID!=0)
            {
                query = query.Where(p => p.FunPlant_Organization_UID == serchModel.FunPlant_Organization_UID);
            }

            //月报表
            if (isMonth)
            {
                query = query.Where(p => p.ProductDate >= serchModel.StartTime && p.ProductDate <= serchModel.EndTime);
            }
            else
            {
                if (serchModel.ProductDate != null)
                {
                    query = query.Where(p => p.ProductDate == serchModel.ProductDate);
                }
            }
            return query.ToList();
        }


      public   GL_IPQCQualityReportDTO GetGLIPQCQualityReportDTO(int StationID, int ShiftID, int TimeIntervalIndex, DateTime ProductDate, string TimeInterval)
        {
            var query = from ipqc in DataContext.IPQCQualityReport
                        where
                         ipqc.GL_Station.StationID == StationID &&
                         ipqc.ProductDate == ProductDate &&
                         ipqc.TimeInterval == TimeInterval &&
                         ipqc.ShiftID == ShiftID &&
                         ipqc.TimeIntervalIndex == TimeIntervalIndex
                        select new GL_IPQCQualityReportDTO
                        {
                            IPQCQualityReport_UID = ipqc.IPQCQualityReport_UID,
                            StationID = ipqc.StationID,
                            ShiftID = ipqc.ShiftID,
                            TimeInterval = ipqc.TimeInterval,
                            ProductDate = ipqc.ProductDate,
                            FirstYield = ipqc.FirstYield,
                            FirstTargetYield = ipqc.FirstTargetYield,
                            SecondYield = ipqc.SecondYield,
                            SecondTargetYield = ipqc.SecondTargetYield,
                            InputNumber = ipqc.InputNumber,
                            TestNumber = ipqc.TestNumber,
                            FirstPassNumber = ipqc.FirstPassNumber,
                            SecondPassNumber = ipqc.SecondPassNumber,
                            RepairNumber = ipqc.RepairNumber,
                            NGNumber = ipqc.NGNumber,
                            TimeIntervalIndex = ipqc.TimeIntervalIndex,
                            WIP = ipqc.WIP,
                        };

            return query.FirstOrDefault();
        }


    }
}
