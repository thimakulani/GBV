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
using Firebase.Auth;
using Firebase.Database;
using GBV_Emergency_Response.Models;

namespace GBV_Emergency_Response.AppDataHelper
{
    public class AlertsMessagesData : Java.Lang.Object, IValueEventListener
    {

        private List<AlertsMessages> items = new List<AlertsMessages>();
        private string UserKey;
        public AlertsMessagesData(string key)
        {
            UserKey = key;

        }

        public event EventHandler<AlertsHandler> RetrivedAlerts;
        public class AlertsHandler : EventArgs
        {
            public List<AlertsMessages> Items { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                items.Clear();
                foreach (DataSnapshot data in child)
                {
                    if (data.Child("UserKey").Value.ToString() != UserKey)
                    {
                        if (data.Child("Attended").Child(UserKey).Exists())
                        {
                            AlertsMessages alerts = new AlertsMessages
                            {
                                AlertId = data.Key,
                                Lat = data.Child("Latitude").Value.ToString(),
                                Lon = data.Child("Longitude").Value.ToString(),
                                Name = data.Child("Name").Value.ToString(),
                                Phone = data.Child("PhoneNr").Value.ToString(),
                                TimeDate = DateTime.Parse(data.Child("TimeDate").Value.ToString()),
                                UserKey = data.Child("UserKey").Value.ToString(),
                                Attended = true,

                            };
                            items.Add(alerts);
                        }
                        else
                        {
                            AlertsMessages alerts = new AlertsMessages
                            {
                                AlertId = data.Key,
                                Lat = data.Child("Latitude").Value.ToString(),
                                Lon = data.Child("Longitude").Value.ToString(),
                                Name = data.Child("Name").Value.ToString(),
                                Phone = data.Child("PhoneNr").Value.ToString(),
                                TimeDate = DateTime.Parse(data.Child("TimeDate").Value.ToString()),
                                UserKey = data.Child("UserKey").Value.ToString(),
                                Attended = false,

                            };
                            items.Add(alerts);
                        }


                    }
                }
                RetrivedAlerts.Invoke(this, new AlertsHandler { Items = items });
            }
        }

        public void GetAlerts()
        {
            FirebaseDatabase.Instance.GetReference("EmergencyAlert").AddValueEventListener(this);
        }
    }
}