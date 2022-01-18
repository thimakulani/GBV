using System;

using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using Android.Locations;
using Xamarin.Essentials;
using AndroidX.RecyclerView.Widget;
using GBV_Emergency_Response.Models;
using Google.Android.Material.Button;
using Plugin.CloudFirestore;

namespace GBV_Emergency_Response.Adapters
{
    class AlertsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<AlertsAdapterClickEventArgs> ItemClick;
        public event EventHandler<AlertsAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<AlertsAdapterClickEventArgs> BtnNavClick;
        public event EventHandler<AlertsAdapterClickEventArgs> FabCallClick;

        readonly List<AlertsMessages> items = new List<AlertsMessages>();
        private readonly Context adapterContext;
        public AlertsAdapter(List<AlertsMessages>  data, Context context)
        {
            items = data;
            adapterContext = context;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.alerts_row, parent, false);
            var vh = new AlertsAdapterViewHolder(itemView, OnClick, OnLongClick, OnBtnNav, OnFabCall);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            // Replace the contents of the view with that element

            var holder = viewHolder as AlertsAdapterViewHolder;

            TimeSpan timeSpan = DateTime.UtcNow - items[position].TimeDate.ToDateTime();
            if (timeSpan.TotalMinutes > 30)
            {
                holder.BtnNavigate.Visibility = ViewStates.Gone;
                holder.FabCall.Visibility = ViewStates.Gone;
            }
            if(items[position].TimeDate.ToDateTime().ToString("dd/MMM/yyyy") == DateTime.UtcNow.ToString("dd/MMM/yyyy"))
            {
                holder.TxtTimeDate.Text = "today: " +  items[position].TimeDate.ToDateTime().ToString("HH:mm tt");
            }
            else
            {
                holder.TxtTimeDate.Text = items[position].TimeDate.ToDateTime().ToString("ddd, dd/MMM/yyyy HH:mm tt");
            }

            double lat = Convert.ToDouble(items[position].Lat);
            double log = Convert.ToDouble(items[position].Lon);
            if(await GetLocationAsync(lat, log) == null)
            {

                holder.TxtLocation.Text = await GetLocationAsync(lat, log);
            }
            else
            {
                holder.TxtLocation.Text = await GetLocationAsync(lat, log);
            }

