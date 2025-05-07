using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeTrackingDB;
using TimeTrackingLib;

namespace TimeTracking.ShiftsForms
{
    public partial class MaintainceServiceStaffTimeTracking : TimeTrackingDataForm
    {
        List<Supervisor> Supervisors;
        List<MaintainShiftEmployee> MainWorkers;
        List<MaintainShiftEmployee> AuxilaryWorkers;
        List<EmployeePosition> employeePositions;
        MaintainShiftSupervisor CurrentSupervisor;
        DB database;

        Shift SelectedShift
        {
            get
            {
                return new Shift(dtpShiftDate.Value.Date, false);
            }
        }
        public MaintainceServiceStaffTimeTracking()
        {
            InitializeComponent();
            database = new DB();
            employeePositions = database.EmployeePositions.ToList();
            FillPage();
        }

        private void FillPage()
        {
            var currentShift = new Shift();
            var selectedShift = SelectedShift;
            SetActionAllowed(selectedShift);
            UpdateSupervisors();
            GetSupervisors(selectedShift, out CurrentSupervisor);
            CreateColumns();
            UpdateData(selectedShift);
        }

        private void UpdateData(Shift selectedShift)
        {
            GetShiftMaintenaceEmployee(selectedShift, out MainWorkers, out AuxilaryWorkers);
            ShowEmployees();
        }
        private void UpdateData()
        {
            UpdateData(SelectedShift);
        }

        //TODO: Какого рода изменения можно вносить в список? Надо ли добавлять и удалять работников или только меня время? Как выбирать подсобных рабочих?
        private void CreateColumns()
        {
            dgvMainEmployee.AutoGenerateColumns = false;
            dgvMainEmployee.Columns.Clear();
            dgvMainEmployee.Columns.AddRange(GetColumns(employeePositions.FindAll(ep => !ep.IsAuxiliary)));

            dgvAuxilaryWorkers.AutoGenerateColumns = false;
            dgvAuxilaryWorkers.Columns.Clear();
            dgvAuxilaryWorkers.Columns.AddRange(GetColumns(employeePositions.FindAll(ep => ep.IsAuxiliary)));
        }

        private DataGridViewColumn[] GetColumns(List<EmployeePosition> positions)
        {
            //DataGridViewTextBoxColumn clFIO = new DataGridViewTextBoxColumn();
            var clFIO = new DataGridViewComboBoxColumn();
            DataGridViewTextBoxColumn clTime = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn clAdditionalTime = new DataGridViewTextBoxColumn();

            clFIO.HeaderText = "ФИО";
            clFIO.Name = "clFIO";
            clFIO.Width = 250;
            clFIO.DataPropertyName = "FIO";
            var positionIDs = positions.Select(p => p.EmployeePositionID);
            clFIO.DataSource = database.Employees.Where(e => positionIDs.Contains(e.EmployeePositionID)).ToList();
            clFIO.DisplayMember = "FIO";
            clFIO.ValueMember = "FIO";

            clTime.HeaderText = "Время";
            clTime.Name = "clTime";
            clTime.DataPropertyName = "WorkingHours";

            clAdditionalTime.HeaderText = "Сверхур.";
            clAdditionalTime.Name = "clAdditionalTime";
            clAdditionalTime.DataPropertyName = "AdditionalHours";

            return new DataGridViewColumn[] {
                clFIO,
                clTime,
                clAdditionalTime};
        }

        private void SetColumnsAccessebility()
        {

        }

        private void UpdateSupervisors()
        {
            Supervisors = GetSupervisors();
            Supervisors.Add(new Supervisor());
            Supervisors = Supervisors.OrderBy(e => e.FIO).ToList();
            cbSupervisor.DisplayMember = "FIO";
            cbSupervisor.ValueMember = "SupervisorID";
            cbSupervisor.DataSource = Supervisors;

        }

        private void tmShiftReload_Tick(object sender, EventArgs e)
        {

        }
        /*protected override void SetActionAllowed()
        {
            base.SetActionAllowed();
        }*/

        public List<Supervisor> GetSupervisors()
        {
            using (var db = new DB())
            {
                var supervisors = db.Supervisors.ToList();
                return supervisors;
            }
        }
        public List<Employee> GetEmployeeBySupervisor(Employee Supervisor)
        {
            using (var db = new DB())
            {
                return db.Employees.Where(e => e.Shift == Supervisor.Shift).ToList();
            }
        }


        private void SaveEmployees()
        {

            /*foreach (var mse in ShiftEmployees)
            {
                database.MaintainShiftEmployees.Add(mse);
            }
            database.SaveChanges();*/

        }

        private MaintainShiftEmployee Employee2MaintainShiftEmployee(Employee e)
        {
            var currentShift = new Shift();
            var mse = new MaintainShiftEmployee();
            mse.EmployeePositionID = e.EmployeePositionID;
            mse.FIO = e.FIO;
            mse.ShiftDate = currentShift.ShiftDate;
            mse.WorkingHours = e.WorkingHours;
            return mse;
        }

        private void ShowEmployees()
        {
            dgvMainEmployee.DataSource = MainWorkers;
            dgvAuxilaryWorkers.DataSource = AuxilaryWorkers;

            if (CurrentSupervisor != null)
            {
                var indCurrSup = Supervisors.FindIndex(s => CurrentSupervisor.FIO == s.FIO);
                cbSupervisor.SelectedIndex = indCurrSup;
            }

        }

