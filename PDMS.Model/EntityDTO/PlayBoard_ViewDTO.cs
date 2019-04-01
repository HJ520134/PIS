using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class PlayBoard_ViewDTO : EntityDTOBase
    {
        public int PlayBoard_View_ID { get; set; }
        public string Name { get; set; }
        public string ActionName { get; set; }
        public string SettingActionName { get; set; }
        public string Desc { get; set; }
        public string ParameterDesc { get; set; }
        public bool IsJsonParameterNeed { get; set; }
        public bool IsEnabled { get; set; }
        public int Created_UID { get; set; }
        public string Title { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Created_Date { get; set; }

        //自定义栏位
        public string Created_UserName { get; set; }
    }
}
