using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Data.Repository.MesDataSyncReposity;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Mes_ShipmentSchedult
{
    public class Program
    {
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();
        public static void Main(string[] args)
        {
            MES_SNOriginalService SNOriginalService = new MES_SNOriginalService
                 (
                        new UnitOfWork(_DatabaseFactory),
                        new MES_SNOriginalRepository(_DatabaseFactory),
                        new Mes_StationColorRepository(_DatabaseFactory),
                        new ShipMentRepository(_DatabaseFactory)
                );

            var count = Process.GetProcessesByName("Mes_ShipmentSchedult").ToList().Count();
            if (count == 0)
            {
                while (SNOriginalService.GetShipMent())
                {
                    SNOriginalService.DeleteShipMent();
                }
            }
        }
    }
}
