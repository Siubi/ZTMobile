using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Threading;
using Android.Support.V4.App;

namespace ZTMobile.Fragments
{
    public class LoginScreen : Android.Support.V4.App.Fragment
    {
        private FragmentActivity myContext;
        private Button buttonSignUp;
        private Button buttonSignIn;
        private ProgressBar progressBar;
        private TextView txtGuestName;

        public event Action LoggedInSuccessfully;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Random rnd = new Random();
            View view = inflater.Inflate(Resource.Layout.LoginScreenLayout, container, false);

            txtGuestName = view.FindViewById<TextView>(Resource.Id.txtGuestName);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBarLoginScreen);
            buttonSignUp = view.FindViewById<Button>(Resource.Id.buttonSignUp);
            buttonSignIn = view.FindViewById<Button>(Resource.Id.buttonSignIn);

            //generate random ID for guest
            txtGuestName.Text = "Guest" + rnd.Next(10000, 99999).ToString();
            FunctionsAndGlobals.guestID = txtGuestName.Text;

            //We want to pop out new windows on clicks
            buttonSignUp.Click += ButtonSignUp_Click;
            buttonSignIn.Click += ButtonSignIn_Click;

            return view;
        }

        public override void OnAttach(Activity activity)
        {
            myContext = (FragmentActivity)activity;
            base.OnAttach(activity);
        }

        private void ButtonSignIn_Click(object sender, EventArgs e)
        {
            Android.App.FragmentTransaction transaction = myContext.FragmentManager.BeginTransaction();

            //Class 'Dialog_SignIn' contains code for window that will pop out
            Dialog_SignIn signInDialog = new Dialog_SignIn();
            signInDialog.Show(transaction, "SignIn fragment transaction");

            signInDialog.mOnSignInComplete += SignInDialog_mOnSignInComplete;
        }

        private void SignInDialog_mOnSignInComplete(object sender, OnSignInEventArgs e)
        {
            progressBar.Visibility = ViewStates.Visible;

            //Turn on progress bar
            Thread thread = new Thread(() => LoginRequest(e.Login, e.Password));
            thread.Start();
        }

        private void ButtonSignUp_Click(object sender, EventArgs e)
        {
            Android.App.FragmentTransaction transaction = myContext.FragmentManager.BeginTransaction();

            //Class 'Dialog_SignUp' contains code for window that will pop out
            Dialog_SignUp signUpDialog = new Dialog_SignUp();
            signUpDialog.Show(transaction, "SignUp fragment transaction");

            signUpDialog.mOnSignUpComplete += SignUpDialog_mOnSignUpComplete;
        }

        private void SignUpDialog_mOnSignUpComplete(object sender, OnSignUpEventArgs e)
        {
            progressBar.Visibility = ViewStates.Visible;

            //Turn on progress bar
            Thread thread = new Thread(() => SignUpRequest(e.Login, e.Email, e.Password));
            thread.Start();
        }

        public Boolean LoginFromFile()
        {
            List<string> userInfo;
            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string filePath = System.IO.Path.Combine(path, "ztmobile_user.txt");

            if (FunctionsAndGlobals.CheckIfFileExists(filePath))
            {
                var lines = System.IO.File.ReadAllLines(filePath);
                userInfo = new List<string>(lines);

                if (userInfo[1] == "0")
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
            FunctionsAndGlobals.userName = userInfo[0];
            FunctionsAndGlobals.isUserLoggedIn = true;

            return true;
        }

        private void LoginRequest(string login, string password)
        {
            if (FunctionsAndGlobals.LogInUserToDatabase(login, FunctionsAndGlobals.EncryptStringToMD5(password)) == true)
            {
                Activity.RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
                Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.signedIn, ToastLength.Short).Show(); });
                FunctionsAndGlobals.userName = login;
                FunctionsAndGlobals.isUserLoggedIn = true;

                FunctionsAndGlobals.EditUserFile(true);

                LoggedInSuccessfully();
            }
            else
            {
                Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.signInError, ToastLength.Short).Show(); });
            }

            Activity.RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
        }

        private void SignUpRequest(string login, string email, string password)
        {
            if (FunctionsAndGlobals.AddNewUserToDatabase(login, email, FunctionsAndGlobals.EncryptStringToMD5(password)) == true)
            {
                Activity.RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
                Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.signedUp, ToastLength.Short).Show(); });
            }
            else
            {
                Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.signUpError, ToastLength.Short).Show(); });
            }

            Activity.RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
        }
    }
}