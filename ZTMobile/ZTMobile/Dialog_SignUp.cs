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
    public class OnSignUpEventArgs : EventArgs
    {
        private string txtLogin;
        private string txtEmail;
        private string txtPassword;
        private string txtPasswordConfirm;

        public string Login
        {
            get { return txtLogin; }
            set { txtLogin = value; }
        }
        public string Email
        {
            get { return txtEmail; }
            set { txtEmail = value; }
        }
        public string Password
        {
            get { return txtPassword; }
            set { txtPassword = value; }
        }
        public string PasswordConfirm
        {
            get { return txtPasswordConfirm; }
            set { txtPasswordConfirm = value; }
        }

        public OnSignUpEventArgs (string login, string email, string password, string passwordConfirm) : base()
        {
            Login = login;
            Email = email;
            Password = password;
            PasswordConfirm = passwordConfirm;
        }
    }

    class Dialog_SignUp : DialogFragment
    {
        private EditText txtLogin;
        private EditText txtEmail;
        private EditText txtPassword;
        private EditText txtPasswordConfirm;
        private Button buttonSignUp;

        public event EventHandler<OnSignUpEventArgs> mOnSignUpComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.dialog_SignUp, container, false);

            txtLogin = view.FindViewById<EditText>(Resource.Id.txtLoginSignUp);
            txtEmail = view.FindViewById<EditText>(Resource.Id.txtEmailSignUp);
            txtPassword = view.FindViewById<EditText>(Resource.Id.txtPasswordSignUp);
            txtPasswordConfirm = view.FindViewById<EditText>(Resource.Id.txtPasswordConfirmSignUp);
            buttonSignUp = view.FindViewById<Button>(Resource.Id.buttonSignUpInDialog);

            buttonSignUp.Click += ButtonSignUp_Click;

            return view;
        }

        private void ButtonSignUp_Click(object sender, EventArgs e)
        {
            if (txtLogin.Length() > 10)
            {
                Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.loginTooLong, ToastLength.Short).Show(); });
                return;
            }
            if (txtPassword.Length() < 6)
            {
                Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.passwordTooShort, ToastLength.Short).Show(); });
                return;
            }
            if (txtPassword.Text != txtPasswordConfirm.Text)
            {
                Activity.RunOnUiThread(() => { Toast.MakeText(Activity.ApplicationContext, Resource.String.passwordsMismatch, ToastLength.Short).Show(); });
                return;
            }

            mOnSignUpComplete.Invoke(this, new OnSignUpEventArgs(txtLogin.Text, txtEmail.Text, txtPassword.Text, txtPasswordConfirm.Text));
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