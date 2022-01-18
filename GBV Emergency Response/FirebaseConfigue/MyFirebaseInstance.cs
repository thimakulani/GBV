using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBV_Emergency_Response.FirebaseConfigue
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseInstance: FirebaseInstanceIdService
    {

        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            SendTokenToServer(refreshedToken);
        }

        private void SendTokenToServer(string refreshed)
        {
            Log.Debug(PackageName, refreshed);
        }

    }
}