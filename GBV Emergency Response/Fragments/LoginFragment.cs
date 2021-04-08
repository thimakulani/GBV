using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Firebase.Auth;
using GBV_Emergency_Response.Activities;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace GBV_Emergency_Response.Fragments
{
    public class LoginFragment : HelpFragment, IOnFailureListener, IOnSuccessListener, IOnCompleteListener
    {
        private MaterialButton BtnLogin;
        private MaterialButton BtnSignup;
        private MaterialButton ForgotPassword;
        private TextInputEditText InputEmail;
        private TextInputEditText InputPassword;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        Context context;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.login, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {

            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.Input_Email);
            InputPassword = view.FindViewById<TextInputEditText>(Resource.Id.Input_Password);
            BtnLogin = view.FindViewById<MaterialButton>(Resource.Id.btnLogin);
            BtnSignup = view.FindViewById<MaterialButton>(Resource.Id.btnCreatAccount);
            ForgotPassword = view.FindViewById<MaterialButton>(Resource.Id.BtnForgotPassword);
            ForgotPassword.Click += ForgotPassword_Click;
            BtnLogin.Click += BtnLogin_Click;
            BtnSignup.Click += BtnSignup_Click;
        }
        public event EventHandler BtnSignUpClickEventHandler;
        private void BtnSignup_Click(object sender, EventArgs e)
        {
            BtnSignUpClickEventHandler(sender, e);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputEmail.Text) && string.IsNullOrWhiteSpace(InputEmail.Text))
            {
                InputEmail.Error = "Please provide your email";//, ToastLength.Long).Show();
                return;
            }
            if (string.IsNullOrEmpty(InputPassword.Text) && string.IsNullOrWhiteSpace(InputPassword.Text))
            {
                InputPassword.Error = "Please provide your password";
                return;
            }
            FirebaseAuth.Instance.SignInWithEmailAndPassword(InputEmail.Text.Trim(), InputPassword.Text.Trim())
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this)
                .AddOnCompleteListener(this);
            LoadingProgress();
        }
        private AlertDialog loading;
        private AlertDialog.Builder loadingBuilder;

        private void LoadingProgress()
        {
            loadingBuilder = new AlertDialog.Builder(context);
            LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.loading, null);


            loadingBuilder.SetView(view);
            loadingBuilder.SetCancelable(false);
            loading = loadingBuilder.Create();
            loading.Show();
        }
        private void ForgotPassword_Click(object sender, EventArgs e)
        {
        }

        public void OnComplete(Task task)
        {
            loading.Dismiss();
        }
        public event EventHandler SuccessEventHandler;
        
        public void OnSuccess(Java.Lang.Object result)
        {
            SuccessEventHandler.Invoke(this, null);
            
            //CloseActivity.Invoke()
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(context);
            alert.SetTitle("Error");
            alert.SetMessage(e.Message);
            alert.SetNeutralButton("OK", delegate
            {
                alert.Dispose();
            });
            alert.Show();
        }
    }
}