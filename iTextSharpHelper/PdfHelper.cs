using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Windows.Forms;
namespace iTextSharpHelper
{
    //enum PdfPageSize { A4, A4_Landscape};

    public static class PdfHelperFontStyle
    {
        public static int Normal = 0;
        public static int Bold = 1;
        public static int Italic = 2;    
    };

    /// <summary>
    /// Klasa pośrednicząca, do obsługi dokumentów PDF
    /// </summary>
    public class PdfHelper : IDisposable
    {
        string fileName;
        /// <summary>
        /// Nazwa pliku PDF
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// Dokument PDF
        /// </summary>
        public Document DokumentPDF;

        private PdfWriter writer;

        TwoColumnHeaderFooter PageEventHandler = null;
        
        #region Deklaracje czcionek
        public Font HeaderFont = FontFactory.GetFont(BaseFont.COURIER, BaseFont.CP1250, 18, 1);
        public Font ParagraphFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, 10, 0);
        public Font ParagraphFontBold = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, 10, 1);
        #endregion


        public PdfHelper()
        {
            FileName = string.Empty;
            DokumentPDF = null;
            writer = null;
        }

        /// <summary>
        /// Otwiera okno dialogowe do wskazania, nazwy tworzonego pliku
        /// </summary>
        /// <param name="DefaultName">Nazwa pliku do inicjalizacji</param>
        /// <returns>Rezultat</returns>
        private bool OpenSaveDialog(string DefaultName)
        {
            using (SaveFileDialog SaveDialog = new SaveFileDialog())
            {
                SaveDialog.Filter = "Pliki pdf (*.pdf)|*.pdf";
                SaveDialog.FilterIndex = 0;
                SaveDialog.RestoreDirectory = true;

                SaveDialog.FileName = DefaultName.Replace("/", "_").Replace("\\", "_").Replace("%", "_");
                if (SaveDialog.ShowDialog() == DialogResult.OK)
                {
                    FileName = SaveDialog.FileName;
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// Utwórz nowy dokument PDF, wg podanego rozmiaru
        /// </summary>
        /// <param name="DefaultName">Nazwa pliku do inicjalizacji</param>
        /// <param name="DefaultHeaderTitle">Tytuł wyświetlany w nagłówku pliku</param>
        /// <param name="pageSize">Rozmiar strony</param>
        public void CreateDocument(string DefaultName, string DefaultHeaderTitle, Rectangle pageSize)
        {
            headerTitle = DefaultHeaderTitle;
            if (!OpenSaveDialog(DefaultName))
            {
                throw new Exception("Anulowano tworzenie dokumentu !!");

            }
#pragma warning disable CS0612 // 'PageSize.A4_LANDSCAPE' is obsolete
            if (pageSize == PageSize.A4_LANDSCAPE)
#pragma warning restore CS0612 // 'PageSize.A4_LANDSCAPE' is obsolete
                pageSize = PageSize.A4.Rotate();
            DokumentPDF = new Document(pageSize, 20, 20, 20, 30);
            
            DokumentPDF.AddLanguage("Polski");            
            writer = PdfWriter.GetInstance(DokumentPDF, new FileStream(FileName, FileMode.Create));
            PageEventHandler = new TwoColumnHeaderFooter();
            if (!string.IsNullOrEmpty(headerTitle))
                PageEventHandler.Title = headerTitle;
            //PageEventHandler.HeaderFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, 6, 1);
            //PageEventHandler.HeaderLeft = "Group";
            //PageEventHandler.HeaderRight = "1";


            writer.PageEvent = PageEventHandler;

            DokumentPDF.Open();
            DokumentPDF.SetMargins(20, 20, 20, 30);
        }

        /// <summary>
        /// Utwórz nowy dokument PDF, wg podanego rozmiaru
        /// </summary>
        /// <param name="DefaultName">Nazwa pliku do inicjalizacji</param>
        /// <param name="pageSize">Rozmiar strony</param>
        public void CreateDocument(string DefaultName, Rectangle pageSize)
        {
            CreateDocument(DefaultName, string.Empty, pageSize);
        }

        /// <summary>
        /// Utwórz nowy dokument PDF
        /// </summary>
        /// <param name="DefaultName">Nazwa pliku do inicjalizacji</param>
        public void CreateDocument(string DefaultName)
        {
            CreateDocument(DefaultName, PageSize.A4);
        }

        /// <summary>
        /// Metoda dodaje akapit do dokumentu, z czcionką akapitu
        /// </summary>
        /// <param name="Text">Treść akapitu</param>
        public void AddParagraph(string Text)
        {
            var paragraph = new Paragraph(Text, ParagraphFont);
            DokumentPDF.Add(paragraph);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        private string headerTitle;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                    if (DokumentPDF!=null)
                        DokumentPDF.Dispose();  
                    if (writer!=null)
                        writer.Dispose();          
                }
                disposedValue = true;
            }
        }

        
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // GC.SuppressFinalize(this);
        }
        #endregion



        public class TwoColumnHeaderFooter : PdfPageEventHelper
        {
            // This is the contentbyte object of the writer
            PdfContentByte cb;
            // we will put the final number of pages in a template
            PdfTemplate template;
            // this is the BaseFont we are going to use for the header / footer
            BaseFont bf = null;
            // This keeps track of the creation time
            DateTime PrintTime = DateTime.Now;
            #region Properties
            private string _Title;
            public string Title
            {
                get { return _Title; }
                set { _Title = value; }
            }

            private string _HeaderLeft;
            public string HeaderLeft
            {
                get { return _HeaderLeft; }
                set { _HeaderLeft = value; }
            }
            private string _HeaderRight;
            public string HeaderRight
            {
                get { return _HeaderRight; }
                set { _HeaderRight = value; }
            }
            private Font _HeaderFont;
            public Font HeaderFont
            {
                get { return _HeaderFont; }
                set { _HeaderFont = value; }
            }
            private Font _FooterFont;
            public Font FooterFont
            {
                get { return _FooterFont; }
                set { _FooterFont = value; }
            }
            #endregion
            // we override the onOpenDocument method
            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    PrintTime = DateTime.Now;                    
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                    template = cb.CreateTemplate(50, 50);
                }
