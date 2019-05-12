using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Vision;
using Android.Gms.Vision.Texts;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace Pic2TextKeyboard.Droid
{
    [Activity(Label = "CameraActivity")]
    public class CameraActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.CameraLayout);

            if (IsThereAnAppToTakePictures())
                TakeAPicture(null, new System.EventArgs());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            string result = string.Empty;

            try
            {
                if (requestCode == 100 && resultCode == Result.Ok)
                {
                    if (data != null && data.Extras != null)
                    {
                        Bitmap imageBitmap = (Bitmap)data.Extras.Get("data");
                        TextRecognizer textRecognizer = new TextRecognizer.Builder(ApplicationContext).Build();
                        Frame frame = new Frame.Builder().SetBitmap(imageBitmap).Build();
                        SparseArray items = textRecognizer.Detect(frame);
                        StringBuilder builder = new StringBuilder();

                        for (int i = 0; i < items.Size(); i++)
                        {
                            TextBlock item = (TextBlock)items.ValueAt(i);
                            builder.Append(item.Value);
                            builder.Append(" ");
                        }

                        result = builder.ToString();
                    }
                }
            }
            catch (System.Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            }

            TextData.LastText = result;
            Finish();
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, System.EventArgs eventArgs)
        {
            try
            {
                StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
                StrictMode.SetVmPolicy(builder.Build());
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                if (intent.ResolveActivity(PackageManager) != null)
                {
                    StartActivityForResult(intent, 100);
                }
            }
            catch (System.Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            }
        }
    }
}