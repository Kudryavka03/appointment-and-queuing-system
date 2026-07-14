using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderClientV2
{
    public partial class Form1 : Form
    {
        public static string OperatorCenterURL = "";
        public static int OperatorID = 0;

        private readonly HttpClient httpClient = new HttpClient();
        private readonly OrderRealtimeClient realtimeClient = new OrderRealtimeClient();
        private RealtimeOrderSnapshot currentSnapshot;
        private bool reconnecting;
        private string AllWindows = "";
        private string typeStr = "";
        private TextBox SetWindowCountText;
        private Button ApplyWindowCountBtn;
        private Button SaveStateBtn;
        private Button LoadStateBtn;

        public Form1()
        {
            InitializeComponent();
            EnsureWindowCountControls();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (currentSnapshot?.Logs != null)
            {
                ApplyLogs(currentSnapshot.Logs);
                return;
            }

            try
            {
                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetLog");
                LogBox1.Text = result;
                LogBox1.Select(LogBox1.TextLength, 0);
                LogBox1.ScrollToCaret();
            }
            catch
            {
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            realtimeClient.SnapshotReceived += RealtimeClient_SnapshotReceived;
            realtimeClient.CommandResultReceived += RealtimeClient_CommandResultReceived;
            realtimeClient.ConnectionStatusChanged += RealtimeClient_ConnectionStatusChanged;

            try
            {
                JObject jObject = JObject.Parse(File.ReadAllText("conf.json"));
                OperatorCenterURL = jObject["OperatorCenterURL"]?.ToString() ?? "http://127.0.0.1:888";
                OperatorID = jObject["OperatorID"]?.ToObject<int>() ?? 0;
                SetServerURL.Text = OperatorCenterURL;
                SetWindowCountText.Text = (jObject["WindowCount"]?.ToString() ?? "5");
            }
            catch
            {
                string settingText = JsonConvert.SerializeObject(new SettingClass
                {
                    OperatorCenterURL = "http://127.0.0.1:888",
                    OperatorID = 1,
                    WindowCount = 5
                });
                File.WriteAllText("conf.json", settingText);
                MessageBox.Show("因无法解析调度配置，配置已重置！");
                OperatorCenterURL = "http://127.0.0.1:888";
                OperatorID = 1;
                SetServerURL.Text = OperatorCenterURL;
                SetWindowCountText.Text = "5";
            }

            timer1.Enabled = true;
            toolStripStatusLabel1.Text = "未连接到调度中心";
            _ = ConnectRealtimeAsync();
        }

        private async Task ConnectRealtimeAsync()
        {
            if (reconnecting || string.IsNullOrWhiteSpace(OperatorCenterURL)) return;
            reconnecting = true;
            try
            {
                await realtimeClient.ConnectAsync(OperatorCenterURL);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "未连接到调度中心：" + ex.Message;
            }
            finally
            {
                reconnecting = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string settingText = JsonConvert.SerializeObject(new SettingClass
            {
                OperatorCenterURL = SetServerURL.Text,
                OperatorID = 0,
                WindowCount = GetWindowCountFromText()
            });
            OperatorCenterURL = SetServerURL.Text;
            OperatorID = 0;
            File.WriteAllText("conf.json", settingText);
            _ = ConnectRealtimeAsync();
            MessageBox.Show("配置保存成功");
        }

        private async void ApplyWindowCountBtn_Click(object sender, EventArgs e)
        {
            int count = GetWindowCountFromText();
            string result = await SendCommandOrHttpAsync(new { Action = "SetWindowCount", WindowCount = count }, "/SetStatus/SetWindowCount/" + count);
            if (!string.IsNullOrWhiteSpace(result)) toolStripStatusLabel1.Text = result;
        }

        private async void SaveStateBtn_Click(object sender, EventArgs e)
        {
            string result = await SendCommandOrHttpAsync(new { Action = "SaveState" }, "/GetStatus/SaveState");
            toolStripStatusLabel1.Text = string.IsNullOrWhiteSpace(result) ? "状态保存命令已发送" : result;
        }

        private async void LoadStateBtn_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "服务端启动时会全新开始，并把上一次状态快照归档。\r\n\r\n确定要加载上一次归档状态吗？当前服务端状态将被覆盖。", 
                "确认加载状态", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning, 
                MessageBoxDefaultButton.Button2);
            if (confirm != DialogResult.Yes) return;

            string result = await SendCommandOrHttpAsync(new { Action = "ReloadState" }, "/GetStatus/LoadState");
            toolStripStatusLabel1.Text = string.IsNullOrWhiteSpace(result) ? "状态加载命令已发送" : result;
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            try
            {
                string mask = BuildTypeMask(checkBox1.Checked, checkBox2.Checked);
                if (mask == "00")
                {
                    MessageBox.Show("您还未勾选取号类型，请确认取号者需要办理的业务类型。");
                    return;
                }

                checkBox1.Checked = false;
                checkBox2.Checked = false;
                await SendCommandOrHttpAsync(new { Action = "GenNewUuid", TypeMask = mask }, "/GetStatus/GenNewUuid/" + mask);
            }
            finally
            {
                button4.Enabled = true;
            }
        }

        public void setWindow(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text == AllWindows) return;

            AllWindows = text;
            var items = text.Split(',').Where(item => !string.IsNullOrWhiteSpace(item)).ToArray();
            var combos = new[] { comboBox1, comboBox2, comboBox3, comboBox4 };
            foreach (var combo in combos)
            {
                string selected = combo.Text;
                combo.Items.Clear();
                combo.Items.AddRange(items);
                if (combo.Items.Count > 0)
                {
                    combo.SelectedItem = combo.Items.Contains(selected) ? selected : combo.Items[0];
                }
            }
        }

        public void setTypeStr(string type)
        {
            if (string.IsNullOrWhiteSpace(type) || typeStr == type) return;

            typeStr = type;
            TypeListBox.Items.Clear();
            foreach (var item in typeStr.Split(','))
            {
                if (!string.IsNullOrWhiteSpace(item)) TypeListBox.Items.Add(item);
            }
            if (TypeListBox.Items.Count > 0) TypeListBox.SelectedIndex = 0;
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (currentSnapshot != null)
                {
                    ApplySnapshot(currentSnapshot);
                    if (!realtimeClient.IsConnected && !reconnecting) _ = ConnectRealtimeAsync();
                    return;
                }

                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetCurrentUuid");
                numLabel.Text = result;
                result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetAllWindows");
                setWindow(result);
                result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetAllType");
                setTypeStr(result);
                toolStripStatusLabel1.Text = "已连接到调度中心";
                button4.Enabled = true;
            }
            catch
            {
                toolStripStatusLabel1.Text = "未连接到调度中心";
                if (!realtimeClient.IsConnected && !reconnecting) _ = ConnectRealtimeAsync();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = checkBox3.Checked;
        }

        private async void SetWindowTypeBtn_Click(object sender, EventArgs e)
        {
            string mask = BuildTypeMask(checkBox5.Checked, checkBox4.Checked);
            if (mask == "00")
            {
                MessageBox.Show("您还未勾选取号类型，请确认需要办理的业务类型。");
                return;
            }

            await SendCommandOrHttpAsync(new { Action = "SetTypes", Window = ToWindow(comboBox1.Text), TypeMask = mask }, $"/SetStatus/SetTypes/{comboBox1.Text}/{mask}");
            RefreshWindowTypeInfo();
        }

        private async void WindowStopBtn_Click(object sender, EventArgs e)
        {
            string action = $"/CallAction/{comboBox1.Text}/STOP";
            await SendCommandOrHttpAsync(new { Action = "CallStop", Window = ToWindow(comboBox1.Text) }, action);
            RefreshWindowTypeInfo();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string action = $"/CallAction/{comboBox1.Text}/START";
            await SendCommandOrHttpAsync(new { Action = "CallStart", Window = ToWindow(comboBox1.Text) }, action);
            RefreshWindowTypeInfo();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string action = $"/SetStatus/{comboBox1.Text}/COMPLETED";
            await SendCommandOrHttpAsync(new { Action = "CallFinished", Window = ToWindow(comboBox1.Text) }, action);
            RefreshWindowTypeInfo();
        }

        private void RefreshWindowTypeInfoBtn_Click(object sender, EventArgs e)
        {
            RefreshWindowTypeInfo();
        }

        private async void RefreshWindowTypeInfo()
        {
            try
            {
                if (currentSnapshot != null)
                {
                    UpdateSelectedWindowInfo();
                    return;
                }

                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/" + comboBox1.Text);
                WinNumLabel.Text = result;
                result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetWindowType/" + comboBox1.Text);
                TypeLabel.Text = result;
            }
            catch
            {
                toolStripStatusLabel1.Text = "未连接到调度中心";
            }
        }

        private async void GetLogA()
        {
            if (currentSnapshot?.Logs != null)
            {
                ApplyLogs(currentSnapshot.Logs);
                return;
            }

            try
            {
                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetLog");
                LogBox1.Text = "";
                LogBox1.AppendText(result);
                LogBox1.ScrollToCaret();
            }
            catch
            {
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                RefreshWindowTypeInfo();
                GetLogA();
            }
        }

        private void RefreshTodayInfo_Click(object sender, EventArgs e)
        {
            if (currentSnapshot == null) return;
            MessageBox.Show($"已叫号：{currentSnapshot.LastestOrderNum}\r\n总取号：{currentSnapshot.Uuid}\r\n等待人数：{currentSnapshot.WaitingCount}\r\n最近10分钟办理：{currentSnapshot.ReportSpeed}");
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string result = await SendCommandOrHttpAsync(new { Action = "GenHighLevelPass", Number = ToWindow(textBox2.Text) }, "/GetStatus/GenNewHighLevelPassUuid/" + textBox2.Text);
                if (!string.IsNullOrWhiteSpace(result)) ReCallResultLabel.Text = result;
            }
            catch
            {
                toolStripStatusLabel1.Text = "未连接到调度中心";
            }
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string result = await SendCommandOrHttpAsync(new { Action = "GenHighLevel", OriginalOperator = 0, TargetWindow = ToWindow(comboBox2.Text), Message = "插队" }, "/GetStatus/GenNewHighLevelUuid/" + "0," + comboBox2.Text + ",插队");
                if (!string.IsNullOrWhiteSpace(result)) InsertResultLabel.Text = result;
            }
            catch
            {
                toolStripStatusLabel1.Text = "未连接到调度中心";
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            try
            {
                string result = await SendCommandOrHttpAsync(new { Action = "GenHighLevel", OriginalOperator = ToWindow(comboBox3.Text), TargetWindow = ToWindow(comboBox4.Text), Message = "转窗" }, "/GetStatus/GenNewHighLevelUuid/" + $"{comboBox3.Text}," + comboBox4.Text + ",转窗");
                if (!string.IsNullOrWhiteSpace(result)) ChangeWindowLabelNum.Text = result;
            }
            catch
            {
                toolStripStatusLabel1.Text = "未连接到调度中心";
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
                if (currentSnapshot != null)
                {
                    foreach (var window in currentSnapshot.Windows ?? Array.Empty<RealtimeWindowSnapshot>())
                    {
                        message += $"{window.Window}号窗 当前分配：{window.DisplayText}      业务类型：{window.TypeText}\r\n";
                    }
                    MessageBox.Show(message);
                    return;
                }

                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    string typeResult = await httpClient.GetStringAsync(OperatorCenterURL + "/GetWindowType/" + comboBox1.Items[i]);
                    string statusResult = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/" + comboBox1.Items[i]);
                    message += ($"{i + 1}号窗 当前分配：{statusResult}      业务类型：{typeResult}\r\n");
                }
                MessageBox.Show(message);
            }
            catch
            {
                toolStripStatusLabel1.Text = "未连接到调度中心";
            }
        }

        private async Task<string> SendCommandOrHttpAsync(object command, string httpPath)
        {
            if (realtimeClient.IsConnected)
            {
                try
                {
                    await realtimeClient.SendCommandAsync(command);
                    return "";
                }
                catch
                {
                    toolStripStatusLabel1.Text = "WebSocket发送失败，已回退HTTP";
                }
            }

            return await httpClient.GetStringAsync(OperatorCenterURL + httpPath);
        }

        private void RealtimeClient_SnapshotReceived(RealtimeOrderSnapshot snapshot)
        {
            currentSnapshot = snapshot;
            SafeUi(() => ApplySnapshot(snapshot));
        }

        private void RealtimeClient_CommandResultReceived(string action, string result)
        {
            SafeUi(() =>
            {
                switch (action)
                {
                    case "GenHighLevelPass":
                        ReCallResultLabel.Text = result;
                        break;
                    case "GenHighLevel":
                        if (tabControl1.SelectedTab == tabPage5) ChangeWindowLabelNum.Text = result;
                        else InsertResultLabel.Text = result;
                        break;
                    case "SetWindowCount":
                    case "SaveState":
                    case "ReloadState":
                    case "SetTypes":
                        toolStripStatusLabel1.Text = result;
                        break;
                    case "error":
                        toolStripStatusLabel1.Text = "错误：" + result;
                        break;
                    default:
                        if (!string.IsNullOrWhiteSpace(result)) toolStripStatusLabel1.Text = action + "：" + result;
                        break;
                }
            });
        }

        private void RealtimeClient_ConnectionStatusChanged(string status)
        {
            SafeUi(() => toolStripStatusLabel1.Text = status);
        }

        private void SafeUi(Action action)
        {
            if (IsDisposed) return;
            if (!IsHandleCreated)
            {
                action();
                return;
            }
            BeginInvoke(action);
        }

        private void ApplySnapshot(RealtimeOrderSnapshot snapshot)
        {
            if (snapshot == null) return;
            numLabel.Text = snapshot.Uuid == 0 ? "请先取号" : snapshot.Uuid.ToString();
            setWindow(string.Join(",", (snapshot.Windows ?? Array.Empty<RealtimeWindowSnapshot>()).Select(w => w.Window.ToString())));
            setTypeStr(string.Join(",", snapshot.TypeNames ?? Array.Empty<string>()));
            if (!SetWindowCountText.Focused) SetWindowCountText.Text = snapshot.TotalWindowNum.ToString();
            ApplyLogs(snapshot.Logs);
            UpdateSelectedWindowInfo();
            toolStripStatusLabel1.Text = realtimeClient.IsConnected ? "已通过WebSocket连接调度中心" : toolStripStatusLabel1.Text;
        }

        private void UpdateSelectedWindowInfo()
        {
            if (currentSnapshot?.Windows == null || comboBox1.SelectedItem == null) return;
            int windowId = ToWindow(comboBox1.Text);
            var window = currentSnapshot.Windows.FirstOrDefault(w => w.Window == windowId);
            if (window == null) return;
            WinNumLabel.Text = window.DisplayText;
            TypeLabel.Text = window.TypeText;
            checkBox5.Checked = window.TypeMask != null && window.TypeMask.Length > 0 && window.TypeMask[0] == '1';
            checkBox4.Checked = window.TypeMask != null && window.TypeMask.Length > 1 && window.TypeMask[1] == '1';
        }

        private void ApplyLogs(string[] logs)
        {
            if (logs == null) return;
            LogBox1.Text = string.Join("\r\n", logs);
            LogBox1.Select(LogBox1.TextLength, 0);
            LogBox1.ScrollToCaret();
        }

        private void EnsureWindowCountControls()
        {
            Label label = new Label
            {
                AutoSize = true,
                Location = new Point(18, 58),
                Text = "窗口数量"
            };
            SetWindowCountText = new TextBox
            {
                Location = new Point(91, 55),
                Size = new Size(80, 27),
                Text = "5"
            };
            ApplyWindowCountBtn = new Button
            {
                Location = new Point(184, 52),
                Size = new Size(120, 34),
                Text = "应用窗口数量"
            };
            ApplyWindowCountBtn.Click += ApplyWindowCountBtn_Click;
            SaveStateBtn = new Button
            {
                Location = new Point(318, 52),
                Size = new Size(100, 34),
                Text = "保存状态"
            };
            SaveStateBtn.Click += SaveStateBtn_Click;
            LoadStateBtn = new Button
            {
                Location = new Point(432, 52),
                Size = new Size(100, 34),
                Text = "加载状态"
            };
            LoadStateBtn.Click += LoadStateBtn_Click;
            tabPage6.Controls.Add(label);
            tabPage6.Controls.Add(SetWindowCountText);
            tabPage6.Controls.Add(ApplyWindowCountBtn);
            tabPage6.Controls.Add(SaveStateBtn);
            tabPage6.Controls.Add(LoadStateBtn);
        }

        private int GetWindowCountFromText()
        {
            if (!int.TryParse(SetWindowCountText.Text, out int count)) count = 5;
            return Math.Max(1, Math.Min(99, count));
        }

        private static int ToWindow(string text)
        {
            return int.TryParse(text, out int value) ? value : 0;
        }

        private static string BuildTypeMask(bool sign, bool infoChange)
        {
            return (sign ? "1" : "0") + (infoChange ? "1" : "0");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            realtimeClient.Dispose();
            base.OnFormClosed(e);
        }
    }

    public class SettingClass
    {
        public string OperatorCenterURL;
        public int OperatorID;
        public int WindowCount;
    }
}
