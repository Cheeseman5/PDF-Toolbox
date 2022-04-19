using PDFToolbox.Models;

namespace Factories
{
    public class PageFactory
    {
        private int currentID = 0;

        private int GetNextID()
        {
            return ++currentID;
        }
        public Page CreatePage()
        {
            var page = new Page();
            page.ID = GetNextID();
            return page;
        }
        public Document CreateDocument()
        {
            var document = CreateDocument("NewDocument");
            return document;
        }
        public Document CreateDocument(string fName)
        {
            var document = new Document();
            document.ID = GetNextID();
            document.FileName = fName;
            return document;
        }
        public Page CopyPage(Page page)
        {
            if (page == null) return null;

            var newPage = new Page()
            {
                ID = GetNextID(),
                Image = page.Image,
                FileName = page.FileName,
                OriginalPageNumber = page.OriginalPageNumber,
                Rotation = page.Rotation,
                OriginalRotation = page.OriginalRotation,
                IsImagePreview = page.IsImagePreview,
                ImageStream = page.ImageStream,
                UIStrings = page.UIStrings
            };
            
            return newPage;
        }

        public Document CopyDocument(Document document)
        {
            if(document == null) return null;

            var newDoc = new Document()
            {
                ID = GetNextID(),
                FileName = document.FileName,
                ImageStream = document.ImageStream,
                Image = document.Image,
                OriginalPageNumber = document.OriginalPageNumber,
                IsImagePreview = document.IsImagePreview,
                UIStrings = document.UIStrings,
                OriginalRotation = document.OriginalRotation,
                Rotation = document.Rotation,
                Pages = document.Pages
            };

            return newDoc;
        }
    }
}
