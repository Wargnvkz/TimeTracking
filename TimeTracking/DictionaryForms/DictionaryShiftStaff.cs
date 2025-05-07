using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TimeTracking.DictionaryForms
{
    public partial class DictionaryShiftStaff : TimeTrackingDataForm
    {
        TimeTrackingDB.DB db;
        public DictionaryShiftStaff()
        {
            InitializeComponent();
            Start();
            ShowData();
        }

        public void Start()
        {
            db = new TimeTrackingDB.DB();
            foreach (DataGridViewColumn c in dgvOperators.Columns)
            {
                var font = new Font("Arial", 12F, GraphicsUnit.Point);
                c.DefaultCellStyle.Font = font;
                c.HeaderCell.Style.Font = font;
            }
        }

        public void Exit()
        {
            try
            {
                db?.SaveChanges();
                db?.Dispose();
            }
            catch { }
        }

        public void AddLine()
        {
            db?.Operators.Add(new TimeTrackingDB.Operator());
            db?.SaveChanges();
        }

        public void DeleteLine(int Key)
        {
            var toDelete = new TimeTrackingDB.Operator() { OperatorID = Key };
            db.Operators.Attach(toDelete);
            db?.Operators.Remove(toDelete);
            db?.SaveChanges();
        }

        public void ShowData()
        {
            dgvOperators.AutoGenerateColumns = false;
            if (db != null)
            {
                var data = db.Operators.ToList();
                if (data.Count == 0)
                {
                    AddLine();
                    data = db.Operators.ToList();
                }
                dgvOperators.DataSource = data;
                dgvOperators.AllowUserToAddRows = true;

            }
        }

        public void SaveData()
        {
            try
            {
                db?.SaveChanges();
            }
            catch { };
        }

        private void DictionaryShiftStaff_FormClosed(object sender, FormClosedEventArgs e)
        {
            Exit();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvOperators_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SaveData();
        }

        private void tsmiAddLine_Click(object sender, EventArgs e)
        {
            var op = new TimeTrackingDB.Operator();
            db.Operators.Add(op);
            db.SaveChanges();
            ShowData();
        }

        private void tsmiDeleteLine_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (dgvOperators.SelectedRows.Count > 0)
                {
                    var obj = dgvOperators.SelectedRows[0].DataBoundItem as TimeTrackingDB.Operator;
                    db.Operators.Remove(obj);
                    db?.SaveChanges();
                    ShowData();
                }
            }
        }

    }
}
