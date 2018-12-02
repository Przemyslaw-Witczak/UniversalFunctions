using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MojeFunkcjeUniwersalneNameSpace.Files
{
    public class FileElement : IPathElement
    {
        private volatile string name;
        /// <summary>
        /// Nazwa pliku
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
        /// <summary>
        /// Kompletna sciezka do pliku
        /// </summary>
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

        public volatile string SHA256;

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
        public int Indeks2;

        public int Indeks
        {
            get
            {
                return indeks;
            }

            set
            {
                indeks = value;
            }
        }

        public FileElement()
        {
            Path = string.Empty;
            SHA256 = string.Empty;
        }

        /// <summary>
        /// Metoda zwraca ścieżkę do pliku, na podstawie rodzica
        /// </summary>
        /// <returns>Ścieżka</returns>
        public string GetPathFromParent()
        {
            string returned_path = string.Empty;

            returned_path = System.IO.Path.Combine(ParentFolder.GetPathFromParent(), Name);
            return returned_path;
        }
    }
}
