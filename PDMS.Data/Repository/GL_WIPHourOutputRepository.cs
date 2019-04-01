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

    public interface IGL_WIPHourOutputRepository : IRepository<GL_WIPHourOutput>
    {
        GL_WIPHourOutput GetGL_WIPHourOutputBy(GL_WIPHourOutputDTO WIPHourOutputL);
        bool AddGL_WIPHourOutput(List<GL_WIPHourOutputDTO> WIPHourOutputList);
        bool UpDateGL_WIPHourOutput(List<GL_WIPHourOutputDTO> WIPHourOutputList);

        List<GL_ShiftTimeDTO> GetShiftTimeList(int BG_Organization_UID);
        List<SystemProjectDTO> GetAllEnGL_Customer();
        SystemProjectDTO GetEnGL_Customer(string ProjectName);
        List<SystemProjectPlantDTO> GetAllMachineGL_Customer();
        GL_StationDTO GetStation(GL_StationDTO stationParam);

        List<GL_WIPHourOutputDTO> QueryWIPHourOutput(GL_WIPHourOutputDTO model);

        GL_ShiftTimeDTO GetShiftTime(int ShiftTimeID);

        List<GL_WIPHourOutputDTO> ExportWIPHourOutput(GL_WIPHourOutputDTO model);



        //获取MESProjectName
        GL_WIPHourOutputDTO GetMESProjectNameByID(int Project_UID);

        //获取LineName
        GL_WIPHourOutputDTO GetMESLineNameByID(int LineID);

        GL_WIPHourOutputDTO GetStationNameByID(string stationName);



        List<GL_StationDTO> GetAllStationDTO(int Project_UID);
        List<GL_StationDTO> GetAllMesStationDTO(int Project_UID);
        List<GL_WIPHourOutputDTO> GetGL_WIPHourOutputDTOs(int StationID, string OutputDate, List<int> HourIndexs, int ShiftTimeID);
    }

    public class GL_WIPHourOutputRepository : RepositoryBase<GL_WIPHourOutput>, IGL_WIPHourOutputRepository
    {
        public GL_WIPHourOutputRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        { }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="WIPHourOutputL"></param>
        /// <returns></returns>
        public GL_WIPHourOutput GetGL_WIPHourOutputBy(GL_WIPHourOutputDTO WIPHourOutputL)
        {
            var query = from wipput in DataContext.GL_WIPHourOutput
                        where wipput.GL_Station.IsEnabled == true && wipput.GL_Station.IsGoldenLine == true && wipput.GL_Line.IsEnabled == true
                        select wipput;
            query = query.Where(p => p.ShiftDate == WIPHourOutputL.ShiftDate && p.CustomerID == WIPHourOutputL.CustomerID &&
                       p.LineID == WIPHourOutputL.LineID && p.StationID == WIPHourOutputL.StationID && p.ShiftTimeID == WIPHourOutputL.ShiftTimeID && p.HourIndex == WIPHourOutputL.HourIndex);

            if (query.Count() > 0)
            {
                return query.FirstOrDefault();
            }
            else
            {
                return null;
            }
            //new GL_WIPHourOutputDTO()
            //{
            //    WHOID = wipput.WHOID
            //,
            //    CustomerID = wipput.CustomerID
            //,
            //    LineID = wipput.LineID
            //,
            //    StationID = wipput.StationID
            //,
            //    AssemblyID = wipput.AssemblyID
            //,
            //    OutputDate = wipput.OutputDate
            //,
            //    ShiftTimeID = wipput.ShiftTimeID
            //,
            //    HourIndex = wipput.HourIndex
            //,
            //    StandOutput = wipput.StandOutput
            //,
            //    ActualOutput = wipput.ActualOutput
            //};

        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="WIPHourOutputList"></param>
        /// <returns></returns>
        public bool AddGL_WIPHourOutput(List<GL_WIPHourOutputDTO> WIPHourOutputList)
        {
            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in WIPHourOutputList)
                    {
                        var sql = @" INSERT INTO dbo.GL_WIPHourOutput
                                      ( CustomerID ,
                                        LineID ,
                                        StationID ,                                     
                                        OutputDate ,
                                        ShiftTimeID ,
                                        HourIndex ,
                                        StandOutput ,
                                        ActualOutput,
                                        shiftdate
                                      )
                                 VALUES 
                                     (      {0} , -- CustomerID - int
                                            {1}, -- LineID - int
                                            {2} , -- StationID - int
                                            '{3}', -- OutputDate - datetime
                                            {4} , -- ShiftTimeID - int
                                            {5} , -- HourIndex - int
                                            {6} , -- StandOutput - int
                                            {7},   -- ActualOutput - int
                                            '{8}'  -- shiftdate - string 
                                       )";
                        sql = string.Format(sql, item.CustomerID, item.LineID, item.StationID, item.OutputDate, item.ShiftTimeID, item.HourIndex, item.StandOutput, item.ActualOutput, item.ShiftDate);
                        sb.AppendLine(sql);
                    }
                    DataContext.Database.ExecuteSqlCommand(sb.ToString());
                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="WIPHourOutputList"></param>
        /// <returns></returns>
        public bool UpDateGL_WIPHourOutput(List<GL_WIPHourOutputDTO> WIPHourOutputList)
        {
            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in WIPHourOutputList)
                    {
                        var sql = $" UPDATE GL_WIPHourOutput SET ActualOutput={item.ActualOutput} where WHOID={item.WHOID}";
                        sb.AppendLine(sql.ToString());
                    }

                    DataContext.Database.ExecuteSqlCommand(sb.ToString());
                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //获取Shift
        public List<GL_ShiftTimeDTO> GetShiftTimeList(int BG_Organization_UID)
        {
            var query = from w in DataContext.GL_ShiftTime
                        select new GL_ShiftTimeDTO
                        {
                            ShiftTimeID = w.ShiftTimeID,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            Shift = w.Shift,
                            StartTime = w.StartTime,
                            End_Time = w.End_Time,
                            IsEnabled = w.IsEnabled,
                            Break_Time=w.Break_Time,
                            Sequence=w.Sequence
                        };

            if (BG_Organization_UID > 0)
            {
                query = query.Where(w => w.BG_Organization_UID == BG_Organization_UID);
            }
            query = query.Where(o => o.IsEnabled == true).OrderBy(O=>O.Sequence);
            return query.ToList();
        }

        public SystemProjectDTO GetEnGL_Customer(string ProjectName)
        {
            var query = from w in DataContext.System_Project
                        where w.MESProject_Name != null && w.MESProject_Name == ProjectName
                        select new SystemProjectDTO
                        {
                            Project_UID = w.Project_UID,
                            Project_Code = w.Project_Code,
                            BU_D_UID = w.BU_D_UID,
                            Project_Name = w.Project_Name,
                            MESProject_Name = w.MESProject_Name,
                            Product_Phase = w.Product_Phase,
                            Start_Date = w.Start_Date,
                            Closed_Date = w.Closed_Date,
                            OP_TYPES = w.OP_TYPES,
                            Organization_UID = w.Organization_UID,
                            Project_Type = w.Project_Type,
                            Modified_UID = w.Modified_UID,
                            Modified_Date = w.Modified_Date
                        };

            return query.FirstOrDefault();
        }

        public List<SystemProjectDTO> GetAllEnGL_Customer()
        {
            var query = from w in DataContext.System_Project
                        where w.MESProject_Name != null && w.MESProject_Name != ""
                        select new SystemProjectDTO
                        {
                            Project_UID = w.Project_UID,
                            Project_Code = w.Project_Code,
                            BU_D_UID = w.BU_D_UID,
                            Project_Name = w.Project_Name,
                            MESProject_Name = w.MESProject_Name,
                            Product_Phase = w.Product_Phase,
                            Start_Date = w.Start_Date,
                            Closed_Date = w.Closed_Date,
                            OP_TYPES = w.OP_TYPES,
                            Organization_UID = w.Organization_UID,
                            Project_Type = w.Project_Type,
                            Modified_UID = w.Modified_UID,
                            Modified_Date = w.Modified_Date
                        };

            return query.ToList();
        }

        public List<SystemProjectPlantDTO> GetAllMachineGL_Customer()
        {
            var query = from w in DataContext.System_Project
                        join orgBom in DataContext.System_OrganizationBOM
                        on  w.Organization_UID equals orgBom.ChildOrg_UID
                        join org in DataContext.System_Organization
                        on orgBom.ParentOrg_UID equals org.Organization_UID
                        where w.MESProject_Name != null && w.MESProject_Name != ""
                        select new SystemProjectPlantDTO
                        {
                            Project_UID = w.Project_UID,
                            Plant_UID=org.Organization_UID,
                            Project_Code = w.Project_Code,
                            BU_D_UID = w.BU_D_UID,
                            Plant=org.Organization_Name,
                            Project_Name = w.Project_Name,
                            MESProject_Name = w.MESProject_Name,
                            Product_Phase = w.Product_Phase,
                            Start_Date = w.Start_Date,
                            Closed_Date = w.Closed_Date,
                            OP_TYPES = w.OP_TYPES,
                            Organization_UID = w.Organization_UID,
                            Project_Type = w.Project_Type,
                            Modified_UID = w.Modified_UID,
                            Modified_Date = w.Modified_Date
                        };

            return query.ToList();
        }
        public GL_StationDTO GetStation(GL_StationDTO stationParam)
        {
            var query = from Station in DataContext.GL_Station
                        where Station.IsEnabled == true && Station.IsGoldenLine == true&&Station.GL_Line.IsEnabled==true
                        && Station.BG_Organization_UID == stationParam.BG_Organization_UID
                        && Station.GL_Line.CustomerID == stationParam.CustomerID
                        && Station.MESStationName == stationParam.StationName
                        select new GL_StationDTO
                        {
                            Plant_Organization_UID = Station.Plant_Organization_UID,
                            BG_Organization_UID = Station.BG_Organization_UID,
                            FunPlant_Organization_UID = Station.FunPlant_Organization_UID,
                            StationName = Station.StationName,
                            MESStationName = Station.MESStationName,
                            StationID = Station.StationID,
                            LineID = Station.LineID,
                            CustomerID = Station.GL_Line.CustomerID
                        };

            if (query.Count() > 0)
            {
                return query.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public List<GL_WIPHourOutputDTO> QueryWIPHourOutput(GL_WIPHourOutputDTO model)
        {
            /*TODO: 2019.01.16 by steven
              修正GroupJion問題，联级也需要确保PIS基础條件Plant_Organization_UID,BG_Organization_UID
            */
            
            var query = from hourOutput in DataContext.GL_WIPHourOutput
                        where hourOutput.GL_Station.IsEnabled == true && hourOutput.GL_Station.IsGoldenLine == true && hourOutput.GL_Line.IsEnabled == true
                        //join s4 in DataContext.GL_LineGroup on hourOutput.LineID equals s4.LineID into t4
                        join s4 in DataContext.GL_LineGroup on new {
                                                                     lineid=(int?)hourOutput.LineID,
                                                                     hourOutput.GL_Station.Plant_Organization_UID,
                                                                     hourOutput.GL_Station.BG_Organization_UID } 
                                                        equals new {
                                                                     lineid=s4.LineID,
                                                                     s4.Plant_Organization_UID,
                                                                     s4.BG_Organization_UID,
                                                                     } into t4
                        from s4 in t4.DefaultIfEmpty()
                            select new GL_WIPHourOutputDTO
                            {
                                WHOID = hourOutput.WHOID,
                                CustomerID = hourOutput.CustomerID,
                                LineID = hourOutput.LineID,
                                StationID = hourOutput.StationID,
                                stationName = hourOutput.GL_Station.StationName,
                                MESStationName = hourOutput.GL_Station.MESStationName,
                                AssemblyID = hourOutput.AssemblyID,
                                OutputDate = hourOutput.OutputDate,
                                ShiftTimeID = hourOutput.ShiftTimeID,
                                HourIndex = hourOutput.HourIndex,
                                StandOutput = hourOutput.StandOutput,
                                ActualOutput = hourOutput.ActualOutput,
                                Plant_Organization_UID = hourOutput.GL_Station.Plant_Organization_UID,
                                BG_Organization_UID = hourOutput.GL_Station.BG_Organization_UID,
                                FunPlant_Organization_UID = hourOutput.GL_Station.FunPlant_Organization_UID,
                                ShiftDate = hourOutput.ShiftDate,
                                LineParent_ID = s4.LineParent_ID
                            };

                if (model.Plant_Organization_UID != 0)
                {
                    query = query.Where(p => p.Plant_Organization_UID == model.Plant_Organization_UID);
                }

                if (model.BG_Organization_UID != 0)
                {
                    query = query.Where(p => p.BG_Organization_UID == model.BG_Organization_UID);
                }

                //if (model.FunPlant_Organization_UID != 0)
                //{
                //    query = query.Where(p => p.FunPlant_Organization_UID == model.FunPlant_Organization_UID);
                //}

                if (model.CustomerID != 0)
                {
                    query = query.Where(p => p.CustomerID == model.CustomerID);
                }

                if (model.LineType == "group_line")
                {
                    query = query.Where(p => p.LineParent_ID == model.LineID);
                }

               /*TODO: 2019.01.16 by steven
                    修正Where 條件問題，不應是and條件，因为找不到model.LineType对应值
              */
               if ((model.LineID != 0) && (model.LineType == "sub_line"))
                {
                    query = query.Where(p => p.LineID == model.LineID);
                }

                if (model.ShiftTimeID != 0)
                {
                    query = query.Where(p => p.ShiftTimeID == model.ShiftTimeID);
                }

                //if (model.StationID != 0)
                //{
                //    query = query.Where(p => p.StationID == model.StationID);
                //}

                if (!string.IsNullOrEmpty(model.myRetriveDate))
                {
                    query = query.Where(p => p.ShiftDate == model.myRetriveDate);
                }

            return query.ToList();
        }

        public GL_ShiftTimeDTO GetShiftTime(int ShiftTimeID)
        {
            var query = from ShiftTB in DataContext.GL_ShiftTime
                        where ShiftTB.IsEnabled == true && ShiftTB.ShiftTimeID == ShiftTimeID
                        select new GL_ShiftTimeDTO
                        {
                            Shift = ShiftTB.Shift,
                            StartTime = ShiftTB.StartTime,
                            End_Time = ShiftTB.End_Time
                        };
            return query.FirstOrDefault();
        }

        public List<GL_WIPHourOutputDTO> ExportWIPHourOutput(GL_WIPHourOutputDTO model)
        {
            return new List<GL_WIPHourOutputDTO>();
        }


        /// <summary>
        /// 获取MESStationName
        /// </summary>
        /// <param name="Project_UID"></param>
        /// <returns></returns>
        public GL_WIPHourOutputDTO GetMESProjectNameByID(int Project_UID)
        {
            var query = from Project in DataContext.System_Project
                        where Project.Project_UID == Project_UID
                        select new GL_WIPHourOutputDTO
                        {
                            MESCustomerName = Project.MESProject_Name,
                            ProjectName = Project.Project_Name
                        };
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取LineName
        /// </summary>
        /// <param name="LineName"></param>
        /// <returns></returns>
        public GL_WIPHourOutputDTO GetMESLineNameByID(int LineID)
        {
            var query = from line in DataContext.GL_Line
                        where line.LineID == LineID && line.IsEnabled == true
                        select new GL_WIPHourOutputDTO
                        {
                            MESLineName = line.MESLineName,
                            LineName = line.LineName,
                        };
            return query.FirstOrDefault();
        }

        public GL_WIPHourOutputDTO GetStationNameByID(string mesStationName)
        {
            var query = from station in DataContext.GL_Station
                        where station.MESStationName == mesStationName && station.IsEnabled == true && station.IsGoldenLine == true && station.GL_Line.IsEnabled == true
                        select new GL_WIPHourOutputDTO
                        {
                            stationName = station.StationName
                        };
            return query.FirstOrDefault();
        }

        public List<GL_StationDTO> GetAllMesStationDTO(int Project_UID)
        {
            /////isone为OEEMES站点
            var query = from Station in DataContext.GL_Station
                        where Station.IsEnabled == true && Station.IsGoldenLine == true && Station.GL_Line.IsEnabled == true
                        &&Station.IsOne==true  
                        select new GL_StationDTO
                        {
                            Plant_Organization_UID = Station.Plant_Organization_UID,
                            BG_Organization_UID = Station.BG_Organization_UID,
                            FunPlant_Organization_UID = Station.FunPlant_Organization_UID,
                            StationName = Station.StationName,
                            MESStationName = Station.MESStationName,
                            StationID = Station.StationID,
                            LineID = Station.LineID,
                            CustomerID = Station.GL_Line.CustomerID,
                            IsEnabled = Station.IsEnabled,
                            IsBirth = Station.IsBirth,
                            IsOutput = Station.IsOutput,
                            IsTest = Station.IsTest,
                            Binding_Seq = Station.Binding_Seq,
                            IsGoldenLine = Station.IsGoldenLine,
                            IsOEE = Station.IsOEE,
                            IsOne = Station.IsOne,
                            IsTwo = Station.IsTwo,
                            IsThree = Station.IsThree,
                            IsFour = Station.IsFour,
                            IsFive = Station.IsFive
                        };
            query = query.Where(p => p.CustomerID == Project_UID);
            query = query.Where(p => p.MESStationName != "" && p.MESStationName != null);
            return query.ToList();

        }
        public List<GL_StationDTO> GetAllStationDTO(int Project_UID)
        {
            var query = from Station in DataContext.GL_Station
                        where Station.IsEnabled == true && Station.IsGoldenLine == true && Station.GL_Line.IsEnabled==true
                        &&Station.IsOne !=true
                        select new GL_StationDTO
                        {
                            Plant_Organization_UID = Station.Plant_Organization_UID,
                            BG_Organization_UID = Station.BG_Organization_UID,
                            FunPlant_Organization_UID = Station.FunPlant_Organization_UID,
                            StationName = Station.StationName,
                            MESStationName = Station.MESStationName,
                            StationID = Station.StationID,
                            LineID = Station.LineID,
                            MESLineName = Station.GL_Line.MESLineName,
                            CustomerID = Station.GL_Line.CustomerID,
                            IsEnabled = Station.IsEnabled,
                            IsBirth = Station.IsBirth,
                            IsOutput = Station.IsOutput,
                            IsTest = Station.IsTest,
                            Binding_Seq = Station.Binding_Seq,
                            IsGoldenLine = Station.IsGoldenLine,
                            IsOEE = Station.IsOEE,
                            IsOne = Station.IsOne,
                            IsTwo = Station.IsTwo,
                            IsThree = Station.IsThree,
                            IsFour = Station.IsFour,
                            IsFive = Station.IsFive
                        };
            query = query.Where(p => p.CustomerID == Project_UID);
            query = query.Where(p => p.MESStationName != "" && p.MESStationName != null);
            return query.ToList();

        }


        public List<GL_WIPHourOutputDTO> GetGL_WIPHourOutputDTOs(int StationID, string OutputDate, List<int> HourIndexs, int ShiftTimeID)
        {
            var query = from hourOutput in DataContext.GL_WIPHourOutput
                        where hourOutput.GL_Station.IsEnabled == true && hourOutput.GL_Station.IsGoldenLine == true && hourOutput.GL_Line.IsEnabled == true
                        select new GL_WIPHourOutputDTO
                        {
                            WHOID = hourOutput.WHOID,
                            CustomerID = hourOutput.CustomerID,
                            LineID = hourOutput.LineID,
                            StationID = hourOutput.StationID,
                            stationName = hourOutput.GL_Station.StationName,
                            MESStationName = hourOutput.GL_Station.MESStationName,
                            AssemblyID = hourOutput.AssemblyID,
                            OutputDate = hourOutput.OutputDate,
                            ShiftTimeID = hourOutput.ShiftTimeID,
                            HourIndex = hourOutput.HourIndex,
                            StandOutput = hourOutput.StandOutput,
                            ActualOutput = hourOutput.ActualOutput,
                            Plant_Organization_UID = hourOutput.GL_Station.Plant_Organization_UID,
                            BG_Organization_UID = hourOutput.GL_Station.BG_Organization_UID,
                            FunPlant_Organization_UID = hourOutput.GL_Station.FunPlant_Organization_UID,
                            ShiftDate = hourOutput.ShiftDate
                        };
            query = query.Where(o => o.StationID == StationID && o.ShiftDate == OutputDate && o.ShiftTimeID == ShiftTimeID && HourIndexs.Contains(o.HourIndex));
            return query.ToList();

        }
    }
}
