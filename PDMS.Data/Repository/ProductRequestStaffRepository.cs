using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels.ProductionPlanning;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
//    public interface IProductRequestStaffRepository : IRepository<Product_RequestStaff>
//    {
//        List<ActiveManPowerVM> ActualPowerInfo(ActiveManPowerSearchVM vm, Page page, out int totalCount);
//        DataTable GetActualAndEstimateHumanInfo(int flowchatMaster, string beginDate, string endDate, string project, string seq, string process, string estimate, string actual);
//        DataTable GetActualAndEstimateHumanInfoForProcess(int flowchatMaster, string beginDate, string endDate, string project, string seq, string process, string estimate, string actual);
//        List<ActiveManPowerVM> GetManPowerDownLoadInfo(int id, int Version);

//        List<ProductEquipmentQTYDTO> EquipInfo(ActiveManPowerSearchVM vm, Page page, out int totalCount);
//        List<ActiveEquipVM> GetEquipDownLoadInfo(int id, int Version);

//        List<string> GetHumanColumn(string beginDate, string endDate,int flag, string project, string seq, string process, string estimate, string actual);
//    }

//    public class ProductRequestStaffRepository : RepositoryBase<Product_RequestStaff>, IProductRequestStaffRepository
//    {
//        public ProductRequestStaffRepository(IDatabaseFactory databaseFactory)
//            : base(databaseFactory)
//        {

//        }

//        /*
//        SELECT B.Process_Seq,C.FunPlant,B.Process,
//        (SELECT Process FROM dbo.Flowchart_Detail_ME AA 
//        WHERE AA.FlowChart_Master_UID=89 AND AA.FlowChart_Version=1 
//        AND AA.Flowchart_Detail_ME_UID = D.Flowchart_Detail_ME_UID) AS subProcess,
//        (SELECT AA.Process_Seq FROM dbo.Flowchart_Detail_ME AA 
//        WHERE AA.FlowChart_Master_UID=89 AND AA.FlowChart_Version=1 
//        AND AA.Flowchart_Detail_ME_UID = D.Flowchart_Detail_ME_UID) AS subProcess_Seq,
//        D.Father_UID,D.Child_UID,D.ProductDate,A.Product_Phase,D.OP_Qty,D.Monitor_Staff_Qty,D.Technical_Staff_Qty,D.Material_Keeper_Qty,D.Others_Qty
//        FROM dbo.FlowChart_Master A
//        JOIN dbo.FlowChart_Detail B
//        ON A.FlowChart_Master_UID = B.FlowChart_Master_UID AND A.FlowChart_Version = B.FlowChart_Version
//        JOIN dbo.System_Function_Plant C
//        ON B.System_FunPlant_UID = C.System_FunPlant_UID
//        JOIN dbo.Product_RequestStaff D
//        ON D.FlowChart_Detail_UID = B.FlowChart_Detail_UID
//        WHERE A.FlowChart_Master_UID=89 AND A.FlowChart_Version = 1
//        --子查询where语句
//        AND (SELECT Process FROM dbo.Flowchart_Detail_ME AA 
//        WHERE AA.FlowChart_Master_UID=89 AND AA.FlowChart_Version=1 
//        AND AA.Flowchart_Detail_ME_UID = D.Flowchart_Detail_ME_UID) = N'CNC1-2' 
//        */
//        //Sql查询语句在上面
//        public List<ActiveManPowerVM> ActualPowerInfo(ActiveManPowerSearchVM vm, Page page, out int totalCount)
//        {
//            var linq = from A in DataContext.FlowChart_Master
//                       join B in DataContext.FlowChart_Detail
//                       on new { A.FlowChart_Master_UID, A.FlowChart_Version } equals new { B.FlowChart_Master_UID, B.FlowChart_Version }
//                       join C in DataContext.System_Function_Plant
//                       on B.System_FunPlant_UID equals C.System_FunPlant_UID
//                       join D in DataContext.Product_RequestStaff
//                       on B.FlowChart_Detail_UID equals D.FlowChart_Detail_UID
//                       where A.FlowChart_Master_UID == vm.id && A.FlowChart_Version == vm.Version
//                       select new ActiveManPowerVM
//                       {
//                          Product_RequestStaff_UID = D.Product_RequestStaff_UID,
//                          FlowChart_Detail_UID = B.FlowChart_Detail_UID,
//                          Flowchart_Detail_ME_UID = D.Flowchart_Detail_ME_UID,
//                          Father_UID = D.Father_UID,
//                          Child_UID = D.Child_UID,
//                          Process_Seq = B.Process_Seq,
//                          FunPlant = C.FunPlant,
//                          Process = B.Process,
//                          Sub_ProcessSeq = (from AA in DataContext.Flowchart_Detail_ME where AA.FlowChart_Master_UID == vm.id  && AA.FlowChart_Version == vm.Version 
//                                            && AA.Flowchart_Detail_ME_UID == D.Flowchart_Detail_ME_UID select AA.Process_Seq).FirstOrDefault(),
//                          SubProcess = (from AA in DataContext.Flowchart_Detail_ME where AA.FlowChart_Master_UID == vm.id && AA.FlowChart_Version == vm.Version
//                          && AA.Flowchart_Detail_ME_UID == D.Flowchart_Detail_ME_UID select AA.Process).FirstOrDefault(),
//                          Product_Phase = D.Product_Phase,
//                          ProductDate = D.ProductDate,
//                          OP_Qty = D.OP_Qty,
//                          Monitor_Staff_Qty = D.Monitor_Staff_Qty,
//                          Technical_Staff_Qty = D.Technical_Staff_Qty,
//                          Material_Keeper_Qty = D.Material_Keeper_Qty,
//                          Others_Qty = D.Others_Qty,
//                          System_FunPlant_UID = B.System_FunPlant_UID, 
//                       };

