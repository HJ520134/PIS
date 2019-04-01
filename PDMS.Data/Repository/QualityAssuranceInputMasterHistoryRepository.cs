using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;

namespace PDMS.Data.Repository
{
    public interface IQualityAssuranceInputMasterHistoryRepository : IRepository<QualityAssurance_InputMaster_History>
    {

    }

    public class QualityAssuranceInputMasterHistoryRepository : RepositoryBase<QualityAssurance_InputMaster_History>, IQualityAssuranceInputMasterHistoryRepository
    {
        public QualityAssuranceInputMasterHistoryRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }


    }
}
