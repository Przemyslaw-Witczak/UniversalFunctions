using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace MojeFunkcjeUniwersalneNameSpace

{
    public sealed class FunkcjeUniwersalne
    {
        private static volatile FunkcjeUniwersalne instance;
        private static object syncRoot = new object();
        private readonly DataProtectionScope scope = DataProtectionScope.CurrentUser;

        private FunkcjeUniwersalne()
        {

        }
        #region Konstruktor i destruktor

        public static FunkcjeUniwersalne Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new FunkcjeUniwersalne();
                        }
                    }
                }

                return instance;
            }
        }
        #endregion

        #region Eksport
        private StringBuilder DataGridtoHTML(DataGridView dg)
        {
            StringBuilder strB = new StringBuilder();
            //create html & table
            strB.AppendLine("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /></head><body><center><" +
                          "table border='1' cellpadding='5' cellspacing='1'>");
            strB.AppendLine("<tr>");
            //cteate table header
            for (int i = 0; i < dg.Columns.Count; i++)
            {
                if (dg.Columns[i].Visible)
                {
                    string str = string.Format("<td align='center' valign='middle' width={0}>" +
                               dg.Columns[i].HeaderText + "</td>", dg.Columns[i].Width.ToString());

                    strB.AppendLine(str);
                }
            }
            //create table body
            strB.AppendLine("<tr>");
            for (int i = 0; i < dg.Rows.Count; i++)
            {
                strB.AppendLine("<tr>");
                //foreach (DataGridViewCell dgvc in dg.Rows[i].Cells)
                for (int j = 0; j < dg.Columns.Count; j++)
                {
                    if (dg.Columns[j].Visible)
                    {
                        string str = string.Format("<td align='left' valign='middle' width={0}>" +
                                        dg.Rows[i].Cells[j].Value.ToString() + "</td>", dg.Columns[j].Width.ToString());

                        strB.AppendLine(str);
                    }
                }
                strB.AppendLine("</tr>");

            }
            //table footer & end of html file
            strB.AppendLine("</table></center></body></html>");
            return strB;
        }
        public void ExportDataGridToWebBrowser(DataGridView DG)
        {
            //DataGridtoHTML(DG);
            string PathToTmpFile = Path.GetTempFileName() + ".html";

            try
            {
                using (StreamWriter file = new StreamWriter(PathToTmpFile/*, false, Encoding.ASCII*/))
                {
                    file.Write(DataGridtoHTML(DG));
                }
                System.Diagnostics.Process.Start(PathToTmpFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eksport się nie powiódł: {ex}");
            }
        }
        private StringBuilder DataGridtoCSV(DataGridView dg)
        {

            StringBuilder strB = new StringBuilder();

            //cteate table header
            string line = "";
            for (int i = 0; i < dg.Columns.Count; i++)
            {
                if (dg.Columns[i].Visible)
                {
                    line += quotString(dg.Columns[i].HeaderText.Trim()).Trim() + ";";
                }
            }
            strB.AppendLine(line);

            for (int i = 0; i < dg.Rows.Count; i++)
            {
                line = "";
                for (int j = 0; j < dg.Columns.Count; j++)
                {
                    if (dg.Columns[j].Visible)
                    {
                        line += quotString(dg.Rows[i].Cells[j].Value?.ToString().Trim())?.Trim() + ";";
                    }
                }
                strB.AppendLine(line);

            }
            return strB;

        }

        /// <summary>
        /// Metoda sprawdza czy string zawiera śrenik, jeśli tak opakowuje go w cudzysłowia
        /// </summary>
        /// <param name="inputString">Łańcuch znaków wejściowy</param>
        /// <returns>Wyjściowy opakowany łańcuch znaków</returns>
        private string quotString(string inputString)
        {
            if (!string.IsNullOrEmpty(inputString))
            {
                inputString = System.Text.RegularExpressions.Regex.Replace(inputString, @"\r\n?|\n", "|");
            }

            if (!string.IsNullOrEmpty(inputString) && inputString.Contains(";"))
            {
                return $"\"{inputString}\"";
            }
            else
            {
                return inputString;
            }
        }

        public void ExportDataGridToCSV(DataGridView DG)
        {
            try
            {
                using (SaveFileDialog SaveDialog = new SaveFileDialog())
                {
                    SaveDialog.Filter = "Wszystkie pliki (*.*)|*.*|Plik *.csv|*.csv";
                    SaveDialog.FilterIndex = 2;
                    SaveDialog.RestoreDirectory = true;
                    SaveDialog.FileName = "";
                    if (SaveDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter file = new StreamWriter(SaveDialog.FileName, false, Encoding.Default))
                        {
                            file.Write(DataGridtoCSV(DG));
                        }
                    }
                    MessageBox.Show("Zakończono eksport do pliku");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eksport się nie powiódł: {ex}");
            }


            //System.Diagnostics.Process.Start(PathToTmpFile);
        }

        #endregion

        #region Import
        public string[][] ImportDataFromCSV()
        {
            using (OpenFileDialog OpenDialog = new OpenFileDialog())
            {
                OpenDialog.Filter = "Wszystkie pliki (*.*)|*.*|Plik *.csv|*.csv";
                OpenDialog.FilterIndex = 2;
                OpenDialog.RestoreDirectory = true;
                OpenDialog.FileName = "";
                if (OpenDialog.ShowDialog() == DialogResult.OK)
                {
                    return ImportDataFromCSV(OpenDialog.FileName);
                }
            }
            return null;

        }

        public string[][] ImportDataFromCSV(string fileName)
        {
            List<string[]> rows = new List<string[]>();
            using (StreamReader reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string[] row = reader.ReadLine().Split(';');
                    rows.Add(row);
                }
            }
            return rows.ToArray();

        }

        public string UnQuote(string quoted_string)
        {
            return quoted_string.Replace("\"", "").Replace("'", "");
        }

        #endregion

        #region ObslugaKomponentow
        public void ZaznaczOdznaczWszystko(CheckedListBox CheckList, bool Checked)
        {
            for (int i = 0; i < CheckList.Items.Count; i++)
            {
                CheckList.SetItemChecked(i, Checked);
            }
        }

        public void PoprawLP(DataGridView dgv, int ACol)
        {
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                dgv.Rows[i].Cells[ACol].Value = (i + 1).ToString();

            }
        }
        #endregion

        #region Konwersje
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public string GetString(byte[] bytes)
        {
            int tableSize = bytes.Length / sizeof(char);
            char[] chars = new char[tableSize];
            Buffer.BlockCopy(bytes, 0, chars, 0, tableSize);
            return new string(chars);
        }

        public string ToHex(byte[] buff, int off, int len)
        {
            StringBuilder sb = new StringBuilder(buff.Length * 3);
            sb.Append(buff[off].ToString("X2"));
            for (int i = 1; i < len; i++)
            {
                sb.Append(" ");
                sb.Append(buff[off + i].ToString("X2"));
            }
            return sb.ToString();
        }

        public byte[] FromHex(string s)
        {
            s = System.Text.RegularExpressions.Regex.Replace(s.ToUpper(), "[^0-9A-F]", "");
            byte[] b = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                b[i / 2] = byte.Parse(s.Substring(i, 2),
                   System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return b;
        }

        //Funkcja konwertujaca cyfry arabskie z zakresu 0-4000 na rzymskie
        public string Arab2Rome(int LiczbaA)
        {
            string LiczbaR = "";
            //tablica która określa liczby arabskie są pzekładalne na liczby rzymskie
            int[] TabA = new int[] { 1000, 500, 100, 50, 10, 5, 1 };
            //tablica z liczbami które można odjąć od danych z tablicy TabA
            //np 1000 to M a 900 to CM czyli 1000 - 100, tak samo jak 400 to CD czyli 500 - 100
            //tażdej liczbie poza 1 z tablicy TabA odpowiada jedna liczba z tablicy TabAP
            int[] TabAP = new int[] { 100, 100, 10, 10, 1, 1 };
            //liczby w zapisie rzymskim odpowiadające liczbom z tablicy TabA
            string[] TabR = new string[] { "M", "D", "C", "L", "X", "V", "I" };
            //tak jak wyżej tyle że liczbom z tablicy TabAP
            string[] TabRP = new string[] { "C", "C", "X", "X", "I", "I" };
            int i = 0;      //określa obecną pozycję w tablicach TabA, TabAP, TabR, TabRP

            while (LiczbaA > 0)
            {
                if (LiczbaA >= TabA[i])
                {
                    LiczbaA -= TabA[i];
                    LiczbaR += TabR[i];
                }
                else if (LiczbaA >= TabA[i] - TabAP[i])
                {
                    LiczbaA -= TabA[i] - TabAP[i];
                    LiczbaR += TabRP[i];
                    LiczbaR += TabR[i];
                }
                else
                {
                    i++;
                }
            }
            return LiczbaR;
        }

        /// <summary>
        /// Metoda konwertuje łańcuch znaków do wartości decimal
        /// </summary>
        /// <param name="currentString"></param>
        /// <returns></returns>
        public decimal FormatujStringNaDecimal(string currentString)
        {
            string outputString = string.Empty;
            if (string.IsNullOrEmpty(currentString))
            {
                currentString = "0";
            }

            char[] workString = currentString.ToCharArray(0, currentString.Length);
            int charCode;

            for (int i = 0; i < currentString.Length; i++)
            {
                charCode = workString[i];

                if (charCode >= 48 && charCode <= 57 /*0 - 9*/
                    //|| (Kod >= 43 && Kod <= 45) /* + , -*/
                    || charCode == 43 || charCode == 45
                    //|| (*WorkString.c_str())==ThousandSeparator //bez tego, bo problem przy konwersji na currency
                    || charCode == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray(0, 1)[0]
                )
                {
                    outputString += workString[i];
                }
            }

            if (string.IsNullOrEmpty(outputString))
            {
                outputString = "0";
            }

            try
            {
                Convert.ToDecimal(outputString);
            }
            catch (Exception)
            {
                outputString = "0";
            }

            return Convert.ToDecimal(outputString);
        }

        public string FormatujDecimalSeparator(string currentString)
        {
            currentString = currentString.Replace(',', System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray(0, 1)[0]);
            currentString = currentString.Replace('.', System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray(0, 1)[0]);

            return currentString;
        }

        #endregion

        #region FunkcjeSkrotu

        #region Metody formatujące
        /// <summary>
        /// Funkcja konwertująca tabelę bajtów z wynikiem funkcji hash, do sformatowanego łańcucha string
        /// </summary>
        /// <param name="data">Tabela bajtów z wynikiem funkcji hash</param>
        /// <returns>Sformatowany łańcuch string</returns>
        private static string byteArrayToString(byte[] data)
        {
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        #endregion

        #region MD5
        /// <summary>
        /// Funkcja zwracająca sumę MD5 dla podanego łańcucha znaków
        /// </summary>
        /// <param name="input">Łańcuch wejściowy</param>
        /// <returns>Wynik funkcji hash, przekonwertowany do typu string</returns>
        public string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                return byteArrayToString(data);
            }
        }



        /// <summary>
        /// Funkcja weryfikująca czy podany hash prawidłowy jest dla podanego łańcucha
        /// </summary>
        /// <param name="input"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool VerifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region SHA256
        /// <summary>
        /// Funkcja zwaca funkcję SHA256 z podanego strumienia
        /// </summary>
        /// <param name="fromStream">Strumień wejściowy</param>
        /// <returns>Sformatowana wartość funkcji hash</returns>
        public string GetSha256Hash(Stream fromStream)
        {
            try
            {
                SHA256 mySHA256 = SHA256.Create();
                byte[] hashValue;
                fromStream.Position = 0;
                hashValue = mySHA256.ComputeHash(fromStream);

                return byteArrayToString(hashValue);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetSha256Hash: " + ex.Message);
            }

        }


        /// <summary>
        /// Funkcja weryfikująca czy podany hash prawidłowy jest dla podanego strumienia
        /// </summary>
        /// <param name="inputStream">Strumień wejściowy</param>
        /// <param name="inputHash">Porównywany hash</param>
        /// <returns>Wynik porównania</returns>
        public bool VerifySHA256Hash(Stream inputStream, string inputHash)
        {
            // Hash the input.
            string hashOfInput = GetSha256Hash(inputStream);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, inputHash))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion
        #endregion

        /// <summary>
        /// Metoda zwraca wartość INT z poprzedzającymi zerami, w celu uzyskania stałej długości
        /// </summary>
        /// <param name="counter">numer kolejny</param>
        /// <param name="length">Wartość maksymalna</param>
        /// <returns></returns>
        public string FormatIntCounter(int counter, int MaxValue)
        {
            string returned_string = counter.ToString();
            int length = MaxValue.ToString().Length;

            while (returned_string.Length < length)
            {
                returned_string = "0" + returned_string;
            }

            return returned_string;


        }

        #region Encode-Decode
        /// <summary>
        /// Encrypts a given password and returns the encrypted data
        /// as a base64 string.
        /// </summary>
        /// <param name="plainText">An unencrypted string that needs
        /// to be secured.</param>
        /// <returns>A base64 encoded string that represents the encrypted
        /// binary data.
        /// </returns>
        /// <remarks>This solution is not really secure as we are
        /// keeping strings in memory. If runtime protection is essential,
        /// <see cref="SecureString"/> should be used.</remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="plainText"/>
        /// is a null reference.</exception>
        public string Encrypt(string plainText)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException("plainText");
            }

            //encrypt data
            var data = Encoding.Unicode.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, null, scope);

            //return as base64 string
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypts a given string.
        /// </summary>
        /// <param name="cipher">A base64 encoded string that was created
        /// through the <see cref="Encrypt(string)"/> or
        /// <see cref="Encrypt(SecureString)"/> extension methods.</param>
        /// <returns>The decrypted string.</returns>
        /// <remarks>Keep in mind that the decrypted string remains in memory
        /// and makes your application vulnerable per se. If runtime protection
        /// is essential, <see cref="SecureString"/> should be used.</remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="cipher"/>
        /// is a null reference.</exception>
        public string Decrypt(string cipher)
        {
            if (cipher == null)
            {
                throw new ArgumentNullException("cipher");
            }

            //parse base64 string
            byte[] data = Convert.FromBase64String(cipher);

            //decrypt data
            byte[] decrypted = ProtectedData.Unprotect(data, null, scope);
            return Encoding.Unicode.GetString(decrypted);
        }
        #endregion

        #region
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        /// remarks https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            if (image == null)
            {
                throw new Exception("No picture to resize!!");
            }
            // Figure out the ratio
            double ratioX = width / (double)image.Width;
            double ratioY = height / (double)image.Height;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            height = Convert.ToInt32(image.Height * ratio);
            width = Convert.ToInt32(image.Width * ratio);


            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        #endregion

    }
}
