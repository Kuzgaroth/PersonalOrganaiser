using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace FinallyApp
{
    [Activity(Label = "@string/settings_title", NoHistory =true)]
    public class SettingsActivity : AppCompatActivity, CompoundButton.IOnCheckedChangeListener
    {
        public const string NOTIFICATIONS_KEY = "notifications_key";
        public const string NOTE_CHOSEN_METHOD_KEY = "chosen_method_for_note";
        public const string TODO_CHOSEN_METHOD_KEY = "chosen_method_for_todo";
        public const string EVENT_CHOSEN_METHOD_KEY = "chosen_method_for_event";
        public const string CHOSEN_DATE_FORMAT = "chose_date_format";
        public const string BASE_NOTE_METHOD = "Последняя созданная заметка";
        public const string BASE_TODO_METHOD = "Горящая задача";
        public const string BASE_EVENT_METHOD = "Ближайшее событие";
        public const string BASE_DATE_FORMAT = "dd Month yyyy г. hh:mm";
        Spinner todo_spinner, note_spinner, event_spinner, date_spinner;
        Switch notification_switch;
        ISharedPreferences mSharedPref;
        ISharedPreferencesEditor mSharedEditor;
        Toolbar toolbar;
        ArrayAdapter note_adapter, todo_adapter, event_adapter, date_adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState); 
            SetContentView(Resource.Layout.preferences);
            mSharedPref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            mSharedEditor = mSharedPref.Edit();
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            toolbar.SetBackgroundResource(Resource.Color.SettingsColor);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            todo_spinner = FindViewById<Spinner>(Resource.Id.todo_spinner);
            note_spinner = FindViewById<Spinner>(Resource.Id.note_spinner);
            event_spinner = FindViewById<Spinner>(Resource.Id.event_spinner);
            notification_switch = FindViewById<Switch>(Resource.Id.notification_switch);
            date_spinner = FindViewById<Spinner>(Resource.Id.datetime_spinner);
            notification_switch.SetOnCheckedChangeListener(this);
            note_spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(NoteSpinner_Selected);
            note_adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.note_prompt, Android.Resource.Layout.SimpleSpinnerItem);
            note_adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            note_spinner.Adapter = note_adapter;

            todo_spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(TodoSpinner_Selected);
            todo_adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.todo_prompt, Android.Resource.Layout.SimpleSpinnerItem);
            todo_adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            todo_spinner.Adapter = todo_adapter;

            event_spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(EventSpinner_Selected);
            event_adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.event_prompt, Android.Resource.Layout.SimpleSpinnerItem);
            event_adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            event_spinner.Adapter = event_adapter;

            date_spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(DateSpinner_Selected);
            date_adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.datetime_prompt, Android.Resource.Layout.SimpleSpinnerItem);
            date_adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            date_spinner.Adapter = date_adapter;

            
        }
        protected override void OnStart()
        {
            base.OnStart();
            RefreshForms();
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.settings_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.reset_settings:
                    ResetSettings();
                    SetResult(Result.Ok);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        private void NoteSpinner_Selected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string name = spinner.GetItemAtPosition(e.Position).ToString();
            Selection.note_selection = Selection.method_dict[name];
            mSharedEditor.PutString(NOTE_CHOSEN_METHOD_KEY, name);
            mSharedEditor.Apply();
            SetResult(Result.Ok);
        }
        private void TodoSpinner_Selected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string name = spinner.GetItemAtPosition(e.Position).ToString();
            Selection.todo_selection = Selection.method_dict[name];
            mSharedEditor.PutString(TODO_CHOSEN_METHOD_KEY, name);
            mSharedEditor.Apply();
            SetResult(Result.Ok);
        }
        private void EventSpinner_Selected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string name = spinner.GetItemAtPosition(e.Position).ToString();
            Selection.event_selection = Selection.method_dict[name];
            mSharedEditor.PutString(EVENT_CHOSEN_METHOD_KEY, name);
            mSharedEditor.Apply();
            SetResult(Result.Ok);
        }
        private void DateSpinner_Selected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string name = spinner.GetItemAtPosition(e.Position).ToString();
            Selection.curr_dateformat = Selection.date_format[name];
            mSharedEditor.PutString(CHOSEN_DATE_FORMAT, name);
            mSharedEditor.Apply();
            SetResult(Result.Ok);
        }
        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            using Switch swt = (Switch)buttonView;
            mSharedEditor.PutBoolean(NOTIFICATIONS_KEY, swt.Checked);
            mSharedEditor.Apply();
        }
        protected override void OnDestroy()
        {
            mSharedPref.Dispose();
            mSharedEditor.Dispose();
            base.OnDestroy();
        }
        private void ResetSettings()
        {
            mSharedEditor.PutBoolean(NOTIFICATIONS_KEY, true);
            mSharedEditor.PutString(NOTE_CHOSEN_METHOD_KEY, BASE_NOTE_METHOD);
            Selection.note_selection = Selection.method_dict[BASE_NOTE_METHOD];
            mSharedEditor.PutString(TODO_CHOSEN_METHOD_KEY, BASE_TODO_METHOD);
            Selection.todo_selection = Selection.method_dict[BASE_TODO_METHOD];
            mSharedEditor.PutString(EVENT_CHOSEN_METHOD_KEY, BASE_EVENT_METHOD);
            Selection.event_selection = Selection.method_dict[BASE_EVENT_METHOD];
            mSharedEditor.PutString(CHOSEN_DATE_FORMAT, BASE_DATE_FORMAT);
            Selection.curr_dateformat = Selection.date_format[BASE_DATE_FORMAT];
            mSharedEditor.Apply();
            RefreshForms();
        }
        private void RefreshForms()
        {
            notification_switch.Checked = mSharedPref.GetBoolean(NOTIFICATIONS_KEY, true);
            note_spinner.SetSelection(note_adapter.GetPosition(mSharedPref.GetString(NOTE_CHOSEN_METHOD_KEY, BASE_NOTE_METHOD)));
            todo_spinner.SetSelection(todo_adapter.GetPosition(mSharedPref.GetString(TODO_CHOSEN_METHOD_KEY, BASE_TODO_METHOD)));
            event_spinner.SetSelection(event_adapter.GetPosition(mSharedPref.GetString(EVENT_CHOSEN_METHOD_KEY, BASE_EVENT_METHOD)));
            date_spinner.SetSelection(date_adapter.GetPosition(mSharedPref.GetString(CHOSEN_DATE_FORMAT, BASE_DATE_FORMAT)));
        }
    }
}