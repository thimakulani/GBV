using System;

using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using AndroidX.RecyclerView.Widget;
using GBV_Emergency_Response.Models;
using Google.Android.Material.Button;
using FFImageLoading;
using Plugin.CloudFirestore;
using Google.Android.Material.ImageView;
using Google.Android.Material.TextView;

namespace GBV_Emergency_Response.Adapters
{
    class AwarenessAdapter : RecyclerView.Adapter
    {
        public event EventHandler<AwarenessAdapterClickEventArgs> ItemClick;
        public event EventHandler<AwarenessAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<AwarenessAdapterClickEventArgs> ItemDeleteClick;
        private readonly List<AwarenessMessages> items = new List<AwarenessMessages>();
        private readonly string KeyId;
        public AwarenessAdapter(List<AwarenessMessages> data, string Key)
        {
            items = data;
            KeyId = Key;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.awareness_row, parent, false);
            var vh = new AwarenessAdapterViewHolder(itemView, OnClick, OnLongClick, OnDeleteClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            //var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as AwarenessAdapterViewHolder;
            //holder.TextView.Text = items[position];
            if(string.IsNullOrWhiteSpace(items[position].ImageUrl))
            {
                holder.ImgAwareness.Visibility = ViewStates.Gone;
            }
            else
            {
                ImageService.Instance.LoadUrl(items[position].ImageUrl)
                    .Retry(3, 500)
                    .IntoAsync(holder.ImgAwareness);
            }
            if (KeyId == items[position].Uid)
            {
                holder.BtnDeleteAwareness.Visibility = ViewStates.Visible;
            }
            else
            {
                holder.BtnDeleteAwareness.Visibility = ViewStates.Gone;

            }
            holder.AwarenessMsg.Text = items[position].Message;
            holder.Sender.Text = $"{items[position].Uid} :{items[position].Dates}";

            CrossCloudFirestore
                .Current
                .Instance
                .Collection("PEOPLE")
                .Document(items[position].Uid)
                .AddSnapshotListener((value, error) =>
                {
                    if (value.Exists)
                    {
                        var users = value.ToObject<AppUsers>();
                        holder.Sender.Text = $"{users.Name} {users.Surname}";
                        holder.Dates.Text = $"{items[position].Dates.ToDateTime():ddd dd-MMM-yyyy HH:mm tt}";
                    }
                });


        }

        public override int ItemCount => items.Count;

        void OnClick(AwarenessAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(AwarenessAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnDeleteClick(AwarenessAdapterClickEventArgs args) => ItemDeleteClick?.Invoke(this, args);
    }

    public class AwarenessAdapterViewHolder : RecyclerView.ViewHolder
    {
        
        public MaterialTextView AwarenessMsg { get; set; }
        public MaterialTextView Sender { get; set; }
        public MaterialTextView Dates { get; set; }
        public MaterialButton BtnDeleteAwareness { get; set; }
        public ShapeableImageView ImgAwareness { get; set; }


        public AwarenessAdapterViewHolder(View itemView, Action<AwarenessAdapterClickEventArgs> clickListener,
                            Action<AwarenessAdapterClickEventArgs> longClickListener, Action<AwarenessAdapterClickEventArgs> deleteClickListener) : base(itemView)
        {
            //TextView = v;
            Dates = itemView.FindViewById<MaterialTextView>(Resource.Id.txtDates);
            AwarenessMsg = itemView.FindViewById<MaterialTextView>(Resource.Id.txtAwareessmessage);
            Sender = itemView.FindViewById<MaterialTextView>(Resource.Id.txtAwarenessSender);
            BtnDeleteAwareness = itemView.FindViewById<MaterialButton>(Resource.Id.BtnDeleteAwareness);
            ImgAwareness = itemView.FindViewById<ShapeableImageView>(Resource.Id.awareness_img);


            itemView.Click += (sender, e) => clickListener(new AwarenessAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new AwarenessAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            BtnDeleteAwareness.Click += (sender, e) => deleteClickListener(new AwarenessAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class AwarenessAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}