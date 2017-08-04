using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CIHelper.CreateRcloud;
using CIHelper.OnbConfig;

namespace CIHelper
{
    public class Program
    {

        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("There should be at least 1 agrument");
                return 1;
            }

            switch (args[0].ToLower())
            {
                case "-setappconfig":
                    return AppConfig.UpdateAppConfigValue(args[1], args[2], args[3]);
                //case "-uod":
                //    return UOD.RequestUOD(args[1]);
                //case "-uodlog":
                //    return UOD.UODBuildAppliedLog(args[1]);
                //case "-uodcheckforlatest":
                //    return UOD.UODBuildAppliedIsLatest(args[1]);
                //case "-checkifuodalreadysubmitted":
                //    return UOD.CheckIfUODAlreadySubmitted(args[1]);
                //case "-checkiftestsrunningonvm":
                //    return UOD.CheckIfTestsRunningonVm(args[1]);
                case "-changeenvironment":
                    return ChangeEnvironment.ChangeEchoEnvironment(args[1], args[2]);
                case "-changeenvironmentxml":
                    return ChangeEnvironment.ChangeEchoEnvironmentXml(args[1], args[2], args[3], args[4]);
                case "-changeparametersxml":
                    return ChangeEnvironment.ChangeEchoParametersXml(args[1], args[2]);
                //case "-changeprojectframework":
                //    return ChangeEnvironment.ChangeProjectFrameworkVersion(args[1], args[2], args[3]);
                //case "-saveresults":
                //    return SaveTestResults.SaveResults(args[1], args[2], args[3]);
                //case "-emailresults":
                //    return SaveTestResults.SendResultsEmail(args[1], args[2], args[3]);
                //case "-logresultsfor":
                //    return ResultsLog.LogResults(args[1]);
                case "-results":
                    return SaveTestResults.SaveTestRunResults(args[1]);
                case "-rcloud":
                    if (args.Length == 4)
                    {
                        return RCloud.ExecuteRCloudCommand(args[1], args[2], args[3]);
                    }
                    if (args.Length == 5)
                    {
                        return RCloud.ExecuteRCloudCommand(args[1], args[2], args[3], args[4]);
                    }
                    if (args.Length == 6)
                    {
                        return RCloud.ExecuteRCloudCommand(args[1], args[2], args[3], args[4], args[5]);
                    }
                    Console.WriteLine("Invalid parameter amount.");
                    return 1;
                case "-reportapi":
                    string textResult = ReadStageText(args[4], args);
                    ApiReporter api = new ApiReporter(textResult, args[4], args[2], Convert.ToInt32(args[3]), args[1]);
                    return 0;
                case "-createstatus":
                    ApiStatus apiStatus = new ApiStatus(args[1], args[2], args[3]);
                    return apiStatus.createStatus();
                case "-updatestatus":
                    ApiStatus apiStatusUpdate = new ApiStatus(args[1], args[2], args[3]);
                    return apiStatusUpdate.updateStatus(args[4]);
                case "-checkeverify":
                    return EverifyChecker.CheckEverify();
                case "-removebuilds":
                    try
                    {
                        var buildDirectory =
                            Directory.GetDirectories(string.Format(@"\\{0}\c$\{1}", args[1], args[2]))
                                .OrderBy(x => x.ToString())
                                .ToList();
                        int buildDelete = Convert.ToInt32(args[3]) - 5;
                        Console.WriteLine("Deleting build directories ending at build "+ buildDelete);
                        for (int i = 0; i <= buildDirectory.Count; i++)
                        {
                            int index = buildDirectory[i].LastIndexOf(@"\");
                            int value = Convert.ToInt32(buildDirectory[i].Substring(index + 1));
                            if(value <= buildDelete)
                            {
                                Directory.Delete(buildDirectory[i], true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to delete build directories " + ex.Message);
                    }
                    return 0;
                case "-getpassed":
                    List<SlackMessage> slackList = new List<SlackMessage>();
                    List<string> fileList = GetTextFiles(args);
                    foreach (var path in fileList)
                    {
                        try
                        {
                            var resultText = File.ReadAllText(path);
                            int value = resultText.LastIndexOf("Passed");
                            var resultarray = resultText.Substring(value).Split(':');
                            var passedValue = resultarray[1].Split(',')[0];
                            var failedValue = resultarray[2].Split(',')[0];
                            var index = path.LastIndexOf(@"\");
                            var newText = path.Substring(index);
                            var stage = newText.Replace(".txt", "").Replace(@"\", "");
                            slackList.Add(new SlackMessage(stage, passedValue, failedValue));
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            slackList.Add(new SlackMessage("ERROR READING STAGE", "0", "0"));
                        }
                    }
                    int totalPassed = 0;
                    int totalFailed = 0;
                    foreach (SlackMessage msg in slackList)
                    {
                        Console.Write(msg.stage+ ": ");
                        Console.WriteLine("" + msg.passed + "/" + msg.failed);
                        totalPassed += Convert.ToInt32(msg.passed);
                        totalFailed += Convert.ToInt32(msg.failed);
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Total: ");
                    Console.WriteLine(totalPassed + "/" + totalFailed);
                    return 0;
                default:
                    throw new Exception("First Argument Not Understood");
            }
        }

        private static List<String> GetTextFiles(string[] args)
        {
            //String result = File.ReadAllText(string.Format(@"\\devodw801\c$\{0}\{1}", args[1], args[2]));
            var resultFiles = Directory.GetFiles(string.Format(@"\\{0}\c$\{1}\{2}", args[1], args[2], args[3]));
            var resultTextFiles = resultFiles.Where(x => x.EndsWith(".txt")).AsEnumerable();
            return resultTextFiles.ToList();
        }

        private static string ReadStageText(string stage, string[] args)
        {
            string fileCopy = string.Format(@"\\{0}\c$\{1}\{2}\stage.txt", args[1], args[2], args[3]);
            List<string> txtFileList = GetTextFiles(args);
            var textFile = txtFileList.Where(x => x.Contains(stage)).ToList();
            File.Copy(textFile[0], fileCopy);
            var fileText =  File.ReadAllText(fileCopy);
            File.Delete(fileCopy);
            return fileText;
        }
    }
}
