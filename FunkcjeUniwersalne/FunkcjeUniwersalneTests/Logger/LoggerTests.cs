using Microsoft.VisualStudio.TestTools.UnitTesting;
using MojeFunkcjeUniwersalneNameSpace.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MojeFunkcjeUniwersalneNameSpace.Logger.Tests
{
    [TestClass()]
    public class LoggerTests
    {
        [TestMethod()]
        public void LogujTest()
        {
            int maxTask = 5;
            
            DateTime dgStart = DateTime.Now;
            List<Task> lista = new List<Task>();
            for (int i = 0; i < maxTask; i++)
            {
                lista.Add(Task.Run(() =>
                {
                    List<Task> lista2 = new List<Task>();
                    for (int j = 0; j < maxTask; j++)
                    {
                        lista2.Add(Task.Run(() =>
                            {
                                //Console.WriteLine($"Uruchomiono o godzinie:{dgStart}, krok: i={i}, j={j}, threadId: { Thread.CurrentThread.ManagedThreadId}.");
                                string logMessage = $"Godzina logowania: {dgStart}; krok: {i}x{j}, threadId: {Thread.CurrentThread.ManagedThreadId}";
                                Logger.Instance.Loguj(logMessage);                                
                            }
                        ));

                    }
                    Task.WhenAll(lista2).Wait();

                }));

            }
            Task.WhenAll(lista).Wait();

            Assert.IsTrue(true);
        }
    }
}