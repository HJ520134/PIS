using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    public class SystemRoleConfiguration : EntityTypeConfiguration<System_Role>
    {
        public SystemRoleConfiguration()
        {
            HasKey(k => k.Role_UID);
        }
    }
}
