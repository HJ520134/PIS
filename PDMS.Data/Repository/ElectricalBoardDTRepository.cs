using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels.Settings;
using System.Data.Entity;
using System.Collections.Generic;
using PDMS.Model.EntityDTO;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace PDMS.Data.Repository
{
    public class ElectricalBoardDTRepository : RepositoryBase<Electrical_Board_DT>, IElectricalBoardDTRepository
    {
        private string connStr = "Server=CNWXIG0LSQLV01A;DataBase=MESLDB;uid=PISuser;pwd=PISuser123";
        public ElectricalBoardDTRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        /// <summary>
        /// 删除对应的专案的数据
        /// </summary>
        /// <param name="FlowChart_Master_UID"></param>
        /// <returns></returns>
        public bool DeleteElectrical_Board_DT(int FlowChart_Master_UID)
        {
            var deleteSql = $"DELETE FROM [dbo].[Electrical_Board_DT] WHERE [FlowChart_Master_UID]={FlowChart_Master_UID}";
            var resultCount = DataContext.Database.ExecuteSqlCommand(deleteSql.ToString());
            if (resultCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddElectrical_Board_DT(DataTable dt)
        {
            SqlParameter Parameter = new SqlParameter("@ElectricalBoardDt", dt);
            Parameter.TypeName = "[dbo].[ElectricalBoardDt]";
            var result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreCommand("usp_Add_ElectricalBoardDtByType @ElectricalBoardDt",
            Parameter);
            return false;
        }

        public IList<TwoHourCapacityDTO> GetTwoHourCapacity()
        {
            var modelList = new List<TwoHourCapacityDTO>();
            var sqlStr = string.Format(@"select * from GoldenLineDashBoard where Line != 'Total'");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sqlStr.ToString(), conn))
                {
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                var model = new TwoHourCapacityDTO();
                                //获取字段信息
                                model.Line = (read["Line"]).ToString();
                                model.Category = (read["Category"]).ToString();
                                model.Total = (read["Total"]).ToString();
                                model.HourFrom06 = (read["06:00-08:00"]).ToString();
                                model.HourFrom08 = (read["08:00-10:00"]).ToString();
                                model.HourFrom10 = (read["10:00-12:00"]).ToString();
                                model.HourFrom12 = (read["12:00-14:00"]).ToString();
                                model.HourFrom14 = (read["14:00-16:00"]).ToString();
                                model.HourFrom16 = (read["16:00-18:00"]).ToString();
                                model.HourFrom18 = (read["18:00-20:00"]).ToString();
                                model.HourFrom20 = (read["20:00-22:00"]).ToString();
                                model.HourFrom22 = (read["22:00-00:00"]).ToString();
                                model.HourFrom00 = (read["00:00-02:00"]).ToString();
                                model.HourFrom02 = (read["02:00-04:00"]).ToString();
                                model.HourFrom04 = (read["04:00-06:00"]).ToString();
                                modelList.Add(model);
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
            return modelList;
        }
        public IList<TwoHourCapacityDTO> GetTwoHourTotalCapacity()
        {
            var modelList = new List<TwoHourCapacityDTO>();
            var sqlStr = string.Format(@"select * from GoldenLineDashBoard where Line = 'Total'");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sqlStr.ToString(), conn))
                {
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                var model = new TwoHourCapacityDTO();
                                //获取字段信息
                                model.Line = (read["Line"]).ToString();
                                model.Category = (read["Category"]).ToString();
                                model.Total = (read["Total"]).ToString();
                                model.HourFrom06 = (read["06:00-08:00"]).ToString();
                                model.HourFrom08 = (read["08:00-10:00"]).ToString();
                                model.HourFrom10 = (read["10:00-12:00"]).ToString();
                                model.HourFrom12 = (read["12:00-14:00"]).ToString();
                                model.HourFrom14 = (read["14:00-16:00"]).ToString();
                                model.HourFrom16 = (read["16:00-18:00"]).ToString();
                                model.HourFrom18 = (read["18:00-20:00"]).ToString();
                                model.HourFrom20 = (read["20:00-22:00"]).ToString();
                                model.HourFrom22 = (read["22:00-00:00"]).ToString();
                                model.HourFrom00 = (read["00:00-02:00"]).ToString();
                                model.HourFrom02 = (read["02:00-04:00"]).ToString();
                                model.HourFrom04 = (read["04:00-06:00"]).ToString();
                                modelList.Add(model);
                            }
                        }
                    }
                    catch (Exception)
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
            return modelList;
        }
    }

    public interface IElectricalBoardDTRepository : IRepository<Electrical_Board_DT>
    {
        IList<TwoHourCapacityDTO> GetTwoHourCapacity();
        IList<TwoHourCapacityDTO> GetTwoHourTotalCapacity();
        bool DeleteElectrical_Board_DT(int FlowChart_Master_UID);

        bool AddElectrical_Board_DT(DataTable dt);
    }
}
