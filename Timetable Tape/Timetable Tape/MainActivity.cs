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
using Android.Provider;
using Android.Database;
using System.Collections.Generic;
using Android.Content.PM;
using Timetable_Tape.Fragments;

namespace Timetable_Tape
{
    [Activity(Label = "Timetable_Tape", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static MainActivity Current { get; private set; }
        public ScheduleManager scheduleManager { get; set; }
        public InteractiveTimetable.BusinessLayer.Managers.UserManager userManager { get; set; }
        public string projectPath { get; set; }
        public bool HasCamera { get; private set; }
        public SQLiteConnection connection { get; set; }

        public Java.IO.File PhotoDirectory;


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

            //var trans = FragmentManager.BeginTransaction();
            //trans.Add(Resource.Id.FragmentContainer, new Fragment_Children_Schedules(), "Fragment_Children_Schedules");
            //trans.Commit();


            // Set our view from the "main" layout resource
            Schedule schedule = new Schedule();

            

            // Creating Database and connection
            var databaseFileName = "InteractiveTimetableDatabase.db3";
            projectPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = Path.Combine(projectPath, databaseFileName);
            connection = new SQLiteConnection(dbPath);

            HasCamera = IsThereAnAppToTakePictures();

            // Creating scheduleManager
            scheduleManager = new ScheduleManager(connection);

            // Creating userManager
            userManager = new InteractiveTimetable.BusinessLayer.Managers.UserManager(connection);

            
        }

        public string GetPathToImage(Activity activity, Android.Net.Uri uri)
        {
            string path = null;
            string[] proj = { MediaStore.Video.Media.InterfaceConsts.Data };
            ICursor mycursor = activity.ContentResolver.Query(uri, proj, null, null, null);
            if (mycursor.MoveToFirst())
            {
                var columnIndex = mycursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.Data);
                path = mycursor.GetString(columnIndex);
            }
            mycursor.Close();
            return path;
        }


        private void CreateDirectoryForPictures()
        {
            PhotoDirectory
                = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures),
                    GetString(Resource.String.app_name));

            if (!PhotoDirectory.Exists())
            {
                PhotoDirectory.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }


    }
}

