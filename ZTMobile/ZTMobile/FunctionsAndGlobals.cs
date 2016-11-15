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
using Android.Graphics;
using Java.IO;
using Android.Util;

namespace ZTMobile
{
    public class FunctionsAndGlobals
    {
        enum DateFormats { Date, Time, DateAndTime }

        public static TextView pointsTextOnActionBar;
        public static TextView pointsValueOnActionBar;
        public static GoogleMap googleMap;
        public static string userName = "";
        public static string guestID = "";
        public static int userPoints = 0;
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

        public static string GetDayOfWeek(DateTime date)
        {
            string dayOfWeek = "";

            switch(date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    dayOfWeek = "Poniedzia³ek";
                    break;
                case DayOfWeek.Tuesday:
                    dayOfWeek = "Wtorek";
                    break;
                case DayOfWeek.Wednesday:
                    dayOfWeek = "Œroda";
                    break;
                case DayOfWeek.Thursday:
                    dayOfWeek = "Czwartek";
                    break;
                case DayOfWeek.Friday:
                    dayOfWeek = "Pi¹tek";
                    break;
                case DayOfWeek.Saturday:
                    dayOfWeek = "Sobota";
                    break;
                case DayOfWeek.Sunday:
                    dayOfWeek = "Niedziela";
                    break;
                default:
                    break;
            }

            return dayOfWeek;
        }

        public static DateTime GetCurrentDateFromTheInternet()
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            var response = myHttpWebRequest.GetResponse();
            string todaysDates = response.Headers["date"];
            DateTime dateTime = DateTime.ParseExact(todaysDates, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal);
            
            return dateTime;
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

        public static Bitmap GetCircleBitmap(Bitmap bitmap)
        {
            bitmap = Bitmap.CreateScaledBitmap(bitmap, 360, 360, false);

            int width = bitmap.Width;
            int height = bitmap.Height;

            Bitmap output = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

            Canvas canvas = new Canvas(output);
            Paint paintColor = new Paint();
            paintColor.Flags = PaintFlags.AntiAlias;

            RectF rectF = new RectF(new Rect(0, 0, width, height));

            canvas.DrawRoundRect(rectF, width / 2, height / 2, paintColor);

            Paint paintImage = new Paint();
            paintImage.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcAtop));
            canvas.DrawBitmap(bitmap, 0, 0, paintImage);

