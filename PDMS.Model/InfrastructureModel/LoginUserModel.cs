using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class LoginUserMoel: BaseModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int IsEmployee { get; set; }
    }
}
