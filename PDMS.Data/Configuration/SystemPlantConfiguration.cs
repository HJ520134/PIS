using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    class SystemPlantConfiguration : EntityTypeConfiguration<System_Plant>
    {
        public SystemPlantConfiguration()
        {
            HasKey(k => k.System_Plant_UID);
        }
    }
}
