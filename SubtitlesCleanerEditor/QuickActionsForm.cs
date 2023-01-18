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
        private List<Subtitle> subtitles;

        public QuickActionsForm(List<Subtitle> subtitles, List<QuickAction> quickActions)
        {
            InitializeComponent();

            this.subtitles = subtitles;

            lstQuickActions.AutoGenerateColumns = false;
            lstQuickActions.DataSource = new BindingList<QuickAction>(quickActions);
        }

        private List<DataGridViewButtonCell> disabledCells = new List<DataGridViewButtonCell>();

        private void lstQuickActions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridViewButtonCell cell = lstQuickActions.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;

                if (disabledCells.Contains(cell) == false)
                {
                    QuickAction quickAction = lstQuickActions.Rows[e.RowIndex].DataBoundItem as QuickAction;

                    QuickActionResult result = quickAction.Do(subtitles);

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
            }
        }
    }
}
