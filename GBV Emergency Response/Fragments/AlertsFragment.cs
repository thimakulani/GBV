using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using GBV_Emergency_Response.Adapters;
using GBV_Emergency_Response.Models;
using Plugin.CloudFirestore;
using Xamarin.Essentials;

namespace GBV_Emergency_Response.Fragments
{
    public class AlertsFragment : Fragment
    {
        private RecyclerView recyclerAlerts;
        private List<AlertsMessages> items = new List<AlertsMessages>();
        private Context context;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.alerts_fragment, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            recyclerAlerts = view.FindViewById<RecyclerView>(Resource.Id.recyclerAlerts);


            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context);
            linearLayoutManager.StackFromEnd = true;
            linearLayoutManager.ReverseLayout = true;
            AlertsAdapter adapter = new AlertsAdapter(items, context);
            recyclerAlerts.SetLayoutManager(linearLayoutManager);
            recyclerAlerts.SetAdapter(adapter);
            adapter.NotifyDataSetChanged();
            adapter.BtnNavClick += Adapter_BtnNavClick;
            adapter.ItemClick += Adapter_ItemClick;
            adapter.FabCallClick += Adapter_FabCallClick;

            
            CrossCloudFirestore.Current.Instance
                .Collection("EMERGENCY")
                .OrderBy("TimeDate", false)
                .AddSnapshotListener((value, errors) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var dc in value.DocumentChanges)
                        {
                            var alert = dc.Document.ToObject<AlertsMessages>();
                            switch (dc.Type)
                            {
                                case DocumentChangeType.Added:
                                    items.Add(alert);
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    items[dc.OldIndex] = alert;
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Removed:
                                    items.RemoveAt(dc.OldIndex);
                                    adapter.NotifyDataSetChanged();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                });

            
        }
        public event EventHandler<ShowMapFragmentArgs> ShowMapHandler;
        public  class ShowMapFragmentArgs : EventArgs
        {
            public AlertsMessages Alerts { get; set; }
            public int Type { get; set; }
        }
        private void Adapter_ItemClick(object sender, AlertsAdapterClickEventArgs e)
        {
            ShowMapHandler.Invoke(this, new ShowMapFragmentArgs { Alerts = items[e.Position], Type = 1 });

        }

        private void Adapter_FabCallClick(object sender, AlertsAdapterClickEventArgs e)
        {
            Xamarin.Essentials.PhoneDialer.Open(items[e.Position].Phone);
        }

        private void Adapter_BtnNavClick(object sender, AlertsAdapterClickEventArgs e)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

            ShowMapHandler.Invoke(this, new ShowMapFragmentArgs { Type = 2, Alerts = items[e.Position] });

           // await OpenMap.NavigateToVictim(double.Parse(items[e.Position].Lat), double.Parse(items[e.Position].Lon));
        }

       
    }
    public static class OpenMap
    {
        public static async Task NavigateToVictim(double lat, double lon)
        {
            
            var location = new Xamarin.Essentials.Location(lat, lon);
            var options = new MapLaunchOptions { Name = "Victim", };

            try
            {
                await Map.OpenAsync(location, options);
            }
            catch(Exception ex)
            {
                // No map application available to open
                Console.WriteLine(ex.Message);
            }
        }
    }
}