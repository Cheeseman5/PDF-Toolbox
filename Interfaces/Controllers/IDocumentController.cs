using PDFToolbox.Models;
using System.Collections.Generic;

namespace PDFToolbox.Interfaces.Controllers
{
    public interface IDocumentController
    {
        Document NewDocument();
        Document CopyDocument(Document document);
        Document AddPageToDocument(Document document, Page page);
        IEnumerable<Document> SplitDocument(Document document, int splitInterval);
    }
}
