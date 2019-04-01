using PDMS.Data.Infrastructure;
using PDMS.Model;
using System.Linq;
using PDMS.Common.Helpers;
using System;
using System.Collections;
using System.Data.Entity;
using PDMS.Data;
using PDMS.Model.ViewModels;

using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using PDMS.Common.Constants;
using System.Text;
using PDMS.Model.EntityDTO;

namespace PDMS.Data.Repository
{
    public class ProductInputRepository : RepositoryBase<Product_Input>, IProductInputRepository
    {
        private Logger log = new Logger("ProductInputRepository");
        #region ProductInput----------------------------------Justin 2015/12/21
        public ProductInputRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
        /// <summary>
        /// 根据登陆UID找到对应功能厂名
        /// </summary>
        /// <returns></returns>
        public string GetCurrentPlantName(int uid)
        {
            var query = from Fun in DataContext.System_Function_Plant
                        join User in DataContext.System_User_FunPlant on Fun.System_FunPlant_UID equals User.System_FunPlant_UID
                        where (User.Account_UID == uid)
                        select Fun.FunPlant;
            var funcPlantName = query.FirstOrDefault();
            return funcPlantName;
        }

        /// <summary>
        /// 根据功能厂名，查询功能厂对象信息
        /// </summary>
        /// <param name="funcPlant"></param>
        /// <returns></returns>
        public System_Function_Plant QueryFuncPlantInfo(string funcPlant)
        {

            var query = from Fun in DataContext.System_Function_Plant

                        where Fun.FunPlant == funcPlant
                        select Fun;


            return query.FirstOrDefault();
        }


        /// <summary>
        /// Product_Input主界面，Load新数据
        /// </summary>
        public List<ProductDataVM> QueryProcessData_Input(ProcessDataSearch search, List<string> currentProject, Page page, out int count)
        {
            #region 测试下面的程式是否能返回数据的等效SQL
            #endregion
            var query = from bu in DataContext.System_BU_D
                        join p in DataContext.System_Project on bu.BU_D_UID equals p.BU_D_UID
                        join master in DataContext.FlowChart_Master on p.Project_UID equals master.Project_UID
                        join deteil in DataContext.FlowChart_Detail on new { master.FlowChart_Master_UID, master.FlowChart_Version } equals new { deteil.FlowChart_Master_UID, deteil.FlowChart_Version }
                        join fPlant in DataContext.System_Function_Plant on deteil.System_FunPlant_UID equals fPlant.System_FunPlant_UID
                        join mgdata in DataContext.FlowChart_MgData on deteil.FlowChart_Detail_UID equals mgdata.FlowChart_Detail_UID
                        join pcbom in DataContext.FlowChart_PC_MH_Relationship on deteil.FlowChart_Detail_UID equals pcbom.FlowChart_Detail_UID
                        where (bu.BU_D_Name == search.Customer && p.Project_Name == search.Project && p.Product_Phase == search.Product_Phase && master.Part_Types == search.Part_Types

                        //&& ((pcbom.MH_UID == search.Account_UID && userRoleId) || (fPlant.FunPlant == search.Func_Plant && !userRoleId))
                        && (pcbom.MH_UID == search.Account_UID)
                        && mgdata.Product_Date == search.Date)
                        select new ProductDataVM
                        {
                            Location_Flag = deteil.Location_Flag,
                            Is_Comfirm = false,
                            Product_Date = search.Date,
                            Time_Interval = search.Time,
                            Customer = search.Customer,
                            Project = search.Project,
                            Part_Types = search.Part_Types,
                            FunPlant = fPlant.FunPlant,
                            FunPlant_Manager = fPlant.FunPlant_Manager,
                            Product_Phase = search.Product_Phase,
                            Process_Seq = deteil.Process_Seq,
                            Place = deteil.Place,
                            Process = deteil.Process,
                            FlowChart_Master_UID = master.FlowChart_Master_UID,
                            FlowChart_Version = master.FlowChart_Version,
                            Color = deteil.Color,
                            Prouct_Plan = mgdata.Product_Plan,
                            Product_Stage = deteil.Product_Stage,
                            Target_Yield = mgdata.Target_Yield,
                            Good_QTY = 0,
                            Picking_QTY = 0,
                            WH_Picking_QTY = 0,
                            NG_QTY = 0,
                            WH_QTY = 0,
                            WIP_QTY = deteil.WIP_QTY,
                            Adjust_QTY = 0,
                            DRI = deteil.DRI,
                            Material_No = deteil.Material_No,
                            FlowChart_Detail_UID = deteil.FlowChart_Detail_UID,
                            Rework_Flag = deteil.Rework_Flag,
                            Rework_QTY = 0,
                            Normal_Good_QTY = 0,
                            Abnormal_Good_QTY = 0,
                            Normal_NG_QTY = 0,
                            Abnormal_NG_QTY = 0,
                            NullWip_QTY = deteil.NullWip,
                            Current_WH_QTY = deteil.Current_WH_QTY,
                            RelatedRepairUID = deteil.RelatedRepairUID
                        };
            //当前用户所属的专案
            query = query.Where(m => currentProject.Contains(m.Project));

            //var temp = query.Where(x => string.IsNullOrEmpty(x.Color)).OrderBy(o => o.Process_Seq).ToList();
            var AllProcess = query.Where(x => string.IsNullOrEmpty(x.Color)).OrderBy(o => o.Process_Seq).ToList(); ;
            var ColorProcess = query.Where(x => !string.IsNullOrEmpty(x.Color)).ToList();
            ColorProcess = ColorProcess.OrderBy(m => m.Color).ThenBy(m => m.Process_Seq).ToList();
            AllProcess.AddRange(ColorProcess);
            count = AllProcess.Count();
            return AllProcess;
        }



        /// <summary>
        /// Product_Input主界面，Load编辑数据
        /// </summary>
        /// <returns></returns>
        public IQueryable<ProductDataDTO> QueryProductDatas(ProcessDataSearch search, Page page, out int count)
        {
            var query = (from PData in DataContext.Product_Input
                         select new ProductDataDTO()
                         {
                             Is_Comfirm = PData.Is_Comfirm,
                             Product_Date = PData.Product_Date,
                             Time_Interval = PData.Time_Interval,
                             Customer = PData.Customer,
                             Project = PData.Project,
                             Part_Types = PData.Part_Types,
                             FunPlant = PData.FunPlant,
                             FunPlant_Manager = PData.FunPlant_Manager,
                             Product_Phase = PData.Product_Phase,
                             Process_Seq = PData.Process_Seq,
                             Place = PData.Place,
                             Process = PData.Process,
                             FlowChart_Master_UID = PData.FlowChart_Master_UID,
                             FlowChart_Version = PData.FlowChart_Version,
                             Color = PData.Color,
                             Prouct_Plan = PData.Prouct_Plan,
                             Product_Stage = PData.Product_Stage,
                             Target_Yield = PData.Target_Yield,
                             Good_QTY = PData.Good_QTY,
                             Picking_QTY = PData.Picking_QTY,
                             WH_Picking_QTY = PData.WH_Picking_QTY,
                             NG_QTY = PData.NG_QTY,
                             WH_QTY = PData.WH_QTY,
                             WIP_QTY = PData.WIP_QTY,
                             NullWip_QTY = PData.NullWip_QTY,
                             Adjust_QTY = PData.Adjust_QTY,
                             Creator_UID = PData.Creator_UID,
                             Create_Date = PData.Create_Date,
                             Material_No = PData.Material_No,
                             Modified_UID = PData.Modified_UID,
                             Modified_Date = PData.Modified_Date,
                             Good_MismatchFlag = PData.Good_MismatchFlag,
                             Picking_MismatchFlag = PData.Picking_MismatchFlag,
                             Normal_Good_QTY = PData.Normal_Good_QTY,
                             Normal_NG_QTY = PData.Normal_NG_QTY,
                             Abnormal_Good_QTY = PData.Abnormal_Good_QTY,
                             Product_UID = PData.Product_UID,
                             Abnormal_NG_QTY = PData.Abnormal_NG_QTY,
                             FlowChart_Detail_UID = PData.FlowChart_Detail_UID,
                             Location_Flag = false, //表明该数据来源于Product_Input表
                             Unacommpolished_Reason = PData.Unacommpolished_Reason
                         })
                          .Union(
                from item in DataContext.Product_Input_Location
                select new ProductDataDTO()
                {
                    Is_Comfirm = item.Is_Comfirm,
                    Product_Date = item.Product_Date,
                    Time_Interval = item.Time_Interval,
                    Customer = item.Customer,
                    Project = item.Project,
                    Part_Types = item.Part_Types,
                    FunPlant = item.FunPlant,
                    FunPlant_Manager = item.FunPlant_Manager,
                    Product_Phase = item.Product_Phase,
                    Process_Seq = item.Process_Seq,
                    Place = item.Place,
                    Process = item.Process,
                    FlowChart_Master_UID = item.FlowChart_Master_UID,
                    FlowChart_Version = item.FlowChart_Version,
                    Color = item.Color,
                    Prouct_Plan = item.Prouct_Plan,
                    Product_Stage = item.Product_Stage,
                    Target_Yield = item.Target_Yield,
                    Good_QTY = item.Good_QTY,
                    Picking_QTY = item.Picking_QTY,
                    WH_Picking_QTY = item.WH_Picking_QTY,
                    NG_QTY = item.NG_QTY,
                    WH_QTY = item.WH_QTY,
                    WIP_QTY = item.WIP_QTY,
                    NullWip_QTY = item.NullWip_QTY,
                    Adjust_QTY = item.Adjust_QTY,
                    Creator_UID = item.Creator_UID,
                    Create_Date = item.Create_Date,
                    Material_No = item.Material_No,
                    Modified_UID = item.Modified_UID,
                    Modified_Date = item.Modified_Date,
                    Good_MismatchFlag = item.Good_MismatchFlag,
                    Picking_MismatchFlag = item.Picking_MismatchFlag,
                    Normal_Good_QTY = item.Normal_Good_QTY,
                    Normal_NG_QTY = item.Normal_NG_QTY,
                    Abnormal_Good_QTY = item.Abnormal_Good_QTY,
                    Product_UID = item.Product_Input_Location_UID,
                    Abnormal_NG_QTY = item.Abnormal_NG_QTY,
                    FlowChart_Detail_UID = item.FlowChart_Detail_UID,
                    Location_Flag = true,//表明该数据来源于Product_Input_Location表,
                    Unacommpolished_Reason = item.Unacommpolished_Reason
                }
                );
            if (!string.IsNullOrWhiteSpace(search.Customer))
            {
                query = query.Where(p => p.Customer == search.Customer);

            }
            if (!string.IsNullOrWhiteSpace(search.Project))
            {
                query = query.Where(p => p.Project == search.Project);
            }
            if (!string.IsNullOrWhiteSpace(search.Product_Phase))
            {
                query = query.Where(p => p.Product_Phase == search.Product_Phase);

            }
            if (!string.IsNullOrWhiteSpace(search.Part_Types))
            {
                query = query.Where(p => p.Part_Types == search.Part_Types);
            }
            if (!string.IsNullOrWhiteSpace(search.Time))
            {
                query = query.Where(p => p.Time_Interval == search.Time);
            }
            if (search.Date.Year != 1)
            {
                query = query.Where(p => p.Product_Date == search.Date);
            }
            if (search.Account_UID != 0)
            {

                var accountItem = DataContext.System_Users.Find(search.Account_UID);
                if (accountItem.MH_Flag)
                {
                    var detail = from mh in DataContext.FlowChart_PC_MH_Relationship
                                 where mh.MH_UID == search.Account_UID
                                 select mh.FlowChart_Detail_UID;
                    query = query.Where(p => detail.Contains((int)p.FlowChart_Detail_UID));
                }


            }
            //query = query.Where(p => p.FunPlant == search.Func_Plant);
            count = query.Count();
            return query.OrderBy(o => o.Color).ThenBy(o => o.Process_Seq).ThenBy(o => o.Place);
        }


        public IQueryable<Product_Input> QueryProductDataForEmergency(ProcessDataSearchModel search, Page page, out int count)
        {

            var query = from PData in DataContext.Product_Input

                        select PData;
            if (!string.IsNullOrWhiteSpace(search.Customer))
            {
                query = query.Where(p => p.Customer == search.Customer);

            }
            if (!string.IsNullOrWhiteSpace(search.Project))
            {
                query = query.Where(p => p.Project == search.Project);
            }
            if (!string.IsNullOrWhiteSpace(search.Product_Phase))
            {
                query = query.Where(p => p.Product_Phase == search.Product_Phase);

            }
            if (!string.IsNullOrWhiteSpace(search.Part_Types))
            {
                query = query.Where(p => p.Part_Types == search.Part_Types);
            }
            if (!string.IsNullOrWhiteSpace(search.Time))
            {
                query = query.Where(p => p.Time_Interval == search.Time);
            }
            if (search.Date.Year != 1)
            {
                query = query.Where(p => p.Product_Date == search.Date);
            }
            //if (search.Account_UID != 0)
            //{
            //    var userRoleId =
            //    DataContext.System_User_Role.Where(m => m.Account_UID == search.Account_UID)
            //        .Select(m => m.System_Users.MH_Flag)
            //        .FirstOrDefault();
            //    if (userRoleId)
            //    {
            //        var detail = from mh in DataContext.FlowChart_PC_MH_Relationship
            //                     where mh.MH_UID == search.Account_UID
            //                     select mh.FlowChart_Detail_UID;
            //        query = query.Where(p => detail.Contains((int)p.FlowChart_Detail_UID));
            //    }
            //}
            //query = query.Where(p => p.FunPlant == search.Func_Plant);
            count = query.Count();
            return query.OrderBy(o => o.Color).ThenBy(o => o.Process_Seq);
        }
        /// <summary>
        /// 查询制程数据
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<ProductDataDTO> QueryProcessData(ProcessDataSearch search, Page page, out int count)
        {
            var query = from bu in DataContext.System_BU_D
                        join p in DataContext.System_Project on bu.BU_D_UID equals p.BU_D_UID
                        join master in DataContext.FlowChart_Master on p.Project_UID equals master.Project_UID
                        join deteil in DataContext.FlowChart_Detail on new { master.FlowChart_Master_UID, master.FlowChart_Version } equals new { deteil.FlowChart_Master_UID, deteil.FlowChart_Version }
                        join fPlant in DataContext.System_Function_Plant on deteil.System_FunPlant_UID equals fPlant.System_FunPlant_UID
                        join mgdata in DataContext.FlowChart_MgData on deteil.FlowChart_Detail_UID equals mgdata.FlowChart_Detail_UID
                        //join pcbom in DataContext.FlowChart_PC_MH_Relationship on deteil.FlowChart_Detail_UID equals pcbom.FlowChart_Detail_UID

                        where (bu.BU_D_Name == search.Customer && p.Project_Name == search.Project && p.Product_Phase == search.Product_Phase
                        && master.Part_Types == search.Part_Types
                        //&& fPlant.FunPlant == search.Func_Plant 
                        //&& pcbom.MH_UID == search.Account_UID
                        && mgdata.Product_Date == search.Date)
                        select new ProductDataDTO
                        {
                            Location_Flag = deteil.Location_Flag,
                            Is_Comfirm = false,
                            Product_Date = search.Date,
                            Time_Interval = search.Time,
                            Customer = search.Customer,
                            Project = search.Project,
                            Part_Types = search.Part_Types,
                            FunPlant = fPlant.FunPlant,
                            FunPlant_Manager = fPlant.FunPlant_Manager,
                            Product_Phase = search.Product_Phase,
                            Process_Seq = deteil.Process_Seq,
                            Place = deteil.Place,
                            Process = deteil.Process,
                            FlowChart_Master_UID = master.FlowChart_Master_UID,
                            FlowChart_Version = master.FlowChart_Version,
                            Color = deteil.Color,
                            Prouct_Plan = mgdata.Product_Plan,
                            Product_Stage = deteil.Product_Stage,
                            Target_Yield = mgdata.Target_Yield,
                            Good_QTY = 0,
                            Picking_QTY = 0,
                            WH_Picking_QTY = 0,
                            NG_QTY = 0,
                            WH_QTY = 0,
                            WIP_QTY = 0,
                            Adjust_QTY = 0,
                            DRI = deteil.DRI,
                            Material_No = deteil.Material_No,
                            FlowChart_Detail_UID = deteil.FlowChart_Detail_UID,
                            Rework_Flag = deteil.Rework_Flag,
                            NullWip_QTY = deteil.NullWip
                        };

            var AllProcess = query.OrderBy(o => o.Color).ThenBy(o => o.Process_Seq).ToList();
            //var AllProcess = query.Where(x => string.IsNullOrEmpty(x.Color)).OrderBy(o => o.Process_Seq).ToList(); ;
            //var ColorProcess = query.Where(x => !string.IsNullOrEmpty(x.Color)).OrderBy(x => x.Color).ThenBy(x => x.Process_Seq).ToList();
            //AllProcess.AddRange(ColorProcess);

            count = AllProcess.Count();
            return AllProcess;
        }

