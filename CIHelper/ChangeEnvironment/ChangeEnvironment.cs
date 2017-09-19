using System;
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
                if (rcloudname!= null)
                {
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

                if (rcloudname != null)
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(environmentXmlPath);

                    var newdoc = xDoc.InnerXml.Replace("%OnbCloudName%", onbcloudname);

                    xDoc.InnerXml = newdoc;
                    xDoc.Save(environmentXmlPath);
                }
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
            xdoc.Load(@"C:\Projects\UltiPro.NET\AutomatedTests\Echo\lib\Echo.Parameters.xml");

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

        public static int ChangeEchoParametersXml(string pipeline, bool rotate = false, string parametersXmlPath = @"C:\Projects\UltiPro.NET\AutomatedTests\Echo\lib\Echo.Parameters.xml")
        {
            string browsername = null;
            if (rotate)
            {
                browsername = ApiStatus.getLastRunBrowser(pipeline);

                switch (browsername.ToLower())
                {
                    case "chrome": browsername = "InternetExplorer";
                        break;
                    case "internetexplorer": browsername = "Firefox";
                        break;
                    case "firefox": browsername = "Chrome";
                        break;

                    default: browsername = "Chrome";
                        break;
                }
                
            }
            else
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

        public static int ChangeEchoParametersXml(string parametersXmlPath = @"C:\Projects\UltiPro.NET\AutomatedTests\Echo\lib\Echo.Parameters.xml", string browsername = "Chrome")
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
