namespace BasicExtends {
    using UnityEngine;
    using System.Net;
    using System.Net.Sockets;


    public class SocketTCPReceiver: MonoBehaviour {
        [SerializeField]
        private int mReceivePort = 0;
        private Socket mConnectSocket = null;


        private void Start () {
            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", "SocketTCPReceiver")) { return; }
                if (msg.Match("act", "Setup")) {
                    Setup();
                    return;
                }
            });
        }

        private ThreadState MakeConnection (object obj) {
            var listen = (Socket) obj;
            var isConnect = listen.Poll(0, SelectMode.SelectRead);
            if (isConnect == false) {
                System.Threading.Thread.Sleep(5);
                return ThreadState.Continue;
            }
            mConnectSocket = listen.Accept();
            return ThreadState.End;
        }

        private ThreadState Receive(object obj ) {
            if (mConnectSocket == null) {
                System.Threading.Thread.Sleep(10);
                return ThreadState.Continue;
            }
            byte [] buffer = new byte [2500];
            int size = mConnectSocket
                .Receive(buffer, buffer.Length, SocketFlags.None);
            if (size < 1) { return ThreadState.Continue; }

            Serializer.SetDatatype(Serializer.SerialType.Binary);
            var ret = Serializer.Deserialize(ByteList.Gen().Add(buffer));
            if (ret.Key == false) { return ThreadState.Continue; }
            return ThreadState.Continue;
        }

        private void Setup () {
            var listen = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            listen.Bind(new IPEndPoint(IPAddress.Any, mReceivePort));
            listen.Listen(1);

            ThreadManager.Get().Work("SocketTCPReceiver Setup1", listen, MakeConnection );
            ThreadManager.Get().Work("SocketTCPReceiver Setup2", null, Receive);
        }
    }

}