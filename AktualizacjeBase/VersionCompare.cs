using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aktualizacje
{

    /// <summary>
    /// Komparator do obiektów klasy Wersja
    /// </summary>
    public static class VersionCompare
    {
        /// <summary>
        /// Metoda zwraca wartość logiczną PRAWDA, jezeli klasa w parametrze A jest mniejsza lub równa klasie w parametrze B
        /// </summary>
        /// <param name="versionA">Obiekt klasy Wersja</param>
        /// <param name="versionB">Obiekt klasy Wersja</param>
        /// <returns></returns>
        public static bool IsLessOrEqual(Wersja versionA, Wersja versionB)
        {
         
            bool returned_value = false;
            if (IsEqual(versionA, versionB))
                return true;

            byte i = 0;
            bool poprzedniaMniejsza = false;
            do
            {
                if (versionA.Elements[i] < versionB.Elements[i])
                {
                    returned_value = true;
                    poprzedniaMniejsza = true;

                }
                else if (versionA.Elements[i] == versionB.Elements[i])
                {
                    returned_value = true;
                    poprzedniaMniejsza = false;
                }
                else
                {
                    returned_value = poprzedniaMniejsza;
                    break;
                }
                i++;
            }
            while (i < 4);

            

            return returned_value;
        }

        public static bool IsEqual(Wersja versionA, Wersja versionB)
        {
            return versionA.Number == versionB.Number;
        }
    }
}
