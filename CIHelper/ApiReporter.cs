using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

namespace CIHelper
{
    public class ApiReporter
    {
        public string pipeline { get; set; }
        
        private static readonly HttpClient client = new HttpClient();
        public string machineName { get; set; }
        public string stage { get; set; }
        public int passed { get; set; }
        public int failed { get; set; }
        public List<string> stageErrors = new List<string>();
        public decimal duration { get; set; }
        public int buildNumber { get; set; }

        public int countErrors { get; set; }

        public int reportStage(int lastDuration)
        {
            if(lastDuration == 0)
            {
                var values = new Dictionary<string, object>
            {
                { "pipeline", this.pipeline },
                { "stage", this.stage },
                {"passed", this.passed.ToString() },
                {"failed", this.failed.ToString() },
                {"stageErrors", this.stageErrors },
                {"duration", this.duration.ToString()},
                {"buildNumber", this.buildNumber.ToString() },
                {"machineName", this.machineName },
                {"dateCompleted", DateTime.Now.ToString()}
            };

                string input = JsonConvert.SerializeObject(values);

                //var content = new FormUrlEncodedContent(values);
                var stringContent = new StringContent(input, Encoding.UTF8, "application/json");

                var response = client.PostAsync("http://wxvdepdprgud077:8000/api/report", stringContent);

                var responseString = response.Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(responseString);
                Debug.Print(responseString);
                if (responseString.Contains("success"))
                {
                    return 0;
                }
                return 1;
            }
            else
            {
                this.duration = lastDuration;
                var values = new Dictionary<string, object>
            {
                { "pipeline", this.pipeline },
                { "stage", this.stage },
                {"passed", this.passed.ToString() },
                {"failed", this.failed.ToString() },
                {"stageErrors", this.stageErrors },
                {"duration", this.duration.ToString()},
                {"buildNumber", this.buildNumber.ToString() },
                {"machineName", this.machineName },
                {"dateCompleted", DateTime.Now.ToString()},
            };

                string input = JsonConvert.SerializeObject(values);

                //var content = new FormUrlEncodedContent(values);
                var stringContent = new StringContent(input, Encoding.UTF8, "application/json");

                var response = client.PostAsync("http://wxvdepdprgud077:8000/api/report", stringContent);

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

        public ApiReporter(string textResult, string stage, string pipeline, int buildNumber, string machineName, int lastDuration = 0)
        {
            this.stage = stage;
            this.pipeline = pipeline;
            this.buildNumber = buildNumber;
            this.machineName = machineName;
            string errorOutput = GetErrorOutput(textResult);
            int count = GetErrorCount(errorOutput);
            this.countErrors = count;
            PopulateStageErrors(count, errorOutput);
            PopulatePassedAndFailed(textResult);
            this.reportStage(lastDuration);
        }

        public void PopulatePassedAndFailed(string text)
        {
            if (text == "")
            {
                this.passed = 0;
                this.failed = 0;
            }
            else
            {
                int value = text.LastIndexOf("Passed");
                var resultarray = text.Substring(value).Split(':');
                this.passed = Convert.ToInt32(resultarray[1].Split(',')[0]);
                this.failed = Convert.ToInt32(resultarray[2].Split(',')[0]);
                var index = text.IndexOf("Duration:");
                var durationList = text.Substring(index).Split(new string[] { "seconds" }, StringSplitOptions.None);
                this.duration = Convert.ToDecimal(durationList[0].Split(':')[1].Trim());
            }
            //var index = path.LastIndexOf(@"\");
            //var newText = path.Substring(index);
            //var stage = newText.Replace(".txt", "").Replace(@"\", "");
        }

        public string GetErrorOutput(string text)
        {
            string errorOutput = "";
            int failureIndex = text.IndexOf("Errors, Failures and Warnings");
            if (failureIndex != -1)
            {
                errorOutput = text.Substring(failureIndex);
            }
            return errorOutput;
        }

        public int GetErrorCount(string errorOutput)
        {
            if (errorOutput == "")
            {
                return 0;
            }
            int initialIndex = 0;
            int count = 0;
            while (initialIndex != -1)
            {
                int errorIndex = errorOutput.IndexOf("Error :", initialIndex + 1);
                int failureIndex = errorOutput.IndexOf("Failed :", initialIndex + 1);
                if(failureIndex != -1 && errorIndex != -1 && failureIndex < errorIndex)
                {
                    initialIndex = failureIndex;
                }
                else if(failureIndex != -1 && errorIndex == -1)
                {
                    initialIndex = failureIndex;
                }
                else
                {
                    initialIndex = errorIndex;
                }
                if (initialIndex != -1)
                {
                    count++;
                }
            }
            return count;
        }

        public void PopulateStageErrors(int count, string errorOutput)
        {
            int nextErrorIndex;
            int runSettingsIndex;
            int lastIndexFound = 0;
            var listString = new List<string>();
            listString.Add("Error :");
            listString.Add("Failed :");
            int listStringIndex;
            for (int i = 1; i <= count; i++)
            {
                listStringIndex = 0;
                var ErrorIndex = errorOutput.IndexOf("Error :", lastIndexFound + 1);
                var FailureIndex = errorOutput.IndexOf("Failed :", lastIndexFound + 1);
                if(FailureIndex > -1 && ErrorIndex > -1 && FailureIndex < ErrorIndex)
                {
                    ErrorIndex = FailureIndex;
                    listStringIndex = 1;
                }
                if(FailureIndex > -1 && ErrorIndex == -1)
                {
                    ErrorIndex = FailureIndex;
                    listStringIndex = 1;
                }
                lastIndexFound = ErrorIndex;
                if (i < count)
                {
                    nextErrorIndex = errorOutput.IndexOf(listString[listStringIndex], ErrorIndex + 1);
                    if(nextErrorIndex < ErrorIndex)
                    {
                        if (listStringIndex == 0)
                        {
                            nextErrorIndex = errorOutput.IndexOf(listString[1], ErrorIndex + 1);
                        }
                        else
                        {
                            nextErrorIndex = errorOutput.IndexOf(listString[0], ErrorIndex + 1);
                        }
                    }                   
                    stageErrors.Add(errorOutput.Substring(ErrorIndex, nextErrorIndex - ErrorIndex));
                    stageErrors[stageErrors.Count - 1] = stageErrors[stageErrors.Count - 1].Replace(i + 1 + ")", "");
                }
                else
                {
                    runSettingsIndex = errorOutput.IndexOf("Run Settings");
                    stageErrors.Add(errorOutput.Substring(ErrorIndex, runSettingsIndex - ErrorIndex));
                }
            }
        }
    }
}
