using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OrderWorkbench
{
    public partial class ChangeWindow : Form
    {
        private HttpClient httpClient = new HttpClient();
        string OperatorCenterURL;
        string OperatorID;
        public ChangeWindow(string OperatorCenterURL,string operatorid)
        {
            InitializeComponent();
            this.OperatorCenterURL = OperatorCenterURL;
            this.OperatorID = operatorid;
        }

        private async void ChangeWindow_Load(object sender, EventArgs e)
        {
            try
            {
                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GetAllWindows");
                var a = result.Split(',');
                comboBox1.Items.Clear();
                foreach (var item in a)
                {
                    if (item != OperatorID) comboBox1.Items.Add(item);
                }
                comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("通讯失败。请检查后台及网络");
                Close();
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/GenNewHighLevelUuid/" + $"{OperatorID}," + comboBox1.Text + ",转窗");
                MessageBox.Show("新的号码为："+result+" 请前往该窗口进行办理。");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("通讯失败。请检查后台及网络");
            }
        }
    }
}
