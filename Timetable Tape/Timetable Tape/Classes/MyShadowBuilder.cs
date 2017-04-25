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

namespace Timetable_Tape.Classes
{
    class MyShadowBuilder : View.DragShadowBuilder
    {
        const int centerOffset = 52;
        int width, height;

        public MyShadowBuilder(View baseView) : base(baseView)
        {
        }

        public override void OnProvideShadowMetrics(Point shadowSize, Point shadowTouchPoint)
        {
            width = View.Width;
            height = View.Height;

            // This is the overall dimension of your drag shadow
            shadowSize.Set(width * 2, height * 2);
            // This one tells the system how to translate your shadow on the screen so
            // that the user fingertip is situated on that point of your canvas.
            // In my case, the touch point is in the middle of the (height, width) top-right rect
            shadowTouchPoint.Set(width + width / 2 - centerOffset, height / 2 + centerOffset);
        }

        public override void OnDrawShadow(Canvas canvas)
        {
            const float sepAngle = (float)Math.PI / 16;
            const float circleRadius = 2f;

            // Draw the shadow circles in the top-right corner
            float centerX = width + width / 2 - centerOffset;
            float centerY = height / 2 + centerOffset;

            var baseColor = Color.Black;
            var paint = new Paint()
            {
                AntiAlias = true,
                Color = baseColor
            };

            // draw a dot where the center of the touch point (i.e. your fingertip) is
            canvas.DrawCircle(centerX, centerY, circleRadius + 1, paint);
            for (int radOffset = 70; centerX + radOffset < canvas.Width; radOffset += 20)
            {
                // Vary the alpha channel based on how far the dot is
                baseColor.A = (byte)(128 * (2f * (width / 2f - 1.3f * radOffset + 60) / width) + 100);
                paint.Color = baseColor;
                // Draw the dots along a circle of radius radOffset and centered on centerX,centerY
                for (float angle = 0; angle < Math.PI * 2; angle += sepAngle)
                {
                    var pointX = centerX + (float)Math.Cos(angle) * radOffset;
                    var pointY = centerY + (float)Math.Sin(angle) * radOffset;
                    canvas.DrawCircle(pointX, pointY, circleRadius, paint);
                }
            }

            // Draw the dragged view in the bottom-left corner
            canvas.DrawBitmap(View.DrawingCache, 0, height, null);
        }
    }
}