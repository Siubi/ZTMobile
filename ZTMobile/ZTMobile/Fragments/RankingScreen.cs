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
using System.Timers;

namespace ZTMobile.Fragments
{
    public class RankingScreen : Android.Support.V4.App.Fragment
    {
        private List<TextView> txtUsers = new List<TextView>();
        private string[,] usersAndPoints = new string[10, 2];

        public event Action LoggedOutSuccessfully;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.RankingScreenLayout, container, false);

            txtUsers.Add(view.FindViewById<TextView>(Resource.Id.txtUser1));
            txtUsers.Add(view.FindViewById<TextView>(Resource.Id.txtUser2));
            txtUsers.Add(view.FindViewById<TextView>(Resource.Id.txtUser3));
            txtUsers.Add(view.FindViewById<TextView>(Resource.Id.txtUser4));
            txtUsers.Add(view.FindViewById<TextView>(Resource.Id.txtUser5));
            txtUsers.Add(view.FindViewById<TextView>(Resource.Id.txtUser6));
            txtUsers.Add(view.FindViewById<TextView>(Resource.Id.txtUser7));
            txtUsers.Add(view.FindViewById<TextView>(Resource.Id.txtUser8));
            txtUsers.Add(view.FindViewById<TextView>(Resource.Id.txtUser9));
            txtUsers.Add(view.FindViewById<TextView>(Resource.Id.txtUser10));
            
            UpdateTopScoresTable(this, null);

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(UpdateTopScoresTable);
            timer.Interval = 60000;
            timer.Enabled = true;

            return view;
        }

        private void UpdateTopScoresTable(object source, ElapsedEventArgs e)
        {
            usersAndPoints = FunctionsAndGlobals.GetTopScores();

            Activity.RunOnUiThread(() => {
                for (int i = 0; i<10; i++)
                {
                    if (usersAndPoints[i, 0] != "")
                        txtUsers[i].Text = (i + 1).ToString() + ". " + usersAndPoints[i, 0] + ": " + usersAndPoints[i, 1];
                    else
                        txtUsers[i].Text = (i + 1).ToString() + ".";
                }
            });
        }
    }
}