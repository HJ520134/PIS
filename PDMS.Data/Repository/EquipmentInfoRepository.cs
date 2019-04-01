using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using PDMS.Data;
using System.Data.Entity.SqlServer;
using PDMS.Common.Constants;

namespace PDMS.Data.Repository
{
    public interface IEquipmentInfoRepository : IRepository<Equipment_Info>
    {
        List<string> getFunplants(string selsectProjects, string opType);
        List<EquipmentReport> GetEquipmentInfoNOTReprot(EquipmentReport searchModel, List<int> Organization_UIDs);
        List<EquipmentReport> GetEquipmentInfoReprot(EquipmentReport searchModel, List<int> Organization_UIDs);
        IQueryable<EquipmentReport> GetEquipmentInfoReprot(EquipmentReport searchModel, List<int> Organization_UIDs, Page page, out int totalcount);

        IQueryable<EquipmentReport> GetEquipmentInfoNOTReprot(EquipmentReport searchModel, List<int> Organization_UIDs, Page page, out int totalcount);
        IQueryable<EquipmentInfoDTO> GetInfo(EquipmentInfoSearchDTO searchModel, List<int> Organization_UIDs, Page page, out int totalcount);

        List<EquipmentInfoDTO> ExportALLEquipmentInfo(EquipmentInfoSearchDTO searchModel, List<int> organization_UIDs);

        List<EquipmentInfoDTO> ExportPartEquipmentInfo(string uids);

        List<EquipmentInfoDTO> GetByUId(string EQP_Uid);
        void UpdateItem(EquipmentInfoDTO dto);
        string InsertItem(List<EquipmentInfoDTO> dtolist);
        string DeleteEquipment(int EQP_Uid);
        EqumentOrgInfo GetOEE_MachineInfoByEMT(int PlantUID, string EMT);
        List<EquipmentInfoDTO> GetDistinctoptype();
        List<System_Organization_PlantDTO> GetOrganization_Plants();
        List<SystemProjectDTO> GetDistinctoptypeByUser(int optype);
        List<SystemRoleDTO> Getuserrole(string userid);
        List<EnumerationDTO> QueryDistinctReason();
        List<EquipmentInfoDTO> GetProjectnameByOptype(string Optype);
        List<SystemFunctionPlantDTO> GetFunPlantByOPType(int Optype, string Optypes);
        List<EquipmentInfoDTO> GetProcessByFunplant(string funplantuid);
        List<string> GetEqpidByProcess(string funplantuid);
        List<EquipmentInfoDTO> GetInfoByUserid(string location, int funplant, int optype);
        List<string> GetDistinctLocation(int Plant_Organization_UID);
        EquipmentInfoDTO GetEqpBySerialAndEMT(string serial, string emt_num);
        List<SystemFunctionPlantDTO> GetFunplantByUser(int userid, int optypeuid);
        List<string> GetOPTypeByUser(int userid);
    }
    public class EquipmentInfoRepository : RepositoryBase<Equipment_Info>, IEquipmentInfoRepository
    {
        public EquipmentInfoRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public EqumentOrgInfo GetOEE_MachineInfoByEMT(int PlantUID, string EMT)
        {
            var query = from E in DataContext.Equipment_Info
                        join P in DataContext.System_Project on E.Project_UID equals P.Project_UID
                        where E.Plant_Organization_UID == PlantUID && E.Equipment == EMT
                        select new EqumentOrgInfo
                        {
                            BG_Organization_UID = E.BG_Organization_UID,
                            FunPlant_Organization_UID = E.FunPlant_Organization_UID,
                            Project_UID = E.Project_UID,
                            Project=P.Project_Name,
                            OP=E.System_Organization1.Organization_Name,
                            FuncPlant=E.System_Organization2.Organization_Name
                        };
                       
            return query.FirstOrDefault();
        }
        public List<string> getFunplants(string selsectProjects, string opType)
        {
            List<string> result = new List<string>();
            string[] funplants = selsectProjects.Split(',');
            foreach (var fp in funplants)
            {
                var PN = fp.Split('_')[0]; var PT = fp.Split('_')[1]; var PP = fp.Split('_')[2];
                if (PT == "ALL")
                {//先获取flowchart最新版本
                    var queryM = from m in DataContext.FlowChart_Master
                                 where m.System_Project.Project_Name == PN && m.Product_Phase == PP
                                 orderby m.FlowChart_Version
                                 select m.FlowChart_Version;
                    if (queryM.Count() > 0)
                    {
                        var query = from f in DataContext.FlowChart_Detail
                                    where f.FlowChart_Master.System_Project.Project_Name == PN  && f.FlowChart_Master.Product_Phase == PP && f.FlowChart_Master.System_Project.OP_TYPES == opType
                                    && f.FlowChart_Version == queryM.FirstOrDefault()
                                    select f.System_Function_Plant.FunPlant;
                        List<string> temp = query.Distinct().ToList();
                        result.AddRange(temp);
                    }
                }
                else
                {
                    //先获取flowchart最新版本
                    var queryM = from m in DataContext.FlowChart_Master
                                 where m.System_Project.Project_Name == PN && m.Part_Types == PT && m.Product_Phase == PP
                                 select m.FlowChart_Version;
                    if (queryM.Count() > 0)
                    {
                        var query = from f in DataContext.FlowChart_Detail
                                    where f.FlowChart_Master.System_Project.Project_Name == PN && f.FlowChart_Master.Part_Types == PT && f.FlowChart_Master.Product_Phase == PP && f.FlowChart_Master.System_Project.OP_TYPES == opType
                                    && f.FlowChart_Version == queryM.FirstOrDefault()
                                    select f.System_Function_Plant.FunPlant;
                        List<string> temp = query.Distinct().ToList();
                        result.AddRange(temp);
                    }
                }
            }

            return result.Distinct().ToList();
        }
        public string DeleteEquipment(int EQP_Uid)
        {
            try
            {
                string sql = "delete Equipment_Info where EQP_Uid={0}";
                sql = string.Format(sql, EQP_Uid);
                int result = DataContext.Database.ExecuteSqlCommand(sql);
                if (result == 1)
                    return "";
                else
                    return "删除设备记录失败";
            }
            catch (Exception e)
            {
                return "此设备在使用中，不能删除！";
            }
        }

