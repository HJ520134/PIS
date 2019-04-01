using PDMS.Core;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GL_WIPHourOutputSchedule
{
    public class Program
    {
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();

        static void Main(string[] args)
        {
            GL_WIPHourOutputService WIPHourOutputService = new GL_WIPHourOutputService
              (
                  new GL_WIPHourOutputRepository(_DatabaseFactory),
                  new GoldenLineRepository(_DatabaseFactory),
                  new GL_LineShiftPerfRepository(_DatabaseFactory),
                  new GL_BuildPlanRepository(_DatabaseFactory),
                  new GL_ShiftTimeRepository(_DatabaseFactory),
                  new UnitOfWork(_DatabaseFactory)
             );
            WIPHourOutputService.ExcuteGL_WIPHourOutput(DateTime.Now);
        }
    }
}
