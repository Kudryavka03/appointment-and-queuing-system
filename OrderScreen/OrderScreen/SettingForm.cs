using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace OrderScreen
{
    public partial class SettingForm : Form
    {
        public static string OperatorCenterURL = "";
        public static int OperatorID = 0;
        private TextBox TitleTextBox;
        private ComboBox ThemeComboBox;
        private CheckBox VoiceCheckBox;
        private NumericUpDown VoiceRepeatBox;
        private NumericUpDown CardFontSizeBox;
        private NumericUpDown TitleFontSizeBox;

        public SettingForm()
        {
            InitializeComponent();
            BuildExtraSettings();
        }

        private void AppltBtn_Click(object sender, EventArgs e)
        {
            SettingClass settingClass = new SettingClass
            {
                OperatorCenterURL = SetServerURL.Text,
                OperatorID = Convert.ToInt32(SetNumText.Text),
                title = TitleTextBox.Text,
                theme = ThemeComboBox.Text,
                voiceEnabled = VoiceCheckBox.Checked,
                voiceRepeatCount = Convert.ToInt32(VoiceRepeatBox.Value),
                cardFontSize = Convert.ToInt32(CardFontSizeBox.Value),
                titleFontSize = Convert.ToInt32(TitleFontSizeBox.Value)
            };
            string settingText = JsonConvert.SerializeObject(settingClass, Formatting.Indented);
            OrderClientOperatorForm.OperatorCenterURL = settingClass.OperatorCenterURL;
            OrderClientOperatorForm.OperatorID = settingClass.OperatorID;
            OrderClientOperatorForm.title = settingClass.title;
            OrderClientOperatorForm.theme = settingClass.theme;
            OrderClientOperatorForm.voiceEnabled = settingClass.voiceEnabled;
            OrderClientOperatorForm.voiceRepeatCount = settingClass.voiceRepeatCount;
            OrderClientOperatorForm.cardFontSize = settingClass.cardFontSize;
            OrderClientOperatorForm.titleFontSize = settingClass.titleFontSize;
            File.WriteAllText("conf.json", settingText);
            Close();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            SetServerURL.Text = OrderClientOperatorForm.OperatorCenterURL;
            SetNumText.Text = OrderClientOperatorForm.OperatorID.ToString();
            TitleTextBox.Text = OrderClientOperatorForm.title;
            ThemeComboBox.Text = OrderClientOperatorForm.theme;
            VoiceCheckBox.Checked = OrderClientOperatorForm.voiceEnabled;
            VoiceRepeatBox.Value = Clamp(OrderClientOperatorForm.voiceRepeatCount, 1, 5);
            CardFontSizeBox.Value = Clamp(OrderClientOperatorForm.cardFontSize, 42, 120);
            TitleFontSizeBox.Value = Clamp(OrderClientOperatorForm.titleFontSize, 24, 54);
            TopMost = true;
        }

        private void BuildExtraSettings()
        {
            ClientSize = new Size(520, 390);
            AppltBtn.Location = new Point(357, 312);
            AppltBtn.Size = new Size(105, 48);

            AddLabel("标题：", 60, 98);
            TitleTextBox = new TextBox { Location = new Point(97, 95), Size = new Size(365, 27) };
            Controls.Add(TitleTextBox);

            AddLabel("主题：", 60, 138);
            ThemeComboBox = new ComboBox
            {
                Location = new Point(97, 135),
                Size = new Size(160, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            ThemeComboBox.Items.AddRange(new object[] { "深海蓝", "清爽白" });
            Controls.Add(ThemeComboBox);

            VoiceCheckBox = new CheckBox
            {
                Location = new Point(285, 137),
                Size = new Size(160, 24),
                Text = "启用语音播报"
            };
            Controls.Add(VoiceCheckBox);

            AddLabel("播报次数：", 28, 181);
            VoiceRepeatBox = new NumericUpDown
            {
                Location = new Point(97, 178),
                Size = new Size(80, 27),
                Minimum = 1,
                Maximum = 5,
                Value = 3
            };
            Controls.Add(VoiceRepeatBox);

            AddLabel("号码字号：", 28, 224);
            CardFontSizeBox = new NumericUpDown
            {
                Location = new Point(97, 221),
                Size = new Size(80, 27),
                Minimum = 42,
                Maximum = 120,
                Value = 72
            };
            Controls.Add(CardFontSizeBox);

            AddLabel("标题字号：", 28, 267);
            TitleFontSizeBox = new NumericUpDown
            {
                Location = new Point(97, 264),
                Size = new Size(80, 27),
                Minimum = 24,
                Maximum = 54,
                Value = 34
            };
            Controls.Add(TitleFontSizeBox);
        }

        private void AddLabel(string text, int x, int y)
        {
            Controls.Add(new Label
            {
                AutoSize = true,
                Location = new Point(x, y),
                Text = text
            });
        }

        private static decimal Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
