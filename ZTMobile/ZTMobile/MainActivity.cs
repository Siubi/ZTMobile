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

namespace ZTMobile
{
    [Activity(Label = "ZTMobile", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/MainTheme")]
    public class MainActivity : ActionBarActivity
    {
        private SupportToolbar toolbar;
        private ActionBarDrawerToggle drawerToggle;
        private DrawerLayout drawerLayout;
        private ListView leftDrawer;
        private SupportFragment currentFragment;
        private TrackingScreen trackingScreenFragment;
        private LoginScreen loginScreenFragment;

        List<String> leftMenuItems = new List<String>();
        ArrayAdapter leftMenuArrayAdapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            leftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);

            leftMenuItems.Add("Śledzenie");
            leftMenuItems.Add("Konto");
            leftMenuArrayAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, leftMenuItems);
            leftDrawer.Adapter = leftMenuArrayAdapter;

            SetSupportActionBar(toolbar);

            trackingScreenFragment = new TrackingScreen();
            loginScreenFragment = new LoginScreen();

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Add(Resource.Id.fragmentContainer, loginScreenFragment, "Login Screen");
            transaction.Hide(loginScreenFragment);
            transaction.Add(Resource.Id.fragmentContainer, trackingScreenFragment, "Tracking Screen");
            transaction.Commit();

            currentFragment = trackingScreenFragment;

            drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, Resource.String.openDrawer, Resource.String.closeDrawer);

            drawerLayout.SetDrawerListener(drawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
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

            drawerLayout.CloseDrawer(Android.Support.V4.View.GravityCompat.Start);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            drawerToggle.OnOptionsItemSelected(item);
            return base.OnOptionsItemSelected(item);
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

