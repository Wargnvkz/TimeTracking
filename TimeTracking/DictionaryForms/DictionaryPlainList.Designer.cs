namespace TimeTracking.DictionaryForms
{
    partial class DictionaryPlainList
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
            this.lblWindowCaption = new System.Windows.Forms.Label();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.cmsDGVActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddLine = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteLine = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.cmsDGVActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblWindowCaption
            // 
            this.lblWindowCaption.AutoSize = true;
            this.lblWindowCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblWindowCaption.Location = new System.Drawing.Point(276, 9);
            this.lblWindowCaption.Name = "lblWindowCaption";
            this.lblWindowCaption.Size = new System.Drawing.Size(129, 20);
            this.lblWindowCaption.TabIndex = 0;
            this.lblWindowCaption.Text = "Заголовок окна";
            // 
            // dgvData
            // 
            this.dgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.ContextMenuStrip = this.cmsDGVActions;
            this.dgvData.Location = new System.Drawing.Point(3, 32);
            this.dgvData.Name = "dgvData";
            this.dgvData.Size = new System.Drawing.Size(794, 418);
            this.dgvData.TabIndex = 1; 
            this.dgvData.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvData_CellEndEdit);
            this.dgvData.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvData_DataError);
            // 
            // cmsDGVActions
            // 
            this.cmsDGVActions.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.cmsDGVActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddLine,
            this.tsmiDeleteLine});
            this.cmsDGVActions.Name = "cmsDGVActions";
            this.cmsDGVActions.Size = new System.Drawing.Size(223, 64);
            // 
            // tsmiAddLine
            // 
            this.tsmiAddLine.Name = "tsmiAddLine";
            this.tsmiAddLine.Size = new System.Drawing.Size(222, 30);
            this.tsmiAddLine.Text = "Добавить строку";
            this.tsmiAddLine.Click += new System.EventHandler(this.tsmiAddLine_Click);
            // 
            // tsmiDeleteLine
            // 
            this.tsmiDeleteLine.Name = "tsmiDeleteLine";
            this.tsmiDeleteLine.Size = new System.Drawing.Size(222, 30);
            this.tsmiDeleteLine.Text = "Удалить строку...";
            this.tsmiDeleteLine.Click += new System.EventHandler(this.tsmiDeleteLine_Click);
            // 
            // DictionaryPlainList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvData);
            this.Controls.Add(this.lblWindowCaption);
            this.Name = "DictionaryPlainList";
            this.Text = "DictionaryPlainList";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DictionaryPlainList_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.cmsDGVActions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWindowCaption;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.ContextMenuStrip cmsDGVActions;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddLine;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteLine;
    }
}