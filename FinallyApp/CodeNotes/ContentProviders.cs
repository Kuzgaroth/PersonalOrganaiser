using Android.Net;
using Android.Content;
using Android.Database;
using OrgDatabase1;

namespace FinallyApp
{
    [ContentProvider(new string[] { AUTHORITY })]
    public class DataBaseProvider : ContentProvider
    {

        public const string AUTHORITY = "ru.kuznetsov.provider.personal_organaiser";
        static readonly string NOTE_PATH = "notes";
        static readonly string TODO_PATH = "todos";
        static readonly string EVENT_PATH = "events";
        public static readonly Uri NOTE_URI = Uri.Parse("content://" + AUTHORITY + "/" + NOTE_PATH);
        public static readonly Uri TODO_URI = Uri.Parse("content://" + AUTHORITY + "/" + TODO_PATH);
        public static readonly Uri EVENT_URI = Uri.Parse("content://" + AUTHORITY + "/" + EVENT_PATH);
        public const string ITEMS_MIME_TYPE = ContentResolver.CursorDirBaseType + "/vnd.ru.kuznetsov.orgdatabase";
        public const string ITEM_MIME_TYPE = ContentResolver.CursorItemBaseType + "/vnd.ru.kuznetsov.orgdatabase";

        public new static class InterfaceConsts
        {
            public const string Id = DatabaseHelper.COLUMN_ID;
            public const string Name = DatabaseHelper.COLUMN_NAME;
            public const string Description = DatabaseHelper.COLUMN_DESCRIPTION;
            public const string CreationDate = DatabaseHelper.COLUMN_CREATION_DATE;
            public const string Deadline = DatabaseHelper.COLUMN_DEADLINE;
            public const string NoteTable = DatabaseHelper.NOTE_TABLE;
            public const string TodoTable = DatabaseHelper.TODO_TABLE;
            public const string EventTable = DatabaseHelper.EVENT_TABLE;
            public const string TableName = DatabaseHelper.TABLE_NAME;
            public const string Status = DatabaseHelper.COLUMN_STATUS;
            public const string StartTime = DatabaseHelper.COLUMN_START_TIME;
            public const string EndTime = DatabaseHelper.COLUMN_END_TIME;
            public const byte StatusActive = 1;
            public const byte StatusMissed = 0;
            public const byte StatusDone = 2;
            public const byte StatusWaiting = 3;
            public const byte StatusStarted = 4;
            public const byte StatusEnded = 5;
        }
        DatabaseHelper DbHelper;

        public override bool OnCreate()
        {
            DbHelper = new DatabaseHelper(Context);
            return true;
        }

