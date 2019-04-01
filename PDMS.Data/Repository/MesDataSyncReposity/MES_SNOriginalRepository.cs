using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using PDMS.Common;

namespace PDMS.Data.Repository
{
    public interface IMES_SNOriginalRepository : IRepository<MES_SNOriginal>
    {
        List<MES_SNOriginalDTO> GetMES_SNOriginal(DataTable dt);

        //添加没有重复的数据
        void AddMES_SNOriginal(DataTable dt);
        void AddShipMent(DataTable dt);

        void DeleteShipMent();
        int GetShipMentCount();


        //删除已经出货SN
        void DeleteMES_SNOriginal(DataTable dt);
        void DeleteMES_SNOriginal(string str);
        List<Mes_StationColorDTO> GetStationColorListByName(string project_Name);
        List<MES_SNOriginalDTO> GetALLData();
        List<MES_SNOriginal> GetTopDate(int top);

        /// <summary>
        /// 获取工站颜色
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        List<MES_SNOriginalDTO> GetStationColor(DataTable dt, string stationList);
    }

    public class MES_SNOriginalRepository : RepositoryBase<MES_SNOriginal>, IMES_SNOriginalRepository
    {
        public MES_SNOriginalRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        { }



        public List<MES_SNOriginalDTO> GetALLData()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("SeriesNumber", typeof(string));
            dataTable.Columns.Add("CustomerName", typeof(string));
            dataTable.Columns.Add("MES_ProcessID", typeof(int));
            dataTable.Columns.Add("StationName", typeof(string));
            dataTable.Columns.Add("Starttime", typeof(DateTime));
            var query = from SNOriginal in DataContext.MES_SNOriginal
                        where SNOriginal.MES_SNOriginal_UID < 100000
                        select SNOriginal;
            //select  new MES_SNOriginalDTO
            //{
            //    MES_SNOriginal_UID= SNOriginal.MES_SNOriginal_UID,
            //    SeriesNumber = SNOriginal.SeriesNumber,
            //    CustomerName = SNOriginal.CustomerName,
            //    StationName = SNOriginal.StationName,
            //    Starttime = SNOriginal.Starttime
            //};

            var tempList = query.ToList();   //  不能将数据库的全部数据转换为实体类再获取前多少条（那怕用take1），这样会直接把内存沾满。

            foreach (var item in tempList)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["SeriesNumber"] = item.SeriesNumber.Substring(0, 2) + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                dataRow["CustomerName"] = item.CustomerName;
                dataRow["Starttime"] = item.Starttime;
                dataRow["MES_ProcessID"] = 1;
                dataRow["StationName"] = item.StationName;
                dataTable.Rows.Add(dataRow);
            }
            AddMES_SNOriginal(dataTable);

