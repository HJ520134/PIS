using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service.ProductionPlanning
{
    //public interface IFlowchartDetailMEService
    //{
    //    string SaveFLDetailMEInfo(FlowchartDetailMEDTO dto, int AccountID);

    //}

    //public class FlowchartDetailMEService : IFlowchartDetailMEService
    //{
    //    #region Private interfaces properties

    //    private readonly IUnitOfWork unitOfWork;
    //    //private readonly IFlowchartDetailMERepository flowchartDetailMERepository;

    //    public FlowchartDetailMEService(
    //        //IFlowchartDetailMERepository flowchartDetailMERepository,
    //        IUnitOfWork unitOfWork)
    //    {
    //        this.flowchartDetailMERepository = flowchartDetailMERepository;
    //        this.unitOfWork = unitOfWork;
    //    }

    //    #endregion //Private interfaces properties

    //    #region Service constructor


    //    public string SaveFLDetailMEInfo(FlowchartDetailMEDTO dto, int AccountID)
    //    {
    //        var item = flowchartDetailMERepository.GetById(dto.Flowchart_Detail_ME_UID);

    //        item.Process_Station = dto.Process_Station;
    //        item.System_FunPlant_UID = dto.System_FunPlant_UID;
    //        item.Process = dto.Process;
    //        item.Process_Desc = dto.Process_Desc;
    //        item.Processing_Equipment = dto.Processing_Equipment;
    //        item.Automation_Equipment = dto.Automation_Equipment;
    //        item.Auxiliary_Equipment = dto.Auxiliary_Equipment;
    //        item.Equipment_CT = dto.Equipment_CT;
    //        item.Setup_Time = dto.Setup_Time;
    //        item.Total_Cycletime = dto.Total_Cycletime;
    //        item.Estimate_Yield = dto.Estimate_Yield / 100;
    //        if (dto.Manpower_Ratio != null)
    //        {
    //            dto.Manpower_Ratio = Math.Round(dto.Manpower_Ratio.Value, 2, MidpointRounding.AwayFromZero);
    //        }
    //        item.Manpower_Ratio = dto.Manpower_Ratio;
    //        item.Modified_UID = AccountID;
    //        item.Modified_Date = DateTime.Now;
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }

    //    #endregion
    //}
}
