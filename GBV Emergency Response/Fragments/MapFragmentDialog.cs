using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.Snackbar;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GBV_Emergency_Response.Fragments
{
    public class MapFragmentDialog : Fragment, IOnMapReadyCallback
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        private Context context;
        private GoogleMap googleMap;
        private string userKey;

        public MapFragmentDialog(string userKey)
        {
            this.userKey = userKey;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.map_fragment, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }
        private double lat, lon;
        private void ConnectViews(View view)
        {
            var mapFragment = ChildFragmentManager.FindFragmentById(Resource.Id.fragMap).JavaCast<SupportMapFragment>();
            mapFragment.GetMapAsync(this);
            
           // Toast.MakeText(context,$"{lat} + {lon}", ToastLength.Long).Show();
        }
        //private async Task<string> GetAddress()
        //{
        //    //Android.Locations.Geocoder geocode;
        //    //var address = await geocode.GetFromLocationAsync(lat,lon, 1);
        //    //return address.ToString();
        //}
        public void OnMapReady(GoogleMap googleMap)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("LOCATION")
                .Document(userKey)
                .AddSnapshotListener((v, e) =>
                {
                    if (v.Exists)
                    {
                        UserLocation userLocation = v.ToObject<UserLocation>();
                        lat = double.Parse(userLocation.Latitude);
                        lon = double.Parse(userLocation.Longitude);
                        LatLng latlngall = new LatLng(lat, lon);
                        MarkerOptions options = new MarkerOptions().SetPosition(latlngall).SetTitle("User");
                        options.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
                        // marker.Tag = point.Id.ToString();


                        this.googleMap = googleMap;
                        this.googleMap.AddMarker(options);

                      

                        googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(latlngall, 17));

                    }
                });
            
            // googleMap.CameraPosition. 
            // Create an IEnumerable<IGeoCoordinate>
            // or IEnumerable<Tuple<double, double>>
            //var GeoPoint = new List<IGeoCoordinate>
            //{
            //     //, 
            //     new GeoCoordinate(-23.869037400561844, 29.48357921705048),
            //     new GeoCoordinate(-23.909845674211308, 29.597562367273195),
            //};
            // var utility = new PolylineUtility();
            // // Encode points to string.
            // var polyLine = utility.Encode(geoPoints); // output: _p~iF~ps|U_ulLnnqC_mqNvxq`@
            // PolylineOptions options = new PolylineOptions()
            //    .AddAll((Java.Lang.IIterable)geoPoints)
            //    .InvokeWidth(10)
            //    .InvokeColor(Color.Teal)
            //    .InvokeStartCap(new SquareCap())
            //    .InvokeEndCap(new SquareCap())
            //    .InvokeJointType(JointType.Round)
            //    .Geodesic(true);

            // this.googleMap.AddPolyline(options);

            // // Decode string to points.
            // var decodedPoints = utility.Decode(polyLine);
            // googleMap.AddPolyline(new Android.Gms.Maps.Model.PolylineOptions());
        }
    }
    public class UserLocation
    {
        [Id]
        public string Id { get; set; }
        public string Latitude { get; set; } 
        public string Longitude { get; set; }
    }
}