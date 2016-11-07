package example.com.ztmobile;

/**
 * Created by Krzysiek on 2016-11-06.
 */

//FUNCTIONS THAT REQUIRES INTERNET CONNECTION ARE USABLE ONLY NEW THREAD

import android.os.AsyncTask;

import java.io.IOException;
import java.math.BigInteger;
import java.net.UnknownHostException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.sql.*;
import java.net.InetAddress;
import java.text.SimpleDateFormat;
import java.util.Date;
import org.apache.commons.net.ntp.NTPUDPClient;
import org.apache.commons.net.ntp.TimeInfo;

import com.google.android.gms.maps.GoogleMap;

public class FunctionsAndGlobals {
    public static GoogleMap googleMap;
    public static String userName = "";
    public static boolean isTrackingEnabled = false;
    public static boolean isUserLoggedIn = false;
    public static long doubleBackButtonClickInterval_ms = 2000;

    private static String connectionString = "SERVER=db4free.net;PORT=3306;DATABASE=ztmobile;UID=ztmobile;PWD=admin123;";
    private static Connection connection;
    private static Statement command;
    private static ResultSet dataResult;

    private static String GetCurrentDateFromInternet() {
        String TIME_SERVER = "time-a.nist.gov";
        NTPUDPClient timeClient = new NTPUDPClient();
        InetAddress inetAddress = null;
        try {
            inetAddress = InetAddress.getByName(TIME_SERVER);
        } catch (Exception e) {
            e.printStackTrace();
        }

        TimeInfo timeInfo = null;
        try {
            timeInfo = timeClient.getTime(inetAddress);
        } catch (IOException e) {
            e.printStackTrace();
        }

        long returnTime = timeInfo.getMessage().getTransmitTimeStamp().getTime();
        Date serverDate = new Date(returnTime);
        SimpleDateFormat formatter1 = new SimpleDateFormat("yyyy-MM-dd");

        return formatter1.format(serverDate.getDate()).toString();
    }

    public static String EncryptStringToMD5(String input) {
        MessageDigest m = null;

        try {
            m = MessageDigest.getInstance("MD5");
        } catch (NoSuchAlgorithmException e) {
            //e.printStackTrace();
        }

        m.reset();
        m.update(input.getBytes());
        byte[] digest = m.digest();
        BigInteger bigInt = new BigInteger(1,digest);
        String hashText = bigInt.toString(16);
        // Now we need to zero pad it if you actually want the full 32 chars.
        while(hashText.length() < 32 ){
            hashText = "0"+hashText;
        }

        return hashText;
    }

    //Password should be already encrypted by MD5
    public static Boolean AddNewUserToDatabase(String userName, String email, String password)
    {
        boolean result;

        try {
            connection = DriverManager.getConnection("SERVER=db4free.net;PORT=3306;DATABASE=ztmobile;UID=ztmobile;PWD=admin123;");
            command = connection.createStatement();
            dataResult = command.executeQuery("INSERT INTO Users(Login,Email,Password,LoggedIn) VALUES(" + userName + "," + email + "," + password + ",0)");
            result = true;
        } catch (Exception e) {
            e.printStackTrace();
            result = false;
        }
        finally
        {
            try {
                connection.close();
            } catch (SQLException e) {
                e.printStackTrace();
            }
        }

        return result;
    }

}
