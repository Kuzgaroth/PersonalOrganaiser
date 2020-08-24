using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;

namespace FinallyApp
{
    [Activity(NoHistory = true, Theme ="@style/LoadingScreen", MainLauncher =true)]
    public class LoadingScreenActivity : AppCompatActivity
    {
        ICursor Ncursor, Tcursor, Ecursor;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Ncursor = ContentResolver.Query(DataBaseProvider.NOTE_URI, null, null, null, null);
            Tcursor = ContentResolver.Query(DataBaseProvider.TODO_URI, null, null, null, null);
            Ecursor = ContentResolver.Query(DataBaseProvider.EVENT_URI, null, null, null, null);
        }
        protected override void OnResume()
        {
            base.OnResume();
            Task loadingWork = new Task(() => { LoadFromDatabase(); });
            loadingWork.Start();
        }
        void LoadFromDatabase()
        {
           Items.ListOfItems.Notes = new List<Items.Note> { };
           for (int i=0;i<Ncursor.Count;i++)
           {
                Ncursor.MoveToPosition(i);
                Items.ListOfItems.Notes.Add(new Items.Note(Ncursor));
           }
            Items.ListOfItems.Todos = new List<Items.Todo> { };
            for (int i = 0; i < Tcursor.Count; i++)
            {
                Tcursor.MoveToPosition(i);
                Items.ListOfItems.Todos.Add(new Items.Todo(Tcursor));
            }
            Items.ListOfItems.Events = new List<Items.Event> { };
            for (int i = 0; i < Ecursor.Count; i++)
            {
                Ecursor.MoveToPosition(i);
                Items.ListOfItems.Events.Add(new Items.Event(Ecursor));
            }
            ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            Selection.note_selection = Selection.method_dict[pref.GetString(SettingsActivity.NOTE_CHOSEN_METHOD_KEY, SettingsActivity.BASE_NOTE_METHOD)];
            Selection.todo_selection = Selection.method_dict[pref.GetString(SettingsActivity.TODO_CHOSEN_METHOD_KEY, SettingsActivity.BASE_TODO_METHOD)];
            Selection.event_selection = Selection.method_dict[pref.GetString(SettingsActivity.EVENT_CHOSEN_METHOD_KEY, SettingsActivity.BASE_EVENT_METHOD)];
            Selection.curr_dateformat = Selection.date_format[pref.GetString(SettingsActivity.CHOSEN_DATE_FORMAT, SettingsActivity.BASE_DATE_FORMAT)];
            StartActivity(new Intent(this, typeof(MainActivity)));
        }
    }
}