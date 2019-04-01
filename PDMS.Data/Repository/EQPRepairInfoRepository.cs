using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using System.Data.Entity.SqlServer;
using PDMS.Common.Constants;

namespace PDMS.Data.Repository
{
    public interface IEQPRepairInfoRepository : IRepository<EQPRepair_Info>
    {
        int GetInfoCount(DateTime date);
        IQueryable<EQPRepairInfoDTO> GetInfo(EQPRepairInfoSearchDTO searchModel, List<int> Organization_UIDs, Page page, out int totalcount, out decimal allcost);
        EQPRepairInfoDTO GetByUId(int Repair_Uid);
        string DeleteByUid(int Repair_Uid);
        List<EQPRepairInfoDTO> DoExportFunction(string uids);
        List<EQPRepairInfoDTO> DoAllEQPMaintenanceReprot(EQPRepairInfoSearchDTO searchModel, List<int> Organization_UIDs);
        IQueryable<EQPRepairInfoDTO> GetReportInfo(EQPRepairInfoSearchDTO searchModel, List<int> Organization_UIDs, Page page, out int totalcount, out decimal allcost);
        List<EQPRepairInfoDTO> DoExportFunction2(string optypes, string projectname, string funplant, string process,
                        string eqpid, string errortype, string repairid, string location, string classdesc, string contact,
                        DateTime fromdate, DateTime todate, string errorlever, string repairresult, string labor,
                        string updatepart, string remark, string status);

        List<EQPRepairInfoDTO> DoPartExportFunctionInfo(string uids);
        string ClosedByUid(int Repair_Uid);
        //电子看板 
        IQueryable<EQPRepairInfoDTO> GetEQPBoardInfo(EQPRepairInfoDTO searchModel, Page page, out int totalcount);
        //仓库管理
        List<EQPRepairInfoDTO> GetSingleEQPRepair(string Repair_id);
        //取得返修品中的前三个月损坏量
        List<int> GetF3MDamageQty(int Material_Uid, string EQP_Type);
        //判斷成本中心是否被引用
        bool CheckCostCtrIsExist(int CostCtr_Uid);
    }
    public class EQPRepairInfoRepository : RepositoryBase<EQPRepair_Info>, IEQPRepairInfoRepository
    {
        public EQPRepairInfoRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public int GetInfoCount(DateTime date)
        {
            var count = DataContext.EQPRepair_Info.Where(i => i.Error_Time == date).Count();
            return count + 1;
        }

        public IQueryable<EQPRepairInfoDTO> GetInfo(EQPRepairInfoSearchDTO searchModel, List<int> Organization_UIDs, Page page, out int totalcount, out decimal allcost)
        {


            var query = from M in DataContext.EQPRepair_Info
                        join equipinfo in DataContext.Equipment_Info
                        on M.EQP_Uid equals equipinfo.EQP_Uid
                        join project in DataContext.System_Project
                        on equipinfo.Project_UID equals project.Project_UID into temp
                        from aa in temp.DefaultIfEmpty()
                        join funplant in DataContext.System_Function_Plant
                        on equipinfo.System_FunPlant_UID equals funplant.System_FunPlant_UID
                        select new EQPRepairInfoDTO
                        {
                            EQP_Location = equipinfo.EQP_Location,
                            Class_Desc = equipinfo.Class_Desc,
                            EQP_Plant_No = equipinfo.EQP_Plant_No,
                            Repair_Uid = M.Repair_Uid,
                            Equipment = equipinfo.Equipment,
                            Status = M.Status,
                            Error_Types = M.Error_Types,
                            Error_Level = M.Error_Level,
                            Contact = M.Contact,
                            Contact_tel = M.Contact_tel,
                            Error_Time = M.Error_Time,
                            Repair_BeginTime = M.Repair_BeginTime,
                            Repair_EndTime = M.Repair_EndTime,
                            TotalTime = Math.Round((double)(SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime) == null ? 0 : (int)SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime)) / 60, 2),
                            Labor_List = M.Labor_List,
                            Update_Part = M.Update_Part,
                            Labor_Time = M.Labor_Time,
                            All_RepairCost = M.All_RepairCost,
                            Modified_Date = M.Modified_Date,
                            Process = equipinfo.Process,
                            Reason_Types = M.Reason_Types,
                            Repair_Reason = M.Repair_Reason,
                            Reason_Analysis = M.Reason_Analysis,
                            Repair_Remark = M.Repair_Remark,
                            OP_TYPES = funplant.OP_Types,
                            Mfg_Serial_Num = equipinfo.Mfg_Serial_Num,
                            Project_Name = aa.Project_Name,
                            FunPlant = funplant.FunPlant,
                            Repair_id = M.Repair_id,
                            Repair_Result = M.Repair_Result,
                            Repair_Method = M.Repair_Method,
                            Apply_Time = (DateTime)M.Apply_Time,
                            Organization_UID = aa.Organization_UID,
                            FunPlant_OrganizationUID = funplant.FunPlant_OrganizationUID.Value,
                            Mentioner=M.Mentioner,
                            CostCtr_UID = M.CostCtr_UID,
                            CostCtr_ID =M.CostCtr_info.CostCtr_ID,
                            CostCtr_Description=M.CostCtr_info.CostCtr_Description,
                            //Modified_Date=M.Modified_Date,
                        };

