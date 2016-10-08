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

            Thread thread = new Thread(ActLikeARequest);
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

            Thread thread = new Thread(ActLikeARequest);
            thread.Start();
        }

        private void ActLikeARequest()
        {
            Thread.Sleep(2500);
            RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
        }
    }
}

