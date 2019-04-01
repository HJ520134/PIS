using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    class SystemUserBusinessGroupConfiguration : EntityTypeConfiguration<System_User_Business_Group>
    {
        public SystemUserBusinessGroupConfiguration()
        {
            HasKey(k => k.System_User_BU_UID);
        }
    }
}
