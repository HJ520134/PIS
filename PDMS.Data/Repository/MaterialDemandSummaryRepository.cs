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
    public interface IMaterialDemandSummaryRepository : IRepository<Material_Demand_Summary>
    {
        IQueryable<MaterialDemandSummaryDTO> GetInfo(MaterialDemandSummaryDTO searchModel, Page page, out int totalcount);
        IQueryable<MaterialDemandSummaryDTO> GetDetailInfo(MaterialDemandSummaryDTO searchModel, Page page, out int totalcount);
        List<MaterialDemandSummaryDTO> DoExportFunction(Material_Demand_Summary MDS);
    }
    public  class MaterialDemandSummaryRepository:RepositoryBase<Material_Demand_Summary>, IMaterialDemandSummaryRepository
    {
        public MaterialDemandSummaryRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IQueryable<MaterialDemandSummaryDTO> GetInfo(MaterialDemandSummaryDTO searchModel, Page page, out int totalcount)
        {
            var query1 = from M in DataContext.Material_Demand_Summary
                             //fky2017/11/13
                         //where M.Status_UID != 414
                         where M.Status_UID != 439
                         group M by new
                         {
                             M.BG_Organization_UID,
                             M.Demand_Date,
                             M.Status_UID
                         } into g
                         select new
                         {
                             MaxUid = g.Max(m => m.Material_Demand_Summary_UID)
                         };
            if (searchModel.Status_UID != 0)
            {
                query1 = from M in DataContext.Material_Demand_Summary
                         where M.Status_UID == searchModel.Status_UID
                         group M by new
                         {
                             M.BG_Organization_UID,
                             M.Demand_Date
                         } into g
                         select new
                         {
                             MaxUid = g.Max(m => m.Material_Demand_Summary_UID)
                         };
            }


            var query = from M in DataContext.Material_Demand_Summary
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
                        join temp in query1 on M.Material_Demand_Summary_UID equals temp.MaxUid
                        select new MaterialDemandSummaryDTO
                        {
                            Material_Demand_Summary_UID = M.Material_Demand_Summary_UID,
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
        public IQueryable<MaterialDemandSummaryDTO> GetDetailInfo(MaterialDemandSummaryDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Material_Demand_Summary
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
                        join appuser in DataContext.System_Users
                        on M.Applicant_UID equals appuser.Account_UID
                        join apruser in DataContext.System_Users
                        on M.Approver_UID equals apruser.Account_UID
                        select new MaterialDemandSummaryDTO
                        {
                            Material_Demand_Summary_UID = M.Material_Demand_Summary_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = funplantorg.Organization_Name,
                            Status = statusenum.Enum_Value,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            NormalDemand_Qty = M.NormalDemand_Qty,
                            SparepartsDemand_Qty=M.SparepartsDemand_Qty,
                            Repair_Demand_Qty=M.Repair_Demand_Qty,
                            Be_Purchase_Qty = M.Be_Purchase_Qty,
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
            query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID &
                                                    m.Demand_Date == searchModel.Demand_Date&m.Status_UID==searchModel.Status_UID);
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Applicant_Date).GetPage(page);
            return query;
        }

        public List<MaterialDemandSummaryDTO> DoExportFunction(Material_Demand_Summary MDS)
        {
            var query = from M in DataContext.Material_Demand_Summary
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
                        join appuser in DataContext.System_Users
                        on M.Applicant_UID equals appuser.Account_UID
                        join apruser in DataContext.System_Users
                        on M.Approver_UID equals apruser.Account_UID
                        select new MaterialDemandSummaryDTO
                        {
                            Material_Demand_Summary_UID = M.Material_Demand_Summary_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = funplantorg.Organization_Name,
                            Status = statusenum.Enum_Value,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            NormalDemand_Qty=M.NormalDemand_Qty,
                            SparepartsDemand_Qty=M.SparepartsDemand_Qty,
                            Repair_Demand_Qty=M.Repair_Demand_Qty,
                            Be_Purchase_Qty = M.Be_Purchase_Qty,
                            Calculation_Date = M.Calculation_Date,
                            Demand_Date = M.Demand_Date,
                            ApplicantUser = appuser.User_Name,
                            Applicant_Date = M.Applicant_Date,
                            ApproverUser = apruser.User_Name,
                            Approver_Date = M.Approver_Date,
                            BG_Organization_UID = M.BG_Organization_UID,
                            Status_UID = M.Status_UID
                        };
            query = query.Where(m => m.BG_Organization_UID == MDS.BG_Organization_UID & m.Demand_Date == MDS.Demand_Date&m.Status_UID==MDS.Status_UID);
            query = query.OrderByDescending(m => m.Applicant_Date);
            return query.ToList();
        }
    }
}
