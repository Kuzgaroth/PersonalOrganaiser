using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System;
using Android.Preferences;
using System.Collections.Generic;
using Android.Content;
using Android.App;
using Android.Support.V4.Content;
using Android.Support.V4.Graphics.Drawable;

namespace FinallyApp
{
    public class NoteViewHolder : RecyclerView.ViewHolder
    {
        public TextView Description { get; private set; }
        public TextView Name { get; private set; }
        public TextView Creation_date { get; private set; }
        private CardView Card { get; set; }
        public NoteViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Description = itemView.FindViewById<TextView>(Resource.Id.descriptionItem);
            Name = itemView.FindViewById<TextView>(Resource.Id.headerItem);
            Creation_date = itemView.FindViewById<TextView>(Resource.Id.creation_dateItem);
            Name.Click += (sender, e) => listener(base.LayoutPosition);
            Description.Click += (sender, e) => listener(base.LayoutPosition);
            Creation_date.Click += (sender, e) => listener(base.LayoutPosition);
            Card = itemView.FindViewById<CardView>(Resource.Id.note_card);
            Card.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
    public class NoteAlbumAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public NoteAlbumAdapter()
        {
        }
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.note_item_view, parent, false);
            NoteViewHolder vh = new NoteViewHolder(itemView, OnClick);
            return vh;
        }
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            NoteViewHolder vh = holder as NoteViewHolder;
            vh.Name.Text = Items.ListOfItems.Notes[position].Name;
            vh.Description.Text = Items.ListOfItems.Notes[position].Description;
            vh.Creation_date.Text = Selection.ConvertToCurrDateFormat(Items.ListOfItems.Notes[position].CreationDate);
        }

        public override int ItemCount
        {
            get { return Items.ListOfItems.Notes.Count; }
        }
        public override long GetItemId(int position)
        {
            return Items.ListOfItems.Notes[position].Id;
        }
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
            
        }

    }


    public class TodoViewHolder : RecyclerView.ViewHolder
    {
        public TextView Description { get; private set; }
        public TextView Name { get; private set; }
        public TextView Creation_date { get; private set; }
        private CardView Card { get; set; }
        public TextView Deadline { get; set; }
        public ImageView AlarmImage { get; private set; }
        public TextView StatusText { get; private set; }
        public TodoViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Description = itemView.FindViewById<TextView>(Resource.Id.todo_description_view);
            Name = itemView.FindViewById<TextView>(Resource.Id.todo_header_view);
            Creation_date = itemView.FindViewById<TextView>(Resource.Id.todo_creation_date_view);
            Deadline = itemView.FindViewById<TextView>(Resource.Id.todo_deadline_view);
            Name.Click += (sender, e) => listener(base.LayoutPosition);
            Description.Click += (sender, e) => listener(base.LayoutPosition);
            Creation_date.Click += (sender, e) => listener(base.LayoutPosition);
            Card = itemView.FindViewById<CardView>(Resource.Id.todo_card);
            Card.Click += (sender, e) => listener(base.LayoutPosition);
            Deadline.Click+= (sender, e) => listener(base.LayoutPosition);
            AlarmImage = itemView.FindViewById<ImageView>(Resource.Id.todo_alarm_icon);
            AlarmImage.Click += (sender, e) => listener(base.LayoutPosition);
            StatusText = itemView.FindViewById<TextView>(Resource.Id.todo_text_status);
            StatusText.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
    public class TodoAlbumAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        Context context;
        public TodoAlbumAdapter(Context context)
        { 
            this.context = context;
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.todo_item_view, parent, false);
            TodoViewHolder vh = new TodoViewHolder(itemView, OnClick);
            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TodoViewHolder vh = holder as TodoViewHolder;
            vh.Name.Text = Items.ListOfItems.Todos[position].Name;
            vh.Description.Text = Items.ListOfItems.Todos[position].Description;
            vh.Creation_date.Text = Selection.ConvertToCurrDateFormat(Items.ListOfItems.Todos[position].CreationDate);
            vh.Deadline.Text = Selection.ConvertToCurrDateFormat(Items.ListOfItems.Todos[position].DeadLine);
            switch (Items.ListOfItems.Todos[position].Status)
            {
                case DataBaseProvider.InterfaceConsts.StatusActive:
                    DrawableCompat.SetTint(vh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.SummaryColor));
                    vh.StatusText.SetText(Resource.String.status_active);
                    break;
                case DataBaseProvider.InterfaceConsts.StatusMissed:
                    DrawableCompat.SetTint(vh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.EventMainColor));
                    vh.StatusText.SetText(Resource.String.status_missed);
                    break;
                case DataBaseProvider.InterfaceConsts.StatusDone:
                    DrawableCompat.SetTint(vh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.SettingsColor));
                    vh.StatusText.SetText(Resource.String.status_done);
                    break;
            }
        }

        public override int ItemCount
        {
            get { return Items.ListOfItems.Todos.Count; }
        }
        public override long GetItemId(int position)
        {
            return Items.ListOfItems.Todos[position].Id;
        }
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }
    public class EventViewHolder : RecyclerView.ViewHolder
    {
        public TextView Description { get; private set; }
        public TextView Name { get; private set; }
        public TextView Creation_date { get; private set; }
        private CardView Card { get; set; }
        public TextView StartTime { get; private set; }
        public TextView EndTime { get; private set; }
        public ImageView AlarmImage { get; private set; }
        public TextView StatusText { get; private set; }
        public EventViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Description = itemView.FindViewById<TextView>(Resource.Id.event_description_view);
            Name = itemView.FindViewById<TextView>(Resource.Id.event_header_view);
            Creation_date = itemView.FindViewById<TextView>(Resource.Id.event_creation_date_view);
            StartTime = itemView.FindViewById<TextView>(Resource.Id.events_start_item);
            StartTime.Click += (sender, e) => listener(base.LayoutPosition);
            EndTime = itemView.FindViewById<TextView>(Resource.Id.events_end_item);
            EndTime.Click += (sender, e) => listener(base.LayoutPosition);
            Name.Click += (sender, e) => listener(base.LayoutPosition);
            Description.Click += (sender, e) => listener(base.LayoutPosition);
            Creation_date.Click += (sender, e) => listener(base.LayoutPosition);
            Card = itemView.FindViewById<CardView>(Resource.Id.event_card);
            Card.Click += (sender, e) => listener(base.LayoutPosition);
            AlarmImage = itemView.FindViewById<ImageView>(Resource.Id.event_alarm_icon);
            AlarmImage.Click += (sender, e) => listener(base.LayoutPosition);
            StatusText = itemView.FindViewById<TextView>(Resource.Id.event_text_status);
            StatusText.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
    public class EventAlbumAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        Context context;
        public EventAlbumAdapter(Context context)
        {
            this.context = context;
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.event_item_view, parent, false);
            EventViewHolder vh = new EventViewHolder(itemView, OnClick);
            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            EventViewHolder vh = holder as EventViewHolder;
            vh.Name.Text = Items.ListOfItems.Events[position].Name;
            vh.Description.Text = Items.ListOfItems.Events[position].Description;
            vh.Creation_date.Text = Selection.ConvertToCurrDateFormat(Items.ListOfItems.Events[position].CreationDate);
            vh.StartTime.Text = Selection.ConvertToCurrDateFormat(Items.ListOfItems.Events[position].StartTime);
            vh.EndTime.Text = Selection.ConvertToCurrDateFormat(Items.ListOfItems.Events[position].EndTime);
            vh.StatusText.Text = Items.ListOfItems.Events[position].Status.ToString();
            switch (Items.ListOfItems.Events[position].Status)
            {
                case DataBaseProvider.InterfaceConsts.StatusWaiting:
                    DrawableCompat.SetTint(vh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.SummaryColor));
                    vh.StatusText.SetText(Resource.String.status_waiting);
                    break;
                case DataBaseProvider.InterfaceConsts.StatusStarted:
                    DrawableCompat.SetTint(vh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.StartedEventColor));
                    vh.StatusText.SetText(Resource.String.status_started);
                    break;
                case DataBaseProvider.InterfaceConsts.StatusEnded:
                    DrawableCompat.SetTint(vh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.SettingsColor));
                    vh.StatusText.SetText(Resource.String.status_ended);
                    break;
            }
        }
        public override int ItemCount
        {
            get { return Items.ListOfItems.Events.Count; }
        }
        public override long GetItemId(int position)
        {
            return Items.ListOfItems.Events[position].Id;
        }
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }
    public class HeaderViewHolder : RecyclerView.ViewHolder
    {
        public TextView Header { get; private set; }
        public HeaderViewHolder(View itemView) : base(itemView)
        {
            Header = itemView.FindViewById<TextView>(Resource.Id.summary_header);
        }
    }
    public class SummaryAdapter :RecyclerView.Adapter
    {
        public const int NOTE_TYPE = 0;
        public const int TODO_TYPE = 1;
        public const int EVENT_TYPE = 2;
        public const int HEADER_TYPE = 3;
        static List<int> indexes;
        List<Items.BaseItem> summary_list;
        public event EventHandler<int> ItemClick;
        Context context;
        public SummaryAdapter(List<Items.BaseItem> list, Context context)
        {
            this.context = context;
            summary_list = list;
            indexes = new List<int> { };
            for (int i=0;i<summary_list.Count;i++)
            {
                if (summary_list[i] == null) indexes.Add(i);
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView;
            
            switch (viewType)
            {
                case NOTE_TYPE:
                    itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.note_item_view, parent, false);
                    NoteViewHolder vhN = new NoteViewHolder(itemView, OnClick);
                    return vhN;
                case TODO_TYPE:
                    itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.todo_item_view, parent, false);
                    TodoViewHolder vhT = new TodoViewHolder(itemView, OnClick);
                    return vhT;
                case EVENT_TYPE:
                    itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.event_item_view, parent, false);
                    EventViewHolder vhE = new EventViewHolder(itemView, OnClick);
                    return vhE;
                case HEADER_TYPE:
                    itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.summary_header, parent, false);
                    HeaderViewHolder vhh = new HeaderViewHolder(itemView);
                    return vhh;
                default:
                    throw new ArgumentException();

            }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            switch (GetItemViewType(position))
            {
                case NOTE_TYPE:
                    NoteViewHolder NoteVh = holder as NoteViewHolder;
                    NoteVh.Name.Text = ((Items.Note)summary_list[position]).Name;
                    NoteVh.Description.Text = ((Items.Note)summary_list[position]).Description;
                    NoteVh.Creation_date.Text = Selection.ConvertToCurrDateFormat(((Items.Note)summary_list[position]).CreationDate);
                    break;
                case TODO_TYPE:
                    TodoViewHolder TodoVh = holder as TodoViewHolder;
                    TodoVh.Name.Text = ((Items.Todo)summary_list[position]).Name;
                    TodoVh.Description.Text = ((Items.Todo)summary_list[position]).Description;
                    TodoVh.Creation_date.Text = Selection.ConvertToCurrDateFormat(((Items.Todo)summary_list[position]).CreationDate);
                    TodoVh.Deadline.Text = Selection.ConvertToCurrDateFormat(((Items.Todo)summary_list[position]).DeadLine);
                    switch (((Items.Todo)summary_list[position]).Status)
                    {
                        case DataBaseProvider.InterfaceConsts.StatusActive:
                            DrawableCompat.SetTint(TodoVh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.SummaryColor));
                            TodoVh.StatusText.SetText(Resource.String.status_active);
                            break;
                        case DataBaseProvider.InterfaceConsts.StatusMissed:
                            DrawableCompat.SetTint(TodoVh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.EventMainColor));
                            TodoVh.StatusText.SetText(Resource.String.status_missed);
                            break;
                        case DataBaseProvider.InterfaceConsts.StatusDone:
                            DrawableCompat.SetTint(TodoVh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.SettingsColor));
                            TodoVh.StatusText.SetText(Resource.String.status_done);
                            break;
                    }
                    break;
                case EVENT_TYPE:
                    EventViewHolder EventVh = holder as EventViewHolder;
                    EventVh.Name.Text = ((Items.Event)summary_list[position]).Name;
                    EventVh.Description.Text = ((Items.Event)summary_list[position]).Description;
                    EventVh.Creation_date.Text = Selection.ConvertToCurrDateFormat(((Items.Event)summary_list[position]).CreationDate);
                    EventVh.StartTime.Text = Selection.ConvertToCurrDateFormat(((Items.Event)summary_list[position]).StartTime);
                    EventVh.EndTime.Text = Selection.ConvertToCurrDateFormat(((Items.Event)summary_list[position]).EndTime);
                    switch (((Items.Event)summary_list[position]).Status)
                    {
                        case DataBaseProvider.InterfaceConsts.StatusWaiting:
                            DrawableCompat.SetTint(EventVh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.SummaryColor));
                            EventVh.StatusText.SetText(Resource.String.status_waiting);
                            break;
                        case DataBaseProvider.InterfaceConsts.StatusStarted:
                            DrawableCompat.SetTint(EventVh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.StartedEventColor));
                            EventVh.StatusText.SetText(Resource.String.status_started);
                            break;
                        case DataBaseProvider.InterfaceConsts.StatusEnded:
                            DrawableCompat.SetTint(EventVh.AlarmImage.Drawable, ContextCompat.GetColor(context, Resource.Color.SettingsColor));
                            EventVh.StatusText.SetText(Resource.String.status_ended);
                            break;
                    }
                    break;
                case HEADER_TYPE:
                    ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
                    HeaderViewHolder HeaderVh = holder as HeaderViewHolder;
                    if (Items.ListOfItems.Notes.Count==0 && position == indexes[0]) HeaderVh.Header.SetText(Resource.String.zero_notes);
                    else if (position == indexes[0]) HeaderVh.Header.Text = pref.GetString(SettingsActivity.NOTE_CHOSEN_METHOD_KEY, SettingsActivity.BASE_NOTE_METHOD);
                    if (Items.ListOfItems.Todos.Count == 0 && position == indexes[1]) HeaderVh.Header.SetText(Resource.String.zero_todos);
                    else if (position == indexes[1]) HeaderVh.Header.Text = pref.GetString(SettingsActivity.TODO_CHOSEN_METHOD_KEY, SettingsActivity.BASE_TODO_METHOD);
                    if (Items.ListOfItems.Events.Count == 0 && position == indexes[2]) HeaderVh.Header.SetText(Resource.String.zero_events);
                    else if (position == indexes[2]) HeaderVh.Header.Text = pref.GetString(SettingsActivity.EVENT_CHOSEN_METHOD_KEY,SettingsActivity.BASE_EVENT_METHOD);
                    break;
            }
        }
        public void SwapList(List<Items.BaseItem> new_list)
        {
            summary_list.Clear();
            summary_list.AddRange(new_list);
            indexes.Clear();
            for (int i = 0; i < summary_list.Count; i++)
            {
                if (summary_list[i] == null) indexes.Add(i);
            }
            NotifyDataSetChanged();
        }
        public override int ItemCount
        {
            get 
            {
                return summary_list.Count;
            }
        }
        public override int GetItemViewType(int position)
        {
            if (summary_list[position] == null) return HEADER_TYPE;
            if (summary_list[position] as Items.Note != null) return NOTE_TYPE;
            if (summary_list[position] as Items.Todo != null) return TODO_TYPE;
            if (summary_list[position] as Items.Event != null) return EVENT_TYPE;
            return -1;
        }
        public override long GetItemId(int position)
        {
            if (summary_list[position] != null) return summary_list[position].Id;
            else return -1;
        }
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }
}