namespace TimeTracking.DictionaryForms
{
    partial class DictionaryShiftStaff
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvOperators = new System.Windows.Forms.DataGridView();
            this.Shift = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FIO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmsOperators = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddLine = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteLine = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOperators)).BeginInit();
            this.cmsOperators.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(326, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Оперативный персонал смен - операторы";
            // 
            // dgvOperators
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvOperators.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvOperators.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOperators.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Shift,
            this.FIO});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvOperators.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvOperators.Location = new System.Drawing.Point(16, 32);
            this.dgvOperators.Name = "dgvOperators";
            this.dgvOperators.Size = new System.Drawing.Size(447, 224);
            this.dgvOperators.TabIndex = 1;
            this.dgvOperators.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOperators_CellEndEdit);
            this.dgvOperators.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // Shift
            // 
            this.Shift.DataPropertyName = "ShiftNumber";
            this.Shift.HeaderText = "Смена";
            this.Shift.Name = "Shift";
            this.Shift.Width = 200;
            // 
            // FIO
            // 
            this.FIO.DataPropertyName = "OperatorName";
            this.FIO.HeaderText = "ФИО";
            this.FIO.Name = "FIO";
            this.FIO.Width = 200;
            // 
            // cmsOperators
            // 
            this.cmsOperators.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddLine,
            this.tsmiDeleteLine});
            this.cmsOperators.Name = "cmsEmployee";
            this.cmsOperators.Size = new System.Drawing.Size(193, 70);
            // 
            // tsmiAddLine
            // 
            this.tsmiAddLine.Name = "tsmiAddLine";
            this.tsmiAddLine.Size = new System.Drawing.Size(192, 22);
            this.tsmiAddLine.Text = "Добавить сотрудника";
            this.tsmiAddLine.Click += new System.EventHandler(this.tsmiAddLine_Click);
            // 
            // tsmiDeleteLine
            // 
            this.tsmiDeleteLine.Name = "tsmiDeleteLine";
            this.tsmiDeleteLine.Size = new System.Drawing.Size(192, 22);
            this.tsmiDeleteLine.Text = "Удалить сотрудника";
            this.tsmiDeleteLine.Click += new System.EventHandler(this.tsmiDeleteLine_Click);
            // 
            // DictionaryShiftStaff
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvOperators);
            this.Controls.Add(this.label1);
            this.Name = "DictionaryShiftStaff";
            this.Text = "SettingsShiftStaff";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DictionaryShiftStaff_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOperators)).EndInit();
            this.cmsOperators.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvOperators;
        private System.Windows.Forms.DataGridViewTextBoxColumn Shift;
        private System.Windows.Forms.DataGridViewTextBoxColumn FIO;
        private System.Windows.Forms.ContextMenuStrip cmsOperators;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddLine;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteLine;
    }
}