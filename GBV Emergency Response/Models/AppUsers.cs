using System;

using Android.Views;
using Android.Widget;

namespace GBV_Emergency_Response.Models
{
    public class AppUsers
    {
        public string Keyid { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string ImgUrl { get; set; }
        public string PhoneNr { get; set; }
        public string DisplayName { get; internal set; }
        public string FriendStatus { get; set; }
    }
}