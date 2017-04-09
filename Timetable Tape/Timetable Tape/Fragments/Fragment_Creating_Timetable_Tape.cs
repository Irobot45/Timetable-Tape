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
using Java.Net;
using Android.Provider;
using Java.IO;
using Android.Content.Res;
using Android.Graphics;

namespace Timetable_Tape
{
    public class Fragment_Creating_Timetable_Tape : Fragment
    {
        static int newScheduledItemId = 0;
        static int scheduledItemId { get { newScheduledItemId++; return newScheduledItemId; } }
        static int newActivityId = 0;
        static int activityId { get { newActivityId++; return newActivityId; } set { newActivityId = value; } }
        static int newMotivationGoalId = 0;
        static int motivationGoalId { get { newMotivationGoalId++; return newMotivationGoalId; } }

        private static int newCard_TypeId = 0;

        private static readonly int RequestCamera = 0;
        private static readonly int SelectFile = 1;



        TextView _date_Textview;
        ImageButton _addEmptyScheduleItemimageButton;
        GridLayout _schedule_GridLayout;
        GridLayout _activities_GridLayout;
        GridLayout _motivation_Goals_GridLayout;

        ScheduleManager scheduleManager;

        List<ScheduleItem> scheduledItems;
        List<CardType> cardTypes;


        private File _photo;
        private Bitmap _bitmap;
        private Android.Net.Uri _currentUri;


        private bool _fromGallery;


        View imageButtonView;
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

            scheduleManager.Cards.SaveCard(new Card() { CardTypeId = 0, PhotoPath = "test" });

            //Finding and setting views by finding id
            _addEmptyScheduleItemimageButton = view.FindViewById<ImageButton>(Resource.Id.Add_ScheduleItem_ImageButton);
            _schedule_GridLayout = view.FindViewById<GridLayout>(Resource.Id.Schedule_GridLayout);
            _activities_GridLayout = view.FindViewById<GridLayout>(Resource.Id.Activities_GridLayout);
            _motivation_Goals_GridLayout = view.FindViewById<GridLayout>(Resource.Id.Motivation_Goals_GridLayout);
            _date_Textview = view.FindViewById<TextView>(Resource.Id.Date_Textview);
            

            //setting tags
            _addEmptyScheduleItemimageButton.SetTag(Resource.String.TagValue2, new JavaObject<Card>(new Card()));


            //Setting events 
            _addEmptyScheduleItemimageButton.Click += AddScheduledItem;

            //setting values
            cardTypes = new List<CardType>(scheduleManager.Cards.CardTypes.GetCardTypes());
            _bitmap = null;

            //filling up the gridlayouts
            LoadActivities();
            LoadMotivationGoals();

            DateTime currentdate = DateTime.Now;
            _date_Textview.Text = string.Format("{0:00}/{1:00}/{2:0000}", currentdate.Day, currentdate.Month, currentdate.Year);

            return view;
        }

        

        public override void OnDestroy()
        {
            //destroying events
            _addEmptyScheduleItemimageButton.Click -= AddScheduledItem;

            GC.Collect();

            base.OnDestroy();
        }



        private void AddScheduledItem(object sender, EventArgs e)
        {

            //adding an empty card
            var button = sender as ImageButton;
            
            ScheduleItem scheduleitem = new ScheduleItem();

            //loading card from tag
            JavaObject<Card> card = (JavaObject<Card>)button.GetTag(Resource.String.TagValue2);
            
            scheduleitem.OrderNumber = scheduledItems.Count;

            scheduledItems.Add(scheduleitem);

            AddNewImageButtonToGridlayout("ScheduledItem_Imagebutton" + scheduledItemId, getUriFromResourceId(Resource.Drawable.emptyButton), _schedule_GridLayout);

            
        }

        private void LoadActivities()
        {
            //removing all cards
            _activities_GridLayout.RemoveAllViews();

            activityId = 0;

            IEnumerable<Card> cards = scheduleManager.Cards.GetCards();

            //getting all activity cards
            CardType cardtype = cardTypes[0];
            foreach(Card card in cardtype.Cards)
            {
                AddNewImageButtonToGridlayout("Activity_Imagebutton" + activityId, Android.Net.Uri.Parse(card.PhotoPath), _activities_GridLayout);
            }

            AddNewImageButtonToGridlayout("AddNewActivity", getUriFromResourceId(Resource.Drawable.plusSign), _activities_GridLayout, AddActivityOrMotivationGoalButtonClicked);
        }

        private void LoadMotivationGoals()
        {
            //removing all cards
            _motivation_Goals_GridLayout.RemoveAllViews();

            activityId = 0;

            IEnumerable<Card> cards = scheduleManager.Cards.GetCards();

            //getting all activity cards
            CardType cardtype = cardTypes[1];
            foreach (Card card in cardtype.Cards)
            {
                AddNewImageButtonToGridlayout("MotivationGoal_Imagebutton" + activityId, Android.Net.Uri.Parse(card.PhotoPath), _motivation_Goals_GridLayout);
            }

            AddNewImageButtonToGridlayout("AddNewMotivationGoal", getUriFromResourceId(Resource.Drawable.plusSign), _motivation_Goals_GridLayout, AddActivityOrMotivationGoalButtonClicked);
        }


