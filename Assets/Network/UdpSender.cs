using UnityEngine;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace BasicExtends {
    public class UdpSender: Singleton<UdpSender>, ISender {
        IConnectionData mData = null;
        bool mIsSetuped = false;

        public UdpSender () {
            mData = new ConnectionData();
            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Match("Network", "true")) {
                    Debug.Log("msg=" + msg.ToJson());
                    Send(msg);
                    msg.Set("Network", "false");
                    return;
                }
                if (msg.Unmatch("to", "Sender")) { return; }
                if (msg.Match("act", "Setup")) {
                    var adrs_r = msg.TryGet("adrs_r");
                    Debug.Log("adrs_r" + adrs_r);
                    Setup(adrs_r);
                    return;
                }
            });
        }

        public void Setup ( string adrs_r ) {
            mData.Setup(new ConectionThread(), ClientType.Sender, adrs_r);
            mData.ConnectThread.LaunchThread(mData, SendLoop);
            Msg.Gen().To("Manager")
                .As("NetworkManager")
                .Set("type", "SenderSetup")
                .Set("result", "Success").Pool();
            mIsSetuped = true;
        }

        private void SendLoop () {


            var sender = new IPEndPoint(IPAddress.Any, 8010);

            byte [] buffer = mData.DataQueue.MsgToByte.Dequeue();


            if (buffer.Length < 1) {
                System.Threading.Thread.Sleep(100);
                return;
            }

            Debug.Log("mData.IsReceiver" + mData.IsReceiver);
            var client = new UdpClient(sender);      // ローカルポート番号を指定。
            client.Send(buffer, buffer.Length, mData.Receiver);  // 同期処理なので、送信し終わるまで処理が止まる。

#if false
            byte [] buffer = mData.DataQueue.MsgToByte.Dequeue();
            if (buffer.Length < 1) {
                System.Threading.Thread.Sleep(5);
                return;
            }

            Msg.Gen().To("Manager")
                .As("NetworkManager")
                .Set("type", "SendLoop")
                .Set("sendsize", "" + buffer.Length)
                .Set("result", "Success").Pool();

            mData.Counter.Increment();
            //mData.Client.Connect(mData.Receiver);  
            Debug.Log("send");
            mData.Client.Send(buffer, buffer.Length, mData.Receiver);
            //mData.Client.Close();
#endif
        }

        private void Send ( Msg message ) {
            message = message
                .Set("Id", "" + mData.Counter.Get());
            Msg.Gen().To("Manager")
                .As("NetworkManager")
                .Set("type", "Sender@Send")
                .Set("Msg", message.ToJson())
                .Set("result", "Success").Pool();
            if (mIsSetuped == false) { return; }
            mData.DataQueue.MsgToByte.Enqueue(message);
        }

        public void Close () {
            mData.Client.Close();
            mData.ConnectThread.ThreadStop();
        }
    }
}
