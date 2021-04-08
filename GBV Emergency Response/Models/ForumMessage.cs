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

namespace GBV_Emergency_Response.Models
{
    public class ForumMessage
    {
        public string Msg { get; set; }
        public string KeyID { get; set; }
        public string SenderName { get; set; }
        public string UserId { get; set; }
        public DateTime Date_Time { get; set; }

    }
}