﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SubtitlesCleanerLibrary;

namespace SubtitlesCleanerEditor
{
    public partial class SubtitlesCleanerEditorForm : Form
    {
        public static readonly bool IsProduction = true;

        #region Form

        public SubtitlesCleanerEditorForm(string[] args)
        {
            InitializeComponent();

            ResetFormTitle();

            lstEditor.AutoGenerateColumns = false;
            lstErrors.AutoGenerateColumns = false;

            txtSubtitle.Init(lstEditor);

            ParseArgs(args);
        }

        private void ParseArgs(string[] args)
        {
            if (args != null && args.Length == 1 && string.IsNullOrEmpty(args[0]) == false && Directory.Exists(args[0]))
            {
                bool isDirectory = Directory.Exists(args[0]);
                bool isFile = File.Exists(args[0]);

                string initialDirectory = null;

                if (isDirectory)
                {
                    initialDirectory = args[0];
                    string[] filePaths = Directory.GetFiles(initialDirectory, "*.srt");
                    if (filePaths != null && filePaths.Length > 0)
                        initialFile = filePaths[0];
                }
                else if (isFile)
                {
                    initialFile = args[0];
                    initialDirectory = Path.GetDirectoryName(args[0]);
                }

                if (string.IsNullOrEmpty(initialDirectory) == false)
                {
                    openFileDialog.InitialDirectory = initialDirectory;
                    saveAsFileDialog.InitialDirectory = initialDirectory;
                }
            }
        }

        private string initialFile;

        private void SubtitlesCleanerEditorForm_Load(object sender, EventArgs e)
        {
            if (IsProduction)
            {
                if (string.IsNullOrEmpty(initialFile) == false)
                    LoadFile(initialFile);
            }
            else
            {
                LoadFile(Path.GetFullPath(Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "..", "..", "..", "Subtitles",
                    "Test.srt"
                )));
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Set Subtitles To Editor

        private List<Subtitle> subtitles;
        private List<Subtitle> originalSubtitles;
        private bool cleanHICaseInsensitive;
        private string filePath;
        private Encoding encoding = Encoding.UTF8;

        private void SetSubtitlesToEditor(List<Subtitle> subtitles)
        {
            this.Cursor = Cursors.WaitCursor;

            this.subtitles = subtitles;
            lstErrors_SortedColumnIndex = 0;
            lstErrors_sortOrder = SortOrder.Ascending;

            CurrentSelectedRow = null;
            CurrentErrorSelectedRow = null;

            lstEditor.SelectionChanged -= lstEditor_SelectionChanged;
            lstErrors.SelectionChanged -= lstErrors_SelectionChanged;

            if (subtitles != null && subtitles.Count > 0)
            {
                lstEditor.DataSource = new BindingList<EditorRow>(subtitles.Select((subtitle, index) =>
                {
                    Subtitle cleanSubtitle = SubtitlesHelper.CleanSubtitles(subtitle.Clone() as Subtitle, cleanHICaseInsensitive, false);

                    return new EditorRow()
                    {
                        Num = index + 1,
                        ShowValue = subtitle.Show,
                        Show = subtitle.ShowToString(),
                        Hide = subtitle.HideToString(),
                        Duration = subtitle.DurationToString(),
                        Text = subtitle.ToStringWithPipe(),
                        Lines = subtitle.ToString(),
                        CleanText = (cleanSubtitle != null ? cleanSubtitle.ToStringWithPipe() : string.Empty),
                        CleanLines = (cleanSubtitle != null ? cleanSubtitle.ToString() : string.Empty),
                        SubtitleError = subtitle.SubtitleError
                    };
                }).ToList());

                CurrentSelectedRow = lstEditor.Rows[0];
            }
            else
            {
                lstEditor.DataSource = new BindingList<EditorRow>(new List<EditorRow>());
            }

            lstEditor.SelectionChanged += lstEditor_SelectionChanged;
            lstErrors.SelectionChanged += lstErrors_SelectionChanged;

            SelectionChanged();
            ErrorSelectionChanged();

            if (subtitles == null || subtitles.Count == 0)
            {
                txtSubtitle.Text = string.Empty;
                txtCleanSubtitle.Text = string.Empty;
                ResetLineLengths();
                timePicker.Reset();
            }

            this.Cursor = Cursors.Default;
        }

        private void SetSubtitlesToEditorAndKeepSubtitleNumber(List<Subtitle> subtitles)
        {
            if (subtitles == null)
                return;

            int num = 1;

            EditorRow editorRow = GetSelectedEditorRow();
            if (editorRow != null)
                num = editorRow.Num;

            SetSubtitlesToEditor(subtitles);

            int index = num - 1;
            if (index < 0)
                index = 0;
            else if (index > lstEditor.Rows.Count - 1)
                index = lstEditor.Rows.Count - 1;

            SelectEditorRow(index);
        }

        #endregion

        #region Load Subtitles File

        private void LoadFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            bool isSRT = string.Compare(extension, ".srt", true) == 0;
            if (isSRT)
            {
                encoding = Encoding.UTF8;
                subtitles = SubtitlesHelper.GetSubtitles(filePath, ref encoding);
                subtitles.CheckSubtitles(cleanHICaseInsensitive);
                originalSubtitles = subtitles.Clone();
                this.filePath = filePath;
                SetSubtitlesToEditor(subtitles);
                SelectFirstError();
                SetFormTitle(false);
                lstEditor.Focus();
            }
        }

        private void ResetFormTitle()
        {
            this.Text =
                "Subtitles Cleaner Editor" + " " +
                Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        private void SetFormTitle(bool isDirty)
        {
            ResetFormTitle();

            this.Text =
                Path.GetFileName(this.filePath) + (isDirty ? " *" : string.Empty) +
                " - " + this.Text;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                LoadFile(openFileDialog.FileName);
        }

        private void SubtitlesCleanerEditorForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void SubtitlesCleanerEditorForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (filePaths != null && filePaths.Length == 1)
                    LoadFile(filePaths[0]);
            }
        }

        #endregion

        #region Subtitles Errors

