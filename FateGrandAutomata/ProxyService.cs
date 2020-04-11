using Android.App;
using Android.Content;
using Android.OS;

namespace FateGrandAutomata
{
    public class ProxyService : Service
    {
        Messenger _messenger;

        public const int MsgRegisterClient = 1;
        public const int MsgUnregisterClient = 2;
        public const int MsgCutout = 3;
        public const int MsgAskStatus = 4;
        public const int MsgReceiveStatus = 5;
        public const int MsgToggleService = 6;
        public const int MsgNoMediaProjectionToken = 7;
        public const int MsgAccessibilityServiceNotRunning = 8;

        public override IBinder OnBind(Intent Intent)
        {
            _messenger = new Messenger(new ServiceIncomingHandler());

            return _messenger.Binder;
        }
    }
}