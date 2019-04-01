using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using PDMS.Common.Constants;

namespace PDMS.Common.Helpers
{
    public class JsonConverterHelperByYMD : IsoDateTimeConverter
    {
        public JsonConverterHelperByYMD()
        {
            base.DateTimeFormat = FormatConstants.DateTimeFormatStringByDate;
        }
    }
}
