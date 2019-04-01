using Newtonsoft.Json;
using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface ICNCMachineRepository : IRepository<CNCMachine>
    {
        IQueryable<CNCMachineDTO> QueryCNCMachineDTOs(CNCMachineDTO searchModel, Page page, out int totalcount);
        int GetEquipment_UID(int BG_Organization_UID, int FunPlant_Organization_UID, string Equipment);

        CNCMachineDTO QueryCNCMachineDTOByUid(int CNCMachineUID);


        List<SystemProjectDTO> GetSystemProjectDTO(int Plant_Organization_UID, int BG_Organization_UID);


        List<CNCMachineDTO> DoAllExportCNCMachineReprot(CNCMachineDTO searchModel);
        List<CNCMachineDTO> DoExportCNCMachineReprot(string uids);

        List<EquipmentInfoDTO> GetAllEquipmentInfoDTOs();
        List<CNCMachineDTO> GetAllCNCMachineDTOList();
        string ImportMachine(List<CNCMachineDTO> CNCMachineDTOs);
        List<CNCMachineReportDTO> QueryReportCNCMachineDatas(CNCMachineDTO searchModel, Page page, out int totalcount);
        //    List<CNCMachineDTO> GetCNCMachineDTOs(CNCMachineDTO searchModel);

        List<CNCMachineReportDTO> DoAllExportMachineReport(CNCMachineDTO searchModel);
        List<CNCMachineDTO> GetCNCMachineList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<CNCMachineHisReportDTO> DoHisExportMachineReport(int Plant_Organization_UID, string Machine_Name, DateTime? Date_From, DateTime? Date_To);
    }
    public class CNCMachineRepository : RepositoryBase<CNCMachine>, ICNCMachineRepository
    {

        private string CTUconnStr = "Server=CNCTUG0MLSQLV1A;DataBase=OEE;uid=pis;pwd=jabil@1234";


        private string WUXIconnStr = "Server=CNWXIG0LSQLV01B\\INST1;DataBase=MESLDB;uid=PISuser;pwd=PISuser123";

        private string HUIZHOUconnStr = "Server=CNHUZM0DB01;DataBase=MESLDB;uid=Huzsystem01;pwd=Jabil";


        public CNCMachineRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public IQueryable<CNCMachineDTO> QueryCNCMachineDTOs(CNCMachineDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.CNCMachine
                        select new CNCMachineDTO
                        {
                            CNCMachineUID = M.CNCMachineUID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            EQP_Uid = M.EQP_Uid,
                            Machine_Name = M.Machine_Name,
                            Machine_ID = M.Machine_ID,
                            Project_UID = M.Project_UID,
                            Is_Enable = M.Is_Enable,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            Equipment = M.Equipment_Info.Equipment,
                            ProjectName = M.System_Project.Project_Name,
                            Modifyer = M.System_Users.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Machine_Name))
                query = query.Where(m => m.Machine_Name == searchModel.Machine_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.Machine_ID))
                query = query.Where(m => m.Machine_ID == searchModel.Machine_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Modifyer))
                query = query.Where(m => m.Modifyer == searchModel.Modifyer);

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;
        }
        public int GetEquipment_UID(int BG_Organization_UID, int FunPlant_Organization_UID, string Equipment)
        {
            var query = from equipment in DataContext.Equipment_Info
                        where equipment.Equipment == Equipment && equipment.System_Function_Plant.OPType_OrganizationUID == BG_Organization_UID
                        //&& equipment.System_Function_Plant.FunPlant_OrganizationUID == FunPlant_Organization_UID                      
                        select equipment;

            if (FunPlant_Organization_UID != 0)
            {
                query = from equipment in DataContext.Equipment_Info
                        where equipment.Equipment == Equipment && equipment.System_Function_Plant.OPType_OrganizationUID == BG_Organization_UID
                        && equipment.System_Function_Plant.FunPlant_OrganizationUID == FunPlant_Organization_UID
                        select equipment;
            }
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
        public List<EquipmentInfoDTO> GetAllEquipmentInfoDTOs()
        {

            var query = from equipment in DataContext.Equipment_Info
                        join project in DataContext.System_Project
                        on equipment.Project_UID equals project.Project_UID
                        into temp
                        from aa in temp.DefaultIfEmpty()
                        join funplant in DataContext.System_Function_Plant
                        on equipment.System_FunPlant_UID equals funplant.System_FunPlant_UID
                        into temp2
                        from bb in temp2.DefaultIfEmpty()
                        select new EquipmentInfoDTO
                        {

                            EQP_Uid = equipment.EQP_Uid,
                            OP_TYPES = bb.OP_Types,
                            Project_Name = aa.Project_Name,
                            FunPlant = bb.FunPlant,
                            process = equipment.Process,
                            Description_1 = equipment.Description_1,
                            EQP_Plant_No = equipment.EQP_Plant_No,
                            EQP_Location = equipment.EQP_Location,
                            Mfg_Part_Number = equipment.Mfg_Part_Number,
                            Asset = equipment.Asset,
                            Modified_Date = equipment.Modified_Date,
                            Project_UID = equipment.Project_UID,
                            Equipment = equipment.Equipment,
                            Mfg_Serial_Num = equipment.Mfg_Serial_Num,
                            Organization_UID = bb.OPType_OrganizationUID,
                            System_FunPlant_UID = bb.System_FunPlant_UID,
                            Model_Number = equipment.Model_Number,
                            Mfg_Of_Asset = equipment.Mfg_Of_Asset,

                        };


            var result = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Asset).ToList();
            return result;
        }
        public List<CNCMachineDTO> GetAllCNCMachineDTOList()
        {

            var query = from M in DataContext.CNCMachine
                        select new CNCMachineDTO
                        {
                            CNCMachineUID = M.CNCMachineUID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            EQP_Uid = M.EQP_Uid,
                            Machine_Name = M.Machine_Name,
                            Machine_ID = M.Machine_ID,
                            Project_UID = M.Project_UID,
                            Is_Enable = M.Is_Enable,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            Equipment = M.Equipment_Info.Equipment,
                            ProjectName = M.System_Project.Project_Name,
                            Modifyer = M.System_Users.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            return query.ToList();

        }
        public CNCMachineDTO QueryCNCMachineDTOByUid(int CNCMachineUID)
        {

            var query = from M in DataContext.CNCMachine
                        where
M.CNCMachineUID == CNCMachineUID
                        select new CNCMachineDTO
                        {
                            CNCMachineUID = M.CNCMachineUID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            EQP_Uid = M.EQP_Uid,
                            Machine_Name = M.Machine_Name,
                            Machine_ID = M.Machine_ID,
                            Project_UID = M.Project_UID,
                            Is_Enable = M.Is_Enable,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            Equipment = M.Equipment_Info.Equipment,
                            ProjectName = M.System_Project.Project_Name,
                            Modifyer = M.System_Users.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            return query.FirstOrDefault();

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
            //  query = query.Where(p => p.MESProject_Name != null && p.MESProject_Name != "");
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
        public List<CNCMachineDTO> DoAllExportCNCMachineReprot(CNCMachineDTO searchModel)
        {
            var query = from M in DataContext.CNCMachine
                        select new CNCMachineDTO
                        {
                            CNCMachineUID = M.CNCMachineUID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            EQP_Uid = M.EQP_Uid,
                            Machine_Name = M.Machine_Name,
                            Machine_ID = M.Machine_ID,
                            Project_UID = M.Project_UID,
                            Is_Enable = M.Is_Enable,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            Equipment = M.Equipment_Info.Equipment,
                            ProjectName = M.System_Project.Project_Name,
                            Modifyer = M.System_Users.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Machine_Name))
                query = query.Where(m => m.Machine_Name == searchModel.Machine_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.Machine_ID))
                query = query.Where(m => m.Machine_ID == searchModel.Machine_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Modifyer))
                query = query.Where(m => m.Modifyer == searchModel.Modifyer);
            return query.ToList();
        }
        public List<CNCMachineDTO> DoExportCNCMachineReprot(string uids)
        {

            uids = "," + uids + ",";
            var query = from M in DataContext.CNCMachine
                        select new CNCMachineDTO
                        {
                            CNCMachineUID = M.CNCMachineUID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            EQP_Uid = M.EQP_Uid,
                            Machine_Name = M.Machine_Name,
                            Machine_ID = M.Machine_ID,
                            Project_UID = M.Project_UID,
                            Is_Enable = M.Is_Enable,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            Equipment = M.Equipment_Info.Equipment,
                            ProjectName = M.System_Project.Project_Name,
                            Modifyer = M.System_Users.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            query = query.Where(m => uids.Contains("," + m.CNCMachineUID + ","));

            return query.ToList();
        }
        public string ImportMachine(List<CNCMachineDTO> CNCMachineDTOs)
        {
            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    if (CNCMachineDTOs != null && CNCMachineDTOs.Count > 0)
                    {

                        foreach (var item in CNCMachineDTOs)
                        {

                            if (item.CNCMachineUID == 0)
                            {
                                //构造插入SQL数据
                                var insertSql = string.Format(@"INSERT INTO CNCMachine
                                                               (Plant_Organization_UID
                                                               ,BG_Organization_UID
                                                               ,FunPlant_Organization_UID
                                                               ,EQP_Uid
                                                               ,Machine_Name
                                                               ,Machine_ID
                                                               ,Project_UID
                                                               ,Is_Enable
                                                               ,Modified_UID
                                                               ,Modified_Date)
                                                         VALUES
                                                               ({0}
                                                               ,{1}
                                                               ,{2}
                                                               ,{3}
                                                               ,N'{4}'
                                                               ,N'{5}'
                                                               ,{6}                                                                
                                                               ,{7}
                                                               ,{8}
                                                               ,'{9}');",
                                                               item.Plant_Organization_UID, item.BG_Organization_UID, item.FunPlant_Organization_UID != null ? item.FunPlant_Organization_UID : -99, item.EQP_Uid != null ? item.EQP_Uid : -99, item.Machine_Name, item.Machine_ID, item.Project_UID,
                                                               item.Is_Enable ? 1 : 0, item.Modified_UID, item.Modified_Date);



                                insertSql = insertSql.Replace("-99", "NULL");
                                sb.AppendLine(insertSql);
                            }
                            else
                            {

                                var updateSql = string.Format(@" UPDATE CNCMachine
                                                                       SET Plant_Organization_UID =  {0}
                                                                          ,BG_Organization_UID = {1}
                                                                          ,FunPlant_Organization_UID = {2}
                                                                          ,EQP_Uid = {3}
                                                                          ,Machine_Name =  N'{4}'
                                                                          ,Machine_ID =  N'{5}'
                                                                          ,Project_UID ={6}
                                                                          ,Is_Enable ={7}
                                                                          ,Modified_UID ={8}
                                                                          ,Modified_Date = '{9}'
                                                                     WHERE CNCMachineUID={10};",
                                                                  item.Plant_Organization_UID, item.BG_Organization_UID, item.FunPlant_Organization_UID != null ? item.FunPlant_Organization_UID : -99, item.EQP_Uid != null ? item.EQP_Uid : -99, item.Machine_Name, item.Machine_ID, item.Project_UID,
                                                                  item.Is_Enable ? 1 : 0, item.Modified_UID, item.Modified_Date, item.CNCMachineUID);



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

        public List<CNCMachineReportDTO> QueryReportCNCMachineDatas(CNCMachineDTO searchModel, Page page, out int totalcount)
        {
            string connStr = GetconnStr(searchModel.Plant_Organization_UID);
            List<CNCMachineReportDTO> reportCNCMachineDatas = new List<CNCMachineReportDTO>();
            //获取PIS的机台
            var cNCMachineDTOs = GetCNCMachineDTOs(searchModel);
            //得到PIS机台名称集合
            var strMachineNames = GetstrMachineNames(cNCMachineDTOs);
            if (strMachineNames != null && strMachineNames != "")
            {
                //获取PIS中对应MES的机台信息
                var API_RegisteredMachines = GetAPI_RegisteredMachines(strMachineNames, connStr);
                //获取MES机台编码列表
                var strAuthCodes = GetstrAuthCodes(API_RegisteredMachines);
                if (strAuthCodes != null && strAuthCodes != "")
                {
                    DateTime queryTime =DateTime.Now;
                    if (searchModel.QueryTime != null)
                    {
                        queryTime = searchModel.QueryTime.Value;
                    }
                   
                    var cNCCollectionData = GetAPI_CNCCollectionData(null, strAuthCodes, queryTime, connStr);
                    var machineStatus= GetAPI_MachineStatus(strAuthCodes, queryTime, connStr);

                    foreach (var item in API_RegisteredMachines)
                    {
                        CNCMachineReportDTO cNCMachineReportDTO = new CNCMachineReportDTO();
                        cNCMachineReportDTO.AuthCode = item.AuthCode;
                        cNCMachineReportDTO.MachineName = item.Name;
                        cNCMachineReportDTO.dSn = item.dSn;
                        //系统IP地址
                         cNCMachineReportDTO.IPaddress = item.IP;
                        //CNC型号
                        cNCMachineReportDTO.CNCModel = item.Model;
                        //CNC类型
                        cNCMachineReportDTO.CNCType = item.batch;
                        //CNC序列号
                        cNCMachineReportDTO.CNCSequence = item.Computer;
                        //CNC版本号 
                        cNCMachineReportDTO.CNCVersion = item.Version;
                        cNCMachineReportDTO.Customer = item.Customer;
                        cNCMachineReportDTO.ProductID = item.Customer;
                        cNCMachineReportDTO.Site = item.Factory;
                        cNCMachineReportDTO.Building = item.Building;
                        cNCMachineReportDTO.ProcessName = item.Station;
                        //设置机台状态
                        SetAPI_MachineStatus(cNCMachineReportDTO, machineStatus);

                        //实例化Group1
                        var CNCDataGroup1 = cNCCollectionData.FirstOrDefault(o => o.ScanType == "CNCDataGroup1" && o.AuthCode == item.AuthCode);
                        if (CNCDataGroup1 != null && CNCDataGroup1.Data != null && CNCDataGroup1.Data != "")
                        {
                            cNCMachineReportDTO = SetCNCMachineGroup1(cNCMachineReportDTO, CNCDataGroup1.Data);

                        }
                        //实例化Group2
                        var CNCDataGroup2 = cNCCollectionData.FirstOrDefault(o => o.ScanType == "CNCDataGroup2" && o.AuthCode == item.AuthCode);
                        if (CNCDataGroup2 != null && CNCDataGroup2.Data != null && CNCDataGroup2.Data != "")
                        {

                            cNCMachineReportDTO = SetCNCMachineGroup2(cNCMachineReportDTO, CNCDataGroup2.Data);

                        }
                        //实例化Group3
                        var CNCDataGroup3 = cNCCollectionData.FirstOrDefault(o => o.ScanType == "CNCDataGroup3" && o.AuthCode == item.AuthCode);
                        if (CNCDataGroup3 != null && CNCDataGroup3.Data != null && CNCDataGroup3.Data != "")
                        {

                            cNCMachineReportDTO = SetCNCMachineGroup3(cNCMachineReportDTO, CNCDataGroup3.Data);

                        }

                        reportCNCMachineDatas.Add(cNCMachineReportDTO);
                    }

                }
            }

            totalcount = 0;
            return reportCNCMachineDatas;
        }
        private CNCMachineReportDTO SetCNCMachineGroup1(CNCMachineReportDTO cNCMachineReportDTO, string Data)
        {

            var cNCDataGroup1 = SetCNCDataGroup1(Data);

            if (cNCDataGroup1 != null)
            {
                #region  CNCDataGroup1

                ////關機時間 (啟/停)
                //cNCMachineReportDTO.ShutdownTm = cNCDataGroup1.ShutdownTm;
                //轴名称 
                cNCMachineReportDTO.ServoName = cNCDataGroup1.ServoName;
                //开机时间(Min)
                cNCMachineReportDTO.PowerOnTm = cNCDataGroup1.PowerOnTm;
                //切削时间
                cNCMachineReportDTO.CuttingTm = cNCDataGroup1.CuttingTm;
                //循环时间 
                cNCMachineReportDTO.CycleRunTm = cNCDataGroup1.CycleRunTm;
                //当前刀具号 
                cNCMachineReportDTO.CurrenttoolNo = cNCDataGroup1.CurrenttoolNo;
                //最大控制轴数
                cNCMachineReportDTO.MaxAxisNo = cNCDataGroup1.MaxAxisNo;
                //当前控制轴数 
                cNCMachineReportDTO.UseAxisNo = cNCDataGroup1.UseAxisNo;
                //CNC模式
                cNCMachineReportDTO.CNCMode = cNCDataGroup1.CNCMode;
                //主程序号
                cNCMachineReportDTO.MainProgram = cNCDataGroup1.MainProgram;
                //当前程序号
                cNCMachineReportDTO.NowProgram = cNCDataGroup1.NowProgram;
                //程序段号 
                cNCMachineReportDTO.ProgramStep = cNCDataGroup1.ProgramStep;
                //单段备注   
                cNCMachineReportDTO.Singlenote = cNCDataGroup1.Singlenote;
                //程序备注 
                cNCMachineReportDTO.Programnote = cNCDataGroup1.Programnote;        
                //主轴指定速度
                cNCMachineReportDTO.S_SetSpeed = cNCDataGroup1.S_SetSpeed;
                //实际速度
                cNCMachineReportDTO.S_Speed = cNCDataGroup1.S_Speed;
                #endregion

            }

            return cNCMachineReportDTO;
        }
        private CNCMachineReportDTO SetCNCMachineGroup2(CNCMachineReportDTO cNCMachineReportDTO, string Data)
        {

            var cNCDataGroup2 = SetCNCDataGroup2(Data);
            if (cNCDataGroup2 != null)
            {
                #region  CNCDataGroup2
                //主軸溫度
                cNCMachineReportDTO.S_Temp = cNCDataGroup2.S_Temp;
                //主軸扭力
                cNCMachineReportDTO.S_Torque = cNCDataGroup2.S_Torque;
                //主軸熱伸長
                cNCMachineReportDTO.S_TE = cNCDataGroup2.S_TE;
                //宏變量 
                cNCMachineReportDTO.Mcode = cNCDataGroup2.Mcode;
                //切削液狀態 
                cNCMachineReportDTO.Cuttingfluid = cNCDataGroup2.Cuttingfluid;
                //主轴誤差
                cNCMachineReportDTO.S_Gap = cNCDataGroup2.S_Gap;
                //伺服温度
                cNCMachineReportDTO.ServoTemp = cNCDataGroup2.ServoTemp;
                //主轴负载  
                cNCMachineReportDTO.S_Load = cNCDataGroup2.S_Load;
                //切削指定速度 
                cNCMachineReportDTO.CuttingSpeed = cNCDataGroup2.CuttingSpeed;
                //实际速度 
                cNCMachineReportDTO.ActualSpeed = cNCDataGroup2.ActualSpeed;
                //倍率（快移）
                cNCMachineReportDTO.RateJOG = cNCDataGroup2.RateJOG;
                //倍率（切削） 
                cNCMachineReportDTO.RateFastmove = cNCDataGroup2.RateFastmove;
                //倍率（HAND）
                cNCMachineReportDTO.RateHAND = cNCDataGroup2.RateHAND;
                //倍率（主轴） 
                cNCMachineReportDTO.RateS = cNCDataGroup2.RateS;
                //快速进给时间常数 
                cNCMachineReportDTO.FastmoveTmC = cNCDataGroup2.FastmoveTmC;
                //快移速度 
                cNCMachineReportDTO.Fastmovespeed = cNCDataGroup2.Fastmovespeed;

                #endregion


                //需要找 CNCDataGroup1 CNCDataGroup3
            }
            return cNCMachineReportDTO;
        }
        private CNCMachineReportDTO SetCNCMachineGroup3(CNCMachineReportDTO cNCMachineReportDTO, string Data)
        {
            var cNCDataGroup3 = SetCNCDataGroup3(Data);
            if (cNCDataGroup3 != null)
            {

                #region  CNCDataGroup3
                //每次加工起始時間
                cNCMachineReportDTO.ProcessStartTm = cNCDataGroup3.ProcessStartTm;
                //工件加工C/T
                cNCMachineReportDTO.ProcessCycleTm = cNCDataGroup3.ProcessCycleTm;
                //X軸的負載
                cNCMachineReportDTO.X_load = cNCDataGroup3.X_load;
                //Y軸的負載 
                cNCMachineReportDTO.Y_load = cNCDataGroup3.Y_load;
                //Z軸的負載 
                cNCMachineReportDTO.Z_load = cNCDataGroup3.Z_load;
                //A軸的負載
                cNCMachineReportDTO.A_load = cNCDataGroup3.A_load;
                //主軸轉速（S）
                cNCMachineReportDTO.S_RPM = cNCDataGroup3.S_RPM;
                //進給(F)  
                cNCMachineReportDTO.Feed = cNCDataGroup3.Feed;
                //四軸角度 
                cNCMachineReportDTO.Axisangle = cNCDataGroup3.Axisangle;
                //對刀儀
                cNCMachineReportDTO.Toolset = cNCDataGroup3.Toolset;
                #endregion

                //需要找 CNCDataGroup1 CNCDataGroup2
            }
            return cNCMachineReportDTO;

        }
        private CNCMachineReportDTO SetAPI_MachineStatus(CNCMachineReportDTO cNCMachineReportDTO, List<API_MachineStatus> machineStatus)
        {
            var API_MachineStatus = machineStatus.FirstOrDefault(o=>o.AuthCode== cNCMachineReportDTO.AuthCode);
            if (API_MachineStatus != null)
            {
                cNCMachineReportDTO.Status = SetStatus(API_MachineStatus.Status);
                cNCMachineReportDTO.TotalNum = API_MachineStatus.TotalNum;
                cNCMachineReportDTO.ErrorType = API_MachineStatus.Category;
                cNCMachineReportDTO.ErrorCode = API_MachineStatus.ErrorCode;

            }
            return cNCMachineReportDTO;

        }

        private string SetStatus(string status)
        {
            string statusName = "";
            switch(status)
            {
                case "1":
                    statusName = "待料";
                    break;
                case "2":
                    statusName = "生产";
                    break;
                case "3":
                    statusName = "宕机";
                    break;
                case "4":
                    statusName = "保养/校验";
                    break;
                case "5":
                    statusName = "调试";
                    break;
                case "6":
                    statusName = "关机/断网";
                    break;
                default:
                    statusName = "";
                    break;
            }



            return statusName;
        }
        private CNCDataGroup1 SetCNCDataGroup1(string result)
        {
            return JsonConvert.DeserializeObject<CNCDataGroup1>(result);

        }
        private CNCDataGroup2 SetCNCDataGroup2(string result)
        {
            return JsonConvert.DeserializeObject<CNCDataGroup2>(result);

        }
        private CNCDataGroup3 SetCNCDataGroup3(string result)
        {
            return JsonConvert.DeserializeObject<CNCDataGroup3>(result);

        }

        /// <summary>
        /// 根据条件获取机台数据
        /// </summary>
        /// <param name="ScanType"></param>
        /// <param name="AuthCodes"></param>
        /// <param name="CreateTime"></param>
        /// <returns></returns>
        private List<API_CNCCollectionData> GetAPI_CNCCollectionData(string ScanType, string AuthCodes, DateTime CreateTime,string connStr)
        {
            List<API_CNCCollectionData> WipeventDTOs = new List<API_CNCCollectionData>();
            API_CNCCollectionData model = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //if (CreateTime == null)
                    //{
                    //    CreateTime = DateTime.Now;
                    //}
                    //var sqlSB = new StringBuilder(string.Format(@"select * from (select ROW_NUMBER()over(partition by AuthCode,ScanType order by CreateTime desc) rowId,* 
                    //                                                               from API_CNCCollectionData
                    //                                                               WHERE ScanType = '{0}' and AuthCode in ({1})  and  CreateTime<='{2}'
                    //                                                               ) as AuctionRecords
                    //                                                               where rowId = 1;", ScanType, AuthCodes, CreateTime));



                    var sqlSB = new StringBuilder(string.Format(@"   SELECT*
                                                                        FROM API_CNCCollectionData a
                                                                        WHERE EXISTS (
                                                                        SELECT AuthCode, ScanType, CreateTime
                                                                        FROM (
                                                                        SELECT AuthCode, ScanType, MAX(CreateTime)CreateTime
                                                                        FROM API_CNCCollectionData
                                                                          WHERE AuthCode in ({1})  and CreateTime<= '{2}'
                                                                               GROUP bY AuthCode, ScanType
                                                                        ) b
                                                                        WHERE a.AuthCode = b.AuthCode AND A.ScanType = B.ScanType AND a.CreateTime = b.CreateTime
                                                                        )", ScanType, AuthCodes, CreateTime));


                    if (ScanType == null)
                    {
                        //sqlSB = new StringBuilder(string.Format(@"select * from (select ROW_NUMBER()over(partition by AuthCode,ScanType order by CreateTime desc) rowId,* 
                        //                                                           from API_CNCCollectionData
                        //                                                           WHERE AuthCode in ({1})  and  CreateTime<='{2}'
                        //                                                           ) as AuctionRecords
                        //                                                           where rowId = 1;", ScanType, AuthCodes, CreateTime));


                        sqlSB = new StringBuilder(string.Format(@"   SELECT*
                                                                    FROM API_CNCCollectionData a
                                                                    WHERE EXISTS (
                                                                    SELECT AuthCode, ScanType, CreateTime
                                                                    FROM (
                                                                    SELECT AuthCode, ScanType, MAX(CreateTime)CreateTime
                                                                    FROM API_CNCCollectionData
                                                                      WHERE AuthCode in ({1})  and CreateTime<= '{2}'
                                                                           GROUP bY AuthCode, ScanType
                                                                    ) b
                                                                    WHERE a.AuthCode = b.AuthCode AND A.ScanType = B.ScanType AND a.CreateTime = b.CreateTime
                                                                    )", ScanType, AuthCodes, CreateTime));


                    }
                    cmd.CommandText = sqlSB.ToString();
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                model = new API_CNCCollectionData
                                {
                                    ID = Convert.ToInt32(read["ID"]),
                                    dSn = Convert.ToString(read["dSn"]),
                                    AuthCode = Convert.ToString(read["AuthCode"]),
                                    ScanType = Convert.ToString(read["ScanType"]),
                                    Data = Convert.ToString(read["Data"]),
                                    CreateTime = Convert.ToDateTime(read["CreateTime"])
                                };
                                WipeventDTOs.Add(model);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
            return WipeventDTOs;
        }
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="ScanType"></param>
        /// <param name="AuthCodes"></param>
        /// <param name="CreateTime"></param>
        /// <returns></returns>
        private List<API_MachineStatus> GetAPI_MachineStatus(string AuthCodes, DateTime CreateTime,string connStr)
        {
            List<API_MachineStatus> WipeventDTOs = new List<API_MachineStatus>();
            API_MachineStatus model = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    //var sqlSB = new StringBuilder(string.Format(@"select * from (select ROW_NUMBER()over(partition by AuthCode order by CreateTime desc) rowId,* 
                    //                                                               from API_MachineStatus
                    //                                                               WHERE AuthCode in ({0})  and  CreateTime<='{1}'
                    //                                                               ) as AuctionRecords
                    //                                                               where rowId = 1;", AuthCodes, CreateTime));


                    var sqlSB = new StringBuilder(string.Format(@" SELECT*
                                                                    FROM API_MachineStatus a
                                                                    WHERE EXISTS (
                                                                    SELECT AuthCode, CreateTime
                                                                    FROM (
                                                                    SELECT AuthCode, MAX(CreateTime)CreateTime
                                                                    FROM API_MachineStatus
                                                                      WHERE AuthCode in ({0})  and CreateTime<='{1}'
                                                                           GROUP bY AuthCode
                                                                    ) b
                                                                    WHERE a.AuthCode = b.AuthCode  AND a.CreateTime = b.CreateTime
                                                                    )", AuthCodes, CreateTime));


                    cmd.CommandText = sqlSB.ToString();
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {


                                model = new API_MachineStatus
                                {
                                    ID = Convert.ToInt32(read["ID"]),
                                    dSn = Convert.ToString(read["dSn"]),
                                    AuthCode = Convert.ToString(read["AuthCode"]),
                                    MachineName = Convert.ToString(read["MachineName"]),
                                    MachineType = Convert.ToString(read["MachineType"]),
                                    PoorNum = Convert.ToString(read["PoorNum"]),
                                    TotalNum = Convert.ToString(read["TotalNum"]),
                                    Status = Convert.ToString(read["Status"]),
                                    Category = Convert.ToString(read["Category"]),
                                    ErrorCode = Convert.ToString(read["ErrorCode"]),
                                    EventTime = Convert.ToString(read["EventTime"]),
                                    CreateTime = Convert.ToDateTime(read["CreateTime"])
                                };

                                WipeventDTOs.Add(model);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
            return WipeventDTOs;
        }


        /// <summary>
        /// 获取对应的机台基本信息
        /// </summary>
        /// <param name="MachineNames"></param>
        /// <returns></returns>
        private List<API_RegisteredMachine> GetAPI_RegisteredMachines(string MachineNames,string connStr)
        {
            List<API_RegisteredMachine> WipeventDTOs = new List<API_RegisteredMachine>();
            API_RegisteredMachine model = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    //var sqlSB = string.Format(@"SELECT ID
                    //                                  ,dSn
                    //                                  ,AuthCode
                    //                                  ,Name
                    //                                  ,chamber
                    //                                  ,batch
                    //                                  ,Computer
                    //                                  ,Customer
                    //                                  ,Assembly
                    //                                  ,Factory
                    //                                  ,Building
                    //                                  ,MA
                    //                                  ,Route
                    //                                  ,RouteStep
                    //                                  ,RouteStepInstance
                    //                                  ,Station
                    //                                  ,Line
                    //                                  ,Category
                    //                                  ,Vendor
                    //                                  ,CheckPV
                    //                                  ,Model
                    //                                  ,Version
                    //                                  ,CreateTime
                    //                                  ,Creator
                    //                                  ,EditTime
                    //                                  ,Editor
                    //                                  ,MAC
                    //                                  ,IP
                    //                                  ,PorCT
                    //                              FROM API_RegisteredMachine  where Category='CNCDataCollection'  ");

                    var sqlSB = string.Format(@"SELECT ID
                                                      ,dSn
                                                      ,AuthCode
                                                      ,Name
                                                      ,chamber
                                                      ,batch
                                                      ,Computer
                                                      ,Customer
                                                      ,Assembly
                                                      ,Factory
                                                      ,Building
                                                      ,MA
                                                      ,Route
                                                      ,RouteStep
                                                      ,RouteStepInstance
                                                      ,Station
                                                      ,Line
                                                      ,Category
                                                      ,Vendor
                                                      ,CheckPV                                               
                                                      ,CreateTime
                                                      ,Creator
                                                      ,EditTime
                                                      ,Editor
                                                      ,MAC
                                                      ,IP
                                                      ,PorCT
                                                  FROM API_RegisteredMachine  where Category='CNCDataCollection'  ");
                    if (MachineNames != "" && MachineNames != null)
                    {
                        sqlSB += string.Format(@" and Name in ({0}) ", MachineNames);
                    }

                    cmd.CommandText = sqlSB.ToString();
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                model = new API_RegisteredMachine
                                {
                                    ID = Convert.ToInt32(read["ID"]),
                                    dSn = Convert.ToString(read["dSn"]),
                                    AuthCode = Convert.ToString(read["AuthCode"]),
                                    Name = Convert.ToString(read["Name"]),
                                    chamber = Convert.ToString(read["chamber"]),
                                    batch = Convert.ToString(read["batch"]),
                                    Computer = Convert.ToString(read["Computer"]),
                                    Customer = Convert.ToString(read["Customer"]),
                                    Assembly = Convert.ToString(read["Assembly"]),
                                    Factory = Convert.ToString(read["Factory"]),
                                    Building = Convert.ToString(read["Building"]),
                                    MA = Convert.ToString(read["MA"]),
                                    Route = Convert.ToString(read["Route"]),
                                    RouteStep = Convert.ToString(read["RouteStep"]),
                                    RouteStepInstance = Convert.ToString(read["RouteStepInstance"]),
                                    Station = Convert.ToString(read["Station"]),
                                    Line = Convert.ToString(read["Line"]),
                                    Category = Convert.ToString(read["Category"]),
                                    Vendor = Convert.ToString(read["Vendor"]),
                                    CheckPV = Convert.ToString(read["CheckPV"]),
                                    //Model = Convert.ToString(read["Model"]),
                                    //Version = Convert.ToString(read["Version"]),
                                    //CreateTime = Convert.ToDateTime(read["CreateTime"]),
                                    Creator = Convert.ToString(read["Creator"]),
                                   // EditTime = Convert.ToDateTime(read["EditTime"]),
                                    Editor = Convert.ToString(read["Editor"]),
                                    MAC = Convert.ToString(read["MAC"]),
                                    IP = Convert.ToString(read["IP"]),
                                    PorCT = Convert.ToString(read["PorCT"])
                                };
                                WipeventDTOs.Add(model);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
            return WipeventDTOs;

        }
        /// <summary>
        /// 根据查询条件获取机台信息
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        private List<CNCMachineDTO> GetCNCMachineDTOs(CNCMachineDTO searchModel)
        {

            var query = from M in DataContext.CNCMachine
                        select new CNCMachineDTO
                        {
                            CNCMachineUID = M.CNCMachineUID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            EQP_Uid = M.EQP_Uid,
                            Machine_Name = M.Machine_Name,
                            Machine_ID = M.Machine_ID,
                            Project_UID = M.Project_UID,
                            Is_Enable = M.Is_Enable,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            Equipment = M.Equipment_Info.Equipment,
                            ProjectName = M.System_Project.Project_Name,
                            Modifyer = M.System_Users.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID != null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Machine_Name))
                query = query.Where(m => m.Machine_Name.Contains(searchModel.Machine_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Machine_ID))
                query = query.Where(m => m.Machine_ID.Contains(searchModel.Machine_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Modifyer))
                query = query.Where(m => m.Modifyer == searchModel.Modifyer);


            return query.ToList();

        }
        /// <summary>
        /// 获取机台名称集合字符串
        /// </summary>
        /// <param name="cNCMachineDTOs"></param>
        /// <returns></returns>
        private string GetstrMachineNames(List<CNCMachineDTO> cNCMachineDTOs)
        {
            cNCMachineDTOs = cNCMachineDTOs.Where(o => o.Is_Enable == true).ToList();
            string strMachineNames = "";
            foreach (var item in cNCMachineDTOs)
            {

                if (item.Machine_Name != "" && item.Machine_Name != null)
                {
                    strMachineNames += "'" + item.Machine_Name + "',";
                }

            }
            if (strMachineNames.Length > 1)
            {
                strMachineNames = strMachineNames.Substring(0, strMachineNames.Length - 1);
            }

            return strMachineNames;

        }
        /// <summary>
        /// 获取机台编码的字符串
        /// </summary>
        /// <param name="aPI_RegisteredMachine"></param>
        /// <returns></returns>
        private string GetstrAuthCodes(List<API_RegisteredMachine> aPI_RegisteredMachine)
        {
            string strAuthCodes = "";
            foreach (var item in aPI_RegisteredMachine)
            {

                if (item.AuthCode != "" && item.AuthCode != null)
                {
                    strAuthCodes += "'" + item.AuthCode + "',";
                }

            }
            if (strAuthCodes.Length > 1)
            {
                strAuthCodes = strAuthCodes.Substring(0, strAuthCodes.Length - 1);
            }

            return strAuthCodes;

        }
        public List<CNCMachineReportDTO> DoAllExportMachineReport(CNCMachineDTO searchModel)
        {
            string connStr = GetconnStr(searchModel.Plant_Organization_UID);
            List<CNCMachineReportDTO> reportCNCMachineDatas = new List<CNCMachineReportDTO>();
            //获取PIS的机台
            var cNCMachineDTOs = GetCNCMachineDTOs(searchModel);
            //得到PIS机台名称集合
            var strMachineNames = GetstrMachineNames(cNCMachineDTOs);
            if (strMachineNames != null && strMachineNames != "")
            {
                //获取PIS中对应MES的机台信息
                var API_RegisteredMachines = GetAPI_RegisteredMachines(strMachineNames, connStr);
                //获取MES机台编码列表
                var strAuthCodes = GetstrAuthCodes(API_RegisteredMachines);
                if (strAuthCodes != null && strAuthCodes != "")
                {
                    DateTime queryTime = DateTime.Now;
                    if (searchModel.QueryTime != null)
                    {
                        queryTime = searchModel.QueryTime.Value;
                    }
                    var cNCCollectionData = GetAPI_CNCCollectionData(null, strAuthCodes, queryTime, connStr);
                    var machineStatus = GetAPI_MachineStatus(strAuthCodes, queryTime, connStr);
                    foreach (var item in API_RegisteredMachines)
                    {
                        CNCMachineReportDTO cNCMachineReportDTO = new CNCMachineReportDTO();
                        cNCMachineReportDTO.AuthCode = item.AuthCode;
                        cNCMachineReportDTO.MachineName = item.Name;
                        cNCMachineReportDTO.dSn = item.dSn;
                        //系统IP地址
                        cNCMachineReportDTO.IPaddress = item.IP;
                        //CNC型号
                        cNCMachineReportDTO.CNCModel = item.Model;
                        //CNC类型
                        cNCMachineReportDTO.CNCType = item.batch;
                        //CNC序列号
                        cNCMachineReportDTO.CNCSequence = item.Computer;
                        //CNC版本号 
                        cNCMachineReportDTO.CNCVersion = item.Version;
                        cNCMachineReportDTO.Customer = item.Customer;
                        cNCMachineReportDTO.ProductID = item.Customer;
                        cNCMachineReportDTO.Site = item.Factory;
                        cNCMachineReportDTO.Building = item.Building;
                        cNCMachineReportDTO.ProcessName = item.Station;
                        //设置机台状态
                        SetAPI_MachineStatus(cNCMachineReportDTO, machineStatus);
                        //实例化Group1
                        var CNCDataGroup1 = cNCCollectionData.FirstOrDefault(o => o.ScanType == "CNCDataGroup1" && o.AuthCode == item.AuthCode);
                        if (CNCDataGroup1 != null && CNCDataGroup1.Data != null && CNCDataGroup1.Data != "")
                        {
                            cNCMachineReportDTO = SetCNCMachineGroup1(cNCMachineReportDTO, CNCDataGroup1.Data);

                        }
                        //实例化Group2
                        var CNCDataGroup2 = cNCCollectionData.FirstOrDefault(o => o.ScanType == "CNCDataGroup2" && o.AuthCode == item.AuthCode);
                        if (CNCDataGroup2 != null && CNCDataGroup2.Data != null && CNCDataGroup2.Data != "")
                        {

                            cNCMachineReportDTO = SetCNCMachineGroup2(cNCMachineReportDTO, CNCDataGroup2.Data);

                        }
                        //实例化Group3
                        var CNCDataGroup3 = cNCCollectionData.FirstOrDefault(o => o.ScanType == "CNCDataGroup3" && o.AuthCode == item.AuthCode);
                        if (CNCDataGroup3 != null && CNCDataGroup3.Data != null && CNCDataGroup3.Data != "")
                        {

                            cNCMachineReportDTO = SetCNCMachineGroup3(cNCMachineReportDTO, CNCDataGroup3.Data);

                        }

                        reportCNCMachineDatas.Add(cNCMachineReportDTO);
                    }

                }
            }


            return reportCNCMachineDatas;

        }

        public List<CNCMachineDTO> GetCNCMachineList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var query = from M in DataContext.CNCMachine where M.Is_Enable==true
                        select new CNCMachineDTO
                        {
                            CNCMachineUID = M.CNCMachineUID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            EQP_Uid = M.EQP_Uid,
                            Machine_Name = M.Machine_Name,
                            Machine_ID = M.Machine_ID,
                            Project_UID = M.Project_UID,
                            Is_Enable = M.Is_Enable,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            BG_Organization_Name = M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = M.System_Organization2.Organization_Name,
                            Equipment = M.Equipment_Info.Equipment,
                            ProjectName = M.System_Project.Project_Name,
                            Modifyer = M.System_Users.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            if (Plant_Organization_UID != 0)
            {
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            }

            if (BG_Organization_UID != 0)
            {
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            }

            if (FunPlant_Organization_UID != 0)
            {
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            }
            return query.ToList();
        }

        public List<CNCMachineHisReportDTO> DoHisExportMachineReport(int Plant_Organization_UID, string Machine_Name, DateTime? Date_From, DateTime? Date_To)
        {
            if (Machine_Name != null && Machine_Name != "")
            {
                var Machine = DataContext.CNCMachine.FirstOrDefault(o => o.Machine_Name == Machine_Name);
                if(Machine!=null)
                {
                    Plant_Organization_UID = Machine.Plant_Organization_UID;
                }
            }

            string connStr = GetconnStr(Plant_Organization_UID);
            List<CNCMachineHisReportDTO> reportCNCMachineDatas = new List<CNCMachineHisReportDTO>();
            //获取PIS的机台  //得到PIS机台名称集合
            var strMachineNames = ""; 

            if(Machine_Name!=""&& Machine_Name!=null)
            {
                strMachineNames = "'" + Machine_Name + "'";
            }
            var API_RegisteredMachines = GetAPI_RegisteredMachines(strMachineNames, connStr);

            //获取MES机台编码列表
            var strAuthCodes = GetstrAuthCodes(API_RegisteredMachines);
            if (strAuthCodes != null && strAuthCodes != "")
            {
                var cNCCollectionData = GetCNCCollectionData(strAuthCodes, Date_From, Date_To, connStr).GroupBy(o=> new { o.AuthCode, o.ScanType,o.CreateTime }).Select(item => item.First()).ToList();
               //  .GroupBy(item => new { item.ID, item.Name }).Select(item => item.First()).ToList<Student>();
                var machineStatus = GetHisAPI_MachineStatus(strAuthCodes, Date_From, Date_To, connStr);

                foreach (var item in cNCCollectionData)
                {
                    CNCMachineHisReportDTO cNCMachineReportDTO = new CNCMachineHisReportDTO();
                    cNCMachineReportDTO.AuthCode = item.AuthCode;
                   var API_RegisteredMachine=  API_RegisteredMachines.FirstOrDefault(o => o.AuthCode == item.AuthCode);
                    cNCMachineReportDTO.MachineName = API_RegisteredMachine.Name;
                    cNCMachineReportDTO.dSn = item.dSn;
                    //系统IP地址
                    cNCMachineReportDTO.IPaddress = API_RegisteredMachine.IP;
                    //CNC型号
                    cNCMachineReportDTO.CNCModel = API_RegisteredMachine.Model;
                    //CNC类型
                    cNCMachineReportDTO.CNCType = API_RegisteredMachine.batch;
                    //CNC序列号
                    cNCMachineReportDTO.CNCSequence = API_RegisteredMachine.Computer;
                    //CNC版本号 
                    cNCMachineReportDTO.CNCVersion = API_RegisteredMachine.Version;
                    cNCMachineReportDTO.Customer = API_RegisteredMachine.Customer;
                    cNCMachineReportDTO.ProductID = API_RegisteredMachine.Customer;
                    cNCMachineReportDTO.Site = API_RegisteredMachine.Factory;
                    cNCMachineReportDTO.Building = API_RegisteredMachine.Building;
                    cNCMachineReportDTO.ProcessName = API_RegisteredMachine.Station;

                    cNCMachineReportDTO.CreateTime = item.CreateTime;
                    cNCMachineReportDTO.ScanType = item.ScanType;
                    //设置机台状态
                    SetHisAPI_MachineStatus(cNCMachineReportDTO, machineStatus,item.CreateTime);

                    if (item.ScanType == "CNCDataGroup1")
                    {
                        cNCMachineReportDTO = SetCNCMachineHisGroup1(cNCMachineReportDTO, item.Data);
                        // 实例化 CNCDataGroup2
                        var Group2Data = cNCCollectionData.OrderByDescending(o => o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup2" && o.CreateTime <= item.CreateTime);
                        if (Group2Data != null)
                        {
                            cNCMachineReportDTO = SetCNCMachineHisGroup2(cNCMachineReportDTO, Group2Data.Data);
                        }
                        else
                        {
                            Group2Data = cNCCollectionData.OrderBy(o => o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup2" && o.CreateTime >= item.CreateTime);
                            if (Group2Data != null)
                            {
                                cNCMachineReportDTO = SetCNCMachineHisGroup2(cNCMachineReportDTO, Group2Data.Data);
                            }
                        }
                        // 实例化  CNCDataGroup3
                        var Group3Data = cNCCollectionData.OrderByDescending(o => o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup3" && o.CreateTime <= item.CreateTime);
                        if (Group3Data != null)
                        {
                            cNCMachineReportDTO = SetCNCMachineHisGroup3(cNCMachineReportDTO, Group3Data.Data);
                        }
                        else
                        {
                            Group3Data = cNCCollectionData.OrderBy(o => o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup3" && o.CreateTime >= item.CreateTime);
                            if (Group3Data != null)
                            {
                                cNCMachineReportDTO = SetCNCMachineHisGroup3(cNCMachineReportDTO, Group3Data.Data);
                            }
                        }
                    }
                    if (item.ScanType == "CNCDataGroup2")
                    {
                        cNCMachineReportDTO = SetCNCMachineHisGroup2(cNCMachineReportDTO, item.Data);

                        // 实例化 CNCDataGroup1 CNCDataGroup3  // 实例化 CNCDataGroup2
                        var Group1Data = cNCCollectionData.OrderByDescending(o => o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup1" && o.CreateTime <= item.CreateTime);
                        if (Group1Data != null)
                        {
                            cNCMachineReportDTO = SetCNCMachineHisGroup1(cNCMachineReportDTO, Group1Data.Data);
                        }
                        else
                        {
                            Group1Data = cNCCollectionData.OrderBy(o => o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup1" && o.CreateTime >= item.CreateTime);
                            if (Group1Data != null)
                            {
                                cNCMachineReportDTO = SetCNCMachineHisGroup1(cNCMachineReportDTO, Group1Data.Data);
                            }

                        }
                        // 实例化  CNCDataGroup3
                        var Group3Data = cNCCollectionData.OrderByDescending(o => o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup3" && o.CreateTime <= item.CreateTime);
                        if (Group3Data != null)
                        {
                            cNCMachineReportDTO = SetCNCMachineHisGroup3(cNCMachineReportDTO, Group3Data.Data);
                        }
                        else
                        {
                            Group3Data = cNCCollectionData.OrderBy(o => o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup3" && o.CreateTime >= item.CreateTime);
                            if (Group3Data != null)
                            {
                                cNCMachineReportDTO = SetCNCMachineHisGroup3(cNCMachineReportDTO, Group3Data.Data);
                            }

                        }
                    }

                    if (item.ScanType == "CNCDataGroup3")
                    {
                        cNCMachineReportDTO = SetCNCMachineHisGroup3(cNCMachineReportDTO, item.Data);
                        // 实例化 CNCDataGroup2
                        var Group1Data = cNCCollectionData.OrderByDescending(o=>o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup1" && o.CreateTime <= item.CreateTime);
                        if (Group1Data != null)
                        {
                            cNCMachineReportDTO = SetCNCMachineHisGroup1(cNCMachineReportDTO, Group1Data.Data);
                        }
                        else
                        {
                            Group1Data = cNCCollectionData.OrderBy(o => o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup1" && o.CreateTime >= item.CreateTime);
                            if (Group1Data != null)
                            {
                                cNCMachineReportDTO = SetCNCMachineHisGroup1(cNCMachineReportDTO, Group1Data.Data);
                            }

                        }
                        // 实例化  CNCDataGroup3
                        var Group2Data = cNCCollectionData.OrderByDescending(o => o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup2" && o.CreateTime <= item.CreateTime);
                        if (Group2Data != null)
                        {
                            cNCMachineReportDTO = SetCNCMachineHisGroup2(cNCMachineReportDTO, Group2Data.Data);
                        }
                        else
                        {
                            Group2Data = cNCCollectionData.OrderBy(o => o.CreateTime).FirstOrDefault(o => o.ScanType == "CNCDataGroup2" && o.CreateTime >= item.CreateTime);
                            if (Group2Data != null)
                            {
                                cNCMachineReportDTO = SetCNCMachineHisGroup2(cNCMachineReportDTO, Group2Data.Data);
                            }

                        }
                        // 实例化 CNCDataGroup1 CNCDataGroup2
                    }

                    reportCNCMachineDatas.Add(cNCMachineReportDTO);
                }

            }


            //}

            return reportCNCMachineDatas;

        }

        private CNCMachineHisReportDTO SetCNCMachineHisGroup1(CNCMachineHisReportDTO cNCMachineReportDTO, string Data)
        {

            var cNCDataGroup1 = SetCNCDataGroup1(Data);

            if (cNCDataGroup1 != null)
            {
                #region  CNCDataGroup1
                //機台開機
               // cNCMachineReportDTO.PowerOn = cNCDataGroup1.PowerOn;
                //關機時間 (啟/停)
                //cNCMachineReportDTO.ShutdownTm = cNCDataGroup1.ShutdownTm;
                ////CNC型号
                //cNCMachineReportDTO.CNCModel = cNCDataGroup1.CNCModel;
                ////CNC类型
                //cNCMachineReportDTO.CNCType = cNCDataGroup1.CNCType;
                ////CNC序列号
                //cNCMachineReportDTO.CNCSequence = cNCDataGroup1.CNCSequence;
                ////CNC版本号 
                //cNCMachineReportDTO.CNCVersion = cNCDataGroup1.CNCVersion;
                //轴名称 
                cNCMachineReportDTO.ServoName = cNCDataGroup1.ServoName;
                //开机时间(Min)
                cNCMachineReportDTO.PowerOnTm = cNCDataGroup1.PowerOnTm;
                //切削时间
                cNCMachineReportDTO.CuttingTm = cNCDataGroup1.CuttingTm;
                //循环时间 
                cNCMachineReportDTO.CycleRunTm = cNCDataGroup1.CycleRunTm;
                //当前刀具号 
                cNCMachineReportDTO.CurrenttoolNo = cNCDataGroup1.CurrenttoolNo;
                //最大控制轴数
                cNCMachineReportDTO.MaxAxisNo = cNCDataGroup1.MaxAxisNo;
                //当前控制轴数 
                cNCMachineReportDTO.UseAxisNo = cNCDataGroup1.UseAxisNo;
                //CNC模式
                cNCMachineReportDTO.CNCMode = cNCDataGroup1.CNCMode;
                //主程序号
                cNCMachineReportDTO.MainProgram = cNCDataGroup1.MainProgram;
                //当前程序号
                cNCMachineReportDTO.NowProgram = cNCDataGroup1.NowProgram;
                //程序段号 
                cNCMachineReportDTO.ProgramStep = cNCDataGroup1.ProgramStep;
                //单段备注   
                cNCMachineReportDTO.Singlenote = cNCDataGroup1.Singlenote;
                //程序备注 
                cNCMachineReportDTO.Programnote = cNCDataGroup1.Programnote;
                ////系统IP地址 
                //cNCMachineReportDTO.IPaddress = cNCDataGroup1.IPaddress;
                //主轴指定速度
                cNCMachineReportDTO.S_SetSpeed = cNCDataGroup1.S_SetSpeed;
                //实际速度
                cNCMachineReportDTO.S_Speed = cNCDataGroup1.S_Speed;
                #endregion

            }

            return cNCMachineReportDTO;
        }
        private CNCMachineHisReportDTO SetCNCMachineHisGroup2(CNCMachineHisReportDTO cNCMachineReportDTO, string Data)
        {

            var cNCDataGroup2 = SetCNCDataGroup2(Data);
            if (cNCDataGroup2 != null)
            {
                #region  CNCDataGroup2
                //主軸溫度
                cNCMachineReportDTO.S_Temp = cNCDataGroup2.S_Temp;
                //主軸扭力
                cNCMachineReportDTO.S_Torque = cNCDataGroup2.S_Torque;
                //主軸熱伸長
                cNCMachineReportDTO.S_TE = cNCDataGroup2.S_TE;
                //宏變量 
                cNCMachineReportDTO.Mcode = cNCDataGroup2.Mcode;
                //切削液狀態 
                cNCMachineReportDTO.Cuttingfluid = cNCDataGroup2.Cuttingfluid;
                //主轴誤差
                cNCMachineReportDTO.S_Gap = cNCDataGroup2.S_Gap;
                //伺服温度
                cNCMachineReportDTO.ServoTemp = cNCDataGroup2.ServoTemp;
                //主轴负载  
                cNCMachineReportDTO.S_Load = cNCDataGroup2.S_Load;
                //切削指定速度 
                cNCMachineReportDTO.CuttingSpeed = cNCDataGroup2.CuttingSpeed;
                //实际速度 
                cNCMachineReportDTO.ActualSpeed = cNCDataGroup2.ActualSpeed;
                //倍率（快移）
                cNCMachineReportDTO.RateJOG = cNCDataGroup2.RateJOG;
                //倍率（切削） 
                cNCMachineReportDTO.RateFastmove = cNCDataGroup2.RateFastmove;
                //倍率（HAND）
                cNCMachineReportDTO.RateHAND = cNCDataGroup2.RateHAND;
                //倍率（主轴） 
                cNCMachineReportDTO.RateS = cNCDataGroup2.RateS;
                //快速进给时间常数 
                cNCMachineReportDTO.FastmoveTmC = cNCDataGroup2.FastmoveTmC;
                //快移速度 
                cNCMachineReportDTO.Fastmovespeed = cNCDataGroup2.Fastmovespeed;

                #endregion


                //需要找 CNCDataGroup1 CNCDataGroup3
            }
            return cNCMachineReportDTO;
        }
        private CNCMachineHisReportDTO SetCNCMachineHisGroup3(CNCMachineHisReportDTO cNCMachineReportDTO, string Data)
        {
            var cNCDataGroup3 = SetCNCDataGroup3(Data);
            if (cNCDataGroup3 != null)
            {

                #region  CNCDataGroup3
                //每次加工起始時間
                cNCMachineReportDTO.ProcessStartTm = cNCDataGroup3.ProcessStartTm;
                //工件加工C/T
                cNCMachineReportDTO.ProcessCycleTm = cNCDataGroup3.ProcessCycleTm;
                //X軸的負載
                cNCMachineReportDTO.X_load = cNCDataGroup3.X_load;
                //Y軸的負載 
                cNCMachineReportDTO.Y_load = cNCDataGroup3.Y_load;
                //Z軸的負載 
                cNCMachineReportDTO.Z_load = cNCDataGroup3.Z_load;
                //A軸的負載
                cNCMachineReportDTO.A_load = cNCDataGroup3.A_load;
                //主軸轉速（S）
                cNCMachineReportDTO.S_RPM = cNCDataGroup3.S_RPM;
                //進給(F)  
                cNCMachineReportDTO.Feed = cNCDataGroup3.Feed;
                //四軸角度 
                cNCMachineReportDTO.Axisangle = cNCDataGroup3.Axisangle;
                //對刀儀
                cNCMachineReportDTO.Toolset = cNCDataGroup3.Toolset;
                #endregion

                //需要找 CNCDataGroup1 CNCDataGroup2
            }
            return cNCMachineReportDTO;

        }

        private List<API_CNCCollectionData> GetCNCCollectionData(string AuthCode, DateTime? Date_From, DateTime? Date_To,string connStr)
        {
            List<API_CNCCollectionData> WipeventDTOs = new List<API_CNCCollectionData>();
            API_CNCCollectionData model = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    var sqlSB = string.Format(@"SELECT ID
                                              ,dSn
                                              ,AuthCode
                                              ,ScanType
                                              ,Data
                                              ,CreateTime
                                          FROM API_CNCCollectionData  where  1=1  ", AuthCode);


                    if (AuthCode != "" && AuthCode != null)
                    {
                        sqlSB += string.Format(@"  and  AuthCode in ({0})  ", AuthCode);
                    }

                    if (Date_From != null && Date_To != null)
                    {
                        Date_From = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "00:00:00"));
                        Date_To = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "23:59:59"));
                        sqlSB += string.Format(@"  and  '{0}'<=CreateTime and CreateTime<='{1}' ", Date_From.Value.ToString(FormatConstants.DateTimeFormatString), Date_To.Value.ToString(FormatConstants.DateTimeFormatString));
                    }
                    if (Date_From == null && Date_To == null)
                    {
                        Date_From = DateTime.Now;
                        Date_From = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "00:00:00"));
                        Date_To = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "23:59:59"));
                        sqlSB += string.Format(@"  and  '{0}'<=CreateTime and CreateTime<='{1}' ", Date_From.Value.ToString(FormatConstants.DateTimeFormatString), Date_To.Value.ToString(FormatConstants.DateTimeFormatString));
                    }
                    if (Date_From != null && Date_To == null)
                    {

                        Date_From = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "00:00:00"));
                        sqlSB += string.Format(@"  and  '{0}'<=CreateTime ", Date_From.Value.ToString(FormatConstants.DateTimeFormatString));
                    }
                    if (Date_From == null && Date_To != null)
                    {
                        Date_To = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "23:59:59"));
                        sqlSB += string.Format(@"  and  CreateTime<='{0}' ", Date_To.Value.ToString(FormatConstants.DateTimeFormatString));
                    }
                    sqlSB += string.Format(@"  order by CreateTime ");

                    cmd.CommandText = sqlSB.ToString();
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                model = new API_CNCCollectionData
                                {
                                    ID = Convert.ToInt32(read["ID"]),
                                    dSn = Convert.ToString(read["dSn"]),
                                    AuthCode = Convert.ToString(read["AuthCode"]),
                                    ScanType = Convert.ToString(read["ScanType"]),
                                    Data = Convert.ToString(read["Data"]),
                                    CreateTime = Convert.ToDateTime(read["CreateTime"])
                                };
                                WipeventDTOs.Add(model);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
            return WipeventDTOs;
        }


        private List<API_MachineStatus> GetHisAPI_MachineStatus(string AuthCodes, DateTime? Date_From, DateTime? Date_To,string connStr)
        {
                  
            List<API_MachineStatus> WipeventDTOs = new List<API_MachineStatus>();
            API_MachineStatus model = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    var sqlSB = string.Format(@"SELECT ID
                                                      ,dSn
                                                      ,AuthCode
                                                      ,MachineName
                                                      ,MachineType
                                                      ,PoorNum
                                                      ,TotalNum
                                                      ,Status
                                                      ,Category
                                                      ,ErrorCode
                                                      ,EventTime
                                                      ,CreateTime
                                                  FROM API_MachineStatus where 1=1 ");

                    if (AuthCodes != "" && AuthCodes != null)
                    {
                        sqlSB += string.Format(@"  and  AuthCode in ({0})  ", AuthCodes);
                    }

                    //if (Date_From != null && Date_To != null)
                    //{
                    //    sqlSB += string.Format(@"  and  '{0}'<=CreateTime and CreateTime<='{1}' ", Date_From, Date_To);
                    //}
                    //if (Date_From == null && Date_To == null)
                    //{
                    //    Date_From = DateTime.Now;
                    //    Date_From = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "00:00:00"));
                    //    Date_To = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "23:59:59"));
                    //    sqlSB += string.Format(@"  and  '{0}'<=CreateTime and CreateTime<='{1}' ", Date_From, Date_To);
                    //}
                    //if (Date_From != null && Date_To == null)
                    //{

                    //    sqlSB += string.Format(@"  and  '{0}'<=CreateTime ", Date_From);
                    //}
                    //if (Date_From == null && Date_To != null)
                    //{

                    //    sqlSB += string.Format(@"  and  CreateTime<='{0}' ", Date_To);
                    //}

                    if (Date_From != null && Date_To != null)
                    {
                        Date_From = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "00:00:00"));
                        Date_To = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "23:59:59"));
                        sqlSB += string.Format(@"  and  '{0}'<=CreateTime and CreateTime<='{1}' ", Date_From.Value.ToString(FormatConstants.DateTimeFormatString), Date_To.Value.ToString(FormatConstants.DateTimeFormatString));
                    }
                    if (Date_From == null && Date_To == null)
                    {
                        Date_From = DateTime.Now;
                        Date_From = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "00:00:00"));
                        Date_To = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "23:59:59"));
                        sqlSB += string.Format(@"  and  '{0}'<=CreateTime and CreateTime<='{1}' ", Date_From.Value.ToString(FormatConstants.DateTimeFormatString), Date_To.Value.ToString(FormatConstants.DateTimeFormatString));
                    }
                    if (Date_From != null && Date_To == null)
                    {

                        Date_From = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "00:00:00"));
                        sqlSB += string.Format(@"  and  '{0}'<=CreateTime ", Date_From.Value.ToString(FormatConstants.DateTimeFormatString));
                    }
                    if (Date_From == null && Date_To != null)
                    {
                        Date_To = Convert.ToDateTime((Date_From.Value.ToString("yyyy-MM-dd ") + "23:59:59"));
                        sqlSB += string.Format(@"  and  CreateTime<='{0}' ", Date_To.Value.ToString(FormatConstants.DateTimeFormatString));
                    }


                    sqlSB += string.Format(@"  order by CreateTime desc ");

                    cmd.CommandText = sqlSB.ToString();
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {


                                model = new API_MachineStatus
                                {
                                    ID = Convert.ToInt32(read["ID"]),
                                    dSn = Convert.ToString(read["dSn"]),
                                    AuthCode = Convert.ToString(read["AuthCode"]),
                                    MachineName = Convert.ToString(read["MachineName"]),
                                    MachineType = Convert.ToString(read["MachineType"]),
                                    PoorNum = Convert.ToString(read["PoorNum"]),
                                    TotalNum = Convert.ToString(read["TotalNum"]),
                                    Status = Convert.ToString(read["Status"]),
                                    Category = Convert.ToString(read["Category"]),
                                    ErrorCode = Convert.ToString(read["ErrorCode"]),
                                    EventTime = Convert.ToString(read["EventTime"]),
                                    CreateTime = Convert.ToDateTime(read["CreateTime"])
                                };

                                WipeventDTOs.Add(model);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
            return WipeventDTOs;
        }

        private CNCMachineHisReportDTO SetHisAPI_MachineStatus(CNCMachineHisReportDTO cNCMachineReportDTO, List<API_MachineStatus> machineStatus, DateTime? dateTime)
        {
            var API_MachineStatus = machineStatus.OrderByDescending(o => o.CreateTime).FirstOrDefault(o => o.AuthCode == cNCMachineReportDTO.AuthCode && o.CreateTime <= dateTime);
            if (API_MachineStatus != null)
            {
                cNCMachineReportDTO.Status = SetStatus(API_MachineStatus.Status);
                cNCMachineReportDTO.TotalNum = API_MachineStatus.TotalNum;
                cNCMachineReportDTO.ErrorType = API_MachineStatus.Category;
                cNCMachineReportDTO.ErrorCode = API_MachineStatus.ErrorCode;

            }else
            {
                API_MachineStatus = machineStatus.OrderBy(o => o.CreateTime).FirstOrDefault(o => o.AuthCode == cNCMachineReportDTO.AuthCode && o.CreateTime >= dateTime);
                if(API_MachineStatus!=null)
                {
                    cNCMachineReportDTO.Status = SetStatus(API_MachineStatus.Status);
                    cNCMachineReportDTO.TotalNum = API_MachineStatus.TotalNum;
                    cNCMachineReportDTO.ErrorType = API_MachineStatus.Category;
                    cNCMachineReportDTO.ErrorCode = API_MachineStatus.ErrorCode;
                }
            }
            return cNCMachineReportDTO;

        }
        private string GetconnStr(int Plant_Organization_UID)
        {
            string connStr = "";
            if (Plant_Organization_UID==1)
            {
                connStr = CTUconnStr;
            }
            else if(Plant_Organization_UID == 35)
            {
                connStr = WUXIconnStr;
            }
            else
            {
                connStr = HUIZHOUconnStr;
            }


                return connStr;
        }


    }
}
