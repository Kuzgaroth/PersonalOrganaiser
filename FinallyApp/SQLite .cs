using Android.Database.Sqlite;
using Android.Content;
using Android.Util;

namespace OrgDatabase1
{
    public class DatabaseHelper : SQLiteOpenHelper
    {
        public const string NOTE_TABLE = "Notes";
        public const string EVENT_TABLE = "Events";
        public const string TODO_TABLE = "Todos";
        public const string COLUMN_ID = "_id";
        public const string COLUMN_NAME = "name";
        public const string COLUMN_CREATION_DATE = "creation_date";
        public const string COLUMN_DESCRIPTION = "description";
        public const string COLUMN_DEADLINE = "deadline";
        public const string TABLE_NAME = "table_name";
        public const string COLUMN_STATUS = "status";
        public const string COLUMN_START_TIME = "start_time";
        public const string COLUMN_END_TIME = "end_time";
        public static readonly string DbName = "Org_database.db";
        public static readonly int DatabaseVersion = 9;
        public DatabaseHelper(Context context) : base(context, DbName, null, DatabaseVersion) { }
        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL("CREATE TABLE [" + NOTE_TABLE + "] ([" + COLUMN_ID + "] INTEGER PRIMARY KEY AUTOINCREMENT, [" + COLUMN_NAME + "] TEXT NOT NULL, [" + COLUMN_DESCRIPTION + "] TEXT, [" + COLUMN_CREATION_DATE + "] INTEGER NOT NULL);");
            db.ExecSQL("CREATE TABLE [" + EVENT_TABLE + "] ([" + COLUMN_ID + "] INTEGER PRIMARY KEY AUTOINCREMENT, [" + COLUMN_NAME + "] TEXT NOT NULL, [" + COLUMN_DESCRIPTION + "] TEXT, [" + COLUMN_CREATION_DATE + "] INTEGER NOT NULL, [" + COLUMN_START_TIME + "] INTEGER NOT NULL, [" + COLUMN_END_TIME + "] INTEGER NOT NULL, [" + COLUMN_STATUS+"] INTEGER NOT NULL);");
            db.ExecSQL("CREATE TABLE [" + TODO_TABLE + "] ([" + COLUMN_ID + "] INTEGER PRIMARY KEY AUTOINCREMENT, [" + COLUMN_NAME + "] TEXT NOT NULL, [" + COLUMN_DESCRIPTION + "] TEXT, [" + COLUMN_CREATION_DATE + "] INTEGER NOT NULL, [" + COLUMN_DEADLINE + "] INTEGER NOT NULL, [" + COLUMN_STATUS + "] INTEGER NOT NULL);");
            Log.Debug("DATABASE","Access");
           
        }
        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL("DROP TABLE IF EXISTS [" + NOTE_TABLE+"]");
            db.ExecSQL("DROP TABLE IF EXISTS [" + TODO_TABLE + "]");
            db.ExecSQL("DROP TABLE IF EXISTS [" + EVENT_TABLE + "]");
            OnCreate(db);
        }
    }
}