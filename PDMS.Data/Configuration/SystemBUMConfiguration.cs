using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    class SystemBUMConfiguration : EntityTypeConfiguration<System_BU_M>
    {
        public SystemBUMConfiguration()
        {
            HasKey(k => k.BU_M_UID);
        }
    }
}
