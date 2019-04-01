using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels.Batch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface ISystemEmailDeliveryRepository : IRepository<System_Email_Delivery>
    {
        List<SystemModuleEmailVM> QueryBatchEmailSetting(SystemModuleEmailVM searchVM, Page page, out int totalcount);
        SystemModuleEmailVM QueryBatchEmailSettingByEdit(int System_Email_Delivery_UID);
        List<SystemModuleEmailFunctionVM> GetBatchEmailSettingFunction(bool isAdmin, int plantUID);
        string SaveBatchEmailSetting(SystemModuleEmailVM item);
        string CheckEmailIsError(SystemModuleEmailVM item);
    }

    public class SystemEmailDeliveryRepository : RepositoryBase<System_Email_Delivery>, ISystemEmailDeliveryRepository
    {
        public SystemEmailDeliveryRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<SystemModuleEmailVM> QueryBatchEmailSetting(SystemModuleEmailVM model, Page page, out int totalcount)
        {
            var linq = from A in DataContext.System_Email_Delivery

                       join B in DataContext.System_Schedule
                       on A.System_Schedule_UID equals B.System_Schedule_UID

                       join C in DataContext.System_Function
                       on B.Function_UID equals C.Function_UID

                       //join D in DataContext.System_Role_Function
                       //on C.Function_UID equals D.Function_UID into CDTemp
                       //from CD in CDTemp.DefaultIfEmpty()


                       join H in DataContext.System_Organization
                       on A.Plant_Organization_UID equals H.Organization_UID

                       join I in DataContext.System_Organization
                       on A.BG_Organization_UID equals I.Organization_UID into BJTemp
                       from BJ in BJTemp.DefaultIfEmpty()

                       join J in DataContext.System_Users
                       on A.Account_UID equals J.Account_UID into AJTemp
                       from AJ in AJTemp.DefaultIfEmpty()

                       select new SystemModuleEmailVM
                       {
                           System_Email_Delivery_UID = A.System_Email_Delivery_UID,
                           System_Schedule_UID = B.System_Schedule_UID,
                           Plant_Organization_UID = A.Plant_Organization_UID,
                           BG_Organization_UID = A.BG_Organization_UID,
                           PlantName = H.Organization_Name,
                           OpType_Name = BJ.Organization_Name,
                           Module_ID = B.System_Module.System_Function.Function_ID,
                           Module_Name = B.System_Module.System_Function.Function_Name,
                           Function_UID = B.Function_UID,
                           Function_ID = C.Function_ID,
                           User_NTID = AJ.User_NTID,
                           Function_Name = C.Function_Name,
                           User_Name_CN = A.User_Name_CN,
                           User_Name_EN = A.User_Name_EN,
                           Email = A.Email,
                           Is_Enable = A.Is_Enable
                       };

            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != null && model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Value.Equals(model.BG_Organization_UID.Value));
            }
            if (model.System_Schedule_UID != 0)
            {
                linq = linq.Where(m => m.System_Schedule_UID == model.System_Schedule_UID);
            }

            //IsAdmin判断
            if (!model.IsAdmin)
            {
                var roleFunctionUIDList = DataContext.System_Role_Function.Where(m => model.RoleUIDList.Contains(m.Role_UID)).Select(m => m.Function_UID).ToList();
                linq = linq.Where(m => roleFunctionUIDList.Contains(m.Function_UID));
            }

            if (model.System_Email_Delivery_UID != 0)
            {
                linq = linq.Where(m => m.System_Email_Delivery_UID == model.System_Email_Delivery_UID);
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                linq = linq.Where(m => m.Email.Contains(model.Email));
            }

            var list = linq.ToList();

            totalcount = list.Count();

            return list.Skip(page.Skip).Take(page.PageSize).ToList();
        }

        public SystemModuleEmailVM QueryBatchEmailSettingByEdit(int System_Email_Delivery_UID)
        {
            var linq = from A in DataContext.System_Email_Delivery

                       join B in DataContext.System_Schedule
                       on A.System_Schedule_UID equals B.System_Schedule_UID

                       join C in DataContext.System_Function
                       on B.Function_UID equals C.Function_UID

                       join H in DataContext.System_Organization
                       on A.Plant_Organization_UID equals H.Organization_UID

                       join I in DataContext.System_Organization
                       on A.BG_Organization_UID equals I.Organization_UID into BJTemp
                       from BJ in BJTemp.DefaultIfEmpty()

                       join J in DataContext.System_Users
                       on A.Account_UID equals J.Account_UID into AJTemp
                       from AJ in AJTemp.DefaultIfEmpty()

                       where A.System_Email_Delivery_UID == System_Email_Delivery_UID

                       select new SystemModuleEmailVM
                       {
                           System_Email_Delivery_UID = A.System_Email_Delivery_UID,
                           Plant_Organization_UID = A.Plant_Organization_UID,
                           BG_Organization_UID = A.BG_Organization_UID,
                           PlantName = H.Organization_Name,
                           OpType_Name = BJ.Organization_Name,
                           System_Schedule_UID = B.System_Schedule_UID,
                           Function_UID = B.Function_UID,
                           Function_ID = C.Function_ID,
                           User_NTID = AJ.User_NTID,
                           Function_Name = C.Function_Name,
                           User_Name_CN = A.User_Name_CN,
                           User_Name_EN = A.User_Name_EN,
                           Email = A.Email,
                           Is_Enable = A.Is_Enable
                       };


            var item = linq.First();
            return item;
        }

        public List<SystemModuleEmailFunctionVM> GetBatchEmailSettingFunction(bool isAdmin, int plantUID)
        {
            var uidList = new List<int>();
            var linq = from A in DataContext.System_Schedule

                       join B in DataContext.System_Function
                       on A.Function_UID equals B.Function_UID

                       join C in DataContext.System_Organization
                       on A.Plant_Organization_UID equals C.Organization_UID

                       where !B.Function_Name.Contains("Mail") && A.Is_Email == true
                       select new SystemModuleEmailFunctionVM
                       {
                           Plant_Organization_UID = A.Plant_Organization_UID,
                           Plant_Name = C.Organization_Name,
                           Function_UID = B.Function_UID,
                           Function_ID = B.Function_ID,
                           Function_Name = B.Function_Name,
                           System_Schedule_UID = A.System_Schedule_UID
                       };


            if (!isAdmin)
            {
                linq = linq.Where(m => m.Plant_Organization_UID == plantUID);
            }
            return linq.ToList();

            //DataContext.System_Schedule.Select(m => m.Function_UID).Distinct().ToList();


            //var linq = from A in DataContext.System_Function

            //           join B in DataContext.System_Schedule
            //           on A.Function_UID equals B.Function_UID

            //           join C in DataContext.System_Organization
            //           on B.Plant_Organization_UID equals C.Organization_UID

            //           where uidList.Contains(A.Function_UID)
            //           select new SystemModuleEmailFunctionVM
            //           {
            //               Plant_Name = C.Organization_Name,
            //               Function_UID = A.Function_UID,
            //               Function_ID = A.Function_ID,
            //               Function_Name = A.Function_Name,
            //               System_Schedule_UID = B.System_Schedule_UID
            //           };
            //var list = linq.ToList();
            //return list;
        }

        public string CheckEmailIsError(SystemModuleEmailVM item)
        {
            string errorInfo = string.Empty;
            //检查邮箱所属是否跟Site一致
            var userInfo = DataContext.System_Users.Where(m => m.Email.ToUpper() == item.Email.ToUpper()).FirstOrDefault();
            if (userInfo != null)
            {
                var currentItem = userInfo.System_UserOrg.FirstOrDefault();
                if (currentItem == null)
                {
                    //errorInfo = "该邮箱所属的用户没有设定厂区";
                    //return errorInfo;
                }
                else
                {
                    if (item.Plant_Organization_UID != currentItem.Plant_OrganizationUID)
                    {
                        errorInfo = "用于所属邮箱不属于当前厂区";
                        return errorInfo;
                    }
                }
                    

            }
            else
            {
                if (!item.Email.ToUpper().Contains("JABIL"))
                {
                    errorInfo = "输入的邮箱不是Jabil邮箱";
                    return errorInfo;
                }
            }

            if (item.Plant_Organization_UID == 0)
            {
                errorInfo = "厂区不能为空";
                return errorInfo;
            }

            //判断是否重复
            if (item.IsEdit)
            {
                var existItem = DataContext.System_Email_Delivery.Where(m => m.System_Email_Delivery_UID != item.System_Email_Delivery_UID
                && m.Plant_Organization_UID == item.Plant_Organization_UID && m.System_Schedule_UID == item.System_Schedule_UID && m.Email == item.Email).FirstOrDefault();
                if (existItem != null)
                {
                    errorInfo = "已经存在相同的邮箱";
                    return errorInfo;
                }
            }
            else
            {
                var existItem = DataContext.System_Email_Delivery.Where(m => m.Plant_Organization_UID == item.Plant_Organization_UID 
                && m.System_Schedule_UID == item.System_Schedule_UID && m.Email == item.Email).FirstOrDefault();
                if (existItem != null)
                {
                    errorInfo = "已经存在相同的邮箱";
                    return errorInfo;
                }
            }
            return errorInfo;
        }

        public string SaveBatchEmailSetting(SystemModuleEmailVM item)
        {
            var linq = from A in DataContext.System_Email_Delivery
                       select new SystemModuleEmailVM
                       {
                           System_Email_Delivery_UID = A.System_Email_Delivery_UID,
                           Plant_Organization_UID = A.Plant_Organization_UID,
                           BG_Organization_UID = A.BG_Organization_UID,
                           System_Schedule_UID = A.System_Schedule_UID,
                           Email = A.Email
                       };
            if (item.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID == item.Plant_Organization_UID);
            }
            if (item.BG_Organization_UID != null)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Value == item.BG_Organization_UID.Value);
            }

            if (item.IsEdit)
            {
                var existItem = DataContext.System_Email_Delivery.Where(m => m.System_Email_Delivery_UID == item.System_Email_Delivery_UID).First();
                existItem.System_Schedule_UID = item.System_Schedule_UID;
                if (item.Account_UID > 0)
                {
                    existItem.Account_UID = item.Account_UID;
                }
                else
                {
                    existItem.Account_UID = null;
                }
                existItem.Plant_Organization_UID = item.Plant_Organization_UID;
                existItem.BG_Organization_UID = item.BG_Organization_UID;
                existItem.User_Name_CN = item.User_Name_CN;
                existItem.User_Name_EN = item.User_Name_EN;
                existItem.Email = item.Email;
                existItem.Is_Enable = item.Is_Enable;
                existItem.Modified_Date = DateTime.Now;
                existItem.Modified_UID = item.Modified_UID;
            }
            else
            {
                System_Email_Delivery newItem = new System_Email_Delivery();
                newItem.Plant_Organization_UID = item.Plant_Organization_UID;
                if (item.BG_Organization_UID != null)
                {
                    newItem.BG_Organization_UID = item.BG_Organization_UID;
                }
                newItem.System_Schedule_UID = item.System_Schedule_UID;
                if (item.Account_UID > 0)
                {
                    newItem.Account_UID = item.Account_UID;
                }
                else
                {
                    newItem.Account_UID = null;
                }
                newItem.User_Name_CN = item.User_Name_CN;
                newItem.User_Name_EN = item.User_Name_EN;
                newItem.Email = item.Email;
                newItem.Is_Enable = item.Is_Enable;
                newItem.Created_UID = item.Created_UID;
                newItem.Created_Date = DateTime.Now;
                newItem.Modified_UID = item.Modified_UID;
                newItem.Modified_Date = DateTime.Now;
                DataContext.System_Email_Delivery.Add(newItem);
                //}
            }
            try
            {
                DataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return "已经存在相同的Email了，不能保存";
            }
            return string.Empty;
        }
    }
}
