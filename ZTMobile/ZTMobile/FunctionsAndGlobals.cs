using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Data;
using MySql.Data.MySqlClient;
using System.Net;
using System.Globalization;
using Android.Gms.Maps;

namespace ZTMobile
{
    public class FunctionsAndGlobals
    {
        public static GoogleMap googleMap;
        public static string userName = "";
        public static bool isTrackingEnabled = false;
        public static bool isUserLoggedIn = false;
        public static long doubleBackButtonClickInterval_ms = 2000;

        public static string GetCurrentDateFromTheInternet()
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            var response = myHttpWebRequest.GetResponse();
            string todaysDates = response.Headers["date"];
            DateTime dateTime = DateTime.ParseExact(todaysDates, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal);

            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string EncryptStringToMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        //Password should be already encrypted by MD5
        public static Boolean AddNewUserToDatabase(string userName, string email, string password)
        {
            MySqlConnection connection = new MySqlConnection("SERVER=db4free.net;PORT=3306;DATABASE=ztmobile;UID=ztmobile;PWD=admin123;");
            MySqlCommand command;
            string query;
            Boolean result;

            try
            {
                connection.Open();
                query = "INSERT INTO Users(Login,Email,Password,LoggedIn) VALUES(@user,@email,@password,@loggedIn)";

                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@user", userName);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@loggedIn", "0");
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        //Password should be already encrypted by MD5
        public static Boolean LogInUserToDatabase(string userName, string password)
        {
            MySqlConnection connection = new MySqlConnection("SERVER=db4free.net;PORT=3306;DATABASE=ztmobile;UID=ztmobile;PWD=admin123;");
            MySqlCommand command;
            MySqlDataReader receivedResponse;
            string query;
            string receivedLogin;
            string receivedPassword;
            Boolean result;

            try
            {
                connection.Open();
                query = "SELECT * FROM Users WHERE Login='" + userName + "'";

                command = new MySqlCommand(query, connection);
                receivedResponse = command.ExecuteReader();

                if (receivedResponse.Read())
                {
                    receivedLogin = receivedResponse.GetString("Login");
                    receivedPassword = receivedResponse.GetString("Password");
                    receivedResponse.Close();

                    if (userName == receivedLogin && password == receivedPassword)
                    {
                        query = "UPDATE Users SET LoggedIn=1, LastLoginDate='" + GetCurrentDateFromTheInternet() + "' WHERE Login='" + userName + "'";
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    //User does not exist
                    result = false;
                }


            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        //Password should be already encrypted by MD5
        public static Boolean LogOutUserFromDatabase(string userName)
        {
            MySqlConnection connection = new MySqlConnection("SERVER=db4free.net;PORT=3306;DATABASE=ztmobile;UID=ztmobile;PWD=admin123;");
            MySqlCommand command;
            MySqlDataReader receivedResponse;
            string query;
            string receivedLogin;
            Boolean result;

            try
            {
                connection.Open();
                query = "SELECT * FROM Users WHERE Login='" + userName + "'";

                command = new MySqlCommand(query, connection);
                receivedResponse = command.ExecuteReader();

                if (receivedResponse.Read())
                {
                    receivedLogin = receivedResponse.GetString("Login");
                    receivedResponse.Close();

                    if (userName == receivedLogin)
                    {
                        query = "UPDATE Users SET LoggedIn=0 WHERE Login='" + userName + "'";
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    //User does not exist
                    result = false;
                }


            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                connection.Close();
            }

            return result;
        }
    }
}