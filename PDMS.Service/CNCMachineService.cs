using PDMS.Data;
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

    public interface ICNCMachineService
    {
        PagedListModel<CNCMachineDTO> QueryCNCMachineDTOs(CNCMachineDTO searchModel, Page page);
        string AddOrEditCNCMachineAPI(CNCMachineDTO dto);
        CNCMachineDTO QueryCNCMachineDTOByUid(int CNCMachineUID);
        List<SystemProjectDTO> GetSystemProjectDTO(int Plant_Organization_UID, int BG_Organization_UID);
        string DeleteCNCMachine(int CNCMachineUID, int userid);

        List<CNCMachineDTO> DoAllExportCNCMachineReprot(CNCMachineDTO searchModel);
        List<CNCMachineDTO> DoExportCNCMachineReprot(string uids);
        List<EquipmentInfoDTO> GetAllEquipmentInfoDTOs();
        List<CNCMachineDTO> GetAllCNCMachineDTOList();
        string ImportMachine(List<CNCMachineDTO> CNCMachineDTOs);

        PagedListModel<CNCMachineReportDTO> QueryReportCNCMachineDatas(CNCMachineDTO searchModel, Page page);
        List<CNCMachineReportDTO> DoAllExportMachineReport(CNCMachineDTO searchModel);
        bool UpdateColumnInfo(int Account_UID, string Column_Name, bool isDisplay);
        List<CNCMachineColumnTableDTO> GetCNCMachineColumnTableDTOs(int Account_UID);
        string InsertMachineColumnTable(List<CNCMachineColumnTableDTO> CNCMachineDTOs);

        List<CNCMachineDTO> GetCNCMachineList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);

        List<CNCMachineHisReportDTO> DoHisExportMachineReport(int Plant_Organization_UID, string Machine_Name, DateTime? Date_From, DateTime? Date_To);
    }
    public class CNCMachineService : ICNCMachineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICNCMachineRepository CNCMachineRepository;
        private readonly ICNCMachineColumnTableRepository CNCMachineColumnTableRepository;
        public CNCMachineService(
          IUnitOfWork unitOfWork,
          ICNCMachineRepository CNCMachineRepository,
          ICNCMachineColumnTableRepository CNCMachineColumnTableRepository
         )
        {
            this._unitOfWork = unitOfWork;
            this.CNCMachineRepository = CNCMachineRepository;
            this.CNCMachineColumnTableRepository = CNCMachineColumnTableRepository;

        }

        public PagedListModel<CNCMachineDTO> QueryCNCMachineDTOs(CNCMachineDTO searchModel, Page page)
        {
            int totalcount;
            var result = CNCMachineRepository.QueryCNCMachineDTOs(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<CNCMachineDTO>(totalcount, result);
            return bd;
        }
        public string AddOrEditCNCMachineAPI(CNCMachineDTO dto)
        {
            try
            {
                int EQP_Uid = CNCMachineRepository.GetEquipment_UID(dto.BG_Organization_UID, dto.FunPlant_Organization_UID == null ? 0 : dto.FunPlant_Organization_UID.Value, dto.Equipment);

                if (EQP_Uid == 0)
                {
                    return "保存失败:没有找到对应的设备EMT号.";
                }

                if (dto.CNCMachineUID == 0)
                {

                    CNCMachine SC = new CNCMachine();

                    SC.Plant_Organization_UID = dto.Plant_Organization_UID;
                    SC.BG_Organization_UID = dto.BG_Organization_UID;
                    SC.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    if (EQP_Uid != 0)
                    {
                        SC.EQP_Uid = EQP_Uid;
                    }
                    else
                    {
                        SC.EQP_Uid = null;
                    }

                    SC.Machine_Name = dto.Machine_Name;
                    SC.Machine_ID = dto.Machine_ID;
                    SC.Project_UID = dto.Project_UID;
                    SC.Is_Enable = dto.Is_Enable;
                    SC.Modified_UID = dto.Modified_UID;
                    SC.Modified_Date = dto.Modified_Date;
                    CNCMachineRepository.Add(SC);

                    _unitOfWork.Commit();
                    return "";
                }
                else
                {

                    var SC = CNCMachineRepository.GetFirstOrDefault(m => m.CNCMachineUID == dto.CNCMachineUID);

                    SC.Plant_Organization_UID = dto.Plant_Organization_UID;
                    SC.BG_Organization_UID = dto.BG_Organization_UID;
                    SC.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    if (EQP_Uid != 0)
                    {
                        SC.EQP_Uid = EQP_Uid;
                    }
                    else
                    {
                        SC.EQP_Uid = null;
                    }
                    SC.Machine_Name = dto.Machine_Name;
                    SC.Machine_ID = dto.Machine_ID;
                    SC.Project_UID = dto.Project_UID;
                    SC.Is_Enable = dto.Is_Enable;
                    SC.Modified_UID = dto.Modified_UID;
                    SC.Modified_Date = dto.Modified_Date;

                    CNCMachineRepository.Update(SC);
                    _unitOfWork.Commit();

                    return "0";
                }

            }
            catch (Exception e)
            {
                return "保存失败:" + e.Message;
            }
        }

      public  CNCMachineDTO QueryCNCMachineDTOByUid(int CNCMachineUID)
        {

            var bud = CNCMachineRepository.QueryCNCMachineDTOByUid(CNCMachineUID);
            return bud;
        }

        public List<SystemProjectDTO> GetSystemProjectDTO(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var result = CNCMachineRepository.GetSystemProjectDTO(Plant_Organization_UID, BG_Organization_UID);
            return result;
        }

        public  string DeleteCNCMachine(int CNCMachineUID, int userid)
        {
            try
            {
                var storagecheck = CNCMachineRepository.GetById(CNCMachineUID);
     
                CNCMachineRepository.Delete(storagecheck);
                _unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除机台信息失败:" + e.Message;
            }

        }


        public List<CNCMachineDTO> DoAllExportCNCMachineReprot(CNCMachineDTO searchModel)
        {
            return CNCMachineRepository.DoAllExportCNCMachineReprot(searchModel);
        }
        public List<CNCMachineDTO> DoExportCNCMachineReprot(string uids)
        {
            return CNCMachineRepository.DoExportCNCMachineReprot(uids);
        }

        public List<EquipmentInfoDTO> GetAllEquipmentInfoDTOs()
        {
            return CNCMachineRepository.GetAllEquipmentInfoDTOs();
        }
        public List<CNCMachineDTO> GetAllCNCMachineDTOList()
        {
            return CNCMachineRepository.GetAllCNCMachineDTOList();
        }

        public string ImportMachine(List<CNCMachineDTO> CNCMachineDTOs)
        {

            return CNCMachineRepository.ImportMachine(CNCMachineDTOs);

        }
        public PagedListModel<CNCMachineReportDTO> QueryReportCNCMachineDatas(CNCMachineDTO searchModel, Page page)
        {
            int totalcount;
            var result = CNCMachineRepository.QueryReportCNCMachineDatas(searchModel, page, out totalcount);
            var bd = new PagedListModel<CNCMachineReportDTO>(totalcount, result);
            return bd;
        }

        public List<CNCMachineReportDTO> DoAllExportMachineReport(CNCMachineDTO searchModel)
        {
            return CNCMachineRepository.DoAllExportMachineReport(searchModel);

        }

        public bool UpdateColumnInfo(int Account_UID, string Column_Name, bool isDisplay)
        {
            bool result = true;
            var columnItem = CNCMachineColumnTableRepository.GetMany(m => m.Column_Name.ToLower() == Column_Name.ToLower() && m.NTID == Account_UID).FirstOrDefault();
            switch (isDisplay)
            {
                case false:
                    if (columnItem != null)
                    {
                        CNCMachineColumnTableRepository.Delete(columnItem);
                        _unitOfWork.Commit();
                    }
                    break;
                case true:
                    if (columnItem == null)
                    {
                        CNCMachineColumnTable item = new CNCMachineColumnTable();
                        item.NTID = Account_UID;
                        item.Column_Name = Column_Name;
                        item.Modified_Date = DateTime.Now;
                        CNCMachineColumnTableRepository.Add(item);
                        _unitOfWork.Commit();
                    }
                    break;


            }
            return result;
        }


        public List<CNCMachineColumnTableDTO> GetCNCMachineColumnTableDTOs(int Account_UID)
        {
            return CNCMachineColumnTableRepository.GetCNCMachineColumnTableDTOs( Account_UID);

        }

       public string InsertMachineColumnTable(List<CNCMachineColumnTableDTO> CNCMachineDTOs)
        {
            return CNCMachineColumnTableRepository.InsertMachineColumnTable(CNCMachineDTOs);
        }

        public List<CNCMachineDTO> GetCNCMachineList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            return CNCMachineRepository.GetCNCMachineList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);

        }
        public List<CNCMachineHisReportDTO> DoHisExportMachineReport(int Plant_Organization_UID, string Machine_Name, DateTime? Date_From, DateTime? Date_To)
        {
            return CNCMachineRepository.DoHisExportMachineReport(Plant_Organization_UID, Machine_Name, Date_From, Date_To);

        }
    }
}
