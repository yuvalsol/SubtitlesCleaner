using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SubtitlesCL;

namespace SubtitlesEditor
{
    public partial class SubtitlesEditorForm : Form
    {
        #region Form

        public SubtitlesEditorForm(string[] args)
        {
            InitializeComponent();

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
                string initialFile = null;

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

                if (string.IsNullOrEmpty(initialFile) == false)
                    LoadFile(initialFile);
            }
        }

        private readonly bool isLoadTest = false;

        private void SubtitlesEditorForm_Load(object sender, EventArgs e)
        {
            if (isLoadTest)
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
        private string filePath;

        private void SetSubtitlesToEditor(List<Subtitle> subtitles)
        {
            this.subtitles = subtitles;
            lstErrors_SortedColumnIndex = 0;
            lstErrors_sortOrder = SortOrder.Ascending;

            if (subtitles != null)
            {

                lstEditor.DataSource = new BindingList<EditorRow>(subtitles.Select((subtitle, index) => new EditorRow()
                {
                    Num = index + 1,
                    ShowValue = subtitle.Show,
                    Show = subtitle.ShowToString(),
                    Text = subtitle.ToStringWithPipe(),
                    Lines = subtitle.ToString(),
                    SubtitleError = subtitle.SubtitleError
                }).ToList());
            }
            else
            {
                lstEditor.DataSource = new BindingList<EditorRow>(new List<EditorRow>());
            }
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

            SelectRow(index);
        }

        #endregion

        #region Load Subtitles File

        private static readonly bool cleanHICaseInsensitive = false;

        private void LoadFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            bool isSRT = string.Compare(extension, ".srt", true) == 0;
            if (isSRT)
            {
                subtitles = SubtitlesHelper.GetSubtitles(filePath);
                subtitles.CheckSubtitles(cleanHICaseInsensitive);
                originalSubtitles = subtitles.Clone();
                this.filePath = filePath;
                SetSubtitlesToEditor(subtitles);
                SetFormTitle(false);
            }
        }

        private void SetFormTitle(bool isDirty)
        {
            this.Text = Path.GetFileName(this.filePath) + (isDirty ? " *" : string.Empty);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                LoadFile(openFileDialog.FileName);
        }

        private void SubtitlesEditorForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void SubtitlesEditorForm_DragDrop(object sender, DragEventArgs e)
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

