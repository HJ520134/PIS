using PDMS.Data.Infrastructure;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_RestRepository : IRepository<GL_Rest>
    {
        List<GL_RestTimeDTO> GetRestTimeList(RestTimeQueryViewModel search, out int count);
        GL_RestTimeDTO GetRestById(int ShiftTimeID);

    }
    public class GL_RestRepository : RepositoryBase<GL_Rest>, IGL_RestRepository
    {
        public GL_RestRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
        public GL_RestTimeDTO GetRestById(int ShiftTimeID)
        {
            var query = from Rest in DataContext.GL_Rest
                        where Rest.RestID == ShiftTimeID
                        select new GL_RestTimeDTO
                        {
                            RestID = Rest.RestID,
                            ShiftTimeID = Rest.ShiftTimeID,
                            Plant_Organization_UID = Rest.GL_ShiftTime.Plant_Organization_UID,
                            BG_Organization_UID = Rest.GL_ShiftTime.BG_Organization_UID,
                            Shift = Rest.GL_ShiftTime.Shift,
                            SEQ = Rest.SEQ,
                            StartTime = Rest.StartTime.ToString().Substring(0, 5),
                            EndTime = Rest.EndTime.ToString().Substring(0, 5),
                            Plant_Organization_Name = Rest.GL_ShiftTime.System_Organization.Organization_Name,
                            BG_Organization_Name = Rest.GL_ShiftTime.System_Organization1.Organization_Name,
                            Modified_UserName = Rest.System_Users.User_Name,
                            Modified_Time = Rest.Modified_Time
                        };
            return query.FirstOrDefault();
        }

        public List<GL_RestTimeDTO> GetRestTimeList(RestTimeQueryViewModel search, out int count)
        {
            List<GL_RestTimeDTO> Resalt = new List<GL_RestTimeDTO>();
            var query = from Rest in DataContext.GL_Rest
                        where Rest.ShiftTimeID == search.ShiftID
                        orderby Rest.SEQ
                        select new GL_RestTimeDTO
                        {
                            RestID=Rest.RestID,
                            ShiftTimeID = Rest.ShiftTimeID,
                            Plant_Organization_UID = Rest.GL_ShiftTime.Plant_Organization_UID,
                            BG_Organization_UID = Rest.GL_ShiftTime.BG_Organization_UID,
                            Shift = Rest.GL_ShiftTime.Shift,
                            SEQ=Rest.SEQ,
                            StartTime = Rest.StartTime.ToString().Substring(0,5),
                            EndTime = Rest.EndTime.ToString().Substring(0, 5),
                            Plant_Organization_Name = Rest.GL_ShiftTime.System_Organization.Organization_Name,
                            BG_Organization_Name = Rest.GL_ShiftTime.System_Organization1.Organization_Name,
                            Modified_UserName=Rest.System_Users.User_Name,
                            Modified_Time = Rest.Modified_Time
                        };
            Resalt = query.ToList();
            count = Resalt.Count;
            return Resalt;
        }

    }
}

