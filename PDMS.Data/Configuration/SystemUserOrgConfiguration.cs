using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    class SystemUserOrgConfiguration: EntityTypeConfiguration<System_UserOrg>
    {
        public SystemUserOrgConfiguration()
        {
            HasKey(k => k.System_UserOrgUID);
        }
    }
}
