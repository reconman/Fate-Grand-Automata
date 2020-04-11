using Android.OS;

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
            base.HandleMessage(Msg);
        }
    }
}