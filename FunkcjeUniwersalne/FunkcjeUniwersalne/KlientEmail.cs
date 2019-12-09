﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MojeFunkcjeUniwersalneNameSpace
{
    public class KlientEmail
    {
        KlientEmailUstawienia serwer_smtp;

        public KlientEmail()
        {
            serwer_smtp = null;
        }
        
        public KlientEmail(KlientEmailUstawienia UstawieniA)
        {
            serwer_smtp = UstawieniA;
        }

        /// <summary>
        /// Funkcja wysyłająca email
        /// </summary>
        /// <param name="body">Treść wiadomości</param>
        public void SendMail(string body)
        {
            SendMail(body, "");

        }

        /// <summary>
        /// Funkcja wysyłająca email
        /// </summary>
        /// <param name="body">Treśc wiadomości</param>
        /// <param name="subject">Temat wiadomości</param>
        public void SendMail(string body, string subject)
        {
            if (serwer_smtp == null)
            {
                return;
            }

            SendMail(body, subject, serwer_smtp.adresat);
        }

        /// <summary>
        /// Funkcja wysyłająca email
        /// </summary>
        /// <param name="body">Treść wiadomości</param>
        /// <param name="subject">Temat wiadomości</param>
        /// <param name="adresat">Adresat</param>
        public void SendMail(string body, string subject, string adresat)
        {
            if (serwer_smtp == null || string.IsNullOrEmpty(serwer_smtp.serwer))
            {
                return;
            }

            SendMail(body, subject, adresat, serwer_smtp.nadawca, serwer_smtp.serwer, serwer_smtp.login, serwer_smtp.haslo, serwer_smtp.port);
        }

        /// <summary>
        /// Funkcja wysyłająca email
        /// </summary>
        /// <param name="body">Treść wiadomości</param>
        /// <param name="subject">Temat wiadomości</param>
        /// <param name="adresat">Adresat wiadomości</param>
        /// <param name="nadawca">Nadawca, adres email</param>
        /// <param name="serwer">Adres serwera SMTP</param>
        /// <param name="login">Login do serwera SMTP</param>
        /// <param name="haslo">Hasło do serwera SMTP</param>
        /// <param name="port">Port usługi SMTP na serwerze</param>
        public void SendMail(string body, string subject, string adresat, string nadawca, string serwer, string login, string haslo, int port)
        {
            //MessageBox.Show(body + " " + subject + " " + adresat + " " + nadawca + " " + serwer + " " + login + " " + haslo);
            if (string.IsNullOrEmpty(serwer) || string.IsNullOrEmpty(adresat) || string.IsNullOrEmpty(nadawca))
            {
                return;
            }
            //string body = @"Using this new feature, you can send an e-mail message from an application very easily.";
            //http://csharp.net-informations.com/communications/csharp-smtp-mail.htm
            //Task.Factory.StartNew(() =>            
            {
                try
                {
                    MailMessage message = new MailMessage(nadawca, adresat, subject, body);
                    message.Sender = new MailAddress(nadawca);

                    SmtpClient client = new SmtpClient();

                    client.UseDefaultCredentials = false;
                    client.Host = serwer;
                    client.Credentials = new System.Net.NetworkCredential(login, haslo);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;
                    client.Port = port;                    
                    //client.Timeout = 2 * 60 * 1000;

                    client.Send(message);
                }
                catch (Exception ex)
                {
                    try
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter("logi_email_" + Application.ProductName + ".txt", true, System.Text.Encoding.Default))
                        {
                            file.WriteLine($"{DateTime.Now}; Błąd wysyłki maila przez serwer:'{serwer}', login:'{nadawca}' Ex:{ex}");
                            //file.Close();
                        }
                        throw ex;

                    }
                    catch(Exception ex2)
                    {
                        Debug.WriteLine($"Error in SendMail test: {ex2}");
#if DEBUG
                        throw ex2;
#endif
                    }
                }
            }
            //);

            
        }
    
    
    
    }
}
