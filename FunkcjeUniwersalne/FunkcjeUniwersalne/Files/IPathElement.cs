namespace MojeFunkcjeUniwersalneNameSpace.Files
{
    public interface IPathElement
    {
        int Indeks { get; set; }

        /// <summary>
        /// Nazwa pliku lub folderu
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Kompletna sciezka do pliku
        /// </summary>
        string Path { get; set; }
        
        int IdKatalogiParent { get; set; }
        IPathElement ParentFolder { get; set; }

        string GetPathFromParent();

        /// <summary>
        /// IdIkony jeśli jest ImageListPodpiety
        /// </summary>
        int ImageIndex { get; set; }
    }
}
