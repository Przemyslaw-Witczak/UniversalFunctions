﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MojeFunkcjeUniwersalneNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojeFunkcjeUniwersalneNameSpace.Tests
{
    [TestClass()]
    public class KlientEmailTests
    {
        [TestMethod()]
        public void SendMailMasternetTest()
        {
            try
            {
                KlientEmailUstawienia.Instance.temat = "Mail testowy z AzzureFunctions";
                KlientEmailUstawienia.Instance.adresat = "witczakprzemyslaw@gmail.com";
                KlientEmailUstawienia.Instance.SendEmail = true;
                KlientEmailUstawienia.Instance.nadawca = "motionpi@dowisoft.masternet.pl";
                KlientEmailUstawienia.Instance.login = "motionpi@dowisoft.masternet.pl";
                KlientEmailUstawienia.Instance.haslo = "!Qaz@Wsx3edc";
                KlientEmailUstawienia.Instance.serwer = "s4.masternet.pl";
                KlientEmailUstawienia.Instance.port = 465;

                KlientEmail klientEmail = new KlientEmail(KlientEmailUstawienia.Instance);
                klientEmail.SendMail($"SendMailTest");
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [TestMethod()]
        public void SendMailGoogleTest()
        {
            try
            {
                KlientEmailUstawienia.Instance.temat = "Mail testowy z Google";
                KlientEmailUstawienia.Instance.adresat = "witczakprzemyslaw@o2.pl";
                KlientEmailUstawienia.Instance.SendEmail = true;
                KlientEmailUstawienia.Instance.nadawca = "witczakprzemyslaw@gmail.com";
                KlientEmailUstawienia.Instance.login = "witczakprzemyslaw@gmail.com";
                KlientEmailUstawienia.Instance.haslo = "27032002Mx";
                KlientEmailUstawienia.Instance.serwer = "smtp.gmail.com";
                KlientEmailUstawienia.Instance.port = 465;

                KlientEmail klientEmail = new KlientEmail(KlientEmailUstawienia.Instance);
                klientEmail.SendMail($"SendMailTest");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [TestMethod()]
        public void SendMailTlenTest()
        {
            try
            {
                KlientEmailUstawienia.Instance.temat = "Mail testowy z Tlen";
                KlientEmailUstawienia.Instance.adresat = "witczakprzemyslaw@google.com";
                KlientEmailUstawienia.Instance.SendEmail = true;
                KlientEmailUstawienia.Instance.nadawca = "witczakprzemyslaw@o2.pl";
                KlientEmailUstawienia.Instance.login = "witczakprzemyslaw@gmail.com";
                KlientEmailUstawienia.Instance.haslo = "27032002Mxxx";
                KlientEmailUstawienia.Instance.serwer = "poczta.o2.pl";
                KlientEmailUstawienia.Instance.port = 465;

                KlientEmail klientEmail = new KlientEmail(KlientEmailUstawienia.Instance);
                klientEmail.SendMail($"SendMailTest");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}