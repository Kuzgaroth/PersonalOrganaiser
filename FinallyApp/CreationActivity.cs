using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System;
using OrgDatabase1;
using Android.Preferences;
using Uri = Android.Net.Uri;

namespace FinallyApp
{
    [Activity(NoHistory =true)]
    public class CreationActivity : AppCompatActivity
    {
        string typeOfCreation;
        EditText header, description;
        Toolbar toolbar;
        TextView deadline, invalid_name,invalid_time, event_start, event_end, invalid_start, invalid_end;
        long deadline_long = 0, start_long=0, end_long=0;
        int id;
        DateTime deadline_dateTime, start_dateTime, end_dateTime;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            typeOfCreation = Intent.Extras.GetString(DataBaseProvider.InterfaceConsts.TableName);
            switch (typeOfCreation)
            {
                case DatabaseHelper.TODO_TABLE:
                    Todo_create();
                    break;
                case DatabaseHelper.NOTE_TABLE:
                    Note_create();
                    break;
                case DatabaseHelper.EVENT_TABLE:
                    Event_create();
                    break;
            }
        }
        protected override void OnStart()
        {
            base.OnStart();
        }
        protected override void OnStop()
        {
            base.OnStop();
        }
        private void Todo_create()
        {
            SetContentView(Resource.Layout.todo_creation_layout);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SetTitle(Resource.String.new_todo);
            toolbar.SetBackgroundResource(Resource.Color.TodoMainColor);
            header = FindViewById<EditText>(Resource.Id.todo_creation_name);
            description = FindViewById<EditText>(Resource.Id.todo_creation_text);
            deadline = FindViewById<TextView>(Resource.Id.todo_deadline);
            invalid_name = FindViewById<TextView>(Resource.Id.todo_name_invalid);
            invalid_time = FindViewById<TextView>(Resource.Id.todo_time_invalid);
            deadline.Click += DateSelect_OnClick;
        }
       
