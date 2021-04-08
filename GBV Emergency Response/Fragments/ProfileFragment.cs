using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using FFImageLoading;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using GBV_Emergency_Response.Models;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Plugin.Media;

namespace GBV_Emergency_Response.Fragments
{
    public class ProfileFragment : HelpFragment, IValueEventListener, IOnSuccessListener, IOnFailureListener
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        private TextInputEditText InputName;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhoneNr;
        private TextInputEditText InputUsername;
        private TextView txt_friends;
        private MaterialButton BtnUpdate;
        private ImageView ImgBackground; 
        private ImageView ImgProfile;
        private Context context;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.profile_fragment, container, false);
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            context = view.Context;
            InputUsername = view.FindViewById<TextInputEditText>(Resource.Id.Input_Username);
            txt_friends = view.FindViewById<TextView>(Resource.Id.txt_friends);
            InputName = view.FindViewById<TextInputEditText>(Resource.Id.Input_Name);
            InputSurname = view.FindViewById<TextInputEditText>(Resource.Id.Input_Surname);
            InputPhoneNr = view.FindViewById<TextInputEditText>(Resource.Id.Input_Phone);
            BtnUpdate = view.FindViewById<MaterialButton>(Resource.Id.BtnUpdate);
            ImgBackground = view.FindViewById<ImageView>(Resource.Id.ImgAvater);
            ImgProfile = view.FindViewById<ImageView>(Resource.Id.ImgProfile);
            BtnUpdate.Click += BtnUpdate_Click;
            ImgProfile.Click += ImgProfile_Click;
            auth = FirebaseAuth.Instance;
            FirebaseDatabase.Instance.GetReference("Users")
               .Child(auth.CurrentUser.Uid)
               .AddValueEventListener(this);
            CountFriends counterData = new CountFriends();
            counterData.GetCount(auth.CurrentUser.Uid);
            counterData.RetriveFreinds += CounterData_RetriveFreinds;
                
            pool = true;
            ImgBackground.Alpha = 0.3f;
            txt_friends.Text = $"{0} Friends";
        }

        private void CounterData_RetriveFreinds(object sender, CountFriends.FirendsValueEventHandler e)
        {
            txt_friends.Text = $"{e.Counter} Connected Friends";
        }

        private void ImgProfile_Click(object sender, EventArgs e)
        {
            ChosePicture();

        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputName.Text.Trim()) && string.IsNullOrWhiteSpace(InputName.Text.Trim()))
            {
                InputName.Error = "Please provide your name";
                return;
            }
            if (string.IsNullOrEmpty(InputSurname.Text.Trim()) && string.IsNullOrWhiteSpace(InputSurname.Text.Trim()))
            {
                InputSurname.Error = "Please provide your surname";
                return;
            }
            if (string.IsNullOrEmpty(InputPhoneNr.Text.Trim()) && string.IsNullOrWhiteSpace(InputPhoneNr.Text.Trim()))
            {
                InputPhoneNr.Error = "Please provide your phone number";//, ToastLength.Long).Show();
                return;
            }
            
            FirebaseDatabase.Instance.GetReference("Users")
                .Child(FirebaseAuth.Instance.CurrentUser.Uid)
                .Child("Username").SetValue(InputUsername.Text.Trim());
            FirebaseDatabase.Instance.GetReference("Users")
                .Child(FirebaseAuth.Instance.CurrentUser.Uid)
                .Child("Name").SetValue(InputName.Text.Trim());
            FirebaseDatabase.Instance.GetReference("Users")
                .Child(FirebaseAuth.Instance.CurrentUser.Uid)
                .Child("Surname").SetValue(InputSurname.Text.Trim());
            FirebaseDatabase.Instance.GetReference("Users")
                .Child(FirebaseAuth.Instance.CurrentUser.Uid)
                .Child("PhoneNumber").SetValue(InputPhoneNr.Text.Trim());
            AndHUD.Shared.ShowSuccess(context, "You have successfully updated your profile", AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(2));
        }
        FirebaseAuth auth;
        private byte[] imageArray;
       
        StorageReference storageRef;
        private bool pool;
        private async void ChosePicture()
        {
            var media =  CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                Toast.MakeText(Android.App.Application.Context, "Upload not supported on this device", ToastLength.Short).Show();
                return;
            }
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                    CompressionQuality = 40,

                });
                imageArray = System.IO.File.ReadAllBytes(file.Path);

                if (imageArray != null)
                {
                    storageRef = FirebaseStorage.Instance.GetReference("Profile").Child(auth.CurrentUser.Uid);//FirebaseHelper.FirebaseData.GetFirebaseStorage().GetReference("UserProfile").Child(auth.CurrentUser.Uid);
                    storageRef.PutBytes(imageArray)
                        .AddOnSuccessListener(this)
                        .AddOnFailureListener(this);
                }

            }
            catch (Exception)
            {

            }

        }
    
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                if (snapshot.Child("Username").Exists())
                {
                    InputUsername.Text = snapshot.Child("Username").Value.ToString();
                }
                if (snapshot.Child("Name").Exists())
                {
                    InputName.Text = snapshot.Child("Name").Value.ToString();
                }
                if (snapshot.Child("Surname").Exists())
                {
                    InputSurname.Text = snapshot.Child("Surname").Value.ToString();

                }
                if (snapshot.Child("PhoneNumber").Exists())
                {
                    InputPhoneNr.Text = snapshot.Child("PhoneNumber").Value.ToString();

                }
                if (snapshot.Child("ImgUrl").Exists())
                {
                    if (pool == true)
                    {
                        ImageService.Instance
                            .LoadUrl(snapshot.Child("ImgUrl").Value.ToString())
                            .Retry(3, 200)
                            .DownSampleInDip(250, 250)
                            .Transform(new ImageTransformations.CircleTransformation())
                            .FadeAnimation(true, true, 300)
                            .IntoAsync(ImgProfile);
                        ImageService.Instance
                            .LoadUrl(snapshot.Child("ImgUrl").Value.ToString())
                            .Retry(3, 200)
                            .FadeAnimation(true, true, 300)
                            .DownSampleInDip(250, 250)
                            .IntoAsync(ImgBackground);
                        pool = false;
                    }



                }
            }
        }

        public async void OnSuccess(Java.Lang.Object result)
        {
            if (storageRef != null)
            {
                var url = await storageRef.GetDownloadUrlAsync();
                if (url != null)
                {
                    FirebaseDatabase.Instance
                    .GetReference("Users")
                    .Child(auth.CurrentUser.Uid)
                    .Child("ImgUrl")
                    .SetValue(url.ToString());
                    pool = true;

                }
            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(context, e.Message, ToastLength.Long).Show();
        }
    }
    public class CountFriends : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<FirendsValueEventHandler> RetriveFreinds;
        public class FirendsValueEventHandler : EventArgs
        {
            public int Counter { get; set; }
        }
        public void GetCount(string key)
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
            int counter = 0;
            if (snapshot.Exists())
            {

                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach (var data in child)
                {
                    if(data.Child("Type").Value.ToString() == "Approved")
                    {
                        counter++;
                    }
                    
                }
            }
            RetriveFreinds.Invoke(this, new FirendsValueEventHandler { Counter = counter });
        }
    }
}