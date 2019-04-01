using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_LineShiftResposibleUserDTO : EntityDTOBase
    {
        public int GL_LineShiftResposibleUserID { get; set; }
        public int LineID { get; set; }
        public int ShiftTimeID { get; set; }
        public int User_UID { get; set; }

        public GL_LineDTO GL_Line { get; set; }
        public virtual SystemUserDTO ResponsibleUser { get; set; }
        public GL_ShiftTimeDTO GL_ShiftTime { get; set; }
    }
}
