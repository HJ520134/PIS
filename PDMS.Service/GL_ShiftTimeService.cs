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
    public interface IGL_ShiftTimeService : IBaseSevice<GL_ShiftTime, GL_ShiftTimeDTO, GL_ShiftTimeModelSearch>
    {

    }
    public class GL_ShiftTimeService : BaseSevice<GL_ShiftTime, GL_ShiftTimeDTO, GL_ShiftTimeModelSearch>, IGL_ShiftTimeService
    {
        private readonly IGL_ShiftTimeRepository shiftTimeRepository;
        public GL_ShiftTimeService(IGL_ShiftTimeRepository shiftTimeRepository) : base(shiftTimeRepository)
        {
            this.shiftTimeRepository = shiftTimeRepository;
        }
    }
}
