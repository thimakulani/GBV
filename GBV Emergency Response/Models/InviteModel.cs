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
    public class InviteModel
    {
        [Id]
        public string Id { get; set; }
        public string Status { get; set; }
    }
}