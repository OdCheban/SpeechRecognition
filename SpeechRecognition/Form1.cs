using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Microsoft.Speech.Recognition;

namespace SpeechRecognition
{
    public partial class Form1 : Form
    {
        public static string urlGet = "http://192.168.0.133:8187/api/speech";
        public List<string> strKey = new List<string>()
        { "список мероприятий", "главное меню","библиотека", "покажи первую презентацию","полноэкранный режим" };

        public Form1()
        {
            InitializeComponent();
        }
        

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.7)
            {
                label1.Text = e.Result.Text;
                POST(e.Result.Text);
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("ru-ru");
            SpeechRecognitionEngine sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
          
            Choices choicesKey = new Choices();
            choicesKey.Add(strKey.ToArray());
            
            GrammarBuilder gb = new GrammarBuilder();
            gb.Culture = ci;
            gb.Append(choicesKey);
            
            Grammar g = new Grammar(gb);
            sre.LoadGrammar(g);

            sre.RecognizeAsync(RecognizeMode.Multiple);


        }


        private static string POST(string Data)
        {
            WebRequest req = WebRequest.Create(urlGet);
            req.Method = "POST";
            req.Timeout = 100000;
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] sentData = Encoding.GetEncoding(1251).GetBytes(Data);
            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();
            WebResponse res = req.GetResponse();
            Stream ReceiveStream = res.GetResponseStream();
            StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8);

            Char[] read = new Char[256];
            int count = sr.Read(read, 0, 256);
            string Out = String.Empty;
            while (count > 0)
            {
                String str = new String(read, 0, count);
                Out += str;
                count = sr.Read(read, 0, 256);
            }
            return Out;
        }
    }
}
