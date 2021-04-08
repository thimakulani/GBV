using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Firebase.Auth;
using Firebase.Database;
using GBV_Emergency_Response.Adapters;
using GBV_Emergency_Response.AppDataHelper;
using GBV_Emergency_Response.Dialogs;
using GBV_Emergency_Response.Models;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Hotstu.WaveDrawableLib;

namespace GBV_Emergency_Response.Fragments
{
    public class HomeFragment : HelpFragment, IValueEventListener
    {
        private MaterialButton BtnPanic;
        private FloatingActionButton FabInvites;
        private TextView txt_invite_count;
        WaveDrawable waveDrawable = new WaveDrawable();
        private RecyclerView recycler;
        private List<InviteModel> items = new List<InviteModel>();
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
            
            FabInvites.Click += FabInvites_Click;
            FirebaseDatabase.Instance.GetReference("Request")
                .Child(FirebaseAuth.Instance.CurrentUser.Uid)
                .AddValueEventListener(this);
        }

        private void FabInvites_Click(object sender, EventArgs e)
        {
            InvitesDialogFragment invitesDialogFragment = new InvitesDialogFragment();
            var fm = ChildFragmentManager.BeginTransaction();
            invitesDialogFragment.Show(fm, invitesDialogFragment.Tag);
        }

 

        private void Adapter_ItemClick(object sender, ContactsAdapterClickEventArgs e)
        {
            
        }

        public event EventHandler PanicButtonEventHandler;
        private void BtnPanic_LongClick(object sender, View.LongClickEventArgs e)
        {
            PanicButtonEventHandler(sender, e);
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }
        
        public void OnDataChange(DataSnapshot snapshot)
        {
            int counter = 0;
            items.Clear();
            if (snapshot.Exists())
            {
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach (var data in child)
                {
                    if (data.Child("Type").Value.ToString() == "Invite")
                    {
                        counter++;
                    }
                    if (data.Child("Type").Value.ToString() == "Approved")
                    {
                        InviteModel invite = new InviteModel()
                        {
                            Key = data.Key,
                            Status = data.Child("Type").Value.ToString(),
                        };
                        items.Add(invite);
                    }
                    
                }
                SetUpRecycler(items);


            }
            txt_invite_count.Text = counter.ToString();
        }

        private void SetUpRecycler(List<InviteModel> items)
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context);
            ContactsAdapter adapter = new ContactsAdapter(items);
            recycler.SetLayoutManager(linearLayoutManager);
            recycler.SetAdapter(adapter);
            adapter.ItemClick += Adapter_ItemClick1;
        }

        private void Adapter_ItemClick1(object sender, ContactsAdapterClickEventArgs e)
        {
            FriendDialogFragment friendFragment = new FriendDialogFragment(items[e.Position].Key);
            friendFragment.Show(ChildFragmentManager.BeginTransaction(), "Friends");

        }

        
    }
}