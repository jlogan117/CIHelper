using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIHelper
{
    public static class ResultsLog
    {
        public static int LogResults(string jobName)
        {
            var jobDirectory = new DirectoryInfo(string.Format(@"\\devhitv10\c$\Program Files (x86)\Jenkins\jobs\{0}\builds", jobName));
            var latestBuild = jobDirectory.GetDirectories().OrderByDescending(f => f.LastWriteTime).First();
            string latestBuildLog = string.Format(@"\\devhitv10\c$\Program Files (x86)\Jenkins\jobs\{0}\builds\{1}\log", jobName, latestBuild);
            string latestBuildLogCopy = string.Format(@"\\devhitv10\c$\Program Files (x86)\Jenkins\jobs\{0}\builds\{1}\logCopy", jobName, latestBuild);

            System.IO.File.Copy(latestBuildLog, latestBuildLogCopy, true); //Need to make copy of log because original log is being used by Jenkins
            
            StreamReader log = new StreamReader(latestBuildLogCopy);
            string logFile = log.ReadToEnd();

            if (logFile.Contains("Finished: ABORTED"))
            {
                System.IO.File.AppendAllText(@"C:\CurrentTests\ResultsLog.txt", string.Format("{0} Finished: ABORTED with No Results check Jenkins to Debug\r\n\r\n" +
                    "0 run, 0 passed, 0 failed, 0 inconclusive, 0 skipped\r\n\r\n\r\n", jobName));
            }

            if (logFile.Contains("Finished: FAILURE") && (!logFile.Contains("Stop time:") || logFile.Contains("Error: A fatal exception occurred")))
            {
                System.IO.File.AppendAllText(@"C:\CurrentTests\ResultsLog.txt", string.Format("{0} Finished: FAILURE with No Results check Jenkins to Debug\r\n\r\n" +
                    "0 run, 0 passed, 0 failed, 0 inconclusive, 0 skipped\r\n\r\n\r\n", jobName));
            }

            if (logFile.Contains("Finished: FAILURE") && (logFile.Contains("Stop time:") && !logFile.Contains("Error: A fatal exception occurred")))
            {
                int startIndex = logFile.IndexOf("Stop time:") + "Stop time:".Length;
                int endIndex = logFile.IndexOf("skipped");
                string resultsString = logFile.Substring(startIndex, endIndex - startIndex);

                System.Console.WriteLine(string.Format("Results copied from {0} to ResultsLog.txt", latestBuildLog));

                System.IO.File.AppendAllText(@"C:\CurrentTests\ResultsLog.txt", string.Format("{0} Finished: FAILURE at{1}skipped\r\n\r\n\r\n", jobName, resultsString));
            }

            if (logFile.Contains("Finished: SUCCESS"))
            {
                int startIndex = logFile.IndexOf("Stop time:") + "Stop time:".Length;
                int endIndex = logFile.IndexOf("skipped");
                string resultsString = logFile.Substring(startIndex, endIndex - startIndex);

                System.Console.WriteLine(string.Format("Results copied from {0} to ResultsLog.txt", latestBuildLog));

                System.IO.File.AppendAllText(@"C:\CurrentTests\ResultsLog.txt", string.Format("{0} Finished: SUCCESS at{1}skipped\r\n\r\n\r\n", jobName, resultsString));
            }

            log.Close();
            System.IO.File.Delete(latestBuildLogCopy);

            return 0;
        }
    }
}
