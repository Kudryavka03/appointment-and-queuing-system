namespace OrderScreen
{
    partial class OrderClientOperatorForm
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
            label1 = new System.Windows.Forms.Label();
            timer1 = new System.Windows.Forms.Timer(components);
            Window1Label = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = System.Drawing.Color.Black;
            label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(9, 15);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(434, 52);
            label1.TabIndex = 0;
            label1.Text = "学生发展中心 叫号大屏";
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // Window1Label
            // 
            Window1Label.AutoSize = true;
            Window1Label.Font = new System.Drawing.Font("Microsoft YaHei UI", 42F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            Window1Label.ForeColor = System.Drawing.Color.White;
            Window1Label.Location = new System.Drawing.Point(22, 90);
            Window1Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            Window1Label.Name = "Window1Label";
            Window1Label.Size = new System.Drawing.Size(350, 93);
            Window1Label.TabIndex = 2;
            Window1Label.Text = "No Data.";
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(993, 18);
            button1.Margin = new System.Windows.Forms.Padding(2);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(81, 28);
            button1.TabIndex = 7;
            button1.Text = "Settings";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // OrderClientOperatorForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(1097, 574);
            Controls.Add(button1);
            Controls.Add(Window1Label);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "OrderClientOperatorForm";
            Text = "OrderScreen";
            TopMost = true;
            FormClosed += OrderClientOperatorForm_FormClosed;
            Load += Form1_Load;
            Resize += OrderClientOperatorForm_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label Window1Label;
        private System.Windows.Forms.Button button1;
    }
}
