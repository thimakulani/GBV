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
using Plugin.CloudFirestore.Attributes;

namespace GBV_Emergency_Response.Models
{
    public class ForumMessage
    {
        [MapTo("Message")]
        public string Msg { get; set; }
        [Id]
        public string KeyID { get; set; }
        [MapTo("Uid")]
        public string UserId { get; set; }
        [MapTo("DateTime")]
        public string Date_Time { get; set; }

    }
}