        private void Note_create()
        {

            SetContentView(Resource.Layout.note_creation_layout);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SetTitle(Resource.String.new_note);
            toolbar.SetBackgroundResource(Resource.Color.NoteMainColor);
            header = FindViewById<EditText>(Resource.Id.note_header_creation);
            description = FindViewById<EditText>(Resource.Id.note_description_creation);
            invalid_name = FindViewById<TextView>(Resource.Id.note_name_invalid);
        }
        private void Event_create()
        {
            SetContentView(Resource.Layout.event_creation_layout);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SetTitle(Resource.String.new_event);
            toolbar.SetBackgroundResource(Resource.Color.EventMainColor);
            header = FindViewById<EditText>(Resource.Id.event_creation_name);
            description = FindViewById<EditText>(Resource.Id.event_creation_text);
            event_start = FindViewById<TextView>(Resource.Id.event_start);
            event_end = FindViewById<TextView>(Resource.Id.event_end);
            event_start.Click += DateSelect_OnClick;
            event_end.Click += DateSelect_OnClick;
            invalid_name = FindViewById<TextView>(Resource.Id.event_name_invalid);
            invalid_start = FindViewById<TextView>(Resource.Id.event_start_invalid);
            invalid_end = FindViewById<TextView>(Resource.Id.event_end_invalid);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            using IMenuItem create_item = menu.FindItem(Resource.Id.menu_create);
            create_item.SetVisible(false);
            using IMenuItem edit_item = menu.FindItem(Resource.Id.menu_edit);
            edit_item.SetVisible(false);
            using IMenuItem delete_item = menu.FindItem(Resource.Id.menu_delete);
            delete_item.SetVisible(false);
            using IMenuItem sort_item = menu.FindItem(Resource.Id.sort_type);
            sort_item.SetVisible(false);
            using IMenuItem settings_item = menu.FindItem(Resource.Id.settings);
            settings_item.SetVisible(false);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_cancel:
                    Finish();
                    return true;
                case Resource.Id.menu_save:
                    ContentValues contentV = new ContentValues();
                    Uri uri;
                    long creationDate_long;
                    switch (typeOfCreation)
                    {
                        case DatabaseHelper.NOTE_TABLE:
                            if (ValidCheck.ValidName(header.Text,invalid_name))
                            {
                                contentV.Put(DataBaseProvider.InterfaceConsts.Name, header.Text);
                                contentV.Put(DataBaseProvider.InterfaceConsts.Description, description.Text);
                                creationDate_long = (DateTime.Now - Selection.base_dateTime).Ticks;
                                contentV.Put(DataBaseProvider.InterfaceConsts.CreationDate, creationDate_long);
                                uri = ContentResolver.Insert(DataBaseProvider.NOTE_URI, contentV);
                                id = Convert.ToInt32(uri.LastPathSegment);
                                Items.Note note = new Items.Note(header.Text, description.Text, creationDate_long, id);
                                Items.ListOfItems.Notes.Add(note);
                                SetResult(Result.Ok);
                                Finish();
                            }
                            break;
                        case DatabaseHelper.TODO_TABLE:
                            if (ValidCheck.ValidName(header.Text, invalid_name) && ValidCheck.ValidDeadline(deadline_long,invalid_time))
                            {
                                contentV.Put(DataBaseProvider.InterfaceConsts.Name, header.Text);
                                contentV.Put(DataBaseProvider.InterfaceConsts.Description, description.Text);
                                creationDate_long = (DateTime.Now - Selection.base_dateTime).Ticks;
                                contentV.Put(DataBaseProvider.InterfaceConsts.CreationDate, creationDate_long);
                                contentV.Put(DataBaseProvider.InterfaceConsts.Deadline, deadline_long);
                                contentV.Put(DataBaseProvider.InterfaceConsts.Status, DataBaseProvider.InterfaceConsts.StatusActive);
                                uri = ContentResolver.Insert(DataBaseProvider.TODO_URI, contentV);
                                id = Convert.ToInt32(uri.LastPathSegment);
                                Items.Todo todo = new Items.Todo(header.Text, description.Text, creationDate_long, deadline_long ,id);
                                Items.ListOfItems.Todos.Add(todo);
                                Notifications.ScheduleNotification(header.Text,id,DataBaseProvider.InterfaceConsts.TodoTable,this,deadline_dateTime);
                                SetResult(Result.Ok);
                                Finish();
                            }
                            break;
                        case DatabaseHelper.EVENT_TABLE:
                            if (ValidCheck.ValidName(header.Text, invalid_name) && ValidCheck.ValidDeadline(start_long, invalid_start) && ValidCheck.ValidEnd(ValidCheck.ValidDeadline(end_long, invalid_end), start_long, end_long,invalid_end))
                            {
                                contentV.Put(DataBaseProvider.InterfaceConsts.Name, header.Text);
                                contentV.Put(DataBaseProvider.InterfaceConsts.Description, description.Text);
                                creationDate_long = (DateTime.Now - Selection.base_dateTime).Ticks;
                                contentV.Put(DataBaseProvider.InterfaceConsts.CreationDate, creationDate_long);
                                contentV.Put(DataBaseProvider.InterfaceConsts.StartTime, start_long);
                                contentV.Put(DataBaseProvider.InterfaceConsts.EndTime, end_long);
                                contentV.Put(DataBaseProvider.InterfaceConsts.Status, DataBaseProvider.InterfaceConsts.StatusWaiting);
                                uri = ContentResolver.Insert(DataBaseProvider.EVENT_URI, contentV);
                                int id = Convert.ToInt32(uri.LastPathSegment);
                                Items.Event evenT = new Items.Event(header.Text, description.Text, creationDate_long, start_long, end_long, id);
                                Items.ListOfItems.Events.Add(evenT);
                                Notifications.ScheduleNotification(header.Text, id, DataBaseProvider.InterfaceConsts.EventTable, this, start_dateTime);
                                Notifications.ScheduleNotification(header.Text, id, DataBaseProvider.InterfaceConsts.EventTable, this, end_dateTime, true);
                                SetResult(Result.Ok);
                                Finish();
                            }
                            break;
                    }
                   
                    return true;
                case Resource.Id.home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
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
                else if ((TextView)sender == deadline)
                {
                    deadline_dateTime = time;
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
                        end_dateTime = new DateTime(end_dateTime.Year, end_dateTime.Month,end_dateTime.Day, time.Hour, time.Minute, 0);
                        end_long = (end_dateTime - Selection.base_dateTime).Ticks;
                        event_end.Text = Selection.ConvertToCurrDateFormat(end_long);
                    }
                    else if ((TextView)sender == deadline)
                    {
                        deadline_dateTime = new DateTime(deadline_dateTime.Year, deadline_dateTime.Month, deadline_dateTime.Day, time.Hour, time.Minute, 0);
                        deadline_long = (deadline_dateTime - Selection.base_dateTime).Ticks;
                        deadline.Text = Selection.ConvertToCurrDateFormat(deadline_long);
                    }
                });

            frag.Show(SupportFragmentManager, TimePickerFragment.TAG);
        }
    }   
}