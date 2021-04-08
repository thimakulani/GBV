﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Firebase.Auth;
using Firebase.Database;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Java.Util;

namespace GBV_Emergency_Response.Fragments
{
    public class SignupFragment : HelpFragment, IOnFailureListener, IOnSuccessListener, IOnCompleteListener
    {

        private MaterialButton BtnRegister;
        private MaterialButton BtnLogin;
        private TextInputEditText InputName;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhoneNumber;
        private TextInputEditText InputPassword;
        private TextInputEditText InputPassword2;
        private TextInputEditText InputEmail;
        Context context;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.sign_up, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            InputName = view.FindViewById<TextInputEditText>(Resource.Id.InputFirstName);
            InputPassword = view.FindViewById<TextInputEditText>(Resource.Id.InputPassword);
            InputPassword2 = view.FindViewById<TextInputEditText>(Resource.Id.InputConfirmPassword);
            InputPhoneNumber = view.FindViewById<TextInputEditText>(Resource.Id.InputPhoneNumber);
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.InputEmail);
            InputSurname = view.FindViewById<TextInputEditText>(Resource.Id.InputLastName);
            BtnRegister = view.FindViewById<MaterialButton>(Resource.Id.btnRegister);
            BtnLogin = view.FindViewById<MaterialButton>(Resource.Id.BtnBackToLogin);
            BtnRegister.Click += BtnRegister_Click;
            BtnLogin.Click += BtnLogin_Click;
        }
        private FirebaseAuth auth;
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputName.Text) && string.IsNullOrWhiteSpace(InputName.Text))
            {
                InputName.Error = "Please provide your name";
                return;
            }
            if (string.IsNullOrEmpty(InputSurname.Text) && string.IsNullOrWhiteSpace(InputSurname.Text))
            {
                InputSurname.Error = "Please provide your surname";
                return;
            }
            
            if (string.IsNullOrEmpty(InputEmail.Text) && string.IsNullOrWhiteSpace(InputEmail.Text))
            {
                InputEmail.Error = "Please provide your email";//, ToastLength.Long).Show();
                return;
            }
            if (string.IsNullOrEmpty(InputPhoneNumber.Text) && string.IsNullOrWhiteSpace(InputPhoneNumber.Text))
            {
                InputPhoneNumber.Error = "Please provide your phone number";//, ToastLength.Long).Show();
                return;
            }
            if (string.IsNullOrEmpty(InputPassword.Text) && string.IsNullOrWhiteSpace(InputPassword.Text))
            {
                InputPassword.Error = "Please provide your password";
                return;
            }
            if (InputPassword.Text != InputPassword2.Text)
            {
                InputPassword.Error = "Password does not match";
                return;
            }
            auth = FirebaseAuth.Instance;
            auth.CreateUserWithEmailAndPassword(InputEmail.Text.Trim(), InputPassword.Text.Trim())
                .AddOnFailureListener(this)
                .AddOnSuccessListener(this)
                .AddOnCompleteListener(this);
            LoadingProgress();
        }

        public event EventHandler LoginHandler;
      

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            LoginHandler(sender, e);
        }
        //loading progress dialog
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
        public void OnFailure(Java.Lang.Exception e)
        {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(context);
            alert.SetTitle("Error");
            alert.SetMessage(e.Message);
            alert.SetNeutralButton("OK", delegate
            {
                alert.Dispose();
            });
            alert.Show();
        }
       
        public void OnSuccess(Java.Lang.Object result)
        {
            HashMap data = new HashMap();
            data.Put("Username", InputName.Text);
            data.Put("Name", InputName.Text);
            data.Put("Surname", InputSurname.Text);
            data.Put("PhoneNumber", InputPhoneNumber.Text);
            data.Put("Email", InputEmail.Text);
            var dbRef = FirebaseDatabase.Instance.GetReference("Users").Child(auth.CurrentUser.Uid);

            dbRef.SetValue(data);
           
            AndHUD.Shared.ShowSuccess(context, "You have successfully created your account", MaskType.Black, TimeSpan.FromSeconds(2));
            BtnLogin.PerformClick();
        }

        public void OnComplete(Task task)
        {
            loading.Dismiss();
        }
    }
}