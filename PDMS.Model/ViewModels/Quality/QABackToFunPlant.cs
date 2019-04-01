using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class QABackToFunPlant : BaseModel
    {
        public string ExceptionTypeName { set; get; }
        public int ExceptionType_UID { set; get; }
        public decimal RejectionRate { set; get; }
        public string RejectionRateVM
        {
            get
            {
                if (RejectionRate != 0)
                {
                    return (Convert.ToDecimal(RejectionRate) * 100).ToString("F2") + "%";
                }
                else
                {
                    return "";
                }
            }
        }

        public Nullable<decimal> CNC_Rate { set; get; }
        public string CNC_RateVM
        {
            get
            {
                if(CNC_Rate!=null)
                {
                    return (Convert.ToDecimal(CNC_Rate) * 100).ToString("F2") + "%";
                }
                else
                {
                    return "";
                }
            }
        }

        public Nullable<decimal> Surface_Rate { set; get; }
        public string Surface_RateVM
        {
            get
            {
                if (Surface_Rate != null)
                {
                    return (Convert.ToDecimal(Surface_Rate) * 100).ToString("F2") + "%";
                }
                else
                {
                    return "";
                }
            }
        }

        public Nullable<decimal> Assemble_Rate { set; get; }
        public string Assemble_RateVM
        {
            get
            {
                if (Assemble_Rate != null)
                {
                    return (Convert.ToDecimal(Assemble_Rate) * 100).ToString("F2") + "%";
                }
                else
                {
                    return "";
                }
            }
        }

        public Nullable<decimal> OQC_Rate { set; get; }
        public string OQC_RateVM
        {
            get
            {
                if (OQC_Rate != null)
                {
                    return (Convert.ToDecimal(OQC_Rate) * 100).ToString("F2") + "%";
                }
                else
                {
                    return "";
                }
            }
        }

        public Nullable<decimal> Anode_Rate { set; get; }
        public string Anode_RateVM
        {
            get
            {
                if (Anode_Rate != null)
                {
                    return (Convert.ToDecimal(Anode_Rate) * 100).ToString("F2") + "%";
                }
                else
                {
                    return "";
                }
            }
        }

        public DateTime ProductDate { set; get; }
        public string Color { set; get; }

        public int Flowchart_Master_UID { set; get; }
        public int FlowChart_Detail_UID { set; get; }
        public int System_FunPlant_UID { set; get; }
        public Guid QualityAssurance_DistributeRate_UID { set; get; }
        public bool CanModify { set; get; }
        public int RateType { set; get; }
        public string MaterialType { set; get; }

    }

    public class QABackToFunPlantListVM : BaseModel
    {
        public List<QABackToFunPlant> DataList = new List<QABackToFunPlant>();
    }
}
