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

namespace ZTMobile.Fragments
{
    public class AccountScreen : Android.Support.V4.App.Fragment
    {
        private ProgressBar progressBar;
        private Button buttonSignOut;
        private TextView txtUserName;

        public event Action LoggedOutSuccessfully;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.AccountScreenLayout, container, false);

            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBarLogOut);
            txtUserName = view.FindViewById<TextView>(Resource.Id.txtUserName);
            buttonSignOut = view.FindViewById<Button>(Resource.Id.buttonSignOut);

            buttonSignOut.Click += ButtonSignOut_Click;

            return view;
        }

        private void ButtonSignOut_Click(object sender, EventArgs e)
        {
            progressBar.Visibility = ViewStates.Visible;

            //Turn on progress bar
            Thread thread = new Thread(() => LogOutRequest());
            thread.Start();
        }

        private void LogOutRequest()
        {
            if (FunctionsAndGlobals.LogOutUserFromDatabase(FunctionsAndGlobals.userName) == true)
            {
                Activity.RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
                Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.signedOut, ToastLength.Short).Show(); });
                FunctionsAndGlobals.isUserLoggedIn = false;
                FunctionsAndGlobals.userName = "";
                LoggedOutSuccessfully();
            }
            else
            {
                Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.error, ToastLength.Short).Show(); });
            }

            Activity.RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
        }

        public void ChangeVisibleUserName(string userName)
        {
            txtUserName.SetText(userName, TextView.BufferType.Normal);
        }
    }
}