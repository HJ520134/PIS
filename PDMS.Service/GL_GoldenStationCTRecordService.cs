using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IGL_GoldenStationCTRecordService
    {
        List<GoldenLineCTRecordDTO> GetGoldenLineCTRecordDTO(int stationID, int shiftTimeID, string cycleTimeDate);
        string InsertANDUpdateGoldenLineCTRecord(List<GoldenLineCTRecordDTO> GoldenLineCTRecordDTOs);
        List<GL_StationDTO> GetStationDTO();
        List<GL_ShiftTimeDTO> GetShiftTimeDTO(int Plant_Organization_UID, int BG_Organization_UID);
        List<GL_LineDTO> GetLineDTO();
        List<SystemProjectDTO> GetCustomerDTOs(int BG_Organization_UID);
        List<GL_StationDTO> GetStationDTOs(int CustomerID);
        List<GL_StationDTO> GetONOMESStationDTOs(int CustomerID);
        
        List<GL_StationDTO> GetStationDTOsByLineID(int LineID);
        List<GL_LineDTO> GetLineDTOs(int CustomerID);
        List<GL_LineDTO> GetReportLineDTOs(int CustomerID);

        List<GL_LineDTO> GetAllLineDTOs(int CustomerID);
        List<WipGroupLineItem> GetGroupLineDTOs(int CustomerID);

        string SyncGoldenLineWeekPlan(List<GLBuildPlanDTO> GLBuildPlanDTOs);
        List<GLBuildPlanDTO> GetWeekPlan(List<string> thisDate);
    }
    public class GL_GoldenStationCTRecordService : IGL_GoldenStationCTRecordService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGL_GoldenStationCTRecordRepository gL_GoldenStationCTRecordRepository;
        public GL_GoldenStationCTRecordService(IGL_GoldenStationCTRecordRepository gL_GoldenStationCTRecordRepository, IUnitOfWork unitOfWork)
        {
            this.gL_GoldenStationCTRecordRepository = gL_GoldenStationCTRecordRepository;
            this.unitOfWork = unitOfWork;
        }


        public List<GoldenLineCTRecordDTO> GetGoldenLineCTRecordDTO(int stationID, int shiftTimeID, string cycleTimeDate)
        {
            return gL_GoldenStationCTRecordRepository.GetGoldenLineCTRecordDTO(stationID, shiftTimeID, cycleTimeDate);
        }
        public    string InsertANDUpdateGoldenLineCTRecord(List<GoldenLineCTRecordDTO> GoldenLineCTRecordDTOs)
        {

            return gL_GoldenStationCTRecordRepository.InsertANDUpdateGoldenLineCTRecord(GoldenLineCTRecordDTOs);
        }


        public List<GL_StationDTO> GetStationDTO() {

            return gL_GoldenStationCTRecordRepository.GetStationDTO();
        }

        public List<GL_ShiftTimeDTO> GetShiftTimeDTO(int Plant_Organization_UID, int BG_Organization_UID)
        {
            return gL_GoldenStationCTRecordRepository.GetShiftTimeDTO(Plant_Organization_UID, BG_Organization_UID);
        }

        public List<GL_LineDTO> GetLineDTO()
        {
            return gL_GoldenStationCTRecordRepository.GetLineDTO();
        }

        public List<SystemProjectDTO> GetCustomerDTOs(int BG_Organization_UID)
        {

            return gL_GoldenStationCTRecordRepository.GetCustomerDTOs(BG_Organization_UID);

        }

        public List<GL_StationDTO> GetStationDTOs(int LineId )
        {
            return gL_GoldenStationCTRecordRepository.GetStationDTOs(LineId);

        }
        public List<GL_StationDTO> GetONOMESStationDTOs(int LineId)
        {
            return gL_GoldenStationCTRecordRepository.GetONOMESStationDTOs(LineId);

        }
        
        public List<GL_StationDTO> GetStationDTOsByLineID(int LineID)
        {

            return gL_GoldenStationCTRecordRepository.GetStationDTOsByLineID(LineID);
        }

        public List<GL_LineDTO> GetLineDTOs(int CustomerID)
        {
            return gL_GoldenStationCTRecordRepository.GetLineDTOs(CustomerID);
        }
        public List<GL_LineDTO> GetReportLineDTOs(int CustomerID)
        {

            return gL_GoldenStationCTRecordRepository.GetReportLineDTOs(CustomerID);
        }


        public List<GL_LineDTO> GetAllLineDTOs(int CustomerID)
        {
            return gL_GoldenStationCTRecordRepository.GetAllLineDTOs(CustomerID);
        }
        public List<WipGroupLineItem> GetGroupLineDTOs(int CustomerID)
        {
            return gL_GoldenStationCTRecordRepository.GetGroupLineDTOs(CustomerID);
        }

        public string SyncGoldenLineWeekPlan(List<GLBuildPlanDTO> GLBuildPlanDTOs)
        {

            return gL_GoldenStationCTRecordRepository.SyncGoldenLineWeekPlan(GLBuildPlanDTOs);
        }

        public  List<GLBuildPlanDTO> GetWeekPlan(List<string> thisDates)
        {

            return gL_GoldenStationCTRecordRepository.GetWeekPlan(thisDates);

        }
    }
}
