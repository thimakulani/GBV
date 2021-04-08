using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextView;
using System;
using System.Collections.Generic;

namespace GBV_Emergency_Response.Adapters
{
    class HelpAdapter : RecyclerView.Adapter
    {
        public event EventHandler<HelpAdapterClickEventArgs> ItemClick;
        public event EventHandler<HelpAdapterClickEventArgs> FabClick;
        public event EventHandler<HelpAdapterClickEventArgs> ItemLongClick;
        private List<HelpLine> Items = new List<HelpLine>();

        public HelpAdapter(List<HelpLine> data)
        {
            Items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.helpline_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new HelpAdapterViewHolder(itemView, OnClick, OnLongClick, OnFabClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as HelpAdapterViewHolder;
            //holder.TextView.Text = items[position];
            holder.Row_Title.Text = Items[position].Title;
        }

        public override int ItemCount => Items.Count;

        void OnFabClick(HelpAdapterClickEventArgs args) => FabClick?.Invoke(this, args);
        void OnClick(HelpAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(HelpAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class HelpAdapterViewHolder : RecyclerView.ViewHolder
    {
        public MaterialTextView Row_Title { get; set; }
        public FloatingActionButton Row_FabCall { get; set; }


        public HelpAdapterViewHolder(View itemView, Action<HelpAdapterClickEventArgs> clickListener,
                            Action<HelpAdapterClickEventArgs> longClickListener,
                            Action<HelpAdapterClickEventArgs> fabClickListener) : base(itemView)
        {
            //TextView = v;
            Row_FabCall = itemView.FindViewById<FloatingActionButton>(Resource.Id.row_help_call);
            Row_Title = itemView.FindViewById<MaterialTextView>(Resource.Id.row_help_title);
            Row_FabCall.Click += (sender, e) => fabClickListener(new HelpAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.Click += (sender, e) => clickListener(new HelpAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new HelpAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class HelpAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}