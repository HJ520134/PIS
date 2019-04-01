using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    public class SystemUserRoleConfiguration : EntityTypeConfiguration<System_User_Role>
    {
        public SystemUserRoleConfiguration()
        {
            HasKey(k => k.System_User_Role_UID);
            HasRequired(p => p.System_Users).WithMany(user => user.System_User_Role).HasForeignKey(p => p.Modified_UID);
        }
    }
}
