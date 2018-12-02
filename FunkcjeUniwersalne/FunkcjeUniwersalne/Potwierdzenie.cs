using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace MojeFunkcjeUniwersalneNameSpace
{
    /// <summary>
    /// Rodzaj komunikatu wyświetlanego przez formularz
    /// </summary>
    public enum KomunikatRodzaj 
    { 
        /// <summary>
        /// Formularz wyświetla przyciski Tak oraz Nie
        /// </summary>
        Pytanie, 
        /// <summary>
        /// Formularz wyświetla przycisk OK
        /// </summary>
        Potwierdzenie 
    };
    public partial class Potwierdzenie : Form
    {
       
        /// <summary>
        /// Konstruktor formularza
        /// </summary>
        /// <param name="Rodzaj">Rodzaj komunikatu, pytanie czy potwierdzenie</param>
        /// <param name="Komunikat">Treść wyświetlanego komunikatu</param>       
        public Potwierdzenie(KomunikatRodzaj Rodzaj, string Komunikat)
        {
            InitializeComponent();

            label1.Text = Komunikat;
            label2.Text = "";
            konstruktor(Rodzaj);
        }

        /// <summary>
        /// Konstruktor formularza
        /// </summary>
        /// <param name="Rodzaj">Rodzaj komunikatu, pytanie czy potwierdzenie</param>
        /// <param name="Komunikat">Pierwsza linia komunikatu</param>
        /// <param name="Komunikat2">Dodatkowa linia komunikatu</param>
        public Potwierdzenie(KomunikatRodzaj Rodzaj, string Komunikat, string Komunikat2)
        {
            InitializeComponent();

            label1.Text = Komunikat;
            label2.Text = Komunikat2;
            konstruktor(Rodzaj);
        }

        /// <summary>
        /// Metoda inicjalizująca formularz, wykorzystywana przez konstruktory
        /// </summary>
        /// <param name="Rodzaj"></param>
        private void konstruktor(KomunikatRodzaj Rodzaj)
        {
            if (Rodzaj == KomunikatRodzaj.Potwierdzenie)
            {
                btnNie.Visible = false;
                btnTak.Visible = false;
                AcceptButton = btnOk;
            }
            else if (Rodzaj == KomunikatRodzaj.Pytanie)
            {
                btnOk.Visible = false;
                AcceptButton = btnTak;
                CancelButton = btnNie;
            }
        }

    }
}
