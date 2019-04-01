using PDMS.Data.Infrastructure;

namespace PDMS.Data.Repository
{
    public interface IRP_Flowchart_Detail_ME_EquipmentRepository : IRepository<RP_Flowchart_Detail_ME_Equipment>
    {

    }
    public class RP_Flowchart_Detail_ME_EquipmentRepository : RepositoryBase<RP_Flowchart_Detail_ME_Equipment>, IRP_Flowchart_Detail_ME_EquipmentRepository
    {
        public RP_Flowchart_Detail_ME_EquipmentRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
}