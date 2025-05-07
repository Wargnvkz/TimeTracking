using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeTrackingDB;

namespace TimeTracking.DictionaryForms
{
    public partial class DictionaryMaintananceServiceStaff : TimeTrackingDataForm
    {
        TimeTrackingDB.DB db;
        List<Employee> Shift0data;
        List<Employee> Shift1data;
        List<Employee> Shift2data;
        List<Supervisor> Supervisors;
        List<EmployeePosition> EmployeePositions;
        List<SupervisorMaitenanceShift> SupervisorMaitenanceShifts=
            new List<SupervisorMaitenanceShift>() { 
                new SupervisorMaitenanceShift() { MaintenanceShift=0, MaintenanceShiftName="-" },
                new SupervisorMaitenanceShift() { MaintenanceShift=1, MaintenanceShiftName="1 смена" },
                new SupervisorMaitenanceShift() { MaintenanceShift=2, MaintenanceShiftName="2 смена" }
            };
        public DictionaryMaintananceServiceStaff()
        {
            InitializeComponent();
            ConnectDB();
            ReloadData();
        }

        private void ConnectDB()
        {
            db = new DB();
        }

        private void ReloadData()
        {
            dgvShift1.DataSource = null;

            foreach (DataGridViewColumn c in dgvShift1.Columns)
            {
                var font = new Font("Arial", 12F, GraphicsUnit.Point);
                c.DefaultCellStyle.Font = font;
                c.HeaderCell.Style.Font = font;
            }

            EmployeePositions = db.EmployeePositions.ToList();
            EmployeePositions.Add(new EmployeePosition() { EmployeePositionID = 0, EmployeePositionName = "--- Выберите должность ---" });
            EmployeePositionShift1.DataSource = EmployeePositions;
            EmployeePositionShift1.DisplayMember = "EmployeePositionName";
            EmployeePositionShift1.ValueMember = "EmployeePositionID";
            dgvShift1.Columns[0].DataPropertyName = "EmployeePositionID";

            Shift1data = db.Employees.Where(e => e.Shift == 1).ToList();
            dgvShift1.AutoGenerateColumns = false;
            dgvShift1.DataSource = Shift1data;

            dgvShift2.DataSource = null;

            foreach (DataGridViewColumn c in dgvShift2.Columns)
            {
                var font = new Font("Arial", 12F, GraphicsUnit.Point);
                c.DefaultCellStyle.Font = font;
                c.HeaderCell.Style.Font = font;
            }

            EmployeePositions = db.EmployeePositions.ToList();
            EmployeePositions.Add(new EmployeePosition() { EmployeePositionID = 0, EmployeePositionName = "--- Выберите должность ---" });
            EmployeePositionShift2.DataSource = EmployeePositions;
            EmployeePositionShift2.DisplayMember = "EmployeePositionName";
            EmployeePositionShift2.ValueMember = "EmployeePositionID";
            dgvShift2.Columns[0].DataPropertyName = "EmployeePositionID";

            Shift2data = db.Employees.Where(e => e.Shift == 2).ToList();
            dgvShift2.AutoGenerateColumns = false;
            dgvShift2.DataSource = Shift2data;


            dgvShift0.DataSource = null;

            foreach (DataGridViewColumn c in dgvShift0.Columns)
            {
                var font = new Font("Arial", 12F, GraphicsUnit.Point);
                c.DefaultCellStyle.Font = font;
                c.HeaderCell.Style.Font = font;
            }

            EmployeePositions = db.EmployeePositions.ToList();
            EmployeePositions.Add(new EmployeePosition() { EmployeePositionID = 0, EmployeePositionName = "--- Выберите должность ---" });
            EmployeePositionShift0.DataSource = EmployeePositions;
            EmployeePositionShift0.DisplayMember = "EmployeePositionName";
            EmployeePositionShift0.ValueMember = "EmployeePositionID";
            dgvShift0.Columns[0].DataPropertyName = "EmployeePositionID";

            Shift0data = db.Employees.Where(e => e.Shift == 0).ToList();
            dgvShift0.AutoGenerateColumns = false;
            dgvShift0.DataSource = Shift0data;

            Supervisors = db.Supervisors.ToList();
            dgvSupervisors.AutoGenerateColumns = false;
            dgvSupervisors.DataSource = Supervisors;
            dgvSupervisors.Columns[0].DataPropertyName = "FIO";

            colMaintenanceShift.DataPropertyName = "MaintenanceShift";
            colMaintenanceShift.DataSource = SupervisorMaitenanceShifts;
            colMaintenanceShift.DisplayMember = "MaintenanceShiftName";
            colMaintenanceShift.ValueMember = "MaintenanceShift";
        }


        private void DictionaryMaintananceServiceStaff_FormClosed(object sender, FormClosedEventArgs e)
        {
            db?.Dispose();
        }

        private void dgvShift1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SaveData();
        }

        private void dgvShift1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void tsmiAddLineShift1_Click(object sender, EventArgs e)
        {
            db.Employees.Add(new Employee() { Shift = 1, EmployeePositionID = 0 });
            db?.SaveChanges();
            ReloadData();
        }

        private void tsmiDeleteLineShift1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (dgvShift1.SelectedRows.Count > 0)
                {
                    var obj = dgvShift1.SelectedRows[0].DataBoundItem as Employee;
                    db.Employees.Remove(obj);
                    db?.SaveChanges();
                    ReloadData();
                }
            }
        }

        private void dgvShift2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SaveData();
        }

        private void tsmiAddLineShift2_Click(object sender, EventArgs e)
        {
            db.Employees.Add(new Employee() { Shift = 2, EmployeePositionID = 0 });
            db?.SaveChanges();
            ReloadData();
        }

        private void tsmiDeleteLineShift2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (dgvShift2.SelectedRows.Count > 0)
                {
                    var obj = dgvShift2.SelectedRows[0].DataBoundItem as Employee;
                    db.Employees.Remove(obj);
                    db?.SaveChanges();
                    ReloadData();
                }
            }
        }

        private void dgvShift2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvShift0_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SaveData();
        }

        private void tsmiAddLineShift0_Click(object sender, EventArgs e)
        {
            db.Employees.Add(new Employee() { Shift = 0, EmployeePositionID = 0 });
            db?.SaveChanges();
            ReloadData();
        }

        private void tsmiDeleteLineShift0_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (dgvShift0.SelectedRows.Count > 0)
                {
                    var obj = dgvShift0.SelectedRows[0].DataBoundItem as Employee;
                    db.Employees.Remove(obj);
                    db?.SaveChanges();
                    ReloadData();
                }
            }

        }
        private void cmsEmployeeShift0_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void dgvShift0_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }


        private void tsmiAddSupervisor_Click(object sender, EventArgs e)
        {
            db.Supervisors.Add(new Supervisor() { SupervisorID=0, MaintenanceShift=0});
            db?.SaveChanges();
            ReloadData();
        }

        private void tsmiDeleteSupervisor_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (dgvSupervisors.SelectedRows.Count > 0)
                {
                    var obj = dgvSupervisors.SelectedRows[0].DataBoundItem as Supervisor;
                    db.Supervisors.Remove(obj);
                    db?.SaveChanges();
                    ReloadData();
                }
            }
        }

        private void dgvSupervisors_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvSupervisors_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {
            try
            {
                db?.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }
    internal class SupervisorMaitenanceShift
    {
        public int MaintenanceShift { get; set; }
        public string MaintenanceShiftName { get; set; }
    }

}