                    foreach (var error in allErrors)
                    {
                        if (error != SubtitleError.None)
                        {
                            errorRows.Add(new ErrorRow()
                            {
                                Num = editorRow.Num,
                                Error = error.ToString().Replace('_', ' ')
                            });
                        }
                    }
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
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
                if (subtitleError.HasFlag(error))
                    yield return error;
            }
        }

        #endregion

        #region Select Row

        private void SelectRow(int index)
        {
            DataGridViewRow row = lstEditor.Rows[index];
            SelectRow(row);
        }

        private void SelectRow(DataGridViewRow row)
        {
            row.Selected = true;
            row.Cells[0].Selected = true;
        }

        private DataGridViewRow GetRowAt(int index)
        {
            return lstEditor.Rows[index];
        }

        private EditorRow GetEditorRowAt(int index)
        {
            DataGridViewRow row = GetRowAt(index);
            return row.DataBoundItem as EditorRow;
        }

        private DataGridViewRow GetSelectedRow()
        {
            if (lstEditor.SelectedRows == null || lstEditor.SelectedRows.Count == 0)
                return null;
            return lstEditor.SelectedRows[0];
        }

        private EditorRow GetSelectedEditorRow()
        {
            DataGridViewRow row = GetSelectedRow();
            if (row != null)
                return row.DataBoundItem as EditorRow;
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

        #region Clear Subtitles Editor

        private void btnClear_Click(object sender, EventArgs e)
        {
            SetSubtitlesToEditor(null);
            originalSubtitles = null;
            filePath = null;
            txtSubtitle.Text = string.Empty;
            timePicker.Reset();
            this.Text = "Subtitles Editor";
        }

        #endregion

        #region Delete Subtitles

        private void lstEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                EditorRow editorRow = GetSelectedEditorRow();
                if (editorRow == null)
                    return;

                int index = editorRow.Num - 1;

                for (int i = index + 1; i < lstEditor.Rows.Count; i++)
                {
                    EditorRow er = GetEditorRowAt(i);
                    er.Num = i;
                }

                if (lstEditor.Rows.Count == 1)
                    txtSubtitle.Text = string.Empty;

                SetSubtitlesErrors();

                SetFormTitle(true);
            }
        }

        #endregion

        #region Subtitles Editor Selection Changed

        private void lstEditor_SelectionChanged(object sender, EventArgs e)
        {
            EditorRow editorRow = GetSelectedEditorRow();
            if (editorRow == null)
                return;

            txtSubtitle.Text = editorRow.Lines;
            timePicker.Value = editorRow.ShowValue;

            isIncludeSelectedRowInSearch = true;
        }

        #endregion

        #region Errors List

        private int lstErrors_SortedColumnIndex;
        private SortOrder lstErrors_sortOrder;

        private void sortErrors()
        {
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

            lstErrors.DataSource = null;
            lstErrors.DataSource = errorRows;
            lstErrors.Columns[lstErrors_SortedColumnIndex].HeaderCell.SortGlyphDirection = lstErrors_sortOrder;
        }

        private void lstErrors_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lstErrors_SortedColumnIndex == e.ColumnIndex)
            {
                if (lstErrors_sortOrder == SortOrder.Ascending)
                    lstErrors_sortOrder = SortOrder.Descending;
                else
                    lstErrors_sortOrder = SortOrder.Ascending;
            }
            else
            {
                lstErrors_SortedColumnIndex = e.ColumnIndex;
                lstErrors_sortOrder = SortOrder.Ascending;
            }

            sortErrors();
        }

        private void lstErrors_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lstErrors.CurrentRow.Selected)
            {
                if (lstErrors.SelectedRows == null || lstErrors.SelectedRows.Count == 0)
                    return;
                ErrorRow selectedErrorRow = lstErrors.SelectedRows[0].DataBoundItem as ErrorRow;

                SelectRow(selectedErrorRow.Num - 1);
            }
        }

        private void lstEditor_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            EditorRow editorRow = GetEditorRowAt(e.RowIndex);

            foreach (DataGridViewRow row in lstErrors.Rows)
            {
                ErrorRow errorRow = row.DataBoundItem as ErrorRow;
                if (errorRow.Num == editorRow.Num)
                {
                    SelectRow(row);
                    break;
                }
            }
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
                    File.WriteAllLines(backPath, originalSubtitles.ToLines(), Encoding.UTF8);
                    message = Path.GetFileName(backPath) + " saved" + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    message = "Failed to save " + Path.GetFileName(backPath) + Environment.NewLine + ex.Message + Environment.NewLine;
                }
            }

            try
            {
                File.WriteAllLines(filePath, subtitles.ToLines(), Encoding.UTF8);
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
                string message = string.Empty;

                try
                {
                    File.WriteAllLines(saveAsFileDialog.FileName, subtitles.ToLines(), Encoding.UTF8);
                    message = Path.GetFileName(saveAsFileDialog.FileName) + " saved";
                }
                catch (Exception ex)
                {
                    message = "Failed to save " + Path.GetFileName(saveAsFileDialog.FileName) + Environment.NewLine + ex.Message;
                }

                message = message.Trim();
                MessageBox.Show(this, message, "Subtitle File Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Move and Shift

        private void btnMoveTo_Click(object sender, EventArgs e)
        {
            EditorRow editorRow = GetSelectedEditorRow();
            if (editorRow == null)
                return;

            subtitles.MoveTo(timePicker.Value, editorRow.Num);
            SetSubtitlesToEditorAndKeepSubtitleNumber(subtitles);
            SetFormTitle(true);
        }

        private void timePicker_MillisecondsAdded(object sender, int ms)
        {
            if (chkShift.Checked)
            {
                EditorRow editorRow = GetSelectedEditorRow();
                if (editorRow == null)
                    return;

                subtitles.Shift(TimeSpan.FromMilliseconds(ms), editorRow.Num);
                SetSubtitlesToEditorAndKeepSubtitleNumber(subtitles);
                SetFormTitle(true);
            }
        }

        #endregion

        #region Adjust

        private void btnAdjust_Click(object sender, EventArgs e)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            string initialDirectory = Path.GetDirectoryName(filePath);
            DateTime firstShow = subtitles[0].Show;
            DateTime lastShow = subtitles[subtitles.Count - 1].Show;

            var dialog = new AdjustForm(initialDirectory, firstShow, lastShow);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                subtitles.Adjust(dialog.X1, dialog.X2, dialog.Y1, dialog.Y2);
                SetSubtitlesToEditorAndKeepSubtitleNumber(subtitles);
                SetFormTitle(true);
            }
        }

        #endregion

        #region Search and Replace

        private void SubtitlesEditorForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && (e.KeyCode == Keys.F || e.KeyCode == Keys.H))
            {
                SearchAndReplace();
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                Save(false);
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

            DataGridViewRow row = GetSelectedRow();
            if (row != null)
                fromIndex = row.Index + (isIncludeSelectedRowInSearch ? 0 : 1);

            for (int i = fromIndex; i < lstEditor.Rows.Count; i++)
            {
                if (IsFound(i, find))
                {
                    SelectRow(i);

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

            DataGridViewRow row = GetSelectedRow();
            if (row != null)
                fromIndex = row.Index + (isIncludeSelectedRowInSearch ? 0 : -1);

            for (int i = fromIndex; i >= 0; i--)
            {
                if (IsFound(i, find))
                {
                    SelectRow(i);

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

            DataGridViewRow row = GetSelectedRow();
            if (row != null)
                fromIndex = row.Index + (isIncludeSelectedRowInSearch ? 0 : 1);

            for (int i = fromIndex; i < lstEditor.Rows.Count; i++)
            {
                if (IsFoundAndReplaced(i, find))
                {
                    Subtitle subtitle = subtitles[i];

                    EditorRow editorRow = GetEditorRowAt(i);

                    editorRow.Text = subtitle.ToStringWithPipe();
                    editorRow.Lines = subtitle.ToString();
                    editorRow.SubtitleError = subtitle.SubtitleError;

                    SetSubtitlesErrors();

                    SelectRow(i);

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

            DataGridViewRow row = GetSelectedRow();
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

                    editorRow.Text = subtitle.ToStringWithPipe();
                    editorRow.Lines = subtitle.ToString();
                    editorRow.SubtitleError = subtitle.SubtitleError;
                }
            }

            if (isFoundAndReplaced)
            {
                SetSubtitlesErrors();

                SelectRow(lastIndexFoundAndReplaced);

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

            EditorRow editorRow = GetEditorRowAt(index);

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
            EditorRow editorRow = e.Item1;
            Subtitle subtitle = subtitles[editorRow.Num - 1];

            subtitle.Lines = (e.Item2 ?? string.Empty).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            subtitle.CheckSubtitle(cleanHICaseInsensitive);

            editorRow.Text = subtitle.ToStringWithPipe();
            editorRow.Lines = subtitle.ToString();
            editorRow.SubtitleError = subtitle.SubtitleError;

            SetSubtitlesErrors();

            SetFormTitle(true);
        }

        #endregion
    }
}
