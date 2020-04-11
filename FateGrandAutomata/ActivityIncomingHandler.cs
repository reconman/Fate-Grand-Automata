using Android.Content;
using Android.Media.Projection;
using Android.OS;
using Android.Provider;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace FateGrandAutomata
{
    public class ActivityIncomingHandler : Handler
    {
        readonly MainActivity _activity;
        
        public ActivityIncomingHandler(MainActivity Activity)
        {
            _activity = Activity;
        }

        public override void HandleMessage(Message Msg)
        {
            switch (Msg.What)
            {
                case ProxyService.MsgReceiveStatus:
                    var statusTextView = _activity.FindViewById<TextView>(Resource.Id.status_textview);

                    var statusText = Msg.Obj.ToString();

                    statusTextView.Text = statusText;
                    break;

                case ProxyService.MsgNoMediaProjectionToken:
                    // This initiates a prompt dialog for the user to confirm screen projection.
                    var mediaProjectionManager = (MediaProjectionManager) _activity.GetSystemService(Context.MediaProjectionService);

                    _activity.StartActivityForResult(mediaProjectionManager.CreateScreenCaptureIntent(), MainActivity.RequestMediaProjection);
                    break;

                case ProxyService.MsgAccessibilityServiceNotRunning:
                    new AlertDialog.Builder(_activity)
                        .SetTitle("Accessibility Disabled")
                        .SetMessage("Turn on accessibility for this app from System settings. If it is already On, turn it OFF and start again.")
                        .SetPositiveButton("Go To Settings", (S, E) =>
                        {
                            // Open Acessibility Settings
                            var intent = new Intent(Settings.ActionAccessibilitySettings);
                            _activity.StartActivity(intent);
                        })
                        .SetNegativeButton("Cancel", (S, E) => { })
                        .Show();
                    break;

                default:
                    base.HandleMessage(Msg);
                    break;
            }
        }
    }
}