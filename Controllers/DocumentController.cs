using PDFToolbox.Interfaces.Controllers;
using PDFToolbox.Interfaces.Factories;
using PDFToolbox.Models;

namespace PDFToolbox.Controllers
{
    public class DocumentController : IDocumentController
    {
        private IPageFactory _pageFactory;
        public DocumentController(IPageFactory pageFactory)
        {
            _pageFactory = pageFactory;
        }

        public Document AddPageToDocument(Document document, Page page)
        {
            document.Pages.Add(page);
            return document;
        }

        public Document CopyDocument(Document document)
        {
            Document doc = _pageFactory.CopyDocument(document);
            return doc;
        }

        public Document NewDocument()
        {
            Document document = _pageFactory.CreateDocument();
            return document;
        }
    }
}
