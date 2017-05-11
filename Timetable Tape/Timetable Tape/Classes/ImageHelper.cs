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
using Android.Graphics;
using System.Drawing;
using static Android.Graphics.PorterDuff;
using static Android.Graphics.Bitmap;

namespace Timetable_Tape.Classes
{
    public class ImageHelper
    {
        public Bitmap getRoundedCornerBitmap(Bitmap bitmap, int pixels)
        {
            Bitmap output = Bitmap.CreateBitmap(bitmap.Width, bitmap
                    .Height,Config.Argb8888);
            Canvas canvas = new Canvas(output);

             
             Paint paint = new Paint();
             Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
             RectF rectF = new RectF(rect);
             float roundPx = pixels;

            paint.AntiAlias = true;
            canvas.DrawARGB(0, 0, 0, 0);
            paint.Color = Android.Graphics.Color.Black;
            canvas.DrawRoundRect(rectF, roundPx, roundPx, paint);

            paint.SetXfermode(new PorterDuffXfermode(Mode.SrcIn));
            canvas.DrawBitmap(bitmap, rect, rect, paint);

            return output;
        }
    }
}