using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IBadProjectTopTenDataServer
    {
        List<QEboardSumModel> GteQEboardSumDetailData(string Projects);
    }
}
