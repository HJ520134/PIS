using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class DynamicColumnModel<T>
    {
        public string DynamicColumn { get; set; }

        public IEnumerable<T> Items { get; set; }

        public DynamicColumnModel(string columnName, IEnumerable<T> items)
        {
            this.Items = items;
            this.DynamicColumn = columnName;
        }
    }
}
