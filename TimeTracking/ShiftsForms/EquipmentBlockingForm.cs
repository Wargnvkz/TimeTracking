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
    public partial class EquipmentBlockingForm : TimeTrackingDataForm
    {
        DB database;
        DataGridViewColumn EquipmentNumber, RemoveBlockingButtonColumn, ReasonsTypeColumn, ProfileColumn, NodeColumn, ElementColumn, MalfunctionTextColumn, MalfunctionTextNameColumn;
        List<EquipmentBlocking> Data;
        int RemoveBlockingButtonColumnIndex=-1;


        private void dtpShiftDate_ValueChanged(object sender, EventArgs e)
        {
            ShowData();
        }


        private void dgvBlockingReason_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void rbDay_CheckedChanged(object sender, EventArgs e)
        {
            ShowData();
        }

        private void ShiftsDowntimeForm_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        public EquipmentBlockingForm() : base()
        {
            InitializeComponent();
            database = new DB();
            Prepare();
            this.Focus();
        }

        private void Prepare()
        {
            EquipmentNumber = new DataGridViewTextBoxColumn();
            EquipmentNumber.HeaderText = "Машина";
            EquipmentNumber.DataPropertyName = "EquipmentNumber";
            EquipmentNumber.Width = 60;
            EquipmentNumber.ReadOnly = true; //Нельзя менять, потому что я не смог придумать, как сделать, чтобы оно сохраняло редактирование по Enter. Приходится переходить в другую ячейку.
            dgvBlockingReasons.Columns.Add(EquipmentNumber);

            ReasonsTypeColumn = new DataGridViewCustomComboBoxColumn<MalfunctionReasonType>(
                "Вид простоя",
                "MalfunctionReasonTypeID",
                "MalfunctionReasonTypeName",
                "MalfunctionReasonTypeID",
                null,
                database.MalfunctionReasonTypes,
                dgvBlockingReasons,
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
                dgvBlockingReasons,
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
                dgvBlockingReasons,
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
                dgvBlockingReasons,
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
                dgvBlockingReasons,
                new DataGridViewColumn[] { ReasonsTypeColumn, ProfileColumn, NodeColumn, ElementColumn }
            );
            MalfunctionTextColumn.Width = 200;

            MalfunctionTextNameColumn = new DataGridViewTextBoxColumn();
            MalfunctionTextNameColumn.HeaderText = "Замечания";
            MalfunctionTextNameColumn.DataPropertyName = "MalfunctionReasonMalfunctionTextComment";
            MalfunctionTextNameColumn.Width = 300;
            dgvBlockingReasons.Columns.Add(MalfunctionTextNameColumn);

            RemoveBlockingButtonColumn = new DataGridViewButtonColumn();
            RemoveBlockingButtonColumn.HeaderText = "Удалить блокировку";
            RemoveBlockingButtonColumn.Width = 120;
            dgvBlockingReasons.Columns.Add(RemoveBlockingButtonColumn);
            RemoveBlockingButtonColumnIndex = dgvBlockingReasons.Columns.Count - 1;


            var fontCell = new Font("Arial", 9);
            var fontHeader = new Font("Arial", 10);

            dgvBlockingReasons.DefaultCellStyle.Font = fontCell;
            dgvBlockingReasons.ColumnHeadersDefaultCellStyle.Font = fontHeader;

            dgvBlockingReasons.CellValueChanged += dgvBlockingReasons_CellValueChanged;
            dgvBlockingReasons.CellPainting += dgvBlockingReasons_CellPainting;
            dgvBlockingReasons.CellContentClick += dgvBlockingReasons_CellContentClick;
            dgvBlockingReasons.AutoGenerateColumns = false;

        }

        private void dgvBlockingReasons_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0|| e.ColumnIndex < 0 || e.ColumnIndex >= dgvBlockingReasons.Columns.Count|| RemoveBlockingButtonColumnIndex<0) return;
            if (RemoveBlockingButtonColumnIndex == e.ColumnIndex)
            {
                var record = Data[e.RowIndex];
                if (MessageBox.Show("Вы уверены, что хотите удалить запись машины " + record.EquipmentNumber.ToString(), "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    RefreshDatabaseWithAttachCurrentToContext();
                    database.EquipmentBlockings.Remove(record);
                    database.SaveChanges();
                    ShowData();
                }
            }
        }

        private void dgvBlockingReasons_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            //I supposed your button column is at index 0
            if (e.ColumnIndex == RemoveBlockingButtonColumnIndex)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.DeleteCross.Width;
                var h = Properties.Resources.DeleteCross.Height;
                var bw = e.CellBounds.Width;
                var bh = e.CellBounds.Height;
                var wratio = (double)w / bw;
                var hratio = (double)h / bh;
                var ratio = Math.Max(wratio, hratio);
                var w1 = (int)(w / ratio);
                var h1 = (int)(h / ratio);
                var x = e.CellBounds.Left + (e.CellBounds.Width - w1) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h1) / 2;

                e.Graphics.DrawImage(Properties.Resources.DeleteCross, new Rectangle(x, y, w1, h1));
                e.Handled = true;
            }
        }

        private void dgvBlockingReason_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            //TODO: Сделать проверку разделенных записей, чтобы время начало и конца не выходило за пределы имеющихся в записи
            //TODO: скопировать время в разделенной записи

            database.SaveChanges();

            dgvBlockingReasons.Update();
            dgvBlockingReasons.Refresh();

        }

        private void dgvBlockingReason_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            RefreshDatabaseWithAttachCurrentToContext();
        }

        private void добавитьМшинуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            database.RefreshContext();
            string machineText = "";
            if (Prompt.ShowDialog(ref machineText, "Введите номер машины", "Ввод номера машины", false))
            {
                if (int.TryParse(machineText, out int machine))
                {
                    if (database.EquipmentBlockings.Where(eb => eb.EquipmentNumber == machine).Count() == 0)
                    {
                        database.EquipmentBlockings.Add(new EquipmentBlocking() { EquipmentNumber = machine });
                        database.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show($"Запись для машины {machine} уже существует");
                    }
                }
                else
                {
                    MessageBox.Show("Номер машины должен быть целым числом");
                }
            }
            ShowData();

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((dgvBlockingReasons.Focused || dgvBlockingReasons.EditingControl != null)
                && keyData == Keys.Enter)
            {
                dgvBlockingReasons.EndEdit();
                return true;
            }
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }

        DateTime LastDataShown = DateTime.MinValue;


        int RefreshTimeout = 300;
        private void timer1_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var dtime = (now - LastDataShown).TotalSeconds;
            if (dtime > RefreshTimeout)
            {
                if (!(dgvBlockingReasons.CurrentCell?.IsInEditMode ?? false))
                {
                    ShowData();
                }
                LastDataShown = now;
            }
            else
            {
                lblRefreshAfterCaption.Text = $"Обновление через: {(RefreshTimeout - dtime):F0} сек";
            }
        }




        private void dgvBlockingReasons_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ShiftsDowntimeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            database.Dispose();
        }

        private void ShowData()
        {
            Data = GetEquipmentBlockings();
            if (Data.Count == 0)
            {
                Data = GetEquipmentBlockings();
            }
            //AddNewLine();//(false);
            RefreshDataView();
            //dgvIdleReason.DataSource = Data;
        }

        private void RefreshDataView()
        {
            dgvBlockingReasons.DataSource = Data;
        }
        private List<EquipmentBlocking> GetEquipmentBlockings()
        {
            database.RefreshContext();
            var data = database.EquipmentBlockings.ToList();
            return data;
        }


        private void RefreshDatabaseWithAttachCurrentToContext()
        {
            database.RefreshContext();
            foreach (var d in Data)
            {
                database.EquipmentBlockings.Attach(d);
            }
        }


    }

}
