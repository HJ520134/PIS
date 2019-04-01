using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    class SystemOrganizationConfiguration : EntityTypeConfiguration<System_Organization>
    {
        public SystemOrganizationConfiguration()
        {
            HasKey(k => k.Organization_UID);
        }
    }
}
