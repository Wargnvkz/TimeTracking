using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeTracking.UserControls;
using TimeTrackingDB;

namespace TimeTracking
{
    public partial class AdditionalRecordsForm : Form
    {
        private DB Database;
        public AdditionalRecordsForm(int equipmentIdleID, int userID, DB database, bool AllowToAddNew)
        {
            InitializeComponent();
            Database = database;
            var msg = new MessageRecordListControl(equipmentIdleID, userID, database, AllowToAddNew);
            pnlMsgList.Controls.Add(msg);
            msg.Location = new Point(0, 0);
            msg.Size = new Size(pnlMsgList.ClientSize.Width, pnlMsgList.ClientSize.Height);
            msg.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            SaveDB();
            Close();
        }

        private void AdditionalRecordsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveDB();
        }
        private void SaveDB()
        {
            Database.ChangeTracker.DetectChanges();
            foreach (var entry in Database.ChangeTracker.Entries())
            {
            }
            Database.SaveChanges();
        }
    }
}
