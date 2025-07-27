namespace OrderWorkbench
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
            label1 = new System.Windows.Forms.Label();
            numLabel = new System.Windows.Forms.Label();
            nextBtn = new System.Windows.Forms.Button();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            暂停ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            继续ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            转窗ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            窗口置顶ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            置顶ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            不置顶ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            转窗ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            timer1 = new System.Windows.Forms.Timer(components);
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(12, 33);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(139, 38);
            label1.TabIndex = 0;
            label1.Text = "当前分配:";
            // 
            // numLabel
            // 
            numLabel.AutoSize = true;
            numLabel.Font = new System.Drawing.Font("Segoe UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            numLabel.Location = new System.Drawing.Point(150, 26);
            numLabel.Name = "numLabel";
            numLabel.Size = new System.Drawing.Size(88, 50);
            numLabel.TabIndex = 1;
            numLabel.Text = "null";
            // 
            // nextBtn
            // 
            nextBtn.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            nextBtn.Location = new System.Drawing.Point(-2, 83);
            nextBtn.Name = "nextBtn";
            nextBtn.Size = new System.Drawing.Size(608, 61);
            nextBtn.TabIndex = 2;
            nextBtn.Text = "下一号";
            nextBtn.UseVisualStyleBackColor = true;
            nextBtn.Click += nextBtn_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { 设置ToolStripMenuItem, 暂停ToolStripMenuItem, 继续ToolStripMenuItem, 转窗ToolStripMenuItem1, 窗口置顶ToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(608, 28);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // 设置ToolStripMenuItem
            // 
            设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            设置ToolStripMenuItem.Size = new System.Drawing.Size(157, 24);
            设置ToolStripMenuItem.Text = "返回准入界面/设置";
            设置ToolStripMenuItem.Click += 设置ToolStripMenuItem_Click;
            // 
            // 暂停ToolStripMenuItem
            // 
            暂停ToolStripMenuItem.Name = "暂停ToolStripMenuItem";
            暂停ToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            暂停ToolStripMenuItem.Text = "暂停";
            暂停ToolStripMenuItem.Click += 暂停ToolStripMenuItem_Click;
            // 
            // 继续ToolStripMenuItem
            // 
            继续ToolStripMenuItem.Name = "继续ToolStripMenuItem";
            继续ToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            继续ToolStripMenuItem.Text = "继续";
            继续ToolStripMenuItem.Click += 继续ToolStripMenuItem_Click;
            // 
            // 转窗ToolStripMenuItem1
            // 
            转窗ToolStripMenuItem1.Name = "转窗ToolStripMenuItem1";
            转窗ToolStripMenuItem1.Size = new System.Drawing.Size(55, 24);
            转窗ToolStripMenuItem1.Text = "转窗";
            转窗ToolStripMenuItem1.Click += 转窗ToolStripMenuItem1_Click;
            // 
            // 窗口置顶ToolStripMenuItem
            // 
            窗口置顶ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { 置顶ToolStripMenuItem, 不置顶ToolStripMenuItem });
            窗口置顶ToolStripMenuItem.Name = "窗口置顶ToolStripMenuItem";
            窗口置顶ToolStripMenuItem.Size = new System.Drawing.Size(87, 24);
            窗口置顶ToolStripMenuItem.Text = "窗口置顶";
            // 
            // 置顶ToolStripMenuItem
            // 
            置顶ToolStripMenuItem.Name = "置顶ToolStripMenuItem";
            置顶ToolStripMenuItem.Size = new System.Drawing.Size(140, 26);
            置顶ToolStripMenuItem.Text = "置顶";
            置顶ToolStripMenuItem.Click += 置顶ToolStripMenuItem_Click;
            // 
            // 不置顶ToolStripMenuItem
            // 
            不置顶ToolStripMenuItem.Name = "不置顶ToolStripMenuItem";
            不置顶ToolStripMenuItem.Size = new System.Drawing.Size(140, 26);
            不置顶ToolStripMenuItem.Text = "不置顶";
            不置顶ToolStripMenuItem.Click += 不置顶ToolStripMenuItem_Click;
            // 
            // 转窗ToolStripMenuItem
            // 
            转窗ToolStripMenuItem.Name = "转窗ToolStripMenuItem";
            转窗ToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            转窗ToolStripMenuItem.Text = "转窗";
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(608, 143);
            Controls.Add(menuStrip1);
            Controls.Add(nextBtn);
            Controls.Add(numLabel);
            Controls.Add(label1);
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "Form1";
            Text = "OrderWorkbench";
            TopMost = true;
            Load += Form1_Load;
            SizeChanged += Form1_SizeChanged;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label numLabel;
        private System.Windows.Forms.Button nextBtn;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 暂停ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 继续ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 转窗ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 转窗ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 窗口置顶ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 置顶ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 不置顶ToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
    }
}
