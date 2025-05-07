namespace TimeTracking.UserControls
{
    partial class MessageRecordControl
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
            this.components = new System.ComponentModel.Container();
            this.txbText = new System.Windows.Forms.TextBox();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.lblTextCaption = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblDatetimeCreatedCaption = new System.Windows.Forms.Label();
            this.lblDateTimeCreate = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiDeleteFile = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txbText
            // 
            this.txbText.AllowDrop = true;
            this.txbText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txbText.Location = new System.Drawing.Point(3, 38);
            this.txbText.Multiline = true;
            this.txbText.Name = "txbText";
            this.txbText.Size = new System.Drawing.Size(650, 73);
            this.txbText.TabIndex = 0;
            this.txbText.TextChanged += new System.EventHandler(this.txbText_TextChanged);
            this.txbText.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvFiles_DragDrop);
            this.txbText.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvFiles_DragEnter);
            // 
            // lvFiles
            // 
            this.lvFiles.AllowDrop = true;
            this.lvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFiles.HideSelection = false;
            this.lvFiles.Location = new System.Drawing.Point(3, 117);
            this.lvFiles.MultiSelect = false;
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(650, 82);
            this.lvFiles.TabIndex = 1;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvFiles_DragDrop);
            this.lvFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvFiles_DragEnter);
            this.lvFiles.DoubleClick += new System.EventHandler(this.lvFiles_DoubleClick);
            this.lvFiles.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvFiles_KeyUp);
            this.lvFiles.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvFiles_MouseClick);
            // 
            // lblTextCaption
            // 
            this.lblTextCaption.AutoSize = true;
            this.lblTextCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTextCaption.Location = new System.Drawing.Point(3, 1);
            this.lblTextCaption.Name = "lblTextCaption";
            this.lblTextCaption.Size = new System.Drawing.Size(194, 17);
            this.lblTextCaption.TabIndex = 2;
            this.lblTextCaption.Text = "Комментарий пользователя";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblUser.Location = new System.Drawing.Point(203, 1);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(13, 17);
            this.lblUser.TabIndex = 3;
            this.lblUser.Text = "-";
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDelete.Location = new System.Drawing.Point(608, 1);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(45, 36);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "🗑️";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblDatetimeCreatedCaption
            // 
            this.lblDatetimeCreatedCaption.AutoSize = true;
            this.lblDatetimeCreatedCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDatetimeCreatedCaption.Location = new System.Drawing.Point(3, 18);
            this.lblDatetimeCreatedCaption.Name = "lblDatetimeCreatedCaption";
            this.lblDatetimeCreatedCaption.Size = new System.Drawing.Size(60, 17);
            this.lblDatetimeCreatedCaption.TabIndex = 5;
            this.lblDatetimeCreatedCaption.Text = "Создан:";
            // 
            // lblDateTimeCreate
            // 
            this.lblDateTimeCreate.AutoSize = true;
            this.lblDateTimeCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDateTimeCreate.Location = new System.Drawing.Point(69, 18);
            this.lblDateTimeCreate.Name = "lblDateTimeCreate";
            this.lblDateTimeCreate.Size = new System.Drawing.Size(13, 17);
            this.lblDateTimeCreate.TabIndex = 6;
            this.lblDateTimeCreate.Text = "-";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDeleteFile});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 48);
            // 
            // удалитьToolStripMenuItem
            // 
            this.tsmiDeleteFile.Name = "удалитьToolStripMenuItem";
            this.tsmiDeleteFile.Size = new System.Drawing.Size(180, 22);
            this.tsmiDeleteFile.Text = "Удалить";
            this.tsmiDeleteFile.Click += new System.EventHandler(this.tsmiDeleteFile_Click);
            // 
            // MessageRecordControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Honeydew;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblDateTimeCreate);
            this.Controls.Add(this.lblDatetimeCreatedCaption);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.lblTextCaption);
            this.Controls.Add(this.lvFiles);
            this.Controls.Add(this.txbText);
            this.MinimumSize = new System.Drawing.Size(400, 2);
            this.Name = "MessageRecordControl";
            this.Size = new System.Drawing.Size(656, 204);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvFiles_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvFiles_DragEnter);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txbText;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.Label lblTextCaption;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblDatetimeCreatedCaption;
        private System.Windows.Forms.Label lblDateTimeCreate;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteFile;
    }
}
