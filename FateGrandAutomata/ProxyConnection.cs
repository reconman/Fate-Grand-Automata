using Android.Content;
using Android.OS;

namespace FateGrandAutomata
{
    public class ProxyConnection : Java.Lang.Object, IServiceConnection
    {
        public ProxyConnection(MainActivity Activity)
        {
            Receiver = new Messenger(new ActivityIncomingHandler(Activity));
        }

        public bool IsBound { get; private set; }

        public Messenger Messenger { get; private set; }

        public Messenger Receiver { get; }

        public void OnServiceConnected(ComponentName Name, IBinder Service)
        {
            Messenger = new Messenger(Service);
            IsBound = true;

            try
            {
                var msg = Message.Obtain(null, ProxyService.MsgRegisterClient);
                msg.ReplyTo = Receiver;
                Messenger.Send(msg);
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
            Messenger = null;
            IsBound = false;
        }
    }
}