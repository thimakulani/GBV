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
using Plugin.CloudFirestore;
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


            adapter = new InvitesAdapter(Items);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context);
            recycler.SetLayoutManager(linearLayoutManager);
            recycler.SetAdapter(adapter);
            adapter.NotifyDataSetChanged();
            adapter.BtnClick += Adapter_BtnClick;
            LoadInvites();

        }
        InvitesAdapter adapter;

        private void App_invites_toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
        {
            Dismiss();
        }

        public List<InviteModel> Items = new List<InviteModel>();

        private void Adapter_BtnClick(object sender, InvitesAdapterClickEventArgs e)
        {

            

        }
        private void LoadInvites()
        {
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("REQUESTS")
                .WhereEqualsTo("Fid", FirebaseAuth.Instance.CurrentUser.Uid)
                .AddSnapshotListener((value, error) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var item in value.DocumentChanges)
                        {
                            switch (item.Type)
                            {
                                case DocumentChangeType.Added:
                                    Items.Add(item.Document.ToObject<InviteModel>());
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                });
        }
    }
}
