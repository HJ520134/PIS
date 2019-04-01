﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_MeetingTypeInfoDTO : BaseModel
    {
        public int MeetingType_UID { get; set; }
        public string Plant_Organization_Name { get; set; }
        public int Plant_Organization_UID { get; set; }
        public string BG_Organization_Name { get; set; }
        public int BG_Organization_UID { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public string MeetingType_ID { get; set; }
        public string MeetingType_Name { get; set; }
        public int Modified_UID { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
    }

    public class OEE_MetricInfoDTO : BaseModel
    {
        public int Metric_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public string Plant_Organization_Name { get; set; }
        public int BG_Organization_UID { get; set; }
        public string BG_Organization_Name { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string Metric_ID { get; set; }
        public string Metric_Name { get; set; }
        public int Modified_UID { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
    }

    public class OEE_ImprovementPlanDTO : BaseModel
    {
        public int ImprovementPlan_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public string Plant_Organization_Name { get; set; }

        public int BG_Organization_UID { get; set; }
        public string BG_Organization_Name { get; set; }

        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string FunPlant_Organization_Name { get; set; }

        public int Project_UID { get; set; }
        public string ProjectName { get; set; }

        public Nullable<int> LineID { get; set; }
        public string LineName { get; set; }
        public Nullable<int> StationID { get; set; }
        public string StationName { get; set; }
        public Nullable<int> OEE_MachineInfo_UID { get; set; }
        public string MachineName { get; set; }
        public int MeetingType_UID { get; set; }
        public string ImprovementPlan_ID { get; set; }
        public string ImprovementPlan_Name { get; set; }
        public string Audience { get; set; }
        public string Responsible { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> Commit_Date { get; set; }
        public Nullable<System.DateTime> Due_Date { get; set; }
        public Nullable<System.DateTime> Close_Date { get; set; }
        public Nullable<System.DateTime> DirDueDate { get; set; }
        public int Created_UID { get; set; }
        public DateTime? Created_Date { get; set; }
        public string Problem_Description { get; set; }
        public string Root_Cause { get; set; }
        public string CACP_Description { get; set; }
        public string Comment { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public string Attachment1 { get; set; }
        public string Attachment6 { get; set; }
        public string Attachment2 { get; set; }
        public string Attachment4 { get; set; }
        public string Attachment5 { get; set; }

        public  List<OEE_ImprovementPlanDDTO> OEE_ImprovementPlanDetailList { get; set; }
    }

    public class OEE_ImprovementPlanDDTO : BaseModel
    {
        public int ImprovementPlanD_UID { get; set; }
        public int ImprovementPlan_UID { get; set; }
        public int Metric_UID { get; set; }
        public string Metric_ID { get; set; }
        public string Metric_Name { get; set; }
    }

    public class OEE_RealStatusDTO 
    {
        public int MachineID { get; set; }
        public string MachineNO { get; set; }
        public int StatusID { get; set; }
        public DateTime  StartTime { get; set; }
        public DateTime EndTime { get; set; }
   
    }

    public class OEE_RealStatusReport
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<OEE_RealStatusDTO> OEE_RealStatusList { get; set; }
    }

}