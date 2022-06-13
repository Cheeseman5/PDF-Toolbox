using PDFToolbox.Models;

namespace PDFToolbox.Interfaces.Factories
{
    public interface IPageFactory
    {
        Page CreatePage();
        Document CreateDocument();
        Document CreateDocument(string fName);
        Page CopyPage(Page page);
        Document CopyDocument(Document document);
    }
}
