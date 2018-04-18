using System;
using System.Threading;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

namespace BasicExtends {
    public interface INetConnectThread {
        bool LaunchThread ( IConnectionData data, Action action );
        bool IsLooping { set; get; }
        void ThreadStop ();
    }

    public class ConectionThread: INetConnectThread {
        protected bool mThreadLoop = false;
        protected Thread mThread = null;
        protected IConnectionData mData = null;
        public bool IsLooping { set; get; }
        private Action mLoopAction;

        private void ThreadLoop (  ) {
            mData.ConnectThread.IsLooping = true;
            mData.IsConnected = true;
            while (mData.ConnectThread.IsLooping && mData.IsConnected) {
                mData.ClientLifeCheck();
                try {
                    mLoopAction();
                    Thread.Sleep(1);
                } catch (Exception e) {

                    Msg.Gen().To("Manager")
                .As("NetworkManager")
                .Set("type", "ThreadLoop2")
                .Set("state", e.ToString())
                .Set("result", "Fail").Pool();
                    return;
                }
            }
        }

        /// <summary>
        /// スレッド起動関数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool LaunchThread ( IConnectionData data, Action action ) {
            mData = data;
            mLoopAction = action;
            try {
                // Dispatch用のスレッド起動.
                mThreadLoop = true;

                mThread = new Thread(ThreadLoop);
                mThread.Start();
            } catch {
                Debug.Log("Cannot launch thread.");
                Msg.Gen().To("Manager").As("NetworkManager")
                    .Set("type", "LaunchThread")
                    .Set("result", "Fail").Pool();
                return false;
            }
            return true;
        }


        public void ThreadStop () {
            mThreadLoop = false;
            if (mThread == null) { return; }
            mThread.Join();
            mThread = null;
            mData.Disconnect();
            Msg.Gen().To("Manager").As("NetworkManager")
                .Set("type", "ThreadStop")
                .Set("result", "Success").Pool();
        }
    }

    public interface IReceiver {
        bool StartServer ();
        void StopServer ();
        void Close ();
    }

    public interface ISender {
        void Setup ( string ip_r );
        void Close ();
    }

    public enum ClientType {
        Sender, Receiver
    }

    public interface IConnectionData {
        IPEndPoint Receiver { get; }
        IPEndPoint Sender { get; }
        UdpClient Client { get; }
        Counter Counter { get; }

        bool IsReceiver { get; }
        bool IsConnected { set; get; }

        PacketQueue DataQueue { set; get; }
        INetConnectThread ConnectThread { set; get; }

        void Setup ( INetConnectThread connector, ClientType type, string ip_r = "" );
        void ClientLifeCheck ();
        void Disconnect ();
    }

    public static class Size {
        public const int KB = 1000, MB = 1000 * KB;
    }

    public class Counter {
        private int mCounter = 0;
        private System.Object mLock = new System.Object();
        public Counter Increment () {
            lock (mLock) {
                mCounter++;
            }
            return this;
        }
        public int Get () {            
            return mCounter;
        }
    }

    public class ConnectionData: IConnectionData {
        public static int sBufferSize = 20 * Size.KB;
        public static int sCallInterval = 5;
        public const int DEFAULT_PORT_S = 8000;
        public const int DEFAULT_PORT_R = 8001;

        public IPEndPoint Receiver { private set; get; }
        public IPEndPoint Sender { private set; get; }
        public UdpClient Client { private set; get; }
        public Counter Counter { private set; get; }

        public bool IsReceiver { private set; get; }
        public bool IsConnected { set; get; }

        public PacketQueue DataQueue { set; get; }
        public INetConnectThread ConnectThread { set; get; }

        public ConnectionData () {
            DataQueue = new PacketQueue();
            Counter = new Counter();
        }

        public void Setup ( INetConnectThread connector, ClientType type, string ip_r = "" ) {
            ConnectThread = connector;
            IsReceiver = type == ClientType.Receiver;
            var address = ip_r.Length > 0 ? IPAddress.Parse(ip_r) : IPAddress.Any;
            //Receiver = new IPEndPoint(address, DEFAULT_PORT_R);

            Receiver = new IPEndPoint(
                    IPAddress.Parse("192.168.100.110"),
                    8011);
            Sender = new IPEndPoint( IPAddress.Any, DEFAULT_PORT_S);
            Client = new UdpClient(IsReceiver ? Receiver : Sender);
        }

        public void Disconnect () {
            if (IsConnected == false) { return; }
            IsConnected = false;
            Msg.Gen().To("Manager").As("NetworkManager")
                .Set("type", "Disconnect")
                .Set("result", "Success").Push();
        }

        public void ClientLifeCheck () {
            if (Client != null) { return; }
            Msg.Gen().To("Manager").As("NetworkManager")
                .Set("type", "ReceiveLoop@client_dead")
                .Set("result", "Fail").Pool();
            var msg = "Error,client is dead.";
            throw new Exception(msg);
        }

        ~ConnectionData () {
            ConnectThread.ThreadStop();
            IsConnected = false;
        }
    }
}
