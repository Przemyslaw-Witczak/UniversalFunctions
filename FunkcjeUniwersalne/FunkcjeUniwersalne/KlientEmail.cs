using System;
using System.Diagnostics;
//using System.Net.Mail;
using System.Windows.Forms;
using EASendMail;
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
                    #region Default SmtpClient
                    //MailMessage message = new MailMessage();
                    //SmtpClient client = new SmtpClient(serwer);                    
                    //message.From = new MailAddress(nadawca);
                    //message.To.Add(adresat);
                    //message.Subject = subject;
                    //message.Body = body;
                    //client.Port = port;
                    //client.Credentials = new System.Net.NetworkCredential(login, haslo);
                    //client.EnableSsl = true;                                      
                    //client.Send(message);
                    #endregion
                    #region Alternative from 
                    //https://www.emailarchitect.net/easendmail/ex/c/3.aspx
                    //EASendMail 
                    SmtpMail oMail = new SmtpMail("TryIt");
                    // Set sender email address, please change it to yours
                    oMail.From = nadawca;

                    // Set recipient email address, please change it to yours
                    oMail.To = adresat;

                    // Set email subject
                    oMail.Subject = subject;

                    // Set email body
                    oMail.TextBody = body;

                    // Your SMTP server address
                    SmtpServer oServer = new SmtpServer(serwer);

                    // User and password for ESMTP authentication, if your server doesn't require
                    // User authentication, please remove the following codes.
                    oServer.User = login;
                    oServer.Password = haslo;

                    // Set 25 or 587 port.
                    oServer.Port = port;

                    // detect TLS connection automatically
                    oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

                    Debug.WriteLine("start to send email ...");

                    SmtpClient oSmtp = new SmtpClient();
                    oSmtp.SendMail(oServer, oMail);

                    Debug.WriteLine("email was sent successfully!");

                    #endregion
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
