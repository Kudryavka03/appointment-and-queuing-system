namespace OrderScreen
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
            AppltBtn = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            SetServerURL = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            SetNumText = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // AppltBtn
            // 
            AppltBtn.Location = new System.Drawing.Point(357, 89);
            AppltBtn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            AppltBtn.Name = "AppltBtn";
            AppltBtn.Size = new System.Drawing.Size(105, 48);
            AppltBtn.TabIndex = 0;
            AppltBtn.Text = "应用设置";
            AppltBtn.UseVisualStyleBackColor = true;
            AppltBtn.Click += AppltBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 16);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(115, 20);
            label1.TabIndex = 1;
            label1.Text = "调度中心URL：";
            // 
            // SetServerURL
            // 
            SetServerURL.Location = new System.Drawing.Point(97, 13);
            SetServerURL.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            SetServerURL.Name = "SetServerURL";
            SetServerURL.Size = new System.Drawing.Size(365, 27);
            SetServerURL.TabIndex = 2;
            SetServerURL.Text = "http://10.30.8.100:88";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(60, 58);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(57, 20);
            label2.TabIndex = 3;
            label2.Text = "字号：";
            // 
            // SetNumText
            // 
            SetNumText.Location = new System.Drawing.Point(97, 55);
            SetNumText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            SetNumText.Name = "SetNumText";
            SetNumText.Size = new System.Drawing.Size(110, 27);
            SetNumText.TabIndex = 4;
            SetNumText.Text = "1";
            // 
            // SettingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(484, 148);
            Controls.Add(SetNumText);
            Controls.Add(label2);
            Controls.Add(SetServerURL);
            Controls.Add(label1);
            Controls.Add(AppltBtn);
            Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingForm";
            ShowIcon = false;
            Text = "SettingForm";
            Load += SettingForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button AppltBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SetServerURL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SetNumText;
    }
}