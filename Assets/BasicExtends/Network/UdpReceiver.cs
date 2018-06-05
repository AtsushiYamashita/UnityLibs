namespace BasicExtends {
    using UnityEngine;
    using System;
    using System.Net;
    using System.Net.Sockets;

    [Serializable]
    public class UdpReceiver: MonoBehaviour {
        LifedThread mLoop = null;
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
            mLoop = ThreadManager.Get();
            Debug.Log("StartServer:mObservePort" + mObservePort);
            try {
                mClient = new UdpClient(new IPEndPoint(IPAddress.Parse(
                    NetworkUnit.GetLocalIPAddress()), mObservePort));
            } catch (Exception e) {
                Msg.Gen().Set(Msg.TO, "Manager").Set(Msg.AS, "NetworkManager")
                    .Set("type", "StartServer")
                    .Set("result", "Fail")
                    .Set("msg", e.ToString())
                    .Push();
                return false;
            }

            Msg.Gen().Set(Msg.TO, "Manager")
                .Set(Msg.AS, "NetworkManager")
                .Set("type", "StartServer")
                .Set("result", "Success").Push();
            mLoop.Work("Udp client receiver", null, ReceiveLoop);
            return true;
        }

        /// <summary>
        /// 受信と待機処理
        /// スレッド側で実行させる
        /// </summary>
        public ThreadState ReceiveLoop ( object obj ) {
            IPEndPoint sender = null;
            mClient.Client.ReceiveTimeout = 3000;
            try {
                Msg.Gen().Set(Msg.TO, "Debug").Set(Msg.ACT, "log").Set(Msg.MSG, "R START").Pool();
                var buffer = mClient.Receive(ref sender);
                Msg.Gen().Set(Msg.TO, "Debug").Set(Msg.ACT, "log").Set(Msg.MSG, "R OK").Pool();
                OnRecieve(buffer, sender);
                return ThreadState.Continue;
            } catch {
                return ThreadState.Continue;
            }
        }

        private static bool OnRecieve ( byte [] receved, IPEndPoint sender ) {
            var test = Msg.Gen().Set(Msg.TO, "Debug")
                .Set(Msg.ACT, "log").Set("what", "get 798789789--------");
            test.Pool();
            if (receved.Length < 1) { return false; }
            CheckedRet<Msg> obj = null;
            try {
                Serializer.SetDatatype(Serializer.SerialType.Binary);
                obj = Serializer.Deserialize<Msg>(ByteList.Zero.Add(receved));
                if (obj.Key == false) {
                    throw new UnDeserializableException();
                }
            } catch (Exception e) {
                DebugLog.Log.Print(e.ToString());
                Msg.Gen().Set(Msg.TO, "Debug").
                   Set(Msg.ACT, "log")
                   .Set(Msg.MSG, "UnDeserializableException").Pool();
                throw new UnDeserializableException();
            }

            obj.Value.Set("From", "" + NetowrkUtil.GetOwnIP()).Pool();
            Msg.Gen().Set(Msg.TO, "Debug").Set(Msg.ACT, "log")
                .Set(Msg.MSG, "receice ok:" + obj.Value.ToJson()).Pool();
            DebugLog.Log.Print("test1 => " + obj.Value.ToJson());
            return true;
        }
    }
}