//            if (vm.Sub_ProcessSeq != 0)
//            {
//                linq = linq.Where(m => m.Sub_ProcessSeq == vm.Sub_ProcessSeq);
//            }

//            if (vm.System_FunPlant_UID != 0)
//            {
//                linq = linq.Where(m => m.System_FunPlant_UID == vm.System_FunPlant_UID);
//            }

//            if (!string.IsNullOrEmpty(vm.Process))
//            {
//                linq = linq.Where(m => m.Process.Contains(vm.Process));
//            }

//            if (!string.IsNullOrEmpty(vm.SubProcess))
//            {
//                linq = linq.Where(m => m.SubProcess.Contains(vm.SubProcess));
//            }


//            if (!string.IsNullOrEmpty(vm.Modified_Date_From))
//            {
//                linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.ProductDate, vm.Modified_Date_From) <= 0);
//            }

//            if (!string.IsNullOrEmpty(vm.Modified_Date_End))
//            {
//                linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.ProductDate, vm.Modified_Date_End) >= 0);
//            }


//            totalCount = linq.Count();
//            linq = linq.OrderBy(m => m.Sub_ProcessSeq).GetPage(page);
//            return linq.ToList();
//        }


//        public List<ActiveManPowerVM> GetManPowerDownLoadInfo(int id, int Version)
//        {
//            string sql = @";WITH one AS 
//                            (
//                            SELECT 
//                            B.FlowChart_Detail_UID,AA.Flowchart_Detail_ME_UID,
//							B.Process_Seq, D.FunPlant,B.Process,AA.Flowchart_Mapping_UID AS Father_UID, NULL AS Child_UID
//                            FROM dbo.FlowChart_Master A
//                            JOIN dbo.FlowChart_Detail B
//                            ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
//                            AND A.FlowChart_Version = B.FlowChart_Version
//                            JOIN dbo.Flowchart_Mapping AA
//                            ON B.FlowChart_Detail_UID = AA.FlowChart_Detail_UID
//                            JOIN dbo.System_Function_Plant D
//                            ON B.System_FunPlant_UID = D.System_FunPlant_UID
//                            WHERE A.FlowChart_Master_UID={0} AND A.FlowChart_Version={1}
//                            ),
//							two AS 
//							(
//							SELECT 
//                            B.FlowChart_Detail_UID,BB.Flowchart_Detail_ME_UID, 
//							B.Process_Seq, D.FunPlant,CC.Process,NULL AS Father_UID, AA.Flowchart_Mapping_UID AS Child_UID
//                            FROM dbo.FlowChart_Master A
//                            JOIN dbo.FlowChart_Detail B
//                            ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
//                            AND A.FlowChart_Version = B.FlowChart_Version
//                            JOIN dbo.Flowchart_Mapping AA
//                            ON B.FlowChart_Detail_UID = AA.FlowChart_Detail_UID
//                            JOIN dbo.System_Function_Plant D
//                            ON B.System_FunPlant_UID = D.System_FunPlant_UID
//                            LEFT JOIN dbo.Flowchart_Detail_ME_Equipment C
//                            ON AA.Flowchart_Detail_ME_UID = C.Flowchart_Detail_ME_UID
//                            LEFT JOIN dbo.PP_Flowchart_Process_Mapping BB
//                            ON AA.Flowchart_Mapping_UID = BB.Flowchart_Mapping_UID
//							LEFT JOIN dbo.Flowchart_Detail_ME CC
//                            ON bb.Flowchart_Detail_ME_UID = CC.Flowchart_Detail_ME_UID

