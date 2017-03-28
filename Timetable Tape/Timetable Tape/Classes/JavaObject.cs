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

namespace Timetable_Tape.Classes
{
    public class JavaObject <T> : Java.Lang.Object
    {
        public T value { get; set; }

        public JavaObject(T obj)
        {
            this.value = obj;
        }
    }
}