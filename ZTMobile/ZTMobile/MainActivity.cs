using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using ZTMobile.Fragments;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;

namespace ZTMobile
{
    [Activity(Label = "ZTMobile", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/MainTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : ActionBarActivity
    {
        private SupportToolbar toolbar;
        private ActionBarDrawerToggle drawerToggle;
        private DrawerLayout drawerLayout;
        private ListView leftDrawer;
        private SupportFragment currentFragment;
        private TrackingScreen trackingScreenFragment;
        private LoginScreen loginScreenFragment;
        private MapScreen mapScreenFragment;
        private DateTime lastBackButtonClickTime;

        List<String> leftMenuItems = new List<String>();
        ArrayAdapter leftMenuArrayAdapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            lastBackButtonClickTime = DateTime.Now;

            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            leftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);

            leftMenuItems.Add("Śledzenie");
            leftMenuItems.Add("Konto");
            leftMenuItems.Add("Mapa");
            leftMenuArrayAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, leftMenuItems);
            leftDrawer.Adapter = leftMenuArrayAdapter;

            SetSupportActionBar(toolbar);

            trackingScreenFragment = new TrackingScreen();
            loginScreenFragment = new LoginScreen();
            mapScreenFragment = new MapScreen();

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Add(Resource.Id.fragmentContainer, mapScreenFragment, "Map Screen");
            transaction.Hide(loginScreenFragment);
            transaction.Add(Resource.Id.fragmentContainer, loginScreenFragment, "Login Screen");
            transaction.Hide(loginScreenFragment);
            transaction.Add(Resource.Id.fragmentContainer, trackingScreenFragment, "Tracking Screen");
            transaction.Commit();

            currentFragment = trackingScreenFragment;

            drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, Resource.String.openDrawer, Resource.String.closeDrawer);

            drawerLayout.SetDrawerListener(drawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            drawerToggle.SyncState();

            leftDrawer.ItemClick += LeftDrawer_ItemClick;
        }

        private void LeftDrawer_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Tracking Screen
            if (e.Position == 0)
            {
                ShowFragment(trackingScreenFragment);
            }
            //Login Screen
            if (e.Position == 1)
            {
                ShowFragment(loginScreenFragment);
            }
            //Map Screen
            if (e.Position == 2)
            {
                if (FunctionsAndGlobals.isTrackingEnabled == true)
                {
                    LatLng latLng = new LatLng(FunctionsAndGlobals.googleMap.MyLocation.Latitude, FunctionsAndGlobals.googleMap.MyLocation.Longitude);
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(latLng, 16);
                    FunctionsAndGlobals.googleMap.AnimateCamera(cameraUpdate);
                }

                ShowFragment(mapScreenFragment);
            }

            drawerLayout.CloseDrawer(Android.Support.V4.View.GravityCompat.Start);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            drawerToggle.OnOptionsItemSelected(item);
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            DateTime pressTime = DateTime.Now;
            if ((pressTime - lastBackButtonClickTime).TotalMilliseconds <= FunctionsAndGlobals.doubleBackButtonClickInterval_ms)
            {
                Java.Lang.JavaSystem.Exit(0);
            }

            Toast.MakeText(this, Resource.String.exitMessage, ToastLength.Short).Show();
            lastBackButtonClickTime = pressTime;
        }

        private void ShowFragment(SupportFragment fragment)
        {
            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Hide(currentFragment);
            transaction.Show(fragment);
            transaction.AddToBackStack(null);
            transaction.Commit();

            currentFragment = fragment;
        }
    }
}

