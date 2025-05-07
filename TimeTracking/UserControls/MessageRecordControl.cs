using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TimeTrackingDB;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TimeTracking.UserControls
{
    public partial class MessageRecordControl : UserControl
    {
        public delegate void OnDeleteButtonEvent(MessageRecordControl sender, AdditionalIdleRecord CurrentMessage);
        int MessageID;
        AdditionalIdleRecord CurrentMessage;
        DB Database;
        private ImageList imageList;
        bool isRefreshing = false;
        bool _ReadOnly = false;
        public event OnDeleteButtonEvent OnDeleteButton;
        public bool ReadOnly
        {
            get => _ReadOnly;
            set
            {
                _ReadOnly = value;
                VisualizeReadonlyStatus(_ReadOnly);
                Refresh();
            }
        }
        public MessageRecordControl(int messageID, DB database)
        {
            InitializeComponent();
            MessageID = messageID;
            Database = database;
            CurrentMessage = Database.AdditionalIdleRecords.Where(air => air.AdditionalIdleRecordID == MessageID).FirstOrDefault();
            Refresh();
        }
        public override void Refresh()
        {
            base.Refresh();
            isRefreshing = true;
            try
            {
                if (CurrentMessage == null) return;
                var username = Database.Users.Where(u => u.UserID == CurrentMessage.UserID).FirstOrDefault()?.UserName ?? $"Неизвестно(UserID={CurrentMessage.UserID})";
                lblUser.Text = username;
                txbText.Text = CurrentMessage.Text;
                lblDateTimeCreate.Text = CurrentMessage.RecordDateTimeCreation.ToString("dd-MM-yyyy HH\\:mm\\:ss");
                imageList = new ImageList
                {
                    ImageSize = new Size(32, 32) // Размер иконок
                };
                lvFiles.Items.Clear();
                lvFiles.LargeImageList = imageList; // Привязываем ImageList к ListView
                var files = Database.AdditionalIdleRecordFiles.Where(airf => airf.AdditionalIdleRecordID == MessageID).ToList();
                for (int i = 0; i < files.Count(); i++)
                {
                    var fileName = String.IsNullOrWhiteSpace(files[i].Filename) ? $"{i}.jpg" : files[i].Filename;
                    Icon fileIcon = FileIconHelper.GetIconByExtension(Path.GetExtension(fileName));// Icon.ExtractAssociatedIcon(fileName);
                    if (fileIcon != null)
                    {
                        imageList.Images.Add(fileName, fileIcon);
                    }
                    var lvi = new ListViewItem()
                    {
                        Text = fileName,
                        ImageKey = fileName
                    };
                    lvi.Tag = files[i].AdditionalIdleRecordFileID;
                    lvFiles.Items.Add(lvi);
                }
                SetElementsPosition();

                tsmiDeleteFile.Enabled = !_ReadOnly;
            }
            finally
            {
                isRefreshing = false;
            }

        }
        protected void SetElementsPosition()
        {
            if (lvFiles.Items.Count > 0)
            {
                lvFiles.Visible = true;
                txbText.Size = new Size(txbText.Size.Width, lvFiles.Top - txbText.Top - 7);
            }
            else
            {
                lvFiles.Visible = false;
                txbText.Size = new Size(txbText.Size.Width, this.ClientSize.Height - txbText.Top - 5);
            }
        }

        private void lvFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (ReadOnly) return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                foreach (string filePath in files)
                {
                    using (var file = File.OpenRead(filePath))
                    {
                        var newFile = new AdditionalIdleRecordFile() { AdditionalIdleRecordID = MessageID };
                        var data = new byte[file.Length];
                        file.Read(data, 0, data.Length);
                        newFile.Data = data;
                        newFile.Filename = Path.GetFileName(filePath);
                        Database.AdditionalIdleRecordFiles.Add(newFile);
                        Database.ChangeTracker.DetectChanges();
                        foreach (var entry in Database.ChangeTracker.Entries<AdditionalIdleRecordFile>())
                        {
                            //Console.WriteLine($"Состояние: {entry.State}"); // Должно быть Added
                        }
                        if (Database.Entry(newFile).State != System.Data.Entity.EntityState.Added)
                            Database.Entry(newFile).State = System.Data.Entity.EntityState.Added;
                        Database.SaveChanges();
                    }
                }
                Refresh();
            }

        }

        private void lvFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (ReadOnly) return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // Разрешаем копирование файлов
            }

        }

        private void txbText_TextChanged(object sender, EventArgs e)
        {
            if (ReadOnly) return;
            if (!isRefreshing)
            {
                CurrentMessage.Text = txbText.Text;
                Database.ChangeTracker.DetectChanges();
                foreach (var entry in Database.ChangeTracker.Entries<AdditionalIdleRecordFile>())
                {
                }
                if (Database.Entry(CurrentMessage).State != System.Data.Entity.EntityState.Modified)
                    Database.Entry(CurrentMessage).State = System.Data.Entity.EntityState.Modified;
                Database.SaveChanges();
                Refresh();

            }
        }

        private void lvFiles_DoubleClick(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0) return; // Если ничего не выбрано, выходим

            ListViewItem item = lvFiles.SelectedItems[0];
            string fileName = item.Text; // Имя файла (из базы)
            int fileID = (int)item.Tag; // Имя файла (из базы)

            // Получаем файл из базы
            AdditionalIdleRecordFile fileData = Database.AdditionalIdleRecordFiles
                .FirstOrDefault(f => f.AdditionalIdleRecordFileID == fileID);

            if (fileData == null)
            {
                MessageBox.Show("Файл не найден в базе!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Создаём временный файл с нужным расширением
            string tempPath = Path.Combine(Path.GetTempPath(), fileName);
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch
                {
                    tempPath = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(tempPath));
                }
            }
            File.WriteAllBytes(tempPath, fileData.Data);

            // Открываем его через ассоциированное приложение
            try
            {
                Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void VisualizeReadonlyStatus(bool ReadOnly)
        {
            if (ReadOnly)
            {
                txbText.ReadOnly = true;
                txbText.BackColor = this.BackColor;
                lvFiles.BackColor = this.BackColor;
                txbText.BorderStyle = BorderStyle.FixedSingle;
                lvFiles.BorderStyle = BorderStyle.None;
                btnDelete.Visible = false;
            }
            else
            {
                txbText.ReadOnly = false;
                txbText.BackColor = SystemColors.Window;
                lvFiles.BackColor = this.BackColor;
                txbText.BorderStyle = BorderStyle.FixedSingle;
                lvFiles.BorderStyle = BorderStyle.None;
                btnDelete.Visible = true;

            }
        }
        public new void Focus()
        {
            if (!_ReadOnly)
                txbText.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_ReadOnly) return;
            if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтвержение удаления", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                OnDeleteButton(this, CurrentMessage);
                Refresh();
            }
        }

        private void lvFiles_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (_ReadOnly) return;
                DeleteSelectedFile();
            }
        }
        private void DeleteSelectedFile()
        {
            if (_ReadOnly) return;
            if (lvFiles.SelectedItems == null) return;
            var selectedFile = lvFiles.SelectedItems[0];
            if (MessageBox.Show($"Вы действительно хотите удалить файл {selectedFile.Text}", "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var fileID = (int)selectedFile.Tag;
                var file = Database.AdditionalIdleRecordFiles.Local.Where(e => e.AdditionalIdleRecordFileID == fileID).FirstOrDefault();
                if (file == null)
                {
                    file = new AdditionalIdleRecordFile() { AdditionalIdleRecordFileID = fileID };
                    Database.AdditionalIdleRecordFiles.Attach(file);
                }
                Database.AdditionalIdleRecordFiles.Remove(file);
                Database.SaveChanges();
                Refresh();
            }
        }

        private void lvFiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = lvFiles.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void tsmiDeleteFile_Click(object sender, EventArgs e)
        {
            if (_ReadOnly) return;
            DeleteSelectedFile();
        }
    }

    public static class FileIconHelper
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        private const uint SHGFI_ICON = 0x100; // Получить иконку
        private const uint SHGFI_USEFILEATTRIBUTES = 0x10; // Использовать расширение вместо реального файла

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        public static Icon GetIconByExtension(string extension)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hImg = SHGetFileInfo(extension, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_USEFILEATTRIBUTES);

            if (hImg != IntPtr.Zero)
            {
                return Icon.FromHandle(shinfo.hIcon);
            }
            return SystemIcons.Application; // Если иконку не нашли, даём стандартную
        }
    }
}
