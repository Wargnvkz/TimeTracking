namespace TimeTracking.DictionaryForms
{
    partial class DictionaryIdleReasonsForm
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
            this.dgvReasons = new System.Windows.Forms.DataGridView();
            this.lblReasonCaption = new System.Windows.Forms.Label();
            this.cbResonTypes = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReasons)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvReasons
            // 
            this.dgvReasons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvReasons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReasons.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvReasons.Location = new System.Drawing.Point(1, 35);
            this.dgvReasons.Name = "dgvReasons";
            this.dgvReasons.Size = new System.Drawing.Size(1552, 412);
            this.dgvReasons.TabIndex = 3;
            this.dgvReasons.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvReasons_DataError);
            // 
            // lblReasonCaption
            // 
            this.lblReasonCaption.AutoSize = true;
            this.lblReasonCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblReasonCaption.Location = new System.Drawing.Point(12, 9);
            this.lblReasonCaption.Name = "lblReasonCaption";
            this.lblReasonCaption.Size = new System.Drawing.Size(156, 20);
            this.lblReasonCaption.TabIndex = 0;
            this.lblReasonCaption.Text = "Причины простоев:";
            // 
            // cbResonTypes
            // 
            this.cbResonTypes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbResonTypes.FormattingEnabled = true;
            this.cbResonTypes.Location = new System.Drawing.Point(174, 6);
            this.cbResonTypes.Name = "cbResonTypes";
            this.cbResonTypes.Size = new System.Drawing.Size(427, 28);
            this.cbResonTypes.TabIndex = 2;
            // 
            // DictionaryIdleReasonsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1555, 450);
            this.Controls.Add(this.cbResonTypes);
            this.Controls.Add(this.lblReasonCaption);
            this.Controls.Add(this.dgvReasons);
            this.Name = "DictionaryIdleReasonsForm";
            this.Text = "DictionaryIdleReasonsForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DictionaryIdleReasonsForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReasons)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvReasons;
        private System.Windows.Forms.Label lblReasonCaption;
        private System.Windows.Forms.ComboBox cbResonTypes;
    }
}