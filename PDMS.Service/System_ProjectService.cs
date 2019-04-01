using PDMS.Data;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface ISystem_ProjectService : IBaseSevice<System_Project, SystemProjectDTO, System_ProjectModelSearch>
    {
    }
    public class System_ProjectService : BaseSevice<System_Project, SystemProjectDTO, System_ProjectModelSearch>, ISystem_ProjectService
    {
        private readonly ISystemProjectRepository systemProjectRepository;
        public System_ProjectService(ISystemProjectRepository customerRepository) : base(customerRepository)
        {
            this.systemProjectRepository = customerRepository;
        }
    }
}