#pragma warning disable CS0168 // The variable 'de' is declared but never used
                catch (DocumentException de)
#pragma warning restore CS0168 // The variable 'de' is declared but never used
                {
                }
#pragma warning disable CS0168 // The variable 'ioe' is declared but never used
                catch (System.IO.IOException ioe)
#pragma warning restore CS0168 // The variable 'ioe' is declared but never used
                {
                }
            }
            public override void OnStartPage(PdfWriter writer, Document document)
            {
                base.OnStartPage(writer, document);
                Rectangle pageSize = document.PageSize;
                if (!string.IsNullOrEmpty(Title))
                {
                    cb.BeginText();
                    cb.SetFontAndSize(bf, 8);                    
                    //cb.SetRGBColorFill(50, 50, 200);
                    //cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetTop(40));
                    cb.SetTextMatrix(pageSize.GetLeft(20), pageSize.GetTop(20));
                    cb.ShowText(Title);
                    cb.EndText();
                }
                if (!string.IsNullOrEmpty(HeaderLeft + HeaderRight))
                {
                    PdfPTable HeaderTable = new PdfPTable(2);
                    HeaderTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    HeaderTable.TotalWidth = pageSize.Width - 80;
                    HeaderTable.SetWidthPercentage(new float[] { 45, 45 }, pageSize);

                    PdfPCell HeaderLeftCell = new PdfPCell(new Phrase(8, HeaderLeft, HeaderFont));
                    HeaderLeftCell.Padding = 5;
                    HeaderLeftCell.PaddingBottom = 8;
                    HeaderLeftCell.BorderWidthRight = 0;
                    HeaderTable.AddCell(HeaderLeftCell);
                    PdfPCell HeaderRightCell = new PdfPCell(new Phrase(8, HeaderRight, HeaderFont));
                    HeaderRightCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    HeaderRightCell.Padding = 5;
                    HeaderRightCell.PaddingBottom = 8;
                    HeaderRightCell.BorderWidthLeft = 0;
                    HeaderTable.AddCell(HeaderRightCell);
                    cb.SetRGBColorFill(0, 0, 0);
                    HeaderTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(50), cb);
                }
            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);
                int pageN = writer.PageNumber;
                String text = "Strona " + pageN + " z ";
                float len = bf.GetWidthPoint(text, 8);
                Rectangle pageSize = document.PageSize;
                cb.SetRGBColorFill(100, 100, 100);
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
                cb.ShowText(text);
                cb.EndText();
                cb.AddTemplate(template, pageSize.GetLeft(40) + len, pageSize.GetBottom(30));

                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
                $"{Application.ProductName} {Application.ProductVersion} Data wydruku {PrintTime.ToString()}",
                pageSize.GetRight(40),
                pageSize.GetBottom(30), 0);
                cb.EndText();
            }
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
                template.BeginText();
                template.SetFontAndSize(bf, 8);
                template.SetTextMatrix(0, 0);
                template.ShowText("" + (writer.PageNumber - 1));
                template.EndText();
            }
        }

        
    }
}
