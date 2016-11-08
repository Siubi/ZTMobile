using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Support.V7.App;
using Android.Support.V4.Widget;

namespace ZTMobile
{
    public class ActionBarDrawerToggle : SupportActionBarDrawerToggle
    {
        private ActionBarActivity hostActivity;
        private int openedSource;
        private int closedSource;
    
        public ActionBarDrawerToggle (ActionBarActivity host, DrawerLayout drawerLayout, int openedSource, int closedSource)
            : base(host, drawerLayout, openedSource, closedSource)
        {
            this.hostActivity = host;
            this.openedSource = openedSource;
            this.closedSource = closedSource;
        }

        public override void OnDrawerOpened(View drawerView)
        {
            base.OnDrawerOpened(drawerView);
            //hostActivity.SupportActionBar.SetTitle(openedSource);
        }

        public override void OnDrawerClosed(View drawerView)
        {
            base.OnDrawerClosed(drawerView);
            //hostActivity.SupportActionBar.SetTitle(closedSource);
        }

        public override void OnDrawerSlide(View drawerView, float slideOffset)
        {
            base.OnDrawerSlide(drawerView, slideOffset);
        }
    }
}