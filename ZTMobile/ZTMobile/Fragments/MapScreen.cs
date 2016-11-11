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
using Android.Gms.Maps.Model;
using Android.Graphics;

namespace ZTMobile.Fragments
{
    public class MapScreen : Android.Support.V4.App.Fragment, IOnMapReadyCallback
    {
        private MapView mapView;
        private static LatLng previousPostion = new LatLng(-1, -1);

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
            FunctionsAndGlobals.googleMap.MyLocationChange += GoogleMap_MyLocationChange;
        }

        private void GoogleMap_MyLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            try
            {
                if (FunctionsAndGlobals.googleMap.MyLocation.Accuracy > 20)
                    return;

                PolylineOptions lineOptions = new PolylineOptions();
                LatLng newPosition = new LatLng(FunctionsAndGlobals.googleMap.MyLocation.Latitude, FunctionsAndGlobals.googleMap.MyLocation.Longitude);

                //'if' just to prevent from first draw
                if (previousPostion.Latitude != -1 && previousPostion.Longitude != -1)
                {
                    lineOptions.Add(previousPostion);
                    lineOptions.Add(newPosition);
                    lineOptions.InvokeWidth(5);
                    lineOptions.InvokeColor(Color.Blue);

                    FunctionsAndGlobals.googleMap.AddPolyline(lineOptions);
                }

                previousPostion.Latitude = newPosition.Latitude;
                previousPostion.Longitude = newPosition.Longitude;
            }
            catch(Exception ex) { }
        }
    }
}