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
    public interface IRepairLocationRepository: IRepository<Repair_Location>
    {
        IQueryable<RepairLocationDTO> GetInfo(RepairLocationDTO searchModel, Page page, out int totalcount);
        string InsertItem(List<RepairLocationDTO> dtolist);
        List<RepairLocationDTO> DoExportFunction(string uids);
        List<RepairLocationDTO> DoAllRLExportFunction(RepairLocationDTO searchModel);
        #region Added by Jay
        List<RepairLocationDTO> GetRepairLocationList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        #endregion
    }
    public class RepairLocationRepository: RepositoryBase<Repair_Location>, IRepairLocationRepository
    {
        public RepairLocationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public IQueryable<RepairLocationDTO> GetInfo(RepairLocationDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Repair_Location
                        join plantorg in DataContext.System_Organization
                        on M.Plant_Organization_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on M.FunPlant_Organization_UID equals funplantorg.Organization_UID into temp
                        from aa in temp.DefaultIfEmpty()
                        join users1 in DataContext.System_Users
                        on M.Created_UID equals users1.Account_UID
                        join users2 in DataContext.System_Users
                        on M.Modified_UID equals users2.Account_UID
                        select new RepairLocationDTO
                        {
                            Repair_Location_UID = M.Repair_Location_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = aa.Organization_Name,
                            Repair_Location_ID = M.Repair_Location_ID,
                            Repair_Location_Name = M.Repair_Location_Name,
                            Repair_Location_Desc = M.Repair_Location_Desc,
                            Is_Enable = M.Is_Enable,
                            Creator = users1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = users2.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID!=null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Location_ID))
                query = query.Where(m => m.Repair_Location_ID.Contains(searchModel.Repair_Location_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Location_Name))
                query = query.Where(m => m.Repair_Location_Name.Contains(searchModel.Repair_Location_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Location_Desc))
                query = query.Where(m => m.Repair_Location_Desc.Contains(searchModel.Repair_Location_Desc));
            if (searchModel.needSearchEnable)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Repair_Location_ID).GetPage(page);
            return query;
        }
        public string InsertItem(List<RepairLocationDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        string strFunplantUid = "null";
                        if (dtolist[i].FunPlant_Organization_UID != null)
                            strFunplantUid = dtolist[i].FunPlant_Organization_UID.ToString();
                        var sql = string.Format(@"INSERT INTO dbo.Repair_Location
                                                        ( Plant_Organization_UID ,
                                                          BG_Organization_UID ,
                                                          FunPlant_Organization_UID ,
                                                          Repair_Location_ID ,
                                                          Repair_Location_Name ,
                                                          Repair_Location_Desc ,
                                                          Is_Enable ,
                                                          Created_UID ,
                                                          Created_Date ,
                                                          Modified_UID ,
                                                          Modified_Date
                                                        )
                                                VALUES  ( {0} , -- Plant_Organization_UID - int
                                                          {1} , -- BG_Organization_UID - int
                                                          {2} , -- FunPlant_Organization_UID - int
                                                          N'{3}' , -- Repair_Location_ID - nvarchar(10)
                                                          N'{4}' , -- Repair_Location_Name - nvarchar(30)
                                                          N'{5}' , -- Repair_Location_Desc - nvarchar(100)
                                                          {6} , -- Is_Enable - bit
                                                          {7} , -- Created_UID - int
                                                          N'{8}' , -- Created_Date - datetime
                                                          {7} , -- Modified_UID - int
                                                          N'{8}'  -- Modified_Date - datetime
                                                        )",
                                            dtolist[i].Plant_Organization_UID,
                                            dtolist[i].BG_Organization_UID,
                                            strFunplantUid,
                                            dtolist[i].Repair_Location_ID,
                                            dtolist[i].Repair_Location_Name,
                                            dtolist[i].Repair_Location_Desc,
                                            dtolist[i].Is_Enable?1:0,
                                            dtolist[i].Created_UID,
                                            DateTime.Now.ToString(FormatConstants.DateTimeFormatString));

                        DataContext.Database.ExecuteSqlCommand(sql);
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
        public List<RepairLocationDTO> DoExportFunction(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Repair_Location
                        join plantorg in DataContext.System_Organization
                        on M.Plant_Organization_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on M.FunPlant_Organization_UID equals funplantorg.Organization_UID into temp
                        from aa in temp.DefaultIfEmpty()
                        join users1 in DataContext.System_Users
                        on M.Created_UID equals users1.Account_UID
                        join users2 in DataContext.System_Users
                        on M.Modified_UID equals users2.Account_UID
                        where uids.Contains("," + M.Repair_Location_UID + ",")
                        select new RepairLocationDTO
                        {
                            Repair_Location_UID = M.Repair_Location_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = aa.Organization_Name,
                            Repair_Location_ID = M.Repair_Location_ID,
                            Repair_Location_Name = M.Repair_Location_Name,
                            Repair_Location_Desc = M.Repair_Location_Desc,
                            Is_Enable = M.Is_Enable,
                            Creator = users1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = users2.User_Name,
                            Modified_Date = M.Modified_Date
                        };
            return query.ToList();
        }
        public List<RepairLocationDTO> DoAllRLExportFunction(RepairLocationDTO searchModel)
        {
            var query = from M in DataContext.Repair_Location
                        join plantorg in DataContext.System_Organization
                        on M.Plant_Organization_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on M.FunPlant_Organization_UID equals funplantorg.Organization_UID into temp
                        from aa in temp.DefaultIfEmpty()
                        join users1 in DataContext.System_Users
                        on M.Created_UID equals users1.Account_UID
                        join users2 in DataContext.System_Users
                        on M.Modified_UID equals users2.Account_UID
                        select new RepairLocationDTO
                        {
                            Repair_Location_UID = M.Repair_Location_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = aa.Organization_Name,
                            Repair_Location_ID = M.Repair_Location_ID,
                            Repair_Location_Name = M.Repair_Location_Name,
                            Repair_Location_Desc = M.Repair_Location_Desc,
                            Is_Enable = M.Is_Enable,
                            Creator = users1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = users2.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Location_ID))
                query = query.Where(m => m.Repair_Location_ID.Contains(searchModel.Repair_Location_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Location_Name))
                query = query.Where(m => m.Repair_Location_Name.Contains(searchModel.Repair_Location_Name));
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_Location_Desc))
                query = query.Where(m => m.Repair_Location_Desc.Contains(searchModel.Repair_Location_Desc));
            if (searchModel.needSearchEnable)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);

           
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Repair_Location_ID);
            return query.ToList();
        }
        #region Added by Jay
        public List<RepairLocationDTO> GetRepairLocationList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from repairLocation in DataContext.Repair_Location
                        where repairLocation.Is_Enable.Equals(true)
                        select new RepairLocationDTO
                        {
                            Repair_Location_UID = repairLocation.Repair_Location_UID,
                            Plant_Organization_UID = repairLocation.Plant_Organization_UID,
                            BG_Organization_UID = repairLocation.BG_Organization_UID,
                            FunPlant_Organization_UID = repairLocation.FunPlant_Organization_UID,
                            Repair_Location_ID = repairLocation.Repair_Location_ID,
                            Repair_Location_Name = repairLocation.Repair_Location_Name,
                            Repair_Location_Desc = repairLocation.Repair_Location_Desc,
                            Is_Enable = repairLocation.Is_Enable
                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }
        #endregion
    }
}
