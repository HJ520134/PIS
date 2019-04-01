using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.Batch
{
    public class BatchMailVM
    {
        public int System_Email_M_UID { get; set; }
        public int System_Schedule_UID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Email_From { get; set; }
        public string Email_To { get; set; }
        public string Email_To_UIDs { get; set; }

        public string Email_CC { get; set; }
        public string Email_CC_UIDs { get; set; }

        public string Email_BCC { get; set; }
        public string Email_BCC_UIDs { get; set; }

        public DateTime? Reservation_Date { get; set; }
        public bool? Is_Send { get; set; }
        public DateTime? Send_Time { get; set; }
        public int Email_Type { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    }

    public class SystemModuleEmailVM : BaseModel
    {
        public int System_Schedule_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int? BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string PlantName { get; set; }
        public string OpType_Name { get; set; }
        public string Func_Name { get; set; }
        public string Module_ID { get; set; }
        public string Module_Name { get; set; }
        public string Function_ID { get; set; }
        public string Function_Name { get; set; }
        public int System_Email_Delivery_UID { get; set; }
        public int Function_UID { get; set; }
        public string User_Name_CN { get; set; }
        public string User_Name_EN { get; set; }
        public string Email { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public int AccountUID { get; set; }
        public int Account_UID { get; set; }
        public string User_NTID { get; set; }
        public bool IsAdmin { get; set; }
        public int RoleUID { get; set; }
        public List<int> RoleUIDList { get; set; }
        public bool IsEdit { get; set; }
    }

    public class SystemModuleEmailFunctionVM
    {
        public int Plant_Organization_UID { get; set; }
        public string Plant_Name { get; set; }
        public int Function_UID { get; set; }
        public string Function_ID { get; set; }
        public string Function_Name { get; set; }
        public int System_Schedule_UID { get; set; }
    }
}
