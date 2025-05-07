namespace TimeTracking.Reports
{
    partial class StaffTimeTrackingReport
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
            this.dtpStartShiftDate = new TimeTracking.NewDateTimePicker();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblWorkingTime = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            this.lblBalanceCaption = new System.Windows.Forms.Label();
            this.lblUsed = new System.Windows.Forms.Label();
            this.lblUsedCaption = new System.Windows.Forms.Label();
            this.lblAvailable = new System.Windows.Forms.Label();
            this.lblAvailableCaption = new System.Windows.Forms.Label();
            this.lblTotalOthers = new System.Windows.Forms.Label();
            this.lblTotalOthersCaption = new System.Windows.Forms.Label();
            this.lblTotalSPM = new System.Windows.Forms.Label();
            this.lblTotalSPMCaption = new System.Windows.Forms.Label();
            this.lvFutureWorks = new System.Windows.Forms.ListView();
            this.dtpEndShiftDate = new TimeTracking.NewDateTimePicker();
            this.btnReport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dtpStartShiftDate
            // 
            this.dtpStartShiftDate.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpStartShiftDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpStartShiftDate.Location = new System.Drawing.Point(70, 4);
            this.dtpStartShiftDate.Name = "dtpStartShiftDate";
            this.dtpStartShiftDate.Size = new System.Drawing.Size(200, 26);
            this.dtpStartShiftDate.TabIndex = 0;
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
            // lblWorkingTime
            // 
            this.lblWorkingTime.AutoSize = true;
            this.lblWorkingTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblWorkingTime.Location = new System.Drawing.Point(134, 40);
            this.lblWorkingTime.Name = "lblWorkingTime";
            this.lblWorkingTime.Size = new System.Drawing.Size(324, 24);
            this.lblWorkingTime.TabIndex = 4;
            this.lblWorkingTime.Text = "Использование рабочего времени";
            // 
            // lblBalance
            // 
            this.lblBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBalance.AutoSize = true;
            this.lblBalance.Location = new System.Drawing.Point(440, 483);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(18, 20);
            this.lblBalance.TabIndex = 39;
            this.lblBalance.Text = "0";
            // 
            // lblBalanceCaption
            // 
            this.lblBalanceCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBalanceCaption.AutoSize = true;
            this.lblBalanceCaption.Location = new System.Drawing.Point(12, 483);
            this.lblBalanceCaption.Name = "lblBalanceCaption";
            this.lblBalanceCaption.Size = new System.Drawing.Size(406, 20);
            this.lblBalanceCaption.TabIndex = 38;
            this.lblBalanceCaption.Text = "Эффективность использования рабочего времени:";
            // 
            // lblUsed
            // 
            this.lblUsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUsed.AutoSize = true;
            this.lblUsed.Location = new System.Drawing.Point(440, 456);
            this.lblUsed.Name = "lblUsed";
            this.lblUsed.Size = new System.Drawing.Size(18, 20);
            this.lblUsed.TabIndex = 37;
            this.lblUsed.Text = "0";
            // 
            // lblUsedCaption
            // 
            this.lblUsedCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUsedCaption.AutoSize = true;
            this.lblUsedCaption.Location = new System.Drawing.Point(12, 456);
            this.lblUsedCaption.Name = "lblUsedCaption";
            this.lblUsedCaption.Size = new System.Drawing.Size(235, 20);
            this.lblUsedCaption.TabIndex = 36;
            this.lblUsedCaption.Text = "Использовано за период (чч):";
            // 
            // lblAvailable
            // 
            this.lblAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAvailable.AutoSize = true;
            this.lblAvailable.Location = new System.Drawing.Point(440, 436);
            this.lblAvailable.Name = "lblAvailable";
            this.lblAvailable.Size = new System.Drawing.Size(18, 20);
            this.lblAvailable.TabIndex = 35;
            this.lblAvailable.Text = "0";
            // 
            // lblAvailableCaption
            // 
            this.lblAvailableCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAvailableCaption.AutoSize = true;
            this.lblAvailableCaption.Location = new System.Drawing.Point(12, 436);
            this.lblAvailableCaption.Name = "lblAvailableCaption";
            this.lblAvailableCaption.Size = new System.Drawing.Size(198, 20);
            this.lblAvailableCaption.TabIndex = 34;
            this.lblAvailableCaption.Text = "Доступно за период (чч):";
            // 
            // lblTotalOthers
            // 
            this.lblTotalOthers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalOthers.AutoSize = true;
            this.lblTotalOthers.Location = new System.Drawing.Point(440, 403);
            this.lblTotalOthers.Name = "lblTotalOthers";
            this.lblTotalOthers.Size = new System.Drawing.Size(18, 20);
            this.lblTotalOthers.TabIndex = 33;
            this.lblTotalOthers.Text = "0";
            // 
            // lblTotalOthersCaption
            // 
            this.lblTotalOthersCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalOthersCaption.AutoSize = true;
            this.lblTotalOthersCaption.Location = new System.Drawing.Point(12, 403);
            this.lblTotalOthersCaption.Name = "lblTotalOthersCaption";
            this.lblTotalOthersCaption.Size = new System.Drawing.Size(173, 20);
            this.lblTotalOthersCaption.TabIndex = 32;
            this.lblTotalOthersCaption.Text = "Итого прочие заказы:";
            // 
            // lblTotalSPM
            // 
            this.lblTotalSPM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalSPM.AutoSize = true;
            this.lblTotalSPM.Location = new System.Drawing.Point(440, 383);
            this.lblTotalSPM.Name = "lblTotalSPM";
            this.lblTotalSPM.Size = new System.Drawing.Size(18, 20);
            this.lblTotalSPM.TabIndex = 31;
            this.lblTotalSPM.Text = "0";
            // 
            // lblTotalSPMCaption
            // 
            this.lblTotalSPMCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalSPMCaption.AutoSize = true;
            this.lblTotalSPMCaption.Location = new System.Drawing.Point(12, 383);
            this.lblTotalSPMCaption.Name = "lblTotalSPMCaption";
            this.lblTotalSPMCaption.Size = new System.Drawing.Size(118, 20);
            this.lblTotalSPMCaption.TabIndex = 30;
            this.lblTotalSPMCaption.Text = "Итого на ППР:";
            // 
            // lvFutureWorks
            // 
            this.lvFutureWorks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFutureWorks.FullRowSelect = true;
            this.lvFutureWorks.GridLines = true;
            this.lvFutureWorks.HideSelection = false;
            this.lvFutureWorks.Location = new System.Drawing.Point(16, 67);
            this.lvFutureWorks.MultiSelect = false;
            this.lvFutureWorks.Name = "lvFutureWorks";
            this.lvFutureWorks.Size = new System.Drawing.Size(608, 313);
            this.lvFutureWorks.TabIndex = 29;
            this.lvFutureWorks.UseCompatibleStateImageBehavior = false;
            this.lvFutureWorks.View = System.Windows.Forms.View.Details;
            // 
            // dtpEndShiftDate
            // 
            this.dtpEndShiftDate.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpEndShiftDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpEndShiftDate.Location = new System.Drawing.Point(303, 4);
            this.dtpEndShiftDate.Name = "dtpEndShiftDate";
            this.dtpEndShiftDate.Size = new System.Drawing.Size(200, 26);
            this.dtpEndShiftDate.TabIndex = 40;
            // 
            // btnReport
            // 
            this.btnReport.Location = new System.Drawing.Point(519, 4);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(105, 26);
            this.btnReport.TabIndex = 41;
            this.btnReport.Text = "Отчет";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // StaffTimeTrackingReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 524);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.dtpEndShiftDate);
            this.Controls.Add(this.lblBalance);
            this.Controls.Add(this.lblBalanceCaption);
            this.Controls.Add(this.lblUsed);
            this.Controls.Add(this.lblUsedCaption);
            this.Controls.Add(this.lblAvailable);
            this.Controls.Add(this.lblAvailableCaption);
            this.Controls.Add(this.lblTotalOthers);
            this.Controls.Add(this.lblTotalOthersCaption);
            this.Controls.Add(this.lblTotalSPM);
            this.Controls.Add(this.lblTotalSPMCaption);
            this.Controls.Add(this.lvFutureWorks);
            this.Controls.Add(this.lblWorkingTime);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.dtpStartShiftDate);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "StaffTimeTrackingReport";
            this.Text = "Отчет по использованию рабочего времени";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NewDateTimePicker dtpStartShiftDate;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblWorkingTime;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.Label lblBalanceCaption;
        private System.Windows.Forms.Label lblUsed;
        private System.Windows.Forms.Label lblUsedCaption;
        private System.Windows.Forms.Label lblAvailable;
        private System.Windows.Forms.Label lblAvailableCaption;
        private System.Windows.Forms.Label lblTotalOthers;
        private System.Windows.Forms.Label lblTotalOthersCaption;
        private System.Windows.Forms.Label lblTotalSPM;
        private System.Windows.Forms.Label lblTotalSPMCaption;
        private System.Windows.Forms.ListView lvFutureWorks;
        private NewDateTimePicker dtpEndShiftDate;
        private System.Windows.Forms.Button btnReport;
    }
}