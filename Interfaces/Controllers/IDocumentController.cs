using PDFToolbox.Models;

namespace PDFToolbox.Interfaces.Controllers
{
    public interface IDocumentController
    {
        Document NewDocument();
        Document CopyDocument(Document document);
        Document AddPageToDocument(Document document, Page page);
    }
}
