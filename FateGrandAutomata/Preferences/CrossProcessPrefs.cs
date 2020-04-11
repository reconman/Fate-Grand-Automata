using Android.Content;
using Android.Database;
using Android.Net;

namespace FateGrandAutomata
{
    public class CrossProcessPrefs
    {
        readonly ContentResolver _resolver;
        readonly string _prefName;

        public CrossProcessPrefs(string PrefName, ContentResolver Resolver)
        {
            _prefName = PrefName;
            _resolver = Resolver;
        }

        public CrossProcessPrefs(ContentResolver Resolver)
            : this(SharedPreferencesContentProvider.DefaultPrefName, Resolver) { }

        static ICursor PerformQuery(Uri Uri, ContentResolver Resolver)
        {
            return Resolver.Query(Uri, null, null, null, null, null);
        }

        static Uri CreateQuery(string PrefName, string Key, int PrefType)
        {
            var baseUrl = PrefType switch
            {
                SharedPreferencesContentProvider.CodeString => SharedPreferencesContentProvider.BasePathString,
                SharedPreferencesContentProvider.CodeInt => SharedPreferencesContentProvider.BasePathInt,
                SharedPreferencesContentProvider.CodeBool => SharedPreferencesContentProvider.BasePathBool,
                _ => ""
            };

            return Uri.Parse($"content://{SharedPreferencesContentProvider.Authority}/{baseUrl}/{PrefName}/{Key}");
        }

        static int ExtractInt(ICursor Cursor, int Default = 0)
        {
            if (Cursor != null)
            {
                if (Cursor.MoveToFirst())
                {
                    return Cursor.GetInt(Cursor.GetColumnIndex(SharedPreferencesContentProvider.ValueColumn));
                }

                Cursor.Close();
            }

            return Default;
        }

        static string ExtractString(ICursor Cursor, string Default = "")
        {
            if (Cursor != null)
            {
                if (Cursor.MoveToFirst())
                {
                    return Cursor.GetString(Cursor.GetColumnIndex(SharedPreferencesContentProvider.ValueColumn));
                }

                Cursor.Close();
            }

            return Default;
        }

        static bool ExtractBool(ICursor Cursor, bool Default = false)
        {
            if (Cursor != null)
            {
                if (Cursor.MoveToFirst())
                {
                    return Cursor.GetInt(Cursor.GetColumnIndex(SharedPreferencesContentProvider.ValueColumn)) == 1;
                }

                Cursor.Close();
            }

            return Default;
        }

        public int GetInt(string Key, int Default = 0)
        {
            var uri = CreateQuery(_prefName, Key, SharedPreferencesContentProvider.CodeInt);
            var cursor = PerformQuery(uri, _resolver);
            return ExtractInt(cursor, Default);
        }

        public string GetString(string Key, string Default = "")
        {
            var uri = CreateQuery(_prefName, Key, SharedPreferencesContentProvider.CodeString);
            var cursor = PerformQuery(uri, _resolver);
            return ExtractString(cursor, Default);
        }

        public bool GetBool(string Key, bool Default = false)
        {
            var uri = CreateQuery(_prefName, Key, SharedPreferencesContentProvider.CodeBool);
            var cursor = PerformQuery(uri, _resolver);
            return ExtractBool(cursor, Default);
        }
    }
}