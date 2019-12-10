using AegisImplicitMail;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mail;
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
            //https://stackoverflow.com/questions/1011245/how-can-i-send-emails-through-ssl-smtp-with-the-net-framework
            //Task.Factory.StartNew(() =>            
            {
                try
                {
                    var message = new MimeMailMessage();
                    
                    message.From = new MailAddress(nadawca);
                    message.To.Add(adresat);
                    message.Subject = subject;
                    message.Body = body;

                    var client = new SmtpSocketClient();
                    client.Host = serwer;
                    client.Port = port;
                    client.SslType = SslMode.Ssl;
                    client.User = login;
                    client.Password = haslo;
                    client.AuthenticationMode = AuthenticationType.Base64;
                    client.MailMessage = message;                    
                    client.SendMailAsync();
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
