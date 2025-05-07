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
    public partial class DictionaryUsers : TimeTrackingDataForm
    {
        List<User> Users;
        public DictionaryUsers()
        {
            InitializeComponent();
            FillUserList();
        }

        private void FillUserList()
        {
            lvUsers.Items.Clear();
            Users = DB.GetUsers();
            foreach (User usr in Users)
            {
                var lvi = new ListViewItem();
                lvi.Text = usr.UserName;

                List<string> rights = new List<string>();
                if ((usr.Rights & UserRight.EngineerMaintainService) != 0) rights.Add("Инженер службы эксплуатации");
                if ((usr.Rights & UserRight.EngineerOperator) != 0) rights.Add("Инженер-оператор");
                if ((usr.Rights & UserRight.Administrator) != 0) rights.Add("Администратор");
                if ((usr.Rights & UserRight.Technologist) != 0) rights.Add("Технолог");

                lvi.SubItems.Add(string.Join(", ", rights.ToArray()));
                lvi.SubItems.Add(usr.AutoLogonComputerNames);
                lvi.Tag = usr;
                lvUsers.Items.Add(lvi);
            }
        }

        private User GetSelectedUser()
        {
            if (lvUsers.SelectedItems.Count > 0)
            {
                return lvUsers.SelectedItems[0].Tag as User;
            }
            return null;
        }

        private void tsmiAddUser_Click(object sender, EventArgs e)
        {
            var d = new DictionaryShiftStaffUserEdit(new User());
            if (d.ShowDialog() == DialogResult.OK)
            {
                DB.AddUser(d.EditUser);
                FillUserList();
            }
        }

        private void tsmiChangeUser_Click(object sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null) return;
            var d = new DictionaryShiftStaffUserEdit(user);
            if (d.ShowDialog() == DialogResult.OK)
            {
                DB.UpdateUser(d.EditUser);
                FillUserList();
            }
        }

        private void tsmiDeleteUser_Click(object sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null) return;
            if (MessageBox.Show("Вы действительно хотите удалить пользователя \"" + user.UserName + "\"?", "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DB.DeleteUser(user);
                FillUserList();
            }
        }

        private void lvUsers_DoubleClick(object sender, EventArgs e)
        {
            tsmiChangeUser.PerformClick();
        }


    }
}
