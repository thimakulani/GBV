using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using GBV_Emergency_Response.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBV_Emergency_Response.Dialogs
{
    public class HelpLineDialogFragment : DialogFragment
    {
        private readonly List<HelpLine> Items = new List<HelpLine>();
        private RecyclerView helpline_recycler;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        Context mContext;   
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            base.OnCreateView(inflater, container, savedInstanceState);
            View view=  inflater.Inflate(Resource.Layout.help_line_fragment, container, false);
            ConnectViews(view);
            mContext=view.Context;  
            return view;
        }

        private void ConnectViews(View view)
        {
            helpline_recycler = view.FindViewById<RecyclerView>(Resource.Id.helpline_recycler);
            Items.Add(new HelpLine { PhoneNumber = "10111", Title = "SAPS" });
            Items.Add(new HelpLine { PhoneNumber = "0800055555", Title = "Childline" });
            Items.Add(new HelpLine { PhoneNumber = "0800 428 428", Title = "GBV Hotline" });
            Items.Add(new HelpLine { PhoneNumber = "911", Title = "Emegency Number 1" });
            Items.Add(new HelpLine { PhoneNumber = "121", Title = "Emegency Number 2" });
            Items.Add(new HelpLine { PhoneNumber = "911", Title = "Fire Fighters" });
            helpline_recycler.SetLayoutManager(new LinearLayoutManager(view.Context));
            HelpAdapter adapter = new HelpAdapter(Items);
            helpline_recycler.SetAdapter(adapter);
            adapter.FabClick += Adapter_FabClick;
        }

        private void Adapter_FabClick(object sender, HelpAdapterClickEventArgs e)
        {
            try
            {
                var uri = Android.Net.Uri.Parse($"tel:{Items[e.Position].PhoneNumber}");
                var intent = new Intent(Intent.ActionDial, uri);
                StartActivity(intent); 
            }
            catch (Exception ex)
            {
                Toast.MakeText(mContext,ex.Message, ToastLength.Long).Show();
            } 

        }
    }
}
class HelpLine
{
    public string Title { get; set; }
    public string PhoneNumber { get; set; }
}