//                            WHERE A.FlowChart_Master_UID={0} AND A.FlowChart_Version={1}
//							AND BB.Flowchart_Detail_ME_UID IS NOT NULL
//							),
//							three AS 
//							(
//							SELECT * FROM one
//							UNION 
//							SELECT * FROM two
//							)

//                            SELECT * FROM three ORDER BY three.Process_Seq";

//            sql = string.Format(sql, id, Version);
//            var list = DataContext.Database.SqlQuery<ActiveManPowerVM>(sql).ToList();
//            return list;
//        }


//        public List<ProductEquipmentQTYDTO> EquipInfo(ActiveManPowerSearchVM vm, Page page, out int totalCount)
//        {
//            var linq = from A in DataContext.FlowChart_Master
//                       join B in DataContext.FlowChart_Detail
//                       on new { P1 = A.FlowChart_Master_UID, P2 = A.FlowChart_Version } equals new {P1 = B.FlowChart_Master_UID, P2 = B.FlowChart_Version }
//                       join C in DataContext.System_Function_Plant
//                       on B.System_FunPlant_UID equals C.System_FunPlant_UID
//                       join E in DataContext.Product_Equipment_QTY
//                       on B.FlowChart_Detail_UID equals E.FlowChart_Detail_UID
//                       join D in DataContext.Flowchart_Detail_ME_Equipment
//                       on E.Flowchart_Detail_ME_UID equals D.Flowchart_Detail_ME_UID into DEJoin
//                       from DE in DEJoin.DefaultIfEmpty()
//                       where A.FlowChart_Master_UID == vm.id && A.FlowChart_Version == vm.Version
//                       select new ProductEquipmentQTYDTO
//                       {
//                           Product_Equipment_QTY_UID = E.Product_Equipment_QTY_UID,
//                           FlowChart_Detail_UID = E.FlowChart_Detail_UID,
//                           Flowchart_Detail_ME_UID = E.Flowchart_Detail_ME_UID,
//                           Flowchart_Detail_ME_Equipment_UID = E.Flowchart_Detail_ME_Equipment_UID,
//                           Father_UID = E.Father_UID,
//                           Child_UID = E.Child_UID,
//                           Process_Seq = B.Process_Seq,
//                           FunPlant = C.FunPlant,
//                           Process = B.Process,
//                           Sub_ProcessSeq = (from AA in DataContext.Flowchart_Detail_ME
//                                             where AA.FlowChart_Master_UID == vm.id && AA.FlowChart_Version == vm.Version
//                                             && AA.Flowchart_Detail_ME_UID == E.Flowchart_Detail_ME_UID
//                                             select AA.Process_Seq).FirstOrDefault(),
//                           SubProcess = (from AA in DataContext.Flowchart_Detail_ME
//                                         where AA.FlowChart_Master_UID == vm.id && AA.FlowChart_Version == vm.Version
//                                         && AA.Flowchart_Detail_ME_UID == E.Flowchart_Detail_ME_UID
//                                         select AA.Process).FirstOrDefault(),
//                           Equipment_Name = DE.Equipment_Name,
//                           ProductDate = E.ProductDate,
//                           System_FunPlant_UID = B.System_FunPlant_UID,
//                           Qty = E.Qty
//                       };

//            if (vm.Sub_ProcessSeq != 0)
//            {
//                linq = linq.Where(m => m.Sub_ProcessSeq == vm.Sub_ProcessSeq);
//            }

//            if (vm.System_FunPlant_UID != 0)
//            {
//                linq = linq.Where(m => m.System_FunPlant_UID == vm.System_FunPlant_UID);
//            }

//            if (!string.IsNullOrEmpty(vm.Process))
//            {
//                linq = linq.Where(m => m.Process.Contains(vm.Process));
//            }

//            if (!string.IsNullOrEmpty(vm.SubProcess))
//            {
//                linq = linq.Where(m => m.SubProcess.Contains(vm.SubProcess));
//            }

//            if (!string.IsNullOrEmpty(vm.Equipment_Name))
//            {
//                linq = linq.Where(m => m.Equipment_Name.Contains(vm.Equipment_Name));
//            }

//            if (!string.IsNullOrEmpty(vm.Modified_Date_From))
//            {
//                linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.ProductDate, vm.Modified_Date_From) <= 0);
//            }

