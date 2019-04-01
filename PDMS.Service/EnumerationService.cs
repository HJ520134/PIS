using PDMS.Data;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IEnumerationService : IBaseSevice<Enumeration, EnumerationDTO, EnumerationModelSearch>
    {
        /// <summary>
        /// 通过枚举类型获取值
        /// </summary>
        /// <param name="machineDataSource"></param>
        /// <returns></returns>
        List<string> GetMachineDataSource(string machineDataSource);
    }
    public class EnumerationService : BaseSevice<Enumeration, EnumerationDTO, EnumerationModelSearch>, IEnumerationService
    {
        private readonly IEnumerationRepository enumberationRepository;
        public EnumerationService(IEnumerationRepository enumberationRepository)
            : base(enumberationRepository)
        {
            this.enumberationRepository = enumberationRepository;
        }

        /// <summary>
        /// 通过枚举类型获取值
        /// </summary>
        /// <param name="machineDataSource"></param>
        /// <returns></returns>
        public List<string> GetMachineDataSource(string machineDataSource)
        {
            return enumberationRepository.GetEnumValuebyType(machineDataSource);
        }
    }
}
