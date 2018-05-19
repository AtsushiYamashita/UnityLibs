namespace BasicExtends {
    using UnityEngine;
    using System.Net;
    using System.Net.Sockets;

    public class UdpSender: MonoBehaviour {

        public UdpClient mSendClient = null;
        private LoopThread mLoop;
        private SafeAccessValue<IPEndPoint> mSendTo = new SafeAccessValue<IPEndPoint>();
        private ThreadsafeCounter mSendId = new ThreadsafeCounter();
        private SafeAccessList<byte []> mMsgList = new SafeAccessList<byte []>();

        [SerializeField]
        private int mUsePort = NetworkUnit.DEFAULT_PORT_S;
        [SerializeField]
        private int mToPort = NetworkUnit.DEFAULT_PORT_R;
        [SerializeField]
        private string mToAddress = "";

        bool mIsSetuped = false;

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
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("As", "UdpSender")) { return; }
                if (msg.Match("act", "Setup")) {
                    Setup();
                    return;
                }
                if (msg.Match("act", "SetUsePort")) {
                    var port = int.Parse(msg.TryGet("port"));
                    SetUsePort(port);
                }
                if (msg.Match("act", "SetSendPort")) {
                    var port = int.Parse(msg.TryGet("port"));
                    SetSendPort(port);
                }
            });
        }

        public void SetUsePort ( int port ) {
            mUsePort = port;
        }

        public void SetSendPort ( int port ) {
            mToPort = port;
            mSendTo.Set(new IPEndPoint(IPAddress.Parse(mToAddress), mToPort));
        }

        public void Setup () {
            mLoop = new LoopThread();
            mSendTo.Set(new IPEndPoint(IPAddress.Parse(mToAddress), mToPort));
            mSendClient = new UdpClient(new IPEndPoint(IPAddress.Any, mUsePort));
            mLoop.AddContinueableCheck(() => { return mSendClient != null; })
                .LaunchThread(SendLoop);
            Msg.Gen().Set(Msg.TO, "Manager")
                .Set(Msg.AS,"NetworkManager")
                .Set("type", "SenderSetup")
                .Set("result", "Success").Pool();
            mIsSetuped = true;
        }

        private void SendLoop () {
            byte [] buffer = mMsgList.Pop();
            if (buffer == null || buffer.Length < 1) {

                System.Threading.Thread.Sleep(NetworkUnit.INTERVAL);
                return;
            }
            mSendClient.Send(buffer, buffer.Length, mSendTo.Get());
            Msg.Gen().Set(Msg.TO,"Manager")
                .Set(Msg.AS, "NetworkManager")
                .Set("type", "Sender@SendLoop")
                .Set("result", "Success").Pool();
        }

        private void SendStack ( Msg message ) {
            message = message
                .Set("Id", "" + mSendId.Get());
            mSendId.Increment();
            //Msg.Gen().To("Manager")
            //    .As("NetworkManager")
            //    .Set("type", "Sender@Send")
            //    .Set("Msg", message.ToJson())
            //    .Set("StackCount", mMsgList.Count())
            //    .Set("result", "Success").Push();
            if (mIsSetuped == false) { return; }
            mMsgList.Add(Serializer.Serialize(message).ToArray());
        }

        public void Close () {
            mSendClient.Close();
            mLoop.ThreadStop();
        }

        public void Reset () {
            UnityEditor.EditorApplication.playModeStateChanged
                += delegate ( UnityEditor.PlayModeStateChange state ) {
                    Close();
                };
        }
    }
}
