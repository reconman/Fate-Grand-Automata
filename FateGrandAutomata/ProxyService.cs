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

        public override IBinder OnBind(Intent Intent)
        {
            _messenger = new Messenger(new ServiceIncomingHandler());

            return _messenger.Binder;
        }
    }
}