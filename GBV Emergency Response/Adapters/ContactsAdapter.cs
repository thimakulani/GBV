using System;

using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using GBV_Emergency_Response.Models;
using FFImageLoading;
using AndroidX.RecyclerView.Widget;
using Firebase.Database;

namespace GBV_Emergency_Response.Adapters
{
    class ContactsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ContactsAdapterClickEventArgs> ItemClick;
        public event EventHandler<ContactsAdapterClickEventArgs> ItemLongClick;
        private readonly List<InviteModel> items = new List<InviteModel>();

        public ContactsAdapter(List<InviteModel> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.contacts_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new ContactsAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var holder = viewHolder as ContactsAdapterViewHolder;


            FirebaseDatabase.Instance.GetReference("Users").Child(items[position].Key)
                .AddValueEventListener(new ValueAdapterEventListener(holder));
        }

        public override int ItemCount => items.Count;

        void OnClick(ContactsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ContactsAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        private class ValueAdapterEventListener : Java.Lang.Object, IValueEventListener
        {
            private readonly ContactsAdapterViewHolder holder;

            public ValueAdapterEventListener(ContactsAdapterViewHolder holder)
            {
                this.holder = holder;
            }

            public void OnCancelled(DatabaseError error)
            {
                
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                if (snapshot.Exists())
                {

                    holder.Name.Text = snapshot.Child("Name").Value.ToString() + " " + snapshot.Child("Surname").Value.ToString();
                    
                    if (snapshot.Child("ImgUrl").Exists())
                    {
                        ImageService.Instance
                           .LoadUrl(snapshot.Child("ImgUrl").Value.ToString())
                           .Retry(3, 200)
                           .DownSampleInDip(250, 250)
                           .Transform(new ImageTransformations.CircleTransformation())
                           .FadeAnimation(true, true, 300)
                           .IntoAsync(holder.Img);
                    }
                }
            }
        }
    }

    public class ContactsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; set; }
        public ImageView Img { get; set; }


        public ContactsAdapterViewHolder(View itemView, Action<ContactsAdapterClickEventArgs> clickListener,
                            Action<ContactsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v; 
            Name = itemView.FindViewById<TextView>(Resource.Id.contact_names);
            Img = itemView.FindViewById<ImageView>(Resource.Id.contact_image);
           
            itemView.Click += (sender, e) => clickListener(new ContactsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new ContactsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class ContactsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}