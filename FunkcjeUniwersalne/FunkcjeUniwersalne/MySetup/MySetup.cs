using DataBaseUniversalFunctions.Model;
using MojeFunkcjeRozszerzajace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace MojeFunkcjeUniwersalneNameSpace
{
    /// <summary>
    /// Klasa służąca do zapisu oraz odczytu konfiguracji
    /// </summary>
    /// <remarks>Udostępnia metody zapisu oraz odczytu konfiguracji z podziałem Sekcje/Parametry/Wartości
    /// Analogicznie do plików INI
    /// Zapis odbywa się do plików XML
    /// Klasa singleton, bez konieczności inicjalizowania, dostęp do metod oraz parametrów przez Instance
    /// Udostępnia metody do automatycznego odczytywania/zapisywania wartości pól w formularzach znajdź, 
    /// zapisywania oraz odczytywania konfiguracji kolumn w DataGrid
    /// </remarks>
    public /*sealed*/ class MySetup 
    {
        #region Parametry i zmiennnie
        /// <summary>
        /// Wskaźnik do okna nadrzędnego w celu poprawnego zapisywania oraz odczytywania wartości pól na formularzu
        /// </summary>
        private Form ParentForm;

        /// <summary>
        /// Nazwa pliku do zapisu/odczytu konfiguracji
        /// </summary>
        private string ConfigFileName;

        /// <summary>
        /// Scieżka do aplikacji
        /// </summary>
        private static string AppCurrentPath;
        //private string TableName;
        /// <summary>
        /// Lista przechowująca konfiguracje: Sekcje/Parametry/Wartosci
        /// </summary>
        [XmlElement("Sekcja")] 
        private List<cKonfiguracja> Konfiguracja;

        /// <summary>
        /// Lista kontrolek, generowana automatycznie, dla metod wewnętrznych w celu zapisu i odczytu wartosci
        /// </summary>
        private List<Control> ListaKontrolek;
        #endregion
        #region Konstruktor i destruktor

        private static volatile MySetup instance;
        private static object syncRoot = new Object();

        /// <summary>
        /// Wystąpienie klasy - Singleton
        /// </summary>
        public static MySetup Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new MySetup();
                        }
                    }
                }

                return instance;
            }
        }

        private MySetup()
        {
            ParentForm = null;
            Konstruktor();
            
        }

        /// <summary>
        /// Metoda wykorzystywana przez konstruktory
        /// </summary>
        private void Konstruktor()
        {
            ConfigFileName = "configuration";

            string fullName = System.Reflection.Assembly.GetEntryAssembly().Location;
            string myName = Path.GetFileNameWithoutExtension(fullName);
            AppCurrentPath = AppDomain.CurrentDomain.BaseDirectory;
            ConfigFileName = myName + "_" + ConfigFileName;
            //TableName = "configuration_table";
            Konfiguracja = new List<cKonfiguracja>();
            GetParamFromDatabase();
            ParentForm = null;
            ListaKontrolek = new List<Control>();           
        }

        /// <summary>
        /// Destruktor, wywoływany podczas Dispose
        /// </summary>
        ~MySetup()
        {
            SetParamToDatabase();

            foreach (cKonfiguracja konf in Konfiguracja)
            {
                konf.Dispose();              
            }
            Konfiguracja.Clear();
            ListaKontrolek.Clear();

        }

        /// <summary>
        /// Metoda przeszukująca formularz nadrzędny, wyszukująca kontrolki
        /// </summary>
        /// <param name="AParentForm">Forma nadrzędna</param>
        /// <remarks>Metoda przeszukująca formularz nadrzędny w celu wyszukania kontrolek których parametry można zapisać, odpala rekurencyjnie metodę wyszukującą kontrolki umieszczone w kontenerach</remarks>
        private void ZnajdzKomponenty(Form AParentForm)
        {
            //if (ParentForm != AParentForm) //zakomentowano, aby zawsze wyszukiwało komponenty, dla komponentów dynamicznie tworzonych
            {                
                ListaKontrolek = AParentForm.GetAllControlsRecursive();
            }
            ParentForm = AParentForm;
        }

        /// <summary>
        /// Metoda przeszukująca kontener
        /// </summary>
        /// <param name="ParentControls">Kontener przeszukiwany</param>
        /// <remarks>Metoda przeszukująca kontenery umieszczone na formularzu nadrzędnym, odpalana rekurencyjnie</remarks>
        //private void ZnajdzKomponenty(Control.ControlCollection ParentControls)
        //{

        //    foreach (Control kontrolka in ParentControls)
        //    {
        //        if (kontrolka is DataGridView
        //          || kontrolka is TextBox
        //          || kontrolka is RadioButton
        //          || kontrolka is CheckBox
        //          || kontrolka is DateTimePicker
        //            //|| kontrolka is Splitter
        //          || kontrolka is ComboBox
        //          || kontrolka is CheckedListBox

        //             )
        //            ListaKontrolek.Add(kontrolka);
        //        else if (kontrolka is TabPage)
        //            ZnajdzKomponenty(kontrolka.Controls);
        //        else if (kontrolka is TabControl)
        //            ZnajdzKomponenty(kontrolka.Controls);
        //        else if (kontrolka is Panel || kontrolka is SplitterPanel)
        //            ZnajdzKomponenty(kontrolka.Controls);
        //        else if (kontrolka is GroupBox)
        //            ZnajdzKomponenty(kontrolka.Controls);
        //        else if (kontrolka is SplitContainer)
        //        {
        //            ListaKontrolek.Add(kontrolka);
        //            ZnajdzKomponenty((kontrolka as SplitContainer).Panel1.Controls);
        //            ZnajdzKomponenty((kontrolka as SplitContainer).Panel2.Controls);
        //        }
        //        else if (kontrolka is Splitter)
        //        {
        //            ListaKontrolek.Add(kontrolka);
        //            //ZnajdzKomponenty((kontrolka as SplitContainer).Panel1.Controls);
        //            //ZnajdzKomponenty((kontrolka as SplitContainer).Panel2.Controls);
        //        }
        //        else if (kontrolka is UserControl)
        //            ZnajdzKomponenty(kontrolka.Controls);
        //    }
        //}
        #endregion

        #region Odczyt i zapis parametrów do pliku xml
        /// <summary>
        /// Metoda inicjalizująca konfigurację, odczytuje dane z pliku XML
        /// </summary>
        public void GetParamFromDatabase()
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<cKonfiguracja>));
				
				string configPath = Path.Combine(AppCurrentPath, (ConfigFileName + ".xml"));
                //using (XmlReader reader = XmlReader.Create(ConfigFileName + ".xml"))
                using (XmlReader reader = XmlReader.Create(configPath))              
                {

                    object obj = deserializer.Deserialize(reader);
                    Konfiguracja = (List<cKonfiguracja>)obj;
                    reader.Close();
                } 
            }
            catch
                {
                    //MessageBox.Show("Blad odczytu ustawien");
                }
        }

        /// <summary>
        /// Metoda zapisująca konfigurację, odpalana automatycznie, podczas DISPOSE, przez destruktor
        /// </summary>
        public void  SetParamToDatabase()
        {

            XmlSerializer serializer = new XmlSerializer(typeof(List<cKonfiguracja>));
            FileMode fileMode = FileMode.CreateNew;
			
			string configPath = Path.Combine(AppCurrentPath, (ConfigFileName + ".xml.tmp"));

            if (File.Exists(configPath))
            {
                fileMode = FileMode.Truncate;
            }

            FileServiceClass fsc = new FileServiceClass();

            if (!fsc.CheckDirectoryAccess(AppCurrentPath))
            {
                return;
            }

            using (Stream stream = new FileStream(configPath, fileMode, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(stream, Konfiguracja);
                stream.Close();
            }
            
            fsc.RenameFile(configPath, string.Format("{0}.xml", ConfigFileName));
            
        }
        #endregion

        #region Odczyt i zapis parametru
        /// <summary>
        /// Metoda odczytująca wartość parametru w podanej sekcji
        /// </summary>
        /// <param name="Sekcja">Nazwa sekcji</param>
        /// <param name="Parametr">Nazwa parametru</param>
        /// <param name="WartoscDefault">Wartość domyślna</param>
        /// <returns>Wartość odczytana z konfiguracji lub wartość domyślna</returns>
        public string GetParam(string Sekcja, string Parametr, string WartoscDefault)
        {
            if (string.IsNullOrEmpty(Sekcja) || string.IsNullOrEmpty(Parametr))
            {
                return WartoscDefault;
            }

            bool Jest = false;
            string WartoscParametru = "";
            cKonfiguracja konfig = null;
            cParametr parametr = null;
            for (int i = 0; i < Konfiguracja.Count; i++)
            {                
                parametr = null;
                if (Konfiguracja[i].Sekcja.Trim().ToUpper().Equals(Sekcja.Trim().ToUpper()))
                {
                    konfig = Konfiguracja[i];
                    for (int j = 0; j < Konfiguracja[i].Parametry.Count; j++)
                    {
                        if (konfig.Parametry[j].Parametr.Trim().ToUpper().Equals(Parametr.Trim().ToUpper()))
                        {
                            parametr = konfig.Parametry[j];
                            Jest = true;
                            WartoscParametru = parametr.Wartosc;
                            //MessageBox.Show("Znalazlem parametr");
                            break;
                        }
                    }
                }
            }

            if (!Jest)
            {
                if (konfig==null)
                {
                    konfig = new cKonfiguracja();
                    konfig.Sekcja = Sekcja.Trim();
                    
                    Konfiguracja.Add(konfig);              
                }
                if (parametr == null)
                {
                    parametr = new cParametr()
                        {
                            Parametr = Parametr.Trim(),
                            Wartosc = WartoscDefault.Trim(),
                            Zmiana = true                            
                        };
                    konfig.Parametry.Add(parametr);
                    
                    WartoscParametru = WartoscDefault.Trim();
                }
            }

            return WartoscParametru;
        }

        /// <summary>
        /// Metoda zapisująca do struktury, wartość parametru w podanej sekcji
        /// </summary>
        /// <param name="Sekcja">Nazwa sekcji</param>
        /// <param name="Parametr">Nazwa parametru</param>
        /// <param name="Wartosc">Wartość do zapisania</param>
        public void SetParam(string Sekcja, string Parametr, string Wartosc)
        {
            if (string.IsNullOrEmpty(Sekcja) || string.IsNullOrEmpty(Parametr))
            {
                return;
            }

            bool Jest = false;
            cKonfiguracja konfig = null;
            cParametr parametr = null;
            for (int i = 0; i < Konfiguracja.Count; i++)
            {               
                //konfig = null;
                parametr = null;
                if (Konfiguracja[i].Sekcja.Trim().ToUpper().Equals(Sekcja.Trim().ToUpper()))
                {
                    konfig = Konfiguracja[i];
                    for (int j=0;j<Konfiguracja[i].Parametry.Count;j++)
                    {
                        if (konfig.Parametry[j].Parametr.Trim().ToUpper().Equals(Parametr.Trim().ToUpper()))
                        {
                            Jest = true;
                            parametr = konfig.Parametry[j];
                            parametr.Wartosc = Wartosc.Trim();
                            parametr.Zmiana = true;
                            //MessageBox.Show("Ustawilem Znalazlem parametr");
                            break;
                        }
                    }
                }
            }

            if (!Jest)
            {
                if (konfig == null)
                {
                    konfig = new cKonfiguracja();
                    konfig.Sekcja = Sekcja.Trim();

                    Konfiguracja.Add(konfig);
                }
                if (parametr == null)
                {
                    parametr = new cParametr()
                    {
                        Parametr = Parametr.Trim(),
                        Wartosc = Wartosc.Trim(),
                        Zmiana = true
                    };
                    konfig.Parametry.Add(parametr);
                }
            }
        }
        #endregion

        #region Konfiguracja kolumn DataGridView

        /// <summary>
        /// Metoda zapisująca szerokości kolumn oraz położenie kolumn
        /// </summary>
        /// <param name="name">Dodatkowy przyrostek nazwy tabeli</param>
        /// <param name="DG">Wskaźnik do dataGridView</param>
        public void SaveColumnsLayout(string name, DataGridView DG)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = "_" + name;
            }

            for (int i = 0; i < DG.Columns.Count; i++)
            {                
                SetParam(ParentForm.Name + "_" + DG.Parent.Name + "_" + DG.Name+name, "Column_"+DG.Columns[i].Name+ "_Width", DG.Columns[i].Width.ToString());
                SetParam(ParentForm.Name + "_" + DG.Parent.Name + "_" + DG.Name+name, "Column_" + DG.Columns[i].Name + "_Index", DG.Columns[i].DisplayIndex.ToString());
            }
        }

        /// <summary>
        /// Metoda odczytująca szerokości kolumn oraz ustawiająca kolejność
        /// </summary>
        /// <param name="name">Dodatkowy przyrostek nazwy tabeli</param>
        /// <param name="DG">Wskaźnik do dataGridView</param>
        public void ReadColumnsLayout(string name, DataGridView DG)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = "_" + name;
            }

            try
            {
                for (int i = 0; i < DG.Columns.Count; i++)
                {
                    DG.Columns[i].Width = Convert.ToInt32(GetParam(ParentForm.Name + "_" + DG.Parent.Name + "_" + DG.Name+name, "Column_" + DG.Columns[i].Name + "_Width", DG.Columns[i].Width.ToString()));
                    if (DG.AllowUserToOrderColumns)
                    {
                        DG.Columns[i].DisplayIndex = Convert.ToInt32(GetParam(ParentForm.Name + "_" + DG.Parent.Name + "_" + DG.Name+name, "Column_" + DG.Columns[i].Name + "_Index", DG.Columns[i].DisplayIndex.ToString()));
                    }
                }
            }
            catch
            {

            }


        }

        /// <summary>
        /// Metoda zapisująca szerokości kolumn oraz położenie kolumn
        /// </summary>
        /// <param name="DG">Wskaźnik do dataGridView</param>
        public void SaveColumnsLayout(DataGridView DG)
        {
            SaveColumnsLayout(string.Empty, DG);
        }

        /// <summary>
        /// Metoda odczytująca szerokości kolumn oraz ustawiająca kolejność
        /// </summary>
        /// <param name="DG">Wskaźnik do dataGridView</param>
        public void ReadColumnsLayout(DataGridView DG)
        {
            ReadColumnsLayout(string.Empty, DG);
        }
        #endregion

        #region Konfiguracja splitterów


        private int getSplitterFullDistance(SplitContainer stContainer)
        {
            return stContainer.Orientation == Orientation.Horizontal ? stContainer.Size.Height : stContainer.Size.Width;
        }
        /// <summary>
        /// Metoda zapisująca położenie splitterów na formularzach
        /// </summary>
        /// <param name="splitter">Wskaźnik do splittera</param>
        private void SaveSplitterData(Control splitter)
        {
            int distancePercent = 0;
            if (splitter is SplitContainer)
            {
                SplitContainer spl = splitter as SplitContainer;

                distancePercent =
                   spl.FixedPanel == FixedPanel.Panel1 ? spl.SplitterDistance :
                   spl.FixedPanel == FixedPanel.Panel2 ? getSplitterFullDistance(spl) - spl.SplitterDistance :
                   (int)((((double)spl.SplitterDistance) / getSplitterFullDistance(spl)) * 100);
                
            }
            else if (splitter is Splitter)
            {
                distancePercent = (splitter as Splitter).SplitPosition;
            }

            string name = GetControlParentName(splitter.Parent) + "_" + splitter.Name;
            SetParam(name , "Distance", distancePercent.ToString());
        }
        /// <summary>
        /// Metoda odczytująca położenie splittera na formularzu
        /// </summary>
        /// <param name="splitter">Wskaźnik do splittera</param>
        private void LoadSplitterData(Control control)
        {
            try
            {
                if (control is SplitContainer)
                {
                    SplitContainer splitContainer = control as SplitContainer;
                    DockStyle dock = splitContainer.Dock;
                    splitContainer.Dock = DockStyle.None;
                    string name = GetControlParentName(splitContainer.Parent) + "_" + control.Name;
                    int storedDistance = Convert.ToInt32(GetParam(name, "Distance", (getSplitterFullDistance(splitContainer) / 2).ToString()));

                    int distanceToRestore =
                       splitContainer.FixedPanel == FixedPanel.Panel1 ? storedDistance :
                       splitContainer.FixedPanel == FixedPanel.Panel2 ? getSplitterFullDistance(splitContainer) - storedDistance :
                       storedDistance * getSplitterFullDistance(splitContainer) / 100;

                    splitContainer.SplitterDistance = distanceToRestore;
                    splitContainer.Dock = dock;
                }
                else if (control is Splitter)
                {
                    Splitter splitter = control as Splitter;
                    string name = GetControlParentName(splitter.Parent) + "_" + control.Name;
                    int distancePercent = Convert.ToInt32(GetParam(name, "Distance", splitter.SplitPosition.ToString()));
                    splitter.SplitPosition = distancePercent;
                }
            }
            catch
            {

            }
        }
        #endregion

        #region Konfiguracja Layouta formularzy
        /// <summary>
        /// Metoda odczytująca i ustawiająca Layout pól na formularzach (położenie splitterów, szerokości kolumn dataGridView
        /// </summary>
        /// <param name="AParentForm">Wskaźnik do formularza</param>
        public void SetFormLayout(Form AParentForm)
        {
            ZnajdzKomponenty(AParentForm);
            for (int i = 0; i < ListaKontrolek.Count; i++)
            {
                if (ListaKontrolek[i] is DataGridView)
                {
                    ReadColumnsLayout((DataGridView)ListaKontrolek[i]);
                }
                else if (ListaKontrolek[i] is SplitContainer)
                {
                    LoadSplitterData((SplitContainer)ListaKontrolek[i]);
                }
                else if (ListaKontrolek[i] is Splitter)
                {
                    LoadSplitterData((Splitter)ListaKontrolek[i]);
                }
                else if (ListaKontrolek[i] is ComboBox)
                {
                    SetComboBoxBehavior2((ComboBox)ListaKontrolek[i]);
                }
            }
            //TODO: Sprawdzać czy ustawienie formularza nie wykroczy poza ekran
            if (GetParam(AParentForm.Name, "WindowState", "Normal")=="Normal")
            {
                try
                {
                    AParentForm.Left = Convert.ToInt32(GetParam(AParentForm.Name, "Left", AParentForm.Left.ToString()));
                    AParentForm.Top = Convert.ToInt32(GetParam(AParentForm.Name, "Top", AParentForm.Top.ToString()));
                    AParentForm.Width = Convert.ToInt32(GetParam(AParentForm.Name, "Width", AParentForm.Width.ToString()));
                    AParentForm.Height = Convert.ToInt32(GetParam(AParentForm.Name, "Height", AParentForm.Height.ToString()));
                }
                catch
                {

                }
            }
            else
            {
                AParentForm.WindowState = FormWindowState.Maximized;
            }
            
            
        }

       

        /// <summary>
        /// Metoda zapisująca Layout pól na formularzach (położenie splitterów, szerokości kolumn dataGridView
        /// </summary>
        /// <param name="AParentForm">Wskaźnik do formularza</param>
        public void SaveFormLayout(Form AParentForm)
        {
            ZnajdzKomponenty(AParentForm);
            
            for (int i = 0; i < ListaKontrolek.Count; i++)
            {
                if (ListaKontrolek[i] is DataGridView)
                {
                    SaveColumnsLayout((DataGridView)ListaKontrolek[i]);
                }
                else if (ListaKontrolek[i] is SplitContainer)
                {
                    SaveSplitterData((SplitContainer)ListaKontrolek[i]);
                }
                else if (ListaKontrolek[i] is Splitter)
                {
                    SaveSplitterData(ListaKontrolek[i]);
                }
            }


            if (AParentForm.WindowState==FormWindowState.Maximized)
            {
                SetParam(AParentForm.Name, "WindowState", "Maximized");
            }
            else
            {
                SetParam(AParentForm.Name, "WindowState", "Normal");
                SetParam(AParentForm.Name, "Width", AParentForm.Width.ToString());
                SetParam(AParentForm.Name, "Height", AParentForm.Height.ToString());
                SetParam(AParentForm.Name, "Left", AParentForm.Left.ToString());
                SetParam(AParentForm.Name, "Top", AParentForm.Top.ToString());
            }

            
        }
        
        #endregion

        #region Konfiguracja formularzy znajdz
        /// <summary>
        /// Wyczyść wszystkie kontrolki na formularzu
        /// </summary>
        /// <param name="AParentForm">Wskaźnik do formularza</param>
        /// <remarks>Obsługuje pola:
        /// TextBox, CheckBox, RadioButton, DateTimePicker, ComboBox, CheckListBox</remarks>
        public void ClearSearchFields(Form AParentForm)
        {
            ZnajdzKomponenty(AParentForm);

            foreach (Control kontrolka in ListaKontrolek)
            {
                if (kontrolka is TextBox)
                {
                    ((TextBox)kontrolka).Text = "";
                }
                else if (kontrolka is CheckBox)
                {
                    ((CheckBox)kontrolka).Checked = false;
                }
                else if (kontrolka is RadioButton)
                {
                    ((RadioButton)kontrolka).Checked = false;
                }
                else if (kontrolka is DateTimePicker)
                {
                    ((DateTimePicker)kontrolka).Checked = false;
                }
                else if (kontrolka is ComboBox)
                {
                    ((ComboBox)kontrolka).SelectedIndex = -1;
                }
                else if (kontrolka is CheckedListBox)
                {
                    for (int i = 0; i < ((CheckedListBox)kontrolka).Items.Count; i++)
                    {
                        ((CheckedListBox)kontrolka).SetItemChecked(i, false);
                    }
                }
            }
        }

        /// <summary>
        /// Zapisz wartości kontrolek na formularzu wyszukiwania
        /// </summary>
        /// <param name="AParentForm">Wskaźnik do formularza</param>
        /// <remarks>Obsługuje pola:
        /// TextBox, CheckBox, RadioButton, DateTimePicker, ComboBox, CheckListBox</remarks>
        public void SaveSearchFields(Form AParentForm)
        {
            ZnajdzKomponenty(AParentForm);
            foreach (Control kontrolka in ListaKontrolek)
            {
                string name = GetControlParentName(kontrolka.Parent);
                if (kontrolka is TextBox)
                {
                    if (!((TextBox)kontrolka).ReadOnly)
                        SetParam(name, ((TextBox)kontrolka).Name, ((TextBox)kontrolka).Text);
                }
                else if (kontrolka is CheckBox)
                {
                    SetParam(name, ((CheckBox)kontrolka).Name, ((CheckBox)kontrolka).Checked.ToString());
                }
                else if (kontrolka is RadioButton)
                {
                    SetParam(name, ((RadioButton)kontrolka).Name, ((RadioButton)kontrolka).Checked.ToString());
                }
                else if (kontrolka is DateTimePicker)
                {
                    if (((DateTimePicker)kontrolka).Checked)
                    {
                        SetParam(name, ((DateTimePicker)kontrolka).Name, ((DateTimePicker)kontrolka).Value.ToString());
                    }
                    else
                    {
                        SetParam(name, ((DateTimePicker)kontrolka).Name, "");
                    }
                }
                else if (kontrolka is ComboBox)
                {
                    var kontrolkaComboBox = kontrolka as ComboBox;
                    Debug.WriteLine(kontrolkaComboBox.Items.GetType());
                    var position = kontrolkaComboBox.SelectedItem;
                    if (position!=null && position is DictionaryListItem)                                                                                            
                    {
                        var identityKey = ((DictionaryListItem)position).IdentityKey;
                        var identity = ((DictionaryListItem)position).Identity;
                        SetParam(name, $"{kontrolkaComboBox.Name}", string.IsNullOrEmpty(identityKey) ? identity.ToString() : identityKey);                                
                    }   
                    else
                    {
                        SetParam(name, $"{kontrolkaComboBox.Name}", "-1");
                    }
                }
                else if (kontrolka is CheckedListBox)
                {
                    var kontrolkaCheckedListBox = kontrolka as CheckedListBox;
                    Debug.WriteLine(kontrolkaCheckedListBox.Items.GetType());
                    int position = 0;
                    for (int i = 0; i < kontrolkaCheckedListBox.Items.Count; i++)
                    {
                        if (kontrolkaCheckedListBox.GetItemChecked(i))
                        {
                            var item = kontrolkaCheckedListBox.Items[i];
                            if (item is DictionaryListItem)
                            {
                                var identityKey = ((DictionaryListItem)item).IdentityKey;
                                var identity = ((DictionaryListItem)item).Identity;                                
                                SetParam(name, $"{kontrolkaCheckedListBox.Name}_{position}", string.IsNullOrEmpty(identityKey) ? identity.ToString() : identityKey);
                                position++;
                            }
                        }
                    }
                    if (position>0)
                        SetParam(name, $"{kontrolkaCheckedListBox.Name}_Count", position.ToString());
                }
            }
        }

        /// <summary>
        /// Odczytaj i ustaw wartości kontrolek na formularzu wyszukiwania
        /// </summary>
        /// <param name="AParentForm">Wskaźnik do formularza</param>
        /// <remarks>Obsługuje pola:
        /// TextBox, CheckBox, RadioButton, DateTimePicker, ComboBox, CheckListBox</remarks>
        public void LoadSearchFields(Form AParentForm)
        {
            ZnajdzKomponenty(AParentForm);
            foreach (Control kontrolka in ListaKontrolek)
            {
                string name = GetControlParentName(kontrolka.Parent);
                if (kontrolka is TextBox)
                {
                    if (!((TextBox)kontrolka).ReadOnly)
                        ((TextBox)kontrolka).Text = GetParam(name, ((TextBox)kontrolka).Name, ((TextBox)kontrolka).Text);
                }
                else if (kontrolka is CheckBox)
                {
                    ((CheckBox)kontrolka).Checked = Convert.ToBoolean(GetParam(name, ((CheckBox)kontrolka).Name, ((CheckBox)kontrolka).Checked.ToString()));
                }
                else if (kontrolka is RadioButton)
                {
                    ((RadioButton)kontrolka).Checked = Convert.ToBoolean(GetParam(name, ((RadioButton)kontrolka).Name, ((RadioButton)kontrolka).Checked.ToString()));
                }
                else if (kontrolka is DateTimePicker)
                {
                    if (GetParam(name, ((DateTimePicker)kontrolka).Name, "").Length>0 )
                    {
                        ((DateTimePicker)kontrolka).Value = Convert.ToDateTime(GetParam(name, ((DateTimePicker)kontrolka).Name, ((DateTimePicker)kontrolka).Value.ToString()));
                    }
                }
                else if (kontrolka is ComboBox)
                {
                    var kontrolkaComboBox = kontrolka as ComboBox;
                    var parentControlName = GetControlParentName(kontrolkaComboBox.Parent);
                    var value = GetParam(parentControlName, kontrolkaComboBox.Name, "-1");
                    bool toIntConvResult = Int32.TryParse(value, out int valueInt);
                    if (toIntConvResult && valueInt>-1)
                        SetComboBoxitem(kontrolkaComboBox, valueInt);
                    else if (!toIntConvResult && value!="-1")
                    {
                        foreach (var item in kontrolkaComboBox.Items)
                        {
                            if (item is DictionaryListItem && (item as DictionaryListItem).IdentityKey == value)
                            { 
                                kontrolkaComboBox.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
                else if (kontrolka is CheckedListBox)
                {
                    var kontrolkaCheckedListBox = kontrolka as CheckedListBox;                    
                    var parentControlName = GetControlParentName(kontrolkaCheckedListBox.Parent);
                    int count = Convert.ToInt32(GetParam(parentControlName, kontrolkaCheckedListBox.Name + "_Count", "0"));
                    for (int i = 0; i < count; i++)
                    {
                        var value = GetParam(parentControlName, kontrolkaCheckedListBox.Name + "_" + i.ToString(), "-1");
                        Setlistboxitem(kontrolkaCheckedListBox, value);
                    }

                }
            }
        }

        /// <summary>
        /// Ustatiwa wartość pola wyszukiwania typu ComboBox
        /// </summary>
        /// <param name="kontrolkaComboBox">Kontrolka formularza znajdź</param>
        /// <param name="value">Jeżeli wartość odczytana -1, to nie przypisuje</param>
        private void SetComboBoxitem(ComboBox kontrolkaComboBox, int value)
        {
            if (value < 0)
                return;

            for(int i=0;i<kontrolkaComboBox.Items.Count;i++)
            {
                if (kontrolkaComboBox.Items[i] is DictionaryListItem)
                {
                    var item = kontrolkaComboBox.Items[i] as DictionaryListItem;
                    if (item.Identity==value)
                    {
                        kontrolkaComboBox.SelectedIndex = i;
                        break;
                    }

                }

            }
        }

        /// <summary>
        /// Ustawia zaznaczone pozycje, zapisane wcześniej w konfiguracji dla pola CheckedListBox
        /// </summary>
        /// <param name="kontrolkaCheckedListBox"></param>
        /// <param name="value"></param>
        private void Setlistboxitem(CheckedListBox kontrolkaCheckedListBox, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }            
            
            for (int i = 0; i < kontrolkaCheckedListBox.Items.Count; i++)
            {
                if (kontrolkaCheckedListBox.Items[i] is DictionaryListItem)
                {
                    var dictItem = (DictionaryListItem)kontrolkaCheckedListBox.Items[i];
                    var isIntegerValue = int.TryParse(value, out int integerValue);
                    if (dictItem.IdentityKey == value || (isIntegerValue && dictItem.Identity == integerValue))
                    {
                        kontrolkaCheckedListBox.SetItemChecked(i, true);
                        break;
                    }                        
                }                    
            }            
        }

        /// <summary>
        /// Metoda zapisująca wybraną pozycję pola Combo
        /// </summary>
        /// <param name="PoleCombo">Wskaźnik do ComboBox</param>
        /// <param name="ListaIndeksow">Opcjonalna lista indeksów wg których rozpoznawane pozycje</param>
        /// <remarks>Zapisuje która pozycja listy została wybrana, jeżeli podano listę indeksów wówczas rozpoznaje wg indeksu
        /// w przeciwnym razie wg pozycji na liście</remarks>
        public void SaveComboBoxField(ComboBox PoleCombo, List<int> ListaIndeksow=null)
        {
            int ValueToSave = -1;
            ValueToSave = PoleCombo.SelectedIndex;

            if (ListaIndeksow != null && ValueToSave>-1 && ValueToSave<ListaIndeksow.Count)
            {
                ValueToSave = ListaIndeksow[ValueToSave];
            }

            string name = GetControlParentName(PoleCombo.Parent);
            SetParam(name, PoleCombo.Name, ValueToSave.ToString());
        }

        /// <summary>
        /// Metoda zapisująca wybraną pozycję pola Combo
        /// </summary>
        /// <param name="PoleCombo">Wskaźnik do ComboBox</param>
        /// <param name="ListaIndeksow">Opcjonalna lista indeksów wg których rozpoznawane pozycje</param>
        /// <remarks>Zapisuje która pozycja listy została wybrana, jeżeli podano listę indeksów wówczas rozpoznaje wg indeksu
        /// w przeciwnym razie wg pozycji na liście</remarks>
        public void SaveComboBoxField(ToolStripComboBox PoleCombo, List<int> ListaIndeksow)
        {
            int ValueToSave = -1;
            ValueToSave = PoleCombo.SelectedIndex;

            if (ListaIndeksow != null && ValueToSave > -1 && ValueToSave < ListaIndeksow.Count)
            {
                ValueToSave = ListaIndeksow[ValueToSave];
            }

            string name = GetControlParentName(PoleCombo.Owner);
            SetParam(name, PoleCombo.Name, ValueToSave.ToString());
        }

        private string GetControlParentName(Control control)
        {
            string returnedName = string.Empty;
            returnedName += control.Name;            
            if (control.Parent != null)
            {
                if (returnedName.Length>0)
                {
                    returnedName = "_"+returnedName;
                }

                returnedName = GetControlParentName(control.Parent) + returnedName;                
            }
            return returnedName;
        }

        /// <summary>
        /// Metoda zapisująca zaznaczenie kontrolek CheckListBox
        /// </summary>
        /// <param name="PoleListy">Wskaźnik do CheckListBox</param>
        /// <param name="ListaIndeksow">Opcjonalna lista indeksów wg których rozpoznawane pozycje</param>
        /// <remarks>Zapisuje które pozycje listy zostały zaznaczone, jeżeli nie podano listy indeksów, wówczas zapisuje wg położenia na liście, 
        /// w przeciwnym wypadku zapisuje wg indeksów z listy indeksów</remarks>
        public void SaveListBoxField(CheckedListBox PoleListy, List<int> ListaIndeksow)
        {
            int Count = 0;
            string name = GetControlParentName(PoleListy.Parent);
            for (int i = 0; i < PoleListy.Items.Count; i++)
            {
                if (PoleListy.GetItemChecked(i))
                {
                    
                    if (ListaIndeksow != null)
                    {
                        SetParam(name, PoleListy.Name + "_"+Count.ToString(), ListaIndeksow[i].ToString());
                    }
                    else
                    {
                        SetParam(name, PoleListy.Name + "_" + Count.ToString(), i.ToString());
                    }

                    Count++;                   
                }
            }
            SetParam(name, PoleListy.Name+"_Count", Count.ToString());
        }

        /// <summary>
        /// Metoda odczytująca wybraną pozycję pola Combo
        /// </summary>
        /// <param name="PoleCombo">Wskaźnik do ComboBox</param>
        /// <param name="ListaIndeksow">Opcjonalna lista indeksów wg których rozpoznawane pozycje</param>
        /// <remarks>Odczytuje która pozycja listy została wybrana, jeżeli podano listę indeksów wówczas rozpoznaje wg indeksu
        /// w przeciwnym razie wg pozycji na liście</remarks>
        public void LoadComboBoxField(ComboBox PoleCombo, List<int> ListaIndeksow=null)
        {
            int ValueToLoad = -1;
            ValueToLoad = Convert.ToInt32(GetParam(GetControlParentName(PoleCombo.Parent), PoleCombo.Name, "-1"));


            if (ListaIndeksow != null)
            {
                for (int i = 0; i < ListaIndeksow.Count; i++)
                {
                    if (ListaIndeksow[i] == ValueToLoad)
                    {
                        ValueToLoad = i;
                        break;
                    }
                }
            }

            if (PoleCombo.Items.Count>ValueToLoad)
            {
                PoleCombo.SelectedIndex = ValueToLoad;
            }
        }

        /// <summary>
        /// Metoda odczytująca wybraną pozycję pola Combo
        /// </summary>
        /// <param name="PoleCombo">Wskaźnik do ComboBox</param>
        /// <param name="ListaIndeksow">Opcjonalna lista indeksów wg których rozpoznawane pozycje</param>
        /// <remarks>Odczytuje która pozycja listy została wybrana, jeżeli podano listę indeksów wówczas rozpoznaje wg indeksu
        /// w przeciwnym razie wg pozycji na liście</remarks>
        public void LoadComboBoxField(ToolStripComboBox PoleCombo, List<int> ListaIndeksow)
        {
            int ValueToLoad = -1;
            ValueToLoad = Convert.ToInt32(GetParam(GetControlParentName(PoleCombo.Owner), PoleCombo.Name, "-1"));


            if (ListaIndeksow != null)
            {
                for (int i = 0; i < ListaIndeksow.Count; i++)
                {
                    if (ListaIndeksow[i] == ValueToLoad)
                    {
                        ValueToLoad = i;
                        break;
                    }
                }
            }

            if (PoleCombo.Items.Count > ValueToLoad)
            {
                PoleCombo.SelectedIndex = ValueToLoad;
            }
        }
        /// <summary>
        /// Metoda odczytująca zaznaczenie kontrolek CheckListBox
        /// </summary>
        /// <param name="PoleListy">Wskaźnik do CheckListBox</param>
        /// <param name="ListaIndeksow">Opcjonalna lista indeksów wg których rozpoznawane pozycje</param>
        /// <remarks>Odczytuje które pozycje listy zostały zaznaczone, jeżeli nie podano listy indeksów, wówczas odczytuje wg położenia na liście, 
        /// w przeciwnym wypadku odczytuje wg indeksów z listy indeksów</remarks>
        public void LoadListBoxField(CheckedListBox PoleListy, List<int> ListaIndeksow)
        {
            string name = GetControlParentName(PoleListy.Parent);
            int Count = Convert.ToInt32(GetParam(name, PoleListy.Name + "_Count", "0"));
            for (int i = 0; i < Count; i++)
            {
                int Indeks = Convert.ToInt32(GetParam(name, PoleListy.Name + "_" + i.ToString(), "-1"));
                setlistboxitem(PoleListy, ListaIndeksow, Indeks); 
            }

        }
        /// <summary>
        /// Metoda sprawdzająca czy pozycja listy powinna być zaznaczona
        /// </summary>
        /// <param name="PoleListy">Wskaźnik do CheckListBox</param>
        /// <param name="ListaIndeksow">Opcjonalna taleba z indeksami pozycji</param>
        /// <param name="indeks">Indeks z tabeli indeksów, lub nr pozycji na liście</param>
        private void setlistboxitem(CheckedListBox PoleListy, List<int> ListaIndeksow, int indeks)
        {
            if (indeks<0)
            {
                return;
            }

            if (ListaIndeksow != null)
            {
                for (int i = 0; i < ListaIndeksow.Count; i++)
                {
                    if (ListaIndeksow[i] == indeks)
                    {
                        PoleListy.SetItemChecked(i, true);
                        break;
                    }
                }
            }
            else
            {
                PoleListy.SetItemChecked(indeks, true);
            }
        }

        #endregion

        #region Komponent ComboBox
        
        /// <summary>
        /// Metoda ustawiająca właściwości i zdarzenia kontrolek ComboBox, w celu poprawnego zachowania podczas wybierania wartości z klawiatury
        /// możłiwa inicjalizacja tylko pierwszym znakiem
        /// </summary>
        /// <param name="comboBox"></param>     
        private void SetComboBoxBehavior2(ComboBox comboBox)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDown;

            comboBox.AutoCompleteMode = AutoCompleteMode.None;
            comboBox.AutoCompleteSource = AutoCompleteSource.None;
            
            comboBox.KeyPress += new KeyPressEventHandler(HandleKeyPress);
        }

        
        int ValueInteraction = 0; //tylko wybór pozycji z listy
        private void HandleKeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine("HandleKeyPress");
            ComboBox comboBox = sender as ComboBox;
            try
            {
                //comboBox.BackColor = System.Drawing.Color.Green;
                string TmpStr;

                bool BackSpace = (e.KeyChar == (char)Keys.Back);
                if (BackSpace && comboBox.SelectionLength>0)
                {
                    TmpStr = comboBox.Text.SubStringEx(0, comboBox.SelectionStart) + comboBox.Text.SubStringEx(comboBox.SelectionLength + comboBox.SelectionStart+1, 255);
                }
                else if (BackSpace) // SelLength == 0
                {
                    TmpStr = comboBox.Text.SubStringEx(0, comboBox.SelectionStart - 1) + comboBox.Text.SubStringEx(comboBox.SelectionStart+1, 255);
                }
                else //Key is a visible character
                {
                    TmpStr = comboBox.Text.SubStringEx(0, comboBox.SelectionStart) + e.KeyChar + comboBox.Text.SubStringEx(comboBox.SelectionLength + comboBox.SelectionStart+1, 255);
                }

                if (string.IsNullOrEmpty(TmpStr))
                {
                    Debug.WriteLine($"TmpStr=isEmpty");
                    return;
                }
                Debug.WriteLine($"TmpStr={TmpStr}");
                // set SelSt to the current insertion point
                int SelSt = comboBox.SelectionStart;

                if (ValueInteraction == 0) //Sprawdzenie warunku czy tylko wybór, czy również dopisywanie
                {
                    e.KeyChar = (char)0;
                }

                if (BackSpace && SelSt > 0)
                {
                    SelSt--;
                }
                else if (!BackSpace)
                {
                    SelSt++;
                }

                if (SelSt == 0)
                {
                    comboBox.Text = "";
                    comboBox.SelectedIndex = -1;
                    return;
                }

                // Now that TmpStr is the currently typed string, see if we can locate a match

                bool Found = false;
                for (int i = 1; i <= comboBox.Items.Count; i++)
                {
                    string value = comboBox.Items[i - 1].ToString().SubStringEx(0, TmpStr.Length).ToUpper();
                        
                    if (TmpStr.ToUpper() == value)
                    {
                        if (ValueInteraction == 1)
                        {
                            e.KeyChar = (char)0;
                        }

                        comboBox.DroppedDown = false;
                        comboBox.Text = comboBox.Items[i - 1].ToString(); // update to the match that was found
                        comboBox.SelectedIndex = i - 1;
                        Found = true;
                        break;
                    }
                }
                    
                if (Found) // select the untyped end of the string
                {
                    comboBox.SelectionStart = SelSt;
                    comboBox.SelectionLength = comboBox.Text.Length - SelSt;
                    Debug.WriteLine("SelectioStart=" + SelSt.ToString());
                }
                e.Handled = true;
                //comboBox.BackColor = System.Drawing.Color.Blue;
            }
            catch (Exception ex)
            {
                //ShowMessage("Error in KeyPress: row:=" + IntToStr(row) + "; " + ex.Message);
                Debug.WriteLine("Error in KeyPress: row:=" + ex.Message);
            }

        }

        public void SetComboBoxDefaultBehavior(ComboBox comboBox)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDown;

            comboBox.AutoCompleteMode = AutoCompleteMode.None;
            comboBox.AutoCompleteSource = AutoCompleteSource.None;

            comboBox.KeyPress -= HandleKeyPress;
        }
        #endregion


    }
}
