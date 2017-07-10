using System;
using System.Data.SqlClient;
using System.Threading;

namespace CIHelper
{
    public static class UOD
    {
        private const string _queuedStatus = "Queued";
        private const string _startedStatus = "Started";
        private const string _completedStatus = "Completed";
        private const string _errorStatus = "Error";
        private const string _terminatedStatus = "Terminated";
        
        public static int CheckIfTestsRunningonVm(string vmNodeName)
        {
            try
            {
                if (System.IO.File.Exists(string.Format(@"\\{0}\c$\CurrentTests\BuildApplied.txt", vmNodeName)))
                {
                    Console.WriteLine(string.Format("Test are currently running on {0} UOD is not allowed", vmNodeName));
                    return 1;
                }
                
                else
                {
                    Console.WriteLine(string.Format("Test are not currently running on {0} it is ok to UOD", vmNodeName));
                    return 0;
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }

        public static int CheckIfUODAlreadySubmitted(string environmentName)
        {
            try
            {
                string latestStatus = string.Empty;

                using (var conn = GetSqlConnection(@"devcinsql1\csi", "guest", "guest", "CSIBuilds"))
                {
                    string select = string.Format("SELECT dbo.LastDeploymentStatus('{0}')", environmentName);
                    conn.Open();

                    var cmd = new SqlCommand(select, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            reader.Read();
                            latestStatus = reader[0].ToString().Trim();

                            if (latestStatus == _startedStatus || latestStatus == _queuedStatus)
                            {
                                Console.WriteLine("UOD has already been submitted try again later");
                                return 1;
                            }
                        }

                    }
                }

                Console.WriteLine("UOD has not been submitted is ok to submit new request");
                return 0;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }

        public static int UODBuildAppliedLog(string logLocation)
        {
            try
            {
                System.IO.File.WriteAllText(string.Format(@"{0}\BuildApplied.txt", logLocation), string.Format("BUILD_VERSION={0}", GetLatestBuildAvailable()));
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }

            return 0;
        }

        public static int UODBuildAppliedIsLatest(string logLocation)
        {
            try
            {
                string buildApplied = System.IO.File.ReadAllText(string.Format(@"{0}\BuildApplied.txt", logLocation));

                string latestbuildAvailable = string.Format("BUILD_VERSION={0}", GetLatestBuildAvailable());

                if (buildApplied == latestbuildAvailable)
                {
                    Console.WriteLine(string.Format("Build Applied is {0}", buildApplied));
                    Console.WriteLine(string.Format("Latest Build is {0}", latestbuildAvailable));
                    Console.WriteLine("Latest Build already applied no need to UOD");
                    return 1;
                }

                else
                {
                    Console.WriteLine(string.Format("Build Applied is {0}", buildApplied));
                    Console.WriteLine(string.Format("Latest Build is {0}", latestbuildAvailable));
                    Console.WriteLine("Latest Build should be applied");
                    return 0;
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }
        
        private static string GetLatestBuildAvailable()
        {
            using (var conn = GetSqlConnection(@"devcinsql1\csi", "guest", "guest", "CSIBuilds"))
            {
                string select = string.Format("SELECT TOP 1 BuildNumber FROM dbo.BuildHist WHERE BuildCompleted = 'Y' ORDER BY BuildNumber DESC");
                conn.Open();
                var cmd = new SqlCommand(select, conn);

                var reader = cmd.ExecuteReader();

                if (reader != null)
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return reader["BuildNumber"].ToString().Trim();
                    }
            }

            throw new Exception("Issues Getting Latest Build Available");
        }

        public static int RequestUOD(string server)
        {
            try
            {
                string latestAvailableBuild = GetLatestBuildAvailable();
                RequestUod(server, Convert.ToInt32(latestAvailableBuild));
                string latestStatus = WaitForUodToComplete(server, latestAvailableBuild);

                if (latestStatus == _errorStatus || latestStatus == _terminatedStatus)
                {
                    Console.WriteLine("UOD Failed on first attempt. Will try one more UOD request");
                    RequestUod(server, Convert.ToInt32(latestAvailableBuild));
                    latestStatus = WaitForUodToComplete(server, latestAvailableBuild);                  

                    if (latestStatus == _errorStatus || latestStatus == _terminatedStatus)
                    {
                        Console.WriteLine("After second attempt UOD Failed with status of " + latestStatus);
                        return 1;
                    }

                    else
                    {
                        Console.WriteLine("After second attempt UOD was Successfully with status of " + latestStatus);
                        return 0;
                    }
                }

                if (latestStatus == _startedStatus)
                {
                    Console.WriteLine("UOD has not completed after 2.5 hours with status of " + latestStatus);
                    return 1;
                }

                else
                {
                    Console.WriteLine("UOD was Successfully with status of " + latestStatus);
                    return 0;
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }

        private static void RequestUod(string server, int buildNumber)
        {
            BuildService.BuildService buildService = new BuildService.BuildService();
            buildService.BuildRequest("devhit", "pa$$word", server, buildNumber);
        }

        private static string WaitForUodToComplete(string server, string buildNumber)
        {
            
            string latestStatus = string.Empty;
            using (var conn = GetSqlConnection(@"devcinsql1\csi", "guest", "guest", "CSIBuilds"))
            {
                string select = string.Format("SELECT dbo.LastDeploymentStatus('{0}')", server);
                conn.Open();

                var initialcmd = new SqlCommand(select, conn);
                Thread.Sleep(60000);
                using (var initialreader = initialcmd.ExecuteReader())
                {
                    initialreader.Read();
                    latestStatus = initialreader[0].ToString().Trim();
                }
                                               
                if (latestStatus == _queuedStatus)
                {
                    for (int i = 0; i < 240; i++) //loop for 4 hours for UOD to be initiated from Queued to Started
                    {
                        using (var initialreader = initialcmd.ExecuteReader())
                        {
                            initialreader.Read();
                            latestStatus = initialreader[0].ToString().Trim();
                        }                        
                                                
                        if (latestStatus == _startedStatus)
                            break;

                        Console.WriteLine("Status Is: " + latestStatus + " as of " + DateTime.Now.ToString("G") + " Will Continue to wait until UOD has Started");
                        
                        Thread.Sleep(60000);
                    }                                       
                }

                DateTime startTime = DateTime.Now;
                while (DateTime.Now.Subtract(startTime).TotalMinutes < 165) //loop for 2.65 hours
                {
                    var cmd = new SqlCommand(select, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            reader.Read();
                            latestStatus = reader[0].ToString().Trim();

                            if (latestStatus == _completedStatus || latestStatus == _errorStatus || latestStatus == _terminatedStatus)
                            {
                                return latestStatus;
                            }
                        }
                    }

                    Console.WriteLine("Status Is: " + latestStatus + " as of " + DateTime.Now.ToString("G") + " Waiting For UOD to apply build " + buildNumber);
                    Thread.Sleep(60000);
                }
            }

            return latestStatus;
        }

        private static SqlConnection GetSqlConnection(string sqlServer, string userName, string password, string initialCatalog)
        {
            SqlConnectionStringBuilder builder2 = new SqlConnectionStringBuilder();
            builder2.DataSource = sqlServer;
            builder2.InitialCatalog = initialCatalog;
            builder2.UserID = userName;
            builder2.Password = password;
            SqlConnectionStringBuilder builder = builder2;
            return new SqlConnection(builder.ConnectionString);
        }
    }
}
