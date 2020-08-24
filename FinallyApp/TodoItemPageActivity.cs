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
    [Activity(NoHistory = true, Label = "@string/single_todo")]
    public class TodoItemPageActivity : AppCompatActivity
    {

        EditText description, name;
        TextView creation_date, deadline, invalid_name, invalid_time, status;
        Toolbar toolbar;
        IMenuItem cancel_item, save_item, edit_item, delete_item;
        int id;
        Intent change_intent=null;
        long deadline_long;
        DateTime deadline_dateTime;
        Items.Todo todo;
        Android.Net.Uri uri;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.todo_item_page);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            toolbar.SetBackgroundResource(Resource.Color.TodoMainColor);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            name = FindViewById<EditText>(Resource.Id.todo_item_name);
            description = FindViewById<EditText>(Resource.Id.todo_item_description);
            creation_date = FindViewById<TextView>(Resource.Id.todo_item_creation_date);
            deadline = FindViewById<TextView>(Resource.Id.todo_item_deadline);
            status = FindViewById<TextView>(Resource.Id.status);
            invalid_name = FindViewById<TextView>(Resource.Id.todo_edit_name_invalid);
            invalid_time = FindViewById<TextView>(Resource.Id.todo_edit_deadline_invalid);
            name.Enabled = description.Enabled = deadline.Enabled = false;
            id = Intent.GetIntExtra(DataBaseProvider.InterfaceConsts.Id, -1);
            uri = Android.Net.Uri.WithAppendedPath(DataBaseProvider.TODO_URI, id.ToString());
            todo = (Items.Todo)Items.ListOfItems.GetItemFromId(Items.ListOfItems.Todos, id);
            creation_date.Text = Selection.ConvertToCurrDateFormat(todo.CreationDate);
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
            if (table_name == DataBaseProvider.InterfaceConsts.TodoTable && this.id == id)
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
            using IMenuItem sort_item = menu.FindItem(Resource.Id.sort_type);
            sort_item.SetVisible(false);
            save_item = menu.FindItem(Resource.Id.menu_save);
            save_item.SetVisible(false);
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
                    if (ValidCheck.ValidDeadline(deadline_long,invalid_time) && ValidCheck.ValidName(name.Text, invalid_name))
                    {
                        var contentV = new ContentValues();
                        Notifications.CancelNotification(todo.Name, id, DataBaseProvider.InterfaceConsts.TodoTable, this);
                        todo.Name = name.Text;
                        todo.Description = description.Text;
                        todo.DeadLine = deadline_long;
                        Notifications.ScheduleNotification(todo.Name, id, DataBaseProvider.InterfaceConsts.TodoTable, this, deadline_dateTime);
                        contentV.Put(DataBaseProvider.InterfaceConsts.Name, todo.Name);
                        contentV.Put(DataBaseProvider.InterfaceConsts.Description, todo.Description);
                        contentV.Put(DataBaseProvider.InterfaceConsts.Deadline, todo.DeadLine);
                        ContentResolver.Update(uri, contentV,null, null);
                        change_intent = new Intent();
                        change_intent.PutExtra(MainActivity.ACTION_TYPE, MainActivity.ACTION_CHANGE);
                        change_intent.PutExtra(Items.POSITION_KEY, Items.ListOfItems.Todos.IndexOf(todo));
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
            name.Text = todo.Name;
            description.Text = todo.Description;
            deadline.Text = Selection.ConvertToCurrDateFormat(todo.DeadLine);
            deadline_long = todo.DeadLine;
            deadline_dateTime = Selection.base_dateTime + new TimeSpan(deadline_long);
            switch (todo.Status)
            {
                case DataBaseProvider.InterfaceConsts.StatusActive:
                    status.SetText(Resource.String.status_active);
                    break;
                case DataBaseProvider.InterfaceConsts.StatusMissed:
                    status.SetText(Resource.String.status_missed);
                    break;
                case DataBaseProvider.InterfaceConsts.StatusDone:
                    status.SetText(Resource.String.status_done);
                    break;
            }
        }
        private void Edit_buttons_visible(bool v)
        {
            if (v)
            {
                name.Enabled = description.Enabled = deadline.Enabled = true;
                delete_item.SetVisible(false);
                edit_item.SetVisible(false);
                save_item.SetVisible(true);
                cancel_item.SetVisible(true);
                deadline.Click += DateSelect_OnClick;
            }
            else
            {
                name.Enabled = description.Enabled = deadline.Enabled = false;
                delete_item.SetVisible(true);
                edit_item.SetVisible(true);
                save_item.SetVisible(false);
                cancel_item.SetVisible(false);
                deadline.Click -= DateSelect_OnClick;
                invalid_name.Visibility = invalid_time.Visibility = ViewStates.Invisible;
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
                deadline_dateTime = time;
                TimeSelect_OnClick(sender, eventArgs);
            });
            frag.Show(SupportFragmentManager, DatePickerFragment.TAG);
        }
        void TimeSelect_OnClick(object sender, EventArgs eventArgs)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(
                delegate (DateTime time)
                {
                    deadline_dateTime = new DateTime(deadline_dateTime.Year, deadline_dateTime.Month, deadline_dateTime.Day, time.Hour, time.Minute, 0);
                    deadline_long = (deadline_dateTime - Selection.base_dateTime).Ticks;
                    deadline.Text = Selection.ConvertToCurrDateFormat(deadline_long);
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
                var position = Items.ListOfItems.Todos.IndexOf(todo);
                ContentResolver.Delete(uri, null, null);
                using ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
                if (pref.GetBoolean(SettingsActivity.NOTIFICATIONS_KEY, true)) Notifications.CancelNotification(name.Text, Items.ListOfItems.Todos[position].Id, DataBaseProvider.InterfaceConsts.TodoTable, this);
                Items.ListOfItems.Todos.RemoveAt(position);
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
 