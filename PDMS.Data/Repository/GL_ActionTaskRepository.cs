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
    public interface IGL_ActionTaskRepository : IRepository<GL_ActionTasker>
    {
        PagedListModel<GL_ActionTaskDTO> QueryActionTaskerInfo(GL_ActionTaskDTO serchModel, Page page);
        string Add_GL_ActionTasker(GL_ActionTaskDTO item);
        string Delete_GL_ActionTaskerById(int ActionTasker_ID);
        GL_ActionTaskDTO Get_GL_ActionTaskerById(int ActionTasker_ID);
        string Update_GL_ActionTasker(GL_ActionTaskDTO item);
        PagedListModel<GL_ActionTaskDTO> QueryActionInfoByCreateDate(GLFourQParamModel serchModel, Page page);
    }

    public class GL_ActionTaskRepository : RepositoryBase<GL_ActionTasker>, IGL_ActionTaskRepository
    {
        public GL_ActionTaskRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public PagedListModel<GL_ActionTaskDTO> QueryActionTaskerInfo(GL_ActionTaskDTO serchModel, Page page)
        {
            var query = from GL_ActionTasker in DataContext.GL_ActionTasker
                        select new GL_ActionTaskDTO
                        {
                            ActionTasker_UID = GL_ActionTasker.ActionTasker_UID
                          ,
                            Plant_Organization_UID = GL_ActionTasker.Plant_Organization_UID
                          ,
                            Plant_Organization_Name = GL_ActionTasker.System_Organization2.Organization_Name
                          ,
                            BG_Organization_UID = GL_ActionTasker.BG_Organization_UID
                          ,
                            BG_Organization_Name = GL_ActionTasker.System_Organization1.Organization_Name,

                            FunPlant_Organization_UID = GL_ActionTasker.FunPlant_Organization_UID
                          ,
                            FunPlant_Organization_Name = GL_ActionTasker.System_Organization.Organization_Name,
                            Project_UID = GL_ActionTasker.Project_UID
                          ,
                            ProjectName = GL_ActionTasker.GL_Line.System_Project.Project_Name,
                            LineID = GL_ActionTasker.LineID
                          ,
                            LineName = GL_ActionTasker.GL_Line.LineName,
                            StationID = GL_ActionTasker.StationID
                          ,
                            StationName = GL_ActionTasker.GL_Station.StationName
                          ,
                            MeetingType_UID = GL_ActionTasker.MeetingType_UID
                          ,
                            ActionTasker_ID = GL_ActionTasker.ActionTasker_ID
                          ,
                            ActionTasker_Name = GL_ActionTasker.ActionTasker_Name
                          ,
                            Audience = GL_ActionTasker.Audience
                          ,
                            Responsible = GL_ActionTasker.Responsible
                          ,
                            Status = GL_ActionTasker.Status
                          ,
                            Commit_Date = GL_ActionTasker.Commit_Date
                          ,
                            Due_Date = GL_ActionTasker.Due_Date
                          ,
                            Close_Date = GL_ActionTasker.Close_Date
                          ,
                            Created_UID = GL_ActionTasker.Created_UID
                          ,
                            Created_Date = GL_ActionTasker.Created_Date
                          ,
                            Problem_Description = GL_ActionTasker.Problem_Description
                          ,
                            Root_Cause = GL_ActionTasker.Root_Cause
                          ,
                            Action_Description = GL_ActionTasker.Action_Description
                          ,
                            Comment = GL_ActionTasker.Comment
                          ,
                            Modified_UID = GL_ActionTasker.Modified_UID
                          ,
                            Modified_Date = GL_ActionTasker.Modified_Date
                          ,
                            Attachment1 = GL_ActionTasker.Attachment1
                          ,
                            Attachment6 = GL_ActionTasker.Attachment6
                          ,
                            Attachment2 = GL_ActionTasker.Attachment2
                          ,
                            Attachment4 = GL_ActionTasker.Attachment4
                          ,
                            Attachment5 = GL_ActionTasker.Attachment5
                          ,
                            Remarks = GL_ActionTasker.Remarks
                          ,
                            Department = GL_ActionTasker.Department
                          ,
                            CloseByUser = GL_ActionTasker.CloseByUser
                          ,
                            DirDueDate = GL_ActionTasker.DirDueDate
                        };

            //厂区
            if (serchModel.Plant_Organization_UID != 0)
            {
                query = query.Where(p => p.Plant_Organization_UID == serchModel.Plant_Organization_UID);
            }

            //OP
            if (serchModel.BG_Organization_UID != 0)
            {
                query = query.Where(p => p.BG_Organization_UID == serchModel.BG_Organization_UID);
            }

            //功能厂
            if (serchModel.FunPlant_Organization_UID != 0 && serchModel.FunPlant_Organization_UID != null)
            {
                query = query.Where(p => p.FunPlant_Organization_UID == serchModel.FunPlant_Organization_UID);
            }

            //专案
            if (serchModel.Project_UID != 0)
            {
                query = query.Where(p => p.Project_UID == serchModel.Project_UID);
            }

            //线别
            if (serchModel.LineID != 0 && serchModel.LineID != null)
            {
                query = query.Where(p => p.LineID == serchModel.LineID);
            }

            //工站
            if (serchModel.StationID != 0 && serchModel.StationID != null)
            {
                query = query.Where(p => p.StationID == serchModel.StationID);
            }

            //OP
            if (serchModel.MeetingType_UID != 0)
            {
                query = query.Where(p => p.MeetingType_UID == serchModel.MeetingType_UID);
            }

            //ActionTasker_ID
            if (!string.IsNullOrEmpty(serchModel.ActionTasker_ID))
            {
                query = query.Where(p => p.ActionTasker_ID == serchModel.ActionTasker_ID);
            }
            //Status
            if (serchModel.Status != 0)
            {
                query = query.Where(p => p.Status == serchModel.Status);
            }

            //ActionTasker_Name
            if (!string.IsNullOrEmpty(serchModel.ActionTasker_Name))
            {
                query = query.Where(p => p.ActionTasker_Name == serchModel.ActionTasker_Name);
            }

            //Audience
            if (!string.IsNullOrEmpty(serchModel.Audience))
            {
                query = query.Where(p => p.Audience == serchModel.Audience);
            }
            //Responsible
            if (!string.IsNullOrEmpty(serchModel.Responsible))
            {
                query = query.Where(p => p.Responsible == serchModel.Responsible);
            }

            //Problem_Description
            if (!string.IsNullOrEmpty(serchModel.Problem_Description))
            {
                query = query.Where(p => p.Problem_Description == serchModel.Problem_Description);
            }

            ///Created_Date
            if (serchModel.Commit_Date != null)
            {
                query = query.Where(p => p.Created_Date == serchModel.Created_Date);
            }

            ///Due_Date
            if (serchModel.Due_Date != null)
            {
                query = query.Where(p => p.Due_Date == serchModel.Due_Date);
            }

            ///Close_Date
            if (serchModel.Close_Date != null)
            {
                query = query.Where(p => p.Close_Date == serchModel.Close_Date);
            }

            ///DirDueDate
            if (serchModel.Close_Date != null)
            {
                query = query.Where(p => p.DirDueDate == serchModel.DirDueDate);
            }

            var totalCount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return new PagedListModel<GL_ActionTaskDTO>(totalCount, query.ToList());
        }

        public GL_ActionTaskDTO Get_GL_ActionTaskerById(int ActionTasker_ID)
        {
            try
            {
                var query = from GL_ActionTasker in DataContext.GL_ActionTasker
                            where GL_ActionTasker.ActionTasker_UID == ActionTasker_ID
                            select new GL_ActionTaskDTO
                            {
                                ActionTasker_UID = GL_ActionTasker.ActionTasker_UID
                              ,
                                Plant_Organization_UID = GL_ActionTasker.Plant_Organization_UID
                              ,
                                BG_Organization_UID = GL_ActionTasker.BG_Organization_UID
                              ,
                                FunPlant_Organization_UID = GL_ActionTasker.FunPlant_Organization_UID
                              ,
                                Project_UID = GL_ActionTasker.Project_UID
                              ,
                                LineID = GL_ActionTasker.LineID
                              ,
                                StationID = GL_ActionTasker.StationID
                              ,
                                MeetingType_UID = GL_ActionTasker.MeetingType_UID
                              ,
                                ActionTasker_ID = GL_ActionTasker.ActionTasker_ID
                              ,
                                ActionTasker_Name = GL_ActionTasker.ActionTasker_Name
                              ,
                                Audience = GL_ActionTasker.Audience
                              ,
                                Responsible = GL_ActionTasker.Responsible
                              ,
                                Status = GL_ActionTasker.Status
                              ,
                                Commit_Date = GL_ActionTasker.Commit_Date
                              ,
                                Due_Date = GL_ActionTasker.Due_Date
                              ,
                                Close_Date = GL_ActionTasker.Close_Date
                              ,
                                Created_UID = GL_ActionTasker.Created_UID
                              ,
                                Created_Date = GL_ActionTasker.Created_Date
                              ,
                                Problem_Description = GL_ActionTasker.Problem_Description
                              ,
                                Root_Cause = GL_ActionTasker.Root_Cause
                              ,
                                Action_Description = GL_ActionTasker.Action_Description
                              ,
                                Comment = GL_ActionTasker.Comment
                              ,
                                Modified_UID = GL_ActionTasker.Modified_UID
                              ,
                                Modified_Date = GL_ActionTasker.Modified_Date
                              ,
                                Attachment1 = GL_ActionTasker.Attachment1
                              ,
                                Attachment6 = GL_ActionTasker.Attachment6
                              ,
                                Attachment2 = GL_ActionTasker.Attachment2
                              ,
                                Attachment4 = GL_ActionTasker.Attachment4
                              ,
                                Attachment5 = GL_ActionTasker.Attachment5
                                ,
                                Department = GL_ActionTasker.Department
                                ,
                                Remarks = GL_ActionTasker.Remarks
                                ,
                                DirDueDate = GL_ActionTasker.DirDueDate
                                ,
                                CloseByUser = GL_ActionTasker.CloseByUser
                            };
                var action = query.FirstOrDefault();
                var queryDetails = from GL_ActionTaskerD in DataContext.GL_ActionTaskerD
                                   where GL_ActionTaskerD.ActionTasker_UID == action.ActionTasker_UID
                                   select new GL_ActionTaskDDTO
                                   {
                                       ActionTaskerD_UID = GL_ActionTaskerD.ActionTaskerD_UID,
                                       ActionTasker_UID = GL_ActionTaskerD.ActionTasker_UID,
                                       Metric_UID = GL_ActionTaskerD.Metric_UID,
                                       Metric_ID = GL_ActionTaskerD.Metric_ID,
                                       Metric_Name = GL_ActionTaskerD.Metric_Name
                                   };
                action.GL_GL_ActionTaskDetailList = queryDetails.ToList();
                return action;
            }
            catch (Exception ex)
            {
                return new GL_ActionTaskDTO { };
            }
        }


        public string Delete_GL_ActionTaskerById(int ActionTasker_ID)
        {
            var sqlDeltails = $"delete from GL_ActionTaskerD where ActionTasker_UID={ActionTasker_ID}";
            DataContext.Database.ExecuteSqlCommand(sqlDeltails.ToString());
            var sql = $"delete from GL_ActionTasker where ActionTasker_UID={ActionTasker_ID}";
            var result = DataContext.Database.ExecuteSqlCommand(sql.ToString());

            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "删除失败";
            }
        }

        public string Add_GL_ActionTasker(GL_ActionTaskDTO item)
        {
            try
            {
                DateTime nowDate = DateTime.Now;
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    var masterSql = GetInsertActionTaskSql(item);
                    DataContext.Database.ExecuteSqlCommand(masterSql);
                    var mUID = "SELECT  SCOPE_IDENTITY();";
                    var flMasterUID = Convert.ToInt32(DataContext.Database.SqlQuery<decimal>(mUID).First());

                    var deleteSql = $"delete from GL_ActionTaskerD where ActionTaskerD_UID={flMasterUID}";
                    DataContext.Database.ExecuteSqlCommand(deleteSql);

                    foreach (var dtoItem in item.GL_GL_ActionTaskDetailList)
                    {
                        dtoItem.ActionTasker_UID = flMasterUID;
                        var sql = GetInsertDetailSql(dtoItem);
                        DataContext.Database.ExecuteSqlCommand(sql);
                    }

                    DataContext.SaveChanges();
                    trans.Commit();
                }
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return "FAILD";
            }
        }

        public string Update_GL_ActionTasker(GL_ActionTaskDTO item)
        {
            try
            {
                DateTime nowDate = DateTime.Now;


                using (var trans = DataContext.Database.BeginTransaction())
                {
                    var result = DataContext.GL_ActionTasker.Where(p => p.ActionTasker_UID == item.ActionTasker_UID).FirstOrDefault();
                    if (result != null)
                    {
                        result.ActionTasker_ID = item.ActionTasker_ID;
                        result.ActionTasker_Name = item.ActionTasker_Name;
                        result.Status = item.Status;
                        result.Audience = item.Audience;
                        result.Responsible = item.Responsible;
                        result.Problem_Description = item.Problem_Description;
                        result.Root_Cause = item.Root_Cause;
                        result.Comment = item.Comment;
                        result.Created_UID = item.Created_UID;
                        result.Modified_UID = item.Modified_UID;
                        result.Modified_Date = item.Modified_Date;
                        result.Commit_Date = item.Commit_Date;
                        if (item.Status == 4)
                        {
                            result.Close_Date = item.Close_Date;
                        }
                        result.Created_Date = item.Created_Date;
                        result.Due_Date = item.Due_Date;
                        result.Department = item.Department;
                        result.Remarks = item.Remarks;
                        if (item.DirDueDate != null)
                            result.CloseByUser = item.CloseByUser;
                        if (item.Status == 2 || item.Status == 4)
                        {
                            result.DirDueDate = item.DirDueDate;
                        }
                        result.Action_Description = item.Action_Description;
                    }
                    DataContext.SaveChanges();
                    DataContext.Commit();

                    var deleteSql = $"delete from GL_ActionTaskerD where ActionTasker_UID={ item.ActionTasker_UID}";
                    DataContext.Database.ExecuteSqlCommand(deleteSql);
                    DataContext.SaveChanges();
                    DataContext.Commit();

                    foreach (var dtoItem in item.GL_GL_ActionTaskDetailList)
                    {
                        dtoItem.ActionTasker_UID = item.ActionTasker_UID;
                        var sql = GetInsertDetailSql(dtoItem);
                        DataContext.Database.ExecuteSqlCommand(sql);
                    }

                    DataContext.SaveChanges();
                    DataContext.Commit();

                    trans.Commit();
                }
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return "FAILD";
            }
        }

        private string GetInsertActionTaskSql(GL_ActionTaskDTO item)
        {
            string sql = string.Empty;
            if (item.ActionTasker_UID == 0)
            {
                sql = @"INSERT INTO [dbo].[GL_ActionTasker]
                               ([Plant_Organization_UID]
                               ,[BG_Organization_UID]
                               ,[FunPlant_Organization_UID]
                               ,[Project_UID]
                               ,[LineID]
                               ,[StationID]
                               ,[MeetingType_UID]
                               ,[ActionTasker_ID]
                               ,[ActionTasker_Name]
                               ,[Audience]
                               ,[Responsible]
                               ,[Status]
                               ,[Commit_Date]
                               ,[Due_Date]
                               ,[Close_Date]
                               ,[Created_UID]
                               ,[Created_Date]
                               ,[Problem_Description]
                               ,[Root_Cause]
                               ,[Action_Description]
                               ,[Comment]
                               ,[Modified_UID]
                               ,[Modified_Date]
                               ,[Attachment1]
                               ,[Attachment6]
                               ,[Attachment2]
                               ,[Attachment4]
                               ,[Attachment5]
                               ,[Department]
                               ,[Remarks]
                               ,[CloseByUser]
                               ,[DirDueDate])
                         VALUES
                               ({0}
                               ,{1}
                               ,{2}
                               ,{3}
                               ,{4}
                               ,{5}
                               ,{6}
                               ,N'{7}'
                               ,N'{8}'
                               ,N'{9}'
                               ,N'{10}'
                               ,N'{11}'
                               ,N'{12}'
                               ,N'{13}'
                               ,N'{14}'
                               ,{15}
                               ,N'{16}'
                               ,N'{17}'
                               ,N'{18}'
                               ,N'{19}'
                               ,N'{20}'
                               ,{21}
                               ,N'{22}'
                               ,N'{23}'
                               ,N'{24}'
                               ,N'{25}'
                               ,N'{26}'
                               ,N'{27}'
                               ,N'{28}'
                               ,N'{29}'
                               ,N'{30}'
                               ,N'{31}')";

                sql = string.Format(sql,
                item.Plant_Organization_UID,
                item.BG_Organization_UID,
                item.FunPlant_Organization_UID,
                item.Project_UID,
                item.LineID,
                item.StationID,
                item.MeetingType_UID,
                item.ActionTasker_ID,
                item.ActionTasker_Name,
                item.Audience,
                item.Responsible,
                item.Status,
                item.Commit_Date.HasValue ? item.Commit_Date.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "",
                item.Due_Date.HasValue?item.Due_Date.Value.ToString("yyyy-MM-dd HH:mm:ss.fff"): "",
                item.Close_Date.HasValue ? item.Close_Date.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "",
                item.Created_UID,
                item.Created_Date.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                item.Problem_Description,
                item.Root_Cause,
                item.Action_Description,
                item.Comment,
                item.Modified_UID,
                item.Modified_Date.HasValue ?item.Modified_Date.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "",
                item.Attachment1,
                item.Attachment6,
                item.Attachment2,
                item.Attachment4,
                item.Attachment5,
                item.Department,
                item.Remarks,
                item.CloseByUser,
                item.DirDueDate
                );
            }
            else
            {
            }
            return sql;
        }

        private string GetInsertDetailSql(GL_ActionTaskDDTO item)
        {
            string sql = string.Empty;
            if (item.ActionTaskerD_UID == 0)
            {
                sql = @"INSERT INTO [dbo].[GL_ActionTaskerD]
                               ([ActionTasker_UID]
                               ,[Metric_UID]
                               ,[Metric_ID]
                               ,[Metric_Name])
                         VALUES
                               ({0}
                               ,{1}
                               ,N'{2}'
                               ,N'{3}')";
                sql = string.Format(sql,
                    item.ActionTasker_UID,
                    item.Metric_UID,
                    item.Metric_ID,
                    item.Metric_Name);
            }

            return sql;
        }

        public PagedListModel<GL_ActionTaskDTO> QueryActionInfoByCreateDate(GLFourQParamModel serchModel, Page page)
        {
            var query = from GL_ActionTasker in DataContext.GL_ActionTasker
                        where GL_ActionTasker.Plant_Organization_UID == serchModel.Plant_Organization_UID
                                && GL_ActionTasker.BG_Organization_UID == serchModel.BG_Organization_UID
                                && GL_ActionTasker.StationID == serchModel.StationID
                        select new GL_ActionTaskDTO
                        {
                            ActionTasker_UID = GL_ActionTasker.ActionTasker_UID
                          ,
                            Plant_Organization_UID = GL_ActionTasker.Plant_Organization_UID
                          ,
                            Plant_Organization_Name = GL_ActionTasker.System_Organization2.Organization_Name
                          ,
                            BG_Organization_UID = GL_ActionTasker.BG_Organization_UID
                          ,
                            BG_Organization_Name = GL_ActionTasker.System_Organization1.Organization_Name,

                            FunPlant_Organization_UID = GL_ActionTasker.FunPlant_Organization_UID
                          ,
                            FunPlant_Organization_Name = GL_ActionTasker.System_Organization.Organization_Name,
                            Project_UID = GL_ActionTasker.Project_UID
                          ,
                            ProjectName = GL_ActionTasker.GL_Line.System_Project.Project_Name,
                            LineID = GL_ActionTasker.LineID
                          ,
                            LineName = GL_ActionTasker.GL_Line.LineName,
                            StationID = GL_ActionTasker.StationID
                          ,
                            StationName = GL_ActionTasker.GL_Station.StationName
                          ,
                            MeetingType_UID = GL_ActionTasker.MeetingType_UID
                          ,
                            ActionTasker_ID = GL_ActionTasker.ActionTasker_ID
                          ,
                            ActionTasker_Name = GL_ActionTasker.ActionTasker_Name
                          ,
                            Audience = GL_ActionTasker.Audience
                          ,
                            Responsible = GL_ActionTasker.Responsible
                          ,
                            Status = GL_ActionTasker.Status
                          ,
                            Commit_Date = GL_ActionTasker.Commit_Date
                          ,
                            Due_Date = GL_ActionTasker.Due_Date
                          ,
                            Close_Date = GL_ActionTasker.Close_Date
                          ,
                            Created_UID = GL_ActionTasker.Created_UID
                          ,
                            Created_Date = GL_ActionTasker.Created_Date
                          ,
                            Problem_Description = GL_ActionTasker.Problem_Description
                          ,
                            Root_Cause = GL_ActionTasker.Root_Cause
                          ,
                            Action_Description = GL_ActionTasker.Action_Description
                          ,
                            Comment = GL_ActionTasker.Comment
                          ,
                            Modified_UID = GL_ActionTasker.Modified_UID
                          ,
                            Modified_Date = GL_ActionTasker.Modified_Date
                          ,
                            Attachment1 = GL_ActionTasker.Attachment1
                          ,
                            Attachment6 = GL_ActionTasker.Attachment6
                          ,
                            Attachment2 = GL_ActionTasker.Attachment2
                          ,
                            Attachment4 = GL_ActionTasker.Attachment4
                          ,
                            Attachment5 = GL_ActionTasker.Attachment5
                          ,
                            Remarks = GL_ActionTasker.Remarks
                          ,
                            Department = GL_ActionTasker.Department
                          ,
                            CloseByUser = GL_ActionTasker.CloseByUser
                          ,
                            DirDueDate = GL_ActionTasker.DirDueDate
                        };

            var startDate = Convert.ToDateTime(serchModel.ActionCreateData.ToString("yyyy-MM-dd 00:00:00"));
            var endDate = Convert.ToDateTime(serchModel.ActionCreateData.ToString("yyyy-MM-dd 23:59:59"));
            query = query.Where(p => p.Created_Date >= startDate && p.Created_Date <= endDate);
            var totalCount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return new PagedListModel<GL_ActionTaskDTO>(totalCount, query.ToList());
        }
    }
}
