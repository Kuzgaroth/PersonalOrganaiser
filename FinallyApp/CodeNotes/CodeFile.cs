using Android.App;
using Android.Content;
using Android.Database;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FinallyApp
{
    public static class ValidCheck
    {
        public static bool ValidName(string name, TextView alert)
        {
          
           
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                alert.Visibility = ViewStates.Visible;
                return false;
            }
            else
            {
                alert.Visibility = ViewStates.Invisible;
                return true;
            }
        }
        public static bool ValidDeadline(long interval, TextView alert)
        {
            if (interval==0)
            {
                alert.Text = "Не выбрано время";
                alert.Visibility = ViewStates.Visible;
                return false;
            }
            DateTime dt = DateTime.Now;
            if ((dt-Selection.base_dateTime).Ticks>=interval)
            {
                alert.Text = "Некорректное время";
                alert.Visibility = ViewStates.Visible;
                return false;
            }
            else 
            {
                alert.Visibility = ViewStates.Invisible;
                return true;
            }
        }
        public static bool ValidEnd(bool previous_check, long start, long end, TextView alert)
        {
            if (previous_check)
            {
                if (end - start < new TimeSpan(0, 5, 0).Ticks)
                {
                    alert.Text = "Некорректное время";
                    alert.Visibility = ViewStates.Visible;
                    return false;
                }
                else
                {
                    alert.Visibility = ViewStates.Invisible;
                    return true;
                }
            }
            else return false;
        }
    }
    public static class Notifications
    {
        const string TODO_MISSED_MESSAGE = "Задание просрочено";
        const string EVENT_STARTED_MESSAGE = "Событие началось";
        const string EVENT_ENDED_MESSAGE = "Событие закончилось";
        private const int DELETE_ACTION_ID = 0;
        private const int HANDLE_ACTION_ID = 1000;
        private const int TODO_ID_CONST = 1;
        private const int EVENT_ID_CONST = -1;
        public static void ScheduleNotification(string name, int id, string table_name, Context context, DateTime set_date, bool started = false)
        {
             
            PendingIntent pendingIntent = GetPendingIntent(name, id, table_name, context, started);
            TimeSpan interval = set_date - DateTime.Now;
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Set(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime()+(long)interval.TotalMilliseconds, pendingIntent);
        }
        public static void CancelNotification(string name, int id, string table_name, Context context, bool started=false)
        {
            PendingIntent pendingIntent = GetPendingIntent(name, id, table_name, context, started);
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(pendingIntent);
        }
        private static PendingIntent GetPendingIntent(string name, int id, string table_name, Context context, bool started)
        {
            int code;
            Intent notificationIntent = new Intent(context, typeof(NotificationPublisher));
            notificationIntent.PutExtra(NotificationPublisher.NOTIFICATION_ID, id);
            Notification notification = GetNotification(table_name, name, context, id, started);
            notificationIntent.PutExtra(NotificationPublisher.NOTIFICATION, notification);
            notificationIntent.PutExtra(NotificationPublisher.ITEM_TYPE, table_name);
            if (!started)
            {
                notificationIntent.PutExtra(NotificationPublisher.STATUS_KEY, ActionsFromNotificationsReciever.ACTION_STARTED);
                code = GetRequestCode(table_name, id) * 1000;
            }
            else
            {
                notificationIntent.PutExtra(NotificationPublisher.STATUS_KEY, ActionsFromNotificationsReciever.ACTION_ENDED);
                code = GetRequestCode(table_name, id) * 10000;
            }
            if (started) notificationIntent.PutExtra(NotificationPublisher.STATUS_KEY, ActionsFromNotificationsReciever.ACTION_ENDED);
            else notificationIntent.PutExtra(NotificationPublisher.STATUS_KEY, ActionsFromNotificationsReciever.ACTION_STARTED);
            
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, code, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }
        public static int GetRequestCode(string table_name,int id)
        {
            return table_name switch
            {
                DataBaseProvider.InterfaceConsts.TodoTable => (id + 1) * TODO_ID_CONST,
                DataBaseProvider.InterfaceConsts.EventTable => (id + 1) * EVENT_ID_CONST,
                _ => throw new Exception(),
            };
        }
        public static Notification GetNotification(string table_name, string name, Context context, int id, bool started)
        {
            Notification.Builder builder = new Notification.Builder(context)
            .SetContentTitle(name)
            .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
            .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
            .SetAutoCancel(true);
            PendingIntent activityIntent;
            Intent actIntent;
            Intent intent = new Intent(context, typeof(ActionsFromNotificationsReciever));
            intent.PutExtra(DataBaseProvider.InterfaceConsts.TableName, table_name);
            intent.PutExtra(DataBaseProvider.InterfaceConsts.Id, id);
            intent.PutExtra(ActionsFromNotificationsReciever.ACTION_TYPE, ActionsFromNotificationsReciever.ACTION_DELETE);
            PendingIntent deleteIntent = PendingIntent.GetBroadcast(context, DELETE_ACTION_ID, intent, PendingIntentFlags.OneShot);
            builder.AddAction(Resource.Drawable.baseline_delete_sweep_24, "Удалить", deleteIntent);
            switch (table_name)
                {
                    case DataBaseProvider.InterfaceConsts.TodoTable:
                        builder.SetContentText(TODO_MISSED_MESSAGE);
                        builder.SetSmallIcon(Resource.Drawable.todo_icon_24);
                        actIntent = new Intent(context, typeof(TodoItemPageActivity));
                        actIntent.PutExtra(DataBaseProvider.InterfaceConsts.Id, id);
                        activityIntent = PendingIntent.GetActivity(context, id ,actIntent, PendingIntentFlags.OneShot);
                        builder.SetContentIntent(activityIntent);
                        Intent intent2 = new Intent(context, typeof(ActionsFromNotificationsReciever));
                        intent2.PutExtra(DataBaseProvider.InterfaceConsts.TableName, table_name);
                        intent2.PutExtra(DataBaseProvider.InterfaceConsts.Id, id);
                        intent2.PutExtra(ActionsFromNotificationsReciever.ACTION_TYPE, ActionsFromNotificationsReciever.ACTION_HANDLE);
                        PendingIntent handleIntent = PendingIntent.GetBroadcast(context, HANDLE_ACTION_ID, intent2, PendingIntentFlags.OneShot);
                        builder.AddAction(Resource.Drawable.baseline_check_24, "Сделано", handleIntent);
                    break;
                    case DataBaseProvider.InterfaceConsts.EventTable:
                        if (!started) builder.SetContentText(EVENT_STARTED_MESSAGE);
                        else builder.SetContentText(EVENT_ENDED_MESSAGE);
                        builder.SetSmallIcon(Resource.Drawable.event_icon_24);
                        actIntent = new Intent(context, typeof(EventItemPageActivity));
                        actIntent.PutExtra(DataBaseProvider.InterfaceConsts.Id, id);
                        activityIntent = PendingIntent.GetActivity(context,id , actIntent, PendingIntentFlags.OneShot);
                        builder.SetContentIntent(activityIntent);
                    break;
                }  
            return builder.Build();
        }
    }
    public static class Selection 
    {
        public const string CURRENT_NOTE_SEL = "curr_note_sel";
        public const string CURRENT_TODO_SEL = "curr_todo_sel";
        public const string CURRENT_EVENT_SEL = "curr_event_sel";
        public delegate List<Items.BaseItem> Chosen_Selection(string table_name);
        public static readonly Dictionary<string, Chosen_Selection> method_dict = new Dictionary<string, Chosen_Selection>
        {
            {"Последняя созданная заметка", GetLastCreated },
            {"Последняя созданная задача", GetLastCreated },
            {"Последнее созданное событие", GetLastCreated },
            {"Горящая задача", GetNearest },
            {"Ближайшее событие", GetNearest},
            {"Самая большая заметка",  GetBiggest},
            {"Самое большое событие", GetBiggest},
            {"Самая большая задача", GetBiggest}
        };
        public static Chosen_Selection note_selection;
        public static Chosen_Selection todo_selection;
        public static Chosen_Selection event_selection;
        public static readonly Dictionary<string, string> date_format = new Dictionary<string, string>
        {
            {"dd Month yyyy г. hh:mm","f"},
            {"dd.MM.yyyy hh:mm","g"}
        };
        public static string curr_dateformat;
        public readonly static DateTime base_dateTime = new DateTime(2020, 1, 1, 0, 0, 0);
        public static List<Items.BaseItem> GetLastCreated(string table_name)
        {
            List<Items.BaseItem> list = new List<Items.BaseItem> { };
            int max = -1;
            switch (table_name)
            {
                case DataBaseProvider.InterfaceConsts.NoteTable:
                    Items.Note item=null;
                    foreach (Items.Note note in Items.ListOfItems.Notes)
                    {
                        if (note.Id > max)
                        {
                            max = note.Id;
                            item = note;
                        }
                    }
                    list.Add(item);
                    return list;
                case DataBaseProvider.InterfaceConsts.TodoTable:
                    Items.Todo item1 = null;
                    foreach (Items.Todo todo in Items.ListOfItems.Todos)
                    {
                        if (todo.Id > max)
                        {
                            max = todo.Id;
                            item1 = todo;
                        }
                    }
                    list.Add(item1);
                    return list;
                case DataBaseProvider.InterfaceConsts.EventTable:
                    Items.Event item2 = null;
                    foreach (Items.Event evenT in Items.ListOfItems.Events)
                    {
                        if (evenT.Id > max)
                        {
                            max = evenT.Id;
                            item2 = evenT;
                        }
                    }
                    list.Add(item2);
                    return list;
                default:
                    throw new ArgumentException();
            }
        }
        public static List<Items.BaseItem> GetNearest(string table_name)
        {
            List<Items.BaseItem> list = new List<Items.BaseItem> { };
            long interval = long.MaxValue;
            switch (table_name)
            {
                case DataBaseProvider.InterfaceConsts.TodoTable:
                    Items.Todo item1 = null;
                    foreach (Items.Todo todo in Items.ListOfItems.Todos)
                    {
                        if (todo.DeadLine < interval)
                        {
                            interval = todo.DeadLine;
                            item1 = todo;
                        }
                    }
                    list.Add(item1);
                    return list;
                case DataBaseProvider.InterfaceConsts.EventTable:
                    Items.Event item2 = null;
                    foreach (Items.Event evenT in Items.ListOfItems.Events)
                    {
                        if (evenT.StartTime < interval)
                        {
                            interval = evenT.StartTime;
                            item2 = evenT;
                        }
                    }
                    list.Add(item2);
                    return list;
                default:
                    throw new ArgumentException();
            } 
        }
        public static List<Items.BaseItem> GetBiggest(string table_name)
        {
            List<Items.BaseItem> list = new List<Items.BaseItem> { };
            int max = -1, size;
            switch (table_name)
            {
                case DataBaseProvider.InterfaceConsts.NoteTable:
                    Items.Note item = null;
                    foreach (Items.Note note in Items.ListOfItems.Notes)
                    {
                        size = note.Name.Length + note.Description.Length;
                        if (size > max)
                        {
                            max = size;
                            item = note;
                        }
                    }
                    list.Add(item);
                    return list;
                case DataBaseProvider.InterfaceConsts.TodoTable:
                    Items.Todo item1 = null;
                    foreach (Items.Todo todo in Items.ListOfItems.Todos)
                    {
                        size = todo.Name.Length + todo.Description.Length;
                        if (size > max)
                        {
                            max = size;
                            item1 = todo;
                        }
                    }
                    list.Add(item1);
                    return list;
                case DataBaseProvider.InterfaceConsts.EventTable:
                    Items.Event item2 = null;
                    foreach (Items.Event evenT in Items.ListOfItems.Events)
                    {
                        size = evenT.Name.Length + evenT.Description.Length;
                        if (size > max)
                        {
                            max = size;
                            item2 = evenT;
                        }
                    }
                    list.Add(item2);
                    return list;
                default:
                    throw new ArgumentException();
            }
        }
        
    
        public static string ConvertToCurrDateFormat(long interval)
        {
            return (base_dateTime + (new TimeSpan(interval))).ToString(curr_dateformat);
        }
        public static List<Items.BaseItem> FormSummary()
        {
            List<Items.BaseItem> Summary = new List<Items.BaseItem> { };
            Summary.Add(null);
            if (Items.ListOfItems.Notes.Count!=0) Summary.AddRange(note_selection?.Invoke(DataBaseProvider.InterfaceConsts.NoteTable));
            Summary.Add(null);
            if (Items.ListOfItems.Todos.Count != 0)  Summary.AddRange(todo_selection?.Invoke(DataBaseProvider.InterfaceConsts.TodoTable));
            Summary.Add(null);
            if (Items.ListOfItems.Events.Count != 0) Summary.AddRange(event_selection?.Invoke(DataBaseProvider.InterfaceConsts.EventTable));
            return Summary;
        }
    }

    public class Items
    {
        public const string POSITION_KEY = "position_key";
        public static class ListOfItems
        {
            public static List<Note> Notes;
            public static List<Todo> Todos;
            public static List<Event> Events;
            public static BaseItem GetItemFromId(List<Note> list, int id)
            {
                foreach (var item in list)
                {
                    if (item.Id == id) return item;
                }
                return null;
            }
            public static int GetPositionFromId(List<Note> list, int id)
            {
                return list.IndexOf((Note)GetItemFromId(list, id));
            }
            public static BaseItem GetItemFromId(List<Todo> list, int id)
            {
                foreach (var item in list)
                {
                    if (item.Id == id) return item;
                }
                return null;
            }
            public static int GetPositionFromId(List<Todo> list, int id)
            {
                return list.IndexOf((Todo)GetItemFromId(list, id));
            }
            public static BaseItem GetItemFromId(List<Event> list, int id)
            {
                foreach (var item in list)
                {
                    if (item.Id == id) return item;
                }
                return null;
            }
            public static int GetPositionFromId(List<Event> list, int id)
            {
                return list.IndexOf((Event)GetItemFromId(list, id));
            }
        }
        public abstract class BaseItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public long CreationDate { get; set; }
            public int Id { get; set; }
            public BaseItem(ICursor cursor)
            {
                Name = cursor.GetString(cursor.GetColumnIndex(DataBaseProvider.InterfaceConsts.Name));
                Description = cursor.GetString(cursor.GetColumnIndex(DataBaseProvider.InterfaceConsts.Description));
                CreationDate = cursor.GetLong(cursor.GetColumnIndex(DataBaseProvider.InterfaceConsts.CreationDate));
                Id = cursor.GetInt(cursor.GetColumnIndex(DataBaseProvider.InterfaceConsts.Id));
            }
            public BaseItem(string name, string description, long creation_date, int id)
            {
                Name = name;
                Description = description;
                CreationDate = creation_date;
                Id = id;
            }
            
        }
        public class Note : BaseItem
        {
            public Note(ICursor cursor) : base(cursor)
            { }
            public Note(string name, string description, long creation_date, int id) : base(name,description,creation_date, id)
            { }
        }
        public class Todo : BaseItem
        {
            public long DeadLine { get; set; }
            public byte Status { get; set; }
            public Todo(ICursor cursor) : base(cursor)
            {
                DeadLine = cursor.GetLong(cursor.GetColumnIndex(DataBaseProvider.InterfaceConsts.Deadline));
                Status = (byte)cursor.GetShort(cursor.GetColumnIndex(DataBaseProvider.InterfaceConsts.Status));
            }
            public Todo(string name, string description, long creation_date, long deadline, int id) : base(name,description,creation_date, id)
            {
                DeadLine = deadline;
                Status = DataBaseProvider.InterfaceConsts.StatusActive;
            }
        }
        public class Event : BaseItem
        {
            public long StartTime { get; set; }
            public long EndTime { get; set; }
            public byte Status { get; set; }
            public Event(ICursor cursor) : base(cursor)
            {
                StartTime = cursor.GetLong(cursor.GetColumnIndex(DataBaseProvider.InterfaceConsts.StartTime));
                EndTime = cursor.GetLong(cursor.GetColumnIndex(DataBaseProvider.InterfaceConsts.EndTime));
                Status = (byte)cursor.GetShort(cursor.GetColumnIndex(DataBaseProvider.InterfaceConsts.Status));
            }
            public Event(string name, string description, long creation_date, long start_time,long end_time, int id) : base(name, description, creation_date, id)
            {
                StartTime = start_time;
                EndTime = end_time;
                Status = DataBaseProvider.InterfaceConsts.StatusWaiting;
            }
        }
        public class Sorts<T> where T: BaseItem
        {
            public class OldToNew : IComparer<T>
            {
                public int Compare(T x, T y)
                {
                    if (x.CreationDate > y.CreationDate) return 1;
                    else if (x.CreationDate < y.CreationDate) return -1;
                    return 0;
                }
            }
            public class  NewToOld : IComparer<T>
            {
                public int Compare(T x, T y)
                {
                    if (x.CreationDate > y.CreationDate) return -1;
                    else if (x.CreationDate < y.CreationDate) return 1;
                    return 0;
                }
            }
            public class AtoZ : IComparer<T>
            {
                public int Compare(T x, T y)
                {
                    int result = new CaseInsensitiveComparer().Compare(x.Name, y.Name);
                    if (result==0) return new CaseInsensitiveComparer().Compare(x.Description, y.Description);
                    else return result;
                }
            }
            public class ZtoA : IComparer<T>
            {
                public int Compare(T x, T y)
                {
                    int result = new CaseInsensitiveComparer().Compare(y.Name, x.Name);
                    if (result == 0) return new CaseInsensitiveComparer().Compare(y.Description, x.Description);
                    else return result;
                }
            }

        }
    }
}