            return new List<MES_SNOriginalDTO>();
        }


        public List<MES_SNOriginal> GetTopDate(int top)
        {
            var sql = @"SELECT TOP {0} [MES_SNOriginal_UID]
                                          ,[SeriesNumber]
                                          ,[CustomerName]
                                          ,[StationName]
                                          ,[Starttime]
                                          ,Color
                                          FROM [PDMS_Dev].[dbo].[MES_SNOriginal] where Color='0'  ";
            sql = string.Format(sql, top);
            string connectionString = ConfigurationManager.ConnectionStrings["SyncGoldenLineCT"].ConnectionString;
            List<MES_SNOriginal> List = new List<MES_SNOriginal>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Invoke RegionUpdate Procedure
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 0;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MES_SNOriginal model = new MES_SNOriginal();
                    model.SeriesNumber = dr["SeriesNumber"].ToString();
                    model.CustomerName = dr["CustomerName"].ToString();
                    model.MES_SNOriginal_UID = int.Parse(dr["MES_SNOriginal_UID"].ToString());
                    model.StationName = dr["StationName"].ToString();
                    model.Starttime = Convert.ToDateTime(dr["Starttime"].ToString());
                    model.Color = dr["Color"].ToString();
                    List.Add(model);
                }
            }

            return List;
        }


        public List<MES_SNOriginalDTO> GetMES_SNOriginal(DataTable dt)
        {
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter() { ParameterName = "MES_RepeatDataType", Value = dt };//值为上面转换的datatable
            var repeatModelList = ExecuteRepeatNonQuery("usp_MES_RepeatDataAnalysis", parameters);
            return repeatModelList;
        }

        /// <summary>
        /// 获取工站颜色
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<MES_SNOriginalDTO> GetStationColor(DataTable dt, string stationList)
        {
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter()
            {
                ParameterName = "MES_RepeatDataType",
                Value = dt
            };

            parameters[1] = new SqlParameter()
            {
                ParameterName = "stationColorName",
                Value = stationList
            };

            //值为上面转换的datatable
            var repeatModelList = ExecuteNonQuery("usp_MES_StationColorList", parameters);
            return repeatModelList;
        }


        /// <summary>
        /// 添加没有重复的数据
        /// </summary>
        /// <param name="dt"></param>
        public void AddMES_SNOriginal(DataTable dt)
        {
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter() { ParameterName = "MES_RepeatDataType", Value = dt };//值为上面转换的datatable
            ExecuteStoredProcedure("usp_Add_RepeatDataAnalysis", parameters);
        }

        /// <summary>
        /// 添加没有重复的数据
        /// </summary>
        /// <param name="dt"></param>
        public void AddShipMent(DataTable dt)
        {
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter() { ParameterName = "MES_ShipMentType", Value = dt };//值为上面转换的datatable
            ExecuteStoredProcedure("usp_Add_MesShipMent", parameters);
        }

        public void DeleteShipMent()
        {
            SqlParameter[] parameters = new SqlParameter[0];
            ExecuteStoredProcedure("usp_DeleteShipment", parameters);
        }

        public int GetShipMentCount()
        {
            var sql = $"SELECT COUNT(1) AS ShipMentCount FROM dbo.Mes_OutShipMent";
            var count = ExcuteShipCount(sql.ToString());
            return count;
        }


        /// <summary>
        /// 删除已经出货的SN记录
        /// </summary>
        /// <param name="dt"></param>
        public void DeleteMES_SNOriginal(DataTable dt)
        {
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter() { ParameterName = "MES_RepeatDataType", Value = dt };//值为上面转换的datatable
            ExecuteStoredProcedure("usp_Delete_RepeatDataAnalysis", parameters);
        }

        public void DeleteMES_SNOriginal(string str)
        {
            str = str.TrimEnd(',');
            var sql = $"delete from MES_SNOriginal where SeriesNumber in({str})";
            ExecuteScalar(sql);
            //DataContext.Database.ExecuteSqlCommand(sql);
        }

        public List<Mes_StationColorDTO> GetStationColorListByName(string project_Name)
        {
            var sql = $"SELECT [CustomerName],[StationName],[Color] FROM [dbo].[Mes_StationColor] WHERE CustomerName=N'{project_Name}'";
            var dr = ExecuteReader(sql.ToString());
            return dr;
        }

        public static void ExecuteStoredProcedure(string spName, SqlParameter[] parameterValues)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SyncGoldenLineCT"].ConnectionString;
            List<MES_SNOriginalDTO> resultList = new List<MES_SNOriginalDTO>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                foreach (SqlParameter p in parameterValues)
                {
                    if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(p);
                }
                cmd.ExecuteNonQuery();
            }
        }

        public static List<MES_SNOriginalDTO> ExecuteRepeatNonQuery(string spName, SqlParameter[] parameterValues)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SyncGoldenLineCT"].ConnectionString;
            List<MES_SNOriginalDTO> resultList = new List<MES_SNOriginalDTO>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                foreach (SqlParameter p in parameterValues)
                {
                    if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(p);
                }

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MES_SNOriginalDTO model = new MES_SNOriginalDTO();
                    model.SeriesNumber = dr["SeriesNumber"].ToString();
                    model.CustomerName = dr["CustomerName"].ToString();
                    model.StationName = dr["StationName"].ToString();
                    resultList.Add(model);
                }
            }
            return resultList;
        }

        public static List<MES_SNOriginalDTO> ExecuteNonQuery(string spName, SqlParameter[] parameterValues)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SyncGoldenLineCT"].ConnectionString;
            List<MES_SNOriginalDTO> resultList = new List<MES_SNOriginalDTO>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                foreach (SqlParameter p in parameterValues)
                {
                    if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(p);
                }

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MES_SNOriginalDTO model = new MES_SNOriginalDTO();
                    model.SeriesNumber = dr["SeriesNumber"].ToString();
                    model.CustomerName = dr["CustomerName"].ToString();
                    model.StationName = dr["StationName"].ToString();
                    model.Color = dr["Color"].ToString();
                    model.MES_ProcessID = int.Parse(dr["MES_ProcessID"].ToString());
                    resultList.Add(model);
                }
            }
            return resultList;
        }

        public static object ExecuteScalar(string cmdText)
        {
            object ret = null;
            SqlConnection conn = null;
            try
            {
                conn = OpenConn();
                ret = ExecuteScalar(conn, null, CommandType.Text, cmdText, null);
                return ret;
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public static object ExecuteScalar(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, conn, trans, cmdType, cmdText, cmdParms);
                object ret = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return ret;
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }

        public static System.Data.SqlClient.SqlConnection OpenConn()
        {
            string connectionString = "Data Source=CNCTUG0PISMES01;Initial Catalog=CTU_PDMS_MES;User ID=pdms_mes;Password=PDMS2018@Dbmes;Persist Security Info=true;Connection Timeout=30000";
            return OpenConn(connectionString);
        }

        /// <summary>
        /// 返回指定的数据库连接
        /// </summary>
        /// <param name="connString">指定的数据库连接字符串</param>
        /// <returns>制定连接字符串代表的数据库连接</returns>
        public static System.Data.SqlClient.SqlConnection OpenConn(string connString)
        {
            try
            {
                SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(connString);
                if (sqlConn.State != ConnectionState.Open)
                    sqlConn.Open();
                return sqlConn;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public static List<Mes_StationColorDTO> ExecuteReader(string cmdText)
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["SyncGoldenLineCT"].ConnectionString;
            string connectionString = "Data Source=CNCTUG0PISMES01;Initial Catalog=CTU_PDMS_MES;User ID=pdms_mes;Password=PDMS2018@Dbmes;Persist Security Info=true;Connection Timeout=30000";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                var dr = ExecuteReader(conn, null, CommandType.Text, cmdText, null);
                var resultList = new List<Mes_StationColorDTO>();
                while (dr.Read())
                {
                    Mes_StationColorDTO model = new Mes_StationColorDTO();
                    model.CustomerName = dr["CustomerName"].ToString();
                    model.StationName = dr["StationName"].ToString();
                    model.Color = dr["Color"].ToString();
                    resultList.Add(model);
                }
                return resultList;
            }
        }

        public static int ExcuteShipCount(string cmdText)
        {
            string connectionString = "Data Source=CNCTUG0PISMES01;Initial Catalog=CTU_PDMS_MES;User ID=pdms_mes;Password=PDMS2018@Dbmes;Persist Security Info=true;Connection Timeout=30000";
            var count = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                var dr = ExecuteReader(conn, null, CommandType.Text, cmdText, null);
                while (dr.Read())
                {
                    count = int.Parse(dr["ShipMentCount"].ToString());
                }
                return count;
            }
        }

        public static SqlDataReader ExecuteReader(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            try
            {
                PrepareCommand(cmd, conn, trans, cmdType, cmdText, cmdParms);
                SqlDataReader SqlReader;
                SqlReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return SqlReader;
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (trans != null)
                conn = trans.Connection;

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            if (trans != null)
                cmd.Transaction = trans;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        /// <summary>
        ///  通过SN
        /// </summary>
        /// <returns></returns>
        //public SN_FlowRecordDTO GetSN_FlowRecord(string sn_Name)
        //{
        //    var query = from FlowRecord in DataContext.SN_FlowRecord
        //                where FlowRecord.SN_Name == sn_Name
        //                select new SN_FlowRecordDTO
        //                {
        //                    SN_FlowRecord_UID = FlowRecord.SN_FlowRecord_UID,
        //                    SN_Name = FlowRecord.SN_Name,
        //                    StationName = FlowRecord.StationName,
        //                    ProductTime = FlowRecord.ProductTime
        //                };
        //    return query.FirstOrDefault();
        //}
    }
}
