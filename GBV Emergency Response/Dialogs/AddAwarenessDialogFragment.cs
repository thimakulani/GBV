using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Activity.Result.Contract;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Firebase.Auth;
using Firebase.Storage;
using GBV_Emergency_Response.Activities;
using GBV_Emergency_Response.Adapters;
using GBV_Emergency_Response.Models;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using ID.IonBit.IonAlertLib;
using Plugin.CloudFirestore;
using Plugin.Media;
using Xamarin.Essentials;

namespace GBV_Emergency_Response.Dialogs
{
    public class AddAwarenessDialogFragment : DialogFragment, IOnSuccessListener, IOnFailureListener, IOnCompleteListener
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        private MaterialButton SubmitAwareness;
        private TextInputEditText AwarenessInput;
        private ImageView ImgAwareness;
        private FloatingActionButton FabChoseImg;
        private Context context;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
             base.OnCreateView(inflater, container, savedInstanceState);
            var view =inflater.Inflate(Resource.Layout.awareness_dialog, container, false);
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            context = view.Context;
            SubmitAwareness = view.FindViewById<MaterialButton>(Resource.Id.dlgBtnSubmiAwareness);
            AwarenessInput = view.FindViewById<TextInputEditText>(Resource.Id.dlgInputAwareness);
            ImgAwareness = view.FindViewById<ImageView>(Resource.Id.dlgImge);
            FabChoseImg = view.FindViewById<FloatingActionButton>(Resource.Id.FabUploadImg);
            FabChoseImg.Click += FabChoseImg_Click;
            SubmitAwareness.Click += SubmitAwareness_Click;

        }
        
        IDocumentReference query;
        IonAlert loadingDialog;
        private async void SubmitAwareness_Click(object sender, EventArgs e)
        {


            if (!string.IsNullOrEmpty(AwarenessInput.Text) && !string.IsNullOrWhiteSpace(AwarenessInput.Text))
            {
                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    { "Uid", FirebaseAuth.Instance.CurrentUser.Uid },
                    { "Dates", FieldValue.ServerTimestamp },
                    { "Message", AwarenessInput.Text },
                    { "ImageUrl", null },
                };
                loadingDialog = new IonAlert(context, IonAlert.ProgressType);
                loadingDialog.SetSpinKit("WanderingCubes")
                    .SetSpinColor("#008D91")
                    .ShowCancelButton(false)
                    .Show();
                query = await CrossCloudFirestore.Current
                    .Instance
                    .Collection("AWARENESS")
                    .AddAsync(data);

                //dbRef = FirebaseDatabase.Instance.GetReference("Awareness").Push();
                //dbRef.SetValue(data);
                if (imageArray != null)
                {
                    //storageRef = FirebaseStorage.Instance.GetReference("AWARENESS");
                    //storageRef.PutBytes(imageArray)
                    //    .AddOnSuccessListener(this)
                    //    .AddOnCompleteListener(this)
                    //    .AddOnFailureListener(this);


                    var storage_ref = Plugin.FirebaseStorage.CrossFirebaseStorage
                        .Current
                        .Instance
                        .RootReference
                        .Child("AWARENESS")
                        .Child(query.Id);

                    await storage_ref.PutStreamAsync(upload_file);

                    var url = await storage_ref.GetDownloadUrlAsync();

                    //    .PutStreamAsync(file.GetStream());
                    await CrossCloudFirestore
                        .Current
                        .Instance
                        .Collection("AWARENESS")
                        .Document(query.Id)
                        .UpdateAsync("ImageUrl", url.ToString());


                }
                else
                {
                    loadingDialog.Dismiss();
                }

            }
            else
            {
                AwarenessInput.Error = "Type a message";
            }
            AwarenessInput.Text = string.Empty;
            Dismiss();
        }
        StorageReference storageRef;
        private byte[] imageArray;
        System.IO.Stream upload_file;
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
                    CompressionQuality = 50,

                });
                if (file != null)
                {
                    upload_file = file.GetStream();
                    imageArray = System.IO.File.ReadAllBytes(file.Path);
                    if (imageArray != null)
                    {
                        Bitmap bmp = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
                        ImgAwareness.SetImageBitmap(bmp);
                    }


                    



                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(context, "error  " + ex.Message, ToastLength.Long).Show();
            }

        }

        private void FabChoseImg_Click(object sender, EventArgs e)
        {
            ChosePicture();
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
        public async void OnSuccess(Java.Lang.Object result)
        {
            if (storageRef != null)
            {
                var url = await storageRef.GetDownloadUrlAsync();
                //Toast.MakeText(context, $"{r.ToString()}", ToastLength.Long).Show();
                if (url != null)
                {
                    await query.UpdateAsync("ImageUrl", url.ToString());
                }
            }

        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(context, e.Message, ToastLength.Long).Show();
        }



        public void OnComplete(Task task)
        {
            loadingDialog.Dismiss();
        }

    }
}