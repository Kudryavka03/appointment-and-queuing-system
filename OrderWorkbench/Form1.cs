using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OrderWorkbench
{
    public partial class Form1 : Form
    {

        private HttpClient httpClient = new HttpClient();
        public string OperatorCenterURL = "";

        public int OperatorID = 0;
        public string typeStr = "00";
        public int height = 0;
        public Form1(string OperatorCenterURL, int id, string typeStr)
        {
            InitializeComponent();
            this.OperatorCenterURL = OperatorCenterURL;
            this.OperatorID = id;
            this.typeStr = typeStr;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Screen screen = Screen.FromPoint(new Point(Cursor.Position.X, Cursor.Position.Y));
            int x = screen.WorkingArea.X + screen.WorkingArea.Width - base.Width;
            int y = screen.WorkingArea.Y + screen.WorkingArea.Height - base.Height;
            nextBtn.Width = base.Width - 1;
            base.Location = new Point(x, y);
            timer1.Enabled = true;
            Text = "工作台 - 未连接到调度中心 " + OperatorID + "号机 通讯失败";
            height = this.Height;
        }

        private void 置顶ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

        private void 不置顶ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string result = await httpClient.GetStringAsync(OperatorCenterURL + "/GetStatus/" + OperatorID);
                numLabel.Text = result;
                Text = "工作台 - 已连接到调度中心 " + OperatorID + "号机";
                nextBtn.Enabled = true;
            }
            catch
            {
                numLabel.Text = "失去连接！";
                Text = "工作台 - 未连接到调度中心 " + OperatorID + "号机 通讯失败";
            }
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            nextBtn.Enabled = false;
        }

        private async void 暂停ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await httpClient.GetStringAsync(OperatorCenterURL + "/CallAction/" + OperatorID + "/STOP");
        }

        private async void 继续ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await httpClient.GetStringAsync(OperatorCenterURL + "/CallAction/" + OperatorID + "/START");
        }

        private void 转窗ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var f = new ChangeWindow(OperatorCenterURL, OperatorID.ToString());
            f.ShowDialog();
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            nextBtn.Width = base.Width - 1;
            this.Height = height;
        }
    }

    public class SettingClass
    {
        public string OperatorCenterURL;

        public int OperatorID;

        public string TypeName;
    }
}
