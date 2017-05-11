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
using InteractiveTimetable.BusinessLayer.Models;
using InteractiveTimetable.BusinessLayer.Managers;
using Timetable_Tape.Classes;
using Android.Graphics;

namespace Timetable_Tape.Fragments
{
    class Fragment_Children_Schedules : Fragment
    {
        #region variables
        View view;
        LinearLayout childrenSchedulesLinearLayout;

        ScheduleManager scheduleManager;
        InteractiveTimetable.BusinessLayer.Managers.UserManager userManager;

        #endregion
        #region constant variables
        const string imageButtonName = "editImageButton";
        #endregion

        #region Event Handlers
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.Children_Schedules_Layout, container, false);

            userManager = MainActivity.Current.userManager;
            scheduleManager = MainActivity.Current.scheduleManager;

            childrenSchedulesLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.Children_Schedules_Linear_Layout);
            TextView _date_Textview = view.FindViewById<TextView>(Resource.Id.Date_Textview);

            
            IEnumerable<User> users = userManager.GetUsers();
            if (users.ToList().Count == 0)
            {
                createTestData();
            }


            //filling date and title
            DateTime currentdate = DateTime.Now;
            _date_Textview.Text = currentdate.ToString("dddd, dd MMM yyyy");


            loadChildren();

            return view;
        }

        

        private void editSchedule(object sender, EventArgs e)
        {
            View imagebutton = sender as View;
            string imagebuttonId = imagebutton.GetTag(Resource.String.TagValue1).ToString();
            int userId = int.Parse(imagebuttonId.Substring(imageButtonName.Length, imagebuttonId.Length - imageButtonName.Length));

            //setting fragment
            Fragment fragment = new Fragment_Creating_Timetable_Tape();
            Bundle bundle = new Bundle();
            bundle.PutInt("userId", userId);
            fragment.Arguments = bundle;
            MainActivity.Current.changeFragment(MainActivity.Current, fragment, false);
        }

        private void createTestData()
        {
            userManager.SaveUser(new User() { FirstName = "Анжелика", LastName = "Анжелика", Id = 0, PhotoPath = getUriFromResourceId(Resource.Drawable.Child_1).ToString(), PatronymicName = "Sully", BirthDate = DateTime.Today });
            userManager.SaveUser(new User() { FirstName = "Витя", LastName = "Витя", Id = 0, PhotoPath = getUriFromResourceId(Resource.Drawable.Child_2).ToString(), PatronymicName = "Sully", BirthDate = DateTime.Today });
            userManager.SaveUser(new User() { FirstName = "Кристина", LastName = "Кристина", Id = 0, PhotoPath = getUriFromResourceId(Resource.Drawable.Child_3).ToString(), PatronymicName = "Sully", BirthDate = DateTime.Today });
            userManager.SaveUser(new User() { FirstName = "Влад", LastName = "Влад", Id = 0, PhotoPath = getUriFromResourceId(Resource.Drawable.Child_4).ToString(), PatronymicName = "Sully", BirthDate = DateTime.Today });
            userManager.SaveUser(new User() { FirstName = "Алиса", LastName = "Алиса", Id = 0, PhotoPath = getUriFromResourceId(Resource.Drawable.Child_5).ToString(), PatronymicName = "Sully", BirthDate = DateTime.Today });
            userManager.SaveUser(new User() { FirstName = "Юрий", LastName = "Юрий", Id = 0, PhotoPath = getUriFromResourceId(Resource.Drawable.Child_6).ToString(), PatronymicName = "Sully", BirthDate = DateTime.Today });
            userManager.SaveUser(new User() { FirstName = "Иван", LastName = "Иван", Id = 0, PhotoPath = getUriFromResourceId(Resource.Drawable.Child_7).ToString(), PatronymicName = "Sully", BirthDate = DateTime.Today });
        }

        #endregion

        private void loadChildren()
        {
            IEnumerable<User> users = userManager.GetUsers();
            foreach (User user in users)
            {
                View childScheduleView = Activity.LayoutInflater.Inflate(Resource.Layout.Child_Schedule, childrenSchedulesLinearLayout, false);

                //setting child's picture
                ImageView childImage = (ImageView)childScheduleView.FindViewById(Resource.Id.Child_Picture_ImageView);

                //giving picture round corners
                ImageHelper imagehelper = new ImageHelper();
                childImage.SetImageBitmap(imagehelper.getRoundedCornerBitmap(GetBitmapFromURI(Android.Net.Uri.Parse(user.PhotoPath)),20));


                //seting child's name
                TextView childNameTextview = (TextView)childScheduleView.FindViewById(Resource.Id.Child_Name_TextView);
                childNameTextview.Text = user.FirstName;

                ImageButton scheduleEditImageButton = (ImageButton)childScheduleView.FindViewById(Resource.Id.Edit_Schedule_Button);
                scheduleEditImageButton.SetTag(Resource.String.TagValue1, imageButtonName + user.Id.ToString());
                scheduleEditImageButton.Click += editSchedule;


                loadSchedule(user, childScheduleView);

                childrenSchedulesLinearLayout.AddView(childScheduleView);
            }
        }

        

        private void loadSchedule(User user, View view)
        {
            Schedule schedule = null;
            IEnumerable<Schedule> schedules = scheduleManager.GetSchedules(user.Id);

            foreach (Schedule SearchSchedule in schedules)
            {
                if (SearchSchedule.CreateTime.Date == DateTime.Today)
                {
                    schedule = SearchSchedule;
                    break;
                }
            }
            if (schedule != null)
            {
                foreach (ScheduleItem scheduleItem in schedule.ScheduleItems)
                {
                    Card card = scheduleManager.Cards.GetCard(scheduleItem.CardId);
                    if (card.CardTypeId != 2)
                    {
                        addScheduleItem(Resource.Layout.ScheduleItem_ImageView, card, view);
                    }
                    else
                    {
                        addScheduleItem(Resource.Layout.Motivationgoal_ImageView, card, view);
                    }
                }
            }
            else
            {
                View emptyScheduleView = Activity.LayoutInflater.Inflate(Resource.Layout.Child_Empty_Schedule, childrenSchedulesLinearLayout, false);

                Button createScheduleButton = emptyScheduleView.FindViewById<Button>(Resource.Id.NoScheduleCreateScheduleButton);
                createScheduleButton.SetTag(Resource.String.TagValue1, imageButtonName + user.Id.ToString());
                createScheduleButton.Click += editSchedule;

                view.FindViewById<LinearLayout>(Resource.Id.Child_Schedule_LinearLayout).AddView(emptyScheduleView);
            }
        }

        private void addScheduleItem(int layoutId, Card card, View view)
        {
            View scheduleItemView = Activity.LayoutInflater.Inflate(layoutId, childrenSchedulesLinearLayout, false);

            ImageView scheduleItem_ImageView = scheduleItemView.FindViewById<ImageView>(Resource.Id.imageView_ScheduleItem);
            scheduleItem_ImageView.SetImageURI(Android.Net.Uri.Parse(card.PhotoPath));

            view.FindViewById<LinearLayout>(Resource.Id.Child_Schedule_LinearLayout).AddView(scheduleItemView);
        }

        private Android.Net.Uri getUriFromResourceId(int resId)
        {
            return Android.Net.Uri.Parse(ContentResolver.SchemeAndroidResource + "://" + Resources.GetResourcePackageName(resId) + '/' + Resources.GetResourceTypeName(resId) + '/' + Resources.GetResourceEntryName(resId));
        }

        private Android.Graphics.Bitmap GetBitmapFromURI(Android.Net.Uri uriImage)
        {
            Android.Graphics.Bitmap mBitmap = null;
            mBitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(MainActivity.Current.ContentResolver, uriImage);
            return mBitmap;
        }
    }
}