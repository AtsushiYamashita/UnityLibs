namespace BasicExtends {
    using UnityEngine;
    using System;
    using System.Net;
    using System.Net.Sockets;

    [Serializable]
    public class UdpReceiver: MonoBehaviour {
        LoopThread mLoop = null;
        UdpClient mClient;

        [SerializeField]
        private int mObservePort = NetworkUnit.DEFAULT_PORT_R;

        private void Start () {
            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", "UdpReceiver")) { return; }
                if (msg.Match("act", "Start")) {
                    StartServer();
                    return;
                }
            });
        }

        /// <summary>
        /// サーバーを起動させたいときに呼ぶ
        /// </summary>
        /// <param name="port"></param>
        /// <param name="connectable"></param>
        /// <returns></returns>
        public bool StartServer () {
            var notReach = NetworkReachability.NotReachable;
            if (Application.internetReachability == notReach) {
                var msg = "application can not connect.";
                throw new NotSupportedException(msg);
            }
            if (mLoop.IsNotNull()) { return true; }
            try {
                mLoop = new LoopThread();
                Debug.Log(mObservePort);
                mClient = new UdpClient(new IPEndPoint(IPAddress.Parse(
                    NetworkUnit.GetLocalIPAddress()), mObservePort));
            } catch (Exception e) {
                Msg.Gen().To("Manager").As("NetworkManager")
                    .Set("type", "StartServer")
                    .Set("result", "Fail")
                    .Set("msg", e.ToString())
                    .Push();
                return false;
            }

            Msg.Gen().To("Manager")
                .As("NetworkManager")
                .Set("type", "StartServer")
                .Set("result", "Success").Push();
            mLoop
                .AddContinueableCheck(() => { return mClient != null; })
                .LaunchThread(ReceiveLoop);
            return true;
        }

        public void StopServer () {
            mLoop.ThreadStop();
        }

        /// <summary>
        /// 受信と待機処理
        /// スレッド側で実行させる
        /// </summary>
        public void ReceiveLoop () {
            IPEndPoint sender = null;
            var buffer = mClient.Receive(ref sender);
            // Receive イベント を実行
            OnRecieve(buffer, sender);
        }

        private static bool OnRecieve ( byte [] receved, IPEndPoint sender ) {
            if (receved.Length < 1) { return false; }
            var msg = Serializer.Deserialize(ByteList.Zero.Add(receved)).Value as Msg;
            if (msg == null) { throw new UnDeserializableException(); }
            msg.Set("From", "" + sender.Address.MapToIPv4()).Pool();
            Debug.Log(msg.ToJson());
            return true;
        }

        public void Close () {
            if (mClient.IsNotNull()) { mClient.Close(); }
            if (mLoop.IsNotNull()) { mLoop.ThreadStop(); }
        }

        private static void CloseCall ( UnityEditor.PlayModeStateChange state ) {
            Debug.LogFormat("PlayModeStateChange({0})", state);
            // Close();
            UnityEditor.EditorApplication.playModeStateChanged += CloseCall;
        }

        public void Reset () {
            UnityEditor.EditorApplication.playModeStateChanged += CloseCall;
        }
    }
}