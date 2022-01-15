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
using Plugin.CloudFirestore;
using Plugin.Media;

namespace GBV_Emergency_Response.Fragments
{
    public class ProfileFragment : Fragment, IOnSuccessListener, IOnFailureListener
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        private TextInputEditText InputName;
        private TextInputEditText InputEmail;
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
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.Input_Email);
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
            //auth = FirebaseAuth.Instance;
            
                
            pool = true;
            ImgBackground.Alpha = 0.3f;
            txt_friends.Text = $"{0} Friends";

            CrossCloudFirestore
                .Current
                .Instance
                .Collection("PEOPLE")
                .Document(FirebaseAuth.Instance.Uid)
                .AddSnapshotListener((value, error) =>
                {
                    if (value.Exists)
                    {
                        var users = value.ToObject<AppUsers>();
                        InputName.Text = users.Name;
                        InputSurname.Text = users.Surname;
                        ///InputUsername.Text = users.Username;
                        InputPhoneNr.Text = users.PhoneNumber;
                        InputEmail.Text = users.Email;
                        if(pool == true)
                        {
                            ImageService.Instance
                                .LoadUrl(users.ImageUrl)
                                .DownSampleInDip(512, 512)
                                .IntoAsync(ImgProfile);
                            ImageService.Instance
                                .LoadUrl(users.ImageUrl)
                                .DownSampleInDip(512, 512)
                                .IntoAsync(ImgBackground);
                        }
                    }
                });




        }

      
        private void ImgProfile_Click(object sender, EventArgs e)
        {
            //Console.WriteLine("Button Image Click");
            ImgProfile.Enabled = false;
            ChosePicture();
            ImgProfile.Enabled = true;

        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputUsername.Text.Trim()) && string.IsNullOrWhiteSpace(InputUsername.Text.Trim()))
            {
                InputUsername.Error = "Please provide your username";
                return;
            }
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

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Username", InputName.Text);
            data.Add("Name", InputName.Text);
            data.Add("Surname", InputSurname.Text);
            data.Add("PhoneNumber", InputPhoneNr.Text);
            data.Add("Email", InputEmail.Text);

            CrossCloudFirestore
                .Current
                .Instance
                .Collection("PEOPLE")
                .Document(FirebaseAuth.Instance.Uid)
                .UpdateAsync(data);
            AndHUD.Shared.ShowSuccess(context, "You have successfully updated your profile", AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(2));
        }
        
        private byte[] imageArray;
       
        StorageReference storageRef;
        private bool pool;
        private async void ChosePicture()
        {
            await CrossMedia.Current.Initialize();
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
               
                    var storage_ref = Plugin.FirebaseStorage.CrossFirebaseStorage
                         .Current
                         .Instance
                         .RootReference
                         .Child("PROFILE")
                         .Child(FirebaseAuth.Instance.CurrentUser.Uid);

                    await storage_ref.PutStreamAsync(file.GetStream());

                    var url = await storage_ref.GetDownloadUrlAsync();

                    //    .PutStreamAsync(file.GetStream());
                    await CrossCloudFirestore
                        .Current
                        .Instance
                        .Collection("PEOPLE")
                        .Document(FirebaseAuth.Instance.Uid)
                        .UpdateAsync("ImageUrl", url.ToString());


                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
            }

        }
    


        public async void OnSuccess(Java.Lang.Object result)
        {
            if (storageRef != null)
            {
                var url = await storageRef.GetDownloadUrlAsync();
                if (url != null)
                {
                    await CrossCloudFirestore
                        .Current
                        .Instance
                        .Collection("PEOPLE")
                        .Document(FirebaseAuth.Instance.Uid)
                        .UpdateAsync("ImageUrl", url);
                    pool = true;

                }
            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(context, e.Message, ToastLength.Long).Show();
        }
    }
   
}