namespace TimeTracking.DictionaryForms
{
    partial class DictionaryEmployeePosition
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
            this.lblWindowCaption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblWindowCaption
            // 
            this.lblWindowCaption.AutoSize = true;
            this.lblWindowCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblWindowCaption.Location = new System.Drawing.Point(276, 23);
            this.lblWindowCaption.Name = "lblWindowCaption";
            this.lblWindowCaption.Size = new System.Drawing.Size(95, 20);
            this.lblWindowCaption.TabIndex = 0;
            this.lblWindowCaption.Text = "Должности";
            // 
            // DictionaryEmployeePosition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblWindowCaption);
            this.Name = "DictionaryEmployeePosition";
            this.Text = "DictionaryEmployeePosition";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWindowCaption;
    }
}