        public IQueryable<EquipmentInfoDTO> GetInfo(EquipmentInfoSearchDTO searchModel, List<int> Organization_UIDs, Page page, out int totalcount)
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
                            Class_Desc = equipment.Class_Desc,
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
                            ProductDate = (equipment.ConY == null || equipment.ConY == string.Empty || equipment.CM == null || equipment.CM == string.Empty) ? string.Empty : equipment.ConY + "-" + equipment.CM
                        };
            if (!string.IsNullOrWhiteSpace(searchModel.Equipment))
                query = query.Where(m => m.Equipment.Contains(searchModel.Equipment));
            if (searchModel.Project_UID != null && searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.System_FunPlant_UID != 0)
                query = query.Where(m => m.System_FunPlant_UID == searchModel.System_FunPlant_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.process))
                query = query.Where(m => m.process.Contains(searchModel.process));
            if (!string.IsNullOrWhiteSpace(searchModel.Mfg_Serial_Num))
                query = query.Where(m => m.Mfg_Serial_Num.Contains(searchModel.Mfg_Serial_Num));
            if (!string.IsNullOrWhiteSpace(searchModel.EQP_Plant_No))
                query = query.Where(m => m.EQP_Plant_No.Contains(searchModel.EQP_Plant_No));
            if (!string.IsNullOrWhiteSpace(searchModel.Asset))
                query = query.Where(m => m.Asset.Contains(searchModel.Asset));
            if (!string.IsNullOrWhiteSpace(searchModel.EQP_Location))
                query = query.Where(m => m.EQP_Location.Contains(searchModel.EQP_Location));
            if (!string.IsNullOrWhiteSpace(searchModel.Class_Desc))
                query = query.Where(m => m.Class_Desc == searchModel.Class_Desc);
            if (!string.IsNullOrWhiteSpace(searchModel.ProductDate))
                query = query.Where(m => searchModel.ProductDate.Contains(m.ProductDate));
            if (searchModel.Organization_UID != null && searchModel.Organization_UID != 0)
                query = query.Where(m => m.Organization_UID == searchModel.Organization_UID);
            // 只是针对厂区的过滤
            if (Organization_UIDs.Count > 0)
                query = query.Where(m => Organization_UIDs.Contains(m.Organization_UID.Value));
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Asset).GetPage(page);
            return query;
        }

        public List<EquipmentInfoDTO> ExportALLEquipmentInfo(EquipmentInfoSearchDTO searchModel, List<int> Organization_UIDs)
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
                            Class_Desc = equipment.Class_Desc,
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
                            CoCd = equipment.CoCd,
                            OpU= equipment.OpU,
                            C= equipment.C,
                            Process_Group= equipment.Process_Group,
                            Class= equipment.Class,
                            AM_CostCtr= equipment.AM_CostCtr,
                            User_Status = equipment.User_Status,
                            Func_Loc = equipment.Func_Loc,
                            Room = equipment.Room,
                            Int_Note_L1 = equipment.Int_Note_L1,
                            Int_Note_L2 = equipment.Int_Note_L2,
                            Cap_date = equipment.Cap_date,
                            Acquisition_Value = equipment.Acquisition_Value,
                            Asset_Life = equipment.Asset_Life,
                            Net_Book_Value = equipment.Net_Book_Value,
                            Monthly_Depreciation = equipment.Monthly_Depreciation,
                            Remaining_Life = equipment.Remaining_Life,
                            Weight = equipment.Weight,
                            Size_dimension = equipment.Size_dimension,
                            MCtry = equipment.MCtry,
                            ConY = equipment.ConY,
                            CM = equipment.CM,
                            Description_2 = equipment.Description_2,
                            Characteristic_1 = equipment.Characteristic_1,
                            Description_3 = equipment.Description_3,
                            Characteristic_2 = equipment.Characteristic_2,
                            Description_4 = equipment.Description_4,
                            Characteristic_3 = equipment.Characteristic_3,
                            Description_5 = equipment.Description_5,
                            Characteristic_4 = equipment.Characteristic_4,
                            Description_6 = equipment.Description_6,
                            Characteristic_5 = equipment.Characteristic_5,

                            ProductDate = (equipment.ConY == null || equipment.ConY == string.Empty || equipment.CM == null || equipment.CM == string.Empty) ? string.Empty : equipment.ConY + "-" + equipment.CM
                        };
            if (!string.IsNullOrWhiteSpace(searchModel.Equipment))
                query = query.Where(m => m.Equipment.Contains(searchModel.Equipment));
            if (searchModel.Project_UID != null && searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.System_FunPlant_UID != 0)
                query = query.Where(m => m.System_FunPlant_UID == searchModel.System_FunPlant_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.process))
                query = query.Where(m => m.process.Contains(searchModel.process));
            if (!string.IsNullOrWhiteSpace(searchModel.Mfg_Serial_Num))
                query = query.Where(m => m.Mfg_Serial_Num.Contains(searchModel.Mfg_Serial_Num));
            if (!string.IsNullOrWhiteSpace(searchModel.EQP_Plant_No))
                query = query.Where(m => m.EQP_Plant_No.Contains(searchModel.EQP_Plant_No));
            if (!string.IsNullOrWhiteSpace(searchModel.Asset))
                query = query.Where(m => m.Asset.Contains(searchModel.Asset));
            if (!string.IsNullOrWhiteSpace(searchModel.EQP_Location))
                query = query.Where(m => m.EQP_Location.Contains(searchModel.EQP_Location));
            if (!string.IsNullOrWhiteSpace(searchModel.Class_Desc))
                query = query.Where(m => m.Class_Desc == searchModel.Class_Desc);
            if (!string.IsNullOrWhiteSpace(searchModel.ProductDate))
                query = query.Where(m => searchModel.ProductDate.Contains(m.ProductDate));
            if (searchModel.Organization_UID != null && searchModel.Organization_UID != 0)
                query = query.Where(m => m.Organization_UID == searchModel.Organization_UID);
            // 只是针对厂区的过滤
            if (Organization_UIDs.Count > 0)
            {
                query = query.Where(m => Organization_UIDs.Contains(m.Organization_UID.Value));
            }
            var result = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Asset).ToList();
            return result;
        }

        public List<EquipmentInfoDTO> ExportPartEquipmentInfo(string uids)
        {
            uids = "," + uids + ",";
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
                            Class_Desc = equipment.Class_Desc,
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
                            CoCd = equipment.CoCd,
                            OpU = equipment.OpU,
                            C = equipment.C,
                            Process_Group = equipment.Process_Group,
                            Class = equipment.Class,
                            AM_CostCtr = equipment.AM_CostCtr,
                            User_Status = equipment.User_Status,
                            Func_Loc = equipment.Func_Loc,
                            Room = equipment.Room,
                            Int_Note_L1 = equipment.Int_Note_L1,
                            Int_Note_L2 = equipment.Int_Note_L2,
                            Cap_date = equipment.Cap_date,
                            Acquisition_Value = equipment.Acquisition_Value,
                            Asset_Life = equipment.Asset_Life,
                            Net_Book_Value = equipment.Net_Book_Value,
                            Monthly_Depreciation = equipment.Monthly_Depreciation,
                            Remaining_Life = equipment.Remaining_Life,
                            Weight = equipment.Weight,
                            Size_dimension = equipment.Size_dimension,
                            MCtry = equipment.MCtry,
                            ConY = equipment.ConY,
                            CM = equipment.CM,
                            Description_2 = equipment.Description_2,
                            Characteristic_1 = equipment.Characteristic_1,
                            Description_3 = equipment.Description_3,
                            Characteristic_2 = equipment.Characteristic_2,
                            Description_4 = equipment.Description_4,
                            Characteristic_3 = equipment.Characteristic_3,
                            Description_5 = equipment.Description_5,
                            Characteristic_4 = equipment.Characteristic_4,
                            Description_6 = equipment.Description_6,
                            Characteristic_5 = equipment.Characteristic_5,
                            ProductDate = (equipment.ConY == null || equipment.ConY == string.Empty || equipment.CM == null || equipment.CM == string.Empty) ? string.Empty : equipment.ConY + "-" + equipment.CM
                        };
            query = query.Where(m => uids.Contains("," + m.EQP_Uid + ","));

            var result = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Asset).ToList();
            return result;
        }

        public List<EquipmentInfoDTO> GetByUId(string EQP_Uid)
        {
            string sql = @"SELECT  t1.*,ConY+'-'+CM ProductDate,ISNULL(t2.Project_Name,'') Project_Name,t2.OP_TYPES,t3.FunPlant,t2.Organization_UID from Equipment_Info t1 LEFT JOIN dbo.System_Project
                        t2 ON t1.Project_UID=t2.Project_UID
                        LEFT JOIN dbo.System_Function_Plant t3 ON t1.System_FunPlant_UID=t3.System_FunPlant_UID where EQP_Uid={0}";
            sql = string.Format(sql, EQP_Uid);
            var dblist = DataContext.Database.SqlQuery<EquipmentInfoDTO>(sql).ToList();
            return dblist;
        }

        public void UpdateItem(EquipmentInfoDTO dto)
        {


            string sql = @"update Equipment_Info set System_FunPlant_UID={0},Project_UID={1},Mfg_Serial_Num=N'{2}',Process=N'{3}',
                           EQP_Plant_No=N'{4}',Asset=N'{5}',EQP_Location=N'{6}',Modified_UID={7},Modified_Date='{8}',Class_Desc=N'{9}',Model_Number=N'{10}',Mfg_Of_Asset=N'{11}',ConY=N'{12}',CM=N'{13}'
                           , BG_Organization_UID ={15}, FunPlant_Organization_UID ={16} WHERE EQP_Uid={14} ;";
            sql = string.Format(sql, dto.System_FunPlant_UID, dto.Project_UID, dto.Mfg_Serial_Num, dto.process, dto.EQP_Plant_No,
                            dto.Asset, dto.EQP_Location, dto.Modified_UID, DateTime.Now.ToString(FormatConstants.DateTimeFormatString), dto.Class_Desc, dto.Model_Number, dto.Mfg_Of_Asset, dto.ConY, dto.CM, dto.EQP_Uid, dto.BG_Organization_UID, dto.FunPlant_Organization_UID);

            //if (dto.BG_Organization_UID != 0 && dto.Project_UID != 0 && dto.Plant_Organization_UID != 0 && dto.FunPlant_Organization_UID!=null && dto.FunPlant_Organization_UID!=0)
            //{
                string sql1 = string.Format(@" UPDATE OEE_MachineInfo
                                               SET
                                                  BG_Organization_UID ={0}
                                                  , FunPlant_Organization_UID ={1}
                                                  , Project_UID ={2}
                                                  , Modify_UID ={3}
                                                  , Modify_Date ='{4}'
                                             WHERE EQP_Uid ={5}  and  Plant_Organization_UID ={6};", 
                                             dto.BG_Organization_UID, dto.FunPlant_Organization_UID, dto.Project_UID, dto.Modified_UID, DateTime.Now, dto.EQP_Uid, dto.Plant_Organization_UID);


                DataContext.Database.ExecuteSqlCommand(sql + sql1);
            //}
            //else
            //{
            //    DataContext.Database.ExecuteSqlCommand(sql);
            //}

        }

        public string InsertItem(List<EquipmentInfoDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        if (dtolist[i].Project_UID == null)
                            dtolist[i].Project_UID = 0;
                        if (dtolist[i].EQP_Uid != 0)
                        {
                            //var sql = string.Format(@"UPDATE dbo.Equipment_Info SET System_FunPlant_UID={0},Project_UID={1},Process=N'{2}',
                            //            CoCd=N'{3}',OpU=N'{4}',C=N'{5}',Process_Group=N'{6}',Class=N'{7}',Class_Desc=N'{8}',
                            //            Mfg_Of_Asset=N'{9}',Model_Number=N'{10}',Mfg_Serial_Num=N'{11}',Mfg_Part_Number=N'{12}',
                            //            Asset=N'{13}',User_Status=N'{14}',AM_CostCtr=N'{15}',Description_1=N'{16}'
                            //            ,Cap_date=N'{17}',Acquisition_Value={18},Asset_Life={19},Net_Book_Value={20},Monthly_Depreciation={21},
                            //            Remaining_Life={22},Func_Loc=N'{23}',Room=N'{24}',MCtry=N'{25}',Weight={26},Un=N'{27}',
                            //            Size_dimension=N'{28}',ConY=N'{29}',CM=N'{30}',Int_Note_L2=N'{31}',Description_2=N'{32}',
                            //            Characteristic_1=N'{33}',Description_3=N'{34}',Characteristic_2=N'{35}',Description_4=N'{36}',
                            //            Characteristic_3=N'{37}',Description_5=N'{38}',Characteristic_4=N'{39}',Description_6=N'{40}',
                            //            Characteristic_5=N'{41}',Int_Note_L1=N'{42}',EQP_Plant_No='{47}',
                            //            Modified_UID={43},Modified_Date=N'{44}', EQP_Location=N'{46}' 
                            //            WHERE EQP_Uid={45};",
                            //dtolist[i].System_FunPlant_UID, dtolist[i].Project_UID, dtolist[i].process, dtolist[i].CoCd,
                            //dtolist[i].OpU, dtolist[i].C, dtolist[i].Process_Group, dtolist[i].Class, dtolist[i].Class_Desc,
                            //dtolist[i].Mfg_Of_Asset, dtolist[i].Model_Number, dtolist[i].Mfg_Serial_Num, dtolist[i].Mfg_Part_Number,
                            //dtolist[i].Asset, dtolist[i].User_Status, dtolist[i].AM_CostCtr, dtolist[i].Description_1,
                            //dtolist[i].Cap_date, dtolist[i].Acquisition_Value, dtolist[i].Asset_Life, dtolist[i].Net_Book_Value,
                            //dtolist[i].Monthly_Depreciation, dtolist[i].Remaining_Life, dtolist[i].Func_Loc, dtolist[i].Room,
                            //dtolist[i].MCtry, dtolist[i].Weight, dtolist[i].Un, dtolist[i].Size_dimension, dtolist[i].ConY,
                            //dtolist[i].CM, dtolist[i].Int_Note_L2, dtolist[i].Description_2, dtolist[i].Characteristic_1,
                            //dtolist[i].Description_3, dtolist[i].Characteristic_2, dtolist[i].Description_4, dtolist[i].Characteristic_3,
                            //dtolist[i].Description_5, dtolist[i].Characteristic_4, dtolist[i].Description_6, dtolist[i].Characteristic_5,
                            //dtolist[i].Int_Note_L1, dtolist[i].Modified_UID, DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                            //dtolist[i].EQP_Uid, dtolist[i].EQP_Location, dtolist[i].EQP_Plant_No);
                            //if (dtolist[i].BG_Organization_UID != 0 && dtolist[i].Project_UID != 0 && dtolist[i].Plant_Organization_UID != 0 && dtolist[i].FunPlant_Organization_UID != null && dtolist[i].FunPlant_Organization_UID != 0)
                            //{
                            var sql = string.Format(@"UPDATE dbo.Equipment_Info SET System_FunPlant_UID={0},Project_UID={1},Process=N'{2}',
                                        CoCd=N'{3}',OpU=N'{4}',C=N'{5}',Process_Group=N'{6}',Class=N'{7}',Class_Desc=N'{8}',
                                        Mfg_Of_Asset=N'{9}',Model_Number=N'{10}',Mfg_Serial_Num=N'{11}',Mfg_Part_Number=N'{12}',
                                        Asset=N'{13}',User_Status=N'{14}',AM_CostCtr=N'{15}',Description_1=N'{16}'
                                        ,Cap_date=N'{17}',Acquisition_Value={18},Asset_Life={19},Net_Book_Value={20},Monthly_Depreciation={21},
                                        Remaining_Life={22},Func_Loc=N'{23}',Room=N'{24}',MCtry=N'{25}',Weight={26},Un=N'{27}',
                                        Size_dimension=N'{28}',ConY=N'{29}',CM=N'{30}',Int_Note_L2=N'{31}',Description_2=N'{32}',
                                        Characteristic_1=N'{33}',Description_3=N'{34}',Characteristic_2=N'{35}',Description_4=N'{36}',
                                        Characteristic_3=N'{37}',Description_5=N'{38}',Characteristic_4=N'{39}',Description_6=N'{40}',
                                        Characteristic_5=N'{41}',Int_Note_L1=N'{42}',EQP_Plant_No=N'{47}',
                                        Modified_UID={43},Modified_Date=N'{44}', EQP_Location=N'{46}',   
                                        BG_Organization_UID ={48} , FunPlant_Organization_UID ={49} 
                                        WHERE EQP_Uid={45};",
                dtolist[i].System_FunPlant_UID, dtolist[i].Project_UID, dtolist[i].process, dtolist[i].CoCd,
                dtolist[i].OpU, dtolist[i].C, dtolist[i].Process_Group, dtolist[i].Class, dtolist[i].Class_Desc,
                dtolist[i].Mfg_Of_Asset, dtolist[i].Model_Number, dtolist[i].Mfg_Serial_Num, dtolist[i].Mfg_Part_Number,
                dtolist[i].Asset, dtolist[i].User_Status, dtolist[i].AM_CostCtr, dtolist[i].Description_1,
                dtolist[i].Cap_date, dtolist[i].Acquisition_Value, dtolist[i].Asset_Life, dtolist[i].Net_Book_Value,
                dtolist[i].Monthly_Depreciation, dtolist[i].Remaining_Life, dtolist[i].Func_Loc, dtolist[i].Room,
                dtolist[i].MCtry, dtolist[i].Weight, dtolist[i].Un, dtolist[i].Size_dimension, dtolist[i].ConY,
                dtolist[i].CM, dtolist[i].Int_Note_L2, dtolist[i].Description_2, dtolist[i].Characteristic_1,
                dtolist[i].Description_3, dtolist[i].Characteristic_2, dtolist[i].Description_4, dtolist[i].Characteristic_3,
                dtolist[i].Description_5, dtolist[i].Characteristic_4, dtolist[i].Description_6, dtolist[i].Characteristic_5,
                dtolist[i].Int_Note_L1, dtolist[i].Modified_UID, DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                dtolist[i].EQP_Uid, dtolist[i].EQP_Location, dtolist[i].EQP_Plant_No, dtolist[i].BG_Organization_UID, dtolist[i].FunPlant_Organization_UID);
                                string sql1 = string.Format(@" UPDATE OEE_MachineInfo
                                               SET
                                                  BG_Organization_UID ={0}
                                                  , FunPlant_Organization_UID ={1}
                                                  , Project_UID ={2}
                                                  , Modify_UID ={3}
                                                  , Modify_Date ='{4}'
                                             WHERE EQP_Uid ={5}  and  Plant_Organization_UID ={6};",
                                             dtolist[i].BG_Organization_UID, dtolist[i].FunPlant_Organization_UID, dtolist[i].Project_UID, dtolist[i].Modified_UID, DateTime.Now, dtolist[i].EQP_Uid, dtolist[i].Plant_Organization_UID);

                                DataContext.Database.ExecuteSqlCommand(sql + sql1);
                            //}
                            //else
                            //{
                            //    DataContext.Database.ExecuteSqlCommand(sql);
                            //}
                        }
                        else
                        {
                            var sql = string.Format(@"insert into Equipment_Info values ({0},{1},N'{2}',N'{3}',N'{4}',N'{5}',N'{6}',N'{7}',N'{8}',N'{9}'
                                                    ,N'{10}',N'{11}',N'{12}',N'{13}',N'{14}',N'{15}',N'{16}',N'{17}',N'{18}',{19},{20},{21}
                                                    ,{22},{23},N'{24}',N'{25}',N'{26}',{27},N'{28}',N'{29}',N'{30}',N'{31}',N'{32}',N'{33}'
                                                    ,N'{34}',N'{35}',N'{36}',N'{37}',N'{38}',N'{39}',N'{40}',N'{41}',N'{42}',N'{43}',N'{44}',N'{45}'
                                                    ,{46},N'{47}',{48},{49},{50})",
                            dtolist[i].System_FunPlant_UID, dtolist[i].Project_UID, dtolist[i].process, dtolist[i].CoCd, dtolist[i].OpU, dtolist[i].C, dtolist[i].Process_Group, dtolist[i].Class,
                            dtolist[i].Class_Desc, dtolist[i].Mfg_Of_Asset, dtolist[i].Model_Number, dtolist[i].Mfg_Serial_Num,
                            dtolist[i].Mfg_Part_Number, dtolist[i].Equipment, dtolist[i].Asset, dtolist[i].User_Status,
                            dtolist[i].AM_CostCtr, dtolist[i].Description_1, dtolist[i].Cap_date, dtolist[i].Acquisition_Value,
                            dtolist[i].Asset_Life, dtolist[i].Net_Book_Value, dtolist[i].Monthly_Depreciation, dtolist[i].Remaining_Life,
                            dtolist[i].Func_Loc, dtolist[i].Room, dtolist[i].MCtry, dtolist[i].Weight, dtolist[i].Un,
                            dtolist[i].Size_dimension, dtolist[i].ConY, dtolist[i].CM, dtolist[i].Int_Note_L2, dtolist[i].Description_2,
                            dtolist[i].Characteristic_1, dtolist[i].Description_3, dtolist[i].Characteristic_2, dtolist[i].Description_4,
                            dtolist[i].Characteristic_3, dtolist[i].Description_5, dtolist[i].Characteristic_4, dtolist[i].Description_6,
                            dtolist[i].Characteristic_5, dtolist[i].Int_Note_L1, dtolist[i].EQP_Plant_No, dtolist[i].EQP_Location, dtolist[i].Modified_UID,
                            DateTime.Now.ToString(FormatConstants.DateTimeFormatString), dtolist[i].Plant_Organization_UID, dtolist[i].BG_Organization_UID, dtolist[i].FunPlant_Organization_UID);
                            DataContext.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    result = "Error" + ex.Message;
                }
                return result;
            }
        }

        public List<EquipmentInfoDTO> GetDistinctoptype()
        {
            string sql = @"SELECT DISTINCT	 t2.OP_TYPES FROM dbo.Equipment_Info t1 INNER JOIN dbo.System_Project t2
                        ON t1.Project_UID=t2.Project_UID ";
            var dblist = DataContext.Database.SqlQuery<EquipmentInfoDTO>(sql).ToList();
            return dblist;
        }
        public List<System_Organization_PlantDTO> GetOrganization_Plants()
        {
            string sql = @"SELECT System_FunPlant_UID
                                  ,System_Plant_UID
                                  ,OP_Types
                                  ,FunPlant 
                                  ,OPType_OrganizationUID
                                  ,FunPlant_OrganizationUID
	                              ,b.ChildOrg_UID
	                              ,b.ParentOrg_UID
                              FROM System_Function_Plant a left join System_OrganizationBOM b on  a.OPType_OrganizationUID=b.ChildOrg_UID ";
            var dblist = DataContext.Database.SqlQuery<System_Organization_PlantDTO>(sql).ToList();
            return dblist;
        }
        public List<SystemProjectDTO> GetDistinctoptypeByUser(int optype)
        {
            string sql = @"SELECT DISTINCT	 t2.OP_TYPES,t2.Organization_UID FROM dbo.Equipment_Info t1 INNER JOIN dbo.System_Project t2
                        ON t1.Project_UID=t2.Project_UID ";
            if (optype != 0)
                sql += " where t2.Organization_UID=" + optype;
            var dblist = DataContext.Database.SqlQuery<SystemProjectDTO>(sql).ToList();
            return dblist;
        }

        public List<SystemRoleDTO> Getuserrole(string userid)
        {
            string sql = @"SELECT * FROM dbo.System_User_Role t1 JOIN  System_Role t2
                            ON t1.Role_UID=t2.Role_UID WHERE t1.Account_UID='{0}' ";
            sql = string.Format(sql, userid);
            var dblist = DataContext.Database.SqlQuery<SystemRoleDTO>(sql).ToList();
            return dblist;
        }

        public List<EnumerationDTO> QueryDistinctReason()
        {
            string sql = @"SELECT DISTINCT Enum_UID,Enum_Value FROM dbo.Enumeration WHERE Enum_Type =N'EQPReason_Type'";
            var dblist = DataContext.Database.SqlQuery<EnumerationDTO>(sql).ToList();
            return dblist;
        }


        public List<EquipmentInfoDTO> GetProjectnameByOptype(string Optype)
        {
            string sql = @"SELECT DISTINCT t2.Project_UID,t2.Project_Name FROM dbo.Equipment_Info t1 LEFT JOIN dbo.System_Project t2
                        ON t1.Project_UID=t2.Project_UID WHERE  t2.OP_TYPES=N'{0}'";
            sql = string.Format(sql, Optype);
            var dblist = DataContext.Database.SqlQuery<EquipmentInfoDTO>(sql).ToList();
            return dblist;
        }

        public List<SystemFunctionPlantDTO> GetFunPlantByOPType(int Optype, string Optypes = "")
        {
            string sql = @"SELECT DISTINCT FunPlant,System_FunPlant_UID ,OPType_OrganizationUID, FunPlant_OrganizationUID FROM dbo.System_Function_Plant 
                           WHERE OPType_OrganizationUID={0}";
            if (Optypes != null && Optypes != "")
            {
                sql = @"SELECT DISTINCT FunPlant,System_FunPlant_UID ,OPType_OrganizationUID, FunPlant_OrganizationUID FROM dbo.System_Function_Plant 
                           WHERE OPType_OrganizationUID={0}  and  OP_Types='{1}'";
            }
            sql = string.Format(sql, Optype, Optypes);
            var dblist = DataContext.Database.SqlQuery<SystemFunctionPlantDTO>(sql).ToList();
            return dblist;
        }

        public List<EquipmentInfoDTO> GetProcessByFunplant(string funplantuid)
        {
            string sql = @"SELECT DISTINCT t1.Process FROM dbo.Equipment_Info t1 LEFT JOIN dbo.System_Project t2 ON t1.Project_UID
                    =t2.Project_UID LEFT JOIN dbo.System_Function_Plant t3 ON t1.System_FunPlant_UID=t3.System_FunPlant_UID
                    WHERE t1.System_FunPlant_UID={0}";
            sql = string.Format(sql, funplantuid);
            var dblist = DataContext.Database.SqlQuery<EquipmentInfoDTO>(sql).ToList();
            return dblist;
        }

        public EquipmentInfoDTO GetEqpBySerialAndEMT(string serial, string emt_num)
        {
            string sql = @" SELECT *
                            FROM dbo.Equipment_Info
                            WHERE Mfg_Serial_Num='{0}' AND Equipment='{1}'";
            sql = string.Format(sql, serial.Replace("'", string.Empty), emt_num.Replace("'", string.Empty));
            var dblist = DataContext.Database.SqlQuery<EquipmentInfoDTO>(sql).FirstOrDefault();
            return dblist;
        }

        public List<string> GetEqpidByProcess(string funplantuid)
        {
            string sql = @"SELECT distinct Equipment FROM dbo.Equipment_Info WHERE  System_FunPlant_UID = {0} ";
            sql = string.Format(sql, funplantuid);
            var dblist = DataContext.Database.SqlQuery<string>(sql).ToList();
            return dblist;
        }

        public List<EquipmentInfoDTO> GetInfoByUserid(string location, int funplant, int optype)
        {
            string sql = @"SELECT * FROM dbo.Equipment_Info t1 inner join System_Project t2
                                        on  t2.Project_UID = t1.Project_UID WHERE EQP_Location LIKE N'%{0}%' AND System_FunPlant_UID = 
                                        {1} and t2.Organization_UID={2} ";
            sql = string.Format(sql, location, funplant, optype);
            var dblist = DataContext.Database.SqlQuery<EquipmentInfoDTO>(sql).ToList();
            return dblist;
        }

        public List<string> GetDistinctLocation( int Plant_Organization_UID )
        {
            string sql = "SELECT DISTINCT EQP_Location FROM dbo.Equipment_Info order by EQP_Location";
            //string sql = "";
            if (Plant_Organization_UID != 0)
            {
                sql = string.Format(@"SELECT  EQP_Location  FROM dbo.Equipment_Info where Plant_Organization_UID={0} group by EQP_Location,Plant_Organization_UID", Plant_Organization_UID);
            }   
            var dblist = DataContext.Database.SqlQuery<string>(sql).ToList();
            return dblist;
        }

        public List<SystemFunctionPlantDTO> GetFunplantByUser(int userid, int optypeuid)
        {
            string sql = "";
            if (userid == 0)
            {
                sql = string.Format(@"SELECT * FROM dbo.System_Function_Plant WHERE OPType_OrganizationUID={0}  AND
                                     OP_Types IN(SELECT Organization_Name FROM System_Organization WHERE Organization_UID={0})
                                     ", optypeuid);
            }
            else
            {
                sql = @"SELECT t2.* FROM dbo.System_UserOrg t1 INNER JOIN dbo.System_Function_Plant t2
                ON t1.Funplant_OrganizationUID=t2.FunPlant_OrganizationUID WHERE Account_UID={0}
                AND t2.OPType_OrganizationUID={1}";
                sql = string.Format(sql, userid, optypeuid);
            }

            sql = string.Format(sql, userid);
            var dblist = DataContext.Database.SqlQuery<SystemFunctionPlantDTO>(sql).ToList();
            return dblist;
        }

        public List<string> GetOPTypeByUser(int userid)
        {
            string sql = @"SELECT t2.Organization_Name FROM dbo.System_UserOrg t1 INNER JOIN dbo.System_Organization t2
                            ON t1.OPType_OrganizationUID=t2.Organization_UID WHERE Account_UID={0}";
            sql = string.Format(sql, userid);
            var dblist = DataContext.Database.SqlQuery<string>(sql).ToList();
            return dblist;
        }

        //获取未使用机台报表
        public IQueryable<EquipmentReport> GetEquipmentInfoNOTReprot(EquipmentReport searchModel, List<int> Organization_UIDs, Page page, out int totalcount)
        {

            string sql = @" SELECT 
	                     B. System_FunPlant_UID,
	                     B. Project_UID,
	                     E. Project_Name, 
	                     B. Class_Desc ,
	                     B. Mfg_Of_Asset,
                         COUNT(1) AS SUMALL,
                         A.OPType_OrganizationUID,
                         A. FunPlant_OrganizationUID,
                         C.Organization_Name AS OP_Name,
                         D. Organization_Name AS FunPlant_Name			
	                    FROM Equipment_Info B 
						LEFT JOIN System_Function_Plant A ON  B. System_FunPlant_UID= A. System_FunPlant_UID 						
					    LEFT JOIN System_Organization C ON C.Organization_UID=A.OPType_OrganizationUID
						LEFT JOIN System_Organization D ON  D.Organization_UID=A.FunPlant_OrganizationUID						 
						LEFT JOIN System_Project E ON B.Project_UID=E.Project_UID	
						WHERE B.Project_UID=0 										
						GROUP BY B.Project_UID, B.System_FunPlant_UID,B.Class_Desc,B.Mfg_Of_Asset,A.OPType_OrganizationUID,
                        A. FunPlant_OrganizationUID, C.Organization_Name,D. Organization_Name,E.Project_Name ";

            var dblist = DataContext.Database.SqlQuery<EquipmentReport>(sql).ToList();
            var query = dblist.AsQueryable();
            // 只是针对厂区的过滤
            if (Organization_UIDs.Count > 0)
            {
                query = query.Where(m => Organization_UIDs.Contains(m.OPType_OrganizationUID));
            }

            if (searchModel.Organization_UID > 0)
            {
                query = query.Where(m => m.OPType_OrganizationUID == searchModel.Organization_UID);

            }
            if (searchModel.OPType_OrganizationUID > 0)
            {
                query = query.Where(m => m.OPType_OrganizationUID == searchModel.OPType_OrganizationUID);

            }
            if (searchModel.FunPlant_OrganizationUID > 0)
            {
                var FunPlant_Name = DataContext.System_Organization.Where(o => o.Organization_UID == searchModel.FunPlant_OrganizationUID).FirstOrDefault().Organization_Name;
                query = query.Where(m => m.FunPlant_Name == FunPlant_Name);
                // query = query.Where(m => m.FunPlant_OrganizationUID == searchModel.FunPlant_OrganizationUID);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Class_Desc))
            {
                query = query.Where(m => m.Class_Desc.ToLower().Contains(searchModel.Class_Desc.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Mfg_Of_Asset))
            {
                query = query.Where(m => m.Mfg_Of_Asset.ToLower().Contains(searchModel.Mfg_Of_Asset.ToLower()));
            }
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.OP_Name).ThenBy(m => m.Project_Name).ThenBy(m => m.FunPlant_Name).ThenBy(m => m.Class_Desc).ThenBy(m => m.Mfg_Of_Asset).GetPage(page);
            return query;

        }

        //获取使用机台报表
        public IQueryable<EquipmentReport> GetEquipmentInfoReprot(EquipmentReport searchModel, List<int> Organization_UIDs, Page page, out int totalcount)
        {

            string sql = @"SELECT 
	                     B. System_FunPlant_UID,
	                     B. Project_UID,
	                     E. Project_Name, 
	                     B. Class_Desc ,
	                     B. Mfg_Of_Asset,
                         COUNT(1) AS SUMALL,
                         A.OPType_OrganizationUID,
                         A. FunPlant_OrganizationUID,
                         C.Organization_Name AS OP_Name,
                         D. Organization_Name AS FunPlant_Name
					     FROM Equipment_Info B 
						LEFT JOIN System_Function_Plant A ON  B. System_FunPlant_UID= A. System_FunPlant_UID 						
					    LEFT JOIN System_Organization C ON C.Organization_UID=A.OPType_OrganizationUID
						LEFT JOIN System_Organization D ON  D.Organization_UID=A.FunPlant_OrganizationUID						 
						LEFT JOIN System_Project E ON B.Project_UID=E.Project_UID	
						WHERE B.Project_UID<>0										
						GROUP BY B.Project_UID, B.System_FunPlant_UID,B.Class_Desc,B.Mfg_Of_Asset,A.OPType_OrganizationUID,
                        A. FunPlant_OrganizationUID, C.Organization_Name,D. Organization_Name,E.Project_Name ";
            var dblist = DataContext.Database.SqlQuery<EquipmentReport>(sql).ToList();

            // 只是针对厂区的过滤
            if (Organization_UIDs.Count > 0)
            {
                dblist = dblist.Where(m => Organization_UIDs.Contains(m.OPType_OrganizationUID)).ToList();
            }

            if (searchModel.Organization_UID > 0)
            {
                dblist = dblist.Where(m => m.OPType_OrganizationUID == searchModel.Organization_UID).ToList();

            }
            if (searchModel.OPType_OrganizationUID > 0)
            {
                dblist = dblist.Where(m => m.OPType_OrganizationUID == searchModel.OPType_OrganizationUID).ToList();

            }
            if (searchModel.FunPlant_OrganizationUID > 0)
            {
                var FunPlant_Name = DataContext.System_Organization.Where(o => o.Organization_UID == searchModel.FunPlant_OrganizationUID).FirstOrDefault().Organization_Name;
                dblist = dblist.Where(m => m.FunPlant_Name == FunPlant_Name).ToList();
                // dblist = dblist.Where(m => m.FunPlant_OrganizationUID == searchModel.FunPlant_OrganizationUID).ToList();
            }
            if (searchModel.Project_UID > 0)
            {
                dblist = dblist.Where(m => m.Project_UID == searchModel.Project_UID).ToList();
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Class_Desc))
            {
                dblist = dblist.Where(m => m.Class_Desc.ToLower().Contains(searchModel.Class_Desc.ToLower())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Mfg_Of_Asset))
            {
                dblist = dblist.Where(m => m.Mfg_Of_Asset.ToLower().Contains(searchModel.Mfg_Of_Asset.ToLower())).ToList();
            }
            string sql1 = @" SELECT B.System_FunPlant_UID,
                         B.Project_UID,
                         B.Class_Desc ,
                         B.Mfg_Of_Asset,
                         A.Repair_Result,
                         C.OPType_OrganizationUID,
                         C.FunPlant_OrganizationUID
                         FROM EQPRepair_Info A, Equipment_Info B, System_Function_Plant C
                         WHERE A.EQP_Uid= B.EQP_Uid AND B. System_FunPlant_UID= C.System_FunPlant_UID ";

            var dblist1 = DataContext.Database.SqlQuery<EquipmentReport>(sql1).ToList();

            foreach (var item in dblist)
            {
                //带备品
                int SumMaintenance = 0;
                //维修中
                int SumSpareparts = 0;
                //可用数量
                int SumAvailable = 0;
                var dblist2 = dblist1.Where(o => o.OPType_OrganizationUID == item.OPType_OrganizationUID && o.FunPlant_OrganizationUID == item.FunPlant_OrganizationUID && o.Project_UID == item.Project_UID && o.Class_Desc == item.Class_Desc && o.Mfg_Of_Asset == item.Mfg_Of_Asset).ToList();
                if (dblist2.Count > 0)
                {
                    SumMaintenance = dblist2.Where(o => o.Repair_Result == "待备品").ToList().Count;
                    SumSpareparts = dblist2.Where(o => o.Repair_Result == "维修中").ToList().Count;

                }

                SumAvailable = item.SumALL - SumMaintenance - SumSpareparts;
                item.SumMaintenance = SumMaintenance;
                item.SumSpareparts = SumSpareparts;
                item.SumAvailable = SumAvailable;
                item.AvailableRate = 1.0*item.SumAvailable / item.SumALL;
            }

            var query = dblist.AsQueryable();
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.OP_Name).ThenBy(m => m.Project_Name).ThenBy(m => m.FunPlant_Name).ThenBy(m => m.Class_Desc).ThenBy(m => m.Mfg_Of_Asset).GetPage(page);

            //   query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;

        }

        //获取未使用机台报表
        public List<EquipmentReport> GetEquipmentInfoNOTReprot(EquipmentReport searchModel, List<int> Organization_UIDs)
        {

            string sql = @" SELECT 
	                     B. System_FunPlant_UID,
	                     B. Project_UID,
	                     E. Project_Name, 
	                     B. Class_Desc ,
	                     B. Mfg_Of_Asset,
                         COUNT(1) AS SUMALL,
                         A.OPType_OrganizationUID,
                         A. FunPlant_OrganizationUID,
                         C.Organization_Name AS OP_Name,
                         D. Organization_Name AS FunPlant_Name			
	                    FROM Equipment_Info B 
						LEFT JOIN System_Function_Plant A ON  B. System_FunPlant_UID= A. System_FunPlant_UID 						
					    LEFT JOIN System_Organization C ON C.Organization_UID=A.OPType_OrganizationUID
						LEFT JOIN System_Organization D ON  D.Organization_UID=A.FunPlant_OrganizationUID						 
						LEFT JOIN System_Project E ON B.Project_UID=E.Project_UID	
						WHERE B.Project_UID=0 										
						GROUP BY B.Project_UID, B.System_FunPlant_UID,B.Class_Desc,B.Mfg_Of_Asset,A.OPType_OrganizationUID,
                        A. FunPlant_OrganizationUID, C.Organization_Name,D. Organization_Name,E.Project_Name ";
            var dblist = DataContext.Database.SqlQuery<EquipmentReport>(sql).ToList();
            // var query = dblist.AsQueryable();
            // 只是针对厂区的过滤
            if (Organization_UIDs.Count > 0)
            {
                dblist = dblist.Where(m => Organization_UIDs.Contains(m.OPType_OrganizationUID)).ToList();
            }

            if (searchModel.Organization_UID > 0)
            {
                dblist = dblist.Where(m => m.OPType_OrganizationUID == searchModel.Organization_UID).ToList();

            }
            if (searchModel.OPType_OrganizationUID > 0)
            {
                dblist = dblist.Where(m => m.OPType_OrganizationUID == searchModel.OPType_OrganizationUID).ToList();

            }
            if (searchModel.FunPlant_OrganizationUID > 0)
            {
                var FunPlant_Name = DataContext.System_Organization.Where(o => o.Organization_UID == searchModel.FunPlant_OrganizationUID).FirstOrDefault().Organization_Name;
                dblist = dblist.Where(m => m.FunPlant_Name == FunPlant_Name).ToList();
                // dblist = dblist.Where(m => m.FunPlant_OrganizationUID == searchModel.FunPlant_OrganizationUID).ToList();
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Class_Desc))
            {
                dblist = dblist.Where(m => m.Class_Desc.ToLower().Contains(searchModel.Class_Desc.ToLower())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Mfg_Of_Asset))
            {
                dblist = dblist.Where(m => m.Mfg_Of_Asset.ToLower().Contains(searchModel.Mfg_Of_Asset.ToLower())).ToList();
            }

            dblist = dblist.OrderByDescending(m => m.OP_Name).ThenBy(m => m.Project_Name).ThenBy(m => m.FunPlant_Name).ThenBy(m => m.Class_Desc).ThenBy(m => m.Mfg_Of_Asset).ToList();

            return dblist;

        }

        //获取使用机台报表
        public List<EquipmentReport> GetEquipmentInfoReprot(EquipmentReport searchModel, List<int> Organization_UIDs)
        {

            string sql = @"SELECT 
	                     B. System_FunPlant_UID,
	                     B. Project_UID,
	                     E. Project_Name, 
	                     B. Class_Desc ,
	                     B. Mfg_Of_Asset,
                         COUNT(1) AS SUMALL,
                         A.OPType_OrganizationUID,
                         A. FunPlant_OrganizationUID,
                         C.Organization_Name AS OP_Name,
                         D. Organization_Name AS FunPlant_Name
				  	FROM Equipment_Info B 
						LEFT JOIN System_Function_Plant A ON  B. System_FunPlant_UID= A. System_FunPlant_UID 						
					    LEFT JOIN System_Organization C ON C.Organization_UID=A.OPType_OrganizationUID
						LEFT JOIN System_Organization D ON  D.Organization_UID=A.FunPlant_OrganizationUID						 
						LEFT JOIN System_Project E ON B.Project_UID=E.Project_UID	
						WHERE B.Project_UID<>0										
						GROUP BY B.Project_UID, B.System_FunPlant_UID,B.Class_Desc,B.Mfg_Of_Asset,A.OPType_OrganizationUID,
                        A. FunPlant_OrganizationUID, C.Organization_Name,D. Organization_Name,E.Project_Name ";
            var dblist = DataContext.Database.SqlQuery<EquipmentReport>(sql).ToList();
            // var query = dblist.AsQueryable();
            // 只是针对厂区的过滤
            if (Organization_UIDs.Count > 0)
            {
                dblist = dblist.Where(m => Organization_UIDs.Contains(m.OPType_OrganizationUID)).ToList();
            }

            if (searchModel.Organization_UID > 0)
            {
                dblist = dblist.Where(m => m.OPType_OrganizationUID == searchModel.Organization_UID).ToList();

            }
            if (searchModel.OPType_OrganizationUID > 0)
            {
                dblist = dblist.Where(m => m.OPType_OrganizationUID == searchModel.OPType_OrganizationUID).ToList();

            }
            if (searchModel.FunPlant_OrganizationUID > 0)
            {
                var FunPlant_Name = DataContext.System_Organization.Where(o => o.Organization_UID == searchModel.FunPlant_OrganizationUID).FirstOrDefault().Organization_Name;
                dblist = dblist.Where(m => m.FunPlant_Name == FunPlant_Name).ToList();
                // dblist = dblist.Where(m => m.FunPlant_OrganizationUID == searchModel.FunPlant_OrganizationUID).ToList();
            }
            if (searchModel.Project_UID > 0)
            {
                dblist = dblist.Where(m => m.Project_UID == searchModel.Project_UID).ToList();
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Class_Desc))
            {
                dblist = dblist.Where(m => m.Class_Desc.ToLower().Contains(searchModel.Class_Desc.ToLower())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Mfg_Of_Asset))
            {
                dblist = dblist.Where(m => m.Mfg_Of_Asset.ToLower().Contains(searchModel.Mfg_Of_Asset.ToLower())).ToList();
            }

            string sql1 = @" SELECT B.System_FunPlant_UID,
                         B.Project_UID,
                         B.Class_Desc ,
                         B.Mfg_Of_Asset,
                         A.Repair_Result,
                         C.OPType_OrganizationUID,
                         C.FunPlant_OrganizationUID
                         FROM EQPRepair_Info A, Equipment_Info B, System_Function_Plant C
                         WHERE A.EQP_Uid= B.EQP_Uid AND B. System_FunPlant_UID= C.System_FunPlant_UID ";

            var dblist1 = DataContext.Database.SqlQuery<EquipmentReport>(sql1).ToList();
            foreach (var item in dblist)
            {
                //带备品
                int SumMaintenance = 0;
                //维修中
                int SumSpareparts = 0;
                //可用数量
                int SumAvailable = 0;
                var dblist2 = dblist1.Where(o => o.OPType_OrganizationUID == item.OPType_OrganizationUID && o.FunPlant_OrganizationUID == item.FunPlant_OrganizationUID && o.Project_UID == item.Project_UID && o.Class_Desc == item.Class_Desc && o.Mfg_Of_Asset == item.Mfg_Of_Asset).ToList();
                if (dblist2.Count > 0)
                {
                    SumMaintenance = dblist2.Where(o => o.Repair_Result == "待备品").ToList().Count;
                    SumSpareparts = dblist2.Where(o => o.Repair_Result == "维修中").ToList().Count;

                }
                SumAvailable = item.SumALL - SumMaintenance - SumSpareparts;
                item.SumMaintenance = SumMaintenance;
                item.SumSpareparts = SumSpareparts;
                item.SumAvailable = SumAvailable;

            }
            dblist = dblist.OrderByDescending(m => m.OP_Name).ThenBy(m => m.Project_Name).ThenBy(m => m.FunPlant_Name).ThenBy(m => m.Class_Desc).ThenBy(m => m.Mfg_Of_Asset).ToList();
            return dblist;

        }

    }
}
