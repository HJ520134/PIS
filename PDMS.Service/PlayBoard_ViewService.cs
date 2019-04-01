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
    public interface IPlayBoard_ViewService : IBaseSevice<PlayBoard_View, PlayBoard_ViewDTO, PlayBoard_ViewModelSearch>
    {
    }

    public class PlayBoard_ViewService : BaseSevice<PlayBoard_View, PlayBoard_ViewDTO, PlayBoard_ViewModelSearch>, IPlayBoard_ViewService
    {
        private readonly IPlayBoard_ViewRepository viewRepository;
        public PlayBoard_ViewService(IPlayBoard_ViewRepository viewRepository) : base(viewRepository)
        {
            this.viewRepository = viewRepository;
        }
    }
}
