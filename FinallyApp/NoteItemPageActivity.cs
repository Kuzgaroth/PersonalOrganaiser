using Android.App;
using Android.Content;
using Uri = Android.Net.Uri;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace FinallyApp
{
    [Activity (NoHistory = true, Label ="@string/single_note")]
    public class NoteItemPageActivity : AppCompatActivity
    {
        EditText description, name;
        TextView creation_date,invalid_name;
        Toolbar toolbar;
        Intent change_intent;
        IMenuItem cancel_item,save_item,edit_item,delete_item;
        int id;
        Items.Note note;
        Uri uri;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.note_item_page);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            toolbar.SetBackgroundResource(Resource.Color.NoteMainColor);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true); 
            SupportActionBar.SetHomeButtonEnabled(true);
            description = FindViewById<EditText>(Resource.Id.note_item_description);
            name = FindViewById<EditText>(Resource.Id.note_item_name);
            creation_date = FindViewById<TextView>(Resource.Id.note_item_creation_date);
            invalid_name = FindViewById<TextView>(Resource.Id.note_edit_name_invalid);
            id = Intent.GetIntExtra(DataBaseProvider.InterfaceConsts.Id, -1);
            uri = Uri.WithAppendedPath(DataBaseProvider.NOTE_URI, id.ToString());
            note = (Items.Note)Items.ListOfItems.GetItemFromId(Items.ListOfItems.Notes, id);
            creation_date.Text = Selection.ConvertToCurrDateFormat(note.CreationDate);
        }
        protected override void OnStart()
        {
            base.OnStart();
            RefreshForms();
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
            cancel_item = menu.FindItem(Resource.Id.menu_cancel);
            delete_item = menu.FindItem(Resource.Id.menu_delete);
            edit_item = menu.FindItem(Resource.Id.menu_edit);
            Edit_buttons_visible(false);
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
                    if (ValidCheck.ValidName(name.Text, invalid_name))
                    {
                        var contentV = new ContentValues();
                        if (name.Text != note.Name)
                        {
                            contentV.Put(DataBaseProvider.InterfaceConsts.Name, name.Text);
                            note.Name = name.Text;
                        }
                        if (description.Text != note.Description)
                        {
                            contentV.Put(DataBaseProvider.InterfaceConsts.Description, description.Text);
                            note.Description = description.Text;
                        }
                        if (contentV.Size() > 0)
                        {
                            ContentResolver.Update(uri, contentV, null, null);
                            if (change_intent == null)
                            {
                                change_intent = new Intent();
                                change_intent.PutExtra(MainActivity.ACTION_TYPE, MainActivity.ACTION_CHANGE);
                                change_intent.PutExtra(Items.POSITION_KEY, Items.ListOfItems.Notes.IndexOf(note));
                                SetResult(Result.Ok, change_intent);
                            }

                        }
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
            name.Text = note.Name;
            description.Text = note.Description;
        }
        private void Edit_buttons_visible(bool v)
        {
            if (v)
            {
                name.Enabled = true;
                description.Enabled = true;
                delete_item.SetVisible(false);
                edit_item.SetVisible(false);
                save_item.SetVisible(true);
                cancel_item.SetVisible(true);
            }
            else
            {
                name.Enabled = false;
                description.Enabled = false;
                delete_item.SetVisible(true);
                edit_item.SetVisible(true);
                save_item.SetVisible(false);
                cancel_item.SetVisible(false);
                invalid_name.Visibility = ViewStates.Invisible;
            }
        }
        protected override void OnStop()
        {
            base.OnStop();
        }
        void ShowWarning()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetPositiveButton(Resource.String.positive_button, (sender, args) =>
            {
                ContentResolver.Delete(uri, null, null);
                Items.ListOfItems.Notes.RemoveAt(Items.ListOfItems.Notes.IndexOf(note));
                change_intent = new Intent();
                change_intent.PutExtra(MainActivity.ACTION_TYPE, MainActivity.ACTION_DELETE);
                change_intent.PutExtra(Items.POSITION_KEY, Items.ListOfItems.Notes.IndexOf(note));
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