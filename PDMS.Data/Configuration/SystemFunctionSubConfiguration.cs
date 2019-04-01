using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    public class SystemFunctionSubConfiguration : EntityTypeConfiguration<System_FunctionSub>
    {
        public SystemFunctionSubConfiguration()
        {
            HasKey(k => k.System_FunctionSub_UID);
        }
    }
}
