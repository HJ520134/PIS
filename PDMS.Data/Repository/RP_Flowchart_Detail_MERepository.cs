using PDMS.Data.Infrastructure;

namespace PDMS.Data.Repository
{
    public interface IRP_Flowchart_Detail_MERepository : IRepository<RP_Flowchart_Detail_ME>
    {

    }
    public class RP_Flowchart_Detail_MERepository : RepositoryBase<RP_Flowchart_Detail_ME>, IRP_Flowchart_Detail_MERepository
    {
        public RP_Flowchart_Detail_MERepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
}
