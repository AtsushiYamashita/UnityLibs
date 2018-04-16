using System.Collections;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BasicExtends {
    public interface IReceiveThread {
        bool LaunchThread ( ServerData data );
        void ThreadStop ();
    }

    public interface IServer {
        bool StartServer ( int port, int connectionSize );
        void StopServer ();
        void Disconnect ();
    }


    public class ServerData {
        // クライアントとの接続用ソケット.
        public Socket mSocket = null;

        // サーバフラグ.	
        public bool IsServer { get; set; }

        // 接続確認.
        public bool IsConnected { set; get; }

        //バッファサイズ
        public static int staticMemorySize = 2048;

        // 受信バッファ.
        public PacketQueue mRecvQueue;

        public ServerData () {
            mRecvQueue = new PacketQueue();
        }
    }

    public class ReceiveThread: Singleton<ReceiveThread>, IReceiveThread {

        // スレッド実行フラグ.
        protected bool mThreadLoop = false;
        protected Thread mThread = null;
        protected ServerData mData = null;

        //seal
        private ReceiveThread () { }

        // スレッド起動関数.
        public bool LaunchThread ( ServerData data ) {
            mData = data;
            try {
                // Dispatch用のスレッド起動.
                mThreadLoop = true;
                mThread = new Thread(new ThreadStart(ReceiveLoop));
                mThread.Start();
            } catch {
                Debug.Log("Cannot launch thread.");
                Msg.Gen().To("Manager").As("NetworkManager")
                    .Set("type", "LaunchThread")
                    .Set("result", "Fail").Push();
                return false;
            }
            return true;
        }

        public void ReceiveLoop () {
            while (mThreadLoop) {
                var socket_living = mData.mSocket != null;
                var connecting = mData.IsConnected == true;
                if (socket_living && connecting) { ReceiveDispatch(); }
                Thread.Sleep(5);
            }
        }

        private void ReceiveDispatch () {
            try {
                while (true) {
                    var connect = mData.mSocket.Poll(0, SelectMode.SelectRead);
                    if (connect == false) { break; }
                    if (Receive() == true) { continue; }
                    Server.Instance.Disconnect();
                }
            } catch {
                return;
            }
        }

        private bool Receive () {
            byte [] buffer = new byte [ServerData.staticMemorySize];
            int recvSize = mData.mSocket
                .Receive(buffer, buffer.Length, SocketFlags.None);
            if (recvSize == 0) { return false; }
            if (recvSize < 0) {
                throw new System.Exception("recvSize:" + recvSize);
            }
            mData.mRecvQueue.Enqueue(buffer, recvSize);
            return true;
        }

        public void ThreadStop () {
            mThreadLoop = false;
            if (mThread == null) { return; }
            mThread.Join();
            mThread = null;
        }
    }


    public class Server: Singleton<Server>, IServer {
        ServerData mData;

        private Server () {
            mData = new ServerData();
        }

        /// <summary>
        /// サーバーを起動させたいときに呼ぶ
        /// </summary>
        /// <param name="port"></param>
        /// <param name="connectionSize"></param>
        /// <returns></returns>
        public bool StartServer ( int port, int connectionSize ) {
            // リスニングソケットを生成します.
            try {
                // ソケットを生成します.
                mData.mSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                // 使用するポート番号を割り当てます.
                mData.mSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            } catch {
                Debug.Log("StartServer fail");
                Msg.Gen().To("Manager").As("NetworkManager")
                    .Set("type", "StartServer")
                    .Set("result", "Fail").Push();
                return false;
            }
            mData.IsConnected = true;
            return ReceiveThread.Instance.LaunchThread(mData);
        }

        public void StopServer () {
            ReceiveThread.Instance.ThreadStop();
            Disconnect();
            mData.IsServer = false;
        }

        public void Disconnect () {
            mData.IsConnected = false;

            if (mData.mSocket == null) { return; }
            mData.mSocket.Shutdown(SocketShutdown.Both);
            mData.mSocket.Close();
            mData.mSocket = null;

            Debug.Log("Disconnect recv from client.");
            Msg.Gen().To("Manager").As("NetworkManager")
                .Set("type", "Disconnect")
                .Set("result", "Success").Push();
        }
    }
}