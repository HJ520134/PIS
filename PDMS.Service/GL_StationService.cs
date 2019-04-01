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
    public interface IGL_StationService : IBaseSevice<GL_Station, GL_StationDTO, GL_StationModelSearch> { }
    public class GL_StationService : BaseSevice<GL_Station, GL_StationDTO, GL_StationModelSearch>, IGL_StationService
    {
        private readonly IGL_StationRepository stationRepository;
        public GL_StationService(IGL_StationRepository stationRepository) : base(stationRepository)
        {
            this.stationRepository = stationRepository;
        }
    }
}
