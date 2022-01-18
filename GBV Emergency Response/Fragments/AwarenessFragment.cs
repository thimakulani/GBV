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
using GBV_Emergency_Response.Dialogs;
using GBV_Emergency_Response.Models;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Java.Util;
using Plugin.CloudFirestore;
using Plugin.Media;

namespace GBV_Emergency_Response.Fragments
{
    public class AwarenessFragment : HelpFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        private ExtendedFloatingActionButton FabCreateAwareness;
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
            FabCreateAwareness = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.FabCreateAwareness);
            Recycler = view.FindViewById<RecyclerView>(Resource.Id.RecyclerAwareness);

            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context);
            AwarenessAdapter adapter = new AwarenessAdapter(items, FirebaseAuth.Instance.CurrentUser.Uid);
            linearLayoutManager.ReverseLayout = true;
            Recycler.SetLayoutManager(linearLayoutManager);
            Recycler.SetAdapter(adapter);
            adapter.ItemDeleteClick += Adapter_ItemDeleteClick;

            

            CrossCloudFirestore.Current.Instance
                .Collection("AWARENESS")
                .OrderBy("Dates", false)
                .AddSnapshotListener((value, errors) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var dc in value.DocumentChanges)
                        {
                            var awareness = dc.Document.ToObject<AwarenessMessages>();
                            switch (dc.Type)
                            {
                                case DocumentChangeType.Added:
                                    items.Add(awareness);
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    items[dc.OldIndex] = awareness;
                                    Toast.MakeText(context, items[dc.OldIndex].Id, ToastLength.Long).Show();
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Removed:
                                    items.RemoveAt(dc.OldIndex);
                                    adapter.NotifyDataSetChanged();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                });

            FabCreateAwareness.Click += FabCreateAwareness_Click;
            //BtnCreateAwareness.Click += BtnCreateAwareness_Click;
        }

        private void FabCreateAwareness_Click(object sender, EventArgs e)
        {
            AddAwarenessDialogFragment dialog = new AddAwarenessDialogFragment();
            dialog.Show(ChildFragmentManager.BeginTransaction(), "");
        }


        //Dialog
     
        

        private async void Adapter_ItemDeleteClick(object sender, AwarenessAdapterClickEventArgs e)
        {
            var picture = items[e.Position].ImageUrl;

            await CrossCloudFirestore.Current.Instance
                 .Collection("AWARENESS")
                 .Document(items[e.Position].Id)
                 .DeleteAsync();
            if (picture != null)
            {
                try
                {
                    await FirebaseStorage.Instance.GetReferenceFromUrl(items[e.Position].ImageUrl).DeleteAsync();
                }catch(Exception ex)
                {
                    AndHUD.Shared.ShowSuccess(context, "You have successfully deleted", AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(2));
                }
            }
            AndHUD.Shared.ShowSuccess(context, "You have successfully deleted", AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(2));
        }


    }
}