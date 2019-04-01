using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IMaterialReasonRepository: IRepository<Material_Reason>
    {
        IQueryable<MaterialReasonDTO> GetInfo(MaterialReasonDTO searchModel, Page page, out int totalcount);
        List<MaterialReasonDTO> GetByUid(int Material_Reason_UID);
        string InsertItem(List<MaterialReasonDTO> dtolist);

        List<MaterialReasonDTO> DoExportMatReasonReprot(string Material_Reason_UIDs);

        List<MaterialReasonDTO> DoAllExportMatReasonReprot(MaterialReasonDTO search);
    }
    public class MaterialReasonRepository: RepositoryBase<Material_Reason>, IMaterialReasonRepository
    {
        public MaterialReasonRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IQueryable<MaterialReasonDTO> GetInfo(MaterialReasonDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Material_Reason
                        join mat in DataContext.Material_Info
                        on M.Material_UID equals mat.Material_Uid
                        join users in DataContext.System_Users
                        on M.Modified_UID equals users.Account_UID
                        select new MaterialReasonDTO
                        {
                            Material_Reason_UID = M.Material_Reason_UID,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Reason = M.Reason,
                            ModifiedUser = users.User_Name,
                            Modified_Date = M.Modified_Date,
                            PlantId= mat.Organization_UID
                        };
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
                query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
                query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
                query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
            if (!string.IsNullOrWhiteSpace(searchModel.ModifiedUser))
                query = query.Where(m => m.ModifiedUser.Contains(searchModel.ModifiedUser));
            if(searchModel.PlantId!=0)
                query = query.Where(m => m.PlantId==searchModel.PlantId);
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Material_Id).GetPage(page);
            return query;
        }

        public List<MaterialReasonDTO> GetByUid(int Material_Reason_UID)
        {
            string sql = @"select * from Material_Reason t1 inner join Material_Info t2
                                    on t1.Material_UID=t2.Material_Uid inner join System_Users t3
                                    on t1.Modified_UID=T3.Account_UID WHERE Material_Reason_UID={0}";
            sql = string.Format(sql, Material_Reason_UID);
            var dblist = DataContext.Database.SqlQuery<MaterialReasonDTO>(sql).ToList();
            return dblist;
        }

        public string InsertItem(List<MaterialReasonDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        if (dtolist[i].Material_Reason_UID != 0)
                        {
                            var sql = string.Format(@"UPDATE dbo.Material_Reason SET Material_UID={0},Reason=N'{1}',Modified_UID={2},
                            Modified_Date = N'{3}'  WHERE Material_Reason_UID = {4}",
                            dtolist[i].Material_UID,
                            dtolist[i].Reason,
                            dtolist[i].Modified_UID,
                            DateTime.Now.ToString(FormatConstants.DateTimeFormatString));
                            DataContext.Database.ExecuteSqlCommand(sql);
                        }
                        else
                        {
                            var sql = string.Format(@"insert into Material_Reason values ({0},N'{1}',{2},N'{3}')",
                            dtolist[i].Material_UID,
                            dtolist[i].Reason,
                            dtolist[i].Modified_UID,
                            DateTime.Now.ToString(FormatConstants.DateTimeFormatString));
                            DataContext.Database.ExecuteSqlCommand(sql);
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    result = "Error" + ex;
                }
                return result;
            }
        }


       public List<MaterialReasonDTO> DoExportMatReasonReprot(string Material_Reason_UIDs)
        {
            Material_Reason_UIDs = "," + Material_Reason_UIDs + ",";
            var query = from M in DataContext.Material_Reason
                        join mat in DataContext.Material_Info
                        on M.Material_UID equals mat.Material_Uid
                        join users in DataContext.System_Users
                        on M.Modified_UID equals users.Account_UID
                        select new MaterialReasonDTO
                        {
                            Material_Reason_UID = M.Material_Reason_UID,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Reason = M.Reason,
                            ModifiedUser = users.User_Name,
                            Modified_Date = M.Modified_Date,
                            PlantId = mat.Organization_UID
                        };
            query = query.Where(m => Material_Reason_UIDs.Contains("," + m.Material_Reason_UID + ","));
           
            return query.ToList();

        }

        public List<MaterialReasonDTO> DoAllExportMatReasonReprot(MaterialReasonDTO search)
        {

            var query = from M in DataContext.Material_Reason
                        join mat in DataContext.Material_Info
                        on M.Material_UID equals mat.Material_Uid
                        join users in DataContext.System_Users
                        on M.Modified_UID equals users.Account_UID
                        select new MaterialReasonDTO
                        {
                            Material_Reason_UID = M.Material_Reason_UID,
                            Material_Id = mat.Material_Id,
                            Material_Name = mat.Material_Name,
                            Material_Types = mat.Material_Types,
                            Reason = M.Reason,
                            ModifiedUser = users.User_Name,
                            Modified_Date = M.Modified_Date,
                            PlantId = mat.Organization_UID
                        };
            if (!string.IsNullOrWhiteSpace(search.Material_Id))
                query = query.Where(m => m.Material_Id.Contains(search.Material_Id));
            if (!string.IsNullOrWhiteSpace(search.Material_Name))
                query = query.Where(m => m.Material_Name.Contains(search.Material_Name));
            if (!string.IsNullOrWhiteSpace(search.Material_Types))
                query = query.Where(m => m.Material_Types.Contains(search.Material_Types));
            if (!string.IsNullOrWhiteSpace(search.ModifiedUser))
                query = query.Where(m => m.ModifiedUser.Contains(search.ModifiedUser));
            if (search.PlantId != 0)
                query = query.Where(m => m.PlantId == search.PlantId);          
            return query.ToList();
        }
    }
}
