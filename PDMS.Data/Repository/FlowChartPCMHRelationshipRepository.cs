using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.ViewModels;

namespace PDMS.Data.Repository
{
    public interface IFlowChartPCMHRelationshipRepository : IRepository<FlowChart_PC_MH_Relationship>
    {
        int InsertBomUserInfo(List<FlowChartPCMHRelationshipVM> list);
    }

    public class FlowChartPCMHRelationshipRepository : RepositoryBase<FlowChart_PC_MH_Relationship>, IFlowChartPCMHRelationshipRepository
    {
        public FlowChartPCMHRelationshipRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

        public int InsertBomUserInfo(List<FlowChartPCMHRelationshipVM> list)
        {
            //System_Users表这张表的主键不是自增列，所以要手动查最大的主键列
            var selectMaxIDSql = "select max(Account_UID) from System_Users";
            var maxID = DataContext.Database.SqlQuery<int>(selectMaxIDSql).First();
            return maxID;
        }


    }
}