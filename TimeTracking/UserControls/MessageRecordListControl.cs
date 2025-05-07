using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeTrackingDB;

namespace TimeTracking.UserControls
{
    public partial class MessageRecordListControl : UserControl
    {
        DB Database;
        int EquipmentIdleID;
        int UserID;
        bool AllowToAddNew;
        public MessageRecordListControl(int equipmentIdleID, int userID, DB database, bool allowToAddNew)
        {
            InitializeComponent();
            EquipmentIdleID = equipmentIdleID;
            UserID = userID;
            Database = database;
            AllowToAddNew = allowToAddNew;
            btnAddNewRecord.Enabled = AllowToAddNew;
            Refresh();
        }
        public override void Refresh()
        {
            base.Refresh();
            var msglist = Database.AdditionalIdleRecords.Where(air => air.EquipmentIdleID == EquipmentIdleID).ToList();
            tblMessagesList.Controls.Clear();
            tblMessagesList.RowCount = 0;
            MessageRecordControl LastMsgRec = null;
            for (int i = 0; i < msglist.Count(); i++)
            {
                var mrControl = new MessageRecordControl(msglist[i].AdditionalIdleRecordID, Database);
                var ReadOnly = msglist[i].UserID != UserID || !AllowToAddNew;
                mrControl.ReadOnly = ReadOnly;
                mrControl.OnDeleteButton += MrControl_OnDeleteButton;

                tblMessagesList.Controls.Add(mrControl, 0, tblMessagesList.RowCount);
                tblMessagesList.RowCount++;
                if (!ReadOnly) LastMsgRec = mrControl;
            }
            if (LastMsgRec != null)
                LastMsgRec.Focus();
        }

        private void MrControl_OnDeleteButton(MessageRecordControl sender, AdditionalIdleRecord CurrentMessage)
        {

            var files = Database.AdditionalIdleRecordFiles.Where(airf => airf.AdditionalIdleRecordID == CurrentMessage.AdditionalIdleRecordID).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                try
                {
                    Database.AdditionalIdleRecordFiles.Remove(files[i]);
                }
                catch { }
            }
            try
            {
                Database.AdditionalIdleRecords.Remove(CurrentMessage);
            }
            catch { }
            tblMessagesList.Controls.Remove(sender);
            Database.ChangeTracker.DetectChanges();
            foreach (var entry in Database.ChangeTracker.Entries<AdditionalIdleRecordFile>())
            {
            }
            if (Database.Entry(CurrentMessage).State != System.Data.Entity.EntityState.Deleted)
                Database.Entry(CurrentMessage).State = System.Data.Entity.EntityState.Deleted;
            for (int i = 0; i < files.Count; i++)
            {
                if (Database.Entry(files[i]).State != System.Data.Entity.EntityState.Deleted)
                    Database.Entry(files[i]).State = System.Data.Entity.EntityState.Deleted;
            }
            Database.SaveChanges();
            Refresh();
        }

        private void btnAddNewRecord_Click(object sender, EventArgs e)
        {
            var newMsg = new AdditionalIdleRecord() { UserID = UserID, EquipmentIdleID = EquipmentIdleID, RecordDateTimeCreation = DateTime.Now };
            Database.AdditionalIdleRecords.Add(newMsg);
            Database.SaveChanges();
            Refresh();
        }
    }

    /*public class MessageRecord
    {
        public int MessageID;
        public DateTime CreatedAt;
        public int UserID;
        public string UserName;
        public string Message;
        public List<MessageRecordFiles> Files;
    }
    public class MessageRecordFiles
    {
        public int FileID;
        public string FileName;
    }*/
}
