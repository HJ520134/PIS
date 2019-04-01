using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IMachine_StationRepository : IRepository<Machine_Station>
    {
        Machine_Station_CustomerDTO GetMachine_Station_Customer(int FlowChart_Master_UID, int FlowChart_Detail_UID);
    }
    public class Machine_StationRepository : RepositoryBase<Machine_Station>, IMachine_StationRepository
    {
        public Machine_StationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
        public Machine_Station_CustomerDTO GetMachine_Station_Customer(int FlowChart_Master_UID, int FlowChart_Detail_UID)
        {
            var srtSql = @"SELECT
	                           A.[Machine_Station_UID] AS [Machine_Station_UID]
                                              ,A.[Machine_Customer_UID] AS [Machine_Customer_UID]
                                              ,A.[MES_Station_Name] AS [MES_Station_Name]
                                              ,A.[PIS_Station_Name] AS [PIS_Station_Name]
	                                          ,A.[MES_Customer_Name] AS [MES_Customer_Name]
                                              ,A.[PIS_Customer_Name] AS [PIS_Customer_Name]
	                                          ,fld.FlowChart_Detail_UID AS FlowChart_Detail_UID
                                              ,fld.FlowChart_Master_UID AS FlowChart_Master_UID
	                                          ,fld.FlowChart_Version AS FlowChart_Version
                FROM
	                (
		                SELECT
			                ms.[Machine_Station_UID] AS [Machine_Station_UID],
			                ms.[Machine_Customer_UID] AS [Machine_Customer_UID],
			                ms.[MES_Station_Name] AS [MES_Station_Name],
			                ms.[PIS_Station_Name] AS [PIS_Station_Name],
			                mc.[MES_Customer_Name] AS [MES_Customer_Name],
			                mc.[PIS_Customer_Name] AS [PIS_Customer_Name]
		                FROM
			                dbo.Machine_Station AS ms
		                LEFT JOIN dbo.Machine_Customer AS mc ON ms.Machine_Customer_UID = mc.Machine_Customer_UID
		                WHERE
			                mc.DataSourceType = 'BadDetail' AND mc.Is_Enable=1 AND ms.Is_Enable=1
	                ) AS A LEFT JOIN dbo.FlowChart_Detail AS fld ON A.[PIS_Customer_Name]=fld.FlowChart_Master_UID AND A.[PIS_Station_Name]=fld.Binding_Seq
                 ";
            var modelList = DataContext.Database.SqlQuery<Machine_Station_CustomerDTO>(srtSql).ToList();
            modelList = modelList.Where(p => p.FlowChart_Detail_UID == FlowChart_Detail_UID && p.FlowChart_Master_UID == FlowChart_Master_UID).ToList();

            if (modelList.Count() > 0)
            {
                var model = modelList.OrderByDescending(p => p.FlowChart_Version).FirstOrDefault();
                return model;
            }
            else
            {
                return new Machine_Station_CustomerDTO();
            }
        }
    }
}
