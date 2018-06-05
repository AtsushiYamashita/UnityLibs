namespace BasicExtends {

    using System.Collections;
    using System.Net.Sockets;
    using System.Net;
    using UnityEngine;

    public class SocketUDPSender: MonoBehaviour {

        [SerializeField]
        private int mUsePort = 8000;

        [SerializeField]
        private int mToPort = 8001;

        [SerializeField]
        private string mToIP = "127.0.0.1";

        private ThreadsafeCounter mSendId = new ThreadsafeCounter();
        bool mIsSetuped = false;
        private SafeAccessList<byte []> mMsgList = new SafeAccessList<byte []>();

        private void Start () {
            MessengerSetup();
        }

        public void MessengerSetup () {
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

        public void Setup () {
            var own_ip = NetowrkUtil.GetOwnIP();
            var socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
            Debug.Log("TCP Setup");
            socket.Bind(new IPEndPoint(IPAddress.Parse(own_ip), mUsePort));
            var to = new IPEndPoint(IPAddress.Parse(mToIP), mToPort);
            mIsSetuped = true;

            ThreadManager.Get().Work("SocketTCPSender Setup", null, ( obj ) =>
            {
                byte [] buffer = mMsgList.Pop();
                if (buffer == null || buffer.Length < 1) {
                    System.Threading.Thread.Sleep(8);
                    return ThreadState.Continue;
                }

                socket.SendTo(buffer, buffer.Length,SocketFlags.None, to);
                Msg.Gen().Set(Msg.TO, "Debug").Set(Msg.ACT, "log")
                .Set(Msg.MSG, "sended success").Pool();
                return ThreadState.Continue;
            });
        }

        public void SendStack ( Msg message ) {
            message = message.Set("Id", "" + mSendId.Get());
            mSendId.Increment();
            if (mIsSetuped == false) { return; }
            Debug.Log("test-2");

            Serializer.SetDatatype(Serializer.SerialType.Binary);
            mMsgList.Add(Serializer.Serialize(message).ToArray());
        }
    }
}
