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

namespace ZTMobile.Fragments
{
    public class TrackingScreen : Android.Support.V4.App.Fragment
    {
        private Button buttonTrackingTrace;
        private bool isTrackingEnabled;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.TrackingScreenLayout, container, false);

            buttonTrackingTrace = view.FindViewById<Button>(Resource.Id.buttonTrackTrace);
            isTrackingEnabled = false;

            buttonTrackingTrace.Click += ButtonTrackingTrace_Click;

            return view;
        }

        private void ButtonTrackingTrace_Click(object sender, EventArgs e)
        {
            if (isTrackingEnabled == false)
            {
                isTrackingEnabled = true;
            }
            else
            {
                isTrackingEnabled = false;
            }
        }
    }
}