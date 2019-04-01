using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    public class SystemRoleFunctionSubConfiguration : EntityTypeConfiguration<System_Role_FunctionSub>
    {
        public SystemRoleFunctionSubConfiguration()
        {
            HasKey(k => k.System_Role_FunctionSub_UID);
        }
    }
}