        public List<ProductDataDTO> QueryProcessDataForEmergency(ProcessDataSearchModel search, Page page, out int count)
        {
            var query = from bu in DataContext.System_BU_D
                        join p in DataContext.System_Project on bu.BU_D_UID equals p.BU_D_UID
                        join master in DataContext.FlowChart_Master on p.Project_UID equals master.Project_UID
                        join deteil in DataContext.FlowChart_Detail on new { master.FlowChart_Master_UID, master.FlowChart_Version } equals new { deteil.FlowChart_Master_UID, deteil.FlowChart_Version }
                        join fPlant in DataContext.System_Function_Plant on deteil.System_FunPlant_UID equals fPlant.System_FunPlant_UID
                        join mgdata in DataContext.FlowChart_MgData on deteil.FlowChart_Detail_UID equals mgdata.FlowChart_Detail_UID
                        //join pcbom in DataContext.FlowChart_PC_MH_Relationship on deteil.FlowChart_Detail_UID equals pcbom.FlowChart_Detail_UID

                        where (bu.BU_D_Name == search.Customer && p.Project_Name == search.Project && p.Product_Phase == search.Product_Phase
                        && master.Part_Types == search.Part_Types
                        //&& fPlant.FunPlant == search.Func_Plant 
                        //&& pcbom.MH_UID == search.Account_UID
                        && mgdata.Product_Date == search.Date)
                        select new ProductDataDTO
                        {
                            Is_Comfirm = false,
                            Product_Date = search.Date,
                            Time_Interval = search.Time,
                            Customer = search.Customer,
                            Project = search.Project,
                            Part_Types = search.Part_Types,
                            FunPlant = fPlant.FunPlant,
                            FunPlant_Manager = fPlant.FunPlant_Manager,
                            Product_Phase = search.Product_Phase,
                            Process_Seq = deteil.Process_Seq,
                            Place = deteil.Place,
                            Process = deteil.Process,
                            FlowChart_Master_UID = master.FlowChart_Master_UID,
                            FlowChart_Version = master.FlowChart_Version,
                            Color = deteil.Color,
                            Prouct_Plan = mgdata.Product_Plan,
                            Product_Stage = deteil.Product_Stage,
                            Target_Yield = mgdata.Target_Yield,
                            Good_QTY = 0,
                            Picking_QTY = 0,
                            WH_Picking_QTY = 0,
                            NG_QTY = 0,
                            WH_QTY = 0,
                            WIP_QTY = 0,
                            Adjust_QTY = 0,
                            DRI = deteil.DRI,
                            Material_No = deteil.Material_No,
                            FlowChart_Detail_UID = deteil.FlowChart_Detail_UID

                        };
            var AllProcess = query.Where(x => string.IsNullOrEmpty(x.Color)).OrderBy(o => o.Process_Seq).ToList(); ;
            var ColorProcess = query.Where(x => !string.IsNullOrEmpty(x.Color)).OrderBy(x => x.Color).ThenBy(x => x.Process_Seq).ToList();
            AllProcess.AddRange(ColorProcess);
            //var result=  (query.Where(x => string.IsNullOrEmpty(x.Color)).OrderBy(o => o.Process_Seq)).Union(
            //      query.Where(x => !string.IsNullOrEmpty(x.Color)).OrderBy(x => x.Color).OrderBy(x => x.Process_Seq));
            count = AllProcess.Count();
            return AllProcess;
        }

        public IQueryable<ProductDataDTO> QueryColorSparations(ProcessDataSearch search)
        {

            var query = from bu in DataContext.System_BU_D
                        join p in DataContext.System_Project on bu.BU_D_UID equals p.BU_D_UID
                        join master in DataContext.FlowChart_Master on p.Project_UID equals master.Project_UID
                        join deteil in DataContext.FlowChart_Detail on new { master.FlowChart_Master_UID, master.FlowChart_Version } equals new { deteil.FlowChart_Master_UID, deteil.FlowChart_Version }
                        join fPlant in DataContext.System_Function_Plant on deteil.System_FunPlant_UID equals fPlant.System_FunPlant_UID
                        join mgdata in DataContext.FlowChart_MgData on deteil.FlowChart_Detail_UID equals mgdata.FlowChart_Detail_UID
                        where (bu.BU_D_Name == search.Customer && p.Project_Name == search.Project && p.Product_Phase == search.Product_Phase && master.Part_Types == search.Part_Types && mgdata.Product_Date == search.Date)
                        select new ProductDataDTO
                        {
                            Is_Comfirm = false,
                            Product_Date = search.Date,
                            Time_Interval = search.Time,
                            Customer = search.Customer,
                            Project = search.Project,
                            Part_Types = search.Part_Types,
                            FunPlant = search.Func_Plant,
                            FunPlant_Manager = fPlant.FunPlant_Manager,
                            Product_Phase = search.Product_Phase,
                            Process_Seq = deteil.Process_Seq,
                            Place = deteil.Place,
                            Process = deteil.Process,
                            FlowChart_Master_UID = master.FlowChart_Master_UID,
                            FlowChart_Version = master.FlowChart_Version,
                            Color = deteil.Color,
                            Prouct_Plan = mgdata.Product_Plan,
                            Product_Stage = deteil.Product_Stage,
                            Target_Yield = mgdata.Target_Yield,
                            Good_QTY = 0,
                            Picking_QTY = 0,
                            WH_Picking_QTY = 0,
                            NG_QTY = 0,
                            WH_QTY = 0,
                            WIP_QTY = 0,
                            Adjust_QTY = 0,
                            Material_No = deteil.Material_No
                        };
            return query.OrderBy(o => o.Process_Seq);

        }
        /// <summary>
        /// 数据提交后执行制程数据判断，调用sp实现
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string ExecAlterSp(Product_Input search)
        {
            //根据search对象获取masterUID

            var FlowChart_Master_UID = new SqlParameter("FlowChart_Master_UID", search.FlowChart_Master_UID);
            var FlowChart_Version = new SqlParameter("FlowChart_Version", search.FlowChart_Version);
            var Time_interval = new SqlParameter("Time_interval", search.Time_Interval);
            var Product_date = new SqlParameter("Product_date", search.Product_Date);
            //var Product_UID = new SqlParameter("Product_UID", search.Product_UID);

            IEnumerable<SPReturnMessage> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<SPReturnMessage>("usp_AlterMisMatchFlag @FlowChart_Master_UID ,@FlowChart_Version, @Time_interval, @Product_date", FlowChart_Master_UID, FlowChart_Version, Time_interval, Product_date).ToArray();

            return "SUCCESS";
        }

        public string ExecAlterMES_PISSp(Product_Input search)
        {
            var FlowChart_Master_UID = new SqlParameter("FlowChart_Master_UID", search.FlowChart_Master_UID);
            var FlowChart_Version = new SqlParameter("FlowChart_Version", search.FlowChart_Version);
            var Time_interval = new SqlParameter("Time_interval", search.Time_Interval);
            var Product_date = new SqlParameter("Product_date", search.Product_Date);
            IEnumerable<SPReturnMessage> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<SPReturnMessage>("usp_AlterMisMatchFlag @FlowChart_Master_UID ,@FlowChart_Version, @Time_interval, @Product_date", FlowChart_Master_UID, FlowChart_Version, Time_interval, Product_date).ToArray();
            return result.ToList()[0].Message;
        }

        /// <summary>
        /// 调用sp 计算当前制程的wip
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public string ExecWIPSp(ProductDataItem search)
        {
            var FlowChart_Master_UID = new SqlParameter("FlowChart_Master_UID", search.FlowChart_Master_UID);
            var Detail_UID = new SqlParameter("FlowChart_Detail_UID", search.FlowChart_Detail_UID);
            var Time_interval = new SqlParameter("Time_interval", search.Time_Interval);
            var Product_date = new SqlParameter("Product_date", search.Product_Date);
            var FunPlant = new SqlParameter("FunPlant", search.FunPlant);
            int id = 0;
            var Product_UID = new SqlParameter("Product_UID", id);
            IEnumerable<SPReturnMessage> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<SPReturnMessage>("usp_CalculateWIP  @Time_Interval,@Product_Date,@FlowChart_Master_UID,@FlowChart_Detail_UID, @FunPlant,@Product_UID", Time_interval, Product_date, FlowChart_Master_UID, Detail_UID, FunPlant, Product_UID).ToArray();
            return result.ToList()[0].Message;
        }

        /// <summary>
        /// 数据更新时候，调用sp计算当前wip
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public string ExecUpdateWIPSp(Product_Input search)
        {
            var FlowChart_Master_UID = new SqlParameter("FlowChart_Master_UID", search.FlowChart_Master_UID);
            var Detail_UID = new SqlParameter("FlowChart_Detail_UID", search.FlowChart_Detail_UID);
            var Time_interval = new SqlParameter("Time_interval", search.Time_Interval);
            var Product_date = new SqlParameter("Product_date", search.Product_Date);
            var FunPlant = new SqlParameter("FunPlant", search.FunPlant);
            var Product_UID = new SqlParameter("Product_UID", search.Product_UID);
            IEnumerable<SPReturnMessage> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<SPReturnMessage>("usp_CalculateWIP  @Time_Interval,@Product_Date,@FlowChart_Master_UID,@FlowChart_Detail_UID, @FunPlant,@Product_UID", Time_interval, Product_date, FlowChart_Master_UID, Detail_UID, FunPlant, Product_UID).ToArray();
            return result.ToList()[0].Message;
        }

        #endregion
        #region 查询Product_Input及Product_Input_Hitory------------------Sidney 2015/12/20
        /// <summary>
        /// SetProductDatas
        /// </summary>
        /// <param name="PDataList"></param>
        /// <returns></returns>
        public List<Product_Input> SetProductDatas(ProductDataList PDataList)
        {
            List<Product_Input> Lists = new List<Product_Input>();
            ProductDataItem search = PDataList.ProductLists[0];
            int count = PDataList.ProductLists.Count;

            var query = from bu in DataContext.System_BU_D
                        join p in DataContext.System_Project on bu.BU_D_UID equals p.BU_D_UID
                        join master in DataContext.FlowChart_Master on p.Project_UID equals master.Project_UID
                        join deteil in DataContext.FlowChart_Detail on master.FlowChart_Master_UID equals deteil.FlowChart_Master_UID
                        join fPlant in DataContext.System_Function_Plant on deteil.System_FunPlant_UID equals fPlant.System_FunPlant_UID
                        join mgdata in DataContext.FlowChart_MgData on deteil.FlowChart_Detail_UID equals mgdata.FlowChart_Detail_UID
                        where (bu.BU_D_Name == search.Customer && p.Project_Name == search.Project && p.Product_Phase == search.Product_Phase && master.Part_Types == search.Part_Types && fPlant.FunPlant == search.FunPlant)
                        select new Product_Input
                        {
                            Is_Comfirm = false,
                            Product_Date = search.Product_Date,
                            Time_Interval = search.Time_Interval,
                            Customer = bu.BU_D_Name,
                            Project = p.Project_Name,
                            Part_Types = master.Part_Types,
                            FunPlant = fPlant.FunPlant,
                            FunPlant_Manager = fPlant.FunPlant_Manager,
                            Product_Phase = p.Product_Phase,
                            Process_Seq = search.Process_Seq,
                            Place = deteil.Place,
                            Process = search.Process,
                            FlowChart_Master_UID = master.FlowChart_Master_UID,
                            FlowChart_Version = master.FlowChart_Version,
                            Color = search.Color,
                            Prouct_Plan = mgdata.Product_Plan,
                            Product_Stage = deteil.Product_Stage,
                            Target_Yield = mgdata.Target_Yield,
                            Good_QTY = search.Good_QTY,
                            Picking_QTY = search.Picking_QTY,
                            WH_Picking_QTY = search.WH_Picking_QTY,
                            NG_QTY = search.NG_QTY,
                            WH_QTY = search.WH_QTY,
                            WIP_QTY = 0,
                            Adjust_QTY = search.Adjust_QTY,
                            Creator_UID = search.Creator_UID,
                            Create_Date = search.Create_Date,
                            Material_No = deteil.Material_No,
                            Modified_UID = search.Modified_UID,
                            Modified_Date = search.Modified_Date
                        };
            Lists = query.ToList();
            return Lists;
        }
        /// <summary>
        /// 检查是否所有功能厂都有数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public List<string> CheckFunPlantDataIsFull(PPCheckDataSearch searchModel)
        {
            //获取所有的功能厂
            var query = from bud in DataContext.System_BU_D
                        join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                        where (bud.BU_D_Name == searchModel.Customer && project.Project_Name == searchModel.Project)
                        select (project.Project_UID);
            var projectUid = query.FirstOrDefault();
            var queryFunPlantQ = from flowmaster in DataContext.FlowChart_Master
                                 join flowdetail in DataContext.FlowChart_Detail on flowmaster.FlowChart_Master_UID equals flowdetail.FlowChart_Master_UID
                                 where (flowmaster.Project_UID == projectUid && flowmaster.Part_Types == searchModel.Part_Types && flowmaster.Is_Closed == false
                                 && flowmaster.FlowChart_Version == flowdetail.FlowChart_Version)
                                 select (flowdetail.System_Function_Plant.FunPlant);
            var queryFunPlant = queryFunPlantQ.Distinct().ToList();
            //获取当前已有数据的功能厂
            var queryNowFunPlantQ = (from inputItem in DataContext.Product_Input_Location
                                     where (inputItem.Customer == searchModel.Customer && inputItem.Project == searchModel.Project
                                            && inputItem.Product_Phase == searchModel.Product_Phase &&
                                            inputItem.Part_Types == searchModel.Part_Types
                                            && DbFunctions.TruncateTime(inputItem.Product_Date) == searchModel.Reference_Date.Date
                                            && inputItem.Time_Interval == searchModel.Tab_Select_Text
                                         )
                                     select (inputItem.FunPlant)).Union
                                  (from inputItem in DataContext.Product_Input
                                   where (inputItem.Customer == searchModel.Customer && inputItem.Project == searchModel.Project
                                          && inputItem.Product_Phase == searchModel.Product_Phase &&
                                          inputItem.Part_Types == searchModel.Part_Types
                                          && DbFunctions.TruncateTime(inputItem.Product_Date) == searchModel.Reference_Date.Date
                                          && inputItem.Time_Interval == searchModel.Tab_Select_Text
                                       )
                                   select (inputItem.FunPlant));
            var queryNowFunPlant = queryNowFunPlantQ.Distinct().ToList();
            List<string> result = queryFunPlant.Except(queryNowFunPlant).ToList();
            return result;

        }

        public List<GetErrorData> CheckProductDataIsFull(PPCheckDataSearch searchModel)
        {
            //获取所有的制程信息
            var query = from bud in DataContext.System_BU_D
                        join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                        where (bud.BU_D_Name == searchModel.Customer && project.Project_Name == searchModel.Project)
                        select (project.Project_UID);
            var projectUid = query.FirstOrDefault();
            var query1 = from master in DataContext.FlowChart_Master
                         where master.Project_UID == projectUid && master.Part_Types == searchModel.Part_Types
                         select new { master.FlowChart_Master_UID, master.FlowChart_Version };
            var AllDetailUid = from detail in DataContext.FlowChart_Detail
                               where detail.FlowChart_Master_UID == query1.FirstOrDefault().FlowChart_Master_UID && detail.FlowChart_Version == query1.FirstOrDefault().FlowChart_Version
                               select detail.FlowChart_Detail_UID;
            //获取已有的制程
            var ExitDetailUid = (from inputItem in DataContext.Product_Input_Location
                                 where (inputItem.FlowChart_Master_UID == query1.FirstOrDefault().FlowChart_Master_UID
                                        && inputItem.FlowChart_Version == query1.FirstOrDefault().FlowChart_Version
                                        && DbFunctions.TruncateTime(inputItem.Product_Date) == searchModel.Reference_Date.Date
                                        && inputItem.Time_Interval == searchModel.Tab_Select_Text
                                     )
                                 select (int)inputItem.FlowChart_Detail_UID).Union(from inputItem in DataContext.Product_Input
                                                                                   where (inputItem.FlowChart_Master_UID == query1.FirstOrDefault().FlowChart_Master_UID
                                                                                          && inputItem.FlowChart_Version == query1.FirstOrDefault().FlowChart_Version
                                                                                          && DbFunctions.TruncateTime(inputItem.Product_Date) == searchModel.Reference_Date.Date
                                                                                          && inputItem.Time_Interval == searchModel.Tab_Select_Text
                                                                                       )
                                                                                   select (int)inputItem.FlowChart_Detail_UID);
            var NotDetailUid = AllDetailUid.Except(ExitDetailUid);
            var result = from detail in DataContext.FlowChart_Detail
                         join i in NotDetailUid on detail.FlowChart_Detail_UID equals i
                         orderby detail.Process_Seq
                         select new GetErrorData
                         {
                             Process_Seq = detail.Process_Seq,
                             Process = detail.Process,
                             Color = detail.Color,
                             Place = detail.Place,
                             FuncPlant = detail.System_Function_Plant.FunPlant,
                             Contact = detail.System_Function_Plant.FunPlant_Contact
                         };
            return result.ToList();
        }

