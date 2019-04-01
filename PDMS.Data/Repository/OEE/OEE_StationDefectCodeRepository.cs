using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;

namespace PDMS.Data.Repository
{
    public interface IOEE_StationDefectCodeRepository : IRepository<OEE_StationDefectCode>
    {
        List<OEE_StationDefectCodeDTO> QueryStationDefectCode(OEE_StationDefectCodeDTO serchModel, Page page);
        IQueryable<OEE_StationDefectCodeDTO> QueryOEE_StationDefectCode(OEE_StationDefectCodeDTO searchModel, Page page, out int totalcount);
        OEE_StationDefectCodeDTO QueryOEE_StationDefectCodeByUid(int OEE_StationDefectCode_UID);
        List<OEE_StationDefectCodeDTO> QueryOEE_StationDefectCodeList(OEE_StationDefectCodeDTO search);
        List<OEE_StationDefectCodeDTO> GetOEE_StationDefectCodeDTOList(string uids);
        List<OEE_StationDefectCodeDTO> GetAllOEE_StationDefectCodeDTOList();
        string ImportOEE_StationDefectCodekExcel(List<OEE_StationDefectCodeDTO> OEE_DownTimeCodeDTOs);

    }

    public class OEE_StationDefectCodeRepository : RepositoryBase<OEE_StationDefectCode>, IOEE_StationDefectCodeRepository
    {
        public OEE_StationDefectCodeRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public List<OEE_StationDefectCodeDTO> GetAllOEE_StationDefectCodeDTOList()
        {
            var query = from M in DataContext.OEE_StationDefectCode
                        select new OEE_StationDefectCodeDTO
                        {
                            OEE_StationDefectCode_UID = M.OEE_StationDefectCode_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Project_UID = M.Project_UID,
                            LineID = M.LineID,
                            StationID = M.StationID,
                            Sequence = M.Sequence,
                            Defect_Code = M.Defect_Code,
                            DefectEnglishName = M.DefectEnglishName,
                            DefecChinesetName = M.DefecChinesetName,
                            Is_Enable = M.Is_Enable,
                            Modify_UID = M.Modify_UID,
                            Modify_Date = M.Modify_Date,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,

                            Project_Name = M.System_Project.Project_Name,
                            Line_Name = M.GL_Line.LineName,
                            Station_Name = M.GL_Station.StationName,
                            Modifyer = M.System_Users.User_Name
                        };

            return query.ToList();

        }
        public List<OEE_StationDefectCodeDTO> QueryStationDefectCode(OEE_StationDefectCodeDTO serchModel, Page page)
        {
            var query = from def in DataContext.OEE_StationDefectCode
                        where def.Plant_Organization_UID == serchModel.Plant_Organization_UID &&
                        def.Plant_Organization_UID == serchModel.Plant_Organization_UID &&
                        def.BG_Organization_UID == serchModel.BG_Organization_UID &&
                        def.Project_UID == serchModel.Project_UID &&
                        def.LineID == serchModel.LineID &&
                        def.StationID == serchModel.StationID &&
                        def.Is_Enable == true
                        select new OEE_StationDefectCodeDTO
                        {
                            OEE_StationDefectCode_UID = def.OEE_StationDefectCode_UID,
                            Plant_Organization_UID = def.Plant_Organization_UID,
                            Plant_Organization_Name = def.System_Organization.Organization_Name,
                            BG_Organization_UID = def.BG_Organization_UID,
                            BG_Organization_Name = def.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = def.OEE_StationDefectCode_UID,
                            FunPlant_Organization_Name = def.System_Organization2.Organization_Name,
                            Project_Name = def.System_Project.Project_Name,
                            Line_Name = def.GL_Line.LineName,
                            Sequence=def.Sequence,
                            Station_Name = def.GL_Station.StationName,
                            LineID = def.GL_Line.LineID,
                            StationID = def.GL_Station.StationID,
                            Defect_Code = def.Defect_Code,
                            DefectEnglishName = def.DefectEnglishName,
                            DefecChinesetName = def.DefecChinesetName,
                            Is_Enable = def.Is_Enable,
                            Modify_UID = def.Modify_UID,
                            Modify_Date = def.Modify_Date
                        };

            return  query.ToList();
        }

