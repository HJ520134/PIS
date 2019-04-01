using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    public class SystemUsersConfiguration: EntityTypeConfiguration<System_Users>
    {
        public SystemUsersConfiguration()
        {
            HasKey(k => k.Account_UID);
        }
    }
}
