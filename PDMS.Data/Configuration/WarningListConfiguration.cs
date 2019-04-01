using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Configuration
{

    public class WarningListConfiguration : EntityTypeConfiguration<Warning_List>
    {
        public WarningListConfiguration()
        {
            HasKey(k => k.Warning_UID);
        }
    }
}
