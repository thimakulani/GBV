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
using Google.Android.Material.Chip;

namespace GBV_Emergency_Response.AppDataHelper
{
    public class ContactsData : Java.Lang.Object, IValueEventListener
    {
        private readonly List<AppUsers> items = new List<AppUsers>();
        public event EventHandler<ContactsEventHandler> RetrieveContactsHandler;
        public class ContactsEventHandler : EventArgs
        {
            public List<AppUsers> Items { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }
        public void GetContacts(string keyId)
        {
            FirebaseDatabase.Instance.GetReference("Contacts")
                .Child(keyId)
                .AddValueEventListener(this);
        }
        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                items.Clear();
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach(var data in child)
                {
                    string imgUrl = null;
                    if(data.Child("ImgUrl").Exists())
                    {
                        imgUrl = data.Child("ImgUrl").Value.ToString();
                    }
                    AppUsers contact = new AppUsers()
                    {
                        Name = data.Child("Name").Value.ToString(),
                        ImgUrl = imgUrl,
                        Keyid = data.Key

                    };
                    items.Add(contact);
                }
                RetrieveContactsHandler.Invoke(this, new ContactsEventHandler { Items = items });
            }
        }
    }
}