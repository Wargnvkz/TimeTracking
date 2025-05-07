namespace TimeTracking.ShiftsForms
{
    partial class ShiftsDowntimeForm
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
            this.dgvIdleReason = new System.Windows.Forms.DataGridView();
            this.lblCaption = new System.Windows.Forms.Label();
            this.rbDay = new System.Windows.Forms.RadioButton();
            this.rbNight = new System.Windows.Forms.RadioButton();
            this.pnlMachines = new System.Windows.Forms.Panel();
            this.dtpShiftDate = new TimeTracking.NewDateTimePicker();
            this.tmChangeShift = new System.Windows.Forms.Timer(this.components);
            this.btnStartIdle = new System.Windows.Forms.Button();
            this.btnGetMaintenance = new System.Windows.Forms.Button();
            this.btnStopIdle = new System.Windows.Forms.Button();
            this.lblRefreshAfterCaption = new System.Windows.Forms.Label();
            this.lblSelectedEquipment = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIdleReason)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvIdleReason
            // 
            this.dgvIdleReason.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvIdleReason.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIdleReason.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvIdleReason.Location = new System.Drawing.Point(3, 118);
            this.dgvIdleReason.Name = "dgvIdleReason";
            this.dgvIdleReason.Size = new System.Drawing.Size(1320, 369);
            this.dgvIdleReason.TabIndex = 0;
            this.dgvIdleReason.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvIdleReason_CellBeginEdit);
            this.dgvIdleReason.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIdleReason_CellContentClick);
            this.dgvIdleReason.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIdleReason_CellDoubleClick);
            this.dgvIdleReason.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIdleReason_CellEndEdit);
            this.dgvIdleReason.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvIdleReason_CellFormatting);
            this.dgvIdleReason.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvIdleReason_DataError);
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCaption.Location = new System.Drawing.Point(332, 9);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(186, 20);
            this.lblCaption.TabIndex = 1;
            this.lblCaption.Text = "Простои оборудования";
            // 
            // rbDay
            // 
            this.rbDay.AutoSize = true;
            this.rbDay.Checked = true;
            this.rbDay.Location = new System.Drawing.Point(240, 94);
            this.rbDay.Name = "rbDay";
            this.rbDay.Size = new System.Drawing.Size(70, 17);
            this.rbDay.TabIndex = 3;
            this.rbDay.TabStop = true;
            this.rbDay.Text = "Дневная";
            this.rbDay.UseVisualStyleBackColor = true;
            this.rbDay.CheckedChanged += new System.EventHandler(this.rbDay_CheckedChanged);
            // 
            // rbNight
            // 
            this.rbNight.AutoSize = true;
            this.rbNight.Location = new System.Drawing.Point(336, 95);
            this.rbNight.Name = "rbNight";
            this.rbNight.Size = new System.Drawing.Size(62, 17);
            this.rbNight.TabIndex = 4;
            this.rbNight.Text = "Ночная";
            this.rbNight.UseVisualStyleBackColor = true;
            // 
            // pnlMachines
            // 
            this.pnlMachines.Location = new System.Drawing.Point(3, 32);
            this.pnlMachines.Name = "pnlMachines";
            this.pnlMachines.Size = new System.Drawing.Size(1310, 48);
            this.pnlMachines.TabIndex = 5;
            // 
            // dtpShiftDate
            // 
            this.dtpShiftDate.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpShiftDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpShiftDate.Location = new System.Drawing.Point(12, 86);
            this.dtpShiftDate.Name = "dtpShiftDate";
            this.dtpShiftDate.Size = new System.Drawing.Size(200, 26);
            this.dtpShiftDate.TabIndex = 2;
            this.dtpShiftDate.ValueChanged += new System.EventHandler(this.dtpShiftDate_ValueChanged);
            // 
            // tmChangeShift
            // 
            this.tmChangeShift.Enabled = true;
            this.tmChangeShift.Interval = 1000;
            this.tmChangeShift.Tick += new System.EventHandler(this.tmChangeShift_Tick);
            // 
            // btnStartIdle
            // 
            this.btnStartIdle.Location = new System.Drawing.Point(3, 9);
            this.btnStartIdle.Name = "btnStartIdle";
            this.btnStartIdle.Size = new System.Drawing.Size(95, 23);
            this.btnStartIdle.TabIndex = 6;
            this.btnStartIdle.Text = "Начать простой";
            this.btnStartIdle.UseVisualStyleBackColor = true;
            this.btnStartIdle.Visible = false;
            this.btnStartIdle.Click += new System.EventHandler(this.btnStartIdle_Click);
            // 
            // btnGetMaintenance
            // 
            this.btnGetMaintenance.Location = new System.Drawing.Point(565, 6);
            this.btnGetMaintenance.Name = "btnGetMaintenance";
            this.btnGetMaintenance.Size = new System.Drawing.Size(118, 23);
            this.btnGetMaintenance.TabIndex = 7;
            this.btnGetMaintenance.Text = "Проверить ППР";
            this.btnGetMaintenance.UseVisualStyleBackColor = true;
            this.btnGetMaintenance.Visible = false;
            this.btnGetMaintenance.Click += new System.EventHandler(this.btnGetMaintenance_Click);
            // 
            // btnStopIdle
            // 
            this.btnStopIdle.Location = new System.Drawing.Point(104, 9);
            this.btnStopIdle.Name = "btnStopIdle";
            this.btnStopIdle.Size = new System.Drawing.Size(123, 23);
            this.btnStopIdle.TabIndex = 8;
            this.btnStopIdle.Text = "Остановить простой";
            this.btnStopIdle.UseVisualStyleBackColor = true;
            this.btnStopIdle.Visible = false;
            this.btnStopIdle.Click += new System.EventHandler(this.btnStopIdle_Click);
            // 
            // lblRefreshAfterCaption
            // 
            this.lblRefreshAfterCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRefreshAfterCaption.AutoSize = true;
            this.lblRefreshAfterCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRefreshAfterCaption.Location = new System.Drawing.Point(1183, 6);
            this.lblRefreshAfterCaption.Name = "lblRefreshAfterCaption";
            this.lblRefreshAfterCaption.Size = new System.Drawing.Size(140, 13);
            this.lblRefreshAfterCaption.TabIndex = 9;
            this.lblRefreshAfterCaption.Text = "Обновление через: 30 сек";
            // 
            // lblSelectedEquipment
            // 
            this.lblSelectedEquipment.AutoSize = true;
            this.lblSelectedEquipment.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSelectedEquipment.Location = new System.Drawing.Point(527, 88);
            this.lblSelectedEquipment.Name = "lblSelectedEquipment";
            this.lblSelectedEquipment.Size = new System.Drawing.Size(122, 24);
            this.lblSelectedEquipment.TabIndex = 10;
            this.lblSelectedEquipment.Text = "Машина № 0";
            // 
            // ShiftsDowntimeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1325, 489);
            this.Controls.Add(this.lblSelectedEquipment);
            this.Controls.Add(this.lblRefreshAfterCaption);
            this.Controls.Add(this.btnStopIdle);
            this.Controls.Add(this.btnGetMaintenance);
            this.Controls.Add(this.btnStartIdle);
            this.Controls.Add(this.pnlMachines);
            this.Controls.Add(this.rbNight);
            this.Controls.Add(this.rbDay);
            this.Controls.Add(this.dtpShiftDate);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.dgvIdleReason);
            this.Name = "ShiftsDowntimeForm";
            this.Text = "Простой машин";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShiftsDowntimeForm_FormClosed);
            this.Load += new System.EventHandler(this.ShiftsDowntimeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvIdleReason)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvIdleReason;
        private System.Windows.Forms.Label lblCaption;
        private NewDateTimePicker dtpShiftDate;
        private System.Windows.Forms.RadioButton rbDay;
        private System.Windows.Forms.RadioButton rbNight;
        private System.Windows.Forms.Panel pnlMachines;
        private System.Windows.Forms.Timer tmChangeShift;
        private System.Windows.Forms.Button btnStartIdle;
        private System.Windows.Forms.Button btnGetMaintenance;
        private System.Windows.Forms.Button btnStopIdle;
        private System.Windows.Forms.Label lblRefreshAfterCaption;
        private System.Windows.Forms.Label lblSelectedEquipment;
    }
}