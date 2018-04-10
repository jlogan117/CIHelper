using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace CIHelper.CreateRcloud
{
    public static class RCloud
    {
        public static int ExecuteRCloudCommand(string command, string name, string product, string owner = "GabrielN", string build = "latest")
        {
            if (product.ToLower().Contains("onb"))
            {
                if (command.ToLower() == "all")
                {
                    return AllStepsONB(name: name);
                }
                if (command.ToLower() == "build")
                {
                    return VerifyOnboardingBuild();
                }
                if (command.ToLower() == "delete")
                {
                    //if (File.Exists(@"C:\\envname.txt"))
                    //{
                    //    name = File.ReadAllText(@"C:\\envname.txt");
                    //}
                    //File.WriteAllText(@"C:\\envname.txt", finalString);
                    return DeleteIfExists(name, owner);
                }
                //RC_osname = win2008r2
                //win2012
                if (command.ToLower() == "create")
                {
                    //Console.WriteLine($@"http://deploy/env/{name}?owner={owner}&team=hit&email=james_logan@ultimatesoftware.com&type=onboarding&osname=win2008r2");
                    //var createResult = CreateAndWaitRCloud(name: name, url: $@"http://deploy/env/{name}?owner={owner}&team=hit&email=james_logan@ultimatesoftware.com&type=onboarding&osname=win2008r2");
                    var createResult = CreateAndWaitRCloud(name: name, url: $@"http://deploy/env/{name}?owner={owner}&team=hit&email=james_logan@ultimatesoftware.com&type=onboarding&osname=win2012");
                    if (createResult == 0)
                    {
                        return createResult;
                    }
                    Console.WriteLine("Retrying to create Onboarding rCloud");
                    var deleteResult = DeleteIfExists(name, owner);
                    if (deleteResult == 1)
                    {
                        Console.WriteLine("FAILED to Delete RCloud on second RCloud creation.");
                        return deleteResult;
                    }
                    //return CreateAndWaitRCloud(name: name, url: $@"http://deploy/env/{name}?owner={owner}&team=hit&email=james_logan@ultimatesoftware.com&type=onboarding&RC_osname=win2008r2");
                    return CreateAndWaitRCloud(name: name, url: $@"http://deploy/env/{name}?owner={owner}&team=hit&email=james_logan@ultimatesoftware.com&type=onboarding&osname=win2012");
                }

                Console.WriteLine("INVALID PARAMETER HAS BEEN PASSED TO THE EXECUTABLE");
                return 1;
            }
            if (product.ToLower().Contains("up"))
            {
                if (command.ToLower() == "all")
                {
                    return AllStepsUP(name: name, owner: owner);
                }
                if (command.ToLower() == "create" && build == "latest")
                {
                    //"env/{0}?owner={1}&email={2}&team=HIT&type=shared_supersite&dbs=ULTIPRO_SB122,ULTIPRO_CALENDAR,ULTIPRO_HRPMCO&apply_oneoffs=true&apply_warmup=true
                    int upCreateValue = CreateAndWaitRCloud(name: name, url: $@"http://deploy/env/{name}?owner={owner}&email=javier_nunez@ultimatesoftware.com&team=HIT&type=shared_supersite&dbs=ULTIPRO_SB122,ULTIPRO_CALENDAR,ULTIPRO_HRPMCO&apply_oneoffs=true&apply_warmup=true&identity=true", timeout: 7200);
                    if (upCreateValue == 0)
                    {
                        return upCreateValue;
                    }
                    Console.WriteLine("Error creating Ultipro RCloud on first try; retrying...");
                    var deleteResult = DeleteIfExists(name, owner);
                    if (deleteResult == 1)
                    {
                        Console.WriteLine("FAILED to Delete RCloud on second RCloud creation.");
                        return deleteResult;
                    }
                    return CreateAndWaitRCloud(name: name, url: $@"http://deploy/env/{name}?owner={owner}&email=javier_nunez@ultimatesoftware.com&team=HIT&type=shared_supersite&dbs=ULTIPRO_SB122,ULTIPRO_CALENDAR,ULTIPRO_HRPMCO&apply_oneoffs=true&apply_warmup=true", timeout: 7200);

                }
                if (command.ToLower() == "create" && build != "latest")
                {
                    if (build != "previous")
                    {
                        return CreateAndWaitRCloud(name: name, url: $@"http://deploy/env/{name}?owner={owner}&email=javier_nunez@ultimatesoftware.com&team=HIT&type=shared_supersite&dbs=ULTIPRO_SB122,ULTIPRO_CALENDAR,ULTIPRO_HRPMCO&apply_oneoffs=true&apply_warmup=true&rev={build}", timeout: 7200);
                    }
                    else
                    {
                        HttpClient client = new HttpClient();
        //Get previous build, do retry.
                        var upbuildReponse = client.GetAsync("http://deploy/products/ultipro");
                        var prevUpbuild = upbuildReponse.Result.Content.ReadAsStringAsync().Result;
                        //prevUpbuild.Substring(53).Split('"')[0]
                        prevUpbuild = prevUpbuild.Substring(53).Split('"')[0];
                        var upcreateValue = CreateAndWaitRCloud(name: name, url: $@"http://deploy/env/{name}?owner={owner}&email=javier_nunez@ultimatesoftware.com&team=HIT&type=shared_supersite&dbs=ULTIPRO_SB122,ULTIPRO_CALENDAR,ULTIPRO_HRPMCO&apply_oneoffs=true&apply_warmup=true&rev={prevUpbuild}", timeout: 7200);
                        if (upcreateValue == 0)
                        {
                            return upcreateValue;
                        }
                        Console.WriteLine("Error creating Ultipro RCloud on first try; retrying...");
                        var deleteResult = DeleteIfExists(name, owner);
                        if (deleteResult == 1)
                        {
                            Console.WriteLine("FAILED to Delete RCloud on second RCloud creation.");
                            return deleteResult;
                        }
                        return CreateAndWaitRCloud(name: name, url: $@"http://deploy/env/{name}?owner={owner}&email=javier_nunez@ultimatesoftware.com&team=HIT&type=shared_supersite&dbs=ULTIPRO_SB122,ULTIPRO_CALENDAR,ULTIPRO_HRPMCO&apply_oneoffs=true&apply_warmup=true&rev={prevUpbuild}", timeout: 7200);
                    }
                }
                if (command.ToLower() == "delete")
                {
                    if (File.Exists(@"C:\\envname.txt"))
                    {
                        name = File.ReadAllText(@"C:\\envname.txt");
                    }
                    return DeleteIfExists(name, owner);
                }

                Console.WriteLine("INVALID PARAMETER HAS BEEN PASSED TO THE EXECUTABLE");
                return 1;
            }

            Console.WriteLine("INVALID PARAMETER HAS BEEN PASSED TO THE EXECUTABLE");
             return 1;
        }

        public static int AllStepsONB(string name)
        {
            if (VerifyOnboardingBuild() == 1)
            {
                return 1;
            }

            if (DeleteIfExists(name: name) == 1)
            {
                return 1;
            }
            return CreateAndWaitRCloud(name: name, url: $@"http://deploy/env/{name}?owner=gabrieln&team=hit&email=james_logan@ultimatesoftware.com&type=onboarding");
        }

        public static int AllStepsUP(string name, string owner)
        {
            if (DeleteIfExists(name: name, owner: owner) == 1)
            {
                return 1;
            }
            return CreateAndWaitRCloud(name: name, url: $@"http://deploy/env/{name}?owner={owner}&team=hit&email=james_logan@ultimatesoftware.com&type=onboarding", timeout: 7200);
        }

        private static int CreateAndWaitRCloud(string name = "ONBCITEST", string url = @"http://deploy/env/OnbCITest?owner=gabrieln&team=hit&email=james_logan@ultimatesoftware.com&type=onboarding", int timeout = 1800)
        {
            Console.WriteLine($"INTIATING {name} CREATION...");
            var response = PutRequest(url);
            if (response == 0)
            {
                Console.WriteLine("WAITING A FEW MINUTES BEFORE CHECKING IF CREATION HAS COMPLETED...");
                Thread.Sleep(120000);
                Stopwatch s = new Stopwatch();
                s.Start();
                while (s.Elapsed < TimeSpan.FromSeconds(timeout))
                {
                    Console.WriteLine($"CHECKING IF {name} HAS COMPLETED CREATION..");
                    if (GetEnvRequest($"http://deploy/env/{name}", name) == 0)
                    {
                        Console.WriteLine($"{name} HAS BEEN SUCCESSFULLY CREATED.");
                        return 0;
                    }
                    Console.WriteLine($"{name} HAS NOT BEEN CREATED YET, WAITING 1 MINUTES BEFORE RETRY.");
                    Thread.Sleep(60000);
                }

                s.Stop();
                Console.WriteLine($"TIMEOUT EXPIRED. {name} WAS NOT SUCCESSFULLY CREATED.");

                return 1;
            }
            return 1;
        }

        private static int DeleteIfExists(string name = "ONBCITEST", string owner = "GabrielN")
        {
            Console.WriteLine($"CHECKING IF {name} ALREADY EXISTS");
            if (GetRCloud(name, owner) != 0)
            {
                Console.WriteLine($"{name} ALREADY EXISTS, INITIATING DELETE");
                return DeleteRCloud(name, owner);
            }
            return 0;
        }

        public static int DeleteRCloud(string name = "ONBCITEST", string owner = "GabrielN")
        {
            Console.WriteLine($"INITIATING {name} DELETION...");
            var response = DeleteRequest($@"http://deploy/env/{name}?owner={owner}&email=james_logan@ultimatesoftware.com&team=HIT");
            if (response == 0)
            {
                Console.WriteLine("WAITING A ONE MINUTE BEFORE CHECKING IF RCloud HAS BEEN DELETED...");
                Thread.Sleep(60000);
                Stopwatch s = new Stopwatch();
                s.Start();
                while (s.Elapsed < TimeSpan.FromSeconds(1200))
                {
                    Console.WriteLine($"CHECKING IF {name} HAS BEEN DELETED..");
                    if (GetRCloud(name, owner) == 0)
                    {
                        Console.WriteLine($"{name} HAS BEEN SUCCESSFULLY DELETED.");
                        return 0;
                    }
                    Console.WriteLine($"{name} HAS NOT BEEN DELETED YET, WAITING 1 MIN BEFORE RETRY");
                    Thread.Sleep(60000);
                }

                s.Stop();
                Console.WriteLine($"TIMEOUT HAS OCCURED, {name} WAS NOT SUCCESSFULLY DELETED.");
                return 1;
            }
            return 1;

        }

        public static int GetRCloud(string name = "ONBCITEST", string owner = "GabrielN")
        {
            Console.WriteLine($"DETERMINING IF {name} EXISTS...");
            return GetUserRequest($@"http://deploy/env?owner={owner}&team=HIT", name);
        }

        public static int PutRequest(string postUrl)
        {
            HttpWebResponse response;
            var request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.ContentLength = 0;
            try
            {
                response = (HttpWebResponse) request.GetResponse();
                Console.WriteLine(ReadBody(response));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
            string returnString = response.StatusCode.ToString();
            return HandleResponse(returnString);
        }

        public static int DeleteRequest(string deleteUrl)
        {
            HttpWebResponse response;
            var request = (HttpWebRequest)WebRequest.Create(deleteUrl);
            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.ContentLength = 0;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(ReadBody(response));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
            string returnString = response.StatusCode.ToString();
            return HandleResponse(returnString);
        }

        public static int GetUserRequest(string getUrl, string name)
        {
            HttpWebResponse response;
            var request = (HttpWebRequest)WebRequest.Create(getUrl);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.ContentLength = 0;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var body = ReadBody(response);
                Console.WriteLine(body);
                if (body.ToLower().Contains(name.ToLower()))
                {
                    Console.WriteLine($"\n{name} ALREADY EXISTS");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
            string returnString = response.StatusCode.ToString();
            return HandleResponse(returnString);
        }

        public static int GetEnvRequest(string getUrl, string name)
        {
            HttpWebResponse response;
            var request = (HttpWebRequest)WebRequest.Create(getUrl);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.ContentLength = 0;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var body = ReadBody(response);
                Console.WriteLine(body);
                if (!body.ToLower().Contains("\"done\":true"))
                {                 
                    Console.WriteLine($"\n{name} Has Not Completed Creation.");
                    return 1;
                }
                else
                {
                    if (!body.ToLower().Contains("\"succeeded\":true"))
                    {
                        throw new Exception("RCloud Failed");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
            string returnString = response.StatusCode.ToString();
            return HandleResponse(returnString);
        }

        public static string GetOnbBuildNumber(string getUrl, string currentBuild = "error")
        {
            HttpWebResponse response;
            var request = (HttpWebRequest)WebRequest.Create(getUrl);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.ContentLength = 0;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var body = ReadBody(response).Substring(17).Split('\"')[0];
                return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return currentBuild;
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

        public static int HandleResponse(string status)
        {
            Console.WriteLine("Status = " + status);
            if (status.ToLower() != "ok")
            {
                Console.WriteLine("This Request Has Failed.");
                return 1;
            }
            return 0;
        }

        public static int VerifyOnboardingBuild()
        {
            string currentOnbBuildNumber;
            currentOnbBuildNumber = GetOnbBuildNumber("http://deploy/products/onboarding");
            try
            {
                var lastBuild = File.ReadAllText(@"C:\\builds\buildFile.txt");
                Console.WriteLine("Last Build Number is: " + lastBuild);
                if (lastBuild != currentOnbBuildNumber)
                {
                    Console.WriteLine("Last Build Ran Was Different then current, skipping waiting for Build.");
                    File.WriteAllText(@"C:\\builds\buildFile.txt", currentOnbBuildNumber);
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Last Build File was not found.");
            }
            if (currentOnbBuildNumber == "error")
            {
                return 1;
            }
            Console.WriteLine("WAITING FOR New Onboarding BUILD: Current Build Number is : " + currentOnbBuildNumber);
            Stopwatch watchBuild = new Stopwatch();
            watchBuild.Start();
            while (watchBuild.Elapsed < TimeSpan.FromSeconds(4500))
            {
                Console.WriteLine("CHECKING IF New Onboarding Build Has Been Deployed..");
                string newBuild = GetOnbBuildNumber("http://deploy/products/onboarding", currentOnbBuildNumber);
                if (newBuild != currentOnbBuildNumber)
                {
                    Console.WriteLine("New Onboarding Build " + newBuild + " has been completed.");
                    File.WriteAllText(@"C:\\buildFile.txt", currentOnbBuildNumber);
                    return 0;
                }
                Console.WriteLine("NEW ONBOARDING BUILD HAS NOT BEEN CREATED YET, WAITING 1 MINUTE BEFORE RETRY.");
                Thread.Sleep(60000);
            }

            watchBuild.Stop();
            return 1;
        }
    }
}
