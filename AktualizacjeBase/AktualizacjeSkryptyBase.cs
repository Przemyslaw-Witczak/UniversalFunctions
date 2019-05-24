using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aktualizacje
{
    /// <summary>
    /// Klasa bazowa aktualizatora, zawiera listę wszystkich skryptów
    /// </summary>
    public abstract class AktualizacjeSkryptyBase
    {
        /// <summary>
        /// Lista wszystkich skryptów i wersji
        /// </summary>
        protected internal readonly List<AktualizacjaSkrypt> aktualizacje;

        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        public AktualizacjeSkryptyBase()
        {
            aktualizacje = new List<AktualizacjaSkrypt>();

            InicjalizacjaAktualizacji();
        }

        /// <summary>
        /// Metoda inicjalizująca listę
        /// </summary>
        public abstract void InicjalizacjaAktualizacji();
        //{
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1030, Skrypt = WersjeSkrypty.Skrypt1030 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1040, Skrypt = WersjeSkrypty.Skrypt1040 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1041, Skrypt = WersjeSkrypty.Skrypt1041 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1060, Skrypt = WersjeSkrypty.Skrypt1060 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1061, Skrypt = WersjeSkrypty.Skrypt1061 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1062, Skrypt = WersjeSkrypty.Skrypt1062 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1063, Skrypt = WersjeSkrypty.Skrypt1063 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1073, Skrypt = WersjeSkrypty.Skrypt1073 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1081, Skrypt = WersjeSkrypty.Skrypt1081 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1090, Skrypt = WersjeSkrypty.Skrypt1090 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1091, Skrypt = WersjeSkrypty.Skrypt1091 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1092, Skrypt = WersjeSkrypty.Skrypt1092 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1093, Skrypt = WersjeSkrypty.Skrypt1093 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1094, Skrypt = WersjeSkrypty.Skrypt1094 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1095, Skrypt = WersjeSkrypty.Skrypt1095 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1096, Skrypt = WersjeSkrypty.Skrypt1096 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1097, Skrypt = WersjeSkrypty.Skrypt1097 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja1099, Skrypt = WersjeSkrypty.Skrypt1099 });
        //    aktualizacje.Add(new AktualizacjaSkrypt() { Wersja = WersjeSkrypty.Wersja10100, Skrypt = WersjeSkrypty.Skrypt10100 });
        //}

        /// <summary>
        /// Metoda zwraca listę skryptów które mają wyższą lub równą wersję bazy danych od której można je instalować
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public List<string> GetSkryptyFromVersion(string version)
        {
            List<string> returned_values = new List<string>();
            bool eksportuj = false;
            for(int i=0;i<aktualizacje.Count;i++)
            {
                Wersja wersjaBazyDanych = new Wersja() { Number = version };
                Wersja wersjaAktualizacji = new Wersja() { Number = aktualizacje[i].Wersja };
                if (eksportuj || (!VersionCompare.IsEqual(wersjaBazyDanych, wersjaAktualizacji) && VersionCompare.IsLessOrEqual(wersjaBazyDanych, wersjaAktualizacji)))                
                {
                    returned_values.Add(aktualizacje[i].Skrypt);
                    eksportuj = true;
                }
            }            
            return returned_values;
        }
    }
}
