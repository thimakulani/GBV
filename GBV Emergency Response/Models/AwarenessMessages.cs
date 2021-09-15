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
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;

namespace GBV_Emergency_Response.Models
{
    public class AwarenessMessages
    {

        [Id]
        public string Id { get; set; }
        public string Message { get; set; }
        public string Uid { get; set; }
        public Timestamp Dates { get; set; }
        public string ImageUrl { get; set; }
    }
}