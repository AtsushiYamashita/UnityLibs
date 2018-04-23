using UnityEngine;
using System.Net;
using System.Net.Sockets;

namespace BasicExtends {
    public class UdpSender: Singleton<UdpSender>, ISender {

        public UdpClient mSendClient = null;
        private LoopThread mLoop;
        private IPEndPoint mSendTo;
        private ThreadsafeCounter mSendId = new ThreadsafeCounter();
        private SafeAccessList<byte[]> mMsgList = new SafeAccessList<byte[]>();

        bool mIsSetuped = false;

        public UdpSender () {
            mLoop = new LoopThread();
            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Match("Network", "true")) {
                    msg.Set("Network", "false");
                    //Debug.Log("send msg=" + msg.ToJson());
                    SendStack(msg);
                    return;
                }
                if (msg.Unmatch("to", "Sender")) { return; }
                if (msg.Match("act", "Setup")) {
                    var adrs_r = msg.TryGet("adrs_r");
                    Setup(adrs_r);
                    return;
                }
            });
        }

        public void Setup ( string adrs_r ) {
            mSendTo = new IPEndPoint(IPAddress.Parse(adrs_r), NetworkUnit.DEFAULT_PORT_R);
            mSendClient = new UdpClient(new IPEndPoint(IPAddress.Any, NetworkUnit.DEFAULT_PORT_S));
            mLoop.AddContinueableCheck(() => { return mSendClient != null; })
                .LaunchThread(SendLoop);
            Msg.Gen().To("Manager")
                .As("NetworkManager")
                .Set("type", "SenderSetup")
                .Set("result", "Success").Pool();
            mIsSetuped = true;
        }



        private void SendLoop () {
            byte [] buffer = mMsgList.Pop();
            if (buffer == null || buffer.Length < 1) {

                System.Threading.Thread.Sleep(NetworkUnit.INTERVAL);
                return;
            }
            mSendClient.Send(buffer, buffer.Length, mSendTo);
            Msg.Gen().To("Manager")
                .As("NetworkManager")
                .Set("type", "Sender@SendLoop")
                .Set("result", "Success").Pool();
        }

        private void SendStack ( Msg message ) {
            message = message
                .Set("Id", "" + mSendId.Get());
            mSendId.Increment();
            Msg.Gen().To("Manager")
                .As("NetworkManager")
                .Set("type", "Sender@Send")
                .Set("Msg", message.ToJson())
                .Set("StackCount", mMsgList.Count())
                .Set("result", "Success").Push();
            if (mIsSetuped == false) { return; }
            mMsgList.Add(BinarySerial.Serialize(message));
        }

        public void Close () {
            mSendClient.Close();
            mLoop.ThreadStop();
        }
    }
}
