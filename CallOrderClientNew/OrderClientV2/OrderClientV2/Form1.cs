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
using System.Net.Http;

namespace OrderClientV2
{
    public partial class Form1 : Form
    {
        public static string OperatorCenterURL = "";

        public static int OperatorID = 0;

        private HttpClient httpClient = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetLog");
                LogBox1.Text = result;
                LogBox1.Select(LogBox1.TextLength, 0);
                LogBox1.ScrollToCaret();
            }
            catch (Exception)
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                JObject jObject = JObject.Parse(File.ReadAllText("conf.json"));
                OperatorCenterURL = jObject["OperatorCenterURL"].ToString();

                OperatorID = Convert.ToInt32(jObject["OperatorID"]);
                SetServerURL.Text = OperatorCenterURL;
            }
            catch (Exception)
            {
                string SettingText = JsonConvert.SerializeObject(new SettingClass
                {
                    OperatorCenterURL = "http://127.0.0.1:888",
                    OperatorID = 1
                });
                File.WriteAllText("conf.json", SettingText);
                MessageBox.Show("因无法解析调度配置，配置已重置！");
                JObject jObject2 = JObject.Parse(File.ReadAllText("conf.json"));
                OperatorCenterURL = jObject2["OperatorCenterURL"].ToString();
                OperatorID = Convert.ToInt32(jObject2["OperatorID"]);
                // new SettingForm().ShowDialog();
            }
            timer1.Enabled = true;

            toolStripStatusLabel1.Text = "未连接到调度中心 ";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string SettingText = JsonConvert.SerializeObject(new SettingClass
            {
                OperatorCenterURL = SetServerURL.Text,
                OperatorID = 0
            });
            OperatorCenterURL = SetServerURL.Text;
            OperatorID = 0;
            File.WriteAllText("conf.json", SettingText);
            MessageBox.Show("配置保存成功");
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
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
                MessageBox.Show("您还未勾选取号类型，请确认取号者需要办理的业务类型。");
                return;
            }
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GenNewUuid/" + typeStr);

        }
        string AllWindows = "";
        public void setWindow(string text)
        {
            if (text != AllWindows)
            {
                AllWindows = text;
                var a = text.Split(',');
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                comboBox4.Items.Clear();
                foreach (var item in a)
                {
                    comboBox1.Items.Add(item);
                    comboBox2.Items.Add(item);
                    comboBox3.Items.Add(item);
                    comboBox4.Items.Add(item);
                }
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
                comboBox4.SelectedIndex = 0;

            }
        }
        string typeStr = "";
        public void setTypeStr(string type)
        {
            if (typeStr != type)
            {
                typeStr = type;
                TypeListBox.Items.Clear();
                var a = typeStr.Split(",");
                var b = new List<string>();


                foreach (var item in a)
                {
                    TypeListBox.Items.Add(item);
                }
                TypeListBox.SelectedIndex = 0;
            }

        }
        private async void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetCurrentUuid");
                numLabel.Text = result;
                result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetAllWindows");
                setWindow(result);
                result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetAllType");
                setTypeStr(result);
                toolStripStatusLabel1.Text = "已连接到调度中心 ";
                button4.Enabled = true;
            }
            catch (Exception)
            {
                toolStripStatusLabel1.Text = "未连接到调度中心 ";
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = checkBox3.Checked;
        }

        private async void SetWindowTypeBtn_Click(object sender, EventArgs e)
        {
            var typeStr = "";
            if (checkBox5.Checked)
            {
                typeStr += "1";
            }
            else typeStr += "0";

            if (checkBox4.Checked)
            {
                typeStr += "1";
            }
            else typeStr += "0";
            if (typeStr == "00")
            {
                MessageBox.Show("您还未勾选取号类型，请确认需要办理的业务类型。");
                return;
            }
            await httpClient.GetStringAsync(OperatorCenterURL + $"/SetStatus/SetTypes/{comboBox1.Text}/{typeStr}");
            RefreshWindowTypeInfo();
        }

        private async void WindowStopBtn_Click(object sender, EventArgs e)
        {
            string action = $"/CallAction/{comboBox1.Text}/STOP";
            await httpClient.GetStringAsync(OperatorCenterURL + action);
            RefreshWindowTypeInfo();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string action = $"/CallAction/{comboBox1.Text}/START";
            await httpClient.GetStringAsync(OperatorCenterURL + action);
            RefreshWindowTypeInfo();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string action = $"/SetStatus/{comboBox1.Text}/COMPLETED";
            await httpClient.GetStringAsync(OperatorCenterURL + action);
            RefreshWindowTypeInfo();
        }

        private async void RefreshWindowTypeInfoBtn_Click(object sender, EventArgs e)
        {
            RefreshWindowTypeInfo();
        }

        private async void RefreshWindowTypeInfo()
        {
            try
            {
                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/" + comboBox1.Text);
                WinNumLabel.Text = result;
                result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetWindowType/" + comboBox1.Text);
                TypeLabel.Text = result;
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "未连接到调度中心 ";
            }
        }
        private async void GetLogA()
        {
            try
            {
                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetLog");
                LogBox1.Text = "";
                LogBox1.AppendText(result);
                LogBox1.ScrollToCaret();
            }
            catch (Exception)
            {

            }
        }

        private async void timer2_Tick(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                RefreshWindowTypeInfo();
                GetLogA();
            }
        }

        private void RefreshTodayInfo_Click(object sender, EventArgs e)
        {

        }

        private async void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GenNewHighLevelPassUuid/" + textBox2.Text);
                ReCallResultLabel.Text = result;
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "未连接到调度中心 ";
            }

        }

        private async void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GenNewHighLevelUuid/" + "0," + comboBox2.Text + ",插队");
                InsertResultLabel.Text = result;
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "未连接到调度中心 ";
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            try
            {


                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GenNewHighLevelUuid/" + $"{comboBox3.Text}," + comboBox4.Text + ",转窗");
                ChangeWindowLabelNum.Text = result;
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "未连接到调度中心 ";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshWindowTypeInfo();
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            string message = "";
            try
            {
                for (int i = 0; i< comboBox1.Items.Count; i++) {
                    string typeResult = await httpClient.GetStringAsync(OperatorCenterURL + "/GetWindowType/" + comboBox1.Items[i]);
                    
                    string statusResult = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/" + comboBox1.Items[i]);

                    message+=($"{i+1}号窗 当前分配：{statusResult}      业务类型：{typeResult}\r\n");
                }
                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "未连接到调度中心 ";
            }
        }
    }


    public class SettingClass
    {
        public string OperatorCenterURL;

        public int OperatorID;
    }

}
