using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using System.Data.Entity.SqlServer;


namespace PDMS.Data.Repository
{
//    public interface IMaterialApplyRepository : IRepository<Material_Apply>
//    {
//        IQueryable<MaterialManageDTO> GetInfo(MaterialManageDTO searchModel, Page page, out int totalcount);
//        List<MaterialManageDTO> GetByUId(int Material_Apply_Uid);
//        List<MaterialManageDTO> DoExportFunction(string materialid, string materialname, string materialtypes,
//            string classification);
//    }
//    public class MaterialApplyRepository : RepositoryBase<Material_Apply>, IMaterialApplyRepository
//    {
//        public MaterialApplyRepository(IDatabaseFactory databaseFactory)
//            : base(databaseFactory)
//        {
//        }
//        public IQueryable<MaterialManageDTO> GetInfo(MaterialManageDTO searchModel, Page page, out int totalcount)
//        {
//            var query = from M in DataContext.Material_Apply
//                        join mat in DataContext.Material_Info
//                        on M.Material_Uid equals mat.Material_Uid
//                        select new MaterialManageDTO
//                        {
//                            Material_Apply_Uid = M.Material_Apply_Uid,
//                            Apply_Type = M.Apply_Type,
//                            Status = M.Status,
//                            Material_Id = mat.Material_Id,
//                            Material_Name = mat.Material_Name,
//                            Material_Types = mat.Material_Types,
//                            Classification = mat.Classification,
//                            Apply_Number = M.Apply_Number,
//                            Actual_Number = 0,
//                            Applyer=M.Applyer,
//                            Apply_Date=M.Apply_Date
//                        };

//            if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
//                query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
//            if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
//                query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
//            if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
//                query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));
//            if (!string.IsNullOrWhiteSpace(searchModel.Classification))
//                query = query.Where(m => m.Classification.Contains(searchModel.Classification));
//            query = query.OrderByDescending(m => m.Apply_Date).ThenBy(m => m.Apply_Number).GetPage(page);
//            totalcount = query.Count();
//            return query;
//        }

//        public List<MaterialManageDTO> GetByUId(int Material_Apply_Uid)
//        {
//            string sql = @"SELECT t1.*,t2.Material_Name,t2.Material_Types FROM dbo.Material_Apply t1 LEFT JOIN dbo.Material_Info t2
//                            ON t1.Material_Uid=t2.Material_Uid  WHERE Material_Apply_Uid={0}";
//            sql = string.Format(sql, Material_Apply_Uid);
//            var dblist = DataContext.Database.SqlQuery<MaterialManageDTO>(sql).ToList();
//            return dblist;
//        }

//        public List<MaterialManageDTO> DoExportFunction(string materialid, string materialname, string materialtypes,
//    string classification)
//        {
//            var query = from M in DataContext.Material_Apply
//                        join mat in DataContext.Material_Info
//                        on M.Material_Uid equals mat.Material_Uid
//                        join user1 in DataContext.System_Users
//                        on M.Acceprter equals user1.Account_UID into temp
//                        from bb in temp.DefaultIfEmpty()
//                        join user2 in DataContext.System_Users
//                        on M.Applyer equals user2.Account_UID
//                        select new MaterialManageDTO
//                        {
//                            Material_Apply_Uid = M.Material_Apply_Uid,
//                            Apply_Type = M.Apply_Type,
//                            Status = M.Status,
//                            Material_Id = mat.Material_Id,
//                            Material_Name = mat.Material_Name,
//                            Material_Types = mat.Material_Types,
//                            Classification = mat.Classification,
//                            Apply_Number = M.Apply_Number,
//                            Actual_Number = 0,
//                            Applyer_Name = user2.User_Name,
//                            Apply_Date = M.Apply_Date
//                        };
//            if (!string.IsNullOrWhiteSpace(materialid))
//                query = query.Where(m => m.Material_Id.Contains(materialid));
//            if (!string.IsNullOrWhiteSpace(materialname))
//                query = query.Where(m => m.Material_Name.Contains(materialname));
//            if (!string.IsNullOrWhiteSpace(materialtypes))
//                query = query.Where(m => m.Material_Types.Contains(materialtypes));
//            if (!string.IsNullOrWhiteSpace(classification))
//                query = query.Where(m => m.Classification.Contains(classification));
//            query = query.OrderByDescending(m => m.Apply_Date).ThenBy(m => m.Apply_Number);
//            return query.ToList();
//        }
//    }
}
