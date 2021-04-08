using System;
using System.Collections.Generic;
using Android.Gms.Tasks;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using Firebase.Database;
using GBV_Emergency_Response.Models;
using Google.Android.Material.Button;

namespace GBV_Emergency_Response.Adapters
{
    class InvitesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<InvitesAdapterClickEventArgs> BtnClick;
        public event EventHandler<InvitesAdapterClickEventArgs> ItemClick;
        public event EventHandler<InvitesAdapterClickEventArgs> ItemLongClick;
        List<InviteModel> items = new List<InviteModel>();
        DatabaseReference dbRef;
        public InvitesAdapter(List<InviteModel> data)
        {
            items = data;
           
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.invite_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new InvitesAdapterViewHolder(itemView, OnClick, OnLongClick, OnBtnClick);
            return vh;
        }
        

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            // Replace the contents of the view with that element
            var holder = viewHolder as InvitesAdapterViewHolder;
            //holder.TextView.Text = items[position];
            if(item.Status == "****")
            {
                holder.BtnAction.Text = "Cancel Request";
            }
            if(item.Status == "Approved")
            {
                holder.BtnAction.Text = "Remove";
            }
            if (item.Status == "Invite")
            {
                holder.BtnAction.Text = "Accept";
            }
            dbRef = FirebaseDatabase.Instance.GetReference("Users");
            dbRef.Child(item.Key).AddValueEventListener(new ValueEventListener(this, holder));

        }

        public override int ItemCount => items.Count;

        void OnBtnClick(InvitesAdapterClickEventArgs args) => BtnClick?.Invoke(this, args);
        void OnClick(InvitesAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(InvitesAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    internal class ValueEventListener :Java.Lang.Object, IValueEventListener
    {
        private InvitesAdapter invitesAdapter;
        private InvitesAdapterViewHolder holder;

        public ValueEventListener(InvitesAdapter invitesAdapter, InvitesAdapterViewHolder holder)
        {
            this.invitesAdapter = invitesAdapter;
            this.holder = holder;
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Exists())
            {
                
                holder.TxtFullNames.Text = snapshot.Child("Name").Value.ToString() + " " + snapshot.Child("Surname").Value.ToString();
                if (snapshot.Child("Username").Exists())
                {
                    holder.TxtDisplayName.Text = snapshot.Child("Username").Value.ToString();
                }
                else
                {
                    holder.TxtDisplayName.Text = string.Empty;
                }
                if (snapshot.Child("ImgUrl").Exists())
                {
                    ImageService.Instance
                       .LoadUrl(snapshot.Child("ImgUrl").Value.ToString())
                       .Retry(3, 200)
                       .DownSampleInDip(250, 250)
                       .Transform(new ImageTransformations.CircleTransformation())
                       .FadeAnimation(true, true, 300)
                       .IntoAsync(holder.ImgProfile);
                }
            }
        }
    }

    public class InvitesAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView TxtDisplayName { get; set; }
        public TextView TxtFullNames { get; set; }
        public ImageView ImgProfile { get; set; }
        public MaterialButton BtnAction { get; set; }

        public InvitesAdapterViewHolder(View itemView, Action<InvitesAdapterClickEventArgs> clickListener,
                            Action<InvitesAdapterClickEventArgs> longClickListener, Action<InvitesAdapterClickEventArgs> btnActionClickListner) : base(itemView)
        {
            TxtDisplayName = itemView.FindViewById<TextView>(Resource.Id.invite_row_display_name);
            TxtFullNames = itemView.FindViewById<TextView>(Resource.Id.invite_row_names);
            BtnAction = itemView.FindViewById<MaterialButton>(Resource.Id.BtnInviteAction);
            ImgProfile = itemView.FindViewById<ImageView>(Resource.Id.row_invite_image);
            //TextView = v;
            BtnAction.Click += (sender, e) => btnActionClickListner(new InvitesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.Click += (sender, e) => clickListener(new InvitesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new InvitesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class InvitesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}