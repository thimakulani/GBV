using System;
using System.Collections.Generic;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Com.Amulyakhare.Textdrawable;
using GBV_Emergency_Response.Adapters;
using GBV_Emergency_Response.Dialogs;
using GBV_Emergency_Response.Models;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Plugin.CloudFirestore;

namespace GBV_Emergency_Response.Fragments
{
    public class HomeFragment : Fragment
    {
        private MaterialButton BtnPanic;
        private FloatingActionButton FabInvites;
        private TextView txt_invite_count;
        private RecyclerView recycler;
        private readonly List<AppUsers> items = new List<AppUsers>();
       // private ContactsData data = new ContactsData();
        private Context context;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return 

            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.home_fragment, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            BtnPanic = view.FindViewById<MaterialButton>(Resource.Id.BtnPanic);
            txt_invite_count = view.FindViewById<TextView>(Resource.Id.txt_invite_count);
            FabInvites = view.FindViewById<FloatingActionButton>(Resource.Id.FabInvites);
            recycler = view.FindViewById<RecyclerView>(Resource.Id.recyclerContactsList);
            BtnPanic.LongClick += BtnPanic_LongClick;

            BtnPanic.Click += BtnPanic_Click;
            
            FabInvites.Click += FabInvites_Click;


            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context);
            ContactsAdapter adapter = new ContactsAdapter(items);
            linearLayoutManager.Orientation = RecyclerView.Horizontal;
            recycler.HasFixedSize = true;
            recycler.SetLayoutManager(linearLayoutManager);

            recycler.SetAdapter(adapter);
            adapter.ItemClick += Adapter_ItemClick1;

            CrossCloudFirestore.Current.Instance
                .Collection("PEOPLE")
                .AddSnapshotListener((value, errors) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var dc in value.DocumentChanges)
                        {
                            var users = dc.Document.ToObject<AppUsers>();
                            switch (dc.Type)
                            {
                                case DocumentChangeType.Added:
                                    items.Add(users);
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    items[dc.OldIndex] = users;
                                    //Toast.MakeText(context, items[dc.OldIndex].Id, ToastLength.Long).Show();
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


            IsPlayServiceAvailabe();
        }

        private Boolean IsPlayServiceAvailabe()
        {
            int resultCode= GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(context);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    Toast.MakeText(context,GoogleApiAvailability.Instance.GetErrorString(resultCode),ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(context, "This device is not supported", ToastLength.Long).Show();
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private void BtnPanic_Click(object sender, EventArgs e)
        {
            PanicButtonEventHandler(sender, e);
        }

        private void FabInvites_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("tel:+27");
            var intent = new Intent(Intent.ActionDial, uri);
            StartActivity(intent);
            //InvitesDialogFragment invitesDialogFragment = new InvitesDialogFragment();
            //var fm = ChildFragmentManager.BeginTransaction();
            //invitesDialogFragment.Show(fm, invitesDialogFragment.Tag);
        }
        public event EventHandler PanicButtonEventHandler;
        private void BtnPanic_LongClick(object sender, View.LongClickEventArgs e)
        {
            PanicButtonEventHandler(sender, e);
        }

        private void Adapter_ItemClick1(object sender, ContactsAdapterClickEventArgs e)
        {
            FriendDialogFragment friendFragment = new FriendDialogFragment(items[e.Position].Id);
            friendFragment.Show(ChildFragmentManager.BeginTransaction(), "Friends");

        }

        
    }
}