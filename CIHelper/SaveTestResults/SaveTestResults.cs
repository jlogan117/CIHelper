using System;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace CIHelper
{
    public static class SaveTestResults
    {
        public static int SendResultsEmail(string suiteName, string environmentName, string nodeName)
        {
            string buildNumber = System.IO.File.ReadAllText(string.Format(@"\\devhitv10\c$\UODLogs\{0}\BuildApplied.txt", environmentName));
            buildNumber = buildNumber.Substring(buildNumber.LastIndexOf("=") + 1);

            MailMessage message = new MailMessage();
            message.From = new MailAddress("devhitv10@ultimatesoftware.com");
            message.To.Add(new MailAddress("James_Logan@ultimatesoftware.com"));
            message.To.Add(new MailAddress("Javier_Nunez@ultimatesoftware.com"));
            message.Subject = string.Format("CI has completed running {0} tests with Build #{1}", suiteName, buildNumber);

            string content = System.IO.File.ReadAllText(@"C:\CurrentTests\ResultsLog.txt");
            message.Body = string.Format("Build: {0}" + "\n" +
                                        "Environment: {1}" + "\n" +
                                        "Node: {2}" + "\n" +
                                        @"Echo Results: \\{3}\c$\AutomatedTestResults" + "\n" +
                                        "Jenkins Results: \"http://devhitv10:8080/view/{4}\"" + "\n",
                                        buildNumber, environmentName, nodeName, nodeName, suiteName) +
                                        "\n\n" + content;

            var client = new SmtpClient("mail.ultimatesoftware.com");

            Attachment attachment = new Attachment(@"C:\CurrentTests\TestResults.zip");
            message.Attachments.Add(attachment);

            client.Send(message);

            return 0;
        }

        public static int SaveResults(string product, string sourceDirectory, string targetDirectory)
        {
            try
            {
                string resultDirectory = string.Format(@"{0}\{1} Results {2}", targetDirectory, product, DateTime.Now.ToString("yyyyMMdd-hhmmss"));
                Directory.CreateDirectory(resultDirectory);

                DirectoryCopy(sourceDirectory, resultDirectory, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }

            return 0;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private static string testRunResult;
        public static int SaveTestRunResults(string testRunFile)
        {
            using (StreamReader reader = new StreamReader(testRunFile))
            {
                string startingWord = "Test Run Summary";
                string endingWord = "seconds";
                var contents = File.ReadAllText(testRunFile);
                var result = contents.Split(new string[] { startingWord }, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var line in result)
                {
                    int position = line.LastIndexOf(endingWord);
                    if (position > -1)
                    {
                        int end = position + endingWord.Length;
                        testRunResult = string.Format("{0}{1}", startingWord, line.Substring(0, end));
                    }
                }
            }

            File.WriteAllText(testRunFile, testRunResult);

            Console.WriteLine(testRunFile);

            return 0;
        }
    }
}
