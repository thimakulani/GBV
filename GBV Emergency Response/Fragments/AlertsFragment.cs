using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Firebase.Auth;
using GBV_Emergency_Response.Adapters;
using GBV_Emergency_Response.Models;
using Xamarin.Essentials;

namespace GBV_Emergency_Response.Fragments
{
    public class AlertsFragment : HelpFragment
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
            adapter.FabCallClick += Adapter_FabCallClick;


        }

      

        private void Adapter_FabCallClick(object sender, AlertsAdapterClickEventArgs e)
        {
            Xamarin.Essentials.PhoneDialer.Open(items[e.Position].Phone);
        }

        private async void Adapter_BtnNavClick(object sender, AlertsAdapterClickEventArgs e)
        {
            await OpenMap.NavigateToVictim(Double.Parse(items[e.Position].Lat), Double.Parse(items[e.Position].Lon));
        }

       
    }
    public static class OpenMap
    {
        public static async Task NavigateToVictim(double lat, double lon)
        {
            var location = new Xamarin.Essentials.Location(lat, lon);
            var options = new MapLaunchOptions { Name = "Victim" };

            try
            {
                await Map.OpenAsync(location, options);
            }
            catch
            {
                // No map application available to open
            }
        }
    }
}