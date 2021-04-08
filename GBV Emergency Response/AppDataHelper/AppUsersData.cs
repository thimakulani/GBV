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
    public class AppUsersData:Java.Lang.Object, IValueEventListener
    {
        private List<AppUsers> items = new List<AppUsers>();
        private List<InviteModel> inviteItems = new List<InviteModel>();
        public event EventHandler<AppUsersEventHandler> RetrieveUsersHandler;
        public class AppUsersEventHandler : EventArgs
        {
            public List<AppUsers> Items { get; set; }
        }
        public void GetAllUsers(List<InviteModel> inviteItems)
        {
            this.inviteItems = inviteItems;
            FirebaseDatabase.Instance.GetReference("Users")
                .AddValueEventListener(this);
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                items.Clear();
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                
                foreach (var data in child)
                {
                    string friendStatus = null;
                    if (inviteItems.Count > 0)
                    {
                        if (inviteItems.Any(d => d.Key == data.Key))
                        {
                            friendStatus = "****";
                        }
                    }
                     
                    string imgUrl = null;
                    if (data.Child("ImgUrl").Exists())
                    {
                        imgUrl = data.Child("ImgUrl").Value.ToString();
                    }
                    string username = null;
                    if (data.Child("Username").Exists())
                    {
                        username = data.Child("Username").Value.ToString();
                    }
                    AppUsers contact = new AppUsers()
                    {
                        Name = $"{data.Child("Name").Value} {data.Child("Surname").Value}",
                        ImgUrl = imgUrl,
                        DisplayName = username,
                        PhoneNr = data.Child("PhoneNumber").Value.ToString(),
                        EmailAddress = data.Child("Email").Value.ToString(),
                        Keyid = data.Key,
                        FriendStatus = friendStatus,
                    };
                    items.Add(contact);
                }
                RetrieveUsersHandler.Invoke(this, new AppUsersEventHandler { Items = items });
            }
            FirebaseDatabase.Instance.GetReference("Users").RemoveEventListener(this);
        }
    }
}