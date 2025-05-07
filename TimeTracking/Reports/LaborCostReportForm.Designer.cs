namespace TimeTracking.Reports
{
    partial class LaborCostReportForm
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
            this.lvFutureWorks = new System.Windows.Forms.ListView();
            this.txbSupervisor = new System.Windows.Forms.TextBox();
            this.lblWorkingTime = new System.Windows.Forms.Label();
            this.lblShift = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpShiftDate = new TimeTracking.NewDateTimePicker();
            this.lblTotalSPMCaption = new System.Windows.Forms.Label();
            this.lblTotalSPM = new System.Windows.Forms.Label();
            this.lblTotalOthers = new System.Windows.Forms.Label();
            this.lblTotalOthersCaption = new System.Windows.Forms.Label();
            this.lblLaborCost = new System.Windows.Forms.Label();
            this.lblLaborCostCaption = new System.Windows.Forms.Label();
            this.lblAvailable = new System.Windows.Forms.Label();
            this.lblAvailableCaption = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            this.lblBalanceCaption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lvFutureWorks
            // 
            this.lvFutureWorks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFutureWorks.HideSelection = false;
            this.lvFutureWorks.Location = new System.Drawing.Point(16, 75);
            this.lvFutureWorks.Name = "lvFutureWorks";
            this.lvFutureWorks.Size = new System.Drawing.Size(831, 231);
            this.lvFutureWorks.TabIndex = 18;
            this.lvFutureWorks.UseCompatibleStateImageBehavior = false;
            this.lvFutureWorks.View = System.Windows.Forms.View.Details;
            // 
            // txbSupervisor
            // 
            this.txbSupervisor.Location = new System.Drawing.Point(537, 6);
            this.txbSupervisor.Name = "txbSupervisor";
            this.txbSupervisor.ReadOnly = true;
            this.txbSupervisor.Size = new System.Drawing.Size(313, 26);
            this.txbSupervisor.TabIndex = 17;
            // 
            // lblWorkingTime
            // 
            this.lblWorkingTime.AutoSize = true;
            this.lblWorkingTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblWorkingTime.Location = new System.Drawing.Point(187, 48);
            this.lblWorkingTime.Name = "lblWorkingTime";
            this.lblWorkingTime.Size = new System.Drawing.Size(505, 24);
            this.lblWorkingTime.TabIndex = 16;
            this.lblWorkingTime.Text = "Трудозатраты на выполнение запланированных работ";
            // 
            // lblShift
            // 
            this.lblShift.AutoSize = true;
            this.lblShift.Location = new System.Drawing.Point(391, 9);
            this.lblShift.Name = "lblShift";
            this.lblShift.Size = new System.Drawing.Size(140, 20);
            this.lblShift.TabIndex = 15;
            this.lblShift.Text = "Инженер (смена):";
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(12, 9);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(52, 20);
            this.lblDate.TabIndex = 14;
            this.lblDate.Text = "Дата:";
            // 
            // dtpShiftDate
            // 
            this.dtpShiftDate.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpShiftDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dtpShiftDate.Location = new System.Drawing.Point(70, 4);
            this.dtpShiftDate.Name = "dtpShiftDate";
            this.dtpShiftDate.Size = new System.Drawing.Size(200, 26);
            this.dtpShiftDate.TabIndex = 13;
            this.dtpShiftDate.ValueChanged += new System.EventHandler(this.dtpShiftDate_ValueChanged);
            // 
            // lblTotalSPMCaption
            // 
            this.lblTotalSPMCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalSPMCaption.AutoSize = true;
            this.lblTotalSPMCaption.Location = new System.Drawing.Point(12, 309);
            this.lblTotalSPMCaption.Name = "lblTotalSPMCaption";
            this.lblTotalSPMCaption.Size = new System.Drawing.Size(118, 20);
            this.lblTotalSPMCaption.TabIndex = 19;
            this.lblTotalSPMCaption.Text = "Итого на ППР:";
            // 
            // lblTotalSPM
            // 
            this.lblTotalSPM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalSPM.AutoSize = true;
            this.lblTotalSPM.Location = new System.Drawing.Point(337, 309);
            this.lblTotalSPM.Name = "lblTotalSPM";
            this.lblTotalSPM.Size = new System.Drawing.Size(18, 20);
            this.lblTotalSPM.TabIndex = 20;
            this.lblTotalSPM.Text = "0";
            // 
            // lblTotalOthers
            // 
            this.lblTotalOthers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalOthers.AutoSize = true;
            this.lblTotalOthers.Location = new System.Drawing.Point(337, 329);
            this.lblTotalOthers.Name = "lblTotalOthers";
            this.lblTotalOthers.Size = new System.Drawing.Size(18, 20);
            this.lblTotalOthers.TabIndex = 22;
            this.lblTotalOthers.Text = "0";
            // 
            // lblTotalOthersCaption
            // 
            this.lblTotalOthersCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalOthersCaption.AutoSize = true;
            this.lblTotalOthersCaption.Location = new System.Drawing.Point(12, 329);
            this.lblTotalOthersCaption.Name = "lblTotalOthersCaption";
            this.lblTotalOthersCaption.Size = new System.Drawing.Size(173, 20);
            this.lblTotalOthersCaption.TabIndex = 21;
            this.lblTotalOthersCaption.Text = "Итого прочие заказы:";
            // 
            // lblLaborCost
            // 
            this.lblLaborCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLaborCost.AutoSize = true;
            this.lblLaborCost.Location = new System.Drawing.Point(337, 382);
            this.lblLaborCost.Name = "lblLaborCost";
            this.lblLaborCost.Size = new System.Drawing.Size(18, 20);
            this.lblLaborCost.TabIndex = 26;
            this.lblLaborCost.Text = "0";
            // 
            // lblLaborCostCaption
            // 
            this.lblLaborCostCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLaborCostCaption.AutoSize = true;
            this.lblLaborCostCaption.Location = new System.Drawing.Point(12, 382);
            this.lblLaborCostCaption.Name = "lblLaborCostCaption";
            this.lblLaborCostCaption.Size = new System.Drawing.Size(329, 20);
            this.lblLaborCostCaption.TabIndex = 25;
            this.lblLaborCostCaption.Text = "Потребность на выполнение заказов (чч):";
            // 
            // lblAvailable
            // 
            this.lblAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAvailable.AutoSize = true;
            this.lblAvailable.Location = new System.Drawing.Point(337, 362);
            this.lblAvailable.Name = "lblAvailable";
            this.lblAvailable.Size = new System.Drawing.Size(18, 20);
            this.lblAvailable.TabIndex = 24;
            this.lblAvailable.Text = "0";
            // 
            // lblAvailableCaption
            // 
            this.lblAvailableCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAvailableCaption.AutoSize = true;
            this.lblAvailableCaption.Location = new System.Drawing.Point(12, 362);
            this.lblAvailableCaption.Name = "lblAvailableCaption";
            this.lblAvailableCaption.Size = new System.Drawing.Size(117, 20);
            this.lblAvailableCaption.TabIndex = 23;
            this.lblAvailableCaption.Text = "Доступно (чч):";
            // 
            // lblBalance
            // 
            this.lblBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBalance.AutoSize = true;
            this.lblBalance.Location = new System.Drawing.Point(337, 409);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(18, 20);
            this.lblBalance.TabIndex = 28;
            this.lblBalance.Text = "0";
            // 
            // lblBalanceCaption
            // 
            this.lblBalanceCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBalanceCaption.AutoSize = true;
            this.lblBalanceCaption.Location = new System.Drawing.Point(12, 409);
            this.lblBalanceCaption.Name = "lblBalanceCaption";
            this.lblBalanceCaption.Size = new System.Drawing.Size(212, 20);
            this.lblBalanceCaption.TabIndex = 27;
            this.lblBalanceCaption.Text = "Баланс рабочего времени:";
            // 
            // LaborCostReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 454);
            this.Controls.Add(this.lblBalance);
            this.Controls.Add(this.lblBalanceCaption);
            this.Controls.Add(this.lblLaborCost);
            this.Controls.Add(this.lblLaborCostCaption);
            this.Controls.Add(this.lblAvailable);
            this.Controls.Add(this.lblAvailableCaption);
            this.Controls.Add(this.lblTotalOthers);
            this.Controls.Add(this.lblTotalOthersCaption);
            this.Controls.Add(this.lblTotalSPM);
            this.Controls.Add(this.lblTotalSPMCaption);
            this.Controls.Add(this.lvFutureWorks);
            this.Controls.Add(this.txbSupervisor);
            this.Controls.Add(this.lblWorkingTime);
            this.Controls.Add(this.lblShift);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.dtpShiftDate);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "LaborCostReportForm";
            this.Text = "Трудозатраты будущих смен";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvFutureWorks;
        private System.Windows.Forms.TextBox txbSupervisor;
        private System.Windows.Forms.Label lblWorkingTime;
        private System.Windows.Forms.Label lblShift;
        private System.Windows.Forms.Label lblDate;
        private NewDateTimePicker dtpShiftDate;
        private System.Windows.Forms.Label lblTotalSPMCaption;
        private System.Windows.Forms.Label lblTotalSPM;
        private System.Windows.Forms.Label lblTotalOthers;
        private System.Windows.Forms.Label lblTotalOthersCaption;
        private System.Windows.Forms.Label lblLaborCost;
        private System.Windows.Forms.Label lblLaborCostCaption;
        private System.Windows.Forms.Label lblAvailable;
        private System.Windows.Forms.Label lblAvailableCaption;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.Label lblBalanceCaption;
    }
}