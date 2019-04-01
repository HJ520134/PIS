using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface ICNCMachineColumnTableRepository : IRepository<CNCMachineColumnTable>
    {
        List<CNCMachineColumnTableDTO> GetCNCMachineColumnTableDTOs(int Account_UID);
        string InsertMachineColumnTable(List<CNCMachineColumnTableDTO> CNCMachineDTOs);
    }
    public class CNCMachineColumnTableRepository : RepositoryBase<CNCMachineColumnTable>, ICNCMachineColumnTableRepository
    {
        public CNCMachineColumnTableRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {


        }

        public List<CNCMachineColumnTableDTO> GetCNCMachineColumnTableDTOs(int Account_UID)
        {
            var query = from M in DataContext.CNCMachineColumnTable
                        select new CNCMachineColumnTableDTO
                        {

                            Column_UID = M.Column_UID,
                            Column_Name = M.Column_Name,
                            NTID = M.NTID,
                            Modified_Date = M.Modified_Date
                        };

            if (Account_UID != 0)
            {
                query = query.Where(o => o.NTID == Account_UID);
            }
            return query.ToList();

        }
        public string InsertMachineColumnTable(List<CNCMachineColumnTableDTO> CNCMachineDTOs)
        {

            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    if (CNCMachineDTOs != null && CNCMachineDTOs.Count > 0)
                    {

                        foreach (var item in CNCMachineDTOs)
                        {

                            if (item.Column_UID == 0)
                            {
                                //构造插入SQL数据
                                var insertSql = string.Format(@"INSERT INTO CNCMachineColumnTable
                                                                               (Column_Name
                                                                               ,NTID
                                                                               ,Modified_Date)
                                                                         VALUES
                                                                               (
		                                                                       '{0}',
                                                                                {1},
                                                                               '{2}'
		                                                                       );",
                                                                         item.Column_Name, item.NTID,  item.Modified_Date);



                          
                                sb.AppendLine(insertSql);
                            }
                            else
                            {

                                var updateSql = string.Format(@"UPDATE CNCMachineColumnTable
                                                                   SET Column_Name = '{0}'
                                                                      ,NTID = {1}
                                                                      ,Modified_Date ='{2}'
                                                                 WHERE Column_UID={3};", item.Column_Name, item.NTID, item.Modified_Date,item.Column_UID);

                                sb.AppendLine(updateSql);

                            }
                        }
                        string sql = sb.ToString();
                        DataContext.Database.ExecuteSqlCommand(sb.ToString());
                        trans.Commit();
                    }
                }

                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
