using UnityEngine;
using BasicExtends;

using System.Net;
using System.Net.Sockets;
using System.Text;

public class NetworkManager: MonoBehaviour {

    public IReceiver mReceiver;
    public ISender mSender;

    private void Start () {
        mReceiver = UdpReceiver.Instance;
        mSender = UdpSender.Instance;

        MessengerSetup();
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
                //Debug.Log(msg.Stringify()); // for debug
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

    public void SenderSetup (string adres_r) {
        mSender.Setup(adres_r);
    }

    private void FailEvent ( Msg msg ) {
        Debug.Log(msg.Stringify());
    }

    public void ConnectionClose () {
        mReceiver.Close();
        mSender.Close();
    }
}
