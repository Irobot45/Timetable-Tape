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
using System;
using Android.Runtime;

namespace Timetable_Tape
{
    [Activity(Label = "Timetable_Tape", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static MainActivity Current { get; private set; }
        public ScheduleManager scheduleManager { get; set; }
        public string projectPath { get; set; }

        public MainActivity()
        {
            Current = this;
        }
        protected MainActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            Current = this;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);


            SetContentView(Resource.Layout.Main);

            //Loading create timetable fragment
            var trans = FragmentManager.BeginTransaction();
            trans.Add(Resource.Id.FragmentContainer, new Fragment_Creating_Timetable_Tape(), "Fragment_Creating_Timetable_Tape");
            trans.Commit();




            // Set our view from the "main" layout resource
            Schedule schedule = new Schedule();

            
            // Creating Database and connection
            var databaseFileName = "InteractiveTimetableDatabase.db3";
            projectPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = Path.Combine(projectPath, databaseFileName);
            SQLiteConnection connection = new SQLiteConnection(dbPath);

            // Creating scheduleManager
            scheduleManager = new ScheduleManager(connection);

        }

        
    }
}

