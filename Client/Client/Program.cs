using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program : WebClient
    {
        
        static void Main(string[] args)
        {
            Program p = new Program();
            do
            {
                //p.Request();
                Console.WriteLine( p.postForm());
                System.Threading.Thread.Sleep(1000);
            } while (true);
            
        }

        string url = "https://192.168.0.115/login.php";
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.KeepAlive = false;
            request.ClientCertificates.Add(new X509Certificate2());
            return request;
        }

        void Request()
        {
            string html = string.Empty;
            
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            var request = GetWebRequest(new Uri(url));
            
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            Console.WriteLine(html);
        }

        private String postForm()
        {

            //setup some variables

            String username = "demo";
            String password = "password";

            //setup some variables end

            String result = "";
            String strPost = "username=" + username + "&password=" + password;
            StreamWriter myWriter = null;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.KeepAlive = false;
            objRequest.Method = "POST";
            objRequest.ContentLength = strPost.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";

            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(strPost);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                myWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr =
               new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();

                // Close and clean up the StreamReader
                sr.Close();
            }
            return result;
        }

        bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
