using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    public class SystemRoleFunctionConfiguration : EntityTypeConfiguration<System_Role_Function>
    {
        public SystemRoleFunctionConfiguration()
        {
            HasKey(k => k.System_Role_Function_UID);
        }
    }
}
