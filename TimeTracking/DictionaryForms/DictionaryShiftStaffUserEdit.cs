using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeTrackingDB;

namespace TimeTracking.DictionaryForms
{
    public partial class DictionaryShiftStaffUserEdit : Form
    {
        private User _User;
        public User EditUser
        {
            get
            {
                GetData();
                return _User;
            }
            set
            {
                _User = value;
                FillData();
            }
        }
        public DictionaryShiftStaffUserEdit(User user)
        {
            InitializeComponent();
            if (user == null)
            {
                EditUser = new User();
            }
            else
            {
                EditUser = user;
            }
        }

        public void FillData()
        {
            txbUserName.Text = _User.UserName;
            txbComputers.Text = _User.AutoLogonComputerNames;
            cbRightMaintainService.Checked = (_User.Rights & UserRight.EngineerMaintainService) != 0;
            cbRightEngineerOperator.Checked = (_User.Rights & UserRight.EngineerOperator) != 0;
            cbRightAdministrator.Checked = (_User.Rights & UserRight.Administrator) != 0;
            cbRightTechnologist.Checked = (_User.Rights & UserRight.Technologist) != 0;
        }
        public void GetData()
        {
            _User.UserName = txbUserName.Text;
            _User.AutoLogonComputerNames = txbComputers.Text;
            _User.Rights = (cbRightMaintainService.Checked ? UserRight.EngineerMaintainService : 0) |
                (cbRightEngineerOperator.Checked ? UserRight.EngineerOperator : 0) |
                (cbRightAdministrator.Checked ? UserRight.Administrator : 0) |
                (cbRightTechnologist.Checked ? UserRight.Technologist : 0);
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            var password = "";
            if (Prompt.ShowDialog(ref password, "Новый пароль:", "Изменение пароля", true))
            {
                _User.PasswordHash = DB.Hash(password);
            }

        }
    }
}
