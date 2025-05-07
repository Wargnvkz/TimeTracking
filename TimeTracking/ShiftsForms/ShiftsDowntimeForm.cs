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
    public partial class ShiftsDowntimeForm : TimeTrackingDataForm
    {
        DB database;
        DataGridViewColumn IdleStartedColumn, IdleEndedColumn, IdleDurationColumn, ReasonsTypeColumn, ProfileColumn, NodeColumn, ElementColumn, MalfunctionTextColumn, MalfunctionTextNameColumn;
        DateTime CurrentShiftDate;
        bool CurrentIsNightShift;
        int CurrentEquipmentNumber;
        List<EquipmentIdle> Data;

        List<RadioButton> MachineButtons;

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

        public ShiftsDowntimeForm() : base()
        {
            InitializeComponent();
            database = new DB();
            Prepare();
            PrepareEquipmentButtons();
            this.Focus();
        }

        private void Prepare()
        {
            IdleStartedColumn = new DataGridViewTextBoxColumn();
            IdleStartedColumn.HeaderText = "Начало простоя";
            IdleStartedColumn.DataPropertyName = "IdleStart";
            IdleStartedColumn.Width = 120;
            IdleStartedColumn.DefaultCellStyle.Format = "HH:mm:ss";
            dgvIdleReason.Columns.Add(IdleStartedColumn);

            IdleEndedColumn = new DataGridViewTextBoxColumn();
            IdleEndedColumn.HeaderText = "Конец простоя";
            IdleEndedColumn.DataPropertyName = "IdleEnd";
            IdleEndedColumn.Width = 120;
            IdleEndedColumn.DefaultCellStyle.Format = "HH:mm:ss";
            dgvIdleReason.Columns.Add(IdleEndedColumn);

            IdleDurationColumn = new DataGridViewTextBoxColumn();
            IdleDurationColumn.HeaderText = "Время простоя";
            IdleDurationColumn.DataPropertyName = "IdleDuration";
            IdleDurationColumn.Width = 120;
            IdleDurationColumn.DefaultCellStyle.Format = "hh\\:mm\\:ss";
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

            MalfunctionTextNameColumn = new DataGridViewTextBoxColumn();
            MalfunctionTextNameColumn.HeaderText = "Замечания/Примечания";
            MalfunctionTextNameColumn.DataPropertyName = "MalfunctionReasonMalfunctionTextComment";
            MalfunctionTextNameColumn.Width = 300;
            dgvIdleReason.Columns.Add(MalfunctionTextNameColumn);

            var AdditionalRecordButton = new DataGridViewButtonColumn();
            AdditionalRecordButton.HeaderText = "Дополнения внешних служб";
            AdditionalRecordButton.Text = "...";
            AdditionalRecordButton.Width = 150;
            AdditionalRecordButton.UseColumnTextForButtonValue = true;
            dgvIdleReason.Columns.Add(AdditionalRecordButton);


            var fontCell = new Font("Arial", 9);
            var fontHeader = new Font("Arial", 10);

            dgvIdleReason.DefaultCellStyle.Font = fontCell;
            dgvIdleReason.ColumnHeadersDefaultCellStyle.Font = fontHeader;

            dgvIdleReason.AutoGenerateColumns = false;



        }

        private void dgvIdleReason_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            //TODO: Сделать проверку разделенных записей, чтобы время начало и конца не выходило за пределы имеющихся в записи
            //TODO: скопировать время в разделенной записи

            var rec = Data[e.RowIndex];
            if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
            {
                if (e.ColumnIndex == 0)
                {
                    if (rec.DivisionParentEquipmentIdleID != null && rec.IdleStart.HasValue)
                    {
                        if (MessageBox.Show("Вы уверены, что хотите установить время в: " + rec.IdleStart.ToString(), "Выбор времени", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            Log.ApplicationInstance.Add($"Машина {rec.EquipmentNumber}. Запись {rec.ShiftStart}{(rec.IsNightShift ? "Н" : "Д")} с {rec.IdleStart:HH\\:mm\\:ss} по {rec.IdleEnd:HH\\:mm\\:ss}. Установка времени начала в {rec.IdleStart:HH\\:mm\\:ss}");
                            var parentRec = Data.Find(d => d.EquipmentIdleID == rec.DivisionParentEquipmentIdleID);
                            parentRec.IdleEnd = rec.IdleStart;
                            parentRec.DivisionChildEquipmentIdleID = null;
                            rec.DivisionParentEquipmentIdleID = null;
                        }
                    }
                    if (rec.DivisionParentEquipmentIdleID == null)
                    {
                        if (rec.IdleStart.HasValue && (rec.IdleStart.Value - rec.ShiftStart.Value).TotalHours > 20)
                        {
                            rec.IdleStart = rec.ShiftStart.Value.Date.Add(rec.IdleStart.Value.TimeOfDay);
                            if (rec.IdleStart.Value.TimeOfDay < new TimeSpan(8, 0, 0))
                            {
                                rec.IdleStart = rec.IdleStart.Value.AddDays(1);
                            }
                        }
                    }
                }
                if (e.ColumnIndex == 1)
                {
                    if (rec.DivisionChildEquipmentIdleID != null && rec.IdleEnd.HasValue)
                    {
                        if (MessageBox.Show("Вы уверены, что хотите установить время в: " + rec.IdleEnd.ToString(), "Выбор времени", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            Log.ApplicationInstance.Add($"Машина {rec.EquipmentNumber}. Запись {rec.ShiftStart}{(rec.IsNightShift ? "Н" : "Д")} с {rec.IdleStart:HH\\:mm\\:ss} по {rec.IdleEnd:HH\\:mm\\:ss}. Установка времени окончания в {rec.IdleStart:HH\\:mm\\:ss}");
                            var childRec = Data.Find(d => d.EquipmentIdleID == rec.DivisionChildEquipmentIdleID);
                            childRec.IdleStart = rec.IdleEnd;
                            rec.DivisionChildEquipmentIdleID = null;
                            childRec.DivisionParentEquipmentIdleID = null;
                        }
                    }
                    if (rec.DivisionChildEquipmentIdleID == null)
                    {
                        if (rec.IdleEnd.HasValue && (rec.IdleEnd.Value - rec.ShiftStart.Value).TotalHours > 32)
                        {
                            rec.IdleEnd = rec.ShiftStart.Value.Date.Add(rec.IdleEnd.Value.TimeOfDay);
                            if (rec.IdleEnd.Value.TimeOfDay < new TimeSpan(8, 0, 0))
                            {
                                rec.IdleEnd = rec.IdleEnd.Value.AddDays(1);
                            }

                        }
                    }
                }
            }
            Log.ApplicationInstance.Add($"Окончание редактирования. Машина {rec.EquipmentNumber}. Запись {rec.ShiftStart}{(rec.IsNightShift ? "Н" : "Д")} с {rec.IdleStart:HH\\:mm\\:ss} по {rec.IdleEnd:HH\\:mm\\:ss}. Поле №{e.ColumnIndex}");


            database.SaveChanges();

            dgvIdleReason.Update();
            dgvIdleReason.Refresh();
        }

        private void dgvIdleReason_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            RefreshDatabaseWithAttachCurrentToContext();
            if (actionEditAlowed == ActionEditAllowed.NoEdit) { e.Cancel = true; return; }
            var rec = Data[e.RowIndex];
            Log.ApplicationInstance.Add($"Начало редактирования. Машина {rec.EquipmentNumber}. Запись {rec.ShiftStart}{(rec.IsNightShift ? "Н" : "Д")} с {rec.IdleStart:HH\\:mm\\:ss} по {rec.IdleEnd:HH\\:mm\\:ss}. Поле №{e.ColumnIndex}");
            if (actionEditAlowed == ActionEditAllowed.UserEdit)
            {
                if (e.ColumnIndex == 0)
                {
                    var row = Data[e.RowIndex];
                    //if (!row.IsRecordDivided) { e.Cancel = true; return; }
                    if (row.IdleStart.HasValue || !row.DivisionParentEquipmentIdleID.HasValue) { e.Cancel = true; return; }
                }
                if (e.ColumnIndex == 1)
                {
                    var row = Data[e.RowIndex];
                    //if (!row.IsRecordDivided) { e.Cancel = true; return; }
                    if (row.IdleEnd.HasValue || !row.DivisionChildEquipmentIdleID.HasValue) { e.Cancel = true; return; }

                }
            }
        }

        private void btnStartIdle_Click(object sender, EventArgs e)
        {
            var dtf = new DateTimeGetForm();
            if (dtf.ShowDialog() == DialogResult.OK)
            {
                using (var db = new DB())
                {
                    db.SetMachineStatus(CurrentEquipmentNumber, DB.MachineStatus.Idle, Time: dtf.DateTimeGot, log: Log.ApplicationInstance);
                }
            }

            ShiftShowData();
        }
        private void btnStopIdle_Click(object sender, EventArgs e)
        {
            var dtf = new DateTimeGetForm();
            if (dtf.ShowDialog() == DialogResult.OK)
            {
                using (var db = new DB())
                {
                    db.SetMachineStatus(CurrentEquipmentNumber, DB.MachineStatus.Working, Time: dtf.DateTimeGot, log: Log.ApplicationInstance);
                }
            }
        }

        private void dgvIdleReason_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 && e.RowIndex >= 0)
            {
                var rec = Data[e.RowIndex];
                if (rec.IsAbleToDivide)
                {
                    if (MessageBox.Show("Вы уверены, что хотете разделить запись?", "Деление записи", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        dgvIdleReason.CancelEdit();
                        try
                        {
                            RefreshDatabaseWithAttachCurrentToContext();
                            database.EquipmentIdleDivideRecord(rec, log: Log.ApplicationInstance);
                        }
                        catch (Exception ex)
                        {
                        }
                        ShiftShowData();
                    }
                }
                else
                {
                    MessageBox.Show("Запись нельзя разделить, пока не будут внесены времена начала и конца простоя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        bool ShiftChanged = false;
        DateTime LastDataShown = DateTime.MinValue;
        int RefreshTimeout = 30;
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
            var now = DateTime.Now;
            var dtime = (now - LastDataShown).TotalSeconds;
            if (dtime > RefreshTimeout)
            {
                if (!(dgvIdleReason.CurrentCell?.IsInEditMode ?? false))
                {

                    ShiftShowData();

                }
                LastDataShown = now;
            }
            else
            {
                lblRefreshAfterCaption.Text = $"Обновление через: {(RefreshTimeout - dtime):F0} сек";
            }


        }

        private void btnGetMaintenance_Click(object sender, EventArgs e)
        {
            var visShift = new Shift(dtpShiftDate.Value, rbNight.Checked);
            var prvShift = visShift.PreviousShift();
            var orders = TimeTrackingSAP.MaintenanceOrders.GetOrders(prvShift.ShiftStartsAt(), visShift.ShiftEndsAt(), false);
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



        public void PrepareEquipmentButtons()
        {
            MachineButtons = new List<RadioButton>();
            List<int> MachinesList;
            try
            {
                var textlist = System.Configuration.ConfigurationManager.AppSettings["MachinesList"];
                MachinesList = textlist.Split(',').Select(el =>
                  {
                      if (int.TryParse(el.Trim(), out int result))
                          return (int?)result;
                      else
                          return null;
                  }
                ).Where(el => el != null).Select(e => e.Value).ToList();
            }
            catch
            {
                MachinesList = new List<int>() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            }
            for (int i = 0; i < MachinesList.Count; i++)
            {
                var machineN = MachinesList[i];
                var rbMachineN = new RadioButton();
                rbMachineN.Top = 10;
                rbMachineN.Left = i * 60 + 10;
                rbMachineN.Width = 50;
                //rbMachineN.Height = 40;
                rbMachineN.Text = "PL" + machineN.ToString("D2");
                rbMachineN.CheckedChanged += RbMachineN_CheckedChanged;
                rbMachineN.Tag = machineN;
                rbMachineN.Visible = true;
                rbMachineN.TextAlign = ContentAlignment.MiddleCenter;
                //rbMachineN.CheckAlign = ContentAlignment.TopCenter;
                if (i == 0)
                {
                    rbMachineN.Checked = true;
                }
                MachineButtons.Add(rbMachineN);
                pnlMachines.Controls.Add(rbMachineN);
            }

        }

        private void dgvIdleReason_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvIdleReason.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                var row = dgvIdleReason.Rows[e.RowIndex];
                var buttonCell = row.Cells[e.ColumnIndex] as DataGridViewButtonCell;
                if (buttonCell != null && !buttonCell.ReadOnly)
                {
                    var dataRecord = Data[e.RowIndex];
                    var frm = new AdditionalRecordsForm(dataRecord.EquipmentIdleID, CurrentData.CurrentUser.UserID, database, actionAddCommentAllowed == ActionAddAdditionalCommentAllowed.Allowed);
                    frm.ShowDialog();
                    database.RefreshContext();
                }
            }
        }

        private void dgvIdleReason_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvIdleReason.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0 && Data.Count > e.RowIndex)
            {
                var row = dgvIdleReason.Rows[e.RowIndex];
                var dataRecord = Data[e.RowIndex];
                var hasComment = database.AdditionalIdleRecords.Where(air => air.EquipmentIdleID == dataRecord.EquipmentIdleID).Count() > 0;
                bool AllowToAddComment = actionAddCommentAllowed == ActionAddAdditionalCommentAllowed.Allowed;
                var buttonCell = row.Cells[e.ColumnIndex] as DataGridViewButtonCell;
                if (buttonCell != null)
                {
                    if (hasComment || AllowToAddComment)
                    {
                        buttonCell.FlatStyle = FlatStyle.Standard;
                        buttonCell.Style.ForeColor = Color.Black;
                        buttonCell.Style.BackColor = Color.White;
                        row.Cells[e.ColumnIndex].ReadOnly = false; // Включаем обратно
                        // Оставляем кнопку, если есть комментарий
                        e.CellStyle.ForeColor = Color.Black;
                        if (hasComment)
                        {
                            e.Value = "Добавлено";
                            e.CellStyle.Font = new Font(e.CellStyle.Font.FontFamily, 10);
                        }
                        else
                        {
                            e.Value = "···";
                            e.CellStyle.Font = new Font(e.CellStyle.Font.FontFamily, 24);
                        }
                        buttonCell.Style.Font = new Font(buttonCell.Style?.Font?.FontFamily ?? new FontFamily("Arial"), 24);
                        if (hasComment) e.CellStyle.BackColor = Color.LimeGreen;
                    }
                    else
                    {
                        buttonCell.FlatStyle = FlatStyle.Flat;
                        buttonCell.Style.ForeColor = Color.Gray;
                        buttonCell.Style.BackColor = Color.LightGray;

                        row.Cells[e.ColumnIndex].ReadOnly = true;  // Делаем кнопку неактивной
                        e.Value = null;  // Убираем текст кнопки
                        e.CellStyle.BackColor = dgvIdleReason.BackgroundColor; // Маскируем под фон
                        e.CellStyle.ForeColor = dgvIdleReason.BackgroundColor;
                    }

                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((dgvIdleReason.Focused || dgvIdleReason.EditingControl != null)
                && keyData == Keys.Enter)
            {
                dgvIdleReason.EndEdit();
                return true;
            }
            else
                return base.ProcessCmdKey(ref msg, keyData);
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
                    lblSelectedEquipment.Text = $"Машина №{CurrentEquipmentNumber}";
                    ShiftShowData();
                }
            }
            //throw new NotImplementedException();
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

            CheckForOpenIdles();
            //dgvIdleReason.DataSource = Data;
            var Readonly = actionEditAlowed == ActionEditAllowed.NoEdit;
            IdleStartedColumn.ReadOnly = Readonly;
            IdleEndedColumn.ReadOnly = Readonly;
            //IdleDurationColumn.ReadOnly = Readonly;
            ReasonsTypeColumn.ReadOnly = Readonly;
            ProfileColumn.ReadOnly = Readonly;
            NodeColumn.ReadOnly = Readonly;
            ElementColumn.ReadOnly = Readonly;
            MalfunctionTextColumn.ReadOnly = Readonly;
            MalfunctionTextNameColumn.ReadOnly = Readonly;
        }

        private void RefreshDataView()
        {
            dgvIdleReason.DataSource = Data;
        }
        private List<EquipmentIdle> GetEquipmentIdles()
        {
            database.RefreshContext();
            var data = database.EquipmentIdles.Where(ei => ei.IsNightShift == CurrentIsNightShift && ei.ShiftStart == CurrentShiftDate && ei.EquipmentNumber == CurrentEquipmentNumber).ToList();
            data = data.OrderBy(d => d.IdleStart.HasValue ? d.IdleStart : d.IdleEnd).ToList();
            return data;

        }

        private void CheckForOpenIdles()
        {
            database.RefreshContext();
            MachineButtons.ForEach(mb => mb.BackColor = SystemColors.Control); ;
            var data = database.EquipmentIdles.Where(ei => ei.IsNightShift == CurrentIsNightShift && ei.ShiftStart == CurrentShiftDate).ToList();
            var grEq = data.GroupBy(d => d.EquipmentNumber);
            foreach (var eq in grEq)
            {
                var mb = MachineButtons.Find(b => (int)b.Tag == eq.Key);
                if (mb != null)
                {
                    var eqdata = eq.ToList();
                    var unfilled = eqdata.Find(d => (d.MalfunctionReasonTypeID ?? 0) == 0);
                    if (unfilled == null)
                    {
                        mb.BackColor = Color.LimeGreen;
                    }
                    else
                    {
                        mb.BackColor = Color.OrangeRed;

                    }
                }
            }
        }

        private void RefreshDatabaseWithAttachCurrentToContext()
        {
            database.RefreshContext();
            foreach (var d in Data)
            {
                database.EquipmentIdles.Attach(d);
            }
        }


    }

}
