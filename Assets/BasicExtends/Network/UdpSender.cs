﻿using UnityEngine;
using System.Net;

namespace BasicExtends {
    public class UdpSender: Singleton<UdpSender>, ISender {
        ConnectionData mData = null;

        bool mIsSetuped = false;

        public UdpSender () {
            mData = new ConnectionData();
            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Match("Network", "true")) {
                    msg.Set("Network", "false");
                    //Debug.Log("send msg=" + msg.ToJson());
                    Send(msg);
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
            mData.Setup(new LoopThread(), ClientType.Sender, adrs_r);
            mData.ConnectThread
                .AddContinueableCheck(()=> { return mData.Client != null; })
                .LaunchThread( SendLoop);
            Msg.Gen().To("Manager")
                .As("NetworkManager")
                .Set("type", "SenderSetup")
                .Set("result", "Success").Pool();
            mIsSetuped = true;
        }

        private void SendLoop () {
            byte [] buffer = null; //mData.DataQueue.MsgToByte.Dequeue();
            if (buffer.Length < 1) {
                System.Threading.Thread.Sleep(NetworkUnit.INTERVAL);
                return;
            }
            mData.Client.Send(buffer, buffer.Length, mData.Receiver);
            mData.Counter.Increment();
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
            // mData.DataQueue.MsgToByte.Enqueue(BinarySerial.Serialize(message));
        }

        public void Close () {
            mData.Client.Close();
            mData.ConnectThread.ThreadStop();
        }
    }
}
