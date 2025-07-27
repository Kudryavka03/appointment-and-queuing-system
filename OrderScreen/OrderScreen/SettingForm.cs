using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace OrderScreen
{
    public partial class SettingForm : Form
    {
        public static string OperatorCenterURL = "";
        public static int OperatorID = 0;
        public SettingForm()
        {
            InitializeComponent();
        }

        private void AppltBtn_Click(object sender, EventArgs e)
        {
            SettingClass settingClass = new SettingClass();
            settingClass.OperatorCenterURL = SetServerURL.Text;
            settingClass.OperatorID = Convert.ToInt32(SetNumText.Text);
            var SettingText = JsonConvert.SerializeObject(settingClass);
            OrderClientOperatorForm.OperatorCenterURL = SetServerURL.Text;
            OrderClientOperatorForm.OperatorID = Convert.ToInt32(SetNumText.Text);
            File.WriteAllText("conf.json", SettingText);
            this.Close();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            SetServerURL.Text = OrderClientOperatorForm.OperatorCenterURL;
            SetNumText.Text = OrderClientOperatorForm.OperatorID.ToString();
        }
    }
}
