using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using PDMS.Common.Constants;

namespace PDMS.Data.Repository
{
    public interface IEQPUserTableRepository : IRepository<EQP_UserTable>
    {
        IQueryable<EQPUserTableDTO> GetInfo(EQPUserTableDTO searchModel, Page page, out int totalcount);
        List<EQPUserTableDTO> GetByUId(int EQPUser_Uid);
        string DeleteEQPUser(int EQPUser_Uid);
        string InsertItem(List<EQPUserTableDTO> dtolist);
        List<EQP_UserTable> GetByUserId(string User_id);
    }
    public class EQPUserTableRepository : RepositoryBase<EQP_UserTable>, IEQPUserTableRepository
    {
        public EQPUserTableRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IQueryable<EQPUserTableDTO> GetInfo(EQPUserTableDTO searchModel, Page page, out int totalcount)
        {
            var query = from user in DataContext.EQP_UserTable
                            //join user_org in DataContext.System_Organization on user.BG_Organization_UID equals user_org.Organization_UID
                            //join user_funPlant in DataContext.System_Organization on user.FunPlant_Organization_UID equals user_funPlant.Organization_UID

                        select new EQPUserTableDTO
                        {
                            EQPUser_Uid = user.EQPUser_Uid,
                            BG_Organization_UID = user.BG_Organization_UID,
                            // BG_Organization= user_org.Organization_Name,
                            FunPlant_Organization_UID = user.FunPlant_Organization_UID,
                            // FunPlant_Organization= user_funPlant.Organization_Name,
                            User_Id = user.User_Id,
                            User_Name = user.User_Name,
                            User_Call = user.User_Call,
                            User_Email = user.User_Email,
                            Modified_Date = user.Modified_Date,
                            Plant_OrganizationUID = user.Plant_OrganizationUID,
                            IsDisable = user.IsDisable,
                            IsDisableName = user.IsDisable == 1 ? "启用" : "禁用",
                        };


            query = SetEQPUserPlantName(query.ToList());

            if (searchModel.User_Id != 0)
                query = query.Where(m => m.User_Id.ToString().Contains(searchModel.User_Id.ToString()));
            if (!string.IsNullOrWhiteSpace(searchModel.User_Name))
                query = query.Where(m => m.User_Name.Contains(searchModel.User_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.User_Call))
                query = query.Where(m => m.User_Call.Contains(searchModel.User_Call));
            if (!string.IsNullOrWhiteSpace(searchModel.User_Email))
                query = query.Where(m => m.User_Email.Contains(searchModel.User_Email));
            //if (searchModel.Plant_OrganizationUID != 0 && searchModel.Organization_UID == 0)
            //{
            //  var oplist=  DataContext.System_OrganizationBOM.Where(o => o.ParentOrg_UID == searchModel.Plant_OrganizationUID).Select(o => o.ChildOrg_UID).ToList();
            //    query = query.Where(m => oplist.Contains(m.BG_Organization_UID.Value));

            //}
            if (searchModel.Plant_OrganizationUID != 0)
            {
                query = query.Where(m => m.Plant_OrganizationUID == searchModel.Plant_OrganizationUID);

            }
            if (searchModel.Plant_OrganizationUID != 0 && searchModel.Organization_UID != null && searchModel.Organization_UID != 0)
            {
                query = query.Where(m => m.BG_Organization_UID == searchModel.Organization_UID);

            }

            if (searchModel.FunPlant_OrganizationUID != null && searchModel.FunPlant_OrganizationUID != 0)
            {
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_OrganizationUID);

            }
            //等于2表示查询全部
            if (searchModel.IsDisable != 2)
            {
                query = query.Where(m => m.IsDisable == searchModel.IsDisable);
            }

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.User_Id).GetPage(page);
            return query;
        }
        //设置获取功能厂名称
        public IQueryable<EQPUserTableDTO> SetEQPUserPlantName(List<EQPUserTableDTO> EQPUserTableDTOs)
        {
            var System_Organization = DataContext.System_Organization.ToList();
            foreach (var item in EQPUserTableDTOs)
            {
                if (item.BG_Organization_UID != null && item.BG_Organization_UID != 0)
                {
                    if (System_Organization.Where(o => o.Organization_UID == item.BG_Organization_UID).Count() > 0)
                        item.BG_Organization = System_Organization.Where(o => o.Organization_UID == item.BG_Organization_UID).FirstOrDefault().Organization_Name;

                }
                if (item.FunPlant_Organization_UID != null && item.FunPlant_Organization_UID != 0)
                {
                    if (System_Organization.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).Count() > 0)
                        item.FunPlant_Organization = System_Organization.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).FirstOrDefault().Organization_Name;
                }
            }

            return EQPUserTableDTOs.AsQueryable();
        }
        public List<EQPUserTableDTO> GetByUId(int EQPUser_Uid)
        {
            string sql = @"SELECT * FROM dbo.EQP_UserTable where EQPUser_Uid={0} ";
            sql = string.Format(sql, EQPUser_Uid);
            var dblist = DataContext.Database.SqlQuery<EQP_UserTable>(sql).ToList();


            var tt = new List<EQPUserTableDTO>();

            foreach (var item in dblist)
            {
                var eQPUserTableDTO = new EQPUserTableDTO()
                {
                    EQPUser_Uid = item.EQPUser_Uid,
                    BG_Organization_UID = item.BG_Organization_UID,
                    FunPlant_Organization_UID = item.FunPlant_Organization_UID,
                    User_Id = item.User_Id,
                    User_Name = item.User_Name,
                    User_IdAndName = item.User_IdAndName,
                    User_Email = item.User_Email,
                    User_Call = item.User_Call,
                    FunPlant_OrganizationUID = item.FunPlant_Organization_UID,
                    Organization_UID = item.BG_Organization_UID,
                    Plant_OrganizationUID = item.Plant_OrganizationUID,
                    IsDisable = item.IsDisable,
                    IsDisableName = item.IsDisable == 1 ? "启用" : "禁用"
                    // BG_Organization= DataContext.System_Organization.Where(o=>o.Organization_UID==item.BG_Organization_UID).FirstOrDefault().Organization_Name,
                    // FunPlant_Organization= DataContext.System_Organization.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).FirstOrDefault().Organization_Name

                };
                var System_Organizations = DataContext.System_Organization.ToList();
                if (item.BG_Organization_UID != null)
                {
                    if (System_Organizations.Where(o => o.Organization_UID == item.BG_Organization_UID).Count() > 0)
                        eQPUserTableDTO.BG_Organization = System_Organizations.Where(o => o.Organization_UID == item.BG_Organization_UID).FirstOrDefault().Organization_Name;

                }
                if (item.FunPlant_Organization_UID != null)
                {
                    if (System_Organizations.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).Count() > 0)
                        eQPUserTableDTO.FunPlant_Organization = System_Organizations.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).FirstOrDefault().Organization_Name;
                }

                tt.Add(eQPUserTableDTO);
            }
            return tt;

            // return dblist;
        }

        public string DeleteEQPUser(int EQPUser_Uid)
        {
            try
            {
                string sql = "delete EQP_UserTable where EQPUser_Uid={0}";
                sql = string.Format(sql, EQPUser_Uid);
                int result = DataContext.Database.ExecuteSqlCommand(sql);
                if (result == 1)
                    return "";
                else
                    return "删除人员账号失败";
            }
            catch (Exception e)
            {
                return "此人员账号在使用中，不能删除！";
            }
        }

        public string InsertItem(List<EQPUserTableDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        if (dtolist[i].EQPUser_Uid != 0)
                        {
                            //var thisBG_Organization_UID= "";
                            //if(dtolist[i].BG_Organization_UID!=null)
                            //{
                            //    thisBG_Organization_UID = dtolist[i].BG_Organization_UID.ToString();
                            //}
                            //var thisFunPlant_Organization_UID = "";
                            //if(dtolist[i].FunPlant_Organization_UID!=null)
                            //{
                            //    thisFunPlant_Organization_UID = dtolist[i].FunPlant_Organization_UID.ToString();
                            //}
                            var sql = string.Format(@"UPDATE EQP_UserTable SET User_Name=N'{0}',User_IdAndName=N'{1}',User_Email=N'{2}',
                                        User_Call = N'{3}', Modified_UID ={4},Modified_Date = N'{5}',BG_Organization_UID='{7}' ,FunPlant_Organization_UID='{8}',Plant_OrganizationUID={9},IsDisable={10} WHERE EQPUser_Uid = {6}",
                            dtolist[i].User_Name,
                            dtolist[i].User_IdAndName,
                            dtolist[i].User_Email,
                            dtolist[i].User_Call,
                            dtolist[i].Modified_UID,
                            DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                            dtolist[i].EQPUser_Uid,
                            dtolist[i].BG_Organization_UID,
                            dtolist[i].FunPlant_Organization_UID,
                            dtolist[i].Plant_OrganizationUID,
                            dtolist[i].IsDisable
                            );
                            DataContext.Database.ExecuteSqlCommand(sql);
                        }
                        else
                        {
                            var sql = string.Format("insert into EQP_UserTable values ('{0}','{1}',{2},N'{3}',N'{4}',N'{5}',N'{6}',N'{7}',N'{8}',{9},N'{10}')",
                            dtolist[i].BG_Organization_UID,
                            dtolist[i].FunPlant_Organization_UID,
                            dtolist[i].User_Id,
                            dtolist[i].User_Name,
                            dtolist[i].User_IdAndName,
                            dtolist[i].User_Call,
                            dtolist[i].Modified_UID,
                            DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                            dtolist[i].User_Email,
                            dtolist[i].Plant_OrganizationUID,
                            dtolist[i].IsDisable
                            );
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

        public List<EQP_UserTable> GetByUserId(string User_id)
        {
            string sql = @"SELECT * FROM dbo.EQP_UserTable where User_id='{0}' ";
            sql = string.Format(sql, User_id);
            var dblist = DataContext.Database.SqlQuery<EQP_UserTable>(sql).ToList();
            return dblist;
        }
    }
}