            CrossCloudFirestore
               .Current
               .Instance
               .Collection("PEOPLE")
               .Document(items[position].UserKey)
               .AddSnapshotListener((value, error) =>
               {
                   try
                   {

                       if (value.Exists)
                       {
                           var users = value.ToObject<AppUsers>();

                           holder.TxtName.Text = users.Username;
                           holder.TxtPhone.Text = users.PhoneNumber;

                       }
                       else { Console.WriteLine("Nonsonse!!!"); }
                   }catch(Exception ex)
                   {
                       Console.WriteLine(ex.Message);
                   }
               });
        }
        private async System.Threading.Tasks.Task<string> GetLocationAsync(double lat, double lon)
        {
            try
            {

                Geocoder geocoder = new Geocoder(adapterContext.ApplicationContext);
                var address = await geocoder.GetFromLocationAsync(lat, lon, 1);
                System.Text.StringBuilder s = new System.Text.StringBuilder();
                if (!string.IsNullOrEmpty(address[0].SubThoroughfare) && !string.IsNullOrWhiteSpace(address[0].SubThoroughfare))
                {
                    s.Append(address[0].SubThoroughfare);
                }
                if (!string.IsNullOrEmpty(address[0].Thoroughfare) && !string.IsNullOrWhiteSpace(address[0].Thoroughfare))
                {
                    if (!string.IsNullOrEmpty(address[0].SubThoroughfare) && !string.IsNullOrWhiteSpace(address[0].SubThoroughfare))
                    {
                        s.Append(", ");
                    }
                    s.Append(address[0].Thoroughfare);
                }
                if (!string.IsNullOrEmpty(address[0].SubLocality) && !string.IsNullOrWhiteSpace(address[0].SubLocality))
                {
                    if (!string.IsNullOrEmpty(address[0].Thoroughfare) && !string.IsNullOrWhiteSpace(address[0].Thoroughfare))
                    {
                        s.Append(", ");
                    }

                    s.Append(address[0].SubLocality);
                }
                if (!string.IsNullOrEmpty(address[0].Locality) && !string.IsNullOrWhiteSpace(address[0].Locality))
                {
                    if (!string.IsNullOrEmpty(address[0].SubLocality) && !string.IsNullOrWhiteSpace(address[0].SubLocality))
                    {
                        s.Append(", ");
                    }
                    s.Append(address[0].Locality);
                }
                if (!string.IsNullOrEmpty(address[0].SubAdminArea) && !string.IsNullOrWhiteSpace(address[0].SubAdminArea))
                {
                    if (!string.IsNullOrEmpty(address[0].Thoroughfare) && !string.IsNullOrWhiteSpace(address[0].Thoroughfare))
                    {
                        s.Append(", ");
                    }
                    s.Append(address[0].SubAdminArea);
                }
                if (!string.IsNullOrEmpty(address[0].AdminArea) && !string.IsNullOrWhiteSpace(address[0].AdminArea))
                {
                    if (!string.IsNullOrEmpty(address[0].SubAdminArea) && !string.IsNullOrWhiteSpace(address[0].SubAdminArea))
                    {
                        s.Append(", ");
                    }
                    s.Append(address[0].AdminArea);
                }
                if (!string.IsNullOrEmpty(address[0].Premises) && !string.IsNullOrWhiteSpace(address[0].Premises))
                {
                    if (!string.IsNullOrEmpty(address[0].AdminArea) && !string.IsNullOrWhiteSpace(address[0].AdminArea))
                    {
                        s.Append(", ");
                    }
                    s.Append(address[0].Premises);
                }
                return s.ToString();
            }
            catch(Exception)
            {

            }
            return string.Empty;
        }
        public override int ItemCount => items.Count;

        void OnFabCall(AlertsAdapterClickEventArgs args) => FabCallClick?.Invoke(this, args);
        void OnBtnNav(AlertsAdapterClickEventArgs args) => BtnNavClick?.Invoke(this, args);
        void OnClick(AlertsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(AlertsAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class AlertsAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView TxtTimeDate { get; set; }
        public TextView TxtName { get; set; }
        public TextView TxtLocation { get; set; }
        public TextView TxtPhone { get; set; }
        public MaterialButton BtnNavigate { get; set; }
        public View View_separator { get; set; }
        public Google.Android.Material.FloatingActionButton.FloatingActionButton FabCall { get; set; }

        public AlertsAdapterViewHolder(View itemView, Action<AlertsAdapterClickEventArgs> clickListener,
                            Action<AlertsAdapterClickEventArgs> longClickListener,
                            Action<AlertsAdapterClickEventArgs> navClickListener,
                            Action<AlertsAdapterClickEventArgs> FabCallClickListener) : base(itemView)
        {
            //TextView = v;
            TxtTimeDate = itemView.FindViewById<TextView>(Resource.Id.RowDateTime);
            TxtName = itemView.FindViewById<TextView>(Resource.Id.Row_User_Name);
            TxtLocation = itemView.FindViewById<TextView>(Resource.Id.Row_Location);
            TxtPhone = itemView.FindViewById<TextView>(Resource.Id.Row_Phone_Number);
            BtnNavigate = itemView.FindViewById<MaterialButton>(Resource.Id.BtnNavigate);
            View_separator = itemView.FindViewById<View>(Resource.Id.view_separator);
            FabCall = itemView.FindViewById<Google.Android.Material.FloatingActionButton.FloatingActionButton>(Resource.Id.FabCall);

            FabCall.Click += (sender, e) => FabCallClickListener(new AlertsAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            BtnNavigate.Click += (sender, e) => navClickListener(new AlertsAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.Click += (sender, e) => clickListener(new AlertsAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new AlertsAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class AlertsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}