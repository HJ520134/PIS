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
    public interface IEQPForecastPowerOnRepository : IRepository<EQP_Forecast_PowerOn>
    {
        IQueryable<EQPForecastPowerOnDTO> QueryEQPForecastPowerOns(EQPForecastPowerOnDTO searchModel, Page page, out int totalcount);
        EQPForecastPowerOnDTO QueryEQPForecastPowerOnByPowerId(int id);
    }
    public class EQPForecastPowerOnRepository: RepositoryBase<EQP_Forecast_PowerOn>, IEQPForecastPowerOnRepository
    {
        public EQPForecastPowerOnRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IQueryable<EQPForecastPowerOnDTO> QueryEQPForecastPowerOns(EQPForecastPowerOnDTO searchModel, Page page, out int totalcount)
        {
            var query = from c in DataContext.EQP_Forecast_PowerOn
                        join modifiedUser in DataContext.System_Users on c.Modified_UID equals modifiedUser.Account_UID
                        join eqp in DataContext.EQP_Type on c.EQP_Type_UID equals eqp.EQP_Type_UID
                        join org in DataContext.System_Organization on eqp.BG_Organization_UID equals org.Organization_UID
                        join org1 in DataContext.System_Organization on eqp.FunPlant_Organization_UID equals org1.Organization_UID
                        join bom in DataContext.System_OrganizationBOM on eqp.BG_Organization_UID equals bom.ChildOrg_UID
                        join org2 in DataContext.System_Organization on bom.ParentOrg_UID equals org2.Organization_UID
                        select new EQPForecastPowerOnDTO
                        {
                            PowerOn_Qty = c.PowerOn_Qty,
                            EQP_Forecast_PowerOn_UID = c.EQP_Forecast_PowerOn_UID,
                            EQP_Type_UID = c.EQP_Type_UID,
                            Demand_Date = c.Demand_Date,
                            PowerOn_DateForShow = c.Demand_Date,
                            Modified_UserName = modifiedUser.User_Name,
                            Modified_UserNTID = modifiedUser.User_NTID,
                            Modified_Date = c.Modified_Date,
                            Modified_UID = c.Modified_UID,
                            EQP_Type1 = eqp.EQP_Type1,
                            OPType = org.Organization_Name,
                            Funplant = org1.Organization_Name + "---" + org.Organization_Desc,
                            Plant = org2.Organization_Name,
                            FunPlantUID = eqp.FunPlant_Organization_UID,
                            BG_Organization_UID = eqp.BG_Organization_UID,
                        };
            if (string.IsNullOrEmpty(searchModel.ExportUIds))
            {
                if (searchModel.EQP_Forecast_PowerOn_UID > 0)
                {
                    query = query.Where(p => p.EQP_Forecast_PowerOn_UID == searchModel.EQP_Forecast_PowerOn_UID);
                }
                if (searchModel.PowerOn_Qty > 0)
                {
                    query = query.Where(p => p.PowerOn_Qty == searchModel.PowerOn_Qty);
                }
                if (!string.IsNullOrEmpty(searchModel.Plant))
                {
                    query = query.Where(p => p.Plant.IndexOf(searchModel.Plant) >= 0);
                }
                if (searchModel.BG_Organization_UID > 0)
                {
                    query = query.Where(p => p.BG_Organization_UID == searchModel.BG_Organization_UID);
                }
                if (searchModel.FunPlantUID > 0)
                {
                    query = query.Where(p => p.FunPlantUID == searchModel.FunPlantUID);
                }
                if (searchModel.EQP_Type_UID > 0)
                {
                    query = query.Where(p => p.EQP_Type_UID == searchModel.EQP_Type_UID);
                }
                if (searchModel.Demand_Date != null)
                {
                    query = query.Where(p => p.Demand_Date >= searchModel.Demand_Date);
                }
                if (searchModel.Modified_Date != null)
                {
                    query = query.Where(p => p.Modified_Date >= searchModel.Modified_Date);
                }
                if (!string.IsNullOrWhiteSpace(searchModel.Modified_UserNTID))
                {
                    query = query.Where(p => p.Modified_UserNTID == searchModel.Modified_UserNTID);
                }
                List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
                if (Plant_UIDs.Count > 0)
                {
                    query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
                }
                totalcount = query.Count();

                return query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(searchModel.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.EQP_Forecast_PowerOn_UID));
                List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
                if (Plant_UIDs.Count > 0)
                {
                    query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
                }
                query = query.Distinct();
                totalcount = 0;
                return query.OrderByDescending(o => o.Modified_Date);
            }
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

        public EQPForecastPowerOnDTO QueryEQPForecastPowerOnByPowerId(int id)
        {
            var query = from c in DataContext.EQP_Forecast_PowerOn
                        join modifiedUser in DataContext.System_Users on c.Modified_UID equals modifiedUser.Account_UID
                        join eqp in DataContext.EQP_Type on c.EQP_Type_UID equals eqp.EQP_Type_UID
                        join org in DataContext.System_Organization on eqp.BG_Organization_UID equals org.Organization_UID
                        join org1 in DataContext.System_Organization on eqp.FunPlant_Organization_UID equals org1.Organization_UID
                        join bom in DataContext.System_OrganizationBOM on eqp.BG_Organization_UID equals bom.ChildOrg_UID
                        join org2 in DataContext.System_Organization on bom.ParentOrg_UID equals org2.Organization_UID
                        where c.EQP_Forecast_PowerOn_UID == id
                        select new EQPForecastPowerOnDTO
                        {
                            PowerOn_Qty = c.PowerOn_Qty,
                            EQP_Forecast_PowerOn_UID = c.EQP_Forecast_PowerOn_UID,
                            EQP_Type_UID = c.EQP_Type_UID,
                            Demand_Date = c.Demand_Date,
                            PowerOn_DateForShow = c.Demand_Date,
                            Modified_UserName = modifiedUser.User_Name,
                            Modified_UserNTID = modifiedUser.User_NTID,
                            Modified_Date = c.Modified_Date,
                            Modified_UID = c.Modified_UID,
                            EQP_Type1 = eqp.EQP_Type1,
                            OPType = org.Organization_Name,
                            Funplant = org1.Organization_Name + "---" + org.Organization_Desc,
                            Plant = org2.Organization_Name,
                            FunPlantUID = eqp.FunPlant_Organization_UID,
                            BG_Organization_UID = eqp.BG_Organization_UID,
                        };
            return query.FirstOrDefault();
        }
    }
}
