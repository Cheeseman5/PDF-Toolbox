using System.Windows.Controls;

namespace PDFToolbox.Interfaces.Managers
{
    public interface IToolboxManager
    {
        void DropOnPage(ListBoxItem lbxItemDropTarget, ViewModels.PageViewModel draggedPage, ViewModels.MainViewModel viewModel);
    }
}
