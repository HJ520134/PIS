using PDMS.Data;
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
    public interface IGL_WIPShiftBatchOutputService : IBaseSevice<GL_WIPShiftBatchOutput, GL_WIPShiftBatchOutputDTO, GL_WIPShiftBatchOutputModelSearch> { }
    public class GL_WIPShiftBatchOutputService : BaseSevice<GL_WIPShiftBatchOutput, GL_WIPShiftBatchOutputDTO, GL_WIPShiftBatchOutputModelSearch>, IGL_WIPShiftBatchOutputService
    {
        private readonly IGL_WIPShiftBatchOutputRepository wipShiftBatchOutputRepository;
        public GL_WIPShiftBatchOutputService(IGL_WIPShiftBatchOutputRepository wipShiftBatchOutputRepository) : base(wipShiftBatchOutputRepository)
        {
            this.wipShiftBatchOutputRepository = wipShiftBatchOutputRepository;
        }
    }
}