//            if (!string.IsNullOrEmpty(vm.Modified_Date_End))
//            {
//                linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.ProductDate, vm.Modified_Date_End) >= 0);
//            }


//            totalCount = linq.Count();
//            linq = linq.OrderBy(m => m.Sub_ProcessSeq).GetPage(page);
//            return linq.ToList();
//        }


//        public List<ActiveEquipVM> GetEquipDownLoadInfo(int id, int Version)
//        {
//            string sql = @";WITH one AS 
//                            (
//                            SELECT 
//                            B.FlowChart_Detail_UID,AA.Flowchart_Detail_ME_UID,C.Flowchart_Detail_ME_Equipment_UID,
//							B.Process_Seq, D.FunPlant,B.Process,C.Equipment_Name,AA.Flowchart_Mapping_UID AS Father_UID, NULL AS Child_UID
//                            FROM dbo.FlowChart_Master A
//                            JOIN dbo.FlowChart_Detail B
//                            ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
//                            AND A.FlowChart_Version = B.FlowChart_Version
//                            JOIN dbo.Flowchart_Mapping AA
//                            ON B.FlowChart_Detail_UID = AA.FlowChart_Detail_UID
//                            JOIN dbo.System_Function_Plant D
//                            ON B.System_FunPlant_UID = D.System_FunPlant_UID
//                            LEFT JOIN dbo.Flowchart_Detail_ME_Equipment C
//                            ON AA.Flowchart_Detail_ME_UID = C.Flowchart_Detail_ME_UID
//                            WHERE A.FlowChart_Master_UID={0} AND A.FlowChart_Version={1}
//                            ),
//							two AS 
//							(
//							SELECT 
//                            B.FlowChart_Detail_UID,BB.Flowchart_Detail_ME_UID, E.Flowchart_Detail_ME_Equipment_UID,
//							B.Process_Seq, D.FunPlant,CC.Process,E.Equipment_Name,NULL AS Father_UID, AA.Flowchart_Mapping_UID AS Child_UID
//                            FROM dbo.FlowChart_Master A
//                            JOIN dbo.FlowChart_Detail B
//                            ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
//                            AND A.FlowChart_Version = B.FlowChart_Version
//                            JOIN dbo.Flowchart_Mapping AA
//                            ON B.FlowChart_Detail_UID = AA.FlowChart_Detail_UID
//                            JOIN dbo.System_Function_Plant D
//                            ON B.System_FunPlant_UID = D.System_FunPlant_UID
//                            LEFT JOIN dbo.Flowchart_Detail_ME_Equipment C
//                            ON AA.Flowchart_Detail_ME_UID = C.Flowchart_Detail_ME_UID
//                            LEFT JOIN dbo.PP_Flowchart_Process_Mapping BB
//                            ON AA.Flowchart_Mapping_UID = BB.Flowchart_Mapping_UID
//                            LEFT JOIN dbo.Flowchart_Detail_ME CC
//                            ON bb.Flowchart_Detail_ME_UID = CC.Flowchart_Detail_ME_UID
//                            LEFT JOIN dbo.Flowchart_Detail_ME_Equipment E
//                            ON BB.Flowchart_Detail_ME_UID = E.Flowchart_Detail_ME_UID
//                            WHERE A.FlowChart_Master_UID={0} AND A.FlowChart_Version={1}
//							AND BB.Flowchart_Detail_ME_UID IS NOT NULL
//							),
//							three AS 
//							(
//							SELECT * FROM one
//							UNION 
//							SELECT * FROM two
//							)

//                            SELECT * FROM three ORDER BY three.Process_Seq";

//            sql = string.Format(sql, id, Version);
//            var list = DataContext.Database.SqlQuery<ActiveEquipVM>(sql).ToList();
//            return list;
//        }


//        public DataTable GetActualAndEstimateHumanInfo(int flowchatMaster,string beginDate, string endDate, string project, string seq, string process, string estimate, string actual)
//        {
//            List<ActualAndEstimateInfo> list = new List<ActualAndEstimateInfo>();
//            string strActual = " where c.FlowChart_Master_UID=" + flowchatMaster + " and a.[ProductDate]<='" + DateTime.Parse(endDate).ToShortDateString() + "' and a.[ProductDate]>='" + DateTime.Parse(beginDate).ToShortDateString() + "'";
//            if (flowchatMaster == 0)
//            {
//                strActual = " where a.[ProductDate]<='" + DateTime.Parse(endDate).ToShortDateString() + "' and a.[ProductDate]>='" + DateTime.Parse(beginDate).ToShortDateString()+"'";
//            }
            
