using PDMS.Data.Infrastructure;
using PDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    //public interface IPPFlowchartProcessMappingRepository : IRepository<PP_Flowchart_Process_Mapping>
    //{
    //    void SavePPFlowchart(FlowChartDetailDTO dto, List<int> idList);
    //}

    //public class PPFlowchartProcessMappingRepository : RepositoryBase<PP_Flowchart_Process_Mapping>, IPPFlowchartProcessMappingRepository
    //{
    //    public PPFlowchartProcessMappingRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
    //    {

    //    }

    //    public void SavePPFlowchart(FlowChartDetailDTO dto, List<int> idList)
    //    {
    //        using (var trans = DataContext.Database.BeginTransaction())
    //        {
    //            var time = DateTime.Now;

    //            var flMappingItem = DataContext.Flowchart_Mapping.Where(m => m.Flowchart_Detail_ME_UID == dto.Flowchart_Detail_ME_UID).FirstOrDefault();
    //            if (flMappingItem != null)
    //            {
    //                var detailItem = DataContext.FlowChart_Detail.Where(m => m.FlowChart_Detail_UID == flMappingItem.FlowChart_Detail_UID).First();
    //                var processMappingList = DataContext.PP_Flowchart_Process_Mapping.Where(m => m.Flowchart_Mapping_UID == flMappingItem.Flowchart_Mapping_UID).ToList();

    //                //全删操作
    //                DataContext.PP_Flowchart_Process_Mapping.RemoveRange(processMappingList);
    //                DataContext.Flowchart_Mapping.Remove(flMappingItem);
    //                DataContext.FlowChart_Detail.Remove(detailItem);
    //            }

    //            //如果PP战情站点没有选中则说明清空所有数据不插入
    //            if (!string.IsNullOrEmpty(dto.chkPPProcess))
    //            {
    //                var item = DataContext.Flowchart_Detail_ME.Where(m => m.Flowchart_Detail_ME_UID == dto.Flowchart_Detail_ME_UID).First();
    //                //全插操作
    //                //插入FlowChart_Detail表
    //                var insertFLDetailSql = InsertFLDetailSql(item, dto, time);
    //                DataContext.Database.ExecuteSqlCommand(insertFLDetailSql);
    //                var uidSql = "SELECT  SCOPE_IDENTITY();";
    //                var flUID = DataContext.Database.SqlQuery<decimal>(uidSql).First();
    //                //插入Flowchart_Mapping表
    //                var insertFlMappingSql = InsertFlMappingSql(dto.Flowchart_Detail_ME_UID, flUID, dto.Modified_UID, time);
    //                DataContext.Database.ExecuteSqlCommand(insertFlMappingSql);

    //                //如果有勾选任何一个checkbox则插入关联数据
    //                if (idList.Count() > 0)
    //                {
    //                    var flMappingUID = DataContext.Database.SqlQuery<decimal>(uidSql).First();
    //                    //插入PP_Flowchart_Process_Mapping表
    //                    var insertPPProcessSql = InsertPPProcessSql(idList, flMappingUID, time, dto.Modified_UID);
    //                    if (!string.IsNullOrEmpty(insertPPProcessSql))
    //                    {
    //                        DataContext.Database.ExecuteSqlCommand(insertPPProcessSql);
    //                    }
    //                }
    //            }
    //            DataContext.SaveChanges();
    //            trans.Commit();
    //        }
    //    }

    //    #region PP Flowchart保存Sql
    //    private string InsertFLDetailSql(Flowchart_Detail_ME item, FlowChartDetailDTO dto, DateTime time)
    //    {
    //        string DRI = string.Empty;
    //        string Place = string.Empty;
    //        if (string.IsNullOrEmpty(dto.DRI))
    //        {
    //            DRI = "无";
    //        }
    //        else
    //        {
    //            DRI = dto.DRI;
    //        }
    //        if (string.IsNullOrEmpty(dto.Place))
    //        {
    //            Place = "无";
    //        }
    //        else
    //        {
    //            Place = dto.Place;
    //        }

    //        string insertFLDetailSql = @"INSERT INTO dbo.FlowChart_Detail
    //                                        ( FlowChart_Master_UID ,
    //                                          System_FunPlant_UID ,
    //                                          Process_Seq ,
    //                                          DRI ,
    //                                          Place ,
    //                                          Process ,
    //                                          Product_Stage ,
    //                                          Color ,
    //                                          Process_Desc ,
    //                                          FlowChart_Version ,
    //                                          FlowChart_Version_Comment ,
    //                                          Modified_UID ,
    //                                          Modified_Date ,
    //                                          IsQAProcess ,
    //                                          Rework_Flag ,
    //                                          WIP_QTY ,
    //                                          Binding_Seq
    //                                        )
    //                                VALUES  ( {0} , -- FlowChart_Master_UID - int
    //                                          {1} , -- System_FunPlant_UID - int
    //                                          {2} , -- Process_Seq - int
    //                                          N'{3}' , -- DRI - nvarchar(50)
    //                                          N'{4}' , -- Place - nvarchar(50)
    //                                          N'{5}' , -- Process - nvarchar(50)
    //                                          0 , -- Product_Stage - int
    //                                          N'{6}' , -- Color - nvarchar(50)
    //                                          N'{7}' , -- Process_Desc - nvarchar(100)
    //                                          {8} , -- FlowChart_Version - int
    //                                          N'{9}' , -- FlowChart_Version_Comment - nvarchar(200)
    //                                          {10} , -- Modified_UID - int
    //                                          '{11}' , -- Modified_Date - datetime
    //                                          N'{12}' , -- IsQAProcess - varchar(50)
    //                                          N'{13}' , -- Rework_Flag - nvarchar(20)
    //                                          0 , -- WIP_QTY - int
    //                                          {14}  -- Binding_Seq - int
    //                                        )";
    //        insertFLDetailSql = string.Format(insertFLDetailSql,
    //            item.FlowChart_Master_UID,
    //            item.System_FunPlant_UID,
    //            item.Process_Seq,
    //            DRI,
    //            Place,
    //            dto.Process,
    //            item.Color,
    //            item.Process_Desc,
    //            item.FlowChart_Version,
    //            item.FlowChart_Version_Comment,
    //            dto.Modified_UID,
    //            time,
    //            dto.IsQAProcess,
    //            dto.Rework_Flag,
    //            item.Binding_Seq
    //            );
    //        return insertFLDetailSql;

    //    }

    //    private string InsertFlMappingSql(int id, decimal flUID, int AccountUID, DateTime time)
    //    {
    //        string sql = @"INSERT INTO dbo.Flowchart_Mapping
    //                            ( Flowchart_Detail_ME_UID ,
    //                              FlowChart_Detail_UID ,
    //                              Created_Date ,
    //                              Created_UID ,
    //                              Modified_Date ,
    //                              Modified_UID
    //                            )
    //                    VALUES  ( {0} , -- Flowchart_Detail_ME_UID - int
    //                              {1} , -- FlowChart_Detail_UID - int
    //                              '{2}' , -- Created_Date - datetime
    //                              {3} , -- Created_UID - int
    //                              '{2}' , -- Modified_Date - datetime
    //                              {3}  -- Modified_UID - int
    //                            )";
    //        sql = string.Format(sql,
    //            id,
    //            Convert.ToInt32(flUID),
    //            time,
    //            AccountUID);
    //        return sql;
    //    }

    //    private string InsertPPProcessSql(List<int> idList, decimal flMappingUID, DateTime time, int AccountUID)
    //    {
    //        StringBuilder sb = new StringBuilder();


    //        foreach (var idItem in idList)
    //        {
    //            string sql = @"INSERT INTO dbo.PP_Flowchart_Process_Mapping
    //                            ( Flowchart_Mapping_UID ,
    //                              Flowchart_Detail_ME_UID ,
    //                              Create_Date ,
    //                              Created_UID ,
    //                              Modified_Date ,
    //                              Modified_UID
    //                            )
    //                    VALUES  ( {0} , -- Flowchart_Mapping_UID - int
    //                              {1} , -- Flowchart_Detail_ME_UID - int
    //                              '{2}' , -- Create_Date - datetime
    //                              {3} , -- Created_UID - int
    //                              '{2}' , -- Modified_Date - datetime
    //                              {3}  -- Modified_UID - int
    //                            ); ";
    //            sql = string.Format(sql,
    //                Convert.ToInt32(flMappingUID),
    //                idItem,
    //                time,
    //                AccountUID
    //                );
    //            sb.Append(sql);
    //        }
    //        if (idList.Count() > 0)
    //        {
    //            return sb.ToString();
    //        }
    //        else
    //        {
    //            return string.Empty;
    //        }
    //    }

    //    #endregion
    //}
}
