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
    public class AwarenessMessages
    {

        public string AwarenessMsg { get; set; }
        public string Sender { get; set; }
        public string SenderId { get; set; }
        public string MsgId { get; set; }
        public string Dates { get; set; }
        public string ImgUrl { get; set; }
    }
}