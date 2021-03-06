﻿using System;
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
using Android.Graphics;
using System.IO;
using Android.Views.Animations;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using Plugin.Permissions;
using Android.Support.V4.Content;

namespace ZTMobile
{
    [Activity(Label = "ZTMobile", MainLauncher = true, Icon = "@drawable/ic_launcher", Theme = "@style/MainTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : ActionBarActivity, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        private SupportToolbar toolbar;
        private ActionBarDrawerToggle drawerToggle;
        private DrawerLayout drawerLayout;
        private ListView leftDrawer;
        private SupportFragment currentFragment;
        private TrackingScreen trackingScreenFragment;
        private LoginScreen loginScreenFragment;
        private MapScreen mapScreenFragment;
        private AccountScreen accountScreenFragment;
        private RankingScreen rankingScreenFragment;
        private DateTime lastBackButtonClickTime;
        private static bool pointOnActionBarLoaded = false;

        List<String> leftMenuItems = new List<String>();
        ArrayAdapter leftMenuArrayAdapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            lastBackButtonClickTime = DateTime.Now;

            CheckLocationPermission();
        }

        private void Initialize()
        {
            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            leftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);

            leftMenuItems.Add("Śledzenie");
            leftMenuItems.Add("Konto");
            leftMenuItems.Add("Ranking");
            leftMenuItems.Add("Mapa");
            leftMenuArrayAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, leftMenuItems);
            leftDrawer.Adapter = leftMenuArrayAdapter;

            SetSupportActionBar(toolbar);

            trackingScreenFragment = new TrackingScreen();
            loginScreenFragment = new LoginScreen();
            mapScreenFragment = new MapScreen();
            accountScreenFragment = new AccountScreen();
            rankingScreenFragment = new RankingScreen();

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Add(Resource.Id.fragmentContainer, accountScreenFragment, "Account Screen");
            transaction.Hide(accountScreenFragment);
            transaction.Add(Resource.Id.fragmentContainer, mapScreenFragment, "Map Screen");
            transaction.Hide(mapScreenFragment);
            transaction.Add(Resource.Id.fragmentContainer, rankingScreenFragment, "Ranking Screen");
            transaction.Hide(rankingScreenFragment);
            transaction.Add(Resource.Id.fragmentContainer, loginScreenFragment, "Login Screen");
            transaction.Hide(loginScreenFragment);
            transaction.Add(Resource.Id.fragmentContainer, trackingScreenFragment, "Tracking Screen");
            transaction.SetCustomAnimations(Android.Resource.Animation.FadeOut, Android.Resource.Animation.FadeIn);
            transaction.CommitAllowingStateLoss();

            currentFragment = trackingScreenFragment;

            drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, Resource.String.openDrawer, Resource.String.closeDrawer);

            drawerLayout.SetDrawerListener(drawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            drawerToggle.SyncState();

            leftDrawer.ItemClick += LeftDrawer_ItemClick;
            loginScreenFragment.LoggedInSuccessfully += LoginScreenFragment_LoggedInSuccessfully;
            accountScreenFragment.LoggedOutSuccessfully += AccountScreenFragment_LoggedOutSuccessfully;
            accountScreenFragment.ActivityCreatedSuccessfully += AccountScreenFragment_ActivityCreatedSuccessfully;
            FunctionsAndGlobals.sendingFileCompleted += FunctionsAndGlobals_sendingFileCompleted;
        }

        private void FunctionsAndGlobals_sendingFileCompleted()
        {
            this.RunOnUiThread(() => { Toast.MakeText(this, Resource.String.fileSentMessage, ToastLength.Short).Show(); });
        }

        private void AccountScreenFragment_ActivityCreatedSuccessfully()
        {
            Thread thread = new Thread(() =>
                {
                    if (loginScreenFragment.LoginFromFile())
                    {
                        LoginScreenFragment_LoggedInSuccessfully();
                    }
                }
            );
            thread.Start();
        }

        private void PointsValueOnActionBar_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            FunctionsAndGlobals.pointsValueOnActionBar.StartAnimation(AnimationUtils.LoadAnimation(this, Resource.Animation.abc_fade_in));

            if (FunctionsAndGlobals.pointsValueOnActionBar.Text != "")
                this.RunOnUiThread(() => { accountScreenFragment.ChangeVisiblePoints(Int32.Parse(FunctionsAndGlobals.pointsValueOnActionBar.Text)); });
        }

        private void LoginScreenFragment_LoggedInSuccessfully()
        {
            this.RunOnUiThread(() => { accountScreenFragment.ChangeVisibleUserName(FunctionsAndGlobals.userName); });

            if (currentFragment == loginScreenFragment)
            {
                ShowFragment(accountScreenFragment);
            }

            GetAndSetUserPoints();

            this.RunOnUiThread(() => { accountScreenFragment.SetProgressBarVisibilityState(true); });
            Bitmap bitmapImage = FunctionsAndGlobals.GetUserPhotoFromDatabase(FunctionsAndGlobals.userName);

            if (bitmapImage != null)
            {
                this.RunOnUiThread(() => { accountScreenFragment.ChangeVisibleUserPhoto(bitmapImage); });
            }
            this.RunOnUiThread(() => { accountScreenFragment.SetProgressBarVisibilityState(false); });
        }

        private void AccountScreenFragment_LoggedOutSuccessfully()
        {
            Thread thread = new Thread(() => ClearActionBar());
            thread.Start();

            if (currentFragment == accountScreenFragment)
            {
                ShowFragment(loginScreenFragment);
            }
        }

        public void ClearActionBar()
        {
            this.RunOnUiThread(() => { FunctionsAndGlobals.pointsTextOnActionBar.Text = ""; });
            this.RunOnUiThread(() => { FunctionsAndGlobals.pointsValueOnActionBar.Text = ""; });
        }

        public void GetAndSetUserPoints()
        {
            int result = -1;
            while (result == -1)
            {
                result = FunctionsAndGlobals.GetUserPointsFromDatabase(FunctionsAndGlobals.userName);
            }

            FunctionsAndGlobals.userPoints = result;

            while (!pointOnActionBarLoaded) { }

            this.RunOnUiThread(() => { FunctionsAndGlobals.pointsTextOnActionBar.Text = "Punkty:"; });
            this.RunOnUiThread(() => { FunctionsAndGlobals.pointsValueOnActionBar.Text = FunctionsAndGlobals.userPoints.ToString(); });

            this.RunOnUiThread(() => { accountScreenFragment.ChangeVisiblePoints(FunctionsAndGlobals.userPoints); });
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
                if (FunctionsAndGlobals.isUserLoggedIn == true)
                {
                    ShowFragment(accountScreenFragment);
                }
                else
                {
                    ShowFragment(loginScreenFragment);
                }
            }
            //Ranking Screen
            if (e.Position == 2)
            {
                ShowFragment(rankingScreenFragment);
            }
            //Map Screen
            if (e.Position == 3)
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            TextView pointsTextOnActionBar = new TextView(this);
            pointsTextOnActionBar.SetTextColor(Color.White);
            pointsTextOnActionBar.SetPadding(0, 0, 20, 0);
            pointsTextOnActionBar.SetTypeface(null, TypefaceStyle.Bold);
            pointsTextOnActionBar.SetTextSize(Android.Util.ComplexUnitType.Dip, 21);
            menu.Add(0, 1, 1, Resource.String.error).SetActionView(pointsTextOnActionBar).SetShowAsAction(ShowAsAction.Always);
            pointsTextOnActionBar.Text = "";
            FunctionsAndGlobals.pointsTextOnActionBar = pointsTextOnActionBar;

            TextView pointsValueOnActionBar = new TextView(this);
            pointsValueOnActionBar.SetTextColor(Color.White);
            pointsValueOnActionBar.SetPadding(0, 0, 20, 0);
            pointsValueOnActionBar.SetTypeface(null, TypefaceStyle.Bold);
            pointsValueOnActionBar.SetTextSize(Android.Util.ComplexUnitType.Dip, 21);
            menu.Add(0, 1, 2, Resource.String.error).SetActionView(pointsValueOnActionBar).SetShowAsAction(ShowAsAction.Always);
            pointsValueOnActionBar.Text = "";
            FunctionsAndGlobals.pointsValueOnActionBar = pointsValueOnActionBar;

            FunctionsAndGlobals.pointsValueOnActionBar.TextChanged += PointsValueOnActionBar_TextChanged;

            pointOnActionBarLoaded = true;
            return base.OnCreateOptionsMenu(menu);
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
            transaction.SetCustomAnimations(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
            transaction.Show(fragment);
            transaction.AddToBackStack(null);
            transaction.Commit();

            currentFragment = fragment;
        }

        private void CheckLocationPermission()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Android.Content.PM.Permission.Granted)
            {
                Initialize();
                return;
            }

            ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessFineLocation }, 1);
        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case 1:
                    {
                        // If request is cancelled, the result arrays are empty.
                        if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        {
                            Initialize();
                            return;
                        }
                        else
                        {
                            Java.Lang.JavaSystem.Exit(0);
                        }
                        break;
                    }
            }
        }
    }
}

