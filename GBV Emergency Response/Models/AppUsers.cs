using System;

using Android.Views;
using Android.Widget;
using Plugin.CloudFirestore.Attributes;

namespace GBV_Emergency_Response.Models
{
    public class AppUsers
    {
        [Id]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        
        
    }
}