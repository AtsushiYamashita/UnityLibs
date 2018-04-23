using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BasicExtends {

    [Serializable]
    public class UdpReceiver: Singleton<UdpReceiver>,IReceiver {
        LoopThread mLoop;
        UdpClient mClient;

        private UdpReceiver () {
            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Unmatch("to", "Sender")) { return; }
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
                    NetworkUnit.GetLocalIPAddress()), NetworkUnit.DEFAULT_PORT_R));
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
            var sender = new IPEndPoint(IPAddress.Any, NetworkUnit.DEFAULT_PORT_S);
            var buffer = mClient.Receive(ref sender);

            // Receive イベント を実行
            OnRecieve(buffer, sender);
        }

        private static bool OnRecieve ( byte [] receved, IPEndPoint sender ) {
            if (receved.Length < 1) { return false; }
            var msg = BinarySerial.Deserialize<Msg>(receved);
            msg.Set("From", "" + sender.Address.MapToIPv4());
            Debug.Log(msg.ToJson());
            //            JsonNode json = JsonNode.Parse(str);
            //            var m = Msg.Gen().Set("From", "" + sender.Address.MapToIPv4());
            //           var keys = new string [] { "To", "As", "Act", "Id", "Msg", "Data" };
            //            foreach(var k in keys) {
            //    var uk = k.ToUpper();
            //    var v = json [uk].Get<string>();
            //    m.Set(uk, v);
            //}
            msg.Pool();
            return true;
        }

        public void Close () {
            mClient.Close();
            mLoop.ThreadStop();
        }
    }
}