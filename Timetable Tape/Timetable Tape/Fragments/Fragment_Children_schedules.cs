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
        #region Event Handlers
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.Children_Schedule_Layout, container, false);

            userManager = MainActivity.Current.userManager;
            scheduleManager = MainActivity.Current.scheduleManager;

            childrenSchedulesLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.Children_Schedules_Linear_Layout);

            loadChildren();

            return view;
        }

        private void loadChildren()
        {
            IEnumerable<User> users = userManager.GetUsers();
            foreach (User user in users)
            {
                View childScheduleView = Activity.LayoutInflater.Inflate(Resource.Layout.Child_Schedule, childrenSchedulesLinearLayout, false);

                //setting child's picture
                ImageView childImage = (ImageView)childScheduleView.FindViewById(Resource.Id.Child_Picture_ImageView);
                childImage.SetImageURI(Android.Net.Uri.Parse(user.PhotoPath));

                //seting child's name
                TextView childNameTextview = (TextView)childScheduleView.FindViewById(Resource.Id.Child_Name_TextView);
                childNameTextview.Text = user.FirstName;

                loadSchedule(user, childScheduleView);

                childrenSchedulesLinearLayout.AddView(childScheduleView);
            }
        }

        private void loadSchedule(User user, View view)
        {
            Schedule schedule = null;
            List<Schedule> schedules = user.Schedules;
            foreach(Schedule SearchSchedule in schedules)
            {
                if(SearchSchedule.CreateTime.Date == DateTime.Now)
                {
                    schedule = SearchSchedule;
                    break;
                }
            }
            foreach(ScheduleItem scheduleItem in schedule.ScheduleItems)
            {
                Card card = scheduleManager.Cards.GetCard(scheduleItem.CardId);
                View scheduleItemView = Activity.LayoutInflater.Inflate(Resource.Layout.ScheduleItem_ImageView, childrenSchedulesLinearLayout, false);

                ImageView scheduleItem_ImageView = scheduleItemView.FindViewById<ImageView>(Resource.Id.imageView_ScheduleItem);
                scheduleItem_ImageView.SetImageURI(Android.Net.Uri.Parse(card.PhotoPath));
            }
        }

        #endregion
    }
}