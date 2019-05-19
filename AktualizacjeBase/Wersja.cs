using System;

namespace Aktualizacje
{
    /// <summary>
    /// Klasa odpowiadająca obiektowi wersji aplikacji, dll 
    /// </summary>
    public class Wersja
    {
        public int A;
        public int B;
        public int C;
        public int D;

        public int[] Elements = new int[4];
        public string Number
        {
            set
            {
                try
                {
                    string[] elements = value.Split(new char[] { '.' }, 4);
                    for (byte i = 0; i < 4; i++)
                        Elements[i] = Convert.ToInt32(elements[i]);
                    A = Convert.ToInt32(Elements[0]);
                    B = Convert.ToInt32(Elements[1]);
                    C = Convert.ToInt32(Elements[2]);
                    D = Convert.ToInt32(Elements[3]);
                    
                }
                catch
                {
                    throw new Exception($"Cannot convert from string {value} to class Wersja!");
                }
            }
            get
            {
                return $"{A}.{B}.{C}.{D}";
            }
        }

    }
}
