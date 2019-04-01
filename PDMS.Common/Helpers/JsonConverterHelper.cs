using Newtonsoft.Json.Converters;
using PDMS.Common.Constants;
using System.Data;
using System.Text;

namespace PDMS.Common.Helpers
{
    public class SPPDateTimeConverter : IsoDateTimeConverter
    {
        public SPPDateTimeConverter()
        {
            base.DateTimeFormat = FormatConstants.DateTimeFormatString;
        }
    }

    public class SPPDateTimeConverterToDate : IsoDateTimeConverter
    {
        public SPPDateTimeConverterToDate()
        {
            base.DateTimeFormat = FormatConstants.DateTimeFormatStringByDate;
        }
    }
    public class SPPDateTimeConverterToDateByMin : IsoDateTimeConverter
    {
        public SPPDateTimeConverterToDateByMin()
        {
            base.DateTimeFormat = FormatConstants.DateTimeFormatStringByMin;
        }
    }
    #region dataTable转换成Json格式
    public class DataTableConvertToJson
    {
        
        /// <summary>  
        /// dataTable转换成Json格式  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static string DataTable2Json(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"");
            jsonBuilder.Append(dt.TableName);
            jsonBuilder.Append("\":[");
           // jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
    }
    #endregion
}
