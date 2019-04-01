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

    public interface IOEE_DownTypeRepository : IRepository<OEE_DownTimeType>
    {
        IQueryable<OEE_DownTypeDTO> QueryOEE_DownType(OEE_DownTypeDTO searchModel, Page page, out int totalcount);

        OEE_DownTypeDTO QueryDownTypeByUid(int OEE_DownTimeType_UID);


    }

    public class OEE_DownTypeRepository : RepositoryBase<OEE_DownTimeType>, IOEE_DownTypeRepository
    {

        public OEE_DownTypeRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }


        public IQueryable<OEE_DownTypeDTO> QueryOEE_DownType(OEE_DownTypeDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.OEE_DownTimeType
                        select new OEE_DownTypeDTO
                        {
                            OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID=M.BG_Organization_UID,
                            FunPlant_Organization_UID=M.FunPlant_Organization_UID,
                            Sequence = M.Sequence,
                            Type_Name = M.Type_Name,
                            Is_Enable = M.Is_Enable,
                            Modify_UID = M.Modify_UID,
                            Modify_Date = M.Modify_Date,
                            Plant_Organization_Name = M.System_Organization2.Organization_Name,
                           Type_Code=M.Type_Code,
                            Modifyer = M.System_Users.User_Name,
                            OP_Organization_Name=M.System_Organization.Organization_Name,
                            FuncPlant_Organization_Name=M.System_Organization1.Organization_Name
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0 && searchModel.BG_Organization_UID !=null)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0 && searchModel.FunPlant_Organization_UID !=null)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);

            if (!string.IsNullOrWhiteSpace(searchModel.Type_Name))
                query = query.Where(m => m.Type_Name == searchModel.Type_Name);
            if (!string.IsNullOrWhiteSpace(searchModel.Type_Code))
                query = query.Where(m => m.Type_Name == searchModel.Type_Code);
            if (!string.IsNullOrWhiteSpace(searchModel.Modifyer))
                query = query.Where(m => m.Modifyer == searchModel.Modifyer);    
            if (searchModel.IsEnabled != null)
                query = query.Where(m => m.Is_Enable == searchModel.IsEnabled);

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modify_Date).GetPage(page);
            return query;
        }

      
        public OEE_DownTypeDTO QueryDownTypeByUid(int OEE_DownTimeType_UID)
        {
            var query = from M in DataContext.OEE_DownTimeType
                        select new OEE_DownTypeDTO
                        {
                            OEE_DownTimeType_UID = M.OEE_DownTimeType_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Type_Name = M.Type_Name,
                            Is_Enable = M.Is_Enable,
                            Modify_UID = M.Modify_UID,
                            Modify_Date = M.Modify_Date,
                            Plant_Organization_Name = M.System_Organization.Organization_Name,
                            Sequence=M.Sequence,
                            Modifyer = M.System_Users.User_Name,
                            Type_Code=M.Type_Code,
                            OP_Organization_Name = M.System_Organization1.Organization_Name,
                            FuncPlant_Organization_Name = M.System_Organization2.Organization_Name
                        };

            query = query.Where(m => m.OEE_DownTimeType_UID == OEE_DownTimeType_UID);
            return query.FirstOrDefault();

        }

        
    }
}
