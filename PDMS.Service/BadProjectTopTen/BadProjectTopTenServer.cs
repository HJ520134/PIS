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
    public class BadProjectTopTenServer : IBadProjectTopTenServer
    {
        private readonly IBadProjectTopTenDataServer BadProjectTopTenData;

        public PagedListModel<QEboardSumModel> GteQEboardSumDetailData(string Projects)
        {
            var totalCount = 0;
            var result = BadProjectTopTenData.GteQEboardSumDetailData(Projects);
            return new PagedListModel<QEboardSumModel>(totalCount, result);
        }
    }
}
