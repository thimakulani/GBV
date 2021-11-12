using Android.Content;
using Android.Gms.Maps;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Fragment.App;


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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.map_fragment, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            var mapFragment = ChildFragmentManager.FindFragmentById(Resource.Id.fragMap).JavaCast<SupportMapFragment>();
            mapFragment.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.googleMap = googleMap;
            // Create an IEnumberable<IGeoCoordinate>
            // or IEnumerable<Tuple<double,double>>
            //var geoPoints = new List<IGeoCoordinate>
            //{
            //    //, 
            //    new GeoCoordinate(-23.869037400561844, 29.48357921705048),
            //    new GeoCoordinate(-23.909845674211308, 29.597562367273195),
            //};
            //var utility = new PolylineUtility();
            //// Encode points to string.
            //var polyLine = utility.Encode(geoPoints); // output: _p~iF~ps|U_ulLnnqC_mqNvxq`@
            //PolylineOptions options = new PolylineOptions()
            //   .AddAll((Java.Lang.IIterable)geoPoints)
            //   .InvokeWidth(10)
            //   .InvokeColor(Color.Teal)
            //   .InvokeStartCap(new SquareCap())
            //   .InvokeEndCap(new SquareCap())
            //   .InvokeJointType(JointType.Round)
            //   .Geodesic(true);

            //this.googleMap.AddPolyline(options);

            //// Decode string to points.
            //var decodedPoints = utility.Decode(polyLine);
            //googleMap.AddPolyline(new Android.Gms.Maps.Model.PolylineOptions())
        }
    }
}