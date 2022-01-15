using System;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using GBV_Emergency_Response.Fragments;

namespace GBV_Emergency_Response.Activities
{
    [Activity(Label = "LoginSignupActivity", NoHistory = true)]
    public class LoginSignupActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.activity_login_signup);
            if(savedInstanceState == null)
            {
                LoginFragment login = new LoginFragment();
                login.BtnSignUpClickEventHandler += Login_BtnSignUpClickEventHandler;
                login.SuccessEventHandler += Login_SuccessEventHandler;
                SupportFragmentManager.BeginTransaction()
                    .SetCustomAnimations(Resource.Animation.Pushup_in, Resource.Animation.Pushup_out)
                    .Add(Resource.Id.loginFragmentHost, login)
                    .Commit();
            }
            


        }

        private void Login_SuccessEventHandler(object sender, EventArgs e)
        {
            Intent intent = new Intent(Application.Context, typeof(HomeActivity));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            Finish();
        }

        private void Login_BtnSignUpClickEventHandler(object sender, EventArgs e)
        {
            SignupFragment signup = new SignupFragment();
            signup.LoginHandler += Signup_LoginHandler;
            SupportFragmentManager.BeginTransaction()
                .SetCustomAnimations(Resource.Animation.right_in, Resource.Animation.right_out, Resource.Animation.left_in, Resource.Animation.left_out)
                .Replace(Resource.Id.loginFragmentHost, signup)
                .Commit();
        }

        private void Signup_LoginHandler(object sender, EventArgs e)
        {
            LoginFragment login = new LoginFragment();
            login.BtnSignUpClickEventHandler += Login_BtnSignUpClickEventHandler;
            login.SuccessEventHandler += Login_SuccessEventHandler;
            SupportFragmentManager.BeginTransaction()
                .SetCustomAnimations( Resource.Animation.left_in, Resource.Animation.left_out, Resource.Animation.right_in, Resource.Animation.right_out)
                .Replace(Resource.Id.loginFragmentHost, login)
                .Commit();
        }
    }
}