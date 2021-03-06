using System;

using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using GBV_Emergency_Response.Models;
using Google.Android.Material.Button;
using FFImageLoading;
using AndroidX.RecyclerView.Widget;
using Firebase.Auth;
using GBV_Emergency_Response.Filters;

namespace GBV_Emergency_Response.Adapters
{
    class AppUsersAdapter : RecyclerView.Adapter
    {
        public event EventHandler<AppUsersAdapterClickEventArgs> ItemClick;
        public event EventHandler<AppUsersAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<AppUsersAdapterClickEventArgs> BtnClick;
        private List<AppUsers> items = new List<AppUsers>();
        private readonly List<AppUsers> currentList = new List<AppUsers>();

        public AppUsersAdapter(List<AppUsers> data)
        {
            items = data;
            currentList = items;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.app_users_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new AppUsersAdapterViewHolder(itemView, OnClick, OnLongClick, OnBtnClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
             
            // Replace the contents of the view with that element
            var holder = viewHolder as AppUsersAdapterViewHolder;
            //holder.TextView.Text = items[position];
            holder.TxtFullNames.Text = items[position].Name;
            holder.TxtDisplayName.Text = items[position].Username;
            if (!string.IsNullOrEmpty(items[position].ImageUrl))
            {
                ImageService.Instance
                   .LoadUrl(items[position].ImageUrl)
                   .Retry(3, 200)
                   .DownSampleInDip(250, 250)
                   .Transform(new ImageTransformations.CircleTransformation())
                   .FadeAnimation(true, true, 300)
                   .IntoAsync(holder.ImgProfile);
            }
            
            if(items[position].Id == FirebaseAuth.Instance.CurrentUser.Uid)
            {
                holder.BtnAction.Visibility = ViewStates.Gone;
            }
            else
            {
                //if (items[position].FriendStatus != null)
                //{
                //    holder.BtnAction.Visibility = ViewStates.Gone;
                //}
            }
        }

        public override int ItemCount => items.Count;

        //public Filter Filter
        //{
        //    //get { return FilterHelper.newInstance(currentList.to, this); }
        //}

        void OnBtnClick(AppUsersAdapterClickEventArgs args) => BtnClick?.Invoke(this, args);
        void OnClick(AppUsersAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(AppUsersAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class AppUsersAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TxtDisplayName { get; set; }
        public TextView TxtFullNames { get; set; }
        public ImageView ImgProfile { get; set; }
        public MaterialButton BtnAction { get; set; }


        public AppUsersAdapterViewHolder(View itemView, Action<AppUsersAdapterClickEventArgs> clickListener,
                            Action<AppUsersAdapterClickEventArgs> longClickListener, Action<AppUsersAdapterClickEventArgs> btnItemAction) : base(itemView)
        {
            TxtDisplayName = itemView.FindViewById<TextView>(Resource.Id.user_row_display_name);
            TxtFullNames = itemView.FindViewById<TextView>(Resource.Id.user_row_names);
            BtnAction = itemView.FindViewById<MaterialButton>(Resource.Id.BtnFriendAction);
            ImgProfile = itemView.FindViewById<ImageView>(Resource.Id.row_user_image);
            //TextView = v;
            BtnAction.Click += (sender, e) => btnItemAction(new AppUsersAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.Click += (sender, e) => clickListener(new AppUsersAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new AppUsersAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class AppUsersAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}