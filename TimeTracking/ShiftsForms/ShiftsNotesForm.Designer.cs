namespace TimeTracking.ShiftsForms
{
    partial class ShiftsNotesForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.rbDay = new System.Windows.Forms.RadioButton();
            this.rbNight = new System.Windows.Forms.RadioButton();
            this.pnlMachines = new System.Windows.Forms.Panel();
            this.dtpShiftDate = new TimeTracking.NewDateTimePicker();
            this.tmChangeShift = new System.Windows.Forms.Timer(this.components);
            this.btnGetMaintenance = new System.Windows.Forms.Button();
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
            this.dgvIdleReason.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIdleReason_CellEndEdit);
            this.dgvIdleReason.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvIdleReason_DataError);
            this.dgvIdleReason.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvIdleReason_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(332, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Простои";
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
            // ShiftsNotesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1325, 489);
            this.Controls.Add(this.btnGetMaintenance);
            this.Controls.Add(this.pnlMachines);
            this.Controls.Add(this.rbNight);
            this.Controls.Add(this.rbDay);
            this.Controls.Add(this.dtpShiftDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvIdleReason);
            this.Name = "ShiftsNotesForm";
            this.Text = "Заметки по ППР";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShiftsDowntimeForm_FormClosed);
            this.Load += new System.EventHandler(this.ShiftsDowntimeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvIdleReason)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvIdleReason;
        private System.Windows.Forms.Label label1;
        private NewDateTimePicker dtpShiftDate;
        private System.Windows.Forms.RadioButton rbDay;
        private System.Windows.Forms.RadioButton rbNight;
        private System.Windows.Forms.Panel pnlMachines;
        private System.Windows.Forms.Timer tmChangeShift;
        private System.Windows.Forms.Button btnGetMaintenance;
    }
}