using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IMachine_CustomerRepository : IRepository<Machine_Customer>
    {
      
    }
    public class Machine_CustomerRepository : RepositoryBase<Machine_Customer>, IMachine_CustomerRepository
    {
  
        public Machine_CustomerRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
}
