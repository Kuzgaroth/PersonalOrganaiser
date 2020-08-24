using Android.App;
using Android.Content;
using Android.Preferences;
using Uri = Android.Net.Uri;

namespace FinallyApp
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class NotificationPublisher : BroadcastReceiver
    {

        public const string NOTIFICATION_ID = "notification-id";
        public static string NOTIFICATION = "notification";
        public const string ITEM_TYPE = "typeOfItem";
        public const string STATUS_KEY = "status_key";
        public delegate void ChangeStatus(string table_name, int id, string action_type);
        static public event ChangeStatus NotifyStatusChange;
        public override void OnReceive(Context context, Intent intent)
        {
            ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            int id = intent.GetIntExtra(NOTIFICATION_ID, -1);
            string table_name = intent.GetStringExtra(ITEM_TYPE);
            if (pref.GetBoolean(SettingsActivity.NOTIFICATIONS_KEY, true))
            {
            Notification notification = (Notification)intent.GetParcelableExtra(NOTIFICATION);
            notificationManager.Notify(table_name, id, notification);
            }
            var contentv = new ContentValues();
                
            Uri uri;
            switch (table_name)
            {
                case DataBaseProvider.InterfaceConsts.TodoTable:
                    uri = Uri.WithAppendedPath(DataBaseProvider.TODO_URI, id.ToString());
                    contentv.Put(DataBaseProvider.InterfaceConsts.Status, DataBaseProvider.InterfaceConsts.StatusMissed);
                    context.ContentResolver.Update(uri, contentv, null, null);
                    NotifyStatusChange?.Invoke(table_name, id, ActionsFromNotificationsReciever.ACTION_MISSED);
                    break;
                case DataBaseProvider.InterfaceConsts.EventTable:
                    uri = Uri.WithAppendedPath(DataBaseProvider.EVENT_URI, id.ToString());
                    string status_key = intent.GetStringExtra(STATUS_KEY);
                    if (status_key==ActionsFromNotificationsReciever.ACTION_STARTED) contentv.Put(DataBaseProvider.InterfaceConsts.Status, DataBaseProvider.InterfaceConsts.StatusStarted);
                    else contentv.Put(DataBaseProvider.InterfaceConsts.Status, DataBaseProvider.InterfaceConsts.StatusEnded);
                    context.ContentResolver.Update(uri, contentv, null, null);
                    NotifyStatusChange?.Invoke(table_name, id, status_key);
                    break;
                default:
                    uri = DataBaseProvider.NOTE_URI;
                    break;
            };

            

            pref.Dispose();
        }
    }
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class ActionsFromNotificationsReciever : BroadcastReceiver
    {
        public const string ACTION_DELETE = "delete";
        public const string ACTION_HANDLE = "handle";
        public const string ACTION_TYPE = "action_type";
        public const string ACTION_MISSED = "missed";
        public const string ACTION_STARTED = "started";
        public const string ACTION_ENDED = "ended";
        public const string ACTION_WAITING = "waiting";
        public delegate void InMainActivity(string table_name, int id, string action_type);
        static public event InMainActivity NotifyAtChanges;
        public override void OnReceive(Context context, Intent intent)
        {
            int id = intent.GetIntExtra(DataBaseProvider.InterfaceConsts.Id, -1);
            string table_name = intent.GetStringExtra(DataBaseProvider.InterfaceConsts.TableName);
            Uri uri;
            string action_type = intent.GetStringExtra(ACTION_TYPE);
            switch (intent.GetStringExtra(DataBaseProvider.InterfaceConsts.TableName))
            {
                case DataBaseProvider.InterfaceConsts.TodoTable:
                    uri = Uri.WithAppendedPath(DataBaseProvider.TODO_URI, id.ToString());
                    break;
                case DataBaseProvider.InterfaceConsts.EventTable:
                    uri = Uri.WithAppendedPath(DataBaseProvider.TODO_URI, id.ToString());
                    break;
                default:
                    uri = DataBaseProvider.NOTE_URI;
                    break;
            };
            ContentValues contentV = new ContentValues();
            switch (action_type)
            {
                case ACTION_DELETE:
                    context.ContentResolver.Delete(uri, null, null);
                    break;
                case ACTION_HANDLE:
                    contentV.Put(DataBaseProvider.InterfaceConsts.Status, DataBaseProvider.InterfaceConsts.StatusDone);
                    context.ContentResolver.Update(uri, contentV, null, null);
                    break;
            }
            NotifyAtChanges?.Invoke(table_name, id, action_type);
            NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            notificationManager.Cancel(table_name, id);
            contentV.Dispose();
        }
    }
}