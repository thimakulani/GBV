using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Auth;
using Firebase.Database;
using GBV_Emergency_Response.Activities;
using Java.Lang;
using System.Threading.Tasks;
using static Firebase.Auth.FirebaseAuth;

namespace GBV_Emergency_Response
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class Splash : AppCompatActivity
    {
       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if(savedInstanceState == null)
            {
                FirebaseDatabase.Instance.SetPersistenceEnabled(true);
            }
            
        }
        protected override void OnResume()
        {
            
            base.OnResume();
            System.Threading.Tasks.Task startWork = new System.Threading.Tasks.Task(() =>
            {
                System.Threading.Tasks.Task.Delay(3000);
            });
            startWork.ContinueWith(t =>
            {
                var auth = FirebaseAuth.Instance;
                if (auth.CurrentUser != null)
                {
                    Intent intent = new Intent(Application.Context, typeof(HomeActivity));
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                    //Toast.MakeText(this, "toast", ToastLength.Long).Show();
                    Finish();
                }
                else
                {
                    Intent intent = new Intent(Application.Context, typeof(LoginSignupActivity));
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                    Finish();
                }
            },
            TaskScheduler.FromCurrentSynchronizationContext());
            startWork.Start();
        }

        
    }
}