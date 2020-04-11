using Android.Content;
using Android.Media.Projection;
using Android.OS;
using Android.Widget;

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

                default:
                    base.HandleMessage(Msg);
                    break;
            }
        }
    }
}