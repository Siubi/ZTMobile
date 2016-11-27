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
        private EditText txtBusDriverID;
        private String lineName;
        private String dirName;
        private String emptyField = "Wybierz";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.TrackingScreenLayout, container, false);

            buttonTrackingTrace = view.FindViewById<Button>(Resource.Id.buttonTrackTrace);
            txtBusDriverID = view.FindViewById<EditText>(Resource.Id.txtEditBusDriverID);
            var txtBusDir = view.FindViewById<TextView>(Resource.Id.txtBusDir);
            txtBusDir.Visibility = ViewStates.Invisible;
            var spinnerDir = view.FindViewById<Spinner>(Resource.Id.spinnerBusDir);
            spinnerDir.Visibility = ViewStates.Invisible;

            txtBusDriverID.TextChanged += TxtBusDriverID_TextChanged;
            
            FunctionsAndGlobals.isTrackingEnabled = false;

            /*     txtBusNumber.KeyPress += (object sender, View.KeyEventArgs e) => {
                     e.Handled = false;
                     if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                     {
                         InputMethodManager manager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                         manager.HideSoftInputFromWindow(txtBusNumber.WindowToken, 0);
                         e.Handled = true;
                     }
                 };
                 */
            List<String> lines = FunctionsAndGlobals.GetAllLines();
            var adapter = new ArrayAdapter<String>(Activity.ApplicationContext, Android.Resource.Layout.SimpleSpinnerItem, lines);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            var spinner = view.FindViewById<Spinner>(Resource.Id.spinnerBusNumber);
            spinner.Adapter = adapter;
            spinner.ItemSelected += (sender, e) =>
            {
                var s = sender as Spinner;
                lineName = (String)s.GetItemAtPosition(e.Position);
                if (lineName != emptyField)
                {
                    List<String> dirs = FunctionsAndGlobals.getDirections((String)s.GetItemAtPosition(e.Position));
                    var adapterDirs = new ArrayAdapter<String>(Activity.ApplicationContext, Android.Resource.Layout.SimpleSpinnerItem, dirs);
                    adapterDirs.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    spinnerDir.Adapter = adapterDirs;
                    spinnerDir.Visibility = ViewStates.Visible;
                    txtBusDir.Visibility = ViewStates.Visible;
                    spinnerDir.ItemSelected += (sender2, e2) =>
                    {
                        var s2 = sender2 as Spinner;
                        dirName = (String)s2.GetItemAtPosition(e2.Position);
                    };
                }
                else
                {
                    txtBusDir.Visibility = ViewStates.Gone;
                    spinnerDir.Visibility = ViewStates.Gone;
                    dirName = emptyField;
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

        private void TxtBusDriverID_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            FunctionsAndGlobals.busDriverId = txtBusDriverID.Text;
        }

        private void ButtonTrackingTrace_Click(object sender, EventArgs e)
        {
            if (FunctionsAndGlobals.isTrackingEnabled == false)
            {
                if (lineName == emptyField)
                {
                    Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.emptyBusNumber, ToastLength.Short).Show(); });
                    return;
                }

                if (dirName == emptyField)
                {
                    Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.emptyBusDirection, ToastLength.Short).Show(); });
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
                
                //txtBusDriverID.Focusable = false;
                //txtBusDriverID.FocusableInTouchMode = false;
                //txtBusDriverID.Enabled = false;
                Thread thread = new Thread(() => FunctionsAndGlobals.SaveAndSendGPSDataToFile(lineName, dirName, txtBusDriverID.Text));
                thread.Start();
            }
            else
            {
                buttonTrackingTrace.Text = "Start";
                buttonTrackingTrace.SetBackgroundResource(Resource.Drawable.rounded_button);
                FunctionsAndGlobals.isTrackingEnabled = false;
                FunctionsAndGlobals.googleMap.MyLocationEnabled = false;
                FunctionsAndGlobals.googleMap.Clear();

                FunctionsAndGlobals.busDriverId = "";

                //txtBusDriverID.Focusable = true;
                //txtBusDriverID.FocusableInTouchMode = true;
                //txtBusDriverID.Enabled = true;

                FileSendConfirmAlert();
            }

        }

        private void FileSendConfirmAlert()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(Context);
            alert.SetTitle("PotwierdŸ poprawnoœæ danych");
            alert.SetMessage("Czy chcesz wys³aæ plik?");
            alert.SetPositiveButton("Tak", (senderAlert, args) => {
                FunctionsAndGlobals.sendFileOptions = (int)FunctionsAndGlobals.SendFileOptions.Send;
            });
            alert.SetNegativeButton("Nie", (senderAlert, args) => {
                FunctionsAndGlobals.sendFileOptions = (int)FunctionsAndGlobals.SendFileOptions.DoNotSend;
            });

            Dialog dialog = alert.Create();
            dialog.Show();
            dialog.CancelEvent += Dialog_CancelEvent;
        }

        private void Dialog_CancelEvent(object sender, EventArgs e)
        {
            FunctionsAndGlobals.sendFileOptions = (int)FunctionsAndGlobals.SendFileOptions.DoNotSend;
        }
    }
}