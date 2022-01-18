using Android;
using Android.App;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Extensions;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using Firebase.Auth;
using Firebase.Iid;
using FirebaseAdmin.Messaging;
using GBV_Emergency_Response.Dialogs;
using GBV_Emergency_Response.Fragments;
using GBV_Emergency_Response.MapHelper;
using GBV_Emergency_Response.Models;
using Google.Android.Material.BottomNavigation;
using IsmaelDiVita.ChipNavigationLib;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Threading;
using Xamarin.Essentials;
using static IsmaelDiVita.ChipNavigationLib.ChipNavigationBar;
using Notification = FirebaseAdmin.Messaging.Notification;
using PopupMenu = AndroidX.AppCompat.Widget.PopupMenu;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace GBV_Emergency_Response.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class HomeActivity : AndroidX.AppCompat.App.AppCompatActivity, IOnItemSelectedListener
    {
        private ChipNavigationBar nav_menu;
        private Toolbar home_toolbar;
        private ImageView ImgBackground;



        /*LOCATION*/
        LocationRequest locationRequest;
        FusedLocationProviderClient FusedLocationProviderClient;

        //permissin

        readonly string[] permission =
      {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.AccessNotificationPolicy,
            Manifest.Permission.BindNotificationListenerService
        };

        const int requestLocationId = 0;
        //

        
        FusedLocationProviderClient locationClient;
        Android.Locations.Location lastLocation;

        private const int UPDATE_INTERVAL = 5;
        private const int UPDATE_FASTEST_INTERVAL = 5;
        private const int DISPLACEMENT = 3;
        private LocationCallBackHelper locationCallBack;

        private void LoadCurrentLocation(Location location)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Latitude", null);
            data.Add("Longitude", null);

            CrossCloudFirestore
                .Current
                .Instance
                .Collection("LOCATION")
                .Document(FirebaseAuth.Instance.CurrentUser.Uid)
                .UpdateAsync(data);
        }

        //private readonly List<AlertsMessages> items = new List<AlertsMessages>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestedOrientation = ScreenOrientation.Portrait;
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            RequestPermissions(permission, 0);
            IsPlayServiceAvailabe();
            Firebase.Messaging.FirebaseMessaging.Instance.SubscribeToTopic("alerts");

            var response = Firebase.Messaging.FirebaseMessaging.Instance.SubscribeToTopic("requests").AsAsync();
            Toast.MakeText(this, response.Id.ToString(), ToastLength.Long).Show();

            nav_menu = FindViewById<ChipNavigationBar>(Resource.Id.bottom_nav_view);
            //nav_menu.InflateMenu(Resource.Menu.nav_menu);
            //nav_menu.SetOnNavigationItemSelectedListener += Nav_menu_NavigationItemSelected;
            nav_menu.SetItemSelected(Resource.Id.navHome);
            nav_menu.SetOnItemSelectedListener(this);

            //var notificationsTab = nav_menu.FindViewById<BottomNavigationItemView>(Resource.Id.navAlerts);
            //badge = LayoutInflater.From(this).Inflate(Resource.Layout.component_badge, notificationsTab, false);
            //_notificationBadgeTextView = badge.FindViewById<TextView>(Resource.Id.badgeTextView);
            //notificationsTab.AddView(badge);
            //_notificationBadgeTextView.Text = "0";

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

                CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("PEOPLE")
                    .Document(FirebaseAuth.Instance.Uid)
                    .AddSnapshotListener((snapshot, error) =>
                    {
                        if (snapshot.Exists)
                        {
                            var user = snapshot.ToObject<AppUsers>();
                            home_toolbar.Title = $"{user.Name} {user.Surname}".ToUpper();
                        }
                    });
            }
            CrossCloudFirestore.Current.Instance
                 .Collection("AWARENESS")
                 .OrderBy("Dates", false)
                 .AddSnapshotListener((value, errors) =>
                 {
                     nav_menu.ShowBadge(Resource.Id.navAlerts, value.Count);
                 });

            CreateLocationRequest();
            if (CheckPermission())
            {
                GetLocation();
                StartLocationUpdate();
            }
            

        }

        private Boolean IsPlayServiceAvailabe()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    Toast.MakeText(this, GoogleApiAvailability.Instance.GetErrorString(resultCode), ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, "This device is not supported", ToastLength.Long).Show();
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private async void Alerts_ShowMapHandler(object sender, AlertsFragment.ShowMapFragmentArgs e)
        {
            if(e.Type == 1)
            {
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
                cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

                MapFragmentDialog profile = new MapFragmentDialog(e.Alerts.UserKey);
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, profile)
                    .Commit();
            }
            else
            {
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
                cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

                var location = new Xamarin.Essentials.Location(double.Parse(e.Alerts.Lat), double.Parse(e.Alerts.Lon));
                var options = new MapLaunchOptions { Name = "Victim", };

                try
                {
                    await Map.OpenAsync(location, options);
                }
                catch (Exception ex)
                {
                    // No map application available to open
                    Console.WriteLine(ex.Message);
                }
            }
        }

        protected override  void OnResume()
        {
            base.OnResume();
            
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
        private IDocumentReference dbref;
        private async void Home_PanicButtonEventHandler(object sender, EventArgs e)
        {

            CrossCloudFirestore
              .Current
              .Instance
              .Collection("PEOPLE")
              .Document(FirebaseAuth.Instance.Uid)
              .AddSnapshotListener(async(value, error) =>
              {
                  if (value.Exists)
                  {
                      var users = value.ToObject<AppUsers>();

                      Dictionary<string, object> data = new Dictionary<string, object>();
                        data.Add("Uid", FirebaseAuth.Instance.CurrentUser.Uid);
                        data.Add("TimeDate", FieldValue.ServerTimestamp);
                        dbref = await CrossCloudFirestore
                            .Current
                            .Instance
                            .Collection("EMERGENCY")
                            .AddAsync(data);
                        var stream = Resources.Assets.Open("ServiceAccount.json");
                        var fcm = FirebaseHelper.FirebaseAdminSDK.GetFirebaseMessaging(stream);
                        FirebaseAdmin.Messaging.Message message = new FirebaseAdmin.Messaging.Message()
                        {
                            Topic = "alerts",
                            Notification = new Notification()
                            {
                                Title = "Emergency Alert",
                                Body = $"${users.Username} may be in need of emergency assistance, please contact him on ${users.PhoneNumber} or contact the police and provide the address in the alert locator in the map.",

                            },

                        };
                        await fcm.SendAsync(message);

                  
                  }
                });
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            //Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

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
            locationRequest = LocationRequest.Create();
            locationRequest.SetInterval(UPDATE_INTERVAL);
            locationRequest.SetFastestInterval(UPDATE_FASTEST_INTERVAL);
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetSmallestDisplacement(DISPLACEMENT);
            locationClient = LocationServices.GetFusedLocationProviderClient(this);
            locationCallBack = new LocationCallBackHelper();
            locationCallBack.CurrentLocation += LocationCallBack_CurrentLocation;
        }
        
        private void LocationCallBack_CurrentLocation(object sender, LocationCallBackHelper.OnLocationCapturedEventArgs e)
        {
            Console.WriteLine($"Latitude {e.location.Latitude}  Longitude: {e.location.Longitude}");
            
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("Latitude", lastLocation.Latitude.ToString());
                data.Add("Longitude", lastLocation.Longitude.ToString());
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("LOCATION")
                .Document(FirebaseAuth.Instance.CurrentUser.Uid)
                .UpdateAsync(data);
            
            //lastLocation = e.location;
            //Dictionary<string, object> data = new Dictionary<string, object>();
            //if (SendAlert)
            //{
            //if (CurrentKey == "X")
            //{
            //    Dictionary<string, object> data = new Dictionary<string, object>();
            //    data.Add("Latitude", lastLocation.Latitude.ToString());
            //    data.Add("Longitude", lastLocation.Longitude.ToString());
            //    data.Add("Uid", FirebaseAuth.Instance.CurrentUser.Uid);
            //    data.Add("TimeDate", FieldValue.ServerTimestamp);
            //    var dbref = await CrossCloudFirestore
            //        .Current
            //        .Instance
            //        .Collection("EMERGENCY")
            //        .AddAsync(data);
            //    CurrentKey = dbref.Id; 

            //}
            //else
            //{
            //    data.Add("Latitude", lastLocation.Latitude.ToString());
            //    data.Add("Longitude", lastLocation.Longitude.ToString());
            //    data.Add("LastTimeStamp", FieldValue.ServerTimestamp);
            //    await CrossCloudFirestore
            //        .Current
            //        .Instance
            //        .Collection("EMERGENCY")
            //        .Document(CurrentKey)
            //        .UpdateAsync(data);


            //}
            //SendAlert = false;
            // }
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
                Toast.MakeText(this, "No permission", ToastLength.Long).Show();
                return;
            }
            lastLocation = await locationClient.GetLastLocationAsync();

        }

        public void OnItemSelected(int id)
        {
            if (id == Resource.Id.navHome)
            {
                HomeFragment home = new HomeFragment();
                home.PanicButtonEventHandler += Home_PanicButtonEventHandler;
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, home)
                    .Commit();
            }
            if (id == Resource.Id.navAwareness)
            {
                AwarenessFragment awareness = new AwarenessFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, awareness)
                    .Commit();
            }
            if (id == Resource.Id.navForum)
            {
                ForumFragment forum = new ForumFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, forum)
                    .Commit();

            }
            if (id == Resource.Id.navAlerts)
            {
                AlertsFragment alerts = new AlertsFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, alerts)
                    .Commit();
                alerts.ShowMapHandler += Alerts_ShowMapHandler;
            }
            if (id == Resource.Id.navUserProfile)
            {
                ProfileFragment profile = new ProfileFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragHost, profile)
                    .Commit();
            }
        }
    }


}