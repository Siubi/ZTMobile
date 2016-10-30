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
using Android.Gms.Maps;

namespace ZTMobile.Fragments
{
    public class MapScreen : Android.Support.V4.App.Fragment, IOnMapReadyCallback
    {
        private MapView mapView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.MapScreenLayout, container, false);
            
            //SetUp Map
            if (FunctionsAndGlobals.googleMap == null)
            {
                mapView = (MapView)view.FindViewById(Resource.Id.map);
                mapView.OnCreate(savedInstanceState);
                mapView.OnResume();
                mapView.GetMapAsync(this);
            }

            return view;
        }

        public void OnMapReady(GoogleMap map)
        {
            FunctionsAndGlobals.googleMap = map;
        }
    }
}