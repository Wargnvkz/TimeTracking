namespace TimeTracking
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmiMSStaff = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMSSTT = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSTTReport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLaborCostShift = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEquipmentBlocking = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBalanceTimeGraph = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMaintenanceIdleEquipment = new System.Windows.Forms.ToolStripMenuItem();
            this.сменыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShiftsDowntime = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShiftsSPMNotice = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShiftsReportDowntime = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDictionary = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDictionaryMaintananceServiceStaff = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDictionaryShiftStaff = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDictionaryIdleReasons = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiProgramSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEmployeePosition = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDictionaryUsers = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlMainForm = new System.Windows.Forms.Panel();
            this.tsmiTechnologistIdleEquipment = new System.Windows.Forms.ToolStripMenuItem();
            this.простоиОборудованияЗаСменуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.AliceBlue;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMSStaff,
            this.сменыToolStripMenuItem,
            this.tsmiTechnologistIdleEquipment,
            this.tsmDictionary,
            this.tsmiProgramSettings,
            this.tsmiOpenWindows,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1605, 33);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmiMSStaff
            // 
            this.tsmiMSStaff.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMSSTT,
            this.tsmiSTTReport,
            this.tsmiLaborCostShift,
            this.tsmiEquipmentBlocking,
            this.tsmiBalanceTimeGraph,
            this.tsmiMaintenanceIdleEquipment});
            this.tsmiMSStaff.Name = "tsmiMSStaff";
            this.tsmiMSStaff.Size = new System.Drawing.Size(213, 29);
            this.tsmiMSStaff.Tag = "1";
            this.tsmiMSStaff.Text = "Служба эксплуатации";
            // 
            // tsmiMSSTT
            // 
            this.tsmiMSSTT.Name = "tsmiMSSTT";
            this.tsmiMSSTT.Size = new System.Drawing.Size(637, 30);
            this.tsmiMSSTT.Text = "Учет рабочего времени сотрудников";
            this.tsmiMSSTT.Click += new System.EventHandler(this.tsmiMSSTT_Click);
            // 
            // tsmiSTTReport
            // 
            this.tsmiSTTReport.Name = "tsmiSTTReport";
            this.tsmiSTTReport.Size = new System.Drawing.Size(637, 30);
            this.tsmiSTTReport.Text = "Отчет использование рабочего времени слесарей-наладчиков";
            this.tsmiSTTReport.Click += new System.EventHandler(this.tsmiSTTReport_Click);
            // 
            // tsmiLaborCostShift
            // 
            this.tsmiLaborCostShift.Name = "tsmiLaborCostShift";
            this.tsmiLaborCostShift.Size = new System.Drawing.Size(637, 30);
            this.tsmiLaborCostShift.Text = "Трудозатраты будущих смен";
            this.tsmiLaborCostShift.Click += new System.EventHandler(this.tsmiLaborCostShift_Click);
            // 
            // tsmiEquipmentBlocking
            // 
            this.tsmiEquipmentBlocking.Name = "tsmiEquipmentBlocking";
            this.tsmiEquipmentBlocking.Size = new System.Drawing.Size(637, 30);
            this.tsmiEquipmentBlocking.Text = "Блокировка машин";
            this.tsmiEquipmentBlocking.Click += new System.EventHandler(this.tsmiEquipmentBlocking_Click);
            // 
            // tsmiBalanceTimeGraph
            // 
            this.tsmiBalanceTimeGraph.Name = "tsmiBalanceTimeGraph";
            this.tsmiBalanceTimeGraph.Size = new System.Drawing.Size(637, 30);
            this.tsmiBalanceTimeGraph.Text = "График использования рабочего времени...";
            this.tsmiBalanceTimeGraph.Click += new System.EventHandler(this.tsmiBalanceTimeGraph_Click);
            // 
            // tsmiMaintenanceIdleEquipment
            // 
            this.tsmiMaintenanceIdleEquipment.Name = "tsmiMaintenanceIdleEquipment";
            this.tsmiMaintenanceIdleEquipment.Size = new System.Drawing.Size(637, 30);
            this.tsmiMaintenanceIdleEquipment.Text = "Простои оборудования за смену";
            this.tsmiMaintenanceIdleEquipment.Click += new System.EventHandler(this.tsmiMaintenanceIdleEquipment_Click);
            // 
            // сменыToolStripMenuItem
            // 
            this.сменыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiShiftsDowntime,
            this.tsmiShiftsSPMNotice,
            this.tsmiShiftsReportDowntime});
            this.сменыToolStripMenuItem.Name = "сменыToolStripMenuItem";
            this.сменыToolStripMenuItem.Size = new System.Drawing.Size(83, 29);
            this.сменыToolStripMenuItem.Tag = "2";
            this.сменыToolStripMenuItem.Text = "Смены";
            // 
            // tsmiShiftsDowntime
            // 
            this.tsmiShiftsDowntime.Name = "tsmiShiftsDowntime";
            this.tsmiShiftsDowntime.Size = new System.Drawing.Size(382, 30);
            this.tsmiShiftsDowntime.Text = "Простои оборудования за смену";
            this.tsmiShiftsDowntime.Click += new System.EventHandler(this.tsmiShiftsDowntime_Click);
            // 
            // tsmiShiftsSPMNotice
            // 
            this.tsmiShiftsSPMNotice.Name = "tsmiShiftsSPMNotice";
            this.tsmiShiftsSPMNotice.Size = new System.Drawing.Size(382, 30);
            this.tsmiShiftsSPMNotice.Text = "Замечания после ППР";
            this.tsmiShiftsSPMNotice.Click += new System.EventHandler(this.tsmiShiftsSPMNotice_Click);
            // 
            // tsmiShiftsReportDowntime
            // 
            this.tsmiShiftsReportDowntime.Name = "tsmiShiftsReportDowntime";
            this.tsmiShiftsReportDowntime.Size = new System.Drawing.Size(382, 30);
            this.tsmiShiftsReportDowntime.Text = "Отчет по простоям оборудования";
            this.tsmiShiftsReportDowntime.Click += new System.EventHandler(this.tsmiShiftsReportDowntime_Click);
            // 
            // tsmDictionary
            // 
            this.tsmDictionary.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDictionaryMaintananceServiceStaff,
            this.tsmiDictionaryShiftStaff,
            this.tsmiDictionaryIdleReasons});
            this.tsmDictionary.Name = "tsmDictionary";
            this.tsmDictionary.Size = new System.Drawing.Size(142, 29);
            this.tsmDictionary.Tag = "4";
            this.tsmDictionary.Text = "Справочники";
            // 
            // tsmiDictionaryMaintananceServiceStaff
            // 
            this.tsmiDictionaryMaintananceServiceStaff.Name = "tsmiDictionaryMaintananceServiceStaff";
            this.tsmiDictionaryMaintananceServiceStaff.Size = new System.Drawing.Size(381, 30);
            this.tsmiDictionaryMaintananceServiceStaff.Text = "Сотрудники службы эксплуатации";
            this.tsmiDictionaryMaintananceServiceStaff.Click += new System.EventHandler(this.tsmiDictionaryMaintananceServiceStaff_Click);
            // 
            // tsmiDictionaryShiftStaff
            // 
            this.tsmiDictionaryShiftStaff.Name = "tsmiDictionaryShiftStaff";
            this.tsmiDictionaryShiftStaff.Size = new System.Drawing.Size(381, 30);
            this.tsmiDictionaryShiftStaff.Text = "Оперативный персонал смен";
            this.tsmiDictionaryShiftStaff.Click += new System.EventHandler(this.tsmiDictionaryShiftStaff_Click);
            // 
            // tsmiDictionaryIdleReasons
            // 
            this.tsmiDictionaryIdleReasons.Name = "tsmiDictionaryIdleReasons";
            this.tsmiDictionaryIdleReasons.Size = new System.Drawing.Size(381, 30);
            this.tsmiDictionaryIdleReasons.Text = "Причины простоя";
            this.tsmiDictionaryIdleReasons.Click += new System.EventHandler(this.tsmiDictionaryIdleReasons_Click);
            // 
            // tsmiProgramSettings
            // 
            this.tsmiProgramSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEmployeePosition,
            this.tsmiDictionaryUsers});
            this.tsmiProgramSettings.Name = "tsmiProgramSettings";
            this.tsmiProgramSettings.Size = new System.Drawing.Size(221, 29);
            this.tsmiProgramSettings.Tag = "4";
            this.tsmiProgramSettings.Text = "Настройка программы";
            // 
            // tsmiEmployeePosition
            // 
            this.tsmiEmployeePosition.Name = "tsmiEmployeePosition";
            this.tsmiEmployeePosition.Size = new System.Drawing.Size(207, 30);
            this.tsmiEmployeePosition.Text = "Должности";
            this.tsmiEmployeePosition.Click += new System.EventHandler(this.tsmiEmployeePosition_Click);
            // 
            // tsmiDictionaryUsers
            // 
            this.tsmiDictionaryUsers.Name = "tsmiDictionaryUsers";
            this.tsmiDictionaryUsers.Size = new System.Drawing.Size(207, 30);
            this.tsmiDictionaryUsers.Text = "Пользователи";
            this.tsmiDictionaryUsers.Click += new System.EventHandler(this.tsmiDictionaryUsers_Click);
            // 
            // tsmiOpenWindows
            // 
            this.tsmiOpenWindows.Name = "tsmiOpenWindows";
            this.tsmiOpenWindows.Size = new System.Drawing.Size(68, 29);
            this.tsmiOpenWindows.Text = "Окна";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(33, 29);
            this.toolStripMenuItem1.Tag = "7";
            this.toolStripMenuItem1.Text = "?";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // pnlMainForm
            // 
            this.pnlMainForm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMainForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pnlMainForm.Location = new System.Drawing.Point(0, 36);
            this.pnlMainForm.Name = "pnlMainForm";
            this.pnlMainForm.Size = new System.Drawing.Size(1605, 682);
            this.pnlMainForm.TabIndex = 1;
            this.pnlMainForm.Visible = false;
            // 
            // tsmiTechnologistIdleEquipment
            // 
            this.tsmiTechnologistIdleEquipment.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.простоиОборудованияЗаСменуToolStripMenuItem});
            this.tsmiTechnologistIdleEquipment.Name = "tsmiTechnologistIdleEquipment";
            this.tsmiTechnologistIdleEquipment.Size = new System.Drawing.Size(103, 29);
            this.tsmiTechnologistIdleEquipment.Tag = "8";
            this.tsmiTechnologistIdleEquipment.Text = "Технолог";
            // 
            // простоиОборудованияЗаСменуToolStripMenuItem
            // 
            this.простоиОборудованияЗаСменуToolStripMenuItem.Name = "простоиОборудованияЗаСменуToolStripMenuItem";
            this.простоиОборудованияЗаСменуToolStripMenuItem.Size = new System.Drawing.Size(370, 30);
            this.простоиОборудованияЗаСменуToolStripMenuItem.Text = "Простои оборудования за смену";
            this.простоиОборудованияЗаСменуToolStripMenuItem.Click += new System.EventHandler(this.простоиОборудованияЗаСменуToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1605, 721);
            this.Controls.Add(this.pnlMainForm);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "Учет рабочего времени";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmDictionary;
        private System.Windows.Forms.Panel pnlMainForm;
        private System.Windows.Forms.ToolStripMenuItem tsmiDictionaryMaintananceServiceStaff;
        private System.Windows.Forms.ToolStripMenuItem tsmiDictionaryShiftStaff;
        private System.Windows.Forms.ToolStripMenuItem tsmiMSStaff;
        private System.Windows.Forms.ToolStripMenuItem сменыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiDictionaryIdleReasons;
        private System.Windows.Forms.ToolStripMenuItem tsmiShiftsDowntime;
        private System.Windows.Forms.ToolStripMenuItem tsmiShiftsSPMNotice;
        private System.Windows.Forms.ToolStripMenuItem tsmiShiftsReportDowntime;
        private System.Windows.Forms.ToolStripMenuItem tsmiProgramSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiEmployeePosition;
        private System.Windows.Forms.ToolStripMenuItem tsmiDictionaryUsers;
        private System.Windows.Forms.ToolStripMenuItem tsmiMSSTT;
        private System.Windows.Forms.ToolStripMenuItem tsmiSTTReport;
        private System.Windows.Forms.ToolStripMenuItem tsmiLaborCostShift;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsmiEquipmentBlocking;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenWindows;
        private System.Windows.Forms.ToolStripMenuItem tsmiBalanceTimeGraph;
        private System.Windows.Forms.ToolStripMenuItem tsmiMaintenanceIdleEquipment;
        private System.Windows.Forms.ToolStripMenuItem tsmiTechnologistIdleEquipment;
        private System.Windows.Forms.ToolStripMenuItem простоиОборудованияЗаСменуToolStripMenuItem;
    }
}

