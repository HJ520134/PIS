using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class ModelLineHRModelSearch : BaseModel
    {
        public int? ModelLineHR_UID { get; set; }
        public string Station { get; set; }
        public int? Total { get; set; }
        public int? ShouldCome { get; set; }
        public int? ActualCome { get; set; }
        public int? VacationLeave { get; set; }
        public int? PersonalLeave { get; set; }
        public int? SickLeave { get; set; }
        public int? AbsentLeave { get; set; }
        public DateTime? Created_Date { get; set; }
        public DateTime? Modified_Date { get; set; }
    }
}