        public IQueryable<PPCheckDataItem> QueryPpCheckDatas(PPCheckDataSearch search, Page page, out int count, string QueryType)
        {

            var Time_Interval = new SqlParameter("Time_Interval", search.Interval_Time);
            var Product_Date = new SqlParameter("Product_Date", DateTime.Now.ToString(FormatConstants.DateTimeFormatStringByDate));
            var Customer = new SqlParameter("Customer", search.Customer);
            var Project = new SqlParameter("Project", search.Project);
            var Product_Phase = new SqlParameter("Product_Phase", search.Product_Phase);
            var Part_Types = new SqlParameter("Part_Types", search.Part_Types);
            var Color = new SqlParameter("Color", search.Color);
            //var Version = new SqlParameter("Version", 4);
            if (search.Tab_Select_Text != "ALL")
            {
                var ShowType = new SqlParameter("ShowType", "NOWDATA");
                IEnumerable<PPCheckDataItem> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<PPCheckDataItem>("usp_PPCheckDataSearch  " +
                "@Time_Interval,@Product_Date,@Customer,@Project, @Product_Phase,@Part_Types,@Color,@ShowType",
                Time_Interval, Product_Date, Customer, Project, Product_Phase, Part_Types, Color, ShowType).ToArray();
                count = result.Count();
                return result.AsQueryable();
            }
            else
            {
                var ShowType = new SqlParameter("ShowType", "ALLDATA");
                IEnumerable<PPCheckDataItem> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<PPCheckDataItem>("usp_PPCheckDataSearch  " +
                "@Time_Interval,@Product_Date,@Customer,@Project, @Product_Phase,@Part_Types,@Color,@ShowType",
                Time_Interval, Product_Date, Customer, Project, Product_Phase, Part_Types, Color, ShowType).ToArray();
                count = result.Count();
                return result.AsQueryable();
            }

        }
        public List<Daily_ProductReportSum> QuerySum_ReportData(NewProductReportSumSearch search, out int count)
        {
            var Time_Interval = new SqlParameter("Time_Interval", search.Tab_Select_Text);
            var Product_Date = new SqlParameter("Product_Date", search.Reference_Date.ToString(FormatConstants.DateTimeFormatStringByDate));
            var OPType = new SqlParameter("OPType", search.OP);
            var Color = new SqlParameter("Color", search.Color);
            var FlowchartMaster_UID = new SqlParameter("FlowchartMaster_UID", search.Flowchart_Master_UID);
            var Flowchart_Version = new SqlParameter("Flowchart_Version", search.input_day_verion);
            var IsColorSum = new SqlParameter("IsColorSum", search.IsColour);
            var Funplant = new SqlParameter("Funplant", search.FunPlant);

            IEnumerable<Daily_ProductReportSum> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<Daily_ProductReportSum>("usp_GetProduct_ReportData @FlowchartMaster_UID,@Flowchart_Version, @Time_Interval,@OPType,@Product_Date,@IsColorSum, @Funplant,@Color",
                FlowchartMaster_UID, Flowchart_Version, Time_Interval, OPType, Product_Date, IsColorSum, Funplant, Color).ToArray();
            count = result.Count();
            return result.ToList();
        }

        public List<Daily_ProductReport> QueryInterval_ReportData(NewProductReportSumSearch search, out int count)
        {
            var Time_Interval = new SqlParameter("Time_Interval", search.Tab_Select_Text);
            var Product_Date = new SqlParameter("Product_Date", search.Reference_Date.ToString(FormatConstants.DateTimeFormatStringByDate));
            var OPType = new SqlParameter("OPType", search.OP);
            var Color = new SqlParameter("Color", search.Color);
            var FlowchartMaster_UID = new SqlParameter("FlowchartMaster_UID", search.Flowchart_Master_UID);
            var Flowchart_Version = new SqlParameter("Flowchart_Version", search.input_day_verion);
            var IsColorSum = new SqlParameter("IsColorSum", search.IsColour);
            var Funplant = new SqlParameter("Funplant", search.FunPlant);
            IEnumerable<Daily_ProductReport> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<Daily_ProductReport>("usp_GetProductInterVal_ReportData @FlowchartMaster_UID,@Flowchart_Version, @Time_Interval,@OPType,@Product_Date,@IsColorSum, @Funplant,@Color",
            FlowchartMaster_UID, Flowchart_Version, Time_Interval, OPType, Product_Date, IsColorSum, Funplant, Color).ToArray();

            count = result.Count();
            return result.ToList();
        }

        /// <summary>
        /// 获取楼栋详情的全天数据
        /// </summary>
        /// <param name="search"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<Daily_ProductReport> GetFullDayInputLocaltion(NewProductReportSumSearch search, out int count)
        {
            var Time_Interval = new SqlParameter("Time_Interval", search.Tab_Select_Text);
            var Product_Date = new SqlParameter("Product_Date", search.Reference_Date.ToString(FormatConstants.DateTimeFormatStringByDate));
            var OPType = new SqlParameter("OPType", search.OP);
            var Color = new SqlParameter("Color", search.Color);
            var FlowchartMaster_UID = new SqlParameter("FlowchartMaster_UID", search.Flowchart_Master_UID);
            var Flowchart_Version = new SqlParameter("Flowchart_Version", search.input_day_verion);
            var IsColorSum = new SqlParameter("IsColorSum", search.IsColour);
            var Funplant = new SqlParameter("Funplant", search.FunPlant);
            IEnumerable<Daily_ProductReport> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<Daily_ProductReport>("usp_GetSumProductLocaltion @FlowchartMaster_UID,@Flowchart_Version, @Time_Interval,@OPType,@Product_Date,@IsColorSum, @Funplant,@Color",
            FlowchartMaster_UID, Flowchart_Version, Time_Interval, OPType, Product_Date, IsColorSum, Funplant, Color).ToArray();

            count = result.Count();
            return result.ToList();
        }

        ////判断ie表中是否有当前所选日期的数据
        //public viod checkDate(ReportDataSearch search, string nowInterval, string nowDate, out int count)
        //{

        //    StringBuilder sql = new StringBuilder();
        //    sql.Append(@"  select count(0) from FlowChart_IEData where IE_TargetDate =search.Reference_Date ");
        //    //int count = DataContext.Database.SqlQuery(sql);
        //    var ret = DataContext.Database.ExecuteSqlCommand(sql.ToString());
        //    count =;
        //    //   return entities;
        //}

        //战情报表-日报查询
        public List<Daily_ProductReportItem> QueryAll_ReportData(ReportDataSearch search, string nowInterval, string nowDate, out int count)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"  select count(0) from FlowChart_IEData where IE_TargetDate =search.Reference_Date ");
            //  var ret = DataContext.Database.ExecuteSqlCommand(sql.ToString());
            var COUNT = DataContext.Database.SqlQuery<int>(sql.ToString());


            var Time_Interval = new SqlParameter("Time_Interval", "Daily_Sum");
            var Shift_Id = new SqlParameter("Time_Interval", search.Tab_Select_Text);
            if (search.Tab_Select_Text == "Daily_Sum")
            {
                Shift_Id = new SqlParameter("Shift_Id", 4);
            }
            if (search.Tab_Select_Text == "Night_Sum")
            {
                Shift_Id = new SqlParameter("Shift_Id", 5);
            }
            if (search.Tab_Select_Text == "ALL")
            {
                Shift_Id = new SqlParameter("Shift_Id", 15);
            }
            if (search.Tab_Select_Text != "ALL" && search.Tab_Select_Text != "Daily_Sum" && search.Tab_Select_Text != "Night_Sum")
            {
                Time_Interval = new SqlParameter("Time_Interval", search.Tab_Select_Text);
                Shift_Id = new SqlParameter("Shift_Id", 15);
            }

            // var Time_Interval = new SqlParameter("Time_Interval", "Daily_Sum");
            var Product_Date = new SqlParameter("Product_Date", search.Reference_Date);
            var Customer = new SqlParameter("Customer", search.Customer);
            var Project = new SqlParameter("Project", search.Project);
            var Product_Phase = new SqlParameter("Product_Phase", search.Product_Phase);
            var Part_Types = new SqlParameter("Part_Types", search.Part_Types);
            var Color = new SqlParameter("Color", search.Color);
            var Now_Interval = new SqlParameter("Now_Interval", nowInterval);

            var Now_Date = new SqlParameter("Now_Date", System.Convert.ToDateTime(nowDate));
            var FLowchart_Version = new SqlParameter("Flowchart_Version", search.input_day_verion);
            var db = ((IObjectContextAdapter)this.DataContext).ObjectContext;
            db.CommandTimeout = 0;
            var result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<Daily_ProductReportItem>("usp_AllProductData1 @Time_Interval,@Product_Date , @Customer, @Project, @Product_Phase, @Part_Types, @Color,@Now_Interval,@Now_Date,@Flowchart_Version,@Shift_Id",
                Time_Interval, Product_Date, Customer, Project, Product_Phase, Part_Types, Color, Now_Interval, Now_Date, FLowchart_Version, Shift_Id).ToList();
            count = result.Count();
            List<Daily_ProductReportItem> list1 = new List<Daily_ProductReportItem>();
            List<Daily_ProductReportItem> result2 = new List<Daily_ProductReportItem>();
            var e = result.GroupBy(item => item.Process).Select(group => new
            {
                sum = group.Sum(item => item.IE_DeptHuman),
                Id = group.Max(item => item.Process)

            }).ToList();
            if (search.Tab_Select_Text == "ALL")
            {
                for (int i = 0; i < result.Count; i++)
                {
                    for (int j = 0; j < e.Count; j++)
                    {
                        if (result[i].Process == e[j].Id)
                        {
                            result[i].IE_DeptHuman = e[j].sum;
                        }
                    }

                }
                //for (int i = 0; i < result.Count - 1; i++)
                //{
                //    if (result[i].Process_Seq == result[i + 1].Process_Seq)
                //    {

                //        var total = result[i].IE_DeptHuman + result[i + 1].IE_DeptHuman;
                //        foreach (var item in result)
                //        {
                //            if (item.Process_Seq == result[i].Process_Seq)
                //            {
                //                item.IE_DeptHuman = total;
                //                //  list1.Add(result.ToList);
                //            }

                //        }
                //    }
                //}




                //result.Where((x, i) => result.FindIndex(z => z.Process_Seq == x.Process_Seq) == i).Select(m => { new Daily_ProductReportItem {   IE_DeptHuman=m.IE_DeptHuman , } );


                var result1 = result.Select(m => new
                {
                    FlowChart_Detail_UID = m.FlowChart_Detail_UID,
                    Product_Input_UID = m.Product_Input_UID,
                    Product_Stage
                   = m.Product_Stage,
                    Is_Comfirm = m.Is_Comfirm,
                    Process_Seq = m.Process_Seq,
                    Color = m.Color,
                    Place = m.Place,
                    FunPlant = m.FunPlant,
                    Process = m.Process,
                    DRI = m.DRI,
                    Target_Yield = m.Target_Yield,
                    All_Product_Plan = m.All_Product_Plan,
                    All_Product_Plan_Sum = m.All_Product_Plan_Sum,

                    All_Picking_QTY = m.All_Picking_QTY,
                    ALL_WH_Picking_QTY = m.ALL_WH_Picking_QTY,
                    All_Good_QTY = m.All_Good_QTY,

                    All_Good_MismatchFlag = m.All_Good_MismatchFlag,

                    All_Adjust_QTY = m.All_Adjust_QTY,

                    All_WH_QTY = m.All_WH_QTY,
                    All_NG_QTY = m.All_NG_QTY,

                    All_Rolling_Yield_Rate = m.All_Rolling_Yield_Rate,
                    All_Finally_Field = m.All_Finally_Field,
                    Product_Plan = m.Product_Plan,

                    IE_TargetEfficacy = m.IE_TargetEfficacy,
                    IE_DeptHuman = m.IE_DeptHuman,
                    ShiftTimeID = m.ShiftTimeID,
                    All_WH_Picking_QTY = m.All_WH_Picking_QTY,
                    Picking_QTY = m.Picking_QTY,
                    Picking_MismatchFlag = m.Picking_MismatchFlag,
                    WH_Picking_QTY = m.WH_Picking_QTY,
                    All_Picking_MismatchFlag = m.All_Picking_MismatchFlag,
                    Good_QTY = m.Good_QTY,
                    Good_MismatchFlag = m.Good_MismatchFlag,
                    WH_QTY = m.WH_QTY,
                    NG_QTY = m.NG_QTY,
                    WIP_QTY = m.WIP_QTY,
                    NullWIP_QTY = m.NullWIP_QTY,
                    OKWIP_QTY = m.OKWIP_QTY,
                    Proper_WIP = m.Proper_WIP,
                    Unacommpolished_Reason = m.Unacommpolished_Reason,
                    Rolling_Yield_Rate = m.Rolling_Yield_Rate,
                    Finally_Field = m.Finally_Field,


                }).Distinct().ToList();


                result2 = result1.Select(m => new Daily_ProductReportItem
                {
                    FlowChart_Detail_UID = m.FlowChart_Detail_UID,
                    Product_Input_UID = m.Product_Input_UID,
                    Product_Stage
               = m.Product_Stage,
                    Is_Comfirm = m.Is_Comfirm,
                    Process_Seq = m.Process_Seq,
                    Color = m.Color,
                    Place = m.Place,
                    FunPlant = m.FunPlant,
                    Process = m.Process,
                    DRI = m.DRI,
                    Target_Yield = m.Target_Yield,
                    All_Product_Plan = m.All_Product_Plan,
                    All_Product_Plan_Sum = m.All_Product_Plan_Sum,

                    All_Picking_QTY = m.All_Picking_QTY,
                    ALL_WH_Picking_QTY = m.ALL_WH_Picking_QTY,
                    All_Good_QTY = m.All_Good_QTY,

                    All_Good_MismatchFlag = m.All_Good_MismatchFlag,

                    All_Adjust_QTY = m.All_Adjust_QTY,

                    All_WH_QTY = m.All_WH_QTY,
                    All_NG_QTY = m.All_NG_QTY,

                    All_Rolling_Yield_Rate = m.All_Rolling_Yield_Rate,
                    All_Finally_Field = m.All_Finally_Field,
                    Product_Plan = m.Product_Plan,

                    IE_TargetEfficacy = m.IE_TargetEfficacy,
                    IE_DeptHuman = m.IE_DeptHuman,
                    ShiftTimeID = m.ShiftTimeID,
                    All_WH_Picking_QTY = m.All_WH_Picking_QTY,
                    Picking_QTY = m.Picking_QTY,
                    Picking_MismatchFlag = m.Picking_MismatchFlag,
                    WH_Picking_QTY = m.WH_Picking_QTY,
                    All_Picking_MismatchFlag = m.All_Picking_MismatchFlag,
                    Good_QTY = m.Good_QTY,
                    Good_MismatchFlag = m.Good_MismatchFlag,
                    WH_QTY = m.WH_QTY,
                    NG_QTY = m.NG_QTY,
                    WIP_QTY = m.WIP_QTY,
                    NullWIP_QTY = m.NullWIP_QTY,
                    OKWIP_QTY = m.OKWIP_QTY,
                    Proper_WIP = m.Proper_WIP,
                    Unacommpolished_Reason = m.Unacommpolished_Reason,
                    Rolling_Yield_Rate = m.Rolling_Yield_Rate,
                    Finally_Field = m.Finally_Field,


                }).Distinct().ToList();

                var temp = from item in result
                           select new Daily_ProductReportItem()
                           {
                               Product_Stage = item.Product_Stage,
                               Process_Seq = item.Process_Seq,
                               Place = item.Place,
                               FunPlant = item.FunPlant,
                               Process = item.Process,
                               Color = item.Color,
                               DRI = item.DRI,

                               IE_TargetEfficacy = item.IE_TargetEfficacy,
                               IE_DeptHuman = item.IE_DeptHuman,

                               Target_Yield = item.Target_Yield,
                               All_Product_Plan = item.All_Product_Plan,
                               All_Product_Plan_Sum = item.All_Product_Plan_Sum,
                               All_Picking_QTY = item.All_Picking_QTY,
                               All_Picking_MismatchFlag = item.All_Picking_MismatchFlag,
                               All_WH_Picking_QTY = item.All_WH_Picking_QTY,
                               All_Good_QTY = item.All_Good_QTY,
                               All_Good_MismatchFlag = item.All_Good_MismatchFlag,
                               All_Adjust_QTY = item.All_Adjust_QTY,
                               All_WH_QTY = item.All_WH_QTY,
                               All_NG_QTY = item.All_NG_QTY,
                               All_Rolling_Yield_Rate = item.All_Rolling_Yield_Rate,
                               All_Finally_Field = item.All_Finally_Field,
                               Product_Plan = 0,
                               Picking_QTY = 0,
                               Picking_MismatchFlag = string.Empty,
                               WH_Picking_QTY = 0,
                               Good_QTY = 0,
                               Good_MismatchFlag = string.Empty,
                               Adjust_QTY = 0,
                               WH_QTY = 0,
                               NG_QTY = 0,
                               Rolling_Yield_Rate = (decimal)0.00,
                               Finally_Field = (decimal)0.00,
                               WIP_QTY = item.WIP_QTY,
                               NullWIP_QTY = item.NullWIP_QTY,
                               Proper_WIP = item.Proper_WIP,
                               OKWIP_QTY = item.OKWIP_QTY,
                               FlowChart_Detail_UID = item.FlowChart_Detail_UID
                           };
            }

