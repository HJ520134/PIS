using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{

    public interface ISyncGoldenLineService
    {


    }
    public class SyncGoldenLineService : ISyncGoldenLineService
    {
        private readonly IUnitOfWork unitOfWork;

        public SyncGoldenLineService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

        }




    }
}
