using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using System.Data.Entity.SqlServer;
using PDMS.Common.Constants;
using PDMS.Model.ViewModels;

namespace PDMS.Data.Repository
{
	public interface IStorageInboundRepository : IRepository<Storage_Inbound>
	{
		IQueryable<StorageInboundDTO> GetCreateInfo(StorageInboundDTO searchModel, Page page, out int totalcount);
		List<StorageInboundDTO> DoExportCreateBoundReprot(string uids);
		List<StorageInboundDTO> DoAllExportCreateBoundReprot(StorageInboundDTO search);
		string InsertCreateItem(List<StorageInboundDTO> dtolist);
		string InsertCreateAll(StringBuilder dtolist);
		string InsertInItem(List<StorageInboundDTO> dtolist);
		List<StorageInboundDTO> GetByUId(int Storage_Inbound_UID);
		IQueryable<InOutBoundInfoDTO> GetDetailInfo(InOutBoundInfoDTO searchModel, Page page, out int totalcount);
		List<StorageInboundDTO> GetWarSt(int inboundtype, int plantuid);
		List<StorageInboundDTO> GetWarStByKey(int inboundtype,int warStUid, string key, int plantuid);
		List<string> GetFunplantByUser(int userid);
		List<StorageInboundDTO> GetAllInfo();
		List<InOutBoundInfoDTO> DoExportUpdateBoundReprot(List<InOutBoundVM> uids);
		List<InOutBoundInfoDTO> DoAllExportUpdateBoundReprot(InOutBoundInfoDTO search);
		List<StorageInboundDTO> GetPuinfo(string PU_NO, int Storage_Inbound_UID);
		string InsertInBoundsWithMAP(List<MaterialInfoDTO> mList, List<StorageInboundDTO> addList);
		string UpdateInBoundsWithMAP(StorageInboundDTO dto);
		string DeleteInBoundsWithMAP(StorageInboundDTO dto);
	}
	public class StorageInboundRepository : RepositoryBase<Storage_Inbound>, IStorageInboundRepository
	{
		public StorageInboundRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{
		}
		public IQueryable<StorageInboundDTO> GetCreateInfo(StorageInboundDTO searchModel, Page page, out int totalcount)
		{
			var query = from M in DataContext.Storage_Inbound
						join bm in DataContext.Material_Info on
						M.Material_UID equals bm.Material_Uid
						join storage in DataContext.Warehouse_Storage on
						M.Warehouse_Storage_UID equals storage.Warehouse_Storage_UID
						join war in DataContext.Warehouse on
						storage.Warehouse_UID equals war.Warehouse_UID
						join funplant in DataContext.System_Organization on
						war.FunPlant_Organization_UID equals funplant.Organization_UID
						join BoundType in DataContext.Enumeration on
						M.PartType_UID equals BoundType.Enum_UID
						join statusenums in DataContext.Enumeration on
						M.Status_UID equals statusenums.Enum_UID
						join bgorg in DataContext.System_Organization on
						war.BG_Organization_UID equals bgorg.Organization_UID
						join users in DataContext.System_Users on
						M.Applicant_UID equals users.Account_UID
						join storagebound in DataContext.Enumeration on
						M.Storage_Inbound_Type_UID equals storagebound.Enum_UID
						select new StorageInboundDTO
						{
							Storage_Inbound_UID = M.Storage_Inbound_UID,
							Funplant = funplant.Organization_Name,
							Material_Id = bm.Material_Id,
							Material_Name = bm.Material_Name,
							Material_Types = bm.Material_Types,
							Classification = bm.Classification,
							Inbound_Qty = M.OK_Qty,
							Modified_Date = M.Applicant_Date,
							Warehouse_ID = war.Warehouse_ID,
							Name_ZH = war.Name_ZH,
							Rack_ID = storage.Rack_ID,
							Storage_ID = storage.Storage_ID,
							PartType = BoundType.Enum_Value,
							Status = statusenums.Enum_Value,
							BG = bgorg.Organization_Name,
							PU_NO = M.PU_NO,
							Accepter = "",
							Modifieder = users.User_Name,
							PU_Qty = M.PU_Qty,
							Storage_Inbound_Type = storagebound.Enum_Value,
							FunPlant_Organization_UID = war.FunPlant_Organization_UID,
							Desc = M.Desc,
							BG_Organization_UID = war.BG_Organization_UID
						};
			query = query.Where(m => m.Storage_Inbound_Type == "开账");
			if (!string.IsNullOrWhiteSpace(searchModel.Warehouse_ID))
				query = query.Where(m => m.Warehouse_ID.Contains(searchModel.Warehouse_ID));
			if (!string.IsNullOrWhiteSpace(searchModel.Name_ZH))
				query = query.Where(m => m.Name_ZH.Contains(searchModel.Name_ZH));
			if (!string.IsNullOrWhiteSpace(searchModel.Rack_ID))
				query = query.Where(m => m.Rack_ID.Contains(searchModel.Rack_ID));
			if (!string.IsNullOrWhiteSpace(searchModel.Storage_ID))
				query = query.Where(m => m.Storage_ID.Contains(searchModel.Storage_ID));
			if (!string.IsNullOrWhiteSpace(searchModel.Material_Id))
				query = query.Where(m => m.Material_Id.Contains(searchModel.Material_Id));
			if (!string.IsNullOrWhiteSpace(searchModel.Material_Name))
				query = query.Where(m => m.Material_Name.Contains(searchModel.Material_Name));
			if (!string.IsNullOrWhiteSpace(searchModel.Material_Types))
				query = query.Where(m => m.Material_Types.Contains(searchModel.Material_Types));

			if (!string.IsNullOrWhiteSpace(searchModel.Status))
				query = query.Where(m => m.Status == searchModel.Status);
			else
				query = query.Where(m => m.Status != "已删除");
			List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
			if (Plant_UIDs.Count > 0)
			{
				query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
			}
			totalcount = query.Count();
			query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
			return query;
		}
		public List<StorageInboundDTO> DoExportCreateBoundReprot(string uids)
		{
			uids = "," + uids + ",";
			var query = from M in DataContext.Storage_Inbound
						join bm in DataContext.Material_Info on
						M.Material_UID equals bm.Material_Uid
						join storage in DataContext.Warehouse_Storage on
						M.Warehouse_Storage_UID equals storage.Warehouse_Storage_UID
						join war in DataContext.Warehouse on
						storage.Warehouse_UID equals war.Warehouse_UID
						join funplant in DataContext.System_Organization on
						war.FunPlant_Organization_UID equals funplant.Organization_UID
						join BoundType in DataContext.Enumeration on
						M.PartType_UID equals BoundType.Enum_UID
						join statusenums in DataContext.Enumeration on
						M.Status_UID equals statusenums.Enum_UID
						join bgorg in DataContext.System_Organization on
						war.BG_Organization_UID equals bgorg.Organization_UID
						join users in DataContext.System_Users on
						M.Applicant_UID equals users.Account_UID
						join storagebound in DataContext.Enumeration on
						M.Storage_Inbound_Type_UID equals storagebound.Enum_UID
						select new StorageInboundDTO
						{
							Storage_Inbound_UID = M.Storage_Inbound_UID,
							Funplant = funplant.Organization_Name,
							Material_Id = bm.Material_Id,
							Material_Name = bm.Material_Name,
							Material_Types = bm.Material_Types,
							Classification = bm.Classification,
							Inbound_Qty = M.OK_Qty,
							Modified_Date = M.Applicant_Date,
							Warehouse_ID = war.Warehouse_ID,
							Name_ZH = war.Name_ZH,
							Rack_ID = storage.Rack_ID,
							Storage_ID = storage.Storage_ID,
							PartType = BoundType.Enum_Value,
							Status = statusenums.Enum_Value,
							BG = bgorg.Organization_Name,
							PU_NO = M.PU_NO,
							Accepter = "",
							Modifieder = users.User_Name,
							PU_Qty = M.PU_Qty,
							Storage_Inbound_Type = storagebound.Enum_Value,
							FunPlant_Organization_UID = war.FunPlant_Organization_UID,
							Desc = M.Desc,
							BG_Organization_UID = war.BG_Organization_UID
						};
			query = query.Where(m => uids.Contains("," + m.Storage_Inbound_UID + ","));

			return query.ToList();
		}
		public List<StorageInboundDTO> DoAllExportCreateBoundReprot(StorageInboundDTO search)
		{
			var query = from M in DataContext.Storage_Inbound
						join bm in DataContext.Material_Info on
						M.Material_UID equals bm.Material_Uid
						join storage in DataContext.Warehouse_Storage on
						M.Warehouse_Storage_UID equals storage.Warehouse_Storage_UID
						join war in DataContext.Warehouse on
						storage.Warehouse_UID equals war.Warehouse_UID
						join funplant in DataContext.System_Organization on
						war.FunPlant_Organization_UID equals funplant.Organization_UID
						join BoundType in DataContext.Enumeration on
						M.PartType_UID equals BoundType.Enum_UID
						join statusenums in DataContext.Enumeration on
						M.Status_UID equals statusenums.Enum_UID
						join bgorg in DataContext.System_Organization on
						war.BG_Organization_UID equals bgorg.Organization_UID
						join users in DataContext.System_Users on
						M.Applicant_UID equals users.Account_UID
						join storagebound in DataContext.Enumeration on
						M.Storage_Inbound_Type_UID equals storagebound.Enum_UID
						select new StorageInboundDTO
						{
							Storage_Inbound_UID = M.Storage_Inbound_UID,
							Funplant = funplant.Organization_Name,
							Material_Id = bm.Material_Id,
							Material_Name = bm.Material_Name,
							Material_Types = bm.Material_Types,
							Classification = bm.Classification,
							Inbound_Qty = M.OK_Qty,
							Modified_Date = M.Applicant_Date,
							Warehouse_ID = war.Warehouse_ID,
							Name_ZH = war.Name_ZH,
							Rack_ID = storage.Rack_ID,
							Storage_ID = storage.Storage_ID,
							PartType = BoundType.Enum_Value,
							Status = statusenums.Enum_Value,
							BG = bgorg.Organization_Name,
							PU_NO = M.PU_NO,
							Accepter = "",
							Modifieder = users.User_Name,
							PU_Qty = M.PU_Qty,
							Storage_Inbound_Type = storagebound.Enum_Value,
							FunPlant_Organization_UID = war.FunPlant_Organization_UID,
							Desc = M.Desc,
							BG_Organization_UID = war.BG_Organization_UID
						};
			query = query.Where(m => m.Storage_Inbound_Type == "开账");
			if (!string.IsNullOrWhiteSpace(search.Warehouse_ID))
				query = query.Where(m => m.Warehouse_ID.Contains(search.Warehouse_ID));
			if (!string.IsNullOrWhiteSpace(search.Name_ZH))
				query = query.Where(m => m.Name_ZH.Contains(search.Name_ZH));
			if (!string.IsNullOrWhiteSpace(search.Rack_ID))
				query = query.Where(m => m.Rack_ID.Contains(search.Rack_ID));
			if (!string.IsNullOrWhiteSpace(search.Storage_ID))
				query = query.Where(m => m.Storage_ID.Contains(search.Storage_ID));
			if (!string.IsNullOrWhiteSpace(search.Material_Id))
				query = query.Where(m => m.Material_Id.Contains(search.Material_Id));
			if (!string.IsNullOrWhiteSpace(search.Material_Name))
				query = query.Where(m => m.Material_Name.Contains(search.Material_Name));
			if (!string.IsNullOrWhiteSpace(search.Material_Types))
				query = query.Where(m => m.Material_Types.Contains(search.Material_Types));

			if (!string.IsNullOrWhiteSpace(search.Status))
				query = query.Where(m => m.Status == search.Status);
			else
				query = query.Where(m => m.Status != "已删除");
			List<int> Plant_UIDs = GetOpType(search.Plant_UID).Select(o => o.Organization_UID).ToList();
			if (Plant_UIDs.Count > 0)
			{
				query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
			}

			return query.ToList();

		}
		public string InsertCreateAll(StringBuilder dtolist)
		{
			string result = "";
			using (var trans = DataContext.Database.BeginTransaction())
			{
				try
				{

					DataContext.Database.ExecuteSqlCommand(dtolist.ToString());
					trans.Commit();
				}
				catch (Exception ex)
				{
					trans.Rollback();
					result = "Error:" + ex.Message;
				}
				return result;
			}
		}
		public string InsertCreateItem(List<StorageInboundDTO> dtolist)
		{
			string result = "";
			using (var trans = DataContext.Database.BeginTransaction())
			{
				try
				{
					for (int i = 0; i < dtolist.Count; i++)
					{
						//(Storage_Inbound_Type_UID,
						// Storage_Inbound_ID,
						// Material_Uid,
						// Warehouse_Storage_UID,
						// Inbound_Price,
						// PartType_UID,
						// PU_NO,
						// PU_Qty,
						// Issue_NO,
						// Be_Check_Qty,
						// OK_Qty,
						// NG_Qty,
						// Applicant_UID,
						// Applicant_Date,
						// Status_UID,
						// Desc,
						// Approver_UID,
						// Approver_Date,
						// Current_POPrice)
						//TODO 2019/1/24 steven 因为增加了Current_POPrice的栏位，所以需要增加[18]=0
						var sql = string.Format(@"insert into Storage_Inbound

								values  ({0},N'{1}',{2},{3},{4},{5},N'{6}',{7},
								N'{8}',{9},{10},{11},{12},N'{13}',{14},N'{15}',{16},N'{17}',{18})",
						dtolist[i].Storage_Inbound_Type_UID,
						dtolist[i].Storage_Inbound_ID,
						dtolist[i].Material_Uid,
						dtolist[i].Warehouse_Storage_UID,
						0,
						dtolist[i].PartType_UID,
						"None",
						0,
						"None",
						dtolist[i].Inbound_Qty,
						dtolist[i].Inbound_Qty,
						0,
						dtolist[i].Modified_UID,
						DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
						dtolist[i].Status_UID,
						dtolist[i].Desc,
						dtolist[i].Modified_UID,
						DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
						0);
						DataContext.Database.ExecuteSqlCommand(sql);
					}
					trans.Commit();
				}
				catch (Exception ex)
				{
					trans.Rollback();
					result = "Error:" + ex.Message;
				}
				return result;
			}
		}
		public string InsertInItem(List<StorageInboundDTO> dtolist)
		{
			string result = "";
			using (var trans = DataContext.Database.BeginTransaction())
			{
				try
				{
					for (int i = 0; i < dtolist.Count; i++)
					{
                        //增加更新料號信息檔累計入庫量, 及增入庫時的單價
                        //TODO 2019.3.4 by Steven于入库新增时，Current_POPrice==null ->0
                        var item = DataContext.Material_Info.Find(dtolist[i].Material_Uid);
						if (item != null)
						{
                            dtolist[i].Current_POPrice = item.Unit_Price;
							if (!string.IsNullOrEmpty(dtolist[i].PU_NO))
							{
								item.Last_Qty = item.Last_Qty + dtolist[i].PU_Qty;
							}
						}
						var sql = string.Format(@"insert into Storage_Inbound([Storage_Inbound_Type_UID],[Storage_Inbound_ID],[Material_UID],[Warehouse_Storage_UID],[Inbound_Price],[PartType_UID]
																			 ,[PU_NO],[PU_Qty],[Issue_NO],[Be_Check_Qty],[OK_Qty],[NG_Qty],[Applicant_UID],[Applicant_Date],[Status_UID],[Desc]
																			 ,[Approver_UID],[Approver_Date],[Current_POPrice])
												  values ({0},N'{1}',{2},{3},{4},{5},N'{6}',{7},N'{8}',{9},{10},{11},{12},N'{13}',{14},N'{15}',{16},N'{17}',{18})",
						dtolist[i].Storage_Inbound_Type_UID,
						dtolist[i].Storage_Inbound_ID,
						dtolist[i].Material_Uid,
						dtolist[i].Warehouse_Storage_UID,
						dtolist[i].Inbound_Price == null ? 0 : (decimal)dtolist[i].Inbound_Price,
						dtolist[i].PartType_UID,
						dtolist[i].PU_NO,
						dtolist[i].PU_Qty,
						dtolist[i].Issue_NO,
						dtolist[i].Be_Check_Qty,
						dtolist[i].OK_Qty,
						dtolist[i].NG_Qty,
						dtolist[i].Modified_UID,
						DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
						dtolist[i].Status_UID,
						dtolist[i].Desc,
						dtolist[i].Modified_UID, 
						DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
						dtolist[i].Current_POPrice==null?0 : (decimal)dtolist[i].Current_POPrice);
						DataContext.Database.ExecuteSqlCommand(sql);
					}
					DataContext.SaveChanges();
					trans.Commit();
				}
				catch (Exception ex)
				{
					trans.Rollback();
					result = "Error:" + ex.Message;
				}
				return result;
			}
		}
		public List<StorageInboundDTO> GetByUId(int Storage_Inbound_UID)
		{
			string sql = @"select M.*,t2.Material_Id,t2.Material_Name,t2.Material_Types,t2.Classification,
								t3.Rack_ID,t3.Storage_ID,T5.Enum_Value PartType,t3.Rack_ID,t3.Storage_ID,
								t4.Warehouse_ID,t4.Name_ZH,t2.Organization_UID AS Plant_UID,t4.BG_Organization_UID,
								t4.FunPlant_Organization_UID,t6.Organization_Name Funplant,t7.Enum_Value Status,
								Storage_Inbound_ID Storage_Inbound_ID,T8.ENUM_Value Storage_Inbound_Type
								from Storage_Inbound M inner join Material_Info t2
								on M.Material_Uid=t2.Material_Uid inner join Warehouse_Storage t3
								on M.Warehouse_Storage_UID =t3.Warehouse_Storage_UID inner join Warehouse t4
								on t3.Warehouse_UID=t4.Warehouse_UID inner join Enumeration t5
								on M.PartType_UID=T5.Enum_UID inner join System_Organization t6
								on t4.FunPlant_Organization_UID=t6.Organization_UID inner join Enumeration t7
								on M.Status_UID=t7.Enum_UID INNER JOIN ENUMERATION T8
								ON M.STORAGE_INBOUND_TYPE_UID=T8.ENUM_UID where Storage_Inbound_UID={0}";
			sql = string.Format(sql, Storage_Inbound_UID);
			var dblist = DataContext.Database.SqlQuery<StorageInboundDTO>(sql).ToList();
			return dblist;
		}
		public IQueryable<InOutBoundInfoDTO> GetDetailInfo(InOutBoundInfoDTO searchModel, Page page, out int totalcount)
		{

			var query = from M in DataContext.Storage_Inbound
						join bm in DataContext.Material_Info on
						M.Material_UID equals bm.Material_Uid
						join storage in DataContext.Warehouse_Storage on
						M.Warehouse_Storage_UID equals storage.Warehouse_Storage_UID
						join war in DataContext.Warehouse on
						storage.Warehouse_UID equals war.Warehouse_UID
						join funplant in DataContext.System_Organization on
						war.FunPlant_Organization_UID equals funplant.Organization_UID
						join BoundType in DataContext.Enumeration on
						M.PartType_UID equals BoundType.Enum_UID
						join statusenums in DataContext.Enumeration on
						M.Status_UID equals statusenums.Enum_UID
						join bgorg in DataContext.System_Organization on
						war.BG_Organization_UID equals bgorg.Organization_UID
						join users in DataContext.System_Users on
						M.Applicant_UID equals users.Account_UID
						join storagebound in DataContext.Enumeration on
						M.Storage_Inbound_Type_UID equals storagebound.Enum_UID
						join appuser in DataContext.System_Users on
						M.Applicant_UID equals appuser.Account_UID
						join apruser in DataContext.System_Users on
						M.Approver_UID equals apruser.Account_UID
						select new InOutBoundInfoDTO
						{
							Storage_Bound_UID = M.Storage_Inbound_UID,
							Storage_Inbound_Type_UID = M.Storage_Inbound_Type_UID,
							Storage_Outbound_Type_UID = 0,
							InOut_Type = "入库单",
							In_Out_Type = storagebound.Enum_Value,
							Storage_Bound_ID = M.Storage_Inbound_ID,
							Status = statusenums.Enum_Value,
							Material_Id = bm.Material_Id,
							Material_Name = bm.Material_Name,
							Material_Types = bm.Material_Types,
							Warehouse_ID = war.Warehouse_ID,
							Name_ZH = war.Name_ZH,
							Rack_ID = storage.Rack_ID,
							Storage_ID = storage.Storage_ID,
							Bound_Qty = M.OK_Qty,
							Accepter = "",
							Applicant_UID = M.Applicant_UID,
							Applicant_Date = M.Applicant_Date,
							Approver_UID = M.Approver_UID,
							Approver_Date = M.Approver_Date,
							Storage_Bound_Type = storagebound.Enum_Value,
							Storage_Bound_Type_UID = M.Storage_Inbound_Type_UID,
							ModifiedUser = appuser.User_Name,
							ApproverUser = apruser.User_Name,
							Status_UID = M.Status_UID,
							Outbound_Account_UID = 0,
							PU_NO = M.PU_NO,
							Issue_NO = M.Issue_NO,
							Repair_id = "",
							Warehouse_Storage_UID = M.Warehouse_Storage_UID,
							Material_Uid = M.Material_UID,
							BG_Organization_UID = war.BG_Organization_UID,
							FunPlant_Organization_UID = war.FunPlant_Organization_UID
						};
			List<int> inlist = new List<int>();
			//fky2017/11/13
			//inlist.Add(383);
			inlist.Add(413);
			query = query.Where(m => inlist.Contains(m.Storage_Inbound_Type_UID));
			var query1 = from M in DataContext.Storage_Outbound_M
						 join D in DataContext.Storage_Outbound_D on
						 M.Storage_Outbound_M_UID equals D.Storage_Outbound_M_UID
						 join statusenum in DataContext.Enumeration on
						 M.Status_UID equals statusenum.Enum_UID
						 join bm in DataContext.Material_Info on
						 D.Material_UID equals bm.Material_Uid
						 join warst in DataContext.Warehouse_Storage on
						 D.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
						 join war in DataContext.Warehouse on
						 warst.Warehouse_UID equals war.Warehouse_UID
						 join eqpuser in DataContext.EQP_UserTable on
						 M.Outbound_Account_UID equals eqpuser.EQPUser_Uid
						 join typeenum in DataContext.Enumeration on
						 M.Storage_Outbound_Type_UID equals typeenum.Enum_UID
						 join appuser in DataContext.System_Users on
						M.Applicant_UID equals appuser.Account_UID
						 join apruser in DataContext.System_Users on
						 M.Approver_UID equals apruser.Account_UID
						 join eqpinfo in DataContext.EQPRepair_Info on
						 M.Repair_Uid equals eqpinfo.Repair_Uid into temp
						 from aa in temp.DefaultIfEmpty()
						 select new InOutBoundInfoDTO
						 {
							 Storage_Bound_UID = M.Storage_Outbound_M_UID,
							 Storage_Inbound_Type_UID = 0,
							 Storage_Outbound_Type_UID = M.Storage_Outbound_Type_UID,
							 InOut_Type = "出库单",
							 In_Out_Type = typeenum.Enum_Value,
							 Storage_Bound_ID = M.Storage_Outbound_ID,
							 Status = statusenum.Enum_Value,
							 Material_Id = bm.Material_Id,
							 Material_Name = bm.Material_Name,
							 Material_Types = bm.Material_Types,
							 Warehouse_ID = war.Warehouse_ID,
							 Name_ZH = war.Name_ZH,
							 Rack_ID = warst.Rack_ID,
							 Storage_ID = warst.Storage_ID,
							 Bound_Qty = 0,
							 Accepter = eqpuser.User_Name,
							 Applicant_UID = M.Applicant_UID,
							 Applicant_Date = M.Applicant_Date,
							 Approver_UID = M.Approver_UID,
							 Approver_Date = M.Approver_Date,
							 Storage_Bound_Type = typeenum.Enum_Value,
							 Storage_Bound_Type_UID = M.Storage_Outbound_Type_UID,
							 ModifiedUser = appuser.User_Name,
							 ApproverUser = apruser.User_Name,
							 Status_UID = M.Status_UID,
							 Outbound_Account_UID = M.Outbound_Account_UID == null ? 0 : (int)M.Outbound_Account_UID,
							 PU_NO = "",
							 Issue_NO = "",
							 Repair_id = aa.Repair_id,
							 Warehouse_Storage_UID = warst.Warehouse_Storage_UID,
							 Material_Uid = D.Material_UID,
							 BG_Organization_UID = war.BG_Organization_UID,
							 FunPlant_Organization_UID = war.FunPlant_Organization_UID
						 };
			List<int> outlist = new List<int>();
			//fky2017/11/13
			// outlist.Add(387);
			outlist.Add(416);
			//fky2017/11/13
			// outlist.Add(388);
			outlist.Add(417);
			query1 = query1.Where(m => outlist.Contains(m.Storage_Outbound_Type_UID));

			if (searchModel.Storage_Bound_Type_UID != 0)
				query1 = query1.Where(m => m.Storage_Bound_Type_UID == searchModel.Storage_Bound_Type_UID);
			if (!string.IsNullOrWhiteSpace(searchModel.Storage_Bound_ID))
				query1 = query1.Where(m => m.Storage_Bound_ID.Contains(searchModel.Storage_Bound_ID));



			if (searchModel.Status_UID != 0)
				query1 = query1.Where(m => m.Status_UID == searchModel.Status_UID);
			else
				//fky2017/11/13
				// query1 = query1.Where(m => m.Status_UID != 392 & m.Status_UID != 376);
				query1 = query1.Where(m => m.Status_UID != 420 & m.Status_UID != 408);
			if (searchModel.Applicant_Date.Year != 1)
			{
				DateTime nextappdate = searchModel.Applicant_Date.AddDays(1);
				query1 = query1.Where(m => m.Applicant_Date >= searchModel.Applicant_Date & m.Applicant_Date < nextappdate);
			}
			if (searchModel.Approver_Date.Year != 1)
			{
				DateTime nextaprdate = searchModel.Approver_Date.AddDays(1);
				query1 = query1.Where(m => m.Approver_Date >= searchModel.Approver_Date & m.Approver_Date < nextaprdate);
			}
			if (searchModel.Outbound_Account_UID != 0)
				query1 = query1.Where(m => m.Outbound_Account_UID == searchModel.Outbound_Account_UID);
			if (!string.IsNullOrWhiteSpace(searchModel.ModifiedUser))
				query1 = query1.Where(m => m.ModifiedUser.Contains(searchModel.ModifiedUser));
			if (!string.IsNullOrWhiteSpace(searchModel.ApproverUser))
				query1 = query1.Where(m => m.ApproverUser.Contains(searchModel.ApproverUser));
			if (!string.IsNullOrWhiteSpace(searchModel.PU_NO))
				query1 = query1.Where(m => m.PU_NO == searchModel.PU_NO);
			if (!string.IsNullOrWhiteSpace(searchModel.Issue_NO))
				query1 = query1.Where(m => m.Issue_NO == searchModel.Issue_NO);
			if (!string.IsNullOrWhiteSpace(searchModel.Repair_id))
				query1 = query1.Where(m => m.Repair_id == searchModel.Repair_id);
			if (searchModel.Warehouse_Storage_UID != 0)
				query1 = query1.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);
			if (searchModel.Material_Uid != 0)
				query1 = query1.Where(m => m.Material_Uid == searchModel.Material_Uid);
			if (searchModel.BG_Organization_UID != 0)
				query1 = query1.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
			if (searchModel.FunPlant_Organization_UID != 0)
				query1 = query1.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
			//List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
			//if (Plant_UIDs.Count > 0)
			//{
			//    query1 = query1.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
			//}