            return output;
        }

        public static void WriteToFile(string fileName, string text)
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string filePath = System.IO.Path.Combine(path, fileName);
            StreamWriter stream = new StreamWriter(filePath, true);
            stream.WriteLine(text);
            stream.Close();
        }

        public static void SaveAndSendGPSDataToFile(string busNumber, string busDriverID)
        {
            int pointsInThisSession = 0;
            DateTime currentDateAndTime = GetCurrentDateFromTheInternet();
            String header = "<Numer autobusu=" + busNumber + " Identyfikator kierowcy=" + busDriverID + " Data=" + currentDateAndTime + " Interwa³ czasowy(ms)=" + timeIntervalBetweenGPSDataSaves.ToString() + ">";
            DateTime localTimeOfFirstSave = DateTime.Now;
            TimeSpan timeInterval;
            Handler h = new Handler(Looper.MainLooper);

            if (userName == "")
                fileNameWithGPSData = guestID + "=bus_" + busNumber + "_" + currentDateAndTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "_") + ".txt";
            else
                fileNameWithGPSData = userName + "=bus_" + busNumber + "_" + currentDateAndTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "_") + ".txt";

            try
            {
                WriteToFile(fileNameWithGPSData, header);
                WriteToFile(fileNameWithGPSData, "timestamp;lat;lon");
            }
            catch (Exception ex) { }
            
            while (isTrackingEnabled)
            {
                try
                {
                    timeInterval = DateTime.Now.Subtract(localTimeOfFirstSave);
                    Action action = new Action(() => { try { WriteToFile(fileNameWithGPSData, currentDateAndTime.AddSeconds(timeInterval.TotalSeconds).ToString("yyyy-MM-dd HH:mm:ss") + ";" + googleMap.MyLocation.Latitude + ";" + googleMap.MyLocation.Longitude); } catch (Exception ex) { } });
                    h.Post(action);

                    //delay between next GPS data saves
                    System.Threading.Thread.Sleep(timeIntervalBetweenGPSDataSaves);
                    pointsInThisSession += 1;

                    if (pointsInThisSession % 10 == 0 && userName != "")
                    {
                        Action postScore = new Action(() => { pointsValueOnActionBar.Text = (userPoints + pointsInThisSession).ToString(); });
                        h.Post(postScore);
                    }
                }
                catch (Exception ex) { }
            }

            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string filePath = System.IO.Path.Combine(path, fileNameWithGPSData);

            bool result = false;
            while (!result)
            {
                if (userName == "")
                    result = SendFileToDatabase(guestID, filePath, busNumber, busDriverID, currentDateAndTime.ToString("yyyy-MM-dd HH:mm:ss"), GetDayOfWeek(currentDateAndTime), currentDateAndTime.ToString("HH:mm:ss"), 0);
                else
                {
                    result = SendFileToDatabase(userName, filePath, busNumber, busDriverID, currentDateAndTime.ToString("yyyy-MM-dd HH:mm:ss"), GetDayOfWeek(currentDateAndTime), currentDateAndTime.ToString("HH:mm:ss"), pointsInThisSession);
                    userPoints += pointsInThisSession;
                    SetUserPointsInDatabase(userName, userPoints);
                    Action postScore = new Action(() => { pointsValueOnActionBar.Text = (userPoints).ToString(); });
                    h.Post(postScore);
                }
            }
        }

        public static Boolean SendFileToDatabase(string userName, string filePath, string busNumber, string busDriverID, string date, string dayOfWeek, string hour, int points)
        {
            MySqlConnection connection = new MySqlConnection("SERVER=s12.hekko.net.pl;PORT=3306;DATABASE=ztmobile_0;UID=ztmobile_0;PWD=admin123;");
            MySqlCommand command;
            string query;
            Boolean result;

            try
            {
                connection.Open();
                query = "INSERT INTO RouteTrackFiles(Login, File, Bus, BusDriverID, Date, DayOfWeek, Hour, Points) VALUES(@user,@file,@busNumber,@busDriverID,@date,@dayOfWeek,@hour,@points)";


                MemoryStream stream = new MemoryStream();
                byte[] byte_arr = System.IO.File.ReadAllBytes(filePath);
                String file_str = Base64.EncodeToString(byte_arr, Base64.Default);

                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@user", userName);
                command.Parameters.AddWithValue("@file", file_str);
                command.Parameters.AddWithValue("@busNumber", busNumber);
                command.Parameters.AddWithValue("@busDriverID", busDriverID);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
                command.Parameters.AddWithValue("@hour", hour);
                command.Parameters.AddWithValue("@points", points);
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
        public static Boolean AddNewUserToDatabase(string userName, string email, string password)
        {
            MySqlConnection connection = new MySqlConnection("SERVER=s12.hekko.net.pl;PORT=3306;DATABASE=ztmobile_0;UID=ztmobile_0;PWD=admin123;");
            MySqlCommand command;
            string query;
            Boolean result;

            try
            {
                connection.Open();
                query = "INSERT INTO Users(Login,Email,Password,LoggedIn,Points) VALUES(@user,@email,@password,@loggedIn,@points)";

                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@user", userName);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@loggedIn", "0");
                command.Parameters.AddWithValue("@points", "0");
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
            MySqlConnection connection = new MySqlConnection("SERVER=s12.hekko.net.pl;PORT=3306;DATABASE=ztmobile_0;UID=ztmobile_0;PWD=admin123;");
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
            MySqlConnection connection = new MySqlConnection("SERVER=s12.hekko.net.pl;PORT=3306;DATABASE=ztmobile_0;UID=ztmobile_0;PWD=admin123;");
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

        public static Boolean AddPhotoDatabase(string userName, Bitmap photo)
        {
            MySqlConnection connection = new MySqlConnection("SERVER=s12.hekko.net.pl;PORT=3306;DATABASE=ztmobile_0;UID=ztmobile_0;PWD=admin123;");
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
                        MemoryStream stream = new MemoryStream();
                        photo.Compress(Bitmap.CompressFormat.Png, 90, stream);
                        byte[] byte_arr = stream.ToArray();
                        String image_str = Base64.EncodeToString(byte_arr, Base64.Default);

                        query = "UPDATE Users SET Photo='" + image_str + "' WHERE Login='" + userName + "'";
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

        public static Bitmap GetUserPhotoFromDatabase(string userName)
        {
            MySqlConnection connection = new MySqlConnection("SERVER=s12.hekko.net.pl;PORT=3306;DATABASE=ztmobile_0;UID=ztmobile_0;PWD=admin123;");
            MySqlCommand command;
            MySqlDataReader receivedResponse;
            string query;
            string receivedLogin;
            string receivedPhotoBLOB;
            Bitmap bitmapPohoto = null;

            try
            {
                connection.Open();
                query = "SELECT * FROM Users WHERE Login='" + userName + "'";

                command = new MySqlCommand(query, connection);
                receivedResponse = command.ExecuteReader();

                if (receivedResponse.Read())
                {
                    receivedLogin = receivedResponse.GetString("Login");
                    receivedPhotoBLOB = receivedResponse.GetString("Photo");
                    receivedResponse.Close();

                    if (userName == receivedLogin && receivedPhotoBLOB != "")
                    {
                        byte[] data = Base64.Decode(receivedPhotoBLOB, Base64Flags.Default);
                        bitmapPohoto = BitmapFactory.DecodeByteArray(data, 0, data.Length, null);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return bitmapPohoto;
        }

        public static int GetUserPointsFromDatabase(string userName)
        {
            MySqlConnection connection = new MySqlConnection("SERVER=s12.hekko.net.pl;PORT=3306;DATABASE=ztmobile_0;UID=ztmobile_0;PWD=admin123;");
            MySqlCommand command;
            MySqlDataReader receivedResponse;
            string query;
            string receivedLogin;
            int receivedPoints;

            int result = 1;

            try
            {
                connection.Open();
                query = "SELECT * FROM Users WHERE Login='" + userName + "'";

                command = new MySqlCommand(query, connection);
                receivedResponse = command.ExecuteReader();

                if (receivedResponse.Read())
                {
                    receivedLogin = receivedResponse.GetString("Login");
                    receivedPoints = receivedResponse.GetInt32("Points");
                    receivedResponse.Close();

                    if (userName == receivedLogin)
                    {
                        result = receivedPoints;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public static string[,] GetTopScores()
        {
            MySqlConnection connection = new MySqlConnection("SERVER=s12.hekko.net.pl;PORT=3306;DATABASE=ztmobile_0;UID=ztmobile_0;PWD=admin123;");
            MySqlCommand command;
            MySqlDataReader receivedResponse;
            string query;
            bool nextResult = true;
            int counter = 0;

            string[,] data = new string[10, 2];

            for (int i = 0; i < 10; i++)
            {
                data[i, 0] = "";
                data[i, 1] = "";
            }

            try
            {
                connection.Open();
                query = "SELECT Login, Points FROM Users ORDER BY Points DESC LIMIT 10";

                command = new MySqlCommand(query, connection);
                receivedResponse = command.ExecuteReader();

                if (receivedResponse.Read())
                {
                    while (nextResult)
                    {
                        data[counter, 0] = receivedResponse.GetString("Login");
                        data[counter, 1] = receivedResponse.GetInt32("Points").ToString();
                        counter++;
                        nextResult = receivedResponse.Read();
                    }
                    receivedResponse.Close();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return data;
        }

        public static Boolean SetUserPointsInDatabase(string userName, int points)
        {
            MySqlConnection connection = new MySqlConnection("SERVER=s12.hekko.net.pl;PORT=3306;DATABASE=ztmobile_0;UID=ztmobile_0;PWD=admin123;");
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
                        query = "UPDATE Users SET Points='" + points.ToString() + "' WHERE Login='" + userName + "'";
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