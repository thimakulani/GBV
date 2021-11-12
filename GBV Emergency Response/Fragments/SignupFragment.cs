using System;
using System.Collections.Generic;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
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
    public class SignupFragment : HelpFragment, IOnFailureListener, IOnSuccessListener, IOnCompleteListener
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
            View view = inflater.Inflate(Resource.Layout.sign_up, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            InputUsername = view.FindViewById<TextInputEditText>(Resource.Id.InputUserName);
            InputName = view.FindViewById<TextInputEditText>(Resource.Id.InputFirstName);
            InputPassword = view.FindViewById<TextInputEditText>(Resource.Id.InputPassword);
            InputPhoneNumber = view.FindViewById<TextInputEditText>(Resource.Id.InputPhoneNumber);
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.InputEmail);
            InputSurname = view.FindViewById<TextInputEditText>(Resource.Id.InputLastName);
            BtnRegister = view.FindViewById<MaterialButton>(Resource.Id.btnRegister);
            BtnLogin = view.FindViewById<MaterialButton>(Resource.Id.BtnBackToLogin);
            BtnRegister.Click += BtnRegister_Click;
            BtnLogin.Click += BtnLogin_Click;
        }
        private FirebaseAuth auth;
        private IonAlert loadingDialog;
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
         
            BtnRegister.Enabled = false;
            loadingDialog = new IonAlert(context, IonAlert.ProgressType);
            loadingDialog.SetSpinKit("DoubleBounce")
                .SetSpinColor("#008D91")
                .ShowCancelButton(false)
                .Show();
            auth = FirebaseAuth.Instance;
            auth.CreateUserWithEmailAndPassword(InputEmail.Text.Trim(), InputPassword.Text.Trim())
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
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Username", InputUsername.Text);
            data.Add("Name", InputName.Text);
            data.Add("Surname", InputSurname.Text);
            data.Add("PhoneNumber", InputPhoneNumber.Text);
            data.Add("Email", InputEmail.Text);
            data.Add("ImageUrl", null);
            await CrossCloudFirestore
                .Current
                .Instance
                .Collection("PEOPLE")
                .Document(FirebaseAuth.Instance.Uid)
                .SetAsync(data);
           
            AndHUD.Shared.ShowSuccess(context, "You have successfully created your account", MaskType.Black, TimeSpan.FromSeconds(2));
            Intent intent = new Intent(context, typeof(HomeActivity));
            StartActivity(intent);
        }

        public void OnComplete(Task task)
        {
            loadingDialog.Dismiss();
            BtnRegister.Enabled = true;
        }
    }
}