using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class PagedListModel<T>
    {
        public int TotalItemCount { get; set; }

        public IEnumerable<T> Items { get; set; }


        public PagedListModel(int Total, IEnumerable<T> items)
        {
            this.Items = items;
            this.TotalItemCount = Total;
        }
    }
}
