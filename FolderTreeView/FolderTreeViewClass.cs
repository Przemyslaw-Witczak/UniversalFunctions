using FbKlientNameSpace;
using FirebirdSql.Data.FirebirdClient;
using MojeFunkcjeUniwersalneNameSpace;
using MojeFunkcjeUniwersalneNameSpace.Files;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderTreeView
{
    /// <summary>
    /// Klasa zawierająca metody do rysowania drzewa
    /// </summary>
    public class FolderTreeViewClass
    {
        /// <summary>
        /// Kontrolka status bar na której można wyświetlać etapy rysowania
        /// </summary>
        public ToolStripStatusLabel StatusBar { get; set; }

        /// <summary>
        /// Właściwość sprawdzania przed wyświetleniem gałęzi czy odczytany z bazy danych katalog istnieje na dysku
        /// </summary>
        public bool CheckDirectoryExists { get; set; } = true;
        private readonly TreeView _libraryTree;
        private readonly Form _parent;
        private readonly FileServiceClass _fileClass;
        private ObservableCollection<WpfTreeNodeModel> _wpfTreeNodeModel;

        /// <summary>
        /// Konstruktor klasy wyświetlającej drzewo
        /// </summary>
        /// <param name="libraryTree">Komponent na którym wyświetlić drzewo</param>
        /// <param name="fileClass">Komponent zawierający obiekty struktury katalogów</param>
        public FolderTreeViewClass(TreeView libraryTree, FileServiceClass fileClass)
        {
            _libraryTree = libraryTree;
            _fileClass = fileClass;
            _parent = _libraryTree.Parent as Form;
        }

        /// <summary>
        /// Konstruktor klasy wyświetlającej drzewo
        /// </summary>
        /// <param name="wpfTreeNodeModel">Struktura drzewa bindowana do widoku</param>
        /// <param name="fileClass">Komponent zawierający obiekty struktury katalogów</param>
        public FolderTreeViewClass(ObservableCollection<WpfTreeNodeModel> wpfTreeNodeModel, FileServiceClass fileClass)
        {
            _wpfTreeNodeModel = wpfTreeNodeModel;
            _fileClass = fileClass;
        }

        /// <summary>
        /// Konstruktor klasy wyświetlającej drzewo
        /// </summary>
        /// <param name="libraryTree">Komponent na którym wyświetlić drzewo</param>
        /// <param name="fileClass">Komponent zawierający obiekty struktury katalogów</param>
        public FolderTreeViewClass(FileServiceClass fileClass)
        {            
            _fileClass = fileClass;
            _parent = _libraryTree.Parent as Form;
        }

        /// <summary>
        /// Właściwość zawierająca ścieżkę do katalogu głównego (root) biblioteki-drzewa
        /// </summary>
        public string LibraryPath { get; set; }

        /// <summary>
        /// Metoda odczytująca strukturę poprzednio odczytanej biblioteki, i wyświetlająca na drzewie
        /// </summary>
        public void InicjalizacjaBiblioteki()
        {
            //FolderElement folder = new FolderElement() { Path = LibraryPath, Name = Path.GetDirectoryName(LibraryPath) };
            FolderElement folder = new FolderElement() { Path = LibraryPath, Name = LibraryPath };
            _fileClass.DirRoot = folder;

            Dictionary<int, FolderElement> strukturaKatalogow = new Dictionary<int, FolderElement> {{0, folder}};
            
            using (var klient = new FbKlient(_parent))
            {
                klient.AddSQL("select * from WYSWIETL_DRZEWO");
                klient.Execute();


                while (klient.Read())
                {
                    if (klient.GetInt16("identyfikator") == 1)
                    {
                        folder = new FolderElement
                        {
                            Name = klient.GetString("folder_name"),
                            Indeks = klient.GetInt32("id_katalogi"),
                            ImageIndex = 0
                        };

                        if (!klient.IsDBNull("id_katalogi_parent"))
                        {
                            folder.IdKatalogiParent = klient.GetInt32("id_katalogi_parent");
                        }
                        else
                        {
                            folder.IdKatalogiParent = 0;
                        }

                        FolderElement parent = strukturaKatalogow.First(x => x.Key == folder.IdKatalogiParent).Value;
                        folder.ParentFolder = parent;
                        parent.Add(folder);

                        //if (fileClass.DirRoot == null)
                        //    fileClass.DirRoot = folder;

                        strukturaKatalogow.Add(folder.Indeks, folder);
                    }
                    else if (klient.GetInt16("identyfikator") == 2)
                    {
                        FileElement file = new FileElement
                        {
                            Indeks = klient.GetInt32("id_pliki"),
                            Name = klient.GetString("file_name"),
                            Path = klient.GetString("file_path"),
                            SHA256 = klient.GetString("sha256"),
                            IdKatalogiParent = klient.GetInt32("id_katalogi_parent")
                        };
                        FolderElement parent = strukturaKatalogow.First(x => x.Key == file.IdKatalogiParent).Value;
                        file.ParentFolder = parent;

                        parent.Add(file);
                    }
                }
            }

            InitializeTreeView();
        }

        /// <summary>
        /// Metoda wyświetlająca drzewo obiektu FileServiceClass na komponencie TreeView
        /// </summary>
        public void InitializeTreeView()
        {
            if (_libraryTree == null && _wpfTreeNodeModel == null)
            {
                throw new Exception("Error initializing TreeView!");
            }

            if (_libraryTree != null)
            {
                _libraryTree.Nodes.Clear();
                if (_fileClass == null)
                {
                    throw new Exception($"FolderTreeViewClass::InitializeTreeView, _fileClass not set.");
                }

                if (_fileClass.DirRoot == null)
                {
                    return;
                }

                if (StatusBar != null)
                {
                    StatusBar.Text = @"Rysowanie drzewa";
                }

                TreeNode node = _libraryTree.Nodes.Add(_fileClass.DirRoot.Name);
                node.Tag = _fileClass.DirRoot;
                _libraryTree.BeginUpdate();
                _libraryTree.Nodes.Clear();
                AddNode(_fileClass.DirRoot, node);
                _libraryTree.Nodes.Add(node);
                _libraryTree.Sort();
                _libraryTree.EndUpdate();
                _libraryTree.ExpandAll();
                if (StatusBar != null)
                {
                    StatusBar.Text = @"Zakończono rysowanie drzewa";
                }
            }
            else if (_wpfTreeNodeModel!=null)
            {
                //ApplicationStatus = "Rysowanie drzewa";
                _wpfTreeNodeModel.Clear();
                //WpfTreeNodeModel.OnSelectedItemChanged = OnSelectedFolder;
                var node = new WpfTreeNodeModel() { Name = _fileClass.DirRoot.Name };
                AddNode(_fileClass.DirRoot, node);
                _wpfTreeNodeModel.Add(node);
                //_wpfTreeNodeModel = new ObservableCollection<WpfTreeNodeModel>(_wpfTreeNodeModel.OrderBy(folder => folder.Name));
                //ApplicationStatus = "Zakończono rysowanie drzewa";
                
            }
        }


        /// <summary>
        /// Metoda dodaje gałąź pod wskazaną gałąź nadrzędną
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="parentNode"></param>
        private void AddNode(IPathElement parentFolder, TreeNode parentNode)
        {
            try
            {
                var element = parentFolder as FolderElement;
                if (element != null)
                {
                    foreach (IPathElement folderElement in element.Children)
                    {
                        if (folderElement is FolderElement)
                        {
                            if (!CheckDirectoryExists || Directory.Exists(folderElement.GetPathFromParent()))
                            {
                                var kategoria = folderElement as FolderElement;
                                TreeNode node = new TreeNode($"{folderElement.Name} [{kategoria.GetChildFoldersList()?.Count}][{kategoria.GetChildFilesList()?.Count}]");
                                if (folderElement.ImageIndex > -1)
                                {
                                    node.ImageIndex = folderElement.ImageIndex;
                                    node.SelectedImageIndex = folderElement.ImageIndex;
                                }
                                node.Tag = folderElement;
                                parentNode.Nodes.Add(node);
                                if ((folderElement as FolderElement).Children.Count > 0)
                                {
                                    AddNode(folderElement, node);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in AddNode " + ex.Message);
            }
        }

        private void AddNode(IPathElement parentFolder, WpfTreeNodeModel parentNode)
        {
            try
            {
                var element = parentFolder as FolderElement;

                if (element != null)
                {
                    foreach (IPathElement folderElement in element.Children.OrderBy(folder => folder.Name))
                    {
                        if (folderElement is FolderElement)
                        {
                            var path = folderElement.GetPathFromParent();
                            if (!CheckDirectoryExists || Directory.Exists(path))
                            {
                                var kategoria = folderElement as FolderElement;
                                WpfTreeNodeModel node = new WpfTreeNodeModel($"{folderElement.Name} [{kategoria.GetChildFoldersList()?.Count}][{kategoria.GetChildFilesList()?.Count}]");
                                node.Tag = folderElement;
                                parentNode.Nodes.Add(node);
                                if ((folderElement as FolderElement).Children.Count > 0)
                                {
                                    AddNode(folderElement, node);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in AddNode " + ex.Message);
            }
        }

        /// <summary>
        /// Metoda czyszcząca drzewo
        /// </summary>
        public void ClearTreeView()
        {
            if (_libraryTree == null)
            {
                throw new Exception("Error clearing TreeView, not set!");
            }

            _libraryTree.Nodes.Clear();
        }

        /// <summary>
        /// Zapisz strukturę do bazy danych
        /// </summary>
        /// <param name="folderElement">Korzeń drzewa</param>
        public void RefreshDataBase(FolderElement folderElement)
        {
            int lineNum = 0;
            try
            {
                using (FbKlient klient = new FbKlient())
                {
                    lineNum = 1;
                    #region Zapis danych do bazy
                    foreach (IPathElement element in folderElement.Children)
                    {
                        #region Pliki
                        if (element is FileElement)
                        {
                            klient.QueryId++;
                            klient.AddSQL("select id_pliki from de_plik(:id_katalogi, :nazwa, :sha)");
                            if (folderElement.Indeks > 0)
                            {
                                klient.ParamByName("id_katalogi", FbDbType.Integer).Value = folderElement.Indeks;
                            }
                            else
                            {
                                klient.SetNull("id_katalogi");
                            }

                            klient.ParamByName("nazwa", FbDbType.VarChar).Value = element.Name;
                            if (string.IsNullOrEmpty((element as FileElement).SHA256))
                            {
                                klient.SetNull("sha");
                            }
                            else
                            {
                                klient.ParamByName("sha", FbDbType.VarChar).Value = (element as FileElement).SHA256;
                            }
                        }
                        #endregion
                        #region Katalogi
                        else if (element is FolderElement)
                        {
                            klient.QueryId++;
                            klient.AddSQL("select id_katalogi from de_katalog(:id_katalogi, :nazwa)");
                            if (folderElement.Indeks > 0)
                            {
                                klient.ParamByName("id_katalogi", FbDbType.Integer).Value = folderElement.Indeks;
                            }
                            else
                            {
                                klient.SetNull("id_katalogi");
                            }

                            klient.ParamByName("nazwa", FbDbType.VarChar).Value = element.Name;
                        }
                        #endregion
                    }
                    #endregion
                    lineNum = 2;
                    klient.Execute();
                    lineNum = 3;
                    #region Odczyt danych z bazy
                    foreach (IPathElement element in folderElement.Children)
                    {
                        if (klient.Read())
                        {
                            if (element is FileElement)
                            {
                                element.Indeks = klient.GetInt32("id_pliki");
                            }
                            else if (element is FolderElement)
                            {
                                if (!klient.IsDBNull("id_katalogi"))
                                {
                                    element.Indeks = klient.GetInt32("id_katalogi");
                                }
                            }

                            if (folderElement.Children.Last() != element)
                            {
                                klient.ResponseId++;
                            }
                        }

                    }
                    klient.DataBaseClose();
                    #endregion
                    lineNum = 4;
                }

                lineNum = 5;
                #region Rekurencja dla podrzędnych
                var folders = from f in folderElement.Children where f is FolderElement select f;
                //Parallel.ForEach(folders, (folder) =>
                //{
                //    RefreshDataBase(folder as FolderElement);
                //}
                //);
                foreach (var pathElement in folders)
                {
                    var folder = (FolderElement) pathElement;
                    if (folder != null)
                    {
                        RefreshDataBase(folder);                            
                    }
                }

                #endregion
                lineNum = 6;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in RefreshDataBase({folderElement.Path}) LineNum={lineNum} {ex.Message} {ex.StackTrace}");
            }


        }

        /// <summary>
        /// Zapisz strukturę do bazy danych
        /// </summary>
        /// <param name="folderElement">Korzeń drzewa</param>
        public void RefreshDataBaseSlow(FolderElement folderElement)
        {
            int lineNum = 0;
            try
            { 
                lineNum = 1;
                #region Zapis danych do bazy
                foreach (IPathElement element in folderElement.Children)
                {
                    #region Pliki
                    if (element is FileElement)
                    {
                        using (FbKlient klient = new FbKlient())
                        {
                            klient.AddSQL("select id_pliki from de_plik(:id_katalogi, :nazwa, :sha)");
                            if (folderElement.Indeks > 0)
                            {
                                klient.ParamByName("id_katalogi", FbDbType.Integer).Value = folderElement.Indeks;
                            }
                            else
                            {
                                klient.SetNull("id_katalogi");
                            }

                            klient.ParamByName("nazwa", FbDbType.VarChar).Value = element.Name;
                            if (string.IsNullOrEmpty((element as FileElement).SHA256))
                            {
                                klient.SetNull("sha");
                            }
                            else
                            {
                                klient.ParamByName("sha", FbDbType.VarChar).Value = (element as FileElement).SHA256;
                            }

                            klient.Execute();
                            if (klient.Read())
                            {
                                element.Indeks = klient.GetInt32("id_pliki");
                            }
                        }                        
                    }
                    #endregion
                    #region Katalogi
                    else if (element is FolderElement)
                    {
                        using (FbKlient klient = new FbKlient())
                        {
                            klient.AddSQL("select id_katalogi from de_katalog(:id_katalogi, :nazwa)");
                            if (folderElement.Indeks > 0)
                            {
                                klient.ParamByName("id_katalogi", FbDbType.Integer).Value = folderElement.Indeks;
                            }
                            else
                            {
                                klient.SetNull("id_katalogi");
                            }

                            klient.ParamByName("nazwa", FbDbType.VarChar).Value = element.Name;
                            klient.Execute();
                            if (klient.Read())
                            {
                                element.Indeks = klient.GetInt32("id_katalogi");
                            }
                        }
                    }
                    #endregion
                }
                #endregion                                

                lineNum = 5;
                #region Rekurencja dla podrzędnych
                var folders = from f in folderElement.Children where f is FolderElement select f;
                Parallel.ForEach(folders, (folder) =>
                    {
                        RefreshDataBaseSlow(folder as FolderElement);
                    }
                );
                /*foreach (FolderElement folder in folders)
                    {
                        if (folder != null)
                        {
                            RefreshDataBaseSlow(folder as FolderElement);                        
                        }
                    }*/

                #endregion
                lineNum = 6;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in RefreshDataBaseSlow({folderElement.Path}) LineNum={lineNum} {ex.Message} {ex.StackTrace}");
            }


        }
    }
}