//            string sql = @"
//select d.Project_Name,
//sum(a.[OP_Qty]+a.[Monitor_Staff_Qty]+a.[Technical_Staff_Qty]+a.[Material_Keeper_Qty]+a.[Others_Qty])as ActualNum,
//c.FlowChart_Master_UID,c.FlowChart_Version,a.[ProductDate] from 
//[dbo].[Product_RequestStaff] as a left  join dbo.FlowChart_Detail as b on a.[FlowChart_Detail_UID]=b.[FlowChart_Detail_UID]
//left  join dbo.FlowChart_Master as c on b.FlowChart_Master_UID=c.FlowChart_Master_UID
//left  join dbo.System_Project as d on c.Project_UID=d.Project_UID {0}
//group by c.FlowChart_Master_UID,a.[ProductDate],d.Project_Name,c.FlowChart_Master_UID,c.FlowChart_Version";
//            sql = string.Format(sql, strActual);
//            var actuallist = DataContext.Database.SqlQuery<ActualInfo>(sql).ToList();

//            string strEstimate = " where c.FlowChart_Master_UID=" + flowchatMaster + " and f.Created_Date<='" + DateTime.Parse(endDate).ToShortDateString() + "' and f.Created_Date>='" + DateTime.Parse(beginDate).ToShortDateString() + "'";
//            if (flowchatMaster == 0)
//            {
//                strEstimate = " where f.Created_Date<='" + DateTime.Parse(endDate).ToShortDateString() + "' and f.Created_Date>='" + DateTime.Parse(beginDate).ToShortDateString() + "'";
//            }
//            sql = @"
//select d.Project_Name,sum(f.VariationOP_Qty+f.RegularOP_Qty+f.MaterialKeeper_Qty+f.Others_Qty+f.SquadLeader_Qty+f.Technician_Qty)*2 
//as EstimateNum,c.FlowChart_Master_UID,c.FlowChart_Version,CONVERT(varchar(100), f.Created_Date, 23) as [ProductDate] from 
//dbo.Flowchart_Detail_IE as f 
//inner  join dbo.Flowchart_Detail_ME as e on e.Flowchart_Detail_ME_UID=f.Flowchart_Detail_ME_UID

//inner  join dbo.FlowChart_Master as c on e.FlowChart_Master_UID=c.FlowChart_Master_UID and c.FlowChart_Version=e.FlowChart_Version
//left  join dbo.System_Project as d on c.Project_UID=d.Project_UID {0}
//group by c.FlowChart_Master_UID,CONVERT(varchar(100), f.Created_Date, 23),d.Project_Name,c.FlowChart_Master_UID,c.FlowChart_Version";
//            sql = string.Format(sql, strEstimate);
//            var estimatelist = DataContext.Database.SqlQuery<EstimateInfo>(sql).ToList();
//            int lengthNum = 0;
//            bool flag = false;
//            ActualAndEstimateColumn col = new ActualAndEstimateColumn();
//            if (actuallist != null && actuallist.Count > 0)
//            {
//                if (estimatelist != null && estimatelist.Count > 0&& actuallist.Count< estimatelist.Count)
//                {
//                    lengthNum = estimatelist.Count;
                    
//                }
//                else
//                {
//                    lengthNum = actuallist.Count;
//                    flag = true;
//                }
//            }
//            else if (estimatelist != null && estimatelist.Count > 0)
//            {
//                lengthNum = estimatelist.Count;
//            }
//            ActualAndEstimateInfo info = new ActualAndEstimateInfo();
//            Dictionary<string, object> dictionary= new Dictionary<string, object>();
//            if (flag)
//            {
//                info.Project_Name = actuallist[0].Project_Name;
//                info.FlowChart_Master_UID = actuallist[0].FlowChart_Master_UID;
//                info.FlowChart_Version = actuallist[0].FlowChart_Version;
//            }
//            else
//            {
//                if (estimatelist != null && estimatelist.Count > 0)
//                {
//                    info.Project_Name = estimatelist[0].Project_Name;
//                    info.FlowChart_Master_UID = estimatelist[0].FlowChart_Master_UID;
//                    info.FlowChart_Version = estimatelist[0].FlowChart_Version;
//                }
//            }
//            for (int i = 0; i < lengthNum; i++)
//            {
                
//                ///string col1 = DateTime.Parse(beginDate).AddDays(i).ToString("yyyy-MM-dd") + "预估";
//                //string col2 = DateTime.Parse(beginDate).AddDays(i).ToString("yyyy-MM-dd") + "实际";

