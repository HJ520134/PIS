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

    public interface IOEE_MachineInfoRepository : IRepository<OEE_MachineInfo>
    {
        PagedListModel<OEE_MachineInfoDTO> QueryOEE_MachineInfo(OEE_MachineInfoDTO serchModel, Page page);
        OEE_MachineInfoDTO GetOEE_MachineInfoByUID(int uid);
        List<OEE_MachineInfoDTO> ExportOEE_Machine(OEE_MachineInfoDTO exportModel);
        List<OEE_MachineInfoDTO> ExportOEE_Machine(string uids);
        string AddOEE_Machine(OEE_MachineInfoDTO addModel);
        string DeleteOEE_Machine(OEE_MachineInfoDTO deleteModel);

        int GetProject_UID(OEE_MachineInfoDTO serchModel);

        int GetLine_UID(OEE_MachineInfoDTO serchModel);

        int GetStation_UID(OEE_MachineInfoDTO serchModel);

        int GetEquipment_UID(OEE_MachineInfoDTO serchModel);

        List<OEE_MachineInfoDTO> GetAllByStationID(int stationUID);

        OEE_MachineInfoDTO GetAllByEQP_UID(int EQP_Uid);
    }

    /// <summary>
    /// OEE站点基本资料的 配置
    /// </summary>
    public class OEE_MachineInfoRepository : RepositoryBase<OEE_MachineInfo>, IOEE_MachineInfoRepository
    {
        public OEE_MachineInfoRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        /// <summary>
        /// 查询工站设备信息
        /// </summary>
        /// <param name="serchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PagedListModel<OEE_MachineInfoDTO> QueryOEE_MachineInfo(OEE_MachineInfoDTO serchModel, Page page)
        {
            var query = from machine in DataContext.OEE_MachineInfo
                        select new OEE_MachineInfoDTO
                        {
                            OEE_MachineInfo_UID = machine.OEE_MachineInfo_UID,
                            Plant_Organization_UID = machine.Plant_Organization_UID,
                            Plant_Organization_Name = machine.System_Organization.Organization_Name,
                            BG_Organization_UID = machine.BG_Organization_UID,
                            BG_Organization_Name = machine.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = machine.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = machine.System_Organization2.Organization_Name,
                            Project_UID = machine.Project_UID,
                            Project_Name = machine.System_Project.Project_Name,
                            LineID = machine.LineID,
                            Line_Name = machine.GL_Line.LineName,
                            StationID = machine.StationID,
                            Station_Name = machine.GL_Station.StationName,
                            EQP_Uid = machine.EQP_Uid,
                            EQP_EMTSerialNum = machine.Equipment_Info.Equipment,
                            MachineNo = machine.MachineNo,
                            Is_Enable = machine.Is_Enable,
                            Modify_UID = machine.Modify_UID,
                            Modify_Name = machine.System_Users.User_Name,
                            Modify_Date = machine.Modify_Date,
                            Line_Is_Enable = machine.GL_Line.IsEnabled,
                        };

            query = query.Where(p => p.Line_Is_Enable == true);
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
            //专案ID
            if (serchModel.Project_UID != 0)
            {
                query = query.Where(p => p.Project_UID == serchModel.Project_UID);
            }
            //LineID
            if (serchModel.LineID != 0)
            {
                query = query.Where(p => p.LineID == serchModel.LineID);
            }
            //StationID
            if (serchModel.StationID != 0)
            {
                query = query.Where(p => p.StationID == serchModel.StationID);
            }

            //机台EMT号
            if (!string.IsNullOrEmpty(serchModel.EQP_EMTSerialNum))
            {
                query = query.Where(p => p.EQP_EMTSerialNum == serchModel.EQP_EMTSerialNum);
            }

            //机台EMT名称
            if (!string.IsNullOrEmpty(serchModel.MachineNo))
            {
                query = query.Where(p => p.MachineNo == serchModel.MachineNo);
            }

            //是否启用
            //if (serchModel.Is_Enable)
            //{
            //    query = query.Where(p => p.Is_Enable == serchModel.Is_Enable);
            //}
            if (serchModel.query_Is_Enable != "-1" && serchModel.query_Is_Enable != null)
            {
                if (serchModel.query_Is_Enable == "0")
                {
                    serchModel.Is_Enable = false;
                }
                else
                {
                    serchModel.Is_Enable = true;
                }

                query = query.Where(p => p.Is_Enable == serchModel.Is_Enable);
            }

            var totalCount = query.Count();
            query = query.OrderByDescending(m => m.Modify_Date).GetPage(page);
            return new PagedListModel<OEE_MachineInfoDTO>(totalCount, query.ToList());
        }

        public OEE_MachineInfoDTO GetOEE_MachineInfoByUID(int uid)
        {
            var query = from machine in DataContext.OEE_MachineInfo
                        where machine.OEE_MachineInfo_UID == uid
                        select new OEE_MachineInfoDTO
                        {
                            OEE_MachineInfo_UID = machine.OEE_MachineInfo_UID,
                            Plant_Organization_UID = machine.Plant_Organization_UID,
                            Plant_Organization_Name = machine.System_Organization.Organization_Name,
                            BG_Organization_UID = machine.BG_Organization_UID,
                            BG_Organization_Name = machine.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = machine.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = machine.System_Organization2.Organization_Name,
                            Project_UID = machine.Project_UID,
                            Project_Name = machine.System_Project.Project_Name,
                            LineID = machine.LineID,
                            Line_Name = machine.GL_Line.LineName,
                            StationID = machine.StationID,
                            Station_Name = machine.GL_Station.StationName,
                            EQP_Uid = machine.EQP_Uid,
                            EQP_EMTSerialNum = machine.Equipment_Info.Equipment,
                            MachineNo = machine.MachineNo,
                            Is_Enable = machine.Is_Enable,
                            Modify_UID = machine.Modify_UID,
                            Modify_Name = machine.System_Users.User_Name,
                            Modify_Date = machine.Modify_Date
                        };
            return query.FirstOrDefault();
        }
        public List<OEE_MachineInfoDTO> ExportOEE_Machine(OEE_MachineInfoDTO exportModel)
        {
            var query = from machine in DataContext.OEE_MachineInfo
                        select new OEE_MachineInfoDTO
                        {
                            OEE_MachineInfo_UID = machine.OEE_MachineInfo_UID,
                            Plant_Organization_UID = machine.Plant_Organization_UID,
                            Plant_Organization_Name = machine.System_Organization.Organization_Name,
                            BG_Organization_UID = machine.BG_Organization_UID,
                            BG_Organization_Name = machine.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = machine.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = machine.System_Organization2.Organization_Name,
                            Project_UID = machine.Project_UID,
                            Project_Name = machine.System_Project.Project_Name,
                            LineID = machine.LineID,
                            Line_Name = machine.GL_Line.LineName,
                            StationID = machine.StationID,
                            Station_Name = machine.GL_Station.StationName,
                            EQP_Uid = machine.EQP_Uid,
                            EQP_EMTSerialNum = machine.Equipment_Info.Equipment,
                            MachineNo = machine.MachineNo,
                            Is_Enable = machine.Is_Enable,
                            Modify_UID = machine.Modify_UID,
                            Modify_Name = machine.System_Users.User_Name,
                            Modify_Date = machine.Modify_Date
                        };
            //厂区
            if (exportModel.Plant_Organization_UID != 0)
            {
                query = query.Where(p => p.Plant_Organization_UID == exportModel.Plant_Organization_UID);
            }

            //OP
            if (exportModel.BG_Organization_UID != 0)
            {
                query = query.Where(p => p.BG_Organization_UID == exportModel.BG_Organization_UID);
            }
            //功能厂
            if (exportModel.FunPlant_Organization_UID != 0 && exportModel.FunPlant_Organization_UID != null)
            {
                query = query.Where(p => p.FunPlant_Organization_UID == exportModel.FunPlant_Organization_UID);
            }

            //专案ID
            if (exportModel.Project_UID != 0)
            {
                query = query.Where(p => p.Project_UID == exportModel.Project_UID);
            }
            //LineID
            if (exportModel.LineID != 0)
            {
                query = query.Where(p => p.LineID == exportModel.LineID);
            }
            //StationID
            if (exportModel.StationID != 0)
            {
                query = query.Where(p => p.StationID == exportModel.StationID);
            }

            //机台EMT号
            if (!string.IsNullOrEmpty(exportModel.EQP_EMTSerialNum))
            {
                query = query.Where(p => p.EQP_EMTSerialNum == exportModel.EQP_EMTSerialNum);
            }

            //机台EMT名称
            if (!string.IsNullOrEmpty(exportModel.MachineNo))
            {
                query = query.Where(p => p.MachineNo == exportModel.MachineNo);
            }

            //是否启用
            //if (exportModel.Is_Enable)
            //{
            //    query = query.Where(p => p.Is_Enable == exportModel.Is_Enable);
            //}
            if (exportModel.query_Is_Enable != "-1" && exportModel.query_Is_Enable != null)
            {
                if (exportModel.query_Is_Enable == "0")
                {
                    exportModel.Is_Enable = false;
                }
                else
                {
                    exportModel.Is_Enable = true;
                }

                query = query.Where(p => p.Is_Enable == exportModel.Is_Enable);
            }
            return query.ToList();
        }

        public List<OEE_MachineInfoDTO> ExportOEE_Machine(string uids)
        {
            uids = "," + uids + ",";
            var query = from machine in DataContext.OEE_MachineInfo
                        select new OEE_MachineInfoDTO
                        {
                            OEE_MachineInfo_UID = machine.OEE_MachineInfo_UID,
                            Plant_Organization_UID = machine.Plant_Organization_UID,
                            Plant_Organization_Name = machine.System_Organization.Organization_Name,
                            BG_Organization_UID = machine.BG_Organization_UID,
                            BG_Organization_Name = machine.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = machine.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = machine.System_Organization2.Organization_Name,
                            Project_UID = machine.Project_UID,
                            Project_Name = machine.System_Project.Project_Name,
                            LineID = machine.LineID,
                            Line_Name = machine.GL_Line.LineName,
                            StationID = machine.StationID,
                            Station_Name = machine.GL_Station.StationName,
                            EQP_Uid = machine.EQP_Uid,
                            EQP_EMTSerialNum = machine.Equipment_Info.Equipment,
                            MachineNo = machine.MachineNo,
                            Is_Enable = machine.Is_Enable,
                            Modify_UID = machine.Modify_UID,
                            Modify_Name = machine.System_Users.User_Name,
                            Modify_Date = machine.Modify_Date
                        };
            var result = query.ToList().Where(m => uids.Contains("," + m.OEE_MachineInfo_UID + ",")).ToList();
            return result.OrderBy(p => p.Modify_Date).ToList();


        }
        public string AddOEE_Machine(OEE_MachineInfoDTO addModel)
        {
            var sql = @"    INSERT INTO dbo.OEE_MachineInfo
                          ( Plant_Organization_UID ,
                            BG_Organization_UID ,
                            FunPlant_Organization_UID ,
                            Project_UID ,
                            LineID ,
                            StationID ,
                            EQP_Uid ,
                            MachineNo ,
                            Is_Enable ,
                            Modify_UID ,
                            Modify_Date
                          )
                  VALUES  ( {0} ,
                            {1} , 
                            {2} , 
                            {3} , 
                            {4} , 
                            {5} , 
                            {6} , 
                            N'{7}',
                            N'{8}',
                            {9}, 
                           N'{10}'
                          )";
            sql = string.Format(sql,
                                  addModel.Plant_Organization_UID,
                                  addModel.BG_Organization_UID,
                                  addModel.FunPlant_Organization_UID,
                                  addModel.Project_UID,
                                  addModel.LineID,
                                  addModel.StationID,
                                  addModel.EQP_Uid,
                                  addModel.MachineNo,
                                  addModel.Is_Enable,
                                  addModel.Modify_UID,
                                  addModel.Modify_Date);
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

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="deleteModel"></param>
        /// <returns></returns>
        public string DeleteOEE_Machine(OEE_MachineInfoDTO deleteModel)
        {
            try
            {
                var sql = $"DELETE FROM dbo.OEE_MachineInfo WHERE OEE_MachineInfo_UID={deleteModel.OEE_MachineInfo_UID}";
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
        //public string ImportOEE_Machine(List<OEE_MachineInfoDTO> importModelList) { }
        //public string UpdateOEE_Machine(OEE_MachineInfoDTO updateModel)
        //{

        //}

        /// <summary>
        /// 获取专案的UID
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public int GetProject_UID(OEE_MachineInfoDTO serchModel)
        {
            var query = from Organization in DataContext.System_Project
                        where Organization.Organization_UID == serchModel.BG_Organization_UID && Organization.Project_Name == serchModel.Project_Name
                        select Organization;
            var result = query.FirstOrDefault();
            if (result != null)
            {
                return result.Project_UID;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取LineUID
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public int GetLine_UID(OEE_MachineInfoDTO serchModel)
        {
            var query = from line in DataContext.GL_Line
                        where line.Plant_Organization_UID == serchModel.Plant_Organization_UID && line.BG_Organization_UID == serchModel.BG_Organization_UID
                        && line.CustomerID == serchModel.Project_UID
                        select line;
            var result = query.FirstOrDefault();
            if (result != null)
            {
                return result.LineID;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取StationUID
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public int GetStation_UID(OEE_MachineInfoDTO serchModel)
        {
            var query = from line in DataContext.GL_Station
                        where line.Plant_Organization_UID == serchModel.Plant_Organization_UID && line.BG_Organization_UID == serchModel.BG_Organization_UID
                        && line.LineID == serchModel.LineID && line.IsOEE == true
                        select line;
            var result = query.FirstOrDefault();
            if (result != null)
            {
                return result.StationID;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public Equipment_Info GetEquipment_Info(OEE_MachineInfoDTO serchModel)
        {
            var query = from equipment in DataContext.Equipment_Info
                        where equipment.System_FunPlant_UID == serchModel.Plant_Organization_UID   //需要修改为厂区的设置，目前是功能厂。
                        select equipment;
            var result = query.FirstOrDefault();

            return result;

        }


        /// <summary>
        /// 获取设备UId
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public int GetEquipment_UID(OEE_MachineInfoDTO serchModel)
        {
            var query = from equipment in DataContext.Equipment_Info
                        where equipment.Equipment == serchModel.EQP_EMTSerialNum && equipment.System_Organization.Organization_UID == serchModel.Plant_Organization_UID
                        select equipment;
            var result = query.FirstOrDefault();
            if (result != null)
            {
                return result.EQP_Uid;
            }
            else
            {
                return 0;
            }
        }

        public List<OEE_MachineInfoDTO> GetAllByStationID(int stationUID)
        {
            var query = from machine in DataContext.OEE_MachineInfo
                        where machine.StationID == stationUID && machine.Is_Enable == true
                        orderby  machine.MachineNo
                        select new OEE_MachineInfoDTO
                        {
                            OEE_MachineInfo_UID = machine.OEE_MachineInfo_UID,
                            Plant_Organization_UID = machine.Plant_Organization_UID,
                            Plant_Organization_Name = machine.System_Organization.Organization_Name,
                            BG_Organization_UID = machine.BG_Organization_UID,
                            BG_Organization_Name = machine.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = machine.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = machine.System_Organization2.Organization_Name,
                            Project_UID = machine.Project_UID,
                            Project_Name = machine.System_Project.Project_Name,
                            LineID = machine.LineID,
                            Line_Name = machine.GL_Line.LineName,
                            StationID = machine.StationID,
                            Station_Name = machine.GL_Station.StationName,
                            EQP_Uid = machine.EQP_Uid,
                            EQP_EMTSerialNum = machine.Equipment_Info.Equipment,
                            MachineNo = machine.MachineNo,
                            Is_Enable = machine.Is_Enable,
                            Modify_UID = machine.Modify_UID,
                            Modify_Name = machine.System_Users.User_Name,
                            Modify_Date = machine.Modify_Date
                        };

            return query.ToList();
        }


        public OEE_MachineInfoDTO GetAllByEQP_UID(int EQP_Uid)
        {
            var query = from machine in DataContext.OEE_MachineInfo
                        where machine.OEE_MachineInfo_UID == EQP_Uid
                        select new OEE_MachineInfoDTO
                        {
                            OEE_MachineInfo_UID = machine.OEE_MachineInfo_UID,
                            Plant_Organization_UID = machine.Plant_Organization_UID,
                            Plant_Organization_Name = machine.System_Organization.Organization_Name,
                            BG_Organization_UID = machine.BG_Organization_UID,
                            BG_Organization_Name = machine.System_Organization1.Organization_Name,
                            FunPlant_Organization_UID = machine.FunPlant_Organization_UID,
                            FunPlant_Organization_Name = machine.System_Organization2.Organization_Name,
                            Project_UID = machine.Project_UID,
                            Project_Name = machine.System_Project.Project_Name,
                            LineID = machine.LineID,
                            Line_Name = machine.GL_Line.LineName,
                            StationID = machine.StationID,
                            Station_Name = machine.GL_Station.StationName,
                            EQP_Uid = machine.EQP_Uid,
                            EQP_EMTSerialNum = machine.Equipment_Info.Equipment,
                            MachineNo = machine.MachineNo,
                            Is_Enable = machine.Is_Enable,
                            Modify_UID = machine.Modify_UID,
                            Modify_Name = machine.System_Users.User_Name,
                            Modify_Date = machine.Modify_Date
                        };

            return query.FirstOrDefault();
        }

    }
}
