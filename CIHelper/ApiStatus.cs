﻿using System;
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

        public int createStatus()
        {
            var values = new Dictionary<string, object>
            {
                { "pipeline", this.pipeline },
                { "buildNumber", this.buildNumber},
                { "machineName", this.machineName },
                {"dateStarted", DateTime.Now.ToString() },
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

        public int updateStatus()
        {
            var values = new Dictionary<string, object>
            {
                { "pipeline", this.pipeline },
                { "buildNumber", this.buildNumber},
                {"dateCompleted", DateTime.Now.ToString() },
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
