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
    public interface IGL_MeetingTypeInfoRepository : IRepository<GL_MeetingTypeInfo>
    {
        PagedListModel<GL_MeetingTypeInfoDTO> QueryGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO serchModel, Page page);
        string AddGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO addModel);
        string DeleteMeetTypeInfo(int meetTypeInfo_UID);
        GL_MeetingTypeInfoDTO GetGL_MeetingTypeInfo(int uid);

        List<GL_MeetingTypeInfoDTO> GetMeetingTypeName(int plantUid, int bgUid, int funplantUid);
    }

    public class GL_MeetingTypeInfoRepository : RepositoryBase<GL_MeetingTypeInfo>, IGL_MeetingTypeInfoRepository
    {
        public GL_MeetingTypeInfoRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public PagedListModel<GL_MeetingTypeInfoDTO> QueryGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO serchModel, Page page)
        {
            var query = from meetTypeInfo in DataContext.GL_MeetingTypeInfo
                        select new GL_MeetingTypeInfoDTO
                        {
                            MeetingType_UID = meetTypeInfo.MeetingType_UID,
                            Plant_Organization_UID = meetTypeInfo.Plant_Organization_UID,
                            Plant_Organization_Name = meetTypeInfo.System_Organization2.Organization_Name,
                            BG_Organization_UID = meetTypeInfo.BG_Organization_UID,
                            BG_Organization_Name = meetTypeInfo.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = meetTypeInfo.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = meetTypeInfo.System_Organization.Organization_Name,
                            MeetingType_ID = meetTypeInfo.MeetingType_ID,
                            MeetingType_Name = meetTypeInfo.MeetingType_Name,
                            Modified_UID = meetTypeInfo.Modified_UID,
                            Modified_Date = meetTypeInfo.Modified_Date,
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
            if (!string.IsNullOrEmpty(serchModel.MeetingType_ID))
            {
                query = query.Where(p => p.MeetingType_ID == serchModel.MeetingType_ID);
            }

            //会议名称
            if (!string.IsNullOrEmpty(serchModel.MeetingType_Name))
            {
                query = query.Where(p => p.MeetingType_Name == serchModel.MeetingType_Name);
            }

            var totalCount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return new PagedListModel<GL_MeetingTypeInfoDTO>(totalCount, query.ToList());
        }

        public GL_MeetingTypeInfoDTO GetGL_MeetingTypeInfo(int uid)
        {
            var query = from meetTypeInfo in DataContext.GL_MeetingTypeInfo
                        where meetTypeInfo.MeetingType_UID == uid
                        select new GL_MeetingTypeInfoDTO
                        {
                            MeetingType_UID = meetTypeInfo.MeetingType_UID,
                            Plant_Organization_UID = meetTypeInfo.Plant_Organization_UID,
                            Plant_Organization_Name = meetTypeInfo.System_Organization2.Organization_Name,
                            BG_Organization_UID = meetTypeInfo.BG_Organization_UID,
                            BG_Organization_Name = meetTypeInfo.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = meetTypeInfo.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = meetTypeInfo.System_Organization.Organization_Name,
                            MeetingType_ID = meetTypeInfo.MeetingType_ID,
                            MeetingType_Name = meetTypeInfo.MeetingType_Name,
                            Modified_UID = meetTypeInfo.Modified_UID,
                            Modified_Date = meetTypeInfo.Modified_Date,
                        };

            return query.FirstOrDefault();
        }

        public string AddGL_MeetingTypeInfo(GL_MeetingTypeInfoDTO addModel)
        {
            try
            {
                var sql = @"  INSERT INTO
                                 [dbo].[GL_MeetingTypeInfo]
                                           ([Plant_Organization_UID]
                                           ,[BG_Organization_UID]
                                           ,[FunPlant_Organization_UID]
                                           ,[MeetingType_ID]
                                           ,[MeetingType_Name]
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
                                      addModel.MeetingType_ID,
                                      addModel.MeetingType_Name,
                                      addModel.Modified_UID,
                                      addModel.Modified_Date.HasValue ? addModel.Modified_Date.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "");
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
        public string DeleteMeetTypeInfo(int meetTypeInfo_UID)
        {
            try
            {
                var sql = $"DELETE FROM dbo.GL_MeetingTypeInfo WHERE MeetingType_UID={meetTypeInfo_UID}";
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
                return "该机台已被使用不能删除";
            }
        }

        public List<GL_MeetingTypeInfoDTO> GetMeetingTypeName(int plantUid, int bgUid, int funplantUid)
        {
            var query = from meetTypeInfo in DataContext.GL_MeetingTypeInfo
                        select new GL_MeetingTypeInfoDTO
                        {
                            MeetingType_UID = meetTypeInfo.MeetingType_UID,
                            Plant_Organization_UID = meetTypeInfo.Plant_Organization_UID,
                            Plant_Organization_Name = meetTypeInfo.System_Organization2.Organization_Name,
                            BG_Organization_UID = meetTypeInfo.BG_Organization_UID,
                            BG_Organization_Name = meetTypeInfo.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = meetTypeInfo.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = meetTypeInfo.System_Organization.Organization_Name,
                            MeetingType_ID = meetTypeInfo.MeetingType_ID,
                            MeetingType_Name = meetTypeInfo.MeetingType_Name,
                            Modified_UID = meetTypeInfo.Modified_UID,
                            Modified_Date = meetTypeInfo.Modified_Date,
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
