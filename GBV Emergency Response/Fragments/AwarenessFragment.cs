using System;
using System.Collections.Generic;

using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Firebase.Auth;
using Firebase.Storage;
using GBV_Emergency_Response.Adapters;
using GBV_Emergency_Response.Models;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Java.Util;
using Plugin.Media;

namespace GBV_Emergency_Response.Fragments
{
    public class AwarenessFragment : HelpFragment, IOnSuccessListener, IOnFailureListener, IOnCompleteListener
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        private MaterialButton BtnCreateAwareness;
        private RecyclerView Recycler;
        private List<AwarenessMessages> items = new List<AwarenessMessages>();
        private Context context;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.awareness_fragment, container, false);
            
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            context = view.Context;
            BtnCreateAwareness = view.FindViewById<MaterialButton>(Resource.Id.BtnCreateAwareness);
            Recycler = view.FindViewById<RecyclerView>(Resource.Id.RecyclerAwareness);

            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context);
            AwarenessAdapter adapter = new AwarenessAdapter(items, FirebaseAuth.Instance.CurrentUser.Uid);
            linearLayoutManager.ReverseLayout = true;
            Recycler.SetLayoutManager(linearLayoutManager);
            Recycler.SetAdapter(adapter);
            adapter.ItemDeleteClick += Adapter_ItemDeleteClick;


            BtnCreateAwareness.Click += BtnCreateAwareness_Click;
        }

        private void BtnCreateAwareness_Click(object sender, EventArgs e)
        {

            DialogAddAwareness();
        }
        //Dialog
        private AlertDialog.Builder dialogBuilder;
        private AlertDialog AwarenessDialog;
        private Button SubmitAwareness;
        private EditText AwarenessInput;
        private string PersonNames;
        private ImageView ImgAwareness;
        private FloatingActionButton FabChoseImg;
        private void DialogAddAwareness()
        {

            dialogBuilder = new AlertDialog.Builder(context);
            LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.awareness_dialog, null);
            SubmitAwareness = view.FindViewById<Button>(Resource.Id.dlgBtnSubmiAwareness);
            AwarenessInput = view.FindViewById<EditText>(Resource.Id.dlgInputAwareness);
            ImgAwareness = view.FindViewById<ImageView>(Resource.Id.dlgImge);
            FabChoseImg = view.FindViewById<FloatingActionButton>(Resource.Id.FabUploadImg);
            FabChoseImg.Click += FabChoseImg_Click;
            SubmitAwareness.Click += SubmitAwareness_Click;

            dialogBuilder.SetView(view);
            dialogBuilder.SetCancelable(true);
            AwarenessDialog = dialogBuilder.Create();
            AwarenessDialog.Show();
        }

        private void FabChoseImg_Click(object sender, EventArgs e)
        {
            ChosePicture();
        }

        private void SubmitAwareness_Click(object sender, EventArgs e)
        {
            HashMap data = new HashMap();
            data.Put("SenderName", PersonNames);
            data.Put("SenderId", FirebaseAuth.Instance.CurrentUser.Uid);
            data.Put("Dates", DateTime.Now.ToString("dddd, dd/MMMM/yyyy, HH:mm tt"));
            data.Put("Message", AwarenessInput.Text);

            if (!string.IsNullOrEmpty(AwarenessInput.Text) && !string.IsNullOrWhiteSpace(AwarenessInput.Text))
            {
                
                //dbRef = FirebaseDatabase.Instance.GetReference("Awareness").Push();
                //dbRef.SetValue(data);
                if (imageArray != null)
                {
                    storageRef = FirebaseStorage.Instance.GetReference("Awareness");
                    storageRef.PutBytes(imageArray)
                        .AddOnSuccessListener(this)
                        .AddOnFailureListener(this);
                }

            }
            AwarenessInput.Text = string.Empty;
            AwarenessDialog.Dismiss();
        }
        StorageReference storageRef;
        private byte[] imageArray;

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

                if(imageArray != null)
                {
                    Android.Graphics.Bitmap bmp = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
                    ImgAwareness.SetImageBitmap(bmp);
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(context, ex.Message , ToastLength.Long).Show();
            }

        }
     

        private async void Adapter_ItemDeleteClick(object sender, AwarenessAdapterClickEventArgs e)
        {
           
            if(items[e.Position].ImgUrl != null)
            {
                await FirebaseStorage.Instance.GetReferenceFromUrl(items[e.Position].ImgUrl).DeleteAsync();
            }
            AndHUD.Shared.ShowSuccess(context, "You have successfully deleted", AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(2));
        }

        public async void OnSuccess(Java.Lang.Object result)
        {
            if (storageRef != null)
            {
               var url = await storageRef.GetDownloadUrlAsync();
                //Toast.MakeText(context, $"{r.ToString()}", ToastLength.Long).Show();
               if(url != null)
                {
                    
                }
            }
           
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(context, e.Message, ToastLength.Long).Show();
        }

  

        public void OnComplete(Task task)
        {
            if(task.IsSuccessful)
            {

            }
        }
    }
}