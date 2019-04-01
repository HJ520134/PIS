using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Common.Common
{
    public class EmailFormat
    {
        #region layout
        /// <summary>
        /// layout
        /// </summary>
        public class layout
        {
            /// <summary>
            /// layout 的主旨
            /// </summary>
            public string Layout_Subject { get; set; }
            /// <summary>
            /// layout 的內容
            /// </summary>
            public string Layout_Content { get; set; }
        }

        /// <summary>
        /// 無 Layout
        /// </summary>
        /// <param name="subject">主旨</param>
        /// <param name="content">內文</param>
        /// <returns></returns>
        public layout NotLayout(string subject, string content)
        {
            layout layout = new layout();
            layout.Layout_Subject = subject;
            layout.Layout_Content = content;
            return layout;
        }

        public layout Layout_SapErrorData(string subject, string content)
        {
            layout layout = new layout();
            string layout_Content = string.Empty;
            layout_Content = @"
<body style='margin:0px; padding:0px'>
    <table width='100%' border='0' align='left'  valign='top'  cellpadding='0' cellspacing='3'>
      <tr>
        <td style='padding-top:5px'>"
            + "<img src =\'cid:LayoutImg0\' style='height:48px; width:auto' />" +
            @"</td>
      </tr>
      <tr>
        <td>      
              <table style='height: 6px; width: 100%; border: 0px;' border='0' cellpadding='0' cellspacing='0'>
                <tr style='height: 5px;'>
                    <td> "
                        + "<img src =\'cid:LayoutImg1\' width='100%' />" +
                    @"</td>
                </tr>

             </table>
        </td>
      </tr>
      <tr>
        <td>
          <!--這是標題td-->
          <h1 style='font-size:18px; color:#00764B; margin:5px 0px; font-weight:bold'>" +
          subject
          + @"</h1>
          <!--標題td結束-->
        </td>
      </tr>
      <tr>
        <td align='left' valign='top' style='padding-bottom:30px; padding-top:10px; font-size:12px ; color: #333; '>" +
        content
        + @"</td>
      </tr>
      <tr>
        <td>
          <table width='100%' border='0' cellpadding='0' cellspacing='0'>
            <tr>
              <td style='width:87px'>"
                + "<img src=\"cid:LayoutImg2\" width='87' height='16'/>" +
                  @"</td>
              <td valign='middle' style='padding-right:5px'>
           
             <table style='height: 1px; width: 100%; border: 0px;' border='0' cellpadding='0' cellspacing='0'>
                <tr style='height: 1px;'>
                    <td style='border:0; height:1px; background-color:#d4d4d4; width: 100%'></td>
                </tr>
             </table>


              </td>
            </tr>
          </table>
        </td>
      </tr>
      <tr>
        <td style='padding:0px 5px 5px 20px'>
          <table width='100%' border='0' cellspacing='0' cellpadding='0'>
            <tr>
              <td height='40' align='left' valign='middle' width='120px'>
                <table border='0' align='left' cellpadding='0' cellspacing='0'>
                  <tr>
                    <td align='left' valign='middle' >
                      <a href='" + new ServerInfoUtility().WebPath + "' target='_blank' style='text-decoration:none;'>"
                          + "<img src =\'cid:LayoutImg3\' />" +
                      @"</a>
                    </td>
                  </tr>
                </table>
              </td>
              <td style='padding-top:5px; color:#22415d;'>
                  This is an automated message, please do not reply.
              </td>
            </tr>

          </table>
        </td>
      </tr>
      <tr>
        <td valign='middle' style='padding-right:5px; padding-left:5px; '>
	

            <table style='height: 2px; width: 100%; border: 0px;' border='0' cellpadding='0' cellspacing='0'>
                <tr style='height: 1px;'>
                    <td style='border:0; height:1px; background-color: #CCC; margin:0px; width: 100%'></td>
                </tr>
                <tr style='height: 1px;'>
                    
                </tr>
             </table>

        </td>
      </tr>
      <tr>
        <td>
            <p style='padding:5px 10px ; color:#999; font-size:10px; margin:0px'>The information contained in this e-mail message (and any attachment transmitted herewith) is for the exclusive use of intended addressee(s) only and is confidential information. If you have received this electronic message in error, please reply to the sender highlighting the error and destroy the original message and all copies immediately.</p>
    	    <p style='padding:2px 10px 5px 10px; color:#999; font-size:10px; margin:0px'>本郵件（包括其附件）所含訊息為保密訊息，僅為特定收件人之用。如你誤收此郵件，請立即通知發件人，並將郵件銷毀。</p>
        </td>
      </tr>
      <tr style='border-bottom: double 4px #d4d4d4'>
        <td><p style='padding:5px 10px ; color:#999; font-size:10px; margin:0px'>"
            + "<img src=\"cid:LayoutImg4\" /></p>" + @"

             <table style='height: 1px; width: 100%; border: 0px;' border='0' cellpadding='0' cellspacing='0'>
                <tr style='height: 1px;'>
                    <td style='border:0; height:1px; background-color: #CCC; margin:1px 0px 0px 0px; width: 100%'></td>
                </tr>
             </table>

        </td>
      </tr>
    </table>
</body>";
            layout.Layout_Subject = subject;
            layout.Layout_Content = layout_Content;
            //layout.ContentImgPathList = contentImgPathList;
            return layout;
        }
        #endregion  

        #region LayoutAttachmentType
        /// <summary>
        /// Email 夾帶附件類別
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<string> Email_Type(int type)
        {
            List<string> list = new List<string>();
            switch (type)
            {
                //無Approve and Reject 按鈕
                case 1:
                    list.Add("Content/img/logo/logo1.png");
                    list.Add("Content/img/mail/line-head01.png");
                    list.Add("Content/img/mail/mail01-system-info01.png");
                    list.Add("Content/img/mail/mail01-gotoPDMS.png");
                    list.Add("Content/img/mail/Copyright.png");
                    break;
                //有Approve and Reject 按鈕
                case 2:
                    list.Add("Content/img/logo/logo1.png");
                    list.Add("Content/img/mail/mail01-system-info01.png");
                    list.Add("Content/img/mail/mail01-gotoSPP.png");
                    list.Add("Content/img/mail/Copyright.jpg");
                    list.Add("Content/img/mail/btn-Approve.png");
                    list.Add("Content/img/mail/btn-Reject.png");
                    break;
                // For Quote 報價單
                case 3:
                    list.Add("Content/img/logo/GreenPoint_logo.png");
                    list.Add("Content/img/logo/Jabil_logo.png");
                    break;
                default:
                    break;
            }
            return list;
        }
        #endregion
    }
}
