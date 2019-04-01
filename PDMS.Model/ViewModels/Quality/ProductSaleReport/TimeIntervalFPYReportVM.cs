using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class TimeIntervalFPYReportVM
    {
        public string Process { set; get; }
        public int Process_Seq { set;get;}
        public string TargetRate { set; get; }
        public string First_TimeInterVal_Rate { set; get; }
        public string Second_TimeInterval_Rate { set; get; }
        public string Third_TimeInterval_Rate { set; get; }
        public string Fourth_TimeInterval_Rate { set; get; }
        public string Fifth_TimeInterval_Rate { set; get; }
        public string Sixth_TimeInterval_Rate { set; get; }
        public string Seventh_TimeInterval_Rate { set; get;}
        public string Eigthth_TimeInterval_Rate { set; get; }
        public string Ninth_TimeInterval_Rate { set; get; }
        public string Tenth_TimeInterval_Rate { set; get; }
        public string Eleventh_TimeInterval_Rate { set; get; }
        public string Twelfth_TimeInterval_Rate { set; get; }

        public string TargetRate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(TargetRate))
                {
                    return (Convert.ToDecimal(TargetRate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string First_TimeInterVal_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(First_TimeInterVal_Rate))
                {
                    if (First_TimeInterVal_Rate.Equals("-"))
                    {
                        return First_TimeInterVal_Rate;
                    }
                    else
                        return (Convert.ToDecimal(First_TimeInterVal_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string Second_TimeInterval_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(Second_TimeInterval_Rate))
                {
                    if (Second_TimeInterval_Rate.Equals("-"))
                    {
                        return Second_TimeInterval_Rate;
                    }
                    else
                        return (Convert.ToDecimal(Second_TimeInterval_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string Third_TimeInterval_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(Third_TimeInterval_Rate))
                {
                    if (Third_TimeInterval_Rate.Equals("-"))
                    {
                        return Third_TimeInterval_Rate;
                    }
                    else
                        return (Convert.ToDecimal(Third_TimeInterval_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string Fourth_TimeInterval_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(Fourth_TimeInterval_Rate))
                {
                    if (Fourth_TimeInterval_Rate.Equals("-"))
                    {
                        return Fourth_TimeInterval_Rate;
                    }
                    else
                        return (Convert.ToDecimal(Fourth_TimeInterval_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string Fifth_TimeInterval_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(Fifth_TimeInterval_Rate))
                {
                    if (Fifth_TimeInterval_Rate.Equals("-"))
                    {
                        return Fifth_TimeInterval_Rate;
                    }
                    else
                        return (Convert.ToDecimal(Fifth_TimeInterval_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string Sixth_TimeInterval_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(Sixth_TimeInterval_Rate))
                {
                    if (Sixth_TimeInterval_Rate.Equals("-"))
                    {
                        return Sixth_TimeInterval_Rate;
                    }
                    else
                        return (Convert.ToDecimal(Sixth_TimeInterval_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string Seventh_TimeInterval_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(Seventh_TimeInterval_Rate))
                {
                    if (Seventh_TimeInterval_Rate.Equals("-"))
                    {
                        return Seventh_TimeInterval_Rate;
                    }
                    else
                        return (Convert.ToDecimal(Seventh_TimeInterval_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string Eigthth_TimeInterval_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(Eigthth_TimeInterval_Rate))
                {
                    if (Eigthth_TimeInterval_Rate.Equals("-"))
                    {
                        return Eigthth_TimeInterval_Rate;
                    }
                    else
                        return (Convert.ToDecimal(Eigthth_TimeInterval_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string Ninth_TimeInterval_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(Ninth_TimeInterval_Rate))
                {
                    if (Ninth_TimeInterval_Rate.Equals("-"))
                    {
                        return Ninth_TimeInterval_Rate;
                    }
                    else
                        return (Convert.ToDecimal(Ninth_TimeInterval_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string Tenth_TimeInterval_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(Tenth_TimeInterval_Rate))
                {
                    if (Tenth_TimeInterval_Rate.Equals("-"))
                    {
                        return Tenth_TimeInterval_Rate;
                    }
                    else
                        return (Convert.ToDecimal(Tenth_TimeInterval_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string Eleventh_TimeInterval_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(Eleventh_TimeInterval_Rate))
                {
                    if (Eleventh_TimeInterval_Rate.Equals("-"))
                    {
                        return Eleventh_TimeInterval_Rate;
                    }
                    else
                        return (Convert.ToDecimal(Eleventh_TimeInterval_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }
        public string Twelfth_TimeInterval_Rate_VM
        {
            get
            {
                if (!string.IsNullOrEmpty(Twelfth_TimeInterval_Rate))
                {
                    if (Twelfth_TimeInterval_Rate.Equals("-"))
                    {
                        return Twelfth_TimeInterval_Rate;
                    }
                    else
                        return (Convert.ToDecimal(Twelfth_TimeInterval_Rate) * 100).ToString("F2") + "%";
                }
                else
                    return "-";
            }
        }

    }
}
