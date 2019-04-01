using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class PlayBoard_SettingDTO : EntityDTOBase
    {
        public int PlayBoard_Setting_ID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int PlayBoard_View_ID { get; set; }
        public int PlaySeq { get; set; }
        public string JsonParameter { get; set; }
        public string Remark { get; set; }
        public bool IsTiming { get; set; }
        public bool IsEnabled { get; set; }
        public int Play_UID { get; set; }
        public string Title { get; set; }
        public int Created_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public System.DateTime Created_Date { get; set; }

        //自定义栏位
        public string PlantName { get; set; }
        public string BGName { get; set; }
        public string FunPlantName { get; set; }
        public string Project_Name { get; set; }
        public string ViewName { get; set; }
        public string SettingActionName { get; set; }
        public string ActionName { get; set; }
        public bool IsJsonParameterNeed { get; set; }
        public string Created_UserName { get; set; }
        public string Play_UserNTID { get; set; }
        public string Play_UserName { get; set; }
        //播放时间,如 08:30~10:30;13:30~23:50
        public string PlayTime { get; set; }
        public List<PlayBoard_PlayTimeDTO> PlayBoard_PlayTimeDTOList { get; set; }
        public string CurrentPlayTime { get; set; }

        //提示上一个播放画面
        public string LastViewTitle { get; set; }

        //提示下一个播放画面
        public string NextViewTiitle { get; set; }
    }
}
