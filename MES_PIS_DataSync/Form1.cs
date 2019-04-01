using ADODB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MES_PIS_DataSync
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.AutoSize = false;
            textBox1.Multiline = true;
            textBox1.Width = 300;//长，
            textBox1.Height = 80;//宽。
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var requestParam = textBox1.Text.ToString();
            MES_PIS_HandSyncBus Syncbus = new MES_PIS_HandSyncBus();
            var uid = 0;
            int.TryParse(requestParam, out uid);

            if (uid == 0)
            {
                if (MessageBox.Show(this, "请输入正确的同步ID？",
               "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                }
            }
            else
            {
                var syncModel = Syncbus.GetSyncFailedRecordByUID(uid);
                if (syncModel == null)
                {
                    if (MessageBox.Show(this, "未获取到需要同步的数据？",
                    "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    {

                    }
                }
                else
                {
                    try
                    {
                        var requestStr = syncModel.SyncRequest.Substring(syncModel.SyncRequest.IndexOf(':') + 1);
                        var reArray = requestStr.Split(',');
                        MesRequestParamModel mesRequest = new MesRequestParamModel();
                        mesRequest.SqlServer = reArray[0].ToString().Split('_')[1];
                        mesRequest.Database = reArray[1].ToString().Split('_')[1];
                        mesRequest.UserID_ID = int.Parse(reArray[2].ToString().Split('_')[1]);
                        mesRequest.StartDate = reArray[3].ToString().Split('_')[1];
                        mesRequest.EndDate = reArray[4].ToString().Split('_')[1];
                        mesRequest.CustomerIDString = reArray[5].ToString().Split('_')[1];
                        mesRequest.AssemblyIDString = reArray[5].ToString().Split('_')[1];

                        Recordset resultMesData = new Recordset();
                        resultMesData = Syncbus.GetMesAPIData(mesRequest);
                        var currenDate = System.Convert.ToDateTime(mesRequest.StartDate);
                        var currentDateInfoModel = Syncbus.GetCurrentTime(currenDate);
                        var MesPeocessInfoList = Syncbus.ConvertMesDateToPis(resultMesData);
                        var result = Syncbus.AddMES_PIS(uid, MesPeocessInfoList, currentDateInfoModel, mesRequest);
                        if (result)
                        {
                            if (MessageBox.Show(this, "同步成功？",
                            "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                            {
                            }
                        }
                        else
                        {
                            if (MessageBox.Show(this, "同步失败？",
                            "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("同步失败:" + ex.Message.ToString());
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.AutoSize = false;
            textBox1.Multiline = true;
            textBox1.Width = 300;//长，
            textBox1.Height = 80;//宽。
        }
    }
}
