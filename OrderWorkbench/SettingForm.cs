using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net.Http;

namespace OrderWorkbench
{
    public partial class SettingForm : Form
    {
        public static string OperatorCenterURL = "";

        public static int OperatorID = 0;
        public static string typeStr = "00";
        private HttpClient httpClient = new HttpClient();
        public SettingForm()
        {
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            try
            {
                JObject jObject = JObject.Parse(File.ReadAllText("conf.json"));
                OperatorCenterURL = jObject["OperatorCenterURL"].ToString();
                OperatorID = Convert.ToInt32(jObject["OperatorID"]);
                typeStr = jObject["TypeName"].ToString();
                switch (typeStr)
                {
                    case "00":
                        break;
                    case "01":
                        checkBox1.Checked = false;
                        checkBox2.Checked = true;
                        break;
                    case "10":
                        checkBox1.Checked = true;
                        checkBox2.Checked = false;
                        break;
                    case "11":
                        checkBox1.Checked = true;
                        checkBox2.Checked = true;
                        break;
                    default:
                        checkBox1.Checked = false;
                        checkBox2.Checked = false;
                        break;
                }


            }
            catch (Exception)
            {
                string SettingText = JsonConvert.SerializeObject(new SettingClass
                {
                    OperatorCenterURL = "http://127.0.0.1:888",
                    OperatorID = 1,
                    TypeName = "11"
                });
                File.WriteAllText("conf.json", SettingText);
                MessageBox.Show("因无法解析调度配置，配置已重置！");
                JObject jObject2 = JObject.Parse(File.ReadAllText("conf.json"));
                OperatorCenterURL = jObject2["OperatorCenterURL"].ToString();
                OperatorID = Convert.ToInt32(jObject2["OperatorID"]);
                
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            var typeStr = "";
            if (checkBox1.Checked)
            {
                typeStr += "1";
            }
            else typeStr += "0";

            if (checkBox2.Checked)
            {
                typeStr += "1";
            }
            else typeStr += "0";
            if (typeStr == "00")
            {
                MessageBox.Show("您还未勾选取号类型，请确认需要办理的业务类型。");
                button1.Enabled = true;
                return;
            }
            string SettingText = JsonConvert.SerializeObject(new SettingClass
            {
                OperatorCenterURL = SetServerURL.Text,
                OperatorID = Convert.ToInt32(SetNumText.Text),
                TypeName = typeStr
            });
            OperatorCenterURL = SetServerURL.Text;
            OperatorID = Convert.ToInt32(SetNumText.Text);
            File.WriteAllText("conf.json", SettingText);
            try
            {
                await httpClient.GetStringAsync(OperatorCenterURL + $"/SetStatus/SetTypes/{OperatorID}/{typeStr}");
                this.Hide();
                new Form1(OperatorCenterURL, OperatorID,typeStr).ShowDialog();
                this.Show();
                button1.Enabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("通讯失败。请在网络良好的环境下再次尝试，或检查后台是否已经启动");
                button1.Enabled = true;
            }

        }
    }
}
