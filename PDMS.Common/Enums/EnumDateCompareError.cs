using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Common
{

    public enum EnumDateCompareError
    {
        Pass,
        NoNeedVerify_SubStartAndEndDateNull,
        NoNeedVerify_SubEndLowerThanSubStartDate,
        NoNeedVerify_HeadEndLowerThanHeadStartDate,
        SubEndLowerThanHeadEndDate,
        SubStartLowerThanHeadStartDate,
        SubStartIsNullHeadStartNotNull,
        SubEndIsNullHeadEndNotNull,
        SubEndLowerThanHeadEndDate_And_SubStartLowerThanHeadStartDate
    }
}
