using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PDMS.Common.Helpers;

namespace PDMS.Data.Repository
{
    public interface IProduct_Input_LocationRepository : IRepository<Product_Input_Location>
    {
     

    }
    public class Product_Input_LocationRepository : RepositoryBase<Product_Input_Location>, IProduct_Input_LocationRepository
    {
        private Logger log = new Logger("NoticeRepository");
        public Product_Input_LocationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

    
    }



}
