using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface ILogMessageRecordService
    {
        bool AddLog(LogMessageRecord logModel);
    }

    public class LogMessageRecordService : ILogMessageRecordService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMessageRecodeRepository LogRecodeRepository;
        public LogMessageRecordService(IUnitOfWork unitOfWork,
             ILogMessageRecodeRepository LogMessageRecodeRepository)
        {
            this.unitOfWork = unitOfWork;
            this.LogRecodeRepository = LogMessageRecodeRepository;
        }

        public bool AddLog(LogMessageRecord logModel)
        {
            try
            {
                var result = LogRecodeRepository.Add(logModel);
                unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