			query1 = from M in query1
					 group M by new
					 {
						 M.Status_UID,
						 M.Storage_Bound_ID,
						 M.Storage_Bound_UID,
						 M.In_Out_Type,
						 M.Status,
						 M.Accepter,
						 M.Applicant_Date,
						 M.Approver_Date,
						 M.ModifiedUser,
						 M.ApproverUser,
						 M.Repair_id
					 } into g
					 select new InOutBoundInfoDTO
					 {
						 Storage_Bound_UID = g.Key.Storage_Bound_UID,
						 Storage_Inbound_Type_UID = 0,
						 Storage_Outbound_Type_UID = g.Max(m => m.Storage_Bound_Type_UID),
						 InOut_Type = "出库单",
						 In_Out_Type = g.Key.In_Out_Type,
						 Storage_Bound_ID = g.Key.Storage_Bound_ID,
						 Status = g.Key.Status,
						 Material_Id = "",
						 Material_Name = "",
						 Material_Types = "",
						 Warehouse_ID = "",
						 Name_ZH = "",
						 Rack_ID = "",
						 Storage_ID = "",
						 Bound_Qty = 0,
						 Accepter = g.Key.Accepter,
						 Applicant_UID = 0,
						 Applicant_Date = g.Key.Applicant_Date,
						 Approver_UID = 0,
						 Approver_Date = g.Key.Approver_Date,
						 Storage_Bound_Type = "",
						 Storage_Bound_Type_UID = g.Max(m => m.Storage_Bound_Type_UID),
						 ModifiedUser = g.Key.ModifiedUser,
						 ApproverUser = g.Key.ApproverUser,
						 Status_UID = g.Key.Status_UID,
						 Outbound_Account_UID = 0,
						 PU_NO = "",
						 Issue_NO = "",
						 Repair_id = g.Key.Repair_id,
						 Warehouse_Storage_UID = 0,
						 Material_Uid = 0,
						 BG_Organization_UID = 0,
						 FunPlant_Organization_UID = 0
					 };