        private void lstEditor_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            SetSubtitlesErrors();
        }

        private void SetSubtitlesErrors()
        {
            List<ErrorRow> errorRows = new List<ErrorRow>();

            foreach (DataGridViewRow row in lstEditor.Rows)
            {
                EditorRow editorRow = row.DataBoundItem as EditorRow;

                if (editorRow.SubtitleError != SubtitleError.None)
                {
                    IEnumerable<SubtitleError> allErrors = GetSubtitleErrors(editorRow.SubtitleError);

                    SubtitleError firstError = allErrors.FirstOrDefault();
                    row.DefaultCellStyle.BackColor = firstError.BackErrorColor();
                    row.DefaultCellStyle.ForeColor = firstError.ForeErrorColor();
                    row.DefaultCellStyle.SelectionBackColor = firstError.BackErrorColor();
                    row.DefaultCellStyle.SelectionForeColor = firstError.ForeErrorColor();

                    foreach (var error in allErrors)
                    {
                        if (error != SubtitleError.None)
                        {
                            errorRows.Add(new ErrorRow()
                            {
                                Num = editorRow.Num,
                                Error = error.ToString().Replace('_', ' '),
                                SubtitleError = error
                            });
                        }
                    }
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    row.DefaultCellStyle.SelectionBackColor = Color.Empty;
                    row.DefaultCellStyle.SelectionForeColor = Color.Empty;
                }
            }

            lstErrors.DataSource = errorRows;

            sortErrors();
        }

        private static readonly SubtitleError[] allSubtitleErrors = Enum.GetValues(typeof(SubtitleError)).Cast<SubtitleError>().OrderByDescending(e => e).ToArray();

        private IEnumerable<SubtitleError> GetSubtitleErrors(SubtitleError subtitleError)
        {
            foreach (SubtitleError error in allSubtitleErrors)
            {
                if (subtitleError.IsSet(error))
                    yield return error;
            }
        }

        #endregion

        #region Select Row

        private void SelectGVRow(DataGridViewRow row)
        {
            row.Selected = true;
            row.Cells[0].Selected = true;
            //row.DataGridView.FirstDisplayedScrollingRowIndex = row.Index;
        }

        private void SelectEditorRow(int index)
        {
            if (0 <= index && index < lstEditor.Rows.Count)
            {
                DataGridViewRow row = lstEditor.Rows[index];
                SelectGVRow(row);
            }
        }

        private void SelectErrorRow(int index)
        {
            DataGridViewRow row = lstErrors.Rows[index];
            SelectGVRow(row);
        }

        private void SelectErrorRowByNum(int num)
        {
            foreach (DataGridViewRow row in lstErrors.Rows)
            {
                ErrorRow errorRow = row.DataBoundItem as ErrorRow;
                if (errorRow.Num == num)
                {
                    SelectGVRow(row);
                    break;
                }
            }
        }

        private EditorRow GetEditorRowAt(int index)
        {
            DataGridViewRow row = lstEditor.Rows[index];
            return row.DataBoundItem as EditorRow;
        }

        private ErrorRow GetErrorRowAt(int index)
        {
            DataGridViewRow row = lstErrors.Rows[index];
            return row.DataBoundItem as ErrorRow;
        }

        private DataGridViewRow GetSelectedEditorGVRow()
        {
            if (lstEditor.SelectedRows == null || lstEditor.SelectedRows.Count == 0)
                return null;
            return lstEditor.SelectedRows[0];
        }

        private DataGridViewRow GetSelectedErrorGVRow()
        {
            if (lstErrors.SelectedRows == null || lstErrors.SelectedRows.Count == 0)
                return null;
            return lstErrors.SelectedRows[0];
        }

        private EditorRow GetSelectedEditorRow()
        {
            DataGridViewRow row = GetSelectedEditorGVRow();
            if (row != null)
                return row.DataBoundItem as EditorRow;
            return null;
        }

        private ErrorRow GetSelectedErrorRow()
        {
            DataGridViewRow row = GetSelectedErrorGVRow();
            if (row != null)
                return row.DataBoundItem as ErrorRow;
            return null;
        }

        private ErrorRow SelectClosestErrorRow(int num)
        {
            foreach (DataGridViewRow row in lstErrors.Rows)
            {
                ErrorRow errorRow = row.DataBoundItem as ErrorRow;
                if (errorRow.Num == num)
                {
                    SelectGVRow(row);
                    return errorRow;
                }
                else if (errorRow.Num > num)
                {
                    SelectGVRow(row);
                    return errorRow;
                }
            }

            return SelectFirstErrorRow();
        }

        private ErrorRow SelectFirstErrorRow()
        {
            if (lstErrors.Rows != null && lstErrors.Rows.Count > 0)
            {
                DataGridViewRow row = lstErrors.Rows[0];
                ErrorRow errorRow = row.DataBoundItem as ErrorRow;
                SelectGVRow(row);
                return errorRow;
            }

            return null;
        }

        #endregion

        #region Clean Subtitles

        private void btnClean_Click(object sender, EventArgs e)
        {
            if (subtitles == null)
                return;

            var newSubtitles = subtitles.CleanSubtitles(cleanHICaseInsensitive, false);
            newSubtitles.CheckSubtitles(cleanHICaseInsensitive);
            SetSubtitlesToEditorAndKeepSubtitleNumber(newSubtitles);
            SetFormTitle(true);
        }

        #endregion

        #region Reorder

        private void btnReorder_Click(object sender, EventArgs e)
        {
            if (subtitles == null)
                return;

            var newSubtitles = subtitles.Reorder();
            newSubtitles.CheckSubtitles(cleanHICaseInsensitive);
            SetSubtitlesToEditorAndKeepSubtitleNumber(newSubtitles);
            SetFormTitle(true);
        }

        #endregion

        #region Balance Lines

        private void btnBalanceLines_Click(object sender, EventArgs e)
        {
            if (subtitles == null)
                return;

            var newSubtitles = subtitles.BalanceLines();
            newSubtitles.CheckSubtitles(cleanHICaseInsensitive);
            SetSubtitlesToEditorAndKeepSubtitleNumber(newSubtitles);
            SetFormTitle(true);
        }

        #endregion

        #region Original Subtitles

        private void btnOriginalSubtitles_Click(object sender, EventArgs e)
        {
            if (originalSubtitles == null)
                return;

            var newSubtitles = originalSubtitles.Clone();
            newSubtitles.CheckSubtitles(cleanHICaseInsensitive);
            SetSubtitlesToEditorAndKeepSubtitleNumber(newSubtitles);
            SetFormTitle(false);
        }

        #endregion

        #region Clear

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            SetSubtitlesToEditor(null);
            originalSubtitles = null;
            filePath = null;
            txtSubtitle.Text = string.Empty;
            txtCleanSubtitle.Text = string.Empty;
            ResetLineLengths();
            timePicker.Reset();
            ResetFormTitle();
        }

        #endregion

        #region Delete Subtitle

        private void lstEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                EditorRow editorRow = GetSelectedEditorRow();
                if (editorRow == null)
                    return;

                DeleteSubtitle(editorRow);
            }
        }

        private void DeleteSubtitle(EditorRow editorRow)
        {
            subtitles.RemoveAt(editorRow.Num - 1);

            for (int num = editorRow.Num; num < lstEditor.Rows.Count; num++)
            {
                EditorRow er = GetEditorRowAt(num);
                er.Num = num;
            }

            if (lstEditor.Rows.Count == 1)
            {
                txtSubtitle.Text = string.Empty;
                txtCleanSubtitle.Text = string.Empty;
                ResetLineLengths();
            }

            SetSubtitlesErrors();

            SetFormTitle(true);
        }

        #endregion

        #region Selection Changed

        private DataGridViewRow CurrentSelectedRow;
        private DataGridViewRow CurrentErrorSelectedRow;

        private void lstEditor_SelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged();
        }

        private void SelectionChanged()
        {
            EditorRow editorRow = GetSelectedEditorRow();
            if (editorRow == null)
                return;

            txtSubtitle.Text = editorRow.Lines;
            txtCleanSubtitle.Text = editorRow.CleanLines;
            SetLineLengths();

            timePicker.Value = editorRow.ShowValue;

            isIncludeSelectedRowInSearch = true;

            DataGridViewRow previousSelectedRow = CurrentSelectedRow;
            CurrentSelectedRow = GetSelectedEditorGVRow();
            if (previousSelectedRow != null)
                previousSelectedRow.DefaultCellStyle.Font = new Font("Tahoma", 9F, FontStyle.Regular);
            if (CurrentSelectedRow != null)
                CurrentSelectedRow.DefaultCellStyle.Font = new Font("Tahoma", 8F, FontStyle.Bold);

            if (chkSyncErrorsAndSubtitles.Checked)
                SyncErrorsAndSubtitles(editorRow);
        }

        private void lstErrors_SelectionChanged(object sender, EventArgs e)
        {
            ErrorSelectionChanged();
        }

        private void ErrorSelectionChanged()
        {
            DataGridViewRow previousErrorSelectedRow = CurrentErrorSelectedRow;
            CurrentErrorSelectedRow = GetSelectedErrorGVRow();
            if (previousErrorSelectedRow != null)
                previousErrorSelectedRow.DefaultCellStyle.Font = new Font("Tahoma", 9F, FontStyle.Regular);
            if (CurrentErrorSelectedRow != null)
                CurrentErrorSelectedRow.DefaultCellStyle.Font = new Font("Tahoma", 8F, FontStyle.Bold);

            if (chkSyncErrorsAndSubtitles.Checked)
                SyncErrorsAndSubtitles(GetSelectedErrorRow());
        }

        #endregion

        #region Sync Errors And Subtitles

        private void SyncErrorsAndSubtitles(EditorRow editorRow)
        {
            if (editorRow == null)
                return;

            int num = editorRow.Num;

            ErrorRow errorRow = GetSelectedErrorRow();
            if (errorRow != null && errorRow.Num == num)
                return;

            SelectErrorRowByNum(num);
        }

        private void SyncErrorsAndSubtitles(ErrorRow errorRow)
        {
            if (errorRow == null)
                return;

            int num = errorRow.Num;

            EditorRow editorRow = GetSelectedEditorRow();
            if (editorRow != null && editorRow.Num == num)
                return;

            int index = num - 1;
            if (0 <= index && index <= lstEditor.Rows.Count - 1)
                SelectEditorRow(index);
        }

        #endregion

        #region Line Lengths

        private void SetLineLengths()
        {
            lblLineLengths.Text =
                (txtSubtitle.Lines.Length > 0 ?
                string.Join(" / ", txtSubtitle.Lines.Select(line => SubtitlesHelper.GetDisplayCharCount(line))) :
                string.Empty);

            lblCleanLineLengths.Text =
                (txtCleanSubtitle.Lines.Length > 0 ?
                string.Join(" / ", txtCleanSubtitle.Lines.Select(line => SubtitlesHelper.GetDisplayCharCount(line))) :
                string.Empty);
        }

        private void ResetLineLengths()
        {
            lblLineLengths.Text = string.Empty;
            lblCleanLineLengths.Text = string.Empty;
        }

        #endregion

        #region Errors List

        private int lstErrors_SortedColumnIndex;
        private SortOrder lstErrors_sortOrder;

        private void lstErrors_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ChangeErrorsSort(e.ColumnIndex);
        }

        private void ChangeErrorsSort(int columnIndex)
        {
            if (lstErrors_SortedColumnIndex == columnIndex)
            {
                if (lstErrors_sortOrder == SortOrder.Ascending)
                    lstErrors_sortOrder = SortOrder.Descending;
                else
                    lstErrors_sortOrder = SortOrder.Ascending;
            }
            else
            {
                lstErrors_SortedColumnIndex = columnIndex;
                lstErrors_sortOrder = SortOrder.Ascending;
            }

            sortErrors();
        }

        private void sortErrors()
        {
            ErrorRow errorRowBeforeSort = GetSelectedErrorRow();

            List<ErrorRow> errorRows = lstErrors.DataSource as List<ErrorRow>;

            if (lstErrors_SortedColumnIndex == 0 && lstErrors_sortOrder == SortOrder.Ascending)
            {
                errorRows.Sort((x, y) =>
                {
                    int result = 0;
                    if ((result = x.Num.CompareTo(y.Num)) != 0)
                        return result;
                    return x.Error.CompareTo(y.Error);
                });
            }
            else if (lstErrors_SortedColumnIndex == 1 && lstErrors_sortOrder == SortOrder.Ascending)
            {
                errorRows.Sort((x, y) =>
                {
                    int result = 0;
                    if ((result = x.Error.CompareTo(y.Error)) != 0)
                        return result;
                    return x.Num.CompareTo(y.Num);
                });
            }
            else if (lstErrors_SortedColumnIndex == 0 && lstErrors_sortOrder == SortOrder.Descending)
            {
                errorRows.Sort((x, y) =>
                {
                    int result = 0;
                    if ((result = x.Num.CompareTo(y.Num)) != 0)
                        return -result;
                    return x.Error.CompareTo(y.Error);
                });
            }
            else if (lstErrors_SortedColumnIndex == 1 && lstErrors_sortOrder == SortOrder.Descending)
            {
                errorRows.Sort((x, y) =>
                {
                    int result = 0;
                    if ((result = x.Error.CompareTo(y.Error)) != 0)
                        return -result;
                    return x.Num.CompareTo(y.Num);
                });
            }

            CurrentErrorSelectedRow = null;

            lstErrors.DataSource = null;
            lstErrors.DataSource = errorRows;

            lstErrors.Columns[lstErrors_SortedColumnIndex].HeaderCell.SortGlyphDirection = lstErrors_sortOrder;

            if (lstErrors.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in lstErrors.Rows)
                {
                    ErrorRow errorRow = row.DataBoundItem as ErrorRow;

                    row.DefaultCellStyle.BackColor = errorRow.SubtitleError.BackErrorColor();
                    row.DefaultCellStyle.ForeColor = errorRow.SubtitleError.ForeErrorColor();
                    row.DefaultCellStyle.SelectionBackColor = errorRow.SubtitleError.BackErrorColor();
                    row.DefaultCellStyle.SelectionForeColor = errorRow.SubtitleError.ForeErrorColor();
                    row.DefaultCellStyle.Font = new Font("Tahoma", 9F, FontStyle.Regular);

                    if (errorRowBeforeSort == errorRow)
                    {
                        CurrentErrorSelectedRow = row;
                        SelectGVRow(row);
                    }
                }

                ErrorSelectionChanged();
            }
        }

        private void SelectFirstError()
        {
            ErrorRow errorRow = SelectFirstErrorRow();
            if (errorRow == null)
                return;

            SelectEditorRowFromErrorRow();
        }

        private void lstErrors_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SelectEditorRowFromErrorRow();
        }

        private void SelectEditorRowFromErrorRow()
        {
            if (lstErrors.CurrentRow.Selected)
            {
                ErrorRow errorRow = GetSelectedErrorRow();
                SyncErrorsAndSubtitles(errorRow);
            }
        }

        private void lstEditor_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SelectErrorRowFromEditorRow(e.RowIndex);
        }

        private void SelectErrorRowFromEditorRow(int rowIndex)
        {
            EditorRow editorRow = GetEditorRowAt(rowIndex);
            SyncErrorsAndSubtitles(editorRow);
        }

        #endregion

        #region HI Case Changed

        private void rdbHICase_CheckedChanged(object sender, EventArgs e)
        {
            ChangeHICaseSensitivity();
        }

        private void ChangeHICaseSensitivity()
        {
            cleanHICaseInsensitive = rdbHIUpperLowerCases.Checked;
            subtitles.CheckSubtitles(cleanHICaseInsensitive);
            SetSubtitlesToEditorAndKeepSubtitleNumber(subtitles);
            SetFormTitle(true);
        }

        #endregion

        #region Save

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save(true);
        }

        private void Save(bool withAlert)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            if (string.IsNullOrEmpty(filePath))
                return;

            string message = string.Empty;

            if (chkBackupSRT.Checked)
            {
                string backPath = filePath.Replace(".srt", ".bak.srt");

                try
                {
                    File.WriteAllLines(backPath, originalSubtitles.ToLines(), encoding);
                    message = Path.GetFileName(backPath) + " saved" + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    message = "Failed to save " + Path.GetFileName(backPath) + Environment.NewLine + ex.Message + Environment.NewLine;
                }
            }

            try
            {
                File.WriteAllLines(filePath, subtitles.ToLines(), encoding);
                SetFormTitle(false);
                message += Path.GetFileName(filePath) + " saved";
            }
            catch (Exception ex)
            {
                message += "Failed to save " + Path.GetFileName(filePath) + Environment.NewLine + ex.Message;
            }

            if (withAlert)
            {
                message = message.Trim();
                MessageBox.Show(this, message, "Subtitle File Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void SaveAs()
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            saveAsFileDialog.FileName = Path.GetFileName(filePath);

            if (saveAsFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    File.WriteAllLines(saveAsFileDialog.FileName, subtitles.ToLines(), encoding);
                    MessageBox.Show(this,
                        Path.GetFileName(saveAsFileDialog.FileName) + " saved",
                        "Subtitle File Save", MessageBoxButtons.OK, MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,
                        "Failed to save " + Path.GetFileName(saveAsFileDialog.FileName) + Environment.NewLine + ex.Message,
                        "Subtitle File Save", MessageBoxButtons.OK, MessageBoxIcon.Information
                    );
                }
            }
        }

        #endregion

        #region Set Show Time

        private void btnSetShowTime_Click(object sender, EventArgs e)
        {
            EditorRow editorRow = GetSelectedEditorRow();
            if (editorRow == null)
                return;

            subtitles.SetShowTime(timePicker.Value, editorRow.Num);
            SetSubtitlesToEditorAndKeepSubtitleNumber(subtitles);
            SetFormTitle(true);
        }

        private void timePicker_MillisecondsAdded(object sender, int ms)
        {
            if (chkInteractiveRetiming.Checked)
            {
                EditorRow editorRow = GetSelectedEditorRow();
                if (editorRow == null)
                    return;

                try
                {
                    subtitles.AddTime(TimeSpan.FromMilliseconds(ms), editorRow.Num);
                }
                catch { }

                SetSubtitlesToEditorAndKeepSubtitleNumber(subtitles);
                SetFormTitle(true);
            }
        }

        #endregion

        #region Add Time

        private void btnAddTime_Click(object sender, EventArgs e)
        {
            EditorRow editorRow = GetSelectedEditorRow();
            if (editorRow == null)
                return;

            try
            {
                subtitles.AddTime(diffTimePicker.DiffValue, editorRow.Num);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Add Time Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SetSubtitlesToEditorAndKeepSubtitleNumber(subtitles);
            SetFormTitle(true);
        }

        #endregion

        #region Adjust Timing

        private void btnAdjustTiming_Click(object sender, EventArgs e)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            string initialDirectory = Path.GetDirectoryName(filePath);
            DateTime firstShow = subtitles[0].Show;
            DateTime lastShow = subtitles[subtitles.Count - 1].Show;

            var dialog = new AdjustTimingForm(initialDirectory, firstShow, lastShow);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (dialog.X1 != dialog.X2 || dialog.Y1 != dialog.Y2)
                {
                    subtitles.AdjustTiming(dialog.X1, dialog.X2, dialog.Y1, dialog.Y2);
                    SetSubtitlesToEditorAndKeepSubtitleNumber(subtitles);
                    SetFormTitle(true);
                }
            }
        }

        #endregion

        #region Search and Replace

        private void SubtitlesCleanerEditorForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && (e.KeyCode == Keys.F || e.KeyCode == Keys.H))
            {
                SearchAndReplace();
            }
            else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.F3)
            {
                if (searchAndReplaceDialog != null)
                    FindPrevious(searchAndReplaceDialog.GetFind());
            }
            else if (e.KeyCode == Keys.F3)
            {
                if (searchAndReplaceDialog != null)
                    FindNext(searchAndReplaceDialog.GetFind());
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                Save(false);
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.G)
            {
                GoToSubtitle();
            }
        }

        private void btnSearchAndReplace_Click(object sender, EventArgs e)
        {
            SearchAndReplace();
        }

        private SearchAndReplaceForm searchAndReplaceDialog;

        private void SearchAndReplace()
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            if (searchAndReplaceDialog == null)
            {
                searchAndReplaceDialog = new SearchAndReplaceForm();
                searchAndReplaceDialog.FindNext += (sender, find) => FindNext(find);
                searchAndReplaceDialog.FindPrevious += (sender, find) => FindPrevious(find);
                searchAndReplaceDialog.ReplaceNext += (sender, find) => ReplaceNext(find);
                searchAndReplaceDialog.ReplaceAll += (sender, find) => ReplaceAll(find);
            }

            searchAndReplaceDialog.ShowDialog(this);
        }

        private bool isIncludeSelectedRowInSearch = true;

        private void FindNext(Find find)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            int fromIndex = 0;

            DataGridViewRow row = GetSelectedEditorGVRow();
            if (row != null)
                fromIndex = row.Index + (isIncludeSelectedRowInSearch ? 0 : 1);

            for (int i = fromIndex; i < lstEditor.Rows.Count; i++)
            {
                if (IsFound(i, find))
                {
                    SelectEditorRow(i);

                    isIncludeSelectedRowInSearch = false;

                    break;
                }
            }
        }

        private void FindPrevious(Find find)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            int fromIndex = lstEditor.Rows.Count - 1;

            DataGridViewRow row = GetSelectedEditorGVRow();
            if (row != null)
                fromIndex = row.Index + (isIncludeSelectedRowInSearch ? 0 : -1);

            for (int i = fromIndex; i >= 0; i--)
            {
                if (IsFound(i, find))
                {
                    SelectEditorRow(i);

                    isIncludeSelectedRowInSearch = false;

                    break;
                }
            }
        }

        private void ReplaceNext(FindAndReplace find)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            int fromIndex = 0;

            DataGridViewRow row = GetSelectedEditorGVRow();
            if (row != null)
                fromIndex = row.Index + (isIncludeSelectedRowInSearch ? 0 : 1);

            for (int i = fromIndex; i < lstEditor.Rows.Count; i++)
            {
                if (IsFoundAndReplaced(i, find))
                {
                    Subtitle subtitle = subtitles[i];

                    EditorRow editorRow = GetEditorRowAt(i);

                    SetSubtitleToEditor(editorRow, subtitle);

                    SetSubtitlesErrors();

                    SelectEditorRow(i);

                    isIncludeSelectedRowInSearch = false;

                    SetFormTitle(true);

                    break;
                }
            }
        }

        private void ReplaceAll(FindAndReplace find)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            int fromIndex = 0;

            DataGridViewRow row = GetSelectedEditorGVRow();
            if (row != null)
                fromIndex = row.Index + (isIncludeSelectedRowInSearch ? 0 : 1);

            bool isFoundAndReplaced = false;
            int lastIndexFoundAndReplaced = -1;

            for (int i = fromIndex; i < lstEditor.Rows.Count; i++)
            {
                if (IsFoundAndReplaced(i, find))
                {
                    isFoundAndReplaced = true;
                    lastIndexFoundAndReplaced = i;

                    Subtitle subtitle = subtitles[i];

                    EditorRow editorRow = GetEditorRowAt(i);

                    SetSubtitleToEditor(editorRow, subtitle);
                }
            }

            if (isFoundAndReplaced)
            {
                SetSubtitlesErrors();

                SelectEditorRow(lastIndexFoundAndReplaced);

                isIncludeSelectedRowInSearch = false;

                SetFormTitle(true);
            }
        }

        private bool IsFound(int index, Find find)
        {
            bool isFound = false;
            bool isReplaced = false;
            FindAndReplaceSubtitle(index, find, null, ref isFound, ref isReplaced);
            return isFound;
        }

        private bool IsFoundAndReplaced(int index, FindAndReplace find)
        {
            bool isFound = false;
            bool isReplaced = false;
            FindAndReplaceSubtitle(index, find, find.Replace ?? string.Empty, ref isFound, ref isReplaced);
            return isFound && isReplaced;
        }

        private void FindAndReplaceSubtitle(int index, Find find, string replace, ref bool isFound, ref bool isReplaced)
        {
            Subtitle subtitle = subtitles[index];

            Regex regex = new Regex(
                string.Format("{1}{0}{1}", Regex.Escape(find.Search), (find.MatchWholeWord ? "\\b" : string.Empty)),
                (find.MatchCase == false ? RegexOptions.IgnoreCase : RegexOptions.None)
            );

            for (int i = 0; i < subtitle.Lines.Count; i++)
            {
                string line = subtitle.Lines[i];

                if (regex.IsMatch(line))
                {
                    isFound = true;

                    if (replace != null)
                    {
                        string newLine = regex.Replace(line, replace);
                        if (line != newLine)
                        {
                            subtitle.Lines[i] = newLine;
                            isReplaced = true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (isFound && isReplaced)
                subtitle.CheckSubtitle(cleanHICaseInsensitive);
        }

        #endregion

        #region Subtitles TextBox

        private void txtSubtitle_LeaveWithChangedText(object sender, Tuple<EditorRow, string> e)
        {
            ChangeSubtitleText(e.Item1, e.Item2);
        }

        private void ChangeSubtitleText(EditorRow editorRow, string text)
        {
            Subtitle subtitle = subtitles[editorRow.Num - 1];

            subtitle.Lines = (text ?? string.Empty).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            subtitle.CheckSubtitle(cleanHICaseInsensitive);

            SetSubtitleToEditor(editorRow, subtitle);

            SetSubtitlesErrors();

            SetFormTitle(true);
        }

        private void SetSubtitleToEditor(EditorRow editorRow, Subtitle subtitle)
        {
            editorRow.Text = subtitle.ToStringWithPipe();
            editorRow.Lines = subtitle.ToString();
            editorRow.SubtitleError = subtitle.SubtitleError;

            Subtitle cleanSubtitle = SubtitlesHelper.CleanSubtitles(subtitle.Clone() as Subtitle, cleanHICaseInsensitive, false);
            editorRow.CleanText = (cleanSubtitle != null ? cleanSubtitle.ToStringWithPipe() : string.Empty);
            editorRow.CleanLines = (cleanSubtitle != null ? cleanSubtitle.ToString() : string.Empty);
            txtCleanSubtitle.Text = editorRow.CleanLines;

            SetLineLengths();
        }

        #endregion

        #region Fix Text

        private void btnFixAndAdvance_Click(object sender, EventArgs e)
        {
            int num = FixText();
            if (num != -1)
            {
                ErrorRow errorRow = SelectClosestErrorRow(num);
                if (errorRow != null)
                {
                    SelectEditorRow(errorRow.Num - 1);
                }
                else
                {
                    int index = num - 1;
                    if (index > lstEditor.Rows.Count - 1)
                        SelectEditorRow(lstEditor.Rows.Count - 1);
                }
            }
        }

        private void btnAdvance_Click(object sender, EventArgs e)
        {
            EditorRow editorRow = GetSelectedEditorRow();
            if (editorRow == null)
                return;

            int num = editorRow.Num + 1;
            ErrorRow errorRow = SelectClosestErrorRow(num);
            if (errorRow != null)
                SelectEditorRow(errorRow.Num - 1);
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            int num = FixText();
            if (num != -1)
            {
                int index = num - 1;
                if (0 <= index && index <= lstEditor.Rows.Count - 1)
                    SelectEditorRow(index);
                else if (index > lstEditor.Rows.Count - 1)
                    SelectEditorRow(lstEditor.Rows.Count - 1);
            }
        }

        private int FixText()
        {
            EditorRow editorRow = GetSelectedEditorRow();
            if (editorRow == null)
                return -1;

            if (editorRow.SubtitleError.IsSet(SubtitleError.Non_Subtitle) ||
                editorRow.SubtitleError.IsSet(SubtitleError.Empty_Line) ||
                string.IsNullOrEmpty(txtCleanSubtitle.Text))
            {
                DeleteSubtitle(editorRow);
                (lstEditor.DataSource as BindingList<EditorRow>).Remove(editorRow);
            }
            else
            {
                txtSubtitle.Text = txtCleanSubtitle.Text;
                ChangeSubtitleText(editorRow, txtCleanSubtitle.Text);
            }

            return editorRow.Num;
        }

        #endregion

        #region Time Calculator

        private void btnTimeCalculator_Click(object sender, EventArgs e)
        {
            var dialog = new TimeCalculatorForm();
            dialog.Load += (object sender1, EventArgs e1) =>
            {
                EditorRow editorRow = GetSelectedEditorRow();
                if (editorRow == null)
                    return;

                dialog.DiffValue1 = editorRow.ShowValue;
            };
            dialog.Show(this);
        }

        #endregion

        #region lstEditor Context Menu

        private DataGridView.HitTestInfo lstEditor_hitTestInfo;

        private void lstEditor_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitTestInfo = lstEditor.HitTest(e.X, e.Y);
                if (hitTestInfo != null && hitTestInfo.Type == DataGridViewHitTestType.Cell)
                {
                    lstEditor_hitTestInfo = hitTestInfo;
                    contextMenuStripEditor.Show(lstEditor, new Point(e.X, e.Y));
                }
                else
                {
                    lstEditor_hitTestInfo = null;
                    contextMenuStripEditor.Hide();
                }
            }
        }

        private void copyTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstEditor_hitTestInfo == null)
                return;
            copyText(lstEditor_hitTestInfo.RowIndex);
            lstEditor_hitTestInfo = null;
        }

        private void copyText(int rowIndex)
        {
            EditorRow editorRow = GetEditorRowAt(rowIndex);
            string text = editorRow.Lines;

            if (string.IsNullOrEmpty(text))
                Clipboard.Clear();
            else
                Clipboard.SetText(text);
        }

        private void copyCleanTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstEditor_hitTestInfo == null)
                return;
            copyCleanText(lstEditor_hitTestInfo.RowIndex);
            lstEditor_hitTestInfo = null;
        }

        private void copyCleanText(int rowIndex)
        {
            EditorRow editorRow = GetEditorRowAt(rowIndex);
            string text = editorRow.CleanLines;

            if (string.IsNullOrEmpty(text))
                Clipboard.Clear();
            else
                Clipboard.SetText(text);
        }

        private void copySubtitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstEditor_hitTestInfo == null)
                return;
            copySubtitle(lstEditor_hitTestInfo.RowIndex);
            lstEditor_hitTestInfo = null;
        }

        private void copySubtitle(int rowIndex)
        {
            EditorRow editorRow = GetEditorRowAt(rowIndex);
            Subtitle subtitle = subtitles[editorRow.Num - 1];
            string[] lines = subtitle.ToLines(rowIndex);
            string text = string.Join(Environment.NewLine, lines);

            if (string.IsNullOrEmpty(text))
                Clipboard.Clear();
            else
                Clipboard.SetText(text);
        }

        private void copyCleanSubtitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstEditor_hitTestInfo == null)
                return;
            copyCleanSubtitle(lstEditor_hitTestInfo.RowIndex);
            lstEditor_hitTestInfo = null;
        }

        private void copyCleanSubtitle(int rowIndex)
        {
            EditorRow editorRow = GetEditorRowAt(rowIndex);
            Subtitle subtitle = subtitles[editorRow.Num - 1];
            Subtitle cleanSubtitle = SubtitlesHelper.CleanSubtitles(subtitle.Clone() as Subtitle, cleanHICaseInsensitive, false);

            string[] lines = null;
            if (cleanSubtitle != null)
            {
                lines = cleanSubtitle.ToLines(rowIndex);
            }
            else
            {
                lines = new string[3];
                lines[0] = (rowIndex + 1).ToString();
                lines[1] = subtitle.TimeToString();
                lines[2] = string.Empty;
            }

            string text = string.Join(Environment.NewLine, lines);

            if (string.IsNullOrEmpty(text))
                Clipboard.Clear();
            else
                Clipboard.SetText(text);
        }

        #endregion

        #region lstErrors Context Menu

        private DataGridView.HitTestInfo lstErrors_hitTestInfo;

        private void lstErrors_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitTestInfo = lstErrors.HitTest(e.X, e.Y);
                if (hitTestInfo != null && hitTestInfo.Type == DataGridViewHitTestType.Cell)
                {
                    lstErrors_hitTestInfo = hitTestInfo;
                    ErrorRow errorRow = GetErrorRowAt(lstErrors_hitTestInfo.RowIndex);
                    fixAllErrorsToolStripMenuItem.Text = "Fix All " + errorRow.Error + (errorRow.Error.EndsWith("Error") ? "s" : " Errors");
                    contextMenuStripErrors.Show(lstErrors, new Point(e.X, e.Y));
                }
                else
                {
                    lstErrors_hitTestInfo = null;
                    contextMenuStripErrors.Hide();
                }
            }
        }

        private void fixErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstErrors_hitTestInfo == null)
                return;
            FixError(lstErrors_hitTestInfo.RowIndex);
            lstErrors_hitTestInfo = null;
        }

        private void FixError(int rowIndex)
        {
            SelectErrorRow(rowIndex);
            ErrorRow selectedErrorRow = GetSelectedErrorRow();
            if (selectedErrorRow == null)
                return;
            SelectEditorRow(selectedErrorRow.Num - 1);
            FixText();
            SelectClosestErrorRow(selectedErrorRow.Num);
        }

        private void fixAllErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstErrors_hitTestInfo == null)
                return;
            FixAllErrors(lstErrors_hitTestInfo.RowIndex);
            lstErrors_hitTestInfo = null;
        }

        private void FixAllErrors(int rowIndex)
        {
            this.Cursor = Cursors.WaitCursor;

            ErrorRow selectedErrorRow = GetErrorRowAt(rowIndex);
            SubtitleError selectedSubtitleError = selectedErrorRow.SubtitleError;

            var errorRowNums = lstErrors.Rows
                .Cast<DataGridViewRow>()
                .Select(row => row.DataBoundItem as ErrorRow)
                .Where(errorRow => errorRow.SubtitleError == selectedSubtitleError)
                .Select(errorRow => errorRow.Num)
                .ToArray();

            var rows = lstEditor.Rows
                .Cast<DataGridViewRow>()
                .Select(row => new { GVEditorRow = row, EditorRow = row.DataBoundItem as EditorRow })
                .Join(
                    errorRowNums,
                    item => item.EditorRow.Num,
                    num => num,
                    (item, num) => new
                    {
                        item.GVEditorRow,
                        item.EditorRow,
                        Subtitle = subtitles[item.EditorRow.Num - 1],
                        IsToDeleteSubtitle =
                            item.EditorRow.SubtitleError.IsSet(SubtitleError.Non_Subtitle) ||
                            item.EditorRow.SubtitleError.IsSet(SubtitleError.Empty_Line) ||
                            string.IsNullOrEmpty(item.EditorRow.CleanLines)
                    }
                )
                .OrderBy(item => item.EditorRow.Num)
                .ToArray();

            foreach (var item in rows.Where(r => r.IsToDeleteSubtitle == false))
            {
                item.Subtitle.Lines = (item.EditorRow.CleanLines ?? string.Empty).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                item.Subtitle.SubtitleError = SubtitleError.None;

                item.EditorRow.Text = item.EditorRow.CleanText = item.Subtitle.ToStringWithPipe();
                item.EditorRow.Lines = item.EditorRow.CleanLines = item.Subtitle.ToString();
                item.EditorRow.SubtitleError = item.Subtitle.SubtitleError;
            }

            var toDeleteGVEditorRows =
                rows.Where(r => r.IsToDeleteSubtitle).Select(item => item.GVEditorRow)
                .Distinct()
                .OrderBy(r => r.Index)
                .ToArray();

            var toDeleteSubtitles =
                rows.Where(r => r.IsToDeleteSubtitle).Select(item => item.Subtitle)
                .Distinct()
                .OrderBy(s => s.Show)
                .ToArray();

            int startIndex = -1;
            if (toDeleteGVEditorRows.Length > 0)
                startIndex = toDeleteGVEditorRows[0].Index;

            foreach (var row in toDeleteGVEditorRows)
                lstEditor.Rows.Remove(row);

            foreach (var subtitle in toDeleteSubtitles)
                subtitles.Remove(subtitle);

            if (startIndex != -1)
            {
                for (int index = startIndex; index < lstEditor.Rows.Count; index++)
                {
                    EditorRow er = GetEditorRowAt(index);
                    er.Num = index + 1;
                }
            }

            SetSubtitlesErrors();

            if (lstErrors.Rows != null && lstErrors.Rows.Count > 0)
            {
                DataGridViewRow row = lstErrors.Rows[0];
                SelectGVRow(row);
                ErrorRow errorRow = row.DataBoundItem as ErrorRow;
                SelectEditorRow(errorRow.Num - 1);
            }
            else
            {
                DataGridViewRow row = GetSelectedEditorGVRow();
                if (row == null && lstEditor.Rows.Count > 0)
                    SelectEditorRow(0);
            }

            SelectionChanged();

            SetFormTitle(true);

            this.Cursor = Cursors.Default;
        }

        #endregion

        #region Go To Subtitle

        private GoToSubtitleForm goToSubtitleDialog;

        private void GoToSubtitle()
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            if (goToSubtitleDialog == null)
                goToSubtitleDialog = new GoToSubtitleForm();

            if (goToSubtitleDialog.ShowDialog(this) == DialogResult.OK)
            {
                int num = goToSubtitleDialog.GetSubtitleNumber();
                if (num != -1)
                {
                    int index = num - 1;
                    if (0 <= index && index <= lstEditor.Rows.Count - 1)
                    {
                        SelectEditorRow(index);
                        SelectErrorRowByNum(num);
                    }
                }
            }
        }

        #endregion

        #region Quick Actions

        private void btnQuickActions_Click(object sender, EventArgs e)
        {
            QuickActions();
        }

        private void QuickActions()
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            List<QuickAction> quickActions = new List<QuickAction>()
            {
                new QuickAction("Remove Empty Lines", "", RemoveEmptyLines)
                ,new QuickAction("Remove Non-Subtitles", "Synced By", RemoveNonSubtitles)
                ,new QuickAction("Fix Dialog", "-Hello. => - Hello.", FixDialog)
                ,new QuickAction("Remove Full Lines Hearing-Impaired", "MAN:", RemoveFullLinesHearingImpaired)
                ,new QuickAction("Fix Hearing-Impaired", "[singing]", FixHearingImpaired)
                ,new QuickAction("Remove Consecutive Italics", "<i>L1</i>|<i>L2</i> => <i>L1|L2</i>", RemoveConsecutiveItalics)
                ,new QuickAction("Fix Three Dots ...", "-- => ...  … => ...", FixThreeDots)
                ,new QuickAction("Fix Notes ♪", "♫¶* => ♪  j\" => ♪", FixNotes)
                ,new QuickAction("Fix Malformed Letters", "I-l => H  L\\/l => M  L/V => W", FixMalformedLetters)
                ,new QuickAction("Remove ASSA Tags", "{\\an1}", RemoveASSATags)
                ,new QuickAction("Remove Space After 1", "1 987 => 1987", RemoveSpaceAfterOne)
                ,new QuickAction("Fix Ordinal Numbers", "1 st => 1st", FixOrdinalNumbers)
                ,new QuickAction("Add Space After Dot", "First.Second => First. Second", AddSpaceAfterDot)
                ,new QuickAction("Add Space After Comma", "One,Two => One, Two", AddSpaceAfterComma)
                ,new QuickAction("Add Space After Three Dots", "Text...Text => Text... Text", AddSpaceAfterThreeDot)
                ,new QuickAction("Fix Non-Ansi Chars", "ﬁ => fi", FixNonAnsiChars)
                ,new QuickAction("Fix Encoded HTML", "&amp; => &  &quot; => \"", FixEncodedHTML)
                ,new QuickAction("Fix Contractions", "I'’m => I'm  sayin ' => sayin'", FixContractions)
                ,new QuickAction("Fix I And L Errors", "I'II => I'll  L'm => I'm", FixIAndLErrors)
                ,new QuickAction("Fix O And 0 Errors", "0ver => Over  1O => 10", FixOAnd0Errors)
                ,new QuickAction("Fix Merged Words", "ofthe => of the", FixMergedWords)
                ,new QuickAction("Add Dot After Abbreviation", "Mr => Mr.  Dr => Dr.  St => St.", AddDotAfterAbbreviation)
                ,new QuickAction("Fix I And 1 Errors", "I6 => 16  1 can => I can", FixIAnd1Errors)
                ,new QuickAction("Merge Lines", "", MergeLines)
            };

            List<Subtitle> newSubtitles = subtitles.Clone();
            var dialog = new QuickActionsForm(this.filePath, newSubtitles, quickActions);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                newSubtitles.CheckSubtitles(cleanHICaseInsensitive);
                SetSubtitlesToEditorAndKeepSubtitleNumber(newSubtitles);
                SetFormTitle(true);
            }
        }

        private QuickActionResult RemoveEmptyLines(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.EmptyLine, isPreview, isRemoveEmptyLines: true);
        }

        private QuickActionResult RemoveNonSubtitles(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.NonSubtitle, isPreview, isRemoveNonSubtitles: true);
        }

        private QuickActionResult FixDialog(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(
                subtitles,
                new QuickActionCleanHandler[] {
                    SubtitlesHelper.CleanDialogSingleLine,
                    SubtitlesHelper.CleanDialogMultipleLines
                },
                SubtitlesHelper.MissingSpaces.ByGroup("Space After Dialog Dash"),
                new QuickActionCleanHandler[] {
                    SubtitlesHelper.CleanMissingDialogDashSingleLine,
                    SubtitlesHelper.CleanMissingDialogDashMultipleLines
                },
                isPreview
            );
        }

        private QuickActionResult RemoveFullLinesHearingImpaired(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(
                subtitles,
                null,
                SubtitlesHelper.HearingImpairedFullLine.ByGroup("HI Full Line"),
                new QuickActionCleanHandler[] { SubtitlesHelper.CleanHearingImpairedMultipleLines },
                isPreview
            );
        }

        private QuickActionResult FixHearingImpaired(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(
                subtitles,
                new QuickActionCleanHandler[] { SubtitlesHelper.CleanHIPrefixWithoutDialogDash },
                SubtitlesHelper.HearingImpairedFullLine
                    .Concat(SubtitlesHelper.HearingImpaired)
                    .Concat(SubtitlesHelper.RedundantItalics)
                    .Concat(SubtitlesHelper.TrimSpaces)
                    .ToArray(),
                new QuickActionCleanHandler[] {
                    SubtitlesHelper.CleanHearingImpairedMultipleLines,
                    SubtitlesHelper.CleanHIPrefixSingleLine,
                    SubtitlesHelper.CleanHIPrefixMultipleLines
                },
                isPreview
            );
        }

        private QuickActionResult RemoveConsecutiveItalics(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.CleanRedundantItalicsMultipleLines, isPreview);
        }

        private QuickActionResult FixThreeDots(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.FindAndReplaceRules.ByGroup("Three Dots"), isPreview);
        }

        private QuickActionResult FixNotes(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(
                subtitles,
                new QuickActionCleanHandler[] { SubtitlesHelper.CleanLyricsMultipleLines },
                SubtitlesHelper.Notes,
                new QuickActionCleanHandler[] { SubtitlesHelper.CleanNotesMultipleLines },
                isPreview
            );
        }

        private QuickActionResult FixMalformedLetters(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.MalformedLetters, isPreview);
        }

        private QuickActionResult RemoveASSATags(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.ASSATags, isPreview);
        }

        private QuickActionResult RemoveSpaceAfterOne(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.RedundantSpaces.ByGroup("Space After 1"), isPreview);
        }

        private QuickActionResult FixOrdinalNumbers(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.RedundantSpaces.ByGroup("Ordinal Numbers"), isPreview);
        }

        private QuickActionResult AddSpaceAfterDot(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.MissingSpaces.ByGroup("Space After Dot"), isPreview);
        }

        private QuickActionResult AddSpaceAfterComma(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.MissingSpaces.ByGroup("Space After Comma"), isPreview);
        }

        private QuickActionResult AddSpaceAfterThreeDot(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.MissingSpaces.ByGroup("Space After Three Dot"), isPreview);
        }

        private QuickActionResult FixNonAnsiChars(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.NonAnsiChars, isPreview);
        }

        private QuickActionResult FixEncodedHTML(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.EncodedHTML, isPreview);
        }

        private QuickActionResult FixContractions(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.Contractions, isPreview);
        }

        private QuickActionResult FixIAndLErrors(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.I_And_L, isPreview);
        }

        private QuickActionResult FixOAnd0Errors(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.O_And_0, isPreview);
        }

        private QuickActionResult FixMergedWords(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.MergedWords, isPreview);
        }

        private QuickActionResult AddDotAfterAbbreviation(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.OCRErrors.ByGroup("Dot After Abbreviation"), isPreview);
        }

        private QuickActionResult FixIAnd1Errors(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.OCRErrors.ByGroup("I And 1"), isPreview);
        }

        private QuickActionResult MergeLines(List<Subtitle> subtitles, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, SubtitlesHelper.CleanMergeLines, isPreview);
        }

        private QuickActionResult QuickActionFindAndReplace(
            List<Subtitle> subtitles,
            SubtitlesCleanerLibrary.FindAndReplace[] rules,
            bool isPreview,
            bool isRemoveEmptyLines = false,
            bool isRemoveNonSubtitles = false)
        {
            return QuickActionFindAndReplace(subtitles, null, rules, null, isPreview, isRemoveEmptyLines, isRemoveNonSubtitles);
        }

        private QuickActionResult QuickActionFindAndReplace(List<Subtitle> subtitles, QuickActionCleanHandler cleaner, bool isPreview)
        {
            return QuickActionFindAndReplace(subtitles, new QuickActionCleanHandler[] { cleaner }, null, null, isPreview);
        }

        private QuickActionResult QuickActionFindAndReplace(
            List<Subtitle> subtitles,
            QuickActionCleanHandler[] preCleaners,
            SubtitlesCleanerLibrary.FindAndReplace[] rules,
            QuickActionCleanHandler[] postCleaners,
            bool isPreview,
            bool isRemoveEmptyLines = false,
            bool isRemoveNonSubtitles = false)
        {
            try
            {
                if (isPreview)
                {
                    List<PreviewSubtitle> preview = new List<PreviewSubtitle>();

                    for (int i = subtitles.Count - 1; i >= 0 && i < subtitles.Count; i--)
                    {
                        Subtitle subtitle = (Subtitle)subtitles[i].Clone();

                        bool isSubtitlesChanged = false;

                        if (isRemoveEmptyLines && subtitle.Lines.Count == 0)
                        {
                            isSubtitlesChanged = true;
                        }
                        else
                        {
                            #region Pre Cleaners

                            if (preCleaners != null && preCleaners.Length > 0)
                            {
                                List<string> cleanLines = new List<string>(subtitle.Lines);
                                if (cleanLines != null && cleanLines.Count > 0)
                                {
                                    foreach (var cleaner in preCleaners)
                                    {
                                        cleanLines = cleaner(cleanLines, cleanHICaseInsensitive);
                                        if (cleanLines == null || cleanLines.Count == 0)
                                            break;
                                    }
                                }

                                if (cleanLines != null && cleanLines.Count > 0)
                                {
                                    for (int k = 0; k < cleanLines.Count; k++)
                                        cleanLines[k] = cleanLines[k].Trim();
                                }

                                if (cleanLines == null || (cleanLines.Count == 0 && subtitle.Lines.Count > 0))
                                {
                                    subtitle.Lines.Clear();
                                    isSubtitlesChanged = true;
                                }
                                else
                                {
                                    bool anyChanges =
                                        cleanLines.Count != subtitle.Lines.Count ||
                                        cleanLines.Zip(subtitle.Lines, (cl, l) => cl != l).Any(isChanged => isChanged);

                                    if (anyChanges)
                                    {
                                        subtitle.Lines = cleanLines;
                                        isSubtitlesChanged = true;
                                    }
                                }
                            }

                            #endregion

                            #region Rules

                            if (rules != null && rules.Length > 0)
                            {
                                for (int k = subtitle.Lines.Count - 1; k >= 0 && k < subtitle.Lines.Count; k--)
                                {
                                    string line = subtitle.Lines[k];
                                    string cleanLine = line;

                                    foreach (var rule in rules)
                                    {
                                        cleanLine = rule.CleanLine(cleanLine, cleanHICaseInsensitive);

                                        if (string.IsNullOrEmpty(cleanLine) || (isRemoveNonSubtitles && line != cleanLine))
                                            break;
                                    }

                                    cleanLine = cleanLine.Trim();

                                    if (isRemoveNonSubtitles && line != cleanLine)
                                    {
                                        subtitle.Lines.Clear();
                                        isSubtitlesChanged = true;
                                        break;
                                    }
                                    else if (string.IsNullOrEmpty(cleanLine))
                                    {
                                        subtitle.Lines.RemoveAt(k);
                                        isSubtitlesChanged = true;
                                        if (subtitle.Lines.Count == 0)
                                            break;
                                    }
                                    else if (line != cleanLine)
                                    {
                                        subtitle.Lines[k] = cleanLine;
                                        isSubtitlesChanged = true;
                                    }
                                }
                            }

                            #endregion

                            #region Post Cleaners

                            if (postCleaners != null && postCleaners.Length > 0)
                            {
                                List<string> cleanLines = new List<string>(subtitle.Lines);
                                if (cleanLines != null && cleanLines.Count > 0)
                                {
                                    foreach (var cleaner in postCleaners)
                                    {
                                        cleanLines = cleaner(cleanLines, cleanHICaseInsensitive);
                                        if (cleanLines == null || cleanLines.Count == 0)
                                            break;
                                    }
                                }

                                if (cleanLines != null && cleanLines.Count > 0)
                                {
                                    for (int k = 0; k < cleanLines.Count; k++)
                                        cleanLines[k] = cleanLines[k].Trim();
                                }

                                if (cleanLines == null || (cleanLines.Count == 0 && subtitle.Lines.Count > 0))
                                {
                                    subtitle.Lines.Clear();
                                    isSubtitlesChanged = true;
                                }
                                else
                                {
                                    bool anyChanges =
                                        cleanLines.Count != subtitle.Lines.Count ||
                                        cleanLines.Zip(subtitle.Lines, (cl, l) => cl != l).Any(isChanged => isChanged);

                                    if (anyChanges)
                                    {
                                        subtitle.Lines = cleanLines;
                                        isSubtitlesChanged = true;
                                    }
                                }
                            }

                            #endregion
                        }

                        if (isSubtitlesChanged)
                        {
                            preview.Add(new PreviewSubtitle()
                            {
                                SubtitleNumber = i + 1,
                                OriginalSubtitle = (Subtitle)subtitles[i].Clone(),
                                CleanedSubtitle = subtitle
                            });
                        }
                    }

                    preview.Sort((x, y) => x.SubtitleNumber.CompareTo(y.SubtitleNumber));

                    return new QuickActionResult()
                    {
                        Succeeded = true,
                        Preview = preview
                    };
                }
                else
                {
                    int countSubtitlesChanged = 0;
                    int countLinesRemoved = 0;
                    int countSubtitlesRemoved = 0;

                    for (int i = subtitles.Count - 1; i >= 0 && i < subtitles.Count; i--)
                    {
                        Subtitle subtitle = subtitles[i];

                        bool isSubtitlesChanged = false;

                        if (isRemoveEmptyLines && subtitle.Lines.Count == 0)
                        {
                            subtitles.RemoveAt(i);
                            isSubtitlesChanged = true;
                            countSubtitlesRemoved++;
                        }
                        else
                        {
                            #region Pre Cleaners

                            if (preCleaners != null && preCleaners.Length > 0)
                            {
                                List<string> cleanLines = new List<string>(subtitle.Lines);
                                if (cleanLines != null && cleanLines.Count > 0)
                                {
                                    foreach (var cleaner in preCleaners)
                                    {
                                        cleanLines = cleaner(cleanLines, cleanHICaseInsensitive);
                                        if (cleanLines == null || cleanLines.Count == 0)
                                            break;
                                    }
                                }

                                if (cleanLines != null && cleanLines.Count > 0)
                                {
                                    for (int k = 0; k < cleanLines.Count; k++)
                                        cleanLines[k] = cleanLines[k].Trim();
                                }

                                if (cleanLines == null || (cleanLines.Count == 0 && subtitle.Lines.Count > 0))
                                {
                                    subtitles.RemoveAt(i);
                                    isSubtitlesChanged = true;
                                    countSubtitlesRemoved++;
                                    countLinesRemoved += subtitle.Lines.Count;
                                }
                                else
                                {
                                    bool anyChanges =
                                        cleanLines.Count != subtitle.Lines.Count ||
                                        cleanLines.Zip(subtitle.Lines, (cl, l) => cl != l).Any(isChanged => isChanged);

                                    if (anyChanges)
                                    {
                                        subtitle.Lines = cleanLines;
                                        isSubtitlesChanged = true;
                                    }
                                }
                            }

                            #endregion

                            #region Rules

                            if (rules != null && rules.Length > 0)
                            {
                                for (int k = subtitle.Lines.Count - 1; k >= 0 && k < subtitle.Lines.Count; k--)
                                {
                                    string line = subtitle.Lines[k];
                                    string cleanLine = line;

                                    foreach (var rule in rules)
                                    {
                                        cleanLine = rule.CleanLine(cleanLine, cleanHICaseInsensitive);

                                        if (string.IsNullOrEmpty(cleanLine) || (isRemoveNonSubtitles && line != cleanLine))
                                            break;
                                    }

                                    cleanLine = cleanLine.Trim();

                                    if (isRemoveNonSubtitles && line != cleanLine)
                                    {
                                        subtitles.RemoveAt(i);
                                        isSubtitlesChanged = true;
                                        countSubtitlesRemoved++;
                                        break;
                                    }
                                    else if (string.IsNullOrEmpty(cleanLine))
                                    {
                                        subtitle.Lines.RemoveAt(k);
                                        isSubtitlesChanged = true;
                                        countLinesRemoved++;
                                        if (subtitle.Lines.Count == 0)
                                        {
                                            subtitles.RemoveAt(i);
                                            countSubtitlesRemoved++;
                                            break;
                                        }
                                    }
                                    else if (line != cleanLine)
                                    {
                                        subtitle.Lines[k] = cleanLine;
                                        isSubtitlesChanged = true;
                                    }
                                }
                            }

                            #endregion

                            #region Post Cleaners

                            if (postCleaners != null && postCleaners.Length > 0)
                            {
                                List<string> cleanLines = new List<string>(subtitle.Lines);
                                if (cleanLines != null && cleanLines.Count > 0)
                                {
                                    foreach (var cleaner in postCleaners)
                                    {
                                        cleanLines = cleaner(cleanLines, cleanHICaseInsensitive);
                                        if (cleanLines == null || cleanLines.Count == 0)
                                            break;
                                    }
                                }

                                if (cleanLines != null && cleanLines.Count > 0)
                                {
                                    for (int k = 0; k < cleanLines.Count; k++)
                                        cleanLines[k] = cleanLines[k].Trim();
                                }

                                if (cleanLines == null || (cleanLines.Count == 0 && subtitle.Lines.Count > 0))
                                {
                                    subtitles.RemoveAt(i);
                                    isSubtitlesChanged = true;
                                    countSubtitlesRemoved++;
                                    countLinesRemoved += subtitle.Lines.Count;
                                }
                                else
                                {
                                    bool anyChanges =
                                        cleanLines.Count != subtitle.Lines.Count ||
                                        cleanLines.Zip(subtitle.Lines, (cl, l) => cl != l).Any(isChanged => isChanged);

                                    if (anyChanges)
                                    {
                                        subtitle.Lines = cleanLines;
                                        isSubtitlesChanged = true;
                                    }
                                }
                            }

                            #endregion
                        }

                        if (isSubtitlesChanged)
                            countSubtitlesChanged++;
                    }

                    var result = new QuickActionResult()
                    {
                        Succeeded = true,
                        CountSubtitlesChanged = countSubtitlesChanged,
                        CountLinesRemoved = countLinesRemoved,
                        CountSubtitlesRemoved = countSubtitlesRemoved
                    };

                    if (isRemoveEmptyLines)
                        result.ResultMessage = string.Format("Removed {0} line{1}", countLinesRemoved, countLinesRemoved == 1 ? "" : "s");
                    else if (isRemoveNonSubtitles)
                        result.ResultMessage = string.Format("Removed {0} non-subtitle{1}", countSubtitlesRemoved, countSubtitlesRemoved == 1 ? "" : "s");
                    else
                        result.ResultMessage = string.Format("Fixed {0} subtitle{1}", countSubtitlesChanged, countSubtitlesChanged == 1 ? "" : "s");

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new QuickActionResult()
                {
                    Succeeded = false,
                    ResultMessage = ex.Message
                };
            }
        }

        #endregion
    }
}
