using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using System.Data.Entity.SqlServer;
namespace PDMS.Data.Repository
{
    public interface IMaterialRepairDemandRepository : IRepository<Material_Repair_Demand>
    {
        IQueryable<MaterialRepairDemandDTO> GetInfo(MaterialRepairDemandDTO searchModel, Page page, out int totalcount);
        IQueryable<MaterialRepairDemandDTO> GetDetailInfo(MaterialRepairDemandDTO searchModel, Page page, out int totalcount);
        List<MaterialRepairDemandDTO> DoExportFunction(Material_Repair_Demand MND);
    }

     public class MaterialRepairDemandRepository: RepositoryBase<Material_Repair_Demand>, IMaterialRepairDemandRepository
    {
        public MaterialRepairDemandRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IQueryable<MaterialRepairDemandDTO> GetInfo(MaterialRepairDemandDTO searchModel, Page page, out int totalcount)
        {
            var query1 = from M in DataContext.Material_Repair_Demand
                             //fky2017/11/13
                         //where M.Status_UID != 414
                         where M.Status_UID != 439
                         group M by new
                         {
                             M.BG_Organization_UID,
                             M.FunPlant_Organization_UID,
                             M.Demand_Date
                         } into g
                         select new
                         {
                             MaxUid = g.Max(m => m.Material_Repair_Demand_UID)
                         };
            if (searchModel.Status_UID != 0)
            {
                query1 = from M in DataContext.Material_Repair_Demand
                         where M.Status_UID == searchModel.Status_UID
                         group M by new
                         {
                             M.BG_Organization_UID,
                             M.FunPlant_Organization_UID,
                             M.Demand_Date
                         } into g
                         select new
                         {
                             MaxUid = g.Max(m => m.Material_Repair_Demand_UID)
                         };
            }


            var query = from M in DataContext.Material_Repair_Demand
                        join plantbom in DataContext.System_OrganizationBOM on
                        M.BG_Organization_UID equals plantbom.ChildOrg_UID
                        join plant in DataContext.System_Organization on
                        plantbom.ParentOrg_UID equals plant.Organization_UID
                        join status in DataContext.Enumeration on
                        M.Status_UID equals status.Enum_UID
                        join appuser in DataContext.System_Users on
                        M.Applicant_UID equals appuser.Account_UID
                        join apruser in DataContext.System_Users on
                        M.Approver_UID equals apruser.Account_UID
                        join bgorg in DataContext.System_Organization
                        on M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on M.FunPlant_Organization_UID equals funplantorg.Organization_UID
                        join temp in query1 on M.Material_Repair_Demand_UID equals temp.MaxUid
                        select new MaterialRepairDemandDTO
                        {
                            Material_Repair_Demand_UID = M.Material_Repair_Demand_UID,
                            Plant = plant.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            FunPlant = funplantorg.Organization_Name,
                            Status = status.Enum_Value,
                            Calculation_Date = M.Calculation_Date,
                            Demand_Date = M.Demand_Date,
                            ApplicantUser = appuser.User_Name,
                            Applicant_Date = M.Applicant_Date,
                            ApproverUser = apruser.User_Name,
                            Approver_Date = M.Approver_Date,
                            Status_UID = M.Status_UID,
                            BG_Organization_UID = M.BG_Organization_UID
                        };
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.Calculation_Date.Year != 1)
            {
                DateTime nextday = searchModel.Calculation_Date.AddDays(1);
                query = query.Where(m => m.Calculation_Date >= searchModel.Calculation_Date & m.Calculation_Date < nextday);
            }
            if (searchModel.Demand_Date.Year != 1)
                query = query.Where(m => m.Demand_Date == searchModel.Demand_Date);
            if (!string.IsNullOrWhiteSpace(searchModel.ApplicantUser))
                query = query.Where(m => m.ApplicantUser.Contains(searchModel.ApplicantUser));
            if (!string.IsNullOrWhiteSpace(searchModel.ApproverUser))
                query = query.Where(m => m.ApproverUser.Contains(searchModel.ApproverUser));
            if (searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);

