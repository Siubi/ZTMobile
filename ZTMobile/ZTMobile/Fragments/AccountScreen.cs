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
using Android.Graphics;
using Android.Provider;

namespace ZTMobile.Fragments
{
    public class AccountScreen : Android.Support.V4.App.Fragment
    {
        private ProgressBar progressBar;
        private Button buttonSignOut;
        private TextView txtUserName;
        private TextView txtPoints;
        private ImageButton buttonUserImage;

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
            txtPoints = view.FindViewById<TextView>(Resource.Id.txtPoints);
            buttonSignOut = view.FindViewById<Button>(Resource.Id.buttonSignOut);
            buttonUserImage = view.FindViewById<ImageButton>(Resource.Id.buttonUserImage);

            buttonUserImage.SetImageBitmap(BitmapFactory.DecodeResource(Resources, Resource.Drawable.plus));

            buttonSignOut.Click += ButtonSignOut_Click;

            buttonUserImage.Click += delegate {

                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select photo"), 0);
            };

            return view;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == (int)Result.Ok)
            {
                Activity.RunOnUiThread(() => { progressBar.Visibility = ViewStates.Visible; });

                var imageButton = this.View.FindViewById<ImageButton>(Resource.Id.buttonUserImage);
                Android.Net.Uri imageUri = (Android.Net.Uri)data.Data;
                Bitmap bitmap = MediaStore.Images.Media.GetBitmap(Context.ContentResolver, imageUri);
                bitmap = FunctionsAndGlobals.GetCircleBitmap(bitmap);

                if (FunctionsAndGlobals.AddPhotoDatabase(FunctionsAndGlobals.userName, bitmap))
                {
                    Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.photoAddedSuccesfully, ToastLength.Short).Show(); });
                }

                imageButton.SetImageBitmap(bitmap);
                Activity.RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
            }
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
                FunctionsAndGlobals.userPoints = 0;
                Activity.RunOnUiThread(() => { buttonUserImage.SetImageBitmap(BitmapFactory.DecodeResource(Resources, Resource.Drawable.plus)); });
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

        public void ChangeVisiblePoints(int points)
        {
            string text;
            text = "Punkty: " + points.ToString();
            txtPoints.SetText(text, TextView.BufferType.Normal);
        }

        public void ChangeVisibleUserPhoto(Bitmap bitmapImage)
        {
            buttonUserImage.SetImageBitmap(bitmapImage);
        }

        public void SetProgressBarVisibilityState(bool state)
        {
            if (state == true)
                progressBar.Visibility = ViewStates.Visible;
            else
                progressBar.Visibility = ViewStates.Invisible;
        }
    }
}