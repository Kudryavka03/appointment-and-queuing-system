using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrderScreen
{
    public partial class OrderClientOperatorForm : Form
    {
        public static string OperatorCenterURL = "";
        public static int OperatorID = 62;
        public static string title = "学生发展中心 叫号大屏";
        public static string theme = "深海蓝";
        public static bool voiceEnabled = true;
        public static int voiceRepeatCount = 3;
        public static int cardFontSize = 72;
        public static int titleFontSize = 34;

        private readonly HttpClient httpClient = new HttpClient();
        private readonly OrderRealtimeClient realtimeClient = new OrderRealtimeClient();
        private readonly Dictionary<int, WindowCard> windowCards = new Dictionary<int, WindowCard>();
        private readonly Queue<string> speakQueue = new Queue<string>();
        private readonly object speakLock = new object();
        private readonly Dictionary<int, int> spokenWindowNums = new Dictionary<int, int>();
        private TableLayoutPanel mainLayout;
        private Panel headerPanel;
        private Label titleLabel;
        private Label timeLabel;
        private Label statusLabel;
        private Label summaryLabel;
        private FlowLayoutPanel cardsPanel;
        private Panel footerPanel;
        private Label prepareLabel;
        private Label waitLabel;
        private Label speedLabel;
        private bool isExitFlags = false;
        private bool reconnecting;
        private RealtimeOrderSnapshot currentSnapshot;

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);

        public OrderClientOperatorForm()
        {
            InitializeComponent();
            BuildScreenUi();
        }

        private void timeLabel_Click(object sender, EventArgs e)
        {
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            KeepWindowOnTop();
            timeLabel.Text = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");

            if (currentSnapshot == null)
            {
                await PollHttpSnapshotAsync();
            }

            if (!realtimeClient.IsConnected && !reconnecting)
            {
                _ = ConnectRealtimeAsync();
            }
        }

        public void speakA()
        {
            using (var synthesizer = new SpeechSynthesizer())
            {
                synthesizer.SetOutputToDefaultAudioDevice();
                while (!isExitFlags)
                {
                string text = null;
                lock (speakLock)
                {
                    if (speakQueue.Count > 0) text = speakQueue.Dequeue();
                }

                if (!string.IsNullOrWhiteSpace(text) && voiceEnabled)
                {
                    try
                    {
                        synthesizer.Volume = 100;
                        synthesizer.SpeakAsync("请注意");
                        for (int i = 0; i < Math.Max(1, voiceRepeatCount); i++)
                        {
                            synthesizer.SpeakAsync(text);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
                Thread.Sleep(100);
            }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TopMost = true;
            LoadSettings();
            ApplyTheme();
            ApplyTypography();
            titleLabel.Text = title;
            timer1.Enabled = true;
            Text = "叫号大屏 - 未连接到调度中心";

            realtimeClient.SnapshotReceived += RealtimeClient_SnapshotReceived;
            realtimeClient.ConnectionStatusChanged += RealtimeClient_ConnectionStatusChanged;
            _ = ConnectRealtimeAsync();
            new Thread(speakA) { IsBackground = true }.Start();
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
                statusLabel.Text = "未连接";
                Text = "叫号大屏 - 未连接到调度中心";
                Debug.WriteLine(ex);
            }
            finally
            {
                reconnecting = false;
            }
        }

        private async Task PollHttpSnapshotAsync()
        {
            try
            {
                string json = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/Snapshot");
                var snapshot = JsonConvert.DeserializeObject<RealtimeOrderSnapshot>(json);
                ApplySnapshot(snapshot);
            }
            catch
            {
                statusLabel.Text = "未连接";
                Text = "叫号大屏 - 未连接到调度中心";
            }
        }

        private void RealtimeClient_SnapshotReceived(RealtimeOrderSnapshot snapshot)
        {
            currentSnapshot = snapshot;
            SafeUi(() => ApplySnapshot(snapshot));
        }

        private void RealtimeClient_ConnectionStatusChanged(string status)
        {
            SafeUi(() =>
            {
                statusLabel.Text = status.Contains("已连接") ? "实时连接" : status;
                Text = status.Contains("已连接") ? "叫号大屏 - 已连接到调度中心" : "叫号大屏 - 未连接到调度中心";
            });
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
            currentSnapshot = snapshot;
            statusLabel.Text = realtimeClient.IsConnected ? "实时连接" : "HTTP刷新";
            Text = "叫号大屏 - 已连接到调度中心";
            summaryLabel.Text = $"总取号 {snapshot.Uuid}   已叫 {snapshot.LastestOrderNum}   等待 {snapshot.WaitingCount}";
            prepareLabel.Text = snapshot.WaitingCount > 0 ? $"请 {snapshot.LastestOrderNum + 1} 号用户准备" : "当前暂无等待用户";
            waitLabel.Text = $"等待排队：{snapshot.WaitingCount} 人";
            speedLabel.Text = $"最近10分钟办理：{snapshot.ReportSpeed} 人";
            UpdateWindowCards(snapshot.Windows ?? Array.Empty<RealtimeWindowSnapshot>());
        }

        private void UpdateWindowCards(RealtimeWindowSnapshot[] windows)
        {
            var existing = new HashSet<int>(windowCards.Keys);
            foreach (var window in windows.OrderBy(w => w.Window))
            {
                existing.Remove(window.Window);
                if (!windowCards.TryGetValue(window.Window, out var card))
                {
                    card = new WindowCard();
                    windowCards[window.Window] = card;
                    cardsPanel.Controls.Add(card.Root);
                }
                UpdateCard(card, window);
                QueueSpeechIfNeeded(window);
            }

            foreach (int stale in existing)
            {
                cardsPanel.Controls.Remove(windowCards[stale].Root);
                windowCards[stale].Root.Dispose();
                windowCards.Remove(stale);
                spokenWindowNums.Remove(stale);
            }

            ResizeCards();
        }

        private void UpdateCard(WindowCard card, RealtimeWindowSnapshot window)
        {
            card.WindowLabel.Text = window.Window + " 号窗口";
            card.NumberLabel.Text = window.IsPaused ? "暂停" : (window.CurrentNum > 0 ? window.CurrentNum.ToString("00") : "--");
            card.StatusLabel.Text = window.IsPaused ? "暂停服务" : (window.IsBusy ? "正在办理" : "等待分配");
            card.TypeLabel.Text = NormalizeTypeText(window.TypeText);
            card.Root.BackColor = window.IsPaused ? Color.FromArgb(77, 88, 110) : (window.IsBusy ? Color.FromArgb(255, 255, 255) : Color.FromArgb(232, 245, 255));
            card.NumberLabel.ForeColor = window.IsPaused ? Color.WhiteSmoke : (window.IsBusy ? Color.FromArgb(11, 87, 164) : Color.FromArgb(68, 86, 110));
            card.WindowLabel.ForeColor = window.IsPaused ? Color.White : Color.FromArgb(33, 43, 64);
            card.StatusLabel.ForeColor = window.IsPaused ? Color.FromArgb(225, 230, 238) : Color.FromArgb(64, 76, 96);
            card.TypeLabel.ForeColor = card.StatusLabel.ForeColor;
        }

        private void QueueSpeechIfNeeded(RealtimeWindowSnapshot window)
        {
            if (!voiceEnabled || window.CurrentNum <= 0 || window.IsPaused) return;
            if (spokenWindowNums.TryGetValue(window.Window, out int oldNum) && oldNum == window.CurrentNum) return;

            spokenWindowNums[window.Window] = window.CurrentNum;
            lock (speakLock)
            {
                speakQueue.Enqueue($"请 {window.CurrentNum} 号用户到 {window.Window} 号窗口办理业务");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            TopMost = false;
            SettingForm settingForm = new SettingForm();
            settingForm.ShowDialog();
            LoadSettings();
            ApplyTheme();
            ApplyTypography();
            titleLabel.Text = title;
            timer1.Enabled = true;
            TopMost = true;
            _ = ConnectRealtimeAsync();
        }

        private void OrderClientOperatorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            isExitFlags = true;
            realtimeClient.Dispose();
        }

        private void OrderClientOperatorForm_Resize(object sender, EventArgs e)
        {
            button1.Location = new Point(Width - button1.Width - 30, 18);
            ResizeCards();
        }

        private void BuildScreenUi()
        {
            Controls.Clear();
            BackColor = Color.FromArgb(12, 22, 38);
            WindowState = FormWindowState.Maximized;

            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(42, 28, 42, 28),
                BackColor = Color.Transparent
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 128));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
            Controls.Add(mainLayout);

            headerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            titleLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Left,
                Width = 900,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };
            timeLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Right,
                Width = 360,
                ForeColor = Color.FromArgb(211, 224, 240),
                TextAlign = ContentAlignment.MiddleRight
            };
            statusLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Right,
                Width = 160,
                ForeColor = Color.FromArgb(142, 220, 181),
                TextAlign = ContentAlignment.MiddleRight
            };
            summaryLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Bottom,
                Height = 40,
                ForeColor = Color.FromArgb(202, 216, 235),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Microsoft YaHei UI", 18, FontStyle.Regular)
            };
            button1 = new Button
            {
                Text = "设置",
                Size = new Size(80, 32),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            button1.Click += button1_Click;
            headerPanel.Controls.Add(button1);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(statusLabel);
            headerPanel.Controls.Add(timeLabel);
            headerPanel.Controls.Add(summaryLabel);
            mainLayout.Controls.Add(headerPanel, 0, 0);

            cardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = false,
                WrapContents = true,
                Padding = new Padding(0, 14, 0, 14)
            };
            mainLayout.Controls.Add(cardsPanel, 0, 1);

            footerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            prepareLabel = new Label
            {
                Dock = DockStyle.Left,
                Width = 760,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Microsoft YaHei UI", 34, FontStyle.Bold)
            };
            speedLabel = new Label
            {
                Dock = DockStyle.Right,
                Width = 360,
                ForeColor = Color.FromArgb(202, 216, 235),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Microsoft YaHei UI", 18, FontStyle.Regular)
            };
            waitLabel = new Label
            {
                Dock = DockStyle.Right,
                Width = 280,
                ForeColor = Color.FromArgb(236, 196, 102),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Microsoft YaHei UI", 22, FontStyle.Bold)
            };
            footerPanel.Controls.Add(prepareLabel);
            footerPanel.Controls.Add(speedLabel);
            footerPanel.Controls.Add(waitLabel);
            mainLayout.Controls.Add(footerPanel, 0, 2);
        }

        private void ResizeCards()
        {
            if (cardsPanel == null || cardsPanel.Width <= 0 || windowCards.Count == 0) return;
            int count = Math.Max(1, windowCards.Count);
            int columns = count <= 4 ? 2 : (count <= 9 ? 3 : (count <= 16 ? 4 : 5));
            int rows = (int)Math.Ceiling(count / (double)columns);
            int cardWidth = Math.Max(360, (cardsPanel.ClientSize.Width - columns * 22) / columns);
            int cardHeight = Math.Max(150, (cardsPanel.ClientSize.Height - rows * 22) / rows);
            int numberFont = Math.Max(34, Math.Min(cardFontSize, cardHeight / 3));
            foreach (var card in windowCards.Values)
            {
                card.Root.Width = cardWidth;
                card.Root.Height = cardHeight;
                card.Root.Margin = new Padding(10);
                card.NumberLabel.Font = new Font("Microsoft YaHei UI", numberFont, FontStyle.Bold);
            }
        }

        private void ApplyTheme()
        {
            if (mainLayout == null) return;
            Color back = theme == "清爽白" ? Color.FromArgb(235, 241, 247) : Color.FromArgb(12, 22, 38);
            Color primaryText = theme == "清爽白" ? Color.FromArgb(29, 43, 62) : Color.White;
            Color secondaryText = theme == "清爽白" ? Color.FromArgb(78, 91, 112) : Color.FromArgb(202, 216, 235);
            BackColor = back;
            mainLayout.BackColor = back;
            titleLabel.ForeColor = primaryText;
            timeLabel.ForeColor = secondaryText;
            summaryLabel.ForeColor = secondaryText;
            prepareLabel.ForeColor = primaryText;
            speedLabel.ForeColor = secondaryText;
        }

        private void ApplyTypography()
        {
            titleLabel.Font = new Font("Microsoft YaHei UI", titleFontSize, FontStyle.Bold);
        }

        private void KeepWindowOnTop()
        {
            try
            {
                TopMost = true;
                SetForegroundWindow(Handle);
                SetWindowPos(Handle, -1, 0, 0, 0, 0, 1 | 2);
                SetActiveWindow(Handle);
            }
            catch
            {
            }
        }

        private void LoadSettings()
        {
            try
            {
                JObject jObject = JObject.Parse(File.ReadAllText("conf.json"));
                OperatorCenterURL = jObject["OperatorCenterURL"]?.ToString() ?? "http://127.0.0.1:888";
                OperatorID = jObject["OperatorID"]?.ToObject<int>() ?? 62;
                title = jObject["title"]?.ToString() ?? "学生发展中心 叫号大屏";
                theme = jObject["theme"]?.ToString() ?? "深海蓝";
                voiceEnabled = jObject["voiceEnabled"]?.ToObject<bool>() ?? true;
                voiceRepeatCount = jObject["voiceRepeatCount"]?.ToObject<int>() ?? 3;
                cardFontSize = jObject["cardFontSize"]?.ToObject<int>() ?? 72;
                titleFontSize = jObject["titleFontSize"]?.ToObject<int>() ?? 34;
            }
            catch
            {
                string settingText = JsonConvert.SerializeObject(new SettingClass
                {
                    OperatorCenterURL = "http://127.0.0.1:888",
                    OperatorID = 62,
                    title = "学生发展中心 叫号大屏",
                    theme = "深海蓝",
                    voiceEnabled = true,
                    voiceRepeatCount = 3,
                    cardFontSize = 72,
                    titleFontSize = 34
                });
                File.WriteAllText("conf.json", settingText);
                MessageBox.Show("因无法解析调度配置，配置已重置！");
                OperatorCenterURL = "http://127.0.0.1:888";
                OperatorID = 62;
                title = "学生发展中心 叫号大屏";
                theme = "深海蓝";
                voiceEnabled = true;
                voiceRepeatCount = 3;
                cardFontSize = 72;
                titleFontSize = 34;
            }
        }

        private static string NormalizeTypeText(string typeText)
        {
            if (string.IsNullOrWhiteSpace(typeText) || typeText == "[]") return "全部业务";
            return typeText.Replace("[", "").Replace("]", "").Replace("SIGN", "签订合同").Replace("INFOCHANGE", "信息变更").Replace(",", " / ");
        }
    }

    public class SettingClass
    {
        public string OperatorCenterURL;
        public int OperatorID;
        public string title;
        public string theme;
        public bool voiceEnabled;
        public int voiceRepeatCount;
        public int cardFontSize;
        public int titleFontSize;
    }

    internal class WindowCard
    {
        public RoundedPanel Root { get; } = new RoundedPanel();
        public Label WindowLabel { get; } = new Label();
        public Label NumberLabel { get; } = new Label();
        public Label StatusLabel { get; } = new Label();
        public Label TypeLabel { get; } = new Label();

        public WindowCard()
        {
            Root.Radius = 18;
            Root.Padding = new Padding(28, 22, 28, 22);
            Root.Controls.Add(NumberLabel);
            Root.Controls.Add(TypeLabel);
            Root.Controls.Add(StatusLabel);
            Root.Controls.Add(WindowLabel);

            WindowLabel.Dock = DockStyle.Top;
            WindowLabel.Height = 100;
            WindowLabel.TextAlign = ContentAlignment.MiddleLeft;
            WindowLabel.Font = new Font("Microsoft YaHei UI", 45, FontStyle.Bold);

            StatusLabel.Dock = DockStyle.Bottom;
            StatusLabel.Height = 38;
            StatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            StatusLabel.Font = new Font("Microsoft YaHei UI", 17, FontStyle.Regular);

            TypeLabel.Dock = DockStyle.Bottom;
            TypeLabel.Height = 36;
            TypeLabel.TextAlign = ContentAlignment.MiddleLeft;
            TypeLabel.Font = new Font("Microsoft YaHei UI", 15, FontStyle.Regular);

            NumberLabel.Dock = DockStyle.Fill;
            NumberLabel.TextAlign = ContentAlignment.MiddleCenter;
            NumberLabel.Font = new Font("Microsoft YaHei UI", 72, FontStyle.Bold);
        }
    }

    internal class RoundedPanel : Panel
    {
        public int Radius { get; set; } = 12;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = Radius * 2;
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(Width - radius - 1, 0, radius, radius, 270, 90);
                path.AddArc(Width - radius - 1, Height - radius - 1, radius, radius, 0, 90);
                path.AddArc(0, Height - radius - 1, radius, radius, 90, 90);
                path.CloseAllFigures();
                Region = new Region(path);
            }
        }
    }
}
