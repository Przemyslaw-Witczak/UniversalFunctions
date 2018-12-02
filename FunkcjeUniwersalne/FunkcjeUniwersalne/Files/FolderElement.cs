using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MojeFunkcjeUniwersalneNameSpace.Files
{
    public class FolderElement : IPathElement
    {
        private volatile string name;
        /// <summary>
        /// Nazwa folderu
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        private volatile IPathElement parentFolder;
        public IPathElement ParentFolder
        {
            get
            {
                return parentFolder;
            }

            set
            {
                parentFolder = value;
            }
        }

        private volatile string path;
        public string Path
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
            }
        }

        private volatile int idKatalogiParent;
        public int IdKatalogiParent
        {
            get
            {
                return idKatalogiParent;
            }

            set
            {
                idKatalogiParent = value;
            }
        }

        private volatile int indeks;
        public int Indeks
        {
            get
            {
                return indeks;
            }

            set
            {
                indeks = value;
                foreach (IPathElement element in Children)
                    element.IdKatalogiParent = value;
            }
        }

        public volatile ConcurrentQueue<IPathElement> Children;

        public FolderElement()
        {
            Children = new ConcurrentQueue<IPathElement>();
            ParentFolder = null;
            IdKatalogiParent = 0;
        }

        public void Add(IPathElement element)
        {
            Children.Enqueue(element);
        }

        /// <summary>
        /// Zwraca listę plików podrzędnych
        /// </summary>
        /// <returns></returns>
        public List<FileElement> GetChildFilesList()
        {
            List<FileElement> filesList = new List<FileElement>();
            try
            {
                if (Children != null)
                foreach (IPathElement element in Children)
                {
                    if (element is FileElement)
                        filesList.Add(element as FileElement);
                    else
                        filesList.AddRange((element as FolderElement).GetChildFilesList());
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"Error in GetChildFilesList({this.Name} {ex})");
            }
            return filesList;
        }

        /// <summary>
        /// Zwraca listę katalogów podrzędnych
        /// </summary>
        /// <returns></returns>
        public List<FolderElement> GetChildFoldersList()
        {
            List<FolderElement> foldersList = new List<FolderElement>();            
            try
            {
                if (Children!=null)
                foreach (IPathElement element in Children)
                {
                        if (element is FolderElement)
                        {
                            foldersList.Add(element as FolderElement);
                            foldersList.AddRange((element as FolderElement).GetChildFoldersList());
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetChildFoldersList({this.Name} {ex})");
            }
            return foldersList;
        }

        /// <summary>
        /// Metoda zwraca ścieżkę do pliku, na podstawie rodzica
        /// </summary>
        /// <returns>Ścieżka</returns>
        public string GetPathFromParent()
        {
            string returned_path = Name;

            if (ParentFolder!=null)
                returned_path = System.IO.Path.Combine(ParentFolder.GetPathFromParent(), returned_path);

            return returned_path;
        }
    }
}