        public IQueryable<OEE_StationDefectCodeDTO> QueryOEE_StationDefectCode(OEE_StationDefectCodeDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.OEE_StationDefectCode
                        where M.GL_Line.IsEnabled==true&& M.GL_Station.IsEnabled==true
                        select new OEE_StationDefectCodeDTO
                        {
                            OEE_StationDefectCode_UID = M.OEE_StationDefectCode_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Project_UID = M.Project_UID,
                            LineID = M.LineID,
                            StationID = M.StationID,
                            Sequence=M.Sequence,
                            Defect_Code = M.Defect_Code,
                            DefectEnglishName = M.DefectEnglishName,
                            DefecChinesetName = M.DefecChinesetName,
                            Is_Enable = M.Is_Enable,
                            Modify_UID = M.Modify_UID,
                            Modify_Date = M.Modify_Date,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            Project_Name = M.System_Project.Project_Name,
                            Line_Name = M.GL_Line.LineName,
                            Station_Name = M.GL_Station.StationName,
                            Modifyer = M.System_Users.User_Name
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);

            if (searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.LineID != 0)
                query = query.Where(m => m.LineID == searchModel.LineID );
            if (searchModel.StationID != 0)
                query = query.Where(m => m.StationID == searchModel.StationID);

            if (!string.IsNullOrWhiteSpace(searchModel.Defect_Code))
                query = query.Where(m => m.Defect_Code == searchModel.Defect_Code);
            if (!string.IsNullOrWhiteSpace(searchModel.DefectEnglishName))
                query = query.Where(m => m.DefectEnglishName == searchModel.DefectEnglishName);
            if (!string.IsNullOrWhiteSpace(searchModel.DefecChinesetName))
                query = query.Where(m => m.DefecChinesetName == searchModel.DefecChinesetName);

            if (!string.IsNullOrWhiteSpace(searchModel.Modifyer))
                query = query.Where(m => m.Modifyer == searchModel.Modifyer);

            if (searchModel.IsEnabled != null)
                query = query.Where(m => m.Is_Enable == searchModel.IsEnabled);

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modify_Date).GetPage(page);
            return query;
        }

        public OEE_StationDefectCodeDTO QueryOEE_StationDefectCodeByUid(int OEE_StationDefectCode_UID)
        {
            var query = from M in DataContext.OEE_StationDefectCode
                        where M.OEE_StationDefectCode_UID == OEE_StationDefectCode_UID
                        select new OEE_StationDefectCodeDTO
                        {
                            OEE_StationDefectCode_UID = M.OEE_StationDefectCode_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Project_UID = M.Project_UID,
                            LineID = M.LineID,
                            StationID = M.StationID,
                            Defect_Code = M.Defect_Code,
                            DefectEnglishName = M.DefectEnglishName,
                            DefecChinesetName = M.DefecChinesetName,
                            Sequence = M.Sequence,
                            Is_Enable = M.Is_Enable,
                            Modify_UID = M.Modify_UID,
                            Modify_Date = M.Modify_Date,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,

                            Project_Name = M.System_Project.Project_Name,
                            Line_Name = M.GL_Line.LineName,
                            Station_Name = M.GL_Station.StationName,
                            Modifyer = M.System_Users.User_Name
                        };

            return query.FirstOrDefault();

        }

        public List<OEE_StationDefectCodeDTO> QueryOEE_StationDefectCodeList(OEE_StationDefectCodeDTO search)
        {
            var query = from M in DataContext.OEE_StationDefectCode
                        where M.GL_Line.IsEnabled == true && M.GL_Station.IsEnabled == true
                        select new OEE_StationDefectCodeDTO
                        {
                            OEE_StationDefectCode_UID = M.OEE_StationDefectCode_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Project_UID = M.Project_UID,
                            LineID = M.LineID,
                            StationID = M.StationID,
                             Sequence=M.Sequence,
                            DefecChinesetName = M.DefecChinesetName,
                            DefectEnglishName = M.DefectEnglishName,
                            Defect_Code = M.Defect_Code,
                            Is_Enable = M.Is_Enable,
                            Modify_UID = M.Modify_UID,
                            Modify_Date = M.Modify_Date,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            Project_Name = M.System_Project.Project_Name,
                            Line_Name = M.GL_Line.LineName,
                            Station_Name = M.GL_Station.StationName,
                            Modifyer = M.System_Users.User_Name
                        };

            if (search.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == search.Plant_Organization_UID);
            if (search.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == search.BG_Organization_UID);
            if (search.FunPlant_Organization_UID != 0 && search.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == search.FunPlant_Organization_UID);

            if (search.Project_UID != 0)
                query = query.Where(m => m.Project_UID == search.Project_UID);
            if (search.LineID != 0)
                query = query.Where(m => m.LineID == search.LineID);
            if (search.StationID != 0)
                query = query.Where(m => m.StationID == search.StationID);

            if (!string.IsNullOrWhiteSpace(search.DefecChinesetName))
                query = query.Where(m => m.DefecChinesetName == search.DefecChinesetName);
            if (!string.IsNullOrWhiteSpace(search.DefectEnglishName))
                query = query.Where(m => m.DefectEnglishName == search.DefectEnglishName);
            if (!string.IsNullOrWhiteSpace(search.Defect_Code))
                query = query.Where(m => m.Defect_Code == search.Defect_Code);

            if (search.IsEnabled != null)
                query = query.Where(m => m.Is_Enable == search.IsEnabled);


            query = query.OrderByDescending(m => m.Modify_Date);
            return query.ToList();
        }

