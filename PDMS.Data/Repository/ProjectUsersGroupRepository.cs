using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;



namespace PDMS.Data.Repository
{

    public class ProjectUsersGroupRepository : RepositoryBase<Project_Users_Group>, IProjectUsersGroupRepository
    {
        public ProjectUsersGroupRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
    public interface IProjectUsersGroupRepository : IRepository<Project_Users_Group>
    {

    }
}
