using Android.Content;
using Android.Database;
using Android.Net;
using AndroidX.Preference;
using Java.Lang;

namespace FateGrandAutomata
{
    [ContentProvider(new [] { Authority }, Exported = false)]
    public class SharedPreferencesContentProvider : ContentProvider
    {
        public const string Authority = "com.mathewsachin.fategrandautomata." + nameof(SharedPreferencesContentProvider);

        public const string ValueColumn = nameof(ValueColumn);

        static readonly UriMatcher UriMatcher = BuildUriMatcher();

        static UriMatcher BuildUriMatcher()
        {
            var matcher = new UriMatcher(UriMatcher.NoMatch);

            matcher.AddURI(Authority, $"{BasePathString}/*/*", CodeString);
            matcher.AddURI(Authority, $"{BasePathInt}/*/*", CodeInt);
            matcher.AddURI(Authority, $"{BasePathBool}/*/*", CodeBool);

            return matcher;
        }

        public const string BasePathString = "str",
            BasePathInt = "int",
            BasePathBool = "bool";

        public const int CodeString = 1,
            CodeInt = 2,
            CodeBool = 3;

        public const string PreferenceMimeType = ContentResolver.CursorItemBaseType + "/vnd.com.xamarin.sample.Pref";

        public const string DefaultPrefName = "__DEFAULT__";

        ISharedPreferences GetSharedPreferences(string Id)
        {
            if (Id != DefaultPrefName)
            {
                return Context.GetSharedPreferences(Id, FileCreationMode.Private);
            }

            var prefs = PreferenceManager.GetDefaultSharedPreferences(Context);

            if (!prefs.GetBoolean(PreferenceManager.KeyHasSetDefaultValues, false))
            {
                PreferenceManager.SetDefaultValues(Context, Resource.Xml.app_preferences, true);
                PreferenceManager.SetDefaultValues(Context, Resource.Xml.refill_preferences, true);
            }

            return prefs;
        }

        public override ICursor Query(Uri Uri, string[] Projection, string Selection, string[] SelectionArgs, string SortOrder)
        {
            var prefs = GetSharedPreferences(Uri.PathSegments[1]);
            var key = Uri.PathSegments[2];
            var has = prefs.Contains(key);

            switch (UriMatcher.Match(Uri))
            {
                case CodeString:
                    return has ? AsCursor(prefs.GetString(key, "")) : null;

                case CodeBool:
                    return has ? AsCursor(prefs.GetBoolean(key, false) ? 1 : 0) : null;

                case CodeInt:
                    return has ? AsCursor(prefs.GetInt(key, 0)) : null;
            }

            return null;
        }

        static ICursor AsCursor(Object Item)
        {
            var cursor = new MatrixCursor(new[] { ValueColumn }, 1);
            var row = cursor.NewRow();
            
            row.Add(Item);

            return cursor;
        }

        public override int Delete(Uri Uri, string Selection, string[] SelectionArgs)
        {
            throw new System.NotImplementedException();
        }

        public override string GetType(Uri Uri)
        {
            switch (UriMatcher.Match(Uri))
            {
                case CodeBool:
                case CodeInt:
                case CodeString:
                    return PreferenceMimeType;

                default:
                    return null;
            }
        }

        public override Uri Insert(Uri Uri, ContentValues Values)
        {
            throw new System.NotImplementedException();
        }

        public override bool OnCreate() => true;

        public override int Update(Uri Uri, ContentValues Values, string Selection, string[] SelectionArgs)
        {
            throw new System.NotImplementedException();
        }
    }
}