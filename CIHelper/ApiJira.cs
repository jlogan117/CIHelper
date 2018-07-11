using CIHelper.CreateRcloud;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CIHelper
{
    public static class ApiJira
    {
        private static readonly HttpClient client = new HttpClient();
        public static List<string> GetOneoffJiras()
        {
            //http://wxvdepdprgud077:8000/api/jirasearch?jql= project = UltiPro AND type = OneOff AND "Target Customer Deployment Date" > 0d AND "Target Customer Deployment Date" < 7d AND status not in (Canceled) ORDER BY key ASC
            string query = @"jql= project = UltiPro AND type = Patch AND ""Target Customer Deployment Date"" >= 0d AND ""Target Customer Deployment Date"" < 10d AND status not in (Canceled) ORDER BY ""Target Customer Deployment Date"" ASC, key ASC";
            var upbuildReponse = client.GetAsync("http://wxvdepdprgud077:8000/api/jirasearch?" + query);
            var response = upbuildReponse.Result.Content.ReadAsStringAsync().Result;
            var jiraNumberArray = response.Split(new string[] { "jiraNumber" }, StringSplitOptions.None).ToList();
            var jiraArray = new List<string>();
            for (int i = 1; i < jiraNumberArray.Count; i++)
            {
                Console.WriteLine(jiraNumberArray[i].Substring(3, 11));
                jiraArray.Add(jiraNumberArray[i].Substring(3, 11));
            }
            return jiraArray;
        }

        public static int ApplyOneoffs(List<string> oneOffList, string envname, string owner = "JavierN", string team = "hit")
        {
            //http://deploy.newgen.corp/one_offs/LakersR1
            // { "one_offs_list":"ULTI-245748, ULTI-244508","owner":"shawnlo","team":"hit","name":"LakersR1","operation":"one_offs"}
            //http://deploy/env/LakersR1 
            string oneoffString = "";
            owner = "hit_ci";
            for (int i = 0; i < oneOffList.Count; i++)
            {
                if (i == 0)
                {
                    oneoffString += oneOffList[i] + ",";
                }
                else if(i == (oneOffList.Count - 1))
                {
                    oneoffString += " " + oneOffList[i];
                }
                else
                {
                    oneoffString += " " + oneOffList[i] + ",";
                }
            }
            Console.WriteLine("ONEOFFS APPLIED TO THIS ENV: ");
            Console.WriteLine(oneoffString);
            Console.WriteLine(envname);
            var values = new Dictionary<string, object>
            {
                { "one_offs_list", oneoffString },
                { "owner", owner },
                {"team", team },
                {"name", envname },
                {"operation", "one_offs" }
            };

            string input = JsonConvert.SerializeObject(values);

            //var content = new FormUrlEncodedContent(values);
            var stringContent = new StringContent(input, Encoding.UTF8, "application/json");

            var response = client.PutAsync($"http://deploy.newgen.corp/one_offs/{envname}", stringContent);
            var responseString = response.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseString);
            //Debug.Print(responseString);
            Console.WriteLine("ONEOFF RESPONSE: ");
            Console.WriteLine(responseString);
            if (responseString.Contains("status_uri"))
            {
                return 0;
            }
            return 1;
        }

        internal static int WaitOnStatus(string envname, int timeout = 3600)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            while (s.Elapsed < TimeSpan.FromSeconds(timeout))
            {
                Console.WriteLine($"CHECKING IF {envname} Oneoffs Have Applied.");
                if (RCloud.GetEnvRequest($"http://deploy/env/{envname}", envname) == 0)
                {
                    Console.WriteLine($"{envname} HAS HAD ITS ONEOFFS SUCCESSFULLY APPLIED.");
                    return 0;
                }
                Console.WriteLine($"{envname} ONEOFFS HAVE NOT BEEN APPLIED, WAITING 1 MINUTES BEFORE RETRY.");
                Thread.Sleep(60000);
            }

            s.Stop();
            Console.WriteLine($"TIMEOUT EXPIRED. Oneoffs for {envname} WERE NOT SUCCESSFULLY APPlIED.");

            return 1;
        }
    }
    //public class Jira
    //{

    //}
}
