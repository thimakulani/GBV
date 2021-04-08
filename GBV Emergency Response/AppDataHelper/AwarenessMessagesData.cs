using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using GBV_Emergency_Response.Models;
using XHUD;

namespace GBV_Emergency_Response.AppDataHelper
{
    public class AwarenessMessagesData : Java.Lang.Object, IValueEventListener
    {
        private List<AwarenessMessages> items = new List<AwarenessMessages>();
        public event EventHandler<AwarenessHandler> RetrivedAwareness;
        public class AwarenessHandler : EventArgs
        {
            public List<AwarenessMessages> Awarenesses { get; set; }
        }
        public void GetAwareness()
        {
            FirebaseDatabase.Instance.GetReference("Awareness")
                .AddValueEventListener(this);
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                items.Clear();
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach (DataSnapshot data in child)
                {
                    string imgUrl = null;
                    if(data.Child("ImgUrl").Exists())
                    {
                        imgUrl = data.Child("ImgUrl").Value.ToString();
                    }
                    AwarenessMessages awareness = new AwarenessMessages
                    {
                        AwarenessMsg = data.Child("Message").Value.ToString(),
                        MsgId = data.Key,
                        Sender = data.Child("SenderName").Value.ToString(),
                        SenderId = data.Child("SenderId").Value.ToString(),
                        Dates = data.Child("Dates").Value.ToString(),
                        ImgUrl = imgUrl
                    };
                    items.Add(awareness);
                }
                RetrivedAwareness.Invoke(this, new AwarenessHandler { Awarenesses = items });
            }
            
        }
    }
}