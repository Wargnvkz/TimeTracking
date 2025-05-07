namespace TimeTracking.UserControls
{
    partial class MessageRecordListControl
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

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tblMessagesList = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddNewRecord = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tblMessagesList
            // 
            this.tblMessagesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblMessagesList.AutoScroll = true;
            this.tblMessagesList.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tblMessagesList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tblMessagesList.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tblMessagesList.ColumnCount = 1;
            this.tblMessagesList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMessagesList.Location = new System.Drawing.Point(0, 39);
            this.tblMessagesList.Name = "tblMessagesList";
            this.tblMessagesList.Size = new System.Drawing.Size(560, 452);
            this.tblMessagesList.TabIndex = 0;
            // 
            // btnAddNewRecord
            // 
            this.btnAddNewRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNewRecord.BackColor = System.Drawing.SystemColors.Control;
            this.btnAddNewRecord.Font = new System.Drawing.Font("Arial Black", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAddNewRecord.ForeColor = System.Drawing.Color.LimeGreen;
            this.btnAddNewRecord.Location = new System.Drawing.Point(526, 0);
            this.btnAddNewRecord.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddNewRecord.Name = "btnAddNewRecord";
            this.btnAddNewRecord.Size = new System.Drawing.Size(37, 36);
            this.btnAddNewRecord.TabIndex = 2;
            this.btnAddNewRecord.Text = "+";
            this.btnAddNewRecord.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNewRecord.UseVisualStyleBackColor = false;
            this.btnAddNewRecord.Click += new System.EventHandler(this.btnAddNewRecord_Click);
            // 
            // MessageRecordListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAddNewRecord);
            this.Controls.Add(this.tblMessagesList);
            this.Name = "MessageRecordListControl";
            this.Size = new System.Drawing.Size(563, 494);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblMessagesList;
        private System.Windows.Forms.Button btnAddNewRecord;
    }
}
