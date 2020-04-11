using Android.OS;
using Android.Views;
using Java.Lang;

namespace FateGrandAutomata
{
    public class ServiceIncomingHandler : Handler
    {
        Messenger _client;

        void SendResponse(int What, Object Obj = null)
        {
            var msg = Obj != null
                ? Message.Obtain(null, What, Obj)
                : Message.Obtain(null, What);

            _client?.Send(msg);
        }

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

                case ProxyService.MsgCutout:
                    CutoutManager.ApplyCutout(Msg.Obj as DisplayCutout);
                    break;

                case ProxyService.MsgAskStatus:
                    SendResponse(ProxyService.MsgReceiveStatus, GetStatusText());
                    break;

                case ProxyService.MsgToggleService:
                    var instance = ScriptRunnerService.Instance;

                    if (instance.ServiceStarted)
                    {
                        instance.Stop();
                    }
                    else
                    {
                        if (ScriptRunnerService.Instance.HasMediaProjectionToken)
                        {
                            instance.Start();
                        }
                        else SendResponse(ProxyService.MsgNoMediaProjectionToken);
                    }
                    break;

                default:
                    base.HandleMessage(Msg);
                    break;
            }
        }

        static string GetStatusText()
        {
            var autoskillOn = Preferences.Instance.EnableAutoSkill;
            var autoskillCmd = autoskillOn
                ? $" - {Preferences.Instance.SkillCommand}"
                : "";

            var refillPrefs = Preferences.Instance.Refill;

            var autoRefillOn = refillPrefs.Enabled;
            var autoRefillStatus = autoRefillOn
                ? $" - {refillPrefs.Enabled} x{refillPrefs.Repetitions}"
                : "";

            var supportPrefs = Preferences.Instance.Support;
            var preferredMode = supportPrefs.SelectionMode == SupportSelectionMode.Preferred;

            static string Any(string Value) => string.IsNullOrWhiteSpace(Value)
                ? "Any"
                : Value;

            var supportStatus = preferredMode
                ? $"Servants: '{Any(supportPrefs.PreferredServants)}', CEs: '{Any(supportPrefs.PreferredCEs)}'"
                : "";

            static string OnOff(bool Value) => Value ? "ON" : "OFF";

            var statusText = $"Mode: {Preferences.Instance.ScriptMode}";

            if (Preferences.Instance.ScriptMode == ScriptMode.Battle)
            {
                statusText += $@"
Server: {Preferences.Instance.GameServer}
Auto Skill: {OnOff(autoskillOn)}{autoskillCmd}
Auto Refill: {OnOff(autoRefillOn)}{autoRefillStatus}
Auto Support Selection: {supportPrefs.SelectionMode}
{supportStatus}
";
            }

            return statusText;
        }
    }
}