using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Configuration;

namespace PDMS.Data
{
    public class SPPEntityConfiguration
    {
        public static void SPPOnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new SystemUsersConfiguration());
            modelBuilder.Configurations.Add(new SystemRoleConfiguration());
            modelBuilder.Configurations.Add(new SystemUserRoleConfiguration());
            modelBuilder.Configurations.Add(new SystemFunctionConfiguration());
            modelBuilder.Configurations.Add(new SystemFunctionSubConfiguration());
            modelBuilder.Configurations.Add(new SystemRoleFunctionConfiguration());
            modelBuilder.Configurations.Add(new SystemRoleFunctionSubConfiguration());
            modelBuilder.Configurations.Add(new SystemBUMConfiguration());
            modelBuilder.Configurations.Add(new SystemBUDConfiguration());
            modelBuilder.Configurations.Add(new WarningListConfiguration());
        }

    }
}
