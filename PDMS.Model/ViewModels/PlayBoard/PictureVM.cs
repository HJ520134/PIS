using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class PictureVM
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime LastWriteTime { get; set; }
        public string SizeMB { get; set; }
    }
}
