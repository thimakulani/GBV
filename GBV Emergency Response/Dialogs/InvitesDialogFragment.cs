using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Firebase.Auth;
using GBV_Emergency_Response.Adapters;
using GBV_Emergency_Response.Models;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace GBV_Emergency_Response.Dialogs
{
    public class InvitesDialogFragment : DialogFragment
    {
        private Toolbar app_invites_toolbar;
        private RecyclerView recycler;
        private Context context;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetStyle(StyleNoFrame, Resource.Style.FullScreenDialogStyle);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.invites_users_dialog, container, false);
            ConnectView(view);
            context = view.Context;

            return view;
        }
        private void ConnectView(View view)
        {
            app_invites_toolbar = view.FindViewById<Toolbar>(Resource.Id.app_invites_toolbar);
            recycler = view.FindViewById<RecyclerView>(Resource.Id.recyclerInvites);
            app_invites_toolbar.SetNavigationIcon(Resource.Mipmap.ic_arrow_back_white_18dp);
            app_invites_toolbar.Title = "Invites";
            app_invites_toolbar.NavigationClick += App_invites_toolbar_NavigationClick;


            InvitesAdapter adapter = new InvitesAdapter(items);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context);
            recycler.SetLayoutManager(linearLayoutManager);
            recycler.SetAdapter(adapter);
            adapter.BtnClick += Adapter_BtnClick;

        }

        private void App_invites_toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
        {
            Dismiss();
        }

        List<InviteModel> items = new List<InviteModel>();

        private void Adapter_BtnClick(object sender, InvitesAdapterClickEventArgs e)
        {
            //if(items[e.Position].Status == "Invite")
            //{
            //    FirebaseDatabase.Instance.GetReference("Request")
            //        .Child(items[e.Position].Key)
            //        .Child(FirebaseAuth.Instance.CurrentUser.Uid)
            //        .Child("Type")
            //        .SetValue("Approved");
            //    FirebaseDatabase.Instance.GetReference("Request")
            //        .Child(FirebaseAuth.Instance.CurrentUser.Uid)
            //        .Child(items[e.Position].Key)
            //        .Child("Type")
            //        .SetValue("Approved");
            //}
            //if (items[e.Position].Status == "Approved")
            //{
            //    AndroidX.AppCompat.App.AlertDialog.Builder builder = new AndroidX.AppCompat.App.AlertDialog.Builder(context);
            //    builder.SetTitle("Confirm");
            //    builder.SetMessage("Are you sure you want to delete");
            //    builder.SetNegativeButton("No", delegate
            //    {
            //        builder.Dispose();
            //    });
            //    builder.SetPositiveButton("Yes", delegate
            //    {
            //        FirebaseDatabase.Instance.GetReference("Request")
            //            .Child(items[e.Position].Key)
            //            .Child(FirebaseAuth.Instance.CurrentUser.Uid)
            //            .RemoveValue();

            //        FirebaseDatabase.Instance.GetReference("Request")
            //            .Child(FirebaseAuth.Instance.CurrentUser.Uid)
            //            .Child(items[e.Position].Key)
            //            .RemoveValue();
            //        builder.Dispose();
            //    });
            //    builder.Show();


            //}
            //if (items[e.Position].Status == "****")
            //{
            //    AndroidX.AppCompat.App.AlertDialog.Builder builder = new AndroidX.AppCompat.App.AlertDialog.Builder(context);
            //    builder.SetTitle("Confirm");
            //    builder.SetMessage("Are you sure you want to cancel");
            //    builder.SetNegativeButton("No", delegate
            //    {
            //        builder.Dispose();
            //    });
            //    builder.SetPositiveButton("Yes", delegate
            //    {
            //        FirebaseDatabase.Instance.GetReference("Request")
            //            .Child(items[e.Position].Key)
            //            .Child(FirebaseAuth.Instance.CurrentUser.Uid)
            //            .RemoveValue();

            //        FirebaseDatabase.Instance.GetReference("Request")
            //            .Child(FirebaseAuth.Instance.CurrentUser.Uid)
            //            .Child(items[e.Position].Key)
            //            .RemoveValue();
            //        builder.Dispose();
            //    });
            //    builder.Show();


        }
    }
}
