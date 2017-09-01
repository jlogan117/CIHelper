using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CIHelper
{
    public class ApiStatus
    {
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

            var response = client.PostAsync("http://wxvdepdprgud077:8000/api/status", stringContent);

            var responseString = response.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseString);
            Debug.Print(responseString);
            if (responseString.Contains("success"))
            {
                return 0;
            }
            return 1;
        }

        public int updateStatus(string result)
        {
            var values = new Dictionary<string, object>
            {
                { "pipeline", this.pipeline },
                { "buildNumber", this.buildNumber},
                {"dateCompleted", DateTime.Now.ToString() },
                {"result",  result}
            };

            string input = JsonConvert.SerializeObject(values);

            //var content = new FormUrlEncodedContent(values);
            var stringContent = new StringContent(input, Encoding.UTF8, "application/json");

            var response = client.PutAsync("http://wxvdepdprgud077:8000/api/status", stringContent);

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
            var values = new Dictionary<string, object>
            {
                { "pipeline", this.pipeline },
                { "buildNumber", this.buildNumber},
                {"currentStage", currentStage},
                {"currentStageNumber", stageNumber },
                {"currentStageStarted", DateTime.Now.ToString() }
            };

            string input = JsonConvert.SerializeObject(values);

            //var content = new FormUrlEncodedContent(values);
            var stringContent = new StringContent(input, Encoding.UTF8, "application/json");

            var response = client.PutAsync("http://wxvdepdprgud077:8000/api/status", stringContent);

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
