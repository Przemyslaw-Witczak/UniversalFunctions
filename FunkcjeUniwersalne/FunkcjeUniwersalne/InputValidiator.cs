using IDowiComponentNamespace;
using IInputValidiatorNamespace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MojeFunkcjeUniwersalneNameSpace
{
    /// <summary>
    /// Kolory pól wymaganych, definicje
    /// </summary>
    public static class InputValidiatorKolory
    {
        /// <summary>
        /// Kolor pól wymagających uzupełnienia
        /// </summary>
        public static Color Wymagany = Color.Aqua;

        public static Color WymaganyRamka = Color.Maroon;
        /// <summary>
        /// Kolor pól wymaganych, poprawnie uzupełnionych
        /// </summary>
        public static Color Poprawny = Color.Azure;

    }
    /// <summary>
    /// Klasa służąca do walidacji wypełnienia pól wymaganych, oraz walidacji wprowadzonych danych
    /// </summary>
    /// <remarks>Wymagane dodanie wysątpienia tej klasy na formularzu</remarks>
    public class InputValidator : IInputValidiator
    {
        private Button destinationControl;

        /// <summary>
        /// Metoda wyrejestrowująca wszystkie kontrolki
        /// </summary>
        public void UnregisterAll()
        {            
            foreach (Control control in controls)
            {
                Unregister(control);
            }

            controls.Clear();

        }

        /// <summary>
        /// Kontrolka docelowa, Button
        /// </summary>
        /// <remarks>To pole będzie włączane lub wyłączane w zależności od wypełnienia pól wymaganych</remarks>
        public Button DestinationControl
        {
            get { return destinationControl; }
            set 
            {
                if (destinationControl != value)
                {
                    destinationControl = value;
                    Validate();
                }

            }
        }

        private bool enabled;
        /// <summary>
        /// Dodatkowa zmienna włączająca/wyłączająca kontrolkę docelową, jeżeli wykorzystywana dodatkowa logika w formularzu
        /// </summary>
        public bool Enabled 
        {
            get
            {
                return this.enabled;
            }
            set
            {
                if (value != this.enabled)
                {
                    this.enabled = value;
                    Validate();
                }
            }
        }
        
        /// <summary>
        /// Zmienna prywatna przechowująca wskaźniki do walidowanych pól tekstowych
        /// </summary>
        private List<TextBox> boxes = new List<TextBox>();

        /// <summary>
        /// Zmienna prywatna przechowująca wskaźniki do walidowanych pól combo
        /// </summary>
        private List<ComboBox> combos = new List<ComboBox>();

        /// <summary>
        /// Zmienna prywatna przechowująca wskaźniki do walidowanych pól hasła
        /// </summary>
        private List<MaskedTextBox> maskedBoxes = new List<MaskedTextBox>();

        /// <summary>
        /// Lista wszystkich kontrolek, do wyładowywania wszystkich
        /// </summary>
        private List<Control> controls = new List<Control>();
        /// <summary>
        /// Klasa służąca do walidacji wypełnienia pól wymaganych, oraz określania jakie znaki może przyjmować kontrolka TextBox
        /// </summary>
        public InputValidator() : this(null)
        {
            
        }

        /// <summary>
        /// Klasa służąca do walidacji wypełnienia pól wymaganych, oraz określania jakie znaki może przyjmować kontrolka TextBox
        /// </summary>
        /// <param name="parentForm">Formatka nadrzędna, do której AcceptButton będzie walidowany</param>
        public InputValidator(Form parentForm)
        {
            boxes.Clear();
            combos.Clear();
            maskedBoxes.Clear();
            if (parentForm != null)
            {
                DestinationControl = parentForm.AcceptButton as Button;
            }
            else
            {
                DestinationControl = null;
            }

            Enabled = true;
        }
                
        /// <summary>
        /// Metoda dodająca pole tekstowe do walidacji
        /// </summary>
        /// <param name="tb"></param>
        public void RegisterTextBox(TextBox tb)
        {
            tb.BackColor = InputValidiatorKolory.Wymagany;
            tb.TextChanged += (s, e) => Validate();
            boxes.Add(tb);
            Validate();
            controls.Add(tb as Control);
        }

        /// <summary>
        /// Metoda dodająca każdy rodzaj kontrolki do walidacji
        /// </summary>
        /// <param name="control">Kontrolka</param>
        public void Register(Control control)
        {
            if (control is TextBox)
            {
                RegisterTextBox(control as TextBox);
            }
            else if (control is ComboBox)
            {
                RegisterComboBox(control as ComboBox);
            }
            else if (control is MaskedTextBox)
            {
                RegisterMaskedTextBox(control as MaskedTextBox);
            }
        }

        /// <summary>
        /// Metoda usuwająca pole tekstowe z listy pól do sprawdzenia
        /// </summary>
        /// <param name="tb"></param>
        public void UnregisterTextBox(TextBox tb)
        {
            tb.BackColor = SystemColors.Window;
            tb.TextChanged -= (s, e) => Validate();
            int indeks = -1;
            for (int i = 0; i < boxes.Count;i++ )
            {
                if (boxes[i] == tb)
                {
                    indeks = i;
                }
            }
            if (indeks > -1)
            {
                boxes.RemoveAt(indeks);
                Validate();
            }
        }
  
        /// <summary>
        /// Metoda dodająca pola kombo do walidacji
        /// </summary>
        /// <param name="cb"></param>
        public void RegisterComboBox(ComboBox cb)
        {
            cb.BackColor = InputValidiatorKolory.Wymagany;
            cb.TextChanged += (s, e) => Validate();
            
            combos.Add(cb);
            
            Validate();
            controls.Add(cb as Control);
        }
    
        /// <summary>
        /// Metoda wyrejestrowująca każdy rodzaj kontrolki z walidacji
        /// </summary>
        /// <param name="control"></param>
        public void Unregister(Control control)
        {
            if (control is TextBox)
            {
                UnregisterTextBox(control as TextBox);
            }
            else if (control is ComboBox)
            {
                UnregisterComboBox(control as ComboBox);
            }
            else if (control is MaskedTextBox)
            {
                UnregisterMaskedTextBox(control as MaskedTextBox);
            }
            else
            {
                throw new Exception("Nie można wyrejestrować kontrolki, ponieważ jest nieobsługiwana:" + control.Name);
            }
        }

        /// <summary>
        /// Metoda usuwająca pole kombo z listy pól wymaganych
        /// </summary>
        /// <param name="tb"></param>
        public void UnregisterComboBox(ComboBox tb)
        {
            tb.BackColor = SystemColors.Window;
            tb.TextChanged -= (s, e) => Validate();
            int indeks = -1;
            for (int i = 0; i < combos.Count; i++)
            {
                if (combos[i] == tb)
                {
                    indeks = i;
                }
            }
            if (indeks > -1)
            {
                combos.RemoveAt(indeks);
                Validate();
            }
        }
        /// <summary>
        /// Metoda dokonująca walidacji wypełnienia powiązanych pól, wywoływana automatycznie podczas zmiany pól powiązanych oraz można ja wywołać ręcznie
        /// </summary>
        public void Validate()
        {
            bool destinationControlValue = Enabled;
            if (DestinationControl == null)
            {
                return;
            }

            foreach (var t in boxes)
            {
                if (t.Enabled)
                {
                    if (string.IsNullOrEmpty(t.Text.Trim()))
                    {
                        destinationControlValue = false;
                        t.BackColor = InputValidiatorKolory.Wymagany;
                        
                    }
                    else
                    {
                        t.BackColor = InputValidiatorKolory.Poprawny;
                    }                    
                }
            }

            foreach (var c in combos)
            {
                if (c.Enabled)
                {
                    if ((c.SelectedIndex < 0 && !(c is IDowiComponent)) || ((c is IDowiComponent) && !(c as IDowiComponent).HasValue))
                    {
                        destinationControlValue = false;
                        c.BackColor = InputValidiatorKolory.Wymagany;
                    }
                    else
                    {
                        c.BackColor = InputValidiatorKolory.Poprawny;
                    }                    
                }
            }

            foreach (var c in maskedBoxes)
            {
                if (c.Enabled)
                {
                    if (string.IsNullOrEmpty(c.Text))
                    {
                        destinationControlValue = false;
                        c.BackColor = InputValidiatorKolory.Wymagany;
                    }
                    else
                    {
                        c.BackColor = InputValidiatorKolory.Poprawny;
                    }
                }
            }

            DestinationControl.Enabled = destinationControlValue;
        }

        #region Sprawdzanie wartosci dozwolonych, podczas uzupełniania pól
        /// <summary>
        /// Metoda dodająca do TextBoxa zdarzenie ograniczające wprowadzanie wartości tylko do liczb całkowitych
        /// </summary>
        /// <param name="txtbox">Kontrolka</param>
        public void AllowOnlyIntegerValues(TextBox txtbox)
        {
            txtbox.KeyPress += (s, e) => ActionKeyPressIntegerValue(s, e);
        }
        /// <summary>
        /// Metoda dodająca do TextBoxa zdarzenie ograniczające wprowadzanie wartości tylko do liczb rzeczywistych
        /// </summary>
        /// <param name="txtbox">Kontrolka</param>
        public void AllowOnlyDecimalValues(TextBox txtbox)
        {
            txtbox.KeyPress += (s, e) => ActionKeyPressDecimalValue(s, e);
        }

        /// <summary>
        /// Zdarzenie naciśnięcia klawisza w polu tekstowym, ograniczające tylko do liczb całkowitych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActionKeyPressIntegerValue(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar!=45)
            {
                e.KeyChar = Convert.ToChar(Keys.Back);
            }
        }

        /// <summary>
        /// Zdarzenie naciśnięcia klawisza w polu tekstowym, ograniczające tylko do liczb z separatorem dziesiętnym
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActionKeyPressDecimalValue(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
            {
                e.KeyChar = Convert.ToChar(Keys.Back);
            }
        }
        #endregion 
    
        /// <summary>
        /// Metoda dodająca pole hasła do listy pól wymaganych
        /// </summary>
        /// <param name="tb"></param>
        public void RegisterMaskedTextBox(MaskedTextBox tb)
        {
            tb.BackColor = InputValidiatorKolory.Wymagany;
            tb.TextChanged += (s, e) => Validate();
            maskedBoxes.Add(tb);
            Validate();
            controls.Add(tb as Control);
        }

        /// <summary>
        /// Metoda usuwająca pole hasła z listy pól wymaganych
        /// </summary>
        /// <param name="tb"></param>
        public void UnregisterMaskedTextBox(MaskedTextBox tb)
        {
            tb.BackColor = SystemColors.Window;
            tb.TextChanged -= (s, e) => Validate();
            int indeks = -1;
            for (int i = 0; i < maskedBoxes.Count; i++)
            {
                if (maskedBoxes[i] == tb)
                {
                    indeks = i;
                }
            }
            if (indeks > -1)
            {
                maskedBoxes.RemoveAt(indeks);
            }

            Validate();
        }
    }
}
