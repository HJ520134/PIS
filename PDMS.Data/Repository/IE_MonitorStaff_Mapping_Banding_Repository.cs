using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PDMS.Common.Helpers;

namespace PDMS.Data.Repository
{
    //public interface IIE_MonitorStaff_Mapping_Banding_Repository : IRepository<IE_MonitorStaff_Mapping_Banding>
    //{
    //    string SaveMappingData(Flowchart_Detail_IE_VMList data);

    //}
    //public class IE_MonitorStaff_Mapping_Banding_Repository : RepositoryBase<IE_MonitorStaff_Mapping_Banding>, IIE_MonitorStaff_Mapping_Banding_Repository
    //{
    //    private Logger log = new Logger("IE_MonitorStaff_Mapping_Banding");
    //    public IE_MonitorStaff_Mapping_Banding_Repository(IDatabaseFactory databaseFactory)
    //        : base(databaseFactory)
    //    {

    //    }

    //    public string SaveMappingData(Flowchart_Detail_IE_VMList data)
    //    {
    //        string result = "Success";
    //        try
    //        {
    //            foreach (Flowchart_Detail_IE_VM temp in data.DataList)
    //            {
    //                string sql = "";
    //                string Technician_Sql = "";
    //                string Monitor_Sql = "";

    //                #region //技术员  sql
    //                if (temp.TechnicianFlag && (temp.Technician_Staff_UID == 0 || temp.Technician_Staff_UID == null))
    //                {
    //                    Technician_Sql = string.Format(@"
    //                                    DELETE  FROM dbo.IE_MonitorStaff_Mapping_Banding
    //                                    WHERE   IE_MonitorStaff_Mapping_Banding_UID = {0}
    //                                            OR Flowchart_Detail_ME_UID = {0}
    //                                            AND MappingType = 2

    //                                    INSERT INTO dbo.IE_MonitorStaff_Mapping_Banding
    //                                            ( Flowchart_Detail_ME_UID ,
    //                                              Flowchart_Detail_ME_UID_Banding ,
    //                                              MappingType ,
    //                                              Created_Date ,
    //                                              Created_UID
    //                                            )
    //                                    VALUES  ( {0} , -- Flowchart_Detail_ME_UID - int
    //                                              {1}, -- Flowchart_Detail_ME_UID_Banding - int
    //                                              2 , -- MappingType - int
    //                                              GETDATE() , -- Created_Date - datetime
    //                                              {2} -- Created_UID - int
    //                                            )", temp.Flowchart_Detail_ME_UID, temp.Flowchart_Detail_ME_UID_Banding, temp.Created_UID);
    //                }
    //                else if (!temp.TechnicianFlag && temp.Technician_Staff_UID != null)
    //                {
    //                    Technician_Sql = string.Format(@"DELETE  FROM dbo.IE_MonitorStaff_Mapping_Banding
    //                        WHERE Flowchart_Detail_ME_UID = {0} AND MappingType = 2", temp.Flowchart_Detail_ME_UID);
    //                }
    //                #endregion

    //                #region //班长 sql
    //                if (temp.MonitorFlag && (temp.Monitor_Staff_UID == 0|| temp.Monitor_Staff_UID == null))
    //                {
    //                    Monitor_Sql = string.Format(@"
    //                                    DELETE  FROM dbo.IE_MonitorStaff_Mapping_Banding
    //                                    WHERE   IE_MonitorStaff_Mapping_Banding_UID = {0}
    //                                            OR Flowchart_Detail_ME_UID = {0}
    //                                            AND MappingType = 1

    //                                    INSERT INTO dbo.IE_MonitorStaff_Mapping_Banding
    //                                            ( Flowchart_Detail_ME_UID ,
    //                                              Flowchart_Detail_ME_UID_Banding ,
    //                                              MappingType ,
    //                                              Created_Date ,
    //                                              Created_UID
    //                                            )
    //                                    VALUES  ( {0} , -- Flowchart_Detail_ME_UID - int
    //                                              {1}, -- Flowchart_Detail_ME_UID_Banding - int
    //                                              1 , -- MappingType - int
    //                                              GETDATE() , -- Created_Date - datetime
    //                                              {2} -- Created_UID - int
    //                                            )", temp.Flowchart_Detail_ME_UID, temp.Flowchart_Detail_ME_UID_Banding, temp.Created_UID);
    //                }
    //                else if (!temp.MonitorFlag && temp.Monitor_Staff_UID != null)
    //                {
    //                    Monitor_Sql = string.Format(@"DELETE  FROM dbo.IE_MonitorStaff_Mapping_Banding
    //                        WHERE Flowchart_Detail_ME_UID = {0} AND MappingType = 1", temp.Flowchart_Detail_ME_UID);
    //                }
    //                #endregion

    //                sql = Technician_Sql + " " + Monitor_Sql;
    //                if (!string.IsNullOrWhiteSpace(sql))
    //                {
    //                    DataContext.Database.ExecuteSqlCommand(sql);
    //                }
    //            }
    //        }
    //        catch(Exception ex)
    //        {
    //            result = ex.Message;
    //            log.Error(ex);
    //        }
    //        return result;
    //    }

    //}

  

}
