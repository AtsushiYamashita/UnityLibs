namespace BasicExtends {
    using UnityEngine;
    using System.Collections.Generic;

    public class NetworkManager: MonoBehaviour {

        private string mOwnIp = string.Empty;

        private void Start () {
            MessengerSetup();
            Serializer.SetDatatype(Serializer.SerialType.Binary2);
            Msg.Gen().Set(Msg.TO, gameObject.name)
                .Set(Msg.AS, "UdpReceiver")
                .Set(Msg.ACT, "Start")
                .Pool();
            Msg.Gen().Set(Msg.TO, gameObject.name)
                .Set(Msg.AS, "UdpSender")
                .Set(Msg.ACT, "Setup")
                .Pool();
            mOwnIp = NetowrkUtil.GetOwnIP();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Unmatch(Msg.TO, "Manager")) { return; }
                if (msg.Unmatch(Msg.AS, "NetworkManager")) { return; }
                if (msg.Match("result", "Fail")) {
                    FailEvent(msg);
                    return;
                }
                if (msg.Match("result", "Success")) {
                    Debug.Log(msg.Stringify()); // for debug
                    return;
                }
                if (msg.ContainsKey("From")) {
                    Debug.Log(msg.Stringify());
                    return;
                }
            });
        }

        private void Update () {
            Messenger.Flash();
        }

        private void FailEvent ( Msg msg ) {
            Debug.Log(msg.Stringify());
        }

    }

}