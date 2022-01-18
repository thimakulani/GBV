using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidAboutPage;
using Android.App;

namespace GBV_Emergency_Response.Fragments
{
    public class AboutFragment : AndroidX.Fragment.App.Fragment
    {
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
            Element element = new Element();
            element.Title = "About";

            View about = new AboutPage(Application.Context)
                .IsRtl(false)
               // .SetImage(Resource.Drawable.gbv_image_splash)
                .SetDescription("GBV")
                .AddItem(new Element() { Title = "Version 1.0.0" })
                .AddItem(element)
                .AddGroup("Contact us on")
                .AddEmail("andriessebola001@gmail.com")
                .AddFacebook("Andries Mpontseng Sebola")
                // .AddWebsite("")
                .AddPlayStore("not availabe")
                .AddItem(CreateCopyright())
                .AddGroup("Developers")
                .AddEmail("andriessebola@gmail.com")
                .AddFacebook("https//m.facebook.com/andriesmpontsheng")
                .AddGitHub("Andries-ui")
                .AddInstagram("not available")
                .Create();
            return about;
        }
        private Element CreateCopyright()
        {
            Element copy = new Element();
            string cr = $"Copyright {DateTime.Now.Year} by Thima Kulani";
            copy.Title = cr;
            //copy.IconDrawable = Resource.Drawable.gbv_image_splash;
            return copy;
        }
    }
}