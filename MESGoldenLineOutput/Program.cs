using ADODB;
using JEMS_3;
using PDMS.Common.Enums;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESGoldenLineOutput
{
    class Program
    {
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();
        static void Main(string[] args)
        {
            //先通过专案和当前时间获取班次相关信息。

            SyncMESGoldenLine();

          //private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();
          //static string MesProject = "Milan-CTU-Housing";
          //static  DateTime ExcTime = DateTime.Now;

          

            //var MESShiftInfo = GL_Services.GetMesTime(ExcTime, MesProject);

        }

        public static void  SyncMESGoldenLine()
        {

            GL_WIPHourOutputService gL_GoldenStation = new GL_WIPHourOutputService(
            new GL_WIPHourOutputRepository(_DatabaseFactory),
             new GoldenLineRepository(_DatabaseFactory),
            new GL_LineShiftPerfRepository(_DatabaseFactory),
             new GL_BuildPlanRepository(_DatabaseFactory),
             new GL_ShiftTimeRepository(_DatabaseFactory),
            new UnitOfWork(_DatabaseFactory)
          );
            string MesProject = "Milan-CTU-Housing";
            DateTime ExcTime = DateTime.Now;
            //先通过信息获取Mes班次信息。
            MESTimeInfo MesTimeInfo= gL_GoldenStation.GetMesTime(ExcTime, MesProject);
            //获取MES同步信息
            var MesData = GetMesAPIData(MesTimeInfo.StartTime,MesTimeInfo.EndTime);

            //将数据传到后端插入到数据库
            gL_GoldenStation.ExcuteGL_MesHourOutPut(MesData, MesTimeInfo, MesProject);

        }

        //获取班次相关信息
        //public static MESTimeInfo GetMESTimeInfo(DateTime ExcTime, string MesProject)
        //{
        //    return GL_Services.GetMesTime(ExcTime, MesProject);
        //}


        //获取MES接口
        public static Dictionary<string ,int>  GetMesAPIData(string startTime, string EndTime)
        {
            //3 转换MES接口的数据
            try
            {
                WP_WIPRouteSteps wp = new WP_WIPRouteSteps();
                List<string> ReturnList = new List<string>();
                Recordset mesRecode = wp.ListWIPHistory("cnctug0SQLV03a", "jems", 257, startTime, EndTime, "22");
                //cnctug0SQLV03a,jems,257,0,0,22,17,Milan-CTU-Housing,FactoryMARouteIDString,RouteStepIDString,SerialNumberString
                //Recordset re = wp.ListWIPHistory("azapsectusql42", "jems", 794, "2018/03/29 12:00", "2018/03/29 14:00", "17");
                //var s = mesRecode.GetString();
                while (!mesRecode.EOF)
                {
                    ReturnList.Add(mesRecode.Fields[19].Value.ToString());
                    mesRecode.MoveNext();
                }
                var dic = ReturnList.GroupBy(p => p).ToDictionary(p => p.Key, q => q.Count());
                mesRecode.Close();

            
                return dic;
            }
            catch (Exception ex)
            {
                return null;
                
            }
        }
        
        

        
    }
}
