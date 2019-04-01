using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;


namespace PDMS.Service
{
    public interface IElectricalBoardService
    {
        PagedListModel<NoticeVM> QueryNotice(NoticeSearch search, Page page);
        string AddNotice(Notice ent);
        PagedListModel<TwoHourCapacityDTO> GetCapacity();
        PagedListModel<TwoHourCapacityDTO> GetTotalCapacity();
        string InsertModelLineHRList(List<ModelLineHRDTO> list);
        string CoverModelLineHRList(List<ModelLineHRDTO> list);
        PagedListModel<ModelLineHRDTO> QueryModelLineHRs();
        ModelLineHRDTO QueryModelLineHRSingle(int uid);
        string AddModelLineHR(ModelLineHRDTO dto);
        string EditModelLineHR(ModelLineHRDTO dto);
        string DeleteModelLineHR(int uid);
    }

    public class ElectricalBoardService : IElectricalBoardService
    {
        #region Private interfaces properties
        private readonly IUnitOfWork unitOfWork;
        private readonly INoticeRepository NoticeRepository;
        private readonly IElectricalBoardDTRepository ElectricalBoardDTRepository;
        private readonly IModelLineHRRepository modelLineHRRepository;

        #endregion //Private interfaces properties

        #region Service constructor
        public ElectricalBoardService(
        INoticeRepository NoticeRepository,IUnitOfWork unitOfWork, IElectricalBoardDTRepository ElectricalBoardDTRepository, IModelLineHRRepository modelLineHRRepository)
        {
            this.NoticeRepository = NoticeRepository;
           
            this.unitOfWork = unitOfWork;
            this.ElectricalBoardDTRepository = ElectricalBoardDTRepository;
            this.modelLineHRRepository = modelLineHRRepository;
        }
        #endregion //Service constructor

        public PagedListModel<NoticeVM> QueryNotice(NoticeSearch search, Page page)
        {
            var totalCount = 0;
            var list = NoticeRepository.QueryNotice(search, page, out totalCount);
            IList<NoticeVM> listDTO = new List<NoticeVM>();

            foreach (var item in list)
            {
                var ent = new NoticeVM();
                ent.Creator_User = item.Creator_User;
                ent.Creat_Time = item.Creat_Time;
                ent.Notice_Content = item.Notice_Content;
                ent.Period = item.Start_Time.ToLocalTime() + "~" + item.End_Time.ToLocalTime();
                ent.Scope = item.Scope;
                ent.State = item.State;
                ent.UID = item.UID;
                listDTO.Add(ent);
            }
            return new PagedListModel<NoticeVM>(totalCount, listDTO);
        }

        public string AddNotice(Notice ent)
        {
          
            try
            {
                NoticeRepository.Add(ent);
                unitOfWork.Commit();
            }
            catch
            {
                return "新增失败，请核对信息后再重试，或联系系统管理员。";
            }
            return "OK";
           
        }
        
        public PagedListModel<TwoHourCapacityDTO> GetCapacity()
        {
            var modelList = ElectricalBoardDTRepository.GetTwoHourCapacity();
            var result = new PagedListModel<TwoHourCapacityDTO>(modelList.Count,modelList);
            return result;
        }

        public PagedListModel<TwoHourCapacityDTO> GetTotalCapacity()
        {
            var modelList = ElectricalBoardDTRepository.GetTwoHourTotalCapacity();
            var result = new PagedListModel<TwoHourCapacityDTO>(modelList.Count,modelList);
            return result;
        }

        public string InsertModelLineHRList(List<ModelLineHRDTO> list) {
            var data = AutoMapper.Mapper.Map<List<ModelLineHR>>(list);
            string result = "";
            try
            {
                modelLineHRRepository.AddList(data);
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }
        public string CoverModelLineHRList(List<ModelLineHRDTO> list) {
            if (list.Count>0)
            {
                //删除全部
                var oldData = modelLineHRRepository.GetAll().ToList();
                modelLineHRRepository.DeleteList(oldData);
                //新增
                var data = AutoMapper.Mapper.Map<List<ModelLineHR>>(list);
                string result = "";
                try
                {
                    modelLineHRRepository.AddList(data);
                    unitOfWork.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    result = "Error" + ex;
                }
                return result;
            }
            else
            {
                return "没有可新增的数据";
            }
        }

        public PagedListModel<ModelLineHRDTO> QueryModelLineHRs()
        {
            var allData = AutoMapper.Mapper.Map<List<ModelLineHRDTO>>(modelLineHRRepository.GetAll().ToList());
            var count = allData.Count;
            var sumData = new ModelLineHRDTO() {
                Station = "合计",
                Total = allData.Sum(m => m.Total),
                ShouldCome = allData.Sum(m => m.ShouldCome),
                ActualCome = allData.Sum(m => m.ActualCome),
                VacationLeave = allData.Sum(m => m.VacationLeave),
                PersonalLeave = allData.Sum(m => m.PersonalLeave),
                SickLeave = allData.Sum(m => m.SickLeave),
                AbsentLeave = allData.Sum(m => m.AbsentLeave),
                Created_Date = DateTime.Now,
                Modified_Date = DateTime.Now
            };
            allData.Add(sumData);
            var result = new PagedListModel<ModelLineHRDTO>(count,allData);
            return result;
        }

        public ModelLineHRDTO QueryModelLineHRSingle(int uid)
        {
            var modelLineHR = modelLineHRRepository.GetById(uid);
            var dto = AutoMapper.Mapper.Map<ModelLineHRDTO>(modelLineHR);
            return dto;
        }

        public string AddModelLineHR(ModelLineHRDTO dto)
        {
            var entity = AutoMapper.Mapper.Map<ModelLineHR>(dto);
            
            modelLineHRRepository.Add(entity);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string EditModelLineHR(ModelLineHRDTO dto)
        {
            var dtoDB = modelLineHRRepository.GetById(dto.ModelLineHR_UID);

            dtoDB.Station = dto.Station;
            dtoDB.Total = dto.Total;
            dtoDB.ShouldCome = dto.ShouldCome;
            dtoDB.ActualCome = dto.ActualCome;
            dtoDB.VacationLeave = dto.VacationLeave;
            dtoDB.PersonalLeave = dto.PersonalLeave;
            dtoDB.SickLeave = dto.SickLeave;
            dtoDB.AbsentLeave = dto.AbsentLeave;
            dtoDB.Modified_Date = DateTime.Now;
            modelLineHRRepository.Update(dtoDB);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string DeleteModelLineHR(int uid)
        {
            var dtoDB = modelLineHRRepository.GetById(uid);
            modelLineHRRepository.Delete(dtoDB);
            unitOfWork.Commit();
            return "SUCCESS";
        }
    }
}
