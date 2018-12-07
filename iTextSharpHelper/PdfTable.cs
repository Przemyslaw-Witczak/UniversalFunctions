using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTextSharpHelper
{
    /// <summary>
    /// Wyrównanie tekstu w komórkach
    /// </summary>
    public enum CellAlignment 
        { 
            /// <summary>
            /// Nie wybrany
            /// </summary>
            caNotSet = -1,
            /// <summary>
            /// Do lewej
            /// </summary>
            caLeft = 0, 
            /// <summary>
            /// Do środka
            /// </summary>
            caCenter = 1, 
            /// <summary>
            /// Do prawej
            /// </summary>
            caRight = 2, 
            /// <summary>
            /// Wyśrodkowany
            /// </summary>
            caMiddle = 3
    
        };

    /// <summary>
    /// Komórka tabeli
    /// </summary>
    public class myPdfCell
    {

        private Image _cellPicture;
        /// <summary>
        /// Obrazek do wyświetlania w komórce
        /// </summary>
        public Image cellPicture
        {
            get
            {
                return _cellPicture;
            }
            
            set
            {
                _cellPicture = value;
                text = string.Empty;
            }
        }

        private string text;
        /// <summary>
        /// Tekst wyświetlany w komórce
        /// </summary>
        public string Text
        {
            get { return text; }
            set 
            { 
                text = value;
                Align = Owner.CellAlign;
                Font = Owner.CellFont;
            }
        }

        /// <summary>
        /// Wyównanie tekstu
        /// </summary>
        public CellAlignment Align;

        /// <summary>
        /// Czcionka
        /// </summary>
        public Font Font;

        /// <summary>
        /// Czy komórka bez obramowania?
        /// </summary>
        /// <remarks>Domyślnie false, czyli komórki z obramowaniem</remarks>
        public bool NoBorder;

        /// <summary>
        /// Określa ile kolumn w danym wierszu scalić, to nie jest indeks kolumny
        /// </summary>
        public int ColumnSpan;

        /// <summary>
        /// Określa ile wierszy w danej kolumnie scalić
        /// </summary>
        public int RowSpan;

        /// <summary>
        /// Określa czy komórkę pominąć, ponieważ kolumna jest scalona wierszami
        /// </summary>
        public bool mergedRow;        

        /// <summary>
        /// Konstruktor
        /// </summary>
        public myPdfCell(PdfTableHalper owner)
        {
            Owner = owner;
            Text = string.Empty;
            Align = Owner.CellAlign;
            Font = Owner.CellFont;
            NoBorder = false;
            RowSpan = 0;
            ColumnSpan = 0;
            mergedRow = false;
            cellPicture = null;
        }

        /// <summary>
        /// Wskaźnik do obiektu nadrzędnego, w celu uproszczenia przekazywania właściwości
        /// </summary>
        public PdfTableHalper Owner;
    }

    /// <summary>
    /// Klasa wspomagająca tworzenie tabel
    /// </summary>
    public class PdfTableHalper
    {       

       
        /// <summary>
        /// Dwuwymiarowa macież odzwierciedlająca komórki tabeli, wiersze i kolumny, pierwszy parametr określa kolumnę, drugi wiersz
        /// </summary>
        public List<List<myPdfCell>> Cells;

        /// <summary>
        /// Szerokości kolumn
        /// </summary>
        public List<float> ColumnWidths;

        /// <summary>
        /// Właściwość ustalająca minimalną wysokość wiersza
        /// </summary>
        public int MinimumRowHeigth { private get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PdfTableHalper()
        {
            Cells = new List<List<myPdfCell>>();
            ColumnWidths = new List<float>();
            ColumnCount = 1;
            RowCount = 1;
            CellAlign = CellAlignment.caNotSet;
            MinimumRowHeigth = 0;
        }

        private int columnCount;
        /// <summary>
        /// Liczba kolumn tabeli, wartosć domyślna 1
        /// </summary>
        public int ColumnCount
        {
            get { return columnCount; }
            set 
            {
                if (columnCount!=value)
                {
                    columnCount = value;
                    Resize();
                }
            }
        }

        /// <summary>
        /// Wyrównanie tekstu dla nowych komórek
        /// </summary>
        public CellAlignment CellAlign;
        /// <summary>
        /// Czcionka dla nowych komórek
        /// </summary>
        public Font CellFont;

        private int rowCount;
        /// <summary>
        /// Liczba wierszy tabeli, wartość domyślna 1
        /// </summary>
        public int RowCount
        {
            get { return rowCount; }
            set 
            { 
                if (rowCount!=value)
                {
                    rowCount = value;
                    Resize();
                }
            }
        }   

        /// <summary>
        /// Metoda zmieniająca rozmiar tabel
        /// </summary>
        private void Resize()
        {
            
            //zwiększam liczbe kolumn
 	        if(Cells.Count<ColumnCount) 
            {
                for (int x=Cells.Count;x<ColumnCount;x++)
                {
                    Cells.Add(new List<myPdfCell>());
                    ColumnWidths.Add(25);
                    for (int y=0;y<RowCount;y++)
                    {
                        Cells[x].Add(new myPdfCell(this));
                    }
                }
                
            }
            //zmniejszam liczbę kolumn
            if (Cells.Count>ColumnCount)
            {
                Cells.RemoveRange(ColumnCount, Cells.Count-ColumnCount);
                ColumnWidths.RemoveRange(ColumnCount, ColumnWidths.Count - ColumnCount);
            }
            
           
            foreach(List<myPdfCell> aColumn in Cells)
            {
                //zwiekszam liczbę wierszy
                if (aColumn.Count < RowCount)
                    for (int y = aColumn.Count; y < RowCount; y++)
                        aColumn.Add(new myPdfCell(this));
                //zmniejszam liczbę wierszy
                if (aColumn.Count > RowCount)
                    aColumn.RemoveRange(RowCount, aColumn.Count - RowCount);
            }
            
           
        }               

        /// <summary>
        /// Metoda zwraca tabelę do narysowania na dokumencie
        /// </summary>
        /// <returns></returns>
        public PdfPTable GetTable()
        {

            if (CellFont == null)
                throw new Exception("Nie ustawiono czcionki domyślnej komórek!!");
            PdfPTable returnedTable = new PdfPTable(ColumnCount);
            returnedTable.WidthPercentage = 100;

            for (int y = 0; y < RowCount; y++)                       
            {
                int x=0;
                while(x<ColumnCount)
                {

                    PdfPCell cell;
                    var currentCell = Cells[x][y];
                    if (currentCell.cellPicture != null)
                    {
                        cell = new PdfPCell(currentCell.cellPicture);
                        if (!currentCell.NoBorder)
                            cell.Padding = 1;
                    }
                    else
                        cell = new PdfPCell(new Phrase(currentCell.Text, currentCell.Font));
                    SetPdfCellAlignment(cell, currentCell.Align);                    
 
                    if (currentCell.NoBorder)
                        cell.Border = 0;

                    if (currentCell.RowSpan > 0)
                    {
                        cell.Rowspan = currentCell.RowSpan;

                        for (int i = y + 1; i < y + currentCell.RowSpan; i++)
                        {
                            if (i<RowCount)
                                Cells[x][i].mergedRow = true;
                        }
                    }
                    else
                    {
                        if (MinimumRowHeigth>0)
                            cell.MinimumHeight = MinimumRowHeigth;
                    }
                   
                    int incrementX=1;
                    if (currentCell.ColumnSpan > 0)
                    {
                        cell.Colspan = currentCell.ColumnSpan;
                        incrementX = currentCell.ColumnSpan;
                    }
                    
                    if (!currentCell.mergedRow)
                        returnedTable.AddCell(cell);

                    x += incrementX;
                }
                                
            }

            returnedTable.SpacingAfter = 10;
            returnedTable.SetWidths(ColumnWidths.ToArray());
            returnedTable.SpacingBefore = 10;

            return returnedTable;
        }

        /// <summary>
        /// Mapuje wyrównanie tekstu komórek tabeli
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="alignment"></param>
        private void SetPdfCellAlignment(PdfPCell cell, CellAlignment alignment)
        {
            switch (alignment)
            {
                case CellAlignment.caLeft:
                    cell.HorizontalAlignment = 0;
                    break;
                case CellAlignment.caCenter:
                    cell.HorizontalAlignment = 1;
                    break;
                case CellAlignment.caRight:
                    cell.HorizontalAlignment = 2;
                    break;
                case CellAlignment.caMiddle:
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = 1;
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Ustawia brak obramowania dla komórek z podanego zakresu
        /// </summary>
        /// <param name="x1">Współrzędna X lewego górnego narożnika</param>
        /// <param name="y1">Współrzędna Y lewego górnego narożnika</param>
        /// <param name="x2">Współrzędna X prawego dolnego narożnika</param>
        /// <param name="y2">Współrzędna Y prawego dolnego narożnika</param>
        public void SetNoBorder(int x1, int y1, int x2, int y2)
        {
            for (int x=x1;x<=x2;x++)
            {
                for(int y=y1;y<=y2;y++)
                {
                    Cells[x][y].NoBorder = true;
                }
            }
        }

    }
}