            List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
            }

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Applicant_Date).GetPage(page);
            return query;
        }

        public IQueryable<MaterialRepairDemandDTO> GetDetailInfo(MaterialRepairDemandDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Material_Repair_Demand
                        join plantbom in DataContext.System_OrganizationBOM
                        on M.BG_Organization_UID equals plantbom.ChildOrg_UID
                        join plantorg in DataContext.System_Organization
                        on plantbom.ParentOrg_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on M.FunPlant_Organization_UID equals funplantorg.Organization_UID
                        join statusenum in DataContext.Enumeration
                        on M.Status_UID equals statusenum.Enum_UID
                        join mat in DataContext.Material_Info
                        on M.Material_UID equals mat.Material_Uid
                        join eqptype in DataContext.EQP_Type
                        on M.EQP_Type_UID equals eqptype.EQP_Type_UID
                        join appuser in DataContext.System_Users
                        on M.Applicant_UID equals appuser.Account_UID
                        join apruser in DataContext.System_Users
                        on M.Approver_UID equals apruser.Account_UID
                        select new MaterialRepairDemandDTO
                        {
                            Material_Repair_Demand_UID = M.Material_Repair_Demand_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = funplantorg.Organization_Name,
                            Status = statusenum.Enum_Value,
                            EQP_Type = eqptype.EQP_Type1,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Forecast_PowerOn_Qty = M.Forecast_PowerOn_Qty,
                            F3M_Damage_Qty=M.F3M_Damage_Qty,
                            F3M_PowerOn_Qty=M.F3M_PowerOn_Qty,
                            Calculated_Demand_Qty = M.Calculated_Demand_Qty,
                            User_Adjustments_Qty = M.User_Adjustments_Qty,
                            Actual_Qty = M.Calculated_Demand_Qty + M.User_Adjustments_Qty,
                            Calculation_Date = M.Calculation_Date,
                            Demand_Date = M.Demand_Date,
                            ApplicantUser = appuser.User_Name,
                            Applicant_Date = M.Applicant_Date,
                            ApproverUser = apruser.User_Name,
                            Approver_Date = M.Approver_Date,
                            BG_Organization_UID = M.BG_Organization_UID,
                            Status_UID = M.Status_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID
                        };
            query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID &m.Demand_Date == searchModel.Demand_Date 
                                                    & m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID&m.Status_UID==searchModel.Status_UID);
            List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
            if (Plant_UIDs.Count > 0)
            {
                query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
            }

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Applicant_Date).GetPage(page);
            return query;
        }
        public List<SystemOrgDTO> GetOpType(int plantorguid)
        {
            var sqlStr = @"SELECT * FROM dbo.System_Organization t1 INNER JOIN dbo.System_OrganizationBOM t2
                                        ON t1.Organization_UID=t2.ChildOrg_UID WHERE  t1.Organization_ID LIKE'%2000%' ";
            if (plantorguid != 0)
            {
                sqlStr = @"SELECT * FROM dbo.System_Organization t1 INNER JOIN dbo.System_OrganizationBOM t2
                                        ON t1.Organization_UID=t2.ChildOrg_UID WHERE  t1.Organization_ID LIKE'%2000%'  and  t2.ParentOrg_UID={0}";
                sqlStr = string.Format(sqlStr, plantorguid);
            }

            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList();
            return dbList;
        }
        public List<MaterialRepairDemandDTO> DoExportFunction(Material_Repair_Demand MND)
        {
            var query = from M in DataContext.Material_Repair_Demand
                        join plantbom in DataContext.System_OrganizationBOM
                        on M.BG_Organization_UID equals plantbom.ChildOrg_UID
                        join plantorg in DataContext.System_Organization
                        on plantbom.ParentOrg_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on M.FunPlant_Organization_UID equals funplantorg.Organization_UID
                        join statusenum in DataContext.Enumeration
                        on M.Status_UID equals statusenum.Enum_UID
                        join mat in DataContext.Material_Info
                        on M.Material_UID equals mat.Material_Uid
                        join eqptype in DataContext.EQP_Type
                        on M.EQP_Type_UID equals eqptype.EQP_Type_UID
                        join appuser in DataContext.System_Users
                        on M.Applicant_UID equals appuser.Account_UID
                        join apruser in DataContext.System_Users
                        on M.Approver_UID equals apruser.Account_UID
                        select new MaterialRepairDemandDTO
                        {
                            Material_Repair_Demand_UID = M.Material_Repair_Demand_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            FunPlant = funplantorg.Organization_Name,
                            Status = statusenum.Enum_Value,
                            EQP_Type = eqptype.EQP_Type1,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Forecast_PowerOn_Qty = M.Forecast_PowerOn_Qty,
                            Calculated_Demand_Qty = M.Calculated_Demand_Qty,
                            User_Adjustments_Qty = M.User_Adjustments_Qty,
                            Actual_Qty = M.Calculated_Demand_Qty + M.User_Adjustments_Qty,
                            Calculation_Date = M.Calculation_Date,
                            Demand_Date = M.Demand_Date,
                            ApplicantUser = appuser.User_Name,
                            Applicant_Date = M.Applicant_Date,
                            ApproverUser = apruser.User_Name,
                            Approver_Date = M.Approver_Date,
                            BG_Organization_UID = M.BG_Organization_UID,
                            Status_UID = M.Status_UID
                        };
            query = query.Where(m => m.BG_Organization_UID == MND.BG_Organization_UID & m.Demand_Date == MND.Demand_Date
                                                        & m.FunPlant_Organization_UID == MND.FunPlant_Organization_UID&m.Status_UID==MND.Status_UID);
            query = query.OrderByDescending(m => m.Applicant_Date);
            return query.ToList();
        }
    }
}
