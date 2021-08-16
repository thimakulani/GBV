using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using FFImageLoading;
using Firebase.Database;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBV_Emergency_Response.Dialogs
{
    public class FriendDialogFragment : DialogFragment
    {
        private MaterialToolbar toolbar_friend;
        private readonly string id;
        private MaterialTextView Names;
        private MaterialTextView Surname;
        private MaterialTextView Phone;
        private AppCompatImageView ImgCover;
        private AppCompatImageView ImgProfile;
        private FloatingActionButton FabCallFriend;

        public FriendDialogFragment(string id)
        {
            this.id = id;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            SetStyle(StyleNoFrame, Resource.Style.FullScreenDialogStyle);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.fragment_friends_profile, container, false);
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            toolbar_friend = view.FindViewById<MaterialToolbar>(Resource.Id.toolbar_friend);
            toolbar_friend.SetNavigationIcon(Resource.Mipmap.ic_arrow_back_white_18dp);
            toolbar_friend.NavigationClick += Toolbar_friend_NavigationClick;

            Names = view.FindViewById<MaterialTextView>(Resource.Id.txt_profile_name);
            Surname = view.FindViewById<MaterialTextView>(Resource.Id.txt_profile_surname);
            Phone = view.FindViewById<MaterialTextView>(Resource.Id.txt_profile_phone_number);
            ImgCover = view.FindViewById<AppCompatImageView>(Resource.Id.F_ProfileImgCover);
            ImgProfile = view.FindViewById<AppCompatImageView>(Resource.Id.F_ProfileImage);

            FabCallFriend = view.FindViewById<FloatingActionButton>(Resource.Id.FabCallFriend);
            FabCallFriend.Click += FabCallFriend_Click;
            
        }

        private void FabCallFriend_Click(object sender, EventArgs e)
        {
            try
            {
                Xamarin.Essentials.PhoneDialer.Open(Phone.Text);
            }
            catch (Exception)
            {

            }
            
        }

        private void Toolbar_friend_NavigationClick(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Dismiss();
        }


    }
}