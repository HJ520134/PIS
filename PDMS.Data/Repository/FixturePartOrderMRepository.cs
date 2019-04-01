using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels.Fixture;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixturePartOrderMRepository : IRepository<Fixture_Part_Order_M>
    {
        List<FixturePart_OrderQueryVM> QueryPurchase(FixturePart_OrderVM model, Page page, out int totalcount);

        //导出采购单面维护信息
        List<FixturePart_OrderEdit> GetExportPurchaseData(FixturePart_OrderVM model);

        List<FixturePart_OrderQueryVM> doAllPurchaseorderMaintain(FixturePart_OrderVM model);

        List<FixturePart_OrderQueryVM> doPartPurchaseorderMaintain(string uids);

        List<Fixture_Part_Order_MDTO> GetFixture_Part_Order_Ms(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);

        FixturePart_OrderEdit QueryPurchaseByUID(int Fixture_Part_Order_M_UID);

        List<FixturePartSettingMVM> GetFixturePartByPlantOptypeFunc(int PlantUID, int Optype, int FuncUID);

        List<FixturePartPurchaseVM> GetFixturePartByMUIDAPI(int UID);

        string SaveFixturePartByMUID(SubmitFixturePartOrderVM item);

        string NewSaveFixturePartByMUID(SubmitFixturePartOrderVM item);

        string SaveSubmitFixturePartByMUID(SubmitFixturePartOrderVM item);
        string SaveDeliveryPeriodByMUID(SubmitFixturePartOrderVM item);
    }

    public class FixturePartOrderMRepository : RepositoryBase<Fixture_Part_Order_M>, IFixturePartOrderMRepository
    {
        public FixturePartOrderMRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<FixturePart_OrderQueryVM> QueryPurchase(FixturePart_OrderVM model, Page page, out int totalcount)
        {
            var linq = from A in DataContext.Fixture_Part_Order_M select A;
            if (model != null)
            {
                if (model.Plant_Organization_UID != 0)
                {
                    linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
                }
                if (model.BG_Organization_UID != 0)
                {
                    linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
                }
                if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
                {
                    linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
                }
                if (!string.IsNullOrEmpty(model.Order_ID))
                {
                    linq = linq.Where(m => m.Order_ID.Equals(model.Order_ID));
                }
                if (model.Order_Date != null)
                {
                    linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.Order_Date, model.Order_Date) == 0);
                }
                if (!string.IsNullOrEmpty(model.Part_ID))
                {
                    linq = linq.Where(m => m.Fixture_Part_Order_D.Any(d => d.Fixture_Part.Part_ID.Contains(model.Part_ID)));
                }
                if (model.Vendor_Info_UID.HasValue && model.Vendor_Info_UID != 0)
                {
                    linq = linq.Where(m => m.Fixture_Part_Order_D.Any(d => d.Vendor_Info_UID == model.Vendor_Info_UID));
                }

                //提单人
                if (!string.IsNullOrEmpty(model.Created_Name))
                {
                    linq = linq.Where(m => m.System_Users.User_Name == model.Created_Name);
                }

                //提单时间
                if (model.SubMit_Date != null)
                {
                    linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.Created_Date, model.SubMit_Date) == 0);
                }

                //修改人
                if (!string.IsNullOrEmpty(model.ModifyName))
                {
                    linq = linq.Where(m => m.System_Users1.User_Name == model.ModifyName);
                }

                //修改时间
                if (model.ModifyTime != null)
                {
                    linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, model.ModifyTime) == 0);
                }
            }

            var linq2 = from A in linq
                        select new FixturePart_OrderQueryVM
                        {
                            Fixture_Part_Order_M_UID = A.Fixture_Part_Order_M_UID,
                            Plant_Organization_UID = A.Plant_Organization_UID,
                            BG_Organization_UID = A.BG_Organization_UID,
                            FunPlant_Organization_UID = A.FunPlant_Organization_UID,
                            PlantName = A.System_Organization.Organization_Name,
                            OpType_Name = A.System_Organization1.Organization_Name,
                            Func_Name = A.System_Organization2.Organization_Name,
                            Order_ID = A.Order_ID,
                            Order_Date = A.Order_Date,
                            Is_Complated = A.Is_Complated,
                            Is_SubmitFlag = A.Is_SubmitFlag,
                            Remarks = A.Remarks,
                            Del_Flag = A.Del_Flag,
                            CreatName = A.System_Users.User_Name,
                            Created_Date = A.Created_Date,
                            ModifyName = A.System_Users1.User_Name,
                            ModifyTime = A.Modified_Date
                        };

            //是否完成
            if (model.Is_ComplatedValue == "0")
            {
                model.Is_Complated = false;
            }
            else if (model.Is_ComplatedValue == "1")
            {
                model.Is_Complated = true;
            }

            if (model.Is_ComplatedValue == "ALL")
            {
            }
            else
            {
                linq2 = linq2.Where(p => p.Is_Complated == model.Is_Complated);
            }

            //是否提交
            var is_SubmitFlag = false;
            if (model.Search_Is_Submit == "0")
            {
                is_SubmitFlag = false;
            }
            else if (model.Search_Is_Submit == "1")
            {
                is_SubmitFlag = true;
            }

            if (model.Search_Is_Submit == "ALL")
            {
            }
            else
            {
                linq2 = linq2.Where(p => p.Is_SubmitFlag == is_SubmitFlag);
            }

            linq2 = linq2.Where(p => p.Del_Flag == model.Del_Flag);
            var list2 = linq2.ToList();
            totalcount = list2.Count();
            return list2.Skip(page.Skip).Take(page.PageSize).OrderByDescending(p => p.ModifyTime).ThenBy(q=>q.Created_Date).ToList();
        }

        public List<FixturePart_OrderEdit> GetExportPurchaseData(FixturePart_OrderVM model)
        {
            var linq = from A in DataContext.Fixture_Part_Order_M select A;
            if (model != null)
            {
                if (model.Plant_Organization_UID != 0)
                {
                    linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
                }
                if (model.BG_Organization_UID != 0)
                {
                    linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
                }
                if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
                {
                    linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
                }
                if (!string.IsNullOrEmpty(model.Order_ID))
                {
                    linq = linq.Where(m => m.Order_ID.Equals(model.Order_ID));
                }
                if (model.Order_Date != null)
                {
                    linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.Order_Date, model.Order_Date) == 0);
                }
                if (!string.IsNullOrEmpty(model.Part_ID))
                {
                    linq = linq.Where(m => m.Fixture_Part_Order_D.Any(d => d.Fixture_Part.Part_ID.Contains(model.Part_ID)));
                }
                if (model.Vendor_Info_UID.HasValue && model.Vendor_Info_UID != 0)
                {
                    linq = linq.Where(m => m.Fixture_Part_Order_D.Any(d => d.Vendor_Info_UID == model.Vendor_Info_UID));
                }
            }

            var linq2 = from A in linq
                        select new FixturePart_OrderQueryVM
                        {
                            Fixture_Part_Order_M_UID = A.Fixture_Part_Order_M_UID,
                            Plant_Organization_UID = A.Plant_Organization_UID,
                            BG_Organization_UID = A.BG_Organization_UID,
                            FunPlant_Organization_UID = A.FunPlant_Organization_UID,
                            PlantName = A.System_Organization.Organization_Name,
                            OpType_Name = A.System_Organization1.Organization_Name,
                            Func_Name = A.System_Organization2.Organization_Name,
                            Order_ID = A.Order_ID,
                            Order_Date = A.Order_Date,
                            Is_Complated = A.Is_Complated,
                            Is_SubmitFlag = A.Is_SubmitFlag,
                            Remarks = A.Remarks,
                            Del_Flag = A.Del_Flag,
                            ModifyName = A.System_Users1.User_Name,
                            CreatName = A.System_Users.User_Name,
                            Created_Date = A.Created_Date,
                            ModifyTime = A.Modified_Date
                        };

            //是否完成
            if (model.Is_ComplatedValue == "0")
            {
                model.Is_Complated = false;
            }
            else if (model.Is_ComplatedValue == "1")
            {
                model.Is_Complated = true;
            }

            if (model.Is_ComplatedValue == "ALL" || model.Is_ComplatedValue == null)
            {
            }
            else
            {
                linq2 = linq2.Where(p => p.Is_Complated == model.Is_Complated);
            }

            //是否提交
            var is_SubmitFlag = false;
            if (model.Search_Is_Submit == "0")
            {
                is_SubmitFlag = false;
            }
            else if (model.Search_Is_Submit == "1")
            {
                is_SubmitFlag = true;
            }

            if (model.Search_Is_Submit == "ALL" || model.Search_Is_Submit == null)
            {
            }
            else
            {
                linq2 = linq2.Where(p => p.Is_SubmitFlag == is_SubmitFlag);
            }

            //linq2 = linq2.Where(p => p.Is_Complated == model.Is_Complated);
            linq2 = linq2.Where(p => p.Del_Flag == model.Del_Flag);

            var linq3 = from A in linq2
                        select new FixturePart_OrderEdit
                        {
                            Fixture_Part_Order_M_UID = A.Fixture_Part_Order_M_UID,
                            Plant_Organization_UID = A.Plant_Organization_UID,
                            BG_Organization_UID = A.BG_Organization_UID,
                            FunPlant_Organization_UID = A.FunPlant_Organization_UID,
                            PlantName = A.PlantName,
                            OpType_Name = A.OpType_Name,
                            Func_Name = A.Func_Name,
                            Order_ID = A.Order_ID,
                            Order_Date = A.Order_Date,
                            Is_Complated = A.Is_Complated,
                            Is_SubmitFlag = A.Is_SubmitFlag,
                            Remarks = A.Remarks,
                            Del_Flag = A.Del_Flag,
                            ModifyName = A.ModifyName,
                            CreatName = A.CreatName,
                            Created_Date = A.Created_Date,
                            ModifyTime = A.ModifyTime
                        };

            var masterInfoList = linq3.ToList();
            //获取明细项
            List<FixturePartOrderDList> detailList = new List<FixturePartOrderDList>();
            string detailSql = @"SELECT A.Fixture_Part_Order_D_UID,A.Fixture_Part_Order_M_UID,A.Fixture_Part_UID, 
                            B.Part_ID,B.Part_Name,B.Part_Spec,C.Vendor_Info_UID,C.Vendor_ID,C.Vendor_Name,
                            A.Price,A.Qty,A.Forcast_Complation_Date,A.Del_Flag,
                            (
                            SELECT ISNULL(SUM(Actual_Delivery_Qty), 0) AS SumActualQty 
                            FROM dbo.Fixture_Part_Order_Schedule dd 
                            WHERE Fixture_Part_Order_D_UID = A.Fixture_Part_Order_D_UID ) AS SumActualQty

                            FROM dbo.Fixture_Part_Order_D A
                            JOIN dbo.Fixture_Part B
                            ON B.Fixture_Part_UID = A.Fixture_Part_UID
                            JOIN dbo.Vendor_Info C
                            ON C.Vendor_Info_UID = A.Vendor_Info_UID
                           ";
            detailList = DataContext.Database.SqlQuery<FixturePartOrderDList>(detailSql).ToList();
            foreach (var list in masterInfoList)
            {
                var tempList = detailList.Where(p => p.Fixture_Part_Order_M_UID == list.Fixture_Part_Order_M_UID).ToList();
                list.FixturePartOrderDList = tempList;

                //获取第三级明细项
                var subUIDList = tempList.Select(m => m.Fixture_Part_Order_D_UID).ToList();
                List<FixturePartOrderScheduleList> FixturePartOrderScheduleList = new List<FixturePartOrderScheduleList>();
                var subLinq = from A in DataContext.Fixture_Part_Order_Schedule
                              join user in DataContext.System_Users on A.Estimated_Data_Maintainer_UID equals user.Account_UID into temp
                              from aa in temp.DefaultIfEmpty()
                              join user in DataContext.System_Users on A.Delivery_Data_Maintainer_UID equals user.Account_UID into temp2
                              from bb in temp2.DefaultIfEmpty()
                              where subUIDList.Contains(A.Fixture_Part_Order_D_UID)
                              select new FixturePartOrderScheduleList
                              {
                                  Fixture_Part_Order_Schedule_UID = A.Fixture_Part_Order_Schedule_UID,
                                  Fixture_Part_Order_D_UID = A.Fixture_Part_Order_D_UID,
                                  Receive_Date = A.Estimated_Delivery_Date,
                                  Forcast_Receive_Qty = A.Estimated_Delivery_Qty,
                                  Actual_Receive_Qty = A.Actual_Delivery_Qty,
                                  Del_Flag = A.Del_Flag,
                                  Delivery_Name = aa.User_Name,//交货人
                                  Delivery_Date = A.Estimated_Delivery_Date,//交货人
                                  DeliveryPeriod_Name = bb.User_Name,//交期人
                                  DeliveryPeriod_Date = A.Actual_Delivery_Date//交期时间
                              };

                list.FixturePartOrderScheduleList = subLinq.ToList();
            }

            return masterInfoList;
        }
        public List<FixturePart_OrderQueryVM> doAllPurchaseorderMaintain(FixturePart_OrderVM model)
        {
            var linq = from A in DataContext.Fixture_Part_Order_M select A;
            if (model != null)
            {
                if (model.Plant_Organization_UID != 0)
                {
                    linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
                }
                if (model.BG_Organization_UID != 0)
                {
                    linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
                }
                if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
                {
                    linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
                }
                if (!string.IsNullOrEmpty(model.Order_ID))
                {
                    linq = linq.Where(m => m.Order_ID.Equals(model.Order_ID));
                }
                if (model.Order_Date != null)
                {
                    linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.Order_Date, model.Order_Date) == 0);
                }
                if (!string.IsNullOrEmpty(model.Part_ID))
                {
                    linq = linq.Where(m => m.Fixture_Part_Order_D.Any(d => d.Fixture_Part.Part_ID.Contains(model.Part_ID)));
                }
                if (model.Vendor_Info_UID.HasValue && model.Vendor_Info_UID != 0)
                {
                    linq = linq.Where(m => m.Fixture_Part_Order_D.Any(d => d.Vendor_Info_UID == model.Vendor_Info_UID));
                }
            }

            var linq2 = from A in linq
                        select new FixturePart_OrderQueryVM
                        {
                            Fixture_Part_Order_M_UID = A.Fixture_Part_Order_M_UID,
                            Plant_Organization_UID = A.Plant_Organization_UID,
                            BG_Organization_UID = A.BG_Organization_UID,
                            FunPlant_Organization_UID = A.FunPlant_Organization_UID,
                            PlantName = A.System_Organization.Organization_Name,
                            OpType_Name = A.System_Organization1.Organization_Name,
                            Func_Name = A.System_Organization2.Organization_Name,
                            Order_ID = A.Order_ID,
                            Order_Date = A.Order_Date,
                            Is_Complated = A.Is_Complated,
                            Remarks = A.Remarks,
                            Del_Flag = A.Del_Flag,
                            ModifyName = A.System_Users1.User_Name,
                            CreatName = A.System_Users.User_Name,
                            Created_Date = A.Created_Date,
                            ModifyTime = A.Modified_Date
                        };

            if (model.Is_ComplatedValue == "0")
            {
                model.Is_Complated = false;
            }
            else if (model.Is_ComplatedValue == "1")
            {
                model.Is_Complated = true;
            }

            if (model.Is_ComplatedValue == "ALL")
            {
            }
            else
            {
                linq2 = linq2.Where(p => p.Is_Complated == model.Is_Complated);
            }

            //linq2 = linq2.Where(p => p.Is_Complated == model.Is_Complated);
            linq2 = linq2.Where(p => p.Del_Flag == model.Del_Flag);
            return linq2.ToList();
        }
        public List<FixturePart_OrderQueryVM> doPartPurchaseorderMaintain(string uids)
        {
            uids = "," + uids + ",";
            var linq = from A in DataContext.Fixture_Part_Order_M select A;
            var linq2 = from A in linq
                        select new FixturePart_OrderQueryVM
                        {
                            Fixture_Part_Order_M_UID = A.Fixture_Part_Order_M_UID,
                            Plant_Organization_UID = A.Plant_Organization_UID,
                            BG_Organization_UID = A.BG_Organization_UID,
                            FunPlant_Organization_UID = A.FunPlant_Organization_UID,
                            PlantName = A.System_Organization.Organization_Name,
                            OpType_Name = A.System_Organization1.Organization_Name,
                            Func_Name = A.System_Organization2.Organization_Name,
                            Order_ID = A.Order_ID,
                            Order_Date = A.Order_Date,
                            Is_Complated = A.Is_Complated,
                            Remarks = A.Remarks,
                            Del_Flag = A.Del_Flag,
                            ModifyName = A.System_Users1.User_Name,

                        };

            return linq2.ToList();
        }


        public List<Fixture_Part_Order_MDTO> GetFixture_Part_Order_Ms(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var query = from record in DataContext.Fixture_Part_Order_M
                        select new Fixture_Part_Order_MDTO
                        {
                            Plant_Organization_UID = record.Plant_Organization_UID,
                            BG_Organization_UID = record.BG_Organization_UID,
                            FunPlant_Organization_UID = record.FunPlant_Organization_UID,
                            Fixture_Part_Order_M_UID = record.Fixture_Part_Order_M_UID,
                            Order_ID = record.Order_ID,
                            Order_Date = record.Order_Date,
                            Is_Complated = record.Is_Complated,
                            Remarks = record.Remarks,
                            Del_Flag = record.Del_Flag,
                            Is_SubmitFlag = record.Is_SubmitFlag
                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            query = query.Where(m => m.Is_SubmitFlag == true);
            
            return query.ToList();
        }

        public FixturePart_OrderEdit QueryPurchaseByUID(int Fixture_Part_Order_M_UID)
        {
            var linq = from A in DataContext.Fixture_Part_Order_M
                       where A.Fixture_Part_Order_M_UID == Fixture_Part_Order_M_UID
                       select new FixturePart_OrderEdit
                       {
                           Fixture_Part_Order_M_UID = A.Fixture_Part_Order_M_UID,
                           Plant_Organization_UID = A.Plant_Organization_UID,
                           BG_Organization_UID = A.BG_Organization_UID,
                           FunPlant_Organization_UID = A.FunPlant_Organization_UID,
                           Order_ID = A.Order_ID,
                           Order_Date = A.Order_Date,
                           Is_Complated = A.Is_Complated,
                           Remarks = A.Remarks,
                           Del_Flag = A.Del_Flag
                       };
            var masterInfo = linq.First();

            //绑定供应商，每一个明细项都需要绑定一个供应商下拉框
            var vendorLinq = from A in DataContext.Vendor_Info
                             where A.Plant_Organization_UID == masterInfo.Plant_Organization_UID
                             && A.BG_Organization_UID == masterInfo.BG_Organization_UID && A.Is_Enable == true
                             select new VendorInfoList
                             {
                                 Vendor_Info_UID = A.Vendor_Info_UID,
                                 Plant_Organization_UID = A.Plant_Organization_UID,
                                 BG_Organization_UID = A.BG_Organization_UID,
                                 Vendor_ID = A.Vendor_ID,
                                 Vendor_Name = A.Vendor_Name,
                                 Is_Enable = A.Is_Enable
                             };
            var vendorList = vendorLinq.ToList();


            //获取明细项
            List<FixturePartOrderDList> detailList = new List<FixturePartOrderDList>();
            string detailSql = @"SELECT A.Fixture_Part_Order_D_UID,A.Fixture_Part_Order_M_UID,A.Fixture_Part_UID, 
                            B.Part_ID,B.Part_Name,B.Part_Spec,C.Vendor_Info_UID,C.Vendor_ID,C.Vendor_Name,
                            A.Price,A.Qty,A.Forcast_Complation_Date,A.Del_Flag,
                            (
                            SELECT ISNULL(SUM(Actual_Delivery_Qty), 0) AS SumActualQty 
                            FROM dbo.Fixture_Part_Order_Schedule dd 
                            WHERE Fixture_Part_Order_D_UID = A.Fixture_Part_Order_D_UID ) AS SumActualQty

                            FROM dbo.Fixture_Part_Order_D A
                            JOIN dbo.Fixture_Part B
                            ON B.Fixture_Part_UID = A.Fixture_Part_UID
                            JOIN dbo.Vendor_Info C
                            ON C.Vendor_Info_UID = A.Vendor_Info_UID

                            WHERE Fixture_Part_Order_M_UID={0}";
            detailSql = string.Format(detailSql, Fixture_Part_Order_M_UID);
            detailList = DataContext.Database.SqlQuery<FixturePartOrderDList>(detailSql).ToList();
            int i = 1;
            foreach (var item in detailList)
            {
                item.index = i;
                item.VendorInfoList = vendorList;
                i++;
            }
            masterInfo.FixturePartOrderDList = detailList;

            //获取第三级明细项
            var subUIDList = detailList.Select(m => m.Fixture_Part_Order_D_UID).ToList();
            List<FixturePartOrderScheduleList> FixturePartOrderScheduleList = new List<FixturePartOrderScheduleList>();
            var subLinq = from A in DataContext.Fixture_Part_Order_Schedule
                          join user in DataContext.System_Users on A.Estimated_Data_Maintainer_UID equals user.Account_UID into temp
                          from aa in temp.DefaultIfEmpty()
                          join user in DataContext.System_Users on A.Delivery_Data_Maintainer_UID equals user.Account_UID into temp2
                          from bb in temp2.DefaultIfEmpty()
                          where subUIDList.Contains(A.Fixture_Part_Order_D_UID)
                          select new FixturePartOrderScheduleList
                          {
                              Fixture_Part_Order_Schedule_UID = A.Fixture_Part_Order_Schedule_UID,
                              Fixture_Part_Order_D_UID = A.Fixture_Part_Order_D_UID,
                              Receive_Date = A.Estimated_Delivery_Date,
                              Forcast_Receive_Qty = A.Estimated_Delivery_Qty,
                              Actual_Receive_Qty = A.Actual_Delivery_Qty,
                              Del_Flag = A.Del_Flag,
                              Delivery_Name = bb.User_Name,//交货人
                              Delivery_Date = A.Modified_Date,//交货维护时间
                              DeliveryPeriod_Name = aa.User_Name,//交期人
                              DeliveryPeriod_Date = A.Estimated_Data_Maintainer_Date//交期时间
                          };

            masterInfo.FixturePartOrderScheduleList = subLinq.ToList();
            return masterInfo;
        }

        public List<FixturePartSettingMVM> GetFixturePartByPlantOptypeFunc(int PlantUID, int Optype, int FuncUID)
        {
            var linq = from A in DataContext.Fixture_Part_Setting_M
                       select new FixturePartSettingMVM
                       {
                           Fixture_Part_Setting_M_UID = A.Fixture_Part_Setting_M_UID,
                           Plant_Organization_UID = A.Plant_Organization_UID,
                           BG_Organization_UID = A.BG_Organization_UID,
                           FunPlant_Organization_UID = A.FunPlant_Organization_UID,
                           Fixture_NO = A.Fixture_NO
                       };
            if (PlantUID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID == PlantUID);
            }
            if (Optype != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID == Optype);
            }
            if (FuncUID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID == FuncUID);
            }
            var list = linq.ToList();
            return list;
        }

        public List<FixturePartPurchaseVM> GetFixturePartByMUIDAPI(int UID)
        {
            var linq = from A in DataContext.Fixture_Part_Setting_D
                       where A.Fixture_Part_Setting_M_UID == UID && A.Is_Enable == true
                       && A.Fixture_Part.Is_Enable == true
                       select new FixturePartPurchaseVM
                       {
                           Fixture_Part_Setting_D_UID = A.Fixture_Part_Setting_D_UID,
                           Fixture_Part_Setting_M_UID = A.Fixture_Part_Setting_M_UID,
                           Fixture_Part_UID = A.Fixture_Part_UID,
                           Part_ID = A.Fixture_Part.Part_ID,
                           Part_Name = A.Fixture_Part.Part_Name,
                           Part_Spec = A.Fixture_Part.Part_Spec
                       };
            return linq.ToList();
        }

        public string SaveFixturePartByMUID(SubmitFixturePartOrderVM item)
        {
            DateTime nowDate = DateTime.Now;
            using (var trans = DataContext.Database.BeginTransaction())
            {

                var oldDetailList = DataContext.Fixture_Part_Order_D.Where(m => m.Fixture_Part_Order_M_UID == item.Fixture_Part_Order_M_UID).ToList();
                var oldUIDList = oldDetailList.Select(m => m.Fixture_Part_Order_D_UID).ToList();
                var oldSubList = DataContext.Fixture_Part_Order_Schedule.Where(m => oldUIDList.Contains(m.Fixture_Part_Order_D_UID)).ToList();


                //更新主表
                var masterItem = DataContext.Fixture_Part_Order_M.Find(item.Fixture_Part_Order_M_UID);
                if (masterItem.Is_SubmitFlag)
                {
                    return "订单已经提交不能重复提交";
                }
                masterItem.Order_Date = item.Order_Date;
                masterItem.Remarks = item.Remarks;
                masterItem.Modified_Date = nowDate;
                masterItem.Modified_UID = item.Created_UID;
                masterItem.Is_SubmitFlag = item.Is_SubmitFlag;
                DataContext.SaveChanges();
                DataContext.Commit();
                //全删
                //DataContext.Fixture_Part_Order_Schedule.RemoveRange(oldSubList);
                //DataContext.Fixture_Part_Order_D.RemoveRange(oldDetailList);
                //DataContext.SaveChanges();
                //DataContext.Commit();

                //删除操作
                try
                {
                    var order_D_UIDList = item.FixturePartOrderDList.Select(p => p.Fixture_Part_Order_D_UID).ToList();
                    var delete_D_Order = oldDetailList.Where(p => !order_D_UIDList.Contains(p.Fixture_Part_Order_D_UID)).ToList();
                    var delete_D_OrderUid = delete_D_Order.Select(q => q.Fixture_Part_Order_D_UID).ToList();
                    var IsExistSchedule = oldSubList.Where(p => delete_D_OrderUid.Contains(p.Fixture_Part_Order_D_UID));
                    if (IsExistSchedule.Count() > 0)
                    {
                        return "采购单交货明细区存在未操作数据，不能删除";
                    }

                    DataContext.Fixture_Part_Order_D.RemoveRange(delete_D_Order);
                    DataContext.SaveChanges();
                    DataContext.Commit();
                }
                catch (Exception ex)
                {
                }

                List<Fixture_Part_Order_D> dList = new List<Fixture_Part_Order_D>();
                List<Fixture_Part_Order_Schedule> subList = new List<Fixture_Part_Order_Schedule>();
                foreach (var dtoItem in item.FixturePartOrderDList)
                {
                    var Order_DInfo = DataContext.Fixture_Part_Order_D.Find(dtoItem.Fixture_Part_Order_D_UID);
                    if (Order_DInfo != null)
                    {
                        Order_DInfo.Vendor_Info_UID = dtoItem.Vendor_Info_UID;
                        Order_DInfo.Fixture_Part_UID = dtoItem.Fixture_Part_UID;
                        Order_DInfo.Price = dtoItem.Price;
                        Order_DInfo.Qty = dtoItem.Qty;
                        Order_DInfo.Forcast_Complation_Date = dtoItem.Forcast_Complation_Date;
                        Order_DInfo.Modified_UID = item.Created_UID;
                        Order_DInfo.Modified_Date = nowDate;
                    }
                    else
                    {
                        var sql = InsertDetailSql(dtoItem, item.Fixture_Part_Order_M_UID, item.Created_UID, nowDate);
                        DataContext.Database.ExecuteSqlCommand(sql);
                    }
                    DataContext.SaveChanges();
                    DataContext.Commit();

                    //var suidSql = "SELECT  SCOPE_IDENTITY();";
                    //var flMasterUID = Convert.ToInt32(DataContext.Database.SqlQuery<decimal>(suidSql).First());
                    //StringBuilder sb = new StringBuilder();
                    //foreach (var dtoSubItem in item.FixturePartOrderScheduleList)
                    //{
                    //    if (dtoSubItem.mIndex == dtoItem.index)
                    //    {
                    //        var subSql = InsertSubSql(dtoSubItem, flMasterUID, item.Created_UID, nowDate);
                    //        sb.AppendLine(subSql);
                    //    }
                    //}
                    //DataContext.Database.ExecuteSqlCommand(sb.ToString());
                }

                trans.Commit();
            }

            if (item.Is_SubmitFlag)
            {
                return "提交成功";
            }
            else
            {
                return "保存成功";
            }
        }

        private string InsertMasterSql(Fixture_Part_Order_M item)
        {
            string sql = string.Empty;
            if (item.FunPlant_Organization_UID != null)
            {
                sql = @"INSERT INTO dbo.Fixture_Part_Order_M
                                ( Plant_Organization_UID ,
                                  BG_Organization_UID ,
                                  FunPlant_Organization_UID ,
                                  Order_ID ,
                                  Order_Date ,
                                  Is_Complated ,
                                  Remarks ,
                                  Del_Flag ,
                                  Created_UID ,
                                  Created_Date ,
                                  Modified_UID ,
                                  Modified_Date,
                                  Is_SubmitFlag
                                )
                        VALUES  ( {0} , -- Plant_Organization_UID - int
                                  {1} , -- BG_Organization_UID - int
                                  {2} , -- FunPlant_Organization_UID - int
                                  N'{3}' , -- Order_ID - nvarchar(50)
                                  '{4}' , -- Order_Date - datetime
                                  0 , -- Is_Complated - bit
                                  N'{5}' , -- Remarks - nvarchar(200)
                                  0 , -- Del_Flag - bit
                                  {6} , -- Created_UID - int
                                  '{7}' , -- Created_Date - datetime
                                  {6} , -- Modified_UID - int
                                  '{7}',  -- Modified_Date - datetime
                                  '{8}'  -- Is_SubmitFlag-Is_SubmitFlag
                                )";

                sql = string.Format(sql,
                item.Plant_Organization_UID,
                item.BG_Organization_UID,
                item.FunPlant_Organization_UID,
                item.Order_ID,
                item.Order_Date,
                item.Remarks,
                item.Created_UID,
                item.Created_Date,
               item.Is_SubmitFlag
                );

            }
            else
            {
                sql = @"INSERT INTO dbo.Fixture_Part_Order_M
                                ( Plant_Organization_UID ,
                                  BG_Organization_UID ,
                                  Order_ID ,
                                  Order_Date ,
                                  Is_Complated ,
                                  Remarks ,
                                  Del_Flag ,
                                  Created_UID ,
                                  Created_Date ,
                                  Modified_UID ,
                                  Modified_Date
                                )
                        VALUES  ( {0} , -- Plant_Organization_UID - int
                                  {1} , -- BG_Organization_UID - int
                                  N'{2}' , -- Order_ID - nvarchar(50)
                                  '{3}' , -- Order_Date - datetime
                                  0 , -- Is_Complated - bit
                                  N'{4}' , -- Remarks - nvarchar(200)
                                  0 , -- Del_Flag - bit
                                  {5} , -- Created_UID - int
                                  '{6}' , -- Created_Date - datetime
                                  {5} , -- Modified_UID - int
                                  '{6}'  -- Modified_Date - datetime
                                )";
                sql = string.Format(sql,
                item.Plant_Organization_UID,
                item.BG_Organization_UID,
                item.Order_ID,
                item.Order_Date,
                item.Remarks,
                item.Created_UID,
                item.Created_Date
                );
            }
            return sql;
        }

        private string InsertDetailSql(FixturePartOrderDList item, int uid, int createUid, DateTime nowDate)
        {
            string sql = @"INSERT INTO dbo.Fixture_Part_Order_D
                                ( Fixture_Part_Order_M_UID ,
                                  Vendor_Info_UID ,
                                  Fixture_Part_UID ,
                                  Price ,
                                  Qty ,
                                  Forcast_Complation_Date ,
                                  Is_Complated ,
                                  Del_Flag ,
                                  Created_UID ,
                                  Created_Date ,
                                  Modified_UID ,
                                  Modified_Date
                                )
                        VALUES  ( {0} , -- Fixture_Part_Order_M_UID - int
                                  {1} , -- Vendor_Info_UID - int
                                  {2} , -- Fixture_Part_UID - int
                                  {3} , -- Price - decimal
                                  {4} , -- Qty - decimal
                                  '{5}' , -- Forcast_Complation_Date - date
                                  0 , -- Is_Complated - bit
                                  0 , -- Del_Flag - bit
                                  {6} , -- Created_UID - int
                                  '{7}' , -- Created_Date - datetime
                                  {6} , -- Modified_UID - int
                                  '{7}'  -- Modified_Date - datetime
                                )";
            sql = string.Format(sql,
                uid,
                item.Vendor_Info_UID,
                item.Fixture_Part_UID,
                item.Price,
                item.Qty,
                item.Forcast_Complation_Date,
                createUid,
                nowDate);
            return sql;
        }

        private string InsertSubSql(FixturePartOrderScheduleList item, int uid, int createUid, DateTime nowDate)
        {
            string sql = @" INSERT  INTO dbo.Fixture_Part_Order_Schedule
                                  ( Fixture_Part_Order_D_UID ,
                                    Estimated_Delivery_Date ,
                                    Estimated_Delivery_Qty ,
                                    Estimated_Data_Maintainer_UID ,
                                    Estimated_Data_Maintainer_Date ,
                                    Actual_Delivery_Date ,
                                    Actual_Delivery_Qty ,
                                    Delivery_Data_Maintainer_UID ,
                                    Modified_Date ,
                                    Del_Flag
                                  )
                        VALUES  (
                                   {0}, -- Fixture_Part_Order_D_UID - int
                                   '{1}' , -- Estimated_Delivery_Date - datetime
                                   {2} , -- Estimated_Delivery_Qty - decimal
                                   {3}, -- Estimated_Data_Maintainer_UID - int
                                    GETDATE() , -- Estimated_Data_Maintainer_Date - datetime
                                    NULL , -- Actual_Delivery_Date - datetime
                                    NULL , -- Actual_Delivery_Qty - decimal
                                    NULL , -- Delivery_Data_Maintainer_UID - int
                                    NULL , -- Modified_Date - datetime
                                    0  -- Del_Flag - bit
                                                        );";

            sql = string.Format(sql,
                uid,
                item.Receive_Date,
                item.Forcast_Receive_Qty,
                createUid
                );
            return sql;
        }

        //新建采购单
        public string NewSaveFixturePartByMUID(SubmitFixturePartOrderVM item)
        {
            DateTime nowDate = DateTime.Now;

            using (var trans = DataContext.Database.BeginTransaction())
            {
                Fixture_Part_Order_M newMasterItem = new Fixture_Part_Order_M();
                newMasterItem = AutoMapper.Mapper.Map<Fixture_Part_Order_M>(item);
                newMasterItem.Created_Date = nowDate;
                newMasterItem.Modified_UID = item.Created_UID;
                newMasterItem.Modified_Date = nowDate;
                //newMasterItem.Order_ID = GetSeqNumber("FixturePart_Order_ID");
                newMasterItem.Order_ID = item.Order_ID;
                var IsExistOrderIdSQl = $"SELECT COUNT(*) FROM [PDMS_Test].[dbo].[Fixture_Part_Order_M] WHERE [Order_ID]=N'{newMasterItem.Order_ID}' and Del_Flag=1";
                int OrderIdCount = DataContext.Database.SqlQuery<int>(IsExistOrderIdSQl).First();
                if (OrderIdCount > 0)
                {
                    return "订单ID已经存在";
                }
                //判断订单是否已经提交
                var isSubmit = DataContext.Fixture_Part_Order_M.Where(p => p.Order_ID == newMasterItem.Order_ID).ToList();
                if (isSubmit != null && isSubmit.Count() > 0)
                {
                    return "订单ID已经提交不能再次提交";
                }

                var masterSql = InsertMasterSql(newMasterItem);
                DataContext.Database.ExecuteSqlCommand(masterSql);
                var mUID = "SELECT  SCOPE_IDENTITY();";
                var flMasterUID = Convert.ToInt32(DataContext.Database.SqlQuery<decimal>(mUID).First());
                //DataContext.Fixture_Part_Order_M.Add(newMasterItem);
                foreach (var dtoItem in item.FixturePartOrderDList)
                {
                    var sql = InsertDetailSql(dtoItem, flMasterUID, item.Created_UID, nowDate);
                    DataContext.Database.ExecuteSqlCommand(sql);

                    var suidSql = "SELECT  SCOPE_IDENTITY();";
                    var flDetailUID = Convert.ToInt32(DataContext.Database.SqlQuery<decimal>(suidSql).First());

                    StringBuilder sb = new StringBuilder();

                    if (item.FixturePartOrderScheduleList != null)
                    {
                        foreach (var dtoSubItem in item.FixturePartOrderScheduleList)
                        {
                            if (dtoSubItem.mIndex == dtoItem.index)
                            {
                                var subSql = InsertSubSql(dtoSubItem, flDetailUID, item.Created_UID, nowDate);
                                sb.AppendLine(subSql);
                            }
                        }
                        DataContext.Database.ExecuteSqlCommand(sb.ToString());
                    }
                }

                DataContext.SaveChanges();
                trans.Commit();
            }

            if (item.Is_SubmitFlag)
            {
                return "提交成功";
            }
            else
            {
                return "保存成功";
            }
        }

        //交货处理
        public string SaveSubmitFixturePartByMUID(SubmitFixturePartOrderVM item)
        {
            var Order_M = DataContext.Fixture_Part_Order_M.Where(p => p.Fixture_Part_Order_M_UID == item.Fixture_Part_Order_M_UID).First();
            Order_M.Modified_Date = DateTime.Now;
            Order_M.Modified_UID = item.Created_UID;

            var oldDetailList = DataContext.Fixture_Part_Order_D.Where(m => m.Fixture_Part_Order_M_UID == item.Fixture_Part_Order_M_UID).ToList();
            var oldUIDList = oldDetailList.Select(m => m.Fixture_Part_Order_D_UID).ToList();
            var oldSubList = DataContext.Fixture_Part_Order_Schedule.Where(m => oldUIDList.Contains(m.Fixture_Part_Order_D_UID)).ToList();

            foreach (var subItem in item.FixturePartOrderScheduleList)
            {
                var oldSubItem = oldSubList.Where(m => m.Fixture_Part_Order_Schedule_UID == subItem.Fixture_Part_Order_Schedule_UID).First();
                oldSubItem.Actual_Delivery_Qty = subItem.Actual_Receive_Qty;
                oldSubItem.Delivery_Data_Maintainer_UID = item.Created_UID;
                oldSubItem.Actual_Delivery_Date = DateTime.Now;
                oldSubItem.Modified_Date = DateTime.Now;
            }

            //检查是否都是完成状态
            var detailUIDList = oldSubList.GroupBy(m => m.Fixture_Part_Order_D_UID).Select(m => m.Key).ToList();
            foreach (var detailUID in detailUIDList)
            {
                //检查最下级是否都是完成状态
                var isNotComplated = oldSubList.Exists(m => m.Fixture_Part_Order_D_UID == detailUID);
                //采购数量总和
                var totalQty1 = oldDetailList.Where(p => p.Fixture_Part_Order_D_UID == detailUID).Sum(p => p.Qty);
                //实际交货总和
                var actualQty1 = oldSubList.Where(p => p.Fixture_Part_Order_D_UID == detailUID).Sum(q => q.Actual_Delivery_Qty);
                if (totalQty1 == actualQty1)
                {
                    //如果最下级状态都是完成状态则更新为完成状态
                    var detailItem = oldDetailList.Where(m => m.Fixture_Part_Order_D_UID == detailUID).First();
                    detailItem.Is_Complated = true;
                }

                //if (!isNotComplated)
                //{
                //    //如果最下级状态都是完成状态则更新为完成状态
                //    var detailItem = oldDetailList.Where(m => m.Fixture_Part_Order_D_UID == detailUID).First();
                //    detailItem.Is_Complated = true;
                //}
            }

            //检查第二级是否都是完成状态，如果是则更新主表为完成状态
            var notComplated = oldDetailList.Exists(m => m.Is_Complated == false);
            if (!notComplated)
            {
                var masterItem = DataContext.Fixture_Part_Order_M.Where(m => m.Fixture_Part_Order_M_UID == item.Fixture_Part_Order_M_UID).First();
                masterItem.Is_Complated = true;
            }

            DataContext.SaveChanges();
            DataContext.Commit();
            return string.Empty;
        }

        //交期处理
        public string SaveDeliveryPeriodByMUID(SubmitFixturePartOrderVM item)
        {
            var Order_M = DataContext.Fixture_Part_Order_M.Where(p => p.Fixture_Part_Order_M_UID == item.Fixture_Part_Order_M_UID).First();
            Order_M.Modified_Date = DateTime.Now;
            Order_M.Modified_UID = item.Created_UID;

            var oldDetailList = DataContext.Fixture_Part_Order_D.Where(m => m.Fixture_Part_Order_M_UID == item.Fixture_Part_Order_M_UID).ToList();
            var oldUIDList = oldDetailList.Select(m => m.Fixture_Part_Order_D_UID).ToList();
            var oldSubList = DataContext.Fixture_Part_Order_Schedule.Where(m => oldUIDList.Contains(m.Fixture_Part_Order_D_UID)).ToList();

            foreach (var OrderDLValue in item.FixturePartOrderDList)
            {
                //var oldSubItem = oldSubList.Where(m => m.Fixture_Part_Order_Schedule_UID == subItem.Fixture_Part_Order_Schedule_UID).First();
                //oldSubItem.Estimated_Delivery_Qty = subItem.Forcast_Receive_Qty;
                //oldSubItem.Estimated_Delivery_Date = subItem.Receive_Date;
                //oldSubItem.Estimated_Data_Maintainer_UID = item.Created_UID;
                //oldSubItem.Estimated_Data_Maintainer_Date = DateTime.Now;

                //筛选需要删除的Fixture_Part_Order_Schedule_UID
                if (item.FixturePartOrderScheduleList != null)
                {
                    try
                    {
                        var NewScheduleList = item.FixturePartOrderScheduleList.Select(q => q.Fixture_Part_Order_Schedule_UID).ToList();
                        var deleteItem = oldSubList.Where(p => !NewScheduleList.Contains(p.Fixture_Part_Order_Schedule_UID));
                        foreach (var deleteItemvalue in deleteItem)
                        {
                            DeleteOrderScheduleById(deleteItemvalue.Fixture_Part_Order_Schedule_UID);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    foreach (var dtoSubItem in item.FixturePartOrderScheduleList)
                    {
                        //添加的时候判断是否重复
                        var isExistResult = isExist(dtoSubItem.Fixture_Part_Order_Schedule_UID);
                        if (isExistResult)
                        {
                            continue;
                        }
                        else
                        {
                            if (dtoSubItem.mIndex == OrderDLValue.index)
                            {
                                var subSql = InsertSubSql(dtoSubItem, OrderDLValue.Fixture_Part_Order_D_UID, item.Created_UID, DateTime.Now);
                                DataContext.Database.ExecuteSqlCommand(subSql);
                            }
                        }
                    }
                }
            }

            DataContext.SaveChanges();
            DataContext.Commit();
            return string.Empty;
        }
        private bool isExist(int uid)
        {
            try
            {
                var sql = $"  SELECT COUNT(*) AS CountNum FROM [Fixture_Part_Order_Schedule] WHERE Fixture_Part_Order_Schedule_UID={uid}";
                var result = DataContext.Database.SqlQuery<int>(sql).First();
                if (result > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除采购单配件明细数据
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        private bool DeleteOrderScheduleById(int uid)
        {
            try
            {
                var sql = $" DELETE FROM Fixture_Part_Order_Schedule WHERE  [Fixture_Part_Order_Schedule_UID]={uid};";
                DataContext.Database.ExecuteSqlCommand(sql);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetSeqNumber(string seqCode)
        {
            SqlParameter[] paras = new SqlParameter[3];
            paras[0] = new SqlParameter("@FixturePart_Order_ID", seqCode);

            paras[1] = new SqlParameter("@ReturnNum", SqlDbType.NVarChar, 50);
            paras[1].Direction = ParameterDirection.Output;

            paras[2] = new SqlParameter("@MessageCode", SqlDbType.NVarChar, 200);
            paras[2].Direction = ParameterDirection.Output;

            string proc = @"exec Sp_GetSeqence @FixturePart_Order_ID, @ReturnNum output, @MessageCode output";
            DataContext.Database.ExecuteSqlCommand(proc, paras);
            return paras[1].Value.ToString();
        }
    }
}
