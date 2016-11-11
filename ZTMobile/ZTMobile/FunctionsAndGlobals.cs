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
using System.IO;
using System.Timers;
using System.Threading;

namespace ZTMobile
{
    public class FunctionsAndGlobals
    {
        enum DateFormats { Date, Time, DateAndTime }

        public static GoogleMap googleMap;
        public static string userName = "";
        public static string guestID = "";
        public static bool isTrackingEnabled = false;
        public static bool isUserLoggedIn = false;
        public static long doubleBackButtonClickInterval_ms = 2000;

        private static int timeIntervalBetweenGPSDataSaves = 1000;
        private static String fileNameWithGPSData;

        public static string GetCurrentDateFromTheInternet(int dateFormat)
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            var response = myHttpWebRequest.GetResponse();
            string todaysDates = response.Headers["date"];
            DateTime dateTime = DateTime.ParseExact(todaysDates, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal);

            if (dateFormat == (int)DateFormats.Date)
                return dateTime.ToString("yyyy-MM-dd");
            else if (dateFormat == (int)DateFormats.Time)
                return dateTime.ToString("HH:mm:ss");
            else if (dateFormat == (int)DateFormats.DateAndTime)
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            else
                return null;
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

        public static void WriteToFile(string fileName, string text)
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string filePath = System.IO.Path.Combine(path, fileName);
            StreamWriter stream = new StreamWriter(filePath, true);
            stream.WriteLine(text);
            stream.Close();
        }

        public static void SaveGPSDataToFile(string busNumber, string busDriverID)
        {
            String currentDateAndTime = GetCurrentDateFromTheInternet((int)DateFormats.DateAndTime);
            String header = "<Numer autobusu=" + busNumber + " Identyfikator kierowcy=" + busNumber + " Data=" + currentDateAndTime + "Interwa³ czasowy(ms)=" + timeIntervalBetweenGPSDataSaves.ToString() + ">";
            Handler h = new Handler(Looper.MainLooper);

            if (userName == "")
                fileNameWithGPSData = guestID + "=bus" + busNumber + "_" + currentDateAndTime.Replace(" ", "_") + ".txt";
            else
                fileNameWithGPSData = userName + "=bus" + busNumber + "_" + currentDateAndTime.Replace(" ", "_") + ".txt";

            try
            {
                WriteToFile(fileNameWithGPSData, header);
            }
            catch (Exception ex) { }
            
            while (isTrackingEnabled)
            {
                try
                {
                    Action action = new Action(() => { try { WriteToFile(fileNameWithGPSData, "Lat=" + googleMap.MyLocation.Latitude + " Lon" + googleMap.MyLocation.Longitude); } catch (Exception ex) { } });
                    h.Post(action);

                    //delay between next GPS data saves
                    System.Threading.Thread.Sleep(timeIntervalBetweenGPSDataSaves);
                }
                catch (Exception ex) { }
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
                        query = "UPDATE Users SET LoggedIn=1, LastLoginDate='" + GetCurrentDateFromTheInternet((int)DateFormats.Date) + "' WHERE Login='" + userName + "'";
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