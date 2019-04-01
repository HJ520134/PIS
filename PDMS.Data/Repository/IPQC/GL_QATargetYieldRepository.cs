using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_QATargetYieldRepository : IRepository<GL_QATargetYield>
    {

        IQueryable<GL_QATargetYieldDTO> QueryGLQAYields(GL_QATargetYieldDTO searchModel, Page page, out int totalcount);
        GL_QATargetYieldDTO QueryGLQAYieldByUID(int GLQATargetYieldID);
        GL_StationDTO GetOneStationDTO(int StationID);
        List<GL_QATargetYieldDTO> GetQATargetYieldDTOList(int CustomerID);
        List<GL_QATargetYieldDTO> GetGL_QATargetYieldDTOs(int StationID);
        string InserOrUpdateTargetYields(List<GL_QATargetYieldDTO> list);
        GL_QATargetYieldDTO GetGLQATargetYieldDTO(int StationID, int Tag, string TargetYieldDate);
        string DeleteGLQAYield(int GLQATargetYieldID);

        List<GL_StationDTO> GetIPQCAllStationDTOs();

        List<GL_QATargetYieldDTO> GetGLQATargetYieldDTOList(int StationID, string TargetYieldDate);
        string InserOrUpdateIPQCReports(List<GL_IPQCQualityDetialDTO> gL_IPQCQualityDetialDTOs, List<GL_IPQCQualityReportDTO> gL_IPQCQualityReportDTOs);
        string InserOrUpdateIPQCWIP(int StationID, int WIP);
        
    }
    public class GL_QATargetYieldRepository : RepositoryBase<GL_QATargetYield>, IGL_QATargetYieldRepository
    {
        public GL_QATargetYieldRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IQueryable<GL_QATargetYieldDTO> QueryGLQAYields(GL_QATargetYieldDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.GL_QATargetYield
                        select new GL_QATargetYieldDTO
                        {
                            GLQATargetYieldID = M.GLQATargetYieldID,
                            StationID = M.StationID,
                            TargetYieldDate = M.TargetYieldDate,
                            TargetYield = M.TargetYield,
                            Tag = M.Tag,
                            Modified_UID = M.Modified_UID,
                            Modified_Date = M.Modified_Date,
                            StationName = M.GL_Station.StationName,
                            Modifieder = M.System_Users.User_Name
                        };
            if (searchModel.StationID != 0)
                query = query.Where(m => m.StationID == searchModel.StationID);
            if (searchModel.TargetYield != 0)
                query = query.Where(m => m.TargetYield == searchModel.TargetYield);
            if (searchModel.Tag != 0)
                query = query.Where(m => m.Tag == searchModel.Tag);
            if (!string.IsNullOrWhiteSpace(searchModel.TargetYieldDate))
                query = query.Where(m => m.TargetYieldDate == searchModel.TargetYieldDate);
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;
        }
        public GL_QATargetYieldDTO QueryGLQAYieldByUID(int GLQATargetYieldID)
        {

            var query = from M in DataContext.GL_QATargetYield
                        select new GL_QATargetYieldDTO
                        {
                            GLQATargetYieldID = M.GLQATargetYieldID,
                            StationID = M.StationID,
                            TargetYieldDate = M.TargetYieldDate,
                            TargetYield = M.TargetYield,
                            Tag = M.Tag,
                            Modified_UID = M.Modified_UID,
                            Modified_Date = M.Modified_Date,
                            StationName = M.GL_Station.StationName,
                            Modifieder = M.System_Users.User_Name
                        };
            return query.FirstOrDefault(o => o.GLQATargetYieldID == GLQATargetYieldID);

        }


        public GL_StationDTO GetOneStationDTO(int StationID)
        {
            var query = from w in DataContext.GL_Station
                        select new GL_StationDTO
                        {
                            StationID = w.StationID,
                            StationName = w.StationName,
                            LineID = w.LineID,
                            LineName = w.GL_Line.LineName,
                            CustomerID = w.GL_Line.CustomerID,
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
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name
                        };

            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.StationID == StationID);
            return query.FirstOrDefault();

        }

        public List<GL_QATargetYieldDTO> GetQATargetYieldDTOList(int CustomerID)
        {

            var query = from w in DataContext.GL_Station
                        where w.IsTest == true && w.GL_Line.IsEnabled == true && w.IsEnabled == true
                        select new GL_QATargetYieldDTO
                        {
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            CustomerID = w.GL_Line.CustomerID,
                            LineID = w.LineID,
                            LineName = w.GL_Line.LineName,
                            StationID = w.StationID,
                            StationName = w.StationName,
                        };
            query = query.Where(o => o.CustomerID == CustomerID);
            return query.ToList();
        }

        public List<GL_QATargetYieldDTO> GetGL_QATargetYieldDTOs(int CustomerID)
        {
            var query = from M in DataContext.GL_QATargetYield
                        select new GL_QATargetYieldDTO
                        {
                            GLQATargetYieldID = M.GLQATargetYieldID,
                            StationID = M.StationID,
                            TargetYieldDate = M.TargetYieldDate,
                            TargetYield = M.TargetYield,
                            Tag = M.Tag,
                            Modified_UID = M.Modified_UID,
                            Modified_Date = M.Modified_Date,
                            StationName = M.GL_Station.StationName,
                            Modifieder = M.System_Users.User_Name,
                            CustomerID = M.GL_Station.GL_Line.CustomerID,
                            ProjectName = M.GL_Station.GL_Line.System_Project.Project_Name,
                            LineID = M.GL_Station.LineID,
                            LineName = M.GL_Station.GL_Line.LineName
                        };

            query = query.Where(o => o.CustomerID == CustomerID);
            return query.ToList();

        }

        public string InserOrUpdateTargetYields(List<GL_QATargetYieldDTO> list)
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

                            if (item.GLQATargetYieldID == 0)
                            {
                                //构造插入SQL数据
                                var insertSql = string.Format(@" INSERT INTO GL_QATargetYield
                                                                               (StationID
                                                                               ,TargetYieldDate
                                                                               ,TargetYield
                                                                               ,Tag
                                                                               ,Modified_UID
                                                                               ,Modified_Date)
                                                                         VALUES
                                                                               ({0}
                                                                               ,'{1}'
                                                                               ,{2}
                                                                               ,{3}
                                                                               ,{4}
                                                                               ,'{5}') ;",
                                                                                  item.StationID, item.TargetYieldDate, item.TargetYield, item.Tag, item.Modified_UID, item.Modified_Date);
                                sb.AppendLine(insertSql);
                            }
                            else
                            {

                                var updateSql = string.Format(@" UPDATE GL_QATargetYield
                                                                           SET StationID = {0}
                                                                              ,TargetYieldDate = '{1}'
                                                                              ,TargetYield = {2}
                                                                              ,Tag = {3}
                                                                              ,Modified_UID = {4}
                                                                              ,Modified_Date = '{5}'
                                                                         WHERE GLQATargetYieldID={6} ;",
                                                           item.StationID, item.TargetYieldDate, item.TargetYield, item.Tag, item.Modified_UID, item.Modified_Date, item.GLQATargetYieldID);

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


        public GL_QATargetYieldDTO GetGLQATargetYieldDTO(int StationID, int Tag, string TargetYieldDate)
        {
            var query = from M in DataContext.GL_QATargetYield
                        select new GL_QATargetYieldDTO
                        {
                            GLQATargetYieldID = M.GLQATargetYieldID,
                            StationID = M.StationID,
                            TargetYieldDate = M.TargetYieldDate,
                            TargetYield = M.TargetYield,
                            Tag = M.Tag,
                            Modified_UID = M.Modified_UID,
                            Modified_Date = M.Modified_Date,
                            StationName = M.GL_Station.StationName,
                            Modifieder = M.System_Users.User_Name,
                            CustomerID = M.GL_Station.GL_Line.CustomerID,
                            ProjectName = M.GL_Station.GL_Line.System_Project.Project_Name,
                            LineID = M.GL_Station.LineID,
                            LineName = M.GL_Station.GL_Line.LineName
                        };

            return query.FirstOrDefault(o => o.StationID == StationID && o.Tag == Tag && o.TargetYieldDate == TargetYieldDate);

        }

        public string DeleteGLQAYield(int GLQATargetYieldID)
        {
            try
            {
                string sql = "delete  GL_QATargetYield  where GLQATargetYieldID={0}";
                sql = string.Format(sql, GLQATargetYieldID);
                int result = DataContext.Database.ExecuteSqlCommand(sql);
                if (result == 1)
                    return "";
                else
                    return "删除记录失败！";
            }
            catch (Exception e)
            {
                return "删除记录失败:" + e.Message;
            }
        }



        public List<GL_StationDTO> GetIPQCAllStationDTOs()
        {
            var query = from w in DataContext.GL_Station
                        where w.IsEnabled == true && w.IsTest == true
                        select new GL_StationDTO
                        {
                            StationID = w.StationID,
                            StationName = w.StationName,
                            LineID = w.LineID,
                            LineName = w.GL_Line.LineName,
                            CustomerID = w.GL_Line.CustomerID,
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
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name,
                            MESLineName=w.GL_Line.MESLineName,
                            MESStationName=w.MESStationName
                        };

            return query.ToList();
        }
        public List<GL_QATargetYieldDTO> GetGLQATargetYieldDTOList(int StationID, string TargetYieldDate)
        {
            var query = from M in DataContext.GL_QATargetYield
                        select new GL_QATargetYieldDTO
                        {
                            GLQATargetYieldID = M.GLQATargetYieldID,
                            StationID = M.StationID,
                            TargetYieldDate = M.TargetYieldDate,
                            TargetYield = M.TargetYield,
                            Tag = M.Tag,
                            Modified_UID = M.Modified_UID,
                            Modified_Date = M.Modified_Date,
                            StationName = M.GL_Station.StationName,
                            Modifieder = M.System_Users.User_Name,
                            CustomerID = M.GL_Station.GL_Line.CustomerID,
                            ProjectName = M.GL_Station.GL_Line.System_Project.Project_Name,
                            LineID = M.GL_Station.LineID,
                            LineName = M.GL_Station.GL_Line.LineName
                        };

            query = query.Where(o => o.StationID == StationID && o.TargetYieldDate == TargetYieldDate);
            return query.ToList();


        }



        public string InserOrUpdateIPQCReports(List<GL_IPQCQualityDetialDTO> gL_IPQCQualityDetialDTOs, List<GL_IPQCQualityReportDTO> gL_IPQCQualityReportDTOs)
        {

            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    //机台时段报表 插入 修改
                    if (gL_IPQCQualityReportDTOs != null && gL_IPQCQualityReportDTOs.Count > 0)
                    {

                        foreach (var item in gL_IPQCQualityReportDTOs)
                        {

                            if (item.IPQCQualityReport_UID == 0)
                            {
                                //构造插入SQL数据
                                var insertSql = string.Format(@" INSERT INTO IPQCQualityReport
                                                                               (StationID
                                                                               ,ShiftID
                                                                               ,TimeInterval
                                                                               ,TimeIntervalIndex
                                                                               ,ProductDate
                                                                               ,FirstYield
                                                                               ,FirstTargetYield
                                                                               ,SecondYield
                                                                               ,SecondTargetYield
                                                                               ,InputNumber
                                                                               ,TestNumber
                                                                               ,FirstPassNumber
                                                                               ,SecondPassNumber
                                                                               ,RepairNumber
                                                                               ,NGNumber
                                                                               ,WIP
                                                                               ,ModifyTime)
                                                                         VALUES
                                                                               ({0}
                                                                               ,{1}
                                                                               ,N'{2}'
                                                                               ,{3}
                                                                               ,'{4}'
                                                                               ,{5}
                                                                               ,{6}
                                                                               ,{7}
                                                                               ,{8}
                                                                               ,{9}
                                                                               ,{10}
                                                                               ,{11}
                                                                               ,{12}
                                                                               ,{13}
                                                                               ,{14}
                                                                               ,{15}
                                                                               ,'{16}') ;",
                                                 item.StationID, item.ShiftID, item.TimeInterval, item.TimeIntervalIndex, item.ProductDate, item.FirstYield, item.FirstTargetYield, item.SecondYield,
                                                 item.SecondTargetYield, item.InputNumber, item.TestNumber, item.FirstPassNumber, item.SecondPassNumber, item.RepairNumber, item.NGNumber, item.WIP, item.ModifyTime);

                                sb.AppendLine(insertSql);
                            }
                            else
                            {

                                var updateSql = string.Format(@" UPDATE IPQCQualityReport
                                                                       SET StationID = {0}
                                                                          ,ShiftID = {1}
                                                                          ,TimeInterval = N'{2}'
                                                                          ,TimeIntervalIndex = {3}
                                                                          ,ProductDate = '{4}'
                                                                          ,FirstYield = {5}
                                                                          ,FirstTargetYield = {6}
                                                                          ,SecondYield = {7}
                                                                          ,SecondTargetYield = {8}
                                                                          ,InputNumber = {9}
                                                                          ,TestNumber ={10}
                                                                          ,FirstPassNumber ={11}
                                                                          ,SecondPassNumber = {12}
                                                                          ,RepairNumber = {13}
                                                                          ,NGNumber = {14}
                                                                          ,WIP ={15}
                                                                          ,ModifyTime ='{16}'
                                                                     WHERE IPQCQualityReport_UID={17} ;",
                                         item.StationID, item.ShiftID, item.TimeInterval, item.TimeIntervalIndex, item.ProductDate, item.FirstYield, item.FirstTargetYield, item.SecondYield,
                                         item.SecondTargetYield, item.InputNumber, item.TestNumber, item.FirstPassNumber, item.SecondPassNumber, item.RepairNumber, item.NGNumber, item.WIP,
                                         item.ModifyTime, item.IPQCQualityReport_UID);

                                sb.AppendLine(updateSql);

                            }

                            //if (item.NowDateTime == item.SerchEndTime)
                            //{
                            //    var updateSql = string.Format(@"  UPDATE GL_QADetectionPoint  SET WIP = {0}   WHERE StationID = {1};", item.WIP, item.StationID);
                            //    sb.AppendLine(updateSql);
                            //}
                        }

                    }



                    //机台明细数据 插入 修改
                    if (gL_IPQCQualityDetialDTOs != null && gL_IPQCQualityDetialDTOs.Count > 0)
                    {

                        foreach (var item in gL_IPQCQualityDetialDTOs)
                        {

                            if (item.IPQCQualityDetial_UID == 0)
                            {
                                //构造插入SQL数据
                                var insertSql = string.Format(@" INSERT INTO IPQCQualityDetial
                                                                               (StationID
                                                                               ,ShiftID
                                                                               ,ProductDate
                                                                               ,TimeInterval
                                                                               ,TimeIntervalIndex
                                                                               ,NGName
                                                                               ,NGNumber
                                                                               ,NGType
                                                                               ,ModifyTime)
                                                                         VALUES
                                                                               ({0}
                                                                               ,{1}
                                                                               ,'{2}'
                                                                               ,N'{3}'
                                                                               ,{4}
                                                                               ,N'{5}'
                                                                               ,{6}
                                                                               ,N'{7}'
                                                                               ,'{8}');",
                                              item.StationID, item.ShiftID, item.ProductDate, item.TimeInterval, item.TimeIntervalIndex,
                                              item.NGName, item.NGNumber, item.NGType, item.ModifyTime);
                                sb.AppendLine(insertSql);
                            }
                            else
                            {

                                var updateSql = string.Format(@" UPDATE IPQCQualityDetial
                                                                       SET StationID ={0}
                                                                          ,ShiftID = {1}
                                                                          ,ProductDate = '{2}'
                                                                          ,TimeInterval = N'{3}'
                                                                          ,TimeIntervalIndex = {4}
                                                                          ,NGName = N'{5}'
                                                                          ,NGNumber ={6}
                                                                          ,NGType = N'{7}'
                                                                          ,ModifyTime = '{8}'
                                                                     WHERE IPQCQualityDetial_UID={9} ;",
                                            item.StationID, item.ShiftID, item.ProductDate, item.TimeInterval, item.TimeIntervalIndex,
                                            item.NGName, item.NGNumber, item.NGType, item.ModifyTime, item.IPQCQualityDetial_UID);

                                sb.AppendLine(updateSql);

                            }
                        }

                    }

                    string sql = sb.ToString();
                    DataContext.Database.ExecuteSqlCommand(sql);
                    trans.Commit();
                }

                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }


        }
        public string InserOrUpdateIPQCWIP(int StationID, int WIP)
        {

            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    var updateSql = string.Format(@"  UPDATE GL_QADetectionPoint  SET WIP = {0}   WHERE StationID = {1};", WIP, StationID);
                    sb.AppendLine(updateSql);
                    string sql = sb.ToString();
                    DataContext.Database.ExecuteSqlCommand(sql);
                    trans.Commit();
                }

                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }


        }

    }
}
