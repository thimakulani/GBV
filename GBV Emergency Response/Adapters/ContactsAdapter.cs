using System;

using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using GBV_Emergency_Response.Models;
using AndroidX.RecyclerView.Widget;

namespace GBV_Emergency_Response.Adapters
{
    class ContactsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ContactsAdapterClickEventArgs> ItemClick;
        public event EventHandler<ContactsAdapterClickEventArgs> ItemLongClick;
        private readonly List<AppUsers> items = new List<AppUsers>();

        public ContactsAdapter(List<AppUsers> data)
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

            holder.Name.Text = $"{items[position].Name} {items[position].Surname}";





        }

        public override int ItemCount => items.Count;

        void OnClick(ContactsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ContactsAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        
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
           
            itemView.Click += (sender, e) => clickListener(new ContactsAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new ContactsAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class ContactsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}