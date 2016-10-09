using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;

namespace ZTMobile
{
    [Activity(Label = "ZTMobile", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button buttonSignUp;
        private Button buttonSignIn;
        private ProgressBar progressBar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBarLoginScreen);
            buttonSignUp = FindViewById<Button>(Resource.Id.buttonSignUp);
            buttonSignIn = FindViewById<Button>(Resource.Id.buttonSignIn);

            //We want to pop out new windows on clicks
            buttonSignUp.Click += ButtonSignUp_Click;
            buttonSignIn.Click += ButtonSignIn_Click;
        }

        private void ButtonSignIn_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();

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
            FragmentTransaction transaction = FragmentManager.BeginTransaction();

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

        private void LoginRequest(string login, string password)
        {
            if (FunctionsAndGlobals.LogInUserToDatabase(login, FunctionsAndGlobals.EncryptStringToMD5(password)) == true)
            {
                RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
                //TODO: Go to main application window after LogIn
            }
            else
            {
                //TODO: Give information about failure (pop out window or text on the bottom)
            }

            RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
        }

        private void SignUpRequest(string login, string email, string password)
        {
            if (FunctionsAndGlobals.AddNewUserToDatabase(login, email, FunctionsAndGlobals.EncryptStringToMD5(password)) == true)
            {
                RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
                //TODO: Pop out window with message about succesfull
            }
            else
            {
                //TODO: Pop out window with message about failure
            }

            RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
        }
    }
}

