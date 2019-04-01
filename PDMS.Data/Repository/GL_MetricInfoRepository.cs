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
    public interface IGL_MetricInfoRepository : IRepository<GL_MetricInfo>
    {
        PagedListModel<GL_MetricInfoDTO> QueryMetricInfoInfo(GL_MetricInfoDTO serchModel, Page page);
        string AddMetricInfoInfo(GL_MetricInfoDTO addModel);
        string DeleteMetricInfo(int MetricInfo_UID);
        GL_MetricInfoDTO GetMetricInfoInfoById(int uid);

        List<GL_MetricInfoDTO> GetMetricName(int plantUid, int bgUid, int funplantUid);

    }
    public class GL_MetricInfoRepository : RepositoryBase<GL_MetricInfo>, IGL_MetricInfoRepository
    {
        public GL_MetricInfoRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public PagedListModel<GL_MetricInfoDTO> QueryMetricInfoInfo(GL_MetricInfoDTO serchModel, Page page)
        {
            var query = from MetricInfo in DataContext.GL_MetricInfo
                        select new GL_MetricInfoDTO
                        {
                            Metric_UID = MetricInfo.Metric_UID,
                            Plant_Organization_UID = MetricInfo.Plant_Organization_UID,
                            Plant_Organization_Name = MetricInfo.System_Organization2.Organization_Name,
                            BG_Organization_UID = MetricInfo.BG_Organization_UID,
                            BG_Organization_Name = MetricInfo.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = MetricInfo.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = MetricInfo.System_Organization.Organization_Name,
                            Metric_ID = MetricInfo.Metric_ID,
                            Metric_Name = MetricInfo.Metric_Name,
                            Modified_UID = MetricInfo.Modified_UID,
                            Modified_Date = MetricInfo.Modified_Date,
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

            //会议ID
            if (!string.IsNullOrEmpty(serchModel.Metric_ID))
            {
                query = query.Where(p => p.Metric_ID == serchModel.Metric_ID);
            }

            //会议名称
            if (!string.IsNullOrEmpty(serchModel.Metric_Name))
            {
                query = query.Where(p => p.Metric_Name == serchModel.Metric_Name);
            }

            var totalCount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return new PagedListModel<GL_MetricInfoDTO>(totalCount, query.ToList());
        }

        public GL_MetricInfoDTO GetMetricInfoInfoById(int uid)
        {
            var query = from MetricInfo in DataContext.GL_MetricInfo
                        where MetricInfo.Metric_UID == uid
                        select new GL_MetricInfoDTO
                        {
                            Metric_UID = MetricInfo.Metric_UID,
                            Plant_Organization_UID = MetricInfo.Plant_Organization_UID,
                            Plant_Organization_Name = MetricInfo.System_Organization2.Organization_Name,
                            BG_Organization_UID = MetricInfo.BG_Organization_UID,
                            BG_Organization_Name = MetricInfo.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = MetricInfo.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = MetricInfo.System_Organization.Organization_Name,
                            Metric_ID = MetricInfo.Metric_ID,
                            Metric_Name = MetricInfo.Metric_Name,
                            Modified_UID = MetricInfo.Modified_UID,
                            Modified_Date = MetricInfo.Modified_Date,
                        };

            return query.FirstOrDefault();
        }

        public string AddMetricInfoInfo(GL_MetricInfoDTO addModel)
        {
            try
            {
                var sql = @"
                                   [dbo].[GL_MetricInfo]
                                   ([Plant_Organization_UID]
                                   ,[BG_Organization_UID]
                                   ,[FunPlant_Organization_UID]
                                   ,[Metric_ID]
                                   ,[Metric_Name]
                                   ,[Modified_UID]
                                   ,[Modified_Date])
                  VALUES  (
                                    {0} ,
                                    {1} , 
                                    {2} , 
                                N'{3}' , 
                                N'{4}' , 
                                    {5} , 
                                   '{6}' 
                          )";
                sql = string.Format(sql,
                                      addModel.Plant_Organization_UID,
                                      addModel.BG_Organization_UID,
                                      addModel.FunPlant_Organization_UID,
                                      addModel.Metric_ID,
                                      addModel.Metric_Name,
                                      addModel.Modified_UID,
                                      addModel.Modified_Date);
                var result = DataContext.Database.ExecuteSqlCommand(sql.ToString());
                if (result > 0)
                {
                    return "SUCCESS";
                }
                else
                {
                    return "添加失败";
                }
            }
            catch (Exception ex)
            {
                return "添加失败: 错误信息" + ex.Message.ToString();
            }
        }
        public string DeleteMetricInfo(int MetricInfo_UID)
        {
            try
            {
                var sql = $"DELETE FROM dbo.GL_MetricInfo WHERE Metric_UID={MetricInfo_UID}";
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
            catch (Exception ex)
            {
                return "数据已经被使用不能删除";
            }
        }

        public List<GL_MetricInfoDTO> GetMetricName(int plantUid, int bgUid, int funplantUid)
        {
            var query = from MetricInfo in DataContext.GL_MetricInfo
                        select new GL_MetricInfoDTO
                        {
                            Metric_UID = MetricInfo.Metric_UID,
                            Plant_Organization_UID = MetricInfo.Plant_Organization_UID,
                            Plant_Organization_Name = MetricInfo.System_Organization2.Organization_Name,
                            BG_Organization_UID = MetricInfo.BG_Organization_UID,
                            BG_Organization_Name = MetricInfo.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = MetricInfo.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = MetricInfo.System_Organization.Organization_Name,
                            Metric_ID = MetricInfo.Metric_ID,
                            Metric_Name = MetricInfo.Metric_Name,
                            Modified_UID = MetricInfo.Modified_UID,
                            Modified_Date = MetricInfo.Modified_Date,
                        };

            //厂区
            if (plantUid != 0)
            {
                query = query.Where(p => p.Plant_Organization_UID == plantUid);
            }

            //OP
            if (bgUid != 0)
            {
                query = query.Where(p => p.BG_Organization_UID == bgUid);
            }

            //功能厂
            if (funplantUid != 0)
            {
                query = query.Where(p => p.FunPlant_Organization_UID == funplantUid);
            }

            return query.ToList();
        }
    }
}
