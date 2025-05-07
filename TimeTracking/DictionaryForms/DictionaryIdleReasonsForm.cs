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
    public partial class DictionaryIdleReasonsForm : TimeTrackingDataForm
    {
        TimeTrackingDB.DB db = null;
        DataGridViewCustomComboBoxColumn<TimeTrackingDB.MalfunctionReasonProfile> profileCol;
        DataGridViewComboBoxColumn /*eqTypeCol,*/  reasTypeCol, nodeCol, elemCol;
        DataGridViewTextBoxColumn defectCol;
        int EquipmentTypeID = 0;
        int ReasonTypeID = 0;
        public DictionaryIdleReasonsForm()
        {
            InitializeComponent();
            Start();
            //ShowData();
        }

        public void Start()
        {
            db = new TimeTrackingDB.DB();
            FillReasonType();
        }

        private void FillReasonType()
        {
            cbResonTypes.Text = "";
            cbResonTypes.ValueMember = "MalfunctionReasonTypeID";
            cbResonTypes.DisplayMember = "MalfunctionReasonTypeName";
            cbResonTypes.SelectedIndexChanged += CbResonTypes_SelectedIndexChanged;
            cbResonTypes.DataSource = db.MalfunctionReasonTypes.ToList();
        }
               
        private void CbResonTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbResonTypes.SelectedItem != null)
            {
                var si = cbResonTypes.SelectedItem as TimeTrackingDB.MalfunctionReasonType;
                if (si != null)
                {
                    ReasonTypeID = si.MalfunctionReasonTypeID;
                    ShowData();
                }

            }
        }

        public void Stop()
        {
            db.Dispose();
        }

        public void ShowData()
        {
            dgvReasons.DataSource = null;
            dgvReasons.Columns.Clear();
            if (ReasonTypeID <= 0) return;
            var datatmp = db.MalfunctionReasonMalfunctionTexts.Where(r => r.MalfunctionReasonTypeID == ReasonTypeID);
            var data = datatmp.OrderBy(t => t.MalfunctionReasonTypeID).ThenBy(p => p.MalfunctionReasonProfileID).ThenBy(n => n.MalfunctionReasonNodeID).ThenBy(e => e.MalfunctionReasonElementID).ToList();


            /*eqTypeCol = new DataGridViewComboBoxColumn();
            eqTypeCol.DataPropertyName = "MalfunctionReasonEquipmentTypeID";
            eqTypeCol.DisplayMember = "MalfunctionReasonEquipmentTypeName";
            eqTypeCol.ValueMember = "MalfunctionReasonEquipmentTypeID";
            eqTypeCol.DataSource = db.MalfunctionReasonEquipmentTypes.ToList();
            eqTypeCol.DefaultCellStyle.Font = new Font("Arial", 12);
            eqTypeCol.Width = 300;
            dgvReasons.Columns.AddRange(eqTypeCol);*/

            /*reasTypeCol = new DataGridViewComboBoxColumn();
            reasTypeCol.DataPropertyName = "MalfunctionReasonTypeID";
            reasTypeCol.DisplayMember = "MalfunctionReasonTypeName";
            reasTypeCol.ValueMember = "MalfunctionReasonTypeID";
            reasTypeCol.DataSource = db.MalfunctionReasonTypes.ToList();
            reasTypeCol.DefaultCellStyle.Font = new Font("Arial", 12);
            reasTypeCol.Width = 300;
            dgvReasons.Columns.AddRange(reasTypeCol);*/

            /*
            profileCol = new DataGridViewComboBoxColumn();
            profileCol.HeaderText = "Профиль";
            profileCol.DataPropertyName = "MalfunctionReasonProfileID";
            profileCol.DisplayMember = "MalfunctionReasonProfileName";
            profileCol.ValueMember = "MalfunctionReasonProfileID";
            profileCol.DataSource = db.MalfunctionReasonProfiles.ToList();
            profileCol.DefaultCellStyle.Font = new Font("Arial", 12);
            profileCol.Width = 300;
            dgvReasons.Columns.AddRange(profileCol);
            */
            profileCol = new DataGridViewCustomComboBoxColumn<TimeTrackingDB.MalfunctionReasonProfile>(
                "Профиль",
                "MalfunctionReasonProfileID",
                "MalfunctionReasonProfileName",
                "MalfunctionReasonProfileID",
                null,//new string[] { "MalfunctionReasonEquipmentTypeID" },
                db.MalfunctionReasonProfiles,
                dgvReasons,
                null
                );
            profileCol.DefaultCellStyle.Font = new Font("Arial", 12);
            profileCol.Width = 300;

            //profileCol.DataSource = db.MalfunctionReasonProfiles.ToList();

            /*nodeCol = new DataGridViewComboBoxColumn();
            nodeCol.HeaderText = "Узел";
            nodeCol.DataPropertyName = "MalfunctionReasonNodeID";
            nodeCol.DisplayMember = "MalfunctionReasonNodeName";
            nodeCol.ValueMember = "MalfunctionReasonNodeID";
            nodeCol.DataSource = db.MalfunctionReasonNodes.ToList();
            nodeCol.DefaultCellStyle.Font = new Font("Arial", 12);
            nodeCol.Width = 300;
            dgvReasons.Columns.AddRange(nodeCol);*/
            nodeCol = new DataGridViewCustomComboBoxColumn<TimeTrackingDB.MalfunctionReasonNode>(
                "Узел",
                "MalfunctionReasonNodeID",
                "MalfunctionReasonNodeName",
                "MalfunctionReasonNodeID",
                new string[] { "MalfunctionReasonProfileID" },
                db.MalfunctionReasonNodes,
                dgvReasons,
                new DataGridViewColumn[] { profileCol }
            );
            nodeCol.DefaultCellStyle.Font = new Font("Arial", 12);
            nodeCol.Width = 300;

            /*elemCol = new DataGridViewComboBoxColumn();
            elemCol.HeaderText = "Элемент";
            elemCol.DataPropertyName = "MalfunctionReasonElementID";
            elemCol.DisplayMember = "MalfunctionReasonElementName";
            elemCol.ValueMember = "MalfunctionReasonElementID";
            elemCol.DataSource = db.MalfunctionReasonElements.ToList();
            elemCol.DefaultCellStyle.Font = new Font("Arial", 12);
            elemCol.Width = 300;
            dgvReasons.Columns.AddRange(elemCol);*/

            elemCol = new DataGridViewCustomComboBoxColumn<TimeTrackingDB.MalfunctionReasonElement>(
                "Элемент", 
                "MalfunctionReasonElementID", 
                "MalfunctionReasonElementName", 
                "MalfunctionReasonElementID",
                new string[] { "MalfunctionReasonNodeID" },
                db.MalfunctionReasonElements, 
                dgvReasons,
                new DataGridViewColumn[] { nodeCol }
            );
            elemCol.DefaultCellStyle.Font = new Font("Arial", 12);
            elemCol.Width = 300;

            defectCol = new DataGridViewTextBoxColumn();
            defectCol.HeaderText = "Текст дефекта";
            defectCol.DataPropertyName = "MalfunctionReasonMalfunctionTextName";
            defectCol.DefaultCellStyle.Font = new Font("Arial", 12);
            defectCol.Width = 300;
            dgvReasons.Columns.AddRange(defectCol);

            dgvReasons.AutoGenerateColumns = false;
            dgvReasons.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12);
            dgvReasons.DataSource = data;
        }

        private void SetMalfunctionReasonTypeComboBoxList(int EquipmentTypeID)
        {
            reasTypeCol = new DataGridViewComboBoxColumn();
            reasTypeCol.DataPropertyName = "MalfunctionReasonTypeID";
            reasTypeCol.DisplayMember = "MalfunctionReasonTypeName";
            reasTypeCol.ValueMember = "MalfunctionReasonTypeID";
            reasTypeCol.DataSource = db.MalfunctionReasonTypes.ToList();
        }
        /*private void SetMalfunctionReasonProfileComboBoxList(int EquipmentTypeID)
        {
            profileCol = new DataGridViewComboBoxColumn();
            profileCol.HeaderText = "Профиль";
            profileCol.DataPropertyName = "MalfunctionReasonProfileID";
            profileCol.DisplayMember = "MalfunctionReasonProfileName";
            profileCol.ValueMember = "MalfunctionReasonProfileID";
            profileCol.DataSource = db.MalfunctionReasonProfiles.Where(r => r.MalfunctionReasonEquipmentTypeID == EquipmentTypeID).ToList();
            profileCol.DefaultCellStyle.Font = new Font("Arial", 12);
            profileCol.Width = 300;
        }*/
        private void SetMalfunctionReasonNodeComboBoxList(int ProfileID)
        {
            nodeCol = new DataGridViewComboBoxColumn();
            nodeCol.HeaderText = "Узел";
            nodeCol.DataPropertyName = "MalfunctionReasonNodeID";
            nodeCol.DisplayMember = "MalfunctionReasonNodeName";
            nodeCol.ValueMember = "MalfunctionReasonNodeID";
            nodeCol.DataSource = db.MalfunctionReasonNodes.Where(r => r.MalfunctionReasonNodeID == ProfileID).ToList();
            nodeCol.DefaultCellStyle.Font = new Font("Arial", 12);
            nodeCol.Width = 300;
        }

        private void dgvReasons_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void SetMalfunctionReasonElementComboBoxList(int NodeID)
        {
            elemCol = new DataGridViewComboBoxColumn();
            elemCol.HeaderText = "Элемент";
            elemCol.DataPropertyName = "MalfunctionReasonElementID";
            elemCol.DisplayMember = "MalfunctionReasonElementName";
            elemCol.ValueMember = "MalfunctionReasonElementID";
            elemCol.DataSource = db.MalfunctionReasonElements.Where(r => r.MalfunctionReasonElementID == NodeID).ToList();
            elemCol.DefaultCellStyle.Font = new Font("Arial", 12);
            elemCol.Width = 300;
        }


        private void DictionaryIdleReasonsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }

    }
}
