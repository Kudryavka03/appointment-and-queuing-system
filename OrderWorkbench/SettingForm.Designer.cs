namespace OrderWorkbench
{
    partial class SettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new System.Windows.Forms.Label();
            SetNumText = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            SetServerURL = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            checkBox1 = new System.Windows.Forms.CheckBox();
            checkBox2 = new System.Windows.Forms.CheckBox();
            button1 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(26, 22);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(57, 20);
            label1.TabIndex = 0;
            label1.Text = "机号：";
            // 
            // SetNumText
            // 
            SetNumText.Location = new System.Drawing.Point(79, 19);
            SetNumText.Name = "SetNumText";
            SetNumText.Size = new System.Drawing.Size(289, 27);
            SetNumText.TabIndex = 1;
            SetNumText.Text = "1";
            SetNumText.TextChanged += SetNumText_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(26, 64);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(57, 20);
            label2.TabIndex = 2;
            label2.Text = "后台：";
            // 
            // SetServerURL
            // 
            SetServerURL.Location = new System.Drawing.Point(79, 61);
            SetServerURL.Name = "SetServerURL";
            SetServerURL.Size = new System.Drawing.Size(289, 27);
            SetServerURL.TabIndex = 3;
            SetServerURL.Text = "http://127.0.0.1:888";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(26, 104);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(57, 20);
            label3.TabIndex = 4;
            label3.Text = "业务：";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new System.Drawing.Point(88, 104);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(95, 24);
            checkBox1.TabIndex = 5;
            checkBox1.Text = "签订合同";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new System.Drawing.Point(201, 104);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new System.Drawing.Size(95, 24);
            checkBox2.TabIndex = 6;
            checkBox2.Text = "信息变更";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(26, 147);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(342, 49);
            button1.TabIndex = 7;
            button1.Text = "保存并进入系统";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // SettingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(400, 208);
            Controls.Add(button1);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(label3);
            Controls.Add(SetServerURL);
            Controls.Add(label2);
            Controls.Add(SetNumText);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "叫号系统准入 - 工作台端";
            Load += SettingForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SetNumText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SetServerURL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button button1;
    }
}