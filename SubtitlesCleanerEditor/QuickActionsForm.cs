using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SubtitlesCleanerLibrary;

namespace SubtitlesCleanerEditor
{
    public partial class QuickActionsForm : Form
    {
        private string filePath;
        private List<Subtitle> subtitles;

        public QuickActionsForm(string filePath, List<Subtitle> subtitles, List<QuickAction> quickActions)
        {
            InitializeComponent();

            this.filePath = filePath;
            this.subtitles = subtitles;

            lstQuickActions.AutoGenerateColumns = false;
            lstQuickActions.DataSource = new BindingList<QuickAction>(quickActions);
        }

        private List<DataGridViewButtonCell> disabledCells = new List<DataGridViewButtonCell>();

        private void lstQuickActions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            bool isFix = (e.ColumnIndex == 0);
            bool isPreview = (e.ColumnIndex == 4);

            if (isFix || isPreview)
            {
                DataGridViewButtonCell cell = lstQuickActions.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;

                if (disabledCells.Contains(cell) == false)
                {
                    QuickAction quickAction = lstQuickActions.Rows[e.RowIndex].DataBoundItem as QuickAction;

                    QuickActionResult result = quickAction.Do(subtitles, isPreview);

                    if (isFix)
                    {
                        if (result.Succeeded)
                        {
                            if (string.IsNullOrEmpty(result.ResultMessage))
                            {
                                quickAction.Result = "Fix succeeded";
                            }
                            else
                            {
                                quickAction.Result = result.ResultMessage;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(result.ResultMessage))
                                quickAction.Result = "Fix failed";
                            else
                                quickAction.Result = "Fix failed. " + result.ResultMessage;
                        }

                        disabledCells.Add(cell);
                    }
                    else if (isPreview)
                    {
                        if (result.Succeeded)
                        {
                            DataGridViewTextBoxCell actionCell = lstQuickActions.Rows[e.RowIndex].Cells[1] as DataGridViewTextBoxCell;
                            string action = actionCell.Value as string;
                            string preview = PreviewToString(result.Preview);
                            new QuickActionPreviewForm(filePath, action, preview).ShowDialog(this);
                        }
                    }
                }
            }
        }

        private string PreviewToString(List<PreviewSubtitle> preview)
        {
            StringBuilder sb = new StringBuilder();

            if (preview != null && preview.Count > 0)
            {
                foreach (var item in preview)
                {
                    sb.AppendLine(item.SubtitleNumber.ToString());
                    sb.AppendLine(item.OriginalSubtitle.TimeToString());

                    List<string> lines1 = item.OriginalSubtitle.Lines;
                    List<string> lines2 = item.CleanedSubtitle.Lines;
                    bool isOriginalSubtitleNotEmpty = (lines1.Count > 0);
                    bool isCleanedSubtitleNotEmpty = (lines2.Count > 0);

                    if (isOriginalSubtitleNotEmpty)
                    {
                        int maxLineLength1 = lines1.Max(l => l.Length);

                        for (int i = 0; i < lines1.Count; i++)
                        {
                            string line1 = lines1[i];
                            if (isCleanedSubtitleNotEmpty)
                            {
                                sb.Append(line1);
                                sb.Append(new String(' ', maxLineLength1 - line1.Length));
                                sb.Append(" | ");
                                if (i < lines2.Count)
                                {
                                    string line2 = lines2[i];
                                    sb.AppendLine(line2);
                                }
                                else
                                {
                                    sb.AppendLine();
                                }
                            }
                            else
                            {
                                sb.AppendLine(line1);
                            }
                        }

                        if (isCleanedSubtitleNotEmpty)
                        {
                            for (int i = lines1.Count; i < lines2.Count; i++)
                            {
                                sb.Append(new String(' ', maxLineLength1));
                                sb.Append(" | ");
                                sb.AppendLine(lines2[i]);
                            }
                        }
                    }

                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}
