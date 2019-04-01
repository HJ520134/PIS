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

    public interface IOEE_DownTimeCodeRepository : IRepository<OEE_DownTimeCode>
    {
        IQueryable<OEE_DownTimeCodeDTO> QueryOEE_DownTimeCodes(OEE_DownTimeCodeDTO searchModel, Page page, out int totalcount);

        List<EnumerationDTO> GetEnumerationDTO(string Enum_Type, string Enum_Name);

        List<SystemProjectDTO> GetSystemProjectDTO(int Plant_Organization_UID, int BG_Organization_UID);
        List<OEE_DownTypeDTO> GetOEE_DownTypeDTO(int Plant_Organization_UID);
        OEE_DownTimeCodeDTO QueryDownTimeCodeByUid(int OEE_DownTimeCode_UID);
        List<OEE_DownTimeCodeDTO> QueryDownTimeCodeList(OEE_DownTimeCodeDTO search);
        List<OEE_DownTimeCodeDTO> GetDownTimeCodeDTOList(string uids);
        List<GL_LineDTO> GetAllGL_LineDTOList();
        List<GL_StationDTO> GetAllGL_StationDTOList();
        List<OEE_DownTimeCodeDTO> GetAllOEE_DownTimeCodeDTOList();
        string ImportOEE_DownTimeCodekExcel(List<OEE_DownTimeCodeDTO> OEE_DownTimeCodeDTOs);

        List<GL_StationDTO> GetStationDTOs(int CustomerID);

        List<GL_LineDTO> GetOEELineDTO(int CustomerID);
    }

    public class OEE_DownTimeCodeRepository : RepositoryBase<OEE_DownTimeCode>, IOEE_DownTimeCodeRepository
    {

        public OEE_DownTimeCodeRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }


        public IQueryable<OEE_DownTimeCodeDTO> QueryOEE_DownTimeCodes(OEE_DownTimeCodeDTO searchModel, Page page, out int totalcount)
        {
            var query = (from M in DataContext.OEE_DownTimeCode
                         where M.GL_Line.IsEnabled == true && M.GL_Station.IsEnabled == true
                         select new OEE_DownTimeCodeDTO
                         {
                             OEE_DownTimeCode_UID = M.OEE_DownTimeCode_UID,
                             Plant_Organization_UID = M.Plant_Organization_UID,
                             BG_Organization_UID = M.BG_Organization_UID,
                             FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                             OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                             Project_UID = M.Project_UID,
                             LineID = M.LineID,
                             StationID = M.StationID,
                             Error_Code = M.Error_Code,
                             Upload_Ways = M.Upload_Ways,
                             Level_Details = M.Level_Details,
                             Error_Reasons = M.Error_Reasons,
                             Remarks = M.Remarks,
                             Is_Enable = M.Is_Enable,
                             Modify_UID = M.Modify_UID,
                             Modify_Date = M.Modify_Date,
                             Plant_Organization_Name = M.System_Organization.Organization_Name,
                             BG_Organization_Name = M.System_Organization1.Organization_Name,
                             FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                             EnumDownTimeCodeType = M.OEE_DownTimeType.Type_Name,
                             ProjectName = M.System_Project.Project_Name,
                             LineName = M.GL_Line.LineName,
                             StationName = M.GL_Station.StationName,
                             Modifyer = M.System_Users.User_Name
                         }).Union(

                from M in DataContext.OEE_DownTimeCode
                where M.GL_Line.IsEnabled == true && M.GL_Station.IsEnabled != true
                select new OEE_DownTimeCodeDTO
                {
                    OEE_DownTimeCode_UID = M.OEE_DownTimeCode_UID,
                    Plant_Organization_UID = M.Plant_Organization_UID,
                    BG_Organization_UID = M.BG_Organization_UID,
                    FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                    OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                    Project_UID = M.Project_UID,
                    LineID = M.LineID,
                    StationID = M.StationID,
                    Error_Code = M.Error_Code,
                    Upload_Ways = M.Upload_Ways,
                    Level_Details = M.Level_Details,
                    Error_Reasons = M.Error_Reasons,
                    Remarks = M.Remarks,
                    Is_Enable = M.Is_Enable,
                    Modify_UID = M.Modify_UID,
                    Modify_Date = M.Modify_Date,
                    Plant_Organization_Name = M.System_Organization.Organization_Name,
                    BG_Organization_Name = M.System_Organization1.Organization_Name,
                    FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                    EnumDownTimeCodeType = M.OEE_DownTimeType.Type_Name,
                    ProjectName = M.System_Project.Project_Name,
                    LineName = M.GL_Line.LineName,
                    StationName = M.GL_Station.StationName,
                    Modifyer = M.System_Users.User_Name
                }).Union(
                      from M in DataContext.OEE_DownTimeCode
                      where M.GL_Line == null && M.GL_Station == null
                      select new OEE_DownTimeCodeDTO
                      {
                          OEE_DownTimeCode_UID = M.OEE_DownTimeCode_UID,
                          Plant_Organization_UID = M.Plant_Organization_UID,
                          BG_Organization_UID = M.BG_Organization_UID,
                          FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                          OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                          Project_UID = M.Project_UID,
                          LineID = M.LineID,
                          StationID = M.StationID,
                          Error_Code = M.Error_Code,
                          Upload_Ways = M.Upload_Ways,
                          Level_Details = M.Level_Details,
                          Error_Reasons = M.Error_Reasons,
                          Remarks = M.Remarks,
                          Is_Enable = M.Is_Enable,
                          Modify_UID = M.Modify_UID,
                          Modify_Date = M.Modify_Date,
                          Plant_Organization_Name = M.System_Organization.Organization_Name,
                          BG_Organization_Name = M.System_Organization1.Organization_Name,
                          FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                          EnumDownTimeCodeType = M.OEE_DownTimeType.Type_Name,
                          ProjectName = M.System_Project.Project_Name,
                          LineName = M.GL_Line.LineName,
                          StationName = M.GL_Station.StationName,
                          Modifyer = M.System_Users.User_Name
                      }
                );


            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.OEE_DownTimeType_UID != 0)
                query = query.Where(m => m.OEE_DownTimeType_UID == searchModel.OEE_DownTimeType_UID);
            if (searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.LineID != 0 && searchModel.LineID != null)
                query = query.Where(m => m.LineID == searchModel.LineID);
            if (searchModel.StationID != 0 && searchModel.StationID != null)
                query = query.Where(m => m.StationID == searchModel.StationID);

            if (!string.IsNullOrWhiteSpace(searchModel.Error_Code))
                query = query.Where(m => m.Error_Code == searchModel.Error_Code);
            if (!string.IsNullOrWhiteSpace(searchModel.Upload_Ways))
                query = query.Where(m => m.Upload_Ways == searchModel.Upload_Ways);
            if (!string.IsNullOrWhiteSpace(searchModel.Level_Details))
                query = query.Where(m => m.Level_Details == searchModel.Level_Details);
            if (!string.IsNullOrWhiteSpace(searchModel.Error_Reasons))
                query = query.Where(m => m.Error_Reasons == searchModel.Error_Reasons);
            if (!string.IsNullOrWhiteSpace(searchModel.Modifyer))
                query = query.Where(m => m.Modifyer == searchModel.Modifyer);

            //if (searchModel.Applicant_Date.Year != 1)
            //{
            //    DateTime nextday = searchModel.Applicant_Date.AddDays(1);
            //    query = query.Where(m => m.Applicant_Date >= searchModel.Applicant_Date & m.Applicant_Date < nextday);
            //}
            //if (searchModel.Approver_Date.Year != 1)
            //{
            //    DateTime nextday = searchModel.Approver_Date.AddDays(1);
            //    query = query.Where(m => m.Approver_Date >= searchModel.Approver_Date & m.Applicant_Date < nextday);
            //}
            if (searchModel.IsEnabled != null)
                query = query.Where(m => m.Is_Enable == searchModel.IsEnabled);
            //query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modify_Date).GetPage(page);
            return query;
        }

        public List<EnumerationDTO> GetEnumerationDTO(string Enum_Type, string Enum_Name)
        {

            var query = from M in DataContext.Enumeration
                        select new EnumerationDTO
                        {
                            Enum_UID = M.Enum_UID,
                            Enum_Type = M.Enum_Type,
                            Enum_Name = M.Enum_Name,
                            Enum_Value = M.Enum_Value,
                            Decription = M.Decription
                        };

            if (!string.IsNullOrWhiteSpace(Enum_Type))
                query = query.Where(m => m.Enum_Type == Enum_Type);
            if (!string.IsNullOrWhiteSpace(Enum_Name))
                query = query.Where(m => m.Enum_Name == Enum_Name);
            return query.ToList();
        }

        public List<SystemProjectDTO> GetSystemProjectDTO(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var query = from M in DataContext.System_Project
                        select new SystemProjectDTO
                        {
                            Project_UID = M.Project_UID,
                            Project_Code = M.Project_Code,
                            OP_TYPES = M.OP_TYPES,
                            BU_D_UID = M.BU_D_UID,
                            Project_Name = M.Project_Name,
                            MESProject_Name = M.MESProject_Name,
                            Product_Phase = M.Product_Phase,
                            Organization_UID = M.Organization_UID,
                            Project_Type = M.Project_Type
                        };
            query = query.Where(p => p.MESProject_Name != null && p.MESProject_Name != "");
            if (BG_Organization_UID != 0)
            {
                query = query.Where(m => m.Organization_UID == BG_Organization_UID);
            }
            else
            {
                List<int> OpTypeIDs = GetOpTypeID(Plant_Organization_UID);
                query = query.Where(m => OpTypeIDs.Contains(m.Organization_UID));
            }

            return query.ToList();
        }

        public List<OEE_DownTypeDTO> GetOEE_DownTypeDTO(int Plant_Organization_UID)
        {

            var query = from M in DataContext.OEE_DownTimeType
                        where M.Is_Enable == true
                        select new OEE_DownTypeDTO
                        {
                            OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            Type_Name = M.Type_Name,
                            Is_Enable = M.Is_Enable,
                            Modify_UID = M.Modify_UID,
                            Modify_Date = M.Modify_Date,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            Sequence = M.Sequence,
                            Modifyer = M.System_Users.User_Name,
                            Type_Code = M.Type_Code
                        };
            if (Plant_Organization_UID != 0)
            {
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            }
            return query.ToList();

        }
        public List<int> GetOpTypeID(int Plant_Organization_UID)
        {

            List<int> OpTypeIDs = new List<int>();
            if (Plant_Organization_UID != 0)
            {
                OpTypeIDs = DataContext.System_OrganizationBOM.Where(o => o.ParentOrg_UID == Plant_Organization_UID).Select(o => o.ChildOrg_UID).ToList();
            }
            else
            {
                List<int> ParentOrg_UIDs = DataContext.System_Organization.Where(o => o.Organization_ID.StartsWith("1000")).Select(o => o.Organization_UID).ToList();
                OpTypeIDs = DataContext.System_OrganizationBOM.Where(o => ParentOrg_UIDs.Contains(o.ParentOrg_UID.Value)).Select(o => o.ChildOrg_UID).ToList();
            }

            return OpTypeIDs;

        }

        public OEE_DownTimeCodeDTO QueryDownTimeCodeByUid(int OEE_DownTimeCode_UID)
        {
            var query = from M in DataContext.OEE_DownTimeCode
                        select new OEE_DownTimeCodeDTO
                        {
                            OEE_DownTimeCode_UID = M.OEE_DownTimeCode_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                            Project_UID = M.Project_UID,
                            LineID = M.LineID,
                            StationID = M.StationID,
                            Error_Code = M.Error_Code,
                            Upload_Ways = M.Upload_Ways,
                            Level_Details = M.Level_Details,
                            Error_Reasons = M.Error_Reasons,
                            Remarks = M.Remarks,
                            Is_Enable = M.Is_Enable,
                            Modify_UID = M.Modify_UID,
                            Modify_Date = M.Modify_Date,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            EnumDownTimeCodeType = M.OEE_DownTimeType.Type_Name,
                            ProjectName = M.System_Project.Project_Name,
                            LineName = M.GL_Line.LineName,
                            StationName = M.GL_Station.StationName,
                            Modifyer = M.System_Users.User_Name
                        };

            query = query.Where(m => m.OEE_DownTimeCode_UID == OEE_DownTimeCode_UID);
            return query.FirstOrDefault();

        }

        public List<OEE_DownTimeCodeDTO> QueryDownTimeCodeList(OEE_DownTimeCodeDTO search)
        {
            var query = (from M in DataContext.OEE_DownTimeCode
                         where M.GL_Line.IsEnabled == true && M.GL_Station.IsEnabled == true
                         select new OEE_DownTimeCodeDTO
                         {
                             OEE_DownTimeCode_UID = M.OEE_DownTimeCode_UID,
                             Plant_Organization_UID = M.Plant_Organization_UID,
                             BG_Organization_UID = M.BG_Organization_UID,
                             FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                             OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                             Project_UID = M.Project_UID,
                             LineID = M.LineID,
                             StationID = M.StationID,
                             Error_Code = M.Error_Code,
                             Upload_Ways = M.Upload_Ways,
                             Level_Details = M.Level_Details,
                             Error_Reasons = M.Error_Reasons,
                             Remarks = M.Remarks,
                             Is_Enable = M.Is_Enable,
                             Modify_UID = M.Modify_UID,
                             Modify_Date = M.Modify_Date,
                             Plant_Organization_Name = M.System_Organization.Organization_Name,
                             BG_Organization_Name = M.System_Organization1.Organization_Name,
                             FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                             EnumDownTimeCodeType = M.OEE_DownTimeType.Type_Name,
                             ProjectName = M.System_Project.Project_Name,
                             LineName = M.GL_Line.LineName,
                             StationName = M.GL_Station.StationName,
                             Modifyer = M.System_Users.User_Name
                         }).Union(

                from M in DataContext.OEE_DownTimeCode
                where M.GL_Line.IsEnabled == true && M.GL_Station.IsEnabled != true
                select new OEE_DownTimeCodeDTO
                {
                    OEE_DownTimeCode_UID = M.OEE_DownTimeCode_UID,
                    Plant_Organization_UID = M.Plant_Organization_UID,
                    BG_Organization_UID = M.BG_Organization_UID,
                    FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                    OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                    Project_UID = M.Project_UID,
                    LineID = M.LineID,
                    StationID = M.StationID,
                    Error_Code = M.Error_Code,
                    Upload_Ways = M.Upload_Ways,
                    Level_Details = M.Level_Details,
                    Error_Reasons = M.Error_Reasons,
                    Remarks = M.Remarks,
                    Is_Enable = M.Is_Enable,
                    Modify_UID = M.Modify_UID,
                    Modify_Date = M.Modify_Date,
                    Plant_Organization_Name = M.System_Organization.Organization_Name,
                    BG_Organization_Name = M.System_Organization1.Organization_Name,
                    FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                    EnumDownTimeCodeType = M.OEE_DownTimeType.Type_Name,
                    ProjectName = M.System_Project.Project_Name,
                    LineName = M.GL_Line.LineName,
                    StationName = M.GL_Station.StationName,
                    Modifyer = M.System_Users.User_Name
                }).Union(
                      from M in DataContext.OEE_DownTimeCode
                      where M.GL_Line == null && M.GL_Station == null
                      select new OEE_DownTimeCodeDTO
                      {
                          OEE_DownTimeCode_UID = M.OEE_DownTimeCode_UID,
                          Plant_Organization_UID = M.Plant_Organization_UID,
                          BG_Organization_UID = M.BG_Organization_UID,
                          FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                          OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                          Project_UID = M.Project_UID,
                          LineID = M.LineID,
                          StationID = M.StationID,
                          Error_Code = M.Error_Code,
                          Upload_Ways = M.Upload_Ways,
                          Level_Details = M.Level_Details,
                          Error_Reasons = M.Error_Reasons,
                          Remarks = M.Remarks,
                          Is_Enable = M.Is_Enable,
                          Modify_UID = M.Modify_UID,
                          Modify_Date = M.Modify_Date,
                          Plant_Organization_Name = M.System_Organization.Organization_Name,
                          BG_Organization_Name = M.System_Organization1.Organization_Name,
                          FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                          EnumDownTimeCodeType = M.OEE_DownTimeType.Type_Name,
                          ProjectName = M.System_Project.Project_Name,
                          LineName = M.GL_Line.LineName,
                          StationName = M.GL_Station.StationName,
                          Modifyer = M.System_Users.User_Name
                      }
                );

            if (search.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == search.Plant_Organization_UID);
            if (search.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == search.BG_Organization_UID);
            if (search.FunPlant_Organization_UID != 0 && search.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == search.FunPlant_Organization_UID);
            if (search.OEE_DownTimeType_UID != 0)
                query = query.Where(m => m.OEE_DownTimeType_UID == search.OEE_DownTimeType_UID);
            if (search.Project_UID != 0)
                query = query.Where(m => m.Project_UID == search.Project_UID);
            if (search.LineID != 0 && search.LineID != null)
                query = query.Where(m => m.LineID == search.LineID);
            if (search.StationID != 0 && search.StationID != null)
                query = query.Where(m => m.StationID == search.StationID);

            if (!string.IsNullOrWhiteSpace(search.Error_Code))
                query = query.Where(m => m.Error_Code == search.Error_Code);
            if (!string.IsNullOrWhiteSpace(search.Upload_Ways))
                query = query.Where(m => m.Upload_Ways == search.Upload_Ways);
            if (!string.IsNullOrWhiteSpace(search.Level_Details))
                query = query.Where(m => m.Level_Details == search.Level_Details);
            if (!string.IsNullOrWhiteSpace(search.Error_Reasons))
                query = query.Where(m => m.Error_Reasons == search.Error_Reasons);
            if (search.IsEnabled != null)
                query = query.Where(m => m.Is_Enable == search.IsEnabled);


            query = query.OrderByDescending(m => m.Modify_Date);
            return query.ToList();
        }

        public List<OEE_DownTimeCodeDTO> GetDownTimeCodeDTOList(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.OEE_DownTimeCode
                        select new OEE_DownTimeCodeDTO
                        {
                            OEE_DownTimeCode_UID = M.OEE_DownTimeCode_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                            Project_UID = M.Project_UID,
                            LineID = M.LineID,
                            StationID = M.StationID,
                            Error_Code = M.Error_Code,
                            Upload_Ways = M.Upload_Ways,
                            Level_Details = M.Level_Details,
                            Error_Reasons = M.Error_Reasons,
                            Remarks = M.Remarks,
                            Is_Enable = M.Is_Enable,
                            Modify_UID = M.Modify_UID,
                            Modify_Date = M.Modify_Date,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            EnumDownTimeCodeType = M.OEE_DownTimeType.Type_Name,
                            ProjectName = M.System_Project.Project_Name,
                            LineName = M.GL_Line.LineName,
                            StationName = M.GL_Station.StationName,
                            Modifyer = M.System_Users.User_Name
                        };

            query = query.Where(m => uids.Contains("," + m.OEE_DownTimeCode_UID + ","));
            return query.ToList();
        }
        public List<GL_LineDTO> GetAllGL_LineDTOList()
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
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID
                        };

            query = query.Where(o => o.IsEnabled == true);
            return query.ToList();
        }

        public List<GL_StationDTO> GetAllGL_StationDTOList()
        {
            var query = from w in DataContext.GL_Station
                        select new GL_StationDTO
                        {

                            StationID = w.StationID,
                            StationName = w.StationName.Trim(),
                            LineID = w.LineID,
                            IsBirth = w.IsBirth,
                            IsOutput = w.IsOutput,
                            IsTest = w.IsTest,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            CustomerID = w.GL_Line.CustomerID,
                            ProjectName = w.GL_Line.System_Project.Project_Name.Trim(),
                            LineName = w.GL_Line.LineName.Trim(),
                            LineIsEnabled = w.GL_Line.IsEnabled,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            LineCycleTime = w.GL_Line.CycleTime,
                            CycleTime = w.CycleTime,
                            MESStationName = w.MESStationName.Trim(),
                            MESLineName = w.GL_Line.MESLineName.Trim(),
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name.Trim(),
                            Binding_Seq = w.Binding_Seq,
                            IsGoldenLine = w.IsGoldenLine,
                            IsOEE = w.IsOEE,
                            IsOne = w.IsOne,
                            IsTwo = w.IsTwo,
                            IsThree = w.IsThree,
                            IsFour = w.IsFour,
                            IsFive = w.IsFive
                        };

            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.IsOEE == true);
            query = query.Where(o => o.LineIsEnabled == true);
            return query.ToList();
        }

        public List<OEE_DownTimeCodeDTO> GetAllOEE_DownTimeCodeDTOList()
        {

            var query = from M in DataContext.OEE_DownTimeCode
                        select new OEE_DownTimeCodeDTO
                        {
                            OEE_DownTimeCode_UID = M.OEE_DownTimeCode_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                            Project_UID = M.Project_UID,
                            LineID = M.LineID,
                            StationID = M.StationID,
                            Error_Code = M.Error_Code,
                            Upload_Ways = M.Upload_Ways,
                            Level_Details = M.Level_Details,
                            Error_Reasons = M.Error_Reasons,
                            Remarks = M.Remarks,
                            Is_Enable = M.Is_Enable,
                            Modify_UID = M.Modify_UID,
                            Modify_Date = M.Modify_Date,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            EnumDownTimeCodeType = M.OEE_DownTimeType.Type_Name,
                            ProjectName = M.System_Project.Project_Name,
                            LineName = M.GL_Line.LineName,
                            StationName = M.GL_Station.StationName,
                            Modifyer = M.System_Users.User_Name
                        };

            return query.ToList();

        }

        public string ImportOEE_DownTimeCodekExcel(List<OEE_DownTimeCodeDTO> OEE_DownTimeCodeDTOs)
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

                            if (item.OEE_DownTimeCode_UID == 0)
                            {
                                //构造插入SQL数据
                                var insertSql = string.Format(@"INSERT INTO OEE_DownTimeCode
                                                               (Plant_Organization_UID
                                                               ,BG_Organization_UID
                                                               ,FunPlant_Organization_UID
                                                               ,OEE_DownTimeType_UID
                                                               ,Project_UID
                                                               ,LineID
                                                               ,StationID
                                                               ,Error_Code
                                                               ,Upload_Ways
                                                               ,Level_Details
                                                               ,Error_Reasons
                                                               ,Is_Enable
                                                               ,Modify_UID
                                                               ,Modify_Date)
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
                                                               ,{11}
                                                               ,{12}
                                                               ,'{13}');",
                                                               item.Plant_Organization_UID, item.BG_Organization_UID, item.FunPlant_Organization_UID != null ? item.FunPlant_Organization_UID : -99, item.OEE_DownTimeType_UID, item.Project_UID,
                                                               item.LineID != null ? item.LineID : -99, item.StationID != null ? item.StationID : -99, item.Error_Code,
                                                               item.Upload_Ways, item.Level_Details, item.Error_Reasons, item.Is_Enable ? 1 : 0, item.Modify_UID, item.Modify_Date);
                                insertSql = insertSql.Replace("-99", "NULL");
                                sb.AppendLine(insertSql);
                            }
                            else
                            {

                                var updateSql = string.Format(@" UPDATE OEE_DownTimeCode
                                                               SET Plant_Organization_UID = {0}
                                                                  ,BG_Organization_UID = {1}
                                                                  ,FunPlant_Organization_UID = {2}
                                                                  ,OEE_DownTimeType_UID = {3}
                                                                  ,Project_UID = {4}
                                                                  ,LineID ={5}
                                                                  ,StationID = {6}
                                                                  ,Error_Code = N'{7}'
                                                                  ,Upload_Ways =N'{8}'
                                                                  ,Level_Details =N'{9}'
                                                                  ,Error_Reasons = N'{10}'
                                                                  ,Is_Enable = {11}
                                                                  ,Modify_UID ={12}
                                                                  ,Modify_Date = '{13}'
                                                             WHERE OEE_DownTimeCode_UID={14};",
                                                             item.Plant_Organization_UID, item.BG_Organization_UID, item.FunPlant_Organization_UID != null ? item.FunPlant_Organization_UID : -99, item.OEE_DownTimeType_UID, item.Project_UID,
                                                             item.LineID != null ? item.LineID : -99, item.StationID != null ? item.StationID : -99, item.Error_Code, item.Upload_Ways, item.Level_Details, item.Error_Reasons,
                                                             item.Is_Enable ? 1 : 0, item.Modify_UID, item.Modify_Date, item.OEE_DownTimeCode_UID);
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

        public List<GL_StationDTO> GetStationDTOs(int LineID)
        {
            var query = from w in DataContext.GL_Station
                        select new GL_StationDTO
                        {

                            StationID = w.StationID,
                            StationName = w.StationName,
                            LineID = w.LineID,
                            IsBirth = w.IsBirth,
                            IsOutput = w.IsOutput,
                            IsTest = w.IsTest,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            CustomerID = w.GL_Line.CustomerID,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            LineName = w.GL_Line.LineName,
                            LineIsEnabled = w.GL_Line.IsEnabled,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            LineCycleTime = w.GL_Line.CycleTime,
                            CycleTime = w.CycleTime,
                            MESStationName = w.MESStationName,
                            MESLineName = w.GL_Line.MESLineName,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name,
                            Binding_Seq = w.Binding_Seq,
                            IsGoldenLine = w.IsGoldenLine,
                            IsOEE = w.IsOEE,
                            IsOne = w.IsOne,
                            IsTwo = w.IsTwo,
                            IsThree = w.IsThree,
                            IsFour = w.IsFour,
                            IsFive = w.IsFive
                        };

            query = query.Where(o => o.IsOEE == true);
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.LineIsEnabled == true);
            query = query.Where(o => o.LineID == LineID);
            return query.ToList();

        }

        public List<GL_LineDTO> GetOEELineDTO(int CustomerID)
        {
            var query = from w in DataContext.GL_Station
                        where w.IsEnabled == true && w.IsOEE == true 
                        select new GL_LineDTO
                        {
                            LineID = w.LineID,
                            LineName = w.GL_Line.LineName,
                            CustomerID = w.GL_Line.CustomerID,
                            Seq = w.GL_Line.Seq,
                            IsEnabled = w.GL_Line.IsEnabled,
                            Modified_UID = w.GL_Line.Modified_UID,
                            Modified_Date = w.GL_Line.Modified_Date,
                            CycleTime = w.GL_Line.CycleTime,
                            Plant_Organization_UID = w.GL_Line.Plant_Organization_UID,
                            BG_Organization_UID = w.GL_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = w.GL_Line.FunPlant_Organization_UID,
                            Plant_Organization = w.GL_Line.System_Organization.Organization_Name,
                            BG_Organization = w.GL_Line.System_Organization1.Organization_Name,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name
                        };
            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.CustomerID == CustomerID);
            return query.Distinct().ToList();
        }
    }
}
