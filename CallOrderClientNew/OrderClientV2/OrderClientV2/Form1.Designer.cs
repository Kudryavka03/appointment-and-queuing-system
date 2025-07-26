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
            TodayInfoBox = new System.Windows.Forms.GroupBox();
            button3 = new System.Windows.Forms.Button();
            textBox1 = new System.Windows.Forms.TextBox();
            RefreshTodayInfo = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            WindowStateInfoLabel = new System.Windows.Forms.Label();
            button2 = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            WindowStopBtn = new System.Windows.Forms.Button();
            TypeListBox = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            SetWindowTypeBtn = new System.Windows.Forms.Button();
            RefreshWindowTypeInfoBtn = new System.Windows.Forms.Button();
            WindowTypeInfoLabel = new System.Windows.Forms.Label();
            comboBox1 = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            groupBox2 = new System.Windows.Forms.GroupBox();
            label4 = new System.Windows.Forms.Label();
            MoreSetBtn = new System.Windows.Forms.Button();
            TodayInfoBox.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // TodayInfoBox
            // 
            TodayInfoBox.Controls.Add(button3);
            TodayInfoBox.Controls.Add(textBox1);
            TodayInfoBox.Controls.Add(RefreshTodayInfo);
            TodayInfoBox.Location = new System.Drawing.Point(12, 239);
            TodayInfoBox.Name = "TodayInfoBox";
            TodayInfoBox.Size = new System.Drawing.Size(728, 152);
            TodayInfoBox.TabIndex = 0;
            TodayInfoBox.TabStop = false;
            TodayInfoBox.Text = "当日信息统计";
            TodayInfoBox.Enter += groupBox1_Enter;
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(616, 32);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(94, 30);
            button3.TabIndex = 3;
            button3.Text = "RAW";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(17, 32);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(583, 104);
            textBox1.TabIndex = 2;
            // 
            // RefreshTodayInfo
            // 
            RefreshTodayInfo.Location = new System.Drawing.Point(616, 68);
            RefreshTodayInfo.Name = "RefreshTodayInfo";
            RefreshTodayInfo.Size = new System.Drawing.Size(94, 68);
            RefreshTodayInfo.TabIndex = 1;
            RefreshTodayInfo.Text = "获取统计";
            RefreshTodayInfo.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(WindowStateInfoLabel);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(WindowStopBtn);
            groupBox1.Controls.Add(TypeListBox);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(SetWindowTypeBtn);
            groupBox1.Controls.Add(RefreshWindowTypeInfoBtn);
            groupBox1.Controls.Add(WindowTypeInfoLabel);
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new System.Drawing.Point(12, 401);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(728, 217);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "窗口设置";
            // 
            // WindowStateInfoLabel
            // 
            WindowStateInfoLabel.AutoSize = true;
            WindowStateInfoLabel.Location = new System.Drawing.Point(256, 40);
            WindowStateInfoLabel.Name = "WindowStateInfoLabel";
            WindowStateInfoLabel.Size = new System.Drawing.Size(153, 20);
            WindowStateInfoLabel.TabIndex = 10;
            WindowStateInfoLabel.Text = "当前窗口状态：暂停";
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(323, 158);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(126, 41);
            button2.TabIndex = 9;
            button2.Text = "下一号";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(172, 158);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(126, 41);
            button1.TabIndex = 8;
            button1.Text = "窗口继续";
            button1.UseVisualStyleBackColor = true;
            // 
            // WindowStopBtn
            // 
            WindowStopBtn.Location = new System.Drawing.Point(22, 158);
            WindowStopBtn.Name = "WindowStopBtn";
            WindowStopBtn.Size = new System.Drawing.Size(126, 41);
            WindowStopBtn.TabIndex = 7;
            WindowStopBtn.Text = "窗口暂停";
            WindowStopBtn.UseVisualStyleBackColor = true;
            // 
            // TypeListBox
            // 
            TypeListBox.FormattingEnabled = true;
            TypeListBox.Location = new System.Drawing.Point(172, 95);
            TypeListBox.Name = "TypeListBox";
            TypeListBox.Size = new System.Drawing.Size(181, 28);
            TypeListBox.TabIndex = 6;
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
            // SetWindowTypeBtn
            // 
            SetWindowTypeBtn.Location = new System.Drawing.Point(616, 86);
            SetWindowTypeBtn.Name = "SetWindowTypeBtn";
            SetWindowTypeBtn.Size = new System.Drawing.Size(94, 44);
            SetWindowTypeBtn.TabIndex = 4;
            SetWindowTypeBtn.Text = "设置业务";
            SetWindowTypeBtn.UseVisualStyleBackColor = true;
            // 
            // RefreshWindowTypeInfoBtn
            // 
            RefreshWindowTypeInfoBtn.Location = new System.Drawing.Point(616, 36);
            RefreshWindowTypeInfoBtn.Name = "RefreshWindowTypeInfoBtn";
            RefreshWindowTypeInfoBtn.Size = new System.Drawing.Size(94, 29);
            RefreshWindowTypeInfoBtn.TabIndex = 3;
            RefreshWindowTypeInfoBtn.Text = "刷新信息";
            RefreshWindowTypeInfoBtn.UseVisualStyleBackColor = true;
            // 
            // WindowTypeInfoLabel
            // 
            WindowTypeInfoLabel.AutoSize = true;
            WindowTypeInfoLabel.Location = new System.Drawing.Point(359, 98);
            WindowTypeInfoLabel.Name = "WindowTypeInfoLabel";
            WindowTypeInfoLabel.Size = new System.Drawing.Size(148, 20);
            WindowTypeInfoLabel.TabIndex = 2;
            WindowTypeInfoLabel.Text = "当前业务类型：N/A";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new System.Drawing.Point(69, 37);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(181, 28);
            comboBox1.TabIndex = 1;
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
            // groupBox2
            // 
            groupBox2.Controls.Add(label4);
            groupBox2.Location = new System.Drawing.Point(12, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(728, 152);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "叫号台";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(22, 40);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(50, 20);
            label4.TabIndex = 0;
            label4.Text = "label1";
            // 
            // MoreSetBtn
            // 
            MoreSetBtn.Location = new System.Drawing.Point(628, 183);
            MoreSetBtn.Name = "MoreSetBtn";
            MoreSetBtn.Size = new System.Drawing.Size(94, 50);
            MoreSetBtn.TabIndex = 2;
            MoreSetBtn.Text = "更多设置";
            MoreSetBtn.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(752, 633);
            Controls.Add(MoreSetBtn);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(TodayInfoBox);
            Name = "Form1";
            Text = "叫号台及后端管理系统";
            TodayInfoBox.ResumeLayout(false);
            TodayInfoBox.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button MoreSetBtn;
        private System.Windows.Forms.Button RefreshTodayInfo;
        private System.Windows.Forms.Label WindowStateInfoLabel;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button WindowStopBtn;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
    }
}
