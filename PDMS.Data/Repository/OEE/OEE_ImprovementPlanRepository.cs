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
    public interface IOEE_ImprovementPlanRepository : IRepository<OEE_ImprovementPlan>
    {
        PagedListModel<OEE_ImprovementPlanDTO> QueryImprovementPlanInfo(OEE_ImprovementPlanDTO serchModel, Page page);
        PagedListModel<OEE_ImprovementPlanDTO> QueryActionInfoByCreateDate(OEEFourQParamModel serchModel, Page page);
        string AddImprovementPlan(OEE_ImprovementPlanDTO item);
        string DeleteImpeovementPlanById(int improvementPlanId);
        OEE_ImprovementPlanDTO GetImprovementPlanById(int improvementPlanId);
        string UpdateImprovementPlan(OEE_ImprovementPlanDTO item);
    }

    public class OEE_ImprovementPlanRepository : RepositoryBase<OEE_ImprovementPlan>, IOEE_ImprovementPlanRepository
    {
        public OEE_ImprovementPlanRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public PagedListModel<OEE_ImprovementPlanDTO> QueryImprovementPlanInfo(OEE_ImprovementPlanDTO serchModel, Page page)
        {
            var query = from improvementPlan in DataContext.OEE_ImprovementPlan
                        select new OEE_ImprovementPlanDTO
                        {
                            ImprovementPlan_UID = improvementPlan.ImprovementPlan_UID
                          ,
                            Plant_Organization_UID = improvementPlan.Plant_Organization_UID
                          ,
                            Plant_Organization_Name = improvementPlan.System_Organization2.Organization_Name
                          ,
                            BG_Organization_UID = improvementPlan.BG_Organization_UID
                          ,
                            BG_Organization_Name = improvementPlan.System_Organization1.Organization_Name,

                            FunPlant_Organization_UID = improvementPlan.FunPlant_Organization_UID
                          ,
                            FunPlant_Organization_Name = improvementPlan.System_Organization.Organization_Name,
                            Project_UID = improvementPlan.Project_UID
                          ,
                            ProjectName = improvementPlan.GL_Line.System_Project.Project_Name,
                            LineID = improvementPlan.LineID
                          ,
                            LineName = improvementPlan.GL_Line.LineName,
                            StationID = improvementPlan.StationID
                          ,
                            StationName = improvementPlan.GL_Station.StationName,
                            OEE_MachineInfo_UID = improvementPlan.OEE_MachineInfo_UID
                          ,
                            MeetingType_UID = improvementPlan.MeetingType_UID
                          ,
                            MachineName = improvementPlan.OEE_MachineInfo.MachineNo,
                            ImprovementPlan_ID = improvementPlan.ImprovementPlan_ID
                          ,
                            ImprovementPlan_Name = improvementPlan.ImprovementPlan_Name
                          ,
                            Audience = improvementPlan.Audience
                          ,
                            Responsible = improvementPlan.Responsible
                          ,
                            Status = improvementPlan.Status
                          ,
                            Commit_Date = improvementPlan.Commit_Date
                          ,
                            Due_Date = improvementPlan.Due_Date
                          ,
                            Close_Date = improvementPlan.Close_Date
                          ,
                            Created_UID = improvementPlan.Created_UID
                          ,
                            Created_Date = improvementPlan.Created_Date
                          ,
                            Problem_Description = improvementPlan.Problem_Description
                          ,
                            Root_Cause = improvementPlan.Root_Cause
                          ,
                            CACP_Description = improvementPlan.CACP_Description
                          ,
                            Comment = improvementPlan.Comment
                          ,
                            Modified_UID = improvementPlan.Modified_UID
                          ,
                            Modified_Date = improvementPlan.Modified_Date
                          ,
                            Attachment1 = improvementPlan.Attachment1
                          ,
                            Attachment6 = improvementPlan.Attachment6
                          ,
                            Attachment2 = improvementPlan.Attachment2
                          ,
                            Attachment4 = improvementPlan.Attachment4
                          ,
                            Attachment5 = improvementPlan.Attachment5
                              ,
                            DirDueDate = improvementPlan.DirDueDate
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

            //机台
            if (serchModel.OEE_MachineInfo_UID != 0 && serchModel.StationID != null)
            {
                query = query.Where(p => p.OEE_MachineInfo_UID == serchModel.OEE_MachineInfo_UID);
            }

            //OP
            if (serchModel.MeetingType_UID != 0)
            {
                query = query.Where(p => p.MeetingType_UID == serchModel.MeetingType_UID);
            }

            //ImprovementPlan_ID
            if (!string.IsNullOrEmpty(serchModel.ImprovementPlan_ID))
            {
                query = query.Where(p => p.ImprovementPlan_ID == serchModel.ImprovementPlan_ID);
            }
            //Status
            if (serchModel.Status != 0)
            {
                query = query.Where(p => p.Status == serchModel.Status);
            }

            //ImprovementPlan_Name
            if (!string.IsNullOrEmpty(serchModel.ImprovementPlan_Name))
            {
                query = query.Where(p => p.ImprovementPlan_Name == serchModel.ImprovementPlan_Name);
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
            if (serchModel.Created_Date.HasValue)
            {
                var startDate = Convert.ToDateTime(serchModel.Created_Date.Value.ToString("yyyy-MM-dd 00:00:00"));
                var endDate = Convert.ToDateTime(serchModel.Created_Date.Value.ToString("yyyy-MM-dd 23:59:59"));
                query = query.Where(p => p.Created_Date >= startDate && p.Created_Date <= endDate);
            }

            ///Due_Date
            if (serchModel.Due_Date != null && serchModel.Due_Date.HasValue)
            {
                var startDate = Convert.ToDateTime(serchModel.Due_Date.Value.ToString("yyyy-MM-dd 00:00:00"));
                var endDate = Convert.ToDateTime(serchModel.Due_Date.Value.ToString("yyyy-MM-dd 23:59:59"));
                query = query.Where(p => p.Due_Date >= startDate && p.Due_Date <= endDate);
            }

            ///Close_Date
            if (serchModel.Close_Date != null && serchModel.Close_Date.HasValue)
            {
                var startDate = Convert.ToDateTime(serchModel.Close_Date.Value.ToString("yyyy-MM-dd 00:00:00"));
                var endDate = Convert.ToDateTime(serchModel.Close_Date.Value.ToString("yyyy-MM-dd 23:59:59"));
                query = query.Where(p => p.Close_Date >= startDate && p.Close_Date <= endDate);
            }

            var totalCount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return new PagedListModel<OEE_ImprovementPlanDTO>(totalCount, query.ToList());
        }

        public PagedListModel<OEE_ImprovementPlanDTO> QueryActionInfoByCreateDate(OEEFourQParamModel serchModel, Page page)
        {
            var query = from improvementPlan in DataContext.OEE_ImprovementPlan
                        where improvementPlan.Plant_Organization_UID == serchModel.Plant_Organization_UID
                                && improvementPlan.BG_Organization_UID == serchModel.BG_Organization_UID
                                && improvementPlan.StationID == serchModel.StationID
                                && improvementPlan.OEE_MachineInfo_UID == serchModel.OEE_MachineInfo_UID
                        select new OEE_ImprovementPlanDTO
                        {
                            ImprovementPlan_UID = improvementPlan.ImprovementPlan_UID
                          ,
                            Plant_Organization_UID = improvementPlan.Plant_Organization_UID
                          ,
                            Plant_Organization_Name = improvementPlan.System_Organization2.Organization_Name
                          ,
                            BG_Organization_UID = improvementPlan.BG_Organization_UID
                          ,
                            BG_Organization_Name = improvementPlan.System_Organization1.Organization_Name,

                            FunPlant_Organization_UID = improvementPlan.FunPlant_Organization_UID
                          ,
                            FunPlant_Organization_Name = improvementPlan.System_Organization.Organization_Name,
                            Project_UID = improvementPlan.Project_UID
                          ,
                            ProjectName = improvementPlan.GL_Line.System_Project.Project_Name,
                            LineID = improvementPlan.LineID
                          ,
                            LineName = improvementPlan.GL_Line.LineName,
                            StationID = improvementPlan.StationID
                          ,
                            StationName = improvementPlan.GL_Station.StationName,
                            OEE_MachineInfo_UID = improvementPlan.OEE_MachineInfo_UID
                          ,
                            MeetingType_UID = improvementPlan.MeetingType_UID
                          ,
                            MachineName = improvementPlan.OEE_MachineInfo.MachineNo,
                            ImprovementPlan_ID = improvementPlan.ImprovementPlan_ID
                          ,
                            ImprovementPlan_Name = improvementPlan.ImprovementPlan_Name
                          ,
                            Audience = improvementPlan.Audience
                          ,
                            Responsible = improvementPlan.Responsible
                          ,
                            Status = improvementPlan.Status
                          ,
                            Commit_Date = improvementPlan.Commit_Date
                          ,
                            Due_Date = improvementPlan.Due_Date
                          ,
                            Close_Date = improvementPlan.Close_Date
                          ,
                            Created_UID = improvementPlan.Created_UID
                          ,
                            Created_Date = improvementPlan.Created_Date
                          ,
                            Problem_Description = improvementPlan.Problem_Description
                          ,
                            Root_Cause = improvementPlan.Root_Cause
                          ,
                            CACP_Description = improvementPlan.CACP_Description
                          ,
                            Comment = improvementPlan.Comment
                          ,
                            Modified_UID = improvementPlan.Modified_UID
                          ,
                            Modified_Date = improvementPlan.Modified_Date
                          ,
                            Attachment1 = improvementPlan.Attachment1
                          ,
                            Attachment6 = improvementPlan.Attachment6
                          ,
                            Attachment2 = improvementPlan.Attachment2
                          ,
                            Attachment4 = improvementPlan.Attachment4
                          ,
                            Attachment5 = improvementPlan.Attachment5
                                   ,
                            DirDueDate = improvementPlan.DirDueDate
                        };

            var startDate = Convert.ToDateTime(serchModel.ActionCreateData.ToString("yyyy-MM-dd 00:00:00"));
            var endDate = Convert.ToDateTime(serchModel.ActionCreateData.ToString("yyyy-MM-dd 23:59:59"));
            query = query.Where(p => p.Created_Date >= startDate && p.Created_Date <= endDate);
            var totalCount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return new PagedListModel<OEE_ImprovementPlanDTO>(totalCount, query.ToList());
        }

        public OEE_ImprovementPlanDTO GetImprovementPlanById(int improvementPlanId)
        {
            try
            {
                var query = from improvementPlan in DataContext.OEE_ImprovementPlan
                            where improvementPlan.ImprovementPlan_UID == improvementPlanId
                            select new OEE_ImprovementPlanDTO
                            {
                                ImprovementPlan_UID = improvementPlan.ImprovementPlan_UID
                              ,
                                Plant_Organization_UID = improvementPlan.Plant_Organization_UID
                              ,
                                BG_Organization_UID = improvementPlan.BG_Organization_UID
                              ,
                                FunPlant_Organization_UID = improvementPlan.FunPlant_Organization_UID
                              ,
                                Project_UID = improvementPlan.Project_UID
                              ,
                                LineID = improvementPlan.LineID
                              ,
                                StationID = improvementPlan.StationID
                              ,
                                OEE_MachineInfo_UID = improvementPlan.OEE_MachineInfo_UID
                              ,
                                MeetingType_UID = improvementPlan.MeetingType_UID
                              ,
                                ImprovementPlan_ID = improvementPlan.ImprovementPlan_ID
                              ,
                                ImprovementPlan_Name = improvementPlan.ImprovementPlan_Name
                              ,
                                Audience = improvementPlan.Audience
                              ,
                                Responsible = improvementPlan.Responsible
                              ,
                                Status = improvementPlan.Status
                              ,
                                Commit_Date = improvementPlan.Commit_Date
                              ,
                                Due_Date = improvementPlan.Due_Date
                              ,
                                Close_Date = improvementPlan.Close_Date
                              ,
                                Created_UID = improvementPlan.Created_UID
                              ,
                                Created_Date = improvementPlan.Created_Date
                              ,
                                Problem_Description = improvementPlan.Problem_Description
                              ,
                                Root_Cause = improvementPlan.Root_Cause
                              ,
                                CACP_Description = improvementPlan.CACP_Description
                              ,
                                Comment = improvementPlan.Comment
                              ,
                                Modified_UID = improvementPlan.Modified_UID
                              ,
                                Modified_Date = improvementPlan.Modified_Date
                              ,
                                Attachment1 = improvementPlan.Attachment1
                              ,
                                Attachment6 = improvementPlan.Attachment6
                              ,
                                Attachment2 = improvementPlan.Attachment2
                              ,
                                Attachment4 = improvementPlan.Attachment4
                              ,
                                Attachment5 = improvementPlan.Attachment5
                                       ,
                                DirDueDate = improvementPlan.DirDueDate
                            };
                var improvement = query.FirstOrDefault();
                var queryDetails = from improvementPlanD in DataContext.OEE_ImprovementPlanD
                                   where improvementPlanD.ImprovementPlan_UID == improvement.ImprovementPlan_UID
                                   select new OEE_ImprovementPlanDDTO
                                   {
                                       ImprovementPlanD_UID = improvementPlanD.ImprovementPlanD_UID,
                                       ImprovementPlan_UID = improvementPlanD.ImprovementPlan_UID,
                                       Metric_UID = improvementPlanD.Metric_UID,
                                       Metric_ID = improvementPlanD.Metric_ID,
                                       Metric_Name = improvementPlanD.Metric_Name
                                   };
                improvement.OEE_ImprovementPlanDetailList = queryDetails.ToList();
                return improvement;
            }
            catch (Exception ex)
            {
                return new OEE_ImprovementPlanDTO { };
            }
        }


        public string DeleteImpeovementPlanById(int improvementPlanId)
        {
            var sqlDeltails = $"delete from OEE_ImprovementPlanD where ImprovementPlan_UID={improvementPlanId}";
            DataContext.Database.ExecuteSqlCommand(sqlDeltails.ToString());
            var sql = $"delete from OEE_ImprovementPlan where ImprovementPlan_UID={improvementPlanId}";
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

        public string AddImprovementPlan(OEE_ImprovementPlanDTO item)
        {
            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    var masterSql = GetInsertImprovementPlanSql(item);
                    DataContext.Database.ExecuteSqlCommand(masterSql);
                    var mUID = "SELECT  SCOPE_IDENTITY();";
                    var flMasterUID = Convert.ToInt32(DataContext.Database.SqlQuery<decimal>(mUID).First());

                    var deleteSql = $"delete from OEE_ImprovementPlanD where ImprovementPlanD_UID={flMasterUID}";
                    DataContext.Database.ExecuteSqlCommand(deleteSql);

                    foreach (var dtoItem in item.OEE_ImprovementPlanDetailList)
                    {
                        dtoItem.ImprovementPlan_UID = flMasterUID;
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

        public string UpdateImprovementPlan(OEE_ImprovementPlanDTO item)
        {
            try
            {
                DateTime nowDate = DateTime.Now;

                using (var trans = DataContext.Database.BeginTransaction())
                {
                    var result = DataContext.OEE_ImprovementPlan.Where(p => p.ImprovementPlan_UID == item.ImprovementPlan_UID).FirstOrDefault();
                    if (result != null)
                    {
                        result.ImprovementPlan_ID = item.ImprovementPlan_ID;
                        result.ImprovementPlan_Name = item.ImprovementPlan_Name;
                        result.Status = item.Status;
                        result.Audience = item.Audience;
                        result.Responsible = item.Responsible;
                        result.Problem_Description = item.Problem_Description;
                        result.Root_Cause = item.Root_Cause;
                        result.Comment = item.Comment;
                        result.Modified_UID = item.Modified_UID;
                        result.Modified_Date = item.Modified_Date;
                        result.Commit_Date = item.Commit_Date;
                        result.DirDueDate = item.DirDueDate;
                        if (item.Status ==4)
                        {
                            result.Close_Date = item.Close_Date;
                        }

                        result.Due_Date = item.Due_Date;
                    }
                    DataContext.SaveChanges();
                    DataContext.Commit();

                    var deleteSql = $"delete from OEE_ImprovementPlanD where ImprovementPlan_UID={ item.ImprovementPlan_UID}";
                    DataContext.Database.ExecuteSqlCommand(deleteSql);
                    DataContext.SaveChanges();
                    DataContext.Commit();

                    foreach (var dtoItem in item.OEE_ImprovementPlanDetailList)
                    {
                        dtoItem.ImprovementPlan_UID = item.ImprovementPlan_UID;
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

        private string GetInsertImprovementPlanSql(OEE_ImprovementPlanDTO item)
        {
            string sql = string.Empty;
            if (item.ImprovementPlan_UID == 0)
            {
                sql = @"INSERT INTO [dbo].[OEE_ImprovementPlan]
                               ([Plant_Organization_UID]
                               ,[BG_Organization_UID]
                               ,[FunPlant_Organization_UID]
                               ,[Project_UID]
                               ,[LineID]
                               ,[StationID]
                               ,[OEE_MachineInfo_UID]
                               ,[MeetingType_UID]
                               ,[ImprovementPlan_ID]
                               ,[ImprovementPlan_Name]
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
                               ,[CACP_Description]
                               ,[Comment]
                               ,[Modified_UID]
                               ,[Modified_Date]
                               ,[Attachment1]
                               ,[Attachment6]
                               ,[Attachment2]
                               ,[Attachment4]
                               ,[Attachment5]
                               ,[DirDueDate]
                           )
                         VALUES
                               ({0}
                               ,{1}
                               ,{2}
                               ,{3}
                               ,{4}
                               ,{5}
                               ,{6}
                               ,{7}
                               ,N'{8}'
                               ,N'{9}'
                               ,N'{10}'
                               ,N'{11}'
                               ,N'{12}'
                               ,N'{13}'
                               ,N'{14}'
                               ,N'{15}'
                               ,{16}
                               ,N'{17}'
                               ,N'{18}'
                               ,N'{19}'
                               ,N'{20}'
                               ,N'{21}'
                               ,{22}
                               ,N'{23}'
                               ,N'{24}'
                               ,N'{25}'
                               ,N'{26}'
                               ,N'{27}'
                               ,N'{28}'
                               ,N'{29}'
                         )";

                sql = string.Format(sql,
                item.Plant_Organization_UID,
                item.BG_Organization_UID,
                item.FunPlant_Organization_UID,
                item.Project_UID,
                item.LineID,
                item.StationID,
                item.OEE_MachineInfo_UID,
                item.MeetingType_UID,
                item.ImprovementPlan_ID,
                item.ImprovementPlan_Name,
                item.Audience,
                item.Responsible,
                item.Status,
                item.Commit_Date,
                item.Due_Date,
                item.Close_Date,
                item.Created_UID,
                item.Created_Date,
                item.Problem_Description,
                item.Root_Cause,
                item.CACP_Description,
                item.Comment,
                item.Modified_UID,
                item.Modified_Date,
                item.Attachment1,
                item.Attachment6,
                item.Attachment2,
                item.Attachment4,
                item.Attachment5,
                item.DirDueDate
                );
            }
            else
            {
            }
            return sql;
        }

        private string GetInsertDetailSql(OEE_ImprovementPlanDDTO item)
        {
            string sql = string.Empty;
            if (item.ImprovementPlanD_UID == 0)
            {
                sql = @"INSERT INTO [dbo].[OEE_ImprovementPlanD]
                               ([ImprovementPlan_UID]
                               ,[Metric_UID]
                               ,[Metric_ID]
                               ,[Metric_Name])
                         VALUES
                               ({0}
                               ,{1}
                               ,N'{2}'
                               ,N'{3}')";
                sql = string.Format(sql,
                    item.ImprovementPlan_UID,
                    item.Metric_UID,
                    item.Metric_ID,
                    item.Metric_Name);
            }

            return sql;
        }
    }
}
