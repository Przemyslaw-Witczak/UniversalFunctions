#define DotNet45
using MojeFunkcjeUniwersalneNameSpace.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;


namespace MojeFunkcjeUniwersalneNameSpace
{
    /// <summary>
    /// Klasa z funkcjami przydatnymi do wykonywania operacji na plikach
    /// </summary>
    public class FileServiceClass
    {

        #region Metody prywatne
        /// <summary>
        /// Metoda rekurencyjna budująca listę ścieżek podrzędnych, rozpoczynając z podanego katalogu
        /// </summary>
        /// <param name="sDir">Katalog początkowy</param>
        /// <param name="extensionsFilter">Rozszerzenia plików - chyba nieobsłużone</param>
        /// <param name="filesList">Lista wynikowa</param>
        private void DirSearch(string sDir, string filter, List<string> filesList)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {

                        filesList.Add(f);
                    }
                    DirSearch(d, filter, filesList);
                }
                foreach (string f in Directory.GetFiles(sDir))
                {

                    filesList.Add(f);
                }               
            }
            catch (System.Exception excpt)
            {
                throw new Exception("Error in DirSearch: " + excpt.Message);
            }
        }

        private void DirSearch(FolderElement dirRoot, string[] extensionsFilter)
        {

            try
            {
#if DotNet45      
                GetFilesFromDirectoryParallel(dirRoot, extensionsFilter);
                Parallel.ForEach(Directory.GetDirectories(dirRoot.Path), (d) =>
                //foreach (var d in Directory.GetDirectories(dirRoot.Path))
                {
                    FolderElement child = new FolderElement() { Name = Path.GetFileName(d), Path = d, ParentFolder = dirRoot };
                    dirRoot.Add(child);
                    DirSearch(child, extensionsFilter);                   
                });
#else
                GetFilesFromDirectory(dirRoot, extensionsFilter);
                foreach (string d in Directory.GetDirectories(dirRoot.Path))
                {
                    FolderElement child = new FolderElement() { Name = Path.GetFileName(d), Path = d };
                    dirRoot.Add(child);                                     
                    DirSearch(child, extensionsFilter);
                }
#endif
            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (System.Exception excpt)
            {
                throw new Exception("Error in DirSearch: " + excpt.Message);
            }
        }

        /// <summary>
        /// Funkcja uzupełnia listę plików spełniających kryterium nazwy - paralelnie
        /// </summary>
        /// <param name="dirRoot"></param>
        /// <param name="extensionsFilter">Filtr zawierający fragmenty nazw plików do wyszukania</param>
        /// <param name="d"></param>        
        private static void GetFilesFromDirectoryParallel(FolderElement dirRoot, string[] extensionsFilter)
        {
            Parallel.ForEach(Directory.GetFiles(dirRoot.Path), (f) =>
            {
                if (extensionsFilter == null)
                    dirRoot.Add(new FileElement() { Name = Path.GetFileName(f), Path = dirRoot.Path, ParentFolder = dirRoot });
                else
                {
                    foreach (string extension in extensionsFilter)
                    {
                        if (f.Trim().ToUpper().Contains(extension.Trim().ToUpper()))
                            dirRoot.Add(new FileElement() { Name = Path.GetFileName(f), Path = dirRoot.Path, ParentFolder = dirRoot });
                    }
                }
            });
        }

        /// <summary>
        /// Funkcja uzupełnia listę plików spełniających kryterium nazwy
        /// </summary>
        /// <param name="dirRoot"></param>
        /// <param name="extensionsFilter">Filtr zawierający fragmenty nazw plików do wyszukania</param>
        /// <param name="d"></param>        
        private static void GetFilesFromDirectory(FolderElement dirRoot, string[] extensionsFilter)
        {
            //Parallel.ForEach(Directory.GetFiles(dirRoot.Path), (f) =>
            foreach(var f in Directory.GetFiles(dirRoot.Path))
            {
                if (extensionsFilter == null)
                    dirRoot.Add(new FileElement() { Name = Path.GetFileName(f), Path = dirRoot.Path, ParentFolder = dirRoot });
                else
                {
                    foreach (string extension in extensionsFilter)
                    {
                        if (f.Trim().ToUpper().Contains(extension.Trim().ToUpper()))
                            dirRoot.Add(new FileElement() { Name = Path.GetFileName(f), Path = dirRoot.Path, ParentFolder = dirRoot });
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Metoda tworząca listę plików/katalogów rozpoczynając z podanego
        /// </summary>
        /// <param name="dir">Katalog początkowy</param>
        /// <returns>Lista ścieżek</returns>
        public List<string> GetFilesList(string dir, string filter = "")
        {
            List<string> filesList = new List<string>();

            DirSearch(dir, filter, filesList);

            return filesList;
        }

        /// <summary>
        /// Metoda zwraca podaną ścieżkę, ale do nazwy pliku dodje przedrostek z pieczątką czasową
        /// </summary>
        /// <param name="fileName">Ścieżka do pliku</param>
        /// <returns>Zmodyfikowana ścieżka do pliku ze zmienioną nazwą pliku</returns>
        public string GetFileNameWTimeStamp(string fileName)
        {
            string returned_value;

            //Path.GetFileNameWithoutExtension(zalacznik.NazwaPliku);

            returned_value = Path.Combine(Path.GetDirectoryName(fileName), string.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "_" + Path.GetFileName(fileName));

            return returned_value;
        }

        /// <summary>
        /// Struktura katalogów
        /// </summary>
        public volatile FolderElement DirRoot;

        /// <summary>
        /// Metoda wypełnia strukturę DirRoot, rozpoczynając z podanego katalogu, dla podanych filtrów
        /// </summary>
        /// <param name="rootElement">Katalog początkowy</param>
        /// <param name="filter">Filtry wyszukiwania</param>
        public void GetDirTree(string rootElement, string[] filter)
        {
            try
            {
                DirRoot = new FolderElement() { Name = rootElement, Path = rootElement };

                DirSearch(DirRoot, filter);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        /// <summary>
        /// Metoda zwraca listę elementów typu FileEmenet
        /// </summary>
        /// <param name="dirRoot">Katalog z którego rozpocząć</param>
        /// <returns>Lista plików</returns>
        public List<FileElement> GetFilesElementList(FolderElement dirRoot)
        {
            List<FileElement> filesList = new List<FileElement>();
            
            foreach (IPathElement element in dirRoot.Children)
            {
                if (element is FileElement)
                    filesList.Add(element as FileElement);
                else
                    filesList.AddRange(GetFilesElementList(element as FolderElement));
            }

            return filesList;
        }

        /// <summary>
        /// Funkcja usuwa katalogi z zawartośćią rozpoczynając z podanego
        /// </summary>
        /// <param name="pathToDelete">Katalog którego zawartość zostanie usunięta</param>        
        public void DelTree(string pathToDelete)
        {
            try
            {
                foreach (string file in Directory.GetFiles(pathToDelete))
                    File.Delete(file);

                foreach (string directory in Directory.GetDirectories(pathToDelete))
                    Directory.Delete(directory, true);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DelTree: " + ex.Message);
            }
        }

        private const string TEMP_FILE = "tempFile.tmp";

        /// <summary>
        /// Checks the ability to create and write to a file in the supplied directory.
        /// </summary>
        /// <param name="directory">String representing the directory path to check.</param>
        /// <returns>True if successful; otherwise false.</returns>
        public bool CheckDirectoryAccess(string directory)
        {
            bool success = false;
            string fullPath = Path.Combine(directory, TEMP_FILE);

            if (Directory.Exists(directory))
            {
                try
                {
                    using (FileStream fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write))
                    {
                        fs.WriteByte(0xff);
                    }

                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        success = true;
                    }
                }
                catch (Exception)
                {
                    success = false;
                }
            }
            return success;
        }

        /// <summary>
        /// Metoda zmienia nazwę podanego pliku
        /// </summary>
        /// <param name="oldFileName">Kompletna ścieżka do pliku którego nazwa ma być zmieniona</param>
        /// <param name="newFileName">Nowa nazwa pliku bez ścieżki!!</param>
        public void RenameFile(string oldFileName, string newFileName)
        {
            try
            {
                FileInfo file = new FileInfo(oldFileName);
                string newPath = Path.Combine(file.Directory.FullName, newFileName);
                File.Delete(newPath);
                file.MoveTo(newFileName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while ranaming file '{oldFileName}' to '{newFileName}' error message: {ex.Message}");
            }
        }

        /// <summary>
        /// Metoda zabezpiecza nazwę pliku przed niedozwolonymi znakami w Windows: \/:*?"<>| oraz %
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReplaceProhibitedCharsFrom(string fileName)
        {
            return fileName.Replace("/", "_").Replace("\\", "_").Replace("%", "_").Replace(":", "_").Replace("*", "_").Replace("?", "_").Replace("\"", "_").Replace("<", "_").Replace(">", "_").Replace("|", "_");
        }
    }
}
