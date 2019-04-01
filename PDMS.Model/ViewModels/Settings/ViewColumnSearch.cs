using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.Settings
{
    public class ViewColumnSearch : BaseModel
    {

    }

    public class ProductReportDisplay : BaseModel
    {
        public List<SystemViewColumnDTO> ColumnDTOList { get; set; }

        public List<SystemUserViewDTO> ViewDTOList { get; set; }
    }
}
