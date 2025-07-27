namespace OrderClientV2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            TodayInfoBox = new System.Windows.Forms.GroupBox();
            button3 = new System.Windows.Forms.Button();
            LogBox1 = new System.Windows.Forms.TextBox();
            RefreshTodayInfo = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            RefreshWindowTypeInfoBtn = new System.Windows.Forms.Button();
            SetWindowTypeBtn = new System.Windows.Forms.Button();
            TypeLabel = new System.Windows.Forms.Label();
            WinNumLabel = new System.Windows.Forms.Label();
            checkBox4 = new System.Windows.Forms.CheckBox();
            checkBox5 = new System.Windows.Forms.CheckBox();
            WindowStateInfoLabel = new System.Windows.Forms.Label();
            button2 = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            WindowStopBtn = new System.Windows.Forms.Button();
            TypeListBox = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            WindowTypeInfoLabel = new System.Windows.Forms.Label();
            comboBox1 = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            checkBox2 = new System.Windows.Forms.CheckBox();
            checkBox1 = new System.Windows.Forms.CheckBox();
            button4 = new System.Windows.Forms.Button();
            numLabel = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            tabPage2 = new System.Windows.Forms.TabPage();
            ReCallResultLabel = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            button5 = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            textBox2 = new System.Windows.Forms.TextBox();
            tabPage4 = new System.Windows.Forms.TabPage();
            InsertResultLabel = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            button6 = new System.Windows.Forms.Button();
            comboBox2 = new System.Windows.Forms.ComboBox();
            label6 = new System.Windows.Forms.Label();
            tabPage5 = new System.Windows.Forms.TabPage();
            ChangeWindowLabelNum = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            button7 = new System.Windows.Forms.Button();
            label11 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            comboBox4 = new System.Windows.Forms.ComboBox();
            comboBox3 = new System.Windows.Forms.ComboBox();
            tabPage3 = new System.Windows.Forms.TabPage();
            label12 = new System.Windows.Forms.Label();
            tabPage6 = new System.Windows.Forms.TabPage();
            checkBox6 = new System.Windows.Forms.CheckBox();
            checkBox3 = new System.Windows.Forms.CheckBox();
            button8 = new System.Windows.Forms.Button();
            SetServerURL = new System.Windows.Forms.TextBox();
            label13 = new System.Windows.Forms.Label();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            timer1 = new System.Windows.Forms.Timer(components);
            timer2 = new System.Windows.Forms.Timer(components);
            button9 = new System.Windows.Forms.Button();
            TodayInfoBox.SuspendLayout();
            groupBox1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage4.SuspendLayout();
            tabPage5.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage6.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // TodayInfoBox
            // 
            TodayInfoBox.Controls.Add(button3);
            TodayInfoBox.Controls.Add(LogBox1);
            TodayInfoBox.Controls.Add(RefreshTodayInfo);
            TodayInfoBox.Location = new System.Drawing.Point(12, 239);
            TodayInfoBox.Name = "TodayInfoBox";
            TodayInfoBox.Size = new System.Drawing.Size(728, 197);
            TodayInfoBox.TabIndex = 0;
            TodayInfoBox.TabStop = false;
            TodayInfoBox.Text = "后台日志";
            TodayInfoBox.Enter += groupBox1_Enter;
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(616, 32);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(94, 30);
            button3.TabIndex = 3;
            button3.Text = "刷新Log";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // LogBox1
            // 
            LogBox1.Location = new System.Drawing.Point(17, 32);
            LogBox1.Multiline = true;
            LogBox1.Name = "LogBox1";
            LogBox1.ReadOnly = true;
            LogBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            LogBox1.Size = new System.Drawing.Size(583, 159);
            LogBox1.TabIndex = 2;
            // 
            // RefreshTodayInfo
            // 
            RefreshTodayInfo.Location = new System.Drawing.Point(616, 68);
            RefreshTodayInfo.Name = "RefreshTodayInfo";
            RefreshTodayInfo.Size = new System.Drawing.Size(94, 123);
            RefreshTodayInfo.TabIndex = 1;
            RefreshTodayInfo.Text = "获取统计";
            RefreshTodayInfo.UseVisualStyleBackColor = true;
            RefreshTodayInfo.Click += RefreshTodayInfo_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button9);
            groupBox1.Controls.Add(RefreshWindowTypeInfoBtn);
            groupBox1.Controls.Add(SetWindowTypeBtn);
            groupBox1.Controls.Add(TypeLabel);
            groupBox1.Controls.Add(WinNumLabel);
            groupBox1.Controls.Add(checkBox4);
            groupBox1.Controls.Add(checkBox5);
            groupBox1.Controls.Add(WindowStateInfoLabel);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(WindowStopBtn);
            groupBox1.Controls.Add(TypeListBox);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(WindowTypeInfoLabel);
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new System.Drawing.Point(12, 442);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(728, 217);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "窗口设置";
            // 
            // RefreshWindowTypeInfoBtn
            // 
            RefreshWindowTypeInfoBtn.Location = new System.Drawing.Point(616, 36);
            RefreshWindowTypeInfoBtn.Name = "RefreshWindowTypeInfoBtn";
            RefreshWindowTypeInfoBtn.Size = new System.Drawing.Size(94, 29);
            RefreshWindowTypeInfoBtn.TabIndex = 3;
            RefreshWindowTypeInfoBtn.Text = "刷新信息";
            RefreshWindowTypeInfoBtn.UseVisualStyleBackColor = true;
            RefreshWindowTypeInfoBtn.Click += RefreshWindowTypeInfoBtn_Click;
            // 
            // SetWindowTypeBtn
            // 
            SetWindowTypeBtn.Location = new System.Drawing.Point(616, 86);
            SetWindowTypeBtn.Name = "SetWindowTypeBtn";
            SetWindowTypeBtn.Size = new System.Drawing.Size(94, 44);
            SetWindowTypeBtn.TabIndex = 4;
            SetWindowTypeBtn.Text = "设置业务";
            SetWindowTypeBtn.UseVisualStyleBackColor = true;
            SetWindowTypeBtn.Click += SetWindowTypeBtn_Click;
            // 
            // TypeLabel
            // 
            TypeLabel.AutoSize = true;
            TypeLabel.Location = new System.Drawing.Point(475, 99);
            TypeLabel.Name = "TypeLabel";
            TypeLabel.Size = new System.Drawing.Size(33, 20);
            TypeLabel.TabIndex = 14;
            TypeLabel.Text = "null";
            // 
            // WinNumLabel
            // 
            WinNumLabel.AutoSize = true;
            WinNumLabel.Location = new System.Drawing.Point(374, 40);
            WinNumLabel.Name = "WinNumLabel";
            WinNumLabel.Size = new System.Drawing.Size(33, 20);
            WinNumLabel.TabIndex = 13;
            WinNumLabel.Text = "null";
            // 
            // checkBox4
            // 
            checkBox4.AutoSize = true;
            checkBox4.Location = new System.Drawing.Point(273, 99);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new System.Drawing.Size(95, 24);
            checkBox4.TabIndex = 12;
            checkBox4.Text = "信息变更";
            checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            checkBox5.AutoSize = true;
            checkBox5.Location = new System.Drawing.Point(172, 99);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new System.Drawing.Size(95, 24);
            checkBox5.TabIndex = 11;
            checkBox5.Text = "签订合同";
            checkBox5.UseVisualStyleBackColor = true;
            // 
            // WindowStateInfoLabel
            // 
            WindowStateInfoLabel.AutoSize = true;
            WindowStateInfoLabel.Location = new System.Drawing.Point(256, 40);
            WindowStateInfoLabel.Name = "WindowStateInfoLabel";
            WindowStateInfoLabel.Size = new System.Drawing.Size(121, 20);
            WindowStateInfoLabel.TabIndex = 10;
            WindowStateInfoLabel.Text = "当前窗口状态：";
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(323, 158);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(126, 41);
            button2.TabIndex = 9;
            button2.Text = "下一号";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(172, 158);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(126, 41);
            button1.TabIndex = 8;
            button1.Text = "窗口继续";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // WindowStopBtn
            // 
            WindowStopBtn.Location = new System.Drawing.Point(22, 158);
            WindowStopBtn.Name = "WindowStopBtn";
            WindowStopBtn.Size = new System.Drawing.Size(126, 41);
            WindowStopBtn.TabIndex = 7;
            WindowStopBtn.Text = "窗口暂停";
            WindowStopBtn.UseVisualStyleBackColor = true;
            WindowStopBtn.Click += WindowStopBtn_Click;
            // 
            // TypeListBox
            // 
            TypeListBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            TypeListBox.FormattingEnabled = true;
            TypeListBox.Location = new System.Drawing.Point(172, 95);
            TypeListBox.Name = "TypeListBox";
            TypeListBox.Size = new System.Drawing.Size(181, 28);
            TypeListBox.TabIndex = 6;
            TypeListBox.Visible = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(22, 98);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(153, 20);
            label3.TabIndex = 5;
            label3.Text = "设置新的业务类型：";
            // 
            // WindowTypeInfoLabel
            // 
            WindowTypeInfoLabel.AutoSize = true;
            WindowTypeInfoLabel.Location = new System.Drawing.Point(365, 99);
            WindowTypeInfoLabel.Name = "WindowTypeInfoLabel";
            WindowTypeInfoLabel.Size = new System.Drawing.Size(121, 20);
            WindowTypeInfoLabel.TabIndex = 2;
            WindowTypeInfoLabel.Text = "当前业务类型：";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new System.Drawing.Point(69, 37);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(181, 28);
            comboBox1.TabIndex = 1;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(22, 40);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(41, 20);
            label1.TabIndex = 0;
            label1.Text = "窗口";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage6);
            tabControl1.Location = new System.Drawing.Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(728, 221);
            tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(checkBox2);
            tabPage1.Controls.Add(checkBox1);
            tabPage1.Controls.Add(button4);
            tabPage1.Controls.Add(numLabel);
            tabPage1.Controls.Add(label2);
            tabPage1.Location = new System.Drawing.Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(720, 188);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "新取号";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new System.Drawing.Point(611, 72);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new System.Drawing.Size(95, 24);
            checkBox2.TabIndex = 4;
            checkBox2.Text = "信息变更";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new System.Drawing.Point(611, 42);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(95, 24);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "签订合同";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            button4.Location = new System.Drawing.Point(13, 109);
            button4.Name = "button4";
            button4.Size = new System.Drawing.Size(693, 58);
            button4.TabIndex = 2;
            button4.Text = "取号";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // numLabel
            // 
            numLabel.AutoSize = true;
            numLabel.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            numLabel.Location = new System.Drawing.Point(143, 19);
            numLabel.Name = "numLabel";
            numLabel.Size = new System.Drawing.Size(141, 81);
            numLabel.TabIndex = 1;
            numLabel.Text = "null";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(25, 42);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(146, 41);
            label2.TabIndex = 0;
            label2.Text = "新取号：";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(ReCallResultLabel);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(button5);
            tabPage2.Controls.Add(label4);
            tabPage2.Controls.Add(textBox2);
            tabPage2.Location = new System.Drawing.Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(720, 188);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "过号重取";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // ReCallResultLabel
            // 
            ReCallResultLabel.AutoSize = true;
            ReCallResultLabel.Font = new System.Drawing.Font("Segoe UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ReCallResultLabel.Location = new System.Drawing.Point(198, 109);
            ReCallResultLabel.Name = "ReCallResultLabel";
            ReCallResultLabel.Size = new System.Drawing.Size(182, 50);
            ReCallResultLabel.TabIndex = 4;
            ReCallResultLabel.Text = "请先报到";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label5.Location = new System.Drawing.Point(13, 113);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(210, 41);
            label5.TabIndex = 3;
            label5.Text = "重报到号码：";
            // 
            // button5
            // 
            button5.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            button5.Location = new System.Drawing.Point(524, 22);
            button5.Name = "button5";
            button5.Size = new System.Drawing.Size(182, 47);
            button5.TabIndex = 2;
            button5.Text = "重新报到";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label4.Location = new System.Drawing.Point(13, 24);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(178, 41);
            label4.TabIndex = 1;
            label4.Text = "过号号码：";
            // 
            // textBox2
            // 
            textBox2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            textBox2.Location = new System.Drawing.Point(197, 24);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(321, 47);
            textBox2.TabIndex = 0;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(InsertResultLabel);
            tabPage4.Controls.Add(label8);
            tabPage4.Controls.Add(button6);
            tabPage4.Controls.Add(comboBox2);
            tabPage4.Controls.Add(label6);
            tabPage4.Location = new System.Drawing.Point(4, 29);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new System.Drawing.Size(720, 188);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "插队";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // InsertResultLabel
            // 
            InsertResultLabel.AutoSize = true;
            InsertResultLabel.Font = new System.Drawing.Font("Segoe UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            InsertResultLabel.Location = new System.Drawing.Point(168, 110);
            InsertResultLabel.Name = "InsertResultLabel";
            InsertResultLabel.Size = new System.Drawing.Size(182, 50);
            InsertResultLabel.TabIndex = 6;
            InsertResultLabel.Text = "请先插队";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label8.Location = new System.Drawing.Point(18, 114);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(178, 41);
            label8.TabIndex = 5;
            label8.Text = "插队号码：";
            // 
            // button6
            // 
            button6.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            button6.Location = new System.Drawing.Point(612, 93);
            button6.Name = "button6";
            button6.Size = new System.Drawing.Size(94, 75);
            button6.TabIndex = 2;
            button6.Text = "插队";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // comboBox2
            // 
            comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new System.Drawing.Point(95, 28);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new System.Drawing.Size(611, 49);
            comboBox2.TabIndex = 1;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label6.Location = new System.Drawing.Point(13, 28);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(82, 41);
            label6.TabIndex = 0;
            label6.Text = "窗口";
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(ChangeWindowLabelNum);
            tabPage5.Controls.Add(label14);
            tabPage5.Controls.Add(button7);
            tabPage5.Controls.Add(label11);
            tabPage5.Controls.Add(label10);
            tabPage5.Controls.Add(label9);
            tabPage5.Controls.Add(comboBox4);
            tabPage5.Controls.Add(comboBox3);
            tabPage5.Location = new System.Drawing.Point(4, 29);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new System.Drawing.Size(720, 188);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "转窗";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // ChangeWindowLabelNum
            // 
            ChangeWindowLabelNum.AutoSize = true;
            ChangeWindowLabelNum.Font = new System.Drawing.Font("Segoe UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ChangeWindowLabelNum.Location = new System.Drawing.Point(176, 104);
            ChangeWindowLabelNum.Name = "ChangeWindowLabelNum";
            ChangeWindowLabelNum.Size = new System.Drawing.Size(182, 50);
            ChangeWindowLabelNum.TabIndex = 9;
            ChangeWindowLabelNum.Text = "请先转窗";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label14.Location = new System.Drawing.Point(18, 108);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(178, 41);
            label14.TabIndex = 8;
            label14.Text = "转窗号码：";
            // 
            // button7
            // 
            button7.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            button7.Location = new System.Drawing.Point(612, 95);
            button7.Name = "button7";
            button7.Size = new System.Drawing.Size(94, 66);
            button7.TabIndex = 7;
            button7.Text = "转窗";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label11.Location = new System.Drawing.Point(646, 25);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(50, 41);
            label11.TabIndex = 6;
            label11.Text = "窗";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label10.Location = new System.Drawing.Point(300, 25);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(114, 41);
            label10.TabIndex = 5;
            label10.Text = "窗转到";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label9.Location = new System.Drawing.Point(18, 25);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(50, 41);
            label9.TabIndex = 4;
            label9.Text = "从";
            // 
            // comboBox4
            // 
            comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox4.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            comboBox4.FormattingEnabled = true;
            comboBox4.Location = new System.Drawing.Point(420, 22);
            comboBox4.Name = "comboBox4";
            comboBox4.Size = new System.Drawing.Size(220, 49);
            comboBox4.TabIndex = 3;
            // 
            // comboBox3
            // 
            comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox3.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new System.Drawing.Point(74, 22);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new System.Drawing.Size(220, 49);
            comboBox3.TabIndex = 2;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(label12);
            tabPage3.Location = new System.Drawing.Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new System.Drawing.Size(720, 188);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "修正叫号";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(13, 11);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(57, 20);
            label12.TabIndex = 0;
            label12.Text = "未开发";
            // 
            // tabPage6
            // 
            tabPage6.Controls.Add(checkBox6);
            tabPage6.Controls.Add(checkBox3);
            tabPage6.Controls.Add(button8);
            tabPage6.Controls.Add(SetServerURL);
            tabPage6.Controls.Add(label13);
            tabPage6.Location = new System.Drawing.Point(4, 29);
            tabPage6.Name = "tabPage6";
            tabPage6.Size = new System.Drawing.Size(720, 188);
            tabPage6.TabIndex = 5;
            tabPage6.Text = "后台设置";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            checkBox6.AutoSize = true;
            checkBox6.Checked = true;
            checkBox6.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox6.Location = new System.Drawing.Point(389, 59);
            checkBox6.Name = "checkBox6";
            checkBox6.Size = new System.Drawing.Size(207, 24);
            checkBox6.TabIndex = 4;
            checkBox6.Text = "自动刷新日志与窗口信息";
            checkBox6.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Checked = true;
            checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox3.Location = new System.Drawing.Point(611, 59);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new System.Drawing.Size(95, 24);
            checkBox3.TabIndex = 3;
            checkBox3.Text = "窗口置顶";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.CheckedChanged += checkBox3_CheckedChanged;
            // 
            // button8
            // 
            button8.Location = new System.Drawing.Point(13, 95);
            button8.Name = "button8";
            button8.Size = new System.Drawing.Size(693, 50);
            button8.TabIndex = 2;
            button8.Text = "保存";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // SetServerURL
            // 
            SetServerURL.Location = new System.Drawing.Point(91, 15);
            SetServerURL.Name = "SetServerURL";
            SetServerURL.Size = new System.Drawing.Size(615, 27);
            SetServerURL.TabIndex = 1;
            SetServerURL.Text = "http://127.0.0.1:888";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(18, 19);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(67, 20);
            label13.TabIndex = 0;
            label13.Text = "后台URL";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new System.Drawing.Point(0, 667);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(752, 26);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(137, 20);
            toolStripStatusLabel1.Text = "未连接到调度中心";
            // 
            // timer1
            // 
            timer1.Interval = 750;
            timer1.Tick += timer1_Tick;
            // 
            // timer2
            // 
            timer2.Enabled = true;
            timer2.Interval = 5000;
            timer2.Tick += timer2_Tick;
            // 
            // button9
            // 
            button9.Location = new System.Drawing.Point(583, 155);
            button9.Name = "button9";
            button9.Size = new System.Drawing.Size(127, 44);
            button9.TabIndex = 15;
            button9.Text = "窗口信息一览";
            button9.UseVisualStyleBackColor = true;
            button9.Click += button9_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(752, 693);
            Controls.Add(statusStrip1);
            Controls.Add(tabControl1);
            Controls.Add(groupBox1);
            Controls.Add(TodayInfoBox);
            MaximizeBox = false;
            Name = "Form1";
            ShowIcon = false;
            Text = "叫号台及后台管理系统";
            TopMost = true;
            Load += Form1_Load;
            TodayInfoBox.ResumeLayout(false);
            TodayInfoBox.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            tabPage6.ResumeLayout(false);
            tabPage6.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox TodayInfoBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox TypeListBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button SetWindowTypeBtn;
        private System.Windows.Forms.Button RefreshWindowTypeInfoBtn;
        private System.Windows.Forms.Label WindowTypeInfoLabel;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button RefreshTodayInfo;
        private System.Windows.Forms.Label WindowStateInfoLabel;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button WindowStopBtn;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox LogBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label numLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label ReCallResultLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label InsertResultLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox SetServerURL;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.Label WinNumLabel;
        private System.Windows.Forms.Label TypeLabel;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.Label ChangeWindowLabelNum;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button9;
    }
}
