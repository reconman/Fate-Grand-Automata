using Android.Content;
using Android.OS;
using Java.Lang;

namespace FateGrandAutomata
{
    public class ProxyConnection : Java.Lang.Object, IServiceConnection
    {
        public ProxyConnection(MainActivity Activity)
        {
            _receiver = new Messenger(new ActivityIncomingHandler(Activity));
        }

        public bool IsBound { get; private set; }

        Messenger _messenger;

        readonly Messenger _receiver;

        public void OnServiceConnected(ComponentName Name, IBinder Service)
        {
            _messenger = new Messenger(Service);
            IsBound = true;

            try
            {
                var msg = Message.Obtain(null, ProxyService.MsgRegisterClient);
                msg.ReplyTo = _receiver;
                _messenger.Send(msg);
            }
            catch (RemoteException)
            {
                // In this case the service has crashed before we could even
                // do anything with it; we can count on soon being
                // disconnected (and then reconnected if it can be restarted)
                // so there is no need to do anything here.
            }
        }

        public void OnServiceDisconnected(ComponentName Name)
        {
            _messenger = null;
            IsBound = false;
        }

        public void SendMessage(int What, Object Obj = null)
        {
            if (!IsBound)
                return;

            if (_messenger == null)
                return;

            var msg = Obj != null
                ? Message.Obtain(null, What, Obj)
                : Message.Obtain(null, What);
            msg.ReplyTo = _receiver;
            _messenger.Send(msg);
        }
    }
}