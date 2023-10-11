using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Services;
using MojeFunkcjeUniwersalneNameSpace;
using MojeFunkcjeUniwersalneNameSpace.Logger;
using System;
using System.Threading.Tasks;

namespace FirebirdBackup
{
    /// <summary>
    /// Klasa metod wykonujących backup oraz odtworzenie bazy danych
    /// </summary>
    public class FirebirdBackupService
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public FirebirdBackupService()
        {
            Verbose = true;
        }

        /// <summary>
        /// Czy backup/restore gadatliwy?
        /// </summary>
        public bool Verbose;
        /// <summary>
        /// Czy tworzenie kopii ma być wykonane asynchronicznie? Domyślnie włączone
        /// </summary>
        public bool AsyncMode = true;

        /// <summary>
        /// Metoda tworząca kopię bazy danych
        /// </summary>
        /// <param name="connectionString">Parametry połączenia do bazy danych</param>
        /// <param name="backupFile">Ścieżka do pliku kopii</param>
        /// <param name="throwErrors">Ustala, czy zwracać komunikaty błędów</param>
        [STAThread]
        public void CreateBackup(FbConnectionStringBuilder connectionString, string backupFile, bool throwErrors=false)
        {

            if (AsyncMode)
            {
                var t = Task.Factory.StartNew(() =>
                    {
                        CrateBackupMethod(connectionString, backupFile, throwErrors);
                    }
                );
            }
            else
                CrateBackupMethod(connectionString, backupFile, throwErrors);
        }
        /// <summary>
        /// Metoda prywatna tworząca kopię bazy danych, uruchamiana synchronicznie lub asynchronicznie
        /// </summary>
        /// <param name="connectionString">Parametry połączenia do bazy danych</param>
        /// <param name="backupFile">Ścieżka do pliku kopii</param>
        /// <param name="throwErrors">Ustala, czy zwracać komunikaty błędów</param>
        private void CrateBackupMethod(FbConnectionStringBuilder connectionString, string backupFile, bool throwErrors)
        {
            Logger.Instance.Loguj("Połączenie do bazy danych: " + connectionString.ToString());
            //Invoke(new LogujDelegate(FunkcjeUniwersalne.Instance.Loguj), "Rozpoczynam tworzenie kopii zapasowej bazy: " + k.Nazwa);
            try
            {
                FbBackup backupSvc = new FbBackup();

                backupSvc.ConnectionString = connectionString.ToString();
                FileServiceClass fileTools = new FileServiceClass();

                string newBackupFile = fileTools.GetFileNameWTimeStamp(backupFile);
                backupSvc.BackupFiles.Add(new FbBackupFile(newBackupFile, 2048));
                backupSvc.Verbose = Verbose;

                backupSvc.Options = FbBackupFlags.IgnoreLimbo;

                //backupSvc.ServiceOutput += new ServiceOutputEventHandler(ServiceOutput);

                backupSvc.Execute();
                Logger.Instance.Loguj($"Zakończono tworzenie kopii bazy {connectionString.Database} w pliku {backupFile}");
            }
            catch (Exception ex)
            {
                string myError = "Wystąpił błąd podczas tworzenia kopii: " + ex.Message;
                Logger.Instance.Loguj(myError);
                if (throwErrors)
                    throw new Exception(myError);
            }
        }


        /// <summary>
        /// Metoda odtwarzająca bazę danych z kopii
        /// </summary>
        /// <param name="connectionString">Parametry połączenia do bazy danych</param>
        /// <param name="backupFile">Ścieżka do pliku kopii</param>
        [STAThread]
        public void RestoreDataBase(FbConnectionStringBuilder connectionString, string backupFile)
        {
            //FbConnectionStringBuilder cs = new FbConnectionStringBuilder();

            //cs.UserID = "SYSDBA";
            //cs.Password = "masterkey";
            //cs.Database = "nunit_testdb";

            FbRestore restoreSvc = new FbRestore();

            restoreSvc.ConnectionString = connectionString.ToString();
            restoreSvc.BackupFiles.Add(new FbBackupFile(backupFile, 2048));
            restoreSvc.Verbose = Verbose;
            restoreSvc.PageSize = 4096;
            restoreSvc.Options = FbRestoreFlags.Create | FbRestoreFlags.Replace;

            //restoreSvc.ServiceOutput += new ServiceOutputEventHandler(ServiceOutput);

            restoreSvc.Execute();
        }

        /// <summary>
        /// Metoda wywoływana podczas wykonywania operacji backupu oraz przywrócenia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void ServiceOutput(object sender, ServiceOutputEventArgs e)
        {
            Logger.Instance.Loguj("BackupServiceOutput: " + e.Message);
        }

    }
}
