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
    public partial class ShiftsNotesForm : TimeTrackingDataForm
    {
        DB database;
        DataGridViewColumn IdleStartedColumn, IdleEndedColumn, IdleDurationColumn, ReasonsTypeColumn, ProfileColumn, NodeColumn, ElementColumn, MalfunctionTextColumn, MalfunctionTextNameColumn;
        DateTime CurrentShiftDate;
        bool CurrentIsNightShift;
        int CurrentEquipmentNumber;
        List<EquipmentIdle> Data;

        private void dtpShiftDate_ValueChanged(object sender, EventArgs e)
        {
            ShiftShowData();
        }


        private void dgvIdleReason_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void rbDay_CheckedChanged(object sender, EventArgs e)
        {
            ShiftShowData();
        }

        private void ShiftsDowntimeForm_Load(object sender, EventArgs e)
        {
            ShiftShowData();
        }

        public ShiftsNotesForm() : base()
        {
            InitializeComponent();
            database = new DB();
            Prepare();
            PrepareEquipmentButtons();
        }

        private void Prepare()
        {
            IdleStartedColumn = new DataGridViewTextBoxColumn();
            IdleStartedColumn.HeaderText = "Начало простоя";
            IdleStartedColumn.DataPropertyName = "IdleStart";
            IdleStartedColumn.Width = 120;
            IdleStartedColumn.DefaultCellStyle.Format = "HH:mm:ss";
            IdleStartedColumn.ReadOnly = true;
            dgvIdleReason.Columns.Add(IdleStartedColumn);

            IdleEndedColumn = new DataGridViewTextBoxColumn();
            IdleEndedColumn.HeaderText = "Конец простоя";
            IdleEndedColumn.DataPropertyName = "IdleEnd";
            IdleEndedColumn.Width = 120;
            IdleEndedColumn.DefaultCellStyle.Format = "HH:mm:ss";
            IdleEndedColumn.ReadOnly = true;
            dgvIdleReason.Columns.Add(IdleEndedColumn);

            IdleDurationColumn = new DataGridViewTextBoxColumn();
            IdleDurationColumn.HeaderText = "Время простоя";
            IdleDurationColumn.DataPropertyName = "IdleDuration";
            IdleDurationColumn.Width = 120;
            IdleDurationColumn.ReadOnly = true;
            //IdleDurationColumn.DefaultCellStyle.Format = "hh:mm:ss";
            dgvIdleReason.Columns.Add(IdleDurationColumn);

            ReasonsTypeColumn = new DataGridViewCustomComboBoxColumn<MalfunctionReasonType>(
                "Вид простоя",
                "MalfunctionReasonTypeID",
                "MalfunctionReasonTypeName",
                "MalfunctionReasonTypeID",
                null,
                database.MalfunctionReasonTypes,
                dgvIdleReason,
                null
                );
            ReasonsTypeColumn.Width = 200;
            ReasonsTypeColumn.ReadOnly = true;

            ProfileColumn = new DataGridViewCustomComboBoxColumn<MalfunctionReasonProfile>(
                "Профиль",
                "MalfunctionReasonProfileID",
                "MalfunctionReasonProfileName",
                "MalfunctionReasonProfileID",
                new string[] { "MalfunctionReasonTypeID" },
                database.MalfunctionReasonProfiles,
                dgvIdleReason,
                new DataGridViewColumn[] { ReasonsTypeColumn }
            );
            ProfileColumn.Width = 200;
            ProfileColumn.ReadOnly = true;

            NodeColumn = new DataGridViewCustomComboBoxColumn<MalfunctionReasonNode>(
                "Узел",
                "MalfunctionReasonNodeID",
                "MalfunctionReasonNodeName",
                "MalfunctionReasonNodeID",
                new string[] { "MalfunctionReasonProfileID" },
                database.MalfunctionReasonNodes,
                dgvIdleReason,
                new DataGridViewColumn[] { ProfileColumn }
            );
            NodeColumn.Width = 200;
            NodeColumn.ReadOnly = true;

            ElementColumn = new DataGridViewCustomComboBoxColumn<MalfunctionReasonElement>(
                "Элемент",
                "MalfunctionReasonElementID",
                "MalfunctionReasonElementName",
                "MalfunctionReasonElementID",
                new string[] { "MalfunctionReasonNodeID" },
                database.MalfunctionReasonElements,
                dgvIdleReason,
                new DataGridViewColumn[] { NodeColumn }
            );
            ElementColumn.Width = 200;
            ElementColumn.ReadOnly = true;

            MalfunctionTextColumn = new DataGridViewCustomComboBoxColumn<MalfunctionReasonMalfunctionText>(
                "Неисправность",
                "MalfunctionReasonMalfunctionTextID",
                "MalfunctionReasonMalfunctionTextName",
                "MalfunctionReasonMalfunctionTextID",
                new string[] { "MalfunctionReasonTypeID", "MalfunctionReasonProfileID", "MalfunctionReasonNodeID", "MalfunctionReasonElementID" },
                database.MalfunctionReasonMalfunctionTexts,
                dgvIdleReason,
                new DataGridViewColumn[] { ReasonsTypeColumn, ProfileColumn, NodeColumn, ElementColumn }
            );
            MalfunctionTextColumn.Width = 200;
            MalfunctionTextColumn.ReadOnly = true;

            MalfunctionTextNameColumn = new DataGridViewTextBoxColumn();
            MalfunctionTextNameColumn.HeaderText = "Замечания";
            MalfunctionTextNameColumn.DataPropertyName = "MalfunctionReasonMalfunctionTextComment";
            MalfunctionTextNameColumn.Width = 300;
            dgvIdleReason.Columns.Add(MalfunctionTextNameColumn);

            var fontCell = new Font("Arial", 9);
            var fontHeader = new Font("Arial", 10);

            dgvIdleReason.DefaultCellStyle.Font = fontCell;
            dgvIdleReason.ColumnHeadersDefaultCellStyle.Font = fontHeader;

            dgvIdleReason.CellValueChanged += DgvIdleReason_CellValueChanged;
            dgvIdleReason.AutoGenerateColumns = false;

        }

        private void dgvIdleReason_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            //TODO: Сделать проверку разделенных записей, чтобы время начало и конца не выходило за пределы имеющихся в записи
            //TODO: скопировать время в разделенной записи

            database.SaveChanges();

            dgvIdleReason.Update();
            dgvIdleReason.Refresh();

        }

        private void dgvIdleReason_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (actionEditAlowed == ActionEditAllowed.NoEdit) { e.Cancel = true; return; }
            if (actionEditAlowed == ActionEditAllowed.UserEdit)
            {
                if (e.ColumnIndex == 0)
                {
                    var row = Data[e.RowIndex];
                    if (!row.IsRecordDivided) { e.Cancel = true; return; }
                    if (!row.DivisionParentEquipmentIdleID.HasValue) { e.Cancel = true; return; }
                }
                if (e.ColumnIndex == 1)
                {
                    var row = Data[e.RowIndex];
                    if (!row.IsRecordDivided) { e.Cancel = true; return; }
                    if (!row.DivisionChildEquipmentIdleID.HasValue) { e.Cancel = true; return; }

                }
            }
        }
        

        bool ShiftChanged = false;
        private void tmChangeShift_Tick(object sender, EventArgs e)
        {
            var d8 = (DateTime.Now.TimeOfDay - Shift._8).TotalSeconds;
            var d20 = (DateTime.Now.TimeOfDay - Shift._20).TotalSeconds;
            if (d8 > 0 && d8 < 10 || d20 > 0 && d20 < 10)
            {
                if (!ShiftChanged && !this.Focused)
                {
                    if (!(dgvIdleReason.CurrentCell?.IsInEditMode ?? false))
                    {
                        var current = new Shift();
                        var pageShift = new Shift(dtpShiftDate.Value, rbNight.Checked);
                        if (current.PreviousShift() == pageShift)
                        {
                            dtpShiftDate.Value = current.ShiftDate;
                            rbNight.Checked = current.IsNight;
                            ShiftShowData();
                        }
                        ShiftChanged = true;
                    }
                }
            }
            else
            {
                ShiftChanged = false;
            }

        }

        private void btnGetMaintenance_Click(object sender, EventArgs e)
        {
            var visShift = new Shift(dtpShiftDate.Value, rbNight.Checked);
            var prvShift = visShift.PreviousShift();
            var orders = TimeTrackingSAP.MaintenanceOrders.GetOrders(prvShift.ShiftStartsAt(), visShift.ShiftEndsAt(),false);
            var OrdersParts = new List<TimeTrackingSAP.MaintenanceOrder>();
            foreach (var ord in orders)
            {
                if (ord.Equipment > 0)
                    OrdersParts.AddRange(ord.DivideOrderForShifts());
            }

            using (var db = new DB())
            {
                foreach (var order in OrdersParts)
                {
                    db.InsertSAPRecord(order.StartDateTime, order.EndDateTime, order.Equipment, order.OrderID, order.Action, log: Log.ApplicationInstance);
                }
            }
            ShiftShowData();
        }

        private void dgvIdleReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                dgvIdleReason.EndEdit();
            }
        }

        public void PrepareEquipmentButtons()
        {
            var MachinesList = new List<int>() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            for (int i = 0; i < MachinesList.Count; i++)
            {
                var machineN = MachinesList[i];
                var rbMachineN = new RadioButton();
                rbMachineN.Top = 10;
                rbMachineN.Left = i * 70 + 10;
                rbMachineN.Width = 70;
                rbMachineN.Text = "PL" + machineN.ToString("D2");
                rbMachineN.CheckedChanged += RbMachineN_CheckedChanged;
                rbMachineN.Tag = machineN;
                rbMachineN.Visible = true;
                if (i == 0)
                {
                    rbMachineN.Checked = true;
                }
                pnlMachines.Controls.Add(rbMachineN);
            }

        }

        private void RbMachineN_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Tag != null && rb.Checked)
                {
                    var machine = (int)rb.Tag;
                    CurrentEquipmentNumber = machine;
                    ShiftShowData();
                }
            }
            //throw new NotImplementedException();
        }

        private void DgvIdleReason_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ShiftsDowntimeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            database.Dispose();
        }

        private void ShiftShowData()
        {
            CurrentShiftDate = dtpShiftDate.Value.Date;
            CurrentIsNightShift = rbNight.Checked;
            SetActionAllowed(new Shift(CurrentShiftDate, CurrentIsNightShift));
            Data = GetEquipmentIdles();
            if (Data.Count == 0)
            {
                Data = GetEquipmentIdles();
            }
            //AddNewLine();//(false);
            RefreshDataView();
            //dgvIdleReason.DataSource = Data;
        }

        private void RefreshDataView()
        {
            SetAutoRowHeight();
            dgvIdleReason.DataSource = Data;
            SetAutoRowHeight();

        }
        private void SetAutoRowHeight()
        {
            var rowHeight = 70;
            dgvIdleReason.RowTemplate.Height = rowHeight;
            dgvIdleReason.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            for (int i = 0; i < dgvIdleReason.Columns.Count; i++)
            {
                dgvIdleReason.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
            for (int i = 0; i < dgvIdleReason.Rows.Count; i++)
            {
                for (int j = 0; j < dgvIdleReason.Rows[i].Cells.Count; j++)
                {
                    dgvIdleReason.Rows[i].Cells[j].Style.WrapMode = DataGridViewTriState.True;
                }
                dgvIdleReason.AutoResizeRow(i, DataGridViewAutoSizeRowMode.AllCells);
                dgvIdleReason.Rows[i].Height = rowHeight;
            }
        }
        private List<EquipmentIdle> GetEquipmentIdles()
        {
            database.RefreshContext();
            var data = database.EquipmentIdles.Where(ei => ei.IsNightShift == CurrentIsNightShift && ei.ShiftStart == CurrentShiftDate && ei.EquipmentNumber == CurrentEquipmentNumber && ei.MalfunctionReasonTypeID == 1).ToList();
            data = data.OrderBy(d => d.IdleStart.HasValue ? d.IdleStart : d.IdleEnd).ToList();
            return data;

        }


    }

}
