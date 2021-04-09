using Android;
using Android.App;
using Android.Content.PM;
using Android.Gms.Extensions;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.App;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Messaging;
using GBV_Emergency_Response.AppDataHelper;
using GBV_Emergency_Response.Dialogs;
using GBV_Emergency_Response.Fragments;
using GBV_Emergency_Response.MapHelper;
using GBV_Emergency_Response.Models;
using Google.Android.Material.BottomNavigation;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PopupMenu = AndroidX.AppCompat.Widget.PopupMenu;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace GBV_Emergency_Response.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class HomeActivity : AndroidX.AppCompat.App.AppCompatActivity, IValueEventListener
    {
        private Google.Android.Material.BottomNavigation.BottomNavigationView nav_menu;
        private Toolbar home_toolbar;
        private ImageView ImgBackground;



        /*LOCATION*/
        LocationRequest locationRequest;
        //permissin

        readonly string[] permission = { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };
        const int requestLocationId = 0;
        //


        FusedLocationProviderClient locationClient;
        Android.Locations.Location lastLocation;


        private const int UPDATE_INTERVAL = 5;
        private const int UPDATE_FASTEST_INTERVAL = 5;
        private const int DISPLACEMENT = 3;
        private LocationCallBackHelper locationCallBack;




        private View badge;
        private TextView _notificationBadgeTextView;
        private List<AlertsMessages> items = new List<AlertsMessages>();
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            await FirebaseMessaging.Instance.SubscribeToTopic("requests");

            nav_menu = FindViewById<BottomNavigationView>(Resource.Id.bottom_nav_view);
            //nav_menu.InflateMenu(Resource.Menu.nav_menu);
            nav_menu.NavigationItemSelected += Nav_menu_NavigationItemSelected;

            var notificationsTab = nav_menu.FindViewById<BottomNavigationItemView>(Resource.Id.navAlerts);
            badge = LayoutInflater.From(this).Inflate(Resource.Layout.component_badge, notificationsTab, false);
            _notificationBadgeTextView = badge.FindViewById<TextView>(Resource.Id.badgeTextView);
            notificationsTab.AddView(badge);
            _notificationBadgeTextView.Text = "0";

            ImgBackground = FindViewById<ImageView>(Resource.Id.ImgBackground);
            ImgBackground.Alpha = 0.3f;

            home_toolbar = FindViewById<Toolbar>(Resource.Id.home_toolbar);
            home_toolbar.MenuItemClick += Home_toolbar_MenuItemClick;
            
            home_toolbar.NavigationClick += Home_toolbar_NavigationClick;
            //FirebaseAuth.Instance.SignOut();
            if (savedInstanceState == null)
            {
                HomeFragment home = new HomeFragment();
                home.PanicButtonEventHandler += Home_PanicButtonEventHandler;
                SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.fragHost, home)
                    .Commit();
            }
            if(FirebaseAuth.Instance.CurrentUser != null)
            {
                FirebaseDatabase.Instance.GetReference("Users")
                .Child(FirebaseAuth.Instance.CurrentUser.Uid)
                .AddValueEventListener(this);
            }
           
            if (CheckPermission())
            {
                //CreateLocationRequest();
                //GetLocation();
                //StartLocationUpdate();
            }
    
            AlertsMessagesData alerts = new AlertsMessagesData(FirebaseAuth.Instance.CurrentUser.Uid);
            alerts.GetAlerts();
            alerts.RetrivedAlerts += Alerts_RetrivedAlerts;
        }
        protected override  void OnResume()
        {
            base.OnResume();
            
        }
        private void Alerts_RetrivedAlerts(object sender, AlertsMessagesData.AlertsHandler e)
        {
            int counter = 0;
            items = e.Items;
            foreach(var data in items)
            {
                TimeSpan timeSpan = DateTime.Now - data.TimeDate;
                if(timeSpan.TotalMinutes <= 20)
                {
                    counter++;
                }
            }
            _notificationBadgeTextView.Text = counter.ToString();
        }

        private void Home_toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
        {
            
            PopupMenu popupMenu = new PopupMenu(this, e.View);
            popupMenu.Menu.Add(Menu.First, 0, 0, "Helpline");
            
            popupMenu.Menu.Add(Menu.First, 1, 1, "About");
            popupMenu.Show();
            popupMenu.MenuItemClick += PopupMenu_MenuItemClick;
        }

        private void PopupMenu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            if(e.Item.ItemId == 0)
            {
                HelpLineDialogFragment about = new HelpLineDialogFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, about)
                    .Commit();

            }
            if(e.Item.ItemId == 1)
            {
                AboutFragment about = new AboutFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, about)
                    .Commit();

            }
        }

        private async void Home_toolbar_MenuItemClick(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            if(Resource.Id.nav_signout == e.Item.ItemId)
            {
                FirebaseAuth.Instance.SignOut();
                await System.Threading.Tasks.Task.Delay(3000);
                Finish();
                
            }
            if (Resource.Id.nav_find_people == e.Item.ItemId)
            {
                var fm = SupportFragmentManager.BeginTransaction();
                AppUsersDialogFragment appUsersDialog = new AppUsersDialogFragment();
                appUsersDialog.Show(fm, appUsersDialog.Tag);
            }
        }

        private void Home_PanicButtonEventHandler(object sender, EventArgs e)
        {
            if (CheckPermission())
            {
                SendAlert = true;
                CreateLocationRequest();
                GetLocation();
                StartLocationUpdate();
            }
            else
            {
                Android.Widget.Toast.MakeText(this, "Permission not granted", Android.Widget.ToastLength.Long).Show();
            }
        }

        private void StartLocationUpdate()
        {
            if (CheckPermission())
            {
                locationClient.RequestLocationUpdates(locationRequest, locationCallBack, null);
            }
        }

        private void CreateLocationRequest()
        {
            locationRequest = new LocationRequest();
            locationRequest.SetInterval(UPDATE_INTERVAL);
            locationRequest.SetFastestInterval(UPDATE_FASTEST_INTERVAL);
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetSmallestDisplacement(DISPLACEMENT);
            locationClient = LocationServices.GetFusedLocationProviderClient(this);
            locationCallBack = new LocationCallBackHelper();
            locationCallBack.CurrentLocation += LocationCallBack_CurrentLocation;
        }
        private string CurrentKey = "X";
        private bool SendAlert = false;
        private string PersonNames;
        private string PersonPhoneNr;
        private void LocationCallBack_CurrentLocation(object sender, LocationCallBackHelper.OnLocationCapturedEventArgs e)
        {
            lastLocation = e.location;
            HashMap data = new HashMap();
            if (SendAlert)
            {


                if (CurrentKey == "X")
                {
                    data.Put("Latitude", lastLocation.Latitude.ToString());
                    data.Put("Longitude", lastLocation.Longitude.ToString());
                    data.Put("Name", PersonNames);
                    data.Put("PhoneNr", PersonPhoneNr);
                    data.Put("UserKey", FirebaseAuth.Instance.CurrentUser.Uid);

                    
                    data.Put("TimeDate", DateTime.Now.ToString("dddd, dd/MMMM/yyyy, HH:mm tt"));
                    var dbref = FirebaseDatabase.Instance.GetReference("EmergencyAlert").Push();
                    dbref.SetValue(data);
                    CurrentKey = dbref.Key.ToString();

                }
                else
                {

                    FirebaseDatabase.Instance.GetReference("EmergencyAlert").Child(CurrentKey).Child("Latitude")
                        .SetValue(e.location.Latitude.ToString());
                    FirebaseDatabase.Instance.GetReference("EmergencyAlert").Child(CurrentKey).Child("Longitude")
                         .SetValue(e.location.Longitude.ToString());

                }
                SendAlert = false;
            }
        }

        private bool CheckPermission()
        {
            bool permisionGranted;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
            {
                permisionGranted = true;
                RequestPermissions(permission, requestLocationId);
            }
            else
            {
                return true;
            }
            return permisionGranted;
        }
        private async void GetLocation()
        {
            if (!CheckPermission())
            {
                return;
            }
            lastLocation = await locationClient.GetLastLocationAsync();

        }

        private void Nav_menu_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.navHome)
            {
                HomeFragment home = new HomeFragment();
                home.PanicButtonEventHandler += Home_PanicButtonEventHandler;
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, home)
                    .Commit();
            }
            if (e.Item.ItemId == Resource.Id.navAwareness)
            {
                AwarenessFragment awareness = new AwarenessFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, awareness)
                    .Commit();
            }
            if (e.Item.ItemId == Resource.Id.navForum)
            {
                ForumFragment forum = new ForumFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, forum)
                    .Commit();
            }
            if (e.Item.ItemId == Resource.Id.navAlerts)
            {
                AlertsFragment alerts = new AlertsFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, alerts)
                    .Commit();
            }
            if (e.Item.ItemId == Resource.Id.navUserProfile)
            {
                ProfileFragment profile = new ProfileFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, profile)
                    .Commit();
            }
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                if (snapshot.Child("Name").Exists() && snapshot.Child("Surname").Exists() && snapshot.Child("PhoneNumber").Exists())
                {
                    home_toolbar.Title = snapshot.Child("Name").Value.ToString() + " " + snapshot.Child("Surname").Value.ToString();
                    PersonNames = snapshot.Child("Name").Value.ToString() + " " + snapshot.Child("Surname").Value.ToString();
                    PersonPhoneNr = snapshot.Child("PhoneNumber").Value.ToString();
                    
                }

            }
        }
    }

    internal class Conmplete : Activity, IOnSuccessListener
    {
        public void OnSuccess(Java.Lang.Object result)
        {
            
        }
    }

    public class AppInvites : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<InvitesValueEventHandler> RetriveInvites;
        private readonly List<InviteModel> items = new List<InviteModel>();
        public class InvitesValueEventHandler : EventArgs
        {
           public List<InviteModel> Items { get; set; }
        }
        public void GetInvites(string key)
        {
            FirebaseDatabase.Instance.GetReference("Request")
                .Child(key)
                .AddValueEventListener(this);
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            items.Clear();
            if (snapshot.Exists())
            {

                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach(var data in child)
                {
                    InviteModel invite = new InviteModel()
                    {
                        Key = data.Key,
                        Status = data.Child("Type").Value.ToString(),
                    };
                    items.Add(invite);
                }
            }
            RetriveInvites.Invoke(this, new InvitesValueEventHandler { Items = items });
        }
    }
}