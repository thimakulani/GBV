using System;
using System.Collections.Generic;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.App;
using Firebase.Auth;
using GBV_Emergency_Response.Activities;
using Google.Android.Material.Button;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;
using ID.IonBit.IonAlertLib;
using Java.Util;
using Plugin.CloudFirestore;

namespace GBV_Emergency_Response.Fragments
{
    public class ResetFragment : HelpFragment, IOnFailureListener, IOnSuccessListener, IOnCompleteListener
    {

        private MaterialButton BtnRegister;
        private MaterialButton BtnLogin;
        private TextInputEditText InputName;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhoneNumber;
        private TextInputEditText InputPassword;
        private TextInputEditText InputEmail;
        private TextInputEditText InputUsername;
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
            View view = inflater.Inflate(Resource.Layout.reset, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.InputEmail);
            BtnRegister = view.FindViewById<MaterialButton>(Resource.Id.btnRegister);
            BtnLogin = view.FindViewById<MaterialButton>(Resource.Id.BtnBackToLogin);
            BtnRegister.Click += BtnRegister_Click;
            BtnLogin.Click += BtnLogin_Click;
        }
        private FirebaseAuth auth;
        private IonAlert loadingDialog;
        private void BtnRegister_Click(object sender, EventArgs e)
        {
          
            if (string.IsNullOrEmpty(InputEmail.Text) && string.IsNullOrWhiteSpace(InputEmail.Text))
            {
                InputEmail.Error = "Please provide your email";//, ToastLength.Long).Show();
                return;
            }
           
            BtnRegister.Enabled = false;
            loadingDialog = new IonAlert(context, IonAlert.ProgressType);
            loadingDialog.SetSpinKit("WanderingCubes")
                .SetSpinColor("#008D91")
                .ShowCancelButton(false)
                .Show();
            auth = FirebaseAuth.Instance;
            auth.SendPasswordResetEmail(InputEmail.Text.Trim())
                .AddOnFailureListener(this)
                .AddOnSuccessListener(this)
                .AddOnCompleteListener(this);
            
        }

        public event EventHandler LoginHandler;
      

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            LoginHandler(sender, e);
        }
        //loading progress dialog
   
        public void OnFailure(Java.Lang.Exception e)
        {
            MaterialAlertDialogBuilder alert = new MaterialAlertDialogBuilder(context);
            //Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(context);
            alert.SetTitle("Error");
            alert.SetMessage(e.Message);
            alert.SetNeutralButton("OK", delegate
            {
                alert.Dispose();
            });
            alert.Show();
        }
       
        public async void OnSuccess(Java.Lang.Object result)
        {
            //MaterialAlertDialogBuilder alert = new MaterialAlertDialogBuilder(context);
            ////Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(context);
            //alert.SetTitle("Error");
            //alert.SetMessage(result.ToString());
            //alert.SetNeutralButton("OK", delegate
            //{
            //    alert.Dispose();
            //});
            //alert.Show(); loadingDialog.Dismiss();
            loadingDialog.Dismiss();
        }

        public void OnComplete(Task task)
        {
            MaterialAlertDialogBuilder alert = new MaterialAlertDialogBuilder(context);
            //Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(context);
            alert.SetTitle("Done");
            alert.SetMessage(task.ToString());
            alert.SetNeutralButton("OK", delegate
            {
                alert.Dispose();
            });
            alert.Show(); loadingDialog.Dismiss();
            BtnRegister.Enabled = true;
        }
    }
}