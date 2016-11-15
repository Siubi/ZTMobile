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
using Android.Locations;
using Android.Views.InputMethods;

namespace ZTMobile.Fragments
{
    public class TrackingScreen : Android.Support.V4.App.Fragment
    {
        private Button buttonTrackingTrace;
        private EditText txtBusNumber;
        private EditText txtBusDriverID;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.TrackingScreenLayout, container, false);

            buttonTrackingTrace = view.FindViewById<Button>(Resource.Id.buttonTrackTrace);
            txtBusNumber = view.FindViewById<EditText>(Resource.Id.txtEditBusNumber);
            txtBusDriverID = view.FindViewById<EditText>(Resource.Id.txtEditBusDriverID);
            FunctionsAndGlobals.isTrackingEnabled = false;

            txtBusNumber.KeyPress += (object sender, View.KeyEventArgs e) => {
                e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    InputMethodManager manager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                    manager.HideSoftInputFromWindow(txtBusNumber.WindowToken, 0);
                    e.Handled = true;
                }
            };

            txtBusDriverID.KeyPress += (object sender, View.KeyEventArgs e) => {
                e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    InputMethodManager manager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                    manager.HideSoftInputFromWindow(txtBusDriverID.WindowToken, 0);
                    e.Handled = true;
                }
            };

            buttonTrackingTrace.Click += ButtonTrackingTrace_Click;

            return view;
        }

        private void ButtonTrackingTrace_Click(object sender, EventArgs e)
        {
            if (FunctionsAndGlobals.isTrackingEnabled == false)
            {
                if (txtBusNumber.Text == "")
                {
                    Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.emptyBusNumber, ToastLength.Short).Show(); });
                    return;
                }
                //if (txtBusDriverID.Text == "")
                //{
                //    Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.emptyBusDriverID, ToastLength.Short).Show(); });
                //    return;
                //}

                LocationManager manager = (LocationManager)Activity.GetSystemService(Context.LocationService);
                if (!manager.IsProviderEnabled(LocationManager.GpsProvider))
                {
                    Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.gpsNotEnabled, ToastLength.Short).Show(); });
                    return;
                }

                    buttonTrackingTrace.Text = "Stop";
                buttonTrackingTrace.SetBackgroundResource(Resource.Drawable.rounded_button_stop);
                FunctionsAndGlobals.isTrackingEnabled = true;
                FunctionsAndGlobals.googleMap.MyLocationEnabled = true;

                txtBusNumber.Focusable = false;
                txtBusNumber.FocusableInTouchMode = false;
                txtBusNumber.Enabled = false;
                txtBusDriverID.Focusable = false;
                txtBusDriverID.FocusableInTouchMode = false;
                txtBusDriverID.Enabled = false;

                Thread thread = new Thread(() => FunctionsAndGlobals.SaveAndSendGPSDataToFile(txtBusNumber.Text, txtBusDriverID.Text));
                thread.Start();
            }
            else
            {
                buttonTrackingTrace.Text = "Start";
                buttonTrackingTrace.SetBackgroundResource(Resource.Drawable.rounded_button);
                FunctionsAndGlobals.isTrackingEnabled = false;
                FunctionsAndGlobals.googleMap.MyLocationEnabled = false;
                FunctionsAndGlobals.googleMap.Clear();

                txtBusNumber.Focusable = true;
                txtBusNumber.FocusableInTouchMode = true;
                txtBusNumber.Enabled = true;
                txtBusDriverID.Focusable = true;
                txtBusDriverID.FocusableInTouchMode = true;
                txtBusDriverID.Enabled = true;
            }
        }
    }
}