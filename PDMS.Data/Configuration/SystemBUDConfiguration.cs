using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    class SystemBUDConfiguration : EntityTypeConfiguration<System_BU_D>
    {
        public SystemBUDConfiguration()
        {
            HasKey(k => k.BU_D_UID);

            //HasRequired(p => p.Modified_User).WithMany(user => user.System_BU_D).HasForeignKey(fk => fk.Modified_UID);
        }
    }
}
