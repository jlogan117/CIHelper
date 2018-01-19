using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace CIHelper.OnbConfig
{
    public static class AppConfig
    {
        private static string connectionString;
        public static int UpdateAppConfigValue(string rcloudName, string appconfigKey, string appconfigValue)
        {
            var onbPassword = "";
            //Ultipro
            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"http://deploy.newgen.corp/env/{rcloudName}");
                var responseString = response.Result.Content.ReadAsStringAsync().Result;
                dynamic envGetObject = JsonConvert.DeserializeObject(responseString);
                onbPassword = envGetObject.creds.sa;
            }
            connectionString = string.Format("server={0}odb;database=ONBRD_SB122;user id=sa;pwd={1}", rcloudName, onbPassword);
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(string.Format("UPDATE dbo.AppConfig SET Value = '{1}' WHERE [Key] = '{0}'", appconfigKey, appconfigValue), conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();

                return 0;
            }
        }
        public static int UpdateMasterConfig(string rcloudName, string appconfigKey, string appconfigValue)
        {
            var onbPassword = "";
            //Ultipro
            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"http://deploy.newgen.corp/env/{rcloudName}");
                var responseString = response.Result.Content.ReadAsStringAsync().Result;
                dynamic envGetObject = JsonConvert.DeserializeObject(responseString);
                onbPassword = envGetObject.creds.sa;
            }
            connectionString = string.Format("server={0}odb;database=ONBRD;user id=sa;pwd={1}", rcloudName, onbPassword);

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(string.Format("UPDATE dbo.MasterConfig SET Value = '{1}' WHERE [Key] = '{0}'", appconfigKey, appconfigValue), conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();

                return 0;
            }
        }
    }
}