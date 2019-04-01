using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    class SystemUserPlantConfiguration : EntityTypeConfiguration<System_User_Plant>
    {
        public SystemUserPlantConfiguration()
        {
            HasKey(k => k.System_User_Plant_UID);
        }
    }
}