        private void GetShiftMaintenaceEmployee(Shift shift, out List<MaintainShiftEmployee> MainWorkers, out List<MaintainShiftEmployee> AuxilaryWorkers)
        {
            database.RefreshContext();
            var ShiftEmployees = database.MaintainShiftEmployees.Where(mse => mse.ShiftDate == shift.ShiftDate).ToList();
            SeparateWorkers(ShiftEmployees, out MainWorkers, out AuxilaryWorkers);
            return;

        }

        private void ClearEmployees(Shift shift)
        {
            database.RefreshContext();
            var tmpEmployees = database.MaintainShiftEmployees.Where(mse => mse.ShiftDate == shift.ShiftDate);
            foreach (var emp in tmpEmployees)
            {
                database.MaintainShiftEmployees.Remove(emp);
            }

        }

        private void SeparateWorkers(List<MaintainShiftEmployee> ShiftEmployees, out List<MaintainShiftEmployee> MainWorkers, out List<MaintainShiftEmployee> AuxilaryWorkers)
        {
            var positions = database.EmployeePositions.ToDictionary(ep => ep.EmployeePositionID);
            MainWorkers = new List<MaintainShiftEmployee>();
            AuxilaryWorkers = new List<MaintainShiftEmployee>();
            foreach (var se in ShiftEmployees)
            {
                if (positions.ContainsKey(se.EmployeePositionID))
                {
                    var position = positions[se.EmployeePositionID];
                    if (position.IsAuxiliary)
                    {
                        AuxilaryWorkers.Add(se);
                    }
                    else
                    {
                        MainWorkers.Add(se);
                    }
                }
            }
        }


        private void MaintainceServiceStaffTimeTracking_FormClosed(object sender, FormClosedEventArgs e)
        {
            database.Dispose();
        }
        public void GetSupervisors(Shift selectedShift, out MaintainShiftSupervisor CurrentSupervisor)
        {
            CurrentSupervisor = database.MaintainShiftSupervisor.Where(mss => mss.ShiftDate == selectedShift.ShiftDate && mss.IsNightShift == selectedShift.IsNight).FirstOrDefault();
        }

        private void cbSupervisor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSupervisor.SelectedItem != null)
            {
                var currentSupervisor = Supervisors[cbSupervisor.SelectedIndex];
                if (String.IsNullOrEmpty(currentSupervisor.FIO)) return;
                var ss =database.MaintainShiftSupervisor.Where(ms => ms.ShiftDate == SelectedShift.ShiftDate && ms.IsNightShift == SelectedShift.IsNight).FirstOrDefault();
                if (ss!=null)
                {
                    ss.FIO = currentSupervisor.FIO;
                    database.SaveChanges();
                }
                else
                {
                    database.MaintainShiftSupervisor.Add(new MaintainShiftSupervisor() { ShiftDate = SelectedShift.ShiftDate, IsNightShift = SelectedShift.IsNight, FIO = currentSupervisor.FIO });
                    database.SaveChanges();
                }
            }
        }

        private void dgvShiftEmployee_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvAuxilaryWorkers_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dtpShiftDate_ValueChanged(object sender, EventArgs e)
        {
            FillPage();
        }

        private void rbNight_CheckedChanged(object sender, EventArgs e)
        {
            FillPage();

        }

        private void dgvShiftEmployee_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {
            database.SaveChanges();
        }

        private void dgvAuxilaryWorkers_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SaveData();
        }

        private void AddLine(Shift SelectedShift, bool IsAuxilary)
        {
            var position = employeePositions.FindAll(ep => ep.IsAuxiliary == IsAuxilary).FirstOrDefault();
            var newline = new MaintainShiftEmployee();
            newline.EmployeePositionID = position.EmployeePositionID;
            newline.ShiftDate = SelectedShift.ShiftDate;
            database.MaintainShiftEmployees.Add(newline);
            database.SaveChanges();
        }

        private void DeleteEmployee(MaintainShiftEmployee mse)
        {
            database.MaintainShiftEmployees.Remove(mse);
            database.SaveChanges();
        }

        private void tsmiAddMainEmployee_Click(object sender, EventArgs e)
        {
            AddLine(SelectedShift, false);
            UpdateData();
        }

        private void tsmiDeleteMainEmployee_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (dgvMainEmployee.SelectedRows.Count > 0)
                {
                    var obj = dgvMainEmployee.SelectedRows[0].DataBoundItem as MaintainShiftEmployee;
                    DeleteEmployee(obj);
                    UpdateData();
                }
            }
        }

        private void tsmiAddAuxilaryEmployee_Click(object sender, EventArgs e)
        {
            AddLine(SelectedShift, true);
            UpdateData();
        }

        private void tsmiDeleteAuxilaryEmployee_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (dgvMainEmployee.SelectedRows.Count > 0)
                {
                    var obj = dgvMainEmployee.SelectedRows[0].DataBoundItem as MaintainShiftEmployee;
                    DeleteEmployee(obj);
                    UpdateData();
                }
            }
        }
    }
}
