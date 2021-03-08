namespace FingerprintSync
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnSync = new System.Windows.Forms.Button();
            this.logBox = new System.Windows.Forms.TextBox();
            this.fromTo = new System.Windows.Forms.Label();
            this.btnAutoDeleteLeave = new System.Windows.Forms.Button();
            this.cblAllUserList = new System.Windows.Forms.CheckedListBox();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnReadAll = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnManulDeleteLeave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(291, 230);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(125, 23);
            this.btnSync.TabIndex = 0;
            this.btnSync.Text = "开始同步";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(299, 12);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logBox.Size = new System.Drawing.Size(346, 212);
            this.logBox.TabIndex = 1;
            // 
            // fromTo
            // 
            this.fromTo.AutoSize = true;
            this.fromTo.Location = new System.Drawing.Point(12, 258);
            this.fromTo.Name = "fromTo";
            this.fromTo.Size = new System.Drawing.Size(0, 12);
            this.fromTo.TabIndex = 2;
            // 
            // btnAutoDeleteLeave
            // 
            this.btnAutoDeleteLeave.Location = new System.Drawing.Point(545, 231);
            this.btnAutoDeleteLeave.Name = "btnAutoDeleteLeave";
            this.btnAutoDeleteLeave.Size = new System.Drawing.Size(100, 23);
            this.btnAutoDeleteLeave.TabIndex = 3;
            this.btnAutoDeleteLeave.Text = "自动删除离职人员";
            this.btnAutoDeleteLeave.UseVisualStyleBackColor = true;
            this.btnAutoDeleteLeave.Click += new System.EventHandler(this.btnDeleteLeave_Click);
            // 
            // cblAllUserList
            // 
            this.cblAllUserList.CheckOnClick = true;
            this.cblAllUserList.FormattingEnabled = true;
            this.cblAllUserList.Location = new System.Drawing.Point(14, 12);
            this.cblAllUserList.Name = "cblAllUserList";
            this.cblAllUserList.Size = new System.Drawing.Size(270, 212);
            this.cblAllUserList.TabIndex = 4;
            this.cblAllUserList.SelectedIndexChanged += new System.EventHandler(this.allUserList_SelectedIndexChanged);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Location = new System.Drawing.Point(123, 230);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(75, 23);
            this.btnCheckAll.TabIndex = 5;
            this.btnCheckAll.Text = "全选";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnReadAll
            // 
            this.btnReadAll.Location = new System.Drawing.Point(14, 230);
            this.btnReadAll.Name = "btnReadAll";
            this.btnReadAll.Size = new System.Drawing.Size(99, 23);
            this.btnReadAll.TabIndex = 6;
            this.btnReadAll.Text = "读取员工列表";
            this.btnReadAll.UseVisualStyleBackColor = true;
            this.btnReadAll.Click += new System.EventHandler(this.btnReadAll_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(208, 230);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "取消选择";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnManulDeleteLeave
            // 
            this.btnManulDeleteLeave.Location = new System.Drawing.Point(426, 231);
            this.btnManulDeleteLeave.Name = "btnManulDeleteLeave";
            this.btnManulDeleteLeave.Size = new System.Drawing.Size(108, 23);
            this.btnManulDeleteLeave.TabIndex = 8;
            this.btnManulDeleteLeave.Text = "手动删除离职人员";
            this.btnManulDeleteLeave.UseVisualStyleBackColor = true;
            this.btnManulDeleteLeave.Click += new System.EventHandler(this.btnManulDeleteLeave_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 279);
            this.Controls.Add(this.btnManulDeleteLeave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnReadAll);
            this.Controls.Add(this.btnCheckAll);
            this.Controls.Add(this.cblAllUserList);
            this.Controls.Add(this.btnAutoDeleteLeave);
            this.Controls.Add(this.fromTo);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.btnSync);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Work指纹同步工具";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Label fromTo;
        private System.Windows.Forms.Button btnAutoDeleteLeave;
        private System.Windows.Forms.CheckedListBox cblAllUserList;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.Button btnReadAll;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnManulDeleteLeave;
    }
}

