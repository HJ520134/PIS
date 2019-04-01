using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;

namespace PDMS.Data.Repository
{
    public interface IOQCInputMasterHistoryRepository : IRepository<OQC_InputMaster_History>
    {

    }


    public class OQCInputMasterHistoryRepository : RepositoryBase<OQC_InputMaster_History>, IOQCInputMasterHistoryRepository
    {
        public OQCInputMasterHistoryRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
    }
}
