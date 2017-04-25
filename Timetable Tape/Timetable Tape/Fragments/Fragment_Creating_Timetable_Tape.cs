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
        #region variables
        static int newScheduledItemId = 0;
        static int scheduledItemId { get { newScheduledItemId++; return newScheduledItemId; } set { newScheduledItemId = value; } }
        static int newActivityId = 0;
        static int activityId { get { newActivityId++; return newActivityId; } set { newActivityId = value; } }
        static int newMotivationGoalId = 0;
        static int motivationGoalId { get { newMotivationGoalId++; return newMotivationGoalId; } }

        private static int newCard_TypeId = 0;

        private static readonly int RequestCamera = 0;
        private static readonly int SelectFile = 1;

        ImageButton focusedImageButton  = null;
        ImageButton scheduledMotivationGoalImageButton = null;



        TextView _date_Textview;
        ImageButton _addEmptyScheduleItemimageButton;
        GridLayout _schedule_GridLayout;
        GridLayout _activities_GridLayout;
        GridLayout _motivation_Goals_GridLayout;

        ScheduleManager scheduleManager;

        List<ScheduleItem> scheduledItems;
        List<CardType> cardTypes;

        //TODO: make constant strings from names, colors from buttons,...


        private File _photo = null;
        private Bitmap _bitmap;
        private Android.Net.Uri _currentUri;


        private bool _fromGallery;


        View imageButtonView;
        View view;

        #endregion
        
        #region Event Handlers
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
            _addEmptyScheduleItemimageButton = view.FindViewById<ImageButton>(Resource.Id.Add_ScheduleItem_ImageButton);
            _schedule_GridLayout = view.FindViewById<GridLayout>(Resource.Id.Schedule_GridLayout);
            _activities_GridLayout = view.FindViewById<GridLayout>(Resource.Id.Activities_GridLayout);
            _motivation_Goals_GridLayout = view.FindViewById<GridLayout>(Resource.Id.Motivation_Goals_GridLayout);
            _date_Textview = view.FindViewById<TextView>(Resource.Id.Date_Textview);
            

            //setting tags
            _addEmptyScheduleItemimageButton.SetTag(Resource.String.TagValue2, new JavaObject<Card>(new Card()));


            //Setting events 
            _addEmptyScheduleItemimageButton.Click += AddEmptyScheduledItem;

            //setting values
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
            _addEmptyScheduleItemimageButton.Click -= AddEmptyScheduledItem;

            GC.Collect();

            base.OnDestroy();
        }



        private void AddEmptyScheduledItem(object sender, EventArgs e)
        {

            //adding an empty card
            ScheduleItem scheduleitem = new ScheduleItem();
            
            scheduleitem.OrderNumber = scheduledItems.Count;

            scheduledItems.Add(scheduleitem);

            AddNewImageButtonToGridlayout("ScheduledItem_Imagebutton" + scheduledItemId, getUriFromResourceId(Resource.Drawable.emptyButton), _schedule_GridLayout, AnyCardClicked);
        }

        private void AddActivityOrMotivationGoalButtonClicked(object sender, EventArgs eventArgs)
        {
            ImageButton imagebutton = sender as ImageButton;
            if (imagebutton.GetTag(Resource.String.TagValue1).ToString() == "AddNewActivity")
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

        void HandleDrag(object sender, Android.Views.View.DragEventArgs e)
        {
            var evt = e.Event;
            switch (evt.Action)
            {
                case DragAction.Started:
                    /* To register your view as a potential drop zone for the current view being dragged
                     * you need to set the event as handled
                     */
                    e.Handled = true;

                    /* An important thing to know is that drop zones need to be visible (i.e. their Visibility)
                     * property set to something other than Gone or Invisible) in order to be considered. A nice workaround
                     * if you need them hidden initially is to have their layout_height set to 1.
                     */

                    break;
                case DragAction.Entered:
                case DragAction.Exited:
                    /* These two states allows you to know when the dragged view is contained atop your drop zone.
                     * Traditionally you will use that tip to display a focus ring or any other similar mechanism
                     * to advertise your view as a drop zone to the user.
                     */

                    break;
                case DragAction.Drop:
                    /* This state is used when the user drops the view on your drop zone. If you want to accept the drop,
                     *  set the Handled value to true like before.
                     */
                    e.Handled = true;
                    /* It's also probably time to get a bit of the data associated with the drag to know what
                     * you want to do with the information.
                     */
                    var data = e.Event.ClipData.GetItemAt(0).Text;

                    break;
                case DragAction.Ended:
                    /* This is the final state, where you still have possibility to cancel the drop happened.
                     * You will generally want to set Handled to true.
                     */
                    e.Handled = true;
                    break;
            }
        }

        private void AnyCardClicked(object sender, EventArgs e)
        {
            string FocusedImageButtonTag = null;
            ImageButton senderImageButton = (ImageButton)sender;
            string senderTag = senderImageButton.GetTag(Resource.String.TagValue1).ToString();

            if (focusedImageButton != null)
                FocusedImageButtonTag = focusedImageButton.GetTag(Resource.String.TagValue1).ToString();

            int compareId = CompareCardTypes_DragnDrop_Selection(senderTag, FocusedImageButtonTag);

            switch (compareId)
            {
                case 0:
                    RemoveFocus();
                    break;
                case 1:
                    ScheduleActivity(focusedImageButton, senderImageButton);
                    RemoveFocus();
                    break;
                case 2:
                    ChangeFocus(senderImageButton);
                    break;
                case 3:
                    ScheduleActivity(senderImageButton, focusedImageButton);
                    RemoveFocus();
                    break;
                case 4:
                    ScheduleMotivationGoal(senderImageButton);
                    break;
                case 5:
                    RemoveMotivationGoal();
                    break;
                case 6:
                    break;
            }

        }

        #endregion

        #region Other Methods

        private void LoadActivities()
        {
            //removing all cards
            _activities_GridLayout.RemoveAllViews();

            activityId = 0;

            IEnumerable<Card> cards = scheduleManager.Cards.GetCards();
            cardTypes = new List<CardType>(scheduleManager.Cards.CardTypes.GetCardTypes());

            //getting all activity cards
            CardType cardtype = cardTypes[0];
            foreach(Card card in cardtype.Cards)
            {
                AddNewImageButtonToGridlayout("Activity_Imagebutton" + activityId, Android.Net.Uri.Parse(card.PhotoPath), _activities_GridLayout, AnyCardClicked, card);
            }

            AddNewImageButtonToGridlayout("AddNewActivity", getUriFromResourceId(Resource.Drawable.plusSign), _activities_GridLayout, AddActivityOrMotivationGoalButtonClicked);
        }

        

        private void LoadMotivationGoals()
        {
            //removing all cards
            _motivation_Goals_GridLayout.RemoveAllViews();

            activityId = 0;

            cardTypes = new List<CardType>(scheduleManager.Cards.CardTypes.GetCardTypes());
            IEnumerable<Card> cards = scheduleManager.Cards.GetCards();

            //getting all motivation goal cards
            CardType cardtype = cardTypes[1];
            foreach (Card card in cardtype.Cards)
            {
                AddNewImageButtonToGridlayout("MotivationGoal_Imagebutton" + motivationGoalId, Android.Net.Uri.Parse(card.PhotoPath), _motivation_Goals_GridLayout, AnyCardClicked, card);
            }

            AddNewImageButtonToGridlayout("AddNewMotivationGoal", getUriFromResourceId(Resource.Drawable.plusSign), _motivation_Goals_GridLayout, AddActivityOrMotivationGoalButtonClicked);
        }


        private void AddNewImageButtonToGridlayout(string id, Android.Net.Uri PhotoPath, GridLayout gridlayout , EventHandler onclickEvent = null, Card card = null, Boolean isScheduledMotivationGoal = false)
        {
            ImageButton imagebutton = null;
            int cardTypeId = 0;

            if (gridlayout == _schedule_GridLayout)
            {
                imageButtonView = Activity.LayoutInflater.Inflate(Resource.Layout.ImageButton_Schedule_Item, gridlayout, false);
                cardTypeId = 0;
            }
            else if (gridlayout == _activities_GridLayout)
            {
                imageButtonView = Activity.LayoutInflater.Inflate(Resource.Layout.ImageButton_Activity, gridlayout, false);

                cardTypeId = 1;
            }
            else if (gridlayout == _motivation_Goals_GridLayout)
            {
                imageButtonView = Activity.LayoutInflater.Inflate(Resource.Layout.ImageButton_MotivationGoal, gridlayout, false);

                cardTypeId = 2;
            }

            //making variable from imagebutton
            imagebutton = (ImageButton)imageButtonView.FindViewById(Resource.Id.imageButton_Card);

            imagebutton.SetImageURI(PhotoPath);


            if (card == null)
            {
                card = new Card() { CardTypeId = cardTypeId, PhotoPath = PhotoPath.ToString(), IsDeleted = false };
            }

            JavaObject<Card> Javacard = new JavaObject<Card>(card);

            //setting id tag
            imagebutton.SetTag(Resource.String.TagValue1, id);
            imagebutton.SetTag(Resource.String.TagValue2, Javacard);



            if (isScheduledMotivationGoal)
            {
                imagebutton.SetBackgroundColor(Color.Red);
            }

            //adding view to gridlayout
            gridlayout.AddView(imageButtonView);

            //adding event to imagebutton
            if (onclickEvent != null)
            {
                imagebutton.Click += onclickEvent;
            }

            GC.Collect();
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
                                            $"Card_{Guid.NewGuid()}.jpg");

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



        

        private void ScheduleActivity(ImageButton activityImageButton, ImageButton scheduleItemImageButton)
        {
            string scheduleItemImageButtonId = scheduleItemImageButton.GetTag(Resource.String.TagValue1).ToString();
            //getting last number from idTag(which is the orderId)
            int orderId = int.Parse(scheduleItemImageButtonId.Substring(("ScheduledItem_Imagebutton").Length, scheduleItemImageButtonId.Length - ("ScheduledItem_Imagebutton").Length));

            //getting cardId from activityImagebutton
            JavaObject<Card> card = (JavaObject<Card>)activityImageButton.GetTag(Resource.String.TagValue2);

            scheduledItems[orderId - 1].CardId = card.value.Id;

            LoadScheduledItems();
        }

        private void ScheduleMotivationGoal(ImageButton motivationGoalImageButton)
        {
            RemoveMotivationGoal();

            //getting cardId from motivationGoalImagebutton
            scheduledMotivationGoalImageButton = motivationGoalImageButton;

            scheduledMotivationGoalImageButton.SetBackgroundColor(Color.Red);

            LoadScheduledItems();
        }

        private void LoadScheduledItems()
        {
            _schedule_GridLayout.RemoveAllViews();
            scheduledItemId = 0;

            foreach(ScheduleItem scheduledItem in scheduledItems)
            {
                Card card = new Card();
                if (scheduledItem.CardId != 0)
                {
                    card = scheduleManager.Cards.GetCard(scheduledItem.CardId);
                }
                else
                {
                    card.PhotoPath = getUriFromResourceId(Resource.Drawable.emptyButton).ToString();
                }
                AddNewImageButtonToGridlayout("ScheduledItem_Imagebutton" + scheduledItemId, Android.Net.Uri.Parse(card.PhotoPath), _schedule_GridLayout, AnyCardClicked,card);
            }
            if(scheduledMotivationGoalImageButton != null)
            {
                JavaObject<Card> card = (JavaObject<Card>)scheduledMotivationGoalImageButton.GetTag(Resource.String.TagValue2);
                AddNewImageButtonToGridlayout("Scheduled_MotvationGoal_Imagebutton", Android.Net.Uri.Parse(card.value.PhotoPath), _schedule_GridLayout,null,null,true);
            }
        }

        private void ChangeFocus(ImageButton newFocusedImageButton)
        {

            newFocusedImageButton.SetBackgroundColor(Color.Green);

             RemoveFocus();

            focusedImageButton = newFocusedImageButton;
        }

        private void RemoveFocus()
        {
            if (focusedImageButton != null)
                focusedImageButton.SetBackgroundColor(Color.ParseColor("#F3DBDBDB"));

            focusedImageButton = null;
        }

        

        private void RemoveMotivationGoal()
        {
            if (scheduledMotivationGoalImageButton != null)
            {
                scheduledMotivationGoalImageButton.SetBackgroundColor(Color.ParseColor("#F3DBDBDB"));
            }
            scheduledMotivationGoalImageButton = null;
        }

        private int CompareCardTypes_DragnDrop_Selection(string CardSenderTag, string currentCardTag)
        {
            if (CardSenderTag.Contains("MotivationGoal"))
            {
                if (scheduledMotivationGoalImageButton!= null && CardSenderTag == scheduledMotivationGoalImageButton.GetTag(Resource.String.TagValue1).ToString())
                {
                    return 5;
                }
                else
                {
                    return 4;
                }
            }

            if (focusedImageButton == null)
            {
                
                return 2;
            }

            else
            {
                string FocusedImageButtonTag = focusedImageButton.GetTag(Resource.String.TagValue1).ToString();
                if (CardSenderTag == currentCardTag)
                {
                    return 0;
                }
                else if (CardSenderTag.Contains("ScheduledItem") && FocusedImageButtonTag.Contains("Activity"))
                {
                    return 1;
                }
                else if (CardSenderTag.Contains("Activity") && FocusedImageButtonTag.Contains("Activity")
               || CardSenderTag.Contains("ScheduledItem") && FocusedImageButtonTag.Contains("ScheduledItem"))
                {
                    return 2;
                }
                else if (CardSenderTag.Contains("Activity") && FocusedImageButtonTag.Contains("ScheduledItem"))
                {
                    return 3;
                }
                return 6;
            }
        }
        #endregion



    }

}