            if (searchModel.Organization_UID != 0)
                query = query.Where(m => m.Organization_UID == searchModel.Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Project_Name))
                query = query.Where(m => m.Project_Name.Contains(searchModel.Project_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.FunPlant))
                query = query.Where(m => m.FunPlant.Contains(searchModel.FunPlant));
            if (searchModel.CostCtr_UID!=0)
                query = query.Where(m => m.CostCtr_UID == searchModel.CostCtr_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Process))
                query = query.Where(m => m.Process.Contains(searchModel.Process));
            if (!string.IsNullOrWhiteSpace(searchModel.Mfg_Serial_Num))
                query = query.Where(m => m.Mfg_Serial_Num.Contains(searchModel.Mfg_Serial_Num));
            if (!string.IsNullOrWhiteSpace(searchModel.Error_Types))
                query = query.Where(m => m.Error_Types.Contains(searchModel.Error_Types));
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_id))
                query = query.Where(m => m.Repair_id.Contains(searchModel.Repair_id));
            if (!string.IsNullOrWhiteSpace(searchModel.EQP_Location))
                query = query.Where(m => m.EQP_Location == searchModel.EQP_Location);
            if (!string.IsNullOrWhiteSpace(searchModel.Class_Desc))
                query = query.Where(m => m.Class_Desc.Contains(searchModel.Class_Desc));
            if (!string.IsNullOrWhiteSpace(searchModel.Contact))
                query = query.Where(m => m.Contact.Contains(searchModel.Contact));
            if (searchModel.End_Date_From.Year != 1)
                query = query.Where(m => m.Repair_EndTime >= searchModel.End_Date_From);
            if (!string.IsNullOrWhiteSpace(searchModel.Error_Level))
                query = query.Where(m => m.Error_Level == searchModel.Error_Level);
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Result))
                query = query.Where(m => m.Repair_Result == searchModel.Repair_Result);
            if (!string.IsNullOrWhiteSpace(searchModel.Update_Part))
                query = query.Where(m => m.Update_Part.Contains(searchModel.Update_Part));
            if (!string.IsNullOrWhiteSpace(searchModel.Labor_List))
                query = query.Where(m => m.Labor_List.Contains(searchModel.Labor_List));
            if (!string.IsNullOrWhiteSpace(searchModel.Status))
                query = query.Where(m => m.Status == searchModel.Status);
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Result))
                query = query.Where(m => m.Repair_Result == searchModel.Repair_Result);
            if (!string.IsNullOrWhiteSpace(searchModel.Mentioner))
                    query = query.Where(m => m.Mentioner .Contains(searchModel.Mentioner));
            if (searchModel.End_Date_To.Year != 1)
            {
                searchModel.End_Date_To = searchModel.End_Date_To.AddDays(1);
                query = query.Where(m => m.Repair_EndTime < searchModel.End_Date_To);
            }
            //根据功能厂过滤
            if (searchModel.FunPlant_OrganizationUID != 0)
            {
                query = query.Where(m => m.FunPlant_OrganizationUID == searchModel.FunPlant_OrganizationUID);
            }
            // 只是针对厂区的过滤
            if (Organization_UIDs.Count > 0)
                query = query.Where(m => Organization_UIDs.Contains(m.Organization_UID));

        
            totalcount = query.Count();
            if (totalcount == 0)
                allcost = 0;
            else
                allcost = (decimal)query.Sum(m => (SqlFunctions.DateDiff("MINUTE", m.Repair_BeginTime, m.Repair_EndTime)) == null ? 0 : (SqlFunctions.DateDiff("MINUTE", m.Repair_BeginTime, m.Repair_EndTime)));
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Contact).GetPage(page);
            return query;
        }

        public EQPRepairInfoDTO GetByUId(int Repair_Uid)
        {
            string sql = @"SELECT Error_Types,Apply_Time,Error_Level,Repair_id,Equipment,Reason_Types,Repair_Method,Repair_Result
                          ,Mentioner ,ISNULL(Repair_BeginTime,GETDATE()) Repair_BeginTime,t1.Repair_Uid,t1.EQP_Uid,t2.System_FunPlant_UID,
                            ISNULL(Repair_EndTime,GETDATE()) Repair_EndTime,isnull(Error_Time,GETDATE()) Error_Time,
                             Contact,Contact_tel,Reason_Analysis,Repair_Reason,Repair_Remark,Asset,EQP_Plant_No,EQP_Location,
                            Mfg_Serial_Num,OP_Types,FunPlant,Process,EQP_Plant_No,Class_Desc,t3.OPType_OrganizationUID Organization_UID,t3.FunPlant_OrganizationUID 
                            ,t4.CostCtr_UID,t4.CostCtr_ID,t4.CostCtr_Description 
                            FROM dbo.EQPRepair_Info t1 
                            inner join Equipment_Info t2 on t1.EQP_Uid=t2.EQP_Uid 
                            inner join dbo.System_Function_Plant t3 on t2.System_FunPlant_UID=t3.System_FunPlant_UID  
                            left join CostCtr_info t4 on t1.CostCtr_UID=t4.CostCtr_UID 
                            where Repair_Uid={0}";
            sql = string.Format(sql, Repair_Uid);
            var dblist = DataContext.Database.SqlQuery<EQPRepairInfoDTO>(sql).ToList();
            return dblist[0];
        }

        public string DeleteByUid(int Repair_Uid)
        {
            try
            {
                string sql = "delete  EQPRepair_Info where Repair_Uid={0}";
                sql = string.Format(sql, Repair_Uid);
                int result = DataContext.Database.ExecuteSqlCommand(sql);
                if (result == 1)
                    return "";
                else
                    return "删除维修记录失败";
            }
            catch (Exception e)
            {
                return "删除维修记录失败:" + e.Message;
            }
        }

        public string ClosedByUid(int Repair_Uid)
        {
            try
            {
                string sql = "update EQPRepair_Info set Status='Closed' where Repair_Uid={0}";
                sql = string.Format(sql, Repair_Uid);
                int result = DataContext.Database.ExecuteSqlCommand(sql);
                if (result == 1)
                    return "";
                else
                    return "删除维修记录失败";
            }
            catch (Exception e)
            {
                return "删除维修记录失败:" + e.Message;
            }
        }

        public List<EQPRepairInfoDTO> DoExportFunction(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.EQPRepair_Info
                        join equipinfo in DataContext.Equipment_Info
                        on M.EQP_Uid equals equipinfo.EQP_Uid into temp
                        from aa in temp.DefaultIfEmpty()
                        join project in DataContext.System_Project
                        on aa.Project_UID equals project.Project_UID into temp2
                        from bb in temp2.DefaultIfEmpty()
                        join fun in DataContext.System_Function_Plant
                        on aa.System_FunPlant_UID equals fun.System_FunPlant_UID into temp3
                        from cc in temp3.DefaultIfEmpty()
                        where uids.Contains("," + M.Repair_Uid + ",")
                        select new EQPRepairInfoDTO
                        {
                            Repair_Date = M.Error_Time.Year.ToString() + (M.Error_Time.Month.ToString().Length > 1 ? M.Error_Time.Month.ToString() : "0" + M.Error_Time.Month.ToString())
                                        + (M.Error_Time.Day.ToString().Length > 1 ? M.Error_Time.Day.ToString() : "0" + M.Error_Time.Day.ToString()),
                            //Repair_Time = M.Error_Time.Hour.ToString() + ":" + M.Error_Time.Minute.ToString(),
                            Repair_Time = M.Error_Time.Year.ToString() + "-" + (M.Error_Time.Month > 9 ? M.Error_Time.Month.ToString() : "0" + M.Error_Time.Month.ToString()) + "-" + (M.Error_Time.Day > 9 ? M.Error_Time.Day.ToString() : "0" + M.Error_Time.Day.ToString()) + " " + M.Error_Time.Hour.ToString() + ":" + M.Error_Time.Minute.ToString(),
                            EQP_Location = aa.EQP_Location,
                            Class_Desc = aa.Class_Desc,
                            EQP_Plant_No = aa.EQP_Plant_No,
                            Repair_Reason = M.Repair_Reason,
                            Error_Types = M.Error_Types,
                            Reason_Analysis = M.Reason_Analysis,
                            Repair_Method = M.Repair_Method,
                            Repair_BeginTime = M.Repair_BeginTime,
                            Repair_EndTime = M.Repair_EndTime,
                            TotalTime = Math.Round((double)(SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime) == null ? 0 : (int)SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime)) / 60, 2),
                            Labor_List = M.Labor_List,
                            All_RepairCost = M.All_RepairCost,
                            Update_Part = M.Update_Part,
                            Repair_Remark = M.Repair_Remark,
                            Modified_Date = M.Modified_Date,
                            Process = aa.Process,
                            Reason_Types = M.Reason_Types,
                            Repair_Result = M.Repair_Result,
                            OP_TYPES = bb.OP_TYPES,
                            FunPlant = cc.FunPlant,
                            Repair_id = M.Repair_id,
                            Project_Name = bb.Project_Name,
                            Equipment = aa.Equipment,
                            Mfg_Serial_Num = aa.Mfg_Serial_Num,
                            Project_UID = bb.Project_UID,
                            System_FunPlant_UID = aa.System_FunPlant_UID,
                            Apply_Time = M.Apply_Time,
                            Contact = M.Contact,                    
                            Error_Level = M.Error_Level,
                            Status = M.Status,
                            Repair_Uid = M.Repair_Uid,
                            Mentioner = M.Mentioner,
                            CostCtr_ID = M.CostCtr_info.CostCtr_ID,
                            CostCtr_Description=M.CostCtr_info.CostCtr_Description
                        };
            return query.ToList();
        }

        public List<EQPRepairInfoDTO> DoExportFunction2(string optypes, string projectname, string funplant, string process,
                        string eqpid, string errortype, string repairid, string location, string classdesc, string contact,
                        DateTime fromdate, DateTime todate, string errorlever, string repairresult, string labor,
                        string updatepart, string remark, string status)
        {

            var query = from M in DataContext.EQPRepair_Info
                        join equipinfo in DataContext.Equipment_Info
                        on M.EQP_Uid equals equipinfo.EQP_Uid into temp
                        from aa in temp.DefaultIfEmpty()
                        join project in DataContext.System_Project
                        on aa.Project_UID equals project.Project_UID into temp2
                        from bb in temp2.DefaultIfEmpty()
                        join fun in DataContext.System_Function_Plant
                        on aa.System_FunPlant_UID equals fun.System_FunPlant_UID into temp3
                        from cc in temp3.DefaultIfEmpty()
                        select new EQPRepairInfoDTO
                        {
                            Repair_Date = M.Error_Time.Year.ToString() + (M.Error_Time.Month.ToString().Length > 1 ? M.Error_Time.Month.ToString() : "0" + M.Error_Time.Month.ToString())
                                        + (M.Error_Time.Day.ToString().Length > 1 ? M.Error_Time.Day.ToString() : "0" + M.Error_Time.Day.ToString()),
                            Repair_Time = M.Error_Time.Year.ToString() + "-" + (M.Error_Time.Month > 9 ? M.Error_Time.Month.ToString() : "0" + M.Error_Time.Month.ToString()) + "-" + (M.Error_Time.Day > 9 ? M.Error_Time.Day.ToString() : "0" + M.Error_Time.Day.ToString()) + " " + M.Error_Time.Hour.ToString() + ":" + M.Error_Time.Minute.ToString(),
                            EQP_Location = aa.EQP_Location,
                            Class_Desc = aa.Class_Desc,
                            EQP_Plant_No = aa.EQP_Plant_No,
                            Repair_Reason = M.Repair_Reason,
                            Error_Types = M.Error_Types,
                            Reason_Analysis = M.Reason_Analysis,
                            Repair_Method = M.Repair_Method,
                            Repair_BeginTime = M.Repair_BeginTime,
                            Repair_EndTime = M.Repair_EndTime,
                            TotalTime = Math.Round((double)(SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime) == null ? 0 : (int)SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime)) / 60, 2),
                            Labor_List = M.Labor_List,
                            All_RepairCost = M.All_RepairCost == null ? 0 : M.All_RepairCost,
                            Update_Part = M.Update_Part,
                            Repair_Remark = M.Repair_Remark,
                            Modified_Date = M.Modified_Date,
                            Process = aa.Process,
                            Reason_Types = M.Reason_Types,
                            Repair_Result = M.Repair_Result,
                            OP_TYPES = bb.OP_TYPES,
                            FunPlant = cc.FunPlant,
                            Repair_id = M.Repair_id,
                            Project_Name = bb.Project_Name,
                            Equipment = aa.Equipment,
                            Mfg_Serial_Num = aa.Mfg_Serial_Num,
                            Project_UID = bb.Project_UID.ToString() != null && bb.Project_UID.ToString() != "" ? bb.Project_UID : 0,
                            System_FunPlant_UID = aa.System_FunPlant_UID,
                            Contact = M.Contact,
                            Error_Level = M.Error_Level,
                            Status = M.Status,
                            Repair_Uid = M.Repair_Uid,
                            Apply_Time=M.Apply_Time,
                            Mentioner = M.Mentioner
                        };
            if (!string.IsNullOrWhiteSpace(repairid))
                query = query.Where(m => m.Repair_id.Contains(repairid));
            if (!string.IsNullOrWhiteSpace(optypes))
                query = query.Where(m => m.OP_TYPES.Contains(optypes));
            if (!string.IsNullOrWhiteSpace(projectname))
            {
                int projectuid = Convert.ToInt16(projectname);
                query = query.Where(m => m.Project_UID == projectuid);
            }
            if (!string.IsNullOrWhiteSpace(eqpid))
                query = query.Where(m => m.Mfg_Serial_Num.Contains(eqpid));
            if (!string.IsNullOrWhiteSpace(funplant))
            {
                int funplantuid = Convert.ToInt16(funplant);
                query = query.Where(m => m.System_FunPlant_UID == funplantuid);
            }
            if (!string.IsNullOrWhiteSpace(process))
                query = query.Where(m => m.Process.Contains(process));
            if (!string.IsNullOrWhiteSpace(errortype))
                query = query.Where(m => m.Error_Types == errortype);
            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(m => m.EQP_Location == location);
            if (!string.IsNullOrWhiteSpace(classdesc))
                query = query.Where(m => m.Class_Desc.Contains(classdesc));
            if (!string.IsNullOrWhiteSpace(contact))
                query = query.Where(m => m.Contact.Contains(contact));
            if (fromdate.Year != 1)
                query = query.Where(m => m.Repair_EndTime >= fromdate);
            if (todate.Year != 1)
            {
                todate = todate.AddDays(1);
                query = query.Where(m => m.Repair_EndTime < todate);
            }
            if (!string.IsNullOrWhiteSpace(remark))
                query = query.Where(m => m.Repair_Remark.Contains(remark));
            if (!string.IsNullOrWhiteSpace(errorlever))
                query = query.Where(m => m.Error_Level == errorlever);
            if (!string.IsNullOrWhiteSpace(repairresult))
                query = query.Where(m => m.Repair_Result == repairresult);
            if (!string.IsNullOrWhiteSpace(updatepart))
                query = query.Where(m => m.Update_Part.Contains(updatepart));
            if (!string.IsNullOrWhiteSpace(labor))
                query = query.Where(m => m.Labor_List.Contains(labor));
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(m => m.Status == status);
            return query.ToList();
        }


        public List<EQPRepairInfoDTO> DoPartExportFunctionInfo(string uids)
        {
            uids = "," + uids + ",";

            var query = from M in DataContext.EQPRepair_Info
                        join equipinfo in DataContext.Equipment_Info
                        on M.EQP_Uid equals equipinfo.EQP_Uid into temp
                        from aa in temp.DefaultIfEmpty()
                        join project in DataContext.System_Project
                        on aa.Project_UID equals project.Project_UID into temp2
                        from bb in temp2.DefaultIfEmpty()
                        join fun in DataContext.System_Function_Plant
                        on aa.System_FunPlant_UID equals fun.System_FunPlant_UID into temp3
                        from cc in temp3.DefaultIfEmpty()
                        select new EQPRepairInfoDTO
                        {
                            Repair_Date = M.Error_Time.Year.ToString() + (M.Error_Time.Month.ToString().Length > 1 ? M.Error_Time.Month.ToString() : "0" + M.Error_Time.Month.ToString())
                                        + (M.Error_Time.Day.ToString().Length > 1 ? M.Error_Time.Day.ToString() : "0" + M.Error_Time.Day.ToString()),
                            Repair_Time = M.Error_Time.Year.ToString() + "-" + (M.Error_Time.Month > 9 ? M.Error_Time.Month.ToString() : "0" + M.Error_Time.Month.ToString()) + "-" + (M.Error_Time.Day > 9 ? M.Error_Time.Day.ToString() : "0" + M.Error_Time.Day.ToString()) + " " + M.Error_Time.Hour.ToString() + ":" + M.Error_Time.Minute.ToString(),
                            EQP_Location = aa.EQP_Location,
                            Class_Desc = aa.Class_Desc,
                            EQP_Plant_No = aa.EQP_Plant_No,
                            Repair_Reason = M.Repair_Reason,
                            Error_Types = M.Error_Types,
                            Reason_Analysis = M.Reason_Analysis,
                            Repair_Method = M.Repair_Method,
                            Repair_BeginTime = M.Repair_BeginTime,
                            Repair_EndTime = M.Repair_EndTime,
                            TotalTime = Math.Round((double)(SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime) == null ? 0 : (int)SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime)) / 60, 2),
                            Labor_List = M.Labor_List,
                            All_RepairCost = M.All_RepairCost == null ? 0 : M.All_RepairCost,
                            Update_Part = M.Update_Part,
                            Repair_Remark = M.Repair_Remark,
                            Modified_Date = M.Modified_Date,
                            Process = aa.Process,
                            Reason_Types = M.Reason_Types,
                            Repair_Result = M.Repair_Result,
                            OP_TYPES = bb.OP_TYPES,
                            FunPlant = cc.FunPlant,
                            Repair_id = M.Repair_id,
                            Project_Name = bb.Project_Name,
                            Equipment = aa.Equipment,
                            Mfg_Serial_Num = aa.Mfg_Serial_Num,
                            Project_UID = bb.Project_UID.ToString() != null && bb.Project_UID.ToString() != "" ? bb.Project_UID : 0,
                            System_FunPlant_UID = aa.System_FunPlant_UID,
                            Contact = M.Contact,
                            Error_Level = M.Error_Level,
                            Status = M.Status,
                            Repair_Uid = M.Repair_Uid,
                            Apply_Time = M.Apply_Time,
                            Mentioner = M.Mentioner
                        };
            query = query.Where(m => uids.Contains("," + m.Repair_Uid + ","));
            return query.ToList();
        }
        public int GetInt(string inti)
        {
            int m;
            if (int.TryParse(inti, out m))
            {
                return m;
            }
            else
            {
                return m;
            }
        }

        public IQueryable<EQPRepairInfoDTO> GetReportInfo(EQPRepairInfoSearchDTO searchModel, List<int> Organization_UIDs, Page page, out int totalcount, out decimal allcost)
        {
            var query = from M in DataContext.EQPRepair_Info
                        join equipinfo in DataContext.Equipment_Info
                        on M.EQP_Uid equals equipinfo.EQP_Uid into temp
                        from aa in temp.DefaultIfEmpty()
                        join funplant in DataContext.System_Function_Plant
                        on aa.System_FunPlant_UID equals funplant.System_FunPlant_UID into temp2
                        from bb in temp2.DefaultIfEmpty()
                        join project in DataContext.System_Project
                        on aa.Project_UID equals project.Project_UID into temp3
                        from cc in temp3.DefaultIfEmpty()

                        join materialupdateinfo in DataContext.Meterial_UpdateInfo
                 on M.Repair_Uid equals materialupdateinfo.Repair_Uid into temp4
                        from dd in temp4.DefaultIfEmpty()

                        join materialinfo in DataContext.Material_Info
                   on dd.Material_Uid equals materialinfo.Material_Uid into temp5
                        from ee in temp5.DefaultIfEmpty()
                        select new EQPRepairInfoDTO
                        {
                            Class_Desc = aa.Class_Desc,
                            Repair_Uid = M.Repair_Uid,
                            Repair_id = M.Repair_id,
                            Repair_Date = M.Error_Time.Year.ToString() + (M.Error_Time.Month > 9 ? M.Error_Time.Month.ToString() : "0" + M.Error_Time.Month.ToString()) + (M.Error_Time.Day > 9 ? M.Error_Time.Day.ToString() : "0" + M.Error_Time.Day.ToString()),
                            Repair_Time = M.Error_Time.Year.ToString()+"-" + (M.Error_Time.Month > 9 ? M.Error_Time.Month.ToString() : "0" + M.Error_Time.Month.ToString()) +"-"+ (M.Error_Time.Day > 9 ? M.Error_Time.Day.ToString() : "0" + M.Error_Time.Day.ToString())+" "+ M.Error_Time.Hour.ToString() + ":" + M.Error_Time.Minute.ToString(),
                            Repair_Reason = M.Repair_Reason,
                            Error_Types = M.Error_Types,
                            Reason_Analysis = M.Reason_Analysis,
                            Repair_Method = M.Repair_Method,
                            Repair_BeginTime = M.Repair_BeginTime,
                            Repair_EndTime = M.Repair_EndTime,
                            TotalTime = Math.Round((double)(SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime) == null ? 0 : (int)SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime)) / 60, 2),
                            Labor_List = M.Labor_List,
                            All_RepairCost = M.All_RepairCost,
                            Update_Part = M.Update_Part,
                            Repair_Remark = M.Repair_Remark,
                            Modified_Date = M.Modified_Date,
                            Reason_Types = M.Reason_Types,
                            Repair_Result = M.Repair_Result,
                            Contact = M.Contact,
                            EQP_Location = aa.EQP_Location,
                            FunPlant = bb.FunPlant,
                            OP_TYPES = bb.OP_Types,
                            Project_Name = cc.Project_Name,
                            EQP_Plant_No = aa.EQP_Plant_No,
                            Equipment = aa.Equipment,
                            Process = aa.Process,
                            Mfg_Serial_Num = aa.Mfg_Serial_Num,

                            Material_Name = ee.Material_Name,
                            Material_Id= ee.Material_Id,
                            Material_Types= ee.Material_Types,
                            Material_Uid = SqlFunctions.StringConvert((decimal)ee.Material_Uid),
                            Apply_Time = M.Apply_Time,
                            Error_Level = M.Error_Level,
                            Status = M.Status,
                            Contact_tel = M.Contact_tel,
                            Organization_UID = cc.Organization_UID,
                            FunPlant_OrganizationUID = bb.FunPlant_OrganizationUID.Value,
                            Mentioner = M.Mentioner

                        };
            if (searchModel.Organization_UID != 0)
                query = query.Where(m => m.Organization_UID == searchModel.Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Project_Name))
                query = query.Where(m => m.Project_Name.Contains(searchModel.Project_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.FunPlant))
                query = query.Where(m => m.FunPlant.Contains(searchModel.FunPlant));
            if (!string.IsNullOrWhiteSpace(searchModel.Process))
                query = query.Where(m => m.Process.Contains(searchModel.Process));
            if (!string.IsNullOrWhiteSpace(searchModel.Mfg_Serial_Num))
                query = query.Where(m => m.Mfg_Serial_Num.Contains(searchModel.Mfg_Serial_Num));

            //EMT
            if (!string.IsNullOrWhiteSpace(searchModel.Equipment))
                query = query.Where(m => m.Equipment.Contains(searchModel.Equipment));
            //厂内编号
            if (!string.IsNullOrWhiteSpace(searchModel.EQP_Plant_No))
                query = query.Where(m => m.EQP_Plant_No.Contains(searchModel.EQP_Plant_No));
            //配件名称
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                query = query.Where(ee => ee.Material_Name.Contains(searchModel.Material_Name));
            //配件料号
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                query = query.Where(ee => ee.Material_Uid.Contains(searchModel.Material_Id));
            //配件型号
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                query = query.Where(ee => ee.Material_Types.Contains(searchModel.Material_Types));

            if (!string.IsNullOrWhiteSpace(searchModel.Error_Types))
                query = query.Where(m => m.Error_Types.Contains(searchModel.Error_Types));
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_id))
                query = query.Where(m => m.Repair_id.Contains(searchModel.Repair_id));
            if (!string.IsNullOrWhiteSpace(searchModel.EQP_Location))
                query = query.Where(m => m.EQP_Location == searchModel.EQP_Location);
            if (!string.IsNullOrWhiteSpace(searchModel.Class_Desc))
                query = query.Where(m => m.Class_Desc.Contains(searchModel.Class_Desc));
            if (!string.IsNullOrWhiteSpace(searchModel.Contact))
                query = query.Where(m => m.Contact.Contains(searchModel.Contact));
            if (searchModel.End_Date_From.Year != 1)
                query = query.Where(m => m.Repair_EndTime >= searchModel.End_Date_From);
            if (!string.IsNullOrWhiteSpace(searchModel.Error_Level))
                query = query.Where(m => m.Error_Level == searchModel.Error_Level);
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Result))
                query = query.Where(m => m.Repair_Result == searchModel.Repair_Result);
            if (!string.IsNullOrWhiteSpace(searchModel.Update_Part))
                query = query.Where(m => m.Update_Part.Contains(searchModel.Update_Part));
            if (!string.IsNullOrWhiteSpace(searchModel.Labor_List))
                query = query.Where(m => m.Labor_List.Contains(searchModel.Labor_List));
            if (!string.IsNullOrWhiteSpace(searchModel.Status))
                query = query.Where(m => m.Status == searchModel.Status);
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Result))
                query = query.Where(m => m.Repair_Result == searchModel.Repair_Result);
            if (!string.IsNullOrWhiteSpace(searchModel.Mentioner))
                query = query.Where(m => m.Mentioner.Contains(searchModel.Mentioner));
            if (searchModel.End_Date_To.Year != 1)
            {
                searchModel.End_Date_To = searchModel.End_Date_To.AddDays(1);
                query = query.Where(m => m.Repair_EndTime < searchModel.End_Date_To);
            }
            // 只是针对厂区的过滤
            if (Organization_UIDs.Count > 0)
                query = query.Where(m => Organization_UIDs.Contains(m.Organization_UID));

            totalcount = query.Count();
            if (totalcount == 0)
                allcost = 0;
            else
                allcost = (decimal)query.Sum(m => (SqlFunctions.DateDiff("MINUTE", m.Repair_BeginTime, m.Repair_EndTime)) == null ? 0 : (SqlFunctions.DateDiff("MINUTE", m.Repair_BeginTime, m.Repair_EndTime)));
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Contact).GetPage(page);
            return query;
        }

        public List<EQPRepairInfoDTO> DoAllEQPMaintenanceReprot(EQPRepairInfoSearchDTO searchModel, List<int> Organization_UIDs)
        {
            var query = from M in DataContext.EQPRepair_Info
                        join equipinfo in DataContext.Equipment_Info
                        on M.EQP_Uid equals equipinfo.EQP_Uid into temp
                        from aa in temp.DefaultIfEmpty()
                        join funplant in DataContext.System_Function_Plant
                        on aa.System_FunPlant_UID equals funplant.System_FunPlant_UID into temp2
                        from bb in temp2.DefaultIfEmpty()
                        join project in DataContext.System_Project
                        on aa.Project_UID equals project.Project_UID into temp3
                        from cc in temp3.DefaultIfEmpty()
                        select new EQPRepairInfoDTO
                        {
                            Class_Desc = aa.Class_Desc,
                            Repair_Uid = M.Repair_Uid,
                            Repair_id = M.Repair_id,
                            Repair_Date = M.Error_Time.Year.ToString() + (M.Error_Time.Month > 9 ? M.Error_Time.Month.ToString() : "0" + M.Error_Time.Month.ToString()) + (M.Error_Time.Day > 9 ? M.Error_Time.Day.ToString() : "0" + M.Error_Time.Day.ToString()),
                            Repair_Time = M.Error_Time.Year.ToString() + "-" + (M.Error_Time.Month > 9 ? M.Error_Time.Month.ToString() : "0" + M.Error_Time.Month.ToString()) + "-" + (M.Error_Time.Day > 9 ? M.Error_Time.Day.ToString() : "0" + M.Error_Time.Day.ToString()) + " " + M.Error_Time.Hour.ToString() + ":" + M.Error_Time.Minute.ToString(),
                            Repair_Reason = M.Repair_Reason,
                            Error_Types = M.Error_Types,
                            Reason_Analysis = M.Reason_Analysis,
                            Repair_Method = M.Repair_Method,
                            Repair_BeginTime = M.Repair_BeginTime,
                            Repair_EndTime = M.Repair_EndTime,
                            TotalTime = Math.Round((double)(SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime) == null ? 0 : (int)SqlFunctions.DateDiff("MINUTE", M.Repair_BeginTime, M.Repair_EndTime)) / 60, 2),
                            Labor_List = M.Labor_List,
                            All_RepairCost = M.All_RepairCost,
                            Update_Part = M.Update_Part,
                            Repair_Remark = M.Repair_Remark,
                            Modified_Date = M.Modified_Date,
                            Reason_Types = M.Reason_Types,
                            Repair_Result = M.Repair_Result,
                            Contact = M.Contact,
                            EQP_Location = aa.EQP_Location,
                            FunPlant = bb.FunPlant,
                            OP_TYPES = bb.OP_Types,
                            Project_Name = cc.Project_Name,
                            EQP_Plant_No = aa.EQP_Plant_No,
                            Equipment = aa.Equipment,
                            Process = aa.Process,
                            Mfg_Serial_Num = aa.Mfg_Serial_Num,
                            Apply_Time = M.Apply_Time,
                            Error_Level = M.Error_Level,
                            Status = M.Status,
                            Contact_tel = M.Contact_tel,
                            Organization_UID = cc.Organization_UID,
                            FunPlant_OrganizationUID = bb.FunPlant_OrganizationUID.Value,
                            Mentioner=M.Mentioner,
                            CostCtr_UID = M.CostCtr_UID,
                            CostCtr_ID = M.CostCtr_info.CostCtr_ID,
                            CostCtr_Description = M.CostCtr_info.CostCtr_Description
                        };
            if (searchModel.Organization_UID != 0)
                query = query.Where(m => m.Organization_UID == searchModel.Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Project_Name))
                query = query.Where(m => m.Project_Name.Contains(searchModel.Project_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.FunPlant))
                query = query.Where(m => m.FunPlant.Contains(searchModel.FunPlant));
            if (searchModel.CostCtr_UID != 0)
                query = query.Where(m => m.CostCtr_UID == searchModel.CostCtr_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Process))
                query = query.Where(m => m.Process.Contains(searchModel.Process));
            if (!string.IsNullOrWhiteSpace(searchModel.Mfg_Serial_Num))
                query = query.Where(m => m.Mfg_Serial_Num.Contains(searchModel.Mfg_Serial_Num));
            if (!string.IsNullOrWhiteSpace(searchModel.Error_Types))
                query = query.Where(m => m.Error_Types.Contains(searchModel.Error_Types));
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_id))
                query = query.Where(m => m.Repair_id.Contains(searchModel.Repair_id));
            if (!string.IsNullOrWhiteSpace(searchModel.EQP_Location))
                query = query.Where(m => m.EQP_Location == searchModel.EQP_Location);
            if (!string.IsNullOrWhiteSpace(searchModel.Class_Desc))
                query = query.Where(m => m.Class_Desc.Contains(searchModel.Class_Desc));
            if (!string.IsNullOrWhiteSpace(searchModel.Contact))
                query = query.Where(m => m.Contact.Contains(searchModel.Contact));
            if (searchModel.End_Date_From.Year != 1)
                query = query.Where(m => m.Repair_EndTime >= searchModel.End_Date_From);
            if (!string.IsNullOrWhiteSpace(searchModel.Error_Level))
                query = query.Where(m => m.Error_Level == searchModel.Error_Level);
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Result))
                query = query.Where(m => m.Repair_Result == searchModel.Repair_Result);
            if (!string.IsNullOrWhiteSpace(searchModel.Update_Part))
                query = query.Where(m => m.Update_Part.Contains(searchModel.Update_Part));
            if (!string.IsNullOrWhiteSpace(searchModel.Labor_List))
                query = query.Where(m => m.Labor_List.Contains(searchModel.Labor_List));
            if (!string.IsNullOrWhiteSpace(searchModel.Status))
                query = query.Where(m => m.Status == searchModel.Status);
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Result))
                query = query.Where(m => m.Repair_Result == searchModel.Repair_Result);
            if (searchModel.End_Date_To.Year != 1)
            {
                searchModel.End_Date_To = searchModel.End_Date_To.AddDays(1);
                query = query.Where(m => m.Repair_EndTime < searchModel.End_Date_To);
            }
            // 只是针对厂区的过滤
            if (Organization_UIDs.Count > 0)
                query = query.Where(m => Organization_UIDs.Contains(m.Organization_UID));

            return query.ToList();
        }
        #region 电子看板

        public IQueryable<EQPRepairInfoDTO> GetEQPBoardInfo(EQPRepairInfoDTO searchModel, Page page, out int totalcount)
        {

            var query = from M in DataContext.EQPRepair_Info
                        join equipinfo in DataContext.Equipment_Info
                        on M.EQP_Uid equals equipinfo.EQP_Uid into temp
                        from aa in temp.DefaultIfEmpty()
                        join funplant in DataContext.System_Function_Plant
                        on aa.System_FunPlant_UID equals funplant.System_FunPlant_UID into temp2
                        from bb in temp2.DefaultIfEmpty()
                        join project in DataContext.System_Project
                        on aa.Project_UID equals project.Project_UID into temp3
                        from cc in temp3.DefaultIfEmpty()
                        where searchModel.EQP_Location.Contains(aa.EQP_Location)
                        select new EQPRepairInfoDTO
                        {
                            Class_Desc = aa.Class_Desc,
                            Repair_Uid = M.Repair_Uid,
                            Repair_id = M.Repair_id,
                            Repair_Reason = M.Repair_Reason,
                            Modified_Date = M.Modified_Date,
                            Repair_Result = M.Repair_Result,
                            Contact = M.Contact,
                            Contact_tel = M.Contact_tel,
                            Status = M.Status,
                            EQP_Location = aa.EQP_Location,
                            FunPlant = bb.FunPlant,
                            Project_Name = cc.Project_Name,
                            Process = aa.Process,
                            Mfg_Serial_Num = aa.Mfg_Serial_Num
                        };
            query = query.Where(m => m.Repair_Result != "已完成");
            //if (!string.IsNullOrWhiteSpace(searchModel.EQP_Location))
            //    query = query.Where(m => m.EQP_Location.Contains(searchModel.EQP_Location));
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Contact).GetPage(page);
            return query;
        }
        #endregion

        public List<EQPRepairInfoDTO> GetSingleEQPRepair(string Repair_id)
        {
            string sql = @"SELECT TOP 1 t1.*,t2.EQP_Location,t2.Equipment,t3.OP_Types as OP_TYPES,t3.FunPlant,t4.EQPUser_Uid 
                           FROM EQPRepair_Info t1 
                           inner join Equipment_Info t2 on t1.EQP_Uid=t2.EQP_Uid 
                           left join System_Function_Plant t3 on t2.System_FunPlant_UID=t3.System_FunPlant_UID 
                           left join Labor_UsingInfo t4 on t1.Repair_Uid=t4.Repair_Uid
                           where Repair_id='{0}'
                           order by t4.Labor_Using_Uid;";
            sql = string.Format(sql, Repair_id);
            var dblist = DataContext.Database.SqlQuery<EQPRepairInfoDTO>(sql).ToList();
            return dblist;
        }

        public List<int> GetF3MDamageQty(int Material_Uid, string EQP_Type)
        {
            string sql = @"select t3.Update_No from EQPRepair_Info t1 inner join Equipment_Info t2
                            on t1.EQP_Uid=t2.EQP_Uid inner join Meterial_UpdateInfo t3
                            on t1.Repair_Uid=t3.Repair_Uid where t2.Mfg_Of_Asset=N'{0}' and t3.Material_Uid={1}";
            sql = string.Format(sql, EQP_Type, Material_Uid);
            var dblist = DataContext.Database.SqlQuery<int>(sql).ToList();
            return dblist;
        }

        //判斷成本中心是否被引用
        public bool CheckCostCtrIsExist(int CostCtr_Uid)
        {
            bool isExist = (DataContext.EQPRepair_Info.Where(e => e.CostCtr_UID == CostCtr_Uid).Count() > 0);
            return isExist;
        }
    }
}
