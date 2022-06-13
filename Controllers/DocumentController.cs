using PDFToolbox.Interfaces.Controllers;
using PDFToolbox.Interfaces.Factories;
using PDFToolbox.Models;
using System;
using System.Collections.Generic;

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
            if(document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            if(page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            document.Pages.Add(page);
            return document;
        }

        public Document CopyDocument(Document document)
        {
            if(document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            Document doc = _pageFactory.CopyDocument(document);
            return doc;
        }

        public Document NewDocument()
        {
            Document document = _pageFactory.CreateDocument();
            return document;
        }
        public IEnumerable<Document> SplitDocument(Document document, int splitInterval)
        {
            if(document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            if(splitInterval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(splitInterval), 
                    $"SplitInterval is expected to be a positive, non-zero integer.  Interval of '{splitInterval}' is invalid.");
            }

            var newDocuments = new List<Document>();
            Document tempDoc = _pageFactory.CreateDocument();

            while (document.Pages.Count > splitInterval)
            {
                // current doc reached its goal page-count - start the next one...
                if (tempDoc.Pages.Count >= splitInterval)
                {
                    Document newDoc = _pageFactory.CreateDocument();

                    newDoc.FileName = tempDoc.FileName;
                    newDoc.Image = tempDoc.Pages[0].Image;

                    newDocuments.Add(newDoc);
                    tempDoc = newDoc;
                }
                else
                {
                    tempDoc.Pages.Add(document.Pages[splitInterval]);
                    document.Pages.RemoveAt(splitInterval);
                }
            }
            return newDocuments;
        }
    }
}