            if (search.Tab_Select_Text == "Night_Sum" || search.Tab_Select_Text == "Daily_Sum" || search.Tab_Select_Text == "ALL")
            {
                var temp = from item in result
                           select new Daily_ProductReportItem()
                           {
                               Product_Stage = item.Product_Stage,
                               Process_Seq = item.Process_Seq,
                               Place = item.Place,
                               FunPlant = item.FunPlant,
                               Process = item.Process,
                               Color = item.Color,
                               DRI = item.DRI,

                               IE_TargetEfficacy = item.IE_TargetEfficacy,
                               IE_DeptHuman = item.IE_DeptHuman,

                               Target_Yield = item.Target_Yield,
                               All_Product_Plan = item.All_Product_Plan,
                               All_Product_Plan_Sum = item.All_Product_Plan_Sum,
                               All_Picking_QTY = item.All_Picking_QTY,
                               All_Picking_MismatchFlag = item.All_Picking_MismatchFlag,
                               All_WH_Picking_QTY = item.All_WH_Picking_QTY,
                               All_Good_QTY = item.All_Good_QTY,
                               All_Good_MismatchFlag = item.All_Good_MismatchFlag,
                               All_Adjust_QTY = item.All_Adjust_QTY,
                               All_WH_QTY = item.All_WH_QTY,
                               All_NG_QTY = item.All_NG_QTY,
                               All_Rolling_Yield_Rate = item.All_Rolling_Yield_Rate,
                               All_Finally_Field = item.All_Finally_Field,
                               Product_Plan = 0,
                               Picking_QTY = 0,
                               Picking_MismatchFlag = string.Empty,
                               WH_Picking_QTY = 0,
                               Good_QTY = 0,
                               Good_MismatchFlag = string.Empty,
                               Adjust_QTY = 0,
                               WH_QTY = 0,
                               NG_QTY = 0,
                               Rolling_Yield_Rate = (decimal)0.00,
                               Finally_Field = (decimal)0.00,
                               WIP_QTY = item.WIP_QTY,
                               NullWIP_QTY = item.NullWIP_QTY,
                               Proper_WIP = item.Proper_WIP,
                               OKWIP_QTY = item.OKWIP_QTY,
                               FlowChart_Detail_UID = item.FlowChart_Detail_UID
                           };

                // result = temp.Distinct(new ReportComparer());
            }
            if (search.Building == 2)
            {
                var q = from p in result
                        group p by p.Place into g
                        select new Daily_ProductReportItem()
                        {
                            Product_Stage = g.FirstOrDefault().Product_Stage,
                            Process_Seq = g.FirstOrDefault().Process_Seq,
                            Place = "",
                            FunPlant = g.FirstOrDefault().FunPlant,
                            Process = g.FirstOrDefault().Process,
                            Color = g.FirstOrDefault().Color,
                            DRI = g.FirstOrDefault().DRI,
                            Target_Yield = g.FirstOrDefault().Target_Yield,
                            All_Product_Plan = g.FirstOrDefault().All_Product_Plan,
                            All_Product_Plan_Sum = g.FirstOrDefault().All_Product_Plan_Sum,
                            All_Picking_QTY = g.Sum(t => t.All_Picking_QTY),
                            All_Picking_MismatchFlag = g.FirstOrDefault().All_Picking_MismatchFlag,
                            All_WH_Picking_QTY = g.Sum(t => t.All_WH_Picking_QTY),
                            All_Good_QTY = g.Sum(t => t.All_Good_QTY),
                            All_Good_MismatchFlag = g.FirstOrDefault().All_Good_MismatchFlag,
                            All_Adjust_QTY = g.Sum(t => t.All_Adjust_QTY),
                            All_WH_QTY = g.Sum(t => t.All_WH_QTY),
                            All_NG_QTY = g.Sum(t => t.All_NG_QTY),
                            All_Rolling_Yield_Rate = g.Sum(t => t.All_Rolling_Yield_Rate),
                            All_Finally_Field = g.Sum(t => t.All_Finally_Field),
                            Product_Plan = 0,
                            Picking_QTY = 0,
                            Picking_MismatchFlag = string.Empty,
                            WH_Picking_QTY = 0,
                            Good_QTY = 0,
                            Good_MismatchFlag = string.Empty,
                            Adjust_QTY = 0,
                            WH_QTY = 0,
                            NG_QTY = 0,
                            Rolling_Yield_Rate = (decimal)0.00,
                            Finally_Field = (decimal)0.00,
                            WIP_QTY = g.Sum(t => t.WIP_QTY),
                            NullWIP_QTY = g.Sum(t => t.NullWIP_QTY),
                            OKWIP_QTY = g.Sum(t => t.OKWIP_QTY),
                            FlowChart_Detail_UID = g.FirstOrDefault().FlowChart_Detail_UID
                        };
                return q.ToList();
            }
            else
            {
                if (search.Tab_Select_Text == "ALL")
                {
                    return result2.ToList();
                }
                else
                {
                    return result.ToList();
                }
            }
        }




        //战情报表-日报查询
        public List<Daily_ProductReportItem> QueryAll_ReportData1(ReportDataSearch search, string nowInterval, string nowDate, out int count)
        {


            var Time_Interval = new SqlParameter("Time_Interval", search.Tab_Select_Text);
            var Product_Date = new SqlParameter("Product_Date", search.Reference_Date);
            var Customer = new SqlParameter("Customer", search.Customer);
            var Project = new SqlParameter("Project", search.Project);
            var Product_Phase = new SqlParameter("Product_Phase", search.Product_Phase);
            var Part_Types = new SqlParameter("Part_Types", search.Part_Types);
            var Color = new SqlParameter("Color", search.Color);
            var Now_Interval = new SqlParameter("Now_Interval", nowInterval);
            var Now_Date = new SqlParameter("Now_Date", System.Convert.ToDateTime(nowDate));
            var FLowchart_Version = new SqlParameter("Flowchart_Version", search.input_day_verion);

            var result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<Daily_ProductReportItem>("usp_AllProductData @Time_Interval,@Product_Date , @Customer, @Project, @Product_Phase, @Part_Types, @Color,@Now_Interval,@Now_Date,@Flowchart_Version",
                Time_Interval, Product_Date, Customer, Project, Product_Phase, Part_Types, Color, Now_Interval, Now_Date, FLowchart_Version).ToList();
            count = result.Count();
            var temp = from item in result
                       select new Daily_ProductReportItem()
                       {
                           Product_Stage = item.Product_Stage,
                           Process_Seq = item.Process_Seq,
                           Place = item.Place,
                           FunPlant = item.FunPlant,
                           Process = item.Process,
                           Color = item.Color,
                           DRI = item.DRI,

                           IE_TargetEfficacy = item.IE_TargetEfficacy,
                           IE_DeptHuman = item.IE_DeptHuman,

                           Target_Yield = item.Target_Yield,
                           All_Product_Plan = item.All_Product_Plan,
                           All_Product_Plan_Sum = item.All_Product_Plan_Sum,
                           All_Picking_QTY = item.All_Picking_QTY,
                           All_Picking_MismatchFlag = item.All_Picking_MismatchFlag,
                           All_WH_Picking_QTY = item.All_WH_Picking_QTY,
                           All_Good_QTY = item.All_Good_QTY,
                           All_Good_MismatchFlag = item.All_Good_MismatchFlag,
                           All_Adjust_QTY = item.All_Adjust_QTY,
                           All_WH_QTY = item.All_WH_QTY,
                           All_NG_QTY = item.All_NG_QTY,
                           All_Rolling_Yield_Rate = item.All_Rolling_Yield_Rate,
                           All_Finally_Field = item.All_Finally_Field,
                           Product_Plan = 0,
                           Picking_QTY = 0,
                           Picking_MismatchFlag = string.Empty,
                           WH_Picking_QTY = 0,
                           Good_QTY = 0,
                           Good_MismatchFlag = string.Empty,
                           Adjust_QTY = 0,
                           WH_QTY = 0,
                           NG_QTY = 0,
                           Rolling_Yield_Rate = (decimal)0.00,
                           Finally_Field = (decimal)0.00,
                           WIP_QTY = item.WIP_QTY,
                           NullWIP_QTY = item.NullWIP_QTY,
                           Proper_WIP = item.Proper_WIP,
                           OKWIP_QTY = item.OKWIP_QTY,
                           FlowChart_Detail_UID = item.FlowChart_Detail_UID
                       };


            if (search.Tab_Select_Text == "Night_Sum" || search.Tab_Select_Text == "Daily_Sum" || search.Tab_Select_Text == "ALL")
            {
                temp = from item in result
                       select new Daily_ProductReportItem()
                       {
                           Product_Stage = item.Product_Stage,
                           Process_Seq = item.Process_Seq,
                           Place = item.Place,
                           FunPlant = item.FunPlant,
                           Process = item.Process,
                           Color = item.Color,
                           DRI = item.DRI,

                           IE_TargetEfficacy = item.IE_TargetEfficacy,
                           IE_DeptHuman = item.IE_DeptHuman,

                           Target_Yield = item.Target_Yield,
                           All_Product_Plan = item.All_Product_Plan,
                           All_Product_Plan_Sum = item.All_Product_Plan_Sum,
                           All_Picking_QTY = item.All_Picking_QTY,
                           All_Picking_MismatchFlag = item.All_Picking_MismatchFlag,
                           All_WH_Picking_QTY = item.All_WH_Picking_QTY,
                           All_Good_QTY = item.All_Good_QTY,
                           All_Good_MismatchFlag = item.All_Good_MismatchFlag,
                           All_Adjust_QTY = item.All_Adjust_QTY,
                           All_WH_QTY = item.All_WH_QTY,
                           All_NG_QTY = item.All_NG_QTY,
                           All_Rolling_Yield_Rate = item.All_Rolling_Yield_Rate,
                           All_Finally_Field = item.All_Finally_Field,
                           Product_Plan = 0,
                           Picking_QTY = 0,
                           Picking_MismatchFlag = string.Empty,
                           WH_Picking_QTY = 0,
                           Good_QTY = 0,
                           Good_MismatchFlag = string.Empty,
                           Adjust_QTY = 0,
                           WH_QTY = 0,
                           NG_QTY = 0,
                           Rolling_Yield_Rate = (decimal)0.00,
                           Finally_Field = (decimal)0.00,
                           WIP_QTY = item.WIP_QTY,
                           NullWIP_QTY = item.NullWIP_QTY,
                           Proper_WIP = item.Proper_WIP,
                           OKWIP_QTY = item.OKWIP_QTY,
                           FlowChart_Detail_UID = item.FlowChart_Detail_UID
                       };

                // result = temp.Distinct(new ReportComparer());
            }
            if (search.Building == 2)
            {
                var q = from p in result
                        group p by p.Place into g
                        select new Daily_ProductReportItem()
                        {
                            Product_Stage = g.FirstOrDefault().Product_Stage,
                            Process_Seq = g.FirstOrDefault().Process_Seq,
                            Place = "",
                            FunPlant = g.FirstOrDefault().FunPlant,
                            Process = g.FirstOrDefault().Process,
                            Color = g.FirstOrDefault().Color,
                            DRI = g.FirstOrDefault().DRI,
                            Target_Yield = g.FirstOrDefault().Target_Yield,
                            All_Product_Plan = g.FirstOrDefault().All_Product_Plan,
                            All_Product_Plan_Sum = g.FirstOrDefault().All_Product_Plan_Sum,
                            All_Picking_QTY = g.Sum(t => t.All_Picking_QTY),
                            All_Picking_MismatchFlag = g.FirstOrDefault().All_Picking_MismatchFlag,
                            All_WH_Picking_QTY = g.Sum(t => t.All_WH_Picking_QTY),
                            All_Good_QTY = g.Sum(t => t.All_Good_QTY),
                            All_Good_MismatchFlag = g.FirstOrDefault().All_Good_MismatchFlag,
                            All_Adjust_QTY = g.Sum(t => t.All_Adjust_QTY),
                            All_WH_QTY = g.Sum(t => t.All_WH_QTY),
                            All_NG_QTY = g.Sum(t => t.All_NG_QTY),
                            All_Rolling_Yield_Rate = g.Sum(t => t.All_Rolling_Yield_Rate),
                            All_Finally_Field = g.Sum(t => t.All_Finally_Field),
                            Product_Plan = 0,
                            Picking_QTY = 0,
                            Picking_MismatchFlag = string.Empty,
                            WH_Picking_QTY = 0,
                            Good_QTY = 0,
                            Good_MismatchFlag = string.Empty,
                            Adjust_QTY = 0,
                            WH_QTY = 0,
                            NG_QTY = 0,
                            Rolling_Yield_Rate = (decimal)0.00,
                            Finally_Field = (decimal)0.00,
                            WIP_QTY = g.Sum(t => t.WIP_QTY),
                            NullWIP_QTY = g.Sum(t => t.NullWIP_QTY),
                            OKWIP_QTY = g.Sum(t => t.OKWIP_QTY),
                            FlowChart_Detail_UID = g.FirstOrDefault().FlowChart_Detail_UID
                        };
                return q.ToList();
            }
            else
            {


                return result.ToList();

            }
        }


        public List<Daily_ProductReportItem> QueryAll_ReportDataAPP(ReportDataSearch search, string nowInterval, string nowDate, out int count)
        {
            var Time_Interval = new SqlParameter("Time_Interval", search.Tab_Select_Text);
            var Product_Date = new SqlParameter("Product_Date", search.Reference_Date);
            var Customer = new SqlParameter("Customer", search.Customer);
            var Project = new SqlParameter("Project", search.Project);
            var Product_Phase = new SqlParameter("Product_Phase", search.Product_Phase);
            var Part_Types = new SqlParameter("Part_Types", search.Part_Types);
            var Color = new SqlParameter("Color", search.Color);
            var Now_Interval = new SqlParameter("Now_Interval", nowInterval);
            var Now_Date = new SqlParameter("Now_Date", System.Convert.ToDateTime(nowDate));

            IEnumerable<Daily_ProductReportItem> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<Daily_ProductReportItem>("usp_AllProductData @Time_Interval,@Product_Date , @Customer, @Project, @Product_Phase, @Part_Types, @Color,@Now_Interval,@Now_Date",
                Time_Interval, Product_Date, Customer, Project, Product_Phase, Part_Types, Color, Now_Interval, Now_Date).ToArray();
            count = result.Count();
            if (search.Tab_Select_Text == "Night_Sum" || search.Tab_Select_Text == "Daily_Sum" || search.Tab_Select_Text == "ALL")
            {
                var temp = from item in result
                           select new Daily_ProductReportItem()
                           {
                               Product_Stage = item.Product_Stage,
                               Process_Seq = item.Process_Seq,
                               Place = item.Place,
                               FunPlant = item.FunPlant,
                               Process = item.Process,
                               Color = item.Color,
                               DRI = item.DRI,
                               Target_Yield = item.Target_Yield,
                               All_Product_Plan = item.All_Product_Plan,
                               All_Product_Plan_Sum = item.All_Product_Plan_Sum,
                               All_Picking_QTY = item.All_Picking_QTY,
                               All_Picking_MismatchFlag = item.All_Picking_MismatchFlag,
                               All_WH_Picking_QTY = item.All_WH_Picking_QTY,
                               All_Good_QTY = item.All_Good_QTY,
                               All_Good_MismatchFlag = item.All_Good_MismatchFlag,
                               All_Adjust_QTY = item.All_Adjust_QTY,
                               All_WH_QTY = item.All_WH_QTY,
                               All_NG_QTY = item.All_NG_QTY,
                               All_Rolling_Yield_Rate = item.All_Rolling_Yield_Rate,
                               All_Finally_Field = item.All_Finally_Field,
                               Product_Plan = 0,
                               Picking_QTY = 0,
                               Picking_MismatchFlag = string.Empty,
                               WH_Picking_QTY = 0,
                               Good_QTY = 0,
                               Good_MismatchFlag = string.Empty,
                               Adjust_QTY = 0,
                               WH_QTY = 0,
                               NG_QTY = 0,
                               Rolling_Yield_Rate = (decimal)0.00,
                               Finally_Field = (decimal)0.00,
                               WIP_QTY = item.WIP_QTY
                           };

                result = temp.Distinct(new ReportComparer());
            }
            return result.ToList();
        }
        #endregion
        //新建Compare类
        public class ReportComparer : IEqualityComparer<Daily_ProductReportItem>
        {
            #region IEqualityComparer<User> 成员  
            public bool Equals(Daily_ProductReportItem x, Daily_ProductReportItem y)
            {
                if (x.Product_Plan == y.Product_Plan && x.Picking_QTY == y.Picking_QTY && x.Product_Stage == y.Product_Stage &&
                    x.Process_Seq == y.Process_Seq && x.Place == y.Place && x.FunPlant == y.FunPlant && x.Process == y.Process && x.Color == y.Color &&
                    x.DRI == y.DRI && x.Target_Yield == y.Target_Yield && x.All_Product_Plan == y.All_Product_Plan
                           && x.All_Product_Plan_Sum == y.All_Product_Plan_Sum
                           && x.All_Picking_QTY == y.All_Picking_QTY
                           && x.All_Picking_MismatchFlag == y.All_Picking_MismatchFlag
                           && x.All_WH_Picking_QTY == y.All_WH_Picking_QTY
                           && x.All_Good_QTY == y.All_Good_QTY
                           && x.All_Good_MismatchFlag == y.All_Good_MismatchFlag
                           && x.All_Adjust_QTY == y.All_Adjust_QTY
                           && x.All_WH_QTY == y.All_WH_QTY
                           && x.All_NG_QTY == y.All_NG_QTY
                           && x.All_Rolling_Yield_Rate == y.All_Rolling_Yield_Rate
                           && x.All_Finally_Field == y.All_Finally_Field)
                    return true;
                else
                    return false;
            }

            public int GetHashCode(Daily_ProductReportItem obj)
            {
                return 0;
            }
            #endregion
        }

        //时段查询   2016-12-20 add by karl --------------start 
        public List<TimeSpanReport_2> QueryTimeSpanReport_2(ReportDataSearch searchModel)
        {
            SqlParameter startDate = null;
            SqlParameter endDate = null;
            SqlParameter flowChart_Version = null;
            var color = new SqlParameter("Color", searchModel.Color);
            startDate = new SqlParameter("StartDate", ((DateTime)searchModel.Interval_Date_Start).ToString(FormatConstants.DateTimeFormatStringByDate).Substring(0, 10));
            endDate = new SqlParameter("EndDate", ((DateTime)searchModel.Interval_Date_End).ToString(FormatConstants.DateTimeFormatStringByDate).Substring(0, 10));
            flowChart_Version = new SqlParameter("FlowChart_Version", searchModel.Verion_Interval);
            var funPlant = new SqlParameter("FunPlant", string.IsNullOrEmpty(searchModel.FunPlant) ? "" : searchModel.FunPlant);
            var flowChart_Master_UID = new SqlParameter("Flowchart_Master_UID", GetFlowchartMasterUID(searchModel));
            IEnumerable<TimeSpanReport_2> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<TimeSpanReport_2>("usp_GetTimeSpanReport  @Color,@StartDate,@EndDate,@FlowChart_Version,@Flowchart_Master_UID",
                color, startDate, endDate, flowChart_Version, flowChart_Master_UID).ToArray();
            return result.ToList();
        }


        //时段查询   2016-12-20 add by karl --------------end
        #region 周，时间段 查询 生产表报 -----------------------Destiny 2016/01/31
        public List<TimeSpanReport> QueryTimeSpanReport(ReportDataSearch searchModel)
        {
            SqlParameter startDate = null;
            SqlParameter endDate = null;
            SqlParameter flowChart_Version = null;
            var color = new SqlParameter("Color", searchModel.Color == "ALL" ? "" : searchModel.Color);
            if (searchModel.Select_Type == "monthly") //月查询
            {
                //不管用户输入的是月份的几号，指定从1号开始查询
                DateTime firstDay = new DateTime(searchModel.Month_Date_Start.Value.Year, searchModel.Month_Date_Start.Value.Month, 1);
                DateTime endDay = firstDay.AddMonths(1).AddDays(-1);
                startDate = new SqlParameter("StartDate", firstDay);
                endDate = new SqlParameter("EndDate", endDay);
                flowChart_Version = new SqlParameter("FlowChart_Version", searchModel.Month_Version);

            }
            else //时间段查询
            {
                startDate = new SqlParameter("StartDate", searchModel.Interval_Date_Start);
                endDate = new SqlParameter("EndDate", searchModel.Interval_Date_End);
                flowChart_Version = new SqlParameter("FlowChart_Version", searchModel.Verion_Interval);
            }
            var funPlant = new SqlParameter("FunPlant", string.IsNullOrEmpty(searchModel.FunPlant) ? "" : searchModel.FunPlant);
            var flowChart_Master_UID = new SqlParameter("Flowchart_Master_UID", GetFlowchartMasterUID(searchModel));
            IEnumerable<TimeSpanReport> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<TimeSpanReport>("usp_GetMonthReport  @Color,@StartDate,@EndDate, @FunPlant,@FlowChart_Version,@Flowchart_Master_UID",
                color, startDate, endDate, funPlant, flowChart_Version, flowChart_Master_UID).ToArray();
            return result.ToList();
        }

        public List<ChartDailyReport> QueryChartDailyData(ReportDataSearch searchModel)
        {
            SqlParameter startDate = null;
            SqlParameter endDate = null;
            SqlParameter flowChart_Version = null;
            var color = new SqlParameter("Color", searchModel.Color == "ALL" ? "" : searchModel.Color);

            startDate = new SqlParameter("StartDate", searchModel.Interval_Date_Start);
            endDate = new SqlParameter("EndDate", searchModel.Interval_Date_End);
            flowChart_Version = new SqlParameter("FlowChart_Version", searchModel.Verion_Interval);

            var funPlant = new SqlParameter("FunPlant", string.IsNullOrEmpty(searchModel.FunPlant) ? "" : searchModel.FunPlant);
            var flowChart_Master_UID = new SqlParameter("Flowchart_Master_UID", GetFlowchartMasterUID(searchModel));
            IEnumerable<ChartDailyReport> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ChartDailyReport>("usp_ChartDailyData  @Color,@StartDate,@EndDate, @FunPlant,@FlowChart_Version,@Flowchart_Master_UID",
                color, startDate, endDate, funPlant, flowChart_Version, flowChart_Master_UID).ToArray();
            return result.ToList();
        }
        public List<YieldVM> QueryDailyYield(ReportDataSearch searchModel)
        {

            var color = new SqlParameter("Color", searchModel.Color == "ALL" ? "" : searchModel.Color);
            var ProductDate = new SqlParameter("ProductDate", searchModel.Reference_Date);
            var funPlant = new SqlParameter("FunPlant", string.IsNullOrEmpty(searchModel.FunPlant) ? "" : searchModel.FunPlant);
            //var flowChart_Version = new SqlParameter("FlowChart_Version", searchModel.Verion_Interval);
            var flowChart_Master_UID = new SqlParameter("Flowchart_Master_UID", GetFlowchartMasterUID(searchModel));
            IEnumerable<YieldVM> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<YieldVM>("usp_getYieldData  @Color,@ProductDate, @FunPlant,@Flowchart_Master_UID",
                color, ProductDate, funPlant, flowChart_Master_UID).ToArray();
            return result.ToList();
        }


        ////战情报表-周报查询
        public List<WeekReport> QueryWeekReport(ReportDataSearch searchModel)
        {
            var color = new SqlParameter("Color", searchModel.Color == "ALL" ? "" : searchModel.Color);
            var SearchDate = new SqlParameter("SearchDate", searchModel.Week_Date_Start);
            var funPlant = new SqlParameter("FunPlant", string.IsNullOrEmpty(searchModel.FunPlant) ? "" : searchModel.FunPlant);
            var flowChart_Version = new SqlParameter("FlowChart_Version", searchModel.Week_Version);
            var flowChart_Master_UID = new SqlParameter("Flowchart_Master_UID", GetFlowchartMasterUID(searchModel));

            IEnumerable<WeekReport> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<WeekReport>("usp_GetWeekReport  @Color,@SearchDate,@FlowChart_Version,@Flowchart_Master_UID, @FunPlant",
                color, SearchDate, flowChart_Version, flowChart_Master_UID, funPlant).ToArray();
            return result.ToList();
        }


        public int GetFlowchartMasterUID(ReportDataSearch searcharModel)
        {


            var query = from FM in DataContext.FlowChart_Master
                        where FM.System_Project.Project_Name == searcharModel.Project
                      && FM.Part_Types == searcharModel.Part_Types && FM.System_Project.Product_Phase == searcharModel.Product_Phase
                        select FM.FlowChart_Master_UID;
            return query.FirstOrDefault();

        }

        #endregion
        public string Edit_WIP(EditWIPDTO dto)
        {
            var FlowChart_Master_UID = new SqlParameter("FlowChart_Master_UID", dto.FlowChart_Master_UID);
            var Last_ProductDate = new SqlParameter("Last_ProductDate", dto.Date_Last);
            var Now_ProductDate = new SqlParameter("Now_ProductDate", dto.Date_Now);
            var Last_TimeInterval = new SqlParameter("Last_TimeInterval", dto.Interval_Last);
            var Now_TimeInterval = new SqlParameter("Now_TimeInterval", dto.Interval_Now);
            IEnumerable<string> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<string>(
            "MODIFY_LAST_WIP  @Now_TimeInterval,@Now_ProductDate,@Last_TimeInterval,@Last_ProductDate, @FlowChart_Master_UID",
             Now_TimeInterval, Now_ProductDate, Last_TimeInterval, Last_ProductDate, FlowChart_Master_UID).ToArray();
            return result.ToList()[0];
        }
        #region Rework Module--------------------------Sidney 2016/04/22


        /// <summary>
        /// 获取返工的Product_Uid
        /// </summary>
        /// <param name="nowTime"></param>
        /// <param name="TimeInterval"></param>
        /// <returns></returns>
        public string GetRepairUid(DateTime nowTime, string TimeInterval)
        {
            var repair_detail_uid = from product in DataContext.Product_Input
                                    join detail in DataContext.FlowChart_Detail on product.FlowChart_Detail_UID equals
                                        detail.FlowChart_Detail_UID
                                    where
                                        (detail.Rework_Flag == "Repair" && product.Product_Date == nowTime &&
                                         product.Time_Interval == TimeInterval)
                                    select product.Product_UID;
            return repair_detail_uid.FirstOrDefault().ToString();
        }
        #endregion
        public ErrorInfoVM GetErrorInfo(int productUid, string ErrorType)
        {
            var strSql = @"DECLARE @ErrorType NVARCHAR(20) ,
    @Color NVARCHAR(20) ,
    @FlowChart_Master_UID INT ,
    @FlowChart_Detail_UID INT ,
    @Process_Seq INT ,
    @tempCount INT
    SET @ErrorType='{1}'
SELECT  @FlowChart_Master_UID = FlowChart_Master_UID ,
        @FlowChart_Detail_UID = FlowChart_Detail_UID ,
        @Process_Seq = Process_Seq ,
        @Color = Color
FROM    dbo.Product_Input AS pi
WHERE   IsLast = 1
        AND Product_UID = {0};
IF @ErrorType != 'Good_QTY'
    BEGIN 
        SELECT DISTINCT
                Process ,
                FunPlant ,
                FunPlant_Manager ,
                CONVERT(NVARCHAR(20),su.Tel) ,
                su.Email
        FROM    dbo.Product_Input AS pi ,
                dbo.System_Users AS su
        WHERE   IsLast = 1
                AND FlowChart_Master_UID = @FlowChart_Master_UID
                AND Process_Seq = @Process_Seq - 1
                AND pi.FunPlant_Manager = su.User_Name
    END
ELSE
    BEGIN
        SELECT DISTINCT
                Process ,
                FunPlant ,
                FunPlant_Manager ,
                CONVERT(NVARCHAR(20),su.Tel) ,
                su.Email
        FROM    dbo.Product_Input AS pi ,
                dbo.System_Users AS su
        WHERE   IsLast = 1
                AND FlowChart_Master_UID = @FlowChart_Master_UID
                AND Process_Seq = @Process_Seq + 1
                AND pi.FunPlant_Manager = su.User_Name
    END    
    ";
            strSql = string.Format(strSql, productUid, ErrorType);
            var dbList = DataContext.Database.SqlQuery<ErrorInfoVM>(strSql).ToList();
            return dbList.FirstOrDefault();
        }
        public string InsertOrUpdateProductAndWIP(Product_Input dataInput)
        {
            //建立事务，防止状态改变
            using (var temp = DataContext.Database.BeginTransaction())
            {
                try
                {

                    var detailSingle =
                        DataContext.FlowChart_Detail.SingleOrDefault(
                            m => m.FlowChart_Detail_UID == dataInput.FlowChart_Detail_UID);
                    if (detailSingle != null)
                    {
                        //找到当前的初始WIP
                        var wipQty = detailSingle.WIP_QTY;
                        //加上当前WIP
                        wipQty += dataInput.Picking_QTY + dataInput.WH_Picking_QTY - dataInput.NG_QTY -
                                  dataInput.Adjust_QTY - dataInput.Good_QTY - dataInput.WH_QTY;
                        //更新Detail表中的WIP_QTY
                        detailSingle.WIP_QTY = wipQty;
                        //更新Product_Input表中的WIP_QTY
                        dataInput.WIP_QTY = wipQty;
                        DataContext.FlowChart_Detail.AddOrUpdate(detailSingle);
                        DataContext.Product_Input.AddOrUpdate(dataInput);
                        DataContext.SaveChanges();
                        temp.Commit();
                        return "SUCCESS";
                    }
                    else
                    {
                        return "Detail_Uid:" + dataInput.FlowChart_Detail_UID.ToString() + "不存在！";
                    }
                }
                catch (Exception e)
                {
                    temp.Rollback();
                    return e.Message;
                }
            }
        }
        public List<ExportPPCheck_Data> ExportPPCheckData(ExportSearch search)
        {
            var Time_Interval = new SqlParameter("Time_Interval", search.Time_Interval);
            var Product_Date = new SqlParameter("Product_Date", DateTime.Now.ToString(FormatConstants.DateTimeFormatStringByDate));
            var Customer = new SqlParameter("Customer", search.Customer);
            var Project = new SqlParameter("Project", search.Project);
            var Product_Phase = new SqlParameter("Product_Phase", search.Product_Phase);
            var Part_Types = new SqlParameter("Part_Types", search.Part_Types);
            var Color = new SqlParameter("Color", search.Color);
            //var Version = new SqlParameter("Version", 4);
            var ShowType = new SqlParameter("ShowType", "EXPORTDATA");
            IEnumerable<ExportPPCheck_Data> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ExportPPCheck_Data>("usp_PPCheckDataSearch  " +
            "@Time_Interval,@Product_Date,@Customer,@Project, @Product_Phase,@Part_Types,@Color,@ShowType",
            Time_Interval, Product_Date, Customer, Project, Product_Phase, Part_Types, Color, ShowType).ToArray();
            return result.ToList();
        }

        public Customer_Info GetCustomerInfo(DateTime productDate, int flowchart_Master_UID)
        {
            var query = from A in DataContext.Product_Input
                        where A.Product_Date == productDate && A.FlowChart_Master_UID == flowchart_Master_UID
                        select new Customer_Info
                        {
                            Project = A.Project,
                            Part_Types = A.Part_Types,
                            Customer = A.Customer,
                            Product_Phase = A.Product_Phase
                        };
            return query.FirstOrDefault();
        }
        public bool KeyProcessVertify(string projectName, string Part_Types)
        {
            var query = from K in DataContext.Enumeration
                        where K.Enum_Type == "Report_Key_Process" && K.Decription == projectName && K.Enum_Name == Part_Types
                        select K;
            if (query.Count() > 0)
                return true;
            else
                return false;
        }
        public List<CheckProductInputQty> CheckReworkAndRepairQty(PPCheckDataSearch search)
        {
            var linq = (from A in DataContext.Product_Input
                        join B in DataContext.FlowChart_Detail
                        on A.FlowChart_Detail_UID equals B.FlowChart_Detail_UID
                        join C in DataContext.Product_Rework_Info
                        on A.FlowChart_Detail_UID equals C.FlowChart_Detail_UID
                        where A.Customer == search.Customer && A.Project == search.Project && A.Product_Phase == search.Product_Phase
                        && A.Part_Types == search.Part_Types && C.Product_Date == search.Reference_Date && C.Time_Interval == search.Tab_Select_Text && C.Is_Match == false &&
                        (B.Rework_Flag == StructConstants.ReworkFlag.Rework || B.Rework_Flag == StructConstants.ReworkFlag.Repair) && A.Product_Date == search.Reference_Date && A.Time_Interval == search.Tab_Select_Text
                        select new CheckProductInputQty
                        {
                            Product_UID = A.Product_UID,
                            Product_Date = A.Product_Date,
                            Time_Interval = A.Time_Interval,
                            Customer = A.Customer,
                            Project = A.Project,
                            Part_Types = A.Part_Types,
                            Process = A.Process,
                            Color = A.Color,
                            Opposite_Detail_UID = C.Opposite_Detail_UID,
                            FlowChart_Master_UID = A.FlowChart_Master_UID,
                            FlowChart_Version = A.FlowChart_Version,
                            FlowChart_Detail_UID = A.FlowChart_Detail_UID,
                            Rework_Flag = B.Rework_Flag,
                            Rework_Type = C.Rework_Type,
                            Opposite_QTY = C.Opposite_QTY.Value,
                            Is_Match = C.Is_Match
                        }).ToList();
            return linq;
        }

        #region 修改不可用wip ---------------------2016-11-22 add by karl

        public IQueryable<PPCheckDataItem> QueryNullWIPDatas(PPCheckDataSearch search, Page page, out int count)
        {
            var Time_Interval = new SqlParameter("Time_Interval", search.Interval_Time);
            var Product_Date = new SqlParameter("Product_Date", search.Product_Date.ToString(FormatConstants.DateTimeFormatStringByDate));
            var Customer = new SqlParameter("Customer", search.Customer);
            var Project = new SqlParameter("Project", search.Project);
            var Product_Phase = new SqlParameter("Product_Phase", search.Product_Phase);
            var Part_Types = new SqlParameter("Part_Types", search.Part_Types);
            var Color = new SqlParameter("Color", search.Color);

            IEnumerable<PPCheckDataItem> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<PPCheckDataItem>("usp_PPModifyNullWIPSearch " +
            "@Time_Interval,@Product_Date,@Customer,@Project,@Product_Phase,@Part_Types,@Color",
            Time_Interval, Product_Date, Customer, Project, Product_Phase, Part_Types, Color).ToArray();
            count = result.Count();
            return result.AsQueryable();
        }

        public List<ExportPPCheck_Data> DoExportFunction(ExportSearch search)
        {
            var Time_Interval = new SqlParameter("Time_Interval", search.Time_Interval);
            var Product_Date = new SqlParameter("Product_Date", search.Product_Date.ToString(FormatConstants.DateTimeFormatStringByDate));
            var Customer = new SqlParameter("Customer", search.Customer);
            var Project = new SqlParameter("Project", search.Project);
            var Product_Phase = new SqlParameter("Product_Phase", search.Product_Phase);
            var Part_Types = new SqlParameter("Part_Types", search.Part_Types);
            var Color = new SqlParameter("Color", search.Color);
            IEnumerable<ExportPPCheck_Data> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ExportPPCheck_Data>("usp_PPModifyNullWIPSearch  " +
            "@Time_Interval,@Product_Date,@Customer,@Project, @Product_Phase,@Part_Types,@Color",
            Time_Interval, Product_Date, Customer, Project, Product_Phase, Part_Types, Color).ToArray();
            return result.ToList();
        }

        public string EditWipWithZero(ExportSearch search)
        {
            try
            {
                string sql = @"UPDATE Product_Input SET Is_Comfirm=1,Modified_UID={0},Modified_Date='{1}' 
                         WHERE Is_Comfirm=0 and Product_Date='{2}'
                         AND Customer=N'{3}' AND Project=N'{4}' AND  Part_Types=N'{5}'";
                sql = string.Format(sql, search.Modified_UID, DateTime.Now.ToString(FormatConstants.DateTimeFormatString), search.Product_Date, search.Customer, search.Project, search.Part_Types);
                if (!string.IsNullOrWhiteSpace(search.Product_Phase) && search.Product_Phase != "ALL")
                    sql += string.Format("AND Product_Phase=N'{0}'", search.Product_Phase);
                if (!string.IsNullOrWhiteSpace(search.Color) && search.Color != "ALL")
                    sql += string.Format("AND Color=N'{0}'", search.Color);
                if (!string.IsNullOrWhiteSpace(search.Time_Interval) && search.Time_Interval != "全天")
                    sql += string.Format("AND Time_Interval=N'{0}'", search.Time_Interval);
                int result = DataContext.Database.ExecuteSqlCommand(sql);
                if (result == 0)
                    return "验证数据出错";
                else
                    return "";
            }
            catch (Exception e)
            {
                return "Error:" + e.Message;
            }
        }

        public List<SystemRoleDTO> Getuserrole(string userid)
        {
            string sql = @"SELECT * FROM dbo.System_User_Role t1 JOIN  System_Role t2
                            ON t1.Role_UID=t2.Role_UID WHERE t1.Account_UID='{0}' ";
            sql = string.Format(sql, userid);
            var dblist = DataContext.Database.SqlQuery<SystemRoleDTO>(sql).ToList();
            return dblist;
        }


        /// <summary>
        /// 获取该制程制定的返工数据列表
        /// </summary>
        /// <param name="master_UID"></param>
        /// <param name="Process_Seq"></param>
        /// <param name="place"></param>
        /// <param name="color"></param>
        /// <param name="productDate"></param>
        /// <param name="TimeInterval"></param>
        /// <returns></returns>
        public List<Product_Rework_Info> getReworkList(int Detail_UID, DateTime productDate, string TimeInterval)
        {
            var query = from rework in DataContext.Product_Rework_Info
                        where rework.FlowChart_Detail_UID == Detail_UID && productDate == rework.Product_Date && TimeInterval == rework.Time_Interval
                        select rework;
            return query.ToList();
        }

        public void AddReworkAndRepairInfo(List<string> insertSqlList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in insertSqlList)
            {
                sb.AppendLine(item);
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                DataContext.Database.ExecuteSqlCommand(sb.ToString());
            }
        }

        public void ModifyReworkNumInfo(StringBuilder sb)
        {
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                DataContext.Database.ExecuteSqlCommand(sb.ToString());
            }
        }

        #endregion

        public void updateConfirmInfo(DateTime Product_Date, int modified_UID, string Time_Interval, int FlowChart_Master_UID, int FlowChart_Version)
        {
            string SQL = @"UPDATE dbo.Product_Input SET Is_Comfirm=1 ,Modified_Date=GETDATE(), Modified_UID={4} WHERE FlowChart_Master_UID={0} AND FlowChart_Version={1} AND Product_Date='{2}' AND Time_Interval='{3}'";
            var strSql = string.Format(SQL, FlowChart_Master_UID, FlowChart_Version, Product_Date, Time_Interval, modified_UID);
            DataContext.Database.ExecuteSqlCommand(strSql);
        }

        public void updateMesSynsData(MES_PISParamDTO MesModel, string sqlStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("UPDATE dbo.Product_Input SET Is_MesSynchroniize = 1");
            //if (MesModel.PIS_Pick_Number > 0)
            //{
            //    var sqlPIS_Pick_Number = $",Picking_QTY ={MesModel.PIS_Pick_Number}";
            //    sb.AppendLine(sqlPIS_Pick_Number);
            //}

            //if (MesModel.IsSyncNG)
            //{
            //    var sqlPIS_GP_Number = $",Good_QTY ={MesModel.PIS_GP_Number}";
            //    sb.AppendLine(sqlPIS_GP_Number);
            //}
            //else
            //{
            //    if (MesModel.PIS_GP_Number > 0)
            //    {
            //        var sqlPIS_GP_Number = $",Good_QTY ={MesModel.PIS_GP_Number}";
            //        sb.AppendLine(sqlPIS_GP_Number);
            //    }
            //}

            //if (MesModel.PIS_NG_Number > 0)
            //{
            //    var sqlPIS_NG_Number = $",NG_QTY ={MesModel.PIS_NG_Number}";
            //    sb.AppendLine(sqlPIS_NG_Number);
            //}

            //if (MesModel.PIS_Rework_Number > 0)
            //{
            //    //var sqlPIS_Rework_Number = $",Picking_QTY ={MesModel.PIS_Rework_Number}";
            //    //sb.AppendLine(sqlPIS_Rework_Number);
            //}

            //var sqlPIS_WIP_Number = $",WIP_QTY ={MesModel.pis_WIPNum}";
            //sb.AppendLine(sqlPIS_WIP_Number);
            sqlStr = sb.ToString() + sqlStr;
            var SQlWhere = $" WHERE [FlowChart_Detail_UID] ='{ MesModel.PIS_ProcessID}' AND [Product_Date] = '{MesModel.Date}'  AND Time_Interval = '{MesModel.TimeInterVal}' AND FlowChart_Version = '{MesModel.FlowChart_Version}'";
            var strSql = sqlStr.ToString() + SQlWhere;

            //string SQL = @"UPDATE dbo.Product_Input SET Is_MesSynchroniize=1, Picking_QTY={0} ;
            //var strSql = string.Format(SQL, MesModel.ProductQuantity, item.PIS_ProcessID, item.Date, item.TimeInterVal);
            DataContext.Database.ExecuteSqlCommand(strSql);
        }

        /// <summary>
        /// 更新下个制程的领料数
        /// </summary>
        /// <param name="MesModel"></param>
        public void updateNextPickData(Product_Input MesModel)
        {
            var strSql = $"UPDATE dbo.Product_Input SET Picking_QTY ={MesModel.Picking_QTY},WIP_QTY={MesModel.WIP_QTY}  where [Product_Date]='{MesModel.Product_Date.ToString("yyyy-MM-dd")}'  AND Time_Interval = '{MesModel.Time_Interval}'  AND Color=N'{MesModel.Color}' AND FlowChart_Master_UID=N'{MesModel.FlowChart_Master_UID}' and Process_Seq={MesModel.Process_Seq} and FlowChart_Version={MesModel.FlowChart_Version} ";
            DataContext.Database.ExecuteSqlCommand(strSql);
        }

        /// <summary>
        /// 更新上个制程的良品数
        /// </summary>
        /// <param name="MesModel"></param>
        public void updateProPickData(Product_Input MesModel)
        {
            var strSql = $"UPDATE dbo.Product_Input SET Good_QTY ={MesModel.Good_QTY},Normal_NG_QTY ={MesModel.Good_QTY},WIP_QTY={MesModel.WIP_QTY} where [Product_Date]='{MesModel.Product_Date.ToString("yyyy-MM-dd")}'  AND Time_Interval = '{MesModel.Time_Interval}'  AND Color=N'{MesModel.Color}' AND FlowChart_Master_UID=N'{MesModel.FlowChart_Master_UID}' and Process_Seq={MesModel.Process_Seq}  and FlowChart_Version={MesModel.FlowChart_Version}";
            DataContext.Database.ExecuteSqlCommand(strSql);
        }

        public void updateConfirmLocationInfo(DateTime Product_Date, string Time_Interval, int FlowChart_Master_UID, int FlowChart_Version)
        {
            string SQL = @"UPDATE dbo.Product_Input_Location SET Is_Comfirm=1  WHERE FlowChart_Master_UID={0} AND FlowChart_Version={1} AND Product_Date='{2}' AND Time_Interval='{3}'";
            var strSql = string.Format(SQL, FlowChart_Master_UID, FlowChart_Version, Product_Date, Time_Interval);
            DataContext.Database.ExecuteSqlCommand(strSql);
        }

        public List<Product_Input> getQAProductData(DateTime Product_Date, string Time_Interval, int FlowChart_Master_UID, int FlowChart_Version)
        {
            var query = from P in DataContext.Product_Input
                        join FD in DataContext.FlowChart_Detail on P.FlowChart_Detail_UID equals FD.FlowChart_Detail_UID
                        where P.FlowChart_Master_UID == FlowChart_Master_UID && P.Time_Interval == Time_Interval && P.FlowChart_Version == FlowChart_Version &&
                        P.Product_Date == Product_Date && FD.IsQAProcess != null && !FD.IsQAProcess.Contains("IPQC")
                        select P;
            return query.ToList();
        }

        public List<Product_Input_Location> getQAProductLocationData(DateTime Product_Date, string Time_Interval, int FlowChart_Master_UID, int FlowChart_Version)
        {
            var query = from P in DataContext.Product_Input_Location
                        join FD in DataContext.FlowChart_Detail on P.FlowChart_Detail_UID equals FD.FlowChart_Detail_UID
                        where P.FlowChart_Master_UID == FlowChart_Master_UID && P.Time_Interval == Time_Interval && P.FlowChart_Version == FlowChart_Version &&
                        P.Product_Date == Product_Date && FD.IsQAProcess != null && !FD.IsQAProcess.Contains("IPQC")
                        select P;
            return query.ToList();
        }

        public string GetOPByFlowchartMasterUID(int masterUID)
        {
            var query = from P in DataContext.System_Project
                        join F in DataContext.FlowChart_Master on P.Project_UID equals F.Project_UID
                        where F.FlowChart_Master_UID == masterUID
                        select P.OP_TYPES;
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 战情日报表导出楼栋详情
        /// </summary>
        /// <returns></returns>
        public List<ExportPPCheck_Data> ExportFloorDetialDayReport(ReportDataSearch search)
        {
            string sql = @"SELECT
	                            p.Process_Seq,
	                            p.FunPlant,
	                            p.Process,
	                            p.Color,
	                            p.Good_QTY,
	                            p.Picking_QTY,
	                            p.WH_Picking_QTY,
	                            p.NG_QTY,
	                            p.WH_QTY,
	                            p.Adjust_QTY,
	                            p.NG_QTY,
	                            p.Place,
	                            p.Time_Interval,
                                p.WIP_QTY,
                                f.Location_Flag,
                                p.FlowChart_Detail_UID,
                                f.Rework_Flag       	                          
                            FROM
	                            Product_Input_Location AS p
                            JOIN dbo.FlowChart_Detail f ON f.FlowChart_Detail_UID = p.FlowChart_Detail_UID
                            WHERE Is_Comfirm = 1 
                            ";

            var SqlParam =
                $" AND p.Project = N'{search.Project}'  AND p.Product_Phase = N'{search.Product_Phase}' AND p.Customer = N'{search.Customer}' AND p.Part_Types = N'{search.Part_Types}' AND p.Product_Date = '{search.Reference_Date}' And p.FlowChart_Version=N'{search.input_day_verion}'";

            var strBuilder = new StringBuilder();
            var Interval_TimeFlag = search.Interval_Time != "全天" && search.Interval_Time != "白班小计"
                 && search.Interval_Time != "ALL" && search.Interval_Time != "Daily_Sum" && search.Interval_Time != "Night_Sum"
                 && search.Interval_Time != "夜班小计";
            if (Interval_TimeFlag)
            {
                var Time_Interval = $" and Time_Interval='{search.Interval_Time}'";
                strBuilder.AppendLine(Time_Interval);
            }

            if (search.Color != "ALL")
            {
                var Color = $" and (p.Color=N'{search.Color}' or p.Color=' ' or p.Color=null)";
                strBuilder.AppendLine(Color);
            }

            if (search.FunPlant != "ALL")
            {
                var FunPlant = $" and p.FunPlant=N'{search.FunPlant}'";
                strBuilder.AppendLine(FunPlant);
            }

            sql = sql + SqlParam + strBuilder.ToString();
            var dblist = DataContext.Database.SqlQuery<ExportPPCheck_Data>(sql).ToList();
            var tempList = new List<ExportPPCheck_Data>();
            var TimeInterValConfig = GetMaxTimeInterVal(search);

            //白天
            var blankTimePartList = TimeInterValConfig.Where(p => int.Parse(p.Enum_Name) <= 6);
            //夜晚
            var blackTimePartList = TimeInterValConfig.Where(p => int.Parse(p.Enum_Name) > 6);

            if (search.Interval_Time == "白班小计" || search.Interval_Time == "Daily_Sum")
            {
                TimeInterValConfig = blankTimePartList.ToList();
                dblist = dblist.Where(p => blankTimePartList.Select(q => q.Enum_Value).ToList().Contains(p.Time_Interval)).ToList();
            }

            if (search.Interval_Time == "夜班小计" || search.Interval_Time == "Night_Sum")
            {
                dblist = dblist.Where(p => blackTimePartList.Select(q => q.Enum_Value).ToList().Contains(p.Time_Interval)).ToList();
                TimeInterValConfig = blackTimePartList.ToList();
            }

            if (dblist.Count <= 0)
            {
                return tempList;
            }

            //所有制程下面的数据
            var ProcessDic = dblist.GroupBy(p => p.Process).ToDictionary(p => p.Key, p => p);
            //按制程分组
            foreach (var proList in ProcessDic)
            {
                #region 按制程不区分颜色统计
                if (search.IsColour == 1)
                {
                    //1 按楼栋汇总
                    var floorDic = proList.Value.GroupBy(p => p.Place).ToDictionary(p => p.Key, q => q);
                    //按楼栋分组
                    foreach (var item in floorDic)
                    {
                        //按颜色分组 查询全天的数据
                        var colorDic = item.Value.GroupBy(p => p.Color).ToDictionary(p => p.Key, q => q);
                        var WIP_QTY = 0;
                        if (!Interval_TimeFlag)
                        {
                            foreach (var colorItem in colorDic)
                            {
                                var wipDic = new Dictionary<int, int>();
                                foreach (var data in colorItem.Value)
                                {
                                    var intervalModel = TimeInterValConfig.Where(p => p.Enum_Value == data.Time_Interval);
                                    var enumTimeInterVal = intervalModel.FirstOrDefault();
                                    if (enumTimeInterVal != null)
                                    {
                                        var intervalNumber = int.Parse(enumTimeInterVal.Enum_Name);
                                        wipDic.Add(intervalNumber, data.WIP_QTY);
                                    }
                                }

                                WIP_QTY += wipDic.OrderByDescending(p => p.Key).FirstOrDefault().Value;
                            }
                        }
                        else
                        {
                            WIP_QTY = item.Value.Sum(p => p.WIP_QTY);
                        }

                        var model = new ExportPPCheck_Data();
                        var orDefault = item.Value.FirstOrDefault();
                        if (orDefault != null)
                        {
                            model.Process_Seq = orDefault.Process_Seq;
                            model.Rework_Flag = orDefault.Rework_Flag;
                        }
                        var checkData = item.Value.FirstOrDefault();
                        if (checkData != null) model.FunPlant = checkData.FunPlant;
                        var firstOrDefault = item.Value.FirstOrDefault();
                        if (firstOrDefault != null) model.Process = firstOrDefault.Process;
                        var ppCheckData = item.Value.FirstOrDefault();
                        if (ppCheckData != null) model.Color = ppCheckData.Color;
                        model.Place = item.Key;
                        model.Picking_QTY = item.Value.Sum(p => p.Picking_QTY);
                        model.WH_Picking_QTY = item.Value.Sum(p => p.WH_Picking_QTY);
                        model.Good_QTY = item.Value.Sum(p => p.Good_QTY);
                        model.NG_QTY = item.Value.Sum(p => p.NG_QTY);
                        model.Adjust_QTY = item.Value.Sum(p => p.Adjust_QTY);
                        model.WH_QTY = item.Value.Sum(p => p.WH_QTY);
                        //最大时段的WIP值
                        model.WIP_QTY = WIP_QTY;

                        //分楼栋才添加
                        var @default = item.Value.FirstOrDefault();
                        if (@default != null && @default.Location_Flag)
                        {
                            tempList.Add(model);
                        }
                    }
                }
                #endregion
                else
                {
                    #region 按制程区分颜色统计
                    //1 按楼栋汇总
                    var floorDic = proList.Value.GroupBy(p => p.Place).ToDictionary(p => p.Key, q => q);
                    //按楼栋分组
                    foreach (var flooritem in floorDic)
                    {
                        //按颜色分组 查询全天的数据
                        var colorDic = flooritem.Value.GroupBy(p => p.Color).ToDictionary(p => p.Key, q => q);
                        foreach (var colorItem in colorDic)
                        {
                            var WIP_QTY = 0;
                            //统计全天的
                            if (!Interval_TimeFlag)
                            {
                                var wipDic = new Dictionary<int, int>();
                                foreach (var data in colorItem.Value)
                                {
                                    var intervalModel =
                                        TimeInterValConfig.Where(p => p.Enum_Value == data.Time_Interval);
                                    var enumTimeInterVal = intervalModel.FirstOrDefault();
                                    if (enumTimeInterVal != null)
                                    {
                                        var intervalNumber = int.Parse(enumTimeInterVal.Enum_Name);
                                        wipDic.Add(intervalNumber, data.WIP_QTY);
                                    }
                                }

                                WIP_QTY = wipDic.OrderByDescending(p => p.Key).FirstOrDefault().Value;
                            }
                            else
                            {
                                WIP_QTY = colorItem.Value.FirstOrDefault().WIP_QTY;
                            }

                            var model = new ExportPPCheck_Data();
                            var orDefault = colorItem.Value.FirstOrDefault();
                            if (orDefault != null)
                            {
                                model.Process_Seq = orDefault.Process_Seq;
                                model.Rework_Flag = orDefault.Rework_Flag;
                            }
                            var checkData = colorItem.Value.FirstOrDefault();
                            if (checkData != null) model.FunPlant = checkData.FunPlant;
                            var firstOrDefault = colorItem.Value.FirstOrDefault();
                            if (firstOrDefault != null) model.Process = firstOrDefault.Process;
                            var ppCheckData = colorItem.Value.FirstOrDefault();
                            if (ppCheckData != null) model.Color = ppCheckData.Color;
                            model.Place = colorItem.Value.FirstOrDefault().Place;
                            model.Picking_QTY = colorItem.Value.Sum(p => p.Picking_QTY);
                            model.WH_Picking_QTY = colorItem.Value.Sum(p => p.WH_Picking_QTY);
                            model.Good_QTY = colorItem.Value.Sum(p => p.Good_QTY);
                            model.NG_QTY = colorItem.Value.Sum(p => p.NG_QTY);
                            model.Adjust_QTY = colorItem.Value.Sum(p => p.Adjust_QTY);
                            model.WH_QTY = colorItem.Value.Sum(p => p.WH_QTY);
                            //最大时段的WIP值
                            model.WIP_QTY = WIP_QTY;
                            //分楼栋才添加
                            var @default = flooritem.Value.FirstOrDefault();
                            if (@default != null && @default.Location_Flag)
                            {
                                tempList.Add(model);
                            }
                        }
                    }
                }
                #endregion
            }

            //return tempList.OrderBy(p => p.Process_Seq).ToList();

            RePaireParam param = new RePaireParam
            {
                //process = model.Process,
                //color = model.Color,
                //place = model.Place,
                Product_Date = System.Convert.ToString(search.Reference_Date),
                Time_Interval = search.Interval_Time,
            };

            var RepairList = GetRepair(param, SqlParam + strBuilder, TimeInterValConfig);


            foreach (var item in tempList)
            {
                item.repairInputCount = RepairList.Where(p => p.Process == item.Process && p.Color == item.Color && p.Place == item.Place && p.rework_type == "Input").Sum(p => p.Opposite_QTY);
                item.repairOutputCount = RepairList.Where(p => p.Process == item.Process && p.Color == item.Color && p.Place == item.Place && p.rework_type == "Output").Sum(p => p.Opposite_QTY);
            }



            return tempList.OrderBy(p => p.Process_Seq).ToList();
        }




        public List<string> GetUnacommpolished_Reason()
        {
            var sqlTxt = @"SELECT E.Enum_Value FROM dbo.Enumeration E  WHERE E.Enum_Type='Unacommpolished_Reason' ORDER BY 
 CONVERT( INT ,E.Decription)";

            return DataContext.Database.SqlQuery<string>(sqlTxt).ToList();

        }

        /// <summary>
        /// 获取的时段配置表的最大时段
        /// </summary>
        /// <returns></returns>
        public List<EnumTimeInterVal> GetMaxTimeInterVal(ReportDataSearch search)
        {
            string strSql = @"SELECT
	                            Enum_UID,
	                            Enum_Type,
	                            Enum_Name,
	                            Enum_Value,
	                            Decription
                            FROM
	                            Enumeration
                          ";
            var param = $"  WHERE Enum_Type = 'Time_InterVal_{search.OP}'";
            strSql = strSql + param;
            var dblist = DataContext.Database.SqlQuery<EnumTimeInterVal>(strSql).ToList();
            if (dblist.Count > 0)
            {
                return dblist;
            }
            return new List<EnumTimeInterVal>();
        }
        public int GetSelctMasterUID(string ProjectName, string Part_Types, string Product_Phase, string opType)
        {
            var query = from P in DataContext.System_Project
                        join FM in DataContext.FlowChart_Master
on P.Project_UID equals FM.Project_UID
                        where P.Project_Name == ProjectName && P.Product_Phase == Product_Phase && P.OP_TYPES == opType && FM.Part_Types == Part_Types
                        select FM.FlowChart_Master_UID;
            return query.FirstOrDefault();
        }

        /// <summary>
        ///计算返修数
        /// </summary>
        /// <returns></returns>
        public List<ExportPPCheck_Data> GetRepair(RePaireParam search, string SqlParam, List<EnumTimeInterVal> TimeInterValConfig)
        {
            StringBuilder sb_flochart = new StringBuilder();
            StringBuilder sb_rework = new StringBuilder();
            //制程
            //if (!string.IsNullOrEmpty(search.process))
            //{
            //    var strProcess = $"and p.Process =N'{search.process}'";
            //    sb_flochart.AppendLine(strProcess);
            //}

            ////是否分颜色
            //if (search.isColor == 0)
            //{
            //    if (!string.IsNullOrEmpty(search.color))
            //    {
            //        var strColor = $"and p.color =N'{search.color}'";
            //        sb_flochart.AppendLine(strColor);
            //    }
            //}
            //else
            //{
            //}

            //if (!string.IsNullOrEmpty(search.place))
            //{
            //    var strPlace = $"and p.place =N'{search.place}'";
            //    sb_flochart.AppendLine(strPlace);
            //}

            var Interval_TimeFlag = search.Time_Interval != "全天" && search.Time_Interval != "白天小计" &&
                search.Time_Interval != "ALL" &&
                search.Time_Interval != "Daily_Sum" &&
                search.Time_Interval != "Night_Sum" &&
                                  search.Time_Interval != "夜班小计";

            if (Interval_TimeFlag)
            {
                var Time_Interval = $" and r.Time_Interval =N'{search.Time_Interval}'";
                sb_rework.AppendLine(Time_Interval);
            }
            if (search.Time_Interval == "白班小计" || search.Time_Interval == "Daily_Sum")
            {
                SqlParam = SqlParam + "AND p.Time_Interval IN (SELECT ee.Enum_Value FROM dbo.Enumeration ee WHERE ee.Enum_Type='Time_InterVal_OP2' AND ee.Enum_Name<=6 )";
            }

            if (search.Time_Interval == "夜班小计" || search.Time_Interval == "Night_Sum")
            {
                SqlParam = SqlParam + "AND p.Time_Interval IN (SELECT ee.Enum_Value FROM dbo.Enumeration ee WHERE ee.Enum_Type='Time_InterVal_OP2' AND ee.Enum_Name>6 )";
            }
            var blankTimePartList = TimeInterValConfig.Where(p => int.Parse(p.Enum_Name) <= 6);
            //夜晚
            var blackTimePartList = TimeInterValConfig.Where(p => int.Parse(p.Enum_Name) > 6);


            //var Rework_Type = $" and r.Rework_Type =N'{type}'";
            //sb_rework.AppendLine(Rework_Type);

            //日期
            var Product_Date = $" and r.Product_Date =N'{search.Product_Date}'";
            sb_rework.AppendLine(Product_Date);

            string srtSQL = @"SELECT  DISTINCT
	                                q.Process_Seq,
	                                q.FunPlant,
	                                q.Process,
	                                q.Color,
	                                q.Good_QTY,
	                                q.Picking_QTY,
	                                q.WH_Picking_QTY,
	                                q.NG_QTY,
	                                q.WH_QTY,
	                                q.Adjust_QTY,
	                                q.Place,
	                                q.Time_Interval,
	                                q.WIP_QTY,
	                                q.Location_Flag,
	                                q.FlowChart_Detail_UID,
	                                q.Rework_Flag,
                                    re.rework_type,
	                                re.Opposite_QTY
                                FROM
	                                (
		                                SELECT
			                                p.Process_Seq,
			                                p.FunPlant,
			                                p.Process,
			                                p.Color,
			                                p.Good_QTY,
			                                p.Picking_QTY,
			                                p.WH_Picking_QTY,
			                                p.NG_QTY,
			                                p.WH_QTY,
			                                p.Adjust_QTY,
			                                p.Place,
			                                p.Time_Interval,
			                                p.WIP_QTY,
			                                f.Location_Flag,
			                                p.FlowChart_Detail_UID,
			                                f.Rework_Flag
		                                FROM
			                                Product_Input_Location AS p
		                                JOIN dbo.FlowChart_Detail f ON f.FlowChart_Detail_UID = p.FlowChart_Detail_UID
		                                WHERE
			                                Is_Comfirm = 1
                                    AND (f.Rework_Flag = 'Rework' or f.Rework_Flag = 'Repair') 
		                                {0}	                                ) q
                                LEFT JOIN (
	                                SELECT
		                                *
	                                FROM
		                                Product_Rework_Info AS r
	                                WHERE
		                                1 = 1
	                             {1}
	                                UNION
		                                SELECT
			                                *
		                                FROM
			                                Product_Rework_Info_History AS r
		                                WHERE
			                                1 = 1
		                               {1}
                                ) re ON q.FlowChart_Detail_UID = re.FlowChart_Detail_UID";

            var searchSQL = string.Format(srtSQL, SqlParam, sb_rework);
            var dblist = DataContext.Database.SqlQuery<ExportPPCheck_Data>(searchSQL).ToList();
            if (!dblist.Any())
            {

                return new List<ExportPPCheck_Data>();
            }
            else
            {

                return dblist;
            }
        }

        public Product_Input GetProductInputByDate(Product_Input dataInput)
        {
            var sql = @"
                             SELECT        [Product_UID]
                                          ,[Is_Comfirm]
                                          ,[Product_Date]
                                          ,[Time_Interval]
                                          ,[Customer]
                                          ,[Project]
                                          ,[Part_Types]
                                          ,[FunPlant]
                                          ,[FunPlant_Manager]
                                          ,[Product_Phase]
                                          ,[Process_Seq]
                                          ,[Place]
                                          ,[Process]
                                          ,[FlowChart_Master_UID]
                                          ,[FlowChart_Version]
                                          ,[Color]
                                          ,[Prouct_Plan]
                                          ,[Product_Stage]
                                          ,[Target_Yield]
                                          ,[Good_QTY]
                                          ,[Good_MismatchFlag]
                                          ,[Picking_QTY]
                                          ,[WH_Picking_QTY]
                                          ,[Picking_MismatchFlag]
                                          ,[NG_QTY]
                                          ,[WH_QTY]
                                          ,[WIP_QTY]
                                          ,[Adjust_QTY]
                                          ,[Creator_UID]
                                          ,[Create_Date]
                                          ,[Material_No]
                                          ,[Modified_UID]
                                          ,[Modified_Date]
                                          ,[IsLast]
                                          ,[DRI]
                                          ,[FlowChart_Detail_UID]
                                          ,[Normal_Good_QTY]
                                          ,[Abnormal_Good_QTY]
                                          ,[Normal_NG_QTY]
                                          ,[Abnormal_NG_QTY]
                                          ,[RowVersion]
                                          ,[NullWip_QTY]
                                          ,[Config]
                                          ,[NotNullWIP]
                                          ,[Unacommpolished_Reason]
                                          ,[Is_MesSynchroniize]
                                      FROM [dbo].[Product_Input] WITH (NOLOCK) 
                                     ";
            var sqlWhere = $" WHERE  [Product_Date]='{dataInput.Product_Date.ToString("yyyy-MM-dd")}' AND [Time_Interval]='{dataInput.Time_Interval}' AND [FlowChart_Detail_UID]='{dataInput.FlowChart_Detail_UID}' AND [FlowChart_Master_UID]='{dataInput.FlowChart_Master_UID}'";
            var inputModel = DataContext.Database.SqlQuery<Product_Input>(sql + sqlWhere).ToList().FirstOrDefault();
            return inputModel;
        }

        public Product_Input GetProductInputBySeq(Product_Input dataInput)
        {
            var sql = @"
                                   SELECT TOP 1 [Product_UID]
                                          ,[Is_Comfirm]
                                          ,[Product_Date]
                                          ,[Time_Interval]
                                          ,[Customer]
                                          ,[Project]
                                          ,[Part_Types]
                                          ,[FunPlant]
                                          ,[FunPlant_Manager]
                                          ,[Product_Phase]
                                          ,[Process_Seq]
                                          ,[Place]
                                          ,[Process]
                                          ,[FlowChart_Master_UID]
                                          ,[FlowChart_Version]
                                          ,[Color]
                                          ,[Prouct_Plan]
                                          ,[Product_Stage]
                                          ,[Target_Yield]
                                          ,[Good_QTY]
                                          ,[Good_MismatchFlag]
                                          ,[Picking_QTY]
                                          ,[WH_Picking_QTY]
                                          ,[Picking_MismatchFlag]
                                          ,[NG_QTY]
                                          ,[WH_QTY]
                                          ,[WIP_QTY]
                                          ,[Adjust_QTY]
                                          ,[Creator_UID]
                                          ,[Create_Date]
                                          ,[Material_No]
                                          ,[Modified_UID]
                                          ,[Modified_Date]
                                          ,[IsLast]
                                          ,[DRI]
                                          ,[FlowChart_Detail_UID]
                                          ,[Normal_Good_QTY]
                                          ,[Abnormal_Good_QTY]
                                          ,[Normal_NG_QTY]
                                          ,[Abnormal_NG_QTY]
                                          ,[RowVersion]
                                          ,[NullWip_QTY]
                                          ,[Config]
                                          ,[NotNullWIP]
                                          ,[Unacommpolished_Reason]
                                          ,[Is_MesSynchroniize]
                                      FROM [dbo].[Product_Input] WITH (NOLOCK) 
                                     ";
            var sqlWhere = $" where[Product_Date] = '{dataInput.Product_Date.ToString("yyyy-MM-dd")}'  AND Time_Interval = '{ dataInput.Time_Interval}'  AND Color=N'{ dataInput.Color}' AND FlowChart_Master_UID=N'{ dataInput.FlowChart_Master_UID}' and Process_Seq>{dataInput.Process_Seq} and FlowChart_Version={dataInput.FlowChart_Version} ORDER BY Process_Seq ASC";
            var inputModel = DataContext.Database.SqlQuery<Product_Input>(sql + sqlWhere).ToList().FirstOrDefault();
            return inputModel;
        }


        public Product_Input GetProductProInputBySeq(Product_Input dataInput)
        {
            var sql = @"
                                   SELECT TOP 1 [Product_UID]
                                          ,[Is_Comfirm]
                                          ,[Product_Date]
                                          ,[Time_Interval]
                                          ,[Customer]
                                          ,[Project]
                                          ,[Part_Types]
                                          ,[FunPlant]
                                          ,[FunPlant_Manager]
                                          ,[Product_Phase]
                                          ,[Process_Seq]
                                          ,[Place]
                                          ,[Process]
                                          ,[FlowChart_Master_UID]
                                          ,[FlowChart_Version]
                                          ,[Color]
                                          ,[Prouct_Plan]
                                          ,[Product_Stage]
                                          ,[Target_Yield]
                                          ,[Good_QTY]
                                          ,[Good_MismatchFlag]
                                          ,[Picking_QTY]
                                          ,[WH_Picking_QTY]
                                          ,[Picking_MismatchFlag]
                                          ,[NG_QTY]
                                          ,[WH_QTY]
                                          ,[WIP_QTY]
                                          ,[Adjust_QTY]
                                          ,[Creator_UID]
                                          ,[Create_Date]
                                          ,[Material_No]
                                          ,[Modified_UID]
                                          ,[Modified_Date]
                                          ,[IsLast]
                                          ,[DRI]
                                          ,[FlowChart_Detail_UID]
                                          ,[Normal_Good_QTY]
                                          ,[Abnormal_Good_QTY]
                                          ,[Normal_NG_QTY]
                                          ,[Abnormal_NG_QTY]
                                          ,[RowVersion]
                                          ,[NullWip_QTY]
                                          ,[Config]
                                          ,[NotNullWIP]
                                          ,[Unacommpolished_Reason]
                                          ,[Is_MesSynchroniize]
                                      FROM [dbo].[Product_Input] WITH (NOLOCK) 
                                     ";
            var sqlWhere = $" where[Product_Date] = '{dataInput.Product_Date.ToString("yyyy-MM-dd")}'  AND Time_Interval = '{ dataInput.Time_Interval}'  AND Color=N'{ dataInput.Color}' AND FlowChart_Master_UID=N'{ dataInput.FlowChart_Master_UID}' and Process_Seq<{dataInput.Process_Seq} and FlowChart_Version={dataInput.FlowChart_Version} ORDER BY Process_Seq DESC";
            var inputModel = DataContext.Database.SqlQuery<Product_Input>(sql + sqlWhere).ToList().FirstOrDefault();
            return inputModel;
        }

        public Product_Input GetProductInputByNOColor(Product_Input dataInput)
        {
            var sql = @"
                                    SELECT  [Product_UID]
                                          ,[Is_Comfirm]
                                          ,[Product_Date]
                                          ,[Time_Interval]
                                          ,[Customer]
                                          ,[Project]
                                          ,[Part_Types]
                                          ,[FunPlant]
                                          ,[FunPlant_Manager]
                                          ,[Product_Phase]
                                          ,[Process_Seq]
                                          ,[Place]
                                          ,[Process]
                                          ,[FlowChart_Master_UID]
                                          ,[FlowChart_Version]
                                          ,[Color]
                                          ,[Prouct_Plan]
                                          ,[Product_Stage]
                                          ,[Target_Yield]
                                          ,[Good_QTY]
                                          ,[Good_MismatchFlag]
                                          ,[Picking_QTY]
                                          ,[WH_Picking_QTY]
                                          ,[Picking_MismatchFlag]
                                          ,[NG_QTY]
                                          ,[WH_QTY]
                                          ,[WIP_QTY]
                                          ,[Adjust_QTY]
                                          ,[Creator_UID]
                                          ,[Create_Date]
                                          ,[Material_No]
                                          ,[Modified_UID]
                                          ,[Modified_Date]
                                          ,[IsLast]
                                          ,[DRI]
                                          ,[FlowChart_Detail_UID]
                                          ,[Normal_Good_QTY]
                                          ,[Abnormal_Good_QTY]
                                          ,[Normal_NG_QTY]
                                          ,[Abnormal_NG_QTY]
                                          ,[RowVersion]
                                          ,[NullWip_QTY]
                                          ,[Config]
                                          ,[NotNullWIP]
                                          ,[Unacommpolished_Reason]
                                          ,[Is_MesSynchroniize]
                                      FROM [dbo].[Product_Input] WITH (NOLOCK) 
                                     ";
            var sqlWhere = $" WHERE  [Product_Date]='{dataInput.Product_Date.ToString("yyyy-MM-dd")}' AND [Time_Interval]='{dataInput.Time_Interval}' AND [Process_Seq]='{dataInput.Process_Seq}' AND [Color]='{dataInput.Color}'";
            var inputModel = DataContext.Database.SqlQuery<Product_Input>(sql + sqlWhere).ToList().FirstOrDefault();
            return inputModel;
        }

        public Product_Input GetUpdateProductProInput(Product_Input dataInput)
        {
            var sql = @"
                                   SELECT  [Product_UID]
                                          ,[Is_Comfirm]
                                          ,[Product_Date]
                                          ,[Time_Interval]
                                          ,[Customer]
                                          ,[Project]
                                          ,[Part_Types]
                                          ,[FunPlant]
                                          ,[FunPlant_Manager]
                                          ,[Product_Phase]
                                          ,[Process_Seq]
                                          ,[Place]
                                          ,[Process]
                                          ,[FlowChart_Master_UID]
                                          ,[FlowChart_Version]
                                          ,[Color]
                                          ,[Prouct_Plan]
                                          ,[Product_Stage]
                                          ,[Target_Yield]
                                          ,[Good_QTY]
                                          ,[Good_MismatchFlag]
                                          ,[Picking_QTY]
                                          ,[WH_Picking_QTY]
                                          ,[Picking_MismatchFlag]
                                          ,[NG_QTY]
                                          ,[WH_QTY]
                                          ,[WIP_QTY]
                                          ,[Adjust_QTY]
                                          ,[Creator_UID]
                                          ,[Create_Date]
                                          ,[Material_No]
                                          ,[Modified_UID]
                                          ,[Modified_Date]
                                          ,[IsLast]
                                          ,[DRI]
                                          ,[FlowChart_Detail_UID]
                                          ,[Normal_Good_QTY]
                                          ,[Abnormal_Good_QTY]
                                          ,[Normal_NG_QTY]
                                          ,[Abnormal_NG_QTY]
                                          ,[RowVersion]
                                          ,[NullWip_QTY]
                                          ,[Config]
                                          ,[NotNullWIP]
                                          ,[Unacommpolished_Reason]
                                          ,[Is_MesSynchroniize]
                                      FROM [dbo].[Product_Input] WITH (NOLOCK) 
                                     ";
            var sqlWhere = $" where[Product_Date] = '{dataInput.Product_Date.ToString("yyyy-MM-dd")}'  AND Time_Interval = '{ dataInput.Time_Interval}'  AND Color=N'{ dataInput.Color}' AND FlowChart_Master_UID=N'{ dataInput.FlowChart_Master_UID}' and Process_Seq={dataInput.Process_Seq} and FlowChart_Version={dataInput.FlowChart_Version}";
            var inputModel = DataContext.Database.SqlQuery<Product_Input>(sql + sqlWhere).ToList().FirstOrDefault();
            return inputModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataInput"></param>
        /// <returns></returns>
        public int GetFlowChart_Master_UID(Product_Input dataInput)
        {
            //var sql = $"SELECT TOP 1 MAX(FlowChart_Version) AS FlowChart_Version  FROM[dbo].[Product_Input] WHERE[Product_Date] = '{dataInput.Product_Date.ToString("yyyy-MM-dd")}' AND[Time_Interval] = '{dataInput.Time_Interval}'  AND FlowChart_Master_UID = N'{ dataInput.FlowChart_Master_UID}'";

            var sql = $"SELECT FlowChart_Version FROM [dbo].[FlowChart_Master] WITH (NOLOCK)  WHERE FlowChart_Master_UID={dataInput.FlowChart_Master_UID}";
            var result = DataContext.Database.SqlQuery<int>(sql).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 删除7天前的数据
        /// </summary>
        /// <param name="dataInput"></param>
        public void DeleteMES_StationDataRecord(Product_Input dataInput)
        {
            var sevenDay = dataInput.Product_Date.AddDays(-7).ToString("yyyy/MM/dd");
            var sql = $" DELETE [dbo].[MES_StationDataRecord]  WHERE Date<'{sevenDay}'";
            DataContext.Database.ExecuteSqlCommand(sql.ToString());
        }

        public bool IsChecked(int FlowchartMasterUID, int FlowchartVersion, string ProductDate, string TimeInterval)
        {

            var sql = $"SELECT COUNT(*) FROM  dbo.Product_Input WHERE   Product_Date = '{ProductDate}' AND Time_Interval = '{TimeInterval}' AND FlowChart_Master_UID = {FlowchartMasterUID} AND FlowChart_Version = {FlowchartVersion} AND Is_Comfirm = 1";
            var result = DataContext.Database.SqlQuery<int>(sql).FirstOrDefault();
            if (result > 0)
                return true;
            else
                return false;

        }

    }

    public interface IProductInputRepository : IRepository<Product_Input>
    {
        #region ProductInput----------------------------------Justin 2015/12/21
        System_Function_Plant QueryFuncPlantInfo(string funcPlant);
        IQueryable<ProductDataDTO> QueryProductDatas(ProcessDataSearch search, Page page, out int count);
        string GetCurrentPlantName(int uid);
        List<ProductDataDTO> QueryProcessData(ProcessDataSearch search, Page page, out int count);
        List<ProductDataDTO> QueryProcessDataForEmergency(ProcessDataSearchModel search, Page page, out int count);
        IQueryable<ProductDataDTO> QueryColorSparations(ProcessDataSearch search);
        List<Product_Input> SetProductDatas(ProductDataList PDataList);
        string ExecAlterSp(Product_Input search);
        string ExecAlterMES_PISSp(Product_Input search);
        string ExecWIPSp(ProductDataItem search);
        string ExecUpdateWIPSp(Product_Input search);

        List<Product_Rework_Info> getReworkList(int Detai_UID, DateTime productDate, string TimeInterval);
        void AddReworkAndRepairInfo(List<string> insertSqlList);
        void ModifyReworkNumInfo(StringBuilder sb);
        #endregion

        //修改无锡厂区的不可用wip数量     2016-11-22 add by karl
        IQueryable<PPCheckDataItem> QueryNullWIPDatas(PPCheckDataSearch search, Page page, out int count);
        List<ExportPPCheck_Data> DoExportFunction(ExportSearch search);
        int GetSelctMasterUID(string ProjectName, string Part_Types, string Product_Phase, string opType);
        string EditWipWithZero(ExportSearch search);
        List<SystemRoleDTO> Getuserrole(string userid);
        string GetOPByFlowchartMasterUID(int masterUID);

        #region 查询Product_Input及Product_Input_Hitory-----------Sidney 2015/12/20
        IQueryable<PPCheckDataItem> QueryPpCheckDatas(PPCheckDataSearch search, Page page, out int count, string QueryType);
        List<string> CheckFunPlantDataIsFull(PPCheckDataSearch searchModel);
        List<string> GetUnacommpolished_Reason();
        List<GetErrorData> CheckProductDataIsFull(PPCheckDataSearch searchModel);
        List<Daily_ProductReportItem> QueryAll_ReportData(ReportDataSearch search, string nowInterval, string nowDate, out int count);
        List<Daily_ProductReportItem> QueryAll_ReportData1(ReportDataSearch search, string nowInterval, string nowDate, out int count);



        List<Daily_ProductReportItem> QueryAll_ReportDataAPP(ReportDataSearch search, string nowInterval, string nowDate, out int count);
        #endregion
        List<TimeSpanReport> QueryTimeSpanReport(ReportDataSearch searchModel);

        List<ChartDailyReport> QueryChartDailyData(ReportDataSearch searchModel);
        //2016-12-20 add by karl  时段报表查询
        List<TimeSpanReport_2> QueryTimeSpanReport_2(ReportDataSearch searchModel);
        List<YieldVM> QueryDailyYield(ReportDataSearch searchModel);
        List<WeekReport> QueryWeekReport(ReportDataSearch searchModel);
        string Edit_WIP(EditWIPDTO dto);

        #region Rework Module--------------------------Sidney 2016/04/22
        List<ProductDataVM> QueryProcessData_Input(ProcessDataSearch search, List<string> currentProject, Page page, out int count);
        string GetRepairUid(DateTime nowTime, string TimeInterval);

        IQueryable<Product_Input> QueryProductDataForEmergency(ProcessDataSearchModel search, Page page, out int count);
        #endregion
        ErrorInfoVM GetErrorInfo(int productUid, string ErrorType);
        string InsertOrUpdateProductAndWIP(Product_Input dataInput);
        List<ExportPPCheck_Data> ExportPPCheckData(ExportSearch search);
        bool KeyProcessVertify(string projectName, string Part_Types);
        List<CheckProductInputQty> CheckReworkAndRepairQty(PPCheckDataSearch search);
        Customer_Info GetCustomerInfo(DateTime productDate, int flowchart_Master_UID);
        void updateConfirmInfo(DateTime Product_Date, int modified_UID, string Time_Interval, int FlowChart_Master_UID, int FlowChart_Version);
        void updateConfirmLocationInfo(DateTime Product_Date, string Time_Interval, int FlowChart_Master_UID, int FlowChart_Version);
        List<Product_Input> getQAProductData(DateTime Product_Date, string Time_Interval, int FlowChart_Master_UID, int FlowChart_Version);
        List<Product_Input_Location> getQAProductLocationData(DateTime Product_Date, string Time_Interval, int FlowChart_Master_UID, int FlowChart_Version);

        /// <summary>
        /// 导出战情日报表的楼栋详情
        /// </summary>
        List<ExportPPCheck_Data> ExportFloorDetialDayReport(ReportDataSearch search);
        List<Daily_ProductReport> QueryInterval_ReportData(NewProductReportSumSearch search, out int count);
        List<Daily_ProductReportSum> QuerySum_ReportData(NewProductReportSumSearch search, out int count);

        List<Daily_ProductReport> GetFullDayInputLocaltion(NewProductReportSumSearch search, out int count);

        void updateMesSynsData(MES_PISParamDTO MesModel, string sqlStr);

        void updateNextPickData(Product_Input MesModel);
        void updateProPickData(Product_Input MesModel);
        Product_Input GetProductInputByDate(Product_Input dataInput);

        Product_Input GetProductInputBySeq(Product_Input dataInput);

        Product_Input GetProductProInputBySeq(Product_Input dataInput);
        //获取需要更新的数据
        Product_Input GetUpdateProductProInput(Product_Input dataInput);
        int GetFlowChart_Master_UID(Product_Input dataInput);
        /// <summary>
        /// 根据制程序号和颜色判断上个制程
        /// </summary>
        /// <param name="dataInput"></param>
        /// <returns></returns>
        Product_Input GetProductInputByNOColor(Product_Input dataInput);


        //删除两小时同步数据7天前的数据
        void DeleteMES_StationDataRecord(Product_Input dataInput);

        bool IsChecked(int FlowchartMasterUID, int FlowchartVersion, string ProductDate, string TimeInterval);
    }
}
