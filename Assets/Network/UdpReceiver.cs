using UnityEngine;
using System;
using System.Net;
using System.Text;

namespace BasicExtends {

    [Serializable]
    public class UdpReceiver: Singleton<UdpReceiver>,IReceiver {
        IConnectionData mData = null;

        private UdpReceiver () {
            mData = new ConnectionData();
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
                mData.Setup(new ConectionThread(), ClientType.Receiver);
            } catch (Exception e) {
                Msg.Gen().To("Manager").As("NetworkManager")
                    .Set("type", "StartServer")
                    .Set("result", "Fail")
                    .Set("msg", e.ToString())
                    .Push();
                return false;
            }

            mData.IsConnected = true;
            Msg.Gen().To("Manager").As("NetworkManager")
                .Set("type", "StartServer")
                .Set("result", "Success").Push();
            return mData.ConnectThread.LaunchThread(mData,ReceiveLoop);
        }

        public void StopServer () {
            mData.ConnectThread.ThreadStop();
        }

        /// <summary>
        /// 受信と待機処理
        /// スレッド側で実行させる
        /// </summary>
        public void ReceiveLoop () {
            
            var sender = mData.Sender;
            var buffer = mData.Client.Receive(ref sender);

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
            mData.Client.Close();
            mData.ConnectThread.ThreadStop();
        }
    }
}