using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using PDMS.Model.EntityDTO;
using System.Data.Entity.SqlServer;
using PDMS.Data.Repository.APP;
using PDMS.Model.EntityDTO.APP;

namespace PDMS.Service
{
    public interface IAPPService
    {
        PagedListModel<EquipmentReport> QueryEquipmentInfoReprot(EquipmentReport searchModel, Page page);
        string addUserFav(userFav UserFav);
    }
    public class APPService: IAPPService
    {
        private readonly IEquipmentInfoRepository equipmentInfoRepository;
        private readonly ISystemProjectRepository systemProjectRepository;
        private readonly IAPP_USER_FAVORITES_FUNCTIONRepository APP_USER_FAVORITES_FUNCTIONRepository;
        private readonly IUnitOfWork unitOfWork;

        public APPService(IEquipmentInfoRepository equipmentInfoRepository,
             ISystemProjectRepository systemProjectRepository,
             IAPP_USER_FAVORITES_FUNCTIONRepository APP_USER_FAVORITES_FUNCTIONRepository,
             IUnitOfWork unitOfWork)
        {
            this.equipmentInfoRepository = equipmentInfoRepository;
            this.systemProjectRepository = systemProjectRepository;
            this.APP_USER_FAVORITES_FUNCTIONRepository = APP_USER_FAVORITES_FUNCTIONRepository;
            this.unitOfWork = unitOfWork;
        }

        public PagedListModel<EquipmentReport> QueryEquipmentInfoReprot(EquipmentReport searchModel, Page page)
        {
            var totalcount = 0;
            List<int> organization_UIDs = new List<int>();

            var userOp = APP_USER_FAVORITES_FUNCTIONRepository.getUserOp(searchModel.ntid);
            if (userOp != null)
                searchModel.OPType_OrganizationUID = userOp.Organization_UID;
            var result = equipmentInfoRepository.GetEquipmentInfoReprot(searchModel, organization_UIDs, page, out totalcount);
            return new PagedListModel<EquipmentReport>(totalcount, result);
        }

        public string addUserFav(userFav UserFav)
        {
            var hasItem = APP_USER_FAVORITES_FUNCTIONRepository.GetMany(m => m.Account_UID == UserFav.Account_UID &
                                    m.Function_UID == UserFav.Function_UID).FirstOrDefault();
            if (hasItem == null)
            {
                APP_USER_FAVORITES_FUNCTION AUFF = new APP_USER_FAVORITES_FUNCTION();
                AUFF.APP_USER_FAVORITES_FUNCTION_UID = Guid.NewGuid().ToString();
                AUFF.Account_UID = UserFav.Account_UID;
                AUFF.Function_UID = UserFav.Function_UID;
                AUFF.WEIGHT = 1;
                AUFF.MODIFY_DATE = DateTime.Now;
                APP_USER_FAVORITES_FUNCTIONRepository.Add(AUFF);

            }
            else
            {
                APP_USER_FAVORITES_FUNCTIONRepository.Delete(hasItem);
            }
            unitOfWork.Commit();
            return "";
        }
    }
}
