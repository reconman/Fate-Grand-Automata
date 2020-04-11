using System;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace FateGrandAutomata
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle SavedInstanceState)
        {
            base.OnCreate(SavedInstanceState);
            Xamarin.Essentials.Platform.Init(this, SavedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var serviceToggleBtn = FindViewById<Button>(Resource.Id.service_toggle_btn);
            serviceToggleBtn.Click += ServiceToggleBtnOnClick;

            var settingsBtn = FindViewById<Button>(Resource.Id.configure_btn);
            settingsBtn.Click += (S, E) => OpenSettings();

            CheckPermissions();
            IgnoreBatteryOptimizations();
            ShowStatusText();

            _connection = new ProxyConnection(this);

            BindService(new Intent(this, typeof(ProxyService)),
                _connection,
                Bind.AutoCreate);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_connection.IsBound)
            {
                try
                {
                    _connection.SendMessage(ProxyService.MsgUnregisterClient);
                }
                catch (RemoteException)
                {
                    // There is nothing special we need to do if the service has crashed.
                }

                UnbindService(_connection);
            }
        }

        ProxyConnection _connection;

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            // Android P added support for display cutouts
            if (Build.VERSION.SdkInt < BuildVersionCodes.P)
                return;

            _connection.SendMessage(ProxyService.MsgCutout, Window.DecorView.RootWindowInsets.DisplayCutout);
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            ShowStatusText();
        }

        void ShowStatusText()
        {
            _connection?.SendMessage(ProxyService.MsgAskStatus);
        }

        void IgnoreBatteryOptimizations()
        {
            var powerManager = (PowerManager) GetSystemService(PowerService);

            if (powerManager.IsIgnoringBatteryOptimizations(PackageName))
            {
                return;
            }

            StartActivity(new Intent(Settings.ActionRequestIgnoreBatteryOptimizations,
                Android.Net.Uri.Parse("package:" + PackageName)));
        }

        protected override void OnActivityResult(int RequestCode, Result ResultCode, Intent Data)
        {
            if (RequestCode == RequestMediaProjection)
            {
                if (ResultCode != Result.Ok)
                {
                    Toast.MakeText(this, "Canceled", ToastLength.Short).Show();
                    return;
                }

                _connection.SendMessage(ProxyService.MsgStartWithMediaProjectionToken, Data);
            }
        }

        void OpenSettings()
        {
            StartActivity(typeof(SettingsActivity));
        }

        void CheckPermissions()
        {
            var permissionsToCheck = new[]
            {
                Manifest.Permission.WriteExternalStorage,
                Manifest.Permission.ReadExternalStorage
            };

            var permissionsToRequest = permissionsToCheck
                .Where(M => ContextCompat.CheckSelfPermission(this, M) != Permission.Granted)
                .ToArray();

            if (permissionsToRequest.Length > 0)
            {
                ActivityCompat.RequestPermissions(
                    this,
                    permissionsToRequest,
                    0);
            }
        }

        void ServiceToggleBtnOnClick(object Sender, EventArgs EventArgs)
        {
            _connection.SendMessage(ProxyService.MsgToggleService);
        }

        public override void OnRequestPermissionsResult(int RequestCode, string[] Permissions, [GeneratedEnum] Permission[] GrantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(RequestCode, Permissions, GrantResults);

            base.OnRequestPermissionsResult(RequestCode, Permissions, GrantResults);
        }

        public const int RequestMediaProjection = 1;
    }
}

