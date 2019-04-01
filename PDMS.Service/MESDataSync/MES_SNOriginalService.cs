using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using PDMS.Data;
using System.Data.SqlClient;
using System.Configuration;
using PDMS.Data.Repository.MesDataSyncReposity;

namespace PDMS.Service
{
    public interface IMES_SNOriginalService
    {
        List<MES_SNOriginalDTO> GetMES_SNOriginal(DataTable SNOriginalList);
        MES_SNOriginal GetMES_SNOriginal();
        void AddMES_SNOriginal(DataTable SNOriginalList);
        void AddShipMent(DataTable dt);
        void DeleteShipMent();
        bool GetShipMent();
        void DeleteMES_SNOriginal(DataTable dt);
        List<MES_SNOriginalDTO> GetALLData();
        List<MES_SNOriginal> GetTopDate(int top);

        List<Mes_StationColorDTO> GetStationColorList(string Customer);
        List<MES_SNOriginalDTO> GetStationColor(DataTable dt, string stationList);
        IQueryable<MES_SNOriginal> QueryMES_SNOriginalBySN(IList<string> SNList);
        List<Mes_StationColorDTO> GetStationColorListByName(string mesProjectID);
        List<MES_SNOriginalDTO> GetMES_SNOriginalBySN(IList<string> SNList);
    }

    public class MES_SNOriginalService : IMES_SNOriginalService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMES_SNOriginalRepository SNOriginalRepository;
        private readonly IMes_StationColorRepository StationColorRepository;
        private readonly IShipMentRepository ShipMentRepository;
        public MES_SNOriginalService(IUnitOfWork unitOfWork,
               IMES_SNOriginalRepository SNOriginalRepository,
               IMes_StationColorRepository StationColorRepository,
               IShipMentRepository ShipMentRepository
            )
        {
            this.unitOfWork = unitOfWork;
            this.SNOriginalRepository = SNOriginalRepository;
            this.StationColorRepository = StationColorRepository;
            this.ShipMentRepository = ShipMentRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SNOriginalList"></param>
        /// <returns></returns>
        public List<MES_SNOriginalDTO> GetMES_SNOriginal(DataTable SNOriginalList)
        {
            return SNOriginalRepository.GetMES_SNOriginal(SNOriginalList);
        }

        public void AddMES_SNOriginal(DataTable SNOriginalList)
        {
            SNOriginalRepository.AddMES_SNOriginal(SNOriginalList);
        }

        public void DeleteMES_SNOriginal(DataTable dt)
        {
            SNOriginalRepository.DeleteMES_SNOriginal(dt);
        }

        public void DeleteMES_SNOriginal(string str)
        {
            SNOriginalRepository.DeleteMES_SNOriginal(str);
        }

        public void AddShipMent(DataTable dt)
        {
            SNOriginalRepository.AddShipMent(dt);
        }

        /// <summary>
        /// 获取ShipMent
        /// </summary>
        /// <returns></returns>
        public bool GetShipMent()
        {
            try
            {
                var count = SNOriginalRepository.GetShipMentCount();
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void DeleteShipMent()
        {
            SNOriginalRepository.DeleteShipMent();
        }

        public MES_SNOriginal GetMES_SNOriginal()
        {
            return SNOriginalRepository.GetById(604);
        }

        public List<MES_SNOriginalDTO> GetALLData()
        {
            return SNOriginalRepository.GetALLData();
        }

        public List<Mes_StationColorDTO> GetStationColorListByName(string mesProjectName)
        {
            return SNOriginalRepository.GetStationColorListByName(mesProjectName);
        }

        public List<MES_SNOriginal> GetTopDate(int top)
        {
            return SNOriginalRepository.GetTopDate(top);
        }

        public List<Mes_StationColorDTO> GetStationColorList(string CustomerName)
        {
            return StationColorRepository.GetStationColorList(CustomerName);
        }

        public List<Mes_StationColorDTO> GetAllStationColor()
        {
            var result = StationColorRepository.GetStationColorList("Milan-CTU-Housing");
            //List<Mes_StationColorDTO> modelList = new List<Mes_StationColorDTO>();
            //foreach (var item in result)
            //{
            //    Mes_StationColorDTO model = new Mes_StationColorDTO();
            //    model.Mes_StationColor_UID = item.Mes_StationColor_UID;
            //    model.StationName = item.StationName;
            //    model.CustomerName = item.CustomerName;
            //    model.Color = item.Color;
            //    modelList.Add(model);
            //}
            return result;
        }

        /// <summary>
        /// 获取工站颜色
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <returns></returns>
        public List<MES_SNOriginalDTO> GetStationColor(DataTable dt, string stationList)
        {
            return SNOriginalRepository.GetStationColor(dt, stationList);
        }

        public IQueryable<MES_SNOriginal> QueryMES_SNOriginalBySN(IList<string> SNList)
        {
            var SNOriginalQuery = SNOriginalRepository.GetMany(m => SNList.Contains(m.SeriesNumber));
            return SNOriginalQuery;
        }

        public List<MES_SNOriginalDTO> GetMES_SNOriginalBySN(IList<string> SNList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in SNList)
            {
                sb.AppendFormat("\'{0}\'", item);
                sb.Append(",");
            }
            var str = sb.ToString().TrimEnd(',');
            var sql = @" SELECT [MES_SNOriginal_UID]
                              ,[SeriesNumber]
                              ,[CustomerName]
                              ,[StationName]
                              ,[Starttime]
                              ,[Color]
                          FROM [dbo].[MES_SNOriginal]
                          WHERE Color!='0' AND SeriesNumber IN ({0});";
            sql = string.Format(sql, str);
            var resultList = ExecuteReader(sql);
            return resultList;
        }

        /// <summary>
        /// 返回指定的数据库连接
        /// </summary>
        /// <param name="connString">指定的数据库连接字符串</param>
        /// <returns>制定连接字符串代表的数据库连接</returns>
        public static SqlConnection OpenConn(string connString)
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


        public static List<MES_SNOriginalDTO> ExecuteReader(string cmdText)
        {
            var connectionString = "Data Source=CNCTUG0PISMES01;Initial Catalog=CTU_PDMS_MES;User ID=pdms_mes;Password=PDMS2018@Dbmes;Persist Security Info=true;Connection Timeout=300";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                var dr = ExecuteReader(conn, null, CommandType.Text, cmdText, null);
                List<MES_SNOriginalDTO> resultList = new List<MES_SNOriginalDTO>();
                while (dr.Read())
                {
                    MES_SNOriginalDTO model = new MES_SNOriginalDTO();
                    model.MES_SNOriginal_UID = int.Parse(dr["MES_SNOriginal_UID"].ToString());
                    model.SeriesNumber = dr["SeriesNumber"].ToString();
                    model.CustomerName = dr["CustomerName"].ToString();
                    model.StationName = dr["StationName"].ToString();
                    model.Color = dr["Color"].ToString();
                    model.Starttime = Convert.ToDateTime(dr["Starttime"].ToString());
                    resultList.Add(model);
                }
                return resultList;
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

    }
}
