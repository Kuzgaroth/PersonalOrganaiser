using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Android.Preferences;

namespace FinallyApp
{
    [Activity(NoHistory = true, Label = "@string/single_event")]
    public class EventItemPageActivity : AppCompatActivity
    {
        EditText description, name;
        TextView creation_date, invalid_name, event_start, event_end, invalid_start, invalid_end, status;
        Toolbar toolbar;
        Intent change_intent;
        IMenuItem cancel_item, save_item, edit_item, delete_item;
        int id;
        long start_long=0, end_long=0;
        Items.Event Event;
        DateTime start_dateTime, end_dateTime;
        Android.Net.Uri uri;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.event_item_page);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            toolbar.SetBackgroundResource(Resource.Color.EventMainColor);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            name = FindViewById<EditText>(Resource.Id.event_item_name);
            status = FindViewById<TextView>(Resource.Id.status);
            description = FindViewById<EditText>(Resource.Id.event_item_description);
            creation_date = FindViewById<TextView>(Resource.Id.event_item_creation_date);
            invalid_name = FindViewById<TextView>(Resource.Id.event_edit_name_invalid);
            id = Intent.GetIntExtra(DataBaseProvider.InterfaceConsts.Id, -1);
            Event = (Items.Event)Items.ListOfItems.GetItemFromId(Items.ListOfItems.Events, id);
            uri = Android.Net.Uri.WithAppendedPath(DataBaseProvider.EVENT_URI, id.ToString());
            creation_date.Text = Selection.ConvertToCurrDateFormat(Event.CreationDate);
            event_start = FindViewById<TextView>(Resource.Id.event_start_page);
            event_end = FindViewById<TextView>(Resource.Id.event_end_page);
            invalid_start = FindViewById<TextView>(Resource.Id.event_start_invalid_page);
            invalid_end = FindViewById<TextView>(Resource.Id.event_end_invalid_page);
            name.Enabled = description.Enabled = event_start.Enabled = event_end.Enabled = false;
        }
        protected override void OnStart()
        {
            base.OnStart();
            ActionsFromNotificationsReciever.NotifyAtChanges += ActionsFromNotif;
            NotificationPublisher.NotifyStatusChange += ActionsFromNotif;
            RefreshForms();
        }
        public void ActionsFromNotif(string table_name, int id, string action_type)
        {
            if (table_name == DataBaseProvider.InterfaceConsts.EventTable && this.id == id)
            {
                if (action_type == ActionsFromNotificationsReciever.ACTION_DELETE)
                {
                    SetResult(Result.Canceled);
                    Finish();
                }
                else RefreshForms();
            }
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            using IMenuItem create_item = menu.FindItem(Resource.Id.menu_create);
            create_item.SetVisible(false);
            using IMenuItem settings_item = menu.FindItem(Resource.Id.settings);
            settings_item.SetVisible(false);
            save_item = menu.FindItem(Resource.Id.menu_save);
            save_item.SetVisible(false);
            using IMenuItem sort_item = menu.FindItem(Resource.Id.sort_type);
            sort_item.SetVisible(false);
            cancel_item = menu.FindItem(Resource.Id.menu_cancel);
            cancel_item.SetVisible(false);
            delete_item = menu.FindItem(Resource.Id.menu_delete);
            edit_item = menu.FindItem(Resource.Id.menu_edit);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_delete:
                    ShowWarning();
                    return true;
                case Resource.Id.menu_edit:
                    Edit_buttons_visible(true);
                    return true;
                case Resource.Id.menu_cancel:
                    RefreshForms();
                    Edit_buttons_visible(false);
                    return true;
                case Resource.Id.menu_save:
                    if (ValidCheck.ValidDeadline(start_long, invalid_start) && ValidCheck.ValidName(name.Text, invalid_name) && ValidCheck.ValidEnd(ValidCheck.ValidDeadline(end_long, invalid_end), start_long, end_long, invalid_end))
                    {
                        var contentV = new ContentValues();
                        Notifications.CancelNotification(Event.Name, id, DataBaseProvider.InterfaceConsts.EventTable, this);
                        Notifications.CancelNotification(Event.Name, id, DataBaseProvider.InterfaceConsts.EventTable, this, true);
                        Event.Name = name.Text;
                        Event.Description = description.Text;
                        Event.StartTime = start_long;
                        Event.EndTime = end_long;
                        Notifications.ScheduleNotification(Event.Name, id, DataBaseProvider.InterfaceConsts.EventTable, this, start_dateTime);
                        Notifications.ScheduleNotification(Event.Name, id, DataBaseProvider.InterfaceConsts.EventTable, this, end_dateTime, true);
                        contentV.Put(DataBaseProvider.InterfaceConsts.Name, Event.Name);
                        contentV.Put(DataBaseProvider.InterfaceConsts.Description, Event.Description);
                        contentV.Put(DataBaseProvider.InterfaceConsts.StartTime, Event.StartTime);
                        contentV.Put(DataBaseProvider.InterfaceConsts.EndTime, Event.EndTime);
                        ContentResolver.Update(uri, contentV, null, null);
                        change_intent = new Intent();
                        change_intent.PutExtra(MainActivity.ACTION_TYPE, MainActivity.ACTION_CHANGE);
                        change_intent.PutExtra(Items.POSITION_KEY, Items.ListOfItems.Events.IndexOf(Event));
                            
                        SetResult(Result.Ok, change_intent);
                        Edit_buttons_visible(false);
                    }
                    return true;
                case Resource.Id.home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        void RefreshForms()
        {
            name.Text = Event.Name;
            description.Text = Event.Description;
            event_start.Text = Selection.ConvertToCurrDateFormat(Event.StartTime);
            event_end.Text = Selection.ConvertToCurrDateFormat(Event.EndTime);
            start_long = Event.StartTime;
            end_long = Event.EndTime;
            switch (Event.Status)
            {
                case DataBaseProvider.InterfaceConsts.StatusWaiting:
                    status.SetText(Resource.String.status_waiting);
                    break;
                case DataBaseProvider.InterfaceConsts.StatusStarted:
                    status.SetText(Resource.String.status_started);
                    break;
                case DataBaseProvider.InterfaceConsts.StatusEnded:
                    status.SetText(Resource.String.status_ended);
                    break;
            }
        }
        private void Edit_buttons_visible(bool v)
        {
            if (v)
            {
                name.Enabled = description.Enabled = event_start.Enabled = event_end.Enabled = true;
                delete_item.SetVisible(false);
                edit_item.SetVisible(false);
                save_item.SetVisible(true);
                cancel_item.SetVisible(true);
                event_start.Click += DateSelect_OnClick;
                event_end.Click += DateSelect_OnClick;
            }
            else
            {
                name.Enabled = description.Enabled = event_start.Enabled = event_end.Enabled = false;
                delete_item.SetVisible(true);
                edit_item.SetVisible(true);
                save_item.SetVisible(false);
                cancel_item.SetVisible(false);
                event_start.Click -= DateSelect_OnClick;
                event_end.Click -= DateSelect_OnClick;
                invalid_name.Visibility = invalid_start.Visibility= invalid_end.Visibility = ViewStates.Invisible;
            }
        }
        protected override void OnStop()
        {
            ActionsFromNotificationsReciever.NotifyAtChanges -= ActionsFromNotif;
            base.OnStop();
        }
        void DateSelect_OnClick(object sender, EventArgs eventArgs)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                if ((TextView)sender == event_start)
                {
                    start_dateTime = time;
                }
                else if ((TextView)sender == event_end)
                {
                    end_dateTime = time;
                }
                TimeSelectOnClick(sender, eventArgs);
            });
            frag.Show(SupportFragmentManager, DatePickerFragment.TAG);
        }
        void TimeSelectOnClick(object sender, EventArgs eventArgs)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(
                delegate (DateTime time)
                {
                    if ((TextView)sender == event_start)
                    {
                        start_dateTime = new DateTime(start_dateTime.Year, start_dateTime.Month, start_dateTime.Day, time.Hour, time.Minute, 0);
                        start_long = (start_dateTime - Selection.base_dateTime).Ticks;
                        event_start.Text = Selection.ConvertToCurrDateFormat(start_long);
                    }
                    else if ((TextView)sender == event_end)
                    {
                        end_dateTime = new DateTime(end_dateTime.Year, end_dateTime.Month, end_dateTime.Day, time.Hour, time.Minute, 0);
                        end_long = (end_dateTime - Selection.base_dateTime).Ticks;
                        event_end.Text = Selection.ConvertToCurrDateFormat(end_long);
                    } 
                });

            frag.Show(SupportFragmentManager, TimePickerFragment.TAG);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        void ShowWarning()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetPositiveButton(Resource.String.positive_button, (sender, args) =>
            {
                var position = Items.ListOfItems.Events.IndexOf(Event);
                ContentResolver.Delete(uri, null, null);
                using ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
                if (pref.GetBoolean(SettingsActivity.NOTIFICATIONS_KEY, true)) Notifications.CancelNotification(name.Text, Items.ListOfItems.Events[position].Id, DataBaseProvider.InterfaceConsts.EventTable, this);
                Items.ListOfItems.Events.RemoveAt(position);
                change_intent = new Intent();
                change_intent.PutExtra(MainActivity.ACTION_TYPE, MainActivity.ACTION_DELETE);
                change_intent.PutExtra(Items.POSITION_KEY, position);
                SetResult(Result.Ok, change_intent);
                Finish();
            });
            builder.SetNegativeButton(Resource.String.negative_button, (sender, args) =>
            {

            });
            builder.SetMessage(Resource.String.delete_confirm);
            AlertDialog dialog = builder.Create();
            dialog.Show();
        }
    }
}