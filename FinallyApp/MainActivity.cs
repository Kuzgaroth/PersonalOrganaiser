using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace FinallyApp
{
    [Activity(Label = "@string/title_summary")]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        const int CHANGING_TODO_CODE = 20, CHANGING_NOTE_CODE = 10, CHANGING_EVENT_CODE = 30;
        const int NEW_ELEMENT = 1;
        const int SETTINGS_CODE = 100;
        public const string ACTION_TYPE = "action_type";
        public const string ACTION_DELETE = "delete";
        public const string ACTION_CHANGE = "change";
        TextView textMessage;
        IMenuItem create_item, sort_item;
        BottomNavigationView navigation;
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        NoteAlbumAdapter nAdapter;
        TodoAlbumAdapter tAdapter;
        EventAlbumAdapter eAdapter;
        SummaryAdapter summaryAdapter;
        Toolbar toolbar;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            SetContentView(Resource.Layout.activity_main);
            textMessage = FindViewById<TextView>(Resource.Id.message);
            navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            toolbar.SetTitle(Resource.String.title_summary);
            toolbar.SetBackgroundResource(Resource.Color.SummaryColor);
            nAdapter = new NoteAlbumAdapter();
            tAdapter = new TodoAlbumAdapter(this);
            eAdapter = new EventAlbumAdapter(this);
            

            summaryAdapter = new SummaryAdapter(Selection.FormSummary(), this);
            summaryAdapter.ItemClick += OnSummaryItemClick;
            tAdapter.ItemClick += OnItemClick;
            nAdapter.ItemClick += OnItemClick;
            eAdapter.ItemClick += OnItemClick;

            mRecyclerView.SetAdapter(summaryAdapter);
            ActionsFromNotificationsReciever.NotifyAtChanges += ActionsForAdapters;
            NotificationPublisher.NotifyStatusChange += ActionsForAdapters;
        }
        
        protected override void OnStart()
        {
            base.OnStart();
            
        }
        protected override void OnResume()
        {
            base.OnResume();
            
        }
        void OnSummaryItemClick(object sender, int position)
        {
            Intent intent;
            int type = summaryAdapter.GetItemViewType(position);
            int id = (int)summaryAdapter.GetItemId(position);
            switch (type)
            {
                case SummaryAdapter.NOTE_TYPE:
                    
                    intent = new Intent(this, typeof(NoteItemPageActivity));
                    intent.PutExtra(DataBaseProvider.InterfaceConsts.Id, id);
                    StartActivityForResult(intent,CHANGING_NOTE_CODE);
                    break;
                case SummaryAdapter.TODO_TYPE:
                    intent = new Intent(this, typeof(TodoItemPageActivity));
                    intent.PutExtra(DataBaseProvider.InterfaceConsts.Id, id);
                    StartActivityForResult(intent,CHANGING_TODO_CODE);
                    break;
                case SummaryAdapter.EVENT_TYPE:
                    intent = new Intent(this, typeof(EventItemPageActivity));
                    intent.PutExtra(DataBaseProvider.InterfaceConsts.Id, id);
                    StartActivityForResult(intent,CHANGING_EVENT_CODE);
                    break;
            }

        }
        void OnItemClick(object sender, int position)
        {
            Intent intent;
            int id = (int)mRecyclerView.GetAdapter().GetItemId(position);
            switch (navigation.SelectedItemId)
            {
                
                case Resource.Id.navigation_note:
                    intent = new Intent(this, typeof(NoteItemPageActivity));
                    intent.PutExtra(DataBaseProvider.InterfaceConsts.Id, id);
                    StartActivityForResult(intent,CHANGING_NOTE_CODE);
                    break;
                case Resource.Id.navigation_event:
                    intent = new Intent(this, typeof(EventItemPageActivity));
                    intent.PutExtra(DataBaseProvider.InterfaceConsts.Id, id);
                    StartActivityForResult(intent,CHANGING_EVENT_CODE);
                    break;
                case Resource.Id.navigation_todo:
                    intent = new Intent(this, typeof(TodoItemPageActivity));
                    intent.PutExtra(DataBaseProvider.InterfaceConsts.Id, id);
                    StartActivityForResult(intent,CHANGING_TODO_CODE);
                    break;
            }
        }
       
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_summary:
                    toolbar.SetTitle(Resource.String.title_summary);
                    toolbar.SetBackgroundResource(Resource.Color.SummaryColor);
                    mRecyclerView.SetAdapter(summaryAdapter);
                    create_item.SetVisible(false);
                    sort_item.SetVisible(false);
                    textMessage.Visibility = ViewStates.Invisible;
                    return true;
                case Resource.Id.navigation_note:
                    toolbar.SetTitle(Resource.String.title_note);
                    toolbar.SetBackgroundResource(Resource.Color.NoteMainColor);
                    mRecyclerView.SetAdapter(nAdapter);
                    create_item.SetVisible(true);
                    sort_item.SetVisible(true);
                    textMessage.SetText(Resource.String.zero_notes);
                    if (nAdapter.ItemCount == 0)
                    {
                        textMessage.Visibility = ViewStates.Visible;
                    }
                    else textMessage.Visibility = ViewStates.Invisible;
                    return true;
                case Resource.Id.navigation_todo:
                    toolbar.SetTitle(Resource.String.title_todo);
                    toolbar.SetBackgroundResource(Resource.Color.TodoMainColor);
                    create_item.SetVisible(true);
                    mRecyclerView.SetAdapter(tAdapter);
                    sort_item.SetVisible(true);
                    textMessage.SetText(Resource.String.zero_todos);
                    if (tAdapter.ItemCount == 0)
                    {
                        textMessage.Visibility = ViewStates.Visible;
                    }
                    else textMessage.Visibility = ViewStates.Invisible;
                    return true;
                case Resource.Id.navigation_event:
                    toolbar.SetTitle(Resource.String.title_event);
                    toolbar.SetBackgroundResource(Resource.Color.EventMainColor);
                    mRecyclerView.SetAdapter(eAdapter);
                    create_item.SetVisible(true);
                    sort_item.SetVisible(true);
                    textMessage.SetText(Resource.String.zero_events);
                    if (eAdapter.ItemCount == 0)
                    {
                        textMessage.Visibility = ViewStates.Visible;
                    }
                    else textMessage.Visibility = ViewStates.Invisible;
                    return true;
            }
            return false;
        }
        protected override void OnSaveInstanceState(Bundle savedInstanceState)
        {
            
            base.OnSaveInstanceState(savedInstanceState);
        }
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            var delete_item = menu.FindItem(Resource.Id.menu_delete);
            delete_item.SetVisible(false);
            var edit_item = menu.FindItem(Resource.Id.menu_edit);
            edit_item.SetVisible(false);
            var save_item = menu.FindItem(Resource.Id.menu_save);
            save_item.SetVisible(false);
            var cancel_item = menu.FindItem(Resource.Id.menu_cancel);
            cancel_item.SetVisible(false);
            create_item = menu.FindItem(Resource.Id.menu_create);
            create_item.SetVisible(false);
            sort_item = menu.FindItem(Resource.Id.sort_type);
            sort_item.SetVisible(false);
            var cur_sort = menu.FindItem(Resource.Id.old_to_new);
            cur_sort.SetChecked(true);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Intent intent;
            if (item.IsCheckable) item.SetChecked(true);
            switch (item.ItemId)
            {
                case Resource.Id.menu_create:
                    switch (navigation.SelectedItemId)
                    {
                        case Resource.Id.navigation_note:
                            intent = new Intent(this, typeof(CreationActivity));
                            intent.PutExtra(DataBaseProvider.InterfaceConsts.TableName, DataBaseProvider.InterfaceConsts.NoteTable);
                            StartActivityForResult(intent, NEW_ELEMENT);                           
                            break;
                        case Resource.Id.navigation_todo:
                            intent = new Intent(this, typeof(CreationActivity));
                            intent.PutExtra(DataBaseProvider.InterfaceConsts.TableName, DataBaseProvider.InterfaceConsts.TodoTable);

                            StartActivityForResult(intent, NEW_ELEMENT);
                            break;
                        case Resource.Id.navigation_event:
                            intent = new Intent(this, typeof(CreationActivity));
                            intent.PutExtra(DataBaseProvider.InterfaceConsts.TableName, DataBaseProvider.InterfaceConsts.EventTable);
                            
                            StartActivityForResult(intent, NEW_ELEMENT);
                            break;
                    }
                    return true;
                case Resource.Id.settings:
                    StartActivityForResult(new Intent(this, typeof(SettingsActivity)), SETTINGS_CODE);
                    return true;
                case Resource.Id.ZtoA:
                    switch (navigation.SelectedItemId)
                    {
                        case Resource.Id.navigation_note:
                            Items.ListOfItems.Notes.Sort(new Items.Sorts<Items.BaseItem>.ZtoA());
                            nAdapter.NotifyDataSetChanged();
                            break;
                        case Resource.Id.navigation_todo:
                            Items.ListOfItems.Todos.Sort(new Items.Sorts<Items.BaseItem>.ZtoA());
                            tAdapter.NotifyDataSetChanged();
                            break;
                        case Resource.Id.navigation_event:
                            Items.ListOfItems.Events.Sort(new Items.Sorts<Items.BaseItem>.ZtoA());
                            eAdapter.NotifyDataSetChanged();
                            break;
                    }
                    return true;
                case Resource.Id.AtoZ:
                    switch (navigation.SelectedItemId)
                    {
                        case Resource.Id.navigation_note:
                            Items.ListOfItems.Notes.Sort(new Items.Sorts<Items.BaseItem>.AtoZ());
                            nAdapter.NotifyDataSetChanged();
                            break;
                        case Resource.Id.navigation_todo:
                            Items.ListOfItems.Todos.Sort(new Items.Sorts<Items.BaseItem>.AtoZ());
                            tAdapter.NotifyDataSetChanged();
                            break;
                        case Resource.Id.navigation_event:
                            Items.ListOfItems.Events.Sort(new Items.Sorts<Items.BaseItem>.AtoZ());
                            eAdapter.NotifyDataSetChanged();
                            break;
                    }
                    return true;
                case Resource.Id.new_to_old:
                    switch (navigation.SelectedItemId)
                    {
                        case Resource.Id.navigation_note:
                            Items.ListOfItems.Notes.Sort(new Items.Sorts<Items.BaseItem>.NewToOld());
                            nAdapter.NotifyDataSetChanged();
                            break;
                        case Resource.Id.navigation_todo:
                            Items.ListOfItems.Todos.Sort(new Items.Sorts<Items.BaseItem>.NewToOld());
                            tAdapter.NotifyDataSetChanged();
                            break;
                        case Resource.Id.navigation_event:
                            Items.ListOfItems.Events.Sort(new Items.Sorts<Items.BaseItem>.NewToOld());
                            eAdapter.NotifyDataSetChanged();
                            break;
                    }
                    return true;
                case Resource.Id.old_to_new:
                    switch (navigation.SelectedItemId)
                    {
                        case Resource.Id.navigation_note:
                            Items.ListOfItems.Notes.Sort(new Items.Sorts<Items.BaseItem>.OldToNew());
                            nAdapter.NotifyDataSetChanged();
                            break;
                        case Resource.Id.navigation_todo:
                            Items.ListOfItems.Todos.Sort(new Items.Sorts<Items.BaseItem>.OldToNew());
                            tAdapter.NotifyDataSetChanged();
                            break;
                        case Resource.Id.navigation_event:
                            Items.ListOfItems.Events.Sort(new Items.Sorts<Items.BaseItem>.OldToNew());
                            eAdapter.NotifyDataSetChanged();
                            break;
                    }
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode==Result.Ok)
            {
                switch (requestCode)
                {
                    case NEW_ELEMENT:
                        mRecyclerView.GetAdapter().NotifyItemInserted(mRecyclerView.GetAdapter().ItemCount - 1);
                        summaryAdapter.SwapList(Selection.FormSummary());
                        if (mRecyclerView.GetAdapter().ItemCount == 1) textMessage.Visibility = ViewStates.Invisible;
                        break;
                    case CHANGING_NOTE_CODE:
                        switch (data.GetStringExtra(ACTION_TYPE))
                        {
                            case ACTION_DELETE:
                                nAdapter.NotifyItemRemoved(data.GetIntExtra(Items.POSITION_KEY, -1));
                                if (nAdapter.ItemCount == 0) textMessage.Visibility = ViewStates.Visible;
                                break;
                            case ACTION_CHANGE:
                                nAdapter.NotifyItemChanged(data.GetIntExtra(Items.POSITION_KEY, -1));
                                break;
                        }
                        summaryAdapter.SwapList(Selection.FormSummary());
                        break;
                    case CHANGING_TODO_CODE:
                        switch (data.GetStringExtra(ACTION_TYPE))
                        {
                            case ACTION_DELETE:
                                tAdapter.NotifyItemRemoved(data.GetIntExtra(Items.POSITION_KEY, -1));
                                if (tAdapter.ItemCount == 0) textMessage.Visibility = ViewStates.Visible;
                                break;
                            case ACTION_CHANGE:
                                tAdapter.NotifyItemChanged(data.GetIntExtra(Items.POSITION_KEY, -1));
                                break;
                        }
                        summaryAdapter.SwapList(Selection.FormSummary());
                        break;
                    case CHANGING_EVENT_CODE:
                        switch (data.GetStringExtra(ACTION_TYPE))
                        {
                            case ACTION_DELETE:
                                eAdapter.NotifyItemRemoved(data.GetIntExtra(Items.POSITION_KEY, -1));
                                if (eAdapter.ItemCount == 0) textMessage.Visibility = ViewStates.Visible;
                                break;
                            case ACTION_CHANGE:
                                eAdapter.NotifyItemChanged(data.GetIntExtra(Items.POSITION_KEY, -1));
                                break;
                        }
                        summaryAdapter.SwapList(Selection.FormSummary());
                        break;
                    case SETTINGS_CODE:
                        summaryAdapter.SwapList(Selection.FormSummary());
                        nAdapter.NotifyDataSetChanged();
                        tAdapter.NotifyDataSetChanged();
                        eAdapter.NotifyDataSetChanged();
                        break;
                }
            }
        }
        public void ActionsForAdapters(string table_name, int id, string action_type)
        {
            int position;
            switch (table_name)
            {
                case DataBaseProvider.InterfaceConsts.TodoTable:
                    position = Items.ListOfItems.GetPositionFromId(Items.ListOfItems.Todos, id);
                    Items.Todo Titem = (Items.Todo)Items.ListOfItems.GetItemFromId(Items.ListOfItems.Todos, id);
                    switch (action_type)
                    {
                        case ActionsFromNotificationsReciever.ACTION_DELETE:
                            Items.ListOfItems.Todos.RemoveAt(position);
                            tAdapter.NotifyItemRemoved(position);
                            if (tAdapter.ItemCount == 0) textMessage.Visibility = ViewStates.Visible;
                            break;
                        case ActionsFromNotificationsReciever.ACTION_HANDLE:
                            Titem.Status = DataBaseProvider.InterfaceConsts.StatusDone;
                            tAdapter.NotifyItemChanged(position);
                            break;
                        case ActionsFromNotificationsReciever.ACTION_MISSED:
                            Titem.Status = DataBaseProvider.InterfaceConsts.StatusMissed;
                            tAdapter.NotifyItemChanged(position);
                            break;
                    }
                    break;
                case DataBaseProvider.InterfaceConsts.EventTable:
                    position = Items.ListOfItems.GetPositionFromId(Items.ListOfItems.Events, id);
                    Items.Event Eitem = (Items.Event)Items.ListOfItems.GetItemFromId(Items.ListOfItems.Events, id);

                    switch (action_type)
                    {
                        case ActionsFromNotificationsReciever.ACTION_DELETE:
                            Items.ListOfItems.Events.RemoveAt(position);
                            eAdapter.NotifyItemRemoved(position);
                            if (eAdapter.ItemCount == 0) textMessage.Visibility = ViewStates.Visible;
                            break;
                        case ActionsFromNotificationsReciever.ACTION_STARTED:
                            Eitem.Status = DataBaseProvider.InterfaceConsts.StatusStarted;
                            eAdapter.NotifyItemChanged(position);
                            break;
                        case ActionsFromNotificationsReciever.ACTION_ENDED:
                            Eitem.Status = DataBaseProvider.InterfaceConsts.StatusEnded;
                            eAdapter.NotifyItemChanged(position);
                            break;
                    }
                    break;
            }
            summaryAdapter.SwapList(Selection.FormSummary());
        }
        protected override void OnStop()
        {
            base.OnStop();
        }
        protected override void OnDestroy()
        {
            ActionsFromNotificationsReciever.NotifyAtChanges -= ActionsForAdapters;
            NotificationPublisher.NotifyStatusChange -= ActionsForAdapters;
            base.OnDestroy();
        }
    } 
}

