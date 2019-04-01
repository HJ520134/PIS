using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Common.Enums
{
    public enum EnumAuthorize
    {
        PageNotAuthorized = 0,
        PageAuthorized = 1,
        NotPageRequest = 2,
        TokenInvalid = 3,
        Success = 4,
        Fail = 5
    }
}
