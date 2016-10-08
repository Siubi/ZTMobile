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

namespace ZTMobile
{
    public class OnSignInEventArgs : EventArgs
    {
        private string txtLogin;
        private string txtPassword;

        public string Login
        {
            get { return txtLogin; }
            set { txtLogin = value; }
        }
        public string Password
        {
            get { return txtPassword; }
            set { txtPassword = value; }
        }

        public OnSignInEventArgs(string login, string password) : base()
        {
            Login = login;
            Password = password;
        }
    }

    class Dialog_SignIn : DialogFragment
    {
        private EditText txtLogin;
        private EditText txtPassword;
        private Button buttonSignIn;

        public event EventHandler<OnSignInEventArgs> mOnSignInComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.dialog_SignIn, container, false);

            txtLogin = view.FindViewById<EditText>(Resource.Id.txtLoginSignIn);
            txtPassword = view.FindViewById<EditText>(Resource.Id.txtPasswordSignIn);
            buttonSignIn = view.FindViewById<Button>(Resource.Id.buttonSignInInDialog);

            buttonSignIn.Click += ButtonSignIn_Click;

            return view;
        }

        private void ButtonSignIn_Click(object sender, EventArgs e)
        {
            mOnSignInComplete.Invoke(this, new OnSignInEventArgs(txtLogin.Text, txtPassword.Text));
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            //Sets the title bar to invisible
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);

            base.OnActivityCreated(savedInstanceState);
        }
    }
}