namespace TimeTracking.Reports
{
    partial class ShiftsReportDowntimeForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lvMachines = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvShifts = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dtpTo = new TimeTracking.NewDateTimePicker();
            this.dtpFrom = new TimeTracking.NewDateTimePicker();
            this.btnExcelEquipmentReport = new System.Windows.Forms.Button();
            this.btnExcelShiftReport = new System.Windows.Forms.Button();
            this.btnEmptyIdles = new System.Windows.Forms.Button();
            this.btnNotesIdle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(262, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Отчет по простоям";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Период:";
            // 
            // lvMachines
            // 
            this.lvMachines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvMachines.CheckBoxes = true;
            this.lvMachines.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvMachines.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lvMachines.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvMachines.HideSelection = false;
            this.lvMachines.Location = new System.Drawing.Point(16, 73);
            this.lvMachines.Name = "lvMachines";
            this.lvMachines.Size = new System.Drawing.Size(161, 385);
            this.lvMachines.TabIndex = 10;
            this.lvMachines.Tag = "";
            this.lvMachines.UseCompatibleStateImageBehavior = false;
            this.lvMachines.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Машины";
            this.columnHeader2.Width = 130;
            // 
            // lvShifts
            // 
            this.lvShifts.CheckBoxes = true;
            this.lvShifts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvShifts.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lvShifts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvShifts.HideSelection = false;
            this.lvShifts.Location = new System.Drawing.Point(193, 73);
            this.lvShifts.Name = "lvShifts";
            this.lvShifts.Size = new System.Drawing.Size(128, 154);
            this.lvShifts.TabIndex = 9;
            this.lvShifts.Tag = "";
            this.lvShifts.UseCompatibleStateImageBehavior = false;
            this.lvShifts.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Смены";
            this.columnHeader1.Width = 100;
            // 
            // dtpTo
            // 
            this.dtpTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpTo.Location = new System.Drawing.Point(296, 41);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(200, 26);
            this.dtpTo.TabIndex = 1;
            // 
            // dtpFrom
            // 
            this.dtpFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpFrom.Location = new System.Drawing.Point(90, 41);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(200, 26);
            this.dtpFrom.TabIndex = 0;
            // 
            // btnExcelEquipmentReport
            // 
            this.btnExcelEquipmentReport.Location = new System.Drawing.Point(364, 105);
            this.btnExcelEquipmentReport.Name = "btnExcelEquipmentReport";
            this.btnExcelEquipmentReport.Size = new System.Drawing.Size(97, 87);
            this.btnExcelEquipmentReport.TabIndex = 11;
            this.btnExcelEquipmentReport.Text = "Отчет по машинам";
            this.btnExcelEquipmentReport.UseVisualStyleBackColor = true;
            this.btnExcelEquipmentReport.Click += new System.EventHandler(this.btnExcelEquipmentReport_Click);
            // 
            // btnExcelShiftReport
            // 
            this.btnExcelShiftReport.Location = new System.Drawing.Point(467, 105);
            this.btnExcelShiftReport.Name = "btnExcelShiftReport";
            this.btnExcelShiftReport.Size = new System.Drawing.Size(97, 87);
            this.btnExcelShiftReport.TabIndex = 12;
            this.btnExcelShiftReport.Text = "Отчет по сменам";
            this.btnExcelShiftReport.UseVisualStyleBackColor = true;
            this.btnExcelShiftReport.Click += new System.EventHandler(this.btnExcelShiftReport_Click);
            // 
            // btnEmptyIdles
            // 
            this.btnEmptyIdles.Location = new System.Drawing.Point(570, 105);
            this.btnEmptyIdles.Name = "btnEmptyIdles";
            this.btnEmptyIdles.Size = new System.Drawing.Size(97, 87);
            this.btnEmptyIdles.TabIndex = 13;
            this.btnEmptyIdles.Text = "Отчет по незаполненным простоям";
            this.btnEmptyIdles.UseVisualStyleBackColor = true;
            this.btnEmptyIdles.Click += new System.EventHandler(this.btnEmptyIdles_Click);
            // 
            // btnNotesIdle
            // 
            this.btnNotesIdle.Location = new System.Drawing.Point(673, 105);
            this.btnNotesIdle.Name = "btnNotesIdle";
            this.btnNotesIdle.Size = new System.Drawing.Size(97, 87);
            this.btnNotesIdle.TabIndex = 14;
            this.btnNotesIdle.Text = "Отчет по замечаниям";
            this.btnNotesIdle.UseVisualStyleBackColor = true;
            this.btnNotesIdle.Click += new System.EventHandler(this.btnNotesIdle_Click);
            // 
            // ShiftsReportDowntimeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(994, 517);
            this.Controls.Add(this.btnNotesIdle);
            this.Controls.Add(this.btnEmptyIdles);
            this.Controls.Add(this.btnExcelShiftReport);
            this.Controls.Add(this.btnExcelEquipmentReport);
            this.Controls.Add(this.lvMachines);
            this.Controls.Add(this.lvShifts);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpTo);
            this.Controls.Add(this.dtpFrom);
            this.Name = "ShiftsReportDowntimeForm";
            this.Text = "Отчет по простоям";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NewDateTimePicker dtpFrom;
        private NewDateTimePicker dtpTo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView lvMachines;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView lvShifts;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnExcelEquipmentReport;
        private System.Windows.Forms.Button btnExcelShiftReport;
        private System.Windows.Forms.Button btnEmptyIdles;
        private System.Windows.Forms.Button btnNotesIdle;
    }
}