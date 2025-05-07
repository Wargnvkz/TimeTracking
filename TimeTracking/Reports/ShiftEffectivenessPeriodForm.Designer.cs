namespace TimeTracking.Reports
{
    partial class ShiftEffectivenessPeriodForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.lvShifts = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPeriodFromCaaption = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblToCaption = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.btnReport = new System.Windows.Forms.Button();
            this.chBalanceGraph = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chBalanceGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // lvShifts
            // 
            this.lvShifts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvShifts.AutoArrange = false;
            this.lvShifts.FullRowSelect = true;
            this.lvShifts.HideSelection = false;
            this.lvShifts.Location = new System.Drawing.Point(629, 5);
            this.lvShifts.MultiSelect = false;
            this.lvShifts.Name = "lvShifts";
            this.lvShifts.Size = new System.Drawing.Size(456, 103);
            this.lvShifts.TabIndex = 0;
            this.lvShifts.UseCompatibleStateImageBehavior = false;
            this.lvShifts.View = System.Windows.Forms.View.Details;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(559, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Смены:";
            // 
            // lblPeriodFromCaaption
            // 
            this.lblPeriodFromCaaption.AutoSize = true;
            this.lblPeriodFromCaaption.Location = new System.Drawing.Point(12, 19);
            this.lblPeriodFromCaaption.Name = "lblPeriodFromCaaption";
            this.lblPeriodFromCaaption.Size = new System.Drawing.Size(94, 20);
            this.lblPeriodFromCaaption.TabIndex = 3;
            this.lblPeriodFromCaaption.Text = "Период: от";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Location = new System.Drawing.Point(112, 14);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(200, 26);
            this.dtpFrom.TabIndex = 4;
            // 
            // lblToCaption
            // 
            this.lblToCaption.AutoSize = true;
            this.lblToCaption.Location = new System.Drawing.Point(318, 19);
            this.lblToCaption.Name = "lblToCaption";
            this.lblToCaption.Size = new System.Drawing.Size(29, 20);
            this.lblToCaption.TabIndex = 5;
            this.lblToCaption.Text = "до";
            // 
            // dtpTo
            // 
            this.dtpTo.Location = new System.Drawing.Point(353, 14);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(200, 26);
            this.dtpTo.TabIndex = 6;
            // 
            // btnReport
            // 
            this.btnReport.Location = new System.Drawing.Point(16, 58);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(607, 50);
            this.btnReport.TabIndex = 7;
            this.btnReport.Text = "Сформированить график загруженности смен";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // chBalanceGraph
            // 
            this.chBalanceGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.AxisX.MinorGrid.Enabled = true;
            chartArea1.Name = "ChartArea1";
            this.chBalanceGraph.ChartAreas.Add(chartArea1);
            this.chBalanceGraph.Location = new System.Drawing.Point(16, 114);
            this.chBalanceGraph.Name = "chBalanceGraph";
            series1.ChartArea = "ChartArea1";
            series1.Name = "Series1";
            this.chBalanceGraph.Series.Add(series1);
            this.chBalanceGraph.Size = new System.Drawing.Size(1075, 388);
            this.chBalanceGraph.TabIndex = 8;
            this.chBalanceGraph.Text = "chart1";
            // 
            // ShiftEffectivenessPeriodForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 499);
            this.Controls.Add(this.chBalanceGraph);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.dtpTo);
            this.Controls.Add(this.lblToCaption);
            this.Controls.Add(this.dtpFrom);
            this.Controls.Add(this.lblPeriodFromCaaption);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvShifts);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ShiftEffectivenessPeriodForm";
            this.Text = "ShiftEffectivenessPeriodForm";
            ((System.ComponentModel.ISupportInitialize)(this.chBalanceGraph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvShifts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPeriodFromCaaption;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblToCaption;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.DataVisualization.Charting.Chart chBalanceGraph;
    }
}