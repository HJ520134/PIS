using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class CurrentUserDataPermission : BaseModel
    {
        //当前用户所拥有的OP类型
        public List<string> Op_Types { get; set; }
        //当前用户所拥有的Project_UID，可与专案表链接查询相关信息
        public List<int> Project_UID { get; set; }
        //当前用户所拥有的FlowChart_Detail_UID
        public List<ProcessInfo> ProcessInfo { get; set; }
        //当前用所在组织
        public UserOrgInfo UserOrgInfo { get; set; }
    }

    public class ProcessInfo : BaseModel
    {
        public int FlowChart_Detail_UID { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public string Process { get; set; }
        public string Place { get; set; }
    }

    public class UserOrgInfo : BaseModel
    {
        List<OrganiztionVM> Orgnizations = new List<OrganiztionVM>();
    }
}
