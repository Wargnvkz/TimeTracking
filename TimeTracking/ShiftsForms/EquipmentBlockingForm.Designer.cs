namespace TimeTracking.ShiftsForms
{
    partial class EquipmentBlockingForm
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
            this.components = new System.ComponentModel.Container();
            this.dgvBlockingReasons = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.добавитьМшинуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCaption = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblRefreshAfterCaption = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBlockingReasons)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvBlockingReasons
            // 
            this.dgvBlockingReasons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBlockingReasons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBlockingReasons.ContextMenuStrip = this.contextMenuStrip1;
            this.dgvBlockingReasons.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvBlockingReasons.Location = new System.Drawing.Point(3, 32);
            this.dgvBlockingReasons.Name = "dgvBlockingReasons";
            this.dgvBlockingReasons.Size = new System.Drawing.Size(1320, 455);
            this.dgvBlockingReasons.TabIndex = 0;
            this.dgvBlockingReasons.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvBlockingReason_CellBeginEdit);
            this.dgvBlockingReasons.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBlockingReason_CellEndEdit);
            this.dgvBlockingReasons.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvBlockingReason_DataError);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.добавитьМшинуToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(170, 26);
            // 
            // добавитьМшинуToolStripMenuItem
            // 
            this.добавитьМшинуToolStripMenuItem.Name = "добавитьМшинуToolStripMenuItem";
            this.добавитьМшинуToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.добавитьМшинуToolStripMenuItem.Text = "Добавить мшину";
            this.добавитьМшинуToolStripMenuItem.Click += new System.EventHandler(this.добавитьМшинуToolStripMenuItem_Click);
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCaption.Location = new System.Drawing.Point(332, 9);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(212, 20);
            this.lblCaption.TabIndex = 1;
            this.lblCaption.Text = "Блокировки оборудования";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblRefreshAfterCaption
            // 
            this.lblRefreshAfterCaption.AutoSize = true;
            this.lblRefreshAfterCaption.Location = new System.Drawing.Point(1107, 16);
            this.lblRefreshAfterCaption.Name = "lblRefreshAfterCaption";
            this.lblRefreshAfterCaption.Size = new System.Drawing.Size(10, 13);
            this.lblRefreshAfterCaption.TabIndex = 2;
            this.lblRefreshAfterCaption.Text = ".";
            // 
            // EquipmentBlockingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1325, 489);
            this.Controls.Add(this.lblRefreshAfterCaption);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.dgvBlockingReasons);
            this.Name = "EquipmentBlockingForm";
            this.Text = "Блокировки оборудования";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShiftsDowntimeForm_FormClosed);
            this.Load += new System.EventHandler(this.ShiftsDowntimeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBlockingReasons)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvBlockingReasons;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem добавитьМшинуToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblRefreshAfterCaption;
    }
}