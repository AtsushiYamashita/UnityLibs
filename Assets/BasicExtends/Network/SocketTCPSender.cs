namespace BasicExtends {
    using System.Net;
    using System.Net.Sockets;
    using UnityEngine;

    public class SocketTCPSender : MonoBehaviour {

        [SerializeField]
        private int mSendToPort = 0;

        [SerializeField]
        private string mSendToIp = "";

        private Socket mSocket = null;
        private ThreadsafeCounter mSendId = new ThreadsafeCounter();
        bool mIsSetuped = false;
        private SafeAccessList<byte []> mMsgList = new SafeAccessList<byte []>();

        private static string GetOwnIP () {
            try {
                //ホスト名を取得
                string hostname = Dns.GetHostName();

                //ホスト名からIPアドレスを取得
                IPAddress [] addr_arr = Dns.GetHostAddresses(hostname);

                foreach (IPAddress addr in addr_arr) {
                    string addr_str = addr.ToString();
                    bool isIPv4 = addr_str.IndexOf(".") > 0;
                    bool notLocalhost = addr_str.StartsWith("127.") == false;
                    if (isIPv4 && notLocalhost) { return addr_str; }
                }
            } catch (System.Exception e) {
                Msg.Gen().Set(Msg.TO, "Debug")
                    .Set(Msg.ACT, "log")
                    .Set(Msg.MSG, e.ToString()).Pool();
            }
            return "";
        }

        private void Setup () {
            var recv_ip = GetOwnIP();
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.SendBufferSize = 0;
            ThreadManager.Get().Work(null, ( obj ) =>
            {
                IPAddress recv = IPAddress.Any;
                try {
                    socket.Connect(mSendToIp, mSendToPort);
                } catch {
                    return ThreadState.Continue;
                }
                mIsSetuped = true;
                return ThreadState.End;
            });

            ThreadManager.Get().Work(null, ( obj ) =>
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
            message = message
                .Set("Id", "" + mSendId.Get());
            mSendId.Increment();
            if (mIsSetuped == false) { return; }
            mMsgList.Add(Serializer.Serialize(message).ToArray());
        }

    }


}