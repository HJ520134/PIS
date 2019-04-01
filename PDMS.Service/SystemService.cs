using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using System.Collections.Generic;
using PDMS.Common.Enums;
using System;
using System.Linq;

namespace PDMS.Service
{
    public interface ISystemService
    {
        #region define System interface ---------------------Modify by Wesley 2015/12/01
        IEnumerable<System_Function> GetFunctionsByUserUId(int uid, int LanguageId);
        IEnumerable<MobileSystemMenu> GetMobileFunctionsByUserUId(int uid);
        bool HasActionQulification(int uid, string url);
        EnumAuthorize HasPageQulification(string controller, string action);
        IEnumerable<PageUnauthorizedElement> GetUnauthorizedElementsByNTId(string ntid);
        System_Users GetSystemUserByUId(int uid);
        IEnumerable<SystemFunctionDTO> GetSystemValidFunctions();
        void updateLastLoginDate(int UID);
        #endregion //define System interface
    }

    public class SystemService : ISystemService
    {
        #region Private interfaces properties
        private readonly IUnitOfWork unitOfWork;
        private readonly ISystemFunctionRepository systemFunctionRepository;
        private readonly ISystemUserRepository systemUserRepository;
        private readonly ISystemBUMRepository systemBUMRepository;
        private readonly ISystemPlantRepository systemPlantRepository;
        #endregion //Private interfaces properties

        #region Service constructor
        public SystemService(
            ISystemFunctionRepository systemFunctionRepository,
            ISystemUserRepository systemUserRepository,
            ISystemBUMRepository systemBUMRepository,
            ISystemPlantRepository systemPlantRepository,
            IUnitOfWork unitOfWork)
        {
            this.systemFunctionRepository = systemFunctionRepository;
            this.systemUserRepository = systemUserRepository;
            this.systemBUMRepository = systemBUMRepository;
            this.systemPlantRepository = systemPlantRepository;
            this.unitOfWork = unitOfWork;
        }
        #endregion //Service constructor

        #region System Module
        /// <summary>
        /// 获取用户拥有权限的菜单
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public IEnumerable<System_Function> GetFunctionsByUserUId(int uid, int LanguageId)
        {
            return systemFunctionRepository.GetFunctionsByUserUId(uid, LanguageId);
        }

        /// <summary>
        /// 获取Mobile用户拥有权限的菜单
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public IEnumerable<MobileSystemMenu> GetMobileFunctionsByUserUId(int uid)
        {
            var menus = systemFunctionRepository.GetMobileFunctionsByUserUId(uid).ToArray();
            var rootMenus = menus.Where(q => q.Parent_Function_UID == null);
            IList<MobileSystemMenu> orderedMenus = new List<MobileSystemMenu>();
            if (rootMenus.Count() > 0)
            {
                rootMenus = rootMenus.OrderBy(k => k.Order_Index);
                foreach (var root in rootMenus)
                {
                    var key = new MobileMenuItem
                    {
                        Function_UID = root.Function_UID,
                        Function_ID = root.Function_ID,
                        title = root.Function_Name.Replace(' ', '-'),
                        icon = root.Icon_ClassName,
                        onExecute = root.Mobile_URL
                    };


                    IList<MobileMenuItem> orderedItemMenus =null;
                    #region Menu Title
                    var titleMenus = menus.Where(q => q.Parent_Function_UID == root.Function_UID);
                    if (titleMenus.Count() > 0)
                    {
                        orderedItemMenus = new List<MobileMenuItem>();
                        titleMenus = titleMenus.OrderBy(k => k.Order_Index);
                        foreach (var title in titleMenus)
                        {
                            var itemTitle = new MobileMenuItem
                            {
                                Function_UID = title.Function_UID,
                                Function_ID = title.Function_ID,
                                title = title.Function_Name.Replace(' ', '-'),
                                icon = "fa icon-bar-menu text-gp",
                                onExecute = title.Mobile_URL,
                                visible = true,
                                disabled = true
                            };
                            orderedItemMenus.Add(itemTitle);
                            #region Menu Items
                            var itemMenus = menus.Where(q => q.Parent_Function_UID == title.Function_UID);
                            if (itemMenus.Count() > 0)
                            {
                                itemMenus = itemMenus.OrderBy(k => k.Order_Index);
                                foreach (var item in itemMenus)
                                {
                                    var itemMenu = new MobileMenuItem
                                    {
                                        Function_UID = item.Function_UID,
                                        Function_ID = item.Function_ID,
                                        title = item.Function_Name.Replace(' ', '-'),
                                        icon = "na",
                                        onExecute = item.Mobile_URL,
                                        visible = true,
                                        disabled = false
                                    };

                                    orderedItemMenus.Add(itemMenu);
                                }

                            }
                            #endregion
                        }
                    }
                    #endregion
                    
                    orderedMenus.Add(new MobileSystemMenu { key = key,items = orderedItemMenus??orderedItemMenus.AsEnumerable()});
                }

            }
            return orderedMenus;
        }

        public bool HasActionQulification(int uid, string url)
        {
            return systemFunctionRepository.HasActionQulification(uid, url);
        }
        public void updateLastLoginDate(int UID)
        {
            var user = systemUserRepository.GetById(UID);
            if (user!=null)
            {
                user.LastLoginDate = DateTime.Now;
                try
                {
                    unitOfWork.Commit();
                }
                catch (Exception e)
                {

                }
            }
        }
        public IEnumerable<PageUnauthorizedElement> GetUnauthorizedElementsByNTId(string ntid)
        {
            return systemFunctionRepository.GetUnauthorizedElementsByNTId(ntid);
        }

        public EnumAuthorize HasPageQulification(string controller, string action)
        {
            var url = string.Format("{0}/{1}", controller.ToUpper(), action.ToUpper());
            var menu = systemFunctionRepository.GetFirstOrDefault(q => q.URL == url);

            if (menu == null)
            {
                return EnumAuthorize.NotPageRequest;
            }
            else
            {
                if (menu.Is_Show)
                {
                    return EnumAuthorize.PageAuthorized;
                }
                else
                {
                    return EnumAuthorize.PageNotAuthorized;
                }
            }
        }

        public System_Users GetSystemUserByUId(int uid)
        {
            return systemUserRepository.GetById(uid);
        }

        public IEnumerable<SystemFunctionDTO> GetSystemValidFunctions()
        {
            var dtoList = systemFunctionRepository.GetFunctionLanguageList();
            return dtoList;
            //return AutoMapper.Mapper.Map<IEnumerable<SystemFunctionDTO>>(systemFunctionRepository.GetMany(q => q.URL != null));
        }
        #endregion //System Module
    }
}
