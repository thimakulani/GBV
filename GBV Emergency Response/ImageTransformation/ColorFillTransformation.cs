using Android.Graphics;
using FFImageLoading;
using FFImageLoading.Transformations;

namespace GBV_Emergency_Response.ImageTransformations
{
    public class ColorFillTransformation : TransformationBase
    {
        public ColorFillTransformation() : this("#000000")
        {
        }

        public ColorFillTransformation(string hexColor)
        {
            HexColor = hexColor;
        }

        public string HexColor { get; set; }

        public override string Key => string.Format("ColorFillTransformation,hexColor={0}", HexColor);

        protected override Bitmap Transform(Bitmap sourceBitmap, string path, FFImageLoading.Work.ImageSource source, bool isPlaceholder, string key)
        {
            double sourceWidth = sourceBitmap.Width;
            double sourceHeight = sourceBitmap.Height;
            Bitmap bitmap = Bitmap.CreateBitmap((int)sourceWidth, (int)sourceHeight, Bitmap.Config.Argb8888);
            bitmap.HasAlpha = true;

            using (Canvas canvas = new Canvas(bitmap))
            using (Paint paint = new Paint())
            {
                paint.AntiAlias = true;
                canvas.DrawColor(HexColor.ToColor());
                canvas.DrawBitmap(sourceBitmap, 0, 0, paint);

                return bitmap;
            }
        }
    }
}
