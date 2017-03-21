using Android.App;
using Android.Widget;
using Android.OS;
using InteractiveTimetable.BusinessLayer.Models;
using InteractiveTimetable.BusinessLayer.Managers;
using SQLite;
using Android.Views;
using Android.Content;
using Android.Util;
using System.IO;

namespace Timetable_Tape
{
    [Activity(Label = "Timetable_Tape", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            //Loading create timetable fragment
            var trans = FragmentManager.BeginTransaction();
            trans.Add(Resource.Id.FragmentContainer, new Fragment_Creating_Timetable_Tape(), "Fragment_Creating_Timetable_Tape");
            trans.Commit();




            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
            Schedule schedule = new Schedule();

            
            var databaseFileName = "InteractiveTimetableDatabase.db3";
            var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), databaseFileName);

            SQLiteConnection connection = new SQLiteConnection(dbPath);
            ScheduleManager sm = new ScheduleManager(connection);

        }

        
    }
}