//                string col1 = DateTime.Parse(beginDate).AddDays(i).ToString(FormatConstants.DateTimeFormatStringByDate) + estimate;
//                string col2 = DateTime.Parse(beginDate).AddDays(i).ToString(FormatConstants.DateTimeFormatStringByDate) + actual;
//                if (flag)
//                {
//                    string name = "column" + i;
//                    if (estimatelist.Count > 0 &&i< estimatelist.Count && estimatelist[i] != null)
//                    {
//                        dictionary.Add(col1, estimatelist[i].EstimateNum);
//                    }
//                    else
//                    {
//                        dictionary.Add(col1,0);
//                    }
//                    name= "column" + i+1;
//                    if (actuallist.Count > 0 && i < actuallist.Count && actuallist[i] != null)
//                    {
//                        dictionary.Add(col2, actuallist[i].ActualNum);
//                    }
//                    else
//                    {
//                        dictionary.Add(col2, 0);
//                    }
//                }
//                else
//                {
//                    string name = "column" + i;
//                    if (estimatelist.Count > 0 && i < estimatelist.Count && estimatelist[i] != null)
//                    {
//                        dictionary.Add(col1, estimatelist[i].EstimateNum);
//                    }
//                    else
//                    {
//                        dictionary.Add(col1, 0);
//                    }
//                    name = "column" + i + 1;
//                    if (actuallist.Count > 0 && i < actuallist.Count && actuallist[i] != null)
//                    {
//                        dictionary.Add(col2, actuallist[i].ActualNum);
//                    }
//                    else
//                    {
//                        dictionary.Add(col2, 0);
//                    }
//                }
//            }
//            col.ColumnList = GetHumanColumn(beginDate,endDate,1, project, seq,  process,  estimate, actual);
//            info.dictionary = dictionary;
//            info.ShowColumn = col;
//            list.Add(info);

//            return FillDataTable(list,1,project, seq, process);
//        }

//        public List<string> GetHumanColumn(string beginDate, string endDate,int flag,string project,string seq ,string process,string estimate ,string actual)
//        {
//            string dateString = string.Empty;
//            List<string> colList = new List<string>();
//            if (flag == 1)
//            {
//                //colList.Add("专案");
//                colList.Add(project);
//                colList.Add("FlowChart_Master_UID");
//            }
//            else if (flag == 2)
//            {
//                //colList.Add("序号");
//                //colList.Add("制程");
//                colList.Add(seq);
//                colList.Add(process);
//            }
//            else if (flag == 3)
//            {
//               // colList.Add("专案");
//                colList.Add(project);
//            }
//            TimeSpan d3 = DateTime.Parse(beginDate).Subtract(DateTime.Parse(endDate));
//            int diff = Math.Abs(d3.Days);
//            for (int j = 0; j < diff; j++)
//            {
//                dateString += "[" + DateTime.Parse(beginDate).AddDays(j).ToShortDateString() + "],";
//                //string col1 = DateTime.Parse(beginDate).AddDays(j).ToString("yyyy-MM-dd") + "预估";
//                //string col2 = DateTime.Parse(beginDate).AddDays(j).ToString("yyyy-MM-dd") + "实际";
//                string col1 = DateTime.Parse(beginDate).AddDays(j).ToString(FormatConstants.DateTimeFormatStringByDate) + estimate;
//                string col2 = DateTime.Parse(beginDate).AddDays(j).ToString(FormatConstants.DateTimeFormatStringByDate) + actual;
//                colList.Add(col1);
//                colList.Add(col2);
//            }
//            return colList;
//        }