        const int GET_NOTES_ALL = 100; 
        const int GET_NOTE_ONE = 101;
        const int GET_TODOS_ALL = 200;
        const int GET_TODO_ONE = 201;
        const int GET_EVENTS_ALL = 300;
        const int GET_EVENT_ONE = 301;
        static UriMatcher uriMatcher = BuildUriMatcher();
        static UriMatcher BuildUriMatcher()
        {
            var matcher = new UriMatcher(UriMatcher.NoMatch);
            matcher.AddURI(AUTHORITY, NOTE_PATH, GET_NOTES_ALL); 
            matcher.AddURI(AUTHORITY, NOTE_PATH + "/#", GET_NOTE_ONE); 
            matcher.AddURI(AUTHORITY, TODO_PATH, GET_TODOS_ALL); 
            matcher.AddURI(AUTHORITY, TODO_PATH + "/#", GET_TODO_ONE); 
            matcher.AddURI(AUTHORITY, EVENT_PATH, GET_EVENTS_ALL); 
            matcher.AddURI(AUTHORITY, EVENT_PATH + "/#", GET_EVENT_ONE); 

            return matcher;
        }
        public override ICursor Query(Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            string id;
            switch (uriMatcher.Match(uri))
            {
                case GET_NOTES_ALL:
                    return DbHelper.ReadableDatabase.Query(InterfaceConsts.NoteTable, projection, selection, selectionArgs, null, null, sortOrder);
                case GET_NOTE_ONE:
                    id = uri.LastPathSegment;
                    return DbHelper.ReadableDatabase.Query(InterfaceConsts.NoteTable, projection, InterfaceConsts.Id + "=" + id, selectionArgs, null, null, sortOrder);
                case GET_TODOS_ALL:
                    return DbHelper.ReadableDatabase.Query(InterfaceConsts.TodoTable, projection, selection, selectionArgs, null, null, sortOrder);
                case GET_TODO_ONE:
                    id = uri.LastPathSegment;
                    return DbHelper.ReadableDatabase.Query(InterfaceConsts.TodoTable, projection, InterfaceConsts.Id + "=" + id, selectionArgs, null, null, sortOrder);
                case GET_EVENTS_ALL:
                    return DbHelper.ReadableDatabase.Query(InterfaceConsts.EventTable, projection, selection, selectionArgs, null, null, sortOrder);
                case GET_EVENT_ONE:
                    id = uri.LastPathSegment;
                    return DbHelper.ReadableDatabase.Query(InterfaceConsts.EventTable, projection, InterfaceConsts.Id + "=" + id, selectionArgs, null, null, sortOrder);
                default:
                    throw new Java.Lang.IllegalArgumentException("Unknown Uri: " + uri);
            }
        }
        public override string GetType(Uri uri)
        {
            return (uriMatcher.Match(uri)) switch
            {
                GET_NOTES_ALL => ITEMS_MIME_TYPE,
                GET_NOTE_ONE => ITEM_MIME_TYPE,
                GET_TODO_ONE => ITEM_MIME_TYPE,
                GET_EVENT_ONE => ITEM_MIME_TYPE,
                GET_TODOS_ALL => ITEMS_MIME_TYPE,
                GET_EVENTS_ALL => ITEMS_MIME_TYPE,
                _ => throw new Java.Lang.IllegalArgumentException("Unknown Uri: " + uri),
            };
        }

        public override int Delete(Uri uri, string selection, string[] selectionArgs)
        {
            string id = uri.LastPathSegment;
            return (uriMatcher.Match(uri)) switch
            {
                GET_NOTE_ONE => DbHelper.ReadableDatabase.Delete(InterfaceConsts.NoteTable, InterfaceConsts.Id + "=" + id, selectionArgs),
                GET_TODO_ONE => DbHelper.ReadableDatabase.Delete(InterfaceConsts.TodoTable, InterfaceConsts.Id + "=" + id, selectionArgs),
                GET_EVENT_ONE => DbHelper.ReadableDatabase.Delete(InterfaceConsts.EventTable, InterfaceConsts.Id + "=" + id, selectionArgs),
                _ => throw new Java.Lang.UnsupportedOperationException(),
            };
        }

        public override Uri Insert(Uri uri, ContentValues values)
        {
            long id;
            switch (uriMatcher.Match(uri))
            {
                case GET_NOTES_ALL:
                    id = DbHelper.ReadableDatabase.Insert(InterfaceConsts.NoteTable,null,values);
                    return ContentUris.WithAppendedId(uri,id);
                case GET_TODOS_ALL:
                    id = DbHelper.ReadableDatabase.Insert(InterfaceConsts.TodoTable, null, values);
                    return ContentUris.WithAppendedId(uri, id);
                case GET_EVENTS_ALL:
                    id = DbHelper.ReadableDatabase.Insert(InterfaceConsts.EventTable, null, values);
                    return ContentUris.WithAppendedId(uri, id);
                default:
                    throw new Java.Lang.UnsupportedOperationException();
            }
        }

        public override int Update(Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            string id = uri.LastPathSegment;
            return (uriMatcher.Match(uri)) switch
            {
                GET_NOTE_ONE => DbHelper.ReadableDatabase.Update(InterfaceConsts.NoteTable, values, InterfaceConsts.Id + "=" + id, selectionArgs),
                GET_TODO_ONE => DbHelper.ReadableDatabase.Update(InterfaceConsts.TodoTable, values, InterfaceConsts.Id + "=" + id, selectionArgs),
                GET_EVENT_ONE => DbHelper.ReadableDatabase.Update(InterfaceConsts.EventTable, values, InterfaceConsts.Id + "=" + id, selectionArgs),
                _ => throw new Java.Lang.UnsupportedOperationException(),
            };
        }
    }
}