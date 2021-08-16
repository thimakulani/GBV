﻿using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Firebase.Auth;
using GBV_Emergency_Response.Adapters;
using GBV_Emergency_Response.Models;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace GBV_Emergency_Response.Dialogs
{
    public class AppUsersDialogFragment : DialogFragment
    {
        private SearchView searchView;
        private Toolbar app_users_toolbar;
        private RecyclerView recyclerAppUsers;
        private Context context;
        private List<AppUsers> items = new List<AppUsers>();
        private List<AppUsers> TempList = new List<AppUsers>();
        private List<InviteModel> inviteItems = new List<InviteModel>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            HasOptionsMenu = true;
            
            SetStyle(StyleNoFrame, Resource.Style.FullScreenDialogStyle);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.app_users_diaog, container, false);
            ConnectView(view);
            context = view.Context;
            
            return view;
        }

        


        private void ConnectView(View view)
        {
            app_users_toolbar = view.FindViewById<Toolbar>(Resource.Id.app_users_toolbar);
            recyclerAppUsers = view.FindViewById<RecyclerView>(Resource.Id.recyclerAppUsers);
            searchView = view.FindViewById<SearchView>(Resource.Id.searchContacts);
            app_users_toolbar.SetNavigationIcon(Resource.Mipmap.ic_arrow_back_white_18dp);
            app_users_toolbar.Title = "GBV Users";
            app_users_toolbar.NavigationClick += App_users_toolbar_NavigationClick;
            //searchView.QueryTextSubmit += SearchView_QueryTextSubmit;

            


            
            

        }

        

        private void SearchView_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            items = (from data in TempList
                         where
                         data.Name.Contains(e.NewText) ||
                         data.EmailAddress.Contains(e.NewText) ||
                         data.PhoneNr.Contains(e.NewText)
                         select data)
                         .ToList();
            
            SetUpRecycler(items);

        }

        //private void SearchView_QueryTextSubmit(object sender, AndroidX.AppCompat.Widget.SearchView.QueryTextSubmitEventArgs e)
        //{
        //    data.GetAllUsers();
        //    data.RetrieveUsersHandler += Data_RetrieveUsersHandler;
        //    searchView.ClearFocus();
        //}

        private void SetUpRecycler(List<AppUsers> users)
        {
            
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context);
            recyclerAppUsers.SetLayoutManager(linearLayoutManager);
            AppUsersAdapter adapter = new AppUsersAdapter(users);
            recyclerAppUsers.SetAdapter(adapter);
            adapter.BtnClick += Adapter_BtnClick;
            
        }

        private void Adapter_BtnClick(object sender, AppUsersAdapterClickEventArgs e)
        {
            //FirebaseDatabase.Instance.GetReference("Request")
            //    .Child(items[e.Position].Keyid)
            //    .Child(FirebaseAuth.Instance.CurrentUser.Uid)
            //    .Child("Type")
            //    .SetValue("Invite");
            //FirebaseDatabase.Instance.GetReference("Request")
            //    .Child(FirebaseAuth.Instance.CurrentUser.Uid)
            //    .Child(items[e.Position].Keyid)
            //    .Child("Type")
            //    .SetValue("****");
            Toast.MakeText(context, e.Position.ToString() + items[e.Position].Name, ToastLength.Long).Show();
        }

        private void App_users_toolbar_NavigationClick(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Dismiss();
        }

    }
}