//        /// <summary>
//        /// 将行转列的数据转换成DataTable
//        /// </summary>
//        /// <param name="modelList">实体类列表</param>
//        /// <returns></returns>
//        public DataTable FillDataTable(List<ActualAndEstimateInfo> info,int flag, string project, string seq, string process)
//        {
//            DataTable table = new DataTable("table1");
//            foreach(var tmp in info[0].ShowColumn.ColumnList)
//            {
//                table.Columns.Add(tmp, Type.GetType("System.String"));
//            }
//            foreach (var t in info)
//            {
//                int i = 1;
//                DataRow row = table.NewRow();//创建新行  
//                table.Rows.Add(row);//将新行加入到表中  
//                if (flag == 1)
//                {
//                    //row["专案"] = t.Project_Name;
//                    row[project] = t.Project_Name;
//                    row["FlowChart_Master_UID"] = t.FlowChart_Master_UID;
//                }
//                else
//                {
//                    //row["序号"] = i;
//                    //i++;
//                    //row["制程"] = t.Process;
//                    row[seq] = i;
//                    i++;
//                    row[process] = t.Process;
//                }
//                foreach (var tmp in t.ShowColumn.ColumnList)
//                {
//                    if (flag == 1)
//                    {
//                        //if (tmp != "专案" && tmp != "FlowChart_Master_UID")
//                        //{
//                        if (tmp != project && tmp != "FlowChart_Master_UID")
//                        {
//                            if (t.dictionary.ContainsKey(tmp))
//                            {
//                                row[tmp] = t.dictionary[tmp];
//                            }
//                            else
//                            {
//                                row[tmp] = 0;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        //if (tmp != "序号" && tmp != "制程")
//                        //{
//                            if (tmp != seq && tmp != process)
//                            {
//                                if (t.dictionary.ContainsKey(tmp))
//                            {
//                                row[tmp] = t.dictionary[tmp];
//                            }
//                            else
//                            {
//                                row[tmp] = 0;
//                            }
//                        }
//                    }
//                }
//            }
//            return table;
//        }

//        public DataTable GetActualAndEstimateHumanInfoForProcess(int flowchatMaster, string beginDate, string endDate, string project, string seq, string process, string estimate, string actual)
//        {
//            List<ActualAndEstimateInfo> list = new List<ActualAndEstimateInfo>();
//            string strActual = " where c.FlowChart_Master_UID=" + flowchatMaster + " and a.[ProductDate]>'" + DateTime.Parse(beginDate).ToShortDateString()+ "' and a.[ProductDate]<'" + DateTime.Parse(endDate).ToShortDateString() + "'";
//            if (flowchatMaster == 0)
//            {
//                strActual = " where a.[ProductDate]<'" + DateTime.Parse(endDate).ToShortDateString() + "' and a.[ProductDate]>'" + DateTime.Parse(beginDate).ToShortDateString() + "'";
//            }
//            string sql = @"select d.Project_Name,(a.[OP_Qty]+a.[Monitor_Staff_Qty]+a.[Technical_Staff_Qty]+a.[Material_Keeper_Qty]+a.[Others_Qty])as ActualNum,c.FlowChart_Master_UID,c.FlowChart_Version,a.[ProductDate],b.Process,b.[Process_Seq],b.FlowChart_Detail_UID from 
//[dbo].[Product_RequestStaff] as a left  join dbo.FlowChart_Detail as b on a.[FlowChart_Detail_UID]=b.[FlowChart_Detail_UID]
//left  join dbo.FlowChart_Master as c on b.FlowChart_Master_UID=c.FlowChart_Master_UID
//left  join dbo.System_Project as d on c.Project_UID=d.Project_UID {0}";
//            sql = string.Format(sql, strActual);
//            var actuallist = DataContext.Database.SqlQuery<ActualInfo>(sql).ToList();

//            string strEstimate = " where c.FlowChart_Master_UID=" + flowchatMaster + " and f.Created_Date<'" + DateTime.Parse(endDate).ToShortDateString() + "' and f.Created_Date>'" + DateTime.Parse(beginDate).ToShortDateString() + "'";
//            if (flowchatMaster == 0)
//            {
//                strEstimate = " where f.Created_Date<'" + DateTime.Parse(endDate).ToShortDateString() + "' and f.Created_Date>'" + DateTime.Parse(beginDate).ToShortDateString() + "'";
//            }
//            sql = @"select d.Project_Name,(f.VariationOP_Qty+f.RegularOP_Qty+f.MaterialKeeper_Qty+f.Others_Qty+f.SquadLeader_Qty+f.Technician_Qty)*2 
//as EstimateNum,c.FlowChart_Master_UID,c.FlowChart_Version,CONVERT(varchar(100), f.Created_Date, 23) as [ProductDate],e.Process,e.[Process_Seq],t.FlowChart_Detail_UID from 
//dbo.Flowchart_Detail_IE as f 
//inner  join dbo.Flowchart_Detail_ME as e on e.Flowchart_Detail_ME_UID=f.Flowchart_Detail_ME_UID
//inner join dbo.Flowchart_Mapping as m on m.Flowchart_Detail_ME_UID=e.Flowchart_Detail_ME_UID
//inner join dbo.FlowChart_Detail as t on m.FlowChart_Detail_UID=t.FlowChart_Detail_UID
//inner  join dbo.FlowChart_Master as c on e.FlowChart_Master_UID=c.FlowChart_Master_UID and c.FlowChart_Version=e.FlowChart_Version
//left  join dbo.System_Project as d on c.Project_UID=d.Project_UID {0}";
//            sql = string.Format(sql, strEstimate);
//            var estimatelist = DataContext.Database.SqlQuery<EstimateInfo>(sql).ToList();
//            int lengthNum = 0;
//            bool flag = false;
//            ActualAndEstimateColumn col = new ActualAndEstimateColumn();
//            if (actuallist != null && actuallist.Count > 0)
//            {
//                if (estimatelist != null && estimatelist.Count > 0 && actuallist.Count < estimatelist.Count)
//                {
//                    lengthNum = estimatelist.Count;

