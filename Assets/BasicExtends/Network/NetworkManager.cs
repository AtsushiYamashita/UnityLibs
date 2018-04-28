using UnityEngine;
using BasicExtends;

public class NetworkManager: MonoBehaviour {

    private void Start () {
        MessengerSetup();
        Serializer.SetDatatype(Serializer.SerialType.Binary2);
        Msg.Gen().To(gameObject.name)
            .As("UdpReceiver")
            .Act("Start")
            .Pool();
        Msg.Gen().To(gameObject.name)
            .As("UdpSender")
            .Act("Setup")
            .Pool();
    }

    private void MessengerSetup () {
        Messenger.Assign(( Msg msg ) =>
        {
            if (msg.Unmatch("to", "Manager")) { return; }
            if (msg.Unmatch("as", "NetworkManager")) { return; }
            if (msg.Match("result", "Fail") ) {
                FailEvent(msg);
                return;
            }
            if (msg.Match("result", "Success") ) {
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
