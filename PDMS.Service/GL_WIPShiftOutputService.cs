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
    public interface IGL_WIPShiftOutputService : IBaseSevice<GL_WIPShiftOutput, GL_WIPShiftOutputDTO, GL_WIPShiftOutputModelSearch> { }
    public class GL_WIPShiftOutputService : BaseSevice<GL_WIPShiftOutput, GL_WIPShiftOutputDTO, GL_WIPShiftOutputModelSearch>, IGL_WIPShiftOutputService
    {
        private readonly IGL_WIPShiftOutputRepository stationRepository;
        public GL_WIPShiftOutputService(IGL_WIPShiftOutputRepository stationRepository) : base(stationRepository)
        {
            this.stationRepository = stationRepository;
        }
    }
}
