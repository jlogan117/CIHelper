using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CIHelper
{
    public class ApiStatus
    {
        private static string apiServerName = "wxvdepdprgud077";
        //private static string apiServerName = "localhost";

        private static readonly HttpClient client = new HttpClient();
        public string pipeline { get; set; }

        public string buildNumber { get; set; }

        public string machineName { get; set; }

        public ApiStatus(string machineName, string pipeline, string buildNumber)
        {
            this.machineName = machineName;
            this.pipeline = pipeline;
            this.buildNumber = buildNumber;
        }

        public int createStatus(string stageTotal)
        {
            var upbuild = "";
            var obbuild = "";
            if (this.pipeline.ToLower().Contains("ultipro") || this.pipeline.ToLower().Contains("integration"))
            {
                //get latest ultipro build
                var upbuildReponse = client.GetAsync("http://deploy/products/ultipro");
                upbuild = upbuildReponse.Result.Content.ReadAsStringAsync().Result;
                upbuild = upbuild.Substring(17).Split('"')[0];
            }
            if (this.pipeline.ToLower().Contains("onboarding"))
            {
                var obBuildResponse = client.GetAsync("http://deploy/products/onboarding");
                obbuild = obBuildResponse.Result.Content.ReadAsStringAsync().Result;
                obbuild = obbuild.Substring(17).Split('"')[0];
            }
            var values = new Dictionary<string, object>
            {
                { "pipeline", this.pipeline },
                { "buildNumber", this.buildNumber},
                { "machineName", this.machineName },
                {"dateStarted", DateTime.Now.ToString() },
                {"stageTotal",  stageTotal},
                {"upBuildNumber", upbuild },
                {"onbBuildNumber", obbuild }
            };

            string input = JsonConvert.SerializeObject(values);

            //var content = new FormUrlEncodedContent(values);
            var stringContent = new StringContent(input, Encoding.UTF8, "application/json");

            var response = client.PostAsync(string.Format("http://{0}:8000/api/status", apiServerName), stringContent);

            var responseString = response.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseString);
            Debug.Print(responseString);
            if (responseString.Contains("success"))
            {
                return 0;
            }
            return 1;
        }

        public static string getLastRunBrowser(string pipelineName)
        {
            var response = client.GetAsync(string.Format("http://{0}:8000/api/status?pipeline={1}&status=completed", apiServerName, pipelineName));

            var responseString = response.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseString);

            string browser;
            try
            {
                int indexBrowser = responseString.IndexOf("browserType");
                if (indexBrowser != -1)
                {
                    browser = responseString.Substring(indexBrowser + 14).Split(',')[0]
                        .Replace("\"", "");
                }
                else
                {
                    browser = "none";
                }
            }
            catch (Exception e)
            {
                browser = "none";
            }

            
            return browser;
        }

        public int updateStatus(string result)
        {
            var browserType = ChangeEnvironment.GetEchoParametersXml();
            
            var values = new Dictionary<string, object>
            {
                { "pipeline", this.pipeline },
                { "buildNumber", this.buildNumber},
                { "dateCompleted", DateTime.Now.ToString() },
                { "result",  result},
                { "browserType", browserType }
            };

            string input = JsonConvert.SerializeObject(values);

            //var content = new FormUrlEncodedContent(values);
            var stringContent = new StringContent(input, Encoding.UTF8, "application/json");

            var response = client.PutAsync(string.Format("http://{0}:8000/api/status", apiServerName), stringContent);

            var responseString = response.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseString);
            Debug.Print(responseString);
            if (responseString.Contains("success"))
            {
                return 0;
            }
            return 1;
        }

        public int updateStatusWithCurrentStage(string currentStage, string stageNumber)
        {
            var apiEndPoint = string.Format("http://{0}:8000/api/laststageduration?pipeline=", apiServerName) + this.pipeline +"&stage=" + currentStage;
            var lastDurationResponse = client.GetAsync(apiEndPoint);
            var lastDuration = lastDurationResponse.Result.Content.ReadAsStringAsync().Result;
            if (!lastDuration.Contains("500"))
            {
                lastDuration = lastDuration.Split(':')[4].Split('}')[0].Replace("\"", "");
            }
            else
            {
                lastDuration = "0";
            }
            var values = new Dictionary<string, object>
            {
                { "pipeline", this.pipeline },
                { "buildNumber", this.buildNumber},
                {"currentStage", currentStage},
                {"currentStageNumber", stageNumber },
                {"currentStageStarted", DateTime.Now.ToString() },
                {"currentStageLastDuration", lastDuration }
            };

            string input = JsonConvert.SerializeObject(values);

            //var content = new FormUrlEncodedContent(values);
            var stringContent = new StringContent(input, Encoding.UTF8, "application/json");

            var response = client.PutAsync(string.Format("http://{0}:8000/api/status", apiServerName), stringContent);

            var responseString = response.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseString);
            Debug.Print(responseString);
            if (responseString.Contains("success"))
            {
                return 0;
            }
            return 1;
        }
    }
}
