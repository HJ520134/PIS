using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Infrastructure
{
    public interface IDatabaseFactory : IDisposable
    {
        SPPContext Get();
    }
}
