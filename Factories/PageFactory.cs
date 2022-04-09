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
            document.fName = fName;
            return document;
        }
        public Page CopyPage(Page page)
        {
            if (page == null) return null;

            var newPage = new Page()
            {
                ID = GetNextID(),
                image = page.image,
                fName = page.fName,
                number = page.number,
                rotation = page.rotation,
                originalRotation = page.originalRotation,
                isImagePreview = page.isImagePreview,
                imageStream = page.imageStream,
                uiStrings = page.uiStrings
            };
            
            return newPage;
        }

        public Document CopyDocument(Document document)
        {
            if(document == null) return null;

            var newDoc = new Document()
            {
                ID = GetNextID(),
                fName = document.fName,
                imageStream = document.imageStream,
                image = document.image,
                originalPageNumber = document.originalPageNumber,
                isImagePreview = document.isImagePreview,
                uiStrings = document.uiStrings,
                originalRotation = document.originalRotation,
                rotation = document.rotation,
                pages = document.pages
            };

            return newDoc;
        }
    }
}
