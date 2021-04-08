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
    public class AlertsMessages
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public DateTime TimeDate { get; set; }
        public string UserKey { get; set; }
        public string AlertId { get; set; }
        public bool Attended { get; set; }
    }
}