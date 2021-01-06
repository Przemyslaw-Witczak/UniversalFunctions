using MojeFunkcjeUniwersalneNameSpace.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerMultithreadingTests
{
    class Program
    {
        static void Main(string[] args)
        {

            int maxTask = 5;
            if (args.Length > 0)
                maxTask = Convert.ToInt32(args[0]);
            DateTime dgStart = DateTime.Now;
            DateTime dgStop = dgStart.AddSeconds(10);
            var msg = $"Uruchomiono {Thread.CurrentThread.ManagedThreadId} z {args.Length} parametrami o godzinie:{dgStart}, zatrzymanie {dgStop}, tasków {maxTask}.";
            Console.WriteLine(msg);
            Logger.Instance.Loguj(msg);
            List<Task> lista = new List<Task>();
            while (DateTime.Compare(DateTime.Now, dgStop)<0)
            for (int i = 0; i < maxTask; i++)
            {
                lista.Add(Task.Run(() =>
                {
                    List<Task> lista2 = new List<Task>();
                    for (int j = 0; j < maxTask; j++)
                    {
                        lista2.Add(Task.Run(() =>
                        {
                            try
                            {
                                //string logMessage = $"Godzina logowania: {dgStart}; krok: {i}x{j}, threadId: {Thread.CurrentThread.ManagedThreadId}";
                                var logMessage = "Test";
                                Console.WriteLine(logMessage);
                                Logger.Instance.Loguj(logMessage);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Błąd w jednym z wątków logowania {ex}");
                            }
                        }
                        ));

                    }
                    Task.WhenAll(lista2).Wait();

                }));

            }
            Task.WhenAll(lista).Wait();
            msg = $"Zatrzymanie {Thread.CurrentThread.ManagedThreadId}.";
            Console.WriteLine(msg);
            Logger.Instance.Loguj(msg);

        }
    }
}
