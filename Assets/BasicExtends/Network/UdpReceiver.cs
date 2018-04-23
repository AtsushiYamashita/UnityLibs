using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BasicExtends {

    [Serializable]
    public class UdpReceiver : MonoBehaviour {
        LoopThread mLoop;
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

            try {
                mLoop = new LoopThread();
                mClient = new UdpClient(new IPEndPoint(IPAddress.Parse(
                    NetworkUnit.GetLocalIPAddress()),mObservePort ));
            } catch (Exception e) {
                Msg.Gen().To("Manager").As("NetworkManager")
                    .Set("type", "StartServer")
                    .Set("result", "Fail")
                    .Set("msg", e.ToString())
                    .Push();
                return false;
            }

            Msg.Gen().To("Manager").As("NetworkManager")
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
            var sender = new IPEndPoint(IPAddress.Any, 0);
            var buffer = mClient.Receive(ref sender);

            // Receive イベント を実行
            OnRecieve(buffer, sender);
        }

        private static bool OnRecieve ( byte [] receved, IPEndPoint sender ) {
            if (receved.Length < 1) { return false; }
            var msg = BinarySerial.Deserialize<Msg>(receved);
            msg.Set("From", "" + sender.Address.MapToIPv4()).Pool();
            Debug.Log(msg.ToJson());
            return true;
        }

        public void Close () {
            mClient.Close();
            mLoop.ThreadStop();
        }
    }
}