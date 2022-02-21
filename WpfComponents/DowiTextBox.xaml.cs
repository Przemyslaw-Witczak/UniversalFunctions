using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfComponents
{
    //First we have to define a delegate that acts as a signature for the
    //function that is ultimately called when the event is triggered.
    //You will notice that the second parameter is of MyEventArgs type.
    //This object will contain information about the triggered event.
    public delegate void OnExitModifiedHandler(object source, OnExitModifiedArgs e);

    //This is a class which describes the event to the class that recieves it.
    //An EventArgs class must always derive from System.EventArgs.
    public class OnExitModifiedArgs : RoutedEventArgs
    {
        private readonly string _eventInfo;
        public OnExitModifiedArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            _eventInfo = "On exit, with modified text";
        }

        public string GetInfo()
        {
            return _eventInfo;
        }
    }

    /// <summary>
    /// Dopuszczalne wartości parametru Kind
    /// </summary>
    /// <remarks>Zawiera wartości konfiguracyjne, jak pole tekstowe ma się zachowywać podczas wpisywania znaków</remarks>
    public enum DowiTextBoxKindValue
    {
        /// <summary>
        /// Wszystkie znaki
        /// </summary>
        kvEverything,

        /// <summary>
        /// 0-9, minus, kropka, przecinek
        /// </summary>
        kvFloat,

        /// <summary>
        /// 0-9, minus
        /// </summary>
        kvInteger,

        /// <summary>
        /// Litery
        /// </summary>
        kvText,

        /// <summary>
        /// 0-9, kropka, przecinek
        /// </summary>
        kvFloatPositive
    }
    /// <summary>
    /// Interaction logic for DowiTextBox.xaml
    /// </summary>
    public partial class DowiTextBox : UserControl
    {
        #region Zdarzenie OnExitModified
        public static readonly RoutedEvent OnExitModifiedEvent =
        EventManager.RegisterRoutedEvent(
            "OnExitModified",
            //RoutingStrategy.Bubble,
            RoutingStrategy.Direct,
            //typeof(RoutedEventHandler),
            typeof(OnExitModifiedHandler),
            typeof(DowiTextBox));

        /// <summary>
        /// Nowe zdarzenie TextBoxa, obsługujące moment wyjścia z pola tekstowego, gdy pole traci focus
        /// </summary>
        public event RoutedEventHandler OnExitModified
        {
            add { AddHandler(OnExitModifiedEvent, value); }
            remove { RemoveHandler(OnExitModifiedEvent, value); }
        }
        #endregion
        /// <summary>
        /// Zmienna do porównywania zmian tekstu, przechowuje poprzednią wartość, wykorzystywane przez zdarzenie OnExitModified
        /// </summary>
        private string OldText;

        private void SetOldTextValue()
        {
            if (OldText != poleTekstowe.Text)
            {
                OldText = poleTekstowe.Text;
            }
        }

        #region TextProperty
        /// <summary>
        /// Właśliwość Text, rejestracja
        /// </summary>
        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DowiTextBox), new FrameworkPropertyMetadata("",
                                                    FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                                               ));

        /// <summary>
        /// Zmienna przechowująca właściwość tekst
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                if (Text != value)
                {
                    SetValue(TextProperty, value);
                }
            }
        }
        #endregion

        #region KindValueProperty
        /// <summary>
        /// Właściwość opisująca jaki rodzaj danych przyjmuje pole tekstowe
        /// </summary>
        public static DependencyProperty KindProperty = DependencyProperty.Register("Kind", typeof(DowiTextBoxKindValue), typeof(DowiTextBox));

        /// <summary>
        /// Zmienna przechowująca właściwość jaki rodzaj danych przyjmuje pole tekstowe
        /// </summary>
        public DowiTextBoxKindValue Kind
        {
            get => (DowiTextBoxKindValue)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }
        #endregion

        #region IsReadonlyProperty
        /// <summary>
        /// Właściwość opisująca czy pole tekstowe dopuszcza edytowanie
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DowiTextBox));

        /// <summary>
        /// Zmienna przechowująca wartość właściwości czy pole tekstowe dopuszcza edytowanie
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        #endregion

        public DowiTextBox()
        {
            Kind = DowiTextBoxKindValue.kvEverything;
            OldText = string.Empty;
            InitializeComponent();
            DataObject.AddPastingHandler(this, new DataObjectPastingEventHandler(OnPaste));
        }
        #region Zdarzenia wykorzystywane przez kontrolkę, w celu walidacji
        /// <summary>
        /// Gdy pole traci fokus, na skutek wyścia, oraz zmieniła się wartość Text, następuje wywołanie zdarzenia OnExitModified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void poleTekstowe_LostFocus(object sender, RoutedEventArgs e)
        {
            //przypisuję spowrotem do zmiennej
            //Text = poleTekstowe.Text;   
            if (OldText != poleTekstowe.Text)
            {
                OldText = poleTekstowe.Text;

                poleTekstowe.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource(); //aktualizuje zbindowany model na podstawie wartości kontrolki
                RaiseEvent(new OnExitModifiedArgs(DowiTextBox.OnExitModifiedEvent, sender));
            }
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                ((UIElement)e.OriginalSource).MoveFocus(request);
                //OldText = poleTekstowe.Text;

                //poleTekstowe.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource(); //aktualizuje zbindowany model na podstawie wartości kontrolki
                //RaiseEvent(new OnEnterKeyPressArgs(PrimeTextBox.OnEnterKeyPressEvent, sender));
            }
        }
        #endregion

        #region InvertCall
        public static readonly RoutedEvent InvertCallEvent = EventManager.RegisterRoutedEvent("InvertCall",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DowiTextBox));

        public event RoutedEventHandler InvertCall
        {
            add { AddHandler(InvertCallEvent, value); }
            remove { RemoveHandler(InvertCallEvent, value); }
        }

        private void OnInvertCall()
        {
            RoutedEventArgs args = new RoutedEventArgs(InvertCallEvent);
            RaiseEvent(args);
        }

        #endregion

        /// <summary>
        /// Po wejściu w pole tekstowe, zainicjalizuj wartość OldText, na potrzeby porównywania
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void poleTekstowe_GotFocus(object sender, RoutedEventArgs e)
        {
            OldText = poleTekstowe.Text;
        }

        /// <summary>
        /// Metoda sprawdzająca, czy znak na podanej pozycji jest dopuszczalny
        /// </summary>
        /// <param name="value">Wejściowy ciąg znaków do sprawdzenia</param>
        /// <param name="position">Pozycja w ciągu znaków</param>
        /// <returns></returns>
        private bool IsValueNotAllowed(string value, int position)
        {
            bool returned_value = false;

            if (Kind == DowiTextBoxKindValue.kvFloat)
            {
                if (!char.IsDigit(value, position)
                    && !value.Contains(".") && !value.Contains(",") && !value.Contains("-"))
                {
                    returned_value = true;
                }
            }
            else if (Kind == DowiTextBoxKindValue.kvInteger)
            {
                if ((!char.IsDigit(value, position) && !value.Contains("-"))
                    || value.Substring(position, 1).Contains(".") || value.Substring(position, 1).Contains(",")
                    )
                {
                    returned_value = true;
                }
            }
            else if (Kind == DowiTextBoxKindValue.kvFloatPositive)
            {
                if ((!char.IsDigit(value, position) || value.Contains("-"))
                    && !value.Substring(position, 1).Contains(".") && !value.Substring(position, 1).Contains(",")
                    )
                {

                    returned_value = true;
                }
            }
            else if (Kind == DowiTextBoxKindValue.kvText)
            {
                if (!char.IsLetter(value, position))
                    returned_value = true;
            }
            return returned_value;
        }

        /// <summary>
        /// Metoda sprawdzająca, jakie wartości są wpisywane do pola
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void poleTekstowe_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsValueNotAllowed(e.Text, e.Text.Length - 1);

        }

        /// <summary>
        /// Metod - zdarzenie wywoływane podczas wklejania wartości w pole tekstowe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(System.Windows.DataFormats.Text, true);
            if (!isText)
                return;

            string value = e.SourceDataObject.GetData(DataFormats.Text) as string;
            for (int i = 0; i < value.Length; i++)
            {
                if (IsValueNotAllowed(value, i))
                    e.CancelCommand();
            }
        }
    }
}
