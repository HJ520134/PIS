using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{
    public class SystemFunctionConfiguration : EntityTypeConfiguration<System_Function>
    {
        public SystemFunctionConfiguration()
        {
            HasKey(k => k.Function_UID);
        }
    }
}
