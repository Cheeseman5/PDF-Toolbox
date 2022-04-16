using PDFToolbox.Helpers;
using PDFToolbox.Interfaces.Managers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Media;
using Factories;

namespace PDFToolbox.Managers
{
    public class ToolboxManager : IToolboxManager
    {
        private Toolbox _toolbox;
        public ToolboxManager(Toolbox toolbox)
        {
            _toolbox = toolbox;
        }

        public void DropOnPage(ListBoxItem lbxItemDropTarget, ViewModels.PageViewModel draggedPage, ViewModels.MainViewModel viewModel)
        {

            // Move pageDict to last element if dropped on blank-space
            if (lbxItemDropTarget == null)
            {
                viewModel.MovePage(
                    viewModel.SelectedDocument.GetPageIndex(draggedPage),
                    viewModel.Pages.Count - 1);
                return;
            }

            var targetPage = lbxItemDropTarget.DataContext as ViewModels.PageViewModel;
            viewModel.MovePage(
                viewModel.SelectedDocument.GetPageIndex(draggedPage),
                viewModel.SelectedDocument.GetPageIndex(targetPage));
        }
    }
}
