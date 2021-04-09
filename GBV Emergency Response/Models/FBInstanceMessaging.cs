using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using GBV_Emergency_Response.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBV_Emergency_Response.Models
{
  
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FBInstanceMessaging : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage remoteMessage)
        {
            if (remoteMessage.GetNotification() != null)
            {
                SendNotification(remoteMessage.GetNotification().Body, remoteMessage);
            }
            base.OnMessageReceived(remoteMessage);
        }


        void SendNotification(string messageBody, RemoteMessage remoteMessage)
        {
            var intent = new Intent(this, typeof(HomeActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0 /* Request code */, intent, PendingIntentFlags.OneShot);

            var defaultSoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            var notificationBuilder = new NotificationCompat.Builder(this, "MY_NOTIFICATION")
                .SetSmallIcon(Resource.Mipmap.iconfinder_Sed_16_2232599)
                .SetContentTitle(remoteMessage.GetNotification().Title)
                .SetContentText(messageBody)
                .SetAutoCancel(true)

                .SetBadgeIconType(Resource.Mipmap.iconfinder_Sed_16_2232599)
                .SetSound(defaultSoundUri)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);

            notificationManager.Notify(0 /* ID of notification */, notificationBuilder.Build());
        }
    }
}