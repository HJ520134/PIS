using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    class SystemOrganizationBOMConfiguration : EntityTypeConfiguration<System_OrganizationBOM>
    {
        public SystemOrganizationBOMConfiguration()
        {
            HasKey(k => k.OrganizationBOM_UID);
        }
    }
}
