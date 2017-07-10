using System;
using CIHelper.CreateRcloud;

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
                default:
                    throw new Exception("First Argument Not Understood");
            }
        }
    }
}
