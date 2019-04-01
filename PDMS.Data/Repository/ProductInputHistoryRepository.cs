using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace PDMS.Data.Repository
{
    public interface IProductInputHistoryRepository : IRepository<Product_Input_History>
    {

    }
    public class ProductInputHistoryRepository : RepositoryBase<Product_Input_History>, IProductInputHistoryRepository
    {

        public ProductInputHistoryRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

    }
}

