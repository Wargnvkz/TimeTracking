namespace TimeTracking.ShiftsForms
{
    partial class MaintainceServiceStaffTimeTracking
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
            this.dtpShiftDate = new TimeTracking.NewDateTimePicker();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblShift = new System.Windows.Forms.Label();
            this.cbSupervisor = new System.Windows.Forms.ComboBox();
            this.lblWorkingTime = new System.Windows.Forms.Label();
            this.dgvMainEmployee = new System.Windows.Forms.DataGridView();
            this.cmsMainEmployee = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddMainEmployee = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteMainEmployee = new System.Windows.Forms.ToolStripMenuItem();
            this.lblShiftEmployee = new System.Windows.Forms.Label();
            this.lblShiftEmployeeAdd = new System.Windows.Forms.Label();
            this.dgvAuxilaryWorkers = new System.Windows.Forms.DataGridView();
            this.cmsAuxilaryEmployee = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddAuxilaryEmployee = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteAuxilaryEmployee = new System.Windows.Forms.ToolStripMenuItem();
            this.tmShiftReload = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMainEmployee)).BeginInit();
            this.cmsMainEmployee.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuxilaryWorkers)).BeginInit();
            this.cmsAuxilaryEmployee.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtpShiftDate
            // 
            this.dtpShiftDate.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpShiftDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpShiftDate.Location = new System.Drawing.Point(70, 4);
            this.dtpShiftDate.Name = "dtpShiftDate";
            this.dtpShiftDate.Size = new System.Drawing.Size(200, 26);
            this.dtpShiftDate.TabIndex = 0;
            this.dtpShiftDate.ValueChanged += new System.EventHandler(this.dtpShiftDate_ValueChanged);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(12, 9);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(52, 20);
            this.lblDate.TabIndex = 1;
            this.lblDate.Text = "Дата:";
            // 
            // lblShift
            // 
            this.lblShift.AutoSize = true;
            this.lblShift.Location = new System.Drawing.Point(391, 9);
            this.lblShift.Name = "lblShift";
            this.lblShift.Size = new System.Drawing.Size(140, 20);
            this.lblShift.TabIndex = 2;
            this.lblShift.Text = "Инженер (смена):";
            // 
            // cbSupervisor
            // 
            this.cbSupervisor.FormattingEnabled = true;
            this.cbSupervisor.Location = new System.Drawing.Point(537, 6);
            this.cbSupervisor.Name = "cbSupervisor";
            this.cbSupervisor.Size = new System.Drawing.Size(338, 28);
            this.cbSupervisor.TabIndex = 3;
            this.cbSupervisor.SelectedIndexChanged += new System.EventHandler(this.cbSupervisor_SelectedIndexChanged);
            // 
            // lblWorkingTime
            // 
            this.lblWorkingTime.AutoSize = true;
            this.lblWorkingTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblWorkingTime.Location = new System.Drawing.Point(347, 64);
            this.lblWorkingTime.Name = "lblWorkingTime";
            this.lblWorkingTime.Size = new System.Drawing.Size(138, 24);
            this.lblWorkingTime.TabIndex = 4;
            this.lblWorkingTime.Text = "Время работы";
            // 
            // dgvMainEmployee
            // 
            this.dgvMainEmployee.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMainEmployee.ContextMenuStrip = this.cmsMainEmployee;
            this.dgvMainEmployee.Location = new System.Drawing.Point(12, 145);
            this.dgvMainEmployee.Name = "dgvMainEmployee";
            this.dgvMainEmployee.Size = new System.Drawing.Size(500, 332);
            this.dgvMainEmployee.TabIndex = 5;
            this.dgvMainEmployee.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvShiftEmployee_CellEndEdit);
            this.dgvMainEmployee.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvShiftEmployee_DataError);
            // 
            // cmsMainEmployee
            // 
            this.cmsMainEmployee.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddMainEmployee,
            this.tsmiDeleteMainEmployee});
            this.cmsMainEmployee.Name = "cmsEmployee";
            this.cmsMainEmployee.Size = new System.Drawing.Size(193, 48);
            // 
            // tsmiAddMainEmployee
            // 
            this.tsmiAddMainEmployee.Name = "tsmiAddMainEmployee";
            this.tsmiAddMainEmployee.Size = new System.Drawing.Size(192, 22);
            this.tsmiAddMainEmployee.Text = "Добавить сотрудника";
            this.tsmiAddMainEmployee.Click += new System.EventHandler(this.tsmiAddMainEmployee_Click);
            // 
            // tsmiDeleteMainEmployee
            // 
            this.tsmiDeleteMainEmployee.Name = "tsmiDeleteMainEmployee";
            this.tsmiDeleteMainEmployee.Size = new System.Drawing.Size(192, 22);
            this.tsmiDeleteMainEmployee.Text = "Удалить сотрудника";
            this.tsmiDeleteMainEmployee.Click += new System.EventHandler(this.tsmiDeleteMainEmployee_Click);
            // 
            // lblShiftEmployee
            // 
            this.lblShiftEmployee.AutoSize = true;
            this.lblShiftEmployee.Location = new System.Drawing.Point(219, 122);
            this.lblShiftEmployee.Name = "lblShiftEmployee";
            this.lblShiftEmployee.Size = new System.Drawing.Size(74, 20);
            this.lblShiftEmployee.TabIndex = 6;
            this.lblShiftEmployee.Text = "Слесари";
            // 
            // lblShiftEmployeeAdd
            // 
            this.lblShiftEmployeeAdd.AutoSize = true;
            this.lblShiftEmployeeAdd.Location = new System.Drawing.Point(712, 122);
            this.lblShiftEmployeeAdd.Name = "lblShiftEmployeeAdd";
            this.lblShiftEmployeeAdd.Size = new System.Drawing.Size(163, 20);
            this.lblShiftEmployeeAdd.TabIndex = 8;
            this.lblShiftEmployeeAdd.Text = "Подсобные рабочие";
            // 
            // dgvAuxilaryWorkers
            // 
            this.dgvAuxilaryWorkers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAuxilaryWorkers.ContextMenuStrip = this.cmsAuxilaryEmployee;
            this.dgvAuxilaryWorkers.Location = new System.Drawing.Point(537, 145);
            this.dgvAuxilaryWorkers.Name = "dgvAuxilaryWorkers";
            this.dgvAuxilaryWorkers.Size = new System.Drawing.Size(500, 332);
            this.dgvAuxilaryWorkers.TabIndex = 7;
            this.dgvAuxilaryWorkers.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAuxilaryWorkers_CellEndEdit);
            this.dgvAuxilaryWorkers.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvAuxilaryWorkers_DataError);
            // 
            // cmsAuxilaryEmployee
            // 
            this.cmsAuxilaryEmployee.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddAuxilaryEmployee,
            this.tsmiDeleteAuxilaryEmployee});
            this.cmsAuxilaryEmployee.Name = "cmsEmployee";
            this.cmsAuxilaryEmployee.Size = new System.Drawing.Size(193, 70);
            // 
            // tsmiAddAuxilaryEmployee
            // 
            this.tsmiAddAuxilaryEmployee.Name = "tsmiAddAuxilaryEmployee";
            this.tsmiAddAuxilaryEmployee.Size = new System.Drawing.Size(192, 22);
            this.tsmiAddAuxilaryEmployee.Text = "Добавить сотрудника";
            this.tsmiAddAuxilaryEmployee.Click += new System.EventHandler(this.tsmiAddAuxilaryEmployee_Click);
            // 
            // tsmiDeleteAuxilaryEmployee
            // 
            this.tsmiDeleteAuxilaryEmployee.Name = "tsmiDeleteAuxilaryEmployee";
            this.tsmiDeleteAuxilaryEmployee.Size = new System.Drawing.Size(192, 22);
            this.tsmiDeleteAuxilaryEmployee.Text = "Удалить сотрудника";
            this.tsmiDeleteAuxilaryEmployee.Click += new System.EventHandler(this.tsmiDeleteAuxilaryEmployee_Click);
            // 
            // tmShiftReload
            // 
            this.tmShiftReload.Enabled = true;
            this.tmShiftReload.Interval = 10000;
            this.tmShiftReload.Tick += new System.EventHandler(this.tmShiftReload_Tick);
            // 
            // MaintainceServiceStaffTimeTracking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 493);
            this.Controls.Add(this.lblShiftEmployeeAdd);
            this.Controls.Add(this.dgvAuxilaryWorkers);
            this.Controls.Add(this.lblShiftEmployee);
            this.Controls.Add(this.dgvMainEmployee);
            this.Controls.Add(this.lblWorkingTime);
            this.Controls.Add(this.cbSupervisor);
            this.Controls.Add(this.lblShift);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.dtpShiftDate);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MaintainceServiceStaffTimeTracking";
            this.Text = "Учет времени работы сотрудников";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MaintainceServiceStaffTimeTracking_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMainEmployee)).EndInit();
            this.cmsMainEmployee.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuxilaryWorkers)).EndInit();
            this.cmsAuxilaryEmployee.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NewDateTimePicker dtpShiftDate;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblShift;
        private System.Windows.Forms.ComboBox cbSupervisor;
        private System.Windows.Forms.Label lblWorkingTime;
        private System.Windows.Forms.DataGridView dgvMainEmployee;
        private System.Windows.Forms.Label lblShiftEmployee;
        private System.Windows.Forms.Label lblShiftEmployeeAdd;
        private System.Windows.Forms.DataGridView dgvAuxilaryWorkers;
        private System.Windows.Forms.Timer tmShiftReload;
        private System.Windows.Forms.ContextMenuStrip cmsMainEmployee;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddMainEmployee;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteMainEmployee;
        private System.Windows.Forms.ContextMenuStrip cmsAuxilaryEmployee;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddAuxilaryEmployee;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteAuxilaryEmployee;
    }
}