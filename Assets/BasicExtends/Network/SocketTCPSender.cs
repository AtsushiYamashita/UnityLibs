namespace BasicExtends {
    using System.Net;
    using System.Net.Sockets;
    using UnityEngine;

    public class SocketTCPSender : MonoBehaviour {

        [SerializeField]
        private int mSendToPort = 0;

        [SerializeField]
        private string mSendToIp = "";

        private ThreadsafeCounter mSendId = new ThreadsafeCounter();
        bool mIsSetuped = false;
        private SafeAccessList<byte []> mMsgList = new SafeAccessList<byte []>();

        private void Start () {
            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Match("Network", "true")) {
                    msg.Set("Network", "false");
                    SendStack(msg);
                    return;
                }
                if (msg.Unmatch(Msg.TO, gameObject.name)) { return; }
                if (msg.Unmatch(Msg.AS, "SocketTCPSender")) { return; }
                if (msg.Match(Msg.ACT, "Setup")) {
                    Setup();
                    return;
                }
            });
        }

        private void Setup () {
            //var recv_ip = NetowrkUtil.GetOwnIP();
            var socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.SendBufferSize = 0;
            Debug.Log("TCP Setup");

            ThreadManager.Get().Work("SocketTCPSender Setup1", null, ( obj ) =>
            {
                try {
                    socket.Connect(mSendToIp, mSendToPort);
                } catch {
                    return ThreadState.Continue;
                }
                mIsSetuped = true;
                Debug.Log("TCP ConnectSuccess");
                return ThreadState.End;
            });

            ThreadManager.Get().Work("SocketTCPSender Setup2", null, ( obj ) =>
            {
                byte [] buffer = mMsgList.Pop();
                if (buffer == null || buffer.Length < 1) {
                    System.Threading.Thread.Sleep(NetworkUnit.INTERVAL);
                    return ThreadState.Continue ;
                }
                socket.Send(buffer, buffer.Length,SocketFlags.None);
                return ThreadState.Continue;
            });
        }

        private void SendStack ( Msg message ) {
            message = message.Set("Id", "" + mSendId.Get());
            mSendId.Increment();
            if (mIsSetuped == false) { return; }

            Serializer.SetDatatype(Serializer.SerialType.Binary);
            mMsgList.Add(Serializer.Serialize(message).ToArray());
        }
    }
}