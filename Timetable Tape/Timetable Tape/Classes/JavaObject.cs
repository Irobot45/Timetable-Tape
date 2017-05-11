namespace Timetable_Tape.Classes
{
    public class JavaObject <T> : Java.Lang.Object
    {
        public T value { get; set; }

        public JavaObject(T obj)
        {
            value = obj;
        }
    }
}