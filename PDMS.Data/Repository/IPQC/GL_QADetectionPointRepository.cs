using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_QADetectionPointRepository : IRepository<GL_QADetectionPoint>
    {
        List<GL_QADetectionPointDTO> GetGLQADetectionPoint(GLQAReportVM model, Page page);
        GL_QADetectionPointDTO GetGLQADetectionPointByID(int QADetectionPointID);
        GL_QADetectionPointDTO GetStationsDetectionPointByID(int StationID);
        GL_LineDTO GetOneLineDTO(int LineID);
        List<GL_QADetectionPointDTO> GetQADetectionPointDTOList(int CustomerID);

        List<GL_QADetectionPointDTO> GetGL_QADetectionPointDTO(int CustomerID);
        string InserOrUpdateDetectionPoints(List<GL_QADetectionPointDTO> list);
        List<GL_QADetectionPointDTO> GetAllGLQADetectionPointDTO();
    }
    public class GL_QADetectionPointRepository : RepositoryBase<GL_QADetectionPoint>, IGL_QADetectionPointRepository
    {
        public GL_QADetectionPointRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<GL_QADetectionPointDTO> GetGLQADetectionPoint(GLQAReportVM model, Page page)
        {
            var query = from M in DataContext.GL_QADetectionPoint
                        select new GL_QADetectionPointDTO
                        {
                            QADetectionPointID = M.QADetectionPointID,
                            WIP = M.WIP,
                            ScanIN = M.ScanIN,
                            ScanOUT = M.ScanOUT,
                            ScanNG = M.ScanNG,
                            ScanBACK = M.ScanBACK,
                            IsEnabled = M.IsEnabled,
                            StationID = M.StationID,
                            StationName = M.GL_Station.StationName,
                            Modified_UID = M.System_Users.Account_UID,
                            Modified_Date = M.Modified_Date,
                            Modifieder = M.System_Users.User_Name
                        };

            if (model.StationID != 0)
            {
                query = query.Where(m => m.StationID == model.StationID);
            }
            query = query.Where(m => m.IsEnabled == true);
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query.ToList();
        }

        public GL_QADetectionPointDTO GetGLQADetectionPointByID(int QADetectionPointID)
        {
            var query = from M in DataContext.GL_QADetectionPoint
                        select new GL_QADetectionPointDTO
                        {
                            QADetectionPointID = M.QADetectionPointID,
                            WIP = M.WIP,
                            ScanIN = M.ScanIN,
                            ScanOUT = M.ScanOUT,
                            ScanNG = M.ScanNG,
                            ScanBACK = M.ScanBACK,
                            IsEnabled = M.IsEnabled,
                            StationID = M.StationID,
                            StationName = M.GL_Station.StationName,
                            Modified_UID = M.System_Users.Account_UID,
                            Modified_Date = M.Modified_Date,
                            Modifieder = M.System_Users.User_Name
                        };
            query = query.Where(m => m.QADetectionPointID == QADetectionPointID);
            return query.FirstOrDefault();
        }

        public GL_QADetectionPointDTO GetStationsDetectionPointByID(int StationID)
        {
            var query = from M in DataContext.GL_QADetectionPoint
                        select new GL_QADetectionPointDTO
                        {
                            QADetectionPointID = M.QADetectionPointID,
                            WIP = M.WIP,
                            ScanIN = M.ScanIN,
                            ScanOUT = M.ScanOUT,
                            ScanNG = M.ScanNG,
                            ScanBACK = M.ScanBACK,
                            IsEnabled = M.IsEnabled,
                            StationID = M.StationID,
                            StationName = M.GL_Station.StationName,
                            Modified_UID = M.System_Users.Account_UID,
                            Modified_Date = M.Modified_Date,
                            Modifieder = M.System_Users.User_Name
                        };
            query = query.Where(m => m.StationID == StationID);
            return query.FirstOrDefault();
        }

        public GL_LineDTO GetOneLineDTO(int LineID)
        {
            var query = from w in DataContext.GL_Line
                        select new GL_LineDTO
                        {

                            LineID = w.LineID,
                            LineName = w.LineName,
                            CustomerID = w.CustomerID,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            Modified_UID = w.Modified_UID,
                            Modified_Date = w.Modified_Date,
                            CycleTime = w.CycleTime,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            Plant_Organization = w.System_Organization.Organization_Name,
                            BG_Organization = w.System_Organization1.Organization_Name,
                            ProjectName = w.System_Project.Project_Name,
                            MESProjectName = w.System_Project.MESProject_Name
                        };

            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.LineID == LineID);
            return query.FirstOrDefault();


        }

        public List<GL_QADetectionPointDTO> GetQADetectionPointDTOList(int CustomerID)
        {

            var query = from w in DataContext.GL_Station
                        where w.IsTest == true && w.GL_Line.IsEnabled == true && w.IsEnabled == true
                        select new GL_QADetectionPointDTO
                        {
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            CustomerID = w.GL_Line.CustomerID,
                            LineID = w.LineID,
                            LineName = w.GL_Line.LineName,
                            StationID = w.StationID,
                            StationName = w.StationName,
                        };
            return query.ToList();
        }

        public List<GL_QADetectionPointDTO> GetGL_QADetectionPointDTO(int CustomerID)
        {
            var query = from M in DataContext.GL_QADetectionPoint
                        select new GL_QADetectionPointDTO
                        {
                            QADetectionPointID = M.QADetectionPointID,
                            WIP = M.WIP,
                            ScanIN = M.ScanIN,
                            ScanOUT = M.ScanOUT,
                            ScanNG = M.ScanNG,
                            ScanBACK = M.ScanBACK,
                            IsEnabled = M.IsEnabled,
                            //StationID = M.StationID,
                            //StationName = M.GL_Station.StationName,
                            Modified_UID = M.System_Users.Account_UID,
                            Modified_Date = M.Modified_Date,
                            Modifieder = M.System_Users.User_Name,
                            ProjectName = M.GL_Station.GL_Line.System_Project.Project_Name,
                            CustomerID = M.GL_Station.GL_Line.CustomerID,
                            LineID = M.GL_Station.LineID,
                            LineName = M.GL_Station.GL_Line.LineName,
                            StationID = M.StationID,
                            StationName = M.GL_Station.StationName,
                        };
            query = query.Where(m => m.CustomerID == CustomerID);
            return query.ToList();

        }

        public string InserOrUpdateDetectionPoints(List<GL_QADetectionPointDTO> list)
        {


            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    if (list != null && list.Count > 0)
                    {

                        foreach (var item in list)
                        {

                            if (item.QADetectionPointID == 0)
                            {
                                //构造插入SQL数据
                                var insertSql = string.Format(@" INSERT INTO GL_QADetectionPoint
                                                                           (
                                                                           StationID
                                                                           ,WIP
                                                                           ,ScanIN
                                                                           ,ScanOUT
                                                                           ,ScanNG
                                                                           ,ScanBACK
                                                                           ,IsEnabled
                                                                           ,Modified_UID
                                                                           ,Modified_Date)
                                                                     VALUES
                                                                           (
                                                                            {0}
                                                                           ,{1}
                                                                           ,N'{2}'
                                                                           ,N'{3}'
                                                                           ,N'{4}'
                                                                           ,N'{5}'
                                                                           ,{6}
                                                                           ,{7}
                                                                           ,'{8}') ;",
                                                               item.StationID, item.WIP, item.ScanIN, item.ScanOUT, item.ScanNG, item.ScanBACK, item.IsEnabled ? 1 : 0, item.Modified_UID, item.Modified_Date);
                                sb.AppendLine(insertSql);
                            }
                            else
                            {

                                var updateSql = string.Format(@" UPDATE GL_QADetectionPoint
                                                                       SET 
                                                                          StationID ={0}
                                                                          ,WIP ={1}
                                                                          ,ScanIN =N'{2}'
                                                                          ,ScanOUT =N'{3}'
                                                                          ,ScanNG = N'{4}'
                                                                          ,ScanBACK= N'{5}'
                                                                          ,IsEnabled = {6}
                                                                          ,Modified_UID = {7}
                                                                          ,Modified_Date = '{8}'
                                                                     WHERE QADetectionPointID={9} ;",
                                                           item.StationID, item.WIP, item.ScanIN, item.ScanOUT, item.ScanNG, item.ScanBACK, item.IsEnabled ? 1 : 0, item.Modified_UID, item.Modified_Date, item.QADetectionPointID);

                                sb.AppendLine(updateSql);

                            }
                        }
                        string sql = sb.ToString();
                        DataContext.Database.ExecuteSqlCommand(sql);
                        trans.Commit();

                    }
                }

                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public List<GL_QADetectionPointDTO> GetAllGLQADetectionPointDTO()
        {
            var query = from M in DataContext.GL_QADetectionPoint
                        select new GL_QADetectionPointDTO
                        {
                            QADetectionPointID = M.QADetectionPointID,
                            WIP = M.WIP,
                            ScanIN = M.ScanIN,
                            ScanOUT = M.ScanOUT,
                            ScanNG = M.ScanNG,
                            ScanBACK = M.ScanBACK,
                            IsEnabled = M.IsEnabled,
                            Modified_UID = M.System_Users.Account_UID,
                            Modified_Date = M.Modified_Date,
                            Modifieder = M.System_Users.User_Name,
                            ProjectName = M.GL_Station.GL_Line.System_Project.Project_Name,
                            CustomerID = M.GL_Station.GL_Line.CustomerID,
                            LineID = M.GL_Station.LineID,
                            LineName = M.GL_Station.GL_Line.LineName,
                            StationID = M.StationID,
                            StationName = M.GL_Station.StationName,
                        };
            query = query.Where(m => m.IsEnabled == true);
            return query.ToList();
        }
    }
}
