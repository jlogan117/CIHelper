using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Xml;

namespace CIHelper
{
    public static class ChangeEnvironment
    {
        public static int ChangeEchoEnvironment(string dllPath, string environment)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(dllPath);

                XmlNodeList userNodes = doc.GetElementsByTagName("add");

                foreach (XmlNode userNode in userNodes)
                {
                    if (userNode.Attributes["key"].Value == "EnvironmentName")
                    {
                        userNode.Attributes["value"].Value = environment;
                        break;
                    }
                }

                doc.Save(dllPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }

            return 0;
        }

        public static int ChangeEchoEnvironmentXml(string environmentXmlPath, string rcloudname = "%UpCloudName%", string uodenvnumber = "%UodEnvNumber%", string onbcloudname = "%OnbCloudName%")
        {
            try
            {
                if (rcloudname != null)
                {
                    Console.WriteLine(rcloudname);
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(environmentXmlPath);

                    var newdoc = xDoc.InnerXml.Replace("%UpCloudName%", rcloudname);
                    
                    xDoc.InnerXml = newdoc;
                    xDoc.Save(environmentXmlPath);          
                }

                if (rcloudname != null)
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(environmentXmlPath);

                    var newdoc = xDoc.InnerXml.Replace("%UodEnvNumber%", uodenvnumber);

                    xDoc.InnerXml = newdoc;
                    xDoc.Save(environmentXmlPath);
                }

                if (onbcloudname != null)
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(environmentXmlPath);

                    var newdoc = xDoc.InnerXml.Replace("%OnbCloudName%", onbcloudname);

                    xDoc.InnerXml = newdoc;
                    xDoc.Save(environmentXmlPath);
                }
                //var upPassword = "";
                ////Ultipro
                //if (rcloudname != null)
                //{
                //    using (var client = new HttpClient())
                //    {
                //        var response = client.GetAsync($"http://deploy.newgen.corp/env/{rcloudname}");
                //        var responseString = response.Result.Content.ReadAsStringAsync().Result;
                //        dynamic envGetObject = JsonConvert.DeserializeObject(responseString);
                //        upPassword = envGetObject.creds.sa;
                //    }
                //}
                //var onbPassword = "";
                ////Ultipro
                //if (onbcloudname != null)
                //{
                //    using (var client = new HttpClient())
                //    {
                //        var response = client.GetAsync($"http://deploy.newgen.corp/env/{onbcloudname}");
                //        var responseString = response.Result.Content.ReadAsStringAsync().Result;
                //        dynamic envGetObject = JsonConvert.DeserializeObject(responseString);
                //        onbPassword = envGetObject.creds.sa;
                //    }
                //}

                //XmlDocument doc = new XmlDocument();
                //if (rcloudname != null)
                //{
                //    doc.Load(environmentXmlPath);
                //    XmlNodeList userNodes = doc.GetElementsByTagName("UltiPro");
                //    Console.WriteLine(userNodes.Count);
                //    Console.WriteLine(userNodes.ToString());
                //    foreach (XmlNode userNode in userNodes)
                //    {
                //        var test = userNode.FirstChild;
                //        while (test != null)
                //        {
                //            if (test.Attributes["Password"] != null)
                //            {
                //                test.Attributes["Password"].Value = upPassword;
                //                //break;
                //            }
                //            test = test.NextSibling;
                //        }
                //    }

                //    doc.Save(environmentXmlPath);
                //}

                //if (onbcloudname != null)
                //{
                //    doc.Load(environmentXmlPath);
                //    XmlNodeList userNodesTwo = doc.GetElementsByTagName("Onboarding");

                //    foreach (XmlNode userNode in userNodesTwo)
                //    {
                //        var test = userNode.FirstChild;
                //        while (test != null)
                //        {
                //            if (test.Attributes != null && test.Attributes.Count > 2 && test.Attributes["Password"] != null)
                //            {
                //                test.Attributes["Password"].Value = onbPassword;
                //                //break;
                //            }
                //            test = test.NextSibling;
                //            Console.WriteLine(test);
                //        }
                //    }

                //    doc.Save(environmentXmlPath);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }

            return 0;
        }

        public static string GetEchoParametersXml()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@"C:\Projects\UltiPro.NET\AutomatedTests\Echo\Echo.Parameters.xml");

            XmlNodeList userNodes = xdoc.GetElementsByTagName("Configuration");

            foreach (XmlNode userNode in userNodes)
            {
                if (userNode.Attributes["Name"].Value == ".NET")
                {
                    return userNode.FirstChild.FirstChild.InnerText;
                }
            }
            
            return ".NET was not found in App.config";
        }

        public static int ChangeEchoParametersXml(string pipeline, bool rotate = false, string parametersXmlPath = @"C:\Projects\UltiPro.NET\AutomatedTests\Echo\Echo.Parameters.xml", string specifiedBrowser = null)
        {
            string browsername = null;
            if(specifiedBrowser != null)
            {
                browsername = specifiedBrowser;
            }
            if (rotate && browsername == null)
            {
                browsername = ApiStatus.getLastRunBrowser(pipeline);

                switch (browsername.ToLower())
                {
                    case "chrome": browsername = "InternetExplorer";
                        break;
                    case "internetexplorer": browsername = "Chrome";
                        break;
                    //case "edge": browsername = "Chrome";
                    //    break;

                    default: browsername = "Chrome";
                        break;
                }              
            }
            else if(browsername == null)
            {
                browsername = "Chrome";
            }

            try
            {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.Load(parametersXmlPath);

                    XmlNodeList userNodes = xdoc.GetElementsByTagName("Configuration");

                    foreach (XmlNode userNode in userNodes)
                    {
                        if (userNode.Attributes["Name"].Value == ".NET")
                        {
                            userNode.FirstChild.FirstChild.InnerText = browsername;
                            break;
                        }
                    }

                    xdoc.Save(parametersXmlPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }

            return 0;
        }

        public static int ChangeEchoParametersXml(string parametersXmlPath = @"C:\Projects\UltiPro.NET\AutomatedTests\Echo\Echo.Parameters.xml", string browsername = "Chrome")
        {
            try
            {
                if (browsername != null)
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.Load(parametersXmlPath);

                    XmlNodeList userNodes = xdoc.GetElementsByTagName("Configuration");

                    foreach (XmlNode userNode in userNodes)
                    {
                        if (userNode.Attributes["Name"].Value == ".NET")
                        {
                            userNode.FirstChild.FirstChild.InnerText = browsername;
                            break;
                        }
                    }

                    xdoc.Save(parametersXmlPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }

            return 0;
        }

        public static int ChangeProjectFrameworkVersion(string projectFilePath, string projectFileNamespace = "http://schemas.microsoft.com/developer/msbuild/2003", string dotnetversion = "v4.6.1")
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(projectFilePath);

                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
                namespaceManager.AddNamespace("ns0", projectFileNamespace);

                doc.SelectSingleNode("ns0:Project/ns0:PropertyGroup/ns0:TargetFrameworkVersion", namespaceManager).InnerText = dotnetversion;

                doc.Save(projectFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }

            return 0;
        }
    }
}
