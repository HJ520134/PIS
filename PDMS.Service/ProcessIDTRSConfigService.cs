using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.ProcessIDTraConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PDMS.Service
{
    public class ProcessIDTRSConfigService : IProcessIDTRSConfigService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEnumerationRepository EnumerationRepository;
        private readonly IProcessIDTRSConfigRepository ProcessIDTRSConfigRepository;
        private readonly IProductInputRepository ProductInputRepository;
        private readonly IMES_PIS_SyncFailedRecordRepository mES_PIS_SyncFailedRecordReposity;
        private readonly IFlowChartDetailRepository FlowChartDetailRepository;

        public ProcessIDTRSConfigService(IUnitOfWork unitOfWork,
            IEnumerationRepository EnumerationRepository,
            IProcessIDTRSConfigRepository ProcessIDTRSConfigRepository,
            IProductInputRepository ProductInputRepository,
            IMES_PIS_SyncFailedRecordRepository mES_PIS_SyncFailedRecordReposity,
            IFlowChartDetailRepository flowChartDetailRepository
            )
        {
            this.unitOfWork = unitOfWork;
            this.EnumerationRepository = EnumerationRepository;
            this.ProductInputRepository = ProductInputRepository;
            this.ProcessIDTRSConfigRepository = ProcessIDTRSConfigRepository;
            this.mES_PIS_SyncFailedRecordReposity = mES_PIS_SyncFailedRecordReposity;
            this.FlowChartDetailRepository = flowChartDetailRepository;
        }

        public string AddProcessIDConfigInfo(List<ProcessIDTransformConfigDTO> AddData)
        {
            List<ProcessIDTransformConfig> dtoList = AutoMapper.Mapper.Map<List<ProcessIDTransformConfig>>(AddData);
            var result = ProcessIDTRSConfigRepository.AddProcessIDConfigInfo(dtoList);
            return result;
        }

        public string AddOrEditProcessInfo(ProcessIDTransformConfigDTO editModel)
        {
            var result = ProcessIDTRSConfigRepository.AddOrEditProcessInfo(editModel);
            return result;
        }

        /// <summary>
        /// 返回true:存在
        /// </summary>
        /// <param name="ParamList"></param>
        /// <returns></returns>
        public bool IsExist(List<ProcessIDTransformConfigDTO> ParamList)
        {
            var dateList = ParamList.Select(m => m.PIS_ProcessID).ToList();
            var result = ProcessIDTRSConfigRepository.GetMany(p => dateList.Contains(p.PIS_ProcessID));

            if (result != null && result.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public PagedListModel<ProcessIDTransformConfigDTO> GetProcessIDConfigData(ProcessIDTransformConfigDTO serchModel, Page page)
        {
            var result = ProcessIDTRSConfigRepository.GetAll();
            List<ProcessIDTransformConfigDTO> dtoList = AutoMapper.Mapper.Map<List<ProcessIDTransformConfigDTO>>(result);
            if (serchModel.Binding_Seq != 0)
            {
                dtoList = dtoList.Where(p => p.Binding_Seq == serchModel.Binding_Seq).ToList();
            }

            if (serchModel.PIS_ProcessID != 0)
            {
                dtoList = dtoList.Where(p => p.PIS_ProcessID == serchModel.PIS_ProcessID).ToList();
            }

            if (!string.IsNullOrEmpty(serchModel.PIS_ProcessName))
            {
                dtoList = dtoList.Where(p => p.PIS_ProcessName == serchModel.PIS_ProcessName).ToList();
            }

            if (!string.IsNullOrEmpty(serchModel.MES_NgID))
            {
                dtoList = dtoList.Where(p => p.MES_NgID == serchModel.MES_NgID).ToList();
            }

            if (!string.IsNullOrEmpty(serchModel.MES_PickingID))
            {
                dtoList = dtoList.Where(p => p.MES_PickingID == serchModel.MES_PickingID).ToList();
            }

            if (!string.IsNullOrEmpty(serchModel.MES_GoodProductID))
            {
                dtoList = dtoList.Where(p => p.MES_GoodProductID == serchModel.MES_GoodProductID).ToList();
            }

            if (!string.IsNullOrEmpty(serchModel.MES_ReworkID))
            {
                dtoList = dtoList.Where(p => p.MES_ReworkID == serchModel.MES_ReworkID).ToList();
            }
            if (!string.IsNullOrEmpty(serchModel.Color))
            {
                dtoList = dtoList.Where(p => p.Color == serchModel.Color).ToList();
            }

            if (serchModel.Modified_UID != 0)
            {
                dtoList = dtoList.Where(p => p.Modified_UID == serchModel.Modified_UID).ToList();
            }

            if (serchModel.VM_IsEnabled != "全部" && !string.IsNullOrEmpty(serchModel.VM_IsEnabled))
            {
                serchModel.IsEnabled = serchModel.VM_IsEnabled == "1" ? true : false;
                dtoList = dtoList.Where(p => p.IsEnabled == serchModel.IsEnabled).ToList();
            }

            if (serchModel.VM_IsSyncNG != "全部" && !string.IsNullOrEmpty(serchModel.VM_IsSyncNG))
            {
                serchModel.IsSyncNG = serchModel.VM_IsSyncNG == "1" ? true : false;
                dtoList = dtoList.Where(p => p.IsSyncNG == serchModel.IsSyncNG).ToList();
            }

            var totalCount = dtoList.Count();
            var resultList = dtoList.Skip(page.PageSize * page.PageNumber).Take(page.PageSize).ToList();
            return new PagedListModel<ProcessIDTransformConfigDTO>(totalCount, resultList);
        }

        public bool AddMESProcessIDInfo(List<ProcessIDTransformConfigDTO> AddData)
        {
            List<ProcessIDTransformConfig> dtoList = AutoMapper.Mapper.Map<List<ProcessIDTransformConfig>>(AddData);
            ProcessIDTRSConfigRepository.AddList(dtoList);
            unitOfWork.Commit();
            return true;
        }

        public List<ProcessIDTransformConfigDTO> GetAll()
        {
            var result = ProcessIDTRSConfigRepository.GetAll();
            List<ProcessIDTransformConfigDTO> dtoList = AutoMapper.Mapper.Map<List<ProcessIDTransformConfigDTO>>(result);
            return dtoList;
        }

        /// <summary>
        /// 获取制程配置信息ByUID
        /// </summary>
        /// <param name="Process_UID"></param>
        /// <returns></returns>
        public ProcessIDTransformConfigDTO GetProcessDataByUID(int Process_UID)
        {
            var resultList = ProcessIDTRSConfigRepository.GetProcessDataByUID(Process_UID);
            return resultList;
        }


        /// <summary>
        /// 获取需要的同步的数据
        /// </summary>
        /// <returns></returns>
        public List<ProcessIDTransformConfigDTO> GetSyscProcessConfig()
        {
            var resultList = ProcessIDTRSConfigRepository.GetSyscProcessConfig();
            return resultList;
        }

        /// <summary>
        /// 导出全部的配置信息
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public List<ProcessIDTransformConfigDTO> ExportAllProcessInfo(ProcessIDTransformConfigDTO serchModel)
        {
            var result = ProcessIDTRSConfigRepository.GetAll();
            List<ProcessIDTransformConfigDTO> dtoList = AutoMapper.Mapper.Map<List<ProcessIDTransformConfigDTO>>(result);
            if (serchModel.Binding_Seq != 0)
            {
                dtoList = dtoList.Where(p => p.Binding_Seq == serchModel.Binding_Seq).ToList();
            }

            if (serchModel.PIS_ProcessID != 0)
            {
                dtoList = dtoList.Where(p => p.PIS_ProcessID == serchModel.PIS_ProcessID).ToList();
            }

            if (!string.IsNullOrEmpty(serchModel.PIS_ProcessName))
            {
                dtoList = dtoList.Where(p => p.PIS_ProcessName == serchModel.PIS_ProcessName).ToList();
            }

            if (!string.IsNullOrEmpty(serchModel.MES_NgID))
            {
                dtoList = dtoList.Where(p => p.MES_NgID == serchModel.MES_NgID).ToList();
            }

            if (!string.IsNullOrEmpty(serchModel.MES_PickingID))
            {
                dtoList = dtoList.Where(p => p.MES_PickingID == serchModel.MES_PickingID).ToList();
            }

            if (!string.IsNullOrEmpty(serchModel.MES_GoodProductID))
            {
                dtoList = dtoList.Where(p => p.MES_GoodProductID == serchModel.MES_GoodProductID).ToList();
            }


            if (!string.IsNullOrEmpty(serchModel.MES_ReworkID))
            {
                dtoList = dtoList.Where(p => p.MES_ReworkID == serchModel.MES_ReworkID).ToList();
            }
            if (!string.IsNullOrEmpty(serchModel.Color))
            {
                dtoList = dtoList.Where(p => p.Color == serchModel.Color).ToList();
            }

            if (serchModel.Modified_UID != 0)
            {
                dtoList = dtoList.Where(p => p.Modified_UID == serchModel.Modified_UID).ToList();
            }

            if (serchModel.VM_IsEnabled != "全部" && !string.IsNullOrEmpty(serchModel.VM_IsEnabled))
            {
                serchModel.IsEnabled = serchModel.VM_IsEnabled == "1" ? true : false;
                dtoList = dtoList.Where(p => p.IsEnabled == serchModel.IsEnabled).ToList();
            }

            if (serchModel.VM_IsSyncNG != "全部" && !string.IsNullOrEmpty(serchModel.VM_IsSyncNG))
            {
                serchModel.IsSyncNG = serchModel.VM_IsSyncNG == "1" ? true : false;
                dtoList = dtoList.Where(p => p.IsSyncNG == serchModel.IsSyncNG).ToList();
            }

            return dtoList.OrderBy(p => p.Binding_Seq).ToList();
        }

        /// <summary>
        /// 导出部分的配置信息
        /// </summary>
        /// <param name="uids"></param>
        /// <returns></returns>
        public List<ProcessIDTransformConfigDTO> ExportPartProcessInfo(string uids)
        {
            uids = "," + uids + ",";
            var result = ProcessIDTRSConfigRepository.GetAll();
            List<ProcessIDTransformConfigDTO> dtoList = AutoMapper.Mapper.Map<List<ProcessIDTransformConfigDTO>>(result);
            dtoList = dtoList.Where(m => uids.Contains("," + m.ProcessTransformConfig_UID + ",")).ToList();
            return dtoList.OrderBy(p => p.Binding_Seq).ToList();
        }

        /// <summary>
        /// 通过uid 日期，时段获取数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeInterval"></param>
        /// <param name="flowChartDetailUID"></param>
        /// <returns></returns>
        public QAMasterVM GetProcessDataById(string date, string timeInterval, int flowChartDetailUID)
        {
            var mesPickList = ProcessIDTRSConfigRepository.GetProcessDataById(date, timeInterval, flowChartDetailUID, "PIS-PICK");
            var mesNgList = ProcessIDTRSConfigRepository.GetProcessDataById(date, timeInterval, flowChartDetailUID, "PIS-NG");

            QAMasterVM model = new QAMasterVM();
            if (mesPickList.Count() > 0 && mesNgList.Count() > 0)
            {
                var pisPickNum = mesPickList.Sum(q => q.ProductQuantity);
                model.Input = pisPickNum;
                var pisNgNum = mesNgList.Sum(q => q.ProductQuantity);
                model.NG_Qty = pisNgNum;
            }
            else if (mesPickList.Count() > 0 && mesNgList.Count() == 0)
            {
                var pisPickNum = mesPickList.Sum(q => q.ProductQuantity);
                model.Input = pisPickNum;
                model.NG_Qty = null;
            }
            else if (mesPickList.Count() == 0 && mesNgList.Count() > 0)
            {
                var pisNgNum = mesNgList.Sum(q => q.ProductQuantity);
                model.Input = null;
                model.NG_Qty = pisNgNum;
            }
            else
            {
                model.Input = null;
                model.NG_Qty = null;
            }

            return model;
        }

        public string SynchronizeMesInfo(MesSyncParam syncParam)
        {
            try
            {
                var GP_Dic = new Dictionary<int, int>();
                var PICK_Dic = new Dictionary<int, int>();
                var NG_Dic = new Dictionary<int, int>();

                var all_GP_Dic = new Dictionary<int, int>();
                var all_PICK_Dic = new Dictionary<int, int>();
                var all_NG_Dic = new Dictionary<int, int>();
                Product_Input paramModel = new Product_Input();
                bool kkflag = true;

                //获取所有的启用的配置
                var ProcessConfigList = ProcessIDTRSConfigRepository.GetSyscProcessConfig();
                ProcessConfigList = ProcessConfigList.Where(p => p.FlowChart_Master_UID == syncParam.FlowChartMaster_UID).ToList();
                var configDic = ProcessConfigList.GroupBy(p => p.PIS_ProcessID).ToDictionary(p => p.Key, m => m);
                var PickingConfigList = ProcessConfigList.Where(m => m.MES_PickingID != "0").Select(p => p.PIS_ProcessID);
                var GPgConfigList = ProcessConfigList.Where(m => m.MES_GoodProductID != "0").Select(p => p.PIS_ProcessID);
                var NGgConfigList = ProcessConfigList.Where(m => m.MES_NgID != "0").Select(p => p.PIS_ProcessID);
                //pick启用的配置
                foreach (var item in PickingConfigList)
                {
                    if (!all_PICK_Dic.Keys.Contains(item))
                    {
                        all_PICK_Dic.Add(item, 0);
                    }
                }
                //GP启用的配置
                foreach (var item in GPgConfigList)
                {
                    if (!all_GP_Dic.Keys.Contains(item))
                    {
                        all_GP_Dic.Add(item, 0);
                    }
                }

                //NG启用的配置
                foreach (var item in NGgConfigList)
                {
                    if (!all_NG_Dic.Keys.Contains(item))
                    {
                        all_NG_Dic.Add(item, 0);
                    }
                }

                //所有同步的制程
                var all_Sync_GP_Dic = all_GP_Dic;
                var all_Sync_PICK_Dic = all_PICK_Dic;
                #region 同步更新
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
new TimeSpan(0, 6, 0)))
                {
                    //通过日期时段
                    //1 获取Mes临时表数据
                    //2 匹配ProductInput表的数据
                    //3 更新字段
                    var mesList = ProcessIDTRSConfigRepository.GteMesDataInfoByDate(syncParam.currentDate, syncParam.currentInterval, syncParam.FlowChartMaster_UID);
                    var sumMesProcessList = mesList.GroupBy(p => p.PIS_ProcessID).ToDictionary(m => m.Key, n => n.ToList());
                    List<MES_StationDataRecord> MesModelList = new List<MES_StationDataRecord>();
                    var FirstModel = sumMesProcessList.FirstOrDefault();
                    Product_Input inputFirstModel = new Product_Input();
                    inputFirstModel.Product_Date = Convert.ToDateTime(syncParam.currentDate);
                    inputFirstModel.Time_Interval = syncParam.currentInterval;
                    inputFirstModel.FlowChart_Master_UID = syncParam.FlowChartMaster_UID;
                    #region 同步制程有数据
                    foreach (var item in sumMesProcessList)
                    {
                        StringBuilder sb = new StringBuilder();
                        MES_PISParamDTO model = new MES_PISParamDTO();
                        model.PIS_ProcessID = item.Key;
                        model.TimeInterVal = item.Value.FirstOrDefault().TimeInterVal;
                        model.Date = Convert.ToDateTime(item.Value.FirstOrDefault().Date).ToString("yyyy-MM-dd");

                        DateTime productD = Convert.ToDateTime(model.Date);
                        Product_Input inputModel = new Product_Input();
                        inputModel.Product_Date = Convert.ToDateTime(model.Date);
                        inputModel.Time_Interval = model.TimeInterVal;
                        inputModel.FlowChart_Detail_UID = model.PIS_ProcessID;
                        inputModel.FlowChart_Master_UID = syncParam.FlowChartMaster_UID;
                        var resultModel = ProductInputRepository.GetProductInputByDate(inputModel);
                        if (resultModel == null)
                        {
                            continue;
                        }

                        if (kkflag)
                        {
                            kkflag = false;
                            paramModel.FlowChart_Master_UID = resultModel.FlowChart_Master_UID;
                            paramModel.FlowChart_Detail_UID = resultModel.FlowChart_Detail_UID;
                            paramModel.Time_Interval = resultModel.Time_Interval;
                            paramModel.Product_Date = resultModel.Product_Date;
                            paramModel.FlowChart_Version = resultModel.FlowChart_Version;
                        }

                        model.FlowChart_Version = resultModel.FlowChart_Version;
                        var alterPickWIP = 0;
                        var alterNGWIP = 0;
                        var alterGPWIP = 0;

                        //领料数
                        if (item.Value.Where(p => p.ProcessType == "PIS-PICK").FirstOrDefault() != null)
                        {
                            model.PIS_Pick_Number = item.Value.Where(p => p.ProcessType == "PIS-PICK").Sum(p => p.ProductQuantity);
                            alterPickWIP = model.PIS_Pick_Number - resultModel.Picking_QTY;
                            PICK_Dic.Add(model.PIS_ProcessID, 0);

                            var sqlPIS_Pick_Number = $",Picking_QTY ={model.PIS_Pick_Number}";
                            sb.AppendLine(sqlPIS_Pick_Number);
                        }

                        //判断该良品数是否需要减去NG
                        var pis_ProcessModel = ProcessConfigList.Where(p => p.PIS_ProcessID == item.Key).FirstOrDefault();
                        if (pis_ProcessModel != null)
                        {
                            model.IsSyncNG = pis_ProcessModel.IsSyncNG;
                        }

                        //NG
                        if (item.Value.Where(p => p.ProcessType == "PIS-NG").FirstOrDefault() != null)
                        {
                            model.PIS_NG_Number = item.Value.Where(p => p.ProcessType == "PIS-NG").Sum(p => p.ProductQuantity);
                            alterNGWIP = (model.PIS_NG_Number - resultModel.NG_QTY);

                            var sqlPIS_NG_Number = $",NG_QTY ={model.PIS_NG_Number}";
                            sb.AppendLine(sqlPIS_NG_Number);

                            NG_Dic.Add(model.PIS_ProcessID, 0);
                        }

                        //良品数
                        if (item.Value.Where(p => p.ProcessType == "PIS-GP").FirstOrDefault() != null)
                        {
                            model.PIS_GP_Number = item.Value.Where(p => p.ProcessType == "PIS-GP").Sum(p => p.ProductQuantity);
                            if (model.IsSyncNG && model.PIS_GP_Number > 0)
                            {
                                var ngNumber = 0;
                                if (model.PIS_NG_Number > 0)
                                {
                                    ngNumber = model.PIS_NG_Number;
                                }
                                else
                                {
                                    ngNumber = resultModel.NG_QTY;
                                }

                                model.PIS_GP_Number = model.PIS_GP_Number - ngNumber - resultModel.WH_QTY;
                            }

                            alterGPWIP = (model.PIS_GP_Number - resultModel.Good_QTY);
                            GP_Dic.Add(model.PIS_ProcessID, 0);

                            var sqlPIS_GP_Number = $",Good_QTY ={model.PIS_GP_Number}";
                            var sqlPIS_Normal_GP_Number = $",Normal_NG_QTY ={model.PIS_GP_Number}";//同步更新Normal_QTY
                            sb.AppendLine(sqlPIS_GP_Number);
                            sb.AppendLine(sqlPIS_Normal_GP_Number);
                        }

                        ////返工返修数
                        //if (item.Value.Where(p => p.ProcessType == "PIS-REWORK").FirstOrDefault() == null)
                        //{
                        //    model.PIS_Rework_Number = item.Value.Where(p => p.ProcessType == "PIS-REWORK").Sum(p => p.ProductQuantity);
                        //}

                        var wipNumber = resultModel.WIP_QTY + alterPickWIP - alterNGWIP - alterGPWIP;
                        model.pis_WIPNum = int.Parse(wipNumber.ToString());

                        var sqlPIS_WIP_Number = $",WIP_QTY ={model.pis_WIPNum}";
                        sb.AppendLine(sqlPIS_WIP_Number);

                        ProductInputRepository.updateMesSynsData(model, sb.ToString());
                    }
                    #endregion

                    if (kkflag)
                    {
                        paramModel.FlowChart_Master_UID = syncParam.FlowChartMaster_UID;
                        paramModel.Time_Interval = syncParam.currentInterval;
                        paramModel.Product_Date = Convert.ToDateTime(syncParam.currentDate);
                        paramModel.FlowChart_Version = ProductInputRepository.GetFlowChart_Master_UID(inputFirstModel);
                    }

                    #region 同步制程没有数据
                    var noSyncPickDic = all_PICK_Dic.Keys.Except(PICK_Dic.Keys);//没有同步的领料数
                    foreach (var item in noSyncPickDic)
                    {
                        MES_PISParamDTO model = new MES_PISParamDTO();
                        model.Date = paramModel.Product_Date.ToString("yyyy-MM-dd");
                        model.TimeInterVal = paramModel.Time_Interval;
                        model.FlowChart_Version = paramModel.FlowChart_Version;
                        model.FlowChart_Master_UID = paramModel.FlowChart_Master_UID;
                        model.PIS_ProcessID = item;

                        Product_Input inputModel = new Product_Input();
                        inputModel.Product_Date = Convert.ToDateTime(model.Date);
                        inputModel.Time_Interval = model.TimeInterVal;
                        inputModel.FlowChart_Detail_UID = item;
                        inputModel.FlowChart_Master_UID = syncParam.FlowChartMaster_UID;
                        var resultModel = ProductInputRepository.GetProductInputByDate(inputModel);
                        if (resultModel == null)
                        {
                            continue;
                        }
                        var sqlPIS_Pick_Number = $",Picking_QTY ={0}";
                        var wipNumber = resultModel.WIP_QTY - resultModel.Picking_QTY;
                        var sqlPIS_WIP_Number = $",WIP_QTY ={wipNumber}";//更新后替换原WIP
                        var sql = sqlPIS_Pick_Number + sqlPIS_WIP_Number;
                        ProductInputRepository.updateMesSynsData(model, sql);
                    }
                    var noSyncGPDic = all_GP_Dic.Keys.Except(GP_Dic.Keys);//没有同步的良品数
                    foreach (var item in noSyncGPDic)
                    {
                        MES_PISParamDTO model = new MES_PISParamDTO();
                        model.Date = paramModel.Product_Date.ToString("yyyy-MM-dd");
                        model.TimeInterVal = paramModel.Time_Interval;
                        model.FlowChart_Version = paramModel.FlowChart_Version;
                        model.FlowChart_Master_UID = paramModel.FlowChart_Master_UID;
                        model.PIS_ProcessID = item;

                        Product_Input inputModel = new Product_Input();
                        inputModel.Product_Date = Convert.ToDateTime(model.Date);
                        inputModel.Time_Interval = model.TimeInterVal;
                        inputModel.FlowChart_Detail_UID = item;
                        inputModel.FlowChart_Master_UID = syncParam.FlowChartMaster_UID;
                        var resultModel = ProductInputRepository.GetProductInputByDate(inputModel);
                        if (resultModel == null)
                        {
                            continue;
                        }

                        var sqlPIS_GP_Number = $",Good_QTY ={0}";
                        var sqlPIS_Normal_GP_Number = $",Normal_NG_QTY ={0}";//同步更新Normal_QTY
                        var wipNumber = resultModel.WIP_QTY + resultModel.Good_QTY;
                        var sqlPIS_WIP_Number = $",WIP_QTY ={wipNumber}";//更新后替换原WIP
                        var sql = sqlPIS_GP_Number + sqlPIS_WIP_Number + sqlPIS_Normal_GP_Number;
                        ProductInputRepository.updateMesSynsData(model, sql);
                    }
                    var noSyncNGDic = all_NG_Dic.Keys.Except(NG_Dic.Keys);//没有同步的NG
                    foreach (var item in noSyncNGDic)
                    {
                        MES_PISParamDTO model = new MES_PISParamDTO();
                        model.Date = paramModel.Product_Date.ToString("yyyy-MM-dd");
                        model.TimeInterVal = paramModel.Time_Interval;
                        model.FlowChart_Version = paramModel.FlowChart_Version;
                        model.FlowChart_Master_UID = paramModel.FlowChart_Master_UID;
                        model.PIS_ProcessID = item;
                        Product_Input inputModel = new Product_Input();
                        inputModel.Product_Date = Convert.ToDateTime(model.Date);
                        inputModel.Time_Interval = model.TimeInterVal;
                        inputModel.FlowChart_Detail_UID = item;
                        inputModel.FlowChart_Master_UID = syncParam.FlowChartMaster_UID;
                        var resultModel = ProductInputRepository.GetProductInputByDate(inputModel);
                        if (resultModel == null)
                        {
                            continue;
                        }

                        var sqlPIS_NG_Number = $",NG_QTY ={0}";
                        var sqlPIS_Normal_GP_Number = $",Normal_NG_QTY ={0}";//同步更新Normal_QTY
                        var wipNumber = resultModel.WIP_QTY + resultModel.NG_QTY;//更新后替换原WIP
                        var sqlPIS_WIP_Number = $",WIP_QTY ={wipNumber}";
                        var sql = sqlPIS_NG_Number + sqlPIS_WIP_Number + sqlPIS_Normal_GP_Number;
                        ProductInputRepository.updateMesSynsData(model, sql);
                    }
                    #endregion
                    scope.Complete();
                }
                #endregion
                //更新上下制程领料数和良品数
                #region 同步覆盖
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
new TimeSpan(0, 6, 0)))
                {
                    var mesList = ProcessIDTRSConfigRepository.GteMesDataInfoByDate(syncParam.currentDate, syncParam.currentInterval, syncParam.FlowChartMaster_UID);
                    var sumMesProcessList = mesList.GroupBy(p => p.PIS_ProcessID).ToDictionary(m => m.Key, n => n.ToList());
                    #region
                    foreach (var item in configDic)
                    {
                        MES_PISParamDTO model = new MES_PISParamDTO();
                        model.PIS_ProcessID = item.Key;
                        model.TimeInterVal = paramModel.Time_Interval;
                        model.Date = paramModel.Product_Date.ToString("yyyy-MM-dd");
                        model.FlowChart_Master_UID = paramModel.FlowChart_Master_UID;
                        Product_Input inputModel = new Product_Input();
                        inputModel.Product_Date = Convert.ToDateTime(paramModel.Product_Date.ToString("yyyy-MM-dd"));
                        inputModel.Time_Interval = paramModel.Time_Interval;
                        inputModel.FlowChart_Detail_UID = model.PIS_ProcessID;
                        inputModel.FlowChart_Master_UID = syncParam.FlowChartMaster_UID;

                        var CurrentModel = ProductInputRepository.GetProductInputByDate(inputModel);
                        if (CurrentModel == null)
                        {
                            continue;
                        }

                        //1 GP直接覆盖下一个的领料
                        if (all_Sync_GP_Dic.Keys.Contains(CurrentModel.FlowChart_Detail_UID))
                        {
                            Product_Input NextPicktModel = new Product_Input();
                            NextPicktModel.Product_Date = Convert.ToDateTime(model.Date);
                            NextPicktModel.Time_Interval = model.TimeInterVal;
                            NextPicktModel.Color = ProcessConfigList.Where(p => p.PIS_ProcessID == item.Key).FirstOrDefault().Color;
                            NextPicktModel.Process_Seq = CurrentModel.Process_Seq; //+1
                            NextPicktModel.FlowChart_Master_UID = CurrentModel.FlowChart_Master_UID;
                            NextPicktModel.FlowChart_Version = CurrentModel.FlowChart_Version;
                            //覆盖下制程的领料
                            //获取需要更新的制程
                            var flowChartDetailModel = FlowChartDetailRepository.GetNextProcess_Seq(NextPicktModel.FlowChart_Master_UID, NextPicktModel.FlowChart_Version, NextPicktModel.Process_Seq, NextPicktModel.Color);
                            if (flowChartDetailModel == null)
                            {
                                continue;
                            }
                            NextPicktModel.Process_Seq = flowChartDetailModel.Process_Seq;
                            var updateNextPicktModel = ProductInputRepository.GetUpdateProductProInput(NextPicktModel);
                            if (updateNextPicktModel == null)
                            {
                                continue;
                            }
                            NextPicktModel.WIP_QTY = updateNextPicktModel.WIP_QTY + (CurrentModel.Good_QTY - updateNextPicktModel.Picking_QTY);
                            NextPicktModel.Picking_QTY = CurrentModel.Good_QTY;
                            NextPicktModel.Process_Seq = updateNextPicktModel.Process_Seq;
                            ProductInputRepository.updateNextPickData(NextPicktModel);
                        }//2 PICK覆盖GP
                        else if (all_Sync_PICK_Dic.Keys.Contains(CurrentModel.FlowChart_Detail_UID) && !all_Sync_GP_Dic.Keys.Contains(CurrentModel.FlowChart_Detail_UID))
                        {
                            Product_Input ProPicktModel = new Product_Input();
                            ProPicktModel.Product_Date = Convert.ToDateTime(model.Date);
                            ProPicktModel.Time_Interval = model.TimeInterVal;
                            ProPicktModel.Color = ProcessConfigList.Where(p => p.PIS_ProcessID == item.Key).FirstOrDefault().Color;
                            ProPicktModel.Process_Seq = CurrentModel.Process_Seq;//-1
                            ProPicktModel.FlowChart_Master_UID = CurrentModel.FlowChart_Master_UID;
                            ProPicktModel.FlowChart_Version = CurrentModel.FlowChart_Version;
                            //覆盖上制程GP

                            //获取需要覆盖制程
                            var flowChartDetailModel = FlowChartDetailRepository.GetProProcess_Seq(ProPicktModel.FlowChart_Master_UID, ProPicktModel.FlowChart_Version, ProPicktModel.Process_Seq, ProPicktModel.Color);
                            if (flowChartDetailModel == null)
                            {
                                continue;
                            }
                            ProPicktModel.Process_Seq = flowChartDetailModel.Process_Seq;
                            var updateNextPicktModel = ProductInputRepository.GetUpdateProductProInput(ProPicktModel);

                            if (updateNextPicktModel == null)
                            {
                                continue;
                            }

                            ProPicktModel.WIP_QTY = updateNextPicktModel.WIP_QTY - (CurrentModel.Picking_QTY - updateNextPicktModel.Good_QTY);
                            ProPicktModel.Good_QTY = CurrentModel.Picking_QTY;
                            ProPicktModel.Process_Seq = updateNextPicktModel.Process_Seq;
                            ProductInputRepository.updateProPickData(ProPicktModel);
                        }
                    }
                    #endregion
                    ProductInputRepository.ExecAlterMES_PISSp(paramModel);
                    ProductInputRepository.DeleteMES_StationDataRecord(paramModel);
                    scope.Complete();
                }

                #endregion
                #region
                //foreach (var item in sumMesProcessList)
                //{
                //    MES_PISParamDTO model = new MES_PISParamDTO();
                //    model.PIS_ProcessID = item.Key;
                //    model.TimeInterVal = item.Value.FirstOrDefault().TimeInterVal;
                //    model.Date = System.Convert.ToDateTime(item.Value.FirstOrDefault().Date).ToString("yyyy-MM-dd");

                //    DateTime productD = Convert.ToDateTime(model.Date);
                //    Product_Input inputModel = new Product_Input();
                //    inputModel.Product_Date = Convert.ToDateTime(model.Date);
                //    inputModel.Time_Interval = model.TimeInterVal;
                //    inputModel.FlowChart_Detail_UID = model.PIS_ProcessID;

                //    //获取当前制程
                //    var CurrentModel = ProductInputRepository.GetProductInputByDate(inputModel);
                //    if (CurrentModel == null)
                //    {
                //        continue;
                //    }

                //    //1 GP直接覆盖下一个的领料
                //    if (GP_Dic.Keys.Contains(CurrentModel.FlowChart_Detail_UID))
                //    {
                //        Product_Input NextPicktModel = new Product_Input();
                //        NextPicktModel.Product_Date = Convert.ToDateTime(model.Date);
                //        NextPicktModel.Time_Interval = model.TimeInterVal;
                //        NextPicktModel.Color = ProcessConfigList.Where(p => p.PIS_ProcessID == item.Key).FirstOrDefault().Color;
                //        NextPicktModel.Process_Seq = CurrentModel.Process_Seq; //+1
                //        NextPicktModel.FlowChart_Master_UID = CurrentModel.FlowChart_Master_UID;
                //        NextPicktModel.FlowChart_Version = CurrentModel.FlowChart_Version;
                //        //覆盖下制程的领料
                //        var updateNextPicktModel = ProductInputRepository.GetProductInputBySeq(NextPicktModel);
                //        if (updateNextPicktModel == null)
                //        {
                //            continue;
                //        }
                //        NextPicktModel.WIP_QTY = updateNextPicktModel.WIP_QTY + (CurrentModel.Good_QTY - updateNextPicktModel.Picking_QTY);
                //        NextPicktModel.Picking_QTY = CurrentModel.Good_QTY;
                //        NextPicktModel.Process_Seq = updateNextPicktModel.Process_Seq;
                //        ProductInputRepository.updateNextPickData(NextPicktModel);
                //    }//2 PICK覆盖GP
                //    else if (PICK_Dic.Keys.Contains(CurrentModel.FlowChart_Detail_UID) && !GP_Dic.Keys.Contains(CurrentModel.FlowChart_Detail_UID))
                //    {
                //        Product_Input ProPicktModel = new Product_Input();
                //        ProPicktModel.Product_Date = Convert.ToDateTime(model.Date);
                //        ProPicktModel.Time_Interval = model.TimeInterVal;
                //        ProPicktModel.Color = ProcessConfigList.Where(p => p.PIS_ProcessID == item.Key).FirstOrDefault().Color;
                //        ProPicktModel.Process_Seq = CurrentModel.Process_Seq;//-1
                //        ProPicktModel.FlowChart_Master_UID = CurrentModel.FlowChart_Master_UID;
                //        ProPicktModel.FlowChart_Version = CurrentModel.FlowChart_Version;
                //        //覆盖上制程GP
                //        var updateNextPicktModel = ProductInputRepository.GetProductProInputBySeq(ProPicktModel);
                //        if (updateNextPicktModel == null)
                //        {
                //            continue;
                //        }

                //        ProPicktModel.WIP_QTY = updateNextPicktModel.WIP_QTY - (CurrentModel.Picking_QTY - updateNextPicktModel.Good_QTY);
                //        ProPicktModel.Good_QTY = CurrentModel.Picking_QTY;
                //        ProPicktModel.Process_Seq = updateNextPicktModel.Process_Seq;
                //        ProductInputRepository.updateProPickData(ProPicktModel);
                //}
                //    }
                #endregion

                return "SUCCESS";

            }
            catch (Exception ex)
            {
                return "同步失败";
            }
        }

        /// <summary>
        /// 删除配置信息
        /// </summary>
        /// <param name="uids"></param>
        /// <returns></returns>
        public string DeleteProcessConfig(List<int> uids)
        {
            return ProcessIDTRSConfigRepository.DeleteProcessConfig(uids);
        }

        public string DeleteProcessByUID(int Process_UID)
        {
            return ProcessIDTRSConfigRepository.DeleteProcessByUID(Process_UID);
        }

        public PagedListModel<MES_PIS_SyncFailedRecordDTO> QuerySyncFailedRecord(MES_PIS_SyncFailedRecordDTO paramModel, Page page)
        {
            var totalCount = 0;
            var resultList = ProcessIDTRSConfigRepository.QuerySyncFailedRecord(paramModel, page, out totalCount);
            //totalCount = resultList.Count();
            return new PagedListModel<MES_PIS_SyncFailedRecordDTO>(totalCount, resultList);
        }

        public MES_PIS_SyncFailedRecordDTO GetMES_PIS_SyncFailedRecordDTOByID(int MES_PIS_SyncFailedRecord_UID)
        {
            return ProcessIDTRSConfigRepository.GetMES_PIS_SyncFailedRecordDTOByID(MES_PIS_SyncFailedRecord_UID);
        }
        public string updateSyncFailedLog(MES_PIS_SyncFailedRecordDTO logModel)
        {
            try
            {
                var mES_PIS_SyncFailedRecord = mES_PIS_SyncFailedRecordReposity.GetById(logModel.MES_PIS_SyncFailedRecord_UID);
                mES_PIS_SyncFailedRecord.Is_ManuallySuccess = logModel.Is_ManuallySuccess;
                if (logModel.Is_ManuallySuccess)
                {
                }
                else
                {
                    mES_PIS_SyncFailedRecord.FailedNumber += 1;
                }

                mES_PIS_SyncFailedRecord.OperateID = logModel.OperateID;
                mES_PIS_SyncFailedRecordReposity.Update(mES_PIS_SyncFailedRecord);
                unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        //通过请求参数获取失败记录信息

        /// <summary>
        /// 获取需要同步的专案名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetNeedSysncProjectName()
        {
            var resultList = EnumerationRepository.GetMany(p => p.Enum_Type == "NeedSyncProjectName");
            var listName = new List<string>();
            foreach (var item in resultList)
            {
                listName.Add(item.Enum_Value);
            }
            return listName;
        }
    }

    public interface IProcessIDTRSConfigService
    {
        string AddProcessIDConfigInfo(List<ProcessIDTransformConfigDTO> AddData);
        string AddOrEditProcessInfo(ProcessIDTransformConfigDTO editModel);

        ProcessIDTransformConfigDTO GetProcessDataByUID(int Process_UID);

        string DeleteProcessByUID(int Process_UID);
        bool IsExist(List<ProcessIDTransformConfigDTO> ParamList);
        PagedListModel<ProcessIDTransformConfigDTO> GetProcessIDConfigData(ProcessIDTransformConfigDTO serchModel, Page page);

        QAMasterVM GetProcessDataById(string date, string timeInterval, int flowChartDetailUID);

        List<ProcessIDTransformConfigDTO> ExportAllProcessInfo(ProcessIDTransformConfigDTO ProcessModel);

        List<ProcessIDTransformConfigDTO> ExportPartProcessInfo(string uids);
        string SynchronizeMesInfo(MesSyncParam syncParam);

        //AddMES数据到临时表
        bool AddMESProcessIDInfo(List<ProcessIDTransformConfigDTO> AddData);

        /// <summary>
        /// 获取所有的配置表的数据
        /// </summary>
        /// <returns></returns>
        List<ProcessIDTransformConfigDTO> GetAll();

        /// <summary>
        /// 获取配置表中需要同步的数据
        /// </summary>
        /// <returns></returns>
        List<ProcessIDTransformConfigDTO> GetSyscProcessConfig();
        string DeleteProcessConfig(List<int> uids);
        PagedListModel<MES_PIS_SyncFailedRecordDTO> QuerySyncFailedRecord(MES_PIS_SyncFailedRecordDTO paramModel, Page page);
        MES_PIS_SyncFailedRecordDTO GetMES_PIS_SyncFailedRecordDTOByID(int MES_PIS_SyncFailedRecord_UID);
        string updateSyncFailedLog(MES_PIS_SyncFailedRecordDTO logModel);
        List<string> GetNeedSysncProjectName();
    }
}
