using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine;

namespace BasicExtends {
    public class ConnectionData: IConnectionData {
        public static int sBufferSize = 20 * NetworkUnit.KB;

        public IPEndPoint Receiver { private set; get; }
        public IPEndPoint Sender { private set; get; }
        public UdpClient Client { private set; get; }
        public ThreadsafeCounter Counter { private set; get; }

        public bool IsReceiver { private set; get; }
        public bool IsConnected { set; get; }

        public PacketQueue DataQueue { set; get; }
        public INetConnectThread ConnectThread { set; get; }

        public ConnectionData () {
            DataQueue = new PacketQueue();
            Counter = new ThreadsafeCounter();
        }

        public void Setup ( INetConnectThread connector, ClientType type, string ip_r = "" ) {
            ConnectThread = connector;
            IsReceiver = type == ClientType.Receiver;
            var address = IPAddress.Parse(IsReceiver ? NetworkUnit.GetLocalIPAddress() : ip_r);
            Receiver = new IPEndPoint(address, NetworkUnit.DEFAULT_PORT_R);
            Sender = new IPEndPoint(IPAddress.Any, NetworkUnit.DEFAULT_PORT_S);
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