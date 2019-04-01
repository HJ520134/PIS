using PDMS.Core.Authentication;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PDMS.Core
{
    [SPPAPIAuthentication]
    //API跨越资源分配
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public abstract class ApiControllerBase : ApiController
    {

    }
}