        private void AddNewImageButtonToGridlayout(string id, Android.Net.Uri PhotoPath, GridLayout gridlayout, EventHandler onclickEvent = null)
        {
            imageButtonView = Activity.LayoutInflater.Inflate(Resource.Layout.ImageButton_Card, gridlayout, false);

            //making variable from imagebutton
            ImageButton imagebutton = (ImageButton)imageButtonView.FindViewById(Resource.Id.imageButton_Card);

            //TODO : Stop imagebutton from fucking up the freaking image, and be able to take pictures with camera
            //rescaling and setting image file
            //_bitmap = PhotoPath.ToString().LoadAndResizeBitmap(imagebutton.Width, imagebutton.Height);
            //if (_bitmap != null)
            //{
            //    imagebutton.SetImageBitmap(_bitmap);
            //    _bitmap = null;
            //}

            //setting id tag
            imagebutton.SetTag(Resource.String.TagValue1, id);

            //adding view to gridlayout
            gridlayout.AddView(imageButtonView);

            //adding event to imagebutton
            if(onclickEvent != null)
                imagebutton.Click += onclickEvent;

            GC.Collect();
        }

        private void AddActivityOrMotivationGoalButtonClicked(object sender, EventArgs eventArgs)
        {
            ImageButton imagebutton = sender as ImageButton;
            if(imagebutton.GetTag(Resource.String.TagValue1).ToString() == "AddNewActivity")
            {
                newCard_TypeId = scheduleManager.Cards.CardTypes.GetCardType(1).Id;
            }
            else
            {
                newCard_TypeId = scheduleManager.Cards.CardTypes.GetCardType(2).Id;
            }

            if (MainActivity.Current.HasCamera)
            {
                ChoosePhotoIfHasCamera();
            }
            else
            {
                ChoosePhotoIfNoCamera();
            }
        }

        private void ChoosePhotoIfHasCamera()
        {
            /* Preparing dialog items */
            string[] items =
            {
                GetString(Resource.String.take_a_photo),
                GetString(Resource.String.choose_from_gallery),
                GetString(Resource.String.cancel_button)
            };

            /* Constructing dialog */
            using (var dialogBuilder = new AlertDialog.Builder(Activity))
            {
                dialogBuilder.SetTitle(GetString(Resource.String.add_photo));

                dialogBuilder.SetItems(items, (d, args) => {

                    /* Taking a photo */
                    if (args.Which == 0)
                    {
                        var intent = new Intent(MediaStore.ActionImageCapture);

                        _photo = new File(MainActivity.Current.PhotoDirectory,
                                            $"user_{Guid.NewGuid()}.jpg");

                        intent.PutExtra(
                            MediaStore.ExtraOutput,
                            Android.Net.Uri.FromFile(_photo));

                        StartActivityForResult(intent, RequestCamera);
                    }
                    /* Choosing from gallery */
                    else if (args.Which == 1)
                    {
                        ChoosePhotoIfNoCamera();
                    }
                });

                dialogBuilder.Show();
            }
        }

        private void ChoosePhotoIfNoCamera()
        {
            var intent = new Intent(
                            Intent.ActionPick,
                            MediaStore.Images.Media.ExternalContentUri);

            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            StartActivityForResult(
                Intent.CreateChooser(
                    intent,
                    GetString(Resource.String.choose_photo)),
                SelectFile);
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            /* If user chose photo */
            if (resultCode == Result.Ok)
            {
                if (requestCode == RequestCamera)
                {
                    /* Making photo available in the gallery */
                    Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                    var contentUri = Android.Net.Uri.FromFile(_photo);
                    mediaScanIntent.SetData(contentUri);
                    Activity.SendBroadcast(mediaScanIntent);

                    _currentUri = Android.Net.Uri.FromFile(_photo);

                    /* Setting a flag to choose method to get image path */
                    _fromGallery = false;


                    /* Dispose of the Java side bitmap. */
                    GC.Collect();

                }
                else if (requestCode == SelectFile && data != null)
                {
                    var uri = data.Data;
                    _currentUri = uri;

                    /* Setting a flag to choose method to get image path */
                    _fromGallery = true;
                }

                /* Adding new card to database with image path */
                Card newCard = new Card() { IsDeleted = false, PhotoPath = _currentUri.ToString(), CardTypeId = newCard_TypeId };
                scheduleManager.Cards.SaveCard(newCard);

            }
            LoadActivities();
            LoadMotivationGoals();
        }

        private Android.Net.Uri getUriFromResourceId(int resId)
        {
            return Android.Net.Uri.Parse(ContentResolver.SchemeAndroidResource + "://" + Resources.GetResourcePackageName(resId) + '/' + Resources.GetResourceTypeName(resId) + '/' + Resources.GetResourceEntryName(resId));
        }


    }

}