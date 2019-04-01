using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class FixturePartScanCodeDTO : EntityDTOBase
    {

        /// <summary>
        /// 治具主檔流水號
        /// </summary>
        public int Fixture_M_UID { get; set; }

        /// <summary>
        /// 治具編號(圖號)
        /// </summary>
        public string Fixture_NO { get; set; }
        /// <summary>
        /// 治具唯一編號
        /// </summary>
        public string Fixture_Unique_ID { get; set; }
        /// <summary>
        /// 治具名稱
        /// </summary>
        public string Fixture_Name { get; set; }
        /// <summary>
        /// 治具短碼
        /// </summary>
        public string ShortCode { get; set; }
        /// <summary>
        /// 治具二維條碼
        /// </summary>
        public string TwoD_Barcode { get; set; }

        /// <summary>
        /// 扫码总数
        /// </summary>
        public int? UseTimesTotal { get; set; }

        /// <summary>
        /// 当前扫码时间
        /// </summary>
        public DateTime? ScanDateTime{ get; set; }
        /// <summary>
        /// 上次扫码时间
        /// </summary>
        public DateTime? NextScanDateTime { get; set; }
        /// <summary>
        /// 扫码配件明细
        /// </summary>
        public List<FixturePartScanDTO> FixturePartScanDTOs { get; set; }
        /// <summary>
        /// 返回的异常信息
        /// </summary>
        public string Messages { get; set; }
        /// <summary>
        /// 1.代表成功。0.代表失败
        /// </summary>
        public int Code { get; set; }


        public  string ProjectName { get; set; }

        public int? UseTimesScanInterval { get; set; }
    }
}
