using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IModelLineHRService : IBaseSevice<ModelLineHR, ModelLineHRDTO, ModelLineHRModelSearch>
    {
        string InsertList(List<ModelLineHRDTO> list);
    }
    public class ModelLineHRService : BaseSevice<ModelLineHR, ModelLineHRDTO, ModelLineHRModelSearch>, IModelLineHRService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IModelLineHRRepository modelLineHRRepository;
        public ModelLineHRService(IUnitOfWork unitOfWork,IModelLineHRRepository modelLineHRRepository)
            : base(modelLineHRRepository)
        {
            this.unitOfWork = unitOfWork;
            this.modelLineHRRepository = modelLineHRRepository;
        }

        public string InsertList(List<ModelLineHRDTO> list)
        {
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
    }
}