			query = query.Union(query1);

			if (searchModel.Storage_Bound_Type_UID != 0)
				query = query.Where(m => m.Storage_Bound_Type_UID == searchModel.Storage_Bound_Type_UID);
			if (!string.IsNullOrWhiteSpace(searchModel.Storage_Bound_ID))
				query = query.Where(m => m.Storage_Bound_ID.Contains(searchModel.Storage_Bound_ID));
			if (searchModel.Status_UID != 0)
				query = query.Where(m => m.Status_UID == searchModel.Status_UID);
			else
				//fky2017/11/13
				// query = query.Where(m => m.Status_UID != 392&m.Status_UID!=376);
				query = query.Where(m => m.Status_UID != 420 & m.Status_UID != 408);
			if (searchModel.Applicant_Date.Year != 1)
			{
				DateTime nextappdate = searchModel.Applicant_Date.AddDays(1);
				query = query.Where(m => m.Applicant_Date >= searchModel.Applicant_Date & m.Applicant_Date < nextappdate);
			}
			if (searchModel.Approver_Date.Year != 1)
			{
				DateTime nextaprdate = searchModel.Approver_Date.AddDays(1);
				query = query.Where(m => m.Approver_Date >= searchModel.Approver_Date & m.Approver_Date < nextaprdate);
			}
			if (searchModel.Outbound_Account_UID != 0)
				query = query.Where(m => m.Outbound_Account_UID == searchModel.Outbound_Account_UID);
			if (!string.IsNullOrWhiteSpace(searchModel.ModifiedUser))
				query = query.Where(m => m.ModifiedUser.Contains(searchModel.ModifiedUser));
			if (!string.IsNullOrWhiteSpace(searchModel.ApproverUser))
				query = query.Where(m => m.ApproverUser.Contains(searchModel.ApproverUser));
			if (!string.IsNullOrWhiteSpace(searchModel.PU_NO))
				query = query.Where(m => m.PU_NO == searchModel.PU_NO);
			if (!string.IsNullOrWhiteSpace(searchModel.Issue_NO))
				query = query.Where(m => m.Issue_NO == searchModel.Issue_NO);
			if (!string.IsNullOrWhiteSpace(searchModel.Repair_id))
				query = query.Where(m => m.Repair_id == searchModel.Repair_id);
			if (searchModel.Warehouse_Storage_UID != 0)
				query = query.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);
			if (searchModel.Material_Uid != 0)
				query = query.Where(m => m.Material_Uid == searchModel.Material_Uid);
			if (searchModel.BG_Organization_UID != 0)
				query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
			if (searchModel.FunPlant_Organization_UID != 0)
				query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);

			List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
			Plant_UIDs.Add(0);
			if (Plant_UIDs.Count > 0)
			{
				query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
			}


			totalcount = query.Count();
			query = query.OrderByDescending(m => m.Applicant_Date).GetPage(page);
			return query;
		}
		public List<InOutBoundInfoDTO> DoExportUpdateBoundReprot(List<InOutBoundVM> uids)
		{
			List<int> inID = uids.Where(o => o.InOutType == "入库单").Select(o => o.InOutID).ToList();
			List<int> OutID = uids.Where(o => o.InOutType != "入库单").Select(o => o.InOutID).ToList();
			var query = from M in DataContext.Storage_Inbound
						join bm in DataContext.Material_Info on
						M.Material_UID equals bm.Material_Uid
						join storage in DataContext.Warehouse_Storage on
						M.Warehouse_Storage_UID equals storage.Warehouse_Storage_UID
						join war in DataContext.Warehouse on
						storage.Warehouse_UID equals war.Warehouse_UID
						join funplant in DataContext.System_Organization on
						war.FunPlant_Organization_UID equals funplant.Organization_UID
						join BoundType in DataContext.Enumeration on
						M.PartType_UID equals BoundType.Enum_UID
						join statusenums in DataContext.Enumeration on
						M.Status_UID equals statusenums.Enum_UID
						join bgorg in DataContext.System_Organization on
						war.BG_Organization_UID equals bgorg.Organization_UID
						join users in DataContext.System_Users on
						M.Applicant_UID equals users.Account_UID
						join storagebound in DataContext.Enumeration on
						M.Storage_Inbound_Type_UID equals storagebound.Enum_UID
						join appuser in DataContext.System_Users on
						M.Applicant_UID equals appuser.Account_UID
						join apruser in DataContext.System_Users on
						M.Approver_UID equals apruser.Account_UID
						select new InOutBoundInfoDTO
						{
							Storage_Bound_UID = M.Storage_Inbound_UID,
							Storage_Inbound_Type_UID = M.Storage_Inbound_Type_UID,
							Storage_Outbound_Type_UID = 0,
							InOut_Type = "入库单",
							In_Out_Type = storagebound.Enum_Value,
							Storage_Bound_ID = M.Storage_Inbound_ID,
							Status = statusenums.Enum_Value,
							Material_Id = bm.Material_Id,
							Material_Name = bm.Material_Name,
							Material_Types = bm.Material_Types,
							Warehouse_ID = war.Warehouse_ID,
							Name_ZH = war.Name_ZH,
							Rack_ID = storage.Rack_ID,
							Storage_ID = storage.Storage_ID,
							Bound_Qty = M.OK_Qty,
							Accepter = "",
							Applicant_UID = M.Applicant_UID,
							Applicant_Date = M.Applicant_Date,
							Approver_UID = M.Approver_UID,
							Approver_Date = M.Approver_Date,
							Storage_Bound_Type = storagebound.Enum_Value,
							Storage_Bound_Type_UID = M.Storage_Inbound_Type_UID,
							ModifiedUser = appuser.User_Name,
							ApproverUser = apruser.User_Name,
							Status_UID = M.Status_UID,
							Outbound_Account_UID = 0,
							PU_NO = M.PU_NO,
							Issue_NO = M.Issue_NO,
							Repair_id = "",
							Warehouse_Storage_UID = M.Warehouse_Storage_UID,
							Material_Uid = M.Material_UID,
							BG_Organization_UID = war.BG_Organization_UID,
							FunPlant_Organization_UID = war.FunPlant_Organization_UID
						};
			List<int> inlist = new List<int>();
			inlist.Add(413);
			query = query.Where(m => inlist.Contains(m.Storage_Inbound_Type_UID) && inID.Contains(m.Storage_Bound_UID));
			var query1 = from M in DataContext.Storage_Outbound_M
						 join D in DataContext.Storage_Outbound_D on
						 M.Storage_Outbound_M_UID equals D.Storage_Outbound_M_UID
						 join statusenum in DataContext.Enumeration on
						 M.Status_UID equals statusenum.Enum_UID
						 join bm in DataContext.Material_Info on
						 D.Material_UID equals bm.Material_Uid
						 join warst in DataContext.Warehouse_Storage on
						 D.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
						 join war in DataContext.Warehouse on
						 warst.Warehouse_UID equals war.Warehouse_UID
						 join eqpuser in DataContext.EQP_UserTable on
						 M.Outbound_Account_UID equals eqpuser.EQPUser_Uid
						 join typeenum in DataContext.Enumeration on
						 M.Storage_Outbound_Type_UID equals typeenum.Enum_UID
						 join appuser in DataContext.System_Users on
						M.Applicant_UID equals appuser.Account_UID
						 join apruser in DataContext.System_Users on
						 M.Approver_UID equals apruser.Account_UID
						 join eqpinfo in DataContext.EQPRepair_Info on
						 M.Repair_Uid equals eqpinfo.Repair_Uid into temp
						 from aa in temp.DefaultIfEmpty()
						 select new InOutBoundInfoDTO
						 {
							 Storage_Bound_UID = M.Storage_Outbound_M_UID,
							 Storage_Inbound_Type_UID = 0,
							 Storage_Outbound_Type_UID = M.Storage_Outbound_Type_UID,
							 InOut_Type = "出库单",
							 In_Out_Type = typeenum.Enum_Value,
							 Storage_Bound_ID = M.Storage_Outbound_ID,
							 Status = statusenum.Enum_Value,
							 Material_Id = bm.Material_Id,
							 Material_Name = bm.Material_Name,
							 Material_Types = bm.Material_Types,
							 Warehouse_ID = war.Warehouse_ID,
							 Name_ZH = war.Name_ZH,
							 Rack_ID = warst.Rack_ID,
							 Storage_ID = warst.Storage_ID,
							 Bound_Qty = 0,
							 Accepter = eqpuser.User_Name,
							 Applicant_UID = M.Applicant_UID,
							 Applicant_Date = M.Applicant_Date,
							 Approver_UID = M.Approver_UID,
							 Approver_Date = M.Approver_Date,
							 Storage_Bound_Type = typeenum.Enum_Value,
							 Storage_Bound_Type_UID = M.Storage_Outbound_Type_UID,
							 ModifiedUser = appuser.User_Name,
							 ApproverUser = apruser.User_Name,
							 Status_UID = M.Status_UID,
							 Outbound_Account_UID = M.Outbound_Account_UID == null ? 0 : (int)M.Outbound_Account_UID,
							 PU_NO = "",
							 Issue_NO = "",
							 Repair_id = aa.Repair_id,
							 Warehouse_Storage_UID = warst.Warehouse_Storage_UID,
							 Material_Uid = D.Material_UID,
							 BG_Organization_UID = war.BG_Organization_UID,
							 FunPlant_Organization_UID = war.FunPlant_Organization_UID
						 };
			List<int> outlist = new List<int>();
			outlist.Add(416);
			outlist.Add(417);
			query1 = query1.Where(m => outlist.Contains(m.Storage_Outbound_Type_UID) && OutID.Contains(m.Storage_Bound_UID));
			query1 = from M in query1
					 group M by new
					 {
						 M.Status_UID,
						 M.Storage_Bound_ID,
						 M.Storage_Bound_UID,
						 M.In_Out_Type,
						 M.Status,
						 M.Accepter,
						 M.Applicant_Date,
						 M.Approver_Date,
						 M.ModifiedUser,
						 M.ApproverUser
					 } into g
					 select new InOutBoundInfoDTO
					 {
						 Storage_Bound_UID = g.Key.Storage_Bound_UID,
						 Storage_Inbound_Type_UID = 0,
						 Storage_Outbound_Type_UID = g.Max(m => m.Storage_Bound_Type_UID),
						 InOut_Type = "出库单",
						 In_Out_Type = g.Key.In_Out_Type,
						 Storage_Bound_ID = g.Key.Storage_Bound_ID,
						 Status = g.Key.Status,
						 Material_Id = "",
						 Material_Name = "",
						 Material_Types = "",
						 Warehouse_ID = "",
						 Name_ZH = "",
						 Rack_ID = "",
						 Storage_ID = "",
						 Bound_Qty = 0,
						 Accepter = g.Key.Accepter,
						 Applicant_UID = 0,
						 Applicant_Date = g.Key.Applicant_Date,
						 Approver_UID = 0,
						 Approver_Date = g.Key.Approver_Date,
						 Storage_Bound_Type = "",
						 Storage_Bound_Type_UID = g.Max(m => m.Storage_Bound_Type_UID),
						 ModifiedUser = g.Key.ModifiedUser,
						 ApproverUser = g.Key.ApproverUser,
						 Status_UID = g.Key.Status_UID,
						 Outbound_Account_UID = 0,
						 PU_NO = "",
						 Issue_NO = "",
						 Repair_id = "",
						 Warehouse_Storage_UID = 0,
						 Material_Uid = 0,
						 BG_Organization_UID = 0,
						 FunPlant_Organization_UID = 0
					 };
			query = query.Union(query1);
			return query.ToList();
		}
		public List<InOutBoundInfoDTO> DoAllExportUpdateBoundReprot(InOutBoundInfoDTO searchModel)
		{

			//var query = from M in DataContext.Storage_Inbound
			//            join bm in DataContext.Material_Info on
			//            M.Material_UID equals bm.Material_Uid
			//            join storage in DataContext.Warehouse_Storage on
			//            M.Warehouse_Storage_UID equals storage.Warehouse_Storage_UID
			//            join war in DataContext.Warehouse on
			//            storage.Warehouse_UID equals war.Warehouse_UID
			//            join funplant in DataContext.System_Organization on
			//            war.FunPlant_Organization_UID equals funplant.Organization_UID
			//            join BoundType in DataContext.Enumeration on
			//            M.PartType_UID equals BoundType.Enum_UID
			//            join statusenums in DataContext.Enumeration on
			//            M.Status_UID equals statusenums.Enum_UID
			//            join bgorg in DataContext.System_Organization on
			//            war.BG_Organization_UID equals bgorg.Organization_UID
			//            join users in DataContext.System_Users on
			//            M.Applicant_UID equals users.Account_UID
			//            join storagebound in DataContext.Enumeration on
			//            M.Storage_Inbound_Type_UID equals storagebound.Enum_UID
			//            join appuser in DataContext.System_Users on
			//            M.Applicant_UID equals appuser.Account_UID
			//            join apruser in DataContext.System_Users on
			//            M.Approver_UID equals apruser.Account_UID
			//            select new InOutBoundInfoDTO
			//            {
			//                Storage_Bound_UID = M.Storage_Inbound_UID,
			//                Storage_Inbound_Type_UID = M.Storage_Inbound_Type_UID,
			//                Storage_Outbound_Type_UID = 0,
			//                InOut_Type = "入库单",
			//                In_Out_Type = storagebound.Enum_Value,
			//                Storage_Bound_ID = M.Storage_Inbound_ID,
			//                Status = statusenums.Enum_Value,
			//                Material_Id = bm.Material_Id,
			//                Material_Name = bm.Material_Name,
			//                Material_Types = bm.Material_Types,
			//                Warehouse_ID = war.Warehouse_ID,
			//                Name_ZH = war.Name_ZH,
			//                Rack_ID = storage.Rack_ID,
			//                Storage_ID = storage.Storage_ID,
			//                Bound_Qty = M.OK_Qty,
			//                Accepter = "",
			//                Applicant_UID = M.Applicant_UID,
			//                Applicant_Date = M.Applicant_Date,
			//                Approver_UID = M.Approver_UID,
			//                Approver_Date = M.Approver_Date,
			//                Storage_Bound_Type = storagebound.Enum_Value,
			//                Storage_Bound_Type_UID = M.Storage_Inbound_Type_UID,
			//                ModifiedUser = appuser.User_Name,
			//                ApproverUser = apruser.User_Name,
			//                Status_UID = M.Status_UID,
			//                Outbound_Account_UID = 0,
			//                PU_NO = M.PU_NO,
			//                Issue_NO = M.Issue_NO,
			//                Repair_id = "",
			//                Warehouse_Storage_UID = M.Warehouse_Storage_UID,
			//                Material_Uid = M.Material_UID,
			//                BG_Organization_UID = war.BG_Organization_UID,
			//                FunPlant_Organization_UID = war.FunPlant_Organization_UID
			//            };
			//List<int> inlist = new List<int>();
			//inlist.Add(413);
			//query = query.Where(m => inlist.Contains(m.Storage_Inbound_Type_UID));
			//var query1 = from M in DataContext.Storage_Outbound_M
			//             join D in DataContext.Storage_Outbound_D on
			//             M.Storage_Outbound_M_UID equals D.Storage_Outbound_M_UID
			//             join statusenum in DataContext.Enumeration on
			//             M.Status_UID equals statusenum.Enum_UID
			//             join bm in DataContext.Material_Info on
			//             D.Material_UID equals bm.Material_Uid
			//             join warst in DataContext.Warehouse_Storage on
			//             D.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
			//             join war in DataContext.Warehouse on
			//             warst.Warehouse_UID equals war.Warehouse_UID
			//             join eqpuser in DataContext.EQP_UserTable on
			//             M.Outbound_Account_UID equals eqpuser.EQPUser_Uid
			//             join typeenum in DataContext.Enumeration on
			//             M.Storage_Outbound_Type_UID equals typeenum.Enum_UID
			//             join appuser in DataContext.System_Users on
			//            M.Applicant_UID equals appuser.Account_UID
			//             join apruser in DataContext.System_Users on
			//             M.Approver_UID equals apruser.Account_UID
			//             join eqpinfo in DataContext.EQPRepair_Info on
			//             M.Repair_Uid equals eqpinfo.Repair_Uid into temp
			//             from aa in temp.DefaultIfEmpty()
			//             select new InOutBoundInfoDTO
			//             {
			//                 Storage_Bound_UID = M.Storage_Outbound_M_UID,
			//                 Storage_Inbound_Type_UID = 0,
			//                 Storage_Outbound_Type_UID = M.Storage_Outbound_Type_UID,
			//                 InOut_Type = "出库单",
			//                 In_Out_Type = typeenum.Enum_Value,
			//                 Storage_Bound_ID = M.Storage_Outbound_ID,
			//                 Status = statusenum.Enum_Value,
			//                 Material_Id = bm.Material_Id,
			//                 Material_Name = bm.Material_Name,
			//                 Material_Types = bm.Material_Types,
			//                 Warehouse_ID = war.Warehouse_ID,
			//                 Name_ZH = war.Name_ZH,
			//                 Rack_ID = warst.Rack_ID,
			//                 Storage_ID = warst.Storage_ID,
			//                 Bound_Qty = 0,
			//                 Accepter = eqpuser.User_Name,
			//                 Applicant_UID = M.Applicant_UID,
			//                 Applicant_Date = M.Applicant_Date,
			//                 Approver_UID = M.Approver_UID,
			//                 Approver_Date = M.Approver_Date,
			//                 Storage_Bound_Type = typeenum.Enum_Value,
			//                 Storage_Bound_Type_UID = M.Storage_Outbound_Type_UID,
			//                 ModifiedUser = appuser.User_Name,
			//                 ApproverUser = apruser.User_Name,
			//                 Status_UID = M.Status_UID,
			//                 Outbound_Account_UID = M.Outbound_Account_UID == null ? 0 : (int)M.Outbound_Account_UID,
			//                 PU_NO = "",
			//                 Issue_NO = "",
			//                 Repair_id = aa.Repair_id,
			//                 Warehouse_Storage_UID = warst.Warehouse_Storage_UID,
			//                 Material_Uid = D.Material_UID,
			//                 BG_Organization_UID = war.BG_Organization_UID,
			//                 FunPlant_Organization_UID = war.FunPlant_Organization_UID
			//             };
			//List<int> outlist = new List<int>();
			//outlist.Add(416);
			//outlist.Add(417);
			//query1 = query1.Where(m => outlist.Contains(m.Storage_Outbound_Type_UID));

			//if (searchModel.Storage_Bound_Type_UID != 0)
			//    query1 = query1.Where(m => m.Storage_Bound_Type_UID == searchModel.Storage_Bound_Type_UID);
			//if (!string.IsNullOrWhiteSpace(searchModel.Storage_Bound_ID))
			//    query1 = query1.Where(m => m.Storage_Bound_ID.Contains(searchModel.Storage_Bound_ID));
			//if (searchModel.Status_UID != 0)
			//    query1 = query1.Where(m => m.Status_UID == searchModel.Status_UID);
			//else
			//    query1 = query1.Where(m => m.Status_UID != 420 & m.Status_UID != 408);
			//if (searchModel.Applicant_Date.Year != 1)
			//{
			//    DateTime nextappdate = searchModel.Applicant_Date.AddDays(1);
			//    query1 = query1.Where(m => m.Applicant_Date >= searchModel.Applicant_Date & m.Applicant_Date < nextappdate);
			//}
			//if (searchModel.Approver_Date.Year != 1)
			//{
			//    DateTime nextaprdate = searchModel.Approver_Date.AddDays(1);
			//    query1 = query1.Where(m => m.Approver_Date >= searchModel.Approver_Date & m.Approver_Date < nextaprdate);
			//}
			//if (searchModel.Outbound_Account_UID != 0)
			//    query1 = query1.Where(m => m.Outbound_Account_UID == searchModel.Outbound_Account_UID);
			//if (!string.IsNullOrWhiteSpace(searchModel.ModifiedUser))
			//    query1 = query1.Where(m => m.ModifiedUser.Contains(searchModel.ModifiedUser));
			//if (!string.IsNullOrWhiteSpace(searchModel.ApproverUser))
			//    query1 = query1.Where(m => m.ApproverUser.Contains(searchModel.ApproverUser));
			//if (!string.IsNullOrWhiteSpace(searchModel.PU_NO))
			//    query1 = query1.Where(m => m.PU_NO == searchModel.PU_NO);
			//if (!string.IsNullOrWhiteSpace(searchModel.Issue_NO))
			//    query1 = query1.Where(m => m.Issue_NO == searchModel.Issue_NO);
			//if (!string.IsNullOrWhiteSpace(searchModel.Repair_id))
			//    query1 = query1.Where(m => m.Repair_id == searchModel.Repair_id);
			//if (searchModel.Warehouse_Storage_UID != 0)
			//    query1 = query1.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);
			//if (searchModel.Material_Uid != 0)
			//    query1 = query1.Where(m => m.Material_Uid == searchModel.Material_Uid);
			//if (searchModel.BG_Organization_UID != 0)
			//    query1 = query1.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
			//if (searchModel.FunPlant_Organization_UID != 0)
			//    query1 = query1.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);

			//query1 = from M in query1
			//         group M by new
			//         {
			//             M.Status_UID,
			//             M.Storage_Bound_ID,
			//             M.Storage_Bound_UID,
			//             M.In_Out_Type,
			//             M.Status,
			//             M.Accepter,
			//             M.Applicant_Date,
			//             M.Approver_Date,
			//             M.ModifiedUser,
			//             M.ApproverUser
			//         } into g
			//         select new InOutBoundInfoDTO
			//         {
			//             Storage_Bound_UID = g.Key.Storage_Bound_UID,
			//             Storage_Inbound_Type_UID = 0,
			//             Storage_Outbound_Type_UID = g.Max(m => m.Storage_Bound_Type_UID),
			//             InOut_Type = "出库单",
			//             In_Out_Type = g.Key.In_Out_Type,
			//             Storage_Bound_ID = g.Key.Storage_Bound_ID,
			//             Status = g.Key.Status,
			//             Material_Id = "",
			//             Material_Name = "",
			//             Material_Types = "",
			//             Warehouse_ID = "",
			//             Name_ZH = "",
			//             Rack_ID = "",
			//             Storage_ID = "",
			//             Bound_Qty = 0,
			//             Accepter = g.Key.Accepter,
			//             Applicant_UID = 0,
			//             Applicant_Date = g.Key.Applicant_Date,
			//             Approver_UID = 0,
			//             Approver_Date = g.Key.Approver_Date,
			//             Storage_Bound_Type = "",
			//             Storage_Bound_Type_UID = g.Max(m => m.Storage_Bound_Type_UID),
			//             ModifiedUser = g.Key.ModifiedUser,
			//             ApproverUser = g.Key.ApproverUser,
			//             Status_UID = g.Key.Status_UID,
			//             Outbound_Account_UID = 0,
			//             PU_NO = "",
			//             Issue_NO = "",
			//             Repair_id = "",
			//             Warehouse_Storage_UID = 0,
			//             Material_Uid = 0,
			//             BG_Organization_UID = 0,
			//             FunPlant_Organization_UID = 0
			//         };

			//query = query.Union(query1);

			//if (searchModel.Storage_Bound_Type_UID != 0)
			//    query = query.Where(m => m.Storage_Bound_Type_UID == searchModel.Storage_Bound_Type_UID);
			//if (!string.IsNullOrWhiteSpace(searchModel.Storage_Bound_ID))
			//    query = query.Where(m => m.Storage_Bound_ID.Contains(searchModel.Storage_Bound_ID));
			//if (searchModel.Status_UID != 0)
			//    query = query.Where(m => m.Status_UID == searchModel.Status_UID);
			//else
			//    query = query.Where(m => m.Status_UID != 420 & m.Status_UID != 408);
			//if (searchModel.Applicant_Date.Year != 1)
			//{
			//    DateTime nextappdate = searchModel.Applicant_Date.AddDays(1);
			//    query = query.Where(m => m.Applicant_Date >= searchModel.Applicant_Date & m.Applicant_Date < nextappdate);
			//}
			//if (searchModel.Approver_Date.Year != 1)
			//{
			//    DateTime nextaprdate = searchModel.Approver_Date.AddDays(1);
			//    query = query.Where(m => m.Approver_Date >= searchModel.Approver_Date & m.Approver_Date < nextaprdate);
			//}
			//if (searchModel.Outbound_Account_UID != 0)
			//    query = query.Where(m => m.Outbound_Account_UID == searchModel.Outbound_Account_UID);
			//if (!string.IsNullOrWhiteSpace(searchModel.ModifiedUser))
			//    query = query.Where(m => m.ModifiedUser.Contains(searchModel.ModifiedUser));
			//if (!string.IsNullOrWhiteSpace(searchModel.ApproverUser))
			//    query = query.Where(m => m.ApproverUser.Contains(searchModel.ApproverUser));
			//if (!string.IsNullOrWhiteSpace(searchModel.PU_NO))
			//    query = query.Where(m => m.PU_NO == searchModel.PU_NO);
			//if (!string.IsNullOrWhiteSpace(searchModel.Issue_NO))
			//    query = query.Where(m => m.Issue_NO == searchModel.Issue_NO);
			//if (!string.IsNullOrWhiteSpace(searchModel.Repair_id))
			//    query = query.Where(m => m.Repair_id == searchModel.Repair_id);
			//if (searchModel.Warehouse_Storage_UID != 0)
			//    query = query.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);
			//if (searchModel.Material_Uid != 0)
			//    query = query.Where(m => m.Material_Uid == searchModel.Material_Uid);
			//if (searchModel.BG_Organization_UID != 0)
			//    query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
			//if (searchModel.FunPlant_Organization_UID != 0)
			//    query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);




			//List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
			//Plant_UIDs.Add(0);
			//if (Plant_UIDs.Count > 0)
			//{
			//    query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
			//}
			var query = from M in DataContext.Storage_Inbound
						join bm in DataContext.Material_Info on
						M.Material_UID equals bm.Material_Uid
						join storage in DataContext.Warehouse_Storage on
						M.Warehouse_Storage_UID equals storage.Warehouse_Storage_UID
						join war in DataContext.Warehouse on
						storage.Warehouse_UID equals war.Warehouse_UID
						join funplant in DataContext.System_Organization on
						war.FunPlant_Organization_UID equals funplant.Organization_UID
						join BoundType in DataContext.Enumeration on
						M.PartType_UID equals BoundType.Enum_UID
						join statusenums in DataContext.Enumeration on
						M.Status_UID equals statusenums.Enum_UID
						join bgorg in DataContext.System_Organization on
						war.BG_Organization_UID equals bgorg.Organization_UID
						join users in DataContext.System_Users on
						M.Applicant_UID equals users.Account_UID
						join storagebound in DataContext.Enumeration on
						M.Storage_Inbound_Type_UID equals storagebound.Enum_UID
						join appuser in DataContext.System_Users on
						M.Applicant_UID equals appuser.Account_UID
						join apruser in DataContext.System_Users on
						M.Approver_UID equals apruser.Account_UID
						select new InOutBoundInfoDTO
						{
							Storage_Bound_UID = M.Storage_Inbound_UID,
							Storage_Inbound_Type_UID = M.Storage_Inbound_Type_UID,
							Storage_Outbound_Type_UID = 0,
							InOut_Type = "入库单",
							In_Out_Type = storagebound.Enum_Value,
							Storage_Bound_ID = M.Storage_Inbound_ID,
							Status = statusenums.Enum_Value,
							Material_Id = bm.Material_Id,
							Material_Name = bm.Material_Name,
							Material_Types = bm.Material_Types,
							Warehouse_ID = war.Warehouse_ID,
							Name_ZH = war.Name_ZH,
							Rack_ID = storage.Rack_ID,
							Storage_ID = storage.Storage_ID,
							Bound_Qty = M.OK_Qty,
							Accepter = "",
							Applicant_UID = M.Applicant_UID,
							Applicant_Date = M.Applicant_Date,
							Approver_UID = M.Approver_UID,
							Approver_Date = M.Approver_Date,
							Storage_Bound_Type = storagebound.Enum_Value,
							Storage_Bound_Type_UID = M.Storage_Inbound_Type_UID,
							ModifiedUser = appuser.User_Name,
							ApproverUser = apruser.User_Name,
							Status_UID = M.Status_UID,
							Outbound_Account_UID = 0,
							PU_NO = M.PU_NO,
							Issue_NO = M.Issue_NO,
							Repair_id = "",
							Warehouse_Storage_UID = M.Warehouse_Storage_UID,
							Material_Uid = M.Material_UID,
							BG_Organization_UID = war.BG_Organization_UID,
							FunPlant_Organization_UID = war.FunPlant_Organization_UID
						};
			List<int> inlist = new List<int>();
			//fky2017/11/13
			//inlist.Add(383);
			inlist.Add(413);
			query = query.Where(m => inlist.Contains(m.Storage_Inbound_Type_UID));
			var query1 = from M in DataContext.Storage_Outbound_M
						 join D in DataContext.Storage_Outbound_D on
						 M.Storage_Outbound_M_UID equals D.Storage_Outbound_M_UID
						 join statusenum in DataContext.Enumeration on
						 M.Status_UID equals statusenum.Enum_UID
						 join bm in DataContext.Material_Info on
						 D.Material_UID equals bm.Material_Uid
						 join warst in DataContext.Warehouse_Storage on
						 D.Warehouse_Storage_UID equals warst.Warehouse_Storage_UID
						 join war in DataContext.Warehouse on
						 warst.Warehouse_UID equals war.Warehouse_UID
						 join eqpuser in DataContext.EQP_UserTable on
						 M.Outbound_Account_UID equals eqpuser.EQPUser_Uid
						 join typeenum in DataContext.Enumeration on
						 M.Storage_Outbound_Type_UID equals typeenum.Enum_UID
						 join appuser in DataContext.System_Users on
						M.Applicant_UID equals appuser.Account_UID
						 join apruser in DataContext.System_Users on
						 M.Approver_UID equals apruser.Account_UID
						 join eqpinfo in DataContext.EQPRepair_Info on
						 M.Repair_Uid equals eqpinfo.Repair_Uid into temp
						 from aa in temp.DefaultIfEmpty()
						 select new InOutBoundInfoDTO
						 {
							 Storage_Bound_UID = M.Storage_Outbound_M_UID,
							 Storage_Inbound_Type_UID = 0,
							 Storage_Outbound_Type_UID = M.Storage_Outbound_Type_UID,
							 InOut_Type = "出库单",
							 In_Out_Type = typeenum.Enum_Value,
							 Storage_Bound_ID = M.Storage_Outbound_ID,
							 Status = statusenum.Enum_Value,
							 Material_Id = bm.Material_Id,
							 Material_Name = bm.Material_Name,
							 Material_Types = bm.Material_Types,
							 Warehouse_ID = war.Warehouse_ID,
							 Name_ZH = war.Name_ZH,
							 Rack_ID = warst.Rack_ID,
							 Storage_ID = warst.Storage_ID,
							 Bound_Qty = 0,
							 Accepter = eqpuser.User_Name,
							 Applicant_UID = M.Applicant_UID,
							 Applicant_Date = M.Applicant_Date,
							 Approver_UID = M.Approver_UID,
							 Approver_Date = M.Approver_Date,
							 Storage_Bound_Type = typeenum.Enum_Value,
							 Storage_Bound_Type_UID = M.Storage_Outbound_Type_UID,
							 ModifiedUser = appuser.User_Name,
							 ApproverUser = apruser.User_Name,
							 Status_UID = M.Status_UID,
							 Outbound_Account_UID = M.Outbound_Account_UID == null ? 0 : (int)M.Outbound_Account_UID,
							 PU_NO = "",
							 Issue_NO = "",
							 Repair_id = aa.Repair_id,
							 Warehouse_Storage_UID = warst.Warehouse_Storage_UID,
							 Material_Uid = D.Material_UID,
							 BG_Organization_UID = war.BG_Organization_UID,
							 FunPlant_Organization_UID = war.FunPlant_Organization_UID
						 };
			List<int> outlist = new List<int>();
			//fky2017/11/13
			// outlist.Add(387);
			outlist.Add(416);
			//fky2017/11/13
			// outlist.Add(388);
			outlist.Add(417);
			query1 = query1.Where(m => outlist.Contains(m.Storage_Outbound_Type_UID));

			if (searchModel.Storage_Bound_Type_UID != 0)
				query1 = query1.Where(m => m.Storage_Bound_Type_UID == searchModel.Storage_Bound_Type_UID);
			if (!string.IsNullOrWhiteSpace(searchModel.Storage_Bound_ID))
				query1 = query1.Where(m => m.Storage_Bound_ID.Contains(searchModel.Storage_Bound_ID));



			if (searchModel.Status_UID != 0)
				query1 = query1.Where(m => m.Status_UID == searchModel.Status_UID);
			else
				//fky2017/11/13
				// query1 = query1.Where(m => m.Status_UID != 392 & m.Status_UID != 376);
				query1 = query1.Where(m => m.Status_UID != 420 & m.Status_UID != 408);
			if (searchModel.Applicant_Date.Year != 1)
			{
				DateTime nextappdate = searchModel.Applicant_Date.AddDays(1);
				query1 = query1.Where(m => m.Applicant_Date >= searchModel.Applicant_Date & m.Applicant_Date < nextappdate);
			}
			if (searchModel.Approver_Date.Year != 1)
			{
				DateTime nextaprdate = searchModel.Approver_Date.AddDays(1);
				query1 = query1.Where(m => m.Approver_Date >= searchModel.Approver_Date & m.Approver_Date < nextaprdate);
			}
			if (searchModel.Outbound_Account_UID != 0)
				query1 = query1.Where(m => m.Outbound_Account_UID == searchModel.Outbound_Account_UID);
			if (!string.IsNullOrWhiteSpace(searchModel.ModifiedUser))
				query1 = query1.Where(m => m.ModifiedUser.Contains(searchModel.ModifiedUser));
			if (!string.IsNullOrWhiteSpace(searchModel.ApproverUser))
				query1 = query1.Where(m => m.ApproverUser.Contains(searchModel.ApproverUser));
			if (!string.IsNullOrWhiteSpace(searchModel.PU_NO))
				query1 = query1.Where(m => m.PU_NO == searchModel.PU_NO);
			if (!string.IsNullOrWhiteSpace(searchModel.Issue_NO))
				query1 = query1.Where(m => m.Issue_NO == searchModel.Issue_NO);
			if (!string.IsNullOrWhiteSpace(searchModel.Repair_id))
				query1 = query1.Where(m => m.Repair_id == searchModel.Repair_id);
			if (searchModel.Warehouse_Storage_UID != 0)
				query1 = query1.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);
			if (searchModel.Material_Uid != 0)
				query1 = query1.Where(m => m.Material_Uid == searchModel.Material_Uid);
			if (searchModel.BG_Organization_UID != 0)
				query1 = query1.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
			if (searchModel.FunPlant_Organization_UID != 0)
				query1 = query1.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
			//List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
			//if (Plant_UIDs.Count > 0)
			//{
			//    query1 = query1.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
			//}


			query1 = from M in query1
					 group M by new
					 {
						 M.Status_UID,
						 M.Storage_Bound_ID,
						 M.Storage_Bound_UID,
						 M.In_Out_Type,
						 M.Status,
						 M.Accepter,
						 M.Applicant_Date,
						 M.Approver_Date,
						 M.ModifiedUser,
						 M.ApproverUser
					 } into g
					 select new InOutBoundInfoDTO
					 {
						 Storage_Bound_UID = g.Key.Storage_Bound_UID,
						 Storage_Inbound_Type_UID = 0,
						 Storage_Outbound_Type_UID = g.Max(m => m.Storage_Bound_Type_UID),
						 InOut_Type = "出库单",
						 In_Out_Type = g.Key.In_Out_Type,
						 Storage_Bound_ID = g.Key.Storage_Bound_ID,
						 Status = g.Key.Status,
						 Material_Id = "",
						 Material_Name = "",
						 Material_Types = "",
						 Warehouse_ID = "",
						 Name_ZH = "",
						 Rack_ID = "",
						 Storage_ID = "",
						 Bound_Qty = 0,
						 Accepter = g.Key.Accepter,
						 Applicant_UID = 0,
						 Applicant_Date = g.Key.Applicant_Date,
						 Approver_UID = 0,
						 Approver_Date = g.Key.Approver_Date,
						 Storage_Bound_Type = "",
						 Storage_Bound_Type_UID = g.Max(m => m.Storage_Bound_Type_UID),
						 ModifiedUser = g.Key.ModifiedUser,
						 ApproverUser = g.Key.ApproverUser,
						 Status_UID = g.Key.Status_UID,
						 Outbound_Account_UID = 0,
						 PU_NO = "",
						 Issue_NO = "",
						 Repair_id = "",
						 Warehouse_Storage_UID = 0,
						 Material_Uid = 0,
						 BG_Organization_UID = 0,
						 FunPlant_Organization_UID = 0
					 };

			query = query.Union(query1);

			if (searchModel.Storage_Bound_Type_UID != 0)
				query = query.Where(m => m.Storage_Bound_Type_UID == searchModel.Storage_Bound_Type_UID);
			if (!string.IsNullOrWhiteSpace(searchModel.Storage_Bound_ID))
				query = query.Where(m => m.Storage_Bound_ID.Contains(searchModel.Storage_Bound_ID));
			if (searchModel.Status_UID != 0)
				query = query.Where(m => m.Status_UID == searchModel.Status_UID);
			else
				//fky2017/11/13
				// query = query.Where(m => m.Status_UID != 392&m.Status_UID!=376);
				query = query.Where(m => m.Status_UID != 420 & m.Status_UID != 408);
			if (searchModel.Applicant_Date.Year != 1)
			{
				DateTime nextappdate = searchModel.Applicant_Date.AddDays(1);
				query = query.Where(m => m.Applicant_Date >= searchModel.Applicant_Date & m.Applicant_Date < nextappdate);
			}
			if (searchModel.Approver_Date.Year != 1)
			{
				DateTime nextaprdate = searchModel.Approver_Date.AddDays(1);
				query = query.Where(m => m.Approver_Date >= searchModel.Approver_Date & m.Approver_Date < nextaprdate);
			}
			if (searchModel.Outbound_Account_UID != 0)
				query = query.Where(m => m.Outbound_Account_UID == searchModel.Outbound_Account_UID);
			if (!string.IsNullOrWhiteSpace(searchModel.ModifiedUser))
				query = query.Where(m => m.ModifiedUser.Contains(searchModel.ModifiedUser));
			if (!string.IsNullOrWhiteSpace(searchModel.ApproverUser))
				query = query.Where(m => m.ApproverUser.Contains(searchModel.ApproverUser));
			if (!string.IsNullOrWhiteSpace(searchModel.PU_NO))
				query = query.Where(m => m.PU_NO == searchModel.PU_NO);
			if (!string.IsNullOrWhiteSpace(searchModel.Issue_NO))
				query = query.Where(m => m.Issue_NO == searchModel.Issue_NO);
			if (!string.IsNullOrWhiteSpace(searchModel.Repair_id))
				query = query.Where(m => m.Repair_id == searchModel.Repair_id);
			if (searchModel.Warehouse_Storage_UID != 0)
				query = query.Where(m => m.Warehouse_Storage_UID == searchModel.Warehouse_Storage_UID);
			if (searchModel.Material_Uid != 0)
				query = query.Where(m => m.Material_Uid == searchModel.Material_Uid);
			if (searchModel.BG_Organization_UID != 0)
				query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
			if (searchModel.FunPlant_Organization_UID != 0)
				query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);

			List<int> Plant_UIDs = GetOpType(searchModel.Plant_UID).Select(o => o.Organization_UID).ToList();
			Plant_UIDs.Add(0);
			if (Plant_UIDs.Count > 0)
			{
				query = query.Where(m => Plant_UIDs.Contains(m.BG_Organization_UID));
			}
			return query.ToList();
		}
		public List<SystemOrgDTO> GetOpType(int plantorguid)
		{
			var sqlStr = @"SELECT * FROM dbo.System_Organization t1 INNER JOIN dbo.System_OrganizationBOM t2
										ON t1.Organization_UID=t2.ChildOrg_UID WHERE  t1.Organization_ID LIKE'%2000%' ";
			if (plantorguid != 0)
			{
				sqlStr = @"SELECT * FROM dbo.System_Organization t1 INNER JOIN dbo.System_OrganizationBOM t2
										ON t1.Organization_UID=t2.ChildOrg_UID WHERE  t1.Organization_ID LIKE'%2000%'  and  t2.ParentOrg_UID={0}";
				sqlStr = string.Format(sqlStr, plantorguid);
			}

			var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList();
			return dbList;
		}
		public List<StorageInboundDTO> GetWarSt(int inboundtype, int plantuid)
		{
			string sql = @"SELECT t1.Warehouse_UID,Warehouse_ID,Rack_ID,Storage_ID,Warehouse_Storage_UID ,t1.BG_Organization_UID,t1.FunPlant_Organization_UID FROM 
								Warehouse t1 INNER JOIN Warehouse_Storage t2 on t1.Warehouse_UID=t2.Warehouse_UID
								inner join Enumeration t3 on t1.Warehouse_Type_UID=t3.Enum_UID 
								inner join System_OrganizationBOM t4 on t1.BG_Organization_UID=t4.ChildOrg_UID ";
			//fky2017/11/13
			//  if (inboundtype == 407)
			if (inboundtype == 432)
			{
				//fky2017/11/13
				//sql += " where Warehouse_Type_UID=371";
				sql += " where Warehouse_Type_UID=405";
			}
			else if (inboundtype==0)
			{
		 
			}
			else
			{
				//fky2017/11/13
				// sql += " where Warehouse_Type_UID=398";
				sql += " where Warehouse_Type_UID=418";
			}
			if (plantuid != 0)
			{
				sql += " and t4.ParentOrg_UID=" + plantuid;
			}
			var dblist = DataContext.Database.SqlQuery<StorageInboundDTO>(sql).ToList();
			return SetBGandFunplant(dblist);
		}

		public List<StorageInboundDTO> GetWarStByKey(int inboundtype,int warStUid, string key, int plantuid)
		{
			string sql = @"SELECT t1.Warehouse_UID,Warehouse_ID,Rack_ID,Storage_ID,Warehouse_Storage_UID 
								 ,t1.BG_Organization_UID,t5.Organization_Name as BG,t1.FunPlant_Organization_UID,t6.Organization_Name as Funplant 
							FROM Warehouse t1 
							inner join Warehouse_Storage t2 on t1.Warehouse_UID=t2.Warehouse_UID
							inner join Enumeration t3 on t1.Warehouse_Type_UID=t3.Enum_UID 
							inner join System_OrganizationBOM t4 on t1.BG_Organization_UID=t4.ChildOrg_UID 
							inner join System_Organization t5 on t5.Organization_UID = t1.BG_Organization_UID 
							inner join System_Organization t6 on t6.Organization_UID = t1.FunPlant_Organization_UID ";
			if (inboundtype == 432)
			{
				//fky2017/11/13
				//sql += " where Warehouse_Type_UID=371";
				sql += " where Warehouse_Type_UID=405";
			}
			else if (inboundtype != 0)
			{
				sql += " where Warehouse_Type_UID=418";
			}
			if (warStUid > 0)
			{
				sql += " and Warehouse_Storage_UID=" + warStUid;
			}
			if (!string.IsNullOrEmpty(key)) {
				//效能考量不開放BG及FunPlant的關鍵字查詢
				sql += string.Format(@" and (Warehouse_ID like '%{0}%' or Rack_ID like '%{0}%' or Storage_ID like '%{0}%')", key);//or t5.Organization_Name like '%{0}%' or t6.Organization_Name like '%{0}%')"
			}
			if (plantuid != 0)
			{
				sql += " and t4.ParentOrg_UID=" + plantuid;
			}
			var dblist = DataContext.Database.SqlQuery<StorageInboundDTO>(sql).ToList();
			return dblist;
		}

		public List<StorageInboundDTO> SetBGandFunplant(List<StorageInboundDTO> MaterialInventoryDTOs)
		{
			var System_Organizations = DataContext.System_Organization.ToList();
			foreach (var item in MaterialInventoryDTOs)
			{
				item.BG = System_Organizations.FirstOrDefault(o => o.Organization_UID == item.BG_Organization_UID).Organization_Name;
				item.Funplant = System_Organizations.FirstOrDefault(o => o.Organization_UID == item.FunPlant_Organization_UID).Organization_Name;
			}
			return MaterialInventoryDTOs;
		}


		public List<string> GetFunplantByUser(int userid)
		{
			string sql = @"select t2.Organization_Name from System_UserOrg t1 inner join System_Organization t2 
										on t1.Funplant_OrganizationUid=t2.Organization_UID where Account_UID={0}";
			sql = string.Format(sql, userid);
			var dblist = DataContext.Database.SqlQuery<string>(sql).ToList();
			return dblist;
		}
		public List<StorageInboundDTO> GetAllInfo()
		{
			string sql = @"select distinct t1.Material_Uid,Material_Id,Material_Name,Material_Types 
									from Material_Inventory t1 inner join Material_Info t2 on t1.Material_Uid=t2.Material_Uid";
			var dblist = DataContext.Database.SqlQuery<StorageInboundDTO>(sql).ToList();
			return dblist;
		}
		public List<StorageInboundDTO> GetPuinfo(string PU_NO, int Storage_Inbound_UID)
		{
            //TODO 2019.3.4 by steven 修正状态码 审核状态Status_UID=3.已删除（420）也不包含，在取采购单前次采购数量
            var sqlStr = @"select MAX(PU_Qty) PU_Qty,sum(OK_Qty) OK_Qty from 
										Storage_Inbound where PU_NO='{0}' and Storage_Inbound_UID<>{1} and Status_UID=420 group by	PU_NO";
			sqlStr = string.Format(sqlStr, PU_NO, Storage_Inbound_UID);
			var dbList = DataContext.Database.SqlQuery<StorageInboundDTO>(sqlStr).ToList();
			return dbList;
		}

		public string InsertInBoundsWithMAP(List<MaterialInfoDTO> mList, List<StorageInboundDTO> addList)
		{
			string result = "";

			//取得目前最新單號
			string preInbound = "In" + DateTime.Today.ToString("yyyyMMdd");
			var maxPro = DataContext.Storage_Inbound.Where(m => m.Storage_Inbound_ID.StartsWith(preInbound)).Count();

			using (var trans = DataContext.Database.BeginTransaction())
			{
				try
				{
					//處理材料信息
					foreach (var mDto in mList)
					{
						var item = DataContext.Material_Info.Find(mDto.Material_Uid);
						if (item == null)
						{
							throw new Exception("料号[" + mDto.Material_Id + "]查无资料");
						}
						item.Unit_Price = mDto.Unit_Price;
						item.Last_Qty = mDto.Last_Qty;
					}
					//DataContext.SaveChanges();
					//處理入庫申請單
					foreach (var dto in addList)
					{
						//取得物料信息(含移動成本)
						var mDto = mList.FirstOrDefault(m => m.Material_Id == dto.Material_Id);

						maxPro++;//單號+1
						string proInbound = (maxPro).ToString("0000");
						Storage_Inbound item = new Storage_Inbound();
						item.Storage_Inbound_ID = preInbound + proInbound;
						item.Storage_Inbound_Type_UID = 413;
						item.Status_UID = 407;
						item.Inbound_Price = mDto.Unit_Price;//移動成本
						item.Current_POPrice = dto.Current_POPrice;
						item.PU_NO = dto.CoupaPO_ID;
						item.PU_Qty = dto.PU_Qty;
						item.OK_Qty = dto.OK_Qty;
						item.Be_Check_Qty = 0;
						item.NG_Qty = 0;
						item.Issue_NO = "None";
						item.Desc = dto.Desc;
						item.Material_UID = mDto.Material_Uid;//材料信息檔
						item.Warehouse_Storage_UID = mDto.Warehouse_Storage_UID ?? 0;//材料信息檔
						item.PartType_UID = 432;//正常料良品
						item.Applicant_UID = dto.Applicant_UID;
						item.Applicant_Date = dto.Applicant_Date;
						item.Approver_UID = dto.Applicant_UID;
						item.Approver_Date = dto.Applicant_Date;
						DataContext.Storage_Inbound.Add(item);
					}
					DataContext.SaveChanges();
					trans.Commit();
				}
				catch (Exception ex)
				{
					trans.Rollback();
					result = "Error:" + ex.Message;
				}
			}
			return result;
		}

		public string UpdateInBoundsWithMAP(StorageInboundDTO dto)
		{
			string result = "0";
			using (var trans = DataContext.Database.BeginTransaction())
			{
				try
				{
					var inbound = DataContext.Storage_Inbound.Find(dto.Storage_Inbound_UID);
					//處理材料信息                    
					var item = DataContext.Material_Info.Find(inbound.Material_UID);
					if (item != null)
					{
						//計算移動成本
						decimal? newPrcie = 0;
						decimal? newQty = 0;                        
						//若修改前有值，則扣除原數量、金額重算移動成本
						if (!string.IsNullOrEmpty(inbound.PU_NO)) {
							newQty = item.Last_Qty - inbound.PU_Qty;
							if (newQty > 0)
							{
								newPrcie = ((item.Last_Qty * item.Unit_Price) - (inbound.PU_Qty * inbound.Current_POPrice)) / newQty;
							}
							item.Unit_Price = newPrcie;
							item.Last_Qty = newQty;
						}
						//若修改後有值，則加入編輯的數量、金額重算移動成本
						if (!string.IsNullOrEmpty(dto.PU_NO)) {
							newQty = item.Last_Qty + dto.PU_Qty;
							if (newQty > 0)
							{
								newPrcie = ((item.Last_Qty * item.Unit_Price) + (dto.PU_Qty * inbound.Current_POPrice)) / newQty;
							}
							item.Unit_Price = newPrcie;
							item.Last_Qty = newQty;
						}
					}

					//處理入庫申請單
					inbound.PU_NO = dto.PU_NO;
					inbound.PU_Qty = dto.PU_Qty;
					inbound.Issue_NO = dto.Issue_NO;
					inbound.NG_Qty = dto.NG_Qty;
					inbound.PartType_UID = dto.PartType_UID;
					inbound.Material_UID = dto.Material_Uid;
					inbound.Be_Check_Qty = dto.Be_Check_Qty;
					inbound.OK_Qty = dto.OK_Qty;
					inbound.Warehouse_Storage_UID = dto.Warehouse_Storage_UID;
					DataContext.SaveChanges();
					trans.Commit();
				}
				catch (Exception ex)
				{
					trans.Rollback();
					result = "Error:" + ex.Message;
				}
			}
			return result;
		}

		public string DeleteInBoundsWithMAP(StorageInboundDTO dto)
		{
			string result = "";
			using (var trans = DataContext.Database.BeginTransaction())
			{
				try
				{
					var inbound = DataContext.Storage_Inbound.Find(dto.Storage_Inbound_UID);
					//處理材料信息                    
					var item = DataContext.Material_Info.Find(inbound.Material_UID);
					if (item != null)
					{
						//計算移動成本
						decimal? newPrcie = 0;
						decimal? newQty = item.Last_Qty - inbound.PU_Qty;
						if (newQty > 0)
						{
							newPrcie = ((item.Last_Qty * item.Unit_Price) - (inbound.PU_Qty * inbound.Current_POPrice)) / newQty;
						}
						item.Unit_Price = newPrcie;
						item.Last_Qty = newQty;
					}

					//處理入庫申請單
					inbound.Applicant_Date = dto.Applicant_Date;
					inbound.Applicant_UID = dto.Applicant_UID;
					inbound.Status_UID = dto.Status_UID;
					DataContext.SaveChanges();
					trans.Commit();
				}
				catch (Exception ex)
				{
					trans.Rollback();
					result = "Error:" + ex.Message;
				}
			}
			return result;
		}
	}
}
