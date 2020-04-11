using Android.OS;

namespace FateGrandAutomata
{
    public class ServiceIncomingHandler : Handler
    {
        Messenger _client;

        public override void HandleMessage(Message Msg)
        {
            switch (Msg.What)
            {
                case ProxyService.MsgRegisterClient:
                    _client = Msg.ReplyTo;
                    break;

                case ProxyService.MsgUnregisterClient:
                    _client = null;
                    break;

                default:
                    base.HandleMessage(Msg);
                    break;
            }
        }
    }
}