using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CIHelper
{
    public static class EverifyChecker
    {
        public static int CheckEverify()
        {
            HttpWebResponse response;
            var request = (HttpWebRequest)WebRequest.Create("https://stage.n-e-verify.uscis.gov/services/EmployerWebServiceV29.svc?wsdl");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.ContentLength = 0;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var body = ReadBody(response);
                Console.WriteLine(body);
                string returnString = response.StatusCode.ToString();
                if (returnString.ToLower() == "ok" && body.ToLower().Contains("http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"))
                {
                    Console.WriteLine("Everify is up and running..");
                    return 0;
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        public static string ReadBody(HttpWebResponse response)
        {
            string responseBody;
            using (Stream receiveStream = response.GetResponseStream())
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    responseBody = readStream.ReadToEnd();
                }
            }
            return responseBody;
        }
    }
}
