using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MojeFunkcjeUniwersalneNameSpace
{
    public enum DataGridViewColumnValueType
    {
        NotSet,
        Decimal,
        Integer
    }

    /// <summary>
    /// Klasa służąca do inicjalizowania DataGridów
    /// </summary>
    /// <remarks>Ustawia właściwości zawijania wierszy, blokowania edycji, ipp.</remarks>
    public class DataGridInitializer
    {
        private readonly List<DataGridViewColumn> gridKolumny;
        private readonly DataGridView gridKomponent;
        private readonly Dictionary<int,DataGridViewColumnValueType> typyKolumn;

        /// <summary>
        /// Metoda dodająca kolumnę typu CheckBox do grida
        /// </summary>
        /// <param name="columnText">Etykieta kolumny</param>
        public void AddCheckBoxColumn(string columnText, bool visible = true)
        {
            DataGridViewCheckBoxColumn kolumna = new DataGridViewCheckBoxColumn()
            {
                Name = columnText,
                Visible = visible
            };
            
            gridKolumny.Add(kolumna);
        }
        
        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        /// <param name="gridPozycje">Grid do inicjalizacji</param>
        public DataGridInitializer(DataGridView gridPozycje)
        {
            this.gridKomponent = gridPozycje;

            gridKolumny = new List<DataGridViewColumn>();
            typyKolumn = new Dictionary<int, DataGridViewColumnValueType>();
            gridKomponent.AllowUserToAddRows = false;
            gridKomponent.ColumnCount = 0;
            gridKomponent.ColumnHeadersVisible = true;          
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Verdana", 8, FontStyle.Bold);
            gridKomponent.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
            //ukryj boczną kolumnę, wskaźnik wiersza
            gridKomponent.RowHeadersVisible = false;

        }

        
        /// <summary>
        /// Metoda dodająca kolumnę do grida
        /// </summary>
        /// <param name="columnText">Etykieta kolumny</param>
        /// <param name="columnVisible">Czy kolumna widoczna</param>
        /// <param name="SortMode">Sposób sortowania kolumny</param>
        public void AddColumn(string columnText, bool columnVisible = true, DataGridViewColumnSortMode SortMode = DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment alignment = DataGridViewContentAlignment.NotSet, bool readOnly = true, DataGridViewColumnValueType columnValueType = DataGridViewColumnValueType.NotSet)
        {           
            DataGridViewColumn kolumna = new DataGridViewColumn()
            {
                Name = columnText,              
                Visible = columnVisible,
                SortMode = SortMode,
                ReadOnly = readOnly
            };

            if (alignment != DataGridViewContentAlignment.NotSet)
            {
                kolumna.DefaultCellStyle.Alignment = alignment;
            }

            //Domyślne ustawienie szerokości kolumn LP, początkowe
            if (columnText.Trim().ToUpper() == "LP")
            {
                kolumna.Width = 30;
            }

            if (columnValueType!=DataGridViewColumnValueType.NotSet)
            {
                if (typyKolumn.Count==0)
                {
                    gridKomponent.EditingControlShowing += GridKomponent_EditingControlShowing;
                }

                typyKolumn.Add(gridKolumny.Count, columnValueType);

            }
            if (!readOnly)
            {
                gridKomponent.CellFormatting += GridKomponent_CellFormatting;
            }


            gridKolumny.Add(kolumna);
        }

        #region Zdarzenia przypinane do grida
        private void GridKomponent_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(GridViewKolumn_KeyPressOnlyInteger);
            e.Control.KeyPress -= new KeyPressEventHandler(GridViewKolumn_KeyPressDecimal);

            DataGridViewColumnValueType value;
            if (typyKolumn.TryGetValue(gridKomponent.CurrentCell.ColumnIndex, out value) && value!=DataGridViewColumnValueType.NotSet)
            {                
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    switch (value)
                    {
                        case DataGridViewColumnValueType.Integer:
                            tb.KeyPress += new KeyPressEventHandler(GridViewKolumn_KeyPressOnlyInteger);
                            break;
                        case DataGridViewColumnValueType.Decimal:
                            tb.KeyPress += new KeyPressEventHandler(GridViewKolumn_KeyPressDecimal);
                            break;
                    }  
                    
                }
            }

        }

        private void GridViewKolumn_KeyPressOnlyInteger(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void GridViewKolumn_KeyPressDecimal(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)
                && e.KeyChar != Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
            {
                e.Handled = true;
            }
        }

        private void GridKomponent_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = sender as DataGridView;
            if (e.RowIndex < 0 || grid.Rows[e.RowIndex].Tag==null || grid.Rows[e.RowIndex].ReadOnly)
            {
                return;
            }

            if (!gridKolumny[e.ColumnIndex].ReadOnly)
            {
                if (!(gridKolumny[e.ColumnIndex] is DataGridViewCheckBoxColumn))
                {
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Aqua;
                }
            }            

        }
        #endregion

        /// <summary>
        /// Metoda dodająca kolumnę do grida
        /// </summary>
        /// <param name="columnText">Nagłówek kolumny</param>
        /// <param name="columnVisible">Czy kolumna ma być widoczna</param>
        /// <param name="readOnly">Czy kolumna edytowalna</param>
        public void AddEditableColumn(string columnText, bool columnVisible = true)
        {
            AddColumn(columnText, columnVisible, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.NotSet, false);
        }
        /// <summary>
        /// Zakończenie dodawania kolumn, finalizowanie ustawień
        /// </summary>
        public void EndInitialize()
        {
            gridKomponent.ColumnCount = gridKolumny.Count;

            for (int i = 0; i < gridKolumny.Count;i++ )
            {
                if (gridKolumny[i] is DataGridViewCheckBoxColumn || gridKolumny[i] is DataGridViewImageColumn)
                {
                    gridKomponent.Columns.Insert(i, gridKolumny[i]);
                    gridKomponent.Columns.RemoveAt(i + 1);
                    gridKomponent.Columns[i].ReadOnly = false;
                }
                else
                {
                    if (!gridKolumny[i].ReadOnly)
                    {
                        gridKomponent.ReadOnly = false;
                    }

                    gridKomponent.Columns[i].ReadOnly = gridKolumny[i].ReadOnly;                    
                }
                gridKomponent.Columns[i].Name = gridKolumny[i].Name;
                gridKomponent.Columns[i].Visible = gridKolumny[i].Visible;
                gridKomponent.Columns[i].SortMode = gridKolumny[i].SortMode;
                gridKomponent.Columns[i].DefaultCellStyle.Alignment = gridKolumny[i].DefaultCellStyle.Alignment;
                gridKomponent.Columns[i].Width = gridKolumny[i].Width;
                
            }

            gridKomponent.DefaultCellStyle.WrapMode = DataGridViewTriState.True;            
            gridKomponent.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            //for (Int16 i = 1; i < gridKomponent.ColumnCount; i++)
            //    gridKomponent.Columns[i].ReadOnly = true;         
        }

        public void AddImageColumn(string columnText)
        {
            DataGridViewImageColumn kolumna = new DataGridViewImageColumn()
            {
                Name = columnText,
                ValuesAreIcons = false,
                ImageLayout = DataGridViewImageCellLayout.Zoom,

            };

            

            gridKolumny.Add(kolumna);
        }
    }
}
