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

namespace GBV_Emergency_Response.AppDataHelper
{
    class ForumMessagesData: Java.Lang.Object, IValueEventListener
    {

        private List<ForumMessage> items = new List<ForumMessage>();

        public event EventHandler<RetriveMessages> RetrivedMsgs;
        public class RetriveMessages: EventArgs
        {
            public List<ForumMessage> Messages { get; set; }
        }
        public void CreateMessages()
        {
            FirebaseDatabase.Instance.GetReference("ChatForum").AddValueEventListener(this);
        }

        public void OnCancelled(DatabaseError error)
        {
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                items.Clear();
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach(DataSnapshot data in child)
                {
                    ForumMessage chats = new ForumMessage
                    {
                        Date_Time = DateTime.Parse(data.Child("DateTime").Value.ToString()),
                        Msg = data.Child("Message").Value.ToString(),
                        SenderName = data.Child("SenderName").Value.ToString(),
                        UserId = data.Child("SenderId").Value.ToString(),
                        KeyID = data.Key
                    };
                    items.Add(chats);
                }
                RetrivedMsgs.Invoke(this, new RetriveMessages { Messages = items });
            }
        }
    }
}