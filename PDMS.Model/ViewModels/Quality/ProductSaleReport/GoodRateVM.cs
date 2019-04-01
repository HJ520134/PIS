using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class ProductSaleReport_RateVM
    {
        public string Process { set; get; }
        public int Flowchart_Detail_UID { set; get; }
        public string FunPlant { set; get; }
        public string Item { set; get; }

        public string SearchDataRate { set; get; }
        public string OndDayBeforeSearchDateRate { set; get; }
        public string TwoDayBeforeSearchDateRate { set; get; }
        public string ThreeDayBeforeSearchDateRate { set; get; }
        public string FourDayBeforeSearchDateRate { set; get; }

        public string SearchDataRate_VM
        {
            get
            {
                if ("投入数,良品数".Contains(Item))
                {
                    if (!string.IsNullOrEmpty(SearchDataRate))
                    {
                        int Pointindex = SearchDataRate.IndexOf(".");
                        return SearchDataRate.Substring(0, Pointindex);
                    }
                    else
                        return "0";
                }
                else
                {
                    if(SearchDataRate=="-1")
                    {
                        return "-";
                    }
                    else
                        return (Convert.ToDecimal(SearchDataRate) * 100).ToString("F2") + "%";
                }
            }
        }
        public string OndDayBeforeSearchDateRate_VM
        {
            get
            {
                if ("投入数,良品数".Contains(Item))
                {
                    if (!string.IsNullOrEmpty(OndDayBeforeSearchDateRate))
                    {
                        int Pointindex = OndDayBeforeSearchDateRate.IndexOf(".");
                        return OndDayBeforeSearchDateRate.Substring(0, Pointindex);
                    }
                    else
                        return "0";
                }
                else
                {
                    if (OndDayBeforeSearchDateRate == "-1")
                    {
                        return "-";
                    }
                    else
                        return (Convert.ToDecimal(OndDayBeforeSearchDateRate) * 100).ToString("F2") + "%";
                }
            }

        }
        public string TwoDayBeforeSearchDateRate_VM
        {
            get
            {
                if ("投入数,良品数".Contains(Item))
                {
                    if (!string.IsNullOrEmpty(TwoDayBeforeSearchDateRate))
                    {
                        int Pointindex = TwoDayBeforeSearchDateRate.IndexOf(".");
                        return TwoDayBeforeSearchDateRate.Substring(0, Pointindex);
                    }
                    else
                        return "0";
                }
                else
                {
                    if (TwoDayBeforeSearchDateRate == "-1")
                    {
                        return "-";
                    }
                    else
                        return (Convert.ToDecimal(TwoDayBeforeSearchDateRate) * 100).ToString("F2") + "%";
                }
            }
        }
        public string ThreeDayBeforeSearchDateRate_VM
        {
            get
            {
                if ("投入数,良品数".Contains(Item))
                {
                    if (!string.IsNullOrEmpty(ThreeDayBeforeSearchDateRate))
                    {
                        int Pointindex = ThreeDayBeforeSearchDateRate.IndexOf(".");
                        return ThreeDayBeforeSearchDateRate.Substring(0, Pointindex);
                    }
                    else
                        return "0";
                }
                else
                {
                    if (ThreeDayBeforeSearchDateRate == "-1")
                    {
                        return "-";
                    }
                    else
                        return (Convert.ToDecimal(ThreeDayBeforeSearchDateRate) * 100).ToString("F2") + "%";
                }
            }
        }
        public string FourDayBeforeSearchDateRate_VM
        {
            get
            {
                if ("投入数,良品数".Contains(Item))
                {
                    if (!string.IsNullOrEmpty(FourDayBeforeSearchDateRate))
                    {
                        int Pointindex = FourDayBeforeSearchDateRate.IndexOf(".");
                        return FourDayBeforeSearchDateRate.Substring(0, Pointindex);
                    }
                    else
                        return "0";
                }
                else
                {
                    if (FourDayBeforeSearchDateRate == "-1")
                    {
                        return "-";
                    }
                    else
                        return (Convert.ToDecimal(FourDayBeforeSearchDateRate) * 100).ToString("F2") + "%";
                }
            }
        }
        public int IndexNum { set; get; }



    }
}
