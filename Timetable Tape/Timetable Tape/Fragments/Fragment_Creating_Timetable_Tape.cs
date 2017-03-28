using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using InteractiveTimetable.BusinessLayer.Managers;
using InteractiveTimetable.BusinessLayer.Models;
using Timetable_Tape.Classes;
using Android.Graphics.Drawables;

namespace Timetable_Tape
{
    public class Fragment_Creating_Timetable_Tape : Fragment
    {
        static int NewScheduledItemId;



        ImageButton _addEmptyScheduleItemimageButton;
        GridLayout _timetable_Tape_Scrollview_GridLayout;

        ScheduleManager scheduleManager;

        List<ScheduleItem> scheduledItems;


        

        View view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            scheduledItems = new List<ScheduleItem>();

            view = inflater.Inflate(Resource.Layout.Creating_Timetable_Tape_Layout, container, false);

            //setting the scheduleManager
            scheduleManager = MainActivity.Current.scheduleManager;

            //Finding and setting views by finding id
            _addEmptyScheduleItemimageButton = view.FindViewById<ImageButton>(Resource.Id.Add_TimetableTape_ImageButton);
            _timetable_Tape_Scrollview_GridLayout = view.FindViewById<GridLayout>(Resource.Id.Timetable_Tape_Scrollview_GridLayout);


            

            //setting tags
            _addEmptyScheduleItemimageButton.SetTag(Resource.String.TagValue1, new JavaObject<Card>(new Card()));
            _addEmptyScheduleItemimageButton.SetTag(Resource.String.TagValue2, Resources.GetDrawable(Resource.Drawable.emptyButton));


            //Setting events 
            _addEmptyScheduleItemimageButton.Click += AddScheduledItem;



            return view;
        }

        

        public override void OnDestroy()
        {
            //destroying events
            _addEmptyScheduleItemimageButton.Click -= AddScheduledItem;
            
            
            base.OnDestroy();
        }



        private void AddScheduledItem(object sender, EventArgs e)
        {
            //adding an empty card
            var button = sender as ImageButton;
            
            ScheduleItem scheduleitem = new ScheduleItem();

            //loading card from tag
            JavaObject<Card> card = (JavaObject<Card>)button.GetTag(Resource.String.TagValue1);

            scheduleitem.OrderNumber = scheduledItems.Count;
            
            
            
            
            
            scheduledItems.Add(scheduleitem);

            //making empty imagebutton for the empty card with costum theme
            ImageButton imagebutton = new ImageButton(new ContextThemeWrapper(this.View.Context, Resource.Style.Theme_Custom));


            //setting image file from tag
            imagebutton.SetImageDrawable((Drawable)button.GetTag(Resource.String.TagValue2));
            
            
            //adding image button
            _timetable_Tape_Scrollview_GridLayout.AddView(imagebutton);

            //setting same Layoutparameters as the add_button
            imagebutton.LayoutParameters.Height = _addEmptyScheduleItemimageButton.LayoutParameters.Height;
            imagebutton.LayoutParameters.Width = _addEmptyScheduleItemimageButton.LayoutParameters.Width;
            imagebutton.SetPadding(_addEmptyScheduleItemimageButton.PaddingLeft, _addEmptyScheduleItemimageButton.PaddingTop, _addEmptyScheduleItemimageButton.PaddingRight, _addEmptyScheduleItemimageButton.PaddingBottom);
            

            //adding click event to button
            //imagebutton.Click += AddEmptyActivity(imagebutton);
        }

        
    }

}