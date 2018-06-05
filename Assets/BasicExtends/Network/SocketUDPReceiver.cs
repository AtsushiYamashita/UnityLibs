namespace BasicExtends {
    using System.Net;
    using System.Net.Sockets;
    using UnityEngine;

    public class SocketUDPReceiver: MonoBehaviour {
        [SerializeField]
        private int mReceivePort = 0;
        private Socket mSocket = null;


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

        public void Setup () {
            mSocket = new Socket(AddressFamily.InterNetwork, 
                SocketType.Dgram, ProtocolType.Udp);
            var recv_ip = NetowrkUtil.GetOwnIP();
            Debug.Log("Observe IP = " + recv_ip);
            Debug.Log("Observe Port = " + mReceivePort);
            mSocket.Bind(new IPEndPoint(IPAddress.Any, mReceivePort));
            var sender = new IPEndPoint(IPAddress.Any, 0) as EndPoint;

            ThreadManager.Get().Work("SocketUDPReceiver Setup", null, ( e ) =>
            {
                if (mSocket == null) {
                    System.Threading.Thread.Sleep(10);
                    return ThreadState.Continue;
                }
                Msg.Gen().Set(Msg.TO, "Debug").Set(Msg.ACT, "log")
                .Set(Msg.MSG, "Recv").Pool();

                byte [] buffer = new byte [2500];
                int size = mSocket
                    .ReceiveFrom(buffer, 0, 2499, SocketFlags.None,ref sender);
                if (size < 1) {
                    return ThreadState.Continue;
                }

                Serializer.SetDatatype(Serializer.SerialType.Binary);
                CheckedRet<Msg> ret = null;
                try {
                    var buf = ByteList.Zero.Add(buffer);
                    var count = buffer.Length - size;
                    buf.RemoveRangeBase(size, count);
                    ret = Serializer.Deserialize<Msg>(buf);

                } catch  {
                    Msg.Gen().Set(Msg.TO, "Debug").Set(Msg.ACT, "log")
                    .Set(Msg.MSG, "get FAIL").Pool();
                }
                Debug.Log("ret.Key" + ret.Key);
                if (ret.Key == false) { return ThreadState.Continue; }
                ret.Value.Set("From", sender.AddressFamily.ToString()).Pool();
                return ThreadState.Continue;
            });
        }
    }



}