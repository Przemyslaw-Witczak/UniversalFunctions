namespace iTextSharpHelper
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Metoda konwertuje standardowy obiekt System.Drawing.Image do obiektu iTextSharp.text.Image
        /// </summary>
        /// <param name="image">Obraz źródłowy</param>
        /// <returns></returns>
        public static iTextSharp.text.Image ConvertToiTextSharpImage(this System.Drawing.Image image)
        {
            if (image == null)
                return null;
            return iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
