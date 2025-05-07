namespace TimeTracking.DictionaryForms
{
    partial class DictionaryShiftStaffUserEdit
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
            this.lblUserName = new System.Windows.Forms.Label();
            this.txbUserName = new System.Windows.Forms.TextBox();
            this.txbComputers = new System.Windows.Forms.TextBox();
            this.lblComputers = new System.Windows.Forms.Label();
            this.lblRights = new System.Windows.Forms.Label();
            this.cbRightMaintainService = new System.Windows.Forms.CheckBox();
            this.cbRightEngineerOperator = new System.Windows.Forms.CheckBox();
            this.cbRightAdministrator = new System.Windows.Forms.CheckBox();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbRightTechnologist = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(3, 9);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(157, 20);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "Имя пользователя:";
            // 
            // txbUserName
            // 
            this.txbUserName.Location = new System.Drawing.Point(7, 32);
            this.txbUserName.Name = "txbUserName";
            this.txbUserName.Size = new System.Drawing.Size(298, 26);
            this.txbUserName.TabIndex = 1;
            // 
            // txbComputers
            // 
            this.txbComputers.Location = new System.Drawing.Point(7, 84);
            this.txbComputers.Name = "txbComputers";
            this.txbComputers.Size = new System.Drawing.Size(298, 26);
            this.txbComputers.TabIndex = 3;
            // 
            // lblComputers
            // 
            this.lblComputers.AutoSize = true;
            this.lblComputers.Location = new System.Drawing.Point(3, 61);
            this.lblComputers.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblComputers.Name = "lblComputers";
            this.lblComputers.Size = new System.Drawing.Size(196, 20);
            this.lblComputers.TabIndex = 2;
            this.lblComputers.Text = "Компьютеры автовхода:";
            // 
            // lblRights
            // 
            this.lblRights.AutoSize = true;
            this.lblRights.Location = new System.Drawing.Point(3, 113);
            this.lblRights.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRights.Name = "lblRights";
            this.lblRights.Size = new System.Drawing.Size(174, 20);
            this.lblRights.TabIndex = 4;
            this.lblRights.Text = "Права пользователя:";
            // 
            // cbRightMaintainService
            // 
            this.cbRightMaintainService.AutoSize = true;
            this.cbRightMaintainService.Location = new System.Drawing.Point(20, 136);
            this.cbRightMaintainService.Name = "cbRightMaintainService";
            this.cbRightMaintainService.Size = new System.Drawing.Size(264, 24);
            this.cbRightMaintainService.TabIndex = 5;
            this.cbRightMaintainService.Text = "Инженер службы эксплуатации";
            this.cbRightMaintainService.UseVisualStyleBackColor = true;
            // 
            // cbRightEngineerOperator
            // 
            this.cbRightEngineerOperator.AutoSize = true;
            this.cbRightEngineerOperator.Location = new System.Drawing.Point(20, 166);
            this.cbRightEngineerOperator.Name = "cbRightEngineerOperator";
            this.cbRightEngineerOperator.Size = new System.Drawing.Size(172, 24);
            this.cbRightEngineerOperator.TabIndex = 6;
            this.cbRightEngineerOperator.Text = "Инженер-оператор";
            this.cbRightEngineerOperator.UseVisualStyleBackColor = true;
            // 
            // cbRightAdministrator
            // 
            this.cbRightAdministrator.AutoSize = true;
            this.cbRightAdministrator.Location = new System.Drawing.Point(20, 226);
            this.cbRightAdministrator.Name = "cbRightAdministrator";
            this.cbRightAdministrator.Size = new System.Drawing.Size(150, 24);
            this.cbRightAdministrator.TabIndex = 7;
            this.cbRightAdministrator.Text = "Администратор";
            this.cbRightAdministrator.UseVisualStyleBackColor = true;
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnChangePassword.Location = new System.Drawing.Point(7, 262);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(298, 37);
            this.btnChangePassword.TabIndex = 8;
            this.btnChangePassword.Text = "Изменить пароль";
            this.btnChangePassword.UseVisualStyleBackColor = true;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(7, 305);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 32);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "ОК";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(217, 305);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 32);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cbRightTechnologist
            // 
            this.cbRightTechnologist.AutoSize = true;
            this.cbRightTechnologist.Location = new System.Drawing.Point(20, 196);
            this.cbRightTechnologist.Name = "cbRightTechnologist";
            this.cbRightTechnologist.Size = new System.Drawing.Size(97, 24);
            this.cbRightTechnologist.TabIndex = 11;
            this.cbRightTechnologist.Text = "Технолог";
            this.cbRightTechnologist.UseVisualStyleBackColor = true;
            // 
            // DictionaryShiftStaffUserEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 349);
            this.Controls.Add(this.cbRightTechnologist);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnChangePassword);
            this.Controls.Add(this.cbRightAdministrator);
            this.Controls.Add(this.cbRightEngineerOperator);
            this.Controls.Add(this.cbRightMaintainService);
            this.Controls.Add(this.lblRights);
            this.Controls.Add(this.txbComputers);
            this.Controls.Add(this.lblComputers);
            this.Controls.Add(this.txbUserName);
            this.Controls.Add(this.lblUserName);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "DictionaryShiftStaffUserEdit";
            this.Text = "DictionaryShiftStaffUserEdit";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox txbUserName;
        private System.Windows.Forms.TextBox txbComputers;
        private System.Windows.Forms.Label lblComputers;
        private System.Windows.Forms.Label lblRights;
        private System.Windows.Forms.CheckBox cbRightMaintainService;
        private System.Windows.Forms.CheckBox cbRightEngineerOperator;
        private System.Windows.Forms.CheckBox cbRightAdministrator;
        private System.Windows.Forms.Button btnChangePassword;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbRightTechnologist;
    }
}