        public List<OEE_StationDefectCodeDTO> GetOEE_StationDefectCodeDTOList(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.OEE_StationDefectCode
                        select new OEE_StationDefectCodeDTO
                        {
                            OEE_StationDefectCode_UID = M.OEE_StationDefectCode_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Project_UID = M.Project_UID,
                            LineID = M.LineID,
                            StationID = M.StationID,
                            Sequence=M.Sequence,
                            DefecChinesetName = M.DefecChinesetName,
                            DefectEnglishName = M.DefectEnglishName,
                            Defect_Code = M.Defect_Code,
                            Is_Enable = M.Is_Enable,
                            Modify_UID = M.Modify_UID,
                            Modify_Date = M.Modify_Date,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            Project_Name = M.System_Project.Project_Name,
                            Line_Name = M.GL_Line.LineName,
                            Station_Name = M.GL_Station.StationName,
                            Modifyer = M.System_Users.User_Name
                        };

            query = query.Where(m => uids.Contains("," + m.OEE_StationDefectCode_UID + ","));
            return query.ToList();
        }

        public string ImportOEE_StationDefectCodekExcel(List<OEE_StationDefectCodeDTO> OEE_DownTimeCodeDTOs)
        {
            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    if (OEE_DownTimeCodeDTOs != null && OEE_DownTimeCodeDTOs.Count > 0)
                    {

                        foreach (var item in OEE_DownTimeCodeDTOs)
                        {

                            if (item.OEE_StationDefectCode_UID == 0)
                            {
                                //构造插入SQL数据
                                var insertSql = string.Format(@"INSERT INTO dbo.OEE_StationDefectCode ( Plant_Organization_UID ,
                                                                BG_Organization_UID ,
                                                                FunPlant_Organization_UID ,
                                                                Project_UID ,
                                                                LineID ,
                                                                StationID ,
                                                                Sequence ,
                                                                Defect_Code ,
                                                                DefectEnglishName ,
                                                                DefecChinesetName ,
                                                                Is_Enable ,
                                                                Modify_UID ,
                                                                Modify_Date )
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
                                                               ,{10}
                                                               ,{11}
                                                               ,'{12}');",
                                                               item.Plant_Organization_UID, item.BG_Organization_UID, item.FunPlant_Organization_UID != null ? item.FunPlant_Organization_UID : -99, item.Project_UID,
                                                               item.LineID ,  item.StationID , item.Sequence,
                                                               item.Defect_Code, item.DefectEnglishName, item.DefecChinesetName, item.Is_Enable ? 1 : 0, item.Modify_UID, item.Modify_Date);
                                insertSql = insertSql.Replace("-99", "NULL");
                                sb.AppendLine(insertSql);
                            }
                            else
                            {

                                var updateSql = string.Format(@" UPDATE OEE_StationDefectCode
                                                               SET Plant_Organization_UID = {0}
                                                                  ,BG_Organization_UID = {1}
                                                                  ,FunPlant_Organization_UID = {2}
                                                                  ,Project_UID = {3}
                                                                  ,LineID ={4}
                                                                  ,StationID = {5}
                                                                  ,Sequence = 6
                                                                  ,Defect_Code =N'{7}'
                                                                  ,DefectEnglishName =N'{8}'
                                                                  ,DefecChinesetName = N'{9}'
                                                                  ,Is_Enable = {10}
                                                                  ,Modify_UID ={11}
                                                                  ,Modify_Date = '{12}'
                                                             WHERE OEE_StationDefectCode_UID={13};",
                                                             item.Plant_Organization_UID, item.BG_Organization_UID, item.FunPlant_Organization_UID != null ? item.FunPlant_Organization_UID : -99,item.Project_UID,
                                                             item.LineID, item.StationID, item.Sequence, item.Defect_Code, item.DefectEnglishName, item.DefecChinesetName,
                                                             item.Is_Enable ? 1 : 0, item.Modify_UID, item.Modify_Date, item.OEE_StationDefectCode_UID);
                                updateSql = updateSql.Replace("-99", "NULL");
                                sb.AppendLine(updateSql);

                            }
                        }
                        string sql = sb.ToString();
                        DataContext.Database.ExecuteSqlCommand(sb.ToString());
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
    }
}
