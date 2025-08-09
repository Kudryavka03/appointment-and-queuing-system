using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using System.Globalization;
using System.Runtime.InteropServices;

namespace OrderScreen
{
    public partial class OrderClientOperatorForm : Form
    {
        private StringBuilder resultTextBuilder = new StringBuilder();
        public static string OperatorCenterURL = "";
        public static int OperatorID = 0;
        HttpClient httpClient = new HttpClient();
        int[] arr0 = new int[5] { -1, -1, -1, -1, -1 };
        static ArrayList speakList = new ArrayList();
        bool isExitFlags = false;
        public string title;

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);
        public OrderClientOperatorForm()
        {
            InitializeComponent();
        }

        private void timeLabel_Click(object sender, EventArgs e)
        {

        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                TopMost = true;
                SetForegroundWindow(base.Handle);
                SetWindowPos(base.Handle, -1, 0, 0, 0, 0, 1 | 2);
                SetActiveWindow(base.Handle);
                SetForegroundWindow(base.Handle);
            }
            catch (Exception)
            {
            }
 
            string time = DateTime.Now.ToString();
            label1.Text = title + "      " + time;

            if (Window1Label.Font.Size != (float)OperatorID)
            {
                Window1Label.Font = new Font("微软雅黑", OperatorID, FontStyle.Bold);
            }
            try
            {
                int[] arr = new int[5];
                resultTextBuilder.Clear();
                // speakList = new ArrayList();
                var current_status = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetCurrentLastestOrder");
                var lastest_id = Convert.ToInt32(current_status.Split(',')[1]);
                var lastest_order = Convert.ToInt32(current_status.Split(',')[0]);
                var lastest_wait = Convert.ToInt32(current_status.Split(',')[2]);
                for (int i = 1; i < 6; i++)
                {
                    var id = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/" + i);
                    // var lastest_id = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetCurrentUuid");

                    // Thread.Sleep(50);
                    var idid = id.Length < 2 ? $"0{id}" : id;
                    Debug.WriteLine(idid);
                    int idnum;
                    try
                    {
                        if (!id.Contains("分配"))
                            idnum = Convert.ToInt32(id);
                        else idnum = -1;
                        // Debug.WriteLine(idnum);
                    }
                    catch
                    {
                        idnum = -1;

                    }
                    arr[i - 1] = idnum;
                    if (arr[i - 1] != arr0[i - 1])
                    {
                        // Debug.WriteLine(idnum);
                        if (idnum != -1)
                        {
                            arr0[i - 1] = idnum;
                            speakList.Add($"请 {idid} 号用户到 {i} 号窗口办理业务。\r\n");
                        }
                    }

                    if (!id.Contains("分配")) resultTextBuilder.Append($"请 {idid} 号用户到 {i} 号窗口办理业务。\r\n");
                    else if (!id.Contains("暂停")) resultTextBuilder.Append($"{i} 号窗口无业务\r\n");
                    else resultTextBuilder.Append($"{i} 号窗口暂停服务\r\n");
                }
                if (lastest_wait != 0)
                {
                    resultTextBuilder.Append($"请 {lastest_order + 1} 号用户准备 | 等待排队：{lastest_id - lastest_order} 人\r\n");
                }
                Window1Label.Text = resultTextBuilder.ToString();
                this.Text = "叫号大屏 - 已连接到调度中心";
            }
            catch
            {
                this.Text = "叫号大屏 - 未连接到调度中心";
            }
        }
        public void speakA()
        {
            int index = 0;
            var synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
            // synthesizer.SelectVoice("Microsoft Huihui");
            // synthesizer.
            while (!isExitFlags)
            {
                if (speakList.Count > index)
                {
                    try
                    {
                        // Debug.WriteLine(speakList.Count);
                        synthesizer.Volume = 100;
                        synthesizer.SpeakAsync("，注意");
                        synthesizer.SpeakAsync(speakList[index].ToString());
                        synthesizer.SpeakAsync(speakList[index].ToString());
                        synthesizer.SpeakAsync(speakList[index].ToString());
                        // Debug.WriteLine(speakList[index]);

                    }
                    catch (Exception e) { Debug.WriteLine(e); }
                    index++;

                }
                Thread.Sleep(100);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // this.WindowState = FormWindowState.Maximized;
            base.TopMost = true;
            try
            {
                JObject jObject = JObject.Parse(File.ReadAllText("conf.json"));
                OperatorCenterURL = jObject["OperatorCenterURL"].ToString();
                OperatorID = Convert.ToInt32(jObject["OperatorID"]);
                title = jObject["title"].ToString();
                if (title == "")
                {
                    title = "学生发展中心 叫号大屏";
                }
                if (OperatorID < 5)
                {
                    OperatorID = 62;
                }
            }
            catch (Exception)
            {
                string SettingText = JsonConvert.SerializeObject(new SettingClass
                {
                    OperatorCenterURL = "http://127.0.0.1:888",
                    OperatorID = 62,
                    title = "学生发展中心 叫号大屏"
                });
                File.WriteAllText("conf.json", SettingText);
                MessageBox.Show("因无法解析调度配置，配置已重置！");
                JObject jObject2 = JObject.Parse(File.ReadAllText("conf.json"));
                OperatorCenterURL = jObject2["OperatorCenterURL"].ToString();
                OperatorID = Convert.ToInt32(jObject2["OperatorID"]);
                title = "学生发展中心 叫号大屏";
                new SettingForm().ShowDialog();
            }
            timer1.Enabled = true;
            Text = "叫号大屏 - 未连接到调度中心";
            new Thread(speakA).Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled=false;
            this.TopMost = false;
            SettingForm settingForm = new SettingForm();
            settingForm.ShowDialog();
            timer1.Enabled = true;
            this.TopMost = true;
        }

        private void OrderClientOperatorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            isExitFlags = true;
        }

        private void OrderClientOperatorForm_Resize(object sender, EventArgs e)
        {
            button1.Location = new System.Drawing.Point(this.Width - button1.Width-30,0);
            

        }
    }
    public class SettingClass
    {
        public string OperatorCenterURL;
        public int OperatorID;
        public string title;
    }
}