//                }
//                else
//                {
//                    lengthNum = actuallist.Count;
//                    flag = true;
//                }
//            }
//            else if (estimatelist != null && estimatelist.Count > 0)
//            {
//                lengthNum = estimatelist.Count;
//            }
//            ActualAndEstimateInfo info = new ActualAndEstimateInfo();
//            Dictionary<string, object> dictionary = new Dictionary<string, object>();

//            for (int i = 0; i < lengthNum; i++)
//            {

//                if (flag)
//                {
//                    info.Project_Name = actuallist[0].Project_Name;
//                    info.FlowChart_Master_UID = actuallist[0].FlowChart_Master_UID;
//                    info.FlowChart_Version = actuallist[0].FlowChart_Version;
//                    info.Process = actuallist[0].Process;
//                    info.Process_Seq = actuallist[0].Process_Seq;
//                    info.FlowChart_Detail_UID = actuallist[0].FlowChart_Detail_UID;
//                }
//                else
//                {
//                    info.Project_Name = estimatelist[0].Project_Name;
//                    info.FlowChart_Master_UID = estimatelist[0].FlowChart_Master_UID;
//                    info.FlowChart_Version = estimatelist[0].FlowChart_Version;
//                    info.Process = estimatelist[0].Process;
//                    info.Process_Seq = estimatelist[0].Process_Seq;
//                    info.FlowChart_Detail_UID = estimatelist[0].FlowChart_Detail_UID;
//                }
//                //string col1 = DateTime.Parse(beginDate).AddDays(i).ToString("yyyy-MM-dd") + "预估";
//                //string col2 = DateTime.Parse(beginDate).AddDays(i).ToString("yyyy-MM-dd") + "实际";

//                string col1 = DateTime.Parse(beginDate).AddDays(i).ToString(FormatConstants.DateTimeFormatStringByDate) +estimate;
//                string col2 = DateTime.Parse(beginDate).AddDays(i).ToString(FormatConstants.DateTimeFormatStringByDate) + actual;
//                if (flag)
//                {
//                    string name = "column" + i;
//                    if (estimatelist.Count > 0 && i < estimatelist.Count && estimatelist[i] != null)
//                    {
//                        dictionary.Add(col1, estimatelist[i].EstimateNum);
//                    }
//                    else
//                    {
//                        dictionary.Add(col1, 0);
//                    }
//                    name = "column" + i + 1;
//                    if (actuallist.Count > 0 && i < actuallist.Count && actuallist[i] != null)
//                    {
//                        dictionary.Add(col2, actuallist[i].ActualNum);
//                    }
//                    else
//                    {
//                        dictionary.Add(col2, 0);
//                    }
//                }
//                else
//                {
//                    string name = "column" + i;
//                    if (estimatelist.Count > 0 && i < estimatelist.Count && estimatelist[i] != null)
//                    {
//                        dictionary.Add(col1, estimatelist[i].EstimateNum);
//                    }
//                    else
//                    {
//                        dictionary.Add(col1, 0);
//                    }
//                    name = "column" + i + 1;
//                    if (actuallist.Count > 0 && i < actuallist.Count && actuallist[i] != null)
//                    {
//                        dictionary.Add(col2, actuallist[i].ActualNum);
//                    }
//                    else
//                    {
//                        dictionary.Add(col2, 0);
//                    }
//                }
//                col.ColumnList = GetHumanColumn(beginDate, endDate, 2,  project,  seq, process, estimate,  actual);
//                info.dictionary = dictionary;
//                info.ShowColumn = col;
//                var ff = list.Find(a => a.FlowChart_Detail_UID == info.FlowChart_Detail_UID);
//                if (ff != null && ff.FlowChart_Detail_UID == info.FlowChart_Detail_UID)
//                {
//                    continue;
//                }
//                else
//                {
//                    list.Add(info);
//                }
//            }
            

//            return FillDataTable(list,2, project, seq, process);
//        }

//    }
}
